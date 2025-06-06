﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModels.CivilWithLegsDTOs;
using TLIS_DAL.ViewModels.LogisticalDTOs;

namespace TLIS_DAL.ViewModels.CabinetTelecomLibraryDTOs
{
    public class EditCabinetTelecomLibraryObject
    {
        public EditCabinetTelecomLibraryAttributes AttributesActivatedLibrary { get; set; }
        public List<AddDdynamicAttributeInstallationValueViewModel> DynamicAttributes { get; set; }
        public AddLogisticalViewModel LogisticalItems { get; set; }
        public class EditCabinetTelecomLibraryAttributes
        {
            public int Id { get; set; }
            public string Model { get; set; }
            public float MaxWeight { get; set; } = 0;
            public string? LayoutCode { get; set; } = " ";
            public string? Dimension_W_D_H { get; set; } = " ";
            public float Width { get; set; } = 0;
            public float Depth { get; set; } = 0;
            public float Height { get; set; } = 0;
            public float SpaceLibrary { get; set; } = 0;
            public int TelecomTypeId { get; set; }
            public bool Active { get; set; } 
            public bool Deleted { get; set; } = false;

        }
    }
}
