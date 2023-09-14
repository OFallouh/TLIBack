using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TLIS_DAL.Helper;
using TLIS_DAL.Helper.Filters;
using TLIS_DAL.Helpers;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModels.ActionDTOs;
using TLIS_DAL.ViewModels.ActionItemOptionDTOs;
using TLIS_DAL.ViewModels.ActorDTOs;
using TLIS_DAL.ViewModels.GroupDTOs;
using TLIS_DAL.ViewModels.IntegrationDTOs;
using TLIS_DAL.ViewModels.ItemStatusDTOs;
using TLIS_DAL.ViewModels.OrderStatusDTOs;
using TLIS_DAL.ViewModels.StepActionDTOs;
using TLIS_DAL.ViewModels.StepDTOs;
using TLIS_DAL.ViewModels.UserDTOs;
using TLIS_DAL.ViewModels.WorkFlowDTOs;
using TLIS_DAL.ViewModels.WorkFlowGroupDTOs;
using TLIS_DAL.ViewModels.WorkFlowTypeDTOs;
using TLIS_Repository.Base;
using TLIS_Service.IService;

namespace TLIS_Service.Services
{
    public class WorkflowService : IWorkflowService
    {
        IUnitOfWork _unitOfWork;
        IServiceCollection _services;
        private IMapper _mapper;
        public WorkflowService(IUnitOfWork unitOfWork, IServiceCollection services, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _services = services;
            ServiceProvider serviceProvider = _services.BuildServiceProvider();
            _mapper = mapper;
        }


        //-------------------------------------------  WorkFlow

        /// <summary>
        /// return all workflow that their deleted flag is false
        /// </summary>
        public Response<List<ListWorkFlowViewModel>> GetAllWorkflows(ParameterPagination parameterPagination, List<FilterObjectList> filter)
        {
            int count = 0;
            try
            {
                var workflowTemplate = _unitOfWork.WorkFlowRepository.GetAllIncludeMultiple(parameterPagination, filter, out count); //
                var workflowModel = _mapper.Map<List<ListWorkFlowViewModel>>(workflowTemplate);
                foreach (var item in workflowModel)
                {
                    item.WorkFlowGroups = _mapper.Map<List<WorkFlowGroupVM>>(_unitOfWork.WorkFlowGroupRepository.GetAllAsQueryable().Where(x => x.WorkFlowId == item.Id).ToList());
                    item.WorkFlowTypes = _mapper.Map<List<ListWorkFlowTypeViewModel>>(_unitOfWork.WorkFlowTypeRepository.GetAllAsQueryable().Where(x => x.WorkFlowId == item.Id).ToList());

                }
                return new Response<List<ListWorkFlowViewModel>>(true, workflowModel, null, null, (int)Helpers.Constants.ApiReturnCode.success, count);
            }
            catch (Exception err)
            {
                
                return new Response<List<ListWorkFlowViewModel>>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }

        }
        public Response<ListWorkFlowViewModel> GetWorkflowbyId(int Id)
        {
            try
            {

                return new Response<ListWorkFlowViewModel>(true, GetOneWorkflowbyId(Id), null, null, (int)Helpers.Constants.ApiReturnCode.success);
            }
            catch (Exception err)
            {
                
                return new Response<ListWorkFlowViewModel>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }

        }

        public Response<ListWorkFlowViewModel> AddWorkflow(AddWorkFlowViewModel Workflow)
        {
            try
            {
                if (CheckWorkflowName(Workflow.Name, null))
                {
                    TLIworkFlow entity = _mapper.Map<TLIworkFlow>(Workflow);
                    _unitOfWork.WorkFlowRepository.Add(entity);
                    _unitOfWork.SaveChanges();
                    return new Response<ListWorkFlowViewModel>(_mapper.Map<ListWorkFlowViewModel>(entity));
                }
                else
                {
                    return new Response<ListWorkFlowViewModel>(true, null, null, $"This Workflow {Workflow.Name} is already exists", (int)Helpers.Constants.ApiReturnCode.fail);
                }
            }
            catch (Exception err)
            {
                
                return new Response<ListWorkFlowViewModel>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }

        }

