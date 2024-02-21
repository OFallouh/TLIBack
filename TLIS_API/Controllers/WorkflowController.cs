using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TLIS_API.Middleware.WorkFlow;
using TLIS_DAL.Helper;
using TLIS_DAL.Helper.Filters;
using TLIS_DAL.Helpers;
using TLIS_DAL.ViewModels.ActionDTOs;
using TLIS_DAL.ViewModels.StepActionDTOs;
using TLIS_DAL.ViewModels.StepDTOs;
using TLIS_DAL.ViewModels.WorkFlowDTOs;
using TLIS_DAL.ViewModels.WorkFlowTypeDTOs;
using TLIS_Repository.Base;
using TLIS_Service.Helpers;
using TLIS_Service.ServiceBase;

namespace TLIS_API.Controllers
{
    [ServiceFilter(typeof(WorkFlowMiddleware))]
    [ServiceFilter(typeof(LogFilterAttribute))]
    [Route("api/[controller]")]
    [ApiController]
    public class WorkflowController : ControllerBase
    {
        IUnitOfWorkService _unitOfWork;
        public WorkflowController(IUnitOfWorkService unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // Workflow 
        [HttpPost("GetAllWorkflows")]
        [ProducesResponseType(200, Type = typeof(List<ListWorkFlowViewModel>))]
        public IActionResult GetAllWorkflows([FromQuery]ParameterPagination parameterPagination, [FromBody]List<FilterObjectList> filters)
        {
            var response = _unitOfWork.WorkflowService.GetAllWorkflows(parameterPagination, filters);
            return Ok(response);
        }
        [HttpGet("GetWorkflowById/{WorkflowId}")]
        [ProducesResponseType(200, Type = typeof(ListWorkFlowViewModel))]
        public IActionResult GetWorkflowById(int WorkflowId)
        {
            var response = _unitOfWork.WorkflowService.GetWorkflowbyId(WorkflowId);
            return Ok(response);
        }
        [HttpPost("AddWorkflow")]
        [ProducesResponseType(200, Type = typeof(Nullable))]
        public IActionResult AddWorkflow(AddWorkFlowViewModel Workflow)
        {
            try
            {
                if (TryValidateModel(Workflow, nameof(AddWorkFlowViewModel)))
                {
                    var response =  _unitOfWork.WorkflowService.AddWorkflow(Workflow);
                    return Ok(response);
                }
                else
                {
                    var ErrorMessages = from state in ModelState.Values
                                        from error in state.Errors
                                        select error.ErrorMessage;
                    return Ok(new Response<EditWorkFlowViewModel>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
                }
            }
            catch (Exception ex)
            {
                return Ok(new Response<EditWorkFlowViewModel>(true, null, new string[] { ex.Message }, null, (int)Helpers.Constants.ApiReturnCode.Invalid));
            }
        }
        [HttpPost("UpdateWorkflow")]
        [ProducesResponseType(200, Type = typeof(Nullable))]
        public IActionResult UpdateWorkflow(EditWorkFlowViewModel Workflow)
        {
            if (TryValidateModel(Workflow, nameof(EditWorkFlowViewModel)))
            {
                var response =  _unitOfWork.WorkflowService.UpdateWorkflow(Workflow);
                return Ok(response);
            }
            else
            {
                var ErrorMessages = from state in ModelState.Values
                                    from error in state.Errors
                                    select error.ErrorMessage;
                return Ok(new Response<EditWorkFlowViewModel>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
            }
        }
        [HttpPost("EditMailStepAction")]
        [ProducesResponseType(200, Type = typeof(Nullable))]
        public IActionResult EditMailStepAction(EditMailStepActionViewModel Workflow)
        {
            if (TryValidateModel(Workflow, nameof(EditMailStepActionViewModel)))
            {
                var response = _unitOfWork.WorkflowService.EditMailStepAction(Workflow);
                return Ok(response);
            }
            else
            {
                var ErrorMessages = from state in ModelState.Values
                                    from error in state.Errors
                                    select error.ErrorMessage;
                return Ok(new Response<EditWorkFlowViewModel>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
            }
        }
        [HttpPost("EditStepActionApplyCalculation")]
        [ProducesResponseType(200, Type = typeof(Nullable))]
        public IActionResult EditStepActionApplyCalculation(EditStepActionApplyCalculationViewModel Workflow)
        {
            if (TryValidateModel(Workflow, nameof(EditMailStepActionViewModel)))
            {
                var response = _unitOfWork.WorkflowService.EditStepActionApplyCalculation(Workflow);
                return Ok(response);
            }
            else
            {
                var ErrorMessages = from state in ModelState.Values
                                    from error in state.Errors
                                    select error.ErrorMessage;
                return Ok(new Response<EditWorkFlowViewModel>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
            }
        }
        [HttpPost("EditCheckAvailableSpaceStepAction")]
        [ProducesResponseType(200, Type = typeof(Nullable))]
        public IActionResult EditCheckAvailableSpaceStepAction(EditStepActionCheckAvailableSpaceViewModel Workflow)
        {
            if (TryValidateModel(Workflow, nameof(EditMailStepActionViewModel)))
            {
                var response = _unitOfWork.WorkflowService.EditCheckAvailableSpaceStepAction(Workflow);
                return Ok(response);
            }
            else
            {
                var ErrorMessages = from state in ModelState.Values
                                    from error in state.Errors
                                    select error.ErrorMessage;
                return Ok(new Response<EditWorkFlowViewModel>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
            }
        }
        [HttpPost("EditCivilDecisionStepAction")]
        [ProducesResponseType(200, Type = typeof(Nullable))]
        public IActionResult EditCivilDecisionStepAction(EditStepActionCivilDecisionViewModel Workflow)
        {
            if (TryValidateModel(Workflow, nameof(EditMailStepActionViewModel)))
            {
                var response = _unitOfWork.WorkflowService.EditCivilDecisionStepAction(Workflow);
                return Ok(response);
            }
            else
            {
                var ErrorMessages = from state in ModelState.Values
                                    from error in state.Errors
                                    select error.ErrorMessage;
                return Ok(new Response<EditWorkFlowViewModel>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
            }
        }
        [HttpPost("EditConditionStepAction")]
        [ProducesResponseType(200, Type = typeof(Nullable))]
        public IActionResult EditConditionStepAction(EditStepActionConditionViewModel Workflow)
        {
            if (TryValidateModel(Workflow, nameof(EditMailStepActionViewModel)))
            {
                var response = _unitOfWork.WorkflowService.EditConditionStepAction(Workflow);
                return Ok(response);
            }
            else
            {
                var ErrorMessages = from state in ModelState.Values
                                    from error in state.Errors
                                    select error.ErrorMessage;
                return Ok(new Response<EditWorkFlowViewModel>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
            }
        }
        [HttpPost("EditInsertDataStepAction")]
        [ProducesResponseType(200, Type = typeof(Nullable))]
        public IActionResult EditInsertDataStepAction(EditStepActionInsertDataViewModel Workflow)
        {
            if (TryValidateModel(Workflow, nameof(EditMailStepActionViewModel)))
            {
                var response = _unitOfWork.WorkflowService.EditInsertDataStepAction(Workflow);
                return Ok(response);
            }
            else
            {
                var ErrorMessages = from state in ModelState.Values
                                    from error in state.Errors
                                    select error.ErrorMessage;
                return Ok(new Response<EditWorkFlowViewModel>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
            }
        }
        [HttpPost("EditUpdateDataStepAction")]
        [ProducesResponseType(200, Type = typeof(Nullable))]
        public IActionResult EditUpdateDataStepAction(EditStepActionInsertDataViewModel Workflow)
        {
            if (TryValidateModel(Workflow, nameof(EditMailStepActionViewModel)))
            {
                var response = _unitOfWork.WorkflowService.EditUpdateDataStepAction(Workflow);
                return Ok(response);
            }
            else
            {
                var ErrorMessages = from state in ModelState.Values
                                    from error in state.Errors
                                    select error.ErrorMessage;
                return Ok(new Response<EditWorkFlowViewModel>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
            }
        }
        [HttpPost("EditCorrectDataStepAction")]
        [ProducesResponseType(200, Type = typeof(Nullable))]
        public IActionResult EditCorrectDataStepAction(EditStepActionInsertDataViewModel Workflow)
        {
            if (TryValidateModel(Workflow, nameof(EditMailStepActionViewModel)))
            {
                var response = _unitOfWork.WorkflowService.EditCorrectDataStepAction(Workflow);
                return Ok(response);
            }
            else
            {
                var ErrorMessages = from state in ModelState.Values
                                    from error in state.Errors
                                    select error.ErrorMessage;
                return Ok(new Response<EditWorkFlowViewModel>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
            }
        }
        [HttpPost("EditStudyResultStepAction")]
        [ProducesResponseType(200, Type = typeof(Nullable))]
        public IActionResult EditStudyResultStepAction(EditStepActionProposalApprovedViewModel Workflow)
        {
            if (TryValidateModel(Workflow, nameof(EditMailStepActionViewModel)))
            {
                var response = _unitOfWork.WorkflowService.EditStudyResultStepAction(Workflow);
                return Ok(response);
            }
            else
            {
                var ErrorMessages = from state in ModelState.Values
                                    from error in state.Errors
                                    select error.ErrorMessage;
                return Ok(new Response<EditWorkFlowViewModel>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
            }
        }
        [HttpPost("EditProposalApprovedStepAction")]
        [ProducesResponseType(200, Type = typeof(Nullable))]
        public IActionResult EditProposalApprovedStepAction(EditStepActionProposalApprovedViewModel Workflow)
        {
            if (TryValidateModel(Workflow, nameof(EditMailStepActionViewModel)))
            {
                var response = _unitOfWork.WorkflowService.EditProposalApprovedStepAction(Workflow);
                return Ok(response);
            }
            else
            {
                var ErrorMessages = from state in ModelState.Values
                                    from error in state.Errors
                                    select error.ErrorMessage;
                return Ok(new Response<EditWorkFlowViewModel>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
            }
        }
        [HttpPost("EditSelectTargetSupportStepAction")]
        [ProducesResponseType(200, Type = typeof(Nullable))]
        public IActionResult EditSelectTargetSupportStepAction(EditStepActionSelectTargetSupportViewModel Workflow)
        {
            if (TryValidateModel(Workflow, nameof(EditMailStepActionViewModel)))
            {
                var response = _unitOfWork.WorkflowService.EditSelectTargetSupportStepAction(Workflow);
                return Ok(response);
            }
            else
            {
                var ErrorMessages = from state in ModelState.Values
                                    from error in state.Errors
                                    select error.ErrorMessage;
                return Ok(new Response<EditWorkFlowViewModel>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
            }
        }
        [HttpPost("EditTelecomValidationStepAction")]
        [ProducesResponseType(200, Type = typeof(Nullable))]
        public IActionResult EditTelecomValidationStepAction(EditStepActionTelecomValidationViewModel Workflow)
        {
            if (TryValidateModel(Workflow, nameof(EditMailStepActionViewModel)))
            {
                var response = _unitOfWork.WorkflowService.EditTelecomValidationStepAction(Workflow);
                return Ok(response);
            }
            else
            {
                var ErrorMessages = from state in ModelState.Values
                                    from error in state.Errors
                                    select error.ErrorMessage;
                return Ok(new Response<EditWorkFlowViewModel>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
            }
        }
        [HttpPost("EditCivilValidationStepAction")]
        [ProducesResponseType(200, Type = typeof(Nullable))]
        public IActionResult EditCivilValidationStepAction(EditStepActionTelecomValidationViewModel Workflow)
        {
            if (TryValidateModel(Workflow, nameof(EditMailStepActionViewModel)))
            {
                var response = _unitOfWork.WorkflowService.EditCivilValidationStepAction(Workflow);
                return Ok(response);
            }
            else
            {
                var ErrorMessages = from state in ModelState.Values
                                    from error in state.Errors
                                    select error.ErrorMessage;
                return Ok(new Response<EditWorkFlowViewModel>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
            }
        }
        [HttpPost("EditTicketStatusStepAction")]
        [ProducesResponseType(200, Type = typeof(Nullable))]
        public IActionResult EditTicketStatusStepAction(EditStepActionTicketStatusViewModel Workflow)
        {
            if (TryValidateModel(Workflow, nameof(EditStepActionTicketStatusViewModel)))
            {
                var response = _unitOfWork.WorkflowService.EditTicketStatusStepAction(Workflow);
                return Ok(response);
            }
            else
            {
                var ErrorMessages = from state in ModelState.Values
                                    from error in state.Errors
                                    select error.ErrorMessage;
                return Ok(new Response<EditWorkFlowViewModel>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
            }
        }
        [HttpPost("WorkflowPermissions")]
        [ProducesResponseType(200, Type = typeof(Nullable))]
        public  IActionResult WorkflowPermissions(WorkFlowGroupsViewModel Workflow)
        {
            if (ModelState.IsValid)
            {
                var response =  _unitOfWork.WorkflowService.WorkflowPermissions(Workflow);
                return Ok(response);
            }
            else
            {
                var ErrorMessages = from state in ModelState.Values
                                    from error in state.Errors
                                    select error.ErrorMessage;
                return Ok(new Response<WorkFlowViewModel>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
            }
        }
        [HttpGet("DeleteWorkflow/{WorkflowId}")]
        [ProducesResponseType(200, Type = typeof(ListWorkFlowViewModel))]
        public IActionResult DeleteWorkflow(int WorkflowId)
        {
            var response = _unitOfWork.WorkflowService.DeleteWorkflow(WorkflowId);
            return Ok(response);
        }

        [HttpGet("ChangeWorkflowStatus/{WorkflowId}")]
        [ProducesResponseType(200, Type = typeof(ListWorkFlowViewModel))]
        public IActionResult ChangeWorkflowStatus(int WorkflowId)
        {
            var response = _unitOfWork.WorkflowService.ChangeWorkflowStatus(WorkflowId);
            return Ok(response);
        }

        // end of workflow


        // Workflow Type
        [HttpPost("GetAllWorkflowTyps/{WorkflowId}")]
        [ProducesResponseType(200, Type = typeof(List<ListWorkFlowTypeViewModel>))]
        public IActionResult GetAllWorkflowTyps([FromQuery]ParameterPagination parameterPagination, [FromBody]List<FilterObjectList> filters, int WorkflowId)
        {
            var response = _unitOfWork.WorkflowService.GetAllWorkflowTypes(parameterPagination, filters, WorkflowId);
            return Ok(response);
        }
        [HttpGet("GetWorkflowTypeById/{WorkflowTypeId}")]
        [ProducesResponseType(200, Type = typeof(ListWorkFlowTypeViewModel))]
        public IActionResult GetWorkflowTypeById(int WorkflowTypeId)
        {
            var response = _unitOfWork.WorkflowService.GetWorkflowTypeById(WorkflowTypeId);
            return Ok(response);
        }
        [HttpPost("AddWorkflowType")]
        [ProducesResponseType(200, Type = typeof(Nullable))]
        public IActionResult AddWorkflowType(AddWorkFlowTypeViewModel WorkflowType)
        {
            try
            {
                if (TryValidateModel(WorkflowType, nameof(AddWorkFlowTypeViewModel)))
                {
                    var response = _unitOfWork.WorkflowService.AddWorkflowType(WorkflowType);
                    return Ok(response);
                }
                else
                {
                    var ErrorMessages = from state in ModelState.Values
                                        from error in state.Errors
                                        select error.ErrorMessage;
                    return Ok(new Response<ListWorkFlowTypeViewModel>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
                }
            }
            catch (Exception ex)
            {
                return Ok(new Response<ListWorkFlowTypeViewModel>(true, null, new string[] { ex.Message }, null, (int)Helpers.Constants.ApiReturnCode.Invalid));
            }
        }
        [HttpPost("UpdateWorkflowType")]
        [ProducesResponseType(200, Type = typeof(Nullable))]
        public IActionResult UpdateWorkflowType(EditWorkFlowTypeViewModel Workflowtype)
        {
            if (TryValidateModel(Workflowtype, nameof(EditWorkFlowTypeViewModel)))
            {
                var response = _unitOfWork.WorkflowService.UpdateWorkflowType(Workflowtype);
                return Ok(response);
            }
            else
            {
                var ErrorMessages = from state in ModelState.Values
                                    from error in state.Errors
                                    select error.ErrorMessage;
                return Ok(new Response<ListWorkFlowTypeViewModel>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
            }
        }
        
        [HttpGet("DeleteWorkflowType/{WorkflowTypeId}")]
        [ProducesResponseType(200, Type = typeof(ListWorkFlowTypeViewModel))]
        public IActionResult DeleteWorkflowType(int WorkflowTypeId)
        {
            var response = _unitOfWork.WorkflowService.DeleteWorkflowType(WorkflowTypeId);
            return Ok(response);
        }



        // end of workflow Type

        // step 
        /*
        [HttpPost("GetAllSteps")]
        [ProducesResponseType(200, Type = typeof(List<StepListViewModel>))]
        public IActionResult GetAllSteps([FromQuery]ParameterPagination parameterPagination, [FromBody]List<FilterObjectList> filters)
        {
            var response = _unitOfWork.WorkflowService.GetAllSteps(parameterPagination, filters);
            return Ok(response);
        }
        [HttpPost("GetWorkFlowSteps/{workflowId}")]
        [ProducesResponseType(200, Type = typeof(List<StepListViewModel>))]
        public IActionResult GetWorkFlowSteps([FromQuery]ParameterPagination parameterPagination, [FromBody]List<FilterObjectList> filters, int workflowId)
        {
            var response = _unitOfWork.WorkflowService.GetWorkFlowSteps(parameterPagination, filters, workflowId);
            return Ok(response);
        }
        [HttpPost("GetSubStep/{StepId}")]
        [ProducesResponseType(200, Type = typeof(List<StepListViewModel>))]
        public IActionResult GetSubStep([FromQuery]ParameterPagination parameterPagination, [FromBody]List<FilterObjectList> filters, int StepId)
        {
            var response = _unitOfWork.WorkflowService.GetSubStep(parameterPagination, filters, StepId);
            return Ok(response);
        }
        [HttpGet("GetStepById/{StepId}")]
        [ProducesResponseType(200, Type = typeof(StepListViewModel))]
        public IActionResult GetStepById(int StepId)
        {
            var response = _unitOfWork.WorkflowService.GetStepById(StepId);
            return Ok(response);
        }
        [HttpPost("AddStep")]
        [ProducesResponseType(200, Type = typeof(Nullable))]
        public async Task<IActionResult> AddStep(StepAddViewModel step)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var response = await _unitOfWork.WorkflowService.AddStep(step);
                    return Ok(response);
                }
                else
                {
                    var ErrorMessages = from state in ModelState.Values
                                        from error in state.Errors
                                        select error.ErrorMessage;
                    return Ok(new Response<StepEditViewModel>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
                }
            }
            catch (Exception ex)
            {
                return Ok(new Response<StepEditViewModel>(true, null, new string[] { ex.Message }, null, (int)Helpers.Constants.ApiReturnCode.Invalid));
            }
        }
        [HttpPost("UpdateStep")]
        [ProducesResponseType(200, Type = typeof(Nullable))]
        public async Task<IActionResult> UpdateStep(StepEditViewModel step)
        {
            if (ModelState.IsValid)
            {
                var response = await _unitOfWork.WorkflowService.UpdateStep(step);
                return Ok(response);
            }
            else
            {
                var ErrorMessages = from state in ModelState.Values
                                    from error in state.Errors
                                    select error.ErrorMessage;
                return Ok(new Response<StepEditViewModel>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
            }
        }
        //*/
        // end of step

        //------------- Actions

        [HttpGet("GetAllActions")]
        [ProducesResponseType(200, Type = typeof(List<ActionListViewModel>))]
        public IActionResult GetAllActions()
        {
            var response = _unitOfWork.WorkflowService.GetAllActions();
            return Ok(response);
        }
        [HttpGet("GetConditionActions")]
        [ProducesResponseType(200, Type = typeof(List<ListConditionActionViewModel>))]
        public IActionResult GetConditionActions()
        {
            var response = _unitOfWork.WorkflowService.GetConditionActions();
            return Ok(response);
        }
        [HttpGet("GetTelecomValidationActions")]
        [ProducesResponseType(200, Type = typeof(List<ListConditionActionViewModel>))]
        public IActionResult GetTelecomValidationActions()
        {
            var response = _unitOfWork.WorkflowService.GetTelecomValidationActions();
            return Ok(response);
        }
        [HttpGet("GetCivilValidationActions")]
        [ProducesResponseType(200, Type = typeof(List<ListConditionActionViewModel>))]
        public IActionResult GetCivilValidationActions()
        {
            var response = _unitOfWork.WorkflowService.GetCivilValidationActions();
            return Ok(response);
        }
        [HttpGet("GetCivilDecisionActions")]
        [ProducesResponseType(200, Type = typeof(List<ListCivilDecisionActionViewModel>))]
        public IActionResult GetCivilDecisionActions()
        {
            var response = _unitOfWork.WorkflowService.GetCivilDecisionActions();
            return Ok(response);
        }
        [HttpGet("GetCivilDecisionAction")]
        [ProducesResponseType(200, Type = typeof(ListCivilDecisionActionViewModel))]
        public IActionResult GetCivilDecisionAction()
        {
            var response = _unitOfWork.WorkflowService.GetCivilDecisionAction();
            return Ok(response);
        }
        [HttpGet("GetAvailableSpaceOptions")]
        [ProducesResponseType(200, Type = typeof(List<ListActionOptionViewModel>))]
        public IActionResult GetAvailableSpaceOptions()
        {
            var response = _unitOfWork.WorkflowService.GetAvailableSpaceOptions();
            return Ok(response);
        }

        [HttpGet("GetActionById/{ActionId}")]
        [ProducesResponseType(200, Type = typeof(ActionListViewModel))]
        public IActionResult GetActionById(int ActionId)
        {
            var response = _unitOfWork.WorkflowService.GetActionById(ActionId);
            return Ok(response);
        }

        // end of Action
        //------------- StepActions

        [HttpGet("GetAllStepActions")]
        [ProducesResponseType(200, Type = typeof(List<ListStepActionViewModel>))]
        public IActionResult GetAllStepActions()
        {
            var response = _unitOfWork.WorkflowService.GetAllStepActions();
            return Ok(response);
        }

        [HttpGet("GetWorkFlowStepActions/{WorkFlowId}")]
        [ProducesResponseType(200, Type = typeof(List<ListStepActionViewModel>))]
        public IActionResult GetWorkFlowStepActions(int WorkFlowId)
        {
            var response = _unitOfWork.WorkflowService.GetWorkFlowStepActions(WorkFlowId);
            return Ok(response);
        }

        [HttpGet("GetStepActionById/{StepActionId}")]
        [ProducesResponseType(200, Type = typeof(StepActionWithNamesViewModel))]
        public IActionResult GetStepActionById(int StepActionId)
        {
            var response = _unitOfWork.WorkflowService.GetStepActionById(StepActionId);
            return Ok(response);
        }
        /*
        [HttpPost("AddStepAction")]
        [ProducesResponseType(200, Type = typeof(Nullable))]
        public IActionResult AddStepAction(AddStepActionViewModel step)
        {
            try
            {
                if (TryValidateModel(step, nameof(AddStepActionViewModel)))
                {
                    var response = _unitOfWork.WorkflowService.AddStepAction(step);
                    return Ok(response);
                }
                else
                {
                    var ErrorMessages = from state in ModelState.Values
                                        from error in state.Errors
                                        select error.ErrorMessage;
                    return Ok(new Response<ListStepActionViewModel>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
                }
            }
            catch (Exception ex)
            {
                return Ok(new Response<ListStepActionViewModel>(true, null, new string[] { ex.Message }, null, (int)Helpers.Constants.ApiReturnCode.Invalid));
            }
        }
        //*/

        [HttpPost("AddMailStepAction")]
        [ProducesResponseType(200, Type = typeof(Nullable))]
        public IActionResult AddMailStepAction(AddMailStepActionViewModel step)
        {
            try
            {
                if (TryValidateModel(step, nameof(AddMailStepActionViewModel)))
                {
                    var response = _unitOfWork.WorkflowService.AddMailStepAction(step);
                    return Ok(response);
                }
                else
                {
                    var ErrorMessages = from state in ModelState.Values
                                        from error in state.Errors
                                        select error.ErrorMessage;
                    return Ok(new Response<StepActionWithNamesViewModel>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
                }
            }
            catch (Exception ex)
            {
                return Ok(new Response<StepActionWithNamesViewModel>(true, null, new string[] { ex.Message }, null, (int)Helpers.Constants.ApiReturnCode.Invalid));
            }
        }
        [HttpPost("AddUploadFileStepAction")]
        [ProducesResponseType(200, Type = typeof(Nullable))]
        public IActionResult AddUploadFileStepAction(AddUploadFileStepActionViewModel step)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var response = _unitOfWork.WorkflowService.AddUploadFileStepAction(step);
                    return Ok(response);
                }
                else
                {
                    var ErrorMessages = from state in ModelState.Values
                                        from error in state.Errors
                                        select error.ErrorMessage;
                    return Ok(new Response<StepActionWithNamesViewModel>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
                }
            }
            catch (Exception ex)
            {
                return Ok(new Response<StepActionWithNamesViewModel>(true, null, new string[] { ex.Message }, null, (int)Helpers.Constants.ApiReturnCode.Invalid));
            }
        }
        [HttpPost("AddInsertDataStepAction")]
        [ProducesResponseType(200, Type = typeof(Nullable))]
        public IActionResult AddInsertDataStepAction(AddStepActionInsertDataViewModel step)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var response = _unitOfWork.WorkflowService.AddInsertDataStepAction(step);
                    return Ok(response);
                }
                else
                {
                    var ErrorMessages = from state in ModelState.Values
                                        from error in state.Errors
                                        select error.ErrorMessage;
                    return Ok(new Response<StepActionWithNamesViewModel>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
                }
            }
            catch (Exception ex)
            {
                return Ok(new Response<StepActionWithNamesViewModel>(true, null, new string[] { ex.Message }, null, (int)Helpers.Constants.ApiReturnCode.Invalid));
            }
        }
        [HttpPost("AddUpdateDataStepAction")]
        [ProducesResponseType(200, Type = typeof(Nullable))]
        public IActionResult AddUpdateDataStepAction(AddStepActionInsertDataViewModel step)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var response = _unitOfWork.WorkflowService.AddUpdateDataStepAction(step);
                    return Ok(response);
                }
                else
                {
                    var ErrorMessages = from state in ModelState.Values
                                        from error in state.Errors
                                        select error.ErrorMessage;
                    return Ok(new Response<StepActionWithNamesViewModel>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
                }
            }
            catch (Exception ex)
            {
                return Ok(new Response<StepActionWithNamesViewModel>(true, null, new string[] { ex.Message }, null, (int)Helpers.Constants.ApiReturnCode.Invalid));
            }
        }
        [HttpPost("AddCorrectDataStepAction")]
        [ProducesResponseType(200, Type = typeof(Nullable))]
        public IActionResult AddCorrectDataStepAction(AddStepActionInsertDataViewModel step)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var response = _unitOfWork.WorkflowService.AddCorrectDataStepAction(step);
                    return Ok(response);
                }
                else
                {
                    var ErrorMessages = from state in ModelState.Values
                                        from error in state.Errors
                                        select error.ErrorMessage;
                    return Ok(new Response<StepActionWithNamesViewModel>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
                }
            }
            catch (Exception ex)
            {
                return Ok(new Response<StepActionWithNamesViewModel>(true, null, new string[] { ex.Message }, null, (int)Helpers.Constants.ApiReturnCode.Invalid));
            }
        }
        [HttpPost("AddStepActionApplyCalculation")]
        [ProducesResponseType(200, Type = typeof(Nullable))]
        public IActionResult AddStepActionApplyCalculation(AddStepActionApplyCalculationViewModel step)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var response = _unitOfWork.WorkflowService.AddStepActionApplyCalculation(step);
                    return Ok(response);
                }
                else
                {
                    var ErrorMessages = from state in ModelState.Values
                                        from error in state.Errors
                                        select error.ErrorMessage;
                    return Ok(new Response<StepActionWithNamesViewModel>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
                }
            }
            catch (Exception ex)
            {
                return Ok(new Response<StepActionWithNamesViewModel>(true, null, new string[] { ex.Message }, null, (int)Helpers.Constants.ApiReturnCode.Invalid));
            }
        }
        [HttpPost("AddConditionStepAction")]
        [ProducesResponseType(200, Type = typeof(Nullable))]
        public IActionResult AddConditionStepAction(AddStepActionConditionViewModel step)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var response = _unitOfWork.WorkflowService.AddConditionStepAction(step);
                    return Ok(response);
                }
                else
                {
                    var ErrorMessages = from state in ModelState.Values
                                        from error in state.Errors
                                        select error.ErrorMessage;
                    return Ok(new Response<StepActionWithNamesViewModel>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
                }
            }
            catch (Exception ex)
            {
                return Ok(new Response<StepActionWithNamesViewModel>(true, null, new string[] { ex.Message }, null, (int)Helpers.Constants.ApiReturnCode.Invalid));
            }
        }
        [HttpPost("AddSelectTargetSupportStepAction")]
        [ProducesResponseType(200, Type = typeof(Nullable))]
        public IActionResult AddSelectTargetSupportStepAction(AddStepActionSelectTargetSupportViewModel step)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var response = _unitOfWork.WorkflowService.AddSelectTargetSupportStepAction(step);
                    return Ok(response);
                }
                else
                {
                    var ErrorMessages = from state in ModelState.Values
                                        from error in state.Errors
                                        select error.ErrorMessage;
                    return Ok(new Response<StepActionWithNamesViewModel>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
                }
            }
            catch (Exception ex)
            {
                return Ok(new Response<StepActionWithNamesViewModel>(true, null, new string[] { ex.Message }, null, (int)Helpers.Constants.ApiReturnCode.Invalid));
            }
        }
        [HttpPost("AddCheckAvailableSpaceStepAction")]
        [ProducesResponseType(200, Type = typeof(Nullable))]
        public IActionResult AddCheckAvailableSpaceStepAction(AddStepActionCheckAvailableSpaceViewModel step)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var response = _unitOfWork.WorkflowService.AddCheckAvailableSpaceStepAction(step);
                    return Ok(response);
                }
                else
                {
                    var ErrorMessages = from state in ModelState.Values
                                        from error in state.Errors
                                        select error.ErrorMessage;
                    return Ok(new Response<StepActionWithNamesViewModel>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
                }
            }
            catch (Exception ex)
            {
                return Ok(new Response<StepActionWithNamesViewModel>(true, null, new string[] { ex.Message }, null, (int)Helpers.Constants.ApiReturnCode.Invalid));
            }
        }
        [HttpPost("AddStudyResultStepAction")]
        [ProducesResponseType(200, Type = typeof(Nullable))]
        public IActionResult AddStudyResultStepAction(AddStepActionProposalApprovedViewModel step)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var response = _unitOfWork.WorkflowService.AddStudyResultStepAction(step);
                    return Ok(response);
                }
                else
                {
                    var ErrorMessages = from state in ModelState.Values
                                        from error in state.Errors
                                        select error.ErrorMessage;
                    return Ok(new Response<StepActionWithNamesViewModel>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
                }
            }
            catch (Exception ex)
            {
                return Ok(new Response<StepActionWithNamesViewModel>(true, null, new string[] { ex.Message }, null, (int)Helpers.Constants.ApiReturnCode.Invalid));
            }
        }
        [HttpPost("AddProposalApprovedStepAction")]
        [ProducesResponseType(200, Type = typeof(Nullable))]
        public IActionResult AddProposalApprovedStepAction(AddStepActionProposalApprovedViewModel step)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var response = _unitOfWork.WorkflowService.AddProposalApprovedStepAction(step);
                    return Ok(response);
                }
                else
                {
                    var ErrorMessages = from state in ModelState.Values
                                        from error in state.Errors
                                        select error.ErrorMessage;
                    return Ok(new Response<StepActionWithNamesViewModel>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
                }
            }
            catch (Exception ex)
            {
                return Ok(new Response<StepActionWithNamesViewModel>(true, null, new string[] { ex.Message }, null, (int)Helpers.Constants.ApiReturnCode.Invalid));
            }
        }
        [HttpPost("AddTelecomValidationStepAction")]
        [ProducesResponseType(200, Type = typeof(Nullable))]
        public IActionResult AddTelecomValidationStepAction(AddStepActionTelecomValidationViewModel step)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var response = _unitOfWork.WorkflowService.AddTelecomValidationStepAction(step);
                    return Ok(response);
                }
                else
                {
                    var ErrorMessages = from state in ModelState.Values
                                        from error in state.Errors
                                        select error.ErrorMessage;
                    return Ok(new Response<StepActionWithNamesViewModel>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
                }
            }
            catch (Exception ex)
            {
                return Ok(new Response<StepActionWithNamesViewModel>(true, null, new string[] { ex.Message }, null, (int)Helpers.Constants.ApiReturnCode.Invalid));
            }
        }
        [HttpPost("AddCivilValidationStepAction")]
        [ProducesResponseType(200, Type = typeof(Nullable))]
        public IActionResult AddCivilValidationStepAction(AddStepActionTelecomValidationViewModel step)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var response = _unitOfWork.WorkflowService.AddCivilValidationStepAction(step);
                    return Ok(response);
                }
                else
                {
                    var ErrorMessages = from state in ModelState.Values
                                        from error in state.Errors
                                        select error.ErrorMessage;
                    return Ok(new Response<StepActionWithNamesViewModel>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
                }
            }
            catch (Exception ex)
            {
                return Ok(new Response<StepActionWithNamesViewModel>(true, null, new string[] { ex.Message }, null, (int)Helpers.Constants.ApiReturnCode.Invalid));
            }
        }
        [HttpPost("AddCivilDecisionStepAction")]
        [ProducesResponseType(200, Type = typeof(Nullable))]
        public IActionResult AddCivilDecisionStepAction(AddStepActionCivilDecisionViewModel step)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var response = _unitOfWork.WorkflowService.AddCivilDecisionStepAction(step);
                    return Ok(response);
                }
                else
                {
                    var ErrorMessages = from state in ModelState.Values
                                        from error in state.Errors
                                        select error.ErrorMessage;
                    return Ok(new Response<StepActionWithNamesViewModel>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
                }
            }
            catch (Exception ex)
            {
                return Ok(new Response<StepActionWithNamesViewModel>(true, null, new string[] { ex.Message }, null, (int)Helpers.Constants.ApiReturnCode.Invalid));
            }
        }
        [HttpPost("AddTicketStatusStepAction")]
        [ProducesResponseType(200, Type = typeof(Nullable))]
        public IActionResult AddTicketStatusStepAction(AddTicketStatusStepActionViewModel step)
        {
            try
            {
                if (TryValidateModel(step, nameof(AddTicketStatusStepActionViewModel)))
                {
                    var response = _unitOfWork.WorkflowService.AddTicketStatusStepAction(step);
                    return Ok(response);
                }
                else
                {
                    var ErrorMessages = from state in ModelState.Values
                                        from error in state.Errors
                                        select error.ErrorMessage;
                    return Ok(new Response<StepActionWithNamesViewModel>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
                }
            }
            catch (Exception ex)
            {
                return Ok(new Response<StepActionWithNamesViewModel>(true, null, new string[] { ex.Message }, null, (int)Helpers.Constants.ApiReturnCode.Invalid));
            }
        }
        [HttpPost("SetStepActionPermission")]
        [ProducesResponseType(200, Type = typeof(Nullable))]
        public IActionResult SetStepActionPermission(int StepActionId, StepActionGroupPermission permissions)
        {
            var response = _unitOfWork.WorkflowService.SetStepActionPermission(StepActionId, permissions);
            return Ok(response);
        }

        [HttpPost("UpdateStepAction")]
        [ProducesResponseType(200, Type = typeof(Nullable))]
        public IActionResult UpdateStepAction(EditStepActionViewModel step)
        {
            if (TryValidateModel(step, nameof(EditStepActionViewModel)))
            {
                var response = _unitOfWork.WorkflowService.UpdateStepAction(step);
                return Ok(response);
            }
            else
            {
                var ErrorMessages = from state in ModelState.Values
                                    from error in state.Errors
                                    select error.ErrorMessage;
                return Ok(new Response<ListStepActionViewModel>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
            }
        }

        [HttpGet("MoveUpStepAction/{StepActionId}")]
        [ProducesResponseType(200, Type = typeof(Nullable))]
        public IActionResult MoveUpStepAction(int StepActionId)
        {
            var response = _unitOfWork.WorkflowService.MoveUpStepAction(StepActionId);
            return Ok(response);
        }
        [HttpGet("MoveDownStepAction/{StepActionId}")]
        [ProducesResponseType(200, Type = typeof(Nullable))]
        public IActionResult MoveDownStepAction(int StepActionId)
        {
            var response = _unitOfWork.WorkflowService.MoveDownStepAction(StepActionId);
            return Ok(response);
        }
        [HttpGet("DelStepAction/{StepActionId}")]
        [ProducesResponseType(200, Type = typeof(Nullable))]
        public IActionResult DelStepAction(int StepActionId)
        {
            var response = _unitOfWork.WorkflowService.DelStepAction(StepActionId);
            return Ok(response);
        }
        // end of StepActions



    }

}