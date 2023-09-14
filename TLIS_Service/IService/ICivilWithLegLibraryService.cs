using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TLIS_DAL.Helper;
using TLIS_DAL.Helper.Filters;
using TLIS_DAL.Helpers;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModelBase;
using TLIS_DAL.ViewModels.CivilWithLegDTOs;
using TLIS_DAL.ViewModels.CivilWithLegLibraryDTOs;
using TLIS_DAL.ViewModels.DynamicAttDTOs;
using TLIS_DAL.ViewModels.DynamicAttInstValueDTOs;

namespace TLIS_Service.IService
{
    public interface ICivilWithLegLibraryService
    {
        Task<Response<ReturnWithFilters<CivilWithLegLibraryViewModel>>> getCivilWithLegLibrariesAsync(List<FilterObjectList> filters, bool WithFilterData, ParameterPagination parameters);
        Response<AllItemAttributes> GetById(int Id);
        Response<CivilWithLegLibraryViewModel> AddCivilWithLegLibrary(AddCivilWithLegLibraryViewModel CivilWithLegLibraryViewModel = null, List<AddDynamicLibAttValueViewModel> DynamicLibAttsValue = null, List<AddDynamicAttInstValueViewModel> DynamicAttsInstValue = null);
        Task EditCivilWithLegLibrary(EditCivilWithLegLibraryViewModels editCivilWithLegLibraryViewModels);
        Task<Response<CivilWithLegLibraryViewModel>> Disable(int Id);
    }
}
