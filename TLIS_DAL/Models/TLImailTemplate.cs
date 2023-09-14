using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Toolbelt.ComponentModel.DataAnnotations.Schema;

namespace TLIS_DAL.Models
{
    public class TLImailTemplate
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        [Index(IsUnique = true)]
        public string Name { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public bool Deleted { get; set; }
        public DateTime? DateDeleted { get; set; }
       // public List<TLIstepAction> StepActions { get; set; }
    }
}
