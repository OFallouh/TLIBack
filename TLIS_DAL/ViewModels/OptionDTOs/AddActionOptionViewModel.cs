using System;
using System.Collections.Generic;
using System.Text;

namespace TLIS_DAL.ViewModels.OptionDTOs
{
    public class AddActionOptionViewModel
    {
        public int? ActionId { get; set; }
        public int? ParentId { get; set; }
        public int? ItemStatusId { get; set; }
        public string Name { get; set; }
    }
}
