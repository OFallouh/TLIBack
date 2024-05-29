using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TLIS_API.Middleware.WorkFlow;
using TLIS_DAL.Helper;
using TLIS_DAL.Helpers;
using TLIS_DAL.ViewModelBase;
using TLIS_Service.Helpers;
using TLIS_Service.ServiceBase;

namespace TLIS_API.Controllers
{
   // [ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
    [ServiceFilter(typeof(LogFilterAttribute))]
    [Route("api/[controller]")]
    [ApiController]
    public class ConfigurationAttsController : ControllerBase
    {
        private IUnitOfWorkService _unitOfWorkService;
        public ConfigurationAttsController(IUnitOfWorkService unitOfWorkService)
        {
            _unitOfWorkService = unitOfWorkService;
        }
        [HttpGet("GetConfigrationTables")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<ConfigurationListViewModel>))]
        public IActionResult GetConfigurationTables(string filterName)
        {
            var response = _unitOfWorkService.ConfigurationAttsService.GetConfigrationTables(filterName);
            return Ok(response);
        }
        [HttpPost("GetAll")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<ConfigurationAttsViewModel>))]
        public IActionResult GetConfigurationAtts(string TableName, [FromQuery]ParameterPagination Pagination)
        {
            var response = _unitOfWorkService.ConfigurationAttsService.GetAll(TableName, Pagination);
            return Ok(response);
        }

        [HttpGet("GetById")]
        [ProducesResponseType(200, Type = typeof(ConfigurationAttsViewModel))]
        public IActionResult GetById(string TableName, int Id)
        {
            var response = _unitOfWorkService.ConfigurationAttsService.GetById(TableName, Id);
            return Ok(response);
        }

        [HttpPost("Add")]
        [ProducesResponseType(200, Type = typeof(ConfigurationAttsViewModel))]
        public IActionResult Add([FromBody]AddConfigrationAttViewModel model)
        {
            if(TryValidateModel(model, nameof(ConfigurationAttsViewModel)))
            {
                var response = _unitOfWorkService.ConfigurationAttsService.Add(model);
                return Ok(response);
            }
            else
            {
                var ErrorMessages = from state in ModelState.Values
                                    from error in state.Errors
                                    select error.ErrorMessage;
                return Ok(new Response<ConfigurationAttsViewModel>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
            }
        }

        [HttpPost("Update")]
        [ProducesResponseType(200, Type = typeof(ConfigurationAttsViewModel))]
        public async Task<IActionResult> Update(string TableName, string ListName, string NewName, int RecordId)
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
            var response = await _unitOfWorkService.ConfigurationAttsService.Update(TableName, ListName, RecordId, NewName, userId);
            return Ok(response);          
        }
        [HttpPost("Disable")]
        [ProducesResponseType(200, Type = typeof(TableAffected))]
        public async Task<IActionResult> Disable( string TableName, int RecordId, string ListName)
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
            var response = await _unitOfWorkService.ConfigurationAttsService.Disable(TableName, RecordId, ListName, userId);
            return Ok(response);
        }
        [HttpPost("Delete")]
        [ProducesResponseType(200, Type = typeof(ConfigurationAttsViewModel))]
        public async Task<IActionResult> Delete(string TableName, int RecordId, string ListName)
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
            var response = await _unitOfWorkService.ConfigurationAttsService.Delete(TableName, RecordId, userId, ListName);
            return Ok(response);
        }
    }
}
