using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Toolbelt.ComponentModel.DataAnnotations.Schema;

namespace TLIS_DAL.ViewModels.OwnerDTOs
{
    public class AddOwnerViewModel
    {
        [Required]
        public string OwnerName { get; set; }
        public string Remark { get; set; }
        public bool Deleted { get; set; }
        public bool Disable { get; set; }
    }
}
