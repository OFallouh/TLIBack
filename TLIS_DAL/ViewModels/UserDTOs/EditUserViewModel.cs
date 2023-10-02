using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using TLIS_DAL.ViewModels.GroupDTOs;
using TLIS_DAL.ViewModels.PermissionDTOs;

namespace TLIS_DAL.ViewModels.UserDTOs
{
    public class EditUserViewModel
    {
        [Required]
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        // [Required]
        [EmailAddress]
        public string Email { get; set; }
        public string MobileNumber { get; set; }
        [Required]
        public string UserName { get; set; }

        public string Password { get; set; }
        //public string ConfirmPassword { get; set; }

        public string Domain { get; set; }
        public string AdGUID { get; set; }
        [Required]
        public int UserType { get; set; }
        [Required]
        public bool Active { get; set; }
        [Required]
        public bool Deleted { get; set; }
        public List<string> permissions { get; set; }

        public DateTime ChangedPasswordDate { get; set; }
        public List<GroupNamesViewModel> Groups { get; set; }
    }
}
