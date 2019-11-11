// © 2019 Mobsites. All rights reserved.
// Licensed under the MIT License.

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;

namespace Mobsites.AspNetCore.Identity.Cosmos
{
    /// <summary>
    ///     Contains extension methods to <see cref="IServiceCollection"/> for configuring Cosmos Identity.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        ///     Registers the default Cosmos storage provider that the Cosmos Identity library provides.
        /// </summary>
        /// <param name="services">The services available in the application.</param>
        /// <param name="setupAction">An action to configure the <see cref="CosmosStorageProviderOptions"/>.</param>
        /// <returns>The <see cref="IServiceCollection"/> instance this method extends.</returns>
        public static IServiceCollection AddCosmosStorageProvider(this IServiceCollection services, Action<CosmosStorageProviderOptions> setupAction = default)
        {
            if (setupAction != null)
            {
                services.Configure(setupAction);
            }

            services
                .TryAddSingleton<CosmosStorageProvider>();

            return services;
        }


        /// <summary>
        ///     Registers a custom or an extended Cosmos storage provider that the Cosmos Identity library can use.
        /// </summary>
        /// <param name="services">The services available in the application.</param>
        /// <param name="setupAction">An action to configure the <see cref="CosmosStorageProviderOptions"/>.</param>
        /// <returns>The <see cref="IServiceCollection"/> instance this method extends.</returns>
        public static IServiceCollection AddCosmosStorageProvider<TCustomStorageProvider>(this IServiceCollection services, Action<CosmosStorageProviderOptions> setupAction = default)
            where TCustomStorageProvider : IIdentityStorageProvider
        {
            if (setupAction != null)
            {
                services.Configure(setupAction);
            }

            services
                .TryAddSingleton(typeof(TCustomStorageProvider));

            return services;
        }


        /// <summary>
        ///     Adds and configures the identity system to use <see cref="CosmosStorageProvider"/> as its persistence store.
        /// </summary>
        /// <param name="services">The services available in the application.</param>
        /// <param name="setupAction">An action to configure the <see cref="IdentityOptions"/>.</param>
        /// <returns>An <see cref="IdentityBuilder"/> for creating and configuring the identity system.</returns>
        public static IdentityBuilder AddDefaultCosmosIdentity(this IServiceCollection services, Action<IdentityOptions> setupAction = default)
            => services.AddDefaultCosmosIdentity<IdentityUser, IdentityRole>(setupAction);


        /// <summary>
        ///     Adds and configures the identity system to use <see cref="CosmosStorageProvider"/> as its persistence store with the specified <typeparamref name="TUser"/> model.
        /// </summary>
        /// <typeparam name="TUser">The type representing a user.</typeparam>
        /// <param name="services">The services available in the application.</param>
        /// <param name="setupAction">An action to configure the <see cref="IdentityOptions"/>.</param>
        /// <returns>An <see cref="IdentityBuilder"/> for creating and configuring the identity system.</returns>
        public static IdentityBuilder AddDefaultCosmosIdentity<TUser>(this IServiceCollection services, Action<IdentityOptions> setupAction = default)
            where TUser : IdentityUser, new()
            => services.AddDefaultCosmosIdentity<TUser, IdentityRole>(setupAction);


        /// <summary>
        ///     Adds and configures the identity system to use <see cref="CosmosStorageProvider"/> as its persistence store with the specified <typeparamref name="TUser"/> and <typeparamref name="TRole"/> models.
        /// </summary>
        /// <typeparam name="TUser">The type representing a user.</typeparam>
        /// <typeparam name="TRole">The type representing a role.</typeparam>
        /// <param name="services">The services available in the application.</param>
        /// <param name="setupAction">An action to configure the <see cref="IdentityOptions"/>.</param>
        /// <returns>An <see cref="IdentityBuilder"/> for creating and configuring the identity system.</returns>
        public static IdentityBuilder AddDefaultCosmosIdentity<TUser, TRole>(this IServiceCollection services, Action<IdentityOptions> setupAction = default)
            where TUser : IdentityUser, new()
            where TRole : IdentityRole, new()
        {
            return services
                .AddIdentity<TUser, TRole>(setupAction)
                .AddCosmosStores<CosmosStorageProvider>();
        }


        /// <summary>
        ///     Adds and configures the identity system to use <see cref="CosmosStorageProvider"/> as its persistence store with the specified identity models.
        /// </summary>
        /// <typeparam name="TUser">The type representing a user.</typeparam>
        /// <typeparam name="TRole">The type representing a role.</typeparam>
        /// <param name="services">The services available in the application.</param>
        /// <param name="setupAction">An action to configure the <see cref="IdentityOptions"/>.</param>
        /// <returns>An <see cref="IdentityBuilder"/> for creating and configuring the identity system.</returns>
        public static IdentityBuilder AddDefaultCosmosIdentity<TUser, TRole, TUserClaim, TUserRole, TUserLogin, TUserToken, TRoleClaim>(this IServiceCollection services, Action<IdentityOptions> setupAction = default)
            where TUser : IdentityUser, new()
            where TRole : IdentityRole, new()
            where TUserClaim : IdentityUserClaim, new()
            where TUserRole : IdentityUserRole, new()
            where TUserLogin : IdentityUserLogin, new()
            where TUserToken : IdentityUserToken, new()
            where TRoleClaim : IdentityRoleClaim, new()
        {
            return services
                .AddIdentity<TUser, TRole>(setupAction)
                .AddCosmosStores<CosmosStorageProvider, TUserClaim, TUserRole, TUserLogin, TUserToken, TRoleClaim>();
        }


