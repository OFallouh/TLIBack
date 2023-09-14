using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Toolbelt.ComponentModel.DataAnnotations.Schema;

namespace TLIS_DAL.ViewModels.CivilTypeDTOs
{
    public class CivilTypeViewModel
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
    }
}
