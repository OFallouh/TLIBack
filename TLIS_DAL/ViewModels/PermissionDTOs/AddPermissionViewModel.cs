using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TLIS_DAL.ViewModels.PermissionDTOs
{
    public class AddPermissionViewModel
    {
        [Required]
        public string Name { get; set; }
        public string Module { get; set; }
        public string PermissionType { get; set; }
        [Required]
        public string ControllerName { get; set; }
        [Required]
        public string ActionName { get; set; }
        public bool Active { get; set; } = true;
        public bool Deleted { get; set; } = false;
    }
}
