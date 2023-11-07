using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace SmartVault.Program
{
    partial class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                return;
            }

            WriteEveryThirdFileToFile(args[0]);
            GetAllFileSizes();
            Console.ReadLine();
        }

        private static void GetAllFileSizes()
        {
            string path = Directory.GetParent(Assembly.GetExecutingAssembly().Location) + "\\files";
            long totalsize = GetDirectorySize(path);
            Console.WriteLine($"Total Size of Files: {totalsize} bytes");
        }

        private static void WriteEveryThirdFileToFile(string accountId)
        {
            // TODO: Implement functionality
        }

        static long GetDirectorySize(string path)
        {
            return Directory.GetFiles(path, "*", SearchOption.AllDirectories)
                                       .Select(file => new FileInfo(file).Length)
                                       .Sum();
        }
    }
}