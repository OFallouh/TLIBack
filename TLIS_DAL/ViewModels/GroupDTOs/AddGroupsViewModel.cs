using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLIS_DAL.ViewModels.RoleDTOs;
using TLIS_DAL.ViewModels.UserDTOs;

namespace TLIS_DAL.ViewModels.GroupDTOs
{
    public class AddGroupsViewModel
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public int? ParentId { get; set; }
        public string ParentName { get; set; }
        public int? UpperId { get; set; }
        public string UpperName { get; set; }
        [Required]
        public int GroupType { get; set; }
        [Required]
        public bool Active { get; set; }
        [Required]
        public bool Deleted { get; set; }
        public int? ActorId { get; set; }
        public string ActorName { get; set; }
        public List<UserNameViewModel> Users { get; set; } = new List<UserNameViewModel>();
        public List<RoleViewModel> Roles { get; set; } = new List<RoleViewModel>();
    }
}
