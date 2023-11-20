using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TLIS_DAL.Helpers;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModels.GroupDTOs;
using TLIS_DAL.ViewModels.GroupRoleDTOs;
using TLIS_DAL.ViewModels.GroupUserDTOs;
using TLIS_Repository.Base;
using TLIS_Service.IService;
using TLIS_Service.Helpers;
using TLIS_DAL.Helper.Filters;
using System.Linq;
using System.Transactions;
using System.DirectoryServices.AccountManagement;
using TLIS_DAL.ViewModels.UserDTOs;
using TLIS_DAL.ViewModels.RoleDTOs;
using TLIS_DAL.Helper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TLIS_DAL.ViewModelBase;
using TLIS_DAL;
using AutoMapper;
using TLIS_DAL.ViewModels.RowDTOs;
using TLIS_Repository.Repositories;
using System.ComponentModel;
using TLIS_DAL.ViewModels.StepActionDTOs;
using Org.BouncyCastle.Asn1.Ocsp;
using System.Text.RegularExpressions;

namespace TLIS_Service.Services
{
    public class GroupService : IGroupService
    {
        IUnitOfWork _unitOfWork;
        IServiceCollection _services;
        private IMapper _mapper;
        public GroupService(IUnitOfWork unitOfWork, IServiceCollection services, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _services = services;
            _mapper = mapper;
        }
        //Function take 2 parameters 
        //First GroupId to get group by Id
        //Second add actorId to group 
        public async Task<Response<GroupViewModel>> AddActorToGroup(int GroupId, int ActorId)
        {
            try
            {
                var group = _unitOfWork.GroupRepository.GetByID(GroupId);
                group.ActorId = ActorId;
                _unitOfWork.GroupRepository.Update(group);
                await _unitOfWork.SaveChangesAsync();
                var groups = _unitOfWork.GroupRepository.GetIncludeWhere(g => g.ActorId == null && g.ParentId == GroupId).ToList();
                if (groups.Count > 0)
                {
                    Parallel.ForEach(groups, gr =>
                    {
                        gr.ActorId = ActorId;
                        _unitOfWork.GroupRepository.Update(gr);
                    });
                }
                await _unitOfWork.SaveChangesAsync();
                return new Response<GroupViewModel>();
            }
            catch (Exception err)
            {
                return new Response<GroupViewModel>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
        }
        //Function take 1 parameter
        //First validate the group name from active directory
        //Second validate the group name from database
        //if the name is already exist in database return error message
        //if the name is already exist in active directory and not exsit in database add the group as internal group
        //if the name not exist in database and active directory then add the group as external group
        //public Response<AddGroupViewModel> AddGroup(AddGroupViewModel model, string domain)
        //{
        //    using (TransactionScope transaction = new TransactionScope())
        //    {
        //        try
        //        {
        //           bool DatabaseExists = ValidateGroupNameFromDatabaseAdd(model.Name);
        //            if (DatabaseExists)
        //            {
        //                return new Response<AddGroupViewModel>(true, null, null, $"This Group {model.Name} is already exists in Database", (int)Constants.ApiReturnCode.fail);
        //            }

        //            bool DomainExists = ValidateGroupNameFromADAdd(model.Name, domain);
        //            if (!DomainExists && model.GroupType == 1)
        //            {
        //                return new Response<AddGroupViewModel>(true, null, null, $"This Group {model.Name} is Not Exists in AD", (int)Constants.ApiReturnCode.fail);
        //            }

        //            else if (DomainExists && model.GroupType == 2)
        //            {
        //                return new Response<AddGroupViewModel>(true, null, null, $"This Group {model.Name} is Exists in AD And Can't Be An External User", (int)Constants.ApiReturnCode.fail);
        //            }

        //            else if(DomainExists && model.GroupType == 1)
        //            {
        //                TLIgroup Group = _mapper.Map<TLIgroup>(model);
        //                Group.GroupType = model.GroupType;
        //                _unitOfWork.GroupRepository.Add(Group);
        //                _unitOfWork.SaveChanges();
        //                if (model.UsersId != null ? model.UsersId.Count > 0 : false)
        //                {
        //                    foreach (var UserId in model.UsersId)
        //                    {
        //                        TLIgroupUser group = new TLIgroupUser();
        //                        group.groupId = Group.Id;
        //                        group.userId = UserId.Id;
        //                        _unitOfWork.GroupUserRepository.Add(group);
        //                        _unitOfWork.SaveChanges();
        //                    }
        //                }
        //                if (model.RolesId != null ? model.RolesId.Count > 0 : false)
        //                {
        //                    _unitOfWork.GroupRoleRepository.AddGroupRoles(Group.Id, model.RolesId);
        //                }
        //            }

        //            else if(!DomainExists && model.GroupType == 2)
        //            {
        //                TLIgroup Group = _mapper.Map<TLIgroup>(model);
        //                Group.GroupType = model.GroupType;
        //                _unitOfWork.GroupRepository.Add(Group);
        //                _unitOfWork.SaveChanges();

        //                if (model.UsersId != null ? model.UsersId.Count > 0 : false)
        //                {
        //                    foreach (var UserId in model.UsersId)
        //                    {
        //                        TLIgroupUser group = new TLIgroupUser();
        //                        group.groupId = Group.Id;
        //                        group.userId = UserId.Id;
        //                        _unitOfWork.GroupUserRepository.Add(group);
        //                        _unitOfWork.SaveChanges();
        //                    }
        //                }
        //                if (model.RolesId != null ? model.RolesId.Count > 0 : false)
        //                {
        //                    _unitOfWork.GroupRoleRepository.AddGroupRoles(Group.Id, model.RolesId);
        //                }
        //            }
        //            transaction.Complete();
        //            return new Response<AddGroupViewModel>(true, model, null, null, (int)Constants.ApiReturnCode.success);
        //        }
        //        catch (Exception err)
        //        {
        //            return new Response<AddGroupViewModel>(true, null, null, err.Message, (int)Constants.ApiReturnCode.fail);
        //        }               
        //    }
        //}
        public Response<AddGroupViewModel> AddGroup(AddGroupViewModel model)
        {
            using (TransactionScope Transaction = new TransactionScope())
            {
                try
                {
                    TLIgroup DatabaseExists = _unitOfWork.GroupRepository.GetWhereFirst(x =>
                        x.Name.ToLower() == model.Name.ToLower() && !x.Deleted && x.Active);

                    TLIgroup Group = new TLIgroup();

                    if (DatabaseExists != null)
                    {
                        if (!DatabaseExists.Deleted)
                        {
                            return new Response<AddGroupViewModel>(false, null, null, $"This Group {model.Name} is already exists in TLIS", (int)Constants.ApiReturnCode.fail);
                        }
                        else if (DatabaseExists.Deleted)
                        {
                            DatabaseExists.Deleted = !DatabaseExists.Deleted;
                            Group = DatabaseExists;
                            _unitOfWork.SaveChanges();
                        }
                    }
                    else
                    {
                        Group = _mapper.Map<TLIgroup>(model);
                        _unitOfWork.GroupRepository.Add(Group);
                        _unitOfWork.SaveChanges();


                    }

                    if (model.UsersId != null ? model.UsersId.Count() > 0 : false)
                    {
                        foreach (var userId in model.UsersId)
                        {
                            TLIgroupUser groupUser = new TLIgroupUser();
                            groupUser.groupId = Group.Id;
                            groupUser.userId = userId.Id;
                            _unitOfWork.GroupUserRepository.Add(groupUser);
                            _unitOfWork.SaveChanges();
                        }
                    }

                    if (model.RolesId != null ? model.RolesId.Count() > 0 : false)
                    {
                        foreach (var Role in model.RolesId)
                        {
                            TLIgroupRole groupRole = new TLIgroupRole();
                            groupRole.groupId = Group.Id;
                            groupRole.roleId = Role.Id;
                            _unitOfWork.GroupRoleRepository.Add(groupRole);
                            _unitOfWork.SaveChanges();


                        }
                    }

                    if (model.ParentId != null)
                    {
                        var ParentGroupRole = _unitOfWork.GroupRoleRepository.GetGroupRolesId((int)model.ParentId);

                        if (ParentGroupRole.Count != 0)
                        {
                            if (model.RolesId != null)
                            {
                                foreach (var ERole in model.RolesId)
                                {
                                    if (ParentGroupRole.Contains(ERole.Id))
                                    {
                                        ParentGroupRole.Remove(ERole.Id);
                                    }
                                }
                                foreach (var pg in ParentGroupRole)
                                {

                                    TLIgroupRole PgroupRole = new TLIgroupRole();
                                    PgroupRole.groupId = Group.Id;
                                    PgroupRole.roleId = pg;
                                    _unitOfWork.GroupRoleRepository.Add(PgroupRole);
                                    _unitOfWork.SaveChanges();
                                }
                            }

                            else
                            {
                                foreach (var pg in ParentGroupRole)
                                {

                                    TLIgroupRole PgroupRole = new TLIgroupRole();
                                    PgroupRole.groupId = Group.Id;
                                    PgroupRole.roleId = pg;
                                    _unitOfWork.GroupRoleRepository.Add(PgroupRole);
                                    _unitOfWork.SaveChanges();
                                }
                            }
                        }
                    }

                    Transaction.Complete();

                    return new Response<AddGroupViewModel>(true, model, null, null, (int)Constants.ApiReturnCode.success);
                }
                catch (Exception err)
                {
                    return new Response<AddGroupViewModel>(false, null, null, err.Message, (int)Constants.ApiReturnCode.fail);
                }
            }
        }
        //Frunction take 1 parameter
        //GroupId 
        //Frunction return if group have childrens return true or false
        public Response<bool> CheckGroupChildrens(int GroupId)
        {
            try
            {
                List<TLIgroup> GroupChildrens = _unitOfWork.GroupRepository.GetWhere(g =>
                    g.ParentId == GroupId && g.Active && !g.Deleted).ToList();

                if (GroupChildrens.Count == 0)
                    return new Response<bool>(true, false, null, null, (int)Helpers.Constants.ApiReturnCode.success);

                return new Response<bool>(true, true, null, null, (int)Helpers.Constants.ApiReturnCode.success);
            }
            catch (Exception err)
            {
                return new Response<bool>(true, false, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
        }
        //Function take 1 parameter 
        //get group by Id and then set ActorId to null
        public async Task<Response<GroupViewModel>> DeleteActorToGroup(int GroupId)
        {
            try
            {
                TLIgroup group = _unitOfWork.GroupRepository.GetByID(GroupId);
                group.ActorId = null;
                _unitOfWork.GroupRepository.Update(group);
                await _unitOfWork.SaveChangesAsync();
                return new Response<GroupViewModel>();
            }
            catch (Exception err)
            {
                return new Response<GroupViewModel>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
        }
        //Function take 1 parameter
        //First GrouId 
        //If group don't have childrens then delete group and relations
        //Else return group childrens
        public async Task<Response<GroupViewModel>> DeleteGroup(int GroupId)
        {
            try
            {
                List<TLIgroup> GroupCildrens = _unitOfWork.GroupRepository.GetWhere(g => g.ParentId == GroupId).ToList();
                TLIgroup DeleteGroup = _unitOfWork.GroupRepository.GetWhereFirst(x => x.Id == GroupId);
                if (GroupCildrens.Count > 0)
                {
                    foreach (var GroupCildren in GroupCildrens)
                    {
                        GroupCildren.ParentId = null;
                        _unitOfWork.GroupRepository.Update(GroupCildren);
                        await _unitOfWork.SaveChangesAsync();
                    }
                    DeleteGroup.Deleted = true;
                    DeleteGroup.ParentId = null;
                    DeleteGroup.UpperId = null;
                    DeleteGroup.Name = DeleteGroup.Name + DateTime.Now.ToString();
                    _unitOfWork.GroupRepository.Update(DeleteGroup);
                    await _unitOfWork.SaveChangesAsync();
                }

                List<TLIgroup> GroupLowers = _unitOfWork.GroupRepository.GetWhere(x => x.UpperId == GroupId).ToList();
                if (GroupLowers.Count > 0)
                {
                    foreach (var GroupLower in GroupLowers)
                    {
                        GroupLower.UpperId = null;
                        _unitOfWork.GroupRepository.Update(GroupLower);
                        await _unitOfWork.SaveChangesAsync();
                    }
                  
                    DeleteGroup.Deleted = true;
                    DeleteGroup.ParentId = null;
                    DeleteGroup.UpperId = null;
                    DeleteGroup.Name = DeleteGroup.Name + DateTime.Now.ToString();
                    _unitOfWork.GroupRepository.Update(DeleteGroup);
                    await _unitOfWork.SaveChangesAsync();
                }
                else if (GroupLowers.Count == 0 && GroupCildrens.Count == 0)
                {
                    DeleteGroup.Deleted = true;
                    DeleteGroup.ParentId = null;
                    DeleteGroup.UpperId = null;
                    DeleteGroup.Name = DeleteGroup.Name + DateTime.Now.ToString();
                    _unitOfWork.GroupRepository.Update(DeleteGroup);
                    await _unitOfWork.SaveChangesAsync();
                }
                return new Response<GroupViewModel>();
            }
            catch (Exception err)
            {
                return new Response<GroupViewModel>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
        }
        //public Response <GroupChildsViewModel> CheckGroupForParent(int GroupId)
        //{
        //   GroupChildsViewModel GroupChild =new GroupChildsViewModel {Group = new List<GroupViewModel>(), User = new List<UserViewModel>() };
        //    var GroupChilds = _mapper.Map<List<GroupViewModel>>(_unitOfWork.GroupRepository.GetWhere(x => x.ParentId == GroupId).ToList());
        //    foreach (var Group in GroupChilds)
        //    {
        //        GroupChild.Group.Add(Group);
        //        var users = _mapper.Map<List<UserViewModel>>(_unitOfWork.GroupUserRepository.GetIncludeWhere(x => x.groupId == Group.Id, x => x.user).Select(x => x.user).ToList());



        //        foreach (var user in users)
        //        {
        //            if (GroupChild.User.FirstOrDefault(x => x.UserName == user.UserName) == null)
        //            {
        //                GroupChild.User.Add(user);
        //            }                                                               
        //        }
        //        _unitOfWork.SaveChanges();
        //    }
        //    return new Response<GroupChildsViewModel> (true, GroupChild, null, null, (int)Helpers.Constants.ApiReturnCode.success);
        //}
        public Response<List<GroupViewModel>> CheckGroup(int GroupId)
        {
            List<GroupViewModel> group = new List<GroupViewModel>();
            var Parents = _mapper.Map<List<GroupViewModel>>(_unitOfWork.GroupRepository.GetWhere(x => x.ParentId == GroupId).ToList());

            foreach (var Parent in Parents)
            {

                var users = _unitOfWork.GroupUserRepository.GetIncludeWhere(x => x.groupId == Parent.Id, s => s.user).Select(s => new { s.user.UserName, s.user.Id }).ToList();
                var Users = _mapper.Map<List<UserNameViewModel>>(users);
                foreach (var User in Users)
                {
                    Parent.Users.Add(User);
                }
                group.Add(Parent);


            }
            return new Response<List<GroupViewModel>>(true, group, null, null, (int)Helpers.Constants.ApiReturnCode.success);
        }

        //Function take 1 parameter
        //First Check group name in active directory and database
        //return error message if the name exist in active directory or database
        //Update group
        //Update group roles if roles changed (delete role or add role)
        //Update group users if users changed (delete user or add user)
        private void LoopForChilds(int GroupId, List<int> AllChildsIds)
        {
            try
            {
                List<int> ChildsIds = _unitOfWork.GroupRepository.GetWhere(x =>
                    x.ParentId == GroupId && x.Active && !x.Deleted && !AllChildsIds.Any(y => y == x.Id)).Select(x => x.Id).ToList();

                foreach (int ChildId in ChildsIds)
                {
                    if (!AllChildsIds.Any(x => x == ChildId))
                        AllChildsIds.Add(ChildId);

                    List<int> NewChildsIds = _unitOfWork.GroupRepository.GetWhere(x =>
                        x.ParentId == ChildId && !AllChildsIds.Any(y => y == x.Id)).Select(x => x.Id).Distinct().ToList();

                    if (NewChildsIds.Count() > 0)
                        LoopForChilds(ChildId, AllChildsIds);
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        public async Task<Response<GroupViewModel>> EditGroup(GroupViewModel model, string domain)
        {
            bool DatabaseExists = await ValidateGroupNameFromDatabaseUpdate(model.Name, model.Id);
            if (DatabaseExists)
            {
                return new Response<GroupViewModel>(false, null, null, $"The Group {model.Name} is already exists in Database", (int)Constants.ApiReturnCode.fail);
            }
            //bool DomainExists = ValidateGroupNameFromADAdd(model.Name, domain);
            //if (DomainExists == true)
            //{
            //    return new Response<GroupViewModel>(true, null, null, $"The Group {model.Name} is already exists in AD", (int)Constants.ApiReturnCode.fail);
            //}
            try
            {
                TLIgroup groupEntity = _mapper.Map<TLIgroup>(model);
                _unitOfWork.GroupRepository.Update(groupEntity);

                List<int> AllChildsIds = new List<int>();
                LoopForChilds(groupEntity.Id, AllChildsIds);
                AllChildsIds.Add(groupEntity.Id);
                await _unitOfWork.GroupRepository.UpdateGroupRoles(model.Roles, AllChildsIds);
                //await _unitOfWork.GroupRepository.UpdateGroupUsers(model.Users, groupEntity.Id);
                await UpdateGroupUsersHelperMethod(model.Users, groupEntity.Id);

                await _unitOfWork.SaveChangesAsync();
                return new Response<GroupViewModel>(true, null, null, null, (int)Constants.ApiReturnCode.success);
            }
            catch (Exception err)
            {
                return new Response<GroupViewModel>(false, null, null, err.Message, (int)Constants.ApiReturnCode.fail);
            }
        }
        private async Task UpdateGroupUsersHelperMethod(List<UserNameViewModel> Users, int GroupId)
        {
            try
            {
                using (TransactionScope transaction = new TransactionScope())
                {
                    List<UserNameViewModel> GroupUsers = _mapper.Map<List<UserNameViewModel>>(_unitOfWork.GroupUserRepository.GetIncludeWhere(x =>
                            x.groupId == GroupId, x => x.user)).ToList();

                    List<UserNameViewModel> UsersToAdd = Users.Except(GroupUsers).ToList();
                    List<TLIgroupUser> AddOnes = new List<TLIgroupUser>();
                    foreach (UserNameViewModel UserToAdd in UsersToAdd)
                    {
                        AddOnes.Add(new TLIgroupUser
                        {
                            groupId = GroupId,
                            userId = UserToAdd.Id,
                            Deleted = false,
                            Active = true
                        });
                    }
                    await _unitOfWork.GroupUserRepository.AddRangeAsync(AddOnes);

                    var UsersToDelete = GroupUsers.Except(Users).Select(x=>x.UserName).ToList();
                    List<TLIgroupUser> DeleteOnes = _unitOfWork.GroupUserRepository.GetIncludeWhere(x =>
                        UsersToDelete.Any(y => y == x.user.UserName), x => x.user).ToList();
                    _unitOfWork.GroupUserRepository.RemoveRangeItems(DeleteOnes);

                    transaction.Complete();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public Response<GroupViewModel> UnAssignParentRleation(int GroupId)
        {
            try
            {
                var Group = _unitOfWork.GroupRepository.GetByID(GroupId);
                if (Group != null)
                {
                    Group.ParentId = null;
                    _unitOfWork.SaveChanges();

                }
                return new Response<GroupViewModel>(true, null, null, null, (int)Constants.ApiReturnCode.success);
            }
            catch (Exception err)
            {

                return new Response<GroupViewModel>(true, null, null, err.Message, (int)Constants.ApiReturnCode.fail);
            }
        }
        //public Response <GroupViewModel>UpdateGroup (GroupViewModel Model)
        //{
        //    try
        //    {

        //    }
        //    catch (Exception)
        //    {

        //        throw;
        //    }
        //}
        //Function take 1 parameter filters
        //Function return filtered groups and with every group list of group users and list of group roles
        public Response<IEnumerable<AddGroupsViewModel>> GetAllGroups(List<FilterObjectList> filters)
        {
            try
            {
                List<AddGroupsViewModel> Groups = new List<AddGroupsViewModel>();

                List<TLIgroup> GroupAll = _unitOfWork.GroupRepository.GetWhere(x => !x.Deleted && x.Active).ToList();
                foreach (var item in GroupAll)
                {
                    int? L1 = null; int? L2 = null;  int? L3 = null; int? L2u = null; int? L3u = null;
                    string L1Name = null;  string L2Name = null;  string L3Name = null; string ParentName = null; string ActorName = null;
                    if (item.UpperId != null)
                    {
                         L1 = _unitOfWork.GroupRepository.GetWhereFirst(x => x.Id == item.UpperId).Id;
                        if (L1 != null)
                        {
                            TLIgroup objL1 = _unitOfWork.GroupRepository.GetWhereFirst(x => x.Id == L1 && x.Active && !x.Deleted);
                            L1Name = objL1?.Name;
                          L2 = _unitOfWork.GroupRepository.GetWhereFirst(x => x.Id == L1).UpperId;

                        }
                        if (L2 != null)
                        {
                            TLIgroup objl2 = _unitOfWork.GroupRepository.GetWhereFirst(x => x.Id == L2 && x.Active && !x.Deleted);
                            L2Name = objl2?.Name;
                          L3 = _unitOfWork.GroupRepository.GetWhereFirst(x => x.Id == L2 && x.Active && !x.Deleted).UpperId;

                        }
                        if (L3 != null)
                        {
                            TLIgroup objl3 = _unitOfWork.GroupRepository.GetWhereFirst(x => x.Id == L3 && x.Active && !x.Deleted);
                            L3Name = objl3?.Name;
                        }
                        
                    }
                    if (item.ParentId != null)
                    {
                        TLIgroup objparentname = _unitOfWork.GroupRepository.GetWhereFirst(x => x.Id == item.ParentId && x.Active && !x.Deleted);
                        ParentName = objparentname?.Name;
                    }
                    if (item.ActorId != null)
                    {
                        TLIactor ObjActorName = _unitOfWork.ActorRepository.GetWhereFirst(x => x.Id == item.ActorId);
                        ActorName = ObjActorName?.Name;

                    }
                    Groups.Add(new AddGroupsViewModel()
                    {
                        Id = item.Id,
                        Name = item?.Name,
                        ParentId = item?.ParentId,
                        ParentName = ParentName,
                        GroupType = item?.GroupType,
                        Active = item.Active,
                        Deleted = item.Deleted,
                        ActorId = item.ActorId,
                        ActorName = ActorName,
                        UpperLevel1Id = L1,
                        UpperLevel1Name = L1Name,
                        UpperLevel2Id = L2,
                        UpperLevel2Name = L2Name,
                        UpperLevel3Id = L3,
                        UpperLevel3Name = L3Name

                    });
                }

                if (filters != null ? filters.Count() > 0 : false)
                {


                    Groups = Groups.Where(x => x.Name.ToLower().StartsWith(filters.FirstOrDefault().value.FirstOrDefault().ToString().ToLower())).ToList().OrderBy(x => x.Name).ToList();


                }
                foreach (var Group in Groups)
                {
                    List<UserNameViewModel> userNameViewModels = new List<UserNameViewModel>();
                    List<TLIgroupUser> Users = _unitOfWork.GroupUserRepository.GetWhere
                        (x => x.groupId == Group.Id && x.Active && !x.Deleted).ToList();
                    foreach (var item in Users)
                    {
                        int? UserType = _unitOfWork.UserRepository.GetWhereFirst(x => x.Id == item.userId).UserType;
                        userNameViewModels.Add(new UserNameViewModel()
                        {
                            Id = item.userId,
                            UserName = item.user.UserName,
                            UserType = UserType,
                            Deleted=item.Deleted,
                            Active=item.Active
                            
                        });
                    }
                    Group.Users = userNameViewModels;
                    Group.Roles = _unitOfWork.GroupRoleRepository.GetWhereAndSelect(x => x.groupId == Group.Id && x.Active && !x.Deleted,
                        x => new RoleViewModel { Id = x.roleId, Name = x.role.Name, Active = x.role.Active, Deleted = x.role.Deleted }).ToList();
                }

                return new Response<IEnumerable<AddGroupsViewModel>>(true, Groups.OrderBy(x=>x.Name), null, null, (int)Helpers.Constants.ApiReturnCode.success, Groups.Count());
            }
            catch (Exception err)
            {
                return new Response<IEnumerable<AddGroupsViewModel>>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }

        }
        //Function take 1 parameter GroupId
        //Function return the group and list of group roles and list of group roles
        public Response<GroupViewModel> GetById(int GroupId)
        {
            try
            {
                var group = _unitOfWork.GroupRepository.GetIncludeWhereFirst(x => x.Id == GroupId, s => s.Actor);
                var GroupModel = _mapper.Map<GroupViewModel>(group);
                var users = _unitOfWork.GroupUserRepository.GetWhere(g => g.groupId == GroupId && g.user.Active == true).ToList();
                foreach (var user in users)
                {
                    var userEntity = _unitOfWork.UserRepository.GetByID(user.userId);
                    var UserModel = _mapper.Map<UserNameViewModel>(userEntity);
                    GroupModel.Users.Add(UserModel);
                }
                var roles = _unitOfWork.GroupRoleRepository.GetWhere(g => g.groupId == GroupId).ToList();
                foreach (var role in roles)
                {
                    var roleEntity = _unitOfWork.RoleRepository.GetByID(role.roleId);
                    var roleModel = _mapper.Map<RoleViewModel>(roleEntity);
                    GroupModel.Roles.Add(roleModel);
                }
                return new Response<GroupViewModel>(true, GroupModel, null, null, (int)Helpers.Constants.ApiReturnCode.success);
            }
            catch (Exception err)
            {

                return new Response<GroupViewModel>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }

        }
        //Function take 1 parameter
        //Function return list of groups start with specific string and with every group his parent group
        public Response<IEnumerable<GroupViewModel>> GetGroupByName_WFVersion()
        {
            try
            {
                List<GroupViewModel> GroupsViewModels = _mapper.Map<List<GroupViewModel>>(_unitOfWork.GroupRepository
                    .GetAllWithoutCount().ToList()).ToList();

                return new Response<IEnumerable<GroupViewModel>>(true, GroupsViewModels, null, null, (int)Helpers.Constants.ApiReturnCode.success, GroupsViewModels.Count());
            }
            catch (Exception err)
            {
                return new Response<IEnumerable<GroupViewModel>>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
        }
        public Response<IEnumerable<GroupViewModel>> GetGroupByName(string GroupName)
        {
            try
            {
                if (string.IsNullOrEmpty(GroupName))
                {
                    List<GroupViewModel> GroupsViewModels = _mapper.Map<List<GroupViewModel>>(_unitOfWork.GroupRepository
                        .GetWhere(x => !x.Deleted).ToList()).OrderBy(x => x.Name).ToList();

                    return new Response<IEnumerable<GroupViewModel>>(true, GroupsViewModels, null, null, (int)Helpers.Constants.ApiReturnCode.success, GroupsViewModels.Count());
                }
                else
                {
                    var Groups = _unitOfWork.GroupRepository.GetWhere(x => !x.Deleted && x.Name.ToLower().StartsWith(GroupName.ToLower())).ToList();
                    for (int i = 0; i < Groups.Count; i++)
                    {
                        TLIgroup group = null;
                        if (Groups[i].ParentId != null ? Groups[i].ParentId != 0 : false)
                        {
                            if (Groups.FirstOrDefault(x => x.Id == Groups[i].ParentId) == null)
                            {
                                group = _unitOfWork.GroupRepository.GetByID((int)Groups[i].ParentId);
                                if (group != null)
                                {
                                    Groups.Add(group);
                                }
                            }
                        }
                    }
                    var GroupsViewModels = _mapper.Map<List<GroupViewModel>>(Groups);
                    return new Response<IEnumerable<GroupViewModel>>(true, GroupsViewModels, null, null, (int)Helpers.Constants.ApiReturnCode.success, GroupsViewModels.Count());
                }




            }
            catch (Exception err)
            {
                return new Response<IEnumerable<GroupViewModel>>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }

        }
        //Function take 2 parameter
        //First GroupId to get group by Id
        //Second Update selected group with ActorId
        public async Task<Response<GroupViewModel>> UpdateActorToGroup(int GroupId, int ActorId)
        {

            try
            {
                TLIgroup group = _unitOfWork.GroupRepository.GetWhereFirst(x => x.Id == GroupId && x.Active && !x.Deleted);

                group.ActorId = ActorId;
                await _unitOfWork.GroupRepository.UpdateItem(group);
                await _unitOfWork.SaveChangesAsync();
                return new Response<GroupViewModel>();
            }
            catch (Exception err)
            {

                return new Response<GroupViewModel>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
        }
        //Function take 2 parameters
        //Function update group roles (add role or delete role)
        public void UpdateGroupRoles(List<RoleViewModel> roles, int groupId)
        {
            List<int> GroupId = new List<int>();
            GroupId.Add(groupId);
            _unitOfWork.GroupRepository.UpdateGroupRoles(roles, GroupId);
        }
        //Function take 2 parameters
        //Function update group users (add user or delete user)
        public void UpdateGroupUsers(List<UserNameViewModel> users, int groupId)
        {
            _unitOfWork.GroupRepository.UpdateGroupUsers(users, groupId);
        }
        //Check group name in active directory for add
        public bool ValidateGroupNameFromADAdd(string GroupName, string domain)
        {
            return _unitOfWork.GroupRepository.ValidateGroupNameFromADAdd(GroupName, domain);
        }
        //Check group name in database for add
        public bool ValidateGroupNameFromDatabaseAdd(string GroupName)
        {
            return _unitOfWork.GroupRepository.ValidateGroupNameFromDatabaseAdd(GroupName);
        }
        //Check group name in database for update
        public async Task<bool> ValidateGroupNameFromDatabaseUpdate(string GroupName, int GroupId)
        {
            return await _unitOfWork.GroupRepository.ValidateGroupNameFromDatabaseUpdate(GroupName, GroupId);
        }
        public List<int> GetGroupsTest()
        {
            List<int> result = new List<int>();
            List<Tuple<string, string, string, object>> filters = new List<Tuple<string, string, string, object>>();
            filters.Add(new Tuple<string, string, string, object>("TLIdynamicAttLibValue", "Value", "==", "1"));
            filters.Add(new Tuple<string, string, string, object>("TLIdynamicAtt", "Key", "==", "Test"));
            filters.Add(new Tuple<string, string, string, object>("TLItablesNames", "TableName", "==", "TLIcivilWithLegLibrary"));
            return result;
        }
        public async Task<Response<GroupViewModel>> DeleteAssingedUppers(int GroupId)
        {
            try
            {
                TLIgroup MainGroup = _unitOfWork.GroupRepository.GetByID(GroupId);
                if (MainGroup != null)
                {
                    if (MainGroup.UpperId != null)
                    {
                        MainGroup.UpperId = null;
                    }
                    else
                    {
                        return new Response<GroupViewModel>(true, null, null, $"No Upper Group Found For This Id : {GroupId}", (int)Helpers.Constants.ApiReturnCode.fail);
                    }
                }
                else
                {
                    return new Response<GroupViewModel>(true, null, null, $"No Group is Found With This Id : {GroupId}", (int)Helpers.Constants.ApiReturnCode.fail);
                }
                _unitOfWork.GroupRepository.Update(MainGroup);
                await _unitOfWork.SaveChangesAsync();
                return new Response<GroupViewModel>();
            }
            catch (Exception err)
            {
                return new Response<GroupViewModel>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }

        }
        public Response<GroupUppersLevels> GetUppersOfGroup(int GroupId)
        {
            try
            {
                GroupUppersLevels Groups = new GroupUppersLevels();
                TLIgroup MainGroup = _unitOfWork.GroupRepository.GetIncludeWhereFirst(x => x.Active && !x.Deleted && x.Id == GroupId, x => x.Parent, x => x.Upper);
                if (MainGroup == null)
                {
                    return new Response<GroupUppersLevels>(true, null, null, $"No Group Found For This Id : {GroupId}", (int)Helpers.Constants.ApiReturnCode.fail);
                }
                if (MainGroup.UpperId != null)
                {
                    GroupViewModel UpperLevel1 = _mapper.Map<GroupViewModel>(_unitOfWork.GroupRepository.GetIncludeWhereFirst(x => x.Id == MainGroup.UpperId && x.Active && !x.Deleted, x => x.Parent, x => x.Upper));
                    if (UpperLevel1 != null)
                    {
                        Groups.Level1 = UpperLevel1;
                        if (UpperLevel1.UpperId != null)
                        {
                            GroupViewModel UpperLevel2 = _mapper.Map<GroupViewModel>(_unitOfWork.GroupRepository.GetIncludeWhereFirst(x => x.Id == UpperLevel1.UpperId.Value && x.Active && !x.Deleted, x => x.Parent, x => x.Upper));
                            if (UpperLevel2 != null)
                            {
                                Groups.Level2 = UpperLevel2;
                                if (UpperLevel2.UpperId != null)
                                {
                                    GroupViewModel UpperLevel3 = _mapper.Map<GroupViewModel>(_unitOfWork.GroupRepository.GetIncludeWhereFirst(x => x.Id == UpperLevel2.UpperId.Value && x.Active && !x.Deleted, x => x.Parent, x => x.Upper));
                                    if (UpperLevel3 != null)
                                    {
                                        Groups.Level3 = UpperLevel3;
                                    }
                                }
                            }
                        }
                    }
                }  
                int Count = (Groups.Level1 != null ? (Groups.Level2 != null ? (Groups.Level3 != null ? 3 : 2) : 1) : 0);
                return new Response<GroupUppersLevels>(true, Groups, null, null, (int)Helpers.Constants.ApiReturnCode.success, Count);
            }
            catch (Exception err)
            {
                return new Response<GroupUppersLevels>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
        }
        public async Task<Response<GroupUppersLevels>> AssignUpperToGroup(int GroupId, int NewUpperId)
        {
            try
            {
                if (GroupId == NewUpperId)
                    return new Response<GroupUppersLevels>(true, null, null, $"You Can't Assign Group To Itself", (int)Helpers.Constants.ApiReturnCode.fail);

                TLIgroup MainGroup = _unitOfWork.GroupRepository.GetWhereFirst(x => x.Id == GroupId && x.Active && !x.Deleted);

                if (MainGroup == null)
                    return new Response<GroupUppersLevels>(true, null, null, $"No Group Found With This Id : {GroupId}", (int)Helpers.Constants.ApiReturnCode.fail);

                bool Check = _unitOfWork.GroupRepository.GetWhereFirst(x => x.Id == NewUpperId) != null;

                if (!Check)
                    return new Response<GroupUppersLevels>(true, null, null, $"No Group Found With This Id : {NewUpperId}", (int)Helpers.Constants.ApiReturnCode.fail);

                MainGroup.UpperId = NewUpperId;
                await _unitOfWork.SaveChangesAsync();

                GroupUppersLevels Groups = GetUppersOfGroup(GroupId).Data;

                int Count = (Groups.Level3 != null ? 3 : (Groups.Level2 != null ? 2 : (Groups.Level1 != null ? 1 : 0)));

                return new Response<GroupUppersLevels>(true, Groups, null, null, (int)Helpers.Constants.ApiReturnCode.success, Count);
            }
            catch (Exception err)
            {
                return new Response<GroupUppersLevels>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
        }

        // Helper Method For GetAllGroupsWithoutLowerLevelOfUppers Api
        private void LoopForLowerGroups(List<int> LowersIds, List<int> exeptLowerGroups)
        {
            try
            {
                foreach (var LowerId in LowersIds)
                {
                    if (!exeptLowerGroups.Any(x => x == LowerId))
                        exeptLowerGroups.Add(LowerId);
                    List<int> NewLowersIds = _unitOfWork.GroupRepository.GetWhere(x =>
                        x.UpperId == LowerId && !exeptLowerGroups.Any(y => y == x.Id) && x.Active && !x.Deleted).Select(x => x.Id).Distinct().ToList();
                    if (LowersIds.Count() > 0)
                        LoopForLowerGroups(NewLowersIds, exeptLowerGroups);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public Response<List<GroupViewModel>> GetAllGroupsWithoutLowerLevelOfUppers(int GroupId, List<FilterObjectList> filters)
        {
            try
            {
                TLIgroup Group = _unitOfWork.GroupRepository.GetWhereFirst(x => x.Id == GroupId && !x.Deleted && x.Active);

                if (Group == null)
                    return new Response<List<GroupViewModel>>(true, null, null, $"No Group is found with this Id {GroupId}", (int)Helpers.Constants.ApiReturnCode.fail);

                List<TLIgroup> AllGroups = new List<TLIgroup>();
                List<int> exeptLowerGroups = new List<int>();
                List<int> LowersIds = new List<int>();
    
                LowersIds = _unitOfWork.GroupRepository.GetWhere(x => (x.UpperId == GroupId || x.Id == GroupId) && x.Active && !x.Deleted).Select(x => x.Id).ToList();
                LoopForLowerGroups(LowersIds, exeptLowerGroups);

                if (Group.UpperId != null)
                    exeptLowerGroups.Add(Group.UpperId.Value);
                //List<int> Groups = GetLastSon(GroupId);

                //if (Groups != null)
                //{
                //    exeptLowerGroups.AddRange(Groups);
                //}

                if (filters != null ? filters.Count() > 0 : false)
                {
                    AllGroups = _unitOfWork.GroupRepository.GetWhere(x => !exeptLowerGroups.Any(y => y == x.Id) && !x.Deleted && x.Active &&
                        x.Name.ToLower().StartsWith(filters.FirstOrDefault().value.FirstOrDefault().ToString().ToLower())).OrderBy(x => x.Name).ToList();
                }
                else
                {
                    AllGroups = _unitOfWork.GroupRepository.GetWhere(x => !exeptLowerGroups.Any(y => y == x.Id) && !x.Deleted && x.Active).ToList();
                }
                //else if (Level == 2)
                //{
                //    if (Group.UpperId == null)
                //    {
                //        return new Response<List<GroupViewModel>>(true, null, null, $"upper level 1 is not exist so you have to insert one before try to insert upper level 2", (int)Helpers.Constants.ApiReturnCode.fail);
                //    }

                //    TLIgroup grouplevel1 = _unitOfWork.GroupRepository.GetWhereFirst(x => x.Id == Group.UpperId.Value);

                //    LowersIds = _unitOfWork.GroupRepository.GetWhere(x =>
                //        (x.UpperId == GroupId || x.Id == GroupId ||
                //        x.UpperId == grouplevel1.Id || x.Id == grouplevel1.Id) && x.Active && !x.Deleted).Select(x => x.Id).ToList();

                //    LoopForLowerGroups(LowersIds, exeptLowerGroups);

                //    if (grouplevel1.UpperId != null)
                //        exeptLowerGroups.Add(grouplevel1.UpperId.Value);
                //    List<int> Groups = GetLastSon(GroupId);

                //    if (Groups != null)
                //    {
                //        exeptLowerGroups.AddRange(Groups);
                //    }
                //    AllGroups = _unitOfWork.GroupRepository.GetWhere(x => !exeptLowerGroups.Any(y => y == x.Id) && !x.Deleted && x.Active).ToList();

                //}
                //else if (Level == 3)
                //{
                //    if (Group.UpperId == null)
                //    {
                //        return new Response<List<GroupViewModel>>(true, null, null, $"upper level 1 is not exist so you have to insert one before try to insert upper level 2", (int)Helpers.Constants.ApiReturnCode.fail);
                //    }

                //    TLIgroup grouplevel1 = _unitOfWork.GroupRepository.GetWhereFirst(x => x.Id == Group.UpperId.Value);

                //    if (grouplevel1.UpperId == null)
                //        return new Response<List<GroupViewModel>>(true, null, null, $"upper level 2 is not exist so you have to insert one before try to insert upper level 3", (int)Helpers.Constants.ApiReturnCode.fail);

                //    TLIgroup grouplevel2 = _unitOfWork.GroupRepository.GetWhereFirst(x => x.Id == grouplevel1.UpperId.Value);
                //    LowersIds = _unitOfWork.GroupRepository.GetWhere(x =>
                //        (x.UpperId == GroupId || x.Id == GroupId ||
                //        x.UpperId == grouplevel1.Id || x.Id == grouplevel1.Id ||
                //        x.UpperId == grouplevel2.Id || x.Id == grouplevel2.Id) && x.Active && !x.Deleted).Select(x => x.Id).ToList();

                //    LoopForLowerGroups(LowersIds, exeptLowerGroups);

                //    if (grouplevel2.UpperId != null)
                //        exeptLowerGroups.Add(grouplevel2.UpperId.Value);
                //    List<int> Groups = GetLastSon(GroupId);

                //    if (Groups != null)
                //    {
                //        exeptLowerGroups.AddRange(Groups);
                //    }
                //    AllGroups = _unitOfWork.GroupRepository.GetWhere(x => !exeptLowerGroups.Any(y => y == x.Id) && !x.Deleted && x.Active).ToList();
                //}
                //else
                //{
                //    return new Response<List<GroupViewModel>>(false, null, null, $"No Level Found For This Level {Level}", (int)Helpers.Constants.ApiReturnCode.fail);
                //}

                return new Response<List<GroupViewModel>>(true, _mapper.Map<List<GroupViewModel>>(AllGroups), null, null, (int)Helpers.Constants.ApiReturnCode.success);
            }
            catch (Exception err)
            {
                return new Response<List<GroupViewModel>>(false, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
        }

        public Response<string> DeleteGroupWithItsChildren(int GroupId, bool DeleteChilds)
        {
            try
            {
                TLIgroup GroupToDelete = _unitOfWork.GroupRepository.GetWhereFirst(x => x.Id == GroupId && x.Active && !x.Deleted);
                if (GroupToDelete != null)
                {
                    if (DeleteChilds)
                    {
                        DeleteGroupChildren(GroupId);
                    }
                    else
                    {
                        using (TransactionScope transaction = new TransactionScope())
                        {
                            List<TLIgroup> Childs = _unitOfWork.GroupRepository.GetWhere(x =>
                                x.ParentId == GroupId && x.Active && !x.Deleted).ToList();
                           
                            foreach (TLIgroup Child in Childs)
                            {
                                Child.ParentId = null;
                            }
                            _unitOfWork.GroupRepository.UpdateRange(Childs);
                            _unitOfWork.SaveChangesAsync();

                            transaction.Complete();
                        }
                    }
                    using (TransactionScope transaction = new TransactionScope())
                    {
                        List<TLIgroupRole> Roles = _unitOfWork.GroupRoleRepository.GetWhere(x =>
                            x.groupId == GroupId).ToList();
                        Roles.Any(x => x.Deleted).Equals(true);
                        _unitOfWork.GroupRoleRepository.UpdateRange(Roles);

                        List<TLIgroupUser> Users = _unitOfWork.GroupUserRepository.GetWhere(x =>
                            x.groupId == GroupId).ToList();
                        Users.Any(x => x.Deleted).Equals(true);
                        _unitOfWork.GroupUserRepository.UpdateRange(Users);

                        List<TLIgroup> Uppers = _unitOfWork.GroupRepository.GetWhere(x =>
                              x.UpperId == GroupId).ToList();

                        foreach (TLIgroup upper in Uppers)
                        {
                            upper.UpperId = null;
                        }
                        _unitOfWork.GroupRepository.UpdateRange(Uppers);

                        GroupToDelete.Deleted = true;
                        GroupToDelete.ParentId = null;
                        GroupToDelete.UpperId = null;
                        GroupToDelete.Name = GroupToDelete.Name + DateTime.Now.ToString();
                        _unitOfWork.GroupRepository.UpdateItem(GroupToDelete);
                        _unitOfWork.SaveChangesAsync();

                        transaction.Complete();
                    }
                }

                return new Response<string>(true, "Succeed", null, null, (int)Helpers.Constants.ApiReturnCode.success);
            }
            catch (Exception err)
            {
                return new Response<string>(false, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
        }

        public Response<string> DeleteGroupChildren(int GroupId)
        {
            try
            {
                using (TransactionScope transaction = new TransactionScope(TransactionScopeOption.Required, TimeSpan.FromSeconds(60)))
                {
                    List<int> FirstLevelOfChildsIds = _unitOfWork.GroupRepository.GetWhere(x =>
                        x.ParentId == GroupId).Select(x => x.Id).ToList();
                    List<int> AllChildsLevels = new List<int>();

                    GetAllChildsIds(FirstLevelOfChildsIds, AllChildsLevels);

                    List<TLIgroup> DeleteGroups = _unitOfWork.GroupRepository.GetWhere(x => AllChildsLevels.Any(y => y == x.Id)&&!x.Deleted).ToList();

                    foreach (TLIgroup Child in DeleteGroups)
                    {
                        Child.Deleted = true;
                        Child.ParentId = null;
                        Child.UpperId = null;
                        Child.Name = Child.Name + DateTime.Now.ToString();
                    }
                    _unitOfWork.GroupRepository.UpdateRange(DeleteGroups);
                    List<TLIgroup> GroupS = _unitOfWork.GroupRepository.GetAllWithoutCount().ToList();
                    List<TLIgroup> Uppers = GroupS.Where(x=> DeleteGroups.Any(y=>y.Id==x.UpperId)).ToList();

                    foreach (var upper in Uppers)
                    {
                        upper.UpperId = null;
                    }
                    _unitOfWork.GroupRepository.UpdateRange(Uppers);
                    _unitOfWork.SaveChangesAsync();
                    transaction.Complete();
                }
                return new Response<string>(true, "Succeed", null, null, (int)Helpers.Constants.ApiReturnCode.fail);
            }
            catch (Exception err)
            {
                return new Response<string>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
        }
        public Response<EscalationWFViewModel> GetUpperToEscalationWF(int UserId)
        {
            try
            {
                EscalationWFViewModel Groups = new EscalationWFViewModel();
                var Group = _unitOfWork.GroupUserRepository.GetWhere(x => x.userId == UserId).ToList();
                foreach (var item in Group)
                {
                    TLIgroup MainGroup = _unitOfWork.GroupRepository.GetIncludeWhereFirst(x => x.Active && !x.Deleted && x.Id == item.groupId, x => x.Parent, x => x.Upper);
                    if (MainGroup != null)
                    {
                        if (MainGroup.UpperId != null)
                        {
                            GroupViewModel UpperLevel1 = _mapper.Map<GroupViewModel>(_unitOfWork.GroupRepository.GetIncludeWhereFirst(x => x.Id == MainGroup.UpperId && x.Active && !x.Deleted, x => x.Parent, x => x.Upper));
                            if (UpperLevel1 != null)
                            {
                                Groups.AllLevel1 = UpperLevel1;
                                if (UpperLevel1.UpperId != null)
                                {
                                    GroupViewModel UpperLevel2 = _mapper.Map<GroupViewModel>(_unitOfWork.GroupRepository.GetIncludeWhereFirst(x => x.Id == UpperLevel1.UpperId.Value && x.Active && !x.Deleted, x => x.Parent, x => x.Upper));
                                    if (UpperLevel2 != null)
                                    {
                                        Groups.AllLevel2 = UpperLevel2;
                                        if (UpperLevel2.UpperId != null)
                                        {
                                            GroupViewModel UpperLevel3 = _mapper.Map<GroupViewModel>(_unitOfWork.GroupRepository.GetIncludeWhereFirst(x => x.Id == UpperLevel2.UpperId.Value && x.Active && !x.Deleted, x => x.Parent, x => x.Upper));
                                            if (UpperLevel3 != null)
                                            {
                                                Groups.AllLevel3 = UpperLevel3;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                  
                }
                if(Groups.AllLevel1 != null)
                {
                    var Group1Id = Groups.AllLevel1.UpperId;
                    var Userofgroup1 = _unitOfWork.GroupUserRepository.GetWhere(x => Group1Id == x.groupId).Select(x => x.userId).ToList();
                    var MailUserofGroup1 = _unitOfWork.UserRepository.GetWhere(x => Userofgroup1.Any(y => y == x.Id)).ToList();
                    var Mailinfoofgroup1 = _mapper.Map<List<EscalationViewModel>>(MailUserofGroup1);
                    Groups.Level1 = _mapper.Map < List<EscalationViewModel>>(Mailinfoofgroup1);
                }
                if (Groups.AllLevel2 != null)
                {
                    var Group2Id = Groups.AllLevel2.UpperId;
                    var Userofgroup2 = _unitOfWork.GroupUserRepository.GetWhere(x => Group2Id == x.groupId).Select(x => x.userId).ToList();
                    var MailUserofGroup2 = _unitOfWork.UserRepository.GetWhere(x => Userofgroup2.Any(y => y == x.Id)).Select(x => x.Email).ToList();
                    var Mailinfoofgroup2 = _mapper.Map<List<EscalationViewModel>>(MailUserofGroup2);
                    Groups.Level2 = _mapper.Map<List<EscalationViewModel>>(Mailinfoofgroup2);
                }
                if (Groups.AllLevel3 != null)
                {
                    var Group3Id = Groups.AllLevel2.UpperId;
                    var Userofgroup3 = _unitOfWork.GroupUserRepository.GetWhere(x => Group3Id == x.groupId).Select(x => x.userId).ToList();
                    var MailUserofGroup3 = _unitOfWork.UserRepository.GetWhere(x => Userofgroup3.Any(y => y == x.Id)).Select(x => x.Email).ToList();
                    var Mailinfoofgroup3 = _mapper.Map<List<EscalationViewModel>>(MailUserofGroup3);
                    Groups.Level3 = _mapper.Map<List<EscalationViewModel>>(Mailinfoofgroup3);
                }
                return new Response<EscalationWFViewModel>(true, Groups, null, null, (int)Helpers.Constants.ApiReturnCode.success);
            }
            catch (Exception err)
            {

                return new Response<EscalationWFViewModel>(false, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }


        }

        // Helper Method For DeleteGroupWithItsChilds Api
        public void GetAllChildsIds(List<int> FirstLevelOfChildsIds, List<int> AllChildsLevels)
        {
            try
            {
                foreach (int ChildId in FirstLevelOfChildsIds)
                {
                    if (!AllChildsLevels.Any(x => x == ChildId))
                        AllChildsLevels.Add(ChildId);

                    List<int> NewChilds = _unitOfWork.GroupRepository.GetWhere(x =>
                        x.ParentId == ChildId && !AllChildsLevels.Any(y => y == x.Id)).Select(x => x.Id).ToList();

                    if (NewChilds.Count() > 0)
                        GetAllChildsIds(NewChilds, AllChildsLevels);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<int> GetLastSon(int groupId)
        {
            List<int> Childs = new List<int>();
            var currentGroup = _unitOfWork.GroupRepository.GetWhere(x => x.Id == groupId && !x.Deleted && x.Active).Select(x=>x.Id).ToList();
            var Group = _unitOfWork.GroupRepository.GetWhere(x => x.Active && !x.Deleted).ToList();
            while ( currentGroup.Count != 0)
            {           
                    Childs.AddRange(currentGroup);
                
                    currentGroup = Group.Where(x => currentGroup.Any(y=>y==x.ParentId)).Select(x=>x.Id).ToList();
                
            }
            return Childs;
        }
    }
}
