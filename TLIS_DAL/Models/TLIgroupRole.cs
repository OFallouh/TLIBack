using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Toolbelt.ComponentModel.DataAnnotations.Schema;

namespace TLIS_DAL.Models
{
    public class TLIgroupRole
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }     
        public TLIgroup group { get; set; }
        [Index("GroupIdAndRoleId", 1, IsUnique = true)]
        [Required]
        public int groupId { get; set; }
        public TLIrole role { get; set; }
        [Index("GroupIdAndRoleId", 2, IsUnique = true)]
        [Required]
        public int roleId { get; set; }
        [Required]
        public bool Active { get; set; }
        [Required]
        public bool Deleted { get; set; }
    }
}
