using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TLIS_DAL.Helpers;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModels.RoleDTOs;
using TLIS_Repository.Base;
using TLIS_Service.IService;
using TLIS_Service.Helpers;
using System.Transactions;
using System.Linq;
using TLIS_DAL.Helper.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TLIS_DAL.ViewModels.GroupDTOs;
using TLIS_DAL.ViewModels.PermissionDTOs;
using Nancy;
using AutoMapper;

namespace TLIS_Service.Services
{
    public class RoleService : IRoleService
    {
        IUnitOfWork _unitOfWork;
        IServiceCollection _services;
        private IMapper _mapper;
        public RoleService(IUnitOfWork unitOfWork, IServiceCollection services, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _services = services;
            ServiceProvider serviceProvider = _services.BuildServiceProvider();
            _mapper = mapper;
        }
        //Function take 1 parameter
        //check the name of the role if already exists the return error message
        //else add role and permissions to the role
        public Response<RoleViewModel> AddRole(AddRoleViewModel addRole)
        {
            TLIrole CheckRoleNameIfAlreadyExist = _unitOfWork.RoleRepository.GetWhereFirst(x => !x.Deleted && x.Name.ToLower() == addRole.Name.ToLower());

            if (CheckRoleNameIfAlreadyExist != null)
            {
                return new Response<RoleViewModel>(true, null, null, $"Role {addRole.Name} is already exists in database", (int)Constants.ApiReturnCode.fail);
            }

            using (TransactionScope transaction = new TransactionScope())
            {
                TLIrole role = new TLIrole()
                {
                    Name = addRole.Name,
                    Deleted = false,
                    Active = true
                };

                _unitOfWork.RoleRepository.Add(role);
                _unitOfWork.SaveChanges();

                RoleViewModel RoleModel = _mapper.Map<RoleViewModel>(role);

                if (addRole.permissions != null ? addRole.permissions.Count != 0 : false)
                {
                    _unitOfWork.RolePermissionRepository.AddRolePermissionList(role.Id, addRole.permissions);
                }

                transaction.Complete();
                return new Response<RoleViewModel>(true, RoleModel, null, null, (int)Constants.ApiReturnCode.success);
            }
        }
        //Function check if there are groups take role 
        //Function return true or false
        public Response<bool> CheckRoleGroups(int RoleId)
        {
            try
            {
                var RoleGroups = _unitOfWork.GroupRoleRepository

                  .GetWhere(x => x.roleId == RoleId)
                  .ToList();
                if (RoleGroups.Count == 0)
                {
                    return new Response<bool>(true, false, null, null, (int)Helpers.Constants.ApiReturnCode.success);
                }
                return new Response<bool>(true, true, null, null, (int)Helpers.Constants.ApiReturnCode.success);
            }
            catch (Exception err)
            {

                return new Response<bool>(true, false, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
        }
        //Function check role name in database 
        //Function return true or false
        public bool CheckRoleNameInDatabaseAdd(string RoleName)
        {
            return _unitOfWork.RoleRepository.CheckRoleNameInDatabaseAdd(RoleName);
        }
        //Function check role name in database 
        //Function return true or false
        public bool CheckRoleNameInDatabaseUpdate(string RoleName, int RoleId)
        {
            return _unitOfWork.RoleRepository.CheckRoleNameInDatabaseUpdate(RoleName, RoleId);
        }
        //Function take 1 parameter
        //check if there are groups take that role
        //if not then delete the role
        //else delete all groupsrole for this role then delete role
        public Response<RoleViewModel> DeleteRole(int RoleId)
        {
            try
            {
                var RoleGroups = _unitOfWork.GroupRoleRepository.GetWhere(x => x.roleId == RoleId).ToList();
                if (RoleGroups.Count == 0)
                {
                    _unitOfWork.RoleRepository.DeleteRole(RoleId);
                    return new Response<RoleViewModel>(true, null, null, null, (int)Constants.ApiReturnCode.success);
                }
                else
                {
                    DeleteRoleGroups(RoleId);
                    return new Response<RoleViewModel>(true, null, null, null, (int)Constants.ApiReturnCode.success);
                }
            }
            catch (Exception err)
            {

                return new Response<RoleViewModel>(true, null, null, err.Message, (int)Constants.ApiReturnCode.fail);
            }
        }
        //delete groupsrole for specific role
        //delete role
        public Response<RoleViewModel> DeleteRoleGroups(int RoleId)
        {
            try
            {
                var RoleGroups = _unitOfWork.GroupRoleRepository.GetWhere(x => x.roleId == RoleId).ToList();
                foreach (var item in RoleGroups)
                {
                    item.Deleted = true;
                }
                // _unitOfWork.GroupRoleRepository.RemoveRangeItems(RoleGroups);
                _unitOfWork.SaveChanges();
                var Role = _unitOfWork.RoleRepository.GetByID(RoleId);
                Role.Deleted = true;
                // _unitOfWork.RoleRepository.RemoveItem(Role);
                _unitOfWork.SaveChanges();
                return new Response<RoleViewModel>();
            }
            catch (Exception err)
            {

                return new Response<RoleViewModel>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
        }
        //Function 1 parameter
        //Function check the name in database
        //if the name is already exists return error message 
        //get record by Id
        //update Entity
        //update permissions
        public async Task<Response<RoleViewModel>> EditRole(EditRoleViewModel editRole)
        {
            TLIrole CheckRoleNameIfExist = _unitOfWork.RoleRepository
                .GetWhereFirst(x => !x.Deleted && x.Name.ToLower() == editRole.Name.ToLower() && x.Id != editRole.Id);

            if (CheckRoleNameIfExist != null)
            {
                return new Response<RoleViewModel>(true, null, null, $"Role {editRole.Name} is already exists in database", (int)Constants.ApiReturnCode.fail);
            }

            TLIrole RoleEntity = _unitOfWork.RoleRepository.GetByID(editRole.Id);
            RoleEntity.Name = editRole.Name;

            _unitOfWork.RoleRepository.Update(RoleEntity);
            await _unitOfWork.SaveChangesAsync();

            RoleViewModel RoleModel = _mapper.Map<RoleViewModel>(RoleEntity);

            List<int> NewPermissions = editRole.permissions.Select(x => x.Id).ToList();
            List<int> OldPermissions = _unitOfWork.RolePermissionRepository.GetWhere(x => x.roleId == editRole.Id).Select(x => x.permissionId).ToList();
            List<int> DeletePermissions = OldPermissions.Except(NewPermissions).ToList();
            List<int> AddPermissions = NewPermissions.Except(OldPermissions).ToList();

            using (TransactionScope transaction = new TransactionScope())
            {
                if (DeletePermissions.Count > 0)
                {
                    List<TLIrolePermission> DeleteRolePermission = _unitOfWork.RolePermissionRepository.GetWhere(x =>
                        x.roleId == editRole.Id && DeletePermissions.Contains(x.permissionId)).ToList();
                    _unitOfWork.RolePermissionRepository.RemoveRangeItems(DeleteRolePermission);
                }
                if (AddPermissions.Count > 0)
                {
                    foreach (int PermissionId in AddPermissions)
                    {
                        TLIrolePermission NewRolePermission = new TLIrolePermission();
                        NewRolePermission.permissionId = PermissionId;
                        NewRolePermission.roleId = editRole.Id;
                        await _unitOfWork.RolePermissionRepository.AddAsync(NewRolePermission);
                    }
                }
                await _unitOfWork.SaveChangesAsync();
                transaction.Complete();
            }
            return new Response<RoleViewModel>(true, RoleModel, null, null, (int)Constants.ApiReturnCode.success);
        }
        //Function take 1 parameter
        //Function return all roles depened on filters
        public async Task<Response<IEnumerable<RoleViewModel>>> GetRoles(List<FilterObjectList> filters)
        {
            try
            {
                List<RoleViewModel> RolesModel = new List<RoleViewModel>();
                if (filters != null ? filters.Count() > 0 : false)
                    RolesModel = _mapper.Map<List<RoleViewModel>>(_unitOfWork.RoleRepository.GetWhere(x => x.Deleted != true && x.Name.ToLower()
                        .StartsWith(filters.FirstOrDefault().value.FirstOrDefault().ToString().ToLower())).OrderBy(x => x.Name).ToList());
                else
                    RolesModel = _mapper.Map<List<RoleViewModel>>(_unitOfWork.RoleRepository.GetWhere(x => x.Deleted != true).OrderBy(x => x.Name).ToList());

                return new Response<IEnumerable<RoleViewModel>>(true, RolesModel, null, null, (int)Helpers.Constants.ApiReturnCode.success);
            }
            catch (Exception err)
            {
                return new Response<IEnumerable<RoleViewModel>>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
        }
        public async Task<Response<IEnumerable<RoleViewModel>>> GetRolesFor_WF()
        {
            try
            {
                List<RoleViewModel> RolesModel = new List<RoleViewModel>();
                var res = _unitOfWork.RoleRepository.GetAllWithoutCount();
                RolesModel = _mapper.Map<List<RoleViewModel>>(res);
                return new Response<IEnumerable<RoleViewModel>>(true, RolesModel, null, null, (int)Helpers.Constants.ApiReturnCode.success);
            }
            catch (Exception err)
            {
                return new Response<IEnumerable<RoleViewModel>>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
        }
        public Response<List<RoleViewModel>> GetRoleByName(string RoleName)
        {
            try
            {
                if (String.IsNullOrEmpty(RoleName))
                {
                    List<RoleViewModel> RoleModel = _mapper.Map<List<RoleViewModel>>(_unitOfWork.RoleRepository.GetWhere(x => x.Active && !x.Deleted));
                    return new Response<List<RoleViewModel>>(true, RoleModel, null, null, (int)Helpers.Constants.ApiReturnCode.success, RoleModel.Count);
                }
                else
                {
                    List<RoleViewModel> rolemodel = _mapper.Map<List<RoleViewModel>>(_unitOfWork.RoleRepository.GetWhere(x =>
                       x.Name.ToLower().StartsWith(RoleName.ToLower()) && x.Active && !x.Deleted).ToList());
                    return new Response<List<RoleViewModel>>(true, rolemodel, null, null, (int)Helpers.Constants.ApiReturnCode.success, rolemodel.Count);
                }
            }
            catch (Exception err)
            {
                return new Response<List<RoleViewModel>>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail); ;
            }
        }
        public Response<RoleViewModel> GetRoleByRoleName(string RoleName)
        {
            RoleViewModel Response = _mapper.Map<RoleViewModel>(_unitOfWork.RoleRepository.GetWhereFirst(x =>
                x.Name.ToLower() == RoleName.ToLower()));

            Response.Permissions = _mapper.Map<List<PermissionViewModel>>(_unitOfWork.RolePermissionRepository
                .GetIncludeWhere(x => x.roleId == Response.Id && !x.Deleted && x.Active,
                    x => x.permission).ToList().Select(x => x.permission));

            return new Response<RoleViewModel>(true, Response, null, null, (int)Helpers.Constants.ApiReturnCode.success);
        }
    }
}
