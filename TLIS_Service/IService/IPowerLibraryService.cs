using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TLIS_DAL.Helper;
using TLIS_DAL.Helper.Filters;
using TLIS_DAL.Helpers;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModelBase;
using TLIS_DAL.ViewModels.PowerDTOs;

namespace TLIS_Service.IService
{
    public interface IPowerLibraryService
    {
        Response<ReturnWithFilters<PowerLibraryViewModel>> GetPowerLibraries(ParameterPagination parameters, List<FilterObjectList> filters = null);
        Response<AllItemAttributes> GetById(int Id, string TableName);
        Response<AllItemAttributes> AddPowerLibrary(AddPowerLibraryViewModel PowerLibraryViewModel, string connectionString);
        Task<Response<AllItemAttributes>> EditPowerLibrary(string TableName, EditPowerLibraryViewModel PowerLibraryViewModel);
        Task<Response<AllItemAttributes>> DisablePowerLibrary(string TableName, int Id);
        Task<Response<AllItemAttributes>> DeletePowerLibrary(string TableName, int Id);
        Response<AllItemAttributes> GetForAdd(string TableName);
        Response<ReturnWithFilters<object>> GetPowerLibrariesWithEnableAttributes(CombineFilters CombineFilters, ParameterPagination parameterPagination);
    }
}
