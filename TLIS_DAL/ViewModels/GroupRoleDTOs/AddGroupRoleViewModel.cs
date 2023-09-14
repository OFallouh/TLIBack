using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TLIS_DAL.ViewModels.GroupRoleDTOs
{
    public class AddGroupRoleViewModel
    {
        [Required]
        public int groupId { get; set; }
        [Required]
        public int roleId { get; set; }
        public bool Active { get; set; }
        public bool Deleted { get; set; }
    }
}
