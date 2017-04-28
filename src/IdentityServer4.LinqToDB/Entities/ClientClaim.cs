using System.Security.Claims;
using LinqToDB.Mapping;

namespace IdentityServer4.LinqToDB.Entities
{
	public class ClientClaim : Claim
	{
		public ClientClaim(string type, string value) : base(type, value)
		{
		}

		public ClientClaim() : this(string.Empty, string.Empty)
		{
		}

		[PrimaryKey]
		[Identity]
		public int Id { get; set; }

		public string ClientId { get; set; }
	}
}