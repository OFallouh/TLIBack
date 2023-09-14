using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace TLIS_DAL.Models
{
    public class TLIitemStatus
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public bool Active { get; set; }
        public bool Deleted { get; set; }
        public bool DeleteDate { get; set; }
        public List<TLIstepActionIncomeItemStatus> StepActionsIncomeItemStatus { get; set; }
        public List<TLIstepActionItemOption> StepActionsItemOptions { get; set; }
        public List<TLIstepActionOption> StepActionsOptions { get; set; }
        public List<TLIticketEquipmentAction> TicketEquipmentActions { get; set; }
    }
}
