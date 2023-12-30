using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Text;

namespace TLIS_Service.Helpers
{
    public class Constants
    {
        public enum SideArmInstallationMissedAttributes
        {
            [Description("DateTime")]
            Created,

            [Description("String")]
            Created_By,

            [Description("DateTime")]
            Modified,

            [Description("String")]
            Modified_By,

            [Description("String")]
            Related_Plan,

            [Description("String")]
            user,

            [Description("String")]
            Current_Plan,

            [Description("String")]
            collect_plan,

            [Description("String")]
            Title,

            [Description("String")]
            Notes
        }
        public enum CivilNonSteelInstallationMissedAttributes
        {
            [Description("String")]
            Related_Plan,

            [Description("String")]
            Current_Status,

            [Description("DateTime")]
            Status_Date,

            [Description("DateTime")]
            Site_visit_date,

            [Description("String")]
            Created_By,

            [Description("DateTime")]
            Created,

            [Description("String")]
            Modified_By,

            [Description("DateTime")]
            Modified,

            [Description("String")]
            Title,

            [Description("String")]
            TTT,

            [Description("String")]
            te,

            [Description("Double")]
            Height,

            [Description("String")]
            getcollectplanstatue,

            [Description("String")]
            dddddd,

            [Description("String")]
            sinarule,

            [Description("String")]
            Add_Date,

            [Description("String")]
            Item_Type,

            [Description("String")]
            Path
        }
        public enum CivilWithLegsInstallationMissedAttributes
        {
            [Description("DateTime")]
            Site_visit_date,

            [Description("DateTime")]
            Created,

            [Description("String")]
            Created_By,

            [Description("DateTime")]
            Modified,

            [Description("String")]
            Modified_By,

            [Description("String")]
            Related_Plan,

            [Description("Double")]
            Building_Height_H3,

            [Description("String")]
            Title,

            [Description("String")]
            TTT,

            [Description("String")]
            te,

            [Description("Double")]
            Height,

            [Description("String")]
            getcollectplanstatue,

            [Description("String")]
            dddddd,

            [Description("String")]
            sinarule,

            [Description("String")]
            Add_Date,

            [Description("String")]
            Item_Type,

            [Description("String")]
            Path,
        }
        public enum CivilWithoutLegCategories
        {
            Mast,
            Capsule,
            Monopole
        }
        public enum SideArmTypes
        {
            Normal,
            Special
        }
        public enum SideArmInstallationPlace
        {
            Leg,
            Bracing,
            SupportPole,
            SupportFence,
            MastAboveSupport,
            WallSideArm
        }
        public enum PathToCheckDependencyValidation
        {
            #region CivilWithLeg
            [Description("TLIcivilWithLegs TLIsite SiteCode")]
            TLIcivilWithLegsTLIsite,

            [Description("SiteCode")]
            TLIcivilWithLegsTLIsiteGoal,

            [Description("TLIcivilWithLegs TLIcivilWithLegLibrary Id")]
            TLIcivilWithLegsTLIcivilWithLegLibrary,

            [Description("CivilWithLegsLibId")]
            TLIcivilWithLegsTLIcivilWithLegLibraryGoal,
            #endregion
            #region CivilWithoutLeg
            [Description("TLIcivilWithoutLeg TLIsite SiteCode")]
            TLIcivilWithoutLegTLIsite,

            [Description("SiteCode")]
            TLIcivilWithoutLegTLIsiteGoal,

            [Description("TLIcivilWithoutLeg TLIcivilWithoutLegLibrary Id")]
            TLIcivilWithoutLegTLIcivilWithoutLegLibrary,

            [Description("CivilWithoutlegsLibId")]
            TLIcivilWithoutLegTLIcivilWithoutLegLibraryGoal,
            #endregion
            #region CivilNonSteel
            [Description("TLIcivilNonSteel TLIsite SiteCode")]
            TLIcivilNonSteelTLIsite,

            [Description("SiteCode")]
            TLIcivilNonSteelTLIsiteGoal,

            [Description("TLIcivilNonSteel TLIcivilNonSteelLibrary Id")]
            TLIcivilNonSteelTLIcivilNonSteelLibrary,

            [Description("CivilNonSteelLibraryId")]
            TLIcivilNonSteelTLIcivilNonSteelLibraryGoal,
            #endregion

            #region SideArm
            [Description("TLIsideArm TLIsite SiteCode")]
            TLIsideArmTLIsite,

            [Description("SiteCode")]
            TLIsideArmTLIsiteGoal,

            [Description("TLIsideArm TLIcivilWithLegs Id" +
                " TLIallCivilInst civilWithLegsId Id")]
            TLIsideArmTLIcivilWithLegs,

            [Description("TLIcivilLoads allCivilInstId")]
            TLIsideArmTLIcivilWithLegsGoal,

            [Description("TLIsideArm TLIcivilWithLegLibrary Id" +
                " TLIcivilWithLegs CivilWithLegsLibId Id" +
                " TLIallCivilInst civilWithLegsId Id")]
            TLIsideArmTLIcivilWithLegLibrary,

            [Description("TLIcivilLoads allCivilInstId")]
            TLIsideArmTLIcivilWithLegLibraryGoal,

            [Description("TLIsideArm TLIcivilWithoutLeg Id" +
                " TLIallCivilInst civilWithoutLegId Id")]
            TLIsideArmTLIcivilWithoutLeg,

            [Description("TLIcivilLoads allCivilInstId")]
            TLIsideArmTLIcivilWithoutLegGoal,

            [Description("TLIsideArm TLIcivilWithoutLegLibrary Id" +
                " TLIcivilWithoutLeg CivilWithoutlegsLibId Id" +
                " TLIallCivilInst civilWithoutLegId Id")]
            TLIsideArmTLIcivilWithoutLegLibrary,

            [Description("TLIcivilLoads allCivilInstId")]
            TLIsideArmTLIcivilWithoutLegLibraryGoal,

            [Description("TLIsideArm TLIcivilNonSteel Id" +
                " TLIallCivilInst civilNonSteelId Id")]
            TLIsideArmTLIcivilNonSteel,

            [Description("TLIcivilLoads allCivilInstId")]
            TLIsideArmTLIcivilNonSteelGoal,

            [Description("TLIsideArm TLIcivilNonSteelLibrary Id" +
                " TLIcivilNonSteel CivilNonSteelLibraryId Id" +
                " TLIallCivilInst civilNonSteelId Id")]
            TLIsideArmTLIcivilNonSteelLibrary,

            [Description("TLIcivilLoads allCivilInstId")]
            TLIsideArmTLIcivilNonSteelLibraryGoal,

            [Description("TLIsideArm TLIsideArmLibrary Id")]
            TLIsideArmTLIsideArmLibrary,

            [Description("sideArmLibraryId")]
            TLIsideArmTLIsideArmLibraryGoal,
            #endregion

            #region Cabinet
            [Description("TLIcabinet TLIsite SiteCode")]
            TLIcabinetTLIsite,

            [Description("SiteCode")]
            TLIcabinetTLIsiteGoal,

            [Description("TLIcabinet TLIcabinetPowerLibrary Id")]
            TLIcabinetTLIcabinetPowerLibrary,

            [Description("CabinetPowerLibraryId")]
            TLIcabinetTLIcabinetPowerLibraryGoal,

            [Description("TLIcabinet TLIcabinetTelecomLibrary Id")]
            TLIcabinetTLIcabinetTelecomLibrary,

            [Description("CabinetTelecomLibraryId")]
            TLIcabinetTLIcabinetTelecomLibraryGoal,
            #endregion
            #region Solar
            [Description("TLIsolar TLIsite SiteCode")]
            TLIsolarTLIsite,

            [Description("SiteCode")]
            TLIsolarTLIsiteGoal,

            [Description("TLIsolar TLIsolarLibrary Id")]
            TLIsolarTLIsolarLibrary,

            [Description("SolarLibraryId")]
            TLIsolarTLIsolarLibraryGoal,
            #endregion
            #region Generator
            [Description("TLIgenerator TLIsite SiteCode")]
            TLIgeneratorTLIsite,

            [Description("SiteCode")]
            TLIgeneratorTLIsiteGoal,

            [Description("TLIgenerator TLIgeneratorLibrary Id")]
            TLIgeneratorTLIgeneratorLibrary,

            [Description("GeneratorLibraryId")]
            TLIgeneratorTLIgeneratorLibraryGoal,
            #endregion

            #region MW_Dish
            [Description("TLImwDish TLIsite SiteCode")]
            TLImwDishTLIsite,

            [Description("SiteCode")]
            TLImwDishTLIsiteGoal,

            [Description("TLImwDish TLIcivilWithLegs Id" +
                " TLIallCivilInst civilWithLegsId Id")]
            TLImwDishTLIcivilWithLegs,

            [Description("TLIcivilLoads allCivilInstId")]
            TLImwDishTLIcivilWithLegsGoal,

            [Description("TLImwDish TLIcivilWithLegLibrary Id" +
                " TLIcivilWithLegs CivilWithLegsLibId Id" +
                " TLIallCivilInst civilWithLegsId Id")]
            TLImwDishTLIcivilWithLegLibrary,

            [Description("TLIcivilLoads allCivilInstId")]
            TLImwDishTLIcivilWithLegLibraryGoal,

            [Description("TLImwDish TLIcivilWithoutLeg Id" +
                " TLIallCivilInst civilWithoutLegId Id")]
            TLImwDishTLIcivilWithoutLeg,

            [Description("TLIcivilLoads allCivilInstId")]
            TLImwDishTLIcivilWithoutLegGoal,

            [Description("TLImwDish TLIcivilWithoutLegLibrary Id" +
                " TLIcivilWithoutLeg CivilWithoutlegsLibId Id" +
                " TLIallCivilInst civilWithoutLegId Id")]
            TLImwDishTLIcivilWithoutLegLibrary,

            [Description("TLIcivilLoads allCivilInstId")]
            TLImwDishTLIcivilWithoutLegLibraryGoal,

            [Description("TLImwDish TLIcivilNonSteel Id" +
                " TLIallCivilInst civilNonSteelId Id")]
            TLImwDishTLIcivilNonSteel,

            [Description("TLIcivilLoads allCivilInstId")]
            TLImwDishTLIcivilNonSteelGoal,

            [Description("TLImwDish TLIcivilNonSteelLibrary Id" +
                " TLIcivilNonSteel CivilNonSteelLibraryId Id" +
                " TLIallCivilInst civilNonSteelId Id")]
            TLImwDishTLIcivilNonSteelLibrary,

            [Description("TLIcivilLoads allCivilInstId")]
            TLImwDishTLIcivilNonSteelLibraryGoal,

            [Description("TLImwDish TLIsideArm Id")]
            TLImwDishTLIsideArm,

            [Description("TLIcivilLoads sideArmId")]
            TLImwDishTLIsideArmGoal,

            [Description("TLImwDish TLIsideArmLibrary Id" +
                " TLIsideArm sideArmLibraryId Id")]
            TLImwDishTLIsideArmLibrary,

            [Description("TLIcivilLoads sideArmId")]
            TLImwDishTLIsideArmLibraryGoal,

            [Description("TLImwDish TLImwDishLibrary Id")]
            TLImwDishTLImwDishLibrary,

            [Description("MwDishLibraryId")]
            TLImwDishTLImwDishLibraryGoal,
            #endregion
            #region MW_BU
            [Description("TLImwBU TLIsite SiteCode")]
            TLImwBUTLIsite,

            [Description("SiteCode")]
            TLImwBUTLIsiteGoal,

            [Description("TLImwBU TLIcivilWithLegs Id" +
                " TLIallCivilInst civilWithLegsId Id")]
            TLImwBUTLIcivilWithLegs,

            [Description("TLIcivilLoads allCivilInstId")]
            TLImwBUTLIcivilWithLegsGoal,

            [Description("TLImwBU TLIcivilWithLegLibrary Id" +
                " TLIcivilWithLegs CivilWithLegsLibId Id" +
                " TLIallCivilInst civilWithLegsId Id")]
            TLImwBUTLIcivilWithLegLibrary,

            [Description("TLIcivilLoads allCivilInstId")]
            TLImwBUTLIcivilWithLegLibraryGoal,

