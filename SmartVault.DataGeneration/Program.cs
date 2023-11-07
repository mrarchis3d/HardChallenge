using Microsoft.Extensions.Configuration;
using SmartVault.DataGeneration.Utils;
using SmartVault.Library;
using SmartVault.Program.BusinessObjects;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

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

            //// This going to generate 100 users and 20 docs for each user, in local and database
            /// please modify what amount you want for both
            /// create localdocuments or not with the boolean
            GenerateDataSeed(connectionString, 20, 20, true);
            Console.ReadLine();
        }
        /// <summary>
        /// Generate data seed
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="bulkUser">amount of users to create</param>
        /// <param name="bulkDocs">amount of documents of each user</param>
        /// <param name="createLocalDocuments">create local documents</param>
        private static void GenerateDataSeed(string connectionString, int bulkUser, int bulkDocs, bool createLocalDocuments = false)
        {
            string[] files = Directory.GetFiles(@"..\..\..\BusinessObjectSchema");
            var content = ContentGeneration.GetContent(32);
            File.WriteAllText("files\\TestDoc.txt", content);
            var document = new FileInfo("files\\TestDoc.txt");
            List<Document> documents = new List<Document>();
            using (var dataSeed = new DataSeedService(
                new DataSeedConfiguration
                {
                    ConnectionString = connectionString,
                    BulkUsers = bulkUser,
                    BulkDocuments = bulkDocs,
                    Content = content,
                    DocumentFileLenght = document.Length,
                }))
            {
                dataSeed.GenerateData();
                documents = dataSeed.GetDocuments;
                Parallel.Invoke(
                    () =>
                    {
                        dataSeed.ExecuteDDLScripts( ScriptGeneration.GetAllBussinessScript(files));
                        dataSeed.ExecuteDMLScripts(true);
                        Console.WriteLine("Database operations finished");   
                    },
                    () =>
                    {
                        if (createLocalDocuments)
                        {
                            dataSeed.GenerateLocalDocuments();
                            Console.WriteLine("Local Files operations finished");
                        }else
                            Console.WriteLine("No local files");
                    }
                );
                
            }

        }

    }
}