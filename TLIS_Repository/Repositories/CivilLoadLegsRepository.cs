using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using TLIS_DAL;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModels.CivilLoadLegsDTOs;
using TLIS_Repository.Base;
using TLIS_Repository.IRepository;

namespace TLIS_Repository.Repositories
{
    public class CivilLoadLegsRepository: RepositoryBase<TLIcivilLoadLegs, CivilLoadLegsViewModel, int>, ICivilLoadLegsRepository
    {
        private readonly ApplicationDbContext _dbContext;
        IMapper _mapper;
        public CivilLoadLegsRepository(ApplicationDbContext dbContext, IMapper mapper):base(dbContext, mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }
    }
}
