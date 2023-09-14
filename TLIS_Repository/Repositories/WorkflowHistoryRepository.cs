using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;
using TLIS_DAL;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModels.WorkflowHistoryDTOs;
using TLIS_Repository.Base;
using TLIS_Repository.IRepository;

namespace TLIS_Repository.Repositories
{
    public class WorkflowHistoryRepository: RepositoryBase<TLIworkflowTableHistory, WorkflowHistoryViewModel, int>, IWorkflowHistoryRepository
    {

        private readonly ApplicationDbContext _context;
        //private readonly IHttpContextAccessor _httpContextAccessor;
        //private ISession _session => _httpContextAccessor.HttpContext.Session;
        IMapper _mapper;
        public WorkflowHistoryRepository(ApplicationDbContext context, IMapper mapper) : base(context, mapper)
        {
            _context = context;
            _mapper = mapper;

        }

        public void AddWorkflowHistory(AddWorkflowHistoryViewModel addModel)
        {
            using (TransactionScope transaction = new TransactionScope())
            {
                try
                {
                    TLIworkflowTableHistory entity = _mapper.Map<TLIworkflowTableHistory>(addModel); ;
                    entity.Date = DateTime.Now;
                    entity.PreviousHistoryId = _context.TLIworkflowTableHistory.Where(x => (x.RecordId == addModel.RecordId && x.TablesNameId == addModel.TablesNameId && x.TicketId == addModel.TicketId)).OrderByDescending(t => t.Date).Select(x => x.Id).FirstOrDefault();
                    _context.TLIworkflowTableHistory.Add(entity);
                    _context.SaveChanges();
                    if (addModel.HistoryTypeId == _context.TLIhistoryType.Where(x=>x.Name=="Update").Select(x=>x.Id).FirstOrDefault())
                    {
                        foreach (var value in addModel.values)
                        {
                            TLIhistoryDetails historyDetails = new TLIhistoryDetails();
                            historyDetails.WorkflowTableHistoryId = entity.Id;
                            historyDetails.AttName = value.Item1;
                            historyDetails.OldValue = value.Item2;
                            historyDetails.NewValue = value.Item3;
                            _context.TLIhistoryDetails.Add(historyDetails);
                            _context.SaveChanges();
                        }
                    }
                    transaction.Complete();
                }
                catch (Exception)
                {

                }
            }
        }

        public bool IsExecutedAction(int TicketActionId)
        {
            var requests = GetAllAsQueryable().Where(x => x.TicketActionId == TicketActionId);
            if (requests != null)
            {
                if (requests.Count() > 0)
                {
                    return true;
                }
            }
            return false;
        }
    }
}