using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLIS_DAL.ViewModels.CivilWithLegsDTOs;
using TLIS_DAL.ViewModels.LogisticalDTOs;
using static TLIS_DAL.ViewModels.SideArmLibraryDTOs.EditSideArmLibraryObject;

namespace TLIS_DAL.ViewModels
{
    internal class AddSideArmLibraryObject
    {
        public SideArmlLibraryAttributes attributesActivatedLibrary { get; set; }
        public List<AddDdynamicAttributeInstallationValueViewModel> dynamicAttributes { get; set; }
        public LogisticalObject logisticalItems { get; set; }
        public class SideArmlLibraryAttributes
        {
            public string Model { get; set; }
            public float Width { get; set; }
            public float? Weight { get; set; }
            public float Length { get; set; }
            public float Height { get; set; }
            public float SpaceLibrary { get; set; }
            public string Note { get; set; }
            public bool Active { get; set; }
            public bool Deleted { get; set; }
        }
    }
}
