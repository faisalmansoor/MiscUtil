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
                .Case(Colors.Green, () => Console.WriteLine("Green"))
                .Execute();
        }

        [Test]
        public void Cases()
        {
            new EnumSwitch<Colors>(Colors.Red)
                .Case(new[] { Colors.Red, Colors.Blue }, () => Console.WriteLine("Red or Blue"))
                .Case(Colors.Green, () => Console.WriteLine("Green"))
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
    }
}