using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLIS_DAL.Helper;
using TLIS_DAL.Helper.Filters;
using TLIS_DAL.Helpers;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModels.ActionDTOs;
using TLIS_DAL.ViewModels.ActionItemOptionDTOs;
using TLIS_DAL.ViewModels.ConditionDTOs;
using TLIS_DAL.ViewModels.ItemStatusDTOs;
using TLIS_DAL.ViewModels.MailTemplateDTOs;
using TLIS_DAL.ViewModels.OptionDTOs;
using TLIS_DAL.ViewModels.OrderStatusDTOs;
using TLIS_DAL.ViewModels.PartDTOs;
using TLIS_DAL.ViewModels.SubOptionDTOs;
using TLIS_DAL.ViewModels.TaskStatusDTOs;
using TLIS_Repository.Base;
using TLIS_Repository.Repositories;
using TLIS_Service.Helpers;
using TLIS_Service.IService;

namespace TLIS_Service.Services
{
    public class WorkFlowSettingService : IWorkFlowSettingService
    {
        IUnitOfWork _unitOfWork;
        IServiceCollection _services;
        private IMapper _mapper;
        public WorkFlowSettingService(IUnitOfWork unitOfWork, IServiceCollection services, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _services = services;
            ServiceProvider serviceProvider = _services.BuildServiceProvider();
            _mapper = mapper;
        }

        //------------- Action Item option
        public async Task<Response<List<ActionItemOptionListViewModel>>> GetAllActionItemOptions()
        {
            try
            {
                var actionOptionItemTemplate = (await _unitOfWork.ActionItemOptionListRepository.GetAllAsync()); //.Where(x => x.Deleted == false);//; //
                var actionItemOptionModel = _mapper.Map<List<ActionItemOptionListViewModel>>(actionOptionItemTemplate);
                return new Response<List<ActionItemOptionListViewModel>>(true, actionItemOptionModel, null, null, (int)Helpers.Constants.ApiReturnCode.success);
            }
            catch (Exception err)
            {
                
                return new Response<List<ActionItemOptionListViewModel>>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }

        }
        public async Task<Response<ActionItemOptionListViewModel>> GetActionItemOptionById(int Id)
        {
            try
            {
                var actionOptionItemTemplate = (await _unitOfWork.ActionItemOptionListRepository.SingleOrDefaultAsync(x => x.Deleted == false&& x.Id == Id));
                var actionItemOptionModel = _mapper.Map<ActionItemOptionListViewModel>(actionOptionItemTemplate);
                return new Response<ActionItemOptionListViewModel>(true, actionItemOptionModel, null, null, (int)Helpers.Constants.ApiReturnCode.success);
            }
            catch (Exception err)
            {
                
                return new Response<ActionItemOptionListViewModel>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }

        }
        public Response<ActionItemOptionEditViewModel> AddActionItemOption(ActionItemOptionAddViewModel actionItemOption)
        {
            try
            {
                if (CheckActionItemOptionName(actionItemOption.Name, null))
                {
                    TLIactionItemOption entity = new TLIactionItemOption();
                    entity.Name = actionItemOption.Name;
                    entity.ActionId = actionItemOption.ActionId;
                    entity.Deleted = false;
                    _unitOfWork.ActionItemOptionListRepository.Add(entity);
                    _unitOfWork.SaveChanges();
                    return new Response<ActionItemOptionEditViewModel>(_mapper.Map<ActionItemOptionEditViewModel>(entity));
                }
                else
                {
                    return new Response<ActionItemOptionEditViewModel>(true, null, null, $"This Action Item Option {actionItemOption.Name} is already exists", (int)Helpers.Constants.ApiReturnCode.fail);
                }
            }
            catch (Exception err)
            {
                
                return new Response<ActionItemOptionEditViewModel>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }

        }
        public Response<ActionItemOptionEditViewModel> EditActionItemOption(ActionItemOptionEditViewModel actionItemOption)
        {
            try
            {
                if (CheckActionItemOptionName(actionItemOption.Name, actionItemOption.Id))
                {
                    TLIactionItemOption entity = _unitOfWork.ActionItemOptionListRepository.GetAllAsQueryable().Where(x => x.Id == actionItemOption.Id).SingleOrDefault();
                    entity.Name = actionItemOption.Name;
                    entity.ActionId = actionItemOption.ActionId;
                    entity.StepActinItemOptions = actionItemOption.StepActinItemOptions;
                    _unitOfWork.ActionItemOptionListRepository.Update(entity);
                    _unitOfWork.SaveChanges();
                    return new Response<ActionItemOptionEditViewModel>(_mapper.Map<ActionItemOptionEditViewModel>(entity));
                }
                else
                {
                    return new Response<ActionItemOptionEditViewModel>(true, null, null, $"This Action Item Option {actionItemOption.Name} is already exitsts", (int)Helpers.Constants.ApiReturnCode.fail);
                }
            }
            catch (Exception err)
            {
                
                return new Response<ActionItemOptionEditViewModel>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }

        }
        private bool CheckActionItemOptionName(string Name, int? Id)
        {
            int cnt = 1;
            if (Id != null)
            {
                cnt = _unitOfWork.ActionItemOptionListRepository.GetAllAsQueryable().Where(x => x.Name == Name && x.Id != Id).ToList().Count;
            }
            else
            {
                cnt = _unitOfWork.ActionItemOptionListRepository.GetAllAsQueryable().Where(x => x.Name == Name).ToList().Count;
            }
            if (cnt == 0)
            {
                return true;
            }
            return false;
        }

