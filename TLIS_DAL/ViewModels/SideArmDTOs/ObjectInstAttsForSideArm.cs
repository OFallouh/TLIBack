using System;
using System.Collections.Generic;
using System.Text;
using TLIS_DAL.ViewModelBase;
using TLIS_DAL.ViewModels.DynamicAttDTOs;

namespace TLIS_DAL.ViewModels.SideArmDTOs
{
    public class ObjectInstAttsForSideArm
    {
        public IEnumerable<BaseAttView> LibraryActivatedAttributes { get; set; } = new List<BaseAttView>();

        public IEnumerable<BaseInstAttView> AttributesActivated { get; set; } = new List<BaseInstAttView>();

        public IEnumerable<BaseInstAttView> CivilSiteDate { get; set; } = new List<BaseInstAttView>();

        public IEnumerable<BaseInstAttView> CivilSupportDistance { get; set; } = new List<BaseInstAttView>();

        public IEnumerable<BaseInstAttView> CivilLoads { get; set; } = new List<BaseInstAttView>();

        public IEnumerable<BaseInstAttView> OtherInSite { get; set; } = new List<BaseInstAttView>();

        public IEnumerable<BaseInstAttView> OtherInventoryDistance { get; set; } = new List<BaseInstAttView>();

        public IEnumerable<DynaminAttInstViewModel> DynamicAtts { get; set; } = new List<DynaminAttInstViewModel>();

        public IEnumerable<KeyValuePair<string, List<DropDownListFilters>>> RelatedTables { get; set; } = new List<KeyValuePair<string, List<DropDownListFilters>>>();
        public IEnumerable<BaseInstAttView> SideArmInstallationInfo { get; set; } = new List<BaseInstAttView>();
    }
}
