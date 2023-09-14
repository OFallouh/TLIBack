using System;
using System.Collections.Generic;
using System.Text;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModels.UserDTOs;
using TLIS_Repository.Base;

namespace TLIS_Repository.IRepository
{
    public interface ITokenRepository:IRepositoryBase<TLIuser,UserViewModel,int>
    {
        UserViewModel Authenticate(LoginViewModel login, out string ErrorMessage,string domain,string domainGroup);
        UserViewModel InternalAuthenticate(LoginViewModel login, out string ErrorMessage, string domain, string domainGroup);

    }
}
