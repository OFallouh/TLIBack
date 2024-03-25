using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TLIS_API.Middleware.WorkFlow;
using TLIS_DAL.Helpers;
using TLIS_DAL.ViewModels.CivilWithoutLegCategoryDTOs;
using TLIS_Service.Helpers;
using TLIS_Service.ServiceBase;

namespace TLIS_API.Controllers
{
    [ServiceFilter(typeof(LogFilterAttribute))]
    [Route("api/[controller]")]
    [ApiController]
    public class CivilWithoutLegCategoryController : ControllerBase
    {
        private IUnitOfWorkService _unitOfWorkService;
        public CivilWithoutLegCategoryController(IUnitOfWorkService unitOfWorkService)
        {
            _unitOfWorkService = unitOfWorkService;
        }
        [ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
        [HttpGet("getAll")]
        [ProducesResponseType(200, Type = typeof(List<CivilWithoutLegCategoryViewModel>))]
        public async Task<IActionResult> GetList()
        {
            var response = await _unitOfWorkService.CivilWithoutLegCategoryService.GetList();
            return Ok(response);
        }
        [ServiceFilter(typeof(WorkFlowMiddleware))]
        [HttpPost("AddCivilWithoutLegCategory")]
        [ProducesResponseType(200, Type = typeof(CivilWithoutLegCategoryViewModel))]
        public async Task<IActionResult> AddCivilWithoutLegCategory([FromBody]AddCivilWithoutLegCategoryViewModel categoryViewModel)
        {
            if(TryValidateModel(categoryViewModel, nameof(AddCivilWithoutLegCategoryViewModel)))
            {
                var response = await _unitOfWorkService.CivilWithoutLegCategoryService.Add(categoryViewModel);
                return Ok(response);
            }
            else
            {
                var ErrorMessages = from state in ModelState.Values
                                    from error in state.Errors
                                    select error.ErrorMessage;
                return Ok(new Response<CivilWithoutLegCategoryViewModel>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
            }
        }
        [ServiceFilter(typeof(WorkFlowMiddleware))]
        [HttpPost("EditCivilWithoutLegCategory")]
        [ProducesResponseType(200, Type = typeof(CivilWithoutLegCategoryViewModel))]
        public async Task<IActionResult> EditCategoury([FromBody]CivilWithoutLegCategoryViewModel EditCategory)
        {
            if (TryValidateModel(EditCategory, nameof(CivilWithoutLegCategoryViewModel)))
            {
                var response = await _unitOfWorkService.CivilWithoutLegCategoryService.Edit(EditCategory);
                return Ok(response);
            }
            else
            {
                var ErrorMessages = from state in ModelState.Values
                                    from error in state.Errors
                                    select error.ErrorMessage;
                return Ok(new Response<CivilWithoutLegCategoryViewModel>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
            }
        }
        [ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
        [HttpGet("getById/{id}")]
        [ProducesResponseType(200, Type = typeof(CivilWithoutLegCategoryViewModel))]
        public async Task<IActionResult> GetCategoury(int id)
        {
            var response = await _unitOfWorkService.CivilWithoutLegCategoryService.GetCivilWithoutLegCategory(id);
            return Ok(response);
        }
        [ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
        [HttpPost("GetByName")]
        [ProducesResponseType(200, Type = typeof(CivilWithoutLegCategoryViewModel))]
        public IActionResult GetByName(string CategoryName)
        {
            var response = _unitOfWorkService.CivilWithoutLegCategoryService.GetCivilWithoutLegCategoryByName(CategoryName);
            return Ok(response);

        }
        [ServiceFilter(typeof(WorkFlowMiddleware))]
        [HttpPost("DisableCivilWithoutLegCategory")]
        [ProducesResponseType(200, Type = typeof(List<CivilWithoutLegCategoryViewModel>))]
        public async Task<IActionResult> DisableCivilWithoutLegCategory(int Id,bool disable )
        {
            var response = await _unitOfWorkService.CivilWithoutLegCategoryService.Disable_EnableCivilWithoutLegCategory(Id, disable);
            return Ok(response);
        }
    }
}