using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace TLIS_DAL.Models
{
    public class TLIcivilLoadLegs
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public TLIcivilLoads civilLoads { get; set; }
        public int civilLoadsId { get; set; }
        public TLIleg leg { get; set; }
        public int legId { get; set; }
    }
}
