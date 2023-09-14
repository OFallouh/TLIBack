using System;
using System.Collections.Generic;
using System.Text;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModels.ActionItemOptionDTOs;
using TLIS_Repository.Base;

namespace TLIS_Repository.IRepository
{
    public interface IActionItemOptionListRepository : IRepositoryBase<TLIactionItemOption, ActionItemOptionListViewModel, int>
    {
    }
    /*
    public interface IActionItemOptionAddRepository : IRepositoryBase<TLIactionItemOption, ActionItemOptionAddViewModel, string>
    {
    }
    public interface IActionItemOptionEditRepository : IRepositoryBase<TLIactionItemOption, ActionItemOptionEditViewModel, string>
    {
    }
    //*/
}
