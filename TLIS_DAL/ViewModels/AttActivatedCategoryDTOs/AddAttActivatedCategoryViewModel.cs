using System;
using System.Collections.Generic;
using System.Text;

namespace TLIS_DAL.ViewModels.AttActivatedCategoryDTOs
{
    public class AddAttActivatedCategoryViewModel
    {
        public int? attributeActivatedId { get; set; }
        public int? civilWithoutLegCategoryId { get; set; }
        public bool enable { get; set; }
        public bool Required { get; set; }
        public string Label { get; set; }
        public string Description { get; set; }
        public bool IsLibrary { get; set; }

    }
}
