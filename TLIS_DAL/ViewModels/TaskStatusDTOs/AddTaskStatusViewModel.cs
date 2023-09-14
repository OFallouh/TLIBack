using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Toolbelt.ComponentModel.DataAnnotations.Schema;

namespace TLIS_DAL.ViewModels.TaskStatusDTOs
{
    public class AddTaskStatusViewModel
    {
        [Required]
        public string Name { get; set; }
        public bool Disable { get; set; } = false;
        public DateTime DisableDate { get; set; }
    }
}
