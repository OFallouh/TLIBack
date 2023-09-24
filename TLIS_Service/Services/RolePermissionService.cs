using AutoMapper;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Engineering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLIS_DAL.Helpers;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModels.GroupRoleDTOs;
using TLIS_DAL.ViewModels.PermissionDTOs;
using TLIS_DAL.ViewModels.RoleDTOs;
using TLIS_DAL.ViewModels.RolePermissionDTOs;
using TLIS_Repository.Base;
using TLIS_Service.IService;

namespace TLIS_Service.Services
{
    public class RolePermissionService : IRolePermissionService
    {
        IUnitOfWork _unitOfWork;
        IServiceCollection _services;
        private IMapper _mapper;
        public RolePermissionService(IUnitOfWork unitOfWork, IServiceCollection services, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _services = services;
            _mapper = mapper;
        }
        //Function take 2 parameters 
        //Function add permissions to specific role
        //if role already have permission then don't add the permission
        public void AddRolePermissionList(int RoleId,List<PermissionViewModel> permissions)
        {
            _unitOfWork.RolePermissionRepository.AddRolePermissionList(RoleId, permissions);
        }
        //Function take 2 parameters
        //Function change the permissions for specific role
        //Function take the permissions from users and find which permissions should add and which permissions should delete 
        public async Task EditRolePermissionList(int RoleId,List<PermissionViewModel> permissions, List<int> AffectedGroupsIds)
        {
            await _unitOfWork.RolePermissionRepository.EditRolePermissionList(RoleId, permissions, AffectedGroupsIds);
        }
        //Function take 1 parameter
        //get all permissions for specific role
        //public Response<RoleViewModel> GetAllPermissionsByRoleId(int RoleId)
        //{
        //    try
        //    {
        //        var roleEnitiy = _unitOfWork.RoleRepository.GetByID(RoleId);
        //        RoleViewModel role = _mapper.Map<RoleViewModel>(roleEnitiy);
        //        role.Permissions = _unitOfWork.RolePermissionRepository.GetAllPermissionsByRoleId(RoleId);
        //        return new Response<RoleViewModel>(true, role, null, null, (int)Helpers.Constants.ApiReturnCode.success);
        //    }
        //    catch(Exception err)
        //    {
                  
        //        return new Response<RoleViewModel>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
        //    } 
        //}
        //Function return all roles permissions
        public async Task<IEnumerable<TLIrolePermission>> GetAllRolePermissions()
        {
            return await _unitOfWork.RolePermissionRepository.GetAllAsync();
        }
        public Response<List<RolePermissionViewModel>>GetAllPermissionForW_F()
        {
            var result = _unitOfWork.RolePermissionRepository.GetAllAsQueryable();
            var res = _mapper.Map<List<RolePermissionViewModel>>(result);
            return new Response<List<RolePermissionViewModel>>(true, res, null, null, (int)Helpers.Constants.ApiReturnCode.success, res.Count());
        }
    }
}
