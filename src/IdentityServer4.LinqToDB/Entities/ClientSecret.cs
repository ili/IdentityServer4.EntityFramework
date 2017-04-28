namespace IdentityServer4.LinqToDB.Entities
{
	public class ClientSecret : Secret
	{
		public string ClientId { get; set; }
	}
}