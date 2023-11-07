using SmartVault.Library;
using System;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace SmartVault.DataGeneration.Utils
{
    public static class ScriptGeneration
    {
        public static string GetAllBussinessScript(string[] files)
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
