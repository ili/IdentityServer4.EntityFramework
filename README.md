# IdentityServer4.LinqToDB 
* Current build status [![Build status](https://ci.appveyor.com/api/projects/status/8vn4m6ip1rt5hkgh?svg=true)](https://ci.appveyor.com/project/igor-tkachev/identityserver4-linqtodb)
* Master build status [![Master status](https://ci.appveyor.com/api/projects/status/8vn4m6ip1rt5hkgh/branch/master?svg=true)](https://ci.appveyor.com/project/igor-tkachev/identityserver4-linqtodb/branch/master)

## Intro 
`IdentityServer4.LinqToDB` is a persistence layer for [IdentityServer4](https://identityserver.io/) configuration data that uses [Linq To DB](https://github.com/linq2db/linq2db) as it's database abstraction.

Project's source code is originally based on [IdentityServer4.EntityFramework](https://github.com/IdentityServer/IdentityServer4.EntityFramework)

## Feeds
* Release builds can be found on [NuGet](https://www.nuget.org/packages?q=linq2db)
* [MyGet](https://www.myget.org/gallery/linq2db)
  * V2 `https://www.myget.org/F/linq2db/api/v2`
  * V3 `https://www.myget.org/F/linq2db/api/v3/index.json`

## Usage
Install package:

`PM> Install-Package IdentityServer4.LinqToDB`

### POCOs
All POCOs are under `IdentityServer4.LinqToDB.Entities` namespace. Mostly POCOs are inherited from `IdentityServer4.Models.*` classes (to avoid unnesesary mapping), with adding only `Id` properties for identities and foreign keys. `IdentityServer4.Models.Client.ClientId` is used as promary key for `Client` entity. All `Id`'s are identity by default.

### Running
Firstly you should create your connection factory as implementation of `IdentityServer4.LinqToDB.Interfaces.IDataConnectionFactory`, for example:
```cs
public class MyConnectionFactory : IDataConnectionFactory
{
    public DataContext GetContext() => new DataContext();
    public DataConnection GetConnection() => new DataConnection();
}
```
In your `Strartup.cs`

```cs
public IServiceProvider ConfigureServices(IServiceCollection services)
{
    // ...
     
    // create factory instance 
    var factory = new MyConnectionFactory();

    services.AddIdentityServer() // Add IdentityServer
        .AddTemporarySigningCredential()
        // some other stuff
	
        // Configure Linq To DB storage for IdentityServer
        .AddConfigurationStore(factory)
        .AddOperationalStore(factory);

    // ...
      
    return services.BuildServiceProvider(true);
}
```
