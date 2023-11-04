using System;
using System.Collections.Generic;
using System.Text;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModelBase;
using TLIS_DAL.ViewModels.MW_ODUDTOs;
using TLIS_Repository.Base;

namespace TLIS_Repository.IRepository
{
    public interface IMW_ODURepository : IRepositoryBase<TLImwODU, MW_ODUViewModel, int>
    {
        List<KeyValuePair<string, List<DropDownListFilters>>> GetRelatedTables(string SiteCode);
        List<KeyValuePair<string, List<DropDownListFilters>>> GetRelatedTablesForEdit(string SiteCode, int AllCivilInstId);
    }
}
