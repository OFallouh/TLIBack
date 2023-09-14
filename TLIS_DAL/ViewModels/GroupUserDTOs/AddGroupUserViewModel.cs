using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TLIS_DAL.ViewModels.GroupUserDTOs
{
    public class AddGroupUserViewModel
    {
        [Required]
        public int groupId { get; set; }
        [Required]
        public int userId { get; set; }
        public bool Active { get; set; } = true;
        public bool Deleted { get; set; } = false;
    }
}
