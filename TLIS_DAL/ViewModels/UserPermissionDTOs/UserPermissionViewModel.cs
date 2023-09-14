using System;
using System.Collections.Generic;
using System.Text;

namespace TLIS_DAL.ViewModels.UserPermissionDTOs
{
    public class UserPermissionViewModel
    {
        public int Id { get; set; }
        public int permissionId { get; set; }
        public int userId { get; set; }
        public string ControllerName { get; set; }
        public string ActionName { get; set; }
        public bool Active { get; set; }
        public bool Deleted { get; set; }
    }
}
