using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using TLIS_API.Helpers;
using TLIS_API.Middleware.WorkFlow;
using TLIS_DAL.Helper;
using TLIS_DAL.Helper.Filters;
using TLIS_DAL.Helpers;
using TLIS_DAL.ViewModelBase;
using TLIS_DAL.ViewModels.DynamicAttDTOs;
using TLIS_DAL.ViewModels.DynamicAttInstValueDTOs;
using TLIS_DAL.ViewModels.MW_ODUDTOs;
using TLIS_Service.Helpers;
using TLIS_Service.ServiceBase;

namespace TLIS_API.Controllers.LoadLibrary
{
    [ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
    [ServiceFilter(typeof(LogFilterAttribute))]
    [Route("api/[controller]")]
   
    public class MW_ODULibraryController : Controller
    {
        private readonly IUnitOfWorkService _unitOfWorkService;
        private readonly IConfiguration _configuration;
        public MW_ODULibraryController(IUnitOfWorkService unitOfWorkService, IConfiguration configuration)
        {
            _unitOfWorkService = unitOfWorkService;
            _configuration = configuration;
        }

        [HttpPost("getAll")]
        [ProducesResponseType(200, Type = typeof(List<MW_ODULibraryViewModel>))]
        public IActionResult GetMW_ODULibrary([FromBody]List<FilterObjectList> filters, bool WithFilterData, ParameterPagination parameters)
        {
            var response = _unitOfWorkService.MWLibraryService.get_MW_ODU_LibrariesAsync(filters, WithFilterData, parameters);
            return Ok(response);
        }
        [HttpPost("GetMW_ODULibraries")]
        [ProducesResponseType(200, Type = typeof(Response<ReturnWithFilters<object>>))]
        public IActionResult GetMW_ODULibraries([FromBody] CombineFilters CombineFilters, bool WithFilterData, [FromQuery]ParameterPagination parameters)
        {
            var response = _unitOfWorkService.MWLibraryService.GetMW_ODULibraries(CombineFilters, WithFilterData, parameters);
            return Ok(response);
        }
        [HttpGet("getById/{id}")]
        [ProducesResponseType(200, Type = typeof(AllItemAttributes))]
        public IActionResult GetODULibrary(int id)
        {
            var response = _unitOfWorkService.MWLibraryService.GetById(id, Helpers.Constants.LoadSubType.TLImwODULibrary.ToString());
            return Ok(response);
        }
        [HttpPost("AddMW_ODULibrary")]
        [ProducesResponseType(200, Type = typeof(AddMW_ODULibraryViewModel))]
        public IActionResult AddMW_ODULibrary([FromBody]AddMW_ODULibraryViewModel addMW_ODULibraryViewModel)
        {
            if(TryValidateModel(addMW_ODULibraryViewModel, nameof(AddMW_ODULibraryViewModel)))
            {
                var ConnectionString = _configuration["ConnectionStrings:ActiveConnection"];
                var response = _unitOfWorkService.MWLibraryService.AddMWLibrary(Helpers.Constants.LoadSubType.TLImwODULibrary.ToString(), addMW_ODULibraryViewModel, ConnectionString);
                return Ok(response);
            }
            else
            {
                var ErrorMessages = from state in ModelState.Values
                                    from error in state.Errors
                                    select error.ErrorMessage;
                return Ok(new Response<AddMW_ODULibraryViewModel>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
            }
        }
        [HttpPost("EditMW_ODULibrary")]
        [ProducesResponseType(200, Type = typeof(EditMW_ODULibraryViewModel))]
        public async Task<IActionResult> EditMW_ODULibrary([FromBody]EditMW_ODULibraryViewModel editMW_RFULibraryViewModel)
        {
            if(TryValidateModel(editMW_RFULibraryViewModel, nameof(EditMW_ODULibraryViewModel)))
            {
                var response = await _unitOfWorkService.MWLibraryService.EditMWLibrary(Helpers.Constants.LoadSubType.TLImwODULibrary.ToString(), editMW_RFULibraryViewModel);
                return Ok(response);
            }
            else
            {
                var ErrorMessages = from state in ModelState.Values
                                    from error in state.Errors
                                    select error.ErrorMessage;
                return Ok(new Response<EditMW_ODULibraryViewModel>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
            }
        }

        [HttpPost("DisableMW_ODULibrary/{Id}")]
        [ProducesResponseType(200, Type = typeof(MW_ODULibraryViewModel))]
        public async Task<IActionResult> DisableMW_ODULibrary(int Id)
        {
            var response = await _unitOfWorkService.MWLibraryService.Disable(Id, Helpers.Constants.LoadSubType.TLImwODULibrary.ToString());
            return Ok(response);
        }

        [HttpGet("GetForAdd")]
        [ProducesResponseType(200, Type = typeof(AllItemAttributes))]
        public IActionResult GetForAdd()
        {
            var response = _unitOfWorkService.MWLibraryService.GetForAdd(Helpers.Constants.LoadSubType.TLImwODULibrary.ToString());
            return Ok(response);
        }
        [HttpPost("DeleteMW_ODULibrary/{Id}")]
        [ProducesResponseType(200, Type = typeof(MW_ODULibraryViewModel))]
        public async Task<IActionResult> DeleteMW_ODULibrary(int Id)
        {
            var response = await _unitOfWorkService.MWLibraryService.Delete(Id, Helpers.Constants.LoadSubType.TLImwODULibrary.ToString());
            return Ok(response);
        }
    }
}