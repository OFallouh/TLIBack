using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace TLIS_DAL.Models
{
    public class TLIallCivilInst
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public TLIcivilWithLegs civilWithLegs { get; set; }
        public int? civilWithLegsId { get; set; }
        public TLIcivilWithoutLeg civilWithoutLeg { get; set; }
        public int? civilWithoutLegId { get; set; }
        public TLIcivilNonSteel civilNonSteel { get; set; }
        public int? civilNonSteelId { get; set; }
        public IEnumerable<TLIcivilSiteDate> civilSiteDates { get; set; }
        public IEnumerable<TLIcivilSupportDistance> civilSupportDistances { get; set; }
        public IEnumerable<TLIcivilLoads> civilLoads { get; set; }
        public int? ItemStatusId { get; set; }
        public TLIitemStatus ItemStatus { get; set; }
        public int? TicketId { get; set; }
        public TLIticket Ticket { get; set; }
        public bool Draft { get; set; }
    }
}
