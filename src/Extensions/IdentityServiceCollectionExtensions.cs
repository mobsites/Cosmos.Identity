// © 2019 Mobsites. All rights reserved.
// Licensed under the MIT License.

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Mobsites.AspNetCore.Identity.Cosmos
{
    /// <summary>
    ///     Contains extension methods to <see cref="IServiceCollection"/> for configuring identity services.
    /// </summary>
    public static class IdentityServiceCollectionExtensions
    {
        /// <summary>
        ///     Adds and configures the identity system to use Cosmos as its persistence store.
        /// </summary>
        /// <param name="services">The services available in the application.</param>
        /// <param name="setupAction">An action to configure the <see cref="IdentityOptions"/>.</param>
        public static IdentityBuilder AddCosmosIdentity(this IServiceCollection services, Action<IdentityOptions> setupAction = default)
        {
            services
                .AddSingleton<ICosmosIdentityContainer, CosmosIdentityContainer>();

            return services
                .AddIdentity<IdentityUser, IdentityRole>(setupAction)
                .AddCosmosStores();
        }


        /// <summary>
        ///     Adds and configures the identity system to use Cosmos as its persistence store.
        /// </summary>
        /// <param name="services">The services available in the application.</param>
        /// <param name="setupAction">An action to configure the <see cref="IdentityOptions"/>.</param>
        public static IdentityBuilder AddCosmosIdentity<TCosmosIdentityContainer>(this IServiceCollection services, Action<IdentityOptions> setupAction = default)
            where TCosmosIdentityContainer : ICosmosIdentityContainer
        {
            services
                .AddSingleton(typeof(ICosmosIdentityContainer), typeof(TCosmosIdentityContainer));

            return services
                .AddIdentity<IdentityUser, IdentityRole>(setupAction)
                .AddCosmosStores();
        }


        /// <summary>
        ///     Adds and configures the identity system to use Cosmos as its persistence store.
        /// </summary>
        /// <typeparam name="TUser">The type representing a user.</typeparam>
        /// <param name="services">The services available in the application.</param>
        /// <param name="setupAction">An action to configure the <see cref="IdentityOptions"/>.</param>
        public static IdentityBuilder AddCosmosIdentity<TCosmosIdentityContainer, TUser>(this IServiceCollection services, Action<IdentityOptions> setupAction = default)
            where TCosmosIdentityContainer : ICosmosIdentityContainer
            where TUser : IdentityUser, new()
        {
            services
                .AddSingleton(typeof(ICosmosIdentityContainer), typeof(TCosmosIdentityContainer));

            return services
                .AddIdentity<TUser, IdentityRole>(setupAction)
                .AddCosmosStores();
        }


        /// <summary>
        ///     Adds and configures the identity system to use Cosmos as its persistence store.
        /// </summary>
        /// <typeparam name="TUser">The type representing a user.</typeparam>
        /// <typeparam name="TRole">The type representing a role.</typeparam>
        /// <param name="services">The services available in the application.</param>
        /// <param name="setupAction">An action to configure the <see cref="IdentityOptions"/>.</param>
        public static IdentityBuilder AddCosmosIdentity<TCosmosIdentityContainer, TUser, TRole>(this IServiceCollection services, Action<IdentityOptions> setupAction = default)
            where TCosmosIdentityContainer : ICosmosIdentityContainer
            where TUser : IdentityUser, new()
            where TRole : IdentityRole, new()
        {
            services
                .AddSingleton(typeof(ICosmosIdentityContainer), typeof(TCosmosIdentityContainer));

            return services
                .AddIdentity<TUser, TRole>(setupAction)
                .AddCosmosStores();
        }


        /// <summary>
        ///     Adds and configures the identity system to use Cosmos as its persistence store.
        /// </summary>
        /// <typeparam name="TUser">The type representing a user.</typeparam>
        /// <typeparam name="TRole">The type representing a role.</typeparam>
        /// <typeparam name="TUserClaim">The type representing a user claim.</typeparam>
        /// <typeparam name="TUserRole">The type representing a user role.</typeparam>
        /// <typeparam name="TUserLogin">The type representing a user external login.</typeparam>
        /// <typeparam name="TUserToken">The type representing a user token.</typeparam>
        /// <typeparam name="TRoleClaim">The type representing a role claim.</typeparam>
        /// <param name="services">The services available in the application.</param>
        /// <param name="setupAction">An action to configure the <see cref="IdentityOptions"/>.</param>
        public static IdentityBuilder AddCosmosIdentity<TCosmosIdentityContainer,TUser, TRole, TUserClaim, TUserRole, TUserLogin, TUserToken, TRoleClaim>(this IServiceCollection services, Action<IdentityOptions> setupAction = default)
            where TCosmosIdentityContainer : ICosmosIdentityContainer
            where TUser : IdentityUser, new()
            where TRole : IdentityRole, new()
            where TUserClaim : IdentityUserClaim, new()
            where TUserRole : IdentityUserRole, new()
            where TUserLogin : IdentityUserLogin, new()
            where TUserToken : IdentityUserToken, new()
            where TRoleClaim : IdentityRoleClaim, new()
        {
            services
                .AddSingleton(typeof(ICosmosIdentityContainer), typeof(TCosmosIdentityContainer));

            return services
                .AddIdentity<TUser, TRole>(setupAction)
                .AddCosmosStores<TUserClaim, TUserRole, TUserLogin, TUserToken, TRoleClaim>();
        }
    }
}
