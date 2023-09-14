using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Toolbelt.ComponentModel.DataAnnotations.Schema;

namespace TLIS_DAL.ViewModels.TaskStatusDTOs
{
    public class TaskStatusViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool Disable { get; set; }
        public DateTime DisableDate { get; set; }
    }
}
