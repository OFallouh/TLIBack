using System;
using System.Collections.Generic;
using System.Text;

namespace TLIS_DAL.ViewModels.AttributeActivatedDTOs
{
    public class AddAttributeActivatedViewModel
    {
        
        public string Key { get; set; }
        public string Label { get; set; }
        public string Tabel { get; set; }
        public string Description { get; set; }
        public bool enable { get; set; }
    }
}