        public Response<ListWorkFlowViewModel> DeleteWorkflow(int workflowId)
        {
            try
            {
                var workflowTemplate = GetOneFullWorkFlowById(workflowId);
                workflowTemplate.DateDeleted = DateTime.Now;
                workflowTemplate.Deleted = true;
                _unitOfWork.WorkFlowRepository.Update(workflowTemplate);
                _unitOfWork.SaveChanges();
                return new Response<ListWorkFlowViewModel>(_mapper.Map<ListWorkFlowViewModel>(workflowTemplate));
            }
            catch (Exception err)
            {
                
                return new Response<ListWorkFlowViewModel>(false, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }

        }

        //*
        public ListWorkFlowViewModel GetOneWorkflowbyId(int Id)
        {
            try
            {
                var item = _mapper.Map<ListWorkFlowViewModel>(GetOneFullWorkFlowById(Id));
                return item;
            }
            catch (Exception err)
            {
                
                return null;
            }

        }
        public TLIworkFlow GetOneFullWorkFlowById(int id)
        {
            var item = _unitOfWork.WorkFlowRepository.GetAllAsQueryable().Where(x => x.Id == id).FirstOrDefault();
            item.WorkFlowGroups = _unitOfWork.WorkFlowGroupRepository.GetAllAsQueryable().Where(x => x.WorkFlowId == id).ToList();
            item.WorkFlowTypes = _unitOfWork.WorkFlowTypeRepository.GetAllAsQueryable().Where(x => x.WorkFlowId == id).ToList();
            return item;
        }
        //*/

        public Response<ListWorkFlowViewModel> ChangeWorkflowStatus(int workflowId)
        {
            try
            {
                var workflowTemplate = GetOneFullWorkFlowById(workflowId);
                workflowTemplate.Active = !workflowTemplate.Active;
                _unitOfWork.WorkFlowRepository.Update(workflowTemplate);
                _unitOfWork.SaveChanges();
                return new Response<ListWorkFlowViewModel>();
            }
            catch (Exception err)
            {
                
                return new Response<ListWorkFlowViewModel>(false, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }

        }


        public Response<ListWorkFlowViewModel> UpdateWorkflow(EditWorkFlowViewModel Workflow)
        {
            try
            {
                if (Workflow.Name == null || Workflow.Name == "")
                {
                    return new Response<ListWorkFlowViewModel>(true, null, null, $"WorkFlow name couldnot be null", (int)Helpers.Constants.ApiReturnCode.fail);
                }
                if (CheckWorkflowName(Workflow.Name, Workflow.Id))
                {
                    var workflowTemplate = GetOneFullWorkFlowById(Workflow.Id);
                    workflowTemplate.Name = Workflow.Name;
                    workflowTemplate.SiteStatusId = Workflow.SiteStatusId;
                    _unitOfWork.WorkFlowRepository.Update(workflowTemplate);
                    _unitOfWork.SaveChanges();
                    return new Response<ListWorkFlowViewModel>();
                }
                else
                {
                    return new Response<ListWorkFlowViewModel>(true, null, null, $"This workflow {Workflow.Name} is already exitsts", (int)Helpers.Constants.ApiReturnCode.fail);
                }
            }
            catch (Exception err)
            {
                
                return new Response<ListWorkFlowViewModel>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }

        }
        public Response<ListWorkFlowViewModel> WorkflowPermissions(WorkFlowGroupsViewModel Workflow)
        {
            try
            {
                var workflowTemplate = GetOneFullWorkFlowById(Workflow.Id);
                //*
                if (workflowTemplate.WorkFlowGroups == null)
                {
                    workflowTemplate.WorkFlowGroups = new List<TLIworkFlowGroup>();
                }
                workflowTemplate.WorkFlowGroups.Clear();
                _unitOfWork.WorkFlowRepository.Update(workflowTemplate);
                _unitOfWork.SaveChanges();
                if (Workflow.WorkFlowGroups.Groups != null)
                    foreach (var wfg in Workflow.WorkFlowGroups.Groups)
                    {
                        TLIworkFlowGroup wfGroup = new TLIworkFlowGroup();
                        wfGroup.WorkFlowId = Workflow.Id;
                        //wfGroup.WorkFlow = workflowTemplate;
                        wfGroup.GroupId = wfg;
                        //wfGroup.Group = _unitOfWork.GroupRepository.GetAllAsQueryable().Where(x => x.Id == wfg).FirstOrDefault();
                        workflowTemplate.WorkFlowGroups.Add(wfGroup);
                    }
                if (Workflow.WorkFlowGroups.Actors != null)
                    foreach (var wfg in Workflow.WorkFlowGroups.Actors)
                    {
                        TLIworkFlowGroup wfGroup = new TLIworkFlowGroup();
                        wfGroup.WorkFlowId = Workflow.Id;
                        //wfGroup.WorkFlow = workflowTemplate;
                        wfGroup.ActorId = wfg;
                        //wfGroup.Actor = _unitOfWork.ActorRepository.GetAllAsQueryable().Where(x => x.Id == wfg).FirstOrDefault();
                        workflowTemplate.WorkFlowGroups.Add(wfGroup);
                    }
                if (Workflow.WorkFlowGroups.Integrations != null)
                    foreach (var wfg in Workflow.WorkFlowGroups.Integrations)
                    {
                        TLIworkFlowGroup wfGroup = new TLIworkFlowGroup();
                        wfGroup.WorkFlowId = Workflow.Id;
                        //wfGroup.WorkFlow = workflowTemplate;
                        wfGroup.IntegrationId = wfg;
                        //wfGroup.Integration = _unitOfWork.IntegrationRepo.GetAllAsQueryable().Where(x => x.Id == wfg).FirstOrDefault();
                        workflowTemplate.WorkFlowGroups.Add(wfGroup);
                    }
                if (Workflow.WorkFlowGroups.Users != null)
                    foreach (var wfg in Workflow.WorkFlowGroups.Users)
                    {
                        TLIworkFlowGroup wfGroup = new TLIworkFlowGroup();
                        wfGroup.WorkFlowId = Workflow.Id;
                        //wfGroup.WorkFlow = workflowTemplate;
                        wfGroup.UserId = wfg;
                        //wfGroup.User = _unitOfWork.UserRepository.GetAllAsQueryable().Where(x => x.Id == wfg).FirstOrDefault();
                        workflowTemplate.WorkFlowGroups.Add(wfGroup);
                    }
                //*/
                _unitOfWork.WorkFlowRepository.Update(workflowTemplate);
                _unitOfWork.SaveChanges();
                return new Response<ListWorkFlowViewModel>();
            }
            catch (Exception err)
            {
                
                return new Response<ListWorkFlowViewModel>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }

        }

        /// <summary>
        /// check if there is a WorkFlow already exists on Database by same name
        /// </summary>
        /// <param name="Name">this name must be unique in database</param>
        /// <returns>result will be true in case of this name is not existon database or exist for same id. otherwise, response would be false </returns>
        private bool CheckWorkflowName(string Name, int? Id)
        {
            if (Id != null)
            {
                var item = _unitOfWork.WorkFlowRepository.GetAllAsQueryable().Where(x => x.Name == Name && x.Deleted == false && x.Id != Id).FirstOrDefault();
                if (item != null)
                {
                    return false;
                }
            }
            else
            {
                var item = _unitOfWork.WorkFlowRepository.GetAllAsQueryable().Where(x => x.Name == Name && x.Deleted == false).FirstOrDefault();
                if (item != null)
                {
                    return false;
                }
            }
            return true;
        }
        //-------------------------------------------  end of WorkFlow


        //-------------------------------------------  Workflow Type       

        public Response<List<ListWorkFlowTypeViewModel>> GetAllWorkflowTypes(ParameterPagination parameterPagination, List<FilterObjectList> filter, int WorkflowId)
        {
            int count = 0;
            try
            {
                if (filter == null)
                {
                    filter = new List<FilterObjectList>();
                }
                List<object> filterToWorkFlow = new List<object>();
                filterToWorkFlow.Add("" + WorkflowId);
                var belongToWorkFlow = new FilterObjectList("WorkFlowId", filterToWorkFlow);
                filter.Add(belongToWorkFlow);
                var workflowTemplate = _unitOfWork.WorkFlowTypeRepository.GetAllIncludeMultiple(parameterPagination, filter, out count); //
                var workflowModel = _mapper.Map<List<ListWorkFlowTypeViewModel>>(workflowTemplate);
                return new Response<List<ListWorkFlowTypeViewModel>>(true, workflowModel, null, null, (int)Helpers.Constants.ApiReturnCode.success);
            }
            catch (Exception err)
            {
                
                return new Response<List<ListWorkFlowTypeViewModel>>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }

        }
        public TLIworkFlowType GetOneWorkflowTypeById(int Id)
        {
            try
            {
                return _unitOfWork.WorkFlowTypeRepository.GetAllAsQueryable().Where(x => x.Id == Id && x.Deleted == false).SingleOrDefault();
            }
            catch (Exception err)
            {
                
                return null;
            }

        }
        public Response<ListWorkFlowTypeViewModel> GetWorkflowTypeById(int Id)
        {
            try
            {
                var workflowModel = _mapper.Map<ListWorkFlowTypeViewModel>(GetOneWorkflowTypeById(Id));
                return new Response<ListWorkFlowTypeViewModel>(true, workflowModel, null, null, (int)Helpers.Constants.ApiReturnCode.success);
            }
            catch (Exception err)
            {
                
                return new Response<ListWorkFlowTypeViewModel>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }

        }

        public Response<ListWorkFlowTypeViewModel> AddWorkflowType(AddWorkFlowTypeViewModel WorkflowType)
        {
            try
            {
                if (WorkflowType.Name == null || WorkflowType.Name == "")
                {
                    return new Response<ListWorkFlowTypeViewModel>(true, null, null, $"The name of workflow type couldnot be null", (int)Helpers.Constants.ApiReturnCode.fail);
                }
                if (CheckWorkflowTypeName(WorkflowType.WorkFlowId, WorkflowType.Name, null))
                {
                    _unitOfWork.WorkFlowTypeRepository.Add(_mapper.Map<TLIworkFlowType>(WorkflowType));
                    _unitOfWork.SaveChanges();
                    return new Response<ListWorkFlowTypeViewModel>();
                }
                else
                {
                    return new Response<ListWorkFlowTypeViewModel>(true, null, null, $"This Workflow Type {WorkflowType.Name} is already exists", (int)Helpers.Constants.ApiReturnCode.fail);
                }
            }
            catch (Exception err)
            {
                
                return new Response<ListWorkFlowTypeViewModel>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }

        }

        public Response<ListWorkFlowTypeViewModel> DeleteWorkflowType(int workflowTypeId)
        {
            try
            {
                var workflowTemplate = GetOneWorkflowTypeById(workflowTypeId);
                workflowTemplate.DateDeleted = DateTime.Now;
                workflowTemplate.Deleted = true;
                _unitOfWork.WorkFlowTypeRepository.Update(workflowTemplate);
                _unitOfWork.SaveChanges();
                return new Response<ListWorkFlowTypeViewModel>();
            }
            catch (Exception err)
            {
                
                return new Response<ListWorkFlowTypeViewModel>(false, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }

        }

        public Response<ListWorkFlowTypeViewModel> UpdateWorkflowType(EditWorkFlowTypeViewModel workflowType)
        {
            try
            {
                if (workflowType.Name == null || workflowType.Name == "")
                {
                    return new Response<ListWorkFlowTypeViewModel>(true, null, null, $"The name of workflow type couldnot be null", (int)Helpers.Constants.ApiReturnCode.fail);
                }
                if (CheckWorkflowTypeName(workflowType.WorkFlowId, workflowType.Name, workflowType.Id))
                {
                    var workflowTemplate = GetOneWorkflowTypeById(workflowType.Id);
                    workflowTemplate.Name = workflowType.Name;
                    workflowTemplate.nextStepActionId = workflowType.nextStepActionId;
                    workflowTemplate.WorkFlowId = workflowType.WorkFlowId;
                    _unitOfWork.WorkFlowTypeRepository.Update(workflowTemplate);
                    _unitOfWork.SaveChanges();
                    return new Response<ListWorkFlowTypeViewModel>();
                }
                else
                {
                    return new Response<ListWorkFlowTypeViewModel>(true, null, null, $"This Workflow Type {workflowType.Name} is already exitsts", (int)Helpers.Constants.ApiReturnCode.fail);
                }
            }
            catch (Exception err)
            {
                
                return new Response<ListWorkFlowTypeViewModel>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }

        }

        private bool CheckWorkflowTypeName(int workFlowId, string Name, int? Id)
        {
            if (Id != null)
            {
                var item = _unitOfWork.WorkFlowTypeRepository.GetAllAsQueryable().Where(x => x.WorkFlowId == workFlowId && x.Name == Name && x.Deleted == false && x.Id != Id).FirstOrDefault();
                if (item != null)
                {
                    return false;
                }
            }
            else
            {
                var item = _unitOfWork.WorkFlowTypeRepository.GetAllAsQueryable().Where(x => x.WorkFlowId == workFlowId && x.Name == Name && x.Deleted == false).FirstOrDefault();
                if (item != null)
                {
                    return false;
                }
            }
            return true;
        }
        //-------------------------------------------  end of WorkFlow Type


        //-------------------------------------------  Step

        /// <summary>
        /// return all steps that their deleted flag is false
        /// </summary>
        public Response<List<StepListViewModel>> GetAllSteps(ParameterPagination parameterPagination, List<FilterObjectList> filter)
        {
            int count = 0;
            try
            {
                var stepTemplate = _unitOfWork.StepListRepository.GetAllIncludeMultiple(parameterPagination, filter, out count); //
                var stepModel = _mapper.Map<List<StepListViewModel>>(stepTemplate);
                return new Response<List<StepListViewModel>>(true, stepModel, null, null, (int)Helpers.Constants.ApiReturnCode.success);
            }
            catch (Exception err)
            {
                
                return new Response<List<StepListViewModel>>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }

        }

        public Response<List<StepListViewModel>> GetWorkFlowSteps(ParameterPagination parameterPagination, List<FilterObjectList> filter, int workflowId)
        {
            int count = 0;
            try
            {
                if (filter == null)
                {
                    filter = new List<FilterObjectList>();
                }
                List<object> filterToWorkFlow = new List<object>();
                filterToWorkFlow.Add("" + workflowId);
                var belongToWorkFlow = new FilterObjectList("WorkFlowId", filterToWorkFlow);
                filter.Add(belongToWorkFlow);
                var stepTemplate = _unitOfWork.StepListRepository.GetAllIncludeMultiple(parameterPagination, filter, out count); //
                var stepModel = _mapper.Map<List<StepListViewModel>>(stepTemplate);
                return new Response<List<StepListViewModel>>(true, stepModel, null, null, (int)Helpers.Constants.ApiReturnCode.success);
            }
            catch (Exception err)
            {
                
                return new Response<List<StepListViewModel>>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }

        }
        public Response<List<StepListViewModel>> GetSubStep(ParameterPagination parameterPagination, List<FilterObjectList> filter, int stepId)
        {
            int count = 0;
            try
            {
                if (filter == null)
                {
                    filter = new List<FilterObjectList>();
                }
                List<object> filterToWorkFlow = new List<object>();
                filterToWorkFlow.Add("" + stepId);
                var belongToWorkFlow = new FilterObjectList("ParentStepId", filterToWorkFlow);
                filter.Add(belongToWorkFlow);
                var stepTemplate = _unitOfWork.StepListRepository.GetAllIncludeMultiple(parameterPagination, filter, out count); //
                var stepModel = _mapper.Map<List<StepListViewModel>>(stepTemplate);
                return new Response<List<StepListViewModel>>(true, stepModel, null, null, (int)Helpers.Constants.ApiReturnCode.success);
            }
            catch (Exception err)
            {
                
                return new Response<List<StepListViewModel>>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }

        }

        public async Task<Response<StepListViewModel>> GetStepById(int Id)
        {
            try
            {
                var stepTemplate = await _unitOfWork.StepListRepository.SingleOrDefaultAsync(x => x.Id == Id && x.Deleted == false);
                var stepModel = _mapper.Map<StepListViewModel>(stepTemplate);
                return new Response<StepListViewModel>(true, stepModel, null, null, (int)Helpers.Constants.ApiReturnCode.success);
            }
            catch (Exception err)
            {
                
                return new Response<StepListViewModel>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
        }

        public async Task<Response<StepEditViewModel>> AddStep(StepAddViewModel step)
        {
            try
            {
                if (CheckStepName(step.Name, null))
                {
                    await _unitOfWork.StepAddRepository.AddModelAsync(step);
                    return new Response<StepEditViewModel>();
                }
                else
                {
                    return new Response<StepEditViewModel>(true, null, null, $"This Workflow {step.Name} is already exists", (int)Helpers.Constants.ApiReturnCode.fail);
                }
            }
            catch (Exception err)
            {
                
                return new Response<StepEditViewModel>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }

        }


        public async Task<Response<StepEditViewModel>> UpdateStep(StepEditViewModel step)
        {
            try
            {
                if (CheckStepName(step.Name, step.Id))
                {
                    await _unitOfWork.StepEditRepository.UpdateItem(step);
                    await _unitOfWork.SaveChangesAsync();
                    return new Response<StepEditViewModel>();
                }
                else
                {
                    return new Response<StepEditViewModel>(true, null, null, $"This Mail Template {step.Name} is already exitsts", (int)Helpers.Constants.ApiReturnCode.fail);
                }
            }
            catch (Exception err)
            {
                
                return new Response<StepEditViewModel>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }

        }

        /// <summary>
        /// check if there is a step already exists on Database by same name
        /// </summary>
        /// <param name="Name">this name must be unique in database</param>
        /// <returns>result will be true in case of this name is not existon database or exist for same id. otherwise, response would be false </returns>
        private bool CheckStepName(string Name, int? Id)
        {
            if (Id != null)
            {
                if (_unitOfWork.StepListRepository.SingleOrDefaultAsync(x => x.Deleted == false && x.Id != Id) != null)
                {
                    return true;
                }
            }
            else
            {
                if (_unitOfWork.StepListRepository.SingleOrDefaultAsync(x => x.Deleted == false) != null)
                {
                    return true;
                }
            }
            return false;
        }
        //-------------------------------------------  end of step


        //-------------------------------------------  Action

        /// <summary>
        /// return all actions that their deleted flag is false
        /// </summary>
        public Response<List<ActionListViewModel>> GetAllActions() //ParameterPagination parameterPagination, List<FilterObjectList> filter
        {
            try
            {
                int count = 0;
                var actionTemplate = (_unitOfWork.ActionRepository.GetAll(out count));//.Where(x => x.Deleted == false)
                var actionModel = _mapper.Map<List<ActionListViewModel>>(actionTemplate);
                foreach (ActionListViewModel al in actionModel)
                {
                    al.ActionOptions = _mapper.Map<List<ListActionOptionViewModel>>(_unitOfWork.OptionRepository.GetAllWithoutCount().Where(x => x.ActionId == al.Id && x.Deleted == false).ToList());
                    al.ActionItemOptions = _mapper.Map<List<ListActionItemOptionViewModel>>(_unitOfWork.ActionItemOptionListRepository.GetAllWithoutCount().Where(x => x.ActionId == al.Id && x.Deleted == false).ToList());
                }
                return new Response<List<ActionListViewModel>>(true, actionModel, null, null, (int)Helpers.Constants.ApiReturnCode.success, count);
            }
            catch (Exception err)
            {
                
                return new Response<List<ActionListViewModel>>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }

        }
        public async Task<Response<ActionListViewModel>> GetActionById(int Id)
        {
            try
            {
                var actionTemplate = await _unitOfWork.ActionRepository.SingleOrDefaultAsync(x => x.Id == Id && x.Deleted == false);
                var actionModel = _mapper.Map<ActionListViewModel>(actionTemplate);
                actionModel.ActionOptions = new List<ListActionOptionViewModel>();
                var actionOptions = _mapper.Map<List<ListActionOptionViewModel>>(_unitOfWork.OptionRepository.GetAllWithoutCount().Where(x => x.ActionId == actionModel.Id && x.Deleted == false).ToList());
                foreach (var a in actionOptions)
                {
                    ListActionOptionViewModel ao = new ListActionOptionViewModel();
                    ao.Id = a.Id;
                    ao.Name = a.Name;
                    ao.ParentId = a.ParentId;
                    ao.ItemStatusId = a.ItemStatusId;
                    ao.SubOptions = getListActionOption(ao.Id);
                    actionModel.ActionOptions.Add(ao);
                }
                actionModel.ActionItemOptions = _mapper.Map<List<ListActionItemOptionViewModel>>(_unitOfWork.ActionItemOptionListRepository.GetAllWithoutCount().Where(x => x.ActionId == actionModel.Id && x.Deleted == false).ToList());
                return new Response<ActionListViewModel>(true, actionModel, null, null, (int)Helpers.Constants.ApiReturnCode.success);
            }
            catch (Exception err)
            {
                
                return new Response<ActionListViewModel>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
        }
        public List<ListActionOptionViewModel> getListActionOption(int ParentId)
        {
            var response = new List<ListActionOptionViewModel>();
            var ActionOptions = _unitOfWork.OptionRepository.GetAllWithoutCount().Where(x => x.ParentId == ParentId && x.Deleted == false).ToList();
            foreach (var a in ActionOptions)
            {
                ListActionOptionViewModel ao = new ListActionOptionViewModel();
                ao.Id = a.Id;
                ao.Name = a.Name;
                ao.ParentId = a.ParentId;
                ao.ItemStatusId = a.ItemStatusId;
                ao.SubOptions = getListActionOption(ao.Id);
                response.Add(ao);
            }
            return response;
        }
        public Response<List<ListConditionActionViewModel>> GetConditionActions()
        {
            try
            {
                var actionModel = _mapper.Map<List<ListConditionActionViewModel>>(_mapper.Map<List<ActionListViewModel>>(_unitOfWork.ActionRepository.GetAllAsQueryable().Where(x => x.Type == ActionType.Condition && x.Deleted == false)));
                foreach (var act in actionModel)
                {
                    if (act.ActionOptions == null)
                    {
                        act.ActionOptions = new List<ListActionOptionViewModel>();
                    }
                    var ActionOptions = _unitOfWork.OptionRepository.GetAllAsQueryable().Where(x => x.ActionId == act.Id && x.Deleted == false).ToList();
                    foreach (var a in ActionOptions)
                    {
                        ListActionOptionViewModel ao = new ListActionOptionViewModel();
                        ao.Id = a.Id;
                        ao.Name = a.Name;
                        ao.ParentId = a.ParentId;
                        ao.ItemStatusId = a.ItemStatusId;
                        ao.SubOptions = getListActionOption(ao.Id);
                        act.ActionOptions.Add(ao);
                    }
                }
                return new Response<List<ListConditionActionViewModel>>(true, actionModel, null, null, (int)Helpers.Constants.ApiReturnCode.success);

            }
            catch (Exception err)
            {
                
                return new Response<List<ListConditionActionViewModel>>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
        }
        public Response<List<ListConditionActionViewModel>> GetTelecomValidationActions()
        {
            try
            {
                var actionModel = _mapper.Map<List<ListConditionActionViewModel>>(_mapper.Map<List<ActionListViewModel>>(_unitOfWork.ActionRepository.GetAllAsQueryable().Where(x => x.Type == ActionType.TelecomValidation && x.Deleted == false)));
                foreach (var act in actionModel)
                {
                    if (act.ActionOptions == null)
                    {
                        act.ActionOptions = new List<ListActionOptionViewModel>();
                    }
                    var ActionOptions = _unitOfWork.OptionRepository.GetAllAsQueryable().Where(x => x.ActionId == act.Id && x.Deleted == false).ToList();
                    foreach (var a in ActionOptions)
                    {
                        ListActionOptionViewModel ao = new ListActionOptionViewModel();
                        ao.Id = a.Id;
                        ao.Name = a.Name;
                        ao.ParentId = a.ParentId;
                        ao.ItemStatusId = a.ItemStatusId;
                        ao.SubOptions = getListActionOption(ao.Id);
                        act.ActionOptions.Add(ao);
                    }
                }
                return new Response<List<ListConditionActionViewModel>>(true, actionModel, null, null, (int)Helpers.Constants.ApiReturnCode.success);

            }
            catch (Exception err)
            {
                
                return new Response<List<ListConditionActionViewModel>>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
        }
        public Response<List<ListConditionActionViewModel>> GetCivilValidationActions()
        {
            try
            {
                var actionModel = _mapper.Map<List<ListConditionActionViewModel>>(_mapper.Map<List<ActionListViewModel>>(_unitOfWork.ActionRepository.GetAllAsQueryable().Where(x => x.Type == ActionType.CivilValidation && x.Deleted == false)));
                foreach (var act in actionModel)
                {
                    if (act.ActionOptions == null)
                    {
                        act.ActionOptions = new List<ListActionOptionViewModel>();
                    }
                    var ActionOptions = _unitOfWork.OptionRepository.GetAllAsQueryable().Where(x => x.ActionId == act.Id && x.Deleted == false).ToList();
                    foreach (var a in ActionOptions)
                    {
                        ListActionOptionViewModel ao = new ListActionOptionViewModel();
                        ao.Id = a.Id;
                        ao.Name = a.Name;
                        ao.ParentId = a.ParentId;
                        ao.ItemStatusId = a.ItemStatusId;
                        ao.SubOptions = getListActionOption(ao.Id);
                        act.ActionOptions.Add(ao);
                    }
                }
                return new Response<List<ListConditionActionViewModel>>(true, actionModel, null, null, (int)Helpers.Constants.ApiReturnCode.success);

            }
            catch (Exception err)
            {
                
                return new Response<List<ListConditionActionViewModel>>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
        }
        public Response<List<ListCivilDecisionActionViewModel>> GetCivilDecisionActions()
        {
            try
            {
                var actionModel = _mapper.Map<List<ListCivilDecisionActionViewModel>>(_mapper.Map<List<ActionListViewModel>>(_unitOfWork.ActionRepository.GetAllAsQueryable().Where(x => x.Type == ActionType.CivilDecision && x.Deleted == false)));
                foreach (var act in actionModel)
                {
                    if (act.ActionOptions == null)
                    {
                        act.ActionOptions = new List<ListActionOptionViewModel>();
                    }
                    var ActionOptions = _unitOfWork.OptionRepository.GetAllAsQueryable().Where(x => x.ActionId == act.Id && x.Deleted == false).ToList();
                    foreach (var a in ActionOptions)
                    {
                        ListActionOptionViewModel ao = new ListActionOptionViewModel();
                        ao.Id = a.Id;
                        ao.Name = a.Name;
                        ao.ParentId = a.ParentId;
                        ao.ItemStatusId = a.ItemStatusId;
                        ao.SubOptions = getListActionOption(ao.Id);
                        act.ActionOptions.Add(ao);
                    }
                    if (act.ActionItemOptions == null)
                    {
                        act.ActionItemOptions = new List<ActionItemOptionListViewModel>();
                    }
                    var ActionItemOptions = _unitOfWork.ActionItemOptionListRepository.GetAllAsQueryable().Where(x => x.ActionId == act.Id && x.Deleted == false).ToList();
                    foreach (var a in ActionItemOptions)
                    {
                        ActionItemOptionListViewModel ao = new ActionItemOptionListViewModel();
                        ao.Id = a.Id;
                        ao.Name = a.Name;
                        ao.ActionId = a.ActionId;
                        act.ActionItemOptions.Add(ao);
                    }
                }
                return new Response<List<ListCivilDecisionActionViewModel>>(true, actionModel, null, null, (int)Helpers.Constants.ApiReturnCode.success);

            }
            catch (Exception err)
            {
                
                return new Response<List<ListCivilDecisionActionViewModel>>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
        }
        public Response<ListCivilDecisionActionViewModel> GetCivilDecisionAction()
        {
            try
            {
                var act = _mapper.Map<List<ListCivilDecisionActionViewModel>>(_mapper.Map<List<ActionListViewModel>>(_unitOfWork.ActionRepository.GetAllAsQueryable().Where(x => x.Type == ActionType.CivilDecision && x.Deleted == false))).FirstOrDefault();
                if (act!=null)
                {
                    if (act.ActionOptions == null)
                    {
                        act.ActionOptions = new List<ListActionOptionViewModel>();
                    }
                    var ActionOptions = _unitOfWork.OptionRepository.GetAllAsQueryable().Where(x => x.ActionId == act.Id && x.Deleted == false).ToList();
                    foreach (var a in ActionOptions)
                    {
                        ListActionOptionViewModel ao = new ListActionOptionViewModel();
                        ao.Id = a.Id;
                        ao.Name = a.Name;
                        ao.ParentId = a.ParentId;
                        ao.ItemStatusId = a.ItemStatusId;
                        ao.SubOptions = getListActionOption(ao.Id);
                        act.ActionOptions.Add(ao);
                    }
                    if (act.ActionItemOptions == null)
                    {
                        act.ActionItemOptions = new List<ActionItemOptionListViewModel>();
                    }
                    var ActionItemOptions = _unitOfWork.ActionItemOptionListRepository.GetAllAsQueryable().Where(x => x.ActionId == act.Id && x.Deleted == false).ToList();
                    foreach (var a in ActionItemOptions)
                    {
                        ActionItemOptionListViewModel ao = new ActionItemOptionListViewModel();
                        ao.Id = a.Id;
                        ao.Name = a.Name;
                        ao.ActionId = a.ActionId;
                        //ao.ItemStatusId = a.ItemStatusId;
                        act.ActionItemOptions.Add(ao);
                    }
                }
                return new Response<ListCivilDecisionActionViewModel>(true, act, null, null, (int)Helpers.Constants.ApiReturnCode.success);

            }
            catch (Exception err)
            {
                
                return new Response<ListCivilDecisionActionViewModel>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
        }
        public Response<List<ListActionOptionViewModel>> GetAvailableSpaceOptions()
        {
            try
            {
                var act = _mapper.Map<List<ListConditionActionViewModel>>(_mapper.Map<List<ActionListViewModel>>(_unitOfWork.ActionRepository.GetAllAsQueryable().Where(x => x.Type == ActionType.CheckAvailableSpace && x.Deleted == false))).FirstOrDefault();
                if (act == null) {
                    return new Response<List<ListActionOptionViewModel>>(true, null, null, "this is no options", (int)Helpers.Constants.ApiReturnCode.fail);
                }
                if (act.ActionOptions == null)
                {
                    act.ActionOptions = new List<ListActionOptionViewModel>();
                }
                var ActionOptions = _unitOfWork.OptionRepository.GetAllAsQueryable().Where(x => x.ActionId == act.Id && x.Deleted == false).ToList();
                foreach (var a in ActionOptions)
                {
                    ListActionOptionViewModel ao = new ListActionOptionViewModel();
                    ao.Id = a.Id;
                    ao.Name = a.Name;
                    ao.ParentId = a.ParentId;
                    ao.ItemStatusId = a.ItemStatusId;
                    ao.SubOptions = getListActionOption(ao.Id);
                    act.ActionOptions.Add(ao);
                }
                return new Response<List<ListActionOptionViewModel>>(true, act.ActionOptions, null, null, (int)Helpers.Constants.ApiReturnCode.success);

            }
            catch (Exception err)
            {
                
                return new Response<List<ListActionOptionViewModel>>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
        }
        //-------------------------------------------  end of Action

        //-------------------------------------------  StepAction

        /// <summary>
        /// return all StepActions that their deleted flag is false
        /// </summary>
        public Response<List<ListStepActionViewModel>> GetAllStepActions() //ParameterPagination parameterPagination, List<FilterObjectList> filter
        {
            try
            {
                int count = 0;
                var actionTemplate = (_unitOfWork.StepActionRepository.GetAllAsQueryable(out count));//.Where(x => x.Deleted == false)
                var actionModel = _mapper.Map<List<ListStepActionViewModel>>(actionTemplate);
                return new Response<List<ListStepActionViewModel>>(true, actionModel, null, null, (int)Helpers.Constants.ApiReturnCode.success, count);
            }
            catch (Exception err)
            {
                
                return new Response<List<ListStepActionViewModel>>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }

        }
        public Response<List<ListStepActionViewModel>> GetWorkFlowStepActions(int workFlowId) //ParameterPagination parameterPagination, List<FilterObjectList> filter
        {
            try
            {
                var actionModel = _mapper.Map<List<ListStepActionViewModel>>(_unitOfWork.StepActionRepository.GetAllAsQueryable().Where(x => x.WorkflowId == workFlowId && x.Deleted == false));
                return new Response<List<ListStepActionViewModel>>(true, actionModel, null, null, (int)Helpers.Constants.ApiReturnCode.success, actionModel.Count);
            }
            catch (Exception err)
            {
                
                return new Response<List<ListStepActionViewModel>>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }

        }
        public Response<StepActionWithNamesViewModel> GetStepActionById(int Id)
        {
            try
            {
                return new Response<StepActionWithNamesViewModel>(true,StepActionWithNames(GetOneStepAction(Id)), null, null, (int)Helpers.Constants.ApiReturnCode.success);
            }
            catch (Exception err)
            {
                
                return new Response<StepActionWithNamesViewModel>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
        }

        public TLIstepAction GetOneStepActionById(int Id)
        {
            return _unitOfWork.StepActionRepository.GetAllAsQueryable().Where(x => x.Id == Id && x.Deleted == false).SingleOrDefault();
        }

        public ListStepActionViewModel GetOneStepAction(int Id)
        {
            return MapStepAction(GetOneStepActionById(Id));
        }
        public Response<StepActionWithNamesViewModel> AddTelecomValidationStepAction(AddStepActionTelecomValidationViewModel action)
        {
            try
            {
                AddStepActionViewModel step = new AddStepActionViewModel();
                step.ActionId = (int)ActionType.TelecomValidation;
                step.type = ActionType.TelecomValidation;
                step.IsStepActionMail = false;
                step.CalculateLandSpace = false;
                step.CalculateLoadSpace = false;
                step.Period = action.Period;
                step.label = action.label;
                step.WorkflowId = action.WorkflowId;
                step.AllowUploadFile = false;
                step.UploadFileIsMandatory = false;
                step.StepActionItemOption = _mapper.Map<List<AddStepActionItemOptionViewModel>>(action.StepActionItemOption);
                step.StepActionGroup = action.StepActionGroup;
                step.StepActionPart = _mapper.Map<List<AddStepActionPartViewModel>>(action.StepActionPart);
                return AddStepAction(step);
            }
            catch (Exception err)
            {
                
                return new Response<StepActionWithNamesViewModel>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }

        }
        public Response<StepActionWithNamesViewModel> EditTelecomValidationStepAction(EditStepActionTelecomValidationViewModel action)
        {
            try
            {
                EditStepActionViewModel step = new EditStepActionViewModel();
                step.ActionId = (int)ActionType.TelecomValidation;
                step.type = ActionType.TelecomValidation;
                step.IsStepActionMail = false;
                step.CalculateLandSpace = false;
                step.CalculateLoadSpace = false;
                step.Period = action.Period;
                step.label = action.label;
                step.Id = action.Id;
                step.AllowUploadFile = false;
                step.UploadFileIsMandatory = false;
                step.StepActionItemOption = _mapper.Map<List<AddStepActionItemOptionViewModel>>(action.StepActionItemOption);
                step.StepActionGroup = action.StepActionGroup;
                step.StepActionPart = _mapper.Map<List<AddStepActionPartViewModel>>(action.StepActionPart);
                return UpdateStepAction(step);
            }
            catch (Exception err)
            {
                
                return new Response<StepActionWithNamesViewModel>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }

        }
        public Response<StepActionWithNamesViewModel> AddCivilValidationStepAction(AddStepActionTelecomValidationViewModel action)
        {
            try
            {
                AddStepActionViewModel step = new AddStepActionViewModel();
                step.ActionId = (int)ActionType.CivilValidation;
                step.type = ActionType.CivilValidation;
                step.IsStepActionMail = false;
                step.CalculateLandSpace = false;
                step.CalculateLoadSpace = false;
                step.Period = action.Period;
                step.label = action.label;
                step.WorkflowId = action.WorkflowId;
                step.AllowUploadFile = false;
                step.UploadFileIsMandatory = false;
                step.StepActionItemOption = _mapper.Map<List<AddStepActionItemOptionViewModel>>(action.StepActionItemOption);
                step.StepActionGroup = action.StepActionGroup;
                //step.NextStepActions = action.NextStepActions;
                step.IncomItemStatus =_mapper.Map<List<ListItemStatusViewModel>>(action.IncomItemStatus);
                step.StepActionPart = _mapper.Map<List<AddStepActionPartViewModel>>(action.StepActionPart);
                return AddStepAction(step);
            }
            catch (Exception err)
            {
                
                return new Response<StepActionWithNamesViewModel>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }

        }
        public Response<StepActionWithNamesViewModel> EditCivilValidationStepAction(EditStepActionTelecomValidationViewModel action)
        {
            try
            {
                EditStepActionViewModel step = new EditStepActionViewModel();
                step.ActionId = (int)ActionType.CivilValidation;
                step.type = ActionType.CivilValidation;
                step.IsStepActionMail = false;
                step.CalculateLandSpace = false;
                step.CalculateLoadSpace = false;
                step.Period = action.Period;
                step.label = action.label;
                step.Id = action.Id;
                step.AllowUploadFile = false;
                step.UploadFileIsMandatory = false;
                step.StepActionItemOption = _mapper.Map<List<AddStepActionItemOptionViewModel>>(action.StepActionItemOption);
                step.StepActionGroup = action.StepActionGroup;
                //step.NextStepActions = action.NextStepActions;
                step.StepActionPart = _mapper.Map<List<AddStepActionPartViewModel>>(action.StepActionPart);
                return UpdateStepAction(step);
            }
            catch (Exception err)
            {
                
                return new Response<StepActionWithNamesViewModel>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }

        }

        public Response<StepActionWithNamesViewModel> AddStudyResultStepAction(AddStepActionProposalApprovedViewModel action)
        {
            try
            {
                AddStepActionViewModel step = new AddStepActionViewModel();
                step.ActionId = (int)ActionType.StudyResult;
                step.type = ActionType.StudyResult;
                step.IsStepActionMail = false;
                step.CalculateLandSpace = false;
                step.CalculateLoadSpace = false;
                step.Period = action.Period;
                step.label = action.label;
                step.WorkflowId = action.WorkflowId;
                step.AllowUploadFile = false;
                step.UploadFileIsMandatory = false;
                //step.IncomItemStatus = action.IncomItemStatus;
                step.IncomItemStatus = _mapper.Map<List<ListItemStatusViewModel>>(action.IncomItemStatus);
                step.StepActionGroup = action.StepActionGroup;
                //step.NextStepActions = action.NextStepActions;
                return AddStepAction(step);
            }
            catch (Exception err)
            {
                
                return new Response<StepActionWithNamesViewModel>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }

        }
        public Response<StepActionWithNamesViewModel> EditStudyResultStepAction(EditStepActionProposalApprovedViewModel action)
        {
            try
            {
                EditStepActionViewModel step = new EditStepActionViewModel();
                step.ActionId = (int)ActionType.StudyResult;
                step.type = ActionType.StudyResult;
                step.IsStepActionMail = false;
                step.CalculateLandSpace = false;
                step.CalculateLoadSpace = false;
                step.Period = action.Period;
                step.label = action.label;
                step.Id = action.Id;
                step.AllowUploadFile = false;
                step.UploadFileIsMandatory = false;
                //step.IncomItemStatus = action.IncomItemStatus;
                step.IncomItemStatus = _mapper.Map<List<ListItemStatusViewModel>>(action.IncomItemStatus);
                step.StepActionGroup = action.StepActionGroup;
                //step.NextStepActions = action.NextStepActions;
                return UpdateStepAction(step);
            }
            catch (Exception err)
            {
                
                return new Response<StepActionWithNamesViewModel>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }

        }
        public Response<StepActionWithNamesViewModel> AddProposalApprovedStepAction(AddStepActionProposalApprovedViewModel action)
        {
            try
            {
                AddStepActionViewModel step = new AddStepActionViewModel();
                step.ActionId = (int)ActionType.ProposalApproved;
                step.type = ActionType.ProposalApproved;
                step.IsStepActionMail = false;
                step.CalculateLandSpace = false;
                step.CalculateLoadSpace = false;
                step.Period = action.Period;
                step.label = action.label;
                step.WorkflowId = action.WorkflowId;
                step.AllowUploadFile = false;
                step.UploadFileIsMandatory = false;
                //step.IncomItemStatus = action.IncomItemStatus;
                step.IncomItemStatus = _mapper.Map<List<ListItemStatusViewModel>>(action.IncomItemStatus);
                step.StepActionGroup = action.StepActionGroup;
                //step.NextStepActions = action.NextStepActions;
                return AddStepAction(step);
            }
            catch (Exception err)
            {
                
                return new Response<StepActionWithNamesViewModel>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }

        }
        public Response<StepActionWithNamesViewModel> EditProposalApprovedStepAction(EditStepActionProposalApprovedViewModel action)
        {
            try
            {
                EditStepActionViewModel step = new EditStepActionViewModel();
                step.ActionId = (int)ActionType.ProposalApproved;
                step.type = ActionType.ProposalApproved;
                step.IsStepActionMail = false;
                step.CalculateLandSpace = false;
                step.CalculateLoadSpace = false;
                step.Period = action.Period;
                step.label = action.label;
                step.Id = action.Id;
                step.AllowUploadFile = false;
                step.UploadFileIsMandatory = false;
                //step.IncomItemStatus = action.IncomItemStatus;
                step.IncomItemStatus = _mapper.Map<List<ListItemStatusViewModel>>(action.IncomItemStatus);
                step.StepActionGroup = action.StepActionGroup;
                //step.NextStepActions = action.NextStepActions;
                return UpdateStepAction(step);
            }
            catch (Exception err)
            {
                
                return new Response<StepActionWithNamesViewModel>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }

        }
        public Response<StepActionWithNamesViewModel> AddCivilDecisionStepAction(AddStepActionCivilDecisionViewModel action)
        {
            try
            {
                AddStepActionViewModel step = new AddStepActionViewModel();
                step.ActionId = (int)ActionType.CivilDecision;
                step.type = ActionType.CivilDecision;
                step.IsStepActionMail = false;
                step.CalculateLandSpace = false;
                step.CalculateLoadSpace = false;
                step.Period = action.Period;
                step.label = action.label;
                step.WorkflowId = action.WorkflowId;
                step.AllowUploadFile = false;
                step.UploadFileIsMandatory = false;
                step.IncomItemStatus = _mapper.Map<List<ListItemStatusViewModel>>(action.IncomItemStatus);
                //step.IncomItemStatus = action.IncomItemStatus;
                step.StepActionGroup = action.StepActionGroup;
                //step.NextStepActions = action.NextStepActions;
                //step.ItemStatus = action.StepActionItemStatus;
                step.StepActionOption = _mapper.Map<List<ListStepActionOptionViewModel>>(_mapper.Map<List<AddStepActionOptionViewModel>>(action.StepActionOption));
                step.StepActionItemOption = _mapper.Map<List<AddStepActionItemOptionViewModel>>(action.StepActionItemOption);
                return AddStepAction(step);
            }
            catch (Exception err)
            {
                
                return new Response<StepActionWithNamesViewModel>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }

        }
        public Response<StepActionWithNamesViewModel> EditCivilDecisionStepAction(EditStepActionCivilDecisionViewModel action)
        {
            try
            {
                EditStepActionViewModel step = new EditStepActionViewModel();
                step.ActionId = (int)ActionType.CivilDecision;
                step.type = ActionType.CivilDecision;
                step.IsStepActionMail = false;
                step.CalculateLandSpace = false;
                step.CalculateLoadSpace = false;
                step.Period = action.Period;
                step.label = action.label;
                step.Id = action.Id;
                step.AllowUploadFile = false;
                step.UploadFileIsMandatory = false;
                //step.IncomItemStatus = action.IncomItemStatus;
                step.IncomItemStatus = _mapper.Map<List<ListItemStatusViewModel>>(action.IncomItemStatus);
                step.StepActionGroup = action.StepActionGroup;
                //step.NextStepActions = action.NextStepActions;
                //step.ItemStatus = action.StepActionItemStatus;
                step.StepActionOption = _mapper.Map<List<AddStepActionOptionViewModel>>(action.StepActionOption);
                step.StepActionItemOption = _mapper.Map<List<AddStepActionItemOptionViewModel>>(action.StepActionItemOption);
                return UpdateStepAction(step);
            }
            catch (Exception err)
            {
                
                return new Response<StepActionWithNamesViewModel>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }

        }
        public Response<StepActionWithNamesViewModel> AddSelectTargetSupportStepAction(AddStepActionSelectTargetSupportViewModel action)
        {
            try
            {
                AddStepActionViewModel step = new AddStepActionViewModel();
                step.ActionId = (int)ActionType.SelectTargetSupport;
                step.type = ActionType.SelectTargetSupport;
                step.IsStepActionMail = false;
                step.CalculateLandSpace = false;
                step.CalculateLoadSpace = false;
                step.Period = action.Period;
                step.label = action.label;
                step.WorkflowId = action.WorkflowId;
                step.AllowUploadFile = false;
                step.UploadFileIsMandatory = false;
                step.StepActionGroup = action.StepActionGroup;
                step.NextStepActions = action.NextStepActions;
                return AddStepAction(step);
            }
            catch (Exception err)
            {
                
                return new Response<StepActionWithNamesViewModel>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }

        }
        public Response<StepActionWithNamesViewModel> EditSelectTargetSupportStepAction(EditStepActionSelectTargetSupportViewModel action)
        {
            try
            {
                EditStepActionViewModel step = new EditStepActionViewModel();
                step.ActionId = (int)ActionType.SelectTargetSupport;
                step.type = ActionType.SelectTargetSupport;
                step.IsStepActionMail = false;
                step.CalculateLandSpace = false;
                step.CalculateLoadSpace = false;
                step.Period = action.Period;
                step.label = action.label;
                step.Id = action.Id;
                step.AllowUploadFile = false;
                step.UploadFileIsMandatory = false;
                step.StepActionGroup = action.StepActionGroup;
                step.NextStepActions = action.NextStepActions;
                return UpdateStepAction(step);
            }
            catch (Exception err)
            {
                
                return new Response<StepActionWithNamesViewModel>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }

        }
        public Response<StepActionWithNamesViewModel> AddCheckAvailableSpaceStepAction(AddStepActionCheckAvailableSpaceViewModel action)
        {
            try
            {
                AddStepActionViewModel step = new AddStepActionViewModel();
                step.ActionId = (int)ActionType.CheckAvailableSpace;
                step.type = ActionType.CheckAvailableSpace;
                step.IsStepActionMail = false;
                step.CalculateLandSpace = false;
                step.CalculateLoadSpace = false;
                step.Period = action.Period;
                step.label = action.label;
                step.WorkflowId = action.WorkflowId;
                step.AllowUploadFile = false;
                step.UploadFileIsMandatory = false;
                step.StepActionOption = _mapper.Map<List<ListStepActionOptionViewModel>>(_mapper.Map<List<AddStepActionOptionViewModel>>(action.StepActionOption));
                //step.StepActionGroup = action.StepActionGroup;
                //step.NextStepActions = action.NextStepActions;
                return AddStepAction(step);
            }
            catch (Exception err)
            {
                
                return new Response<StepActionWithNamesViewModel>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }

        }
        public Response<StepActionWithNamesViewModel> EditCheckAvailableSpaceStepAction(EditStepActionCheckAvailableSpaceViewModel action)
        {
            try
            {
                EditStepActionViewModel step = new EditStepActionViewModel();
                step.ActionId = (int)ActionType.CheckAvailableSpace;
                step.type = ActionType.CheckAvailableSpace;
                step.IsStepActionMail = false;
                step.CalculateLandSpace = false;
                step.CalculateLoadSpace = false;
                step.Period = action.Period;
                step.label = action.label;
                step.Id = action.Id;
                step.AllowUploadFile = false;
                step.UploadFileIsMandatory = false;
                step.StepActionOption =_mapper.Map<List<AddStepActionOptionViewModel>>(action.StepActionOption);
                //step.StepActionGroup = action.StepActionGroup;
                //step.NextStepActions = action.NextStepActions;
                return UpdateStepAction(step);
            }
            catch (Exception err)
            {
                
                return new Response<StepActionWithNamesViewModel>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }

        }
        public Response<StepActionWithNamesViewModel> AddConditionStepAction(AddStepActionConditionViewModel action)
        {
            try
            {
                AddStepActionViewModel step = new AddStepActionViewModel();
                step.ActionId = (int)ActionType.Condition;
                step.type = ActionType.Condition;
                step.IsStepActionMail = false;
                step.CalculateLandSpace = false;
                step.CalculateLoadSpace = false;
                step.Period = action.Period;
                step.label = action.label;
                step.WorkflowId = action.WorkflowId;
                step.AllowUploadFile = false;
                step.UploadFileIsMandatory = false;
                step.StepActionOption = _mapper.Map<List<ListStepActionOptionViewModel>>(_mapper.Map<List<AddStepActionOptionViewModel>>(action.StepActionOption));
                step.StepActionGroup = action.StepActionGroup;
                return AddStepAction(step);
            }
            catch (Exception err)
            {
                
                return new Response<StepActionWithNamesViewModel>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }

        }
        public Response<StepActionWithNamesViewModel> EditConditionStepAction(EditStepActionConditionViewModel action)
        {
            try
            {
                EditStepActionViewModel step = new EditStepActionViewModel();
                step.ActionId = (int)ActionType.Condition;
                step.type = ActionType.Condition;
                step.IsStepActionMail = false;
                step.CalculateLandSpace = false;
                step.CalculateLoadSpace = false;
                step.Period = action.Period;
                step.label = action.label;
                step.Id = action.Id;
                step.AllowUploadFile = false;
                step.UploadFileIsMandatory = false;
                step.StepActionOption = _mapper.Map<List<AddStepActionOptionViewModel>>(action.StepActionOption);
                step.StepActionGroup = action.StepActionGroup;
                return UpdateStepAction(step);
            }
            catch (Exception err)
            {
                
                return new Response<StepActionWithNamesViewModel>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }

        }
        public Response<StepActionWithNamesViewModel> AddUpdateDataStepAction(AddStepActionInsertDataViewModel action)
        {
            try
            {
                AddStepActionViewModel step = new AddStepActionViewModel();
                step.ActionId = (int)ActionType.UpdateData;
                step.type = ActionType.UpdateData;
                step.IsStepActionMail = false;
                step.CalculateLandSpace = false;
                step.CalculateLoadSpace = false;
                step.Period = action.Period;
                step.label = action.label;
                step.Operation = action.Operation;
                step.WorkflowId = action.WorkflowId;
                //step.IncomItemStatus = action.IncomItemStatus;
                step.IncomItemStatus = _mapper.Map<List<ListItemStatusViewModel>>(action.IncomItemStatus);
                step.AllowUploadFile = false; // action.AllowUploadFile;
                step.UploadFileIsMandatory = false; // action.UploadFileIsMandatory;
                //step.StepActionFileGroup = action.StepActionFileGroup;
                step.StepActionGroup = action.StepActionGroup;
                step.StepActionPart = action.StepActionPart;
                step.NextStepActions = action.NextStepActions;
                return AddStepAction(step);
            }
            catch (Exception err)
            {
                
                return new Response<StepActionWithNamesViewModel>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }

        }
        public Response<StepActionWithNamesViewModel> EditUpdateDataStepAction(EditStepActionInsertDataViewModel action)
        {
            try
            {
                EditStepActionViewModel step = new EditStepActionViewModel();
                step.ActionId = (int)ActionType.UpdateData;
                step.type = ActionType.UpdateData;
                step.IsStepActionMail = false;
                step.CalculateLandSpace = false;
                step.CalculateLoadSpace = false;
                step.Period = action.Period;
                step.label = action.label;
                step.Operation = action.Operation;
                step.Id = action.Id;
                //step.IncomItemStatus = action.IncomItemStatus;
                step.IncomItemStatus = _mapper.Map<List<ListItemStatusViewModel>>(action.IncomItemStatus);
                step.AllowUploadFile = false; // action.AllowUploadFile;
                step.UploadFileIsMandatory = false; // action.UploadFileIsMandatory;
                //step.StepActionFileGroup = action.StepActionFileGroup;
                step.StepActionGroup = action.StepActionGroup;
                step.StepActionPart = action.StepActionPart;
                step.NextStepActions = action.NextStepActions;
                return UpdateStepAction(step);
            }
            catch (Exception err)
            {
                
                return new Response<StepActionWithNamesViewModel>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }

        }
        public Response<StepActionWithNamesViewModel> AddCorrectDataStepAction(AddStepActionInsertDataViewModel action)
        {
            try
            {
                AddStepActionViewModel step = new AddStepActionViewModel();
                step.ActionId = (int)ActionType.Correction;
                step.type = ActionType.Correction;
                step.IsStepActionMail = false;
                step.CalculateLandSpace = false;
                step.CalculateLoadSpace = false;
                step.Period = action.Period;
                step.label = action.label;
                step.Operation = action.Operation;
                step.WorkflowId = action.WorkflowId;
                //step.IncomItemStatus = action.IncomItemStatus;
                step.IncomItemStatus = _mapper.Map<List<ListItemStatusViewModel>>(action.IncomItemStatus);
                step.AllowUploadFile = false; // action.AllowUploadFile;
                step.UploadFileIsMandatory = false; // action.UploadFileIsMandatory;
                //step.StepActionFileGroup = action.StepActionFileGroup;
                step.StepActionGroup = action.StepActionGroup;
                step.StepActionPart = action.StepActionPart;
                step.NextStepActions = action.NextStepActions;
                return AddStepAction(step);
            }
            catch (Exception err)
            {
                
                return new Response<StepActionWithNamesViewModel>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }

        }
        public Response<StepActionWithNamesViewModel> EditCorrectDataStepAction(EditStepActionInsertDataViewModel action)
        {
            try
            {
                EditStepActionViewModel step = new EditStepActionViewModel();
                step.ActionId = (int)ActionType.Correction;
                step.type = ActionType.Correction;
                step.IsStepActionMail = false;
                step.CalculateLandSpace = false;
                step.CalculateLoadSpace = false;
                step.Period = action.Period;
                step.label = action.label;
                step.Operation = action.Operation;
                step.Id = action.Id;
                //step.IncomItemStatus = action.IncomItemStatus;
                step.IncomItemStatus = _mapper.Map<List<ListItemStatusViewModel>>(action.IncomItemStatus);
                step.AllowUploadFile = false; // action.AllowUploadFile;
                step.UploadFileIsMandatory = false; // action.UploadFileIsMandatory;
                //step.StepActionFileGroup = action.StepActionFileGroup;
                step.StepActionGroup = action.StepActionGroup;
                step.StepActionPart = action.StepActionPart;
                step.NextStepActions = action.NextStepActions;
                return UpdateStepAction(step);
            }
            catch (Exception err)
            {
                
                return new Response<StepActionWithNamesViewModel>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }

        }
        public Response<StepActionWithNamesViewModel> AddInsertDataStepAction(AddStepActionInsertDataViewModel action)
        {
            try
            {
                AddStepActionViewModel step = new AddStepActionViewModel();
                step.ActionId = (int)ActionType.InsertData;
                step.type = ActionType.InsertData;
                step.IsStepActionMail = false;
                step.CalculateLandSpace = false;
                step.CalculateLoadSpace = false;
                step.Period = action.Period;
                step.label = action.label;
                step.Operation = action.Operation;
                step.WorkflowId = action.WorkflowId;
                //step.IncomItemStatus = action.IncomItemStatus;
                step.IncomItemStatus = _mapper.Map<List<ListItemStatusViewModel>>(action.IncomItemStatus);
                step.AllowUploadFile = false;// action.AllowUploadFile;
                step.UploadFileIsMandatory = false; // action.UploadFileIsMandatory;
                //step.StepActionFileGroup = action.StepActionFileGroup;
                step.StepActionGroup = action.StepActionGroup;
                step.StepActionPart = action.StepActionPart;
                step.NextStepActions = action.NextStepActions;
                return AddStepAction(step);
            }
            catch (Exception err)
            {
                
                return new Response<StepActionWithNamesViewModel>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }

        }
        public Response<StepActionWithNamesViewModel> EditInsertDataStepAction(EditStepActionInsertDataViewModel action)
        {
            try
            {
                EditStepActionViewModel step = new EditStepActionViewModel();
                step.ActionId = (int)ActionType.InsertData;
                step.type = ActionType.InsertData;
                step.IsStepActionMail = false;
                step.CalculateLandSpace = false;
                step.CalculateLoadSpace = false;
                step.Period = action.Period;
                step.label = action.label;
                step.Operation = action.Operation;
                step.Id = action.Id;
                //step.IncomItemStatus = action.IncomItemStatus;
                step.IncomItemStatus = _mapper.Map<List<ListItemStatusViewModel>>(action.IncomItemStatus);
                step.AllowUploadFile = false;// action.AllowUploadFile;
                step.UploadFileIsMandatory = false; // action.UploadFileIsMandatory;
                //step.StepActionFileGroup = action.StepActionFileGroup;
                step.StepActionGroup = action.StepActionGroup;
                step.StepActionPart =action.StepActionPart;
                step.NextStepActions = action.NextStepActions;
                return UpdateStepAction(step);
            }
            catch (Exception err)
            {
                
                return new Response<StepActionWithNamesViewModel>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }

        }
        public Response<StepActionWithNamesViewModel> AddUploadFileStepAction(AddUploadFileStepActionViewModel action)
        {
            try
            {
                AddStepActionViewModel step = new AddStepActionViewModel();
                step.ActionId = (int)ActionType.UploadFile;
                step.type = ActionType.UploadFile;
                step.IsStepActionMail = false;
                step.CalculateLandSpace = false;
                step.CalculateLoadSpace = false;
                step.Period = action.Period;
                step.label = action.label;
                step.WorkflowId = action.WorkflowId;
                step.AllowUploadFile = true;
                step.UploadFileIsMandatory = false;
                //step.StepActionFileGroup = action.StepActionFileGroup;
                step.StepActionGroup = action.StepActionGroup;
                step.NextStepActions = action.NextStepActions;
                step.StepActionPart = _mapper.Map<List<AddStepActionPartViewModel>>(action.StepActionPart);
                return AddStepAction(step);
            }
            catch (Exception err)
            {
                
                return new Response<StepActionWithNamesViewModel>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }

        }
        public Response<StepActionWithNamesViewModel> EditTicketStatusStepAction(EditStepActionTicketStatusViewModel action)
        {
            try
            {
                EditStepActionViewModel step = new EditStepActionViewModel();
                step.ActionId = (int)ActionType.TicketStatus;
                step.type = ActionType.TicketStatus;
                step.IsStepActionMail = false;
                step.CalculateLandSpace = false;
                step.CalculateLoadSpace = false;
                step.Period = action.Period;
                step.label = action.label;
                step.OrderStatusId = action.OrderStatusId;
                step.Id = action.Id;
                step.NextStepActions = action.NextStepActions;
                return UpdateStepAction(step);
            }
            catch (Exception err)
            {
                
                return new Response<StepActionWithNamesViewModel>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }

        }
        public Response<StepActionWithNamesViewModel> AddTicketStatusStepAction(AddTicketStatusStepActionViewModel action)
        {
            try
            {
                AddStepActionViewModel step = new AddStepActionViewModel();
                step.ActionId = (int)ActionType.TicketStatus;
                step.type = ActionType.TicketStatus;
                step.IsStepActionMail = false;
                step.CalculateLandSpace = false;
                step.CalculateLoadSpace = false;
                step.Period = action.Period;
                step.label = action.label;
                step.WorkflowId = action.WorkflowId;
                step.OrderStatusId = action.OrderStatusId;
                step.NextStepActions = action.NextStepActions;
                return AddStepAction(step);
            }
            catch (Exception err)
            {
                
                return new Response<StepActionWithNamesViewModel>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }

        }
        public Response<StepActionWithNamesViewModel> AddStepActionApplyCalculation(AddStepActionApplyCalculationViewModel action)
        {
            try
            {
                AddStepActionViewModel step = new AddStepActionViewModel();
                step.ActionId = (int)ActionType.AppyCalculation;
                step.type = ActionType.AppyCalculation;
                step.IsStepActionMail = false;
                step.CalculateLandSpace = action.CalculateLandSpace;
                step.CalculateLoadSpace = action.CalculateLoadSpace;
                step.Period = action.Period;
                step.label = action.label;
                step.WorkflowId = action.WorkflowId;
                step.NextStepActions = action.NextStepActions;
                return AddStepAction(step);
            }
            catch (Exception err)
            {
                
                return new Response<StepActionWithNamesViewModel>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }

        }
        public Response<StepActionWithNamesViewModel> EditStepActionApplyCalculation(EditStepActionApplyCalculationViewModel action)
        {
            try
            {
                EditStepActionViewModel step = new EditStepActionViewModel();
                step.ActionId = (int)ActionType.AppyCalculation;
                step.type = ActionType.AppyCalculation;
                step.IsStepActionMail = false;
                step.CalculateLandSpace = action.CalculateLandSpace;
                step.CalculateLoadSpace = action.CalculateLoadSpace;
                step.Period = action.Period;
                step.label = action.label;
                step.Id = action.Id;
                step.NextStepActions = action.NextStepActions;
                return UpdateStepAction(step);
            }
            catch (Exception err)
            {
                
                return new Response<StepActionWithNamesViewModel>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }

        }
        public Response<StepActionWithNamesViewModel> AddMailStepAction(AddMailStepActionViewModel action)
        {
            try
            {
                AddStepActionViewModel step = new AddStepActionViewModel();
                step.ActionId = (int)ActionType.Email;
                step.type = ActionType.Email;
                step.IsStepActionMail = true;
                step.CalculateLandSpace = false;
                step.CalculateLoadSpace = false;
                step.MailBody = action.MailBody;
                step.MailSubject = action.MailSubject;
                step.Period = action.Period;
                step.label = action.label;
                //step.StepActionMailFrom = action.StepActionMailFrom;
                step.StepActionMailTo = action.StepActionMailTo;
                step.StepActionMailCC = action.StepActionMailCC;
                step.WorkflowId = action.WorkflowId;
                step.NextStepActions = action.NextStepActions;
                return AddStepAction(step);
            }
            catch (Exception err)
            {
                
                return new Response<StepActionWithNamesViewModel>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }

        }

        public Response<StepActionWithNamesViewModel> EditMailStepAction(EditMailStepActionViewModel action)
        {
            try
            {
                EditStepActionViewModel step = new EditStepActionViewModel();
                step.ActionId = (int)ActionType.Email;
                step.Id = action.Id;
                step.type = ActionType.Email;
                step.IsStepActionMail = true;
                step.CalculateLandSpace = false;
                step.CalculateLoadSpace = false;
                step.Period = action.Period;
                step.MailBody = action.MailBody;
                step.MailSubject = action.MailSubject;
                step.label = action.label;
                step.StepActionMailTo = action.StepActionMailTo;
                step.StepActionMailCC = action.StepActionMailCC;
                //step.WorkflowId = action.WorkflowId;
                step.NextStepActions = action.NextStepActions;
                return UpdateStepAction(step);
            }
            catch (Exception err)
            {
                
                return new Response<StepActionWithNamesViewModel>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }

        }

        public Response<StepActionWithNamesViewModel> AddStepAction(AddStepActionViewModel step)
        {
            try
            {
                TLIstepAction sa = _mapper.Map<TLIstepAction>(step);
                if (sa.IncomItemStatus == null)
                {
                    sa.IncomItemStatus = new List<TLIstepActionIncomeItemStatus>();
                }
                if (sa.StepActionMailTo == null)
                {
                    sa.StepActionMailTo = new List<TLIstepActionMailTo>();
                }
                if (sa.StepActionFileGroup == null)
                {
                    sa.StepActionFileGroup = new List<TLIstepActionFileGroup>();
                }
                if (sa.StepActionGroup == null)
                {
                    sa.StepActionGroup = new List<TLIstepActionGroup>();
                }
                if (sa.StepActionPart == null)
                {
                    sa.StepActionPart = new List<TLIstepActionPart>();
                }
                if (sa.StepActionOption == null)
                {
                    sa.StepActionOption = new List<TLIstepActionOption>();
                }
                if (sa.NextStepActions == null)
                {
                    sa.NextStepActions = new List<TLInextStepAction>();
                }
                /*
                if (sa.ItemStatus == null)
                {
                    sa.ItemStatus = new List<TLIstepActionItemStatus>();
                }
                //*/
                if (sa.StepActionItemOption == null)
                {
                    sa.StepActionItemOption = new List<TLIstepActionItemOption>();
                }
                sa.IncomItemStatus.Clear();
                sa.StepActionMailTo.Clear();
                sa.StepActionFileGroup.Clear();
                sa.StepActionGroup.Clear();
                sa.StepActionPart.Clear();
                sa.StepActionOption.Clear();
                sa.NextStepActions.Clear();
                //sa.ItemStatus.Clear();
                sa.StepActionItemOption.Clear();
                sa.sequence = 0;
                /*
                if (step.StepActionMailFrom != null)
                {
                    sa.StepActionMailFrom = new TLIstepActionMail();
                    sa.StepActionMailFrom.ActorId = step.StepActionMailFrom.ActorId;
                    //   sa.StepActionMailFrom.IntegrationId = step.StepActionMailFrom.IntegrationId;
                    sa.StepActionMailFrom.GroupId = step.StepActionMailFrom.GroupId;
                    sa.StepActionMailFrom.UserId = step.StepActionMailFrom.UserId;
                    //_unitOfWork.StepActionMailFromRepository.Add()
                    sa.StepActionMailFromId = sa.StepActionMailFrom.Id;
                }
                //*/
                _unitOfWork.StepActionRepository.Add(sa);
                _unitOfWork.SaveChanges();
                if (step.NextStepActions != null)
                {
                    foreach (var next in step.NextStepActions)
                    {
                        var iis = new TLInextStepAction();
                        iis.NextStepActionId = next;
                        iis.StepActionId = sa.Id;
                        sa.NextStepActions.Add(iis);
                    }
                }
                if (step.IncomItemStatus != null)
                {
                    foreach (var incomestatus in step.IncomItemStatus)
                    {
                        var iis = new TLIstepActionIncomeItemStatus();
                        //iis.ItemStatus = getItemStatusById(incomestatus);
                        iis.ItemStatusId = incomestatus.Id;
                        //iis.StepAction = sa;
                        iis.StepActionId = sa.Id;
                        sa.IncomItemStatus.Add(iis);
                    }
                }
                if (step.StepActionMailTo != null)
                {
                    if (sa.StepActionMailTo == null)
                    {
                        sa.StepActionMailTo = new List<TLIstepActionMailTo>();
                    }
                    if (step.StepActionMailTo.users != null)
                    {
                        foreach (var usr in step.StepActionMailTo.users)
                        {
                            var mt = new TLIstepActionMailTo();
                            mt.StepAction = sa;
                            mt.StepActionId = sa.Id;
                            mt.Type = MailtType.To;
                            //mt.ActorId = file.ActorId;
                            //mt.IntegrationId = file.IntegrationId;
                            //mt.GroupId = file.GroupId;
                            mt.UserId = usr.Id;
                            sa.StepActionMailTo.Add(mt);
                        }
                    }
                    if (step.StepActionMailTo.groups != null)
                    {
                        foreach (var grp in step.StepActionMailTo.groups)
                        {
                            var mt = new TLIstepActionMailTo();
                            mt.StepAction = sa;
                            mt.StepActionId = sa.Id;
                            mt.Type = MailtType.To;
                            //mt.ActorId = file.ActorId;
                            //mt.IntegrationId = file.IntegrationId;
                            mt.GroupId = grp.Id;
                            //mt.UserId = usr.Id;
                            sa.StepActionMailTo.Add(mt);
                        }
                    }
                    if (step.StepActionMailTo.actors != null)
                    {
                        foreach (var act in step.StepActionMailTo.actors)
                        {
                            var mt = new TLIstepActionMailTo();
                            mt.StepAction = sa;
                            mt.StepActionId = sa.Id;
                            mt.Type = MailtType.To;
                            mt.ActorId = act.Id;
                            //mt.IntegrationId = file.IntegrationId;
                            //mt.GroupId = file.GroupId;
                            //mt.UserId = usr.Id;
                            sa.StepActionMailTo.Add(mt);
                        }
                    }
                    /*
                    if (step.StepActionMailTo.integration != null)
                    {
                        foreach (var integration in step.StepActionMailTo.integration)
                        {
                            var mt = new TLIstepActionMailTo();
                            mt.StepAction = sa;
                            mt.StepActionId = sa.Id;
                            mt.Type = MailtType.To;
                            //mt.ActorId = file.ActorId;
                            mt.IntegrationId = integration.Id;
                            //mt.GroupId = file.GroupId;
                            //mt.UserId = usr.Id;
                            sa.StepActionMailTo.Add(mt);
                        }
                    }
                    //*/


                    /*
                    foreach (AddStepActionMailToViewModel mailTo in step.StepActionMailTo)
                    {
                        var mt = new TLIstepActionMailTo();
                        mt.StepAction = sa;
                        mt.StepActionId = sa.Id;
                        mt.ActorId = mailTo.ActorId;
                        //mt.IntegrationId = mailTo.IntegrationId;
                        mt.GroupId = mailTo.GroupId;
                        mt.UserId = mailTo.UserId;
                        mt.Type = mailTo.Type;
                        sa.StepActionMailTo.Add(mt);
                    }
                    //*/
                }
                if (step.StepActionMailCC != null)
                {
                    if (sa.StepActionMailTo == null)
                    {
                        sa.StepActionMailTo = new List<TLIstepActionMailTo>();
                    }
                    if (step.StepActionMailCC.users != null)
                    {
                        foreach (var usr in step.StepActionMailCC.users)
                        {
                            var mt = new TLIstepActionMailTo();
                            mt.StepAction = sa;
                            mt.StepActionId = sa.Id;
                            mt.Type = MailtType.CC;
                            //mt.ActorId = file.ActorId;
                            //mt.IntegrationId = file.IntegrationId;
                            //mt.GroupId = file.GroupId;
                            mt.UserId = usr.Id;
                            sa.StepActionMailTo.Add(mt);
                        }
                    }
                    if (step.StepActionMailCC.groups != null)
                    {
                        foreach (var grp in step.StepActionMailCC.groups)
                        {
                            var mt = new TLIstepActionMailTo();
                            mt.StepAction = sa;
                            mt.StepActionId = sa.Id;
                            mt.Type = MailtType.CC;
                            //mt.ActorId = file.ActorId;
                            //mt.IntegrationId = file.IntegrationId;
                            mt.GroupId = grp.Id;
                            //mt.UserId = usr.Id;
                            sa.StepActionMailTo.Add(mt);
                        }
                    }
                    if (step.StepActionMailCC.actors != null)
                    {
                        foreach (var act in step.StepActionMailCC.actors)
                        {
                            var mt = new TLIstepActionMailTo();
                            mt.StepAction = sa;
                            mt.StepActionId = sa.Id;
                            mt.Type = MailtType.CC;
                            mt.ActorId = act.Id;
                            //mt.IntegrationId = file.IntegrationId;
                            //mt.GroupId = file.GroupId;
                            //mt.UserId = usr.Id;
                            sa.StepActionMailTo.Add(mt);
                        }
                    }
                    /*
                    if (step.StepActionGroup.integration != null)
                    {
                        foreach (var integration in step.StepActionGroup.integration)
                        {
                            var mt = new TLIstepActionMailTo();
                            mt.StepAction = sa;
                            mt.StepActionId = sa.Id;
                            mt.Type = MailtType.To;
                            //mt.ActorId = file.ActorId;
                            mt.IntegrationId = integration.Id;
                            //mt.GroupId = file.GroupId;
                            //mt.UserId = usr.Id;
                            sa.StepActionMailTo.Add(mt);
                        }
                    }
                    //*/


                    /*
                    foreach (AddStepActionMailToViewModel mailTo in step.StepActionMailTo)
                    {
                        var mt = new TLIstepActionMailTo();
                        mt.StepAction = sa;
                        mt.StepActionId = sa.Id;
                        mt.ActorId = mailTo.ActorId;
                        //mt.IntegrationId = mailTo.IntegrationId;
                        mt.GroupId = mailTo.GroupId;
                        mt.UserId = mailTo.UserId;
                        mt.Type = mailTo.Type;
                        sa.StepActionMailTo.Add(mt);
                    }
                    //*/
                }
                /*
                if (step.StepActionFileGroup != null)
                {
                    if (step.StepActionFileGroup.users != null)
                    {
                        foreach (var usr in step.StepActionFileGroup.users)
                        {
                            var mt = new TLIstepActionFileGroup();
                            mt.StepAction = sa;
                            mt.StepActionId = sa.Id;
                            //mt.ActorId = file.ActorId;
                            //mt.IntegrationId = file.IntegrationId;
                            //mt.GroupId = file.GroupId;
                            mt.UserId = usr.Id;
                            mt.Active = true;
                            mt.Deleted = false;
                            sa.StepActionFileGroup.Add(mt);
                        }
                    }
                    if (step.StepActionFileGroup.groups != null)
                    {
                        foreach (var grp in step.StepActionFileGroup.groups)
                        {
                            var mt = new TLIstepActionFileGroup();
                            mt.StepAction = sa;
                            mt.StepActionId = sa.Id;
                            //mt.ActorId = file.ActorId;
                            //mt.IntegrationId = file.IntegrationId;
                            mt.GroupId = grp.Id;
                            //mt.UserId = usr.Id;
                            mt.Active = true;
                            mt.Deleted = false;
                            sa.StepActionFileGroup.Add(mt);
                        }
                    }
                    if (step.StepActionFileGroup.actors != null)
                    {
                        foreach (var act in step.StepActionFileGroup.actors)
                        {
                            var mt = new TLIstepActionFileGroup();
                            mt.StepAction = sa;
                            mt.StepActionId = sa.Id;
                            mt.ActorId = act.Id;
                            //mt.IntegrationId = file.IntegrationId;
                            //mt.GroupId = grp.Id;
                            //mt.UserId = usr.Id;
                            mt.Active = true;
                            mt.Deleted = false;
                            sa.StepActionFileGroup.Add(mt);
                        }
                    }
                    if (step.StepActionFileGroup.integration != null)
                    {
                        foreach (var integration in step.StepActionFileGroup.integration)
                        {
                            var mt = new TLIstepActionFileGroup();
                            mt.StepAction = sa;
                            mt.StepActionId = sa.Id;
                            //mt.ActorId = file.ActorId;
                            mt.IntegrationId = integration.Id;
                            //mt.GroupId = grp.Id;
                            //mt.UserId = usr.Id;
                            mt.Active = true;
                            mt.Deleted = false;
                            sa.StepActionFileGroup.Add(mt);
                        }
                    }
                }
                //*/
                if (step.StepActionGroup != null)
                {
                    if (step.StepActionGroup.users != null)
                    {
                        var usr = step.StepActionGroup.users;
                        //foreach (var usr in step.StepActionGroup.users)
                        //{
                            var mt = new TLIstepActionGroup();
                            mt.StepAction = sa;
                            mt.StepActionId = sa.Id;
                            //mt.ActorId = file.ActorId;
                            //mt.IntegrationId = file.IntegrationId;
                            //mt.GroupId = file.GroupId;
                            mt.UserId = usr.Id;
                            mt.Deleted = false;
                            mt.Active = true;
                            sa.StepActionGroup.Add(mt);
                        //}
                    }
                    if (step.StepActionGroup.groups != null)
                    {
                        var grp = step.StepActionGroup.groups;
                        //foreach (var grp in step.StepActionGroup.groups)
                        //{
                            var mt = new TLIstepActionGroup();
                            mt.StepAction = sa;
                            mt.StepActionId = sa.Id;
                            //mt.ActorId = file.ActorId;
                            //mt.IntegrationId = file.IntegrationId;
                            mt.GroupId = grp.Id;
                            //mt.UserId = usr.Id;
                            mt.Deleted = false;
                            mt.Active = true;
                            sa.StepActionGroup.Add(mt);
                        //}
                    }
                    if (step.StepActionGroup.actors != null)
                    {
                        var act = step.StepActionGroup.actors;
                        //foreach (var act in step.StepActionGroup.actors)
                        //{
                            var mt = new TLIstepActionGroup();
                            mt.StepAction = sa;
                            mt.StepActionId = sa.Id;
                            mt.ActorId = act.Id;
                            //mt.IntegrationId = file.IntegrationId;
                            //mt.GroupId = file.GroupId;
                            //mt.UserId = usr.Id;
                            mt.Deleted = false;
                            mt.Active = true;
                            sa.StepActionGroup.Add(mt);
                        //}
                    }
                    if (step.StepActionGroup.integration != null)
                    {
                        var integration = step.StepActionGroup.integration;
                        //foreach (var integration in step.StepActionGroup.integration)
                        //{
                            var mt = new TLIstepActionGroup();
                            mt.StepAction = sa;
                            mt.StepActionId = sa.Id;
                            //mt.ActorId = file.ActorId;
                            mt.IntegrationId = integration.Id;
                            //mt.GroupId = file.GroupId;
                            //mt.UserId = usr.Id;
                            mt.Deleted = false;
                            mt.Active = true;
                            sa.StepActionGroup.Add(mt);
                        //}
                    }
                }
                if (step.StepActionPart != null)
                    foreach (var part in step.StepActionPart)
                    {
                        if (part.PartId == 0)  // site
                        {
                            if (sa.ActionId == (int)ActionType.InsertData || sa.ActionId == (int)ActionType.UpdateData || sa.ActionId == (int)ActionType.UploadFile) //|| sa.ActionId == (int)ActionType.TelecomValidation || sa.ActionId == (int)ActionType.CivilValidation
                            {
                                /*
                                if (part.StepActionPartGroup != null)
                                {
                                    if (sa.StepActionFileGroup == null)
                                    {
                                        sa.StepActionFileGroup = new List<TLIstepActionFileGroup>();
                                    }
                                    if (part.StepActionPartGroup.users != null && part.StepActionPartGroup.users.Count > 0)
                                    {
                                        foreach (var pg in part.StepActionPartGroup.users)
                                        {
                                            var mt = new TLIstepActionFileGroup();
                                            mt.StepAction = sa;
                                            mt.StepActionId = sa.Id;
                                            //mt.ActorId = pg.Id;
                                            mt.UserId = pg.Id;
                                            //mt.GroupId = pg.Id;
                                            //mt.IntegrationId = pg.Id;
                                            mt.Active = true;
                                            mt.Deleted = false;
                                            sa.StepActionFileGroup.Add(mt);
                                        }
                                    }
                                    if (part.StepActionPartGroup.groups != null && part.StepActionPartGroup.groups.Count > 0)
                                    {
                                        foreach (var pg in part.StepActionPartGroup.groups)
                                        {
                                            var mt = new TLIstepActionFileGroup();
                                            mt.StepAction = sa;
                                            mt.StepActionId = sa.Id;
                                            //mt.ActorId = pg.Id;
                                            //mt.UserId = pg.Id;
                                            mt.GroupId = pg.Id;
                                            //mt.IntegrationId = pg.Id;
                                            mt.Active = true;
                                            mt.Deleted = false;
                                            sa.StepActionFileGroup.Add(mt);
                                        }
                                    }
                                    if (part.StepActionPartGroup.actors != null && part.StepActionPartGroup.actors.Count > 0)
                                    {
                                        foreach (var pg in part.StepActionPartGroup.actors)
                                        {
                                            var mt = new TLIstepActionFileGroup();
                                            mt.StepAction = sa;
                                            mt.StepActionId = sa.Id;
                                            mt.ActorId = pg.Id;
                                            //mt.UserId = pg.Id;
                                            //mt.GroupId = pg.Id;
                                            //mt.IntegrationId = pg.Id;
                                            mt.Active = true;
                                            mt.Deleted = false;
                                            sa.StepActionFileGroup.Add(mt);
                                        }
                                    }
                                    if (part.StepActionPartGroup.integration != null && part.StepActionPartGroup.integration.Count > 0)
                                    {
                                        foreach (var pg in part.StepActionPartGroup.integration)
                                        {
                                            var mt = new TLIstepActionFileGroup();
                                            mt.StepAction = sa;
                                            mt.StepActionId = sa.Id;
                                            //mt.ActorId = pg.Id;
                                            //mt.UserId = pg.Id;
                                            //mt.GroupId = pg.Id;
                                            mt.IntegrationId = pg.Id;
                                            mt.Active = true;
                                            mt.Deleted = false;
                                            sa.StepActionFileGroup.Add(mt);
                                        }
                                    }
                                }
                                //*/
                                var mt = new TLIstepActionFileGroup();
                                mt.StepAction = sa;
                                mt.StepActionId = sa.Id;
                                mt.Active = true;
                                mt.Deleted = false;
                                sa.StepActionFileGroup.Add(mt);


                                sa.AllowUploadFile = part.AllowUploadFile;
                                sa.UploadFileIsMandatory = part.UploadFileIsMandatory;
                            }
                        }
                        else
                        if (part.PartId == -1) // all parts
                        {
                            if (sa.ActionId == (int)ActionType.InsertData || sa.ActionId == (int)ActionType.UpdateData || sa.ActionId == (int)ActionType.UploadFile || sa.ActionId == (int)ActionType.TelecomValidation || sa.ActionId == (int)ActionType.CivilValidation) //
                            {
                                int count = 0;
                                var PartsList = (_unitOfWork.PartRepository.GetAllAsQueryable(out count)).ToList();
                                foreach (TLIpart p in PartsList)
                                {
                                    var mt = new TLIstepActionPart();
                                    mt.StepActionPartGroup = new List<TLIstepActionPartGroup>();
                                    mt.StepAction = sa;
                                    mt.StepActionId = sa.Id;
                                    mt.AllowUploadFile = part.AllowUploadFile;
                                    mt.UploadFileIsMandatory = part.UploadFileIsMandatory;
                                    mt.PartId = p.Id;
                                    /*
                                    if (part.StepActionPartGroup != null)
                                    {
                                        foreach (var pg in part.StepActionPartGroup.users)
                                        {
                                            var partg = new TLIstepActionPartGroup();
                                            partg.StepActionPartId = mt.Id;
                                            //partg.ActorId = pg.Id;
                                            partg.UserId = pg.Id;
                                            //partg.GroupId = pg.Id;
                                            //partg.IntegrationId = pg.Id;
                                            mt.StepActionPartGroup.Add(partg);
                                        }
                                        foreach (var pg in part.StepActionPartGroup.groups)
                                        {
                                            var partg = new TLIstepActionPartGroup();
                                            partg.StepActionPartId = mt.Id;
                                            //partg.ActorId = pg.Id;
                                            //partg.UserId = pg.Id;
                                            partg.GroupId = pg.Id;
                                            //partg.IntegrationId = pg.Id;
                                            mt.StepActionPartGroup.Add(partg);
                                        }
                                        foreach (var pg in part.StepActionPartGroup.actors)
                                        {
                                            var partg = new TLIstepActionPartGroup();
                                            partg.StepActionPartId = mt.Id;
                                            partg.ActorId = pg.Id;
                                            //partg.UserId = pg.Id;
                                            //partg.GroupId = pg.Id;
                                            //partg.IntegrationId = pg.Id;
                                            mt.StepActionPartGroup.Add(partg);
                                        }
                                        foreach (var pg in part.StepActionPartGroup.integration)
                                        {
                                            var partg = new TLIstepActionPartGroup();
                                            partg.StepActionPartId = mt.Id;
                                            //partg.ActorId = pg.Id;
                                            //partg.UserId = pg.Id;
                                            //partg.GroupId = pg.Id;
                                            partg.IntegrationId = pg.Id;
                                            mt.StepActionPartGroup.Add(partg);
                                        }
                                    }
                                    //*/
                                    mt.Active = true;
                                    mt.Deleted = false;
                                    sa.StepActionPart.Add(mt);
                                }



                            }
                        }
                        else
                        {
                            var mt = new TLIstepActionPart();
                            mt.StepActionPartGroup = new List<TLIstepActionPartGroup>();
                            mt.StepAction = sa;
                            mt.StepActionId = sa.Id;
                            mt.AllowUploadFile = part.AllowUploadFile;
                            mt.UploadFileIsMandatory = part.UploadFileIsMandatory;
                            mt.PartId = part.PartId;
                            /*
                            if (part.StepActionPartGroup != null)
                            {
                                foreach (var pg in part.StepActionPartGroup.users)
                                {
                                    var partg = new TLIstepActionPartGroup();
                                    partg.StepActionPartId = mt.Id;
                                    //partg.ActorId = pg.Id;
                                    partg.UserId = pg.Id;
                                    //partg.GroupId = pg.Id;
                                    //partg.IntegrationId = pg.Id;
                                    mt.StepActionPartGroup.Add(partg);
                                }
                                foreach (var pg in part.StepActionPartGroup.groups)
                                {
                                    var partg = new TLIstepActionPartGroup();
                                    partg.StepActionPartId = mt.Id;
                                    //partg.ActorId = pg.Id;
                                    //partg.UserId = pg.Id;
                                    partg.GroupId = pg.Id;
                                    //partg.IntegrationId = pg.Id;
                                    mt.StepActionPartGroup.Add(partg);
                                }
                                foreach (var pg in part.StepActionPartGroup.integration)
                                {
                                    var partg = new TLIstepActionPartGroup();
                                    partg.StepActionPartId = mt.Id;
                                    //partg.ActorId = pg.Id;
                                    //partg.UserId = pg.Id;
                                    //partg.GroupId = pg.Id;
                                    partg.IntegrationId = pg.Id;
                                    mt.StepActionPartGroup.Add(partg);
                                }
                                foreach (var pg in part.StepActionPartGroup.actors)
                                {
                                    var partg = new TLIstepActionPartGroup();
                                    partg.StepActionPartId = mt.Id;
                                    partg.ActorId = pg.Id;
                                    //partg.UserId = pg.Id;
                                    //partg.GroupId = pg.Id;
                                    //partg.IntegrationId = pg.Id;
                                    mt.StepActionPartGroup.Add(partg);
                                }
                            }
                            //*/
                            mt.Active = true;
                            mt.Deleted = false;
                            sa.StepActionPart.Add(mt);
                        }
                    }
                if (step.StepActionOption != null)
                {
                    foreach (var option in step.StepActionOption)
                    {
                        var mt = new TLIstepActionOption();
                        mt.StepAction = sa;
                        mt.StepActionId = sa.Id;
                        mt.ActionOptionId = option.ActionOptionId;
                        mt.AllowNote = option.AllowNote;
                        mt.NoteIsMandatory = option.NoteIsMandatory;
                        mt.NextStepActions =new List<TLInextStepAction>();
                        foreach( int next in option.NextStepActions)
                        {
                            TLInextStepAction nextAction = new TLInextStepAction();
                            nextAction.NextStepActionId = next;
                            nextAction.StepActionId = sa.Id;
                            nextAction.StepActionOptionId = mt.Id;
                            mt.NextStepActions.Add(nextAction);
                        }
                        mt.OrderStatusId = option.OrderStatusId;
                        mt.ItemStatusId = option.ItemStatusId;
                        mt.Deleted = false;
                        sa.StepActionOption.Add(mt);
                    }
                }
                if (step.StepActionItemOption != null)
                {
                    foreach (var option in step.StepActionItemOption)
                    {
                        var mt = new TLIstepActionItemOption();
                        mt.StepAction = sa;
                        mt.StepActionId = sa.Id;
                        mt.ActionItemOptionId = option.ActionItemOptionId;
                        //mt.NextStepActionId = option.NextStepActionId;
                        mt.NextStepActions = new List<TLInextStepAction>();
                        if (option.NextStepActions != null)
                        {
                            foreach (int next in option.NextStepActions)
                            {
                                TLInextStepAction nextAction = new TLInextStepAction();
                                nextAction.StepActionItemOptionId = mt.Id;
                                nextAction.NextStepActionId = next;
                                nextAction.StepActionId = sa.Id;
                                mt.NextStepActions.Add(nextAction);
                            }
                        }

                        mt.OrderStatusId = option.OrderStatusId;
                        mt.AllowNote = option.AllowNote;
                        mt.NoteIsMandatory = option.NoteIsMandatory;
                        if (option.StepActionItemStatus != null)
                        {
                            mt.StepActionItemStatus = new List<TLIstepActionItemStatus>();
                            if (option.StepActionItemStatus != null)
                            {
                                foreach (var pg in option.StepActionItemStatus)
                                {
                                    var partg = new TLIstepActionItemStatus();
                                    partg.StepActionItemOptionId = mt.Id;
                                    partg.IncomingItemStatusId = pg.IncomingItemStatusId;
                                    partg.OutgoingItemStatusId = pg.OutgoingItemStatusId;
                                    mt.StepActionItemStatus.Add(partg);
                                }
                            }
                        }

                        //mt.ItemStatusId = option.ItemStatusId;

                        //_unitOfWork.StepActionItemOptionRepository.Add(mt);
                        //_unitOfWork.SaveChanges();
                        sa.StepActionItemOption.Add(mt);
                    }
                }
                /*
                if (step.ItemStatus != null)
                {
                    foreach (var itemstatus in step.ItemStatus)
                    {
                        var mt = new TLIstepActionItemStatus();
                        mt.StepAction = sa;
                        mt.StepActionId = sa.Id;
                        mt.IncomingItemStatusId = itemstatus.IncomingItemStatusId;
                        mt.OutgoingItemStatusId = itemstatus.OutgoingItemStatusId;
                        sa.ItemStatus.Add(mt);
                    }
                }
                //*/
                sa.sequence = sa.Id;
                _unitOfWork.StepActionRepository.Update(sa);
                _unitOfWork.SaveChanges();
                return new Response<StepActionWithNamesViewModel>(StepActionWithNames(MapStepAction(sa)));
            }
            catch (Exception err)
            {
                
                return new Response<StepActionWithNamesViewModel>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }

        }

        private StepActionGroupsViewModel mapPartGroup(List<TLIstepActionPartGroup> parts)
        {
            if (parts != null && parts.Count>0)
            {
                StepActionGroupsViewModel newPart = new StepActionGroupsViewModel();
                foreach (var part in parts)
                {
                    if (part.UserId != null)
                    {
                        if (newPart.users == null)
                        {
                            newPart.users = new List<UserViewModel>();
                        }
                        newPart.users.Add(_mapper.Map<UserViewModel>(_unitOfWork.UserRepository.GetByID((int)part.UserId)));
                    }
                    if (part.GroupId != null)
                    {
                        if (newPart.groups == null)
                        {
                            newPart.groups = new List<GroupViewModel>();
                        }
                        newPart.groups.Add(_mapper.Map<GroupViewModel>(_unitOfWork.GroupRepository.GetByID((int)part.GroupId)));
                    }
                    if (part.ActorId == null)
                    {
                        if (newPart.actors == null)
                        {
                            newPart.actors = new List<ActorViewModel>();
                        }
                        newPart.actors.Add(_mapper.Map<ActorViewModel>(_unitOfWork.ActorRepository.GetByID((int)part.ActorId)));
                    }
                    if (part.IntegrationId == null)
                    {
                        if (newPart.integration == null)
                        {
                            newPart.integration = new List<IntegrationViewModel>();
                        }
                        newPart.integration.Add(_mapper.Map<IntegrationViewModel>(_unitOfWork.IntegrationRepository.GetByID((int)part.IntegrationId)));
                    }
                }
            }
            return null;
        }

        private List<ActorsName> FillActorsName(List<AddStepActionGroupViewModel> Actors)
        {
            List<ActorsName> actorName = new List<ActorsName>();
            if(Actors == null)
            {
                return null;
            }
            foreach(var act in Actors)
            {
                ActorsName actor = new ActorsName();
                actor.UserId = act.UserId;
                actor.GroupId = act.GroupId;
                actor.ActorId = act.ActorId;
                actor.IntegrationId = act.IntegrationId;
                if (act.UserId != null)
                {
                    var entity = _unitOfWork.UserRepository.GetByID((int)act.UserId);
                    actor.UserName = entity.FirstName + " " + entity.LastName;
                }
                if (act.GroupId != null)
                {
                    var entity = _unitOfWork.GroupRepository.GetByID((int)act.GroupId);
                    actor.GroupName = entity.Name;
                }
                if (act.ActorId != null)
                {
                    var entity = _unitOfWork.ActorRepository.GetByID((int)act.ActorId);
                    actor.ActorName = entity.Name;
                }
                if (act.IntegrationId != null)
                {
                    var entity = _unitOfWork.IntegrationRepository.GetByID((int)act.IntegrationId);
                    actor.IntegrationName = entity.Name;

                }
                actorName.Add(actor);
            }
            return actorName;
        }

        private ListStepActionViewModel MapStepAction(TLIstepAction entity)
        {
            if (entity == null) return null;
            ListStepActionViewModel model = _mapper.Map<ListStepActionViewModel>(entity);
            model.StepActionPart = _mapper.Map<List<ListStepActionPartViewModel>>(_unitOfWork.StepActionPartRepository.GetAllAsQueryable().Where(x => x.StepActionId == entity.Id).ToList());
            foreach (var part in model.StepActionPart)
            {
                var xpart = _unitOfWork.PartRepository.GetByID(part.PartId);
                part.PartName = xpart.Name;
                var entities = _unitOfWork.StepActionPartGroupRepository.GetAllAsQueryable().Where(x => x.StepActionPartId == part.Id).ToList();
                if (entities!=null)
                {
                    if (part.StepActionPartGroup == null)
                    {
                        part.StepActionPartGroup = new StepActionGroupsViewModel();
                    }
                    foreach (var ent in entities)
                    {
                        if (ent.UserId!=null)
                        {
                            if (part.StepActionPartGroup.users == null)
                            {
                                part.StepActionPartGroup.users = new List<UserViewModel>();
                            }
                            part.StepActionPartGroup.users.Add(_mapper.Map<UserViewModel>(_unitOfWork.UserRepository.GetByID((int)ent.UserId)));
                        }
                        if (ent.GroupId!=null)
                        {
                            if (part.StepActionPartGroup.groups == null)
                            {
                                part.StepActionPartGroup.groups = new List<GroupViewModel>();
                            }
                            part.StepActionPartGroup.groups.Add(_mapper.Map<GroupViewModel>(_unitOfWork.GroupRepository.GetByID((int)ent.GroupId)));
                        }
                        if (ent.ActorId!=null)
                        {
                            if (part.StepActionPartGroup.actors == null)
                            {
                                part.StepActionPartGroup.actors = new List<ActorViewModel>();
                            }
                            part.StepActionPartGroup.actors.Add(_mapper.Map<ActorViewModel>(_unitOfWork.ActorRepository.GetByID((int)ent.ActorId)));
                        }
                        if (ent.IntegrationId!=null)
                        {
                            if (part.StepActionPartGroup.integration == null)
                            {
                                part.StepActionPartGroup.integration = new List<IntegrationViewModel>();
                            }
                            part.StepActionPartGroup.integration.Add(_mapper.Map<IntegrationViewModel>(_unitOfWork.IntegrationRepository.GetByID((int)ent.IntegrationId)));
                        }
                        
                    }
                }
            }
            model.StepActionFileGroup = _mapper.Map<List<ListStepActionGroupViewModel>>(_unitOfWork.StepActionFileGroupRepository.GetAllAsQueryable().Where(x => x.StepActionId == entity.Id).ToList());
            model.StepActionGroup = _mapper.Map<List<ListStepActionGroupViewModel>>(_unitOfWork.StepActionGroupRepository.GetAllAsQueryable().Where(x => x.StepActionId == entity.Id).ToList());
            if (model.NextStepActions == null)
            {
                model.NextStepActions = new List<int>();
            }
            model.NextStepActions.Clear();
            var next=_unitOfWork.NextStepActionRepository.GetAllAsQueryable().Where(x => x.StepActionId == entity.Id).ToList();
            foreach( var n in next)
            {
                model.NextStepActions.Add(n.NextStepActionId);
            }
            model.IncomItemStatus = new List<ListItemStatusViewModel>();
            var lst = _unitOfWork.StepActionIncomeItemStatusRepository.GetAllAsQueryable().Where(x => x.StepActionId == entity.Id).ToList();
            if (lst != null) {
                foreach (var it in lst)
                {
                    model.IncomItemStatus.Add(_mapper.Map<ListItemStatusViewModel>(_unitOfWork.ItemStatusRepository.GetByID(it.ItemStatusId)));
                }
            }
            model.StepActionMailTo = new StepActionGroupsViewModel();
            model.StepActionMailCC = new StepActionGroupsViewModel();
            var lst2 = _unitOfWork.StepActionMailToRepository.GetAllAsQueryable().Where(x => x.StepActionId == entity.Id).ToList();
            if (lst2 != null)
            {
                foreach (var ent in lst2)
                {
                    if (ent.Type == MailtType.To)
                    {
                        if (ent.UserId != null)
                        {
                            if (model.StepActionMailTo.users == null)
                            {
                                model.StepActionMailTo.users = new List<UserViewModel>();
                            }
                            model.StepActionMailTo.users.Add(_mapper.Map<UserViewModel>(_unitOfWork.UserRepository.GetByID((int)ent.UserId)));
                        }
                        if (ent.GroupId != null)
                        {
                            if (model.StepActionMailTo.groups == null)
                            {
                                model.StepActionMailTo.groups = new List<GroupViewModel>();
                            }
                            model.StepActionMailTo.groups.Add(_mapper.Map<GroupViewModel>(_unitOfWork.GroupRepository.GetByID((int)ent.GroupId)));
                        }
                        if (ent.ActorId != null)
                        {
                            if (model.StepActionMailTo.actors == null)
                            {
                                model.StepActionMailTo.actors = new List<ActorViewModel>();
                            }
                            model.StepActionMailTo.actors.Add(_mapper.Map<ActorViewModel>(_unitOfWork.ActorRepository.GetByID((int)ent.ActorId)));
                        }



                    }
                    else if (ent.Type == MailtType.CC)
                    {
                        if (ent.UserId != null)
                        {
                            if (model.StepActionMailCC.users == null)
                            {
                                model.StepActionMailCC.users = new List<UserViewModel>();
                            }
                            model.StepActionMailCC.users.Add(_mapper.Map<UserViewModel>(_unitOfWork.UserRepository.GetByID((int)ent.UserId)));
                        }
                        if (ent.GroupId != null)
                        {
                            if (model.StepActionMailCC.groups == null)
                            {
                                model.StepActionMailCC.groups = new List<GroupViewModel>();
                            }
                            model.StepActionMailCC.groups.Add(_mapper.Map<GroupViewModel>(_unitOfWork.GroupRepository.GetByID((int)ent.GroupId)));
                        }
                        if (ent.ActorId != null)
                        {
                            if (model.StepActionMailCC.actors == null)
                            {
                                model.StepActionMailCC.actors = new List<ActorViewModel>();
                            }
                            model.StepActionMailCC.actors.Add(_mapper.Map<ActorViewModel>(_unitOfWork.ActorRepository.GetByID((int)ent.ActorId)));
                        }


                    }

                }
            }
           //odel.StepActionMailTo = _mapper.Map<List<ListStepActionMailToViewModel>>(_unitOfWork.StepActionMailToRepository.GetAllAsQueryable().Where(x => x.StepActionId == entity.Id).ToList());
            //model.StepActionMailFrom = _mapper.Map<PermissionStepActionViewModel>(_unitOfWork.StepActionMailFromRepository.GetAllAsQueryable().Where(x => x.Id == entity.StepActionMailFromId).SingleOrDefault());
            model.StepActionOption = _mapper.Map<List<AddStepActionOptionViewModel>>(_unitOfWork.StepActionOptionRepository.GetAllAsQueryable().Where(x => x.StepActionId == entity.Id).ToList());
            var StepActionItemOptions = _mapper.Map<List<ListStepActionItemOptionViewModel>>(_unitOfWork.StepActionItemOptionRepository.GetAllAsQueryable().Where(x => x.StepActionId == entity.Id).ToList());
            foreach (var option in StepActionItemOptions)
            {
                option.StepActionItemStatus = _mapper.Map<List<AddStepActionItemStatusViewModel>>(_unitOfWork.StepActionItemStatusRepository.GetAllAsQueryable().Where(x => x.StepActionItemOptionId == option.Id).ToList());
                var nextActions = _unitOfWork.NextStepActionRepository.GetAllAsQueryable().Where(x => x.StepActionItemOptionId == option.Id).ToList();
                option.NextStepActions = new List<int>();
                foreach (var n in nextActions)
                {
                    option.NextStepActions.Add(n.NextStepActionId);
                }
            }
            model.StepActionItemOption = _mapper.Map<List<AddStepActionItemOptionViewModel>>(StepActionItemOptions);

            return model;
        }

        public TLIitemStatus getItemStatusById(int Id)
        {

            return _unitOfWork.ItemStatusRepository.GetAllAsQueryable().Where(x => x.Id == Id && x.Deleted == false).SingleOrDefault();
        }
        public Response<ListStepActionViewModel> SetStepActionPermission(int StepActionId, StepActionGroupPermission permissions)
        {
            var entity = _unitOfWork.StepActionRepository.GetAllAsQueryable().Where(x => x.Id == StepActionId && x.Deleted == false).SingleOrDefault();
            if (permissions != null && permissions.Permissions != null && entity != null)
            {
                if (entity.StepActionGroup == null)
                {
                    entity.StepActionGroup = new List<TLIstepActionGroup>();
                }
                entity.StepActionGroup.Clear();
                foreach (var per in permissions.Permissions)
                {
                    TLIstepActionGroup sag = _mapper.Map<TLIstepActionGroup>(per);
                    sag.StepActionId = StepActionId;
                    entity.StepActionGroup.Add(sag);
                }
            }
            _unitOfWork.StepActionRepository.Update(entity);
            _unitOfWork.SaveChanges();

            return new Response<ListStepActionViewModel>(true, MapStepAction(entity), null, null, (int)Helpers.Constants.ApiReturnCode.success);
        }

        public Response<ListStepActionViewModel> MoveUpStepAction(int Id)
        {
            try
            {
                var entity = _unitOfWork.StepActionRepository.GetAllAsQueryable().Where(x => x.Id == Id && x.Deleted == false).SingleOrDefault();
                var prevEntities = _unitOfWork.StepActionRepository.GetAllAsQueryable().Where(x => x.WorkflowId == entity.WorkflowId && x.sequence < entity.sequence && x.Deleted == false).ToList();//.Max(x => x.sequence); //.SingleOrDefault();
                if (prevEntities == null || prevEntities.Count() == 0)
                {
                    return new Response<ListStepActionViewModel>(true, null, null, "there is no upper items", (int)Helpers.Constants.ApiReturnCode.fail);
                }
                int maxSequence = prevEntities.Max(x => x.sequence);
                var upperEntity = prevEntities.Where(x => x.sequence == maxSequence).SingleOrDefault();
                upperEntity.sequence = entity.sequence;
                entity.sequence = maxSequence;
                _unitOfWork.StepActionRepository.Update(entity);
                _unitOfWork.StepActionRepository.Update(upperEntity);
                _unitOfWork.SaveChanges();
                return new Response<ListStepActionViewModel>(true, _mapper.Map<ListStepActionViewModel>(entity), null, null, (int)Helpers.Constants.ApiReturnCode.success);
            }catch(Exception err)
            {
                
                return new Response<ListStepActionViewModel>(true, null, null, "there is no upper items", (int)Helpers.Constants.ApiReturnCode.fail);

            }

        }
        public Response<ListStepActionViewModel> MoveDownStepAction(int Id)
        {
            try
            {
                var entity = _unitOfWork.StepActionRepository.GetAllAsQueryable().Where(x => x.Id == Id && x.Deleted == false).SingleOrDefault();
                var prevEntities = _unitOfWork.StepActionRepository.GetAllAsQueryable().Where(x => x.WorkflowId == entity.WorkflowId && x.sequence > entity.sequence && x.Deleted == false);//.Max(x => x.sequence); //.SingleOrDefault();
                if (prevEntities == null || prevEntities.Count() == 0)
                {
                    return new Response<ListStepActionViewModel>(true, null, null, "there is no lower items", (int)Helpers.Constants.ApiReturnCode.fail);
                }
                int minSequence = prevEntities.Min(x => x.sequence);
                var lowerEntity = prevEntities.Where(x => x.sequence == minSequence).SingleOrDefault();
                lowerEntity.sequence = entity.sequence;
                entity.sequence = minSequence;
                _unitOfWork.StepActionRepository.Update(entity);
                _unitOfWork.StepActionRepository.Update(lowerEntity);
                _unitOfWork.SaveChanges();
                return new Response<ListStepActionViewModel>(true, _mapper.Map<ListStepActionViewModel>(entity), null, null, (int)Helpers.Constants.ApiReturnCode.success);
            }catch(Exception err)
            {
                
                return new Response<ListStepActionViewModel>(true, null, null, "there is no upper items", (int)Helpers.Constants.ApiReturnCode.fail);
            }

        }


        public Response<StepActionWithNamesViewModel> UpdateStepAction(EditStepActionViewModel step)
        {
            try
            {
                TLIstepAction sa = _unitOfWork.StepActionRepository.GetByID(step.Id);
                var sa1 = _mapper.Map<TLIstepAction>(step);
                sa.label = sa1.label;
                sa.OutputMode = sa1.OutputMode;
                sa.InputMode = sa1.InputMode;
                if(sa.StepActionFileGroup==null)
                {
                    sa.StepActionFileGroup = new List<TLIstepActionFileGroup>();
                    var oldData = _unitOfWork.StepActionFileGroupRepository.GetAllAsQueryable().Where(x => x.StepActionId == sa.Id);
                    foreach (var i in oldData)
                    {
                        _unitOfWork.StepActionFileGroupRepository.RemoveItem(i);
                    }
                }
                sa.StepActionFileGroup.Clear();
                //sa.NextStepActionId = sa1.NextStepActionId;
                /*
                if (sa.NextStepActions == null)
                {
                    

                    
                    var oldData = _unitOfWork.NextStepActionRepository.GetAllAsQueryable().Where(x => x.StepActionId == sa.Id);
                    foreach (var i in oldData)
                    {
                        _unitOfWork.NextStepActionRepository.RemoveItem(i);
                    }
                    sa.NextStepActions = new List<TLInextStepAction>();
                }

                sa.NextStepActions.Clear();
                _unitOfWork.StepActionRepository.Update(sa);
                _unitOfWork.SaveChanges();
                foreach (int next in step.NextStepActions)
                {
                    TLInextStepAction nextAction = new TLInextStepAction();
                    nextAction.StepActionId = step.Id;
                    nextAction.NextStepActionId = next;
                    sa.NextStepActions.Add(nextAction);
                }
                //*/
                sa.AllowUploadFile = sa1.AllowUploadFile;
                sa.UploadFileIsMandatory = sa1.UploadFileIsMandatory;
                sa.AllowNote = sa1.AllowNote;
                sa.NoteIsMandatory = sa1.NoteIsMandatory;
                sa.CalculateLoadSpace = sa1.CalculateLoadSpace;
                sa.CalculateLandSpace = sa1.CalculateLandSpace;
                sa.OrderStatusId = sa1.OrderStatusId;
                sa.Active = sa1.Active;
                sa.Operation = sa1.Operation;
                sa.IsStepActionMail = sa1.IsStepActionMail;
                sa.MailSubject = sa1.MailSubject;
                sa.MailBody = sa1.MailBody;
                sa.Period = sa1.Period;
                //sa.sequence = 0;
                /*
                if (step.StepActionMailFrom != null)
                {
                    sa.StepActionMailFrom = new TLIstepActionMail();
                    sa.StepActionMailFrom.ActorId = step.StepActionMailFrom.ActorId;
                    //   sa.StepActionMailFrom.IntegrationId = step.StepActionMailFrom.IntegrationId;
                    sa.StepActionMailFrom.GroupId = step.StepActionMailFrom.GroupId;
                    sa.StepActionMailFrom.UserId = step.StepActionMailFrom.UserId;
                    sa.StepActionMailFromId = sa.StepActionMailFrom.Id;
                }
                //*/
                if (step.NextStepActions != null)
                {
                    if (sa.NextStepActions == null)
                    {
                        var oldData= _unitOfWork.NextStepActionRepository.GetAllAsQueryable().Where(x => x.StepActionId == sa.Id);
                        foreach(var i in oldData)
                        {
                            _unitOfWork.NextStepActionRepository.RemoveItem(i);
                        }
                        sa.NextStepActions = new List<TLInextStepAction>();
                    }
                    sa.NextStepActions.Clear();
                    _unitOfWork.StepActionRepository.Update(sa);
                    _unitOfWork.SaveChanges();
                    foreach (var next in step.NextStepActions)
                    {
                        var iis = new TLInextStepAction();
                        //iis.ItemStatus = getItemStatusById(incomestatus);
                        iis.NextStepActionId = next;
                        //iis.StepAction = sa;
                        iis.StepActionId = sa.Id;
                        sa.NextStepActions.Add(iis);
                    }
                }
                if (step.IncomItemStatus != null)
                {
                    if (sa.IncomItemStatus == null)
                    {
                        var oldData= _unitOfWork.StepActionIncomeItemStatusRepository.GetAllAsQueryable().Where(x => x.StepActionId == sa.Id);
                        foreach(var i in oldData)
                        {
                            _unitOfWork.StepActionIncomeItemStatusRepository.RemoveItem(i);
                        }
                        sa.IncomItemStatus = new List<TLIstepActionIncomeItemStatus>();
                    }
                    sa.IncomItemStatus.Clear();
                    _unitOfWork.StepActionRepository.Update(sa);
                    _unitOfWork.SaveChanges();
                    foreach (var incomestatus in step.IncomItemStatus)
                    {
                        var iis = new TLIstepActionIncomeItemStatus();
                        //iis.ItemStatus = getItemStatusById(incomestatus);
                        iis.ItemStatusId = incomestatus.Id;
                        //iis.StepAction = sa;
                        iis.StepActionId = sa.Id;
                        sa.IncomItemStatus.Add(iis);
                    }
                }

                if (sa.StepActionMailTo == null)
                {
                    var oldData = _unitOfWork.StepActionMailToRepository.GetAllAsQueryable().Where(x => x.StepActionId == sa.Id);
                    foreach (var i in oldData)
                    {
                        _unitOfWork.StepActionMailToRepository.RemoveItem(i);
                    }
                    sa.StepActionMailTo = new List<TLIstepActionMailTo>();
                }
                sa.StepActionMailTo.Clear();
                _unitOfWork.StepActionRepository.Update(sa);
                _unitOfWork.SaveChanges();
                if (step.StepActionMailTo != null)
                {
                    if (step.StepActionMailTo.users != null)
                    {
                        foreach (var usr in step.StepActionMailTo.users)
                        {
                            var mt = new TLIstepActionMailTo();
                            mt.StepAction = sa;
                            mt.StepActionId = sa.Id;
                            mt.Type = MailtType.To;
                            //mt.ActorId = file.ActorId;
                            //mt.IntegrationId = file.IntegrationId;
                            //mt.GroupId = file.GroupId;
                            mt.UserId = usr.Id;
                            sa.StepActionMailTo.Add(mt);
                        }
                    }
                    if (step.StepActionMailTo.groups != null)
                    {
                        foreach (var grp in step.StepActionMailTo.groups)
                        {
                            var mt = new TLIstepActionMailTo();
                            mt.StepAction = sa;
                            mt.StepActionId = sa.Id;
                            mt.Type = MailtType.To;
                            //mt.ActorId = file.ActorId;
                            //mt.IntegrationId = file.IntegrationId;
                            mt.GroupId = grp.Id;
                            //mt.UserId = usr.Id;
                            sa.StepActionMailTo.Add(mt);
                        }
                    }
                    if (step.StepActionMailTo.actors != null)
                    {
                        foreach (var act in step.StepActionMailTo.actors)
                        {
                            var mt = new TLIstepActionMailTo();
                            mt.StepAction = sa;
                            mt.StepActionId = sa.Id;
                            mt.Type = MailtType.To;
                            mt.ActorId = act.Id;
                            //mt.IntegrationId = file.IntegrationId;
                            //mt.GroupId = file.GroupId;
                            //mt.UserId = usr.Id;
                            sa.StepActionMailTo.Add(mt);
                        }
                    }
                    /*
                    if (step.StepActionMailTo.integration != null)
                    {
                        foreach (var integration in step.StepActionMailTo.integration)
                        {
                            var mt = new TLIstepActionMailTo();
                            mt.StepAction = sa;
                            mt.StepActionId = sa.Id;
                            mt.Type = MailtType.To;
                            //mt.ActorId = file.ActorId;
                            mt.IntegrationId = integration.Id;
                            //mt.GroupId = file.GroupId;
                            //mt.UserId = usr.Id;
                            sa.StepActionMailTo.Add(mt);
                        }
                    }
                    //*/


                    /*
                    foreach (AddStepActionMailToViewModel mailTo in step.StepActionMailTo)
                    {
                        var mt = new TLIstepActionMailTo();
                        mt.StepAction = sa;
                        mt.StepActionId = sa.Id;
                        mt.ActorId = mailTo.ActorId;
                        //mt.IntegrationId = mailTo.IntegrationId;
                        mt.GroupId = mailTo.GroupId;
                        mt.UserId = mailTo.UserId;
                        mt.Type = mailTo.Type;
                        sa.StepActionMailTo.Add(mt);
                    }
                    //*/
                }
                if (step.StepActionMailCC != null)
                {
                    if (step.StepActionMailCC.users != null)
                    {
                        foreach (var usr in step.StepActionMailCC.users)
                        {
                            var mt = new TLIstepActionMailTo();
                            mt.StepAction = sa;
                            mt.StepActionId = sa.Id;
                            mt.Type = MailtType.CC;
                            //mt.ActorId = file.ActorId;
                            //mt.IntegrationId = file.IntegrationId;
                            //mt.GroupId = file.GroupId;
                            mt.UserId = usr.Id;
                            sa.StepActionMailTo.Add(mt);
                        }
                    }
                    if (step.StepActionMailCC.groups != null)
                    {
                        foreach (var grp in step.StepActionMailCC.groups)
                        {
                            var mt = new TLIstepActionMailTo();
                            mt.StepAction = sa;
                            mt.StepActionId = sa.Id;
                            mt.Type = MailtType.CC;
                            //mt.ActorId = file.ActorId;
                            //mt.IntegrationId = file.IntegrationId;
                            mt.GroupId = grp.Id;
                            //mt.UserId = usr.Id;
                            sa.StepActionMailTo.Add(mt);
                        }
                    }
                    if (step.StepActionMailCC.actors != null)
                    {
                        foreach (var act in step.StepActionMailCC.actors)
                        {
                            var mt = new TLIstepActionMailTo();
                            mt.StepAction = sa;
                            mt.StepActionId = sa.Id;
                            mt.Type = MailtType.CC;
                            mt.ActorId = act.Id;
                            //mt.IntegrationId = file.IntegrationId;
                            //mt.GroupId = file.GroupId;
                            //mt.UserId = usr.Id;
                            sa.StepActionMailTo.Add(mt);
                        }
                    }
                    /*
                    if (step.StepActionGroup.integration != null)
                    {
                        foreach (var integration in step.StepActionGroup.integration)
                        {
                            var mt = new TLIstepActionMailTo();
                            mt.StepAction = sa;
                            mt.StepActionId = sa.Id;
                            mt.Type = MailtType.To;
                            //mt.ActorId = file.ActorId;
                            mt.IntegrationId = integration.Id;
                            //mt.GroupId = file.GroupId;
                            //mt.UserId = usr.Id;
                            sa.StepActionMailTo.Add(mt);
                        }
                    }
                    //*/


                    /*
                    foreach (AddStepActionMailToViewModel mailTo in step.StepActionMailTo)
                    {
                        var mt = new TLIstepActionMailTo();
                        mt.StepAction = sa;
                        mt.StepActionId = sa.Id;
                        mt.ActorId = mailTo.ActorId;
                        //mt.IntegrationId = mailTo.IntegrationId;
                        mt.GroupId = mailTo.GroupId;
                        mt.UserId = mailTo.UserId;
                        mt.Type = mailTo.Type;
                        sa.StepActionMailTo.Add(mt);
                    }
                    //*/
                }
                /*

               if (step.StepActionMailTo != null)
               {


                   if (sa.StepActionMailTo == null)
                   {
                       sa.StepActionMailTo = new List<TLIstepActionMailTo>();
                   }
                   sa.StepActionMailTo.Clear();
                   foreach (ListStepActionMailToViewModel mailTo in step.StepActionMailTo)
                   {
                       var mt = new TLIstepActionMailTo();
                       mt.StepAction = sa;
                       mt.StepActionId = sa.Id;
                       mt.ActorId = mailTo.ActorId;
                       //mt.IntegrationId = mailTo.IntegrationId;
                       mt.GroupId = mailTo.GroupId;
                       mt.UserId = mailTo.UserId;
                       mt.Type = mailTo.Type;
                       sa.StepActionMailTo.Add(mt);
                   }

               }
               //*/
               /*
                if (step.StepActionFileGroup != null)
                {
                    if (sa.StepActionFileGroup == null)
                    {
                        sa.StepActionFileGroup = new List<TLIstepActionFileGroup>();
                    }
                    sa.StepActionFileGroup.Clear();
                    if (step.StepActionFileGroup.users != null)
                    {
                        foreach (var usr in step.StepActionFileGroup.users)
                        {
                            var mt = new TLIstepActionFileGroup();
                            mt.StepAction = sa;
                            mt.StepActionId = sa.Id;
                            //mt.ActorId = file.ActorId;
                            //mt.IntegrationId = file.IntegrationId;
                            //mt.GroupId = file.GroupId;
                            mt.UserId = usr.Id;
                            mt.Active = true;
                            mt.Deleted = false;
                            sa.StepActionFileGroup.Add(mt);
                        }
                    }
                    if (step.StepActionFileGroup.groups != null)
                    {
                        foreach (var grp in step.StepActionFileGroup.groups)
                        {
                            var mt = new TLIstepActionFileGroup();
                            mt.StepAction = sa;
                            mt.StepActionId = sa.Id;
                            //mt.ActorId = file.ActorId;
                            //mt.IntegrationId = file.IntegrationId;
                            mt.GroupId = grp.Id;
                            //mt.UserId = usr.Id;
                            mt.Active = true;
                            mt.Deleted = false;
                            sa.StepActionFileGroup.Add(mt);
                        }
                    }
                    if (step.StepActionFileGroup.actors != null)
                    {
                        foreach (var act in step.StepActionFileGroup.actors)
                        {
                            var mt = new TLIstepActionFileGroup();
                            mt.StepAction = sa;
                            mt.StepActionId = sa.Id;
                            mt.ActorId = act.Id;
                            //mt.IntegrationId = file.IntegrationId;
                            //mt.GroupId = grp.Id;
                            //mt.UserId = usr.Id;
                            mt.Active = true;
                            mt.Deleted = false;
                            sa.StepActionFileGroup.Add(mt);
                        }
                    }
                    if (step.StepActionFileGroup.integration != null)
                    {
                        foreach (var integration in step.StepActionFileGroup.integration)
                        {
                            var mt = new TLIstepActionFileGroup();
                            mt.StepAction = sa;
                            mt.StepActionId = sa.Id;
                            //mt.ActorId = file.ActorId;
                            mt.IntegrationId = integration.Id;
                            //mt.GroupId = grp.Id;
                            //mt.UserId = usr.Id;
                            mt.Active = true;
                            mt.Deleted = false;
                            sa.StepActionFileGroup.Add(mt);
                        }
                    }
                }
                //*/
                if (step.StepActionGroup != null)
                {
                    if (sa.StepActionGroup == null)
                    {
                        var oldData = _unitOfWork.StepActionGroupRepository.GetAllAsQueryable().Where(x => x.StepActionId == sa.Id);
                        foreach (var i in oldData)
                        {
                            _unitOfWork.StepActionGroupRepository.RemoveItem(i);
                        }

                        sa.StepActionGroup = new List<TLIstepActionGroup>();
                    }
                    sa.StepActionGroup.Clear();
                    _unitOfWork.StepActionRepository.Update(sa);
                    _unitOfWork.SaveChanges();
                    if (step.StepActionGroup.users != null)
                    {
                        var usr = step.StepActionGroup.users;
                        //foreach (var usr in step.StepActionGroup.users)
                        //{
                            var mt = new TLIstepActionGroup();
                            mt.StepAction = sa;
                            mt.StepActionId = sa.Id;
                            //mt.ActorId = file.ActorId;
                            //mt.IntegrationId = file.IntegrationId;
                            //mt.GroupId = file.GroupId;
                            mt.UserId = usr.Id;
                            mt.Deleted = false;
                            mt.Active = true;
                            sa.StepActionGroup.Add(mt);
                        //}
                    }
                    if (step.StepActionGroup.groups != null)
                    {
                        var grp = step.StepActionGroup.groups;
                        //foreach (var grp in step.StepActionGroup.groups)
                        //{
                            var mt = new TLIstepActionGroup();
                            mt.StepAction = sa;
                            mt.StepActionId = sa.Id;
                            //mt.ActorId = file.ActorId;
                            //mt.IntegrationId = file.IntegrationId;
                            mt.GroupId = grp.Id;
                            //mt.UserId = usr.Id;
                            mt.Deleted = false;
                            mt.Active = true;
                            sa.StepActionGroup.Add(mt);
                        //}
                    }
                    if (step.StepActionGroup.actors != null)
                    {
                        var act = step.StepActionGroup.actors;
                        //foreach (var act in step.StepActionGroup.actors)
                        //{
                            var mt = new TLIstepActionGroup();
                            mt.StepAction = sa;
                            mt.StepActionId = sa.Id;
                            mt.ActorId = act.Id;
                            //mt.IntegrationId = file.IntegrationId;
                            //mt.GroupId = file.GroupId;
                            //mt.UserId = usr.Id;
                            mt.Deleted = false;
                            mt.Active = true;
                            sa.StepActionGroup.Add(mt);
                        //}
                    }
                    if (step.StepActionGroup.integration != null)
                    {
                        var integration = step.StepActionGroup.integration;
                        //foreach (var integration in step.StepActionGroup.integration)
                        //{
                            var mt = new TLIstepActionGroup();
                            mt.StepAction = sa;
                            mt.StepActionId = sa.Id;
                            //mt.ActorId = file.ActorId;
                            mt.IntegrationId = integration.Id;
                            //mt.GroupId = file.GroupId;
                            //mt.UserId = usr.Id;
                            mt.Deleted = false;
                            mt.Active = true;
                            sa.StepActionGroup.Add(mt);
                        //}

                    }
                }
                if (step.StepActionPart != null)
                {
                    if (sa.StepActionPart == null)
                    {
                        var oldData = _unitOfWork.StepActionPartRepository.GetAllAsQueryable().Where(x => x.StepActionId == sa.Id);
                        foreach (var i in oldData)
                        {
                            _unitOfWork.StepActionPartRepository.RemoveItem(i);
                        }

                        sa.StepActionPart = new List<TLIstepActionPart>();
                    }
                    sa.StepActionPart.Clear();
                    _unitOfWork.StepActionRepository.Update(sa);
                    _unitOfWork.SaveChanges();
                    foreach (var part in step.StepActionPart)
                    {
                        if (part.PartId == 0)  // site
                        {
                            if (sa.ActionId == (int)ActionType.InsertData || sa.ActionId == (int)ActionType.UpdateData || sa.ActionId == (int)ActionType.UploadFile) //|| sa.ActionId == (int)ActionType.TelecomValidation || sa.ActionId == (int)ActionType.CivilValidation
                            {
                                if (sa.StepActionFileGroup == null)
                                {
                                    sa.StepActionFileGroup = new List<TLIstepActionFileGroup>();
                                }
                                sa.StepActionFileGroup.Clear();                                
                                /*
                                if (part.StepActionPartGroup != null)
                                {

                                    if (part.StepActionPartGroup.users != null && part.StepActionPartGroup.users.Count > 0)
                                    {
                                        foreach (var pg in part.StepActionPartGroup.users)
                                        {
                                            var mt = new TLIstepActionFileGroup();
                                            mt.StepAction = sa;
                                            mt.StepActionId = sa.Id;
                                            //mt.ActorId = pg.Id;
                                            mt.UserId = pg.Id;
                                            //mt.GroupId = pg.Id;
                                            //mt.IntegrationId = pg.Id;
                                            mt.Active = true;
                                            mt.Deleted = false;
                                            sa.StepActionFileGroup.Add(mt);
                                        }
                                    }
                                    if (part.StepActionPartGroup.groups != null && part.StepActionPartGroup.groups.Count > 0)
                                    {
                                        foreach (var pg in part.StepActionPartGroup.groups)
                                        {
                                            var mt = new TLIstepActionFileGroup();
                                            mt.StepAction = sa;
                                            mt.StepActionId = sa.Id;
                                            //mt.ActorId = pg.Id;
                                            //mt.UserId = pg.Id;
                                            mt.GroupId = pg.Id;
                                            //mt.IntegrationId = pg.Id;
                                            mt.Active = true;
                                            mt.Deleted = false;
                                            sa.StepActionFileGroup.Add(mt);
                                        }
                                    }
                                    if (part.StepActionPartGroup.actors != null && part.StepActionPartGroup.actors.Count > 0)
                                    {
                                        foreach (var pg in part.StepActionPartGroup.actors)
                                        {
                                            var mt = new TLIstepActionFileGroup();
                                            mt.StepAction = sa;
                                            mt.StepActionId = sa.Id;
                                            mt.ActorId = pg.Id;
                                            //mt.UserId = pg.Id;
                                            //mt.GroupId = pg.Id;
                                            //mt.IntegrationId = pg.Id;
                                            mt.Active = true;
                                            mt.Deleted = false;
                                            sa.StepActionFileGroup.Add(mt);
                                        }
                                    }
                                    if (part.StepActionPartGroup.integration != null && part.StepActionPartGroup.integration.Count > 0)
                                    {
                                        foreach (var pg in part.StepActionPartGroup.integration)
                                        {
                                            var mt = new TLIstepActionFileGroup();
                                            mt.StepAction = sa;
                                            mt.StepActionId = sa.Id;
                                            //mt.ActorId = pg.Id;
                                            //mt.UserId = pg.Id;
                                            //mt.GroupId = pg.Id;
                                            mt.IntegrationId = pg.Id;
                                            mt.Active = true;
                                            mt.Deleted = false;
                                            sa.StepActionFileGroup.Add(mt);
                                        }
                                    }
                                }
                                //*/
                                var mt = new TLIstepActionFileGroup();
                                mt.StepAction = sa;
                                mt.StepActionId = sa.Id;
                                //mt.ActorId = pg.Id;
                                //mt.UserId = pg.Id;
                                //mt.GroupId = pg.Id;
                                //mt.IntegrationId = pg.Id;
                                mt.Active = true;
                                mt.Deleted = false;
                                sa.StepActionFileGroup.Add(mt);
                                sa.AllowUploadFile = part.AllowUploadFile;
                                sa.UploadFileIsMandatory = part.UploadFileIsMandatory;
                            }
                        }
                        else
                        if (part.PartId == -1) // all parts
                        {
                            if (sa.ActionId == (int)ActionType.InsertData || sa.ActionId == (int)ActionType.UpdateData || sa.ActionId == (int)ActionType.UploadFile || sa.ActionId == (int)ActionType.TelecomValidation || sa.ActionId == (int)ActionType.CivilValidation) //
                            {
                                int count = 0;
                                var PartsList = (_unitOfWork.PartRepository.GetAllAsQueryable(out count)).ToList();
                                foreach (TLIpart p in PartsList)
                                {
                                    var mt = new TLIstepActionPart();
                                    mt.StepActionPartGroup = new List<TLIstepActionPartGroup>();
                                    mt.StepAction = sa;
                                    mt.StepActionId = sa.Id;
                                    mt.AllowUploadFile = part.AllowUploadFile;
                                    mt.UploadFileIsMandatory = part.UploadFileIsMandatory;
                                    mt.PartId = p.Id;
                                    /*
                                    if (part.StepActionPartGroup != null)
                                    {
                                        foreach (var pg in part.StepActionPartGroup.users)
                                        {
                                            var partg = new TLIstepActionPartGroup();
                                            partg.StepActionPartId = mt.Id;
                                            //partg.ActorId = pg.Id;
                                            partg.UserId = pg.Id;
                                            //partg.GroupId = pg.Id;
                                            //partg.IntegrationId = pg.Id;
                                            mt.StepActionPartGroup.Add(partg);
                                        }
                                        foreach (var pg in part.StepActionPartGroup.groups)
                                        {
                                            var partg = new TLIstepActionPartGroup();
                                            partg.StepActionPartId = mt.Id;
                                            //partg.ActorId = pg.Id;
                                            //partg.UserId = pg.Id;
                                            partg.GroupId = pg.Id;
                                            //partg.IntegrationId = pg.Id;
                                            mt.StepActionPartGroup.Add(partg);
                                        }
                                        foreach (var pg in part.StepActionPartGroup.actors)
                                        {
                                            var partg = new TLIstepActionPartGroup();
                                            partg.StepActionPartId = mt.Id;
                                            partg.ActorId = pg.Id;
                                            //partg.UserId = pg.Id;
                                            //partg.GroupId = pg.Id;
                                            //partg.IntegrationId = pg.Id;
                                            mt.StepActionPartGroup.Add(partg);
                                        }
                                        foreach (var pg in part.StepActionPartGroup.integration)
                                        {
                                            var partg = new TLIstepActionPartGroup();
                                            partg.StepActionPartId = mt.Id;
                                            //partg.ActorId = pg.Id;
                                            //partg.UserId = pg.Id;
                                            //partg.GroupId = pg.Id;
                                            partg.IntegrationId = pg.Id;
                                            mt.StepActionPartGroup.Add(partg);
                                        }
                                    }
                                    //*/
                                    mt.Active = true;
                                    mt.Deleted = false;
                                    sa.StepActionPart.Add(mt);
                                }



                            }
                        }
                        else
                        {




                            var mt = new TLIstepActionPart();
                            mt.StepActionPartGroup = new List<TLIstepActionPartGroup>();
                            mt.StepAction = sa;
                            mt.StepActionId = sa.Id;
                            mt.AllowUploadFile = part.AllowUploadFile;
                            mt.UploadFileIsMandatory = part.UploadFileIsMandatory;
                            mt.PartId = part.PartId;
                            var partx = _unitOfWork.PartRepository.GetByID(part.PartId);
                            /*
                            if (part.StepActionPartGroup != null)
                            {
                                foreach (var pg in part.StepActionPartGroup.users)
                                {
                                    var partg = new TLIstepActionPartGroup();
                                    partg.StepActionPartId = mt.Id;
                                    //partg.ActorId = pg.Id;
                                    partg.UserId = pg.Id;
                                    //partg.GroupId = pg.Id;
                                    //partg.IntegrationId = pg.Id;
                                    mt.StepActionPartGroup.Add(partg);
                                }
                                foreach (var pg in part.StepActionPartGroup.groups)
                                {
                                    var partg = new TLIstepActionPartGroup();
                                    partg.StepActionPartId = mt.Id;
                                    //partg.ActorId = pg.Id;
                                    //partg.UserId = pg.Id;
                                    partg.GroupId = pg.Id;
                                    //partg.IntegrationId = pg.Id;
                                    mt.StepActionPartGroup.Add(partg);
                                }
                                foreach (var pg in part.StepActionPartGroup.actors)
                                {
                                    var partg = new TLIstepActionPartGroup();
                                    partg.StepActionPartId = mt.Id;
                                    partg.ActorId = pg.Id;
                                    //partg.UserId = pg.Id;
                                    //partg.GroupId = pg.Id;
                                    //partg.IntegrationId = pg.Id;
                                    mt.StepActionPartGroup.Add(partg);
                                }
                                foreach (var pg in part.StepActionPartGroup.integration)
                                {
                                    var partg = new TLIstepActionPartGroup();
                                    partg.StepActionPartId = mt.Id;
                                    //partg.ActorId = pg.Id;
                                    //partg.UserId = pg.Id;
                                    //partg.GroupId = pg.Id;
                                    partg.IntegrationId = pg.Id;
                                    mt.StepActionPartGroup.Add(partg);
                                }
                            }
                            //*/
                            mt.Active = true;
                            mt.Deleted = false;
                            sa.StepActionPart.Add(mt);
                        }
                    }
                }
                if (step.StepActionOption != null)
                {
                    if (sa.StepActionOption == null)
                    {
                        var oldData = _unitOfWork.StepActionOptionRepository.GetAllAsQueryable().Where(x => x.StepActionId == sa.Id);
                        foreach (var i in oldData)
                        {
                            _unitOfWork.StepActionOptionRepository.RemoveItem(i);
                        }

                        sa.StepActionOption = new List<TLIstepActionOption>();
                    }
                    sa.StepActionOption.Clear();
                    _unitOfWork.StepActionRepository.Update(sa);
                    _unitOfWork.SaveChanges();
                    foreach (var option in step.StepActionOption)
                    {
                        var mt = new TLIstepActionOption();
                        mt.StepAction = sa;
                        mt.StepActionId = sa.Id;
                        mt.ActionOptionId = option.ActionOptionId;
                        mt.AllowNote = option.AllowNote;
                        mt.NoteIsMandatory = option.NoteIsMandatory;
                        //mt.NextStepActionId = option.NextStepActionId;
                        mt.NextStepActions = new List<TLInextStepAction>();
                        foreach (int next in option.NextStepActions)
                        {
                            TLInextStepAction nextAction = new TLInextStepAction();
                            nextAction.StepActionId = sa.Id;
                            nextAction.NextStepActionId = next;
                            nextAction.StepActionOptionId = mt.Id;
                            mt.NextStepActions.Add(nextAction);
                        }
                        mt.OrderStatusId = option.OrderStatusId;
                        mt.ItemStatusId = option.ItemStatusId;
                        mt.Deleted = false;
                        sa.StepActionOption.Add(mt);
                    }
                }
                if (step.StepActionItemOption != null)
                {
                    if (sa.StepActionItemOption == null)
                    {
                        var oldData = _unitOfWork.StepActionItemOptionRepository.GetAllAsQueryable().Where(x => x.StepActionId == sa.Id);
                        foreach (var i in oldData)
                        {
                            _unitOfWork.StepActionItemOptionRepository.RemoveItem(i);
                        }

                        sa.StepActionItemOption = new List<TLIstepActionItemOption>();
                    }
                    sa.StepActionItemOption.Clear();
                    _unitOfWork.StepActionRepository.Update(sa);
                    _unitOfWork.SaveChanges();
                    foreach (var option in step.StepActionItemOption)
                    {
                        var mt = new TLIstepActionItemOption();
                        mt.StepAction = sa;
                        mt.StepActionId = sa.Id;
                        mt.ActionItemOptionId = option.ActionItemOptionId;
                        //mt.NextStepActionId = option.NextStepActionId;
                        mt.NextStepActions = new List<TLInextStepAction>();
                        foreach (int next in option.NextStepActions)
                        {
                            TLInextStepAction nextAction = new TLInextStepAction();
                            nextAction.NextStepActionId = next;
                            nextAction.StepActionId = sa.Id;
                            nextAction.StepActionItemOptionId = mt.Id;
                            mt.NextStepActions.Add(nextAction);
                        }
                        mt.OrderStatusId = option.OrderStatusId;
                        //mt.ItemStatusId = option.ItemStatusId;
                        if (option.StepActionItemStatus != null)
                        {
                            mt.StepActionItemStatus = new List<TLIstepActionItemStatus>();
                            foreach (var pg in option.StepActionItemStatus)
                            {
                                var partg = new TLIstepActionItemStatus();
                                partg.StepActionItemOptionId = mt.Id;
                                partg.IncomingItemStatusId = pg.IncomingItemStatusId;
                                partg.OutgoingItemStatusId = pg.OutgoingItemStatusId;
                                mt.StepActionItemStatus.Add(partg);
                            }
                        }
                        sa.StepActionItemOption.Add(mt);
                    }
                }
                //sa.sequence = sa.Id;
                _unitOfWork.StepActionRepository.Update(sa);
                _unitOfWork.SaveChanges();
                return new Response<StepActionWithNamesViewModel>(StepActionWithNames(MapStepAction(sa)));
            }
            catch (Exception err)
            {
                
                return new Response<StepActionWithNamesViewModel>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
        }
        public Response<ListStepActionViewModel> DelStepAction(int Id)
        {
            var entity = _unitOfWork.StepActionRepository.GetAllAsQueryable().Where(x => x.Id == Id && x.Deleted == false).SingleOrDefault();
            entity.Deleted = true;
            entity.DateDeleted = DateTime.Now;
            _unitOfWork.StepActionRepository.Update(entity);
            _unitOfWork.SaveChanges();
            return new Response<ListStepActionViewModel>(true, _mapper.Map<ListStepActionViewModel>(entity), null, null, (int)Helpers.Constants.ApiReturnCode.success);

        }


        public StepActionWithNamesViewModel StepActionWithNames(ListStepActionViewModel action)
        {
            StepActionWithNamesViewModel newAction = _mapper.Map<StepActionWithNamesViewModel>(action);
            /*
            if (newAction.ItemStatusId != null)
            {
                var entity = _unitOfWork.ItemStatusRepository.GetByID((int)newAction.ItemStatusId);
                newAction.ItemStatusName = entity.Name;
            }
            //*/
            if (newAction.OrderStatusId != null)
            {
                var entity = _unitOfWork.OrderStatusListRepository.GetByID((int)newAction.OrderStatusId);
                newAction.OrderStatusName = entity.Name;
            }
            /*
            if (action.IncomItemStatus != null)
            {
                if (newAction.IncomItemStatus == null)
                {
                    newAction.IncomItemStatus = new List<ListStepActionItemStatusWiNameViewModel>();
                }
                foreach (var item in action.IncomItemStatus)
                {
                    ListStepActionItemStatusWiNameViewModel it = new ListStepActionItemStatusWiNameViewModel();
                    it.Id = item.Id;
                    it.Name = item.Name;
                    newAction.IncomItemStatus.Add(it);
                }
            }
            //*/
            if(action.StepActionFileGroup != null && action.StepActionFileGroup.Count>0)
            {
                if (newAction.StepActionPart == null)
                {
                    newAction.StepActionPart = new List<AddStepActionPartViewModel>();
                }
                var part = new AddStepActionPartViewModel();
                part.PartId = 0;
                part.AllowUploadFile = action.AllowUploadFile;
                part.UploadFileIsMandatory = action.UploadFileIsMandatory;
                /*
                part.StepActionPartGroup = new StepActionGroupsViewModel();
                foreach (var act in action.StepActionFileGroup)
                {
                    if (act.UserId != null)
                    {
                        if (part.StepActionPartGroup.users == null)
                        {
                            part.StepActionPartGroup.users = new List<UserViewModel>();
                        }
                        part.StepActionPartGroup.users.Add(_mapper.Map<UserViewModel>(_unitOfWork.UserRepository.GetByID((int)act.UserId)));

                    }
                    if (act.GroupId != null)
                    {
                        if (part.StepActionPartGroup.groups == null)
                        {
                            part.StepActionPartGroup.groups = new List<GroupViewModel>();
                        }
                        part.StepActionPartGroup.groups.Add(_mapper.Map<GroupViewModel>(_unitOfWork.GroupRepository.GetByID((int)act.GroupId)));
                    }
                    if (act.ActorId != null)
                    {
                        if (part.StepActionPartGroup.actors == null)
                        {
                            part.StepActionPartGroup.actors = new List<ActorViewModel>();
                        }
                        part.StepActionPartGroup.actors.Add(_mapper.Map<ActorViewModel>(_unitOfWork.ActorRepository.GetByID((int)act.ActorId)));
                    }
                    if (act.IntegrationId != null)
                    {
                        if (part.StepActionPartGroup.integration == null)
                        {
                            part.StepActionPartGroup.integration = new List<IntegrationViewModel>();
                        }
                        part.StepActionPartGroup.integration.Add(_mapper.Map<IntegrationViewModel>(_unitOfWork.IntegrationRepository.GetByID((int)act.IntegrationId)));
                    }
                }
                //*/
                newAction.StepActionPart.Add(part);
                //newAction.StepActionFileGroup = FillActorsName(_mapper.Map<List<AddStepActionGroupViewModel>>(action.StepActionFileGroup));
            }
            //*
            if(action.StepActionGroup != null)
            {
                if (newAction.StepActionGroup == null)
                {
                    newAction.StepActionGroup = new StepActionGroupViewModel();
                    //newAction.StepActionGroup = new StepActionGroupsViewModel();
                }
                foreach (var act in action.StepActionGroup)
                {
                    if (act.UserId != null)
                    {
                        //if (newAction.StepActionGroup.users == null)
                        //{
                        //    newAction.StepActionGroup.users = new List<UserViewModel>();
                        //}
                        //newAction.StepActionGroup.users.Add(_mapper.Map<UserViewModel>(_unitOfWork.UserRepository.GetByID((int)act.UserId)));
                        newAction.StepActionGroup.users = _mapper.Map<UserViewModel>(_unitOfWork.UserRepository.GetByID((int)act.UserId));
                    }
                    if (act.GroupId != null)
                    {
                        //if (newAction.StepActionGroup.groups == null)
                        //{
                        //    newAction.StepActionGroup.groups = new List<GroupViewModel>();
                        //}
                        //newAction.StepActionGroup.groups.Add(_mapper.Map<GroupViewModel>(_unitOfWork.GroupRepository.GetByID((int)act.GroupId)));
                        newAction.StepActionGroup.groups = _mapper.Map<GroupViewModel>(_unitOfWork.GroupRepository.GetByID((int)act.GroupId));
                    }
                    if (act.ActorId != null)
                    {
                        //if (newAction.StepActionGroup.actors == null)
                        //{
                        //    newAction.StepActionGroup.actors = new List<ActorViewModel>();
                        //}
                        //newAction.StepActionGroup.actors.Add(_mapper.Map<ActorViewModel>(_unitOfWork.ActorRepository.GetByID((int)act.ActorId)));
                        newAction.StepActionGroup.actors = (_mapper.Map<ActorViewModel>(_unitOfWork.ActorRepository.GetByID((int)act.ActorId)));
                    }
                    if (act.IntegrationId != null)
                    {
                        //if (newAction.StepActionGroup.integration == null)
                        //{
                        //    newAction.StepActionGroup.integration = new List<IntegrationViewModel>();
                        //}
                        //newAction.StepActionGroup.integration.Add(_mapper.Map<IntegrationViewModel>(_unitOfWork.IntegrationRepository.GetByID((int)act.IntegrationId)));
                        newAction.StepActionGroup.integration = (_mapper.Map<IntegrationViewModel>(_unitOfWork.IntegrationRepository.GetByID((int)act.IntegrationId)));
                    }
                }

                //newAction.StepActionGroup = FillActorsName(_mapper.Map<List<AddStepActionGroupViewModel>>(action.StepActionGroup));
            }
            //*/
            /*
            if (action.StepActionMailTo != null)
            {
                if (newAction.StepActionMailTo == null) {
                    newAction.StepActionMailTo = new List<ListStepActionMailToWithNameViewModel>();
                }
                foreach (var act in action.StepActionMailTo)
                {
                    ListStepActionMailToWithNameViewModel actor = new ListStepActionMailToWithNameViewModel();
                    actor.Id = act.Id;
                    actor.StepActionId = act.StepActionId;
                    actor.Type = act.Type;
                    actor.UserId = act.UserId;
                    actor.GroupId = act.GroupId;
                    actor.ActorId = act.ActorId;
                    //mailTo.IntegrationId = act.IntegrationId;
                    if (act.UserId != null)
                    {
                        var entity = _unitOfWork.UserRepository.GetByID((int)act.UserId);
                        actor.UserName = entity.FirstName + " " + entity.LastName;
                    }
                    if (act.GroupId != null)
                    {
                        var entity = _unitOfWork.GroupRepository.GetByID((int)act.GroupId);
                        actor.GroupName = entity.Name;
                    }
                    if (act.ActorId != null)
                    {
                        var entity = _unitOfWork.ActorRepository.GetByID((int)act.ActorId);
                        actor.ActorName = entity.Name;
                    }
                    //if (act.IntegrationId != null)
                    //{
                    //    var entity = _unitOfWork.IntegrationRepository.GetByID((int)act.IntegrationId);
                    //    actor.IntegrationName = entity.Name;
                    //}
                    newAction.StepActionMailTo.Add(actor);
                }
                //newAction.StepActionMailTo = FillActorsName(_mapper.Map<List<AddStepActionGroupViewModel>>(action.StepActionMailTo));
            }
            //*/
            if (action.StepActionItemOption != null)
            {
                if (newAction.StepActionItemOption == null)
                {
                    newAction.StepActionItemOption = new List<ListStepActionItemOptionWithNameViewModel>();
                }
                foreach (var item in action.StepActionItemOption)
                {
                    ListStepActionItemOptionWithNameViewModel newItem = new ListStepActionItemOptionWithNameViewModel();
                    //newItem.Id = item.Id;
                    //newItem.StepActionId = item.StepActionId;
                    newItem.ActionItemOptionId = item.ActionItemOptionId;
                    //newItem.NextStepActionId = item.NextStepActionId;
                    newItem.NextStepActions = item.NextStepActions;
                    newItem.OrderStatusId = item.OrderStatusId;
                    newItem.AllowNote = item.AllowNote;
                    newItem.NoteIsMandatory = item.NoteIsMandatory;
                    //*
                    if (item.StepActionItemStatus != null)
                    {
                        if (newItem.StepActionItemStatus == null)
                        {
                            newItem.StepActionItemStatus = new List<AddStepActionItemStatusWithNameViewModel>();
                        }
                        foreach (var i in item.StepActionItemStatus)
                        {
                            AddStepActionItemStatusWithNameViewModel it = new AddStepActionItemStatusWithNameViewModel();
                            it.IncomingItemStatus = _mapper.Map<ListStepActionItemStatusWiNameViewModel>(_unitOfWork.ItemStatusRepository.GetByID((int)i.IncomingItemStatusId));
                            it.OutgoingItemStatus = _mapper.Map<ListStepActionItemStatusWiNameViewModel>(_unitOfWork.ItemStatusRepository.GetByID((int)i.OutgoingItemStatusId));
                            newItem.StepActionItemStatus.Add(it);
                        }
                    }
                    if(item.NextStepActions!=null && item.NextStepActions.Count > 0)
                    {
                        newItem.NextStepActions = item.NextStepActions;
                    }
                        
                    //*/
                    if (item.OrderStatusId != null)
                    {
                        var entity = _unitOfWork.OrderStatusListRepository.GetByID((int)item.OrderStatusId);
                        if (entity != null)
                            newItem.OrderStatusName = entity.Name;
                    }

                    var entity2 = _unitOfWork.ActionItemOptionListRepository.GetByID(item.ActionItemOptionId);
                    if (entity2 != null)
                        newItem.OrderStatusName = entity2.Name;
                    newAction.StepActionItemOption.Add(newItem);
                }
            }
            if (action.StepActionOption != null)
            {
                if (newAction.StepActionOption == null)
                {
                    newAction.StepActionOption = new List<ListStepActionOptionWithNameViewModel>();
                }
                foreach (var item in action.StepActionOption)
                {
                    ListStepActionOptionWithNameViewModel newItem = new ListStepActionOptionWithNameViewModel();
                    //newItem.Id = item.Id;
                    //newItem.StepActionId = item.StepActionId;
                    //newItem.NextStepActionId = item.NextStepActionId;
                    newItem.NextStepActions = item.NextStepActions;
                    newItem.ActionOptionId = item.ActionOptionId;
                    var option= _unitOfWork.OptionRepository.GetByID(item.ActionOptionId);
                    newItem.ActionOptionName = option.Name;
                    if (item.ItemStatusId != null)
                    {
                        newItem.ItemStatus = _mapper.Map<ListItemStatusViewModel>(_unitOfWork.ItemStatusRepository.GetByID((int)item.ItemStatusId));
                    }
                    if (item.OrderStatusId!=null)
                    {
                        newItem.OrderStatus = _mapper.Map<OrderStatusViewModel>(_unitOfWork.OrderStatusListRepository.GetByID((int)item.OrderStatusId));
                    }
                    
                    newItem.AllowNote = item.AllowNote;
                    newItem.NoteIsMandatory = item.NoteIsMandatory;
                    //*
                    if (item.ItemStatusId != null)
                    {
                        newItem.ItemStatus = _mapper.Map<ListItemStatusViewModel>(_unitOfWork.ItemStatusRepository.GetByID((int)item.ItemStatusId));
                    }
                    //*/
                    if (item.OrderStatusId != null)
                    {
                        newItem.OrderStatus = _mapper.Map<OrderStatusViewModel>(_unitOfWork.OrderStatusListRepository.GetByID((int)item.OrderStatusId));
                    }
                    newAction.StepActionOption.Add(newItem);
                }
            }
            return newAction;
        }


        //-------------------------------------------  end of StepAction


    }
}
