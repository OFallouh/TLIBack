using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModels.GroupUserDTOs;
using TLIS_Repository.Base;

namespace TLIS_Repository.IRepository
{
    public interface IGroupUserRepository:IRepositoryBase<TLIgroupUser, GroupUserViewModel,int>
    {
        Task<IEnumerable<TLIgroupUser>> GetGroupsByUserId(int UserId);
        Task<IEnumerable<TLIgroupUser>> GetUsersByGroupId(int GroupId);
        void EditGroupUsers(int groupId,List<int> UsersId);
    }
}
