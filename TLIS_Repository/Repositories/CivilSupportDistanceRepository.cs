using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using TLIS_DAL;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModels.CivilSupportDistanceDTOs;
using TLIS_Repository.Base;
using TLIS_Repository.IRepository;

namespace TLIS_Repository.Repositories
{
    public class CivilSupportDistanceRepository: RepositoryBase<TLIcivilSupportDistance, CivilSupportDistanceViewModel, int>, ICivilSupportDistanceRepository
    {
        private readonly ApplicationDbContext _dbContext;
        IMapper _mapper;
        public CivilSupportDistanceRepository(ApplicationDbContext dbContext, IMapper mapper):base(dbContext, mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }
    }
}
