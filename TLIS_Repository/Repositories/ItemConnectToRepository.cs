using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using TLIS_DAL;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModels.ItemConnectToDTOs;
using TLIS_Repository.Base;
using TLIS_Repository.IRepository;

namespace TLIS_Repository.Repositories
{
    public class ItemConnectToRepository: RepositoryBase <TLIitemConnectTo,ItemConnectToViewModel,int>, IItemConnectToRepository
    {
        private readonly ApplicationDbContext _context;
        IMapper _mapper;
        public ItemConnectToRepository(ApplicationDbContext context, IMapper mapper):base(context, mapper)
        {
            _context = context;
            _mapper = mapper;
        }
    }
}
