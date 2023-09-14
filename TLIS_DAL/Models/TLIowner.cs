using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Toolbelt.ComponentModel.DataAnnotations.Schema;

namespace TLIS_DAL.Models
{
    public class TLIowner
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        public string OwnerName { get; set; }
        public string Remark { get; set; }
        public bool Deleted { get; set; }
        public bool Disable { get; set; }
        public IEnumerable<TLIcivilWithLegs> CivilWithLegs { get; set; }
        public IEnumerable<TLIcivilWithoutLeg> CivilWithoutLegs { get; set; }
        public IEnumerable<TLIRadioRRU> RadioRRUs { get; set; }
        public IEnumerable<TLIradioAntenna> radioAntennas { get; set; }
        public IEnumerable <TLIsideArm> SideArms { get; set; }
        public IEnumerable<TLImwBU> Mw_Bu { get; set; }
        public IEnumerable<TLImwODU> Mw_ODU { get; set; }
        public IEnumerable<TLIradioOther> radioOthers { get; set; }
        public IEnumerable<TLIpower> powers { get; set; }
        public IEnumerable<TLIcivilNonSteel> civilNonSteel { get; set; }
        public IEnumerable<TLImwDish> mwDish { get; set; }
        public IEnumerable<TLImwRFU> mwRFU { get; set; }
    }
}
