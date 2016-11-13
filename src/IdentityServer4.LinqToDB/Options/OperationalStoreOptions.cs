namespace IdentityServer4.LinqToDB.Options
{
    public class OperationalStoreOptions
    {
        public string DefaultSchema { get; set; } = null;

        public TableConfiguration PersistedGrants { get; set; } = new TableConfiguration("PersistedGrants");
    }
}