        //------------- End of action Item option

        //-------------------------------------------  Mail Template

        /// <summary>
        /// return all mail template that their deleted flag is false
        /// </summary>
        public async Task<Response<List<MailTemplateViewModel>>> GetAllMailTemplates()
        {
            try
            {
                var MailTemplate = (await _unitOfWork.MailTemplateRepository.GetAllAsync()).Where(x => x.Deleted == false);//; //
                var MailTemplateModel = _mapper.Map<List<MailTemplateViewModel>>(MailTemplate);
                return new Response<List<MailTemplateViewModel>>(true, MailTemplateModel, null, null, (int)Helpers.Constants.ApiReturnCode.success);
            }
            catch (Exception err)
            {
                
                return new Response<List<MailTemplateViewModel>>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }

        }

        public Response<MailTemplateViewModel> AddMailTemplate(AddMailTemplateViewModel mailTemplate)
        {
            try
            {
                if (CheckMailTemplateName(mailTemplate.Name, null))
                {
                    _unitOfWork.MailTemplateRepository.Add(_mapper.Map<TLImailTemplate>(mailTemplate));
                    _unitOfWork.SaveChanges();
                    return new Response<MailTemplateViewModel>();
                }
                else
                {
                    return new Response<MailTemplateViewModel>(true, null, null, $"This Mail Template {mailTemplate.Name} is already exists", (int)Helpers.Constants.ApiReturnCode.fail);
                }
            }
            catch (Exception err)
            {
                
                return new Response<MailTemplateViewModel>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }

        }


        public async Task<Response<MailTemplateViewModel>> UpdateMailTemplate(MailTemplateViewModel mailTemplate)
        {
            try
            {
                if (CheckMailTemplateName(mailTemplate.Name, mailTemplate.Id))
                {
                    await _unitOfWork.MailTemplateRepository.UpdateItem(mailTemplate);
                    await _unitOfWork.SaveChangesAsync();
                    return new Response<MailTemplateViewModel>();
                }
                else
                {
                    return new Response<MailTemplateViewModel>(true, null, null, $"This Mail Template {mailTemplate.Name} is already exitsts", (int)Helpers.Constants.ApiReturnCode.fail);
                }
            }
            catch (Exception err)
            {
                
                return new Response<MailTemplateViewModel>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }

        }

