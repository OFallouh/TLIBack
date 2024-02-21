using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TLIS_API.Middleware.WorkFlow;
using TLIS_Service.Helpers;

namespace TLIS_API.Controllers
{
    [ServiceFilter(typeof(WorkFlowMiddleware))]
    [ServiceFilter(typeof(LogFilterAttribute))]
    [Route("api/[controller]")]
    [ApiController]
    public class TestDynamicDtoController : ControllerBase
    {
        [HttpPost("getAll")]
        [ProducesResponseType(200, Type = typeof(List<object>))]
        public IActionResult GetCivilNonSteelLibrary()
        {
            List<string> lstDummyFields = new List<string>
            {
            "idWorker",
            "first_name",
            "last_name",
            "birthday",
            "adress"

            };

            dynamic dto = new ExpandoObject();
            foreach (var p in lstDummyFields)
            {
                ((IDictionary<String, Object>)dto).Add(new KeyValuePair<string, object>(p, null));
            }
            return Ok(dto);
        }
    }
}