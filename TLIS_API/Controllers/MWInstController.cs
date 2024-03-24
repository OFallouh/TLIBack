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
        [ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
        [HttpGet("GetAttForAddMW_BU")]
        [ProducesResponseType(200, Type = typeof(ObjectInstAtts))]
        public IActionResult GetAttForAddMW_BU(int LibId, string SiteCode)
        {
            var response = _unitOfWorkService.MWInstService.GetAttForAdd(Helpers.Constants.LoadSubType.TLImwBU.ToString(), LibId, SiteCode);
            return Ok(response);
        }
        [ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
        [HttpGet("GetAttForAddMW_ODU")]
        [ProducesResponseType(200, Type = typeof(ObjectInstAtts))]
        public IActionResult GetAttForAddMW_ODU(int LibId, string SiteCode, int AllCivilInstId)
        {
            var response = _unitOfWorkService.MWInstService.GetAttForAddForMW_ODUOnly(Helpers.Constants.LoadSubType.TLImwODU.ToString(), LibId, SiteCode, AllCivilInstId);
            return Ok(response);
        }
        [ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
        [HttpGet("GetAttForAddMW_Dish")]
        [ProducesResponseType(200, Type = typeof(ObjectInstAtts))]
        public IActionResult GetAttForAddMW_Dish(int LibId, string SiteCode)
        {
            var response = _unitOfWorkService.MWInstService.GetAttForAdd(Helpers.Constants.LoadSubType.TLImwDish.ToString(), LibId, SiteCode);
            return Ok(response);
        }
        [ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
        [HttpGet("GetAttForAddMW_RFU")]
        [ProducesResponseType(200, Type = typeof(ObjectInstAtts))]
        public IActionResult GetAttForAddMW_RFU(int LibId, string SiteCode)
        {
            var response = _unitOfWorkService.MWInstService.GetAttForAdd(Helpers.Constants.LoadSubType.TLImwRFU.ToString(), LibId, SiteCode);
            return Ok(response);
        }
        [ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
        [HttpGet("GetAttForAddMW_Other")]
        [ProducesResponseType(200, Type = typeof(ObjectInstAtts))]
        public IActionResult GetAttForAddMW_Other(int LibId, string SiteCode)
        {
            var response = _unitOfWorkService.MWInstService.GetAttForAdd(Helpers.Constants.LoadSubType.TLImwOther.ToString(), LibId, SiteCode);
            return Ok(response);
        }
        [ServiceFilter(typeof(WorkFlowMiddleware))]

        [HttpPost("AddMW_BU")]
        [ProducesResponseType(200, Type = typeof(AddMW_BUViewModel))]
        public IActionResult AddMW_BU([FromBody]AddMW_BUViewModel AddMW_BUViewModel, string SiteCode, int TaskId)
        {
            if (AddMW_BUViewModel.TLIcivilLoads.sideArmId == 0)
                AddMW_BUViewModel.TLIcivilLoads.sideArmId = null;
            var ConnectionString = _configuration["ConnectionStrings:ActiveConnection"];
            if (TryValidateModel(AddMW_BUViewModel, nameof(AddMW_BUViewModel)))
            {
                var response = _unitOfWorkService.MWInstService.AddMWInstallation(AddMW_BUViewModel, Helpers.Constants.LoadSubType.TLImwBU.ToString(), SiteCode, ConnectionString, TaskId);
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
        [ServiceFilter(typeof(WorkFlowMiddleware))]
        [HttpPost("AddMW_ODU")]
        [ProducesResponseType(200, Type = typeof(AddMW_ODUViewModel))]
        public IActionResult AddMW_ODU([FromBody]AddMW_ODUViewModel AddMW_ODUViewModel, string SiteCode, int TaskId)
        {
            if (AddMW_ODUViewModel.TLIcivilLoads.sideArmId == 0)
                AddMW_ODUViewModel.TLIcivilLoads.sideArmId = null;
            var ConnectionString = _configuration["ConnectionStrings:ActiveConnection"];
            if (TryValidateModel(AddMW_ODUViewModel, nameof(AddMW_ODUViewModel)))
            {
                var response = _unitOfWorkService.MWInstService.AddMWInstallation(AddMW_ODUViewModel, Helpers.Constants.LoadSubType.TLImwODU.ToString(), SiteCode, ConnectionString, TaskId);
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
        [ProducesResponseType(200, Type = typeof(AddMW_DishViewModel))]
        public IActionResult AddMW_Dish([FromBody]AddMW_DishViewModel AddMW_DishViewModel, string SiteCode, int TaskId)
        {
            if (AddMW_DishViewModel.TLIcivilLoads.sideArmId == 0)
                AddMW_DishViewModel.TLIcivilLoads.sideArmId = null;
            var ConnectionString = _configuration["ConnectionStrings:ActiveConnection"];
            if (TryValidateModel(AddMW_DishViewModel, nameof(AddMW_DishViewModel)))
            {
                var response = _unitOfWorkService.MWInstService.AddMWInstallation(AddMW_DishViewModel, Helpers.Constants.LoadSubType.TLImwDish.ToString(), SiteCode, ConnectionString, TaskId);
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
        [HttpPost("AddMW_RFU")]
        [ProducesResponseType(200, Type = typeof(AddMW_RFUViewModel))]
        public IActionResult AddMW_RFU([FromBody]AddMW_RFUViewModel AddMW_RFUViewModel, string SiteCode, int TaskId)
        {
            if (AddMW_RFUViewModel.TLIcivilLoads.sideArmId == 0)
                AddMW_RFUViewModel.TLIcivilLoads.sideArmId = null;
            if (AddMW_RFUViewModel.MwPortId == 0)
                AddMW_RFUViewModel.MwPortId = null;
            var ConnectionString = _configuration["ConnectionStrings:ActiveConnection"];
            if (TryValidateModel(AddMW_RFUViewModel, nameof(AddMW_RFUViewModel)))
            {
                var response = _unitOfWorkService.MWInstService.AddMWInstallation(AddMW_RFUViewModel, Helpers.Constants.LoadSubType.TLImwRFU.ToString(), SiteCode, ConnectionString, TaskId);
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
        [ServiceFilter(typeof(WorkFlowMiddleware))]
        [HttpPost("AddMW_Other")]
        [ProducesResponseType(200, Type = typeof(AddMw_OtherViewModel))]
        public IActionResult AddMW_Other([FromBody] AddMw_OtherViewModel AddMw_OtherViewModel, string SiteCode, int TaskId)
        {
            if (AddMw_OtherViewModel.TLIcivilLoads.sideArmId == 0)
                AddMw_OtherViewModel.TLIcivilLoads.sideArmId = null;
            var ConnectionString = _configuration["ConnectionStrings:ActiveConnection"];
            if (TryValidateModel(AddMw_OtherViewModel, nameof(AddMw_OtherViewModel)))
            {
                var response = _unitOfWorkService.MWInstService.AddMWInstallation(AddMw_OtherViewModel, Helpers.Constants.LoadSubType.TLImwOther.ToString(), SiteCode, ConnectionString, TaskId);
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

        [ServiceFilter(typeof(WorkFlowMiddleware))]

        [HttpPost("EditMW_BU")]
        [ProducesResponseType(200, Type = typeof(EditMW_BUViewModel))]
        public async Task<IActionResult> EditMW_BU([FromBody]EditMW_BUViewModel MW_BU,int TaskId)
        {
            if (TryValidateModel(MW_BU, nameof(EditMW_BUViewModel)))
            {
                var response = await _unitOfWorkService.MWInstService.EditMWInstallation(MW_BU, Helpers.Constants.LoadSubType.TLImwBU.ToString(), TaskId);
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
        [ServiceFilter(typeof(WorkFlowMiddleware))]
        [HttpPost("EditMW_Dish")]
        [ProducesResponseType(200, Type = typeof(EditMW_DishViewModel))]
        public async Task<IActionResult> EditMW_Dish([FromBody]EditMW_DishViewModel MW_Dish,int TaskId)
        {
            if (TryValidateModel(MW_Dish, nameof(EditMW_DishViewModel)))
            {
                var response = await _unitOfWorkService.MWInstService.EditMWInstallation(MW_Dish, Helpers.Constants.LoadSubType.TLImwDish.ToString(), TaskId);
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
        [ServiceFilter(typeof(WorkFlowMiddleware))]
        [HttpPost("EditMW_ODU")]
        [ProducesResponseType(200, Type = typeof(EditMW_ODUViewModel))]
        public async Task<IActionResult> EditMW_ODU([FromBody]EditMW_ODUViewModel MW_ODU,int TaskId)
        {
            if (TryValidateModel(MW_ODU, nameof(EditMW_ODUViewModel)))
            {
                var response = await _unitOfWorkService.MWInstService.EditMWInstallation(MW_ODU, Helpers.Constants.LoadSubType.TLImwODU.ToString(), TaskId);
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
        [ServiceFilter(typeof(WorkFlowMiddleware))]
        [HttpPost("EditMW_RFU")]
        [ProducesResponseType(200, Type = typeof(EditMW_RFUViewModel))]
        public async Task<IActionResult> EditMW_RFU([FromBody]EditMW_RFUViewModel MW_RFU,int TaskId)
        {
            if (TryValidateModel(MW_RFU, nameof(EditMW_RFUViewModel)))
            {
                var response = await _unitOfWorkService.MWInstService.EditMWInstallation(MW_RFU, Helpers.Constants.LoadSubType.TLImwRFU.ToString(), TaskId);
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
        [ServiceFilter(typeof(WorkFlowMiddleware))]
        [HttpPost("EditMw_Other")]
        [ProducesResponseType(200, Type = typeof(EditMw_OtherViewModel))]
        public async Task<IActionResult> EditMw_Other([FromBody] EditMw_OtherViewModel Mw_Other, int TaskId)
        {
            if (TryValidateModel(Mw_Other, nameof(EditMw_OtherViewModel)))
            {
                var response = await _unitOfWorkService.MWInstService.EditMWInstallation(Mw_Other, Helpers.Constants.LoadSubType.TLImwOther.ToString(), TaskId);
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
        [ServiceFilter(typeof(WorkFlowMiddleware))]
        [HttpPost("DismantleMW_BU")]
        public IActionResult DismantleMW_BU(string sitecode, int LoadId, string LoadName,int TaskId)
        {
            var response = _unitOfWorkService.MWInstService.DismantleLoads(sitecode, LoadId, LoadName, TaskId);
            return Ok(response);
        }
        [ServiceFilter(typeof(WorkFlowMiddleware))]
        [HttpPost("DismantleMW_ODU")]
        public IActionResult DismantleMW_ODU(string sitecode, int LoadId, string LoadName, int TaskId)
        {
            var response = _unitOfWorkService.MWInstService.DismantleLoads(sitecode, LoadId, LoadName, TaskId);
            return Ok(response);
        }
        [ServiceFilter(typeof(WorkFlowMiddleware))]
        [HttpPost("DismantleMW_RFU")]

        public IActionResult DismantleMW_RFU(string sitecode, int LoadId, string LoadName, int TaskId)
        {
            var response = _unitOfWorkService.MWInstService.DismantleLoads(sitecode, LoadId, LoadName, TaskId);
            return Ok(response);
        }
        [ServiceFilter(typeof(WorkFlowMiddleware))]
        [HttpPost("DismantleMW_Dish")]

        public IActionResult DismantleMW_Dish(string sitecode, int LoadId, string LoadName, int TaskId)
        {
            var response = _unitOfWorkService.MWInstService.DismantleLoads(sitecode, LoadId, LoadName, TaskId);
            return Ok(response);
        }
        [ServiceFilter(typeof(WorkFlowMiddleware))]
        [HttpPost("DismantleMW_Other")]

        public IActionResult DismantleMW_Other(string sitecode, int LoadId, string LoadName, int TaskId)
        {
            var response = _unitOfWorkService.MWInstService.DismantleLoads(sitecode, LoadId, LoadName, TaskId);
            return Ok(response);
        }
        [ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
        [HttpGet("GetMW_BUById")]
        [ProducesResponseType(200, Type = typeof(ObjectInstAttsForSideArm))]
        public IActionResult GetMW_BUById(int MW_BU)
        {
            var response = _unitOfWorkService.MWInstService.GetById(MW_BU, Helpers.Constants.LoadSubType.TLImwBU.ToString());
            return Ok(response);
        }
        [ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
        [HttpGet("GetMW_ODUById")]
        [ProducesResponseType(200, Type = typeof(ObjectInstAttsForSideArm))]
        public IActionResult GetMW_ODUById(int MW_ODU)
        {
            var response = _unitOfWorkService.MWInstService.GetById(MW_ODU, Helpers.Constants.LoadSubType.TLImwODU.ToString());
            return Ok(response);
        }
        [ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
        [HttpGet("GetMW_DishById")]
        [ProducesResponseType(200, Type = typeof(ObjectInstAttsForSideArm))]
        public IActionResult GetMW_DishById(int MW_Dish)
        {
            var response = _unitOfWorkService.MWInstService.GetById(MW_Dish, Helpers.Constants.LoadSubType.TLImwDish.ToString());
            return Ok(response);
        }
        [ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
        [HttpGet("GetMW_RFUById")]
        [ProducesResponseType(200, Type = typeof(ObjectInstAttsForSideArm))]
        public IActionResult GetMW_RFUById(int MW_RFU)
        {
            var response = _unitOfWorkService.MWInstService.GetById(MW_RFU, Helpers.Constants.LoadSubType.TLImwRFU.ToString());
            return Ok(response);
        }
        [ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
        [HttpGet("GetMW_OtherById")]
        [ProducesResponseType(200, Type = typeof(ObjectInstAttsForSideArm))]
        public IActionResult GetMWOtherId(int mwOther)
        {
            var response = _unitOfWorkService.MWInstService.GetById(mwOther, Helpers.Constants.LoadSubType.TLImwOther.ToString());
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
    }
}
