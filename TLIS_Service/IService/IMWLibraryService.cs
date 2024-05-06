using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TLIS_DAL.Helper;
using TLIS_DAL.Helper.Filters;
using TLIS_DAL.Helpers;
using TLIS_DAL.ViewModelBase;
using TLIS_DAL.ViewModels.CivilWithLegLibraryDTOs;
using TLIS_DAL.ViewModels.DynamicAttDTOs;
using TLIS_DAL.ViewModels.DynamicAttInstValueDTOs;
using TLIS_DAL.ViewModels.MW_BULibraryDTOs;
using TLIS_DAL.ViewModels.MW_DishLbraryDTOs;
using TLIS_DAL.ViewModels.MW_ODUDTOs;
using TLIS_DAL.ViewModels.MW_OtherLibraryDTOs;
using TLIS_DAL.ViewModels.MW_RFUDTOs;

namespace TLIS_Service.IService
{
    public interface IMWLibraryService
    {
        Task MW_OtherLibrarySeedDataForTest();
        Task MW_ODULibrarySeedDataForTest();
        Task MW_DishLibrarySeedDataForTest();
        Response<GetEnableAttribute> GetMWDishLibrariesEnabledAtt(string ConnectionString);
        Response<ReturnWithFilters<MW_BULibraryViewModel>> get_MW_BU_LibrariesAsync(List<FilterObjectList> filters, bool WithFilterData, ParameterPagination parameters);
        Response<ReturnWithFilters<MW_DishLibraryViewModel>> get_MW_Dish_LibrariesAsync(List<FilterObjectList> filters, bool WithFilterData, ParameterPagination parameters);
        Response<ReturnWithFilters<MW_ODULibraryViewModel>> get_MW_ODU_LibrariesAsync(List<FilterObjectList> filters, bool WithFilterData, ParameterPagination parameters);
        Response<ReturnWithFilters<MW_RFULibraryViewModel>> get_MW_RFU_LibrariesAsync(List<FilterObjectList> filters, bool WithFilterData, ParameterPagination parameters);
        Response<ReturnWithFilters<MW_OtherLibraryViewModel>> get_MW_Other_LibrariesAsync(List<FilterObjectList> filters, ParameterPagination parameters);
        Response<GetForAddCivilLibrarybject> GetById(int Id, string TableName);
        Response<GetForAddCivilLibrarybject> AddMWLibrary(int UserId, string TableName, object LoadLibraryViewModel, string connectionString);
        Task<Response<EditMWBULibraryObject>> EditMWBULibrary(int userId, EditMWBULibraryObject editMWBULibrary, string TableName);
        Task<Response<AllItemAttributes>> Disable(int Id, string TableName);
        Task<Response<AllItemAttributes>> Delete(int Id, string TableName);
        Response<GetForAddCivilLibrarybject> GetForAdd(string TableName);
        Response<AddMWDishLibraryObject> AddMWDishLibrary(int UserId, string TableName, AddMWDishLibraryObject addMWDishLibraryObject, string connectionString);
        Response<ReturnWithFilters<object>> GetMW_BULibraries(CombineFilters CombineFilters, bool WithFilterData, ParameterPagination parameterPagination);
        Response<ReturnWithFilters<object>> GetMW_DishLibraries(CombineFilters CombineFilters, bool WithFilterData, ParameterPagination parameterPagination);
        Response<ReturnWithFilters<object>> GetMW_ODULibraries(CombineFilters CombineFilters, bool WithFilterData, ParameterPagination parameterPagination);
        Response<ReturnWithFilters<object>> GetMW_RFULibraries(CombineFilters CombineFilters, bool WithFilterData, ParameterPagination parameterPagination);
        Response<ReturnWithFilters<object>> GetMW_OtherLibraries(CombineFilters CombineFilters, ParameterPagination parameterPagination);
    }
}
