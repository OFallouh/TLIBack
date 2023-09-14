using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TLIS_DAL.Helper;
using TLIS_DAL.Helper.Filters;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModels.CityDTOs;

namespace TLIS_Service.IService
{
    public interface ICityService
    {
        Task<IEnumerable<TLIcity>> GetCitiess(ParameterPagination parameterPagination);
        Task<IEnumerable<TLIcity>> GetCitiesRep(ParameterPagination parameterPagination);
        Task<IEnumerable<TLIcity>> GetCitiess(ParameterPagination parameterPagination, List<FilterObjectList> filters);

        Task AddCity(AddCityViewModel addCityViewModel);
        IEnumerable<TLIcity> whereCity(List<FilterObjectList> filter);
        IEnumerable<TLIcity> GetCitiess2();

    }
}
