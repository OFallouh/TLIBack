using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Toolbelt.ComponentModel.DataAnnotations.Schema;

namespace TLIS_DAL.Models
{
    public class TLIpermission
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string Module { get; set; }
        public string PermissionType { get; set; }
        [Required]
        [Index("ControllerNameAndActionName", 1,IsUnique = true)]
        public string ControllerName { get; set; }
        [Required]
        [Index("ControllerNameAndActionName", 2,IsUnique = true)]
        public string ActionName { get; set; }
        [Required]
        public bool Active { get; set; }
        [Required]
        public bool Deleted { get; set; }
        public IEnumerable<TLIuserPermission> userPermissions { get; set; }
        public IEnumerable<TLIrolePermission> rolePermission { get; set; }
    }
}
