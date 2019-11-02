// © 2019 Mobsites. All rights reserved.
// Licensed under the MIT License.

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;

namespace Mobsites.AspNetCore.Identity.Cosmos
{
    /// <summary>
    ///     Contains extension methods to <see cref="IdentityBuilder"/> for adding Cosmos stores.
    /// </summary>
    public static class IdentityBuilderExtensions
    {
        /// <summary>
        ///     Adds a Cosmos implementation of identity information stores.
        /// </summary>
        /// <param name="builder">The <see cref="IdentityBuilder"/> instance this method extends.</param>
        /// <returns>The <see cref="IdentityBuilder"/> instance this method extends.</returns>
        public static IdentityBuilder AddCosmosStores(this IdentityBuilder builder)
        {
            if (!typeof(IdentityUser).IsAssignableFrom(builder.UserType))
            {
                throw new InvalidOperationException($"{builder.UserType.Name} must extend {typeof(IdentityUser).FullName}.");
            }
            if (!typeof(IdentityRole).IsAssignableFrom(builder.RoleType))
            {
                throw new InvalidOperationException($"{builder.RoleType.Name} must extend {typeof(IdentityRole).FullName}.");
            }

            Type userStoreType = typeof(UserStore<,,,,,,>).MakeGenericType(
                builder.UserType,
                builder.RoleType,
                typeof(IdentityUserClaim),
                typeof(IdentityUserRole),
                typeof(IdentityUserLogin),
                typeof(IdentityUserToken),
                typeof(IdentityRoleClaim));

            Type roleStoreType = typeof(RoleStore<,,>).MakeGenericType(
                builder.RoleType,
                typeof(IdentityUserRole),
                typeof(IdentityRoleClaim));

            builder.Services.TryAddScoped<ICosmosIdentityStorageProvider, CosmosIdentityStorageProvider>();
            builder.Services.TryAddScoped(typeof(IUserStore<>).MakeGenericType(builder.UserType), userStoreType);
            builder.Services.TryAddScoped(typeof(IRoleStore<>).MakeGenericType(builder.RoleType), roleStoreType);

            return builder;
        }

        /// <summary>
        ///     Adds a Cosmos implementation of identity information stores.
        /// </summary>
        /// <typeparam name="TUserClaim">The type representing a user claim.</typeparam>
        /// <typeparam name="TUserRole">The type representing a user role.</typeparam>
        /// <typeparam name="TUserLogin">The type representing a user external login.</typeparam>
        /// <typeparam name="TUserToken">The type representing a user token.</typeparam>
        /// <typeparam name="TRoleClaim">The type representing a role claim.</typeparam>
        /// <param name="builder">The <see cref="IdentityBuilder"/> instance this method extends.</param>
        /// <returns>The <see cref="IdentityBuilder"/> instance this method extends.</returns>
        public static IdentityBuilder AddCosmosStores<TUserClaim, TUserRole, TUserLogin, TUserToken, TRoleClaim>(this IdentityBuilder builder)
            where TUserClaim : IdentityUserClaim, new()
            where TUserRole : IdentityUserRole, new()
            where TUserLogin : IdentityUserLogin, new()
            where TUserToken : IdentityUserToken, new()
            where TRoleClaim : IdentityRoleClaim, new()
        {
            if (!typeof(IdentityUser).IsAssignableFrom(builder.UserType))
            {
                throw new InvalidOperationException($"{builder.UserType.Name} must extend {typeof(IdentityUser).FullName}.");
            }
            if (!typeof(IdentityUserClaim).IsAssignableFrom(typeof(TUserClaim)))
            {
                throw new InvalidOperationException($"{typeof(TUserClaim).Name} must extend {typeof(IdentityUserClaim).FullName}.");
            }
            if (!typeof(IdentityUserRole).IsAssignableFrom(typeof(TUserRole)))
            {
                throw new InvalidOperationException($"{typeof(TUserRole).Name} must extend {typeof(IdentityUserRole).FullName}.");
            }
            if (!typeof(IdentityUserLogin).IsAssignableFrom(typeof(TUserLogin)))
            {
                throw new InvalidOperationException($"{typeof(TUserLogin).Name} must extend {typeof(IdentityUserLogin).FullName}.");
            }
            if (!typeof(IdentityUserToken).IsAssignableFrom(typeof(TUserToken)))
            {
                throw new InvalidOperationException($"{typeof(TUserToken).Name} must extend {typeof(IdentityUserToken).FullName}.");
            }
            if (!typeof(IdentityRole).IsAssignableFrom(builder.RoleType))
            {
                throw new InvalidOperationException($"{builder.RoleType.Name} must extend {typeof(IdentityRole).FullName}.");
            }
            if (!typeof(IdentityRoleClaim).IsAssignableFrom(typeof(TRoleClaim)))
            {
                throw new InvalidOperationException($"{typeof(TRoleClaim).Name} must extend {typeof(IdentityRoleClaim).FullName}.");
            }

            Type userStoreType = typeof(UserStore<,,,,,,>).MakeGenericType(
                builder.UserType, 
                builder.RoleType, 
                typeof(TUserClaim), 
                typeof(TUserRole), 
                typeof(TUserLogin), 
                typeof(TUserToken), 
                typeof(TRoleClaim));

            Type roleStoreType = typeof(RoleStore<,,>).MakeGenericType(
                builder.RoleType,
                typeof(TUserRole),
                typeof(TRoleClaim));

            builder.Services.TryAddScoped<ICosmosIdentityStorageProvider, CosmosIdentityStorageProvider>();
            builder.Services.TryAddScoped(typeof(IUserStore<>).MakeGenericType(builder.UserType), userStoreType);
            builder.Services.TryAddScoped(typeof(IRoleStore<>).MakeGenericType(builder.RoleType), roleStoreType);

            return builder;
        }
    }
}
