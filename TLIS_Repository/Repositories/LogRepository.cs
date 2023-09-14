using AutoMapper;
using Dapper;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TLIS_DAL;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModels.LogDTOs;
using TLIS_Repository.Base;
using TLIS_Repository.IRepository;

namespace TLIS_Repository.Repositories
{
    public class LogRepository : RepositoryBase<TLIlog, LogViewModel, int>, ILogRepository
    {
        private readonly ApplicationDbContext _dbContext;
        IMapper _mapper;
        public LogRepository(ApplicationDbContext dbContext, IMapper mapper) : base(dbContext, mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }
        public void InsertAuditLogs(TLIlog LogViewModel)
        {
            try
            {
                _dbContext.TLIlog.AddAsync(LogViewModel);
                _dbContext.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
