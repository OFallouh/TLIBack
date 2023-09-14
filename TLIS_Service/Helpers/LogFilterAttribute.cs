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

namespace TLIS_Service.Helpers
{
    public class LogFilterAttribute: ActionFilterAttribute
    {
        static public int? UserId { get; set; }
        IServiceCollection _services;
        ServiceProvider _serviceProvider;
        private readonly ILogger<LogFilterAttribute> _logger;
        public static List<ActionParamaters> MyParametersList { get; set; } = new List<ActionParamaters>();
        public class ActionParamaters
        {
            public Guid GuidId { get; set; }
            public IDictionary<string, object> Parameters { get; set; }
        }
        public LogFilterAttribute(IServiceCollection services, ILogger<LogFilterAttribute> logger)
        {
            _services = services;
            _serviceProvider = _services.BuildServiceProvider();
            _logger = logger;
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
                UnitOfWork UnitOfWork = _serviceProvider.GetService<UnitOfWork>();
                TLIlogUsersActions NewLog = new TLIlogUsersActions();

                // 1. Date..
                NewLog.Date = DateTime.UtcNow;

                // 2. User Id..
                if (UserId != null)
                    NewLog.UserId = UserId.Value;

                // 3. Controller Name..
                List<object> Controller_Function_Name = filterContext.RouteData.Values.Values.ToList();
                NewLog.ControllerName = Controller_Function_Name[1].ToString();

                // 4. Function Name..
                NewLog.FunctionName = Controller_Function_Name[0].ToString();

                // 5. Body Parameters..
                IDictionary<string, object> Parameters = MyParametersList.Where(x =>
                    x.GuidId == Guid.Parse(filterContext.HttpContext.TraceIdentifier)).Select(x => x.Parameters).FirstOrDefault();
                NewLog.BodyParameters = Newtonsoft.Json.JsonConvert.SerializeObject(Parameters);

                // 6. Header Paramaters..
                NewLog.HeaderParameters = Newtonsoft.Json.JsonConvert.SerializeObject(filterContext.HttpContext.Request.Headers);

                // Check If There Any Exception In API Response..
                Exception Exceptions = filterContext.Exception;
                if(Exceptions != null)
                {
                    // 7. Response Status..
                    NewLog.ResponseStatus = "Failed";

                    // 8. Result...
                    NewLog.Result = Exceptions.Message;
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
                            NewLog.Result = DynamicObject.Errors;
                        }
                        else if (DynamicObject.Code == 0)
                        {
                            // 7. Response Status..
                            NewLog.ResponseStatus = "Success";

                            // 8. Result...
                            NewLog.Result = Newtonsoft.Json.JsonConvert.SerializeObject(DynamicObject);
                        }
                    }
                }
                //UnitOfWork.LogUsersActionsRepository.AddAsync(NewLog);
                //UnitOfWork.SaveChangesAsync();
            }
            catch (Exception err)
            {
                _logger.LogError(err.Message);
            }
        }
    }
}
