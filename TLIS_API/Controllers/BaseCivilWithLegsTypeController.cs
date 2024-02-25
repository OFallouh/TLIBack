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
using TLIS_DAL.ViewModels.BaseCivilWithLegsTypeDTOs;
using TLIS_Service.Helpers;
using TLIS_Service.ServiceBase;

namespace TLIS_API.Controllers
{
    [ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
    [ServiceFilter(typeof(LogFilterAttribute))]
    [Route("api/[controller]")]
    [ApiController]
    public class BaseCivilWithLegsTypeController : Controller
    {
        private IUnitOfWorkService _unitOfWorkService;
        public BaseCivilWithLegsTypeController(IUnitOfWorkService unitOfWorkService)
        {
            _unitOfWorkService = unitOfWorkService;
        }

        [HttpPost("getAll")]
        [ProducesResponseType(200, Type = typeof(List<BaseCivilWithLegsTypeViewModel>))]
        public async Task<IActionResult> GetBaseCivilWithLegsTypes([FromQueryAttribute]ParameterPagination parameterPagination, [FromBody] List<FilterObjectList> Filter = null)
        {
            var response = await _unitOfWorkService.BaseCivilWithLegsTypeService.GetBaseCivilWithLegsTypes(parameterPagination, Filter);
            return Ok(response);
        }

        [HttpGet("getById/{id}")]
        [ProducesResponseType(200, Type = typeof(BaseCivilWithLegsTypeViewModel))]
        public async Task<IActionResult> GetBaseCivilWithLegsType(int id)
        {
            var response = await _unitOfWorkService.BaseCivilWithLegsTypeService.GetBaseCivilWithLegsType(id);
            return Ok(response);

        }

        [HttpPost("AddBaseCivilWithLegsType")]
        [ProducesResponseType(200, Type = typeof(BaseCivilWithLegsTypeViewModel))]
        public async Task<IActionResult> AddBaseCivilWithLegsType([FromBody]AddBaseCivilWithLegsTypeViewModel addBaseCivil)
        {
            if(TryValidateModel(addBaseCivil, nameof(AddBaseCivilWithLegsTypeViewModel)))
            {
                var response = await _unitOfWorkService.BaseCivilWithLegsTypeService.AddBaseCivilWithLegsType(addBaseCivil);
                return Ok(response);
            }
            else
            {
                var ErrorMessages = from state in ModelState.Values
                                    from error in state.Errors
                                    select error.ErrorMessage;
                return Ok(new Response<BaseCivilWithLegsTypeViewModel>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
            }
        }

        [HttpPost("EditBaseCivilWithLegsType")]
        [ProducesResponseType(200, Type = typeof(BaseCivilWithLegsTypeViewModel))]
        public async Task<IActionResult> EditBaseCivilWithLegsType([FromBody]EditBaseCivilWithLegsTypeViewModel editBaseCivil)
        {
            if(TryValidateModel(editBaseCivil, nameof(EditBaseCivilWithLegsTypeViewModel)))
            {
                var response = await _unitOfWorkService.BaseCivilWithLegsTypeService.EditBaseCivilWithLegsType(editBaseCivil);
                return Ok(response);
            }
            else
            {
                var ErrorMessages = from state in ModelState.Values
                                    from error in state.Errors
                                    select error.ErrorMessage;
                return Ok(new Response<BaseCivilWithLegsTypeViewModel>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
            }
        }
    }
}