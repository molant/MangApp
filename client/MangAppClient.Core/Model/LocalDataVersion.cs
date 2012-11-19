namespace MangAppClient.Core.Model
{
    using SQLite;

    public class LocalDataVersion
    {
        public LocalDataVersion(int version)
        {
            this.Version = version;
        }

        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public int Version { get; set; }
    }
}
