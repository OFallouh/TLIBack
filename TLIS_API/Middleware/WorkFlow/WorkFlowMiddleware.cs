using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using System.Linq;
using System.Text;
using System;
using TLIS_DAL;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.EntityFrameworkCore;
using TLIS_DAL.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.DirectoryServices.Protocols;
using WF_API.Model;
using AutoMapper;
using TLIS_DAL.ViewModels.wf;
using Azure;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Engineering;
using TLIS_DAL.ViewModels.SiteStatusDTOs;

namespace TLIS_API.Middleware.WorkFlow
{
    public class WorkFlowMiddleware : IActionFilter
    {
        private readonly IConfiguration _configuration;
        private readonly TLIS_DAL.ApplicationDbContext db;
        private IMapper _mapper;
        public WorkFlowMiddleware(ApplicationDbContext _ApplicationDbContext, IConfiguration configuration, IMapper mapper)
        {
            db = _ApplicationDbContext;
            _configuration = configuration;
            _mapper = mapper;
        }
        void IActionFilter.OnActionExecuted(ActionExecutedContext context)
        {
            
            var clientIPAddress = context.HttpContext.Connection.RemoteIpAddress.ToString();

            if (context.HttpContext.Request.Headers.TryGetValue("X-Forwarded-For", out var forwardedFor))
            {
                clientIPAddress = forwardedFor.ToString().Split(", ")[0];
            }
            string authHeader = context.HttpContext.Request.Headers["Authorization"];

            if (!string.IsNullOrEmpty(authHeader) && authHeader.ToLower().StartsWith("bearer "))
            {
                string token = authHeader.Substring("Bearer ".Length).Trim();

                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_configuration["JWT:Key"]);

                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero 
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                var userId = jwtToken.Claims.FirstOrDefault(x => x.Type == "sub").Value;
                var session = db.TLIsession.FirstOrDefault(x => x.UserId == Convert.ToInt64(userId) && x.IP== clientIPAddress && x.LoginDate<DateTime.Now);
                if (session != null)
                {
                    var WorkFlowMode = _configuration["WorkFlowMode"];
                    if (WorkFlowMode.ToLower() == "true")
                    {
                        var taskId = context.HttpContext.Request.Query["TaskId"];
                        if (!string.IsNullOrEmpty(taskId))
                        {
                            var userIdInt64 = Convert.ToInt32(userId);
                            var userPermission = GetPermissionByTask(userIdInt64);
                            if (userPermission!=null) 
                            {
                                context.Result = context.Result;
                                return;
                            }
                        }

                    }
                        
                }
                else
                {
                    context.Result = new UnauthorizedObjectResult("401 Unauthorized");
                    return;
                }
                
                
            }
        }
        public class PermissionResult
        {
            public List<MetaLinkViewDto> MetaLinkInfo { get; set; }
            public string ErrorMessage { get; set; }
        }

        public PermissionResult GetPermissionByTask(int UserId)
        {
            try
            {
                List<int> ints=new List<int>();
                var DelegationTasks=db.T_WF_DELEGATIONS.Include(x=>x.Task).ThenInclude(x=>x.PhaseAction).Where(x=>x.AssignToId== UserId && x.Task.Status== Task_Status_Enum .Open&& x.EndDate<DateTime.Now).Select(x=>x.Task.PhaseAction.Id).ToList();
                List<int> PhaseAction = db.T_WF_PHASE_ACTIONS.Where(x => x.UserAssignToId == UserId).Select(x => x.Id).ToList();
                List<int> Tasks = db.T_WF_TASKS.Where(x => PhaseAction.Any(y => y == x.PhaseActionId) && x.Status == Task_Status_Enum.Open).Select(x => x.PhaseActionId).ToList();
                ints.AddRange(Tasks);
                ints.AddRange(DelegationTasks);
                List<int> Actions = db.T_WF_PHASE_ACTIONS.Where(x => ints.Any(y => y == x.Id)).Select(x => x.ActionId).ToList();
                List<T_WF_META_LINK> MetaLink = db.T_WF_META_LINKS.Where(x => Actions.Any(y => y == x.ActionId)).ToList();
                List<MetaLinkViewDto> MetaLinkInfo = _mapper.Map<List<MetaLinkViewDto>>(MetaLink);
                return new PermissionResult { MetaLinkInfo = MetaLinkInfo };
            }
            catch (Exception ex)
            {
                return new PermissionResult { ErrorMessage = ex.Message };
            }


        }

        void IActionFilter.OnActionExecuting(ActionExecutingContext context)
        {
           
        }
    }
}

