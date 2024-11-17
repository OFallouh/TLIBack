using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TLIS_DAL.Helper;
using TLIS_DAL.Helper.Filters;
using TLIS_DAL.Helpers;
using TLIS_DAL.ViewModelBase;
using TLIS_DAL.ViewModels;
using TLIS_DAL.ViewModels.MW_BUDTOs;
using TLIS_DAL.ViewModels.MW_DishDTOs;
using TLIS_DAL.ViewModels.MW_ODUDTOs;
using TLIS_DAL.ViewModels.MW_RFUDTOs;
using TLIS_DAL.ViewModels.MW_PortDTOs;
using TLIS_DAL.ViewModels.MW_BULibraryDTOs;
using TLIS_DAL.ViewModels.SideArmDTOs;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModels.CivilWithLegLibraryDTOs;
using TLIS_DAL.ViewModels.CivilLoadsDTOs;
using TLIS_DAL.ViewModels.Mw_OtherDTOs;
using static TLIS_Service.Services.MWInstService;

namespace TLIS_Service.IService
{
    public interface IMWInstService
    {
        Response<GetForAddMWDishInstallationObject> AddMWRFUInstallation(object AddMWRFUInstallation, string TableName, string SiteCode, string ConnectionString, int? TaskId, int UserId,bool ExternalSy);
        Response<GetForAddMWDishInstallationObject> GetAttForAddMWDishInstallation(string TableName, int LibraryID, string SiteCode, int UserId, bool ExternalSy);
        Response<GetForAddMWDishInstallationObject> GetAttForAddMWRFUInstallation(string TableName, int LibraryID, string SiteCode, int UserId, bool ExternalSy);
        Response<GetEnableAttribute> GetMWBUInstallationWithEnableAtt(string? SiteCode, string ConnectionString, int? UserId);
        Task<Response<GetForAddMWDishInstallationObject>> EditMWOtherInstallation(int UserId, EditMWOtherInstallationObject editmwOtherInstallationObject, string TableName, int? TaskId, string ConnectionString, bool ExternalSy);
        Response<GetForAddLoadObject> GetMWOtherInstallationById(int MWOtherId, string TableName, int UserId, bool ExternalSy);
        Response<GetEnableAttribute> GetMWRFUInstallationWithEnableAtt(string? SiteCode, string ConnectionString, int? UserId);
        Response<GetEnableAttribute> GetMWOtherInstallationWithEnableAtt(string? SiteCode, string ConnectionString, int? UserId);
        Response<GetForAddMWDishInstallationObject> GetAttForAddMWOtherInstallation(string TableName, int LibraryID, string SiteCode, int UserId, bool ExternalSy);
        Response<GetForAddLoadObject> GetMWBUInstallationById(int MWInsId, string TableName, int UserId, bool ExternalSy);
        Response<GetForAddLoadObject> GetMWRFUInstallationById(int MWInsId, string TableName, int UserId, bool ExternalSy);
        Task<Response<GetForAddMWDishInstallationObject>> EditMWRFUInstallation(EditMWRFUInstallationObject editMWRFUInstallationObject, string TableName, string ConnectionString, int? TaskId, int UserId, bool ExternalSy);
        Task<Response<GetForAddMWDishInstallationObject>> EditMWBUInstallation(int UserId, EditMWBUInstallationObject MWInstallationViewModel, string TableName, int? TaskId, string ConnectionString,bool ExternalSy);
        Response<GetForAddMWDishInstallationObject> GetAttForAddMWBUInstallation(string TableName, int LibraryID, string SiteCode, int UserId, bool ExternalSy);
        Task<Response<GetForAddMWDishInstallationObject>> EditMWODUInstallation(int UserId, EditMWODUInstallationObject MWInstallationViewModel, string TableName, int? TaskId, string ConnectionString, bool ExternalSy);
        Response<GetForAddMWDishInstallationObject> GetAttForAddMWODUInstallation(string TableName, int LibraryID, string SiteCode, int UserId, bool ExternalSy);
        Task<Response<GetForAddMWDishInstallationObject>> EditMWInstallation(int UserId, object MWInstallationViewModel, string TableName, int? TaskId);
        Response<GetForAddLoadObject> GetMWDishInstallationById(int MWInsId, string TableName, int UserId, bool ExternalSy);
        Task<Response<GetForAddMWDishInstallationObject>> EditMWDishInstallation(int UserId, EditMWDishInstallationObject MWInstallationViewModel, string TableName, int? TaskId, string ConnectionString, bool ExternalSy);
        Response<ObjectInstAtts> GetAttForAdd(string TableName, int LibraryID, string SiteCode);
        Response<bool> DismantleLoads(string sitecode, int LoadId, string LoadName, int? TaskId, int UserId, string connectionString, bool ExternalSys);
        Response<GetForAddMWDishInstallationObject> AddMWInstallation(int UserId, object MWInstallationViewModel, string TableName, string SiteCode, string ConnectionString, int? TaskId, bool ExternalSy);    
        Response<ReturnWithFilters<MW_ODUViewModel>> getMW_ODU(List<FilterObjectList> filters, bool WithFilterData, ParameterPagination parameters);
        Response<ReturnWithFilters<MW_BUViewModel>> getMW_BU(List<FilterObjectList> filters, bool WithFilterData, ParameterPagination parameters);
        Response<ReturnWithFilters<MW_DishViewModel>> getMW_Dish(List<FilterObjectList> filters, bool WithFilterData, ParameterPagination parameters);
        Response<ReturnWithFilters<MW_RFUViewModel>> getMW_RFU(List<FilterObjectList> filters, bool WithFilterData, ParameterPagination parameters);
        Response<GetForAddLoadObject> GetById(int MWInsId, string TableName);
        public Response<List<InstallationPlaceViewModel>> GetInstallationType(string TableName);
        Response<List<InstallationPlaceViewModel>> GetInstallationPlaces(string TableName, string LoadType);
        Response<List<MW_PortViewModel>> GetMW_PortsForMW_RFUInstallation(int AllCivilInstId);
        Response<List<MW_BULibraryViewModel>> GetMW_BULibrariesForMW_BUInstallation();

        Response<List<MW_Free_BUInstDto>> GetMw_Free_BuInst(int AllCivilInstId);
        Response<GetForAddLoadObject> GetMWODUInstallationById(int MWInsId, string TableName, int UserId, bool ExternalSy);
        Response<List<MW_Free_BUInstDto>> GetMw_Free_Cascade_BuInst(int AllCivilInstId);

        Response<List<MW_PortViewModel>> GetFreePortOnBU(int BUid);
        Response<List<MW_DishGetForAddViewModel>> GetFreeDishesForMW_ODU(int AllCivilInstId);

        Response<ObjectInstAtts> GetAttForAddForMW_ODUOnly(string TableName, int LibraryID, string SiteCode, int AllCivilInstId);
        Response<List<MW_PortViewModel>> GetPortCascadedByBUId(int BUId, int? MainBUId);
        Response<GetEnableAttribute> GetMWODUInstallationWithEnableAtt(string? SiteCode, string ConnectionString, int? UserId);
        Response<GetEnableAttribute> GetMWDishInstallationWithEnableAtt(string? SiteCode, string ConnectionString, int? UserId);
    }
}
