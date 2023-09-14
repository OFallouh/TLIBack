using System;
using System.Collections.Generic;
using System.Text;

namespace TLIS_DAL.ViewModels.DynamicAttDTOs
{
    class EditDynamicAttLibViewModel
    {
        public int Id { get; set; }
        public string Key { get; set; }
        public bool LibraryAtt { get; set; }
        public int? DataTypeId { get; set; }
        public string Value { get; set; }
    }
}
