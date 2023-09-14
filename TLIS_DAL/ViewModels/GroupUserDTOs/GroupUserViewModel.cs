using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using TLIS_DAL.ViewModels.RoleDTOs;
using TLIS_DAL.ViewModels.UserDTOs;

namespace TLIS_DAL.ViewModels.GroupUserDTOs
{
    public class GroupUserViewModel
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public int groupId { get; set; }
        [Required]
        public int userId { get; set; }
        [Required]
        public bool Active { get; set; }
        [Required]
        public bool Deleted { get; set; }
    }
}
