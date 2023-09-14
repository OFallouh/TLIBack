using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Toolbelt.ComponentModel.DataAnnotations.Schema;

namespace TLIS_DAL.Models
{
    public class TLIcivilWithoutLegCategory
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        [Index(IsUnique = true)]
        public string Name { get; set; }
        public bool disable { get; set; }
        public IEnumerable<TLIcivilWithoutLegLibrary> civilWithoutLegLibraries { get; set; }
        public IEnumerable<TLIattActivatedCategory> activatedCategories { get; set; }
        public IEnumerable<TLIdynamicAtt> dynamicAtts { get; set; }
        public IEnumerable<TLIeditableManagmentView> EditableManagmentViews { get; set; }
    }
}
