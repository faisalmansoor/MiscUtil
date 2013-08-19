using System;
using System.ComponentModel;
using System.IO;
using System.Xml.Serialization;

namespace DynamicDefaultValue
{
    public class Configuration
    {
        [DefaultValue(@"C:\Database")]
        public string DatabasePath = @"C:\Database";

        [DynamicDefaultValue("TEMP_PATH")]
        public string TempPath = Path.GetTempPath();
    }

    class Program
    {
        static void Main(string[] args)
        {
            DynamicDefaultValueAttribute.AddOrUpdate("TEMP_PATH", Path.GetTempPath());

            var serializer = new XmlSerializer(typeof(Configuration));
            var config = new Configuration { DatabasePath = @"C:\Database", TempPath = Path.GetTempPath() };

            // Should not print any thing. All values are set to default.
            serializer.Serialize(Console.Out, config);


            Console.Write("\n\n");

            config.TempPath = @"C:\Temp";
            serializer.Serialize(Console.Out, config);
        }
    }
}
