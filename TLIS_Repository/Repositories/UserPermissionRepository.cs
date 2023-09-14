using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TLIS_DAL;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModels.PermissionDTOs;
using TLIS_DAL.ViewModels.UserPermissionDTOs;
using TLIS_Repository.Base;
using TLIS_Repository.IRepository;

namespace TLIS_Repository.Repositories
{
    public class UserPermissionRepository:RepositoryBase<TLIuserPermission, UserPermissionViewModel, int>, IUserPermissionRepository
    {
        private readonly  ApplicationDbContext _context;
        IMapper _mapper;
        public UserPermissionRepository(ApplicationDbContext context, IMapper mapper):base(context,mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public IEnumerable<PermissionViewModel> GetUserPermissionsByUserId(int UserId)
        {
            var Permissions = _context.TLIuserPermission.Where(u => u.userId.Equals(UserId)).Select(u => new PermissionViewModel(u.permissionId, null, null)).ToList();

            
            return _mapper.Map<IEnumerable<PermissionViewModel>>(Permissions);
        }
    }
}
