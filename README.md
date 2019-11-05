# Cosmos Identity

**NOTE: This library uses the .Net Standard 2.0 based [Azure Cosmos SDK v3](https://github.com/Azure/azure-cosmos-dotnet-v3).**

Cosmos Identity is a storage provider for [ASP.NET Core Identity](https://github.com/aspnet/AspNetCore/tree/master/src/Identity) that uses [Azure Cosmos DB](https://azure.microsoft.com/en-us/services/cosmos-db/) as its data store. 

## Design and Development

The out-of-the-box (and open source) [Entity Framework Core](https://github.com/aspnet/AspNetCore/tree/master/src/Identity/EntityFrameworkCore/src) solution was used as the principal guide in design and development. As such, Cosmos Identity supports the same use cases and features that the default Entity Framework Core solution does. 

Also considered during development were two third party Cosmos-based solutions Bernhard Koenig's [AspNetCore.Identity.DocumentDb](https://github.com/codekoenig/AspNetCore.Identity.DocumentDb), which uses the older Microsoft.Azure.DocumentDB.Core SDK, and f14shm4n's [AspNetCore.Identity.DocumentDb](https://github.com/f14shm4n/AspNetCore.Identity.DocumentDb), which uses the newer .Net Standard 2.0 based one.

## Install via [Nuget.org](https://www.nuget.org/packages/Mobsites.AspNetCore.Identity.Cosmos)

```shell
Install-Package Mobsites.AspNetCore.Identity.Cosmos
```
