using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModels.NewPermissionsDTOs.Permissions;
using TLIS_DAL.ViewModels.NewPermissionsDTOs.RolePermissions;
using TLIS_Repository.Base;

namespace TLIS_Repository.IRepository
{
    public interface IRolePermissionsRepository : IRepositoryBase<TLIrolePermissions, NewRolePermissionsViewModel, int>
    {
    }
}
