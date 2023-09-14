using System;
using System.Collections.Generic;
using System.Text;
using TLIS_DAL.ViewModels.DataTypeDTOs;
using TLIS_DAL.ViewModels.DynamicAttInstValueDTOs;
using TLIS_DAL.ViewModels.DynamicAttLibValueDTOs;

namespace TLIS_DAL.ViewModels.CabinetDTOs
{
    public class DynamicAttDto
    {
        public string Key { get; set; }
        public DataTypeViewModel DataType { get; set; }
        public DynamicAttInstValueViewModel DynamicAttInstValue { get; set; }
        public DynamicAttLibValueViewMdodel DynamicAttLibValue { get; set; }
    }
}
