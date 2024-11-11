using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace TLIS_DAL.Models
{
    //public enum documenttype
    //{
    //    pdf,
    //    Word,
    //    Excel,
    //    Image,
    //    As_Built_file,
    //    Other
    //}
    public class TLIattachedFiles
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; }
        public string SiteCode { get; set; }
        public string Description { get; set; }
        public string Description2 { get; set; }
        public float fileSize { get; set; }
        public string Path { get; set; }
        public int? RecordId { get; set; }
        public virtual TLIdocumentType documenttype { get; set; }
        [ForeignKey("documenttype")]
        public int? documenttypeId { get; set; }
        public virtual TLItablesNames tablesName { get; set; }
        [ForeignKey("tablesName")]
        public int? tablesNamesId { get; set; }
        public bool IsImg { get; set; }
        public bool UnAttached { get; set; }
    }
}
