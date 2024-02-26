using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TLIS_API.Middleware.WorkFlow;
using TLIS_DAL.Helpers;
using TLIS_DAL.ViewModels.MW_PortDTOs;
using TLIS_Service.Helpers;
using TLIS_Service.ServiceBase;

namespace TLIS_API.Controllers
{
    [ServiceFilter(typeof(WorkFlowMiddleware))]
    [ServiceFilter(typeof(LogFilterAttribute))]
    [Route("api/[controller]")]
    [ApiController]
    public class MW_PortController : ControllerBase
    {
        private IUnitOfWorkService _unitOfWorkService;
        public MW_PortController(IUnitOfWorkService unitOfWorkService)
        {
            _unitOfWorkService = unitOfWorkService;
        }
        [HttpPost("GetPorts")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<MW_PortViewModel>))]
        public async Task<IActionResult> GetPorts()
        {
            var response = await _unitOfWorkService.MW_PortService.GetPorts();
            return Ok(response);
        }
        [HttpPost("GetById/{Id}")]
        [ProducesResponseType(200, Type = typeof(MW_PortViewModel))]
        public async Task<IActionResult> GetPortById(int Id)
        {
            var response = await _unitOfWorkService.MW_PortService.GetPortById(Id);
            return Ok(response);
        }
        [HttpPost("AddPort")]
        [ProducesResponseType(200, Type = typeof(AddMW_PortViewModel))]
        public async Task<IActionResult> Create([FromBody]AddMW_PortViewModel Port)
        {
            if (TryValidateModel(Port, nameof(AddMW_PortViewModel)))
            {
                var response = await _unitOfWorkService.MW_PortService.Create(Port);
                return Ok(response);
            }
            else
            {
                var ErrorMessages = from state in ModelState.Values
                                    from error in state.Errors
                                    select error.ErrorMessage;
                return Ok(new Response<AddMW_PortViewModel>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
            }
        }
        [HttpPost("UpdatePort")]
        [ProducesResponseType(200, Type = typeof(EditMW_PortViewModel))]
        public async Task<IActionResult> UpdateActor([FromBody]EditMW_PortViewModel Port)
        {
            var response = await _unitOfWorkService.MW_PortService.Update(Port);
            return Ok(response);
        }
    }
}