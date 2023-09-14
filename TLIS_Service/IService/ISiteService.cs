using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TLIS_DAL.Helper;
using TLIS_DAL.Helper.Filters;
using TLIS_DAL.Helpers;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModelBase;
using TLIS_DAL.ViewModels.AreaDTOs;
using TLIS_DAL.ViewModels.AttachedFilesDTOs;
using TLIS_DAL.ViewModels.LoadPartDTOs;
using TLIS_DAL.ViewModels.LocationTypeDTOs;
using TLIS_DAL.ViewModels.MW_BUDTOs;
using TLIS_DAL.ViewModels.MW_DishDTOs;
using TLIS_DAL.ViewModels.MW_ODUDTOs;
using TLIS_DAL.ViewModels.MW_RFUDTOs;
using TLIS_DAL.ViewModels.PowerDTOs;
using TLIS_DAL.ViewModels.RadioAntennaDTOs;
using TLIS_DAL.ViewModels.RadioRRUDTOs;
using TLIS_DAL.ViewModels.RegionDTOs;
using TLIS_DAL.ViewModels.SideArmDTOs;
using TLIS_DAL.ViewModels.SiteDTOs;
using TLIS_DAL.ViewModels.SiteStatusDTOs;

namespace TLIS_Service.IService
{
    public interface ISiteService
    {
        Response<AddSiteViewModel> AddSite(AddSiteViewModel AddSiteViewModel);
        Response<EditSiteViewModel> EditSite(EditSiteViewModel EditSiteViewModel);
        Response<List<AreaViewModel>> GetAllAreasForSiteOperation();
        Response<List<SiteStatusViewModel>> GetAllSiteStatusForSiteOperation();
        Response<List<RegionViewModel>> GetAllRegionsForSiteOperation();
        Response<List<LocationTypeViewModel>> GetAllLocationTypesForSiteOperation();
        Response<IEnumerable<SiteViewModel>> GetSites(ParameterPagination parameterPagination, List<FilterObjectList> filters = null);
        Task<Response<SiteViewModel>> UpdateRentedSpace(string SiteCode, float RentedSpaceValue, int installationSpace);
        bool CheckRentedSpace(string SiteCode, float ReservedSpaceValue);
        Response<List<KeyValuePair<string, float>>> GetSpaceDetails(string SiteCode);
        Response<SiteViewModel> GetSiteMainSpaces(string SiteCode);
        Response<List<ListSiteStatusViewModel>> GetAllSiteStatus();//ParameterPagination parameterPagination, List<FilterObjectList> filters
        Response<ListSiteStatusViewModel> GetSiteStatusbyId(int SiteStatusId);
        Response<ListSiteStatusViewModel> AddSiteStatus(AddSiteStatusViewModel SiteStatus);
        Response<ListSiteStatusViewModel> UpdatSiteStatus(ListSiteStatusViewModel SiteStatus);
        Response<ListSiteStatusViewModel> DeleteSiteStatus(int SiteStatusId);
        ImageResponse GetSitePhotosSlideshow(string SiteCode);
        Response<SiteCivilsViewModel> GetCivilsBySiteCode(string SiteCode);
        Response<SiteCivilsViewModel> GetCivilsWithAllCivilInstIdsBySiteCode(string SiteCode, string CivilType);
        Response<List<SideArmViewModel>> GetSideArmsBySiteCode(string SiteCode);
        Response<SiteViewModel> GetSitebyId(string SiteCode);
        Response<SiteOtherInventoriesViewModel> GetOtherInventoriesBySiteCode(string SiteCode);
        Response<SiteLoadsViewModel> GetLoadsBySiteCode(string SiteCode);
        Response<List<KeyValuePair<string, int>>> GetSteelCivil(string SiteCode);
        Response<List<KeyValuePair<string, int>>> GetNonSteel(string SiteCode);
        void test();
        Response<LoadsViewModel> GetLoadsOnSite(LoadsOnSiteFilter BaseFilter, bool WithFilterData);
        Response<ReturnWithFilters<object>> GetMW_DishOnSiteWithEnableAtt(LoadsOnSiteFilter BaseFilter, bool WithFilterData, CombineFilters CombineFilters, ParameterPagination parameterPagination, int? CivilId, string CivilType);
        Response<ReturnWithFilters<object>> GetMW_BUOnSiteWithEnableAtt(LoadsOnSiteFilter BaseFilter, bool WithFilterData, CombineFilters CombineFilters, ParameterPagination parameterPagination, int? CivilId, string CivilType);
        Response<ReturnWithFilters<object>> GetMW_ODUOnSiteWithEnableAtt(LoadsOnSiteFilter BaseFilter, bool WithFilterData, CombineFilters CombineFilters, ParameterPagination parameterPagination, int? CivilId, string CivilType);
        Response<ReturnWithFilters<object>> GetMW_RFUOnSiteWithEnableAtt(LoadsOnSiteFilter BaseFilter, bool WithFilterData, CombineFilters CombineFilters, ParameterPagination parameterPagination, int? CivilId, string CivilType);
        Response<ReturnWithFilters<object>> GetMW_OtherOnSiteWithEnableAtt(LoadsOnSiteFilter BaseFilter, bool WithFilterData, CombineFilters CombineFilters, ParameterPagination parameterPagination, int? CivilId, string CivilType);
        Response<ReturnWithFilters<object>> GetRadioAntennaOnSiteWithEnableAtt(LoadsOnSiteFilter BaseFilter, bool WithFilterData, CombineFilters CombineFilters, ParameterPagination parameterPagination, int? CivilId, string CivilType);
        Response<ReturnWithFilters<object>> GetRadioRRUOnSiteWithEnableAtt(LoadsOnSiteFilter BaseFilter, bool WithFilterData, CombineFilters CombineFilters, ParameterPagination parameterPagination, int? CivilId, string CivilType);
        Response<ReturnWithFilters<object>> GetRadioOtherOnSiteWithEnableAtt(LoadsOnSiteFilter BaseFilter, bool WithFilterData, CombineFilters CombineFilters, ParameterPagination parameterPagination, int? CivilId, string CivilType);
        Response<ReturnWithFilters<object>> GetPowerOnSiteWithEnableAtt(LoadsOnSiteFilter BaseFilter, bool WithFilterData, CombineFilters CombineFilters, ParameterPagination parameterPagination, int? CivilId, string CivilType);
        Task<string> GetSMIS_Site();
        Response<List<AreaViewModel>> GetAllArea();
        Response<List<RegionViewModel>> GetAllRegion();
        Response<SiteViewModel> DisplaySiteDetailsBySiteCode(string SiteCode);
        Task<Response<bool>> EditSitesMainSpaces(float RentedSpace, float ReservedSpace, string SiteCode);
        Response<List<GetAllsiteOnMultiRegion>> GetAllsiteonMultiRegion(List<RegionForSiteViewModel> Region);
        Response<List<GetAllsiteOnMultiAreaViewModel>> GetAllsiteonMultiArea(List<AreaForSiteViewModel> Area);
        Task<List<GetSiteNameBySitCode>> GetSiteNameBySitCode(List<SiteCodeForW_F> SiteCode);
        List<SiteViewModel> GetAllSitesWithoutPaginationForWorkFlow();
    }
}