            [Description("TLImwBU TLIcivilWithoutLeg Id" +
                " TLIallCivilInst civilWithoutLegId Id")]
            TLImwBUTLIcivilWithoutLeg,

            [Description("TLIcivilLoads allCivilInstId")]
            TLImwBUTLIcivilWithoutLegGoal,

            [Description("TLImwBU TLIcivilWithoutLegLibrary Id" +
                " TLIcivilWithoutLeg CivilWithoutlegsLibId Id" +
                " TLIallCivilInst civilWithoutLegId Id")]
            TLImwBUTLIcivilWithoutLegLibrary,

            [Description("TLIcivilLoads allCivilInstId")]
            TLImwBUTLIcivilWithoutLegLibraryGoal,

            [Description("TLImwBU TLIcivilNonSteel Id" +
                " TLIallCivilInst civilNonSteelId Id")]
            TLImwBUTLIcivilNonSteel,

            [Description("TLIcivilLoads allCivilInstId")]
            TLImwBUTLIcivilNonSteelGoal,

            [Description("TLImwBU TLIcivilNonSteelLibrary Id" +
                " TLIcivilNonSteel CivilNonSteelLibraryId Id" +
                " TLIallCivilInst civilNonSteelId Id")]
            TLImwBUTLIcivilNonSteelLibrary,

            [Description("TLIcivilLoads allCivilInstId")]
            TLImwBUTLIcivilNonSteelLibraryGoal,

            [Description("TLImwBU TLIsideArm Id")]
            TLImwBUTLIsideArm,

            [Description("TLIcivilLoads sideArmId")]
            TLImwBUTLIsideArmGoal,

            [Description("TLImwBU TLIsideArmLibrary Id" +
                " TLIsideArm sideArmLibraryId Id")]
            TLImwBUTLIsideArmLibrary,

            [Description("TLIcivilLoads sideArmId")]
            TLImwBUTLIsideArmLibraryGoal,

            [Description("TLImwBU TLImwBULibrary Id")]
            TLImwBUTLImwBULibrary,

            [Description("MwBULibraryId")]
            TLImwBUTLImwBULibraryGoal,
            #endregion
            #region MW_ODU
            [Description("TLImwODU TLIsite SiteCode")]
            TLImwODUTLIsite,

            [Description("SiteCode")]
            TLImwODUTLIsiteGoal,

            [Description("TLImwODU TLIcivilWithLegs Id" +
                " TLIallCivilInst civilWithLegsId Id")]
            TLImwODUTLIcivilWithLegs,

            [Description("TLIcivilLoads allCivilInstId")]
            TLImwODUTLIcivilWithLegsGoal,

            [Description("TLImwODU TLIcivilWithLegLibrary Id" +
                " TLIcivilWithLegs CivilWithLegsLibId Id" +
                " TLIallCivilInst civilWithLegsId Id")]
            TLImwODUTLIcivilWithLegLibrary,

            [Description("TLIcivilLoads allCivilInstId")]
            TLImwODUTLIcivilWithLegLibraryGoal,

            [Description("TLImwODU TLIcivilWithoutLeg Id" +
                " TLIallCivilInst civilWithoutLegId Id")]
            TLImwODUTLIcivilWithoutLeg,

            [Description("TLIcivilLoads allCivilInstId")]
            TLImwODUTLIcivilWithoutLegGoal,

            [Description("TLImwODU TLIcivilWithoutLegLibrary Id" +
                " TLIcivilWithoutLeg CivilWithoutlegsLibId Id" +
                " TLIallCivilInst civilWithoutLegId Id")]
            TLImwODUTLIcivilWithoutLegLibrary,

            [Description("TLIcivilLoads allCivilInstId")]
            TLImwODUTLIcivilWithoutLegLibraryGoal,

            [Description("TLImwODU TLIcivilNonSteel Id" +
                " TLIallCivilInst civilNonSteelId Id")]
            TLImwODUTLIcivilNonSteel,

            [Description("TLIcivilLoads allCivilInstId")]
            TLImwODUTLIcivilNonSteelGoal,

            [Description("TLImwODU TLIcivilNonSteelLibrary Id" +
                " TLIcivilNonSteel CivilNonSteelLibraryId Id" +
                " TLIallCivilInst civilNonSteelId Id")]
            TLImwODUTLIcivilNonSteelLibrary,

            [Description("TLIcivilLoads allCivilInstId")]
            TLImwODUTLIcivilNonSteelLibraryGoal,

            [Description("TLImwODU TLIsideArm Id")]
            TLImwODUTLIsideArm,

            [Description("TLIcivilLoads sideArmId")]
            TLImwODUTLIsideArmGoal,

            [Description("TLImwODU TLIsideArmLibrary Id" +
                " TLIsideArm sideArmLibraryId Id")]
            TLImwODUTLIsideArmLibrary,

            [Description("TLIcivilLoads sideArmId")]
            TLImwODUTLIsideArmLibraryGoal,

            [Description("TLImwODU TLImwODULibrary Id")]
            TLImwODUTLImwODULibrary,

            [Description("MwODULibraryId")]
            TLImwODUTLImwODULibraryGoal,
            #endregion
            #region MW_RFU
            [Description("TLImwRFU TLIsite SiteCode")]
            TLImwRFUTLIsite,

            [Description("SiteCode")]
            TLImwRFUTLIsiteGoal,

            [Description("TLImwRFU TLIcivilWithLegs Id" +
                " TLIallCivilInst civilWithLegsId Id")]
            TLImwRFUTLIcivilWithLegs,

            [Description("TLIcivilLoads allCivilInstId")]
            TLImwRFUTLIcivilWithLegsGoal,

            [Description("TLImwRFU TLIcivilWithLegLibrary Id" +
                " TLIcivilWithLegs CivilWithLegsLibId Id" +
                " TLIallCivilInst civilWithLegsId Id")]
            TLImwRFUTLIcivilWithLegLibrary,

            [Description("TLIcivilLoads allCivilInstId")]
            TLImwRFUTLIcivilWithLegLibraryGoal,

            [Description("TLImwRFU TLIcivilWithoutLeg Id" +
                " TLIallCivilInst civilWithoutLegId Id")]
            TLImwRFUTLIcivilWithoutLeg,

            [Description("TLIcivilLoads allCivilInstId")]
            TLImwRFUTLIcivilWithoutLegGoal,

            [Description("TLImwRFU TLIcivilWithoutLegLibrary Id" +
                " TLIcivilWithoutLeg CivilWithoutlegsLibId Id" +
               " TLIallCivilInst civilWithoutLegId Id")]
            TLImwRFUTLIcivilWithoutLegLibrary,

            [Description("TLIcivilLoads allCivilInstId")]
            TLImwRFUTLIcivilWithoutLegLibraryGoal,

            [Description("TLImwRFU TLIcivilNonSteel Id" +
                " TLIallCivilInst civilNonSteelId Id")]
            TLImwRFUTLIcivilNonSteel,

            [Description("TLIcivilLoads allCivilInstId")]
            TLImwRFUTLIcivilNonSteelGoal,

            [Description("TLImwRFU TLIcivilNonSteelLibrary Id" +
                " TLIcivilNonSteel CivilNonSteelLibraryId Id" +
                " TLIallCivilInst civilNonSteelId Id")]
            TLImwRFUTLIcivilNonSteelLibrary,

            [Description("TLIcivilLoads allCivilInstId")]
            TLImwRFUTLIcivilNonSteelLibraryGoal,

            //[Description("TLImwRFU TLIsideArm Id")]
            //TLImwRFUTLIsideArm,

            //[Description("TLIcivilLoads sideArmId")]
            //TLImwRFUTLIsideArmGoal,

            [Description("TLImwRFU TLImwBU Id" +
                " TLIallLoadInst mwBUId Id")]
            TLImwRFUTLImwBU,

            [Description("TLIcivilLoads allLoadInstId")]
            TLImwRFUTLImwBUGoal,

            [Description("TLImwRFU TLImwBULibrary Id" +
                " TLImwBU MwBULibraryId Id" +
                " TLIallLoadInst mwBUId Id")]
            TLImwRFUTLImwBULibrary,

            [Description("TLIcivilLoads allLoadInstId")]
            TLImwRFUTLImwBULibraryGoal,

            //[Description("TLImwRFU TLIsideArmLibrary Id" +
            //    " TLIsideArm sideArmLibraryId Id")]
            //TLImwRFUTLIsideArmLibrary,

            //[Description("TLIcivilLoads sideArmId")]
            //TLImwRFUTLIsideArmLibraryGoal,

            [Description("TLImwRFU TLImwRFULibrary Id")]
            TLImwRFUTLImwRFULibrary,

            [Description("MwRFULibraryId")]
            TLImwRFUTLImwRFULibraryGoal,
            #endregion
            #region MW_Other
            [Description("TLImwOther TLIsite SiteCode")]
            TLImwOtherTLIsite,

            [Description("SiteCode")]
            TLImwOtherTLIsiteGoal,

            [Description("TLImwOther TLIcivilWithLegs Id" +
                " TLIallCivilInst civilWithLegsId Id")]
            TLImwOtherTLIcivilWithLegs,

            [Description("TLIcivilLoads allCivilInstId")]
            TLImwOtherTLIcivilWithLegsGoal,

            [Description("TLImwOther TLIcivilWithLegLibrary Id" +
                " TLIallCivilInst civilWithLegsId Id" +
                " TLIcivilWithLegs CivilWithLegsLibId Id")]
            TLImwOtherTLIcivilWithLegLibrary,

            [Description("TLIcivilLoads allCivilInstId")]
            TLImwOtherTLIcivilWithLegLibraryGoal,

            [Description("TLImwOther TLIcivilWithoutLeg Id" +
                " TLIallCivilInst civilWithoutLegId Id")]
            TLImwOtherTLIcivilWithoutLeg,

            [Description("TLIcivilLoads allCivilInstId")]
            TLImwOtherTLIcivilWithoutLegGoal,

            [Description("TLImwOther TLIcivilWithoutLegLibrary Id" +
                " TLIcivilWithoutLeg CivilWithoutlegsLibId Id" +
                " TLIallCivilInst civilWithoutLegId Id")]
            TLImwOtherTLIcivilWithoutLegLibrary,

            [Description("TLIcivilLoads allCivilInstId")]
            TLImwOtherTLIcivilWithoutLegLibraryGoal,

            [Description("TLImwOther TLIcivilNonSteel Id" +
                " TLIallCivilInst civilNonSteelId Id")]
            TLImwOtherTLIcivilNonSteel,

            [Description("TLIcivilLoads allCivilInstId")]
            TLImwOtherTLIcivilNonSteelGoal,

            [Description("TLImwOther TLIcivilNonSteelLibrary Id" +
                " TLIcivilNonSteel CivilNonSteelLibraryId Id" +
                " TLIallCivilInst civilNonSteelId Id")]
            TLImwOtherTLIcivilNonSteelLibrary,

            [Description("TLIcivilLoads allCivilInstId")]
            TLImwOtherTLIcivilNonSteelLibraryGoal,

            [Description("TLImwOther TLIsideArm Id")]
            TLImwOtherTLIsideArm,

            [Description("TLIcivilLoads sideArmId")]
            TLImwOtherTLIsideArmGoal,

            [Description("TLImwOther TLIsideArmLibrary Id" +
                " TLIsideArm sideArmLibraryId Id")]
            TLImwOtherTLIsideArmLibrary,

            [Description("TLIcivilLoads sideArmId")]
            TLImwOtherTLIsideArmLibraryGoal,

            [Description("TLImwOther TLImwOtherLibrary Id")]
            TLImwOtherTLImwOtherLibrary,

            [Description("mwOtherLibraryId")]
            TLImwOtherTLImwOtherLibraryGoal,
            #endregion

            #region RadioAntenna
            [Description("TLIradioAntenna TLIsite SiteCode" +
                " TLIcivilLoads SiteCode allCivilInstId")]
            TLIradioAntennaTLIsite,

            [Description("TLIcivilLoads allCivilInstId")]
            TLIradioAntennaTLIsiteGoal,

            [Description("TLIradioAntenna TLIcivilWithLegs Id" +
                " TLIallCivilInst civilWithLegsId Id")]
            TLIradioAntennaTLIcivilWithLegs,

            [Description("TLIcivilLoads allCivilInstId")]
            TLIradioAntennaTLIcivilWithLegsGoal,

