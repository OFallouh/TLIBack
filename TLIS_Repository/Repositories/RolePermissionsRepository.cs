using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModels.NewPermissionsDTOs.Permissions;
using TLIS_DAL;
using TLIS_Repository.Base;
using TLIS_Repository.IRepository;
using TLIS_DAL.ViewModels.NewPermissionsDTOs.RolePermissions;

namespace TLIS_Repository.Repositories
{
    public class RolePermissionsRepository : RepositoryBase<TLIrole_Permissions, NewRolePermissionsViewModel, int>, IRolePermissionsRepository
    {
        private ApplicationDbContext _context = null;
        IMapper _mapper;

        public RolePermissionsRepository(ApplicationDbContext context, IMapper mapper) : base(context, mapper)
        {
            _context = context;
            _mapper = mapper;
        }
    }
}