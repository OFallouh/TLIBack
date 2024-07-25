using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TLIS_DAL.Helper;
using TLIS_DAL.Helper.Filters;
using TLIS_DAL.Helpers;
using TLIS_DAL.ViewModelBase;
using TLIS_DAL.ViewModels.CivilLoadsDTOs;
using TLIS_DAL.ViewModels.CivilWithLegLibraryDTOs;
using TLIS_DAL.ViewModels.LoadOtherLibraryDTOs;

namespace TLIS_Service.IService
{
    public interface ILoadOtherLibraryService
    {
        Response<ReturnWithFilters<LoadOtherLibraryViewModel>> GetLoadOtherLibraries(ParameterPagination parameters, List<FilterObjectList> filters);
        Response<GetForAddCivilLibrarybject> GetById(int Id, string TableName);
        Task<Response<EditLoadOtherLibraryObject>> EditLoadOtherLibrary(EditLoadOtherLibraryObject editLoadOtherLibraryObjectc, int UserId, string connectionString);
        Response<GetEnableAttribute> GetLoadOtherLibrariesEnabledAtt(string ConnectionString);
        Response<AddLoadOtherLibraryObject> AddLoadOtherLibrary( AddLoadOtherLibraryObject addLoadOtherLibraryObject, string connectionString, int UserId);
        Task<Response<AllItemAttributes>> DisableLoadOtherLibrary(int Id, int UserId);
        Task<Response<AllItemAttributes>> DeletedLoadOtherLibrary(int Id, int UserId);
        Response<GetForAddCivilLibrarybject> GetForAdd(string TableName);
        Response<ReturnWithFilters<object>> GetLoadOtherLibrariesWithEnableAtt(CombineFilters CombineFilters, ParameterPagination parameterPagination);
    }
}
