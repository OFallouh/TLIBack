using System;
using System.Collections.Generic;
using System.Text;
using TLIS_DAL.ViewModels.CivilWithLegDTOs;

namespace TLIS_DAL.ViewModels.CivilSteelSupportCategoryDTOs
{
   public  class EditCivilSteelSupportCategory
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public IEnumerable<CivilWithLegLibraryViewModel> civilWithLeg { get; set; }
    }
}
