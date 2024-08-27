using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TLIS_DAL.Helpers;
using TLIS_Service.ServiceBase;
using Oracle.ManagedDataAccess.Client;
using Microsoft.Extensions.Configuration;
using TLIS_DAL.ViewModels.AttachedFilesDTOs;
using TLIS_Service.Helpers;
using System.Net.Http;
using System.Net;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.StaticFiles;
using TLIS_DAL.Helper;
using TLIS_API.Middleware.WorkFlow;
using System.IdentityModel.Tokens.Jwt;
using TLIS_DAL.ViewModels.CivilWithLegsDTOs;

namespace TLIS_API.Controllers
{
    
    [ServiceFilter(typeof(LogFilterAttribute))]
    [Route("api/[controller]")]
    [ApiController]
    public class FileManagmentController : ControllerBase
    {
        private readonly IUnitOfWorkService _unitOfWorkService;
        private readonly IConfiguration _configuration;
        private readonly IHostingEnvironment Environment;
        private readonly IHostingEnvironment _hostingEnvironment;

        public FileManagmentController(IUnitOfWorkService unitOfWorkService, IConfiguration configuration, IHostingEnvironment _environment, IHostingEnvironment hostingEnvironment)
        {
            _unitOfWorkService = unitOfWorkService;
            _configuration = configuration;
            Environment = _environment;
            _hostingEnvironment = hostingEnvironment;


        }
        [ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
        [HttpGet("GenerateExcelTemplacteByTableName")]
        [ProducesResponseType(200, Type = typeof(Nullable))]
        public async Task<ActionResult> GenerateExcelTemplacteByTableName(string TableName, int? CategoryId)
        {
            try
            {


                string contentRootPath = _configuration["StoreFiles"];

                // string userRoot = System.Environment.GetEnvironmentVariable("USERPROFILE");
                // string downloadFolder = Path.Combine(userRoot, "Downloads");

                string downloadFolder = Path.Combine(contentRootPath, "GenerateFiles");
                /*  if (TableName.Contains("Library"))
                   {
                       userRoot = Path.Combine(userRoot, "ExcelTemplates", "Library");
                   }
                   else
                   {
                       userRoot = Path.Combine(userRoot, "ExcelTemplates", "Installation");

                   }*/
                if (!Directory.Exists(downloadFolder))
                {
                    DirectoryInfo di = Directory.CreateDirectory(downloadFolder);

                }
                var response = _unitOfWorkService.FileManagmentService.GenerateExcelTemplacteByTableName(TableName, downloadFolder, CategoryId);

                var fullPath = response.Data + "/" + TableName + ".xlsx";
                var bytes = await System.IO.File.ReadAllBytesAsync(fullPath);
                var provider = new FileExtensionContentTypeProvider();
                if (!provider.TryGetContentType(fullPath, out var contentType))
                {
                    contentType = "application/octet-stream";
                }
                return File(bytes, contentType, Path.GetFileName(fullPath));

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
        [HttpPost("ImportFile")]
        [ProducesResponseType(200, Type = typeof(Nullable))]
        public IActionResult ImportFile(string TableName = null, int? CategoryId = null)
        {
            var ConnectionString = _configuration["ConnectionStrings:ActiveConnection"];
            string authHeader = HttpContext.Request.Headers["Authorization"];
            var file = Request.Form.Files[0];
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

           
            var response = _unitOfWorkService.FileManagmentService.ImportFile(file, TableName, CategoryId, ConnectionString, userId);
            return Ok(response);
          
           
        }
        [ServiceFilter(typeof(WorkFlowMiddleware))]
        [HttpPost("AttachFileInstallation")]
        [ProducesResponseType(200, Type = typeof(Nullable))]
        public IActionResult AttachFile(string RecordId, string TableName, int DocumentTypeId, string Model = null, string Name = null, string SiteCode = null)
        {
            var asset = _configuration["assets"];

            //string contentRootPath = _hostingEnvironment.ContentRootPath;
            string contentRootPath = _configuration["StoreFiles"];

            string AttachFolder = Path.Combine(contentRootPath, "AttachFiles");

            if (!Directory.Exists(AttachFolder))
            {
                DirectoryInfo di = Directory.CreateDirectory(AttachFolder);
            }

            var File = Request.Form.Files[0];
            var ConnectionString = _configuration["ConnectionStrings:ActiveConnection"];

            var response = _unitOfWorkService.FileManagmentService.AttachFile(File, DocumentTypeId, Model, Name, SiteCode, RecordId, TableName, ConnectionString, AttachFolder, asset);

            if (response.Code == (int)Helpers.Constants.ApiReturnCode.fail)
                return BadRequest(response);
            return Ok(response);
        }
        [ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
        [HttpPost("AttachFileLibrary")]
        [ProducesResponseType(200, Type = typeof(Nullable))]
        public IActionResult AttachFileLibrary(string RecordId, string TableName, int DocumentTypeId, string Model = null, string Name = null, string SiteCode = null)
        {
            var asset = _configuration["assets"];

            //string contentRootPath = _hostingEnvironment.ContentRootPath;
            string contentRootPath = _configuration["StoreFiles"];

            string AttachFolder = Path.Combine(contentRootPath, "AttachFiles");

            if (!Directory.Exists(AttachFolder))
            {
                DirectoryInfo di = Directory.CreateDirectory(AttachFolder);
            }

            var File = Request.Form.Files[0];
            var ConnectionString = _configuration["ConnectionStrings:ActiveConnection"];

            var response = _unitOfWorkService.FileManagmentService.AttachFile(File, DocumentTypeId, Model, Name, SiteCode, RecordId, TableName, ConnectionString, AttachFolder, asset);

            if (response.Code == (int)Helpers.Constants.ApiReturnCode.fail)
                return BadRequest(response);
            return Ok(response);
        }
        [ServiceFilter(typeof(WorkFlowMiddleware))]
        [HttpGet("DeleteFileInstallation")]
        [ProducesResponseType(200, Type = typeof(Nullable))]
        public IActionResult DeleteFile(string FileName, int RecordId, string TableName, string SiteCode)
        {
            var Response = _unitOfWorkService.FileManagmentService.DeleteFile(FileName, RecordId, TableName, SiteCode);
            return Ok(Response);
        }
        [ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
        [HttpGet("DeleteFileLibrary")]
        [ProducesResponseType(200, Type = typeof(Nullable))]
        public IActionResult DeleteFileLibrary(string FileName, int RecordId, string TableName, string SiteCode)
        {
            var Response = _unitOfWorkService.FileManagmentService.DeleteFile(FileName, RecordId, TableName, SiteCode);
            return Ok(Response);
        }
       // [ServiceFilter(typeof(WorkFlowMiddleware))]
        [HttpGet("GetFilesByRecordIdAndTableNameInstallation")]
        [ProducesResponseType(200, Type = typeof(Nullable))]
        public IActionResult GetFilesByRecordIdAndTableName(int RecordId, string TableName, string SiteCode)
        {
            var response = _unitOfWorkService.FileManagmentService.GetFilesByRecordIdAndTableName(RecordId, TableName, SiteCode);
            return Ok(response);
        }
        [ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
        [HttpGet("GetFilesByRecordIdAndTableNameLibrary")]
        [ProducesResponseType(200, Type = typeof(Nullable))]
        public IActionResult GetFilesByRecordIdAndTableNameLibrary(int RecordId, string TableName,string SiteCode)
        {
            var response = _unitOfWorkService.FileManagmentService.GetFilesByRecordIdAndTableName(RecordId, TableName, SiteCode);
            return Ok(response);
        }
        [HttpPost("GetAttachecdFiles")]
        [ProducesResponseType(200, Type = typeof(Response<List<AttachedFilesViewModel>>))]
        public IActionResult GetAttachecdFiles(int RecordId, string TableName)
        {
            var response = _unitOfWorkService.FileManagmentService.GetAttachecdFiles(RecordId, TableName);
            return Ok(response);
        }
        [ServiceFilter(typeof(WorkFlowMiddleware))]
        [HttpPost("GetAttachecdFilesBySite")]
        [ProducesResponseType(200, Type = typeof(Response<List<AttachedFilesViewModel>>))]
        public IActionResult GetAttachecdFilesBySite(string SiteCode )
        {
            var response = _unitOfWorkService.FileManagmentService.GetAttachecdFilesBySite(SiteCode);
            return Ok(response);
        }
        [HttpPost("AttachedUnAttached")]
        [ProducesResponseType(200, Type = typeof(AttachedFilesViewModel))]
        public IActionResult AttachedUnAttached(int Id)
        {
            var response = _unitOfWorkService.FileManagmentService.AttachedUnAttached(Id);
            return Ok(response);
        }
        [ServiceFilter(typeof(WorkFlowMiddleware))]
        [HttpGet("GetAttachFileInstallation")]
        public async Task<IActionResult> GetAttachFile(string filename, int recordid, string tablename)
        {
            var asset = _configuration["assets"];
            var response = _unitOfWorkService.FileManagmentService.GetAttachedToDownload(filename, recordid, tablename);

            if (response == null)
            {
                return BadRequest("This file is not found");
            }
            if ((response.Data.ToLower().Contains(".jpg") || response.Data.ToLower().Contains(".jpeg") || response.Data.ToLower().Contains(".png")) && tablename.Contains("TLIsite"))
            {
                response.Data = asset + "\\" + filename + "";
            }
            var provider = new FileExtensionContentTypeProvider();
            if (!provider.TryGetContentType(response.Data, out var contentType))
            {
                contentType = "application/octet-stream";
            }

            var bytes = await System.IO.File.ReadAllBytesAsync(response.Data);

            Response.Headers.Add("x-file-name", Path.GetFileName(response.Data));
            Response.Headers.Add("Access-Control-Expose-Headers", "x-file-name");
            return File(bytes, contentType, Path.GetFileName(response.Data));

        }
        [ServiceFilter(typeof(MiddlewareLibraryAndUserManagment))]
        [HttpGet("GetAttachFileLibrary")]
        public async Task<IActionResult> GetAttachFileLibrary(string filename, int recordid, string tablename)
        {
            var asset = _configuration["assets"];
            var response = _unitOfWorkService.FileManagmentService.GetAttachedToDownload(filename, recordid, tablename);

            if (response == null)
            {
                return BadRequest("This file is not found");
            }
            if ((response.Data.ToLower().Contains(".jpg") || response.Data.ToLower().Contains(".jpeg") || response.Data.ToLower().Contains(".png")) && tablename.Contains("TLIsite"))
            {
                response.Data = asset + "\\" + filename + "";
            }
            var provider = new FileExtensionContentTypeProvider();
            if (!provider.TryGetContentType(response.Data, out var contentType))
            {
                contentType = "application/octet-stream";
            }

            var bytes = await System.IO.File.ReadAllBytesAsync(response.Data);

            Response.Headers.Add("x-file-name", Path.GetFileName(response.Data));
            Response.Headers.Add("Access-Control-Expose-Headers", "x-file-name");
            return File(bytes, contentType, Path.GetFileName(response.Data));
        }


    }
}