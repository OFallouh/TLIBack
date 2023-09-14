using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TLIS_DAL.Helper;
using TLIS_DAL.Helper.Filters;
using TLIS_DAL.Helpers;
using TLIS_DAL.ViewModels.GuyLineTypeDTOs;
using TLIS_Service.Helpers;
using TLIS_Service.ServiceBase;

namespace TLIS_API.Controllers
{
    [ServiceFilter(typeof(LogFilterAttribute))]
    [Route("api/[controller]")]
    [ApiController]
    public class GuyLineTypeController : ControllerBase
    {
        private IUnitOfWorkService _unitOfWorkService;
        public GuyLineTypeController(IUnitOfWorkService unitOfWorkService)
        {
            _unitOfWorkService = unitOfWorkService;
        }
        [HttpPost("getAll")]
        [ProducesResponseType(200, Type = typeof(List<GuyLineTypeViewModel>))]
        public async Task<IActionResult> GetGuyLineTypes([FromQueryAttribute]ParameterPagination parameterPagination, [FromBody] List<FilterObjectList> filters = null)
        {
            var response = await _unitOfWorkService.GuyLineTypeService.GetGuyLineTypes(parameterPagination, filters);
            return Ok(response);
        }
        [HttpGet("getById/{id}")]
        [ProducesResponseType(200, Type = typeof(GuyLineTypeViewModel))]
        public IActionResult GetGuyLineType(int id)
        {
            var response = _unitOfWorkService.GuyLineTypeService.GetGuyLineType(id);
            return Ok(response);
        }
        [HttpPost("AddGuyLineType")]
        [ProducesResponseType(200, Type = typeof(GuyLineTypeViewModel))]
        public async Task<IActionResult> AddGuyLineType([FromBody]AddGuyLineTypeViewModel addGuyLine)
        {
            if (TryValidateModel(addGuyLine, nameof(AddGuyLineTypeViewModel)))
            {
                var response = await _unitOfWorkService.GuyLineTypeService.AddGuyLineType(addGuyLine);
                return Ok(response);
            }
            else
            {
                var ErrorMessages = from state in ModelState.Values
                                    from error in state.Errors
                                    select error.ErrorMessage;
                return Ok(new Response<AddGuyLineTypeViewModel>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
            }
        }
        [HttpPost("EditBaseCivilWithLegsType")]
        [ProducesResponseType(200, Type = typeof(GuyLineTypeViewModel))]
        public async Task<IActionResult> EditBaseCivilWithLegsType([FromBody]EditGuyLineTypeViewModel editGuyLine)
        {
            if(TryValidateModel(editGuyLine, nameof(EditGuyLineTypeViewModel)))
            {
                var response = await _unitOfWorkService.GuyLineTypeService.EditGuyLineType(editGuyLine);
                return Ok(response);
            }
            else
            {
                var ErrorMessages = from state in ModelState.Values
                                    from error in state.Errors
                                    select error.ErrorMessage;
                return Ok(new Response<EditGuyLineTypeViewModel>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
            }
        }
    }
}