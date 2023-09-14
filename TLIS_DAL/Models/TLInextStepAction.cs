using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace TLIS_DAL.Models
{
    public class TLInextStepAction
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [ForeignKey("TLIstepActionItemOption")]
        public int? StepActionItemOptionId { get; set; }
        public TLIstepActionItemOption StepActionItemOption { get; set; }
        [ForeignKey("TLIstepActionOption")]
        public int? StepActionOptionId { get; set; }
        public TLIstepActionOption StepActionOption { get; set; }
        [ForeignKey("TLIstepAction")]
        public int? StepActionId { get; set; }
        public TLIstepAction StepAction { get; set; }
        //[ForeignKey("TLIstepAction")]
        public int NextStepActionId { get; set; }
        //public TLIstepAction NextStepAction { get; set; }
    }
}
