using System;
using System.Collections.Generic;
using System.Text;
using TLIS_DAL.ViewModels.LoadOtherDTOs;
using TLIS_DAL.ViewModels.LocationTypeDTOs;
using TLIS_DAL.ViewModels.MW_BUDTOs;
using TLIS_DAL.ViewModels.MW_DishDTOs;
using TLIS_DAL.ViewModels.MW_ODUDTOs;
using TLIS_DAL.ViewModels.Mw_OtherDTOs;
using TLIS_DAL.ViewModels.MW_RFUDTOs;
using TLIS_DAL.ViewModels.PowerDTOs;
using TLIS_DAL.ViewModels.RadioAntennaDTOs;
using TLIS_DAL.ViewModels.RadioOtherDTOs;
using TLIS_DAL.ViewModels.RadioRRUDTOs;
using TLIS_DAL.ViewModels.SideArmDTOs;

namespace TLIS_DAL.ViewModels.CivilLoadsDTOs
{
    using System.Collections.Generic;
    using System.Text.Json.Serialization;

    public class CivilLoads
    {
        public CivilLoads()
        {
            TLIsideArm = new List<LoadandsidearmViewDto>();
            TLImwODU = new List<LoadandsidearmViewDto>();
            TLImwDish = new List<LoadandsidearmViewDto>();
            TLImwRFU = new List<LoadandsidearmViewDto>();
            TLImwBU = new List<LoadandsidearmViewDto>();
            TLImwOther = new List<LoadandsidearmViewDto>();
            TLIradioAntenna = new List<LoadandsidearmViewDto>();
            TLIRadioRRU = new List<LoadandsidearmViewDto>();
            TLIradioOther = new List<LoadandsidearmViewDto>();
            TLIpower = new List<LoadandsidearmViewDto>();
            TLIloadOther = new List<LoadandsidearmViewDto>();
        }

        [JsonPropertyName("TLIsideArm")]
        public List<LoadandsidearmViewDto> TLIsideArm { get; set; }
        [JsonPropertyName("TLImwODU")]
        public List<LoadandsidearmViewDto> TLImwODU { get; set; }
        [JsonPropertyName("TLImwDish")]
        public List<LoadandsidearmViewDto> TLImwDish { get; set; }
        [JsonPropertyName("TLImwRFU")]
        public List<LoadandsidearmViewDto> TLImwRFU { get; set; }
        [JsonPropertyName("TLImwBU")]
        public List<LoadandsidearmViewDto> TLImwBU { get; set; }
        [JsonPropertyName("TLImwOther")]
        public List<LoadandsidearmViewDto> TLImwOther { get; set; }
        [JsonPropertyName("TLIradioAntenna")]
        public List<LoadandsidearmViewDto> TLIradioAntenna { get; set; }
        [JsonPropertyName("TLIRadioRRU")]
        public List<LoadandsidearmViewDto> TLIRadioRRU { get; set; }
        [JsonPropertyName("TLIradioOther")]
        public List<LoadandsidearmViewDto> TLIradioOther { get; set; }
        [JsonPropertyName("TLIpower")]
        public List<LoadandsidearmViewDto> TLIpower { get; set; }
        [JsonPropertyName("TLIloadOther")]
        public List<LoadandsidearmViewDto> TLIloadOther { get; set; }

        public class LoadandsidearmViewDto
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public float EquivalentSpace { get; set; }
            public bool ReservedSpace { get; set; }
        }
    }

}
