using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static TLIS_DAL.ViewModels.SideArmLibraryDTOs.EditSideArmLibraryObject;
using TLIS_DAL.ViewModels.CivilWithLegsDTOs;
using TLIS_DAL.Models;

namespace TLIS_DAL.ViewModels.MW_RFULibraryDTOs
{
    public class EditMWRFULibrary
    {
        public EditMWRFULibraryAttributes AttributesActivatedLibrary { get; set; }
        public LogisticalObject LogisticalItems { get; set; }
        public List<AddDdynamicAttributeInstallationValueViewModel> DynamicAttributes { get; set; }
        public class EditMWRFULibraryAttributes
        {
            public int Id { get; set; }
            public string Model { get; set; }
            public string? Note { get; set; } = " ";
            public float Weight { get; set; } = 0;
            public string? L_W_H { get; set; } = " ";
            public float Length { get; set; } = 0;
            public float Width { get; set; } = 0;
            public float Height { get; set; } = 0;
            public string? size { get; set; } = " ";
            public bool tx_parity { get; set; }
            public string? frequency_band { get; set; } = " ";
            public string? FrequencyRange { get; set; } = " ";
            public RFUType RFUType { get; set; }
            public string? VenferBoardName { get; set; } = " ";
            public float SpaceLibrary { get; set; } = 0;
            public bool Active { get; set; } 
            public bool Deleted { get; set; } = false;
            public int diversityTypeId { get; set; }
            public int boardTypeId { get; set; }

        }
    }
}