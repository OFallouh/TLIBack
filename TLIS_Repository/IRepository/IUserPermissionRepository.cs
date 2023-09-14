using System;
using System.Collections.Generic;
using System.Text;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModels.PermissionDTOs;
using TLIS_DAL.ViewModels.UserPermissionDTOs;
using TLIS_Repository.Base;

namespace TLIS_Repository.IRepository
{
    public interface IUserPermissionRepository:IRepositoryBase<TLIuserPermission, UserPermissionViewModel,int>
    {
        IEnumerable<PermissionViewModel> GetUserPermissionsByUserId(int UserId);
    }
}
