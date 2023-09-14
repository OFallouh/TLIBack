using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace TLIS_DAL.Models
{
    public class TLIstepActionItemStatus
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        //*
        [ForeignKey("TLIstepActionItemOption")]
        public int StepActionItemOptionId { get; set; }
        public TLIstepActionItemOption StepActionItemOption { get; set; }
        //*/
        [ForeignKey("TLIitemStatus")]
        public int IncomingItemStatusId { get; set; }
        public TLIitemStatus IncomingItemStatus { get; set; }
        [ForeignKey("TLIitemStatus")]
        public int OutgoingItemStatusId { get; set; }
        public TLIitemStatus OutgoingItemStatus { get; set; }
    }

}
