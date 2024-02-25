using AutoMapper;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System;
using TLIS_DAL.ViewModels.SiteDTOs;
using TLIS_DAL;
using Microsoft.Extensions.Configuration;

namespace TLIS_API.Middleware.WorkFlow
{
    public class MiddlewareLibraryAndUserManagment : IActionFilter
    {
        private readonly IConfiguration _configuration;
        private readonly TLIS_DAL.ApplicationDbContext db;
        private IMapper _mapper;
        IServiceProvider Services;
        public MiddlewareLibraryAndUserManagment(ApplicationDbContext _ApplicationDbContext, IConfiguration configuration, IMapper mapper, IServiceProvider service)
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
                var session = db.TLIsession.FirstOrDefault(x => x.UserId == Convert.ToInt64(userId) && x.IP == clientIPAddress && x.LoginDate < DateTime.Now);
                var userIdInt64 = Convert.ToInt32(userId);
                if (session != null)
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
            else
            {
                context.Result = new UnauthorizedObjectResult("401 Unauthorized");
                return;

            }
        }
        public void OnActionExecuted(ActionExecutedContext context)
        {
        }
    }
}
