using IdentityServer4.Models;
using LinqToDB.Mapping;

namespace IdentityServer4.LinqToDB.Entities
{
	public class ApiScope : Scope
	{
		[PrimaryKey]
		[Identity]
		public int Id { get; set; }

		public int ApiResourceId { get; set; }
	}
}