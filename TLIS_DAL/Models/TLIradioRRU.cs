﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace TLIS_DAL.Models
{
    public class TLIRadioRRU
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string SerialNumber { get; set; }
        public string Name { get; set; }
        public string? Notes { get; set; }
        public float HeightBase { get; set; }
        public float HeightLand { get; set; }
        public float SpaceInstallation { get; set; }
        public string? VisibleStatus { get; set; }
        public TLIradioRRULibrary radioRRULibrary { get; set; }
        public int radioRRULibraryId { get; set; }
        public TLIowner owner { get; set; }
        public int? ownerId { get; set; }
        public TLIradioAntenna radioAntenna { get; set; }
        public int? radioAntennaId { get; set; }
        public float CenterHigh { get; set; }
        public float HBA { get; set; }
        public float HieghFromLand { get; set; }
        public float EquivalentSpace { get; set; }
        public TLIinstallationPlace installationPlace { get; set; }
        public int? installationPlaceId { get; set; }
        public float Azimuth { get; set; }
        public IEnumerable<TLIallLoadInst> allLoadInsts { get; set; }
        public IEnumerable<TLIantennaRRUInst> antennaRRUInst { get; set; }
    }
}
