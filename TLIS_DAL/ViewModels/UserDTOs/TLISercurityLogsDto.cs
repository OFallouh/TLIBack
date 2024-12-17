using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLIS_DAL.Models;

namespace TLIS_DAL.ViewModels.UserDTOs
{
    public class TLISercurityLogsDto
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string UserType { get; set; }
        public string Title { get; set; }
        public string UserName { get; set; }
        public string ControllerName { get; set; }
        public string FunctionName { get; set; }
        public string ResponseStatus { get; set; }
        public string Message { get; set; }
    }
}
