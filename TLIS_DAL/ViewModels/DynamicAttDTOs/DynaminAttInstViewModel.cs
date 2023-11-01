using System;
using System.Collections.Generic;
using System.Text;
using TLIS_DAL.ViewModels.DependencyDTOs;

namespace TLIS_DAL.ViewModels.DynamicAttDTOs
{
    public class DynaminAttInstViewModel
    {
        public int Id { get; set; }
        public string Key { get; set; }
        public int? DataTypeId { get; set; }
        public string DataType { get; set; }
        public string ? ValueString { get; set; }
        public DateTime ? ValueDateTime { get; set; }
        public double ? ValueDouble { get; set; }
        public bool ? ValueBoolean { get; set; }

        public bool Required { get; set; }
        public bool disable { get; set; }
        //public List<ValidationViewModel> Validations { get; set; }
        //public List<DependencyViewModel> Dependencies { get; set; }
    }
}
