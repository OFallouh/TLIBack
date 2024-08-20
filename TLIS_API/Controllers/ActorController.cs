using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TLIS_API.Middleware.ActionFilters;
using TLIS_API.Middleware.WorkFlow;
using TLIS_DAL.Helper;
using TLIS_DAL.Helper.Filters;
using TLIS_DAL.Helpers;
using TLIS_DAL.ViewModels.ActorDTOs;
using TLIS_DAL.ViewModels.GroupDTOs;
using TLIS_Service.Helpers;
using TLIS_Service.ServiceBase;

namespace TLIS_API.Controllers
{
    [ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
    [ServiceFilter(typeof(LogFilterAttribute))]
    [Route("api/[controller]")]
    [ApiController]
    public class ActorController : ControllerBase
    {
        private IUnitOfWorkService _unitOfWorkService;
        public ActorController(IUnitOfWorkService unitOfWorkService)
        {
            _unitOfWorkService = unitOfWorkService;
        }
        [HttpPost("GetActors")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<ActorViewModel>))]
        public IActionResult GetActors([FromBody] List<FilterObjectList> filters)
        {
            var response = _unitOfWorkService.ActorService.GetActors(filters);
            return Ok(response);
        }
        [HttpPost("GetActorByName")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<ActorViewModel>))]
        public IActionResult GetActorsByName(string ActorName)
        {
            var response = _unitOfWorkService.ActorService.GetActorByName(ActorName);
            return Ok(response);
        }
        [HttpPost("AddActor")]
        [ProducesResponseType(200, Type = typeof(ActorViewModel))]
        public async Task<IActionResult> AddActor([FromBody] AddActorViewModel Actor)
        {
            if (TryValidateModel(Actor, nameof(AddActorViewModel)))
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
                var response = await _unitOfWorkService.ActorService.AddActor(Actor, userId);

                return Ok(response);
            }
            else
            {
                var ErrorMessages = from state in ModelState.Values
                                    from error in state.Errors
                                    select error.ErrorMessage;
                return Ok(new Response<ActorViewModel>(true, null, ErrorMessages.ToArray(), null, (int)Helpers.Constants.ApiReturnCode.Invalid));
            }
        }
        [HttpPost("UpdateActor")]
        [ProducesResponseType(200, Type = typeof(ActorViewModel))]
        public async Task<IActionResult> UpdateActor([FromBody] EditActorViewModel Actor)
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
            var response = await _unitOfWorkService.ActorService.UpdateActor(Actor, userId);
            return Ok(response);
        }
        [HttpPost("DeleteActor")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<GroupViewModel>))]
        public async Task<IActionResult> DeleteActor([FromBody] ActorViewModel actorViewModel)
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
            //var userId = HttpContext.Session.GetString("UserId");
            var response = await _unitOfWorkService.ActorService.DeleteActor(actorViewModel,userId);
            return Ok(response);
        }
        [HttpPost("CheckIfActorIsExistInWorkflow")]
        [ProducesResponseType(200, Type = typeof(List<GroupViewModel>))]
        public IActionResult CheckIfActorIsExistInWorkflow(int ActorId)
        {
            var response = _unitOfWorkService.ActorService.CheckIfActorIsExistInWorkflow(ActorId);
            return Ok(response);
        }
        [HttpPost("DeleteActorFromGroups")]
        [ProducesResponseType(200, Type = typeof(ActorViewModel))]
        public async Task<IActionResult> DeleteActorFromGroups([FromBody] ActorViewModel actorViewModel)
        {
            var response = await _unitOfWorkService.ActorService.DeleteActorFromGroups(actorViewModel);
            return Ok(response);
        }
        [HttpPost("GetActorGroups")]
        [ProducesResponseType(200, Type = typeof(List<GroupViewModel>))]
        public IActionResult GetActorGroups(int ActorId)
        {
            var response = _unitOfWorkService.ActorService.GetActorGroups(ActorId);
            return Ok(response);

        }
    }
}