using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TLIS_DAL.ViewModels.RolePermissionDTOs
{
    public class AddRolePermissionViewModel
    {
        [Required]
        public int roleId { get; set; }
        [Required]
        public int permissionId { get; set; }
        public bool Active { get; set; } = true;
        public bool Deleted { get; set; } = false;
    }
}
