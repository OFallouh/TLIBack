using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TLIS_API.Helpers;
using TLIS_API.Middleware.WorkFlow;
using TLIS_DAL.Helper;
using TLIS_DAL.Helper.Filters;
using TLIS_DAL.Helpers;
using TLIS_DAL.ViewModelBase;
using TLIS_DAL.ViewModels.CivilWithoutLegDTOs;
using TLIS_DAL.ViewModels.DynamicAttDTOs;
using TLIS_DAL.ViewModels.DynamicAttInstValueDTOs;
using TLIS_Service.Helpers;
using TLIS_Service.ServiceBase;

namespace TLIS_API.Controllers
{
    [ServiceFilter(typeof(WorkFlowMiddleware))]
    [ServiceFilter(typeof(LogFilterAttribute))]
    [Route("api/[controller]")]
    public class CivilWithoutLegLibraryController : Controller
    {
        private readonly IUnitOfWorkService _unitOfWorkService;
        private readonly IConfiguration _configuration;
        public CivilWithoutLegLibraryController(IUnitOfWorkService unitOfWorkService, IConfiguration configuration)
        {
            _unitOfWorkService = unitOfWorkService;
            _configuration = configuration;
        }

        [HttpPost("getAll")]
        [ProducesResponseType(200, Type = typeof(List<CivilWithoutLegLibraryViewModel>))]
        public IActionResult GetCivilWithoutLegLibrary([FromBody]List<FilterObjectList> filters, bool WithFilterData, [FromQuery] ParameterPagination parameters)
        {
            var response = _unitOfWorkService.CivilLibraryService.getCivilWithoutLegLibraries(filters, WithFilterData, parameters);
            return Ok(response);
        }
        [HttpPost("GetCivilWithoutLegLibrariesEnabledAtt")]
        [ProducesResponseType(200, Type = typeof(Response<ReturnWithFilters<object>>))]
        public IActionResult GetCivilWithoutLegLibrariesEnabledAtt([FromBody] CombineFilters ComineOutPut, bool WithFilterData, int CategoryId, [FromQuery] ParameterPagination parameters, bool? isRefresh)
        {
            var response = _unitOfWorkService.CivilLibraryService.GetCivilWithoutLegLibrariesEnabledAtt(ComineOutPut, WithFilterData, CategoryId, parameters, isRefresh);
            return Ok(response);
        }
        [HttpGet("getById/{id}")]
        [ProducesResponseType(200, Type = typeof(CivilWithoutLegLibraryViewModel))]
        public IActionResult GetCivilWithoutLegLibrary(int id)
        {
            var response = _unitOfWorkService.CivilLibraryService.GetById(id, Helpers.Constants.CivilType.TLIcivilWithoutLegLibrary.ToString());
            return Ok(response);

        }
        [HttpPost("AddCivilWithoutLegLibrary")]
        [ProducesResponseType(200, Type = typeof(AddCivilWithoutLegLibraryViewModel))]
        public IActionResult AddCivilWithoutLegLibrary([FromBody]AddCivilWithoutLegLibraryViewModel addCivilWithoutLegLibraryViewModel)
        {
            if(TryValidateModel(addCivilWithoutLegLibraryViewModel, nameof(AddCivilWithoutLegLibraryViewModel)))
            {
                var ConnectionString = _configuration["ConnectionStrings:ActiveConnection"];
                var response = _unitOfWorkService.CivilLibraryService.AddCivilLibrary(Helpers.Constants.CivilType.TLIcivilWithoutLegLibrary.ToString(), addCivilWithoutLegLibraryViewModel, ConnectionString);
                return Ok(response);
            }
            else
            {
                var ErrorMessages = from state in ModelState.Values
                                    from error in state.Errors
                                    select error.ErrorMessage;
                return Ok(new Response<AddCivilWithoutLegLibraryViewModel>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
            }
        }
        [HttpPost("EditCivilWithoutLegLibrary")]
        [ProducesResponseType(200, Type = typeof(EditCivilWithoutLegLibraryViewModel))]
        public async Task<IActionResult> EditCivilWithoutLegLibrary([FromBody]EditCivilWithoutLegLibraryViewModel editCivilWithoutLegLibraryViewModel)
        {
            if(TryValidateModel(editCivilWithoutLegLibraryViewModel, nameof(EditCivilWithoutLegLibraryViewModel)))
            {
                var response = await _unitOfWorkService.CivilLibraryService.EditCivilLibrary(editCivilWithoutLegLibraryViewModel, Helpers.Constants.CivilType.TLIcivilWithoutLegLibrary.ToString());
                return Ok(response);
            }
            else
            {
                var ErrorMessages = from state in ModelState.Values
                                    from error in state.Errors
                                    select error.ErrorMessage;
                return Ok(new Response<EditCivilWithoutLegLibraryViewModel>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
            }
        }
        [HttpPost("DisableCivilWithoutLegLibrary/{Id}")]
        [ProducesResponseType(200, Type = typeof(CivilWithoutLegLibraryViewModel))]
        public async Task<IActionResult> DisableCivilWithoutLegLibrary(int Id)
        {
            var response = await _unitOfWorkService.CivilLibraryService.Disable(Id, Helpers.Constants.CivilType.TLIcivilWithoutLegLibrary.ToString());
            return Ok(response);
        }
        [HttpPost("DeleteCivilWithoutLegLibrary/{Id}")]
        [ProducesResponseType(200, Type = typeof(string))]
        public async Task<IActionResult> DeleteCivilWithoutLegLibrary(int Id)
        {
            var response = await _unitOfWorkService.CivilLibraryService.Delete(Id, Helpers.Constants.CivilType.TLIcivilWithoutLegLibrary.ToString());
            return Ok(response);
        }
    }
}
