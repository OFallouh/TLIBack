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
using TLIS_DAL.ViewModels.MW_DishDTOs;
using TLIS_DAL.ViewModels.PowerDTOs;
using TLIS_DAL.ViewModels.PowerTypeDTOs;
using TLIS_DAL.ViewModels.SideArmDTOs;

namespace TLIS_Service.IService
{
    public interface IPowerService
    {
        Response<ObjectInstAtts> GetAttForAdd(int Id, string SiteCode);
        Response<bool> DismantleLoads(string sitecode, int LoadId, string LoadName, int? TaskId, int UserId, string connectionString);
        Response<ObjectInstAtts> AddPower(AddPowerViewModel PowerViewModel, string SiteCode, string ConnectionString, int? TaskId);
        Task<Response<ObjectInstAtts>> EditPower(EditPowerViewModel PowerViewModel, int? TaskId);
        Response<ObjectInstAttsForSideArm> GetById(int Id);
        Response<ReturnWithFilters<PowerViewModel>> GetList(List<FilterObjectList> filters, bool WithFilterData, ParameterPagination parameters);
        Response<List<PowerTypeViewModel>> GetPowerTypes();
        Task<Response<GetForAddMWDishInstallationObject>> EditPowerInstallation(object PowerInstallationViewModel, int? TaskId, int UserId, string ConnectionString);
        Response<GetForAddMWDishInstallationObject> GetAttForAddPowerInstallation(int LibraryID, string SiteCode);
        Response<GetForAddLoadObject> GetPowerInstallationById(int PowerId);
        Response<GetEnableAttribute> GetPowerInstallationWithEnableAtt(string SiteCode, string ConnectionString);
        Response<GetForAddMWDishInstallationObject> AddPowerInstallation(object PowerInstallationViewModel, string SiteCode, string ConnectionString, int? TaskId, int UserId);
    }
}
