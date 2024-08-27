using System;
using System.Collections.Generic;
using System.Text;

namespace TLIS_Repository.Helpers
{
    public class Constants
    {
        public const int RowNum = 10;
        public enum TablePartName
        {
            SideArm,
            LoadOther,
            CivilSupport,
            MW,
            Radio,
            Power,
            OtherInventory
        }
        public enum TLIlogisticalType
        {
            Vendor,
            Designer,
            Supplier,
            Manufacturer
        }
        public enum TLIhistoryType
        {
            Add,
            Edit,
            Delete
        }
        public enum ApiReturnCode
        {
            success = 0,
            fail = 1,
            uncompleted = 2,
            NeedUpdate = 3,
        }
        public enum GroupType
        {
            Domain = 1,
            NotDomain = 2
        }
        public enum UserType
        {
            Internal = 1,
            External = 2
        }
        public enum CivilType
        {
            TLIcivilWithLegLibrary,
            TLIcivilWithoutLegLibrary,
            TLIcivilNonSteelLibrary,
            TLIcivilWithLegs,
            TLIcivilWithoutLeg,
            TLIcivilNonSteel,
        }

        public enum LoadSubType
        {

            TLImwBULibrary,
            TLImwDishLibrary,
            TLImwODULibrary,
            TLImwRFULibrary,
            TLImwOtherLibrary,
            TLIradioRRULibrary,
            TLIradioAntennaLibrary,
            TLIradioOtherLibrary,
            TLIpowerLibrary,
            TLIloadOtherLibrary,
            TLIpower,
            TLIradioRRU,
            TLIradioAntenna,
            TLImwODU,
            TLImwDish,
            TLImwBU,
            TLImwRFU,
            TLImwOther,
            TLIradioOther,
            TLIloadOther
        }

        public enum OtherInventoryType
        {
            TLIsolarLibrary,
            TLIgeneratorLibrary,
            TLIcabinetPowerLibrary,
            TLIcabinetTelecomLibrary,
            TLIcabinet,
            TLIsolar,
            TLIgenerator
        }
        public enum TablesNames
        {
            TLIradioOtherLibrary,
            TLIactor,
            TLIarea,
            TLIasType,
            TLIattActivatedCategory,
            TLIattributeActivated,
            TLIbaseCivilWithLegsType,
            TLIbaseGeneratorType,
            TLIboardType,
            TLIloadOtherLibrary,
            TLIcabinet,
            TLIcabinetPowerLibrary,
            TLIcabinetPowerType,
            TLIcabinetTelecomLibrary,
            TLIcapacity,
            TLIcity,
            TLIcivilNonSteel,
            TLIcivilNonSteelLibrary,
            TLIcivilSiteDate,
            TLIcivilSteelSupportCategory,
            TLIcivilType,
            TLIcivilWithLegLibrary,
            TLImwOtherLibrary,
            TLIcivilWithLegs,
            TLIcivilWithoutLeg,
            TLIcivilWithoutLegCategory,
            TLIcivilWithoutLegLibrary,
            TLIcondition,
            TLIconditionType,
            TLIdataType,
            TLIdependency,
            TLIdependencyRow,
            TLIdiversityType,
            TLIdynamicAtt,
            TLIdynamicAttInstValue,
            TLIdynamicAttLibValue,
            TLIgenerator,
            TLIgeneratorLibrary,
            TLIgroup,
            TLIgroupRole,
            TLIgroupUser,
            TLIguyLineType,
            TLIhistoryDetails,
            TLIhistoryType,
            TLIinstallationCivilwithoutLegsType,
            TLIitemStatus,
            TLIleg,
            TLIloadPart,
            TLIloadSubType,
            TLIloadType,
            TLIlog,
            TLIlogicalOperation,
            TLIlogistical,
            TLIlogisticalitem,
            TLIlogisticalType,
            TLImwBULibrary,
            TLImwDishLibrary,
            TLImwODULibrary,
            TLImwRFULibrary,
            TLIoperation,
            TLIoption,
            TLIotherInSite,
            TLIotherInventoryDistance,
            TLIotherInventoryType,
            TLIowner,
            TLIparity,
            TLIpermission,
            TLIpolarityType,
            TLIpowerLibrary,
            TLIradioAntennaLibrary,
            TLIradioOther,
            TLIradioRRULibrary,
            TLIregion,
            TLIrenewableCabinetType,
            TLIrole,
            TLIrolePermission,
            TLIrow,
            TLIrowRule,
            TLIrule,
            TLIsectionsLegType,
            TLIsideArmLibrary,
            TLIsite,
            TLIsolar,
            TLIsolarLibrary,
            TLIstructureType,
            TLIsuboption,
            TLIsupportTypeDesigned,
            TLIsupportTypeImplemented,
            TLItablesHistory,
            TLItaskStatus,
            TLItelecomType,
            TLIuser,
            TLIuserPermission,
            TLIvalidation,
            TLIinstallationPlace,
            TLIradioAntenna,
            TLIradioRRU,
            TLIsideArm,
            TLIpower,
            TLIcivilSupportDistance,
            TLIcivilLoads,
            TLImwBU,
            TLImwOther,
            TLIloadOther,
            TLImwODU,
            TLImwDish,
            TLImwRFU,
            TLIcabinetTelecom,
            TLIcabinetPower,
        }
    }
}
