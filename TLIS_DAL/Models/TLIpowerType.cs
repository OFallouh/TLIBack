using System;
using System.Collections.Generic;
using System.Text;

namespace TLIS_DAL.Models
{
    public class TLIpowerType
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool Disable { get; set; }
        public bool Delete { get; set; }
        public IEnumerable<TLIpower> power { get; set; }
    }
}
