using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace EnumFiles
{
    class Program
    {
        static void Main(string[] args)
        {
            foreach (string name in GetFiles(@"C:\Temp\Test"))
            {
                Console.WriteLine(name);
            }
        }
        
        public static IEnumerable<string> GetFiles(string root, string searchPattern = "*")
        {
            var files = new List<string>();

            try
            {
                foreach (var dir in Directory.GetDirectories(root))
                {
                    try
                    {
                        var foundFIles = Directory.GetFiles(dir, searchPattern, SearchOption.TopDirectoryOnly).ToList();
                        files.AddRange(foundFIles);
                    }
                    catch(Exception ex)
                    {
                        Console.WriteLine("INFO: Cannot enumerate file in folder: {0}. Error: {1}", dir, ex.Message);
                        continue;
                    }

                    var recursiveFiles = GetFiles(dir, searchPattern);

                    if (recursiveFiles != null)
                    {
                        files = files.Concat(recursiveFiles).ToList();
                    }
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine("INFO: Cannot enumerate file in folder: {0}. Error: {1}", root, ex.Message);
            }

            return files;
        }
    }
}
