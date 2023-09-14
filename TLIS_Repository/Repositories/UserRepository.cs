using AutoMapper;
using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TLIS_DAL;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModels.UserDTOs;
using TLIS_Repository.Base;
using TLIS_Repository.IRepository;
using static System.Net.WebRequestMethods;

namespace TLIS_Repository.Repositories
{
    public class UserRepository:RepositoryBase<TLIuser, UserViewModel, int>, IUserRepository
    {
        private readonly ApplicationDbContext _context;
        IMapper _mapper;
        public UserRepository(ApplicationDbContext context, IMapper mapper):base(context,mapper)
        {
            _context = context;
            _mapper = mapper;
        }
    }
}
