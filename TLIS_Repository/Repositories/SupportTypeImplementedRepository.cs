using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLIS_DAL;
using TLIS_DAL.Helper;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModels.SupportTypeImplementedDTOs;
using TLIS_Repository.Base;
using TLIS_Repository.IRepository;

namespace TLIS_Repository.Repositories
{
    public class SupportTypeImplementedRepository : RepositoryBase<TLIsupportTypeImplemented, SupportTypeImplementedViewModel, int>, ISupportTypeImplementedRepository
    {

        private ApplicationDbContext _context = null;
        IMapper _mapper;

        public SupportTypeImplementedRepository(ApplicationDbContext context, IMapper mapper) : base(context, mapper)
        {
            _context = context;
            _mapper = mapper;
        }
    }
}
