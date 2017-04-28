using LinqToDB.Mapping;

namespace IdentityServer4.LinqToDB.Entities
{
	public abstract class Secret : Models.Secret
	{
		[PrimaryKey]
		[Identity]
		public int Id { get; set; }
	}
}