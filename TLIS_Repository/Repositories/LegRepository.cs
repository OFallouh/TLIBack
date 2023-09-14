using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLIS_DAL;
using TLIS_DAL.Helper;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModels.LegDTOs;
using TLIS_Repository.Base;
using TLIS_Repository.IRepository;

namespace TLIS_Repository.Repositories
{
    public class LegRepository : RepositoryBase<TLIleg, LegViewModel, int>, ILegRepository
    {
        private ApplicationDbContext _context = null;
        IMapper _mapper;
        public LegRepository(ApplicationDbContext context, IMapper mapper) : base(context, mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public void AddLegs(List<AddLegViewModel> LegsViewModel, int CivilWithLegId)
        {
            foreach (var leg in LegsViewModel)
            {
                // leg.CivilWithLegInstId = CivilWithLegId;
                TLIleg LegEntity = _mapper.Map<TLIleg>(leg);
                _context.TLIleg.Add(LegEntity);
                _context.SaveChanges();
            }
        }
    }
}
