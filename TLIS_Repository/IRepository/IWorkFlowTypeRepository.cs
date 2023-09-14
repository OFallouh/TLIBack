using System;
using System.Collections.Generic;
using System.Text;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModels.WorkFlowTypeDTOs;
using TLIS_Repository.Base;

namespace TLIS_Repository.IRepository
{
    public interface IWorkFlowTypeRepository : IRepositoryBase<TLIworkFlowType, WorkFlowTypeViewModel, int>
    {
    }
    /*
    public interface IListWorkFlowTypeRepository : IRepositoryBase<TLIworkFlowType, ListWorkFlowTypeViewModel, string>
    {
    }
    public interface IAddWorkFlowTypeRepository : IRepositoryBase<TLIworkFlowType, AddWorkFlowTypeViewModel, string>
    {
    }
    public interface IEditWorkFlowTypeRepository : IRepositoryBase<TLIworkFlowType, EditWorkFlowTypeViewModel, string>
    {
    }
    public interface IDeleteWorkFlowTypeRepository : IRepositoryBase<TLIworkFlowType, DeleteWorkFlowTypeViewModel, string>
    {
    }
    //*/
}
