using System;
using NUnit.Framework;

namespace EnumSwitch
{
    public enum Colors
    {
        Red,
        Blue,
        Green
    }

    [TestFixture]
    public class EnumSwitchTests
    {
        [Test]
        public void Case()
        {
            new EnumSwitch<Colors>(Colors.Red)
                .Case(Colors.Blue, () => Console.WriteLine("Blue"))
                .Case(Colors.Red, () => Console.WriteLine("Red"))
                .Ignore(Colors.Green)
                .Execute();
        }

        [Test]
        public void Cases()
        {
            new EnumSwitch<Colors>(Colors.Red)
                .Case(new[]
                          {
                              Colors.Red, 
                              Colors.Blue
                          }, 
                          () => Console.WriteLine("Red or Blue"))
                .Case(Colors.Green, 
                () => Console.WriteLine("Green"))
                .Execute();
        }

        [Test]
        public void ExecuteMultipleTimes()
        {
            var eSwitch = new EnumSwitch<Colors>()
                .Case(new[] { Colors.Red, Colors.Blue }, () => Console.WriteLine("Red or Blue"))
                .Case(Colors.Green, () => Console.WriteLine("Green"));

            eSwitch.Execute(Colors.Red);
            eSwitch.Execute(Colors.Blue);
        }

        [Test]
        public void Ignore()
        {
            new EnumSwitch<Colors>(Colors.Red)
                .Case(Colors.Blue, () => Console.WriteLine("Blue"))
                .Ignore(Colors.Red, Colors.Green)
                .Execute();
        }

        [Test]
        public void SwitchOnNotProvided()
        {
            Exception ex = Assert.Throws<InvalidOperationException>(
                () =>
                new EnumSwitch<Colors>().Execute()
                );
            Assert.AreEqual("SwitchOn not provided, either provide SwitchOn in constructor or use Execute(switchOn)", ex.Message);
        }

        [Test]
        public void InvalidTypeArgument()
        {
            Exception ex = Assert.Throws<ArgumentException>(
                () =>
                new EnumSwitch<int>(10).Execute(10)
                );
            Assert.AreEqual("Type argument System.Int32 must be an enum", ex.Message);
        }

        [Test]
        public void DuplicateCaseLabel()
        {
            Exception ex = Assert.Throws(
                typeof(ArgumentException),
                () => new EnumSwitch<Colors>(Colors.Red)
                          .Case(Colors.Blue, () => Console.WriteLine("Blue"))
                          .Case(Colors.Blue, () => Console.WriteLine("Blue Again"))
                          .Case(Colors.Green, () => Console.WriteLine("Green"))
                          .Execute());

            Assert.AreEqual("Duplicate case label Blue\r\nParameter name: option", ex.Message);
        }

        [Test]
        public void UnhandledCase()
        {
            Exception ex = Assert.Throws(
                typeof(NotImplementedException),
                () => new EnumSwitch<Colors>(Colors.Red)
                          .Case(Colors.Blue, () => Console.WriteLine("Blue"))
                          .Case(Colors.Green, () => Console.WriteLine("Green"))
                          .Execute());

            Assert.AreEqual(ex.Message, "Switch statement didn't handle cases: Red");
        }

        [Test]
        public void HanldDefault()
        {
            var color = Colors.Red;
            new EnumSwitch<Colors>(color)
                .Case(Colors.Blue, () => Console.WriteLine("Blue"))
                .Case(Colors.Green, () => Console.WriteLine("Green"))
                .Default(() =>
                               {
                                   Console.WriteLine("Color not handled: {0}", color);
                                   return true;
                               })
                .Execute();
        }

        [Test]
        public void HanldDefaultButLetEnumSwitchThrowException()
        {
            var color = Colors.Red;
            Exception ex = Assert.Throws(
                typeof (NotImplementedException),
                () => new EnumSwitch<Colors>(Colors.Red)
                          .Case(Colors.Blue, () => Console.WriteLine("Blue"))
                          .Case(Colors.Green, () => Console.WriteLine("Green"))
                          .Default(() =>
                                         {
                                             Console.WriteLine("Color not handled: {0}", color);
                                             return false;
                                         })
                          .Execute());

            Assert.AreEqual(ex.Message, "Switch statement didn't handle cases: Red");
        }

        [Test]
        public void Performance()
        {
            Array colors = Enum.GetValues(typeof(Colors));
            int length = colors.Length;

            var start = DateTime.Now;
            for(int i=0; i<10000; i++)
            {
                PrintColorUsingSwitch((Colors)colors.GetValue(i%length));
            }

            Console.WriteLine("PrintColorUsingSwitch took: {0}", DateTime.Now - start);

            start = DateTime.Now;
            for (int i = 0; i < 10000; i++)
            {
                PrintColorUsingEnumSwitch((Colors)colors.GetValue(i % length));
            }

            Console.WriteLine("PrintColorUsingEnumSwitch took: {0}", DateTime.Now - start);
        }

        public string PrintColorUsingEnumSwitch(Colors color)
        {
            string str = null;
            new EnumSwitch<Colors>(color)
                .Case(Colors.Red, () => str = "Red")
                .Case(Colors.Blue, () => str = "Blue")
                .Case(Colors.Green, () => str = "Green")
                .Execute();
            return str;
        }

        public string PrintColorUsingCachedEnumSwitch(Colors color)
        {
            string str = null;
            new EnumSwitch<Colors>()
                .Case(Colors.Red, () => str = "Red")
                .Case(Colors.Blue, () => str = "Blue")
                .Case(Colors.Green, () => str = "Green")
                .Execute(color);
            
            return str;
        }

        public string PrintColorUsingSwitch(Colors color)
        {
            string str = null;
            switch (color)
            {
                case Colors.Red:
                    return "Red";
                    break;
                case Colors.Blue:
                    return  "Blue";
                    break;
                case Colors.Green:
                    return  "Green";
                    break;
                default:
                    throw new ArgumentOutOfRangeException("color");
            }
            return str;
        }
    }
}