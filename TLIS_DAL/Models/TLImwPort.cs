using System;
using System.Collections.Generic;
using System.Text;

namespace TLIS_DAL.Models
{
    public class TLImwPort
    {
        public int Id { get; set; }
        public string Port_Name { get; set; }
        public string TX_Frequency { get; set; }
        public TLImwBULibrary MwBULibrary { get; set; }
        public int MwBULibraryId { get; set; }
        public TLImwBU MwBU { get; set; }
        public int MwBUId { get; set; }
        public int Port_Type { get; set; }
        public IEnumerable<TLImwRFU> mwRFUs { get; set; }

    }
}
