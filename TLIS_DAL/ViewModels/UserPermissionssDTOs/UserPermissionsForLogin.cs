using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLIS_DAL.ViewModels.NewPermissionsDTOs.Permissions;

namespace TLIS_DAL.ViewModels.UserPermissionssDTOs
{
    public class UserPermissionsForLogin
    {
        public string Token { get; set; }
        public List<NewPermissionsViewModel> Permissions { get; set; }
    }
}
