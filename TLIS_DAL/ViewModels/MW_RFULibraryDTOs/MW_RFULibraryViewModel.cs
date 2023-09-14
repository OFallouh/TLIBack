using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TLIS_DAL.ViewModels.MW_RFUDTOs
{
   public class MW_RFULibraryViewModel
    {
        public int Id { get; set; }
        public string Model { get; set; }
        public string RFUType { get; set; }
        public string Note { get; set; }
        public float? Weight { get; set; }
        public string L_W_H { get; set; }
        public float Length { get; set; }
        public string VenferBoardName { get; set; }
        public string FrequencyRange { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }
        public string size { get; set; }
        public string tx_parity { get; set; }
        public string frequency_band { get; set; }
        public float SpaceLibrary { get; set; }
        public bool Active { get; set; }
        public bool Deleted { get; set; }
        public int? diversityTypeId { get; set; }
        public string diversityType_Name { get; set; }
        public int? boardTypeId { get; set; }
        public string boardType_Name { get; set; }
    }
}
