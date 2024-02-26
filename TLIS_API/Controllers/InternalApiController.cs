using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using TLIS_API.Middleware.ActionFilters;
using TLIS_API.Middleware.WorkFlow;
using TLIS_DAL.Helper;
using TLIS_DAL.Helper.Filters;
using TLIS_DAL.Helpers;
using TLIS_DAL.ViewModelBase;
using TLIS_DAL.ViewModels.AllCivilInstDTOs;
using TLIS_DAL.ViewModels.CabinetDTOs;
using TLIS_DAL.ViewModels.CabinetPowerLibraryDTOs;
using TLIS_DAL.ViewModels.CabinetTelecomLibraryDTOs;
using TLIS_DAL.ViewModels.CivilLoadsDTOs;
using TLIS_DAL.ViewModels.CivilNonSteelDTOs;
using TLIS_DAL.ViewModels.CivilWithLegDTOs;
using TLIS_DAL.ViewModels.CivilWithLegsDTOs;
using TLIS_DAL.ViewModels.CivilWithoutLegDTOs;
using TLIS_DAL.ViewModels.DependencyDTOs;
using TLIS_DAL.ViewModels.DynamicAttDTOs;
using TLIS_DAL.ViewModels.GeneratorDTOs;
using TLIS_DAL.ViewModels.GeneratorLibraryDTOs;
using TLIS_DAL.ViewModels.LoadPartDTOs;
using TLIS_DAL.ViewModels.MW_BUDTOs;
using TLIS_DAL.ViewModels.MW_BULibraryDTOs;
using TLIS_DAL.ViewModels.MW_DishDTOs;
using TLIS_DAL.ViewModels.MW_DishLbraryDTOs;
using TLIS_DAL.ViewModels.MW_ODUDTOs;
using TLIS_DAL.ViewModels.Mw_OtherDTOs;
using TLIS_DAL.ViewModels.MW_OtherLibraryDTOs;
using TLIS_DAL.ViewModels.MW_RFUDTOs;
using TLIS_DAL.ViewModels.RadioAntennaDTOs;
using TLIS_DAL.ViewModels.RadioAntennaLibraryDTOs;
using TLIS_DAL.ViewModels.RadioOtherDTOs;
using TLIS_DAL.ViewModels.RadioOtherLibraryDTOs;
using TLIS_DAL.ViewModels.RadioRRUDTOs;
using TLIS_DAL.ViewModels.RadioRRULibraryDTOs;
using TLIS_DAL.ViewModels.SideArmDTOs;
using TLIS_DAL.ViewModels.SideArmLibraryDTOs;
using TLIS_DAL.ViewModels.SiteDTOs;
using TLIS_DAL.ViewModels.SolarDTOs;
using TLIS_DAL.ViewModels.SolarLibraryDTOs;
using TLIS_DAL.ViewModels.TablesNamesDTOs;
using TLIS_Service.Helpers;
using TLIS_Service.ServiceBase;
using static TLIS_API.Helpers.Constants;

