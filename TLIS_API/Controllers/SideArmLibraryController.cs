using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using TLIS_API.Middleware.WorkFlow;
using TLIS_DAL.Helper;
using TLIS_DAL.Helper.Filters;
using TLIS_DAL.Helpers;
using TLIS_DAL.ViewModelBase;
using TLIS_DAL.ViewModels.SideArmLibraryDTOs;
using TLIS_Service.Helpers;
using TLIS_Service.ServiceBase;

namespace TLIS_API.Controllers
{
    [ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
    [ServiceFilter(typeof(LogFilterAttribute))]
    [Route("api/[controller]")]
    
    public class SideArmLibraryController : Controller
    {
        private readonly IUnitOfWorkService _unitOfWorkService;
        private readonly IConfiguration _configuration;
        public SideArmLibraryController(IUnitOfWorkService unitOfWorkService, IConfiguration configuration)
        {
            _unitOfWorkService = unitOfWorkService;
            _configuration = configuration;
        }
        [HttpPost("SideArmLibrarySeedDataForTest")]
        public IActionResult SideArmLibrarySeedDataForTest()
        {
            _unitOfWorkService.SideArmLibraryService.SeedDataForTest();
            return Ok();
        }
        [HttpPost("getAll")]
        [ProducesResponseType(200, Type = typeof(List<SideArmLibraryViewModel>))]
        public async Task<IActionResult> GetSideArmLibraries([FromBody]List<FilterObjectList> filters, [FromQuery]ParameterPagination parameters)
        {
            var response = await _unitOfWorkService.SideArmLibraryService.GetSideArmLibraries(filters, parameters);
            return Ok(response);
        }
        [HttpPost("GetSideArmLibrariesWithEnabledAttributes")]
        [ProducesResponseType(200, Type = typeof(Response<ReturnWithFilters<object>>))]
        public IActionResult GetSideArmLibrariesWithEnabledAttributes([FromBody] CombineFilters CombineFilters, [FromQuery]ParameterPagination parameters)
        {
            var response = _unitOfWorkService.SideArmLibraryService.GetSideArmLibrariesWithEnabledAttributes(CombineFilters, parameters);
            return Ok(response);
        }
        [HttpGet("GetSideArmLibraryById/{id}")]
        [ProducesResponseType(200, Type = typeof(SideArmLibraryViewModel))]
        public IActionResult GetSideArmLibraryById(int id)
        {
            var response = _unitOfWorkService.SideArmLibraryService.GetSideArmLibraryById(id);
            //var resultVM = Mapper.Map<SideArmLibraryViewModel>(result);
            return Ok(response);
        }

        [HttpPost("AddSideArmLibrary")]
        [ProducesResponseType(200, Type = typeof(SideArmLibraryViewModel))]
        public IActionResult AddSideArmLibrary([FromBody]AddSideArmLibraryViewModel addSideArmLibraryViewModel)
        {
            if(TryValidateModel(addSideArmLibraryViewModel, nameof(AddSideArmLibraryViewModel)))
            {
                var ConnectionString = _configuration["ConnectionStrings:ActiveConnection"];
                var response = _unitOfWorkService.SideArmLibraryService.AddSideArmLibrary(addSideArmLibraryViewModel, ConnectionString);
                return Ok(response);
            }
            else
            {
                var ErrorMessages = from state in ModelState.Values
                                    from error in state.Errors
                                    select error.ErrorMessage;
                return Ok(new Response<AddSideArmLibraryViewModel>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
            }
        }

        [HttpPost("EditSideArmLibrary")]
        [ProducesResponseType(200, Type = typeof(SideArmLibraryViewModel))]
        public async Task<IActionResult> EditSideArmLibrary([FromBody]EditSideArmLibraryViewModel editSideArmLibraryViewModel)
        {
            if(TryValidateModel(editSideArmLibraryViewModel, nameof(EditSideArmLibraryViewModel)))
            {
                var response = await _unitOfWorkService.SideArmLibraryService.EditSideArmLibrary(editSideArmLibraryViewModel);
                return Ok(response);
            }
            else
            {
                var ErrorMessages = from state in ModelState.Values
                                    from error in state.Errors
                                    select error.ErrorMessage;
                return Ok(new Response<EditSideArmLibraryViewModel>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
            }
        }

        [HttpPost("DisableSideArmLibrary")]
        [ProducesResponseType(200, Type = typeof(SideArmLibraryViewModel))]
        public async Task<IActionResult> DisableSideArmLibrary(int id)
        {
            var response = await _unitOfWorkService.SideArmLibraryService.Disable(id);
            return Ok(response);
        }
        
        [HttpPost("DeleteSideArmLibrary")]
        [ProducesResponseType(200, Type = typeof(SideArmLibraryViewModel))]
        public async Task<IActionResult> DeleteSideArmLibrary(int id)
        {
            var response = await _unitOfWorkService.SideArmLibraryService.Delete(id);
            return Ok(response);
        }
        [HttpGet("GetSideArmLibs")]
        [ProducesResponseType(200, Type = typeof(List<KeyValuePair<string, int>>))]
        public IActionResult GetSideArmLibs()
        {
            var response = _unitOfWorkService.SideArmLibraryService.GetSideArmLibs();
            return Ok(response);
        }
    }
}