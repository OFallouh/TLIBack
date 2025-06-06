﻿using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebSockets;
using Microsoft.Extensions.Configuration;
using TLIS_API.Helpers;
using TLIS_API.Middleware.WorkFlow;
using TLIS_DAL.Helper;
using TLIS_DAL.Helper.Filters;
using TLIS_DAL.Helpers;
using TLIS_DAL.ViewModelBase;
using TLIS_DAL.ViewModels.CivilNonSteelDTOs;
using TLIS_DAL.ViewModels.CivilNonSteelLibraryDTOs;
using TLIS_DAL.ViewModels.DynamicAttDTOs;
using TLIS_DAL.ViewModels.DynamicAttInstValueDTOs;
using TLIS_Service.Helpers;
using TLIS_Service.ServiceBase;
using Constants = TLIS_API.Helpers.Constants;

namespace TLIS_API.Controllers
{
    //[ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
    [ServiceFilter(typeof(LogFilterAttribute))]
    [Route("api/[controller]")]
    public class CivilNonSteelLibraryController : Controller
    {
        private readonly IUnitOfWorkService _unitOfWorkService;
        private readonly IConfiguration _configuration;
        public CivilNonSteelLibraryController(IUnitOfWorkService unitOfWorkService, IConfiguration configuration)
        {
            _unitOfWorkService = unitOfWorkService;
            _configuration = configuration;
        }
        [HttpPost("getAll")]
        [ProducesResponseType(200, Type = typeof(List<CivilNonSteelLibraryViewModel>))]
        public IActionResult GetCivilNonSteelLibrary([FromBody]List<FilterObjectList> filters, [FromQuery]ParameterPagination parameterPagination)
        {
            var response = _unitOfWorkService.CivilLibraryService.getCivilNonSteelLibraries(filters, parameterPagination);
            return Ok(response);
        }
        [HttpPost("GetCivilNonSteelLibrariesEnabledAtt")]
        [ProducesResponseType(200, Type = typeof(Response<ReturnWithFilters<object>>))]
        public IActionResult GetCivilNonSteelLibrariesEnabledAtt()
        {
            string ConnectionString = _configuration["ConnectionStrings:ActiveConnection"];
            var response = _unitOfWorkService.CivilLibraryService.GetCivilNonSteelLibrariesEnabledAtt(ConnectionString);
            return Ok(response);
        }
        [HttpGet("getById/{id}")]
        [ProducesResponseType(200, Type = typeof(CivilNonSteelLibraryViewModel))]
        public IActionResult GetCivilNonSteelLibrary(int id)
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
            var response = _unitOfWorkService.CivilLibraryService.GetCivilNonSteelLibraryById(id, Helpers.Constants.CivilType.TLIcivilNonSteelLibrary.ToString(), userId,false);
            return Ok(response);

        }
        [HttpPost("AddCivilNonSteelLibrary")]
        [ProducesResponseType(200, Type = typeof(AddCivilNonSteelLibraryObject))]
        public IActionResult AddCivilNonSteelLibrary([FromBody] AddCivilNonSteelLibraryObject addCivilNonSteelLibraryViewModel)
        {
            if (TryValidateModel(addCivilNonSteelLibraryViewModel, nameof(AddCivilNonSteelLibraryObject)))
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
                var response = _unitOfWorkService.CivilLibraryService.AddCivilNonSteelLibrary(Helpers.Constants.CivilType.TLIcivilNonSteelLibrary.ToString(), addCivilNonSteelLibraryViewModel, ConnectionString, userId,false);
                return Ok(response);
            }
            else
            {
                var ErrorMessages = from state in ModelState.Values
                                    from error in state.Errors
                                    select error.ErrorMessage;
                return Ok(new Response<AddCivilNonSteelLibraryObject>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
            }
        }
        [HttpPost("EditCivilNonSteelLibrary")]
        [ProducesResponseType(200, Type = typeof(EditCivilNonSteelLibraryObject))]
        public async Task<IActionResult> EditCivilNonSteelLibrary([FromBody] EditCivilNonSteelLibraryObject editCivilNonSteelLibraryViewModel)
        {
            if (TryValidateModel(editCivilNonSteelLibraryViewModel, nameof(EditCivilNonSteelLibraryObject)))
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
                string ConnectionString = _configuration["ConnectionStrings:ActiveConnection"];
                var response = await _unitOfWorkService.CivilLibraryService.EditCivilNonSteelLibrary(editCivilNonSteelLibraryViewModel, Helpers.Constants.CivilType.TLIcivilNonSteelLibrary.ToString(), userId, ConnectionString,false);
                return Ok(response);
            }
            else
            {
                var ErrorMessages = from state in ModelState.Values
                                    from error in state.Errors
                                    select error.ErrorMessage;
                return Ok(new Response<EditCivilNonSteelLibraryViewModel>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
            }
        }
        [HttpPost("DisableCivilNonSteelLibrary")]
        [ProducesResponseType(200, Type = typeof(List<CivilNonSteelLibraryViewModel>))]
        public async Task<IActionResult> DisableCivilNonSteelLibrary(int id)
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
            string ConnectionString = _configuration["ConnectionStrings:ActiveConnection"];
            var response = await _unitOfWorkService.CivilLibraryService.Disable(id, Helpers.Constants.CivilType.TLIcivilNonSteelLibrary.ToString(), userId, ConnectionString);
            return Ok(response);
        }
        [HttpPost("DeleteCivilNonSteelLibrary")]
        [ProducesResponseType(200, Type = typeof(List<CivilNonSteelLibraryViewModel>))]
        public async Task<IActionResult> DeleteCivilNonSteelLibrary(int id)
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
            string ConnectionString = _configuration["ConnectionStrings:ActiveConnection"];
            var response = await _unitOfWorkService.CivilLibraryService.Delete(id, Helpers.Constants.CivilType.TLIcivilNonSteelLibrary.ToString(), userId, ConnectionString);
            return Ok(response);
        }
        [HttpGet("GetForAddCivilNonSteelLibrary")] 
        public IActionResult GetForAddCivilNonSteelLibrary()
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
            var response = _unitOfWorkService.CivilLibraryService.GetForAdd(Helpers.Constants.CivilType.TLIcivilNonSteelLibrary.ToString(), userId,false);
            return Ok(response);
        }

    }
}