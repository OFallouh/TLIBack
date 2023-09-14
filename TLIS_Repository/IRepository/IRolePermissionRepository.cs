using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModels.PermissionDTOs;
using TLIS_DAL.ViewModels.RolePermissionDTOs;
using TLIS_Repository.Base;

namespace TLIS_Repository.IRepository
{
    public interface IRolePermissionRepository:IRepositoryBase<TLIrolePermission, RolePermissionViewModel,int>
    {
        List<PermissionViewModel> GetAllPermissionsByRoleId(int RoleId);
        void AddRolePermissionList(int RoleId, List<PermissionViewModel> permissions);
        Task EditRolePermissionList(int RoleId, List<PermissionViewModel> permissions, List<int> AffectedGroupsIds);
        Task AddUserPermission(int RoleId, int permission, List<int> AffectedGroupsIds);
    }
}
