﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLIS_DAL.ViewModels.CivilWithLegsDTOs;
using TLIS_DAL.ViewModels.DynamicAttDTOs;

namespace TLIS_DAL.ViewModels.CivilLoadsDTOs
{
    public class GetForAddCivilLoadObject
    {
        public IEnumerable<BaseInstAttViews> LibraryAttribute { get; set; } = new List<BaseInstAttViews>();

        public IEnumerable<BaseInstAttViews> InstallationAttributes { get; set; } = new List<BaseInstAttViews>();

        public IEnumerable<BaseInstAttViews> CivilLoads { get; set; } = new List<BaseInstAttViews>();
        public IEnumerable<BaseInstAttViewDynamic> dynamicAttribute { get; set; } = new List<BaseInstAttViewDynamic>();
    }
}
