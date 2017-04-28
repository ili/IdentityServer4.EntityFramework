using LinqToDB.Mapping;

namespace IdentityServer4.LinqToDB.Entities
{
	public class ClientGrantType
	{
		[PrimaryKey]
		[Identity]
		public int Id { get; set; }

		public string GrantType { get; set; }
		public string ClientId { get; set; }
	}
}