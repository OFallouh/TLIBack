using System;
using System.Collections.Generic;
using System.Text;
using TLIS_DAL.Helper;
using TLIS_DAL.Helpers;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModels.ComplixFilter;
using TLIS_DAL.ViewModels.IntegrationBinding;
using TLIS_DAL.ViewModels.IntegrationDto;
using TLIS_DAL.ViewModels.IntegrationDTOs;

namespace TLIS_Service.IService
{
    public interface IexternalSysService
    {
        public Response<List<ExternalPermission>> GetAllExternalPermission();
        Response<string> CreateExternalSys(AddExternalSysBinding mod,int UserId);
        Response<bool> DisableExternalSys(int id, int UserId);
        Response<bool> DeleteExternalSys(int id, int UserId);
        Response<string> EditExternalSys(EditExternalSysBinding mod, int UserId);
        Response<List<GetAllExternalSysDto>> GetAllExternalSys();
        Response<GetAllExternalSysDto> GetByIdExternalSys(int id);
        Response<List<IntegrationViewModel>> GetAllIntegrationAPI();
        Response<string> RegenerateToken(int id);
        Response<List<TLIintegrationAccessLog>> GetListErrorLog(ClxFilter f);
        Response<string> GetListErrorLogExport();

    }
}
