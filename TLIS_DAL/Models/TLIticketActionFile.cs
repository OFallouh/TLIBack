using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace TLIS_DAL.Models
{
    public class TLIticketActionFile
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [ForeignKey("TLIticketAction")]
        public int TicketActionId { get; set; }
        public TLIticketAction TicketAction { get; set; }
        public string filename { get; set; }
        public string mimetype { get; set; }
        /// <summary>
        /// this column has to be blob type or something like that
        /// </summary>
        public byte[] filebody { get; set; }
    }
}
