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
   [ServiceFilter(typeof(WorkFlowMiddleware))]
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

        [HttpGet("GetAttForAddCivilWithLegs")]
        [ProducesResponseType(200, Type = typeof(ObjectInstAtts))]
        public IActionResult GetAttForAddCivilWithLegs(int CivilLibraryId, string SiteCode, int? CategoryId)
        {
            var response = _unitOfWorkService.CivilInstService.GetAttForAdd(Helpers.Constants.CivilType.TLIcivilWithLegs.ToString(), CivilLibraryId, CategoryId, SiteCode);
            return Ok(response);
        }

        [HttpPost("GetCivilWithLegsWithEnableAtt")]
        [ProducesResponseType(200, Type = typeof(object))]
        public IActionResult GetCivilWithLegsWithEnableAtt([FromBody] CombineFilters CombineFilters, [FromQuery] SiteBaseFilter BaseFilter, bool WithFilterData, [FromQuery] ParameterPagination parameterPagination)
        {
            var response = _unitOfWorkService.CivilInstService.GetCivilWithLegsWithEnableAtt(BaseFilter, WithFilterData, CombineFilters, parameterPagination);
            return Ok(response);
        }

        [HttpPost("GetLoadsAndSideArmsForCivil")]
        [ProducesResponseType(200, Type = typeof(CivilLoads))]
        public IActionResult GetLoadsAndSideArmsForCivil(int CivilId, string CivilType)
        {
            var response = _unitOfWorkService.CivilInstService.GetLoadsAndSideArmsForCivil(CivilId, CivilType);
            return Ok(response);
        }

        [HttpPost("GetLoadsOnSideArm")]
        [ProducesResponseType(200, Type = typeof(LoadsOnSideArm))]
        public IActionResult GetLoadsOnSideArm(int SideArmId)
        {
            var response = _unitOfWorkService.CivilInstService.GetLoadsOnSideArm(SideArmId);
            return Ok(response);
        }

        [HttpPost("GetCivilWithoutLegWithEnableAtt")]
        [ProducesResponseType(200, Type = typeof(object))]
        public IActionResult GetCivilWithoutLegWithEnableAtt([FromQuery] SiteBaseFilter BaseFilter, bool WithFilterData, [FromBody] CombineFilters CombineFilters, [FromQuery] ParameterPagination parameterPagination, int CategoryId)
        {
            var response = _unitOfWorkService.CivilInstService.GetCivilWithoutLegWithEnableAtt(BaseFilter, WithFilterData, CombineFilters, parameterPagination, CategoryId);
            return Ok(response);
        }
        [HttpPost("GetCivilNonSteelWithEnableAtt")]
        [ProducesResponseType(200, Type = typeof(object))]
        public IActionResult GetCivilNonSteelWithEnableAtt([FromQuery] SiteBaseFilter BaseFilter, bool WithFilterData, [FromBody] CombineFilters CombineFilters, [FromQuery] ParameterPagination parameterPagination)
        {
            string ConnectionString = _configuration["ConnectionStrings:ActiveConnection"];
            var response = _unitOfWorkService.CivilInstService.GetCivilNonSteelWithEnableAtt(BaseFilter, WithFilterData, CombineFilters, parameterPagination,ConnectionString);
            return Ok(response);
        }
        [HttpPost("GetAllCivils")]
        [ProducesResponseType(200, Type = typeof(object))]
        public IActionResult GetAllCivils([FromQuery] SiteBaseFilter BaseFilter, bool WithFilterData, [FromQuery] ParameterPagination parameterPagination)
        {
            var response = _unitOfWorkService.CivilInstService.GetAllCivils(BaseFilter, WithFilterData, parameterPagination);
            return Ok(response);
        }
        [HttpGet("GetAttForAddCivilWithoutLegs")]
        [ProducesResponseType(200, Type = typeof(ObjectInstAtts))]
        public IActionResult GetAttForAddCivilWithoutLegs(int CivilLibraryId, string SiteCode, int? CategoryId)
        {
            var response = _unitOfWorkService.CivilInstService.GetAttForAdd(Helpers.Constants.CivilType.TLIcivilWithoutLeg.ToString(), CivilLibraryId, CategoryId, SiteCode);
            return Ok(response);
        }
        [HttpGet("GetAttForAddCivilNonSteel")]
        [ProducesResponseType(200, Type = typeof(ObjectInstAtts))]
        public IActionResult GetAttForAddCivilNonSteel(int CivilLibraryId, string SiteCode, int? CategoryId)
        {
            var response = _unitOfWorkService.CivilInstService.GetAttForAdd(Helpers.Constants.CivilType.TLIcivilNonSteel.ToString(), CivilLibraryId, CategoryId, SiteCode);
            return Ok(response);
        }
        [HttpPost("AddCivilWithLegs/{SiteCode}")]
        [ProducesResponseType(200, Type = typeof(AddCivilWithLegsViewModel))]
        public IActionResult AddCivilWithLegs([FromBody] AddCivilWithLegsViewModel addCivilWithLeg, string SiteCode, int TaskId)
        {
            var ConnectionString = _configuration["ConnectionStrings:ActiveConnection"];
            if (addCivilWithLeg.TLIcivilSiteDate.ReservedSpace == true)
            {
                var CheckReservedSapce = _unitOfWorkService.SiteService.CheckRentedSpace(SiteCode, addCivilWithLeg.SpaceInstallation);
                if (CheckReservedSapce == true)
                {
                    if (TryValidateModel(addCivilWithLeg, nameof(AddCivilWithLegsViewModel)))
                    {
                        var response = _unitOfWorkService.CivilInstService.AddCivilInstallation(addCivilWithLeg, Helpers.Constants.CivilType.TLIcivilWithLegs.ToString(), SiteCode, ConnectionString,TaskId);
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
            else if (addCivilWithLeg.TLIcivilSiteDate.ReservedSpace == false)
            {
                if (TryValidateModel(addCivilWithLeg, nameof(AddCivilWithLegsViewModel)))
                {
                    var response = _unitOfWorkService.CivilInstService.AddCivilInstallation(addCivilWithLeg, Helpers.Constants.CivilType.TLIcivilWithLegs.ToString(), SiteCode, ConnectionString, TaskId);
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

        [HttpPost("AddCivilWithoutLegs/{SiteCode}")]
        [ProducesResponseType(200, Type = typeof(AddCivilWithoutLegViewModel))]
        public IActionResult AddCivilWithoutLegs([FromBody] AddCivilWithoutLegViewModel addCivilWithoutLeg, string SiteCode, int TaskId )
        {
            var ConnectionString = _configuration["ConnectionStrings:ActiveConnection"];
            if (addCivilWithoutLeg.TLIcivilSiteDate.ReservedSpace == true)
            {
                var CheckReservedSapce = _unitOfWorkService.SiteService.CheckRentedSpace(SiteCode, addCivilWithoutLeg.SpaceInstallation);
                if (CheckReservedSapce == true)
                {
                    if (TryValidateModel(addCivilWithoutLeg, nameof(AddCivilWithoutLegViewModel)))
                    {
                        var response = _unitOfWorkService.CivilInstService.AddCivilInstallation(addCivilWithoutLeg, Helpers.Constants.CivilType.TLIcivilWithoutLeg.ToString(), SiteCode, ConnectionString, TaskId);
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
            }
            else if (addCivilWithoutLeg.TLIcivilSiteDate.ReservedSpace == false)
            {
                if (TryValidateModel(addCivilWithoutLeg, nameof(AddCivilWithoutLegViewModel)))
                {
                    var response = _unitOfWorkService.CivilInstService.AddCivilInstallation(addCivilWithoutLeg, Helpers.Constants.CivilType.TLIcivilWithoutLeg.ToString(), SiteCode, ConnectionString, TaskId);
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
            return Ok(new Response<AddCivilWithoutLegViewModel>(true, null, null, "There is no space on the site", (int)Helpers.Constants.ApiReturnCode.fail));

        }

        [HttpPost("AddCivilNonSteel/{SiteCode}")]
        [ProducesResponseType(200, Type = typeof(AddCivilNonSteelViewModel))]
        public IActionResult AddCivilNonSteel([FromBody] AddCivilNonSteelViewModel addCivilNonSteel, string SiteCode, int TaskId)
        {
            var ConnectionString = _configuration["ConnectionStrings:ActiveConnection"];
            if (addCivilNonSteel.TLIcivilSiteDate.ReservedSpace == true)
            {
                var CheckReservedSapce = _unitOfWorkService.SiteService.CheckRentedSpace(SiteCode, addCivilNonSteel.SpaceInstallation);
                if (CheckReservedSapce == true)
                {
                    if (TryValidateModel(addCivilNonSteel, nameof(AddCivilNonSteelViewModel)))
                    {
                        var response = _unitOfWorkService.CivilInstService.AddCivilInstallation(addCivilNonSteel, Helpers.Constants.CivilType.TLIcivilNonSteel.ToString(), SiteCode, ConnectionString, TaskId);
                        return Ok(response);
                    }
                    else
                    {
                        var ErrorMessages = from state in ModelState.Values
                                            from error in state.Errors
                                            select error.ErrorMessage;
                        return Ok(new Response<AddCivilNonSteelViewModel>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
                    }
                }
            }
            else if (addCivilNonSteel.TLIcivilSiteDate.ReservedSpace == false)
            {
                if (TryValidateModel(addCivilNonSteel, nameof(AddCivilNonSteelViewModel)))
                {
                    var response = _unitOfWorkService.CivilInstService.AddCivilInstallation(addCivilNonSteel, Helpers.Constants.CivilType.TLIcivilNonSteel.ToString(), SiteCode, ConnectionString, TaskId);
                    return Ok(response);
                }
                else
                {
                    var ErrorMessages = from state in ModelState.Values
                                        from error in state.Errors
                                        select error.ErrorMessage;
                    return Ok(new Response<AddCivilNonSteelViewModel>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
                }
            }
            return Ok(new Response<AddCivilNonSteelViewModel>(true, null, null, "There is no space on the site", (int)Helpers.Constants.ApiReturnCode.fail));
        }
        [HttpGet("GetCivilWithLegsById")]
        [ProducesResponseType(200, Type = typeof(ObjectInstAtts))]
        public IActionResult GetCivilWithLegsById(int CivilId)
        {
            var response = _unitOfWorkService.CivilInstService.GetById(CivilId, Helpers.Constants.CivilType.TLIcivilWithLegs.ToString());
            return Ok(response);
        }
        [HttpGet("GetCivilWithoutLegsById")]
        [ProducesResponseType(200, Type = typeof(ObjectInstAtts))]
        public IActionResult GetCivilWithoutLegsById(int CivilId)
        {
            var response = _unitOfWorkService.CivilInstService.GetById(CivilId, Helpers.Constants.CivilType.TLIcivilWithoutLeg.ToString());
            return Ok(response);
        }
        [HttpGet("GetRelationshipBetweenloads")]
        [ProducesResponseType(200, Type = typeof(ObjectInstAtts))]
        public IActionResult GetRelationshipBetweenloads(int loadid, string Loadname)
        {
            var response = _unitOfWorkService.CivilInstService.GetRelationshipBetweenloads(loadid, Loadname);
            return Ok(response);
        }
        [HttpGet("GetCivilNonSteelById")]
        [ProducesResponseType(200, Type = typeof(ObjectInstAtts))]
        public IActionResult GetCivilNonSteelById(int CivilId)
        {
            var response = _unitOfWorkService.CivilInstService.GetById(CivilId, Helpers.Constants.CivilType.TLIcivilNonSteel.ToString());
            return Ok(response);
        }

        [HttpPost("EditCivilWithLegs")]
        [ProducesResponseType(200, Type = typeof(CivilWithLegsViewModel))]
        public async Task<IActionResult> EditCivilWithLegs([FromBody] EditCivilWithLegsViewModel CivilWithLeg, int TaskId)
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

        [HttpPost("EditCivilWithoutLegs")]
        [ProducesResponseType(200, Type = typeof(CivilWithoutLegViewModel))]
        public async Task<IActionResult> EditCivilWithoutLegs([FromBody] EditCivilWithoutLegViewModel CivilWithoutLeg, int TaskId)
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

        [HttpPost("EditCivilNonSteel")]
        [ProducesResponseType(200, Type = typeof(CivilNonSteelViewModel))]
        public async Task<IActionResult> EditCivilNonSteel([FromBody] EditCivilNonSteelViewModel CivilNonSteel, int TaskId)
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


        [HttpPost("DismantleCivil")]

        public IActionResult DismantleCivil(string SiteCode, int CivilId, string CivilName, int TaskId)
        {
            var response = _unitOfWorkService.CivilInstService.DismantleCivil(SiteCode, CivilId, CivilName, TaskId);
            return Ok(response);

        }
        [HttpPost("CheckLoadsBeforDismantle")]

        public IActionResult CheckLoadsBeforDismantle(string TableName, int loadId)
        {
            var response = _unitOfWorkService.CivilInstService.CheckLoadsBeforDismantle(TableName, loadId);
            return Ok(response);

        }

        [HttpGet("GetLibraryAttForInstallations")]

        public IActionResult GetLibraryAttForInstallations(string InstTableName, int? CatId)
        {
            var response = _unitOfWorkService.CivilInstService.GetLibraryAttForInstallations(InstTableName, CatId);
            return Ok(response);

        }
        [HttpGet("GetlogicalOperation")]
        [ProducesResponseType(200, Type = typeof(LogicalOperationViewModel))]
        public IActionResult GetlogicalOperation()
        {
            var response = _unitOfWorkService.CivilInstService.GetlogicalOperation();
            return Ok(response);
        }
        [HttpGet("CheckFilterSideArm_LoadsOnCivils")]
        [ProducesResponseType(200, Type = typeof(SideArmAndLoadsOnCivil))]
        public IActionResult CheckFilterSideArm_LoadsOnCivils(int CivilId, string CivilType)
        {
            var response = _unitOfWorkService.CivilInstService.CheckFilterSideArm_LoadsOnCivils(CivilId, CivilType);
            return Ok(response);
        }
        [HttpGet("RecalculatSpace")]
        [ProducesResponseType(200, Type = typeof(List<RecalculatSpace>))]
        public IActionResult RecalculatSpace(int CivilId, string CivilType)
        {
            var response = _unitOfWorkService.CivilInstService.RecalculatSpace(CivilId, CivilType);
            return Ok(response);
        }
        [HttpGet("CheckLoadsOnSideArm")]
        [ProducesResponseType(200, Type = typeof(LoadsCountOnSideArm))]
        public IActionResult CheckLoadsOnSideArm(int SideArmId)
        {
            var response = _unitOfWorkService.CivilInstService.CheckLoadsOnSideArm(SideArmId);
            return Ok(response);
        }
    }
}