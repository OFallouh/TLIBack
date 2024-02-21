using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using TLIS_API.Middleware.ActionFilters;
using TLIS_API.Middleware.WorkFlow;
using TLIS_DAL.Helper;
using TLIS_DAL.ViewModels.ComplixFilter;
using TLIS_DAL.ViewModels.IntegrationBinding;
using TLIS_Service.ServiceBase;

namespace TLIS_API.Controllers
{
    [ServiceFilter(typeof(WorkFlowMiddleware))]
    [Route("api/[controller]")]
    [ApiController]
    public class IntegrationController : ControllerBase
    {
        private IUnitOfWorkService _unitOfWorkService;
        public IntegrationController(IUnitOfWorkService unitOfWorkService)
        {
            _unitOfWorkService = unitOfWorkService;
        }

        [HttpPost("CreateExtSystem")]

        public IActionResult CreateExtSystem(AddExternalSysBinding req)
        {
            var response = _unitOfWorkService.ExternalSysService.CreateExternalSys(req);
            return Ok(response);
        }


        [HttpPut("EditExtSystem/{id}")]
        public IActionResult EditExtSystem(int id,EditExternalSysBinding req)
        {
            if(id!=req.Id)
            {
                return BadRequest("Id invalid");
            }
            var response = _unitOfWorkService.ExternalSysService.EditExternalSys(req);
            return Ok(response);
        }

        [HttpGet("DeleteExtSystem")]
        public IActionResult DeleteExtSystem(int id)
        {
           
            var response = _unitOfWorkService.ExternalSysService.DeleteExternalSys(id);
            return Ok(response);
        }


        [HttpGet("DisableExtSystem")]
        public IActionResult DisableExtSystem(int id)
        {

            var response = _unitOfWorkService.ExternalSysService.DisableExternalSys(id);
            return Ok(response);
        }


        [HttpPost("GetallExtSystem")]
        public IActionResult GetallExtSystem(string systemName,[FromBody] ParameterPagination parameter)
        {

            var response = _unitOfWorkService.ExternalSysService.GetAllExternalSys(systemName, parameter);
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
    }
}
