using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Engineering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLIS_DAL;
using TLIS_DAL.Helpers;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModels.PermissionDTOs;
using TLIS_DAL.ViewModels.UserPermissionDTOs;
using TLIS_Repository.Base;
using TLIS_Service.IService;

namespace TLIS_Service.Services
{
    public class PermissionService : IPermissionService
    {
        IUnitOfWork _unitOfWork;
        IServiceCollection _services;
        private readonly ApplicationDbContext _dbContext;
        private IMapper _mapper;

        public PermissionService(IUnitOfWork unitOfWork, IServiceCollection services, ApplicationDbContext context, IMapper mapper)
        {
            _dbContext = context;
            _unitOfWork = unitOfWork;
            _services = services;
            ServiceProvider serviceProvider = _services.BuildServiceProvider();
            _mapper = mapper;
        }
        //Function take 1 parameter addPermission
        //Function check if ControllerName and ActionName are already exist then return error message
        //Else add permission
        public async Task<Response<PermissionViewModel>> AddPermission(AddPermissionViewModel addPermission)
        {
            try
            {
                //var Check = _unitOfWork.PermissionRepository.GetWhereFirst(x => x.ControllerName == addPermission.ControllerName && x.ActionName == addPermission.ActionName);
                //if(Check != null)
                //{
                //    return new Response<PermissionViewModel>(true, null, null, "This permission has Controller and Action already exists", (int)Helpers.Constants.ApiReturnCode.fail);
                //}
                //await _unitOfWork.PermissionRepository.AddAsync(_mapper.Map<TLIpermission>(addPermission));
                //await _unitOfWork.SaveChangesAsync();
                return new Response<PermissionViewModel>();
            }
            catch (Exception err)
            {
                return new Response<PermissionViewModel>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
        }
        //Function return All permissions
        public Response<IEnumerable<PermissionViewModel>> GetPermissions()
        {
            try
            {
                IEnumerable<TLIpermission> Permissions = _unitOfWork.PermissionRepository.GetAllWithoutCount();
                IEnumerable<PermissionViewModel> PermissionsModels = _mapper.Map<IEnumerable<PermissionViewModel>>(Permissions);
                return new Response<IEnumerable<PermissionViewModel>>(true, PermissionsModels, null, null, (int)Helpers.Constants.ApiReturnCode.success, PermissionsModels.Count());
            }
            catch (Exception err)
            {
                return new Response<IEnumerable<PermissionViewModel>>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
        }
        public Response<List<PermissionFor_WFViewModel>> GetAllPermissionsFor_WF()
        {
            try
            {
                var Permissions = _unitOfWork.PermissionRepository.GetAllWithoutCount().ToList();
                var PermissionsModels = _mapper.Map<List<PermissionFor_WFViewModel>>(Permissions);
                return new Response<List<PermissionFor_WFViewModel>>(true, PermissionsModels, null, null, (int)Helpers.Constants.ApiReturnCode.success, PermissionsModels.Count());
            }
            catch (Exception err)
            {
                return new Response<List<PermissionFor_WFViewModel>>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
        }
        public Response<List<PermissionViewModel>> GetPermissionsForUser(int UserId)
        {
            try
            {
                List<int> UserPermissions = _unitOfWork.UserPermissionRepository.GetIncludeWhere(x => x.Active && !x.Deleted && x.userId == UserId).Select(x => x.permissionId).Distinct().ToList();
                List<PermissionViewModel> Permission2 = _mapper.Map<List<PermissionViewModel>>(_unitOfWork.PermissionRepository.GetWhere(x => UserPermissions.Any(y => y == x.Id)).ToList());

                return new Response<List<PermissionViewModel>>(true, Permission2, null, null, (int)Helpers.Constants.ApiReturnCode.success);
            }
            catch (Exception err)
            {
                return new Response<List<PermissionViewModel>>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
        }
        public Response<List<ModulesNamesViewModel>> GetAllModulesNames()
        {
            try
            {
                return new Response<List<ModulesNamesViewModel>>();
                //List<ModulesNamesViewModel> OutPut = _mapper.Map<List<ModulesNamesViewModel>>(_dbContext.TLIpermission.Where(x => x.Active).Select(x => x.Module).Distinct().Select(x => new TLIpermission { Module = x }).ToList());
                //return new Response<List<ModulesNamesViewModel>>(true, OutPut, null, null, (int)Helpers.Constants.ApiReturnCode.success);
            }
            catch (Exception err)
            {
                return new Response<List<ModulesNamesViewModel>>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
        }
        public Response<List<PermissionViewModel>> GetPermissionsByModuleName(string ModuleName)
        {
            try
            {
                return new Response<List<PermissionViewModel>>();
                //List<PermissionViewModel> Permissions = _mapper.Map<List<PermissionViewModel>>(_unitOfWork.PermissionRepository.GetWhere(x => x.Module == ModuleName).ToList());
                //return new Response<List<PermissionViewModel>>(true, Permissions, null, null, (int)Helpers.Constants.ApiReturnCode.success);
            }
            catch (Exception err)
            {
                return new Response<List<PermissionViewModel>>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
        }
        public Response<List<PermissionViewModel>> GetPermissionsByName(string PermissionName)
        {
            try
            {
                if (String.IsNullOrEmpty(PermissionName))
                {
                    var permissions = _unitOfWork.PermissionRepository.GetAllAsQueryable();
                    List<PermissionViewModel> permissionsModels = _mapper.Map<List<PermissionViewModel>>(permissions);
                    return new Response<List<PermissionViewModel>>(true, permissionsModels, null, null, (int)Helpers.Constants.ApiReturnCode.success, permissionsModels.Count());
                }
                else
                {
                    return new Response<List<PermissionViewModel>>();
                    //List<TLIpermission> Permissions = _unitOfWork.PermissionRepository.GetWhere(x => x.Name.ToLower().StartsWith(PermissionName.ToLower())).ToList();
                    //List<PermissionViewModel> PermissionsModels = _mapper.Map<List<PermissionViewModel>>(Permissions);
                    //return new Response<List<PermissionViewModel>>(true, PermissionsModels, null, null, (int)Helpers.Constants.ApiReturnCode.success, PermissionsModels.Count());
                }
            }
            catch (Exception err)
            {
                return new Response<List<PermissionViewModel>>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
        }

    }
}