﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TLIS_DAL.Helper;
using TLIS_DAL.Helper.Filters;
using TLIS_DAL.Helpers;
using TLIS_DAL.ViewModelBase;
using TLIS_DAL.ViewModels.CabinetPowerLibraryDTOs;
using TLIS_DAL.ViewModels.CabinetTelecomLibraryDTOs;
using TLIS_DAL.ViewModels.CivilWithLegLibraryDTOs;
using TLIS_DAL.ViewModels.GeneratorLibraryDTOs;
using TLIS_DAL.ViewModels.SolarLibraryDTOs;

namespace TLIS_Service.IService
{
    public interface IOtherInventoryLibraryService
    {
        Task RadioRRULibrarySeedDataForTest();
        Task GeneratorLibrarySeedDataForTest();
        Task SolarLibrarySeedDataForTest();
        Response<ReturnWithFilters<SolarLibraryViewModel>> GetSolarLibraries(List<FilterObjectList> filters, bool WithFilterData, ParameterPagination parameters);
        Response<ReturnWithFilters<GeneratorLibraryViewModel>> GetGeneratorLibraries(List<FilterObjectList> filters, bool WithFilterData, ParameterPagination parameters);
        Response<ReturnWithFilters<CabinetPowerLibraryViewModel>> GetCabinetPowerLibraries(List<FilterObjectList> filters, bool WithFilterData, ParameterPagination parameters);
        Response<ReturnWithFilters<CabinetTelecomLibraryViewModel>> GetCabinetTelecomLibraries(List<FilterObjectList> filters, bool WithFilterData, ParameterPagination parameters);
        Response<GetForAddCivilLibrarybject> GetById(int Id, string TableName,int UserId,bool ExternalSys);
        Task<Response<EditGeneratorLibraryObject>> EditGeneratorLibrary(int userId, EditGeneratorLibraryObject editGeneratorLibraryObject, string TableName, string connectionString, bool ExternalSys);
        Response<AddGeneratorLibraryObject> AddGenertatoLibrary(int UserId, string TableName, AddGeneratorLibraryObject addGeneratorLibraryObject, string connectionString, bool ExternalSys);
        Response<AllItemAttributes> AddOtherInventoryLibrary(string OtherInventoryType, object OtherInventoryLibraryViewModel, string connectionString);
        // Task<Response<AllItemAttributes>> EditOtherInventoryLibrary(object OtherInventoryLibraryViewModel, string OtherInventoryType);
        Task<Response<AllItemAttributes>> Disable(int Id, string OtherInventoryType, string ConnectionString,int UserId);
        Task<Response<AllItemAttributes>> Delete(int Id, string OtherInventoryType, string ConnectionString, int UserId);
        Response<GetForAddCivilLibrarybject> GetForAdd(string TableName, int UserId, bool ExternalSys);
        Response<ReturnWithFilters<object>> GetCabinetPowerLibraryEnabledAtt(CombineFilters CombineFilters, bool WithFilterData, ParameterPagination parameterPagination);
        Response<ReturnWithFilters<object>> GetCabinetTelecomLibraryEnabledAtt(CombineFilters CombineFilters, bool WithFilterData, ParameterPagination parameterPagination);
        Response<ReturnWithFilters<object>> GetSolarLibraryEnabledAtt(CombineFilters CombineFilters, bool WithFilterData, ParameterPagination parameterPagination);
        Response<ReturnWithFilters<object>> GetGeneratorLibraryEnabledAtt(CombineFilters CombineFilters, bool WithFilterData, ParameterPagination parameterPagination);
        Response<GetEnableAttribute> GetGeneratorLibrariesEnabledAtt(string ConnectionString);
        Task<Response<EditSolarLibraryObject>> EditSolarLibrary(int userId, EditSolarLibraryObject editSolarLibraryObject, string TableName, string connectionString, bool ExternalSys);
        Response<AddSolarLibraryObject> AddSolarLibrary(int UserId, string TableName, AddSolarLibraryObject addSolarLibraryObject, string connectionString, bool ExternalSys);
        Response<GetEnableAttribute> GetSolarLibrariesEnabledAtt(string ConnectionString);
        Response<AddCabinetPowerLibraryObject> AddCabinetPowerLibrary(int UserId, string TableName, AddCabinetPowerLibraryObject addCabinetPowerLibraryObject, string connectionString);
        Response<AddCabinetTelecomLibraryObject> AddCabinetTelecomLibrary(int UserId, string TableName, AddCabinetTelecomLibraryObject addCabinetTelecomLibraryObject, string connectionString);
        Task<Response<EditCabinetPowerLibraryObject>> EditCabinetPowerLibrary(int userId, EditCabinetPowerLibraryObject editCabinetPowerLibraryObject, string TableName, string connectionString,bool ExternalSys);
        Task<Response<EditCabinetTelecomLibraryObject>> EditCabinetTelecomLibrary(int userId, EditCabinetTelecomLibraryObject editCabinetTelecomLibraryObject, string TableName, string connectionString);
        Response<GetEnableAttribute> GetCabinetPowerLibrariesEnabledAtt(string ConnectionString);
        Response<GetEnableAttribute> GetCabinetTelecomLibrariesEnabledAtt(string ConnectionString);
    }
}
