using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using TLIS_DAL;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModels.DataTypeDTOs;
using TLIS_Repository.Base;
using TLIS_Repository.IRepository;

namespace TLIS_Repository.Repositories
{
    public class DataTypeRepository: RepositoryBase<TLIdataType, DataTypeViewModel, int>, IDataTypeRepository
    {
        private readonly ApplicationDbContext _context;
        IMapper _mapper;
        public DataTypeRepository(ApplicationDbContext context, IMapper mapper):base(context, mapper)
        {
            _context = context;
            _mapper = mapper;
        }
    }
}
