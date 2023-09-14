using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLIS_DAL.Helpers;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModels.GroupRoleDTOs;
using TLIS_Repository.Base;
using TLIS_Service.IService;

namespace TLIS_Service.Services
{
    public class GroupRoleService : IGroupRoleService
    {
        IUnitOfWork _unitOfWork;
        IServiceCollection _services;
        private IMapper _mapper;
        public GroupRoleService(IUnitOfWork unitOfWork, IServiceCollection services,IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _services = services;
            _mapper = mapper;
        }
        //Function to add group role
        //First check if group role is already exist then return error message
        //Else add group role
        public async Task<Response<GroupRoleViewModel>> AddGroupRole(AddGroupRoleViewModel model)
        {
            try
            {
                TLIgroupRole GroupRoleEntity = _mapper.Map<TLIgroupRole>(model);
                var Check = _unitOfWork.GroupRoleRepository.GetWhereFirst(x => x.groupId == model.groupId && x.roleId == model.roleId);
                if(Check != null)
                {
                    return new Response<GroupRoleViewModel>(true, null, null, "This Group and Role is already exists", (int)Helpers.Constants.ApiReturnCode.fail);
                }
                await _unitOfWork.GroupRoleRepository.AddAsync(GroupRoleEntity);
                await _unitOfWork.SaveChangesAsync();
                return new Response<GroupRoleViewModel>();
            }
            catch(Exception err)
            {
                return new Response<GroupRoleViewModel>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
        }

        //Get all group roles
        public async Task<Response<IEnumerable<GroupRoleViewModel>>> GetGroupRoles()
        {
            try
            {
                IEnumerable<TLIgroupRole> GroupRole = await _unitOfWork.GroupRoleRepository.GetAllAsync();
                IEnumerable<GroupRoleViewModel> GroupRoleModel = _mapper.Map<IEnumerable<GroupRoleViewModel>>(GroupRole);
                return new Response<IEnumerable<GroupRoleViewModel>>(true, GroupRoleModel, null, null, (int)Helpers.Constants.ApiReturnCode.success, GroupRoleModel.Count());
            }
            catch(Exception err)
            {
                return new Response<IEnumerable<GroupRoleViewModel>>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
        }
        //Get groups by role Id
        public async Task<Response<IEnumerable<GroupRoleViewModel>>> GetGroupsByRoleId(int RoleId)
        {
            try
            {
                IEnumerable<TLIgroupRole> GroupRoles = await _unitOfWork.GroupRoleRepository.GetGroupsByRoleId(RoleId);
                IEnumerable<GroupRoleViewModel> GroupRolesModel = _mapper.Map<IEnumerable<GroupRoleViewModel>>(GroupRoles);
                return new Response<IEnumerable<GroupRoleViewModel>>(true, GroupRolesModel, null, null, (int)Helpers.Constants.ApiReturnCode.success);
            }
            catch(Exception err)
            {
                return new Response<IEnumerable<GroupRoleViewModel>>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            } 
        }
        //Get all roles by group Id
        public async Task<Response<IEnumerable<GroupRoleViewModel>>> GetRolesByGroupId(int GroupId)
        {
            try
            {
                IEnumerable<TLIgroupRole> GroupRoles = await _unitOfWork.GroupRoleRepository.GetRolesByGroupId(GroupId);
                IEnumerable<GroupRoleViewModel> GroupRolesModel = _mapper.Map<IEnumerable<GroupRoleViewModel>>(GroupRoles);
                return new Response<IEnumerable<GroupRoleViewModel>>(true, GroupRolesModel, null, null, (int)Helpers.Constants.ApiReturnCode.success);
            }
            catch(Exception err)
            {
                return new Response<IEnumerable<GroupRoleViewModel>>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
        }
    }
}
