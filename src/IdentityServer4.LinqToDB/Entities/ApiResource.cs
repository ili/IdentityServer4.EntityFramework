using LinqToDB.Mapping;

namespace IdentityServer4.LinqToDB.Entities
{
	[Table("ApiResource")]
	public class ApiResource : Models.ApiResource
	{
		[PrimaryKey]
		[Identity]
		public int Id { get; set; }
	}
}