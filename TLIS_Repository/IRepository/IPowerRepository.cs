using System;
using System.Collections.Generic;
using System.Text;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModelBase;
using TLIS_DAL.ViewModels.PowerDTOs;
using TLIS_Repository.Base;

namespace TLIS_Repository.IRepository
{
    public interface IPowerRepository:IRepositoryBase<TLIpower, PowerViewModel, int>
    {
        List<KeyValuePair<string, List<DropDownListFilters>>> GetRelatedTables();
    }
}
