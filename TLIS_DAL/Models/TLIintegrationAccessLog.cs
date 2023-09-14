using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using System.ComponentModel.DataAnnotations;
using KeyAttribute = System.ComponentModel.DataAnnotations.KeyAttribute;

namespace TLIS_DAL.Models
{
    public class TLIintegrationAccessLog
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string SystemName { get; set; }
        public string UserName { get; set; }
        public string IP { get; set; }
        public string ResponseMessage { get; set; }
        public DateTime ActionDate { get; set; }


        public TLIintegrationAccessLog() { }
        public TLIintegrationAccessLog(string SystemName,string UserName,string IP,string ResponseMessage)
        {
            this.SystemName = SystemName;
            this.UserName = UserName;
            this.ActionDate = DateTime.Now;
            this.IP = IP;
            this.ResponseMessage = ResponseMessage;
        }

    }
}
