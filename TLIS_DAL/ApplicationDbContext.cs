using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TLIS_DAL.Helper.Enums;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModels.LogDTOs;
using Toolbelt.ComponentModel.DataAnnotations;
using System.Data;

namespace TLIS_DAL
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        { }


        public virtual DbSet<TLIarea> TLIarea { get; set; }
        public virtual DbSet<TLIasType> TLIasType { get; set; }
        public virtual DbSet<TLIattributeActivated> TLIattributeActivated { get; set; }
        public virtual DbSet<TLIboardType> TLIboardType { get; set; }
        public virtual DbSet<TLIcity> TLIcity { get; set; }
        public virtual DbSet<TLIcivilNonSteelLibrary> TLIcivilNonSteelLibrary { get; set; }
        public virtual DbSet<TLIcivilSteelSupportCategory> TLIcivilSteelSupportCategory { get; set; }
        public virtual DbSet<TLIcivilWithLegLibrary> TLIcivilWithLegLibrary { get; set; }
        public virtual DbSet<TLIcivilWithoutLegLibrary> TLIcivilWithoutLegLibrary { get; set; }
        public virtual DbSet<TLIconditionType> TLIconditionType { get; set; }
        public virtual DbSet<TLIdiversityType> TLIdiversityType { get; set; }
        public virtual DbSet<TLIinstallationCivilwithoutLegsType> TLIinstallationCivilwithoutLegsType { get; set; }
        public virtual DbSet<TLIleg> TLIleg { get; set; }
        public virtual DbSet<TLIlog> TLIlog { get; set; }
        public virtual DbSet<TLIlogistical> TLIlogistical { get; set; }
        public virtual DbSet<TLIlogisticalitem> TLIlogisticalitem { get; set; }
        public virtual DbSet<TLIlogisticalType> TLIlogisticalType { get; set; }
        public virtual DbSet<TLImwBULibrary> TLImwBULibrary { get; set; }
        public virtual DbSet<TLImwDishLibrary> TLImwDishLibrary { get; set; }
        public virtual DbSet<TLImwODULibrary> TLImwODULibrary { get; set; }
        public virtual DbSet<TLImwRFULibrary> TLImwRFULibrary { get; set; }
        public virtual DbSet<TLIparity> TLIparity { get; set; }
        public virtual DbSet<TLIpolarityType> TLIpolarityType { get; set; }
        public virtual DbSet<TLIpowerLibrary> TLIpowerLibrary { get; set; }
        public virtual DbSet<TLIregion> TLIregion { get; set; }
        public virtual DbSet<TLIsectionsLegType> TLIsectionsLegType { get; set; }
        public virtual DbSet<TLIsideArmLibrary> TLIsideArmLibrary { get; set; }
        public virtual DbSet<TLIsite> TLIsite { get; set; }
        public virtual DbSet<TLIstructureType> TLIstructureType { get; set; }
        public virtual DbSet<TLIsupportTypeDesigned> TLIsupportTypeDesigned { get; set; }
        public virtual DbSet<TLIsupportTypeImplemented> TLIsupportTypeImplemented { get; set; }
        public virtual DbSet<TLIlogUsersActions> TLIlogUsersActions { get; set; }
        //public virtual DbSet<TLIcategory> TLIcategory { get; set; }
        //public virtual DbSet<TLIsupportTypeImplemented> TLIsupportTypeImplemented { get; set; }
        public virtual DbSet<TLIgroup> TLIgroup { get; set; }
        public virtual DbSet<TLIgroupRole> TLIgroupRole { get; set; }
        public virtual DbSet<TLIgroupUser> TLIgroupUser { get; set; }
        public virtual DbSet<TLIrole> TLIrole { get; set; }
        public virtual DbSet<TLIrolePermission> TLIrolePermission { get; set; }
        public virtual DbSet<TLIuser> TLIuser { get; set; }
        public virtual DbSet<TLIuserPermission> TLIuserPermission { get; set; }
        public virtual DbSet<TLIpermission> TLIpermission { get; set; }
        public virtual DbSet<TLIdynamicAtt> TLIdynamicAtt { get; set; }
        public virtual DbSet<TLIdynamicAttInstValue> TLIdynamicAttInstValue { get; set; }
        public virtual DbSet<TLIdynamicAttLibValue> TLIdynamicAttLibValue { get; set; }
        public virtual DbSet<TLIoperation> TLIoperation { get; set; }
        public virtual DbSet<TLIvalidation> TLIvalidation { get; set; }
        public virtual DbSet<TLItaskStatus> TLItaskStatus { get; set; }

        public virtual DbSet<TLIcivilWithLegs> TLIcivilWithLegs { get; set; }
        public virtual DbSet<TLIcivilWithoutLeg> TLIcivilWithoutLeg { get; set; }
        public virtual DbSet<TLIcivilNonSteel> TLIcivilNonSteel { get; set; }
        public virtual DbSet<TLIcivilSiteDate> TLIcivilSiteDate { get; set; }
        public virtual DbSet<TLIsubType> TLIsubType { get; set; }
        public virtual DbSet<TLIowner> TLIowner { get; set; }
        public virtual DbSet<TLIbaseCivilWithLegsType> TLIbaseCivilWithLegsType { get; set; }
        public virtual DbSet<TLIguyLineType> TLIguyLineType { get; set; }
        public virtual DbSet<TLIattActivatedCategory> TLIattActivatedCategory { get; set; }
        public virtual DbSet<TLIlogicalOperation> TLIlogicalOperation { get; set; }
        public virtual DbSet<TLIrule> TLIrule { get; set; }
        public virtual DbSet<TLIrow> TLIrow { get; set; }
        public virtual DbSet<TLIrowRule> TLIrowRule { get; set; }
        public virtual DbSet<TLIdependency> TLIdependency { get; set; }
        public virtual DbSet<TLIdependencyRow> TLIdependencyRow { get; set; }
        public virtual DbSet<TLIcondition> TLIcondition { get; set; }
        public virtual DbSet<TLIradioRRULibrary> TLIradioRRULibrary { get; set; }
        public virtual DbSet<TLIradioAntennaLibrary> TLIradioAntennaLibrary { get; set; }
        public virtual DbSet<TLIradioOtherLibrary> TLIradioOtherLibrary { get; set; }
        public virtual DbSet<TLIoption> TLIoption { get; set; }
        public virtual DbSet<TLIsuboption> TLIsuboption { get; set; }
        public virtual DbSet<TLIitemStatus> TLIitemStatus { get; set; }
        public virtual DbSet<TLIcapacity> TLIcapacity { get; set; }
        public virtual DbSet<TLItelecomType> TLItelecomType { get; set; }
        public virtual DbSet<TLIcabinetPowerType> TLIcabinetPowerType { get; set; }
        public virtual DbSet<TLIsolarLibrary> TLIsolarLibrary { get; set; }
        public virtual DbSet<TLIgeneratorLibrary> TLIgeneratorLibrary { get; set; }
        public virtual DbSet<TLIcabinetTelecomLibrary> TLIcabinetTelecomLibrary { get; set; }
        public virtual DbSet<TLIcabinetPowerLibrary> TLIcabinetPowerLibrary { get; set; }
        public virtual DbSet<TLIactor> TLIactor { get; set; }
        public virtual DbSet<TLIbaseGeneratorType> TLIbaseGeneratorType { get; set; }
        public virtual DbSet<TLIgenerator> TLIgenerator { get; set; }
        public virtual DbSet<TLIsolar> TLIsolar { get; set; }
        public virtual DbSet<TLIrenewableCabinetType> TLIrenewableCabinetType { get; set; }
        public virtual DbSet<TLIcabinet> TLIcabinet { get; set; }
        public virtual DbSet<TLIotherInventoryDistance> TLIotherInventoryDistance { get; set; }
        public virtual DbSet<TLIotherInSite> TLIotherInSite { get; set; }
        public virtual DbSet<TLIhistoryType> TLIhistoryType { get; set; }
        public virtual DbSet<TLItablesHistory> TLItablesHistory { get; set; }
        public virtual DbSet<TLIhistoryDetails> TLIhistoryDetails { get; set; }
        public virtual DbSet<TLIsiteStatus> TLIsiteStatus { get; set; }
        public virtual DbSet<TLIinstallationPlace> TLIinstallationPlace { get; set; }
        public virtual DbSet<TLIRadioRRU> TLIRadioRRU { get; set; }
        public virtual DbSet<TLIradioAntenna> TLIradioAntenna { get; set; }
        public virtual DbSet<TLIpolarityOnLocation> TLIpolarityOnLocation { get; set; }
        public virtual DbSet<TLIallCivilInst> TLIallCivilInst { get; set; }
        public virtual DbSet<TLIallOtherInventoryInst> TLIallOtherInventoryInst { get; set; }
        public virtual DbSet<TLIcivilWithoutLegCategory> TLIcivilWithoutLegCategory { get; set; }
        public virtual DbSet<TLIbaseType> TLIbaseType { get; set; }
        public virtual DbSet<TLIdataType> TLIdataType { get; set; }
        public virtual DbSet<TLIitemConnectTo> TLIitemConnectTo { get; set; }
        public virtual DbSet<TLIrepeaterType> TLIrepeaterType { get; set; }
        public virtual DbSet<TLIoduInstallationType> TLIoduInstallationType { get; set; }
        public virtual DbSet<TLIsideArmInstallationPlace> TLIsideArmInstallationPlace { get; set; }
        public virtual DbSet<TLIsideArm> TLIsideArm { get; set; }
        public virtual DbSet<TLImwBU> TLImwBU { get; set; }
        public virtual DbSet<TLImwDish> TLImwDish { get; set; }
        public virtual DbSet<TLImwODU> TLImwODU { get; set; }
        public virtual DbSet<TLItablesNames> TLItablesNames { get; set; }
        public virtual DbSet<TLItablePartName> TLItablePartName { get; set; }
        public virtual DbSet<TLImwRFU> TLImwRFU { get; set; }
        public virtual DbSet<TLImwPort> TLImwPort { get; set; }
        public virtual DbSet<TLImwOtherLibrary> TLImwOtherLibrary { get; set; }
        public virtual DbSet<TLIloadOtherLibrary> TLIloadOtherLibrary { get; set; }
        public virtual DbSet<TLIattachedFiles> TLIattachedFiles { get; set; }
        public virtual DbSet<TLIcivilLoads> TLIcivilLoads { get; set; }
        public virtual DbSet<TLIallLoadInst> TLIallLoadInst { get; set; }
        public virtual DbSet<TLImwOther> TLImwOther { get; set; }
        public virtual DbSet<TLIradioOther> TLIradioOther { get; set; }
        public virtual DbSet<TLIloadOther> TLIloadOther { get; set; }
        public virtual DbSet<TLIpower> TLIpower { get; set; }
        public virtual DbSet<TLIenforcmentCategory> TLIenforcmentCategory { get; set; }
        public virtual DbSet<TLIinstallationType> TLIinstallationType { get; set; }
        public virtual DbSet<TLIcivilLoadLegs> TLIcivilLoadLegs { get; set; }
        public virtual DbSet<TLIantennaRRUInst> TLIantennaRRUInst { get; set; }
        public virtual DbSet<TLIpowerType> TLIpowerType { get; set; }
        public virtual DbSet<TLImailTemplate> TLImailTemplate { get; set; }
        public virtual DbSet<TLIcivilNonSteelType> TLIcivilNonSteelType { get; set; }
        public virtual DbSet<TLIlocationType> TLIlocationType { get; set; }
        public virtual DbSet<TLIticket> TLIticket { get; set; }
        public virtual DbSet<TLIsideArmType> TLIsideArmType { get; set; }
        public virtual DbSet<TLIticketActionFile> TLIticketActionFile { get; set; }
        public virtual DbSet<TLIticketEquipmentActionFile> TLIticketEquipmentActionFile { get; set; }
        public virtual DbSet<TLIstepActionItemStatus> TLIstepActionItemStatus { get; set; }
        public virtual DbSet<TLIagenda> TLIagenda { get; set; }
        public virtual DbSet<TLIbaseBU> TLIbaseBU { get; set; }
        public virtual DbSet<TLIattributeViewManagment> TLIattributeViewManagment { get; set; }
        public virtual DbSet<TLIeditableManagmentView> TLIeditableManagmentView { get; set; }
        public virtual DbSet<TLIworkflowTableHistory> TLIworkflowTableHistory { get; set; }
        public virtual DbSet<TLInextStepAction> TLInextStepAction { get; set; }
        public virtual DbSet<TLIdocumentType> TLIdocumentType { get; set; }
        public virtual DbSet<TLIimportSheet> TLIimportSheets { get; set; }
        public virtual DbSet<TLIexternalSys> TLIexternalSys { get; set; }
        public virtual DbSet<TLIinternalApis> TLIinternalApis { get; set; }
        public virtual DbSet<TLIexternalSysPermissions> TLIexternalSysPermissions { get; set; }
        public virtual DbSet<TLIintegrationAccessLog> TLIintegrationAccessLog { get; set; }
        public virtual DbSet<TLIpermissions> TLIpermissions { get; set; }
        public virtual DbSet<TLIuserPermissions> TLIuserPermissions { get; set; }
        public virtual DbSet<TLIgroupPermissions> TLIgroupPermissions { get; set; }
        public virtual DbSet<TLIrolePermissions> TLIrolePermissions { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<TLIsite>()
                .HasOne(e => e.Region)
                .WithMany(z => z.sites)
                .HasForeignKey(c => c.RegionCode);

            builder.Entity<TLIcivilLoads>()
                .HasOne(e => e.site)
                .WithMany(z => z.CivilLoads)
                .HasForeignKey(c => c.SiteCode);

            builder.Entity<TLIcivilSiteDate>()
               .HasOne(e => e.Site)
               .WithMany(z => z.CivilSiteDate)
               .HasForeignKey(c => c.SiteCode);

            builder.Entity<TLIcivilSupportDistance>()
             .HasOne(e => e.site)
             .WithMany(z => z.civilSupportDistances)
             .HasForeignKey(c => c.SiteCode);


            builder.Entity<TLIotherInSite>()
             .HasOne(e => e.Site)
             .WithMany(z => z.OthersInSite)
             .HasForeignKey(c => c.SiteCode);

            builder.Entity<TLIotherInSite>()
              .HasOne(e => e.Site)
              .WithMany(z => z.OthersInSite)
              .HasForeignKey(c => c.SiteCode);

            builder.Entity<TLIotherInventoryDistance>()
                .HasOne(e => e.site)
                .WithMany(z => z.OtherInventoryDistances)
                .HasForeignKey(c => c.SiteCode);

            builder.Entity<TLIuserPermissions>()
                .HasOne(e => e.User)
                .WithMany(z => z.userPermissionss)
                .HasForeignKey(c => c.User_Id);

            builder.Entity<TLIuserPermissions>()
                .HasOne(e => e.Permission)
                .WithMany(z => z.userPermissionss)
                .HasForeignKey(c => c.Permission_Id);

            builder.Entity<TLIticket>()
                .HasOne(e => e.Site)
                .WithMany(z => z.Tickets)
                .HasForeignKey(c => c.SiteCode);

            //     builder.Entity<TLIdiversityType>().HasData(
            //new TLIdiversityType {  Name = "Di_s" }
            //new TLIdiversityType { BookId = 2, AuthorId = 1, Title = "King Lear" },
            //new TLIdiversityType { BookId = 3, AuthorId = 1, Title = "Othello" }
            // );

            builder.BuildIndexesFromAnnotations();
            builder.seed();

            builder.Entity<TLIpowerLibrary>().Property(x => x.Active).HasDefaultValue(true);

            builder.Entity<TLIpowerLibrary>().Property(x => x.Deleted).HasDefaultValue(false);
            //builder.Entity<TLIticket>().Property(x => x.Deleted).HasDefaultValue(false);

            builder.Entity<TLIsideArmLibrary>().Property(x => x.Active).HasDefaultValue(true);

            builder.Entity<TLIsideArmLibrary>().Property(x => x.Deleted).HasDefaultValue(false);

            builder.Entity<TLIuser>().Property(x => x.Active).HasDefaultValue(true);

            builder.Entity<TLIuser>().Property(x => x.Deleted).HasDefaultValue(false);

            builder.Entity<TLIrole>().Property(x => x.Active).HasDefaultValue(true);

            builder.Entity<TLIrole>().Property(x => x.Deleted).HasDefaultValue(false);

            builder.Entity<TLIrolePermission>().Property(x => x.Active).HasDefaultValue(true);

            builder.Entity<TLIrolePermission>().Property(x => x.Deleted).HasDefaultValue(false);

            builder.Entity<TLIpermission>().Property(x => x.Active).HasDefaultValue(true);

            builder.Entity<TLIuserPermission>().Property(x => x.Active).HasDefaultValue(true);

            builder.Entity<TLIuserPermission>().Property(x => x.Deleted).HasDefaultValue(false);

            builder.Entity<TLIgroup>().Property(x => x.Active).HasDefaultValue(true);

            builder.Entity<TLIgroup>().Property(x => x.Deleted).HasDefaultValue(false);

            builder.Entity<TLIgroupRole>().Property(x => x.Active).HasDefaultValue(true);

            builder.Entity<TLIgroupRole>().Property(x => x.Deleted).HasDefaultValue(false);

            builder.Entity<TLIgroupUser>().Property(x => x.Active).HasDefaultValue(true);

            builder.Entity<TLIgroupUser>().Property(x => x.Deleted).HasDefaultValue(false);

            builder.Entity<TLIdynamicAtt>().Property(x => x.disable).HasDefaultValue(false);

            builder.Entity<TLIdynamicAtt>().Property(x => x.Required).HasDefaultValue(false);

            builder.Entity<TLIdynamicAttLibValue>().Property(x => x.disable).HasDefaultValue(false);

            builder.Entity<TLIdynamicAttInstValue>().Property(x => x.disable).HasDefaultValue(false);

            builder.Entity<TLIattActivatedCategory>().Property(x => x.enable).HasDefaultValue(true);

            builder.Entity<TLIradioRRULibrary>().Property(x => x.Active).HasDefaultValue(true);

            builder.Entity<TLIradioRRULibrary>().Property(x => x.Deleted).HasDefaultValue(false);

            builder.Entity<TLIradioAntennaLibrary>().Property(x => x.Active).HasDefaultValue(true);

            builder.Entity<TLIradioAntennaLibrary>().Property(x => x.Deleted).HasDefaultValue(false);

            builder.Entity<TLIradioOtherLibrary>().Property(x => x.Active).HasDefaultValue(true);

            builder.Entity<TLIradioOtherLibrary>().Property(x => x.Deleted).HasDefaultValue(false);

            builder.Entity<TLItaskStatus>().Property(x => x.Disable).HasDefaultValue(false);

            builder.Entity<TLIitemStatus>().Property(x => x.Active).HasDefaultValue(true);

            builder.Entity<TLIitemStatus>().Property(x => x.Deleted).HasDefaultValue(false);

            builder.Entity<TLIsolarLibrary>().Property(x => x.Active).HasDefaultValue(true);

            builder.Entity<TLIsolarLibrary>().Property(x => x.Deleted).HasDefaultValue(false);

            builder.Entity<TLIgeneratorLibrary>().Property(x => x.Active).HasDefaultValue(true);

            builder.Entity<TLIgeneratorLibrary>().Property(x => x.Deleted).HasDefaultValue(false);

            builder.Entity<TLIcabinetTelecomLibrary>().Property(x => x.Active).HasDefaultValue(true);

            builder.Entity<TLIcabinetTelecomLibrary>().Property(x => x.Deleted).HasDefaultValue(false);

            builder.Entity<TLIcabinetPowerLibrary>().Property(x => x.Active).HasDefaultValue(true);

            builder.Entity<TLIcabinetPowerLibrary>().Property(x => x.Deleted).HasDefaultValue(false);

            builder.Entity<TLIattributeActivated>().Property(x => x.Required).HasDefaultValue(false);

            builder.Entity<TLIattributeActivated>().Property(x => x.Manage).HasDefaultValue(false);

            builder.Entity<TLIattributeActivated>().Property(x => x.enable).HasDefaultValue(true);

            builder.Entity<TLIattributeActivated>().Property(x => x.AutoFill).HasDefaultValue(false);

            builder.Entity<TLIcivilNonSteelLibrary>().Property(x => x.Active).HasDefaultValue(true);

            builder.Entity<TLIcivilNonSteelLibrary>().Property(x => x.Deleted).HasDefaultValue(false);

            builder.Entity<TLIcivilWithLegLibrary>().Property(x => x.Active).HasDefaultValue(true);

            builder.Entity<TLIcivilWithLegLibrary>().Property(x => x.Deleted).HasDefaultValue(false);

            builder.Entity<TLIcivilWithoutLegLibrary>().Property(x => x.Active).HasDefaultValue(true);

            builder.Entity<TLIcivilWithoutLegLibrary>().Property(x => x.Deleted).HasDefaultValue(false);

            builder.Entity<TLImwBULibrary>().Property(x => x.Active).HasDefaultValue(true);

            builder.Entity<TLImwBULibrary>().Property(x => x.Deleted).HasDefaultValue(false);

            builder.Entity<TLImwDishLibrary>().Property(x => x.Active).HasDefaultValue(true);

            builder.Entity<TLImwDishLibrary>().Property(x => x.Deleted).HasDefaultValue(false);

            builder.Entity<TLImwODULibrary>().Property(x => x.Active).HasDefaultValue(true);

            builder.Entity<TLImwODULibrary>().Property(x => x.Deleted).HasDefaultValue(false);

            builder.Entity<TLImwRFULibrary>().Property(x => x.Active).HasDefaultValue(true);

            builder.Entity<TLImwRFULibrary>().Property(x => x.Deleted).HasDefaultValue(false);

            builder.Entity<TLIworkFlow>().Property(x => x.Active).HasDefaultValue(true);

            builder.Entity<TLIworkFlow>().Property(x => x.Deleted).HasDefaultValue(false);

            builder.Entity<TLImwOtherLibrary>().Property(x => x.Active).HasDefaultValue(true);

            builder.Entity<TLImwOtherLibrary>().Property(x => x.Deleted).HasDefaultValue(false);

            builder.Entity<TLIloadOtherLibrary>().Property(x => x.Active).HasDefaultValue(true);

            builder.Entity<TLIloadOtherLibrary>().Property(x => x.Deleted).HasDefaultValue(false);

            builder.Entity<TLIcivilWithoutLegCategory>().Property(x => x.disable).HasDefaultValue(false);

            builder.Entity<TLItablesNames>().Property(x => x.IsEquip).HasDefaultValue(false);

            builder.Entity<TLIdiversityType>().Property(x => x.Disable).HasDefaultValue(false);

            builder.Entity<TLItelecomType>().Property(x => x.Disable).HasDefaultValue(false);

            builder.Entity<TLIsupportTypeDesigned>().Property(x => x.Disable).HasDefaultValue(false);

            builder.Entity<TLIsupportTypeImplemented>().Property(x => x.Disable).HasDefaultValue(false);

            builder.Entity<TLIstructureType>().Property(x => x.Disable).HasDefaultValue(false);

            builder.Entity<TLIsectionsLegType>().Property(x => x.Disable).HasDefaultValue(false);

            builder.Entity<TLIlogisticalType>().Property(x => x.Disable).HasDefaultValue(false);

            builder.Entity<TLIbaseCivilWithLegsType>().Property(x => x.Disable).HasDefaultValue(false);

            builder.Entity<TLIbaseGeneratorType>().Property(x => x.Disable).HasDefaultValue(false);

            builder.Entity<TLIinstallationCivilwithoutLegsType>().Property(x => x.Disable).HasDefaultValue(false);

            builder.Entity<TLIboardType>().Property(x => x.Disable).HasDefaultValue(false);

            builder.Entity<TLIguyLineType>().Property(x => x.Disable).HasDefaultValue(false);

            builder.Entity<TLIpolarityOnLocation>().Property(x => x.Disable).HasDefaultValue(false);

            builder.Entity<TLIitemConnectTo>().Property(x => x.Disable).HasDefaultValue(false);

            builder.Entity<TLIrepeaterType>().Property(x => x.Disable).HasDefaultValue(false);

            builder.Entity<TLIoduInstallationType>().Property(x => x.Disable).HasDefaultValue(false);

            builder.Entity<TLIsideArmInstallationPlace>().Property(x => x.Disable).HasDefaultValue(false);

            builder.Entity<TLIdataType>().Property(x => x.Disable).HasDefaultValue(false);

            builder.Entity<TLIoperation>().Property(x => x.Disable).HasDefaultValue(false);

            builder.Entity<TLIlogicalOperation>().Property(x => x.Disable).HasDefaultValue(false);

            builder.Entity<TLIenforcmentCategory>().Property(x => x.Disable).HasDefaultValue(false);

            builder.Entity<TLIpowerType>().Property(x => x.Disable).HasDefaultValue(false);

            builder.Entity<TLIdiversityType>().Property(x => x.Deleted).HasDefaultValue(false);

            builder.Entity<TLItelecomType>().Property(x => x.Deleted).HasDefaultValue(false);

            builder.Entity<TLIsupportTypeDesigned>().Property(x => x.Deleted).HasDefaultValue(false);

            builder.Entity<TLIsupportTypeImplemented>().Property(x => x.Deleted).HasDefaultValue(false);

            builder.Entity<TLIstructureType>().Property(x => x.Deleted).HasDefaultValue(false);

            builder.Entity<TLIsectionsLegType>().Property(x => x.Deleted).HasDefaultValue(false);

            builder.Entity<TLIlogisticalType>().Property(x => x.Deleted).HasDefaultValue(false);

            builder.Entity<TLIbaseCivilWithLegsType>().Property(x => x.Deleted).HasDefaultValue(false);

            builder.Entity<TLIbaseGeneratorType>().Property(x => x.Deleted).HasDefaultValue(false);

            builder.Entity<TLIinstallationCivilwithoutLegsType>().Property(x => x.Deleted).HasDefaultValue(false);

            builder.Entity<TLIboardType>().Property(x => x.Deleted).HasDefaultValue(false);

            builder.Entity<TLIguyLineType>().Property(x => x.Deleted).HasDefaultValue(false);

            builder.Entity<TLIpolarityOnLocation>().Property(x => x.Deleted).HasDefaultValue(false);

            builder.Entity<TLIitemConnectTo>().Property(x => x.Deleted).HasDefaultValue(false);

            builder.Entity<TLIrepeaterType>().Property(x => x.Deleted).HasDefaultValue(false);

            builder.Entity<TLIoduInstallationType>().Property(x => x.Deleted).HasDefaultValue(false);

            builder.Entity<TLIsideArmInstallationPlace>().Property(x => x.Deleted).HasDefaultValue(false);

            builder.Entity<TLIdataType>().Property(x => x.Deleted).HasDefaultValue(false);

            builder.Entity<TLIoperation>().Property(x => x.Deleted).HasDefaultValue(false);

            builder.Entity<TLIlogicalOperation>().Property(x => x.Deleted).HasDefaultValue(false);

            builder.Entity<TLIenforcmentCategory>().Property(x => x.Deleted).HasDefaultValue(false);

            builder.Entity<TLIpowerType>().Property(x => x.Delete).HasDefaultValue(false);

            builder.Entity<TLIdynamicListValues>().Property(x => x.Delete).HasDefaultValue(false);

            builder.Entity<TLIdynamicListValues>().Property(x => x.Disable).HasDefaultValue(false);

            builder.Entity<TLIsideArmType>().Property(x => x.Deleted).HasDefaultValue(false);

            builder.Entity<TLIsideArmType>().Property(x => x.Disable).HasDefaultValue(false);

            builder.Entity<TLIdependency>().Property(x => x.IsResult).HasDefaultValue(false);

            builder.Entity<TLIeditableManagmentView>().HasIndex(x => new { x.View, x.CivilWithoutLegCategoryId }).IsUnique();

            builder.Entity<TLImwBU>().Property(x => x.Active).HasDefaultValue(true);

            builder.Entity<TLIattachedFiles>().Property(x => x.UnAttached).HasDefaultValue(false);

            builder.Entity<TLIgroup>()
              .HasOne(a => a.Parent)
              .WithOne()
              .HasForeignKey<TLIgroup>(e => e.ParentId);

            builder.Entity<TLIgroup>()
              .HasOne(a => a.Upper)
              .WithOne()
              .HasForeignKey<TLIgroup>(e => e.UpperId);


        }
        public virtual async Task<int> SaveChangesAsync(string userId = null)
        {
            //OnBeforeSaveChanges(userId);
            var result = await base.SaveChangesAsync();
            return result;
        }
        //private void OnBeforeSaveChanges(string userId)
        //{
        //    ChangeTracker.DetectChanges();
        //    var auditEntries = new List<LogViewModel>();
        //    foreach (var entry in ChangeTracker.Entries())
        //    {
        //        if (entry.Entity is TLIlog || entry.State == EntityState.Detached || entry.State == EntityState.Unchanged)
        //            continue;
        //        var auditEntry = new LogViewModel(entry);
        //        auditEntry.TableName = entry.Entity.GetType().Name;
        //        auditEntry.UserId = userId;
        //        auditEntries.Add(auditEntry);
        //        foreach (var property in entry.Properties)
        //        {
        //            string propertyName = property.Metadata.Name;
        //            if (property.Metadata.IsPrimaryKey())
        //            {
        //                auditEntry.KeyValues[propertyName] = property.CurrentValue;
        //                continue;
        //            }

        //            switch (entry.State)
        //            {
        //                case EntityState.Added:
        //                    auditEntry.AuditType = AuditType.Create;
        //                    auditEntry.NewValues[propertyName] = property.CurrentValue;
        //                    break;

        //                case EntityState.Deleted:
        //                    auditEntry.AuditType = AuditType.Delete;
        //                    auditEntry.OldValues[propertyName] = property.OriginalValue;
        //                    break;

        //                case EntityState.Modified:
        //                    if (property.IsModified)
        //                    {
        //                        auditEntry.ChangedColumns.Add(propertyName);
        //                        auditEntry.AuditType = AuditType.Update;
        //                        auditEntry.OldValues[propertyName] = property.OriginalValue;
        //                        auditEntry.NewValues[propertyName] = property.CurrentValue;
        //                    }
        //                    break;
        //            }
        //        }
        //    }
        //    foreach (var auditEntry in auditEntries)
        //    {
        //        TLIlog.Add(auditEntry.ToAudit());
        //    }
        //}

    }



}
