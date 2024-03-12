using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Toolbelt.ComponentModel.DataAnnotations.Schema;

namespace TLIS_DAL.ViewModels.OwnerDTOs
{
    public class OwnerViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
