using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using TLIS_DAL;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModels.TaskStatusDTOs;
using TLIS_Repository.Base;
using TLIS_Repository.IRepository;

namespace TLIS_Repository.Repositories
{
    public class TaskStatusRepository1 : RepositoryBase<TLItaskStatus,TaskStatusViewModel,int>,ITaskStatusRepository
    {
        private ApplicationDbContext _context = null;
        IMapper _mapper;
        public TaskStatusRepository1(ApplicationDbContext context, IMapper mapper) : base(context,mapper)
        {
            _context = context;
            _mapper = mapper;
        }
    }
}
