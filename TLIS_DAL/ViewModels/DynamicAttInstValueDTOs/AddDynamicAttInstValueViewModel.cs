using System;
using System.Collections.Generic;
using System.Text;

namespace TLIS_DAL.ViewModels.DynamicAttInstValueDTOs
{
    public class AddDynamicAttInstValueViewModel
    {
        public int DynamicAttId { get; set; }
        public string ValueString { get; set; }
        public double? ValueDouble { get; set; }
        public DateTime? ValueDateTime { get; set; }
        public bool? ValueBoolean { get; set; }
        //public int? dynamicListValuesId { get; set; }
        //public int? CivilInstallationId { get; set; }
        //public bool disable { get; set; }
        //public int? LoadInstallationId { get; set; }
        //public int? OtherInventoryLibId { get; set; }
    }
}
