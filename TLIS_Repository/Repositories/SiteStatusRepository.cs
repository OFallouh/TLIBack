using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using TLIS_DAL;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModels.SiteStatusDTOs;
using TLIS_Repository.Base;
using TLIS_Repository.IRepository;

namespace TLIS_Repository.Repositories
{
    public class SiteStatusRepository:RepositoryBase<TLIsiteStatus, SiteStatusViewModel, string>,ISiteStatusRepository
    {
        private ApplicationDbContext _context = null;
        IMapper _mapper;
        public SiteStatusRepository(ApplicationDbContext context, IMapper mapper) : base(context, mapper)
        {
            _context = context;
            _mapper = mapper;
        }
    }
}