            [Description("TLIradioAntenna TLIcivilWithLegLibrary Id" +
                " TLIcivilWithLegs CivilWithLegsLibId Id" +
                " TLIallCivilInst civilWithLegsId Id")]
            TLIradioAntennaTLIcivilWithLegLibrary,

            [Description("TLIcivilLoads allCivilInstId")]
            TLIradioAntennaTLIcivilWithLegLibraryGoal,

            [Description("TLIradioAntenna TLIcivilWithoutLeg Id" +
                " TLIallCivilInst civilWithoutLegId Id")]
            TLIradioAntennaTLIcivilWithoutLeg,

            [Description("TLIcivilLoads allCivilInstId")]
            TLIradioAntennaTLIcivilWithoutLegGoal,

            [Description("TLIradioAntenna TLIcivilWithoutLegLibrary Id" +
                " TLIcivilWithoutLeg CivilWithoutlegsLibId Id" +
                " TLIallCivilInst civilWithoutLegId Id")]
            TLIradioAntennaTLIcivilWithoutLegLibrary,

            [Description("TLIcivilLoads allCivilInstId")]
            TLIradioAntennaTLIcivilWithoutLegLibraryGoal,

            [Description("TLIradioAntenna TLIcivilNonSteel Id" +
                " TLIallCivilInst civilNonSteelId Id")]
            TLIradioAntennaTLIcivilNonSteel,

            [Description("TLIcivilLoads allCivilInstId")]
            TLIradioAntennaTLIcivilNonSteelGoal,

            [Description("TLIradioAntenna TLIcivilNonSteelLibrary Id" +
                " TLIcivilNonSteel CivilNonSteelLibraryId Id" +
                " TLIallCivilInst civilNonSteelId Id")]
            TLIradioAntennaTLIcivilNonSteelLibrary,

            [Description("TLIcivilLoads allCivilInstId")]
            TLIradioAntennaTLIcivilNonSteelLibraryGoal,

            [Description("TLIradioAntenna TLIsideArm Id")]
            TLIradioAntennaTLIsideArm,

            [Description("TLIcivilLoads sideArmId")]
            TLIradioAntennaTLIsideArmGoal,

            [Description("TLIradioAntenna TLIsideArmLibrary Id" +
                " TLIsideArm sideArmLibraryId Id")]
            TLIradioAntennaTLIsideArmLibrary,

            [Description("TLIcivilLoads sideArmId")]
            TLIradioAntennaTLIsideArmLibraryGoal,

            [Description("TLIradioAntenna TLIradioAntennaLibrary Id")]
            TLIradioAntennaTLIradioAntennaLibrary,

            [Description("radioAntennaLibraryId")]
            TLIradioAntennaTLIradioAntennaLibraryGoal,
            #endregion
            #region RadioRRU
            [Description("TLIRadioRRU TLIsite SiteCode" +
                " TLIcivilLoads SiteCode allCivilInstId")]
            tliradiorrutlisite,

            [Description("TLIcivilLoads allCivilInstId")]
            tliradiorrutlisitegoal,

            [Description("TLIRadioRRU TLIcivilWithLegs Id" +
                " TLIallCivilInst civilWithLegsId Id")]
            tliradiorrutlicivilwithlegs,

            [Description("TLIcivilLoads allCivilInstId")]
            tliradiorrutlicivilwithlegsgoal,

            [Description("TLIRadioRRU TLIcivilWithLegLibrary Id" +
                " TLIcivilWithLegs CivilWithLegsLibId Id" +
                " TLIallCivilInst civilWithLegsId Id")]
            tliradiorrutlicivilwithleglibrary,

            [Description("TLIcivilLoads allCivilInstId")]
            tliradiorrutlicivilwithleglibrarygoal,

            [Description("TLIRadioRRU TLIcivilWithoutLeg Id" +
                " TLIallCivilInst civilWithoutLegId Id")]
            tliradiorrutlicivilwithoutleg,

            [Description("TLIcivilLoads allCivilInstId")]
            tliradiorrutlicivilwithoutleggoal,

            [Description("TLIRadioRRU TLIcivilWithoutLegLibrary Id" +
                " TLIcivilWithoutLeg CivilWithoutlegsLibId Id" +
                " TLIallCivilInst civilWithoutLegId Id")]
            tliradiorrutlicivilwithoutleglibrary,

            [Description("TLIcivilLoads allCivilInstId")]
            tliradiorrutlicivilwithoutleglibrarygoal,

            [Description("TLIRadioRRU TLIcivilNonSteel Id" +
                " TLIallCivilInst civilNonSteelId Id")]
            tliradiorrutlicivilnonsteel,

            [Description("TLIcivilLoads allCivilInstId")]
            tliradiorrutlicivilnonsteelgoal,

            [Description("TLIRadioRRU TLIcivilNonSteelLibrary Id" +
                " TLIcivilNonSteel CivilNonSteelLibraryId Id" +
                " TLIallCivilInst civilNonSteelId Id")]
            tliradiorrutlicivilnonsteellibrary,

            [Description("TLIcivilLoads allCivilInstId")]
            tliradiorrutlicivilnonsteellibrarygoal,

            [Description("TLIRadioRRU TLIsideArm Id")]
            tliradiorrutlisidearm,

            [Description("TLIcivilLoads sideArmId")]
            tliradiorrutlisidearmgoal,

            [Description("TLIRadioRRU TLIsideArmLibrary Id" +
                " TLIsideArm sideArmLibraryId Id")]
            tliradiorrutlisidearmlibrary,

            [Description("TLIcivilLoads sideArmId")]
            tliradiorrutlisidearmlibrarygoal,

            [Description("TLIRadioRRU TLIradioRRULibrary Id")]
            tliradiorrutliradiorrulibrary,

            [Description("radioRRULibraryId")]
            tliradiorrutliradiorrulibrarygoal,
            #endregion
            #region RadioOther
            [Description("TLIradioOther TLIsite SiteCode" +
                " TLIcivilLoads SiteCode allCivilInstId")]
            TLIradioOtherTLIsite,

            [Description("TLIcivilLoads allCivilInstId")]
            TLIradioOtherTLIsiteGoal,

            [Description("TLIradioOther TLIcivilWithLegs Id" +
                " TLIallCivilInst civilWithLegsId Id")]
            TLIradioOtherTLIcivilWithLegs,

            [Description("TLIcivilLoads allCivilInstId")]
            TLIradioOtherTLIcivilWithLegsGoal,

            [Description("TLIradioOther TLIcivilWithLegLibrary Id" +
                " TLIcivilWithLegs CivilWithLegsLibId Id" +
                " TLIallCivilInst civilWithLegsId Id")]
            TLIradioOtherTLIcivilWithLegLibrary,

            [Description("TLIcivilLoads allCivilInstId")]
            TLIradioOtherTLIcivilWithLegLibraryGoal,

            [Description("TLIradioOther TLIcivilWithoutLeg Id" +
                " TLIallCivilInst civilWithoutLegId Id")]
            TLIradioOtherTLIcivilWithoutLeg,

            [Description("TLIcivilLoads allCivilInstId")]
            TLIradioOtherTLIcivilWithoutLegGoal,

            [Description("TLIradioOther TLIcivilWithoutLegLibrary Id" +
                " TLIcivilWithoutLeg CivilWithoutlegsLibId Id" +
                " TLIallCivilInst civilWithoutLegId Id")]
            TLIradioOtherTLIcivilWithoutLegLibrary,

            [Description("TLIcivilLoads allCivilInstId")]
            TLIradioOtherTLIcivilWithoutLegLibraryGoal,

            [Description("TLIradioOther TLIcivilNonSteel Id" +
                " TLIallCivilInst civilNonSteelId Id")]
            TLIradioOtherTLIcivilNonSteel,

            [Description("TLIcivilLoads allCivilInstId")]
            TLIradioOtherTLIcivilNonSteelGoal,

            [Description("TLIradioOther TLIcivilNonSteelLibrary Id" +
                " TLIcivilNonSteel CivilNonSteelLibraryId Id" +
                " TLIallCivilInst civilNonSteelId Id")]
            TLIradioOtherTLIcivilNonSteelLibrary,

            [Description("TLIcivilLoads allCivilInstId")]
            TLIradioOtherTLIcivilNonSteelLibraryGoal,

            [Description("TLIradioOther TLIsideArm Id")]
            TLIradioOtherTLIsideArm,

            [Description("TLIcivilLoads sideArmId")]
            TLIradioOtherTLIsideArmGoal,

            [Description("TLIradioOther TLIsideArmLibrary Id" +
                " TLIsideArm sideArmLibraryId Id")]
            TLIradioOtherTLIsideArmLibrary,

            [Description("TLIcivilLoads sideArmId")]
            TLIradioOtherTLIsideArmLibraryGoal,

            [Description("TLIradioOther TLIradioOtherLibrary Id")]
            TLIradioOtherTLIradioOtherLibrary,

            [Description("radioOtherLibraryId")]
            TLIradioOtherTLIradioOtherLibraryGoal,
            #endregion

            #region Power
            [Description("TLIpower TLIsite SiteCode")]
            TLIpowerTLIsite,

            [Description("SiteCode")]
            TLIpowerTLIsiteGoal,

            [Description("TLIpower TLIcivilWithLegs Id" +
                " TLIallCivilInst civilWithLegsId Id")]
            TLIpowerTLIcivilWithLegs,

            [Description("TLIcivilLoads allCivilInstId")]
            TLIpowerTLIcivilWithLegsGoal,

            [Description("TLIpower TLIcivilWithLegLibrary Id" +
                " TLIcivilWithLegs CivilWithLegsLibId Id" +
                " TLIallCivilInst civilWithLegsId Id")]
            TLIpowerTLIcivilWithLegLibrary,

            [Description("TLIcivilLoads allCivilInstId")]
            TLIpowerTLIcivilWithLegLibraryGoal,

            [Description("TLIpower TLIcivilWithoutLeg Id" +
                " TLIallCivilInst civilWithoutLegId Id")]
            TLIpowerTLIcivilWithoutLeg,

            [Description("TLIcivilLoads allCivilInstId")]
            TLIpowerTLIcivilWithoutLegGoal,

            [Description("TLIpower TLIcivilWithoutLegLibrary Id" +
                " TLIcivilWithoutLeg CivilWithoutlegsLibId Id" +
                " TLIallCivilInst civilWithoutLegId Id")]
            TLIpowerTLIcivilWithoutLegLibrary,

            [Description("TLIcivilLoads allCivilInstId")]
            TLIpowerTLIcivilWithoutLegLibraryGoal,

            [Description("TLIpower TLIcivilNonSteel Id" +
                " TLIallCivilInst civilNonSteelId Id")]
            TLIpowerTLIcivilNonSteel,

            [Description("TLIcivilLoads allCivilInstId")]
            TLIpowerTLIcivilNonSteelGoal,

            [Description("TLIpower TLIcivilNonSteelLibrary Id" +
                " TLIcivilNonSteel CivilNonSteelLibraryId Id" +
                " TLIallCivilInst civilNonSteelId Id")]
            TLIpowerTLIcivilNonSteelLibrary,

            [Description("TLIcivilLoads allCivilInstId")]
            TLIpowerTLIcivilNonSteelLibraryGoal,

            [Description("TLIpower TLIsideArm Id")]
            TLIpowerTLIsideArm,

            [Description("TLIcivilLoads sideArmId")]
            TLIpowerTLIsideArmGoal,

            [Description("TLIpower TLIsideArmLibrary Id" +
                " TLIsideArm sideArmLibraryId Id")]
            TLIpowerTLIsideArmLibrary,

            [Description("TLIcivilLoads sideArmId")]
            TLIpowerTLIsideArmLibraryGoal,

            [Description("TLIpower TLIpowerLibrary Id")]
            TLIpowerTLIpowerLibrary,

            [Description("powerLibraryId")]
            TLIpowerTLIpowerLibraryGoal,
            #endregion

            #region LoadOther
            [Description("TLIloadOther TLIsite SiteCode")]
            TLIloadOtherTLIsite,

            [Description("SiteCode")]
            TLIloadOtherTLIsiteGoal,

            [Description("TLIloadOther TLIcivilWithLegs Id" +
                " TLIallCivilInst civilWithLegsId Id")]
            TLIloadOtherTLIcivilWithLegs,

