using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TLIS_DAL.Models
{
    public class TLIuserPermissions
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int Permission_Id { get; set; }
        public TLIpermissions Permission { get; set; }
        public int User_Id { get; set; }
        public TLIuser User { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
