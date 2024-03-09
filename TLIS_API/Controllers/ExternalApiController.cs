using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;
using TLIS_API.Middleware.WorkFlow;
using TLIS_DAL.ViewModels.GroupDTOs;
using TLIS_DAL.ViewModels.SiteDTOs;
using TLIS_DAL.ViewModels.UserDTOs;
using TLIS_Service.ServiceBase;
using static TLIS_Service.Services.UserService;

namespace TLIS_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExternalApiController : ControllerBase
    {
        private readonly IUnitOfWorkService _unitOfWorkService;
        private readonly IConfiguration _configuration;
        public ExternalApiController(IUnitOfWorkService unitOfWorkService, IConfiguration configuration)
        {
            _unitOfWorkService = unitOfWorkService;
            _configuration = configuration;
        }
        [HttpPost("GetEmailByUserId")]
        [ProducesResponseType(200, Type = typeof(Task<CallTLIResponse>))]
        public IActionResult GetEmailByUserId(int UserId)
        {
            var response = _unitOfWorkService.UserService.GetEmailByUserId(UserId);
            return Ok(response);
        }
        [HttpPost("GetNameByUserId")]
        [ProducesResponseType(200, Type = typeof(Task<CallTLIResponse>))]
        public IActionResult GetNameByUserId(int UserId)
        {
            var response = _unitOfWorkService.UserService.GetNameByUserId(UserId);
            return Ok(response);

        }
        [HttpPost("GetSession")]
        [ProducesResponseType(200, Type = typeof(bool))]
        public IActionResult GetSession(int UserId, string Ip)
        {
            var response = _unitOfWorkService.UserService.GetSession(UserId, Ip);
            return Ok(response);
        }
        [HttpPost("GetSiteInfo")]
        [ProducesResponseType(200, Type = typeof(List<SiteInfo>))]
        public IActionResult GetSiteInfo(string SiteCode)
        {
            var response = _unitOfWorkService.SiteService.GetSiteInfo(SiteCode);
            return Ok(response);
        }
        [HttpPost("GetUpperToEscalationWF")]
        [ProducesResponseType(200, Type = typeof(Task<EscalationWFViewModel>))]
        public IActionResult GetUpperToEscalationWF(int UserId)
        {
            var response = _unitOfWorkService.GroupService.GetUpperToEscalationWF(UserId);
            return Ok(response);
        }
    }
}
