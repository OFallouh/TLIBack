using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TLIS_DAL.Helper;
using TLIS_DAL.Helper.Filters;
using TLIS_DAL.Helpers;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModelBase;
using TLIS_DAL.ViewModels.CivilLoadsDTOs;
using TLIS_DAL.ViewModels.CivilWithLegLibraryDTOs;
using TLIS_DAL.ViewModels.SideArmDTOs;
using TLIS_DAL.ViewModels.SideArmInstallationPlaceDTOs;
using TLIS_DAL.ViewModels.SideArmTypeDTOs;

namespace TLIS_Service.IService
{
    public interface ISideArmService
    {

        Response<SideArmViewDto> AddSideArm(SideArmViewDto addSideArms, string SiteCode, int? TaskId, int UserId, string ConnectionString,bool ExternalSys);
        Response<GetEnableAttribute> GetSideArmInstallationWithEnableAtt(string? SiteCode, string ConnectionString);
        Response<ReturnWithFilters<SideArmDisplayedOnTableViewModel>> getSideArms(CivilLoadsFilter BaseFilter, bool WithFilterData, List<FilterObjectList> filters);
        Response<ReturnWithFilters<object>> GetSideArmsWithEnabledAtt(CivilLoadsFilter BaseFilter, bool WithFilterData, ParameterPagination parameterPagination, CombineFilters CombineFilters, int? CivilId, string CivilType);
        Response<List<KeyValuePair<string, int>>> getSideArmsForAdd(string SiteCode, int CivilId, int? LegId, int? MinHeight, int? MaxHeight, int? NumberOfLoadsOnSideArm, int? MinAzimuth, int? MaxAzimuth);
        Response<GetForAddLoadObject> GetById(int Id,int UserId, bool ExternalSys);
        Response<bool> DismantleSideArm(string SiteCode, int sideArmId, int? TaskId, string ConnectionString, int UserId, bool ExternalSys);
        Response<ObjectInstAttsForSideArm> GetSideArmById(int SideArmId, string TableName);

        Task<Response<EditSidearmInstallationObject>> UpdateSideArm(EditSidearmInstallationObject SideArmViewModel, int? TaskId, int UserId, string ConnectionStrin, bool ExternalSys);
        Task<Response<IEnumerable<SideArmInstallationPlaceViewModel>>> GetSideArmInstallationPlace(int civilInstallationPlaceType);
        Task<Response<TLIsideArmInstallationPlace>> AddSideArmInstallationPlace(AddSideArmInstallationPlaceViewModel SideArmInstallationPlace);
        Task<Response<TLIsideArmInstallationPlace>> UpdateSideArmInstallationPlace(EditSideArmInstallationPlaceViewModel SideArmInstallationPlace);
        Response<GetForAddCivilLoadObject> GetAttForAdd(int LibraryId, int UserId, bool ExternalSys);
        Response<IEnumerable<SideArmTypeViewModel>> GetSideArmType();
        Response<List<SideArmInstallationPlaceViewModel>> GetSideArmInstallationPlace(string CivilType, int SideArmTypeId);
        Response<List<SideArmTypeViewModel>> GetSideArmTypes(string tablename);
        Response<List<SideArmViewModel>> GetSideArmsByAllCivilInstId(int AllCivilInstId);
        Response<List<SideArmViewModel>> GetSideArmsByFilters(int AllCivilInstId, float? MaxAzimuth, float? MinAzimuth, float? MaxHeightBase, float? MinHeightBase);
    }
}
