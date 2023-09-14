using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TLIS_DAL.Helper;
using TLIS_DAL.Helper.Filters;
using TLIS_DAL.Helpers;
using TLIS_DAL.ViewModelBase;
using TLIS_DAL.ViewModels.LoadOtherLibraryDTOs;
using TLIS_DAL.ViewModels.RadioAntennaLibraryDTOs;
using TLIS_DAL.ViewModels.RadioOtherLibraryDTOs;
using TLIS_DAL.ViewModels.RadioRRULibraryDTOs;

namespace TLIS_Service.IService
{
    public interface IRadioLibraryService
    {
        Response<ReturnWithFilters<RadioAntennaLibraryViewModel>> GetRadioAntennaLibraries(ParameterPagination parameters, List<FilterObjectList> filters = null);
        Response<ReturnWithFilters<RadioRRULibraryViewModel>> GetRadioRRULibraries(ParameterPagination parameters, List<FilterObjectList> filters = null);
        Response<ReturnWithFilters<RadioOtherLibraryViewModel>> GetOtherRadioLibraries(ParameterPagination parameters, List<FilterObjectList> filters = null);
        Response<AllItemAttributes> GetById(int Id, string TableName);
        Response<AllItemAttributes> AddRadioLibrary(string TableName, object RadioLibraryViewModel, string connectionString);
        Task<Response<AllItemAttributes>> EditRadioLibrary(string TableName, object RadioLibraryViewModel);
        Task<Response<AllItemAttributes>> DisableRadioLibrary(string TableName, int Id);
        Task<Response<AllItemAttributes>> DeletedRadioLibrary(string TableName, int Id);
        Response<AllItemAttributes> GetForAdd(string TableName);
        Response<ReturnWithFilters<object>> GetRadioAntennaLibrariesWithEnabledAttribute(CombineFilters CombineFilters, ParameterPagination parameterPagination);
        Response<ReturnWithFilters<object>> GetRadioRRULibrariesWithEnabledAttribute(CombineFilters CombineFilters, ParameterPagination parameterPagination);
        Response<ReturnWithFilters<object>> GetRadioOtherLibrariesWithEnabledAttribute(CombineFilters CombineFilters, ParameterPagination parameterPagination);
    }
}
