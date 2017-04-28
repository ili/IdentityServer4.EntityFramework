using LinqToDB.Mapping;

namespace IdentityServer4.LinqToDB.Entities
{
	public abstract class UserClaim
	{
		[PrimaryKey]
		[Identity]
		public int Id { get; set; }

		public string Type { get; set; }
	}
}