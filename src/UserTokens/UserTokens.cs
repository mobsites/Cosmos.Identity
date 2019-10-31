// © 2019 Mobsites. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Mobsites.AspNetCore.Identity.Cosmos
{
    public class UserTokens<TUserToken> : IUserTokens<TUserToken>
        where TUserToken : IdentityUserToken
    {
        public Task AddAsync(TUserToken token)
        {
            throw new NotImplementedException();
        }

        public Task<TUserToken> FindAsync(string userId, string loginProvider, string name, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<IList<TUserToken>> GetTokensAsync(string userId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task RemoveAsync(TUserToken token)
        {
            throw new NotImplementedException();
        }
    }
}
