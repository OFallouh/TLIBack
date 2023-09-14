using System;
using System.Collections.Generic;
using System.Text;
using TLIS_Service.IService;

namespace TLIS_Service.ServiceBase
{
    public interface IUnitOfWorkService 
    {
        ILogisticalService LogisiticalService { get; }
        ICityService CityService { get; }
        ISiteService SiteService { get; }
        ILegService LegService { get; }
        ICivilWithLegLibraryService CivilWithLegLibraryService { get; }
        ICivilWithoutLegLibraryService CivilWithoutLegLibraryService { get; }
        ICivilNonSteelLibraryService CivilNonSteelLibraryService { get; }
        IPowerLibraryService PowerLibraryService { get; }
        IMW_RFULibraryService MW_RFULibraryService { get; }
        IMW_ODULibraryService MW_ODULibraryService { get; }
        IMW_BULibraryService MW_BULibraryService { get; }
        ISupportTypeImplementedService SupportTypeImplementedService { get; }
        IAttributeActivatedService AttributeActivatedService { get; }
        ISideArmLibraryService SideArmLibraryService { get; }
        IRolePermissionService RolePermissionService { get; }
        IRoleService RoleService { get; }
        IGroupUserService GroupUserService { get; }
        IGroupRoleService GroupRoleService { get; }
        IGroupService GroupService { get; }
        IUserService UserService { get; }
        

        IDynamicAttService DynamicAttService { get; }
       
        IUserPermissionService UserPermissionService { get; }
        ITokenService TokenService { get; }
        ICivilLibraryService CivilLibraryService { get; }
        ICivilWithoutLegCategoryService CivilWithoutLegCategoryService { get; }
        IMWLibraryService MWLibraryService { get; }
        IBaseCivilWithLegsTypeService BaseCivilWithLegsTypeService { get; }
        IOwnerService OwnerService { get; }
        IGuyLineTypeService GuyLineTypeService { get; }
        IPermissionService PermissionService { get; }
        ICivilInstService CivilInstService { get; }
        IEditItemOnSiteService EditItemOnSiteService { get; }
        ICivilWithLegsService CivilWithLegsService { get; }
        IWorkFlowSettingService WorkFlowSettingService { get; }
        IRadioLibraryService RadioLibraryService { get; }
        IOtherInventoryLibraryService OtherInventoryLibraryService { get; }
        IActorService ActorService { get; }
        ILogUsersActionsService LogErrorService { get; }
        IConfigurationAttsService ConfigurationAttsService { get; }
        IRadioInstService RadioInstService { get; }
        ISideArmService SideArmService { get; }
        IMWInstService MWInstService { get; }
        IOtherInventoryInstService OtherInventoryInstService { get; }
        IWorkflowService WorkflowService { get; }
        IFileManagmentService FileManagmentService { get; }
        ITicketService TicketService { get; }
        IMW_PortService MW_PortService { get; }
        IPowerService PowerService { get; }

        IImportSiteDataService ImportSiteDataService { get; }
        ILoadOtherLibraryService LoadOtherLibraryService { get; }
        ILoadOtherService LoadOtherService { get; }
        IAttributeViewManagmentService AttributeViewManagmentService { get; }
        IAttributeHistoryService AttributeHistoryService { get; }
        IexternalSysService ExternalSysService { get; }
        IInternalApiService InternalApiService { get; }

    }
}
