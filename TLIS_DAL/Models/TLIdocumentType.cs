using System;
using System.Collections.Generic;
using System.Text;

namespace TLIS_DAL.Models
{
    public class TLIdocumentType
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public IEnumerable<TLIattachedFiles> attachedFiles { get; set; }
    }
}
