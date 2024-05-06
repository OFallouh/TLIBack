using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Toolbelt.ComponentModel.DataAnnotations.Schema;

namespace TLIS_DAL.Models
{
    public class TLImwDishLibrary
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Model { get; set; }
        public string? Description { get; set; }
        public string? Note { get; set; }
        public float Weight { get; set; }
        public string? dimensions { get; set; }
        public float Length { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }
        public float diameter { get; set; }
        public string? frequency_band { get; set; }
        public float SpaceLibrary { get; set; }
        public bool Active { get; set; }
        public bool Deleted { get; set; }
        [ForeignKey("TLIpolarityType")]
        public int polarityTypeId { get; set; }
        public TLIpolarityType polarityType { get; set; }
        [ForeignKey("TLIasType")]
        public int asTypeId { get; set; }
        public TLIasType asType { get; set; }
        public IEnumerable<TLImwDish> Mw_Dishes { get; set; }
    }
}
