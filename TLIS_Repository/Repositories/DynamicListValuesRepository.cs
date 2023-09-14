using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using TLIS_DAL;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModels.DynamicListValuesDTOs;
using TLIS_Repository.Base;
using TLIS_Repository.IRepository;

namespace TLIS_Repository.Repositories
{
    public class DynamicListValuesRepository:RepositoryBase<TLIdynamicListValues, DynamicListValuesViewModel, int>, IDynamicListValuesRepository
    {
        private readonly ApplicationDbContext _dbContext;
        IMapper _mapper;
        public DynamicListValuesRepository(ApplicationDbContext dbContext, IMapper mapper):base(dbContext, mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }
    }
}
