using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TLIS_DAL.ViewModels.IntegrationBinding
{
    public class EditExternalSysBinding
    {
        public int Id { get; set; }
        [Required]
        public string SysName { get; set; }
        [Required]
        public string UserName { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters long.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,}$",
        ErrorMessage = "Password must contain at least one lowercase letter, one uppercase letter, one numeric digit, and one special character.")]
        public string Password { get; set; }
        [Required]
        public string IP { get; set; }
        public int LifeTime { get; set; }
        public bool IsToken { get; set; }

        public List<int> ApiPermissionIds { get; set; }

    }
}
