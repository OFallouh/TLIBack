using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TLIS_DAL.Helper;
using TLIS_DAL.Helper.Filters;
using TLIS_DAL.Helpers;
using TLIS_DAL.ViewModelBase;
using TLIS_DAL.ViewModels.AllCivilInstDTOs;
using TLIS_DAL.ViewModels.CivilLoadsDTOs;
using TLIS_DAL.ViewModels.CivilWithLegsDTOs;
using TLIS_DAL.ViewModels.DependencyDTOs;
using TLIS_DAL.ViewModels.DynamicAttDTOs;
using TLIS_DAL.ViewModels.LoadPartDTOs;
using TLIS_DAL.ViewModels.SideArmDTOs;
using TLIS_DAL.ViewModels.SiteDTOs;
using TLIS_DAL.ViewModels.TablesNamesDTOs;

namespace TLIS_Service.IService
{
    public interface IInternalApiService
    {
        Response<AllCivilInstallationViewModel> GetCivilsBySiteCode(SiteFilter BaseFilter,CombineFilters CombineFilters, bool WithFilterData, ParameterPagination parameterPagination);
       Response<List<ObjectInstAttForSideArm>> GetSideArmsBySiteCode(string SiteCode, int? CivilId, int? LegId, float? MinAzimuth, float? MaxAzimuth, float? MinHeightBase, float? MaxHeightBase);
        Response<ReturnWithFilters<object>> GetLibraryforSpecificType(string TableNameLibrary, int CategoryId, CombineFilters CombineFilters, bool WithFilterData, ParameterPagination parameterPagination);
        Response<IEnumerable<SiteViewModel>> GetAllSitesDetails(ParameterPagination parameterPagination, List<FilterObjectList> filters = null);
        Response<LoadsDto> GetAllLoadonSitebyPartandType(String SiteCode, string PartName, string TypeName);
        Response<ReturnWithFilters<object>> GetAlOtherInventoryonSitebyType(string OtherInventoryTypeName, SiteFilter BaseFilter, bool WithFilterData, CombineFilters CombineFilters, ParameterPagination parameterPaginationstring ,string LibraryType);
        Response<List<ListOfCivilLoads>> GetAllItemsonSite(string SiteCode);
        Response<ReturnWithFilters<object>> GetConfigurationTables(SiteFilter BaseFilters, string TableNameInstallation, int CategoryId, bool WithFilterData, CombineFilters CombineFilters, ParameterPagination parameterPagination, string LibraryType);
        Response<List<BassAttViewModel>> GetConfigurationAttributes(string TableName, bool IsDynamic, int CategoryId);
        Response<AddDependencyInstViewModel> AddDynamicAttInst(AddDependencyInstViewModel addDependencyInstViewModel, string ConnectionString);
        Response<AddDependencyViewModel> AddDynamicAtts(AddDependencyViewModel addDependencyViewModel, string ConnectionString);
        Task<Response<DynamicAttViewModel>> Edit(EditDynamicAttViewModel dynamicAttViewModel);
        Response<AllItemAttributes> AddRadioLibrary(string TableName, object RadioLibraryViewModel, string connectionString);
        Response<AllAtributes> GetForAdd(string TableName);
        Response<ObjectAttributeInst> GetAttForAdd(string TableName, int LibId, string SiteCode);
        Response<ObjectInstAtts> AddRadioInstallation(object RadioInstallationViewModel, string TableName, string SiteCode, string ConnectionString);
        Response<string> AttachFile(IFormFile file, int documenttypeId, string Model, string Name, string SiteCode, string RecordId, string TableName, string connection, string AttachFolder, string asset);
    }
}
