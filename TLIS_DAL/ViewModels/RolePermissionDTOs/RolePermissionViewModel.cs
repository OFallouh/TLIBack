using System;
using System.Collections.Generic;
using System.Text;

namespace TLIS_DAL.ViewModels.RolePermissionDTOs
{
    public class RolePermissionViewModel
    {
        public int Id { get; set; }
        public int roleId { get; set; }
        public int permissionId { get; set; }
        public bool Active { get; set; }
        public bool Deleted { get; set; }
    }
}
