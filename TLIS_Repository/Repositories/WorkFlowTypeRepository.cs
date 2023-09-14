using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using TLIS_DAL;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModels.WorkFlowTypeDTOs;
using TLIS_Repository.Base;
using TLIS_Repository.IRepository;

namespace TLIS_Repository.Repositories
{
    public class WorkFlowTypeRepository : RepositoryBase<TLIworkFlowType, WorkFlowTypeViewModel, int>, IWorkFlowTypeRepository
    {
        private readonly ApplicationDbContext _context;
        IMapper _mapper;
        public WorkFlowTypeRepository(ApplicationDbContext context, IMapper mapper) : base(context, mapper)
        {
            _context = context;
            _mapper = mapper;
        }
    }
    /*
    public class ListWorkFlowTypeRepository : RepositoryBase<TLIworkFlowType, ListWorkFlowTypeViewModel, string>, IListWorkFlowTypeRepository
    {
        private readonly ApplicationDbContext _context;
        IMapper _mapper;
        public ListWorkFlowTypeRepository(ApplicationDbContext context, IMapper mapper) : base(context, mapper)
        {
            _context = context;
            _mapper = mapper;
        }
    }
    public class AddWorkFlowTypeRepository : RepositoryBase<TLIworkFlowType, AddWorkFlowTypeViewModel, string>, IAddWorkFlowTypeRepository
    {
        private readonly ApplicationDbContext _context;
        IMapper _mapper;
        public AddWorkFlowTypeRepository(ApplicationDbContext context, IMapper mapper) : base(context, mapper)
        {
            _context = context;
            _mapper = mapper;
        }
    }
    public class EditWorkFlowTypeRepository : RepositoryBase<TLIworkFlowType, EditWorkFlowTypeViewModel, string>, IEditWorkFlowTypeRepository
    {
        private readonly ApplicationDbContext _context;
        IMapper _mapper;
        public EditWorkFlowTypeRepository(ApplicationDbContext context, IMapper mapper) : base(context, mapper)
        {
            _context = context;
            _mapper = mapper;
        }
    }
    public class DeleteWorkFlowTypeRepository : RepositoryBase<TLIworkFlowType, DeleteWorkFlowTypeViewModel, string>, IDeleteWorkFlowTypeRepository
    {
        private readonly ApplicationDbContext _context;
        IMapper _mapper;
        public DeleteWorkFlowTypeRepository(ApplicationDbContext context, IMapper mapper) : base(context, mapper)
        {
            _context = context;
            _mapper = mapper;
        }
    }
    //*/
}
