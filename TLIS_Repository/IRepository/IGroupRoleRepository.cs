using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModels.GroupRoleDTOs;
using TLIS_DAL.ViewModels.RoleDTOs;
using TLIS_Repository.Base;

namespace TLIS_Repository.IRepository
{
    public interface IGroupRoleRepository:IRepositoryBase<TLIgroupRole, GroupRoleViewModel,int>
    {
        Task<IEnumerable<TLIgroupRole>> GetGroupsByRoleId(int RoleId);
        Task<IEnumerable<TLIgroupRole>> GetRolesByGroupId(int GroupId);
        void EditGroupRoles(int GroupId,List<RoleViewModel> RolesId);
        void AddGroupRoles(int GroupId, List<RoleViewModel> RolesId);

        List<int> GetGroupRolesId(int GroupId);
    }
}
