using System;
using System.Collections.Generic;
using System.Text;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModels.ActionDTOs;
using TLIS_Repository.Base;

namespace TLIS_Repository.IRepository
{
    public interface IActionRepository : IRepositoryBase<TLIaction, ActionListViewModel, int>
    {
    }
}
