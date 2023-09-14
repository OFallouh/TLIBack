using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TLIS_DAL.Helpers;
using TLIS_DAL.ViewModels.PermissionDTOs;

namespace TLIS_Service.IService
{
    public interface IPermissionService
    {
        Response<IEnumerable<PermissionViewModel>> GetPermissions();
        Task<Response<PermissionViewModel>> AddPermission(AddPermissionViewModel addPermission);
        Response<List<PermissionViewModel>> GetPermissionsForUser(int UserId);
        Response<List<PermissionViewModel>> GetPermissionsByName(string PermissionName);
        Response<List<ModulesNamesViewModel>> GetAllModulesNames();
        Response<List<PermissionViewModel>> GetPermissionsByModuleName(string ModuleName);
        Response<List<PermissionFor_WFViewModel>> GetAllPermissionsFor_WF();
    }
}
