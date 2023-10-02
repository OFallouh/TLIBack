using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Toolbelt.ComponentModel.DataAnnotations.Schema;

namespace TLIS_DAL.Models
{
    public class TLIgroup
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        [Index(IsUnique = true)]
        public string Name { get; set; }
        public ICollection<TLIgroup> Parent { get; set; }
        public int? ParentId { get; set; }
        public TLIgroup Upper { get; set; }
        public int? UpperId { get; set; }
        [Required]
        public int GroupType { get; set; }
        [Required]
        public bool Active { get; set; }
        [Required]
        public bool Deleted { get; set; }
        public TLIactor Actor { get; set; }
        public int? ActorId { get; set; }
        public IEnumerable<TLIgroupRole> TLIgroupRole { get; set; }
        public IEnumerable<TLIgroupUser> TLIgroupUser { get; set; }
    }
}
