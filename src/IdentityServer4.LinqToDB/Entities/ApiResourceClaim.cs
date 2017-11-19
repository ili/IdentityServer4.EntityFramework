namespace IdentityServer4.Contrib.LinqToDB.Entities
{
	/// <summary>
	///     Represents <see cref="Models.ApiResource.UserClaims" /> in database
	/// </summary>
	public class ApiResourceClaim : UserClaim
	{
		/// <summary>
		///     <see cref="ApiResource.Id" />
		/// </summary>
		public int ApiResourceId { get; set; }
	}
}