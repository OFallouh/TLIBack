using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TLIS_DAL.Models
{
    public class TLISecurityLogs
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public int UserType { get; set; }
        public string Title { get; set; }
        public int UserId { get; set; }
        public TLIuser User { get; set; }
        public string ControllerName { get; set; }
        public string FunctionName { get; set; }
        public string ResponseStatus { get; set; }
        public string Message { get; set; }

    }
}
