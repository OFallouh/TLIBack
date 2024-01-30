using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TLIS_DAL.ViewModels.SiteDTOs
{
    public class ResponseTLI<T>
    {
        public T Data { get; set; }
        public string ErrorMessage { get; set; }
    }
}
