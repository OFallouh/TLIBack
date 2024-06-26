using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TLIS_API.Helpers
{

    public class Constants
    {
        public const int RowNum = 10;

       
        public enum ApiReturnCode
        {
            success = 0,  
            fail = 1,
            uncompleted = 2,
            NeedUpdate = 3,
            Invalid = 4
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
            TLIradioRRU,
            TLIradioAntenna,
            TLImwBU,
            TLImwDish,
            TLImwODU,
            TLImwRFU,
            TLImwOther,
            TLIradioOther,
            TLIloadOther,
            TLIpower
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
            TLIactor,
            TLIarea,
            TLIasType,
            TLIattActivatedCategory,
            TLIattributeActivated,
            TLIbaseCivilWithLegsType,
            TLIbaseGeneratorType,
            TLIboardType,
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
            TLIInstCivilwithoutLegsType,
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
            TLIsideArm
        }

        public enum ConfigrationTables
        {
            TLIdiversityType,
            TLItelecomType,
            TLIsupportTypeDesigned,
            TLIsupportTypeImplemented,
            TLIstructureType,
            TLIsectionsLegType,
            TLIlogisticalType,
            TLIbaseCivilWithLegsType,
            TLIbaseGeneratorType,
            TLIInstCivilwithoutLegsType,
            TLIattActivatedCategory,
            TLIboardType,
            TLIguyLineType,
            TLIpolarityOnLocation,
            TLIitemConnectTo,
            TLIrepeaterType,
            TLIoduInstallationType,
            TLIsideArmInstallationPlace,
            
        }

    }

    

    public class Messages : Constants
    { 
        private Messages(string value) { Value = value; }
        public string Value { get; set; }
        public static Messages Trace { get { return new Messages("Trace"); } }
        public static Messages Debug { get { return new Messages("Debug"); } }
        public static Messages Info { get { return new Messages("Info"); } }
        public static Messages Warning { get { return new Messages("Warning"); } }
        public static Messages Error { get { return new Messages("Error"); } }
        public static Messages Success { get { return new Messages("Success!"); } }
    }


}
