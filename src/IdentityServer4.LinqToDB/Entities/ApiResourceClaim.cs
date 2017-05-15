using IdentityServer4.Models;

namespace IdentityServer4.LinqToDB.Entities
{
	/// <summary>
	/// Represents <see cref="Models.ApiResource.UserClaims"/> in database
	/// </summary>
	public class ApiResourceClaim : UserClaim
	{
		/// <summary>
		/// <see cref="ApiResource.Id"/>
		/// </summary>
		public int ApiResourceId { get; set; }
	}
}