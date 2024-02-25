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
using TLIS_DAL.ViewModels.DynamicAttDTOs;
using TLIS_DAL.ViewModels.DynamicAttInstValueDTOs;
using TLIS_DAL.ViewModels.MW_DishLbraryDTOs;
using TLIS_Service.Helpers;
using TLIS_Service.ServiceBase;

namespace TLIS_API.Controllers.LoadLibrary
{
    [ServiceFilter(typeof(WorkFlowMiddleware))]
    [ServiceFilter(typeof(LogFilterAttribute))]
    [Route("api/[controller]")]
    public class MW_DishLibraryController : ControllerBase
    {
        private readonly IUnitOfWorkService _unitOfWorkService;
        private readonly IConfiguration _configuration;
        public MW_DishLibraryController(IUnitOfWorkService unitOfWorkService, IConfiguration configuration)
        {
            _unitOfWorkService = unitOfWorkService;
            _configuration = configuration;
        }
        [HttpPost("getAll")]
        [ProducesResponseType(200, Type = typeof(List<MW_DishLibraryViewModel>))]
        public IActionResult GetMW_DishLibrary([FromBody]List<FilterObjectList> filters, bool WithFilterData, [FromQuery]ParameterPagination parameters)
        {
            var response = _unitOfWorkService.MWLibraryService.get_MW_Dish_LibrariesAsync(filters, WithFilterData, parameters);
            return Ok(response);
        }
        [HttpPost("GetMW_DishLibraries")]
        [ProducesResponseType(200, Type = typeof(Response<ReturnWithFilters<object>>))]
        public IActionResult GetMW_DishLibraries([FromBody] CombineFilters CombineFilters, bool WithFilterData, [FromQuery]ParameterPagination parameters)
        {
            var response = _unitOfWorkService.MWLibraryService.GetMW_DishLibraries(CombineFilters, WithFilterData, parameters);
            return Ok(response);
        }
        [HttpGet("getById/{id}")]
        [ProducesResponseType(200, Type = typeof(AllItemAttributes))]
        public IActionResult GetDishLibrary(int id)
        {
            var response = _unitOfWorkService.MWLibraryService.GetById(id, Helpers.Constants.LoadSubType.TLImwDishLibrary.ToString());
            return Ok(response);
        }

        [HttpPost("AddMW_DishLibrary")]
        [ProducesResponseType(200, Type = typeof(AddMW_DishLibraryViewModel))]
        public IActionResult AddMW_DishLibrary([FromBody]AddMW_DishLibraryViewModel addMW_BULibraryViewModel)
        {
            if(TryValidateModel(addMW_BULibraryViewModel, nameof(AddMW_DishLibraryViewModel)))
            {
                var ConnectionString = _configuration["ConnectionStrings:ActiveConnection"];
                var response = _unitOfWorkService.MWLibraryService.AddMWLibrary(Helpers.Constants.LoadSubType.TLImwDishLibrary.ToString(), addMW_BULibraryViewModel, ConnectionString);
                return Ok(response);
            }
            else
            {
                var ErrorMessages = from state in ModelState.Values
                                    from error in state.Errors
                                    select error.ErrorMessage;
                return Ok(new Response<AddMW_DishLibraryViewModel>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
            }
        }

        [HttpPost("EditMW_DishLibrary")]
        [ProducesResponseType(200, Type = typeof(EditMW_DishLibraryViewModel))]
        public async Task<IActionResult> EditMW_DishLibrary([FromBody]EditMW_DishLibraryViewModel editMW_DishLibraryViewModel)
        {
            if(TryValidateModel(editMW_DishLibraryViewModel, nameof(EditMW_DishLibraryViewModel)))
            {
                var response = await _unitOfWorkService.MWLibraryService.EditMWLibrary(Helpers.Constants.LoadSubType.TLImwDishLibrary.ToString(), editMW_DishLibraryViewModel);
                return Ok(response);
            }
            else
            {
                var ErrorMessages = from state in ModelState.Values
                                    from error in state.Errors
                                    select error.ErrorMessage;
                return Ok(new Response<EditMW_DishLibraryViewModel>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
            }
        }

        [HttpPost("DisableMW_DishLibrary/{Id}")]
        [ProducesResponseType(200, Type = typeof(EditMW_DishLibraryViewModel))]
        public async Task<IActionResult> DisableMW_DishLibrary(int Id)
        {
            var response = await _unitOfWorkService.MWLibraryService.Disable(Id, Helpers.Constants.LoadSubType.TLImwDishLibrary.ToString());
            return Ok(response);
        }

        [HttpGet("GetForAdd")]
        [ProducesResponseType(200, Type = typeof(AllItemAttributes))]
        public IActionResult GetForAdd()
        {
            var response = _unitOfWorkService.MWLibraryService.GetForAdd(Helpers.Constants.LoadSubType.TLImwDishLibrary.ToString());
            return Ok(response);
        }

        [HttpPost("DeleteMW_DishLibrary/{Id}")]
        [ProducesResponseType(200, Type = typeof(EditMW_DishLibraryViewModel))]
        public async Task<IActionResult> DeleteMW_DishLibrary(int Id)
        {
            var response = await _unitOfWorkService.MWLibraryService.Delete(Id, Helpers.Constants.LoadSubType.TLImwDishLibrary.ToString());
            return Ok(response);
        }
    }
}