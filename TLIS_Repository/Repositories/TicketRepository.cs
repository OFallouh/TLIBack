using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using TLIS_DAL;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModels.TicketActionDTOs;
using TLIS_DAL.ViewModels.TicketDTOs;
using TLIS_DAL.ViewModels.TicketOptionNoteDTOs;
using TLIS_DAL.ViewModels.TicketTargetDTOs;
using TLIS_Repository.Base;
using TLIS_Repository.IRepository;

namespace TLIS_Repository.Repositories
{
    public class TicketRepository : RepositoryBase<TLIticket, ListTicketViewModel, int>, ITicketRepository
    {
        private readonly ApplicationDbContext _context;
        IMapper _mapper;
        public TicketRepository(ApplicationDbContext context, IMapper mapper) : base(context, mapper)
        {
            _context = context;
            _mapper = mapper;
        }
    }
    public class TicketActionRepository : RepositoryBase<TLIticketAction, TicketActionViewModel, int>, ITicketActionRepository
    {
        private readonly ApplicationDbContext _context;
        IMapper _mapper;
        public TicketActionRepository(ApplicationDbContext context, IMapper mapper) : base(context, mapper)
        {
            _context = context;
            _mapper = mapper;
        }
    }
    public class TicketTargetRepository : RepositoryBase<TLIticketTarget, TicketTargetViewModel, int>, ITicketTargetRepository
    {
        private readonly ApplicationDbContext _context;
        IMapper _mapper;
        public TicketTargetRepository(ApplicationDbContext context, IMapper mapper) : base(context, mapper)
        {
            _context = context;
            _mapper = mapper;
        }
    }
    public class TicketOptionNoteRepository : RepositoryBase<TLIticketOptionNote, TicketOptionNoteViewModel, int>, ITicketOptionNoteRepository
    {
        private readonly ApplicationDbContext _context;
        IMapper _mapper;
        public TicketOptionNoteRepository(ApplicationDbContext context, IMapper mapper) : base(context, mapper)
        {
            _context = context;
            _mapper = mapper;
        }
    }
}
