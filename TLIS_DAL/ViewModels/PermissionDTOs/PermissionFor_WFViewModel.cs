using System;
using System.Collections.Generic;
using System.Text;

namespace TLIS_DAL.ViewModels.PermissionDTOs
{
    public class PermissionFor_WFViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ControllerName { get; set; }
        public string ActionName { get; set; }
        public bool Deleted { get; set; }
    }
}
