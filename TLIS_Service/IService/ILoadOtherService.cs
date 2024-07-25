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
using TLIS_DAL.ViewModels.LoadOtherDTOs;
using TLIS_DAL.ViewModels.MW_DishDTOs;
using TLIS_DAL.ViewModels.SideArmDTOs;
using TLIS_DAL.ViewModels.SiteDTOs;

namespace TLIS_Service.IService
{
    public interface ILoadOtherService
    {
        Response<GetForAddMWDishInstallationObject> GetAttForAddLoadOtherInstallation(int LibraryID, string SiteCode);
        Response<GetForAddLoadObject> GetLoadOtherInstallationById(int LoadOtherId, string TableName);
        Response<GetForAddMWDishInstallationObject> AddLoadOther(AddLoadOtherInstallationObject LoadOtherViewModel, string SiteCode, string ConnectionString, int? TaskId, int UserId);
        Response<GetEnableAttribute> GetLoadOtherInstallationWithEnableAtt(string SiteCode, string ConnectionString);
        Response<bool> DismantleLoads(string sitecode, int LoadId, string LoadName, int? TaskId, int UserId, string connectionString);
        Task<Response<GetForAddMWDishInstallationObject>> EditLoadOtherInstallation(EditLoadOtherInstallationObject LoadOtherViewModel, int? TaskId, int UserId, string ConnectionString);
        Response<ObjectInstAttsForSideArm> GetById(int Id);
        Response<ReturnWithFilters<LoadOtherViewModel>> GetLoadOtherList(List<FilterObjectList> filters, bool WithFilterData, ParameterPagination parameters);
        Response<ReturnWithFilters<LoadsOtherDisplayedOnTableViewModel>> GetLoadsOtherBySite(LoadsOnSiteFilter BaseFilter, bool WithFilterData, List<FilterObjectList> ObjectAttributeFilters, ParameterPagination parameterPagination);
        Response<ReturnWithFilters<object>> GetLoadOtherOnSiteWithEnableAtt(LoadsOnSiteFilter BaseFilter, bool WithFilterData, CombineFilters CombineFilters, ParameterPagination parameterPagination, int? CivilId, string CivilType);
    }
}
