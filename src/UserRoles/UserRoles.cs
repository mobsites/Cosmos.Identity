// © 2019 Mobsites. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Mobsites.AspNetCore.Identity.Cosmos
{
    public class UserRoles<TUserRole> : IUserRoles<TUserRole>
        where TUserRole : IdentityUserRole
    {
        public Task AddAsync(TUserRole userRole, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<TUserRole> FindAsync(string userId, string roleId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<IList<string>> GetRoleNamesAsync(string userId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<IList<TUser>> GetUsersAsync<TUser>(string roleId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task RemoveAsync(TUserRole userRole, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
