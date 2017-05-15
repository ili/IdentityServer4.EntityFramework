using System;
using IdentityServer4.LinqToDB.Interfaces;
using LinqToDB;
using LinqToDB.Data;

namespace Host
{
	class DataConnectionFactory : IDataConnectionFactory
	{
		public DataConnection GetConnection()
		{
			return new DataConnection();
		}

		public DataContext GetContext()
		{
			return new DataContext();
		}
	}
}