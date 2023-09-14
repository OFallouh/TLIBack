using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using TLIS_DAL.ViewModelBase;
using TLIS_DAL.ViewModels.OtherInSiteDTOs;
using TLIS_DAL.ViewModels.OtherInventoryDistanceDTOs;

namespace TLIS_DAL.ViewModels.GeneratorDTOs
{
    public class EditGeneratorViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string SerialNumber { get; set; }
        public int? NumberOfFuelTanks { get; set; }
        public string LocationDescription { get; set; }
        public bool? BaseExisting { get; set; }
        public float SpaceInstallation { get; set; }
        public string VisibleStatus { get; set; }
        public int? BaseGeneratorTypeId { get; set; }
        public int? GeneratorLibraryId { get; set; }
        public EditOtherInSiteViewModel TLIotherInSite { get; set; }
        public EditOtherInventoryDistanceViewModel TLIotherInventoryDistance { get; set; }
        public List<BaseInstAttView> DynamicInstAttsValue { get; set; }
    }
}
