using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TLIS_DAL.Helper;
using TLIS_DAL.Helper.Filters;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModels.MW_BULibraryDTOs;

namespace TLIS_Service.IService
{
    public interface IMW_BULibraryService
    {
        Task MW_BULibrarySeedDataForTest();
        Task<IEnumerable<TLImwBULibrary>> GetMW_BULibrary(ParameterPagination parameterPagination, MW_BULibraryFilter MW_BULibraryFilter);
        Task<MW_BULibraryViewModel> GetById(int Id);
        Task AddMW_BULibrary(AddMW_BULibraryViewModel addMW_BULibraryViewModel);
        Task EditMW_BULibrary(EditMW_BULibraryViewModel EditMW_BULibraryViewModel);
    }
}
