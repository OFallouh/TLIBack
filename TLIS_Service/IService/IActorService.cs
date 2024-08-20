using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TLIS_DAL.Helper;
using TLIS_DAL.Helper.Filters;
using TLIS_DAL.Helpers;
using TLIS_DAL.ViewModels.ActorDTOs;
using TLIS_DAL.ViewModels.GroupDTOs;

namespace TLIS_Service.IService
{
    public interface IActorService
    {
        Response<IEnumerable<ActorViewModel>> GetActors(List<FilterObjectList> filters);
        Task<Response<ActorViewModel>> AddActor(AddActorViewModel Actor,int UserId);
        Task<Response<ActorViewModel>> UpdateActor(EditActorViewModel Actor, int UserId);
        Task<Response<IEnumerable<GroupViewModel>>> DeleteActor(ActorViewModel actorViewModel, int UserId);
        Response<List<GroupViewModel>> CheckIfActorIsExistInWorkflow(int ActorId);
        Task<Response<ActorViewModel>> DeleteActorFromGroups(ActorViewModel actorViewModel);
        Response<List<ActorViewModel>> GetActorByName(string ActorName);
        Response<List<GroupViewModel>> GetActorGroups(int ActorId);
    }
}
