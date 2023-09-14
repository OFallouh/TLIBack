using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace TLIS_DAL.Models
{

    public class TLIagenda
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public DateTime CreationDate
        {
            get
            {
                return this.dateCreated.HasValue
                   ? this.dateCreated.Value
                   : DateTime.Now;
            }

            set { this.dateCreated = value; }
        }
        private DateTime? dateCreated;
        [ForeignKey("TLIticketAction")]
        public int TicketActionId { get; set; }
        public TLIticketAction TicketAction { get; set; }
        public int? period { get; set; }
        [ForeignKey("TLIuser")]
        public int? ExecuterId { get; set; }
        public TLIuser Executer { get; set; }
        public DateTime ExecutionDate { get; set; }
        public List<TLIagendaGroup> AgendaGroups { get; set; }
    }
}
