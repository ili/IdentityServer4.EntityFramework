using LinqToDB.Mapping;

namespace IdentityServer4.LinqToDB.Entities
{
	public class ClientRedirectUri
	{
		[PrimaryKey]
		[Identity]
		public int Id { get; set; }

		public string RedirectUri { get; set; }
		public string ClientId { get; set; }
	}
}