using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModels.CivilSiteDateDTOs;
using TLIS_DAL.ViewModels.CivilSupportDistanceDTOs;
using TLIS_DAL.ViewModels.CivilWithLegsDTOs;
using TLIS_DAL.ViewModels.LegDTOs;
using TLIS_DAL.ViewModels.OtherInSiteDTOs;
using TLIS_DAL.ViewModels.OtherInventoryDistanceDTOs;

namespace TLIS_DAL.ViewModels.GeneratorDTOs
{
    public class AddGeneratorInstallationObject
    {
        public LibraryAttributesGenerator GeneratorType { get; set; }
        public installationAttributesGenerator installationAttributes { get; set; }
        public AddOtherInSiteViewModel OtherInSite { get; set; }
        public AddOtherInventoryDistanceViewModel OtherInventoryDistance { get; set; }
        public List<AddDdynamicAttributeInstallationValueViewModel> dynamicAttribute { get; set; }
        public class LibraryAttributesGenerator
        {
            public int GeneratorLibraryId { get; set; }
        }
       
        public class installationAttributesGenerator
        {
            public string Name { get; set; }
            public string SerialNumber { get; set; }
            public int NumberOfFuelTanks { get; set; }
            public bool BaseExisting { get; set; }
            public float SpaceInstallation { get; set; }
            public string VisibleStatus { get; set; }
            public int? BaseGeneratorTypeId { get; set; }

        }
    }
}
