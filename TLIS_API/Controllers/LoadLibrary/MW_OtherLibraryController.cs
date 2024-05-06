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
using TLIS_DAL.ViewModels.CivilWithLegLibraryDTOs;
using TLIS_DAL.ViewModels.MW_OtherLibraryDTOs;
using TLIS_Service.Helpers;
using TLIS_Service.ServiceBase;

namespace TLIS_API.Controllers.LoadLibrary
{
    [ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
    [ServiceFilter(typeof(LogFilterAttribute))]
    [Route("api/[controller]")]
    [ApiController]
    public class MW_OtherLibraryController : ControllerBase
    {
        private readonly IUnitOfWorkService _unitOfWorkService;
        private readonly IConfiguration _configuration;
        public MW_OtherLibraryController(IUnitOfWorkService unitOfWorkService, IConfiguration configuration)
        {
            _unitOfWorkService = unitOfWorkService;
            _configuration = configuration;
        }

        [HttpPost("getAll")]
        [ProducesResponseType(200, Type = typeof(List<MW_OtherLibraryViewModel>))]
        public IActionResult GetMW_ODULibrary([FromBody]List<FilterObjectList> filters, bool WithFilterData, [FromQuery]ParameterPagination parameters)
        {
            var response = _unitOfWorkService.MWLibraryService.get_MW_Other_LibrariesAsync(filters, parameters);
            return Ok(response);
        }
        [HttpGet("getById/{id}")]
        [ProducesResponseType(200, Type = typeof(AllItemAttributes))]
        public IActionResult GetODULibrary(int id)
        {
            var response = _unitOfWorkService.MWLibraryService.GetById(id, Helpers.Constants.LoadSubType.TLImwOtherLibrary.ToString());
            return Ok(response);
        }
        //[HttpPost("AddMW_OtherLibrary")]
        //[ProducesResponseType(200, Type = typeof(AddMWOtherLibraryObject))]
        //public IActionResult AddMW_OtherLibrary([FromBody] AddMWOtherLibraryObject addMW_OtherLibraryViewModel)
        //{
        //    if (TryValidateModel(addMW_OtherLibraryViewModel, nameof(AddMWOtherLibraryObject)))
        //    {
        //        var ConnectionString = _configuration["ConnectionStrings:ActiveConnection"];
        //        var response = _unitOfWorkService.MWLibraryService.AddMWLibrary(Helpers.Constants.LoadSubType.TLImwOtherLibrary.ToString(), addMW_OtherLibraryViewModel, ConnectionString);
        //        return Ok(response);
        //    }
        //    else
        //    {
        //        var ErrorMessages = from state in ModelState.Values
        //                            from error in state.Errors
        //                            select error.ErrorMessage;
        //        return Ok(new Response<AddMW_OtherLibraryViewModel>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
        //    }
       //}
        //[HttpPost("EditMW_OtherLibrary")]
        //[ProducesResponseType(200, Type = typeof(EditMW_OtherLibraryViewModel))]

        //public async Task<IActionResult> EditMW_OtherLibrary([FromBody]EditMW_OtherLibraryViewModel editMW_OtherLibraryViewModel)
        //{
        //    if (TryValidateModel(editMW_OtherLibraryViewModel, nameof(EditMW_OtherLibraryViewModel)))
        //    {
        //        var response = await _unitOfWorkService.MWLibraryService.EditMWLibrary(Helpers.Constants.LoadSubType.TLImwOtherLibrary.ToString(), editMW_OtherLibraryViewModel);
        //        return Ok(response);
        //    }
        //    else
        //    {
        //        var ErrorMessages = from state in ModelState.Values
        //                            from error in state.Errors
        //                            select error.ErrorMessage;
        //        return Ok(new Response<EditMW_OtherLibraryViewModel>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
        //    }
        //}

        [HttpPost("DisableMW_OtherLibrary/{Id}")]
        [ProducesResponseType(200, Type = typeof(MW_OtherLibraryViewModel))]
        public async Task<IActionResult> DisableMW_OtherLibrary(int Id)
        {
            var response = await _unitOfWorkService.MWLibraryService.Disable(Id, Helpers.Constants.LoadSubType.TLImwOtherLibrary.ToString());
            return Ok(response);
        }

        [HttpGet("GetForAddMWOtherLibrary")]
        [ProducesResponseType(200, Type = typeof(Response<GetForAddCivilLibrarybject>))]
        public IActionResult GetForAddMWOtherLibrary()
        {
            var response = _unitOfWorkService.MWLibraryService.GetForAdd(Helpers.Constants.LoadSubType.TLImwOtherLibrary.ToString());
            return Ok(response);
        }
        [HttpPost("DeleteMW_OtherLibrary/{Id}")]
        [ProducesResponseType(200, Type = typeof(MW_OtherLibraryViewModel))]
        public async Task<IActionResult> DeleteMW_OtherLibrary(int Id)
        {
            var response = await _unitOfWorkService.MWLibraryService.Delete(Id, Helpers.Constants.LoadSubType.TLImwOtherLibrary.ToString());
            return Ok(response);
        }
        [HttpPost("GetMW_OtherLibraries")]
        [ProducesResponseType(200, Type = typeof(Response<ReturnWithFilters<object>>))]
        public IActionResult GetMW_OtherLibraries([FromBody] CombineFilters CombineFilters, [FromQuery] ParameterPagination parameters)
        {
            var response = _unitOfWorkService.MWLibraryService.GetMW_OtherLibraries(CombineFilters, parameters);
            return Ok(response);
        }
    }
}