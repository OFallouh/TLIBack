using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TLIS_DAL.ViewModels.EditableManagmentViewDTOs
{
    public class EditableManagmentViewModel
    {
        [Required]
        public int Id { get; set; }
        public string View { get; set; }
        public int Table1Id { get; set; }
        public int? Table2Id { get; set; }
    }
}
