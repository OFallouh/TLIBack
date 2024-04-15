using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModels.ConditionTypeDTOs;
using TLIS_DAL.ViewModels.SiteDTOs;
using TLIS_DAL.ViewModels.CityDTOs;
using TLIS_DAL.ViewModels.LegDTOs;
using TLIS_DAL.ViewModels.CivilWithoutLegDTOs;
using TLIS_DAL.ViewModels.CivilWithLegDTOs;
using TLIS_DAL.ViewModels.CivilNonSteelDTOs;
using TLIS_DAL.ViewModels.PowerDTOs;
using TLIS_DAL.ViewModels.MW_RFUDTOs;
using TLIS_DAL.ViewModels.MW_ODUDTOs;
using TLIS_DAL.ViewModels.MW_BULibraryDTOs;
using TLIS_DAL.ViewModels.MW_DishLbraryDTOs;
using TLIS_DAL.ViewModels.SupportTypeImplementedDTOs;
//using TLIS_DAL.ViewModels.CategoryDTOs;
using TLIS_DAL.ViewModels.PolarityTypeDTOs;
using TLIS_DAL.ViewModels.SideArmLibraryDTOs;
using TLIS_DAL.ViewModels.PermissionDTOs;
using TLIS_DAL.ViewModels.RoleDTOs;
using TLIS_DAL.ViewModels.RolePermissionDTOs;
using TLIS_DAL.ViewModels.UserPermissionDTOs;
using TLIS_DAL.ViewModels.UserDTOs;
using TLIS_DAL.ViewModels.DynamicAttDTOs;
using TLIS_DAL.ViewModels.GroupDTOs;
using TLIS_DAL.ViewModels.TaskStatusDTOs;
using TLIS_DAL.ViewModels.CivilWithLegsDTOs;
using TLIS_DAL.ViewModels.DynamicAttInstValueDTOs;
using TLIS_DAL.ViewModels.DynamicAttLibValueDTOs;
using TLIS_DAL.ViewModels.CivilWithoutLegCategoryDTOs;
using TLIS_DAL.ViewModels.BaseCivilWithLegsTypeDTOs;
using TLIS_DAL.ViewModels.OwnerDTOs;
using TLIS_DAL.ViewModels.GuyLineTypeDTOs;
using TLIS_DAL.ViewModelBase;
using TLIS_DAL.ViewModels.ConditionDTOs;
using TLIS_DAL.ViewModels.RadioRRULibraryDTOs;
using TLIS_DAL.ViewModels.RadioAntennaLibraryDTOs;
using TLIS_DAL.ViewModels.RadioOtherLibraryDTOs;
using TLIS_DAL.ViewModels.OptionDTOs;
using TLIS_DAL.ViewModels.SubOptionDTOs;
using TLIS_DAL.ViewModels.ItemStatusDTOs;
using TLIS_DAL.ViewModels.CapacityDTOs;
using TLIS_DAL.ViewModels.TelecomTypeDTOs;
using TLIS_DAL.ViewModels.CabinetPowerTypeDTOs;
using TLIS_DAL.ViewModels.SolarLibraryDTOs;
using TLIS_DAL.ViewModels.GeneratorLibraryDTOs;
using TLIS_DAL.ViewModels.CabinetTelecomLibraryDTOs;
using TLIS_DAL.ViewModels.CabinetPowerLibraryDTOs;
using TLIS_DAL.ViewModels.ActorDTOs;
using TLIS_DAL.ViewModels.BaseGeneratorTypeDTOs;
using TLIS_DAL.ViewModels.GeneratorDTOs;
using TLIS_DAL.ViewModels.SolarDTOs;
using TLIS_DAL.ViewModels.RenewableCabinetTypeDTOs;
using TLIS_DAL.ViewModels.CabinetDTOs;
using TLIS_DAL.ViewModels.OtherInventoryDistanceDTOs;
using TLIS_DAL.ViewModels.OtherInSiteDTOs;
using TLIS_DAL.ViewModels.HistoryTypeDTOs;
using TLIS_DAL.ViewModels.TablesHistoryDTOs;
using TLIS_DAL.ViewModels.HistoryDetails;
using TLIS_DAL.ViewModels.SiteStatusDTOs;
using TLIS_DAL.ViewModels.DiversityTypeDTOs;
using TLIS_DAL.ViewModels.SupportTypeDesignedDTOs;
using TLIS_DAL.ViewModels.StructureTypeDTOs;
using TLIS_DAL.ViewModels.InstCivilwithoutLegsTypeDTOs;
using TLIS_DAL.ViewModels.AttActivatedCategoryDTOs;
using TLIS_DAL.ViewModels.SectionsLegTypeDTOs;
using TLIS_DAL.ViewModels.BoardTypeDTOs;
using TLIS_DAL.ViewModels.LogisticalTypeDTOs;
using TLIS_DAL.ViewModels.PolarityOnLocationDTOs;
using TLIS_DAL.ViewModels.ItemConnectToDTOs;
using TLIS_DAL.ViewModels.RepeaterTypeDTOs;
using TLIS_DAL.ViewModels.OduInstallationTypeDTOs;
using TLIS_DAL.ViewModels.SideArmInstallationPlaceDTOs;
using TLIS_DAL.ViewModels.SideArmDTOs;
using TLIS_DAL.ViewModels.MW_DishDTOs;
using TLIS_DAL.ViewModels.MW_BUDTOs;
using TLIS_DAL.ViewModels.OperationDTOs;
using TLIS_DAL.ViewModels.LogicalOperationDTOs;
using TLIS_DAL.ViewModels.DependencyDTOs;
using TLIS_DAL.ViewModels.RuleDTOs;
using TLIS_DAL.ViewModels.MailTemplateDTOs;
using TLIS_DAL.ViewModels.OrderStatusDTOs;
using TLIS_DAL.ViewModels.WorkFlowDTOs;
using TLIS_DAL.ViewModels.MW_OtherLibraryDTOs;
using TLIS_DAL.ViewModels.LoadOtherLibraryDTOs;
using TLIS_DAL.ViewModels.StepDTOs;
using TLIS_DAL.ViewModels.ActionDTOs;
using TLIS_DAL.ViewModels.StepActionDTOs;
using TLIS_DAL.ViewModels.MW_PortDTOs;
using TLIS_DAL.ViewModels.ActionItemOptionDTOs;
using TLIS_DAL.ViewModels.DataTypeDTOs;
using TLIS_DAL.ViewModels.AttachedFilesDTOs;
using TLIS_DAL.ViewModels.WorkFlowTypeDTOs;
using TLIS_DAL.ViewModels.WorkFlowGroupDTOs;
using TLIS_DAL.ViewModels.EnforcmentCategoryDTOs;
using TLIS_DAL.ViewModels.GroupRoleDTOs;
using TLIS_DAL.ViewModels.PowerTypeDTOs;
using TLIS_DAL.ViewModels.AttributeActivatedDTOs;
using TLIS_DAL.ViewModels.DynamicListValuesDTOs;
using TLIS_DAL.ViewModels.CivilLoadsDTOs;
using TLIS_DAL.ViewModels.PartDTOs;
using TLIS_DAL.ViewModels.TicketDTOs;
using TLIS_DAL.ViewModels.RadioAntennaDTOs;
using TLIS_DAL.ViewModels.RadioRRUDTOs;
using TLIS_DAL.ViewModels.TicketActionDTOs;
using TLIS_DAL.Helper.Filters;
using TLIS_DAL.ViewModels.IntegrationDTOs;
using TLIS_DAL.ViewModels.AgendaDTOs;
using TLIS_DAL.ViewModels.RadioOtherDTOs;
using TLIS_DAL.ViewModels.LoadOtherDTOs;
using TLIS_DAL.ViewModels.AttributeViewManagmentDTOs;
using TLIS_DAL.ViewModels.EditableManagmentViewDTOs;
using TLIS_DAL.ViewModels.WorkflowHistoryDTOs;
using TLIS_DAL.ViewModels.LogisticalDTOs;
using TLIS_DAL.ViewModels.TablePartNameDTOs;
using TLIS_DAL.ViewModels.SideArmTypeDTOs;
using TLIS_DAL.ViewModels.TicketTargetDTOs;
using TLIS_DAL.ViewModels.TicketOptionNoteDTOs;
using TLIS_DAL.ViewModels.BaseBUDTOs;
using TLIS_DAL.ViewModels.AllLoadInstDTOs;
using TLIS_DAL.ViewModels.DocumentTypeDTOs;
using TLIS_DAL.ViewModels.LocationTypeDTOs;
using TLIS_DAL.ViewModels.LogErrorDTOs;
using TLIS_DAL.ViewModels.Mw_OtherDTOs;
using TLIS_DAL.ViewModels.SubTypeDTOs;
using TLIS_DAL.ViewModels.CivilSteelSupportCategoryDTOs;
using TLIS_DAL.ViewModels.CivilNonSteelLibraryDTOs;
using TLIS_DAL.ViewModels.AsTypeDTOs;
using TLIS_DAL.ViewModels.ParityDTOs;
using TLIS_DAL.ViewModels.BaseTypeDTOs;
using TLIS_DAL.ViewModels.InstallationPlaceDTOs;
using TLIS_DAL.ViewModels.ImportSheetDtos;
using TLIS_DAL.ViewModels.AllCivilInstDTOs;
using TLIS_DAL.ViewModels.RegionDTOs;
using TLIS_DAL.ViewModels.AreaDTOs;
using TLIS_DAL.ViewModels.NewPermissionsDTOs.Permissions;
using static TLIS_DAL.ViewModels.CivilWithLegLibraryDTOs.AddCivilWithLegsLibraryObject;
using TLIS_DAL.ViewModels.CivilWithLegLibraryDTOs;
using static TLIS_DAL.ViewModels.CivilWithLegLibraryDTOs.EditCivilWithLegsLibraryObject;
using static TLIS_DAL.ViewModels.CivilWithoutLegLibraryDTOs.AddCivilWithoutLegsLibraryObject;
using TLIS_DAL.ViewModels.CivilWithoutLegLibraryDTOs;
using static TLIS_DAL.ViewModels.CivilWithoutLegLibraryDTOs.EditCivilWithoutLegsLibraryObject;
using static TLIS_DAL.ViewModels.CivilWithoutLegDTOs.EditCivilWithoutLegsInstallationObject;

