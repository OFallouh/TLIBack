using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TLIS_DAL.ViewModels.PermissionDTOs
{
    public class RolePermissionsViewModel
    {
        public int Id { get; set; }

        public string RoleName { get; set; }

        public string PageUrl { get; set; }

        public bool Active { get; set; }

        public bool Delete { get; set; }
    }
}
