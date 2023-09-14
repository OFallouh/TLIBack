using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TLIS_DAL.Helper;
using TLIS_DAL.Helper.Filters;
using TLIS_DAL.Helpers;
using TLIS_DAL.ViewModelBase;
using TLIS_DAL.ViewModels.PowerDTOs;
using TLIS_DAL.ViewModels.PowerTypeDTOs;
using TLIS_DAL.ViewModels.SideArmDTOs;

namespace TLIS_Service.IService
{
    public interface IPowerService
    {
        Response<ObjectInstAtts> GetAttForAdd(int Id, string SiteCode);
        public Response<bool> DismantleLoads(string sitecode, int LoadId, string LoadName);
        Response<ObjectInstAtts> AddPower(AddPowerViewModel PowerViewModel, string SiteCode, string ConnectionString);
        Task<Response<ObjectInstAtts>> EditPower(EditPowerViewModel PowerViewModel);
        Response<ObjectInstAttsForSideArm> GetById(int Id);
        Response<ReturnWithFilters<PowerViewModel>> GetList(List<FilterObjectList> filters, bool WithFilterData, ParameterPagination parameters);
        Response<List<PowerTypeViewModel>> GetPowerTypes();
    }
}
