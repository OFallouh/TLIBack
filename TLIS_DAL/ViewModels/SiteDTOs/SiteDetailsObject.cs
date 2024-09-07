using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static TLIS_DAL.ViewModels.SiteDTOs.SiteDetailsObject;

namespace TLIS_DAL.ViewModels.SiteDTOs
{
    public static class Enums
    {
        public enum ValidationStatus
        {
            [Description("Pending O&M")]
            PendingOM = 1,

            [Description("Done")]
            Done = 2
        }

        public enum MDType
        {
            [Description("Change")]
            Change = 1,

            [Description("Dismantle")]
            Dismantle = 2,

            [Description("Install")]
            Install = 3
        }

        public enum CollectionDataPlanStatus
        {
            [Description("Open")]
            Open = 1,

            [Description("Done Civil")]
            DoneCivil = 2,

            [Description("Done O&M")]
            DoneOM = 3,

            [Description("Done Validation")]
            DoneValidation = 4,

            [Description("Close")]
            Close = 5
        }

        public enum CollectionDataPendingType
        {
            [Description("Pending Civil")]
            PendingCivil = 1,

            [Description("Pending O&M")]
            PendingOM = 2,

            [Description("Pending Validation")]
            PendingValidation = 3
        }

        public enum RadioValidationStatus
        {
            [Description("Pending Civil")]
            PendingCivil = 1,

            [Description("Pending O&M")]
            PendingOM = 2,

            [Description("Done")]
            Done = 3
        }

        public enum MWMDPlanStatus
        {
            [Description("Open")]
            Open = 1,

            [Description("Done Implementation")]
            DoneImplementation = 2,

            [Description("Done Validation")]
            DoneValidation = 3,

            [Description("Close")]
            Close = 4
        }

        public enum MWMDPendingType
        {
            [Description("Pending Implementation")]
            PendingImplementation = 1,

            [Description("Pending Validation")]
            PendingValidation = 2
        }

        public enum MWValidationStatus
        {
            [Description("Pending Implementation")]
            PendingImplementation = 1,

            [Description("Done")]
            Done = 2
        }

        public enum OtherMDPlanStatus
        {
            [Description("Open")]
            Open = 1,

            [Description("Done O&M")]
            DoneOM = 2,

            [Description("Done Validation")]
            DoneValidation = 3,

            [Description("Close")]
            Close = 4
        }

        public enum OtherMDPendingType
        {
            [Description("Pending O&M")]
            PendingOM = 1,

            [Description("Pending Validation")]
            PendingValidation = 2
        }

        public enum OtherValidationStatus
        {
            [Description("Pending O&M")]
            PendingOM = 1,

            [Description("Done")]
            Done = 2
        }

        public enum PlanType
        {
            [Description("Collect Data")]
            CollectData = 1,

            [Description("MW MD")]
            MWMD = 2,

            [Description("Power MD")]
            PowerMD = 3,

            [Description("Radio MD")]
            RadioMD = 4
        }
    }
    public class SiteDetailsObject
    {
        public string SiteCode { get; set; }
        public List<Enums.PlanType> PlanType { get; set; }
        public CollectData CollectData { get; set; }
        public MWMd MWMd { get; set; }
        public RadioMd RadioMd { get; set; }
        public PowerMd PowerMd { get; set; }
        
    }
    public class CollectData
    {
        public Enums.CollectionDataPlanStatus? PlanStatus { get; set; }
        public Enums.CollectionDataPendingType? PendingType { get; set; }
        public Enums.ValidationStatus? MwValidationStatus { get; set; }
        public string MwValidationRemark { get; set; }
        public Enums.RadioValidationStatus? RadioValidationStatus { get; set; }
        public string RadioValidationRemark { get; set; }
        public Enums.ValidationStatus? PowerValidationStatus { get; set; }
        public string PowerValidationRemark { get; set; }
    }

    public class MWMd
    {
        public Enums.MDType? MdType { get; set; }
        public string Description { get; set; }
        public Enums.MWMDPlanStatus? PlanStatus { get; set; }
        public Enums.MWMDPendingType? PendingType { get; set; }
        public Enums.MWValidationStatus? MwValidationStatus { get; set; }
        public string MwValidationRemark { get; set; }
    }

    public class RadioMd
    {
        public Enums.MDType? MdType { get; set; }
        public string Description { get; set; }
        public Enums.OtherMDPlanStatus? PlanStatus { get; set; }
        public Enums.OtherMDPendingType? PendingType { get; set; }
        public Enums.OtherValidationStatus? RadioValidationStatus { get; set; }
        public string RadioValidationRemark { get; set; }
    }

    public class PowerMd
    {
        public Enums.MDType? MdType { get; set; }
        public string Description { get; set; }
        public Enums.OtherMDPlanStatus? PlanStatus { get; set; }
        public Enums.OtherMDPendingType? PendingType { get; set; }
        public Enums.OtherValidationStatus? PowerValidationStatus { get; set; }
        public string PowerValidationRemark { get; set; }
    }
}
    

