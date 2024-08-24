using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TLIS_DAL.Helper;
using TLIS_DAL.Helper.Filters;
using TLIS_DAL.Helpers;
using TLIS_DAL.ViewModelBase;
using TLIS_DAL.ViewModels.CivilWithLegLibraryDTOs;
using TLIS_DAL.ViewModels.LoadOtherLibraryDTOs;
using TLIS_DAL.ViewModels.RadioAntennaLibraryDTOs;
using TLIS_DAL.ViewModels.RadioOtherLibraryDTOs;
using TLIS_DAL.ViewModels.RadioRRULibraryDTOs;
using static TLIS_DAL.ViewModels.RadioAntennaLibraryDTOs.EditRadioAntennaLibraryObject;

namespace TLIS_Service.IService
{
    public interface IRadioLibraryService
    {
        Response<ReturnWithFilters<RadioAntennaLibraryViewModel>> GetRadioAntennaLibraries(ParameterPagination parameters, List<FilterObjectList> filters = null);
        Response<ReturnWithFilters<RadioRRULibraryViewModel>> GetRadioRRULibraries(ParameterPagination parameters, List<FilterObjectList> filters = null);
        Task<Response<EditRadioAntennaLibraryObject>> EditRadioAntennaLibrary(string TableName, EditRadioAntennaLibraryObject RadioLibraryViewModel, int UserId,string connectionString);
        Response<ReturnWithFilters<RadioOtherLibraryViewModel>> GetOtherRadioLibraries(ParameterPagination parameters, List<FilterObjectList> filters = null);
        Response<GetForAddCivilLibrarybject> GetById(int Id, string TableName);
        Task<Response<EditRadioOtherLibraryObject>> EditRadioOtherLibrary(string TableName, EditRadioOtherLibraryObject editRadioOtherLibraryObject, int UserId, string connectionString);
        Response<GetEnableAttribute> GetRadioOtherLibrariesEnabledAtt(string ConnectionString);
        Response<AddRadioOtherLibraryObject> AddRadioOtherLibrary(string TableName, AddRadioOtherLibraryObject addRadioOtherLibraryObject, string connectionString, int UserId);
        Response<GetEnableAttribute> GetRadioRRULibrariesEnabledAtt(string ConnectionString);
        Response<AllItemAttributes> AddRadioLibrary(string TableName, object RadioLibraryViewModel, string connectionString, int UserId);
        // Task<Response<AllItemAttributes>> EditRadioLibrary(string TableName, object RadioLibraryViewModel);
        Task<Response<AllItemAttributes>> DisableRadioLibrary(string TableName, int Id, int UserId, string connectionString);
        Task<Response<AllItemAttributes>> DeletedRadioLibrary(string TableName, int Id, int UserId, string connectionString);
        Response<GetForAddCivilLibrarybject> GetForAdd(string TableName, int? UserId, string UserName);
        Task<Response<EditRadioRRULibraryObject>> EditRadioRRULibrary(string TableName, EditRadioRRULibraryObject RadioLibraryViewModel, int UserId, string connectionString);
        Response<AddRadioRRULibraryObject> AddRadioRRULibrary(string TableName, AddRadioRRULibraryObject RadioLibraryViewModel, string connectionString, int UserId);
        Response<GetEnableAttribute> GetRadioAntennaLibrariesEnabledAtt(string ConnectionString);
        Response<AddRadioAntennaLibraryObject> AddRadioAntennaLibrary(string TableName, AddRadioAntennaLibraryObject RadioLibraryViewModel, string connectionString, int UserId);
        Response<ReturnWithFilters<object>> GetRadioAntennaLibrariesWithEnabledAttribute(CombineFilters CombineFilters, ParameterPagination parameterPagination);
        Response<ReturnWithFilters<object>> GetRadioRRULibrariesWithEnabledAttribute(CombineFilters CombineFilters, ParameterPagination parameterPagination);
        Response<ReturnWithFilters<object>> GetRadioOtherLibrariesWithEnabledAttribute(CombineFilters CombineFilters, ParameterPagination parameterPagination);
    }
}
