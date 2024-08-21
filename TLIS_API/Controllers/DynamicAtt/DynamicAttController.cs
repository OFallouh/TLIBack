using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using TLIS_API.Middleware.WorkFlow;
using TLIS_DAL.Helper;
using TLIS_DAL.Helper.Filters;
using TLIS_DAL.Helpers;
using TLIS_DAL.ViewModels.DependencyDTOs;
using TLIS_DAL.ViewModels.DynamicAttDTOs;
using TLIS_DAL.ViewModels.OperationDTOs;
using TLIS_Service.Helpers;
using TLIS_Service.ServiceBase;
using static TLIS_Service.Services.DynamicAttService;

namespace TLIS_API.Controllers.DynamicAtt
{
   
    [ServiceFilter(typeof(LogFilterAttribute))]
    [Route("api/[controller]")]
    [ApiController]
    public class DynamicAttController : Controller
    {
        private readonly IUnitOfWorkService _unitOfWorkService;
        private readonly IConfiguration _configuration;
        public DynamicAttController(IUnitOfWorkService unitOfWorkService, IConfiguration configuration)
        {
            _unitOfWorkService = unitOfWorkService;
            _configuration = configuration;
        }

        [HttpPost("AddDynamicAtts")]
        [ProducesResponseType(200, Type = typeof(AddDynamicAttViewModel))]
        public IActionResult AddDynamicAtts([FromBody] AddDependencyViewModel addDependencyView)
        {
            if (ModelState.IsValid)
            {
                var ConnectionString = _configuration["ConnectionStrings:ActiveConnection"];
                var responceResult = _unitOfWorkService.DynamicAttService.AddDynamicAtts(addDependencyView, ConnectionString);
                return Ok(responceResult);
            }
            else
            {
                var ErrorMessages = from state in ModelState.Values
                                    from error in state.Errors
                                    select error.ErrorMessage;
                return Ok(new Response<AddDynamicAttViewModel>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
            }

        }
        [ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
        [HttpPost("AddDynamicAttLibValue")]
        [ProducesResponseType(200, Type = typeof(List<AddDynamicLibAttValueViewModel>))]
        public IActionResult AddLibDynamicAttLibValue([FromBody] AddDynamicLibAttValueViewModel addDynamicLibAttValueViewModel)
        {
            if (ModelState.IsValid)
            {
                var responceResult = _unitOfWorkService.DynamicAttService.AddDynamicAttLibValue(addDynamicLibAttValueViewModel);
                return Ok(responceResult);
            }
            else
            {
                var ErrorMessages = from state in ModelState.Values
                                    from error in state.Errors
                                    select error.ErrorMessage;
                return Ok(new Response<List<AddDynamicLibAttValueViewModel>>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
            }
        }
        [ServiceFilter(typeof(WorkFlowMiddleware))]
        [HttpPost("AddDynamicAttInst")]
        [ProducesResponseType(200, Type = typeof(List<AddDependencyInstViewModel>))]
        public IActionResult AddLibDynamicAttLIns([FromBody] AddDependencyInstViewModel addDependencyInstViewModel)
        {
            if (ModelState.IsValid)
            {
                var ConnectionString = _configuration["ConnectionStrings:ActiveConnection"];
                var responceResult = _unitOfWorkService.DynamicAttService.AddDynamicAttInst(addDependencyInstViewModel, ConnectionString);
                return Ok(responceResult);
            }
            else
            {
                var ErrorMessages = from state in ModelState.Values
                                    from error in state.Errors
                                    select error.ErrorMessage;
                return Ok(new Response<List<AddDynamicAttInstViewModel>>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
            }
        }
        //[ServiceFilter(typeof(WorkFlowMiddleware))]
        [HttpPost("AddDynamicLibrary")]
        [ProducesResponseType(200, Type = typeof(List<AddDynamicObject>))]
        public IActionResult AddDynamic([FromBody] AddDynamicObject addDependencyInstViewModel,string TabelName,int? CategoryId)
        {
            if (ModelState.IsValid)
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
                var responceResult = _unitOfWorkService.DynamicAttService.AddDynamic(addDependencyInstViewModel, ConnectionString, TabelName, userId, CategoryId);
                return Ok(responceResult);
            }
            else
            {
                var ErrorMessages = from state in ModelState.Values
                                    from error in state.Errors
                                    select error.ErrorMessage;
                return Ok(new Response<List<AddDynamicAttInstViewModel>>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
            }
        }
        [ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
        [HttpGet("GetDependencyPropertyLib")]
        [ProducesResponseType(200, Type = typeof(List<DependencyColumnForAdd>))]
        public IActionResult GetDependencyPropertyLib(string tableName, int? CategoryId)
        {
            var responceResult = _unitOfWorkService.DynamicAttService.GetDependencyLib(tableName, CategoryId);
            return Ok(responceResult);
        }
        [HttpPost("GetDynamicAttributeValidation")]
        [ProducesResponseType(200, Type = typeof(DynamicAttributeValidations))]
        public IActionResult GetDynamicAttributeValidation(int DynamicAttId)
        {
            var responceResult = _unitOfWorkService.DynamicAttService.GetDynamicAttributeValidation(DynamicAttId);
            return Ok(responceResult);
        }

        [HttpGet("GetDependencyInst")]
        [ProducesResponseType(200, Type = typeof(List<DependencyColumnForAdd>))]
        public IActionResult GetDependencyInst(string Layer, int? CategoryId, bool IsLibrary = false)
        {
            var responceResult = _unitOfWorkService.DynamicAttService.GetDependencyInst(Layer, CategoryId, IsLibrary);
            return Ok(responceResult);
        }

        [HttpGet("AddDependencyLib")]
        [ProducesResponseType(200, Type = typeof(List<DependencyColumnForAdd>))]
        public IActionResult AddDependencyLib(string tableName, int? CategoryId)
        {
            if (ModelState.IsValid)
            {
                var responceResult = _unitOfWorkService.DynamicAttService.GetDependencyLib(tableName, CategoryId);
                return Ok(responceResult);
            }
            else
            {
                var ErrorMessages = from state in ModelState.Values
                                    from error in state.Errors
                                    select error.ErrorMessage;
                return Ok(new Response<List<DependencyColumn>>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
            }
        }
        [HttpGet("GetForAdd")]
        [ProducesResponseType(200, Type = typeof(DynamicAttLibForAddViewModel))]
        public IActionResult GetForAdd()
        {
            var response = _unitOfWorkService.DynamicAttService.GetForAdd();
            return Ok(response);
        }
        [HttpPost("GetDynamicAtts")]
        [ProducesResponseType(200, Type = typeof(List<DynamicAttViewModel>))]
        public IActionResult GetDynamicAtts([FromBody] List<FilterObjectList> filters, [FromQuery] ParameterPagination parameters)
        {
            var response = _unitOfWorkService.DynamicAttService.GetDynamicAtts(filters, parameters);
            return Ok(response);
        }
        [HttpPost("GetDynamicAttsByTableName")]
        [ProducesResponseType(200, Type = typeof(List<DynamicAttViewModel>))]
        public IActionResult GetDynamicAttsByTableName([FromBody] List<FilterObjectList> filters, [FromQuery] ParameterPagination parameters, string TableName, int? CategoryId)
        {
            var response = _unitOfWorkService.DynamicAttService.GetDynamicAttsByTableName(filters, parameters, TableName, CategoryId);
            return Ok(response);
        }
        [HttpPost("GeStaticAttsAndDynamicAttsByTableName")]
        [ProducesResponseType(200, Type = typeof(List<DynamicAttViewModel>))]
        public IActionResult GeStaticAttsAndDynamicAttsByTableName( String TabelName,bool IsLibrary,int? CategoryId)
        {
            var response = _unitOfWorkService.DynamicAttService.GeStaticAttsAndDynamicAttsByTableName(TabelName, IsLibrary, CategoryId);
            return Ok(response);
        }
        [HttpGet("GetById")]
        [ProducesResponseType(200, Type = typeof(DynamicAttViewModel))]
        public IActionResult GetById(int Id)
        {
            var response = _unitOfWorkService.DynamicAttService.GetById(Id);
            return Ok(response);
        }
        [HttpPost("EditDynamicAtt")]
        [ProducesResponseType(200, Type = typeof(DynamicAttViewModel))]
        public async Task<IActionResult> EditDynamicAtt([FromBody] EditDynamicAttViewModel dynamicAttViewModel)
        {
            var ConnectionString = _configuration["ConnectionStrings:ActiveConnection"];
            var response = await _unitOfWorkService.DynamicAttService.Edit(dynamicAttViewModel, ConnectionString);
            return Ok(response);
        }
        [HttpPost("Disable")]
        [ProducesResponseType(200, Type = typeof(DynamicAttViewModel))]
        public IActionResult Disable(int RecordId)
        {
            var ConnectionString = _configuration["ConnectionStrings:ActiveConnection"];
            var response = _unitOfWorkService.DynamicAttService.Disable(RecordId, ConnectionString);
            return Ok(response);
        }
        [HttpPost("RequiredNOTRequired")]
        [ProducesResponseType(200, Type = typeof(DynamicAttViewModel))]
        public IActionResult RequiredNOTRequired(int DynamicAttId)
        {
            var ConnectionString = _configuration["ConnectionStrings:ActiveConnection"];
            var response = _unitOfWorkService.DynamicAttService.RequiredNOTRequired(DynamicAttId, ConnectionString);
            return Ok(response);
        }
        [HttpPost("GetLayers")]
        [ProducesResponseType(200, Type = typeof(List<OutPutString>))]
        public IActionResult GetLayers(string TableName)
        {
            var response = _unitOfWorkService.DynamicAttService.GetLayers(TableName);
            return Ok(response);
        }
        [HttpPost("GetForAddingDynamicAttribute")]
        [ProducesResponseType(200, Type = typeof(FirstStepAddDependencyViewModel))]
        public IActionResult GetForAddingDynamicAttribute(string TableName)
        {
            var response = _unitOfWorkService.DynamicAttService.GetForAddingDynamicAttribute(TableName);
            return Ok(response);
        }
        [HttpGet("CheckEditingDynamicAttDataType")]
        [ProducesResponseType(200, Type = typeof(DynamicAttViewModel))]
        public IActionResult CheckEditingDynamicAttDataType(int DynamicAttributeId, int NewDataTypeId)
        {
            var response = _unitOfWorkService.DynamicAttService.CheckEditingDynamicAttDataType(DynamicAttributeId, NewDataTypeId);
            return Ok(response);

        }
        [HttpPost("GetDynamicLibraryById")]
        [ProducesResponseType(200, Type = typeof(FirstStepAddDependencyViewModel))]
        public IActionResult GetDynamicLibraryById(int DynamicAttributeId)
        {
            var response = _unitOfWorkService.DynamicAttService.GetDynamicLibraryById(DynamicAttributeId);
            return Ok(response);
        }
    }
}