using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using TLIS_API.Helpers;
using TLIS_API.Middleware.WorkFlow;
using TLIS_DAL.Helper;
using TLIS_DAL.Helper.Filters;
using TLIS_DAL.Helpers;
using TLIS_DAL.ViewModelBase;
using TLIS_DAL.ViewModels.PowerDTOs;
using TLIS_Service.Helpers;
using TLIS_Service.ServiceBase;

namespace TLIS_API.Controllers.Load
{
    [ServiceFilter(typeof(WorkFlowMiddleware))]
    [ServiceFilter(typeof(LogFilterAttribute))]
    [Route("api/[controller]")]
    public class PowerLibraryController : Controller
    {
        private readonly IUnitOfWorkService _unitOfWorkService;
        private readonly IConfiguration _configuration;
        public PowerLibraryController(IUnitOfWorkService unitOfWorkService, IConfiguration configuration)
        {
            _unitOfWorkService = unitOfWorkService;
            _configuration = configuration;
        }

        [HttpPost("getAll")]
        [ProducesResponseType(200, Type = typeof(List<PowerLibraryViewModel>))]
        public IActionResult GetPowerLibrary([FromQuery]ParameterPagination parameterPagination,[FromBody]List<FilterObjectList> filters)
        {
            var response = _unitOfWorkService.PowerLibraryService.GetPowerLibraries(parameterPagination, filters);
            return Ok(response);
        }
        [HttpPost("GetPowerLibrariesWithEnableAttributes")]
        [ProducesResponseType(200, Type = typeof(Response<ReturnWithFilters<object>>))]
        public IActionResult GetPowerLibrariesWithEnableAttributes([FromBody] CombineFilters CombineFilters, [FromQuery]ParameterPagination parameterPagination, bool? isRefresh)
        {
            var response = _unitOfWorkService.PowerLibraryService.GetPowerLibrariesWithEnableAttributes(CombineFilters, parameterPagination, isRefresh);
            return Ok(response);
        }
        [HttpPost("GetForAdd")]
        [ProducesResponseType(200, Type = typeof(AllItemAttributes))]
        public IActionResult GetForAdd()
        {
            var response = _unitOfWorkService.PowerLibraryService.GetForAdd(Helpers.Constants.LoadSubType.TLIpowerLibrary.ToString());
            return Ok(response);
        }
        [HttpGet("getById/{id}")]
        [ProducesResponseType(200, Type = typeof(PowerLibraryViewModel))]
        public IActionResult GetPowerLibrary(int id)
        {
            var response = _unitOfWorkService.PowerLibraryService.GetById(id,Helpers.Constants.LoadSubType.TLIpowerLibrary.ToString());
            return Ok(response);

        }

        [HttpPost("AddPowerLibrary")]
        [ProducesResponseType(200, Type = typeof(Nullable))]
        public IActionResult AddPowerLibrary([FromBody]AddPowerLibraryViewModel addPowerLibraryViewModel)
        {
            if(TryValidateModel(addPowerLibraryViewModel, nameof(AddPowerLibraryViewModel)))
            {
                var ConnectionString = _configuration["ConnectionStrings:ActiveConnection"];
                var response = _unitOfWorkService.PowerLibraryService.AddPowerLibrary(addPowerLibraryViewModel, ConnectionString);
                return Ok(response);
            }
            else
            {
                var ErrorMessages = from state in ModelState.Values
                                    from error in state.Errors
                                    select error.ErrorMessage;
                return Ok(new Response<AddPowerLibraryViewModel>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
            }
        }

        [HttpPost("EditPowerLibrary")]
        [ProducesResponseType(200, Type = typeof(Nullable))]

        public async Task<IActionResult> EditPowerLibrary([FromBody]EditPowerLibraryViewModel editPowerLibraryViewModel)
        {
            if(TryValidateModel(editPowerLibraryViewModel, nameof(EditPowerLibraryViewModel)))
            {
                var response = await _unitOfWorkService.PowerLibraryService.EditPowerLibrary(Helpers.Constants.LoadSubType.TLIpowerLibrary.ToString(), editPowerLibraryViewModel);
                return Ok(response);
            }
            else
            {
                var ErrorMessages = from state in ModelState.Values
                                    from error in state.Errors
                                    select error.ErrorMessage;
                return Ok(new Response<EditPowerLibraryViewModel>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
            }
        }
        [HttpPost("DisablePowerLibrary")]
        [ProducesResponseType(200, Type = typeof(Nullable))]
        public async Task<IActionResult> DisablePowerLibrary(int Id)
        {
            var response = await _unitOfWorkService.PowerLibraryService.DisablePowerLibrary(Helpers.Constants.LoadSubType.TLIpowerLibrary.ToString(), Id);
            return Ok(response);
        }
        [HttpPost("DeletePowerLibrary")]
        [ProducesResponseType(200, Type = typeof(Nullable))]
        public async Task<IActionResult> DeletePowerLibrary(int Id)
        {
            var response = await _unitOfWorkService.PowerLibraryService.DeletePowerLibrary(Helpers.Constants.LoadSubType.TLIpowerLibrary.ToString(), Id);
            return Ok(response);
        }
    }
}