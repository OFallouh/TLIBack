using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using TLIS_DAL;
using TLIS_Repository.Base;
using TLIS_Service.IService;
using TLIS_Service.Services;

namespace TLIS_Service.ServiceBase
{
    public class UnitOfWorkService : IUnitOfWorkService
    {

        ICityService _cityService;
        ILogisticalService _LogisiticalService;
        ISiteService _siteService;
        ILegService _legService;
        ICivilWithLegLibraryService _CivilWithLegLibraryService;
        ICivilWithoutLegLibraryService _CivilWithoutLegLibraryService;
        ICivilNonSteelLibraryService _CivilNonSteelLibraryService;
        IPowerLibraryService _PowerLibraryService;
        IMW_RFULibraryService _MW_RFULibraryService;
        IMW_ODULibraryService _MW_ODULibraryService;
        IMW_BULibraryService _MW_BULibraryService;
        ISupportTypeImplementedService _SupportTypeImplementedService;
        IAttributeActivatedService _AttributeActivatedService;
        ISideArmLibraryService _sideArmLibraryService;
        IRolePermissionService _RolePermissionService;
        IRoleService _RoleService;
        IGroupUserService _GroupUserService;
        IGroupRoleService _GroupRoleService;
        IGroupService _GroupService;
        IUserService _UserService;
        ITokenService _TokenService;
        IUnitOfWork _unitOfWork;
        IServiceCollection _services;
        IDynamicAttService _dynamicAttService;
        IUserPermissionService _UserPermissionService;
        IConfiguration _config;
        ICivilLibraryService _CivilLibraryService;
        ICivilWithoutLegCategoryService _CivilWithoutLegCategoryService;
        IMWLibraryService _MWLibraryService;
        IBaseCivilWithLegsTypeService _BaseCivilWithLegsTypeService;
        IOwnerService _OwnerService;
        IGuyLineTypeService _GuyLineTypeService;
        IPermissionService _PermissionService;
        ICivilInstService _CivilInstService;
        IEditItemOnSiteService _EditItemOnSiteService;
        ICivilWithLegsService _CivilWithLegsService;
        IWorkFlowSettingService _WorkFlowSettingService;
        IRadioLibraryService _RadioLibraryService;
        IOtherInventoryLibraryService _OtherInventoryLibraryService;
        IActorService _ActorService;
        ILogUsersActionsService _LogErrorService;
        IConfigurationAttsService _ConfigurationAttsService;
        IRadioInstService _RadioInstService;
        ISideArmService _SideArmService;
        IMWInstService _MWInstService;
        IOtherInventoryInstService _OtherInventoryInstService;
        IWorkflowService _workflowService;
        IFileManagmentService _FileManagmentService;
        ITicketService _ticketService;
        IMW_PortService _MW_PortService;
        IPowerService _PowerService;
        IImportSiteDataService _ImportSiteDataService;
        ILoadOtherLibraryService _LoadOtherLibraryService;
        ILoadOtherService _LoadOtherService;
        IAttributeViewManagmentService _AttributeViewManagmentService;
        IAttributeHistoryService _AttributeHistoryService;
        IexternalSysService _IexternalSysService;
        IInternalApiService _InternalApiService;
        ApplicationDbContext db;
        IHostingEnvironment _hostingEnvironment;
        IMapper _mapper;
        public UnitOfWorkService(IUnitOfWork unitOfWork, IConfiguration config, IServiceCollection services, ApplicationDbContext _context,
            IHostingEnvironment hostingEnvironment, IMapper mapper)
        {
            db = _context;
            _config = config;
            _unitOfWork = unitOfWork;
            _services = services;
            _hostingEnvironment = hostingEnvironment;
            _mapper = mapper;
        }
        public UnitOfWorkService(IUnitOfWork unitOfWork, IConfiguration config)
        {
            _config = config;
            _unitOfWork = unitOfWork;
        }

        public IAttributeViewManagmentService AttributeViewManagmentService
        {
            get
            {
                if (_AttributeViewManagmentService == null)
                    _AttributeViewManagmentService = new AttributeViewManagmentService(_unitOfWork, _services,_mapper);

                return _AttributeViewManagmentService;
            }
        }

        public ICityService CityService
        {
            get
            {
                if (_cityService == null)
                    _cityService = new CityService(_unitOfWork, _services, _mapper);

                return _cityService;
            }
        }
        public ILogisticalService LogisiticalService
        {
            get
            {
                if (_LogisiticalService == null)
                    _LogisiticalService = new LogisticalService(_unitOfWork,_mapper);

                return _LogisiticalService;
            }
        }

