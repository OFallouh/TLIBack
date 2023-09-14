using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TLIS_DAL.Helper;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModelBase;
using TLIS_DAL.ViewModels.MW_ODUDTOs;
using TLIS_Repository.Base;

namespace TLIS_Repository.IRepository
{
   public interface IMW_ODULibraryRepository : IRepositoryBase<TLImwODULibrary, MW_ODULibraryViewModel, int>
    {
        List<KeyValuePair<string, List<DropDownListFilters>>> GetRelatedTables();
    }
}
