using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using TLIS_DAL;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModels.ItemStatusDTOs;
using TLIS_Repository.Base;
using TLIS_Repository.IRepository;

namespace TLIS_Repository.Repositories
{
    public class StepActionIncomeItemStatusRepository : RepositoryBase<TLIstepActionIncomeItemStatus, StepActionIncomeItemStatusViewModel, int>, IStepActionIncomeItemStatusRepository
    {
        private readonly ApplicationDbContext _context;
        IMapper _mapper;
        public StepActionIncomeItemStatusRepository(ApplicationDbContext context, IMapper mapper) : base(context, mapper)
        {
            _context = context;
            _mapper = mapper;
        }
    }
}