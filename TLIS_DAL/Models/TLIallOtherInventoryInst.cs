using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace TLIS_DAL.Models
{
    public class TLIallOtherInventoryInst
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public TLIcabinet cabinet { get; set; }
        public int? cabinetId { get; set; }
        public TLIsolar solar { get; set; }
        public int? solarId { get; set; }
        public TLIgenerator generator { get; set; }
        public int? generatorId { get; set; }
        public IEnumerable<TLIotherInSite> otherInSites { get; set; }
        public IEnumerable<TLIotherInventoryDistance> otherInventoryDistances { get; set; }
        public int? ItemStatusId { get; set; }
        public TLIitemStatus ItemStatus { get; set; }
        public int? TicketId { get; set; }
        public TLIticket Ticket { get; set; }
        public bool Draft { get; set; }
    }
}
