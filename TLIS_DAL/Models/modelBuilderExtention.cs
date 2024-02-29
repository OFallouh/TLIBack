using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace TLIS_DAL.Models
{
    public static class modelBuilderExtention
    {
        public static void seed(this ModelBuilder modelBuilder)
        {
            #region I
            #region TLIinstallationPlace
            //modelBuilder.Entity<TLIinstallationPlace>().HasData(
            //    new TLIinstallationPlace
            //    {
            //        Id = 1,
            //        Name = "Direct"
            //    }
            //);
            //modelBuilder.Entity<TLIinstallationPlace>().HasData(
            //    new TLIinstallationPlace
            //    {
            //        Id = 2,
            //        Name = "Leg"
            //    }
            //);
            //modelBuilder.Entity<TLIinstallationPlace>().HasData(
            //    new TLIinstallationPlace
            //    {
            //        Id = 3,
            //        Name = "SideArm"
            //    }
            //);
            #endregion
            #endregion

            #region S
            #region TLIsideArmInstallationPlace
            //modelBuilder.Entity<TLIsideArmInstallationPlace>().HasData(
            //    new TLIsideArmInstallationPlace
            //    {
            //        Id = 0,
            //        Name = "NA",
            //    }
            //);
            //modelBuilder.Entity<TLIsideArmInstallationPlace>().HasData(
            //    new TLIsideArmInstallationPlace
            //    {
            //        Id = 1,
            //        Name = "Leg",
            //    }
            //);
            //modelBuilder.Entity<TLIsideArmInstallationPlace>().HasData(
            //    new TLIsideArmInstallationPlace
            //    {
            //        Id = 2,
            //        Name = "Bracing",
            //    }
            //);
            //modelBuilder.Entity<TLIsideArmInstallationPlace>().HasData(
            //   new TLIsideArmInstallationPlace
            //   {
            //       Id = 3,
            //       Name = "SupportPole",
            //   }
            //);
            //modelBuilder.Entity<TLIsideArmInstallationPlace>().HasData(
            //   new TLIsideArmInstallationPlace
            //   {
            //       Id = 4,
            //       Name = "SupportFence",
            //   }
            //);
            //modelBuilder.Entity<TLIsideArmInstallationPlace>().HasData(
            //   new TLIsideArmInstallationPlace
            //   {
            //       Id = 5,
            //       Name = "MastAboveSupport",
            //   }
            //);
            //modelBuilder.Entity<TLIsideArmInstallationPlace>().HasData(
            //   new TLIsideArmInstallationPlace
            //   {
            //       Id = 6,
            //       Name = "WallSideArm",
            //   }
            //);
            #endregion
            #endregion

            //modelBuilder.Entity<TLIowner>().HasData(
            //    new TLIowner
            //    {
            //        Id = 0,
            //        OwnerName = "NA",
            //    }
            //    );

            //modelBuilder.Entity<TLIsupportTypeImplemented>().HasData(
            //    new TLIsupportTypeImplemented
            //    {
            //        Id = 0,
            //        Name = "NA",
            //        Deleted=false,
            //        Disable=false,
            //    }
            //    );
            //modelBuilder.Entity<TLIlocationType>().HasData(
            //    new TLIlocationType
            //    {
            //        Id = 0,
            //        Name = "NA",
            //    }
            //    );
            modelBuilder.Entity<TLIcivilNonSteelLibrary>().HasData(
                new TLIcivilNonSteelLibrary
                {
                    Id = -1,
                    Model = "Water Tank Body",
                    Active = false,
                    civilNonSteelTypeId = -1,
                    Deleted = false,
                    Hight = 0,
                    Note = null,
                    NumberofBoltHoles = 0,
                    Manufactured_Max_Load = 0,
                    SpaceLibrary = 0,
                    VerticalMeasured = false
                },
                new TLIcivilNonSteelLibrary
                {
                    Id = -2,
                    Model = "Wall",
                    Active = false,
                    civilNonSteelTypeId = -2,
                    Deleted = false,
                    Hight = 0,
                    Note = null,
                    NumberofBoltHoles = 0,
                    Manufactured_Max_Load = 0,
                    SpaceLibrary = 0,
                    VerticalMeasured = false
                },
                new TLIcivilNonSteelLibrary
                {
                    Id = -3,
                    Model = "Ceiling",
                    Active = false,
                    civilNonSteelTypeId = -3,
                    Deleted = false,
                    Hight = 0,
                    Note = null,
                    NumberofBoltHoles = 0,
                    Manufactured_Max_Load = 0,
                    SpaceLibrary = 0,
                    VerticalMeasured = false
                }
            );
            modelBuilder.Entity<TLIcivilNonSteelType>().HasData(
                new TLIcivilNonSteelType
                {
                    Id = 1,
                    Name = "Water Tank Body",
                    Disable = false,
                    Deleted = false
                },
                new TLIcivilNonSteelType
                {
                    Id = 2,
                    Name = "Wall",
                    Disable = false,
                    Deleted = false
                },
                new TLIcivilNonSteelType
                {
                    Id = 3,
                    Name = "Ceiling",
                    Disable = false,
                    Deleted = false
                }
            );

            //modelBuilder.Entity<TLIsupportTypeDesigned>().HasData(
            //    new TLIsupportTypeDesigned
            //    {
            //        Id = 0,
            //        Name = "NA",
            //        Deleted=false,
            //        Disable = false,
            //    }
            //    );
            //modelBuilder.Entity<TLIsectionsLegType>().HasData(
            //    new TLIsectionsLegType
            //   {
            //       Id = 0,
            //       Name = "NA",
            //       Deleted = false,
            //       Disable = false,
            //   }
            //   );
            //modelBuilder.Entity<TLIstructureType>().HasData(
            //    new TLIstructureType
            //   {
            //      Id = 0,
            //      Name = "NA",
            //      Deleted = false,
            //      Disable = false,
            //   }
            //   );
            //modelBuilder.Entity<TLIcivilSteelSupportCategory>().HasData(
            //    new TLIcivilSteelSupportCategory
            //   {
            //    Id = 0,
            //    Name = "NA",

            //   }
            //   );
            //modelBuilder.Entity<TLIbaseType>().HasData(
            //    new TLIbaseType
            //   {
            //   Id = 0,
            //   Name = "NA",

            //   }
            //   );
            //modelBuilder.Entity<TLIbaseCivilWithLegsType>().HasData(
            //    new TLIbaseCivilWithLegsType
            //   {
            //   Id = 0,
            //   Name = "NA",
            //   Deleted = false,
            //   Disable = false,

            //   }
            //   );
            //modelBuilder.Entity<TLIguyLineType>().HasData(
            //    new TLIguyLineType
            //   {
            //   Id = 0,
            //   Name = "NA",
            //   Deleted = false,
            //   Disable = false,

            //   }
            //  );
            //modelBuilder.Entity<TLIenforcmentCategory>().HasData(
            //    new TLIenforcmentCategory
            //    {
            //        Id = 0,
            //        Name = "NA",
            //        Deleted = false,
            //        Disable = false,

            //    }
            //   );
            //modelBuilder.Entity<TLIsubType>().HasData(
            //    new TLIsubType
            //    {
            //        Id = 0,
            //        Name = "NA",

            //    }
            //   );
            //modelBuilder.Entity<TLIInstCivilwithoutLegsType>().HasData(
            //   new TLIInstCivilwithoutLegsType
            //   {
            //       Id = 0,
            //       Name = "NA",
            //       Deleted = false,
            //       Disable = false,

            //   }
            //  );
            //modelBuilder.Entity<TLIcivilWithoutLegCategory>().HasData(
            //   new TLIcivilWithoutLegCategory
            //   {
            //       Id = 0,
            //       Name = "NA",
            //       disable = false,

            //   }
            //  );
            //modelBuilder.Entity<TLIcivilWithoutLegCategory>().HasData(
            //    new TLIcivilWithoutLegCategory
            //   {
            //       Id = 0,
            //       Name = "NA",
            //       disable = false,

            //    }
            //   );
            //modelBuilder.Entity<TLIsideArmInstallationPlace>().HasData(
            //    new TLIsideArmInstallationPlace
            //   {
            //      Id = 0,
            //      Name = "NA",
            //      Deleted = false,
            //      Disable = false,

            //   }
            //   );
            //modelBuilder.Entity<TLIsideArmType>().HasData(
            //    new TLIsideArmType
            //   {
            //     Id = 0,
            //     Name = "NA",
            //     Deleted = false,
            //     Disable = false,

            //   }
            //   );
            //modelBuilder.Entity<TLIitemStatus>().HasData(
            //   new TLIitemStatus
            //   {
            //       Id = 0,
            //       Name = "NA",
            //       Deleted = false,
            //       Active=true,

            //   }
            //  );
            //modelBuilder.Entity<TLIinstallationPlace>().HasData(
            //   new TLIinstallationPlace
            //   {
            //       Id = 0,
            //       Name = "NA",

            //   }
            //  );
            //modelBuilder.Entity<TLIpowerType>().HasData(
            //   new TLIpowerType
            //  {
            //      Id = 0,
            //      Name = "NA",
            //      Delete= false,
            //      Disable = false,

            //  }
            //  );
            //modelBuilder.Entity<TLIbaseBU>().HasData(
            //    new TLIbaseBU
            //    {
            //       Id = 0,
            //       Name = "NA",

            //   }
            //   );
            //modelBuilder.Entity<TLIdiversityType>().HasData(
            //    new TLIdiversityType
            //   {
            //       Id = 0,
            //       Name = "NA",
            //       Deleted = false,
            //       Disable = false,

            //   }
            //   );
            //modelBuilder.Entity<TLIrepeaterType>().HasData(
            //    new TLIrepeaterType
            //    {
            //        Id = 0,
            //        Name = "NA",
            //        Deleted = false,
            //        Disable = false,

            //    }
            //   );
            //modelBuilder.Entity<TLIpolarityOnLocation>().HasData(
            //    new TLIpolarityOnLocation
            //    {
            //       Id = 0,
            //       Name = "NA",
            //       Deleted = false,
            //       Disable = false,

            //   }
            //   );
            //modelBuilder.Entity<TLIitemConnectTo>().HasData(
            //     new TLIitemConnectTo
            //     {
            //        Id = 0,
            //        Name = "NA",
            //        Deleted = false,
            //        Disable = false,

            //    }
            //    );
            //modelBuilder.Entity<TLIpolarityType>().HasData(
            //     new TLIpolarityType
            //     {
            //        Id = 0,
            //        Name = "NA",

            //    }
            //    );
            //modelBuilder.Entity<TLIasType>().HasData(
            //      new TLIasType
            //      {
            //         Id = 0,
            //         Name = "NA",
            //     }
            //     );
            //modelBuilder.Entity<TLIoduInstallationType>().HasData(
            //       new TLIoduInstallationType
            //      {
            //          Id = 0,
            //          Name = "NA",
            //          Deleted = false,
            //          Disable = false,

            //       }
            //      );
            //modelBuilder.Entity<TLIparity>().HasData(
            //        new TLIparity
            //       {
            //           Id = 0,
            //           Name = "NA",
            //       }
            //       );
            //modelBuilder.Entity<TLIcapacity>().HasData(
            //       new TLIcapacity
            //       {
            //           Id = 0,
            //           Name = "NA",

            //       }
            //      );
            //modelBuilder.Entity<TLIrenewableCabinetType>().HasData(
            //       new TLIrenewableCabinetType
            //       {
            //           Id = 0,
            //           Name = "NA",

            //       }
            //      );
            //modelBuilder.Entity<TLIcabinetPowerType>().HasData(
            //      new TLIcabinetPowerType
            //      {
            //          Id = 0,
            //          Name = "NA",


            //      }
            //     );
            //modelBuilder.Entity<TLItelecomType>().HasData(
            //    new TLItelecomType
            //    {
            //        Id = 0,
            //        Name = "NA",
            //        Deleted = false,
            //        Disable = false,

            //    }
            //   );
            /// modelBuilder.Entity<TLImwPort>().HasData(
            //    new TLImwPort
            //    {
            //        Id = 0,
            //        Port_Name = "NA",

            //    }
            //   );
            //modelBuilder.Entity<TLIboardType>().HasData(
            //   new TLIboardType
            //   {
            //       Id = 0,
            //       Name = "NA",
            //       Deleted = false,
            //       Disable = false,

            //   }
            //  );









            modelBuilder.Entity<TLItablesNames>().HasData(
                new TLItablesNames { Id = 1, TableName = "TLIaction", tablePartNameId = null },
                new TLItablesNames { Id = 2, TableName = "TLIactionItemOption", tablePartNameId = null },
                new TLItablesNames { Id = 3, TableName = "TLIactionOption", tablePartNameId = null },
                new TLItablesNames { Id = 4, TableName = "TLIactor", tablePartNameId = null },
                new TLItablesNames { Id = 5, TableName = "TLIarea", tablePartNameId = null },
                new TLItablesNames { Id = 6, TableName = "TLIasType", tablePartNameId = null },
                new TLItablesNames { Id = 7, TableName = "TLIattActivatedCategory", tablePartNameId = null },
                new TLItablesNames { Id = 8, TableName = "TLIattributeActivated", tablePartNameId = null },
                new TLItablesNames { Id = 9, TableName = "TLIbaseCivilWithLegsType", tablePartNameId = null },
                new TLItablesNames { Id = 10, TableName = "TLIbaseGeneratorType", tablePartNameId = null },
                new TLItablesNames { Id = 11, TableName = "TLIboardType", tablePartNameId = null },
                new TLItablesNames { Id = 12, TableName = "TLIcabinet", tablePartNameId = null },
                new TLItablesNames { Id = 13, TableName = "TLIcabinetPowerLibrary", tablePartNameId = null },
                new TLItablesNames { Id = 14, TableName = "TLIcabinetPowerType", tablePartNameId = null },
                new TLItablesNames { Id = 15, TableName = "TLIcabinetTelecomLibrary", tablePartNameId = null },
                new TLItablesNames { Id = 16, TableName = "TLIcapacity", tablePartNameId = null },
                new TLItablesNames { Id = 17, TableName = "TLIcity", tablePartNameId = null },
                new TLItablesNames { Id = 18, TableName = "TLIcivilNonSteel", tablePartNameId = null },
                new TLItablesNames { Id = 19, TableName = "TLIcivilNonSteelLibrary", tablePartNameId = null },
                new TLItablesNames { Id = 20, TableName = "TLIcivilSiteDate", tablePartNameId = null },
                new TLItablesNames { Id = 21, TableName = "TLIcivilSteelSupportCategory", tablePartNameId = null },
                new TLItablesNames { Id = 22, TableName = "TLIcivilWithLegLibrary", tablePartNameId = null },
                new TLItablesNames { Id = 23, TableName = "TLIcivilWithLegs", tablePartNameId = null },
                new TLItablesNames { Id = 24, TableName = "TLIcivilWithoutLeg", tablePartNameId = null },
                new TLItablesNames { Id = 25, TableName = "TLIcivilWithoutLegCategory", tablePartNameId = null },
                new TLItablesNames { Id = 26, TableName = "TLIcivilWithoutLegLibrary", tablePartNameId = null },
                new TLItablesNames { Id = 27, TableName = "TLIcondition", tablePartNameId = null },
                new TLItablesNames { Id = 28, TableName = "TLIconditionType", tablePartNameId = null },
                new TLItablesNames { Id = 29, TableName = "TLIdataType", tablePartNameId = null },
                new TLItablesNames { Id = 30, TableName = "TLIdependency", tablePartNameId = null },
                new TLItablesNames { Id = 31, TableName = "TLIdependencyRow", tablePartNameId = null },
                new TLItablesNames { Id = 32, TableName = "TLIdiversityType", tablePartNameId = null },
                new TLItablesNames { Id = 33, TableName = "TLIdynamicAtt", tablePartNameId = null },
                new TLItablesNames { Id = 34, TableName = "TLIdynamicAttInstValue", tablePartNameId = null },
                new TLItablesNames { Id = 35, TableName = "TLIdynamicAttLibValue", tablePartNameId = null },
                new TLItablesNames { Id = 36, TableName = "TLIgenerator", tablePartNameId = null },
                new TLItablesNames { Id = 37, TableName = "TLIgeneratorLibrary", tablePartNameId = null },
                new TLItablesNames { Id = 38, TableName = "TLIgroup", tablePartNameId = null },
                new TLItablesNames { Id = 39, TableName = "TLIgroupRole", tablePartNameId = null },
                new TLItablesNames { Id = 40, TableName = "TLIgroupUser", tablePartNameId = null },
                new TLItablesNames { Id = 41, TableName = "TLIguyLineType", tablePartNameId = null },
                new TLItablesNames { Id = 42, TableName = "TLIhistoryDetails", tablePartNameId = null },
                new TLItablesNames { Id = 43, TableName = "TLIhistoryType", tablePartNameId = null },
                new TLItablesNames { Id = 44, TableName = "TLIInstCivilwithoutLegsType", tablePartNameId = null },
                new TLItablesNames { Id = 45, TableName = "TLIinstallationPlace", tablePartNameId = null },
                new TLItablesNames { Id = 46, TableName = "TLIitemConnectTo", tablePartNameId = null },
                new TLItablesNames { Id = 47, TableName = "TLIitemStatus", tablePartNameId = null },
                new TLItablesNames { Id = 48, TableName = "TLIleg", tablePartNameId = null },
                new TLItablesNames { Id = 49, TableName = "TLIlog", tablePartNameId = null },
                new TLItablesNames { Id = 50, TableName = "TLIlogicalOperation", tablePartNameId = null },
                new TLItablesNames { Id = 51, TableName = "TLIlogistical", tablePartNameId = null },
                new TLItablesNames { Id = 52, TableName = "TLIlogisticalitem", tablePartNameId = null },
                new TLItablesNames { Id = 53, TableName = "TLIlogisticalType", tablePartNameId = null },
                new TLItablesNames { Id = 54, TableName = "TLImwBULibrary", tablePartNameId = null },
                new TLItablesNames { Id = 55, TableName = "TLImwDishLibrary", tablePartNameId = null },
                new TLItablesNames { Id = 56, TableName = "TLImwODULibrary", tablePartNameId = null },
                new TLItablesNames { Id = 57, TableName = "TLImwRFULibrary", tablePartNameId = null },
                new TLItablesNames { Id = 58, TableName = "TLIoduInstallationType", tablePartNameId = null },
                new TLItablesNames { Id = 59, TableName = "TLIoperation", tablePartNameId = null },
                new TLItablesNames { Id = 60, TableName = "TLIoption", tablePartNameId = null },
                new TLItablesNames { Id = 61, TableName = "TLIotherInSite", tablePartNameId = null },
                new TLItablesNames { Id = 62, TableName = "TLIotherInventoryDistance", tablePartNameId = null },
                new TLItablesNames { Id = 63, TableName = "TLIowner", tablePartNameId = null },
                new TLItablesNames { Id = 64, TableName = "TLIparity", tablePartNameId = null },
                new TLItablesNames { Id = 65, TableName = "TLIpermission", tablePartNameId = null },
                new TLItablesNames { Id = 66, TableName = "TLIpolarityOnLocation", tablePartNameId = null },
                new TLItablesNames { Id = 67, TableName = "TLIpolarityType", tablePartNameId = null },
                new TLItablesNames { Id = 68, TableName = "TLIpowerLibrary", tablePartNameId = null },
                new TLItablesNames { Id = 69, TableName = "TLIradioAntenna", tablePartNameId = null },
                new TLItablesNames { Id = 70, TableName = "TLIradioAntennaLibrary", tablePartNameId = null },
                new TLItablesNames { Id = 71, TableName = "TLIradioOtherLibrary", tablePartNameId = null },
                new TLItablesNames { Id = 72, TableName = "TLIradioRRU", tablePartNameId = null },
                new TLItablesNames { Id = 73, TableName = "TLIradioRRULibrary", tablePartNameId = null },
                new TLItablesNames { Id = 74, TableName = "TLIregion", tablePartNameId = null },
                new TLItablesNames { Id = 75, TableName = "TLIrenewableCabinetType", tablePartNameId = null },
                new TLItablesNames { Id = 76, TableName = "TLIrepeaterType", tablePartNameId = null },
                new TLItablesNames { Id = 77, TableName = "TLIrole", tablePartNameId = null },
                new TLItablesNames { Id = 78, TableName = "TLIrolePermission", tablePartNameId = null },
                new TLItablesNames { Id = 79, TableName = "TLIrow", tablePartNameId = null },
                new TLItablesNames { Id = 80, TableName = "TLIrowRule", tablePartNameId = null },
                new TLItablesNames { Id = 81, TableName = "TLIrule", tablePartNameId = null },
                new TLItablesNames { Id = 82, TableName = "TLIsectionsLegType", tablePartNameId = null },
                new TLItablesNames { Id = 83, TableName = "TLIsideArm", tablePartNameId = null },
                new TLItablesNames { Id = 84, TableName = "TLIsideArmInstallationPlace", tablePartNameId = null },
                new TLItablesNames { Id = 85, TableName = "TLIsideArmLibrary", tablePartNameId = null },
                new TLItablesNames { Id = 86, TableName = "TLIsite", tablePartNameId = null },
                new TLItablesNames { Id = 87, TableName = "TLIsiteStatus", tablePartNameId = null },
                new TLItablesNames { Id = 88, TableName = "TLIsolar", tablePartNameId = null },
                new TLItablesNames { Id = 90, TableName = "TLIsolarLibrary", tablePartNameId = null },
                new TLItablesNames { Id = 91, TableName = "TLIstep", tablePartNameId = null },
                new TLItablesNames { Id = 92, TableName = "TLIstepAction", tablePartNameId = null },
                new TLItablesNames { Id = 93, TableName = "TLIstepActionIncomeItemStatus", tablePartNameId = null },
                new TLItablesNames { Id = 94, TableName = "TLIstepActionItemOption", tablePartNameId = null },
                new TLItablesNames { Id = 95, TableName = "TLIstepActionOption", tablePartNameId = null },
                new TLItablesNames { Id = 96, TableName = "TLIstructureType", tablePartNameId = null },
                new TLItablesNames { Id = 97, TableName = "TLIsuboption", tablePartNameId = null },
                new TLItablesNames { Id = 98, TableName = "TLIsupportTypeDesigned", tablePartNameId = null },
                new TLItablesNames { Id = 99, TableName = "TLIsupportTypeImplemented", tablePartNameId = null },
                new TLItablesNames { Id = 101, TableName = "TLItablesHistory", tablePartNameId = null },
                new TLItablesNames { Id = 102, TableName = "TLItablesNames", tablePartNameId = null },
                new TLItablesNames { Id = 103, TableName = "TLItaskStatus", tablePartNameId = null },
                new TLItablesNames { Id = 104, TableName = "TLItelecomType", tablePartNameId = null },
                new TLItablesNames { Id = 105, TableName = "TLIuser", tablePartNameId = null },
                new TLItablesNames { Id = 106, TableName = "TLIuserPermission", tablePartNameId = null },
                new TLItablesNames { Id = 107, TableName = "TLIvalidation", tablePartNameId = null },
                new TLItablesNames { Id = 108, TableName = "TLIworkFlow", tablePartNameId = null },
                new TLItablesNames { Id = 109, TableName = "TLIworkFlowType", tablePartNameId = null },
                new TLItablesNames { Id = 110, TableName = "TLImwBU", tablePartNameId = null },
                new TLItablesNames { Id = 111, TableName = "TLImwDish", tablePartNameId = null },
                new TLItablesNames { Id = 112, TableName = "TLImwODU", tablePartNameId = null },
                new TLItablesNames { Id = 113, TableName = "TLImwRFU", tablePartNameId = null },
                new TLItablesNames { Id = 114, TableName = "TLIpower", tablePartNameId = null },
                new TLItablesNames { Id = 115, TableName = "TLIloadOther", tablePartNameId = null },
                new TLItablesNames { Id = 116, TableName = "TLIloadOtherLibrary", tablePartNameId = null },
                new TLItablesNames { Id = 117, TableName = "TLImwOther", tablePartNameId = null },
                new TLItablesNames { Id = 118, TableName = "TLImwOtherLibrary", tablePartNameId = null },
                new TLItablesNames { Id = 119, TableName = "TLIradioOther", tablePartNameId = null },
                new TLItablesNames { Id = 120, TableName = "TLImwPort", tablePartNameId = null }
                );

            modelBuilder.Entity<TLIeditableManagmentView>().HasData(
                new TLIeditableManagmentView
                {
                    Id = 1,
                    View = "CivilWithLegInstallation",
                    TLItablesNames1Id = 23
                },
                new TLIeditableManagmentView
                {
                    Id = 2,
                    View = "CivilWithLegLibrary",
                    TLItablesNames1Id = 22
                },
                new TLIeditableManagmentView
                {
                    Id = 3,
                    View = "CivilWithoutLegInstallation",
                    TLItablesNames1Id = 24
                },
                new TLIeditableManagmentView
                {
                    Id = 6,
                    View = "CivilNonSteelLibrary",
                    TLItablesNames1Id = 19
                },
                new TLIeditableManagmentView
                {
                    Id = 7,
                    View = "CabinetPowerLibrary",
                    TLItablesNames1Id = 13
                },
                new TLIeditableManagmentView
                {
                    Id = 8,
                    View = "CabinetTelecomLibrary",
                    TLItablesNames1Id = 15
                },
                new TLIeditableManagmentView
                {
                    Id = 9,
                    View = "CabinetInstallation",
                    TLItablesNames1Id = 12
                },
                new TLIeditableManagmentView
                {
                    Id = 10,
                    View = "SolarInstallation",
                    TLItablesNames1Id = 88
                },
                new TLIeditableManagmentView
                {
                    Id = 11,
                    View = "SolarLibrary",
                    TLItablesNames1Id = 90
                },
                new TLIeditableManagmentView
                {
                    Id = 12,
                    View = "GeneratorInstallation",
                    TLItablesNames1Id = 36
                },
                new TLIeditableManagmentView
                {
                    Id = 13,
                    View = "GeneratorLibrary",
                    TLItablesNames1Id = 37
                },
                new TLIeditableManagmentView
                {
                    Id = 14,
                    View = "MW_ODUInstallation",
                    TLItablesNames1Id = 112
                },
                new TLIeditableManagmentView
                {
                    Id = 15,
                    View = "MW_ODULibrary",
                    TLItablesNames1Id = 56
                },
                new TLIeditableManagmentView
                {
                    Id = 16,
                    View = "MW_RFUInstallation",
                    TLItablesNames1Id = 113
                },
                new TLIeditableManagmentView
                {
                    Id = 17,
                    View = "MW_RFULibrary",
                    TLItablesNames1Id = 57
                },
                new TLIeditableManagmentView
                {
                    Id = 18,
                    View = "MW_DishInstallation",
                    TLItablesNames1Id = 111
                },
                new TLIeditableManagmentView
                {
                    Id = 19,
                    View = "MW_DishLibrary",
                    TLItablesNames1Id = 55
                },
                new TLIeditableManagmentView
                {
                    Id = 20,
                    View = "MW_BULibrary",
                    TLItablesNames1Id = 54
                },
                new TLIeditableManagmentView
                {
                    Id = 21,
                    View = "MW_BUInstallation",
                    TLItablesNames1Id = 110
                },
                new TLIeditableManagmentView
                {
                    Id = 22,
                    View = "RadioAntennaInstallation",
                    TLItablesNames1Id = 69
                },
                new TLIeditableManagmentView
                {
                    Id = 23,
                    View = "RadioAntennaLibrary",
                    TLItablesNames1Id = 70
                },
                new TLIeditableManagmentView
                {
                    Id = 24,
                    View = "PowerLibrary",
                    TLItablesNames1Id = 68
                },
                new TLIeditableManagmentView
                {
                    Id = 25,
                    View = "PowerInstallation",
                    TLItablesNames1Id = 114
                },
                new TLIeditableManagmentView
                {
                    Id = 26,
                    View = "SideArmInstallation",
                    TLItablesNames1Id = 83
                },
                new TLIeditableManagmentView
                {
                    Id = 27,
                    View = "SideArmLibrary",
                    TLItablesNames1Id = 85
                },
                new TLIeditableManagmentView
                {
                    Id = 28,
                    View = "OtherLoadInstallation",
                    TLItablesNames1Id = 115
                },
                new TLIeditableManagmentView
                {
                    Id = 29,
                    View = "OtherLoadLibrary",
                    TLItablesNames1Id = 116
                },
                new TLIeditableManagmentView
                {
                    Id = 30,
                    View = "OtherMWInstallation",
                    TLItablesNames1Id = 117
                },
                new TLIeditableManagmentView
                {
                    Id = 31,
                    View = "OtherMWLibrary",
                    TLItablesNames1Id = 118
                },
                new TLIeditableManagmentView
                {
                    Id = 32,
                    View = "OtherRadioInstallation",
                    TLItablesNames1Id = 119
                },
                new TLIeditableManagmentView
                {
                    Id = 33,
                    View = "OtherRadioLibrary",
                    TLItablesNames1Id = 71
                },
                new TLIeditableManagmentView
                {
                    Id = 34,
                    View = "RadioRRULibrary",
                    TLItablesNames1Id = 73
                },
                new TLIeditableManagmentView
                {
                    Id = 35,
                    View = "RadioRRUInstallation",
                    TLItablesNames1Id = 72
                },
                new TLIeditableManagmentView
                {
                    Id = 36,
                    View = "CivilWithoutLegsLibraryMast",
                    TLItablesNames1Id = 26,
                    CivilWithoutLegCategoryId = 1
                },
                new TLIeditableManagmentView
                {
                    Id = 37,
                    View = "CivilWithoutLegsLibraryCapsule",
                    TLItablesNames1Id = 26,
                    CivilWithoutLegCategoryId = 2
                },
                new TLIeditableManagmentView
                {
                    Id = 38,
                    View = "CivilWithoutLegsLibraryMonopole",
                    TLItablesNames1Id = 26,
                    CivilWithoutLegCategoryId = 3
                },
                new TLIeditableManagmentView
                {
                    Id = 39,
                    View = "CivilWithoutLegInstallationMast",
                    TLItablesNames1Id = 24,
                    CivilWithoutLegCategoryId = 1
                },
                new TLIeditableManagmentView
                {
                    Id = 40,
                    View = "CivilWithoutLegInstallationCapsule",
                    TLItablesNames1Id = 24,
                    CivilWithoutLegCategoryId = 2
                },
                new TLIeditableManagmentView
                {
                    Id = 41,
                    View = "CivilWithoutLegInstallationMonopole",
                    TLItablesNames1Id = 24,
                    CivilWithoutLegCategoryId = 2
                });

            modelBuilder.Entity<TLItablePartName>().HasData(
                new TLItablePartName { Id = 1, PartName = "CivilSupport" },
                new TLItablePartName { Id = 2, PartName = "MW" },
                new TLItablePartName { Id = 3, PartName = "Radio" },
                new TLItablePartName { Id = 4, PartName = "Power" },
                new TLItablePartName { Id = 5, PartName = "OtherInventory" }
                );

            modelBuilder.Entity<TLIlogisticalType>().HasData(
                new TLIlogisticalType { Id = 1, Deleted = false, Disable = false, Name = "Vendor" },
                new TLIlogisticalType { Id = 2, Deleted = false, Disable = false, Name = "Designer" },
                new TLIlogisticalType { Id = 3, Deleted = false, Disable = false, Name = "Supplier" },
                new TLIlogisticalType { Id = 4, Deleted = false, Disable = false, Name = "Manufacturer" },
                new TLIlogisticalType { Id = 5, Deleted = false, Disable = false, Name = "Contractor" }
                );

            modelBuilder.Entity<TLIuserPermission>().HasData(
                new TLIuserPermission { Id = 1, Active = true, Deleted = false, permissionId = 1, userId = 434 },
                new TLIuserPermission { Id = 2, Active = true, Deleted = false, permissionId = 2, userId = 434 },
                new TLIuserPermission { Id = 3, Active = true, Deleted = false, permissionId = 3, userId = 434 },
                new TLIuserPermission { Id = 4, Active = true, Deleted = false, permissionId = 4, userId = 434 },
                new TLIuserPermission { Id = 5, Active = true, Deleted = false, permissionId = 5, userId = 434 },
                new TLIuserPermission { Id = 6, Active = true, Deleted = false, permissionId = 6, userId = 434 },
                new TLIuserPermission { Id = 7, Active = true, Deleted = false, permissionId = 7, userId = 434 },
                new TLIuserPermission { Id = 8, Active = true, Deleted = false, permissionId = 8, userId = 434 },
                new TLIuserPermission { Id = 9, Active = true, Deleted = false, permissionId = 9, userId = 434 },

                new TLIuserPermission { Id = 10, Active = true, Deleted = false, permissionId = 10, userId = 434 },
                new TLIuserPermission { Id = 11, Active = true, Deleted = false, permissionId = 11, userId = 434 },
                new TLIuserPermission { Id = 12, Active = true, Deleted = false, permissionId = 12, userId = 434 },
                new TLIuserPermission { Id = 13, Active = true, Deleted = false, permissionId = 13, userId = 434 },
                new TLIuserPermission { Id = 14, Active = true, Deleted = false, permissionId = 14, userId = 434 },
                new TLIuserPermission { Id = 15, Active = true, Deleted = false, permissionId = 15, userId = 434 },
                new TLIuserPermission { Id = 16, Active = true, Deleted = false, permissionId = 16, userId = 434 },
                new TLIuserPermission { Id = 17, Active = true, Deleted = false, permissionId = 17, userId = 434 },
                new TLIuserPermission { Id = 18, Active = true, Deleted = false, permissionId = 18, userId = 434 },
                new TLIuserPermission { Id = 19, Active = true, Deleted = false, permissionId = 19, userId = 434 },

                new TLIuserPermission { Id = 20, Active = true, Deleted = false, permissionId = 20, userId = 434 },
                new TLIuserPermission { Id = 21, Active = true, Deleted = false, permissionId = 21, userId = 434 },
                new TLIuserPermission { Id = 22, Active = true, Deleted = false, permissionId = 22, userId = 434 },
                new TLIuserPermission { Id = 23, Active = true, Deleted = false, permissionId = 23, userId = 434 },
                new TLIuserPermission { Id = 24, Active = true, Deleted = false, permissionId = 24, userId = 434 },
                new TLIuserPermission { Id = 25, Active = true, Deleted = false, permissionId = 25, userId = 434 },
                new TLIuserPermission { Id = 26, Active = true, Deleted = false, permissionId = 26, userId = 434 },
                new TLIuserPermission { Id = 27, Active = true, Deleted = false, permissionId = 27, userId = 434 },
                new TLIuserPermission { Id = 28, Active = true, Deleted = false, permissionId = 28, userId = 434 },
                new TLIuserPermission { Id = 29, Active = true, Deleted = false, permissionId = 29, userId = 434 },

                new TLIuserPermission { Id = 30, Active = true, Deleted = false, permissionId = 30, userId = 434 },
                new TLIuserPermission { Id = 31, Active = true, Deleted = false, permissionId = 31, userId = 434 },
                new TLIuserPermission { Id = 32, Active = true, Deleted = false, permissionId = 32, userId = 434 },
                new TLIuserPermission { Id = 33, Active = true, Deleted = false, permissionId = 33, userId = 434 },
                new TLIuserPermission { Id = 34, Active = true, Deleted = false, permissionId = 34, userId = 434 },
                new TLIuserPermission { Id = 35, Active = true, Deleted = false, permissionId = 35, userId = 434 },
                new TLIuserPermission { Id = 36, Active = true, Deleted = false, permissionId = 36, userId = 434 },
                new TLIuserPermission { Id = 37, Active = true, Deleted = false, permissionId = 37, userId = 434 },
                new TLIuserPermission { Id = 38, Active = true, Deleted = false, permissionId = 38, userId = 434 },
                new TLIuserPermission { Id = 39, Active = true, Deleted = false, permissionId = 39, userId = 434 },

                new TLIuserPermission { Id = 40, Active = true, Deleted = false, permissionId = 40, userId = 434 },
                new TLIuserPermission { Id = 41, Active = true, Deleted = false, permissionId = 41, userId = 434 },
                new TLIuserPermission { Id = 42, Active = true, Deleted = false, permissionId = 42, userId = 434 },
                new TLIuserPermission { Id = 43, Active = true, Deleted = false, permissionId = 43, userId = 434 },
                new TLIuserPermission { Id = 44, Active = true, Deleted = false, permissionId = 44, userId = 434 },
                new TLIuserPermission { Id = 45, Active = true, Deleted = false, permissionId = 45, userId = 434 },
                new TLIuserPermission { Id = 46, Active = true, Deleted = false, permissionId = 46, userId = 434 },
                new TLIuserPermission { Id = 47, Active = true, Deleted = false, permissionId = 47, userId = 434 },
                new TLIuserPermission { Id = 48, Active = true, Deleted = false, permissionId = 48, userId = 434 },
                new TLIuserPermission { Id = 49, Active = true, Deleted = false, permissionId = 49, userId = 434 },

                new TLIuserPermission { Id = 50, Active = true, Deleted = false, permissionId = 50, userId = 434 },
                new TLIuserPermission { Id = 51, Active = true, Deleted = false, permissionId = 51, userId = 434 },
                new TLIuserPermission { Id = 52, Active = true, Deleted = false, permissionId = 52, userId = 434 },
                new TLIuserPermission { Id = 53, Active = true, Deleted = false, permissionId = 53, userId = 434 },
                new TLIuserPermission { Id = 54, Active = true, Deleted = false, permissionId = 54, userId = 434 },
                new TLIuserPermission { Id = 55, Active = true, Deleted = false, permissionId = 55, userId = 434 },
                new TLIuserPermission { Id = 56, Active = true, Deleted = false, permissionId = 56, userId = 434 },
                new TLIuserPermission { Id = 57, Active = true, Deleted = false, permissionId = 57, userId = 434 },
                new TLIuserPermission { Id = 58, Active = true, Deleted = false, permissionId = 58, userId = 434 },
                new TLIuserPermission { Id = 59, Active = true, Deleted = false, permissionId = 59, userId = 434 },

                new TLIuserPermission { Id = 60, Active = true, Deleted = false, permissionId = 60, userId = 434 },
                new TLIuserPermission { Id = 61, Active = true, Deleted = false, permissionId = 61, userId = 434 },
                new TLIuserPermission { Id = 62, Active = true, Deleted = false, permissionId = 62, userId = 434 },
                new TLIuserPermission { Id = 63, Active = true, Deleted = false, permissionId = 63, userId = 434 },
                new TLIuserPermission { Id = 64, Active = true, Deleted = false, permissionId = 64, userId = 434 },
                new TLIuserPermission { Id = 65, Active = true, Deleted = false, permissionId = 65, userId = 434 },
                new TLIuserPermission { Id = 66, Active = true, Deleted = false, permissionId = 66, userId = 434 },
                new TLIuserPermission { Id = 67, Active = true, Deleted = false, permissionId = 67, userId = 434 },
                new TLIuserPermission { Id = 68, Active = true, Deleted = false, permissionId = 68, userId = 434 },
                new TLIuserPermission { Id = 69, Active = true, Deleted = false, permissionId = 69, userId = 434 },

                new TLIuserPermission { Id = 70, Active = true, Deleted = false, permissionId = 70, userId = 434 },
                new TLIuserPermission { Id = 71, Active = true, Deleted = false, permissionId = 71, userId = 434 },
                new TLIuserPermission { Id = 72, Active = true, Deleted = false, permissionId = 72, userId = 434 },
                new TLIuserPermission { Id = 73, Active = true, Deleted = false, permissionId = 73, userId = 434 },
                new TLIuserPermission { Id = 74, Active = true, Deleted = false, permissionId = 74, userId = 434 },
                new TLIuserPermission { Id = 75, Active = true, Deleted = false, permissionId = 75, userId = 434 },
                new TLIuserPermission { Id = 76, Active = true, Deleted = false, permissionId = 76, userId = 434 },
                new TLIuserPermission { Id = 77, Active = true, Deleted = false, permissionId = 77, userId = 434 },
                new TLIuserPermission { Id = 78, Active = true, Deleted = false, permissionId = 78, userId = 434 },
                new TLIuserPermission { Id = 79, Active = true, Deleted = false, permissionId = 79, userId = 434 },

                new TLIuserPermission { Id = 80, Active = true, Deleted = false, permissionId = 80, userId = 434 },
                new TLIuserPermission { Id = 81, Active = true, Deleted = false, permissionId = 81, userId = 434 },
                new TLIuserPermission { Id = 82, Active = true, Deleted = false, permissionId = 82, userId = 434 },
                new TLIuserPermission { Id = 83, Active = true, Deleted = false, permissionId = 83, userId = 434 },
                new TLIuserPermission { Id = 84, Active = true, Deleted = false, permissionId = 84, userId = 434 },
                new TLIuserPermission { Id = 85, Active = true, Deleted = false, permissionId = 85, userId = 434 },
                new TLIuserPermission { Id = 86, Active = true, Deleted = false, permissionId = 86, userId = 434 },
                new TLIuserPermission { Id = 87, Active = true, Deleted = false, permissionId = 87, userId = 434 },
                new TLIuserPermission { Id = 88, Active = true, Deleted = false, permissionId = 88, userId = 434 },
                new TLIuserPermission { Id = 89, Active = true, Deleted = false, permissionId = 89, userId = 434 },

                new TLIuserPermission { Id = 90, Active = true, Deleted = false, permissionId = 90, userId = 434 },
                new TLIuserPermission { Id = 91, Active = true, Deleted = false, permissionId = 91, userId = 434 },
                new TLIuserPermission { Id = 92, Active = true, Deleted = false, permissionId = 92, userId = 434 },
                new TLIuserPermission { Id = 93, Active = true, Deleted = false, permissionId = 93, userId = 434 },
                new TLIuserPermission { Id = 94, Active = true, Deleted = false, permissionId = 94, userId = 434 },
                new TLIuserPermission { Id = 95, Active = true, Deleted = false, permissionId = 95, userId = 434 },
                new TLIuserPermission { Id = 96, Active = true, Deleted = false, permissionId = 96, userId = 434 },
                new TLIuserPermission { Id = 97, Active = true, Deleted = false, permissionId = 97, userId = 434 },
                new TLIuserPermission { Id = 98, Active = true, Deleted = false, permissionId = 98, userId = 434 },
                new TLIuserPermission { Id = 99, Active = true, Deleted = false, permissionId = 99, userId = 434 },

                new TLIuserPermission { Id = 100, Active = true, Deleted = false, permissionId = 100, userId = 434 },
                new TLIuserPermission { Id = 101, Active = true, Deleted = false, permissionId = 101, userId = 434 },
                new TLIuserPermission { Id = 102, Active = true, Deleted = false, permissionId = 102, userId = 434 },
                new TLIuserPermission { Id = 103, Active = true, Deleted = false, permissionId = 103, userId = 434 },
                new TLIuserPermission { Id = 104, Active = true, Deleted = false, permissionId = 104, userId = 434 },
                new TLIuserPermission { Id = 105, Active = true, Deleted = false, permissionId = 105, userId = 434 },
                new TLIuserPermission { Id = 106, Active = true, Deleted = false, permissionId = 106, userId = 434 },
                new TLIuserPermission { Id = 107, Active = true, Deleted = false, permissionId = 107, userId = 434 },
                new TLIuserPermission { Id = 108, Active = true, Deleted = false, permissionId = 108, userId = 434 },
                new TLIuserPermission { Id = 109, Active = true, Deleted = false, permissionId = 109, userId = 434 },

                new TLIuserPermission { Id = 110, Active = true, Deleted = false, permissionId = 110, userId = 434 },
                new TLIuserPermission { Id = 111, Active = true, Deleted = false, permissionId = 111, userId = 434 },
                new TLIuserPermission { Id = 112, Active = true, Deleted = false, permissionId = 112, userId = 434 },
                new TLIuserPermission { Id = 113, Active = true, Deleted = false, permissionId = 113, userId = 434 },
                new TLIuserPermission { Id = 114, Active = true, Deleted = false, permissionId = 114, userId = 434 },
                new TLIuserPermission { Id = 115, Active = true, Deleted = false, permissionId = 115, userId = 434 },
                new TLIuserPermission { Id = 116, Active = true, Deleted = false, permissionId = 116, userId = 434 },
                new TLIuserPermission { Id = 117, Active = true, Deleted = false, permissionId = 117, userId = 434 },
                new TLIuserPermission { Id = 118, Active = true, Deleted = false, permissionId = 118, userId = 434 },
                new TLIuserPermission { Id = 119, Active = true, Deleted = false, permissionId = 119, userId = 434 },

                new TLIuserPermission { Id = 120, Active = true, Deleted = false, permissionId = 120, userId = 434 },
                new TLIuserPermission { Id = 121, Active = true, Deleted = false, permissionId = 121, userId = 434 },
                new TLIuserPermission { Id = 122, Active = true, Deleted = false, permissionId = 122, userId = 434 },
                new TLIuserPermission { Id = 123, Active = true, Deleted = false, permissionId = 123, userId = 434 },
                new TLIuserPermission { Id = 124, Active = true, Deleted = false, permissionId = 124, userId = 434 },
                new TLIuserPermission { Id = 125, Active = true, Deleted = false, permissionId = 125, userId = 434 },
                new TLIuserPermission { Id = 126, Active = true, Deleted = false, permissionId = 126, userId = 434 },
                new TLIuserPermission { Id = 127, Active = true, Deleted = false, permissionId = 127, userId = 434 },
                new TLIuserPermission { Id = 128, Active = true, Deleted = false, permissionId = 128, userId = 434 },
                new TLIuserPermission { Id = 129, Active = true, Deleted = false, permissionId = 129, userId = 434 },

                new TLIuserPermission { Id = 130, Active = true, Deleted = false, permissionId = 130, userId = 434 },
                new TLIuserPermission { Id = 131, Active = true, Deleted = false, permissionId = 131, userId = 434 },
                new TLIuserPermission { Id = 132, Active = true, Deleted = false, permissionId = 132, userId = 434 },
                new TLIuserPermission { Id = 133, Active = true, Deleted = false, permissionId = 133, userId = 434 },
                new TLIuserPermission { Id = 134, Active = true, Deleted = false, permissionId = 134, userId = 434 },
                new TLIuserPermission { Id = 135, Active = true, Deleted = false, permissionId = 135, userId = 434 },
                new TLIuserPermission { Id = 136, Active = true, Deleted = false, permissionId = 136, userId = 434 },
                new TLIuserPermission { Id = 137, Active = true, Deleted = false, permissionId = 137, userId = 434 },
                new TLIuserPermission { Id = 138, Active = true, Deleted = false, permissionId = 138, userId = 434 },
                new TLIuserPermission { Id = 139, Active = true, Deleted = false, permissionId = 139, userId = 434 },

                new TLIuserPermission { Id = 140, Active = true, Deleted = false, permissionId = 140, userId = 434 },
                new TLIuserPermission { Id = 141, Active = true, Deleted = false, permissionId = 141, userId = 434 },
                new TLIuserPermission { Id = 142, Active = true, Deleted = false, permissionId = 142, userId = 434 },
                new TLIuserPermission { Id = 143, Active = true, Deleted = false, permissionId = 143, userId = 434 },
                new TLIuserPermission { Id = 144, Active = true, Deleted = false, permissionId = 144, userId = 434 },
                new TLIuserPermission { Id = 145, Active = true, Deleted = false, permissionId = 145, userId = 434 },
                new TLIuserPermission { Id = 146, Active = true, Deleted = false, permissionId = 146, userId = 434 },
                new TLIuserPermission { Id = 147, Active = true, Deleted = false, permissionId = 147, userId = 434 },
                new TLIuserPermission { Id = 148, Active = true, Deleted = false, permissionId = 148, userId = 434 },
                new TLIuserPermission { Id = 149, Active = true, Deleted = false, permissionId = 149, userId = 434 },

                new TLIuserPermission { Id = 150, Active = true, Deleted = false, permissionId = 150, userId = 434 },
                new TLIuserPermission { Id = 151, Active = true, Deleted = false, permissionId = 151, userId = 434 },
                new TLIuserPermission { Id = 152, Active = true, Deleted = false, permissionId = 152, userId = 434 },
                new TLIuserPermission { Id = 153, Active = true, Deleted = false, permissionId = 153, userId = 434 },
                new TLIuserPermission { Id = 154, Active = true, Deleted = false, permissionId = 154, userId = 434 },
                new TLIuserPermission { Id = 155, Active = true, Deleted = false, permissionId = 155, userId = 434 },
                new TLIuserPermission { Id = 156, Active = true, Deleted = false, permissionId = 156, userId = 434 },
                new TLIuserPermission { Id = 157, Active = true, Deleted = false, permissionId = 157, userId = 434 },
                new TLIuserPermission { Id = 158, Active = true, Deleted = false, permissionId = 158, userId = 434 },
                new TLIuserPermission { Id = 159, Active = true, Deleted = false, permissionId = 159, userId = 434 },

                new TLIuserPermission { Id = 160, Active = true, Deleted = false, permissionId = 160, userId = 434 },
                new TLIuserPermission { Id = 161, Active = true, Deleted = false, permissionId = 161, userId = 434 },
                new TLIuserPermission { Id = 162, Active = true, Deleted = false, permissionId = 162, userId = 434 },
                new TLIuserPermission { Id = 163, Active = true, Deleted = false, permissionId = 163, userId = 434 },
                new TLIuserPermission { Id = 164, Active = true, Deleted = false, permissionId = 164, userId = 434 },
                new TLIuserPermission { Id = 165, Active = true, Deleted = false, permissionId = 165, userId = 434 },
                new TLIuserPermission { Id = 166, Active = true, Deleted = false, permissionId = 166, userId = 434 },
                new TLIuserPermission { Id = 167, Active = true, Deleted = false, permissionId = 167, userId = 434 },
                new TLIuserPermission { Id = 168, Active = true, Deleted = false, permissionId = 168, userId = 434 },
                new TLIuserPermission { Id = 169, Active = true, Deleted = false, permissionId = 169, userId = 434 },

                new TLIuserPermission { Id = 170, Active = true, Deleted = false, permissionId = 170, userId = 434 },
                new TLIuserPermission { Id = 171, Active = true, Deleted = false, permissionId = 171, userId = 434 },
                new TLIuserPermission { Id = 172, Active = true, Deleted = false, permissionId = 172, userId = 434 },
                new TLIuserPermission { Id = 173, Active = true, Deleted = false, permissionId = 173, userId = 434 },
                new TLIuserPermission { Id = 174, Active = true, Deleted = false, permissionId = 174, userId = 434 },
                new TLIuserPermission { Id = 175, Active = true, Deleted = false, permissionId = 175, userId = 434 },
                new TLIuserPermission { Id = 176, Active = true, Deleted = false, permissionId = 176, userId = 434 },
                new TLIuserPermission { Id = 177, Active = true, Deleted = false, permissionId = 177, userId = 434 },
                new TLIuserPermission { Id = 178, Active = true, Deleted = false, permissionId = 178, userId = 434 },
                new TLIuserPermission { Id = 179, Active = true, Deleted = false, permissionId = 179, userId = 434 },

                new TLIuserPermission { Id = 180, Active = true, Deleted = false, permissionId = 180, userId = 434 },
                new TLIuserPermission { Id = 181, Active = true, Deleted = false, permissionId = 181, userId = 434 },
                new TLIuserPermission { Id = 182, Active = true, Deleted = false, permissionId = 182, userId = 434 },
                new TLIuserPermission { Id = 183, Active = true, Deleted = false, permissionId = 183, userId = 434 },
                new TLIuserPermission { Id = 184, Active = true, Deleted = false, permissionId = 184, userId = 434 },
                new TLIuserPermission { Id = 185, Active = true, Deleted = false, permissionId = 185, userId = 434 },
                new TLIuserPermission { Id = 186, Active = true, Deleted = false, permissionId = 186, userId = 434 },
                new TLIuserPermission { Id = 187, Active = true, Deleted = false, permissionId = 187, userId = 434 },
                new TLIuserPermission { Id = 188, Active = true, Deleted = false, permissionId = 188, userId = 434 },
                new TLIuserPermission { Id = 189, Active = true, Deleted = false, permissionId = 189, userId = 434 },

                new TLIuserPermission { Id = 190, Active = true, Deleted = false, permissionId = 190, userId = 434 },
                new TLIuserPermission { Id = 191, Active = true, Deleted = false, permissionId = 191, userId = 434 },
                new TLIuserPermission { Id = 192, Active = true, Deleted = false, permissionId = 192, userId = 434 },
                new TLIuserPermission { Id = 193, Active = true, Deleted = false, permissionId = 193, userId = 434 },
                new TLIuserPermission { Id = 194, Active = true, Deleted = false, permissionId = 194, userId = 434 },
                new TLIuserPermission { Id = 195, Active = true, Deleted = false, permissionId = 195, userId = 434 },
                new TLIuserPermission { Id = 196, Active = true, Deleted = false, permissionId = 196, userId = 434 },
                new TLIuserPermission { Id = 197, Active = true, Deleted = false, permissionId = 197, userId = 434 },
                new TLIuserPermission { Id = 198, Active = true, Deleted = false, permissionId = 198, userId = 434 },
                new TLIuserPermission { Id = 199, Active = true, Deleted = false, permissionId = 199, userId = 434 },

                new TLIuserPermission { Id = 200, Active = true, Deleted = false, permissionId = 200, userId = 434 },
                new TLIuserPermission { Id = 201, Active = true, Deleted = false, permissionId = 201, userId = 434 },
                new TLIuserPermission { Id = 202, Active = true, Deleted = false, permissionId = 202, userId = 434 },
                new TLIuserPermission { Id = 203, Active = true, Deleted = false, permissionId = 203, userId = 434 },
                new TLIuserPermission { Id = 204, Active = true, Deleted = false, permissionId = 204, userId = 434 },
                new TLIuserPermission { Id = 205, Active = true, Deleted = false, permissionId = 205, userId = 434 },
                new TLIuserPermission { Id = 206, Active = true, Deleted = false, permissionId = 206, userId = 434 },
                new TLIuserPermission { Id = 207, Active = true, Deleted = false, permissionId = 207, userId = 434 },
                new TLIuserPermission { Id = 208, Active = true, Deleted = false, permissionId = 208, userId = 434 },
                new TLIuserPermission { Id = 209, Active = true, Deleted = false, permissionId = 209, userId = 434 },

                new TLIuserPermission { Id = 210, Active = true, Deleted = false, permissionId = 210, userId = 434 },
                new TLIuserPermission { Id = 211, Active = true, Deleted = false, permissionId = 211, userId = 434 },
                new TLIuserPermission { Id = 212, Active = true, Deleted = false, permissionId = 212, userId = 434 },
                new TLIuserPermission { Id = 213, Active = true, Deleted = false, permissionId = 213, userId = 434 },
                new TLIuserPermission { Id = 214, Active = true, Deleted = false, permissionId = 214, userId = 434 },
                new TLIuserPermission { Id = 215, Active = true, Deleted = false, permissionId = 215, userId = 434 },
                new TLIuserPermission { Id = 216, Active = true, Deleted = false, permissionId = 216, userId = 434 },
                new TLIuserPermission { Id = 217, Active = true, Deleted = false, permissionId = 217, userId = 434 },
                new TLIuserPermission { Id = 218, Active = true, Deleted = false, permissionId = 218, userId = 434 },
                new TLIuserPermission { Id = 219, Active = true, Deleted = false, permissionId = 219, userId = 434 },

                new TLIuserPermission { Id = 220, Active = true, Deleted = false, permissionId = 220, userId = 434 },
                new TLIuserPermission { Id = 221, Active = true, Deleted = false, permissionId = 221, userId = 434 },
                new TLIuserPermission { Id = 222, Active = true, Deleted = false, permissionId = 222, userId = 434 },
                new TLIuserPermission { Id = 223, Active = true, Deleted = false, permissionId = 223, userId = 434 },
                new TLIuserPermission { Id = 224, Active = true, Deleted = false, permissionId = 224, userId = 434 },
                new TLIuserPermission { Id = 225, Active = true, Deleted = false, permissionId = 225, userId = 434 },
                new TLIuserPermission { Id = 226, Active = true, Deleted = false, permissionId = 226, userId = 434 },
                new TLIuserPermission { Id = 227, Active = true, Deleted = false, permissionId = 227, userId = 434 },
                new TLIuserPermission { Id = 228, Active = true, Deleted = false, permissionId = 228, userId = 434 },
                new TLIuserPermission { Id = 229, Active = true, Deleted = false, permissionId = 229, userId = 434 },

                new TLIuserPermission { Id = 230, Active = true, Deleted = false, permissionId = 230, userId = 434 },
                new TLIuserPermission { Id = 231, Active = true, Deleted = false, permissionId = 231, userId = 434 },
                new TLIuserPermission { Id = 232, Active = true, Deleted = false, permissionId = 232, userId = 434 },
                new TLIuserPermission { Id = 233, Active = true, Deleted = false, permissionId = 233, userId = 434 },
                new TLIuserPermission { Id = 234, Active = true, Deleted = false, permissionId = 234, userId = 434 },
                new TLIuserPermission { Id = 235, Active = true, Deleted = false, permissionId = 235, userId = 434 },
                new TLIuserPermission { Id = 236, Active = true, Deleted = false, permissionId = 236, userId = 434 },
                new TLIuserPermission { Id = 237, Active = true, Deleted = false, permissionId = 237, userId = 434 },
                new TLIuserPermission { Id = 238, Active = true, Deleted = false, permissionId = 238, userId = 434 },
                new TLIuserPermission { Id = 239, Active = true, Deleted = false, permissionId = 239, userId = 434 },

                new TLIuserPermission { Id = 240, Active = true, Deleted = false, permissionId = 240, userId = 434 },
                new TLIuserPermission { Id = 241, Active = true, Deleted = false, permissionId = 241, userId = 434 },
                new TLIuserPermission { Id = 242, Active = true, Deleted = false, permissionId = 242, userId = 434 },
                new TLIuserPermission { Id = 243, Active = true, Deleted = false, permissionId = 243, userId = 434 },
                new TLIuserPermission { Id = 244, Active = true, Deleted = false, permissionId = 244, userId = 434 },
                new TLIuserPermission { Id = 245, Active = true, Deleted = false, permissionId = 245, userId = 434 },
                new TLIuserPermission { Id = 246, Active = true, Deleted = false, permissionId = 246, userId = 434 },
                new TLIuserPermission { Id = 247, Active = true, Deleted = false, permissionId = 247, userId = 434 },
                new TLIuserPermission { Id = 248, Active = true, Deleted = false, permissionId = 248, userId = 434 },
                new TLIuserPermission { Id = 249, Active = true, Deleted = false, permissionId = 249, userId = 434 },

                new TLIuserPermission { Id = 250, Active = true, Deleted = false, permissionId = 250, userId = 434 },
                new TLIuserPermission { Id = 251, Active = true, Deleted = false, permissionId = 251, userId = 434 },
                new TLIuserPermission { Id = 252, Active = true, Deleted = false, permissionId = 252, userId = 434 },
                new TLIuserPermission { Id = 253, Active = true, Deleted = false, permissionId = 253, userId = 434 },
                new TLIuserPermission { Id = 254, Active = true, Deleted = false, permissionId = 254, userId = 434 },
                new TLIuserPermission { Id = 255, Active = true, Deleted = false, permissionId = 255, userId = 434 },
                new TLIuserPermission { Id = 256, Active = true, Deleted = false, permissionId = 256, userId = 434 },
                new TLIuserPermission { Id = 257, Active = true, Deleted = false, permissionId = 257, userId = 434 },
                new TLIuserPermission { Id = 258, Active = true, Deleted = false, permissionId = 258, userId = 434 },
                new TLIuserPermission { Id = 259, Active = true, Deleted = false, permissionId = 259, userId = 434 },

                new TLIuserPermission { Id = 260, Active = true, Deleted = false, permissionId = 260, userId = 434 },
                new TLIuserPermission { Id = 261, Active = true, Deleted = false, permissionId = 261, userId = 434 },
                new TLIuserPermission { Id = 262, Active = true, Deleted = false, permissionId = 262, userId = 434 },
                new TLIuserPermission { Id = 263, Active = true, Deleted = false, permissionId = 263, userId = 434 },
                new TLIuserPermission { Id = 264, Active = true, Deleted = false, permissionId = 264, userId = 434 },
                new TLIuserPermission { Id = 265, Active = true, Deleted = false, permissionId = 265, userId = 434 },
                new TLIuserPermission { Id = 266, Active = true, Deleted = false, permissionId = 266, userId = 434 },
                new TLIuserPermission { Id = 267, Active = true, Deleted = false, permissionId = 267, userId = 434 },
                new TLIuserPermission { Id = 268, Active = true, Deleted = false, permissionId = 268, userId = 434 },
                new TLIuserPermission { Id = 269, Active = true, Deleted = false, permissionId = 269, userId = 434 },

                new TLIuserPermission { Id = 270, Active = true, Deleted = false, permissionId = 270, userId = 434 },
                new TLIuserPermission { Id = 271, Active = true, Deleted = false, permissionId = 271, userId = 434 },
                new TLIuserPermission { Id = 272, Active = true, Deleted = false, permissionId = 272, userId = 434 },
                new TLIuserPermission { Id = 273, Active = true, Deleted = false, permissionId = 273, userId = 434 },
                new TLIuserPermission { Id = 274, Active = true, Deleted = false, permissionId = 274, userId = 434 },
                new TLIuserPermission { Id = 275, Active = true, Deleted = false, permissionId = 275, userId = 434 },
                new TLIuserPermission { Id = 276, Active = true, Deleted = false, permissionId = 276, userId = 434 },
                new TLIuserPermission { Id = 277, Active = true, Deleted = false, permissionId = 277, userId = 434 },
                new TLIuserPermission { Id = 278, Active = true, Deleted = false, permissionId = 278, userId = 434 },
                new TLIuserPermission { Id = 279, Active = true, Deleted = false, permissionId = 279, userId = 434 },

                new TLIuserPermission { Id = 280, Active = true, Deleted = false, permissionId = 280, userId = 434 },
                new TLIuserPermission { Id = 281, Active = true, Deleted = false, permissionId = 281, userId = 434 },
                new TLIuserPermission { Id = 282, Active = true, Deleted = false, permissionId = 282, userId = 434 },
                new TLIuserPermission { Id = 283, Active = true, Deleted = false, permissionId = 283, userId = 434 },
                new TLIuserPermission { Id = 284, Active = true, Deleted = false, permissionId = 284, userId = 434 },
                new TLIuserPermission { Id = 285, Active = true, Deleted = false, permissionId = 285, userId = 434 },
                new TLIuserPermission { Id = 286, Active = true, Deleted = false, permissionId = 286, userId = 434 },
                new TLIuserPermission { Id = 287, Active = true, Deleted = false, permissionId = 287, userId = 434 },
                new TLIuserPermission { Id = 288, Active = true, Deleted = false, permissionId = 288, userId = 434 },
                new TLIuserPermission { Id = 289, Active = true, Deleted = false, permissionId = 289, userId = 434 },

                new TLIuserPermission { Id = 290, Active = true, Deleted = false, permissionId = 290, userId = 434 },
                new TLIuserPermission { Id = 291, Active = true, Deleted = false, permissionId = 291, userId = 434 },
                new TLIuserPermission { Id = 292, Active = true, Deleted = false, permissionId = 292, userId = 434 },
                new TLIuserPermission { Id = 293, Active = true, Deleted = false, permissionId = 293, userId = 434 },
                new TLIuserPermission { Id = 294, Active = true, Deleted = false, permissionId = 294, userId = 434 },
                new TLIuserPermission { Id = 295, Active = true, Deleted = false, permissionId = 295, userId = 434 },
                new TLIuserPermission { Id = 296, Active = true, Deleted = false, permissionId = 296, userId = 434 },
                new TLIuserPermission { Id = 297, Active = true, Deleted = false, permissionId = 297, userId = 434 },
                new TLIuserPermission { Id = 298, Active = true, Deleted = false, permissionId = 298, userId = 434 },
                new TLIuserPermission { Id = 299, Active = true, Deleted = false, permissionId = 299, userId = 434 },

                new TLIuserPermission { Id = 300, Active = true, Deleted = false, permissionId = 300, userId = 434 },
                new TLIuserPermission { Id = 301, Active = true, Deleted = false, permissionId = 301, userId = 434 },
                new TLIuserPermission { Id = 302, Active = true, Deleted = false, permissionId = 302, userId = 434 },
                new TLIuserPermission { Id = 303, Active = true, Deleted = false, permissionId = 303, userId = 434 },
                new TLIuserPermission { Id = 304, Active = true, Deleted = false, permissionId = 304, userId = 434 },
                new TLIuserPermission { Id = 305, Active = true, Deleted = false, permissionId = 305, userId = 434 },
                new TLIuserPermission { Id = 306, Active = true, Deleted = false, permissionId = 306, userId = 434 },
                new TLIuserPermission { Id = 307, Active = true, Deleted = false, permissionId = 307, userId = 434 },
                new TLIuserPermission { Id = 308, Active = true, Deleted = false, permissionId = 308, userId = 434 },
                new TLIuserPermission { Id = 309, Active = true, Deleted = false, permissionId = 309, userId = 434 },

                new TLIuserPermission { Id = 310, Active = true, Deleted = false, permissionId = 310, userId = 434 },
                new TLIuserPermission { Id = 311, Active = true, Deleted = false, permissionId = 311, userId = 434 },
                new TLIuserPermission { Id = 312, Active = true, Deleted = false, permissionId = 312, userId = 434 },
                new TLIuserPermission { Id = 313, Active = true, Deleted = false, permissionId = 313, userId = 434 },
                new TLIuserPermission { Id = 314, Active = true, Deleted = false, permissionId = 314, userId = 434 },
                new TLIuserPermission { Id = 315, Active = true, Deleted = false, permissionId = 315, userId = 434 },
                new TLIuserPermission { Id = 316, Active = true, Deleted = false, permissionId = 316, userId = 434 },
                new TLIuserPermission { Id = 317, Active = true, Deleted = false, permissionId = 317, userId = 434 },
                new TLIuserPermission { Id = 318, Active = true, Deleted = false, permissionId = 318, userId = 434 },
                new TLIuserPermission { Id = 319, Active = true, Deleted = false, permissionId = 319, userId = 434 },

                new TLIuserPermission { Id = 320, Active = true, Deleted = false, permissionId = 320, userId = 434 },
                new TLIuserPermission { Id = 321, Active = true, Deleted = false, permissionId = 321, userId = 434 },
                new TLIuserPermission { Id = 322, Active = true, Deleted = false, permissionId = 322, userId = 434 },
                new TLIuserPermission { Id = 323, Active = true, Deleted = false, permissionId = 323, userId = 434 },
                new TLIuserPermission { Id = 324, Active = true, Deleted = false, permissionId = 324, userId = 434 },
                new TLIuserPermission { Id = 325, Active = true, Deleted = false, permissionId = 325, userId = 434 },
                new TLIuserPermission { Id = 326, Active = true, Deleted = false, permissionId = 326, userId = 434 },
                new TLIuserPermission { Id = 327, Active = true, Deleted = false, permissionId = 327, userId = 434 },
                new TLIuserPermission { Id = 328, Active = true, Deleted = false, permissionId = 328, userId = 434 },
                new TLIuserPermission { Id = 329, Active = true, Deleted = false, permissionId = 329, userId = 434 },

                new TLIuserPermission { Id = 330, Active = true, Deleted = false, permissionId = 330, userId = 434 },
                new TLIuserPermission { Id = 331, Active = true, Deleted = false, permissionId = 331, userId = 434 },
                new TLIuserPermission { Id = 332, Active = true, Deleted = false, permissionId = 332, userId = 434 },
                new TLIuserPermission { Id = 333, Active = true, Deleted = false, permissionId = 333, userId = 434 },
                new TLIuserPermission { Id = 334, Active = true, Deleted = false, permissionId = 334, userId = 434 },
                new TLIuserPermission { Id = 335, Active = true, Deleted = false, permissionId = 335, userId = 434 },
                new TLIuserPermission { Id = 336, Active = true, Deleted = false, permissionId = 336, userId = 434 },
                new TLIuserPermission { Id = 337, Active = true, Deleted = false, permissionId = 337, userId = 434 },
                new TLIuserPermission { Id = 338, Active = true, Deleted = false, permissionId = 338, userId = 434 },
                new TLIuserPermission { Id = 339, Active = true, Deleted = false, permissionId = 339, userId = 434 },

                new TLIuserPermission { Id = 340, Active = true, Deleted = false, permissionId = 340, userId = 434 },
                new TLIuserPermission { Id = 341, Active = true, Deleted = false, permissionId = 341, userId = 434 },
                new TLIuserPermission { Id = 342, Active = true, Deleted = false, permissionId = 342, userId = 434 },
                new TLIuserPermission { Id = 343, Active = true, Deleted = false, permissionId = 343, userId = 434 },
                new TLIuserPermission { Id = 344, Active = true, Deleted = false, permissionId = 344, userId = 434 },
                new TLIuserPermission { Id = 345, Active = true, Deleted = false, permissionId = 345, userId = 434 },
                new TLIuserPermission { Id = 346, Active = true, Deleted = false, permissionId = 346, userId = 434 },
                new TLIuserPermission { Id = 347, Active = true, Deleted = false, permissionId = 347, userId = 434 },
                new TLIuserPermission { Id = 348, Active = true, Deleted = false, permissionId = 348, userId = 434 },
                new TLIuserPermission { Id = 349, Active = true, Deleted = false, permissionId = 349, userId = 434 },

                new TLIuserPermission { Id = 350, Active = true, Deleted = false, permissionId = 350, userId = 434 },
                new TLIuserPermission { Id = 351, Active = true, Deleted = false, permissionId = 351, userId = 434 },
                new TLIuserPermission { Id = 352, Active = true, Deleted = false, permissionId = 352, userId = 434 },
                new TLIuserPermission { Id = 353, Active = true, Deleted = false, permissionId = 353, userId = 434 },
                new TLIuserPermission { Id = 354, Active = true, Deleted = false, permissionId = 354, userId = 434 },
                new TLIuserPermission { Id = 355, Active = true, Deleted = false, permissionId = 355, userId = 434 },
                new TLIuserPermission { Id = 356, Active = true, Deleted = false, permissionId = 356, userId = 434 },
                new TLIuserPermission { Id = 357, Active = true, Deleted = false, permissionId = 357, userId = 434 },
                new TLIuserPermission { Id = 358, Active = true, Deleted = false, permissionId = 358, userId = 434 },
                new TLIuserPermission { Id = 359, Active = true, Deleted = false, permissionId = 359, userId = 434 },

                new TLIuserPermission { Id = 360, Active = true, Deleted = false, permissionId = 360, userId = 434 },
                new TLIuserPermission { Id = 361, Active = true, Deleted = false, permissionId = 361, userId = 434 },
                new TLIuserPermission { Id = 362, Active = true, Deleted = false, permissionId = 362, userId = 434 },
                new TLIuserPermission { Id = 363, Active = true, Deleted = false, permissionId = 363, userId = 434 },
                new TLIuserPermission { Id = 364, Active = true, Deleted = false, permissionId = 364, userId = 434 },
                new TLIuserPermission { Id = 365, Active = true, Deleted = false, permissionId = 365, userId = 434 },
                new TLIuserPermission { Id = 366, Active = true, Deleted = false, permissionId = 366, userId = 434 },
                new TLIuserPermission { Id = 367, Active = true, Deleted = false, permissionId = 367, userId = 434 },
                new TLIuserPermission { Id = 368, Active = true, Deleted = false, permissionId = 368, userId = 434 },
                new TLIuserPermission { Id = 369, Active = true, Deleted = false, permissionId = 369, userId = 434 },

                new TLIuserPermission { Id = 370, Active = true, Deleted = false, permissionId = 370, userId = 434 },
                new TLIuserPermission { Id = 371, Active = true, Deleted = false, permissionId = 371, userId = 434 },
                new TLIuserPermission { Id = 372, Active = true, Deleted = false, permissionId = 372, userId = 434 },
                new TLIuserPermission { Id = 373, Active = true, Deleted = false, permissionId = 373, userId = 434 },
                new TLIuserPermission { Id = 374, Active = true, Deleted = false, permissionId = 374, userId = 434 },
                new TLIuserPermission { Id = 375, Active = true, Deleted = false, permissionId = 375, userId = 434 },
                new TLIuserPermission { Id = 376, Active = true, Deleted = false, permissionId = 376, userId = 434 },
                new TLIuserPermission { Id = 377, Active = true, Deleted = false, permissionId = 377, userId = 434 },
                new TLIuserPermission { Id = 378, Active = true, Deleted = false, permissionId = 378, userId = 434 },
                new TLIuserPermission { Id = 379, Active = true, Deleted = false, permissionId = 379, userId = 434 },

                new TLIuserPermission { Id = 380, Active = true, Deleted = false, permissionId = 380, userId = 434 },
                new TLIuserPermission { Id = 381, Active = true, Deleted = false, permissionId = 381, userId = 434 },
                new TLIuserPermission { Id = 382, Active = true, Deleted = false, permissionId = 382, userId = 434 },
                new TLIuserPermission { Id = 383, Active = true, Deleted = false, permissionId = 383, userId = 434 },
                new TLIuserPermission { Id = 384, Active = true, Deleted = false, permissionId = 384, userId = 434 },
                new TLIuserPermission { Id = 385, Active = true, Deleted = false, permissionId = 385, userId = 434 },
                new TLIuserPermission { Id = 386, Active = true, Deleted = false, permissionId = 386, userId = 434 },
                new TLIuserPermission { Id = 387, Active = true, Deleted = false, permissionId = 387, userId = 434 },
                new TLIuserPermission { Id = 388, Active = true, Deleted = false, permissionId = 388, userId = 434 },
                new TLIuserPermission { Id = 389, Active = true, Deleted = false, permissionId = 389, userId = 434 },

                new TLIuserPermission { Id = 390, Active = true, Deleted = false, permissionId = 390, userId = 434 },
                new TLIuserPermission { Id = 391, Active = true, Deleted = false, permissionId = 391, userId = 434 },
                new TLIuserPermission { Id = 392, Active = true, Deleted = false, permissionId = 392, userId = 434 },
                new TLIuserPermission { Id = 393, Active = true, Deleted = false, permissionId = 393, userId = 434 },
                new TLIuserPermission { Id = 394, Active = true, Deleted = false, permissionId = 394, userId = 434 },
                new TLIuserPermission { Id = 395, Active = true, Deleted = false, permissionId = 395, userId = 434 },
                new TLIuserPermission { Id = 396, Active = true, Deleted = false, permissionId = 396, userId = 434 },
                new TLIuserPermission { Id = 397, Active = true, Deleted = false, permissionId = 397, userId = 434 },
                new TLIuserPermission { Id = 398, Active = true, Deleted = false, permissionId = 398, userId = 434 },
                new TLIuserPermission { Id = 399, Active = true, Deleted = false, permissionId = 399, userId = 434 },

                new TLIuserPermission { Id = 400, Active = true, Deleted = false, permissionId = 400, userId = 434 },
                new TLIuserPermission { Id = 401, Active = true, Deleted = false, permissionId = 401, userId = 434 },
                new TLIuserPermission { Id = 402, Active = true, Deleted = false, permissionId = 402, userId = 434 },
                new TLIuserPermission { Id = 403, Active = true, Deleted = false, permissionId = 403, userId = 434 },
                new TLIuserPermission { Id = 404, Active = true, Deleted = false, permissionId = 404, userId = 434 },
                new TLIuserPermission { Id = 405, Active = true, Deleted = false, permissionId = 405, userId = 434 },
                new TLIuserPermission { Id = 406, Active = true, Deleted = false, permissionId = 406, userId = 434 },
                new TLIuserPermission { Id = 407, Active = true, Deleted = false, permissionId = 407, userId = 434 },
                new TLIuserPermission { Id = 408, Active = true, Deleted = false, permissionId = 408, userId = 434 },
                new TLIuserPermission { Id = 409, Active = true, Deleted = false, permissionId = 409, userId = 434 },

                new TLIuserPermission { Id = 410, Active = true, Deleted = false, permissionId = 410, userId = 434 },
                new TLIuserPermission { Id = 411, Active = true, Deleted = false, permissionId = 411, userId = 434 },
                new TLIuserPermission { Id = 412, Active = true, Deleted = false, permissionId = 412, userId = 434 },
                new TLIuserPermission { Id = 413, Active = true, Deleted = false, permissionId = 413, userId = 434 },
                new TLIuserPermission { Id = 414, Active = true, Deleted = false, permissionId = 414, userId = 434 },
                new TLIuserPermission { Id = 415, Active = true, Deleted = false, permissionId = 415, userId = 434 },
                new TLIuserPermission { Id = 416, Active = true, Deleted = false, permissionId = 416, userId = 434 },
                new TLIuserPermission { Id = 417, Active = true, Deleted = false, permissionId = 417, userId = 434 },
                new TLIuserPermission { Id = 418, Active = true, Deleted = false, permissionId = 418, userId = 434 },
                new TLIuserPermission { Id = 419, Active = true, Deleted = false, permissionId = 419, userId = 434 },

                new TLIuserPermission { Id = 420, Active = true, Deleted = false, permissionId = 420, userId = 434 },
                new TLIuserPermission { Id = 421, Active = true, Deleted = false, permissionId = 421, userId = 434 },
                new TLIuserPermission { Id = 422, Active = true, Deleted = false, permissionId = 422, userId = 434 },
                new TLIuserPermission { Id = 423, Active = true, Deleted = false, permissionId = 423, userId = 434 },
                new TLIuserPermission { Id = 424, Active = true, Deleted = false, permissionId = 424, userId = 434 },
                new TLIuserPermission { Id = 425, Active = true, Deleted = false, permissionId = 425, userId = 434 },
                new TLIuserPermission { Id = 426, Active = true, Deleted = false, permissionId = 426, userId = 434 },
                new TLIuserPermission { Id = 427, Active = true, Deleted = false, permissionId = 427, userId = 434 },
                new TLIuserPermission { Id = 428, Active = true, Deleted = false, permissionId = 428, userId = 434 },
                new TLIuserPermission { Id = 429, Active = true, Deleted = false, permissionId = 429, userId = 434 },

                new TLIuserPermission { Id = 430, Active = true, Deleted = false, permissionId = 430, userId = 434 },
                new TLIuserPermission { Id = 431, Active = true, Deleted = false, permissionId = 431, userId = 434 },
                new TLIuserPermission { Id = 432, Active = true, Deleted = false, permissionId = 432, userId = 434 },
                new TLIuserPermission { Id = 433, Active = true, Deleted = false, permissionId = 433, userId = 434 },
                new TLIuserPermission { Id = 434, Active = true, Deleted = false, permissionId = 434, userId = 434 },
                new TLIuserPermission { Id = 435, Active = true, Deleted = false, permissionId = 435, userId = 434 },
                new TLIuserPermission { Id = 436, Active = true, Deleted = false, permissionId = 436, userId = 434 },
                new TLIuserPermission { Id = 437, Active = true, Deleted = false, permissionId = 437, userId = 434 },
                new TLIuserPermission { Id = 438, Active = true, Deleted = false, permissionId = 438, userId = 434 },
                new TLIuserPermission { Id = 439, Active = true, Deleted = false, permissionId = 439, userId = 434 },

                new TLIuserPermission { Id = 440, Active = true, Deleted = false, permissionId = 440, userId = 434 },
                new TLIuserPermission { Id = 441, Active = true, Deleted = false, permissionId = 441, userId = 434 },
                new TLIuserPermission { Id = 442, Active = true, Deleted = false, permissionId = 442, userId = 434 },
                new TLIuserPermission { Id = 443, Active = true, Deleted = false, permissionId = 443, userId = 434 },
                new TLIuserPermission { Id = 444, Active = true, Deleted = false, permissionId = 444, userId = 434 },
                new TLIuserPermission { Id = 445, Active = true, Deleted = false, permissionId = 445, userId = 434 },
                new TLIuserPermission { Id = 446, Active = true, Deleted = false, permissionId = 446, userId = 434 },
                new TLIuserPermission { Id = 447, Active = true, Deleted = false, permissionId = 447, userId = 434 },
                new TLIuserPermission { Id = 448, Active = true, Deleted = false, permissionId = 448, userId = 434 },
                new TLIuserPermission { Id = 449, Active = true, Deleted = false, permissionId = 449, userId = 434 },

                new TLIuserPermission { Id = 450, Active = true, Deleted = false, permissionId = 450, userId = 434 },
                new TLIuserPermission { Id = 451, Active = true, Deleted = false, permissionId = 451, userId = 434 },
                new TLIuserPermission { Id = 452, Active = true, Deleted = false, permissionId = 452, userId = 434 },
                new TLIuserPermission { Id = 453, Active = true, Deleted = false, permissionId = 453, userId = 434 },
                new TLIuserPermission { Id = 454, Active = true, Deleted = false, permissionId = 454, userId = 434 },
                new TLIuserPermission { Id = 455, Active = true, Deleted = false, permissionId = 455, userId = 434 },
                new TLIuserPermission { Id = 456, Active = true, Deleted = false, permissionId = 456, userId = 434 },
                new TLIuserPermission { Id = 457, Active = true, Deleted = false, permissionId = 457, userId = 434 },
                new TLIuserPermission { Id = 458, Active = true, Deleted = false, permissionId = 458, userId = 434 },
                new TLIuserPermission { Id = 459, Active = true, Deleted = false, permissionId = 459, userId = 434 },

                new TLIuserPermission { Id = 460, Active = true, Deleted = false, permissionId = 460, userId = 434 },
                new TLIuserPermission { Id = 461, Active = true, Deleted = false, permissionId = 461, userId = 434 },
                new TLIuserPermission { Id = 462, Active = true, Deleted = false, permissionId = 462, userId = 434 },
                new TLIuserPermission { Id = 463, Active = true, Deleted = false, permissionId = 463, userId = 434 },
                new TLIuserPermission { Id = 464, Active = true, Deleted = false, permissionId = 464, userId = 434 },
                new TLIuserPermission { Id = 465, Active = true, Deleted = false, permissionId = 465, userId = 434 },
                new TLIuserPermission { Id = 466, Active = true, Deleted = false, permissionId = 466, userId = 434 },
                new TLIuserPermission { Id = 467, Active = true, Deleted = false, permissionId = 467, userId = 434 },

                new TLIuserPermission { Id = 468, Active = true, Deleted = false, permissionId = 1, userId = 435 },
                new TLIuserPermission { Id = 469, Active = true, Deleted = false, permissionId = 2, userId = 435 },
                new TLIuserPermission { Id = 470, Active = true, Deleted = false, permissionId = 3, userId = 435 },
                new TLIuserPermission { Id = 471, Active = true, Deleted = false, permissionId = 4, userId = 435 },
                new TLIuserPermission { Id = 472, Active = true, Deleted = false, permissionId = 5, userId = 435 },
                new TLIuserPermission { Id = 473, Active = true, Deleted = false, permissionId = 6, userId = 435 },
                new TLIuserPermission { Id = 474, Active = true, Deleted = false, permissionId = 7, userId = 435 },
                new TLIuserPermission { Id = 475, Active = true, Deleted = false, permissionId = 8, userId = 435 },
                new TLIuserPermission { Id = 476, Active = true, Deleted = false, permissionId = 9, userId = 435 },
                new TLIuserPermission { Id = 478, Active = true, Deleted = false, permissionId = 10, userId = 435 },
                new TLIuserPermission { Id = 479, Active = true, Deleted = false, permissionId = 11, userId = 435 },

                new TLIuserPermission { Id = 480, Active = true, Deleted = false, permissionId = 12, userId = 435 },
                new TLIuserPermission { Id = 481, Active = true, Deleted = false, permissionId = 13, userId = 435 },
                new TLIuserPermission { Id = 482, Active = true, Deleted = false, permissionId = 14, userId = 435 },
                new TLIuserPermission { Id = 483, Active = true, Deleted = false, permissionId = 15, userId = 435 },
                new TLIuserPermission { Id = 484, Active = true, Deleted = false, permissionId = 16, userId = 435 },
                new TLIuserPermission { Id = 485, Active = true, Deleted = false, permissionId = 17, userId = 435 },
                new TLIuserPermission { Id = 486, Active = true, Deleted = false, permissionId = 18, userId = 435 },
                new TLIuserPermission { Id = 487, Active = true, Deleted = false, permissionId = 19, userId = 435 },
                new TLIuserPermission { Id = 488, Active = true, Deleted = false, permissionId = 20, userId = 435 },
                new TLIuserPermission { Id = 489, Active = true, Deleted = false, permissionId = 21, userId = 435 },

                new TLIuserPermission { Id = 490, Active = true, Deleted = false, permissionId = 22, userId = 435 },
                new TLIuserPermission { Id = 491, Active = true, Deleted = false, permissionId = 23, userId = 435 },
                new TLIuserPermission { Id = 492, Active = true, Deleted = false, permissionId = 24, userId = 435 },
                new TLIuserPermission { Id = 493, Active = true, Deleted = false, permissionId = 25, userId = 435 },
                new TLIuserPermission { Id = 494, Active = true, Deleted = false, permissionId = 26, userId = 435 },
                new TLIuserPermission { Id = 495, Active = true, Deleted = false, permissionId = 27, userId = 435 },
                new TLIuserPermission { Id = 496, Active = true, Deleted = false, permissionId = 28, userId = 435 },
                new TLIuserPermission { Id = 497, Active = true, Deleted = false, permissionId = 29, userId = 435 },

                new TLIuserPermission { Id = 498, Active = true, Deleted = false, permissionId = 30, userId = 435 },
                new TLIuserPermission { Id = 499, Active = true, Deleted = false, permissionId = 31, userId = 435 },
                new TLIuserPermission { Id = 500, Active = true, Deleted = false, permissionId = 32, userId = 435 },
                new TLIuserPermission { Id = 501, Active = true, Deleted = false, permissionId = 33, userId = 435 },
                new TLIuserPermission { Id = 502, Active = true, Deleted = false, permissionId = 34, userId = 435 },
                new TLIuserPermission { Id = 503, Active = true, Deleted = false, permissionId = 35, userId = 435 },
                new TLIuserPermission { Id = 504, Active = true, Deleted = false, permissionId = 36, userId = 435 },
                new TLIuserPermission { Id = 505, Active = true, Deleted = false, permissionId = 37, userId = 435 },
                new TLIuserPermission { Id = 506, Active = true, Deleted = false, permissionId = 38, userId = 435 },
                new TLIuserPermission { Id = 507, Active = true, Deleted = false, permissionId = 39, userId = 435 },

                new TLIuserPermission { Id = 508, Active = true, Deleted = false, permissionId = 40, userId = 435 },
                new TLIuserPermission { Id = 509, Active = true, Deleted = false, permissionId = 41, userId = 435 },
                new TLIuserPermission { Id = 510, Active = true, Deleted = false, permissionId = 42, userId = 435 },
                new TLIuserPermission { Id = 511, Active = true, Deleted = false, permissionId = 43, userId = 435 },
                new TLIuserPermission { Id = 512, Active = true, Deleted = false, permissionId = 44, userId = 435 },
                new TLIuserPermission { Id = 513, Active = true, Deleted = false, permissionId = 45, userId = 435 },
                new TLIuserPermission { Id = 514, Active = true, Deleted = false, permissionId = 46, userId = 435 },
                new TLIuserPermission { Id = 515, Active = true, Deleted = false, permissionId = 47, userId = 435 },
                new TLIuserPermission { Id = 516, Active = true, Deleted = false, permissionId = 48, userId = 435 },
                new TLIuserPermission { Id = 517, Active = true, Deleted = false, permissionId = 49, userId = 435 },

                new TLIuserPermission { Id = 518, Active = true, Deleted = false, permissionId = 50, userId = 435 },
                new TLIuserPermission { Id = 519, Active = true, Deleted = false, permissionId = 51, userId = 435 },
                new TLIuserPermission { Id = 520, Active = true, Deleted = false, permissionId = 52, userId = 435 },
                new TLIuserPermission { Id = 521, Active = true, Deleted = false, permissionId = 53, userId = 435 },
                new TLIuserPermission { Id = 522, Active = true, Deleted = false, permissionId = 54, userId = 435 },
                new TLIuserPermission { Id = 523, Active = true, Deleted = false, permissionId = 55, userId = 435 },
                new TLIuserPermission { Id = 524, Active = true, Deleted = false, permissionId = 56, userId = 435 },
                new TLIuserPermission { Id = 525, Active = true, Deleted = false, permissionId = 57, userId = 435 },
                new TLIuserPermission { Id = 526, Active = true, Deleted = false, permissionId = 58, userId = 435 },
                new TLIuserPermission { Id = 527, Active = true, Deleted = false, permissionId = 59, userId = 435 },

                new TLIuserPermission { Id = 528, Active = true, Deleted = false, permissionId = 60, userId = 435 },
                new TLIuserPermission { Id = 529, Active = true, Deleted = false, permissionId = 61, userId = 435 },
                new TLIuserPermission { Id = 530, Active = true, Deleted = false, permissionId = 62, userId = 435 },
                new TLIuserPermission { Id = 531, Active = true, Deleted = false, permissionId = 63, userId = 435 },
                new TLIuserPermission { Id = 532, Active = true, Deleted = false, permissionId = 64, userId = 435 },
                new TLIuserPermission { Id = 533, Active = true, Deleted = false, permissionId = 65, userId = 435 },
                new TLIuserPermission { Id = 534, Active = true, Deleted = false, permissionId = 66, userId = 435 },
                new TLIuserPermission { Id = 535, Active = true, Deleted = false, permissionId = 67, userId = 435 },
                new TLIuserPermission { Id = 536, Active = true, Deleted = false, permissionId = 68, userId = 435 },
                new TLIuserPermission { Id = 537, Active = true, Deleted = false, permissionId = 69, userId = 435 },

                new TLIuserPermission { Id = 538, Active = true, Deleted = false, permissionId = 70, userId = 435 },
                new TLIuserPermission { Id = 539, Active = true, Deleted = false, permissionId = 71, userId = 435 },
                new TLIuserPermission { Id = 540, Active = true, Deleted = false, permissionId = 72, userId = 435 },
                new TLIuserPermission { Id = 541, Active = true, Deleted = false, permissionId = 73, userId = 435 },
                new TLIuserPermission { Id = 542, Active = true, Deleted = false, permissionId = 74, userId = 435 },
                new TLIuserPermission { Id = 543, Active = true, Deleted = false, permissionId = 75, userId = 435 },
                new TLIuserPermission { Id = 544, Active = true, Deleted = false, permissionId = 76, userId = 435 },
                new TLIuserPermission { Id = 545, Active = true, Deleted = false, permissionId = 77, userId = 435 },
                new TLIuserPermission { Id = 546, Active = true, Deleted = false, permissionId = 78, userId = 435 },
                new TLIuserPermission { Id = 547, Active = true, Deleted = false, permissionId = 79, userId = 435 },

                new TLIuserPermission { Id = 548, Active = true, Deleted = false, permissionId = 80, userId = 435 },
                new TLIuserPermission { Id = 549, Active = true, Deleted = false, permissionId = 81, userId = 435 },
                new TLIuserPermission { Id = 550, Active = true, Deleted = false, permissionId = 82, userId = 435 },
                new TLIuserPermission { Id = 551, Active = true, Deleted = false, permissionId = 83, userId = 435 },
                new TLIuserPermission { Id = 552, Active = true, Deleted = false, permissionId = 84, userId = 435 },
                new TLIuserPermission { Id = 553, Active = true, Deleted = false, permissionId = 85, userId = 435 },
                new TLIuserPermission { Id = 554, Active = true, Deleted = false, permissionId = 86, userId = 435 },
                new TLIuserPermission { Id = 555, Active = true, Deleted = false, permissionId = 87, userId = 435 },
                new TLIuserPermission { Id = 556, Active = true, Deleted = false, permissionId = 88, userId = 435 },
                new TLIuserPermission { Id = 557, Active = true, Deleted = false, permissionId = 89, userId = 435 },

                new TLIuserPermission { Id = 558, Active = true, Deleted = false, permissionId = 90, userId = 435 },
                new TLIuserPermission { Id = 559, Active = true, Deleted = false, permissionId = 91, userId = 435 },
                new TLIuserPermission { Id = 560, Active = true, Deleted = false, permissionId = 92, userId = 435 },
                new TLIuserPermission { Id = 561, Active = true, Deleted = false, permissionId = 93, userId = 435 },
                new TLIuserPermission { Id = 562, Active = true, Deleted = false, permissionId = 94, userId = 435 },
                new TLIuserPermission { Id = 563, Active = true, Deleted = false, permissionId = 95, userId = 435 },
                new TLIuserPermission { Id = 564, Active = true, Deleted = false, permissionId = 96, userId = 435 },
                new TLIuserPermission { Id = 565, Active = true, Deleted = false, permissionId = 97, userId = 435 },
                new TLIuserPermission { Id = 566, Active = true, Deleted = false, permissionId = 98, userId = 435 },
                new TLIuserPermission { Id = 567, Active = true, Deleted = false, permissionId = 99, userId = 435 },

                new TLIuserPermission { Id = 568, Active = true, Deleted = false, permissionId = 100, userId = 435 },
                new TLIuserPermission { Id = 569, Active = true, Deleted = false, permissionId = 101, userId = 435 },
                new TLIuserPermission { Id = 570, Active = true, Deleted = false, permissionId = 102, userId = 435 },
                new TLIuserPermission { Id = 571, Active = true, Deleted = false, permissionId = 103, userId = 435 },
                new TLIuserPermission { Id = 572, Active = true, Deleted = false, permissionId = 104, userId = 435 },
                new TLIuserPermission { Id = 573, Active = true, Deleted = false, permissionId = 105, userId = 435 },
                new TLIuserPermission { Id = 574, Active = true, Deleted = false, permissionId = 106, userId = 435 },
                new TLIuserPermission { Id = 575, Active = true, Deleted = false, permissionId = 107, userId = 435 },
                new TLIuserPermission { Id = 576, Active = true, Deleted = false, permissionId = 108, userId = 435 },
                new TLIuserPermission { Id = 577, Active = true, Deleted = false, permissionId = 109, userId = 435 },

                new TLIuserPermission { Id = 578, Active = true, Deleted = false, permissionId = 110, userId = 435 },
                new TLIuserPermission { Id = 579, Active = true, Deleted = false, permissionId = 111, userId = 435 },
                new TLIuserPermission { Id = 580, Active = true, Deleted = false, permissionId = 112, userId = 435 },
                new TLIuserPermission { Id = 581, Active = true, Deleted = false, permissionId = 113, userId = 435 },
                new TLIuserPermission { Id = 582, Active = true, Deleted = false, permissionId = 114, userId = 435 },
                new TLIuserPermission { Id = 583, Active = true, Deleted = false, permissionId = 115, userId = 435 },
                new TLIuserPermission { Id = 584, Active = true, Deleted = false, permissionId = 116, userId = 435 },
                new TLIuserPermission { Id = 585, Active = true, Deleted = false, permissionId = 117, userId = 435 },
                new TLIuserPermission { Id = 586, Active = true, Deleted = false, permissionId = 118, userId = 435 },
                new TLIuserPermission { Id = 587, Active = true, Deleted = false, permissionId = 119, userId = 435 },

                new TLIuserPermission { Id = 588, Active = true, Deleted = false, permissionId = 120, userId = 435 },
                new TLIuserPermission { Id = 589, Active = true, Deleted = false, permissionId = 121, userId = 435 },
                new TLIuserPermission { Id = 590, Active = true, Deleted = false, permissionId = 122, userId = 435 },
                new TLIuserPermission { Id = 591, Active = true, Deleted = false, permissionId = 123, userId = 435 },
                new TLIuserPermission { Id = 592, Active = true, Deleted = false, permissionId = 124, userId = 435 },
                new TLIuserPermission { Id = 593, Active = true, Deleted = false, permissionId = 125, userId = 435 },
                new TLIuserPermission { Id = 594, Active = true, Deleted = false, permissionId = 126, userId = 435 },
                new TLIuserPermission { Id = 595, Active = true, Deleted = false, permissionId = 127, userId = 435 },
                new TLIuserPermission { Id = 596, Active = true, Deleted = false, permissionId = 128, userId = 435 },
                new TLIuserPermission { Id = 597, Active = true, Deleted = false, permissionId = 129, userId = 435 },

                new TLIuserPermission { Id = 598, Active = true, Deleted = false, permissionId = 130, userId = 435 },
                new TLIuserPermission { Id = 599, Active = true, Deleted = false, permissionId = 131, userId = 435 },
                new TLIuserPermission { Id = 600, Active = true, Deleted = false, permissionId = 132, userId = 435 },
                new TLIuserPermission { Id = 601, Active = true, Deleted = false, permissionId = 133, userId = 435 },
                new TLIuserPermission { Id = 602, Active = true, Deleted = false, permissionId = 134, userId = 435 },
                new TLIuserPermission { Id = 603, Active = true, Deleted = false, permissionId = 135, userId = 435 },
                new TLIuserPermission { Id = 604, Active = true, Deleted = false, permissionId = 136, userId = 435 },
                new TLIuserPermission { Id = 605, Active = true, Deleted = false, permissionId = 137, userId = 435 },
                new TLIuserPermission { Id = 606, Active = true, Deleted = false, permissionId = 138, userId = 435 },
                new TLIuserPermission { Id = 607, Active = true, Deleted = false, permissionId = 139, userId = 435 },

                new TLIuserPermission { Id = 608, Active = true, Deleted = false, permissionId = 140, userId = 435 },
                new TLIuserPermission { Id = 609, Active = true, Deleted = false, permissionId = 141, userId = 435 },
                new TLIuserPermission { Id = 610, Active = true, Deleted = false, permissionId = 142, userId = 435 },
                new TLIuserPermission { Id = 611, Active = true, Deleted = false, permissionId = 143, userId = 435 },
                new TLIuserPermission { Id = 612, Active = true, Deleted = false, permissionId = 144, userId = 435 },
                new TLIuserPermission { Id = 613, Active = true, Deleted = false, permissionId = 145, userId = 435 },
                new TLIuserPermission { Id = 614, Active = true, Deleted = false, permissionId = 146, userId = 435 },
                new TLIuserPermission { Id = 615, Active = true, Deleted = false, permissionId = 147, userId = 435 },
                new TLIuserPermission { Id = 616, Active = true, Deleted = false, permissionId = 148, userId = 435 },
                new TLIuserPermission { Id = 617, Active = true, Deleted = false, permissionId = 149, userId = 435 },

                new TLIuserPermission { Id = 618, Active = true, Deleted = false, permissionId = 150, userId = 435 },
                new TLIuserPermission { Id = 619, Active = true, Deleted = false, permissionId = 151, userId = 435 },
                new TLIuserPermission { Id = 620, Active = true, Deleted = false, permissionId = 152, userId = 435 },
                new TLIuserPermission { Id = 621, Active = true, Deleted = false, permissionId = 153, userId = 435 },
                new TLIuserPermission { Id = 622, Active = true, Deleted = false, permissionId = 154, userId = 435 },
                new TLIuserPermission { Id = 623, Active = true, Deleted = false, permissionId = 155, userId = 435 },
                new TLIuserPermission { Id = 624, Active = true, Deleted = false, permissionId = 156, userId = 435 },
                new TLIuserPermission { Id = 625, Active = true, Deleted = false, permissionId = 157, userId = 435 },
                new TLIuserPermission { Id = 626, Active = true, Deleted = false, permissionId = 158, userId = 435 },
                new TLIuserPermission { Id = 627, Active = true, Deleted = false, permissionId = 159, userId = 435 },

                new TLIuserPermission { Id = 628, Active = true, Deleted = false, permissionId = 160, userId = 435 },
                new TLIuserPermission { Id = 629, Active = true, Deleted = false, permissionId = 161, userId = 435 },
                new TLIuserPermission { Id = 630, Active = true, Deleted = false, permissionId = 162, userId = 435 },
                new TLIuserPermission { Id = 631, Active = true, Deleted = false, permissionId = 163, userId = 435 },
                new TLIuserPermission { Id = 632, Active = true, Deleted = false, permissionId = 164, userId = 435 },
                new TLIuserPermission { Id = 633, Active = true, Deleted = false, permissionId = 165, userId = 435 },
                new TLIuserPermission { Id = 634, Active = true, Deleted = false, permissionId = 166, userId = 435 },
                new TLIuserPermission { Id = 635, Active = true, Deleted = false, permissionId = 167, userId = 435 },
                new TLIuserPermission { Id = 636, Active = true, Deleted = false, permissionId = 168, userId = 435 },
                new TLIuserPermission { Id = 637, Active = true, Deleted = false, permissionId = 169, userId = 435 },

                new TLIuserPermission { Id = 638, Active = true, Deleted = false, permissionId = 170, userId = 435 },
                new TLIuserPermission { Id = 639, Active = true, Deleted = false, permissionId = 171, userId = 435 },
                new TLIuserPermission { Id = 640, Active = true, Deleted = false, permissionId = 172, userId = 435 },
                new TLIuserPermission { Id = 641, Active = true, Deleted = false, permissionId = 173, userId = 435 },
                new TLIuserPermission { Id = 642, Active = true, Deleted = false, permissionId = 174, userId = 435 },
                new TLIuserPermission { Id = 643, Active = true, Deleted = false, permissionId = 175, userId = 435 },
                new TLIuserPermission { Id = 644, Active = true, Deleted = false, permissionId = 176, userId = 435 },
                new TLIuserPermission { Id = 645, Active = true, Deleted = false, permissionId = 177, userId = 435 },
                new TLIuserPermission { Id = 646, Active = true, Deleted = false, permissionId = 178, userId = 435 },
                new TLIuserPermission { Id = 647, Active = true, Deleted = false, permissionId = 179, userId = 435 },

                new TLIuserPermission { Id = 648, Active = true, Deleted = false, permissionId = 180, userId = 435 },
                new TLIuserPermission { Id = 649, Active = true, Deleted = false, permissionId = 181, userId = 435 },
                new TLIuserPermission { Id = 650, Active = true, Deleted = false, permissionId = 182, userId = 435 },
                new TLIuserPermission { Id = 651, Active = true, Deleted = false, permissionId = 183, userId = 435 },
                new TLIuserPermission { Id = 652, Active = true, Deleted = false, permissionId = 184, userId = 435 },
                new TLIuserPermission { Id = 653, Active = true, Deleted = false, permissionId = 185, userId = 435 },
                new TLIuserPermission { Id = 654, Active = true, Deleted = false, permissionId = 186, userId = 435 },
                new TLIuserPermission { Id = 655, Active = true, Deleted = false, permissionId = 187, userId = 435 },
                new TLIuserPermission { Id = 656, Active = true, Deleted = false, permissionId = 188, userId = 435 },
                new TLIuserPermission { Id = 657, Active = true, Deleted = false, permissionId = 189, userId = 435 },

                new TLIuserPermission { Id = 658, Active = true, Deleted = false, permissionId = 190, userId = 435 },
                new TLIuserPermission { Id = 659, Active = true, Deleted = false, permissionId = 191, userId = 435 },
                new TLIuserPermission { Id = 660, Active = true, Deleted = false, permissionId = 192, userId = 435 },
                new TLIuserPermission { Id = 661, Active = true, Deleted = false, permissionId = 193, userId = 435 },
                new TLIuserPermission { Id = 662, Active = true, Deleted = false, permissionId = 194, userId = 435 },
                new TLIuserPermission { Id = 663, Active = true, Deleted = false, permissionId = 195, userId = 435 },
                new TLIuserPermission { Id = 664, Active = true, Deleted = false, permissionId = 196, userId = 435 },
                new TLIuserPermission { Id = 665, Active = true, Deleted = false, permissionId = 197, userId = 435 },
                new TLIuserPermission { Id = 666, Active = true, Deleted = false, permissionId = 198, userId = 435 },
                new TLIuserPermission { Id = 667, Active = true, Deleted = false, permissionId = 199, userId = 435 },

                new TLIuserPermission { Id = 668, Active = true, Deleted = false, permissionId = 200, userId = 435 },
                new TLIuserPermission { Id = 669, Active = true, Deleted = false, permissionId = 201, userId = 435 },
                new TLIuserPermission { Id = 670, Active = true, Deleted = false, permissionId = 202, userId = 435 },
                new TLIuserPermission { Id = 671, Active = true, Deleted = false, permissionId = 203, userId = 435 },
                new TLIuserPermission { Id = 672, Active = true, Deleted = false, permissionId = 204, userId = 435 },
                new TLIuserPermission { Id = 673, Active = true, Deleted = false, permissionId = 205, userId = 435 },
                new TLIuserPermission { Id = 674, Active = true, Deleted = false, permissionId = 206, userId = 435 },
                new TLIuserPermission { Id = 675, Active = true, Deleted = false, permissionId = 207, userId = 435 },
                new TLIuserPermission { Id = 676, Active = true, Deleted = false, permissionId = 208, userId = 435 },
                new TLIuserPermission { Id = 677, Active = true, Deleted = false, permissionId = 209, userId = 435 },

                new TLIuserPermission { Id = 678, Active = true, Deleted = false, permissionId = 210, userId = 435 },
                new TLIuserPermission { Id = 679, Active = true, Deleted = false, permissionId = 211, userId = 435 },
                new TLIuserPermission { Id = 680, Active = true, Deleted = false, permissionId = 212, userId = 435 },
                new TLIuserPermission { Id = 681, Active = true, Deleted = false, permissionId = 213, userId = 435 },
                new TLIuserPermission { Id = 682, Active = true, Deleted = false, permissionId = 214, userId = 435 },
                new TLIuserPermission { Id = 683, Active = true, Deleted = false, permissionId = 215, userId = 435 },
                new TLIuserPermission { Id = 684, Active = true, Deleted = false, permissionId = 216, userId = 435 },
                new TLIuserPermission { Id = 685, Active = true, Deleted = false, permissionId = 217, userId = 435 },
                new TLIuserPermission { Id = 686, Active = true, Deleted = false, permissionId = 218, userId = 435 },
                new TLIuserPermission { Id = 687, Active = true, Deleted = false, permissionId = 219, userId = 435 },

                new TLIuserPermission { Id = 688, Active = true, Deleted = false, permissionId = 220, userId = 435 },
                new TLIuserPermission { Id = 689, Active = true, Deleted = false, permissionId = 221, userId = 435 },
                new TLIuserPermission { Id = 690, Active = true, Deleted = false, permissionId = 222, userId = 435 },
                new TLIuserPermission { Id = 691, Active = true, Deleted = false, permissionId = 223, userId = 435 },
                new TLIuserPermission { Id = 692, Active = true, Deleted = false, permissionId = 224, userId = 435 },
                new TLIuserPermission { Id = 693, Active = true, Deleted = false, permissionId = 225, userId = 435 },
                new TLIuserPermission { Id = 694, Active = true, Deleted = false, permissionId = 226, userId = 435 },
                new TLIuserPermission { Id = 695, Active = true, Deleted = false, permissionId = 227, userId = 435 },
                new TLIuserPermission { Id = 696, Active = true, Deleted = false, permissionId = 228, userId = 435 },
                new TLIuserPermission { Id = 697, Active = true, Deleted = false, permissionId = 229, userId = 435 },

                new TLIuserPermission { Id = 698, Active = true, Deleted = false, permissionId = 230, userId = 435 },
                new TLIuserPermission { Id = 699, Active = true, Deleted = false, permissionId = 231, userId = 435 },
                new TLIuserPermission { Id = 700, Active = true, Deleted = false, permissionId = 232, userId = 435 },
                new TLIuserPermission { Id = 701, Active = true, Deleted = false, permissionId = 233, userId = 435 },
                new TLIuserPermission { Id = 702, Active = true, Deleted = false, permissionId = 234, userId = 435 },
                new TLIuserPermission { Id = 703, Active = true, Deleted = false, permissionId = 235, userId = 435 },
                new TLIuserPermission { Id = 704, Active = true, Deleted = false, permissionId = 236, userId = 435 },
                new TLIuserPermission { Id = 705, Active = true, Deleted = false, permissionId = 237, userId = 435 },
                new TLIuserPermission { Id = 706, Active = true, Deleted = false, permissionId = 238, userId = 435 },
                new TLIuserPermission { Id = 707, Active = true, Deleted = false, permissionId = 239, userId = 435 },

                new TLIuserPermission { Id = 708, Active = true, Deleted = false, permissionId = 240, userId = 435 },
                new TLIuserPermission { Id = 709, Active = true, Deleted = false, permissionId = 241, userId = 435 },
                new TLIuserPermission { Id = 710, Active = true, Deleted = false, permissionId = 242, userId = 435 },
                new TLIuserPermission { Id = 711, Active = true, Deleted = false, permissionId = 243, userId = 435 },
                new TLIuserPermission { Id = 712, Active = true, Deleted = false, permissionId = 244, userId = 435 },
                new TLIuserPermission { Id = 713, Active = true, Deleted = false, permissionId = 245, userId = 435 },
                new TLIuserPermission { Id = 714, Active = true, Deleted = false, permissionId = 246, userId = 435 },
                new TLIuserPermission { Id = 715, Active = true, Deleted = false, permissionId = 247, userId = 435 },
                new TLIuserPermission { Id = 716, Active = true, Deleted = false, permissionId = 248, userId = 435 },
                new TLIuserPermission { Id = 717, Active = true, Deleted = false, permissionId = 249, userId = 435 },

                new TLIuserPermission { Id = 718, Active = true, Deleted = false, permissionId = 250, userId = 435 },
                new TLIuserPermission { Id = 719, Active = true, Deleted = false, permissionId = 251, userId = 435 },
                new TLIuserPermission { Id = 720, Active = true, Deleted = false, permissionId = 252, userId = 435 },
                new TLIuserPermission { Id = 721, Active = true, Deleted = false, permissionId = 253, userId = 435 },
                new TLIuserPermission { Id = 722, Active = true, Deleted = false, permissionId = 254, userId = 435 },
                new TLIuserPermission { Id = 723, Active = true, Deleted = false, permissionId = 255, userId = 435 },
                new TLIuserPermission { Id = 724, Active = true, Deleted = false, permissionId = 256, userId = 435 },
                new TLIuserPermission { Id = 725, Active = true, Deleted = false, permissionId = 257, userId = 435 },
                new TLIuserPermission { Id = 726, Active = true, Deleted = false, permissionId = 258, userId = 435 },
                new TLIuserPermission { Id = 727, Active = true, Deleted = false, permissionId = 259, userId = 435 },

                new TLIuserPermission { Id = 728, Active = true, Deleted = false, permissionId = 260, userId = 435 },
                new TLIuserPermission { Id = 729, Active = true, Deleted = false, permissionId = 261, userId = 435 },
                new TLIuserPermission { Id = 730, Active = true, Deleted = false, permissionId = 262, userId = 435 },
                new TLIuserPermission { Id = 731, Active = true, Deleted = false, permissionId = 263, userId = 435 },
                new TLIuserPermission { Id = 732, Active = true, Deleted = false, permissionId = 264, userId = 435 },
                new TLIuserPermission { Id = 733, Active = true, Deleted = false, permissionId = 265, userId = 435 },
                new TLIuserPermission { Id = 734, Active = true, Deleted = false, permissionId = 266, userId = 435 },
                new TLIuserPermission { Id = 735, Active = true, Deleted = false, permissionId = 267, userId = 435 },
                new TLIuserPermission { Id = 736, Active = true, Deleted = false, permissionId = 268, userId = 435 },
                new TLIuserPermission { Id = 737, Active = true, Deleted = false, permissionId = 269, userId = 435 },

                new TLIuserPermission { Id = 738, Active = true, Deleted = false, permissionId = 270, userId = 435 },
                new TLIuserPermission { Id = 739, Active = true, Deleted = false, permissionId = 271, userId = 435 },
                new TLIuserPermission { Id = 740, Active = true, Deleted = false, permissionId = 272, userId = 435 },
                new TLIuserPermission { Id = 741, Active = true, Deleted = false, permissionId = 273, userId = 435 },
                new TLIuserPermission { Id = 742, Active = true, Deleted = false, permissionId = 274, userId = 435 },
                new TLIuserPermission { Id = 743, Active = true, Deleted = false, permissionId = 275, userId = 435 },
                new TLIuserPermission { Id = 744, Active = true, Deleted = false, permissionId = 276, userId = 435 },
                new TLIuserPermission { Id = 745, Active = true, Deleted = false, permissionId = 277, userId = 435 },
                new TLIuserPermission { Id = 746, Active = true, Deleted = false, permissionId = 278, userId = 435 },
                new TLIuserPermission { Id = 747, Active = true, Deleted = false, permissionId = 279, userId = 435 },

                new TLIuserPermission { Id = 748, Active = true, Deleted = false, permissionId = 280, userId = 435 },
                new TLIuserPermission { Id = 749, Active = true, Deleted = false, permissionId = 281, userId = 435 },
                new TLIuserPermission { Id = 750, Active = true, Deleted = false, permissionId = 282, userId = 435 },
                new TLIuserPermission { Id = 751, Active = true, Deleted = false, permissionId = 283, userId = 435 },
                new TLIuserPermission { Id = 752, Active = true, Deleted = false, permissionId = 284, userId = 435 },
                new TLIuserPermission { Id = 753, Active = true, Deleted = false, permissionId = 285, userId = 435 },
                new TLIuserPermission { Id = 754, Active = true, Deleted = false, permissionId = 286, userId = 435 },
                new TLIuserPermission { Id = 755, Active = true, Deleted = false, permissionId = 287, userId = 435 },
                new TLIuserPermission { Id = 756, Active = true, Deleted = false, permissionId = 288, userId = 435 },
                new TLIuserPermission { Id = 757, Active = true, Deleted = false, permissionId = 289, userId = 435 },

                new TLIuserPermission { Id = 758, Active = true, Deleted = false, permissionId = 290, userId = 435 },
                new TLIuserPermission { Id = 759, Active = true, Deleted = false, permissionId = 291, userId = 435 },
                new TLIuserPermission { Id = 760, Active = true, Deleted = false, permissionId = 292, userId = 435 },
                new TLIuserPermission { Id = 761, Active = true, Deleted = false, permissionId = 293, userId = 435 },
                new TLIuserPermission { Id = 762, Active = true, Deleted = false, permissionId = 294, userId = 435 },
                new TLIuserPermission { Id = 763, Active = true, Deleted = false, permissionId = 295, userId = 435 },
                new TLIuserPermission { Id = 764, Active = true, Deleted = false, permissionId = 296, userId = 435 },
                new TLIuserPermission { Id = 765, Active = true, Deleted = false, permissionId = 297, userId = 435 },
                new TLIuserPermission { Id = 766, Active = true, Deleted = false, permissionId = 298, userId = 435 },
                new TLIuserPermission { Id = 767, Active = true, Deleted = false, permissionId = 299, userId = 435 },

                new TLIuserPermission { Id = 768, Active = true, Deleted = false, permissionId = 300, userId = 435 },
                new TLIuserPermission { Id = 769, Active = true, Deleted = false, permissionId = 301, userId = 435 },
                new TLIuserPermission { Id = 770, Active = true, Deleted = false, permissionId = 302, userId = 435 },
                new TLIuserPermission { Id = 771, Active = true, Deleted = false, permissionId = 303, userId = 435 },
                new TLIuserPermission { Id = 772, Active = true, Deleted = false, permissionId = 304, userId = 435 },
                new TLIuserPermission { Id = 773, Active = true, Deleted = false, permissionId = 305, userId = 435 },
                new TLIuserPermission { Id = 774, Active = true, Deleted = false, permissionId = 306, userId = 435 },
                new TLIuserPermission { Id = 775, Active = true, Deleted = false, permissionId = 307, userId = 435 },
                new TLIuserPermission { Id = 776, Active = true, Deleted = false, permissionId = 308, userId = 435 },
                new TLIuserPermission { Id = 777, Active = true, Deleted = false, permissionId = 309, userId = 435 },

                new TLIuserPermission { Id = 778, Active = true, Deleted = false, permissionId = 310, userId = 435 },
                new TLIuserPermission { Id = 779, Active = true, Deleted = false, permissionId = 311, userId = 435 },
                new TLIuserPermission { Id = 780, Active = true, Deleted = false, permissionId = 312, userId = 435 },
                new TLIuserPermission { Id = 781, Active = true, Deleted = false, permissionId = 313, userId = 435 },
                new TLIuserPermission { Id = 782, Active = true, Deleted = false, permissionId = 314, userId = 435 },
                new TLIuserPermission { Id = 783, Active = true, Deleted = false, permissionId = 315, userId = 435 },
                new TLIuserPermission { Id = 784, Active = true, Deleted = false, permissionId = 316, userId = 435 },
                new TLIuserPermission { Id = 785, Active = true, Deleted = false, permissionId = 317, userId = 435 },
                new TLIuserPermission { Id = 786, Active = true, Deleted = false, permissionId = 318, userId = 435 },
                new TLIuserPermission { Id = 787, Active = true, Deleted = false, permissionId = 319, userId = 435 },

                new TLIuserPermission { Id = 788, Active = true, Deleted = false, permissionId = 320, userId = 435 },
                new TLIuserPermission { Id = 789, Active = true, Deleted = false, permissionId = 321, userId = 435 },
                new TLIuserPermission { Id = 790, Active = true, Deleted = false, permissionId = 322, userId = 435 },
                new TLIuserPermission { Id = 791, Active = true, Deleted = false, permissionId = 323, userId = 435 },
                new TLIuserPermission { Id = 792, Active = true, Deleted = false, permissionId = 324, userId = 435 },
                new TLIuserPermission { Id = 793, Active = true, Deleted = false, permissionId = 325, userId = 435 },
                new TLIuserPermission { Id = 794, Active = true, Deleted = false, permissionId = 326, userId = 435 },
                new TLIuserPermission { Id = 795, Active = true, Deleted = false, permissionId = 327, userId = 435 },
                new TLIuserPermission { Id = 796, Active = true, Deleted = false, permissionId = 328, userId = 435 },
                new TLIuserPermission { Id = 797, Active = true, Deleted = false, permissionId = 329, userId = 435 },

                new TLIuserPermission { Id = 798, Active = true, Deleted = false, permissionId = 330, userId = 435 },
                new TLIuserPermission { Id = 799, Active = true, Deleted = false, permissionId = 331, userId = 435 },
                new TLIuserPermission { Id = 800, Active = true, Deleted = false, permissionId = 332, userId = 435 },
                new TLIuserPermission { Id = 801, Active = true, Deleted = false, permissionId = 333, userId = 435 },
                new TLIuserPermission { Id = 802, Active = true, Deleted = false, permissionId = 334, userId = 435 },
                new TLIuserPermission { Id = 803, Active = true, Deleted = false, permissionId = 335, userId = 435 },
                new TLIuserPermission { Id = 804, Active = true, Deleted = false, permissionId = 336, userId = 435 },
                new TLIuserPermission { Id = 805, Active = true, Deleted = false, permissionId = 337, userId = 435 },
                new TLIuserPermission { Id = 806, Active = true, Deleted = false, permissionId = 338, userId = 435 },
                new TLIuserPermission { Id = 807, Active = true, Deleted = false, permissionId = 339, userId = 435 },

                new TLIuserPermission { Id = 808, Active = true, Deleted = false, permissionId = 340, userId = 435 },
                new TLIuserPermission { Id = 809, Active = true, Deleted = false, permissionId = 341, userId = 435 },
                new TLIuserPermission { Id = 810, Active = true, Deleted = false, permissionId = 342, userId = 435 },
                new TLIuserPermission { Id = 811, Active = true, Deleted = false, permissionId = 343, userId = 435 },
                new TLIuserPermission { Id = 812, Active = true, Deleted = false, permissionId = 344, userId = 435 },
                new TLIuserPermission { Id = 813, Active = true, Deleted = false, permissionId = 345, userId = 435 },
                new TLIuserPermission { Id = 814, Active = true, Deleted = false, permissionId = 346, userId = 435 },
                new TLIuserPermission { Id = 815, Active = true, Deleted = false, permissionId = 347, userId = 435 },
                new TLIuserPermission { Id = 816, Active = true, Deleted = false, permissionId = 348, userId = 435 },
                new TLIuserPermission { Id = 817, Active = true, Deleted = false, permissionId = 349, userId = 435 },

                new TLIuserPermission { Id = 818, Active = true, Deleted = false, permissionId = 350, userId = 435 },
                new TLIuserPermission { Id = 819, Active = true, Deleted = false, permissionId = 351, userId = 435 },
                new TLIuserPermission { Id = 820, Active = true, Deleted = false, permissionId = 352, userId = 435 },
                new TLIuserPermission { Id = 821, Active = true, Deleted = false, permissionId = 353, userId = 435 },
                new TLIuserPermission { Id = 822, Active = true, Deleted = false, permissionId = 354, userId = 435 },
                new TLIuserPermission { Id = 823, Active = true, Deleted = false, permissionId = 355, userId = 435 },
                new TLIuserPermission { Id = 824, Active = true, Deleted = false, permissionId = 356, userId = 435 },
                new TLIuserPermission { Id = 825, Active = true, Deleted = false, permissionId = 357, userId = 435 },
                new TLIuserPermission { Id = 826, Active = true, Deleted = false, permissionId = 358, userId = 435 },
                new TLIuserPermission { Id = 827, Active = true, Deleted = false, permissionId = 359, userId = 435 },

                new TLIuserPermission { Id = 828, Active = true, Deleted = false, permissionId = 360, userId = 435 },
                new TLIuserPermission { Id = 829, Active = true, Deleted = false, permissionId = 361, userId = 435 },
                new TLIuserPermission { Id = 830, Active = true, Deleted = false, permissionId = 362, userId = 435 },
                new TLIuserPermission { Id = 831, Active = true, Deleted = false, permissionId = 363, userId = 435 },
                new TLIuserPermission { Id = 832, Active = true, Deleted = false, permissionId = 364, userId = 435 },
                new TLIuserPermission { Id = 833, Active = true, Deleted = false, permissionId = 365, userId = 435 },
                new TLIuserPermission { Id = 834, Active = true, Deleted = false, permissionId = 366, userId = 435 },
                new TLIuserPermission { Id = 835, Active = true, Deleted = false, permissionId = 367, userId = 435 },
                new TLIuserPermission { Id = 836, Active = true, Deleted = false, permissionId = 368, userId = 435 },
                new TLIuserPermission { Id = 837, Active = true, Deleted = false, permissionId = 369, userId = 435 },

                new TLIuserPermission { Id = 838, Active = true, Deleted = false, permissionId = 370, userId = 435 },
                new TLIuserPermission { Id = 839, Active = true, Deleted = false, permissionId = 371, userId = 435 },
                new TLIuserPermission { Id = 840, Active = true, Deleted = false, permissionId = 372, userId = 435 },
                new TLIuserPermission { Id = 841, Active = true, Deleted = false, permissionId = 373, userId = 435 },
                new TLIuserPermission { Id = 842, Active = true, Deleted = false, permissionId = 374, userId = 435 },
                new TLIuserPermission { Id = 843, Active = true, Deleted = false, permissionId = 375, userId = 435 },
                new TLIuserPermission { Id = 844, Active = true, Deleted = false, permissionId = 376, userId = 435 },
                new TLIuserPermission { Id = 845, Active = true, Deleted = false, permissionId = 377, userId = 435 },
                new TLIuserPermission { Id = 846, Active = true, Deleted = false, permissionId = 378, userId = 435 },
                new TLIuserPermission { Id = 847, Active = true, Deleted = false, permissionId = 379, userId = 435 },

                new TLIuserPermission { Id = 848, Active = true, Deleted = false, permissionId = 380, userId = 435 },
                new TLIuserPermission { Id = 849, Active = true, Deleted = false, permissionId = 381, userId = 435 },
                new TLIuserPermission { Id = 850, Active = true, Deleted = false, permissionId = 382, userId = 435 },
                new TLIuserPermission { Id = 851, Active = true, Deleted = false, permissionId = 383, userId = 435 },
                new TLIuserPermission { Id = 852, Active = true, Deleted = false, permissionId = 384, userId = 435 },
                new TLIuserPermission { Id = 853, Active = true, Deleted = false, permissionId = 385, userId = 435 },
                new TLIuserPermission { Id = 854, Active = true, Deleted = false, permissionId = 386, userId = 435 },
                new TLIuserPermission { Id = 855, Active = true, Deleted = false, permissionId = 387, userId = 435 },
                new TLIuserPermission { Id = 856, Active = true, Deleted = false, permissionId = 388, userId = 435 },
                new TLIuserPermission { Id = 857, Active = true, Deleted = false, permissionId = 389, userId = 435 },

                new TLIuserPermission { Id = 858, Active = true, Deleted = false, permissionId = 390, userId = 435 },
                new TLIuserPermission { Id = 859, Active = true, Deleted = false, permissionId = 391, userId = 435 },
                new TLIuserPermission { Id = 860, Active = true, Deleted = false, permissionId = 392, userId = 435 },
                new TLIuserPermission { Id = 861, Active = true, Deleted = false, permissionId = 393, userId = 435 },
                new TLIuserPermission { Id = 862, Active = true, Deleted = false, permissionId = 394, userId = 435 },
                new TLIuserPermission { Id = 863, Active = true, Deleted = false, permissionId = 395, userId = 435 },
                new TLIuserPermission { Id = 864, Active = true, Deleted = false, permissionId = 396, userId = 435 },
                new TLIuserPermission { Id = 865, Active = true, Deleted = false, permissionId = 397, userId = 435 },
                new TLIuserPermission { Id = 866, Active = true, Deleted = false, permissionId = 398, userId = 435 },
                new TLIuserPermission { Id = 867, Active = true, Deleted = false, permissionId = 399, userId = 435 },

                new TLIuserPermission { Id = 868, Active = true, Deleted = false, permissionId = 400, userId = 435 },
                new TLIuserPermission { Id = 869, Active = true, Deleted = false, permissionId = 401, userId = 435 },
                new TLIuserPermission { Id = 870, Active = true, Deleted = false, permissionId = 402, userId = 435 },
                new TLIuserPermission { Id = 871, Active = true, Deleted = false, permissionId = 403, userId = 435 },
                new TLIuserPermission { Id = 872, Active = true, Deleted = false, permissionId = 404, userId = 435 },
                new TLIuserPermission { Id = 873, Active = true, Deleted = false, permissionId = 405, userId = 435 },
                new TLIuserPermission { Id = 874, Active = true, Deleted = false, permissionId = 406, userId = 435 },
                new TLIuserPermission { Id = 875, Active = true, Deleted = false, permissionId = 407, userId = 435 },
                new TLIuserPermission { Id = 876, Active = true, Deleted = false, permissionId = 408, userId = 435 },
                new TLIuserPermission { Id = 877, Active = true, Deleted = false, permissionId = 409, userId = 435 },

                new TLIuserPermission { Id = 878, Active = true, Deleted = false, permissionId = 410, userId = 435 },
                new TLIuserPermission { Id = 879, Active = true, Deleted = false, permissionId = 411, userId = 435 },
                new TLIuserPermission { Id = 880, Active = true, Deleted = false, permissionId = 412, userId = 435 },
                new TLIuserPermission { Id = 881, Active = true, Deleted = false, permissionId = 413, userId = 435 },
                new TLIuserPermission { Id = 882, Active = true, Deleted = false, permissionId = 414, userId = 435 },
                new TLIuserPermission { Id = 883, Active = true, Deleted = false, permissionId = 415, userId = 435 },
                new TLIuserPermission { Id = 884, Active = true, Deleted = false, permissionId = 416, userId = 435 },
                new TLIuserPermission { Id = 885, Active = true, Deleted = false, permissionId = 417, userId = 435 },
                new TLIuserPermission { Id = 886, Active = true, Deleted = false, permissionId = 418, userId = 435 },
                new TLIuserPermission { Id = 887, Active = true, Deleted = false, permissionId = 419, userId = 435 },

                new TLIuserPermission { Id = 888, Active = true, Deleted = false, permissionId = 420, userId = 435 },
                new TLIuserPermission { Id = 889, Active = true, Deleted = false, permissionId = 421, userId = 435 },
                new TLIuserPermission { Id = 890, Active = true, Deleted = false, permissionId = 422, userId = 435 },
                new TLIuserPermission { Id = 891, Active = true, Deleted = false, permissionId = 423, userId = 435 },
                new TLIuserPermission { Id = 892, Active = true, Deleted = false, permissionId = 424, userId = 435 },
                new TLIuserPermission { Id = 893, Active = true, Deleted = false, permissionId = 425, userId = 435 },
                new TLIuserPermission { Id = 894, Active = true, Deleted = false, permissionId = 426, userId = 435 },
                new TLIuserPermission { Id = 895, Active = true, Deleted = false, permissionId = 427, userId = 435 },
                new TLIuserPermission { Id = 896, Active = true, Deleted = false, permissionId = 428, userId = 435 },
                new TLIuserPermission { Id = 897, Active = true, Deleted = false, permissionId = 429, userId = 435 },

                new TLIuserPermission { Id = 898, Active = true, Deleted = false, permissionId = 430, userId = 435 },
                new TLIuserPermission { Id = 899, Active = true, Deleted = false, permissionId = 431, userId = 435 },
                new TLIuserPermission { Id = 900, Active = true, Deleted = false, permissionId = 432, userId = 435 },
                new TLIuserPermission { Id = 901, Active = true, Deleted = false, permissionId = 433, userId = 435 },
                new TLIuserPermission { Id = 902, Active = true, Deleted = false, permissionId = 434, userId = 435 },
                new TLIuserPermission { Id = 903, Active = true, Deleted = false, permissionId = 435, userId = 435 },
                new TLIuserPermission { Id = 904, Active = true, Deleted = false, permissionId = 436, userId = 435 },
                new TLIuserPermission { Id = 905, Active = true, Deleted = false, permissionId = 437, userId = 435 },
                new TLIuserPermission { Id = 906, Active = true, Deleted = false, permissionId = 438, userId = 435 },
                new TLIuserPermission { Id = 907, Active = true, Deleted = false, permissionId = 439, userId = 435 },

                new TLIuserPermission { Id = 908, Active = true, Deleted = false, permissionId = 440, userId = 435 },
                new TLIuserPermission { Id = 909, Active = true, Deleted = false, permissionId = 441, userId = 435 },
                new TLIuserPermission { Id = 910, Active = true, Deleted = false, permissionId = 442, userId = 435 },
                new TLIuserPermission { Id = 911, Active = true, Deleted = false, permissionId = 443, userId = 435 },
                new TLIuserPermission { Id = 912, Active = true, Deleted = false, permissionId = 444, userId = 435 },
                new TLIuserPermission { Id = 913, Active = true, Deleted = false, permissionId = 445, userId = 435 },
                new TLIuserPermission { Id = 914, Active = true, Deleted = false, permissionId = 446, userId = 435 },
                new TLIuserPermission { Id = 915, Active = true, Deleted = false, permissionId = 447, userId = 435 },
                new TLIuserPermission { Id = 916, Active = true, Deleted = false, permissionId = 448, userId = 435 },
                new TLIuserPermission { Id = 917, Active = true, Deleted = false, permissionId = 449, userId = 435 },

                new TLIuserPermission { Id = 918, Active = true, Deleted = false, permissionId = 450, userId = 435 },
                new TLIuserPermission { Id = 919, Active = true, Deleted = false, permissionId = 451, userId = 435 },
                new TLIuserPermission { Id = 920, Active = true, Deleted = false, permissionId = 452, userId = 435 },
                new TLIuserPermission { Id = 921, Active = true, Deleted = false, permissionId = 453, userId = 435 },
                new TLIuserPermission { Id = 922, Active = true, Deleted = false, permissionId = 454, userId = 435 },
                new TLIuserPermission { Id = 923, Active = true, Deleted = false, permissionId = 455, userId = 435 },
                new TLIuserPermission { Id = 924, Active = true, Deleted = false, permissionId = 456, userId = 435 },
                new TLIuserPermission { Id = 925, Active = true, Deleted = false, permissionId = 457, userId = 435 },
                new TLIuserPermission { Id = 926, Active = true, Deleted = false, permissionId = 458, userId = 435 },
                new TLIuserPermission { Id = 927, Active = true, Deleted = false, permissionId = 459, userId = 435 },

                new TLIuserPermission { Id = 928, Active = true, Deleted = false, permissionId = 460, userId = 435 },
                new TLIuserPermission { Id = 929, Active = true, Deleted = false, permissionId = 461, userId = 435 },
                new TLIuserPermission { Id = 930, Active = true, Deleted = false, permissionId = 462, userId = 435 },
                new TLIuserPermission { Id = 931, Active = true, Deleted = false, permissionId = 463, userId = 435 },
                new TLIuserPermission { Id = 932, Active = true, Deleted = false, permissionId = 464, userId = 435 },
                new TLIuserPermission { Id = 933, Active = true, Deleted = false, permissionId = 465, userId = 435 },
                new TLIuserPermission { Id = 934, Active = true, Deleted = false, permissionId = 466, userId = 435 },
                new TLIuserPermission { Id = 935, Active = true, Deleted = false, permissionId = 467, userId = 435 }
                );
        }
    }
}