        public ISiteService SiteService
        {
            get
            {
                if (_siteService == null)
                    _siteService = new SiteService(_unitOfWork, _services, db,_mapper);

                return _siteService;
            }
        }

        public ILegService LegService
        {
            get
            {
                if (_legService == null)
                    _legService = new LegService(_unitOfWork, _services, _mapper);

                return _legService;
            }
        }
        public ICivilWithLegLibraryService CivilWithLegLibraryService
        {
            get
            {
                if (_CivilWithLegLibraryService == null)
                    _CivilWithLegLibraryService = new CivilWithLegLibraryService(_unitOfWork, _services,_mapper);

                return _CivilWithLegLibraryService;
            }
        }

        public ICivilWithoutLegLibraryService CivilWithoutLegLibraryService
        {
            get
            {
                if (_CivilWithoutLegLibraryService == null)
                    _CivilWithoutLegLibraryService = new CivilWithoutLegLibraryService(_unitOfWork, _services,_mapper);

                return _CivilWithoutLegLibraryService;
            }
        }

        public ICivilNonSteelLibraryService CivilNonSteelLibraryService
        {
            get
            {
                if (_CivilNonSteelLibraryService == null)
                    _CivilNonSteelLibraryService = new CivilNonSteelLibraryService(_unitOfWork, _services,_mapper);

                return _CivilNonSteelLibraryService;
            }
        }

        public IPowerLibraryService PowerLibraryService
        {
            get
            {
                if (_PowerLibraryService == null)
                    _PowerLibraryService = new PowerLibraryService(_unitOfWork, _services,_mapper);

                return _PowerLibraryService;
            }
        }

        public IMW_RFULibraryService MW_RFULibraryService
        {
            get
            {
                if (_MW_RFULibraryService == null)
                    _MW_RFULibraryService = new MW_RFULibraryService(_unitOfWork, _services,_mapper);

                return _MW_RFULibraryService;
            }
        }
        public IMW_ODULibraryService MW_ODULibraryService
        {
            get
            {
                if (_MW_ODULibraryService == null)
                    _MW_ODULibraryService = new MW_ODULibraryService(_unitOfWork, _services);

                return _MW_ODULibraryService;
            }
        }
        public IMW_BULibraryService MW_BULibraryService
        {
            get
            {
                if (_MW_BULibraryService == null)
                    _MW_BULibraryService = new MW_BULibraryService(_unitOfWork, _services,_mapper);

                return _MW_BULibraryService;
            }
        }
        public ISupportTypeImplementedService SupportTypeImplementedService
        {
            get
            {
                if (_SupportTypeImplementedService == null)
                    _SupportTypeImplementedService = new SupportTypeImplementedService(_unitOfWork, _services,_mapper);
                return _SupportTypeImplementedService;
            }
        }
        public IAttributeActivatedService AttributeActivatedService
        {
            get
            {
                if (_AttributeActivatedService == null)
                    _AttributeActivatedService = new AttributeActivatedService(_unitOfWork, _services,_mapper);
                return _AttributeActivatedService;
            }
        }
        public ISideArmLibraryService SideArmLibraryService
        {
            get
            {
                if (_sideArmLibraryService == null)
                    _sideArmLibraryService = new SideArmLibraryService(_unitOfWork, _services,_mapper);
                return _sideArmLibraryService;
            }
        }

        public IRolePermissionService RolePermissionService
        {
            get
            {
                if (_RolePermissionService == null)
                    _RolePermissionService = new RolePermissionService(_unitOfWork, _services,_mapper);
                return _RolePermissionService;
            }
        }

        public IGroupUserService GroupUserService
        {
            get
            {
                if (_GroupUserService == null)
                    _GroupUserService = new GroupUserService(_unitOfWork, _services,_mapper);
                return _GroupUserService;
            }
        }

        public IGroupRoleService GroupRoleService
        {
            get
            {
                if (_GroupRoleService == null)
                    _GroupRoleService = new GroupRoleService(_unitOfWork, _services,_mapper);
                return _GroupRoleService;
            }
        }
        public IRoleService RoleService
        {
            get
            {
                if (_RoleService == null)
                    _RoleService = new RoleService(_unitOfWork, _services,_mapper);
                return _RoleService;
            }
        }
        public IGroupService GroupService
        {
            get
            {
                if (_GroupService == null)
                    _GroupService = new GroupService(_unitOfWork, _services,_mapper);
                return _GroupService;
            }
        }