            [Description("TLIcivilLoads allCivilInstId")]
            TLIloadOtherTLIcivilWithLegsGoal,

            [Description("TLIloadOther TLIcivilWithLegLibrary Id" +
                " TLIcivilWithLegs CivilWithLegsLibId Id" +
                " TLIallCivilInst civilWithLegsId Id")]
            TLIloadOtherTLIcivilWithLegLibrary,

            [Description("TLIcivilLoads allCivilInstId")]
            TLIloadOtherTLIcivilWithLegLibraryGoal,

            [Description("TLIloadOther TLIcivilWithoutLeg Id" +
                " TLIallCivilInst civilWithoutLegId Id")]
            TLIloadOtherTLIcivilWithoutLeg,

            [Description("TLIcivilLoads allCivilInstId")]
            TLIloadOtherTLIcivilWithoutLegGoal,

            [Description("TLIloadOther TLIcivilWithoutLegLibrary Id" +
                " TLIcivilWithoutLeg CivilWithoutlegsLibId Id" +
                " TLIallCivilInst civilWithoutLegId Id")]
            TLIloadOtherTLIcivilWithoutLegLibrary,

            [Description("TLIcivilLoads allCivilInstId")]
            TLIloadOtherTLIcivilWithoutLegLibraryGoal,

            [Description("TLIloadOther TLIcivilNonSteel Id" +
                " TLIallCivilInst civilNonSteelId Id")]
            TLIloadOtherTLIcivilNonSteel,

            [Description("TLIcivilLoads allCivilInstId")]
            TLIloadOtherTLIcivilNonSteelGoal,

            [Description("TLIloadOther TLIcivilNonSteelLibrary Id" +
                " TLIcivilNonSteel CivilNonSteelLibraryId Id" +
                " TLIallCivilInst civilNonSteelId Id")]
            TLIloadOtherCivilNonSteelLibrary,

            [Description("TLIcivilLoads allCivilInstId")]
            TLIloadOtherCivilNonSteelLibraryGoal,

            [Description("TLIloadOther TLIsideArm Id")]
            TLIloadOtherTLIsideArm,

            [Description("TLIcivilLoads sideArmId")]
            TLIloadOtherTLIsideArmGoal,

            [Description("TLIloadOther TLIsideArmLibrary Id" +
                " TLIsideArm sideArmLibraryId Id")]
            TLIloadOtherTLIsideArmLibrary,

            [Description("TLIcivilLoads sideArmId")]
            TLIloadOtherTLIsideArmLibraryGoal,

            [Description("TLIloadOther TLIloadOtherLibrary Id")]
            TLIloadOtherTLIloadOtherLibrary,

            [Description("loadOtherLibraryId")]
            TLIloadOtherTLIloadOtherLibraryGoal,
            #endregion
        }
        public enum PathToAddDynamicAttValue
        {
            #region CivilWithLeg
            [Description("TLIcivilWithLegs TLIsite SiteCode" +
                " TLIcivilSiteDate SiteCode allCivilInstId" +
                " TLIallCivilInst Id civilWithLegsId")]
            TLIcivilWithLegsTLIsite,

            [Description("TLIcivilWithLegs TLIcivilWithLegs Id")]
            TLIcivilWithLegsTLIcivilWithLegs,

            [Description("TLIcivilWithLegs TLIcivilWithLegLibrary Id" +
                " TLIcivilWithLegs CivilWithLegsLibId Id")]
            TLIcivilWithLegsTLIcivilWithLegLibrary,
            #endregion
            #region CivilWithoutLeg
            [Description("TLIcivilWithoutLeg TLIsite SiteCode" +
                " TLIcivilSiteDate SiteCode allCivilInstId" +
                " TLIallCivilInst Id civilWithoutLegId")]
            TLIcivilWithoutLegTLIsite,

            [Description("TLIcivilWithoutLeg TLIcivilWithoutLeg Id")]
            TLIcivilWithoutLegTLIcivilWithoutLeg,

            [Description("TLIcivilWithoutLeg TLIcivilWithoutLegLibrary Id" +
                " TLIcivilWithoutLeg CivilWithoutlegsLibId Id")]
            TLIcivilWithoutLegTLIcivilWithoutLegLibrary,
            #endregion
            #region CivilNonSteel
            [Description("TLIcivilNonSteel TLIsite SiteCode" +
                " TLIcivilSiteDate SiteCode allCivilInstId" +
                " TLIallCivilInst Id civilNonSteelId")]
            TLIcivilNonSteelTLIsite,

            [Description("TLIcivilNonSteel TLIcivilNonSteel Id")]
            TLIcivilNonSteelTLIcivilNonSteel,

            [Description("TLIcivilNonSteel TLIcivilNonSteelLibrary Id" +
                " TLIcivilNonSteel CivilNonSteelLibraryId Id")]
            TLIcivilNonSteelTLIcivilNonSteelLibrary,
            #endregion

            #region SideArm
            [Description("TLIsideArm TLIsite SiteCode" +
                " TLIcivilLoads SiteCode sideArmId")]
            TLIsideArmTLIsite,

            [Description("TLIsideArm TLIcivilWithLegs Id" +
                " TLIallCivilInst civilWithLegsId Id" +
                " TLIcivilLoads allCivilInstId sideArmId")]
            TLIsideArmTLIcivilWithLegs,

            [Description("TLIsideArm TLIcivilWithLegLibrary Id" +
                " TLIcivilWithLegs CivilWithLegsLibId Id" +
                " TLIallCivilInst civilWithLegsId Id" +
                " TLIcivilLoads allCivilInstId sideArmId")]
            TLIsideArmTLIcivilWithLegLibrary,

            [Description("TLIsideArm TLIcivilWithoutLeg Id" +
                " TLIallCivilInst civilWithoutLegId Id" +
                " TLIcivilLoads allCivilInstId sideArmId")]
            TLIsideArmTLIcivilWithoutLeg,

            [Description("TLIsideArm TLIcivilWithoutLegLibrary Id" +
                " TLIcivilWithoutLeg CivilWithoutlegsLibId Id" +
                " TLIallCivilInst civilWithoutLegId Id" +
                " TLIcivilLoads allCivilInstId sideArmId")]
            TLIsideArmTLIcivilWithoutLegLibrary,

            [Description("TLIsideArm TLIcivilNonSteel Id" +
                " TLIallCivilInst civilNonSteelId Id" +
                " TLIcivilLoads allCivilInstId sideArmId")]
            TLIsideArmTLIcivilNonSteel,

            [Description("TLIsideArm TLIcivilNonSteelLibrary Id" +
                " TLIcivilNonSteel CivilNonSteelLibraryId Id" +
                " TLIallCivilInst civilNonSteelId Id" +
                " TLIcivilLoads allCivilInstId sideArmId")]
            TLIsideArmTLIcivilNonSteelLibrary,

            [Description("TLIsideArm TLIsideArm Id")]
            TLIsideArmTLIsideArm,

            [Description("TLIsideArm TLIsideArmLibrary Id" +
                " TLIsideArm sideArmLibraryId Id")]
            TLIsideArmTLIsideArmLibrary,
            #endregion

            #region Cabinet
            [Description("TLIcabinet TLIsite SiteCode" +
                " TLIotherInSite SiteCode allOtherInventoryInstId" +
                " TLIallOtherInventoryInst Id cabinetId")]
            TLIcabinetTLIsite,

            [Description("TLIcabinet TLIcabinet Id")]
            TLIcabinetTLIcabinet,

            [Description("TLIcabinet TLIcabinetPowerLibrary Id" +
                " TLIcabinet CabinetPowerLibraryId Id")]
            TLIcabinetTLIcabinetPowerLibrary,

            [Description("TLIcabinet TLIcabinetTelecomLibrary Id" +
                " TLIcabinet CabinetTelecomLibraryId Id")]
            TLIcabinetTLIcabinetTelecomLibrary,
            #endregion
            #region Solar
            [Description("TLIsolar TLIsite SiteCode" +
                " TLIotherInSite SiteCode allOtherInventoryInstId" +
                " TLIallOtherInventoryInst Id solarId")]
            TLIsolarTLIsite,

            [Description("TLIsolar TLIsolar Id")]
            TLIsolarTLIsolar,

            [Description("TLIsolar TLIsolarLibrary Id" +
                " TLIsolar SolarLibraryId Id")]
            TLIsolarTLIsolarLibrary,
            #endregion
            #region Generator
            [Description("TLIgenerator TLIsite SiteCode" +
                " TLIotherInSite SiteCode allOtherInventoryInstId" +
                " TLIallOtherInventoryInst Id generatorId")]
            TLIgeneratorTLIsite,

            [Description("TLIgenerator TLIgenerator Id")]
            TLIgeneratorTLIgenerator,

            [Description("TLIgenerator TLIgeneratorLibrary Id" +
                " TLIgenerator GeneratorLibraryId Id")]
            TLIgeneratorTLIgeneratorLibrary,
            #endregion

            #region MW_Dish
            [Description("TLImwDish TLIsite SiteCode" +
                " TLIcivilLoads SiteCode allLoadInstId" +
                " TLIallLoadInst Id mwDishId")]
            TLImwDishTLIsite,

            [Description("TLImwDish TLIcivilWithLegs Id" +
                " TLIallCivilInst civilWithLegsId Id" +
                " TLIcivilLoads allCivilInstId allLoadInstId" +
                " TLIallLoadInst Id mwDishId")]
            TLImwDishTLIcivilWithLegs,

            [Description("TLImwDish TLIcivilWithLegLibrary Id" +
                " TLIcivilWithLegs CivilWithLegsLibId Id" +
                " TLIallCivilInst civilWithLegsId Id" +
                " TLIcivilLoads allCivilInstId allLoadInstId" +
                " TLIallLoadInst Id mwDishId")]
            TLImwDishTLIcivilWithLegLibrary,

            [Description("TLImwDish TLIcivilWithoutLeg Id" +
                " TLIallCivilInst civilWithoutLegId Id" +
                " TLIcivilLoads allCivilInstId allLoadInstId" +
                " TLIallLoadInst Id mwDishId")]
            TLImwDishTLIcivilWithoutLeg,

            [Description("TLImwDish TLIcivilWithoutLegLibrary Id" +
                " TLIcivilWithoutLeg CivilWithoutlegsLibId Id" +
                " TLIallCivilInst civilWithoutLegId Id" +
                " TLIcivilLoads allCivilInstId allLoadInstId" +
                " TLIallLoadInst Id mwDishId")]
            TLImwDishTLIcivilWithoutLegLibrary,

            [Description("TLImwDish TLIcivilNonSteel Id" +
                " TLIallCivilInst civilNonSteelId Id" +
                " TLIcivilLoads allCivilInstId allLoadInstId" +
                " TLIallLoadInst Id mwDishId")]
            TLImwDishTLIcivilNonSteel,

            [Description("TLImwDish TLIcivilNonSteelLibrary Id" +
                " TLIcivilNonSteel CivilNonSteelLibraryId Id" +
                " TLIallCivilInst civilNonSteelId Id" +
                " TLIcivilLoads allCivilInstId allLoadInstId" +
                " TLIallLoadInst Id mwDishId")]
            TLImwDishTLIcivilNonSteelLibrary,

            [Description("TLImwDish TLIsideArm Id" +
                " TLIcivilLoads sideArmId allLoadInstId" +
                " TLIallLoadInst Id mwDishId")]
            TLImwDishTLIsideArm,

            [Description("TLImwDish TLIsideArmLibrary Id" +
                " TLIsideArm sideArmLibraryId Id" +
                " TLIcivilLoads sideArmId allLoadInstId" +
                " TLIallLoadInst Id mwDishId")]
            TLImwDishTLIsideArmLibrary,

            [Description("TLImwDish TLImwDish Id")]
            TLImwDishTLImwDish,

            [Description("TLImwDish TLImwDishLibrary Id" +
                " TLImwDish MwDishLibraryId Id")]
            TLImwDishTLImwDishLibrary,
            #endregion
            #region MW_BU
            [Description("TLImwBU TLIsite SiteCode" +
                " TLIcivilLoads SiteCode allLoadInstId" +
                " TLIallLoadInst Id mwBUId")]
            TLImwBUTLIsite,

