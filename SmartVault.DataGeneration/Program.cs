using Microsoft.Extensions.Configuration;
using SmartVault.Library;
using SmartVault.Program.BusinessObjects;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using static System.Net.WebRequestMethods;

namespace SmartVault.DataGeneration
{
    partial class Program
    {
        static void Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json").Build();

            SQLiteConnection.CreateFile(configuration["DatabaseFileName"]);
            string connectionString = string.Format(configuration?["ConnectionStrings:DefaultConnection"] ?? "", configuration?["DatabaseFileName"]);
            GenerateDataSeed(connectionString);
            
            Console.ReadLine();
        }

        private static void GenerateDataSeed(string connectionString)
        {
            string[] files = Directory.GetFiles(@"..\..\..\BusinessObjectSchema");
            var document = new FileInfo("files\\TestDoc.txt");
            List<Document> documents = new List<Document>();
            using (var dataSeed = new DataSeedService(
                new DataSeedConfiguration
                {
                    ConnectionString = connectionString,
                    BulkUsers = 100,
                    BulkDocuments = 10000,
                    DocumentFileLenght = document.Length,
                    DocumentFilePath = document.FullName
                }))
            {
                dataSeed.GenerateData();
                documents = dataSeed.GetDocuments;
                Parallel.Invoke(
                    () =>
                    {
                        dataSeed.ExecuteDDLScripts(GetAllBussinessScript(files));
                        dataSeed.ExecuteDMLScripts();
                    },
                    () =>
                    {
                        dataSeed.GenerateLocalDocuments();
                    }
                );
                
            }

        }

        private static string GetAllBussinessScript(string[] files)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < files.Length; i++)
            {
                var serializer = new XmlSerializer(typeof(BusinessObject));
                var businessObject = serializer.Deserialize(new StreamReader(files[i])) as BusinessObject;
                sb.Append(businessObject?.Script + Environment.NewLine);
            }
            return sb.ToString();
        }

    }
}