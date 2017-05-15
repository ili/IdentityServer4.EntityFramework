using System.Security.Claims;
using IdentityServer4.Models;
using LinqToDB.Mapping;

namespace IdentityServer4.LinqToDB.Entities
{
	/// <summary>
	/// Represents <see cref="Models.Client.Claims"/> in database
	/// </summary>
	public class ClientClaim : Claim
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="type">sets <see cref="Claim.Type"/></param>
		/// <param name="value">sets <see cref="Claim.Value"/> </param>
		public ClientClaim(string type, string value) : base(type, value)
		{
		}

		/// <summary>
		/// Default constuctor
		/// </summary>
		public ClientClaim() : this(string.Empty, string.Empty)
		{
		}

		/// <summary>
		/// <see cref="int"/> identity field
		/// </summary>
		[PrimaryKey]
		[Identity]
		public int Id { get; set; }

		/// <summary>
		/// <see cref="Models.Client.ClientId"/>
		/// </summary>
		public string ClientId { get; set; }
	}
}