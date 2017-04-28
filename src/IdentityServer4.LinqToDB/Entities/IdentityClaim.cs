namespace IdentityServer4.LinqToDB.Entities
{
	public class IdentityClaim : UserClaim
	{
		public int IdentityResourceId { get; set; }
	}
}