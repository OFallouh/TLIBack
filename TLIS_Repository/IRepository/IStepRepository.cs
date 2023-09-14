using System;
using System.Collections.Generic;
using System.Text;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModels.StepDTOs;
using TLIS_Repository.Base;

namespace TLIS_Repository.IRepository
{
    public interface IStepListRepository : IRepositoryBase<TLIstep, StepListViewModel, int>
    {
    }
    public interface IStepAddRepository : IRepositoryBase<TLIstep, StepAddViewModel, int>
    {
    }
    public interface IStepEditRepository : IRepositoryBase<TLIstep, StepEditViewModel, int>
    {
    }
}
