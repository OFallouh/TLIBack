using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using TLIS_DAL.Models;

namespace TLIS_DAL.ViewModels.MW_BULibraryDTOs
{
    public class MW_BULibraryViewModel
    {
        public int Id { get; set; }
        public string Model { get; set; }
        public string Type { get; set; }
        public string Note { get; set; }
        public string L_W_H { get; set; }
        public float Length { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }
        public string frequency_band { get; set; }
        public float? channel_bandwidth { get; set; }
        public float SpaceLibrary { get; set; }
        public bool Active { get; set; }
        public bool Deleted { get; set; }
        public int? diversityTypeId { get; set; }
        public string diversityType_Name { get; set; }
        public string FreqChannel { get; set; }
        public double Weight { get; set; }
        public string BUSize { get; set; }
        public int NumOfRFU { get; set; }



    }
}
