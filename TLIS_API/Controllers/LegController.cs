using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TLIS_API.Helpers;
using TLIS_DAL.Helper;
using TLIS_DAL.Helper.Filters;
using TLIS_DAL.Helpers;
using TLIS_DAL.ViewModels.LegDTOs;
using TLIS_Service.Helpers;
using TLIS_Service.IService;
using TLIS_Service.ServiceBase;

namespace TLIS_API.Controllers
{
    [ServiceFilter(typeof(LogFilterAttribute))]
    [Route("api/[controller]")]

    public class LegController : Controller
    {
        private IUnitOfWorkService _unitOfWorkService;
        private readonly IMapper _mapper;
        public LegController(IUnitOfWorkService unitOfWorkService,IMapper mapper)
        {
            _unitOfWorkService = unitOfWorkService;
            _mapper = mapper;
        }

        [HttpGet("getAll")]
        [ProducesResponseType(200, Type = typeof(Response<List<LegViewModel>>))]
        public IActionResult GetAll([FromQueryAttribute] ParameterPagination parameterPagination, [FromQueryAttribute] ClassFilter classFilter)
        {

            return Ok(new Response<LegViewModel>());
        }

        [HttpGet("getById/{id}")]
        [ProducesResponseType(200, Type = typeof(LegViewModel))]
        public async Task<IActionResult> GetCivilNonSteelLibrary(int id)
        {
            var result = await _unitOfWorkService.LegService.GetById(id);
            var resultVM = _mapper.Map<LegViewModel>(result);

            return Ok(new Response<LegViewModel>(resultVM));

        }
        [HttpPost("AddLeg")]
        [ProducesResponseType(200, Type = typeof(List<AddLegViewModel>))]
        public IActionResult AddLeg([FromBody] AddLegViewModel addLegViewModel)
        {
            if (TryValidateModel(addLegViewModel, nameof(AddLegViewModel)))
            {
                _unitOfWorkService.LegService.AddLeg(addLegViewModel);
                return Ok();
            }
            else
            {
                var ErrorMessages = from state in ModelState.Values
                                    from error in state.Errors
                                    select error.ErrorMessage;
                return Ok(new Response<AddLegViewModel>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
            }
        }

        [HttpPost("EditLeg")]
        [ProducesResponseType(200, Type = typeof(List<EditLegViewModel>))]
        public async Task<IActionResult> EditLeg([FromBody] EditLegViewModel editLegViewModel)
        {
            if (TryValidateModel(editLegViewModel, nameof(EditLegViewModel)))
            {
                await _unitOfWorkService.LegService.EditLeg(editLegViewModel);
                return Ok();
            }
            else
            {
                var ErrorMessages = from state in ModelState.Values
                                    from error in state.Errors
                                    select error.ErrorMessage;
                return Ok(new Response<EditLegViewModel>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
            }
        }

        [HttpGet("GetLegsByCivilId/{CivilId}")]
        [ProducesResponseType(200, Type = typeof(List<LegViewModel>))]
        public IActionResult GetLegsByCivilId(int CivilId)
        {
            var response = _unitOfWorkService.LegService.GetLegsByCivilId(CivilId);
            return Ok(response);
        }
        [HttpGet("GetLegsByAllCivilInstId/{AllCivilInstId}")]
        [ProducesResponseType(200, Type = typeof(List<LegViewModel>))]
        public IActionResult GetLegsByAllCivilInstId(int AllCivilInstId)
        {
            var response = _unitOfWorkService.LegService.GetLegsByAllCivilInstId(AllCivilInstId);
            return Ok(response);
        }
    }
}