            [Description("TLImwBU TLIcivilWithLegs Id" +
                " TLIallCivilInst civilWithLegsId Id" +
                " TLIcivilLoads allCivilInstId allLoadInstId" +
                " TLIallLoadInst Id mwBUId")]
            TLImwBUTLIcivilWithLegs,

            [Description("TLImwBU TLIcivilWithLegLibrary Id" +
                " TLIcivilWithLegs CivilWithLegsLibId Id" +
                " TLIallCivilInst civilWithLegsId Id" +
                " TLIcivilLoads allCivilInstId allLoadInstId" +
                " TLIallLoadInst Id mwBUId")]
            TLImwBUTLIcivilWithLegLibrary,

            [Description("TLImwBU TLIcivilWithoutLeg Id" +
                " TLIallCivilInst civilWithoutLegId Id" +
                " TLIcivilLoads allCivilInstId allLoadInstId" +
                " TLIallLoadInst Id mwBUId")]
            TLImwBUTLIcivilWithoutLeg,

            [Description("TLImwBU TLIcivilWithoutLegLibrary Id" +
                " TLIcivilWithoutLeg CivilWithoutlegsLibId Id" +
                " TLIallCivilInst civilWithoutLegId Id" +
                " TLIcivilLoads allCivilInstId allLoadInstId" +
                " TLIallLoadInst Id mwBUId")]
            TLImwBUTLIcivilWithoutLegLibrary,

            [Description("TLImwBU TLIcivilNonSteel Id" +
                " TLIallCivilInst civilNonSteelId Id" +
                " TLIcivilLoads allCivilInstId allLoadInstId" +
                " TLIallLoadInst Id mwBUId")]
            TLImwBUTLIcivilNonSteel,

            [Description("TLImwBU TLIcivilNonSteelLibrary Id" +
                " TLIcivilNonSteel CivilNonSteelLibraryId Id" +
                " TLIallCivilInst civilNonSteelId Id" +
                " TLIcivilLoads allCivilInstId allLoadInstId" +
                " TLIallLoadInst Id mwBUId")]
            TLImwBUTLIcivilNonSteelLibrary,

            [Description("TLImwBU TLIsideArm Id" +
                " TLIcivilLoads sideArmId allLoadInstId" +
                " TLIallLoadInst Id mwBUId")]
            TLImwBUTLIsideArm,

            [Description("TLImwBU TLIsideArmLibrary Id" +
                " TLIsideArm sideArmLibraryId Id" +
                " TLIcivilLoads sideArmId allLoadInstId" +
                " TLIallLoadInst Id mwBUId")]
            TLImwBUTLIsideArmLibrary,

            [Description("TLImwBU TLImwBU Id")]
            TLImwBUTLImwBU,

            [Description("TLImwBU TLImwBULibrary Id" +
                " TLImwBU MwBULibraryId Id")]
            TLImwBUTLImwBULibrary,
            #endregion
            #region MW_ODU
            [Description("TLImwODU TLIsite SiteCode" +
                " TLIcivilLoads SiteCode allLoadInstId" +
                " TLIallLoadInst Id mwODUId")]
            TLImwODUTLIsite,

            [Description("TLImwODU TLIcivilWithLegs Id" +
                " TLIallCivilInst civilWithLegsId Id" +
                " TLIcivilLoads allCivilInstId allLoadInstId" +
                " TLIallLoadInst Id mwODUId")]
            TLImwODUTLIcivilWithLegs,

            [Description("TLImwODU TLIcivilWithLegLibrary Id" +
                " TLIcivilWithLegs CivilWithLegsLibId Id" +
                " TLIallCivilInst civilWithLegsId Id" +
                " TLIcivilLoads allCivilInstId allLoadInstId" +
                " TLIallLoadInst Id mwODUId")]
            TLImwODUTLIcivilWithLegLibrary,

            [Description("TLImwODU TLIcivilWithoutLeg Id" +
                " TLIallCivilInst civilWithoutLegId Id" +
                " TLIcivilLoads allCivilInstId allLoadInstId" +
                " TLIallLoadInst Id mwODUId")]
            TLImwODUTLIcivilWithoutLeg,

            [Description("TLImwODU TLIcivilWithoutLegLibrary Id" +
                " TLIcivilWithoutLeg CivilWithoutlegsLibId Id" +
                " TLIallCivilInst civilWithoutLegId Id" +
                " TLIcivilLoads allCivilInstId allLoadInstId" +
                " TLIallLoadInst Id mwODUId")]
            TLImwODUTLIcivilWithoutLegLibrary,

            [Description("TLImwODU TLIcivilNonSteel Id" +
                " TLIallCivilInst civilNonSteelId Id" +
                " TLIcivilLoads allCivilInstId allLoadInstId" +
                " TLIallLoadInst Id mwODUId")]
            TLImwODUTLIcivilNonSteel,

            [Description("TLImwODU TLIcivilNonSteelLibrary Id" +
                " TLIcivilNonSteel CivilNonSteelLibraryId Id" +
                " TLIallCivilInst civilNonSteelId Id" +
                " TLIcivilLoads allCivilInstId allLoadInstId" +
                " TLIallLoadInst Id mwODUId")]
            TLImwODUTLIcivilNonSteelLibrary,

            [Description("TLImwODU TLIsideArm Id" +
                " TLIcivilLoads sideArmId allLoadInstId" +
                " TLIallLoadInst Id mwODUId")]
            TLImwODUTLIsideArm,

            [Description("TLImwODU TLIsideArmLibrary Id" +
                " TLIsideArm sideArmLibraryId Id" +
                " TLIcivilLoads sideArmId allLoadInstId" +
                " TLIallLoadInst Id mwODUId")]
            TLImwODUTLIsideArmLibrary,

            [Description("TLImwODU TLImwODU Id")]
            TLImwODUTLImwODU,

            [Description("TLImwODU TLImwODULibrary Id" +
                " TLImwODU MwODULibraryId Id")]
            TLImwODUTLImwODULibrary,
            #endregion
            #region MW_RFU
            [Description("TLImwRFU TLIsite SiteCode" +
                " TLIcivilLoads SiteCode allLoadInstId" +
                " TLIallLoadInst Id mwRFUId")]
            TLImwRFUTLIsite,

            [Description("TLImwRFU TLIcivilWithLegs Id" +
                " TLIallCivilInst civilWithLegsId Id" +
                " TLIcivilLoads allCivilInstId allLoadInstId" +
                " TLIallLoadInst Id mwRFUId")]
            TLImwRFUTLIcivilWithLegs,

            [Description("TLImwRFU TLIcivilWithLegLibrary Id" +
                " TLIcivilWithLegs CivilWithLegsLibId Id" +
                " TLIallCivilInst civilWithLegsId Id" +
                " TLIcivilLoads allCivilInstId allLoadInstId" +
                " TLIallLoadInst Id mwRFUId")]
            TLImwRFUTLIcivilWithLegLibrary,

            [Description("TLImwRFU TLIcivilWithoutLeg Id" +
                " TLIallCivilInst civilWithoutLegId Id" +
                " TLIcivilLoads allCivilInstId allLoadInstId" +
                " TLIallLoadInst Id mwRFUId")]
            TLImwRFUTLIcivilWithoutLeg,

            [Description("TLImwRFU TLIcivilWithoutLegLibrary Id" +
                " TLIcivilWithoutLeg CivilWithoutlegsLibId Id" +
               " TLIallCivilInst civilWithoutLegId Id" +
               " TLIcivilLoads allCivilInstId allLoadInstId" +
               " TLIallLoadInst Id mwRFUId")]
            TLImwRFUTLIcivilWithoutLegLibrary,

            [Description("TLImwRFU TLIcivilNonSteel Id" +
                " TLIallCivilInst civilNonSteelId Id" +
                " TLIcivilLoads allCivilInstId allLoadInstId" +
                " TLIallLoadInst Id mwRFUId")]
            TLImwRFUTLIcivilNonSteel,

            [Description("TLImwRFU TLIcivilNonSteelLibrary Id" +
                " TLIcivilNonSteel CivilNonSteelLibraryId Id" +
                " TLIallCivilInst civilNonSteelId Id" +
                " TLIcivilLoads allCivilInstId allLoadInstId" +
                " TLIallLoadInst Id mwRFUId")]
            TLImwRFUTLIcivilNonSteelLibrary,

            [Description("TLImwRFU TLImwBU Id" +
                " TLImwPort MwBUId Id" +
                " TLImwRFU MwPortId Id")]
            TLImwRFUTLImwBU,

            [Description("TLImwRFU TLImwBULibrary Id" +
                " TLImwPort MwBULibraryId Id" +
                " TLImwRFU MwPortId Id")]
            TLImwRFUTLImwBULibrary,

            //[Description("TLImwRFU TLIsideArm Id" +
            //    " TLIcivilLoads sideArmId allLoadInstId" +
            //    " TLIallLoadInst Id mwRFUId")]
            //TLImwRFUTLIsideArm,

            //[Description("TLImwRFU TLIsideArmLibrary Id" +
            //    " TLIsideArm sideArmLibraryId Id" +
            //    " TLIcivilLoads sideArmId allLoadInstId" +
            //    " TLIallLoadInst Id mwRFUId")]
            //TLImwRFUTLIsideArmLibrary,

            [Description("TLImwRFU TLImwRFU Id")]
            TLImwRFUTLImwRFU,

            [Description("TLImwRFU TLImwRFULibrary Id" +
                " TLImwRFU MwRFULibraryId Id")]
            TLImwRFUTLImwRFULibrary,
            #endregion
            #region MW_Other
            [Description("TLImwOther TLIsite SiteCode" +
                " TLIcivilLoads SiteCode allLoadInstId" +
                " TLIallLoadInst Id mwOtherId")]
            TLImwOtherTLIsite,

            [Description("TLImwOther TLIcivilWithLegs Id" +
                " TLIallCivilInst civilWithLegsId Id" +
                " TLIcivilLoads allCivilInstId allLoadInstId" +
                " TLIallLoadInst Id mwOtherId")]
            TLImwOtherTLIcivilWithLegs,

            [Description("TLImwOther TLIcivilWithLegLibrary Id" +
                " TLIallCivilInst civilWithLegsId Id" +
                " TLIcivilWithLegs CivilWithLegsLibId Id" +
                " TLIcivilLoads allCivilInstId allLoadInstId" +
                " TLIallLoadInst Id mwOtherId")]
            TLImwOtherTLIcivilWithLegLibrary,

            [Description("TLImwOther TLIcivilWithoutLeg Id" +
                " TLIallCivilInst civilWithoutLegId Id" +
                " TLIcivilLoads allCivilInstId allLoadInstId" +
                " TLIallLoadInst Id mwOtherId")]
            TLImwOtherTLIcivilWithoutLeg,

            [Description("TLImwOther TLIcivilWithoutLegLibrary Id" +
                " TLIcivilWithoutLeg CivilWithoutlegsLibId Id" +
                " TLIallCivilInst civilWithoutLegId Id" +
                " TLIcivilLoads allCivilInstId allLoadInstId" +
                " TLIallLoadInst Id mwOtherId")]
            TLImwOtherTLIcivilWithoutLegLibrary,

            [Description("TLImwOther TLIcivilNonSteel Id" +
                " TLIallCivilInst civilNonSteelId Id" +
                " TLIcivilLoads allCivilInstId allLoadInstId" +
                " TLIallLoadInst Id mwOtherId")]
            TLImwOtherTLIcivilNonSteel,

            [Description("TLImwOther TLIcivilNonSteelLibrary Id" +
                " TLIcivilNonSteel CivilNonSteelLibraryId Id" +
                " TLIallCivilInst civilNonSteelId Id" +
                " TLIcivilLoads allCivilInstId allLoadInstId" +
                " TLIallLoadInst Id mwOtherId")]
            TLImwOtherTLIcivilNonSteelLibrary,

            [Description("TLImwOther TLIsideArm Id" +
                " TLIcivilLoads sideArmId allLoadInstId" +
                " TLIallLoadInst Id mwOtherId")]
            TLImwOtherTLIsideArm,

            [Description("TLImwOther TLIsideArmLibrary Id" +
                " TLIsideArm sideArmLibraryId Id" +
                " TLIcivilLoads sideArmId allLoadInstId" +
                " TLIallLoadInst Id mwOtherId")]
            TLImwOtherTLIsideArmLibrary,

