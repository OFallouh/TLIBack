using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TLIS_API.Helpers
{
    public class ResponseFormat
    {
        public object Data { get; set; }
        public int Code { get; set; }
        public string Massage { get; set; }
        
    }
}
