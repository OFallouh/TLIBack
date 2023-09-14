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
using TLIS_DAL.ViewModels.CabinetDTOs;
using TLIS_DAL.ViewModels.CabinetPowerLibraryDTOs;
using TLIS_DAL.ViewModels.CabinetTelecomLibraryDTOs;
using TLIS_DAL.ViewModels.CivilWithLegsDTOs;
using TLIS_DAL.ViewModels.GeneratorDTOs;
using TLIS_DAL.ViewModels.GeneratorLibraryDTOs;
using TLIS_DAL.ViewModels.SolarDTOs;
using TLIS_DAL.ViewModels.SolarLibraryDTOs;
using TLIS_Service.Helpers;
using TLIS_Service.ServiceBase;

namespace TLIS_API.Controllers
{
    [ServiceFilter(typeof(LogFilterAttribute))]
    [Route("api/[controller]")]
    [ApiController]
    public class OtherInventoryInstController : ControllerBase
    {
        private readonly IUnitOfWorkService _unitOfWorkService;
        private readonly IConfiguration _configuration;
        public OtherInventoryInstController(IUnitOfWorkService unitOfWorkService, IConfiguration configuration)
        {
            _unitOfWorkService = unitOfWorkService;
            _configuration = configuration;
        }
        [HttpGet("GetAttForAddCabinet")]
        [ProducesResponseType(200, Type = typeof(ObjectInstAtts))]
        public IActionResult GetAttForAddCabinet(string CabinetLibraryType, int OtherInventoryId, string SiteCode)
        {
            var response = _unitOfWorkService.OtherInventoryInstService.GetAttForAdd(Helpers.Constants.OtherInventoryType.TLIcabinet.ToString(), CabinetLibraryType, OtherInventoryId, SiteCode);
            return Ok(response);
        }
        [HttpGet("GetAttForAddSolar")]
        [ProducesResponseType(200, Type = typeof(ObjectInstAtts))]
        public IActionResult GetAttForAddSolar(string CabinetLibraryType, int OtherInventoryId, string SiteCode)
        {
            var response = _unitOfWorkService.OtherInventoryInstService.GetAttForAdd(Helpers.Constants.OtherInventoryType.TLIsolar.ToString(), CabinetLibraryType, OtherInventoryId, SiteCode);
            return Ok(response);
        }
        [HttpGet("GetAttForAddGenerator")]
        [ProducesResponseType(200, Type = typeof(ObjectInstAtts))]
        public IActionResult GetAttForAddGenerator(string CabinetLibraryType, int OtherInventoryId, string SiteCode)
        {
            var response = _unitOfWorkService.OtherInventoryInstService.GetAttForAdd(Helpers.Constants.OtherInventoryType.TLIgenerator.ToString(), CabinetLibraryType, OtherInventoryId, SiteCode);
            return Ok(response);
        }
        [HttpPost("AddCabinet")]
        [ProducesResponseType(200, Type = typeof(AddCabinetViewModel))]
        public IActionResult AddCabinet([FromBody] AddCabinetViewModel addCabinetViewModel, string SiteCode)
        {
            var ConnectionString = _configuration["ConnectionStrings:ActiveConnection"];
            if (addCabinetViewModel.TLIotherInSite.ReservedSpace == true)
            {
                var CheckReservedSapce = _unitOfWorkService.SiteService.CheckRentedSpace(SiteCode, addCabinetViewModel.SpaceInstallation);
                if (CheckReservedSapce == true)
                {
                    if (TryValidateModel(addCabinetViewModel, nameof(AddCabinetViewModel)))
                    {
                        var response = _unitOfWorkService.OtherInventoryInstService.AddOtherInventoryInstallation(addCabinetViewModel, Helpers.Constants.OtherInventoryType.TLIcabinet.ToString(), SiteCode, ConnectionString);
                        return Ok(response);
                    }
                    else
                    {
                        var ErrorMessages = from state in ModelState.Values
                                            from error in state.Errors
                                            select error.ErrorMessage;
                        return Ok(new Response<AddCabinetViewModel>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
                    }
                }
            }
            else if (addCabinetViewModel.TLIotherInSite.ReservedSpace == false)
            {
                if (TryValidateModel(addCabinetViewModel, nameof(AddCabinetViewModel)))
                {
                    var response = _unitOfWorkService.OtherInventoryInstService.AddOtherInventoryInstallation(addCabinetViewModel, Helpers.Constants.OtherInventoryType.TLIcabinet.ToString(), SiteCode, ConnectionString);
                    return Ok(response);
                }
                else
                {
                    var ErrorMessages = from state in ModelState.Values
                                        from error in state.Errors
                                        select error.ErrorMessage;
                    return Ok(new Response<AddCabinetViewModel>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
                }
            }
            return Ok(new Response<AddCabinetViewModel>(true, null, null, "There is no space on the site", (int)Helpers.Constants.ApiReturnCode.fail));

        }
        [HttpPost("AddSolar")]
        [ProducesResponseType(200, Type = typeof(AddSolarViewModel))]
        public IActionResult AddSolar([FromBody] AddSolarViewModel addSolarViewModel, string SiteCode)
        {
            var ConnectionString = _configuration["ConnectionStrings:ActiveConnection"];
            if (addSolarViewModel.TLIotherInSite.ReservedSpace == true)
            {
                var CheckReservedSapce = _unitOfWorkService.SiteService.CheckRentedSpace(SiteCode, addSolarViewModel.SpaceInstallation);
                if (CheckReservedSapce == true)
                {
                    if (TryValidateModel(addSolarViewModel, nameof(AddSolarViewModel)))
                    {
                        var response = _unitOfWorkService.OtherInventoryInstService.AddOtherInventoryInstallation(addSolarViewModel, Helpers.Constants.OtherInventoryType.TLIsolar.ToString(), SiteCode, ConnectionString);
                        return Ok(response);
                    }
                    else
                    {
                        var ErrorMessages = from state in ModelState.Values
                                            from error in state.Errors
                                            select error.ErrorMessage;
                        return Ok(new Response<AddSolarViewModel>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
                    }
                }
            }
            else if (addSolarViewModel.TLIotherInSite.ReservedSpace == false)
            {
                if (TryValidateModel(addSolarViewModel, nameof(AddSolarViewModel)))
                {
                    var response = _unitOfWorkService.OtherInventoryInstService.AddOtherInventoryInstallation(addSolarViewModel, Helpers.Constants.OtherInventoryType.TLIsolar.ToString(), SiteCode, ConnectionString);
                    return Ok(response);
                }
                else
                {
                    var ErrorMessages = from state in ModelState.Values
                                        from error in state.Errors
                                        select error.ErrorMessage;
                    return Ok(new Response<AddSolarViewModel>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
                }
            }
            return Ok(new Response<AddSolarViewModel>(true, null, null, "There is no space on the site", (int)Helpers.Constants.ApiReturnCode.fail));

        }
        [HttpPost("AddGenerator")]
        [ProducesResponseType(200, Type = typeof(AddGeneratorViewModel))]
        public IActionResult AddGenerator([FromBody] AddGeneratorViewModel addGeneratorViewModel, string SiteCode)
        {
            var ConnectionString = _configuration["ConnectionStrings:ActiveConnection"];
            if (addGeneratorViewModel.TLIotherInSite.ReservedSpace == true)
            {
                var CheckReservedSapce = _unitOfWorkService.SiteService.CheckRentedSpace(SiteCode, addGeneratorViewModel.SpaceInstallation);
                if (CheckReservedSapce == true)
                {
                    if (TryValidateModel(addGeneratorViewModel, nameof(AddGeneratorViewModel)))
                    {
                        var response = _unitOfWorkService.OtherInventoryInstService.AddOtherInventoryInstallation(addGeneratorViewModel, Helpers.Constants.OtherInventoryType.TLIgenerator.ToString(), SiteCode, ConnectionString);
                        return Ok(response);
                    }
                    else
                    {
                        var ErrorMessages = from state in ModelState.Values
                                            from error in state.Errors
                                            select error.ErrorMessage;
                        return Ok(new Response<AddGeneratorViewModel>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
                    }
                }
            }
            else if (addGeneratorViewModel.TLIotherInSite.ReservedSpace == false)
            {
                if (TryValidateModel(addGeneratorViewModel, nameof(AddGeneratorViewModel)))
                {
                    var response = _unitOfWorkService.OtherInventoryInstService.AddOtherInventoryInstallation(addGeneratorViewModel, Helpers.Constants.OtherInventoryType.TLIgenerator.ToString(), SiteCode, ConnectionString);
                    return Ok(response);
                }
                else
                {
                    var ErrorMessages = from state in ModelState.Values
                                        from error in state.Errors
                                        select error.ErrorMessage;
                    return Ok(new Response<AddGeneratorViewModel>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
                }
            }
            return Ok(new Response<AddGeneratorViewModel>(true, null, null, "There is no space on the site", (int)Helpers.Constants.ApiReturnCode.fail));

        }

        [HttpGet("GetCabinetById")]
        [ProducesResponseType(200, Type = typeof(ObjectInstAtts))]
        public IActionResult GetCabinetById(int OtherInventoryInstId)
        {
            var response = _unitOfWorkService.OtherInventoryInstService.GetById(OtherInventoryInstId, Helpers.Constants.OtherInventoryType.TLIcabinet.ToString());
            return Ok(response);
        }
        [HttpGet("GetSolarById")]
        [ProducesResponseType(200, Type = typeof(ObjectInstAtts))]
        public IActionResult GetSolarById(int OtherInventoryInstId)
        {
            var response = _unitOfWorkService.OtherInventoryInstService.GetById(OtherInventoryInstId, Helpers.Constants.OtherInventoryType.TLIsolar.ToString());
            return Ok(response);
        }
        [HttpGet("GetGeneratorById")]
        [ProducesResponseType(200, Type = typeof(ObjectInstAtts))]
        public IActionResult GetGeneratorById(int OtherInventoryInstId)
        {
            var response = _unitOfWorkService.OtherInventoryInstService.GetById(OtherInventoryInstId, Helpers.Constants.OtherInventoryType.TLIgenerator.ToString());
            return Ok(response);
        }

        [HttpPost("EditCabinet")]
        [ProducesResponseType(200, Type = typeof(EditCabinetViewModel))]
        public async Task<IActionResult> EditCabinet([FromBody] EditCabinetViewModel editCabinetViewModel)
        {
            if (TryValidateModel(editCabinetViewModel, nameof(EditCabinetViewModel)))
            {
                var response = await _unitOfWorkService.OtherInventoryInstService.EditOtherInventoryInstallation(editCabinetViewModel, Helpers.Constants.OtherInventoryType.TLIcabinet.ToString());
                return Ok(response);
            }
            else
            {
                var ErrorMessages = from state in ModelState.Values
                                    from error in state.Errors
                                    select error.ErrorMessage;
                return Ok(new Response<EditCabinetViewModel>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
            }
        }

        [HttpPost("EditSolar")]
        [ProducesResponseType(200, Type = typeof(EditSolarViewModel))]
        public async Task<IActionResult> EditSolar([FromBody] EditSolarViewModel editSolarViewModel)
        {
            if (TryValidateModel(editSolarViewModel, nameof(EditSolarViewModel)))
            {
                var response = await _unitOfWorkService.OtherInventoryInstService.EditOtherInventoryInstallation(editSolarViewModel, Helpers.Constants.OtherInventoryType.TLIsolar.ToString());
                return Ok(response);
            }
            else
            {
                var ErrorMessages = from state in ModelState.Values
                                    from error in state.Errors
                                    select error.ErrorMessage;
                return Ok(new Response<EditSolarViewModel>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
            }
        }

        [HttpPost("EditGenerator")]
        [ProducesResponseType(200, Type = typeof(EditGeneratorViewModel))]
        public async Task<IActionResult> EditCivilNonSteel([FromBody] EditGeneratorViewModel editGeneratorViewModel)
        {
            if (TryValidateModel(editGeneratorViewModel, nameof(EditGeneratorViewModel)))
            {
                var response = await _unitOfWorkService.OtherInventoryInstService.EditOtherInventoryInstallation(editGeneratorViewModel, Helpers.Constants.OtherInventoryType.TLIgenerator.ToString());
                return Ok(response);
            }
            else
            {
                var ErrorMessages = from state in ModelState.Values
                                    from error in state.Errors
                                    select error.ErrorMessage;
                return Ok(new Response<EditGeneratorViewModel>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
            }
        }
        [HttpGet("DismantleOtherInventory")]

        public IActionResult DismantleOtherInventory(string SiteCode, int OtherInventoryId, string OtherInventoryName)
        {
            var response = _unitOfWorkService.OtherInventoryInstService.DismantleOtherInventory(SiteCode, OtherInventoryId , OtherInventoryName);
            return Ok(response);

        }
        [HttpPost("GetCabinetBySiteWithEnabledAtt")]
        [ProducesResponseType(200, Type = typeof(object))]
        public IActionResult GetCabinetBySiteWithEnabledAtt([FromBody] CombineFilters CombineFilters, [FromQuery] SiteBaseFilter BaseFilter, bool WithFilterData, [FromQuery] ParameterPagination parameterPagination, string LibraryType)
        {
            var response = _unitOfWorkService.OtherInventoryInstService.GetCabinetBySiteWithEnabledAtt(BaseFilter, WithFilterData, CombineFilters, parameterPagination, LibraryType);
            return Ok(response);
        }
        [HttpPost("GetSolarBySiteWithEnabledAtt")]
        [ProducesResponseType(200, Type = typeof(object))]
        public IActionResult GetSolarBySiteWithEnabledAtt([FromBody] CombineFilters CombineFilters, [FromQuery] SiteBaseFilter BaseFilter, bool WithFilterData, [FromQuery] ParameterPagination parameterPagination)
        {
            var response = _unitOfWorkService.OtherInventoryInstService.GetSolarBySiteWithEnabledAtt(BaseFilter, WithFilterData, CombineFilters, parameterPagination);
            return Ok(response);
        }
        [HttpPost("GetGeneratorBySiteWithEnabledAtt")]
        [ProducesResponseType(200, Type = typeof(object))]
        public IActionResult GetGeneratorBySiteWithEnabledAtt([FromBody] CombineFilters CombineFilters, [FromQuery] SiteBaseFilter BaseFilter, bool WithFilterData, [FromQuery] ParameterPagination parameterPagination)
        {
            var response = _unitOfWorkService.OtherInventoryInstService.GetGeneratorBySiteWithEnabledAtt(BaseFilter, WithFilterData, CombineFilters, parameterPagination);
            return Ok(response);
        }
    }
}