        /// <summary>
        /// check if there is a MAil tempalte already exists on Database by same name
        /// </summary>
        /// <param name="Name">this name must be unique in database</param>
        /// <returns>result will be true in case of this name is not existon database or exist for same id. otherwise, response would be false </returns>
        private bool CheckMailTemplateName(string Name, int? Id)
        {
            int cnt = 1;
            if (Id != null)
            {
                cnt = _unitOfWork.MailTemplateRepository.GetAllAsQueryable().Where(x => x.Name == Name && x.Id != Id).ToList().Count;
            }
            else
            {
                cnt = _unitOfWork.MailTemplateRepository.GetAllAsQueryable().Where(x => x.Name == Name ).ToList().Count;
            }
            if (cnt == 0)
            {
                return true;
            }
            return false;
        }
        //-------------------------------------------  end of mail template

        //-------------------------------------------  Parts

        public Response<List<ListPartViewModel>> GetAllParts()
        {
            try
            {
                int count = 0;
                var OrderStatusList = (_unitOfWork.PartRepository.GetAllAsQueryable(out count)).ToList();

                var OrderStatusListModel = _mapper.Map<List<ListPartViewModel>>(OrderStatusList);
                return new Response<List<ListPartViewModel>>(true, OrderStatusListModel, null, null, (int)Helpers.Constants.ApiReturnCode.success, count);
            }
            catch (Exception err)
            {
                
                return new Response<List<ListPartViewModel>>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }

        }
        //-------------------------------------------  end of Parts

        //-------------------------------------------  Order Status

        public Response<List<OrderStatusViewModel>> GetAllOrderStatus()
        {
            try
            {
                int count = 0;
                var OrderStatusList = ( _unitOfWork.OrderStatusListRepository.GetAll(out count)).Where(x => x.Deleted == false);
                var OrderStatusListModel = _mapper.Map<List<OrderStatusViewModel>>(OrderStatusList);
                return new Response<List<OrderStatusViewModel>>(true, OrderStatusListModel, null, null, (int)Helpers.Constants.ApiReturnCode.success, count);
            }
            catch (Exception err)
            {
                
                return new Response<List<OrderStatusViewModel>>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }

        }


        public Response<OrderStatusViewModel> GetOrderStatusById(int Id)
        {
            try
            {
                var OrderStatusListModel = _mapper.Map<OrderStatusViewModel>(_unitOfWork.OrderStatusListRepository.GetAllAsQueryable().Where(x => x.Id == Id && x.Deleted == false).SingleOrDefault());
                return new Response<OrderStatusViewModel>(true, OrderStatusListModel, null, null, (int)Helpers.Constants.ApiReturnCode.success);
            }
            catch (Exception err)
            {
                
                return new Response<OrderStatusViewModel>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }

        }

        public Response<OrderStatusViewModel> AddOrderStatus(OrderStatusAddViewModel orderStatus)
        {
            try
            {
                if (CheckOrderStatusName(orderStatus.Name, null))
                {
                    if (orderStatus.IsDefault == null) orderStatus.IsDefault = false;
                    if (orderStatus.IsFinish == null) orderStatus.IsFinish = false;
                     TLIorderStatus order= _mapper.Map<TLIorderStatus>(orderStatus);
                    _unitOfWork.OrderStatusListRepository.Add(order);
                    _unitOfWork.SaveChanges();
                    return new Response<OrderStatusViewModel>(_mapper.Map<OrderStatusViewModel>(order));
                }
                else
                {
                    return new Response<OrderStatusViewModel>(true, null, null, $"This Mail Template {orderStatus.Name} is already exists", (int)Helpers.Constants.ApiReturnCode.fail);
                }
            }
            catch (Exception err)
            {
                
                return new Response<OrderStatusViewModel>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }

        }


        public Response<OrderStatusViewModel> UpdateOrderStatus(OrderStatusViewModel orderStatus)
        {
            try
            {
                if (CheckOrderStatusName(orderStatus.Name, orderStatus.Id))
                {
                    if (orderStatus.IsDefault == null) orderStatus.IsDefault = false;
                    if (orderStatus.IsFinish == null) orderStatus.IsFinish = false;
                    TLIorderStatus order = _mapper.Map<TLIorderStatus>(orderStatus);
                    _unitOfWork.OrderStatusListRepository.Update(order);
                    _unitOfWork.SaveChanges();
                    return new Response<OrderStatusViewModel>(_mapper.Map<OrderStatusViewModel>(order));
                }
                else
                {
                    return new Response<OrderStatusViewModel>(true, null, null, $"This Mail Template {orderStatus.Name} is already exitsts", (int)Helpers.Constants.ApiReturnCode.fail);
                }
            }
            catch (Exception err)
            {
                
                return new Response<OrderStatusViewModel>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }

        }

