using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using TLIS_DAL;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModels.InstallationTypeDTOs;
using TLIS_Repository.Base;
using TLIS_Repository.IRepository;

namespace TLIS_Repository.Repositories
{
    public class InstallationTypeRepository: RepositoryBase<TLIinstallationType, InstallationTypeViewModel, int>, IInstallationTypeRepository
    {
        private readonly ApplicationDbContext _dbContext;
        IMapper _mapper;
        public InstallationTypeRepository(ApplicationDbContext dbContext, IMapper mapper):base(dbContext, mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }
    }
}
