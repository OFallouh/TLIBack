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
using TLIS_DAL.Helper;
using TLIS_DAL.Helper.Filters;
using TLIS_DAL.Helpers;
using TLIS_DAL.ViewModelBase;
using TLIS_DAL.ViewModels.AllCivilInstDTOs;
using TLIS_DAL.ViewModels.CivilLoadsDTOs;
using TLIS_DAL.ViewModels.CivilWithLegsDTOs;
using TLIS_DAL.ViewModels.DependencyDTOs;
using TLIS_DAL.ViewModels.DynamicAttDTOs;
using TLIS_DAL.ViewModels.LoadPartDTOs;
using TLIS_DAL.ViewModels.RadioAntennaDTOs;
using TLIS_DAL.ViewModels.RadioAntennaLibraryDTOs;
using TLIS_DAL.ViewModels.RadioOtherDTOs;
using TLIS_DAL.ViewModels.RadioOtherLibraryDTOs;
using TLIS_DAL.ViewModels.RadioRRUDTOs;
using TLIS_DAL.ViewModels.RadioRRULibraryDTOs;
using TLIS_DAL.ViewModels.SideArmDTOs;
using TLIS_DAL.ViewModels.SiteDTOs;
using TLIS_DAL.ViewModels.TablesNamesDTOs;
using TLIS_Service.Helpers;
using TLIS_Service.ServiceBase;
using static TLIS_API.Helpers.Constants;

namespace TLIS_API.Controllers
{
    //[ServiceFilter(typeof(LogFilterAttribute))]
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

        [ServiceFilter(typeof(ExternalSystemFilter))]
        [HttpPost("GetCivilsInstalledonSite")]
        [ProducesResponseType(200, Type = typeof(AllCivilInstallationViewModel))]
        public IActionResult GetCivilsBySiteCode([FromQuery] SiteFilter BaseFilter, CombineFilters CombineFilters, bool WithFilterData, [FromQuery] ParameterPagination parameterPagination)
        {
            var response = _unitOfWorkService.InternalApiService.GetCivilsBySiteCode( BaseFilter, CombineFilters, WithFilterData,  parameterPagination);
            return Ok(response);
        }

