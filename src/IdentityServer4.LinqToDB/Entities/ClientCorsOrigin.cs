using LinqToDB.Mapping;

namespace IdentityServer4.LinqToDB.Entities
{
	public class ClientCorsOrigin
	{
		[PrimaryKey]
		[Identity]
		public int Id { get; set; }

		public string Origin { get; set; }
		public string ClientId { get; set; }
	}
}