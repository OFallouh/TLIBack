using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TLIS_DAL.Helper;
using TLIS_DAL.Helper.Filters;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModels.MW_ODUDTOs;

namespace TLIS_Service.IService
{
    public interface IMW_ODULibraryService
    {
        Task<IEnumerable<TLImwODULibrary>> GetMW_ODULibrary(ParameterPagination parameterPagination, MW_ODULibraryFilter MW_ODULibraryFilter);
        void AddMW_ODULibrary(MW_ODULibraryViewModel addMW_ODULibraryViewModel);
        void EditMW_ODULibrary(MW_ODULibraryViewModel EditMW_ODULibraryViewModel);

    }
}
