using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TLIS_DAL.ViewModels.BaseCivilWithLegsTypeDTOs
{
    public class EditBaseCivilWithLegsTypeViewModel
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
    }
}
