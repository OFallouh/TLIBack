using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TLIS_DAL.Helpers;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModels.GroupUserDTOs;

namespace TLIS_Service.IService
{
    public interface IGroupUserService
    {
        Task<Response<IEnumerable<GroupUserViewModel>>> GetAllGroupUsers();
        Task<Response<IEnumerable<GroupUserViewModel>>> GetGroupsByUserId(int UserId);
        Task<Response<IEnumerable<GroupUserViewModel>>> GetUsersByGroupId(int GroupId);
         //void AddGroupUsers(string GroupId, List<string> UsersId);
    }
}
