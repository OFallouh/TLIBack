using System;
using System.Collections.Generic;
using System.Text;
using TLIS_DAL.Helpers;
using TLIS_DAL.ViewModels.UserDTOs;
using TLIS_DAL.ViewModels.UserPermissionDTOs;

namespace TLIS_Service.IService
{
    public interface IUserPermissionService
    {
        void EditUserPermissionDependOnRolePermission(List<int> UserIds, Dictionary<int, bool> permissions);
        Response<UserViewModel> GetUserPermissionsByUserId(int UserId);
    }
}
