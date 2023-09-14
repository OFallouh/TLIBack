using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TLIS_DAL.ViewModels.LoadPartDTOs
{
    public class AddLoadPartViewModel
    {
        [Required]
        public string Name { get; set; }
    }
}
