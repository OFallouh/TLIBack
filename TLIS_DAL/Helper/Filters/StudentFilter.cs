using System;
using System.Collections.Generic;
using System.Text;

namespace TLIS_DAL.Helper.Filters
{
    public class StudentFilter
    {
        public string Name { get; set; }
        public Guid? ClassId { get; set; }
    }
}
