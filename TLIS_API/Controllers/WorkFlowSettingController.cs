using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TLIS_API.Middleware.WorkFlow;
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
using TLIS_Service.Helpers;
using TLIS_Service.ServiceBase;

namespace TLIS_API.Controllers
{
    [ServiceFilter(typeof(WorkFlowMiddleware))]
    [ServiceFilter(typeof(LogFilterAttribute))]
    [Route("api/[controller]")]
    [ApiController]
    public class WorkFlowSettingController : ControllerBase

    {
        
        IUnitOfWorkService _unitOfWork;
        public WorkFlowSettingController(IUnitOfWorkService unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        //------------- part

        [HttpGet("GetAllParts")]
        [ProducesResponseType(200, Type = typeof(List<ListPartViewModel>))]
        public IActionResult GetAllParts()
        {
            var response = _unitOfWork.WorkFlowSettingService.GetAllParts();
            return Ok(response);
        }
        //------------- end of part
        //------------- Action Item Option

        [HttpGet("GetAllActionItemOptions")]
        [ProducesResponseType(200, Type = typeof(List<ActionItemOptionListViewModel>))]
        public async Task<IActionResult> GetAllActionItemOptions()
        {
            var response = await _unitOfWork.WorkFlowSettingService.GetAllActionItemOptions();
            return Ok(response);
        }
        [HttpPost("AddActionItemOption")]
        [ProducesResponseType(200, Type = typeof(Nullable))]
        public IActionResult AddActionItemOption(ActionItemOptionAddViewModel ActionItemOption)
        {
            if (TryValidateModel(ActionItemOption, nameof(ActionItemOptionAddViewModel)))
            {
                var response = _unitOfWork.WorkFlowSettingService.AddActionItemOption(ActionItemOption);
                return Ok(response);
            }
            else
            {
                var ErrorMessages = from state in ModelState.Values
                                    from error in state.Errors
                                    select error.ErrorMessage;
                return Ok(new Response<ActionItemOptionEditViewModel>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
            }
        }
        [HttpPost("UpdateActionItemOption")]
        [ProducesResponseType(200, Type = typeof(Nullable))]
        public IActionResult UpdateActionItemOption([FromBody]ActionItemOptionEditViewModel ActionItemOption)
        {
            if (TryValidateModel(ActionItemOption, nameof(ActionItemOptionEditViewModel)))
            {
                var response = _unitOfWork.WorkFlowSettingService.EditActionItemOption(ActionItemOption);
                return Ok(response);
            }
            else
            {
                var ErrorMessages = from state in ModelState.Values
                                    from error in state.Errors
                                    select error.ErrorMessage;
                return Ok(new Response<ActionItemOptionEditViewModel>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
            }
        }

        //------------- End of Mail Template

        //------------- Mail Template

        [HttpGet("GetAllMailTemplates")]
        [ProducesResponseType(200, Type = typeof(List<MailTemplateViewModel>))]
        public async Task<IActionResult> GetAllMailTemplates()
        {
            var response = await _unitOfWork.WorkFlowSettingService.GetAllMailTemplates();
            return Ok(response);
        }
        [HttpPost("AddMailTemplate")]
        [ProducesResponseType(200, Type = typeof(Nullable))]
        public IActionResult AddMailTemplate(AddMailTemplateViewModel mailTemplate)
        {
            if (TryValidateModel(mailTemplate, nameof(MailTemplateViewModel)))
            {
                var response = _unitOfWork.WorkFlowSettingService.AddMailTemplate(mailTemplate);
                return Ok(response);
            }
            else
            {
                var ErrorMessages = from state in ModelState.Values
                                    from error in state.Errors
                                    select error.ErrorMessage;
                return Ok(new Response<MailTemplateViewModel>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
            }
        }
        [HttpPost("UpdateMailTemplate")]
        [ProducesResponseType(200, Type = typeof(Nullable))]
        public async Task<IActionResult> UpdateMailTemplate([FromBody]MailTemplateViewModel mailTemplate)
        {
            if (TryValidateModel(mailTemplate, nameof(MailTemplateViewModel)))
            {
                var response = await _unitOfWork.WorkFlowSettingService.UpdateMailTemplate(mailTemplate);
                return Ok(response);
            }
            else
            {
                var ErrorMessages = from state in ModelState.Values
                                    from error in state.Errors
                                    select error.ErrorMessage;
                return Ok(new Response<MailTemplateViewModel>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
            }
        }

        //------------- End of Mail Template

        //------------- Order Status

        [HttpGet("getAllTicketStatus")]
        [ProducesResponseType(200, Type = typeof(List<OrderStatusViewModel>))]
        public IActionResult getAllTicketStatus()
        {
            var response = _unitOfWork.WorkFlowSettingService.GetAllOrderStatus();
            return Ok(response);
        }
        [HttpGet("getTicketStatusById")]
        [ProducesResponseType(200, Type = typeof(OrderStatusViewModel))]
        public IActionResult getTicketStatusById(int Id)
        {
            var response = _unitOfWork.WorkFlowSettingService.GetOrderStatusById(Id);
            return Ok(response);
        }
        [HttpPost("AddTicketStatus")]
        [ProducesResponseType(200, Type = typeof(Nullable))]
        public IActionResult AddTicketStatus(OrderStatusAddViewModel ordeStatus)
        {
            if (TryValidateModel(ordeStatus, nameof(OrderStatusAddViewModel)))
            {
                var response = _unitOfWork.WorkFlowSettingService.AddOrderStatus(ordeStatus);
                return Ok(response);
            }
            else
            {
                var ErrorMessages = from state in ModelState.Values
                                    from error in state.Errors
                                    select error.ErrorMessage;
                return Ok(new Response<OrderStatusAddViewModel>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
            }
        }
        [HttpPost("EditTicketStatus")]
        [ProducesResponseType(200, Type = typeof(Nullable))]
        public IActionResult EditTicketStatus([FromBody]OrderStatusViewModel ordeStatus)
        {
            if (TryValidateModel(ordeStatus, nameof(OrderStatusViewModel)))
            {
                var response = _unitOfWork.WorkFlowSettingService.UpdateOrderStatus(ordeStatus);
                return Ok(response);
            }
            else
            {
                var ErrorMessages = from state in ModelState.Values
                                    from error in state.Errors
                                    select error.ErrorMessage;
                return Ok(new Response<OrderStatusEditViewModel>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
            }
        }

        //------------- End of Order Status

        [HttpGet("GetOptionsByConditionId/{ConditionId}")]
        [ProducesResponseType(200, Type = typeof(List<ConditionViewModel>))]
        public IActionResult GetOptionsByConditionId(int ConditionId)
        {
            var response = _unitOfWork.WorkFlowSettingService.GetOptionsByConditionId(ConditionId);
            return Ok(response);
        }
        [HttpGet("GetAllConditions")]
        [ProducesResponseType(200, Type = typeof(List<ConditionViewModel>))]
        public async Task<IActionResult> GetAllConditions()
        {
            var response = await _unitOfWork.WorkFlowSettingService.GetAllConditions();
            return Ok(response);
        }
        [HttpPost("AddCondition")]
        [ProducesResponseType(200, Type = typeof(Nullable))]
        public IActionResult AddCondition(AddConditionViewModel condition)
        {
            if(TryValidateModel(condition, nameof(AddConditionViewModel)))
            {
                var response = _unitOfWork.WorkFlowSettingService.AddCondition(condition);
                return Ok(response);
            }
            else
            {
                var ErrorMessages = from state in ModelState.Values
                                    from error in state.Errors
                                    select error.ErrorMessage;
                return Ok(new Response<ConditionViewModel>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
            }
        }
        [HttpPost("UpdateCondition")]
        [ProducesResponseType(200, Type = typeof(Nullable))]
        public IActionResult UpdateCondition(ConditionViewModel condition)
        {
            if(TryValidateModel(condition, nameof(TLIaction)))
            {
                var response =  _unitOfWork.WorkFlowSettingService.UpdateCondition(condition);
                return Ok(response);
            }
            else
            {
                var ErrorMessages = from state in ModelState.Values
                                    from error in state.Errors
                                    select error.ErrorMessage;
                return Ok(new Response<ConditionViewModel>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
            }
        }
        [HttpPost("GetAllOptions")]
        [ProducesResponseType(200, Type = typeof(List<OptionViewModel>))]
        public IActionResult GetAllOptions([FromQuery] ParameterPagination parameterPagination, [FromBody] List<FilterObjectList> filters)
        {
            var response = _unitOfWork.WorkFlowSettingService.GetAllOptions(parameterPagination, filters);
            return Ok(response);
        }
        [HttpPost("AddOption")]
        [ProducesResponseType(200, Type = typeof(Nullable))]
        public IActionResult AddOption([FromBody]AddActionOptionViewModel Option)
        {
            if(TryValidateModel(Option, nameof(OptionViewModel)))
            {
                var response = _unitOfWork.WorkFlowSettingService.AddOption(Option);
                return Ok(response);
            }
            else
            {
                var ErrorMessages = from state in ModelState.Values
                                    from error in state.Errors
                                    select error.ErrorMessage;
                return Ok(new Response<OptionViewModel>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
            }
        }
        [HttpPost("UpdateOption")]
        [ProducesResponseType(200, Type = typeof(Nullable))]
        public IActionResult UpdateOption(ListActionOptionViewModel Option)
        {
            if(TryValidateModel(Option, nameof(ListActionOptionViewModel)))
            {
                var response = _unitOfWork.WorkFlowSettingService.UpdateOption(Option);
                return Ok(response);
            }
            else
            {
                var ErrorMessages = from state in ModelState.Values
                                    from error in state.Errors
                                    select error.ErrorMessage;
                return Ok(new Response<OptionViewModel>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
            }
        }
        [HttpPost("GetAllSubOptions")]
        [ProducesResponseType(200, Type = typeof(List<SubOptionViewModel>))]
        public IActionResult GetAllSubOptions([FromQuery] ParameterPagination parameterPagination, [FromBody] List<FilterObjectList> filters)
        {
            var response = _unitOfWork.WorkFlowSettingService.GetAllSubOptions(parameterPagination, filters);
            return Ok(response);
        }
        [HttpPost("AddSubOption")]
        [ProducesResponseType(200, Type = typeof(Nullable))]
        public IActionResult AddSubOption(AddActionOptionViewModel SubOption)
        {
            if(TryValidateModel(SubOption, nameof(OptionViewModel)))
            {
                var response = _unitOfWork.WorkFlowSettingService.AddSubOption(SubOption);
                return Ok(response);
            }
            else
            {
                var ErrorMessages = from state in ModelState.Values
                                    from error in state.Errors
                                    select error.ErrorMessage;
                return Ok(new Response<OptionViewModel>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
            }
        }
        [HttpPost("UpdateSubOption")]
        [ProducesResponseType(200, Type = typeof(Nullable))]
        public async Task<IActionResult> UpdateSubOption(TLIactionOption SubOption)
        {
            if (TryValidateModel(SubOption, nameof(TLIactionOption)))
            {
                var response = await _unitOfWork.WorkFlowSettingService.UpdateSubOption(SubOption);
                return Ok(response);
            }
            else
            {
                var ErrorMessages = from state in ModelState.Values
                                    from error in state.Errors
                                    select error.ErrorMessage;
                return Ok(new Response<OptionViewModel>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
            }
        }
        [HttpGet("GetSubOptionsByOptionId/{OptionId}")]
        [ProducesResponseType(200, Type = typeof(List<SubOptionViewModel>))]
        public IActionResult GetSubOptionsByOptionId(int OptionId)
        {
            var response = _unitOfWork.WorkFlowSettingService.GetSubOptionsByOptionId(OptionId);
            return Ok(response);
        }
        /*
        [HttpPost("getAllTaskStatus")]
        [ProducesResponseType(200, Type = typeof(List<TaskStatusViewModel>))]
        public IActionResult getAllTaskStatus([FromQueryAttribute] ParameterPagination parameterPagination, [FromBody] List<FilterObjectList> filters)
        {
            var response = _unitOfWork.WorkFlowSettingService.getAllTaskStatus(parameterPagination, filters);
            return Ok(response);
        }
        [HttpPost("AddTaskStatus")]
        [ProducesResponseType(200, Type = typeof(AddTaskStatusViewModel))]
        public async Task<IActionResult> AddTaskStatus([FromBody]TLItaskStatus TaskStatus)
        {
            if(ModelState.IsValid)
            {
                var response = await _unitOfWork.WorkFlowSettingService.AddTaskStatus(TaskStatus);
                return Ok(response);
            }
            else
            {
                var ErrorMessages = from state in ModelState.Values
                                    from error in state.Errors
                                    select error.ErrorMessage;
                return Ok(new Response<TLItaskStatus>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
            }
        }
        [HttpPost("EditTaskStatus")]
        [ProducesResponseType(200, Type = typeof(TaskStatusViewModel))]
        public async Task<IActionResult> EditTaskStatus([FromBody]TaskStatusViewModel TaskStatusViewModel)
        {
            if(ModelState.IsValid)
            {
                var response = await _unitOfWork.WorkFlowSettingService.EditTaskStatus(TaskStatusViewModel);
                return Ok(response);
            }
            else
            {
                var ErrorMessages = from state in ModelState.Values
                                    from error in state.Errors
                                    select error.ErrorMessage;
                return Ok(new Response<TaskStatusViewModel>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
            }
        }
        [HttpGet("GetTaskStatusById/{TaskStatusId}")]
        [ProducesResponseType(200, Type = typeof(TaskStatusViewModel))]
        public IActionResult GetTaskStatusById(int TaskStatusId)
        {
            var response = _unitOfWork.WorkFlowSettingService.GetTaskStatusbyId(TaskStatusId);
            return Ok(response);
        }
        //*/
        [HttpPost("GetAllItemStatus")]
        [ProducesResponseType(200, Type = typeof(List<ListItemStatusViewModel>))]
        public IActionResult GetAllItemStatus()//[FromQuery]ParameterPagination parameterPagination,[FromBody]List<FilterObjectList> filters
        {
            var response = _unitOfWork.WorkFlowSettingService.GetAllItemStatus();//parameterPagination, filters
            return Ok(response);
        }
        [HttpGet("GetItemStatusById/{ItemStatusId}")]
        [ProducesResponseType(200, Type = typeof(ListItemStatusViewModel))]
        public IActionResult GetItemStatusById(int ItemStatusId)
        {
            var response = _unitOfWork.WorkFlowSettingService.GetItemStatusbyId(ItemStatusId);
            return Ok(response);
        }
        [HttpPost("AddItemStatus")]
        [ProducesResponseType(200, Type = typeof(Nullable))]
        public IActionResult AddItemStatus(AddItemStatusViewModel ItemStatus)
        {
            if(TryValidateModel(ItemStatus, nameof(AddItemStatusViewModel)))
            {
                var response = _unitOfWork.WorkFlowSettingService.AddItemStatus(ItemStatus);
                return Ok(response);
            }
            else
            {
                var ErrorMessages = from state in ModelState.Values
                                    from error in state.Errors
                                    select error.ErrorMessage;
                return Ok(new Response<ListItemStatusViewModel>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
            }
        }
        [HttpPost("UpdateItemStatus")]
        [ProducesResponseType(200, Type = typeof(Nullable))]
        public IActionResult UpdateItemStatus(ListItemStatusViewModel ItemStatus)
        {
            if(TryValidateModel(ItemStatus, nameof(ListItemStatusViewModel)))
            {
                var response = _unitOfWork.WorkFlowSettingService.UpdatItemStatus(ItemStatus);
                return Ok(response);
            }
            else
            {
                var ErrorMessages = from state in ModelState.Values
                                    from error in state.Errors
                                    select error.ErrorMessage;
                return Ok(new Response<ListItemStatusViewModel>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
            }
        }
    }
}

