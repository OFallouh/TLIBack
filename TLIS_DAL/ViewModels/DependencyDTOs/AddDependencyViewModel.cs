using System;
using System.Collections.Generic;
using System.Text;
using TLIS_DAL.ViewModels.DependencyRowDTOs;
using TLIS_DAL.ViewModels.DynamicAttDTOs;
using TLIS_DAL.ViewModels.DynamicListValuesDTOs;

namespace TLIS_DAL.ViewModels.DependencyDTOs
{
    public class AddDependencyViewModel
    {
        //public int? DynamicAttId { get; set; }
        public string Key { get; set; }
        public bool LibraryAtt { get; set; }
        public int? DataTypeId { get; set; }
        public string Description { get; set; }
        public int tablesNamesId { get; set; }
        public int? CivilWithoutLegCategoryId { get; set; }
        public bool Required { get; set; }
        public bool disable { get; set; }
        public string TableName { get; set; }
        public List<ValidationViewModel> validations { get; set; }

        // General Default Value...
        public string StringDefaultValue { get; set; }
        public double? DoubleDefaultValue { get; set; }
        public DateTime? DateTimeDefaultValue { get; set; }
        public bool? BooleanDefaultValue { get; set; }

        // After Selecting Dependency and User Want to Select New Result (Another DefaultValue)...
        public string StringResult { get; set; }
        public double? DoubleResult { get; set; }
        public DateTime? DateTimeResult { get; set; }
        public bool? BooleanResult { get; set; }
        public List<DependencyViewModel> Dependencies { get; set; }
        // public string? DefaultValue { get; set; }
    }
}
