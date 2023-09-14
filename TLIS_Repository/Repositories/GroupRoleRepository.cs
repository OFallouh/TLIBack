using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using TLIS_DAL;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModels.GroupRoleDTOs;
using TLIS_DAL.ViewModels.RoleDTOs;
using TLIS_Repository.Base;
using TLIS_Repository.IRepository;

namespace TLIS_Repository.Repositories
{
    public class GroupRoleRepository:RepositoryBase<TLIgroupRole, GroupRoleViewModel, int>, IGroupRoleRepository
    {
        private readonly ApplicationDbContext _context;
        IMapper _mapper;
        public GroupRoleRepository(ApplicationDbContext context, IMapper mapper):base(context,mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public void AddGroupRoles(int GroupId, List<RoleViewModel> RolesId)
        {
            using (TransactionScope transaction = new TransactionScope())
            {

            }
            List<int> groupUsersIDList = _context.TLIgroupUser.AsNoTracking().Where(g => g.groupId.Equals(GroupId)).Select(g => g.userId).ToList();
            foreach (RoleViewModel RoleId in RolesId)
            {
                TLIgroupRole groupRole = new TLIgroupRole();
                groupRole.groupId = GroupId;
                groupRole.roleId = RoleId.Id;
                _context.TLIgroupRole.Add(groupRole);
                _context.SaveChanges();
                List<int> RolePermissions = _context.TLIrolePermission.AsNoTracking().Where(r => r.roleId.Equals(RoleId)).Select(r => r.permissionId).ToList();
               
                foreach (int userID in groupUsersIDList)
                {
                    List<int> userPermissions = _context.TLIuserPermission.AsNoTracking().Where(u => u.userId.Equals(userID)).Select(u => u.permissionId).ToList();
                    IEnumerable<int> PermissionsToAdd = RolePermissions.Except(userPermissions);
                    foreach (int permission in PermissionsToAdd)
                    {
                        TLIuserPermission userPermission = new TLIuserPermission();
                        userPermission.permissionId = permission;
                        userPermission.userId = userID;
                        _context.TLIuserPermission.Add(userPermission);
                    }
                    _context.SaveChanges();
                }
            }
        }
        public void EditGroupRoles(int GroupId, List<RoleViewModel> RolesId)
        {
            var GroupRoles = _context.TLIgroupRole.Where(g => g.groupId == GroupId && g.Deleted == true);
            foreach(var GroupRole in GroupRoles)
            {
                var test = RolesId.Any(r => r.Equals(GroupId));
                if(test == false)
                {
                    var groupRole = _context.TLIgroupRole.FirstOrDefault(r => r.Id.Equals(GroupRole));
                    groupRole.Deleted = true;
                    _context.TLIgroupRole.Update(groupRole);
                    _context.SaveChanges();
                }
            }
            foreach(var RoleId in RolesId)
            {
                var test = GroupRoles.Any(g => g.Id == RoleId.Id);
                if(test == false)
                {
                    TLIgroupRole grRole = new TLIgroupRole();
                    grRole.groupId = GroupId;
                    grRole.roleId = RoleId.Id;
                    _context.TLIgroupRole.Add(grRole);
                    _context.SaveChanges();
                }
            }
        }

        public async Task<IEnumerable<TLIgroupRole>> GetGroupsByRoleId(int RoleId)
        {
            return await _context.TLIgroupRole.AsNoTracking().Where(g => g.roleId.Equals(RoleId)).ToListAsync();
        }

        public async Task<IEnumerable<TLIgroupRole>> GetRolesByGroupId(int GroupId)
        {
            return await _context.TLIgroupRole.Where(g => g.groupId.Equals(GroupId)).ToListAsync();
        }


        public List<int> GetGroupRolesId(int GroupId)
        {
            return _context.TLIgroupRole.Where(x=>x.groupId==GroupId && x.Active==true).Select(y=>y.roleId).ToList();
        }
    }
}