            [Description("TLImwOther TLImwOther Id")]
            TLImwOtherTLImwOther,

            [Description("TLImwOther TLImwOtherLibrary Id" +
                " TLImwOther mwOtherLibraryId Id")]
            TLImwOtherTLImwOtherLibrary,
            #endregion

            #region RadioAntenna
            [Description("TLIradioAntenna TLIsite SiteCode" +
                " TLIcivilLoads SiteCode allLoadInstId" +
                " TLIallLoadInst Id radioAntennaId")]
            TLIradioAntennaTLIsite,

            [Description("TLIradioAntenna TLIcivilWithLegs Id" +
                " TLIallCivilInst civilWithLegsId Id" +
                " TLIcivilLoads allCivilInstId allLoadInstId" +
                " TLIallLoadInst Id radioAntennaId")]
            TLIradioAntennaTLIcivilWithLegs,

            [Description("TLIradioAntenna TLIcivilWithLegLibrary Id" +
                " TLIcivilWithLegs CivilWithLegsLibId Id" +
                " TLIallCivilInst civilWithLegsId Id" +
                " TLIcivilLoads allCivilInstId allLoadInstId" +
                " TLIallLoadInst Id radioAntennaId")]
            TLIradioAntennaTLIcivilWithLegLibrary,

            [Description("TLIradioAntenna TLIcivilWithoutLeg Id" +
                " TLIallCivilInst civilWithoutLegId Id" +
                " TLIcivilLoads allCivilInstId allLoadInstId" +
                " TLIallLoadInst Id radioAntennaId")]
            TLIradioAntennaTLIcivilWithoutLeg,

            [Description("TLIradioAntenna TLIcivilWithoutLegLibrary Id" +
                " TLIcivilWithoutLeg CivilWithoutlegsLibId Id" +
                " TLIallCivilInst civilWithoutLegId Id" +
                " TLIcivilLoads allCivilInstId allLoadInstId" +
                " TLIallLoadInst Id radioAntennaId")]
            TLIradioAntennaTLIcivilWithoutLegLibrary,

            [Description("TLIradioAntenna TLIcivilNonSteel Id" +
                " TLIallCivilInst civilNonSteelId Id" +
                " TLIcivilLoads allCivilInstId allLoadInstId" +
                " TLIallLoadInst Id radioAntennaId")]
            TLIradioAntennaTLIcivilNonSteel,

            [Description("TLIradioAntenna TLIcivilNonSteelLibrary Id" +
                " TLIcivilNonSteel CivilNonSteelLibraryId Id" +
                " TLIallCivilInst civilNonSteelId Id" +
                " TLIcivilLoads allCivilInstId allLoadInstId" +
                " TLIallLoadInst Id radioAntennaId")]
            TLIradioAntennaTLIcivilNonSteelLibrary,

            [Description("TLIradioAntenna TLIsideArm Id" +
                " TLIcivilLoads sideArmId allLoadInstId" +
                " TLIallLoadInst Id radioAntennaId")]
            TLIradioAntennaTLIsideArm,

            [Description("TLIradioAntenna TLIsideArmLibrary Id" +
                " TLIsideArm sideArmLibraryId Id" +
                " TLIcivilLoads sideArmId allLoadInstId" +
                " TLIallLoadInst Id radioAntennaId")]
            TLIradioAntennaTLIsideArmLibrary,

            [Description("TLIradioAntenna TLIradioAntenna Id")]
            TLIradioAntennaTLIradioAntenna,

            [Description("TLIradioAntenna TLIradioAntennaLibrary Id" +
                " TLIradioAntenna radioAntennaLibraryId Id")]
            TLIradioAntennaTLIradioAntennaLibrary,
            #endregion
            #region RadioRRU
            [Description("TLIRadioRRU TLIsite SiteCode" +
                " TLIcivilLoads SiteCode allLoadInstId" +
                " TLIallLoadInst Id radioRRUId")]
            tliradiorrutlisite,

            [Description("TLIRadioRRU TLIcivilWithLegs Id" +
                " TLIallCivilInst civilWithLegsId Id" +
                " TLIcivilLoads allCivilInstId allLoadInstId" +
                " TLIallLoadInst Id radioRRUId")]
            tliradiorrutlicivilwithlegs,

            [Description("TLIRadioRRU TLIcivilWithLegLibrary Id" +
                " TLIcivilWithLegs CivilWithLegsLibId Id" +
                " TLIallCivilInst civilWithLegsId Id" +
                " TLIcivilLoads allCivilInstId allLoadInstId" +
                " TLIallLoadInst Id radioRRUId")]
            tliradiorrutlicivilwithleglibrary,

            [Description("TLIRadioRRU TLIcivilWithoutLeg Id" +
                " TLIallCivilInst civilWithoutLegId Id" +
                " TLIcivilLoads allCivilInstId allLoadInstId" +
                " TLIallLoadInst Id radioRRUId")]
            tliradiorrutlicivilwithoutleg,

            [Description("TLIRadioRRU TLIcivilWithoutLegLibrary Id" +
                " TLIcivilWithoutLeg CivilWithoutlegsLibId Id" +
                " TLIallCivilInst civilWithoutLegId Id" +
                " TLIcivilLoads allCivilInstId allLoadInstId" +
                " TLIallLoadInst Id radioRRUId")]
            tliradiorrutlicivilwithoutleglibrary,

            [Description("TLIRadioRRU TLIcivilNonSteel Id" +
                " TLIallCivilInst civilNonSteelId Id" +
                " TLIcivilLoads allCivilInstId allLoadInstId" +
                " TLIallLoadInst Id radioRRUId")]
            tliradiorrutlicivilnonsteel,

            [Description("TLIRadioRRU TLIcivilNonSteelLibrary Id" +
                " TLIcivilNonSteel CivilNonSteelLibraryId Id" +
                " TLIallCivilInst civilNonSteelId Id" +
                " TLIcivilLoads allCivilInstId allLoadInstId" +
                " TLIallLoadInst Id radioRRUId")]
            tliradiorrutlicivilnonsteellibrary,

            [Description("TLIRadioRRU TLIsideArm Id" +
                " TLIcivilLoads sideArmId allLoadInstId" +
                " TLIallLoadInst Id radioRRUId")]
            tliradiorrutlisidearm,

            [Description("TLIRadioRRU TLIsideArmLibrary Id" +
                " TLIsideArm sideArmLibraryId Id" +
                " TLIcivilLoads sideArmId allLoadInstId" +
                " TLIallLoadInst Id radioRRUId")]
            tliradiorrutlisidearmlibrary,

            [Description("TLIRadioRRU TLIRadioRRU Id")]
            tliradiorrutliradiorru,

            [Description("TLIRadioRRU TLIradioRRULibrary Id" +
                " TLIRadioRRU radioRRULibraryId Id")]
            tliradiorrutliradiorrulibrary,
            #endregion
            #region RadioOther
            [Description("TLIradioOther TLIsite SiteCode" +
                " TLIcivilLoads SiteCode allLoadInstId" +
                " TLIallLoadInst Id radioOtherId")]
            TLIradioOtherTLIsite,

            [Description("TLIradioOther TLIcivilWithLegs Id" +
                " TLIallCivilInst civilWithLegsId Id" +
                " TLIcivilLoads allCivilInstId allLoadInstId" +
                " TLIallLoadInst Id radioOtherId")]
            TLIradioOtherTLIcivilWithLegs,

            [Description("TLIradioOther TLIcivilWithLegLibrary Id" +
                " TLIcivilWithLegs CivilWithLegsLibId Id" +
                " TLIallCivilInst civilWithLegsId Id" +
                " TLIcivilLoads allCivilInstId allLoadInstId" +
                " TLIallLoadInst Id radioOtherId")]
            TLIradioOtherTLIcivilWithLegLibrary,

            [Description("TLIradioOther TLIcivilWithoutLeg Id" +
                " TLIallCivilInst civilWithoutLegId Id" +
                " TLIcivilLoads allCivilInstId allLoadInstId" +
                " TLIallLoadInst Id radioOtherId")]
            TLIradioOtherTLIcivilWithoutLeg,

            [Description("TLIradioOther TLIcivilWithoutLegLibrary Id" +
                " TLIcivilWithoutLeg CivilWithoutlegsLibId Id" +
                " TLIallCivilInst civilWithoutLegId Id" +
                " TLIcivilLoads allCivilInstId allLoadInstId" +
                " TLIallLoadInst Id radioOtherId")]
            TLIradioOtherTLIcivilWithoutLegLibrary,

            [Description("TLIradioOther TLIcivilNonSteel Id" +
                " TLIallCivilInst civilNonSteelId Id" +
                " TLIcivilLoads allCivilInstId allLoadInstId" +
                " TLIallLoadInst Id radioOtherId")]
            TLIradioOtherTLIcivilNonSteel,

            [Description("TLIradioOther TLIcivilNonSteelLibrary Id" +
                " TLIcivilNonSteel CivilNonSteelLibraryId Id" +
                " TLIallCivilInst civilNonSteelId Id" +
                " TLIcivilLoads allCivilInstId allLoadInstId" +
                " TLIallLoadInst Id radioOtherId")]
            TLIradioOtherTLIcivilNonSteelLibrary,

            [Description("TLIradioOther TLIsideArm Id" +
                " TLIcivilLoads sideArmId allLoadInstId" +
                " TLIallLoadInst Id radioOtherId")]
            TLIradioOtherTLIsideArm,

            [Description("TLIradioOther TLIsideArmLibrary Id" +
                " TLIsideArm sideArmLibraryId Id" +
                " TLIcivilLoads sideArmId allLoadInstId" +
                " TLIallLoadInst Id radioOtherId")]
            TLIradioOtherTLIsideArmLibrary,

            [Description("TLIradioOther TLIradioOther Id")]
            TLIradioOtherTLIradioOther,

            [Description("TLIradioOther TLIradioOtherLibrary Id" +
                " TLIradioOther radioOtherLibraryId Id")]
            TLIradioOtherTLIradioOtherLibrary,
            #endregion

            #region Power
            [Description("TLIpower TLIsite SiteCode" +
                " TLIcivilLoads SiteCode allLoadInstId" +
                " TLIallLoadInst Id powerId")]
            TLIpowerTLIsite,

            [Description("TLIpower TLIcivilWithLegs Id" +
                " TLIallCivilInst civilWithLegsId Id" +
                " TLIcivilLoads allCivilInstId allLoadInstId" +
                " TLIallLoadInst Id powerId")]
            TLIpowerTLIcivilWithLegs,

            [Description("TLIpower TLIcivilWithLegLibrary Id" +
                " TLIcivilWithLegs CivilWithLegsLibId Id" +
                " TLIallCivilInst civilWithLegsId Id" +
                " TLIcivilLoads allCivilInstId allLoadInstId" +
                " TLIallLoadInst Id powerId")]
            TLIpowerTLIcivilWithLegLibrary,

            [Description("TLIpower TLIcivilWithoutLeg Id" +
                " TLIallCivilInst civilWithoutLegId Id" +
                " TLIcivilLoads allCivilInstId allLoadInstId" +
                " TLIallLoadInst Id powerId")]
            TLIpowerTLIcivilWithoutLeg,

            [Description("TLIpower TLIcivilWithoutLegLibrary Id" +
                " TLIcivilWithoutLeg CivilWithoutlegsLibId Id" +
                " TLIallCivilInst civilWithoutLegId Id" +
                " TLIcivilLoads allCivilInstId allLoadInstId" +
                " TLIallLoadInst Id powerId")]
            TLIpowerTLIcivilWithoutLegLibrary,

            [Description("TLIpower TLIcivilNonSteel Id" +
                " TLIallCivilInst civilNonSteelId Id" +
                " TLIcivilLoads allCivilInstId allLoadInstId" +
                " TLIallLoadInst Id powerId")]
            TLIpowerTLIcivilNonSteel,

            [Description("TLIpower TLIcivilNonSteelLibrary Id" +
                " TLIcivilNonSteel CivilNonSteelLibraryId Id" +
                " TLIallCivilInst civilNonSteelId Id" +
                " TLIcivilLoads allCivilInstId allLoadInstId" +
                " TLIallLoadInst Id powerId")]
            TLIpowerTLIcivilNonSteelLibrary,

