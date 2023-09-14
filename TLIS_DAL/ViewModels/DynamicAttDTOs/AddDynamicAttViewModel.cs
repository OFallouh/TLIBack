using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using TLIS_DAL.ViewModels.DynamicListValuesDTOs;

namespace TLIS_DAL.ViewModels.DynamicAttDTOs
{
    public class AddDynamicAttViewModel
    {
        public string Key { get; set; }
        public bool LibraryAtt { get; set; }
        public int? DataTypeId { get; set; }
        public string Description { get; set; }
        public int? CivilWithoutLegCategoryId { get; set; }
        public int tablesNamesId { get; set; }
        public bool Required { get; set; }
        public bool disable { get; set; }
        public List<DynamicListValuesViewModel> dynamicListValues { get; set; }
    }
}
