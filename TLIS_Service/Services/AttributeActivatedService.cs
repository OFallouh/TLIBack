using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLIS_DAL.Helper;
using TLIS_DAL.Helper.Filters;
using TLIS_DAL.Helpers;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModelBase;
using TLIS_DAL.ViewModels.AttActivatedCategoryDTOs;
using TLIS_DAL.ViewModels.AttributeActivatedDTOs;
using TLIS_DAL.ViewModels.TablesHistoryDTOs;
using TLIS_DAL.ViewModels.TablesNamesDTOs;
using TLIS_Repository.Base;
using TLIS_Service.IService;

namespace TLIS_Service.Services
{
    public class AttributeActivatedService : IAttributeActivatedService
    {
        IUnitOfWork _unitOfWork;
        IServiceCollection _services;
        private IMapper _mapper;
        public AttributeActivatedService(IUnitOfWork unitOfWork, IServiceCollection services,IMapper mapper) 
        {
            _unitOfWork = unitOfWork;
            _services = services;
            _mapper = mapper;
        }
        public Response<AttributeActivatedViewModel> GetById(int Id)
        {
            try
            {
                var Att = _unitOfWork.AttributeActivatedRepository.GetByID(Id);
                var AttViewModel = _mapper.Map<AttributeActivatedViewModel>(Att);
                return new Response<AttributeActivatedViewModel>(true, AttViewModel, null, null, (int)Helpers.Constants.ApiReturnCode.success);
            }
            catch (Exception err)
            {
                return new Response<AttributeActivatedViewModel>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
        }
        public Response<TablesNamesViewModel> GetTableNameByAttributeId(int AttributeId, bool IsDynamic)
        {
            try
            {
                if (IsDynamic)
                {
                    string TableName = _unitOfWork.DynamicAttRepository
                        .GetIncludeWhereFirst(x => x.Id == AttributeId, x => x.tablesNames).tablesNames.TableName;

                    TablesNamesViewModel TablesNamesViewModel = new TablesNamesViewModel
                    {
                        Id = _unitOfWork.TablesNamesRepository.GetWhereFirst(x => x.TableName.ToLower() == TableName.ToLower()).Id,
                        TableName = TableName
                    };

                    return new Response<TablesNamesViewModel>(true, TablesNamesViewModel, null, null, (int)Helpers.Constants.ApiReturnCode.success);
                }
                else
                {
                    string TableName = _unitOfWork.AttributeActivatedRepository
                        .GetWhereFirst(x => x.Id == AttributeId).Tabel;

                    TablesNamesViewModel TablesNamesViewModel = new TablesNamesViewModel
                    {
                        Id = _unitOfWork.TablesNamesRepository.GetWhereFirst(x => x.TableName.ToLower() == TableName.ToLower()).Id,
                        TableName = TableName
                    };

                    return new Response<TablesNamesViewModel>(true, TablesNamesViewModel, null, null, (int)Helpers.Constants.ApiReturnCode.success);
                }
            }
            catch (Exception err)
            {
                return new Response<TablesNamesViewModel>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
        }
        public Response<IEnumerable<TLIattributeActivated>> GetStaticAttsWithoutPagination(int? CivilWithoutLegCategoryId, string AttributeName, string TableName)
        {
            try
            {
                List<TLIattributeActivated> Attributes = new List<TLIattributeActivated>();
                List<TLIattActivatedCategory> AttrbiuteViewManagment = new List<TLIattActivatedCategory?>();
                if (CivilWithoutLegCategoryId == null)
                {
                    if (!string.IsNullOrEmpty(AttributeName))
                    {
                        Attributes = _unitOfWork.AttributeActivatedRepository.GetWhere(x =>
                            x.Key.ToLower().StartsWith(AttributeName.ToLower()) &&
                            x.Tabel.ToLower() == TableName.ToLower() &&
                            x.DataType != "List" && x.Key.ToLower() != "id" && x.Key.ToLower() != "active" && x.Key.ToLower() != "deleted").Distinct().ToList();
                    }
                    else
                    {
                        Attributes = _unitOfWork.AttributeActivatedRepository.GetWhere(x =>
                            (x.Tabel.ToLower() == TableName.ToLower() &&
                             x.DataType != "List" && x.Key.ToLower() != "id" && x.Key.ToLower() != "active" && x.Key.ToLower() != "deleted") ||
                            (x.Tabel.ToLower() == TableName.ToLower() && x.Key.ToLower() == "model")).Distinct().ToList();
                    }
                }

                else if (CivilWithoutLegCategoryId != null)
                {
                    bool isLibrary = false;
                    if (TableName.ToLower() == Helpers.Constants.TablesNames.TLIcivilWithoutLegLibrary.ToString().ToLower())
                    {
                        isLibrary = true;
                    }

                    AttrbiuteViewManagment = _unitOfWork.AttActivatedCategoryRepository.GetIncludeWhere(x =>
                        (x.attributeActivatedId != null && x.civilWithoutLegCategoryId == CivilWithoutLegCategoryId &&
                         x.IsLibrary == isLibrary) ||
                        (x.attributeActivated.Key.ToLower() == "model" && x.civilWithoutLegCategoryId == CivilWithoutLegCategoryId &&
                        x.IsLibrary == isLibrary), x => x.attributeActivated).ToList();

                    List<TLIattributeActivated> AttributeActivated = AttrbiuteViewManagment
                        .Select(x => x.attributeActivated).ToList();

                    if (!string.IsNullOrEmpty(AttributeName))
                    {
                        Attributes = AttributeActivated.Where(x => x.Key.ToLower().StartsWith(AttributeName.ToLower()) && x.DataType != "List" &&
                            x.Key.ToLower() != "id" && x.Key.ToLower() != "active" && x.Key.ToLower() != "deleted").Distinct().ToList();
                    }
                    else
                    {
                        Attributes = AttributeActivated.Where(x => (x.DataType != "List" &&
                             x.Key.ToLower() != "id" && x.Key.ToLower() != "active" && x.Key.ToLower() != "deleted") ||
                            (x.Tabel.ToLower() == TableName.ToLower() && x.Key.ToLower() == "model")).Distinct().ToList();
                    }
                    foreach (var Attribute in Attributes)
                    {
                        TLIattActivatedCategory UpdateAttribute = AttrbiuteViewManagment
                            .FirstOrDefault(x => x.attributeActivatedId.Value == Attribute.Id);

                        Attribute.Label = UpdateAttribute.Label;
                        Attribute.enable = UpdateAttribute.enable;
                        Attribute.Required = UpdateAttribute.Required;
                        Attribute.Description = UpdateAttribute.Description;
                    }
                }

                List<AttributeActivatedViewModel> AttributesViewModel = _mapper.Map<List<AttributeActivatedViewModel>>(Attributes);
                Attributes = _mapper.Map<List<TLIattributeActivated>>(AttributesViewModel);

                int count = Attributes.Count();

                foreach (TLIattributeActivated Attribute in Attributes)
                {
                    if (Attribute.Key.ToLower() == "length" || Attribute.Key.ToLower() == "width" ||
                        Attribute.Key.ToLower() == "diameter" || Attribute.Key.ToLower() == "height")
                    {
                        Attribute.Manage = true;
                        Attribute.enable = true;
                        Attribute.Required = true;
                    }

                    else if (Attribute.Key.ToLower() == "model")
                    {
                        Attribute.enable = true;
                        Attribute.Required = true;
                    }

                    if (CivilWithoutLegCategoryId != null)
                    {
                        TLIattActivatedCategory AttributeActivatedCategory = AttrbiuteViewManagment.FirstOrDefault(x =>
                            x.attributeActivatedId == Attribute.Id && x.civilWithoutLegCategoryId == CivilWithoutLegCategoryId);

                        if (AttributeActivatedCategory != null)
                        {
                            if (!string.IsNullOrEmpty(AttributeActivatedCategory.Label))
                                Attribute.Label = AttributeActivatedCategory.Label;

                            if (!string.IsNullOrEmpty(AttributeActivatedCategory.Description))
                                Attribute.Description = AttributeActivatedCategory.Description;

                            Attribute.Required = AttributeActivatedCategory.Required;
                            Attribute.enable = AttributeActivatedCategory.enable;
                        }
                    }
                }
                return new Response<IEnumerable<TLIattributeActivated>>(true, Attributes, null, null, (int)Helpers.Constants.ApiReturnCode.success, count);
            }
            catch (Exception err)
            {
                return new Response<IEnumerable<TLIattributeActivated>>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
        }
        public Response<IEnumerable<TLIattributeActivated>> GetStaticAtts(int? CivilWithoutLegCategoryId, string AttributeName, ParameterPagination parameters, string TableName)
        {
            try
            {
                List<TLIattributeActivated> Attributes = new List<TLIattributeActivated>();
                List<TLIattActivatedCategory> AttrbiuteViewManagment = new List<TLIattActivatedCategory?>();
                if (CivilWithoutLegCategoryId == null)
                {
                    if (!string.IsNullOrEmpty(AttributeName))
                    {
                        Attributes = _unitOfWork.AttributeActivatedRepository.GetWhere(x =>
                            x.Key.ToLower().StartsWith(AttributeName.ToLower()) &&
                            x.Tabel.ToLower() == TableName.ToLower() &&
                            x.DataType != "List" && x.Key.ToLower() != "id" && x.Key.ToLower() != "active" && x.Key.ToLower() != "deleted").Distinct().ToList();
                    }
                    else
                    {
                        Attributes = _unitOfWork.AttributeActivatedRepository.GetWhere(x =>
                            (x.Tabel.ToLower() == TableName.ToLower() &&
                             x.DataType != "List" && x.Key.ToLower() != "id" && x.Key.ToLower() != "active" && x.Key.ToLower() != "deleted") ||
                            (x.Tabel.ToLower() == TableName.ToLower() && x.Key.ToLower() == "model")).Distinct().ToList();
                    }
                }

                else if (CivilWithoutLegCategoryId != null)
                {
                    bool isLibrary = false;
                    if (TableName.ToLower() == Helpers.Constants.TablesNames.TLIcivilWithoutLegLibrary.ToString().ToLower())
                    {
                        isLibrary = true;
                    }

                    AttrbiuteViewManagment = _unitOfWork.AttActivatedCategoryRepository.GetIncludeWhere(x =>
                        (x.attributeActivatedId != null && x.civilWithoutLegCategoryId == CivilWithoutLegCategoryId &&
                         x.IsLibrary == isLibrary) ||
                        (x.attributeActivated.Key.ToLower() == "model" && x.civilWithoutLegCategoryId == CivilWithoutLegCategoryId &&
                        x.IsLibrary == isLibrary), x => x.attributeActivated).ToList();

                    List<TLIattributeActivated> AttributeActivated = AttrbiuteViewManagment
                        .Select(x => x.attributeActivated).ToList();

                    if (!string.IsNullOrEmpty(AttributeName))
                    {
                        Attributes = AttributeActivated.Where(x => x.Key.ToLower().StartsWith(AttributeName.ToLower()) && x.DataType != "List" &&
                            x.Key.ToLower() != "id" && x.Key.ToLower() != "active" && x.Key.ToLower() != "deleted").Distinct().ToList();
                    }
                    else
                    {
                        Attributes = AttributeActivated.Where(x => (x.DataType != "List" &&
                             x.Key.ToLower() != "id" && x.Key.ToLower() != "active" && x.Key.ToLower() != "deleted") ||
                            (x.Tabel.ToLower() == TableName.ToLower() && x.Key.ToLower() == "model")).Distinct().ToList();
                    }
                    foreach (var Attribute in Attributes)
                    {
                        TLIattActivatedCategory UpdateAttribute = AttrbiuteViewManagment
                            .FirstOrDefault(x => x.attributeActivatedId.Value == Attribute.Id);

                        Attribute.Label = UpdateAttribute.Label;
                        Attribute.enable = UpdateAttribute.enable;
                        Attribute.Required = UpdateAttribute.Required;
                        Attribute.Description = UpdateAttribute.Description;
                    }
                }

                TLIattributeActivated NameAttribute = Attributes.FirstOrDefault(x => x.Key.ToLower() == "Name".ToLower() ||
                    x.Key.ToLower() == "DishName".ToLower());

                if (NameAttribute != null)
                {
                    TLIattributeActivated Swap = Attributes[0];
                    Attributes[Attributes.IndexOf(NameAttribute)] = Swap;
                    Attributes[0] = NameAttribute;
                }

                List<AttributeActivatedViewModel> AttributesViewModel = _mapper.Map<List<AttributeActivatedViewModel>>(Attributes);
                Attributes = _mapper.Map<List<TLIattributeActivated>>(AttributesViewModel);

                int count = Attributes.Count();

                Attributes = Attributes.Skip((parameters.PageNumber - 1) * parameters.PageSize).
                    Take(parameters.PageSize).ToList();

                foreach (TLIattributeActivated Attribute in Attributes)
                {
                    if (Attribute.Key.ToLower() == "length" || Attribute.Key.ToLower() == "width" ||
                        Attribute.Key.ToLower() == "diameter" || Attribute.Key.ToLower() == "height")
                    {
                        Attribute.Manage = true;
                        Attribute.enable = true;
                        Attribute.Required = true;
                    }

                    else if (Attribute.Key.ToLower() == "model")
                    {
                        Attribute.enable = true;
                        Attribute.Required = true;
                    }

                    if (CivilWithoutLegCategoryId != null)
                    {
                        TLIattActivatedCategory AttributeActivatedCategory = AttrbiuteViewManagment.FirstOrDefault(x =>
                            x.attributeActivatedId == Attribute.Id && x.civilWithoutLegCategoryId == CivilWithoutLegCategoryId);

                        if (AttributeActivatedCategory != null)
                        {
                            if (!string.IsNullOrEmpty(AttributeActivatedCategory.Label))
                                Attribute.Label = AttributeActivatedCategory.Label;

                            if (!string.IsNullOrEmpty(AttributeActivatedCategory.Description))
                                Attribute.Description = AttributeActivatedCategory.Description;

                            Attribute.Required = AttributeActivatedCategory.Required;
                            Attribute.enable = AttributeActivatedCategory.enable;
                        }
                    }
                }
                return new Response<IEnumerable<TLIattributeActivated>>(true, Attributes, null, null, (int)Helpers.Constants.ApiReturnCode.success, count);
            }
            catch (Exception err)
            {
                return new Response<IEnumerable<TLIattributeActivated>>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
        }

        //public async Task AddAttributeActivated(AddAttributeActivatedViewModel addAttributeActivatedViewModel)
        //{
        //    TLIattributeActivated AttributeActivatedEntites = _mapper.Map<TLIattributeActivated>(addAttributeActivatedViewModel);

        //    await _unitOfWork.AttributeActivatedRepository.AddAsync(AttributeActivatedEntites);
        //    await _unitOfWork.SaveChangesAsync();

        //}

        //Function that take the Properties of each table and use addAtts function to add data to TLIattributeActivated table
        public async Task AddTablesActivatedAttributes()
        {
            //TLIaction action = new TLIaction();
            //await addAtts(action);

            //TLIactionItemOption ActionItemOption = new TLIactionItemOption();
            //await addAtts(ActionItemOption);

            //TLIactionOption ActionOption = new TLIactionOption();
            //await addAtts(ActionOption);

            //TLIagenda Agenda = new TLIagenda();
            //await addAtts(Agenda);

            //TLIagendaGroup AgendaGroup = new TLIagendaGroup();
            //await addAtts(AgendaGroup);

            //TLIallCivilInst AllCivilInst = new TLIallCivilInst();
            //await addAtts(AllCivilInst);

            //TLIallLoadInst AllLoadInst = new TLIallLoadInst();
            //await addAtts(AllLoadInst);

            //TLIallOtherInventoryInst AllOtherInventoryInst = new TLIallOtherInventoryInst();
            //await addAtts(AllOtherInventoryInst);

            //TLIantennaRRUInst AntennaRRUInst = new TLIantennaRRUInst();
            //await addAtts(AntennaRRUInst);

            //TLIattachedFiles AttachedFiles = new TLIattachedFiles();
            //await addAtts(AttachedFiles);


            TLIactor actor = new TLIactor();
            await addAtts(actor);
            TLIarea area = new TLIarea();
            await addAtts(area);
            TLIasType asType = new TLIasType();
            await addAtts(asType);
            TLIattActivatedCategory attActivatedCategory = new TLIattActivatedCategory();
            await addAtts(attActivatedCategory);
            TLIbaseCivilWithLegsType baseCivil = new TLIbaseCivilWithLegsType();
            await addAtts(baseCivil);
            TLIbaseGeneratorType baseGeneratorType = new TLIbaseGeneratorType();
            await addAtts(baseGeneratorType);
            TLIboardType boardType = new TLIboardType();
            await addAtts(boardType);
            TLIcabinet cabinet = new TLIcabinet();
            await addAtts(cabinet);
            TLIcabinetPowerLibrary cabinetPowerLibrary = new TLIcabinetPowerLibrary();
            await addAtts(cabinetPowerLibrary);
            TLIcabinetPowerType cabinetPowerType = new TLIcabinetPowerType();
            await addAtts(cabinetPowerType);
            TLIcabinetTelecomLibrary cabinetTelecomLibrary = new TLIcabinetTelecomLibrary();
            await addAtts(cabinetTelecomLibrary);
            TLIcapacity capacity = new TLIcapacity();
            await addAtts(capacity);
            TLIcity city = new TLIcity();
            await addAtts(city);
            TLIcivilNonSteel civilNonSteel = new TLIcivilNonSteel();
            await addAtts(civilNonSteel);
            TLIcivilNonSteelLibrary civilNonSteelLibrary = new TLIcivilNonSteelLibrary();
            await addAtts(civilNonSteelLibrary);
            TLIcivilSiteDate civilSiteDate = new TLIcivilSiteDate();
            await addAtts(civilSiteDate);
            TLIcivilSteelSupportCategory civilSteelSupportCategory = new TLIcivilSteelSupportCategory();
            await addAtts(civilSteelSupportCategory);
            TLIcivilWithLegLibrary civilWithLegLibrary = new TLIcivilWithLegLibrary();
            await addAtts(civilWithLegLibrary);
            TLIcivilWithLegs civilWithLegs = new TLIcivilWithLegs();
            await addAtts(civilWithLegs);
            TLIcivilWithoutLeg CivilWithoutLeg = new TLIcivilWithoutLeg();
            await addAtts(CivilWithoutLeg);
            TLIcivilWithoutLegCategory civilWithoutLegCategory = new TLIcivilWithoutLegCategory();
            await addAtts(civilWithoutLegCategory);
            TLIcivilWithoutLegLibrary civilWithoutLegLibrary = new TLIcivilWithoutLegLibrary();
            await addAtts(civilWithoutLegLibrary);
            TLIcondition condition = new TLIcondition();
            await addAtts(condition);
            TLIconditionType conditionType = new TLIconditionType();
            await addAtts(conditionType);
            TLIdataType dataType = new TLIdataType();
            await addAtts(dataType);
            TLIdependency dependency = new TLIdependency();
            await addAtts(dependency);
            TLIdependencyRow dependencyRow = new TLIdependencyRow();
            await addAtts(dependencyRow);
            TLIdiversityType diversityType = new TLIdiversityType();
            await addAtts(diversityType);
            TLIdynamicAtt dynamicAtt = new TLIdynamicAtt();
            await addAtts(dynamicAtt);
            TLIdynamicAttInstValue dynamicAttInst = new TLIdynamicAttInstValue();
            await addAtts(dynamicAttInst);
            TLIdynamicAttLibValue dynamicAttLibValue = new TLIdynamicAttLibValue();
            await addAtts(dynamicAttLibValue);
            TLIgenerator generator = new TLIgenerator();
            await addAtts(generator);
            TLIgeneratorLibrary generatorLibrary = new TLIgeneratorLibrary();
            await addAtts(generatorLibrary);
            TLIgroup group = new TLIgroup();
            await addAtts(group);
            TLIgroupRole groupRole = new TLIgroupRole();
            await addAtts(groupRole);
            TLIgroupUser groupUser = new TLIgroupUser();
            await addAtts(groupUser);
            TLIguyLineType guyLineType = new TLIguyLineType();
            await addAtts(guyLineType);
            TLIhistoryDetails historyDetails = new TLIhistoryDetails();
            await addAtts(historyDetails);
            TLIhistoryType historyType = new TLIhistoryType();
            await addAtts(historyType);
            TLIinstallationCivilwithoutLegsType installationCivilwithoutLegsType = new TLIinstallationCivilwithoutLegsType();
            await addAtts(installationCivilwithoutLegsType);
            TLIitemStatus itemStatus = new TLIitemStatus();
            await addAtts(itemStatus);
            TLIleg leg = new TLIleg();
            await addAtts(leg);
            TLIlog log = new TLIlog();
            await addAtts(log);
            TLIlogicalOperation logicalOperation = new TLIlogicalOperation();
            await addAtts(logicalOperation);
            TLIlogistical logistical = new TLIlogistical();
            await addAtts(logistical);
            TLIlogisticalitem logisticalitem = new TLIlogisticalitem();
            await addAtts(logisticalitem);
            TLIlogisticalType logisticalType = new TLIlogisticalType();
            await addAtts(logisticalType);
            TLImwBULibrary mwBULibrary = new TLImwBULibrary();
            await addAtts(mwBULibrary);
            TLImwDishLibrary mwDishLibrary = new TLImwDishLibrary();
            await addAtts(mwDishLibrary);
            TLImwODULibrary mwODULibrary = new TLImwODULibrary();
            await addAtts(mwODULibrary);
            TLImwRFULibrary mwRFULibrary = new TLImwRFULibrary();
            await addAtts(mwRFULibrary);
            TLIoperation operation = new TLIoperation();
            await addAtts(operation);
            TLIoption option = new TLIoption();
            await addAtts(option);
            TLIotherInSite otherInSite = new TLIotherInSite();
            await addAtts(otherInSite);
            TLIotherInventoryDistance otherInventoryDistance = new TLIotherInventoryDistance();
            await addAtts(otherInventoryDistance);
            TLIowner owner = new TLIowner();
            await addAtts(owner);
            TLIparity parity = new TLIparity();
            await addAtts(parity);
            TLIpermission permission = new TLIpermission();
            await addAtts(permission);
            TLIpolarityType polarityType = new TLIpolarityType();
            await addAtts(polarityType);
            TLIpowerLibrary powerLibrary = new TLIpowerLibrary();
            await addAtts(powerLibrary);
            TLIradioAntennaLibrary radioAntennaLibrary = new TLIradioAntennaLibrary();
            await addAtts(radioAntennaLibrary);
            TLIradioOtherLibrary radioOtherLibrary = new TLIradioOtherLibrary();
            await addAtts(radioOtherLibrary);
            TLIradioRRULibrary radioRRULibrary = new TLIradioRRULibrary();
            await addAtts(radioRRULibrary);
            TLIregion region = new TLIregion();
            await addAtts(region);
            TLIrenewableCabinetType renewableCabinetType = new TLIrenewableCabinetType();
            await addAtts(renewableCabinetType);
            TLIrole role = new TLIrole();
            await addAtts(role);
            TLIrolePermission rolePermission = new TLIrolePermission();
            await addAtts(rolePermission);
            TLIrow row = new TLIrow();
            await addAtts(row);
            TLIrowRule rowRule = new TLIrowRule();
            await addAtts(rowRule);
            TLIrule rule = new TLIrule();
            await addAtts(rule);
            TLIsectionsLegType sectionsLegType = new TLIsectionsLegType();
            await addAtts(sectionsLegType);
            TLIsideArmLibrary sideArmLibrary = new TLIsideArmLibrary();
            await addAtts(sideArmLibrary);
            TLIsite site = new TLIsite();
            await addAtts(site);
            TLIsolar solar = new TLIsolar();
            await addAtts(solar);
            TLIsolarLibrary solarLibrary = new TLIsolarLibrary();
            await addAtts(solarLibrary);
            TLIstructureType structureType = new TLIstructureType();
            await addAtts(structureType);
            TLIsuboption suboption = new TLIsuboption();
            await addAtts(suboption);
            TLIsupportTypeDesigned supportTypeDesigned = new TLIsupportTypeDesigned();
            await addAtts(supportTypeDesigned);
            TLIsupportTypeImplemented supportTypeImplemented = new TLIsupportTypeImplemented();
            await addAtts(supportTypeImplemented);
            TLItablesHistory tablesHistory = new TLItablesHistory();
            await addAtts(tablesHistory);
            TLItaskStatus taskStatus = new TLItaskStatus();
            await addAtts(taskStatus);
            TLItelecomType telecomType = new TLItelecomType();
            await addAtts(telecomType);
            TLIuser user = new TLIuser();
            await addAtts(user);
            TLIuserPermission userPermission = new TLIuserPermission();
            await addAtts(userPermission);
            TLIvalidation validation = new TLIvalidation();
            await addAtts(validation);
            TLImwOtherLibrary mwOtherLibrary = new TLImwOtherLibrary();
            await addAtts(mwOtherLibrary);
            TLIloadOtherLibrary loadOtherLibrary = new TLIloadOtherLibrary();
            await addAtts(loadOtherLibrary);
            TLIRadioRRU radioRRU = new TLIRadioRRU();
            await addAtts(radioRRU);
            TLIradioAntenna radioAntenna = new TLIradioAntenna();
            await addAtts(radioAntenna);
            TLImwOther mwOther = new TLImwOther();
            await addAtts(mwOther);
            TLIradioOther radioOther = new TLIradioOther();
            await addAtts(radioOther);
            TLIloadOther loadOther = new TLIloadOther();
            await addAtts(loadOther);
            TLImwBU mwBU = new TLImwBU();
            await addAtts(mwBU);
            TLImwODU mwODU = new TLImwODU();
            await addAtts(mwODU);
            TLImwDish mwDish = new TLImwDish();
            await addAtts(mwDish);
            TLImwRFU mwRFU = new TLImwRFU();
            await addAtts(mwRFU);
            TLIpower power = new TLIpower();
            await addAtts(power);
            TLIsideArm sideArm = new TLIsideArm();
            await addAtts(sideArm);
            TLIcivilSupportDistance civilSupportDistance = new TLIcivilSupportDistance();
            await addAtts(civilSupportDistance);
            TLIcivilLoads civilLoads = new TLIcivilLoads();
            await addAtts(civilLoads);
        }
        //add record for each property in database
        private async Task addAtts(object model)
        {
            var Culomns = model.GetType().GetProperties().ToList();
            foreach (var Culomn in Culomns)
            {
                var type = Culomn.PropertyType.Name;
                //var DeclareType = Culomn.DeclaringType.BaseType;
                //var DeclareType_1 = Culomn.PropertyType.;
                
                if (type == "Int32" || type == "String" || type == "Nullable`1" || type == "Boolean" || type == "Single" || type == "Double" || type == "DateTime")
                {
                    TLIattributeActivated attributeActivated = new TLIattributeActivated();
                    attributeActivated.Key = Culomn.Name.ToString();

                    if (Culomn.Name.Contains("Id") && Culomn.Name != "Id")
                    {
                        attributeActivated.Label = attributeActivated.Key.Split("Id")[0] + "_Name";
                        attributeActivated.Description = Culomns
                            .FirstOrDefault(x => x.Name.ToLower() == attributeActivated.Key.Split("Id")[0].ToLower()).GetType().Name;
                    }
                    else
                    {
                        attributeActivated.Label = Culomn.Name.ToString();
                        attributeActivated.Description = Culomn.Name.ToString();
                    }
                    
                    var TableName = model.GetType().Name;
                    attributeActivated.Tabel = TableName;
                    if (Culomn.Name.ToString() == "Name")
                    {
                        attributeActivated.Required = true;
                    }
                    else
                    {
                        attributeActivated.Required = false;
                    }
                    if (Culomn.Name.ToString() == "Id")
                    {
                        attributeActivated.enable = false;
                    }
                    else
                    {
                        attributeActivated.enable = true;
                    }
                    attributeActivated.AutoFill = false;
                    attributeActivated.Manage = false;
                    attributeActivated.Required = false;
                    if (Culomn.Name.ToString() != "Id" && Culomn.Name.ToString().Contains("Id"))
                    {
                        attributeActivated.DataType = "List";
                    }
                    else if (type == "Int32")
                    {
                        attributeActivated.DataType = "int";
                    }
                    else if (type == "Boolean")
                    {
                        attributeActivated.DataType = "bool";
                    }
                    else if (type == "String")
                    {
                        attributeActivated.DataType = "string";
                    }
                    else if (type == "Single")
                    {
                        attributeActivated.DataType = "float";
                    }
                    else if (type == "Double")
                    {
                        attributeActivated.DataType = "double";
                    }
                    else if (type == "DateTime")
                    {
                        attributeActivated.DataType = "DateTime";
                    }
                    else if (type == "Nullable`1")
                    {
                        if (Culomn.PropertyType == typeof(int?))
                        {
                            attributeActivated.DataType = "int";
                        }
                        else if (Culomn.PropertyType == typeof(float?))
                        {
                            attributeActivated.DataType = "float";
                        }
                        else if (Culomn.PropertyType == typeof(double?))
                        {
                            attributeActivated.DataType = "double";
                        }
                        else if (Culomn.PropertyType == typeof(bool?))
                        {
                            attributeActivated.DataType = "bool";
                        }
                        else if (Culomn.PropertyType == typeof(DateTime?))
                        {
                            attributeActivated.DataType = "DateTime";
                        }
                    }
                    _unitOfWork.AttributeActivatedRepository.Add(attributeActivated);
                    
                    await _unitOfWork.SaveChangesAsync();

                    if(TableName == "TLIcivilWithLegLibrary" || TableName == "TLIcivilWithoutLegLibrary")
                    {
                        TLIattActivatedCategory attActivatedCategory = new  TLIattActivatedCategory();
                        attActivatedCategory.attributeActivatedId = attributeActivated.Id;

                        if (Culomn.Name.Contains("Id") && Culomn.Name != "Id")
                        {
                            attActivatedCategory.Label = attributeActivated.Key.Split("Id")[0] + "_Name";
                            attActivatedCategory.Description = Culomns
                                .FirstOrDefault(x => x.Name.ToLower() == attributeActivated.Key.Split("Id")[0].ToLower()).GetType().Name;
                        }
                        else
                        {
                            attActivatedCategory.Label = Culomn.Name.ToString();
                            attActivatedCategory.Description = Culomn.Name.ToString();
                        }

                    }
                }
            }
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<Response<AttributeActivatedViewModel>> Disable(int AttributeActivatedId, int? CivilWithoutLegCategoryId)
        {
            try
            {
                TLIattributeActivated AttributeActivatedTest = _unitOfWork.AttributeActivatedRepository.GetWhereFirst(x => x.Id == AttributeActivatedId);
                
                if(AttributeActivatedTest.Manage)
                    return new Response<AttributeActivatedViewModel>(true, null, null, $"{AttributeActivatedTest.Key} attribute is included in space calculation and can't be disabled", (int)Helpers.Constants.ApiReturnCode.fail);

                if (CivilWithoutLegCategoryId == null)
                {
                    TLIattributeActivated AttributeActivated = _unitOfWork.AttributeActivatedRepository.GetByID(AttributeActivatedId);
                    if (AttributeActivated.Key.ToLower() == "length" || AttributeActivated.Key.ToLower() == "width" ||
                        AttributeActivated.Key.ToLower() == "diameter" || AttributeActivated.Key.ToLower() == "height")
                    {
                        return new Response<AttributeActivatedViewModel>(true, null, null, "This Attribute is Engaged in Space Calculation Formula So Its Enable Status Can't Be Updated", (int)Helpers.Constants.ApiReturnCode.fail);
                    }

                    AttributeActivatedViewModel BeforeUpdate = _mapper.Map<AttributeActivatedViewModel>(AttributeActivated);

                    AttributeActivated.enable = !(AttributeActivated.enable);
                    if (AttributeActivated.enable == false)
                    {
                        AttributeActivated.Required = false;
                    }

                    AttributeActivatedViewModel AfterUpdate = _mapper.Map<AttributeActivatedViewModel>(AttributeActivated);

                    EditHistoryDetails testUpdate = CheckUpdateObject(BeforeUpdate, AfterUpdate);

                    await _unitOfWork.AttributeActivatedRepository.UpdateItem(AttributeActivated);
                    // AddHistoryForEditAttActivated(AttributeActivated.Id, "Update", testUpdate.Details.ToList());
                    await _unitOfWork.SaveChangesAsync();
                }
                else
                {
                    TLIattActivatedCategory AttributeActivatedCategory = _unitOfWork.AttActivatedCategoryRepository.GetIncludeWhereFirst(x =>
                        x.civilWithoutLegCategoryId == CivilWithoutLegCategoryId.Value && x.attributeActivatedId == AttributeActivatedId, x => x.attributeActivated);

                    if (AttributeActivatedCategory.attributeActivated.Key.ToLower() == "length" || AttributeActivatedCategory.attributeActivated.Key.ToLower() == "width" ||
                        AttributeActivatedCategory.attributeActivated.Key.ToLower() == "diameter" || AttributeActivatedCategory.attributeActivated.Key.ToLower() == "height")
                    {
                        return new Response<AttributeActivatedViewModel>(true, null, null, "This Attribute is Engaged in Space Calculation Formula So Its Enable Status Can't Be Updated", (int)Helpers.Constants.ApiReturnCode.fail);
                    }

                    TLIattActivatedCategory BeforeUpdate = _mapper.Map<TLIattActivatedCategory>(AttributeActivatedCategory);

                    AttributeActivatedCategory.enable = !(AttributeActivatedCategory.enable);
                    if(AttributeActivatedCategory.enable == false)
                    {
                        AttributeActivatedCategory.Required = false;
                    }

                    TLIattActivatedCategory AfterUpdate = _mapper.Map<TLIattActivatedCategory>(AttributeActivatedCategory);

                    EditHistoryDetails testUpdate = CheckUpdateObject(BeforeUpdate, AfterUpdate);

                    await _unitOfWork.AttActivatedCategoryRepository.UpdateItem(AttributeActivatedCategory);
                    // AddHistoryForEditAttActivated(AttributeActivated.Id, "Update", testUpdate.Details.ToList());
                    await _unitOfWork.SaveChangesAsync();
                }
                return new Response<AttributeActivatedViewModel>();
            }
            catch (Exception err)
            {
                return new Response<AttributeActivatedViewModel>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
        }
        public async Task<Response<AttributeActivatedViewModel>> RequiredNOTRequired(int AttributeActivatedId, int? CivilWithoutLegCategoryId)
        {
            try
            {
                if (CivilWithoutLegCategoryId == null)
                {
                    TLIattributeActivated AttributeActivated = _unitOfWork.AttributeActivatedRepository.GetByID(AttributeActivatedId);
                    if (AttributeActivated.Key.ToLower() == "length" || AttributeActivated.Key.ToLower() == "width" ||
                        AttributeActivated.Key.ToLower() == "diameter" || AttributeActivated.Key.ToLower() == "height")
                    {
                        return new Response<AttributeActivatedViewModel>(true, null, null, "This Attribute is Engaged in Space Calculation Formula So Its Required Status Can't Be Updated", (int)Helpers.Constants.ApiReturnCode.fail);
                    }

                    AttributeActivatedViewModel BeforeUpdate = _mapper.Map<AttributeActivatedViewModel>(AttributeActivated);
                    AttributeActivated.Required = !(AttributeActivated.Required);
                    AttributeActivatedViewModel AfterUpdate = _mapper.Map<AttributeActivatedViewModel>(AttributeActivated);

                    EditHistoryDetails testUpdate = CheckUpdateObject(BeforeUpdate, AfterUpdate);
                    await _unitOfWork.AttributeActivatedRepository.UpdateItem(AttributeActivated);
                    // AddHistoryForEditAttActivated(AttributeActivated.Id, "Update", testUpdate.Details.ToList());
                    await _unitOfWork.SaveChangesAsync();
                }
                else
                {
                    TLIattActivatedCategory AttributeActivatedCategory = _unitOfWork.AttActivatedCategoryRepository.GetIncludeWhereFirst(x =>
                        x.attributeActivatedId == AttributeActivatedId && x.civilWithoutLegCategoryId == CivilWithoutLegCategoryId.Value, x => x.attributeActivated);

                    if (AttributeActivatedCategory.attributeActivated.Key.ToLower() == "length" || AttributeActivatedCategory.attributeActivated.Key.ToLower() == "width" ||
                        AttributeActivatedCategory.attributeActivated.Key.ToLower() == "diameter" || AttributeActivatedCategory.attributeActivated.Key.ToLower() == "height")
                    {
                        return new Response<AttributeActivatedViewModel>(true, null, null, "This Attribute is Engaged in Space Calculation Formula So Its Required Status Can't Be Updated", (int)Helpers.Constants.ApiReturnCode.fail);
                    }

                    AttActivatedCategoryViewModel BeforeUpdate = _mapper.Map<AttActivatedCategoryViewModel>(AttributeActivatedCategory);
                    AttributeActivatedCategory.Required = !(AttributeActivatedCategory.Required);
                    AttActivatedCategoryViewModel AfterUpdate = _mapper.Map<AttActivatedCategoryViewModel>(AttributeActivatedCategory);

                    EditHistoryDetails testUpdate = CheckUpdateObject(BeforeUpdate, AfterUpdate);
                    await _unitOfWork.AttActivatedCategoryRepository.UpdateItem(AttributeActivatedCategory);
                    // AddHistoryForEditAttActivated(AttributeActivated.Id, "Update", testUpdate.Details.ToList());
                    await _unitOfWork.SaveChangesAsync();
                }
                return new Response<AttributeActivatedViewModel>();
            }
            catch (Exception err)
            {
                return new Response<AttributeActivatedViewModel>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
        }
        public async Task<Response<AttributeActivatedViewModel>> EditAttributeActivated(EditAttributeActivatedViewModel editAttributeActivatedViewModel, int? CivilWithoutLegCategoryId)
        {
            try
            {
                if (CivilWithoutLegCategoryId == null)
                {
                    TLIattributeActivated AttributeActivated = _unitOfWork.AttributeActivatedRepository.GetByID(editAttributeActivatedViewModel.Id);

                    AttributeActivated.Label = editAttributeActivatedViewModel.Label;
                    AttributeActivated.Description = editAttributeActivatedViewModel.Description;

                    await _unitOfWork.AttributeActivatedRepository.UpdateItem(AttributeActivated);
                    await _unitOfWork.SaveChangesAsync();
                }
                else
                {
                    TLIattActivatedCategory AttributeActivatedCategory = _unitOfWork.AttActivatedCategoryRepository.GetIncludeWhereFirst(x =>
                        x.attributeActivatedId == editAttributeActivatedViewModel.Id && x.civilWithoutLegCategoryId == CivilWithoutLegCategoryId.Value);

                    AttributeActivatedCategory.Label = editAttributeActivatedViewModel.Label;
                    AttributeActivatedCategory.Description = editAttributeActivatedViewModel.Description;

                    await _unitOfWork.AttActivatedCategoryRepository.UpdateItem(AttributeActivatedCategory);
                    await _unitOfWork.SaveChangesAsync();
                }
                return new Response<AttributeActivatedViewModel>();
            }
            catch (Exception err)
            {
                return new Response<AttributeActivatedViewModel>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
        }
        public void AddHistoryForEditAttActivated(int RecordId, string HistoryType, List<TLIhistoryDetails> details)
        {
            try
            {
                AddTablesHistoryViewModel tablesHistory = new AddTablesHistoryViewModel();
                tablesHistory.Date = DateTime.Now;
                tablesHistory.RecordId = RecordId;
                int TableNameid = _unitOfWork.TablesNamesRepository.GetWhereSelectFirst(x => x.TableName == "TLIattributeActivated", x => new { x.Id }).Id;
                tablesHistory.TablesNameId = TableNameid;
                var CheckTableHistory = _unitOfWork.TablesHistoryRepository.GetWhereFirst(x => x.HistoryType.Name == HistoryType && x.RecordId == RecordId && x.TablesNameId == TableNameid);
                if (CheckTableHistory != null)
                {
                    var TableHistory = _unitOfWork.TablesHistoryRepository.GetWhereAndSelect(x => x.HistoryType.Name == HistoryType && x.RecordId == RecordId && x.TablesNameId == TableNameid, x => new { x.Id }).ToList().Max(x => x.Id);
                    tablesHistory.PreviousHistoryId = TableHistory;
                }
                tablesHistory.UserId = 83;
                tablesHistory.HistoryTypeId = _unitOfWork.HistoryTypeRepository.GetWhereSelectFirst(x => x.Name == HistoryType, x => new { x.Id }).Id;
                _unitOfWork.TablesHistoryRepository.AddTableHistory(tablesHistory, details);
            }
            catch (Exception er)
            {

            }
        }
        public EditHistoryDetails CheckUpdateObject(object originalObj, object updateObj)
        {
            EditHistoryDetails result = new EditHistoryDetails();
            result.original = originalObj;
            result.Details = new List<TLIhistoryDetails>();
            foreach (var property in updateObj.GetType().GetProperties())
            {

                var x = property.GetValue(updateObj);
                var y = property.GetValue(originalObj);
                if (x != null && y != null)
                {
                    if (!x.Equals(y))
                    {
                        property.SetValue(result.original, x);
                        TLIhistoryDetails historyDetails = new TLIhistoryDetails();
                        //   historyDetails.AttributeType = AttributeType.Static;
                        historyDetails.AttName = property.Name;
                        historyDetails.OldValue = y.ToString();
                        historyDetails.NewValue = x.ToString();
                        result.Details.Add(historyDetails);
                        // _unitOfWork.HistoryDetailsRepository.Add(historyDetails);
                        // _unitOfWork.SaveChanges();
                        //property.SetValue(originalObj, updateObj.GetType().GetProperty(property.Name)
                        //.GetValue(originalObj, null));
                    }
                }



            }
            return result;
        }
    }
}
