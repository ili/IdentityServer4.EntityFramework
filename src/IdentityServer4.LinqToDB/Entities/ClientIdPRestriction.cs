using LinqToDB.Mapping;

namespace IdentityServer4.LinqToDB.Entities
{
	public class ClientIdPRestriction
	{
		[PrimaryKey]
		[Identity]
		public int Id { get; set; }

		public string Provider { get; set; }
		public string ClientId { get; set; }
	}
}