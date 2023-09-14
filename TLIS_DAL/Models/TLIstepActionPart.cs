using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace TLIS_DAL.Models
{
    public class TLIstepActionPart
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [ForeignKey("TLIstepAction")]
        public int StepActionId { get; set; }
        public TLIstepAction StepAction { get; set; }
        [ForeignKey("TLIpart")]
        public int PartId { get; set; }
        public TLIpart Part { get; set; }
        public List<TLIstepActionPartGroup> StepActionPartGroup { get; set; }
        public bool Active { get; set; }
        public bool AllowUploadFile { get; set; }
        public bool UploadFileIsMandatory { get; set; }
        public bool Deleted { get; set; }
        public DateTime? DateDeleted { get; set; }
    }
}
