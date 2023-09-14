using System;
using System.Collections.Generic;
using System.Text;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModelBase;
using TLIS_DAL.ViewModels.MW_DishLbraryDTOs;
using TLIS_Repository.Base;

namespace TLIS_Repository.IRepository
{
    public interface IMW_DishLibraryRepository:IRepositoryBase<TLImwDishLibrary, MW_DishLibraryViewModel,int>
    {
        List<KeyValuePair<string, List<DropDownListFilters>>> GetRelatedTables();
    }
}
