using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TLIS_DAL.Helper;
using TLIS_DAL.Helper.Filters;
using TLIS_DAL.Helpers;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModelBase;
using TLIS_DAL.ViewModels.GroupDTOs;
using TLIS_DAL.ViewModels.RoleDTOs;
using TLIS_DAL.ViewModels.UserDTOs;

namespace TLIS_Service.IService
{
    public interface IGroupService
    {
        Response<IEnumerable<AddGroupsViewModel>> GetAllGroups(List<FilterObjectList> filters);
       // Response<AddGroupViewModel> AddGroup(AddGroupViewModel model, string domain);
        bool ValidateGroupNameFromADAdd(string GroupName, string domain);
        bool ValidateGroupNameFromDatabaseAdd(string GroupName);
        Task<bool> ValidateGroupNameFromDatabaseUpdate(string GroupName, int GroupId);
        Task<Response<GroupViewModel>> EditGroup([FromBody]GroupViewModel model, string domain);
        void UpdateGroupRoles(List<RoleViewModel> roles, int groupId);
        void UpdateGroupUsers(List<UserNameViewModel> users, int groupId);
        Task<Response<GroupViewModel>> DeleteGroup(int GrouId);
        Task<Response<GroupViewModel>> AddActorToGroup(int GroupId, int ActorId);
        Task<Response<GroupViewModel>> UpdateActorToGroup(int GroupId, int ActorId);
        Task<Response<GroupViewModel>> DeleteActorToGroup(int GroupId);
        Response<GroupViewModel> GetById(int GroupId);
        Response<IEnumerable<GroupViewModel>> GetGroupByName(string GroupName);
        Response<bool> CheckGroupChildrens(int GroupId);
        List<int> GetGroupsTest();
        Task<Response<GroupViewModel>> DeleteAssingedUppers(int GroupId);
        Response<GroupUppersLevels> GetUppersOfGroup(int GroupId);
        Task<Response<GroupUppersLevels>> AssignUpperToGroup(int GroupId, int NewUpperId);
        Response<List<GroupViewModel>> GetAllGroupsWithoutLowerLevelOfUppers(int GroupId, List<FilterObjectList> filters);
       // Response<GroupChildsViewModel> CheckGroupForParent(int GroupId);
        Response<List<GroupViewModel>> CheckGroup(int groupId);
        Response<string> DeleteGroupWithItsChildren(int GroupId, bool DeleteChilds);
        Response<AddGroupViewModel> AddGroup(AddGroupViewModel model);
        Response<GroupViewModel> UnAssignParentRleation(int GroupId);
        Response<string> DeleteGroupChildren(int GroupId);
        Response<IEnumerable<GroupViewModel>> GetGroupByName_WFVersion();
        Task<EscalationWFViewModel> GetUpperToEscalationWF(int UserId);
    }
}
