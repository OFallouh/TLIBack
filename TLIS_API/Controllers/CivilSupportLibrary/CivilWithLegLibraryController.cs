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
using TLIS_DAL.ViewModels.CivilWithLegDTOs;
using TLIS_DAL.ViewModels.CivilWithLegLibraryDTOs;
using TLIS_DAL.ViewModels.CivilWithLegsDTOs;
using TLIS_DAL.ViewModels.CivilWithoutLegLibraryDTOs;
using TLIS_DAL.ViewModels.DynamicAttDTOs;
using TLIS_DAL.ViewModels.DynamicAttInstValueDTOs;
using TLIS_Service.Helpers;
using TLIS_Service.ServiceBase;
using static TLIS_Service.Services.CivilLibraryService;

namespace TLIS_API.Controllers
{
   // [ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
    [ServiceFilter(typeof(LogFilterAttribute))]
    [Route("api/[controller]")]
    public class CivilWithLegLibraryController : Controller
    {
        private readonly IUnitOfWorkService _unitOfWorkService;
        private readonly IConfiguration _configuration;
        public CivilWithLegLibraryController(IUnitOfWorkService unitOfWorkService, IConfiguration configuration)
        {
            _unitOfWorkService = unitOfWorkService;
            _configuration = configuration;
        }
        [HttpPost("getAll")]
        [ProducesResponseType(200, Type = typeof(ReturnWithFilters<CivilWithLegLibraryViewModel>))]
        public IActionResult getCivilWithLegLibraries([FromBody]List<FilterObjectList> filters, bool WithFilterData, [FromQuery] ParameterPagination parameters)
        {
            var response = _unitOfWorkService.CivilLibraryService.getCivilWithLegLibraries(filters, WithFilterData, parameters);
            return Ok(response);
        }

        
        [HttpPost("GetCivilWithLegLibrariesEnabledAtt")]
        [ProducesResponseType(200, Type = typeof(Response<ReturnWithFilters<object>>))]
        public IActionResult GetCivilWithLegLibrariesEnabledAtt([FromBody] CombineFilters CombineOutPut, bool WithFilterData, [FromQuery] ParameterPagination parameters)
        {
            var response = _unitOfWorkService.CivilLibraryService.GetCivilWithLegLibrariesEnabledAtt(CombineOutPut, WithFilterData, parameters);
            return Ok(response);
        }
        [HttpGet("getById/{id}")]
        [ProducesResponseType(200, Type = typeof(AllItemAttributes))]
        public  IActionResult GetCivilWithLegLibrary(int id)
        {
            var response = _unitOfWorkService.CivilLibraryService.GetById(id, Helpers.Constants.CivilType.TLIcivilWithLegLibrary.ToString());
            return Ok(response);

        }
        [HttpPost("AddCivilWithLegLibrary")]
        [ProducesResponseType(200, Type = typeof(AddCivilWithLegsLibraryObject))]
        public IActionResult AddCivilWithLegLibrary([FromBody] AddCivilWithLegsLibraryObject CivilWithLegLibraryViewModel)
        {
            var ConnectionString = _configuration["ConnectionStrings:ActiveConnection"];
            if (TryValidateModel(CivilWithLegLibraryViewModel, nameof(AddCivilWithLegsLibraryObject)))
            {
                var response = _unitOfWorkService.CivilLibraryService.AddCivilLibrary(Helpers.Constants.CivilType.TLIcivilWithLegLibrary.ToString(), CivilWithLegLibraryViewModel, ConnectionString);
                return Ok(response);
            }
            else
            {
                var ErrorMessages = from state in ModelState.Values
                            from error in state.Errors
                            select error.ErrorMessage;
                return Ok(new Response<AddCivilWithoutLegsLibraryObject>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
            }
        }
        //[HttpPost("EditCivilWithLegLibrary")]
        //[ProducesResponseType(200, Type = typeof(EditCivilWithLegLibraryViewModels))]

        //public async Task<IActionResult> EditCivilWithLegLibrary([FromBody]EditCivilWithLegLibraryViewModels editCivilWithLegLibraryViewModel)
        //{
        //    if(TryValidateModel(editCivilWithLegLibraryViewModel, nameof(EditCivilWithLegLibraryViewModels)))
        //    {
        //        var response = await _unitOfWorkService.CivilLibraryService.EditCivilLibrary(editCivilWithLegLibraryViewModel, Helpers.Constants.CivilType.TLIcivilWithLegLibrary.ToString());
        //        return Ok(response);
        //    }
        //    else
        //    {
        //        var ErrorMessages = from state in ModelState.Values
        //                            from error in state.Errors
        //                            select error.ErrorMessage;
        //        return Ok(new Response<EditCivilWithLegLibraryViewModels>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
        //    }
        //}
        [HttpPost("DisableCivilWithLegLibrary/{Id}")]
        [ProducesResponseType(200, Type = typeof(string))]
        public async Task<IActionResult> DisableCivilWithLegLibrary(int Id)
        {
            var response = await _unitOfWorkService.CivilLibraryService.Disable(Id, Helpers.Constants.CivilType.TLIcivilWithLegLibrary.ToString());
            return Ok(response);
        }
        [HttpPost("DeleteCivilWithLegLibrary/{Id}")]
        [ProducesResponseType(200, Type = typeof(string))]
        public async Task<IActionResult> DeleteCivilWithLegLibrary(int Id)
        {
            var response = await _unitOfWorkService.CivilLibraryService.Delete(Id, Helpers.Constants.CivilType.TLIcivilWithLegLibrary.ToString());
            return Ok(response);
        }
        [HttpGet("GetForAddCivilWithLegsLibrary")]
        public IActionResult GetForAddCivilWithLegsLibrary()
        {
            var response = _unitOfWorkService.CivilLibraryService.GetForAdd(Helpers.Constants.CivilType.TLIcivilWithLegLibrary.ToString());
            return Ok(response);
        }
    }
}
