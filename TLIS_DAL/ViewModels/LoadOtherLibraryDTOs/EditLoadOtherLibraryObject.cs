using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLIS_DAL.ViewModels.CivilWithLegsDTOs;
using TLIS_DAL.ViewModels.LogisticalDTOs;
using static TLIS_DAL.ViewModels.SideArmLibraryDTOs.EditSideArmLibraryObject;

namespace TLIS_DAL.ViewModels.LoadOtherLibraryDTOs
{
    public class EditLoadOtherLibraryObject
    {
        public EditLoadOtherLibraryAttribute AttributesActivatedLibrary { get; set; }
        public LogisticalObject LogisticalItems { get; set; }
        public List<AddDdynamicAttributeInstallationValueViewModel> DynamicAttributes { get; set; }
        public class EditLoadOtherLibraryAttribute
        {
            public int Id { get; set; }
            public string Model { get; set; }
            public string? Note { get; set; } = " ";
            public float Length { get; set; } = 0;
            public float Width { get; set; } = 0;
            public float Height { get; set; } = 0;
            public float SpaceLibrary { get; set; } = 0;
            public bool Active { get; set; } 
            public bool Deleted { get; set; } = false;
        }
    }
}
