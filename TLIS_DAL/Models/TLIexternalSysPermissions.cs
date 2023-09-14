using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace TLIS_DAL.Models
{
    public class TLIexternalSysPermissions
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        [ForeignKey("TLIexternalSys")]
        public int ExtSysId { get; set; }
        [Required]
        [ForeignKey("TLIinternalApi")]
        public int InternalApiId { get; set; }

        public virtual TLIexternalSys TLIexternalSys { get; set; }
        public virtual TLIinternalApis TLIinternalApi { get; set; }

        public TLIexternalSysPermissions() { }

        public TLIexternalSysPermissions(int ext,int api)
        {
            ExtSysId= ext;
            InternalApiId= api;
        }


    }
}