        /// <summary>
        ///      Adds and configures the identity system to use <typeparamref name="TCosmosStorageProvider" /> as its persistence store.
        /// </summary>
        /// <typeparam name="TCosmosStorageProvider">The type representing a custom or an extended storage provider.</typeparam>
        /// <param name="services">The services available in the application.</param>
        /// <param name="setupAction">An action to configure the <see cref="IdentityOptions"/>.</param>
        /// <returns>An <see cref="IdentityBuilder"/> for creating and configuring the identity system.</returns>
        public static IdentityBuilder AddCosmosIdentity<TCosmosStorageProvider>(this IServiceCollection services, Action<IdentityOptions> setupAction = default)
            where TCosmosStorageProvider : IIdentityStorageProvider
            => services.AddCosmosIdentity<TCosmosStorageProvider, IdentityUser, IdentityRole>(setupAction);


        /// <summary>
        ///      Adds and configures the identity system to use <typeparamref name="TCosmosStorageProvider" /> as its persistence store with the specified <typeparamref name="TUser"/> model.
        /// </summary>
        /// <typeparam name="TCosmosStorageProvider">The type representing a custom or an extended storage provider.</typeparam>
        /// <typeparam name="TUser">The type representing a user.</typeparam>
        /// <param name="services">The services available in the application.</param>
        /// <param name="setupAction">An action to configure the <see cref="IdentityOptions"/>.</param>
        /// <returns>An <see cref="IdentityBuilder"/> for creating and configuring the identity system.</returns>
        public static IdentityBuilder AddCosmosIdentity<TCosmosStorageProvider, TUser>(this IServiceCollection services, Action<IdentityOptions> setupAction = default)
            where TCosmosStorageProvider : IIdentityStorageProvider
            where TUser : IdentityUser, new()
            => services.AddCosmosIdentity<TCosmosStorageProvider, TUser, IdentityRole>(setupAction);


        /// <summary>
        ///      Adds and configures the identity system to use <typeparamref name="TCosmosStorageProvider" /> as its persistence store with the specified <typeparamref name="TUser"/> and <typeparamref name="TRole"/> models.
        /// </summary>
        /// <typeparam name="TCosmosStorageProvider">The type representing a custom or an extended storage provider.</typeparam>
        /// <typeparam name="TUser">The type representing a user.</typeparam>
        /// <typeparam name="TRole">The type representing a role.</typeparam>
        /// <param name="services">The services available in the application.</param>
        /// <param name="setupAction">An action to configure the <see cref="IdentityOptions"/>.</param>
        /// <returns>An <see cref="IdentityBuilder"/> for creating and configuring the identity system.</returns>
        public static IdentityBuilder AddCosmosIdentity<TCosmosStorageProvider, TUser, TRole>(this IServiceCollection services, Action<IdentityOptions> setupAction = default)
            where TCosmosStorageProvider : IIdentityStorageProvider
            where TUser : IdentityUser, new()
            where TRole : IdentityRole, new()
        {
            return services
                .AddIdentity<TUser, TRole>(setupAction)
                .AddCosmosStores<TCosmosStorageProvider>();
        }


        /// <summary>
        ///      Adds and configures the identity system to use <typeparamref name="TCosmosStorageProvider" /> as its persistence store with the specified identity models.
        /// </summary>
        /// <typeparam name="TCosmosStorageProvider">The type representing a custom or an extended storage provider.</typeparam>
        /// <typeparam name="TUser">The type representing a user.</typeparam>
        /// <typeparam name="TRole">The type representing a role.</typeparam>
        /// <typeparam name="TUserClaim">The type representing a user claim.</typeparam>
        /// <typeparam name="TUserRole">The type representing a user role.</typeparam>
        /// <typeparam name="TUserLogin">The type representing a user external login.</typeparam>
        /// <typeparam name="TUserToken">The type representing a user token.</typeparam>
        /// <typeparam name="TRoleClaim">The type representing a role claim.</typeparam>
        /// <param name="services">The services available in the application.</param>
        /// <param name="setupAction">An action to configure the <see cref="IdentityOptions"/>.</param>
        /// <returns>An <see cref="IdentityBuilder"/> for creating and configuring the identity system.</returns>
        public static IdentityBuilder AddCosmosIdentity<TCosmosStorageProvider, TUser, TRole, TUserClaim, TUserRole, TUserLogin, TUserToken, TRoleClaim>(this IServiceCollection services, Action<IdentityOptions> setupAction = default)
            where TCosmosStorageProvider : IIdentityStorageProvider
            where TUser : IdentityUser, new()
            where TRole : IdentityRole, new()
            where TUserClaim : IdentityUserClaim, new()
            where TUserRole : IdentityUserRole, new()
            where TUserLogin : IdentityUserLogin, new()
            where TUserToken : IdentityUserToken, new()
            where TRoleClaim : IdentityRoleClaim, new()
        {
            return services
                .AddIdentity<TUser, TRole>(setupAction)
                .AddCosmosStores<TCosmosStorageProvider, TUserClaim, TUserRole, TUserLogin, TUserToken, TRoleClaim>();
        }
    }
}
