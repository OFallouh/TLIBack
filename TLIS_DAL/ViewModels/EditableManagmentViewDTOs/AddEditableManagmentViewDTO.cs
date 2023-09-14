using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using TLIS_DAL.Models;

namespace TLIS_DAL.ViewModels.EditableManagmentViewDTOs
{
    public class AddEditableManagmentViewDTO
    {
        [Required]
        public string View { get; set; }

        [Required]
        public int Table1Id { get; set; }

        public int? Table2Id { get; set; }

        public string Description { get; set; }

    }
}
