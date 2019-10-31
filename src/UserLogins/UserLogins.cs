// © 2019 Mobsites. All rights reserved.
// Licensed under the MIT License.

using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Mobsites.AspNetCore.Identity.Cosmos
{
    public class UserLogins<TUserLogin> : IUserLogins<TUserLogin>
        where TUserLogin : IdentityUserLogin
    {
        public Task AddAsync(TUserLogin userLogin, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<TUserLogin> FindAsync(string loginProvider, string providerKey, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<TUserLogin> FindAsync(string userId, string loginProvider, string providerKey, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<IList<UserLoginInfo>> GetLoginsAsync(string userId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task RemoveAsync(TUserLogin userLogin, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
