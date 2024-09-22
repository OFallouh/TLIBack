using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using System;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLIS_API.Middleware.ActionFilters;
using TLIS_API.Middleware.WorkFlow;
using TLIS_DAL.Helper;
using TLIS_DAL.ViewModels.CivilWithLegsDTOs;
using TLIS_DAL.ViewModels.ComplixFilter;
using TLIS_DAL.ViewModels.IntegrationBinding;
using TLIS_Service.ServiceBase;
using Microsoft.Extensions.Configuration;
using Org.BouncyCastle.Ocsp;
namespace TLIS_API.Controllers
{
    [ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
    [Route("api/[controller]")]
    [ApiController]
    public class IntegrationController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private IUnitOfWorkService _unitOfWorkService;
        public IntegrationController(IUnitOfWorkService unitOfWorkService, IConfiguration configuration)
        {
            _unitOfWorkService = unitOfWorkService;
            _configuration = configuration;
        }

        [HttpPost("CreateExtSystem")]

        public IActionResult CreateExtSystem(AddExternalSysBinding req)

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

            
            var response = _unitOfWorkService.ExternalSysService.CreateExternalSys(req, userId);
            return Ok(response);
            
            
        }


        [HttpPost("EditExtSystem/{id}")]
        public IActionResult EditExtSystem(int id,EditExternalSysBinding req)
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


            var response = _unitOfWorkService.ExternalSysService.EditExternalSys(req,userId);
            return Ok(response);
           
        }

        [HttpGet("DeleteExtSystem")]
        public IActionResult DeleteExtSystem(int id)

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


            var response = _unitOfWorkService.ExternalSysService.DeleteExternalSys(id, userId);
            return Ok(response);
          
        }


        [HttpGet("DisableExtSystem")]
        public IActionResult DisableExtSystem(int id)
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


            var response = _unitOfWorkService.ExternalSysService.DisableExternalSys(id, userId);
            return Ok(response);
           
        }


        [HttpPost("GetallExtSystem")]
        public IActionResult GetallExtSystem()
        {

            var response = _unitOfWorkService.ExternalSysService.GetAllExternalSys();
            return Ok(response);
        }

        [HttpGet("GetByIdExtSystem")]
        public IActionResult GetByIdExtSystem(int id)
        {

            var response = _unitOfWorkService.ExternalSysService.GetByIdExternalSys(id);
            return Ok(response);
        }

        [HttpGet("GetAllIntegrationAPI")]
        public IActionResult GetAllIntegrationAPI()
        {

            var response = _unitOfWorkService.ExternalSysService.GetAllIntegrationAPI();
            return Ok(response);
        }

        [HttpGet("RegenerateToken")]
        public IActionResult RegenerateToken(int id)
        {

            var response = _unitOfWorkService.ExternalSysService.RegenerateToken(id);
            return Ok(response);
        }

        [HttpPost("GetListErrorLog")]
        public IActionResult GetListErrorLog(ClxFilter f)
        {

            var response = _unitOfWorkService.ExternalSysService.GetListErrorLog(f);
            return Ok(response);
        }
        [HttpPost("GetRawContent")]
        public async Task<IActionResult> GetRawContent()
        {
            string rawContent = string.Empty;
            using (var reader = new StreamReader(Request.Body,
                          encoding: Encoding.UTF8, detectEncodingFromByteOrderMarks: false))
            {
                rawContent = await reader.ReadToEndAsync();
            }

            return Ok(rawContent);

        }
        [HttpGet("GetAllExternalPermission")]
        public IActionResult GetAllExternalPermission()
        {

            var response = _unitOfWorkService.ExternalSysService.GetAllExternalPermission();
            return Ok(response);
        }

        [HttpPost("ExportErrorLog")]
        public async Task<ActionResult> ExportErrorLog()
        {

            var response = _unitOfWorkService.ExternalSysService.GetListErrorLogExport();
            var fullPath = response.Data;
            var bytes = await System.IO.File.ReadAllBytesAsync(fullPath);
            var provider = new FileExtensionContentTypeProvider();
            if (!provider.TryGetContentType(fullPath, out var contentType))
            {
                contentType = "application/octet-stream";
            }
            return File(bytes, contentType, Path.GetFileName(fullPath));
        }
    }
}
