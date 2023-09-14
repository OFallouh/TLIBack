using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TLIS_DAL.Helper;
using TLIS_DAL.Helper.Filters;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModels.CivilNonSteelDTOs;


namespace TLIS_Service.IService
{
    public interface ICivilNonSteelLibraryService
    {
        Task<IEnumerable<TLIcivilNonSteelLibrary>> GetCivilNonSteelLibrary(ParameterPagination parameterPagination, CivilNonSteelLibraryFilter civilNonSteelLibraryFilter);
        Task<CivilNonSteelLibraryViewModel> GetById(int Id);

       Task AddCivilNonSteelLibrary(AddCivilNonSteelLibraryViewModel addCivilNonSteelLibraryViewModel);
       Task EditCivilNonSteelLibrary(EditCivilNonSteelLibraryViewModel editCivilNonSteelLibraryViewModel);
      
        Task Disable_EnableCivilNonSteelLibrary(int id, bool active, CivilNonSteelLibraryViewModel civilNonSteelLibraryViewModel);
    }
}
