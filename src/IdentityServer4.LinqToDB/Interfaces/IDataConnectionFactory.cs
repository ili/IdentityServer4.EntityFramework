using LinqToDB;
using LinqToDB.Data;

namespace IdentityServer4.Contrib.LinqToDB.Interfaces
{
	public interface IDataConnectionFactory
	{
		DataContext GetContext();
		DataConnection GetConnection();
	}
}