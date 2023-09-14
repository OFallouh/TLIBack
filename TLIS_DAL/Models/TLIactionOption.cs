using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace TLIS_DAL.Models
{

    /// <summary>
    /// action option could be belongto an step action or to parent actionOption
    /// </summary>
    public class TLIactionOption
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [ForeignKey("TLIaction")]
        public int? ActionId { get; set; }
        public TLIaction Action { get; set; }
        [ForeignKey("TLIactionOption")]
        public int? ParentId { get; set; }
        public TLIactionOption Parent { get; set; }
        //public TLIstepAction NextStepAction { get; set; }
        public string Name { get; set; }
        //public bool AllowNote { get; set; }
        //public bool NoteIsMandatory { get; set; }
        public bool Deleted { get; set; }
        public DateTime? DateDeleted { get; set; }
        [ForeignKey("TLIitemStatus")]
        public int? ItemStatusId { get; set; }
        public TLIitemStatus ItemStatus { get; set; }

    }
}
