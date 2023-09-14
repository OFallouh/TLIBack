using System;
using System.Collections.Generic;
using System.Text;

namespace TLIS_DAL.ViewModelBase
{
    public class DropDownListFilters
    {
        public DropDownListFilters() { }
        public DropDownListFilters(int id, string value)
        {
            Id = id;
            Value = value;
        }
        public int Id { get; set; }
        public string Value { get; set; }

        public bool Deleted { get; set; }

        public bool Disable { get; set; }
    }
}