namespace TLIS_API.Controllers
{
    [ServiceFilter(typeof(WorkFlowMiddleware))]
    [ServiceFilter(typeof(ExternalSystemFilter))]
    [Route("api/[controller]")]
    [ApiController]
    public class InternalApiController : ControllerBase
    {
        private IUnitOfWorkService _unitOfWorkService;
        private readonly IConfiguration _configuration;
        public InternalApiController(IUnitOfWorkService unitOfWorkService, IConfiguration configuration)
        {
            _unitOfWorkService = unitOfWorkService;
            _configuration = configuration;
        }
        [HttpPost("GetCivilsInstalledonSite")]
        [ProducesResponseType(200, Type = typeof(AllCivilInstallationViewModel))]
        public IActionResult GetCivilsBySiteCode(string SiteCode, CombineFilters CombineFilters, bool WithFilterData, [FromQuery] ParameterPagination parameterPagination)
        {
            var response = _unitOfWorkService.InternalApiService.GetCivilsBySiteCode(SiteCode, CombineFilters, WithFilterData, parameterPagination);
            return Ok(response);
        }
        [HttpPost("GetSideArmsInstalledonCivil")]
        [ProducesResponseType(200, Type = typeof(ObjectInstAttForSideArm))]
        public IActionResult GetSideArmsInstalledonCivil([Required] string SiteCode, string CivilType, string CivilName, int? LegId, float? MinAzimuth, float? MaxAzimuth, float? MinHeightBase, float? MaxHeightBase)
        {
            var response = _unitOfWorkService.InternalApiService.GetSideArmsBySiteCode(SiteCode, CivilType, CivilName, LegId, MinAzimuth, MaxAzimuth, MinHeightBase, MaxHeightBase);
            return Ok(response);
        }
        [HttpPost("GetLibraryforSpecificType")]
        [ProducesResponseType(200, Type = typeof(Response<ReturnWithFilters<object>>))]
        public IActionResult GetLibraryforSpecificType(string TableNameLibrary, int CategoryId, [FromBody] CombineFilters CombineFilters, bool WithFilterData, [FromQuery] ParameterPagination parameterPagination)
        {
            var response = _unitOfWorkService.InternalApiService.GetLibraryforSpecificType(TableNameLibrary, CategoryId, CombineFilters, WithFilterData, parameterPagination);
            return Ok(response);
        }
        [HttpPost("GetAllSitesDetails")]
        [ProducesResponseType(200, Type = typeof(List<SiteViewModel>))]
        public IActionResult GetAllSitesDetails([FromQueryAttribute] ParameterPagination parameterPagination, [FromBody] List<FilterObjectList> filters)
        {
            var response = _unitOfWorkService.InternalApiService.GetAllSitesDetails(parameterPagination, filters);
            return Ok(response);
        }
        [HttpGet("GetAllLoadonSitebyPartandType")]
        [ProducesResponseType(200, Type = typeof(Response<LoadsDto>))]
        public IActionResult GetAllLoadonSitebyPartandType([Required] String SiteCode, string PartName, string TypeName)
        {
            var response = _unitOfWorkService.InternalApiService.GetAllLoadonSitebyPartandType(SiteCode, PartName, TypeName);
            return Ok(response);

        }
        [HttpPost("GetAlOtherInventoryonSitebyType")]
        [ProducesResponseType(200, Type = typeof(Response<ReturnWithFilters<object>>))]
        public IActionResult GetAlOtherInventoryonSitebyType([Required] string OtherInventoryTypeName, [FromQuery] string SiteCode, bool WithFilterData, [FromBody] CombineFilters CombineFilters, [FromQuery] ParameterPagination parameterPagination, string LibraryType)
        {
            var response = _unitOfWorkService.InternalApiService.GetAlOtherInventoryonSitebyType(OtherInventoryTypeName, SiteCode, WithFilterData, CombineFilters, parameterPagination, LibraryType);
            return Ok(response);
        }
        [HttpGet("GetAllItemsonSite ")]
        [ProducesResponseType(200, Type = typeof(Response<List<ListOfCivilLoads>>))]
        public IActionResult GetAllItemsonSite(string SiteCode)
        {
            var response = _unitOfWorkService.InternalApiService.GetAllItemsonSite(SiteCode);
            return Ok(response);
        }
        [HttpPost("GetConfigurationTablesInstallation ")]
        [ProducesResponseType(200, Type = typeof(Response<ReturnWithFilters<object>>))]
        public IActionResult GetConfigurationTablesInstallation([FromQuery] string siteCode, [Required] string TableNameInstallation, int CategoryId, bool WithFilterData, [FromBody] CombineFilters CombineFilters, [FromQuery] ParameterPagination parameterPagination, string LibraryType)
        {
            var response = _unitOfWorkService.InternalApiService.GetConfigurationTables( siteCode, TableNameInstallation, CategoryId, WithFilterData, CombineFilters, parameterPagination, LibraryType);
            return Ok(response);

        }
        [HttpPost("GetConfigurationAttributes")]
        [ProducesResponseType(200, Type = typeof(Response<List<BassAttViewModel>>))]
        public IActionResult GetConfigurationAttributes(string TableName, bool IsDynamic, int CategoryId)
        {
            var response = _unitOfWorkService.InternalApiService.GetConfigurationAttributes(TableName, IsDynamic, CategoryId);
            return Ok(response);

        }
        [HttpPost("AddDynamicAttLibrary")]
        [ProducesResponseType(200, Type = typeof(AddDynamicAttViewModel))]
        public IActionResult AddDynamicAttLibrary([FromBody] AddDependencyViewModel addDependencyView)
        {
            if (ModelState.IsValid)
            {
                var ConnectionString = _configuration["ConnectionStrings:ActiveConnection"];
                var responceResult = _unitOfWorkService.InternalApiService.AddDynamicAtts(addDependencyView, ConnectionString);
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
        [HttpPost("AddDynamicAttInstallation")]
        [ProducesResponseType(200, Type = typeof(List<AddDependencyInstViewModel>))]
        public IActionResult AddDynamicAttInstallation([FromBody] AddDependencyInstViewModel addDependencyInstViewModel)
        {
            if (ModelState.IsValid)
            {
                var ConnectionString = _configuration["ConnectionStrings:ActiveConnection"];
                var responceResult = _unitOfWorkService.InternalApiService.AddDynamicAttInst(addDependencyInstViewModel, ConnectionString);
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
        [HttpPost("EditDynamicAttLibraryAndInstallation")]
        [ProducesResponseType(200, Type = typeof(DynamicAttViewModel))]
        public async Task<IActionResult> EditDynamicAttLibraryAndInstallation([FromBody] EditDynamicAttViewModel dynamicAttViewModel)
        {
            var response = await _unitOfWorkService.InternalApiService.Edit(dynamicAttViewModel);
            return Ok(response);
        }
        [HttpPost("AddRadioRRULibrary")]
        [ProducesResponseType(200, Type = typeof(AddRadioRRULibraryViewModel))]
        public IActionResult AddRadioRRULibrary(AddRadioRRULibraryViewModel addRadioRRU)
        {
            if (TryValidateModel(addRadioRRU, nameof(AddRadioRRULibraryViewModel)))
            {
                var ConnectionString = _configuration["ConnectionStrings:ActiveConnection"];
                var response = _unitOfWorkService.InternalApiService.AddRadioLibrary(Helpers.Constants.LoadSubType.TLIradioRRULibrary.ToString(), addRadioRRU, ConnectionString);
                return Ok(response);
            }
            else
            {
                var ErrorMessages = from state in ModelState.Values
                                    from error in state.Errors
                                    select error.ErrorMessage;
                return Ok(new Response<AddRadioRRULibraryViewModel>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
            }
        }
        [HttpPost("AddRadioOtherLibrary")]
        [ProducesResponseType(200, Type = typeof(AddRadioOtherLibraryViewModel))]
        public IActionResult AddRadioOtherLibrary(AddRadioOtherLibraryViewModel addRadioOther)
        {
            if (TryValidateModel(addRadioOther, nameof(AddRadioOtherLibraryViewModel)))
            {
                var ConnectionString = _configuration["ConnectionStrings:ActiveConnection"];
                var response = _unitOfWorkService.InternalApiService.AddRadioLibrary(Helpers.Constants.LoadSubType.TLIradioOtherLibrary.ToString(), addRadioOther, ConnectionString);
                return Ok(response);
            }
            else
            {
                var ErrorMessages = from state in ModelState.Values
                                    from error in state.Errors
                                    select error.ErrorMessage;
                return Ok(new Response<AddRadioOtherLibraryViewModel>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
            }
        }
        [HttpPost("AddRadioAntennaLibrary")]
        [ProducesResponseType(200, Type = typeof(AddRadioAntennaLibraryViewModel))]
        public IActionResult AddRadioAntennaLibrary(AddRadioAntennaLibraryViewModel addRadioAntenna)
        {
            if (TryValidateModel(addRadioAntenna, nameof(AddRadioAntennaLibraryViewModel)))
            {
                var ConnectionString = _configuration["ConnectionStrings:ActiveConnection"];
                var response = _unitOfWorkService.InternalApiService.AddRadioLibrary(Helpers.Constants.LoadSubType.TLIradioAntennaLibrary.ToString(), addRadioAntenna, ConnectionString);
                return Ok(response);
            }
            else
            {
                var ErrorMessages = from state in ModelState.Values
                                    from error in state.Errors
                                    select error.ErrorMessage;
                return Ok(new Response<AddRadioAntennaLibraryViewModel>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
            }
        }
        [HttpGet("GetForAddRadioLibrary")]
        [ProducesResponseType(200, Type = typeof(List<AllAtributes>))]
        public IActionResult GetForAddLibrary(string TableName)
        {
            var response = _unitOfWorkService.InternalApiService.GetForAddRadioLibrary(TableName);
            return Ok(response);
        }
        [HttpGet("GetAttForAddRadioInstallation")]
        [ProducesResponseType(200, Type = typeof(ObjectAttributeInst))]
        public IActionResult GetAttForAdd(string RadioInstType, int LibId, string SiteCode)
        {
            var response = _unitOfWorkService.InternalApiService.GetAttForAddRadioInstallation(RadioInstType, LibId, SiteCode);
            return Ok(response);
        }
        [HttpPost("AddRadioAntennaInstallation")]
        [ProducesResponseType(200, Type = typeof(AddRadioAntennaIntegration))]
        public IActionResult AddRadioAntennaInstallation([FromBody] AddRadioAntennaIntegration addRadioAntenna, string SiteCode, int TaskId)
        {

            try
            {

                if (addRadioAntenna.TLIcivilLoads.sideArmId == 0)
                    addRadioAntenna.TLIcivilLoads.sideArmId = null;
                var ConnectionString = _configuration["ConnectionStrings:ActiveConnection"];
                if (TryValidateModel(addRadioAntenna, nameof(AddRadioAntennaIntegration)))
                {
                    var response = _unitOfWorkService.InternalApiService.AddRadioInstallation(addRadioAntenna, Helpers.Constants.LoadSubType.TLIradioAntenna.ToString(), SiteCode, ConnectionString, TaskId);

                    return Ok(response);
                }
                else
                {
                    var ErrorMessages = from state in ModelState.Values
                                        from error in state.Errors
                                        select error.ErrorMessage;
                    return Ok(new Response<AddRadioAntennaIntegration>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost("AddRadioRRUInstallation")]
        [ProducesResponseType(200, Type = typeof(AddRadioRRUIntegration))]
        public IActionResult AddRadioRRUInstallation([FromBody] AddRadioRRUIntegration addRadioRRU, string SiteCode,int TaskId)
        {
            if (addRadioRRU.TLIcivilLoads.sideArmId == 0)
                addRadioRRU.TLIcivilLoads.sideArmId = null;
            var ConnectionString = _configuration["ConnectionStrings:ActiveConnection"];
            if (TryValidateModel(addRadioRRU, nameof(AddRadioRRUIntegration)))
            {
                var response = _unitOfWorkService.InternalApiService.AddRadioInstallation(addRadioRRU, Helpers.Constants.LoadSubType.TLIradioRRU.ToString(), SiteCode, ConnectionString, TaskId);
                return Ok(response);
            }
            else
            {
                var ErrorMessages = from state in ModelState.Values
                                    from error in state.Errors
                                    select error.ErrorMessage;
                return Ok(new Response<AddRadioRRUIntegration>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
            }
        }
        [HttpPost("AddRadioOtherInstallation")]
        [ProducesResponseType(200, Type = typeof(AddRadioOtherIntegration))]
        public IActionResult AddRadioOtherInstallation([FromBody] AddRadioOtherIntegration addRadioOther, string SiteCode, int TaskId)
        {
            if (addRadioOther.TLIcivilLoads.sideArmId == 0)
                addRadioOther.TLIcivilLoads.sideArmId = null;
            var ConnectionString = _configuration["ConnectionStrings:ActiveConnection"];
            if (TryValidateModel(addRadioOther, nameof(AddRadioOtherIntegration)))
            {
                var response = _unitOfWorkService.InternalApiService.AddRadioInstallation(addRadioOther, Helpers.Constants.LoadSubType.TLIradioOther.ToString(), SiteCode, ConnectionString, TaskId);
                return Ok(response);
            }
            else
            {
                var ErrorMessages = from state in ModelState.Values
                                    from error in state.Errors
                                    select error.ErrorMessage;
                return Ok(new Response<AddRadioOtherIntegration>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
            }
        }
        [HttpGet("GetAttForAddMW_BU")]
        [ProducesResponseType(200, Type = typeof(ObjectInstAtts))]
        public IActionResult GetAttForAddMW_BU(int LibId, string SiteCode)
        {
            var response = _unitOfWorkService.InternalApiService.GetAttForAddMwInst(Helpers.Constants.LoadSubType.TLImwBU.ToString(), LibId, SiteCode);
            return Ok(response);
        }
        [HttpGet("GetAttForAddMW_ODU")]
        [ProducesResponseType(200, Type = typeof(ObjectInstAtts))]
        public IActionResult GetAttForAddMW_ODU(int LibId, string SiteCode, int AllCivilInstId)
        {
            var response = _unitOfWorkService.InternalApiService.GetAttForAddMwInst(Helpers.Constants.LoadSubType.TLImwODU.ToString(), LibId, SiteCode);
            return Ok(response);
        }
        [HttpGet("GetAttForAddMW_Dish")]
        [ProducesResponseType(200, Type = typeof(ObjectInstAtts))]
        public IActionResult GetAttForAddMW_Dish(int LibId, string SiteCode)
        {
            var response = _unitOfWorkService.InternalApiService.GetAttForAddMwInst(Helpers.Constants.LoadSubType.TLImwDish.ToString(), LibId, SiteCode);
            return Ok(response);
        }
        [HttpGet("GetAttForAddMW_RFU")]
        [ProducesResponseType(200, Type = typeof(ObjectInstAtts))]
        public IActionResult GetAttForAddMW_RFU(int LibId, string SiteCode)
        {
            var response = _unitOfWorkService.InternalApiService.GetAttForAddMwInst(Helpers.Constants.LoadSubType.TLImwRFU.ToString(), LibId, SiteCode);
            return Ok(response);
        }
        [HttpGet("GetAttForAddMW_Other")]
        [ProducesResponseType(200, Type = typeof(ObjectInstAtts))]
        public IActionResult GetAttForAddMW_Other(int LibId, string SiteCode)
        {
            var response = _unitOfWorkService.InternalApiService.GetAttForAddMwInst(Helpers.Constants.LoadSubType.TLImwOther.ToString(), LibId, SiteCode);
            return Ok(response);
        }


        [HttpPost("AddMW_BU")]
        [ProducesResponseType(200, Type = typeof(AddMW_BUViewModel))]
        public IActionResult AddMW_BU([FromBody] AddMW_BUViewModel AddMW_BUViewModel, string SiteCode, int TaskId)
        {
            if (AddMW_BUViewModel.TLIcivilLoads.sideArmId == 0)
                AddMW_BUViewModel.TLIcivilLoads.sideArmId = null;
            var ConnectionString = _configuration["ConnectionStrings:ActiveConnection"];
            if (TryValidateModel(AddMW_BUViewModel, nameof(AddMW_BUViewModel)))
            {
                var response = _unitOfWorkService.InternalApiService.AddMWInstallation(AddMW_BUViewModel, Helpers.Constants.LoadSubType.TLImwBU.ToString(), SiteCode, ConnectionString, TaskId);
                return Ok(response);
            }
            else
            {
                var ErrorMessages = from state in ModelState.Values
                                    from error in state.Errors
                                    select error.ErrorMessage;
                return Ok(new Response<AddMW_BUViewModel>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
            }
        }
        [HttpPost("AddMW_ODU")]
        [ProducesResponseType(200, Type = typeof(AddMW_ODUViewModel))]
        public IActionResult AddMW_ODU([FromBody] AddMW_ODUViewModel AddMW_ODUViewModel, string SiteCode, int TaskId)
        {
            if (AddMW_ODUViewModel.TLIcivilLoads.sideArmId == 0)
                AddMW_ODUViewModel.TLIcivilLoads.sideArmId = null;
            var ConnectionString = _configuration["ConnectionStrings:ActiveConnection"];
            if (TryValidateModel(AddMW_ODUViewModel, nameof(AddMW_ODUViewModel)))
            {
                var response = _unitOfWorkService.InternalApiService.AddMWInstallation(AddMW_ODUViewModel, Helpers.Constants.LoadSubType.TLImwODU.ToString(), SiteCode, ConnectionString, TaskId);
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
        [HttpPost("AddMW_Dish")]
        [ProducesResponseType(200, Type = typeof(AddMW_DishViewModel))]
        public IActionResult AddMW_Dish([FromBody] AddMW_DishViewModel AddMW_DishViewModel, string SiteCode, int TaskId)
        {
            if (AddMW_DishViewModel.TLIcivilLoads.sideArmId == 0)
                AddMW_DishViewModel.TLIcivilLoads.sideArmId = null;
            var ConnectionString = _configuration["ConnectionStrings:ActiveConnection"];
            if (TryValidateModel(AddMW_DishViewModel, nameof(AddMW_DishViewModel)))
            {
                var response = _unitOfWorkService.InternalApiService.AddMWInstallation(AddMW_DishViewModel, Helpers.Constants.LoadSubType.TLImwDish.ToString(), SiteCode, ConnectionString, TaskId);
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
        [HttpPost("AddMW_RFU")]
        [ProducesResponseType(200, Type = typeof(AddMW_RFUViewModel))]
        public IActionResult AddMW_RFU([FromBody] AddMW_RFUViewModel AddMW_RFUViewModel, string SiteCode, int TaskId)
        {
            if (AddMW_RFUViewModel.TLIcivilLoads.sideArmId == 0)
                AddMW_RFUViewModel.TLIcivilLoads.sideArmId = null;
            if (AddMW_RFUViewModel.MwPortId == 0)
                AddMW_RFUViewModel.MwPortId = null;
            var ConnectionString = _configuration["ConnectionStrings:ActiveConnection"];
            if (TryValidateModel(AddMW_RFUViewModel, nameof(AddMW_RFUViewModel)))
            {
                var response = _unitOfWorkService.InternalApiService.AddMWInstallation(AddMW_RFUViewModel, Helpers.Constants.LoadSubType.TLImwRFU.ToString(), SiteCode, ConnectionString, TaskId);
                return Ok(response);
            }
            else
            {
                var ErrorMessages = from state in ModelState.Values
                                    from error in state.Errors
                                    select error.ErrorMessage;
                return Ok(new Response<AddMW_RFUViewModel>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
            }
        }

        [HttpPost("AddMW_Other")]
        [ProducesResponseType(200, Type = typeof(AddMw_OtherViewModel))]
        public IActionResult AddMW_Other([FromBody] AddMw_OtherViewModel AddMw_OtherViewModel, string SiteCode, int TaskId)
        {
            if (AddMw_OtherViewModel.TLIcivilLoads.sideArmId == 0)
                AddMw_OtherViewModel.TLIcivilLoads.sideArmId = null;
            var ConnectionString = _configuration["ConnectionStrings:ActiveConnection"];
            if (TryValidateModel(AddMw_OtherViewModel, nameof(AddMw_OtherViewModel)))
            {
                var response = _unitOfWorkService.InternalApiService.AddMWInstallation(AddMw_OtherViewModel, Helpers.Constants.LoadSubType.TLImwOther.ToString(), SiteCode, ConnectionString, TaskId);
                return Ok(response);
            }
            else
            {
                var ErrorMessages = from state in ModelState.Values
                                    from error in state.Errors
                                    select error.ErrorMessage;
                return Ok(new Response<AddMW_RFUViewModel>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
            }
        }
        [HttpPost("EditMW_BU")]
        [ProducesResponseType(200, Type = typeof(EditMW_BUViewModel))]
        public async Task<IActionResult> EditMW_BU([FromBody] EditMW_BUViewModel MW_BU,int TaskId)
        {
            if (TryValidateModel(MW_BU, nameof(EditMW_BUViewModel)))
            {
                var response = await _unitOfWorkService.InternalApiService.EditMWInstallation(MW_BU, Helpers.Constants.LoadSubType.TLImwBU.ToString(), TaskId);
                return Ok(response);
            }
            else
            {
                var ErrorMessages = from state in ModelState.Values
                                    from error in state.Errors
                                    select error.ErrorMessage;
                return Ok(new Response<EditMW_BUViewModel>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
            }
        }
        [HttpPost("EditMW_Dish")]
        [ProducesResponseType(200, Type = typeof(EditMW_DishViewModel))]
        public async Task<IActionResult> EditMW_Dish([FromBody] EditMW_DishViewModel MW_Dish, int TaskId)
        {
            if (TryValidateModel(MW_Dish, nameof(EditMW_DishViewModel)))
            {
                var response = await _unitOfWorkService.InternalApiService.EditMWInstallation(MW_Dish, Helpers.Constants.LoadSubType.TLImwDish.ToString(), TaskId);
                return Ok(response);
            }
            else
            {
                var ErrorMessages = from state in ModelState.Values
                                    from error in state.Errors
                                    select error.ErrorMessage;
                return Ok(new Response<EditMW_DishViewModel>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
            }
        }
        [HttpPost("EditMW_ODU")]
        [ProducesResponseType(200, Type = typeof(EditMW_ODUViewModel))]
        public async Task<IActionResult> EditMW_ODU([FromBody] EditMW_ODUViewModel MW_ODU,int TaskId)
        {
            if (TryValidateModel(MW_ODU, nameof(EditMW_ODUViewModel)))
            {
                var response = await _unitOfWorkService.InternalApiService.EditMWInstallation(MW_ODU, Helpers.Constants.LoadSubType.TLImwODU.ToString(), TaskId);
                return Ok(response);
            }
            else
            {
                var ErrorMessages = from state in ModelState.Values
                                    from error in state.Errors
                                    select error.ErrorMessage;
                return Ok(new Response<EditMW_ODUViewModel>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
            }
        }
        [HttpPost("EditMW_RFU")]
        [ProducesResponseType(200, Type = typeof(EditMW_RFUViewModel))]
        public async Task<IActionResult> EditMW_RFU([FromBody] EditMW_RFUViewModel MW_RFU,int TaskId)
        {
            if (TryValidateModel(MW_RFU, nameof(EditMW_RFUViewModel)))
            {
                var response = await _unitOfWorkService.InternalApiService.EditMWInstallation(MW_RFU, Helpers.Constants.LoadSubType.TLImwRFU.ToString(), TaskId);
                return Ok(response);
            }
            else
            {
                var ErrorMessages = from state in ModelState.Values
                                    from error in state.Errors
                                    select error.ErrorMessage;
                return Ok(new Response<EditMW_RFUViewModel>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
            }
        }
        [HttpPost("EditMw_Other")]
        [ProducesResponseType(200, Type = typeof(EditMw_OtherViewModel))]
        public async Task<IActionResult> EditMw_Other([FromBody] EditMw_OtherViewModel Mw_Other,int TaskId)
        {
            if (TryValidateModel(Mw_Other, nameof(EditMw_OtherViewModel)))
            {
                var response = await _unitOfWorkService.InternalApiService.EditMWInstallation(Mw_Other, Helpers.Constants.LoadSubType.TLImwOther.ToString(), TaskId);
                return Ok(response);
            }
            else
            {
                var ErrorMessages = from state in ModelState.Values
                                    from error in state.Errors
                                    select error.ErrorMessage;
                return Ok(new Response<EditMw_OtherViewModel>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
            }
        }
        [HttpPost("DismantleMW_BU")]

        public IActionResult DismantleMW_BU(string sitecode, int LoadId, string LoadName,int TaskId)
        {
            var response = _unitOfWorkService.InternalApiService.DismantleLoads(sitecode, LoadId, LoadName, TaskId);
            return Ok(response);
        }
        [HttpPost("DismantleMW_ODU")]

        public IActionResult DismantleMW_ODU(string sitecode, int LoadId, string LoadName,int TaskId)
        {
            var response = _unitOfWorkService.InternalApiService.DismantleLoads(sitecode, LoadId, LoadName, TaskId);
            return Ok(response);
        }
        [HttpPost("DismantleMW_RFU")]

        public IActionResult DismantleMW_RFU(string sitecode, int LoadId, string LoadName,int TaskId)
        {
            var response = _unitOfWorkService.InternalApiService.DismantleLoads(sitecode, LoadId, LoadName, TaskId);
            return Ok(response);
        }
        [HttpPost("DismantleMW_Dish")]

        public IActionResult DismantleMW_Dish(string sitecode, int LoadId, string LoadName,int TaskId)
        {
            var response = _unitOfWorkService.InternalApiService.DismantleLoads(sitecode, LoadId, LoadName, TaskId);
            return Ok(response);
        }
        [HttpPost("DismantleMW_Other")]

        public IActionResult DismantleMW_Other(string sitecode, int LoadId, string LoadName,int TaskId)
        {
            var response = _unitOfWorkService.InternalApiService.DismantleLoads(sitecode, LoadId, LoadName, TaskId);
            return Ok(response);
        }
        [HttpPost("DismantlesideArm")]
        public IActionResult DismantlesideArm(string SiteCode, int sideArmId,int  TaskId)
        {
            var response = _unitOfWorkService.InternalApiService.DismantleSideArm(SiteCode, sideArmId, TaskId);
            return Ok(response);
        }
        [HttpGet("GetMW_BUById")]
        [ProducesResponseType(200, Type = typeof(ObjectInstAttsForSideArm))]
        public IActionResult GetMW_BUById(int MW_BU)
        {
            var response = _unitOfWorkService.InternalApiService.GetByIdMWInstallation(MW_BU, Helpers.Constants.LoadSubType.TLImwBU.ToString());
            return Ok(response);
        }
        [HttpGet("GetMW_ODUById")]
        [ProducesResponseType(200, Type = typeof(ObjectInstAttsForSideArm))]
        public IActionResult GetMW_ODUById(int MW_ODU)
        {
            var response = _unitOfWorkService.InternalApiService.GetByIdMWInstallation(MW_ODU, Helpers.Constants.LoadSubType.TLImwODU.ToString());
            return Ok(response);
        }
        [HttpGet("GetMW_DishById")]
        [ProducesResponseType(200, Type = typeof(ObjectInstAttsForSideArm))]
        public IActionResult GetMW_DishById(int MW_Dish)
        {
            var response = _unitOfWorkService.InternalApiService.GetByIdMWInstallation(MW_Dish, Helpers.Constants.LoadSubType.TLImwDish.ToString());
            return Ok(response);
        }
        [HttpGet("GetMW_RFUById")]
        [ProducesResponseType(200, Type = typeof(ObjectInstAttsForSideArm))]
        public IActionResult GetMW_RFUById(int MW_RFU)
        {
            var response = _unitOfWorkService.InternalApiService.GetByIdMWInstallation(MW_RFU, Helpers.Constants.LoadSubType.TLImwRFU.ToString());
            return Ok(response);
        }
        [HttpGet("GetMW_OtherById")]
        [ProducesResponseType(200, Type = typeof(ObjectInstAttsForSideArm))]
        public IActionResult GetMWOtherId(int mwOther)
        {
            var response = _unitOfWorkService.InternalApiService.GetByIdMWInstallation(mwOther, Helpers.Constants.LoadSubType.TLImwOther.ToString());
            return Ok(response);
        }
        [HttpGet("GetAttForAddSolar")]
        [ProducesResponseType(200, Type = typeof(ObjectInstAtts))]
        public IActionResult GetAttForAddSolar(string CabinetLibraryType, int OtherInventoryId, string SiteCode)
        {
            var response = _unitOfWorkService.InternalApiService.GetAttForAddOtherInventoryInstallation(Helpers.Constants.OtherInventoryType.TLIsolar.ToString(), CabinetLibraryType, OtherInventoryId, SiteCode);
            return Ok(response);
        }
        [HttpGet("GetAttForAddGenerator")]
        [ProducesResponseType(200, Type = typeof(ObjectInstAtts))]
        public IActionResult GetAttForAddGenerator(string CabinetLibraryType, int OtherInventoryId, string SiteCode)
        {
            var response = _unitOfWorkService.InternalApiService.GetAttForAddOtherInventoryInstallation(Helpers.Constants.OtherInventoryType.TLIgenerator.ToString(), CabinetLibraryType, OtherInventoryId, SiteCode);
            return Ok(response);
        }
        [HttpPost("AddSolar")]
        [ProducesResponseType(200, Type = typeof(AddSolarViewModel))]
        public IActionResult AddSolar([FromBody] AddSolarViewModel addSolarViewModel, string SiteCode,int TaskId)
        {
            var ConnectionString = _configuration["ConnectionStrings:ActiveConnection"];
            if (addSolarViewModel.TLIotherInSite.ReservedSpace == true)
            {
                var CheckReservedSapce = _unitOfWorkService.SiteService.CheckRentedSpace(SiteCode, addSolarViewModel.SpaceInstallation);
                if (CheckReservedSapce == true)
                {
                    if (TryValidateModel(addSolarViewModel, nameof(AddSolarViewModel)))
                    {
                        var response = _unitOfWorkService.InternalApiService.AddOtherInventoryInstallation(addSolarViewModel, Helpers.Constants.OtherInventoryType.TLIsolar.ToString(), SiteCode, ConnectionString, TaskId);
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
                    var response = _unitOfWorkService.InternalApiService.AddOtherInventoryInstallation(addSolarViewModel, Helpers.Constants.OtherInventoryType.TLIsolar.ToString(), SiteCode, ConnectionString, TaskId);
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
        public IActionResult AddGenerator([FromBody] AddGeneratorViewModel addGeneratorViewModel, string SiteCode,int TaskId)
        {
            var ConnectionString = _configuration["ConnectionStrings:ActiveConnection"];
            if (addGeneratorViewModel.TLIotherInSite.ReservedSpace == true)
            {
                var CheckReservedSapce = _unitOfWorkService.SiteService.CheckRentedSpace(SiteCode, addGeneratorViewModel.SpaceInstallation);
                if (CheckReservedSapce == true)
                {
                    if (TryValidateModel(addGeneratorViewModel, nameof(AddGeneratorViewModel)))
                    {
                        var response = _unitOfWorkService.InternalApiService.AddOtherInventoryInstallation(addGeneratorViewModel, Helpers.Constants.OtherInventoryType.TLIgenerator.ToString(), SiteCode, ConnectionString, TaskId);
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
                    var response = _unitOfWorkService.InternalApiService.AddOtherInventoryInstallation(addGeneratorViewModel, Helpers.Constants.OtherInventoryType.TLIgenerator.ToString(), SiteCode, ConnectionString, TaskId);
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
        [HttpGet("GetSolarById")]
        [ProducesResponseType(200, Type = typeof(ObjectInstAtts))]
        public IActionResult GetSolarById(int OtherInventoryInstId)
        {
            var response = _unitOfWorkService.InternalApiService.GetByIdOtherInventoryInstallation(OtherInventoryInstId, Helpers.Constants.OtherInventoryType.TLIsolar.ToString());
            return Ok(response);
        }
        [HttpGet("GetGeneratorById")]
        [ProducesResponseType(200, Type = typeof(ObjectInstAtts))]
        public IActionResult GetGeneratorById(int OtherInventoryInstId)
        {
            var response = _unitOfWorkService.InternalApiService.GetByIdOtherInventoryInstallation(OtherInventoryInstId, Helpers.Constants.OtherInventoryType.TLIgenerator.ToString());
            return Ok(response);
        }
        [HttpPost("EditSolar")]
        [ProducesResponseType(200, Type = typeof(EditSolarViewModel))]
        public async Task<IActionResult> EditSolar([FromBody] EditSolarViewModel editSolarViewModel,int TaskId)
        {
            if (TryValidateModel(editSolarViewModel, nameof(EditSolarViewModel)))
            {
                var response = await _unitOfWorkService.InternalApiService.EditOtherInventoryInstallation(editSolarViewModel, Helpers.Constants.OtherInventoryType.TLIsolar.ToString(), TaskId);
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
        public async Task<IActionResult> EditCivilNonSteel([FromBody] EditGeneratorViewModel editGeneratorViewModel,int TaskId)
        {
            if (TryValidateModel(editGeneratorViewModel, nameof(EditGeneratorViewModel)))
            {
                var response = await _unitOfWorkService.InternalApiService.EditOtherInventoryInstallation(editGeneratorViewModel, Helpers.Constants.OtherInventoryType.TLIgenerator.ToString(), TaskId);
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
        public IActionResult DismantleOtherInventory(string SiteCode, int OtherInventoryId, string OtherInventoryName,int TaskId)
        {
            var response = _unitOfWorkService.InternalApiService.DismantleOtherInventory(SiteCode, OtherInventoryId, OtherInventoryName, TaskId);
            return Ok(response);

        } 
        [HttpPost("GetSolarBySiteWithEnabledAtt")]
        [ProducesResponseType(200, Type = typeof(object))]
        public IActionResult GetSolarBySiteWithEnabledAtt([FromBody] CombineFilters CombineFilters, [FromQuery] SiteBaseFilter BaseFilter, bool WithFilterData, [FromQuery] ParameterPagination parameterPagination)
        {
            var response = _unitOfWorkService.InternalApiService.GetSolarBySiteWithEnabledAtt(BaseFilter, WithFilterData, CombineFilters, parameterPagination);
            return Ok(response);
        }
        [HttpPost("GetGeneratorBySiteWithEnabledAtt")]
        [ProducesResponseType(200, Type = typeof(object))]
        public IActionResult GetGeneratorBySiteWithEnabledAtt([FromBody] CombineFilters CombineFilters, [FromQuery] SiteBaseFilter BaseFilter, bool WithFilterData, [FromQuery] ParameterPagination parameterPagination)
        {
            var response = _unitOfWorkService.InternalApiService.GetGeneratorBySiteWithEnabledAtt(BaseFilter, WithFilterData, CombineFilters, parameterPagination);
            return Ok(response);
        }
        [HttpPost("GetMW_DishOnSiteWithEnableAtt")]
        [ProducesResponseType(200, Type = typeof(object))]
        public IActionResult GetMW_DishOnSiteWithEnableAtt([FromQuery] LoadsOnSiteFilter BaseFilter, bool WithFilterData, [FromBody] CombineFilters CombineFilters, int? CivilId, string CivilType, [FromQuery] ParameterPagination parameterPagination)
        {
            var response = _unitOfWorkService.InternalApiService.GetMW_DishOnSiteWithEnableAtt(BaseFilter, WithFilterData, CombineFilters, parameterPagination, CivilId, CivilType);
            return Ok(response);
        }
        [HttpPost("GetMW_BUOnSiteWithEnableAtt")]
        [ProducesResponseType(200, Type = typeof(object))]
        public IActionResult GetMW_BUOnSiteWithEnableAtt([FromQuery] LoadsOnSiteFilter BaseFilter, bool WithFilterData, [FromBody] CombineFilters CombineFilters, int? CivilId, string CivilType, [FromQuery] ParameterPagination parameterPagination)
        {
            var response = _unitOfWorkService.InternalApiService.GetMW_BUOnSiteWithEnableAtt(BaseFilter, WithFilterData, CombineFilters, parameterPagination, CivilId, CivilType);
            return Ok(response);
        }
        [HttpPost("GetMW_ODUOnSiteWithEnableAtt")]
        [ProducesResponseType(200, Type = typeof(object))]
        public IActionResult GetMW_ODUOnSiteWithEnableAtt([FromQuery] LoadsOnSiteFilter BaseFilter, bool WithFilterData, [FromBody] CombineFilters CombineFilters, int? CivilId, string CivilType, [FromQuery] ParameterPagination parameterPagination)
        {
            var response = _unitOfWorkService.InternalApiService.GetMW_ODUOnSiteWithEnableAtt(BaseFilter, WithFilterData, CombineFilters, parameterPagination, CivilId, CivilType);
            return Ok(response);
        }
        [HttpPost("GetMW_RFUOnSiteWithEnableAtt")]
        [ProducesResponseType(200, Type = typeof(object))]
        public IActionResult GetMW_RFUOnSiteWithEnableAtt([FromQuery] LoadsOnSiteFilter BaseFilter, bool WithFilterData, [FromBody] CombineFilters CombineFilters, int? CivilId, string CivilType, [FromQuery] ParameterPagination parameterPagination)
        {
            var response = _unitOfWorkService.InternalApiService.GetMW_RFUOnSiteWithEnableAtt(BaseFilter, WithFilterData, CombineFilters, parameterPagination, CivilId, CivilType);
            return Ok(response);
        }
        [HttpPost("GetMW_OtherOnSiteWithEnableAtt")]
        [ProducesResponseType(200, Type = typeof(object))]
        public IActionResult GetMW_OtherOnSiteWithEnableAtt([FromQuery] LoadsOnSiteFilter BaseFilter, bool WithFilterData, [FromBody] CombineFilters CombineFilters, int? CivilId, string CivilType, [FromQuery] ParameterPagination parameterPagination)
        {
            var response = _unitOfWorkService.InternalApiService.GetMW_OtherOnSiteWithEnableAtt(BaseFilter, WithFilterData, CombineFilters, parameterPagination, CivilId, CivilType);
            return Ok(response);
        }
        [HttpGet("GetSideArmById")]
        [ProducesResponseType(200, Type = typeof(ObjectInstAttsForSideArm))]
        public IActionResult GetSideArmById(int SideId)
        {
            var response = _unitOfWorkService.InternalApiService.GetSideArmById(SideId, Helpers.Constants.TablesNames.TLIsideArm.ToString());
            return Ok(response);
        }
        [HttpPost("AddSideArm")]
        [ProducesResponseType(200, Type = typeof(AllItemAttributes))]
        public IActionResult AddSideArm([FromBody] AddSideArmViewModel SideArmViewModel, string SiteCode,int TaskId)
        {
            var ConnectionString = _configuration["ConnectionStrings:ActiveConnection"];
            var Response = _unitOfWorkService.InternalApiService.AddSideArm(SideArmViewModel, SiteCode, ConnectionString, TaskId);
            return Ok(Response);
        }
        [HttpPost("UpdateSideArm")]
        [ProducesResponseType(200, Type = typeof(AllItemAttributes))]
        public async Task<IActionResult> UpdateSideArm([FromBody] EditSideArmViewModel SideArmViewModel,int TaskId)
        {
            var response = await _unitOfWorkService.InternalApiService.UpdateSideArm(SideArmViewModel, TaskId);
            return Ok(response);
        }
        [HttpGet("GetAttForAddSideArm")]
        [ProducesResponseType(200, Type = typeof(ObjectInstAtts))]
        public IActionResult GetAttForAdd(int LibId)
        {
            var response = _unitOfWorkService.InternalApiService.GetAttForAddSideArm(LibId);
            return Ok(response);
        }
        [HttpPost("getSideArmsWithEnabledAtt")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<object>))]
        public IActionResult getSideArmsWithEnabledAtt([FromBody] CombineFilters CombineFilters, [FromQuery] CivilLoadsFilter BaseFilter, [FromQuery] bool WithFilterData, [FromQuery] int? CivilRecordId, string CivilType, [FromQuery] ParameterPagination parameters)
        {
            var response = _unitOfWorkService.InternalApiService.GetSideArmsWithEnabledAtt(BaseFilter, WithFilterData, parameters, CombineFilters, CivilRecordId, CivilType);
            return Ok(response);
        }
        [HttpPost("GetCivilNonSteelLibrariesEnabledAtt")]
        [ProducesResponseType(200, Type = typeof(Response<ReturnWithFilters<object>>))]
        public IActionResult GetCivilNonSteelLibrariesEnabledAtt([FromBody] CombineFilters CombineFilters, bool WithFilterData, [FromQuery] ParameterPagination parameterPagination)
        {
            var response = _unitOfWorkService.InternalApiService.GetCivilNonSteelLibrariesEnabledAtt(CombineFilters, WithFilterData, parameterPagination);
            return Ok(response);
        }
        [HttpPost("AddCivilNonSteelLibrary")]
        [ProducesResponseType(200, Type = typeof(AddCivilNonSteelLibraryViewModel))]
        public IActionResult AddCivilNonSteelLibrary([FromBody] AddCivilNonSteelLibraryViewModel addCivilNonSteelLibraryViewModel)
        {
            if (TryValidateModel(addCivilNonSteelLibraryViewModel, nameof(AddCivilNonSteelLibraryViewModel)))
            {
                var ConnectionString = _configuration["ConnectionStrings:ActiveConnection"];
                var response = _unitOfWorkService.InternalApiService.AddCivilLibrary(Helpers.Constants.CivilType.TLIcivilNonSteelLibrary.ToString(), addCivilNonSteelLibraryViewModel, ConnectionString);
                return Ok(response);
            }
            else
            {
                var ErrorMessages = from state in ModelState.Values
                                    from error in state.Errors
                                    select error.ErrorMessage;
                return Ok(new Response<AddCivilNonSteelLibraryViewModel>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
            }
        }
        [HttpPost("GetCivilWithoutLegLibrariesEnabledAtt")]
        [ProducesResponseType(200, Type = typeof(Response<ReturnWithFilters<object>>))]
        public IActionResult GetCivilWithoutLegLibrariesEnabledAtt([FromBody] CombineFilters ComineOutPut, bool WithFilterData, int CategoryId, [FromQuery] ParameterPagination parameters)
        {
            var response = _unitOfWorkService.InternalApiService.GetCivilWithoutLegLibrariesEnabledAtt(ComineOutPut, WithFilterData, CategoryId, parameters);
            return Ok(response);
        }
        [HttpPost("GetMW_ODULibraries")]
        [ProducesResponseType(200, Type = typeof(Response<ReturnWithFilters<object>>))]
        public IActionResult GetMW_ODULibraries([FromBody] CombineFilters CombineFilters, bool WithFilterData, [FromQuery] ParameterPagination parameters)
        {
            var response = _unitOfWorkService.InternalApiService.GetMW_ODULibraries(CombineFilters, WithFilterData, parameters);
            return Ok(response);
        }
        [HttpPost("GetMW_BULibraries")]
        [ProducesResponseType(200, Type = typeof(Response<ReturnWithFilters<object>>))]
        public IActionResult GetMW_BULibraries([FromBody] CombineFilters CombineFilters, bool WithFilterData, [FromQuery] ParameterPagination parameters)
        {
            var response = _unitOfWorkService.InternalApiService.GetMW_BULibraries(CombineFilters, WithFilterData, parameters);
           
            return Ok(response);
        }
        [HttpPost("GetMW_OtherLibraries")]
        [ProducesResponseType(200, Type = typeof(Response<ReturnWithFilters<object>>))]
        public IActionResult GetMW_OtherLibraries([FromBody] CombineFilters CombineFilters, [FromQuery] ParameterPagination parameters)
        {
            var response = _unitOfWorkService.InternalApiService.GetMW_OtherLibraries(CombineFilters, parameters);
            return Ok(response);
        }
        [HttpPost("AddSolarLibrary")]
        [ProducesResponseType(200, Type = typeof(Nullable))]
        public IActionResult AddSolarLibrary([FromBody] AddSolarLibraryViewModel addSolarLibrary)
        {
            if (TryValidateModel(addSolarLibrary, nameof(AddSolarLibraryViewModel)))
            {
                var ConnectionString = _configuration["ConnectionStrings:ActiveConnection"];
                var response = _unitOfWorkService.InternalApiService.AddOtherInventoryLibrary(Helpers.Constants.OtherInventoryType.TLIsolarLibrary.ToString(), addSolarLibrary, ConnectionString);
                return Ok(response);
            }
            else
            {
                var ErrorMessages = from state in ModelState.Values
                                    from error in state.Errors
                                    select error.ErrorMessage;
                return Ok(new Response<AddSolarLibraryViewModel>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
            }
        }
        [HttpPost("GetGeneratorLibraryEnabledAtt")]
        [ProducesResponseType(200, Type = typeof(Response<ReturnWithFilters<object>>))]
        public IActionResult GetGeneratorLibraryEnabledAtt([FromBody] CombineFilters CombineFilters, [FromQuery] bool WithFilterData, [FromQuery] ParameterPagination parameters)
        {
            var response = _unitOfWorkService.InternalApiService.GetGeneratorLibraryEnabledAtt(CombineFilters, WithFilterData, parameters);
            return Ok(response);
        }
        [HttpPost("AddGeneratorLibrary")]
        [ProducesResponseType(200, Type = typeof(Nullable))]
        public IActionResult AddGeneratorLibrary([FromBody] AddGeneratorLibraryViewModel addGeneratorLibrary)
        {
            if (TryValidateModel(addGeneratorLibrary, nameof(AddGeneratorLibraryViewModel)))
            {
                var ConnectionString = _configuration["ConnectionStrings:ActiveConnection"];
                var response = _unitOfWorkService.InternalApiService.AddOtherInventoryLibrary(Helpers.Constants.OtherInventoryType.TLIgeneratorLibrary.ToString(), addGeneratorLibrary, ConnectionString);
                return Ok(response);
            }
            else
            {
                var ErrorMessages = from state in ModelState.Values
                                    from error in state.Errors
                                    select error.ErrorMessage;
                return Ok(new Response<AddGeneratorLibraryViewModel>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
            }
        }
      
        [HttpPost("AddMW_RFULibrary")]
        [ProducesResponseType(200, Type = typeof(AddMW_RFULibraryViewModel))]
        public IActionResult AddMW_RFULibrary([FromBody] AddMW_RFULibraryViewModel addMW_RFULibraryViewModel)
        {
            if (TryValidateModel(addMW_RFULibraryViewModel, nameof(AddMW_RFULibraryViewModel)))
            {
                var ConnectionString = _configuration["ConnectionStrings:ActiveConnection"];
                var response = _unitOfWorkService.InternalApiService.AddMWLibrary(Helpers.Constants.LoadSubType.TLImwRFULibrary.ToString(), addMW_RFULibraryViewModel, ConnectionString);
                return Ok(response);
            }
            else
            {
                var ErrorMessages = from state in ModelState.Values
                                    from error in state.Errors
                                    select error.ErrorMessage;
                return Ok(new Response<AddMW_RFULibraryViewModel>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
            }
        }

        [HttpPost("GetMW_RFULibraries")]
        [ProducesResponseType(200, Type = typeof(Response<ReturnWithFilters<object>>))]
        public IActionResult GetMW_RFULibraries([FromBody] CombineFilters CombineFilters, bool WithFilterData, [FromQuery] ParameterPagination parameters)
        {
            var response = _unitOfWorkService.InternalApiService.GetMW_RFULibraries(CombineFilters, WithFilterData, parameters);
            return Ok(response);
        }
        [HttpPost("AddMW_OtherLibrary")]
        [ProducesResponseType(200, Type = typeof(AddMW_OtherLibraryViewModel))]
        public IActionResult AddMW_OtherLibrary([FromBody] AddMW_OtherLibraryViewModel addMW_OtherLibraryViewModel)
        {
            if (TryValidateModel(addMW_OtherLibraryViewModel, nameof(AddMW_OtherLibraryViewModel)))
            {
                var ConnectionString = _configuration["ConnectionStrings:ActiveConnection"];
                var response = _unitOfWorkService.InternalApiService.AddMWLibrary(Helpers.Constants.LoadSubType.TLImwOtherLibrary.ToString(), addMW_OtherLibraryViewModel, ConnectionString);
                return Ok(response);
            }
            else
            {
                var ErrorMessages = from state in ModelState.Values
                                    from error in state.Errors
                                    select error.ErrorMessage;
                return Ok(new Response<AddMW_OtherLibraryViewModel>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
            }
        }
        [HttpPost("AddMW_BULibrary")]
        [ProducesResponseType(200, Type = typeof(AddMW_BULibraryViewModel))]
        public IActionResult AddMW_BULibrary([FromBody] AddMW_BULibraryViewModel addMW_BULibraryViewModel)
        {
            if (TryValidateModel(addMW_BULibraryViewModel, nameof(AddMW_BULibraryViewModel)))
            {
                var ConnectionString = _configuration["ConnectionStrings:ActiveConnection"];
                var response = _unitOfWorkService.InternalApiService.AddMWLibrary(Helpers.Constants.LoadSubType.TLImwBULibrary.ToString(), addMW_BULibraryViewModel, ConnectionString);
                return Ok(response);
            }
            else
            {
                var ErrorMessages = from state in ModelState.Values
                                    from error in state.Errors
                                    select error.ErrorMessage;
                return Ok(new Response<AddMW_BULibraryViewModel>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
            }
        }
        [HttpPost("AddMW_DishLibrary")]
        [ProducesResponseType(200, Type = typeof(AddMW_DishLibraryViewModel))]
        public IActionResult AddMW_DishLibrary([FromBody] AddMW_DishLibraryViewModel addMW_BULibraryViewModel)
        {
            if (TryValidateModel(addMW_BULibraryViewModel, nameof(AddMW_DishLibraryViewModel)))
            {
                var ConnectionString = _configuration["ConnectionStrings:ActiveConnection"];
                var response = _unitOfWorkService.InternalApiService.AddMWLibrary(Helpers.Constants.LoadSubType.TLImwDishLibrary.ToString(), addMW_BULibraryViewModel, ConnectionString);
                return Ok(response);
            }
            else
            {
                var ErrorMessages = from state in ModelState.Values
                                    from error in state.Errors
                                    select error.ErrorMessage;
                return Ok(new Response<AddMW_DishLibraryViewModel>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
            }
        }
        [HttpPost("GetMW_DishLibraries")]
        [ProducesResponseType(200, Type = typeof(Response<ReturnWithFilters<object>>))]
        public IActionResult GetMW_DishLibraries([FromBody] CombineFilters CombineFilters, bool WithFilterData, [FromQuery] ParameterPagination parameters)
        {
            var response = _unitOfWorkService.InternalApiService.GetMW_DishLibraries(CombineFilters, WithFilterData, parameters);
            return Ok(response);
        }
        [HttpGet("GetForAddLibrary")]
        [ProducesResponseType(200, Type = typeof(AllItemAttributes))]
        public IActionResult GetForAddLibrary(string TableName, int? CategoryId = null)
        {
            var response = _unitOfWorkService.InternalApiService.GetForAddAllLibrary( TableName, CategoryId);
            return Ok(response);
        }
        [HttpPost("AddMW_ODULibrary")]
        [ProducesResponseType(200, Type = typeof(AddMW_ODULibraryViewModel))]
        public IActionResult AddMW_ODULibrary([FromBody] AddMW_ODULibraryViewModel addMW_ODULibraryViewModel)
        {
            if (TryValidateModel(addMW_ODULibraryViewModel, nameof(AddMW_ODULibraryViewModel)))
            {
                var ConnectionString = _configuration["ConnectionStrings:ActiveConnection"];
                var response = _unitOfWorkService.InternalApiService.AddMWLibrary(Helpers.Constants.LoadSubType.TLImwODULibrary.ToString(), addMW_ODULibraryViewModel, ConnectionString);
                return Ok(response);
            }
            else
            {
                var ErrorMessages = from state in ModelState.Values
                                    from error in state.Errors
                                    select error.ErrorMessage;
                return Ok(new Response<AddMW_ODULibraryViewModel>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
            }
        }
        [HttpPost("AddCivilWithoutLegLibrary")]
        [ProducesResponseType(200, Type = typeof(AddCivilWithoutLegLibraryViewModel))]
        public IActionResult AddCivilWithoutLegLibrary([FromBody] AddCivilWithoutLegLibraryViewModel addCivilWithoutLegLibraryViewModel)
        {
            if (TryValidateModel(addCivilWithoutLegLibraryViewModel, nameof(AddCivilWithoutLegLibraryViewModel)))
            {
                var ConnectionString = _configuration["ConnectionStrings:ActiveConnection"];
                var response = _unitOfWorkService.InternalApiService.AddCivilLibrary(Helpers.Constants.CivilType.TLIcivilWithoutLegLibrary.ToString(), addCivilWithoutLegLibraryViewModel, ConnectionString);
                return Ok(response);
            }
            else
            {
                var ErrorMessages = from state in ModelState.Values
                                    from error in state.Errors
                                    select error.ErrorMessage;
                return Ok(new Response<AddCivilWithoutLegLibraryViewModel>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
            }
        }
        [HttpPost("AddCivilWithLegLibrary")]
        [ProducesResponseType(200, Type = typeof(AddCivilWithLegLibraryViewModel))]
        public IActionResult AddCivilWithLegLibrary([FromBody] AddCivilWithLegLibraryViewModel CivilWithLegLibraryViewModel)
        {
            var ConnectionString = _configuration["ConnectionStrings:ActiveConnection"];
            if (TryValidateModel(CivilWithLegLibraryViewModel, nameof(AddCivilWithLegLibraryViewModel)))
            {
                var response = _unitOfWorkService.InternalApiService.AddCivilLibrary(Helpers.Constants.CivilType.TLIcivilWithLegLibrary.ToString(), CivilWithLegLibraryViewModel, ConnectionString);
                return Ok(response);
            }
            else
            {
                var ErrorMessages = from state in ModelState.Values
                                    from error in state.Errors
                                    select error.ErrorMessage;
                return Ok(new Response<AddCivilWithLegLibraryViewModel>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
            }
        }
        [HttpPost("GetCivilWithLegLibrariesEnabledAtt")]
        [ProducesResponseType(200, Type = typeof(Response<ReturnWithFilters<object>>))]
        public IActionResult GetCivilWithLegLibrariesEnabledAtt([FromBody] CombineFilters CombineOutPut, bool WithFilterData, [FromQuery] ParameterPagination parameters)
        {
            var response = _unitOfWorkService.InternalApiService.GetCivilWithLegLibrariesEnabledAtt(CombineOutPut, WithFilterData, parameters);
            return Ok(response);
        }
        [HttpPost("GetSolarLibraryEnabledAtt")]
        [ProducesResponseType(200, Type = typeof(Response<ReturnWithFilters<object>>))]
        public IActionResult GetSolarLibraryEnabledAtt([FromBody] CombineFilters CombineFilters, bool WithFilterData, [FromQuery] ParameterPagination parameters)
        {
            var response = _unitOfWorkService.InternalApiService.GetSolarLibraryEnabledAtt(CombineFilters, WithFilterData, parameters);
            return Ok(response);
        }
        [HttpGet("GetAttForAddCivilWithLegs")]
        [ProducesResponseType(200, Type = typeof(ObjectInstAtts))]
        public IActionResult GetAttForAddCivilWithLegs(int CivilLibraryId, string SiteCode, int? CategoryId)
        {
            var response = _unitOfWorkService.InternalApiService.GetAttForAddCivilInstallation(Helpers.Constants.CivilType.TLIcivilWithLegs.ToString(), CivilLibraryId, CategoryId, SiteCode);
            return Ok(response);
        }

        [HttpPost("GetCivilWithLegsWithEnableAtt")]
        [ProducesResponseType(200, Type = typeof(Response<ReturnWithFilters<object>>))]
        public IActionResult GetCivilWithLegsWithEnableAtt([FromQuery] string siteCode, [FromQuery] bool WithFilterData, [FromBody] CombineFilters CombineFilters, [FromQuery] ParameterPagination parameterPagination)
        {
            var response = _unitOfWorkService.InternalApiService.GetCivilWithLegsWithEnableAtt(siteCode, WithFilterData, CombineFilters, parameterPagination);
            return Ok(response);
        }
        [HttpPost("GetCivilWithoutLegWithEnableAtt")]
        [ProducesResponseType(200, Type = typeof(Response<ReturnWithFilters<object>>))]
        public IActionResult GetCivilWithoutLegWithEnableAtt([FromQuery] string siteCode, [FromQuery] bool WithFilterData, [FromBody] CombineFilters CombineFilters, [FromQuery] ParameterPagination parameterPagination, int CategoryId)
        {
            var response = _unitOfWorkService.InternalApiService.GetCivilWithoutLegWithEnableAtt(siteCode,WithFilterData, CombineFilters, parameterPagination, CategoryId);
            return Ok(response);
        }
        [HttpPost("GetCivilNonSteelWithEnableAtt")]
        [ProducesResponseType(200, Type = typeof(Response<ReturnWithFilters<object>>))]
        public IActionResult GetCivilNonSteelWithEnableAtt([FromQuery] string siteCode, [FromQuery] bool WithFilterData, [FromBody] CombineFilters CombineFilters, [FromQuery] ParameterPagination parameterPagination)
        {
            var response = _unitOfWorkService.InternalApiService.GetCivilNonSteelWithEnableAtt(siteCode,WithFilterData, CombineFilters, parameterPagination);
            return Ok(response);
        }
        [HttpGet("GetAttForAddCivilWithoutLegs")]
        [ProducesResponseType(200, Type = typeof(ObjectInstAtts))]
        public IActionResult GetAttForAddCivilWithoutLegs(int CivilLibraryId, string SiteCode, int? CategoryId)
        {
            var response = _unitOfWorkService.InternalApiService.GetAttForAddCivilInstallation(Helpers.Constants.CivilType.TLIcivilWithoutLeg.ToString(), CivilLibraryId, CategoryId, SiteCode);
            return Ok(response);
        }
        [HttpGet("GetAttForAddCivilNonSteel")]
        [ProducesResponseType(200, Type = typeof(ObjectInstAtts))]
        public IActionResult GetAttForAddCivilNonSteel(int CivilLibraryId, string SiteCode, int? CategoryId)
        {
            var response = _unitOfWorkService.InternalApiService.GetAttForAddCivilInstallation(Helpers.Constants.CivilType.TLIcivilNonSteel.ToString(), CivilLibraryId, CategoryId, SiteCode);
            return Ok(response);
        }
        [HttpPost("AddCivilWithLegs/{SiteCode}")]
        [ProducesResponseType(200, Type = typeof(AddCivilWithLegsViewModelInternal))]
        public IActionResult AddCivilWithLegs([FromBody] AddCivilWithLegsViewModelInternal addCivilWithLeg, string SiteCode,int TaskId)
        {
            var ConnectionString = _configuration["ConnectionStrings:ActiveConnection"];
            if (addCivilWithLeg.TLIcivilSiteDate.ReservedSpace == true)
            {
                var CheckReservedSapce = _unitOfWorkService.SiteService.CheckRentedSpace(SiteCode, addCivilWithLeg.SpaceInstallation);
                if (CheckReservedSapce == true)
                {
                    if (TryValidateModel(addCivilWithLeg, nameof(AddCivilWithLegsViewModelInternal)))
                    {
                        var response = _unitOfWorkService.InternalApiService.AddCivilInstallation(addCivilWithLeg, Helpers.Constants.CivilType.TLIcivilWithLegs.ToString(), SiteCode, ConnectionString, TaskId);
                        return Ok(response);
                    }
                    else
                    {
                        var ErrorMessages = from state in ModelState.Values
                                            from error in state.Errors
                                            select error.ErrorMessage;
                        return Ok(new Response<AddCivilWithLegsViewModelInternal>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
                    }
                }
            }
            else if (addCivilWithLeg.TLIcivilSiteDate.ReservedSpace == false)
            {
                if (TryValidateModel(addCivilWithLeg, nameof(AddCivilWithLegsViewModelInternal)))
                {
                    var response = _unitOfWorkService.InternalApiService.AddCivilInstallation(addCivilWithLeg, Helpers.Constants.CivilType.TLIcivilWithLegs.ToString(), SiteCode, ConnectionString, TaskId);
                    return Ok(response);
                }
                else
                {
                    var ErrorMessages = from state in ModelState.Values
                                        from error in state.Errors
                                        select error.ErrorMessage;
                    return Ok(new Response<AddCivilWithLegsViewModelInternal>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
                }
            }
            return Ok(new Response<AddCivilWithLegsViewModelInternal>(true, null, null, "There is no space on the site", (int)Helpers.Constants.ApiReturnCode.fail));

        }

        [HttpPost("AddCivilWithoutLegs/{SiteCode}")]
        [ProducesResponseType(200, Type = typeof(AddCivilWithoutLegViewModelIntegration))]
        public IActionResult AddCivilWithoutLegs([FromBody] AddCivilWithoutLegViewModelIntegration addCivilWithoutLeg, string SiteCode,int TaskId)
        {
            var ConnectionString = _configuration["ConnectionStrings:ActiveConnection"];
            if (addCivilWithoutLeg.TLIcivilSiteDate.ReservedSpace == true)
            {
                var CheckReservedSapce = _unitOfWorkService.SiteService.CheckRentedSpace(SiteCode, addCivilWithoutLeg.SpaceInstallation);
                if (CheckReservedSapce == true)
                {
                    if (TryValidateModel(addCivilWithoutLeg, nameof(AddCivilWithoutLegViewModelIntegration)))
                    {
                        var response = _unitOfWorkService.InternalApiService.AddCivilInstallation(addCivilWithoutLeg, Helpers.Constants.CivilType.TLIcivilWithoutLeg.ToString(), SiteCode, ConnectionString, TaskId);
                        return Ok(response);
                    }
                    else
                    {
                        var ErrorMessages = from state in ModelState.Values
                                            from error in state.Errors
                                            select error.ErrorMessage;
                        return Ok(new Response<AddCivilWithoutLegViewModelIntegration>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
                    }
                }
            }
            else if (addCivilWithoutLeg.TLIcivilSiteDate.ReservedSpace == false)
            {
                if (TryValidateModel(addCivilWithoutLeg, nameof(AddCivilWithoutLegViewModelIntegration)))
                {
                    var response = _unitOfWorkService.InternalApiService.AddCivilInstallation(addCivilWithoutLeg, Helpers.Constants.CivilType.TLIcivilWithoutLeg.ToString(), SiteCode, ConnectionString, TaskId);
                    return Ok(response);
                }
                else
                {
                    var ErrorMessages = from state in ModelState.Values
                                        from error in state.Errors
                                        select error.ErrorMessage;
                    return Ok(new Response<AddCivilWithoutLegViewModelIntegration>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
                }
            }
            return Ok(new Response<AddCivilWithoutLegViewModelIntegration>(true, null, null, "There is no space on the site", (int)Helpers.Constants.ApiReturnCode.fail));

        }

        [HttpPost("AddCivilNonSteel/{SiteCode}")]
        [ProducesResponseType(200, Type = typeof(AddCivilNonSteelViewModelIntegration))]
        public IActionResult AddCivilNonSteel([FromBody] AddCivilNonSteelViewModelIntegration addCivilNonSteel, string SiteCode,int TaskId)
        {
            var ConnectionString = _configuration["ConnectionStrings:ActiveConnection"];
            if (addCivilNonSteel.TLIcivilSiteDate.ReservedSpace == true)
            {
                var CheckReservedSapce = _unitOfWorkService.SiteService.CheckRentedSpace(SiteCode, addCivilNonSteel.SpaceInstallation);
                if (CheckReservedSapce == true)
                {
                    if (TryValidateModel(addCivilNonSteel, nameof(AddCivilNonSteelViewModelIntegration)))
                    {
                        var response = _unitOfWorkService.InternalApiService.AddCivilInstallation(addCivilNonSteel, Helpers.Constants.CivilType.TLIcivilNonSteel.ToString(), SiteCode, ConnectionString, TaskId);
                        return Ok(response);
                    }
                    else
                    {
                        var ErrorMessages = from state in ModelState.Values
                                            from error in state.Errors
                                            select error.ErrorMessage;
                        return Ok(new Response<AddCivilNonSteelViewModelIntegration>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
                    }
                }
            }
            else if (addCivilNonSteel.TLIcivilSiteDate.ReservedSpace == false)
            {
                if (TryValidateModel(addCivilNonSteel, nameof(AddCivilNonSteelViewModelIntegration)))
                {
                    var response = _unitOfWorkService.InternalApiService.AddCivilInstallation(addCivilNonSteel, Helpers.Constants.CivilType.TLIcivilNonSteel.ToString(), SiteCode, ConnectionString, TaskId);
                    return Ok(response);
                }
                else
                {
                    var ErrorMessages = from state in ModelState.Values
                                        from error in state.Errors
                                        select error.ErrorMessage;
                    return Ok(new Response<AddCivilNonSteelViewModelIntegration>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
                }
            }
            return Ok(new Response<AddCivilNonSteelViewModelIntegration>(true, null, null, "There is no space on the site", (int)Helpers.Constants.ApiReturnCode.fail));
        }
        [HttpGet("GetCivilWithLegsById")]
        [ProducesResponseType(200, Type = typeof(ObjectInstAtts))]
        public IActionResult GetCivilWithLegsById(int CivilId)
        {
            var response = _unitOfWorkService.InternalApiService.GetCivilInstallationById(CivilId, Helpers.Constants.CivilType.TLIcivilWithLegs.ToString());
            return Ok(response);
        }
        [HttpGet("GetCivilWithoutLegsById")]
        [ProducesResponseType(200, Type = typeof(ObjectInstAtts))]
        public IActionResult GetCivilWithoutLegsById(int CivilId)
        {
            var response = _unitOfWorkService.InternalApiService.GetCivilInstallationById(CivilId, Helpers.Constants.CivilType.TLIcivilWithoutLeg.ToString());
            return Ok(response);
        }
        [HttpGet("GetCivilNonSteelById")]
        [ProducesResponseType(200, Type = typeof(ObjectInstAtts))]
        public IActionResult GetCivilNonSteelById(int CivilId)
        {
            var response = _unitOfWorkService.InternalApiService.GetCivilInstallationById(CivilId, Helpers.Constants.CivilType.TLIcivilNonSteel.ToString());
            return Ok(response);
        }

        [HttpPost("EditCivilWithLegs")]
        [ProducesResponseType(200, Type = typeof(EditCivilWithLegsViewModelIntegration))]
        public async Task<IActionResult> EditCivilWithLegs([FromBody] EditCivilWithLegsViewModelIntegration CivilWithLeg,int TaskId)
        {
            if (TryValidateModel(CivilWithLeg, nameof(EditCivilWithLegsViewModel)))
            {
                var response = await _unitOfWorkService.InternalApiService.EditCivilInstallation(CivilWithLeg, Helpers.Constants.CivilType.TLIcivilWithLegs.ToString(), TaskId);
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
        public async Task<IActionResult> EditCivilWithoutLegs([FromBody] EditCivilWithoutLegViewModel CivilWithoutLeg,int TaskId)
        {
            if (TryValidateModel(CivilWithoutLeg, nameof(EditCivilWithoutLegViewModel)))
            {
                var response = await _unitOfWorkService.InternalApiService.EditCivilInstallation(CivilWithoutLeg, Helpers.Constants.CivilType.TLIcivilWithoutLeg.ToString(), TaskId);
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
        public async Task<IActionResult> EditCivilNonSteel([FromBody] EditCivilNonSteelViewModel CivilNonSteel,int TaskId)
        {
            if (TryValidateModel(CivilNonSteel, nameof(EditCivilNonSteelViewModel)))
            {
                var response = await _unitOfWorkService.InternalApiService.EditCivilInstallation(CivilNonSteel, Helpers.Constants.CivilType.TLIcivilNonSteel.ToString(), TaskId);
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
        [HttpPost("DismantleCivil")]

        public IActionResult DismantleCivil(string SiteCode, int CivilId, string CivilName,int TaskId)
        {
            var response = _unitOfWorkService.InternalApiService.DismantleCivil(SiteCode, CivilId, CivilName, TaskId);
            return Ok(response);

        }
    }
}