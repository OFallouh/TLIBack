using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TLIS_DAL.Helper;
using TLIS_DAL.Helper.Filters;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModels.MW_RFUDTOs;

namespace TLIS_Service.IService
{
    public interface IMW_RFULibraryService
    {
        Task<IEnumerable<TLImwRFULibrary>> GetMW_RFULibrary(ParameterPagination parameterPagination, MW_RFULibraryFilter MW_RFULibraryFilter);
        void AddMW_RFULibrary(MW_RFULibraryViewModel addMW_RFULibraryViewModel);
        void EditMW_RFULibrary(MW_RFULibraryViewModel EditMW_RFULibraryViewModel);
    }
}
