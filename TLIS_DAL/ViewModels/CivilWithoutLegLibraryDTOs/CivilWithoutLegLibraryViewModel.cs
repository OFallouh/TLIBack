using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModels.CivilSteelSupportCategoryDTOs;

namespace TLIS_DAL.ViewModels.CivilWithoutLegDTOs
{
    public class CivilWithoutLegLibraryViewModel
    {
        public int Id { get; set; }

        public string Model { get; set; }

        public string? Note { get; set; }
        public float Height_Designed { get; set; } 
        public float Max_Load { get; set; } 
        public float SpaceLibrary { get; set; } 
        //public string Civil_Type_Name { get; set; }
        public bool Active { get; set; } = false;
        public bool Deleted { get; set; } = false;
        public float HeightBase { get; set; } 
        public string? Prefix { get; set; }
        public int CivilSteelSupportCategoryId { get; set; }
        public string CivilSteelSupportCategory_Name { get; set; }
        public int? InstCivilwithoutLegsTypeId { get; set; }
        public string? InstCivilwithoutLegsType_Name { get; set; }
        public int? CivilWithoutLegCategoryId { get; set; }
        public string? CivilWithoutLegCategory_Name { get; set; }
        public int? structureTypeId { get; set; }
        public float Manufactured_Max_Load { get; set; } 
        public string? structureType_Name { get; set; }
        public string? WidthVariation { get; set; }


    }
}
