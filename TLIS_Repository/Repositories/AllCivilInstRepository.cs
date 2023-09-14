using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using TLIS_DAL;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModels.AllCivilInstDTOs;
using TLIS_Repository.Base;
using TLIS_Repository.IRepository;

namespace TLIS_Repository.Repositories
{
    public class AllCivilInstRepository: RepositoryBase<TLIallCivilInst, AllCivilInstViewModel, int>, IAllCivilInstRepository
    {
        private readonly ApplicationDbContext _dbContext;
        IMapper _mapper;
        public AllCivilInstRepository(ApplicationDbContext dbContext, IMapper mapper):base(dbContext, mapper)
        {
            _mapper = mapper;
            _dbContext = dbContext;
        }
    }
}
