using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace TLIS_DAL.Models
{
    public class TLIequipment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string Table { get; set; }
        public int TableId { get; set; }
        [ForeignKey("TLIpart")]
        public int PartId { get; set; }
        public TLIpart Part { get; set; }
        public List<TLIticketEquipment> TicketEquipment { get; set; }
        
    }
}
