using System;
using System.Collections.Generic;
using System.Text;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModels.WorkflowHistoryDTOs;
using TLIS_Repository.Base;

namespace TLIS_Repository.IRepository
{
    public interface IWorkflowHistoryRepository : IRepositoryBase<TLIworkflowTableHistory, WorkflowHistoryViewModel, int>
    {
        void AddWorkflowHistory(AddWorkflowHistoryViewModel addModel);
        bool IsExecutedAction(int TicketActionId);
    }
   
}
