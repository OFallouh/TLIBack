using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Engineering;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using TLIS_DAL;
using TLIS_DAL.Helper;
using TLIS_DAL.Helper.Filters;
using TLIS_DAL.Helpers;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModelBase;
using TLIS_DAL.ViewModels.AsTypeDTOs;
using TLIS_DAL.ViewModels.AttActivatedCategoryDTOs;
using TLIS_DAL.ViewModels.BaseBUDTOs;
using TLIS_DAL.ViewModels.BaseCivilWithLegsTypeDTOs;
using TLIS_DAL.ViewModels.BaseGeneratorTypeDTOs;
using TLIS_DAL.ViewModels.BaseTypeDTOs;
using TLIS_DAL.ViewModels.BoardTypeDTOs;
using TLIS_DAL.ViewModels.CabinetPowerTypeDTOs;
using TLIS_DAL.ViewModels.CapacityDTOs;
using TLIS_DAL.ViewModels.CivilNonSteelLibraryDTOs;
using TLIS_DAL.ViewModels.CivilSteelSupportCategoryDTOs;
using TLIS_DAL.ViewModels.DataTypeDTOs;
using TLIS_DAL.ViewModels.DiversityTypeDTOs;
using TLIS_DAL.ViewModels.DocumentTypeDTOs;
using TLIS_DAL.ViewModels.DynamicListValuesDTOs;
using TLIS_DAL.ViewModels.EnforcmentCategoryDTOs;
using TLIS_DAL.ViewModels.GuyLineTypeDTOs;
using TLIS_DAL.ViewModels.InstallationCivilwithoutLegsTypeDTOs;
using TLIS_DAL.ViewModels.InstallationPlaceDTOs;
using TLIS_DAL.ViewModels.ItemConnectToDTOs;
using TLIS_DAL.ViewModels.ItemStatusDTOs;
using TLIS_DAL.ViewModels.LocationTypeDTOs;
using TLIS_DAL.ViewModels.LogicalOperationDTOs;
using TLIS_DAL.ViewModels.LogisticalTypeDTOs;
using TLIS_DAL.ViewModels.OduInstallationTypeDTOs;
using TLIS_DAL.ViewModels.OperationDTOs;
using TLIS_DAL.ViewModels.OwnerDTOs;
using TLIS_DAL.ViewModels.ParityDTOs;
using TLIS_DAL.ViewModels.PolarityOnLocationDTOs;
using TLIS_DAL.ViewModels.PolarityTypeDTOs;
using TLIS_DAL.ViewModels.PowerTypeDTOs;
using TLIS_DAL.ViewModels.RenewableCabinetTypeDTOs;
using TLIS_DAL.ViewModels.RepeaterTypeDTOs;
using TLIS_DAL.ViewModels.SectionsLegTypeDTOs;
using TLIS_DAL.ViewModels.SideArmInstallationPlaceDTOs;
using TLIS_DAL.ViewModels.SideArmTypeDTOs;
using TLIS_DAL.ViewModels.StructureTypeDTOs;
using TLIS_DAL.ViewModels.SubTypeDTOs;
using TLIS_DAL.ViewModels.SupportTypeDesignedDTOs;
using TLIS_DAL.ViewModels.SupportTypeImplementedDTOs;
using TLIS_DAL.ViewModels.TelecomTypeDTOs;
using TLIS_Repository.Base;
using TLIS_Service.Helpers;
using TLIS_Service.IService;
using static TLIS_Service.Helpers.Constants;

