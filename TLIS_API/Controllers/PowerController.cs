using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using TLIS_DAL.Helper;
using TLIS_DAL.Helper.Filters;
using TLIS_DAL.Helpers;
using TLIS_DAL.ViewModelBase;
using TLIS_DAL.ViewModels.PowerDTOs;
using TLIS_Service.Helpers;
using TLIS_Service.ServiceBase;
using TLIS_DAL.ViewModels.PowerTypeDTOs;
using TLIS_DAL.ViewModels.SideArmDTOs;
using TLIS_API.Middleware.WorkFlow;
using System.IdentityModel.Tokens.Jwt;

namespace TLIS_API.Controllers
{
    [ServiceFilter(typeof(LogFilterAttribute))]
    [Route("api/[controller]")]
    [ApiController]
    public class PowerController : ControllerBase
    {
        private readonly IUnitOfWorkService _unitOfWorkService;
        private readonly IConfiguration _configuration;
        public PowerController(IUnitOfWorkService unitOfWorkService, IConfiguration configuration)
        {
            _unitOfWorkService = unitOfWorkService;
            _configuration = configuration;
        }
        [ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
        [HttpGet("GetAttForAdd")]
        [ProducesResponseType(200, Type = typeof(ObjectInstAtts))]
        public IActionResult GetAttForAdd(int LibraryId, string SiteCode)
        {
            var response = _unitOfWorkService.PowerService.GetAttForAdd(LibraryId, SiteCode);
            return Ok(response);
        }
        [ServiceFilter(typeof(WorkFlowMiddleware))]
        [HttpPost("AddPower")]
        [ProducesResponseType(200, Type = typeof(ObjectInstAtts))]
        public IActionResult AddPower([FromBody] AddPowerViewModel Power, string SiteCode, int TaskId)
        {
            if (Power.TLIcivilLoads.sideArmId == 0)
                Power.TLIcivilLoads.sideArmId = null;
            var ConnectionString = _configuration["ConnectionStrings:ActiveConnection"];
            if (TryValidateModel(Power, nameof(AddPowerViewModel)))
            {
                var response = _unitOfWorkService.PowerService.AddPower(Power, SiteCode, ConnectionString, TaskId);
                return Ok(response);
            }
            else
            {
                var ErrorMessages = from state in ModelState.Values
                                    from error in state.Errors
                                    select error.ErrorMessage;
                return Ok(new Response<ObjectInstAtts>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
            }
        }
        [ServiceFilter(typeof(WorkFlowMiddleware))]
        [HttpGet("DismantlePower")]

        public IActionResult DismantlePower(string sitecode, int Id , int TaskId)
        {
            string authHeader = HttpContext.Request.Headers["Authorization"];

            if (string.IsNullOrEmpty(authHeader) || !authHeader.ToLower().StartsWith("bearer "))
            {
                return Unauthorized();
            }

            var token = authHeader.Substring("Bearer ".Length).Trim();
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(token) as JwtSecurityToken;

            if (jsonToken == null)
            {
                return Unauthorized();
            }

            string userInfo = jsonToken.Claims.First(c => c.Type == "sub").Value;
            var userId = Convert.ToInt32(userInfo);
            var ConnectionString = _configuration["ConnectionStrings:ActiveConnection"];
            var response = _unitOfWorkService.PowerService.DismantleLoads(sitecode, Id, Helpers.Constants.LoadSubType.TLIpower.ToString(), TaskId, userId, ConnectionString);
            return Ok(response);

        }
        [ServiceFilter(typeof(WorkFlowMiddleware))]
        [HttpPost("EditPower")]
        [ProducesResponseType(200, Type = typeof(EditPowerViewModel))]
        public async Task<IActionResult> EditPower([FromBody] EditPowerViewModel Power,int TaskId)
        {
            if (TryValidateModel(Power, nameof(EditPowerViewModel)))
            {
                var response = await _unitOfWorkService.PowerService.EditPower(Power, TaskId);
                return Ok(response);
            }
            else
            {
                var ErrorMessages = from state in ModelState.Values
                                    from error in state.Errors
                                    select error.ErrorMessage;
                return Ok(new Response<EditPowerViewModel>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
            }
        }
        [ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
        [HttpGet("GetById/{Id}")]
        [ProducesResponseType(200, Type = typeof(ObjectInstAttsForSideArm))]
        public IActionResult GetById(int Id)
        {
            var response = _unitOfWorkService.PowerService.GetById(Id);
            return Ok(response);
        }
        [ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
        [HttpPost("GetList")]
        [ProducesResponseType(200, Type = typeof(List<PowerViewModel>))]
        public IActionResult GetList([FromBody] List<FilterObjectList> filters, bool WithFilterData, [FromQuery] ParameterPagination parameters)
        {
            var response = _unitOfWorkService.PowerService.GetList(filters, WithFilterData, parameters);
            return Ok(response);
        }
        [ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
        [HttpGet("GetPowerTypes")]
        [ProducesResponseType(200, Type = typeof(List<PowerTypeViewModel>))]
        public IActionResult GetPowerTypes()
        {
            var response = _unitOfWorkService.PowerService.GetPowerTypes();
            return Ok(response);
        }
    }
}