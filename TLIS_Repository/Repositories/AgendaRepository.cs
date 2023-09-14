using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using TLIS_DAL;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModels.AgendaDTOs;
using TLIS_Repository.Base;
using TLIS_Repository.IRepository;

namespace TLIS_Repository.Repositories
{
    public class AgendaRepository : RepositoryBase<TLIagenda, AgendaViewModel, int> , IAgendaRepository
    {
        private readonly ApplicationDbContext _context;
        IMapper _mapper;
        public AgendaRepository(ApplicationDbContext context, IMapper mapper) : base(context, mapper)
        {
            _context = context;
            _mapper = mapper;
        }
    }
    public class AgendaGroupRepository : RepositoryBase<TLIagendaGroup, AgendaGroupViewModel, int>, IAgendaGroupRepository
    {
        private readonly ApplicationDbContext _context;
        IMapper _mapper;
        public AgendaGroupRepository(ApplicationDbContext context, IMapper mapper) : base(context, mapper)
        {
            _context = context;
            _mapper = mapper;
        }
    }
}