namespace TLIS_Service.Services
{
    public class ConfigurationAttsService : IConfigurationAttsService
    {
        IUnitOfWork _unitOfWork;
        IServiceCollection _services;
        private IMapper _mapper;
        public ConfigurationAttsService(IUnitOfWork unitOfWork, IServiceCollection services,IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _services = services;
            _mapper = mapper;
        }
        public Response<ConfigurationAttsViewModel> Add(AddConfigrationAttViewModel Viewmodel)
        {
            try
            {
                if(string.IsNullOrWhiteSpace(Viewmodel.Name) || string.IsNullOrEmpty(Viewmodel.Name))
                    return new Response<ConfigurationAttsViewModel>(true, null, null, $"You Can't Add Empty Name", (int)Helpers.Constants.ApiReturnCode.fail);

                string TableName = Viewmodel.TableName;
                ConfigurationAttsViewModel model = new ConfigurationAttsViewModel(Viewmodel.Name);

                if (ConfigrationTables.TLIdiversityType.ToString() == TableName)
                {
                    var diversityType = _mapper.Map<AddDiversityTypeViewModel>(model);
                    var diversityTypeEntity = _mapper.Map<TLIdiversityType>(diversityType);
                    if(!ValidateAdd(ConfigrationTables.TLIdiversityType.ToString(), diversityTypeEntity))
                    {
                        _unitOfWork.DiversityTypeRepository.Add(diversityTypeEntity);
                        _unitOfWork.SaveChanges();
                    }
                    else
                    {
                        return new Response<ConfigurationAttsViewModel>(true, null, null, $"This diversityType {diversityTypeEntity.Name} is already exists", (int)Helpers.Constants.ApiReturnCode.fail);
                    }
                    
                }
                else if (ConfigrationTables.TLIdataType.ToString() == TableName)
                {
                    var dataType = _mapper.Map<AddDataTypeViewModel>(model);
                    var dataTypeEntity = _mapper.Map<TLIdataType>(dataType);
                    if (!ValidateAdd(ConfigrationTables.TLIdataType.ToString(), dataTypeEntity))
                    {
                        _unitOfWork.DataTypeRepository.Add(dataTypeEntity);
                        _unitOfWork.SaveChanges();
                    }
                    else
                    {
                        return new Response<ConfigurationAttsViewModel>(true, null, null, $"This dataType {dataTypeEntity.Name} is already exists", (int)Helpers.Constants.ApiReturnCode.fail);
                    }

                }
                else if (ConfigrationTables.TLIoperation.ToString() == TableName)
                {
                    var operation = _mapper.Map<AddOperationViewModel>(model);
                    var operationEntity = _mapper.Map<TLIoperation>(operation);
                    if (!ValidateAdd(ConfigrationTables.TLIoperation.ToString(), operationEntity))
                    {
                        _unitOfWork.OperationRepository.Add(operationEntity);
                        _unitOfWork.SaveChanges();
                    }
                    else
                    {
                        return new Response<ConfigurationAttsViewModel>(true, null, null, $"This operation {operationEntity.Name} is already exists", (int)Helpers.Constants.ApiReturnCode.fail);
                    }

                }
                else if (ConfigrationTables.TLIlogicalOperation.ToString() == TableName)
                {
                    var logicalOperation = _mapper.Map<AddLogicalOperationViewModel>(model);
                    var logicalOperationEntity = _mapper.Map<TLIlogicalOperation>(logicalOperation);
                    if (!ValidateAdd(ConfigrationTables.TLIlogicalOperation.ToString(), logicalOperationEntity))
                    {
                        _unitOfWork.LogicalOperationRepository.Add(logicalOperationEntity);
                        _unitOfWork.SaveChanges();
                    }
                    else
                    {
                        return new Response<ConfigurationAttsViewModel>(true, null, null, $"This logical operation {logicalOperationEntity.Name} is already exists", (int)Helpers.Constants.ApiReturnCode.fail);
                    }

                }
                else if (ConfigrationTables.TLItelecomType.ToString() == TableName)
                {
                    var telecomType = _mapper.Map<AddTelecomTypeViewModel>(model);
                    var telecomTypeEntity = _mapper.Map<TLItelecomType>(telecomType);
                    if (!ValidateAdd(ConfigrationTables.TLItelecomType.ToString(),telecomTypeEntity))
                    {
                        _unitOfWork.TelecomTypeRepository.Add(telecomTypeEntity);
                        _unitOfWork.SaveChanges();
                    }
                    else
                    {
                        return new Response<ConfigurationAttsViewModel>(true, null, null, $"This TelecomType {telecomTypeEntity.Name} is already exists", (int)Helpers.Constants.ApiReturnCode.fail);
                    }
                   
                }
                else if (ConfigrationTables.TLIsideArmInstallationPlace.ToString() == TableName)
                {
                    var sideArmInstallationPlace = _mapper.Map<AddSideArmInstallationPlaceViewModel>(model);
                    var sideArmInstallationPlaceEntity = _mapper.Map<TLIsideArmInstallationPlace>(sideArmInstallationPlace);
                    if(!ValidateAdd(ConfigrationTables.TLIsideArmInstallationPlace.ToString(),sideArmInstallationPlaceEntity))
                    {
                        _unitOfWork.SideArmInstallationPlaceRepository.Add(sideArmInstallationPlaceEntity);
                        _unitOfWork.SaveChanges();
                    }
                    else
                    {
                        return new Response<ConfigurationAttsViewModel>(true, null, null, $"This sideArmInstallationPlace {sideArmInstallationPlaceEntity.Name} is already exists", (int)Helpers.Constants.ApiReturnCode.fail);
                    }
                }
                else if (ConfigrationTables.TLIoduInstallationType.ToString() == TableName)
                {
                    var oduInstallationType = _mapper.Map<AddOduInstallationTypeViewModel>(model);
                    var oduInstallationTypeEntity = _mapper.Map<TLIoduInstallationType>(oduInstallationType);
                    if (!ValidateAdd(ConfigrationTables.TLIoduInstallationType.ToString(), oduInstallationTypeEntity))
                    {
                        _unitOfWork.OduInstallationTypeRepository.Add(oduInstallationTypeEntity);
                        _unitOfWork.SaveChanges();
                    }
                      else
                      {
                        return new Response<ConfigurationAttsViewModel>(true, null, null, $"This oduInstallationType {oduInstallationTypeEntity.Name} is already exists", (int)Helpers.Constants.ApiReturnCode.fail);
                      }
                }
                else if (ConfigrationTables.TLIrepeaterType.ToString() == TableName)
                {
                    var repeaterType = _mapper.Map<AddRepeaterTypeViewModel>(model);
                    var repeaterTypeEntity = _mapper.Map<TLIrepeaterType>(repeaterType);
                    if (!ValidateAdd(ConfigrationTables.TLIrepeaterType.ToString(), repeaterTypeEntity))
                    {
                        _unitOfWork.RepeaterTypeRepository.Add(repeaterTypeEntity);
                        _unitOfWork.SaveChanges();
                    }
                    else
                    {
                        return new Response<ConfigurationAttsViewModel>(true, null, null, $"This repeaterType {repeaterTypeEntity.Name} is already exists", (int)Helpers.Constants.ApiReturnCode.fail);
                    }

                }
                else if (ConfigrationTables.TLIitemConnectTo.ToString() == TableName)
                {
                    var itemConnectTo = _mapper.Map<AddTelecomTypeViewModel>(model);
                    var itemConnectToEntity = _mapper.Map<TLIitemConnectTo>(itemConnectTo);
                    if (!ValidateAdd(ConfigrationTables.TLIitemConnectTo.ToString(),itemConnectToEntity))
                    {
                        _unitOfWork.ItemConnectToRepository.Add(itemConnectToEntity);
                        _unitOfWork.SaveChanges();
                    }
                  else
                    {
                        return new Response<ConfigurationAttsViewModel>(true, null, null, $"This itemConnectTo { itemConnectToEntity.Name } is already exists", (int)Helpers.Constants.ApiReturnCode.fail);
                    }
                }
                else if (ConfigrationTables.TLIsupportTypeDesigned.ToString() == TableName)
                {
                    var supportTypeDesigned = _mapper.Map<AddSupportTypeDesignedViewModel>(model);
                    var supportTypeDesignedEntity = _mapper.Map<TLIsupportTypeDesigned>(supportTypeDesigned);
                    if (!ValidateAdd(ConfigrationTables.TLIsupportTypeDesigned.ToString(),supportTypeDesignedEntity))
                    {
                        _unitOfWork.SupportTypeDesignedRepository.Add(supportTypeDesignedEntity);
                        _unitOfWork.SaveChanges();
                    }
                    else
                    {
                        return new Response<ConfigurationAttsViewModel>(true, null, null, $"This supportTypeDesigned { supportTypeDesigned.Name } is already exists", (int)Helpers.Constants.ApiReturnCode.fail);
                    }
                }
                else if (ConfigrationTables.TLIpolarityOnLocation.ToString() == TableName)
                {
                    var polarityOnLocation = _mapper.Map<AddPolarityOnLocationViewModel>(model);
                    var polarityOnLocationEntity = _mapper.Map<TLIpolarityOnLocation>(polarityOnLocation);
                    if (!ValidateAdd(ConfigrationTables.TLIpolarityOnLocation.ToString(),polarityOnLocationEntity))
                    {
                        _unitOfWork.PolarityOnLocationRepository.Add(polarityOnLocationEntity);
                        _unitOfWork.SaveChanges();
                    }
                    else
                    {
                        return new Response<ConfigurationAttsViewModel>(true, null, null, $"This polarityOnLocation { polarityOnLocation.Name } is already exists", (int)Helpers.Constants.ApiReturnCode.fail);
                    }
                }
                else if (ConfigrationTables.TLIsupportTypeImplemented.ToString() == TableName)
                {
                    var supportTypeImplemented = _mapper.Map<AddSupportTypeImplementedViewModel>(model);
                    var supportTypeImplementedEntity = _mapper.Map<TLIsupportTypeImplemented>(supportTypeImplemented);
                    if (!ValidateAdd(ConfigrationTables.TLIsupportTypeImplemented.ToString(), supportTypeImplementedEntity))
                    {
                        _unitOfWork.SupportTypeImplementedRepository.Add(supportTypeImplementedEntity);
                        _unitOfWork.SaveChanges();
                    }
                   else
                    {
                        return new Response<ConfigurationAttsViewModel>(true, null, null, $"This supportTypeImplemented { supportTypeImplemented.Name } is already exists", (int)Helpers.Constants.ApiReturnCode.fail);
                    }
                }
                else if (ConfigrationTables.TLIstructureType.ToString() == TableName)
                {
                    var structureType = _mapper.Map<AddStructureTypeViewModel>(model);
                    var structureTypeEntity = _mapper.Map<TLIstructureType>(structureType);
                    if (!ValidateAdd(ConfigrationTables.TLIstructureType.ToString(),structureTypeEntity))
                    {
                        _unitOfWork.StructureTypeRepository.Add(structureTypeEntity);
                        _unitOfWork.SaveChanges();
                    }
                   else
                    {
                        return new Response<ConfigurationAttsViewModel>(true, null, null, $"This structureType { structureType.Name } is already exists", (int)Helpers.Constants.ApiReturnCode.fail);
                    }
                }
                else if (ConfigrationTables.TLIsectionsLegType.ToString() == TableName)
                {
                    var sectionsLegType = _mapper.Map<AddSectionsLegTypeViewModel>(model);
                    var sectionsLegTypeEntity = _mapper.Map<TLIsectionsLegType>(sectionsLegType);
                    if(!ValidateAdd(ConfigrationTables.TLIsectionsLegType.ToString(),sectionsLegTypeEntity))
                    {
                        _unitOfWork.SectionsLegTypeRepository.Add(sectionsLegTypeEntity);
                        _unitOfWork.SaveChanges();
                    }
                   else
                    {
                        return new Response<ConfigurationAttsViewModel>(true, null, null, $"This sectionsLegType { sectionsLegType.Name } is already exists", (int)Helpers.Constants.ApiReturnCode.fail);
                    }
                }
                else if (ConfigrationTables.TLIlogisticalType.ToString() == TableName)
                {
                    var logisticalType = _mapper.Map<AddLogisticalTypeViewModel>(model);
                    var logisticalTypeEntity = _mapper.Map<TLIlogisticalType>(logisticalType);
                    if(!ValidateAdd(ConfigrationTables.TLIlogisticalType.ToString(),logisticalTypeEntity))
                    {
                        _unitOfWork.logisticalTypeRepository.Add(logisticalTypeEntity);
                        _unitOfWork.SaveChanges();
                    }
                  else
                    {
                        return new Response<ConfigurationAttsViewModel>(true, null, null, $"This logisticalType { logisticalType.Name } is already exists", (int)Helpers.Constants.ApiReturnCode.fail);
                    }
                }
                else if (ConfigrationTables.TLIbaseCivilWithLegsType.ToString() == TableName)
                {
                    var baseCivilWithLegsType = _mapper.Map<AddBaseCivilWithLegsTypeViewModel>(model);
                    var baseCivilWithLegsTypeEntity = _mapper.Map<TLIbaseCivilWithLegsType>(baseCivilWithLegsType);
                    if(!ValidateAdd(ConfigrationTables.TLIbaseCivilWithLegsType.ToString(),baseCivilWithLegsTypeEntity))
                    {
                        _unitOfWork.BaseCivilWithLegsTypeRepository.Add(baseCivilWithLegsTypeEntity);
                        _unitOfWork.SaveChanges();
                    }
                    else
                    {
                        return new Response<ConfigurationAttsViewModel>(true, null, null, $"This baseCivilWithLegsType { baseCivilWithLegsType.Name } is already exists", (int)Helpers.Constants.ApiReturnCode.fail);
                    }
                }
                else if (ConfigrationTables.TLIbaseGeneratorType.ToString() == TableName)
                {
                    var baseGeneratorType = _mapper.Map<AddBaseGeneratorTypeViewModel>(model);
                    var baseGeneratorTypeEntity = _mapper.Map<TLIbaseGeneratorType>(baseGeneratorType);
                    if(!ValidateAdd(ConfigrationTables.TLIbaseGeneratorType.ToString(),baseGeneratorTypeEntity))
                    {
                        _unitOfWork.BaseGeneratorTypeRepository.Add(baseGeneratorTypeEntity);
                        _unitOfWork.SaveChanges();
                    }
                   else
                    {
                        return new Response<ConfigurationAttsViewModel>(true, null, null, $"This baseGeneratorType { baseGeneratorType.Name } is already exists", (int)Helpers.Constants.ApiReturnCode.fail);
                    }
                }
                else if (ConfigrationTables.TLIinstallationCivilwithoutLegsType.ToString() == TableName)
                {
                    var installationCivilwithoutLegsType = _mapper.Map<AddInstallationCivilwithoutLegsTypeViewModel>(model);
                    var installationCivilwithoutLegsTypeEntity = _mapper.Map<TLIinstallationCivilwithoutLegsType>(installationCivilwithoutLegsType);
                    if(!ValidateAdd(ConfigrationTables.TLIinstallationCivilwithoutLegsType.ToString(),installationCivilwithoutLegsTypeEntity))
                    {
                        _unitOfWork.InstallationCivilwithoutLegsTypeRepository.Add(installationCivilwithoutLegsTypeEntity);
                        _unitOfWork.SaveChanges();
                    }
                  else
                    {
                        return new Response<ConfigurationAttsViewModel>(true, null, null, $"This installationCivilwithoutLegsType { installationCivilwithoutLegsType.Name } is already exists", (int)Helpers.Constants.ApiReturnCode.fail);
                    }
                }
                else if (ConfigrationTables.TLIboardType.ToString() == TableName)
                {
                    var boardType = _mapper.Map<AddBoardTypeViewModel>(model);
                    var boardTypeEntity = _mapper.Map<TLIboardType>(boardType);
                    if(!ValidateAdd(ConfigrationTables.TLIboardType.ToString(),boardTypeEntity))
                    {
                        _unitOfWork.BoardTypeRepository.Add(boardTypeEntity);
                        _unitOfWork.SaveChanges();
                    }
                   else
                    {
                        return new Response<ConfigurationAttsViewModel>(true, null, null, $"This boardType { boardType.Name } is already exists", (int)Helpers.Constants.ApiReturnCode.fail);
                    }
                }
                else if (ConfigrationTables.TLIguyLineType.ToString() == TableName)
                {
                    var guyLineType = _mapper.Map<AddGuyLineTypeViewModel>(model);
                    var guyLineTypeEntity = _mapper.Map<TLIguyLineType>(guyLineType);
                    if(!ValidateAdd(ConfigrationTables.TLIguyLineType.ToString(),guyLineTypeEntity))
                    {
                        _unitOfWork.GuyLineTypeRepository.Add(guyLineTypeEntity);
                        _unitOfWork.SaveChanges();
                    }
                   else
                    {
                        return new Response<ConfigurationAttsViewModel>(true, null, null, $"This guyLineType { guyLineType.Name } is already exists", (int)Helpers.Constants.ApiReturnCode.fail);
                    }
                }
                else if (ConfigrationTables.TLIenforcmentCategory.ToString() == TableName)
                {
                    var enforcmentCategory = _mapper.Map<AddEnforcmentCategoryViewModel>(model);
                    var enforcmentCategoryEntity = _mapper.Map<TLIenforcmentCategory>(enforcmentCategory);
                    if (!ValidateAdd(ConfigrationTables.TLIenforcmentCategory.ToString(), enforcmentCategoryEntity))
                    {
                        _unitOfWork.EnforcmentCategoryRepository.Add(enforcmentCategoryEntity);
                        _unitOfWork.SaveChanges();
                    }
                    else
                    {
                        return new Response<ConfigurationAttsViewModel>(true, null, null, $"This enforcmentCategory { enforcmentCategory.Name } is already exists", (int)Helpers.Constants.ApiReturnCode.fail);
                    }
                }
                else if (ConfigrationTables.TLIdocumentType.ToString() == TableName)
                {
                    var documentType = _mapper.Map<AddDocumentTypeViewModel>(model);
                    var documentTypeEntity = _mapper.Map<TLIdocumentType>(documentType);
                    if (!ValidateAdd(ConfigrationTables.TLIdocumentType.ToString(), documentTypeEntity))
                    {
                        _unitOfWork.DocumentTypeRepository.Add(documentTypeEntity);
                        _unitOfWork.SaveChanges();
                    }
                    else
                    {
                        return new Response<ConfigurationAttsViewModel>(true, null, null, $"This documentType { documentType.Name } is already exists", (int)Helpers.Constants.ApiReturnCode.fail);
                    }
                }
                else if (ConfigrationTables.TLIpowerType.ToString() == TableName)
                {
                    var powerType = _mapper.Map<AddPowerTypeViewModel>(model);
                    var powerTypeEntity = _mapper.Map<TLIpowerType>(powerType);
                    if (!ValidateAdd(ConfigrationTables.TLIpowerType.ToString(), powerTypeEntity))
                    {
                        _unitOfWork.PowerTypeRepository.Add(powerTypeEntity);
                        _unitOfWork.SaveChanges();
                    }
                    else
                    {
                        return new Response<ConfigurationAttsViewModel>(true, null, null, $"This powerType { powerType.Name } is already exists", (int)Helpers.Constants.ApiReturnCode.fail);
                    }
                }

                else if (ConfigrationTables.TLIcivilSteelSupportCategory.ToString() == TableName)
                {
                    var civilSteelSupportCategory = _mapper.Map<AddCivilSteelSupportCategoryViewModel>(model);
                    var civilSteelSupportCategoryEntity = _mapper.Map<TLIcivilSteelSupportCategory>(civilSteelSupportCategory);
                    if (!ValidateAdd(ConfigrationTables.TLIcivilSteelSupportCategory.ToString(), civilSteelSupportCategoryEntity))
                    {
                        _unitOfWork.CivilSteelSupportCategoryRepository.Add(civilSteelSupportCategoryEntity);
                        _unitOfWork.SaveChanges();
                    }
                    else
                    {
                        return new Response<ConfigurationAttsViewModel>(true, null, null, $"This civilSteelSupportCategory {civilSteelSupportCategoryEntity.Name} is already exists", (int)Helpers.Constants.ApiReturnCode.fail);
                    }

                }

                else if (ConfigrationTables.TLIcapacity.ToString() == TableName)
                {
                    var capacity = _mapper.Map<AddCapacityViewModel>(model);
                    var capacityEntity = _mapper.Map<TLIcapacity>(capacity);
                    if (!ValidateAdd(ConfigrationTables.TLIcapacity.ToString(), capacityEntity))
                    {
                        _unitOfWork.CapacityRepository.Add(capacityEntity);
                        _unitOfWork.SaveChanges();
                    }
                    else
                    {
                        return new Response<ConfigurationAttsViewModel>(true, null, null, $"This capacity {capacityEntity.Name} is already exists", (int)Helpers.Constants.ApiReturnCode.fail);
                    }

                }

                else if (ConfigrationTables.TLIcabinetPowerType.ToString() == TableName)
                {
                    AddCabinetPowerTypeViewModel cabinetPowerType = _mapper.Map<AddCabinetPowerTypeViewModel>(model);
                    TLIcabinetPowerType cabinetPowerTypeEntity = _mapper.Map<TLIcabinetPowerType>(cabinetPowerType);
                    if (!ValidateAdd(ConfigrationTables.TLIcabinetPowerType.ToString(), cabinetPowerTypeEntity))
                    {
                        _unitOfWork.CabinetPowerTypeRepository.Add(cabinetPowerTypeEntity);
                        _unitOfWork.SaveChanges();
                    }
                    else
                    {
                        return new Response<ConfigurationAttsViewModel>(true, null, null, $"This cabinetPowerType {cabinetPowerTypeEntity.Name} is already exists", (int)Helpers.Constants.ApiReturnCode.fail);
                    }

                }

                else if (ConfigrationTables.TLIcivilNonSteelType.ToString() == TableName)
                {
                    var civilNonSteelType = _mapper.Map<AddcivilNonSteelTypeViewModel>(model);
                    var civilNonSteelTypeEntity = _mapper.Map<TLIcivilNonSteelType>(civilNonSteelType);
                    if (!ValidateAdd(ConfigrationTables.TLIcivilNonSteelType.ToString(), civilNonSteelTypeEntity))
                    {
                        _unitOfWork.CivilNonSteelTypeRepository.Add(civilNonSteelTypeEntity);
                        _unitOfWork.SaveChanges();
                    }
                    else
                    {
                        return new Response<ConfigurationAttsViewModel>(true, null, null, $"This civilNonSteelType {civilNonSteelTypeEntity.Name} is already exists", (int)Helpers.Constants.ApiReturnCode.fail);
                    }

                }

                else if (ConfigrationTables.TLIasType.ToString() == TableName)
                {
                    var asType = _mapper.Map<AddAsTypeViewModel>(model);
                    var TLIasTypeEntity = _mapper.Map<TLIasType>(asType);
                    if (!ValidateAdd(ConfigrationTables.TLIasType.ToString(), TLIasTypeEntity))
                    {
                        _unitOfWork.AsTypeRepository.Add(TLIasTypeEntity);
                        _unitOfWork.SaveChanges();
                    }
                    else
                    {
                        return new Response<ConfigurationAttsViewModel>(true, null, null, $"This TLIasType {TLIasTypeEntity.Name} is already exists", (int)Helpers.Constants.ApiReturnCode.fail);
                    }

                }

                else if (ConfigrationTables.TLIpolarityType.ToString() == TableName)
                {
                    var polarityType = _mapper.Map<AddPolarityTypeViewModel>(model);
                    var polarityTypeEntity = _mapper.Map<TLIpolarityType>(polarityType);
                    if (!ValidateAdd(ConfigrationTables.TLIpolarityType.ToString(), polarityTypeEntity))
                    {
                        _unitOfWork.PolarityTypeRepository.Add(polarityTypeEntity);
                        _unitOfWork.SaveChanges();
                    }
                    else
                    {
                        return new Response<ConfigurationAttsViewModel>(true, null, null, $"This polarityType {polarityTypeEntity.Name} is already exists", (int)Helpers.Constants.ApiReturnCode.fail);
                    }

                }

                else if (ConfigrationTables.TLIparity.ToString() == TableName)
                {
                    var parity = _mapper.Map<AddParityViewModel>(model);
                    var parityEntity = _mapper.Map<TLIparity>(parity);
                    if (!ValidateAdd(ConfigrationTables.TLIparity.ToString(), parityEntity))
                    {
                        _unitOfWork.ParityRepository.Add(parityEntity);
                        _unitOfWork.SaveChanges();
                    }
                    else
                    {
                        return new Response<ConfigurationAttsViewModel>(true, null, null, $"This parity {parityEntity.Name} is already exists", (int)Helpers.Constants.ApiReturnCode.fail);
                    }

                }

                else if (ConfigrationTables.TLIsubType.ToString() == TableName)
                {
                    var subType = _mapper.Map<AddTLIsubTypeViewModel>(model);
                    var subTypeEntity = _mapper.Map<TLIsubType>(subType);
                    if (!ValidateAdd(ConfigrationTables.TLIsubType.ToString(), subTypeEntity))
                    {
                        _unitOfWork.SubTypeRepository.Add(subTypeEntity);
                        _unitOfWork.SaveChanges();
                    }
                    else
                    {
                        return new Response<ConfigurationAttsViewModel>(true, null, null, $"This civilSteelSupportCategory {subTypeEntity.Name} is already exists", (int)Helpers.Constants.ApiReturnCode.fail);
                    }

                }

                //////////////////////////////////////////
                
                else if (ConfigrationTables.TLIowner.ToString() == TableName)
                {
                    AddOwnerViewModel AddOwnerViewModel = _mapper.Map<AddOwnerViewModel>(model);
                    TLIowner OwnerEntity = _mapper.Map<TLIowner>(AddOwnerViewModel);
                    if (!ValidateAdd(ConfigrationTables.TLIowner.ToString(), OwnerEntity))
                    {
                        _unitOfWork.OwnerRepository.Add(OwnerEntity);
                        _unitOfWork.SaveChanges();
                    }
                    else
                    {
                        return new Response<ConfigurationAttsViewModel>(true, null, null, $"This Owner Name {OwnerEntity.OwnerName} is already exists", (int)Helpers.Constants.ApiReturnCode.fail);
                    }
                }
                else if (ConfigrationTables.TLIlocationType.ToString() == TableName)
                {
                    AddLocationTypeViewModel AddLocationTypeViewModel = _mapper.Map<AddLocationTypeViewModel>(model);
                    TLIlocationType LocationTypeEntity = _mapper.Map<TLIlocationType>(AddLocationTypeViewModel);
                    if (!ValidateAdd(ConfigrationTables.TLIlocationType.ToString(), LocationTypeEntity))
                    {
                        _unitOfWork.LocationTypeRepository.Add(LocationTypeEntity);
                        _unitOfWork.SaveChanges();
                    }
                    else
                    {
                        return new Response<ConfigurationAttsViewModel>(true, null, null, $"This Location Type Name {LocationTypeEntity.Name} is already exists", (int)Helpers.Constants.ApiReturnCode.fail);
                    }
                }
                else if (ConfigrationTables.TLIbaseType.ToString() == TableName)
                {
                    AddBaseTypeViewModel AddBaseTypeViewModel = _mapper.Map<AddBaseTypeViewModel>(model);
                    TLIbaseType BaseTypeEntity = _mapper.Map<TLIbaseType>(AddBaseTypeViewModel);
                    if (!ValidateAdd(ConfigrationTables.TLIbaseType.ToString(), BaseTypeEntity))
                    {
                        _unitOfWork.BaseTypeRepository.Add(BaseTypeEntity);
                        _unitOfWork.SaveChanges();
                    }
                    else
                    {
                        return new Response<ConfigurationAttsViewModel>(true, null, null, $"This Base Type Name {BaseTypeEntity.Name} is already exists", (int)Helpers.Constants.ApiReturnCode.fail);
                    }
                }
                else if (ConfigrationTables.TLIbaseBU.ToString() == TableName)
                {
                    AddBaseBUViewModel AddBaseBUViewModel = _mapper.Map<AddBaseBUViewModel>(model);
                    TLIbaseBU BaseBUEntity = _mapper.Map<TLIbaseBU>(AddBaseBUViewModel);
                    if (!ValidateAdd(ConfigrationTables.TLIbaseBU.ToString(), BaseBUEntity))
                    {
                        _unitOfWork.BaseBURepository.Add(BaseBUEntity);
                        _unitOfWork.SaveChanges();
                    }
                    else
                    {
                        return new Response<ConfigurationAttsViewModel>(true, null, null, $"This Base BU Name {BaseBUEntity.Name} is already exists", (int)Helpers.Constants.ApiReturnCode.fail);
                    }
                }
                else if (ConfigrationTables.TLIrenewableCabinetType.ToString() == TableName)
                {
                    AddRenewableCabinetTypeViewModel AddRenewableCabinetTypeViewModel = _mapper.Map<AddRenewableCabinetTypeViewModel>(model);
                    TLIrenewableCabinetType RenewableCabinetTypeEntity = _mapper.Map<TLIrenewableCabinetType>(AddRenewableCabinetTypeViewModel);
                    if (!ValidateAdd(ConfigrationTables.TLIrenewableCabinetType.ToString(), RenewableCabinetTypeEntity))
                    {
                        _unitOfWork.RenewableCabinetTypeRepository.Add(RenewableCabinetTypeEntity);
                        _unitOfWork.SaveChanges();
                    }
                    else
                    {
                        return new Response<ConfigurationAttsViewModel>(true, null, null, $"This Renewable Cabinet Type Name {RenewableCabinetTypeEntity.Name} is already exists", (int)Helpers.Constants.ApiReturnCode.fail);
                    }
                }
                else if (ConfigrationTables.TLIsideArmType.ToString() == TableName)
                {
                    AddSideArmTypeViewModel AddSideArmTypeViewModel = _mapper.Map<AddSideArmTypeViewModel>(model);
                    TLIsideArmType SideArmTypeViewModelEntity = _mapper.Map<TLIsideArmType>(AddSideArmTypeViewModel);
                    if (!ValidateAdd(ConfigrationTables.TLIsideArmType.ToString(), SideArmTypeViewModelEntity))
                    {
                        _unitOfWork.SideArmTypeRepository.Add(SideArmTypeViewModelEntity);
                        _unitOfWork.SaveChanges();
                    }
                    else
                    {
                        return new Response<ConfigurationAttsViewModel>(true, null, null, $"This Side Arm Type Name {SideArmTypeViewModelEntity.Name} is already exists", (int)Helpers.Constants.ApiReturnCode.fail);
                    }
                }
                else if (ConfigrationTables.TLIitemStatus.ToString() == TableName)
                {
                    AddItemStatusViewModel AddItemStatusViewModel = _mapper.Map<AddItemStatusViewModel>(model);
                    TLIitemStatus ItemStatusEntity = _mapper.Map<TLIitemStatus>(AddItemStatusViewModel);
                    if (!ValidateAdd(ConfigrationTables.TLIitemStatus.ToString(), ItemStatusEntity))
                    {
                        _unitOfWork.ItemStatusRepository.Add(ItemStatusEntity);
                        _unitOfWork.SaveChanges();
                    }
                    else
                    {
                        return new Response<ConfigurationAttsViewModel>(true, null, null, $"This Item Status Name {ItemStatusEntity.Name} is already exists", (int)Helpers.Constants.ApiReturnCode.fail);
                    }
                }
                else if (ConfigrationTables.TLIinstallationPlace.ToString() == TableName)
                {
                    AddInstallationPlaceViewModel AddInstallationPlaceViewModel = _mapper.Map<AddInstallationPlaceViewModel>(model);
                    TLIinstallationPlace InstallationPlaceEntity = _mapper.Map<TLIinstallationPlace>(AddInstallationPlaceViewModel);
                    if (!ValidateAdd(ConfigrationTables.TLIinstallationPlace.ToString(), InstallationPlaceEntity))
                    {
                        _unitOfWork.InstallationPlaceRepository.Add(InstallationPlaceEntity);
                        _unitOfWork.SaveChanges();
                    }
                    else
                    {
                        return new Response<ConfigurationAttsViewModel>(true, null, null, $"This Installation Place Name {InstallationPlaceEntity.Name} is already exists", (int)Helpers.Constants.ApiReturnCode.fail);
                    }
                }
                else
                {

                    // int DynamicAttID = _unitOfWork.DynamicAttRepository.GetAllAsQueryable().Where(x => x.Key == TableName).Select(x => x.Id).FirstOrDefault();
                   
                    int DynamicAttID = _unitOfWork.DynamicAttRepository.GetWhereSelectFirst(x => x.Key == TableName, x => new { x.Id }).Id;
                    TLIdynamicListValues dynamicListValues = new TLIdynamicListValues();
                    dynamicListValues.Value = ((ConfigurationAttsViewModel)model).Name;
                    dynamicListValues.dynamicAttId = DynamicAttID;
                    var dynamicListValueValidation = _unitOfWork.DynamicListValuesRepository.GetWhereFirst(x => (x.Value == dynamicListValues.Value && x.dynamicAttId == dynamicListValues.dynamicAttId));
                    if (dynamicListValueValidation != null)
                    {
                        return new Response<ConfigurationAttsViewModel>(true, null, null, $"The Name is already exists", (int)Helpers.Constants.ApiReturnCode.fail);
                    }
                    else
                    {
                        _unitOfWork.DynamicListValuesRepository.Add(dynamicListValues);
                        _unitOfWork.SaveChanges();
                    }
                    
                }
                return new Response<ConfigurationAttsViewModel>();
            }
            catch(Exception err)
            {
                return new Response<ConfigurationAttsViewModel>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
        }

        public async Task<Response<ConfigurationAttsViewModel>> Delete(string TableName, int Id)
        {
            try
            {
                if (ConfigrationTables.TLIdiversityType.ToString() == TableName)
                {
                    var diversityType = _unitOfWork.DiversityTypeRepository.GetByID(Id);

                    var TLIMW_BULibrary = _unitOfWork.MW_BULibraryRepository
                        .GetWhere(x => x.diversityTypeId == Id && !x.Deleted).ToList();

                    var MW_RFULibraries = _unitOfWork.MW_RFULibraryRepository
                        .GetWhere(x => x.diversityTypeId == Id && !x.Deleted).ToList();

                    if (TLIMW_BULibrary.Count() > 0 || MW_RFULibraries.Count() > 0)
                    {
                        diversityType.Deleted = (false);
                        return new Response<ConfigurationAttsViewModel>(true, null, null, $"This Record Can Not Be Deleted ", (int)Helpers.Constants.ApiReturnCode.fail);
                    }
                    else
                    {
                        diversityType.Deleted = (true);
                    }
                    _unitOfWork.DiversityTypeRepository.Update(diversityType);
                    await _unitOfWork.SaveChangesAsync();
                }
                else if (ConfigrationTables.TLItelecomType.ToString() == TableName)
                {
                    var telecomType = _unitOfWork.TelecomTypeRepository.GetByID(Id);

                    var TLIcabinetTelecomLibrary = _unitOfWork.CabinetTelecomLibraryRepository
                        .GetWhere(x => x.TelecomTypeId == Id && !x.Deleted).ToList();

                    if (TLIcabinetTelecomLibrary.Count() > 0)
                    {
                        telecomType.Deleted = (false);
                        return new Response<ConfigurationAttsViewModel>(true, null, null, $"This Record Can Not Be Deleted ", (int)Helpers.Constants.ApiReturnCode.fail);
                    }
                    else
                    {
                        telecomType.Deleted = (true);
                    }
                    _unitOfWork.TelecomTypeRepository.Update(telecomType);
                    await _unitOfWork.SaveChangesAsync();
                }
                else if (ConfigrationTables.TLIsupportTypeDesigned.ToString() == TableName)
                {
                    var supportTypeDesigned = _unitOfWork.SupportTypeDesignedRepository.GetByID(Id);

                    var TLIcivilWithLegLibrary = _unitOfWork.CivilWithLegLibraryRepository
                        .GetWhere(x => x.supportTypeDesignedId == Id && !x.Deleted).ToList();

                    if (TLIcivilWithLegLibrary.Count() > 0)
                    {
                        supportTypeDesigned.Deleted = (false);
                        return new Response<ConfigurationAttsViewModel>(true, null, null, $"This Record Can Not Be Deleted ", (int)Helpers.Constants.ApiReturnCode.fail);
                    }
                    else
                    {
                        supportTypeDesigned.Deleted = (true);
                    }
                    _unitOfWork.SupportTypeDesignedRepository.Update(supportTypeDesigned);
                    await _unitOfWork.SaveChangesAsync();
                }
                else if (ConfigrationTables.TLIstructureType.ToString() == TableName)
                {
                    var structureType = _unitOfWork.StructureTypeRepository.GetByID(Id);

                    var CivilWithLegLibraries = _unitOfWork.CivilWithLegLibraryRepository
                        .GetWhere(x => x.structureTypeId == Id && !x.Deleted).ToList();

                    var CivilWithoutLegLibraries = _unitOfWork.CivilWithoutLegLibraryRepository
                        .GetWhere(x => x.structureTypeId == Id && !x.Deleted).ToList();

                    if (CivilWithLegLibraries.Count() > 0 && CivilWithoutLegLibraries.Count() > 0)
                    {
                        structureType.Deleted = (false);
                        return new Response<ConfigurationAttsViewModel>(true, null, null, $"This Record Can Not Be Deleted ", (int)Helpers.Constants.ApiReturnCode.fail);
                    }
                    else
                    {
                        structureType.Deleted = (true);
                    }
                    _unitOfWork.StructureTypeRepository.Update(structureType);
                    await _unitOfWork.SaveChangesAsync();
                }
                else if (ConfigrationTables.TLIsectionsLegType.ToString() == TableName)
                {
                    var sectionsLegType = _unitOfWork.SectionsLegTypeRepository.GetByID(Id);

                    var TLIcivilWithLegLibrary = _unitOfWork.CivilWithLegLibraryRepository
                        .GetWhere(x => x.sectionsLegTypeId == Id && !x.Deleted).ToList();

                    if (TLIcivilWithLegLibrary.Count() > 0)
                    {
                        sectionsLegType.Deleted = (false);
                        return new Response<ConfigurationAttsViewModel>(true, null, null, $"This Record Can Not Be Deleted ", (int)Helpers.Constants.ApiReturnCode.fail);
                    }
                    else
                    {
                        sectionsLegType.Deleted = (true);
                    }
                    _unitOfWork.SectionsLegTypeRepository.Update(sectionsLegType);
                    await _unitOfWork.SaveChangesAsync();
                }
                else if (ConfigrationTables.TLIsupportTypeImplemented.ToString() == TableName)
                {
                    var Civils = _unitOfWork.CivilSiteDateRepository
                        .GetIncludeWhere(x => !x.Dismantle && !x.allCivilInst.Draft && 
                            (x.allCivilInst.civilWithLegsId != null ? x.allCivilInst.civilWithLegs.SupportTypeImplementedId == Id :
                             x.allCivilInst.civilNonSteelId != null ? x.allCivilInst.civilNonSteel.supportTypeImplementedId == Id : false),
                                x => x.allCivilInst, x => x.allCivilInst.civilWithLegs, x => x.allCivilInst.civilNonSteel).ToList();

                    var Entity = _unitOfWork.SupportTypeImplementedRepository.GetByID(Id);

                    if (Civils.Count() > 0)
                    {
                        Entity.Deleted = (false);
                        return new Response<ConfigurationAttsViewModel>(true, null, null, $"This Record Can Not Be Deleted ", (int)Helpers.Constants.ApiReturnCode.fail);
                    }
                    else
                    {
                        Entity.Deleted = (true);
                    }
                    _unitOfWork.SupportTypeImplementedRepository.Update(Entity);
                    await _unitOfWork.SaveChangesAsync();
                }
                else if (ConfigrationTables.TLIbaseCivilWithLegsType.ToString() == TableName)
                {
                    var Civils = _unitOfWork.CivilSiteDateRepository.GetIncludeWhere(x => !x.Dismantle && !x.allCivilInst.Draft &&
                        (x.allCivilInst.civilWithLegsId != null ? x.allCivilInst.civilWithLegs.BaseCivilWithLegTypeId == Id : false),
                            x => x.allCivilInst, x => x.allCivilInst.civilWithLegs).ToList();

                    var Entity = _unitOfWork.BaseCivilWithLegsTypeRepository.GetByID(Id);

                    if (Civils.Count() > 0)
                    {
                        Entity.Deleted = (false);
                        return new Response<ConfigurationAttsViewModel>(true, null, null, $"This Record Can Not Be Deleted ", (int)Helpers.Constants.ApiReturnCode.fail);
                    }
                    else
                    {
                        Entity.Deleted = (true);
                    }
                    _unitOfWork.BaseCivilWithLegsTypeRepository.Update(Entity);
                    await _unitOfWork.SaveChangesAsync();
                }
                else if (ConfigrationTables.TLIbaseGeneratorType.ToString() == TableName)
                {
                    var Generators = _unitOfWork.OtherInSiteRepository.GetIncludeWhere(x => !x.Dismantle && !x.allOtherInventoryInst.Draft &&
                        (x.allOtherInventoryInst.generatorId != null ? 
                            x.allOtherInventoryInst.generator.BaseGeneratorTypeId == Id : false)).ToList();
                    
                    var Entity = _unitOfWork.BaseGeneratorTypeRepository.GetByID(Id);

                    if (Generators.Count() > 0)
                    {
                        Entity.Deleted = (false);
                        return new Response<ConfigurationAttsViewModel>(true, null, null, $"This Record Can Not Be Deleted ", (int)Helpers.Constants.ApiReturnCode.fail);
                    }
                    else
                    {
                        Entity.Deleted = (true);
                    }
                    _unitOfWork.BaseGeneratorTypeRepository.Update(Entity);
                    await _unitOfWork.SaveChangesAsync();
                }
                else if (ConfigrationTables.TLIinstallationCivilwithoutLegsType.ToString() == TableName)
                {
                    var CivilWithoutLegLibrary = _unitOfWork.CivilWithoutLegLibraryRepository
                        .GetWhere(x => x.InstallationCivilwithoutLegsTypeId == Id && !x.Deleted).ToList();

                    var Entity = _unitOfWork.InstallationCivilwithoutLegsTypeRepository.GetByID(Id);
                    if (CivilWithoutLegLibrary.Count() > 0)
                    {
                        Entity.Deleted = (false);
                        return new Response<ConfigurationAttsViewModel>(true, null, null, $"This Record Can Not Be Deleted ", (int)Helpers.Constants.ApiReturnCode.fail);
                    }
                    else
                    {
                        Entity.Deleted = (true);
                    }
                    _unitOfWork.InstallationCivilwithoutLegsTypeRepository.Update(Entity);
                    await _unitOfWork.SaveChangesAsync();
                }
                else if (ConfigrationTables.TLIboardType.ToString() == TableName)
                {
                    var MW_RFULibraries = _unitOfWork.MW_RFULibraryRepository
                        .GetWhere(x => x.boardTypeId == Id && !x.Deleted).ToList();

                    var Entity = _unitOfWork.BoardTypeRepository.GetByID(Id);

                    if (MW_RFULibraries.Count() > 0)
                    {
                        Entity.Deleted = (false);
                        return new Response<ConfigurationAttsViewModel>(true, null, null, $"This Record Can Not Be Deleted ", (int)Helpers.Constants.ApiReturnCode.fail);
                    }
                    else
                    {
                        Entity.Deleted = (true);
                    }
                    _unitOfWork.BoardTypeRepository.Update(Entity);
                    await _unitOfWork.SaveChangesAsync();
                }
                else if (ConfigrationTables.TLIguyLineType.ToString() == TableName)
                {
                    var Civils = _unitOfWork.CivilSiteDateRepository
                        .GetIncludeWhere(x => !x.Dismantle && 
                            (x.allCivilInst.civilWithLegsId != null ? x.allCivilInst.civilWithLegs.GuylineTypeId == Id : false)).ToList();
                    
                    var Entity = _unitOfWork.GuyLineTypeRepository.GetByID(Id);

                    if (Civils.Count() > 0)
                    {
                        Entity.Deleted = (false);
                        return new Response<ConfigurationAttsViewModel>(true, null, null, $"This Record Can Not Be Deleted ", (int)Helpers.Constants.ApiReturnCode.fail);
                    }
                    else
                    {
                        Entity.Deleted = (true);
                    }
                    _unitOfWork.GuyLineTypeRepository.Update(Entity);
                    await _unitOfWork.SaveChangesAsync();
                }
                else if (ConfigrationTables.TLIpolarityOnLocation.ToString() == TableName)
                {
                    var MW_Dishes = _unitOfWork.CivilLoadsRepository
                        .GetIncludeWhere(x => !x.Dismantle && 
                            (x.allLoadInstId != null ? (!x.allLoadInst.Draft && (x.allLoadInst.mwDishId != null ? 
                                x.allLoadInst.mwDish.PolarityOnLocationId == Id : false)) : false),
                                    x => x.allLoadInst, x => x.allLoadInst.mwDish).ToList();

                    var Entity = _unitOfWork.PolarityOnLocationRepository.GetByID(Id);

                    if (MW_Dishes.Count() > 0)
                    {
                        Entity.Deleted = (false);
                        return new Response<ConfigurationAttsViewModel>(true, null, null, $"This Record Can Not Be Deleted ", (int)Helpers.Constants.ApiReturnCode.fail);
                    }
                    else
                    {
                        Entity.Deleted = (true);
                    }
                    _unitOfWork.PolarityOnLocationRepository.Update(Entity);
                    await _unitOfWork.SaveChangesAsync();
                }
                else if (ConfigrationTables.TLIitemConnectTo.ToString() == TableName)
                {
                    var MW_Dishes = _unitOfWork.CivilLoadsRepository
                        .GetIncludeWhere(x => !x.Dismantle &&
                            (x.allLoadInstId != null ? (!x.allLoadInst.Draft && (x.allLoadInst.mwDishId != null ?
                                x.allLoadInst.mwDish.ItemConnectToId == Id : false)) : false),
                                    x => x.allLoadInst, x => x.allLoadInst.mwDish).ToList();

                    var Entity = _unitOfWork.ItemConnectToRepository.GetByID(Id);

                    if (MW_Dishes.Count() > 0)
                    {
                        Entity.Deleted = (false);
                        return new Response<ConfigurationAttsViewModel>(true, null, null, $"This Record Can Not Be Deleted ", (int)Helpers.Constants.ApiReturnCode.fail);
                    }
                    else
                    {
                        Entity.Deleted = (true);
                    }
                    _unitOfWork.ItemConnectToRepository.Update(Entity);
                    await _unitOfWork.SaveChangesAsync();
                }
                else if (ConfigrationTables.TLIrepeaterType.ToString() == TableName)
                {
                    var MW_Dishes = _unitOfWork.CivilLoadsRepository
                        .GetIncludeWhere(x => !x.Dismantle &&
                            (x.allLoadInstId != null ? (!x.allLoadInst.Draft && (x.allLoadInst.mwDishId != null ?
                                x.allLoadInst.mwDish.RepeaterTypeId == Id : false)) : false),
                                    x => x.allLoadInst, x => x.allLoadInst.mwDish).ToList();

                    var Entity = _unitOfWork.RepeaterTypeRepository.GetByID(Id);

                    if (MW_Dishes.Count() > 0)
                    {
                        Entity.Deleted = (false);
                        return new Response<ConfigurationAttsViewModel>(true, null, null, $"This Record Can Not Be Deleted ", (int)Helpers.Constants.ApiReturnCode.fail);
                    }
                    else
                    {
                        Entity.Deleted = (true);
                    }
                    _unitOfWork.RepeaterTypeRepository.Update(Entity);
                    await _unitOfWork.SaveChangesAsync();
                }
                else if (ConfigrationTables.TLIoduInstallationType.ToString() == TableName)
                {
                    var MW_ODUes = _unitOfWork.CivilLoadsRepository
                       .GetIncludeWhere(x => !x.Dismantle &&
                           (x.allLoadInstId != null ? (!x.allLoadInst.Draft && (x.allLoadInst.mwODUId != null ?
                               x.allLoadInst.mwODU.OduInstallationTypeId == Id : false)) : false),
                                   x => x.allLoadInst, x => x.allLoadInst.mwODU).ToList();

                    var Entity = _unitOfWork.OduInstallationTypeRepository.GetByID(Id);

                    if (MW_ODUes.Count() > 0)
                    {
                        Entity.Deleted = (false);
                        return new Response<ConfigurationAttsViewModel>(true, null, null, $"This Record Can Not Be Deleted ", (int)Helpers.Constants.ApiReturnCode.fail);
                    }
                    else
                    {
                        Entity.Deleted = (true);
                    }
                    _unitOfWork.OduInstallationTypeRepository.Update(Entity);
                    await _unitOfWork.SaveChangesAsync();
                }
                else if (ConfigrationTables.TLIsideArmInstallationPlace.ToString() == TableName)
                {
                    var SideArms = _unitOfWork.CivilLoadsRepository
                        .GetIncludeWhere(x => !x.Dismantle && (x.sideArmId != null ? 
                            (!x.sideArm.Draft && x.sideArm.sideArmInstallationPlaceId == Id) : false), x => x.sideArm).ToList();

                    var Entity = _unitOfWork.SideArmInstallationPlaceRepository.GetByID(Id);

                    if (SideArms.Count() > 0)
                    {
                        Entity.Deleted = (false);
                        return new Response<ConfigurationAttsViewModel>(true, null, null, $"This Record Can Not Be Deleted ", (int)Helpers.Constants.ApiReturnCode.fail);
                    }
                    else
                    {
                        Entity.Deleted = (true);
                    }
                    _unitOfWork.SideArmInstallationPlaceRepository.Update(Entity);
                    await _unitOfWork.SaveChangesAsync();
                }
                else if (ConfigrationTables.TLIenforcmentCategory.ToString() == TableName)
                {
                    var Civils = _unitOfWork.CivilSiteDateRepository
                        .GetIncludeWhere(x => !x.Dismantle && 
                            (x.allCivilInst.civilWithLegsId != null ? x.allCivilInst.civilWithLegs.enforcmentCategoryId == Id : false),
                                x => x.allCivilInst, x => x.allCivilInst.civilWithLegs).ToList();

                    var Entity = _unitOfWork.EnforcmentCategoryRepository.GetByID(Id);

                    if (Civils.Count() > 0)
                    {
                        Entity.Deleted = (false);
                        return new Response<ConfigurationAttsViewModel>(true, null, null, $"This Record Can Not Be Deleted ", (int)Helpers.Constants.ApiReturnCode.fail);
                    }
                    else
                    {
                        Entity.Deleted = (true);
                    }
                    _unitOfWork.EnforcmentCategoryRepository.Update(Entity);
                    await _unitOfWork.SaveChangesAsync();
                }
                else if (ConfigrationTables.TLIpowerType.ToString() == TableName)
                {
                    var Powers = _unitOfWork.CivilLoadsRepository
                        .GetIncludeWhere(x => !x.Dismantle && 
                            (x.allLoadInstId != null ? 
                                (x.allLoadInst.powerId != null ? x.allLoadInst.power.powerTypeId == Id : false) : false), 
                            x => x.allLoadInst, x => x.allLoadInst.power).ToList();

                    var Entity = _unitOfWork.PowerTypeRepository.GetByID(Id);

                    if (Powers.Count() > 0)
                    {
                        Entity.Delete = (false);
                        return new Response<ConfigurationAttsViewModel>(true, null, null, $"This Record Can Not Be Deleted ", (int)Helpers.Constants.ApiReturnCode.fail);
                    }
                    else
                    {
                        Entity.Delete = (true);
                    }
                    _unitOfWork.PowerTypeRepository.Update(Entity);
                    await _unitOfWork.SaveChangesAsync();
                }

                //----------------------------------------------------------
                else if (ConfigrationTables.TLIsubType.ToString() == TableName)
                {
                    var Civils = _unitOfWork.CivilSiteDateRepository
                        .GetIncludeWhere(x => !x.Dismantle && 
                            (x.allCivilInst.civilWithoutLegId != null ? x.allCivilInst.civilWithoutLeg.subTypeId == Id : false),
                                x => x.allCivilInst, x => x.allCivilInst.civilWithoutLeg).ToList();

                    var Entity = _unitOfWork.SubTypeRepository.GetByID(Id);

                    if (Civils.Count() > 0)
                    {
                        Entity.Delete = (false);
                        return new Response<ConfigurationAttsViewModel>(true, null, null, $"This Record Can Not Be Deleted ", (int)Helpers.Constants.ApiReturnCode.fail);
                    }
                    else
                    {
                        Entity.Delete = (true);
                    }
                    _unitOfWork.SubTypeRepository.Update(Entity);
                    await _unitOfWork.SaveChangesAsync();
                }

                else if (ConfigrationTables.TLIasType.ToString() == TableName)
                {
                    var MW_DishesLibraries = _unitOfWork.MW_DishLibraryRepository
                        .GetWhere(x => !x.Deleted && x.asTypeId == Id).ToList();

                    var Entity = _unitOfWork.AsTypeRepository.GetByID(Id);

                    if (MW_DishesLibraries.Count() > 0)
                    {
                        Entity.Delete = (false);
                        return new Response<ConfigurationAttsViewModel>(true, null, null, $"This Record Can Not Be Deleted ", (int)Helpers.Constants.ApiReturnCode.fail);
                    }
                    else
                    {
                        Entity.Delete = (true);
                    }
                    _unitOfWork.AsTypeRepository.Update(Entity);
                    await _unitOfWork.SaveChangesAsync();
                }

                else if (ConfigrationTables.TLIpolarityType.ToString() == TableName)
                {
                    var MW_DishesLibraries = _unitOfWork.MW_DishLibraryRepository
                        .GetWhere(x => !x.Deleted && x.polarityTypeId == Id).ToList();

                    var Entity = _unitOfWork.PolarityTypeRepository.GetByID(Id);

                    if (MW_DishesLibraries.Count() > 0)
                    {
                        Entity.Delete = (false);
                        return new Response<ConfigurationAttsViewModel>(true, null, null, $"This Record Can Not Be Deleted ", (int)Helpers.Constants.ApiReturnCode.fail);
                    }
                    else
                    {
                        Entity.Delete = (true);
                    }
                    _unitOfWork.PolarityTypeRepository.Update(Entity);
                    await _unitOfWork.SaveChangesAsync();
                }

                else if (ConfigrationTables.TLIparity.ToString() == TableName)
                {
                    var MW_DishesLibraries = _unitOfWork.MW_ODULibraryRepository
                        .GetWhere(x => !x.Deleted && x.parityId == Id).ToList();

                    var Entity = _unitOfWork.ParityRepository.GetByID(Id);

                    if (Entity == null)
                    {
                        Entity.Delete = (false);
                        return new Response<ConfigurationAttsViewModel>(true, null, null, $"This Record Can Not Be Deleted ", (int)Helpers.Constants.ApiReturnCode.fail);
                    }
                    else
                    {
                        Entity.Delete = (true);
                    }
                    _unitOfWork.ParityRepository.Update(Entity);
                    await _unitOfWork.SaveChangesAsync();
                }

                else if (ConfigrationTables.TLIcabinetPowerType.ToString() == TableName)
                {
                    var CabinetPowerLibraries = _unitOfWork.CabinetPowerLibraryRepository
                        .GetWhere(x => !x.Deleted && x.CabinetPowerTypeId == Id).ToList();

                    var Entity = _unitOfWork.CabinetPowerTypeRepository.GetByID(Id);

                    if (CabinetPowerLibraries.Count() > 0)
                    {
                        Entity.Delete = (false);
                        return new Response<ConfigurationAttsViewModel>(true, null, null, $"This Record Can Not Be Deleted ", (int)Helpers.Constants.ApiReturnCode.fail);
                    }
                    else
                    {
                        Entity.Delete = (true);
                    }
                    _unitOfWork.CabinetPowerTypeRepository.Update(Entity);
                    await _unitOfWork.SaveChangesAsync();
                }

                else if (ConfigrationTables.TLIcapacity.ToString() == TableName)
                {
                    var SolarLibraries = _unitOfWork.SolarLibraryRepository
                        .GetWhere(x => !x.Deleted && x.CapacityId == Id).ToList();

                    var GeneratorLibraries = _unitOfWork.GeneratorLibraryRepository
                        .GetWhere(x => !x.Deleted && x.CapacityId == Id).ToList();

                    var Entity = _unitOfWork.CapacityRepository.GetByID(Id);

                    if (SolarLibraries.Count() > 0 || GeneratorLibraries.Count() > 0)
                    {
                        Entity.Delete = (false);
                        return new Response<ConfigurationAttsViewModel>(true, null, null, $"This Record Can Not Be Deleted ", (int)Helpers.Constants.ApiReturnCode.fail);
                    }
                    else
                    {
                        Entity.Delete = (true);
                    }
                    _unitOfWork.CapacityRepository.Update(Entity);
                    await _unitOfWork.SaveChangesAsync();
                }
                else if (ConfigrationTables.TLIcivilNonSteelType.ToString() == TableName)
                {
                    var CivilNonSteelLibraries = _unitOfWork.CivilNonSteelLibraryRepository
                        .GetWhere(x => !x.Deleted && x.civilNonSteelTypeId == Id).ToList();

                    var Entity = _unitOfWork.CivilNonSteelTypeRepository.GetByID(Id);

                    if (CivilNonSteelLibraries.Count() > 0)
                    {
                        Entity.Deleted = (false);
                        return new Response<ConfigurationAttsViewModel>(true, null, null, $"This Record Can Not Be Deleted ", (int)Helpers.Constants.ApiReturnCode.fail);
                    }
                    else
                    {
                        Entity.Deleted = (true);
                    }
                    _unitOfWork.CivilNonSteelTypeRepository.Update(Entity);
                    await _unitOfWork.SaveChangesAsync();
                }
                else if (ConfigrationTables.TLIowner.ToString() == TableName)
                {
                    var Civils = _unitOfWork.CivilSiteDateRepository
                        .GetIncludeWhere(x => !x.Dismantle && !x.allCivilInst.Draft && (x.allCivilInst.civilWithLegs != null ? x.allCivilInst.civilWithLegs.OwnerId == Id :
                            x.allCivilInst.civilWithoutLegId != null ? x.allCivilInst.civilWithoutLeg.OwnerId == Id :
                            x.allCivilInst.civilNonSteel.ownerId == Id), x => x.allCivilInst, x => x.allCivilInst.civilWithLegs,
                            x => x.allCivilInst.civilWithoutLeg, x => x.allCivilInst.civilNonSteel).ToList();

                    var Loads = _unitOfWork.CivilLoadsRepository.GetIncludeWhere(x => !x.Dismantle &&
                        x.allLoadInstId != null ?
                            ((!x.allLoadInst.Draft) &&
                            (x.allLoadInst.mwBUId != null ? x.allLoadInst.mwBU.OwnerId == Id :
                             x.allLoadInst.mwDishId != null ? x.allLoadInst.mwDish.ownerId == Id :
                             x.allLoadInst.mwODUId != null ? x.allLoadInst.mwODU.OwnerId == Id :
                             x.allLoadInst.mwRFUId != null ? x.allLoadInst.mwRFU.OwnerId == Id :
                             x.allLoadInst.radioAntennaId != null ? x.allLoadInst.radioAntenna.ownerId == Id :
                             x.allLoadInst.radioRRUId != null ? x.allLoadInst.radioRRU.ownerId == Id :
                             x.allLoadInst.radioOtherId != null ? x.allLoadInst.radioOther.ownerId == Id :
                             x.allLoadInst.powerId != null ? x.allLoadInst.power.ownerId == Id : false)) : 
                        x.sideArmId != null ? 
                             (x.sideArm.ownerId == Id && !x.sideArm.Draft) : false, 
                        x => x.allLoadInst, x => x.allLoadInst.mwBU, x => x.allLoadInst.mwDish, x => x.allLoadInst.mwODU,
                        x => x.allLoadInst.mwRFU, x => x.allLoadInst.radioAntenna, x => x.allLoadInst.radioOther,
                        x => x.allLoadInst.radioRRU, x => x.allLoadInst.power, x => x.sideArm).ToList();

                    var Entity = _unitOfWork.OwnerRepository.GetByID(Id);

                    if (Entity == null || Civils.Count() > 0 || Loads.Count() > 0)
                    {
                        Entity.Deleted = (false);
                        return new Response<ConfigurationAttsViewModel>(true, null, null, $"This Record Can Not Be Deleted ", (int)Helpers.Constants.ApiReturnCode.fail);
                    }
                    else
                    {
                        Entity.Deleted = (true);
                    }
                    _unitOfWork.OwnerRepository.Update(Entity);
                    await _unitOfWork.SaveChangesAsync();
                }
                else if (ConfigrationTables.TLIlocationType.ToString() == TableName)
                {
                    var Civils = _unitOfWork.CivilSiteDateRepository
                        .GetIncludeWhere(x => !x.Dismantle && x.allCivilInst.Draft && (x.allCivilInst.civilWithLegsId != null ?
                            x.allCivilInst.civilWithLegs.locationTypeId.Value == Id :
                            x.allCivilInst.civilNonSteelId != null ? 
                                x.allCivilInst.civilNonSteel.locationTypeId.Value == Id : false), 
                        x => x.allCivilInst, x => x.allCivilInst.civilWithLegs, x => x.allCivilInst.civilNonSteel).ToList();

                    var Entity = _unitOfWork.LocationTypeRepository.GetByID(Id);
                    if (Entity == null || Civils.Count() > 0)
                    {
                        Entity.Deleted = (false);
                        return new Response<ConfigurationAttsViewModel>(true, null, null, $"This Record Can Not Be Deleted ", (int)Helpers.Constants.ApiReturnCode.fail);
                    }
                    else
                    {
                        Entity.Deleted = (true);
                    }
                    _unitOfWork.LocationTypeRepository.Update(Entity);
                    await _unitOfWork.SaveChangesAsync();
                }
                else if (ConfigrationTables.TLIbaseType.ToString() == TableName)
                {
                    var Civils = _unitOfWork.CivilSiteDateRepository
                        .GetIncludeWhere(x => !x.Dismantle && (x.allCivilInst.civilWithLegsId != null ?
                            (x.allCivilInst.civilWithLegs.baseTypeId == Id && !x.allCivilInst.Draft) : false),
                                x => x.allCivilInst, x => x.allCivilInst.civilWithLegs).ToList();

                    var Entity = _unitOfWork.LocationTypeRepository.GetByID(Id);
                    if (Entity == null || Civils.Count() > 0)
                    {
                        Entity.Deleted = (false);
                        return new Response<ConfigurationAttsViewModel>(true, null, null, $"This Record Can Not Be Deleted ", (int)Helpers.Constants.ApiReturnCode.fail);
                    }
                    else
                    {
                        Entity.Deleted = (true);
                    }
                    _unitOfWork.LocationTypeRepository.Update(Entity);
                    await _unitOfWork.SaveChangesAsync();
                }
                else if (ConfigrationTables.TLIbaseBU.ToString() == TableName)
                {
                    var Loads = _unitOfWork.CivilLoadsRepository
                        .GetIncludeWhere(x => !x.Dismantle && !x.allCivilInst.Draft && 
                            (x.allLoadInstId != null ? (x.allLoadInst.mwBUId != null ? x.allLoadInst.mwBU.BaseBUId == Id : false) : false),
                                x => x.allCivilInst, x => x.allLoadInst, x => x.allLoadInst.mwBU).ToList();

                    var Entity = _unitOfWork.BaseBURepository.GetByID(Id);
                    if (Entity == null || Loads.Count() > 0)
                    {
                        Entity.Deleted = (false);
                        return new Response<ConfigurationAttsViewModel>(true, null, null, $"This Record Can Not Be Deleted ", (int)Helpers.Constants.ApiReturnCode.fail);
                    }
                    else
                    {
                        Entity.Deleted = (true);
                    }
                    _unitOfWork.BaseBURepository.Update(Entity);
                    await _unitOfWork.SaveChangesAsync();
                }
                else if (ConfigrationTables.TLIrenewableCabinetType.ToString() == TableName)
                {
                    var Cabinets = _unitOfWork.OtherInSiteRepository
                        .GetIncludeWhere(x => !x.Dismantle && !x.allOtherInventoryInst.Draft && 
                            (x.allOtherInventoryInst.cabinetId != null ? 
                                x.allOtherInventoryInst.cabinet.RenewableCabinetTypeId == Id : false),
                                    x => x.allOtherInventoryInst, x => x.allOtherInventoryInst.cabinet).ToList();

                    var Entity = _unitOfWork.RenewableCabinetTypeRepository.GetByID(Id);
                    if (Entity == null || Cabinets.Count() > 0)
                    {
                        Entity.Deleted = (false);
                        return new Response<ConfigurationAttsViewModel>(true, null, null, $"This Record Can Not Be Deleted ", (int)Helpers.Constants.ApiReturnCode.fail);
                    }
                    else
                    {
                        Entity.Deleted = (true);
                    }
                    _unitOfWork.RenewableCabinetTypeRepository.Update(Entity);
                    await _unitOfWork.SaveChangesAsync();
                }
                else if (ConfigrationTables.TLIsideArmType.ToString() == TableName)
                {
                    var Loads = _unitOfWork.CivilLoadsRepository
                        .GetIncludeWhere(x => !x.Dismantle && 
                            (x.sideArmId != null ? (!x.sideArm.Draft && x.sideArm.sideArmTypeId == Id) : false),
                                x => x.sideArm).ToList();

                    var Entity = _unitOfWork.LocationTypeRepository.GetByID(Id);
                    if (Entity == null || Loads.Count() > 0)
                    {
                        Entity.Deleted = (false);
                        return new Response<ConfigurationAttsViewModel>(true, null, null, $"This Record Can Not Be Deleted ", (int)Helpers.Constants.ApiReturnCode.fail);
                    }
                    else
                    {
                        Entity.Deleted = (true);
                    }
                    _unitOfWork.LocationTypeRepository.Update(Entity);
                    await _unitOfWork.SaveChangesAsync();
                }
                else if (ConfigrationTables.TLIinstallationPlace.ToString() == TableName)
                {
                    var Loads = _unitOfWork.CivilLoadsRepository
                        .GetIncludeWhere(x => !x.Dismantle &&
                            (x.allLoadInstId != null ? (!x.allLoadInst.Draft && 
                                (x.allLoadInst.mwBUId != null ? x.allLoadInst.mwBU.InstallationPlaceId == Id : 
                                 x.allLoadInst.mwDishId != null ? x.allLoadInst.mwDish.InstallationPlaceId == Id :
                                 x.allLoadInst.mwOtherId != null ? x.allLoadInst.mwOther.InstallationPlaceId == Id :
                                 x.allLoadInst.radioAntennaId != null ? x.allLoadInst.radioAntenna.installationPlaceId == Id :
                                 x.allLoadInst.radioRRUId != null ? x.allLoadInst.radioRRU.installationPlaceId == Id :
                                 x.allLoadInst.radioOtherId != null ? x.allLoadInst.radioOther.installationPlaceId == Id :
                                 x.allLoadInst.powerId != null ? x.allLoadInst.power.installationPlaceId == Id :
                                 x.allLoadInst.loadOtherId != null ? x.allLoadInst.loadOther.InstallationPlaceId == Id : false)) : false),
                            x => x.allLoadInst).ToList();

                    var Entity = _unitOfWork.LocationTypeRepository.GetByID(Id);
                    if (Entity == null || Loads.Count() > 0)
                    {
                        Entity.Deleted = (false);
                        return new Response<ConfigurationAttsViewModel>(true, null, null, $"This Record Can Not Be Deleted ", (int)Helpers.Constants.ApiReturnCode.fail);
                    }
                    else
                    {
                        Entity.Deleted = (true);
                    }
                    _unitOfWork.LocationTypeRepository.Update(Entity);
                    await _unitOfWork.SaveChangesAsync();
                }
                //---------------------------------------------------------
                else
                {
                    //add validation after adding relation between dy
                }
                return new Response<ConfigurationAttsViewModel>();
            }
            catch(Exception err)
            {
                return new Response<ConfigurationAttsViewModel>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
        }

        public async Task<Response<List<TableAffected>>> Disable(string TableName, int Id, bool isForced)
        {
            try
            {
                if (isForced)
                {
                    if (ConfigrationTables.TLIdiversityType.ToString() == TableName)
                    {
                        var diversityType = _unitOfWork.DiversityTypeRepository.GetByID(Id);
                        diversityType.Disable = !(diversityType.Disable);

                        _unitOfWork.MW_BULibraryRepository
                            .GetWhere(x => x.diversityTypeId == Id && !x.Deleted).ToList()
                            .ForEach(ListItem =>
                            {
                                ListItem.diversityTypeId = null;
                            });

                        _unitOfWork.MW_RFULibraryRepository
                            .GetWhere(x => x.diversityTypeId == Id && !x.Deleted).ToList()
                            .ForEach(ListItem =>
                            {
                                ListItem.diversityTypeId = null;
                            });

                        await _unitOfWork.DiversityTypeRepository.UpdateItem(diversityType);
                        await _unitOfWork.SaveChangesAsync();
                    }
                    else if (ConfigrationTables.TLItelecomType.ToString() == TableName)
                    {
                        var TelecomType = _unitOfWork.TelecomTypeRepository.GetByID(Id);
                        TelecomType.Disable = !(TelecomType.Disable);

                        _unitOfWork.CabinetTelecomLibraryRepository
                            .GetIncludeWhere(x => x.TelecomTypeId == Id && !x.Deleted).ToList()
                            .ForEach(ListItem =>
                            {
                                ListItem.TelecomTypeId = null;
                            });

                        await _unitOfWork.TelecomTypeRepository.UpdateItem(TelecomType);
                        await _unitOfWork.SaveChangesAsync();
                    }
                    else if (ConfigrationTables.TLIsupportTypeDesigned.ToString() == TableName)
                    {
                        var supportTypeDesigned = _unitOfWork.SupportTypeDesignedRepository.GetByID(Id);
                        supportTypeDesigned.Disable = !(supportTypeDesigned.Disable);

                        _unitOfWork.CivilWithLegLibraryRepository
                            .GetWhere(x => x.supportTypeDesignedId == Id && !x.Deleted).ToList()
                            .ForEach(ListItem =>
                            {
                                ListItem.supportTypeDesignedId = 0;
                            });

                        await _unitOfWork.SupportTypeDesignedRepository.UpdateItem(supportTypeDesigned);
                        await _unitOfWork.SaveChangesAsync();
                    }
                    else if (ConfigrationTables.TLIsupportTypeImplemented.ToString() == TableName)
                    {
                        var supportTypeImplemented = _unitOfWork.SupportTypeImplementedRepository.GetByID(Id);
                        supportTypeImplemented.Disable = !(supportTypeImplemented.Disable);

                        _unitOfWork.CivilSiteDateRepository
                            .GetIncludeWhere(x => !x.Dismantle && !x.allCivilInst.Draft && (x.allCivilInst.civilWithLegsId != null ? 
                                x.allCivilInst.civilWithLegs.SupportTypeImplementedId == Id : false), 
                                    x => x.allCivilInst, x => x.allCivilInst.civilWithLegs).Select(x => x.allCivilInst.civilWithLegs)
                            .ToList().ForEach(ListItem =>
                            {
                                ListItem.SupportTypeImplementedId = null;
                            });

                        _unitOfWork.CivilSiteDateRepository
                            .GetIncludeWhere(x => !x.Dismantle && !x.allCivilInst.Draft && (x.allCivilInst.civilNonSteelId != null ? 
                                x.allCivilInst.civilNonSteel.supportTypeImplementedId == Id : false), 
                                    x => x.allCivilInst, x => x.allCivilInst.civilNonSteel).Select(x => x.allCivilInst.civilNonSteel)
                            .ToList().ForEach(ListItem =>
                            {
                                ListItem.supportTypeImplementedId = null;
                            });

                        await _unitOfWork.SupportTypeImplementedRepository.UpdateItem(supportTypeImplemented);
                        await _unitOfWork.SaveChangesAsync();
                    }
                    else if (ConfigrationTables.TLIstructureType.ToString() == TableName)
                    {
                        var structureType = _unitOfWork.StructureTypeRepository.GetByID(Id);
                        structureType.Disable = !(structureType.Disable);

                        _unitOfWork.CivilWithLegLibraryRepository
                            .GetWhere(x => x.structureTypeId == Id && !x.Deleted).ToList()
                            .ForEach(ListItem =>
                            {
                                ListItem.structureTypeId = null;
                            });

                        _unitOfWork.CivilWithoutLegLibraryRepository
                            .GetWhere(x => x.structureTypeId == Id && !x.Deleted).ToList()
                            .ForEach(ListItem =>
                            {
                                ListItem.structureTypeId = 0;
                            });

                        await _unitOfWork.StructureTypeRepository.UpdateItem(structureType);
                        await _unitOfWork.SaveChangesAsync();
                    }
                    else if (ConfigrationTables.TLIsectionsLegType.ToString() == TableName)
                    {
                        var sectionsLegType = _unitOfWork.SectionsLegTypeRepository.GetByID(Id);
                        sectionsLegType.Disable = !(sectionsLegType.Disable);

                        _unitOfWork.CivilWithLegLibraryRepository
                            .GetWhere(x => x.sectionsLegTypeId == Id && !x.Deleted).ToList()
                            .ForEach(ListItem =>
                            {
                                ListItem.sectionsLegTypeId = 0;
                            });

                        await _unitOfWork.SectionsLegTypeRepository.UpdateItem(sectionsLegType);
                        await _unitOfWork.SaveChangesAsync();
                    }
                    else if (ConfigrationTables.TLIlogisticalType.ToString() == TableName)
                    {
                        var logisticalType = _unitOfWork.logisticalTypeRepository.GetByID(Id);
                        logisticalType.Disable = !(logisticalType.Disable);
                        await _unitOfWork.logisticalTypeRepository.UpdateItem(logisticalType);
                        await _unitOfWork.SaveChangesAsync();
                    }
                    else if (ConfigrationTables.TLIbaseCivilWithLegsType.ToString() == TableName)
                    {
                        var baseCivilWithLegs = _unitOfWork.BaseCivilWithLegsTypeRepository.GetByID(Id);
                        baseCivilWithLegs.Disable = !(baseCivilWithLegs.Disable);

                        _unitOfWork.CivilSiteDateRepository
                            .GetIncludeWhere(x => !x.Dismantle && !x.allCivilInst.Draft && (x.allCivilInst.civilWithLegsId != null ? 
                                x.allCivilInst.civilWithLegs.BaseCivilWithLegTypeId == Id : false), 
                                    x => x.allCivilInst, x => x.allCivilInst.civilWithLegs).Select(x => x.allCivilInst.civilWithLegs)
                            .ToList().ForEach(ListItem =>
                            {
                                ListItem.BaseCivilWithLegTypeId = 0;
                            });

                        await _unitOfWork.BaseCivilWithLegsTypeRepository.UpdateItem(baseCivilWithLegs);
                        await _unitOfWork.SaveChangesAsync();
                    }
                    else if (ConfigrationTables.TLIbaseGeneratorType.ToString() == TableName)
                    {
                        var baseGeneratorType = _unitOfWork.BaseGeneratorTypeRepository.GetByID(Id);
                        baseGeneratorType.Disable = !(baseGeneratorType.Disable);

                        _unitOfWork.OtherInSiteRepository
                            .GetWhere(x => !x.Dismantle && !x.allOtherInventoryInst.Draft && (x.allOtherInventoryInst.generatorId != null ?    
                                x.allOtherInventoryInst.generator.BaseGeneratorTypeId == Id : false)).Select(x => x.allOtherInventoryInst.generator)
                            .ToList().ForEach(ListItem =>
                            {
                                ListItem.BaseGeneratorTypeId = null;
                            });

                        await _unitOfWork.BaseGeneratorTypeRepository.UpdateItem(baseGeneratorType);
                        await _unitOfWork.SaveChangesAsync();
                    }
                    else if (ConfigrationTables.TLIinstallationCivilwithoutLegsType.ToString() == TableName)
                    {
                        var installationCivilwithoutLegsType = _unitOfWork.InstallationCivilwithoutLegsTypeRepository.GetByID(Id);
                        installationCivilwithoutLegsType.Disable = !(installationCivilwithoutLegsType.Disable);

                        _unitOfWork.CivilWithoutLegLibraryRepository
                            .GetWhere(x => x.InstallationCivilwithoutLegsTypeId == Id && !x.Deleted).ToList()
                            .ForEach(ListItem =>
                            {
                                ListItem.InstallationCivilwithoutLegsTypeId = null;
                            });

                        await _unitOfWork.InstallationCivilwithoutLegsTypeRepository.UpdateItem(installationCivilwithoutLegsType);
                        await _unitOfWork.SaveChangesAsync();
                    }
                    else if (ConfigrationTables.TLIboardType.ToString() == TableName)
                    {
                        var boardType = _unitOfWork.BoardTypeRepository.GetByID(Id);
                        boardType.Disable = !(boardType.Disable);

                        _unitOfWork.MW_RFULibraryRepository
                            .GetWhere(x => x.boardTypeId == Id && !x.Deleted).ToList()
                            .ForEach(ListItem =>
                            {
                                ListItem.boardTypeId = null;
                            });

                        await _unitOfWork.BoardTypeRepository.UpdateItem(boardType);
                        await _unitOfWork.SaveChangesAsync();
                    }
                    else if (ConfigrationTables.TLIguyLineType.ToString() == TableName)
                    {
                        var guyLineType = _unitOfWork.GuyLineTypeRepository.GetByID(Id);
                        guyLineType.Disable = !(guyLineType.Disable);

                        _unitOfWork.CivilSiteDateRepository
                            .GetIncludeWhere(x => !x.Dismantle && !x.allCivilInst.Draft && (x.allCivilInst.civilWithLegsId != null ?
                                x.allCivilInst.civilWithLegs.GuylineTypeId == Id : false),
                                    x => x.allCivilInst, x => x.allCivilInst.civilWithLegs).Select(x => x.allCivilInst.civilWithLegs)
                            .ToList().ForEach(ListItem =>
                            {
                                ListItem.GuylineTypeId = null;
                            });

                        await _unitOfWork.GuyLineTypeRepository.UpdateItem(guyLineType);
                        await _unitOfWork.SaveChangesAsync();
                    }
                    else if (ConfigrationTables.TLIpolarityOnLocation.ToString() == TableName)
                    {
                        var polarityOnLocation = _unitOfWork.PolarityOnLocationRepository.GetByID(Id);
                        polarityOnLocation.Disable = !(polarityOnLocation.Disable);

                        _unitOfWork.CivilLoadsRepository
                            .GetIncludeWhere(x => !x.Dismantle && !x.allLoadInst.Draft && (x.allLoadInst.mwDishId != null ? 
                                x.allLoadInst.mwDish.PolarityOnLocationId == Id : false), 
                                    x => x.allLoadInst, x => x.allLoadInst.mwDish).Select(x => x.allLoadInst.mwDish)
                            .ToList().ForEach(ListItem =>
                            {
                                ListItem.PolarityOnLocationId = null;
                            });

                        await _unitOfWork.PolarityOnLocationRepository.UpdateItem(polarityOnLocation);
                        await _unitOfWork.SaveChangesAsync();
                    }
                    else if (ConfigrationTables.TLIitemConnectTo.ToString() == TableName)
                    {
                        var itemConnectTo = _unitOfWork.ItemConnectToRepository.GetByID(Id);
                        itemConnectTo.Disable = !(itemConnectTo.Disable);

                        _unitOfWork.CivilLoadsRepository
                            .GetIncludeWhere(x => !x.Dismantle && !x.allLoadInst.Draft && (x.allLoadInst.mwDishId != null ?
                                x.allLoadInst.mwDish.ItemConnectToId == Id : false),
                                    x => x.allLoadInst, x => x.allLoadInst.mwDish).Select(x => x.allLoadInst.mwDish)
                            .ToList().ForEach(ListItem =>
                            {
                                ListItem.ItemConnectToId = null;
                            });

                        await _unitOfWork.ItemConnectToRepository.UpdateItem(itemConnectTo);
                        await _unitOfWork.SaveChangesAsync();
                    }
                    else if (ConfigrationTables.TLIrepeaterType.ToString() == TableName)
                    {
                        var repeaterType = _unitOfWork.RepeaterTypeRepository.GetByID(Id);
                        repeaterType.Disable = !(repeaterType.Disable);

                        _unitOfWork.CivilLoadsRepository
                            .GetIncludeWhere(x => !x.Dismantle && !x.allLoadInst.Draft && (x.allLoadInst.mwDishId != null ?
                                x.allLoadInst.mwDish.RepeaterTypeId == Id : false),
                                    x => x.allLoadInst, x => x.allLoadInst.mwDish).Select(x => x.allLoadInst.mwDish)
                            .ToList().ForEach(ListItem =>
                            {
                                ListItem.RepeaterTypeId = null;
                            });

                        await _unitOfWork.RepeaterTypeRepository.UpdateItem(repeaterType);
                        await _unitOfWork.SaveChangesAsync();
                    }
                    else if (ConfigrationTables.TLIoduInstallationType.ToString() == TableName)
                    {
                        var oduInstallationType = _unitOfWork.OduInstallationTypeRepository.GetByID(Id);
                        oduInstallationType.Disable = !(oduInstallationType.Disable);
                        await _unitOfWork.OduInstallationTypeRepository.UpdateItem(oduInstallationType);
                        await _unitOfWork.SaveChangesAsync();
                    }
                    else if (ConfigrationTables.TLIsideArmInstallationPlace.ToString() == TableName)
                    {
                        var sideArmInstallationPlace = _unitOfWork.SideArmInstallationPlaceRepository.GetByID(Id);
                        sideArmInstallationPlace.Disable = !(sideArmInstallationPlace.Disable);
                        await _unitOfWork.SideArmInstallationPlaceRepository.UpdateItem(sideArmInstallationPlace);
                        await _unitOfWork.SaveChangesAsync();
                    }
                    else if (ConfigrationTables.TLIdataType.ToString() == TableName)
                    {
                        var dataType = _unitOfWork.DataTypeRepository.GetByID(Id);
                        dataType.Disable = !(dataType.Disable);
                        await _unitOfWork.DataTypeRepository.UpdateItem(dataType);
                        await _unitOfWork.SaveChangesAsync();
                    }
                    else if (ConfigrationTables.TLIoperation.ToString() == TableName)
                    {
                        var operation = _unitOfWork.OperationRepository.GetByID(Id);
                        operation.Disable = !(operation.Disable);
                        await _unitOfWork.OperationRepository.UpdateItem(operation);
                        await _unitOfWork.SaveChangesAsync();
                    }
                    else if (ConfigrationTables.TLIlogicalOperation.ToString() == TableName)
                    {
                        var logicalOperation = _unitOfWork.LogicalOperationRepository.GetByID(Id);
                        logicalOperation.Disable = !(logicalOperation.Disable);
                        await _unitOfWork.LogicalOperationRepository.UpdateItem(logicalOperation);
                        await _unitOfWork.SaveChangesAsync();
                    }
                    else if (ConfigrationTables.TLIenforcmentCategory.ToString() == TableName)
                    {
                        var enforcmentCategory = _unitOfWork.EnforcmentCategoryRepository.GetByID(Id);
                        enforcmentCategory.Disable = !(enforcmentCategory.Disable);

                        _unitOfWork.CivilSiteDateRepository
                            .GetIncludeWhere(x => !x.Dismantle && !x.allCivilInst.Draft && (x.allCivilInst.civilWithLegsId != null ?
                                x.allCivilInst.civilWithLegs.enforcmentCategoryId == Id : false),
                                    x => x.allCivilInst, x => x.allCivilInst.civilWithLegs).Select(x => x.allCivilInst.civilWithLegs)
                            .ToList().ForEach(ListItem =>
                            {
                                ListItem.enforcmentCategoryId = null;
                            });

                        await _unitOfWork.EnforcmentCategoryRepository.UpdateItem(enforcmentCategory);
                        await _unitOfWork.SaveChangesAsync();
                    }
                    else if (ConfigrationTables.TLIpowerType.ToString() == TableName)
                    {
                        var powerType = _unitOfWork.PowerTypeRepository.GetByID(Id);
                        powerType.Disable = !(powerType.Disable);

                        _unitOfWork.CivilLoadsRepository
                            .GetIncludeWhere(x => !x.Dismantle && !x.allLoadInst.Draft && (x.allLoadInst.powerId != null ?
                                x.allLoadInst.power.powerTypeId == Id : false),
                                    x => x.allLoadInst, x => x.allLoadInst.power).Select(x => x.allLoadInst.power)
                            .ToList().ForEach(ListItem =>
                            {
                                ListItem.powerTypeId = null;
                            });

                        await _unitOfWork.PowerTypeRepository.UpdateItem(powerType);
                        await _unitOfWork.SaveChangesAsync();
                    }
                    else if (ConfigrationTables.TLIcivilNonSteelType.ToString() == TableName)
                    {
                        var civilNonSteelType = _unitOfWork.CivilNonSteelTypeRepository.GetByID(Id);
                        civilNonSteelType.Disable = !(civilNonSteelType.Disable);

                        _unitOfWork.CivilNonSteelLibraryRepository
                            .GetWhere(x => x.civilNonSteelTypeId == Id && !x.Deleted).ToList()
                            .ForEach(ListItem =>
                            {
                                ListItem.civilNonSteelTypeId = 0;
                            });

                        await _unitOfWork.CivilNonSteelTypeRepository.UpdateItem(civilNonSteelType);
                        await _unitOfWork.SaveChangesAsync();
                    }
                    else if (ConfigrationTables.TLIsubType.ToString() == TableName)
                    {
                        var subType = _unitOfWork.SubTypeRepository.GetByID(Id);
                        subType.Disable = !(subType.Disable);

                        _unitOfWork.CivilSiteDateRepository
                            .GetIncludeWhere(x => !x.Dismantle && !x.allCivilInst.Draft && (x.allCivilInst.civilWithoutLegId != null ?
                                x.allCivilInst.civilWithoutLeg.subTypeId == Id : false),
                                    x => x.allCivilInst, x => x.allCivilInst.civilWithoutLeg).Select(x => x.allCivilInst.civilWithoutLeg)
                            .ToList().ForEach(ListItem =>
                            {
                                ListItem.subTypeId = null;
                            });

                        await _unitOfWork.SubTypeRepository.UpdateItem(subType);
                        await _unitOfWork.SaveChangesAsync();
                    }
                    else if (ConfigrationTables.TLIasType.ToString() == TableName)
                    {
                        var asType = _unitOfWork.AsTypeRepository.GetByID(Id);
                        asType.Disable = !(asType.Disable);

                        _unitOfWork.MW_DishLibraryRepository
                            .GetWhere(x => x.asTypeId == Id && !x.Deleted).ToList()
                            .ForEach(ListItem =>
                            {
                                ListItem.asTypeId = null;
                            });

                        await _unitOfWork.AsTypeRepository.UpdateItem(asType);
                        await _unitOfWork.SaveChangesAsync();
                    }
                    else if (ConfigrationTables.TLIpolarityType.ToString() == TableName)
                    {
                        var polarityType = _unitOfWork.PolarityTypeRepository.GetByID(Id);
                        polarityType.Disable = !(polarityType.Disable);

                        _unitOfWork.MW_DishLibraryRepository
                            .GetWhere(x => x.polarityTypeId == Id && !x.Deleted).ToList()
                            .ForEach(ListItem =>
                            {
                                ListItem.polarityTypeId = null;
                            });

                        await _unitOfWork.PolarityTypeRepository.UpdateItem(polarityType);
                        await _unitOfWork.SaveChangesAsync();
                    }
                    else if (ConfigrationTables.TLIparity.ToString() == TableName)
                    {
                        var parity = _unitOfWork.ParityRepository.GetByID(Id);
                        parity.Disable = !(parity.Disable);

                        _unitOfWork.MW_ODULibraryRepository
                            .GetWhere(x => x.parityId == Id && !x.Deleted).ToList()
                            .ForEach(ListItem =>
                            {
                                ListItem.parityId = null;
                            });

                        await _unitOfWork.ParityRepository.UpdateItem(parity);
                        await _unitOfWork.SaveChangesAsync();
                    }
                    else if (ConfigrationTables.TLIcabinetPowerType.ToString() == TableName)
                    {
                        var cabinetPowerType = _unitOfWork.CabinetPowerTypeRepository.GetByID(Id);
                        cabinetPowerType.Disable = !(cabinetPowerType.Disable);

                        _unitOfWork.CabinetPowerLibraryRepository
                            .GetWhere(x => x.CabinetPowerTypeId == Id && !x.Deleted).ToList()
                            .ForEach(ListItem =>
                            {
                                ListItem.CabinetPowerTypeId = null;
                            });

                        await _unitOfWork.CabinetPowerTypeRepository.UpdateItem(cabinetPowerType);
                        await _unitOfWork.SaveChangesAsync();
                    }
                    else if (ConfigrationTables.TLIcapacity.ToString() == TableName)
                    {
                        var capacity = _unitOfWork.CapacityRepository.GetByID(Id);
                        capacity.Disable = !(capacity.Disable);

                        _unitOfWork.GeneratorLibraryRepository
                            .GetWhere(x => x.CapacityId == Id && !x.Deleted).ToList()
                            .ForEach(ListItem =>
                            {
                                ListItem.CapacityId = null;
                            });

                        _unitOfWork.SolarLibraryRepository
                            .GetWhere(x => x.CapacityId == Id && !x.Deleted).ToList()
                            .ForEach(ListItem =>
                            {
                                ListItem.CapacityId = null;
                            });

                        await _unitOfWork.CapacityRepository.UpdateItem(capacity);
                        await _unitOfWork.SaveChangesAsync();
                    }
                    else if (ConfigrationTables.TLIowner.ToString() == TableName)
                    {
                        var Entity = _unitOfWork.OwnerRepository.GetByID(Id);
                        Entity.Disable = !(Entity.Disable);

                        _unitOfWork.CivilSiteDateRepository
                            .GetIncludeWhere(x => !x.Dismantle && !x.allCivilInst.Draft && (x.allCivilInst.civilWithLegsId != null ?
                                x.allCivilInst.civilWithLegs.OwnerId == Id : false),
                                    x => x.allCivilInst, x => x.allCivilInst.civilWithLegs).Select(x => x.allCivilInst.civilWithLegs)
                            .ToList().ForEach(ListItem =>
                            {
                                ListItem.OwnerId = null;
                            });

                        _unitOfWork.CivilSiteDateRepository
                            .GetIncludeWhere(x => !x.Dismantle && !x.allCivilInst.Draft && (x.allCivilInst.civilWithoutLegId != null ?
                                x.allCivilInst.civilWithoutLeg.OwnerId == Id : false),
                                    x => x.allCivilInst, x => x.allCivilInst.civilWithoutLeg).Select(x => x.allCivilInst.civilWithoutLeg)
                            .ToList().ForEach(ListItem =>
                            {
                                ListItem.OwnerId = null;
                            });

                        _unitOfWork.CivilSiteDateRepository
                            .GetIncludeWhere(x => !x.Dismantle && !x.allCivilInst.Draft && (x.allCivilInst.civilNonSteelId != null ?
                                x.allCivilInst.civilNonSteel.ownerId == Id : false),
                                    x => x.allCivilInst, x => x.allCivilInst.civilNonSteel).Select(x => x.allCivilInst.civilNonSteel)
                            .ToList().ForEach(ListItem =>
                            {
                                ListItem.ownerId = null;
                            });

                        _unitOfWork.CivilLoadsRepository
                            .GetIncludeWhere(x => !x.Dismantle && !x.allLoadInst.Draft && (x.allLoadInst.mwRFUId != null ?
                                x.allLoadInst.mwRFU.OwnerId == Id : false),
                                    x => x.allLoadInst, x => x.allLoadInst.mwRFU).Select(x => x.allLoadInst.mwRFU)
                            .ToList().ForEach(ListItem =>
                            {
                                ListItem.OwnerId = null;
                            });

                        _unitOfWork.CivilLoadsRepository
                            .GetIncludeWhere(x => !x.Dismantle && !x.allLoadInst.Draft && (x.allLoadInst.mwBUId != null ?
                                x.allLoadInst.mwBU.OwnerId == Id : false),
                                    x => x.allLoadInst, x => x.allLoadInst.mwBU).Select(x => x.allLoadInst.mwBU)
                            .ToList().ForEach(ListItem =>
                            {
                                ListItem.OwnerId = null;
                            });

                        _unitOfWork.CivilLoadsRepository
                            .GetIncludeWhere(x => !x.Dismantle && !x.allLoadInst.Draft && (x.allLoadInst.mwDishId != null ?
                                x.allLoadInst.mwDish.ownerId == Id : false),
                                    x => x.allLoadInst, x => x.allLoadInst.mwDish).Select(x => x.allLoadInst.mwDish)
                            .ToList().ForEach(ListItem =>
                            {
                                ListItem.ownerId = null;
                            });

                        _unitOfWork.CivilLoadsRepository
                            .GetIncludeWhere(x => !x.Dismantle && !x.allLoadInst.Draft && (x.allLoadInst.mwODUId != null ?
                                x.allLoadInst.mwODU.OwnerId == Id : false),
                                    x => x.allLoadInst, x => x.allLoadInst.mwODU).Select(x => x.allLoadInst.mwODU)
                            .ToList().ForEach(ListItem =>
                            {
                                ListItem.OwnerId = null;
                            });

                        _unitOfWork.CivilLoadsRepository
                            .GetIncludeWhere(x => !x.Dismantle && !x.allLoadInst.Draft && (x.allLoadInst.radioAntennaId != null ?
                                x.allLoadInst.radioAntenna.ownerId == Id : false),
                                    x => x.allLoadInst, x => x.allLoadInst.radioAntenna).Select(x => x.allLoadInst.radioAntenna)
                            .ToList().ForEach(ListItem =>
                            {
                                ListItem.ownerId = null;
                            });

                        _unitOfWork.CivilLoadsRepository
                            .GetIncludeWhere(x => !x.Dismantle && !x.allLoadInst.Draft && (x.allLoadInst.radioRRUId != null ?
                                x.allLoadInst.radioRRU.ownerId == Id : false),
                                    x => x.allLoadInst, x => x.allLoadInst.radioRRU).Select(x => x.allLoadInst.radioRRU)
                            .ToList().ForEach(ListItem =>
                            {
                                ListItem.ownerId = null;
                            });

                        _unitOfWork.CivilLoadsRepository
                            .GetIncludeWhere(x => !x.Dismantle && !x.allLoadInst.Draft && (x.allLoadInst.radioOtherId != null ?
                                x.allLoadInst.radioOther.ownerId == Id : false),
                                    x => x.allLoadInst, x => x.allLoadInst.radioOther).Select(x => x.allLoadInst.radioOther)
                            .ToList().ForEach(ListItem =>
                            {
                                ListItem.ownerId = 0;
                            });

                        _unitOfWork.CivilLoadsRepository
                            .GetIncludeWhere(x => !x.Dismantle && !x.allLoadInst.Draft && (x.allLoadInst.powerId != null ?
                                x.allLoadInst.power.ownerId == Id : false),
                                    x => x.allLoadInst, x => x.allLoadInst.power).Select(x => x.allLoadInst.power)
                            .ToList().ForEach(ListItem =>
                            {
                                ListItem.ownerId = null;
                            });

                        _unitOfWork.CivilLoadsRepository
                            .GetIncludeWhere(x => !x.Dismantle && (x.sideArmId != null ? 
                                (!x.sideArm.Draft && x.sideArm.ownerId == Id) : false),
                                    x => x.sideArm).Select(x => x.sideArm)
                            .ToList().ForEach(ListItem =>
                            {
                                ListItem.ownerId = null;
                            });

                        await _unitOfWork.OwnerRepository.UpdateItem(Entity);
                        await _unitOfWork.SaveChangesAsync();
                    }
                    else if (ConfigrationTables.TLIlocationType.ToString() == TableName)
                    {
                        var Entity = _unitOfWork.LocationTypeRepository.GetByID(Id);
                        Entity.Disable = !(Entity.Disable);

                        _unitOfWork.CivilSiteDateRepository
                            .GetIncludeWhere(x => !x.Dismantle && !x.allCivilInst.Draft && (x.allCivilInst.civilWithLegsId != null ?
                                x.allCivilInst.civilWithLegs.locationTypeId == Id : false),
                                    x => x.allCivilInst, x => x.allCivilInst.civilWithLegs).Select(x => x.allCivilInst.civilWithLegs)
                            .ToList().ForEach(ListItem =>
                            {
                                ListItem.locationTypeId = null;
                            });

                        _unitOfWork.CivilSiteDateRepository
                            .GetIncludeWhere(x => !x.Dismantle && !x.allCivilInst.Draft && (x.allCivilInst.civilNonSteelId != null ?
                                x.allCivilInst.civilNonSteel.locationTypeId == Id : false),
                                    x => x.allCivilInst, x => x.allCivilInst.civilNonSteel).Select(x => x.allCivilInst.civilNonSteel)
                            .ToList().ForEach(ListItem =>
                            {
                                ListItem.locationTypeId = null;
                            });

                        await _unitOfWork.LocationTypeRepository.UpdateItem(Entity);
                        await _unitOfWork.SaveChangesAsync();
                    }
                    else if (ConfigrationTables.TLIbaseType.ToString() == TableName)
                    {
                        var Entity = _unitOfWork.BaseTypeRepository.GetByID(Id);
                        Entity.Disable = !(Entity.Disable);

                        _unitOfWork.CivilSiteDateRepository
                            .GetIncludeWhere(x => !x.Dismantle && !x.allCivilInst.Draft && (x.allCivilInst.civilWithLegsId != null ?
                                x.allCivilInst.civilWithLegs.baseTypeId == Id : false),
                                    x => x.allCivilInst, x => x.allCivilInst.civilWithLegs).Select(x => x.allCivilInst.civilWithLegs)
                            .ToList().ForEach(ListItem =>
                            {
                                ListItem.baseTypeId = 0;
                            });

                        await _unitOfWork.BaseTypeRepository.UpdateItem(Entity);
                        await _unitOfWork.SaveChangesAsync();
                    }
                    else if (ConfigrationTables.TLIbaseBU.ToString() == TableName)
                    {
                        var Entity = _unitOfWork.BaseBURepository.GetByID(Id);
                        Entity.Disable = !(Entity.Disable);

                        _unitOfWork.CivilLoadsRepository
                            .GetIncludeWhere(x => !x.Dismantle && !x.allLoadInst.Draft && (x.allLoadInst.mwBUId != null ?
                                x.allLoadInst.mwBU.BaseBUId == Id : false),
                                    x => x.allLoadInst, x => x.allLoadInst.mwBU).Select(x => x.allLoadInst.mwBU)
                            .ToList().ForEach(ListItem =>
                            {
                                ListItem.BaseBUId = 0;
                            });

                        await _unitOfWork.BaseBURepository.UpdateItem(Entity);
                        await _unitOfWork.SaveChangesAsync();
                    }
                    else if (ConfigrationTables.TLIrenewableCabinetType.ToString() == TableName)
                    {
                        var Entity = _unitOfWork.RenewableCabinetTypeRepository.GetByID(Id);
                        Entity.Disable = !(Entity.Disable);

                        _unitOfWork.OtherInSiteRepository
                            .GetIncludeWhere(x => !x.Dismantle && !x.allOtherInventoryInst.Draft && (x.allOtherInventoryInst.cabinetId != null ?
                                x.allOtherInventoryInst.cabinet.RenewableCabinetTypeId == Id : false), 
                                    x => x.allOtherInventoryInst, x => x.allOtherInventoryInst.cabinet).Select(x => x.allOtherInventoryInst.cabinet)
                            .ToList().ForEach(ListItem =>
                            {
                                ListItem.RenewableCabinetTypeId = null;
                            });

                        await _unitOfWork.RenewableCabinetTypeRepository.UpdateItem(Entity);
                        await _unitOfWork.SaveChangesAsync();
                    }
                    else if (ConfigrationTables.TLIsideArmType.ToString() == TableName)
                    {
                        var Entity = _unitOfWork.SideArmTypeRepository.GetByID(Id);
                        Entity.Disable = !(Entity.Disable);
                        await _unitOfWork.SideArmTypeRepository.UpdateItem(Entity);
                        await _unitOfWork.SaveChangesAsync();
                    }
                    else if (ConfigrationTables.TLIitemStatus.ToString() == TableName)
                    {
                        var Entity = _unitOfWork.ItemStatusRepository.GetByID(Id);
                        Entity.Active = !(Entity.Active);
                        await _unitOfWork.ItemStatusRepository.UpdateItem(Entity);
                        await _unitOfWork.SaveChangesAsync();
                    }
                    else if (ConfigrationTables.TLIinstallationPlace.ToString() == TableName)
                    {
                        var Entity = _unitOfWork.InstallationPlaceRepository.GetByID(Id);
                        Entity.Disable = !(Entity.Disable);
                        await _unitOfWork.InstallationPlaceRepository.UpdateItem(Entity);
                        await _unitOfWork.SaveChangesAsync();
                    }
                    else
                    {
                        var dynamicListValues = _unitOfWork.DynamicListValuesRepository.GetByID(Id);
                        dynamicListValues.Disable = !(dynamicListValues.Disable);
                        await _unitOfWork.DynamicListValuesRepository.UpdateItem(dynamicListValues);
                        await _unitOfWork.SaveChangesAsync();
                    }
                }
                else
                {
                    if (ConfigrationTables.TLIdiversityType.ToString() == TableName)
                    {
                        List<TLImwBULibrary> MW_BULibraries = _unitOfWork.MW_BULibraryRepository
                            .GetWhere(x => x.diversityTypeId == Id && !x.Deleted).ToList();

                        List<TLImwRFULibrary> MW_RFULibraries = _unitOfWork.MW_RFULibraryRepository
                            .GetWhere(x => x.diversityTypeId == Id && !x.Deleted).ToList();

                        List<TableAffected> ListOfResponse = new List<TableAffected>();

                        if (MW_BULibraries.Count != 0)
                        {
                            ListOfResponse.Add(new TableAffected()
                            {
                                TableName = TablesNames.TLImwBULibrary.ToString(),
                                isLibrary = true,
                                RecordsAffected = MW_BULibraries.Select(x => new RecordAffected
                                {
                                    RecordName = x.Model,
                                    SiteCode = null
                                }).ToList()
                            });
                        }    
                        if (MW_RFULibraries.Count != 0)
                        {
                            ListOfResponse.Add(new TableAffected()
                            {
                                TableName = TablesNames.TLImwRFULibrary.ToString(),
                                isLibrary = true,
                                RecordsAffected = MW_RFULibraries.Select(x => new RecordAffected
                                {
                                    RecordName = x.Model,
                                    SiteCode = null
                                }).ToList()
                            });
                        }

                        if (ListOfResponse.Count != 0)
                            return new Response<List<TableAffected>>(true, ListOfResponse, null, null, (int)Helpers.Constants.ApiReturnCode.success);

                        var diversityType = _unitOfWork.DiversityTypeRepository.GetByID(Id);
                        diversityType.Disable = !(diversityType.Disable);
                        _unitOfWork.DiversityTypeRepository.Update(diversityType);
                        await _unitOfWork.SaveChangesAsync();
                    }
                    else if (ConfigrationTables.TLItelecomType.ToString() == TableName)
                    {
                        List<TLIcabinetTelecomLibrary> CabinetTelecomLibrary = _unitOfWork.CabinetTelecomLibraryRepository
                            .GetIncludeWhere(x => x.TelecomTypeId == Id && !x.Deleted).ToList();

                        List<TableAffected> ListOfResponse = new List<TableAffected>();

                        if (CabinetTelecomLibrary.Count != 0)
                        {
                            ListOfResponse.Add(new TableAffected()
                            {
                                TableName = TablesNames.TLIcabinetTelecomLibrary.ToString(),
                                isLibrary = true,
                                RecordsAffected = CabinetTelecomLibrary.Select(x => new RecordAffected
                                {
                                    RecordName = x.Model,
                                    SiteCode = null
                                }).ToList()
                            });
                        }

                        if (ListOfResponse.Count != 0)
                            return new Response<List<TableAffected>>(true, ListOfResponse, null, null, (int)Helpers.Constants.ApiReturnCode.success);

                        var TelecomType = _unitOfWork.TelecomTypeRepository.GetByID(Id);
                        TelecomType.Disable = !(TelecomType.Disable);
                        await _unitOfWork.TelecomTypeRepository.UpdateItem(TelecomType);
                        await _unitOfWork.SaveChangesAsync();
                    }
                    else if (ConfigrationTables.TLIsupportTypeDesigned.ToString() == TableName)
                    {
                        List<TLIcivilWithLegLibrary> CivilWithLegLibrary = _unitOfWork.CivilWithLegLibraryRepository
                            .GetWhere(x => x.supportTypeDesignedId == Id && !x.Deleted).ToList();

                        List<TableAffected> ListOfResponse = new List<TableAffected>();

                        if (CivilWithLegLibrary.Count != 0)
                        {
                            ListOfResponse.Add(new TableAffected()
                            {
                                TableName = TablesNames.TLIcivilWithLegLibrary.ToString(),
                                isLibrary = true,
                                RecordsAffected = CivilWithLegLibrary.Select(x => new RecordAffected
                                {
                                    RecordName = x.Model,
                                    SiteCode = null
                                }).ToList()
                            });
                        }

                        if (ListOfResponse.Count != 0)
                            return new Response<List<TableAffected>>(true, ListOfResponse, null, null, (int)Helpers.Constants.ApiReturnCode.success);

                        var supportTypeDesigned = _unitOfWork.SupportTypeDesignedRepository.GetByID(Id);
                        supportTypeDesigned.Disable = !(supportTypeDesigned.Disable);
                        await _unitOfWork.SupportTypeDesignedRepository.UpdateItem(supportTypeDesigned);
                        await _unitOfWork.SaveChangesAsync();
                    }
                    else if (ConfigrationTables.TLIsupportTypeImplemented.ToString() == TableName)
                    {
                        List<TLIcivilSiteDate> CivilWithLegInstallation = _unitOfWork.CivilSiteDateRepository
                            .GetIncludeWhere(x => !x.Dismantle && !x.allCivilInst.Draft && (x.allCivilInst.civilWithLegsId != null ? 
                                x.allCivilInst.civilWithLegs.SupportTypeImplementedId == Id : false), 
                                    x => x.allCivilInst, x => x.allCivilInst.civilWithLegs).ToList();

                        List<TLIcivilSiteDate> CivilNonSteelInstallation = _unitOfWork.CivilSiteDateRepository
                            .GetIncludeWhere(x => !x.Dismantle && !x.allCivilInst.Draft && (x.allCivilInst.civilNonSteelId != null ?
                                x.allCivilInst.civilNonSteel.supportTypeImplementedId == Id : false),
                                    x => x.allCivilInst, x => x.allCivilInst.civilNonSteel).ToList();

                        List<TableAffected> ListOfResponse = new List<TableAffected>();

                        if (CivilWithLegInstallation.Count != 0)
                        {
                            ListOfResponse.Add(new TableAffected()
                            {
                                TableName = TablesNames.TLIcivilWithLegs.ToString(),
                                isLibrary = false,
                                RecordsAffected = CivilWithLegInstallation.Select(x => new RecordAffected
                                {
                                    RecordName = x.allCivilInst.civilWithLegs.Name,
                                    SiteCode = x.SiteCode
                                }).ToList()
                            });
                        }
                        if (CivilNonSteelInstallation.Count != 0)
                        {
                            ListOfResponse.Add(new TableAffected()
                            {
                                TableName = TablesNames.TLIcivilNonSteel.ToString(),
                                isLibrary = false,
                                RecordsAffected = CivilNonSteelInstallation.Select(x => new RecordAffected
                                {
                                    RecordName = x.allCivilInst.civilNonSteel.Name,
                                    SiteCode = x.SiteCode
                                }).ToList()
                            });
                        }

                        if (ListOfResponse.Count != 0)
                            return new Response<List<TableAffected>>(true, ListOfResponse, null, null, (int)Helpers.Constants.ApiReturnCode.success);

                        var supportTypeImplemented = _unitOfWork.SupportTypeImplementedRepository.GetByID(Id);
                        supportTypeImplemented.Disable = !(supportTypeImplemented.Disable);
                        await _unitOfWork.SupportTypeImplementedRepository.UpdateItem(supportTypeImplemented);
                        await _unitOfWork.SaveChangesAsync();
                    }
                    else if (ConfigrationTables.TLIstructureType.ToString() == TableName)
                    {
                        List<TLIcivilWithLegLibrary> CivilWithLegLibrary = _unitOfWork.CivilWithLegLibraryRepository
                            .GetWhere(x => x.structureTypeId == Id && !x.Deleted).ToList();

                        List<TLIcivilWithoutLegLibrary> CivilWithoutLegLibrary = _unitOfWork.CivilWithoutLegLibraryRepository
                            .GetWhere(x => x.structureTypeId == Id).ToList();

                        List<TableAffected> ListOfResponse = new List<TableAffected>();

                        if (CivilWithLegLibrary.Count != 0)
                        {
                            ListOfResponse.Add(new TableAffected()
                            {
                                TableName = TablesNames.TLIcivilWithLegLibrary.ToString(),
                                isLibrary = true,
                                RecordsAffected = CivilWithLegLibrary.Select(x => new RecordAffected
                                {
                                    RecordName = x.Model,
                                    SiteCode = null
                                }).ToList()
                            });
                        }
                        if (CivilWithoutLegLibrary.Count != 0)
                        {
                            ListOfResponse.Add(new TableAffected()
                            {
                                TableName = TablesNames.TLIcivilWithoutLegLibrary.ToString(),
                                isLibrary = true,
                                RecordsAffected = CivilWithoutLegLibrary.Select(x => new RecordAffected
                                {
                                    RecordName = x.Model,
                                    SiteCode = null
                                }).ToList()
                            });
                        }

                        if (ListOfResponse.Count != 0)
                            return new Response<List<TableAffected>>(true, ListOfResponse, null, null, (int)Helpers.Constants.ApiReturnCode.success);

                        var structureType = _unitOfWork.StructureTypeRepository.GetByID(Id);
                        structureType.Disable = !(structureType.Disable);
                        await _unitOfWork.StructureTypeRepository.UpdateItem(structureType);
                        await _unitOfWork.SaveChangesAsync();
                    }
                    else if (ConfigrationTables.TLIsectionsLegType.ToString() == TableName)
                    {
                        List<TLIcivilWithLegLibrary> CivilWithLegLibrary = _unitOfWork.CivilWithLegLibraryRepository
                            .GetWhere(x => x.sectionsLegTypeId == Id && !x.Deleted).ToList();

                        List<TableAffected> ListOfResponse = new List<TableAffected>();

                        if (CivilWithLegLibrary.Count != 0)
                        {
                            ListOfResponse.Add(new TableAffected()
                            {
                                TableName = TablesNames.TLIcivilWithLegLibrary.ToString(),
                                isLibrary = true,
                                RecordsAffected = CivilWithLegLibrary.Select(x => new RecordAffected
                                {
                                    RecordName = x.Model,
                                    SiteCode = null
                                }).ToList()
                            });
                        }
                        
                        if (ListOfResponse.Count != 0)
                            return new Response<List<TableAffected>>(true, ListOfResponse, null, null, (int)Helpers.Constants.ApiReturnCode.success);

                        var sectionsLegType = _unitOfWork.SectionsLegTypeRepository.GetByID(Id);
                        sectionsLegType.Disable = !(sectionsLegType.Disable);
                        await _unitOfWork.SectionsLegTypeRepository.UpdateItem(sectionsLegType);
                        await _unitOfWork.SaveChangesAsync();
                    }
                    else if (ConfigrationTables.TLIlogisticalType.ToString() == TableName)
                    {
                        var logisticalType = _unitOfWork.logisticalTypeRepository.GetByID(Id);
                        logisticalType.Disable = !(logisticalType.Disable);
                        await _unitOfWork.logisticalTypeRepository.UpdateItem(logisticalType);
                        await _unitOfWork.SaveChangesAsync();
                    }
                    else if (ConfigrationTables.TLIbaseCivilWithLegsType.ToString() == TableName)
                    {
                        List<TLIcivilSiteDate> CivilWithLegsInstallation = _unitOfWork.CivilSiteDateRepository
                            .GetIncludeWhere(x => !x.Dismantle && !x.allCivilInst.Draft && (x.allCivilInst.civilWithLegsId != null ?
                                x.allCivilInst.civilWithLegs.BaseCivilWithLegTypeId == Id : false),
                                    x => x.allCivilInst, x => x.allCivilInst.civilWithLegs).ToList();

                        List<TableAffected> ListOfResponse = new List<TableAffected>();

                        if (CivilWithLegsInstallation.Count != 0)
                        {
                            ListOfResponse.Add(new TableAffected()
                            {
                                TableName = TablesNames.TLIcivilWithLegs.ToString(),
                                isLibrary = false,
                                RecordsAffected = CivilWithLegsInstallation.Select(x => new RecordAffected
                                {
                                    RecordName = x.allCivilInst.civilWithLegs.Name,
                                    SiteCode = x.SiteCode
                                }).ToList()
                            });
                        }

                        if (ListOfResponse.Count != 0)
                            return new Response<List<TableAffected>>(true, ListOfResponse, null, null, (int)Helpers.Constants.ApiReturnCode.success);

                        var baseCivilWithLegs = _unitOfWork.BaseCivilWithLegsTypeRepository.GetByID(Id);
                        baseCivilWithLegs.Disable = !(baseCivilWithLegs.Disable);
                        await _unitOfWork.BaseCivilWithLegsTypeRepository.UpdateItem(baseCivilWithLegs);
                        await _unitOfWork.SaveChangesAsync();
                    }
                    else if (ConfigrationTables.TLIbaseGeneratorType.ToString() == TableName)
                    {
                        List<TLIotherInSite> GeneratorInstallation = _unitOfWork.OtherInSiteRepository
                            .GetWhere(x => !x.Dismantle && !x.allOtherInventoryInst.Draft && (x.allOtherInventoryInst.generatorId != null ?
                                x.allOtherInventoryInst.generator.BaseGeneratorTypeId == Id : false)).ToList();

                        List<TableAffected> ListOfResponse = new List<TableAffected>();

                        if (GeneratorInstallation.Count != 0)
                        {
                            ListOfResponse.Add(new TableAffected()
                            {
                                TableName = TablesNames.TLIgenerator.ToString(),
                                isLibrary = false,
                                RecordsAffected = GeneratorInstallation.Select(x => new RecordAffected
                                {
                                    RecordName = x.allOtherInventoryInst.generator.Name,
                                    SiteCode = x.SiteCode
                                }).ToList()
                            });
                        }

                        if (ListOfResponse.Count != 0)
                            return new Response<List<TableAffected>>(true, ListOfResponse, null, null, (int)Helpers.Constants.ApiReturnCode.success);

                        var baseGeneratorType = _unitOfWork.BaseGeneratorTypeRepository.GetByID(Id);
                        baseGeneratorType.Disable = !(baseGeneratorType.Disable);
                        await _unitOfWork.BaseGeneratorTypeRepository.UpdateItem(baseGeneratorType);
                        await _unitOfWork.SaveChangesAsync();
                    }
                    else if (ConfigrationTables.TLIinstallationCivilwithoutLegsType.ToString() == TableName)
                    {
                        List<TLIcivilWithoutLegLibrary> CivilWithoutLegLibrary = _unitOfWork.CivilWithoutLegLibraryRepository
                            .GetWhere(x => x.InstallationCivilwithoutLegsTypeId == Id && !x.Deleted).ToList();

                        List<TableAffected> ListOfResponse = new List<TableAffected>();

                        if (CivilWithoutLegLibrary.Count != 0)
                        {
                            ListOfResponse.Add(new TableAffected()
                            {
                                TableName = TablesNames.TLIcivilWithoutLegLibrary.ToString(),
                                isLibrary = true,
                                RecordsAffected = CivilWithoutLegLibrary.Select(x => new RecordAffected
                                {
                                    RecordName = x.Model,
                                    SiteCode = null
                                }).ToList()
                            });
                        }

                        if (ListOfResponse.Count != 0)
                            return new Response<List<TableAffected>>(true, ListOfResponse, null, null, (int)Helpers.Constants.ApiReturnCode.success);

                        var installationCivilwithoutLegsType = _unitOfWork.InstallationCivilwithoutLegsTypeRepository.GetByID(Id);
                        installationCivilwithoutLegsType.Disable = !(installationCivilwithoutLegsType.Disable);
                        await _unitOfWork.InstallationCivilwithoutLegsTypeRepository.UpdateItem(installationCivilwithoutLegsType);
                        await _unitOfWork.SaveChangesAsync();
                    }
                    else if (ConfigrationTables.TLIboardType.ToString() == TableName)
                    {
                        List<TLImwRFULibrary> MW_RFULibrary = _unitOfWork.MW_RFULibraryRepository
                            .GetWhere(x => x.boardTypeId == Id && !x.Deleted).ToList();

                        List<TableAffected> ListOfResponse = new List<TableAffected>();

                        if (MW_RFULibrary.Count != 0)
                        {
                            ListOfResponse.Add(new TableAffected()
                            {
                                TableName = TablesNames.TLImwRFULibrary.ToString(),
                                isLibrary = true,
                                RecordsAffected = MW_RFULibrary.Select(x => new RecordAffected
                                {
                                    RecordName = x.Model,
                                    SiteCode = null
                                }).ToList()
                            });
                        }

                        if (ListOfResponse.Count != 0)
                            return new Response<List<TableAffected>>(true, ListOfResponse, null, null, (int)Helpers.Constants.ApiReturnCode.success);

                        var boardType = _unitOfWork.BoardTypeRepository.GetByID(Id);
                        boardType.Disable = !(boardType.Disable);
                        await _unitOfWork.BoardTypeRepository.UpdateItem(boardType);
                        await _unitOfWork.SaveChangesAsync();
                    }
                    else if (ConfigrationTables.TLIguyLineType.ToString() == TableName)
                    {
                        List<TLIcivilSiteDate> CivilWithLegInstallation = _unitOfWork.CivilSiteDateRepository
                            .GetIncludeWhere(x => !x.Dismantle && !x.allCivilInst.Draft && (x.allCivilInst.civilWithLegsId != null ?
                                x.allCivilInst.civilWithLegs.GuylineTypeId == Id : false),
                                    x => x.allCivilInst, x => x.allCivilInst.civilWithLegs).ToList();

                        List<TableAffected> ListOfResponse = new List<TableAffected>();

                        if (CivilWithLegInstallation.Count != 0)
                        {
                            ListOfResponse.Add(new TableAffected()
                            {
                                TableName = TablesNames.TLIcivilWithLegs.ToString(),
                                isLibrary = false,
                                RecordsAffected = CivilWithLegInstallation.Select(x => new RecordAffected
                                {
                                    RecordName = x.allCivilInst.civilWithLegs.Name,
                                    SiteCode = x.SiteCode
                                }).ToList()
                            });
                        }

                        if (ListOfResponse.Count != 0)
                            return new Response<List<TableAffected>>(true, ListOfResponse, null, null, (int)Helpers.Constants.ApiReturnCode.success);

                        var guyLineType = _unitOfWork.GuyLineTypeRepository.GetByID(Id);
                        guyLineType.Disable = !(guyLineType.Disable);
                        await _unitOfWork.GuyLineTypeRepository.UpdateItem(guyLineType);
                        await _unitOfWork.SaveChangesAsync();
                    }
                    else if (ConfigrationTables.TLIpolarityOnLocation.ToString() == TableName)
                    {
                        List<TLIcivilLoads> MW_DishInstallation = _unitOfWork.CivilLoadsRepository
                            .GetIncludeWhere(x => !x.Dismantle && !x.allLoadInst.Draft && (x.allLoadInst.mwDishId != null ?
                                x.allLoadInst.mwDish.PolarityOnLocationId == Id : false),
                                    x => x.allLoadInst, x => x.allLoadInst.mwDish).ToList();

                        List<TableAffected> ListOfResponse = new List<TableAffected>();

                        if (MW_DishInstallation.Count != 0)
                        {
                            ListOfResponse.Add(new TableAffected()
                            {
                                TableName = TablesNames.TLImwDish.ToString(),
                                isLibrary = false,
                                RecordsAffected = MW_DishInstallation.Select(x => new RecordAffected
                                {
                                    RecordName = x.allLoadInst.mwDish.DishName,
                                    SiteCode = x.SiteCode
                                }).ToList()
                            });
                        }

                        if (ListOfResponse.Count != 0)
                            return new Response<List<TableAffected>>(true, ListOfResponse, null, null, (int)Helpers.Constants.ApiReturnCode.success);

                        var polarityOnLocation = _unitOfWork.PolarityOnLocationRepository.GetByID(Id);
                        polarityOnLocation.Disable = !(polarityOnLocation.Disable);
                        await _unitOfWork.PolarityOnLocationRepository.UpdateItem(polarityOnLocation);
                        await _unitOfWork.SaveChangesAsync();
                    }
                    else if (ConfigrationTables.TLIitemConnectTo.ToString() == TableName)
                    {
                        List<TLIcivilLoads> MW_DishInstallation = _unitOfWork.CivilLoadsRepository
                            .GetIncludeWhere(x => !x.Dismantle && !x.allLoadInst.Draft && (x.allLoadInst.mwDishId != null ?
                                x.allLoadInst.mwDish.ItemConnectToId == Id : false),
                                    x => x.allLoadInst, x => x.allLoadInst.mwDish).ToList();

                        List<TableAffected> ListOfResponse = new List<TableAffected>();

                        if (MW_DishInstallation.Count != 0)
                        {
                            ListOfResponse.Add(new TableAffected()
                            {
                                TableName = TablesNames.TLImwDish.ToString(),
                                isLibrary = false,
                                RecordsAffected = MW_DishInstallation.Select(x => new RecordAffected
                                {
                                    RecordName = x.allLoadInst.mwDish.DishName,
                                    SiteCode = x.SiteCode
                                }).ToList()
                            });
                        }

                        if (ListOfResponse.Count != 0)
                            return new Response<List<TableAffected>>(true, ListOfResponse, null, null, (int)Helpers.Constants.ApiReturnCode.success);

                        var itemConnectTo = _unitOfWork.ItemConnectToRepository.GetByID(Id);
                        itemConnectTo.Disable = !(itemConnectTo.Disable);
                        await _unitOfWork.ItemConnectToRepository.UpdateItem(itemConnectTo);
                        await _unitOfWork.SaveChangesAsync();
                    }
                    else if (ConfigrationTables.TLIrepeaterType.ToString() == TableName)
                    {
                        List<TLIcivilLoads> MW_DishInstallation = _unitOfWork.CivilLoadsRepository
                            .GetIncludeWhere(x => !x.Dismantle && !x.allLoadInst.Draft && (x.allLoadInst.mwDishId != null ?
                                x.allLoadInst.mwDish.RepeaterTypeId == Id : false),
                                    x => x.allLoadInst, x => x.allLoadInst.mwDish).ToList();

                        List<TableAffected> ListOfResponse = new List<TableAffected>();

                        if (MW_DishInstallation.Count != 0)
                        {
                            ListOfResponse.Add(new TableAffected()
                            {
                                TableName = TablesNames.TLImwDish.ToString(),
                                isLibrary = false,
                                RecordsAffected = MW_DishInstallation.Select(x => new RecordAffected
                                {
                                    RecordName = x.allLoadInst.mwDish.DishName,
                                    SiteCode = x.SiteCode
                                }).ToList()
                            });
                        }

                        if (ListOfResponse.Count != 0)
                            return new Response<List<TableAffected>>(true, ListOfResponse, null, null, (int)Helpers.Constants.ApiReturnCode.success);

                        var repeaterType = _unitOfWork.RepeaterTypeRepository.GetByID(Id);
                        repeaterType.Disable = !(repeaterType.Disable);
                        await _unitOfWork.RepeaterTypeRepository.UpdateItem(repeaterType);
                        await _unitOfWork.SaveChangesAsync();
                    }
                    else if (ConfigrationTables.TLIoduInstallationType.ToString() == TableName)
                    {
                        var oduInstallationType = _unitOfWork.OduInstallationTypeRepository.GetByID(Id);
                        oduInstallationType.Disable = !(oduInstallationType.Disable);
                        await _unitOfWork.OduInstallationTypeRepository.UpdateItem(oduInstallationType);
                        await _unitOfWork.SaveChangesAsync();
                    }
                    else if (ConfigrationTables.TLIsideArmInstallationPlace.ToString() == TableName)
                    {
                        var sideArmInstallationPlace = _unitOfWork.SideArmInstallationPlaceRepository.GetByID(Id);
                        sideArmInstallationPlace.Disable = !(sideArmInstallationPlace.Disable);
                        await _unitOfWork.SideArmInstallationPlaceRepository.UpdateItem(sideArmInstallationPlace);
                        await _unitOfWork.SaveChangesAsync();
                    }
                    else if (ConfigrationTables.TLIdataType.ToString() == TableName)
                    {
                        var dataType = _unitOfWork.DataTypeRepository.GetByID(Id);
                        dataType.Disable = !(dataType.Disable);
                        await _unitOfWork.DataTypeRepository.UpdateItem(dataType);
                        await _unitOfWork.SaveChangesAsync();
                    }
                    else if (ConfigrationTables.TLIoperation.ToString() == TableName)
                    {
                        var operation = _unitOfWork.OperationRepository.GetByID(Id);
                        operation.Disable = !(operation.Disable);
                        await _unitOfWork.OperationRepository.UpdateItem(operation);
                        await _unitOfWork.SaveChangesAsync();
                    }
                    else if (ConfigrationTables.TLIlogicalOperation.ToString() == TableName)
                    {
                        var logicalOperation = _unitOfWork.LogicalOperationRepository.GetByID(Id);
                        logicalOperation.Disable = !(logicalOperation.Disable);
                        await _unitOfWork.LogicalOperationRepository.UpdateItem(logicalOperation);
                        await _unitOfWork.SaveChangesAsync();
                    }
                    else if (ConfigrationTables.TLIenforcmentCategory.ToString() == TableName)
                    {
                        List<TLIcivilSiteDate> CivilWithLegInstallation = _unitOfWork.CivilSiteDateRepository
                            .GetIncludeWhere(x => !x.Dismantle && !x.allCivilInst.Draft && (x.allCivilInst.civilWithLegsId != null ?
                                x.allCivilInst.civilWithLegs.enforcmentCategoryId == Id : false),
                                    x => x.allCivilInst, x => x.allCivilInst.civilWithLegs).ToList();

                        List<TableAffected> ListOfResponse = new List<TableAffected>();

                        if (CivilWithLegInstallation.Count != 0)
                        {
                            ListOfResponse.Add(new TableAffected()
                            {
                                TableName = TablesNames.TLIcivilWithLegs.ToString(),
                                isLibrary = false,
                                RecordsAffected = CivilWithLegInstallation.Select(x => new RecordAffected
                                {
                                    RecordName = x.allCivilInst.civilWithLegs.Name,
                                    SiteCode = x.SiteCode
                                }).ToList()
                            });
                        }

                        if (ListOfResponse.Count != 0)
                            return new Response<List<TableAffected>>(true, ListOfResponse, null, null, (int)Helpers.Constants.ApiReturnCode.success);

                        var enforcmentCategory = _unitOfWork.EnforcmentCategoryRepository.GetByID(Id);
                        enforcmentCategory.Disable = !(enforcmentCategory.Disable);
                        await _unitOfWork.EnforcmentCategoryRepository.UpdateItem(enforcmentCategory);
                        await _unitOfWork.SaveChangesAsync();
                    }
                    else if (ConfigrationTables.TLIpowerType.ToString() == TableName)
                    {
                        List<TLIcivilLoads> PowerInstallation = _unitOfWork.CivilLoadsRepository
                            .GetIncludeWhere(x => !x.Dismantle && !x.allLoadInst.Draft && (x.allLoadInst.powerId != null ?
                                x.allLoadInst.power.powerTypeId == Id : false),
                                    x => x.allLoadInst, x => x.allLoadInst.power).ToList();

                        List<TableAffected> ListOfResponse = new List<TableAffected>();

                        if (PowerInstallation.Count != 0)
                        {
                            ListOfResponse.Add(new TableAffected()
                            {
                                TableName = TablesNames.TLIpower.ToString(),
                                isLibrary = false,
                                RecordsAffected = PowerInstallation.Select(x => new RecordAffected
                                {
                                    RecordName = x.allLoadInst.power.Name,
                                    SiteCode = x.SiteCode
                                }).ToList()
                            });
                        }

                        if (ListOfResponse.Count != 0)
                            return new Response<List<TableAffected>>(true, ListOfResponse, null, null, (int)Helpers.Constants.ApiReturnCode.success);

                        var powerType = _unitOfWork.PowerTypeRepository.GetByID(Id);
                        powerType.Disable = !(powerType.Disable);
                        await _unitOfWork.PowerTypeRepository.UpdateItem(powerType);
                        await _unitOfWork.SaveChangesAsync();
                    }
                    else if (ConfigrationTables.TLIcivilNonSteelType.ToString() == TableName)
                    {
                        List<TLIcivilNonSteelLibrary> CivilNonSteelLibrary = _unitOfWork.CivilNonSteelLibraryRepository
                            .GetWhere(x => x.civilNonSteelTypeId == Id && !x.Deleted).ToList();

                        List<TableAffected> ListOfResponse = new List<TableAffected>();

                        if (CivilNonSteelLibrary.Count != 0)
                        {
                            ListOfResponse.Add(new TableAffected()
                            {
                                TableName = TablesNames.TLIcivilNonSteelLibrary.ToString(),
                                isLibrary = true,
                                RecordsAffected = CivilNonSteelLibrary.Select(x => new RecordAffected
                                {
                                    RecordName = x.Model,
                                    SiteCode = null
                                }).ToList()
                            });
                        }

                        if (ListOfResponse.Count != 0)
                            return new Response<List<TableAffected>>(true, ListOfResponse, null, null, (int)Helpers.Constants.ApiReturnCode.success);

                        var civilNonSteelType = _unitOfWork.CivilNonSteelTypeRepository.GetByID(Id);
                        civilNonSteelType.Disable = !(civilNonSteelType.Disable);
                        await _unitOfWork.CivilNonSteelTypeRepository.UpdateItem(civilNonSteelType);
                        await _unitOfWork.SaveChangesAsync();
                    }
                    else if (ConfigrationTables.TLIsubType.ToString() == TableName)
                    {
                        List<TLIcivilSiteDate> CivilWithoutLegInstallation = _unitOfWork.CivilSiteDateRepository
                            .GetIncludeWhere(x => !x.Dismantle && !x.allCivilInst.Draft && (x.allCivilInst.civilWithoutLegId != null ?
                                x.allCivilInst.civilWithoutLeg.subTypeId == Id : false),
                                    x => x.allCivilInst, x => x.allCivilInst.civilWithoutLeg).ToList();

                        List<TableAffected> ListOfResponse = new List<TableAffected>();

                        if (CivilWithoutLegInstallation.Count != 0)
                        {
                            ListOfResponse.Add(new TableAffected()
                            {
                                TableName = TablesNames.TLIcivilWithoutLeg.ToString(),
                                isLibrary = false,
                                RecordsAffected = CivilWithoutLegInstallation.Select(x => new RecordAffected
                                {
                                    RecordName = x.allCivilInst.civilWithoutLeg.Name,
                                    SiteCode = x.SiteCode
                                }).ToList()
                            });
                        }

                        if (ListOfResponse.Count != 0)
                            return new Response<List<TableAffected>>(true, ListOfResponse, null, null, (int)Helpers.Constants.ApiReturnCode.success);

                        var subType = _unitOfWork.SubTypeRepository.GetByID(Id);
                        subType.Disable = !(subType.Disable);
                        await _unitOfWork.SubTypeRepository.UpdateItem(subType);
                        await _unitOfWork.SaveChangesAsync();
                    }
                    else if (ConfigrationTables.TLIasType.ToString() == TableName)
                    {
                        List<TLImwDishLibrary> MW_DishLibrary = _unitOfWork.MW_DishLibraryRepository
                            .GetWhere(x => x.asTypeId == Id && !x.Deleted).ToList();

                        List<TableAffected> ListOfResponse = new List<TableAffected>();

                        if (MW_DishLibrary.Count != 0)
                        {
                            ListOfResponse.Add(new TableAffected()
                            {
                                TableName = TablesNames.TLImwDishLibrary.ToString(),
                                isLibrary = true,
                                RecordsAffected = MW_DishLibrary.Select(x => new RecordAffected
                                {
                                    RecordName = x.Model,
                                    SiteCode = null
                                }).ToList()
                            });
                        }

                        if (ListOfResponse.Count != 0)
                            return new Response<List<TableAffected>>(true, ListOfResponse, null, null, (int)Helpers.Constants.ApiReturnCode.success);

                        var asType = _unitOfWork.AsTypeRepository.GetByID(Id);
                        asType.Disable = !(asType.Disable);
                        await _unitOfWork.AsTypeRepository.UpdateItem(asType);
                        await _unitOfWork.SaveChangesAsync();
                    }
                    else if (ConfigrationTables.TLIpolarityType.ToString() == TableName)
                    {
                        List<TLImwDishLibrary> MW_DishLibrary = _unitOfWork.MW_DishLibraryRepository
                            .GetWhere(x => x.polarityTypeId == Id && !x.Deleted).ToList();

                        List<TableAffected> ListOfResponse = new List<TableAffected>();

                        if (MW_DishLibrary.Count != 0)
                        {
                            ListOfResponse.Add(new TableAffected()
                            {
                                TableName = TablesNames.TLImwDishLibrary.ToString(),
                                isLibrary = true,
                                RecordsAffected = MW_DishLibrary.Select(x => new RecordAffected
                                {
                                    RecordName = x.Model,
                                    SiteCode = null
                                }).ToList()
                            });
                        }

                        if (ListOfResponse.Count != 0)
                            return new Response<List<TableAffected>>(true, ListOfResponse, null, null, (int)Helpers.Constants.ApiReturnCode.success);

                        var polarityType = _unitOfWork.PolarityTypeRepository.GetByID(Id);
                        polarityType.Disable = !(polarityType.Disable);
                        await _unitOfWork.PolarityTypeRepository.UpdateItem(polarityType);
                        await _unitOfWork.SaveChangesAsync();
                    }
                    else if (ConfigrationTables.TLIparity.ToString() == TableName)
                    {
                        List<TLImwODULibrary> MW_ODULibrary = _unitOfWork.MW_ODULibraryRepository
                            .GetWhere(x => x.parityId == Id && !x.Deleted).ToList();

                        List<TableAffected> ListOfResponse = new List<TableAffected>();

                        if (MW_ODULibrary.Count != 0)
                        {
                            ListOfResponse.Add(new TableAffected()
                            {
                                TableName = TablesNames.TLImwODULibrary.ToString(),
                                isLibrary = true,
                                RecordsAffected = MW_ODULibrary.Select(x => new RecordAffected
                                {
                                    RecordName = x.Model,
                                    SiteCode = null
                                }).ToList()
                            });
                        }

                        if (ListOfResponse.Count != 0)
                            return new Response<List<TableAffected>>(true, ListOfResponse, null, null, (int)Helpers.Constants.ApiReturnCode.success);

                        var parity = _unitOfWork.ParityRepository.GetByID(Id);
                        parity.Disable = !(parity.Disable);
                        await _unitOfWork.ParityRepository.UpdateItem(parity);
                        await _unitOfWork.SaveChangesAsync();
                    }
                    else if (ConfigrationTables.TLIcabinetPowerType.ToString() == TableName)
                    {
                        List<TLIcabinetPowerLibrary> CabinetPowerLibrary = _unitOfWork.CabinetPowerLibraryRepository
                            .GetWhere(x => x.CabinetPowerTypeId == Id && !x.Deleted).ToList();

                        List<TableAffected> ListOfResponse = new List<TableAffected>();

                        if (CabinetPowerLibrary.Count != 0)
                        {
                            ListOfResponse.Add(new TableAffected()
                            {
                                TableName = TablesNames.TLIcabinetPowerLibrary.ToString(),
                                isLibrary = true,
                                RecordsAffected = CabinetPowerLibrary.Select(x => new RecordAffected
                                {
                                    RecordName = x.Model,
                                    SiteCode = null
                                }).ToList()
                            });
                        }

                        if (ListOfResponse.Count != 0)
                            return new Response<List<TableAffected>>(true, ListOfResponse, null, null, (int)Helpers.Constants.ApiReturnCode.success);

                        var cabinetPowerType = _unitOfWork.CabinetPowerTypeRepository.GetByID(Id);
                        cabinetPowerType.Disable = !(cabinetPowerType.Disable);
                        await _unitOfWork.CabinetPowerTypeRepository.UpdateItem(cabinetPowerType);
                        await _unitOfWork.SaveChangesAsync();
                    }
                    else if (ConfigrationTables.TLIcapacity.ToString() == TableName)
                    {
                        List<TLIgeneratorLibrary> GeneratorLibrary = _unitOfWork.GeneratorLibraryRepository
                            .GetWhere(x => x.CapacityId == Id && !x.Deleted).ToList();

                        List<TLIsolarLibrary> SolarLibrary = _unitOfWork.SolarLibraryRepository
                            .GetWhere(x => x.CapacityId == Id && !x.Deleted).ToList();

                        List<TableAffected> ListOfResponse = new List<TableAffected>();

                        if (GeneratorLibrary.Count != 0)
                        {
                            ListOfResponse.Add(new TableAffected()
                            {
                                TableName = TablesNames.TLIgeneratorLibrary.ToString(),
                                isLibrary = true,
                                RecordsAffected = GeneratorLibrary.Select(x => new RecordAffected
                                {
                                    RecordName = x.Model,
                                    SiteCode = null
                                }).ToList()
                            });
                        }
                        if (SolarLibrary.Count != 0)
                        {
                            ListOfResponse.Add(new TableAffected()
                            {
                                TableName = TablesNames.TLIsolarLibrary.ToString(),
                                isLibrary = true,
                                RecordsAffected = SolarLibrary.Select(x => new RecordAffected
                                {
                                    RecordName = x.Model,
                                    SiteCode = null
                                }).ToList()
                            });
                        }

                        if (ListOfResponse.Count != 0)
                            return new Response<List<TableAffected>>(true, ListOfResponse, null, null, (int)Helpers.Constants.ApiReturnCode.success);

                        var capacity = _unitOfWork.CapacityRepository.GetByID(Id);
                        capacity.Disable = !(capacity.Disable);
                        await _unitOfWork.CapacityRepository.UpdateItem(capacity);
                        await _unitOfWork.SaveChangesAsync();
                    }
                    else if (ConfigrationTables.TLIowner.ToString() == TableName)
                    {
                        List<TableAffected> ListOfResponse = new List<TableAffected>();

                        List<TLIcivilSiteDate> CivilWithLegInstallation = _unitOfWork.CivilSiteDateRepository
                            .GetIncludeWhere(x => !x.Dismantle && !x.allCivilInst.Draft && (x.allCivilInst.civilWithLegsId != null ?
                                x.allCivilInst.civilWithLegs.OwnerId == Id : false),
                                    x => x.allCivilInst, x => x.allCivilInst.civilWithLegs).ToList();

                        if (CivilWithLegInstallation.Count != 0)
                        {
                            ListOfResponse.Add(new TableAffected()
                            {
                                TableName = TablesNames.TLIcivilWithLegs.ToString(),
                                isLibrary = false,
                                RecordsAffected = CivilWithLegInstallation.Select(x => new RecordAffected
                                {
                                    RecordName = x.allCivilInst.civilWithLegs.Name,
                                    SiteCode = x.SiteCode
                                }).ToList()
                            });
                        }

                        List<TLIcivilSiteDate> CivilWithoutLegInstallation = _unitOfWork.CivilSiteDateRepository
                            .GetIncludeWhere(x => !x.Dismantle && !x.allCivilInst.Draft && (x.allCivilInst.civilWithoutLegId != null ?
                                x.allCivilInst.civilWithoutLeg.OwnerId == Id : false),
                                    x => x.allCivilInst, x => x.allCivilInst.civilWithoutLeg).ToList();

                        if (CivilWithoutLegInstallation.Count != 0)
                        {
                            ListOfResponse.Add(new TableAffected()
                            {
                                TableName = TablesNames.TLIcivilWithoutLeg.ToString(),
                                isLibrary = false,
                                RecordsAffected = CivilWithoutLegInstallation.Select(x => new RecordAffected
                                {
                                    RecordName = x.allCivilInst.civilWithoutLeg.Name,
                                    SiteCode = x.SiteCode
                                }).ToList()
                            });
                        }

                        List<TLIcivilSiteDate> CivilNonSteelInstallation = _unitOfWork.CivilSiteDateRepository
                            .GetIncludeWhere(x => !x.Dismantle && !x.allCivilInst.Draft && (x.allCivilInst.civilNonSteelId != null ?
                                x.allCivilInst.civilNonSteel.ownerId == Id : false),
                                    x => x.allCivilInst, x => x.allCivilInst.civilNonSteel).ToList();

                        if (CivilNonSteelInstallation.Count != 0)
                        {
                            ListOfResponse.Add(new TableAffected()
                            {
                                TableName = TablesNames.TLIcivilNonSteel.ToString(),
                                isLibrary = false,
                                RecordsAffected = CivilNonSteelInstallation.Select(x => new RecordAffected
                                {
                                    RecordName = x.allCivilInst.civilNonSteel.Name,
                                    SiteCode = x.SiteCode
                                }).ToList()
                            });
                        }

                        List<TLIcivilLoads> MW_RFUInstallation = _unitOfWork.CivilLoadsRepository
                            .GetIncludeWhere(x => !x.Dismantle && !x.allLoadInst.Draft && (x.allLoadInst.mwRFUId != null ?
                                x.allLoadInst.mwRFU.OwnerId == Id : false),
                                    x => x.allLoadInst, x => x.allLoadInst.mwRFU).ToList();

                        if (MW_RFUInstallation.Count != 0)
                        {
                            ListOfResponse.Add(new TableAffected()
                            {
                                TableName = TablesNames.TLImwRFU.ToString(),
                                isLibrary = false,
                                RecordsAffected = MW_RFUInstallation.Select(x => new RecordAffected
                                {
                                    RecordName = x.allLoadInst.mwRFU.Name,
                                    SiteCode = x.SiteCode
                                }).ToList()
                            });
                        }

                        List<TLIcivilLoads> MW_BUInstallation = _unitOfWork.CivilLoadsRepository
                            .GetIncludeWhere(x => !x.Dismantle && !x.allLoadInst.Draft && (x.allLoadInst.mwBUId != null ?
                                x.allLoadInst.mwBU.OwnerId == Id : false),
                                    x => x.allLoadInst, x => x.allLoadInst.mwBU).ToList();

                        if (MW_BUInstallation.Count != 0)
                        {
                            ListOfResponse.Add(new TableAffected()
                            {
                                TableName = TablesNames.TLImwBU.ToString(),
                                isLibrary = false,
                                RecordsAffected = MW_BUInstallation.Select(x => new RecordAffected
                                {
                                    RecordName = x.allLoadInst.mwBU.Name,
                                    SiteCode = x.SiteCode
                                }).ToList()
                            });
                        }

                        List<TLIcivilLoads> MW_DishInstallation = _unitOfWork.CivilLoadsRepository
                            .GetIncludeWhere(x => !x.Dismantle && !x.allLoadInst.Draft && (x.allLoadInst.mwDishId != null ?
                                x.allLoadInst.mwDish.ownerId == Id : false),
                                    x => x.allLoadInst, x => x.allLoadInst.mwDish).ToList();

                        if (MW_DishInstallation.Count != 0)
                        {
                            ListOfResponse.Add(new TableAffected()
                            {
                                TableName = TablesNames.TLImwDish.ToString(),
                                isLibrary = false,
                                RecordsAffected = MW_DishInstallation.Select(x => new RecordAffected
                                {
                                    RecordName = x.allLoadInst.mwDish.DishName,
                                    SiteCode = x.SiteCode
                                }).ToList()
                            });
                        }

                        List<TLIcivilLoads> MW_ODUInstallation = _unitOfWork.CivilLoadsRepository
                            .GetIncludeWhere(x => !x.Dismantle && !x.allLoadInst.Draft && (x.allLoadInst.mwODUId != null ?
                                x.allLoadInst.mwODU.OwnerId == Id : false),
                                    x => x.allLoadInst, x => x.allLoadInst.mwODU).ToList();

                        if (MW_ODUInstallation.Count != 0)
                        {
                            ListOfResponse.Add(new TableAffected()
                            {
                                TableName = TablesNames.TLImwODU.ToString(),
                                isLibrary = false,
                                RecordsAffected = MW_ODUInstallation.Select(x => new RecordAffected
                                {
                                    RecordName = x.allLoadInst.mwODU.Name,
                                    SiteCode = x.SiteCode
                                }).ToList()
                            });
                        }

                        List<TLIcivilLoads> RadioAnteenInstallation = _unitOfWork.CivilLoadsRepository
                            .GetIncludeWhere(x => !x.Dismantle && !x.allLoadInst.Draft && (x.allLoadInst.radioAntennaId != null ?
                                x.allLoadInst.radioAntenna.ownerId == Id : false),
                                    x => x.allLoadInst, x => x.allLoadInst.radioAntenna).ToList();

                        if (RadioAnteenInstallation.Count != 0)
                        {
                            ListOfResponse.Add(new TableAffected()
                            {
                                TableName = TablesNames.TLIradioAntenna.ToString(),
                                isLibrary = false,
                                RecordsAffected = RadioAnteenInstallation.Select(x => new RecordAffected
                                {
                                    RecordName = x.allLoadInst.radioAntenna.Name,
                                    SiteCode = x.SiteCode
                                }).ToList()
                            });
                        }

                        List<TLIcivilLoads> RadioRRUInstallation = _unitOfWork.CivilLoadsRepository
                            .GetIncludeWhere(x => !x.Dismantle && !x.allLoadInst.Draft && (x.allLoadInst.radioRRUId != null ?
                                x.allLoadInst.radioRRU.ownerId == Id : false),
                                    x => x.allLoadInst, x => x.allLoadInst.radioRRU).ToList();

                        if (RadioRRUInstallation.Count != 0)
                        {
                            ListOfResponse.Add(new TableAffected()
                            {
                                TableName = TablesNames.TLIradioRRU.ToString(),
                                isLibrary = false,
                                RecordsAffected = RadioRRUInstallation.Select(x => new RecordAffected
                                {
                                    RecordName = x.allLoadInst.radioRRU.Name,
                                    SiteCode = x.SiteCode
                                }).ToList()
                            });
                        }

                        List<TLIcivilLoads> RadioOtherInstallation = _unitOfWork.CivilLoadsRepository
                            .GetIncludeWhere(x => !x.Dismantle && !x.allLoadInst.Draft && (x.allLoadInst.radioOtherId != null ?
                                x.allLoadInst.radioOther.ownerId == Id : false),
                                    x => x.allLoadInst, x => x.allLoadInst.radioOther).ToList();

                        if (RadioOtherInstallation.Count != 0)
                        {
                            ListOfResponse.Add(new TableAffected()
                            {
                                TableName = TablesNames.TLIradioOther.ToString(),
                                isLibrary = false,
                                RecordsAffected = RadioOtherInstallation.Select(x => new RecordAffected
                                {
                                    RecordName = x.allLoadInst.radioOther.Name,
                                    SiteCode = x.SiteCode
                                }).ToList()
                            });
                        }

                        List<TLIcivilLoads> PowerInstallation = _unitOfWork.CivilLoadsRepository
                            .GetIncludeWhere(x => !x.Dismantle && !x.allLoadInst.Draft && (x.allLoadInst.powerId != null ?
                                x.allLoadInst.power.ownerId == Id : false),
                                    x => x.allLoadInst, x => x.allLoadInst.power).ToList();

                        if (PowerInstallation.Count != 0)
                        {
                            ListOfResponse.Add(new TableAffected()
                            {
                                TableName = TablesNames.TLIpower.ToString(),
                                isLibrary = false,
                                RecordsAffected = PowerInstallation.Select(x => new RecordAffected
                                {
                                    RecordName = x.allLoadInst.power.Name,
                                    SiteCode = x.SiteCode
                                }).ToList()
                            });
                        }

                        List<TLIcivilLoads> SideArmInstallation = _unitOfWork.CivilLoadsRepository
                            .GetIncludeWhere(x => !x.Dismantle && (x.sideArmId != null ?
                                (!x.sideArm.Draft && x.sideArm.ownerId == Id) : false),
                                    x => x.sideArm).ToList();

                        if (SideArmInstallation.Count != 0)
                        {
                            ListOfResponse.Add(new TableAffected()
                            {
                                TableName = TablesNames.TLIsideArm.ToString(),
                                isLibrary = false,
                                RecordsAffected = SideArmInstallation.Select(x => new RecordAffected
                                {
                                    RecordName = x.sideArm.Name,
                                    SiteCode = x.SiteCode
                                }).ToList()
                            });
                        }

                        if (ListOfResponse.Count != 0)
                            return new Response<List<TableAffected>>(true, ListOfResponse, null, null, (int)Helpers.Constants.ApiReturnCode.success);

                        var Entity = _unitOfWork.OwnerRepository.GetByID(Id);
                        Entity.Disable = !(Entity.Disable);
                        await _unitOfWork.OwnerRepository.UpdateItem(Entity);
                        await _unitOfWork.SaveChangesAsync();
                    }
                    else if (ConfigrationTables.TLIlocationType.ToString() == TableName)
                    {
                        List<TableAffected> ListOfResponse = new List<TableAffected>();

                        List<TLIcivilSiteDate> CivilWithLegInstallation = _unitOfWork.CivilSiteDateRepository
                            .GetIncludeWhere(x => !x.Dismantle && !x.allCivilInst.Draft && (x.allCivilInst.civilWithLegsId != null ?
                                x.allCivilInst.civilWithLegs.locationTypeId == Id : false),
                                    x => x.allCivilInst, x => x.allCivilInst.civilWithLegs).ToList();

                        if (CivilWithLegInstallation.Count != 0)
                        {
                            ListOfResponse.Add(new TableAffected()
                            {
                                TableName = TablesNames.TLIcivilWithLegs.ToString(),
                                isLibrary = false,
                                RecordsAffected = CivilWithLegInstallation.Select(x => new RecordAffected
                                {
                                    RecordName = x.allCivilInst.civilWithLegs.Name,
                                    SiteCode = x.SiteCode
                                }).ToList()
                            });
                        }

                        List<TLIcivilSiteDate> CivilNonSteelInstallation = _unitOfWork.CivilSiteDateRepository
                            .GetIncludeWhere(x => !x.Dismantle && !x.allCivilInst.Draft && (x.allCivilInst.civilNonSteelId != null ?
                                x.allCivilInst.civilNonSteel.locationTypeId == Id : false),
                                    x => x.allCivilInst, x => x.allCivilInst.civilNonSteel).ToList();

                        if (CivilNonSteelInstallation.Count != 0)
                        {
                            ListOfResponse.Add(new TableAffected()
                            {
                                TableName = TablesNames.TLIcivilNonSteel.ToString(),
                                isLibrary = false,
                                RecordsAffected = CivilNonSteelInstallation.Select(x => new RecordAffected
                                {
                                    RecordName = x.allCivilInst.civilNonSteel.Name,
                                    SiteCode = x.SiteCode
                                }).ToList()
                            });
                        }

                        if (ListOfResponse.Count != 0)
                            return new Response<List<TableAffected>>(true, ListOfResponse, null, null, (int)Helpers.Constants.ApiReturnCode.success);

                        var Entity = _unitOfWork.LocationTypeRepository.GetByID(Id);
                        Entity.Disable = !(Entity.Disable);
                        await _unitOfWork.LocationTypeRepository.UpdateItem(Entity);
                        await _unitOfWork.SaveChangesAsync();
                    }
                    else if (ConfigrationTables.TLIbaseType.ToString() == TableName)
                    {
                        List<TLIcivilSiteDate> CivilWithLegInstallation = _unitOfWork.CivilSiteDateRepository
                            .GetIncludeWhere(x => !x.Dismantle && !x.allCivilInst.Draft && (x.allCivilInst.civilWithLegsId != null ?
                                x.allCivilInst.civilWithLegs.baseTypeId == Id : false),
                                    x => x.allCivilInst, x => x.allCivilInst.civilWithLegs).ToList();

                        List<TableAffected> ListOfResponse = new List<TableAffected>();

                        if (CivilWithLegInstallation.Count != 0)
                        {
                            ListOfResponse.Add(new TableAffected()
                            {
                                TableName = TablesNames.TLIcivilWithLegs.ToString(),
                                isLibrary = false,
                                RecordsAffected = CivilWithLegInstallation.Select(x => new RecordAffected
                                {
                                    RecordName = x.allCivilInst.civilWithLegs.Name,
                                    SiteCode = x.SiteCode
                                }).ToList()
                            });
                        }

                        if (ListOfResponse.Count != 0)
                            return new Response<List<TableAffected>>(true, ListOfResponse, null, null, (int)Helpers.Constants.ApiReturnCode.success);

                        var Entity = _unitOfWork.BaseTypeRepository.GetByID(Id);
                        Entity.Disable = !(Entity.Disable);
                        await _unitOfWork.BaseTypeRepository.UpdateItem(Entity);
                        await _unitOfWork.SaveChangesAsync();
                    }
                    else if (ConfigrationTables.TLIbaseBU.ToString() == TableName)
                    {
                        List<TLIcivilLoads> MW_BUInstallation = _unitOfWork.CivilLoadsRepository
                            .GetIncludeWhere(x => !x.Dismantle && !x.allLoadInst.Draft && (x.allLoadInst.mwBUId != null ?
                                x.allLoadInst.mwBU.BaseBUId == Id : false),
                                    x => x.allLoadInst, x => x.allLoadInst.mwBU).ToList();

                        List<TableAffected> ListOfResponse = new List<TableAffected>();

                        if (MW_BUInstallation.Count != 0)
                        {
                            ListOfResponse.Add(new TableAffected()
                            {
                                TableName = TablesNames.TLImwBU.ToString(),
                                isLibrary = false,
                                RecordsAffected = MW_BUInstallation.Select(x => new RecordAffected
                                {
                                    RecordName = x.allLoadInst.mwBU.Name,
                                    SiteCode = x.SiteCode
                                }).ToList()
                            });
                        }

                        if (ListOfResponse.Count != 0)
                            return new Response<List<TableAffected>>(true, ListOfResponse, null, null, (int)Helpers.Constants.ApiReturnCode.success);

                        var Entity = _unitOfWork.BaseBURepository.GetByID(Id);
                        Entity.Disable = !(Entity.Disable);
                        await _unitOfWork.BaseBURepository.UpdateItem(Entity);
                        await _unitOfWork.SaveChangesAsync();
                    }
                    else if (ConfigrationTables.TLIrenewableCabinetType.ToString() == TableName)
                    {
                        List<TLIotherInSite> CabinetInstallation = _unitOfWork.OtherInSiteRepository
                            .GetIncludeWhere(x => !x.Dismantle && !x.allOtherInventoryInst.Draft && (x.allOtherInventoryInst.cabinetId != null ?
                                x.allOtherInventoryInst.cabinet.RenewableCabinetTypeId == Id : false),
                                    x => x.allOtherInventoryInst, x => x.allOtherInventoryInst.cabinet).ToList();

                        List<TableAffected> ListOfResponse = new List<TableAffected>();

                        if (CabinetInstallation.Count != 0)
                        {
                            ListOfResponse.Add(new TableAffected()
                            {
                                TableName = TablesNames.TLIcabinet.ToString(),
                                isLibrary = false,
                                RecordsAffected = CabinetInstallation.Select(x => new RecordAffected
                                {
                                    RecordName = x.allOtherInventoryInst.cabinet.Name,
                                    SiteCode = x.SiteCode
                                }).ToList()
                            });
                        }

                        if (ListOfResponse.Count != 0)
                            return new Response<List<TableAffected>>(true, ListOfResponse, null, null, (int)Helpers.Constants.ApiReturnCode.success);

                        var Entity = _unitOfWork.RenewableCabinetTypeRepository.GetByID(Id);
                        Entity.Disable = !(Entity.Disable);
                        await _unitOfWork.RenewableCabinetTypeRepository.UpdateItem(Entity);
                        await _unitOfWork.SaveChangesAsync();
                    }
                    else if (ConfigrationTables.TLIsideArmType.ToString() == TableName)
                    {
                        var Entity = _unitOfWork.SideArmTypeRepository.GetByID(Id);
                        Entity.Disable = !(Entity.Disable);
                        await _unitOfWork.SideArmTypeRepository.UpdateItem(Entity);
                        await _unitOfWork.SaveChangesAsync();
                    }
                    else if (ConfigrationTables.TLIitemStatus.ToString() == TableName)
                    {
                        var Entity = _unitOfWork.ItemStatusRepository.GetByID(Id);
                        Entity.Active = !(Entity.Active);
                        await _unitOfWork.ItemStatusRepository.UpdateItem(Entity);
                        await _unitOfWork.SaveChangesAsync();
                    }
                    else if (ConfigrationTables.TLIinstallationPlace.ToString() == TableName)
                    {
                        var Entity = _unitOfWork.InstallationPlaceRepository.GetByID(Id);
                        Entity.Disable = !(Entity.Disable);
                        await _unitOfWork.InstallationPlaceRepository.UpdateItem(Entity);
                        await _unitOfWork.SaveChangesAsync();
                    }
                    else
                    {
                        var dynamicListValues = _unitOfWork.DynamicListValuesRepository.GetByID(Id);
                        dynamicListValues.Disable = !(dynamicListValues.Disable);
                        await _unitOfWork.DynamicListValuesRepository.UpdateItem(dynamicListValues);
                        await _unitOfWork.SaveChangesAsync();
                    }
                }
                
                return new Response<List<TableAffected>>();
            }
            catch(Exception err)
            {
                return new Response<List<TableAffected>>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
        }

        public Response<List<ConfigurationListViewModel>> GetConfigrationTables(string TableName)
        {
            try
            {
                List<ConfigurationListViewModel> Result = new List<ConfigurationListViewModel>();

                bool exists = Enum.IsDefined(typeof(ConfigrationTablesAfterUpdate), TableName);
                if (exists)
                {
                    ConfigrationTablesAfterUpdate config = (ConfigrationTablesAfterUpdate)Enum.Parse(typeof(ConfigrationTablesAfterUpdate), TableName);

                    List<string> Descriptions = Constants.GetEnumDescription(config).Split(" ").ToList();

                    foreach (string Description in Descriptions)
                    {
                        if (Description.ToString().ToLower() != "TLIcivilSteelSupportCategory".ToLower() && Description.ToString().ToLower() != "TLIcivilSteelSupportCategory".ToLower() &&
                            Description.ToString().ToLower() != "TLIcivilWithoutLegCategory".ToLower())
                        {
                            Result.Add(new ConfigurationListViewModel(Description.ToString(), false));
                        }
                    }
                }

                //List<ConfigurationListViewModel> dynamicList = _unitOfWork.DynamicAttRepository.GetIncludeWhereSelect(x => 
                //    (x.tablesNames.TableName.ToLower() == TableName.ToLower() && x.DataType.Name.ToLower() == "list"),
                //        x => (new ConfigurationListViewModel(x.Id, x.Key, true)), x => x.tablesNames, x => x.DataType).ToList();

                //Result.AddRange(dynamicList);
                return new Response<List<ConfigurationListViewModel>>(true, Result, null, null, (int)Helpers.Constants.ApiReturnCode.success);
            }
            catch (Exception err)
            {
                return new Response<List<ConfigurationListViewModel>>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
        }
        public Response<List<ConfigurationAttsViewModel>> GetAll(string TableName, ParameterPagination Pagination)
        {
            try
            {
                int Count = 0;
                List<ConfigurationAttsViewModel> ConfigurationAtts = new List<ConfigurationAttsViewModel>();

                if (ConfigrationTables.TLIdiversityType.ToString() == TableName)
                {
                    ConfigurationAtts = _unitOfWork.DiversityTypeRepository.GetWhere(x => !x.Deleted && x.Id > 0)
                        .Select(x => new ConfigurationAttsViewModel(x.Id, x.Name, TableName, x.Disable)).ToList();
                }
                else if (ConfigrationTables.TLIdataType.ToString() == TableName)
                {
                    ConfigurationAtts = _unitOfWork.DataTypeRepository.GetWhere(x => !x.Deleted && x.Id > 0)
                        .Select(x => new ConfigurationAttsViewModel(x.Id, x.Name, TableName, x.Disable)).ToList();
                }
                else if (ConfigrationTables.TLIoperation.ToString() == TableName)
                {
                    ConfigurationAtts = _unitOfWork.OperationRepository.GetWhere(x => !x.Deleted && x.Id > 0)
                        .Select(x => new ConfigurationAttsViewModel(x.Id, x.Name, TableName, x.Disable)).ToList();
                }
                    
                else if (ConfigrationTables.TLIlogicalOperation.ToString() == TableName)
                {
                    ConfigurationAtts = _unitOfWork.LogicalOperationRepository.GetWhere(x => !x.Deleted && x.Id > 0)
                        .Select(x => new ConfigurationAttsViewModel(x.Id, x.Name, TableName, x.Disable)).ToList();
                }
                else if (ConfigrationTables.TLItelecomType.ToString() == TableName)
                {
                    ConfigurationAtts = _unitOfWork.TelecomTypeRepository.GetWhere(x => !x.Deleted && x.Id > 0)
                        .Select(x => new ConfigurationAttsViewModel(x.Id, x.Name, TableName, x.Disable)).ToList();
                }
                //else if (ConfigrationTables.TLIoduInstallationType.ToString() == TableName)
                //{
                //    ConfigurationAtts = _unitOfWork.OduInstallationTypeRepository.GetWhere(x => !x.Deleted && x.Id > 0)
                //        .Select(x => new ConfigurationAttsViewModel(x.Id, x.Name, TableName, x.Disable)).ToList();
                //}
                //else if (ConfigrationTables.TLIsideArmInstallationPlace.ToString() == TableName)
                //{
                //    ConfigurationAtts = _unitOfWork.SideArmInstallationPlaceRepository.GetWhere(x => !x.Deleted && x.Id > 0)
                //        .Select(x => new ConfigurationAttsViewModel(x.Id, x.Name, TableName, x.Disable)).ToList();
                //}
                else if (ConfigrationTables.TLIrepeaterType.ToString() == TableName)
                {
                    ConfigurationAtts = _unitOfWork.RepeaterTypeRepository.GetWhere(x => !x.Deleted && x.Id > 0)
                        .Select(x => new ConfigurationAttsViewModel(x.Id, x.Name, TableName, x.Disable)).ToList();
                }
                else if (ConfigrationTables.TLIpolarityOnLocation.ToString() == TableName)
                {
                    ConfigurationAtts = _unitOfWork.PolarityOnLocationRepository.GetWhere(x => !x.Deleted && x.Id > 0)
                        .Select(x => new ConfigurationAttsViewModel(x.Id, x.Name, TableName, x.Disable)).ToList();
                }
                else if (ConfigrationTables.TLIitemConnectTo.ToString() == TableName)
                {
                    ConfigurationAtts = _unitOfWork.ItemConnectToRepository.GetWhere(x => !x.Deleted && x.Id > 0)
                        .Select(x => new ConfigurationAttsViewModel(x.Id, x.Name, TableName, x.Disable)).ToList();
                }
                else if (ConfigrationTables.TLIsupportTypeDesigned.ToString() == TableName)
                {
                    ConfigurationAtts = _unitOfWork.SupportTypeDesignedRepository.GetWhere(x => !x.Deleted && x.Id > 0)
                        .Select(x => new ConfigurationAttsViewModel(x.Id, x.Name, TableName, x.Disable)).ToList();
                }
                else if (ConfigrationTables.TLIsupportTypeImplemented.ToString() == TableName)
                {
                    ConfigurationAtts = _unitOfWork.SupportTypeImplementedRepository.GetWhere(x => !x.Deleted && x.Id > 0)
                        .Select(x => new ConfigurationAttsViewModel(x.Id, x.Name, TableName, x.Disable)).ToList();
                }
                else if (ConfigrationTables.TLIstructureType.ToString() == TableName)
                {
                    ConfigurationAtts = _unitOfWork.StructureTypeRepository.GetWhere(x => !x.Deleted && x.Id > 0)
                        .Select(x => new ConfigurationAttsViewModel(x.Id, x.Name, TableName, x.Disable)).ToList();
                }
                else if (ConfigrationTables.TLIsectionsLegType.ToString() == TableName)
                {
                    ConfigurationAtts = _unitOfWork.SectionsLegTypeRepository.GetWhere(x => !x.Deleted && x.Id > 0)
                        .Select(x => new ConfigurationAttsViewModel(x.Id, x.Name, TableName, x.Disable)).ToList();
                }
                else if (ConfigrationTables.TLIlogisticalType.ToString() == TableName)
                {
                    ConfigurationAtts = _unitOfWork.logisticalTypeRepository.GetWhere(x => !x.Deleted && x.Id > 0)
                        .Select(x => new ConfigurationAttsViewModel(x.Id, x.Name, TableName, x.Disable)).ToList();
                }
                else if (ConfigrationTables.TLIbaseCivilWithLegsType.ToString() == TableName)
                {
                    ConfigurationAtts = _unitOfWork.BaseCivilWithLegsTypeRepository.GetWhere(x => !x.Deleted && x.Id > 0)
                        .Select(x => new ConfigurationAttsViewModel(x.Id, x.Name, TableName, x.Disable)).ToList();
                }
                else if (ConfigrationTables.TLIbaseGeneratorType.ToString() == TableName)
                {
                    ConfigurationAtts = _unitOfWork.BaseGeneratorTypeRepository.GetWhere(x => !x.Deleted && x.Id > 0)
                        .Select(x => new ConfigurationAttsViewModel(x.Id, x.Name, TableName, x.Disable)).ToList();
                }
                else if (ConfigrationTables.TLIinstallationCivilwithoutLegsType.ToString() == TableName)
                {
                    ConfigurationAtts = _unitOfWork.InstallationCivilwithoutLegsTypeRepository.GetWhere(x => !x.Deleted && x.Id > 0)
                        .Select(x => new ConfigurationAttsViewModel(x.Id, x.Name, TableName, x.Disable)).ToList();
                }
                else if (ConfigrationTables.TLIboardType.ToString() == TableName)
                {
                    ConfigurationAtts = _unitOfWork.BoardTypeRepository.GetWhere(x => !x.Deleted && x.Id > 0)
                        .Select(x => new ConfigurationAttsViewModel(x.Id, x.Name, TableName, x.Disable)).ToList();
                }
                else if (ConfigrationTables.TLIguyLineType.ToString() == TableName)
                {
                    ConfigurationAtts = _unitOfWork.GuyLineTypeRepository.GetWhere(x => !x.Deleted && x.Id > 0)
                        .Select(x => new ConfigurationAttsViewModel(x.Id, x.Name, TableName, x.Disable)).ToList();
                }
                else if (ConfigrationTables.TLIenforcmentCategory.ToString() == TableName)
                {
                    ConfigurationAtts = _unitOfWork.EnforcmentCategoryRepository.GetWhere(x => !x.Deleted && x.Id > 0)
                        .Select(x => new ConfigurationAttsViewModel(x.Id, x.Name, TableName, x.Disable)).ToList();
                }
                else if (ConfigrationTables.TLIdocumentType.ToString() == TableName)
                {
                    ConfigurationAtts = _unitOfWork.DocumentTypeRepository.GetSelect(x => new ConfigurationAttsViewModel(x.Id, x.Name, TableName, true)).ToList();
                }
                else if (ConfigrationTables.TLIpowerType.ToString() == TableName)
                {
                    ConfigurationAtts = _unitOfWork.PowerTypeRepository.GetWhere(x => !x.Delete && x.Id > 0)
                        .Select(x => new ConfigurationAttsViewModel(x.Id, x.Name, TableName, x.Disable)).ToList();
                }
                else if (ConfigrationTables.TLIsubType.ToString() == TableName)
                {
                    ConfigurationAtts = _unitOfWork.SubTypeRepository.GetWhere(x => !x.Delete && x.Id > 0)
                        .Select(x => new ConfigurationAttsViewModel(x.Id, x.Name, TableName, x.Disable)).ToList();
                }
                else if (ConfigrationTables.TLIcivilNonSteelType.ToString() == TableName)
                {
                    ConfigurationAtts = _unitOfWork.CivilNonSteelTypeRepository.GetWhere(x => !x.Deleted && x.Id > 0)
                        .Select(x => new ConfigurationAttsViewModel(x.Id, x.Name, TableName, x.Disable)).ToList();
                }
                else if (ConfigrationTables.TLIasType.ToString() == TableName)
                {
                    ConfigurationAtts = _unitOfWork.AsTypeRepository.GetWhere(x => !x.Delete && x.Id > 0)
                        .Select(x => new ConfigurationAttsViewModel(x.Id, x.Name, TableName, x.Disable)).ToList();
                }
                else if (ConfigrationTables.TLIpolarityType.ToString() == TableName)
                {
                    ConfigurationAtts = _unitOfWork.PolarityTypeRepository.GetWhere(x => !x.Delete && x.Id > 0)
                        .Select(x => new ConfigurationAttsViewModel(x.Id, x.Name, TableName, x.Disable)).ToList();
                }
                else if (ConfigrationTables.TLIparity.ToString() == TableName)
                {
                    ConfigurationAtts = _unitOfWork.ParityRepository.GetWhere(x => !x.Delete && x.Id > 0)
                        .Select(x => new ConfigurationAttsViewModel(x.Id, x.Name, TableName, x.Disable)).ToList();
                }
                else if (ConfigrationTables.TLIcabinetPowerType.ToString() == TableName)
                {
                    ConfigurationAtts = _unitOfWork.CabinetPowerTypeRepository.GetWhere(x => !x.Delete && x.Id > 0)
                        .Select(x => new ConfigurationAttsViewModel(x.Id, x.Name, TableName, x.Disable)).ToList();
                }
                else if (ConfigrationTables.TLIcapacity.ToString() == TableName)
                {
                    ConfigurationAtts = _unitOfWork.CapacityRepository.GetWhere(x => !x.Delete && x.Id > 0)
                        .Select(x => new ConfigurationAttsViewModel(x.Id, x.Name, TableName, x.Disable)).ToList();
                }
                else if (ConfigrationTables.TLIlocationType.ToString() == TableName)
                {
                    ConfigurationAtts = _unitOfWork.LocationTypeRepository.GetWhere(x => !x.Deleted && x.Id > 0)
                        .Select(x => new ConfigurationAttsViewModel(x.Id, x.Name, TableName, x.Disable)).ToList();
                }
                else if (ConfigrationTables.TLIbaseType.ToString() == TableName)
                {
                    ConfigurationAtts = _unitOfWork.BaseTypeRepository.GetWhere(x => !x.Deleted && x.Id > 0)
                        .Select(x => new ConfigurationAttsViewModel(x.Id, x.Name, TableName, x.Disable)).ToList();
                }
                else if (ConfigrationTables.TLIbaseGeneratorType.ToString() == TableName)
                {
                    ConfigurationAtts = _unitOfWork.BaseGeneratorTypeRepository.GetWhere(x => !x.Deleted && x.Id > 0)
                        .Select(x => new ConfigurationAttsViewModel(x.Id, x.Name, TableName, x.Disable)).ToList();
                }
                else if (ConfigrationTables.TLIowner.ToString() == TableName)
                {
                    ConfigurationAtts = _unitOfWork.OwnerRepository.GetWhere(x => !x.Deleted && x.Id > 0)
                        .Select(x => new ConfigurationAttsViewModel(x.Id, x.OwnerName, TableName, x.Disable)).ToList();
                }
                else if (ConfigrationTables.TLIbaseBU.ToString() == TableName)
                {
                    ConfigurationAtts = _unitOfWork.BaseBURepository.GetWhere(x => !x.Deleted && x.Id > 0)
                        .Select(x => new ConfigurationAttsViewModel(x.Id, x.Name, TableName, x.Disable)).ToList();
                }
                else if (ConfigrationTables.TLIrenewableCabinetType.ToString() == TableName)
                {
                    ConfigurationAtts = _unitOfWork.RenewableCabinetTypeRepository.GetWhere(x => !x.Deleted && x.Id > 0)
                        .Select(x => new ConfigurationAttsViewModel(x.Id, x.Name, TableName, x.Disable)).ToList();
                }
                //else if (ConfigrationTables.TLIsideArmType.ToString() == TableName)
                //{
                //    ConfigurationAtts = _unitOfWork.SideArmTypeRepository.GetWhere(x => !x.Deleted && x.Id > 0)
                //        .Select(x => new ConfigurationAttsViewModel(x.Id, x.Name, TableName, x.Disable)).ToList();
                //}
                else if (ConfigrationTables.TLIitemStatus.ToString() == TableName)
                {
                    ConfigurationAtts = _unitOfWork.ItemStatusRepository.GetWhere(x => !x.Deleted && x.Id > 0)
                        .Select(x => new ConfigurationAttsViewModel(x.Id, x.Name, TableName, !x.Active)).ToList();
                }
                //else if (ConfigrationTables.TLIinstallationPlace.ToString() == TableName)
                //{
                //    ConfigurationAtts = _unitOfWork.InstallationPlaceRepository.GetWhere(x => !x.Deleted && x.Id > 0)
                //        .Select(x => new ConfigurationAttsViewModel(x.Id, x.Name, TableName, x.Disable)).ToList();
                //}
                Count = ConfigurationAtts.Count();

                ConfigurationAtts = ConfigurationAtts.Skip((Pagination.PageNumber - 1) * Pagination.PageSize).
                    Take(Pagination.PageSize).ToList();

                return new Response<List<ConfigurationAttsViewModel>>(true, ConfigurationAtts, null, null , (int)Helpers.Constants.ApiReturnCode.success,Count); 
            }
            catch(Exception err)
            {
                return new Response<List<ConfigurationAttsViewModel>>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
        }

        public Response<ConfigurationAttsViewModel> GetById(string TableName, int Id)
        {
            try
            {
                ConfigurationAttsViewModel ConfigurationAtts = new ConfigurationAttsViewModel();
                if (ConfigrationTables.TLIdiversityType.ToString() == TableName)
                {
                    var diversityType = _unitOfWork.DiversityTypeRepository.GetByID(Id);
                    ConfigurationAtts = _mapper.Map<ConfigurationAttsViewModel>(diversityType);
                }
                else if (ConfigrationTables.TLIdataType.ToString() == TableName)
                {
                    var dataType = _unitOfWork.DataTypeRepository.GetByID(Id);
                    ConfigurationAtts = _mapper.Map<ConfigurationAttsViewModel>(dataType);
                }
                else if (ConfigrationTables.TLIoperation.ToString() == TableName)
                {
                    var operation = _unitOfWork.OperationRepository.GetByID(Id);
                    ConfigurationAtts = _mapper.Map<ConfigurationAttsViewModel>(operation);
                }
                else if (ConfigrationTables.TLIlogicalOperation.ToString() == TableName)
                {
                    var logicalOperation = _unitOfWork.LogicalOperationRepository.GetByID(Id);
                    ConfigurationAtts = _mapper.Map<ConfigurationAttsViewModel>(logicalOperation);
                }
                else if (ConfigrationTables.TLItelecomType.ToString() == TableName)
                {
                    var telecomType = _unitOfWork.TelecomTypeRepository.GetByID(Id);
                    ConfigurationAtts = _mapper.Map<ConfigurationAttsViewModel>(telecomType);
                }
                else if (ConfigrationTables.TLIrepeaterType.ToString() == TableName)
                {
                    var repeaterType = _unitOfWork.RepeaterTypeRepository.GetByID(Id);
                    ConfigurationAtts = _mapper.Map<ConfigurationAttsViewModel>(repeaterType);
                }
                //else if (ConfigrationTables.TLIsideArmInstallationPlace.ToString() == TableName)
                //{
                //    var sideArmInstallationPlace = _unitOfWork.SideArmInstallationPlaceRepository.GetByID(Id);
                //    ConfigurationAtts = _mapper.Map<ConfigurationAttsViewModel>(sideArmInstallationPlace);
                //}
                else if (ConfigrationTables.TLIoduInstallationType.ToString() == TableName)
                {
                    var oduInstallationType = _unitOfWork.OduInstallationTypeRepository.GetByID(Id);
                    ConfigurationAtts = _mapper.Map<ConfigurationAttsViewModel>(oduInstallationType);
                }
                else if (ConfigrationTables.TLIitemConnectTo.ToString() == TableName)
                {
                    var itemConnectTo = _unitOfWork.ItemConnectToRepository.GetByID(Id);
                    ConfigurationAtts = _mapper.Map<ConfigurationAttsViewModel>(itemConnectTo);
                }
                else if (ConfigrationTables.TLIpolarityOnLocation.ToString() == TableName)
                {
                    var polarityOnLocation = _unitOfWork.PolarityOnLocationRepository.GetByID(Id);
                    ConfigurationAtts = _mapper.Map<ConfigurationAttsViewModel>(polarityOnLocation);
                }
                else if (ConfigrationTables.TLIsupportTypeDesigned.ToString() == TableName)
                {
                    var TLIsupportTypeDesigned = _unitOfWork.SupportTypeDesignedRepository.GetByID(Id);
                    ConfigurationAtts = _mapper.Map<ConfigurationAttsViewModel>(TLIsupportTypeDesigned);
                }
                else if (ConfigrationTables.TLIsupportTypeImplemented.ToString() == TableName)
                {
                    var TLIsupportTypeImplemented = _unitOfWork.SupportTypeImplementedRepository.GetByID(Id);
                    ConfigurationAtts = _mapper.Map<ConfigurationAttsViewModel>(TLIsupportTypeImplemented);
                }
                else if (ConfigrationTables.TLIstructureType.ToString() == TableName)
                {
                    var TLIstructureType = _unitOfWork.StructureTypeRepository.GetByID(Id);
                    ConfigurationAtts = _mapper.Map<ConfigurationAttsViewModel>(TLIstructureType);
                }
                else if (ConfigrationTables.TLIsectionsLegType.ToString() == TableName)
                {
                    var TLIsectionsLegType = _unitOfWork.SectionsLegTypeRepository.GetByID(Id);
                    ConfigurationAtts = _mapper.Map<ConfigurationAttsViewModel>(TLIsectionsLegType);
                }
                else if (ConfigrationTables.TLIlogisticalType.ToString() == TableName)
                {
                    var TLIlogisticalType = _unitOfWork.logisticalTypeRepository.GetByID(Id);
                    ConfigurationAtts = _mapper.Map<ConfigurationAttsViewModel>(TLIlogisticalType);
                }
                else if (ConfigrationTables.TLIbaseCivilWithLegsType.ToString() == TableName)
                {
                    var TLIbaseCivilWithLegsType = _unitOfWork.BaseCivilWithLegsTypeRepository.GetByID(Id);
                    ConfigurationAtts = _mapper.Map<ConfigurationAttsViewModel>(TLIbaseCivilWithLegsType);
                }
                else if (ConfigrationTables.TLIbaseGeneratorType.ToString() == TableName)
                {
                    var TLIbaseGeneratorType = _unitOfWork.BaseGeneratorTypeRepository.GetByID(Id);
                    ConfigurationAtts = _mapper.Map<ConfigurationAttsViewModel>(TLIbaseGeneratorType);
                }
                else if (ConfigrationTables.TLIinstallationCivilwithoutLegsType.ToString() == TableName)
                {
                    var TLIinstallationCivilwithoutLegsType = _unitOfWork.InstallationCivilwithoutLegsTypeRepository.GetByID(Id);
                    ConfigurationAtts = _mapper.Map<ConfigurationAttsViewModel>(TLIinstallationCivilwithoutLegsType);
                }
                else if (ConfigrationTables.TLIboardType.ToString() == TableName)
                {
                    var TLIboardType = _unitOfWork.BoardTypeRepository.GetByID(Id);
                    ConfigurationAtts = _mapper.Map<ConfigurationAttsViewModel>(TLIboardType);
                }
                else if (ConfigrationTables.TLIguyLineType.ToString() == TableName)
                {
                    var TLIguyLineType = _unitOfWork.GuyLineTypeRepository.GetByID(Id);
                    ConfigurationAtts = _mapper.Map<ConfigurationAttsViewModel>(TLIguyLineType);
                }
                else if (ConfigrationTables.TLIenforcmentCategory.ToString() == TableName)
                {
                    var TLIenforcmentCategory = _unitOfWork.EnforcmentCategoryRepository.GetByID(Id);
                    ConfigurationAtts = _mapper.Map<ConfigurationAttsViewModel>(TLIenforcmentCategory);
                }
                else if (ConfigrationTables.TLIpowerType.ToString() == TableName)
                {
                    var TLIpowerType = _unitOfWork.PowerTypeRepository.GetByID(Id);
                    ConfigurationAtts = _mapper.Map<ConfigurationAttsViewModel>(TLIpowerType);
                }
                else if (ConfigrationTables.TLIdocumentType.ToString() == TableName)
                {
                    var TLIdocumentType = _unitOfWork.DocumentTypeRepository.GetByID(Id);
                    ConfigurationAtts = _mapper.Map<ConfigurationAttsViewModel>(TLIdocumentType);
                }
                else if (ConfigrationTables.TLIcivilNonSteelType.ToString() == TableName)
                {
                    var CivilNonSteelType = _unitOfWork.CivilNonSteelTypeRepository.GetByID(Id);
                    ConfigurationAtts = _mapper.Map<ConfigurationAttsViewModel>(CivilNonSteelType);
                }
                return new Response<ConfigurationAttsViewModel>(true, ConfigurationAtts, null, null, (int)Helpers.Constants.ApiReturnCode.success);
            }
            catch(Exception err)
            {
                return new Response<ConfigurationAttsViewModel>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }  
        }

        public async Task<Response<ConfigurationAttsViewModel>> Update(ConfigurationAttsViewModel viewModel)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(viewModel.Name) || string.IsNullOrEmpty(viewModel.Name))
                    return new Response<ConfigurationAttsViewModel>(true, null, null, $"You Can't Add Empty Name", (int)Helpers.Constants.ApiReturnCode.fail);

                String TableName = viewModel.TableName;
                object model = (object)viewModel;
                if (ConfigrationTables.TLIdiversityType.ToString() == TableName)
                {
                    TLIdiversityType CheckName = _unitOfWork.DiversityTypeRepository
                        .GetWhereFirst(x => x.Name.ToLower() == viewModel.Name.ToLower() && x.Id != viewModel.Id && !x.Deleted);

                    if (CheckName == null)
                    {
                        TLIdiversityType OldEntity = _unitOfWork.DiversityTypeRepository
                            .GetWhereFirst(x => x.Id == viewModel.Id);

                        OldEntity.Name = viewModel.Name;

                        _unitOfWork.DiversityTypeRepository.Update(OldEntity);
                        await _unitOfWork.SaveChangesAsync();
                    }
                    else
                    {
                        return new Response<ConfigurationAttsViewModel>(true, null, null, $"The Diversity Type Name {viewModel.Name} is already exists", (int)Helpers.Constants.ApiReturnCode.fail);
                    }
                }
                else if (ConfigrationTables.TLIdataType.ToString() == TableName)
                {
                    TLIdataType CheckName = _unitOfWork.DataTypeRepository
                        .GetWhereFirst(x => x.Name.ToLower() == viewModel.Name.ToLower() && x.Id != viewModel.Id && !x.Deleted);

                    if (CheckName == null)
                    {
                        TLIdataType OldEntity = _unitOfWork.DataTypeRepository
                            .GetWhereFirst(x => x.Id == viewModel.Id);

                        OldEntity.Name = viewModel.Name;

                        _unitOfWork.DataTypeRepository.Update(OldEntity);
                        await _unitOfWork.SaveChangesAsync();
                    }
                    else
                    {
                        return new Response<ConfigurationAttsViewModel>(true, null, null, $"The Data Type Name {viewModel.Name} is already exists", (int)Helpers.Constants.ApiReturnCode.fail);
                    }
                }
                else if (ConfigrationTables.TLIoperation.ToString() == TableName)
                {
                    TLIoperation CheckName = _unitOfWork.OperationRepository
                        .GetWhereFirst(x => x.Name.ToLower() == viewModel.Name.ToLower() && x.Id != viewModel.Id && !x.Deleted);

                    if (CheckName == null)
                    {
                        TLIoperation OldEntity = _unitOfWork.OperationRepository
                            .GetWhereFirst(x => x.Id == viewModel.Id);

                        OldEntity.Name = viewModel.Name;

                        _unitOfWork.OperationRepository.Update(OldEntity);
                        await _unitOfWork.SaveChangesAsync();
                    }
                    else
                    {
                        return new Response<ConfigurationAttsViewModel>(true, null, null, $"The Operation Name {viewModel.Name} is already exists", (int)Helpers.Constants.ApiReturnCode.fail);
                    }
                }
                else if (ConfigrationTables.TLIlogicalOperation.ToString() == TableName)
                {
                    TLIlogicalOperation CheckName = _unitOfWork.LogicalOperationRepository
                        .GetWhereFirst(x => x.Name.ToLower() == viewModel.Name.ToLower() && x.Id != viewModel.Id && !x.Deleted);

                    if (CheckName == null)
                    {
                        TLIlogicalOperation OldEntity = _unitOfWork.LogicalOperationRepository
                            .GetWhereFirst(x => x.Id == viewModel.Id);

                        OldEntity.Name = viewModel.Name;

                        _unitOfWork.LogicalOperationRepository.Update(OldEntity);
                        await _unitOfWork.SaveChangesAsync();
                    }
                    else
                    {
                        return new Response<ConfigurationAttsViewModel>(true, null, null, $"The Logical Operation Name {viewModel.Name} is already exists", (int)Helpers.Constants.ApiReturnCode.fail);
                    }
                }
                else if (ConfigrationTables.TLItelecomType.ToString() == TableName)
                {
                    TLItelecomType CheckName = _unitOfWork.TelecomTypeRepository
                        .GetWhereFirst(x => x.Name.ToLower() == viewModel.Name.ToLower() && x.Id != viewModel.Id && !x.Deleted);

                    if (CheckName == null)
                    {
                        TLItelecomType OldEntity = _unitOfWork.TelecomTypeRepository
                            .GetWhereFirst(x => x.Id == viewModel.Id);

                        OldEntity.Name = viewModel.Name;

                        _unitOfWork.TelecomTypeRepository.Update(OldEntity);
                        await _unitOfWork.SaveChangesAsync();
                    }
                    else
                    {
                        return new Response<ConfigurationAttsViewModel>(true, null, null, $"The Telecom Type Name {viewModel.Name} is already exists", (int)Helpers.Constants.ApiReturnCode.fail);
                    }
                }
                else if (ConfigrationTables.TLIoduInstallationType.ToString() == TableName)
                {
                    TLIoduInstallationType CheckName = _unitOfWork.OduInstallationTypeRepository
                        .GetWhereFirst(x => x.Name.ToLower() == viewModel.Name.ToLower() && x.Id != viewModel.Id && !x.Deleted);

                    if (CheckName == null)
                    {
                        TLIoduInstallationType OldEntity = _unitOfWork.OduInstallationTypeRepository
                            .GetWhereFirst(x => x.Id == viewModel.Id);

                        OldEntity.Name = viewModel.Name;

                        _unitOfWork.OduInstallationTypeRepository.Update(OldEntity);
                        await _unitOfWork.SaveChangesAsync();
                    }
                    else
                    {
                        return new Response<ConfigurationAttsViewModel>(true, null, null, $"The ODU Installation Type Name {viewModel.Name} is already exists", (int)Helpers.Constants.ApiReturnCode.fail);
                    }
                }
                else if (ConfigrationTables.TLIsideArmInstallationPlace.ToString() == TableName)
                {
                    TLIsideArmInstallationPlace CheckName = _unitOfWork.SideArmInstallationPlaceRepository
                        .GetWhereFirst(x => x.Name.ToLower() == viewModel.Name.ToLower() && x.Id != viewModel.Id && !x.Deleted);

                    if (CheckName == null)
                    {
                        TLIsideArmInstallationPlace OldEntity = _unitOfWork.SideArmInstallationPlaceRepository
                            .GetWhereFirst(x => x.Id == viewModel.Id);

                        OldEntity.Name = viewModel.Name;

                        _unitOfWork.SideArmInstallationPlaceRepository.Update(OldEntity);
                        await _unitOfWork.SaveChangesAsync();
                    }
                    else
                    {
                        return new Response<ConfigurationAttsViewModel>(true, null, null, $"The Side Arm Installation Place Name {viewModel.Name} is already exists", (int)Helpers.Constants.ApiReturnCode.fail);
                    }
                }
                else if (ConfigrationTables.TLIitemConnectTo.ToString() == TableName)
                {
                    TLIitemConnectTo CheckName = _unitOfWork.ItemConnectToRepository
                        .GetWhereFirst(x => x.Name.ToLower() == viewModel.Name.ToLower() && x.Id != viewModel.Id && !x.Deleted);

                    if (CheckName == null)
                    {
                        TLIitemConnectTo OldEntity = _unitOfWork.ItemConnectToRepository
                            .GetWhereFirst(x => x.Id == viewModel.Id);

                        OldEntity.Name = viewModel.Name;

                        _unitOfWork.ItemConnectToRepository.Update(OldEntity);
                        await _unitOfWork.SaveChangesAsync();
                    }
                    else
                    {
                        return new Response<ConfigurationAttsViewModel>(true, null, null, $"The Item Connect To Name {viewModel.Name} is already exists", (int)Helpers.Constants.ApiReturnCode.fail);
                    }
                }
                else if (ConfigrationTables.TLIrepeaterType.ToString() == TableName)
                {
                    TLIrepeaterType CheckName = _unitOfWork.RepeaterTypeRepository
                        .GetWhereFirst(x => x.Name.ToLower() == viewModel.Name.ToLower() && x.Id != viewModel.Id && !x.Deleted);

                    if (CheckName == null)
                    {
                        TLIrepeaterType OldEntity = _unitOfWork.RepeaterTypeRepository
                            .GetWhereFirst(x => x.Id == viewModel.Id);

                        OldEntity.Name = viewModel.Name;

                        _unitOfWork.RepeaterTypeRepository.Update(OldEntity);
                        await _unitOfWork.SaveChangesAsync();
                    }
                    else
                    {
                        return new Response<ConfigurationAttsViewModel>(true, null, null, $"The Repeater Type Name {viewModel.Name} is already exists", (int)Helpers.Constants.ApiReturnCode.fail);
                    }
                }
                else if (ConfigrationTables.TLIsupportTypeDesigned.ToString() == TableName)
                {
                    TLIsupportTypeDesigned CheckName = _unitOfWork.SupportTypeDesignedRepository
                        .GetWhereFirst(x => x.Name.ToLower() == viewModel.Name.ToLower() && x.Id != viewModel.Id && !x.Deleted);

                    if (CheckName == null)
                    {
                        TLIsupportTypeDesigned OldEntity = _unitOfWork.SupportTypeDesignedRepository
                            .GetWhereFirst(x => x.Id == viewModel.Id);

                        OldEntity.Name = viewModel.Name;

                        _unitOfWork.SupportTypeDesignedRepository.Update(OldEntity);
                        await _unitOfWork.SaveChangesAsync();
                    }
                    else
                    {
                        return new Response<ConfigurationAttsViewModel>(true, null, null, $"The Support Type Designed Name {viewModel.Name} is already exists", (int)Helpers.Constants.ApiReturnCode.fail);
                    }
                }
                else if (ConfigrationTables.TLIsupportTypeImplemented.ToString() == TableName)
                {
                    TLIsupportTypeImplemented CheckName = _unitOfWork.SupportTypeImplementedRepository
                        .GetWhereFirst(x => x.Name.ToLower() == viewModel.Name.ToLower() && x.Id != viewModel.Id && !x.Deleted);

                    if (CheckName == null)
                    {
                        TLIsupportTypeImplemented OldEntity = _unitOfWork.SupportTypeImplementedRepository
                            .GetWhereFirst(x => x.Id == viewModel.Id);

                        OldEntity.Name = viewModel.Name;

                        _unitOfWork.SupportTypeImplementedRepository.Update(OldEntity);
                        await _unitOfWork.SaveChangesAsync();
                    }
                    else
                    {
                        return new Response<ConfigurationAttsViewModel>(true, null, null, $"The Support Type Implemented Name {viewModel.Name} is already exists", (int)Helpers.Constants.ApiReturnCode.fail);
                    }
                }
                else if (ConfigrationTables.TLIstructureType.ToString() == TableName)
                {
                    TLIstructureType CheckName = _unitOfWork.StructureTypeRepository
                        .GetWhereFirst(x => x.Name.ToLower() == viewModel.Name.ToLower() && x.Id != viewModel.Id && !x.Deleted);

                    if (CheckName == null)
                    {
                        TLIstructureType OldEntity = _unitOfWork.StructureTypeRepository
                            .GetWhereFirst(x => x.Id == viewModel.Id);

                        OldEntity.Name = viewModel.Name;

                        _unitOfWork.StructureTypeRepository.Update(OldEntity);
                        await _unitOfWork.SaveChangesAsync();
                    }
                    else
                    {
                        return new Response<ConfigurationAttsViewModel>(true, null, null, $"The Structure Type Name {viewModel.Name} is already exists", (int)Helpers.Constants.ApiReturnCode.fail);
                    }
                }
                else if (ConfigrationTables.TLIpolarityOnLocation.ToString() == TableName)
                {
                    TLIpolarityOnLocation CheckName = _unitOfWork.PolarityOnLocationRepository
                        .GetWhereFirst(x => x.Name.ToLower() == viewModel.Name.ToLower() && x.Id != viewModel.Id && !x.Deleted);

                    if (CheckName == null)
                    {
                        TLIpolarityOnLocation OldEntity = _unitOfWork.PolarityOnLocationRepository
                            .GetWhereFirst(x => x.Id == viewModel.Id);

                        OldEntity.Name = viewModel.Name;

                        _unitOfWork.PolarityOnLocationRepository.Update(OldEntity);
                        await _unitOfWork.SaveChangesAsync();
                    }
                    else
                    {
                        return new Response<ConfigurationAttsViewModel>(true, null, null, $"The Polarity On Location Name {viewModel.Name} is already exists", (int)Helpers.Constants.ApiReturnCode.fail);
                    }
                }
                else if (ConfigrationTables.TLIsectionsLegType.ToString() == TableName)
                {
                    TLIsectionsLegType CheckName = _unitOfWork.SectionsLegTypeRepository
                        .GetWhereFirst(x => x.Name.ToLower() == viewModel.Name.ToLower() && x.Id != viewModel.Id && !x.Deleted);

                    if (CheckName == null)
                    {
                        TLIsectionsLegType OldEntity = _unitOfWork.SectionsLegTypeRepository
                            .GetWhereFirst(x => x.Id == viewModel.Id);

                        OldEntity.Name = viewModel.Name;

                        _unitOfWork.SectionsLegTypeRepository.Update(OldEntity);
                        await _unitOfWork.SaveChangesAsync();
                    }
                    else
                    {
                        return new Response<ConfigurationAttsViewModel>(true, null, null, $"The Sections Leg Type Name {viewModel.Name} is already exists", (int)Helpers.Constants.ApiReturnCode.fail);
                    }
                }
                else if (ConfigrationTables.TLIlogisticalType.ToString() == TableName)
                {
                    TLIlogisticalType CheckName = _unitOfWork.logisticalTypeRepository
                        .GetWhereFirst(x => x.Name.ToLower() == viewModel.Name.ToLower() && x.Id != viewModel.Id && !x.Deleted);

                    if (CheckName == null)
                    {
                        TLIlogisticalType OldEntity = _unitOfWork.logisticalTypeRepository
                            .GetWhereFirst(x => x.Id == viewModel.Id);

                        OldEntity.Name = viewModel.Name;

                        _unitOfWork.logisticalTypeRepository.Update(OldEntity);
                        await _unitOfWork.SaveChangesAsync();
                    }
                    else
                    {
                        return new Response<ConfigurationAttsViewModel>(true, null, null, $"The logistical Type Name {viewModel.Name} is already exists", (int)Helpers.Constants.ApiReturnCode.fail);
                    }
                }
                else if (ConfigrationTables.TLIbaseCivilWithLegsType.ToString() == TableName)
                {
                    TLIbaseCivilWithLegsType CheckName = _unitOfWork.BaseCivilWithLegsTypeRepository
                        .GetWhereFirst(x => x.Name.ToLower() == viewModel.Name.ToLower() && x.Id != viewModel.Id && !x.Deleted);

                    if (CheckName == null)
                    {
                        TLIbaseCivilWithLegsType OldEntity = _unitOfWork.BaseCivilWithLegsTypeRepository
                            .GetWhereFirst(x => x.Id == viewModel.Id);

                        OldEntity.Name = viewModel.Name;

                        _unitOfWork.BaseCivilWithLegsTypeRepository.Update(OldEntity);
                        await _unitOfWork.SaveChangesAsync();
                    }
                    else
                    {
                        return new Response<ConfigurationAttsViewModel>(true, null, null, $"The Base Civil With Legs Type Name {viewModel.Name} is already exists", (int)Helpers.Constants.ApiReturnCode.fail);
                    }
                }
                else if (ConfigrationTables.TLIbaseGeneratorType.ToString() == TableName)
                {
                    TLIbaseGeneratorType CheckName = _unitOfWork.BaseGeneratorTypeRepository
                        .GetWhereFirst(x => x.Name.ToLower() == viewModel.Name.ToLower() && x.Id != viewModel.Id && !x.Deleted);

                    if (CheckName == null)
                    {
                        TLIbaseGeneratorType OldEntity = _unitOfWork.BaseGeneratorTypeRepository
                            .GetWhereFirst(x => x.Id == viewModel.Id);

                        OldEntity.Name = viewModel.Name;

                        _unitOfWork.BaseGeneratorTypeRepository.Update(OldEntity);
                        await _unitOfWork.SaveChangesAsync();
                    }
                    else
                    {
                        return new Response<ConfigurationAttsViewModel>(true, null, null, $"The Base Generator Type Name {viewModel.Name} is already exists", (int)Helpers.Constants.ApiReturnCode.fail);
                    }
                }
                else if (ConfigrationTables.TLIinstallationCivilwithoutLegsType.ToString() == TableName)
                {
                    TLIinstallationCivilwithoutLegsType CheckName = _unitOfWork.InstallationCivilwithoutLegsTypeRepository
                        .GetWhereFirst(x => x.Name.ToLower() == viewModel.Name.ToLower() && x.Id != viewModel.Id && !x.Deleted);

                    if (CheckName == null)
                    {
                        TLIinstallationCivilwithoutLegsType OldEntity = _unitOfWork.InstallationCivilwithoutLegsTypeRepository
                            .GetWhereFirst(x => x.Id == viewModel.Id);

                        OldEntity.Name = viewModel.Name;

                        _unitOfWork.InstallationCivilwithoutLegsTypeRepository.Update(OldEntity);
                        await _unitOfWork.SaveChangesAsync();
                    }
                    else
                    {
                        return new Response<ConfigurationAttsViewModel>(true, null, null, $"The Installation Civil without Legs Type Name {viewModel.Name} is already exists", (int)Helpers.Constants.ApiReturnCode.fail);
                    }
                }
                else if (ConfigrationTables.TLIboardType.ToString() == TableName)
                {
                    TLIboardType CheckName = _unitOfWork.BoardTypeRepository
                        .GetWhereFirst(x => x.Name.ToLower() == viewModel.Name.ToLower() && x.Id != viewModel.Id && !x.Deleted);

                    if (CheckName == null)
                    {
                        TLIboardType OldEntity = _unitOfWork.BoardTypeRepository
                            .GetWhereFirst(x => x.Id == viewModel.Id);

                        OldEntity.Name = viewModel.Name;

                        _unitOfWork.BoardTypeRepository.Update(OldEntity);
                        await _unitOfWork.SaveChangesAsync();
                    }
                    else
                    {
                        return new Response<ConfigurationAttsViewModel>(true, null, null, $"The Board Type Name {viewModel.Name} is already exists", (int)Helpers.Constants.ApiReturnCode.fail);
                    }
                }
                else if (ConfigrationTables.TLIguyLineType.ToString() == TableName)
                {
                    TLIguyLineType CheckName = _unitOfWork.GuyLineTypeRepository
                        .GetWhereFirst(x => x.Name.ToLower() == viewModel.Name.ToLower() && x.Id != viewModel.Id && !x.Deleted);

                    if (CheckName == null)
                    {
                        TLIguyLineType OldEntity = _unitOfWork.GuyLineTypeRepository
                            .GetWhereFirst(x => x.Id == viewModel.Id);

                        OldEntity.Name = viewModel.Name;

                        _unitOfWork.GuyLineTypeRepository.Update(OldEntity);
                        await _unitOfWork.SaveChangesAsync();
                    }
                    else
                    {
                        return new Response<ConfigurationAttsViewModel>(true, null, null, $"The Guy Line Type Name {viewModel.Name} is already exists", (int)Helpers.Constants.ApiReturnCode.fail);
                    }
                }
                else if (ConfigrationTables.TLIenforcmentCategory.ToString() == TableName)
                {
                    TLIenforcmentCategory CheckName = _unitOfWork.EnforcmentCategoryRepository
                        .GetWhereFirst(x => x.Name.ToLower() == viewModel.Name.ToLower() && x.Id != viewModel.Id && !x.Deleted);

                    if (CheckName == null)
                    {
                        TLIenforcmentCategory OldEntity = _unitOfWork.EnforcmentCategoryRepository
                            .GetWhereFirst(x => x.Id == viewModel.Id);

                        OldEntity.Name = viewModel.Name;

                        _unitOfWork.EnforcmentCategoryRepository.Update(OldEntity);
                        await _unitOfWork.SaveChangesAsync();
                    }
                    else
                    {
                        return new Response<ConfigurationAttsViewModel>(true, null, null, $"The Enforcment Category Name {viewModel.Name} is already exists", (int)Helpers.Constants.ApiReturnCode.fail);
                    }
                }
                else if (ConfigrationTables.TLIpowerType.ToString() == TableName)
                {
                    TLIpowerType CheckName = _unitOfWork.PowerTypeRepository
                        .GetWhereFirst(x => x.Name.ToLower() == viewModel.Name.ToLower() && x.Id != viewModel.Id && !x.Delete);

                    if (CheckName == null)
                    {
                        TLIpowerType OldEntity = _unitOfWork.PowerTypeRepository
                            .GetWhereFirst(x => x.Id == viewModel.Id);

                        OldEntity.Name = viewModel.Name;

                        _unitOfWork.PowerTypeRepository.Update(OldEntity);
                        await _unitOfWork.SaveChangesAsync();
                    }
                    else
                    {
                        return new Response<ConfigurationAttsViewModel>(true, null, null, $"The Power Type Name {viewModel.Name} is already exists", (int)Helpers.Constants.ApiReturnCode.fail);
                    }
                }

                //-------------------------------------------------------------
                else if (ConfigrationTables.TLIsubType.ToString() == TableName)
                {
                    TLIsubType CheckName = _unitOfWork.SubTypeRepository
                        .GetWhereFirst(x => x.Name.ToLower() == viewModel.Name.ToLower() && x.Id != viewModel.Id && !x.Delete);

                    if (CheckName == null)
                    {
                        TLIsubType OldEntity = _unitOfWork.SubTypeRepository
                            .GetWhereFirst(x => x.Id == viewModel.Id);

                        OldEntity.Name = viewModel.Name;

                        _unitOfWork.SubTypeRepository.Update(OldEntity);
                        await _unitOfWork.SaveChangesAsync();
                    }
                    else
                    {
                        return new Response<ConfigurationAttsViewModel>(true, null, null, $"The Sub Type Name {viewModel.Name} is already exists", (int)Helpers.Constants.ApiReturnCode.fail);
                    }
                }

                else if (ConfigrationTables.TLIasType.ToString() == TableName)
                {
                    TLIasType CheckName = _unitOfWork.AsTypeRepository
                        .GetWhereFirst(x => x.Name.ToLower() == viewModel.Name.ToLower() && x.Id != viewModel.Id && !x.Delete);

                    if (CheckName == null)
                    {
                        TLIasType OldEntity = _unitOfWork.AsTypeRepository
                            .GetWhereFirst(x => x.Id == viewModel.Id);

                        OldEntity.Name = viewModel.Name;

                        _unitOfWork.AsTypeRepository.Update(OldEntity);
                        await _unitOfWork.SaveChangesAsync();
                    }
                    else
                    {
                        return new Response<ConfigurationAttsViewModel>(true, null, null, $"The As Type Name {viewModel.Name} is already exists", (int)Helpers.Constants.ApiReturnCode.fail);
                    }
                }

                else if (ConfigrationTables.TLIpolarityType.ToString() == TableName)
                {
                    TLIpolarityType CheckName = _unitOfWork.PolarityTypeRepository
                        .GetWhereFirst(x => x.Name.ToLower() == viewModel.Name.ToLower() && x.Id != viewModel.Id && !x.Delete);

                    if (CheckName == null)
                    {
                        TLIpolarityType OldEntity = _unitOfWork.PolarityTypeRepository
                            .GetWhereFirst(x => x.Id == viewModel.Id);

                        OldEntity.Name = viewModel.Name;

                        _unitOfWork.PolarityTypeRepository.Update(OldEntity);
                        await _unitOfWork.SaveChangesAsync();
                    }
                    else
                    {
                        return new Response<ConfigurationAttsViewModel>(true, null, null, $"The Polarity Type Name {viewModel.Name} is already exists", (int)Helpers.Constants.ApiReturnCode.fail);
                    }
                }

                else if (ConfigrationTables.TLIparity.ToString() == TableName)
                {
                    TLIparity CheckName = _unitOfWork.ParityRepository
                        .GetWhereFirst(x => x.Name.ToLower() == viewModel.Name.ToLower() && x.Id != viewModel.Id && !x.Delete);

                    if (CheckName == null)
                    {
                        TLIparity OldEntity = _unitOfWork.ParityRepository
                            .GetWhereFirst(x => x.Id == viewModel.Id);

                        OldEntity.Name = viewModel.Name;

                        _unitOfWork.ParityRepository.Update(OldEntity);
                        await _unitOfWork.SaveChangesAsync();
                    }
                    else
                    {
                        return new Response<ConfigurationAttsViewModel>(true, null, null, $"The Parity Name {viewModel.Name} is already exists", (int)Helpers.Constants.ApiReturnCode.fail);
                    }
                }

                else if (ConfigrationTables.TLIcabinetPowerType.ToString() == TableName)
                {
                    TLIcabinetPowerType CheckName = _unitOfWork.CabinetPowerTypeRepository
                        .GetWhereFirst(x => x.Name.ToLower() == viewModel.Name.ToLower() && x.Id != viewModel.Id && !x.Delete);

                    if (CheckName == null)
                    {
                        TLIcabinetPowerType OldEntity = _unitOfWork.CabinetPowerTypeRepository
                            .GetWhereFirst(x => x.Id == viewModel.Id);

                        OldEntity.Name = viewModel.Name;

                        _unitOfWork.CabinetPowerTypeRepository.Update(OldEntity);
                        await _unitOfWork.SaveChangesAsync();
                    }
                    else
                    {
                        return new Response<ConfigurationAttsViewModel>(true, null, null, $"The Cabinet Power Type Name {viewModel.Name} is already exists", (int)Helpers.Constants.ApiReturnCode.fail);
                    }
                }
                else if (ConfigrationTables.TLIcapacity.ToString() == TableName)
                {
                    TLIcapacity CheckName = _unitOfWork.CapacityRepository
                        .GetWhereFirst(x => x.Name.ToLower() == viewModel.Name.ToLower() && x.Id != viewModel.Id && !x.Delete);

                    if (CheckName == null)
                    {
                        TLIcapacity OldEntity = _unitOfWork.CapacityRepository
                            .GetWhereFirst(x => x.Id == viewModel.Id);

                        OldEntity.Name = viewModel.Name;

                        _unitOfWork.CapacityRepository.Update(OldEntity);
                        await _unitOfWork.SaveChangesAsync();
                    }
                    else
                    {
                        return new Response<ConfigurationAttsViewModel>(true, null, null, $"The Capacity Name {viewModel.Name} is already exists", (int)Helpers.Constants.ApiReturnCode.fail);
                    }
                }
                else if (ConfigrationTables.TLIpowerType.ToString() == TableName)
                {
                    TLIpowerType CheckName = _unitOfWork.PowerTypeRepository
                        .GetWhereFirst(x => x.Name.ToLower() == viewModel.Name.ToLower() && x.Id != viewModel.Id && !x.Delete);

                    if (CheckName == null)
                    {
                        TLIpowerType OldEntity = _unitOfWork.PowerTypeRepository
                            .GetWhereFirst(x => x.Id == viewModel.Id);

                        OldEntity.Name = viewModel.Name;

                        _unitOfWork.PowerTypeRepository.Update(OldEntity);
                        await _unitOfWork.SaveChangesAsync();
                    }
                    else
                    {
                        return new Response<ConfigurationAttsViewModel>(true, null, null, $"The Power Type Name {viewModel.Name} is already exists", (int)Helpers.Constants.ApiReturnCode.fail);
                    }
                }

                else if (ConfigrationTables.TLIcivilNonSteelType.ToString() == TableName)
                {
                    TLIcivilNonSteelType CheckName = _unitOfWork.CivilNonSteelTypeRepository
                        .GetWhereFirst(x => x.Name.ToLower() == viewModel.Name.ToLower() && x.Id != viewModel.Id && !x.Deleted);

                    if (CheckName == null)
                    {
                        TLIcivilNonSteelType OldEntity = _unitOfWork.CivilNonSteelTypeRepository
                            .GetWhereFirst(x => x.Id == viewModel.Id);

                        OldEntity.Name = viewModel.Name;

                        _unitOfWork.CivilNonSteelTypeRepository.Update(OldEntity);
                        await _unitOfWork.SaveChangesAsync();
                    }
                    else
                    {
                        return new Response<ConfigurationAttsViewModel>(true, null, null, $"The Civil Non Steel Type Name {viewModel.Name} is already exists", (int)Helpers.Constants.ApiReturnCode.fail);
                    }
                }
                
                else if (ConfigrationTables.TLIowner.ToString() == TableName)
                {
                    TLIowner CheckName = _unitOfWork.OwnerRepository
                        .GetWhereFirst(x => x.OwnerName.ToLower() == viewModel.Name.ToLower() && x.Id != viewModel.Id && !x.Deleted);

                    if (CheckName == null)
                    {
                        TLIowner OldEntity = _unitOfWork.OwnerRepository
                            .GetWhereFirst(x => x.Id == viewModel.Id);

                        OldEntity.OwnerName = viewModel.Name;

                        _unitOfWork.OwnerRepository.Update(OldEntity);
                        await _unitOfWork.SaveChangesAsync();
                    }
                    else
                    {
                        return new Response<ConfigurationAttsViewModel>(true, null, null, $"The Owner Name {viewModel.Name} is already exists", (int)Helpers.Constants.ApiReturnCode.fail);
                    }
                }
                else if (ConfigrationTables.TLIlocationType.ToString() == TableName)
                {
                    TLIlocationType CheckName = _unitOfWork.LocationTypeRepository
                        .GetWhereFirst(x => x.Name.ToLower() == viewModel.Name.ToLower() && x.Id != viewModel.Id && !x.Deleted);

                    if (CheckName == null)
                    {
                        TLIlocationType OldEntity = _unitOfWork.LocationTypeRepository
                            .GetWhereFirst(x => x.Id == viewModel.Id);

                        OldEntity.Name = viewModel.Name;

                        _unitOfWork.LocationTypeRepository.Update(OldEntity);
                        await _unitOfWork.SaveChangesAsync();
                    }
                    else
                    {
                        return new Response<ConfigurationAttsViewModel>(true, null, null, $"The Location Type Name {viewModel.Name} is already exists", (int)Helpers.Constants.ApiReturnCode.fail);
                    }
                }
                else if (ConfigrationTables.TLIbaseType.ToString() == TableName)
                {
                    TLIbaseType CheckName = _unitOfWork.BaseTypeRepository
                        .GetWhereFirst(x => x.Name.ToLower() == viewModel.Name.ToLower() && x.Id != viewModel.Id && !x.Deleted);

                    if (CheckName == null)
                    {
                        TLIbaseType OldEntity = _unitOfWork.BaseTypeRepository
                            .GetWhereFirst(x => x.Id == viewModel.Id);

                        OldEntity.Name = viewModel.Name;

                        _unitOfWork.BaseTypeRepository.Update(OldEntity);
                        await _unitOfWork.SaveChangesAsync();
                    }
                    else
                    {
                        return new Response<ConfigurationAttsViewModel>(true, null, null, $"The Base Type Name {viewModel.Name} is already exists", (int)Helpers.Constants.ApiReturnCode.fail);
                    }
                }
                else if (ConfigrationTables.TLIbaseBU.ToString() == TableName)
                {
                    TLIbaseBU CheckName = _unitOfWork.BaseBURepository
                        .GetWhereFirst(x => x.Name.ToLower() == viewModel.Name.ToLower() && x.Id != viewModel.Id && !x.Deleted);

                    if (CheckName == null)
                    {
                        TLIbaseBU OldEntity = _unitOfWork.BaseBURepository
                            .GetWhereFirst(x => x.Id == viewModel.Id);

                        OldEntity.Name = viewModel.Name;

                        _unitOfWork.BaseBURepository.Update(OldEntity);
                        await _unitOfWork.SaveChangesAsync();
                    }
                    else
                    {
                        return new Response<ConfigurationAttsViewModel>(true, null, null, $"The Base BU Name {viewModel.Name} is already exists", (int)Helpers.Constants.ApiReturnCode.fail);
                    }
                }
                else if (ConfigrationTables.TLIrenewableCabinetType.ToString() == TableName)
                {
                    TLIrenewableCabinetType CheckName = _unitOfWork.RenewableCabinetTypeRepository
                        .GetWhereFirst(x => x.Name.ToLower() == viewModel.Name.ToLower() && x.Id != viewModel.Id && !x.Deleted);

                    if (CheckName == null)
                    {
                        TLIrenewableCabinetType OldEntity = _unitOfWork.RenewableCabinetTypeRepository
                            .GetWhereFirst(x => x.Id == viewModel.Id);

                        OldEntity.Name = viewModel.Name;

                        _unitOfWork.RenewableCabinetTypeRepository.Update(OldEntity);
                        await _unitOfWork.SaveChangesAsync();
                    }
                    else
                    {
                        return new Response<ConfigurationAttsViewModel>(true, null, null, $"The Renewable Cabinet Type Name {viewModel.Name} is already exists", (int)Helpers.Constants.ApiReturnCode.fail);
                    }
                }
                else if (ConfigrationTables.TLIsideArmType.ToString() == TableName)
                {
                    TLIsideArmType CheckName = _unitOfWork.SideArmTypeRepository
                        .GetWhereFirst(x => x.Name.ToLower() == viewModel.Name.ToLower() && x.Id != viewModel.Id && !x.Deleted);

                    if (CheckName == null)
                    {
                        TLIsideArmType OldEntity = _unitOfWork.SideArmTypeRepository
                            .GetWhereFirst(x => x.Id == viewModel.Id);

                        OldEntity.Name = viewModel.Name;

                        _unitOfWork.SideArmTypeRepository.Update(OldEntity);
                        await _unitOfWork.SaveChangesAsync();
                    }
                    else
                    {
                        return new Response<ConfigurationAttsViewModel>(true, null, null, $"The Side Arm Type Name {viewModel.Name} is already exists", (int)Helpers.Constants.ApiReturnCode.fail);
                    }
                }
                else if (ConfigrationTables.TLIitemStatus.ToString() == TableName)
                {
                    TLIitemStatus CheckName = _unitOfWork.ItemStatusRepository
                        .GetWhereFirst(x => x.Name.ToLower() == viewModel.Name.ToLower() && x.Id != viewModel.Id && !x.Deleted);

                    if (CheckName == null)
                    {
                        TLIitemStatus OldEntity = _unitOfWork.ItemStatusRepository
                            .GetWhereFirst(x => x.Id == viewModel.Id);

                        OldEntity.Name = viewModel.Name;

                        _unitOfWork.ItemStatusRepository.Update(OldEntity);
                        await _unitOfWork.SaveChangesAsync();
                    }
                    else
                    {
                        return new Response<ConfigurationAttsViewModel>(true, null, null, $"The Item Status Name {viewModel.Name} is already exists", (int)Helpers.Constants.ApiReturnCode.fail);
                    }
                }
                else if (ConfigrationTables.TLIinstallationPlace.ToString() == TableName)
                {
                    TLIinstallationPlace CheckName = _unitOfWork.InstallationPlaceRepository
                        .GetWhereFirst(x => x.Name.ToLower() == viewModel.Name.ToLower() && x.Id != viewModel.Id && !x.Deleted);

                    if (CheckName == null)
                    {
                        TLIinstallationPlace OldEntity = _unitOfWork.InstallationPlaceRepository
                            .GetWhereFirst(x => x.Id == viewModel.Id);

                        OldEntity.Name = viewModel.Name;

                        _unitOfWork.InstallationPlaceRepository.Update(OldEntity);
                        await _unitOfWork.SaveChangesAsync();
                    }
                    else
                    {
                        return new Response<ConfigurationAttsViewModel>(true, null, null, $"The Installation Place Name {viewModel.Name} is already exists", (int)Helpers.Constants.ApiReturnCode.fail);
                    }
                }
                //-------------------------------------------------------------

                else 
                {
                    string name = ((ConfigurationAttsViewModel)model).Name; 
                    var TLIdynamicListValue = _unitOfWork.DynamicListValuesRepository.GetByID(((ConfigurationAttsViewModel)model).Id.Value);
                    TLIdynamicListValue.Value = ((ConfigurationAttsViewModel)model).Name;
                    var validation = _unitOfWork.DynamicListValuesRepository.GetWhereFirst(x => x.Value == name && x.Id!= TLIdynamicListValue.Id);
                    if (validation == null)
                    {
                        _unitOfWork.DynamicListValuesRepository.Update(TLIdynamicListValue);
                    }
                    else
                    {
                        return new Response<ConfigurationAttsViewModel>(true, null, null, $"The Name is already exists", (int)Helpers.Constants.ApiReturnCode.fail);
                    }
                    _unitOfWork.DynamicListValuesRepository.Update(TLIdynamicListValue);
                    await _unitOfWork.SaveChangesAsync();
                }
                return new Response<ConfigurationAttsViewModel>();
            }
            catch (Exception err)
            {
                return new Response<ConfigurationAttsViewModel>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
        }
        private bool ValidateAdd(string TableName, object entity)
        {
            if (ConfigrationTables.TLIdiversityType.ToString() == TableName)
            {
                TLIdiversityType diversityTypeEntity = _mapper.Map<TLIdiversityType>(entity);

                TLIdiversityType diversityType = _unitOfWork.DiversityTypeRepository
                    .GetWhereFirst(x => x.Name.ToLower() == diversityTypeEntity.Name.ToLower() && !x.Deleted);

                if (diversityType != null)
                    return true;

                else
                    return false;
            }
            else if (ConfigrationTables.TLItelecomType.ToString() == TableName)
            {
                TLItelecomType telecomTypeEntity = _mapper.Map<TLItelecomType>(entity);

                TLItelecomType telecomType = _unitOfWork.TelecomTypeRepository
                    .GetWhereFirst(x => x.Name.ToLower() == telecomTypeEntity.Name.ToLower() && !x.Deleted);

                if (telecomType != null)
                    return true;

                else
                    return false;
            }
            else if (ConfigrationTables.TLIpolarityType.ToString() == TableName)
            {
                TLIpolarityType _mapperEntity = _mapper.Map<TLIpolarityType>(entity);

                TLIpolarityType Entity = _unitOfWork.PolarityTypeRepository
                    .GetWhereFirst(x => x.Name.ToLower() == _mapperEntity.Name.ToLower() && !x.Delete);

                if (Entity != null)
                    return true;

                else
                    return false;
            }
            else if (ConfigrationTables.TLIasType.ToString() == TableName)
            {
                TLIasType _mapperEntity = _mapper.Map<TLIasType>(entity);

                TLIasType Entity = _unitOfWork.AsTypeRepository
                    .GetWhereFirst(x => x.Name.ToLower() == _mapperEntity.Name.ToLower() && !x.Delete);

                if (Entity != null)
                    return true;

                else
                    return false;
            }
            else if (ConfigrationTables.TLIparity.ToString() == TableName)
            {
                TLIparity _mapperEntity = _mapper.Map<TLIparity>(entity);

                TLIparity Entity = _unitOfWork.ParityRepository
                    .GetWhereFirst(x => x.Name.ToLower() == _mapperEntity.Name.ToLower() && !x.Delete);

                if (Entity != null)
                    return true;

                else
                    return false;
            }
            else if (ConfigrationTables.TLIcabinetPowerType.ToString() == TableName)
            {
                TLIcabinetPowerType _mapperEntity = _mapper.Map<TLIcabinetPowerType>(entity);

                TLIcabinetPowerType Entity = _unitOfWork.CabinetPowerTypeRepository
                    .GetWhereFirst(x => x.Name.ToLower() == _mapperEntity.Name.ToLower() && !x.Delete);

                if (Entity != null)
                    return true;

                else
                    return false;
            }
            else if (ConfigrationTables.TLIcapacity.ToString() == TableName)
            {
                TLIcapacity _mapperEntity = _mapper.Map<TLIcapacity>(entity);

                TLIcapacity Entity = _unitOfWork.CapacityRepository
                    .GetWhereFirst(x => x.Name.ToLower() == _mapperEntity.Name.ToLower() && !x.Delete);

                if (Entity != null)
                    return true;

                else
                    return false;
            }
            else if (ConfigrationTables.TLIsubType.ToString() == TableName)
            {
                TLIsubType _mapperEntity = _mapper.Map<TLIsubType>(entity);

                TLIsubType Entity = _unitOfWork.SubTypeRepository
                    .GetWhereFirst(x => x.Name.ToLower() == _mapperEntity.Name.ToLower() && !x.Delete);

                if (Entity != null)
                    return true;

                else
                    return false;
            }

            else if (ConfigrationTables.TLIsideArmInstallationPlace.ToString() == TableName)
            {
                TLIsideArmInstallationPlace sideArmInstallationPlaceEntity = _mapper.Map<TLIsideArmInstallationPlace>(entity);
                
                TLIsideArmInstallationPlace sideArmInstallationPlace = _unitOfWork.SideArmInstallationPlaceRepository
                    .GetWhereFirst(x => x.Name.ToLower() == sideArmInstallationPlaceEntity.Name.ToLower() && !x.Deleted);

                if (sideArmInstallationPlace != null)
                    return true;

                else
                    return false;
            }
            else if (ConfigrationTables.TLIoduInstallationType.ToString() == TableName)
            {
                TLIoduInstallationType oduInstallationTypeEntity = _mapper.Map<TLIoduInstallationType>(entity);

                TLIoduInstallationType oduInstallationType = _unitOfWork.OduInstallationTypeRepository
                    .GetWhereFirst(x => x.Name.ToLower() == oduInstallationTypeEntity.Name.ToLower() && !x.Deleted);

                if (oduInstallationType != null)
                    return true;

                else
                    return false;
            }
            else if (ConfigrationTables.TLIrepeaterType.ToString() == TableName)
            {
                TLIrepeaterType repeaterTypeEntity = _mapper.Map<TLIrepeaterType>(entity);

                TLIrepeaterType repeaterType = _unitOfWork.RepeaterTypeRepository
                    .GetWhereFirst(x => x.Name.ToLower() == repeaterTypeEntity.Name.ToLower() && !x.Deleted);

                if (repeaterType != null)
                    return true;

                else
                    return false;

            }
            else if (ConfigrationTables.TLIitemConnectTo.ToString() == TableName)
            {
                TLIitemConnectTo itemConnctToEntity = _mapper.Map<TLIitemConnectTo>(entity);

                TLIitemConnectTo itemConnctTo = _unitOfWork.ItemConnectToRepository
                    .GetWhereFirst(x => x.Name.ToLower() == itemConnctToEntity.Name.ToLower() && !x.Deleted);

                if (itemConnctTo != null)
                    return true;

                else
                    return false;
            }
            else if (ConfigrationTables.TLIsupportTypeDesigned.ToString() == TableName)
            {
                TLIsupportTypeDesigned supportTypeDesignedEntity = _mapper.Map<TLIsupportTypeDesigned>(entity);

                TLIsupportTypeDesigned supportTypeDesigned = _unitOfWork.SupportTypeDesignedRepository
                    .GetWhereFirst(x => x.Name.ToLower() == supportTypeDesignedEntity.Name.ToLower() && !x.Deleted);

                if (supportTypeDesigned != null)
                    return true;

                else
                    return false;
            }
            else if (ConfigrationTables.TLIpolarityOnLocation.ToString() == TableName)
            {
                TLIpolarityOnLocation polarityOnLocationEntity = _mapper.Map<TLIpolarityOnLocation>(entity);

                TLIpolarityOnLocation polarityOnLocation = _unitOfWork.PolarityOnLocationRepository
                    .GetWhereFirst(x => x.Name.ToLower() == polarityOnLocationEntity.Name.ToLower() && !x.Deleted);

                if (polarityOnLocation != null)
                    return true;

                else
                    return false;

            }
            else if (ConfigrationTables.TLIsupportTypeImplemented.ToString() == TableName)
            {
                TLIsupportTypeImplemented supportTypeImplementedEntity = _mapper.Map<TLIsupportTypeImplemented>(entity);

                TLIsupportTypeImplemented supportTypeImplemented = _unitOfWork.SupportTypeImplementedRepository
                    .GetWhereFirst(x => x.Name.ToLower() == supportTypeImplementedEntity.Name.ToLower() && !x.Deleted);

                if (supportTypeImplemented != null)
                    return true;

                else
                    return false;
            }
            else if (ConfigrationTables.TLIstructureType.ToString() == TableName)
            {
                TLIstructureType structureTypeEntity = _mapper.Map<TLIstructureType>(entity);

                TLIstructureType structureType = _unitOfWork.StructureTypeRepository
                    .GetWhereFirst(x => x.Name.ToLower() == structureTypeEntity.Name.ToLower() && !x.Deleted);

                if (structureType != null)
                    return true;

                else
                    return false;
            }
            else if (ConfigrationTables.TLIsectionsLegType.ToString() == TableName)
            {
                TLIsectionsLegType sectionsLegTypeEntity = _mapper.Map<TLIsectionsLegType>(entity);

                TLIsectionsLegType sectionsLegType = _unitOfWork.SectionsLegTypeRepository
                    .GetWhereFirst(x => x.Name.ToLower() == sectionsLegTypeEntity.Name.ToLower() && !x.Deleted);

                if (sectionsLegType != null)
                    return true;

                else
                    return false;
            }
            else if (ConfigrationTables.TLIlogisticalType.ToString() == TableName)
            {
                TLIlogisticalType logisticalTypeEntity = _mapper.Map<TLIlogisticalType>(entity);

                TLIlogisticalType logisticalType = _unitOfWork.logisticalTypeRepository
                    .GetWhereFirst(x => x.Name.ToLower() == logisticalTypeEntity.Name.ToLower() && !x.Deleted);

                if (logisticalType != null)
                    return true;

                else
                    return false;
            }
            else if (ConfigrationTables.TLIbaseCivilWithLegsType.ToString() == TableName)
            {
                TLIbaseCivilWithLegsType baseCivilWithLegsTypeEntity = _mapper.Map<TLIbaseCivilWithLegsType>(entity);

                TLIbaseCivilWithLegsType baseCivilWithLegsType = _unitOfWork.BaseCivilWithLegsTypeRepository
                    .GetWhereFirst(x => x.Name.ToLower() == baseCivilWithLegsTypeEntity.Name.ToLower() && !x.Deleted);

                if (baseCivilWithLegsType != null)
                    return true;

                else
                    return false;
            }
            else if (ConfigrationTables.TLIbaseGeneratorType.ToString() == TableName)
            {
                TLIbaseGeneratorType baseGeneratorTypeEntity = _mapper.Map<TLIbaseGeneratorType>(entity);

                TLIbaseGeneratorType baseGeneratorType = _unitOfWork.BaseGeneratorTypeRepository
                    .GetWhereFirst(x => x.Name.ToLower() == baseGeneratorTypeEntity.Name.ToLower() && !x.Deleted);

                if (baseGeneratorType != null)
                    return true;

                else
                    return false;
            }
            else if (ConfigrationTables.TLIinstallationCivilwithoutLegsType.ToString() == TableName)
            {
                TLIinstallationCivilwithoutLegsType installationCivilwithoutLegsTypeEntity = _mapper.Map<TLIinstallationCivilwithoutLegsType>(entity);

                TLIinstallationCivilwithoutLegsType installationCivilwithoutLegsType = _unitOfWork.InstallationCivilwithoutLegsTypeRepository
                    .GetWhereFirst(x => x.Name.ToLower() == installationCivilwithoutLegsTypeEntity.Name.ToLower() && !x.Deleted);
                
                if (installationCivilwithoutLegsType != null)
                    return true;

                else
                    return false;
            }
            else if (ConfigrationTables.TLIboardType.ToString() == TableName)
            {
                TLIboardType boardTypeEntity = _mapper.Map<TLIboardType>(entity);

                TLIboardType boardType = _unitOfWork.BoardTypeRepository
                    .GetWhereFirst(x => x.Name.ToLower() == boardTypeEntity.Name.ToLower() && !x.Deleted);

                if (boardType != null)
                    return true;

                else
                    return false;
            }
            else if (ConfigrationTables.TLIenforcmentCategory.ToString() == TableName)
            {
                TLIenforcmentCategory enforcmentCategoryEntity = _mapper.Map<TLIenforcmentCategory>(entity);

                TLIenforcmentCategory enforcmentCategory = _unitOfWork.EnforcmentCategoryRepository
                    .GetWhereFirst(x => x.Name.ToLower() == enforcmentCategoryEntity.Name.ToLower() && !x.Deleted);

                if (enforcmentCategory != null)
                    return true;

                else
                    return false;
            }

            else if (ConfigrationTables.TLIguyLineType.ToString() == TableName)
            {
                TLIguyLineType guyLineTypeEntity = _mapper.Map<TLIguyLineType>(entity);

                TLIguyLineType guyLineType = _unitOfWork.GuyLineTypeRepository
                    .GetWhereFirst(x => x.Name.ToLower() == guyLineTypeEntity.Name.ToLower() && !x.Deleted);

                if (guyLineType != null)
                    return true;

                else
                    return false;
            }
            else if (ConfigrationTables.TLIpowerType.ToString() == TableName)
            {
                TLIpowerType powerTypeEntity = _mapper.Map<TLIpowerType>(entity);

                TLIpowerType powerType = _unitOfWork.PowerTypeRepository
                    .GetWhereFirst(x => x.Name.ToLower() == powerTypeEntity.Name.ToLower() && !x.Delete);

                if (powerType != null)
                    return true;

                else
                    return false;
            }

            else if (ConfigrationTables.TLIowner.ToString() == TableName)
            {
                TLIowner OwnerEntity = _mapper.Map<TLIowner>(entity);

                TLIowner OwnerType = _unitOfWork.OwnerRepository
                    .GetWhereFirst(x => x.OwnerName == OwnerEntity.OwnerName && !x.Deleted);

                if (OwnerType != null)
                    return true;

                else
                    return false;
            }
            else if (ConfigrationTables.TLIlocationType.ToString() == TableName)
            {
                TLIlocationType LocationTypeEntity = _mapper.Map<TLIlocationType>(entity);

                TLIlocationType LocationTypeType = _unitOfWork.LocationTypeRepository
                    .GetWhereFirst(x => x.Name.ToLower() == LocationTypeEntity.Name.ToLower() && !x.Deleted);

                if (LocationTypeType != null)
                    return true;

                else
                    return false;
            }
            else if (ConfigrationTables.TLIbaseType.ToString() == TableName)
            {
                TLIbaseType BaseTypeEntity = _mapper.Map<TLIbaseType>(entity);

                TLIbaseType BaseTypeType = _unitOfWork.BaseTypeRepository
                    .GetWhereFirst(x => x.Name.ToLower() == BaseTypeEntity.Name.ToLower() && !x.Deleted);

                if (BaseTypeType != null)
                    return true;

                else
                    return false;
            }
            else if (ConfigrationTables.TLIbaseBU.ToString() == TableName)
            {
                TLIbaseBU BaseBUEntity = _mapper.Map<TLIbaseBU>(entity);

                TLIbaseBU BaseBUType = _unitOfWork.BaseBURepository
                    .GetWhereFirst(x => x.Name.ToLower() == BaseBUEntity.Name.ToLower() && !x.Deleted);

                if (BaseBUType != null)
                    return true;

                else
                    return false;
            }
            else if (ConfigrationTables.TLIrenewableCabinetType.ToString() == TableName)
            {
                TLIrenewableCabinetType RenewableCabinetTypeEntity = _mapper.Map<TLIrenewableCabinetType>(entity);

                TLIrenewableCabinetType RenewableCabinetType = _unitOfWork.RenewableCabinetTypeRepository
                    .GetWhereFirst(x => x.Name.ToLower() == RenewableCabinetTypeEntity.Name.ToLower() && !x.Deleted);

                if (RenewableCabinetType != null)
                    return true;

                else
                    return false;
            }
            else if (ConfigrationTables.TLIsideArmType.ToString() == TableName)
            {
                TLIsideArmType SideArmTypeEntity = _mapper.Map<TLIsideArmType>(entity);

                TLIsideArmType SideArmType = _unitOfWork.SideArmTypeRepository
                    .GetWhereFirst(x => x.Name.ToLower() == SideArmTypeEntity.Name.ToLower() && !x.Deleted);

                if (SideArmType != null)
                    return true;

                else
                    return false;
            }
            else if (ConfigrationTables.TLIitemStatus.ToString() == TableName)
            {
                TLIitemStatus ItemStatusEntity = _mapper.Map<TLIitemStatus>(entity);

                TLIitemStatus ItemStatusType = _unitOfWork.ItemStatusRepository
                    .GetWhereFirst(x => x.Name.ToLower() == ItemStatusEntity.Name.ToLower() && !x.Deleted);

                if (ItemStatusType != null)
                    return true;

                else
                    return false;
            }
            else if (ConfigrationTables.TLIinstallationPlace.ToString() == TableName)
            {
                TLIinstallationPlace InstallationPlaceEntity = _mapper.Map<TLIinstallationPlace>(entity);

                TLIinstallationPlace InstallationPlaceType = _unitOfWork.InstallationPlaceRepository
                    .GetWhereFirst(x => x.Name.ToLower() == InstallationPlaceEntity.Name.ToLower() && !x.Deleted);

                if (InstallationPlaceType != null)
                    return true;

                else
                    return false;
            }
            else if (ConfigrationTables.TLIcivilNonSteelType.ToString() == TableName)
            {
                TLIcivilNonSteelType CivilNonSteelType = _mapper.Map<TLIcivilNonSteelType>(entity);

                TLIcivilNonSteelType CheckName = _unitOfWork.CivilNonSteelTypeRepository
                    .GetWhereFirst(x => x.Name.ToLower() == CivilNonSteelType.Name.ToLower() && !x.Deleted);

                if (CheckName != null)
                    return true;

                else
                    return false;
            }
            else
            {
                return false;
            }
            

        }

        private bool ValidateUpdate(string Name, object entity)
        {
            if (ConfigrationTables.TLIdiversityType.ToString() == Name)
            {
                var diversityTypeEntity = _mapper.Map<TLIdiversityType>(entity);
              
                var diversityType = _unitOfWork.DiversityTypeRepository.GetWhereFirst(x => (x.Name == diversityTypeEntity.Name && x.Id != diversityTypeEntity.Id));

                if (diversityType == null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else if (ConfigrationTables.TLItelecomType.ToString() == Name)
            {
                var telecomTypeEntity = _mapper.Map<TLItelecomType>(entity);
              
                var telecomType = _unitOfWork.TelecomTypeRepository.GetWhereFirst(x => (x.Name == telecomTypeEntity.Name && x.Id != telecomTypeEntity.Id));

                if (telecomType == null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else if (ConfigrationTables.TLIoduInstallationType.ToString() == Name)
            {
                var oduInstallationTypeEntity = _mapper.Map<TLIoduInstallationType>(entity);
               
                var oduInstallationType = _unitOfWork.OduInstallationTypeRepository.GetWhereFirst(x => (x.Name == oduInstallationTypeEntity.Name && x.Id != oduInstallationTypeEntity.Id));
                if (oduInstallationType == null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else if (ConfigrationTables.TLIsideArmInstallationPlace.ToString() == Name)
            {
                var sideArmInstallationPlaceEntity = _mapper.Map<TLIsideArmInstallationPlace>(entity);
                                                                                                                                      
                var sideArmInstallationPlace = _unitOfWork.SideArmInstallationPlaceRepository.GetWhereFirst(x => (x.Name == sideArmInstallationPlaceEntity.Name && x.Id != sideArmInstallationPlaceEntity.Id));
                if (sideArmInstallationPlace == null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else if (ConfigrationTables.TLIitemConnectTo.ToString() == Name)
            {
                var itemConnectToEntity = _mapper.Map<TLIitemConnectTo>(entity);
                var itemConnectTo = _unitOfWork.ItemConnectToRepository.GetWhereFirst(x => (x.Name == itemConnectToEntity.Name && x.Id != itemConnectToEntity.Id));
                if (itemConnectTo == null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else if (ConfigrationTables.TLIrepeaterType.ToString() == Name)
            {
                var repeaterTypeEntity = _mapper.Map<TLIrepeaterType>(entity);
                
                var repeaterType = _unitOfWork.RepeaterTypeRepository.GetWhereFirst(x => (x.Name == repeaterTypeEntity.Name && x.Id != repeaterTypeEntity.Id));
                if (repeaterType == null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else if (ConfigrationTables.TLIsupportTypeDesigned.ToString() == Name)
            {
                var supportTypeDesignedEntity = _mapper.Map<TLIsupportTypeDesigned>(entity);
                //var supportTypeDesigned = _unitOfWork.SupportTypeDesignedRepository.WhereFilters(new List<TLIS_DAL.Helper.Filters.FilterExperssionOneValue> { new TLIS_DAL.Helper.Filters.FilterExperssionOneValue { propertyName = "Name", comparison = "==", value = supportTypeDesignedEntity.Name },
                //                                                                                                                                  new TLIS_DAL.Helper.Filters.FilterExperssionOneValue { propertyName = "Id", comparison = "!=", value = supportTypeDesignedEntity.Id.ToString() }}).FirstOrDefault();
                var supportTypeDesigned = _unitOfWork.SupportTypeDesignedRepository.GetWhereFirst(x => (x.Name == supportTypeDesignedEntity.Name && x.Id != supportTypeDesignedEntity.Id));
                if (supportTypeDesigned == null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else if (ConfigrationTables.TLIsupportTypeImplemented.ToString() == Name)
            {
                var supportTypeImplementedEntity = _mapper.Map<TLIsupportTypeImplemented>(entity);
                //  var supportTypeImplemented = _unitOfWork.SupportTypeImplementedRepository.WhereFilters(new List<TLIS_DAL.Helper.Filters.FilterExperssionOneValue> { new TLIS_DAL.Helper.Filters.FilterExperssionOneValue { propertyName = "Name", comparison = "==", value = supportTypeImplementedEntity.Name },
                var supportTypeImplemented = _unitOfWork.SupportTypeImplementedRepository.GetWhereFirst(x => (x.Name == supportTypeImplementedEntity.Name && x.Id != supportTypeImplementedEntity.Id));                                                                                                                              //  new TLIS_DAL.Helper.Filters.FilterExperssionOneValue { propertyName = "Id", comparison = "!=", value = supportTypeImplementedEntity.Id.ToString() }}).FirstOrDefault();
                if (supportTypeImplemented == null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else if (ConfigrationTables.TLIstructureType.ToString() == Name)
            {
                var structureTypeEntity = _mapper.Map<TLIstructureType>(entity);
                // var structureType = _unitOfWork.StructureTypeRepository.WhereFilters(new List<TLIS_DAL.Helper.Filters.FilterExperssionOneValue> { new TLIS_DAL.Helper.Filters.FilterExperssionOneValue { propertyName = "Name", comparison = "==", value = structureTypeEntity.Name },
                var structureType = _unitOfWork.StructureTypeRepository.GetWhereFirst(x => (x.Name == structureTypeEntity.Name && x.Id != structureTypeEntity.Id));                                                                                                                        //      new TLIS_DAL.Helper.Filters.FilterExperssionOneValue { propertyName = "Id", comparison = "!=", value = structureTypeEntity.Id.ToString() }}).FirstOrDefault();

                if (structureType == null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else if (ConfigrationTables.TLIpolarityOnLocation.ToString() == Name)
            {
                var polarityOnLocationEntity = _mapper.Map<TLIpolarityOnLocation>(entity);
                //  var polarityOnLocation = _unitOfWork.PolarityOnLocationRepository.WhereFilters(new List<TLIS_DAL.Helper.Filters.FilterExperssionOneValue> { new TLIS_DAL.Helper.Filters.FilterExperssionOneValue { propertyName = "Name", comparison = "==", value = polarityOnLocationEntity.Name },
                var polarityOnLocation = _unitOfWork.PolarityOnLocationRepository.GetWhereFirst(x => (x.Name == polarityOnLocationEntity.Name && x.Id != polarityOnLocationEntity.Id));                                                                                                                              //  new TLIS_DAL.Helper.Filters.FilterExperssionOneValue { propertyName = "Id", comparison = "!=", value = polarityOnLocationEntity.Id.ToString() }}).FirstOrDefault();
                if (polarityOnLocation == null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else if (ConfigrationTables.TLIsectionsLegType.ToString() == Name)
            {
                var sectionsLegTypeEntity = _mapper.Map<TLIsectionsLegType>(entity);
                // var sectionsLegType = _unitOfWork.SectionsLegTypeRepository.WhereFilters(new List<TLIS_DAL.Helper.Filters.FilterExperssionOneValue> { new TLIS_DAL.Helper.Filters.FilterExperssionOneValue { propertyName = "Name", comparison = "==", value = sectionsLegTypeEntity.Name },
                var sectionsLegType = _unitOfWork.SectionsLegTypeRepository.GetWhereFirst(x => (x.Name == sectionsLegTypeEntity.Name && x.Id != sectionsLegTypeEntity.Id));                                                                                                                             //   new TLIS_DAL.Helper.Filters.FilterExperssionOneValue { propertyName = "Id", comparison = "!=", value = sectionsLegTypeEntity.Id.ToString() }}).FirstOrDefault();
                if (sectionsLegType == null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else if (ConfigrationTables.TLIlogisticalType.ToString() == Name)
            {
                var logisticalTypeEntity = _mapper.Map<TLIlogisticalType>(entity);
                // var logisticalType = _unitOfWork.logisticalTypeRepository.WhereFilters(new List<TLIS_DAL.Helper.Filters.FilterExperssionOneValue> { new TLIS_DAL.Helper.Filters.FilterExperssionOneValue { propertyName = "Name", comparison = "==", value = logisticalTypeEntity.Name },
                var logisticalType = _unitOfWork.logisticalTypeRepository.GetWhereFirst(x => (x.Name == logisticalTypeEntity.Name && x.Id != logisticalTypeEntity.Id));                                                                                                                            //   new TLIS_DAL.Helper.Filters.FilterExperssionOneValue { propertyName = "Id", comparison = "!=", value = logisticalTypeEntity.Id.ToString() }}).FirstOrDefault();
                if (logisticalType == null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else if (ConfigrationTables.TLIbaseCivilWithLegsType.ToString() == Name)
            {
                var baseCivilWithLegsTypeEntity = _mapper.Map<TLIbaseCivilWithLegsType>(entity);
                // var baseCivilWithLegsType = _unitOfWork.BaseCivilWithLegsTypeRepository.WhereFilters(new List<TLIS_DAL.Helper.Filters.FilterExperssionOneValue> { new TLIS_DAL.Helper.Filters.FilterExperssionOneValue { propertyName = "Name", comparison = "==", value = baseCivilWithLegsTypeEntity.Name },
                var baseCivilWithLegsType = _unitOfWork.BaseCivilWithLegsTypeRepository.GetWhereFirst(x => (x.Name == baseCivilWithLegsTypeEntity.Name && x.Id != baseCivilWithLegsTypeEntity.Id));                                                                                                                        //      new TLIS_DAL.Helper.Filters.FilterExperssionOneValue { propertyName = "Id", comparison = "!=", value = baseCivilWithLegsTypeEntity.Id.ToString() }}).FirstOrDefault();
                if (baseCivilWithLegsType == null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else if (ConfigrationTables.TLIbaseGeneratorType.ToString() == Name)
            {
                var baseGeneratorTypeEntity = _mapper.Map<TLIbaseGeneratorType>(entity);
                // var baseGeneratorType = _unitOfWork.BaseGeneratorTypeRepository.WhereFilters(new List<TLIS_DAL.Helper.Filters.FilterExperssionOneValue> { new TLIS_DAL.Helper.Filters.FilterExperssionOneValue { propertyName = "Name", comparison = "==", value = baseGeneratorTypeEntity.Name },
                var baseGeneratorType = _unitOfWork.BaseGeneratorTypeRepository.GetWhereFirst(x => (x.Name == baseGeneratorTypeEntity.Name && x.Id != baseGeneratorTypeEntity.Id));                                                                                                                       //          new TLIS_DAL.Helper.Filters.FilterExperssionOneValue { propertyName = "Id", comparison = "!=", value = baseGeneratorTypeEntity.Id.ToString() }}).FirstOrDefault();
                if (baseGeneratorType == null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else if (ConfigrationTables.TLIinstallationCivilwithoutLegsType.ToString() == Name)
            {
                var installationCivilwithoutLegsTypeEntity = _mapper.Map<TLIinstallationCivilwithoutLegsType>(entity);
                //  var installationCivilwithoutLegsType = _unitOfWork.InstallationCivilwithoutLegsTypeRepository.WhereFilters(new List<TLIS_DAL.Helper.Filters.FilterExperssionOneValue> { new TLIS_DAL.Helper.Filters.FilterExperssionOneValue { propertyName = "Name", comparison = "==", value = installationCivilwithoutLegsTypeEntity.Name },
                var installationCivilwithoutLegsType = _unitOfWork.InstallationCivilwithoutLegsTypeRepository.GetWhereFirst(x => (x.Name == installationCivilwithoutLegsTypeEntity.Name && x.Id != installationCivilwithoutLegsTypeEntity.Id));                                                                                                                              // new TLIS_DAL.Helper.Filters.FilterExperssionOneValue { propertyName = "Id", comparison = "!=", value = installationCivilwithoutLegsTypeEntity.Id.ToString() }}).FirstOrDefault();
                if (installationCivilwithoutLegsType == null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else if (ConfigrationTables.TLIboardType.ToString() == Name)
            {
                var boardTypeEntity = _mapper.Map<TLIboardType>(entity);
                //  var boardType = _unitOfWork.BoardTypeRepository.WhereFilters(new List<TLIS_DAL.Helper.Filters.FilterExperssionOneValue> { new TLIS_DAL.Helper.Filters.FilterExperssionOneValue { propertyName = "Name", comparison = "==", value = boardTypeEntity.Name },
                var boardType = _unitOfWork.BoardTypeRepository.GetWhereFirst(x => (x.Name == boardTypeEntity.Name && x.Id != boardTypeEntity.Id));                                                                                                                              // new TLIS_DAL.Helper.Filters.FilterExperssionOneValue { propertyName = "Id", comparison = "!=", value = boardTypeEntity.Id.ToString() }}).FirstOrDefault();
                if (boardType == null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else if (ConfigrationTables.TLIguyLineType.ToString() == Name)
            {
                var guyLineTypeEntity = _mapper.Map<TLIguyLineType>(entity);
                // var guyLineType = _unitOfWork.GuyLineTypeRepository.WhereFilters(new List<TLIS_DAL.Helper.Filters.FilterExperssionOneValue> { new TLIS_DAL.Helper.Filters.FilterExperssionOneValue { propertyName = "Name", comparison = "==", value = guyLineTypeEntity.Name },
                var guyLineType = _unitOfWork.GuyLineTypeRepository.GetWhereFirst(x => (x.Name == guyLineTypeEntity.Name && x.Id != guyLineTypeEntity.Id));                                                                                                                //      new TLIS_DAL.Helper.Filters.FilterExperssionOneValue { propertyName = "Id", comparison = "!=", value = guyLineTypeEntity.Id.ToString() }}).FirstOrDefault();
                if (guyLineType == null)
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }
            else if (ConfigrationTables.TLIenforcmentCategory.ToString() == Name)
            {
                var enforcmentCategoryEntity = _mapper.Map<TLIenforcmentCategory>(entity);
                // var enforcmentCategory = _unitOfWork.EnforcmentCategoryRepository.WhereFilters(new List<TLIS_DAL.Helper.Filters.FilterExperssionOneValue> { new TLIS_DAL.Helper.Filters.FilterExperssionOneValue { propertyName = "Name", comparison = "==", value = enforcmentCategoryEntity.Name },
                var enforcmentCategory = _unitOfWork.EnforcmentCategoryRepository.GetWhereFirst(x => (x.Name == enforcmentCategoryEntity.Name && x.Id != enforcmentCategoryEntity.Id));                                                                                                                                //   new TLIS_DAL.Helper.Filters.FilterExperssionOneValue { propertyName = "Id", comparison = "!=", value = enforcmentCategoryEntity.Id.ToString() }}).FirstOrDefault();
                if (enforcmentCategory == null)
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }
            else if (ConfigrationTables.TLIpowerType.ToString() == Name)
            {
                var powerTypeEntity = _mapper.Map<TLIpowerType>(entity);
                // var powerType = _unitOfWork.PowerTypeRepository.WhereFilters(new List<TLIS_DAL.Helper.Filters.FilterExperssionOneValue> { new TLIS_DAL.Helper.Filters.FilterExperssionOneValue { propertyName = "Name", comparison = "==", value = powerTypeEntity.Name },
                var powerType = _unitOfWork.PowerTypeRepository.GetWhereFirst(x => (x.Name == powerTypeEntity.Name && x.Id != powerTypeEntity.Id));                                                                                                                         //         new TLIS_DAL.Helper.Filters.FilterExperssionOneValue { propertyName = "Id", comparison = "!=", value = powerTypeEntity.Id.ToString() }}).FirstOrDefault();
                if (powerType == null)
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }

            else
            {
                return false;
            }
        }
    }
}
