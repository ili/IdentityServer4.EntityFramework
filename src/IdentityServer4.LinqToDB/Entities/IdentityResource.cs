using LinqToDB.Mapping;

namespace IdentityServer4.LinqToDB.Entities
{
	public class IdentityResource : Models.IdentityResource
	{
		[PrimaryKey]
		[Identity]
		public int Id { get; set; }
	}
}