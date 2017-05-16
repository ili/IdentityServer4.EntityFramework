namespace IdentityServer4.LinqToDB.Entities
{
	/// <summary>
	///     Represents <see cref="Models.IdentityResource.UserClaims" /> in database
	/// </summary>
	public class IdentityClaim : UserClaim
	{
		/// <summary>
		///     <see cref="IdentityResource.Id" />
		/// </summary>
		public int IdentityResourceId { get; set; }
	}
}