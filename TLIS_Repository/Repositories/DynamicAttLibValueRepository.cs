using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModels.AllCivilInstDTOs;
using TLIS_DAL;
using TLIS_Repository.Base;
using TLIS_Repository.IRepository;
using TLIS_DAL.ViewModels.DynamicAttDTOs;
using TLIS_DAL.ViewModels.AllOtherInventoryInstDTOs;

namespace TLIS_Repository.Repositories
{
    public class DynamicAttLibValueRepository : RepositoryBase<TLIdynamicAttLibValue, DynamicAttLibViewModel, int>, IDynamicAttLibValueRepository
    {
        private readonly ApplicationDbContext _dbContext;
        IMapper _mapper;
        public DynamicAttLibValueRepository(ApplicationDbContext dbContext, IMapper mapper) : base(dbContext, mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }
    }
}
