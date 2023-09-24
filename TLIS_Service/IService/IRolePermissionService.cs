using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TLIS_DAL.Helpers;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModels.PermissionDTOs;
using TLIS_DAL.ViewModels.RoleDTOs;
using TLIS_DAL.ViewModels.RolePermissionDTOs;

namespace TLIS_Service.IService
{
    public interface IRolePermissionService
    {
        Task<IEnumerable<TLIrolePermission>> GetAllRolePermissions();
       // Response<RoleViewModel> GetAllPermissionsByRoleId(int RoleId);
        void AddRolePermissionList(int RoleId,List<PermissionViewModel> permissions);
        Task EditRolePermissionList(int RoleId,List<PermissionViewModel> permissions, List<int> AffectedGroupsIds);
        Response<List<RolePermissionViewModel>> GetAllPermissionForW_F();
    }
}
