using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using TLIS_DAL;
using TLIS_DAL.Helper;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModels.GroupDTOs;
using TLIS_DAL.ViewModels.RoleDTOs;
using TLIS_DAL.ViewModels.UserDTOs;
using TLIS_Repository.Base;
using TLIS_Repository.IRepository;

namespace TLIS_Repository.Repositories
{
    public class GroupRepository:RepositoryBase<TLIgroup, GroupViewModel, int>, IGroupRepository
    {
        private readonly ApplicationDbContext _context;
        IMapper _mapper;

        public GroupRepository(ApplicationDbContext context, IMapper mapper) :base(context,mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task DeleteGroup(int GroupId)
        {
            using (TransactionScope transaction = new TransactionScope())
            {
                try
                {
                    var groupUsers = await _context.TLIgroupUser.AsNoTracking().Where(g => g.groupId.Equals(GroupId)).ToListAsync();
                    var groupusersID = await GetGroupUsersIdByGroupId(GroupId);
                    var groupRoles = await _context.TLIgroupRole.AsNoTracking().Where(g => g.groupId.Equals(GroupId)).ToListAsync();
                    var GroupChilds = await _context.TLIgroup.AsNoTracking().Where(x => x.ParentId == GroupId).ToListAsync();
                    _context.TLIgroupUser.RemoveRange(groupUsers);
                    await _context.SaveChangesAsync();
                    _context.TLIgroupRole.RemoveRange(groupRoles);
                    await _context.SaveChangesAsync();
                    foreach (var user in groupusersID)
                    {
                        var userGroups = await _context.TLIgroupUser.AsNoTracking().Where(g => g.userId.Equals(user)).Select(g => g.groupId).ToListAsync();
                        var groupsRoles = await GetGroupRolesIdByGroupsId(userGroups);
                        var rolesPermissions = await GetRolesPermissionsIdByRolesId(groupsRoles);
                        var userPermissions = await GetUserPermissionsByUserId(user);
                        var permissionsToDelete = userPermissions.Except(rolesPermissions);
                        await DeleteUserPermissions(user, permissionsToDelete);
                    }
                    var group = await _context.TLIgroup.AsNoTracking().FirstOrDefaultAsync(g => g.Id.Equals(GroupId));
                    _context.TLIgroup.Remove(group);
                    await _context.SaveChangesAsync();
                    foreach (var GroupChild in GroupChilds)
                    {
                        GroupChild.ParentId = null;
                        _context.TLIgroup.Update(GroupChild); 
                    }
                    await _context.SaveChangesAsync();
                    transaction.Complete();
                }
                catch(Exception err)
                {

                }
            }
        }
        public async Task UpdateGroupRoles(List<RoleViewModel> roles, List<int> AllChildsIds)
        {
            using (TransactionScope transaction = new TransactionScope(TransactionScopeOption.Required, TimeSpan.FromSeconds(60)))
            {
                try
                {
                    foreach (int ChildId in AllChildsIds)
                    {
                        var GroupRoles = await _context.TLIgroupRole.AsNoTracking().Where(g => g.groupId.Equals(ChildId)).Select(g => g.roleId).ToListAsync();
                        var rolesId = roles.Select(r => r.Id);
                        var RolesToAdd = rolesId.Except(GroupRoles);
                        var RolesToDelete = GroupRoles.Except(rolesId);
                        var groupUsersID = await GetGroupUsersIdByGroupId(ChildId);
                        foreach (var Role in RolesToAdd)
                        {
                            var rolePermissions = await _context.TLIrolePermission.AsNoTracking().Where(r => r.roleId.Equals(Role)).Select(r => r.permissionId).ToListAsync();
                            foreach (var User in groupUsersID)
                            {
                                var userPermissions = await GetUserPermissionsByUserId(User);
                                var PermissionsToAdd = rolePermissions.Except(rolePermissions);
                                await AddUserPermissions(User, PermissionsToAdd);
                            }
                            await AddGroupRole(ChildId, Role);
                        }
                        await DeleteGroupRoles(ChildId, RolesToDelete);
                        foreach (var user in groupUsersID)
                        {
                            var userGroups = await _context.TLIgroupUser.AsNoTracking().Where(u => u.userId.Equals(user)).Select(u => u.groupId).ToListAsync();
                            var GroupsRoles = await GetGroupRolesIdByGroupsId(userGroups);
                            var RolesPermissions = await GetRolesPermissionsIdByRolesId(GroupsRoles);
                            var userPermissions = await GetUserPermissionsByUserId(user);
                            var PermissionToDelete = userPermissions.Except(RolesPermissions);
                            await DeleteUserPermissions(user, PermissionToDelete);
                        }
                    }
                    
                    transaction.Complete();
                }
                catch (Exception)
                {

                }
            }
                
        }
        
        public async Task UpdateGroupUsers(List<UserNameViewModel> users, int groupId)
        {
            using (TransactionScope transaction = new TransactionScope(TransactionScopeOption.Required, TimeSpan.FromSeconds(60)))
            {
                try
                {
                    var groupusersID = await GetGroupUsersIdByGroupId(groupId);
                    var groupRoles = await _context.TLIgroupRole.AsNoTracking().Where(g => g.groupId.Equals(groupId)).Select(g => g.roleId).ToListAsync();
                    var RolesPermissions = await GetRolesPermissionsIdByRolesId(groupRoles);
                    var usersId = users.Select(u => u.Id);
                    var usersToAdd = usersId.Except(groupusersID);
                    var usersToDelete = groupusersID.Except(usersId);
                    foreach (var user in usersToAdd)
                    {
                        await AddGroupUser(groupId, user);
                        var userPermissions = await GetUserPermissionsByUserId(user);
                        var PermissionToAdd = userPermissions.Except(RolesPermissions);
                        await AddUserPermissions(user, PermissionToAdd);
                    }
                    foreach (var user in usersToDelete)
                    {
                        await DeleteGroupUser(user, groupId);
                        var userGroups = _context.TLIgroupUser.Where(g => g.userId.Equals(user)).Select(g => g.groupId).ToList();
                        userGroups.Remove(groupId);
                        var GroupRoles = await GetGroupRolesIdByGroupsId(userGroups);
                        var rolePermissions = await GetRolesPermissionsIdByRolesId(GroupRoles);
                        var userPermissions = await GetUserPermissionsByUserId(user);
                        var PermissionToDelete = userPermissions.Except(rolePermissions);
                        await DeleteUserPermissions(user, PermissionToDelete);
                    }
                    transaction.Complete();
                }
                catch (Exception)
                {

                }
            }      
        }
        public bool ValidateGroupNameFromADAdd(string GroupName, string domain)
        {
            using (PrincipalContext context = new PrincipalContext(ContextType.Domain, domain, null, ContextOptions.SimpleBind))
            {
                GroupPrincipal groupPrincipal = null;
                if (context != null)
                {
                    groupPrincipal = GroupPrincipal.FindByIdentity(context, IdentityType.SamAccountName, GroupName);
                }
                if (groupPrincipal == null)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }
        public bool ValidateGroupNameFromDatabaseAdd(string GroupName)
        {
            return _context.TLIgroup.Any(g => g.Name == GroupName);
        }
        private async Task<List<int>> GetGroupUsersIdByGroupId(int GroupId)
        {
            return await _context.TLIgroupUser.AsNoTracking().Where(g => g.groupId.Equals(GroupId)).Select(g => g.userId).ToListAsync();
        }
        private async Task<List<int>> GetGroupRolesIdByGroupsId(List<int> GroupsId)
        {
            return await _context.TLIgroupRole.AsNoTracking().Where(g => (GroupsId.Contains(g.groupId))).Select(g => g.roleId).Distinct().ToListAsync();
        }
        private async Task<List<int>> GetRolesPermissionsIdByRolesId(List<int> RolesId)
        {
            return await _context.TLIrolePermission.AsNoTracking().Where(r => (RolesId.Contains(r.roleId))).Select(r => r.permissionId).Distinct().ToListAsync();
        }
        private async Task<List<int>> GetUserPermissionsByUserId(int UserId)
        {
            return await _context.TLIuserPermission.AsNoTracking().Where(u => u.userId.Equals(UserId)).Select(u => u.permissionId).ToListAsync();
        }
        private async Task DeleteUserPermissions(int UserId,IEnumerable<int> Permissions)
        {
            using (TransactionScope transaction = new TransactionScope())
            {
                try
                {
                    foreach (var permission in Permissions)
                    {
                        var userPermission = await _context.TLIuserPermission.AsNoTracking().FirstOrDefaultAsync(u => u.userId.Equals(UserId) && u.permissionId.Equals(permission));
                        _context.TLIuserPermission.Remove(userPermission);
                    }
                    await _context.SaveChangesAsync();
                    transaction.Complete();
                }
                catch(Exception)
                {

                }
            }    
        }
        private async Task AddUserPermissions(int UserId,IEnumerable<int> Permissions)
        {
            using (TransactionScope transaction = new TransactionScope())
            {
                var UserPermissions = _context.TLIuserPermission.Where(x => x.userId == UserId).Select(x => x.permissionId).ToList();
                try
                {
                    foreach (var Permission in Permissions)
                    {
                        var Check = UserPermissions.Contains(Permission);
                        if(Check == false)
                        {
                            TLIuserPermission userPermission = new TLIuserPermission();
                            userPermission.permissionId = Permission;
                            userPermission.userId = UserId;
                            await _context.TLIuserPermission.AddAsync(userPermission);
                        }
                    }
                    await _context.SaveChangesAsync();
                    transaction.Complete();
                }
                catch(Exception)
                {

                }
            } 
        }
        private async Task AddGroupRole(int GroupId,int RoleId)
        {
            using (TransactionScope transaction = new TransactionScope(TransactionScopeOption.Required, TimeSpan.FromSeconds(60)))
            {
                var Check = _context.TLIgroupRole.FirstOrDefault(x => x.groupId == GroupId && x.roleId == RoleId);
                if (Check == null)
                {
                    TLIgroupRole groupRole = new TLIgroupRole();
                    groupRole.groupId = GroupId;
                    groupRole.roleId = RoleId;
                    await _context.TLIgroupRole.AddAsync(groupRole);
                    await _context.SaveChangesAsync();
                }
                transaction.Complete();
            }
        }

        private async Task DeleteGroupRoles(int GroupId,IEnumerable<int> Roles)
        {
            using (TransactionScope transaction = new TransactionScope(TransactionScopeOption.Required, TimeSpan.FromSeconds(60)))
            {
                foreach (var role in Roles)
                {
                    var groupRole = await _context.TLIgroupRole.AsNoTracking().FirstOrDefaultAsync(g => g.groupId.Equals(GroupId) && g.roleId.Equals(role));
                    _context.TLIgroupRole.Remove(groupRole);
                    await _context.SaveChangesAsync();
                }
                transaction.Complete();
            }
        }

        private async Task AddGroupUser(int GroupId,int UserId)
        {
            using (TransactionScope transaction = new TransactionScope(TransactionScopeOption.Required, TimeSpan.FromSeconds(60)))
            {
                var Check = _context.TLIgroupUser.FirstOrDefault(x => x.groupId == GroupId && x.userId == UserId);
                if (Check == null)
                {
                    TLIgroupUser groupUser = new TLIgroupUser();
                    groupUser.groupId = GroupId;
                    groupUser.userId = UserId;
                    await _context.TLIgroupUser.AddAsync(groupUser);
                    await _context.SaveChangesAsync();
                }
                transaction.Complete();
            }
        }
        private async Task DeleteGroupUser(int? UserId,int? GroupId)
        {
            using (TransactionScope transaction = new TransactionScope(TransactionScopeOption.Required, TimeSpan.FromSeconds(60)))
            {
                var userToDelete = await _context.TLIgroupUser.AsNoTracking().FirstOrDefaultAsync(g => g.groupId.Equals(GroupId) && g.userId.Equals(UserId));
                _context.TLIgroupUser.Remove(userToDelete);
                await _context.SaveChangesAsync();
                transaction.Complete();
            }
        }

        public async Task<bool> ValidateGroupNameFromDatabaseUpdate(string GroupName, int GroupId)
        {
            return await _context.TLIgroup.AsNoTracking().AnyAsync(g => g.Name.Equals(GroupName) && g.Id != GroupId);
        }
        /// <summary>
        /// Test
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        

        public void GetGroupsTest<TEntity>(params string[] Values) 
        {
            //BinaryExpression expression = null;
            //var Parameter = Expression.Parameter(typeof(T), "x");
            //foreach (var Value in Values)
            //{
            //    var Left = Expression.Property(Parameter, typeof(T).GetProperty("Actor"));
            //    Left = Expression.Property(Left, typeof(F).GetProperty("Name"));
            //    var value = Expression.Constant(Value);
            //    var expression_1 = Expression.Equal(Left, value);
            //    if(expression == null)
            //    {
            //        expression = expression_1;
            //    }
            //    else
            //    {
            //        expression = Expression.Or(expression, expression_1);
            //    }
            //}
            //////////////////////////////////////////////////////////
            //var lambda = Expression.Lambda<Func<TLIgroup, bool>>(expression, Parameter).Compile();
            //var Perdicate = ExpressionUtils.False<TLIgroup>();
            //foreach(var word in words)
            //{
            //    Perdicate = Perdicate.Or(p => p.Name.Contains(word));
            //}
            //////////////////////////////////////////////////////////
            //var result = _context.TLIdynamicAtt.Select(x => x.Id).ToList();
            //var DynamicattParameter = Expression.Parameter(typeof(T), "x");
            //var DynamicPreperty = Expression.Property(DynamicattParameter, typeof(T).GetProperty("tablesNamesId"));
            //var DynamicPreperty_1 = Expression.Property(DynamicattParameter, typeof(T).GetProperty("Id"));
            //var DynamicArgs = new ParameterExpression[] { DynamicattParameter };
            //Expression<Func<TLIdynamicAtt, int>> l1 = Expression.Lambda<Func<TLIdynamicAtt, int>>(DynamicPreperty, DynamicArgs);
            //var TableNameParameter = Expression.Parameter(typeof(F), "y");
            //var TableNameProperty = Expression.Property(TableNameParameter, typeof(F).GetProperty("Id"));
            //var TableNameArgs = new ParameterExpression[] { TableNameParameter };
            //Expression<Func<TLItablesNames, int>> l2 = Expression.Lambda<Func<TLItablesNames, int>>(TableNameProperty, TableNameArgs);
            //ParameterExpression ap1 = Expression.Parameter(typeof(T),"x");
            //ParameterExpression ap2 = Expression.Parameter(typeof(F),"y");
            //Expression ap3 = ap1;
            //var args3 = new ParameterExpression[] { ap1, ap2 };
            ////var expression = Expression.Equal(DynamicArgs, TableNameProperty);
            //Expression<Func<TLIdynamicAtt, TLItablesNames, int>> l3 = Expression.Lambda<Func<TLIdynamicAtt, TLItablesNames, int>>(DynamicPreperty_1, args3);
            //var test = _context.TLIdynamicAtt.Join(_context.TLItablesNames, x => x.tablesNamesId, y => y.Id, (x, y) => x.Id).ToList();
            //var test_1 = _context.TLIdynamicAtt.Join(_context.TLItablesNames, l1, l2, l3).ToList();
            ////return _context.TLIdynamicAtt.Join(_context.TLItablesNames, l1, l2, l3).ToList();
            //return result;
            ////////////////////////////////////////////////////////////////////////////////////////
            
        }
        /////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////
    }
}
