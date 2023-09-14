using System;
using System.Collections.Generic;
using System.Text;
using TLIS_DAL.ViewModels.CivilSteelSupportCategoryDTOs;
using TLIS_DAL.ViewModels.CivilTypeDTOs;
using TLIS_DAL.ViewModels.LogisticalitemDTOs;
using TLIS_DAL.ViewModels.Related_List;
using TLIS_DAL.ViewModels.SectionsLegTypeDTOs;
using TLIS_DAL.ViewModels.StructureTypeDTOs;
using TLIS_DAL.ViewModels.SupportTypeDesignedDTOs;

namespace TLIS_DAL.ViewModels.CivilWithLegLibraryDTOs
{
    public class CivilWithLegListFiltter
    {

        public IEnumerable<RelatedList> supportTypeDesigned { get; set; }
        public IEnumerable<RelatedList> sectionsLegType { get; set; }
        public IEnumerable<RelatedList> structureType { get; set; }
        public IEnumerable<RelatedList> civilSteelSupportCategory { get; set; } 
        public IEnumerable<RelatedList> logisticalitem { get; set; }

    }
}
