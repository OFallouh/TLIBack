using System;
using System.Collections.Generic;
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

namespace TLIS_Service.IService
{
    public interface IWorkFlowSettingService
    {
        Task<Response<List<ConditionViewModel>>> GetAllConditions();
        Response<ConditionViewModel> AddCondition(AddConditionViewModel condition);
        Response<ConditionViewModel> UpdateCondition(ConditionViewModel condition);
        Response<List<OptionViewModel>> GetAllOptions(ParameterPagination parameterPagination, List<FilterObjectList> filters);
        Response<List<OptionViewModel>> GetOptionsByConditionId(int ConditionId);
        Response<OptionViewModel> AddOption(AddActionOptionViewModel Option);
        Response<OptionViewModel> UpdateOption(ListActionOptionViewModel Option);
        Response<List<OptionViewModel>> GetAllSubOptions(ParameterPagination parameterPagination, List<FilterObjectList> filters);
        Response<List<OptionViewModel>> GetSubOptionsByOptionId(int OptionId);
        Response<OptionViewModel> AddSubOption(AddActionOptionViewModel SubOption);
        Task<Response<OptionViewModel>> UpdateSubOption(TLIactionOption SubOption);
        //Response<IEnumerable<TaskStatusViewModel>> getAllTaskStatus(ParameterPagination parameterPagination, List<FilterObjectList> filters);
        //Task<Response<TaskStatusViewModel>> AddTaskStatus(TLItaskStatus TaskStatus);
        //Task<Response<TaskStatusViewModel>> EditTaskStatus(TaskStatusViewModel taskStatusViewModel);
        //Response<TaskStatusViewModel> GetTaskStatusbyId(int TaskStatusId);
        //------------- Item status
        Response<List<ListItemStatusViewModel>> GetAllItemStatus();//(ParameterPagination parameterPagination, List<FilterObjectList> filters
        Response<ListItemStatusViewModel> GetItemStatusbyId(int ItemStatusId);
        Response<ListItemStatusViewModel> AddItemStatus(AddItemStatusViewModel ItemStatus);
        Response<ListItemStatusViewModel> UpdatItemStatus(ListItemStatusViewModel ItemStatus);

        //------------- End of Item status

        //------------- Mail Template
        Task<Response<List<MailTemplateViewModel>>> GetAllMailTemplates();
        Response<MailTemplateViewModel> AddMailTemplate(AddMailTemplateViewModel mailTemplate);
        Task<Response<MailTemplateViewModel>> UpdateMailTemplate(MailTemplateViewModel mailTemplate);

        //------------- End of Mail Template
        //------------- Order status
        Response<List<OrderStatusViewModel>> GetAllOrderStatus();
        Response<OrderStatusViewModel> GetOrderStatusById(int Id);
        Response<OrderStatusViewModel> AddOrderStatus(OrderStatusAddViewModel orderStatus);
        Response<OrderStatusViewModel> UpdateOrderStatus(OrderStatusViewModel orderStatus);

        //------------- End of Order status        
        //------------- Action Item option
        Task<Response<List<ActionItemOptionListViewModel>>> GetAllActionItemOptions();
        Task<Response<ActionItemOptionListViewModel>> GetActionItemOptionById(int Id);
        Response<ActionItemOptionEditViewModel> AddActionItemOption(ActionItemOptionAddViewModel actionItemOption);
        Response<ActionItemOptionEditViewModel> EditActionItemOption(ActionItemOptionEditViewModel actionItemOption);

        //------------- End of action Item option
        //------------- part
        Response<List<ListPartViewModel>> GetAllParts();
        //------------- End of part
    }
}
