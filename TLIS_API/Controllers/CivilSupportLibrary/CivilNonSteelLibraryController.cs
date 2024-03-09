using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebSockets;
using Microsoft.Extensions.Configuration;
using TLIS_API.Helpers;
using TLIS_API.Middleware.WorkFlow;
using TLIS_DAL.Helper;
using TLIS_DAL.Helper.Filters;
using TLIS_DAL.Helpers;
using TLIS_DAL.ViewModelBase;
using TLIS_DAL.ViewModels.CivilNonSteelDTOs;
using TLIS_DAL.ViewModels.DynamicAttDTOs;
using TLIS_DAL.ViewModels.DynamicAttInstValueDTOs;
using TLIS_Service.Helpers;
using TLIS_Service.ServiceBase;
using Constants = TLIS_API.Helpers.Constants;

namespace TLIS_API.Controllers
{
    [ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
    [ServiceFilter(typeof(LogFilterAttribute))]
    [Route("api/[controller]")]
    public class CivilNonSteelLibraryController : Controller
    {
        private readonly IUnitOfWorkService _unitOfWorkService;
        private readonly IConfiguration _configuration;
        public CivilNonSteelLibraryController(IUnitOfWorkService unitOfWorkService, IConfiguration configuration)
        {
            _unitOfWorkService = unitOfWorkService;
            _configuration = configuration;
        }
        [HttpPost("getAll")]
        [ProducesResponseType(200, Type = typeof(List<CivilNonSteelLibraryViewModel>))]
        public IActionResult GetCivilNonSteelLibrary([FromBody]List<FilterObjectList> filters, [FromQuery]ParameterPagination parameterPagination)
        {
            var response = _unitOfWorkService.CivilLibraryService.getCivilNonSteelLibraries(filters, parameterPagination);
            return Ok(response);
        }
        [HttpPost("GetCivilNonSteelLibrariesEnabledAtt")]
        [ProducesResponseType(200, Type = typeof(Response<ReturnWithFilters<object>>))]
        public IActionResult GetCivilNonSteelLibrariesEnabledAtt([FromBody] CombineFilters CombineFilters, bool WithFilterData, [FromQuery]ParameterPagination parameterPagination)
        {
            var response = _unitOfWorkService.CivilLibraryService.GetCivilNonSteelLibrariesEnabledAtt(CombineFilters, WithFilterData, parameterPagination);
            return Ok(response);
        }
        [HttpGet("getById/{id}")]
        [ProducesResponseType(200, Type = typeof(CivilNonSteelLibraryViewModel))]
        public IActionResult GetCivilNonSteelLibrary(int id)
        {
            var response = _unitOfWorkService.CivilLibraryService.GetById(id, Helpers.Constants.CivilType.TLIcivilNonSteelLibrary.ToString());
            return Ok(response);

        }
        [HttpPost("AddCivilNonSteelLibrary")]
        [ProducesResponseType(200, Type = typeof(AddCivilNonSteelLibraryViewModel))]
        public IActionResult AddCivilNonSteelLibrary([FromBody]AddCivilNonSteelLibraryViewModel addCivilNonSteelLibraryViewModel)
        {
            if(TryValidateModel(addCivilNonSteelLibraryViewModel, nameof(AddCivilNonSteelLibraryViewModel)))
            {
                var ConnectionString = _configuration["ConnectionStrings:ActiveConnection"];
                var response = _unitOfWorkService.CivilLibraryService.AddCivilLibrary(Helpers.Constants.CivilType.TLIcivilNonSteelLibrary.ToString(), addCivilNonSteelLibraryViewModel, ConnectionString);
                return Ok(response);
            }
            else
            {
                var ErrorMessages = from state in ModelState.Values
                                  from error in state.Errors
                                   select error.ErrorMessage;
              return Ok(new Response<AddCivilNonSteelLibraryViewModel>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
            }
        }
        [HttpPost("EditCivilNonSteelLibrary")]
        [ProducesResponseType(200, Type = typeof(EditCivilNonSteelLibraryViewModel))]
        public async Task<IActionResult> EditCivilNonSteelLibrary([FromBody]EditCivilNonSteelLibraryViewModel editCivilNonSteelLibraryViewModel)
        {
            if(TryValidateModel(editCivilNonSteelLibraryViewModel, nameof(EditCivilNonSteelLibraryViewModel)))
            {
                var response = await _unitOfWorkService.CivilLibraryService.EditCivilLibrary(editCivilNonSteelLibraryViewModel, Helpers.Constants.CivilType.TLIcivilNonSteelLibrary.ToString());
                return Ok(response);
            }
            else
            {
                var ErrorMessages = from state in ModelState.Values
                                    from error in state.Errors
                                    select error.ErrorMessage;
                return Ok(new Response<EditCivilNonSteelLibraryViewModel>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
            }
        }
        [HttpPost("DisableCivilNonSteelLibrary")]
        [ProducesResponseType(200, Type = typeof(List<CivilNonSteelLibraryViewModel>))]
        public async Task<IActionResult> DisableCivilNonSteelLibrary(int id)
        {
            var response = await _unitOfWorkService.CivilLibraryService.Disable(id, Helpers.Constants.CivilType.TLIcivilNonSteelLibrary.ToString());
            return Ok(response);
        }
        [HttpPost("DeleteCivilNonSteelLibrary")]
        [ProducesResponseType(200, Type = typeof(List<CivilNonSteelLibraryViewModel>))]
        public async Task<IActionResult> DeleteCivilNonSteelLibrary(int id)
        {
            var response = await _unitOfWorkService.CivilLibraryService.Delete(id, Helpers.Constants.CivilType.TLIcivilNonSteelLibrary.ToString());
            return Ok(response);
        }
        [HttpGet("GetForAddCivilNonSteelLibrary")] 
        public IActionResult GetForAddCivilNonSteelLibrary()
        {
            var response = _unitOfWorkService.CivilLibraryService.GetForAdd(Helpers.Constants.CivilType.TLIcivilNonSteelLibrary.ToString());
            return Ok(response);
        }

    }
}