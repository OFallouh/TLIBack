using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TLIS_DAL;
using TLIS_Repository.IRepository;
using TLIS_Repository.Repositories;

namespace TLIS_Repository.Base
{
    public class UnitOfWork : IUnitOfWork
    {
        readonly ApplicationDbContext _context;
        private IConfiguration iConfig;

        ISubTypeRepository _SubTypeRepository { get; set; }
        IimportSheetRepository _ImportSheetRepository { get; set; }
        ICivilNonSteelTypeRepository _CivilNonSteelTypeRepository { get; set; }
        IBaseTypeRepository _BaseTypeRepository { get; set; }
        ICityRepository _cityRepository;
        ISiteRepository _siteRepository;
        IMapper _mapper;
        ILegRepository _legRepository;
        ICivilWithLegLibraryRepository _CivilWithLegLibraryRepository;
        ICivilWithoutLegLibraryRepository _CivilWithoutLegLibraryRepository;
        ICivilNonSteelLibraryRepository _CivilNonSteelLibraryRepository;
        IDynamicAttRepository _dynamicAttRepository;
        IDynamicAttLibRepository _dynamicAttLibRepository;
        IValidationRepository _validationRepository;
        IPowerLibraryRepository _PowerLibraryRepository;
        IMW_RFULibraryRepository _MW_RFULibraryRepository;
        IMW_ODULibraryRepository _MW_ODULibraryRepository;
        IMW_BULibraryRepository _MW_BULibraryRepository;
        ISupportTypeImplementedRepository _SupportTypeImplementedRepository;
        IAttributeActivatedRepository _AttributeActivatedRepository;
        ISideArmLibraryRepository _SideArmLibraryRepository;
        IRolePermissionRepository _RolePermissionRepository;
        IRoleRepository _RoleRepository;
        IUserPermissionRepository _UserPermissionRepository;
        IUserRepository _UserRepository;
        IGroupUserRepository _GroupUserRepository;
        IGroupRoleRepository _GroupRoleRepository;
        IGroupRepository _GroupRepository;

        ISectionsLegTypeRepository _SectionsLegTypeRepository;
        IStructureTypeRepository _StructureTypeRepository;
        ICivilSteelSupportCategoryRepository _CivilSteelSupportCategoryRepository;
        IInstallationCivilwithoutLegsTypeRepository _InstallationCivilwithoutLegsTypeRepository;
        ISupportTypeDesignedRepository _SupportTypeDesignedRepository;
        ILogRepository _LogRepository;
        ITokenRepository _TokenRepository;
        //ITaskStatusRepository _TaskStatusRepository;
        ICivilWithLegsRepository _CivilWithLegsRepository;
        ICivilNonSteelRepository _CivilNonSteelRepository;
        ICivilWithoutLegCategoryRepository _CivilWithoutLegCategoryRepository;
        IDiversityTypeRepository _DiversityTypeRepository;
        IMW_DishLibraryRepository _MW_DishLibraryRepository;
        IPolarityTypeRepository _PolarityTypeRepository;
        IAsTypeRepository _AsTypeRepository;
        IParityRepository _ParityRepository;
        IAttActivatedCategoryRepository _AttActivatedCategoryRepository;
        IBaseCivilWithLegsTypeRepository _BaseCivilWithLegsTypeRepository;
        IOwnerRepository _OwnerRepository;
        IGuyLineTypeRepository _GuyLineTypeRepository;
        IPermissionRepository _PermissionRepository;
        IDynamicAttInstValueRepository _DynamicAttInstValueRepository;
        ICivilWithoutLegRepository _CivilWithoutLegRepository;
        IConditionRepository _ConditionRepository;
        IRadioRRULibraryRepository _RadioRRULibraryRepository;
        IRadioAntennaLibraryRepository _RadioAntennaLibraryRepository;
        IRadioOtherLibraryRepository _RadioOtherLibraryRepository;
        ILogisticalitemRepository _LogisticalitemRepository;
        IOptionRepository _OptionRepository;
        ISubOptionRepository _SubOptionRepository;
        IItemStatusRepository _ItemStatusRepository;
        ICapacityRepository _CapacityRepository;
        ITelecomTypeRepository _TelecomTypeRepository;
        ICabinetPowerTypeRepository _CabinetPowerTypeRepository;
        ISolarLibraryRepository _SolarLibraryRepository;
        IGeneratorLibraryRepository _GeneratorLibraryRepository;
        ICabinetTelecomLibraryRepository _CabinetTelecomLibraryRepository;
        ICabinetPowerLibraryRepository _CabinetPowerLibraryRepository;
        IBaseGeneratorTypeRepository _BaseGeneratorTypeRepository;
        IActorRepository _ActorRepository;
        IGeneratorRepository _GeneratorRepository;
        ISolarRepository _SolarRepository;
        IRenewableCabinetTypeRepository _RenewableCabinetTypeRepository;
        ICabinetRepository _CabinetRepository;
        IOtherInventoryDistanceRepository _OtherInventoryDistanceRepository;
        IOtherInSiteRepository _OtherInSiteRepository;
        IHistoryTypeRepository _HistoryTypeRepository;
        ITablesHistoryRepository _TablesHistoryRepository;
        IHistoryDetailsRepository _HistoryDetailsRepository;
        ISiteStatusRepository _SiteStatusRepository;
        IBoardTypeRepository _BoardTypeRepository;
        ILogisticalTypeRepository _LogisticalTypeRepository;
        IInstallationPlaceRepository _InstallationPlaceRepository;
        IRadioRRURepository _RadioRRURepository;
        IRadioAntennaRepository _RadioAntennaRepository;
        IPolarityOnLocationRepository _PolarityOnLocationRepository;
        IItemConnectToRepository _ItemConnectToRepository;
        IRepeaterTypeRepository _RepeaterTypeRepository;
        IOduInstallationTypeRepository _OduInstallationTypeRepository;
        ISideArmInstallationPlaceRepository _SideArmInstallationPlaceRepository;
        ISideArmRepository _SideArmRepository;
        IDataTypeRepository _DataTypeRepository;
        IOperationRepository _OperationRepository;
        ILogicalOperationRepository _LogicalOperationRepository;
        IMW_ODURepository _MW_ODURepository;
        IMW_DishRepository _MW_DishRepository;
        IMW_BURepository _MW_BURepository;
        IDynamicListValuesRepository _DynamicListValuesRepository;
        IRowRepository _RowRepository;
        IRuleRepository _RuleRepository;
        IDependencieRepository _DependencieRepository;
        IRowRuleRepository _RowRuleRepository;
        IDependencyRowRepository _DependencyRowRepository;
        IAllCivilInstRepository _AllCivilInstRepository;
        ICivilSiteDateRepository _CivilSiteDateRepository;
        ICivilSupportDistanceRepository _CivilSupportDistanceRepository;
        IMailTemplateRepository _MailTemplateRepository;
        IMW_OtherLibraryRepository _MW_OtherLibraryRepository;
        IOrderStatusListRepository _OrderStatusListRepository;
        IWorkFlowRepository _workflowRepository;
        IStepActionItemStatusRepository _stepActionItemStatusRepository;
        //IAddWorkFlowRepository _addWorkflowRepository;
        //IEditWorkFlowRepository _editWorkflowRepository;
        //IWorkFlowDeleteRepository _workflowDeleteRepository;
        //IPermissionWorkFlowRepository _workflowPermissionRepository;
        IWorkFlowGroupRepository _workFlowGroupRepository;
        IWorkFlowTypeRepository _workflowTypeRepository;
        INextStepActionRepository _nextStepActionRepository;
        //IListWorkFlowTypeRepository _listWorkflowTypeRepository;
        //IAddWorkFlowTypeRepository _addWorkflowTypeRepository;
        //IEditWorkFlowTypeRepository _editWorkflowTypeRepository;
        //IDeleteWorkFlowTypeRepository _deleteWorkflowTypeRepository;
        //IListWorkFlowRepository _listWorkflowRepository;
        ITicketRepository _ticketRepository;
        ITicketActionRepository _ticketActionRepository;
        ITicketTargetRepository _ticketTargetRepository;
        ITicketOptionNoteRepository _ticketOptionNoteRepository;
        IAgendaRepository _agendaRepository;
        IAgendaGroupRepository _agendaGroupRepository;
        IIntegrationRepository _integrationRepository;
        ILoadOtherLibraryRepository _LoadOtherLibraryRepository;
        IAllOtherInventoryInstRepository _AllOtherInventoryInstRepository;
        IStepListRepository _stepListRepository;
        IStepAddRepository _stepAddRepository;
        IStepEditRepository _stepEditRepository;
        IStepActionIncomeItemStatusRepository _stepActionIncomeItemStatusRepository;
        IActionRepository _actionRepository;
        IMW_RFURepository _MW_RFURepository;
        IMW_PortRepository _MW_PortRepository;
        ITablesNamesRepository _TablesNamesRepository;
        IStepActionRepository _stepActionListRepository;
        IStepActionMailFromRepository _stepActionMailFromRepository;
        IStepActionGroupRepository _stepActionGroupRepository;
        IStepActionPartRepository _stepActionPartRepository;
        IStepActionPartGroupRepository _stepActionPartGroupRepository;
        IStepActionMailToRepository _stepActionMailToRepository;
        IStepActionFileGroupRepository _stepActionFileGroupRepository;
        IStepActionOptionRepository _stepActionOptionRepository;
        IStepActionItemOptionRepository _stepActionItemOptionRepository;
        IPartRepository _partRepository;
        IActionItemOptionListRepository _actionItemOptionListRepository;
        //IActionItemOptionAddRepository _actionItemOptionAddRepository;
        //IActionItemOptionEditRepository _actionItemOptionEditRepository;
        IAttachedFilesRepository _AttachedFilesRepository;
        IEnforcmentCategoryRepository _EnforcmentCategoryRepository;
        ILogUsersActionsRepository _LogUsersActionsRepository;
        IPowerTypeRepository _PowerTypeRepository;

