using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using TLIS_DAL;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModels.StepActionDTOs;
using TLIS_DAL.ViewModels.WorkFlowDTOs;
using TLIS_Repository.Base;
using TLIS_Repository.IRepository;

namespace TLIS_Repository.Repositories
{
    public class WorkFlowRepository : RepositoryBase<TLIworkFlow, WorkFlowViewModel, int>, IWorkFlowRepository
    {
        private readonly ApplicationDbContext _context;
        IMapper _mapper;
        public WorkFlowRepository(ApplicationDbContext context, IMapper mapper) : base(context, mapper)
        {
            _context = context;
            _mapper = mapper;
        }
    }
    /*
    public class ListWorkFlowRepository : RepositoryBase<TLIworkFlow, ListWorkFlowViewModel, string>, IListWorkFlowRepository
    {
        private readonly ApplicationDbContext _context;
        IMapper _mapper;
        public ListWorkFlowRepository(ApplicationDbContext context, IMapper mapper) : base(context, mapper)
        {
            _context = context;
            _mapper = mapper;
        }
    }
    public class AddWorkFlowRepository : RepositoryBase<TLIworkFlow, AddWorkFlowViewModel, string>, IAddWorkFlowRepository
    {
        private readonly ApplicationDbContext _context;
        IMapper _mapper;
        public AddWorkFlowRepository(ApplicationDbContext context, IMapper mapper) : base(context, mapper)
        {
            _context = context;
            _mapper = mapper;
        }
    }
    //*/
    public class StepActionMailFromRepository : RepositoryBase<TLIstepActionMail, PermissionWorkFlowViewModel, int>, IStepActionMailFromRepository
    {
        private readonly ApplicationDbContext _context;
        IMapper _mapper;
        public StepActionMailFromRepository(ApplicationDbContext context, IMapper mapper) : base(context, mapper)
        {
            _context = context;
            _mapper = mapper;
        }
    }
    public class StepActionItemOptionRepository : RepositoryBase<TLIstepActionItemOption, ListStepActionItemOptionViewModel, int>, IStepActionItemOptionRepository
    {
        private readonly ApplicationDbContext _context;
        IMapper _mapper;
        public StepActionItemOptionRepository(ApplicationDbContext context, IMapper mapper) : base(context, mapper)
        {
            _context = context;
            _mapper = mapper;
        }
    }
    /*
    public class EditWorkFlowRepository : RepositoryBase<TLIworkFlow, EditWorkFlowViewModel, string>, IEditWorkFlowRepository
    {
        private readonly ApplicationDbContext _context;
        IMapper _mapper;
        public EditWorkFlowRepository(ApplicationDbContext context, IMapper mapper) : base(context, mapper)
        {
            _context = context;
            _mapper = mapper;
        }
    }
    public class WorkFlowDeleteRepository : RepositoryBase<TLIworkFlow, DeleteWorkFlowViewModel, string>, IWorkFlowDeleteRepository
    {
        private readonly ApplicationDbContext _context;
        IMapper _mapper;
        public WorkFlowDeleteRepository(ApplicationDbContext context, IMapper mapper) : base(context, mapper)
        {
            _context = context;
            _mapper = mapper;
        }
    }
    public class PermissionWorkFlowRepository : RepositoryBase<TLIworkFlow, PermissionWorkFlowViewModel, string>, IPermissionWorkFlowRepository
    {
        private readonly ApplicationDbContext _context;
        IMapper _mapper;
        public PermissionWorkFlowRepository(ApplicationDbContext context, IMapper mapper) : base(context, mapper)
        {
            _context = context;
            _mapper = mapper;
        }
    }
    //*/


}
