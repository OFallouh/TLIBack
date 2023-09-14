using System;
using System.Collections.Generic;
using System.Text;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModels.AgendaDTOs;
using TLIS_Repository.Base;

namespace TLIS_Repository.IRepository
{
    public interface IAgendaRepository : IRepositoryBase<TLIagenda, AgendaViewModel, int>
    {
    }

    public interface IAgendaGroupRepository : IRepositoryBase<TLIagendaGroup, AgendaGroupViewModel, int>
    {
    }
}
