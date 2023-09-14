using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using TLIS_DAL;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModels.GuyLineTypeDTOs;
using TLIS_Repository.Base;
using TLIS_Repository.IRepository;

namespace TLIS_Repository.Repositories
{
    public class GuyLineTypeRepository:RepositoryBase<TLIguyLineType, GuyLineTypeViewModel, int>, IGuyLineTypeRepository
    {
        private readonly ApplicationDbContext _context;
        IMapper _mapper;
        public GuyLineTypeRepository(ApplicationDbContext context, IMapper mapper):base(context,mapper)
        {
            _context = context;
            _mapper = mapper;
        }
    }
}