namespace TLIS_DAL.ViewModels
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {


            //CreateMap<ConditionType, ConditionTypeViewModel>().ReverseMap();
           // ---------------------------------------------------------------------------------------

            CreateMap<LocationTypeViewModel, TLIcivilSteelSupportCategory>().ReverseMap();
            CreateMap<StructureTypeViewModel, TLIinstallationCivilwithoutLegsType>().ReverseMap();
            CreateMap<AddDdynamicAttributeInstallationValueViewModel, TLIdynamicAttLibValue>().ReverseMap();
            CreateMap<AddDdynamicAttributeInstallationValueViewModel, TLIdynamicAtt>().ReverseMap();
            CreateMap<AddDdynamicAttributeInstallationValueViewModel, AddCivilWithoutLegLibraryViewModel>().ReverseMap();
            CreateMap<TLIcivilWithLegLibrary, CivilWihtLegsLibraryAttributes>().ReverseMap();
            CreateMap<LocationTypeViewModel, TLIinstallationCivilwithoutLegsType>().ReverseMap();
            CreateMap<LocationTypeViewModel, TLIcivilWithoutLegCategory>().ReverseMap();
            CreateMap<LocationTypeViewModel, TLIsectionsLegType>().ReverseMap();
            CreateMap<LocationTypeViewModel, TLIsupportTypeDesigned>().ReverseMap();
            CreateMap<LocationTypeViewModel, TLIcivilNonSteelType>().ReverseMap();
            CreateMap<LocationTypeViewModel, TLIstructureType>().ReverseMap();
            CreateMap<LocationTypeViewModel, TLIcivilWithLegs>().ReverseMap();
            CreateMap<LocationTypeViewModel, TLIcivilSiteDate>().ReverseMap()
                .ForMember(x => x.Name, x => x.MapFrom(f => f.allCivilInst.civilWithLegs.Name));
            CreateMap<EditCivilWithLegsInstallationObject, TLIcivilWithLegs>().ReverseMap();
            CreateMap<EditinstallationAttributesCivilWithLegs, TLIcivilWithLegs>().ReverseMap();
            CreateMap<CivilWihtoutLegsLibraryAttributes, TLIcivilWithoutLegLibrary>().ReverseMap();
            CreateMap<AddCivilWithoutLegsLibraryObject, TLIcivilWithoutLegLibrary>().ReverseMap();
            CreateMap<EditCivilWihtoutLegsLibraryAttributes, TLIcivilWithoutLegLibrary>().ReverseMap();


            //---------------------------------------------------------------------------



            CreateMap<TLIcity, CityViewModel>().ReverseMap();
            CreateMap<BaseInstAttViewDynamic, TLIdynamicAtt>().ReverseMap()
                .ForMember(x => x.DataType, x => x.MapFrom(f => f.DataType.Name))
                  .ForMember(x => x.Label, x => x.MapFrom(f => f.Key));
            CreateMap<TLIbaseBU, BaseBUViewModel>().ReverseMap();
            CreateMap<TLIbaseBU, AddBaseBUViewModel>().ReverseMap();
            CreateMap<TLIbaseBU, EditBaseBUViewModel>().ReverseMap();
            CreateMap<TLIimportSheet, ImportSheetViewModel>().ReverseMap();

            CreateMap<TLIlogUsersActions, LogUsersActionsViewModel>().ReverseMap();

            CreateMap<AddCityViewModel, TLIcity>().ForMember(cit =>
            cit.Name,
            citV => citV.MapFrom(cit => cit.Name));

            CreateMap<TLIleg, LegViewModel>().ReverseMap();
            CreateMap<TLIleg, AddLegViewModel>().ReverseMap();
            CreateMap<TLIleg, EditLegViewModel>().ReverseMap();
            CreateMap<TLIcivilWithoutLeg, installationAttributesCivilWithoutLegs>().ReverseMap();
            CreateMap<TLIcivilNonSteel, installationAttributesCivilNonSteelLegs>().ReverseMap();

            CreateMap<TLIcivilNonSteelType, CivilNonSteelTypeViewModel>().ReverseMap();

            CreateMap<TLIcivilWithoutLegLibrary, CivilWithoutLegLibraryViewModel>()
                .ForMember(c => c.CivilSteelSupportCategory_Name, c => c.MapFrom(s => s.CivilSteelSupportCategory.Name))
                .ForMember(c => c.InstCivilwithoutLegsType_Name, c => c.MapFrom(s => s.InstCivilwithoutLegsType.Name))
                .ForMember(c => c.CivilWithoutLegCategory_Name, c => c.MapFrom(s => s.CivilWithoutLegCategory.Name))
                .ForMember(c => c.structureType_Name, c => c.MapFrom(s => s.structureType.Name))
                .ForMember(c => c.Manufactured_Max_Load, c => c.MapFrom(s => s.Manufactured_Max_Load));

            CreateMap<TLIcivilWithoutLegLibrary, AddCivilWithoutLegLibraryViewModel>().ReverseMap();

            CreateMap<TLIcivilWithoutLegLibrary, EditCivilWithoutLegLibraryViewModel>().ReverseMap();

            CreateMap<TLIcivilWithoutLegLibrary, LibraryNamesViewModel>().ReverseMap();

            CreateMap<TLIcivilWithLegLibrary, CivilWithLegLibraryViewModel>()
                .ForMember(c => c.SupportTypeDesigned_Name, c => c.MapFrom(s => s.supportTypeDesigned.Name))
                .ForMember(c => c.SectionsLegType_Name, c => c.MapFrom(s => s.sectionsLegType.Name))
                .ForMember(c => c.StructureType_Name, c => c.MapFrom(s => s.structureType.Name))
                .ForMember(c => c.CivilSteelSupportCategory_Name, c => c.MapFrom(s => s.civilSteelSupportCategory.Name));

            CreateMap<TLIcivilWithLegLibrary, AddCivilWithLegLibraryViewModel>().ReverseMap();

            CreateMap<TLIcivilWithLegLibrary, EditCivilWithLegLibraryViewModels>().ReverseMap();
            CreateMap<TLIsite, EditSiteViewModel>().ReverseMap();
            CreateMap<TLIsite, SiteForGetViewModel>().ReverseMap();
            CreateMap<TLIsite, GetSiteNameBySitCode>().ReverseMap();
            CreateMap<TLIcivilWithLegLibrary, LibraryNamesViewModel>().ReverseMap();

            CreateMap<CivilWithLegLibraryViewModel, EditCivilWithLegLibraryViewModels>().ReverseMap();

            CreateMap<TLIcivilNonSteelLibrary, CivilNonSteelLibraryViewModel>()
                .ForMember(c => c.civilNonSteelType_Name, c => c.MapFrom(x => x.civilNonSteelType.Name));


            CreateMap<CivilNonSteelLibraryViewModel, TLIcivilNonSteelLibrary>();

            CreateMap<TLIcivilNonSteelLibrary, AddCivilNonSteelLibraryViewModel>().ReverseMap();

            CreateMap<TLIcivilNonSteelLibrary, EditCivilNonSteelLibraryViewModel>().ReverseMap();
            CreateMap<TLIticketTarget, TicketTargetViewModel>().ReverseMap();
            CreateMap<TLIticketOptionNote, TicketOptionNoteViewModel>().ReverseMap();

            CreateMap<TLIcivilNonSteelLibrary, LibraryNamesViewModel>().ReverseMap();

            CreateMap<TLIpowerLibrary, PowerLibraryViewModel>().ReverseMap();

            CreateMap<TLIpowerLibrary, AddPowerLibraryViewModel>().ReverseMap();

            CreateMap<TLIpowerLibrary, EditPowerLibraryViewModel>().ReverseMap();

            CreateMap<TLImwRFULibrary, MW_RFULibraryViewModel>()
                .ForMember(m => m.diversityType_Name, m => m.MapFrom(s => s.diversityType.Name))
                .ForMember(m => m.boardType_Name, m => m.MapFrom(s => s.boardType.Name));

            CreateMap<TLImwRFULibrary, AddMW_RFULibraryViewModel>().ReverseMap();

            CreateMap<TLImwRFULibrary, EditMW_RFULibraryViewModel>().ReverseMap();

            CreateMap<TLImwODULibrary, MW_ODULibraryViewModel>()
                .ForMember(m => m.parity_Name, m => m.MapFrom(s => s.parity.Name));

            CreateMap<TLImwODULibrary, AddMW_ODULibraryViewModel>().ReverseMap();

            CreateMap<TLImwODULibrary, EditMW_ODULibraryViewModel>().ReverseMap();

            CreateMap<TLImwBULibrary, MW_BULibraryViewModel>()
                .ForMember(m => m.diversityType_Name, m => m.MapFrom(s => s.diversityType.Name));

            CreateMap<TLImwBULibrary, AddMW_BULibraryViewModel>().ReverseMap();

            CreateMap<TLImwBULibrary, EditMW_BULibraryViewModel>().ReverseMap();

            CreateMap<TLImwDishLibrary, MW_DishLibraryViewModel>()
                .ForMember(m => m.asType_Name, m => m.MapFrom(s => s.asType.Name))
                .ForMember(m => m.polarityType_Name, m => m.MapFrom(s => s.polarityType.Name));

            CreateMap<TLImwDishLibrary, AddMW_DishLibraryViewModel>().ReverseMap();

            CreateMap<TLImwDishLibrary, EditMW_DishLibraryViewModel>().ReverseMap();

            CreateMap<TLIsupportTypeImplemented, SupportTypeImplementedViewModel>().ReverseMap();

            CreateMap<TLIsupportTypeImplemented, AddSupportTypeImplementedViewModel>().ReverseMap();

            CreateMap<TLIsupportTypeImplemented, EditSupportTypeImplementedViewModel>().ReverseMap();

            //CreateMap<TLIcategory, CategoryViewModel>().ReverseMap();

            CreateMap<TLIpolarityType, PolarityTypeViewModel>().ReverseMap();

            CreateMap<TLItablePartName, TablePartNameViewModel>().ReverseMap();

            CreateMap<TLIsideArmLibrary, SideArmLibraryViewModel>().ReverseMap();

            CreateMap<AddSideArmLibraryViewModel, TLIsideArmLibrary>();

            CreateMap<EditSideArmLibraryViewModel, TLIsideArmLibrary>();

            CreateMap<PermissionViewModel, TLIpermission>().ReverseMap();

            CreateMap<AddPermissionViewModel, TLIpermission>().ReverseMap();

            CreateMap<TLIrole, RoleViewModel>().ReverseMap();

            CreateMap<TLIrole, AddRoleViewModel>();

            CreateMap<BaseAttView, BaseInstAttView>().ReverseMap();

            CreateMap<TLIrole, EditRoleViewModel>();

            CreateMap<TLIgroup, GroupViewModel>()
                 .ForMember(x => x.ActorName, x => x.MapFrom(s => s.Actor.Name))
                 .ForMember(x => x.ParentName, x => x.MapFrom(s => s.Parent.Select(x=>x.Name)))
                 .ForMember(x => x.UpperName, x => x.MapFrom(s => s.Upper.Name));

            CreateMap<TLIgroup, GroupNamesViewModel>()
                .ForMember(x => x.Id, x => x.MapFrom(y => y.Id))
                .ForMember(x => x.Name, x => x.MapFrom(y => y.Name));

            CreateMap<GroupViewModel, TLIgroup>();

            CreateMap<TLIgroup, AddGroupViewModel>().ReverseMap();

            CreateMap<RolePermissionViewModel, TLIrolePermission>().ReverseMap();

            CreateMap<AddRolePermissionViewModel, TLIrolePermission>();

            CreateMap<EditRolePermissionViewModel, TLIrolePermission>();

            CreateMap<UserPermissionViewModel, TLIuserPermission>().ReverseMap();

            CreateMap<EditUserPermissionViewModel, TLIuserPermission>();

            CreateMap<UserViewModel, TLIuser>().ReverseMap();

            CreateMap<TLIdynamicAtt, AddDynamicAttViewModel>()
                .ForMember(x => x.dynamicListValues, x => x.Ignore());

            CreateMap<AddDynamicAttViewModel, TLIdynamicAtt>()
                .ForMember(x => x.dependencies, x => x.Ignore())
                .ForMember(x => x.dynamicAttInstValues, x => x.Ignore())
                .ForMember(x => x.dynamicAttLibValues, x => x.Ignore())
                .ForMember(x => x.dynamicListValues, x => x.Ignore());

            CreateMap<TLIdynamicAtt, EditDynamicAttViewModel>()
                .ForMember(x => x.CivilWithoutLegCategory_Name, x => x.MapFrom(d => d.CivilWithoutLegCategory.Name))
                .ForMember(x => x.DataType_Name, x => x.MapFrom(d => d.DataType.Name))
                .ForMember(x => x.tablesNames_Name, x => x.MapFrom(d => d.tablesNames.TableName));

            CreateMap<EditDynamicAttViewModel, TLIdynamicAtt>();

            CreateMap<TLIdynamicAtt, DynamicAttViewModel>()
                .ForMember(x => x.DataType_Name, x => x.MapFrom(d => d.DataType.Name))
                .ForMember(x => x.CivilWithoutLegCategory_Name, x => x.MapFrom(d => d.CivilWithoutLegCategory.Name))
                .ForMember(x => x.tablesNames_Name, x => x.MapFrom(d => d.tablesNames.TableName));
            CreateMap<DynamicAttViewModel, TLIdynamicAtt>()
                .ForMember(x => x.DataType, x => x.Ignore());

            CreateMap<AddDynamicAttViewModel, TLIdynamicAtt>()
                .ForMember(x => x.dynamicListValues, x => x.Ignore());

            CreateMap<AddDynamicLibAttValueViewModel, TLIdynamicAtt>().ReverseMap();

            CreateMap<TLIallLoadInst, AllLoadInstViewModel>().ReverseMap();

            CreateMap<AddDynamicLibAttValueViewModel, TLIdynamicAttLibValue>().ReverseMap();

            CreateMap<AddDynamicAttInstViewModel, TLIdynamicAtt>().ReverseMap();

            CreateMap<DynamicAttLibValueViewMdodel, TLIdynamicAttLibValue>().ReverseMap();

            CreateMap<DynamicAttLibViewModel, TLIdynamicAttLibValue>();

            CreateMap<TLIdynamicAttLibValue, DynamicAttLibViewModel>();

            CreateMap<AddUserViewModel, TLIuser>().ReverseMap();

            CreateMap<EditUserViewModel, TLIuser>().ReverseMap();

            CreateMap<TaskStatusViewModel, TLItaskStatus>().ReverseMap();

            CreateMap<AddTaskStatusViewModel, TLItaskStatus>().ReverseMap();

            CreateMap<TLIcivilWithLegs, CivilWithLegsViewModel>()
                .ForMember(x => x.Owner_Name, x => x.MapFrom(f => f.Owner.OwnerName))
                .ForMember(x => x.baseType_Name, x => x.MapFrom(f => f.baseType.Name))
                .ForMember(x => x.CivilWithLegsLib_Name, x => x.MapFrom(f => f.CivilWithLegsLib.Model))
                .ForMember(x => x.BaseCivilWithLegType_Name, x => x.MapFrom(f => f.BaseCivilWithLegType.Name))
                .ForMember(x => x.GuyLineType_Name, x => x.MapFrom(f => f.GuyLineType.Name))
                .ForMember(x => x.enforcmentCategory_Name, x => x.MapFrom(f => f.enforcmentCategory.Name))
                .ForMember(x => x.locationType_Name, x => x.MapFrom(f => f.locationType.Name))
                .ForMember(x => x.SupportTypeImplemented_Name, x => x.MapFrom(f => f.SupportTypeImplemented.Name));
            CreateMap<TLIcivilWithLegs, AddCivilWithLegsViewModel>().ReverseMap();

            CreateMap<TLIcivilWithLegs, EditCivilWithLegsViewModel>().ReverseMap();

            CreateMap<TLIcivilWithoutLegCategory, CivilWithoutLegCategoryViewModel>().ReverseMap();

            CreateMap<TLIinternalApis, IntegrationViewModel>().ForMember(dst => dst.Id, opt => opt.MapFrom(src => src.Id))
             .ForMember(dst => dst.Name, opt => opt.MapFrom(src => src.Label));



            CreateMap<TLIcivilWithoutLegCategory, AddCivilWithoutLegCategoryViewModel>().ReverseMap();

            CreateMap<BaseCivilWithLegsTypeViewModel, TLIbaseCivilWithLegsType>().ReverseMap();

            CreateMap<AddBaseCivilWithLegsTypeViewModel, TLIbaseCivilWithLegsType>().ReverseMap();

            CreateMap<EditBaseCivilWithLegsTypeViewModel, TLIbaseCivilWithLegsType>().ReverseMap();

            CreateMap<TLIsite, SiteViewModel>()
                .ForMember(dst => dst.Area, opt => opt.MapFrom(src => src.Area.AreaName))
                .ForMember(dst => dst.Region, opt => opt.MapFrom(src => src.Region.RegionName))
                .ForMember(x => x.Status, y => y.MapFrom(s => s.siteStatus.Name))
                .ForMember(x => x.CityName, y => y.MapFrom(s => s.Zone));

            CreateMap<TLIsite, SiteViewModelForGetAll>()
                .ForMember(dst => dst.Area, opt => opt.MapFrom(src => src.Area.AreaName))
                .ForMember(dst => dst.Region, opt => opt.MapFrom(src => src.Region.RegionName))
                .ForMember(x => x.Status, y => y.MapFrom(s => s.siteStatus.Name))
                .ForMember(x => x.CityName, y => y.MapFrom(s => s.Zone));

            CreateMap<TLIsite, AddSiteViewModel>().ReverseMap();

            CreateMap<TLIowner, OwnerViewModel>()
                .ForMember(x => x.Name, x => x.MapFrom(f => f.OwnerName));
            CreateMap<OwnerViewModel, TLIowner>();

            CreateMap<AddOwnerViewModel, TLIowner>().ReverseMap();

            CreateMap<EditOwnerViewModel, TLIowner>().ReverseMap();



            CreateMap<GuyLineTypeViewModel, TLIguyLineType>().ReverseMap();

            CreateMap<AddGuyLineTypeViewModel, TLIguyLineType>().ReverseMap();

            CreateMap<EditGuyLineTypeViewModel, TLIguyLineType>().ReverseMap();

            CreateMap<TLIcivilWithoutLeg, CivilWithoutLegViewModel>()
                .ForMember(x => x.Owner_Name, x => x.MapFrom(f => f.Owner.OwnerName))
                .ForMember(x => x.civilWithoutLegsLib_Name, x => x.MapFrom(f => f.CivilWithoutlegsLib.Model))
                .ForMember(x => x.subType_Name, x => x.MapFrom(f => f.subType.Name));


            CreateMap<AddCivilWithoutLegViewModel, TLIcivilWithoutLeg>().ReverseMap();

            CreateMap<TLIcivilWithoutLeg, EditCivilWithoutLegViewModel>().ReverseMap();
            CreateMap<TLIcivilNonSteel, CivilNonSteelViewModel>()
                .ForMember(x => x.CivilNonsteelLibrary_Name, x => x.MapFrom(f => f.CivilNonsteelLibrary.Model))
                .ForMember(x => x.owner_Name, x => x.MapFrom(f => f.owner.OwnerName))
                .ForMember(x => x.supportTypeImplemented_Name, x => x.MapFrom(f => f.supportTypeImplemented.Name))
                .ForMember(x => x.locationType_Name, x => x.MapFrom(f => f.locationType.Name));

            CreateMap<CivilNonSteelViewModel, TLIcivilNonSteel>();
            CreateMap<AddCivilNonSteelViewModel, TLIcivilNonSteel>().ReverseMap();

            CreateMap<EditCivilNonSteelViewModel, TLIcivilNonSteel>().ReverseMap();
            CreateMap<EditCivilWithLegsLibraryObject, TLIcivilWithLegLibrary>().ReverseMap();
            CreateMap<EditCivilWihtLegsLibraryAttributes, TLIcivilWithLegLibrary>().ReverseMap();

            CreateMap<TLIradioRRULibrary, RadioRRULibraryViewModel>().ReverseMap();

            CreateMap<EditRadioRRULibraryViewModel, TLIradioRRULibrary>().ReverseMap();

            CreateMap<AddRadioRRULibraryViewModel, TLIradioRRULibrary>().ReverseMap();

            CreateMap<TLIradioAntennaLibrary, RadioAntennaLibraryViewModel>().ReverseMap();

            CreateMap<TLIradioAntennaLibrary, AddRadioAntennaLibraryViewModel>().ReverseMap();

            CreateMap<TLIradioAntennaLibrary, EditRadioAntennaLibraryViewModel>().ReverseMap();

            CreateMap<TLIradioOtherLibrary, RadioOtherLibraryViewModel>().ReverseMap();

            CreateMap<TLIradioOtherLibrary, AddRadioOtherLibraryViewModel>().ReverseMap();

            CreateMap<TLIradioOtherLibrary, EditRadioOtherLibraryViewModel>().ReverseMap();

            CreateMap<TLIdocumentType, DocumentTypeViewModel>().ReverseMap();

            CreateMap<TLIdocumentType, AddDocumentTypeViewModel>().ReverseMap();

            CreateMap<ConfigurationAttsViewModel, AddDocumentTypeViewModel>().ReverseMap();

            CreateMap<ConfigurationAttsViewModel, TLIdocumentType>().ReverseMap();


            CreateMap<BaseInstAttViews, BaseAttView>().ReverseMap();
            CreateMap<BaseInstAttViews, TLIlogistical>().ReverseMap();

            CreateMap<TLIradioOther, RadioOtherViewModel>()
                .ForMember(x => x.installationPlace_Name, x => x.MapFrom(f => f.installationPlace.Name))
                .ForMember(x => x.owner_Name, x => x.MapFrom(f => f.owner.OwnerName))
                .ForMember(x => x.radioOtherLibrary_Name, x => x.MapFrom(f => f.radioOtherLibrary.Model));

            CreateMap<TLIradioOther, AddRadioOtherViewModel>()
                .ForMember(x => x.TLIcivilLoads, x => x.Ignore());

            CreateMap<AddRadioOtherViewModel, TLIradioOther>();

            CreateMap<TLIradioOther, EditRadioOtherViewModel>();

            CreateMap<installationAttributesCivilWithLegs, TLIcivilWithLegs>();

            CreateMap<EditRadioOtherViewModel, TLIradioOther>();
            // CreateMap<TLIcondition, ConditionViewModel>().ReverseMap();
            CreateMap<OptionViewModel, TLIactionOption>()
                //.ForMember(c => c.NextStepAction, c => c.Ignore())
                .ForMember(c => c.DateDeleted, c => c.Ignore())
                .BeforeMap((s, d) => d.Deleted = false)
                .ReverseMap();
            CreateMap<ListActionOptionViewModel, TLIactionOption>()
                //.ForMember(c => c.NextStepAction, c => c.Ignore())
                .ForMember(c => c.DateDeleted, c => c.Ignore())
                .BeforeMap((s, d) => d.Deleted = false)
                .ReverseMap();
            CreateMap<OptionViewModel, ListActionOptionViewModel>()
                .ForMember(c => c.SubOptions, c => c.Ignore())
                .ReverseMap();
            CreateMap<ListActionItemOptionViewModel, TLIactionItemOption>()
                .ForMember(c => c.DeleteDate, c => c.Ignore())
                .BeforeMap((s, d) => d.Deleted = false)
                ;
            //CreateMap<OptionViewModel, TLIactionOption>().ReverseMap();
            CreateMap<TLIsuboption, SubOptionViewModel>()
            .ForMember(c => c.OptionName, c => c.MapFrom(s => s.Option.Name));
            CreateMap<SubOptionViewModel, TLIsuboption>();

            CreateMap<TLIitemStatus, ItemStatusViewModel>().ReverseMap();
            CreateMap<TLIcivilWithLegs, SupportTypeImplementedViewModel>().ReverseMap();
            CreateMap<TLIcivilWithoutLeg, SupportTypeImplementedViewModel>().ReverseMap();
            CreateMap<TLIcivilNonSteel, SupportTypeImplementedViewModel>().ReverseMap();
            CreateMap<TLIcivilSiteDate, SupportTypeImplementedViewModel>().ReverseMap();
            CreateMap<TLIallCivilInst, SupportTypeImplementedViewModel>().ReverseMap();
            CreateMap<ListConditionActionViewModel, ActionListViewModel>()
                .ForMember(x => x.Proposal, x => x.Ignore())
                .ForMember(x => x.Type, x => x.Ignore())
                .ForMember(x => x.ActionOptions, x => x.Ignore())
                .ForMember(x => x.ActionItemOptions, x => x.Ignore())
                .ReverseMap();
            CreateMap<ListCivilDecisionActionViewModel, ActionListViewModel>()
                .ForMember(x => x.Proposal, x => x.Ignore())
                .ForMember(x => x.Type, x => x.Ignore())
                .ForMember(x => x.ActionOptions, x => x.Ignore())
                .ForMember(x => x.ActionItemOptions, x => x.Ignore())
                .ReverseMap();
            CreateMap<ListItemStatusViewModel, TLIitemStatus>()
                .ForMember(x => x.DeleteDate, x => x.Ignore())
                .BeforeMap((s, d) => d.Deleted = false)
                .ReverseMap();
            CreateMap<AddItemStatusViewModel, TLIitemStatus>()
                .ForMember(x => x.DeleteDate, x => x.Ignore())
                .BeforeMap((s, d) => d.Deleted = false)
                .BeforeMap((s, d) => d.Active = true)
                .ReverseMap();

            CreateMap<AddStepActionPartFileViewModel, AddStepActionPartViewModel>()
                .BeforeMap((s, d) => d.AllowUploadFile = true)
                .BeforeMap((s, d) => d.UploadFileIsMandatory = true)
                .ReverseMap();

            CreateMap<TLIcapacity, CapacityViewModel>().ReverseMap();

            CreateMap<TLItelecomType, TelecomTypeViewModel>().ReverseMap();

            CreateMap<TLItelecomType, AddTelecomTypeViewModel>().ReverseMap();

            CreateMap<TLItelecomType, EditTelecomTypeViewModel>().ReverseMap();

            CreateMap<TLIcabinetPowerType, CabinetPowerTypeViewModel>().ReverseMap();

            CreateMap<TLIsolarLibrary, SolarLibraryViewModel>()
                .ForMember(s => s.Capacity_Name, s => s.MapFrom(o => o.Capacity.Name));

            CreateMap<TLIsolarLibrary, AddSolarLibraryViewModel>().ReverseMap();

            CreateMap<TLIsolarLibrary, EditSolarLibraryViewModel>().ReverseMap();

            CreateMap<TLIgeneratorLibrary, GeneratorLibraryViewModel>()
                .ForMember(s => s.Capacity_Name, s => s.MapFrom(o => o.Capacity.Name));

            CreateMap<TLIgeneratorLibrary, AddGeneratorLibraryViewModel>().ReverseMap();

            CreateMap<TLIgeneratorLibrary, EditGeneratorLibraryViewModel>().ReverseMap();

            CreateMap<TLIcabinetTelecomLibrary, CabinetTelecomLibraryViewModel>()
                .ForMember(c => c.TelecomType_Name, c => c.MapFrom(a => a.TelecomType.Name));

            CreateMap<TLIcabinetTelecomLibrary, AddCabinetTelecomLibraryViewModel>().ReverseMap();

            CreateMap<TLIcabinetTelecomLibrary, EditCabinetTelecomLibraryViewModel>().ReverseMap();

            CreateMap<TLIcabinetPowerLibrary, CabinetPowerLibraryViewModel>()
                .ForMember(c => c.CabinetPowerType_Name, c => c.MapFrom(a => a.CabinetPowerType.Name));

            CreateMap<TLIcabinetPowerLibrary, AddCabinetPowerLibraryViewModel>().ReverseMap();

            CreateMap<TLIcabinetPowerLibrary, EditCabinetPowerLibraryViewModel>().ReverseMap();

            CreateMap<TLIactor, ActorViewModel>().ReverseMap();

            CreateMap<TLIactor, AddActorViewModel>().ReverseMap();

            CreateMap<TLIactor, EditActorViewModel>().ReverseMap();

            CreateMap<TLIbaseGeneratorType, BaseGeneratorTypeViewModel>().ReverseMap();

            CreateMap<TLIbaseGeneratorType, AddBaseGeneratorTypeViewModel>().ReverseMap();

            CreateMap<TLIbaseGeneratorType, EditBaseGeneratorTypeViewModel>().ReverseMap();

            CreateMap<TLIgenerator, GeneratorViewModel>()
                .ForMember(g => g.BaseGeneratorType_Name, g => g.MapFrom(s => s.BaseGeneratorType.Name))
                .ForMember(g => g.GeneratorLibrary_Name, g => g.MapFrom(s => s.GeneratorLibrary.Model));

            CreateMap<TLIgenerator, AddGeneratorViewModel>().ReverseMap();

            CreateMap<TLIgenerator, EditGeneratorViewModel>().ReverseMap();

            CreateMap<TLIsolar, SolarViewModel>()
                .ForMember(s => s.SolarLibrary_Name, s => s.MapFrom(o => o.SolarLibrary.Model))
                .ForMember(s => s.Cabinet_Name, s => s.MapFrom(o => o.Cabinet.Name));

            CreateMap<TLIsolar, AddSolarViewModel>().ReverseMap();

            CreateMap<TLIsolar, EditSolarViewModel>().ReverseMap();

            CreateMap<TLIrenewableCabinetType, RenewableCabinetTypeViewModel>().ReverseMap();

            CreateMap<TLIrenewableCabinetType, AddRenewableCabinetTypeViewModel>().ReverseMap();

            CreateMap<TLIrenewableCabinetType, EditRenewableCabinetTypeViewModel>().ReverseMap();

            CreateMap<TLIbaseType, BaseTypeViewModel>().ReverseMap();
            CreateMap<TLIbaseType, AddBaseTypeViewModel>().ReverseMap();
            CreateMap<TLIbaseType, EditBaseTypeViewModel>().ReverseMap();

            CreateMap<TLIcabinet, CabinetViewModel>()
                .ForMember(c => c.CabinetPowerLibrary_Name, c => c.MapFrom(f => f.CabinetPowerLibrary.Model))
                .ForMember(c => c.CabinetTelecomLibrary_Name, c => c.MapFrom(f => f.CabinetTelecomLibrary.Model))
                .ForMember(c => c.RenewableCabinetType_Name, c => c.MapFrom(f => f.RenewableCabinetType.Name));

            CreateMap<TLIcabinet, EditCabinetViewModel>().ReverseMap();

            CreateMap<TLIcabinet, AddCabinetViewModel>().ReverseMap();

            CreateMap<TLIotherInventoryDistance, OtherInventoryDistanceViewModel>().ReverseMap();
            //.ForMember(o => o.otherInventoryType_Name_1, o => o.MapFrom(t => t.otherInventoryType1.Name));
            //.ForMember(o => o.otherInventoryType_Name_2, o => o.MapFrom(t => t.otherInventoryType2.Name));

            CreateMap<TLIotherInventoryDistance, AddOtherInventoryDistanceViewModel>().ReverseMap();

            CreateMap<TLIotherInventoryDistance, EditOtherInventoryDistanceViewModel>().ReverseMap();

            CreateMap<TLIotherInSite, OtherInSiteViewModel>()
                .ForMember(o => o.SiteName, o => o.MapFrom(t => t.Site.SiteName));

            CreateMap<TLIotherInSite, AddOtherInSiteViewModel>().ReverseMap();

            CreateMap<TLIotherInSite, EditOtherInSiteViewModel>().ReverseMap();

            CreateMap<TLIhistoryType, HistoryTypeViewModel>().ReverseMap();

            CreateMap<TLItablesHistory, TablesHistoryViewModel>().ReverseMap();

            CreateMap<TLItablesHistory, AddTablesHistoryViewModel>().ReverseMap();

            CreateMap<TLItablesHistory, EditTablesHistoryViewModel>().ReverseMap();

            CreateMap<TLIhistoryDetails, HistoryDetailsViewModel>().ReverseMap();

            CreateMap<TLIhistoryDetails, AddHistoryDetailsViewModel>().ReverseMap();

            CreateMap<TLIhistoryDetails, EditHistoryDetailsViewModel>().ReverseMap();

            CreateMap<TLIlogistical, DropDownListFilters>()
                .ForMember(r => r.Value, r => r.MapFrom(s => s.Name));

            CreateMap<TLIrenewableCabinetType, DropDownListFilters>()
                .ForMember(r => r.Value, r => r.MapFrom(s => s.Name));

            CreateMap<LogisticalViewModel, TLIlogisticalType>().ReverseMap();
            CreateMap<LogisticalViewModel, DropDownListFilters>()
                .ForMember(r => r.Value, r => r.MapFrom(s => s.Name));

            CreateMap<SubTypeViewModel, TLIsubType>().ReverseMap();

            CreateMap<SubTypeViewModel, DropDownListFilters>()
               .ForMember(r => r.Value, r => r.MapFrom(s => s.Name));
            CreateMap<TLIsubType, DropDownListFilters>()
                .ForMember(r => r.Value, r => r.MapFrom(s => s.Name));
            CreateMap<TLIbaseBU, DropDownListFilters>()
               .ForMember(r => r.Value, r => r.MapFrom(s => s.Name));
            CreateMap<TLIregion, DropDownListFilters>()
                .ForMember(r => r.Value, r => r.MapFrom(s => s.RegionName));
            CreateMap<TLIarea, DropDownListFilters>()
                .ForMember(r => r.Value, r => r.MapFrom(s => s.AreaName));
            CreateMap<TLIlocationType, DropDownListFilters>()
                .ForMember(r => r.Value, r => r.MapFrom(s => s.Name));
            CreateMap<TLIbaseType, DropDownListFilters>()
                .ForMember(r => r.Value, r => r.MapFrom(s => s.Name));
            CreateMap<TLIbaseGeneratorType, DropDownListFilters>()
                .ForMember(g => g.Value, g => g.MapFrom(s => s.Name));

            CreateMap<TLIsiteStatus, SiteStatusViewModel>().ReverseMap();
            CreateMap<ListSiteStatusViewModel, TLIsiteStatus>()
                .ForMember(x => x.DateDeleted, x => x.Ignore())
                .BeforeMap((s, d) => d.Deleted = false)
                .ReverseMap();
            CreateMap<AddSiteStatusViewModel, TLIsiteStatus>()
                .ForMember(x => x.DateDeleted, x => x.Ignore())
                .BeforeMap((s, d) => d.Deleted = false)
                .BeforeMap((s, d) => d.Active = true)
                .ReverseMap();

            CreateMap<TLIgroup, UserNameViewModel>().ReverseMap();

            CreateMap<TLIattActivatedCategory, ConfigurationAttsViewModel>().ReverseMap();

            CreateMap<TLIdiversityType, AddDiversityTypeViewModel>().ReverseMap();
            CreateMap<TLIdiversityType, DiversityTypeViewModel>().ReverseMap();

            CreateMap<TLIdiversityType, EditDiversityTypeViewModel>().ReverseMap();

            CreateMap<TLIdiversityType, ConfigurationAttsViewModel>().ReverseMap();

            CreateMap<TLItelecomType, ConfigurationAttsViewModel>().ReverseMap();

            CreateMap<TLIsupportTypeDesigned, ConfigurationAttsViewModel>().ReverseMap();

            CreateMap<TLIsupportTypeDesigned, AddSupportTypeDesignedViewModel>().ReverseMap();
            CreateMap<TLIsupportTypeDesigned, SupportTypeDesignedViewModel>().ReverseMap();

            CreateMap<TLIsupportTypeDesigned, EditSupportTypeDesignedViewModel>().ReverseMap();

            CreateMap<TLIsupportTypeImplemented, ConfigurationAttsViewModel>().ReverseMap();

            CreateMap<TLIstructureType, ConfigurationAttsViewModel>().ReverseMap();

            CreateMap<TLIstructureType, AddStructureTypeViewModel>().ReverseMap();

            CreateMap<TLIstructureType, EditStructureTypeViewModel>().ReverseMap();

            CreateMap<TLIsectionsLegType, ConfigurationAttsViewModel>().ReverseMap();
            CreateMap<TLIsectionsLegType, SectionsLegTypeViewModel>().ReverseMap();

            CreateMap<TLIsectionsLegType, AddSectionsLegTypeViewModel>().ReverseMap();

            CreateMap<TLIsectionsLegType, EditSectionsLegTypeViewModel>().ReverseMap();

            CreateMap<TLIlogisticalType, ConfigurationAttsViewModel>().ReverseMap();
            CreateMap<TLIlogicalOperation, LogicalOperationViewModel>().ReverseMap();

            CreateMap<TLIbaseCivilWithLegsType, ConfigurationAttsViewModel>().ReverseMap();

            CreateMap<TLIbaseGeneratorType, ConfigurationAttsViewModel>().ReverseMap();

            CreateMap<TLIinstallationCivilwithoutLegsType, ConfigurationAttsViewModel>().ReverseMap();

            CreateMap<TLIinstallationCivilwithoutLegsType, AddInstCivilwithoutLegsTypeViewModel>().ReverseMap();

            CreateMap<TLIinstallationCivilwithoutLegsType, EditInstCivilwithoutLegsTypeViewModel>().ReverseMap();

            CreateMap<TLIattActivatedCategory, ConfigurationAttsViewModel>().ReverseMap();

            CreateMap<TLIattActivatedCategory, AddAttActivatedCategoryViewModel>().ReverseMap();

            CreateMap<TLIboardType, ConfigurationAttsViewModel>().ReverseMap();

            CreateMap<TLIguyLineType, ConfigurationAttsViewModel>().ReverseMap();

            CreateMap<TLIpowerType, ConfigurationAttsViewModel>().ReverseMap();

            CreateMap<TLIlocationType, LocationTypeViewModel>().ReverseMap();
            CreateMap<TLIlocationType, AddLocationTypeViewModel>().ReverseMap();
            CreateMap<TLIlocationType, EditLocationTypeViewModel>().ReverseMap();

            CreateMap<TLIdynamicListValues, ConfigurationAttsViewModel>().ReverseMap();
            CreateMap<TLIitemConnectTo, ConfigurationAttsViewModel>().ReverseMap();
            CreateMap<TLIsideArmInstallationPlace, ConfigurationAttsViewModel>().ReverseMap();
            CreateMap<AddSideArmInstallationPlaceViewModel, ConfigurationAttsViewModel>().ReverseMap();

            CreateMap<ConfigurationAttsViewModel, EditDynamicListValuesViewModel>().ForMember(x => x.Value, o => o.MapFrom(t => t.Name));

            CreateMap<ConfigurationAttsViewModel, AddOwnerViewModel>().ForMember(x => x.OwnerName, o => o.MapFrom(t => t.Name));
            CreateMap<ConfigurationAttsViewModel, AddLocationTypeViewModel>().ForMember(x => x.Name, o => o.MapFrom(t => t.Name));
            CreateMap<ConfigurationAttsViewModel, AddBaseTypeViewModel>().ForMember(x => x.Name, o => o.MapFrom(t => t.Name));
            CreateMap<ConfigurationAttsViewModel, AddBaseBUViewModel>().ForMember(x => x.Name, o => o.MapFrom(t => t.Name));
            CreateMap<ConfigurationAttsViewModel, AddRenewableCabinetTypeViewModel>().ForMember(x => x.Name, o => o.MapFrom(t => t.Name));
            CreateMap<ConfigurationAttsViewModel, AddSideArmTypeViewModel>().ForMember(x => x.Name, o => o.MapFrom(t => t.Name));
            CreateMap<ConfigurationAttsViewModel, AddItemStatusViewModel>().ForMember(x => x.Name, o => o.MapFrom(t => t.Name));
            CreateMap<ConfigurationAttsViewModel, AddInstallationPlaceViewModel>().ForMember(x => x.Name, o => o.MapFrom(t => t.Name));
            CreateMap<ConfigurationAttsViewModel, AddTLIsubTypeViewModel>().ForMember(x => x.Name, o => o.MapFrom(t => t.Name));
            CreateMap<ConfigurationAttsViewModel, AddParityViewModel>().ForMember(x => x.Name, o => o.MapFrom(t => t.Name));
            CreateMap<ConfigurationAttsViewModel, AddPolarityTypeViewModel>().ForMember(x => x.Name, o => o.MapFrom(t => t.Name));
            CreateMap<ConfigurationAttsViewModel, AddAsTypeViewModel>().ForMember(x => x.Name, o => o.MapFrom(t => t.Name));
            CreateMap<ConfigurationAttsViewModel, AddcivilNonSteelTypeViewModel>().ForMember(x => x.Name, o => o.MapFrom(t => t.Name));
            CreateMap<ConfigurationAttsViewModel, AddPowerTypeViewModel>().ForMember(x => x.Name, o => o.MapFrom(t => t.Name));
            CreateMap<ConfigurationAttsViewModel, AddCapacityViewModel>().ForMember(x => x.Name, o => o.MapFrom(t => t.Name));
            CreateMap<ConfigurationAttsViewModel, AddCivilSteelSupportCategoryViewModel>().ForMember(x => x.Name, o => o.MapFrom(t => t.Name));
            CreateMap<ConfigurationAttsViewModel, AddCabinetPowerTypeViewModel>().ForMember(x => x.Name, o => o.MapFrom(t => t.Name));
            CreateMap<ConfigurationAttsViewModel, AddDocumentTypeViewModel>().ForMember(x => x.Name, o => o.MapFrom(t => t.Name));
            CreateMap<ConfigurationAttsViewModel, AddEnforcmentCategoryViewModel>().ForMember(x => x.Name, o => o.MapFrom(t => t.Name));
            CreateMap<ConfigurationAttsViewModel, AddGuyLineTypeViewModel>().ForMember(x => x.Name, o => o.MapFrom(t => t.Name));
            CreateMap<ConfigurationAttsViewModel, AddBoardTypeViewModel>().ForMember(x => x.Name, o => o.MapFrom(t => t.Name));
            CreateMap<ConfigurationAttsViewModel, AddInstCivilwithoutLegsTypeViewModel>().ForMember(x => x.Name, o => o.MapFrom(t => t.Name));
            CreateMap<ConfigurationAttsViewModel, AddBaseGeneratorTypeViewModel>().ForMember(x => x.Name, o => o.MapFrom(t => t.Name));
            CreateMap<ConfigurationAttsViewModel, AddBaseCivilWithLegsTypeViewModel>().ForMember(x => x.Name, o => o.MapFrom(t => t.Name));
            CreateMap<ConfigurationAttsViewModel, AddLogisticalTypeViewModel>().ForMember(x => x.Name, o => o.MapFrom(t => t.Name));
            CreateMap<ConfigurationAttsViewModel, AddSectionsLegTypeViewModel>().ForMember(x => x.Name, o => o.MapFrom(t => t.Name));
            CreateMap<ConfigurationAttsViewModel, AddStructureTypeViewModel>().ForMember(x => x.Name, o => o.MapFrom(t => t.Name));
            CreateMap<ConfigurationAttsViewModel, AddSupportTypeImplementedViewModel>().ForMember(x => x.Name, o => o.MapFrom(t => t.Name));
            CreateMap<ConfigurationAttsViewModel, AddPolarityOnLocationViewModel>().ForMember(x => x.Name, o => o.MapFrom(t => t.Name));
            CreateMap<ConfigurationAttsViewModel, AddSupportTypeDesignedViewModel>().ForMember(x => x.Name, o => o.MapFrom(t => t.Name));
            CreateMap<ConfigurationAttsViewModel, AddTelecomTypeViewModel>().ForMember(x => x.Name, o => o.MapFrom(t => t.Name));
            CreateMap<ConfigurationAttsViewModel, AddRepeaterTypeViewModel>().ForMember(x => x.Name, o => o.MapFrom(t => t.Name));
            CreateMap<ConfigurationAttsViewModel, AddOduInstallationTypeViewModel>().ForMember(x => x.Name, o => o.MapFrom(t => t.Name));
            CreateMap<ConfigurationAttsViewModel, AddSideArmInstallationPlaceViewModel>().ForMember(x => x.Name, o => o.MapFrom(t => t.Name));
            CreateMap<ConfigurationAttsViewModel, AddTelecomTypeViewModel>().ForMember(x => x.Name, o => o.MapFrom(t => t.Name));
            CreateMap<ConfigurationAttsViewModel, AddLogicalOperationViewModel>().ForMember(x => x.Name, o => o.MapFrom(t => t.Name));
            CreateMap<ConfigurationAttsViewModel, AddOperationViewModel>().ForMember(x => x.Name, o => o.MapFrom(t => t.Name));
            CreateMap<ConfigurationAttsViewModel, AddDataTypeViewModel>().ForMember(x => x.Name, o => o.MapFrom(t => t.Name));
            CreateMap<ConfigurationAttsViewModel, AddDiversityTypeViewModel>().ForMember(x => x.Name, o => o.MapFrom(t => t.Name));


            CreateMap<TLIboardType, BoardTypeViewModel>().ReverseMap();

            CreateMap<TLIboardType, AddBoardTypeViewModel>().ReverseMap();

            CreateMap<TLIboardType, EditBoardTypeViewModel>().ReverseMap();

            CreateMap<TLIlogisticalType, LogisticalTypeViewModel>().ReverseMap();

            CreateMap<TLIlogisticalType, AddLogisticalTypeViewModel>().ReverseMap();

            CreateMap<TLIlogisticalType, EditLogisticalTypeViewModel>().ReverseMap();

            CreateMap<TLIpolarityOnLocation, PolarityOnLocationViewModel>().ReverseMap();

            CreateMap<TLIpolarityOnLocation, AddPolarityOnLocationViewModel>().ReverseMap();

            CreateMap<TLIpolarityOnLocation, EditPolarityOnLocationViewModel>().ReverseMap();

            CreateMap<TLIpowerType, PowerTypeViewModel>().ReverseMap();
            CreateMap<TLIcivilWithoutLeg, EditinstallationAttributesCivilWithoutLegs>().ReverseMap();

            CreateMap<TLIpowerType, AddPowerTypeViewModel>().ReverseMap();
            CreateMap<TLIsubType, AddTLIsubTypeViewModel>().ReverseMap();

            CreateMap<TLIcivilSteelSupportCategory, AddCivilSteelSupportCategoryViewModel>().ReverseMap();
            CreateMap<TLIcivilSteelSupportCategory, CivilSteelSupportCategoryViewModel>().ReverseMap();


            CreateMap<TLIpowerType, EditPowerTypeViewModel>().ReverseMap();

            CreateMap<TLIattributeActivated, BaseInstAttView>().ReverseMap();

            CreateMap<TLIdynamicListValues, EditDynamicListValuesViewModel>().ReverseMap();


            CreateMap<TLIattributeActivated, AttributeActivatedViewModel>().ReverseMap();
            CreateMap<TLIattributeActivated, EditAttributeActivatedViewModel>().ReverseMap();

            CreateMap<TLIinstallationPlace, DropDownListFilters>()

                .ForMember(I => I.Value, I => I.MapFrom(s => s.Name));

            #region related_list_DropDownListFilters
            CreateMap<TLIsupportTypeDesigned, DropDownListFilters>()
                .ForMember(c => c.Value, c => c.MapFrom(f => f.Name));
            CreateMap<TLIsectionsLegType, DropDownListFilters>()
               .ForMember(c => c.Value, c => c.MapFrom(f => f.Name));
            CreateMap<TLIstructureType, StructureTypeViewModel>().ReverseMap();
            CreateMap<TLIstructureType, DropDownListFilters>()
              .ForMember(c => c.Value, c => c.MapFrom(f => f.Name));
            CreateMap<TLIcivilSteelSupportCategory, DropDownListFilters>()
            .ForMember(c => c.Value, c => c.MapFrom(f => f.Name));
            CreateMap<TLIinstallationCivilwithoutLegsType, InstCivilwithoutLegsTypeViewModel>().ReverseMap();
            CreateMap<TLIinstallationCivilwithoutLegsType, DropDownListFilters>()
            .ForMember(c => c.Value, c => c.MapFrom(f => f.Name));
            CreateMap<TLIbaseCivilWithLegsType, DropDownListFilters>()
                .ForMember(x => x.Value, x => x.MapFrom(f => f.Name));
            CreateMap<TLIsupportTypeImplemented, DropDownListFilters>()
                .ForMember(x => x.Value, x => x.MapFrom(f => f.Name));
            CreateMap<TLIguyLineType, DropDownListFilters>()
                .ForMember(x => x.Value, x => x.MapFrom(f => f.Name));
            CreateMap<TLIowner, DropDownListFilters>()
                .ForMember(o => o.Value, o => o.MapFrom(s => s.OwnerName));
            CreateMap<TLImwDish, DropDownListFilters>()
               .ForMember(o => o.Value, o => o.MapFrom(s => s.DishName));
            CreateMap<TLImwODULibrary, DropDownListFilters>()
              .ForMember(o => o.Value, o => o.MapFrom(s => s.Model));
            CreateMap<TLIcivilWithLegLibrary, DropDownListFilters>()
                .ForMember(c => c.Value, c => c.MapFrom(s => s.Model));
            CreateMap<TLIcivilWithoutLegLibrary, DropDownListFilters>()
                .ForMember(c => c.Value, c => c.MapFrom(s => s.Model));
            CreateMap<TLIcivilNonSteelLibrary, DropDownListFilters>()
                .ForMember(c => c.Value, c => c.MapFrom(s => s.Model));
            CreateMap<TLIdataType, DropDownListFilters>()
                .ForMember(x => x.Value, x => x.MapFrom(f => f.Name));
            CreateMap<TLIcivilWithoutLegCategory, DropDownListFilters>()
                .ForMember(x => x.Value, x => x.MapFrom(f => f.Name));
            CreateMap<TLIpolarityType, DropDownListFilters>()
                .ForMember(x => x.Value, x => x.MapFrom(f => f.Name));
            CreateMap<TLIasType, DropDownListFilters>()
                .ForMember(x => x.Value, x => x.MapFrom(f => f.Name));
            CreateMap<TLIoduInstallationType, DropDownListFilters>()
                .ForMember(x => x.Value, x => x.MapFrom(s => s.Name));
            CreateMap<TLImwBULibrary, DropDownListFilters>()
                .ForMember(x => x.Value, x => x.MapFrom(f => f.Model));
            CreateMap<TLIrepeaterType, DropDownListFilters>()
                .ForMember(x => x.Value, x => x.MapFrom(f => f.Name));
            CreateMap<TLIpolarityOnLocation, DropDownListFilters>()
               .ForMember(x => x.Value, x => x.MapFrom(f => f.Name));
            CreateMap<TLIitemConnectTo, DropDownListFilters>()
            .ForMember(x => x.Value, x => x.MapFrom(f => f.Name));
            CreateMap<TLImwDishLibrary, DropDownListFilters>()
           .ForMember(x => x.Value, x => x.MapFrom(f => f.Model));
            CreateMap<TLIboardType, DropDownListFilters>()
           .ForMember(x => x.Value, x => x.MapFrom(f => f.Name));

            CreateMap<TLIcabinetPowerLibrary, DropDownListFilters>()
                .ForMember(x => x.Value, x => x.MapFrom(f => f.Model));
            CreateMap<TLIcabinetTelecomLibrary, DropDownListFilters>()
                .ForMember(x => x.Value, x => x.MapFrom(f => f.Model));
            CreateMap<TLIcabinet, DropDownListFilters>()
                .ForMember(x => x.Value, x => x.MapFrom(f => f.Name));
            CreateMap<TLIleg, DropDownListFilters>()
                .ForMember(x => x.Value, x => x.MapFrom(f => f.CiviLegName));
            CreateMap<TLIsolarLibrary, DropDownListFilters>()
                .ForMember(x => x.Value, x => x.MapFrom(f => f.Model));
            CreateMap<TLIgeneratorLibrary, DropDownListFilters>()
                .ForMember(x => x.Value, x => x.MapFrom(f => f.Model));
            CreateMap<TLIdiversityType, DropDownListFilters>()
                .ForMember(x => x.Value, x => x.MapFrom(f => f.Name));
            CreateMap<TLIparity, DropDownListFilters>()
                .ForMember(x => x.Value, x => x.MapFrom(f => f.Name));
            CreateMap<TLIlogisticalitem, DropDownListFilters>()
                .ForMember(x => x.Value, x => x.MapFrom(f => f.Name));
            CreateMap<TLIcabinetPowerType, DropDownListFilters>()
                .ForMember(x => x.Value, x => x.MapFrom(f => f.Name));
            CreateMap<TLItelecomType, DropDownListFilters>()
                .ForMember(x => x.Value, x => x.MapFrom(f => f.Name));
            CreateMap<TLIcapacity, DropDownListFilters>()
                .ForMember(x => x.Value, x => x.MapFrom(f => f.Name));
            CreateMap<TLItablesNames, DropDownListFilters>()
                .ForMember(x => x.Value, x => x.MapFrom(f => f.TableName));
            CreateMap<TLIradioAntenna, DropDownListFilters>()
                .ForMember(x => x.Value, x => x.MapFrom(f => f.Name));
            CreateMap<TLIradioRRULibrary, DropDownListFilters>()
                .ForMember(x => x.Value, x => x.MapFrom(f => f.Model));
            CreateMap<TLIradioOtherLibrary, DropDownListFilters>()
               .ForMember(x => x.Value, x => x.MapFrom(f => f.Model));
            CreateMap<TLIradioAntennaLibrary, DropDownListFilters>()
                .ForMember(x => x.Value, x => x.MapFrom(f => f.Model));
            CreateMap<TLIsiteStatus, DropDownListFilters>()
               .ForMember(x => x.Value, x => x.MapFrom(f => f.Name));
            CreateMap<TLIcivilNonSteelType, DropDownListFilters>()
               .ForMember(x => x.Value, x => x.MapFrom(f => f.Name));
            CreateMap<TLImwOtherLibrary, DropDownListFilters>()
                .ForMember(x => x.Value, x => x.MapFrom(f => f.Model));
            CreateMap<TLIpowerType, DropDownListFilters>()
                .ForMember(x => x.Value, x => x.MapFrom(f => f.Name));
            CreateMap<TLIloadOther, DropDownListFilters>()
                .ForMember(x => x.Value, x => x.MapFrom(f => f.Name));
            CreateMap<TLIenforcmentCategory, DropDownListFilters>()
                .ForMember(x => x.Value, x => x.MapFrom(f => f.Name));
            CreateMap<TLIlocationType, DropDownListFilters>()
                .ForMember(x => x.Value, x => x.MapFrom(f => f.Name));
            CreateMap<TLImwRFULibrary, DropDownListFilters>()
                .ForMember(x => x.Value, x => x.MapFrom(f => f.Model));
            CreateMap<TLImwPort, DropDownListFilters>()
                .ForMember(x => x.Value, x => x.MapFrom(f => f.Port_Name));
            CreateMap<TLIpowerLibrary, DropDownListFilters>()
                .ForMember(x => x.Value, x => x.MapFrom(f => f.Model));
            CreateMap<TLIsideArmLibrary, DropDownListFilters>()
                .ForMember(x => x.Value, x => x.MapFrom(f => f.Model));
            CreateMap<TLIsideArmInstallationPlace, DropDownListFilters>()
                .ForMember(x => x.Value, x => x.MapFrom(f => f.Name));
            CreateMap<TLIsideArmType, DropDownListFilters>()
                .ForMember(x => x.Value, x => x.MapFrom(f => f.Name));
            CreateMap<TLIloadOtherLibrary, DropDownListFilters>()
                .ForMember(x => x.Value, x => x.MapFrom(f => f.Model));
            CreateMap<TLIsideArm, DropDownListFilters>()
                .ForMember(x => x.Value, x => x.MapFrom(f => f.Name));
            #endregion

            CreateMap<TLIinstallationPlace, InstallationPlaceViewModel>().ReverseMap();
            CreateMap<TLIoduInstallationType, InstallationPlaceViewModel>().ReverseMap();
            CreateMap<TLIinstallationPlace, AddInstallationPlaceViewModel>().ReverseMap();
            CreateMap<TLIinstallationPlace, EditInstallationPlaceViewModel>().ReverseMap();

            CreateMap<TLIsideArmType, SideArmTypeViewModel>().ReverseMap();
            CreateMap<TLIsideArmType, AddSideArmTypeViewModel>().ReverseMap();
            CreateMap<TLIsideArmType, EditSideArmTypeViewModel>().ReverseMap();

            CreateMap<TLIitemConnectTo, ItemConnectToViewModel>().ReverseMap();

            CreateMap<TLIitemConnectTo, AddItemConnectToViewModel>().ReverseMap();

            CreateMap<TLIitemConnectTo, EditItemConnectToViewModel>().ReverseMap();
            CreateMap<TLIitemConnectTo, AddTelecomTypeViewModel>().ReverseMap();

            CreateMap<TLIrepeaterType, RepeaterTypeViewModel>().ReverseMap();

            CreateMap<TLIrepeaterType, AddRepeaterTypeViewModel>().ReverseMap();

            CreateMap<TLIrepeaterType, EditRepeaterTypeViewModel>().ReverseMap();

            CreateMap<TLIoduInstallationType, OduInstallationTypeViewModel>().ReverseMap();

            CreateMap<TLIoduInstallationType, AddOduInstallationTypeViewModel>().ReverseMap();
            CreateMap<TLIoduInstallationType, ConfigurationAttsViewModel>().ReverseMap();

            CreateMap<TLIoduInstallationType, EditOduInstallationTypeViewModel>().ReverseMap();

            CreateMap<TLIsideArmInstallationPlace, SideArmInstallationPlaceViewModel>().ReverseMap();

            CreateMap<TLIsideArmInstallationPlace, AddSideArmInstallationPlaceViewModel>().ReverseMap();

            CreateMap<TLIsideArmInstallationPlace, ConfigurationAttsViewModel>().ReverseMap();

            CreateMap<ConfigurationAttsViewModel, EditSideArmInstallationPlaceViewModel>().ReverseMap();
            CreateMap<TLIsideArmInstallationPlace, EditSideArmInstallationPlaceViewModel>().ReverseMap();

            CreateMap<TLIsideArm, SideArmViewModel>()
                .ForMember(x => x.ItemStatus_Name, x => x.MapFrom(f => f.ItemStatus.Name))
                .ForMember(x => x.sideArmInstallationPlace_Name, x => x.MapFrom(f => f.sideArmInstallationPlace.Name))
                .ForMember(x => x.sideArmLibrary_Name, x => x.MapFrom(f => f.sideArmLibrary.Model))
                .ForMember(x => x.owner_Name, x => x.MapFrom(f => f.owner.OwnerName))
                .ForMember(x => x.sideArmType_Name, x => x.MapFrom(f => f.sideArmType.Name));

            CreateMap<TLIsideArm, EditSideArmViewModel>().ReverseMap();
            CreateMap<TLIsideArm, AddSideArmViewModel>().ReverseMap();

            CreateMap<TLImwODU, MW_ODUViewModel>()
                .ForMember(x => x.MwODULibrary_Name, x => x.MapFrom(f => f.MwODULibrary.Model))
                .ForMember(x => x.Mw_Dish_Name, x => x.MapFrom(f => f.Mw_Dish.DishName))
                .ForMember(x => x.OduInstallationType_Name, x => x.MapFrom(f => f.OduInstallationType.Name))
                .ForMember(x => x.Owner_Name, x => x.MapFrom(f => f.Owner.OwnerName));

            CreateMap<MW_ODUViewModel, TLImwODU>();
            CreateMap<TLImwODU, AddMW_ODUViewModel>().ReverseMap();

            CreateMap<TLImwODU, EditMW_ODUViewModel>().ReverseMap();

            CreateMap<TLImwDish, MW_DishViewModel>()
                .ForMember(x => x.InstallationPlace_Name, x => x.MapFrom(f => f.InstallationPlace.Name))
                .ForMember(x => x.ItemConnectTo_Name, x => x.MapFrom(f => f.ItemConnectTo.Name))
                .ForMember(x => x.MwDishLibrary_Name, x => x.MapFrom(f => f.MwDishLibrary.Model))
                .ForMember(x => x.owner_Name, x => x.MapFrom(f => f.owner.OwnerName))
                .ForMember(x => x.PolarityOnLocation_Name, x => x.MapFrom(f => f.PolarityOnLocation.Name))
                .ForMember(x => x.RepeaterType_Name, x => x.MapFrom(f => f.RepeaterType.Name))
                .ForMember(x => x.DishName, x => x.MapFrom(f => f.DishName));

            CreateMap<MW_DishViewModel, TLImwDish>();
            CreateMap<TLImwDish, MW_DishGetForAddViewModel>().ForMember(x => x.Name, x => x.MapFrom(f => f.DishName));
            CreateMap<TLImwDish, AddMW_DishViewModel>().ReverseMap();

            CreateMap<TLImwDish, EditMW_DishViewModel>().ReverseMap();

            CreateMap<TLImwBU, MW_BUViewModel>()
                .ForMember(x => x.BaseBU_Name, x => x.MapFrom(f => f.baseBU.Name))
                .ForMember(x => x.InstallationPlace_Name, x => x.MapFrom(f => f.InstallationPlace.Name))
                .ForMember(x => x.MainDish_Name, x => x.MapFrom(f => f.MainDish.DishName))
                .ForMember(x => x.MwBULibrary_Name, x => x.MapFrom(f => f.MwBULibrary.Model))
                .ForMember(x => x.Owner_Name, x => x.MapFrom(f => f.Owner.OwnerName));
            CreateMap<MW_BUViewModel, TLImwBU>();

            CreateMap<TLImwBU, AddMW_BUViewModel>().ReverseMap();

            CreateMap<TLImwBU, EditMW_BUViewModel>().ReverseMap();

            CreateMap<TLIenforcmentCategory, EnforcmentCategoryViewModel>().ReverseMap();

            CreateMap<TLIenforcmentCategory, EditEnforcmentCategoryViewModel>().ReverseMap();

            CreateMap<TLIenforcmentCategory, AddEnforcmentCategoryViewModel>().ReverseMap();

            CreateMap<MW_PortViewModel, TLImwPort>()
                .ForMember(x => x.Port_Name, x => x.MapFrom(f => f.Value));

            CreateMap<TLImwPort, MW_PortViewModel>()
                .ForMember(x => x.Value, x => x.MapFrom(f => f.Port_Name))
                .ForMember(x => x.MwBULibrary_Name, x => x.MapFrom(f => f.MwBULibrary.Model))
                .ForMember(x => x.MwBU_Name, x => x.MapFrom(f => f.MwBU.Name));

            CreateMap<TLImwPort, AddMW_PortViewModel>().ReverseMap();

            CreateMap<TLImwPort, EditMW_PortViewModel>().ReverseMap();
            CreateMap<TLIlogistical, AddLogisticalViewModel>().ReverseMap();
            CreateMap<TLIlogistical, EditLogisticalViewModel>().ReverseMap();
            CreateMap<TLIlogistical, LogisticalViewModel>().ReverseMap();
            CreateMap<TLIlogisticalitem, LogisticalViewModel>().ReverseMap();
            CreateMap<TLIlogistical, MainLogisticalViewModel>()
                .ForMember(c => c.logisticalType_Name, c => c.MapFrom(s => s.logisticalType.Name))
                .ForMember(c => c.tablePartName_Name, c => c.MapFrom(s => s.tablePartName.PartName));

            CreateMap<TLIdynamicAtt, BaseInstAttView>()
                .ForMember(d => d.Desc, d => d.MapFrom(s => s.Description))
                .ForMember(d => d.DataType, d => d.MapFrom(s => s.DataType.Name));
            CreateMap<TLIattributeActivated, BaseAttView>()
                .ForMember(a => a.Desc, a => a.MapFrom(s => s.Description));

            CreateMap<TLIdynamicAtt, DynamicAttLibViewModel>();
            CreateMap<TLIdynamicAtt, DynaminAttInstViewModel>();

            CreateMap<TLImwPort, AddMW_PortViewModel>();

            CreateMap<TLImwPort, EditMW_PortViewModel>();

            CreateMap<TLImwRFU, MW_RFUViewModel>()
                .ForMember(x => x.MwPort_Name, x => x.MapFrom(f => f.MwPort.Port_Name))
                .ForMember(x => x.MwRFULibrary_Name, x => x.MapFrom(f => f.MwRFULibrary.Model))
                .ForMember(x => x.Owner_Name, x => x.MapFrom(f => f.Owner.OwnerName));
            CreateMap<MW_RFUViewModel, TLImwRFU>();

            CreateMap<TLImwRFU, AddMW_RFUViewModel>().ReverseMap();

            CreateMap<TLImwRFU, EditMW_RFUViewModel>().ReverseMap();
            CreateMap<TLIdynamicAtt, DynamicAttLibViewModel>()
                .ForMember(x => x.DataType, x => x.MapFrom(f => f.DataType.Name));
            CreateMap<TLIdynamicAtt, DynaminAttInstViewModel>()
                .ForMember(x => x.DataType, x => x.MapFrom(f => f.DataType.Name));
            CreateMap<TLIdataType, ConfigurationAttsViewModel>().ReverseMap();
            CreateMap<TLIoperation, ConfigurationAttsViewModel>().ReverseMap();

            CreateMap<TLIoperation, AddOperationViewModel>().ReverseMap();
            CreateMap<TLIoperation, OperationViewModel>().ReverseMap();
            CreateMap<TLIoperation, EditOperationViewModel>().ReverseMap();

            CreateMap<TLIlogicalOperation, ConfigurationAttsViewModel>().ReverseMap();
            CreateMap<TLIoperation, OperationViewModel>().ReverseMap();

            CreateMap<TLIlogicalOperation, LogicalOperationViewModel>().ReverseMap();
            CreateMap<TLIlogicalOperation, AddLogicalOperationViewModel>().ReverseMap();
            CreateMap<TLIlogicalOperation, EditLogicalOperationViewModel>().ReverseMap();

            CreateMap<AddDependencyViewModel, TLIdynamicAtt>()
                .ForMember(x => x.dynamicListValues, x => x.Ignore())
                .ForMember(x => x.dependencies, x => x.Ignore())
                .ForMember(x => x.dynamicAttInstValues, x => x.Ignore())
                .ForMember(x => x.dynamicAttLibValues, x => x.Ignore())
                .ForMember(x => x.validations, x => x.Ignore())
                .ForMember(x => x.rules, x => x.Ignore());
            CreateMap<AddDependencyInstViewModel, TLIdynamicAtt>()
                .ForMember(x => x.dynamicListValues, x => x.Ignore());
            CreateMap<TLIrule, AddRuleViewModel>().ReverseMap();
            CreateMap<TLIrule, AddInstRuleViewModel>().ReverseMap();
            //CreateMap<TLIaction, ConditionViewModel>().ForMember(x => x.)
            //*
            CreateMap<ConditionViewModel, TLIaction>()
                .ForMember(x => x.ActionItemOptions, x => x.Ignore())
                .ForMember(x => x.ActionOptions, x => x.Ignore())
                .ForMember(x => x.Proposal, x => x.Ignore())
                .ForMember(x => x.StepActions, x => x.Ignore())
                .ForMember(x => x.Deleted, x => x.Ignore())
                .ForMember(x => x.DateDeleted, x => x.Ignore())
                .BeforeMap((s, d) => d.Type = ActionType.Condition);
            //.ForMember(x => x.Type, x => x.Ignore())
            // .ForMember(x => x.Type,  ActionType.Condition)
            ;
            CreateMap<ListStepActionOptionViewModel, TLIstepActionOption>()
                .ForMember(x => x.DateDeleted, x => x.Ignore())
                .BeforeMap((s, d) => d.Deleted = false)
                .ReverseMap();
            CreateMap<AddStepActionOptionViewModel, TLIstepActionOption>()
                .ForMember(x => x.StepActionId, x => x.Ignore())
                .ForMember(x => x.DateDeleted, x => x.Ignore())
                .BeforeMap((s, d) => d.Deleted = false)
                .ReverseMap();
            CreateMap<AddStepActionOptionViewModel, ListStepActionOptionViewModel>()
                .ForMember(x => x.StepActionId, x => x.Ignore())
                .ReverseMap();
            CreateMap<AddStepActionOptionConditionViewModel, AddStepActionOptionViewModel>()
                .ForMember(x => x.ItemStatusId, x => x.Ignore())
                .ForMember(x => x.OrderStatusId, x => x.Ignore())
                .ReverseMap();
            CreateMap<AddStepActionOptionDecisionViewModel, AddStepActionOptionViewModel>()
                .ForMember(x => x.AllowNote, x => x.Ignore())
                .ForMember(x => x.NoteIsMandatory, x => x.Ignore())
                .ForMember(x => x.ItemStatusId, x => x.Ignore())
                .ForMember(x => x.OrderStatusId, x => x.Ignore())
                .ReverseMap();
            CreateMap<AddStepActionItemOptionValidationViewModel, AddStepActionItemOptionViewModel>()
                .ForMember(x => x.OrderStatusId, x => x.Ignore())
                .ReverseMap();
            CreateMap<AddStepActionItemOptionDecisionViewModel, AddStepActionItemOptionViewModel>()
                .ForMember(x => x.OrderStatusId, x => x.Ignore())
                .ForMember(x => x.NextStepActionId, x => x.Ignore())
                .ReverseMap();
            CreateMap<AddStepActionItemOptionViewModel, ListStepActionItemOptionViewModel>()
                .ForMember(x => x.StepActionId, x => x.Ignore())
                .ReverseMap();
            CreateMap<ListStepActionItemOptionViewModel, TLIstepActionItemOption>()
             .ReverseMap();
            CreateMap<AddStepActionItemOptionViewModel, TLIstepActionItemOption>()
                .ForMember(x => x.StepActionId, x => x.Ignore())
                .ReverseMap();
            //*/

            CreateMap<MailTemplateViewModel, TLImailTemplate>()
            .ForMember(x => x.DateDeleted, x => x.Ignore())
            .BeforeMap((s, d) => d.Deleted = false)
                .ReverseMap();
            CreateMap<AddMailTemplateViewModel, TLImailTemplate>()
            .ForMember(x => x.DateDeleted, x => x.Ignore())
            .BeforeMap((s, d) => d.Deleted = false)
                .ReverseMap();

            CreateMap<OrderStatusViewModel, TLIorderStatus>()
            .ForMember(x => x.DateDeleted, x => x.Ignore())
            .BeforeMap((s, d) => d.Deleted = false)
            .ReverseMap();

            CreateMap<OrderStatusAddViewModel, TLIorderStatus>()
            .ForMember(x => x.DateDeleted, x => x.Ignore())
            .BeforeMap((s, d) => d.Deleted = false)
            .ReverseMap();

            CreateMap<OrderStatusEditViewModel, TLIorderStatus>()
            .ForMember(x => x.DateDeleted, x => x.Ignore())
            .BeforeMap((s, d) => d.Deleted = false)
            .ReverseMap();
            // --------------------- WorkFlow
            CreateMap<ListWorkFlowViewModel, TLIworkFlow>()
            //.ForMember(x => x.StepActions, x => x.Ignore())
            //.ForMember(x => x.WorkFlowGroups, x => x.Ignore())
            //.ForMember(x => x.WorkFlowTypes, x => x.Ignore())
            .ForMember(x => x.Tickets, x => x.Ignore())
            .ForMember(x => x.DateDeleted, x => x.Ignore())
            .ForMember(x => x.Deleted, x => x.Ignore())
            ;
            CreateMap<TLIworkFlow, ListWorkFlowViewModel>()
            //.ForMember(x => x.StepActions, x => x.Ignore())
            //.ForMember(x => x.WorkFlowGroups, x => x.Ignore())
            //.ForMember(x => x.WorkFlowTypes, x => x.Ignore())
            //.ForMember(x => x.Tickets, x => x.Ignore())
            ;
            CreateMap<AddWorkFlowViewModel, TLIworkFlow>()
            //.ForMember(x => x.StepActions, x => x.Ignore())
            //.ForMember(x => x.WorkFlowGroups, x => x.Ignore())
            //.ForMember(x => x.WorkFlowTypes, x => x.Ignore())
            .ForMember(x => x.DateDeleted, x => x.Ignore())
            //.ForMember(x => x.Tickets, x => x.Ignore())
            .BeforeMap((s, d) => d.Deleted = false)
            .BeforeMap((s, d) => d.Active = true);
            /*
            CreateMap<EditWorkFlowViewModel, TLIworkFlow>()
            .ForMember(x => x.StepActions, x => x.Ignore())
            .ForMember(x => x.WorkFlowTypes, x => x.Ignore())
            .ForMember(x => x.WorkFlowGroups, x => x.Ignore())
            .ForMember(x => x.Tickets, x => x.Ignore())
            .ForMember(x => x.Active, x => x.Ignore())
            .ForMember(x => x.DateDeleted, x => x.Ignore())
            .ForMember(x => x.Deleted, x => x.Ignore());
            //*/
            /*
            CreateMap<PermissionWorkFlowViewModel, TLIworkFlow>()
            .ForMember(x => x.Name, x => x.Ignore())
            .ForMember(x => x.Active, x => x.Ignore())
            .ForMember(x => x.Deleted, x => x.Ignore())
            .ForMember(x => x.DateDeleted, x => x.Ignore())
            .ForMember(x => x.SiteStatusId, x => x.Ignore())
            //.ForMember(x => x.SiteStatus, x => x.Ignore())
            .ForMember(x => x.WorkFlowTypes, x => x.Ignore())
            .ForMember(x => x.StepActions, x => x.Ignore())
            .ForMember(x => x.Tickets, x => x.Ignore());
            //*/
            //CreateMap<DeleteWorkFlowViewModel, TLIworkFlow>().ReverseMap();
            CreateMap<WorkFlowViewModel, TLIworkFlow>().ReverseMap();
            CreateMap<TLIworkFlowGroup, WorkFlowGroupVM>().ReverseMap();
            // ====================   end of workflow
            // --------------------   Ticket
            CreateMap<TLIticket, ListTicketViewModel>().ReverseMap();
            CreateMap<TLIticketAction, TicketActionViewModel>().ReverseMap();
            // ====================   end of Ticket
            // --------------------   WorkFlow Type
            CreateMap<WorkFlowTypeViewModel, TLIworkFlowType>()
            .ForMember(x => x.DateDeleted, x => x.Ignore())
            .BeforeMap((s, d) => d.Deleted = false)
            .ReverseMap();
            CreateMap<DeleteWorkFlowTypeViewModel, TLIworkFlowType>().ReverseMap();
            CreateMap<ListWorkFlowTypeViewModel, TLIworkFlowType>()
            //.ForMember(x => x.Tickets, x => x.Ignore())
            .ForMember(x => x.DateDeleted, x => x.Ignore())
            .ForMember(x => x.Deleted, x => x.Ignore()).ReverseMap()
            ;
            CreateMap<AddWorkFlowTypeViewModel, TLIworkFlowType>()
            .ForMember(x => x.DateDeleted, x => x.Ignore())
            //.ForMember(x => x.Tickets, x => x.Ignore())
            .BeforeMap((s, d) => d.Deleted = false);
            //*
            CreateMap<EditWorkFlowTypeViewModel, TLIworkFlowType>()
            //.ForMember(x => x.Tickets, x => x.Ignore())
            .ForMember(x => x.DateDeleted, x => x.Ignore())
            .BeforeMap((s, d) => d.Deleted = false);
            //*/
            // ========================  end of workflow type
            CreateMap<StepAddViewModel, TLIstep>()
            .ForMember(x => x.DateDeleted, x => x.Ignore())
            .BeforeMap((s, d) => d.Deleted = false);
            CreateMap<StepEditViewModel, TLIstep>().ReverseMap();
            CreateMap<StepListViewModel, TLIstep>().ReverseMap();

            //new map
            CreateMap<TLImwOther, AddMw_OtherViewModel>().ReverseMap();
            CreateMap<TLImwOther, Mw_OtherViewModel>()
                .ForMember(x => x.mwOtherLibrary_Name, x => x.MapFrom(f => f.mwOtherLibrary.Model))
                .ForMember(x => x.InstallationPlace_Name, x => x.MapFrom(f => f.InstallationPlace.Name));
            CreateMap<Mw_OtherViewModel, TLImwOther>();

            //--------------
            CreateMap<TLImwOther, EditMw_OtherViewModel>().ReverseMap();


            CreateMap<TLImwOtherLibrary, MW_OtherLibraryViewModel>().ReverseMap();

            CreateMap<TLImwOtherLibrary, AddMW_OtherLibraryViewModel>().ReverseMap();

            CreateMap<TLImwOtherLibrary, EditMW_OtherLibraryViewModel>().ReverseMap();

            CreateMap<TLIloadOtherLibrary, LoadOtherLibraryViewModel>().ReverseMap();

            CreateMap<TLIloadOtherLibrary, AddLoadOtherLibraryViewModel>().ReverseMap();

            CreateMap<TLIloadOtherLibrary, EditLoadOtherLibraryViewModel>().ReverseMap();
            CreateMap<ActionListViewModel, TLIaction>()
               .ForMember(x => x.StepActions, x => x.Ignore())
               .ForMember(x => x.DateDeleted, x => x.Ignore())
               .ForMember(x => x.Deleted, x => x.Ignore());

            CreateMap<TLIloadOther, LoadOtherViewModel>()
                .ForMember(x => x.loadOtherLibrary_Name, x => x.MapFrom(f => f.loadOtherLibrary.Model));
            CreateMap<LoadOtherViewModel, TLIloadOther>();

            CreateMap<TLIloadOther, AddLoadOtherViewModel>()
                .ForMember(x => x.TLIcivilLoads, x => x.Ignore());

            CreateMap<AddLoadOtherViewModel, TLIloadOther>();

            CreateMap<TLIloadOther, EditLoadOtherViewModel>();

            CreateMap<EditLoadOtherViewModel, TLIloadOther>();

            CreateMap<ListPartViewModel, TLIpart>()
                .ForMember(x => x.Equipments, x => x.Ignore())
                .ForMember(x => x.StepActionPart, x => x.Ignore())
                .ReverseMap();

            // ------------------- Step Action
            /*
            CreateMap<TLIstepAction, StepActionViewModel>()
                            .ForMember(x => x.IncomItemStatus, x => x.Ignore())
.ReverseMap();
            //*/
            CreateMap<AddStepActionItemStatusViewModel, TLIstepActionItemStatus>()
            .ForMember(x => x.Id, x => x.Ignore())
            //.ForMember(x => x.StepActionId, x => x.Ignore())
                .ReverseMap();
            CreateMap<ListItemStatusViewModel, TLIstepActionIncomeItemStatus>()
                .ForMember(x => x.ItemStatusId, x => x.Ignore())
                .ForMember(x => x.StepActionId, x => x.Ignore());
            CreateMap<TLIstepActionIncomeItemStatus, ListItemStatusViewModel>()
                .ForMember(x => x.Name, x => x.Ignore())
                .ForMember(x => x.active, x => x.Ignore());
            CreateMap<List<TLIstepActionMailTo>, StepActionGroupsViewModel>()
                .ForMember(x => x.actors, x => x.Ignore())
                .ForMember(x => x.groups, x => x.Ignore())
                .ForMember(x => x.integration, x => x.Ignore())
                .ForMember(x => x.users, x => x.Ignore())
                .ReverseMap();
            CreateMap<ListStepActionViewModel, TLIstepAction>()
            .ForMember(x => x.StepActionMailFrom, x => x.Ignore())
            //.ForMember(x => x.StepActionMailFromId, x => x.Ignore())
            .ForMember(x => x.StepActionMailTo, x => x.Ignore())
            .ForMember(x => x.WorkFlowTypes, x => x.Ignore())
            .ForMember(x => x.TicketActions, x => x.Ignore())
            //.ForMember(x => x.StepActionGroup, x => x.Ignore())
            //.ForMember(x => x.StepActionFileGroup, x => x.Ignore())
            //.ForMember(x => x.StepActionPart, x => x.Ignore())
            .ForMember(x => x.NextStepActions, x => x.Ignore())
            .ForMember(x => x.IncomItemStatus, x => x.Ignore())
            .ForMember(x => x.DateDeleted, x => x.Ignore())
            .BeforeMap((s, d) => d.Deleted = false)
            .BeforeMap((s, d) => d.Period = 0)
            //.BeforeMap((s, d) => d.Active = true)
            //.BeforeMap((s, d) => d.StepActionMailFromId = d.StepActionMailFrom.Id)
                .ReverseMap();
            CreateMap<int, TLInextStepAction>()
                .ForMember(x => x.Id, x => x.Ignore())
                .ForMember(x => x.StepActionId, x => x.Ignore())
                .ForMember(x => x.StepActionItemOptionId, x => x.Ignore())
                .ForMember(x => x.StepActionOptionId, x => x.Ignore())
                .ForMember(x => x.NextStepActionId, x => x.Ignore())
                .ForMember(x => x.StepAction, x => x.Ignore())
                .BeforeMap((s, d) => s = d.NextStepActionId)
                .ReverseMap();
            CreateMap<ListStepActionItemStatusViewModel, ListStepActionItemStatusWiNameViewModel>()
                .ForMember(x => x.Name, x => x.Ignore())
                .ReverseMap();
            CreateMap<ListStepActionItemStatusWiNameViewModel, ListItemStatusViewModel>()
             .ForMember(x => x.active, x => x.Ignore())
             .ReverseMap();
            CreateMap<ListStepActionItemStatusWiNameViewModel, TLIitemStatus>()
             .ForMember(x => x.Active, x => x.Ignore())
            .ForMember(x => x.DeleteDate, x => x.Ignore())
            .BeforeMap((s, d) => d.Deleted = false)
             .ReverseMap();
            CreateMap<NextStepAction, TLInextStepAction>()
             .ForMember(x => x.StepAction, x => x.Ignore())
             .ReverseMap();
            CreateMap<List<ListStepActionGroupViewModel>, StepActionGroupViewModel>()
             .ForMember(x => x.actors, x => x.Ignore())
             .ForMember(x => x.groups, x => x.Ignore())
             .ForMember(x => x.integration, x => x.Ignore())
             .ForMember(x => x.users, x => x.Ignore())
             .ReverseMap();
            //CreateMap<TLIstepAction, StepActionViewModel>().ReverseMap();
            CreateMap<ListStepActionViewModel, StepActionWithNamesViewModel>()
            //.ForMember(x => x.ItemStatusName, x => x.Ignore())
            .ForMember(x => x.OrderStatusName, x => x.Ignore())
            //.ForMember(x => x.IncomItemStatus, x => x.Ignore())
            //.ForMember(x => x.StepActionFileGroup, x => x.Ignore())
            .ForMember(x => x.StepActionGroup, x => x.Ignore())
            //.ForMember(x => x.StepActionMailTo, x => x.Ignore())
            .ForMember(x => x.StepActionOption, x => x.Ignore())
            .ForMember(x => x.StepActionItemOption, x => x.Ignore())
            .ReverseMap();
            CreateMap<AddStepActionGroupViewModel, ListStepActionGroupViewModel>()
            .ForMember(x => x.Id, x => x.Ignore())
            .ForMember(x => x.StepActionId, x => x.Ignore())
            .ReverseMap();
            CreateMap<AddStepActionGroupViewModel, ListStepActionMailToViewModel>()
            .ForMember(x => x.Id, x => x.Ignore())
            .ForMember(x => x.StepActionId, x => x.Ignore())
            .ForMember(x => x.Type, x => x.Ignore())
            .ReverseMap();
            CreateMap<PermissionStepActionViewModel, TLIstepActionMail>()
                .ReverseMap();
            CreateMap<AddStepActionMailToViewModel, TLIstepActionMailTo>()
                .ReverseMap();
            CreateMap<AddStepActionViewModel, TLIstepAction>()
            //.ForMember(x => x.NextStepActionId, x => x.Ignore())
            .ForMember(x => x.StepActionFileGroup, x => x.Ignore())
            .ForMember(x => x.StepActionGroup, x => x.Ignore())
            .ForMember(x => x.StepActionPart, x => x.Ignore())
            .ForMember(x => x.TicketActions, x => x.Ignore())
            .ForMember(x => x.WorkFlowTypes, x => x.Ignore())
            .ForMember(x => x.StepActionMailTo, x => x.Ignore())
            .ForMember(x => x.StepActionMailFromId, x => x.Ignore())
            .ForMember(x => x.StepActionMailFrom, x => x.Ignore())
            .ForMember(x => x.IncomItemStatus, x => x.Ignore())
            .ForMember(x => x.NextStepActions, x => x.Ignore())
            //.ForMember(x => x.ItemStatus, x => x.Ignore())
            //.ForMember(x => x.StepActionItemOption, x => x.Ignore())
            .ForMember(x => x.DateDeleted, x => x.Ignore())
            .BeforeMap((s, d) => d.Deleted = false)
            //.BeforeMap((s, d) => d.Period = 0)
            .BeforeMap((s, d) => d.Active = true);
            CreateMap<EditStepActionViewModel, TLIstepAction>()
            //.ForMember(x => x.NextStepActionId, x => x.Ignore())
            .ForMember(x => x.StepActionFileGroup, x => x.Ignore())
            .ForMember(x => x.StepActionGroup, x => x.Ignore())
            .ForMember(x => x.StepActionPart, x => x.Ignore())
            .ForMember(x => x.TicketActions, x => x.Ignore())
            .ForMember(x => x.WorkFlowTypes, x => x.Ignore())
            .ForMember(x => x.StepActionMailTo, x => x.Ignore())
            .ForMember(x => x.StepActionMailFromId, x => x.Ignore())
            .ForMember(x => x.StepActionMailFrom, x => x.Ignore())
            .ForMember(x => x.IncomItemStatus, x => x.Ignore())
            .ForMember(x => x.NextStepActions, x => x.Ignore())
            .ForMember(x => x.StepActionItemOption, x => x.Ignore())
            .ForMember(x => x.DateDeleted, x => x.Ignore())
            .BeforeMap((s, d) => d.Deleted = false)
            //.BeforeMap((s, d) => d.Period = 0)
            //.BeforeMap((s, d) => d.Active = true)
            ;
            CreateMap<StepActionMailFromViewModel, TLIstepActionMail>()
                //.ForMember(x => x.DateDeleted, x => x.Ignore())
                //.BeforeMap((s, d) => d.Deleted = false)
                //.BeforeMap((s, d) => d.Active = true)
                .ReverseMap();
            CreateMap<StepActionMailToViewModel, TLIstepActionMailTo>().ReverseMap();
            CreateMap<ListStepActionGroupViewModel, TLIstepActionGroup>()
                .ForMember(x => x.DateDeleted, x => x.Ignore())
                .BeforeMap((s, d) => d.Deleted = false)
                .BeforeMap((s, d) => d.Active = true)
                .ReverseMap();
            CreateMap<AddStepActionGroupViewModel, TLIstepActionGroup>()
                .ForMember(x => x.StepActionId, x => x.Ignore())
                .ForMember(x => x.DateDeleted, x => x.Ignore())
                .BeforeMap((s, d) => d.Deleted = false)
                .BeforeMap((s, d) => d.Active = true)
                .ReverseMap();
            CreateMap<ListStepActionGroupViewModel, TLIstepActionFileGroup>()
                .ForMember(x => x.DateDeleted, x => x.Ignore())
                .BeforeMap((s, d) => d.Deleted = false)
                .BeforeMap((s, d) => d.Active = true)
                .ReverseMap();
            CreateMap<ListStepActionPartGroupViewModel, TLIstepActionPartGroup>()
                .ReverseMap();
            CreateMap<ListStepActionPartViewModel, TLIstepActionPart>()
                .ForMember(x => x.StepActionPartGroup, x => x.Ignore())
                .ForMember(x => x.DateDeleted, x => x.Ignore())
                .BeforeMap((s, d) => d.Deleted = false)
                .BeforeMap((s, d) => d.Active = true)
                .ReverseMap();
            CreateMap<List<TLIstepActionPartGroup>, StepActionGroupsViewModel>()
                .ForMember(x => x.actors, x => x.Ignore())
                .ForMember(x => x.groups, x => x.Ignore())
                .ForMember(x => x.integration, x => x.Ignore())
                .ForMember(x => x.users, x => x.Ignore())
                .ReverseMap();

            CreateMap<ActionItemOptionAddViewModel, TLIactionItemOption>()
                .ForMember(x => x.Action, x => x.Ignore())
                .ForMember(x => x.StepActinItemOptions, x => x.Ignore())
                .ForMember(x => x.DeleteDate, x => x.Ignore())
                .BeforeMap((s, d) => d.Deleted = false);
            CreateMap<TLIstepActionIncomeItemStatus, StepActionIncomeItemStatusViewModel>()
                .ReverseMap();
            // ==================  end of step action
            CreateMap<ActionItemOptionListViewModel, TLIactionItemOption>()
                .ForMember(x => x.Action, x => x.Ignore())
                .ForMember(x => x.StepActinItemOptions, x => x.Ignore())
                .ForMember(x => x.DeleteDate, x => x.Ignore())
                .BeforeMap((s, d) => d.Deleted = false)
                .ReverseMap();
            CreateMap<TLIactionItemOption, ActionItemOptionEditViewModel>().ReverseMap();

            CreateMap<TLIdataType, DataTypeViewModel>().ReverseMap();
            CreateMap<TLIdataType, AddDataTypeViewModel>().ReverseMap();
            CreateMap<TLIdataType, EditDataTypeViewModel>().ReverseMap();

            CreateMap<TLIattachedFiles, AttachedFilesViewModel>()
                .ForMember(x => x.TablesName, x => x.MapFrom(f => f.tablesName.TableName));

            CreateMap<TLIattachedFiles, SitePhotosSlideshowViewModel>()
               .ForMember(x => x.tablesNames_Name, x => x.MapFrom(f => f.tablesName.TableName))
               .ForMember(x => x.documenttype_Name, x => x.MapFrom(f => f.documenttype.Name));

            CreateMap<TLIpower, PowerViewModel>()
                .ForMember(x => x.installationPlace_Name, x => x.MapFrom(f => f.installationPlace.Name))
                .ForMember(x => x.owner_Name, x => x.MapFrom(f => f.owner.OwnerName))
                .ForMember(x => x.powerLibrary_Name, x => x.MapFrom(f => f.powerLibrary.Model))
                .ForMember(x => x.powerType_Name, x => x.MapFrom(f => f.powerType.Name));
            CreateMap<PowerViewModel, TLIpower>();
            CreateMap<TLIpower, AddPowerViewModel>().ReverseMap();
            CreateMap<TLIpower, EditPowerViewModel>().ReverseMap();
            CreateMap<TLIgroupRole, AddGroupRoleViewModel>().ReverseMap();
            CreateMap<TLIcivilLoads, AddCivilLoadsViewModel>().ReverseMap();
            CreateMap<TLIradioAntenna, RadioAntennaViewModel>()
                .ForMember(x => x.owner_Name, x => x.MapFrom(f => f.owner.OwnerName))
                .ForMember(x => x.installationPlace_Name, x => x.MapFrom(f => f.installationPlace.Name))
                .ForMember(x => x.radioAntennaLibrary_Name, x => x.MapFrom(f => f.radioAntennaLibrary.Model))
                .ForMember(x => x.Model, x => x.MapFrom(f => f.radioAntennaLibrary.Model));
            CreateMap<RadioAntennaViewModel, TLIradioAntenna>();
            CreateMap<TLIradioAntenna, AddRadioAntennaViewModel>().ReverseMap();
            CreateMap<TLIradioAntenna, EditRadioAntennaViewModel>().ReverseMap();
            CreateMap<TLIRadioRRU, RadioRRUViewModel>()
                .ForMember(x => x.installationPlace_Name, x => x.MapFrom(f => f.installationPlace.Name))
                .ForMember(x => x.owner_Name, x => x.MapFrom(f => f.owner.OwnerName))
                .ForMember(x => x.radioAntenna_Name, x => x.MapFrom(f => f.radioAntenna.Name))
                .ForMember(x => x.radioRRULibrary_Name, x => x.MapFrom(f => f.radioRRULibrary.Model));
            CreateMap<RadioRRUViewModel, TLIRadioRRU>();
            CreateMap<TLIRadioRRU, AddRadioRRUViewModel>().ReverseMap();
            CreateMap<TLIRadioRRU, EditRadioRRUViewModel>().ReverseMap();
            CreateMap<TLIagendaGroup, AgendaGroupViewModel>().ReverseMap();
            CreateMap<TLIagenda, AgendaViewModel>().ReverseMap();
            CreateMap<AddDependencyInstViewModel, TLIdynamicAtt>()
                .ForMember(x => x.dynamicListValues, x => x.Ignore())
                .ForMember(x => x.dynamicAttInstValues, x => x.Ignore())
                .ForMember(x => x.dynamicAttLibValues, x => x.Ignore())
                .ForMember(x => x.dependencies, x => x.Ignore())
                .ForMember(x => x.validations, x => x.Ignore());
            CreateMap<TLIdynamicAtt, AddDependencyInstViewModel>();
            // Table Managment View
            CreateMap<AddAttributeViewManagmentDTO, TLIattributeViewManagment>().ReverseMap();
            CreateMap<TLIattributeViewManagment, AttributeViewManagmentViewModel>().
                ForMember(dest => dest.AttributeViewManagmentId, opt => opt.MapFrom(src => src.Id)).
                ForMember(dest => dest.Key, opt => opt.MapFrom(src => src.DynamicAttId != null ? (src.DynamicAtt.Key) : src.AttributeActivated.Key));
            CreateMap<AttributeViewManagmentViewModel, TLIattributeViewManagment>();
            CreateMap<AddEditableManagmentViewDTO, TLIeditableManagmentView>().ReverseMap();
            CreateMap<EditableManagmentViewModel, TLIeditableManagmentView>().ReverseMap();
            CreateMap<AddAttributeViewManagmentDTO, TLIeditableManagmentView>().ReverseMap();
            CreateMap<AddAttributeViewManagmentDTO, AttributeViewManagmentViewModel>().ReverseMap();

            CreateMap<TLIdynamicAttInstValue, DynamicAttInstValueViewModel>().ReverseMap();

            // CreateMap<TLIdynamicAttInstValue, CivilWithLegsDisplayedOnTableViewModel>().ReverseMap();
            // 
            CreateMap<TLIlog, LoginViewModel>().ReverseMap();
            CreateMap<FilterObject, FilterObjectList>()
                       .ForMember(dest => dest.value, opt => opt.MapFrom((src, dest) =>
                       {
                           List<object> destinationValue = new List<object>();
                           destinationValue.Add(src.value2);
                           // mapping logic goes here

                           return destinationValue;
                       }));


            CreateMap<IntegrationViewModel, TLIintegration>()
                .ForMember(x => x.AgendaGroups, x => x.Ignore())
                .ForMember(x => x.StepActionFileGroup, x => x.Ignore())
                .ForMember(x => x.StepActionGroups, x => x.Ignore())
                .ForMember(x => x.StepActionMailFrom, x => x.Ignore())
                .ForMember(x => x.StepActionMailTo, x => x.Ignore())
                .ForMember(x => x.StepActionPartGroup, x => x.Ignore())
                .ForMember(x => x.WorkFlowGroups, x => x.Ignore())
                .ReverseMap();

            CreateMap<TLIworkflowTableHistory, WorkflowHistoryViewModel>().ReverseMap();
            CreateMap<TLIworkflowTableHistory, AddWorkflowHistoryViewModel>().ReverseMap();
            CreateMap<TLItablesHistory, AddTablesHistoryViewModel>().ReverseMap();
            CreateMap<AddTablesHistoryViewModel, TicketAttributes>().ReverseMap();
            CreateMap<AddWorkflowHistoryViewModel, TicketAttributes>().ReverseMap();
            CreateMap<TLIcapacity, AddCapacityViewModel>().ReverseMap();
            CreateMap<TLIcabinetPowerType, AddCabinetPowerTypeViewModel>().ReverseMap();
            CreateMap<TLIcivilNonSteelType, AddcivilNonSteelTypeViewModel>().ReverseMap();
            CreateMap<TLIasType, AddAsTypeViewModel>().ReverseMap();
            CreateMap<TLIpolarityType, AddPolarityTypeViewModel>().ReverseMap();
            CreateMap<TLIparity, AddParityViewModel>().ReverseMap();
            CreateMap<TLIpermission, PermissionFor_WFViewModel>().ReverseMap();
            CreateMap<TLIradioAntenna, AddRadioAntennaIntegration>().ReverseMap();
            CreateMap<TLIRadioRRU, AddRadioRRUIntegration>().ReverseMap();
            CreateMap<TLIradioOther, AddRadioOtherIntegration>().ReverseMap();
            CreateMap<TLIcivilLoads, CivilLoadsViewModel>().ReverseMap();
            CreateMap<TLIcivilWithLegLibrary, LibraryDataDto>().ReverseMap();
            CreateMap<TLIcivilWithoutLegLibrary, LibraryDataDto>().ReverseMap();
            CreateMap<TLIcivilNonSteelLibrary, LibraryDataDto>().ReverseMap();
            CreateMap<TLIsideArmLibrary, LibraryDataDto>().ReverseMap();
            CreateMap<TLIgeneratorLibrary, LibraryDataDto>().ReverseMap();
            CreateMap<TLImwBULibrary, LibraryDataDto>().ReverseMap();
            CreateMap<TLImwDishLibrary, LibraryDataDto>().ReverseMap();
            CreateMap<TLImwOtherLibrary, LibraryDataDto>().ReverseMap();
            CreateMap<TLImwRFULibrary, LibraryDataDto>().ReverseMap();
            CreateMap<TLImwODULibrary, LibraryDataDto>().ReverseMap();
            CreateMap<TLIcabinetPowerLibrary, LibraryDataDto>().ReverseMap();
            CreateMap<TLIradioAntennaLibrary, LibraryDataDto>().ReverseMap();
            CreateMap<TLIloadOtherLibrary, LibraryDataDto>().ReverseMap();
            CreateMap<TLIcivilNonSteelLibrary, LibraryDataDto>().ReverseMap();
            CreateMap<TLIpowerLibrary, LibraryDataDto>().ReverseMap();
            CreateMap<TLIcabinetTelecomLibrary, LibraryDataDto>().ReverseMap();
            CreateMap<TLIradioOtherLibrary, LibraryDataDto>().ReverseMap();
            CreateMap<TLIradioRRULibrary, LibraryDataDto>().ReverseMap();
            CreateMap<TLIsolarLibrary, LibraryDataDto>().ReverseMap();
            CreateMap<TLIregion, RegionViewModel>().ReverseMap();
            CreateMap<TLIattActivatedCategory, AttActivatedCategoryViewModel>().ReverseMap();
            CreateMap<TLIarea, AreaViewModel>().ReverseMap();


            CreateMap<TLIdiversityType, DiversityTypeViewModel>().ReverseMap();
            CreateMap<TLIasType, AsTypeViewModel>().ReverseMap();
            CreateMap<TLIparity, ParityViewModel>().ReverseMap();
            CreateMap<TLIsupportTypeDesigned, SupportTypeDesignedViewModel>().ReverseMap();
            CreateMap<TLIinstallationCivilwithoutLegsType, InstCivilwithoutLegsTypeViewModel>().ReverseMap();
            CreateMap<TLIsectionsLegType, SectionsLegTypeViewModel>().ReverseMap();
            CreateMap<TLIstructureType, StructureTypeViewModel>().ReverseMap();
            CreateMap<DicMod, DropDownListFilters>().ReverseMap();
            CreateMap<TLIgroupUser, UserNameViewModel>()
            .ForMember(x => x.UserName, x => x.MapFrom(f => f.user.UserName));
            CreateMap<TLIrole_Permissions, RolePermissionsViewModel>()
                .ForMember(x => x.RoleName, x => x.MapFrom(f => f.Role.Name));
            CreateMap<TLIuser_Permissions, NewPermissionsViewModel > ().ReverseMap();
            CreateMap<TLIrole_Permissions, NewPermissionsViewModel>().ReverseMap();
            CreateMap<TLIuser, UserNameViewModel>().ReverseMap();
            CreateMap<TLIuser, EscalationViewModel>().ReverseMap();
            CreateMap<TLIuser, UserWithoutGroupViewModel>().ReverseMap();
            CreateMap<TLIcivilWithLegs, AddCivilWithLegsViewModelInternal>().ReverseMap();
            CreateMap<TLIcivilWithoutLeg, AddCivilWithoutLegViewModelIntegration>().ReverseMap();
            CreateMap<TLIcivilNonSteel, AddCivilNonSteelViewModelIntegration>().ReverseMap();
            CreateMap<TLIcivilWithLegs, EditCivilWithLegsViewModelIntegration>().ReverseMap();
        }
       
    }
}

