using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TLIS_DAL.ViewModels.IntegrationDto
{
    public class ExternalPermission
    {
        public int  Id { get; set; }
        public string label { get; set; }
        public string Type { get; set; }
        public string EndPoint { get; set; }
    }
}
