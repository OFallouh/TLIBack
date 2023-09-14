using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TLIS_DAL.ViewModels.UserPermissionDTOs
{
    public class EditUserPermissionViewModel
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public int permissionId { get; set; }
        [Required]
        public int userId { get; set; }
        public string ControllerName { get; set; }
        public string ActionName { get; set; }
        [Required]
        public bool Active { get; set; }
        [Required]
        public bool Deleted { get; set; }
    }
}
