using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Dynamic;
using System.IdentityModel.Tokens.Jwt;
using System;
using TLIS_DAL.Models;
using TLIS_DAL;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using TLIS_DAL.Models;
using TLIS_Repository.Base;
using System.Dynamic;
using System.Diagnostics;
using TLIS_Service.ServiceBase;
using TLIS_DAL;
namespace TLIS_API.Middleware
{
    public class SecurityLogFilter : ActionFilterAttribute
    {
        static public int? UserId { get; set; }
        IServiceCollection _services;
        ServiceProvider _serviceProvider;
        private readonly ILogger<SecurityLogFilter> _logger;
        private readonly ApplicationDbContext _dbContext;
        public static List<ActionParamaters> MyParametersList { get; set; } = new List<ActionParamaters>();
        public class ActionParamaters
        {
            public Guid GuidId { get; set; }
            public IDictionary<string, object> Parameters { get; set; }
        }
        public SecurityLogFilter(IServiceCollection services, ILogger<SecurityLogFilter> logger, ApplicationDbContext context)
        {
            _services = services;
            _serviceProvider = _services.BuildServiceProvider();
            _logger = logger;
            _dbContext = context;
        }
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            Guid GuidId = Guid.Parse(filterContext.HttpContext.TraceIdentifier);
            Trace.CorrelationManager.ActivityId = GuidId;
            IDictionary<string, object> Parameters = filterContext.ActionArguments;
            MyParametersList.Add(new ActionParamaters
            {
                GuidId = GuidId,
                Parameters = Parameters
            });
            string token = filterContext.HttpContext.Request.Headers["Authorization"].ToString();

            if (!string.IsNullOrEmpty(token) && token.ToLower() != "bearer null")
                UserId = Int32.Parse(new JwtSecurityTokenHandler().ReadJwtToken(token.Split(" ")[1]).Claims.ToList()[0].Value);
        }

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            try
            {
                //UnitOfWork UnitOfWork = _serviceProvider.GetService<UnitOfWork>();
                TLISecurityLogs NewLog = new TLISecurityLogs();

                // 1. Date..
                NewLog.Date = DateTime.UtcNow;

                // 2. User Id..
                if (UserId != null)
                    NewLog.UserId = UserId.Value;

                var UserType = _dbContext.TLIuser.FirstOrDefault(x => x.Id == UserId).UserType;
                NewLog.UserType = UserType;
                // 3. Controller Name..
                List<object> Controller_Function_Name = filterContext.RouteData.Values.Values.ToList();
                NewLog.ControllerName = Controller_Function_Name[1].ToString();

                // 4. Function Name..
                NewLog.FunctionName = Controller_Function_Name[0].ToString();

                // 5. Body Parameters..
                IDictionary<string, object> Parameters = MyParametersList.Where(x =>
                    x.GuidId == Guid.Parse(filterContext.HttpContext.TraceIdentifier)).Select(x => x.Parameters).FirstOrDefault();


                // Check If There Any Exception In API Response..
                Exception Exceptions = filterContext.Exception;
                if (Exceptions != null)
                {
                    // 7. Response Status..
                    NewLog.ResponseStatus = "Failed";

                    // 8. Result...
                    NewLog.Message = Exceptions.Message;
                }
                else
                {
                    IActionResult ActionResult = filterContext.Result;
                    if (ActionResult is OkObjectResult json)
                    {
                        dynamic DynamicObject = new ExpandoObject();
                        DynamicObject = json.Value;
                        if (!string.IsNullOrEmpty(DynamicObject.Errors))
                        {
                            // 7. Response Status..
                            NewLog.ResponseStatus = "Failed";

                            // 8. Result...
                            NewLog.Message = DynamicObject.Errors;
                        }
                        else
                        {
                            // 7. Response Status..
                            NewLog.ResponseStatus = "Success";

                            // 8. Result...
                            NewLog.Message = Newtonsoft.Json.JsonConvert.SerializeObject(DynamicObject);
                        }
                    }
                }
                
                _dbContext.TLISecurityLogs.AddAsync(NewLog);
                _dbContext.SaveChanges();
            }
            catch (Exception err)
            {
                _logger.LogError(err.Message);
            }
        }
    }
}