        public IUserService UserService
        {
            get
            {
                if (_UserService == null)
                    _UserService = new UserService(_unitOfWork, _config, db, _services,_mapper);
                return _UserService;
            }
        }

        public IDynamicAttService DynamicAttService
        {
            get
            {
                if (_dynamicAttService == null)
                    _dynamicAttService = new DynamicAttService(_unitOfWork, _services, db,_mapper);
                return _dynamicAttService;
            }
        }



        public ITokenService TokenService
        {
            get
            {
                if (_TokenService == null)
                    _TokenService = new TokenService(_unitOfWork, _config, _mapper);
                return _TokenService;
            }
        }
        public IUserPermissionService UserPermissionService
        {
            get
            {
                if (_UserPermissionService == null)
                    _UserPermissionService = new UserPermissionService(_unitOfWork, _services,_mapper);
                return _UserPermissionService;
            }
        }

        public ICivilLibraryService CivilLibraryService
        {
            get
            {
                if (_CivilLibraryService == null)
                    _CivilLibraryService = new CivilLibraryService(_unitOfWork, _services, db,_mapper);
                return _CivilLibraryService;
            }
        }

        public ICivilWithoutLegCategoryService CivilWithoutLegCategoryService
        {
            get
            {
                if (_CivilWithoutLegCategoryService == null)
                    _CivilWithoutLegCategoryService = new CivilWithoutLegCategoryService(_unitOfWork, _services,_mapper);
                return _CivilWithoutLegCategoryService;
            }
        }

        public IMWLibraryService MWLibraryService
        {
            get
            {
                if (_MWLibraryService == null)
                    _MWLibraryService = new MWLibraryService(_unitOfWork, _services,_mapper);
                return _MWLibraryService;
            }
        }

        public IBaseCivilWithLegsTypeService BaseCivilWithLegsTypeService
        {
            get
            {
                if (_BaseCivilWithLegsTypeService == null)
                    _BaseCivilWithLegsTypeService = new BaseCivilWithLegsTypeService(_unitOfWork, _services,_mapper);
                return _BaseCivilWithLegsTypeService;
            }
        }

        public IOwnerService OwnerService
        {
            get
            {
                if (_OwnerService == null)
                    _OwnerService = new OwnerService(_unitOfWork, _services,_mapper);
                return _OwnerService;
            }
        }
        public IGuyLineTypeService GuyLineTypeService
        {
            get
            {
                if (_GuyLineTypeService == null)
                    _GuyLineTypeService = new GuyLineTypeService(_unitOfWork, _services,_mapper);
                return _GuyLineTypeService;
            }
        }

        public IPermissionService PermissionService
        {
            get
            {
                if (_PermissionService == null)
                    _PermissionService = new PermissionService(_unitOfWork, _services, db,_mapper);
                return _PermissionService;
            }
        }

        public ICivilInstService CivilInstService
        {
            get
            {
                if (_CivilInstService == null)
                    _CivilInstService = new CivilInstService(_unitOfWork, _services, db,_mapper);
                return _CivilInstService;
            }
        }
        public IEditItemOnSiteService EditItemOnSiteService
        {
            get
            {
                if (_EditItemOnSiteService == null)
                    _EditItemOnSiteService = new EditItemOnSiteService(_unitOfWork, _services);
                return _EditItemOnSiteService;
            }
        }

        public ICivilWithLegsService CivilWithLegsService
        {
            get
            {
                if (_CivilWithLegsService == null)
                    _CivilWithLegsService = new CivilWithLegsService(_unitOfWork, _services);
                return _CivilWithLegsService;
            }
        }
        public IWorkFlowSettingService WorkFlowSettingService
        {
            get
            {
                if (_WorkFlowSettingService == null)
                    _WorkFlowSettingService = new WorkFlowSettingService(_unitOfWork, _services,_mapper);
                return _WorkFlowSettingService;
            }
        }

        public IRadioLibraryService RadioLibraryService
        {
            get
            {
                if (_RadioLibraryService == null)
                    _RadioLibraryService = new RadioLibraryService(_unitOfWork, _services,_mapper);
                return _RadioLibraryService;
            }
        }

        public IOtherInventoryLibraryService OtherInventoryLibraryService
        {
            get
            {
                if (_OtherInventoryLibraryService == null)
                    _OtherInventoryLibraryService = new OtherInventoryLibraryService(_unitOfWork, _services,_mapper);
                return _OtherInventoryLibraryService;
            }
        }

