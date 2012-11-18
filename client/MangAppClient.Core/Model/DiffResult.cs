namespace MangAppClient.Core.Model
{
    public abstract class DiffResult
    {
        protected DiffResult(string id)
        {
            this.Id = id;
        }

        public string Id { get; private set; }
    }
}
