using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLIS_DAL.Models;

namespace TLIS_DAL.ViewModels.NewPermissionsDTOs.RolePermissions
{
    public class NewRolePermissionsViewModel
    {
        public int Id { get; set; }
        public int RoleId { get; set; }
        public string Role_Name { get; set; }
        public int PermissionId { get; set; }
        public string Permission_Name { get; set; }
        public bool IsActive { get; set; }
    }
}
