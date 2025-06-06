﻿using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using TLIS_API.Helpers;
using TLIS_API.Middleware.WorkFlow;
using TLIS_DAL.Helper;
using TLIS_DAL.Helper.Filters;
using TLIS_DAL.Helpers;
using TLIS_DAL.ViewModelBase;
using TLIS_DAL.ViewModels.CivilWithLegLibraryDTOs;
using TLIS_DAL.ViewModels.DynamicAttDTOs;
using TLIS_DAL.ViewModels.DynamicAttInstValueDTOs;
using TLIS_DAL.ViewModels.MW_ODUDTOs;
using TLIS_DAL.ViewModels.MW_ODULibraryDTOs;
using TLIS_DAL.ViewModels.MW_OtherLibraryDTOs;
using TLIS_Service.Helpers;
using TLIS_Service.ServiceBase;

namespace TLIS_API.Controllers.LoadLibrary
{
    //[ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
    [ServiceFilter(typeof(LogFilterAttribute))]
    [Route("api/[controller]")]
   
    public class MW_ODULibraryController : Controller
    {
        private readonly IUnitOfWorkService _unitOfWorkService;
        private readonly IConfiguration _configuration;
        public MW_ODULibraryController(IUnitOfWorkService unitOfWorkService, IConfiguration configuration)
        {
            _unitOfWorkService = unitOfWorkService;
            _configuration = configuration;
        }

        [HttpPost("getAll")]
        [ProducesResponseType(200, Type = typeof(List<MW_ODULibraryViewModel>))]
        public IActionResult GetMW_ODULibrary([FromBody]List<FilterObjectList> filters, bool WithFilterData, ParameterPagination parameters)
        {
            var response = _unitOfWorkService.MWLibraryService.get_MW_ODU_LibrariesAsync(filters, WithFilterData, parameters);
            return Ok(response);
        }
        [HttpPost("GetMWODULibrariesEnabledAtt")]
        [ProducesResponseType(200, Type = typeof(Response<ReturnWithFilters<object>>))]
        public IActionResult GetMW_ODULibraries()
        {
            string ConnectionString = _configuration["ConnectionStrings:ActiveConnection"];
            var response = _unitOfWorkService.MWLibraryService.GetMWODULibrariesEnabledAtt(ConnectionString);
            return Ok(response);
        }
        [HttpGet("getById/{id}")]
        [ProducesResponseType(200, Type = typeof(AllItemAttributes))]
        public IActionResult GetODULibrary(int id)
        {
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
            var ConnectionString = _configuration["ConnectionStrings:ActiveConnection"];
            var response = _unitOfWorkService.MWLibraryService.GetById(id, Helpers.Constants.LoadSubType.TLImwODULibrary.ToString(), userId,false);
            return Ok(response);
        }
        [HttpPost("AddMWODULibrary")]
        [ProducesResponseType(200, Type = typeof(ADDMWODULibraryObject))]
        public IActionResult AddMW_ODULibrary([FromBody] ADDMWODULibraryObject addMW_ODULibraryViewModel)
        {
            if (TryValidateModel(addMW_ODULibraryViewModel, nameof(ADDMWODULibraryObject)))
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
                var response = _unitOfWorkService.MWLibraryService.AddMWODULibrary(userId,Helpers.Constants.LoadSubType.TLImwODULibrary.ToString(), addMW_ODULibraryViewModel, ConnectionString,false);
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
        [HttpPost("EditMWODULibrary")]
        [ProducesResponseType(200, Type = typeof(EditMWODULibraryObject))]
        public async Task<IActionResult> EditMW_ODULibrary([FromBody] EditMWODULibraryObject editMW_RFULibraryViewModel)
        {
            if (TryValidateModel(editMW_RFULibraryViewModel, nameof(EditMWODULibraryObject)))
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
                var response = await _unitOfWorkService.MWLibraryService.EditMWODULibrary(userId, editMW_RFULibraryViewModel, Helpers.Constants.LoadSubType.TLImwODULibrary.ToString(), ConnectionString,false);
                return Ok(response);
            }
            else
            {
                var ErrorMessages = from state in ModelState.Values
                                    from error in state.Errors
                                    select error.ErrorMessage;
                return Ok(new Response<EditMW_ODULibraryViewModel>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
            }
        }

        [HttpPost("DisableMW_ODULibrary/{Id}")]
        [ProducesResponseType(200, Type = typeof(MW_ODULibraryViewModel))]
        public async Task<IActionResult> DisableMW_ODULibrary(int Id)
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
            var response = await _unitOfWorkService.MWLibraryService.Disable(Id, Helpers.Constants.LoadSubType.TLImwODULibrary.ToString(), ConnectionString, userId);
            return Ok(response);
        }

        [HttpGet("GetForAddMWODULibrary")]
        [ProducesResponseType(200, Type = typeof(Response<GetForAddCivilLibrarybject>))]
        public IActionResult GetForAddMWODULibrary()
        {
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
            var ConnectionString = _configuration["ConnectionStrings:ActiveConnection"];
            var response = _unitOfWorkService.MWLibraryService.GetForAdd(Helpers.Constants.LoadSubType.TLImwODULibrary.ToString(), userId, false);
            return Ok(response);
        }
        [HttpPost("DeleteMW_ODULibrary/{Id}")]
        [ProducesResponseType(200, Type = typeof(MW_ODULibraryViewModel))]
        public async Task<IActionResult> DeleteMW_ODULibrary(int Id)
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
            var response = await _unitOfWorkService.MWLibraryService.Delete(Id, Helpers.Constants.LoadSubType.TLImwODULibrary.ToString(), ConnectionString, userId);
            return Ok(response);
        }
    }
}