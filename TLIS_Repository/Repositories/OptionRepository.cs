using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using TLIS_DAL;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModels.OptionDTOs;
using TLIS_Repository.Base;
using TLIS_Repository.IRepository;

namespace TLIS_Repository.Repositories
{
   public class OptionRepository:RepositoryBase<TLIactionOption, OptionViewModel, int>, IOptionRepository
    {
        private readonly ApplicationDbContext _context;
        IMapper _mapper;
        public OptionRepository(ApplicationDbContext context, IMapper mapper) : base(context, mapper)
        {
            _context = context;
            _mapper = mapper;
        }
    }
}
