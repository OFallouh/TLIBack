using System;
using System.Collections.Generic;
using System.Text;
using TLIS_DAL.Models;

namespace TLIS_DAL.ViewModelBase
{
    public class EditHistoryDetails
    {
        public object original { get; set; }
        public IList<TLIhistoryDetails> Details;
    }
}