            [Description("TLIpower TLIsideArm Id" +
                " TLIcivilLoads sideArmId allLoadInstId" +
                " TLIallLoadInst Id powerId")]
            TLIpowerTLIsideArm,

            [Description("TLIpower TLIsideArmLibrary Id" +
                " TLIsideArm sideArmLibraryId Id" +
                " TLIcivilLoads sideArmId allLoadInstId" +
                " TLIallLoadInst Id powerId")]
            TLIpowerTLIsideArmLibrary,

            [Description("TLIpower TLIpower Id")]
            TLIpowerTLIpower,

            [Description("TLIpower TLIpowerLibrary Id" +
                " TLIpower powerLibraryId Id")]
            TLIpowerTLIpowerLibrary,
            #endregion

            #region LoadOther
            [Description("TLIloadOther TLIsite SiteCode" +
                " TLIcivilLoads SiteCode allLoadInstId" +
                " TLIallLoadInst Id loadOtherId")]
            TLIloadOtherTLIsite,

            [Description("TLIloadOther TLIcivilWithLegs Id" +
                " TLIallCivilInst civilWithLegsId Id" +
                " TLIcivilLoads allCivilInstId allLoadInstId" +
                " TLIallLoadInst Id loadOtherId")]
            TLIloadOtherTLIcivilWithLegs,

            [Description("TLIloadOther TLIcivilWithLegLibrary Id" +
                " TLIcivilWithLegs CivilWithLegsLibId Id" +
                " TLIallCivilInst civilWithLegsId Id" +
                " TLIcivilLoads allCivilInstId allLoadInstId" +
                " TLIallLoadInst Id loadOtherId")]
            TLIloadOtherTLIcivilWithLegLibrary,

            [Description("TLIloadOther TLIcivilWithoutLeg Id" +
                " TLIallCivilInst civilWithoutLegId Id" +
                " TLIcivilLoads allCivilInstId allLoadInstId" +
                " TLIallLoadInst Id loadOtherId")]
            TLIloadOtherTLIcivilWithoutLeg,

            [Description("TLIloadOther TLIcivilWithoutLegLibrary Id" +
                " TLIcivilWithoutLeg CivilWithoutlegsLibId Id" +
                " TLIallCivilInst civilWithoutLegId Id" +
                " TLIcivilLoads allCivilInstId allLoadInstId" +
                " TLIallLoadInst Id loadOtherId")]
            TLIloadOtherTLIcivilWithoutLegLibrary,

            [Description("TLIloadOther TLIcivilNonSteel Id" +
                " TLIallCivilInst civilNonSteelId Id" +
                " TLIcivilLoads allCivilInstId allLoadInstId" +
                " TLIallLoadInst Id loadOtherId")]
            TLIloadOtherTLIcivilNonSteel,

            [Description("TLIloadOther TLIcivilNonSteelLibrary Id" +
                " TLIcivilNonSteel CivilNonSteelLibraryId Id" +
                " TLIallCivilInst civilNonSteelId Id" +
                " TLIcivilLoads allCivilInstId allLoadInstId" +
                " TLIallLoadInst Id loadOtherId")]
            TLIloadOtherCivilNonSteelLibrary,

            [Description("TLIloadOther TLIsideArm Id" +
                " TLIcivilLoads sideArmId allLoadInstId" +
                " TLIallLoadInst Id loadOtherId")]
            TLIloadOtherTLIsideArm,

            [Description("TLIloadOther TLIsideArmLibrary Id" +
                " TLIsideArm sideArmLibraryId Id" +
                " TLIcivilLoads sideArmId allLoadInstId" +
                " TLIallLoadInst Id loadOtherId")]
            TLIloadOtherTLIsideArmLibrary,

            [Description("TLIloadOther TLIloadOther Id")]
            TLIloadOtherTLIloadOther,

            [Description("TLIloadOther TLIloadOtherLibrary Id" +
                " TLIloadOther loadOtherLibraryId Id")]
            TLIloadOtherTLIloadOtherLibrary
            #endregion
        }
        public enum RelationsBetweenEntitiesAndViewModels
        {
            [Description("TLIcivilWithLegs CivilWithLegsDTOs")]
            CivilWithLegsViewModel,

            [Description("TLIcivilWithLegLibrary CivilWithLegLibraryDTOs")]
            CivilWithLegLibraryViewModel,

            [Description("TLIcivilNonSteel CivilNonSteelDTOs")]
            CivilNonSteelViewModel,

            [Description("TLIcivilNonSteelLibrary CivilNonSteelLibraryDTOs")]
            CivilNonSteelLibraryViewModel,

            [Description("TLIcivilWithoutLeg CivilWithoutLegDTOs")]
            CivilWithoutLegViewModel,

            [Description("TLIcivilWithoutLegLibrary CivilWithoutLegLibraryDTOs")]
            CivilWithoutLegLibraryViewModel,

            [Description("TLIsideArm SideArmDTOs")]
            SideArmViewModel,

            [Description("TLIsideArmLibrary SideArmLibraryDTOs")]
            SideArmLibraryViewModel,

            [Description("TLImwBU MW_BUDTOs")]
            MW_BUViewModel,

            [Description("TLImwBULibrary MW_BULibraryDTOs")]
            MW_BULibraryViewModel,

            [Description("TLImwDish MW_DishDTOs")]
            MW_DishViewModel,

            [Description("TLImwDishLibrary MW_DishLbraryDTOs")]
            MW_DishLibraryViewModel,

            [Description("TLImwODU MW_ODUDTOs")]
            MW_ODUViewModel,

            [Description("TLImwODULibrary MW_ODULibraryDTOs")]
            MW_ODULibraryViewModel,

            [Description("TLImwOther Mw_OtherDTOs")]
            Mw_OtherViewModel,

            [Description("TLImwOtherLibrary MW_OtherLibraryDTOs")]
            MW_OtherLibraryViewModel,

            [Description("TLImwRFU MW_RFUDTOs")]
            MW_RFUViewModel,

            [Description("TLImwRFULibrary MW_RFULibraryDTOs")]
            MW_RFULibraryViewModel,

            [Description("TLIradioOther RadioOtherDTOs")]
            RadioOtherViewModel,

            [Description("TLIradioOtherLibrary RadioOtherLibraryDTOs")]
            RadioOtherLibraryViewModel,

            [Description("TLIRadioRRU RadioRRUDTOs")]
            RadioRRUViewModel,

            [Description("TLIradioRRULibrary RadioRRULibraryDTOs")]
            RadioRRULibraryViewModel,

            [Description("TLIradioAntenna RadioAntennaDTOs")]
            RadioAntennaViewModel,

            [Description("TLIradioAntennaLibrary RadioAntennaLibraryDTOs")]
            RadioAntennaLibraryViewModel,

            [Description("TLIpower PowerDTOs")]
            PowerViewModel,

            [Description("TLIpowerLibrary PowerLibraryDTOs")]
            PowerLibraryViewModel,

            [Description("TLIloadOther LoadOtherDTOs")]
            LoadOtherViewModel,

            [Description("TLIloadOtherLibrary LoadOtherLibraryDTOs")]
            LoadOtherLibraryViewModel,

            [Description("TLIcabinet CabinetDTOs")]
            CabinetViewModel,

            [Description("TLIcabinetPowerLibrary CabinetPowerLibraryDTOs")]
            CabinetPowerLibraryViewModel,

            [Description("TLIcabinetTelecomLibrary CabinetTelecomLibraryDTOs")]
            CabinetTelecomLibraryViewModel,

            [Description("TLIsolar SolarDTOs")]
            SolarViewModel,

            [Description("TLIsolarLibrary SolarLibraryDTOs")]
            SolarLibraryViewModel,

            [Description("TLIgenerator GeneratorDTOs")]
            GeneratorViewModel,

            [Description("TLIgeneratorLibrary GeneratorLibraryDTOs")]
            GeneratorLibraryViewModel
        }
        public enum Layers
        {
            Civil,
            CivilWithLegs,
            CivilWithoutLeg,
            CivilNonSteel,

            Site,

            SideArm,

            Cabinet,
            CabinetPower,
            CabinetTelecom,
            Solar,
            Generator,

            MW_Dish,
            MW_ODU,
            MW_RFU,
            MW_BU,
            MW_Other,
            RadioAntenna,
            RadioRRU,
            RadioOther,
            Power,
            LoadOther
        }
        public enum EditableManamgmantViewNames
        {
            CivilWithLegInstallation,
            CivilWithLegsLibrary,
            CivilWithoutLegInstallationMast,
            CivilWithoutLegInstallationMonopole,
            CivilWithoutLegInstallationCapsule,
            CivilNonSteelInstallation,
            CivilNonSteelLibrary,
            CabinetPowerLibrary,
            CabinetTelecomLibrary,
            CabinetInstallation,
            SolarInstallation,
            SolarLibrary,
            GeneratorInstallation,
            GeneratorLibrary,
            MW_ODUInstallation,
            MW_ODULibrary,
            MW_RFUInstallation,
            MW_RFULibrary,
            MW_DishInstallation,
            MW_DishLibrary,
            MW_BULibrary,
            MW_BUInstallation,
            RadioAntennaInstallation,
            RadioAntennaLibrary,
            RadioRRULibrary,
            RadioRRUInstallation,
            RadioOtherLibrary,
            RadioOtherInstallation,
            PowerLibrary,
            PowerInstallation,
            SideArmInstallation,
            SideArmLibrary,
            OtherLoadLibrary,
            OtherLoadInstallation,
            OtherMWLibrary,
            OtherMWInstallation,
            CivilWithoutLegsLibraryMast,
            CivilWithoutLegsLibraryCapsule,
            CivilWithoutLegsLibraryMonopole
        }
        public const int RowNum = 10;
        public enum MouduleNames
        {
            CivilNonSteelLibrary,
            CivilWithLegLibrary,
            CivilWithoutLegLibrary,
            DynamicAtt,
            DynamicAttLibValue,
            DynamicAttInst,
            LoadOtherLibrary,
            MW_BULibrary,
            MW_DishLibrary,
            MW_ODULibrary,
            MW_OtherLibrary,
            MW_RFULibrary,
            PowerLibrary,
            RadioAntennaLibrary,
            RadioOtherLibrary,
            RadioRRULibrary,
            CabinetPowerLibrary,
            CabinetTelecomLibrary,
            GeneratorLibrary,
            SolarLibrary,
            Actor,
            BaseCivilWithLegs,
            CivilWithLegs,
            CivilWithoutLegs,
            CivilNonSteel,
            Civil,
            CivilWithLegsLibrary,
            CivilWithoutLegsCategory,
            Configuration,
            ExcelFile,
            Groups,
            GroupRole,
            GroupUser,
            GuyLineType,
            SiteFile,
            LibraryFile,
            InstallationFile,
            Inventory,
            Leg,
            LoadOther,
            ManageAttributes,
            MW_Ports,
            MW_BU,
            MW_ODU,
            MW_Dish,
            MW_RFU,
            MW_Other,
            Mw_Other,
            Cabinet,
            Solar,
            Generator,
            OtherInventory,
            Owner,
            Permission,
            Power,
            Radio,
            RadioAntenna,
            RadioRRU,
            RadioOther,
            RadioAntennaInstallation,
            RadioRRUInstallation,
            OtherRadioLibrary,
            Role,
            RolePermissions,
            SideArm,
            SideArmLibrary,
            Site,
            Loads,
            SiteSpace,
            SiteStatus,
            CivilSteel,
            SupportTypeImplemented,
            Ticket,
            Requestes,
            Token,
            User,
            Workflow,
            WorkflowSetting
        }
        public enum LibraryView
        {
            CivilWithLegsLibrary,
            CivilWithoutLegsLibrary,
            CivilNonSteelLibrary,
            OtherLoadLibrary,
            PowerLibrary,
            MW_BULibrary,
            MW_ODULibrary,
            MW_RFULibrary,
            MW_DishLibrary,
            OtherMWLibrary,
            CabinetPowerLibrary,
            CabinetTelecomLibrary,
            GeneratorLibrary,
            SolarLibrary,
            SideArmLibrary,
            RadioAntennaLibrary,
            RadioRRULibrary,
            OtherRadioLibrary
        }
        public enum InstalltionView
        {
            CivilWithLegs,
            CivilWithoutLegs,
            CivilNonSteel,
            OtherLoad,
            Power,
            MW_BU,
            MW_ODU,
            MW_RFU,
            MW_Dish,
            OtherMW,
            Cabinet,
            Generator,
            Solar,
            SideArm,
            RadioAntenna,
            RadioRRU,
            OtherRadio
        }
        public enum CivilInstallationPlaceType
        {
            CivilWithLegs = 1,
            CivilWithoutLegs = 2,
            NonCivilStael = 3
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
        public enum LogisticalType
        {
            Vendor = 1,
            Designer = 2,
            Supplier = 3,
            Manufacturer = 4
        }
        public enum TablePartName
        {
            CivilSupport = 1,
            MW = 2,
            Radio = 3,
            Power = 4,
            OtherInventory = 5,
            SideArm = 6,
            LoadOther = 7
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
            TLIsideArm,
            TLIsideArmLibrary,
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
            TLIsiteStatus,
            TLIcivilNonSteelType,
            TLIallCivilInst,
            TLImwPort,
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
            TLIinstallationCivilwithoutLegsType,
            TLIitemStatus,
            TLIleg,
            TLIloadPart,
            TLIloadSubType,
            TLIloadType,
            TLIloadOtherLibrary,
            TLIloadOther,
            TLIlog,
            TLIlogicalOperation,
            TLIlogistical,
            TLIlogisticalitem,
            TLIlogisticalType,
            TLImwBULibrary,
            TLImwDishLibrary,
            TLImwODULibrary,
            TLImwRFULibrary,
            TLImwRFU,
            TLImwOtherLibrary,
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
            TLIradioOtherLibrary,
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
            TLImwDish,
            TLImwODU,
            TLImwOther,
            TLIsideArmType,
            TLIsideArmInstallationPlace
        }
        public enum ConfigrationTablesAfterUpdate
        {
            //[Description("TLIcivilWithLegLibrary")]
            //TLIsupportTypeDesigned,
            //[Description("TLIcivilWithLegLibrary")]
            //TLIsectionsLegType,
            //[Description("TLIcivilWithLegLibrary TLIcivilWithoutLegLibrary")]
            //TLIcivilSteelSupportCategory,
            //[Description("TLIcivilWithLegLibrary TLIcivilWithoutLegLibrary")]
            //TLIstructureType,
            //[Description("TLIcivilWithoutLegLibrary")]
            //TLIinstallationCivilwithoutLegsType,
            //[Description("TLIcivilWithoutLegLibrary")]
            //TLIcivilWithoutLegCategory,
            //[Description("TLIcivilWithoutLegLibrary")]
            //TLIsubType,
            //[Description("TLIcivilNonSteelLibrary")]
            //TLIcivilNonSteelType,
            //[Description("TLImwBULibrary  TLImwRFULibrary")]
            //TLIdiversityType,
            //[Description("TLIcabinetTelecomLibrary")]
            //TLItelecomType,
            //[Description("TLImwRFULibrary")]
            //TLIboardType,
            //[Description("TLIcabinetPowerLibrary")]
            //TLIcabinetPowerType,
            //[Description("TLIgeneratorLibrary  TLIsolarLibrary")]
            //TLIcapacity,
            //[Description("TLImwDishLibrary")]
            //TLIasType,
            //[Description("TLImwDishLibrary")]
            //TLIpolarityType,
            //[Description("TLImwODULibrary")]
            //TLIparity,

