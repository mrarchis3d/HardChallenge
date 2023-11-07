using Dapper;
using SmartVault.Library;
using SmartVault.Program.BusinessObjects;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using Document = SmartVault.Program.BusinessObjects.Document;

namespace SmartVault.DataGeneration
{
    public class DataSeedService : IDisposable
    {

        private List<User>? _userData;
        private List<Account>? _accountData;
        private List<Document>? _documentData;
        private DataSeedConfiguration _dataSeedConfiguration;



        public DataSeedService(DataSeedConfiguration dataSeedConfiguration)
        {
            _dataSeedConfiguration = dataSeedConfiguration;
            _userData = new List<User>();
            _accountData = new List<Account>();
            _documentData = new List<Document>();
        }
        public int ExecuteDDLScripts(string script)
        {
            int executedScript = 0;
            using (IDbConnection connection = new SQLiteConnection(_dataSeedConfiguration.ConnectionString))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        executedScript += connection.Execute(script);
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        Console.WriteLine($"Error DLL Scripts: {ex.Message}");
                    }
                }
            }
            return executedScript;
        }

        public int ExecuteDMLScripts()
        {
            int executedScript= 0;
            using (IDbConnection connection = new SQLiteConnection(_dataSeedConfiguration.ConnectionString))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        executedScript += connection.Execute("INSERT INTO User " +
                            "(Id, FirstName, LastName, DateOfBirth, AccountId, Username, Password, CreatedDate) " +
                            "VALUES (@Id, @FirstName, @LastName, @DateOfBirth, @AccountId, @Username, @Password, @CreatedDate)", _userData, transaction);
                        executedScript += connection.Execute($"INSERT INTO Account (Id, Name, CreatedDate) VALUES( @Id, @Name, @CreatedDate)", _accountData, transaction);

                        executedScript += connection.Execute(
                                    "INSERT INTO Document " +
                                    "(Id, Name, FilePath, Length, AccountId, CreatedDate) " +
                                    "VALUES (@Id, @Name, @FilePath, @Length, @AccountId, @CreatedDate)", _documentData, transaction);

                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        Console.WriteLine($"Error DML SCRIPS: {ex.Message}");
                    }
                }

                ConsoleUtils.PrintSeedingInformation(connection);
            }
            return executedScript;
        }

        public void GenerateData()
        {
            int userId = 0;
            int docId = 0;
            while (userId< _dataSeedConfiguration.BulkUsers)
            {
                _userData!.Add(GetUserData(userId));
                _accountData!.Add(GetAccount(userId));
                int docCounter = 0;
                while (docCounter < _dataSeedConfiguration.BulkDocuments)
                {
                    _documentData!.Add(GetDocumentData(docId, userId, docCounter));
                    docId++;
                    docCounter++;
                }
                userId++;
            }
        }
        private Document GetDocumentData(int id, int userId, int docCounter)
        {
            return new Document
            {
                Id = id,
                Name = Constants.DOCUMENT + userId + Constants.HYPHEN + docCounter + Constants.EXTENSION,
                FilePath = _dataSeedConfiguration.DocumentFilePath,
                Length = _dataSeedConfiguration.DocumentFileLenght,
                AccountId = userId,
            };
        }
        private User GetUserData(int id)
        {
            string firstName = Constants.FNAME + id;
            string lastName = Constants.LNAME + id;
            DateTime dateOfBirth = RandomDay();
            int accountId = id;
            string username = Constants.USERNAME + Constants.HYPHEN + id;
            string password = Constants.PASSWORD;
            return new User
            {
                Id = id,
                FirstName = firstName,
                LastName = lastName,
                DateOfBirth = dateOfBirth,
                AccountId = accountId,
                Username = username,
                Password = password,
                CreatedDate = DateTime.UtcNow
            };
        }
        private Account GetAccount(int id)
        {
            return new Account
            {
                Id = id,
                Name = Constants.ACCOUNT + id,
                CreatedDate = DateTime.UtcNow
            };
        }
        private DateTime RandomDay()
        {
            DateTime start = new DateTime(1985, 1, 1);
            Random gen = new Random();
            int range = (DateTime.Today - start).Days;
            while (true)
                return start.AddDays(gen.Next(range));
        }

        public void Dispose()
        {
            if (_userData != null)
            {
                _userData = null; 
            }

            if (_accountData != null)
            {
                _accountData = null;
            }

            if (_documentData != null)
            {
                _documentData = null;
            }

            if (_dataSeedConfiguration != null)
            {
                _dataSeedConfiguration = null; 
            }
        }
    }
}
