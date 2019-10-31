// © 2019 Mobsites. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Mobsites.AspNetCore.Identity.Cosmos
{
    public class UserClaims<TUserClaim> : IUserClaims<TUserClaim>
        where TUserClaim : IdentityUserClaim
    {
        public Task AddAsync(TUserClaim userClaim, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<IList<TUserClaim>> FindAsync(string userId, Claim claim, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<IList<Claim>> GetClaimsAsync(string userId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<IList<TUser>> GetUsersAsync<TUser>(Claim claim, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task RemoveAsync(TUserClaim userClaim, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(TUserClaim userClaim, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
