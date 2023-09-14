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
using TLIS_DAL.ViewModels.PermissionDTOs;
using TLIS_DAL.ViewModels.RolePermissionDTOs;
using TLIS_Repository.Base;
using TLIS_Repository.IRepository;

namespace TLIS_Repository.Repositories
{
    public class RolePermissionRepository:RepositoryBase<TLIrolePermission, RolePermissionViewModel,int>, IRolePermissionRepository
    {
        private readonly ApplicationDbContext _context;
        IMapper _mapper;
        public RolePermissionRepository(ApplicationDbContext context, IMapper mapper):base(context,mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public void AddRolePermissionList(int RoleId, List<PermissionViewModel> permissions)
        {
            List<int> permissionList = _context.TLIrolePermission.Where(x => x.roleId == RoleId).Select(x => x.permissionId).ToList();
            IEnumerable<int> Permissions = permissions.Select(x => x.Id);
            IEnumerable<int> PermissionToAdd = Permissions.Except(permissionList);
            foreach (var permission in PermissionToAdd)
            {
                TLIrolePermission entity = new TLIrolePermission();
                entity.roleId = RoleId;
                entity.permissionId = permission;
                _context.TLIrolePermission.Add(entity);
            }
            _context.SaveChanges();
        }

        public async Task EditRolePermissionList(int RoleId, List<PermissionViewModel> permissions, List<int> AffectedGroupsIds)
        {
            List<int> rolePermissions = await _context.TLIrolePermission.AsNoTracking().Where(r => r.roleId.Equals(RoleId)).Select(r => r.permissionId).ToListAsync();
            List<int> PermissionsIds = permissions.Select(p => p.Id).ToList();
            IEnumerable<int> PermissionToAdd = PermissionsIds.Except(rolePermissions);
            IEnumerable<int> PermissionToDelete = rolePermissions.Except(PermissionsIds);

            using (TransactionScope AddTransaction = new TransactionScope())
            {
                foreach (int permission in PermissionToAdd)
                {
                    await AddRolePermission(RoleId, permission);
                    await AddUserPermission(RoleId, permission, AffectedGroupsIds);
                }
                AddTransaction.Complete();
            }

            using (TransactionScope DeleteTransaction = new TransactionScope())
            {
                foreach (int permission in PermissionToDelete)
                {
                    await DeleteRolePermission(RoleId, permission);
                    foreach (int GroupId in AffectedGroupsIds)
                    {
                        //var rolePermission = await _context.TLIrolePermission.AsNoTracking().FirstOrDefaultAsync(r => r.roleId.Equals(RoleId) && r.permissionId.Equals(PermissionId));
                        //_context.TLIrolePermission.Remove(rolePermission);
                        //await _context.SaveChangesAsync();
                    }
                }
                DeleteTransaction.Complete();
            }

            // rolePermissions = await _context.TLIrolePermission.AsNoTracking().Where(r => r.roleId.Equals(RoleId)).Select(r => r.permissionId).ToListAsync();
            List<int> RoleGroups = await _context.TLIgroupRole.AsNoTracking().Where(g => g.roleId == RoleId).Select(g => g.groupId).ToListAsync();
            List<int> GroupUsers = await _context.TLIgroupUser.AsNoTracking().Where(g => (RoleGroups.Contains(g.groupId))).Select(g => g.userId).Distinct().ToListAsync();
            using (TransactionScope transaction = new TransactionScope())
            {
                foreach (int userId in GroupUsers)
                {
                    List<int> userPermissions = await _context.TLIuserPermission.AsNoTracking().Where(u => u.userId == userId).Select(u => u.permissionId).ToListAsync();
                    PermissionToAdd = PermissionsIds.Except(userPermissions);
                    foreach (int permission in PermissionToAdd)
                    {
                        await AddUserPermission(userId, permission);
                    }
                }
                transaction.Complete();
            }
        }

        public List<PermissionViewModel> GetAllPermissionsByRoleId(int RoleId)
        {
            //var RolePermissionsList = _context.TLIrolePermission
            //    .Include(r => r.permission)
            //    .Where(r => r.roleId.Equals(RoleId))
            //    .Select(r => new PermissionViewModel(r.permissionId, r.permission.Name, r.permission.Module))
            //    .ToList();
            //return RolePermissionsList;
            return new List<PermissionViewModel>();
        }
        public async Task AddUserPermission(int RoleId, int permission, List<int> AffectedGroupsIds)
        {
            using(TransactionScope transaction = new TransactionScope())
            {
                foreach (int GroupId in AffectedGroupsIds)
                {
                    List<int> UserIds = await _context.TLIgroupUser.AsNoTracking().Where(x => x.groupId == GroupId).Select(x => x.Id).Distinct().ToListAsync();

                    foreach (int UserId in UserIds)
                    {
                        bool CheckPermissionForUser = _context.TLIuserPermission.Any(x => x.permissionId == permission && x.userId == UserId);
                        if (!CheckPermissionForUser)
                        {
                            TLIuserPermission NewUserPermssion = new TLIuserPermission();
                            NewUserPermssion.userId = UserId;
                            NewUserPermssion.permissionId = permission;
                            await _context.TLIuserPermission.AddAsync(NewUserPermssion);
                            await _context.SaveChangesAsync();
                        }
                    }
                }
                transaction.Complete();
            }
        }
        private async Task AddRolePermission(int RoleId, int PermissionId)
        {
            bool Check = _context.TLIrolePermission.Any(x => x.permissionId == PermissionId && x.roleId == RoleId);
            if(Check == false)
            {
                TLIrolePermission rolePermission = new TLIrolePermission();
                rolePermission.permissionId = PermissionId;
                rolePermission.roleId = RoleId;
                await _context.TLIrolePermission.AddAsync(rolePermission);
                await _context.SaveChangesAsync();
            }
        }
        private async Task DeleteRolePermission(int RoleId, int PermissionId)
        {
            var rolePermission = await _context.TLIrolePermission.AsNoTracking().FirstOrDefaultAsync(r => r.roleId.Equals(RoleId) && r.permissionId.Equals(PermissionId));
            _context.TLIrolePermission.Remove(rolePermission);
            await _context.SaveChangesAsync();
        }
        private async Task AddUserPermission(int UserId, int PermissionId)
        {
            var Check = _context.TLIuserPermission.Any(x => x.permissionId == PermissionId && x.userId == UserId);
            if(Check == false)
            {
                TLIuserPermission userPermission = new TLIuserPermission();
                userPermission.permissionId = PermissionId;
                userPermission.userId = UserId;
                userPermission.Active = true;
                userPermission.Deleted = false;
                await _context.TLIuserPermission.AddAsync(userPermission);
                await _context.SaveChangesAsync();
            }
        }

        private void DeleteUserPermission(int UserId, int PermissionId)
        {
            var userPermission = _context.TLIuserPermission.FirstOrDefault(u => u.permissionId.Equals(PermissionId) && u.userId.Equals(UserId));
            _context.TLIuserPermission.Remove(userPermission);
            _context.SaveChanges();
        }
    }
}
