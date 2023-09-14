using System;
using System.Collections.Generic;
using System.Text;
using TLIS_DAL.ViewModels.CabinetDTOs;

namespace TLIS_DAL.ViewModels.CivilWithLegsDTOs
{
    public class DynamicAttributesDisplayOnTable
    {
        public dynamic Civilwithlegs { get; set; }
        public dynamic CivilWithLegLibrary { get; set; }
        public IEnumerable<DynamicAttDto> DynamicAttList { get; set; }
    }
}
