using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;
using TLIS_DAL;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModels.RoleDTOs;
using TLIS_Repository.Base;
using TLIS_Repository.IRepository;

namespace TLIS_Repository.Repositories
{
    public class RoleRepository : RepositoryBase<TLIrole, RoleViewModel, int>, IRoleRepository
    {
        private readonly ApplicationDbContext _context;
        IMapper _mapper;
        public RoleRepository(ApplicationDbContext context, IMapper mapper) : base(context, mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public TLIrole AddRole(string RoleName)
        {
            TLIrole entity = new TLIrole();
            entity.Name = RoleName;
            entity.Active = true;
            entity.Deleted = false;
            _context.TLIrole.Add(entity);
            _context.SaveChanges();
            return entity;
        }

        public bool CheckRoleNameInDatabaseAdd(string RoleName)
        {
            return _context.TLIrole.Any(r => r.Name.Equals(RoleName));
        }

        public bool CheckRoleNameInDatabaseUpdate(string RoleName, int RoleId)
        {
            return _context.TLIrole.Any(r => r.Name.Equals(RoleName) && r.Id != RoleId);
        }

        public void DeleteRole(int RoleId)
        {
            using (TransactionScope transaction = new TransactionScope())
            {
                try
                {
                    var RoleGroups = _context.TLIgroupRole.Where(g => g.roleId.Equals(RoleId) && g.Deleted != true).ToList();
                    var GroupsUsers = _context.TLIgroupUser.Where(g => (RoleGroups.Select(x => x.groupId).Contains(g.groupId))).Select(g => g.userId).ToList();
                    foreach (var item in RoleGroups)
                    {
                        item.Deleted = true;
                    }
                    // _context.TLIgroupRole.RemoveRange(RoleGroups);
                    _context.SaveChanges();
                    var RolePermissions = _context.TLIrolePermission.Where(r => r.roleId.Equals(RoleId) && r.Deleted != true).ToList();
                    foreach (var item in RolePermissions)
                    {
                        item.Deleted = true;
                    }
                    // _context.TLIrolePermission.RemoveRange(RolePermissions);
                    _context.SaveChanges();
                    var Role = _context.TLIrole.FirstOrDefault(r => r.Id.Equals(RoleId));
                    Role.Deleted = true;
                    Role.Name = Role.Name + DateTime.Now.ToString();
                    // _context.TLIrole.Remove(Role);
                    _context.SaveChanges();
                    foreach (var userId in GroupsUsers)
                    {
                        var userPermissions = _context.TLIuserPermission.Where(u => u.userId == userId && u.Deleted != false).Select(u => u.permissionId).ToList();
                        var UserGroups = _context.TLIgroupUser.Where(g => g.userId == userId && g.Deleted != true).Select(g => g.groupId).ToList();
                        var GroupRoles = _context.TLIgroupRole.Where(g => (UserGroups.Contains(g.groupId)) && g.Deleted != true).Select(g => g.roleId).Distinct().ToList();
                        var RolesPermissions = _context.TLIrolePermission.Where(r => (GroupRoles.Contains(r.roleId)) && r.Deleted != true).Select(r => r.permissionId).Distinct().ToList();
                        var PermissionToDelete = userPermissions.Except(RolesPermissions);
                        foreach (var permission in PermissionToDelete)
                        {
                            var UserPermission = _context.TLIuserPermission.FirstOrDefault(x => x.permissionId == permission && x.userId == userId);
                            UserPermission.Deleted = true;
                            // _context.TLIuserPermission.Remove(UserPermission);
                            _context.SaveChanges();
                        }
                    }
                    transaction.Complete();
                }
                catch (Exception)
                {

                }
            }

        }

        public IEnumerable<TLIrole> GetRoles(string filter = null)
        {
            if (filter == null)
            {
                var RoleList = _context.TLIrole.ToList();
                return RoleList;
            }
            else
            {
                var RoleList = _context.TLIrole.Where(r => r.Name.Contains(filter)).ToList();
                return RoleList;
            }
        }

        private void DeleteUserPermission(int UserId, int PermissionId)
        {
            var UserPermission = _context.TLIuserPermission.FirstOrDefault(r => r.userId.Equals(UserId) && r.permissionId.Equals(PermissionId));
            _context.TLIuserPermission.Remove(UserPermission);
            _context.SaveChanges();
        }
    }
}
