using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TLIS_DAL.Helper;
using TLIS_DAL.Helpers;
using TLIS_DAL.ViewModelBase;
using TLIS_Service.Helpers;
using TLIS_Service.ServiceBase;

namespace TLIS_API.Controllers
{
    [ServiceFilter(typeof(LogFilterAttribute))]
    [Route("api/[controller]")]
    [ApiController]
    public class ConfigurationAttsController : ControllerBase
    {
        private IUnitOfWorkService _unitOfWorkService;
        public ConfigurationAttsController(IUnitOfWorkService unitOfWorkService)
        {
            _unitOfWorkService = unitOfWorkService;
        }
        [HttpGet("GetConfigrationTables")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<ConfigurationListViewModel>))]
        public IActionResult GetConfigurationTables(string filterName)
        {
            var response = _unitOfWorkService.ConfigurationAttsService.GetConfigrationTables(filterName);
            return Ok(response);
        }
        [HttpPost("GetAll")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<ConfigurationAttsViewModel>))]
        public IActionResult GetConfigurationAtts(string TableName, [FromQuery]ParameterPagination Pagination)
        {
            var response = _unitOfWorkService.ConfigurationAttsService.GetAll(TableName, Pagination);
            return Ok(response);
        }

        [HttpGet("GetById")]
        [ProducesResponseType(200, Type = typeof(ConfigurationAttsViewModel))]
        public IActionResult GetById(string TableName, int Id)
        {
            var response = _unitOfWorkService.ConfigurationAttsService.GetById(TableName, Id);
            return Ok(response);
        }

        [HttpPost("Add")]
        [ProducesResponseType(200, Type = typeof(ConfigurationAttsViewModel))]
        public IActionResult Add([FromBody]AddConfigrationAttViewModel model)
        {
            if(TryValidateModel(model, nameof(ConfigurationAttsViewModel)))
            {
                var response = _unitOfWorkService.ConfigurationAttsService.Add(model);
                return Ok(response);
            }
            else
            {
                var ErrorMessages = from state in ModelState.Values
                                    from error in state.Errors
                                    select error.ErrorMessage;
                return Ok(new Response<ConfigurationAttsViewModel>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
            }
        }

        [HttpPost("Update")]
        [ProducesResponseType(200, Type = typeof(ConfigurationAttsViewModel))]
        public async Task<IActionResult> Update([FromBody]ConfigurationAttsViewModel model)
        {
            if(TryValidateModel(model, nameof(ConfigurationAttsViewModel)))
            {
                var response = await _unitOfWorkService.ConfigurationAttsService.Update(model);
                return Ok(response);
            }
            else
            {
                var ErrorMessages = from state in ModelState.Values
                                    from error in state.Errors
                                    select error.ErrorMessage;
                return Ok(new Response<ConfigurationAttsViewModel>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
            }
        }
        [HttpPost("Disable")]
        [ProducesResponseType(200, Type = typeof(ConfigurationAttsViewModel))]
        public async Task<IActionResult> Disable( string TableName, int Id)
        {
            var response = await _unitOfWorkService.ConfigurationAttsService.Disable(TableName, Id);
            return Ok(response);
        }
        [HttpPost("Delete")]
        [ProducesResponseType(200, Type = typeof(ConfigurationAttsViewModel))]
        public async Task<IActionResult> Delete(string TableName, int Id)
        {
            var response = await _unitOfWorkService.ConfigurationAttsService.Delete(TableName, Id);
            return Ok(response);
        }
    }
}
