using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using TLIS_DAL.Helpers;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModels.NewPermissionsDTOs.Permissions;
using TLIS_DAL.ViewModels.PermissionDTOs;
using TLIS_DAL.ViewModels.UserDTOs;
using TLIS_DAL.ViewModels.UserPermissionssDTOs;
using TLIS_Repository.Base;
using TLIS_Service.IService;

namespace TLIS_Service.Services
{
    public class TokenService : ITokenService
    {
        IUnitOfWork _unitOfWork;
        IConfiguration _config;
        public TokenService(IUnitOfWork unitOfWork, IConfiguration config)
        {
            _unitOfWork = unitOfWork;
            _config = config;
        }
        //Function to authenticate login
        public UserViewModel Authenticate(LoginViewModel login, out string ErrorMessage, string domain, string domainGroup)
        {
            return _unitOfWork.TokenRepository.Authenticate(login, out ErrorMessage, domain, domainGroup);
        }
        public UserViewModel InternalAuthenticate(LoginViewModel login, out string ErrorMessage, string domain, string domainGroup)
        {
            return _unitOfWork.TokenRepository.InternalAuthenticate(login, out ErrorMessage, domain, domainGroup);
        }
        //Function build token depened on user data
        //Function return token
        public string BuildToken(UserViewModel user, string secretKey)
        {
            List<Claim> claims = new List<Claim> {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.FamilyName, user.LastName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken("https://localhost:44311/",
              "https://localhost:44311/",
              claims,
              expires: DateTime.Now.AddMinutes(30),
              signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        //Function take 1 parameter
        //first authenticate the user
        //if user is authenticate build token for user and return the token
        //else give error message The login failed
        public Response<UserPermissionsForLogin> CreateToken(LoginViewModel login, string secretKey, string domain, string domainGroup)
        {
            string ErrorMessage;
            UserViewModel CkeckUser = Authenticate(login, out ErrorMessage, domain, domainGroup);

            List<NewPermissionsViewModel> AllUserPermissions = new List<NewPermissionsViewModel>();

            if (CkeckUser != null)
            {
                string Token = BuildToken(CkeckUser, secretKey);

                List<NewPermissionsViewModel> UserPermissionsURLs = _unitOfWork.UserPermissionssRepository
                    .GetIncludeWhere(x => x.User_Id == CkeckUser.Id && x.IsActive, x => x.Permission)
                    .Select(x => new NewPermissionsViewModel()
                    {
                        Id = x.Id,
                        Page_URL = x.Permission.Page_URL
                    }).ToList();

                List<NewPermissionsViewModel> RolePermissionsURLs = new List<NewPermissionsViewModel>();
                List<NewPermissionsViewModel> GroupPermissionsURLs = new List<NewPermissionsViewModel>();

                List<TLIgroupUser> GroupUsers = _unitOfWork.GroupUserRepository.GetWhere(x => x.userId == CkeckUser.Id).ToList();

                if (GroupUsers != null ? GroupUsers.Count() > 0 : false)
                {
                    foreach (TLIgroupUser GroupUser in GroupUsers)
                    {
                        GroupPermissionsURLs = _unitOfWork.GroupPermissionsRepository
                            .GetIncludeWhere(x => x.GroupId == GroupUser.groupId && x.IsActive, x => x.Permission)
                            .Select(x => new NewPermissionsViewModel()
                            {
                                Id = x.Id,
                                Page_URL = x.Permission.Page_URL
                            }).ToList();

                        List<TLIgroupRole> GroupRoles = _unitOfWork.GroupRoleRepository.GetWhere(x => x.groupId == GroupUser.groupId).ToList();

                        if (GroupRoles != null ? GroupRoles.Count() > 0 : false)
                        {
                            foreach (TLIgroupRole GroupRole in GroupRoles)
                            {
                                RolePermissionsURLs = _unitOfWork.RolePermissionsRepository
                                    .GetIncludeWhere(x => x.RoleId == GroupRole.roleId && x.IsActive, x => x.Permission)
                                    .Select(x => new NewPermissionsViewModel()
                                    {
                                        Id = x.Id,
                                        Page_URL = x.Permission.Page_URL
                                    }).ToList();
                            }
                        }
                    }
                }

                AllUserPermissions = UserPermissionsURLs.Concat(GroupPermissionsURLs).Concat(RolePermissionsURLs)
                    .Distinct().ToList();

                return new Response<UserPermissionsForLogin>(true, new UserPermissionsForLogin()
                {
                    Token = Token,
                    Permissions = AllUserPermissions
                }, null, null, (int)Helpers.Constants.ApiReturnCode.success);
            }
            else
            {
                return new Response<UserPermissionsForLogin>(true, null, null, ErrorMessage, (int)Helpers.Constants.ApiReturnCode.uncompleted);
            }
        }

        public Response<string> CreateInternalToken(LoginViewModel login, string secretKey, string domain, string domainGroup)
        {
            Response<string> response = null;
            string ErrorMessage;
            var user = InternalAuthenticate(login, out ErrorMessage, domain, domainGroup);

            if (user != null)
            {
                var tokenString = BuildToken(user, secretKey);
                response = new Response<string>(true, tokenString, null, null, (int)Helpers.Constants.ApiReturnCode.success);
            }
            else
            {
                response = new Response<string>(true, null, null, ErrorMessage, (int)Helpers.Constants.ApiReturnCode.uncompleted);
            }

            return response;
        }













    }
}
