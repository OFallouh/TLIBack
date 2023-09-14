using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TLIS_DAL.Helper;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModelBase;
using TLIS_DAL.ViewModels.CivilWithoutLegDTOs;
using TLIS_Repository.Base;

namespace TLIS_Repository.IRepository
{
    public interface ICivilWithoutLegLibraryRepository : IRepositoryBase <TLIcivilWithoutLegLibrary, CivilWithoutLegLibraryViewModel, int>
    {
        List<KeyValuePair<string, List<DropDownListFilters>>> GetRelatedTables();
    }
}