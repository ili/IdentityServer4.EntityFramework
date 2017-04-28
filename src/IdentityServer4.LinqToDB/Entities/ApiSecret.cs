namespace IdentityServer4.LinqToDB.Entities
{
	public class ApiSecret : Secret
	{
		public int ApiResourceId { get; set; }
	}
}