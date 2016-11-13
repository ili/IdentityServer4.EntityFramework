// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System;
using System.Collections.Generic;
using System.Linq;
using IdentityServer4.EntityFramework.Interfaces;
using LinqToDB;

namespace IdentityServer4.EntityFramework.IntegrationTests
{
    /// <summary>
    /// xUnit ClassFixture for creating and deleting integration test databases.
    /// </summary>
    /// <typeparam name="T">DbContext of Type T</typeparam>
    public class DatabaseProviderFixture : IDisposable 
    {
	    public IEnumerable<IDataConnectionFactory> Connections
	    {
		    get
		    {
			    yield break;
		    }
	    }
        
        public void Dispose()
        {
            //foreach (var option in Options.ToList())
            //{
            //    using (var context = (T)Activator.CreateInstance(typeof(T), option, StoreOptions))
            //    {
            //        context.Database.EnsureDeleted();
            //    }
            //}
        }
    }
}