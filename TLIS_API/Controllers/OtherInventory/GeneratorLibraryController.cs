using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using TLIS_API.Middleware.WorkFlow;
using TLIS_DAL.Helper;
using TLIS_DAL.Helper.Filters;
using TLIS_DAL.Helpers;
using TLIS_DAL.ViewModelBase;
using TLIS_DAL.ViewModels.GeneratorLibraryDTOs;
using TLIS_Service.Helpers;
using TLIS_Service.ServiceBase;

namespace TLIS_API.Controllers.OtherInventory
{
    [ServiceFilter(typeof(WorkFlowMiddleware))]
    [ServiceFilter(typeof(LogFilterAttribute))]
    [Route("api/[controller]")]
    [ApiController]
    public class GeneratorLibraryController : ControllerBase
    {
        private readonly IUnitOfWorkService _unitOfWorkService;
        private readonly IConfiguration _configuration;
        public GeneratorLibraryController(IUnitOfWorkService unitOfWorkService, IConfiguration configuration)
        {
            _unitOfWorkService = unitOfWorkService;
            _configuration = configuration;
        }
        [HttpPost("GetGeneratorLibraries")]
        [ProducesResponseType(200, Type = typeof(List<GeneratorLibraryViewModel>))]
        public IActionResult GetGeneratorLibraries([FromBody]List<FilterObjectList> filters, bool WithFilterData, [FromQuery]ParameterPagination parameters)
        {
            var response = _unitOfWorkService.OtherInventoryLibraryService.GetGeneratorLibraries(filters, WithFilterData, parameters);
            return Ok(response);
        }
        [HttpPost("GetGeneratorLibraryEnabledAtt")]
        [ProducesResponseType(200, Type = typeof(Response<ReturnWithFilters<object>>))]
        public IActionResult GetGeneratorLibraryEnabledAtt([FromBody] CombineFilters CombineFilters, [FromQuery]bool WithFilterData, [FromQuery]ParameterPagination parameters)
        {
            var response = _unitOfWorkService.OtherInventoryLibraryService.GetGeneratorLibraryEnabledAtt(CombineFilters, WithFilterData, parameters);
            return Ok(response);
        }
        [HttpGet("GetGeneratorLibraryById/{id}")]
        [ProducesResponseType(200, Type = typeof(AllItemAttributes))]
        public IActionResult GetGeneratorLibraryById(int id)
        {
            var response = _unitOfWorkService.OtherInventoryLibraryService.GetById(id, Helpers.Constants.OtherInventoryType.TLIgeneratorLibrary.ToString());
            return Ok(response);
        }
        [HttpPost("AddGeneratorLibrary")]
        [ProducesResponseType(200, Type = typeof(Nullable))]
        public IActionResult AddGeneratorLibrary([FromBody]AddGeneratorLibraryViewModel addGeneratorLibrary)
        {
            if (TryValidateModel(addGeneratorLibrary, nameof(AddGeneratorLibraryViewModel)))
            {
                var ConnectionString = _configuration["ConnectionStrings:ActiveConnection"];
                var response = _unitOfWorkService.OtherInventoryLibraryService.AddOtherInventoryLibrary(Helpers.Constants.OtherInventoryType.TLIgeneratorLibrary.ToString(), addGeneratorLibrary, ConnectionString);
                return Ok(response);
            }
            else
            {
                var ErrorMessages = from state in ModelState.Values
                                    from error in state.Errors
                                    select error.ErrorMessage;
                return Ok(new Response<AddGeneratorLibraryViewModel>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
            }
        }
        [HttpPost("UpdateGeneratorLibrary")]
        [ProducesResponseType(200, Type = typeof(Nullable))]
        public async Task<IActionResult> UpdateGeneratorLibrary([FromBody]EditGeneratorLibraryViewModel editGeneratorLibrary)
        {
            if (TryValidateModel(editGeneratorLibrary, nameof(EditGeneratorLibraryViewModel)))
            {
                var response = await _unitOfWorkService.OtherInventoryLibraryService.EditOtherInventoryLibrary(editGeneratorLibrary, Helpers.Constants.OtherInventoryType.TLIgeneratorLibrary.ToString());
                return Ok(response);
            }
            else
            {
                var ErrorMessages = from state in ModelState.Values
                                    from error in state.Errors
                                    select error.ErrorMessage;
                return Ok(new Response<EditGeneratorLibraryViewModel>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
            }
        }
        [HttpPost("DisableGeneratorLibrary/{Id}")]
        [ProducesResponseType(200, Type = typeof(Nullable))]
        public async Task<IActionResult> DisableGeneratorLibrary(int Id)
        {
            var response = await _unitOfWorkService.OtherInventoryLibraryService.Disable(Id, Helpers.Constants.OtherInventoryType.TLIgeneratorLibrary.ToString());
            return Ok(response);
        }
        [HttpPost("DeleteGeneratorLibrary/{Id}")]
        [ProducesResponseType(200, Type = typeof(Nullable))]
        public async Task<IActionResult> DeleteGeneratorLibrary(int Id)
        {
            var response = await _unitOfWorkService.OtherInventoryLibraryService.Delete(Id, Helpers.Constants.OtherInventoryType.TLIgeneratorLibrary.ToString());
            return Ok(response);
        }
    }
}