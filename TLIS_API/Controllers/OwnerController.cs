using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TLIS_DAL.Helper;
using TLIS_DAL.Helper.Filters;
using TLIS_DAL.Helpers;
using TLIS_DAL.ViewModels.OwnerDTOs;
using TLIS_Repository.Base;
using TLIS_Service.Helpers;
using TLIS_Service.ServiceBase;

namespace TLIS_API.Controllers
{
    [ServiceFilter(typeof(LogFilterAttribute))]
    [Route("api/[controller]")]
    [ApiController]
    public class OwnerController : ControllerBase
    {
        private IUnitOfWorkService _unitOfWorkService;
        public OwnerController(IUnitOfWorkService unitOfWorkService)
        {
            _unitOfWorkService = unitOfWorkService;
        }
        [HttpPost("getAll")]
        [ProducesResponseType(200, Type = typeof(List<OwnerViewModel>))]
        public async Task<IActionResult> GetOwner([FromQueryAttribute]ParameterPagination parameterPagination,[FromBody]List<FilterObjectList> filters = null)
        {
            var response = await _unitOfWorkService.OwnerService.GetOwners(parameterPagination, filters);
            return Ok(response);
        }
        [HttpGet("getById/{id}")]
        [ProducesResponseType(200, Type = typeof(OwnerViewModel))]
        public IActionResult GetOwner(int id)
        {
            var response = _unitOfWorkService.OwnerService.GetOwner(id);
            return Ok(response);
        }
        [HttpPost("AddOwner")]
        [ProducesResponseType(200, Type = typeof(AddOwnerViewModel))]
        public async Task<IActionResult> AddOwner([FromBody]AddOwnerViewModel addOwner)
        {
            if(TryValidateModel(addOwner, nameof(AddOwnerViewModel)))
            {
                var response = await _unitOfWorkService.OwnerService.AddOwner(addOwner);
                return Ok(response);
            }
            else
            {
                var ErrorMessages = from state in ModelState.Values
                                    from error in state.Errors
                                    select error.ErrorMessage;
                return Ok(new Response<AddOwnerViewModel>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
            }
        }
        [HttpPost("EditOwner")]
        [ProducesResponseType(200, Type = typeof(EditOwnerViewModel))]
        public async Task<IActionResult> EditOwner([FromBody]EditOwnerViewModel editOwner)
        {
            if(TryValidateModel(editOwner, nameof(EditOwnerViewModel)))
            {
                var response = await _unitOfWorkService.OwnerService.EditOwner(editOwner);
                return Ok(response);
            }
            else
            {
                var ErrorMessages = from state in ModelState.Values
                                    from error in state.Errors
                                    select error.ErrorMessage;
                return Ok(new Response<EditOwnerViewModel>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
            }
        }
    }
}