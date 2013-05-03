using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using Newtonsoft.Json;
using NUnit.Framework;
using Formatting = Newtonsoft.Json.Formatting;

namespace BeyondCompareLauncher
{
    public class BeyondCompare
    {
        public static void Files(string left, string right)
        {
            string bcomparePath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86),
                @"Beyond Compare 3\BCompare.exe");

            if (File.Exists(bcomparePath))
            {
                left = Path.GetFullPath(left);
                right = Path.GetFullPath(right);
                string args = String.Format(@"/fv=""Text Compare"" ""{0}"" ""{1}""", left, right);
                Process.Start(bcomparePath, args);
            }
        }

        public static void Text(string left, string right)
        {
            string leftFile = Path.Combine(Path.GetTempPath(), Path.GetTempFileName());
            string rightFile = Path.Combine(Path.GetTempPath(), Path.GetTempFileName());

            File.WriteAllText(leftFile, left);
            File.WriteAllText(rightFile, right);

            Files(leftFile, rightFile);
        }

        public static void Objects(object left, object right)
        {
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
                NullValueHandling = NullValueHandling.Include,
                PreserveReferencesHandling = PreserveReferencesHandling.Objects,
                DateFormatHandling = DateFormatHandling.MicrosoftDateFormat,
                Formatting = Formatting.Indented,
                TypeNameHandling = TypeNameHandling.All
            };

            string leftObj = JsonConvert.SerializeObject(left, settings);
            string rightObj = JsonConvert.SerializeObject(right, settings);

            Text(leftObj, rightObj);
        }
    }

    [Ignore]
    [TestFixture]
    public class BeyondCompareTests
    {
        [Test]
        public void Files()
        {
            string left = Path.Combine(Path.GetTempPath(), Path.GetTempFileName());
            string right = Path.Combine(Path.GetTempPath(), Path.GetTempFileName());

            File.WriteAllText(left, "This is left file");
            File.WriteAllText(right, "This is right file");

            BeyondCompare.Files(left, right);
        }

        [Test]
        public void Text()
        {
            BeyondCompare.Text("this is left text", "this is right text");
        }

        [Test]
        public void Objects()
        {
            BeyondCompare.Objects(Thread.CurrentThread, Thread.CurrentThread);
        }
    }
}