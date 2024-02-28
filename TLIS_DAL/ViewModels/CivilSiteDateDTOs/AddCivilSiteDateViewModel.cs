using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TLIS_DAL.ViewModels.CivilSiteDateDTOs
{
    public class AddCivilSiteDateViewModel
    {
        public float LongitudinalSpindleLengthm { get; set; } = 0;
        public float HorizontalSpindleLengthm { get; set; } = 0;
        public DateTime InstallationDate { get; set; }=DateTime.Now;
        public bool ReservedSpace { get; set; } = false;
        public bool Dismantle { get; set; } = false;
    }
}
