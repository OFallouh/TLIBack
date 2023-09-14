using TLIS_DAL.Models;
using TLIS_DAL.ViewModels.UserPermissionDTOs;
using TLIS_DAL.ViewModels.UserPermissionssDTOs;
using TLIS_Repository.Base;

namespace TLIS_Repository.IRepository
{
    public interface IUserPermissionssRepository : IRepositoryBase<TLIuserPermissions, UserPermissionsViewModel, int>
    {
    }
}