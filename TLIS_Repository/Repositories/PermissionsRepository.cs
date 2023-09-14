using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModels.NewPermissionsDTOs.GroupPermissions;
using TLIS_DAL;
using TLIS_Repository.Base;
using TLIS_Repository.IRepository;
using TLIS_DAL.ViewModels.NewPermissionsDTOs.Permissions;

namespace TLIS_Repository.Repositories
{
    public class PermissionsRepository : RepositoryBase<TLIpermissions, NewPermissionsViewModel, int>, IPermissionsRepository
    {
        private ApplicationDbContext _context = null;
        IMapper _mapper;

        public PermissionsRepository(ApplicationDbContext context, IMapper mapper) : base(context, mapper)
        {
            _context = context;
            _mapper = mapper;
        }
    }
}