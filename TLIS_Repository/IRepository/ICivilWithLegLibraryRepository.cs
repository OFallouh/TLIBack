using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TLIS_DAL.Helper;
using TLIS_DAL.Helper.Filters;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModelBase;
using TLIS_DAL.ViewModels.CivilWithLegDTOs;
using TLIS_Repository.Base;

namespace TLIS_Repository.IRepository
{
    public interface ICivilWithLegLibraryRepository : IRepositoryBase<TLIcivilWithLegLibrary, CivilWithLegLibraryViewModel, int>
    {
        List<KeyValuePair<string, List<DropDownListFilters>>> GetRelatedTables();
    }
}
