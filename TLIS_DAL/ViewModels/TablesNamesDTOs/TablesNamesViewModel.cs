using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TLIS_DAL.ViewModels.TablesNamesDTOs
{
    public class TablesNamesViewModel
    {
        public int Id { get; set; }
        [Required]
        public string TableName { get; set; }
        public int? tablePartNameId { get; set; }
    }
}
