using Dapper;
using SmartVault.DataGeneration.Utils;
using SmartVault.Program.BusinessObjects;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Reflection;
using Document = SmartVault.Program.BusinessObjects.Document;

namespace SmartVault.DataGeneration
{
    public class DataSeedService : IDisposable
    {

        private List<User>? _userData;
        private List<Account>? _accountData;
        private List<Document>? _documentData;
        private DataSeedConfiguration _dataSeedConfiguration;
        private readonly string _path;

        public List<Document> GetDocuments => _documentData ?? (_documentData = new List<Document>());
        public List<User> GetUsers => _userData ?? (_userData = new List<User>());
        public List<Account> GetAccounts => _accountData ?? (_accountData = new List<Account>());

        public DataSeedService(DataSeedConfiguration dataSeedConfiguration)
        {
            _dataSeedConfiguration = dataSeedConfiguration;
            _userData = new List<User>();
            _accountData = new List<Account>();
            _documentData = new List<Document>();
            _path = Directory.GetParent(Assembly.GetExecutingAssembly().Location) + "\\files";
        }


        public void ExecuteDDLScripts(string script)
        {
            using (IDbConnection connection = new SQLiteConnection(_dataSeedConfiguration.ConnectionString))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        connection.Execute(script);
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }
        public void ExecuteDMLScripts(bool printResult = false)
        {
            using (IDbConnection connection = new SQLiteConnection(_dataSeedConfiguration.ConnectionString))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        connection.Execute("INSERT INTO User " +
                            "(Id, FirstName, LastName, DateOfBirth, AccountId, Username, Password, CreatedDate) " +
                            "VALUES (@Id, @FirstName, @LastName, @DateOfBirth, @AccountId, @Username, @Password, @CreatedDate)", _userData, transaction);
                        connection.Execute($"INSERT INTO Account (Id, Name, CreatedDate) VALUES( @Id, @Name, @CreatedDate)", _accountData, transaction);

                        connection.Execute(
                                    "INSERT INTO Document " +
                                    "(Id, Name, FilePath, Length, AccountId, CreatedDate) " +
                                    "VALUES (@Id, @Name, @FilePath, @Length, @AccountId, @CreatedDate)", _documentData, transaction);

                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        Console.WriteLine($"Error DML SCRIPS: {ex.Message}");
                        throw;
                    }
                }
                if(printResult)
                    ConsoleUtils.PrintSeedingInformation(connection);
            }
        }
        public void GenerateLocalDocuments()
        {
            foreach (var item in _documentData!)
            {
                CreateFileData(item.Name);
            }
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
        public Document GetDocumentData(int id, int userId, int docCounter)
        {
            var document =  new Document
            {
                Id = id,
                Name = Constants.DOCUMENT + userId + Constants.HYPHEN + docCounter + Constants.EXTENSION,
                Length = _dataSeedConfiguration.DocumentFileLenght,
                AccountId = userId,
            };
            document.FilePath = Path.Combine(_path, document.Name);
            return document;
        }
        public User GetUserData(int id)
        {
            string firstName = Constants.FNAME + id;
            string lastName = Constants.LNAME + id;
            DateTime dateOfBirth = ContentGeneration.RandomDay();
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
        public void CreateFileData(string name)
        {
            string filePath = Path.Combine(_path, name);

            if (!Directory.Exists(_path))
            {
                Directory.CreateDirectory(_path);
            }
            File.WriteAllText(filePath, _dataSeedConfiguration.Content);
        }
        public Account GetAccount(int id)
        {
            return new Account
            {
                Id = id,
                Name = Constants.ACCOUNT + id,
                CreatedDate = DateTime.UtcNow
            };
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
