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
using TLIS_DAL.ViewModels.UserDTOs;
using TLIS_DAL.ViewModels.UserPermissionDTOs;
using TLIS_Repository.Base;
using TLIS_Service.IService;

namespace TLIS_Service.Services
{
    public class UserPermissionService : IUserPermissionService
    {
        IUnitOfWork _unitOfWork;
        IServiceCollection _services;
        private IMapper _mapper;
        public UserPermissionService(IUnitOfWork unitOfWork, IServiceCollection services, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _services = services;
            ServiceProvider serviceProvider = _services.BuildServiceProvider();
            _mapper = mapper;
        }
        //Function take 2 parameters
        //function update user permissions
        public void EditUserPermissionDependOnRolePermission(List<int> UserIds, Dictionary<int, bool> permissions)
        {
            Parallel.ForEach(UserIds, UserId =>
            {
                var user = _unitOfWork.UserRepository.Get(UserId);
                Parallel.ForEach(permissions, permission =>
                {
                    //List<TLIuserPermission> UserPermissions = _unitOfWork.
                    TLIuserPermission Entity = new TLIuserPermission();
                    Entity.userId = UserId;
                    Entity.permissionId = permission.Key;
                    Entity.Active = permission.Value;
                    _unitOfWork.UserPermissionRepository.Update(Entity);
                    _unitOfWork.SaveChanges();
                });
            });
        }
        //Function get user permissions by UserId
        public Response<UserViewModel> GetUserPermissionsByUserId(int UserId)
        {
            try
            {
                UserViewModel User = new UserViewModel();
                TLIuser user = _unitOfWork.UserRepository.GetByID(UserId);

                if (user == null)
                    return new Response<UserViewModel>(true, null, null, $"No User Found With This Id: {UserId}", (int)Helpers.Constants.ApiReturnCode.fail, 0);

                User = _mapper.Map<UserViewModel>(user);
                // User.Permissions = _unitOfWork.UserPermissionRepository.GetUserPermissionsByUserId(UserId).ToList();
                return new Response<UserViewModel>(true, User, null, null, (int)Helpers.Constants.ApiReturnCode.success, 0);
            }
            catch (Exception err)
            {
                return new Response<UserViewModel>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail, 0);
            }
        }
    }
}
