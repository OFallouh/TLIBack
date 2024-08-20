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
using TLIS_DAL.ViewModels;
using TLIS_DAL.ViewModels.CivilWithLegsDTOs;
using TLIS_DAL.ViewModels.MW_BUDTOs;
using TLIS_DAL.ViewModels.MW_DishDTOs;
using TLIS_DAL.ViewModels.MW_ODUDTOs;
using TLIS_DAL.ViewModels.Mw_OtherDTOs;
using TLIS_DAL.ViewModels.MW_RFUDTOs;
using TLIS_Service.Helpers;
using TLIS_Service.ServiceBase;
using TLIS_DAL.ViewModels.MW_BULibraryDTOs;
using TLIS_DAL.ViewModels.MW_PortDTOs;
using TLIS_DAL.ViewModels.SideArmDTOs;
using TLIS_API.Middleware.WorkFlow;
using System.IdentityModel.Tokens.Jwt;
using TLIS_DAL.ViewModels.SiteDTOs;
using TLIS_DAL.ViewModels.CivilLoadsDTOs;

namespace TLIS_API.Controllers
{
    [ServiceFilter(typeof(LogFilterAttribute))]
    [Route("api/[controller]")]
    [ApiController]
    public class MWInstController : ControllerBase
    {
        private readonly IUnitOfWorkService _unitOfWorkService;
        private readonly IConfiguration _configuration;
        public MWInstController(IUnitOfWorkService unitOfWorkService, IConfiguration configuration)
        {
            _unitOfWorkService = unitOfWorkService;
            _configuration = configuration;
        }
       // [ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
        [HttpGet("GetAttForAddMWBUInstallation")]
        [ProducesResponseType(200, Type = typeof(ObjectInstAtts))]
        public IActionResult GetAttForAddMWBUInstallation(int LibId, string SiteCode)
        {
            var response = _unitOfWorkService.MWInstService.GetAttForAddMWBUInstallation(Helpers.Constants.LoadSubType.TLImwBU.ToString(), LibId, SiteCode);
            return Ok(response);
        }
        //[ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
        [HttpGet("GetAttForAddMWODUInstallation")]
        [ProducesResponseType(200, Type = typeof(ObjectInstAtts))]
        public IActionResult GetAttForAddMW_ODU(int LibId, string SiteCode)
        {
            var response = _unitOfWorkService.MWInstService.GetAttForAddMWODUInstallation(Helpers.Constants.LoadSubType.TLImwODU.ToString(), LibId, SiteCode);
            return Ok(response);
        }
        //[ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
        [HttpGet("GetAttForAddMW_DishInstallation")]
        [ProducesResponseType(200, Type = typeof(GetForAddMWDishInstallationObject))]
        public IActionResult GetAttForAddMW_Dish(int LibId, string SiteCode)
        {
            var response = _unitOfWorkService.MWInstService.GetAttForAddMWDishInstallation(Helpers.Constants.LoadSubType.TLImwDish.ToString(), LibId, SiteCode);
            return Ok(response);
        }
        //[ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
        [HttpGet("GetAttForAddMWRFUInstallation")]
        [ProducesResponseType(200, Type = typeof(ObjectInstAtts))]
        public IActionResult GetAttForAddMWRFUInstallation(int LibId, string SiteCode)
        {
            var response = _unitOfWorkService.MWInstService.GetAttForAddMWRFUInstallation(Helpers.Constants.LoadSubType.TLImwRFU.ToString(), LibId, SiteCode);
            return Ok(response);
        }
        [ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
        [HttpGet("GetAttForAddMWOtherInstallation")]
        [ProducesResponseType(200, Type = typeof(ObjectInstAtts))]
        public IActionResult GetAttForAddMWOtherInstallation(int LibId, string SiteCode)
        {
            var response = _unitOfWorkService.MWInstService.GetAttForAddMWOtherInstallation(Helpers.Constants.LoadSubType.TLImwOther.ToString(), LibId, SiteCode);
            return Ok(response);
        }
        [ServiceFilter(typeof(WorkFlowMiddleware))]
        [HttpPost("AddMW_BUInstallation")]
        [ProducesResponseType(200, Type = typeof(AddMWBUInstallationObject))]
        public IActionResult AddMW_BUInstallation([FromBody] AddMWBUInstallationObject AddMW_BUViewModel, string SiteCode, int? TaskId)
        {
            if (TryValidateModel(AddMW_BUViewModel, nameof(AddMW_BUViewModel)))
                
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
                var response = _unitOfWorkService.MWInstService.AddMWInstallation(userId, AddMW_BUViewModel, Helpers.Constants.LoadSubType.TLImwBU.ToString(), SiteCode, ConnectionString, TaskId);
                return Ok(response);
            }
            else
            {
                var ErrorMessages = from state in ModelState.Values
                                    from error in state.Errors
                                    select error.ErrorMessage;
                return Ok(new Response<AddMWBUInstallationObject>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
            }
        }
        [ServiceFilter(typeof(WorkFlowMiddleware))]
        [HttpPost("AddMW_ODUInstallation")]
        [ProducesResponseType(200, Type = typeof(AddMwODUinstallationObject))]
        public IActionResult AddMW_ODU([FromBody] AddMwODUinstallationObject AddMW_ODUViewModel, string SiteCode, int? TaskId)
        {
         
            if (TryValidateModel(AddMW_ODUViewModel, nameof(AddMW_ODUViewModel)))
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
                var response = _unitOfWorkService.MWInstService.AddMWInstallation(userId,AddMW_ODUViewModel, Helpers.Constants.LoadSubType.TLImwODU.ToString(), SiteCode, ConnectionString, TaskId);
                return Ok(response);
            }
            else
            {
                var ErrorMessages = from state in ModelState.Values
                                    from error in state.Errors
                                    select error.ErrorMessage;
                return Ok(new Response<AddMW_ODUViewModel>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
            }
        }
        [ServiceFilter(typeof(WorkFlowMiddleware))]
        [HttpPost("AddMW_Dish")]
        [ProducesResponseType(200, Type = typeof(AddMWDishInstallationObject))]
        public IActionResult AddMW_Dish([FromBody]AddMWDishInstallationObject AddMW_DishViewModel, string SiteCode, int? TaskId)
        {
           
            if (TryValidateModel(AddMW_DishViewModel, nameof(AddMW_DishViewModel)))
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
                var response = _unitOfWorkService.MWInstService.AddMWInstallation(userId, AddMW_DishViewModel, Helpers.Constants.LoadSubType.TLImwDish.ToString(), SiteCode, ConnectionString, TaskId);
                return Ok(response);
            }
            else
            {
                var ErrorMessages = from state in ModelState.Values
                                    from error in state.Errors
                                    select error.ErrorMessage;
                return Ok(new Response<AddMW_DishViewModel>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
            }
        }
        [ServiceFilter(typeof(WorkFlowMiddleware))]
        [HttpPost("AddMWRFUInstallation")]
        [ProducesResponseType(200, Type = typeof(AddMWRFUInstallation))]
        public IActionResult AddMW_RFU([FromBody] AddMWRFUInstallation AddMW_RFUViewModel, string SiteCode, int? TaskId)
        {
            if (TryValidateModel(AddMW_RFUViewModel, nameof(AddMW_RFUViewModel)))
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
                var response = _unitOfWorkService.MWInstService.AddMWRFUInstallation(AddMW_RFUViewModel, Helpers.Constants.LoadSubType.TLImwRFU.ToString(), SiteCode, ConnectionString, TaskId, userId);
                return Ok(response);
            }
            else
            {
                var ErrorMessages = from state in ModelState.Values
                                    from error in state.Errors
                                    select error.ErrorMessage;
                return Ok(new Response<AddMWRFUInstallation>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
            }
        }
        [ServiceFilter(typeof(WorkFlowMiddleware))]
        [HttpPost("AddMW_OtherInstallation")]
        [ProducesResponseType(200, Type = typeof(AddMWOtherInstallationObject))]
        public IActionResult AddMW_Other([FromBody] AddMWOtherInstallationObject AddMw_OtherViewModel, string SiteCode, int? TaskId)
        {
           
            if (TryValidateModel(AddMw_OtherViewModel, nameof(AddMw_OtherViewModel)))
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
                var response = _unitOfWorkService.MWInstService.AddMWInstallation(userId,AddMw_OtherViewModel, Helpers.Constants.LoadSubType.TLImwOther.ToString(), SiteCode, ConnectionString, TaskId);
                return Ok(response);
            }
            else
            {
                var ErrorMessages = from state in ModelState.Values
                                    from error in state.Errors
                                    select error.ErrorMessage;
                return Ok(new Response<AddMWOtherInstallationObject>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
            }
        }

        //[ServiceFilter(typeof(WorkFlowMiddleware))]

        [HttpPost("EditMW_BUInstallation")]
        [ProducesResponseType(200, Type = typeof(EditMWBUInstallationObject))]
        public async Task<IActionResult> EditMW_BU([FromBody] EditMWBUInstallationObject MW_BU, int? TaskId)
        {
            if (TryValidateModel(MW_BU, nameof(EditMWBUInstallationObject)))
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
                var response = await _unitOfWorkService.MWInstService.EditMWBUInstallation(userId,MW_BU, Helpers.Constants.LoadSubType.TLImwBU.ToString(), TaskId, ConnectionString);
                return Ok(response);
            }
            else
            {
                var ErrorMessages = from state in ModelState.Values
                                    from error in state.Errors
                                    select error.ErrorMessage;
                return Ok(new Response<EditMWBUInstallationObject>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
            }
        }
        [ServiceFilter(typeof(WorkFlowMiddleware))]
        [HttpPost("EditMWDishInstallation")]
        [ProducesResponseType(200, Type = typeof(EditMWDishInstallationObject))]
        public async Task<IActionResult> EditMW_Dish([FromBody] EditMWDishInstallationObject MW_Dish,int? TaskId)
        {
            if (TryValidateModel(MW_Dish, nameof(EditMWDishInstallationObject)))
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
                var response = await _unitOfWorkService.MWInstService.EditMWDishInstallation(userId,MW_Dish, Helpers.Constants.LoadSubType.TLImwDish.ToString(), TaskId, ConnectionString);
                return Ok(response);
            }
            else
            {
                var ErrorMessages = from state in ModelState.Values
                                    from error in state.Errors
                                    select error.ErrorMessage;
                return Ok(new Response<EditMWDishInstallationObject>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
            }
        }
        [ServiceFilter(typeof(WorkFlowMiddleware))]
        [HttpPost("EditMWODUInstallation")]
        [ProducesResponseType(200, Type = typeof(EditMWODUInstallationObject))]
        public async Task<IActionResult> EditMW_ODU([FromBody] EditMWODUInstallationObject MW_ODU, int? TaskId)
        {
            if (TryValidateModel(MW_ODU, nameof(EditMWODUInstallationObject)))
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
                var response = await _unitOfWorkService.MWInstService.EditMWODUInstallation(userId, MW_ODU, Helpers.Constants.LoadSubType.TLImwODU.ToString(), TaskId, ConnectionString);
                return Ok(response);
            }
            else
            {
                var ErrorMessages = from state in ModelState.Values
                                    from error in state.Errors
                                    select error.ErrorMessage;
                return Ok(new Response<EditMWODUInstallationObject>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
            }
        }
        [ServiceFilter(typeof(WorkFlowMiddleware))]
        [HttpPost("EditMWRFUInstallation")]
        [ProducesResponseType(200, Type = typeof(EditMWRFUInstallationObject))]
        public async Task<IActionResult> EditMWRFUInstallation([FromBody] EditMWRFUInstallationObject MW_RFU, int? TaskId)
        {
            if (TryValidateModel(MW_RFU, nameof(EditMWRFUInstallationObject)))
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
                var response =await  _unitOfWorkService.MWInstService.EditMWRFUInstallation(MW_RFU, Helpers.Constants.LoadSubType.TLImwRFU.ToString(), ConnectionString, TaskId, userId);
                return Ok(response);
            }
            else
            {
                var ErrorMessages = from state in ModelState.Values
                                    from error in state.Errors
                                    select error.ErrorMessage;
                return Ok(new Response<EditMWRFUInstallationObject>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
            }
        }
        [ServiceFilter(typeof(WorkFlowMiddleware))]
        [HttpPost("EditMWOtherInstallation")]
        [ProducesResponseType(200, Type = typeof(EditMWOtherInstallationObject))]
        public async Task<IActionResult> EditMWOtherInstallation([FromBody] EditMWOtherInstallationObject Mw_Other, int? TaskId)
        {
            if (TryValidateModel(Mw_Other, nameof(EditMWOtherInstallationObject)))
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
                var response = await _unitOfWorkService.MWInstService.EditMWOtherInstallation(userId,Mw_Other, Helpers.Constants.LoadSubType.TLImwOther.ToString(), TaskId, ConnectionString);
                return Ok(response);
            }
            else
            {
                var ErrorMessages = from state in ModelState.Values
                                    from error in state.Errors
                                    select error.ErrorMessage;
                return Ok(new Response<EditMWOtherInstallationObject>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
            }
        }
        [ServiceFilter(typeof(WorkFlowMiddleware))]
        [HttpPost("DismantleMW_BU")]
        public IActionResult DismantleMW_BU(string sitecode, int Id,int? TaskId)
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
            var response = _unitOfWorkService.MWInstService.DismantleLoads(sitecode, Id, Helpers.Constants.LoadSubType.TLImwBU.ToString(), TaskId, userId, ConnectionString);
            return Ok(response);
        }
        [ServiceFilter(typeof(WorkFlowMiddleware))]
        [HttpPost("DismantleMW_ODU")]
        public IActionResult DismantleMW_ODU(string sitecode, int Id, int? TaskId)
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
            var response = _unitOfWorkService.MWInstService.DismantleLoads(sitecode, Id, Helpers.Constants.LoadSubType.TLImwODU.ToString(), TaskId, userId, ConnectionString);
            return Ok(response);
        }
        [ServiceFilter(typeof(WorkFlowMiddleware))]
        [HttpPost("DismantleMW_RFU")]