        /// <summary>
        /// check if there is an order status already exists on Database by same name
        /// </summary>
        /// <param name="Name">this name must be unique in database</param>
        /// <returns>result will be true in case of this name is not existon database or exist for same id. otherwise, response would be false </returns>
        private bool CheckOrderStatusName(string Name, int? Id)
        {
            int cnt = 1;
            if (Id != null)
            {
                cnt = _unitOfWork.OrderStatusListRepository.GetAllAsQueryable().Where(x => x.Name == Name && x.Id != Id).ToList().Count;
            }
            else
            {
                cnt = _unitOfWork.OrderStatusListRepository.GetAllAsQueryable().Where(x => x.Name == Name).ToList().Count;
            }
            if (cnt == 0)
            {
                return true;
            }
            return false;
        }
        //-------------------------------------------  end of Order Status

        public Response<ConditionViewModel> AddCondition(AddConditionViewModel condition)
        {
            try
            {
                if (CheckConditionNameForAdd(condition.Name))
                {
                    TLIaction cond = new TLIaction();
                    cond.Name = condition.Name;
                    cond.Type = ActionType.Condition;
                    cond.Proposal = StepProposal.proposalNotAllowed;
                    cond.Deleted = false;
                    _unitOfWork.ConditionRepository.Add(cond);
                    _unitOfWork.SaveChanges();
                    return new Response<ConditionViewModel>(_mapper.Map<ConditionViewModel>(cond));
                }
                else
                {
                    return new Response<ConditionViewModel>(true, null, null, $"This condition {condition.Name} is already exists", (int)Helpers.Constants.ApiReturnCode.fail);
                }
            }
            catch (Exception err)
            {
                
                return new Response<ConditionViewModel>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }

        }

