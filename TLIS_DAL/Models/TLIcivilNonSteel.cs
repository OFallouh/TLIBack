using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Toolbelt.ComponentModel.DataAnnotations.Schema;

namespace TLIS_DAL.Models
{
    public class TLIcivilNonSteel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; }
        public double CurrentLoads { get; set; }
        public float SpaceInstallation { get; set; }
        public int CivilNonSteelLibraryId { get; set; }
        public TLIcivilNonSteelLibrary CivilNonsteelLibrary { get; set; }
        public int? ownerId { get; set; }
        public TLIowner? owner { get; set; }
        public int? supportTypeImplementedId { get; set; }
        public TLIsupportTypeImplemented? supportTypeImplemented { get; set; }
        public int? locationTypeId { get; set; }
        public TLIlocationType? locationType { get; set; }
        public float locationHeight { get; set; }
        public float BuildingMaxLoad { get; set; }
        public float CivilSupportCurrentLoad { get; set; }
        public float H2Height { get; set; }
        public float Support_Limited_Load { get; set; }
        public float CenterHigh { get; set; }
        public float HBA { get; set; }
        public float HieghFromLand { get; set; }
        public float EquivalentSpace { get; set; }

        public IEnumerable<TLIallCivilInst> allCivilInsts { get; set; }
    }
}
