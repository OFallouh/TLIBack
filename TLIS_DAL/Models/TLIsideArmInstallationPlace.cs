﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Toolbelt.ComponentModel.DataAnnotations.Schema;

namespace TLIS_DAL.Models
{
    public class TLIsideArmInstallationPlace
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public bool Deleted { get; set; }
        public bool Disable { get; set; }
        public IEnumerable<TLIsideArm> sideArms { get; set; }
        public int CivilInstallationPlaceType { get; set; }
    }
}