        public IActorService ActorService
        {
            get
            {
                if (_ActorService == null)
                    _ActorService = new ActorService(_unitOfWork, _services, db, _mapper);
                return _ActorService;
            }
        }
        public ILogUsersActionsService LogErrorService
        {
            get
            {
                if (_LogErrorService == null)
                    _LogErrorService = new LogUsersActionsService(_unitOfWork, _services);
                return _LogErrorService;
            }
        }

        public IConfigurationAttsService ConfigurationAttsService
        {
            get
            {
                if (_ConfigurationAttsService == null)
                    _ConfigurationAttsService = new ConfigurationAttsService(_unitOfWork, _services,_mapper);
                return _ConfigurationAttsService;
            }
        }

        public IRadioInstService RadioInstService
        {
            get
            {
                if (_RadioInstService == null)
                    _RadioInstService = new RadioInstService(_unitOfWork, _services, db,_mapper);
                return _RadioInstService;
            }
        }
        public ISideArmService SideArmService
        {
            get
            {
                if (_SideArmService == null)
                    _SideArmService = new SideArmService(_unitOfWork, _services, db,_mapper);
                return _SideArmService;
            }
        }
        public IWorkflowService WorkflowService
        {
            get
            {
                if (_workflowService == null)
                    _workflowService = new WorkflowService(_unitOfWork, _services,_mapper);
                return _workflowService;

            }
        }

        public IOtherInventoryInstService OtherInventoryInstService
        {
            get
            {
                if (_OtherInventoryInstService == null)
                    _OtherInventoryInstService = new OtherInventoryInstService(_unitOfWork, _services, db,_mapper);
                return _OtherInventoryInstService;
            }
        }

        public IMWInstService MWInstService
        {
            get
            {
                if (_MWInstService == null)
                    _MWInstService = new MWInstService(_unitOfWork, _services, db,_mapper);
                return _MWInstService;
            }
        }

        public IFileManagmentService FileManagmentService
        {
            get
            {
                if (_FileManagmentService == null)
                    _FileManagmentService = new FileManagmentService(_unitOfWork, _services, _config,_mapper);
                return _FileManagmentService;
            }
        }
        public ITicketService TicketService
        {
            get
            {
                if (_ticketService == null)
                    _ticketService = new TicketService(_unitOfWork, _services,_mapper);
                return _ticketService;
            }
        }

        public IMW_PortService MW_PortService
        {
            get
            {
                if (_MW_PortService == null)
                    _MW_PortService = new MW_PortService(_unitOfWork, _services,_mapper);
                return _MW_PortService;
            }
        }

        public IPowerService PowerService
        {
            get
            {
                if (_PowerService == null)
                    _PowerService = new PowerService(_unitOfWork, _services, db,_mapper);
                return _PowerService;
            }
        }
        public IImportSiteDataService ImportSiteDataService
        {
            get
            {
                if (_ImportSiteDataService == null)
                    _ImportSiteDataService = new ImportSiteDataService(_unitOfWork, _services, _config, db, _hostingEnvironment,_mapper);
                return _ImportSiteDataService;
            }
        }

        public ILoadOtherLibraryService LoadOtherLibraryService
        {
            get
            {
                if (_LoadOtherLibraryService == null)
                    _LoadOtherLibraryService = new LoadOtherLibraryService(_unitOfWork, _services,_mapper);
                return _LoadOtherLibraryService;
            }
        }
        public ILoadOtherService LoadOtherService
        {
            get
            {
                if (_LoadOtherService == null)
                    _LoadOtherService = new LoadOtherService(_unitOfWork, _services, db,_mapper);
                return _LoadOtherService;
            }
        }
        public IAttributeHistoryService AttributeHistoryService
        {
            get
            {
                if (_AttributeHistoryService == null)
                    _AttributeHistoryService = new AttributeHistory(_unitOfWork, _services,_mapper);
                return _AttributeHistoryService;
            }
        }

        public IexternalSysService ExternalSysService
        {
            get
            {
                if (_IexternalSysService == null)
                    _IexternalSysService = new ExternalSysService(_unitOfWork, _services,db,_mapper);
                return _IexternalSysService;
            }
        }
        public IInternalApiService InternalApiService
        {
            get
            {
                if (_InternalApiService == null)
                    _InternalApiService = new InternalApiService(_unitOfWork, _services, db,_mapper);
                return _InternalApiService;
            }
        }





    }
}
