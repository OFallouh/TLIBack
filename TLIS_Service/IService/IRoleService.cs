using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TLIS_DAL.Helper.Filters;
using TLIS_DAL.Helpers;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModels.GroupDTOs;
using TLIS_DAL.ViewModels.RoleDTOs;

namespace TLIS_Service.IService
{
    public interface IRoleService
    {
        Task<Response<IEnumerable<RoleViewModel>>> GetRoles(List<FilterObjectList> filters);
        Task<Response<RoleViewModel>> EditRole(EditRoleViewModel editRole,int UserId);
        Task<Response<RoleViewModel>> AddRole(AddRoleViewModel addRole, int UserId);
        bool CheckRoleNameInDatabaseAdd(string RoleName);
        bool CheckRoleNameInDatabaseUpdate(string RoleName, int RoleId);
        Response<RoleViewModel> DeleteRole(int RoleId, int UserId);
        Response<RoleViewModel> DeleteRoleGroups(int RoleId);
        Response<bool> CheckRoleGroups(int RoleId);
        Response<List<RoleViewModel>> GetRoleByName(string RoleName);
        Task<Response<IEnumerable<RoleViewModel>>> GetRolesFor_WF();
        Response<List<RoleViewModel>> GetRoleByRoleName(string RoleName);
    }
}