            ////Installations


            //[Description("TLIdocumentType")]
            //TLIdocumentType,
            //[Description("TLIcivilWithLegs TLIcivilNonSteel")]
            //TLIsupportTypeImplemented,
            //[Description("General")]
            //TLIlogisticalType,
            //[Description("TLIcivilWithLegs")]
            //TLIbaseCivilWithLegsType,


            //[Description("TLIcivilWithLegs")]
            //TLIguyLineType,
            //[Description("TLImwDish")]
            //TLIpolarityOnLocation,
            //[Description("TLImwDish")]
            //TLIitemConnectTo,
            //[Description("TLImwDish")]
            //TLIrepeaterType,
            //[Description("TLImwODU")]
            //TLIoduInstallationType,
            //[Description("TLIsideArm")]
            //TLIsideArmInstallationPlace,
            //[Description("TLIdynamicAtt")]
            //TLIdataType,
            //[Description("")]
            //TLIoperation,
            //[Description("")]
            //TLIlogicalOperation,
            //[Description("TLIcivilWithLegs")]
            //TLIenforcmentCategory,
            //[Description("TLIpower")]
            //TLIpowerType,


            /// <summary>
            /// New Fields
            /// </summary>

            // this table don't have a relation with civilwithlegs..
            //[Description(/*TLIcivilWithLegs*/ "TLIgenerator")]
            //TLIbaseGeneratorType,

            //[Description("TLIcivilWithLegs TLIcivilNonSteel")]
            //TLIlocationType,

            //[Description("TLIcivilWithLegs")]
            //TLIbaseType,

            //[Description("TLIcivilWithLegs TLIcivilNonSteel TLIcivilWithoutLeg TLImwBU " +
            //    "TLImwDish TLImwODU TLImwRFU TLIradioAntenna TLIradioOther TLIRadioRRU " +
            //    "TLIpower TLIsideArm")]
            //TLIowner,

            //[Description("TLImwBU")]
            //TLIbaseBU,

            //[Description("TLIcabinet")]
            //TLIrenewableCabinetType,

            //[Description("TLIsideArm")]
            //TLIsideArmType,

            //[Description("TLIsideArm")]
            //TLIitemStatus,

            //[Description("TLImwBU TLImwDish TLImwOther TLIradioAntenna TLIradioOther TLIradioOther " +
            //    "TLIpower TLIloadOther")]
            //TLIinstallationPlace,

            /// <summary>
            /// After Update (Library)
            /// </summary>

            [Description("TLIsupportTypeDesigned TLIsectionsLegType TLIstructureType TLIcivilSteelSupportCategory")]
            TLIcivilWithLegLibrary,

            [Description("TLIstructureType TLIcivilSteelSupportCategory TLIinstallationCivilwithoutLegsType")]
            TLIcivilWithoutLegLibrary,

            [Description("TLIcivilNonSteelType")]
            TLIcivilNonSteelLibrary,

            [Description("TLIdiversityType")]
            TLImwBULibrary,

            [Description("TLIpolarityType TLIasType")]
            TLImwDishLibrary,

            [Description("TLIparity")]
            TLImwODULibrary,

            [Description("TLIdiversityType TLIboardType")]
            TLImwRFULibrary,

            [Description("TLIcabinetPowerType")]
            TLIcabinetPowerLibrary,

            [Description("TLItelecomType")]
            TLIcabinetTelecomLibrary,

            [Description("TLIcapacity")]
            TLIsolarLibrary,

            [Description("TLIcapacity")]
            TLIgeneratorLibrary,

            // Don't Have Any Foreign Key
            // [Description("")]
            // TLImwOtherLibrary,

            // [Description("")]
            // TLIradioOtherLibrary,

            // [Description("")]
            // TLIloadOtherLibrary,

            // [Description("")]
            // TLIradioAntennaLibrary,

            // [Description("")]
            // TLIradioRRULibrary,

            // [Description("")]
            // TLIsideArmLibrary,

            // [Description("")]
            // TLIpowerLibrary,

            /// <summary>
            /// After Update (Installation)
            /// </summary>

            [Description("TLIlocationType TLIbaseType TLIowner TLIbaseCivilWithLegsType " +
                "TLIguyLineType TLIsupportTypeImplemented TLIenforcmentCategory")]
            TLIcivilWithLegs,

            [Description("TLIowner TLIsupportTypeImplemented TLIlocationType")]
            TLIcivilNonSteel,

            [Description("TLIowner TLIsubType")]
            TLIcivilWithoutLeg,

            [Description("TLIbaseBU TLIowner")]
            TLImwBU,

            [Description("TLIowner TLIrepeaterType TLIpolarityOnLocation " +
                "TLIitemConnectTo")]
            TLImwDish,

            [Description("TLIowner")]
            TLImwODU,

            [Description("TLIowner")]
            TLImwRFU,

            [Description("TLIowner")]
            TLIradioAntenna,

            [Description("TLIowner")]
            TLIRadioRRU,

            [Description("TLIowner")]
            TLIradioRRU,

            [Description("TLIowner")]
            TLIradioOther,

            [Description("TLIowner TLIpowerType")]
            TLIpower,

            //[Description("")]
            //TLIloadOther,
            // mwother

            [Description("TLIowner")]
            TLIsideArm,

            [Description("TLIrenewableCabinetType")]
            TLIcabinet,

            [Description("TLIbaseGeneratorType")]
            TLIgenerator,

            // Don't Have Any Foreign Key
            // [Description("")]
            // TLIsolar
        }
        public enum ConfigrationTables
        {
            [Description("TLIcivilWithLegLibrary")]
            TLIsupportTypeDesigned,
            [Description("TLIcivilWithLegLibrary")]
            TLIsectionsLegType,
            [Description("TLIcivilWithLegLibrary TLIcivilWithoutLegLibrary")]
            TLIcivilSteelSupportCategory,
            [Description("TLIcivilWithLegLibrary TLIcivilWithoutLegLibrary")]
            TLIstructureType,
            [Description("TLIcivilWithoutLegLibrary")]
            TLIinstallationCivilwithoutLegsType,
            [Description("TLIcivilWithoutLegLibrary")]
            TLIcivilWithoutLegCategory,
            [Description("TLIcivilWithoutLegLibrary")]
            TLIsubType,
            [Description("TLIcivilNonSteelLibrary")]
            TLIcivilNonSteelType,
            [Description("TLImwBULibrary  TLImwRFULibrary")]
            TLIdiversityType,
            [Description("TLIcabinetTelecomLibrary")]
            TLItelecomType,
            [Description("TLImwRFULibrary")]
            TLIboardType,
            [Description("TLIcabinetPowerLibrary")]
            TLIcabinetPowerType,
            [Description("TLIgeneratorLibrary  TLIsolarLibrary")]
            TLIcapacity,
            [Description("TLImwDishLibrary")]
            TLIasType,
            [Description("TLImwDishLibrary")]
            TLIpolarityType,
            [Description("TLImwODULibrary")]
            TLIparity,

            //Installations

            [Description("TLIdocumentType")]
            TLIdocumentType,
            [Description("TLIcivilWithLegs TLIcivilNonSteel")]
            TLIsupportTypeImplemented,
            [Description("General")]
            TLIlogisticalType,
            [Description("TLIcivilWithLegs")]
            TLIbaseCivilWithLegsType,
            [Description("TLIcivilWithLegs TLIgenerator")]
            TLIbaseGeneratorType,
            [Description("TLIcivilWithLegs")]
            TLIguyLineType,
            [Description("TLImwDish")]
            TLIpolarityOnLocation,
            [Description("TLImwDish")]
            TLIitemConnectTo,
            [Description("TLImwDish")]
            TLIrepeaterType,
            [Description("TLImwODU")]
            TLIoduInstallationType,
            [Description("TLIsideArm")]
            TLIsideArmInstallationPlace,
            [Description("TLIdynamicAtt")]
            TLIdataType,
            [Description("")]
            TLIoperation,
            [Description("")]
            TLIlogicalOperation,
            [Description("TLIcivilWithLegs")]
            TLIenforcmentCategory,
            [Description("TLIpower")]
            TLIpowerType,

            [Description("TLImwBU")]
            TLIbaseBU,

            [Description("")]
            TLIowner,

            [Description("")]
            TLIlocationType,

            [Description("")]
            TLIbaseType,

            [Description("")]
            TLIrenewableCabinetType,

            [Description("")]
            TLIsideArmType,

            [Description("")]
            TLIitemStatus,

            [Description("")]
            TLIinstallationPlace
        }

        public static string GetEnumDescription(Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());

            DescriptionAttribute[] attributes =
                (DescriptionAttribute[])fi.GetCustomAttributes(
                typeof(DescriptionAttribute),
                false);

            if (attributes != null &&
                attributes.Length > 0)
                return attributes[0].Description;
            else
                return value.ToString();
        }
        public enum HistoryType
        {
            Add,
            Delete,
            UnAttached,
            Update
        }
    }
}
