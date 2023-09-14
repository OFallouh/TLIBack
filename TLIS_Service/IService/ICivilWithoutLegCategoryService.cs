using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TLIS_DAL.Helpers;
using TLIS_DAL.ViewModels.CivilWithoutLegCategoryDTOs;

namespace TLIS_Service.IService
{
    public interface ICivilWithoutLegCategoryService
    {
        Task<Response<IEnumerable<CivilWithoutLegCategoryViewModel>>> GetList();
        Task<Response<CivilWithoutLegCategoryViewModel>> Add(AddCivilWithoutLegCategoryViewModel categoryViewModel);
        Task<Response<CivilWithoutLegCategoryViewModel>> Edit(CivilWithoutLegCategoryViewModel CivilWithoutLegCategoryViewModel);
        Task<Response<CivilWithoutLegCategoryViewModel>> GetCivilWithoutLegCategory(int Id);
        Response<CivilWithoutLegCategoryViewModel> GetCivilWithoutLegCategoryByName(string Name);
        Task<Response<CivilWithoutLegCategoryViewModel>> Disable_EnableCivilWithoutLegCategory(int Id, bool Disable);

    }
}
