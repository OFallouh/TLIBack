using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Toolbelt.ComponentModel.DataAnnotations.Schema;

namespace TLIS_DAL.Models
{
    public class TLIuser
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        public string MobileNumber { get; set; }
        [Required]
        [Index(IsUnique = true)]
        public string UserName { get; set; }
        [DataType(DataType.Password)]
        public string Password { get; set; }
        public string Domain { get; set; }
        public string AdGUID { get; set; }
        [Required]
        public int UserType { get; set; }
        public DateTime? ChangedPasswordDate { get; set; }
        [Required]
        public bool Active { get; set; }
        [Required]
        public bool Deleted { get; set; }
        public string ConfirmationCode { get; set; }
        public bool ValidateAccount { get; set; } = true;
        public IEnumerable<TLIuserPermission> userPermissions { get; set; }
        public IEnumerable<TLIuserPermissions> userPermissionss { get; set; }
        public IEnumerable<TLIgroupUser> groupUser { get; set; }
    }
}
