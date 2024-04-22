using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TLIS_DAL.Helper;
using TLIS_DAL.Helper.Filters;
using TLIS_DAL.Helpers;
using TLIS_DAL.ViewModelBase;
using TLIS_DAL.ViewModels.CivilNonSteelDTOs;
using TLIS_DAL.ViewModels.CivilNonSteelLibraryDTOs;
using TLIS_DAL.ViewModels.CivilWithLegDTOs;
using TLIS_DAL.ViewModels.CivilWithLegLibraryDTOs;
using TLIS_DAL.ViewModels.CivilWithLegsDTOs;
using TLIS_DAL.ViewModels.CivilWithoutLegDTOs;
using TLIS_DAL.ViewModels.CivilWithoutLegLibraryDTOs;
using TLIS_DAL.ViewModels.DynamicAttDTOs;
using TLIS_DAL.ViewModels.DynamicAttInstValueDTOs;
using static TLIS_Service.Services.CivilLibraryService;

namespace TLIS_Service.IService
{
    public interface ICivilLibraryService
    {
        Response<GetForAddCivilLibrarybject> GetCivilNonSteelLibraryById(int Id, string TableName);
        Task<Response<EditCivilNonSteelLibraryObject>> EditCivilNonSteelLibrary(EditCivilNonSteelLibraryObject editCivilNonSteelLibraryObject, string TableName, int userId);
        Response<GetForAddCivilLibrarybject> GetForAddCivilWithoutCapsuleLibrary(string TableName);
        Response<AddCivilNonSteelLibraryObject> AddCivilNonSteelLibrary(string TableName, AddCivilNonSteelLibraryObject AddCivilNonSteelLibraryObject, string connectionString, int UserId);
        Response<AddCivilWithoutLegsLibraryObject> AddCivilWithoutLegsLibrary(string TableName, AddCivilWithoutLegsLibraryObject AddCivilWithoutLegsLibraryObject, string connectionString, int UserId);
        Response<GetForAddCivilLibrarybject> GetForAddCivilWithoutMonopleLibrary(string TableName);
        Response<GetForAddCivilLibrarybject> GetForAddCivilWithoutMastLibrary(string TableName);
        Response<ReturnWithFilters<CivilWithLegLibraryViewModel>> getCivilWithLegLibraries(List<FilterObjectList> filters, bool WithFilterData, ParameterPagination parameters);
        Response<ReturnWithFilters<CivilWithoutLegLibraryViewModel>> getCivilWithoutLegLibraries(List<FilterObjectList> filters, bool WithFilterData, ParameterPagination parameters);
        Response<ReturnWithFilters<CivilNonSteelLibraryViewModel>> getCivilNonSteelLibraries(List<FilterObjectList> filters, ParameterPagination parameters);
        Response<GetForAddCivilLibrarybject> GetCivilWithLegsLibraryById(int Id, string TableName);
        Response<GetForAddCivilLibrarybject> GetCivilWithoutLegsLibraryById(int Id, string TableName);
        Task<Response<EditCivilWithoutLegsLibraryObject>> EditCivilWithoutlegsLibrary(EditCivilWithoutLegsLibraryObject editCivilWithoutLegsLibraryObject, string TableName, int userId);
        Response<AddCivilWithLegsLibraryObject> AddCivilWithLegsLibrary(string TableName, AddCivilWithLegsLibraryObject AddCivilWithLegsLibraryObject, string connectionString,int UserId);
        Task<Response<EditCivilWithLegsLibraryObject>> EditCivilWithLegsLibrary(EditCivilWithLegsLibraryObject editCivilWithLegsLibrary, string TableName, int userId);
        Task<Response<AllItemAttributes>> Disable(int Id,string CivilType);
        Task<Response<AllItemAttributes>> Delete(int Id, string CivilType);
        Response<IEnumerable<LibraryNamesViewModel>> GetCivilLibraryByType(string CivilType, int? CivilWithoutLegCategoryId = null);
        Response<GetForAddCivilLibrarybject> GetForAdd(string TableName);
        Response<GetEnableAttribute> GetCivilWithLegLibrariesEnabledAtt( string ConnectionString);
        Response<GetEnableAttribute> GetCivilWithoutLegMastLibrariesEnabledAtt( string ConnectionString);
        Response<GetEnableAttribute> GetCivilWithoutLegCapsuleLibrariesEnabledAtt( string ConnectionString);
        Response<GetEnableAttribute> GetCivilWithoutLegMonopoleLibrariesEnabledAtt(string ConnectionString);
        Response<GetEnableAttribute> GetCivilNonSteelLibrariesEnabledAtt(string ConnectionString);
    }
}
