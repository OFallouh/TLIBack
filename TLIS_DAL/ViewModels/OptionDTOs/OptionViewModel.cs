using System;
using System.Collections.Generic;
using System.Text;
using TLIS_DAL.Models;

namespace TLIS_DAL.ViewModels.OptionDTOs
{
   public class OptionViewModel
    {
        public int Id { get; set; }
        public int? ActionId { get; set; }
        //public TLIaction Action { get; set; }
        public int? ParentId { get; set; }
        //public TLIactionOption Parent { get; set; }
        public string Name { get; set; }
        public int? ItemStatusId { get; set; }
        //public bool AllowNote { get; set; }
        //public bool NoteIsMandatory { get; set; }
    }
}
