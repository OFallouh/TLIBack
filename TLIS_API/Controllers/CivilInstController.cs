using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.Linq.Mapping;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using TLIS_API.Middleware.WorkFlow;
using TLIS_DAL.Helper;
using TLIS_DAL.Helper.Filters;
using TLIS_DAL.Helpers;
using TLIS_DAL.ViewModelBase;
using TLIS_DAL.ViewModels;
using TLIS_DAL.ViewModels.CivilLoadsDTOs;
using TLIS_DAL.ViewModels.CivilNonSteelDTOs;
using TLIS_DAL.ViewModels.CivilWithLegsDTOs;
using TLIS_DAL.ViewModels.CivilWithoutLegDTOs;
using TLIS_DAL.ViewModels.DismantleDto;
using TLIS_DAL.ViewModels.LogicalOperationDTOs;
using TLIS_DAL.ViewModels.SideArmDTOs;
using TLIS_Service.Helpers;
using TLIS_Service.ServiceBase;

namespace TLIS_API.Controllers
{
    
    [ServiceFilter(typeof(LogFilterAttribute))]
    [Route("api/[controller]")]
    [ApiController]
    public class CivilInstController : ControllerBase
    {
        private readonly IUnitOfWorkService _unitOfWorkService;
        private readonly IConfiguration _configuration;
        public CivilInstController(IUnitOfWorkService unitOfWorkService, IConfiguration configuration)
        {
            _unitOfWorkService = unitOfWorkService;
            _configuration = configuration;
        }
        //[ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
        [HttpGet("GetForAddCivilWithLegInstallation")]
        [ProducesResponseType(200, Type = typeof(ObjectInstAtts))]
        public IActionResult GetAttForAddCivilWithLegs(int CivilLibraryId,string SiteCode)
        {
            var response = _unitOfWorkService.CivilInstService.GetForAddCivilWithLegInstallation(Helpers.Constants.CivilType.TLIcivilWithLegs.ToString(), CivilLibraryId, SiteCode);
            return Ok(response);
        }
        //[ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
        [HttpGet("GetForAddCivilWithOutLegInstallation_Capsule")]
        [ProducesResponseType(200, Type = typeof(ObjectInstAtts))]
        public IActionResult GetForAddCivilWithOutLegInstallation_Capsule(int CivilLibraryId, string SiteCode)
        {
            var response = _unitOfWorkService.CivilInstService.GetForAddCivilWithOutLegInstallation_Capsule(Helpers.Constants.CivilType.TLIcivilWithoutLeg.ToString(), CivilLibraryId, SiteCode);
            return Ok(response);
        }
        ///[ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
        [HttpGet("GetForAddCivilWithOutLegInstallation_Mast")]
        [ProducesResponseType(200, Type = typeof(ObjectInstAtts))]
        public IActionResult GetForAddCivilWithOutLegInstallation_Mast(int CivilLibraryId, string SiteCode)
        {
            var response = _unitOfWorkService.CivilInstService.GetForAddCivilWithOutLegInstallation_Mast(Helpers.Constants.CivilType.TLIcivilWithoutLeg.ToString(), CivilLibraryId, SiteCode);
            return Ok(response);
        }
        //[ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
        [HttpGet("GetForAddCivilWithOutLegInstallation_Monople")]
        [ProducesResponseType(200, Type = typeof(ObjectInstAtts))]
        public IActionResult GetForAddCivilWithOutLegInstallation_Monople(int CivilLibraryId, string SiteCode)
        {
            var response = _unitOfWorkService.CivilInstService.GetForAddCivilWithOutLegInstallation_Monople(Helpers.Constants.CivilType.TLIcivilWithoutLeg.ToString(), CivilLibraryId, SiteCode);
            return Ok(response);
        }
        //[ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
        [HttpGet("GetForAddCiviNonSteelInstallation")]
        [ProducesResponseType(200, Type = typeof(ObjectInstAtts))]
        public IActionResult GetForAddCiviNonSteelInstallation(int CivilLibraryId, string SiteCode)
        {
            var response = _unitOfWorkService.CivilInstService.GetForAddCiviNonSteelInstallation(Helpers.Constants.CivilType.TLIcivilNonSteel.ToString(), CivilLibraryId, SiteCode);
            return Ok(response);
        }
        //[ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
        [HttpPost("GetCivilWithLegsWithEnableAtt")]
        [ProducesResponseType(200, Type = typeof(object))]
        public IActionResult GetCivilWithLegsWithEnableAtt([FromQuery] string? SiteCode)
        {
            string ConnectionString = _configuration["ConnectionStrings:ActiveConnection"];
            var response = _unitOfWorkService.CivilInstService.GetCivilWithLegsWithEnableAtt(SiteCode, ConnectionString);
            return Ok(response);
        }
        //[ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
        [HttpPost("GetLoadsAndSideArmsForCivil")]
        [ProducesResponseType(200, Type = typeof(CivilLoads))]
        public IActionResult GetLoadsAndSideArmsForCivil(int CivilId, string CivilType)
        {
            var response = _unitOfWorkService.CivilInstService.GetLoadsAndSideArmsForCivil(CivilId, CivilType);
            return Ok(response);
        }
        [ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
        [HttpPost("GetLoadsOnSideArm")]
        [ProducesResponseType(200, Type = typeof(LoadsOnSideArm))]
        public IActionResult GetLoadsOnSideArm(int SideArmId)
        {
            var response = _unitOfWorkService.CivilInstService.GetLoadsOnSideArm(SideArmId);
            return Ok(response);
        }
        //[ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
        [HttpPost("GetCivilWithoutLegMastWithEnableAtt")]
        [ProducesResponseType(200, Type = typeof(object))]
        public IActionResult GetCivilWithoutLegMastWithEnableAttt( [FromQuery] string? SiteCode)
        {
            string ConnectionString = _configuration["ConnectionStrings:ActiveConnection"];
            var response = _unitOfWorkService.CivilInstService.GetCivilWithoutLegMastWithEnableAtt(SiteCode, ConnectionString);
            return Ok(response);
        }
        [ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
        [HttpPost("GetCivilWithoutLegMonopoleWithEnableAtt")]
        [ProducesResponseType(200, Type = typeof(object))]
        public IActionResult GetCivilWithoutLegMonopoleWithEnableAtt([FromQuery] string? SiteCode)
        {
            string ConnectionString = _configuration["ConnectionStrings:ActiveConnection"];
            var response = _unitOfWorkService.CivilInstService.GetCivilWithoutLegMonopoleWithEnableAtt(SiteCode, ConnectionString);
            return Ok(response);
        }
        [ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
        [HttpPost("GetCivilWithoutLegCapsuleWithEnableAtt")]
        [ProducesResponseType(200, Type = typeof(object))]
        public IActionResult GetCivilWithoutLegCapsuleWithEnableAtt([FromQuery] string? SiteCode )
        {
            string ConnectionString = _configuration["ConnectionStrings:ActiveConnection"];
            var response = _unitOfWorkService.CivilInstService.GetCivilWithoutLegCapsuleWithEnableAtt(SiteCode, ConnectionString);
            return Ok(response);
        }
       // [ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
        [HttpPost("GetCivilNonSteelWithEnableAtt")]
        [ProducesResponseType(200, Type = typeof(object))]
        public IActionResult GetCivilNonSteelWithEnableAtt( [FromQuery] string? SiteCode)
        {
            string ConnectionString = _configuration["ConnectionStrings:ActiveConnection"];
            var response = _unitOfWorkService.CivilInstService.GetCivilNonSteelWithEnableAtt(SiteCode, ConnectionString);
            return Ok(response);
        }
        [ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
        //[HttpPost("GetAllCivils")]
        //[ProducesResponseType(200, Type = typeof(object))]
        //public IActionResult GetAllCivils([FromQuery] SiteBaseFilter BaseFilter, bool WithFilterData, [FromQuery] ParameterPagination parameterPagination,string SiteCode)
        //{
        //    var response = _unitOfWorkService.CivilInstService.GetAllCivils(BaseFilter, WithFilterData, parameterPagination, SiteCode);
        //    return Ok(response);
        //}
        [ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
        [HttpGet("GetAttForAddCivilWithoutLegs")]
        [ProducesResponseType(200, Type = typeof(ObjectInstAtts))]
        public IActionResult GetAttForAddCivilWithoutLegs(int CivilLibraryId, string SiteCode, int? CategoryId)
        {
            var response = _unitOfWorkService.CivilInstService.GetAttForAdd(Helpers.Constants.CivilType.TLIcivilWithoutLeg.ToString(), CivilLibraryId, CategoryId, SiteCode);
            return Ok(response);
        }
        [ServiceFilter(typeof(WorkFlowMiddleware))]
        [HttpPost("AddCivilWithLegs/{SiteCode}")]
        [ProducesResponseType(200, Type = typeof(AddCivilWithLegsViewModel))]
        public IActionResult AddCivilWithLegs([FromBody] AddCivilWithLegsViewModel addCivilWithLeg,[Parameter] string SiteCode,  int? TaskId)
        {
            var ConnectionString = _configuration["ConnectionStrings:ActiveConnection"];
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
  
            if (TryValidateModel(addCivilWithLeg, nameof(AddCivilWithLegsViewModel)))
            {
                var response = _unitOfWorkService.CivilInstService.AddCivilWithLegsInstallation(addCivilWithLeg, Helpers.Constants.CivilType.TLIcivilWithLegs.ToString(), SiteCode, ConnectionString,TaskId, userId);
                return Ok(response);
            }
            else
            {
                var ErrorMessages = from state in ModelState.Values
                                    from error in state.Errors
                                    select error.ErrorMessage;
                return Ok(new Response<AddCivilWithLegsViewModel>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
            }
              

        }
        
        [HttpPost("AddCivilWithoutLegs/{SiteCode}")]
        [ProducesResponseType(200, Type = typeof(AddCivilWithoutLegViewModel))]
        public IActionResult AddCivilWithoutLegs([FromBody] AddCivilWithoutLegViewModel addCivilWithoutLeg, string SiteCode, int? TaskId)
        {
            var ConnectionString = _configuration["ConnectionStrings:ActiveConnection"];
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

            if (TryValidateModel(addCivilWithoutLeg, nameof(AddCivilWithoutLegViewModel)))
            {
                var response = _unitOfWorkService.CivilInstService.AddCivilWithoutLegsInstallation(addCivilWithoutLeg, Helpers.Constants.CivilType.TLIcivilWithoutLeg.ToString(), SiteCode, ConnectionString, TaskId,userId);
                return Ok(response);
            }
            else
            {
                var ErrorMessages = from state in ModelState.Values
                                    from error in state.Errors
                                    select error.ErrorMessage;
                return Ok(new Response<AddCivilWithoutLegViewModel>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
            }
              

        }

        [HttpPost("AddCivilNonSteel/{SiteCode}")]
        [ProducesResponseType(200, Type = typeof(AddCivilNonSteelObject))]
        public IActionResult AddCivilNonSteel([FromBody] AddCivilNonSteelObject addCivilNonSteel, string SiteCode, int? TaskId)
        {
            var ConnectionString = _configuration["ConnectionStrings:ActiveConnection"];
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

            if (TryValidateModel(addCivilNonSteel, nameof(AddCivilNonSteelObject)))
            {
                var response = _unitOfWorkService.CivilInstService.AddCivilNonSteelInstallation(addCivilNonSteel, Helpers.Constants.CivilType.TLIcivilNonSteel.ToString(), SiteCode, ConnectionString, TaskId, userId);
                return Ok(response);
            }
            else
            {
                var ErrorMessages = from state in ModelState.Values
                                    from error in state.Errors
                                    select error.ErrorMessage;
                return Ok(new Response<AddCivilNonSteelObject>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
            }
                
           
        }
        //[ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
        [HttpGet("GetCivilWithLegsById")]
        [ProducesResponseType(200, Type = typeof(ObjectInstAtts))]
        public IActionResult GetCivilWithLegsInstallationById(int CivilId)
        {
            var response = _unitOfWorkService.CivilInstService.GetCivilWithLegsInstallationById(CivilId, Helpers.Constants.CivilType.TLIcivilWithLegs.ToString());
            return Ok(response);
        }
        //[ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
        [HttpGet("GetCivilWithoutLegsInstallationById")]
        [ProducesResponseType(200, Type = typeof(ObjectInstAtts))]
        public IActionResult GetCivilWithoutLegsInstallationById(int CivilId,int CategoryId)
        {
            var response = _unitOfWorkService.CivilInstService.GetCivilWithoutLegsInstallationById(CivilId, Helpers.Constants.CivilType.TLIcivilWithoutLeg.ToString(), CategoryId);
            return Ok(response);
        }
        [ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
        [HttpGet("GetRelationshipBetweenloads")]
        [ProducesResponseType(200, Type = typeof(ObjectInstAtts))]
        public IActionResult GetRelationshipBetweenloads(int loadid, string Loadname,string SiteCode)
        {
            var response = _unitOfWorkService.CivilInstService.GetRelationshipBetweenloads(loadid, Loadname, SiteCode);
            return Ok(response);
        }
        //[ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
        [HttpGet("GetCivilNonSteelById")]
        [ProducesResponseType(200, Type = typeof(ObjectInstAtts))]
        public IActionResult GetCivilNonSteelById(int CivilId)
        {
            var response = _unitOfWorkService.CivilInstService.GetCivilNonSteelInstallationById(CivilId, Helpers.Constants.CivilType.TLIcivilNonSteel.ToString());
            return Ok(response);
        }
        [ServiceFilter(typeof(WorkFlowMiddleware))]
        [HttpPost("EditCivilWithLegs")]
        [ProducesResponseType(200, Type = typeof(EditCivilWithLegsInstallationObject))]
        public async Task<IActionResult> EditCivilWithLegs([FromBody] EditCivilWithLegsInstallationObject CivilWithLeg, int? TaskId)
        {
            var ConnectionString = _configuration["ConnectionStrings:ActiveConnection"];
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
            if (TryValidateModel(CivilWithLeg, nameof(EditCivilWithLegsInstallationObject)))
            {
                var response = await _unitOfWorkService.CivilInstService.EditCivilWithLegsInstallation(CivilWithLeg, Helpers.Constants.CivilType.TLIcivilWithLegs.ToString(), TaskId, userId, ConnectionString);
                return Ok(response);
            }
            else
            {
                var ErrorMessages = from state in ModelState.Values
                                    from error in state.Errors
                                    select error.ErrorMessage;
                return Ok(new Response<CivilWithLegsViewModel>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
            }
        }
        [ServiceFilter(typeof(WorkFlowMiddleware))]
        [HttpPost("EditCivilWithoutLegsInstallation")]
        [ProducesResponseType(200, Type = typeof(EditCivilWithoutLegsInstallationObject))]
        public async Task<IActionResult> EditCivilWithoutLegsInstallation([FromBody] EditCivilWithoutLegsInstallationObject CivilWithoutLeg, int? TaskId)
        {
            var ConnectionString = _configuration["ConnectionStrings:ActiveConnection"];
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
            if (TryValidateModel(CivilWithoutLeg, nameof(EditCivilWithoutLegsInstallationObject)))
            {
                var response = await _unitOfWorkService.CivilInstService.EditCivilWithoutLegsInstallation(CivilWithoutLeg, Helpers.Constants.CivilType.TLIcivilWithoutLeg.ToString(), TaskId, userId, ConnectionString);
                return Ok(response);
            }
            else
            {
                var ErrorMessages = from state in ModelState.Values
                                    from error in state.Errors
                                    select error.ErrorMessage;
                return Ok(new Response<CivilWithoutLegViewModel>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
            }
        }
        [ServiceFilter(typeof(WorkFlowMiddleware))]
        [HttpPost("EditCivilNonSteel")]
        [ProducesResponseType(200, Type = typeof(EditCivilNonSteelInstallationObject))]
        public async Task<IActionResult> EditCivilNonSteel([FromBody] EditCivilNonSteelInstallationObject CivilNonSteel, int? TaskId)
        {
            var ConnectionString = _configuration["ConnectionStrings:ActiveConnection"];
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
            if (TryValidateModel(CivilNonSteel, nameof(EditCivilNonSteelInstallationObject)))
            {
                var response = await _unitOfWorkService.CivilInstService.EditCivilNonSteelInstallation(CivilNonSteel, Helpers.Constants.CivilType.TLIcivilNonSteel.ToString(), TaskId, userId, ConnectionString);
                return Ok(response);
            }
            else
            {
                var ErrorMessages = from state in ModelState.Values
                                    from error in state.Errors
                                    select error.ErrorMessage;
                return Ok(new Response<CivilNonSteelViewModel>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
            }
        }
        [ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
        [HttpPost("GetAllCivilWithLoad")]

        public IActionResult GetAllCivilWithLoad(string SearchName, [FromBody] ParameterPagination parameters)
        {
            var response = _unitOfWorkService.CivilInstService.GetAllCivilLoad(SearchName, parameters);
            return Ok(response);

        }
        [ServiceFilter(typeof(WorkFlowMiddleware))]
        [HttpPost("DismantleCivilWithLegsInstallation")]
        public IActionResult DismantleCivilWithLegsInstallation(string SiteCode, int CivilId, int? TaskId)
        {
            var ConnectionString = _configuration["ConnectionStrings:ActiveConnection"];
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
            var response = _unitOfWorkService.CivilInstService.DismantleCivilWithLegsInstallation(userId, SiteCode, CivilId, TaskId, ConnectionString);
            return Ok(response);

        }
        //[ServiceFilter(typeof(WorkFlowMiddleware))]
        [HttpPost("DismantleCivilWithoutLegsInstallation")]
        public IActionResult DismantleCivilWithoutLegsInstallation(string SiteCode, int CivilId, int? TaskId)
        {
            var ConnectionString = _configuration["ConnectionStrings:ActiveConnection"];
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
            var response = _unitOfWorkService.CivilInstService.DismantleCivilWithoutLegsInstallation(userId, SiteCode, CivilId, TaskId, ConnectionString);
            return Ok(response);

        }
        [ServiceFilter(typeof(WorkFlowMiddleware))]
        [HttpPost("DismantleCivilNonSteelInstallation")]
        public IActionResult DismantleCivilNonSteelInstallation(string SiteCode, int CivilId, int? TaskId)
        {
            var ConnectionString = _configuration["ConnectionStrings:ActiveConnection"];
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
            var response = _unitOfWorkService.CivilInstService.DismantleCivilNonSteelInstallation(userId, SiteCode, CivilId, TaskId, ConnectionString);
            return Ok(response);

        }
        [ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
        [HttpPost("CheckLoadsBeforDismantle")]

        public IActionResult CheckLoadsBeforDismantle(string TableName, int loadId, string SiteCode)
        {
            var response = _unitOfWorkService.CivilInstService.CheckLoadsBeforDismantle(TableName, loadId, SiteCode);
            return Ok(response);

        }
        //[ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
        [HttpGet("GetLibraryAttForInstallations")]

        public IActionResult GetLibraryAttForInstallations(string InstTableName, int? CatId)
        {
            var response = _unitOfWorkService.CivilInstService.GetLibraryAttForInstallations(InstTableName, CatId);
            return Ok(response);

        }
        [ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
        [HttpGet("GetlogicalOperation")]
        [ProducesResponseType(200, Type = typeof(LogicalOperationViewModel))]
        public IActionResult GetlogicalOperation()
        {
            var response = _unitOfWorkService.CivilInstService.GetlogicalOperation();
            return Ok(response);
        }
        //[ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
        [HttpGet("CheckFilterSideArm_LoadsOnCivils")]
        [ProducesResponseType(200, Type = typeof(SideArmAndLoadsOnCivil))]
        public IActionResult CheckFilterSideArm_LoadsOnCivils(int CivilId, string CivilType)
        {
            var response = _unitOfWorkService.CivilInstService.CheckFilterSideArm_LoadsOnCivils(CivilId, CivilType);
            return Ok(response);
        }
        [ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
        [HttpGet("RecalculatSpace")]
        [ProducesResponseType(200, Type = typeof(List<RecalculatSpace>))]
        public IActionResult RecalculatSpace(int CivilId, string CivilType)
        {
            var response = _unitOfWorkService.CivilInstService.RecalculatSpace(CivilId, CivilType);
            return Ok(response);
        }
        [ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
        [HttpGet("CheckLoadsOnSideArm")]
        [ProducesResponseType(200, Type = typeof(LoadsCountOnSideArm))]
        public IActionResult CheckLoadsOnSideArm(int SideArmId)
        {
            var response = _unitOfWorkService.CivilInstService.CheckLoadsOnSideArm(SideArmId);
            return Ok(response);
        }
    }
}