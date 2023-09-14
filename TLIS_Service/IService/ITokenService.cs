using System;
using System.Collections.Generic;
using System.Text;
using TLIS_DAL.Helpers;
using TLIS_DAL.ViewModels.UserDTOs;
using TLIS_DAL.ViewModels.UserPermissionssDTOs;

namespace TLIS_Service.IService
{
    public interface ITokenService
    {
        UserViewModel Authenticate(LoginViewModel login, out string ErrorMessage, string domain, string domainGroup);
        string BuildToken(UserViewModel user, string secretKey);
        Response<UserPermissionsForLogin> CreateToken(LoginViewModel login, string secretKey, string domain, string domainGroup);
        Response<string> CreateInternalToken(LoginViewModel login, string secretKey, string domain, string domainGroup);

    }
}
