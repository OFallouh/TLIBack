using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace TLIS_DAL.Models
{
    public class TLIoption
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; }
        [ForeignKey("TLIcondition")]
        public int? ConditionId { get; set; }
        public TLIcondition Condition { get; set; }
        public bool Deleted { get; set; }
        public DateTime? DateDeleted { get; set; }

        public IEnumerable<TLIsuboption> SubOptions { get; set; }


    }
}
