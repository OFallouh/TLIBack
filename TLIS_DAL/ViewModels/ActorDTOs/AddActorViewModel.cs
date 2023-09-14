using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TLIS_DAL.ViewModels.ActorDTOs
{
    public class AddActorViewModel
    {
        [Required]
        public string Name { get; set; }
    }
}
