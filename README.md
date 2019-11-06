# Cosmos Identity

Cosmos Identity is a storage provider for [ASP.NET Core Identity](https://github.com/aspnet/AspNetCore/tree/master/src/Identity) that uses [Azure Cosmos DB](https://azure.microsoft.com/en-us/services/cosmos-db/) as its data store. 

**NOTE: In step with Azure Cosmos, which has moved away from non-partitioned containers, this library supports partitioned containers only.**

## Dependencies

###### .NETStandard 2.0
* Microsoft.AspNetCore.Identity (>= 2.2.0)
* Microsoft.Azure.Cosmos (>= 3.4.0)
* Microsoft.Extensions.Identity.Stores (>= 3.0.0)
* System.Text.Json (>= 4.6.0)

## Design and Development

The out-of-the-box (and open source) [Microsoft.AspNetCore.Identity.EntityFrameworkCore](https://github.com/aspnet/AspNetCore/tree/master/src/Identity/EntityFrameworkCore/src) library was used as the principal guide in design and development. As such, Cosmos Identity supports the same use cases and features that the default `Microsoft.AspNetCore.Identity.EntityFrameworkCore` library does. 

Also considered during development were two third party Cosmos-based solutions:

* Bernhard Koenig's [AspNetCore.Identity.DocumentDb](https://github.com/codekoenig/AspNetCore.Identity.DocumentDb), which uses the older `Microsoft.Azure.DocumentDB.Core` SDK.

* f14shm4n's [AspNetCore.Identity.DocumentDb](https://github.com/f14shm4n/AspNetCore.Identity.DocumentDb), which uses the newer .Net Standard 2.0 based one.

## Getting Started

Using the default implementation of Cosmos Identity is fairly straightforward. Just follow the steps outlined below. 

**NOTE: There is one caveat to keep in mind when using the default implementation—the partition key path will be set to `/PartitionKey` for a newly created identity container. If the container to be used for the identity store already exists, then the container must have a partition key path of `/PartitionKey` in order to use the default implementation, else a custom extended Cosmos Identity approach must be used (see further down below for guidance).**

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

5. In the same class, wire up services in `ConfigureServices(IServiceCollection services)` to add Cosmos Identity. Pass in Identity options or not. Add any other default `IdentityBuilder` methods:

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
        // Add other default IdentityBuilder methods.
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

7. Safely remove any dependencies to `Microsoft.AspNetCore.Identity.EntityFrameworkCore`.

## Samples

The samples demonstrate both the default implementation of Cosmos Identity and a custom extension of Cosmos Identity using a Razor Pages web app built with the .Net Core 3.0 Web App template with individual account users. `Microsoft.AspNetCore.Identity.EntityFrameworkCore` was then stripped out, leaving only `Microsoft.AspNetCore.Identity`. (Note: When wiring up your own project, if any of the built-in Identity UI needs to be scaffold, be sure to do so before stripping out `Microsoft.AspNetCore.Identity.EntityFrameworkCore`.)

### Required to run the samples

As noted above, the samples use .Net Core 3.0, so a suitable dev environment is necessary. Other than that, download and install the [Azure Cosmos Emulator](https://docs.microsoft.com/en-us/azure/cosmos-db/local-emulator-release-notes) and fire up the sample.

### On first running one of the samples

![Sample Home Page](https://github.com/Mobsites/AspNetCore.Identity.Cosmos/blob/master/samples/Default.Cosmos.Identity.Razor.Sample/wwwroot/images/sample-home-page-no-users.png)

### Register users
![Sample Register Page](https://github.com/Mobsites/AspNetCore.Identity.Cosmos/blob/master/samples/Default.Cosmos.Identity.Razor.Sample/wwwroot/images/sample-register-page.png)

### After Registering Users
![Sample Home Page With Users](https://github.com/Mobsites/AspNetCore.Identity.Cosmos/blob/master/samples/Default.Cosmos.Identity.Razor.Sample/wwwroot/images/sample-home-page-with-users.png)
