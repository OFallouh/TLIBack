using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using TLIS_DAL;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModels.StepActionDTOs;
using TLIS_Repository.Base;
using TLIS_Repository.IRepository;

namespace TLIS_Repository.Repositories
{
    
    public class StepActionRepository : RepositoryBase<TLIstepAction, StepActionViewModel, int>, IStepActionRepository
    {
        private readonly ApplicationDbContext _context;
        IMapper _mapper;
        public StepActionRepository(ApplicationDbContext context, IMapper mapper) : base(context, mapper)
        {
            _context = context;
            _mapper = mapper;
        }
    }
}
