using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TLIS_DAL.ViewModels.BaseCivilWithLegsTypeDTOs
{
    public class AddBaseCivilWithLegsTypeViewModel
    {
        [Required]
        public string Name { get; set; }
    }
}
