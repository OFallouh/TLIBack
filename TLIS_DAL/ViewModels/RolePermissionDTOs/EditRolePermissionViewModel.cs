using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TLIS_DAL.ViewModels.RolePermissionDTOs
{
    public class EditRolePermissionViewModel
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public int roleId { get; set; }
        [Required]
        public int permissionId { get; set; }
        [Required]
        public bool Active { get; set; }
        [Required]
        public bool Deleted { get; set; }
    }
}
