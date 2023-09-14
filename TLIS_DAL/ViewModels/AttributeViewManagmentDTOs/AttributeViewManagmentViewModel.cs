using System;
using System.Collections.Generic;
using System.Text;

namespace TLIS_DAL.ViewModels.AttributeViewManagmentDTOs
{
    public class AttributeViewManagmentViewModel
    {
        public int  AttributeViewManagmentId { get; set; }
        public bool Enable { get; set; }
        public int? AttributeActivatedId { get; set; }
        public int? DynamicAttId { get; set; }
        public string Key { get; set; }
    }
}
