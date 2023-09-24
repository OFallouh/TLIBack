using TLIS_DAL.Models;
using TLIS_DAL;
using TLIS_Repository.Base;
using TLIS_Repository.IRepository;
using TLIS_DAL.ViewModels.UserPermissionDTOs;
using AutoMapper;
using TLIS_DAL.ViewModels.UserPermissionssDTOs;

namespace TLIS_Repository.Repositories
{
    public class UserPermissionssRepository : RepositoryBase<TLIuser_Permissions, UserPermissionsViewModel, int>, IUserPermissionssRepository
    {
        private readonly ApplicationDbContext _context;
        IMapper _mapper;
        public UserPermissionssRepository(ApplicationDbContext context, IMapper mapper) : base(context, mapper)
        {
            _context = context;
            _mapper = mapper;
        }
    }
}
