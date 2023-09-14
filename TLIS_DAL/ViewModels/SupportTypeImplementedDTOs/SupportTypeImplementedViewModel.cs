using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using TLIS_DAL.ViewModels.CivilWithLegDTOs;
using Toolbelt.ComponentModel.DataAnnotations.Schema;

namespace TLIS_DAL.ViewModels.SupportTypeImplementedDTOs
{
   public class SupportTypeImplementedViewModel
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }

    }
}
