using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

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
            string directoryPath = Path.Combine(
            Directory.GetParent(Assembly.GetExecutingAssembly().Location)!.FullName,
            "files");
            if (Directory.Exists(directoryPath))
            {
                string[] txtFiles = Directory.GetFiles(directoryPath, "*.txt");

                var filesWithIdUser = txtFiles.Where(filePath => Path.GetFileName(filePath).Contains($"Document{accountId}-")).ToList();
                var sortedFiles = filesWithIdUser.OrderBy(file => ExtractNumberFromFileName(file));
                var thirdFiles = filesWithIdUser.Where((file, index) => index % 3 == 0);

                StringBuilder sb = new StringBuilder();

                Parallel.ForEach(thirdFiles, (file) =>
                {
                    string fileContent = File.ReadAllText(file);
                    if (fileContent.Contains("Smith Property"))
                    {
                        sb.Append(Environment.NewLine+fileContent);
                    }
                });

                File.WriteAllText(Path.Combine(directoryPath, $"Document{accountId}-Resume.txt"), sb.ToString());
            }

        }

        static int ExtractNumberFromFileName(string fileName)
        {
            int start = fileName.LastIndexOf('-') + 1;
            int end = fileName.LastIndexOf('.');

            if (start >= 0 && end >= 0)
            {
                string numberPart = fileName.Substring(start, end - start);
                if (int.TryParse(numberPart, out int number))
                {
                    return number;
                }
            }
            return -1;
        }


        static long GetDirectorySize(string path)
        {
            return Directory.GetFiles(path, "*", SearchOption.AllDirectories)
                                       .Select(file => new FileInfo(file).Length)
                                       .Sum();
        }
    }
}