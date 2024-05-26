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

namespace TLIS_Service.IService
{
    public interface IMWInstService
    {
        Response<GetForAddMWDishInstallationObject> GetAttForAddMWDishInstallation(string TableName, int LibraryID, string SiteCode);
        Response<GetForAddMWDishInstallationObject> GetAttForAddMWODUInstallation(string TableName, int LibraryID, string SiteCode);
        Task<Response<GetForAddMWDishInstallationObject>> EditMWInstallation(int UserId, object MWInstallationViewModel, string TableName, int? TaskId);
        Response<GetForAddLoadObject> GetMWDishInstallationById(int MWInsId, string TableName);
        Task<Response<GetForAddMWDishInstallationObject>> EditMWDishInstallation(int UserId, EditMWDishInstallationObject MWInstallationViewModel, string TableName, int? TaskId);
        Response<ObjectInstAtts> GetAttForAdd(string TableName, int LibraryID, string SiteCode);
        public Response<bool> DismantleLoads(string sitecode, int LoadId, string LoadName, int? TaskId);
        Response<GetForAddMWDishInstallationObject> AddMWInstallation(int UserId, object MWInstallationViewModel, string TableName, string SiteCode, string ConnectionString, int? TaskId);    
        Response<ReturnWithFilters<MW_ODUViewModel>> getMW_ODU(List<FilterObjectList> filters, bool WithFilterData, ParameterPagination parameters);
        Response<ReturnWithFilters<MW_BUViewModel>> getMW_BU(List<FilterObjectList> filters, bool WithFilterData, ParameterPagination parameters);
        Response<ReturnWithFilters<MW_DishViewModel>> getMW_Dish(List<FilterObjectList> filters, bool WithFilterData, ParameterPagination parameters);
        Response<ReturnWithFilters<MW_RFUViewModel>> getMW_RFU(List<FilterObjectList> filters, bool WithFilterData, ParameterPagination parameters);
        Response<GetForAddLoadObject> GetById(int MWInsId, string TableName);
        public Response<List<InstallationPlaceViewModel>> GetInstallationType(string TableName);
        Response<List<InstallationPlaceViewModel>> GetInstallationPlaces(string TableName, string LoadType);
        Response<List<MW_PortViewModel>> GetMW_PortsForMW_RFUInstallation(int AllCivilInstId);
        Response<List<MW_BULibraryViewModel>> GetMW_BULibrariesForMW_BUInstallation();
        Response<GetEnableAttribute> GetMWDishInstallationWithEnableAtt(string SiteCode, string ConnectionString);

        Response<List<MW_Free_BUInstDto>> GetMw_Free_BuInst(int AllCivilInstId);
        Response<GetForAddLoadObject> GetMWODUInstallationById(int MWInsId, string TableName);
        Response<List<MW_Free_BUInstDto>> GetMw_Free_Cascade_BuInst(int AllCivilInstId);

        Response<List<MW_PortViewModel>> GetFreePortOnBU(int BUid);
        Response<List<MW_DishGetForAddViewModel>> GetFreeDishesForMW_ODU(int AllCivilInstId);

        Response<ObjectInstAtts> GetAttForAddForMW_ODUOnly(string TableName, int LibraryID, string SiteCode, int AllCivilInstId);
        Response<List<MW_PortViewModel>> GetPortCascadedByBUId(int BUId, int? MainBUId);
        Response<GetEnableAttribute> GetMWODUInstallationWithEnableAtt(string SiteCode, string ConnectionString);
    }
}
