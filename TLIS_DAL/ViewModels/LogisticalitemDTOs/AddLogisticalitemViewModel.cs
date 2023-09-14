using System;
using System.Collections.Generic;
using System.Text;
using TLIS_DAL.ViewModels.CivilWithLegDTOs;
using TLIS_DAL.ViewModels.CivilWithoutLegDTOs;
using TLIS_DAL.ViewModels.MW_BULibraryDTOs;

namespace TLIS_DAL.ViewModels.LogisticalitemDTOs
{
    public class AddLogisticalitemViewModel
    {
        public string Name { get; set; }
        public bool IsLib { get; set; }
        public int? logisticalId { get; set; }
        public int RecordId { get; set; }

        public int tablesNamesId { get; set; }
       // public IEnumerable<CivilWithLegLibraryViewModel> civilWithLeg { get; set; }
       //public IEnumerable<CivilWithoutLegLibraryViewModel> civilWithoutLeg { get; set; }
       // public IEnumerable<MW_BULibraryViewModel> MW_BU { get; set; }

    }
}
