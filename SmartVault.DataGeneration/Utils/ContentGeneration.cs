using SmartVault.Program.BusinessObjects;
using System;
using System.IO;
using System.Linq;
using System.Security.Policy;

namespace SmartVault.DataGeneration.Utils
{
    public static class ContentGeneration
    {
        public static string GetContent(int numberLines)
        {
            string baseText = "This is my test document";
            return string.Join(Environment.NewLine, Enumerable.Repeat(baseText, numberLines));
        }

        public static DateTime RandomDay()
        {
            DateTime start = new DateTime(1985, 1, 1);
            Random gen = new Random();
            int range = (DateTime.Today - start).Days;
            return start.AddDays(gen.Next(range));
        }

    }
}
