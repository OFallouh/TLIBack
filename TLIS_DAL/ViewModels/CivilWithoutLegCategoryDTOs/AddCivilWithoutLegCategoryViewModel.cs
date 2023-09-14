using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TLIS_DAL.ViewModels.CivilWithoutLegCategoryDTOs
{
    public class AddCivilWithoutLegCategoryViewModel
    {
        [Required]
        public string Name { get; set; }
    }
}
