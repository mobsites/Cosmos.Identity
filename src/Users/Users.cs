// © 2019 Mobsites. All rights reserved.
// Licensed under the MIT License.

using Microsoft.AspNetCore.Identity;
using Microsoft.Azure.Cosmos;
using System;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Mobsites.AspNetCore.Identity.Cosmos
{
    /// <summary>
    ///     Represents a new instance of a persistence store for the identity users.
    /// </summary>
    /// <typeparam name="TUser">The type representing a user.</typeparam>
    public class Users<TUser> : IUsers<TUser>
        where TUser : IdentityUser
    {
        #region Setup

        private readonly ICosmos cosmos;

        /// <summary>
        ///     Constructs a new instance of <see cref="Users{TUser}"/>.
        /// </summary>
        /// <param name="cosmos">The context in which to access the Cosmos Container for the identity store.</param>
        public Users(ICosmos cosmos)
        {
            this.cosmos = cosmos ?? throw new ArgumentNullException(nameof(cosmos));
        }

        #endregion

        /// <summary>
        ///     A navigation property for the users the store contains.
        /// </summary>
        public IQueryable<TUser> Queryable => cosmos.IdentityContainer.GetItemLinqQueryable<TUser>(allowSynchronousQueryExecution: true);

        #region Create User

        /// <summary>
        ///     Creates the specified <paramref name="user"/> in the store.
        /// </summary>
        /// <param name="user">The user to create.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        ///     The <see cref="Task"/> that represents the asynchronous operation, containing the <see cref="IdentityResult"/> of the creation operation.
        /// </returns>
        public async Task<IdentityResult> CreateAsync(TUser user, CancellationToken cancellationToken)
        {
            IdentityResult result;

            cancellationToken.ThrowIfCancellationRequested();

            if (user is null)
            {
                result = IdentityResult.Failed(new IdentityError() { Code = HttpStatusCode.BadRequest.ToString(), Description = $"Argument cannot be null." });
            }
            else
            {
                try
                {
                    var partitionKey = string.IsNullOrEmpty(user.PartitionKey) ? PartitionKey.None : new PartitionKey(user.PartitionKey);

                    var response = await cosmos.IdentityContainer.CreateItemAsync(user, partitionKey, null, cancellationToken);

                    result = response.StatusCode >= HttpStatusCode.BadRequest ?
                        IdentityResult.Failed(new IdentityError() { Code = response.StatusCode.ToString(), Description = $"The user {user.UserName} was not created." }) :
                        IdentityResult.Success;
                }
                catch (CosmosException ex)
                {
                    result = IdentityResult.Failed(new IdentityError() { Code = ex.StatusCode.ToString(), Description = ex.Message });
                }
            }

            return result;
        }

        #endregion

        #region Update User

        /// <summary>
        ///     Updates the specified <paramref name="user"/> in the store.
        /// </summary>
        /// <param name="user">The user to update.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        ///     The <see cref="Task"/> that represents the asynchronous operation, containing the <see cref="IdentityResult"/> of the update operation.
        /// </returns>
        public async Task<IdentityResult> UpdateAsync(TUser user, CancellationToken cancellationToken)
        {
            IdentityResult result;

            cancellationToken.ThrowIfCancellationRequested();

            if (user is null)
            {
                result = IdentityResult.Failed(new IdentityError() { Code = HttpStatusCode.BadRequest.ToString(), Description = $"Argument cannot be null." });
            }
            else
            {
                try
                {
                    var partitionKey = string.IsNullOrEmpty(user.PartitionKey) ? PartitionKey.None : new PartitionKey(user.PartitionKey);

                    var response = await cosmos.IdentityContainer.ReplaceItemAsync(user, user.Id, partitionKey, null, cancellationToken);

                    result = response.StatusCode >= HttpStatusCode.BadRequest ?
                        IdentityResult.Failed(new IdentityError() { Code = response.StatusCode.ToString(), Description = $"The user {user.UserName} was not updated." }) :
                        IdentityResult.Success;
                }
                catch (CosmosException ex)
                {
                    result = IdentityResult.Failed(new IdentityError() { Code = ex.StatusCode.ToString(), Description = ex.Message });
                }
            }

            return result;
        }

        #endregion

        #region Delete User

        /// <summary>
        ///     Deletes the specified <paramref name="user"/> from the store.
        /// </summary>
        /// <param name="user">The user to delete.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        ///     The <see cref="Task"/> that represents the asynchronous operation, containing the <see cref="IdentityResult"/> of the update operation.
        /// </returns>
        public async Task<IdentityResult> DeleteAsync(TUser user, CancellationToken cancellationToken)
        {
            IdentityResult result;

            cancellationToken.ThrowIfCancellationRequested();

            if (user is null)
            {
                result = IdentityResult.Failed(new IdentityError() { Code = HttpStatusCode.BadRequest.ToString(), Description = $"Argument cannot be null." });
            }
            else
            {
                try
                {
                    var partitionKey = string.IsNullOrEmpty(user.PartitionKey) ? PartitionKey.None : new PartitionKey(user.PartitionKey);

                    var response = await cosmos.IdentityContainer.DeleteItemAsync<TUser>(user.Id, partitionKey, null, cancellationToken);

                    result = response.StatusCode >= HttpStatusCode.BadRequest ?
                        IdentityResult.Failed(new IdentityError() { Code = response.StatusCode.ToString(), Description = $"The user {user.UserName} was not deleted." }) :
                        IdentityResult.Success;
                }
                catch (CosmosException ex)
                {
                    result = IdentityResult.Failed(new IdentityError() { Code = ex.StatusCode.ToString(), Description = ex.Message });
                }
            }

            return result;
        }

        #endregion

        #region Find User

        /// <summary>
        ///     Finds and returns a user, if any, who has the specified <paramref name="userId"/>.
        /// </summary>
        /// <param name="userId">The user ID to search for.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        ///     The <see cref="Task"/> that represents the asynchronous operation, containing the user matching the specified <paramref name="userId"/> if it exists.
        /// </returns>
        public async Task<TUser> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            TUser user = null;

            cancellationToken.ThrowIfCancellationRequested();

            if (!string.IsNullOrEmpty(userId))
            {
                try
                {
                    var partitionKey = string.IsNullOrEmpty(user.PartitionKey) ? PartitionKey.None : new PartitionKey(user.PartitionKey);

                    user = await cosmos.IdentityContainer.ReadItemAsync<TUser>(userId, partitionKey, null, cancellationToken);
                }
                catch (CosmosException)
                {
                    
                }
            }

            return user;
        }


        /// <summary>
        ///     Finds and returns a user, if any, who has the specified <paramref name="normalizedUserName"/>.
        /// </summary>
        /// <param name="normalizedUserName">The normalized user name to search for.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        ///     The <see cref="Task"/> that represents the asynchronous operation, containing the user matching the specified <paramref name="normalizedUserName"/> if it exists.
        /// </returns>
        public Task<TUser> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            TUser user = null;

            cancellationToken.ThrowIfCancellationRequested();

            if (!string.IsNullOrEmpty(normalizedUserName))
            {
                try
                {
                    user = cosmos.IdentityContainer.GetItemLinqQueryable<TUser>(allowSynchronousQueryExecution: true)
                        .Where(u => u.NormalizedUserName == normalizedUserName)
                        .ToList()
                        .FirstOrDefault();
                }
                catch (CosmosException)
                {
                    
                }
            }

            return Task.FromResult(user);
        }


        /// <summary>
        ///     Finds and returns a user, if any, who has the specified <paramref name="normalizedEmail"/>.
        /// </summary>
        /// <param name="normalizedEmail">The normalized email address to return the user for.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        ///      The <see cref="Task"/> that represents the asynchronous operation, containing the user matching the specified <paramref name="normalizedEmail"/> if it exists.
        /// </returns>
        public Task<TUser> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken)
        {
            TUser user = null;

            cancellationToken.ThrowIfCancellationRequested();

            if (!string.IsNullOrEmpty(normalizedEmail))
            {
                try
                {
                    user = cosmos.IdentityContainer.GetItemLinqQueryable<TUser>(allowSynchronousQueryExecution: true)
                        .Where(u => u.NormalizedEmail == normalizedEmail)
                        .ToList()
                        .FirstOrDefault();
                }
                catch (CosmosException)
                {
                    
                }
            }

            return Task.FromResult(user);
        }

        #endregion
    }
}
