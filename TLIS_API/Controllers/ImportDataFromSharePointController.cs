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
using System.Net.Http.Headers;
using TLIS_Service.Helpers;
using TLIS_DAL.ViewModels.ComplixFilter;
using Microsoft.AspNetCore.StaticFiles;

namespace TLIS_API.Controllers
{
    [ServiceFilter(typeof(LogFilterAttribute))]
    [Route("api/[controller]")]
    [ApiController]
    public class ImportDataFromSharePointController : ControllerBase
    {
        private readonly IUnitOfWorkService _unitOfWorkService;
        private readonly IConfiguration _configuration;
        public ImportDataFromSharePointController(IUnitOfWorkService unitOfWorkService, IConfiguration configuration)
        {
            _unitOfWorkService = unitOfWorkService;
            _configuration = configuration;
        }
        // Response<string> (IFormFile File)
        [HttpPost("ImportSiteData")]
        [ProducesResponseType(200, Type = typeof(Nullable))]
        public IActionResult ImportSiteData()
        {
            var File = Request.Form.Files[0];
            var response = _unitOfWorkService.ImportSiteDataService.ImportSiteData(File);
            return Ok(response);
        }
        [HttpPost("ImportSiteFileData")]
        [ProducesResponseType(200, Type = typeof(Nullable))]
        public IActionResult ImportSiteFileData()
        {

            var file = Request.Form.Files[0];

            var response = _unitOfWorkService.ImportSiteDataService.ImportFileData(file);
            return Ok(response);

        }



        //  var ConnectionString = _configuration["ConnectionStrings:ActiveConnection"];

        [HttpPost("ImportLibraryFileData")]
        [ProducesResponseType(200, Type = typeof(Nullable))]
        public IActionResult ImportLibraryFileData(IFormFile[] files)
        {
            // Add CORS headers to the response 
            Response.Headers.Add("Access-Control-Allow-Origin", "http://10.249.94.86:8085");
            Response.Headers.Add("Access-Control-Allow-Methods", "POST");
            Response.Headers.Add("Access-Control-Allow-Headers", "Content-Type");

            // Handle preflight OPTIONS request    
            if (Request.Method == "OPTIONS")
            {
                return new StatusCodeResult(StatusCodes.Status200OK);
            }

            //var file = Request.Form.Files[0];
            var response = _unitOfWorkService.ImportSiteDataService.ImportLibraryFileData(files[0]);
            return Ok(response);

        }
        //[HttpPost("ImportInstallationFileData")]
        //[ProducesResponseType(200, Type = typeof(Nullable))]
        //public IActionResult ImportInstallationFileData()
        //{
        //    var file = Request.Form.Files[0];

        //    var response = _unitOfWorkService.ImportSiteDataService.ImportInstallationFileData(file);
        //    return Ok(response);

        //}
        [HttpPost("ImportLibraryFileData2")]
        [ProducesResponseType(200, Type = typeof(Nullable))]
        public IActionResult ImportLibraryFileData2()
        {
            var File = Request.Form.Files[0];
            var ConnectionString = _configuration["ConnectionStrings:ActiveConnection"];
            var response = _unitOfWorkService.ImportSiteDataService.ImportLibraryFileData2(File, ConnectionString);
            return Ok(response);
        }
        [HttpPost("ImportInstallationFileData2")]
        [ProducesResponseType(200, Type = typeof(Nullable))]
        public IActionResult ImportInstallationFileData2()
        {
            var File = Request.Form.Files[0];
            var ConnectionString = _configuration["ConnectionStrings:ActiveConnection"];
            var response = _unitOfWorkService.ImportSiteDataService.ImportInstallationFileData2(File, ConnectionString);
            return Ok(response);
        }

        [HttpPost("GetAllWarningData")]
        public IActionResult GetAllWarningData(ClxFilter f)
        {
            var response = _unitOfWorkService.ImportSiteDataService.GetAllWarningData(f);
            return Ok(response);
        }
        [HttpGet("ExportErrorDataTable")]
        [ProducesResponseType(200, Type = typeof(Nullable))]
        public async Task<ActionResult> ExportErrorDataTable(string ErrorType)
        {
            string ContentRootPath = _configuration["StoreFiles"];
            string FileDirectory = Path.Combine(ContentRootPath, "GenerateFiles");

            if (!Directory.Exists(FileDirectory))
            {
                DirectoryInfo di = Directory.CreateDirectory(FileDirectory);
            }

            Response<string> Response = _unitOfWorkService.ImportSiteDataService.ExportErrorDataTable(ErrorType, FileDirectory);

            string FullPath = Response.Data + "/" + ErrorType + ".xlsx";
            byte[] Bytes = await System.IO.File.ReadAllBytesAsync(FullPath);
            FileExtensionContentTypeProvider Provider = new FileExtensionContentTypeProvider();

            if (!Provider.TryGetContentType(FullPath, out var ContentType))
            {
                ContentType = "application/octet-stream";
            }

            return File(Bytes, ContentType, Path.GetFileName(FullPath));
        }
    }
}



