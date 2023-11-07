using Dapper;
using Newtonsoft.Json;
using System;
using System.Data;

namespace SmartVault.DataGeneration
{
    public static class ConsoleUtils
    {
        public static void PrintSeedingInformation(IDbConnection connection)
        {

            var accountData = connection.Query("SELECT COUNT(*) FROM Account;");
            Console.WriteLine($"AccountCount: {JsonConvert.SerializeObject(accountData)}");
            var documentData = connection.Query("SELECT COUNT(*) FROM Document;");
            Console.WriteLine($"DocumentCount: {JsonConvert.SerializeObject(documentData)}");
            var userData = connection.Query("SELECT COUNT(*) FROM User;");
            Console.WriteLine($"UserCount: {JsonConvert.SerializeObject(userData)}");
        }
    }
}
