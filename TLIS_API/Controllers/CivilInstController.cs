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
using TLIS_DAL.ViewModels.CivilLoadsDTOs;
using TLIS_DAL.ViewModels.CivilNonSteelDTOs;
using TLIS_DAL.ViewModels.CivilWithLegsDTOs;
using TLIS_DAL.ViewModels.CivilWithoutLegDTOs;
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
       // [ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
        [HttpGet("GetForAddCivilWithLegInstallation")]
        [ProducesResponseType(200, Type = typeof(ObjectInstAtts))]
        public IActionResult GetAttForAddCivilWithLegs(int CivilLibraryId,string SiteCode)
        {
            var response = _unitOfWorkService.CivilInstService.GetForAddCivilWithLegInstallation(Helpers.Constants.CivilType.TLIcivilWithLegs.ToString(), CivilLibraryId, SiteCode);
            return Ok(response);
        }
        [ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
        [HttpGet("GetForAddCivilWithOutLegInstallation_Capsule")]
        [ProducesResponseType(200, Type = typeof(ObjectInstAtts))]
        public IActionResult GetForAddCivilWithOutLegInstallation_Capsule(int CivilLibraryId, string SiteCode)
        {
            var response = _unitOfWorkService.CivilInstService.GetForAddCivilWithOutLegInstallation_Capsule(Helpers.Constants.CivilType.TLIcivilWithLegs.ToString(), CivilLibraryId, SiteCode);
            return Ok(response);
        }
        [ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
        [HttpGet("GetForAddCivilWithOutLegInstallation_Mast")]
        [ProducesResponseType(200, Type = typeof(ObjectInstAtts))]
        public IActionResult GetForAddCivilWithOutLegInstallation_Mast(int CivilLibraryId, string SiteCode)
        {
            var response = _unitOfWorkService.CivilInstService.GetForAddCivilWithOutLegInstallation_Mast(Helpers.Constants.CivilType.TLIcivilWithLegs.ToString(), CivilLibraryId, SiteCode);
            return Ok(response);
        }
        [ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
        [HttpGet("GetForAddCivilWithOutLegInstallation_Monople")]
        [ProducesResponseType(200, Type = typeof(ObjectInstAtts))]
        public IActionResult GetForAddCivilWithOutLegInstallation_Monople(int CivilLibraryId, string SiteCode)
        {
            var response = _unitOfWorkService.CivilInstService.GetForAddCivilWithOutLegInstallation_Monople(Helpers.Constants.CivilType.TLIcivilWithLegs.ToString(), CivilLibraryId, SiteCode);
            return Ok(response);
        }
        [ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
        [HttpGet("GetForAddCiviNonSteelInstallation")]
        [ProducesResponseType(200, Type = typeof(ObjectInstAtts))]
        public IActionResult GetForAddCiviNonSteelInstallation(int CivilLibraryId, string SiteCode)
        {
            var response = _unitOfWorkService.CivilInstService.GetForAddCiviNonSteelInstallation(Helpers.Constants.CivilType.TLIcivilWithLegs.ToString(), CivilLibraryId, SiteCode);
            return Ok(response);
        }
       // [ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
        [HttpPost("GetCivilWithLegsWithEnableAtt")]
        [ProducesResponseType(200, Type = typeof(object))]
        public IActionResult GetCivilWithLegsWithEnableAtt([FromBody] CombineFilters CombineFilters, [FromQuery] SiteBaseFilter BaseFilter, bool WithFilterData, [FromQuery] ParameterPagination parameterPagination)
        {
            string ConnectionString = _configuration["ConnectionStrings:ActiveConnection"];
            var response = _unitOfWorkService.CivilInstService.GetCivilWithLegsWithEnableAtt(BaseFilter, WithFilterData, CombineFilters, parameterPagination, ConnectionString);
            return Ok(response);
        }
        [ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
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
        [ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
        [HttpPost("GetCivilWithoutLegWithEnableAtt")]
        [ProducesResponseType(200, Type = typeof(object))]
        public IActionResult GetCivilWithoutLegWithEnableAtt([FromQuery] SiteBaseFilter BaseFilter, bool WithFilterData, [FromBody] CombineFilters CombineFilters, [FromQuery] ParameterPagination parameterPagination, int CategoryId)
        {
            var response = _unitOfWorkService.CivilInstService.GetCivilWithoutLegWithEnableAtt(BaseFilter, WithFilterData, CombineFilters, parameterPagination, CategoryId);
            return Ok(response);
        }
        [ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
        [HttpPost("GetCivilNonSteelWithEnableAtt")]
        [ProducesResponseType(200, Type = typeof(object))]
        public IActionResult GetCivilNonSteelWithEnableAtt([FromQuery] SiteBaseFilter BaseFilter, bool WithFilterData, [FromBody] CombineFilters CombineFilters, [FromQuery] ParameterPagination parameterPagination)
        {
            string ConnectionString = _configuration["ConnectionStrings:ActiveConnection"];
            var response = _unitOfWorkService.CivilInstService.GetCivilNonSteelWithEnableAtt(BaseFilter, WithFilterData, CombineFilters, parameterPagination,ConnectionString);
            return Ok(response);
        }
        [ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
        [HttpPost("GetAllCivils")]
        [ProducesResponseType(200, Type = typeof(object))]
        public IActionResult GetAllCivils([FromQuery] SiteBaseFilter BaseFilter, bool WithFilterData, [FromQuery] ParameterPagination parameterPagination)
        {
            var response = _unitOfWorkService.CivilInstService.GetAllCivils(BaseFilter, WithFilterData, parameterPagination);
            return Ok(response);
        }
        [ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
        [HttpGet("GetAttForAddCivilWithoutLegs")]
        [ProducesResponseType(200, Type = typeof(ObjectInstAtts))]
        public IActionResult GetAttForAddCivilWithoutLegs(int CivilLibraryId, string SiteCode, int? CategoryId)
        {
            var response = _unitOfWorkService.CivilInstService.GetAttForAdd(Helpers.Constants.CivilType.TLIcivilWithoutLeg.ToString(), CivilLibraryId, CategoryId, SiteCode);
            return Ok(response);
        }
        [ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
        [HttpGet("GetAttForAddCivilNonSteel")]
        [ProducesResponseType(200, Type = typeof(ObjectInstAtts))]
        public IActionResult GetAttForAddCivilNonSteel(int CivilLibraryId, string SiteCode, int? CategoryId)
        {
            var response = _unitOfWorkService.CivilInstService.GetAttForAdd(Helpers.Constants.CivilType.TLIcivilNonSteel.ToString(), CivilLibraryId, CategoryId, SiteCode);
            return Ok(response);
        }
        //[ServiceFilter(typeof(WorkFlowMiddleware))]
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
            if (addCivilWithLeg.civilSiteDate.ReservedSpace == true)
            { 
                var CheckReservedSapce = _unitOfWorkService.SiteService.CheckRentedSpace(SiteCode, addCivilWithLeg.installationAttributes.SpaceInstallation);
                if (CheckReservedSapce == true)
                {
                    if (TryValidateModel(addCivilWithLeg, nameof(AddCivilWithLegsViewModel)))
                    {
                        var response = _unitOfWorkService.CivilInstService.AddCivilInstallation(addCivilWithLeg, Helpers.Constants.CivilType.TLIcivilWithLegs.ToString(), SiteCode, ConnectionString,TaskId, userId);
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
            }
            else if (addCivilWithLeg.civilSiteDate.ReservedSpace == false)
            {
                if (TryValidateModel(addCivilWithLeg, nameof(AddCivilWithLegsViewModel)))
                {
                    var response = _unitOfWorkService.CivilInstService.AddCivilInstallation(addCivilWithLeg, Helpers.Constants.CivilType.TLIcivilWithLegs.ToString(), SiteCode, ConnectionString, TaskId,userId);
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
            return Ok(new Response<AddCivilWithLegsViewModel>(true, null, null, "There is no space on the site", (int)Helpers.Constants.ApiReturnCode.fail));

        }

        //[HttpPost("AddCivilWithoutLegs/{SiteCode}")]
        //[ProducesResponseType(200, Type = typeof(AddCivilWithoutLegViewModel))]
        //public IActionResult AddCivilWithoutLegs([FromBody] AddCivilWithoutLegViewModel addCivilWithoutLeg, string SiteCode, int? TaskId )
        //{
        //    var ConnectionString = _configuration["ConnectionStrings:ActiveConnection"];
        //    if (addCivilWithoutLeg.civilSiteDate.ReservedSpace == true)
        //    {
        //        var CheckReservedSapce = _unitOfWorkService.SiteService.CheckRentedSpace(SiteCode, addCivilWithoutLeg.installationAttributes.SpaceInstallation);
        //        if (CheckReservedSapce == true)
        //        {
        //            if (TryValidateModel(addCivilWithoutLeg, nameof(AddCivilWithoutLegViewModel)))
        //            {
        //                var response = _unitOfWorkService.CivilInstService.AddCivilInstallation(addCivilWithoutLeg, Helpers.Constants.CivilType.TLIcivilWithoutLeg.ToString(), SiteCode, ConnectionString, TaskId);
        //                return Ok(response);
        //            }
        //            else
        //            {
        //                var ErrorMessages = from state in ModelState.Values
        //                                    from error in state.Errors
        //                                    select error.ErrorMessage;
        //                return Ok(new Response<AddCivilWithoutLegViewModel>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
        //            }
        //        }
        //    }
        //    else if (addCivilWithoutLeg.civilSiteDate.ReservedSpace == false)
        //    {
        //        if (TryValidateModel(addCivilWithoutLeg, nameof(AddCivilWithoutLegViewModel)))
        //        {
        //            var response = _unitOfWorkService.CivilInstService.AddCivilInstallation(addCivilWithoutLeg, Helpers.Constants.CivilType.TLIcivilWithoutLeg.ToString(), SiteCode, ConnectionString, TaskId);
        //            return Ok(response);
        //        }
        //        else
        //        {
        //            var ErrorMessages = from state in ModelState.Values
        //                                from error in state.Errors
        //                                select error.ErrorMessage;
        //            return Ok(new Response<AddCivilWithoutLegViewModel>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
        //        }
        //    }
        //    return Ok(new Response<AddCivilWithoutLegViewModel>(true, null, null, "There is no space on the site", (int)Helpers.Constants.ApiReturnCode.fail));

        //}

        //[HttpPost("AddCivilNonSteel/{SiteCode}")]
        //[ProducesResponseType(200, Type = typeof(AddCivilNonSteelViewModel))]
        //public IActionResult AddCivilNonSteel([FromBody] AddCivilNonSteelViewModel addCivilNonSteel, string SiteCode, int? TaskId)
        //{
        //    var ConnectionString = _configuration["ConnectionStrings:ActiveConnection"];
        //    if (addCivilNonSteel.civilSiteDate.ReservedSpace == true)
        //    {
        //        var CheckReservedSapce = _unitOfWorkService.SiteService.CheckRentedSpace(SiteCode, addCivilNonSteel.installationAttributes.SpaceInstallation);
        //        if (CheckReservedSapce == true)
        //        {
        //            if (TryValidateModel(addCivilNonSteel, nameof(AddCivilNonSteelViewModel)))
        //            {
        //                var response = _unitOfWorkService.CivilInstService.AddCivilInstallation(addCivilNonSteel, Helpers.Constants.CivilType.TLIcivilNonSteel.ToString(), SiteCode, ConnectionString, TaskId);
        //                return Ok(response);
        //            }
        //            else
        //            {
        //                var ErrorMessages = from state in ModelState.Values
        //                                    from error in state.Errors
        //                                    select error.ErrorMessage;
        //                return Ok(new Response<AddCivilNonSteelViewModel>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
        //            }
        //        }
        //    }
        //    else if (addCivilNonSteel.civilSiteDate.ReservedSpace == false)
        //    {
        //        if (TryValidateModel(addCivilNonSteel, nameof(AddCivilNonSteelViewModel)))
        //        {
        //            var response = _unitOfWorkService.CivilInstService.AddCivilInstallation(addCivilNonSteel, Helpers.Constants.CivilType.TLIcivilNonSteel.ToString(), SiteCode, ConnectionString, TaskId);
        //            return Ok(response);
        //        }
        //        else
        //        {
        //            var ErrorMessages = from state in ModelState.Values
        //                                from error in state.Errors
        //                                select error.ErrorMessage;
        //            return Ok(new Response<AddCivilNonSteelViewModel>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
        //        }
        //    }
        //    return Ok(new Response<AddCivilNonSteelViewModel>(true, null, null, "There is no space on the site", (int)Helpers.Constants.ApiReturnCode.fail));
        //}
        //s[ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
        [HttpGet("GetCivilWithLegsById")]
        [ProducesResponseType(200, Type = typeof(ObjectInstAtts))]
        public IActionResult GetCivilWithLegsById(int CivilId)
        {
            var response = _unitOfWorkService.CivilInstService.GetById(CivilId, Helpers.Constants.CivilType.TLIcivilWithLegs.ToString());
            return Ok(response);
        }
        [ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
        [HttpGet("GetCivilWithoutLegsById")]
        [ProducesResponseType(200, Type = typeof(ObjectInstAtts))]
        public IActionResult GetCivilWithoutLegsById(int CivilId)
        {
            var response = _unitOfWorkService.CivilInstService.GetById(CivilId, Helpers.Constants.CivilType.TLIcivilWithoutLeg.ToString());
            return Ok(response);
        }
        [ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
        [HttpGet("GetRelationshipBetweenloads")]
        [ProducesResponseType(200, Type = typeof(ObjectInstAtts))]
        public IActionResult GetRelationshipBetweenloads(int loadid, string Loadname)
        {
            var response = _unitOfWorkService.CivilInstService.GetRelationshipBetweenloads(loadid, Loadname);
            return Ok(response);
        }
        [ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
        [HttpGet("GetCivilNonSteelById")]
        [ProducesResponseType(200, Type = typeof(ObjectInstAtts))]
        public IActionResult GetCivilNonSteelById(int CivilId)
        {
            var response = _unitOfWorkService.CivilInstService.GetById(CivilId, Helpers.Constants.CivilType.TLIcivilNonSteel.ToString());
            return Ok(response);
        }
        [ServiceFilter(typeof(WorkFlowMiddleware))]
        [HttpPost("EditCivilWithLegs")]
        [ProducesResponseType(200, Type = typeof(CivilWithLegsViewModel))]
        public async Task<IActionResult> EditCivilWithLegs([FromBody] EditCivilWithLegsViewModel CivilWithLeg, int? TaskId)
        {
            if (TryValidateModel(CivilWithLeg, nameof(EditCivilWithLegsViewModel)))
            {
                var response = await _unitOfWorkService.CivilInstService.EditCivilInstallation(CivilWithLeg, Helpers.Constants.CivilType.TLIcivilWithLegs.ToString(), TaskId);
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
        [HttpPost("EditCivilWithoutLegs")]
        [ProducesResponseType(200, Type = typeof(CivilWithoutLegViewModel))]
        public async Task<IActionResult> EditCivilWithoutLegs([FromBody] EditCivilWithoutLegViewModel CivilWithoutLeg, int? TaskId)
        {
            if (TryValidateModel(CivilWithoutLeg, nameof(EditCivilWithoutLegViewModel)))
            {
                var response = await _unitOfWorkService.CivilInstService.EditCivilInstallation(CivilWithoutLeg, Helpers.Constants.CivilType.TLIcivilWithoutLeg.ToString(), TaskId);
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
        [ProducesResponseType(200, Type = typeof(CivilNonSteelViewModel))]
        public async Task<IActionResult> EditCivilNonSteel([FromBody] EditCivilNonSteelViewModel CivilNonSteel, int? TaskId)
        {
            if (TryValidateModel(CivilNonSteel, nameof(EditCivilNonSteelViewModel)))
            {
                var response = await _unitOfWorkService.CivilInstService.EditCivilInstallation(CivilNonSteel, Helpers.Constants.CivilType.TLIcivilNonSteel.ToString(), TaskId);
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

        //[HttpPost("DismantleCivil")]

        //public IActionResult DismantleCivil([FromBody] DismantleBinding dis)
        //{
        //    var response = _unitOfWorkService.CivilInstService.CivilDismantle(dis);
        //    return Ok(response);

        //}

        [ServiceFilter(typeof(WorkFlowMiddleware))]
        [HttpPost("DismantleCivil")]

        public IActionResult DismantleCivil(string SiteCode, int CivilId, string CivilName, int? TaskId)
        {
            var response = _unitOfWorkService.CivilInstService.DismantleCivil(SiteCode, CivilId, CivilName, TaskId);
            return Ok(response);

        }
        [ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
        [HttpPost("CheckLoadsBeforDismantle")]

        public IActionResult CheckLoadsBeforDismantle(string TableName, int loadId)
        {
            var response = _unitOfWorkService.CivilInstService.CheckLoadsBeforDismantle(TableName, loadId);
            return Ok(response);

        }
        [ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
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
        [ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
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