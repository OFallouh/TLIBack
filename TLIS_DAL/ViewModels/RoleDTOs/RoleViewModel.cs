using System;
using System.Collections.Generic;
using System.Text;
using TLIS_DAL.ViewModels.NewPermissionsDTOs.Permissions;
using TLIS_DAL.ViewModels.PermissionDTOs;

namespace TLIS_DAL.ViewModels.RoleDTOs
{
    public class RoleViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool Active { get; set; }
        public bool Deleted { get; set; }
        public List<string> Permissions { get; set; }
    }
}
