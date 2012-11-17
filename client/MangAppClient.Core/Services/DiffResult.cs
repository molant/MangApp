namespace MangAppClient.Core.Services
{
    internal abstract class DiffResult
    {
        protected DiffResult(int id)
        {
            this.Id = id;
        }

        internal int Id { get; private set; }
    }
}
