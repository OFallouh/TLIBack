using System;
using System.Collections.Generic;
using System.Text;
using TLIS_DAL.Helper;
using TLIS_DAL.Models;
using TLIS_DAL.Helper.Filters;
using System.Threading.Tasks;
using TLIS_DAL.Helpers;
using TLIS_DAL.ViewModels.CivilWithLegsDTOs;
using TLIS_DAL.ViewModels.CivilWithoutLegDTOs;

namespace TLIS_Service.IService
{
    public interface ICivilWithLegsService
    {

        Task<IEnumerable<TLIcivilWithLegs>> GetCivilWithLegs(ParameterPagination parameterPagination, CivilWithLegsFilter civilWithLegsFilter);
        
    }
}
