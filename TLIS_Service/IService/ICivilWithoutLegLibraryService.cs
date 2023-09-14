using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TLIS_DAL.Helper;
using TLIS_DAL.Helper.Filters;
using TLIS_DAL.Helpers;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModelBase;
using TLIS_DAL.ViewModels.CivilWithoutLegDTOs;

namespace TLIS_Service.IService
{
   public interface ICivilWithoutLegLibraryService
    {
        Task<Response<ReturnWithFilters<CivilWithoutLegLibraryViewModel>>> GetCivilWithoutLegLibrary(List<FilterObjectList> filters, bool WithFilterData, ParameterPagination parameters);
        Task<CivilWithoutLegLibraryViewModel> GetById(int Id);

        Task AddCivilWithoutLegLibrary(AddCivilWithoutLegLibraryViewModel addCivilNonSteelLibraryViewModel);
        Task EditCivilWithoutLegLibrary(EditCivilWithoutLegLibraryViewModel EditCivilNonSteelLibraryViewModel);
    }
}
