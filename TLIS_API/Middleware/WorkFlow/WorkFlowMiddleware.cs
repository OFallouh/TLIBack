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
using System.Threading.Tasks;
using TLIS_DAL.ViewModels.SiteDTOs;
using Microsoft.Extensions.DependencyInjection;
using static TLIS_Repository.Repositories.SiteRepository;
using static TLIS_Service.Services.SiteService;
using System.Text.Json;
using Newtonsoft.Json;



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

       
        void IActionFilter.OnActionExecuting(ActionExecutingContext context)
        {
            string? userId = null;
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

                try
                {
                    tokenHandler.ValidateToken(token, new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ClockSkew = TimeSpan.Zero
                    }, out SecurityToken validatedToken);

                    var jwtToken = (JwtSecurityToken)validatedToken;
                     userId = jwtToken.Claims.FirstOrDefault(x => x.Type == "sub").Value;
                }
                catch (SecurityTokenExpiredException)
                {
                    context.Result = new UnauthorizedObjectResult("401 Unauthorized");
                    return;
                }
                string apiPath = context.HttpContext.Request.Path;
                string[] segments = apiPath.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
                int lastSegmentIndex = segments.Length - 1; 
                string desiredPath = string.Join("/", segments.Take(lastSegmentIndex));
            
            
                string actionName = context.RouteData.Values["action"].ToString();
                var session = db.TLIsession.FirstOrDefault(x => x.UserId == Convert.ToInt64(userId) && x.IP == clientIPAddress && x.LoginDate<DateTime.Now);
                var userIdInt64 = Convert.ToInt32(userId);
                if (session != null)
                {
                    var WorkFlowMode = _configuration["WorkFlowMode"];
                    if (WorkFlowMode.ToLower() == "true")
                    {
                        if (!actionName.ToLower().StartsWith("get"))
                        {
                            if (context.HttpContext.Request.Query.ContainsKey("TaskId"))
                            {
                                var taskId = context.HttpContext.Request.Query["TaskId"];

                                if (!string.IsNullOrEmpty(taskId))
                                {
                                    if (int.TryParse(taskId, out int taskIdInt))
                                    {
                                        var TaskInfo = GetTaskById(taskIdInt);
                                        if (TaskInfo.Result != null)
                                        {
                                            if (TaskInfo.Result.result != null)
                                            {
                                                if (TaskInfo.Result.result.MetaLink.Api != null && TaskInfo.Result.result.AssignToUserId != 0 && TaskInfo.Result.result.Status != null)
                                                {
                                                    string[] ApiParts = TaskInfo.Result.result.MetaLink.Api.Split('/');
                                                    string[] apiPathParts = apiPath.Split('/');

                                                    // Convert the first three parts of apiPath to lowercase
                                                    for (int i = 0; i < 3 && i < apiPathParts.Length; i++)
                                                    {
                                                        apiPathParts[i] = apiPathParts[i].ToLower();
                                                    }

                                                    string lowerCaseApiPath = string.Join("/", apiPathParts.Take(4));

                                                    // Check equality
                                                    bool result = TaskInfo.Result.result.MetaLink.Api.ToLower() == lowerCaseApiPath.ToLower();

                                                    if (result == true && TaskInfo.Result.result.AssignToUserId == userIdInt64 && TaskInfo.Result.result.Status == Task_Status_Enum.Open)
                                                    {
                                                        context.Result = context.Result;
                                                        return;

                                                    }
                                                    else
                                                    {
                                                        context.Result = new UnauthorizedObjectResult("401 Unauthorized");
                                                        return;
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                context.Result = new UnauthorizedObjectResult(TaskInfo.Result.errorMessage);
                                                return;
                                            }

                                        }
                                        else
                                        {
                                            context.Result = new UnauthorizedObjectResult(TaskInfo.Exception);
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
                        else
                        {
                            context.Result = context.Result;
                            return;
                        }

                    }
                    else
                    {
                        context.Result = context.Result;
                        return;
                    }

                }
                else
                {
                    context.Result = new UnauthorizedObjectResult("401 Unauthorized");
                    return;
                }


            }
            else
            {
                context.Result = new UnauthorizedObjectResult("401 Unauthorized");
                return;

            }
        }

        public class TaskInfo
        {
            public int Id { get; set; }
            public string Status { get; set; }
            public string Method { get; set; }
            public string Result { get; set; }
            // Add other properties as needed
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
        }
        private static readonly HttpClient _httpClient = new HttpClient();
        private async Task<SumbitTaskByTLI> GetTaskById(int TaskId)
        {
            var ExternalApi = _configuration["ExternalApi"];
            using (var scope = Services.CreateScope())
            {
                SumbitTaskByTLI sumbitTaskByTLI = new SumbitTaskByTLI();
                IMapper _Mapper = scope.ServiceProvider.GetRequiredService<IMapper>();
                string apiUrl = $"{ExternalApi}/api/TicketManagement/TaskInfo?TaskId={TaskId}";
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
                                var sumbitTaskByT= JsonConvert.DeserializeObject<SumbitTaskByTLI>(responseBody);

                                return sumbitTaskByT;
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
    }
}

