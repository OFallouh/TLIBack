using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLIS_DAL.ViewModelBase;
using TLIS_DAL.ViewModels.DynamicAttDTOs;

namespace TLIS_DAL.ViewModels.CivilWithLegsDTOs
{
    public class GetForAddCivilWithLegObject
    {
        public IEnumerable<BaseAttViews> LibraryAttribute { get; set; } = new List<BaseAttViews>();

        public IEnumerable<BaseInstAttViews> InstallationAttributes { get; set; } = new List<BaseInstAttViews>();

        public IEnumerable<BaseInstAttViews> CivilSiteDate { get; set; } = new List<BaseInstAttViews>();

        public IEnumerable<BaseInstAttViews> CivilSupportDistance { get; set; } = new List<BaseInstAttViews>();
        public IEnumerable<BaseInstAttViews> LegsInfo { get; set; } = new List<BaseInstAttViews>();
        public IEnumerable<BaseInstAttViewDynamic> DynamicAttribute { get; set; } = new List<BaseInstAttViewDynamic>();
 
    }
    public class BaseAttViews
    {
        public string Key { get; set; }
        public object Value { get; set; }
        public string Desc { get; set; }
        public string Label { get; set; }
        public bool Manage { get; set; }
        public bool Required { get; set; }
        public bool enable { get; set; }
        public bool AutoFill { get; set; }
        public string DataType { get; set; }
        public object Options { get; set; } 
    }
    public class BaseInstAttViews
    {
        public string Key { get; set; }
        public object Value { get; set; }
        public string Desc { get; set; }
        public string Label { get; set; }
        public bool Manage { get; set; }
        public bool Required { get; set; }
        public bool enable { get; set; }
        public bool AutoFill { get; set; }
        public int? DataTypeId { get; set; }
        public string DataType { get; set; }
        public object Options { get; set; }
    }
    public class BaseInstAttViewDynamic
    {
        public int Id { get; set; }
        public string Key { get; set; }
        public object Value { get; set; }
        public string Desc { get; set; }
        public string Label { get; set; }
        public bool Manage { get; set; }
        public bool Required { get; set; }
        public bool enable { get; set; }
        public bool AutoFill { get; set; }
        public int? DataTypeId { get; set; }
        public string DataType { get; set; }
        public object Options { get; set; }
    }
}
