using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TLIS_DAL.Helper;
using TLIS_DAL.Helper.Filters;
using TLIS_DAL.Helpers;
using TLIS_DAL.ViewModelBase;
using TLIS_DAL.ViewModels.CabinetPowerLibraryDTOs;
using TLIS_DAL.ViewModels.CabinetTelecomLibraryDTOs;
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
        Response<AllItemAttributes> GetById(int Id, string OtherInventoryType);
        Response<AllItemAttributes> AddOtherInventoryLibrary(string OtherInventoryType, object OtherInventoryLibraryViewModel, string connectionString);
       // Task<Response<AllItemAttributes>> EditOtherInventoryLibrary(object OtherInventoryLibraryViewModel, string OtherInventoryType);
        Task<Response<AllItemAttributes>> Disable(int Id, string OtherInventoryType);
        Task<Response<AllItemAttributes>> Delete(int Id, string OtherInventoryType);
        Response<AllItemAttributes> GetForAdd(string OtherInventoryType);
        Response<ReturnWithFilters<object>> GetCabinetPowerLibraryEnabledAtt(CombineFilters CombineFilters, bool WithFilterData, ParameterPagination parameterPagination);
        Response<ReturnWithFilters<object>> GetCabinetTelecomLibraryEnabledAtt(CombineFilters CombineFilters, bool WithFilterData, ParameterPagination parameterPagination);
        Response<ReturnWithFilters<object>> GetSolarLibraryEnabledAtt(CombineFilters CombineFilters, bool WithFilterData, ParameterPagination parameterPagination);
        Response<ReturnWithFilters<object>> GetGeneratorLibraryEnabledAtt(CombineFilters CombineFilters, bool WithFilterData, ParameterPagination parameterPagination);
    }
}
