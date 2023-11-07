using Moq;
using SmartVault.DataGeneration;
using System.Data;
using NUnit.Framework;
using Dapper;
using Moq.Dapper;
using System.Data.SQLite;
using System.Reflection;
using SmartVault.Program.BusinessObjects;
using System.Reflection.Metadata;
using SmartVault.DataGeneration.Utils;
using System.Xml.Linq;
using System;

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

        }

        [Test]
        public void GetUserData_ReturnsValidUser()
        {
            var dataSeedService = new DataSeedService(new DataSeedConfiguration());
            int userId = 1; 
            User? user = dataSeedService.GetUserData(userId);
            Assert.IsNotNull(user);
            Assert.AreEqual(userId, user!.Id);
            Assert.AreEqual("FName1", user!.FirstName);

        }

        [Test]
        public void GetAccount_ReturnsValidAccount()
        {
            var dataSeedService = new DataSeedService(new DataSeedConfiguration());
            int accountId = 1;
            Account? account = dataSeedService.GetAccount(accountId);
            Assert.IsNotNull(account);
            Assert.AreEqual(accountId, account.Id);
            Assert.AreEqual("Account1", account.Name); 

        }

        [Test]
        public void RandomDay_GeneratesValidRandomDate()
        {
            DateTime randomDate = ContentGeneration.RandomDay();
            DateTime startDate = new DateTime(1985, 1, 1);
            DateTime endDate = DateTime.Today;
            Assert.GreaterOrEqual(randomDate, startDate);
            Assert.LessOrEqual(randomDate, endDate);
        }

        [Test]
        public void GenerateData_PopulatesUserDataAccountDataAndDocumentData()
        {
            var dataSeedConfiguration = new DataSeedConfiguration
            {
                BulkUsers = 5, 
                BulkDocuments = 3,

            };
            var dataSeedService = new DataSeedService(dataSeedConfiguration);

            dataSeedService.GenerateData();

            // Assert
            Assert.IsNotNull(dataSeedService.GetUsers);
            Assert.IsNotNull(dataSeedService.GetAccounts);
            Assert.IsNotNull(dataSeedService.GetDocuments);

            Assert.AreEqual(5, dataSeedService.GetUsers.Count); 
            Assert.AreEqual(5, dataSeedService.GetAccounts.Count); 
            Assert.AreEqual(5 * 3, dataSeedService.GetDocuments.Count);
        }

        [Test]
        public void GetDocumentData_ReturnsValidDocument()
        {
            // Arrange
            var dataSeedConfiguration = new DataSeedConfiguration
            {
                DocumentFileLenght = 1024
            };
            var dataSeedService = new DataSeedService(dataSeedConfiguration);
            int id = 1; // Puedes ajustar el ID según tus necesidades
            int userId = 2; // Puedes ajustar el usuario según tus necesidades
            int docCounter = 3; // Puedes ajustar el contador según tus necesidades

            Program.BusinessObjects.Document? document = dataSeedService.GetDocumentData(id, userId, docCounter);
            Assert.IsNotNull(document);
            Assert.AreEqual(id, document.Id);
            Assert.AreEqual("Document2-3.txt", document.Name);
            Assert.AreEqual(1024, document.Length);
            Assert.AreEqual(userId, document.AccountId);
        }
    }
}