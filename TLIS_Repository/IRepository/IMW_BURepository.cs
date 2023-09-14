using System;
using System.Collections.Generic;
using System.Text;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModelBase;
using TLIS_DAL.ViewModels.MW_BUDTOs;
using TLIS_Repository.Base;

namespace TLIS_Repository.IRepository
{
    public interface IMW_BURepository:IRepositoryBase<TLImwBU,MW_BUViewModel,int>
    {
        List<KeyValuePair<string, List<DropDownListFilters>>> GetRelatedTables(string SiteCode);
    }
}
