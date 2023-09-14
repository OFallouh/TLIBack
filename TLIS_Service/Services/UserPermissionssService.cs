using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLIS_DAL;
using TLIS_DAL.Helpers;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModels.UserPermissionssDTOs;
using TLIS_Repository.Base;
using TLIS_Service.IService;

namespace TLIS_Service.Services
{
    public class UserPermissionssService : IUserPermissionssService
    {
        IUnitOfWork _UnitOfWork;
        IServiceCollection _Services;
        private IMapper _Mapper;
        public UserPermissionssService(IUnitOfWork UnitOfWork, IServiceCollection Services, IMapper Mapper)
        {
            _UnitOfWork = UnitOfWork;
            _Services = Services;
            _Mapper = Mapper;
        }
        //JwtSecurityTokenHandler JWTHandler = new JwtSecurityTokenHandler();

        //int UserId = Convert.ToInt32(JWTHandler.ReadJwtToken(_HttpContextAccessor.HttpContext
        //    .Request.Headers["Authorization"].ToString().Split(" ")[1]).Claims.ToList()[0].Value);
        //public Response<List<UserPermissionsForLogin>> GetAllUserPermissionsAfterLogin(int UserId)
        //{
        //    // Check If UserId is Exist in DB..

        //    TLIuser? UserEntity = _UnitOfWork.UserRepository
        //        .GetWhereFirst(x => x.Id == UserId);

        //    if (UserEntity == null)
        //        return new Response<List<UserPermissionsForLogin>>();

        //    List<UserPermissionsForLogin>
        //}
    }
}
