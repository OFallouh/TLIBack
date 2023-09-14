using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TLIS_DAL.Helpers;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModels.GroupRoleDTOs;

namespace TLIS_Service.IService
{
    public interface IGroupRoleService
    {
        Task<Response<IEnumerable<GroupRoleViewModel>>> GetGroupRoles();
        Task<Response<IEnumerable<GroupRoleViewModel>>> GetGroupsByRoleId(int RoleId);
        Task<Response<IEnumerable<GroupRoleViewModel>>> GetRolesByGroupId(int GroupId);
        Task<Response<GroupRoleViewModel>> AddGroupRole(AddGroupRoleViewModel model);
        //void AddGroupRoles(string GroupId, List<string> RolesId);
    }
}
