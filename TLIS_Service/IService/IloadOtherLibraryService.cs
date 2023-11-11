using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TLIS_DAL.Helper;
using TLIS_DAL.Helper.Filters;
using TLIS_DAL.Helpers;
using TLIS_DAL.ViewModelBase;
using TLIS_DAL.ViewModels.LoadOtherLibraryDTOs;

namespace TLIS_Service.IService
{
    public interface ILoadOtherLibraryService
    {
        Response<ReturnWithFilters<LoadOtherLibraryViewModel>> GetLoadOtherLibraries(ParameterPagination parameters, List<FilterObjectList> filters);
        Response<AllItemAttributes> GetById(int Id);
        Response<AllItemAttributes> AddLoadOtherLibrary(AddLoadOtherLibraryViewModel LoadOtherLibraryViewModel, string connectionString);
        Task<Response<AllItemAttributes>> EditLoadOtherLibrary(EditLoadOtherLibraryViewModel LoadOtherLibraryViewModel);
        Task<Response<AllItemAttributes>> DisableLoadOtherLibrary(int Id);
        Task<Response<AllItemAttributes>> DeletedLoadOtherLibrary(int Id);
        Response<AllItemAttributes> GetForAdd();
        Response<ReturnWithFilters<object>> GetLoadOtherLibrariesWithEnableAtt(CombineFilters CombineFilters, ParameterPagination parameterPagination, bool? isRefresh);
    }
}
