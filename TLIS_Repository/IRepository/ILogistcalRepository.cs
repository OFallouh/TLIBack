using System;
using System.Collections.Generic;
using System.Text;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModelBase;
using TLIS_DAL.ViewModels.LogisticalDTOs;
using TLIS_Repository.Base;

namespace TLIS_Repository.IRepository
{
    public  interface ILogistcalRepository : IRepositoryBase<TLIlogistical, LogisticalViewModel,int>
    {
        IEnumerable<BaseAttView> GetLogistical(string Part, string TableName, int RecordId);
        IEnumerable<BaseAttView> GetLogistical(string Part);
    }
}
