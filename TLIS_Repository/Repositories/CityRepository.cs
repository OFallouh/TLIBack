using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TLIS_DAL;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModels;
using TLIS_DAL.ViewModels.CityDTOs;
using TLIS_Repository.Base;
using TLIS_Repository.IRepository;

namespace TLIS_Repository.Repositories
{
    public class CityRepository: RepositoryBase<TLIcity, CityViewModel, int>, ICityRepository
    {
        private ApplicationDbContext _context = null;
        IMapper _mapper;
        public CityRepository(ApplicationDbContext context, IMapper mapper) : base(context, mapper)
        {
            _context = context;
            _mapper = mapper;
        }

       
       
    }


}

