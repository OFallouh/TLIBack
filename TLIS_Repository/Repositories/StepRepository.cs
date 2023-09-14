using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using TLIS_DAL;
using TLIS_DAL.Models;
using TLIS_Repository.Base;
using TLIS_Repository.IRepository;
using TLIS_DAL.ViewModels.StepDTOs;

namespace TLIS_Repository.Repositories
{
    public class StepListRepository : RepositoryBase<TLIstep, StepListViewModel, int>, IStepListRepository
    {
        private readonly ApplicationDbContext _context;
        IMapper _mapper;
        public StepListRepository(ApplicationDbContext context, IMapper mapper) : base(context, mapper)
        {
            _context = context;
            _mapper = mapper;
        }
    }
    public class StepAddRepository : RepositoryBase<TLIstep, StepAddViewModel, int>, IStepAddRepository
    {
        private readonly ApplicationDbContext _context;
        IMapper _mapper;
        public StepAddRepository(ApplicationDbContext context, IMapper mapper) : base(context, mapper)
        {
            _context = context;
            _mapper = mapper;
        }
    }
    public class StepEditRepository : RepositoryBase<TLIstep, StepEditViewModel, int>, IStepEditRepository
    {
        private readonly ApplicationDbContext _context;
        IMapper _mapper;
        public StepEditRepository(ApplicationDbContext context, IMapper mapper) : base(context, mapper)
        {
            _context = context;
            _mapper = mapper;
        }
    }
}
