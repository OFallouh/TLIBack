using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Toolbelt.ComponentModel.DataAnnotations.Schema;

namespace TLIS_DAL.Models
{
    public class TLIgroupUser
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }       
        [Index("GroupIdAndUserId",1,IsUnique = true)]
        [Required]
        public int groupId { get; set; }
        public TLIgroup group { get; set; }
        [Index("GroupIdAndUserId",2, IsUnique = true)]
        [Required]
        public int userId { get; set; }
        public TLIuser user { get; set; }
        [Required]
        public bool Active { get; set; }
        [Required]
        public bool Deleted { get; set; }
    }
}
