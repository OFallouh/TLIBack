using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TLIS_DAL.ViewModels.CivilNonSteelDTOs
{
   public class CivilNonSteelLibraryViewModel
    {

        public int Id { get; set; }
        public string? Model { get; set; }
        public string? Note { get; set; }
        public float Hight { get; set; } 
        public float SpaceLibrary { get; set; } 
        public bool VerticalMeasured { get; set; } 
        public int? civilNonSteelTypeId { get; set; }
        public string? civilNonSteelType_Name { get; set; }
        public float NumberofBoltHoles { get; set; } = 0;
        public float Manufactured_Max_Load { get; set; }
        public bool Active { get; set; }
        public bool Deleted { get; set; }
    }
}
