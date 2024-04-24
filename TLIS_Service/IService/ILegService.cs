using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TLIS_DAL.Helper;
using TLIS_DAL.Helper.Filters;
using TLIS_DAL.Helpers;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModels.LegDTOs;
namespace TLIS_Service.IService
{
    public interface ILegService
    {
        Task<IEnumerable<TLIleg>> GetCivilNonSteelLibrary(ParameterPagination parameterPagination, LegFilter legFilter);
        Task<LegViewModel> GetById(int Id);
        Response<List<LegViewModel>> GetLegsByAllCivilInstId(int civilid);
        Task AddLeg(AddLegViewModel addLegViewModel);
        Task EditLeg(EditLegViewModel EditLegViewModel);
        Response<List<LegViewModel>> GetLegsByCivilId(int CivilId);
    }
}
