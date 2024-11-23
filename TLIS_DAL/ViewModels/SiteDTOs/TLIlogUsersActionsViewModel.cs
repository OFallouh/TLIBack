using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TLIS_DAL.ViewModels.SiteDTOs
{
    public class TLIlogUsersActionsViewModel
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string UserName { get; set; }
        public string ControllerName { get; set; }
        public string FunctionName { get; set; }
        public string BodyParameters { get; set; }
        public string HeaderParameters { get; set; }
        public string ResponseStatus { get; set; }
        public string Result { get; set; }
    }
}
