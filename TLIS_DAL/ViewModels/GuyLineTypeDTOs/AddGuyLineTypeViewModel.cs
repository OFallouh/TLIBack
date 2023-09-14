using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Toolbelt.ComponentModel.DataAnnotations.Schema;

namespace TLIS_DAL.ViewModels.GuyLineTypeDTOs
{
    public class AddGuyLineTypeViewModel
    {
        [Required]
        public string Name { get; set; }
    }
}
