using Oracle.ManagedDataAccess.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TLIS_DAL.Models
{
    public class AUDIT_TABLE
    {
        public int Id { get; set; }
        public string TABLE_NAME { get; set; }
        public string OPERATION_TYPE { get; set; }
        public int RECORD_ID { get; set; }
        public DateTime OPERATION_DATE { get; set; }
        public int  USER_ID { get; set; }
        public OracleClob OLD_VALUE { get; set; }
        public OracleClob NEW_VALUE { get; set; }
        public string PREVIOUS_HISTORY_ID { get; set; }
    }
}
