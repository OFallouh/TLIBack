using System;
using System.Collections.Generic;
using System.Text;

namespace TLIS_DAL.ViewModels.LogErrorDTOs
{
    public class LogUsersActionsViewModel
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public int UserId { get; set; }
        public string ControllerName { get; set; }
        public string FunctionName { get; set; }
        public string BodyParameters { get; set; }
        public string HeaderParameters { get; set; }
        public string ResponseStatus { get; set; }
        public string Result { get; set; }
    }
}
