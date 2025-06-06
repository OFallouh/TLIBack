﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TLIS_DAL.Helper;
using TLIS_DAL.Helper.Filters;
using TLIS_DAL.Helpers;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModelBase;
using TLIS_DAL.ViewModels;
using TLIS_DAL.ViewModels.AllCivilInstDTOs;
using TLIS_DAL.ViewModels.CivilLoadsDTOs;
using TLIS_DAL.ViewModels.CivilNonSteelDTOs;
using TLIS_DAL.ViewModels.CivilWithLegLibraryDTOs;
using TLIS_DAL.ViewModels.CivilWithLegsDTOs;
using TLIS_DAL.ViewModels.CivilWithoutLegDTOs;
using TLIS_DAL.ViewModels.DismantleDto;
using TLIS_DAL.ViewModels.DynamicAttInstValueDTOs;
using TLIS_DAL.ViewModels.LogicalOperationDTOs;
using TLIS_DAL.ViewModels.SideArmDTOs;

namespace TLIS_Service.IService
{
    public interface ICivilInstService
    {
        // Response<AllCivilInstallation> GetCivilsBySiteCode(string siteCode, string ConnectionString, int? UserId);
        Response<GetForAddCivilWithLegObject> GetForAddCivilWithLegInstallation(string TableName, int CivilLibraryId, string SiteCode, int UserId, bool ExternalSys);
        Response<GetForAddCivilWithOutLegInstallationcs> GetCivilWithoutLegsInstallationById(int CivilInsId, string TableName, int CategoryId, int UserId, bool ExternalSys);
        Task<Response<ObjectInstAtts>> EditCivilNonSteelInstallation(EditCivilNonSteelInstallationObject editCivilNonSteelInstallationObject, string CivilType, int? TaskId, int userId,string connectionString, bool ExternalSys);
        // Task<Response<ObjectInstAtts>> EditCivilInstallation(object CivilInstallationViewModel, string CivilType, int? TaskId);
        Response<GetForAddCivilWithOutLegInstallationcs> GetForAddCiviNonSteelInstallation(string TableName, int CivilLibraryId, string SiteCode, int UserId, bool ExternalSys);
        Response<GetForAddCivilWithOutLegInstallationcs> GetForAddCivilWithOutLegInstallation_Monople(string TableName, int CivilLibraryId, string SiteCode, int UserId, bool ExternalSys);
        Response<GetForAddCivilWithOutLegInstallationcs> GetForAddCivilWithOutLegInstallation_Capsule(string TableName, int CivilLibraryId, string SiteCode, int UserId, bool ExternalSys);
        Response<GetForAddCivilWithOutLegInstallationcs> GetForAddCivilWithOutLegInstallation_Mast(string TableName, int CivilLibraryId, string SiteCode, int UserId, bool ExternalSys);
        Response<CivilLoads> GetLoadsOnSideArm(int SideArmId);
        Response<CheckLoadAndSideArmOnCivil> GetLoadsAndSideArmsForCivil(int CivilId, string CivilType);
        Response<ObjectInstAtts> AddCivilWithoutLegsInstallation(AddCivilWithoutLegViewModel addCivilWithoutLegViewModel, string TableName, string SiteCode, string connectionString, int? TaskId, int UserId, bool ExternalSys);
        Response<ObjectInstAtts> AddCivilNonSteelInstallation(AddCivilNonSteelObject addCivilNonSteelObject, string TableName, string SiteCode, string connectionString, int? TaskId, int UserId, bool ExternalSys);
        Task<Response<ObjectInstAtts>> EditCivilWithoutLegsInstallation(EditCivilWithoutLegsInstallationObject editCivilWithoutLegsInstallationObject, string CivilType, int? TaskId, int userId, string connectionString, bool ExternalSys);
        // Response<ObjectInstAtts> AddCivilInstallation(object CivilInstallationViewModel, string TableName, string SiteCode, string connectionString, int? TaskId);
        Response<ObjectInst> GetAttForAdd(string CivilType, int CivilLibraryId, int? CategoryId, string SiteCode);
        Response<ObjectInstAtts> AddCivilWithLegsInstallation(AddCivilWithLegsViewModel AddCivilWithLegsViewModel, string TableName, string SiteCode, string connectionString, int? TaskId, int UserId, bool ExternalSys);
        Task<Response<ObjectInstAtts>> EditCivilWithLegsInstallation(EditCivilWithLegsInstallationObject editCivilWithLegsInstallationObject, string CivilType, int? TaskId, int userId, string connectionString, bool ExternalSys);
        Response<GetForAddCivilWithLegObject> GetCivilWithLegsInstallationById(int CivilInsId, string TableName, int UserId, bool ExternalSys);
        Response<GetEnableAttribute> GetCivilWithLegsWithEnableAtt(string? SiteCode, string ConnectionString);
        Response<ReturnWithFilters<object>> GetCivilWithoutLegWithEnableAtt(SiteBaseFilter BaseFilter, bool WithFilterData, CombineFilters CombineFilters, ParameterPagination parameterPagination, int CategoryId);
        Response<GetEnableAttribute> GetCivilNonSteelWithEnableAtt(string? SiteCode, string ConnectionString );
        //Response<AllCivilsViewModel> GetAllCivils(SiteBaseFilter BaseFilter, bool WithFilterData, ParameterPagination parameterPagination, string SiteCode);
        Response<List<ListOfCivilLoadDto>> GetAllCivilLoad(string SearchName, ParameterPagination parameters);
        Response<LoadOnSideArm> GetLoadForSideArm(int sidearmid, int civilid);
        List<LoadOnCivil> GetLoadWithoutSideArm(int civilid);
        Response<bool> DismantleCivilWithLegsInstallation(int UserId, string SiteCode, int CivilId, int? TaskId, string connectionString, bool ExternalSys);
        string GetKeyName(TLIallLoadInst m);
        Response<GetForAddCivilWithOutLegInstallationcs> GetCivilNonSteelInstallationById(int CivilInsId, string TableName, int UserId, bool ExternalSys);
        //Response<bool> CivilDismantle(DismantleBinding dis);
        bool ReCalcualateSiteFreeSpace(int civilid, string sitecode, int loadid);
        Response<bool> DismantleCivilNonSteelInstallation(int UserId, string SiteCode, int CivilId,int? TaskId, string connectionString,bool ExternalSys);
        Response<bool> DismantleCivilWithoutLegsInstallation(int UserId, string SiteCode, int CivilId , int? TaskId, string connectionString, bool ExternalSys);
        Response<bool> CheckLoadsBeforDismantle(string TableName, int LoadId, string sitecode);
        bool CheckRelatedLoad(string sitecode, int civilid, int sidearm, int loadid);
        Response<CivilLoads> GetRelationshipBetweenloads(int LoadId, string Loadname, string sitecode);
        //public Response<bool> DismantleCivil(string SiteCode, int CivilId, string CivilName, int? TaskId);
        //Response<bool> OtherLoadDismantale(string sidecode, int loadid);
        string GetOtherLoadKey(int loadid);
        Response<List<LibraryDataDto>> GetLibraryAttForInstallations(string InstTableName, int? CatId);
        Response<List<LogicalOperationViewModel>> GetlogicalOperation();
        Response<SideArmAndLoadsOnCivil> CheckFilterSideArm_LoadsOnCivils(int CivilId, string CivilType);
        Response<LoadsCountOnSideArm> CheckLoadsOnSideArm(int SideArmId);
        Response<List<RecalculatSpace>> RecalculatSpace(int CivilId, string CivilType);
        Response<GetEnableAttribute> GetCivilWithoutLegMastWithEnableAtt(string? SiteCode, string ConnectionString);
        Response<GetEnableAttribute> GetCivilWithoutLegCapsuleWithEnableAtt(string? SiteCode , string ConnectionString);
        Response<GetEnableAttribute> GetCivilWithoutLegMonopoleWithEnableAtt(string? SiteCode, string ConnectionString );
    }
}
