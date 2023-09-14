using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Toolbelt.ComponentModel.DataAnnotations.Schema;

namespace TLIS_DAL.Models
{
    public class TLIattActivatedCategory
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public TLIattributeActivated attributeActivated { get; set; }
        [Index("attributeActivatedIdAndcivilWithoutLegCategoryIdAndSameInstallationOrLibrarys", 1, IsUnique = true)]
        public int? attributeActivatedId { get; set; }
        public TLIcivilWithoutLegCategory civilWithoutLegCategory { get; set; }
        [Index("attributeActivatedIdAndcivilWithoutLegCategoryIdAndSameInstallationOrLibrary", 2, IsUnique = true)]
        public int? civilWithoutLegCategoryId { get; set; }
        public bool enable { get; set; }
        public bool Required { get; set; }
        public string Label { get; set; }
        public string Description { get; set; }
        [Index("attributeActivatedIdAndcivilWithoutLegCategoryIdAndSameInstallationOrLibrary", 3, IsUnique = true)]
        public bool IsLibrary { get; set; }
    }
}
