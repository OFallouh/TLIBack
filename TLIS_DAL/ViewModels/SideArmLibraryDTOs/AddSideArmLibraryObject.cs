﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLIS_DAL.ViewModels.CivilWithLegsDTOs;
using TLIS_DAL.ViewModels.LogisticalDTOs;

namespace TLIS_DAL.ViewModels.SideArmLibraryDTOs
{
    public class AddSideArmLibraryObject
    {
        public SideArmLibraryAttributes attributesActivatedLibrary { get; set; }
        public AddLogisticalViewModels LogisticalItems { get; set; }
        public List<AddDdynamicAttributeInstallationValueViewModel> dynamicAttributes { get; set; }
        public class SideArmLibraryAttributes
        {
            public string Model { get; set; }
            public float Width { get; set; } = 0;
            public float Weight { get; set; } = 0;
            public float Length { get; set; } = 0;
            public float Height { get; set; } = 0;
            public float SpaceLibrary { get; set; } = 0;
            public string? Note { get; set; }
            public bool Active { get; set; } = true;
            public bool Deleted { get; set; } = false;
        }
    }
}
