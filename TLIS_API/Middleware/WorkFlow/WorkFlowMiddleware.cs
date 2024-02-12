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
using AutoMapper;
using TLIS_DAL.ViewModels.wf;
using Azure;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Engineering;
using TLIS_DAL.ViewModels.SiteStatusDTOs;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using TLIS_DAL.ViewModels.SiteDTOs;
using Microsoft.Extensions.DependencyInjection;

namespace TLIS_API.Middleware.WorkFlow
{
    public class WorkFlowMiddleware : IActionFilter
    {
        private readonly IConfiguration _configuration;
        private readonly TLIS_DAL.ApplicationDbContext db;
        private IMapper _mapper;
        IServiceProvider Services;
        public WorkFlowMiddleware(ApplicationDbContext _ApplicationDbContext, IConfiguration configuration, IMapper mapper, IServiceProvider service)
        {
            db = _ApplicationDbContext;
            _configuration = configuration;
            _mapper = mapper;
            Services = service;
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
                            if (userPermission.Result!=null) 
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
            public List<MetaLinkViewDto> Result { get; set; }
            public int Id { get; set; }
            public object Exception { get; set; }
            public int Status { get; set; }
            public bool IsCanceled { get; set; }
            public bool IsCompleted { get; set; }
            public bool IsCompletedSuccessfully { get; set; }
            public int CreationOptions { get; set; }
            public object AsyncState { get; set; }
            public bool IsFaulted { get; set; }
        }

        private static readonly HttpClient _httpClient = new HttpClient();
        private async Task<PermissionResult> GetPermissionByTask(int TaskId)
        {
            var ExternalApi = _configuration["ExternalApi"];
            using (var scope = Services.CreateScope())
            {
                IMapper _Mapper = scope.ServiceProvider.GetRequiredService<IMapper>();
                string apiUrl = $"{ExternalApi}/api/ActionManagement/GetPermissionByTask?UserId={TaskId}";
                int maxRetries = 1; // Number of retries
                int retryDelayMilliseconds = 180000; // 3 minutes in milliseconds
                for (int retryCount = 0; retryCount < maxRetries; retryCount++)
                {
                    try
                    {
                        HttpResponseMessage response = await _httpClient.PostAsync(apiUrl, null);

                        if (response.IsSuccessStatusCode)
                        {
                            string responseBody = await response.Content.ReadAsStringAsync();

                            if (responseBody != null)
                            {
                                var rootObject = JsonSerializer.Deserialize<SumbitTaskByTLI>(responseBody, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                                var res = _Mapper.Map<PermissionResult>(rootObject);
                                return res;
                            }
                        }
                        else
                        {
                            Console.WriteLine($"API request failed with status code: {response.StatusCode}");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"An error occurred: {ex.Message}");
                    }
                    await Task.Delay(retryDelayMilliseconds);
                }

                    return null;

            }
        }

        void IActionFilter.OnActionExecuting(ActionExecutingContext context)
        {
           
        }
    }
}

