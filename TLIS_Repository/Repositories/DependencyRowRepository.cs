using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using TLIS_DAL;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModels.DependencyRowDTOs;
using TLIS_Repository.Base;
using TLIS_Repository.IRepository;

namespace TLIS_Repository.Repositories
{
    public class DependencyRowRepository: RepositoryBase<TLIdependencyRow, DependencyRowViewModel, int>, IDependencyRowRepository
    {
        private readonly ApplicationDbContext _dbContext;
        IMapper _mapper;
        public DependencyRowRepository(ApplicationDbContext dbContext, IMapper mapper):base(dbContext, mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }
    }
}
