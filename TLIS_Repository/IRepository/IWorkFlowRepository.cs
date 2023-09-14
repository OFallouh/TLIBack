using System;
using System.Collections.Generic;
using System.Text;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModels.StepActionDTOs;
using TLIS_DAL.ViewModels.WorkFlowDTOs;
using TLIS_Repository.Base;

namespace TLIS_Repository.IRepository
{
    public interface IWorkFlowRepository : IRepositoryBase<TLIworkFlow, WorkFlowViewModel, int>
    {
    }
    /*
    public interface IListWorkFlowRepository : IRepositoryBase<TLIworkFlow, ListWorkFlowViewModel, string>
    {
    }
    public interface IAddWorkFlowRepository : IRepositoryBase<TLIworkFlow, AddWorkFlowViewModel, string>
    {
    }
    //*/
    public interface IStepActionMailFromRepository : IRepositoryBase<TLIstepActionMail, PermissionWorkFlowViewModel, int>
    {
    }
    public interface IStepActionItemOptionRepository : IRepositoryBase<TLIstepActionItemOption, ListStepActionItemOptionViewModel, int>
    {
    }
    /*
    public interface IWorkFlowDeleteRepository : IRepositoryBase<TLIworkFlow, DeleteWorkFlowViewModel, string>
    {
    }
    public interface IEditWorkFlowRepository : IRepositoryBase<TLIworkFlow, EditWorkFlowViewModel, string>
    {
    }
    public interface IPermissionWorkFlowRepository : IRepositoryBase<TLIworkFlow, PermissionWorkFlowViewModel, string>
    {
    }
    //*/

}
