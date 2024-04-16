using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Toolbelt.ComponentModel.DataAnnotations.Schema;

namespace TLIS_DAL.Models
{
    public class TLIcivilNonSteelLibrary
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Model { get; set; }
        public string? Prefix { get; set; }
        public string? Note { get; set; }
        public float Hight { get; set; }
        public float SpaceLibrary { get; set; }
        public bool VerticalMeasured { get; set; }
        public TLIcivilNonSteelType civilNonSteelType { get; set; }
        public int civilNonSteelTypeId { get; set; }
        public float NumberofBoltHoles { get; set; }
        public bool Active { get; set; }
        public bool Deleted { get; set; }
        public string? WidthVariation { get; set; }
        public float Manufactured_Max_Load { get; set; }
        public IEnumerable<TLIcivilNonSteel> civilNonSteel { get; set; }
    }
}
