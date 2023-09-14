using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using TLIS_DAL;
using TLIS_DAL.Models;
using TLIS_Repository.Base;
using TLIS_Repository.IRepository;
using TLIS_DAL.ViewModels.OrderStatusDTOs;


namespace TLIS_Repository.Repositories
{
    public class OrderStatusListRepository : RepositoryBase<TLIorderStatus, OrderStatusViewModel, int>, IOrderStatusListRepository
    {
        private readonly ApplicationDbContext _context;
        IMapper _mapper;
        public OrderStatusListRepository(ApplicationDbContext context, IMapper mapper) : base(context, mapper)
        {
            _context = context;
            _mapper = mapper;
        }
    }
}
