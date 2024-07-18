using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModels.CivilWithLegsDTOs;
using TLIS_DAL.ViewModels.LogisticalDTOs;

namespace TLIS_DAL.ViewModels.MW_BULibraryDTOs
{
    public class AddMWBULibraryObject
    {
        public MWBULibraryAttributes AttributesActivatedLibrary { get; set; }
        public AddLogisticalViewModel LogisticalItems { get; set; }
        public List<AddDdynamicAttributeInstallationValueViewModel> DynamicAttributes { get; set; }
        public class MWBULibraryAttributes
        {
            public string Model { get; set; }
            public string? Type { get; set; }
            public string? Note { get; set; }
            public string? L_W_H { get; set; }
            public float Length { get; set; }
            public float Width { get; set; }
            public float Height { get; set; }
            public float Weight { get; set; }
            public string? BUSize { get; set; }
            public int NumOfRFU { get; set; }
            public string? frequency_band { get; set; }
            public float? channel_bandwidth { get; set; }
            public string? FreqChannel { get; set; }
            public float SpaceLibrary { get; set; }
            public bool Active { get; set; }
            public bool Deleted { get; set; }
            public int diversityTypeId { get; set; }
     
        }
    }
}
