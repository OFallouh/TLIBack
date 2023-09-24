using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModels.NewPermissionsDTOs.Permissions;
using TLIS_DAL.ViewModels.PermissionDTOs;

namespace TLIS_DAL.ViewModels.RoleDTOs
{
    public class AddRoleViewModel
    {
        [Required]
        public string Name { get; set; }
        public bool Active { get; set; } = true;
        public bool Deleted { get; set; } = false;
        public List<AddPerViewModel> permissions { get; set; } = null;
    }
}
