using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TLIS_API.Middleware.WorkFlow;
using TLIS_DAL.Helper;
using TLIS_DAL.Helper.Filters;
using TLIS_DAL.Helpers;
using TLIS_DAL.ViewModels.SupportTypeImplementedDTOs;
using TLIS_Service.Helpers;
using TLIS_Service.ServiceBase;

namespace TLIS_API.Controllers.CivilSupportLibrary
{
    [ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
    [ServiceFilter(typeof(LogFilterAttribute))]
    [Route("api/[controller]")]
    
    public class SupportTypeImplementedController : Controller
    {
        private IUnitOfWorkService _unitOfWorkService;
        public SupportTypeImplementedController(IUnitOfWorkService unitOfWorkService)
        {
            _unitOfWorkService = unitOfWorkService;
        }

        [HttpPost("getAll")]
        [ProducesResponseType(200, Type = typeof(List<SupportTypeImplementedViewModel>))]
        public async Task<IActionResult> GetSupportTypesImplemented([FromQueryAttribute]ParameterPagination parameterPagination, [FromBody] List<FilterObjectList> Filter = null)
        {
            var response = await _unitOfWorkService.SupportTypeImplementedService.GetSupportTypesImplemented(parameterPagination, Filter);
            return Ok(response);
        }

        [HttpGet("getById/{id}")]
        [ProducesResponseType(200, Type = typeof(SupportTypeImplementedViewModel))]
        public async Task<IActionResult> GetSupportTypeImplemented(int id)
        {
            var response = await _unitOfWorkService.SupportTypeImplementedService.GetById(id);
            return Ok(response);

        }

        [HttpPost("AddSupportTypeImplemented")]
        [ProducesResponseType(200, Type = typeof(List<AddSupportTypeImplementedViewModel>))]
        public async Task<IActionResult> AddSupportTypeImplemented([FromBody]AddSupportTypeImplementedViewModel addSupportTypeImplementedViewModel)
        {
            if(TryValidateModel(addSupportTypeImplementedViewModel, nameof(AddSupportTypeImplementedViewModel)))
            {
                var response = await _unitOfWorkService.SupportTypeImplementedService.AddSupportTypeImplemented(addSupportTypeImplementedViewModel);
                return Ok(response);
            }
            else
            {
                var ErrorMessages = from state in ModelState.Values
                                    from error in state.Errors
                                    select error.ErrorMessage;
                return Ok(new Response<AddSupportTypeImplementedViewModel>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
            }
        }

        [HttpPost("EditSupportTypeImplemented")]
        [ProducesResponseType(200, Type = typeof(List<EditSupportTypeImplementedViewModel>))]

        public async Task<IActionResult> EditSupportTypeImplemented([FromBody]EditSupportTypeImplementedViewModel editSupportTypeImplementedViewModel)
        {
            if(TryValidateModel(editSupportTypeImplementedViewModel, nameof(EditSupportTypeImplementedViewModel)))
            {
                var response = await _unitOfWorkService.SupportTypeImplementedService.EditSupportTypeImplemented(editSupportTypeImplementedViewModel);
                return Ok(response);
            }
            else
            {
                var ErrorMessages = from state in ModelState.Values
                                    from error in state.Errors
                                    select error.ErrorMessage;
                return Ok(new Response<EditSupportTypeImplementedViewModel>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
            }
        }


    }
}
