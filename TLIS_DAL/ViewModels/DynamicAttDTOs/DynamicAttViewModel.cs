using System;
using System.Collections.Generic;
using System.Text;

namespace TLIS_DAL.ViewModels.DynamicAttDTOs
{
    public class DynamicAttViewModel
    {
        public int Id { get; set; }
        public string Key { get; set; }
        public bool LibraryAtt { get; set; }
        public int? DataTypeId { get; set; }
        public string DataType_Name { get; set; }
        public string Description { get; set; }
        public int? CivilWithoutLegCategoryId { get; set; }
        public string CivilWithoutLegCategory_Name { get; set; }
        public int tablesNamesId { get; set; }
        public string tablesNames_Name { get; set; }
        public bool Required { get; set; }
        public bool disable { get; set; }
    }
}
