using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace TLIS_DAL.Models
{
    public class TLIsideArm
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Notes { get; set; }
        public float? HeightBase { get; set; }
        public float? Azimuth{ get; set; }
        public float? ReservedSpace { get; set; }
        public bool Active { get; set; }
        public string VisibleStatus { get; set; }
        public float SpaceInstallation { get; set; }
        public TLIsideArmLibrary sideArmLibrary { get; set; }
        public int sideArmLibraryId { get; set; }
        public TLIsideArmInstallationPlace sideArmInstallationPlace { get; set; }
        public int? sideArmInstallationPlaceId { get; set; }
        public TLIowner owner { get; set; }
        public int? ownerId { get; set; }
        public TLIsideArmType sideArmType { get; set; }
        public int sideArmTypeId { get; set; }
        public IEnumerable<TLIcivilLoads> civilLoads { get; set; }
        public IEnumerable<TLIdynamicAttInstValue> dynamicAttInstValues { get; set; }
        public int? ItemStatusId { get; set; }
        public TLIitemStatus ItemStatus { get; set; }
        public int? TicketId { get; set; }
        public TLIticket Ticket { get; set; }
        public bool Draft { get; set; }
        public float CenterHigh { get; set; }
        public float HBA { get; set; }
        public float HieghFromLand { get; set; }
        public float EquivalentSpace { get; set; }
    }
}
