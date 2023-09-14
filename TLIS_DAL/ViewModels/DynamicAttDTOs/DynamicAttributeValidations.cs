using System;
using System.Collections.Generic;
using System.Text;
using TLIS_DAL.ViewModels.DependencyDTOs;
using TLIS_DAL.ViewModels.RuleDTOs;
using TLIS_DAL.ViewModels.ValidationDTOs;

namespace TLIS_DAL.ViewModels.DynamicAttDTOs
{
    public class DynamicAttributeValidations
    {
        public DynamicAttributeValidations()
        {
            GeneralValidation = new GeneralValidation();
            Dependency = new DynamicAttributeDependency();
            Dependency.Rules = new List<DynamicAttributeDependencyRule>();
        }
        public GeneralValidation GeneralValidation { get; set; }
        public DynamicAttributeDependency Dependency { get; set; }
    }
}
