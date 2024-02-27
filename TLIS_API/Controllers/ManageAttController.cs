using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TLIS_API.Middleware.WorkFlow;
using TLIS_DAL.Helper;
using TLIS_DAL.Helper.Filters;
using TLIS_DAL.Helpers;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModels.AttributeActivatedDTOs;
using TLIS_DAL.ViewModels.OwnerDTOs;
using TLIS_DAL.ViewModels.TablesNamesDTOs;
using TLIS_Service.Helpers;
using TLIS_Service.ServiceBase;

namespace TLIS_API.Controllers
{
    //[ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
    [ServiceFilter(typeof(LogFilterAttribute))]
    [Route("api/[controller]")]
    [ApiController]
    public class ManageAttController : ControllerBase
    {
        IUnitOfWorkService _unitOfWorkService;

        public ManageAttController(IUnitOfWorkService unitOfWorkService)
        {
            _unitOfWorkService = unitOfWorkService;
        }
        [HttpPost("EditStaticAtt")]
        [ProducesResponseType(200, Type = typeof(EditAttributeActivatedViewModel))]
        public async Task<IActionResult> EditStaticAtt([FromBody] EditAttributeActivatedViewModel editAtt, int? CivilWithoutLegCategoryId)
        {
            if (TryValidateModel(editAtt, nameof(EditAttributeActivatedViewModel)))
            {
                var response = await _unitOfWorkService.AttributeActivatedService.EditAttributeActivated(editAtt, CivilWithoutLegCategoryId);
                return Ok(response);
            }
            else
            {
                var ErrorMessages = from state in ModelState.Values
                                    from error in state.Errors
                                    select error.ErrorMessage;
                return Ok(new Response<EditOwnerViewModel>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
            }
        }
        [HttpPost("GetStaticAtts")]
        [ProducesResponseType(200, Type = typeof(List<EditAttributeActivatedViewModel>))]
        public IActionResult GetStaticAtts(int? CivilWithoutLegCategoryId, string AttributeName, [FromQuery] ParameterPagination parameters, string TableName)
        {
            var response = _unitOfWorkService.AttributeActivatedService.GetStaticAtts(CivilWithoutLegCategoryId, AttributeName, parameters, TableName);
            return Ok(response);
        }

        [HttpPost("getById/{id}")]
        [ProducesResponseType(200, Type = typeof(AttributeActivatedViewModel))]
        public IActionResult getById(int id)
        {
            var response = _unitOfWorkService.AttributeActivatedService.GetById(id);
            return Ok(response);
        }
        [HttpPost("Disable")]
        [ProducesResponseType(200, Type = typeof(AttributeActivatedViewModel))]
        public async Task<IActionResult> Disable(int AttributeActivatedId, int? CivilWithoutLegCategoryId)
        {
            var response = await _unitOfWorkService.AttributeActivatedService.Disable(AttributeActivatedId, CivilWithoutLegCategoryId);
            return Ok(response);
        }
        [HttpPost("GetTableNameByAttributeId")]
        [ProducesResponseType(200, Type = typeof(TablesNamesViewModel))]
        public IActionResult GetTableNameByAttributeId(int AttributeId, bool IsDynamic)
        {
            var response = _unitOfWorkService.AttributeActivatedService.GetTableNameByAttributeId(AttributeId, IsDynamic);
            return Ok(response);
        }
        // Response<I> 
        [HttpPost("GetStaticAttsWithoutPagination")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<TLIattributeActivated>))]
        public IActionResult GetStaticAttsWithoutPagination(int? CivilWithoutLegCategoryId, string AttributeName, string TableName)
        {
            var response = _unitOfWorkService.AttributeActivatedService.GetStaticAttsWithoutPagination(CivilWithoutLegCategoryId, AttributeName, TableName);
            return Ok(response);
        }
        [HttpPost("RequiredNOTRequired")]
        [ProducesResponseType(200, Type = typeof(AttributeActivatedViewModel))]
        public async Task<IActionResult> RequiredNOTRequired(int AttributeActivatedId, int? CivilWithoutLegCategoryId)
        {
            var response = await _unitOfWorkService.AttributeActivatedService.RequiredNOTRequired(AttributeActivatedId, CivilWithoutLegCategoryId);
            return Ok(response);
        }
    }
}