        public IActionResult DismantleMW_RFU(string sitecode, int Id, int? TaskId)
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
            var response = _unitOfWorkService.MWInstService.DismantleLoads(sitecode, Id, Helpers.Constants.LoadSubType.TLImwRFU.ToString(), TaskId, userId, ConnectionString);
            return Ok(response);
        }
        [ServiceFilter(typeof(WorkFlowMiddleware))]
        [HttpPost("DismantleMW_Dish")]

        public IActionResult DismantleMW_Dish(string sitecode, int Id, int? TaskId)
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
            var response = _unitOfWorkService.MWInstService.DismantleLoads(sitecode, Id, Helpers.Constants.LoadSubType.TLImwDish.ToString(), TaskId, userId, ConnectionString);
            return Ok(response);
        }
        [ServiceFilter(typeof(WorkFlowMiddleware))]
        [HttpPost("DismantleMW_Other")]

        public IActionResult DismantleMW_Other(string sitecode, int Id, int? TaskId)
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
            var response = _unitOfWorkService.MWInstService.DismantleLoads(sitecode, Id, Helpers.Constants.LoadSubType.TLImwOther.ToString(), TaskId, userId, ConnectionString);
            return Ok(response);
        }
       // [ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
        [HttpGet("GetMWBUInstallationById")]
        [ProducesResponseType(200, Type = typeof(GetForAddMWDishInstallationObject))]
        public IActionResult GetMWBUInstallationById(int MW_BU)
        {
            var response = _unitOfWorkService.MWInstService.GetMWBUInstallationById(MW_BU, Helpers.Constants.LoadSubType.TLImwBU.ToString());
            return Ok(response);
        }
        //[ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
        [HttpGet("GetMWODUInstallationById")]
        [ProducesResponseType(200, Type = typeof(ObjectInstAttsForSideArm))]
        public IActionResult GetMW_ODUById(int MW_ODU)
        {
            var response = _unitOfWorkService.MWInstService.GetMWODUInstallationById(MW_ODU, Helpers.Constants.LoadSubType.TLImwODU.ToString());
            return Ok(response);
        }
       // [ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
        [HttpGet("GetMWDishInstallationById")]
        [ProducesResponseType(200, Type = typeof(GetForAddLoadObject))]
        public IActionResult GetMW_DishById(int MW_Dish)
        {
            var response = _unitOfWorkService.MWInstService.GetMWDishInstallationById(MW_Dish, Helpers.Constants.LoadSubType.TLImwDish.ToString());
            return Ok(response);
        }
       // [ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
        [HttpGet("GetMWRFUInstallationById")]
        [ProducesResponseType(200, Type = typeof(ObjectInstAttsForSideArm))]
        public IActionResult GetMWRFUInstallationById(int MW_RFU)
        {
            var response = _unitOfWorkService.MWInstService.GetMWRFUInstallationById(MW_RFU, Helpers.Constants.LoadSubType.TLImwRFU.ToString());
            return Ok(response);
        }
        //[ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
        [HttpGet("GetMWOtherInstallationById")]
        [ProducesResponseType(200, Type = typeof(ObjectInstAttsForSideArm))]
        public IActionResult GetMWOtherInstallationById(int mwOther)
        {
            var response = _unitOfWorkService.MWInstService.GetMWOtherInstallationById(mwOther, Helpers.Constants.LoadSubType.TLImwOther.ToString());
            return Ok(response);
        }
        [ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
        [HttpPost("getMW_BU")]
        [ProducesResponseType(200, Type = typeof(List<MW_BUViewModel>))]
        public IActionResult GetMW_BU([FromBody]List<FilterObjectList> filters, bool WithFilterData, [FromQuery]ParameterPagination parameters)
        {
            var response = _unitOfWorkService.MWInstService.getMW_BU(filters, WithFilterData, parameters);
            return Ok(response);

        }
        [ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
        [HttpPost("getMW_ODU")]
        [ProducesResponseType(200, Type = typeof(List<MW_ODUViewModel>))]
        public IActionResult GetMW_ODU([FromBody]List<FilterObjectList> filters, bool WithFilterData, [FromQuery]ParameterPagination parameters)
        {
            var response = _unitOfWorkService.MWInstService.getMW_ODU(filters, WithFilterData, parameters);
            return Ok(response);
        }
        [ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
        [HttpPost("getMW_Dish")]
        [ProducesResponseType(200, Type = typeof(List<MW_DishViewModel>))]
        public IActionResult GetMW_Dish([FromBody]List<FilterObjectList> filters, bool WithFilterData, [FromQuery] ParameterPagination parameters)
        {
            var response = _unitOfWorkService.MWInstService.getMW_Dish(filters, WithFilterData, parameters);
            return Ok(response);
        }
        [ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
        [HttpPost("getMW_RFU")]
        [ProducesResponseType(200, Type = typeof(List<MW_RFUViewModel>))]
        public IActionResult GetMW_RFU([FromBody]List<FilterObjectList> filters, bool WithFilterData, [FromQuery] ParameterPagination parameters)
        {
            var response = _unitOfWorkService.MWInstService.getMW_RFU(filters, WithFilterData, parameters);
            return Ok(response);

        }
        [ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
        [HttpGet("GetInstallationPlaces")]
        [ProducesResponseType(200, Type = typeof(List<InstallationPlaceViewModel>))]
        public IActionResult GetInstallationPlaces(string TableName, string? LoadType)
        {
            var response = _unitOfWorkService.MWInstService.GetInstallationPlaces(TableName, LoadType);
            return Ok(response);
        }
        [ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
        [HttpGet("GetInstallationType")]
        [ProducesResponseType(200, Type = typeof(List<InstallationPlaceViewModel>))]
        public IActionResult GetInstallationType(string TableName)
        {
            var response = _unitOfWorkService.MWInstService.GetInstallationType(TableName);
            return Ok(response);
        }
        [ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
        [HttpGet("GetMW_PortsForMW_RFUInstallation")]
        [ProducesResponseType(200, Type = typeof(List<MW_PortViewModel>))]
        public IActionResult GetMW_PortsForMW_RFUInstallation(int AllCivilInstId)
        {
            var response = _unitOfWorkService.MWInstService.GetMW_PortsForMW_RFUInstallation(AllCivilInstId);
            return Ok(response);
        }
        [ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
        [HttpGet("GetMW_BULibrariesForMW_BUInstallation")]
        [ProducesResponseType(200, Type = typeof(List<MW_BULibraryViewModel>))]
        public IActionResult GetMW_BULibrariesForMW_BUInstallation()
        {
            var response = _unitOfWorkService.MWInstService.GetMW_BULibrariesForMW_BUInstallation();
            return Ok(response);
        }

        [ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
        [HttpGet("GetMW_BU_Available")]
        [ProducesResponseType(200, Type = typeof(List<MW_Free_BUInstDto>))]
        public IActionResult GetMW_BU_Available(int AllCivilInstId)
        {
            var response = _unitOfWorkService.MWInstService.GetMw_Free_BuInst(AllCivilInstId);
            return Ok(response);
        }

        [ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
        [HttpGet("GetFreePortOnBU")]
        [ProducesResponseType(200, Type = typeof(List<MW_PortViewModel>))]
        public IActionResult GetFreePortOnBU(int BUid)
        {
            var response = _unitOfWorkService.MWInstService.GetFreePortOnBU(BUid);
            return Ok(response);
        }
        [ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]

        [HttpGet("GetFreeCascadeBU")]
        [ProducesResponseType(200, Type = typeof(List<MW_Free_BUInstDto>))]
        public IActionResult GetMw_Free_Cascade_BuInst(int AllCivilInstId)
        {
            var response = _unitOfWorkService.MWInstService.GetMw_Free_Cascade_BuInst(AllCivilInstId);
            return Ok(response);

        }
        [ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
        [HttpGet("GetFreeDishesForMW_ODU")]
        [ProducesResponseType(200, Type = typeof(List<MW_DishGetForAddViewModel>))]
        public IActionResult GetFreeDishesForMW_ODU(int AllCivilInstId)
        {
            var response = _unitOfWorkService.MWInstService.GetFreeDishesForMW_ODU(AllCivilInstId);
            return Ok(response);
        }
        [ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
        [HttpGet("GetPortCascadedByBUId")]
        [ProducesResponseType(200, Type = typeof(List<MW_PortViewModel>))]
        public IActionResult GetPortCascadedByBUId(int BUId, int? MainBUId)
        {
            var response = _unitOfWorkService.MWInstService.GetPortCascadedByBUId(BUId, MainBUId);
            return Ok(response);
        }
        //[ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
        [HttpPost("GetMWDishInstallationWithEnableAtt")]
        [ProducesResponseType(200, Type = typeof(object))]
        public IActionResult GetMW_DishOnSiteWithEnableAtt([FromQuery] string? SiteCode)
        {
            string ConnectionString = _configuration["ConnectionStrings:ActiveConnection"];
            var response = _unitOfWorkService.MWInstService.GetMWDishInstallationWithEnableAtt(SiteCode, ConnectionString);
            return Ok(response);
        }
        [HttpPost("GetMWODUInstallationWithEnableAtt")]
        [ProducesResponseType(200, Type = typeof(object))]
        public IActionResult GetMWODUInstallationWithEnableAtt([FromQuery] string? SiteCode)
        {
            string ConnectionString = _configuration["ConnectionStrings:ActiveConnection"];
            var response = _unitOfWorkService.MWInstService.GetMWODUInstallationWithEnableAtt(SiteCode, ConnectionString);
            return Ok(response);
        }
        [HttpPost("GetMWBUInstallationWithEnableAtt")]
        [ProducesResponseType(200, Type = typeof(object))]
        public IActionResult GetMWBUInstallationWithEnableAtt([FromQuery] string? SiteCode)
        {
            string ConnectionString = _configuration["ConnectionStrings:ActiveConnection"];
            var response = _unitOfWorkService.MWInstService.GetMWBUInstallationWithEnableAtt(SiteCode, ConnectionString);
            return Ok(response);
        }
        [HttpPost("GetMWOtherInstallationWithEnableAtt")]
        [ProducesResponseType(200, Type = typeof(object))]
        public IActionResult GetMWOtherInstallationWithEnableAtt([FromQuery] string? SiteCode)
        {
            string ConnectionString = _configuration["ConnectionStrings:ActiveConnection"];
            var response = _unitOfWorkService.MWInstService.GetMWOtherInstallationWithEnableAtt(SiteCode, ConnectionString);
            return Ok(response);


        }
        [HttpPost("GetMWRFUInstallationWithEnableAtt")]
        [ProducesResponseType(200, Type = typeof(object))]
        public IActionResult GetMWRFUInstallationWithEnableAtt([FromQuery] string? SiteCode)
        {
            string ConnectionString = _configuration["ConnectionStrings:ActiveConnection"];
            var response = _unitOfWorkService.MWInstService.GetMWRFUInstallationWithEnableAtt(SiteCode, ConnectionString);
            return Ok(response);
        }
    }
}
