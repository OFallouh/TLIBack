using System;
using System.Collections.Generic;
using System.Text;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModels.TicketActionDTOs;
using TLIS_DAL.ViewModels.TicketDTOs;
using TLIS_DAL.ViewModels.TicketOptionNoteDTOs;
using TLIS_DAL.ViewModels.TicketTargetDTOs;
using TLIS_Repository.Base;

namespace TLIS_Repository.IRepository
{
    public interface ITicketRepository : IRepositoryBase<TLIticket, ListTicketViewModel, int>
    {
    }
    public interface ITicketActionRepository : IRepositoryBase<TLIticketAction, TicketActionViewModel, int>
    {
    }
    public interface ITicketTargetRepository : IRepositoryBase<TLIticketTarget, TicketTargetViewModel, int>
    {
    }
    public interface ITicketOptionNoteRepository : IRepositoryBase<TLIticketOptionNote, TicketOptionNoteViewModel, int>
    {
    }
}
