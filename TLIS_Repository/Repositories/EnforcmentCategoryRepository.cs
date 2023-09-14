using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using TLIS_DAL;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModels.EnforcmentCategoryDTOs;
using TLIS_Repository.Base;
using TLIS_Repository.IRepository;

namespace TLIS_Repository.Repositories
{
    public class EnforcmentCategoryRepository:RepositoryBase<TLIenforcmentCategory, EnforcmentCategoryViewModel, int> ,IEnforcmentCategoryRepository
    {
        private readonly ApplicationDbContext _context;
        IMapper _mapper;
        public EnforcmentCategoryRepository(ApplicationDbContext context, IMapper mapper) : base(context, mapper)
        {
            _context = context;
            _mapper = mapper;
        }
    }
}
