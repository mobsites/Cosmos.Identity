# Cosmos Identity

Cosmos Identity is a storage provider for [ASP.NET Core Identity](https://github.com/aspnet/AspNetCore/tree/master/src/Identity) that uses [Azure Cosmos DB](https://azure.microsoft.com/en-us/services/cosmos-db/) as its data store. 

**NOTE: This library uses the .Net Standard 2.0 [Azure Cosmos SDK v3](https://github.com/Azure/azure-cosmos-dotnet-v3).**

## Design and Development

The out-of-the-box (and open source) [Entity Framework Core](https://github.com/aspnet/AspNetCore/tree/master/src/Identity/EntityFrameworkCore/src) solution was used as the principal guide in design and development. As such, Cosmos Identity supports the same use cases and features that the default Entity Framework Core solution does. 

Also considered during development were two third party Cosmos-based solutions:

* Bernhard Koenig's [AspNetCore.Identity.DocumentDb](https://github.com/codekoenig/AspNetCore.Identity.DocumentDb), which uses the older Microsoft.Azure.DocumentDB.Core SDK

* f14shm4n's [AspNetCore.Identity.DocumentDb](https://github.com/f14shm4n/AspNetCore.Identity.DocumentDb), which uses the newer .Net Standard 2.0 based one.

## Getting Started

Using the default implementation of Cosmos Identity is fairly straightforward:

1. Install via [Nuget.org](https://www.nuget.org/packages/Mobsites.AspNetCore.Identity.Cosmos)

```shell
Install-Package Mobsites.AspNetCore.Identity.Cosmos
```

2. Add a Cosmos connection string to appsettings.json using the name `CosmosIdentity`:

```
{
  "ConnectionStrings": {
    "CosmosIdentity": "{cosmos-connection-string}"
  },
  ...
}
```

3. Add the following key-value pairs to appsettings.json using the ids of the Cosmos database and container for values:

```
{
  ...
  "IdentityContainerId": "{containerId}",
  "IdentityDatabaseId": "{databaseId}",
  ...
}
```

4. Add the following `using` statement to the Startup class:

```csharp
using Mobsites.AspNetCore.Identity.Cosmos;
```

5. In the same class, wire up services in `ConfigureServices(IServiceCollection services)` to add Cosmos Identity. Pass in Identity options or not. Add any other `IdentityBuilder` methods desired:

```csharp
public void ConfigureServices(IServiceCollection services)
{
    // Add default Cosmos Identity Implementation.
    // Passing in Identity options are...well, optional.
    services
        .AddCosmosIdentity(options =>
        {
            // User settings
            options.User.RequireUniqueEmail = true;

            // Password settings
            options.Password.RequireDigit = true;
            options.Password.RequiredLength = 8;
            options.Password.RequireLowercase = true;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = true;

            // Lockout settings
            options.Lockout.AllowedForNewUsers = true;
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
            options.Lockout.MaxFailedAccessAttempts = 5;

        })
        .AddDefaultUI()
        .AddDefaultTokenProviders();

    // Add Razor
    services
        .AddRazorPages();
}
```

6. Add one or both of the following `using` statements anywhere else that may be needed to clear up any conflict with the namespace `Microsoft.AspNetCore.Identity`:

```csharp
using IdentityUser = Mobsites.AspNetCore.Identity.Cosmos.IdentityUser;
using IdentityRole = Mobsites.AspNetCore.Identity.Cosmos.IdentityRole;

```

7. Safely remove any dependencies to Identity Entity Framework Core.
