using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TLIS_DAL.Models
{
    public class TLIimportSheet
    {
        [Key]
        public int Id { get;set;}
        public string UniqueName { get;set;}
        public string SheetName { get; set; }
        public string RefTable { get; set; }
        public bool IsLib { get; set; }
        public string ErrMsg { get; set; }  
        public DateTime CreatedAt { get; set; }
        public bool IsDeleted {get; set; }
    }
}
