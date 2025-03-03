using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using TLIS_DAL.ViewModels.GroupDTOs;
using TLIS_DAL.ViewModels.NewPermissionsDTOs.Permissions;
using TLIS_DAL.ViewModels.PermissionDTOs;

namespace TLIS_DAL.ViewModels.UserDTOs
{
    public class AddUserViewModel
    {
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        public string MobileNumber { get; set; }
        [Required]
        public string UserName { get; set; }
        [Required]  
        public string Password { get; set; }
        public string Domain { get; set; }
        public string AdGUID { get; set; }
        [Required]
        public int UserType { get; set; }
        public bool Active { get; set; } = true;
        public bool Deleted { get; set; } = false;
        public bool IsFirstLogin { get; set; }
        public bool ValidateAccount { get; set; } = true;
        public string Permissions { get; set; }
        public List<GroupNamesViewModel> Groups { get; set; }
    }
}
