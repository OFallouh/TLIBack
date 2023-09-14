using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TLIS_DAL.ViewModels.CivilSiteDateDTOs
{
    public class CivilSiteDateViewModel
    {
        public int Id { get; set; }
        public float? LongitudinalSpindleLengthm { get; set; }
        public float? HorizontalSpindleLengthm { get; set; }
        public DateTime InstallationDate { get; set; }
        [Required]
        public int allCivilInstId { get; set; }
        [Required]
        public string SiteCode { get; set; }
        public bool ReservedSpace { get; set; }
        public bool Dismantle { get; set; }
    }
}
