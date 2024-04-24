using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLIS_DAL.ViewModels.CivilWithLegsDTOs;
using TLIS_DAL.ViewModels.LogisticalDTOs;

namespace TLIS_DAL.ViewModels.SideArmLibraryDTOs
{
    public class EditSideArmLibraryObject
    {
        public EditCivilSideArmlLibraryAttributes attributesActivatedLibrary { get; set; }
        public List<AddDdynamicAttributeInstallationValueViewModel> dynamicAttributes { get; set; }
        public LogisticalObject logisticalItems { get; set; }
        public class EditCivilSideArmlLibraryAttributes
        {
            public int Id { get; set; }
            public string Model { get; set; }
            public float Width { get; set; }
            public float Weight { get; set; }
            public float Length { get; set; }
            public float Height { get; set; }
            public float SpaceLibrary { get; set; }
            public string? Note { get; set; }
            public bool Active { get; set; }
            public bool Deleted { get; set; }
        }
        public class LogisticalObject
        {
            public int? Vendor { get; set; }
            public int? Supplier { get; set; }
            public int? Designer { get; set; }
            public int? Manufacturer { get; set; }
            public int? Consultant { get; set; }
            public int? Contractor { get; set; }
        }
    }
}