        //ServiceFilter(typeof(ExternalSystemFilter))]
        [HttpPost("GetSideArmsInstalledonCivil")]
        [ProducesResponseType(200, Type = typeof(ObjectInstAttForSideArm))]
        public IActionResult GetSideArmsBySiteCode([Required] string SiteCode, int? CivilId, int? LegId, float? MinAzimuth, float? MaxAzimuth, float? MinHeightBase, float? MaxHeightBase)
        {
            var response = _unitOfWorkService.InternalApiService.GetSideArmsBySiteCode(SiteCode, CivilId, LegId, MinAzimuth, MaxAzimuth, MinHeightBase, MaxHeightBase);
            return Ok(response);
        }
        [ServiceFilter(typeof(ExternalSystemFilter))]
        [HttpPost("GetLibraryforSpecificType")]
        [ProducesResponseType(200, Type = typeof(Response<ReturnWithFilters<object>>))]
        public IActionResult GetLibraryforSpecificType(string TableNameLibrary, int CategoryId, [FromBody] CombineFilters CombineFilters, bool WithFilterData, [FromQuery] ParameterPagination parameterPagination)
        {
            var response = _unitOfWorkService.InternalApiService.GetLibraryforSpecificType(TableNameLibrary,CategoryId, CombineFilters,WithFilterData,parameterPagination);
            return Ok(response);
        }
        [ServiceFilter(typeof(ExternalSystemFilter))]
        [HttpPost("GetAllSitesDetails")]
        [ProducesResponseType(200, Type = typeof(List<SiteViewModel>))]
        public IActionResult GetAllSitesDetails([FromQueryAttribute] ParameterPagination parameterPagination, [FromBody] List<FilterObjectList> filters)
        {
            var response = _unitOfWorkService.InternalApiService.GetAllSitesDetails(parameterPagination, filters);
            return Ok(response);
        }
        [ServiceFilter(typeof(ExternalSystemFilter))]
        [HttpGet("GetAllLoadonSitebyPartandType")]
        [ProducesResponseType(200, Type = typeof(Response<LoadsDto>))]
        public IActionResult GetAllLoadonSitebyPartandType([Required]String SiteCode, string PartName, string TypeName)
        {
            var response = _unitOfWorkService.InternalApiService.GetAllLoadonSitebyPartandType(SiteCode, PartName, TypeName);
            return Ok(response);

        }
        [ServiceFilter(typeof(ExternalSystemFilter))]
        [HttpPost("GetAlOtherInventoryonSitebyType")]
        [ProducesResponseType(200, Type = typeof(Response<ReturnWithFilters<object>>))]
        public IActionResult GetAlOtherInventoryonSitebyType([Required]string OtherInventoryTypeName, [FromQuery] SiteFilter BaseFilter, bool WithFilterData, [FromBody] CombineFilters CombineFilters, [FromQuery] ParameterPagination parameterPagination, string LibraryType)
        {
            var response = _unitOfWorkService.InternalApiService.GetAlOtherInventoryonSitebyType(OtherInventoryTypeName, BaseFilter,WithFilterData,CombineFilters ,parameterPagination, LibraryType);
            return Ok(response);
        }
        //[ServiceFilter(typeof(ExternalSystemFilter))]
        [HttpGet("GetAllItemsonSite ")]
        [ProducesResponseType(200, Type = typeof(Response<List<ListOfCivilLoads>>))]
        public IActionResult GetAllItemsonSite(string SiteCode)
        {
            var response = _unitOfWorkService.InternalApiService.GetAllItemsonSite(SiteCode);
            return Ok(response);
        }
        [ServiceFilter(typeof(ExternalSystemFilter))]
        [HttpPost("GetConfigurationTablesInstallation ")]
        [ProducesResponseType(200, Type = typeof(Response<ReturnWithFilters<object>>))]
        public IActionResult GetConfigurationTables([FromQuery] SiteFilter BaseFilters,[Required] string TableNameInstallation, int CategoryId, bool WithFilterData, [FromBody] CombineFilters CombineFilters, [FromQuery] ParameterPagination parameterPagination, string LibraryType)
        {
            var response = _unitOfWorkService.InternalApiService.GetConfigurationTables(BaseFilters, TableNameInstallation, CategoryId, WithFilterData, CombineFilters,  parameterPagination, LibraryType);
            return Ok(response);

        }
        [ServiceFilter(typeof(ExternalSystemFilter))]
        [HttpPost("GetConfigurationAttributes")]
        [ProducesResponseType(200, Type = typeof(Response<List<BassAttViewModel>>))]
        public IActionResult GetConfigurationAttributes(string TableName, bool IsDynamic, int CategoryId)
        {
            var response = _unitOfWorkService.InternalApiService.GetConfigurationAttributes(TableName, IsDynamic, CategoryId);
            return Ok(response);

        }
       // [ServiceFilter(typeof(ExternalSystemFilter))]
        [HttpPost("AddDynamicAttLibrary")]
        [ProducesResponseType(200, Type = typeof(AddDynamicAttViewModel))]
        public IActionResult AddDynamicAtts([FromBody] AddDependencyViewModel addDependencyView)
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
       // [ServiceFilter(typeof(ExternalSystemFilter))]
        [HttpPost("AddDynamicAttInstallation")]
        [ProducesResponseType(200, Type = typeof(List<AddDependencyInstViewModel>))]
        public IActionResult AddLibDynamicAttLIns([FromBody] AddDependencyInstViewModel addDependencyInstViewModel)
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

