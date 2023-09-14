using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Web.Administration;
using System.Web;
using TLIS_DAL.ViewModels.ApplicationPool;
using System.DirectoryServices;
using System.Xml;

namespace TLIS_API.Controllers.ApplicationPoolConfig
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApplicationPoolConfigController : ControllerBase
    {
        [HttpPost]

        public ActionResult GetApplicationPoolInfo(ApplicationPoolBindingModel Appmodel)
        {
            try
            {
                //%SystemRoot%\System32\inetsrv\config
                using (ServerManager serverManager = new ServerManager())
                {
                    //"Clr4IntegratedAppPool"

                    ApplicationPool pool = serverManager.ApplicationPools.FirstOrDefault(x=>x.Name== "TLIAPIPool");

                    pool.ProcessModel.IdentityType = ProcessModelIdentityType.SpecificUser;

                    pool.ProcessModel.Password = "Mhdism123#";

                    pool.ProcessModel.UserName = "IDS\\MHDISM";


                    serverManager.CommitChanges();

                    return Ok(pool.Name);

                }

            }
            catch (Exception err)
            {
                return BadRequest(err.Message);

            }
        }

       




    }
}
