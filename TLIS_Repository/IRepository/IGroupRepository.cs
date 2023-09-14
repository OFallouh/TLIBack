using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModels.GroupDTOs;
using TLIS_DAL.ViewModels.RoleDTOs;
using TLIS_DAL.ViewModels.UserDTOs;
using TLIS_Repository.Base;

namespace TLIS_Repository.IRepository
{
    public interface IGroupRepository:IRepositoryBase<TLIgroup, GroupViewModel,int>
    {
        bool ValidateGroupNameFromADAdd(string GroupName, string domain);
        bool ValidateGroupNameFromDatabaseAdd(string GroupName);
        Task<bool> ValidateGroupNameFromDatabaseUpdate(string GroupName,int GroupId);
        Task UpdateGroupRoles(List<RoleViewModel> roles, List<int> AllChildsIds);
        Task UpdateGroupUsers(List<UserNameViewModel> users, int groupId);
        Task DeleteGroup(int GroupId);
        void GetGroupsTest<TEntity>(params string[] Values);
    }
}
