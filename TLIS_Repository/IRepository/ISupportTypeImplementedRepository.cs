using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TLIS_DAL.Helper;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModels.CivilNonSteelDTOs;
using TLIS_DAL.ViewModels.SupportTypeImplementedDTOs;
using TLIS_Repository.Base;

namespace TLIS_Repository.IRepository
{
    public interface ISupportTypeImplementedRepository : IRepositoryBase<TLIsupportTypeImplemented, SupportTypeImplementedViewModel, int>
    {
    }
}
