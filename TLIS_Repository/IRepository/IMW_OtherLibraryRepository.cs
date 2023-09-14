using System;
using System.Collections.Generic;
using System.Text;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModelBase;
using TLIS_DAL.ViewModels.MW_OtherLibraryDTOs;
using TLIS_Repository.Base;

namespace TLIS_Repository.IRepository
{
    public interface IMW_OtherLibraryRepository:IRepositoryBase<TLImwOtherLibrary, MW_OtherLibraryViewModel, int>
    {
        List<KeyValuePair<string, List<DropDownListFilters>>> GetRelatedTables();
    }
}
