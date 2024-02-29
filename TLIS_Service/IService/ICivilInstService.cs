using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TLIS_DAL.Helper;
using TLIS_DAL.Helper.Filters;
using TLIS_DAL.Helpers;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModelBase;
using TLIS_DAL.ViewModels.AllCivilInstDTOs;
using TLIS_DAL.ViewModels.CivilLoadsDTOs;
using TLIS_DAL.ViewModels.CivilNonSteelDTOs;
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
        Response<GetForAddCivilWithLegObject> GetForAddCivilWithLegInstallation(string TableName, int CivilLibraryId, string SiteCode);
        Response<GetForAddCivilWithOutLegInstallationcs> GetForAddCiviNonSteelInstallation(string TableName, int CivilLibraryId, string SiteCode);
        Response<GetForAddCivilWithOutLegInstallationcs> GetForAddCivilWithOutLegInstallation_Monople(string TableName, int CivilLibraryId, string SiteCode);
        Response<GetForAddCivilWithOutLegInstallationcs> GetForAddCivilWithOutLegInstallation_Capsule(string TableName, int CivilLibraryId, string SiteCode);
        Response<GetForAddCivilWithOutLegInstallationcs> GetForAddCivilWithOutLegInstallation_Mast(string TableName, int CivilLibraryId, string SiteCode);
        Response<LoadsOnSideArm> GetLoadsOnSideArm(int SideArmId);

        Response<CivilLoads> GetLoadsAndSideArmsForCivil(int CivilId, string CivilType);
        Response<ObjectInst> GetAttForAdd(string CivilType, int CivilLibraryId, int? CategoryId, string SiteCode);
        Response<ObjectInstAtts> AddCivilInstallation(object CivilInstallationViewModel, string CivilType, string SiteCode, string connectionString, int? TaskId);
        Task<Response<ObjectInstAtts>> EditCivilInstallation(object CivilInstallationViewModel, string CivilType, int? TaskId);
        Response<ObjectInstAtts> GetById(int CivilInsId, string CivilType);
        Response<object> GetCivilWithLegsWithEnableAtt(SiteBaseFilter BaseFilter, bool WithFilterData, CombineFilters CombineFilters, ParameterPagination parameterPagination, string ConnectionString);
        Response<ReturnWithFilters<object>> GetCivilWithoutLegWithEnableAtt(SiteBaseFilter BaseFilter, bool WithFilterData, CombineFilters CombineFilters, ParameterPagination parameterPagination, int CategoryId);
        Response<object> GetCivilNonSteelWithEnableAtt(SiteBaseFilter BaseFilter, bool WithFilterData, CombineFilters CombineFilters, ParameterPagination parameterPagination,string ConnectionString);
        Response<AllCivilsViewModel> GetAllCivils(SiteBaseFilter BaseFilter, bool WithFilterData, ParameterPagination parameterPagination);
        Response<List<ListOfCivilLoadDto>> GetAllCivilLoad(string SearchName, ParameterPagination parameters);
        List<LoadOnSideArm> GetLoadForSideArm(int sidearmid, int civilid);
        List<LoadOnCivil> GetLoadWithoutSideArm(int civilid);
        string GetKeyName(TLIallLoadInst m);
        // Response<bool> CivilDismantle(DismantleBinding dis);
        bool ReCalcualateSiteFreeSpace(int civilid, string sitecode, int loadid);
        public Response<bool> CheckLoadsBeforDismantle(string TableName, int loadId);
        bool CheckRelatedLoad(string sitecode, int civilid, int sidearm, int loadid);
        public Response<CivilLoads> GetRelationshipBetweenloads(int loadid, string Loadname);
        public Response<bool> DismantleCivil(string SiteCode, int CivilId, string CivilName, int? TaskId);
        //Response<bool> OtherLoadDismantale(string sidecode, int loadid);
        string GetOtherLoadKey(int loadid);
        Response<List<LibraryDataDto>> GetLibraryAttForInstallations(string InstTableName, int? CatId);
        Response<List<LogicalOperationViewModel>> GetlogicalOperation();
        Response<SideArmAndLoadsOnCivil> CheckFilterSideArm_LoadsOnCivils(int CivilId, string CivilType);
        Response<LoadsCountOnSideArm> CheckLoadsOnSideArm(int SideArmId);
        Response<List<RecalculatSpace>> RecalculatSpace(int CivilId, string CivilType);
    }
}
