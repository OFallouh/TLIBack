using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using TLIS_DAL;
using TLIS_DAL.Helper;
using TLIS_DAL.Helper.Filters;
using TLIS_DAL.Helpers;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModels.ActorDTOs;
using TLIS_DAL.ViewModels.DynamicAttDTOs;
using TLIS_DAL.ViewModels.DynamicAttInstValueDTOs;
using TLIS_DAL.ViewModels.GroupDTOs;
using TLIS_DAL.ViewModels.RadioRRUDTOs;
using TLIS_DAL.ViewModels.RuleDTOs;
using TLIS_Repository.Base;
using TLIS_Service.IService;
using static TLIS_Service.Helpers.Constants;

namespace TLIS_Service.Services
{
    public class ActorService : IActorService
    {
        private readonly IUnitOfWork _unitOfWork;
        IServiceCollection _services;
        ServiceProvider _serviceProvider;
        ApplicationDbContext _dbContext;
        private IMapper _mapper;

        public ActorService(IUnitOfWork unitOfWork, IServiceCollection services, ApplicationDbContext dbContext, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _services = services;
            _dbContext = dbContext;
            _serviceProvider = _services.BuildServiceProvider();
            _mapper = mapper;
        }

        //Check the name of actor if alredy exists if not then add actor else return message that the actor is already exists
        public async Task<Response<ActorViewModel>> AddActor(AddActorViewModel Actor)
        {
            using (TransactionScope transaction = new TransactionScope())
            {
                try
                {
                    if (CheckNameForAdd(Actor.Name))
                    {
                        TLIactor ActorEntity = _mapper.Map<TLIactor>(Actor);
                        transaction.Complete();
                        _unitOfWork.ActorRepository.Add(ActorEntity);
                        await _unitOfWork.SaveChangesAsync();
                       // transaction.Complete();
                        return new Response<ActorViewModel>();
                    }
                    else
                    {
                        return new Response<ActorViewModel>(true, null, null, $"This Actor {Actor.Name} is already exists", (int)Helpers.Constants.ApiReturnCode.fail);
                    }
                }
                catch (Exception err)
                {
                    return new Response<ActorViewModel>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
                }
            }
        }
        //Depened on Id of actor 
        //if 
        //there is groups take that actor then return groups 
        //else 
        //delete the actor
        public async Task<Response<IEnumerable<GroupViewModel>>> DeleteActor(ActorViewModel actorViewModel)
        {
            try
            {
                List<TLIgroup> groups = _unitOfWork.GroupRepository.GetWhere(g => g.ActorId == actorViewModel.Id).ToList();
                if (groups.Count > 0)
                {
                    return new Response<IEnumerable<GroupViewModel>>(true, null, null, "There are groups that take this actor", (int)Helpers.Constants.ApiReturnCode.success);
                }
                else
                {
                    TLIactor ActorEntity = _mapper.Map<TLIactor>(actorViewModel);
                    _unitOfWork.ActorRepository.RemoveItem(ActorEntity);
                    await _unitOfWork.SaveChangesAsync();
                    return new Response<IEnumerable<GroupViewModel>>();
                }
            }
            catch (Exception err)
            {
                return new Response<IEnumerable<GroupViewModel>>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
        }
        public Response<List<GroupViewModel>> CheckIfActorIsExistInWorkflow(int ActorId)
        {
            try
            {
                TLIworkFlowGroup Workflows = _unitOfWork.WorkFlowGroupRepository.GetWhereFirst(x =>
                    x.ActorId == ActorId);

                if (Workflows != null)
                {
                    return new Response<List<GroupViewModel>>(true, null, null, "This Actor Is Assigned To Workflow Action And Can't Be Deleted", (int)Helpers.Constants.ApiReturnCode.fail);
                }
                else
                {
                    List<GroupViewModel> ActorGroups = GetActorGroups(ActorId).Data;

                    return new Response<List<GroupViewModel>>(true, ActorGroups, null, null, (int)Helpers.Constants.ApiReturnCode.success);
                }
            }
            catch (Exception err)
            {
                return new Response<List<GroupViewModel>>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
        }
        //delete actor from groups first get the groups then delete actor from those groups 
        public async Task<Response<ActorViewModel>> DeleteActorFromGroups(ActorViewModel actorViewModel)
        {
            using (TransactionScope transaction = new TransactionScope())
            {
                try
                {
                    var groups = _unitOfWork.GroupRepository.GetWhere(g => g.ActorId == actorViewModel.Id).ToList();
                    _unitOfWork.GroupRepository.UpdateRange(groups);
                    var actor = _mapper.Map<TLIactor>(actorViewModel);
                    _unitOfWork.ActorRepository.RemoveItem(actor);
                    await _unitOfWork.SaveChangesAsync();
                    transaction.Complete();
                    return new Response<ActorViewModel>();
                }
                catch (Exception err)
                {
                    return new Response<ActorViewModel>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
                }
            }

        }
        //Return the actors depened on filters
        public Response<IEnumerable<ActorViewModel>> GetActors(List<FilterObjectList> filters)
        {
            try
            {
                List<ActorViewModel> Actors = new List<ActorViewModel>();
                if (filters != null ? filters.Count() > 0 : false)
                    Actors = _mapper.Map<List<ActorViewModel>>(_unitOfWork.ActorRepository.GetWhere(x => x.Name.ToLower()
                        .StartsWith(filters.FirstOrDefault().value.FirstOrDefault().ToString().ToLower())).OrderBy(x => x.Name).ToList());

                else
                    Actors = _mapper.Map<List<ActorViewModel>>(_unitOfWork.ActorRepository.GetAllWithoutCount().OrderBy(x => x.Name).ToList());

                return new Response<IEnumerable<ActorViewModel>>(true, Actors, null, null, (int)Helpers.Constants.ApiReturnCode.success);
            }
            catch (Exception err)
            {
                return new Response<IEnumerable<ActorViewModel>>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
        }
        public Response<List<ActorViewModel>> GetActorByName(string ActorName)
        {
            try
            {
                if (string.IsNullOrEmpty(ActorName))
                {
                    List<ActorViewModel> Actormodels = _mapper.Map<List<ActorViewModel>>(_unitOfWork.ActorRepository.GetAllWithoutCount().ToList());
                    return new Response<List<ActorViewModel>>(true, Actormodels, null, null, (int)Helpers.Constants.ApiReturnCode.success, Actormodels.Count());
                }
                else
                {
                    List<ActorViewModel> Actormodel = _mapper.Map<List<ActorViewModel>>(_unitOfWork.ActorRepository.GetWhere(x => x.Name.ToLower().StartsWith(ActorName.ToLower())).ToList());
                    return new Response<List<ActorViewModel>>(true, Actormodel, null, null, (int)Helpers.Constants.ApiReturnCode.success, Actormodel.Count());
                }
            }
            catch (Exception err)
            {
                return new Response<List<ActorViewModel>>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
        }
        //If 
        //actor Id is 0 the return error message 
        //else  
        //if name is already exists and Id not equal to record Id then return error message that the name is already exists
        //else update the actor
        public async Task<Response<ActorViewModel>> UpdateActor(EditActorViewModel ActorModel)
        {
            using (TransactionScope transaction = new TransactionScope())
            {
                try
                {
                    if (ActorModel.Id == 0)
                    {
                        return new Response<ActorViewModel>(true, null, null, $"the Actor Id Shouldn't be null", (int)Helpers.Constants.ApiReturnCode.fail);
                    }
                    else if (await CheckNameForUpdate(ActorModel.Name, ActorModel.Id))
                    {
                        var Actor = _mapper.Map<TLIactor>(ActorModel);
                        _unitOfWork.ActorRepository.Update(Actor);
                        await _unitOfWork.SaveChangesAsync();
                        transaction.Complete();
                        return new Response<ActorViewModel>();
                    }
                    else
                    {
                        return new Response<ActorViewModel>(true, null, null, $"the Actor {ActorModel.Name} is already exists", (int)Helpers.Constants.ApiReturnCode.fail);
                    }
                }
                catch (Exception err)
                {
                    return new Response<ActorViewModel>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
                }
            }
        }
        //Function that Check if the name of actor is already exists return true 
        //else return false
        private bool CheckNameForAdd(string Name)
        {
            var Actor = _unitOfWork.ActorRepository.GetWhereFirst(a => a.Name == Name);
            if (Actor == null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        //Function that Check if the name of actor is exists and Id of actor not equal the Id of record then return true 
        //else return false
        private async Task<bool> CheckNameForUpdate(string Name, int Id)
        {
            var Actor = await _unitOfWork.ActorRepository.SingleOrDefaultAsync(x => x.Name == Name && x.Id != Id);
            if (Actor == null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public Response<List<GroupViewModel>> GetActorGroups(int ActorId)
        {
            try
            {
                List<GroupViewModel> Groups = _mapper.Map<List<GroupViewModel>>(_unitOfWork.GroupRepository.GetWhere(x =>
                    x.ActorId == ActorId).ToList());

                return new Response<List<GroupViewModel>>(true, Groups, null, null, (int)Helpers.Constants.ApiReturnCode.success);
            }
            catch (Exception err)
            {
                return new Response<List<GroupViewModel>>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail); ;
            }
        }
    }
}
