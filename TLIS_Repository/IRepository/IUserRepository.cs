using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModels.UserDTOs;
using TLIS_Repository.Base;

namespace TLIS_Repository.IRepository
{
    public interface IUserRepository : IRepositoryBase<TLIuser, UserViewModel, int>
    {
    }
}
