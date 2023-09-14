using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TLIS_DAL.Models
{
    public class TLIgroupPermissions
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int PermissionId { get; set; }
        public TLIpermissions Permission { get; set; }
        public int GroupId { get; set; }
        public TLIgroup Group { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
