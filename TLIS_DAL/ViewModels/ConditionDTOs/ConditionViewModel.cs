using System;
using System.Collections.Generic;
using System.Text;
using TLIS_DAL.Models;

namespace TLIS_DAL.ViewModels.ConditionDTOs
{
    public class ConditionViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class AddConditionViewModel
    {
        public string Name { get; set; }
    }
}
