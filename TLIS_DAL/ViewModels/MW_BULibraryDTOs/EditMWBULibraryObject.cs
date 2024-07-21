using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static TLIS_DAL.ViewModels.CivilWithLegLibraryDTOs.EditCivilWithLegsLibraryObject;
using TLIS_DAL.ViewModels.CivilWithLegsDTOs;
using TLIS_DAL.ViewModels.LogisticalDTOs;
using System.ComponentModel.DataAnnotations.Schema;
using TLIS_DAL.Models;
using static TLIS_DAL.ViewModels.SideArmLibraryDTOs.EditSideArmLibraryObject;

namespace TLIS_DAL.ViewModels.MW_BULibraryDTOs
{
    public class EditMWBULibraryObject
    {
        public EditMWBULibraryAttributes AttributesActivatedLibrary { get; set; }
        public LogisticalObject LogisticalItems { get; set; }
        public List<AddDdynamicAttributeInstallationValueViewModel> DynamicAttributes { get; set; }
        public class EditMWBULibraryAttributes
        {
            public int Id { get; set; }
            public string Model { get; set; }
            public string? Type { get; set; } = " ";
            public string? Note { get; set; } = " ";
            public string? L_W_H { get; set; } = " ";
            public float Length { get; set; } = 0;
            public float Width { get; set; } = 0;
            public float Height { get; set; } = 0;
            public float Weight { get; set; } = 0;
            public string? BUSize { get; set; } = " ";
            public int NumOfRFU { get; set; } = 0;
            public string? frequency_band { get; set; } = " ";
            public float channel_bandwidth { get; set; }
            public string? FreqChannel { get; set; } = " ";
            public float SpaceLibrary { get; set; } = 0;
            public bool Active { get; set; } = true;
            public bool Deleted { get; set; } = false;
            public int diversityTypeId { get; set; }

        }
    }
}
