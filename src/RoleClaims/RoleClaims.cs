// © 2019 Mobsites. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Mobsites.AspNetCore.Identity.Cosmos
{
    public class RoleClaims<TRoleClaim> : IRoleClaims<TRoleClaim>
        where TRoleClaim : IdentityRoleClaim
    {
        public Task AddAsync(TRoleClaim roleClaim, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<IList<TRoleClaim>> FindAsync(string roleId, Claim claim, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<IList<Claim>> GetClaimsAsync(string roleId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task RemoveAsync(TRoleClaim roleClaim, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
