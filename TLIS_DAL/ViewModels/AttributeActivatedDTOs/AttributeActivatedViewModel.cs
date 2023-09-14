using System;
using System.Collections.Generic;
using System.Text;

namespace TLIS_DAL.ViewModels.AttributeActivatedDTOs
{
    public class AttributeActivatedViewModel
    {
        public int Id { get; set; }
        public string Key { get; set; }
        public string Label { get; set; }
        public string Tabel { get; set; }
        public string Description { get; set; }
        public bool Required { get; set; }
        public bool Manage { get; set; }
        public bool enable { get; set; }
        public bool AutoFill { get; set; }
        public string DataType { get; set; }
    }
}
