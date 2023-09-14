using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLIS_DAL;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModels.GroupUserDTOs;
using TLIS_DAL.ViewModels.PermissionDTOs;
using TLIS_Repository.Base;
using TLIS_Repository.IRepository;

namespace TLIS_Repository.Repositories
{
    public class GroupUserRepository:RepositoryBase<TLIgroupUser, GroupUserViewModel,int>, IGroupUserRepository
    {
        private readonly ApplicationDbContext _context;
        IMapper _mapper;
        public GroupUserRepository(ApplicationDbContext context, IMapper mapper):base(context,mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public void EditGroupUsers(int groupId, List<int> UsersId)
        {
            var groupUsers = _context.TLIgroupUser.Where(g => g.groupId.Equals(groupId)).Select(g => g.userId).ToList();
            var usersToAdd = UsersId.Except(groupUsers);
            var UsersToDelete = groupUsers.Except(UsersId);
            var rolePermissions = GetRolePermissions(groupId);
            foreach(var userId in usersToAdd)
            {
                var userPermissions = _context.TLIuserPermission.Where(u => u.userId.Equals(userId)).Select(u => u.permissionId).ToList();
                var DiffToAdd = rolePermissions.Except(userPermissions);
                foreach(var permission in DiffToAdd)
                {
                    ///try to map then use add in repository
                    TLIuserPermission userPermission = new TLIuserPermission();
                    userPermission.permissionId = permission;
                    userPermission.userId = userId;
                    _context.TLIuserPermission.Add(userPermission);
                    _context.SaveChanges();
                }
            }
            foreach(var userId in UsersToDelete)
            {
                var RolePermissions = GetRolePermissionsWithoutDeletedGroup(userId, groupId);
                var userPermissions = _context.TLIuserPermission.Where(u => u.userId.Equals(userId)).Select(u => u.permissionId).ToList();
                var DiffToDelete = userPermissions.Except(RolePermissions);
                foreach(var permission in DiffToDelete)
                {
                    var userPermission = _context.TLIuserPermission.FirstOrDefault(u => u.userId.Equals(userId));
                    _context.TLIuserPermission.Remove(userPermission);
                    _context.SaveChanges();
                }
            }
        }

        public async Task<IEnumerable<TLIgroupUser>> GetGroupsByUserId(int UserId)
        {
            return await _context.TLIgroupUser.AsNoTracking().Where(g => g.userId.Equals(UserId)).ToListAsync();
        }

        public async Task<IEnumerable<TLIgroupUser>> GetUsersByGroupId(int GroupId)
        {
            return await _context.TLIgroupUser.AsNoTracking().Where(g => g.groupId.Equals(GroupId)).ToListAsync();
        }

        public List<int> GetRolePermissions(int groupId)
        {
            var groupRoles = _context.TLIgroupRole.Where(g => g.groupId.Equals(groupId)).Select(g => g.roleId).ToList();
            var rolePermissions = _context.TLIrolePermission.Where(r => (groupRoles.Contains(r.roleId))).Select(r => r.permissionId).ToList();
            return rolePermissions;
        }

        public List<int> GetRolePermissionsWithoutDeletedGroup(int userId,int groupId)
        {
            var userGroups = _context.TLIgroupUser.Where(g => g.userId.Equals(userId)).Select(g => g.groupId).ToList();
            var userGroupsWihtoutDeletedGroup = userGroups.Remove(groupId);
            var groupRoles = _context.TLIgroupRole.Where(g => g.groupId.Equals(groupId)).Select(g => g.roleId).ToList();
            var rolePermissions = _context.TLIrolePermission.Where(r => (groupRoles.Contains(r.roleId))).Select(r => r.permissionId).ToList();
            return rolePermissions;
        }
    }
}
