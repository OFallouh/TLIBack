using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModels.GroupDTOs;
using TLIS_DAL.ViewModels.NewPermissionsDTOs.Permissions;
using TLIS_DAL.ViewModels.PermissionDTOs;

namespace TLIS_DAL.ViewModels.RoleDTOs
{
    public class EditRoleViewModel
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public bool Active { get; set; }
        [Required]
        public bool Deleted { get; set; }
        public List<string> permissions { get; set; }
    }
}
