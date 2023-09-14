using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TLIS_DAL.Models
{
    public class TLIattributeViewManagment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public bool Enable { get; set; }

        [Required]
        [ForeignKey("TLIeditableManagmentView")]
        public int EditableManagmentViewId { get; set; }
        public TLIeditableManagmentView EditableManagmentView { get; set; }


        [ForeignKey("TLIattributeActivated")]
        public int? AttributeActivatedId { get; set; }
        public TLIattributeActivated AttributeActivated { get; set; }

        [ForeignKey("TLIdynamicAtt")]
        public int? DynamicAttId { get; set; }
        public TLIdynamicAtt DynamicAtt { get; set; }


    }
}
