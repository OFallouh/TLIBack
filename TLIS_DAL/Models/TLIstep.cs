using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace TLIS_DAL.Models
{

    /// <summary>
    /// sequence, parallel_one, parallel_and, parallel_or
    /// </summary>
    public enum StepType
    {
        /// <summary>
        /// all items will move together through all actions
        /// </summary>
        sequence,
        /// <summary>
        /// first one reach no need to look at the rest of items
        /// </summary>
        parallel_one,
        /// <summary>
        /// parallel but all items must reach to the end to move to the next step
        /// </summary>
        parallel_and,
        /// <summary>
        /// parallel but items will continue to the next step without waiting for the rest items
        /// </summary>
        parallel_or
    }



    /// <summary>
    /// each step could be belong to a workflow or to a parent step
    /// </summary>
    public class TLIstep
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Sequence { get; set; }
        [ForeignKey("TLIworkFlow")]
        public int? WorkFlowId { get; set; }
        public TLIworkFlow WorkFlow { get; set; }
        [ForeignKey("TLIstep")]
        public int? ParentStepId { get; set; }
        public TLIstep ParentStep { get; set; }
        public StepType type { get; set; }
        [NotMapped]
        public List<TLIstepAction> StepActions { get; set; }
        public bool Deleted { get; set; }
        public DateTime? DateDeleted { get; set; }
        public List<TLIticketStep> TicketSteps { get; set; }

    }
}
