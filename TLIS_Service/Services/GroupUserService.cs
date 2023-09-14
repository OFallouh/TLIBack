using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TLIS_DAL.Helpers;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModels.GroupUserDTOs;
using TLIS_Repository.Base;
using TLIS_Service.IService;

namespace TLIS_Service.Services
{
    public class GroupUserService: IGroupUserService
    {
        IUnitOfWork _unitOfWork;
        IServiceCollection _services;
        private IMapper _mapper;
        public GroupUserService(IUnitOfWork unitOfWork, IServiceCollection services,IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _services = services;
            ServiceProvider serviceProvider = _services.BuildServiceProvider();
            _mapper = mapper;
        }
        //public void AddGroupUsers(string GroupId, List<string> UsersId)
        //{

        //}
        //Function return group users
        public async Task<Response<IEnumerable<GroupUserViewModel>>> GetAllGroupUsers()
        {
            try
            {
                var GroupUsers = await _unitOfWork.GroupUserRepository.GetAllAsync();
                var GroupUsersModel = _mapper.Map<List<GroupUserViewModel>>(GroupUsers);
                return new Response<IEnumerable<GroupUserViewModel>>(true, GroupUsersModel, null, null, (int)Helpers.Constants.ApiReturnCode.success, GroupUsersModel.Count);
            }
            catch(Exception err)
            {
                
                return new Response<IEnumerable<GroupUserViewModel>>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
        }
        //Function take 1 parameter UserId
        //Function return list of groups by user Id
        public async Task<Response<IEnumerable<GroupUserViewModel>>> GetGroupsByUserId(int UserId)
        {
            try
            {
                var UserGroups = await _unitOfWork.GroupUserRepository.GetGroupsByUserId(UserId);
                var UserGroupsModel = _mapper.Map<IEnumerable<GroupUserViewModel>>(UserGroups);
                return new Response<IEnumerable<GroupUserViewModel>>(true, UserGroupsModel, null, null, (int)Helpers.Constants.ApiReturnCode.success);
            }
            catch(Exception err)
            {
                return new Response<IEnumerable<GroupUserViewModel>>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
        }
        //Function take 1 parameter GroupId
        //Function return users by GroupId
        public async Task<Response<IEnumerable<GroupUserViewModel>>> GetUsersByGroupId(int GroupId)
        {
            try
            {
                var GroupUsers = await _unitOfWork.GroupUserRepository.GetUsersByGroupId(GroupId);
                var GroupUsersModel = _mapper.Map<IEnumerable<GroupUserViewModel>>(GroupUsers);
                return new Response<IEnumerable<GroupUserViewModel>>(true, GroupUsersModel, null, null, (int)Helpers.Constants.ApiReturnCode.success);
            }
            catch(Exception err)
            {
                
                return new Response<IEnumerable<GroupUserViewModel>>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
            
        }
    }
}
