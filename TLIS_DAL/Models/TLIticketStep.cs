using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace TLIS_DAL.Models
{
    public enum stepStatus
    {
        /// <summary>
        /// remove it from current location
        /// </summary>
        UnderProcess,
        /// <summary>
        /// put it on this location
        /// </summary>
        Complete
    }
    public class TLIticketStep
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public TLIticket Ticket { get; set; }
        public TLIstep Step { get; set; }
        public stepStatus Status { get; set; }
    }
}
