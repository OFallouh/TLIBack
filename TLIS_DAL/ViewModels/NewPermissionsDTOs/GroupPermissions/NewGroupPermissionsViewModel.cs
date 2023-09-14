using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLIS_DAL.Models;

namespace TLIS_DAL.ViewModels.NewPermissionsDTOs.GroupPermissions
{
    public class NewGroupPermissionsViewModel
    {
        public int Id { get; set; }
        public int PermissionId { get; set; }
        public string Permission_Name { get; set; }
        public int GroupId { get; set; }
        public string Group_Name { get; set; }
        public bool IsActive { get; set; }
    }
}
