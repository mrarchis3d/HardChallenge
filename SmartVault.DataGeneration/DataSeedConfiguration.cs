namespace SmartVault.DataGeneration
{
    public class DataSeedConfiguration
    {
        public string ConnectionString { get; set; }
        public int BulkUsers { get; set; }
        public int BulkDocuments { get; set; }
        public long DocumentFileLenght { get; set; }
        public string Content { get; set; }

    }
}
