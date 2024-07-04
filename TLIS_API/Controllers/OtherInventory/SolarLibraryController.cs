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
using TLIS_DAL.ViewModels.SolarLibraryDTOs;
using TLIS_Service.Helpers;
using TLIS_Service.ServiceBase;

namespace TLIS_API.Controllers.OtherInventory
{
    [ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
    [ServiceFilter(typeof(LogFilterAttribute))]
    [Route("api/[controller]")]
    [ApiController]
    public class SolarLibraryController : ControllerBase
    {
        private readonly IUnitOfWorkService _unitOfWorkService;
        private readonly IConfiguration _configuration;
        public SolarLibraryController(IUnitOfWorkService unitOfWorkService, IConfiguration configuration)
        {
            _unitOfWorkService = unitOfWorkService;
            _configuration = configuration;
        }
        [HttpPost("GetSolarLibraries")]
        [ProducesResponseType(200, Type = typeof(List<SolarLibraryViewModel>))]
        public IActionResult GetSolarLibraries([FromBody]List<FilterObjectList> filters, [FromQuery]bool WithFilterData, [FromQuery]ParameterPagination parameters)
        {
            var response = _unitOfWorkService.OtherInventoryLibraryService.GetSolarLibraries(filters, WithFilterData, parameters);
            return Ok(response);
        }
        [HttpPost("GetSolarLibraryEnabledAtt")]
        [ProducesResponseType(200, Type = typeof(Response<ReturnWithFilters<object>>))]
        public IActionResult GetSolarLibraryEnabledAtt([FromBody] CombineFilters CombineFilters, bool WithFilterData, [FromQuery]ParameterPagination parameters)
        {
            var response = _unitOfWorkService.OtherInventoryLibraryService.GetSolarLibraryEnabledAtt(CombineFilters, WithFilterData, parameters);
            return Ok(response);
        }
        [HttpGet("GetSolarLibraryById/{id}")]
        [ProducesResponseType(200, Type = typeof(AllItemAttributes))]
        public IActionResult GetSolarLibraryById(int id)
        {
            var response = _unitOfWorkService.OtherInventoryLibraryService.GetById(id, Helpers.Constants.OtherInventoryType.TLIsolarLibrary.ToString());
            return Ok(response);
        }
        [HttpPost("AddSolarLibrary")]
        [ProducesResponseType(200, Type = typeof(Nullable))]
        public IActionResult AddSolarLibrary([FromBody]AddSolarLibraryViewModel addSolarLibrary)
        {
            if (TryValidateModel(addSolarLibrary, nameof(AddSolarLibraryViewModel)))
            {
                var ConnectionString = _configuration["ConnectionStrings:ActiveConnection"];
                var response = _unitOfWorkService.OtherInventoryLibraryService.AddOtherInventoryLibrary(Helpers.Constants.OtherInventoryType.TLIsolarLibrary.ToString(), addSolarLibrary, ConnectionString);
                return Ok(response);
            }
            else
            {
                var ErrorMessages = from state in ModelState.Values
                                    from error in state.Errors
                                    select error.ErrorMessage;
                return Ok(new Response<AddSolarLibraryViewModel>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
            }
        }
        //[HttpPost("UpdateSolarLibrary")]
        //[ProducesResponseType(200, Type = typeof(Nullable))]
        //public async Task<IActionResult> UpdateSolarLibrary([FromBody]EditSolarLibraryViewModel editSolarLibrary)
        //{
        //    if (TryValidateModel(editSolarLibrary, nameof(EditSolarLibraryViewModel)))
        //    {
        //        var response = await _unitOfWorkService.OtherInventoryLibraryService.EditOtherInventoryLibrary(editSolarLibrary, Helpers.Constants.OtherInventoryType.TLIsolarLibrary.ToString());
        //        return Ok(response);
        //    }
        //    else
        //    {
        //        var ErrorMessages = from state in ModelState.Values
        //                            from error in state.Errors
        //                            select error.ErrorMessage;
        //        return Ok(new Response<EditSolarLibraryViewModel>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
        //    }
        //}
        //[HttpPost("DisableSolarLibrary/{Id}")]
        //[ProducesResponseType(200, Type = typeof(Nullable))]
        //public async Task<IActionResult> DisableSolarLibrary(int Id)
        //{
        //    var response = await _unitOfWorkService.OtherInventoryLibraryService.Disable(Id, Helpers.Constants.OtherInventoryType.TLIsolarLibrary.ToString());
        //    return Ok(response);
        //}
        //[HttpPost("DeleteSolarLibrary/{Id}")]
        //[ProducesResponseType(200, Type = typeof(Nullable))]
        //public async Task<IActionResult> DeleteSolarLibrary(int Id)
        //{
        //    var response = await _unitOfWorkService.OtherInventoryLibraryService.Delete(Id, Helpers.Constants.OtherInventoryType.TLIsolarLibrary.ToString());
        //    return Ok(response);
        //}
    }
}