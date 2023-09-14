using System;
using System.Collections.Generic;
using System.Text;

namespace TLIS_DAL.Models
{
    public  class TLIenforcmentCategory
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool Disable { get; set; }
        public bool Deleted { get; set; }
        public IEnumerable<TLIcivilWithLegs> CivilWithLegs { get; set; }
    }
}
