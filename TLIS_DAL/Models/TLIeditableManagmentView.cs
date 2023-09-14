using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace TLIS_DAL.Models
{
    public class TLIeditableManagmentView
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string View { get; set; }

        [Required]
        [ForeignKey("TLItablesNames")]
        public int TLItablesNames1Id { get; set; }
        public TLItablesNames TLItablesNames1 { get; set; }

        [ForeignKey("TLItablesNames")]
        public int? TLItablesNames2Id { get; set; }
        public TLItablesNames TLItablesNames2 { get; set; }

        [ForeignKey("TLItablesNames")]
        public int? TLItablesNames3Id { get; set; }
        public TLItablesNames TLItablesNames3 { get; set; }

        [ForeignKey("TLIcivilWithoutLegCategory")]
        public int? CivilWithoutLegCategoryId { get; set; }
        public TLIcivilWithoutLegCategory CivilWithoutLegCategory { get; set; }

        public string Description { get; set; }

        public IEnumerable<TLIattributeViewManagment> Attributes { get; set; }
    }
}
