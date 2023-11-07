using Moq;
using SmartVault.DataGeneration;
using System.Data;
using NUnit.Framework;
using Dapper;
using Moq.Dapper;
using System.Data.SQLite;
using System.Reflection;

namespace SmartVault.Test
{
    [TestFixture]
    public class Tests
    {
        private string _connectionString;
        private string _testDoc;

        [SetUp]
        public void Setup()
        {
            _connectionString = "data source=unitTests.sqlite";
            string path = Directory.GetParent(Assembly.GetExecutingAssembly().Location) + "\\files";
            string filePath = Path.Combine(path, "TestDoc.txt");
        }

        [Test]
        public void ExecuteDDLScripts_ShouldExecuteScriptsSuccessfully()
        {
            // Configura una base de datos SQLite en memoria
            using (IDbConnection connection = new SQLiteConnection("DataSource=:memory:"))
            {
                connection.Open();

                // Crea una tabla de prueba para simular el script
                connection.Execute("CREATE TABLE TestTable (Id INT, Name TEXT)");

                // Instancia DataSeedService
                var documentInfo = new FileInfo("CREATE TABLE TestTable (Id INT, Name TEXT)");
                var dataSeedService = new DataSeedService(new DataSeedConfiguration
                {
                    ConnectionString = _connectionString,
                    BulkUsers = 1,
                    BulkDocuments = 5,
                    DocumentFileLenght = 256,
                    DocumentFilePath = "myTestDoc.txt"
                });

                // Scripts de prueba
                var scripts = "INSERT INTO TestTable (Id, Name) VALUES (1, 'Test1')";

                // Ejecuta el método y verifica si se ejecuta el script
                int executedScript = dataSeedService.ExecuteDDLScripts(scripts);

                // Verifica que se haya ejecutado el script con éxito
                Assert.AreEqual(1, executedScript);
            }
        }

    }
}