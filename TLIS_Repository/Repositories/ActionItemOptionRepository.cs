using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using TLIS_DAL;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModels.ActionItemOptionDTOs;
using TLIS_Repository.Base;
using TLIS_Repository.IRepository;

namespace TLIS_Repository.Repositories
{
    public class ActionItemOptionListRepository : RepositoryBase<TLIactionItemOption, ActionItemOptionListViewModel, int>, IActionItemOptionListRepository
    {
        private readonly ApplicationDbContext _context;
        IMapper _mapper;
        public ActionItemOptionListRepository(ApplicationDbContext context, IMapper mapper) : base(context, mapper)
        {
            _context = context;
            _mapper = mapper;
        }
    }
    /*
    public class ActionItemOptionAddRepository : RepositoryBase<TLIactionItemOption, ActionItemOptionAddViewModel, string>, IActionItemOptionAddRepository
    {
        private readonly ApplicationDbContext _context;
        IMapper _mapper;
        public ActionItemOptionAddRepository(ApplicationDbContext context, IMapper mapper) : base(context, mapper)
        {
            _context = context;
            _mapper = mapper;
        }
    }
    public class ActionItemOptionEditRepository : RepositoryBase<TLIactionItemOption, ActionItemOptionEditViewModel, string>, IActionItemOptionEditRepository
    {
        private readonly ApplicationDbContext _context;
        IMapper _mapper;
        public ActionItemOptionEditRepository(ApplicationDbContext context, IMapper mapper) : base(context, mapper)
        {
            _context = context;
            _mapper = mapper;
        }
    }
    //*/
}
