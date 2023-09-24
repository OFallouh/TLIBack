using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TLIS_DAL.Models
{
    public class TLIuser_Permissions
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; } 
        public int UserId { get; set; }
        public TLIuser user { get; set; }
        public string PageUrl { get; set; } 
        public bool? Active { get; set; } 
        public bool? Delete { get; set; } 
    }
}