       // [ServiceFilter(typeof(ExternalSystemFilter))]
        [HttpPost("EditDynamicAttLibraryAndInstallation")]
        [ProducesResponseType(200, Type = typeof(DynamicAttViewModel))]
        public async Task<IActionResult> EditDynamicAtt([FromBody] EditDynamicAttViewModel dynamicAttViewModel)
        {
            var response = await _unitOfWorkService.InternalApiService.Edit(dynamicAttViewModel);
            return Ok(response);
        }
       [ServiceFilter(typeof(ExternalSystemFilter))]
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
       [ServiceFilter(typeof(ExternalSystemFilter))]
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
        [ServiceFilter(typeof(ExternalSystemFilter))]
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
        [ServiceFilter(typeof(ExternalSystemFilter))]
        [HttpGet("GetForAddRadioLibrary")]
        [ProducesResponseType(200, Type = typeof(List<AllAtributes>))]
        public IActionResult GetForAddLibrary(string TableName)
        {
            var response = _unitOfWorkService.InternalApiService.GetForAdd(TableName);
            return Ok(response);
        }
        [ServiceFilter(typeof(ExternalSystemFilter))]
        [HttpGet("GetAttForAddRadioInstallation")]
        [ProducesResponseType(200, Type = typeof(ObjectAttributeInst))]
        public IActionResult GetAttForAdd(string RadioInstType, int LibId, string SiteCode)
        {
            var response = _unitOfWorkService.InternalApiService.GetAttForAdd(RadioInstType, LibId, SiteCode);
            return Ok(response);
        }
        [ServiceFilter(typeof(ExternalSystemFilter))]
        [HttpPost("AddRadioAntennaInstallation")]
        [ProducesResponseType(200, Type = typeof(AddRadioAntennaIntegration))]
        public IActionResult AddRadioAntennaInstallation([FromBody] AddRadioAntennaIntegration addRadioAntenna, string SiteCode )
        {
           
            try
            {
                
                if (addRadioAntenna.TLIcivilLoads.sideArmId == 0)
                    addRadioAntenna.TLIcivilLoads.sideArmId = null;
                var ConnectionString = _configuration["ConnectionStrings:ActiveConnection"];      
                if (TryValidateModel(addRadioAntenna, nameof(AddRadioAntennaIntegration)))
                {
                    var response = _unitOfWorkService.InternalApiService.AddRadioInstallation(addRadioAntenna, Helpers.Constants.LoadSubType.TLIradioAntenna.ToString(), SiteCode, ConnectionString);

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
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [ServiceFilter(typeof(ExternalSystemFilter))]
        [HttpPost("AddRadioRRUInstallation")]
        [ProducesResponseType(200, Type = typeof(AddRadioRRUIntegration))]
        public IActionResult AddRadioRRUInstallation([FromForm] AddRadioRRUIntegration addRadioRRU, string SiteCode)
        {
            var asset = _configuration["assets"];

            string contentRootPath = _configuration["StoreFiles"];

            string AttachFolder = Path.Combine(contentRootPath, "AttachFiles");

            if (!Directory.Exists(AttachFolder))
            {
                DirectoryInfo di = Directory.CreateDirectory(AttachFolder);

            }

            if (addRadioRRU.TLIcivilLoads.sideArmId == 0)
                addRadioRRU.TLIcivilLoads.sideArmId = null;
            var ConnectionString = _configuration["ConnectionStrings:ActiveConnection"];
            if (TryValidateModel(addRadioRRU, nameof(AddRadioRRUIntegration)))
            {
                var response = _unitOfWorkService.InternalApiService.AddRadioInstallation( addRadioRRU, LoadSubType.TLIradioRRU.ToString(), SiteCode, ConnectionString);
                if (response.Succeeded==true)
                {
                    if (addRadioRRU.file != null)
                    {
                        foreach (var f in addRadioRRU.file)
                        {
                            var response2 = _unitOfWorkService.InternalApiService.AttachFile(f, 1, LoadSubType.TLIradioRRU.ToString(), f.Name,
                           SiteCode, response.Count.ToString(), LoadSubType.TLIradioRRU.ToString(), ConnectionString, "NA", asset);
                        }
                    }
                }
               
                           
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
        [ServiceFilter(typeof(ExternalSystemFilter))]
        [HttpPost("AddRadioOtherInstallation")]
        [ProducesResponseType(200, Type = typeof(AddRadioOtherIntegration))]
        public IActionResult AddRadioOtherInstallation([FromBody] AddRadioOtherIntegration addRadioOther, string SiteCode)
        {
            if (addRadioOther.TLIcivilLoads.sideArmId == 0)
                addRadioOther.TLIcivilLoads.sideArmId = null;
            var ConnectionString = _configuration["ConnectionStrings:ActiveConnection"];
            if (TryValidateModel(addRadioOther, nameof(AddRadioOtherIntegration)))
            {
                var response = _unitOfWorkService.InternalApiService.AddRadioInstallation(addRadioOther, Helpers.Constants.LoadSubType.TLIradioOther.ToString(), SiteCode, ConnectionString);
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


        
    }
    
}
