using System;
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
using TLIS_DAL.ViewModels.CivilWithLegsDTOs;
using TLIS_DAL.ViewModels.GeneratorDTOs;
using TLIS_DAL.ViewModels.GeneratorLibraryDTOs;
using TLIS_DAL.ViewModels.SolarDTOs;
using TLIS_DAL.ViewModels.SolarLibraryDTOs;

namespace TLIS_Service.IService
{
    public interface IOtherInventoryInstService
    {
        Response<ObjectInstAtts> GetAttForAdd(string TableName, string OtherInventoryLibraryType, int OtherInventoryId, string SiteCode);

        Response<ObjectInstAtts> AddOtherInventoryInstallation(object model, string TableName, string SiteCode, string ConnectionString, int? TaskId, int UserId);

        Task<Response<ObjectInstAtts>> EditOtherInventoryInstallation(object model, string TableName, int? TaskId);

        Response<ObjectInstAtts> GetById(int OtherInventoryInsId, string TableName);
        public Response<bool> DismantleOtherInventory(string SiteCode, int OtherInventoryId, string OtherInventoryName, int? TaskId);
        Response<ReturnWithFilters<object>> GetCabinetBySiteWithEnabledAtt(SiteBaseFilter BaseFilter, bool WithFilterData, CombineFilters CombineFilters, ParameterPagination parameterPagination, string LibraryType);
        Response<ReturnWithFilters<object>> GetSolarBySiteWithEnabledAtt(SiteBaseFilter BaseFilter, bool WithFilterData, CombineFilters CombineFilters, ParameterPagination parameterPagination);
        Response<ReturnWithFilters<object>> GetGeneratorBySiteWithEnabledAtt(SiteBaseFilter BaseFilter, bool WithFilterData, CombineFilters CombineFilters, ParameterPagination parameterPagination);
    }
}
