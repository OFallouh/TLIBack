﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TLIS_DAL.Helper;
using TLIS_DAL.Helper.Filters;
using TLIS_DAL.Helpers;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModelBase;
using TLIS_DAL.ViewModels.CabinetDTOs;
using TLIS_DAL.ViewModels.CabinetPowerLibraryDTOs;
using TLIS_DAL.ViewModels.CabinetTelecomLibraryDTOs;
using TLIS_DAL.ViewModels.CivilWithLegLibraryDTOs;
using TLIS_DAL.ViewModels.CivilWithLegsDTOs;
using TLIS_DAL.ViewModels.GeneratorDTOs;
using TLIS_DAL.ViewModels.GeneratorLibraryDTOs;
using TLIS_DAL.ViewModels.OtherInSiteDTOs;
using TLIS_DAL.ViewModels.SolarDTOs;
using TLIS_DAL.ViewModels.SolarLibraryDTOs;

namespace TLIS_Service.IService
{
    public interface IOtherInventoryInstService
    {
        Response<ObjectInstAtts> GetAttForAdd(string TableName, string OtherInventoryLibraryType, int OtherInventoryId, string SiteCode);
        Response<bool> DismantleOtherInventory(int UserId, string SiteCode, int OtherInventoryId, string OtherInventoryName, int? TaskId, string ConnectionString,bool ExternalSys);
        Response<GetForAddOtherInventoryInstallationObject> GetCabinetPowerInstallationById(int CabinetPowerId, string TableName);
        Response<GetForAddOtherInventoryInstallationObject> GetCabinetTelecomInstallationById(int CabinetTelecomId, string TableName);
        Task<Response<GetForAddOtherInventoryInstallationObject>> EditOtherInventoryInstallation(object model, string TableName, int? TaskId, int UserId, string connectionString, bool ExternalSys);
        Response<GetForAddOtherInventoryInstallationObject> GetGenertorInstallationById(int CivilInsId, string TableName,int UserId, bool ExternalSys);
        // Response<ObjectInstAtts> AddOtherInventoryInstallation(object model, string TableName, string SiteCode, string ConnectionString, int? TaskId, int UserId);
        Response<GetEnableAttribute> GetGeneratorWithEnableAtt(string? SiteCode, string ConnectionString);
        Response<GetForAddOtherInventoryInstallationObject> GetAttForAddGeneratorInstallation(string TableName, int LibraryID, string SiteCode, int UserId, bool ExternalSys);
        Response<AddGeneratorInstallationObject> AddGeneratorInstallation(AddGeneratorInstallationObject addGeneratorInstallationObject, string SiteCode, string ConnectionString, int? TaskId, int UserId, bool ExternalSys);
        Response<ObjectInstAtts> GetById(int OtherInventoryInsId, string TableName);
        Response<GetEnableAttribute> GetSolarWithEnableAtt(string? SiteCode, string ConnectionString);
        Response<GetForAddOtherInventoryInstallationObject> GetAttForAddSolarInstallation(string TableName, int LibraryID, string SiteCode, int UserId, bool ExternalSys);
        Response<GetForAddOtherInventoryInstallationObject> GetSolarInstallationById(int SolarId, string TableName, int UserId, bool ExternalSys);
        Response<AddSolarInstallationObject> AddSolarInstallation(AddSolarInstallationObject addSolarInstallationObject, string SiteCode, string ConnectionString, int? TaskId, int UserId, bool ExternalSys);
      
        Response<ReturnWithFilters<object>> GetCabinetBySiteWithEnabledAtt(SiteBaseFilter BaseFilter, bool WithFilterData, CombineFilters CombineFilters, ParameterPagination parameterPagination, string LibraryType);
        Response<ReturnWithFilters<object>> GetSolarBySiteWithEnabledAtt(SiteBaseFilter BaseFilter, bool WithFilterData, CombineFilters CombineFilters, ParameterPagination parameterPagination);
        Response<ReturnWithFilters<object>> GetGeneratorBySiteWithEnabledAtt(SiteBaseFilter BaseFilter, bool WithFilterData, CombineFilters CombineFilters, ParameterPagination parameterPagination);
        Response<AddCabinetPowerInstallation> AddCabinetPowerInstallation(AddCabinetPowerInstallation addCabinetPowerInstallation, string SiteCode, string ConnectionString, int? TaskId, int UserId, bool ExternalSys);
        Response<GetForAddOtherInventoryInstallationObject> GetAttForAddCabinetTelecomInstallation(string TableName, int LibraryID, string SiteCode);
        Response<GetForAddOtherInventoryInstallationObject> GetAttForAddCabinetPowerInstallation(string TableName, int LibraryID, string SiteCode);
        Response<GetEnableAttribute> GetCabinetTelecomWithEnableAtt(string? SiteCode, string ConnectionString);
        Response<GetEnableAttribute> GetCabinetPowerWithEnableAtt(string? SiteCode, string ConnectionString);
        Task<Response<GetForAddOtherInventoryInstallationObject>> EditCabinetPowerInstallation(EditCabinetPowerInstallationObject editCabinetPowerInstallationObject, string TableName, int? TaskId, int UserId, string connectionString, bool ExternalSys);
        Task<Response<GetForAddOtherInventoryInstallationObject>> EditCabinetTelecomInstallation(EditCabinetTelecomInstallationObject editCabinetTelecomInstallationObject, string TableName, int? TaskId, int UserId, string connectionStringg, bool ExternalSys);
        Response<AddCabinetTelecomInstallationObject> AddCabinetTelecomInstallation(AddCabinetTelecomInstallationObject addCabinetTelecomInstallationObject, string SiteCode, string ConnectionString, int? TaskId, int UserIdg, bool ExternalSys);
    }
}
