namespace MangAppClient.Core.Model
{
    public abstract class DiffResult
    {
        protected DiffResult(int id)
        {
            this.Id = id;
        }

        public int Id { get; private set; }
    }
}
