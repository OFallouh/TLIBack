﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TLIS_DAL.Helper;
using TLIS_DAL.Helper.Filters;
using TLIS_DAL.Helpers;
using TLIS_DAL.ViewModelBase;
using TLIS_DAL.ViewModels.CivilLoadsDTOs;
using TLIS_DAL.ViewModels.CivilWithLegLibraryDTOs;
using TLIS_DAL.ViewModels.CivilWithLegsDTOs;
using TLIS_DAL.ViewModels.LoadOtherDTOs;
using TLIS_DAL.ViewModels.MW_DishDTOs;
using TLIS_DAL.ViewModels.RadioAntennaDTOs;
using TLIS_DAL.ViewModels.RadioOtherDTOs;
using TLIS_DAL.ViewModels.RadioRRUDTOs;
using TLIS_DAL.ViewModels.SideArmDTOs;

namespace TLIS_Service.IService
{
    public interface IRadioInstService
    {
        Response<ObjectInstAtts> GetAttForAdd(string TableName, int LibId, string SiteCode);
        Response<GetForAddLoadObject> GetRadioOtherInstallationById(int RadioId, string TableName);
        Response<GetForAddMWDishInstallationObject> GetAttForAddRadioOtherInstallation(int LibraryID, string SiteCode, int UserId, bool ExternalSy);
        Response<GetEnableAttribute> GetRadioOtherInstallationWithEnableAtt(string SiteCode, string ConnectionString);
        Response<GetForAddMWDishInstallationObject> GetAttForAddRadioRRUInstallation(int LibraryID, string SiteCode, int UserId, bool ExternalSy);
        Task<Response<GetForAddMWDishInstallationObject>> EditRadioInstallation(object RadioInstallationViewModel, string TableName, int? TaskId, int UserId, string ConnectionString, bool ExternalSy);
        Response<GetEnableAttribute> GetRadioAntennaInstallationWithEnableAtt(string SiteCode, string ConnectionString);
        Response<GetForAddMWDishInstallationObject> GetAttForAddRadioAntennaInstallation(int LibraryID, string SiteCode,int UserId, bool ExternalSy);
        Response<GetForAddMWDishInstallationObject> AddRadioInstallation(object RadioInstallationViewModel, string TableName, string SiteCode, string ConnectionString, int? TaskId, int UserId, bool ExternalSy);      
        Response<ObjectInstAttsForSideArm> GetById(int RadioInsId, string TableName);
        Response<GetForAddLoadObject> GetRadioAntennaInstallationById(int RadioId, string TableName);
        Response<bool> DismantleLoads(string sitecode, int LoadId, string LoadName, int? TaskId, int UserId, string connectionString);
        Response<ReturnWithFilters<RadioRRUViewModel>> GetRadioRRUsList(List<FilterObjectList> filters, bool WithFilterData, ParameterPagination parameters);
        Response<ReturnWithFilters<RadioAntennaViewModel>> GetRadioAntennasList(List<FilterObjectList> filters, bool WithFilterData, ParameterPagination parameters);
        Response<ReturnWithFilters<RadioOtherViewModel>> GetRadioOtherList(List<FilterObjectList> filters, bool WithFilterData, ParameterPagination parameters);
        Response<GetEnableAttribute> GetRadioRRUInstallationWithEnableAtt(string SiteCode, string ConnectionString);
        Response<GetForAddLoadObject> GetRadioRRUInstallationById(int RadioId, string TableName);

    }
}
