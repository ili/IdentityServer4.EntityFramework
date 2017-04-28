namespace IdentityServer4.LinqToDB.Entities
{
	public class ApiResourceClaim : UserClaim
	{
		public int ApiResourceId { get; set; }
	}
}