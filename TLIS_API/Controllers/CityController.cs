using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TLIS_API.Middleware.WorkFlow;
using TLIS_DAL.Helper;
using TLIS_DAL.Helper.Filters;
using TLIS_DAL.Helpers;
using TLIS_DAL.ViewModels.CityDTOs;
using TLIS_Service.Helpers;
using TLIS_Service.ServiceBase;

namespace TLIS_API.Controllers
{
    [ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
    [ServiceFilter(typeof(LogFilterAttribute))]
    [Route("api/[controller]")]
    [ApiController]
    public class CityController : Controller
    {
        private IUnitOfWorkService _unitOfWorkService;
        private readonly IMapper _mapper;
        public CityController(IUnitOfWorkService unitOfWorkService,IMapper mapper)
        {
            _unitOfWorkService = unitOfWorkService;
            _mapper = mapper;
        }
        //[Authorize]
        //[HttpPost("getAll")]
        //[ProducesResponseType(200, Type = typeof(List<CityViewModel>))]
        //public async Task<IActionResult> GetCities([FromQueryAttribute]ParameterPagination parameterPagination,[FromBody] List<FilterObjectList> Filter)
        //{
        //    var result = await _unitOfWorkService.CityService.GetCitiess(parameterPagination, Filter);
        //    var resultVM = _mapper.Map<IEnumerable<CityViewModel>>(result);

        //    return Ok(new Response<IEnumerable<CityViewModel>>(resultVM));
        //}

        [HttpPost("getAll")]
        [ProducesResponseType(200, Type = typeof(List<CityViewModel>))]

        public IActionResult GetCities()
        {
            var result =  _unitOfWorkService.CityService.GetCitiess2();
            var resultVM = _mapper.Map<IEnumerable<CityViewModel>>(result);
            return Ok(resultVM);
        }

        [HttpGet("getAllRep")]
        [ProducesResponseType(200, Type = typeof(List<CityViewModel>))]
        public async Task<IActionResult> GetClasses([FromQueryAttribute]ParameterPagination parameterPagination, [FromQueryAttribute] CityFilter classFilter)
        {
            var result = await _unitOfWorkService.CityService.GetCitiesRep(parameterPagination);
            var resultVM = _mapper.Map<IEnumerable<CityViewModel>>(result);

            return Ok(new Response<IEnumerable<CityViewModel>>(resultVM));

        }

        [HttpPost("AddCity")]
        [ProducesResponseType(200, Type = typeof(List<CityViewModel>))]
        public async Task<IActionResult> AddCity([FromBody]AddCityViewModel addCityViewModel)
        {
            if(TryValidateModel(addCityViewModel, nameof(AddCityViewModel)))
            {
                //var userId = HttpContext.Session.GetString("UserId");
                await _unitOfWorkService.CityService.AddCity(addCityViewModel);
                return Ok();
            }
            else
            {
                var ErrorMessages = from state in ModelState.Values
                                    from error in state.Errors
                                    select error.ErrorMessage;
                return Ok(new Response<List<CityViewModel>>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
            }

        }

        [HttpPost("WhereCity")]
        [ProducesResponseType(200, Type = typeof(List<CityViewModel>))]
        public async Task<IActionResult> Where([FromBody]List<FilterObjectList> filter)
        {
            // _unitOfWorkService.CityService.whereCity();
            var result =  _unitOfWorkService.CityService.whereCity(filter);
            var resultVM = _mapper.Map<IEnumerable<CityViewModel>>(result);

            return Ok(new Response<IEnumerable<CityViewModel>>(resultVM));
           // return Ok();
        }
    }
}