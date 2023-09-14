using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModelBase;
using TLIS_DAL.ViewModels.CivilLoadsDTOs;
using TLIS_Repository.Base;

namespace TLIS_Repository.IRepository
{
    public interface ICivilLoadsRepository:IRepositoryBase<TLIcivilLoads, CivilLoadsViewModel, int>
    {
        void AddCivilLoad(AddCivilLoadsViewModel civilLoadsViewModel, int allLoadInstId, string SiteCode);

        List<KeyValuePair<string, List<DropDownListFilters>>> GetRelatedTables(string SiteCode);
    }
}
