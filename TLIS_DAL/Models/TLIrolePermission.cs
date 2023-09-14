using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Toolbelt.ComponentModel.DataAnnotations.Schema;

namespace TLIS_DAL.Models
{
    public class TLIrolePermission
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public TLIrole role { get; set; }
        [Index("RoleIdAndPermissionId",1,IsUnique = true)]
        [Required]
        public int roleId { get; set; }
        public TLIpermission permission { get; set; }
        [Index("RoleIdAndPermissionId", 2,IsUnique = true)]
        [Required]
        public int permissionId { get; set; }
        [Required]
        public bool Active { get; set; }
        [Required]
        public bool Deleted { get; set; }
    }
}
