using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace TLIS_DAL.Models
{
    public class TLIallLoadInst
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public bool Active { get; set; }
        public TLImwBU mwBU { get; set; }
        public int? mwBUId { get; set; }
        public TLImwDish mwDish { get; set; }
        public int? mwDishId { get; set; }
        public TLImwODU mwODU { get; set; }
        public int? mwODUId { get; set; }
        public TLImwRFU mwRFU { get; set; }
        public int? mwRFUId { get; set; }
        public TLImwOther mwOther { get; set; }
        public int? mwOtherId { get; set; }
        public TLIradioAntenna radioAntenna { get; set; }
        public int? radioAntennaId { get; set; }
        public TLIRadioRRU radioRRU { get; set; }
        public int? radioRRUId { get; set; }
        public TLIradioOther radioOther { get; set; }
        public int? radioOtherId { get; set; }
        public TLIpower power { get; set; }
        public int? powerId { get; set; }
        public TLIloadOther loadOther { get; set; }
        public int? loadOtherId { get; set; }
        public IEnumerable<TLIcivilLoads> civilLoads { get; set; }
        public int? ItemStatusId { get; set; }
        public TLIitemStatus ItemStatus { get; set; }
        public int? TicketId { get; set; }
        public TLIticket Ticket { get; set; }
        public bool Draft { get; set; }
    }
}