        IInstallationTypeRepository _InstallationTypeRepository;
        ICivilLoadLegsRepository _CivilLoadLegsRepository;
        ICivilLoadsRepository _CivilLoadsRepository;
        IAllLoadInstRepository _AllLoadInstRepository;
        IAntennaRRUInstRepository _AntennaRRUInstRepository;
        IPowerRepository _PowerRepository;
        IMw_OtherRepository _Mw_OtherRepository;
        IRadioOtherRepository _RadioOtherRepository;
        ILoadOtherRepository _LoadOtherRepository;
        ISideArmTypeRepository _SideArmTypeRepository;
        IAreaRepository _AreaRepository;
        IAttributeViewManagmentRepository _AttributeViewManagmentRepository;
        IEditableManagmentViewRepository _EditableManagmentViewRepository;
        ILogistcalRepository _LogistcalRepository;
        ITablePartNameRepository _TablePartNameRepository;
        IWorkflowHistoryRepository _WorkflowHistoryRepository;
        IBaseBURepository _BaseBURepository;
        IDocumentTypeRepository _DocumentTypeRepository;
        ILocationTypeRepository _LocationTypeRepository;
        IRegionRepository _RegionRepository;
        IUserPermissionssRepository _UserPermissionssRepository;     
        IRolePermissionsRepository _RolePermissionsRepository;
        public UnitOfWork(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public IAttributeViewManagmentRepository AttributeViewManagmentRepository
        {
            get
            {
                if (_AttributeViewManagmentRepository == null)
                    _AttributeViewManagmentRepository = new AttributeViewManagmentRepository(_context, _mapper);

                return _AttributeViewManagmentRepository;
            }
        }
        public ISubTypeRepository SubTypeRepository
        {
            get
            {
                if (_SubTypeRepository == null)
                    _SubTypeRepository = new SubTypeRepository(_context, _mapper);

                return _SubTypeRepository;
            }
        }
        public IimportSheetRepository ImportSheetRepository
        {
            get
            {
                if (_ImportSheetRepository == null)
                    _ImportSheetRepository = new ImportSheetRepository(_context, _mapper);

                return _ImportSheetRepository;
            }
        }
        public ICivilNonSteelTypeRepository CivilNonSteelTypeRepository
        {
            get
            {
                if (_CivilNonSteelTypeRepository == null)
                    _CivilNonSteelTypeRepository = new CivilNonSteelTypeRepository(_context, _mapper);

                return _CivilNonSteelTypeRepository;
            }
        }
        public IBaseTypeRepository BaseTypeRepository
        {
            get
            {
                if (_BaseTypeRepository == null)
                    _BaseTypeRepository = new BaseTypeRepository(_context, _mapper);

                return _BaseTypeRepository;
            }
        }
        public IEditableManagmentViewRepository EditableManagmentViewRepository
        {
            get
            {
                if (_EditableManagmentViewRepository == null)
                    _EditableManagmentViewRepository = new EditableManagmentViewRepository(_context, _mapper);

                return _EditableManagmentViewRepository;
            }
        }
        public IMailTemplateRepository MailTemplateRepository
        {
            get
            {
                if (_MailTemplateRepository == null)
                    _MailTemplateRepository = new MailTemplateRepository(_context, _mapper);

                return _MailTemplateRepository;
            }
        }
        public IOrderStatusListRepository OrderStatusListRepository
        {
            get
            {
                if (_OrderStatusListRepository == null)
                    _OrderStatusListRepository = new OrderStatusListRepository(_context, _mapper);

                return _OrderStatusListRepository;
            }
        }


        public ICityRepository CityRepository
        {
            get
            {
                if (_cityRepository == null)
                    _cityRepository = new CityRepository(_context, _mapper);

                return _cityRepository;
            }
        }
        public ISiteRepository SiteRepository
        {
            get
            {
                if (_siteRepository == null)
                    _siteRepository = new SiteRepository(_context, _mapper);

                return _siteRepository;
            }
        }
        public ILegRepository LegRepository
        {
            get
            {
                if (_legRepository == null)
                    _legRepository = new LegRepository(_context, _mapper);

                return _legRepository;
            }
        }

        public ICivilWithLegLibraryRepository CivilWithLegLibraryRepository
        {
            get
            {
                if (_CivilWithLegLibraryRepository == null)
                    _CivilWithLegLibraryRepository = new CivilWithLegLibraryRepository(_context, _mapper);

                return _CivilWithLegLibraryRepository;
            }
        }
        public ICivilWithoutLegLibraryRepository CivilWithoutLegLibraryRepository
        {
            get
            {
                if (_CivilWithoutLegLibraryRepository == null)
                    _CivilWithoutLegLibraryRepository = new CivilWithoutLegLibraryRepository(_context, _mapper);

                return _CivilWithoutLegLibraryRepository;
            }
        }

        public ICivilNonSteelLibraryRepository CivilNonSteelLibraryRepository
        {
            get
            {
                if (_CivilNonSteelLibraryRepository == null)
                    _CivilNonSteelLibraryRepository = new CivilNonSteelLibraryRepository(_context, _mapper);

                return _CivilNonSteelLibraryRepository;
            }
        }
        public IDynamicAttRepository DynamicAttRepository
        {
            get
            {
                if (_dynamicAttRepository == null)
                    _dynamicAttRepository = new DynamicAttRepository(_context, _mapper);

                return _dynamicAttRepository;
            }
        }
        public IDynamicAttLibRepository DynamicAttLibRepository
        {
            get
            {
                if (_dynamicAttLibRepository == null)
                    _dynamicAttLibRepository = new DynamicAttLibRepository(_context, _mapper);

                return _dynamicAttLibRepository;
            }
        }
        public IPowerLibraryRepository PowerLibraryRepository
        {
            get
            {
                if (_PowerLibraryRepository == null)
                    _PowerLibraryRepository = new PowerLibraryRepository(_context, _mapper);

                return _PowerLibraryRepository;
            }
        }
        public IMW_RFULibraryRepository MW_RFULibraryRepository
        {
            get
            {
                if (_MW_RFULibraryRepository == null)
                    _MW_RFULibraryRepository = new MW_RFULibraryRepository(_context, _mapper);

                return _MW_RFULibraryRepository;
            }
        }
        public IMW_ODULibraryRepository MW_ODULibraryRepository
        {
            get
            {
                if (_MW_ODULibraryRepository == null)
                    _MW_ODULibraryRepository = new MW_ODULibraryRepository(_context, _mapper);

                return _MW_ODULibraryRepository;
            }
        }
        public IMW_BULibraryRepository MW_BULibraryRepository
        {
            get
            {
                if (_MW_BULibraryRepository == null)
                    _MW_BULibraryRepository = new MW_BULibraryRepository(_context, _mapper);

                return _MW_BULibraryRepository;
            }
        }
        public ISupportTypeImplementedRepository SupportTypeImplementedRepository
        {
            get
            {
                if (_SupportTypeImplementedRepository == null)
                    _SupportTypeImplementedRepository = new SupportTypeImplementedRepository(_context, _mapper);

                return _SupportTypeImplementedRepository;
            }
        }
        public ISideArmLibraryRepository SideArmLibraryRepository
        {
            get
            {
                if (_SideArmLibraryRepository == null)
                    _SideArmLibraryRepository = new SideArmLibraryRepository(_context, _mapper);

                return _SideArmLibraryRepository;
            }
        }
        public IRolePermissionRepository RolePermissionRepository
        {
            get
            {
                if (_RolePermissionRepository == null)
                    _RolePermissionRepository = new RolePermissionRepository(_context, _mapper);

                return _RolePermissionRepository;
            }
        }

        public IRoleRepository RoleRepository
        {
            get
            {
                if (_RoleRepository == null)
                    _RoleRepository = new RoleRepository(_context, _mapper);

                return _RoleRepository;
            }
        }

        public IAttributeActivatedRepository AttributeActivatedRepository
        {
            get
            {
                if (_AttributeActivatedRepository == null)
                    _AttributeActivatedRepository = new AttributeActivatedRepository(_context, _mapper);

                return _AttributeActivatedRepository;
            }
        }


        public IUserPermissionRepository UserPermissionRepository
        {
            get
            {
                if (_UserPermissionRepository == null)
                    _UserPermissionRepository = new UserPermissionRepository(_context, _mapper);

                return _UserPermissionRepository;
            }
        }


        public IUserRepository UserRepository
        {
            get
            {
                if (_UserRepository == null)
                    _UserRepository = new UserRepository(_context, _mapper);

                return _UserRepository;
            }
        }


        public IGroupUserRepository GroupUserRepository
        {
            get
            {
                if (_GroupUserRepository == null)
                    _GroupUserRepository = new GroupUserRepository(_context, _mapper);

                return _GroupUserRepository;
            }
        }


        public IGroupRoleRepository GroupRoleRepository
        {
            get
            {
                if (_GroupRoleRepository == null)
                    _GroupRoleRepository = new GroupRoleRepository(_context, _mapper);

                return _GroupRoleRepository;
            }
        }


        public IGroupRepository GroupRepository
        {
            get
            {
                if (_GroupRepository == null)
                    _GroupRepository = new GroupRepository(_context, _mapper);

                return _GroupRepository;
            }
        }


        public ITokenRepository TokenRepository
        {
            get
            {
                if (_TokenRepository == null)
                    _TokenRepository = new TokenRepository(_context, _mapper);

                return _TokenRepository;
            }
        }

        public ISectionsLegTypeRepository SectionsLegTypeRepository
        {
            get
            {
                if (_SectionsLegTypeRepository == null)
                    _SectionsLegTypeRepository = new SectionsLegTypeRepository(_context, _mapper);

                return _SectionsLegTypeRepository;
            }
        }

        public IStructureTypeRepository StructureTypeRepository
        {
            get
            {
                if (_StructureTypeRepository == null)
                    _StructureTypeRepository = new StructureTypeRepository(_context, _mapper);

                return _StructureTypeRepository;
            }
        }

        public ICivilSteelSupportCategoryRepository CivilSteelSupportCategoryRepository
        {
            get
            {
                if (_CivilSteelSupportCategoryRepository == null)
                    _CivilSteelSupportCategoryRepository = new CivilSteelSupportCategoryRepository(_context, _mapper);

                return _CivilSteelSupportCategoryRepository;
            }
        }

        public IInstallationCivilwithoutLegsTypeRepository InstallationCivilwithoutLegsTypeRepository
        {
            get
            {
                if (_InstallationCivilwithoutLegsTypeRepository == null)
                    _InstallationCivilwithoutLegsTypeRepository = new InstallationCivilwithoutLegsTypeRepository(_context, _mapper);

                return _InstallationCivilwithoutLegsTypeRepository;
            }
        }

        public ISupportTypeDesignedRepository SupportTypeDesignedRepository
        {
            get
            {
                if (_SupportTypeDesignedRepository == null)
                    _SupportTypeDesignedRepository = new SupportTypeDesignedRepository(_context, _mapper);

                return _SupportTypeDesignedRepository;
            }
        }

        public ILogRepository LogRepository
        {
            get
            {
                if (_LogRepository == null)
                    _LogRepository = new LogRepository(_context, _mapper);

                return _LogRepository;
            }
        }
        /*
        public ITaskStatusRepository TaskStatusRepository
        {
            get
            {
                if (_TaskStatusRepository == null)
                    _TaskStatusRepository = new TaskStatusRepository(_context, _mapper);

                return _TaskStatusRepository;
            }
        }
        //*/

        public ICivilWithLegsRepository CivilWithLegsRepository
        {
            get
            {
                if (_CivilWithLegsRepository == null)
                    _CivilWithLegsRepository = new CivilWithLegsRepository(_context, _mapper);

                return _CivilWithLegsRepository;
            }
        }

        public IValidationRepository ValidationRepository
        {
            get
            {
                if (_validationRepository == null)
                    _validationRepository = new ValidationRepository(_context, _mapper);

                return _validationRepository;
            }
        }

        public ICivilNonSteelRepository CivilNonSteelRepository
        {
            get
            {
                if (_CivilNonSteelRepository == null)
                    _CivilNonSteelRepository = new CivilNonSteelRepository(_context, _mapper);

                return _CivilNonSteelRepository;
            }
        }

        public ICivilWithoutLegCategoryRepository CivilWithoutLegCategoryRepository
        {
            get
            {
                if (_CivilWithoutLegCategoryRepository == null)
                    _CivilWithoutLegCategoryRepository = new CivilWithoutLegCategoryRepository(_context, _mapper);

                return _CivilWithoutLegCategoryRepository;
            }
        }
        public IDiversityTypeRepository DiversityTypeRepository
        {
            get
            {
                if (_DiversityTypeRepository == null)
                    _DiversityTypeRepository = new DiversityTypeRepository(_context, _mapper);

                return _DiversityTypeRepository;
            }
        }

        public IMW_DishLibraryRepository MW_DishLibraryRepository
        {
            get
            {
                if (_MW_DishLibraryRepository == null)
                    _MW_DishLibraryRepository = new MW_DishLibraryRepository(_context, _mapper);

                return _MW_DishLibraryRepository;
            }
        }

        public IPolarityTypeRepository PolarityTypeRepository
        {
            get
            {
                if (_PolarityTypeRepository == null)
                    _PolarityTypeRepository = new PolarityTypeRepository(_context, _mapper);

                return _PolarityTypeRepository;
            }
        }

        public IAsTypeRepository AsTypeRepository
        {
            get
            {
                if (_AsTypeRepository == null)
                    _AsTypeRepository = new AsTypeRepository(_context, _mapper);

                return _AsTypeRepository;
            }
        }

        public IParityRepository ParityRepository
        {
            get
            {
                if (_ParityRepository == null)
                    _ParityRepository = new ParityRepository(_context, _mapper);

                return _ParityRepository;
            }
        }

        public IAttActivatedCategoryRepository AttActivatedCategoryRepository
        {
            get
            {
                if (_AttActivatedCategoryRepository == null)
                    _AttActivatedCategoryRepository = new AttActivatedCategoryRepository(_context, _mapper);

                return _AttActivatedCategoryRepository;
            }
        }

        public IBaseCivilWithLegsTypeRepository BaseCivilWithLegsTypeRepository
        {
            get
            {
                if (_BaseCivilWithLegsTypeRepository == null)
                    _BaseCivilWithLegsTypeRepository = new BaseCivilWithLegsTypeRepository(_context, _mapper);

                return _BaseCivilWithLegsTypeRepository;
            }
        }

        public IOwnerRepository OwnerRepository
        {
            get
            {
                if (_OwnerRepository == null)
                    _OwnerRepository = new OwnerRepository(_context, _mapper);

                return _OwnerRepository;
            }
        }
        public IGuyLineTypeRepository GuyLineTypeRepository
        {
            get
            {
                if (_GuyLineTypeRepository == null)
                    _GuyLineTypeRepository = new GuyLineTypeRepository(_context, _mapper);

                return _GuyLineTypeRepository;
            }
        }

        public IPermissionRepository PermissionRepository
        {
            get
            {
                if (_PermissionRepository == null)
                    _PermissionRepository = new PermissionRepository(_context, _mapper);

                return _PermissionRepository;
            }
        }

        public IDynamicAttInstValueRepository DynamicAttInstValueRepository
        {
            get
            {
                if (_DynamicAttInstValueRepository == null)
                    _DynamicAttInstValueRepository = new DynamicAttInstValueRepository(_context, _mapper);

                return _DynamicAttInstValueRepository;
            }
        }

        public ICivilWithoutLegRepository CivilWithoutLegRepository
        {
            get
            {
                if (_CivilWithoutLegRepository == null)
                    _CivilWithoutLegRepository = new CivilWithoutLegRepository(_context, _mapper);

                return _CivilWithoutLegRepository;
            }
        }

        public IRadioRRULibraryRepository RadioRRULibraryRepository
        {
            get
            {
                if (_RadioRRULibraryRepository == null)
                    _RadioRRULibraryRepository = new RadioRRULibraryRepository(_context, _mapper);

                return _RadioRRULibraryRepository;
            }
        }

        public IRadioAntennaLibraryRepository RadioAntennaLibraryRepository
        {
            get
            {
                if (_RadioAntennaLibraryRepository == null)
                    _RadioAntennaLibraryRepository = new RadioAntennaLibraryRepository(_context, _mapper);

                return _RadioAntennaLibraryRepository;
            }
        }

        public IRadioOtherLibraryRepository RadioOtherLibraryRepository
        {
            get
            {
                if (_RadioOtherLibraryRepository == null)
                    _RadioOtherLibraryRepository = new RadioOtherLibraryRepository(_context, _mapper);

                return _RadioOtherLibraryRepository;
            }
        }

        public ILogisticalitemRepository LogisticalitemRepository
        {
            get
            {
                if (_LogisticalitemRepository == null)
                    _LogisticalitemRepository = new LogisticalitemRepository(_context, _mapper);

                return _LogisticalitemRepository;
            }
        }
        public IConditionRepository ConditionRepository
        {
            get
            {
                if (_ConditionRepository == null)
                    _ConditionRepository = new ConditionRepository(_context, _mapper);

                return _ConditionRepository;
            }
        }

        public IOptionRepository OptionRepository
        {
            get
            {
                if (_OptionRepository == null)
                    _OptionRepository = new OptionRepository(_context, _mapper);

                return _OptionRepository;
            }
        }
        public ISubOptionRepository SubOptionRepository
        {
            get
            {
                if (_SubOptionRepository == null)
                    _SubOptionRepository = new SubOptionRepository(_context, _mapper);

                return _SubOptionRepository;
            }
        }
        public IItemStatusRepository ItemStatusRepository
        {
            get
            {
                if (_ItemStatusRepository == null)
                    _ItemStatusRepository = new ItemStatusRepository(_context, _mapper);

                return _ItemStatusRepository;
            }
        }

        public ICapacityRepository CapacityRepository
        {
            get
            {
                if (_CapacityRepository == null)
                    _CapacityRepository = new CapacityRepository(_context, _mapper);

                return _CapacityRepository;
            }
        }

        public ITelecomTypeRepository TelecomTypeRepository
        {
            get
            {
                if (_TelecomTypeRepository == null)
                    _TelecomTypeRepository = new TelecomTypeRepository(_context, _mapper);

                return _TelecomTypeRepository;
            }
        }

        public ICabinetPowerTypeRepository CabinetPowerTypeRepository
        {
            get
            {
                if (_CabinetPowerTypeRepository == null)
                    _CabinetPowerTypeRepository = new CabinetPowerTypeRepository(_context, _mapper);

                return _CabinetPowerTypeRepository;
            }
        }

        public ISolarLibraryRepository SolarLibraryRepository
        {
            get
            {
                if (_SolarLibraryRepository == null)
                    _SolarLibraryRepository = new SolarLibraryRepository(_context, _mapper);

                return _SolarLibraryRepository;
            }
        }

        public IGeneratorLibraryRepository GeneratorLibraryRepository
        {
            get
            {
                if (_GeneratorLibraryRepository == null)
                    _GeneratorLibraryRepository = new GeneratorLibraryRepository(_context, _mapper);

                return _GeneratorLibraryRepository;
            }
        }

        public ICabinetTelecomLibraryRepository CabinetTelecomLibraryRepository
        {
            get
            {
                if (_CabinetTelecomLibraryRepository == null)
                    _CabinetTelecomLibraryRepository = new CabinetTelecomLibraryRepository(_context, _mapper);

                return _CabinetTelecomLibraryRepository;
            }
        }

        public ICabinetPowerLibraryRepository CabinetPowerLibraryRepository
        {
            get
            {
                if (_CabinetPowerLibraryRepository == null)
                    _CabinetPowerLibraryRepository = new CabinetPowerLibraryRepository(_context, _mapper);

                return _CabinetPowerLibraryRepository;
            }
        }
        public IActorRepository ActorRepository
        {
            get
            {
                if (_ActorRepository == null)
                    _ActorRepository = new ActorRepository(_context, _mapper);

                return _ActorRepository;
            }
        }

        public IGeneratorRepository GeneratorRepository
        {
            get
            {
                if (_GeneratorRepository == null)
                    _GeneratorRepository = new GeneratorRepository(_context, _mapper);

                return _GeneratorRepository;
            }
        }

        public IBaseGeneratorTypeRepository BaseGeneratorTypeRepository
        {
            get
            {
                if (_BaseGeneratorTypeRepository == null)
                    _BaseGeneratorTypeRepository = new BaseGeneratorTypeRepository(_context, _mapper);

                return _BaseGeneratorTypeRepository;
            }
        }

        public ISolarRepository SolarRepository
        {
            get
            {
                if (_SolarRepository == null)
                    _SolarRepository = new SolarRepository(_context, _mapper);

                return _SolarRepository;
            }
        }

        public IRenewableCabinetTypeRepository RenewableCabinetTypeRepository
        {
            get
            {
                if (_RenewableCabinetTypeRepository == null)
                    _RenewableCabinetTypeRepository = new RenewableCabinetTypeRepository(_context, _mapper);

                return _RenewableCabinetTypeRepository;
            }
        }

        public ICabinetRepository CabinetRepository
        {
            get
            {
                if (_CabinetRepository == null)
                    _CabinetRepository = new CabinetRepository(_context, _mapper);

                return _CabinetRepository;
            }
        }

        public IOtherInventoryDistanceRepository OtherInventoryDistanceRepository
        {
            get
            {
                if (_OtherInventoryDistanceRepository == null)
                    _OtherInventoryDistanceRepository = new OtherInventoryDistanceRepository(_context, _mapper);

                return _OtherInventoryDistanceRepository;
            }
        }

        public IOtherInSiteRepository OtherInSiteRepository
        {
            get
            {
                if (_OtherInSiteRepository == null)
                    _OtherInSiteRepository = new OtherInSiteRepository(_context, _mapper);

                return _OtherInSiteRepository;
            }
        }

        public IHistoryTypeRepository HistoryTypeRepository
        {
            get
            {
                if (_HistoryTypeRepository == null)
                    _HistoryTypeRepository = new HistoryTypeRepository(_context, _mapper);

                return _HistoryTypeRepository;
            }
        }

        public ITablesHistoryRepository TablesHistoryRepository
        {
            get
            {
                if (_TablesHistoryRepository == null)
                    _TablesHistoryRepository = new TablesHistoryRepository(_context, _mapper);

                return _TablesHistoryRepository;
            }
        }

        public IHistoryDetailsRepository HistoryDetailsRepository
        {
            get
            {
                if (_HistoryDetailsRepository == null)
                    _HistoryDetailsRepository = new HistoryDetailsRepository(_context, _mapper);

                return _HistoryDetailsRepository;
            }
        }
        public ISiteStatusRepository SiteStatusRepository
        {
            get
            {
                if (_SiteStatusRepository == null)
                    _SiteStatusRepository = new SiteStatusRepository(_context, _mapper);

                return _SiteStatusRepository;
            }
        }
        public IBoardTypeRepository BoardTypeRepository
        {
            get
            {
                if (_BoardTypeRepository == null)
                    _BoardTypeRepository = new BoardTypeRepository(_context, _mapper);

                return _BoardTypeRepository;
            }
        }
        public ILogisticalTypeRepository logisticalTypeRepository
        {
            get
            {
                if (_LogisticalTypeRepository == null)
                    _LogisticalTypeRepository = new LogisticalTypeRepository(_context, _mapper);

                return _LogisticalTypeRepository;
            }
        }

        public IInstallationPlaceRepository InstallationPlaceRepository
        {
            get
            {
                if (_InstallationPlaceRepository == null)
                    _InstallationPlaceRepository = new InstallationPlaceRepository(_context, _mapper);

                return _InstallationPlaceRepository;
            }
        }

        public IRadioRRURepository RadioRRURepository
        {
            get
            {
                if (_RadioRRURepository == null)
                    _RadioRRURepository = new RadioRRURepository(_context, _mapper);

                return _RadioRRURepository;
            }
        }

        public IRadioAntennaRepository RadioAntennaRepository
        {
            get
            {
                if (_RadioAntennaRepository == null)
                    _RadioAntennaRepository = new RadioAntennaRepository(_context, _mapper);

                return _RadioAntennaRepository;
            }
        }
        public IPolarityOnLocationRepository PolarityOnLocationRepository
        {
            get
            {
                if (_PolarityOnLocationRepository == null)
                    _PolarityOnLocationRepository = new PolarityOnLocationRepository(_context, _mapper);

                return _PolarityOnLocationRepository;
            }
        }
        public IItemConnectToRepository ItemConnectToRepository
        {
            get
            {
                if (_ItemConnectToRepository == null)
                    _ItemConnectToRepository = new ItemConnectToRepository(_context, _mapper);

                return _ItemConnectToRepository;
            }
        }
        public IRepeaterTypeRepository RepeaterTypeRepository
        {
            get
            {
                if (_RepeaterTypeRepository == null)
                    _RepeaterTypeRepository = new RepeaterTypeRepository(_context, _mapper);

                return _RepeaterTypeRepository;
            }
        }
        public IOduInstallationTypeRepository OduInstallationTypeRepository
        {
            get
            {
                if (_OduInstallationTypeRepository == null)
                    _OduInstallationTypeRepository = new OduInstallationTypeRepository(_context, _mapper);

                return _OduInstallationTypeRepository;
            }
        }
        public ISideArmInstallationPlaceRepository SideArmInstallationPlaceRepository
        {
            get
            {
                if (_SideArmInstallationPlaceRepository == null)
                    _SideArmInstallationPlaceRepository = new SideArmInstallationPlaceRepository(_context, _mapper);

                return _SideArmInstallationPlaceRepository;
            }
        }
        public ISideArmRepository SideArmRepository
        {
            get
            {
                if (_SideArmRepository == null)
                    _SideArmRepository = new SideArmRepository(_context, _mapper);

                return _SideArmRepository;
            }
        }
        public IDataTypeRepository DataTypeRepository
        {
            get
            {
                if (_DataTypeRepository == null)
                    _DataTypeRepository = new DataTypeRepository(_context, _mapper);

                return _DataTypeRepository;
            }
        }
        public IOperationRepository OperationRepository
        {
            get
            {
                if (_OperationRepository == null)
                    _OperationRepository = new OperationRepository(_context, _mapper);

                return _OperationRepository;
            }
        }
        public ILogicalOperationRepository LogicalOperationRepository
        {
            get
            {
                if (_LogicalOperationRepository == null)
                    _LogicalOperationRepository = new LogicalOperationRepository(_context, _mapper);

                return _LogicalOperationRepository;
            }
        }
        public IMW_ODURepository MW_ODURepository
        {
            get
            {
                if (_MW_ODURepository == null)
                    _MW_ODURepository = new MW_ODURepository(_context, _mapper);

                return _MW_ODURepository;
            }
        }
        public IMW_DishRepository MW_DishRepository
        {
            get
            {
                if (_MW_DishRepository == null)
                    _MW_DishRepository = new MW_DishRepository(_context, _mapper);

                return _MW_DishRepository;
            }
        }
        public IMW_BURepository MW_BURepository
        {
            get
            {
                if (_MW_BURepository == null)
                    _MW_BURepository = new MW_BURepository(_context, _mapper);

                return _MW_BURepository;
            }
        }
        public IMW_RFURepository MW_RFURepository
        {
            get
            {
                if (_MW_RFURepository == null)
                    _MW_RFURepository = new MW_RFURepository(_context, _mapper);

                return _MW_RFURepository;
            }
        }
        public IMW_PortRepository MW_PortRepository
        {
            get
            {
                if (_MW_PortRepository == null)
                    _MW_PortRepository = new MW_PortRepository(_context, _mapper);

                return _MW_PortRepository;
            }
        }
        public IDynamicListValuesRepository DynamicListValuesRepository
        {
            get
            {
                if (_DynamicListValuesRepository == null)
                    _DynamicListValuesRepository = new DynamicListValuesRepository(_context, _mapper);

                return _DynamicListValuesRepository;
            }
        }

        public IRowRepository RowRepository
        {
            get
            {
                if (_RowRepository == null)
                    _RowRepository = new RowRepository(_context, _mapper);

                return _RowRepository;
            }
        }

        public IRuleRepository RuleRepository
        {
            get
            {
                if (_RuleRepository == null)
                    _RuleRepository = new RuleRepository(_context, _mapper);

                return _RuleRepository;
            }
        }

        public IDependencieRepository DependencieRepository
        {
            get
            {
                if (_DependencieRepository == null)
                    _DependencieRepository = new DependencieRepository(_context, _mapper);

                return _DependencieRepository;
            }
        }

        public IRowRuleRepository RowRuleRepository
        {
            get
            {
                if (_RowRuleRepository == null)
                    _RowRuleRepository = new RowRuleRepository(_context, _mapper);

                return _RowRuleRepository;
            }
        }

        public IDependencyRowRepository DependencyRowRepository
        {
            get
            {
                if (_DependencyRowRepository == null)
                    _DependencyRowRepository = new DependencyRowRepository(_context, _mapper);

                return _DependencyRowRepository;
            }
        }

        public IAllCivilInstRepository AllCivilInstRepository
        {
            get
            {
                if (_AllCivilInstRepository == null)
                    _AllCivilInstRepository = new AllCivilInstRepository(_context, _mapper);

                return _AllCivilInstRepository;
            }
        }

        public ICivilSiteDateRepository CivilSiteDateRepository
        {
            get
            {
                if (_CivilSiteDateRepository == null)
                    _CivilSiteDateRepository = new CivilSiteDateRepository(_context, _mapper);

                return _CivilSiteDateRepository;
            }
        }

        public ICivilSupportDistanceRepository CivilSupportDistanceRepository
        {
            get
            {
                if (_CivilSupportDistanceRepository == null)
                    _CivilSupportDistanceRepository = new CivilSupportDistanceRepository(_context, _mapper);

                return _CivilSupportDistanceRepository;
            }
        }

        public IMW_OtherLibraryRepository MW_OtherLibraryRepository
        {
            get
            {
                if (_MW_OtherLibraryRepository == null)
                    _MW_OtherLibraryRepository = new MW_OtherLibraryRepository(_context, _mapper);

                return _MW_OtherLibraryRepository;
            }
        }

        public IWorkFlowRepository WorkFlowRepository
        {
            get
            {
                if (_workflowRepository == null)
                    _workflowRepository = new WorkFlowRepository(_context, _mapper);
                return _workflowRepository;
            }
        }
        public IStepActionItemStatusRepository StepActionItemStatusRepository
        {
            get
            {
                if (_stepActionItemStatusRepository == null)
                    _stepActionItemStatusRepository = new StepActionItemStatusRepository(_context, _mapper);
                return _stepActionItemStatusRepository;
            }
        }
        /*
        public IAddWorkFlowRepository AddWorkFlowRepository
        {
            get
            {
                if (_addWorkflowRepository == null)
                    _addWorkflowRepository = new AddWorkFlowRepository(_context, _mapper);
                return _addWorkflowRepository;
            }
        }
       
        public IEditWorkFlowRepository EditWorkFlowRepository
        {
            get
            {
                if (_editWorkflowRepository == null)
                    _editWorkflowRepository = new EditWorkFlowRepository(_context, _mapper);
                return _editWorkflowRepository;
            }
        }
        public IWorkFlowDeleteRepository WorkFlowDeleteRepository
        {
            get
            {
                if (_workflowDeleteRepository == null)
                    _workflowDeleteRepository = new WorkFlowDeleteRepository(_context, _mapper);
                return _workflowDeleteRepository;
            }
        }
        public IPermissionWorkFlowRepository PermissionWorkFlowRepository
        {
            get
            {
                if (_workflowPermissionRepository == null)
                    _workflowPermissionRepository = new PermissionWorkFlowRepository(_context, _mapper);
                return _workflowPermissionRepository;
            }
        }
        //*/
        public IWorkFlowGroupRepository WorkFlowGroupRepository
        {
            get
            {
                if (_workFlowGroupRepository == null)
                {
                    _workFlowGroupRepository = new WorkFlowGroupRepository(_context, _mapper);
                }
                return _workFlowGroupRepository;
            }
        }
        /*
        public IListWorkFlowRepository ListWorkFlowRepository
        {
            get
            {
                if (_listWorkflowRepository == null)
                    _listWorkflowRepository = new ListWorkFlowRepository(_context, _mapper);
                return _listWorkflowRepository;
            }
        }
        //*/

        public IWorkFlowTypeRepository WorkFlowTypeRepository
        {
            get
            {
                if (_workflowTypeRepository == null)
                    _workflowTypeRepository = new WorkFlowTypeRepository(_context, _mapper);
                return _workflowTypeRepository;
            }
        }
        public INextStepActionRepository NextStepActionRepository
        {
            get
            {
                if (_nextStepActionRepository == null)
                    _nextStepActionRepository = new NextStepActionRepository(_context, _mapper);
                return _nextStepActionRepository;
            }
        }
        /*
        public IListWorkFlowTypeRepository ListWorkFlowTypeRepository
        {
            get
            {
                if (_listWorkflowTypeRepository == null)
                    _listWorkflowTypeRepository = new ListWorkFlowTypeRepository(_context, _mapper);
                return _listWorkflowTypeRepository;
            }
        }
        public IAddWorkFlowTypeRepository AddWorkFlowTypeRepository
        {
            get
            {
                if (_addWorkflowTypeRepository == null)
                    _addWorkflowTypeRepository = new AddWorkFlowTypeRepository(_context, _mapper);
                return _addWorkflowTypeRepository;
            }
        }
        public IEditWorkFlowTypeRepository EditWorkFlowTypeRepository
        {
            get
            {
                if (_editWorkflowTypeRepository == null)
                    _editWorkflowTypeRepository = new EditWorkFlowTypeRepository(_context, _mapper);
                return _editWorkflowTypeRepository;
            }
        }
        public IDeleteWorkFlowTypeRepository DeleteWorkFlowTypeRepository
        {
            get
            {
                if (_deleteWorkflowTypeRepository == null)
                    _deleteWorkflowTypeRepository = new DeleteWorkFlowTypeRepository(_context, _mapper);
                return _deleteWorkflowTypeRepository;
            }
        }
        //*/
        public IAgendaRepository AgendaRepository
        {
            get
            {
                if (_agendaRepository == null)
                    _agendaRepository = new AgendaRepository(_context, _mapper);
                return _agendaRepository;
            }
        }
        public IAgendaGroupRepository AgendaGroupRepository
        {
            get
            {
                if (_agendaGroupRepository == null)
                    _agendaGroupRepository = new AgendaGroupRepository(_context, _mapper);
                return _agendaGroupRepository;
            }
        }
        public ITicketActionRepository TicketActionRepository
        {
            get
            {
                if (_ticketActionRepository == null)
                    _ticketActionRepository = new TicketActionRepository(_context, _mapper);
                return _ticketActionRepository;
            }
        }
        public ITicketTargetRepository TicketTargetRepository
        {
            get
            {
                if (_ticketTargetRepository == null)
                    _ticketTargetRepository = new TicketTargetRepository(_context, _mapper);
                return _ticketTargetRepository;
            }
        }
        public ITicketOptionNoteRepository TicketOptionNoteRepository
        {
            get
            {
                if (_ticketOptionNoteRepository == null)
                    _ticketOptionNoteRepository = new TicketOptionNoteRepository(_context, _mapper);
                return _ticketOptionNoteRepository;
            }
        }
        public ITicketRepository TicketRepository
        {
            get
            {
                if (_ticketRepository == null)
                    _ticketRepository = new TicketRepository(_context, _mapper);
                return _ticketRepository;
            }
        }

        public IIntegrationRepository IntegrationRepository
        {
            get
            {
                if (_integrationRepository == null)
                    _integrationRepository = new IntegrationRepository(_context, _mapper);
                return _integrationRepository;
            }
        }

        public ILoadOtherLibraryRepository LoadOtherLibraryRepository
        {
            get
            {
                if (_LoadOtherLibraryRepository == null)
                    _LoadOtherLibraryRepository = new LoadOtherLibraryRepository(_context, _mapper);

                return _LoadOtherLibraryRepository;
            }
        }
        public IStepListRepository StepListRepository
        {
            get
            {
                if (_stepListRepository == null)
                    _stepListRepository = new StepListRepository(_context, _mapper);

                return _stepListRepository;
            }
        }
        public IStepAddRepository StepAddRepository
        {
            get
            {
                if (_stepAddRepository == null)
                    _stepAddRepository = new StepAddRepository(_context, _mapper);

                return _stepAddRepository;
            }
        }
        public IStepEditRepository StepEditRepository
        {
            get
            {
                if (_stepEditRepository == null)
                    _stepEditRepository = new StepEditRepository(_context, _mapper);

                return _stepEditRepository;
            }
        }
        public IStepActionIncomeItemStatusRepository StepActionIncomeItemStatusRepository
        {
            get
            {
                if (_stepActionIncomeItemStatusRepository == null)
                    _stepActionIncomeItemStatusRepository = new StepActionIncomeItemStatusRepository(_context, _mapper);

                return _stepActionIncomeItemStatusRepository;
            }
        }
        public IActionRepository ActionRepository
        {
            get
            {
                if (_actionRepository == null)
                    _actionRepository = new ActionRepository(_context, _mapper);

                return _actionRepository;
            }
        }

        public IAllOtherInventoryInstRepository AllOtherInventoryInstRepository
        {
            get
            {
                if (_AllOtherInventoryInstRepository == null)
                    _AllOtherInventoryInstRepository = new AllOtherInventoryInstRepository(_context, _mapper);

                return _AllOtherInventoryInstRepository;
            }
        }
        public IStepActionRepository StepActionRepository
        {
            get
            {
                if (_stepActionListRepository == null)
                    _stepActionListRepository = new StepActionRepository(_context, _mapper);
                return _stepActionListRepository;
            }
        }
        public IStepActionMailFromRepository StepActionMailFromRepository
        {
            get
            {
                if (_stepActionMailFromRepository == null)
                    _stepActionMailFromRepository = new StepActionMailFromRepository(_context, _mapper);
                return _stepActionMailFromRepository;
            }
        }
        public IStepActionGroupRepository StepActionGroupRepository
        {
            get
            {
                if (_stepActionGroupRepository == null)
                    _stepActionGroupRepository = new StepActionGroupRepository(_context, _mapper);
                return _stepActionGroupRepository;
            }
        }
        public IStepActionPartRepository StepActionPartRepository
        {
            get
            {
                if (_stepActionPartRepository == null)
                    _stepActionPartRepository = new StepActionPartRepository(_context, _mapper);
                return _stepActionPartRepository;
            }
        }
        public IStepActionPartGroupRepository StepActionPartGroupRepository
        {
            get
            {
                if (_stepActionPartGroupRepository == null)
                    _stepActionPartGroupRepository = new StepActionPartGroupRepository(_context, _mapper);
                return _stepActionPartGroupRepository;
            }
        }

        public IStepActionMailToRepository StepActionMailToRepository
        {
            get
            {
                if (_stepActionMailToRepository == null)
                    _stepActionMailToRepository = new StepActionMailToRepository(_context, _mapper);
                return _stepActionMailToRepository;
            }
        }

        public IStepActionFileGroupRepository StepActionFileGroupRepository
        {
            get
            {
                if (_stepActionFileGroupRepository == null)
                    _stepActionFileGroupRepository = new StepActionFileGroupRepository(_context, _mapper);
                return _stepActionFileGroupRepository;
            }
        }

        public IStepActionOptionRepository StepActionOptionRepository
        {
            get
            {
                if (_stepActionOptionRepository == null)
                    _stepActionOptionRepository = new StepActionOptionRepository(_context, _mapper);
                return _stepActionOptionRepository;
            }
        }

        public IStepActionItemOptionRepository StepActionItemOptionRepository
        {
            get
            {
                if (_stepActionItemOptionRepository == null)
                    _stepActionItemOptionRepository = new StepActionItemOptionRepository(_context, _mapper);
                return _stepActionItemOptionRepository;
            }
        }

        public IPartRepository PartRepository
        {
            get
            {
                if (_partRepository == null)
                    _partRepository = new PartRepository(_context, _mapper);
                return _partRepository;
            }
        }

        public ITablesNamesRepository TablesNamesRepository
        {
            get
            {
                if (_TablesNamesRepository == null)
                    _TablesNamesRepository = new TablesNamesRepository(_context, _mapper);

                return _TablesNamesRepository;
            }
        }
        public IActionItemOptionListRepository ActionItemOptionListRepository
        {
            get
            {
                if (_actionItemOptionListRepository == null)
                    _actionItemOptionListRepository = new ActionItemOptionListRepository(_context, _mapper);

                return _actionItemOptionListRepository;
            }
        }
        public IUserPermissionssRepository UserPermissionssRepository
        {
            get
            {
                if (_UserPermissionssRepository == null)
                    _UserPermissionssRepository = new UserPermissionssRepository(_context, _mapper);

                return _UserPermissionssRepository;
            }
        }
        /*
        public IActionItemOptionAddRepository ActionItemOptionAddRepository {
            get
            {
                if (_actionItemOptionAddRepository == null)
                    _actionItemOptionAddRepository = new ActionItemOptionAddRepository(_context, _mapper);

                return _actionItemOptionAddRepository;
            }
        }
        public IActionItemOptionEditRepository ActionItemOptionEditRepository {
            get
            {
                if (_actionItemOptionEditRepository == null)
                    _actionItemOptionEditRepository = new ActionItemOptionEditRepository(_context, _mapper);

                return _actionItemOptionEditRepository;
            }
        }
        //*/

        public IAttachedFilesRepository AttachedFilesRepository
        {
            get
            {
                if (_AttachedFilesRepository == null)
                    _AttachedFilesRepository = new AttachedFilesRepository(_context, _mapper);

                return _AttachedFilesRepository;
            }
        }
        public IEnforcmentCategoryRepository EnforcmentCategoryRepository
        {
            get
            {
                if (_EnforcmentCategoryRepository == null)
                    _EnforcmentCategoryRepository = new EnforcmentCategoryRepository(_context, _mapper);

                return _EnforcmentCategoryRepository;
            }
        }
        public ILogUsersActionsRepository LogUsersActionsRepository
        {
            get
            {
                if (_LogUsersActionsRepository == null)
                    _LogUsersActionsRepository = new LogUsersActionsRepository(_context, _mapper);

                return _LogUsersActionsRepository;
            }
        }
        public IPowerTypeRepository PowerTypeRepository
        {
            get
            {
                if (_PowerTypeRepository == null)
                    _PowerTypeRepository = new PowerTypeRepository(_context, _mapper);

                return _PowerTypeRepository;
            }
        }

        public IInstallationTypeRepository InstallationTypeRepository
        {
            get
            {
                if (_InstallationTypeRepository == null)
                    _InstallationTypeRepository = new InstallationTypeRepository(_context, _mapper);

                return _InstallationTypeRepository;
            }
        }

        public ICivilLoadLegsRepository CivilLoadLegsRepository
        {
            get
            {
                if (_CivilLoadLegsRepository == null)
                    _CivilLoadLegsRepository = new CivilLoadLegsRepository(_context, _mapper);

                return _CivilLoadLegsRepository;
            }
        }

        public ICivilLoadsRepository CivilLoadsRepository
        {
            get
            {
                if (_CivilLoadsRepository == null)
                    _CivilLoadsRepository = new CivilLoadsRepository(_context, _mapper);

                return _CivilLoadsRepository;
            }
        }
        public IAllLoadInstRepository AllLoadInstRepository
        {
            get
            {
                if (_AllLoadInstRepository == null)
                    _AllLoadInstRepository = new AllLoadInstRepository(_context, _mapper);

                return _AllLoadInstRepository;
            }
        }

        public IAntennaRRUInstRepository AntennaRRUInstRepository
        {
            get
            {
                if (_AntennaRRUInstRepository == null)
                    _AntennaRRUInstRepository = new AntennaRRUInstRepository(_context, _mapper);

                return _AntennaRRUInstRepository;
            }
        }

        public IPowerRepository PowerRepository
        {
            get
            {
                if (_PowerRepository == null)
                    _PowerRepository = new PowerRepository(_context, _mapper);

                return _PowerRepository;
            }
        }

        public IMw_OtherRepository Mw_OtherRepository
        {
            get
            {
                if (_Mw_OtherRepository == null)
                    _Mw_OtherRepository = new Mw_OtherRepository(_context, _mapper);

                return _Mw_OtherRepository;
            }
        }

        public IRadioOtherRepository RadioOtherRepository
        {
            get
            {
                if (_RadioOtherRepository == null)
                    _RadioOtherRepository = new RadioOtherRepository(_context, _mapper);

                return _RadioOtherRepository;
            }
        }

        public ILoadOtherRepository LoadOtherRepository
        {
            get
            {
                if (_LoadOtherRepository == null)
                    _LoadOtherRepository = new LoadOtherRepository(_context, _mapper);

                return _LoadOtherRepository;
            }
        }

        public ISideArmTypeRepository SideArmTypeRepository
        {
            get
            {
                if (_SideArmTypeRepository == null)
                    _SideArmTypeRepository = new SideArmTypeRepository(_context, _mapper);

                return _SideArmTypeRepository;
            }
        }
        public IAreaRepository AreaRepository
        {
            get
            {
                if (_AreaRepository == null)
                    _AreaRepository = new AreaRepository(_context, _mapper);

                return _AreaRepository;
            }
        }
        public ILogistcalRepository LogistcalRepository
        {
            get
            {
                if (_LogistcalRepository == null)
                    _LogistcalRepository = new LogisticalRepository(_context, _mapper);

                return _LogistcalRepository;
            }
        }
        public ITablePartNameRepository TablePartNameRepository
        {
            get
            {
                if (_TablePartNameRepository == null)
                    _TablePartNameRepository = new TablepartNameRepository(_context, _mapper);

                return _TablePartNameRepository;
            }
        }

        public IWorkflowHistoryRepository WorkflowHistoryRepository
        {
            get
            {
                if (_WorkflowHistoryRepository == null)
                    _WorkflowHistoryRepository = new WorkflowHistoryRepository(_context, _mapper);

                return _WorkflowHistoryRepository;
            }
        }
        public IBaseBURepository BaseBURepository
        {
            get
            {
                if (_BaseBURepository == null)
                    _BaseBURepository = new BaseBURepository(_context, _mapper);

                return _BaseBURepository;
            }
        }
        public IDocumentTypeRepository DocumentTypeRepository
        {
            get
            {
                if (_DocumentTypeRepository == null)
                    _DocumentTypeRepository = new DocumentTypeRepository(_context, _mapper);

                return _DocumentTypeRepository;
            }
        }
        public ILocationTypeRepository LocationTypeRepository
        {
            get
            {
                if (_LocationTypeRepository == null)
                    _LocationTypeRepository = new LocationTypeRepository(_context, _mapper);

                return _LocationTypeRepository;
            }
        }
        public IRegionRepository RegionRepository
        {
            get
            {
                if (_RegionRepository == null)
                    _RegionRepository = new RegionRepositoy(_context, _mapper);

                return _RegionRepository;
            }
        }
        
        public IRolePermissionsRepository RolePermissionsRepository
        {
            get
            {
                if (_RolePermissionsRepository == null)
                    _RolePermissionsRepository = new RolePermissionsRepository(_context, _mapper);

                return _RolePermissionsRepository;
            }
        }   
        public int SaveChanges()
        {
            return _context.SaveChanges();
        }

        public Task<int> SaveChangesAsync()
        {
            return _context.SaveChangesAsync();
        }

    }
}