        public Response<OptionViewModel> AddOption(AddActionOptionViewModel Option)
        {
            try
            {
                if (CheckOptionNameForAdd(Option.Name, Option.ActionId, Option.ParentId))
                {
                    TLIactionOption ao = new TLIactionOption();
                    ao.ActionId = Option.ActionId;
                    ao.ParentId = Option.ParentId;
                    ao.Name = Option.Name;
                    _unitOfWork.OptionRepository.Add(ao);
                    _unitOfWork.SaveChanges();
                    return new Response<OptionViewModel>(_mapper.Map<OptionViewModel>(ao));
                }
                else
                {
                    return new Response<OptionViewModel>(true, null, null, $"This Option {Option.Name} is already exists", (int)Helpers.Constants.ApiReturnCode.fail);
                }
            }
            catch (Exception err)
            {
                
                return new Response<OptionViewModel>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
        }

        public Response<OptionViewModel> AddSubOption(AddActionOptionViewModel SubOption)
        {
            //*
            try
            {
                return AddOption(SubOption);
                /*
                if (CheckOptionNameForAdd(SubOption.Name))
                {
                    await _unitOfWork.OptionRepository.AddModelAsync(SubOption);
                    return new Response<OptionViewModel>();
                }
                else
                {
                    return new Response<OptionViewModel>(true, null, null, $"This SubOption {SubOption.Name} is already exists", (int)Helpers.Constants.ApiReturnCode.fail);
                }
                //*/
            }
            catch (Exception err)
            {
                
                return new Response<OptionViewModel>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
            //*/
        }

        public async Task<Response<List<ConditionViewModel>>> GetAllConditions()
        {
            try
            {
                var Condition = (await _unitOfWork.ConditionRepository.GetAllAsync()).Where(x => x.Type == ActionType.Condition && x.Deleted == false);
                var ConditionModel = _mapper.Map<List<ConditionViewModel>>(Condition);
                return new Response<List<ConditionViewModel>>(true, ConditionModel, null, null, (int)Helpers.Constants.ApiReturnCode.success);
            }
            catch (Exception err)
            {
                
                return new Response<List<ConditionViewModel>>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }

        }

        public Response<List<OptionViewModel>> GetAllOptions(ParameterPagination parameterPagination, List<FilterObjectList> filter)
        {
            try
            {
                int count = 0;
                //List<FilterExperssion> filterExperssions = null;
                //if (filters.Count > 0)
                //{
                //    foreach (var filter in filters)
                //    {
                //        filterExperssions.Add(new FilterExperssion(filter.key, "==", filter.value));
                //    }
                //}


                var Options = _unitOfWork.OptionRepository.GetAllIncludeMultiple(parameterPagination, filter, out count).ToList(); //, o => o.Action
                var OptionsModel = _mapper.Map<List<OptionViewModel>>(Options.Where(o => o.Deleted == false && o.Action.Deleted == false));
                return new Response<List<OptionViewModel>>(true, OptionsModel, null, null, (int)Helpers.Constants.ApiReturnCode.success, count);
            }
            catch (Exception err)
            {
                
                return new Response<List<OptionViewModel>>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
        }

        public Response<List<OptionViewModel>> GetAllSubOptions(ParameterPagination parameterPagination, List<FilterObjectList> filter)
        {
            try
            {
                int count = 0;
                //List<FilterExperssion> filterExperssions = null;
                //if (filters.Count > 0)
                //{
                //    foreach (var filter in filters)
                //    {
                //        filterExperssions.Add(new FilterExperssion(filter.key, "==", filter.value));
                //    }
                //}

                var Options = _unitOfWork.OptionRepository.GetAllIncludeMultiple(parameterPagination, filter, out count).ToList(); //, o => o.Action
                var OptionsModel = _mapper.Map<List<OptionViewModel>>(Options.Where(o => o.Deleted == false && o.Parent != null));
                return new Response<List<OptionViewModel>>(true, OptionsModel, null, null, (int)Helpers.Constants.ApiReturnCode.success, count);


                //var SubOptions = _unitOfWork.SubOptionRepository.GetAllIncludeMultiple(parameterPagination, filter, out count, o => o.Option).ToList();
                //var SubOptionsModel = _mapper.Map<List<SubOptionViewModel>>(SubOptions);
                //return new Response<List<SubOptionViewModel>>(true, SubOptionsModel, null, null, (int)Helpers.Constants.ApiReturnCode.success, count);
            }
            catch (Exception err)
            {
                
                //return new Response<List<SubOptionViewModel>>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
                return new Response<List<OptionViewModel>>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
        }

        public Response<List<OptionViewModel>> GetOptionsByConditionId(int ConditionId)
        {
            try
            {


                var Options = _unitOfWork.OptionRepository.GetAllAsQueryable().Where(c => c.ActionId == ConditionId && c.Deleted == false).ToList();
                var OptionsModel = _mapper.Map<List<OptionViewModel>>(Options);
                return new Response<List<OptionViewModel>>(true, OptionsModel, null, null, (int)Helpers.Constants.ApiReturnCode.success);
            }
            catch (Exception err)
            {
                
                return new Response<List<OptionViewModel>>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
        }

        public Response<List<OptionViewModel>> GetSubOptionsByOptionId(int OptionId)
        {
            try
            {
                var SubOptions = _unitOfWork.OptionRepository.GetAllAsQueryable().Where(o => o.ParentId == OptionId && o.Deleted==false ).ToList();
                var SubOptionsModel = _mapper.Map<List<OptionViewModel>>(SubOptions);
                return new Response<List<OptionViewModel>>(true, SubOptionsModel, null, null, (int)Helpers.Constants.ApiReturnCode.success);
            }
            catch (Exception err)
            {
                
                return new Response<List<OptionViewModel>>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
        }

        public Response<ConditionViewModel> UpdateCondition(ConditionViewModel condition)
        {
            try
            {
                if (CheckConditionNameForUpdate(condition.Name, condition.Id))
                {
                    TLIaction action = _unitOfWork.ActionRepository.GetByID(condition.Id);
                    action.Name = condition.Name;
                    _unitOfWork.ConditionRepository.Update(action);
                    _unitOfWork.SaveChanges();
                    return new Response<ConditionViewModel>(_mapper.Map<ConditionViewModel>(action));
                }
                else
                {
                    return new Response<ConditionViewModel>(true, null, null, $"this Condition {condition.Name} is already exitsts", (int)Helpers.Constants.ApiReturnCode.fail);
                }
            }
            catch (Exception err)
            {
                
                return new Response<ConditionViewModel>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }

        }

        /*
        public async Task<Response<ConditionViewModel>>ChangeConditionStatus(int conditionId)
        {
            try
            {
                //var template = _unitOfWork.ConditionRepository.SingleOrDefaultAsync(x => x.Id = conditionId);
                //var model= _mapper.Map<ConditionViewModel>(template);
                //model.a
                //await _unitOfWork.ConditionRepository.UpdateItem(condition);
                //await _unitOfWork.SaveChangesAsync();
                return new Response<ConditionViewModel>();
            }
            catch (Exception err)
            {
                
                return new Response<ConditionViewModel>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }

        }
        //*/

        public Response<OptionViewModel> UpdateOption(ListActionOptionViewModel Option)
        {
            try
            {
                if (CheckOptionNameForUpdate(Option.Name, Option.Id))
                {

                    TLIactionOption _option = _unitOfWork.OptionRepository.GetByID(Option.Id);
                    if (_option != null)
                    {
                        _option.Name = Option.Name;
                        _option.ParentId = Option.ParentId;
                        _option.ItemStatusId = Option.ItemStatusId;
                        _unitOfWork.OptionRepository.Update(_option);
                        _unitOfWork.SaveChanges();
                        return new Response<OptionViewModel>(_mapper.Map<OptionViewModel>(_option));
                    }
                }
                return new Response<OptionViewModel>(true, null, null, $"this Option {Option.Name} is already exitsts", (int)Helpers.Constants.ApiReturnCode.fail);
            }
            catch (Exception err)
            {
                
                return new Response<OptionViewModel>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
        }

        public async Task<Response<OptionViewModel>> UpdateSubOption(TLIactionOption SubOption)
        {
            try
            {
                if (CheckOptionNameForUpdate(SubOption.Name, SubOption.Id))
                {
                    await _unitOfWork.OptionRepository.UpdateItem(SubOption);
                    await _unitOfWork.SaveChangesAsync();
                    return new Response<OptionViewModel>();
                }
                else
                {
                    return new Response<OptionViewModel>(true, null, null, $"this SubOption {SubOption.Name} is already exitsts", (int)Helpers.Constants.ApiReturnCode.fail);
                }
            }
            catch (Exception err)
            {
                
                return new Response<OptionViewModel>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
        }
        public Response<OrderStatusViewModel> AddTaskStatus(OrderStatusAddViewModel TaskStatus)
        {
            try
            {
                if (CheckTaskStatusNameForAdd(TaskStatus.Name))
                {
                    TLIorderStatus orderstatus = _mapper.Map<TLIorderStatus>(TaskStatus);
                    _unitOfWork.OrderStatusListRepository.Add(orderstatus);
                    _unitOfWork.SaveChanges();
                    return new Response<OrderStatusViewModel>(_mapper.Map<OrderStatusViewModel>(orderstatus));
                }
                else
                {
                    return new Response<OrderStatusViewModel>(true, null, null, $"This Ticket Status {TaskStatus.Name} is already exists", (int)Helpers.Constants.ApiReturnCode.fail);
                }
            }
            catch(Exception err)
            {
                
                return new Response<OrderStatusViewModel>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
            
        }

        public Response<OrderStatusViewModel> EditTaskStatus(OrderStatusViewModel taskStatusViewModel)
        {
            try
            {
                if (CheckTaskStatusNameForUpdate(taskStatusViewModel.Name, taskStatusViewModel.Id))
                {
                    TLIorderStatus orderstatus = _mapper.Map<TLIorderStatus>(taskStatusViewModel);
                    _unitOfWork.OrderStatusListRepository.Update(orderstatus);
                    _unitOfWork.SaveChanges();
                    return new Response<OrderStatusViewModel>(_mapper.Map<OrderStatusViewModel>(orderstatus));
                }
                else
                {
                    return new Response<OrderStatusViewModel>(true, null, null, $"this TaskStatus {taskStatusViewModel.Name} is already exitsts", (int)Helpers.Constants.ApiReturnCode.fail);
                }
            }
            catch (Exception err)
            {
                
                return new Response<OrderStatusViewModel>(true, null, null, err.Message, Int32.Parse(Constants.ApiReturnCode.fail.ToString()));
            }
        }
        /*
        public Response<IEnumerable<TaskStatusViewModel>> getAllTaskStatus(ParameterPagination parameterPagination, List<FilterObjectList> filters)
        {
            try
            {
                int count = 0;
                //List<FilterExperssion> filterExperssions = null;
                //if (filters.Count > 0)
                //{
                //    foreach (var filter in filters)
                //    {
                //        filterExperssions.Add(new FilterExperssion(filter.key, "==", filter.value));
                //    }
                //}
                var TaskStatus = _unitOfWork.TaskStatusRepository.GetAllIncludeMultiple(parameterPagination, filters, out count, null).ToList();
                var TaskStatusModel = _mapper.Map<List<TaskStatusViewModel>>(TaskStatus);
                return new Response<IEnumerable<TaskStatusViewModel>>(true, TaskStatusModel, null, null, (int)Helpers.Constants.ApiReturnCode.success, count);
            }
            catch (Exception err)
            {
                
                return new Response<IEnumerable<TaskStatusViewModel>>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
        }
        //*/


        public Response<List<ListItemStatusViewModel>> GetAllItemStatus()//ParameterPagination parameterPagination, List<FilterObjectList> filter
        {
            try
            {
                int count = 0;
                //List<FilterExperssion> filterExperssions = null;
                //if (filters.Count > 0)
                //{
                //    foreach (var filter in filters)
                //    {
                //        filterExperssions.Add(new FilterExperssion(filter.key, "==", filter.value));
                //    }
                //}
                //var ItemStatus = _unitOfWork.ItemStatusRepository.GetAllIncludeMultiple(parameterPagination, filter, out count, null).ToList();
                var ItemStatus = _unitOfWork.ItemStatusRepository.GetAllAsQueryable().Where(x => x.Deleted == false).ToList();
                var ItemStatusModel = _mapper.Map<List<ListItemStatusViewModel>>(ItemStatus);
                return new Response<List<ListItemStatusViewModel>>(true, ItemStatusModel, null, null, (int)Helpers.Constants.ApiReturnCode.success, count);
            }
            catch (Exception err)
            {
                
                return new Response<List<ListItemStatusViewModel>>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
        }

        public Response<ListItemStatusViewModel> GetItemStatusbyId(int ItemStatusId)
        {
            try
            {
                var ItemStatus = _unitOfWork.ItemStatusRepository.GetAllAsQueryable().Where(i => i.Id == ItemStatusId).FirstOrDefault();
                var ItemStatusModel = _mapper.Map<ListItemStatusViewModel>(ItemStatus);
                return new Response<ListItemStatusViewModel>(true, ItemStatusModel, null, null, (int)Helpers.Constants.ApiReturnCode.success);
            }
            catch (Exception err)
            {
                
                return new Response<ListItemStatusViewModel>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
        }

        public Response<ListItemStatusViewModel> AddItemStatus(AddItemStatusViewModel ItemStatus)
        {
            try
            {
                var entity = _mapper.Map<TLIitemStatus>(ItemStatus);
                _unitOfWork.ItemStatusRepository.Add(entity);
                _unitOfWork.SaveChangesAsync();
                return new Response<ListItemStatusViewModel>(_mapper.Map<ListItemStatusViewModel>(entity));
            }
            catch (Exception err)
            {
                
                return new Response<ListItemStatusViewModel>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
        }

        public Response<ListItemStatusViewModel> UpdatItemStatus(ListItemStatusViewModel ItemStatus)
        {
            try
            {
                var entity = _mapper.Map<TLIitemStatus>(ItemStatus);
                _unitOfWork.ItemStatusRepository.Update(entity);
                _unitOfWork.SaveChanges();
                return new Response<ListItemStatusViewModel>(_mapper.Map<ListItemStatusViewModel>(entity));
            }
            catch (Exception err)
            {
                
                return new Response<ListItemStatusViewModel>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
        }
        /*
        public Response<TaskStatusViewModel> GetTaskStatusbyId(int TaskStatusId)
        {
            try
            {
                var TaskStatus = _unitOfWork.TaskStatusRepository.GetAllAsQueryable().Where(t => t.Id == TaskStatusId).FirstOrDefault();
                var TaskStatusModel = _mapper.Map<TaskStatusViewModel>(TaskStatus);
                return new Response<TaskStatusViewModel>(true, TaskStatusModel, null, null, (int)Helpers.Constants.ApiReturnCode.success);
            }
            catch (Exception err)
            {
                
                return new Response<TaskStatusViewModel>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
        }
        //*/
        private bool CheckConditionNameForAdd(string Name)
        {
            //var Condition = _unitOfWork.ConditionRepository.Where("Name", "==", Name).SingleOrDefault();
            var Condition = _unitOfWork.ConditionRepository.GetWhereFirst(x=>x.Name == Name);
            if (Condition == null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        private bool CheckConditionNameForUpdate(string Name, int Id)
        {
            var Condition = _unitOfWork.ConditionRepository.GetAllAsQueryable().Where(x => x.Name == Name && x.Id != Id).ToList();
            if (Condition == null || Condition.Count == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        private bool CheckOptionNameForAdd(string Name, int? ActionId, int? ParentId)
        {
            if (ActionId != null)
            {
                var Option = _unitOfWork.OptionRepository.GetAllAsQueryable().Where(x=>x.Name== Name && x.ActionId==ActionId).SingleOrDefault();
                if (Option == null)
                {
                    return true;
                }
                return false;
            }
            else if(ParentId!=null)
            {
                var Option = _unitOfWork.OptionRepository.GetAllAsQueryable().Where(x => x.Name == Name && x.ParentId == ParentId).SingleOrDefault();
                if (Option == null)
                {
                    return true;
                }
                return false;

            }
            else // both is null
            {
                return false;
            }
        }
        private bool CheckOptionNameForUpdate(string Name, int Id)
        {
            var Option = _unitOfWork.OptionRepository.GetAllAsQueryable().Where(x => x.Name == Name && x.Id != Id).ToList();
            if (Option == null || Option.Count==0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        private bool CheckSubOptionNameForAdd(string Name)
        {
            //  var SubOption = _unitOfWork.SubOptionRepository.Where("Name", "==", Name).SingleOrDefault();
            var SubOption = _unitOfWork.SubOptionRepository.GetWhereFirst(x => x.Name == Name);
            if (SubOption == null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        private async Task <bool> CheckSubOptionNameForUpdate(string Name, int Id)
        {
            var SubOption =await _unitOfWork.SubOptionRepository.SingleOrDefaultAsync(x => x.Name == Name && x.Id != Id);
            if (SubOption == null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        //*
        private bool CheckTaskStatusNameForAdd(string Name)
        {
            //  var TaskStatus = _unitOfWork.OrderStatusListRepository.Where("Name", "==", Name).SingleOrDefault();
            var TaskStatus = _unitOfWork.OrderStatusListRepository.GetWhereFirst(x => x.Name == Name);
            if (TaskStatus == null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        private bool CheckTaskStatusNameForUpdate(string Name, int Id)
        {
            var Option =_unitOfWork.OrderStatusListRepository.SingleOrDefaultAsync(x => x.Name == Name && x.Id != Id);
            if (Option == null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        //*/
    }
}
