using System;
using System.Collections.Generic;
using System.Text;
using TLIS_DAL.ViewModels.CivilWithLegDTOs;
using TLIS_DAL.ViewModels.LogisticalitemDTOs;

namespace TLIS_DAL.ViewModels.CivilTypeDTOs
{
    public class EditCivilTypeViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public IEnumerable<CivilWithLegLibraryViewModel> civilWithLeg { get; set; }
        public IEnumerable<LogisticalitemViewModel> logisticalitem { get; set; }

    }
}
