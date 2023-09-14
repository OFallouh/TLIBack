using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using TLIS_DAL.Models;

namespace TLIS_DAL.ViewModels.AttributeViewManagmentDTOs
{
    public class AddAttributeViewManagmentDTO
    {
        [Required]
        public int EditableManagmentViewId { get; set; }

        public int? DynamicAttId { get; set; }

        public int? AttributeActivatedId { get; set; }
        public bool Enable { get; set; }
    }
}
