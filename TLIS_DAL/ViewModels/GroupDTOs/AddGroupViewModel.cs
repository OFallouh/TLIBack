using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using TLIS_DAL.ViewModels.RoleDTOs;
using TLIS_DAL.ViewModels.UserDTOs;

namespace TLIS_DAL.ViewModels.GroupDTOs
{
    public class AddGroupViewModel
    {
        [Required]
        public string Name { get; set; }
        public int? ParentId { get; set; }
        public int? UpperId { get; set; }
        [Required]
        public int GroupType { get; set; }
        public bool Active { get; set; }
        public bool Deleted { get; set; }
        public int? ActorId { get; set; }
        public List<UserViewModel> UsersId { get; set; }
        public List<RoleViewModel> RolesId { get; set; }
    }
}
