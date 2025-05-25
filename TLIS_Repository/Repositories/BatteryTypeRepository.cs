using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModels.ActionDTOs;
using TLIS_DAL;
using TLIS_Repository.Base;
using TLIS_Repository.IRepository;
using TLIS_DAL.ViewModels.ActorDTOs;

namespace TLIS_Repository.Repositories
{
    public class BatteryTypeRepository : RepositoryBase<TLIBatteryType, ActorViewModel, int>, IBatteryTypeRepository
    {
       
            private readonly ApplicationDbContext _context;
            IMapper _mapper;
            public BatteryTypeRepository(ApplicationDbContext context, IMapper mapper) : base(context, mapper)
            {
                _context = context;
                _mapper = mapper;
            }
        
    }
}
