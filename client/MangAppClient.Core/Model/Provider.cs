namespace MangAppClient.Core.Model
{
    using SQLite;

    public class Provider
    {
        [PrimaryKey]
        public string Key { get; set; }

        public string Name { get; set; }

        public string Url { get; set; }

        public bool IsDefault { get; set; }

        // TODO: This image has to be recovered by the local service, like manga summary and backgrounds
        internal string LogoImagePath { get; set; }
    }
}
