using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using TLIS_DAL;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModels.MW_PortDTOs;
using TLIS_Repository.Base;
using TLIS_Repository.IRepository;

namespace TLIS_Repository.Repositories
{
    class MW_PortRepository:RepositoryBase<TLImwPort,MW_PortViewModel,int>, IMW_PortRepository
    {
        private readonly ApplicationDbContext _context;
        IMapper _mapper;
        public MW_PortRepository(ApplicationDbContext context, IMapper mapper) : base(context, mapper)
        {
            _context = context;
            _mapper = mapper;
        }
    }
}
