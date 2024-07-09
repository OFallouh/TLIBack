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
using static TLIS_DAL.ViewModels.GeneratorDTOs.AddGeneratorInstallationObject;

namespace TLIS_DAL.ViewModels.GeneratorDTOs
{
    public class EditGeneratorInstallationObject
    {
        public LibraryAttributesGenerator GeneratorType { get; set; }
        public EditinstallationAttributesGenerator installationAttributes { get; set; }
        public AddOtherInSiteViewModel OtherInSite { get; set; }
        public AddOtherInventoryDistanceViewModel OtherInventoryDistance { get; set; }
        public List<AddDdynamicAttributeInstallationValueViewModel> dynamicAttribute { get; set; }
    }
    public class EditinstallationAttributesGenerator
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string SerialNumber { get; set; }
        public int NumberOfFuelTanks { get; set; }
        public bool BaseExisting { get; set; }
        public float SpaceInstallation { get; set; }
        public string VisibleStatus { get; set; }
        public int? BaseGeneratorTypeId { get; set; }

    }
}
