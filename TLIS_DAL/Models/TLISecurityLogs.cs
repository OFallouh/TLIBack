using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TLIS_DAL.Models
{
    public class TLISecurityLogs
    {
 
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public DateTime Date { get; set; }

        public int UserId { get; set; }
        public TLIuser User { get; set; }

        public int TypeofUser { get; set; }
        public string ControllerName { get; set; }
        public string FunctionName { get; set; }

        [Column(TypeName = "CLOB")]
        public string BodyParameters { get; set; }

        [Column(TypeName = "CLOB")]
        public string HeaderParameters { get; set; }
        public string ResponseStatus { get; set; }

        [Column(TypeName = "CLOB")]
        public string Result { get; set; }
    }
}
