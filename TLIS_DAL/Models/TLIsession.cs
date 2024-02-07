using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TLIS_DAL.Models
{
    public class TLIsession
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        [MaxLength(450)]
        public string Token { get; set; }

        [Required]
        [MaxLength(450)]
        public string IP { get; set; }

        [Required]
        public DateTime LoginDate { get; set; }
    }
}
