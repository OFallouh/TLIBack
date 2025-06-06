﻿using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Transactions;
using TLIS_DAL;
using TLIS_DAL.Helper;
using TLIS_DAL.Helper.Filters;
using TLIS_DAL.Helpers;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModelBase;
using TLIS_DAL.ViewModels.AttributeActivatedDTOs;
using TLIS_DAL.ViewModels.BaseGeneratorTypeDTOs;
using TLIS_DAL.ViewModels.CabinetDTOs;
using TLIS_DAL.ViewModels.CabinetPowerLibraryDTOs;
using TLIS_DAL.ViewModels.CabinetTelecomLibraryDTOs;
using TLIS_DAL.ViewModels.CivilWithLegsDTOs;
using TLIS_DAL.ViewModels.DataTypeDTOs;
using TLIS_DAL.ViewModels.DynamicAttDTOs;
using TLIS_DAL.ViewModels.DynamicAttInstValueDTOs;
using TLIS_DAL.ViewModels.DynamicAttLibValueDTOs;
using TLIS_DAL.ViewModels.GeneratorDTOs;
using TLIS_DAL.ViewModels.GeneratorLibraryDTOs;
using TLIS_DAL.ViewModels.RenewableCabinetTypeDTOs;
using TLIS_DAL.ViewModels.RuleDTOs;
using TLIS_DAL.ViewModels.SolarDTOs;
using TLIS_DAL.ViewModels.SolarLibraryDTOs;
using TLIS_DAL.ViewModels.TablesHistoryDTOs;
using TLIS_DAL.ViewModels.WorkflowHistoryDTOs;
using TLIS_Repository.Base;
using TLIS_Service.IService;
using static TLIS_Service.Helpers.Constants;
using TLIS_DAL.ViewModels.MW_DishDTOs;
using System.Drawing;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Engineering;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using static TLIS_DAL.ViewModels.MW_DishLbraryDTOs.EditMWDishLibraryObject;
using TLIS_DAL.ViewModels.AsTypeDTOs;
using TLIS_DAL.ViewModels.ItemConnectToDTOs;
using TLIS_DAL.ViewModels.OwnerDTOs;
using TLIS_DAL.ViewModels.PolarityOnLocationDTOs;
using TLIS_DAL.ViewModels.PolarityTypeDTOs;
using TLIS_DAL.ViewModels.RepeaterTypeDTOs;
using static TLIS_DAL.ViewModels.GeneratorLibraryDTOs.EditGeneratorLibraryObject;
using TLIS_DAL.ViewModels.OtherInSiteDTOs;
using TLIS_DAL.ViewModels.CivilWithLegLibraryDTOs;
using TLIS_DAL.ViewModels.SupportTypeImplementedDTOs;
using TLIS_DAL.ViewModels.LocationTypeDTOs;
using static Org.BouncyCastle.Crypto.Engines.SM2Engine;
using static TLIS_DAL.ViewModels.SolarLibraryDTOs.EditSolarLibraryObject;
using static TLIS_DAL.ViewModels.CabinetPowerLibraryDTOs.EditCabinetPowerLibraryObject;
using static TLIS_DAL.ViewModels.CabinetTelecomLibraryDTOs.EditCabinetTelecomLibraryObject;
using TLIS_DAL.ViewModels.CabinetPowerTypeDTOs;
using TLIS_DAL.ViewModels.TelecomTypeDTOs;
using TLIS_DAL.ViewModels.ParityDTOs;
using TLIS_DAL.ViewModels.InstCivilwithoutLegsTypeDTOs;
using System.Reflection.Emit;

namespace TLIS_Service.Services
{
    public class OtherInventoryInstService : IOtherInventoryInstService
    {
        IUnitOfWork _unitOfWork;
        IServiceCollection _services;
        private readonly ApplicationDbContext _dbContext;
        private IMapper _mapper;
        public OtherInventoryInstService(IUnitOfWork unitOfWork, IServiceCollection services, ApplicationDbContext context, IMapper mapper)
        {
            _dbContext = context;
            _unitOfWork = unitOfWork;
            _services = services;
            ServiceProvider serviceProvider = _services.BuildServiceProvider();
            _mapper = mapper;
        }
        #region GetAttForAdd
        //Function take 4 parameters TableName, OtherInventoryLibraryType, OtherInventoryId, SiteCode
        //First get table name Entity by TableName
        //specify the table i deal with
        //then check library type and get record by Id then get library activated attributes and values
        //get installation activated attributes 
        //get dynamic attributes by TableNameId
        //get related tables
        public Response<ObjectInstAtts> GetAttForAdd(string TableName, string CabinetLibraryType, int OtherInventoryId, string SiteCode)
        {
            try
            {
                TLItablesNames TableNameEntity = _unitOfWork.TablesNamesRepository
                    .GetWhereFirst(o => o.TableName == TableName);

                ObjectInstAtts objectInst = new ObjectInstAtts();
                if (OtherInventoryType.TLIcabinet.ToString() == TableName)
                {
                    if (CabinetLibraryType == OtherInventoryType.TLIcabinetPowerLibrary.ToString())
                    {
                        CabinetPowerLibraryViewModel CabinetPowerLibrary = _mapper.Map<CabinetPowerLibraryViewModel>(_unitOfWork.CabinetPowerLibraryRepository
                            .GetIncludeWhereFirst(x => x.Id == OtherInventoryId, x => x.CabinetPowerType));

                        List<BaseAttView> LibraryAttributes = _unitOfWork.AttributeActivatedRepository
                            .GetAttributeActivated(TablesNames.TLIcabinetPowerLibrary.ToString(), CabinetPowerLibrary, null).ToList();

                        foreach (BaseAttView LibraryAttribute in LibraryAttributes)
                        {
                            if (LibraryAttribute.DataType.ToLower() == "list")
                            {
                                LibraryAttribute.Value = CabinetPowerLibrary.GetType().GetProperties()
                                    .FirstOrDefault(x => x.Name.ToLower() == LibraryAttribute.Label.ToLower()).GetValue(CabinetPowerLibrary);
                            }
                        }

                        List<BaseAttView> AddToLibraryAttributesActivated = _mapper.Map<List<BaseAttView>>(_unitOfWork.LogistcalRepository
                            .GetLogistical(TablePartName.OtherInventory.ToString(), TablesNames.TLIcabinetPowerLibrary.ToString(), CabinetPowerLibrary.Id).ToList());

                        LibraryAttributes.AddRange(AddToLibraryAttributesActivated);

                        objectInst.LibraryActivatedAttributes = LibraryAttributes;

                    }
                    else if (CabinetLibraryType == OtherInventoryType.TLIcabinetTelecomLibrary.ToString())
                    {
                        CabinetTelecomLibraryViewModel CabinetTelecomLibrary = _mapper.Map<CabinetTelecomLibraryViewModel>(_unitOfWork.CabinetTelecomLibraryRepository
                            .GetIncludeWhereFirst(x => x.Id == OtherInventoryId, x => x.TelecomType));

                        List<BaseAttView> LibraryAttributes = _unitOfWork.AttributeActivatedRepository
                            .GetAttributeActivated(TablesNames.TLIcabinetTelecomLibrary.ToString(), CabinetTelecomLibrary, null).ToList();

                        foreach (BaseAttView LibraryAttribute in LibraryAttributes)
                        {
                            if (LibraryAttribute.DataType.ToLower() == "list")
                            {
                                if (CabinetTelecomLibrary != null)
                                {
                                    LibraryAttribute.Value = CabinetTelecomLibrary.GetType().GetProperties()
                                        .FirstOrDefault(x => x.Name.ToLower() == LibraryAttribute.Label.ToLower()).GetValue(CabinetTelecomLibrary);
                                }
                            }
                        }

                        List<BaseAttView> AddToLibraryAttributesActivated = _mapper.Map<List<BaseAttView>>(_unitOfWork.LogistcalRepository
                            .GetLogistical(TablePartName.OtherInventory.ToString(), TablesNames.TLIcabinetTelecomLibrary.ToString(), CabinetTelecomLibrary.Id).ToList());

                        LibraryAttributes.AddRange(AddToLibraryAttributesActivated);

                        objectInst.LibraryActivatedAttributes = LibraryAttributes;
                    }

                    List<BaseInstAttView> ListAttributesActivated = _unitOfWork.AttributeActivatedRepository.
                        GetInstAttributeActivated(TablesNames.TLIcabinet.ToString(), null, "CabinetPowerLibraryId", "CabinetTelecomLibraryId").ToList();

                    BaseInstAttView Swap = ListAttributesActivated[0];
                    BaseInstAttView NameAttribute = ListAttributesActivated.FirstOrDefault(x => x.Key.ToLower() == "Name".ToLower());
                    ListAttributesActivated[ListAttributesActivated.IndexOf(NameAttribute)] = Swap;
                    ListAttributesActivated[0] = NameAttribute;

                    foreach (BaseInstAttView FKitem in ListAttributesActivated)
                    {
                        if (FKitem.Desc.ToLower() == "tlirenewablecabinettype")
                            FKitem.Value = _mapper.Map<List<RenewableCabinetTypeViewModel>>(_unitOfWork.RenewableCabinetTypeRepository.GetWhere(x => !x.Deleted && !x.Disable).ToList());
                    }

                    objectInst.AttributesActivated = ListAttributesActivated;
                    objectInst.OtherInSite = _unitOfWork.AttributeActivatedRepository.GetInstAttributeActivated(TablesNames.TLIotherInSite.ToString(), null, "allOtherInventoryInstId", "Dismantle", "SiteCode");
                    objectInst.OtherInventoryDistance = _unitOfWork.AttributeActivatedRepository
                        .GetInstAttributeActivated(TablesNames.TLIotherInventoryDistance.ToString(), null, "allOtherInventoryInstId", "SiteCode");

                    IEnumerable<DynaminAttInstViewModel> DynamicAttributesWithoutValue = _unitOfWork.DynamicAttRepository
                        .GetDynamicInstAtts(TableNameEntity.Id, null);

                    foreach (DynaminAttInstViewModel DynamicAttribute in DynamicAttributesWithoutValue)
                    {
                        TLIdynamicAtt DynamicAttributeEntity = _unitOfWork.DynamicAttRepository.GetByID(DynamicAttribute.Id);

                        if (!string.IsNullOrEmpty(DynamicAttributeEntity.DefaultValue))
                        {
                            if (DynamicAttribute.DataType.ToLower() == "string".ToLower())
                                DynamicAttribute.ValueString = DynamicAttributeEntity.DefaultValue;

                            else if (DynamicAttribute.DataType.ToLower() == "int".ToLower())
                                DynamicAttribute.ValueDouble = int.Parse(DynamicAttributeEntity.DefaultValue);

                            else if (DynamicAttribute.DataType.ToLower() == "double".ToLower())
                                DynamicAttribute.ValueDouble = double.Parse(DynamicAttributeEntity.DefaultValue);

                            else if (DynamicAttribute.DataType.ToLower() == "boolean".ToLower())
                                DynamicAttribute.ValueBoolean = bool.Parse(DynamicAttributeEntity.DefaultValue);

                            else if (DynamicAttribute.DataType.ToLower() == "datetime".ToLower())
                                DynamicAttribute.ValueDateTime = DateTime.Parse(DynamicAttributeEntity.DefaultValue);
                        }
                        else
                        {
                            DynamicAttribute.ValueString = " ".Split(' ')[0];
                        }
                    }

                    objectInst.DynamicAtts = DynamicAttributesWithoutValue;

                    List<KeyValuePair<string, List<DropDownListFilters>>> RelatedTables = new List<KeyValuePair<string, List<DropDownListFilters>>>();
                    List<KeyValuePair<string, List<DropDownListFilters>>> OtherInventoryDistanceRelatedTables = _unitOfWork.OtherInventoryDistanceRepository.CabientGetRelatedTables(SiteCode);
                    RelatedTables.AddRange(OtherInventoryDistanceRelatedTables);
                    objectInst.RelatedTables = RelatedTables;
                }
                else if (OtherInventoryType.TLIsolar.ToString() == TableName)
                {
                    SolarLibraryViewModel SolarLibrary = _mapper.Map<SolarLibraryViewModel>(_unitOfWork.SolarLibraryRepository
                        .GetIncludeWhereFirst(x => x.Id == OtherInventoryId, x => x.Capacity));

                    List<BaseAttView> LibraryAttributes = _unitOfWork.AttributeActivatedRepository
                        .GetAttributeActivated(TablesNames.TLIsolarLibrary.ToString(), SolarLibrary, null).ToList();

                    foreach (BaseAttView LibraryAttribute in LibraryAttributes)
                    {
                        if (LibraryAttribute.DataType.ToLower() == "list")
                        {
                            LibraryAttribute.Value = SolarLibrary.GetType().GetProperties()
                                .FirstOrDefault(x => x.Name.ToLower() == LibraryAttribute.Label.ToLower()).GetValue(SolarLibrary);
                        }
                    }

                    List<BaseAttView> AddToLibraryAttributesActivated = _mapper.Map<List<BaseAttView>>(_unitOfWork.LogistcalRepository
                        .GetLogistical(TablePartName.OtherInventory.ToString(), TablesNames.TLIsolarLibrary.ToString(), SolarLibrary.Id).ToList());

                    LibraryAttributes.AddRange(AddToLibraryAttributesActivated);

                    objectInst.LibraryActivatedAttributes = LibraryAttributes;

                    List<BaseInstAttView> ListAttributesActivated = _unitOfWork.AttributeActivatedRepository
                        .GetInstAttributeActivated(TableName, null, "SolarLibraryId").ToList();

                    BaseInstAttView Swap = ListAttributesActivated[0];
                    BaseInstAttView NameAttribute = ListAttributesActivated.FirstOrDefault(x => x.Key.ToLower() == "Name".ToLower());
                    ListAttributesActivated[ListAttributesActivated.IndexOf(NameAttribute)] = Swap;
                    ListAttributesActivated[0] = NameAttribute;

                    foreach (BaseInstAttView FKitem in ListAttributesActivated)
                    {
                        if (FKitem.Desc.ToLower() == "tlicabinet")
                        {

                            //var cabinet = _dbContext.TLIotherInSite.Include(x => x.allOtherInventoryInst).Where(x => !x.Dismantle && x.allOtherInventoryInstId != null &&
                            //    x.allOtherInventoryInst.cabinetId != null).Select(x => x.allOtherInventoryInst.cabinetId).ToList();

                            List<TLIcabinet> TLIcabinet = new List<TLIcabinet>();
                            var Cabinets = _dbContext.TLIotherInSite.Include(x => x.allOtherInventoryInst).Where(x => x.SiteCode == SiteCode && !x.Dismantle && x.allOtherInventoryInstId != null &&
                                x.allOtherInventoryInst.cabinetId != null).Select(x => x.allOtherInventoryInst.cabinetId).ToList();
                            foreach (var item in Cabinets)
                            {

                                var cabinetname = _dbContext.TLIcabinet.Where(x => x.Id == item).FirstOrDefault();
                                TLIcabinet.Add(cabinetname);

                            }
                            FKitem.Value = _mapper.Map<List<CabinetViewModel>>(TLIcabinet);

                        }

                    }

                    objectInst.AttributesActivated = ListAttributesActivated;
                    objectInst.OtherInSite = _unitOfWork.AttributeActivatedRepository.GetInstAttributeActivated(TablesNames.TLIotherInSite.ToString(), null, "allOtherInventoryInstId", "Dismantle", "SiteCode");
                    objectInst.OtherInventoryDistance = _unitOfWork.AttributeActivatedRepository.GetInstAttributeActivated(TablesNames.TLIotherInventoryDistance.ToString(), null, "allOtherInventoryInstId", "SiteCode");

                    IEnumerable<DynaminAttInstViewModel> DynamicAttributesWithoutValue = _unitOfWork.DynamicAttRepository
                        .GetDynamicInstAtts(TableNameEntity.Id, null);

                    foreach (DynaminAttInstViewModel DynamicAttribute in DynamicAttributesWithoutValue)
                    {
                        TLIdynamicAtt DynamicAttributeEntity = _unitOfWork.DynamicAttRepository.GetByID(DynamicAttribute.Id);

                        if (!string.IsNullOrEmpty(DynamicAttributeEntity.DefaultValue))
                        {
                            if (DynamicAttribute.DataType.ToLower() == "string".ToLower())
                                DynamicAttribute.ValueString = DynamicAttributeEntity.DefaultValue;

                            else if (DynamicAttribute.DataType.ToLower() == "int".ToLower())
                                DynamicAttribute.ValueDouble = int.Parse(DynamicAttributeEntity.DefaultValue);

                            else if (DynamicAttribute.DataType.ToLower() == "double".ToLower())
                                DynamicAttribute.ValueDouble = double.Parse(DynamicAttributeEntity.DefaultValue);

                            else if (DynamicAttribute.DataType.ToLower() == "boolean".ToLower())
                                DynamicAttribute.ValueBoolean = bool.Parse(DynamicAttributeEntity.DefaultValue);

                            else if (DynamicAttribute.DataType.ToLower() == "datetime".ToLower())
                                DynamicAttribute.ValueDateTime = DateTime.Parse(DynamicAttributeEntity.DefaultValue);
                        }
                        else
                        {
                            DynamicAttribute.ValueString = " ".Split(' ')[0];
                        }
                    }

                    objectInst.DynamicAtts = DynamicAttributesWithoutValue;

                    List<KeyValuePair<string, List<DropDownListFilters>>> RelatedTables = new List<KeyValuePair<string, List<DropDownListFilters>>>();
                    List<KeyValuePair<string, List<DropDownListFilters>>> OtherInventoryDistanceRelatedTables = _unitOfWork.OtherInventoryDistanceRepository.SolarGetRelatedTables(SiteCode);
                    RelatedTables.AddRange(OtherInventoryDistanceRelatedTables);
                    objectInst.RelatedTables = RelatedTables;
                }
                else if (OtherInventoryType.TLIgenerator.ToString() == TableName)
                {
                    GeneratorLibraryViewModel GeneratorLibrary = _mapper.Map<GeneratorLibraryViewModel>(_unitOfWork
                        .GeneratorLibraryRepository.GetIncludeWhereFirst(x => x.Id == OtherInventoryId, x => x.Capacity));

                    List<BaseAttView> LibraryAttributes = _unitOfWork.AttributeActivatedRepository
                        .GetAttributeActivated(TablesNames.TLIgeneratorLibrary.ToString(), GeneratorLibrary, null).ToList();

                    foreach (BaseAttView LibraryAttribute in LibraryAttributes)
                    {
                        if (LibraryAttribute.DataType.ToLower() == "list")
                        {
                            LibraryAttribute.Value = GeneratorLibrary.GetType().GetProperties()
                                .FirstOrDefault(x => x.Name.ToLower() == LibraryAttribute.Label.ToLower()).GetValue(GeneratorLibrary);
                        }
                    }

                    List<BaseAttView> AddToLibraryAttributesActivated = _mapper.Map<List<BaseAttView>>(_unitOfWork.LogistcalRepository
                        .GetLogistical(TablePartName.OtherInventory.ToString(), TablesNames.TLIgeneratorLibrary.ToString(), GeneratorLibrary.Id).ToList());

                    LibraryAttributes.AddRange(AddToLibraryAttributesActivated);

                    objectInst.LibraryActivatedAttributes = LibraryAttributes;

                    List<BaseInstAttView> ListAttributesActivated = _unitOfWork.AttributeActivatedRepository.
                        GetInstAttributeActivated(TablesNames.TLIgenerator.ToString(), null, "GeneratorLibraryId").ToList();

                    BaseInstAttView Swap = ListAttributesActivated[0];
                    BaseInstAttView NameAttribute = ListAttributesActivated.FirstOrDefault(x => x.Key.ToLower() == "Name".ToLower());
                    ListAttributesActivated[ListAttributesActivated.IndexOf(NameAttribute)] = Swap;
                    ListAttributesActivated[0] = NameAttribute;

                    foreach (BaseInstAttView FKitem in ListAttributesActivated)
                    {
                        if (FKitem.Desc.ToLower() == "tlibasegeneratortype")
                            FKitem.Value = _mapper.Map<List<BaseGeneratorTypeViewModel>>(_unitOfWork.BaseGeneratorTypeRepository.GetWhere(x => !x.Deleted && !x.Disable).ToList());
                    }

                    objectInst.AttributesActivated = ListAttributesActivated;
                    objectInst.OtherInSite = _unitOfWork.AttributeActivatedRepository.GetInstAttributeActivated(TablesNames.TLIotherInSite.ToString(), null, "allOtherInventoryInstId", "Dismantle", "SiteCode");
                    objectInst.OtherInventoryDistance = _unitOfWork.AttributeActivatedRepository.GetInstAttributeActivated(TablesNames.TLIotherInventoryDistance.ToString(), null, "allOtherInventoryInstId", "SiteCode");

                    IEnumerable<DynaminAttInstViewModel> DynamicAttributesWithoutValue = _unitOfWork.DynamicAttRepository
                        .GetDynamicInstAtts(TableNameEntity.Id, null);

                    foreach (DynaminAttInstViewModel DynamicAttribute in DynamicAttributesWithoutValue)
                    {
                        TLIdynamicAtt DynamicAttributeEntity = _unitOfWork.DynamicAttRepository.GetByID(DynamicAttribute.Id);

                        if (!string.IsNullOrEmpty(DynamicAttributeEntity.DefaultValue))
                        {
                            if (DynamicAttribute.DataType.ToLower() == "string".ToLower())
                                DynamicAttribute.ValueString = DynamicAttributeEntity.DefaultValue;

                            else if (DynamicAttribute.DataType.ToLower() == "int".ToLower())
                                DynamicAttribute.ValueDouble = int.Parse(DynamicAttributeEntity.DefaultValue);

                            else if (DynamicAttribute.DataType.ToLower() == "double".ToLower())
                                DynamicAttribute.ValueDouble = double.Parse(DynamicAttributeEntity.DefaultValue);

                            else if (DynamicAttribute.DataType.ToLower() == "boolean".ToLower())
                                DynamicAttribute.ValueBoolean = bool.Parse(DynamicAttributeEntity.DefaultValue);

                            else if (DynamicAttribute.DataType.ToLower() == "datetime".ToLower())
                                DynamicAttribute.ValueDateTime = DateTime.Parse(DynamicAttributeEntity.DefaultValue);
                        }
                        else
                        {
                            DynamicAttribute.ValueString = " ".Split(' ')[0];
                        }
                    }

                    objectInst.DynamicAtts = DynamicAttributesWithoutValue;

                    List<KeyValuePair<string, List<DropDownListFilters>>> RelatedTables = new List<KeyValuePair<string, List<DropDownListFilters>>>();
                    List<KeyValuePair<string, List<DropDownListFilters>>> OtherInventoryDistanceRelatedTables = _unitOfWork.OtherInventoryDistanceRepository.GeneratorGetRelatedTables(SiteCode);
                    RelatedTables.AddRange(OtherInventoryDistanceRelatedTables);
                    objectInst.RelatedTables = RelatedTables;
                }
                return new Response<ObjectInstAtts>(true, objectInst, null, null, (int)ApiReturnCode.success);
            }
            catch (Exception err)
            {
                return new Response<ObjectInstAtts>(true, null, null, err.Message, (int)ApiReturnCode.fail);
            }
        }
        public Response<GetForAddOtherInventoryInstallationObject> GetAttForAddGeneratorInstallation(string TableName, int LibraryID, string SiteCode, int UserId, bool ExternalSys)
        {
            try
            {
                TLItablesNames TableNameEntity = _unitOfWork.TablesNamesRepository.GetWhereFirst(x =>
                    x.TableName == TableName);

                GetForAddOtherInventoryInstallationObject objectInst = new GetForAddOtherInventoryInstallationObject();
                List<BaseInstAttViews> ListAttributesActivated = new List<BaseInstAttViews>();

                EditGeneratorLibraryAttributes GeneratorLibrary = _mapper.Map<EditGeneratorLibraryAttributes>(_unitOfWork.GeneratorLibraryRepository
                    .GetIncludeWhereFirst(x => x.Id == LibraryID ));
                if (GeneratorLibrary != null)
                {
                    List<BaseInstAttViews> LibraryAttributes = _unitOfWork.AttributeActivatedRepository
                        .GetAttributeActivatedGetForAdd(TablesNames.TLIgeneratorLibrary.ToString(), GeneratorLibrary, null).ToList();

                    List<BaseInstAttViews> LogisticalAttributes = _mapper.Map<List<BaseInstAttViews>>(_unitOfWork.LogistcalRepository
                        .GetLogisticals(TablePartName.OtherInventory.ToString(), Helpers.Constants.TablesNames.TLIgeneratorLibrary.ToString(), GeneratorLibrary.Id).ToList());

                    LibraryAttributes.AddRange(LogisticalAttributes);

                    objectInst.LibraryAttribute = LibraryAttributes;

                    ListAttributesActivated = _unitOfWork.AttributeActivatedRepository.
                        GetInstAttributeActivatedGetForAdd(OtherInventoryType.TLIgenerator.ToString(), null, "GeneratorLibraryId").ToList();

                    BaseInstAttViews NameAttribute = ListAttributesActivated.FirstOrDefault(x => x.Key.ToLower() == "Name".ToLower());
                    if (NameAttribute != null)
                    {
                        BaseInstAttViews Swap = ListAttributesActivated[0];
                        ListAttributesActivated[ListAttributesActivated.IndexOf(NameAttribute)] = Swap;
                        ListAttributesActivated[0] = NameAttribute;
                    }


                    Dictionary<string, Func<IEnumerable<object>>> repositoryMethods = new Dictionary<string, Func<IEnumerable<object>>>
                    {
                        { "basegeneratortype_name", () => _mapper.Map<List<BaseGeneratorTypeViewModel>>(_unitOfWork.BaseGeneratorTypeRepository.GetWhere(x => !x.Deleted && !x.Disable).ToList())},

                    };
                    ListAttributesActivated = ListAttributesActivated
                        .Select(FKitem =>
                        {
                            if (repositoryMethods.ContainsKey(FKitem.Label.ToLower()))
                            {
                                FKitem.Options = repositoryMethods[FKitem.Label.ToLower()]().ToList();
                            }
                            else
                            {
                                FKitem.Options = new object[0];
                            }

                            return FKitem;
                        })
                        .ToList();
                    objectInst.InstallationAttributes = ListAttributesActivated;
                    objectInst.OtherInSite = _unitOfWork.AttributeActivatedRepository.GetInstAttributeActivatedGetForAdd(TablesNames.TLIotherInSite.ToString(), null, "allOtherInventoryInstId", "Dismantle", "SiteCode");
                    objectInst.OtherInventoryDistance = _unitOfWork.AttributeActivatedRepository.GetInstAttributeActivatedGetForAdd(TablesNames.TLIotherInventoryDistance.ToString(), null, "allOtherInventoryInstId", "SiteCode");
                    var query = _dbContext.TLIotherInSite
                    .Include(x => x.allOtherInventoryInst).Include(x => x.allOtherInventoryInst.generator).Include(x => x.allOtherInventoryInst.cabinet).
                    Include(x => x.allOtherInventoryInst.solar)
                    .Where(x => x.SiteCode == SiteCode && x.allOtherInventoryInst != null && !x.Dismantle)
                    .ToList();


                    var locationTypeViewModels = query
                     .Select(item => new LocationTypeViewModel
                     {
                         Id = item.allOtherInventoryInstId,
                         Name = $"{_dbContext.MV_GENERATOR_VIEW.FirstOrDefault(x => x.Id == item.allOtherInventoryInst.generatorId)?.Name}" +
                         $" {_dbContext.MV_SOLAR_VIEW.FirstOrDefault(x => x.Id == item.allOtherInventoryInst.solarId)?.Name}" +
                         $" {_dbContext.MV_CABINET_POWER_VIEW.FirstOrDefault(x => x.Id == item.allOtherInventoryInst.cabinetId)?.Name}"
                         + $" {_dbContext.MV_CABINET_TELECOM_VIEW.FirstOrDefault(x => x.Id == item.allOtherInventoryInst.cabinetId)?.Name}".Trim()
                     })
                     .ToList();


                    objectInst.OtherInventoryDistance = objectInst.OtherInventoryDistance.Select(x =>
                    {
                        if (x.Label.ToLower() == "referenceotherinventory_name")
                        {
                            x.Options = locationTypeViewModels.ToList();
                        }
                        return x;
                    });
                    objectInst.DynamicAttribute = _unitOfWork.DynamicAttRepository
                    .GetDynamicInstAttInst(TableNameEntity.Id, null);
                    if (ExternalSys == true)
                    {
                        TLIhistory tLIhistory = new TLIhistory()
                        {
                            TablesNameId = TableNameEntity.Id,
                            ExternalSysId = UserId,
                            HistoryTypeId = 4,
                        };
                        _dbContext.TLIhistory.Add(tLIhistory);
                        _dbContext.SaveChanges();
                    }
                    return new Response<GetForAddOtherInventoryInstallationObject>(true, objectInst, null, null, (int)Helpers.Constants.ApiReturnCode.fail);
                }
                else
                {
                    return new Response<GetForAddOtherInventoryInstallationObject>(false, null, null, "this Generatorlibrary is not found", (int)Helpers.Constants.ApiReturnCode.fail);
                }
                return new Response<GetForAddOtherInventoryInstallationObject>(false, null, null, null, (int)Helpers.Constants.ApiReturnCode.fail);
            }
            catch (Exception err)
            {
                return new Response<GetForAddOtherInventoryInstallationObject>(false, null, null, err.Message, (int)ApiReturnCode.fail);
            }
        }
        public Response<GetForAddOtherInventoryInstallationObject> GetAttForAddSolarInstallation(string TableName, int LibraryID, string SiteCode,int UserId, bool ExternalSys)
        {
            try
            {
                TLItablesNames TableNameEntity = _unitOfWork.TablesNamesRepository.GetWhereFirst(x =>
                    x.TableName == TableName);

                GetForAddOtherInventoryInstallationObject objectInst = new GetForAddOtherInventoryInstallationObject();
                List<BaseInstAttViews> ListAttributesActivated = new List<BaseInstAttViews>();

                EditSolarLibraryAttributes SolarLibrary = _mapper.Map<EditSolarLibraryAttributes>(_unitOfWork.SolarLibraryRepository
                    .GetIncludeWhereFirst(x => x.Id == LibraryID));
                if (SolarLibrary != null)
                {
                    List<BaseInstAttViews> LibraryAttributes = _unitOfWork.AttributeActivatedRepository
                        .GetAttributeActivatedGetForAdd(TablesNames.TLIsolarLibrary.ToString(), SolarLibrary, null).ToList();

                    List<BaseInstAttViews> LogisticalAttributes = _mapper.Map<List<BaseInstAttViews>>(_unitOfWork.LogistcalRepository
                        .GetLogisticals(TablePartName.OtherInventory.ToString(), Helpers.Constants.TablesNames.TLIsolarLibrary.ToString(), SolarLibrary.Id).ToList());

                    LibraryAttributes.AddRange(LogisticalAttributes);

                    objectInst.LibraryAttribute = LibraryAttributes;

                    ListAttributesActivated = _unitOfWork.AttributeActivatedRepository.
                        GetInstAttributeActivatedGetForAdd(OtherInventoryType.TLIsolar.ToString(), null, "SolarLibraryId").ToList();

                    BaseInstAttViews NameAttribute = ListAttributesActivated.FirstOrDefault(x => x.Key.ToLower() == "Name".ToLower());
                    if (NameAttribute != null)
                    {
                        BaseInstAttViews Swap = ListAttributesActivated[0];
                        ListAttributesActivated[ListAttributesActivated.IndexOf(NameAttribute)] = Swap;
                        ListAttributesActivated[0] = NameAttribute;
                    }


                    Dictionary<string, Func<IEnumerable<object>>> repositoryMethods = new Dictionary<string, Func<IEnumerable<object>>>
                    {
                        { "cabinet_name", () => _mapper.Map<List<BaseGeneratorTypeViewModel>>(_unitOfWork.OtherInSiteRepository.GetWhereAndInclude(x => !x.Dismantle && 
                        x.SiteCode.ToLower()==SiteCode.ToLower()&& x.allOtherInventoryInst.cabinetId !=null
                        && x.allOtherInventoryInst.cabinet.CabinetPowerLibraryId !=null,x=>x.allOtherInventoryInst,
                        x=>x.allOtherInventoryInst.cabinet).Select(x=>x.allOtherInventoryInst.cabinet).ToList())},

                    };
                  
                    ListAttributesActivated = ListAttributesActivated
                        .Select(FKitem =>
                        {
                            if (repositoryMethods.ContainsKey(FKitem.Label.ToLower()))
                            {
                                FKitem.Options = repositoryMethods[FKitem.Label.ToLower()]().ToList();
                            }
                            else
                            {
                                FKitem.Options = new object[0];
                            }

                            return FKitem;
                        })
                        .ToList();

                    objectInst.InstallationAttributes = ListAttributesActivated;
                    objectInst.OtherInSite = _unitOfWork.AttributeActivatedRepository.GetInstAttributeActivatedGetForAdd(TablesNames.TLIotherInSite.ToString(), null, "allOtherInventoryInstId", "Dismantle", "SiteCode");
                    objectInst.OtherInventoryDistance = _unitOfWork.AttributeActivatedRepository.GetInstAttributeActivatedGetForAdd(TablesNames.TLIotherInventoryDistance.ToString(), null, "allOtherInventoryInstId", "SiteCode");
                    var query = _dbContext.TLIotherInSite
                    .Include(x => x.allOtherInventoryInst).Include(x => x.allOtherInventoryInst.generator).Include(x => x.allOtherInventoryInst.cabinet).
                    Include(x => x.allOtherInventoryInst.solar)
                    .Where(x => x.SiteCode == SiteCode && x.allOtherInventoryInst != null && !x.Dismantle)
                    .ToList();


                    var locationTypeViewModels = query
                     .Select(item => new LocationTypeViewModel
                     {
                         Id = item.allOtherInventoryInstId,
                         Name = $"{_dbContext.MV_GENERATOR_VIEW.FirstOrDefault(x => x.Id == item.allOtherInventoryInst.generatorId)?.Name}" +
                         $" {_dbContext.MV_SOLAR_VIEW.FirstOrDefault(x => x.Id == item.allOtherInventoryInst.solarId)?.Name}" +
                         $" {_dbContext.MV_CABINET_POWER_VIEW.FirstOrDefault(x => x.Id == item.allOtherInventoryInst.cabinetId)?.Name}"
                         + $" {_dbContext.MV_CABINET_TELECOM_VIEW.FirstOrDefault(x => x.Id == item.allOtherInventoryInst.cabinetId)?.Name}".Trim()
                     })
                     .ToList();


                    objectInst.OtherInventoryDistance = objectInst.OtherInventoryDistance.Select(x =>
                    {
                        if (x.Label.ToLower() == "referenceotherinventory_name")
                        {
                            x.Options = locationTypeViewModels.ToList();
                        }
                        return x;
                    });
                    objectInst.DynamicAttribute = _unitOfWork.DynamicAttRepository
                    .GetDynamicInstAttInst(TableNameEntity.Id, null);
                    if (ExternalSys == true)
                    {
                        TLIhistory tLIhistory = new TLIhistory()
                        {
                            TablesNameId = TableNameEntity.Id,
                            ExternalSysId = UserId,
                            HistoryTypeId = 4,
                        };
                        _dbContext.TLIhistory.Add(tLIhistory);
                        _dbContext.SaveChanges();
                    }
                    return new Response<GetForAddOtherInventoryInstallationObject>(true, objectInst, null, null, (int)Helpers.Constants.ApiReturnCode.fail);
                }
                else
                {
                    return new Response<GetForAddOtherInventoryInstallationObject>(false, null, null, "this solarlibrary is not found", (int)Helpers.Constants.ApiReturnCode.fail);
                }
                return new Response<GetForAddOtherInventoryInstallationObject>(false, null, null, null, (int)Helpers.Constants.ApiReturnCode.fail);
            }
            catch (Exception err)
            {
                return new Response<GetForAddOtherInventoryInstallationObject>(false, null, null, err.Message, (int)ApiReturnCode.fail);
            }
        }
        public Response<GetForAddOtherInventoryInstallationObject> GetAttForAddCabinetTelecomInstallation(string TableName, int LibraryID, string SiteCode)
        {
            try
            {
                TLItablesNames TableNameEntity = _unitOfWork.TablesNamesRepository.GetWhereFirst(x =>
                    x.TableName == TableName);

                GetForAddOtherInventoryInstallationObject objectInst = new GetForAddOtherInventoryInstallationObject();
                List<BaseInstAttViews> ListAttributesActivated = new List<BaseInstAttViews>();

                EditCabinetTelecomLibraryAttributes CabinetTelecomLibrary = _mapper.Map<EditCabinetTelecomLibraryAttributes>(_unitOfWork.CabinetTelecomLibraryRepository
                    .GetIncludeWhereFirst(x => x.Id == LibraryID));
                if (CabinetTelecomLibrary != null)
                {
                    List<BaseInstAttViews> LibraryAttributes = _unitOfWork.AttributeActivatedRepository
                        .GetAttributeActivatedGetForAdd(TablesNames.TLIcabinetTelecomLibrary.ToString(), CabinetTelecomLibrary, null).ToList();
                    var telecomtype_name = LibraryAttributes.FirstOrDefault(item => item.Label.ToLower() == "telecomtype_name");
                    if (telecomtype_name != null)
                    {
                        telecomtype_name.Options = _mapper.Map<List<TelecomTypeViewModel>>(_unitOfWork.TelecomTypeRepository.GetWhere(x => !x.Deleted && !x.Disable).ToList());
                        telecomtype_name.Value = _unitOfWork.TelecomTypeRepository != null && CabinetTelecomLibrary.TelecomTypeId != null ?
                            _mapper.Map<TelecomTypeViewModel>(_unitOfWork.TelecomTypeRepository.GetWhereFirst(x => x.Id == CabinetTelecomLibrary.TelecomTypeId)) :
                            null;
                    }
                    List<BaseInstAttViews> LogisticalAttributes = _mapper.Map<List<BaseInstAttViews>>(_unitOfWork.LogistcalRepository
                        .GetLogisticals(TablePartName.OtherInventory.ToString(), Helpers.Constants.TablesNames.TLIcabinetTelecomLibrary.ToString(), CabinetTelecomLibrary.Id).ToList());

                    LibraryAttributes.AddRange(LogisticalAttributes);

                    objectInst.LibraryAttribute = LibraryAttributes;

                    ListAttributesActivated = _unitOfWork.AttributeActivatedRepository.
                        GetInstAttributeActivatedGetForAdd(OtherInventoryType.TLIcabinet.ToString(), null, "CabinetTelecomLibraryId", "CabinetPowerLibraryId").ToList();

                    BaseInstAttViews NameAttribute = ListAttributesActivated.FirstOrDefault(x => x.Key.ToLower() == "Name".ToLower());
                    if (NameAttribute != null)
                    {
                        BaseInstAttViews Swap = ListAttributesActivated[0];
                        ListAttributesActivated[ListAttributesActivated.IndexOf(NameAttribute)] = Swap;
                        ListAttributesActivated[0] = NameAttribute;
                    }


                    Dictionary<string, Func<IEnumerable<object>>> repositoryMethods = new Dictionary<string, Func<IEnumerable<object>>>
                    {
                        { "renewablecabinettype_name", () => _mapper.Map<List<RenewableCabinetTypeViewModel>>(_unitOfWork.RenewableCabinetTypeRepository.GetWhereAndInclude(x => !x.Deleted &&
                        !x.Disable).ToList())},

                    };

                    ListAttributesActivated = ListAttributesActivated
                        .Select(FKitem =>
                        {
                            if (repositoryMethods.ContainsKey(FKitem.Label.ToLower()))
                            {
                                FKitem.Options = repositoryMethods[FKitem.Label.ToLower()]().ToList();
                            }
                            else
                            {
                                FKitem.Options = new object[0];
                            }

                            return FKitem;
                        })
                        .ToList();

                    objectInst.InstallationAttributes = ListAttributesActivated;
                    objectInst.OtherInSite = _unitOfWork.AttributeActivatedRepository.GetInstAttributeActivatedGetForAdd(TablesNames.TLIotherInSite.ToString(), null, "allOtherInventoryInstId", "Dismantle", "SiteCode");
                    objectInst.OtherInventoryDistance = _unitOfWork.AttributeActivatedRepository.GetInstAttributeActivatedGetForAdd(TablesNames.TLIotherInventoryDistance.ToString(), null, "allOtherInventoryInstId", "SiteCode");
                    var query = _dbContext.TLIotherInSite
                    .Include(x => x.allOtherInventoryInst).Include(x => x.allOtherInventoryInst.generator).Include(x => x.allOtherInventoryInst.cabinet).
                    Include(x => x.allOtherInventoryInst.solar)
                    .Where(x => x.SiteCode == SiteCode && x.allOtherInventoryInst != null && !x.Dismantle)
                    .ToList();


                    var locationTypeViewModels = query
                     .Select(item => new LocationTypeViewModel
                     {
                         Id = item.allOtherInventoryInstId,
                         Name = $"{_dbContext.MV_GENERATOR_VIEW.FirstOrDefault(x => x.Id == item.allOtherInventoryInst.generatorId)?.Name}" +
                         $" {_dbContext.MV_SOLAR_VIEW.FirstOrDefault(x => x.Id == item.allOtherInventoryInst.solarId)?.Name}" +
                         $" {_dbContext.MV_CABINET_POWER_VIEW.FirstOrDefault(x => x.Id == item.allOtherInventoryInst.cabinetId)?.Name}"
                         + $" {_dbContext.MV_CABINET_TELECOM_VIEW.FirstOrDefault(x => x.Id == item.allOtherInventoryInst.cabinetId)?.Name}".Trim()
                     })
                     .ToList();


                    objectInst.OtherInventoryDistance = objectInst.OtherInventoryDistance.Select(x =>
                    {
                        if (x.Label.ToLower() == "referenceotherinventory_name")
                        {
                            x.Options = locationTypeViewModels.ToList();
                        }
                        return x;
                    });
                    objectInst.DynamicAttribute = _unitOfWork.DynamicAttRepository
                    .GetDynamicInstAttInst(TableNameEntity.Id, null);
                    return new Response<GetForAddOtherInventoryInstallationObject>(true, objectInst, null, null, (int)Helpers.Constants.ApiReturnCode.fail);
                }
                else
                {
                    return new Response<GetForAddOtherInventoryInstallationObject>(false, null, null, "this cabinetTelecomlibrary is not found", (int)Helpers.Constants.ApiReturnCode.fail);
                }
                return new Response<GetForAddOtherInventoryInstallationObject>(false, null, null, null, (int)Helpers.Constants.ApiReturnCode.fail);
            }
            catch (Exception err)
            {
                return new Response<GetForAddOtherInventoryInstallationObject>(false, null, null, err.Message, (int)ApiReturnCode.fail);
            }
        }
        public Response<GetForAddOtherInventoryInstallationObject> GetAttForAddCabinetPowerInstallation(string TableName, int LibraryID, string SiteCode)
        {
            try
            {
                TLItablesNames TableNameEntity = _unitOfWork.TablesNamesRepository.GetWhereFirst(x =>
                    x.TableName == TableName);

                GetForAddOtherInventoryInstallationObject objectInst = new GetForAddOtherInventoryInstallationObject();
                List<BaseInstAttViews> ListAttributesActivated = new List<BaseInstAttViews>();

                EditCabinetPowerLibraryAttributes CabinetPowerLibrary = _mapper.Map<EditCabinetPowerLibraryAttributes>(_unitOfWork.CabinetPowerLibraryRepository
                    .GetIncludeWhereFirst(x => x.Id == LibraryID));
                if (CabinetPowerLibrary != null)
                {
                    List<BaseInstAttViews> LibraryAttributes = _unitOfWork.AttributeActivatedRepository
                        .GetAttributeActivatedGetForAdd(TablesNames.TLIcabinetPowerLibrary.ToString(), CabinetPowerLibrary, null).ToList();
                    var cabinetpowertype_name = LibraryAttributes.FirstOrDefault(item => item.Label.ToLower() == "cabinetpowertype_name");
                    if (cabinetpowertype_name != null)
                    {
                        cabinetpowertype_name.Options = _mapper.Map<List<CabinetPowerTypeViewModel>>(_unitOfWork.CabinetPowerTypeRepository.GetWhere(x => !x.Delete && !x.Disable).ToList());
                        cabinetpowertype_name.Value = _unitOfWork.CabinetPowerTypeRepository != null && CabinetPowerLibrary.CabinetPowerTypeId != null ?
                            _mapper.Map<CabinetPowerTypeViewModel>(_unitOfWork.CabinetPowerTypeRepository.GetWhereFirst(x => x.Id == CabinetPowerLibrary.CabinetPowerTypeId)) :
                            null;
                    }
                    var integratedwith = LibraryAttributes.FirstOrDefault(item => item.Label.ToLower() == "integratedwith");
                    if (integratedwith != null)
                    {
                        List<EnumOutPut> integratedwiths = new List<EnumOutPut>();
                        integratedwiths.Add(new EnumOutPut
                        {
                            Id = (int)IntegratedWith.Solar,
                            Name = IntegratedWith.Solar.ToString()
                        });
                        integratedwiths.Add(new EnumOutPut
                        {
                            Id = (int)IntegratedWith.Wind,
                            Name = IntegratedWith.Wind.ToString()
                        });
                        if (CabinetPowerLibrary.IntegratedWith != null)
                        {
                            integratedwith.Options = integratedwiths;
                            integratedwith.Value = integratedwiths.FirstOrDefault(shape => shape.Id == (int)CabinetPowerLibrary.IntegratedWith);
                        }
                        else
                        {
                            integratedwith.Options = integratedwiths;
                            integratedwith.Value = null;

                        }

                    }
                    LibraryAttributes = LibraryAttributes.Select(x =>
                    {
                        if (x.Label.ToLower() == "powerintegrated" && (x.Value as bool?) == false)
                        {
                            LibraryAttributes.ForEach(y =>
                            {
                                if (y.Label.ToLower() == "integratedwith")
                                {
                                    y.visible = false;
                                }
                            });
                        }
                        return x;
                    }).ToList();

                    List<BaseInstAttViews> LogisticalAttributes = _mapper.Map<List<BaseInstAttViews>>(_unitOfWork.LogistcalRepository
                        .GetLogisticals(TablePartName.OtherInventory.ToString(), Helpers.Constants.TablesNames.TLIcabinetPowerLibrary.ToString(), CabinetPowerLibrary.Id).ToList());

                    LibraryAttributes.AddRange(LogisticalAttributes);

                    objectInst.LibraryAttribute = LibraryAttributes;

                    ListAttributesActivated = _unitOfWork.AttributeActivatedRepository.
                        GetInstAttributeActivatedGetForAdd(OtherInventoryType.TLIcabinet.ToString(), null, "CabinetTelecomLibraryId", "CabinetPowerLibraryId").ToList();

                    BaseInstAttViews NameAttribute = ListAttributesActivated.FirstOrDefault(x => x.Key.ToLower() == "Name".ToLower());
                    if (NameAttribute != null)
                    {
                        BaseInstAttViews Swap = ListAttributesActivated[0];
                        ListAttributesActivated[ListAttributesActivated.IndexOf(NameAttribute)] = Swap;
                        ListAttributesActivated[0] = NameAttribute;
                    }


                    Dictionary<string, Func<IEnumerable<object>>> repositoryMethods = new Dictionary<string, Func<IEnumerable<object>>>
                    {
                        { "renewablecabinettype_name", () => _mapper.Map<List<RenewableCabinetTypeViewModel>>(_unitOfWork.RenewableCabinetTypeRepository.GetWhereAndInclude(x => !x.Deleted &&
                        !x.Disable).ToList())},
                         { "batterrytype_name", () => _mapper.Map<List<BatterryTypeDto>>(_unitOfWork.BatteryTypeRepository.GetWhereAndInclude(x => !x.Delete &&
                        !x.Disable).ToList())},

                    };

                    ListAttributesActivated = ListAttributesActivated
                        .Select(FKitem =>
                        {
                            if (repositoryMethods.ContainsKey(FKitem.Label.ToLower()))
                            {
                                FKitem.Options = repositoryMethods[FKitem.Label.ToLower()]().ToList();
                            }
                            else
                            {
                                FKitem.Options = new object[0];
                            }

                            return FKitem;
                        })
                        .ToList();

                    objectInst.InstallationAttributes = ListAttributesActivated;
                    objectInst.OtherInSite = _unitOfWork.AttributeActivatedRepository.GetInstAttributeActivatedGetForAdd(TablesNames.TLIotherInSite.ToString(), null, "allOtherInventoryInstId", "Dismantle", "SiteCode");
                    objectInst.OtherInventoryDistance = _unitOfWork.AttributeActivatedRepository.GetInstAttributeActivatedGetForAdd(TablesNames.TLIotherInventoryDistance.ToString(), null, "allOtherInventoryInstId", "SiteCode");
                    var query = _dbContext.TLIotherInSite
                    .Include(x => x.allOtherInventoryInst).Include(x => x.allOtherInventoryInst.generator).Include(x => x.allOtherInventoryInst.cabinet).
                    Include(x => x.allOtherInventoryInst.solar)
                    .Where(x => x.SiteCode == SiteCode && x.allOtherInventoryInst != null && !x.Dismantle)
                    .ToList();

                    var locationTypeViewModels = query
                     .Select(item => new LocationTypeViewModel
                     {
                         Id = item.allOtherInventoryInstId,
                         Name = $"{_dbContext.MV_GENERATOR_VIEW.FirstOrDefault(x => x.Id == item.allOtherInventoryInst.generatorId)?.Name}" +
                         $" {_dbContext.MV_SOLAR_VIEW.FirstOrDefault(x => x.Id == item.allOtherInventoryInst.solarId)?.Name}" +
                         $" {_dbContext.MV_CABINET_POWER_VIEW.FirstOrDefault(x => x.Id == item.allOtherInventoryInst.cabinetId)?.Name}"
                         + $" {_dbContext.MV_CABINET_TELECOM_VIEW.FirstOrDefault(x => x.Id == item.allOtherInventoryInst.cabinetId)?.Name}".Trim()
                     })
                     .ToList();

                    objectInst.OtherInventoryDistance = objectInst.OtherInventoryDistance.Select(x =>
                    {
                        if (x.Label.ToLower() == "referenceotherinventory_name")
                        {
                            x.Options = locationTypeViewModels.ToList();
                        }
                        return x;
                    });
                    objectInst.DynamicAttribute = _unitOfWork.DynamicAttRepository
                    .GetDynamicInstAttInst(TableNameEntity.Id, null);
                    return new Response<GetForAddOtherInventoryInstallationObject>(true, objectInst, null, null, (int)Helpers.Constants.ApiReturnCode.fail);
                }
                else
                {
                    return new Response<GetForAddOtherInventoryInstallationObject>(false, null, null, "this cabinetPowerlibrary is not found", (int)Helpers.Constants.ApiReturnCode.fail);
                }
                return new Response<GetForAddOtherInventoryInstallationObject>(false, null, null, null, (int)Helpers.Constants.ApiReturnCode.fail);
            }
            catch (Exception err)
            {
                return new Response<GetForAddOtherInventoryInstallationObject>(false, null, null, err.Message, (int)ApiReturnCode.fail);
            }
        }
        #endregion    
        #region AddOtherInventoryInstallation
        //Function take 2 parameters model, TableName
        //First get table name Entity by TableName
        //Map model object to ViewModel
        //Map ViewModel to Entity
        //Add Entity
        //Add other inventory to site (Add record to TLIotherInSite)
        //Add other inventory reference if there is other inventory in site before i install my other inventory
        //if i want to this other inventory to reserved space then update reserved space in this site
        #region Helper Methods..
        public void LoopForPath(List<string> Path, int StartIndex, ApplicationDbContext _dbContext, object Value, List<object> OutPutIds)
        {
            if (StartIndex == Path.Count())
            {
                OutPutIds.Add(Value);
            }
            else
            {
                List<object> TableRecords = _mapper.Map<List<object>>(_dbContext.GetType()
                    .GetProperties().FirstOrDefault(x => x.Name.ToLower() == Path[StartIndex].ToLower())
                    .GetValue(_dbContext, null))
                        .Where(x => x.GetType().GetProperty(Path[StartIndex + 1]).GetValue(x, null) != null ?
                            x.GetType().GetProperty(Path[StartIndex + 1]).GetValue(x, null).ToString().ToLower() == Value.ToString().ToLower() : false).ToList();

                foreach (object Record in TableRecords)
                {
                    // The New Value..
                    object PrimaryKeyValue = Record.GetType().GetProperties().FirstOrDefault(x => x.Name.ToLower() == Path[StartIndex + 2].ToLower()).GetValue(Record, null);

                    if (StartIndex + 3 < Path.Count())
                        LoopForPath(Path, StartIndex + 3, _dbContext, PrimaryKeyValue, OutPutIds);

                    else if (StartIndex + 3 == Path.Count())
                        OutPutIds.Add(PrimaryKeyValue);
                }
            }
        }
        public List<object> GetRecordsIds(string MainTableName, AddInstRuleViewModel Rule)
        {
            string SDTableName = Rule.TableName;
            string Operation = _unitOfWork.OperationRepository.GetByID(Rule.OperationId.Value).Name;
            object Value = new object();

            string DataType = "";

            if (Rule.OperationValueBoolean != null)
            {
                DataType = "Bool";
                Value = Rule.OperationValueBoolean;
            }
            else if (Rule.OperationValueDateTime != null)
            {
                DataType = "DateTime";
                Value = Rule.OperationValueDateTime;
            }
            else if (Rule.OperationValueDouble != null)
            {
                DataType = "Double";
                Value = Rule.OperationValueDouble;
            }
            else if (!string.IsNullOrEmpty(Rule.OperationValueString))
            {
                DataType = "String";
                Value = Rule.OperationValueString;
            }

            List<object> OutPutIds = new List<object>();

            PathToCheckDependencyValidation Item = (PathToCheckDependencyValidation)Enum.Parse(typeof(PathToCheckDependencyValidation),
                MainTableName + SDTableName);

            List<string> Path = GetEnumDescription(Item).Split(" ").ToList();

            if (Path[0].ToLower() == MainTableName.ToLower() &&
                Path[1].ToLower() == SDTableName.ToLower())
            {
                List<object> TableRecords = new List<object>();
                if (Rule.attributeActivatedId != null && !Rule.IsDynamic)
                {
                    string AttributeName = _unitOfWork.AttributeActivatedRepository
                        .GetByID(Rule.attributeActivatedId.Value).Key;

                    TableRecords = _mapper.Map<List<object>>(_dbContext.GetType().GetProperty(SDTableName)
                        .GetValue(_dbContext, null)).Where(x => x.GetType().GetProperty(AttributeName).GetValue(x, null) != null ? (Operation == ">" ?
                            (DataType.ToLower() == "DateTime".ToLower() ?
                                Comparer.DefaultInvariant.Compare(DateTime.Parse(x.GetType().GetProperty(AttributeName).GetValue(x, null).ToString()), Value) == 1 :
                            DataType.ToLower() == "Double".ToLower() ?
                                Comparer.DefaultInvariant.Compare(double.Parse(x.GetType().GetProperty(AttributeName).GetValue(x, null).ToString()), Value) == 1 : false) :
                        Operation == ">=" ?
                            (DataType.ToLower() == "DateTime".ToLower() ?
                                (Comparer.DefaultInvariant.Compare(DateTime.Parse(x.GetType().GetProperty(AttributeName).GetValue(x, null).ToString()), Value) == 1 ||
                                    x.GetType().GetProperty(AttributeName).GetValue(x, null).ToString().ToLower() == Value.ToString().ToLower()) :
                            DataType.ToLower() == "Double".ToLower() ?
                                (Comparer.DefaultInvariant.Compare(double.Parse(x.GetType().GetProperty(AttributeName).GetValue(x, null).ToString()), Value) == 1 ||
                                    x.GetType().GetProperty(AttributeName).GetValue(x, null).ToString().ToLower() == Value.ToString().ToLower()) : false) :
                        Operation == "<" ?
                            (DataType.ToLower() == "DateTime".ToLower() ?
                                Comparer.DefaultInvariant.Compare(DateTime.Parse(x.GetType().GetProperty(AttributeName).GetValue(x, null).ToString()), Value) == -1 :
                            DataType.ToLower() == "Double".ToLower() ?
                                Comparer.DefaultInvariant.Compare(double.Parse(x.GetType().GetProperty(AttributeName).GetValue(x, null).ToString()), Value) == -1 : false) :
                        Operation == "<=" ?
                            (DataType.ToLower() == "DateTime".ToLower() ?
                                (Comparer.DefaultInvariant.Compare(DateTime.Parse(x.GetType().GetProperty(AttributeName).GetValue(x, null).ToString()), Value) == -1 ||
                                    x.GetType().GetProperty(AttributeName).GetValue(x, null).ToString().ToLower() == Value.ToString().ToLower()) :
                            DataType.ToLower() == "Double".ToLower() ?
                                (Comparer.DefaultInvariant.Compare(double.Parse(x.GetType().GetProperty(AttributeName).GetValue(x, null).ToString()), Value) == -1 ||
                                    x.GetType().GetProperty(AttributeName).GetValue(x, null).ToString().ToLower() == Value.ToString().ToLower()) : false) :
                        Operation == "==" ?
                            x.GetType().GetProperty(AttributeName).GetValue(x, null).ToString().ToLower() == Value.ToString().ToLower() :
                        Operation == "!=" ?
                            x.GetType().GetProperty(AttributeName).GetValue(x, null).ToString().ToLower() != Value.ToString().ToLower() : false) : false).ToList();

                    foreach (object Record in TableRecords)
                    {
                        object PrimaryKeyValue = Record.GetType().GetProperty(Path[2]).GetValue(Record, null);

                        LoopForPath(Path, 3, _dbContext, PrimaryKeyValue, OutPutIds);
                    }
                }
                else if (Rule.dynamicAttId != null && Rule.IsDynamic)
                {
                    TLIdynamicAtt DynamicAttribute = _unitOfWork.DynamicAttRepository
                        .GetByID(Rule.dynamicAttId.Value);

                    if (!DynamicAttribute.LibraryAtt)
                    {
                        List<TLIdynamicAttInstValue> DynamicAttValues = new List<TLIdynamicAttInstValue>();

                        if (Rule.OperationValueBoolean != null)
                        {
                            DynamicAttValues = _unitOfWork.DynamicAttInstValueRepository
                                .GetWhere(x => (x.DynamicAttId == Rule.dynamicAttId && !x.disable) &&
                                    (x.ValueBoolean != null ? x.ValueBoolean.ToString().ToLower() == Rule.OperationValueBoolean.ToString().ToLower() : false)).ToList();
                        }
                        else if (Rule.OperationValueDateTime != null)
                        {
                            DynamicAttValues = _unitOfWork.DynamicAttInstValueRepository
                                .GetWhere(x => (x.DynamicAttId == Rule.dynamicAttId && !x.disable) &&
                                    (x.ValueDateTime != null ? x.ValueDateTime.ToString().ToLower() == Rule.OperationValueDateTime.ToString().ToLower() : false)).ToList();
                        }
                        else if (Rule.OperationValueDouble != null)
                        {
                            DynamicAttValues = _unitOfWork.DynamicAttInstValueRepository
                                .GetWhere(x => (x.DynamicAttId == Rule.dynamicAttId && !x.disable) &&
                                    (x.ValueDouble != null ? x.ValueDouble == Rule.OperationValueDouble : false)).ToList();
                        }
                        else if (!string.IsNullOrEmpty(Rule.OperationValueString))
                        {
                            DynamicAttValues = _unitOfWork.DynamicAttInstValueRepository
                               .GetWhere(x => (x.DynamicAttId == Rule.dynamicAttId && !x.disable) &&
                                   (!string.IsNullOrEmpty(x.ValueString) ? x.ValueString.ToLower() == Rule.OperationValueString.ToLower() : false)).ToList();
                        }
                        if (DynamicAttValues != null ? DynamicAttValues.Count() > 0 : false)
                        {
                            TableRecords = _mapper.Map<List<object>>(_dbContext.GetType()
                                .GetProperty(Path[1]).GetValue(_dbContext, null))
                                    .Where(x => DynamicAttValues.FirstOrDefault(y =>
                                        y.InventoryId.ToString() == x.GetType().GetProperty("Id").GetValue(x, null).ToString() ? (
                                            (y.ValueBoolean != null ? (
                                                Operation == "==" ? y.ValueBoolean.ToString().ToLower() == Value.ToString().ToLower() :
                                                Operation == "!=" ? y.ValueBoolean.ToString().ToLower() != Value.ToString().ToLower() : false
                                            ) : false) ||
                                            (y.ValueDateTime != null ? (
                                                Operation == "==" ? y.ValueDateTime.ToString().ToLower() == Value.ToString().ToLower() :
                                                Operation == ">" ? Comparer.DefaultInvariant.Compare(y.ValueDateTime, Value) == 1 :
                                                Operation == ">=" ? (Comparer.DefaultInvariant.Compare(y.ValueDateTime, Value) == 1 ||
                                                    y.ValueDateTime.ToString().ToLower() == Value.ToString().ToLower()) :
                                                Operation == "<" ? Comparer.DefaultInvariant.Compare(y.ValueDateTime, Value) == -1 :
                                                Operation == "<=" ? (Comparer.DefaultInvariant.Compare(y.ValueDateTime, Value) == -1 ||
                                                    y.ValueDateTime.ToString().ToLower() == Value.ToString().ToLower()) :
                                                Operation == "!=" ? y.ValueDateTime.ToString().ToLower() != Value.ToString().ToLower() : false
                                            ) : false) ||
                                            (y.ValueDouble != null ? (
                                                Operation == "==" ? y.ValueDouble.ToString().ToLower() == Value.ToString().ToLower() :
                                                Operation == ">" ? Comparer.DefaultInvariant.Compare(y.ValueDouble, Value) == 1 :
                                                Operation == ">=" ? (Comparer.DefaultInvariant.Compare(y.ValueDouble, Value) == 1 ||
                                                    y.ValueDouble.ToString().ToLower() == Value.ToString().ToLower()) :
                                                Operation == "<" ? Comparer.DefaultInvariant.Compare(y.ValueDouble, Value) == -1 :
                                                Operation == "<=" ? (Comparer.DefaultInvariant.Compare(y.ValueDouble, Value) == -1 ||
                                                    y.ValueDouble.ToString().ToLower() == Value.ToString().ToLower()) :
                                                Operation == "!=" ? y.ValueDouble.ToString().ToLower() != Value.ToString().ToLower() : false
                                            ) : false) ||
                                            (y.ValueString != null ? (
                                                Operation == "==" ? y.ValueString.ToLower() == Value.ToString().ToLower() :
                                                Operation == "!=" ? y.ValueString.ToLower() != Value.ToString().ToLower() : false
                                            ) : false)
                                        ) : false) != null).ToList();

                            foreach (object Record in TableRecords)
                            {
                                object PrimaryKeyValue = Record.GetType().GetProperty(Path[2]).GetValue(Record, null);

                                LoopForPath(Path, 3, _dbContext, PrimaryKeyValue, OutPutIds);
                            }
                        }
                    }
                    else
                    {
                        List<TLIdynamicAttLibValue> DynamicAttValues = new List<TLIdynamicAttLibValue>();

                        if (Rule.OperationValueBoolean != null)
                        {
                            DynamicAttValues = _unitOfWork.DynamicAttLibRepository
                                .GetWhere(x => (x.DynamicAttId == Rule.dynamicAttId && !x.disable) &&
                                    (x.ValueBoolean != null ? x.ValueBoolean.ToString().ToLower() == Rule.OperationValueBoolean.ToString().ToLower() : false)).ToList();
                        }
                        else if (Rule.OperationValueDateTime != null)
                        {
                            DynamicAttValues = _unitOfWork.DynamicAttLibRepository
                                .GetWhere(x => (x.DynamicAttId == Rule.dynamicAttId && !x.disable) &&
                                    (x.ValueDateTime != null ? x.ValueDateTime.ToString().ToLower() == Rule.OperationValueDateTime.ToString().ToLower() : false)).ToList();
                        }
                        else if (Rule.OperationValueDouble != null)
                        {
                            DynamicAttValues = _unitOfWork.DynamicAttLibRepository
                                .GetWhere(x => (x.DynamicAttId == Rule.dynamicAttId && !x.disable) &&
                                    (x.ValueDouble != null ? x.ValueDouble == Rule.OperationValueDouble : false)).ToList();
                        }
                        else if (!string.IsNullOrEmpty(Rule.OperationValueString))
                        {
                            DynamicAttValues = _unitOfWork.DynamicAttLibRepository
                               .GetWhere(x => (x.DynamicAttId == Rule.dynamicAttId && !x.disable) &&
                                   (!string.IsNullOrEmpty(x.ValueString) ? x.ValueString.ToLower() == Rule.OperationValueString.ToLower() : false)).ToList();
                        }
                        if (DynamicAttValues != null ? DynamicAttValues.Count() > 0 : false)
                        {
                            TableRecords = _mapper.Map<List<object>>(_dbContext.GetType()
                                .GetProperty(Path[1]).GetValue(_dbContext, null))
                                    .Where(x => DynamicAttValues.FirstOrDefault(y =>
                                        y.InventoryId.ToString() == x.GetType().GetProperty("Id").GetValue(x, null).ToString() ? (
                                            (y.ValueBoolean != null ? (
                                                Operation == "==" ? y.ValueBoolean.ToString().ToLower() == Value.ToString().ToLower() :
                                                Operation == "!=" ? y.ValueBoolean.ToString().ToLower() != Value.ToString().ToLower() : false
                                            ) : false) ||
                                            (y.ValueDateTime != null ? (
                                                Operation == "==" ? y.ValueDateTime.ToString().ToLower() == Value.ToString().ToLower() :
                                                Operation == ">" ? Comparer.DefaultInvariant.Compare(y.ValueDateTime, Value) == 1 :
                                                Operation == ">=" ? (Comparer.DefaultInvariant.Compare(y.ValueDateTime, Value) == 1 ||
                                                    y.ValueDateTime.ToString().ToLower() == Value.ToString().ToLower()) :
                                                Operation == "<" ? Comparer.DefaultInvariant.Compare(y.ValueDateTime, Value) == -1 :
                                                Operation == "<=" ? (Comparer.DefaultInvariant.Compare(y.ValueDateTime, Value) == -1 ||
                                                    y.ValueDateTime.ToString().ToLower() == Value.ToString().ToLower()) :
                                                Operation == "!=" ? y.ValueDateTime.ToString().ToLower() != Value.ToString().ToLower() : false
                                            ) : false) ||
                                            (y.ValueDouble != null ? (
                                                Operation == "==" ? y.ValueDouble.ToString().ToLower() == Value.ToString().ToLower() :
                                                Operation == ">" ? Comparer.DefaultInvariant.Compare(y.ValueDouble, Value) == 1 :
                                                Operation == ">=" ? (Comparer.DefaultInvariant.Compare(y.ValueDouble, Value) == 1 ||
                                                    y.ValueDouble.ToString().ToLower() == Value.ToString().ToLower()) :
                                                Operation == "<" ? Comparer.DefaultInvariant.Compare(y.ValueDouble, Value) == -1 :
                                                Operation == "<=" ? (Comparer.DefaultInvariant.Compare(y.ValueDouble, Value) == -1 ||
                                                    y.ValueDouble.ToString().ToLower() == Value.ToString().ToLower()) :
                                                Operation == "!=" ? y.ValueDouble.ToString().ToLower() != Value.ToString().ToLower() : false
                                            ) : false) ||
                                            (y.ValueString != null ? (
                                                Operation == "==" ? y.ValueString.ToLower() == Value.ToString().ToLower() :
                                                Operation == "!=" ? y.ValueString.ToLower() != Value.ToString().ToLower() : false
                                            ) : false)
                                        ) : false) != null).ToList();

                            foreach (object Record in TableRecords)
                            {
                                object PrimaryKeyValue = Record.GetType().GetProperty(Path[2]).GetValue(Record, null);

                                LoopForPath(Path, 3, _dbContext, PrimaryKeyValue, OutPutIds);
                            }
                        }
                    }
                }
            }
            OutPutIds = OutPutIds.Distinct().ToList();
            return OutPutIds;
        }
        public string CheckDependencyValidation(object Input, string OtherInventoryType, string SiteCode)
        {
            if (OtherInventoryType.ToLower() == TablesNames.TLIcabinet.ToString().ToLower())
            {
                string MainTableName = TablesNames.TLIcabinet.ToString();
                AddCabinetViewModel AddInstallationViewModel = _mapper.Map<AddCabinetViewModel>(Input);

                List<DynamicAttViewModel> DynamicAttributes = _mapper.Map<List<DynamicAttViewModel>>(_unitOfWork.DynamicAttRepository
                    .GetIncludeWhere(x => x.tablesNames.TableName.ToLower() == MainTableName.ToLower() && !x.disable
                        , x => x.tablesNames).ToList());

                foreach (DynamicAttViewModel DynamicAttribute in DynamicAttributes)
                {
                    TLIdependency DynamicAttributeMainDependency = _unitOfWork.DependencieRepository.GetIncludeWhereFirst(x => x.DynamicAttId == DynamicAttribute.Id &&
                        (x.ValueBoolean != null || x.ValueDateTime != null || x.ValueDouble != null || !string.IsNullOrEmpty(x.ValueString)) &&
                            x.OperationId != null, x => x.Operation);

                    if (DynamicAttributeMainDependency == null)
                        continue;

                    List<int> DependencyRows = _unitOfWork.DependencyRowRepository.GetWhere(x => x.DependencyId == DynamicAttributeMainDependency.Id)
                        .Select(x => x.RowId.Value).Distinct().ToList();

                    foreach (int RowId in DependencyRows)
                    {
                        List<TLIrule> Rules = _unitOfWork.RowRuleRepository.GetIncludeWhere(x => x.RowId == RowId, x => x.Rule, x => x.Rule.tablesNames,
                            x => x.Rule.Operation, x => x.Rule.dynamicAtt, x => x.Rule.attributeActivated).Select(x => x.Rule).ToList();

                        int CheckIfSuccessAllRules = 0;

                        foreach (TLIrule Rule in Rules)
                        {
                            string SDTableName = Rule.tablesNames.TableName;

                            string DataType = "";

                            string Operation = Rule.Operation.Name;
                            object OperationValue = new object();

                            if (Rule.OperationValueBoolean != null)
                            {
                                DataType = "Bool";
                                OperationValue = Rule.OperationValueBoolean;
                            }
                            else if (Rule.OperationValueDateTime != null)
                            {
                                DataType = "DateTime";
                                OperationValue = Rule.OperationValueDateTime;
                            }
                            else if (Rule.OperationValueDouble != null)
                            {
                                DataType = "Double";
                                OperationValue = Rule.OperationValueDouble;
                            }
                            else if (!string.IsNullOrEmpty(Rule.OperationValueString))
                            {
                                DataType = "String";
                                OperationValue = Rule.OperationValueString;
                            }

                            if (MainTableName.ToLower() == SDTableName.ToLower())
                            {
                                object InsertedValue = new object();

                                if (Rule.attributeActivatedId != null)
                                {
                                    string AttributeName = Rule.attributeActivated.Key;

                                    object TestValue = AddInstallationViewModel.GetType().GetProperties()
                                        .FirstOrDefault(x => x.Name.ToLower() == AttributeName.ToLower()).GetValue(AddInstallationViewModel, null);

                                    if (TestValue == null)
                                        break;

                                    if (Rule.OperationValueBoolean != null)
                                        InsertedValue = bool.Parse(TestValue.ToString());

                                    else if (Rule.OperationValueDateTime != null)
                                        InsertedValue = DateTime.Parse(TestValue.ToString());

                                    else if (Rule.OperationValueDouble != null)
                                        InsertedValue = double.Parse(TestValue.ToString());

                                    else if (!string.IsNullOrEmpty(Rule.OperationValueString))
                                        InsertedValue = TestValue.ToString();
                                }
                                else if (Rule.dynamicAttId != null)
                                {
                                    AddDynamicAttInstValueViewModel DynamicObject = AddInstallationViewModel.TLIdynamicAttInstValue
                                        .FirstOrDefault(x => x.DynamicAttId == Rule.dynamicAttId.Value);

                                    if (DynamicObject == null)
                                        break;

                                    if (DynamicObject.ValueBoolean != null)
                                        InsertedValue = DynamicObject.ValueBoolean;

                                    else if (DynamicObject.ValueDateTime != null)
                                        InsertedValue = DynamicObject.ValueDateTime;

                                    else if (DynamicObject.ValueDouble != null)
                                        InsertedValue = DynamicObject.ValueDouble;

                                    else if (!string.IsNullOrEmpty(DynamicObject.ValueString))
                                        InsertedValue = DynamicObject.ValueString;
                                }

                                if (Operation == "==" ? InsertedValue.ToString().ToLower() == OperationValue.ToString().ToLower() :
                                    Operation == "!=" ? InsertedValue.ToString().ToLower() != OperationValue.ToString().ToLower() :
                                    Operation == ">" ? Comparer.DefaultInvariant.Compare(InsertedValue, OperationValue) == 1 :
                                    Operation == ">=" ? (Comparer.DefaultInvariant.Compare(InsertedValue, OperationValue) == 1 ||
                                        InsertedValue.ToString().ToLower() == OperationValue.ToString().ToLower()) :
                                    Operation == "<" ? Comparer.DefaultInvariant.Compare(InsertedValue, OperationValue) == -1 :
                                    Operation == "<=" ? (Comparer.DefaultInvariant.Compare(InsertedValue, OperationValue) == -1 ||
                                        InsertedValue.ToString().ToLower() == OperationValue.ToString().ToLower()) : false)
                                {
                                    CheckIfSuccessAllRules++;
                                }
                            }
                            else
                            {
                                List<object> TableRecords = new List<object>();
                                if (Rule.attributeActivatedId != null)
                                {
                                    string AttributeName = Rule.attributeActivated.Key;

                                    if (OperationValue != null)
                                        TableRecords = _mapper.Map<List<object>>(_dbContext.GetType().GetProperty(SDTableName)
                                            .GetValue(_dbContext, null)).Where(x => x.GetType().GetProperty(AttributeName).GetValue(x, null) != null ? (Operation == ">" ?
                                                (DataType.ToLower() == "DateTime".ToLower() ?
                                                    Comparer.DefaultInvariant.Compare(DateTime.Parse(x.GetType().GetProperty(AttributeName).GetValue(x, null).ToString()), OperationValue) == 1 :
                                                DataType.ToLower() == "Double".ToLower() ?
                                                    Comparer.DefaultInvariant.Compare(double.Parse(x.GetType().GetProperty(AttributeName).GetValue(x, null).ToString()), OperationValue) == 1 : false) :
                                            Operation == ">=" ?
                                                (DataType.ToLower() == "DateTime".ToLower() ?
                                                    (Comparer.DefaultInvariant.Compare(DateTime.Parse(x.GetType().GetProperty(AttributeName).GetValue(x, null).ToString()), OperationValue) == 1 ||
                                                        x.GetType().GetProperty(AttributeName).GetValue(x, null).ToString().ToLower() == OperationValue.ToString().ToLower()) :
                                                DataType.ToLower() == "Double".ToLower() ?
                                                    (Comparer.DefaultInvariant.Compare(double.Parse(x.GetType().GetProperty(AttributeName).GetValue(x, null).ToString()), OperationValue) == 1 ||
                                                        x.GetType().GetProperty(AttributeName).GetValue(x, null).ToString().ToLower() == OperationValue.ToString().ToLower()) : false) :
                                            Operation == "<" ?
                                                (DataType.ToLower() == "DateTime".ToLower() ?
                                                    Comparer.DefaultInvariant.Compare(DateTime.Parse(x.GetType().GetProperty(AttributeName).GetValue(x, null).ToString()), OperationValue) == -1 :
                                                DataType.ToLower() == "Double".ToLower() ?
                                                    Comparer.DefaultInvariant.Compare(double.Parse(x.GetType().GetProperty(AttributeName).GetValue(x, null).ToString()), OperationValue) == -1 : false) :
                                            Operation == "<=" ?
                                                (DataType.ToLower() == "DateTime".ToLower() ?
                                                    (Comparer.DefaultInvariant.Compare(DateTime.Parse(x.GetType().GetProperty(AttributeName).GetValue(x, null).ToString()), OperationValue) == -1 ||
                                                        x.GetType().GetProperty(AttributeName).GetValue(x, null).ToString().ToLower() == OperationValue.ToString().ToLower()) :
                                                DataType.ToLower() == "Double".ToLower() ?
                                                    (Comparer.DefaultInvariant.Compare(double.Parse(x.GetType().GetProperty(AttributeName).GetValue(x, null).ToString()), OperationValue) == -1 ||
                                                        x.GetType().GetProperty(AttributeName).GetValue(x, null).ToString().ToLower() == OperationValue.ToString().ToLower()) : false) :
                                            Operation == "==" ?
                                                x.GetType().GetProperty(AttributeName).GetValue(x, null).ToString().ToLower() == OperationValue.ToString().ToLower() :
                                            Operation == "!=" ?
                                                x.GetType().GetProperty(AttributeName).GetValue(x, null).ToString().ToLower() != OperationValue.ToString().ToLower() : false) : false).ToList();

                                }
                                else if (Rule.dynamicAttId != null)
                                {
                                    List<int> DynamicAttValuesInventoryIds = new List<int>();

                                    if (!DynamicAttribute.LibraryAtt)
                                    {
                                        DynamicAttValuesInventoryIds = _unitOfWork.DynamicAttInstValueRepository
                                            .GetWhere(x => (x.DynamicAttId == Rule.dynamicAttId.Value && !x.disable) &&
                                                (Operation == "==" ?
                                                    ((Rule.OperationValueBoolean != null ? x.ValueBoolean.ToString().ToLower() == Rule.OperationValueBoolean.ToString().ToLower() : false) ||
                                                    (Rule.OperationValueDateTime != null ? x.ValueDateTime.ToString().ToLower() == Rule.OperationValueDateTime.ToString().ToLower() : false) ||
                                                    (Rule.OperationValueDouble != null ? x.ValueDouble == Rule.OperationValueDouble : false) ||
                                                    (!string.IsNullOrEmpty(Rule.OperationValueString) ? x.ValueString.ToLower() == Rule.OperationValueString.ToLower() : false)) : false) ||

                                                (Operation == "!=" ?
                                                    ((Rule.OperationValueBoolean != null ? x.ValueBoolean.ToString().ToLower() != Rule.OperationValueBoolean.ToString().ToLower() : false) ||
                                                    (Rule.OperationValueDateTime != null ? x.ValueDateTime.ToString().ToLower() != Rule.OperationValueDateTime.ToString().ToLower() : false) ||
                                                    (Rule.OperationValueDouble != null ? x.ValueDouble != Rule.OperationValueDouble : false) ||
                                                    (!string.IsNullOrEmpty(Rule.OperationValueString) ? x.ValueString.ToLower() != Rule.OperationValueString.ToLower() : false)) : false) ||

                                                (Operation == ">" ?
                                                    ((Rule.OperationValueDateTime != null ? x.ValueDateTime > Rule.OperationValueDateTime : false) ||
                                                    (Rule.OperationValueDouble != null ? x.ValueDouble > Rule.OperationValueDouble : false)) : false) ||

                                                (Operation == ">=" ?
                                                    ((Rule.OperationValueDateTime != null ? x.ValueDateTime >= Rule.OperationValueDateTime : false) ||
                                                    (Rule.OperationValueDouble != null ? x.ValueDouble >= Rule.OperationValueDouble : false)) : false) ||

                                                (Operation == "<" ?
                                                    ((Rule.OperationValueDateTime != null ? x.ValueDateTime < Rule.OperationValueDateTime : false) ||
                                                    (Rule.OperationValueDouble != null ? x.ValueDouble < Rule.OperationValueDouble : false)) : false) ||

                                                (Operation == "<=" ?
                                                    ((Rule.OperationValueDateTime != null ? x.ValueDateTime <= Rule.OperationValueDateTime : false) ||
                                                    (Rule.OperationValueDouble != null ? x.ValueDouble <= Rule.OperationValueDouble : false)) : false)

                                                ).Select(x => x.InventoryId).ToList();
                                    }
                                    else
                                    {
                                        DynamicAttValuesInventoryIds = _unitOfWork.DynamicAttLibRepository
                                            .GetWhere(x => (x.DynamicAttId == Rule.dynamicAttId && !x.disable) &&
                                                (Operation == "==" ?
                                                    ((Rule.OperationValueBoolean != null ? x.ValueBoolean.ToString().ToLower() == Rule.OperationValueBoolean.ToString().ToLower() : false) ||
                                                    (Rule.OperationValueDateTime != null ? x.ValueDateTime.ToString().ToLower() == Rule.OperationValueDateTime.ToString().ToLower() : false) ||
                                                    (Rule.OperationValueDouble != null ? x.ValueDouble == Rule.OperationValueDouble : false) ||
                                                    (!string.IsNullOrEmpty(Rule.OperationValueString) ? x.ValueString.ToLower() == Rule.OperationValueString.ToLower() : false)) : false) ||

                                                (Operation == "!=" ?
                                                    ((Rule.OperationValueBoolean != null ? x.ValueBoolean.ToString().ToLower() != Rule.OperationValueBoolean.ToString().ToLower() : false) ||
                                                    (Rule.OperationValueDateTime != null ? x.ValueDateTime.ToString().ToLower() != Rule.OperationValueDateTime.ToString().ToLower() : false) ||
                                                    (Rule.OperationValueDouble != null ? x.ValueDouble != Rule.OperationValueDouble : false) ||
                                                    (!string.IsNullOrEmpty(Rule.OperationValueString) ? x.ValueString.ToLower() != Rule.OperationValueString.ToLower() : false)) : false) ||

                                                (Operation == ">" ?
                                                    ((Rule.OperationValueDateTime != null ? x.ValueDateTime > Rule.OperationValueDateTime : false) ||
                                                    (Rule.OperationValueDouble != null ? x.ValueDouble > Rule.OperationValueDouble : false)) : false) ||

                                                (Operation == ">=" ?
                                                    ((Rule.OperationValueDateTime != null ? x.ValueDateTime >= Rule.OperationValueDateTime : false) ||
                                                    (Rule.OperationValueDouble != null ? x.ValueDouble >= Rule.OperationValueDouble : false)) : false) ||

                                                (Operation == "<" ?
                                                    ((Rule.OperationValueDateTime != null ? x.ValueDateTime < Rule.OperationValueDateTime : false) ||
                                                    (Rule.OperationValueDouble != null ? x.ValueDouble < Rule.OperationValueDouble : false)) : false) ||

                                                (Operation == "<=" ?
                                                    ((Rule.OperationValueDateTime != null ? x.ValueDateTime <= Rule.OperationValueDateTime : false) ||
                                                    (Rule.OperationValueDouble != null ? x.ValueDouble <= Rule.OperationValueDouble : false)) : false)

                                                ).Select(x => x.InventoryId).ToList();
                                    }
                                    if (DynamicAttValuesInventoryIds != null ? DynamicAttValuesInventoryIds.Count() != 0 : false)
                                    {
                                        TableRecords = _mapper.Map<List<object>>(_dbContext.GetType()
                                            .GetProperty(SDTableName).GetValue(_dbContext, null))
                                                .Where(x => DynamicAttValuesInventoryIds.Contains(Convert.ToInt32(x.GetType().GetProperty("Id").GetValue(x, null)))).ToList();
                                    }
                                }

                                AddInstRuleViewModel AddInstRuleViewModel = new AddInstRuleViewModel();
                                if (Rule.dynamicAttId != null)
                                {
                                    AddInstRuleViewModel = new AddInstRuleViewModel
                                    {
                                        dynamicAttId = Rule.dynamicAttId,
                                        IsDynamic = true,
                                        OperationId = Rule.OperationId,
                                        OperationValueBoolean = Rule.OperationValueBoolean,
                                        OperationValueDateTime = Rule.OperationValueDateTime,
                                        OperationValueDouble = Rule.OperationValueDouble,
                                        OperationValueString = Rule.OperationValueString,
                                        TableName = Rule.tablesNames.TableName
                                    };
                                }
                                else if (Rule.attributeActivatedId != null)
                                {
                                    AddInstRuleViewModel = new AddInstRuleViewModel
                                    {
                                        attributeActivatedId = Rule.attributeActivatedId,
                                        IsDynamic = false,
                                        OperationId = Rule.OperationId,
                                        OperationValueBoolean = Rule.OperationValueBoolean,
                                        OperationValueDateTime = Rule.OperationValueDateTime,
                                        OperationValueDouble = Rule.OperationValueDouble,
                                        OperationValueString = Rule.OperationValueString,
                                        TableName = Rule.tablesNames.TableName
                                    };
                                }
                                List<object> RecordsIds = _mapper.Map<List<object>>(GetRecordsIds(MainTableName, AddInstRuleViewModel));

                                PathToCheckDependencyValidation Item = (PathToCheckDependencyValidation)Enum.Parse(typeof(PathToCheckDependencyValidation),
                                    MainTableName + SDTableName + "Goal");

                                List<string> Path = GetEnumDescription(Item).Split(" ").ToList();

                                object CheckId = new object();

                                if (Path.Count() > 1)
                                {
                                    object CivilLoads = AddInstallationViewModel.GetType().GetProperty(Path[0])
                                        .GetValue(AddInstallationViewModel, null);

                                    CheckId = CivilLoads.GetType().GetProperty(Path[1]).GetValue(CivilLoads, null) != null ?
                                        (int)CivilLoads.GetType().GetProperty(Path[1]).GetValue(CivilLoads, null) : new object();
                                }
                                else if (Path.Count() == 1 && Path[0].ToLower() == "sitecode")
                                {
                                    CheckId = SiteCode;
                                }
                                else if (Path.Count() == 1)
                                {
                                    if (AddInstallationViewModel.GetType().GetProperty(Path[0]).GetValue(AddInstallationViewModel, null) != null)
                                        CheckId = (int)AddInstallationViewModel.GetType().GetProperty(Path[0])
                                            .GetValue(AddInstallationViewModel, null);
                                }

                                if (RecordsIds.Exists(x => x.ToString().ToLower() == CheckId.ToString().ToLower()))
                                {
                                    CheckIfSuccessAllRules++;
                                }
                            }
                        }

                        if (Rules.Count() == CheckIfSuccessAllRules)
                        {
                            string DynamicAttributeName = "";
                            int DynamicAttributeId = _unitOfWork.DependencyRowRepository
                                .GetIncludeWhereFirst(x => x.RowId == RowId, x => x.Dependency).Dependency.DynamicAttId.Value;

                            AddDynamicAttInstValueViewModel InputDynamicAttribute = AddInstallationViewModel.TLIdynamicAttInstValue
                                .FirstOrDefault(x => x.DynamicAttId == DynamicAttributeId);

                            if (InputDynamicAttribute == null)
                            {
                                DynamicAttributeName = _unitOfWork.DynamicAttRepository
                                    .GetWhereFirst(x => x.Id == DynamicAttributeId).Key;

                                return $"({DynamicAttributeName}) value can't be null";
                            }
                            else
                            {
                                string DependencyValidationOperation = DynamicAttributeMainDependency.Operation.Name;

                                object DependencyValidationValue = new object();

                                if (DynamicAttributeMainDependency.ValueBoolean != null)
                                    DependencyValidationValue = DynamicAttributeMainDependency.ValueBoolean;

                                else if (DynamicAttributeMainDependency.ValueDateTime != null)
                                    DependencyValidationValue = DynamicAttributeMainDependency.ValueDateTime;

                                else if (DynamicAttributeMainDependency.ValueDouble != null)
                                    DependencyValidationValue = DynamicAttributeMainDependency.ValueDouble;

                                else if (!string.IsNullOrEmpty(DynamicAttributeMainDependency.ValueString))
                                    DependencyValidationValue = DynamicAttributeMainDependency.ValueString;

                                object InputDynamicValue = new object();

                                if (InputDynamicAttribute.ValueBoolean != null)
                                    InputDynamicValue = InputDynamicAttribute.ValueBoolean;

                                else if (InputDynamicAttribute.ValueDateTime != null)
                                    InputDynamicValue = InputDynamicAttribute.ValueDateTime;

                                else if (InputDynamicAttribute.ValueDouble != null)
                                    InputDynamicValue = InputDynamicAttribute.ValueDouble;

                                else if (!string.IsNullOrEmpty(InputDynamicAttribute.ValueString))
                                    InputDynamicValue = InputDynamicAttribute.ValueString;

                                if (!(DependencyValidationOperation == "==" ? InputDynamicValue.ToString().ToLower() == DependencyValidationValue.ToString().ToLower() :
                                    DependencyValidationOperation == "!=" ? InputDynamicValue.ToString().ToLower() != DependencyValidationValue.ToString().ToLower() :
                                    DependencyValidationOperation == ">" ? Comparer.DefaultInvariant.Compare(InputDynamicValue, DependencyValidationValue) == 1 :
                                    DependencyValidationOperation == ">=" ? (Comparer.DefaultInvariant.Compare(InputDynamicValue, DependencyValidationValue) == 1 ||
                                        InputDynamicValue.ToString().ToLower() == DependencyValidationValue.ToString().ToLower()) :
                                    DependencyValidationOperation == "<" ? Comparer.DefaultInvariant.Compare(InputDynamicValue, DependencyValidationValue) == -1 :
                                    DependencyValidationOperation == "<=" ? (Comparer.DefaultInvariant.Compare(InputDynamicValue, DependencyValidationValue) == -1 ||
                                        InputDynamicValue.ToString().ToLower() == DependencyValidationValue.ToString().ToLower()) : false))
                                {
                                    DynamicAttributeName = _unitOfWork.DynamicAttRepository
                                        .GetWhereFirst(x => x.Id == DynamicAttributeId).Key;

                                    string ReturnOperation = (DependencyValidationOperation == "==" ? "equal to" :
                                        (DependencyValidationOperation == "!=" ? "not equal to" :
                                        (DependencyValidationOperation == ">" ? "bigger than" :
                                        (DependencyValidationOperation == ">=" ? "bigger than or equal to" :
                                        (DependencyValidationOperation == "<" ? "smaller than" :
                                        (DependencyValidationOperation == "<=" ? "smaller than or equal to" : ""))))));

                                    return $"({DynamicAttributeName}) value must be {ReturnOperation} {DependencyValidationValue}";
                                }
                            }
                        }
                    }
                }
            }
            else if (OtherInventoryType.ToLower() == TablesNames.TLIsolar.ToString().ToLower())
            {
                string MainTableName = TablesNames.TLIsolar.ToString();
                AddSolarViewModel AddInstallationViewModel = _mapper.Map<AddSolarViewModel>(Input);

                List<DynamicAttViewModel> DynamicAttributes = _mapper.Map<List<DynamicAttViewModel>>(_unitOfWork.DynamicAttRepository
                    .GetIncludeWhere(x => x.tablesNames.TableName.ToLower() == MainTableName.ToLower() && !x.disable
                        , x => x.tablesNames).ToList());

                foreach (DynamicAttViewModel DynamicAttribute in DynamicAttributes)
                {
                    TLIdependency DynamicAttributeMainDependency = _unitOfWork.DependencieRepository.GetIncludeWhereFirst(x => x.DynamicAttId == DynamicAttribute.Id &&
                        (x.ValueBoolean != null || x.ValueDateTime != null || x.ValueDouble != null || !string.IsNullOrEmpty(x.ValueString)) &&
                        x.OperationId != null, x => x.Operation);

                    if (DynamicAttributeMainDependency == null)
                        continue;

                    List<int> DependencyRows = _unitOfWork.DependencyRowRepository.GetWhere(x => x.DependencyId == DynamicAttributeMainDependency.Id)
                        .Select(x => x.RowId.Value).Distinct().ToList();

                    foreach (int RowId in DependencyRows)
                    {
                        List<TLIrule> Rules = _unitOfWork.RowRuleRepository.GetIncludeWhere(x => x.RowId == RowId, x => x.Rule, x => x.Rule.tablesNames,
                            x => x.Rule.Operation, x => x.Rule.dynamicAtt, x => x.Rule.attributeActivated).Select(x => x.Rule).ToList();

                        int CheckIfSuccessAllRules = 0;

                        foreach (TLIrule Rule in Rules)
                        {
                            string SDTableName = Rule.tablesNames.TableName;

                            string DataType = "";

                            string Operation = Rule.Operation.Name;
                            object OperationValue = new object();

                            if (Rule.OperationValueBoolean != null)
                            {
                                DataType = "Bool";
                                OperationValue = Rule.OperationValueBoolean;
                            }
                            else if (Rule.OperationValueDateTime != null)
                            {
                                DataType = "DateTime";
                                OperationValue = Rule.OperationValueDateTime;
                            }
                            else if (Rule.OperationValueDouble != null)
                            {
                                DataType = "Double";
                                OperationValue = Rule.OperationValueDouble;
                            }
                            else if (!string.IsNullOrEmpty(Rule.OperationValueString))
                            {
                                DataType = "String";
                                OperationValue = Rule.OperationValueString;
                            }

                            if (MainTableName.ToLower() == SDTableName.ToLower())
                            {
                                object InsertedValue = new object();

                                if (Rule.attributeActivatedId != null)
                                {
                                    string AttributeName = Rule.attributeActivated.Key;

                                    object TestValue = AddInstallationViewModel.GetType().GetProperties()
                                        .FirstOrDefault(x => x.Name.ToLower() == AttributeName.ToLower()).GetValue(AddInstallationViewModel, null);

                                    if (TestValue == null)
                                        break;

                                    if (Rule.OperationValueBoolean != null)
                                        InsertedValue = bool.Parse(TestValue.ToString());

                                    else if (Rule.OperationValueDateTime != null)
                                        InsertedValue = DateTime.Parse(TestValue.ToString());

                                    else if (Rule.OperationValueDouble != null)
                                        InsertedValue = double.Parse(TestValue.ToString());

                                    else if (!string.IsNullOrEmpty(Rule.OperationValueString))
                                        InsertedValue = TestValue.ToString();
                                }
                                else if (Rule.dynamicAttId != null)
                                {
                                    AddDynamicAttInstValueViewModel DynamicObject = AddInstallationViewModel.TLIdynamicAttInstValue
                                        .FirstOrDefault(x => x.DynamicAttId == Rule.dynamicAttId.Value);

                                    if (DynamicObject == null)
                                        break;

                                    if (DynamicObject.ValueBoolean != null)
                                        InsertedValue = DynamicObject.ValueBoolean;

                                    else if (DynamicObject.ValueDateTime != null)
                                        InsertedValue = DynamicObject.ValueDateTime;

                                    else if (DynamicObject.ValueDouble != null)
                                        InsertedValue = DynamicObject.ValueDouble;

                                    else if (!string.IsNullOrEmpty(DynamicObject.ValueString))
                                        InsertedValue = DynamicObject.ValueString;
                                }

                                if (Operation == "==" ? InsertedValue.ToString().ToLower() == OperationValue.ToString().ToLower() :
                                    Operation == "!=" ? InsertedValue.ToString().ToLower() != OperationValue.ToString().ToLower() :
                                    Operation == ">" ? Comparer.DefaultInvariant.Compare(InsertedValue, OperationValue) == 1 :
                                    Operation == ">=" ? (Comparer.DefaultInvariant.Compare(InsertedValue, OperationValue) == 1 ||
                                        InsertedValue.ToString().ToLower() == OperationValue.ToString().ToLower()) :
                                    Operation == "<" ? Comparer.DefaultInvariant.Compare(InsertedValue, OperationValue) == -1 :
                                    Operation == "<=" ? (Comparer.DefaultInvariant.Compare(InsertedValue, OperationValue) == -1 ||
                                        InsertedValue.ToString().ToLower() == OperationValue.ToString().ToLower()) : false)
                                {
                                    CheckIfSuccessAllRules++;
                                }
                            }
                            else
                            {
                                List<object> TableRecords = new List<object>();
                                if (Rule.attributeActivatedId != null)
                                {
                                    string AttributeName = Rule.attributeActivated.Key;

                                    if (OperationValue != null)
                                        TableRecords = _mapper.Map<List<object>>(_dbContext.GetType().GetProperty(SDTableName)
                                            .GetValue(_dbContext, null)).Where(x => x.GetType().GetProperty(AttributeName).GetValue(x, null) != null ? (Operation == ">" ?
                                                (DataType.ToLower() == "DateTime".ToLower() ?
                                                    Comparer.DefaultInvariant.Compare(DateTime.Parse(x.GetType().GetProperty(AttributeName).GetValue(x, null).ToString()), OperationValue) == 1 :
                                                DataType.ToLower() == "Double".ToLower() ?
                                                    Comparer.DefaultInvariant.Compare(double.Parse(x.GetType().GetProperty(AttributeName).GetValue(x, null).ToString()), OperationValue) == 1 : false) :
                                            Operation == ">=" ?
                                                (DataType.ToLower() == "DateTime".ToLower() ?
                                                    (Comparer.DefaultInvariant.Compare(DateTime.Parse(x.GetType().GetProperty(AttributeName).GetValue(x, null).ToString()), OperationValue) == 1 ||
                                                        x.GetType().GetProperty(AttributeName).GetValue(x, null).ToString().ToLower() == OperationValue.ToString().ToLower()) :
                                                DataType.ToLower() == "Double".ToLower() ?
                                                    (Comparer.DefaultInvariant.Compare(double.Parse(x.GetType().GetProperty(AttributeName).GetValue(x, null).ToString()), OperationValue) == 1 ||
                                                        x.GetType().GetProperty(AttributeName).GetValue(x, null).ToString().ToLower() == OperationValue.ToString().ToLower()) : false) :
                                            Operation == "<" ?
                                                (DataType.ToLower() == "DateTime".ToLower() ?
                                                    Comparer.DefaultInvariant.Compare(DateTime.Parse(x.GetType().GetProperty(AttributeName).GetValue(x, null).ToString()), OperationValue) == -1 :
                                                DataType.ToLower() == "Double".ToLower() ?
                                                    Comparer.DefaultInvariant.Compare(double.Parse(x.GetType().GetProperty(AttributeName).GetValue(x, null).ToString()), OperationValue) == -1 : false) :
                                            Operation == "<=" ?
                                                (DataType.ToLower() == "DateTime".ToLower() ?
                                                    (Comparer.DefaultInvariant.Compare(DateTime.Parse(x.GetType().GetProperty(AttributeName).GetValue(x, null).ToString()), OperationValue) == -1 ||
                                                        x.GetType().GetProperty(AttributeName).GetValue(x, null).ToString().ToLower() == OperationValue.ToString().ToLower()) :
                                                DataType.ToLower() == "Double".ToLower() ?
                                                    (Comparer.DefaultInvariant.Compare(double.Parse(x.GetType().GetProperty(AttributeName).GetValue(x, null).ToString()), OperationValue) == -1 ||
                                                        x.GetType().GetProperty(AttributeName).GetValue(x, null).ToString().ToLower() == OperationValue.ToString().ToLower()) : false) :
                                            Operation == "==" ?
                                                x.GetType().GetProperty(AttributeName).GetValue(x, null).ToString().ToLower() == OperationValue.ToString().ToLower() :
                                            Operation == "!=" ?
                                                x.GetType().GetProperty(AttributeName).GetValue(x, null).ToString().ToLower() != OperationValue.ToString().ToLower() : false) : false).ToList();

                                }
                                else if (Rule.dynamicAttId != null)
                                {
                                    List<int> DynamicAttValuesInventoryIds = new List<int>();

                                    if (!DynamicAttribute.LibraryAtt)
                                    {
                                        DynamicAttValuesInventoryIds = _unitOfWork.DynamicAttInstValueRepository
                                            .GetWhere(x => (x.DynamicAttId == Rule.dynamicAttId.Value && !x.disable) &&
                                                (Operation == "==" ?
                                                    ((Rule.OperationValueBoolean != null ? x.ValueBoolean.ToString().ToLower() == Rule.OperationValueBoolean.ToString().ToLower() : false) ||
                                                    (Rule.OperationValueDateTime != null ? x.ValueDateTime.ToString().ToLower() == Rule.OperationValueDateTime.ToString().ToLower() : false) ||
                                                    (Rule.OperationValueDouble != null ? x.ValueDouble == Rule.OperationValueDouble : false) ||
                                                    (!string.IsNullOrEmpty(Rule.OperationValueString) ? x.ValueString.ToLower() == Rule.OperationValueString.ToLower() : false)) : false) ||

                                                (Operation == "!=" ?
                                                    ((Rule.OperationValueBoolean != null ? x.ValueBoolean.ToString().ToLower() != Rule.OperationValueBoolean.ToString().ToLower() : false) ||
                                                    (Rule.OperationValueDateTime != null ? x.ValueDateTime.ToString().ToLower() != Rule.OperationValueDateTime.ToString().ToLower() : false) ||
                                                    (Rule.OperationValueDouble != null ? x.ValueDouble != Rule.OperationValueDouble : false) ||
                                                    (!string.IsNullOrEmpty(Rule.OperationValueString) ? x.ValueString.ToLower() != Rule.OperationValueString.ToLower() : false)) : false) ||

                                                (Operation == ">" ?
                                                    ((Rule.OperationValueDateTime != null ? x.ValueDateTime > Rule.OperationValueDateTime : false) ||
                                                    (Rule.OperationValueDouble != null ? x.ValueDouble > Rule.OperationValueDouble : false)) : false) ||

                                                (Operation == ">=" ?
                                                    ((Rule.OperationValueDateTime != null ? x.ValueDateTime >= Rule.OperationValueDateTime : false) ||
                                                    (Rule.OperationValueDouble != null ? x.ValueDouble >= Rule.OperationValueDouble : false)) : false) ||

                                                (Operation == "<" ?
                                                    ((Rule.OperationValueDateTime != null ? x.ValueDateTime < Rule.OperationValueDateTime : false) ||
                                                    (Rule.OperationValueDouble != null ? x.ValueDouble < Rule.OperationValueDouble : false)) : false) ||

                                                (Operation == "<=" ?
                                                    ((Rule.OperationValueDateTime != null ? x.ValueDateTime <= Rule.OperationValueDateTime : false) ||
                                                    (Rule.OperationValueDouble != null ? x.ValueDouble <= Rule.OperationValueDouble : false)) : false)

                                                ).Select(x => x.InventoryId).ToList();
                                    }
                                    else
                                    {
                                        DynamicAttValuesInventoryIds = _unitOfWork.DynamicAttLibRepository
                                            .GetWhere(x => (x.DynamicAttId == Rule.dynamicAttId && !x.disable) &&
                                                (Operation == "==" ?
                                                    ((Rule.OperationValueBoolean != null ? x.ValueBoolean.ToString().ToLower() == Rule.OperationValueBoolean.ToString().ToLower() : false) ||
                                                    (Rule.OperationValueDateTime != null ? x.ValueDateTime.ToString().ToLower() == Rule.OperationValueDateTime.ToString().ToLower() : false) ||
                                                    (Rule.OperationValueDouble != null ? x.ValueDouble == Rule.OperationValueDouble : false) ||
                                                    (!string.IsNullOrEmpty(Rule.OperationValueString) ? x.ValueString.ToLower() == Rule.OperationValueString.ToLower() : false)) : false) ||

                                                (Operation == "!=" ?
                                                    ((Rule.OperationValueBoolean != null ? x.ValueBoolean.ToString().ToLower() != Rule.OperationValueBoolean.ToString().ToLower() : false) ||
                                                    (Rule.OperationValueDateTime != null ? x.ValueDateTime.ToString().ToLower() != Rule.OperationValueDateTime.ToString().ToLower() : false) ||
                                                    (Rule.OperationValueDouble != null ? x.ValueDouble != Rule.OperationValueDouble : false) ||
                                                    (!string.IsNullOrEmpty(Rule.OperationValueString) ? x.ValueString.ToLower() != Rule.OperationValueString.ToLower() : false)) : false) ||

                                                (Operation == ">" ?
                                                    ((Rule.OperationValueDateTime != null ? x.ValueDateTime > Rule.OperationValueDateTime : false) ||
                                                    (Rule.OperationValueDouble != null ? x.ValueDouble > Rule.OperationValueDouble : false)) : false) ||

                                                (Operation == ">=" ?
                                                    ((Rule.OperationValueDateTime != null ? x.ValueDateTime >= Rule.OperationValueDateTime : false) ||
                                                    (Rule.OperationValueDouble != null ? x.ValueDouble >= Rule.OperationValueDouble : false)) : false) ||

                                                (Operation == "<" ?
                                                    ((Rule.OperationValueDateTime != null ? x.ValueDateTime < Rule.OperationValueDateTime : false) ||
                                                    (Rule.OperationValueDouble != null ? x.ValueDouble < Rule.OperationValueDouble : false)) : false) ||

                                                (Operation == "<=" ?
                                                    ((Rule.OperationValueDateTime != null ? x.ValueDateTime <= Rule.OperationValueDateTime : false) ||
                                                    (Rule.OperationValueDouble != null ? x.ValueDouble <= Rule.OperationValueDouble : false)) : false)

                                                ).Select(x => x.InventoryId).ToList();
                                    }
                                    if (DynamicAttValuesInventoryIds != null ? DynamicAttValuesInventoryIds.Count() != 0 : false)
                                    {
                                        TableRecords = _mapper.Map<List<object>>(_dbContext.GetType()
                                            .GetProperty(SDTableName).GetValue(_dbContext, null))
                                                .Where(x => DynamicAttValuesInventoryIds.Contains(Convert.ToInt32(x.GetType().GetProperty("Id").GetValue(x, null)))).ToList();
                                    }
                                }

                                AddInstRuleViewModel AddInstRuleViewModel = new AddInstRuleViewModel();
                                if (Rule.dynamicAttId != null)
                                {
                                    AddInstRuleViewModel = new AddInstRuleViewModel
                                    {
                                        dynamicAttId = Rule.dynamicAttId,
                                        IsDynamic = true,
                                        OperationId = Rule.OperationId,
                                        OperationValueBoolean = Rule.OperationValueBoolean,
                                        OperationValueDateTime = Rule.OperationValueDateTime,
                                        OperationValueDouble = Rule.OperationValueDouble,
                                        OperationValueString = Rule.OperationValueString,
                                        TableName = Rule.tablesNames.TableName
                                    };
                                }
                                else if (Rule.attributeActivatedId != null)
                                {
                                    AddInstRuleViewModel = new AddInstRuleViewModel
                                    {
                                        attributeActivatedId = Rule.attributeActivatedId,
                                        IsDynamic = false,
                                        OperationId = Rule.OperationId,
                                        OperationValueBoolean = Rule.OperationValueBoolean,
                                        OperationValueDateTime = Rule.OperationValueDateTime,
                                        OperationValueDouble = Rule.OperationValueDouble,
                                        OperationValueString = Rule.OperationValueString,
                                        TableName = Rule.tablesNames.TableName
                                    };
                                }
                                List<object> RecordsIds = _mapper.Map<List<object>>(GetRecordsIds(MainTableName, AddInstRuleViewModel));

                                PathToCheckDependencyValidation Item = (PathToCheckDependencyValidation)Enum.Parse(typeof(PathToCheckDependencyValidation),
                                    MainTableName + SDTableName + "Goal");

                                List<string> Path = GetEnumDescription(Item).Split(" ").ToList();

                                object CheckId = new object();

                                if (Path.Count() > 1)
                                {
                                    object CivilLoads = AddInstallationViewModel.GetType().GetProperty(Path[0])
                                        .GetValue(AddInstallationViewModel, null);

                                    CheckId = CivilLoads.GetType().GetProperty(Path[1]).GetValue(CivilLoads, null) != null ?
                                        (int)CivilLoads.GetType().GetProperty(Path[1]).GetValue(CivilLoads, null) : new object();
                                }
                                else if (Path.Count() == 1 && Path[0].ToLower() == "sitecode")
                                {
                                    CheckId = SiteCode;
                                }
                                else if (Path.Count() == 1)
                                {
                                    if (AddInstallationViewModel.GetType().GetProperty(Path[0]).GetValue(AddInstallationViewModel, null) != null)
                                        CheckId = (int)AddInstallationViewModel.GetType().GetProperty(Path[0])
                                            .GetValue(AddInstallationViewModel, null);
                                }

                                if (RecordsIds.Exists(x => x.ToString().ToLower() == CheckId.ToString().ToLower()))
                                {
                                    CheckIfSuccessAllRules++;
                                }
                            }
                        }

                        if (Rules.Count() == CheckIfSuccessAllRules)
                        {
                            string DynamicAttributeName = "";
                            int DynamicAttributeId = _unitOfWork.DependencyRowRepository
                                .GetIncludeWhereFirst(x => x.RowId == RowId, x => x.Dependency).Dependency.DynamicAttId.Value;

                            AddDynamicAttInstValueViewModel InputDynamicAttribute = AddInstallationViewModel.TLIdynamicAttInstValue
                                .FirstOrDefault(x => x.DynamicAttId == DynamicAttributeId);

                            if (InputDynamicAttribute == null)
                            {
                                DynamicAttributeName = _unitOfWork.DynamicAttRepository
                                    .GetWhereFirst(x => x.Id == DynamicAttributeId).Key;

                                return $"({DynamicAttributeName}) value can't be null";
                            }
                            else
                            {
                                string DependencyValidationOperation = DynamicAttributeMainDependency.Operation.Name;

                                object DependencyValidationValue = new object();

                                if (DynamicAttributeMainDependency.ValueBoolean != null)
                                    DependencyValidationValue = DynamicAttributeMainDependency.ValueBoolean;

                                else if (DynamicAttributeMainDependency.ValueDateTime != null)
                                    DependencyValidationValue = DynamicAttributeMainDependency.ValueDateTime;

                                else if (DynamicAttributeMainDependency.ValueDouble != null)
                                    DependencyValidationValue = DynamicAttributeMainDependency.ValueDouble;

                                else if (!string.IsNullOrEmpty(DynamicAttributeMainDependency.ValueString))
                                    DependencyValidationValue = DynamicAttributeMainDependency.ValueString;

                                object InputDynamicValue = new object();

                                if (InputDynamicAttribute.ValueBoolean != null)
                                    InputDynamicValue = InputDynamicAttribute.ValueBoolean;

                                else if (InputDynamicAttribute.ValueDateTime != null)
                                    InputDynamicValue = InputDynamicAttribute.ValueDateTime;

                                else if (InputDynamicAttribute.ValueDouble != null)
                                    InputDynamicValue = InputDynamicAttribute.ValueDouble;

                                else if (!string.IsNullOrEmpty(InputDynamicAttribute.ValueString))
                                    InputDynamicValue = InputDynamicAttribute.ValueString;

                                if (!(DependencyValidationOperation == "==" ? InputDynamicValue.ToString().ToLower() == DependencyValidationValue.ToString().ToLower() :
                                    DependencyValidationOperation == "!=" ? InputDynamicValue.ToString().ToLower() != DependencyValidationValue.ToString().ToLower() :
                                    DependencyValidationOperation == ">" ? Comparer.DefaultInvariant.Compare(InputDynamicValue, DependencyValidationValue) == 1 :
                                    DependencyValidationOperation == ">=" ? (Comparer.DefaultInvariant.Compare(InputDynamicValue, DependencyValidationValue) == 1 ||
                                        InputDynamicValue.ToString().ToLower() == DependencyValidationValue.ToString().ToLower()) :
                                    DependencyValidationOperation == "<" ? Comparer.DefaultInvariant.Compare(InputDynamicValue, DependencyValidationValue) == -1 :
                                    DependencyValidationOperation == "<=" ? (Comparer.DefaultInvariant.Compare(InputDynamicValue, DependencyValidationValue) == -1 ||
                                        InputDynamicValue.ToString().ToLower() == DependencyValidationValue.ToString().ToLower()) : false))
                                {
                                    DynamicAttributeName = _unitOfWork.DynamicAttRepository
                                        .GetWhereFirst(x => x.Id == DynamicAttributeId).Key;

                                    string ReturnOperation = (DependencyValidationOperation == "==" ? "equal to" :
                                        (DependencyValidationOperation == "!=" ? "not equal to" :
                                        (DependencyValidationOperation == ">" ? "bigger than" :
                                        (DependencyValidationOperation == ">=" ? "bigger than or equal to" :
                                        (DependencyValidationOperation == "<" ? "smaller than" :
                                        (DependencyValidationOperation == "<=" ? "smaller than or equal to" : ""))))));

                                    return $"({DynamicAttributeName}) value must be {ReturnOperation} {DependencyValidationValue}";
                                }
                            }
                        }
                    }
                }
            }
            else if (OtherInventoryType.ToLower() == TablesNames.TLIgenerator.ToString().ToLower())
            {
                string MainTableName = TablesNames.TLIgenerator.ToString();
                AddGeneratorViewModel AddInstallationViewModel = _mapper.Map<AddGeneratorViewModel>(Input);

                List<DynamicAttViewModel> DynamicAttributes = _mapper.Map<List<DynamicAttViewModel>>(_unitOfWork.DynamicAttRepository
                    .GetIncludeWhere(x => x.tablesNames.TableName.ToLower() == MainTableName.ToLower() && !x.disable
                        , x => x.tablesNames).ToList());

                foreach (DynamicAttViewModel DynamicAttribute in DynamicAttributes)
                {
                    TLIdependency DynamicAttributeMainDependency = _unitOfWork.DependencieRepository.GetIncludeWhereFirst(x => x.DynamicAttId == DynamicAttribute.Id &&
                        (x.ValueBoolean != null || x.ValueDateTime != null || x.ValueDouble != null || !string.IsNullOrEmpty(x.ValueString)) &&
                        x.OperationId != null, x => x.Operation);

                    if (DynamicAttributeMainDependency == null)
                        continue;

                    List<int> DependencyRows = _unitOfWork.DependencyRowRepository.GetWhere(x => x.DependencyId == DynamicAttributeMainDependency.Id)
                        .Select(x => x.RowId.Value).Distinct().ToList();

                    foreach (int RowId in DependencyRows)
                    {
                        List<TLIrule> Rules = _unitOfWork.RowRuleRepository.GetIncludeWhere(x => x.RowId == RowId, x => x.Rule, x => x.Rule.tablesNames,
                            x => x.Rule.Operation, x => x.Rule.dynamicAtt, x => x.Rule.attributeActivated).Select(x => x.Rule).ToList();

                        int CheckIfSuccessAllRules = 0;

                        foreach (TLIrule Rule in Rules)
                        {
                            string SDTableName = Rule.tablesNames.TableName;

                            string DataType = "";

                            string Operation = Rule.Operation.Name;
                            object OperationValue = new object();

                            if (Rule.OperationValueBoolean != null)
                            {
                                DataType = "Bool";
                                OperationValue = Rule.OperationValueBoolean;
                            }
                            else if (Rule.OperationValueDateTime != null)
                            {
                                DataType = "DateTime";
                                OperationValue = Rule.OperationValueDateTime;
                            }
                            else if (Rule.OperationValueDouble != null)
                            {
                                DataType = "Double";
                                OperationValue = Rule.OperationValueDouble;
                            }
                            else if (!string.IsNullOrEmpty(Rule.OperationValueString))
                            {
                                DataType = "String";
                                OperationValue = Rule.OperationValueString;
                            }

                            if (MainTableName.ToLower() == SDTableName.ToLower())
                            {
                                object InsertedValue = new object();

                                if (Rule.attributeActivatedId != null)
                                {
                                    string AttributeName = Rule.attributeActivated.Key;

                                    object TestValue = AddInstallationViewModel.GetType().GetProperties()
                                        .FirstOrDefault(x => x.Name.ToLower() == AttributeName.ToLower()).GetValue(AddInstallationViewModel, null);

                                    if (TestValue == null)
                                        break;

                                    if (Rule.OperationValueBoolean != null)
                                        InsertedValue = bool.Parse(TestValue.ToString());

                                    else if (Rule.OperationValueDateTime != null)
                                        InsertedValue = DateTime.Parse(TestValue.ToString());

                                    else if (Rule.OperationValueDouble != null)
                                        InsertedValue = double.Parse(TestValue.ToString());

                                    else if (!string.IsNullOrEmpty(Rule.OperationValueString))
                                        InsertedValue = TestValue.ToString();
                                }
                                else if (Rule.dynamicAttId != null)
                                {
                                    AddDynamicAttInstValueViewModel DynamicObject = AddInstallationViewModel.TLIdynamicAttInstValue
                                        .FirstOrDefault(x => x.DynamicAttId == Rule.dynamicAttId.Value);

                                    if (DynamicObject == null)
                                        break;

                                    if (DynamicObject.ValueBoolean != null)
                                        InsertedValue = DynamicObject.ValueBoolean;

                                    else if (DynamicObject.ValueDateTime != null)
                                        InsertedValue = DynamicObject.ValueDateTime;

                                    else if (DynamicObject.ValueDouble != null)
                                        InsertedValue = DynamicObject.ValueDouble;

                                    else if (!string.IsNullOrEmpty(DynamicObject.ValueString))
                                        InsertedValue = DynamicObject.ValueString;
                                }

                                if (Operation == "==" ? InsertedValue.ToString().ToLower() == OperationValue.ToString().ToLower() :
                                    Operation == "!=" ? InsertedValue.ToString().ToLower() != OperationValue.ToString().ToLower() :
                                    Operation == ">" ? Comparer.DefaultInvariant.Compare(InsertedValue, OperationValue) == 1 :
                                    Operation == ">=" ? (Comparer.DefaultInvariant.Compare(InsertedValue, OperationValue) == 1 ||
                                        InsertedValue.ToString().ToLower() == OperationValue.ToString().ToLower()) :
                                    Operation == "<" ? Comparer.DefaultInvariant.Compare(InsertedValue, OperationValue) == -1 :
                                    Operation == "<=" ? (Comparer.DefaultInvariant.Compare(InsertedValue, OperationValue) == -1 ||
                                        InsertedValue.ToString().ToLower() == OperationValue.ToString().ToLower()) : false)
                                {
                                    CheckIfSuccessAllRules++;
                                }
                            }
                            else
                            {
                                List<object> TableRecords = new List<object>();
                                if (Rule.attributeActivatedId != null)
                                {
                                    string AttributeName = Rule.attributeActivated.Key;

                                    if (OperationValue != null)
                                        TableRecords = _mapper.Map<List<object>>(_dbContext.GetType().GetProperty(SDTableName)
                                            .GetValue(_dbContext, null)).Where(x => x.GetType().GetProperty(AttributeName).GetValue(x, null) != null ? (Operation == ">" ?
                                                (DataType.ToLower() == "DateTime".ToLower() ?
                                                    Comparer.DefaultInvariant.Compare(DateTime.Parse(x.GetType().GetProperty(AttributeName).GetValue(x, null).ToString()), OperationValue) == 1 :
                                                DataType.ToLower() == "Double".ToLower() ?
                                                    Comparer.DefaultInvariant.Compare(double.Parse(x.GetType().GetProperty(AttributeName).GetValue(x, null).ToString()), OperationValue) == 1 : false) :
                                            Operation == ">=" ?
                                                (DataType.ToLower() == "DateTime".ToLower() ?
                                                    (Comparer.DefaultInvariant.Compare(DateTime.Parse(x.GetType().GetProperty(AttributeName).GetValue(x, null).ToString()), OperationValue) == 1 ||
                                                        x.GetType().GetProperty(AttributeName).GetValue(x, null).ToString().ToLower() == OperationValue.ToString().ToLower()) :
                                                DataType.ToLower() == "Double".ToLower() ?
                                                    (Comparer.DefaultInvariant.Compare(double.Parse(x.GetType().GetProperty(AttributeName).GetValue(x, null).ToString()), OperationValue) == 1 ||
                                                        x.GetType().GetProperty(AttributeName).GetValue(x, null).ToString().ToLower() == OperationValue.ToString().ToLower()) : false) :
                                            Operation == "<" ?
                                                (DataType.ToLower() == "DateTime".ToLower() ?
                                                    Comparer.DefaultInvariant.Compare(DateTime.Parse(x.GetType().GetProperty(AttributeName).GetValue(x, null).ToString()), OperationValue) == -1 :
                                                DataType.ToLower() == "Double".ToLower() ?
                                                    Comparer.DefaultInvariant.Compare(double.Parse(x.GetType().GetProperty(AttributeName).GetValue(x, null).ToString()), OperationValue) == -1 : false) :
                                            Operation == "<=" ?
                                                (DataType.ToLower() == "DateTime".ToLower() ?
                                                    (Comparer.DefaultInvariant.Compare(DateTime.Parse(x.GetType().GetProperty(AttributeName).GetValue(x, null).ToString()), OperationValue) == -1 ||
                                                        x.GetType().GetProperty(AttributeName).GetValue(x, null).ToString().ToLower() == OperationValue.ToString().ToLower()) :
                                                DataType.ToLower() == "Double".ToLower() ?
                                                    (Comparer.DefaultInvariant.Compare(double.Parse(x.GetType().GetProperty(AttributeName).GetValue(x, null).ToString()), OperationValue) == -1 ||
                                                        x.GetType().GetProperty(AttributeName).GetValue(x, null).ToString().ToLower() == OperationValue.ToString().ToLower()) : false) :
                                            Operation == "==" ?
                                                x.GetType().GetProperty(AttributeName).GetValue(x, null).ToString().ToLower() == OperationValue.ToString().ToLower() :
                                            Operation == "!=" ?
                                                x.GetType().GetProperty(AttributeName).GetValue(x, null).ToString().ToLower() != OperationValue.ToString().ToLower() : false) : false).ToList();

                                }
                                else if (Rule.dynamicAttId != null)
                                {
                                    List<int> DynamicAttValuesInventoryIds = new List<int>();

                                    if (!DynamicAttribute.LibraryAtt)
                                    {
                                        DynamicAttValuesInventoryIds = _unitOfWork.DynamicAttInstValueRepository
                                            .GetWhere(x => (x.DynamicAttId == Rule.dynamicAttId.Value && !x.disable) &&
                                                (Operation == "==" ?
                                                    ((Rule.OperationValueBoolean != null ? x.ValueBoolean.ToString().ToLower() == Rule.OperationValueBoolean.ToString().ToLower() : false) ||
                                                    (Rule.OperationValueDateTime != null ? x.ValueDateTime.ToString().ToLower() == Rule.OperationValueDateTime.ToString().ToLower() : false) ||
                                                    (Rule.OperationValueDouble != null ? x.ValueDouble == Rule.OperationValueDouble : false) ||
                                                    (!string.IsNullOrEmpty(Rule.OperationValueString) ? x.ValueString.ToLower() == Rule.OperationValueString.ToLower() : false)) : false) ||

                                                (Operation == "!=" ?
                                                    ((Rule.OperationValueBoolean != null ? x.ValueBoolean.ToString().ToLower() != Rule.OperationValueBoolean.ToString().ToLower() : false) ||
                                                    (Rule.OperationValueDateTime != null ? x.ValueDateTime.ToString().ToLower() != Rule.OperationValueDateTime.ToString().ToLower() : false) ||
                                                    (Rule.OperationValueDouble != null ? x.ValueDouble != Rule.OperationValueDouble : false) ||
                                                    (!string.IsNullOrEmpty(Rule.OperationValueString) ? x.ValueString.ToLower() != Rule.OperationValueString.ToLower() : false)) : false) ||

                                                (Operation == ">" ?
                                                    ((Rule.OperationValueDateTime != null ? x.ValueDateTime > Rule.OperationValueDateTime : false) ||
                                                    (Rule.OperationValueDouble != null ? x.ValueDouble > Rule.OperationValueDouble : false)) : false) ||

                                                (Operation == ">=" ?
                                                    ((Rule.OperationValueDateTime != null ? x.ValueDateTime >= Rule.OperationValueDateTime : false) ||
                                                    (Rule.OperationValueDouble != null ? x.ValueDouble >= Rule.OperationValueDouble : false)) : false) ||

                                                (Operation == "<" ?
                                                    ((Rule.OperationValueDateTime != null ? x.ValueDateTime < Rule.OperationValueDateTime : false) ||
                                                    (Rule.OperationValueDouble != null ? x.ValueDouble < Rule.OperationValueDouble : false)) : false) ||

                                                (Operation == "<=" ?
                                                    ((Rule.OperationValueDateTime != null ? x.ValueDateTime <= Rule.OperationValueDateTime : false) ||
                                                    (Rule.OperationValueDouble != null ? x.ValueDouble <= Rule.OperationValueDouble : false)) : false)

                                                ).Select(x => x.InventoryId).ToList();
                                    }
                                    else
                                    {
                                        DynamicAttValuesInventoryIds = _unitOfWork.DynamicAttLibRepository
                                            .GetWhere(x => (x.DynamicAttId == Rule.dynamicAttId && !x.disable) &&
                                                (Operation == "==" ?
                                                    ((Rule.OperationValueBoolean != null ? x.ValueBoolean.ToString().ToLower() == Rule.OperationValueBoolean.ToString().ToLower() : false) ||
                                                    (Rule.OperationValueDateTime != null ? x.ValueDateTime.ToString().ToLower() == Rule.OperationValueDateTime.ToString().ToLower() : false) ||
                                                    (Rule.OperationValueDouble != null ? x.ValueDouble == Rule.OperationValueDouble : false) ||
                                                    (!string.IsNullOrEmpty(Rule.OperationValueString) ? x.ValueString.ToLower() == Rule.OperationValueString.ToLower() : false)) : false) ||

                                                (Operation == "!=" ?
                                                    ((Rule.OperationValueBoolean != null ? x.ValueBoolean.ToString().ToLower() != Rule.OperationValueBoolean.ToString().ToLower() : false) ||
                                                    (Rule.OperationValueDateTime != null ? x.ValueDateTime.ToString().ToLower() != Rule.OperationValueDateTime.ToString().ToLower() : false) ||
                                                    (Rule.OperationValueDouble != null ? x.ValueDouble != Rule.OperationValueDouble : false) ||
                                                    (!string.IsNullOrEmpty(Rule.OperationValueString) ? x.ValueString.ToLower() != Rule.OperationValueString.ToLower() : false)) : false) ||

                                                (Operation == ">" ?
                                                    ((Rule.OperationValueDateTime != null ? x.ValueDateTime > Rule.OperationValueDateTime : false) ||
                                                    (Rule.OperationValueDouble != null ? x.ValueDouble > Rule.OperationValueDouble : false)) : false) ||

                                                (Operation == ">=" ?
                                                    ((Rule.OperationValueDateTime != null ? x.ValueDateTime >= Rule.OperationValueDateTime : false) ||
                                                    (Rule.OperationValueDouble != null ? x.ValueDouble >= Rule.OperationValueDouble : false)) : false) ||

                                                (Operation == "<" ?
                                                    ((Rule.OperationValueDateTime != null ? x.ValueDateTime < Rule.OperationValueDateTime : false) ||
                                                    (Rule.OperationValueDouble != null ? x.ValueDouble < Rule.OperationValueDouble : false)) : false) ||

                                                (Operation == "<=" ?
                                                    ((Rule.OperationValueDateTime != null ? x.ValueDateTime <= Rule.OperationValueDateTime : false) ||
                                                    (Rule.OperationValueDouble != null ? x.ValueDouble <= Rule.OperationValueDouble : false)) : false)

                                                ).Select(x => x.InventoryId).ToList();
                                    }
                                    if (DynamicAttValuesInventoryIds != null ? DynamicAttValuesInventoryIds.Count() != 0 : false)
                                    {
                                        TableRecords = _mapper.Map<List<object>>(_dbContext.GetType()
                                            .GetProperty(SDTableName).GetValue(_dbContext, null))
                                                .Where(x => DynamicAttValuesInventoryIds.Contains(Convert.ToInt32(x.GetType().GetProperty("Id").GetValue(x, null)))).ToList();
                                    }
                                }

                                AddInstRuleViewModel AddInstRuleViewModel = new AddInstRuleViewModel();
                                if (Rule.dynamicAttId != null)
                                {
                                    AddInstRuleViewModel = new AddInstRuleViewModel
                                    {
                                        dynamicAttId = Rule.dynamicAttId,
                                        IsDynamic = true,
                                        OperationId = Rule.OperationId,
                                        OperationValueBoolean = Rule.OperationValueBoolean,
                                        OperationValueDateTime = Rule.OperationValueDateTime,
                                        OperationValueDouble = Rule.OperationValueDouble,
                                        OperationValueString = Rule.OperationValueString,
                                        TableName = Rule.tablesNames.TableName
                                    };
                                }
                                else if (Rule.attributeActivatedId != null)
                                {
                                    AddInstRuleViewModel = new AddInstRuleViewModel
                                    {
                                        attributeActivatedId = Rule.attributeActivatedId,
                                        IsDynamic = false,
                                        OperationId = Rule.OperationId,
                                        OperationValueBoolean = Rule.OperationValueBoolean,
                                        OperationValueDateTime = Rule.OperationValueDateTime,
                                        OperationValueDouble = Rule.OperationValueDouble,
                                        OperationValueString = Rule.OperationValueString,
                                        TableName = Rule.tablesNames.TableName
                                    };
                                }
                                List<object> RecordsIds = _mapper.Map<List<object>>(GetRecordsIds(MainTableName, AddInstRuleViewModel));

                                PathToCheckDependencyValidation Item = (PathToCheckDependencyValidation)Enum.Parse(typeof(PathToCheckDependencyValidation),
                                    MainTableName + SDTableName + "Goal");

                                List<string> Path = GetEnumDescription(Item).Split(" ").ToList();

                                object CheckId = new object();

                                if (Path.Count() > 1)
                                {
                                    object CivilLoads = AddInstallationViewModel.GetType().GetProperty(Path[0])
                                        .GetValue(AddInstallationViewModel, null);

                                    CheckId = CivilLoads.GetType().GetProperty(Path[1]).GetValue(CivilLoads, null) != null ?
                                        (int)CivilLoads.GetType().GetProperty(Path[1]).GetValue(CivilLoads, null) : new object();
                                }
                                else if (Path.Count() == 1 && Path[0].ToLower() == "sitecode")
                                {
                                    CheckId = SiteCode;
                                }
                                else if (Path.Count() == 1)
                                {
                                    if (AddInstallationViewModel.GetType().GetProperty(Path[0]).GetValue(AddInstallationViewModel, null) != null)
                                        CheckId = (int)AddInstallationViewModel.GetType().GetProperty(Path[0])
                                            .GetValue(AddInstallationViewModel, null);
                                }

                                if (RecordsIds.Exists(x => x.ToString().ToLower() == CheckId.ToString().ToLower()))
                                {
                                    CheckIfSuccessAllRules++;
                                }
                            }
                        }

                        if (Rules.Count() == CheckIfSuccessAllRules)
                        {
                            string DynamicAttributeName = "";
                            int DynamicAttributeId = _unitOfWork.DependencyRowRepository
                                .GetIncludeWhereFirst(x => x.RowId == RowId, x => x.Dependency).Dependency.DynamicAttId.Value;

                            AddDynamicAttInstValueViewModel InputDynamicAttribute = AddInstallationViewModel.TLIdynamicAttInstValue
                                .FirstOrDefault(x => x.DynamicAttId == DynamicAttributeId);

                            if (InputDynamicAttribute == null)
                            {
                                DynamicAttributeName = _unitOfWork.DynamicAttRepository
                                    .GetWhereFirst(x => x.Id == DynamicAttributeId).Key;

                                return $"({DynamicAttributeName}) value can't be null";
                            }
                            else
                            {
                                string DependencyValidationOperation = DynamicAttributeMainDependency.Operation.Name;

                                object DependencyValidationValue = new object();

                                if (DynamicAttributeMainDependency.ValueBoolean != null)
                                    DependencyValidationValue = DynamicAttributeMainDependency.ValueBoolean;

                                else if (DynamicAttributeMainDependency.ValueDateTime != null)
                                    DependencyValidationValue = DynamicAttributeMainDependency.ValueDateTime;

                                else if (DynamicAttributeMainDependency.ValueDouble != null)
                                    DependencyValidationValue = DynamicAttributeMainDependency.ValueDouble;

                                else if (!string.IsNullOrEmpty(DynamicAttributeMainDependency.ValueString))
                                    DependencyValidationValue = DynamicAttributeMainDependency.ValueString;

                                object InputDynamicValue = new object();

                                if (InputDynamicAttribute.ValueBoolean != null)
                                    InputDynamicValue = InputDynamicAttribute.ValueBoolean;

                                else if (InputDynamicAttribute.ValueDateTime != null)
                                    InputDynamicValue = InputDynamicAttribute.ValueDateTime;

                                else if (InputDynamicAttribute.ValueDouble != null)
                                    InputDynamicValue = InputDynamicAttribute.ValueDouble;

                                else if (!string.IsNullOrEmpty(InputDynamicAttribute.ValueString))
                                    InputDynamicValue = InputDynamicAttribute.ValueString;

                                if (!(DependencyValidationOperation == "==" ? InputDynamicValue.ToString().ToLower() == DependencyValidationValue.ToString().ToLower() :
                                    DependencyValidationOperation == "!=" ? InputDynamicValue.ToString().ToLower() != DependencyValidationValue.ToString().ToLower() :
                                    DependencyValidationOperation == ">" ? Comparer.DefaultInvariant.Compare(InputDynamicValue, DependencyValidationValue) == 1 :
                                    DependencyValidationOperation == ">=" ? (Comparer.DefaultInvariant.Compare(InputDynamicValue, DependencyValidationValue) == 1 ||
                                        InputDynamicValue.ToString().ToLower() == DependencyValidationValue.ToString().ToLower()) :
                                    DependencyValidationOperation == "<" ? Comparer.DefaultInvariant.Compare(InputDynamicValue, DependencyValidationValue) == -1 :
                                    DependencyValidationOperation == "<=" ? (Comparer.DefaultInvariant.Compare(InputDynamicValue, DependencyValidationValue) == -1 ||
                                        InputDynamicValue.ToString().ToLower() == DependencyValidationValue.ToString().ToLower()) : false))
                                {
                                    DynamicAttributeName = _unitOfWork.DynamicAttRepository
                                        .GetWhereFirst(x => x.Id == DynamicAttributeId).Key;

                                    string ReturnOperation = (DependencyValidationOperation == "==" ? "equal to" :
                                        (DependencyValidationOperation == "!=" ? "not equal to" :
                                        (DependencyValidationOperation == ">" ? "bigger than" :
                                        (DependencyValidationOperation == ">=" ? "bigger than or equal to" :
                                        (DependencyValidationOperation == "<" ? "smaller than" :
                                        (DependencyValidationOperation == "<=" ? "smaller than or equal to" : ""))))));

                                    return $"({DynamicAttributeName}) value must be {ReturnOperation} {DependencyValidationValue}";
                                }
                            }
                        }
                    }
                }
            }
            return string.Empty;
        }
        public string CheckGeneralValidationFunction(List<AddDynamicAttInstValueViewModel> TLIdynamicAttInstValue, string TableName)
        {
            List<DynamicAttViewModel> DynamicAttributes = _mapper.Map<List<DynamicAttViewModel>>(_unitOfWork.DynamicAttRepository
                .GetIncludeWhere(x => x.tablesNames.TableName.ToLower() == TableName.ToLower() && !x.disable
                    , x => x.tablesNames).ToList());

            foreach (DynamicAttViewModel DynamicAttributeEntity in DynamicAttributes)
            {
                TLIvalidation Validation = _unitOfWork.ValidationRepository
                    .GetIncludeWhereFirst(x => x.DynamicAttId == DynamicAttributeEntity.Id, x => x.Operation, x => x.DynamicAtt);

                if (Validation != null)
                {
                    AddDynamicAttInstValueViewModel DynmaicAttributeValue = TLIdynamicAttInstValue.FirstOrDefault(x => x.DynamicAttId == DynamicAttributeEntity.Id);

                    if (DynmaicAttributeValue == null)
                        return $"({Validation.DynamicAtt.Key}) value can't be null and must be inserted";

                    string OperationName = Validation.Operation.Name;

                    object InputDynamicValue = new object();

                    if (DynmaicAttributeValue.ValueBoolean != null)
                        InputDynamicValue = DynmaicAttributeValue.ValueBoolean;

                    else if (DynmaicAttributeValue.ValueDateTime != null)
                        InputDynamicValue = DynmaicAttributeValue.ValueDateTime;

                    else if (DynmaicAttributeValue.ValueDouble != null)
                        InputDynamicValue = DynmaicAttributeValue.ValueDouble;

                    else if (!string.IsNullOrEmpty(DynmaicAttributeValue.ValueString))
                        InputDynamicValue = DynmaicAttributeValue.ValueString;

                    object ValidationValue = new object();

                    if (Validation.ValueBoolean != null)
                        ValidationValue = Validation.ValueBoolean;

                    else if (Validation.ValueDateTime != null)
                        ValidationValue = Validation.ValueDateTime;

                    else if (Validation.ValueDouble != null)
                        ValidationValue = Validation.ValueDouble;

                    else if (!string.IsNullOrEmpty(Validation.ValueString))
                        ValidationValue = Validation.ValueString;

                    if (!(OperationName == "==" ? InputDynamicValue.ToString().ToLower() == ValidationValue.ToString().ToLower() :
                        OperationName == "!=" ? InputDynamicValue.ToString().ToLower() != ValidationValue.ToString().ToLower() :
                        OperationName == ">" ? Comparer.DefaultInvariant.Compare(InputDynamicValue, ValidationValue) == 1 :
                        OperationName == ">=" ? (Comparer.DefaultInvariant.Compare(InputDynamicValue, ValidationValue) == 1 ||
                            InputDynamicValue.ToString().ToLower() == ValidationValue.ToString().ToLower()) :
                        OperationName == "<" ? Comparer.DefaultInvariant.Compare(InputDynamicValue, ValidationValue) == -1 :
                        OperationName == "<=" ? (Comparer.DefaultInvariant.Compare(InputDynamicValue, ValidationValue) == -1 ||
                            InputDynamicValue.ToString().ToLower() == ValidationValue.ToString().ToLower()) : false))
                    {
                        string DynamicAttributeName = _unitOfWork.DynamicAttRepository
                            .GetWhereFirst(x => x.Id == Validation.DynamicAttId).Key;

                        string ReturnOperation = (OperationName == "==" ? "equal to" :
                            (OperationName == "!=" ? "not equal to" :
                            (OperationName == ">" ? "bigger than" :
                            (OperationName == ">=" ? "bigger than or equal to" :
                            (OperationName == "<" ? "smaller than" :
                            (OperationName == "<=" ? "smaller than or equal to" : ""))))));

                        return $"({DynamicAttributeName}) value must be {ReturnOperation} {ValidationValue}";
                    }
                }
            }

            return string.Empty;
        }
        #endregion
        //public Response<ObjectInstAtts> AddOtherInventoryInstallation(object model, string TableName, string SiteCode, string ConnectionString, int? TaskId, int UserId)
        //{
        //    int allOtherInventoryInstId = 0;
        //    using (var con = new OracleConnection(ConnectionString))
        //    {
        //        con.Open();
        //        using (var tran = con.BeginTransaction())
        //        {
        //            using (TransactionScope transaction = new TransactionScope())
        //            {
        //                try
        //                {
        //                    string ErrorMessage = string.Empty;
        //                    var TableNameEntity = _unitOfWork.TablesNamesRepository.GetWhereFirst(o => o.TableName == TableName);

        //                    if (OtherInventoryType.TLIcabinet.ToString().ToLower() == TableName.ToLower())
        //                    {
        //                        bool IsCabinetPowerLibraryType = false;
        //                        //Add Cabinet
        //                        var addCabinetViewModel = _mapper.Map<AddCabinetViewModel>(model);
        //                        var Cabinet = _mapper.Map<TLIcabinet>(addCabinetViewModel);
        //                        if (addCabinetViewModel.CabinetPowerLibraryId != null)
        //                        {
        //                            IsCabinetPowerLibraryType = true;
        //                            if (addCabinetViewModel.TLIotherInSite.ReservedSpace == true)
        //                            {
        //                                var CheckSpace = _unitOfWork.SiteRepository.CheckSpaces(UserId, SiteCode, TableName, addCabinetViewModel.CabinetPowerLibraryId.Value, addCabinetViewModel.SpaceInstallation, "Power").Message;
        //                                if (CheckSpace != "Success")
        //                                {
        //                                    return new Response<ObjectInstAtts>(true, null, null, CheckSpace, (int)Helpers.Constants.ApiReturnCode.fail);
        //                                }
        //                            }
        //                        }
        //                        else if (addCabinetViewModel.CabinetTelecomLibraryId != null)
        //                        {
        //                            if (addCabinetViewModel.TLIotherInSite.ReservedSpace == true)
        //                            {

        //                                var CheckSpace = _unitOfWork.SiteRepository.CheckSpaces(UserId, SiteCode, TableName, addCabinetViewModel.CabinetTelecomLibraryId.Value, addCabinetViewModel.SpaceInstallation, "Telecom").Message;
        //                                if (CheckSpace != "Success")
        //                                {
        //                                    return new Response<ObjectInstAtts>(true, null, null, CheckSpace, (int)Helpers.Constants.ApiReturnCode.fail);
        //                                }
        //                            }
        //                        }

        //                        if (addCabinetViewModel.CabinetPowerLibraryId != null)
        //                        {
        //                            if (addCabinetViewModel.NUmberOfPSU == null)
        //                            {
        //                                return new Response<ObjectInstAtts>(true, null, null, "The numberofpsu can't be null in case cabinet library type is power", (int)ApiReturnCode.fail);
        //                            }
        //                            //if (addCabinetViewModel.RenewableCabinetTypeId == 0)
        //                            //{
        //                            //    return new Response<ObjectInstAtts>(true, null, null, "The renewable cabinet type can't be null in case cabinet library type is power", (int)ApiReturnCode.fail);
        //                            //}
        //                        }
        //                        TLIrenewableCabinetType CabinetEntity = new TLIrenewableCabinetType();
        //                        if (addCabinetViewModel.RenewableCabinetTypeId != null)
        //                        {
        //                            CabinetEntity = _unitOfWork.RenewableCabinetTypeRepository.GetByID(addCabinetViewModel.RenewableCabinetTypeId.Value);

        //                            if (CabinetEntity != null)
        //                            {
        //                                if (CabinetEntity.Name.ToLower() == "controller with batteries")
        //                                {
        //                                    if (addCabinetViewModel.RenewableCabinetNumberOfBatteries == null)
        //                                    {
        //                                        return new Response<ObjectInstAtts>(true, null, null, "The Renewable Cabinet Number Of Batteries Can't Be Null  in case Renewable  Cabinet type is controller with batteries ", (int)ApiReturnCode.fail);
        //                                    }

        //                                }
        //                            }
        //                        }

        //                        //Check Validations
        //                        bool test = false;
        //                        string CheckDependencyValidation = this.CheckDependencyValidation(model, TableName, SiteCode);

        //                        if (!string.IsNullOrEmpty(CheckDependencyValidation))
        //                            return new Response<ObjectInstAtts>(true, null, null, CheckDependencyValidation, (int)ApiReturnCode.fail);

        //                        string CheckGeneralValidation = CheckGeneralValidationFunction(addCabinetViewModel.TLIdynamicAttInstValue, TableNameEntity.TableName);

        //                        if (!string.IsNullOrEmpty(CheckGeneralValidation))
        //                            return new Response<ObjectInstAtts>(true, null, null, CheckGeneralValidation, (int)ApiReturnCode.fail);

        //                        test = true;
        //                        if (test == true)
        //                        {
        //                            TLIotherInSite CheckName = _unitOfWork.OtherInSiteRepository.GetIncludeWhereFirst(x => !x.Dismantle && !x.allOtherInventoryInst.Draft &&
        //                                ((x.allOtherInventoryInst.cabinetId != null) ? (x.allOtherInventoryInst.cabinet.Name.ToLower() == Cabinet.Name.ToLower()) : (false)) &&
        //                                ((IsCabinetPowerLibraryType) ? (x.allOtherInventoryInst.cabinet.CabinetPowerLibraryId == Cabinet.CabinetPowerLibraryId) :
        //                                    (x.allOtherInventoryInst.cabinet.CabinetTelecomLibraryId == Cabinet.CabinetTelecomLibraryId)) &&
        //                                     x.SiteCode.ToLower() == SiteCode.ToLower(),
        //                                     x => x.allOtherInventoryInst, x => x.allOtherInventoryInst.cabinet);

        //                            if (CheckName != null)
        //                                return new Response<ObjectInstAtts>(true, null, null, $"This name {Cabinet.Name} is already exists", (int)ApiReturnCode.fail);

        //                            _unitOfWork.CabinetRepository.AddWithHistory(Helpers.LogFilterAttribute.UserId, Cabinet);
        //                            _unitOfWork.SaveChanges();
        //                            //Add to Civil_Site_Date if there is free space
        //                            TLIallOtherInventoryInst allOtherInventoryInst = new TLIallOtherInventoryInst();
        //                            allOtherInventoryInst.cabinetId = Cabinet.Id;
        //                            _unitOfWork.AllOtherInventoryInstRepository.Add(allOtherInventoryInst);
        //                            _unitOfWork.SaveChanges();
        //                            allOtherInventoryInstId = allOtherInventoryInst.Id;
        //                            if (addCabinetViewModel.TLIotherInSite != null && String.IsNullOrEmpty(SiteCode) == false)
        //                            {
        //                                TLIotherInSite otherInSite = new TLIotherInSite();
        //                                otherInSite.SiteCode = SiteCode;
        //                                otherInSite.OtherInSiteStatus = addCabinetViewModel.TLIotherInSite.OtherInSiteStatus;
        //                                otherInSite.OtherInventoryStatus = addCabinetViewModel.TLIotherInSite.OtherInventoryStatus;
        //                                otherInSite.allOtherInventoryInstId = allOtherInventoryInst.Id;
        //                                otherInSite.InstallationDate = addCabinetViewModel.TLIotherInSite.InstallationDate;
        //                                otherInSite.ReservedSpace = addCabinetViewModel.TLIotherInSite.ReservedSpace;
        //                                _unitOfWork.OtherInSiteRepository.Add(otherInSite);
        //                                _unitOfWork.SaveChanges();
        //                            }
        //                            var CheckOtherInventoryReference = _unitOfWork.OtherInSiteRepository.GetWhere(x => x.SiteCode == SiteCode).ToList();
        //                            if (addCabinetViewModel.TLIotherInventoryDistance != null && CheckOtherInventoryReference.Count > 0)
        //                            {
        //                                TLIotherInventoryDistance otherInventoryDistance = new TLIotherInventoryDistance();
        //                                otherInventoryDistance.Distance = addCabinetViewModel.TLIotherInventoryDistance.Distance != null ?
        //                                    addCabinetViewModel.TLIotherInventoryDistance.Distance.Value : 0;
        //                                otherInventoryDistance.Azimuth = addCabinetViewModel.TLIotherInventoryDistance.Azimuth != null ?
        //                                    addCabinetViewModel.TLIotherInventoryDistance.Azimuth.Value : 0;
        //                                otherInventoryDistance.SiteCode = SiteCode;
        //                                otherInventoryDistance.ReferenceOtherInventoryId = addCabinetViewModel.TLIotherInventoryDistance.ReferenceOtherInventoryId != null ?
        //                                    addCabinetViewModel.TLIotherInventoryDistance.ReferenceOtherInventoryId.Value : 0;
        //                                otherInventoryDistance.allOtherInventoryInstId = allOtherInventoryInst.Id;
        //                                _unitOfWork.OtherInventoryDistanceRepository.Add(otherInventoryDistance);
        //                                _unitOfWork.SaveChanges();
        //                            }
        //                            //if (addCabinetViewModel.TLIotherInSite.ReservedSpace == true)
        //                            //{
        //                            //    _unitOfWork.SiteRepository.UpdateReservedSpace(SiteCode, addCabinetViewModel.SpaceInstallation);
        //                            //}
        //                            if (addCabinetViewModel.TLIdynamicAttInstValue != null ? addCabinetViewModel.TLIdynamicAttInstValue.Count > 0 : false)
        //                            {
        //                                foreach (var DynamicAttInstValue in addCabinetViewModel.TLIdynamicAttInstValue)
        //                                {
        //                                    _unitOfWork.DynamicAttInstValueRepository.AddDynamicInstAtts(DynamicAttInstValue, TableNameEntity.Id, Cabinet.Id);
        //                                }
        //                            }
        //                            //AddHistory(addCabinetViewModel.ticketAtt, allOtherInventoryInstId, "Insert");
        //                        }
        //                        else
        //                        {
        //                            return new Response<ObjectInstAtts>(true, null, null, ErrorMessage, (int)ApiReturnCode.fail);
        //                        }

        //                    }
        //                    else if (OtherInventoryType.TLIgenerator.ToString().ToLower() == TableName.ToLower())
        //                    {
        //                        var addGeneratorViewModel = _mapper.Map<AddGeneratorInstallationObject>(model);
        //                        var Generator = _mapper.Map<TLIgenerator>(addGeneratorViewModel);
        //                        if (addGeneratorViewModel.OtherInSite.ReservedSpace == true)
        //                        {
        //                            var CheckSpace = _unitOfWork.SiteRepository.CheckSpaces(UserId, SiteCode, TableName, addGeneratorViewModel.GeneratorType.GeneratorLibraryId, addGeneratorViewModel.installationAttributes.SpaceInstallation, null).Message;
        //                            if (CheckSpace != "Success")
        //                            {
        //                                return new Response<ObjectInstAtts>(true, null, null, CheckSpace, (int)Helpers.Constants.ApiReturnCode.fail);
        //                            }
        //                        }

        //                        //string CheckDependencyValidation = this.CheckDependencyValidation(model, TableName, SiteCode);

        //                        //if (!string.IsNullOrEmpty(CheckDependencyValidation))
        //                        //    return new Response<ObjectInstAtts>(true, null, null, CheckDependencyValidation, (int)ApiReturnCode.fail);

        //                        //string CheckGeneralValidation = CheckGeneralValidationFunction(addGeneratorViewModel.TLIdynamicAttInstValue, TableNameEntity.TableName);

        //                        //if (!string.IsNullOrEmpty(CheckGeneralValidation))
        //                        //    return new Response<ObjectInstAtts>(true, null, null, CheckGeneralValidation, (int)ApiReturnCode.fail);


        //                        TLIotherInSite CheckName = _unitOfWork.OtherInSiteRepository.GetIncludeWhereFirst(x => !x.Dismantle && !x.allOtherInventoryInst.Draft &&
        //                            (x.allOtherInventoryInst.generatorId != null ? x.allOtherInventoryInst.generator.Name.ToLower() == Generator.Name.ToLower() : false) &&
        //                            x.SiteCode.ToLower() == SiteCode.ToLower(),
        //                                x => x.allOtherInventoryInst, x => x.allOtherInventoryInst.generator);

        //                        if (CheckName != null)
        //                            return new Response<ObjectInstAtts>(true, null, null, $"This name {Generator.Name} is already exists", (int)ApiReturnCode.fail);

        //                        var CheckSerialNumber = _unitOfWork.GeneratorRepository.GetWhereFirst(x => x.SerialNumber == Generator.SerialNumber);
        //                        if (CheckSerialNumber != null)
        //                        {
        //                            return new Response<ObjectInstAtts>(true, null, null, $"The serial number {Generator.SerialNumber} is already exists", (int)ApiReturnCode.fail);
        //                        }
        //                        _unitOfWork.GeneratorRepository.AddWithHistory(Helpers.LogFilterAttribute.UserId, Generator);
        //                        _unitOfWork.SaveChanges();
        //                        //Add to Civil_Site_Date if there is free space
        //                        TLIallOtherInventoryInst allOtherInventoryInst = new TLIallOtherInventoryInst();
        //                        allOtherInventoryInst.generatorId = Generator.Id;
        //                        _unitOfWork.AllOtherInventoryInstRepository.Add(allOtherInventoryInst);
        //                        _unitOfWork.SaveChanges();
        //                        allOtherInventoryInstId = allOtherInventoryInst.Id;
        //                        if (addGeneratorViewModel.OtherInSite != null && string.IsNullOrEmpty(SiteCode) == false)
        //                        {
        //                            TLIotherInSite otherInSite = new TLIotherInSite();
        //                            otherInSite.SiteCode = SiteCode;
        //                            otherInSite.OtherInSiteStatus = addGeneratorViewModel.OtherInSite.OtherInSiteStatus;
        //                            otherInSite.OtherInventoryStatus = !string.IsNullOrEmpty(addGeneratorViewModel.OtherInSite.OtherInventoryStatus) ?
        //                                addGeneratorViewModel.OtherInSite.OtherInventoryStatus : string.Empty;
        //                            otherInSite.allOtherInventoryInstId = allOtherInventoryInst.Id;
        //                            otherInSite.InstallationDate = addGeneratorViewModel.OtherInSite.InstallationDate;
        //                            otherInSite.ReservedSpace = addGeneratorViewModel.OtherInSite.ReservedSpace;
        //                            _unitOfWork.OtherInSiteRepository.Add(otherInSite);
        //                            _unitOfWork.SaveChanges();
        //                        }
        //                        var CheckOtherInventoryReference = _unitOfWork.OtherInSiteRepository.GetWhere(x => x.SiteCode == SiteCode).ToList();
        //                        if (addGeneratorViewModel.OtherInventoryDistance != null && CheckOtherInventoryReference.Count > 0)
        //                        {
        //                            TLIotherInventoryDistance otherInventoryDistance = new TLIotherInventoryDistance();
        //                            otherInventoryDistance.Distance = addGeneratorViewModel.OtherInventoryDistance.Distance != null ?
        //                                addGeneratorViewModel.OtherInventoryDistance.Distance.Value : 0;
        //                            otherInventoryDistance.Azimuth = addGeneratorViewModel.OtherInventoryDistance.Azimuth != null ?
        //                                addGeneratorViewModel.OtherInventoryDistance.Azimuth.Value : 0;
        //                            otherInventoryDistance.SiteCode = SiteCode;
        //                            otherInventoryDistance.ReferenceOtherInventoryId = addGeneratorViewModel.OtherInventoryDistance.ReferenceOtherInventoryId != null ?
        //                                addGeneratorViewModel.OtherInventoryDistance.ReferenceOtherInventoryId.Value : 0;
        //                            otherInventoryDistance.allOtherInventoryInstId = allOtherInventoryInst.Id;
        //                            _unitOfWork.OtherInventoryDistanceRepository.Add(otherInventoryDistance);
        //                            _unitOfWork.SaveChanges();
        //                        }
        //                        _unitOfWork.DynamicAttInstValueRepository.AddDdynamicAttributeInstallations(UserId, addGeneratorViewModel.dynamicAttribute, TableNameEntity.Id, Generator.Id, ConnectionString);



        //                    }


        //                    else if (OtherInventoryType.TLIsolar.ToString().ToLower() == TableName.ToLower())
        //                    {
        //                        //Add Solar
        //                        var addSolarViewModel = _mapper.Map<AddSolarViewModel>(model);
        //                        var Solar = _mapper.Map<TLIsolar>(addSolarViewModel);
        //                        if (addSolarViewModel.TLIotherInSite.ReservedSpace == true)
        //                        {
        //                            var CheckSpace = _unitOfWork.SiteRepository.CheckSpaces(UserId, SiteCode, TableName, addSolarViewModel.SolarLibraryId, addSolarViewModel.SpaceInstallation, null).Message;
        //                            if (CheckSpace != "Success")
        //                            {
        //                                return new Response<ObjectInstAtts>(true, null, null, CheckSpace, (int)Helpers.Constants.ApiReturnCode.fail);
        //                            }
        //                        }

        //                        TLIotherInSite CheckName = _unitOfWork.OtherInSiteRepository.GetIncludeWhereFirst(x => !x.Dismantle && !x.allOtherInventoryInst.Draft &&
        //                            (x.allOtherInventoryInst.solarId != null ? x.allOtherInventoryInst.solar.Name.ToLower() == Solar.Name.ToLower() : false) &&
        //                            x.SiteCode.ToLower() == SiteCode.ToLower(),
        //                                x => x.allOtherInventoryInst, x => x.allOtherInventoryInst.solar);

        //                        if (CheckName != null)
        //                            return new Response<ObjectInstAtts>(true, null, null, $"This name {Solar.Name} is already exists", (int)ApiReturnCode.fail);

        //                        //Check Validations
        //                        bool test = false;
        //                        string CheckDependencyValidation = this.CheckDependencyValidation(model, TableName, SiteCode);

        //                        if (!string.IsNullOrEmpty(CheckDependencyValidation))
        //                            return new Response<ObjectInstAtts>(true, null, null, CheckDependencyValidation, (int)ApiReturnCode.fail);

        //                        string CheckGeneralValidation = CheckGeneralValidationFunction(addSolarViewModel.TLIdynamicAttInstValue, TableNameEntity.TableName);

        //                        if (!string.IsNullOrEmpty(CheckGeneralValidation))
        //                            return new Response<ObjectInstAtts>(true, null, null, CheckGeneralValidation, (int)ApiReturnCode.fail);

        //                        test = true;
        //                        if (test == true)
        //                        {
        //                            _unitOfWork.SolarRepository.AddWithHistory(Helpers.LogFilterAttribute.UserId, Solar);
        //                            _unitOfWork.SaveChanges();
        //                            //Add to Civil_Site_Date if there is free space
        //                            TLIallOtherInventoryInst allOtherInventoryInst = new TLIallOtherInventoryInst();
        //                            allOtherInventoryInst.solarId = Solar.Id;
        //                            _unitOfWork.AllOtherInventoryInstRepository.Add(allOtherInventoryInst);
        //                            _unitOfWork.SaveChanges();
        //                            allOtherInventoryInstId = allOtherInventoryInst.Id;
        //                            if (addSolarViewModel.TLIotherInSite != null && string.IsNullOrEmpty(SiteCode) == false)
        //                            {
        //                                TLIotherInSite otherInSite = new TLIotherInSite();
        //                                otherInSite.SiteCode = SiteCode;
        //                                otherInSite.OtherInSiteStatus = addSolarViewModel.TLIotherInSite.OtherInSiteStatus;
        //                                otherInSite.OtherInventoryStatus = addSolarViewModel.TLIotherInSite.OtherInventoryStatus;
        //                                otherInSite.allOtherInventoryInstId = allOtherInventoryInst.Id;
        //                                otherInSite.InstallationDate = addSolarViewModel.TLIotherInSite.InstallationDate;
        //                                otherInSite.ReservedSpace = addSolarViewModel.TLIotherInSite.ReservedSpace;
        //                                _unitOfWork.OtherInSiteRepository.Add(otherInSite);
        //                                _unitOfWork.SaveChanges();
        //                            }
        //                            var CheckOtherInventoryReference = _unitOfWork.OtherInSiteRepository.GetWhere(x => x.SiteCode == SiteCode).ToList();
        //                            if (addSolarViewModel.TLIotherInventoryDistance != null && CheckOtherInventoryReference.Count > 0)
        //                            {
        //                                TLIotherInventoryDistance otherInventoryDistance = new TLIotherInventoryDistance();
        //                                otherInventoryDistance.Distance = addSolarViewModel.TLIotherInventoryDistance.Distance != null ?
        //                                    addSolarViewModel.TLIotherInventoryDistance.Distance.Value : 0;
        //                                otherInventoryDistance.Azimuth = addSolarViewModel.TLIotherInventoryDistance.Azimuth != null ?
        //                                    addSolarViewModel.TLIotherInventoryDistance.Azimuth.Value : 0;
        //                                otherInventoryDistance.SiteCode = SiteCode;
        //                                otherInventoryDistance.ReferenceOtherInventoryId = addSolarViewModel.TLIotherInventoryDistance.ReferenceOtherInventoryId != null ?
        //                                    addSolarViewModel.TLIotherInventoryDistance.ReferenceOtherInventoryId.Value : 0;
        //                                otherInventoryDistance.allOtherInventoryInstId = allOtherInventoryInst.Id;
        //                                _unitOfWork.OtherInventoryDistanceRepository.Add(otherInventoryDistance);
        //                                _unitOfWork.SaveChanges();
        //                            }
        //                            //if (addSolarViewModel.TLIotherInSite.ReservedSpace == true)
        //                            //{
        //                            //    _unitOfWork.SiteRepository.UpdateReservedSpace(SiteCode, addSolarViewModel.SpaceInstallation);
        //                            //}
        //                            //Add Dynamic Attributes
        //                            if (addSolarViewModel.TLIdynamicAttInstValue != null ? addSolarViewModel.TLIdynamicAttInstValue.Count > 0 : false)
        //                            {
        //                                foreach (var DynamicAttInstValue in addSolarViewModel.TLIdynamicAttInstValue)
        //                                {
        //                                    _unitOfWork.DynamicAttInstValueRepository.AddDynamicInstAtts(DynamicAttInstValue, TableNameEntity.Id, Solar.Id);
        //                                }
        //                            }
        //                            //AddHistory(addSolarViewModel.ticketAtt, allOtherInventoryInstId, "Insert");
        //                        }
        //                        else
        //                        {
        //                            return new Response<ObjectInstAtts>(true, null, null, ErrorMessage, (int)ApiReturnCode.fail);
        //                        }

        //                    }
        //                    if (TaskId != null)
        //                    {
        //                        var Submit = _unitOfWork.SiteRepository.SubmitTaskByTLI(TaskId);
        //                        var result = Submit.Result;
        //                        if (result.result == true && result.errorMessage == null)
        //                        {
        //                            _unitOfWork.SaveChanges();
        //                            transaction.Complete();
        //                        }
        //                        else
        //                        {
        //                            transaction.Dispose();
        //                            return new Response<ObjectInstAtts>(true, null, null, result.errorMessage.ToString(), (int)ApiReturnCode.fail);
        //                        }
        //                    }
        //                    else
        //                    {
        //                        _unitOfWork.SaveChanges();
        //                        transaction.Complete();
        //                    }
        //                    return new Response<ObjectInstAtts>();
        //                }
        //                catch (Exception err)
        //                {
        //                    tran.Rollback();
        //                    return new Response<ObjectInstAtts>(true, null, null, err.Message, (int)ApiReturnCode.fail);
        //                }
        //            }
        //        }
        //    }
        //}
        public Response<AddGeneratorInstallationObject> AddGeneratorInstallation(AddGeneratorInstallationObject addGeneratorInstallationObject, string SiteCode, string ConnectionString, int? TaskId, int UserId,bool ExternalSys)
        {
            int allOtherInventoryInstId = 0;
            using (var con = new OracleConnection(ConnectionString))
            {
                con.Open();
                using (var tran = con.BeginTransaction())
                {
                    using (TransactionScope transaction = new TransactionScope())
                    {
                        try
                        {
                            string ErrorMessage = string.Empty;
                            var TableNameEntity = _unitOfWork.TablesNamesRepository.GetWhereFirst(o => o.TableName == "TLIgenerator");

                            var Generator = _mapper.Map<TLIgenerator>(addGeneratorInstallationObject.installationAttributes);
                            if (addGeneratorInstallationObject.OtherInSite.ReservedSpace == true)
                            {
                                var CheckSpace = _unitOfWork.SiteRepository.CheckSpaces(UserId, SiteCode, "TLIgenerator", addGeneratorInstallationObject.GeneratorType.GeneratorLibraryId, addGeneratorInstallationObject.installationAttributes.SpaceInstallation, null).Message;
                                if (CheckSpace != "Success")
                                {
                                    return new Response<AddGeneratorInstallationObject>(true, null, null, CheckSpace, (int)Helpers.Constants.ApiReturnCode.fail);
                                }
                            }
                            if(Generator.BaseExisting == true && Generator.BaseGeneratorTypeId == null)
                                return new Response<AddGeneratorInstallationObject>(true, null, null, "BaseGeneratorType can not be null because BaseExisting is true ", (int)ApiReturnCode.fail);
                            
                            //string CheckDependencyValidation = this.CheckDependencyValidation(model, TableName, SiteCode);

                            //if (!string.IsNullOrEmpty(CheckDependencyValidation))
                            //    return new Response<ObjectInstAtts>(true, null, null, CheckDependencyValidation, (int)ApiReturnCode.fail);

                            //string CheckGeneralValidation = CheckGeneralValidationFunction(addGeneratorViewModel.TLIdynamicAttInstValue, TableNameEntity.TableName);

                            //if (!string.IsNullOrEmpty(CheckGeneralValidation))
                            //    return new Response<ObjectInstAtts>(true, null, null, CheckGeneralValidation, (int)ApiReturnCode.fail);


                            var CheckName = _dbContext.MV_GENERATOR_VIEW
                               .Where(x => x.Name != null &&
                                           x.Name.ToLower() == Generator.Name.ToLower() &&
                                           !x.Dismantle &&
                                           x.SITECODE.ToLower() == SiteCode.ToLower())
                               .ToList();

                            if (CheckName.Count >0 )
                                return new Response<AddGeneratorInstallationObject>(true, null, null, $"This name {Generator.Name} is already exists", (int)ApiReturnCode.fail);

                            var CheckSerialNumber = _dbContext.TLIotherInSite.Include(x=>x.allOtherInventoryInst).ThenInclude(x=>x.generator)
                                .FirstOrDefault(x => x.allOtherInventoryInst.generator.SerialNumber.ToLower()
                                == Generator.SerialNumber.ToLower() && !x.Dismantle);
                        
                            if (CheckSerialNumber != null)
                                return new Response<AddGeneratorInstallationObject>(true, null, null, $"The serial number {Generator.SerialNumber} is already exists", (int)ApiReturnCode.fail);
                            Generator.GeneratorLibraryId = addGeneratorInstallationObject.GeneratorType.GeneratorLibraryId;
                           var HistoryId= _unitOfWork.GeneratorRepository.AddWithHInsatallation(UserId,null, Generator,SiteCode,ExternalSys);
                            _unitOfWork.SaveChanges();

                            TLIallOtherInventoryInst allOtherInventoryInst = new TLIallOtherInventoryInst();
                            allOtherInventoryInst.generatorId = Generator.Id;
                            _unitOfWork.AllOtherInventoryInstRepository.AddWithHistory(UserId,allOtherInventoryInst);
                            _unitOfWork.SaveChanges();
                            allOtherInventoryInstId = allOtherInventoryInst.Id;
                            if (addGeneratorInstallationObject.OtherInSite != null && string.IsNullOrEmpty(SiteCode) == false)
                            {
                                TLIotherInSite otherInSite = new TLIotherInSite();
                                otherInSite.SiteCode = SiteCode;
                                otherInSite.OtherInSiteStatus = addGeneratorInstallationObject.OtherInSite?.OtherInSiteStatus;
                                otherInSite.OtherInventoryStatus = addGeneratorInstallationObject.OtherInSite?.OtherInventoryStatus;
                                otherInSite.allOtherInventoryInstId = allOtherInventoryInst.Id;
                                otherInSite.InstallationDate = addGeneratorInstallationObject.OtherInSite.InstallationDate;
                                otherInSite.ReservedSpace = addGeneratorInstallationObject.OtherInSite.ReservedSpace;
                                var TabelTLIotherInSite = _unitOfWork.TablesNamesRepository.GetWhereFirst(x => x.TableName == "TLIotherInSite").Id;
                                _unitOfWork.OtherInSiteRepository.AddWithHDynamic(UserId, TabelTLIotherInSite, otherInSite,HistoryId);
                                _unitOfWork.SaveChanges();
                            }
                            var CheckOtherInventoryReference = _unitOfWork.OtherInSiteRepository.GetWhere(x => x.SiteCode == SiteCode).ToList();
                            if (addGeneratorInstallationObject.OtherInventoryDistance != null&& addGeneratorInstallationObject.OtherInventoryDistance.ReferenceOtherInventoryId != null && CheckOtherInventoryReference.Count > 0)
                            {
                                var OtherInventoryDistance = _unitOfWork.OtherInventoryDistanceRepository.GetWhere(x => x.ReferenceOtherInventoryId
                                                                == addGeneratorInstallationObject.OtherInventoryDistance.ReferenceOtherInventoryId && x.Distance == addGeneratorInstallationObject.OtherInventoryDistance.Distance
                                                                && x.Azimuth == addGeneratorInstallationObject.OtherInventoryDistance.Azimuth).Select(x => x.ReferenceOtherInventoryId).ToList();
                                var OtherInSite = _unitOfWork.OtherInSiteRepository.GetWhere(x => OtherInventoryDistance.Any(y => y
                                == x.allOtherInventoryInstId) && !x.Dismantle && x.SiteCode.ToLower() == SiteCode.ToLower()).ToList();
                                if (OtherInSite.Count > 0)
                                {
                                    return new Response<AddGeneratorInstallationObject>(false, null, null, "can not select the otherinventory reference on same distance and azimuth because found other otherinventory reference in same place", (int)Helpers.Constants.ApiReturnCode.fail);
                                }
                                TLIotherInventoryDistance otherInventoryDistance = new TLIotherInventoryDistance();
                                otherInventoryDistance.Distance = addGeneratorInstallationObject.OtherInventoryDistance.Distance;
                                otherInventoryDistance.Azimuth = addGeneratorInstallationObject.OtherInventoryDistance.Azimuth;
                                otherInventoryDistance.SiteCode = SiteCode;
                                otherInventoryDistance.ReferenceOtherInventoryId = addGeneratorInstallationObject.OtherInventoryDistance?.ReferenceOtherInventoryId;
                                otherInventoryDistance.allOtherInventoryInstId = allOtherInventoryInst.Id;
                                var TabelTLIotherInventoryDistance = _unitOfWork.TablesNamesRepository.GetWhereFirst(x => x.TableName == "TLIotherInventoryDistance").Id;
                                _unitOfWork.OtherInventoryDistanceRepository.AddWithHDynamic(UserId, TabelTLIotherInventoryDistance, otherInventoryDistance, HistoryId);
                                _unitOfWork.SaveChanges();
                            }
                            var sortedIds = _unitOfWork.CivilWithLegsRepository.ProcessDynamicAttributes(addGeneratorInstallationObject, HistoryId);

                            if (addGeneratorInstallationObject.dynamicAttribute != null && addGeneratorInstallationObject.dynamicAttribute.Count > 0)
                            {
                                var sortedDynamicAttributes = addGeneratorInstallationObject.dynamicAttribute
                                    .OrderBy(item => sortedIds.IndexOf(item.id))
                                    .ToList();

                                foreach (var item in sortedDynamicAttributes)
                                {
                                    var Message = _unitOfWork.CivilWithLegsRepository.CheckDynamicValidationAndDependence(item.id, item.value, Generator.Id, HistoryId).Message;
                                    if (Message != "Success")
                                    {
                                        return new Response<AddGeneratorInstallationObject>(true, null, null, Message, (int)Helpers.Constants.ApiReturnCode.fail);
                                    }
                                }
                            }
                            if (TaskId != null)
                            {
                                var Submit = _unitOfWork.SiteRepository.SubmitTaskByTLI(TaskId);
                                var result = Submit.Result;
                                if (result.result == true && result.errorMessage == null)
                                {
                                    _unitOfWork.SaveChanges();
                                    transaction.Complete();
                                }
                                else
                                {
                                    transaction.Dispose();
                                    return new Response<AddGeneratorInstallationObject>(true, null, null, result.errorMessage.ToString(), (int)ApiReturnCode.fail);
                                }
                            }
                            else
                            {
                                _unitOfWork.SaveChanges();
                                transaction.Complete();
                            }
                            Task.Run(() => _unitOfWork.CivilWithLegsRepository.RefreshView(ConnectionString));
                            return new Response<AddGeneratorInstallationObject>();
                        }
                        catch (Exception err)
                        {
                            tran.Rollback();
                            return new Response<AddGeneratorInstallationObject>(true, null, null, err.Message, (int)ApiReturnCode.fail);
                        }
                    }
                }
            }
        }
        public Response<AddCabinetPowerInstallation> AddCabinetPowerInstallation(AddCabinetPowerInstallation addCabinetPowerInstallation, string SiteCode, string ConnectionString, int? TaskId, int UserId, bool ExternalSys)
        {
            int allOtherInventoryInstId = 0;
            using (var con = new OracleConnection(ConnectionString))
            {
                con.Open();
                using (var tran = con.BeginTransaction())
                {
                    using (TransactionScope transaction = new TransactionScope())
                    {
                        try
                        {
                            string ErrorMessage = string.Empty;
                            var TableNameEntity = _unitOfWork.TablesNamesRepository.GetWhereFirst(o => o.TableName == "TLIcabinet");

                            var CabinetPower = _mapper.Map<TLIcabinet>(addCabinetPowerInstallation.installationAttributes);
                            if (addCabinetPowerInstallation.OtherInSite.ReservedSpace == true)
                            {
                                var CheckSpace = _unitOfWork.SiteRepository.CheckSpaces(UserId, SiteCode, "TLIcabinet", addCabinetPowerInstallation.CabinetPowerType.CabinetPowerLibraryId, addCabinetPowerInstallation.installationAttributes.SpaceInstallation, null).Message;
                                if (CheckSpace != "Success")
                                {
                                    return new Response<AddCabinetPowerInstallation>(true, null, null, CheckSpace, (int)Helpers.Constants.ApiReturnCode.fail);
                                }
                            }

                            //string CheckDependencyValidation = this.CheckDependencyValidation(model, TableName, SiteCode);

                            //if (!string.IsNullOrEmpty(CheckDependencyValidation))
                            //    return new Response<ObjectInstAtts>(true, null, null, CheckDependencyValidation, (int)ApiReturnCode.fail);

                            //string CheckGeneralValidation = CheckGeneralValidationFunction(addGeneratorViewModel.TLIdynamicAttInstValue, TableNameEntity.TableName);

                            //if (!string.IsNullOrEmpty(CheckGeneralValidation))
                            //    return new Response<ObjectInstAtts>(true, null, null, CheckGeneralValidation, (int)ApiReturnCode.fail);


                            var CheckName = _dbContext.MV_CABINET_POWER_VIEW
                               .Where(x => x.Name != null &&
                                           x.Name.ToLower() == CabinetPower.Name.ToLower() &&
                                           !x.Dismantle &&
                                           x.SITECODE.ToLower() == SiteCode.ToLower())
                               .ToList();

                            if (CheckName.Count > 0)
                                return new Response<AddCabinetPowerInstallation>(true, null, null, $"This name {CabinetPower.Name} is already exists", (int)ApiReturnCode.fail);

                            CabinetPower.CabinetPowerLibraryId = addCabinetPowerInstallation.CabinetPowerType.CabinetPowerLibraryId;
                           var HistoryId= _unitOfWork.CabinetRepository.AddWithHInsatallation(UserId,null, CabinetPower,SiteCode,ExternalSys);
                            _unitOfWork.SaveChanges();

                            TLIallOtherInventoryInst allOtherInventoryInst = new TLIallOtherInventoryInst();
                            allOtherInventoryInst.cabinetId = CabinetPower.Id;
                            _unitOfWork.AllOtherInventoryInstRepository.AddWithHistory(UserId, allOtherInventoryInst);
                            _unitOfWork.SaveChanges();
                            allOtherInventoryInstId = allOtherInventoryInst.Id;
                            if (addCabinetPowerInstallation.OtherInSite != null && string.IsNullOrEmpty(SiteCode) == false)
                            {
                                TLIotherInSite otherInSite = new TLIotherInSite();
                                otherInSite.SiteCode = SiteCode;
                                otherInSite.OtherInSiteStatus = addCabinetPowerInstallation.OtherInSite?.OtherInSiteStatus;
                                otherInSite.OtherInventoryStatus = addCabinetPowerInstallation.OtherInSite?.OtherInventoryStatus;
                                otherInSite.allOtherInventoryInstId = allOtherInventoryInst.Id;
                                otherInSite.InstallationDate = addCabinetPowerInstallation.OtherInSite.InstallationDate;
                                otherInSite.ReservedSpace = addCabinetPowerInstallation.OtherInSite.ReservedSpace;
                                var TabelTLIotherInSite = _unitOfWork.TablesNamesRepository.GetWhereFirst(x => x.TableName == "TLIotherInSite").Id;
                                _unitOfWork.OtherInSiteRepository.AddWithHDynamic(UserId, TabelTLIotherInSite, otherInSite,HistoryId);
                                _unitOfWork.SaveChanges();
                            }
                            var CheckOtherInventoryReference = _unitOfWork.OtherInSiteRepository.GetWhere(x => x.SiteCode == SiteCode).ToList();
                            if (addCabinetPowerInstallation.OtherInventoryDistance != null && addCabinetPowerInstallation.OtherInventoryDistance.ReferenceOtherInventoryId != null && CheckOtherInventoryReference.Count > 0)
                            {
                                var OtherInventoryDistance = _unitOfWork.OtherInventoryDistanceRepository.GetWhere(x => x.ReferenceOtherInventoryId
                                == addCabinetPowerInstallation.OtherInventoryDistance.ReferenceOtherInventoryId && x.Distance == addCabinetPowerInstallation.OtherInventoryDistance.Distance
                                && x.Azimuth == addCabinetPowerInstallation.OtherInventoryDistance.Azimuth).Select(x=>x.ReferenceOtherInventoryId).ToList();
                                var OtherInSite = _unitOfWork.OtherInSiteRepository.GetWhere(x => OtherInventoryDistance.Any(y => y
                                == x.allOtherInventoryInstId) && !x.Dismantle && x.SiteCode.ToLower() == SiteCode.ToLower()).ToList();
                                if (OtherInSite.Count > 0)
                                {
                                    return new Response<AddCabinetPowerInstallation>(false, null, null, "can not select the otherinventory reference on same distance and azimuth because found other otherinventory reference in same place", (int)Helpers.Constants.ApiReturnCode.fail);
                                }
                                TLIotherInventoryDistance otherInventoryDistance = new TLIotherInventoryDistance();
                                otherInventoryDistance.Distance = addCabinetPowerInstallation.OtherInventoryDistance.Distance;
                                otherInventoryDistance.Azimuth = addCabinetPowerInstallation.OtherInventoryDistance.Azimuth;
                                otherInventoryDistance.SiteCode = SiteCode;
                                otherInventoryDistance.ReferenceOtherInventoryId = addCabinetPowerInstallation.OtherInventoryDistance?.ReferenceOtherInventoryId;
                                otherInventoryDistance.allOtherInventoryInstId = allOtherInventoryInst.Id;
                                var TabelTLIotherInventoryDistance = _unitOfWork.TablesNamesRepository.GetWhereFirst(x => x.TableName == "TLIotherInventoryDistance").Id;
                                _unitOfWork.OtherInventoryDistanceRepository.AddWithHDynamic(UserId, TabelTLIotherInventoryDistance, otherInventoryDistance,HistoryId);
                                _unitOfWork.SaveChanges();
                            }
                            var sortedIds = _unitOfWork.CivilWithLegsRepository.ProcessDynamicAttributes(addCabinetPowerInstallation, HistoryId);

                            if (addCabinetPowerInstallation.dynamicAttribute != null && addCabinetPowerInstallation.dynamicAttribute.Count > 0)
                            {
                                var sortedDynamicAttributes = addCabinetPowerInstallation.dynamicAttribute
                                    .OrderBy(item => sortedIds.IndexOf(item.id))
                                    .ToList();

                                foreach (var item in sortedDynamicAttributes)
                                {
                                    var Message = _unitOfWork.CivilWithLegsRepository.CheckDynamicValidationAndDependence(item.id, item.value, CabinetPower.Id, HistoryId).Message;
                                    if (Message != "Success")
                                    {
                                        return new Response<AddCabinetPowerInstallation>(true, null, null, Message, (int)Helpers.Constants.ApiReturnCode.fail);
                                    }
                                }
                            }
                            if (TaskId != null)
                            {
                                var Submit = _unitOfWork.SiteRepository.SubmitTaskByTLI(TaskId);
                                var result = Submit.Result;
                                if (result.result == true && result.errorMessage == null)
                                {
                                    _unitOfWork.SaveChanges();
                                    transaction.Complete();
                                }
                                else
                                {
                                    transaction.Dispose();
                                    return new Response<AddCabinetPowerInstallation>(true, null, null, result.errorMessage.ToString(), (int)ApiReturnCode.fail);
                                }
                            }
                            else
                            {
                                _unitOfWork.SaveChanges();
                                transaction.Complete();
                            }
                            Task.Run(() => _unitOfWork.CivilWithLegsRepository.RefreshView(ConnectionString));
                            return new Response<AddCabinetPowerInstallation>();
                        }
                        catch (Exception err)
                        {
                            tran.Rollback();
                            return new Response<AddCabinetPowerInstallation>(true, null, null, err.Message, (int)ApiReturnCode.fail);
                        }
                    }
                }
            }
        }
        public Response<AddCabinetTelecomInstallationObject> AddCabinetTelecomInstallation(AddCabinetTelecomInstallationObject addCabinetTelecomInstallationObject, string SiteCode, string ConnectionString, int? TaskId, int UserId, bool ExternalSys)
        {
            int allOtherInventoryInstId = 0;
            using (var con = new OracleConnection(ConnectionString))
            {
                con.Open();
                using (var tran = con.BeginTransaction())
                {
                    using (TransactionScope transaction = new TransactionScope())
                    {
                        try
                        {
                            string ErrorMessage = string.Empty;
                            var TableNameEntity = _unitOfWork.TablesNamesRepository.GetWhereFirst(o => o.TableName == "TLIcabinet");

                            var CabinetTelecom = _mapper.Map<TLIcabinet>(addCabinetTelecomInstallationObject.installationAttributes);
                            if (addCabinetTelecomInstallationObject.OtherInSite.ReservedSpace == true)
                            {
                                var CheckSpace = _unitOfWork.SiteRepository.CheckSpaces(UserId, SiteCode, "TLIcabinet", addCabinetTelecomInstallationObject.CabinetTelecomType.CabinetTelecomLibraryId, addCabinetTelecomInstallationObject.installationAttributes.SpaceInstallation, null).Message;
                                if (CheckSpace != "Success")
                                {
                                    return new Response<AddCabinetTelecomInstallationObject>(true, null, null, CheckSpace, (int)Helpers.Constants.ApiReturnCode.fail);
                                }
                            }

                            //string CheckDependencyValidation = this.CheckDependencyValidation(model, TableName, SiteCode);

                            //if (!string.IsNullOrEmpty(CheckDependencyValidation))
                            //    return new Response<ObjectInstAtts>(true, null, null, CheckDependencyValidation, (int)ApiReturnCode.fail);

                            //string CheckGeneralValidation = CheckGeneralValidationFunction(addGeneratorViewModel.TLIdynamicAttInstValue, TableNameEntity.TableName);

                            //if (!string.IsNullOrEmpty(CheckGeneralValidation))
                            //    return new Response<ObjectInstAtts>(true, null, null, CheckGeneralValidation, (int)ApiReturnCode.fail);


                            var CheckName = _dbContext.MV_CABINET_TELECOM_VIEW
                               .Where(x => x.Name != null &&
                                           x.Name.ToLower() == CabinetTelecom.Name.ToLower() &&
                                           !x.Dismantle &&
                                           x.SITECODE.ToLower() == SiteCode.ToLower())
                               .ToList();

                            if (CheckName.Count > 0)
                                return new Response<AddCabinetTelecomInstallationObject>(true, null, null, $"This name {CabinetTelecom.Name} is already exists", (int)ApiReturnCode.fail);

                            CabinetTelecom.CabinetTelecomLibraryId = addCabinetTelecomInstallationObject.CabinetTelecomType.CabinetTelecomLibraryId;
                            var HistoryId=_unitOfWork.CabinetRepository.AddWithHInsatallation(UserId,null, CabinetTelecom,SiteCode,ExternalSys);
                            _unitOfWork.SaveChanges();

                            TLIallOtherInventoryInst allOtherInventoryInst = new TLIallOtherInventoryInst();
                            allOtherInventoryInst.cabinetId = CabinetTelecom.Id;
                            _unitOfWork.AllOtherInventoryInstRepository.AddWithHistory(UserId, allOtherInventoryInst);
                            _unitOfWork.SaveChanges();
                            allOtherInventoryInstId = allOtherInventoryInst.Id;
                            if (addCabinetTelecomInstallationObject.OtherInSite != null && string.IsNullOrEmpty(SiteCode) == false)
                            {
                                TLIotherInSite otherInSite = new TLIotherInSite();
                                otherInSite.SiteCode = SiteCode;
                                otherInSite.OtherInSiteStatus = addCabinetTelecomInstallationObject.OtherInSite?.OtherInSiteStatus;
                                otherInSite.OtherInventoryStatus = addCabinetTelecomInstallationObject.OtherInSite?.OtherInventoryStatus;
                                otherInSite.allOtherInventoryInstId = allOtherInventoryInst.Id;
                                otherInSite.InstallationDate = addCabinetTelecomInstallationObject.OtherInSite.InstallationDate;
                                otherInSite.ReservedSpace = addCabinetTelecomInstallationObject.OtherInSite.ReservedSpace;
                                var TabelTLIotherInSite = _unitOfWork.TablesNamesRepository.GetWhereFirst(x => x.TableName == "TLIotherInSite").Id;
                                _unitOfWork.OtherInSiteRepository.AddWithHDynamic(UserId, TabelTLIotherInSite, otherInSite,HistoryId);
                                _unitOfWork.SaveChanges();
                            }
                            var CheckOtherInventoryReference = _unitOfWork.OtherInSiteRepository.GetWhere(x => x.SiteCode == SiteCode).ToList();
                            if (addCabinetTelecomInstallationObject.OtherInventoryDistance != null && addCabinetTelecomInstallationObject.OtherInventoryDistance.ReferenceOtherInventoryId != null && CheckOtherInventoryReference.Count > 0)
                            {

                                var OtherInventoryDistance = _unitOfWork.OtherInventoryDistanceRepository.GetWhere(x => x.ReferenceOtherInventoryId
                                == addCabinetTelecomInstallationObject.OtherInventoryDistance.ReferenceOtherInventoryId && x.Distance == addCabinetTelecomInstallationObject.OtherInventoryDistance.Distance
                                && x.Azimuth == addCabinetTelecomInstallationObject.OtherInventoryDistance.Azimuth).Select(x => x.ReferenceOtherInventoryId).ToList();
                                var OtherInSite = _unitOfWork.OtherInSiteRepository.GetWhere(x => OtherInventoryDistance.Any(y => y
                                == x.allOtherInventoryInstId) && !x.Dismantle && x.SiteCode.ToLower() == SiteCode.ToLower()).ToList();
                                if (OtherInSite.Count > 0)
                                {
                                    return new Response<AddCabinetTelecomInstallationObject>(false, null, null, "can not select the otherinventory reference on same distance and azimuth because found other otherinventory reference in same place", (int)Helpers.Constants.ApiReturnCode.fail);
                                }
                                TLIotherInventoryDistance otherInventoryDistance = new TLIotherInventoryDistance();
                                otherInventoryDistance.Distance = addCabinetTelecomInstallationObject.OtherInventoryDistance.Distance;
                                otherInventoryDistance.Azimuth = addCabinetTelecomInstallationObject.OtherInventoryDistance.Azimuth;
                                otherInventoryDistance.SiteCode = SiteCode;
                                otherInventoryDistance.ReferenceOtherInventoryId = addCabinetTelecomInstallationObject.OtherInventoryDistance?.ReferenceOtherInventoryId;
                                otherInventoryDistance.allOtherInventoryInstId = allOtherInventoryInst.Id;
                                var TabelTLIotherInventoryDistance = _unitOfWork.TablesNamesRepository.GetWhereFirst(x => x.TableName == "TLIotherInventoryDistance").Id;
                                _unitOfWork.OtherInventoryDistanceRepository.AddWithHDynamic(UserId, TabelTLIotherInventoryDistance, otherInventoryDistance,HistoryId);
                                _unitOfWork.SaveChanges();
                            }
                            var sortedIds = _unitOfWork.CivilWithLegsRepository.ProcessDynamicAttributes(addCabinetTelecomInstallationObject, HistoryId);

                            if (addCabinetTelecomInstallationObject.dynamicAttribute != null && addCabinetTelecomInstallationObject.dynamicAttribute.Count > 0)
                            {
                                var sortedDynamicAttributes = addCabinetTelecomInstallationObject.dynamicAttribute
                                    .OrderBy(item => sortedIds.IndexOf(item.id))
                                    .ToList();

                                foreach (var item in sortedDynamicAttributes)
                                {
                                    var Message = _unitOfWork.CivilWithLegsRepository.CheckDynamicValidationAndDependence(item.id, item.value, CabinetTelecom.Id, HistoryId).Message;
                                    if (Message != "Success")
                                    {
                                        return new Response<AddCabinetTelecomInstallationObject>(true, null, null, Message, (int)Helpers.Constants.ApiReturnCode.fail);
                                    }
                                }
                            }
                            if (TaskId != null)
                            {
                                var Submit = _unitOfWork.SiteRepository.SubmitTaskByTLI(TaskId);
                                var result = Submit.Result;
                                if (result.result == true && result.errorMessage == null)
                                {
                                    _unitOfWork.SaveChanges();
                                    transaction.Complete();
                                }
                                else
                                {
                                    transaction.Dispose();
                                    return new Response<AddCabinetTelecomInstallationObject>(true, null, null, result.errorMessage.ToString(), (int)ApiReturnCode.fail);
                                }
                            }
                            else
                            {
                                _unitOfWork.SaveChanges();
                                transaction.Complete();
                            }
                            Task.Run(() => _unitOfWork.CivilWithLegsRepository.RefreshView(ConnectionString));
                            return new Response<AddCabinetTelecomInstallationObject>();
                        }
                        catch (Exception err)
                        {
                            tran.Rollback();
                            return new Response<AddCabinetTelecomInstallationObject>(true, null, null, err.Message, (int)ApiReturnCode.fail);
                        }
                    }
                }
            }
        }
        public Response<AddSolarInstallationObject> AddSolarInstallation(AddSolarInstallationObject addSolarInstallationObject, string SiteCode, string ConnectionString, int? TaskId, int UserId, bool ExternalSys)
        {
            int allOtherInventoryInstId = 0;
            using (var con = new OracleConnection(ConnectionString))
            {
                con.Open();
                using (var tran = con.BeginTransaction())
                {
                    using (TransactionScope transaction = new TransactionScope())
                    {
                        try
                        {
                            string ErrorMessage = string.Empty;
                            var TableNameEntity = _unitOfWork.TablesNamesRepository.GetWhereFirst(o => o.TableName == "TLIsolar");

                            var Solar = _mapper.Map<TLIsolar>(addSolarInstallationObject.installationAttributes);
                            if (addSolarInstallationObject.OtherInSite.ReservedSpace == true)
                            {
                                var CheckSpace = _unitOfWork.SiteRepository.CheckSpaces(UserId, SiteCode, TableNameEntity.TableName, addSolarInstallationObject.SolarType.SolarLibraryId, addSolarInstallationObject.installationAttributes.SpaceInstallation, null).Message;
                                if (CheckSpace != "Success")
                                {
                                    return new Response<AddSolarInstallationObject>(true, null, null, CheckSpace, (int)Helpers.Constants.ApiReturnCode.fail);
                                }
                            }
                           
                            //string CheckDependencyValidation = this.CheckDependencyValidation(model, TableName, SiteCode);

                            //if (!string.IsNullOrEmpty(CheckDependencyValidation))
                            //    return new Response<ObjectInstAtts>(true, null, null, CheckDependencyValidation, (int)ApiReturnCode.fail);

                            //string CheckGeneralValidation = CheckGeneralValidationFunction(addSolarViewModel.TLIdynamicAttInstValue, TableNameEntity.TableName);

                            //if (!string.IsNullOrEmpty(CheckGeneralValidation))
                            //    return new Response<ObjectInstAtts>(true, null, null, CheckGeneralValidation, (int)ApiReturnCode.fail);


                            var CheckName = _dbContext.MV_SOLAR_VIEW
                               .Where(x => x.Name != null &&
                                           x.Name.ToLower() == Solar.Name.ToLower() &&
                                           !x.Dismantle &&
                                           x.SITECODE.ToLower() == SiteCode.ToLower())
                               .ToList();

                            if (CheckName.Count > 0)
                                return new Response<AddSolarInstallationObject>(true, null, null, $"This name {Solar.Name} is already exists", (int)ApiReturnCode.fail);

                            Solar.SolarLibraryId = addSolarInstallationObject.SolarType.SolarLibraryId;
                            var HistoryId=_unitOfWork.SolarRepository.AddWithHInsatallation(UserId,null, Solar,SiteCode,ExternalSys);
                            _unitOfWork.SaveChanges();

                            TLIallOtherInventoryInst allOtherInventoryInst = new TLIallOtherInventoryInst();
                            allOtherInventoryInst.solarId = Solar.Id;
                            _unitOfWork.AllOtherInventoryInstRepository.AddWithHistory(UserId,allOtherInventoryInst);
                            _unitOfWork.SaveChanges();
                            allOtherInventoryInstId = allOtherInventoryInst.Id;
                            if (addSolarInstallationObject.OtherInSite != null && string.IsNullOrEmpty(SiteCode) == false)
                            {
                                TLIotherInSite otherInSite = new TLIotherInSite();
                                otherInSite.SiteCode = SiteCode;
                                otherInSite.OtherInSiteStatus = addSolarInstallationObject.OtherInSite?.OtherInSiteStatus;
                                otherInSite.OtherInventoryStatus = addSolarInstallationObject.OtherInSite?.OtherInventoryStatus;
                                otherInSite.allOtherInventoryInstId = allOtherInventoryInst.Id;
                                otherInSite.InstallationDate = addSolarInstallationObject.OtherInSite.InstallationDate;
                                otherInSite.ReservedSpace = addSolarInstallationObject.OtherInSite.ReservedSpace;
                                var TabelTLIotherInSite = _unitOfWork.TablesNamesRepository.GetWhereFirst(x => x.TableName == "TLIotherInSite").Id;
                                _unitOfWork.OtherInSiteRepository.AddWithHDynamic(UserId, TabelTLIotherInSite, otherInSite,HistoryId);
                                _unitOfWork.SaveChanges();
                            }
                            var CheckOtherInventoryReference = _unitOfWork.OtherInSiteRepository.GetWhere(x => x.SiteCode == SiteCode).ToList();
                            if (addSolarInstallationObject.OtherInventoryDistance != null && addSolarInstallationObject.OtherInventoryDistance.ReferenceOtherInventoryId != null && CheckOtherInventoryReference.Count > 0)
                            {
                                var OtherInventoryDistance = _unitOfWork.OtherInventoryDistanceRepository.GetWhere(x => x.ReferenceOtherInventoryId
                                                               == addSolarInstallationObject.OtherInventoryDistance.ReferenceOtherInventoryId && x.Distance == addSolarInstallationObject.OtherInventoryDistance.Distance
                                                               && x.Azimuth == addSolarInstallationObject.OtherInventoryDistance.Azimuth).Select(x => x.ReferenceOtherInventoryId).ToList();
                                var OtherInSite = _unitOfWork.OtherInSiteRepository.GetWhere(x => OtherInventoryDistance.Any(y => y
                                == x.allOtherInventoryInstId) && !x.Dismantle && x.SiteCode.ToLower() == SiteCode.ToLower()).ToList();
                                if (OtherInSite.Count > 0)
                                {
                                    return new Response<AddSolarInstallationObject>(false, null, null, "can not select the otherinventory reference on same distance and azimuth because found other otherinventory reference in same place", (int)Helpers.Constants.ApiReturnCode.fail);
                                }
                                TLIotherInventoryDistance otherInventoryDistance = new TLIotherInventoryDistance();
                                otherInventoryDistance.Distance = addSolarInstallationObject.OtherInventoryDistance.Distance;
                                otherInventoryDistance.Azimuth = addSolarInstallationObject.OtherInventoryDistance.Azimuth;
                                otherInventoryDistance.SiteCode = SiteCode;
                                otherInventoryDistance.ReferenceOtherInventoryId = addSolarInstallationObject.OtherInventoryDistance?.ReferenceOtherInventoryId;
                                otherInventoryDistance.allOtherInventoryInstId = allOtherInventoryInst.Id;
                                var TabelTLIotherInventoryDistance = _unitOfWork.TablesNamesRepository.GetWhereFirst(x => x.TableName == "TLIotherInventoryDistance").Id;
                                _unitOfWork.OtherInventoryDistanceRepository.AddWithHDynamic(UserId, TabelTLIotherInventoryDistance, otherInventoryDistance,HistoryId);
                                _unitOfWork.SaveChanges();
                            }
                            var sortedIds = _unitOfWork.CivilWithLegsRepository.ProcessDynamicAttributes(addSolarInstallationObject, HistoryId);

                            if (addSolarInstallationObject.dynamicAttribute != null && addSolarInstallationObject.dynamicAttribute.Count > 0)
                            {
                                var sortedDynamicAttributes = addSolarInstallationObject.dynamicAttribute
                                    .OrderBy(item => sortedIds.IndexOf(item.id))
                                    .ToList();

                                foreach (var item in sortedDynamicAttributes)
                                {
                                    var Message = _unitOfWork.CivilWithLegsRepository.CheckDynamicValidationAndDependence(item.id, item.value, Solar.Id, HistoryId).Message;
                                    if (Message != "Success")
                                    {
                                        return new Response<AddSolarInstallationObject>(true, null, null, Message, (int)Helpers.Constants.ApiReturnCode.fail);
                                    }
                                }
                            }
                            if (TaskId != null)
                            {
                                var Submit = _unitOfWork.SiteRepository.SubmitTaskByTLI(TaskId);
                                var result = Submit.Result;
                                if (result.result == true && result.errorMessage == null)
                                {
                                    _unitOfWork.SaveChanges();
                                    transaction.Complete();
                                }
                                else
                                {
                                    transaction.Dispose();
                                    return new Response<AddSolarInstallationObject>(true, null, null, result.errorMessage.ToString(), (int)ApiReturnCode.fail);
                                }
                            }
                            else
                            {
                                _unitOfWork.SaveChanges();
                                transaction.Complete();
                            }
                            Task.Run(() => _unitOfWork.CivilWithLegsRepository.RefreshView(ConnectionString));
                            return new Response<AddSolarInstallationObject>();
                        }
                        catch (Exception err)
                        {
                            tran.Rollback();
                            return new Response<AddSolarInstallationObject>(true, null, null, err.Message, (int)ApiReturnCode.fail);
                        }
                    }
                }
            }
        }
        public Response<GetEnableAttribute> GetGeneratorWithEnableAtt(string? SiteCode, string ConnectionString)
        {
            using (var connection = new OracleConnection(ConnectionString))
            {
                try
                {
                    GetEnableAttribute getEnableAttribute = new GetEnableAttribute();
                    connection.Open();
                    //string storedProcedureName = "create_dynamic_pivot_withleg ";
                    //using (OracleCommand procedureCommand = new OracleCommand(storedProcedureName, connection))
                    //{
                    //    procedureCommand.CommandType = CommandType.StoredProcedure;
                    //    procedureCommand.ExecuteNonQuery();
                    //}
                    var attActivated = _dbContext.TLIattributeViewManagment
                        .Include(x => x.EditableManagmentView)
                        .Include(x => x.AttributeActivated)
                        .Include(x => x.DynamicAtt)
                        .Where(x => x.Enable && x.EditableManagmentView.View == "GeneratorInstallation" &&
                        ((x.AttributeActivatedId != null && x.AttributeActivated.enable) || (x.DynamicAttId != null && !x.DynamicAtt.disable)))
                        .Select(x => new { attribute = x.AttributeActivated.Key, dynamic = x.DynamicAtt.Key, dataType = x.DynamicAtt != null ? x.DynamicAtt.DataType.Name.ToString() : x.AttributeActivated.DataType.ToString() })
                        .OrderByDescending(x => x.attribute.ToLower().StartsWith("name"))
                            .ThenBy(x => x.attribute == null)
                            .ThenBy(x => x.attribute)
                            .ToList();
                    getEnableAttribute.Type = attActivated;
                    List<string> propertyNamesStatic = new List<string>();
                    Dictionary<string, string> propertyNamesDynamic = new Dictionary<string, string>();
                    foreach (var key in attActivated)
                    {
                        if (key.attribute != null)
                        {
                            string name = key.attribute;
                            if (name != "Id" && name.EndsWith("Id"))
                            {
                                string fk = name.Remove(name.Length - 2);
                                propertyNamesStatic.Add(fk);
                            }
                            else
                            {
                                propertyNamesStatic.Add(name);
                            }

                        }
                        else
                        {
                            string name = key.dynamic;
                            string datatype = key.dataType;
                            propertyNamesDynamic.Add(name, datatype);
                        }

                    }
                    propertyNamesStatic.Add("SITECODE");
                    if (SiteCode == null)
                    {
                        if (propertyNamesDynamic.Count == 0)
                        {
                            var query = _dbContext.MV_GENERATOR_VIEW.Where(x =>
                            !x.Dismantle).AsEnumerable()
                            .Select(item => _unitOfWork.CivilWithLegsRepository.BuildDynamicSelect(item, null, propertyNamesStatic, propertyNamesDynamic));
                            int count = query.Count();

                            getEnableAttribute.Model = query;
                            return new Response<GetEnableAttribute>(true, getEnableAttribute, null, "Success", (int)Helpers.Constants.ApiReturnCode.success, count);
                        }
                        else
                        {
                            var query = _dbContext.MV_GENERATOR_VIEW.Where(x =>
                              !x.Dismantle).AsEnumerable()
                        .GroupBy(x => new
                        {

                            Id = x.Id,
                            Name = x.Name,
                            SITECODE = x.SITECODE,
                            SerialNumber = x.SerialNumber,
                            NumberOfFuelTanks = x.NumberOfFuelTanks,
                            BaseExisting = x.BaseExisting,
                            SpaceInstallation = x.SpaceInstallation,
                            VisibleStatus = x.VisibleStatus,
                            BASEGENERATORTYPE = x.BASEGENERATORTYPE,
                            GENERATORLIBRARY = x.GENERATORLIBRARY,
                            Dismantle = x.Dismantle,
                        }).OrderBy(x => x.Key.Name)
                        .Select(x => new { key = x.Key, value = x.ToDictionary(z => z.Key, z => z.INPUTVALUE) })
                        .Select(item => _unitOfWork.CivilWithLegsRepository.BuildDynamicSelect(item.key, item.value, propertyNamesStatic, propertyNamesDynamic));
                            int count = query.Count();
                            getEnableAttribute.Model = query;
                            return new Response<GetEnableAttribute>(true, getEnableAttribute, null, "Success", (int)Helpers.Constants.ApiReturnCode.success, count);
                        }
                    }
                    if (propertyNamesDynamic.Count == 0)
                    {
                        var query = _dbContext.MV_GENERATOR_VIEW.Where(x => x.SITECODE.ToLower() == SiteCode.ToLower()
                        && !x.Dismantle).AsEnumerable()
                    .Select(item => _unitOfWork.CivilWithLegsRepository.BuildDynamicSelect(item, null, propertyNamesStatic, propertyNamesDynamic));
                        int count = query.Count();

                        getEnableAttribute.Model = query;
                        return new Response<GetEnableAttribute>(true, getEnableAttribute, null, "Success", (int)Helpers.Constants.ApiReturnCode.success, count);
                    }
                    else
                    {
                        var query = _dbContext.MV_GENERATOR_VIEW.Where(x => x.SITECODE.ToLower() == SiteCode.ToLower()
                         && !x.Dismantle).AsEnumerable()
                    .GroupBy(x => new
                    {

                        Id = x.Id,
                        Name = x.Name,
                        SITECODE = x.SITECODE,
                        SerialNumber = x.SerialNumber,
                        NumberOfFuelTanks = x.NumberOfFuelTanks,
                        BaseExisting = x.BaseExisting,
                        SpaceInstallation = x.SpaceInstallation,
                        VisibleStatus = x.VisibleStatus,
                        BASEGENERATORTYPE = x.BASEGENERATORTYPE,
                        GENERATORLIBRARY = x.GENERATORLIBRARY,
                        Dismantle = x.Dismantle,
                    }).OrderBy(x => x.Key.Name)
                    .Select(x => new { key = x.Key, value = x.ToDictionary(z => z.Key, z => z.INPUTVALUE) })
                    .Select(item => _unitOfWork.CivilWithLegsRepository.BuildDynamicSelect(item.key, item.value, propertyNamesStatic, propertyNamesDynamic));
                        int count = query.Count();
                        getEnableAttribute.Model = query;
                        return new Response<GetEnableAttribute>(true, getEnableAttribute, null, "Success", (int)Helpers.Constants.ApiReturnCode.success, count);
                    }

                }
                catch (Exception err)
                {
                    return new Response<GetEnableAttribute>(false, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
                }
            }
        }
        public Response<GetEnableAttribute> GetCabinetPowerWithEnableAtt(string? SiteCode, string ConnectionString)
        {
            using (var connection = new OracleConnection(ConnectionString))
            {
                try
                {
                    GetEnableAttribute getEnableAttribute = new GetEnableAttribute();
                    connection.Open();
                    //string storedProcedureName = "create_dynamic_pivot_withleg ";
                    //using (OracleCommand procedureCommand = new OracleCommand(storedProcedureName, connection))
                    //{
                    //    procedureCommand.CommandType = CommandType.StoredProcedure;
                    //    procedureCommand.ExecuteNonQuery();
                    //}
                    var attActivated = _dbContext.TLIattributeViewManagment
                        .Include(x => x.EditableManagmentView)
                        .Include(x => x.AttributeActivated)
                        .Include(x => x.DynamicAtt)
                        .Where(x => x.Enable && x.EditableManagmentView.View == "CabinetPowerInstallation" &&
                        ((x.AttributeActivatedId != null && x.AttributeActivated.enable) || (x.DynamicAttId != null && !x.DynamicAtt.disable)))
                        .Select(x => new { attribute = x.AttributeActivated.Key, dynamic = x.DynamicAtt.Key, dataType = x.DynamicAtt != null ? x.DynamicAtt.DataType.Name.ToString() : x.AttributeActivated.DataType.ToString() })
                        .OrderByDescending(x => x.attribute.ToLower().StartsWith("name"))
                            .ThenBy(x => x.attribute == null)
                            .ThenBy(x => x.attribute)
                            .ToList();
                    getEnableAttribute.Type = attActivated;
                    List<string> propertyNamesStatic = new List<string>();
                    Dictionary<string, string> propertyNamesDynamic = new Dictionary<string, string>();
                    foreach (var key in attActivated)
                    {
                        if (key.attribute != null)
                        {
                            string name = key.attribute;
                            if (name != "Id" && name.EndsWith("Id") )
                            {
                                string fk = name.Remove(name.Length - 2);
                                propertyNamesStatic.Add(fk);
                            }
                            else 
                            {
                                propertyNamesStatic.Add(name);
                            }
                           

                        }
                        else
                        {
                            string name = key.dynamic;
                            string datatype = key.dataType;
                            propertyNamesDynamic.Add(name, datatype);
                        }

                    }
                    propertyNamesStatic.Add("SITECODE");
                    propertyNamesStatic.Remove("CabinetTelecomLibrary");
                  
                    if (SiteCode == null)
                    {
                        if (propertyNamesDynamic.Count == 0)
                        {
                            var query = _dbContext.MV_CABINET_POWER_VIEW.Where(x =>
                            !x.Dismantle).AsEnumerable()
                            .Select(item => _unitOfWork.CivilWithLegsRepository.BuildDynamicSelect(item, null, propertyNamesStatic, propertyNamesDynamic));
                            int count = query.Count();

                            getEnableAttribute.Model = query;
                            return new Response<GetEnableAttribute>(true, getEnableAttribute, null, "Success", (int)Helpers.Constants.ApiReturnCode.success, count);
                        }
                        else
                        {
                            var query = _dbContext.MV_CABINET_POWER_VIEW.Where(x =>
                              !x.Dismantle).AsEnumerable()
                        .GroupBy(x => new
                        {

                            Id = x.Id,
                            Name = x.Name,
                            SITECODE = x.SITECODE,
                            TPVersion = x.TPVersion,
                            RenewableCabinetNumberOfBatteries = x.RenewableCabinetNumberOfBatteries,
                            NUmberOfPSU = x.NUmberOfPSU,
                            SpaceInstallation = x.SpaceInstallation,
                            VisibleStatus = x.VisibleStatus,
                            CABINETPOWERLIBRARY = x.CABINETPOWERLIBRARY,
                            RENEWABLECABINETTYPE = x.RENEWABLECABINETTYPE,
                            Dismantle = x.Dismantle,
                        }).OrderBy(x => x.Key.Name)
                        .Select(x => new { key = x.Key, value = x.ToDictionary(z => z.Key, z => z.INPUTVALUE) })
                        .Select(item => _unitOfWork.CivilWithLegsRepository.BuildDynamicSelect(item.key, item.value, propertyNamesStatic, propertyNamesDynamic));
                            int count = query.Count();
                            getEnableAttribute.Model = query;
                            return new Response<GetEnableAttribute>(true, getEnableAttribute, null, "Success", (int)Helpers.Constants.ApiReturnCode.success, count);
                        }
                    }
                    if (propertyNamesDynamic.Count == 0)
                    {
                        var query = _dbContext.MV_CABINET_POWER_VIEW.Where(x => x.SITECODE.ToLower() == SiteCode.ToLower()
                        && !x.Dismantle).AsEnumerable()
                    .Select(item => _unitOfWork.CivilWithLegsRepository.BuildDynamicSelect(item, null, propertyNamesStatic, propertyNamesDynamic));
                        int count = query.Count();

                        getEnableAttribute.Model = query;
                        return new Response<GetEnableAttribute>(true, getEnableAttribute, null, "Success", (int)Helpers.Constants.ApiReturnCode.success, count);
                    }
                    else
                    {
                        var query = _dbContext.MV_CABINET_POWER_VIEW.Where(x => x.SITECODE.ToLower() == SiteCode.ToLower()
                         && !x.Dismantle).AsEnumerable()
                    .GroupBy(x => new
                    {

                        Id = x.Id,
                        Name = x.Name,
                        SITECODE = x.SITECODE,
                        TPVersion = x.TPVersion,
                        RenewableCabinetNumberOfBatteries = x.RenewableCabinetNumberOfBatteries,
                        NUmberOfPSU = x.NUmberOfPSU,
                        SpaceInstallation = x.SpaceInstallation,
                        VisibleStatus = x.VisibleStatus,
                        CABINETPOWERLIBRARY = x.CABINETPOWERLIBRARY,
                        RENEWABLECABINETTYPE = x.RENEWABLECABINETTYPE,
                        Dismantle = x.Dismantle,
                    }).OrderBy(x => x.Key.Name)
                    .Select(x => new { key = x.Key, value = x.ToDictionary(z => z.Key, z => z.INPUTVALUE) })
                    .Select(item => _unitOfWork.CivilWithLegsRepository.BuildDynamicSelect(item.key, item.value, propertyNamesStatic, propertyNamesDynamic));
                        int count = query.Count();
                        getEnableAttribute.Model = query;
                        return new Response<GetEnableAttribute>(true, getEnableAttribute, null, "Success", (int)Helpers.Constants.ApiReturnCode.success, count);
                    }

                }
                catch (Exception err)
                {
                    return new Response<GetEnableAttribute>(false, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
                }
            }
        }
        //public Response<GetEnableAttribute> GetSolarWithEnableAtt(string? SiteCode, string ConnectionString)
        //{
        //    using (var connection = new OracleConnection(ConnectionString))
        //    {
        //        try
        //        {
        //            GetEnableAttribute getEnableAttribute = new GetEnableAttribute();
        //            connection.Open();
        //            //string storedProcedureName = "create_dynamic_pivot_withleg ";
        //            //using (OracleCommand procedureCommand = new OracleCommand(storedProcedureName, connection))
        //            //{
        //            //    procedureCommand.CommandType = CommandType.StoredProcedure;
        //            //    procedureCommand.ExecuteNonQuery();
        //            //}
        //            var attActivated = _dbContext.TLIattributeViewManagment
        //                .Include(x => x.EditableManagmentView)
        //                .Include(x => x.AttributeActivated)
        //                .Include(x => x.DynamicAtt)
        //                .Where(x => x.Enable && x.EditableManagmentView.View == "SolarInstallation" &&
        //                ((x.AttributeActivatedId != null && x.AttributeActivated.enable) || (x.DynamicAttId != null && !x.DynamicAtt.disable)))
        //                .Select(x => new { attribute = x.AttributeActivated.Key, dynamic = x.DynamicAtt.Key, dataType = x.DynamicAtt != null ? x.DynamicAtt.DataType.Name.ToString() : x.AttributeActivated.DataType.ToString() })
        //                .OrderByDescending(x => x.attribute.ToLower().StartsWith("name"))
        //                    .ThenBy(x => x.attribute == null)
        //                    .ThenBy(x => x.attribute)
        //                    .ToList();
        //            getEnableAttribute.Type = attActivated;
        //            List<string> propertyNamesStatic = new List<string>();
        //            Dictionary<string, string> propertyNamesDynamic = new Dictionary<string, string>();
        //            foreach (var key in attActivated)
        //            {
        //                if (key.attribute != null)
        //                {
        //                    string name = key.attribute;
        //                    if (name != "Id" && name.EndsWith("Id"))
        //                    {
        //                        string fk = name.Remove(name.Length - 2);
        //                        propertyNamesStatic.Add(fk);
        //                    }
        //                    else
        //                    {
        //                        propertyNamesStatic.Add(name);
        //                    }


        //                }
        //                else
        //                {
        //                    string name = key.dynamic;
        //                    string datatype = key.dataType;
        //                    propertyNamesDynamic.Add(name, datatype);
        //                }

        //            }
        //            propertyNamesStatic.Add("SITECODE");

        //            if (SiteCode == null)
        //            {
        //                if (propertyNamesDynamic.Count == 0)
        //                {
        //                    var query = _dbContext.MV_SOLAR_VIEW.Where(x =>
        //                    !x.Dismantle).AsEnumerable()
        //                    .Select(item => _unitOfWork.CivilWithLegsRepository.BuildDynamicSelect(item, null, propertyNamesStatic, propertyNamesDynamic));
        //                    int count = query.Count();

        //                    getEnableAttribute.Model = query;
        //                    return new Response<GetEnableAttribute>(true, getEnableAttribute, null, "Success", (int)Helpers.Constants.ApiReturnCode.success, count);
        //                }
        //                else
        //                {
        //                    var query = _dbContext.MV_SOLAR_VIEW.Where(x =>
        //                      !x.Dismantle).AsEnumerable()
        //                .GroupBy(x => new
        //                {

        //                    Id = x.Id,
        //                    Name = x.Name,
        //                    SITECODE = x.SITECODE,
        //                    PVPanelBrandAndWattage = x.PVPanelBrandAndWattage,
        //                    PVArrayAzimuth = x.PVArrayAzimuth,
        //                    PVArrayAngel = x.PVArrayAngel,
        //                    SpaceInstallation = x.SpaceInstallation,
        //                    VisibleStatus = x.VisibleStatus,
        //                    Prefix = x.Prefix,
        //                    PowerLossRatio = x.PowerLossRatio,
        //                    NumberOfSSU = x.NumberOfSSU,
        //                    NumberOfLightingRod = x.NumberOfLightingRod,
        //                    NumberOfInstallPVs = x.NumberOfInstallPVs,
        //                    LocationDescription = x.LocationDescription,
        //                    ExtenstionDimension = x.ExtenstionDimension,
        //                    Extension = x.Extension,
        //                    SOLARLIBRARY = x.SOLARLIBRARY,
        //                    CABINET = x.CABINET,
        //                    Dismantle = x.Dismantle,

        //                }).OrderBy(x => x.Key.Name)
        //                .Select(x => new { key = x.Key, value = x.ToDictionary(z => z.Key, z => z.INPUTVALUE) })
        //                .Select(item => _unitOfWork.CivilWithLegsRepository.BuildDynamicSelect(item.key, item.value, propertyNamesStatic, propertyNamesDynamic));
        //                    int count = query.Count();
        //                    getEnableAttribute.Model = query;
        //                    return new Response<GetEnableAttribute>(true, getEnableAttribute, null, "Success", (int)Helpers.Constants.ApiReturnCode.success, count);
        //                }
        //            }
        //            if (propertyNamesDynamic.Count == 0)
        //            {
        //                var query = _dbContext.MV_SOLAR_VIEW.Where(x => x.SITECODE.ToLower() == SiteCode.ToLower()
        //                && !x.Dismantle).AsEnumerable()
        //            .Select(item => _unitOfWork.CivilWithLegsRepository.BuildDynamicSelect(item, null, propertyNamesStatic, propertyNamesDynamic));
        //                int count = query.Count();

        //                getEnableAttribute.Model = query;
        //                return new Response<GetEnableAttribute>(true, getEnableAttribute, null, "Success", (int)Helpers.Constants.ApiReturnCode.success, count);
        //            }
        //            else
        //            {
        //                var query = _dbContext.MV_SOLAR_VIEW.Where(x => x.SITECODE.ToLower() == SiteCode.ToLower()
        //                 && !x.Dismantle).AsEnumerable()
        //            .GroupBy(x => new
        //            {
        //                Id = x.Id,
        //                Name = x.Name,
        //                SITECODE = x.SITECODE,
        //                PVPanelBrandAndWattage = x.PVPanelBrandAndWattage,
        //                PVArrayAzimuth = x.PVArrayAzimuth,
        //                PVArrayAngel = x.PVArrayAngel,
        //                SpaceInstallation = x.SpaceInstallation,
        //                VisibleStatus = x.VisibleStatus,
        //                Prefix = x.Prefix,
        //                PowerLossRatio = x.PowerLossRatio,
        //                NumberOfSSU = x.NumberOfSSU,
        //                NumberOfLightingRod = x.NumberOfLightingRod,
        //                NumberOfInstallPVs = x.NumberOfInstallPVs,
        //                LocationDescription = x.LocationDescription,
        //                ExtenstionDimension = x.ExtenstionDimension,
        //                Extension = x.Extension,
        //                SOLARLIBRARY = x.SOLARLIBRARY,
        //                CABINET = x.CABINET,
        //                Dismantle = x.Dismantle,

        //            }).OrderBy(x => x.Key.Name)
        //            .Select(x => new { key = x.Key, value = x.ToDictionary(z => z.Key, z => z.INPUTVALUE) })
        //            .Select(item => _unitOfWork.CivilWithLegsRepository.BuildDynamicSelect(item.key, item.value, propertyNamesStatic, propertyNamesDynamic));
        //                int count = query.Count();
        //                getEnableAttribute.Model = query;
        //                return new Response<GetEnableAttribute>(true, getEnableAttribute, null, "Success", (int)Helpers.Constants.ApiReturnCode.success, count);
        //            }

        //        }
        //        catch (Exception err)
        //        {
        //            return new Response<GetEnableAttribute>(false, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
        //        }
        //    }
        //}

        public Response<GetEnableAttribute> GetSolarWithEnableAtt(string? SiteCode, string ConnectionString)
        {
            using (var connection = new OracleConnection(ConnectionString))
            {
                try
                {
                    GetEnableAttribute getEnableAttribute = new GetEnableAttribute();
                    connection.Open();

                    var attActivated = _dbContext.TLIattributeViewManagment
                        .Include(x => x.EditableManagmentView)
                        .Include(x => x.AttributeActivated)
                        .Include(x => x.DynamicAtt)
                        .Where(x => x.Enable && x.EditableManagmentView.View == "SolarInstallation"
                            && ((x.AttributeActivatedId != null && x.AttributeActivated.enable) || (x.DynamicAttId != null && !x.DynamicAtt.disable)))
                        .Select(x => new
                        {
                            attribute = x.AttributeActivated.Key,
                            dynamic = x.DynamicAtt.Key,
                            dataType = x.DynamicAtt != null ? x.DynamicAtt.DataType.Name.ToString() : x.AttributeActivated.DataType.ToString()
                        })
                        .OrderByDescending(x => x.attribute.ToLower().StartsWith("name"))
                        .ThenBy(x => x.attribute == null)
                        .ThenBy(x => x.attribute)
                        .ToList();

                    getEnableAttribute.Type = attActivated;

                    List<string> propertyNamesStatic = new List<string>();
                    Dictionary<string, string> propertyNamesDynamic = new Dictionary<string, string>();

                    foreach (var key in attActivated)
                    {
                        if (key.attribute != null)
                        {
                            string name = key.attribute;
                            if (name != "Id" && name.EndsWith("Id"))
                            {
                                string fk = name.Remove(name.Length - 2);
                                propertyNamesStatic.Add(fk);
                            }
                            else
                            {
                                propertyNamesStatic.Add(name);
                            }
                        }
                        else
                        {
                            string name = key.dynamic;
                            string datatype = key.dataType;
                            propertyNamesDynamic.Add(name, datatype);
                        }
                    }
                    if (SiteCode == null)
                    {
                        if (propertyNamesDynamic.Count == 0)
                        {
                            // استخدام LINQ بدلاً من ADO.NET هنا
                            var query = _dbContext.MV_SOLAR_VIEW.Where(x => !x.Dismantle).AsEnumerable()
                          .Select(item => _unitOfWork.CivilWithLegsRepository.BuildDynamicSelect(item, null, propertyNamesStatic, propertyNamesDynamic));
                            int count = query.Count();


                            getEnableAttribute.Model = query;

                            return new Response<GetEnableAttribute>(true, getEnableAttribute, null, "Success", (int)Helpers.Constants.ApiReturnCode.success, count);
                        }
                        else
                        {
                            // 2. إنشاء استعلام SELECT من الفيو مع تصفية على الـ Deleted باستخدام OracleCommand
                            string xx = "SELECT * FROM MV_SOLAR_VIEW WHERE \"Dismantle\" = 0";

                            using (OracleCommand command = new OracleCommand(xx, connection))
                            {
                                using (OracleDataReader reader = command.ExecuteReader())
                                {
                                    var query = new List<dynamic>(); // تخزين البيانات هنا باستخدام dynamic



                                    while (reader.Read())
                                    {
                                        var item = new
                                        {
                                            Id = reader["Id"],
                                            Name = reader["Name"],
                                            SITECODE = reader["SITECODE"],
                                            PVPanelBrandAndWattage = reader["PVPanelBrandAndWattage"] == DBNull.Value ? null : reader["PVPanelBrandAndWattage"].ToString(),
                                            PVArrayAzimuth = reader["PVArrayAzimuth"] == DBNull.Value ? null : reader["PVArrayAzimuth"].ToString(),
                                            PVArrayAngel = reader["PVArrayAngel"] == DBNull.Value ? null : reader["PVArrayAngel"].ToString(),
                                            SpaceInstallation = reader["SpaceInstallation"],
                                            VisibleStatus = reader["VisibleStatus"] == DBNull.Value ? null : reader["VisibleStatus"].ToString(),
                                            Prefix = reader["Prefix"] == DBNull.Value ? null : reader["Prefix"].ToString(),
                                            PowerLossRatio = reader["PowerLossRatio"] == DBNull.Value ? null : reader["PowerLossRatio"].ToString(),
                                            NumberOfSSU = reader["NumberOfSSU"],
                                            NumberOfLightingRod = reader["NumberOfLightingRod"],
                                            NumberOfInstallPVs = reader["NumberOfInstallPVs"],
                                            LocationDescription = reader["LocationDescription"] == DBNull.Value ? null : reader["LocationDescription"].ToString(),
                                            ExtenstionDimension = reader["ExtenstionDimension"],
                                            Extension = reader["Extension"] == DBNull.Value ? null : reader["Extension"].ToString(),
                                            SOLARLIBRARY = reader["SOLARLIBRARY"],
                                            CABINET = reader["CABINET"] == DBNull.Value ? null : reader["CABINET"].ToString(),
                                            Dismantle = reader["Dismantle"],
                                            Key = reader["Key"],
                                            INPUTVALUE = reader["INPUTVALUE"],
                                        };

                                        query.Add(item);
                                    }

                                    var dynamicQuery = query
               // تجميع بناءً على الـ Id فقط
               .GroupBy(x => new
               {
                   x.Id,
                   x.Name,
                   x.SITECODE,
                   x.PVPanelBrandAndWattage,
                   x.PVArrayAzimuth,
                   x.PVArrayAngel,
                   x.SpaceInstallation,
                   x.VisibleStatus,
                   x.Prefix,
                   x.PowerLossRatio,
                   x.NumberOfSSU,
                   x.NumberOfLightingRod,
                   x.NumberOfInstallPVs,
                   x.LocationDescription,
                   x.ExtenstionDimension,
                   x.Extension,
                   x.SOLARLIBRARY,
                   x.CABINET,
                   x.Dismantle
               })
               .Select(g => new
               {
                   key = new
                   {

                       Id = g.Key.Id,
                       Name = g.Key.Name,
                       SITECODE = g.Key.SITECODE,
                       PVPanelBrandAndWattage = g.Key.PVPanelBrandAndWattage,
                       PVArrayAzimuth = g.Key.PVArrayAngel,
                       PVArrayAngel = g.Key.PVArrayAngel,
                       SpaceInstallation = g.Key.SpaceInstallation,
                       VisibleStatus = g.Key.VisibleStatus,
                       Prefix = g.Key.Prefix,
                       PowerLossRatio = g.Key.PowerLossRatio,
                       NumberOfSSU = g.Key.NumberOfSSU,
                       NumberOfLightingRod = g.Key.NumberOfLightingRod,
                       NumberOfInstallPVs = g.Key.NumberOfInstallPVs,
                       LocationDescription = g.Key.LocationDescription,
                       ExtenstionDimension = g.Key.ExtenstionDimension,
                       Extension = g.Key.Extension,
                       SOLARLIBRARY = g.Key.SOLARLIBRARY,
                       CABINET = g.Key.CABINET,
                       Dismantl = g.Key.Dismantle,


                   },
                   // دمج القيم المتغيرة مثل INPUTVALUE
                   value = g
                        .Select(x => new { x.Key, x.INPUTVALUE })
                        .GroupBy(x => x.Key)
                        .ToDictionary(
                            grp => string.Join(",", grp.Select(x => x.Key?.ToString() ?? "")),
                            grp => string.Join(",", grp.Select(x => x.INPUTVALUE?.ToString() ?? ""))
                        )
               })
                        .Select(item => _unitOfWork.CivilWithLegsRepository.BuildDynamicSelect(item.key, item.value, propertyNamesStatic, propertyNamesDynamic))
                        .ToList();
                                    int count = dynamicQuery.Count();

                                    getEnableAttribute.Model = dynamicQuery;
                                    return new Response<GetEnableAttribute>(true, getEnableAttribute, null, "Success", (int)Helpers.Constants.ApiReturnCode.success, count);
                                }
                            }
                        }
                    }
                    if (propertyNamesDynamic.Count == 0)
                    {
                        // استخدام LINQ بدلاً من ADO.NET هنا
                        var query = _dbContext.MV_SOLAR_VIEW.Where(x => x.SITECODE.ToLower() == SiteCode.ToLower()
                       && !x.Dismantle).AsEnumerable()
                      .Select(item => _unitOfWork.CivilWithLegsRepository.BuildDynamicSelect(item, null, propertyNamesStatic, propertyNamesDynamic));
                        int count = query.Count();


                        getEnableAttribute.Model = query;

                        return new Response<GetEnableAttribute>(true, getEnableAttribute, null, "Success", (int)Helpers.Constants.ApiReturnCode.success, count);
                    }
                    else
                    {
                        // 2. إنشاء استعلام SELECT من الفيو مع تصفية على الـ Deleted باستخدام OracleCommand
                        string xx = "SELECT * FROM MV_SOLAR_VIEW WHERE \"Dismantle\" = 0 AND \"SITECODE\" = :siteCode";
                        using (OracleCommand command = new OracleCommand(xx, connection))
                        {
                            // إضافة المعامل قبل تنفيذ الاستعلام
                            command.Parameters.Add(new OracleParameter("siteCode", SiteCode));

                            using (OracleDataReader reader = command.ExecuteReader()) // الآن سيعمل بشكل صحيح
                            {
                                var query = new List<dynamic>();

                                while (reader.Read())
                                {
                                    var item = new
                                    {
                                        Id = reader["Id"],
                                        Name = reader["Name"],
                                        SITECODE = reader["SITECODE"],
                                        PVPanelBrandAndWattage = reader["PVPanelBrandAndWattage"] == DBNull.Value ? null : reader["PVPanelBrandAndWattage"].ToString(),
                                        PVArrayAzimuth = reader["PVArrayAzimuth"] == DBNull.Value ? null : reader["PVArrayAzimuth"].ToString(),
                                        PVArrayAngel = reader["PVArrayAngel"] == DBNull.Value ? null : reader["PVArrayAngel"].ToString(),
                                        SpaceInstallation = reader["SpaceInstallation"],
                                        VisibleStatus = reader["VisibleStatus"] == DBNull.Value ? null : reader["VisibleStatus"].ToString(),
                                        Prefix = reader["Prefix"] == DBNull.Value ? null : reader["Prefix"].ToString(),
                                        PowerLossRatio = reader["PowerLossRatio"] == DBNull.Value ? null : reader["PowerLossRatio"].ToString(),
                                        NumberOfSSU = reader["NumberOfSSU"],
                                        NumberOfLightingRod = reader["NumberOfLightingRod"],
                                        NumberOfInstallPVs = reader["NumberOfInstallPVs"],
                                        LocationDescription = reader["LocationDescription"] == DBNull.Value ? null : reader["LocationDescription"].ToString(),
                                        ExtenstionDimension = reader["ExtenstionDimension"],
                                        Extension = reader["Extension"] == DBNull.Value ? null : reader["Extension"].ToString(),
                                        SOLARLIBRARY = reader["SOLARLIBRARY"],
                                        CABINET = reader["CABINET"] == DBNull.Value ? null : reader["CABINET"].ToString(),
                                        Dismantle = reader["Dismantle"],
                                        Key = reader["Key"],
                                        INPUTVALUE = reader["INPUTVALUE"],
                                    };

                                    query.Add(item);
                                }

                                var dynamicQuery = query
           // تجميع بناءً على الـ Id فقط
           .GroupBy(x => new
           {
               x.Id,
               x.Name,
               x.SITECODE,
               x.PVPanelBrandAndWattage,
               x.PVArrayAzimuth,
               x.PVArrayAngel,
               x.SpaceInstallation,
               x.VisibleStatus,
               x.Prefix,
               x.PowerLossRatio,
               x.NumberOfSSU,
               x.NumberOfLightingRod,
               x.NumberOfInstallPVs,
               x.LocationDescription,
               x.ExtenstionDimension,
               x.Extension,
               x.SOLARLIBRARY,
               x.CABINET,
               x.Dismantle,
               
           })
           .Select(g => new
           {
               key = new
               {

                   Id = g.Key.Id,
                   Name = g.Key.Name,
                   SITECODE = g.Key.SITECODE,
                   PVPanelBrandAndWattage = g.Key.PVPanelBrandAndWattage,
                   PVArrayAzimuth = g.Key.PVArrayAngel,
                   PVArrayAngel = g.Key.PVArrayAngel,
                   SpaceInstallation = g.Key.SpaceInstallation,
                   VisibleStatus = g.Key.VisibleStatus,
                   Prefix = g.Key.Prefix,
                   PowerLossRatio = g.Key.PowerLossRatio,
                   NumberOfSSU = g.Key.NumberOfSSU,
                   NumberOfLightingRod = g.Key.NumberOfLightingRod,
                   NumberOfInstallPVs = g.Key.NumberOfInstallPVs,
                   LocationDescription = g.Key.LocationDescription,
                   ExtenstionDimension = g.Key.ExtenstionDimension,
                   Extension = g.Key.Extension,
                   SOLARLIBRARY = g.Key.SOLARLIBRARY,
                   CABINET = g.Key.CABINET,
                   Dismantle = g.Key.Dismantle,


               },
               // دمج القيم المتغيرة مثل INPUTVALUE
               value = g
                    .Select(x => new { x.Key, x.INPUTVALUE })
                    .GroupBy(x => x.Key)
                    .ToDictionary(
                        grp => string.Join(",", grp.Select(x => x.Key?.ToString() ?? "")),
                        grp => string.Join(",", grp.Select(x => x.INPUTVALUE?.ToString() ?? ""))
                    )
           })
                    .Select(item => _unitOfWork.CivilWithLegsRepository.BuildDynamicSelect(item.key, item.value, propertyNamesStatic, propertyNamesDynamic))
                    .ToList();
                                int count = dynamicQuery.Count();

                                getEnableAttribute.Model = dynamicQuery;
                                return new Response<GetEnableAttribute>(true, getEnableAttribute, null, "Success", (int)Helpers.Constants.ApiReturnCode.success, count);
                            }
                        }
                    }
                }
                catch (Exception err)
                {
                    return new Response<GetEnableAttribute>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
                }
            }
        }

        public Response<GetEnableAttribute> GetCabinetTelecomWithEnableAtt(string? SiteCode, string ConnectionString)
        {
            using (var connection = new OracleConnection(ConnectionString))
            {
                try
                {
                    GetEnableAttribute getEnableAttribute = new GetEnableAttribute();
                    connection.Open();
                    //string storedProcedureName = "create_dynamic_pivot_withleg ";
                    //using (OracleCommand procedureCommand = new OracleCommand(storedProcedureName, connection))
                    //{
                    //    procedureCommand.CommandType = CommandType.StoredProcedure;
                    //    procedureCommand.ExecuteNonQuery();
                    //}
                    var attActivated = _dbContext.TLIattributeViewManagment
                        .Include(x => x.EditableManagmentView)
                        .Include(x => x.AttributeActivated)
                        .Include(x => x.DynamicAtt)
                        .Where(x => x.Enable && x.EditableManagmentView.View == "CabinetTelecomInstallation" &&
                        ((x.AttributeActivatedId != null && x.AttributeActivated.enable) || (x.DynamicAttId != null && !x.DynamicAtt.disable)))
                        .Select(x => new { attribute = x.AttributeActivated.Key, dynamic = x.DynamicAtt.Key, dataType = x.DynamicAtt != null ? x.DynamicAtt.DataType.Name.ToString() : x.AttributeActivated.DataType.ToString() })
                        .OrderByDescending(x => x.attribute.ToLower().StartsWith("name"))
                            .ThenBy(x => x.attribute == null)
                            .ThenBy(x => x.attribute)
                            .ToList();
                    getEnableAttribute.Type = attActivated;
                    List<string> propertyNamesStatic = new List<string>();
                    Dictionary<string, string> propertyNamesDynamic = new Dictionary<string, string>();
                    foreach (var key in attActivated)
                    {
                        if (key.attribute != null)
                        {
                            string name = key.attribute;
                            if (name != "Id" && name.EndsWith("Id"))
                            {
                                string fk = name.Remove(name.Length - 2);
                                propertyNamesStatic.Add(fk);
                            }
                            else
                            {
                                propertyNamesStatic.Add(name);
                            }

                        }
                        else
                        {
                            string name = key.dynamic;
                            string datatype = key.dataType;
                            propertyNamesDynamic.Add(name, datatype);
                        }

                    }
                    propertyNamesStatic.Add("SITECODE");
                    propertyNamesStatic.Remove("CabinetPowerLibrary");
                    if (SiteCode == null)
                    {
                        if (propertyNamesDynamic.Count == 0)
                        {
                            var query = _dbContext.MV_CABINET_TELECOM_VIEW.Where(x =>
                            !x.Dismantle).AsEnumerable()
                            .Select(item => _unitOfWork.CivilWithLegsRepository.BuildDynamicSelect(item, null, propertyNamesStatic, propertyNamesDynamic));
                            int count = query.Count();

                            getEnableAttribute.Model = query;
                            return new Response<GetEnableAttribute>(true, getEnableAttribute, null, "Success", (int)Helpers.Constants.ApiReturnCode.success, count);
                        }
                        else
                        {
                            var query = _dbContext.MV_CABINET_TELECOM_VIEW.Where(x =>
                              !x.Dismantle).AsEnumerable()
                        .GroupBy(x => new
                        {

                            Id = x.Id,
                            Name = x.Name,
                            SITECODE = x.SITECODE,
                            TPVersion = x.TPVersion,
                            RenewableCabinetNumberOfBatteries = x.RenewableCabinetNumberOfBatteries,
                            NUmberOfPSU = x.NUmberOfPSU,
                            SpaceInstallation = x.SpaceInstallation,
                            VisibleStatus = x.VisibleStatus,
                            CABINETTELECOMLIBRARY = x.CABINETTELECOMLIBRARY,
                            RENEWABLECABINETTYPE = x.RENEWABLECABINETTYPE,
                            Dismantle = x.Dismantle,
                        }).OrderBy(x => x.Key.Name)
                        .Select(x => new { key = x.Key, value = x.ToDictionary(z => z.Key, z => z.INPUTVALUE) })
                        .Select(item => _unitOfWork.CivilWithLegsRepository.BuildDynamicSelect(item.key, item.value, propertyNamesStatic, propertyNamesDynamic));
                            int count = query.Count();
                            getEnableAttribute.Model = query;
                            return new Response<GetEnableAttribute>(true, getEnableAttribute, null, "Success", (int)Helpers.Constants.ApiReturnCode.success, count);
                        }
                    }
                    if (propertyNamesDynamic.Count == 0)
                    {
                        var query = _dbContext.MV_CABINET_TELECOM_VIEW.Where(x => x.SITECODE.ToLower() == SiteCode.ToLower()
                        && !x.Dismantle).AsEnumerable()
                    .Select(item => _unitOfWork.CivilWithLegsRepository.BuildDynamicSelect(item, null, propertyNamesStatic, propertyNamesDynamic));
                        int count = query.Count();

                        getEnableAttribute.Model = query;
                        return new Response<GetEnableAttribute>(true, getEnableAttribute, null, "Success", (int)Helpers.Constants.ApiReturnCode.success, count);
                    }
                    else
                    {
                        var query = _dbContext.MV_CABINET_TELECOM_VIEW.Where(x => x.SITECODE.ToLower() == SiteCode.ToLower()
                         && !x.Dismantle).AsEnumerable()
                    .GroupBy(x => new
                    {

                        Id = x.Id,
                        Name = x.Name,
                        SITECODE = x.SITECODE,
                        TPVersion = x.TPVersion,
                        RenewableCabinetNumberOfBatteries = x.RenewableCabinetNumberOfBatteries,
                        NUmberOfPSU = x.NUmberOfPSU,
                        SpaceInstallation = x.SpaceInstallation,
                        VisibleStatus = x.VisibleStatus,
                        CABINETTELECOMLIBRARY = x.CABINETTELECOMLIBRARY,
                        RENEWABLECABINETTYPE = x.RENEWABLECABINETTYPE,
                        Dismantle = x.Dismantle,
                    }).OrderBy(x => x.Key.Name)
                    .Select(x => new { key = x.Key, value = x.ToDictionary(z => z.Key, z => z.INPUTVALUE) })
                    .Select(item => _unitOfWork.CivilWithLegsRepository.BuildDynamicSelect(item.key, item.value, propertyNamesStatic, propertyNamesDynamic));
                        int count = query.Count();
                        getEnableAttribute.Model = query;
                        return new Response<GetEnableAttribute>(true, getEnableAttribute, null, "Success", (int)Helpers.Constants.ApiReturnCode.success, count);
                    }

                }
                catch (Exception err)
                {
                    return new Response<GetEnableAttribute>(false, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
                }
            }
        }
        #endregion
        #region UpdateOtherInventoryInstallation
        //Function take 2 parameters model, TableName
        //specify the table i deal with
        //Map model object to ViewModel
        //Map ViewModel to Entity
        //Update Entity
        #endregion
        public async Task<Response<GetForAddOtherInventoryInstallationObject>> EditOtherInventoryInstallation(object model, string TableName, int? TaskId, int UserId, string connectionString, bool ExternalSys)
        {
            using (TransactionScope transaction = new TransactionScope())
            {
                try
                {
                    TLIsite NewSite = new TLIsite();
                    TLItablesNames TableEntity = _unitOfWork.TablesNamesRepository.GetWhereFirst(x => x.TableName.ToLower() == TableName.ToLower());
                    //if (OtherInventoryType.TLIcabinet.ToString().ToLower() == TableName.ToLower())
                    //{
                    //    EditCabinetViewModel CabinetModel = _mapper.Map<EditCabinetViewModel>(model);

                    //    TLIcabinet Cabinet = _mapper.Map<TLIcabinet>(CabinetModel);
                    //    TLIcabinet OldCabinet = _unitOfWork.CabinetRepository.GetAllAsQueryable().AsNoTracking().FirstOrDefault(x => x.Id == CabinetModel.Id);

                    //    TLIotherInSite OtherInSite = _unitOfWork.OtherInSiteRepository.GetIncludeWhereFirst(x => !x.Dismantle &&
                    //        x.allOtherInventoryInst.cabinetId == CabinetModel.Id, x => x.allOtherInventoryInst);

                    //    string SiteCode = "";

                    //    if (OtherInSite != null)
                    //        SiteCode = OtherInSite.SiteCode;

                    //    else
                    //        SiteCode = null;

                    //    TLIotherInSite CheckName = _unitOfWork.OtherInSiteRepository
                    //        .GetIncludeWhereFirst(x => x.allOtherInventoryInst.cabinetId != Cabinet.Id && !x.allOtherInventoryInst.Draft &&
                    //            !x.Dismantle && x.allOtherInventoryInst.cabinet.Name.ToLower() == Cabinet.Name.ToLower() &&
                    //            (Cabinet.CabinetPowerLibraryId != null ? x.allOtherInventoryInst.cabinet.CabinetPowerLibraryId == Cabinet.CabinetPowerLibraryId :
                    //                x.allOtherInventoryInst.cabinet.CabinetTelecomLibraryId == Cabinet.CabinetTelecomLibraryId) &&
                    //            (CabinetModel.CabinetPowerLibraryId != null ? x.allOtherInventoryInst.cabinet.CabinetPowerLibraryId == CabinetModel.CabinetPowerLibraryId :
                    //                CabinetModel.CabinetTelecomLibraryId == x.allOtherInventoryInst.cabinet.CabinetTelecomLibraryId) &&
                    //                x.SiteCode.ToLower() == SiteCode.ToLower(),
                    //                    x => x.allOtherInventoryInst, x => x.allOtherInventoryInst.cabinet);

                    //    if (CheckName != null)
                    //        return new Response<GetForAddOtherInventoryInstallationObject>(true, null, null, $"The name {Cabinet.Name} is already exists", (int)ApiReturnCode.fail);

                    //    if (Cabinet.SpaceInstallation != OldCabinet.SpaceInstallation && CabinetModel.TLIotherInSite.ReservedSpace == true)
                    //    {
                    //        var allother = _dbContext.TLIallOtherInventoryInst.Where(x => x.cabinetId == OldCabinet.Id).Select(x => x.Id).FirstOrDefault();
                    //        var sitescode = _dbContext.TLIotherInSite.Where(x => x.allOtherInventoryInstId == allother).Select(x => x.SiteCode).FirstOrDefault();
                    //        var Site = _dbContext.TLIsite.Where(x => x.SiteCode == sitescode).FirstOrDefault();
                    //        Site.ReservedSpace = Site.ReservedSpace - OldCabinet.SpaceInstallation;
                    //        Site.ReservedSpace = Site.ReservedSpace + Cabinet.SpaceInstallation;
                    //        _dbContext.SaveChanges();
                    //    }
                    //    string CheckGeneralValidation = CheckGeneralValidationFunctionEditVersion(CabinetModel.DynamicInstAttsValue, TableName);

                    //    if (!string.IsNullOrEmpty(CheckGeneralValidation))
                    //        return new Response<GetForAddOtherInventoryInstallationObject>(true, null, null, CheckGeneralValidation, (int)ApiReturnCode.fail);

                    //    string CheckDependencyValidation = CheckDependencyValidationEditVersion(model, SiteCode, TableName);

                    //    if (!string.IsNullOrEmpty(CheckDependencyValidation))
                    //        return new Response<GetForAddOtherInventoryInstallationObject>(true, null, null, CheckDependencyValidation, (int)ApiReturnCode.fail);

                    //    _unitOfWork.CabinetRepository.UpdateWithHistory(Helpers.LogFilterAttribute.UserId, OldCabinet, Cabinet);
                    //    var otherinsite = _unitOfWork.OtherInSiteRepository.GetIncludeWhereFirst(x => x.allOtherInventoryInst.cabinetId == CabinetModel.Id);
                    //    otherinsite.OtherInSiteStatus = CabinetModel.TLIotherInSite.OtherInSiteStatus;
                    //    otherinsite.InstallationDate = CabinetModel.TLIotherInSite.InstallationDate;
                    //    otherinsite.ReservedSpace = CabinetModel.TLIotherInSite.ReservedSpace;
                    //    otherinsite.Dismantle = CabinetModel.TLIotherInSite.Dismantle;
                    //    otherinsite.OtherInventoryStatus = CabinetModel.TLIotherInSite.OtherInventoryStatus;
                    //    _unitOfWork.SaveChanges();
                    //    var allotherinventoryinsId = _unitOfWork.AllOtherInventoryInstRepository.GetWhereFirst(x => x.cabinetId == CabinetModel.Id).Id;
                    //    var otherinventorydistance = _unitOfWork.OtherInventoryDistanceRepository.GetWhereFirst(x => x.allOtherInventoryInstId == allotherinventoryinsId);
                    //    otherinventorydistance.Azimuth = CabinetModel.TLIotherInventoryDistance != null ? CabinetModel.TLIotherInventoryDistance.Azimuth.Value : 0;
                    //    otherinventorydistance.Distance = CabinetModel.TLIotherInventoryDistance != null ? CabinetModel.TLIotherInventoryDistance.Distance.Value : 0;
                    //    otherinventorydistance.ReferenceOtherInventoryId = CabinetModel.TLIotherInventoryDistance != null ? CabinetModel.TLIotherInventoryDistance.ReferenceOtherInventoryId.Value : 0;
                    //    _unitOfWork.SaveChanges();
                    //    if (CabinetModel.DynamicInstAttsValue != null ? CabinetModel.DynamicInstAttsValue.Count > 0 : false)
                    //        _unitOfWork.DynamicAttInstValueRepository.UpdateDynamicValue(CabinetModel.DynamicInstAttsValue, TableEntity.Id, Cabinet.Id);

                    //    await _unitOfWork.SaveChangesAsync();

                    //}
                    if (OtherInventoryType.TLIgenerator.ToString().ToLower() == TableName.ToLower())
                    {
                        EditGeneratorInstallationObject GeneratorModel = _mapper.Map<EditGeneratorInstallationObject>(model);
                        TLIgenerator Generator = _mapper.Map<TLIgenerator>(GeneratorModel.installationAttributes);

                        TLIotherInSite GeneratorInst =
                           _unitOfWork.OtherInSiteRepository
                           .GetAllAsQueryable().AsNoTracking().
                           Include(x => x.allOtherInventoryInst)
                           .ThenInclude(x => x.generator).
                           Include(x=>x.Site).
                           FirstOrDefault(x => x.allOtherInventoryInst
                           .generatorId == Generator.Id
                           && !x.Dismantle);
                        if (GeneratorInst != null)
                        {

                            var CheckName = _dbContext.MV_GENERATOR_VIEW.FirstOrDefault(x =>
                            !x.Dismantle &&
                             x.Id != Generator.Id &&
                             x.Name.ToLower() == Generator.Name.ToLower() &&
                             x.SITECODE.ToLower() == GeneratorInst.SiteCode.ToLower());

                            if (CheckName != null)
                                return new Response<GetForAddOtherInventoryInstallationObject>(true, null, null, $"The name {Generator.Name} is already exists", (int)ApiReturnCode.fail);

                            var CheckSerialNumber = _dbContext.TLIotherInSite.Include(x => x.allOtherInventoryInst).ThenInclude(x => x.generator)
                               .FirstOrDefault(x => x.allOtherInventoryInst.generator.SerialNumber.ToLower()
                               == Generator.SerialNumber.ToLower() && x.allOtherInventoryInst.generatorId != Generator.Id && !x.Dismantle);

                            if (CheckSerialNumber != null)
                                return new Response<GetForAddOtherInventoryInstallationObject>(true, null, null, $"The serial number {Generator.SerialNumber} is already exists", (int)ApiReturnCode.fail);

                            if (GeneratorInst.ReservedSpace == true && GeneratorModel.OtherInSite.ReservedSpace == true)
                            {
                                if (Generator.SpaceInstallation != GeneratorInst.allOtherInventoryInst.generator.SpaceInstallation)
                                {

                                     NewSite = _dbContext.TLIsite.FirstOrDefault(x => x.SiteCode.ToLower() ==
                                            GeneratorInst.SiteCode.ToLower());
                                    NewSite.ReservedSpace = NewSite.ReservedSpace - GeneratorInst.allOtherInventoryInst.generator.SpaceInstallation;
                                    NewSite.ReservedSpace = NewSite.ReservedSpace + Generator.SpaceInstallation;
                                    _unitOfWork.SiteRepository.UpdateSiteWithHistory(UserId, GeneratorInst.Site, NewSite);
                                    _dbContext.SaveChanges();
                                }
                            }
                            else if (GeneratorInst.ReservedSpace == true && GeneratorModel.OtherInSite.ReservedSpace == false)
                            {

                                  NewSite = _dbContext.TLIsite.FirstOrDefault(x => x.SiteCode.ToLower() ==
                                        GeneratorInst.SiteCode.ToLower());
                                NewSite.ReservedSpace = NewSite.ReservedSpace - GeneratorInst.allOtherInventoryInst.generator.SpaceInstallation;
                                _unitOfWork.SiteRepository.UpdateSiteWithHistory(UserId, GeneratorInst.Site, NewSite);
                                _dbContext.SaveChanges();
                            }
                            else if (GeneratorInst.ReservedSpace == false && GeneratorModel.OtherInSite.ReservedSpace == true)
                            {
                                var CheckSpace = _unitOfWork.SiteRepository.CheckSpaces(UserId, GeneratorInst.SiteCode, TableEntity.TableName, GeneratorModel.GeneratorType.GeneratorLibraryId, Generator.SpaceInstallation, null).Message;
                                if (CheckSpace != "Success")
                                {
                                    return new Response<GetForAddOtherInventoryInstallationObject>(true, null, null, CheckSpace, (int)Helpers.Constants.ApiReturnCode.fail);
                                }

                            }
                            //string CheckGeneralValidation = CheckGeneralValidationFunctionEditVersion(GeneratorModel.DynamicInstAttsValue, TableName);

                            //if (!string.IsNullOrEmpty(CheckGeneralValidation))
                            //    return new Response<GetForAddOtherInventoryInstallationObject>(true, null, null, CheckGeneralValidation, (int)ApiReturnCode.fail);

                            //string CheckDependencyValidation = CheckDependencyValidationEditVersion(model, SiteCode, TableName);

                            //if (!string.IsNullOrEmpty(CheckDependencyValidation))
                            //    return new Response<GetForAddOtherInventoryInstallationObject>(true, null, null, CheckDependencyValidation, (int)ApiReturnCode.fail);
                            Generator.GeneratorLibraryId = GeneratorModel.GeneratorType.GeneratorLibraryId;
                            var HistoryId=_unitOfWork.GeneratorRepository.UpdateWithHInstallation(UserId,null, GeneratorInst.allOtherInventoryInst.generator, Generator, GeneratorInst.SiteCode,ExternalSys);
                            _unitOfWork.SaveChanges();
                            //----------------------------------------------------------------------------------//
                            //----------------------OtherOnSite-------------------------------------------------//
                            var NewOtherinsite = _dbContext.TLIotherInSite
                              .Include(x => x.allOtherInventoryInst.otherInventoryDistances).
                              FirstOrDefault(x => x.allOtherInventoryInst.generatorId == Generator.Id);

                            NewOtherinsite.OtherInSiteStatus = GeneratorModel.OtherInSite.OtherInSiteStatus;
                            NewOtherinsite.InstallationDate = GeneratorModel.OtherInSite.InstallationDate;
                            NewOtherinsite.ReservedSpace = GeneratorModel.OtherInSite.ReservedSpace;
                            NewOtherinsite.Dismantle = GeneratorModel.OtherInSite.Dismantle;
                            NewOtherinsite.OtherInventoryStatus = GeneratorModel.OtherInSite.OtherInventoryStatus;
                            var TabelTLIotherInSite = _unitOfWork.TablesNamesRepository.GetWhereFirst(x => x.TableName == "TLIotherInSite").Id;
                            _unitOfWork.OtherInSiteRepository.UpdateWithHLogic(UserId,HistoryId, TabelTLIotherInSite, GeneratorInst, NewOtherinsite);
                            _unitOfWork.SaveChanges();

                            //----------------------------------------------------------------------------------//
                            //----------------------OtherInventoryDistance-------------------------------------------------//

                            var Otherinventorydistance = _unitOfWork.OtherInventoryDistanceRepository.GetIncludeWhereFirst
                                (x => x.allOtherInventoryInst.generatorId == Generator.Id, x => x.allOtherInventoryInst);

                            var OldOtherinventorydistance = _unitOfWork.OtherInventoryDistanceRepository.GetAllAsQueryable().AsNoTracking()
                              .Include(x => x.allOtherInventoryInst).FirstOrDefault(x => x.allOtherInventoryInst.generatorId
                              == Generator.Id);
                            if (OldOtherinventorydistance != null)
                            {
                                if (OldOtherinventorydistance.ReferenceOtherInventoryId != GeneratorModel.OtherInventoryDistance.ReferenceOtherInventoryId)
                                {
                                    var OtherInventoryDistance = _unitOfWork.OtherInventoryDistanceRepository.GetWhere(x => x.ReferenceOtherInventoryId
                                    == GeneratorModel.OtherInventoryDistance.ReferenceOtherInventoryId && x.Distance == GeneratorModel.OtherInventoryDistance.Distance
                                    && x.Azimuth == GeneratorModel.OtherInventoryDistance.Azimuth).Select(x => x.ReferenceOtherInventoryId).ToList();
                                    var OtherInSite = _unitOfWork.OtherInSiteRepository.GetWhere(x => OtherInventoryDistance.Any(y => y
                                    == x.allOtherInventoryInstId) && !x.Dismantle && x.SiteCode.ToLower() == GeneratorInst.SiteCode.ToLower()).ToList();
                                    if (OtherInSite.Count > 0)
                                    {
                                        return new Response<GetForAddOtherInventoryInstallationObject>(false, null, null, "can not select the otherinventory reference on same distance and azimuth because found other otherinventory reference in same place", (int)Helpers.Constants.ApiReturnCode.fail);
                                    }
                                }
                                Otherinventorydistance.Azimuth = GeneratorModel.OtherInventoryDistance.Azimuth;
                                Otherinventorydistance.Distance = GeneratorModel.OtherInventoryDistance.Distance;
                                Otherinventorydistance.ReferenceOtherInventoryId = GeneratorModel.OtherInventoryDistance.ReferenceOtherInventoryId;
                                var TabelTLIotherInventoryDistance = _unitOfWork.TablesNamesRepository.GetWhereFirst(x => x.TableName == "TLIotherInventoryDistance").Id;
                                _unitOfWork.OtherInventoryDistanceRepository.UpdateWithHLogic(UserId,HistoryId, TabelTLIotherInventoryDistance, OldOtherinventorydistance, Otherinventorydistance);
                                _unitOfWork.SaveChanges();

                            }
                            var sortedIds = _unitOfWork.CivilWithLegsRepository.ProcessDynamicAttributes(GeneratorModel, HistoryId);

                            if (GeneratorModel.dynamicAttribute != null && GeneratorModel.dynamicAttribute.Count > 0)
                            {
                                var sortedDynamicAttributes = GeneratorModel.dynamicAttribute
                                    .OrderBy(item => sortedIds.IndexOf(item.id))
                                    .ToList();

                                foreach (var item in sortedDynamicAttributes)
                                {
                                    var Message = _unitOfWork.CivilWithLegsRepository.EditCheckDynamicValidationAndDependence(item.id, item.value, Generator.Id, HistoryId).Message;
                                    if (Message != "Success")
                                    {
                                        return new Response<GetForAddOtherInventoryInstallationObject>(true, null, null, Message, (int)Helpers.Constants.ApiReturnCode.fail);
                                    }
                                }
                            }
                            await _unitOfWork.SaveChangesAsync();

                        }
                        else
                        {
                            return new Response<GetForAddOtherInventoryInstallationObject>(true, null, null, " The Generator is not found", (int)ApiReturnCode.fail);
                        }

                    }
                    else if (OtherInventoryType.TLIsolar.ToString().ToLower() == TableName.ToLower())
                    {
                        TLIsite OldSite = new TLIsite();
                        EditSolarInstallationObject SolarModel = _mapper.Map<EditSolarInstallationObject>(model);
                        TLIsolar Solar = _mapper.Map<TLIsolar>(SolarModel.installationAttributes);

                        TLIotherInSite SolarInst =
                           _dbContext.TLIotherInSite.AsNoTracking()
                          .Include(x => x.allOtherInventoryInst).
                           ThenInclude(x => x.solar).Include(x=>x.Site).FirstOrDefault(x => x.allOtherInventoryInst
                           .solarId == Solar.Id
                           && !x.Dismantle);

                        if (SolarInst != null)
                        {
                          
                            var CheckName = _dbContext.MV_SOLAR_VIEW.FirstOrDefault(x =>
                            !x.Dismantle &&
                             x.Id != Solar.Id &&
                             x.Name.ToLower() == Solar.Name.ToLower() &&
                             x.SITECODE.ToLower() == SolarInst.SiteCode.ToLower());

                            if (CheckName != null)
                                return new Response<GetForAddOtherInventoryInstallationObject>(true, null, null, $"The name {Solar.Name} is already exists", (int)ApiReturnCode.fail);
                        
                            if (SolarInst.ReservedSpace == true && SolarModel.OtherInSite.ReservedSpace == true)
                            {
                                if (Solar.SpaceInstallation != SolarInst.allOtherInventoryInst.solar.SpaceInstallation)
                                {
                                     NewSite = _dbContext.TLIsite.FirstOrDefault(x => x.SiteCode.ToLower() ==
                                    SolarInst.SiteCode.ToLower());

                                    NewSite.ReservedSpace = NewSite.ReservedSpace - SolarInst.allOtherInventoryInst.solar.SpaceInstallation;
                                    NewSite.ReservedSpace = NewSite.ReservedSpace + Solar.SpaceInstallation;
                                    _unitOfWork.SiteRepository.UpdateSiteWithHistory(UserId, SolarInst.Site, NewSite);
                                    _dbContext.SaveChanges();
                                }
                            }
                            else if (SolarInst.ReservedSpace == true && SolarModel.OtherInSite.ReservedSpace == false)
                            {

                                 NewSite = _dbContext.TLIsite.FirstOrDefault(x => x.SiteCode.ToLower() ==
                                SolarInst.SiteCode.ToLower());

                                NewSite.ReservedSpace = NewSite.ReservedSpace - SolarInst.allOtherInventoryInst.solar.SpaceInstallation;
                                _unitOfWork.SiteRepository.UpdateSiteWithHistory(UserId, SolarInst.Site, NewSite);
                                _dbContext.SaveChanges();
                            }
                            if (SolarInst.ReservedSpace == false && SolarModel.OtherInSite.ReservedSpace == true)
                            {
                               

                                var CheckSpace = _unitOfWork.SiteRepository.CheckSpaces(UserId, SolarInst.SiteCode, TableEntity.TableName, SolarModel.SolarType.SolarLibraryId, Solar.SpaceInstallation, null).Message;
                                if (CheckSpace != "Success")
                                {
                                    return new Response<GetForAddOtherInventoryInstallationObject>(true, null, null, CheckSpace, (int)Helpers.Constants.ApiReturnCode.fail);
                                }

                            }
                            //string CheckGeneralValidation = CheckGeneralValidationFunctionEditVersion(SolarModel.DynamicInstAttsValue, TableName);

                            //if (!string.IsNullOrEmpty(CheckGeneralValidation))
                            //    return new Response<GetForAddOtherInventoryInstallationObject>(true, null, null, CheckGeneralValidation, (int)ApiReturnCode.fail);

                            //string CheckDependencyValidation = CheckDependencyValidationEditVersion(model, SiteCode, TableName);

                            //if (!string.IsNullOrEmpty(CheckDependencyValidation))
                            //    return new Response<GetForAddOtherInventoryInstallationObject>(true, null, null, CheckDependencyValidation, (int)ApiReturnCode.fail);
                      
                            Solar.SolarLibraryId = SolarModel.SolarType.SolarLibraryId;
                            var HistoryId=_unitOfWork.SolarRepository.UpdateWithHInstallation(UserId,null, SolarInst.allOtherInventoryInst.solar, Solar, SolarInst.SiteCode, ExternalSys);
                            _unitOfWork.SaveChanges();
                            //----------------------------------------------------------------------------------//
                            //----------------------OtherOnSite-------------------------------------------------//
                            TLIotherInSite NewSolarInst =
                             _dbContext.TLIotherInSite
                            .Include(x => x.allOtherInventoryInst).
                             ThenInclude(x => x.solar).Include(x => x.Site).FirstOrDefault(x => x.allOtherInventoryInst
                             .solarId == Solar.Id
                             && !x.Dismantle);

                            NewSolarInst.OtherInSiteStatus = SolarModel.OtherInSite.OtherInSiteStatus;
                            NewSolarInst.InstallationDate = SolarModel.OtherInSite.InstallationDate;
                            NewSolarInst.ReservedSpace = SolarModel.OtherInSite.ReservedSpace;
                            NewSolarInst.Dismantle = SolarModel.OtherInSite.Dismantle;
                            NewSolarInst.OtherInventoryStatus = SolarModel.OtherInSite.OtherInventoryStatus;
                            var TabelTLIotherInSite = _unitOfWork.TablesNamesRepository.GetWhereFirst(x => x.TableName == "TLIotherInSite").Id;
                            _unitOfWork.OtherInSiteRepository.UpdateWithHLogic(UserId,HistoryId, TabelTLIotherInSite, SolarInst, NewSolarInst);
                            _unitOfWork.SaveChanges();

                            //----------------------------------------------------------------------------------//
                            //----------------------OtherInventoryDistance-------------------------------------------------//

                            var Otherinventorydistance = _unitOfWork.OtherInventoryDistanceRepository.GetIncludeWhereFirst
                                (x => x.allOtherInventoryInst.solarId == Solar.Id, x => x.allOtherInventoryInst);

                            var OldOtherinventorydistance = _unitOfWork.OtherInventoryDistanceRepository.GetAllAsQueryable().AsNoTracking()
                              .Include(x => x.allOtherInventoryInst).FirstOrDefault(x => x.allOtherInventoryInst.solarId
                              == Solar.Id);
                            if (OldOtherinventorydistance != null)
                            {
                                if (OldOtherinventorydistance.ReferenceOtherInventoryId != SolarModel.OtherInventoryDistance.ReferenceOtherInventoryId)
                                {
                                    var OtherInventoryDistance = _unitOfWork.OtherInventoryDistanceRepository.GetWhere(x => x.ReferenceOtherInventoryId
                                   == SolarModel.OtherInventoryDistance.ReferenceOtherInventoryId && x.Distance == SolarModel.OtherInventoryDistance.Distance
                                   && x.Azimuth == SolarModel.OtherInventoryDistance.Azimuth).Select(x => x.ReferenceOtherInventoryId).ToList();
                                    var OtherInSite = _unitOfWork.OtherInSiteRepository.GetWhere(x => OtherInventoryDistance.Any(y => y
                                    == x.allOtherInventoryInstId) && !x.Dismantle && x.SiteCode.ToLower() == SolarInst.SiteCode.ToLower()).ToList();
                                    if (OtherInSite.Count > 0)
                                    {
                                        return new Response<GetForAddOtherInventoryInstallationObject>(false, null, null, "can not select the otherinventory reference on same distance and azimuth because found other otherinventory reference in same place", (int)Helpers.Constants.ApiReturnCode.fail);
                                    }
                                }
                                Otherinventorydistance.Azimuth = SolarModel.OtherInventoryDistance.Azimuth;
                                Otherinventorydistance.Distance = SolarModel.OtherInventoryDistance.Distance;
                                Otherinventorydistance.ReferenceOtherInventoryId = SolarModel.OtherInventoryDistance.ReferenceOtherInventoryId;
                                var TabelTLIotherInventoryDistance = _unitOfWork.TablesNamesRepository.GetWhereFirst(x => x.TableName == "TLIotherInventoryDistance").Id;
                                _unitOfWork.OtherInventoryDistanceRepository.UpdateWithHLogic(UserId,HistoryId, TabelTLIotherInventoryDistance, OldOtherinventorydistance, Otherinventorydistance);
                                _unitOfWork.SaveChanges();
                            }
                            var sortedIds = _unitOfWork.CivilWithLegsRepository.ProcessDynamicAttributes(SolarModel, HistoryId);

                            if (SolarModel.dynamicAttribute != null && SolarModel.dynamicAttribute.Count > 0)
                            {
                                var sortedDynamicAttributes = SolarModel.dynamicAttribute
                                    .OrderBy(item => sortedIds.IndexOf(item.id))
                                    .ToList();

                                foreach (var item in sortedDynamicAttributes)
                                {
                                    var Message = _unitOfWork.CivilWithLegsRepository.EditCheckDynamicValidationAndDependence(item.id, item.value, Solar.Id, HistoryId).Message;
                                    if (Message != "Success")
                                    {
                                        return new Response<GetForAddOtherInventoryInstallationObject>(true, null, null, Message, (int)Helpers.Constants.ApiReturnCode.fail);
                                    }
                                }
                            }
                            await _unitOfWork.SaveChangesAsync();

                        }
                        else
                        {
                            return new Response<GetForAddOtherInventoryInstallationObject>(true, null, null, " The Solar is not found", (int)ApiReturnCode.fail);
                        }

                    }
                    if (TaskId != null)
                    {
                        var Submit = _unitOfWork.SiteRepository.SubmitTaskByTLI(TaskId);
                        var result = Submit.Result;
                        if (result.result == true && result.errorMessage == null)
                        {
                            _unitOfWork.SaveChanges();
                            transaction.Complete();
                        }
                        else
                        {
                            transaction.Dispose();
                            return new Response<GetForAddOtherInventoryInstallationObject>(true, null, null, result.errorMessage.ToString(), (int)ApiReturnCode.fail);
                        }
                    }
                    else
                    {
                        _unitOfWork.SaveChanges();
                        transaction.Complete();
                    }
                    return new Response<GetForAddOtherInventoryInstallationObject>();
                }
                catch (Exception err)
                {
                    return new Response<GetForAddOtherInventoryInstallationObject>(true, null, null, err.Message, (int)ApiReturnCode.fail);
                }
            }
        }
        public async Task<Response<GetForAddOtherInventoryInstallationObject>> EditCabinetPowerInstallation(EditCabinetPowerInstallationObject editCabinetPowerInstallationObject, string TableName, int? TaskId, int UserId, string connectionString,bool ExtenalSys)
        {
            using (TransactionScope transaction = new TransactionScope())
            {
                try
                {

                    TLIsite OldSite = new TLIsite();
                    TLIsite NewSite = new TLIsite();
                    TLItablesNames TableEntity = _unitOfWork.TablesNamesRepository.GetWhereFirst(x => x.TableName.ToLower() == TableName.ToLower());

                    TLIcabinet CabinetPower = _mapper.Map<TLIcabinet>(editCabinetPowerInstallationObject.installationAttributes);

                    TLIotherInSite CabinetPowerInst =
                           _dbContext.TLIotherInSite.AsNoTracking()
                          .Include(x => x.allOtherInventoryInst).
                           ThenInclude(x => x.cabinet).Include(x => x.Site).FirstOrDefault(x => x.allOtherInventoryInst
                           .cabinetId == CabinetPower.Id
                           && !x.Dismantle);

                    if (CabinetPowerInst != null)
                    {

                        var CheckName = _dbContext.MV_CABINET_POWER_VIEW.FirstOrDefault(x =>
                        !x.Dismantle &&
                         x.Id != CabinetPower.Id &&
                         x.Name.ToLower() == CabinetPower.Name.ToLower() &&
                         x.SITECODE.ToLower() == CabinetPowerInst.SiteCode.ToLower());

                        if (CheckName != null)
                            return new Response<GetForAddOtherInventoryInstallationObject>(true, null, null, $"The name {CabinetPower.Name} is already exists", (int)ApiReturnCode.fail);
                      
                        if (CabinetPowerInst.ReservedSpace == true && editCabinetPowerInstallationObject.OtherInSite.ReservedSpace == true)
                        {
                            if (CabinetPower.SpaceInstallation != CabinetPowerInst.allOtherInventoryInst.cabinet.SpaceInstallation)
                            {

                                 NewSite = _dbContext.TLIsite.FirstOrDefault(x => x.SiteCode.ToLower() ==
                                        CabinetPowerInst.SiteCode.ToLower());
                                NewSite.ReservedSpace = NewSite.ReservedSpace - CabinetPowerInst.allOtherInventoryInst.cabinet.SpaceInstallation;
                                NewSite.ReservedSpace = NewSite.ReservedSpace + CabinetPower.SpaceInstallation;
                                _unitOfWork.SiteRepository.UpdateSiteWithHistory(UserId, CabinetPowerInst.Site, NewSite);
                                _dbContext.SaveChanges();
                            }
                        }
                        else if (CabinetPowerInst.ReservedSpace == true && editCabinetPowerInstallationObject.OtherInSite.ReservedSpace == false)
                        {


                             NewSite = _dbContext.TLIsite.FirstOrDefault(x => x.SiteCode.ToLower() ==
                                    CabinetPowerInst.SiteCode.ToLower());
                            NewSite.ReservedSpace = NewSite.ReservedSpace - CabinetPowerInst.allOtherInventoryInst.cabinet.SpaceInstallation;
                            _unitOfWork.SiteRepository.UpdateSiteWithHistory(UserId, CabinetPowerInst.Site, NewSite);
                            _dbContext.SaveChanges();
                        }
                        if (CabinetPowerInst.ReservedSpace == false && editCabinetPowerInstallationObject.OtherInSite.ReservedSpace == true)
                        {


                            var CheckSpace = _unitOfWork.SiteRepository.CheckSpaces(UserId, CabinetPowerInst.SiteCode, TableEntity.TableName, editCabinetPowerInstallationObject.CabinetPowerType.CabinetPowerLibraryId, CabinetPower.SpaceInstallation, null).Message;
                            if (CheckSpace != "Success")
                            {
                                return new Response<GetForAddOtherInventoryInstallationObject>(true, null, null, CheckSpace, (int)Helpers.Constants.ApiReturnCode.fail);
                            }

                        }
                        //string CheckGeneralValidation = CheckGeneralValidationFunctionEditVersion(CabinetPowerModel.DynamicInstAttsValue, TableName);

                        //if (!string.IsNullOrEmpty(CheckGeneralValidation))
                        //    return new Response<GetForAddOtherInventoryInstallationObject>(true, null, null, CheckGeneralValidation, (int)ApiReturnCode.fail);

                        //string CheckDependencyValidation = CheckDependencyValidationEditVersion(model, SiteCode, TableName);

                        //if (!string.IsNullOrEmpty(CheckDependencyValidation))
                        //    return new Response<GetForAddOtherInventoryInstallationObject>(true, null, null, CheckDependencyValidation, (int)ApiReturnCode.fail);

                        CabinetPower.CabinetPowerLibraryId = editCabinetPowerInstallationObject.CabinetPowerType.CabinetPowerLibraryId;
                       var HistoryId= _unitOfWork.CabinetRepository.UpdateWithHInstallation(UserId,null, CabinetPowerInst.allOtherInventoryInst.cabinet, CabinetPower, CabinetPowerInst.SiteCode,ExtenalSys);
                        _unitOfWork.SaveChanges();
                        //----------------------------------------------------------------------------------//
                        //----------------------OtherOnSite-------------------------------------------------//
                        TLIotherInSite NewCabinetPowerInst =
                         _dbContext.TLIotherInSite
                        .Include(x => x.allOtherInventoryInst).
                         ThenInclude(x => x.cabinet).Include(x => x.Site).FirstOrDefault(x => x.allOtherInventoryInst
                         .cabinetId == CabinetPower.Id
                         && !x.Dismantle);

                        NewCabinetPowerInst.OtherInSiteStatus = editCabinetPowerInstallationObject.OtherInSite.OtherInSiteStatus;
                        NewCabinetPowerInst.InstallationDate = editCabinetPowerInstallationObject.OtherInSite.InstallationDate;
                        NewCabinetPowerInst.ReservedSpace = editCabinetPowerInstallationObject.OtherInSite.ReservedSpace;
                        NewCabinetPowerInst.Dismantle = editCabinetPowerInstallationObject.OtherInSite.Dismantle;
                        NewCabinetPowerInst.OtherInventoryStatus = editCabinetPowerInstallationObject.OtherInSite.OtherInventoryStatus;
                        var TabelTLIotherInSite = _unitOfWork.TablesNamesRepository.GetWhereFirst(x => x.TableName == "TLIotherInSite").Id;
                        _unitOfWork.OtherInSiteRepository.UpdateWithHLogic(UserId,HistoryId, TabelTLIotherInSite, CabinetPowerInst, NewCabinetPowerInst);
                        _unitOfWork.SaveChanges();

                        //----------------------------------------------------------------------------------//
                        //----------------------OtherInventoryDistance-------------------------------------------------//

                        var Otherinventorydistance = _unitOfWork.OtherInventoryDistanceRepository.GetIncludeWhereFirst
                            (x => x.allOtherInventoryInst.cabinetId == CabinetPower.Id, x => x.allOtherInventoryInst);

                        var OldOtherinventorydistance = _unitOfWork.OtherInventoryDistanceRepository.GetAllAsQueryable().AsNoTracking()
                          .Include(x => x.allOtherInventoryInst).FirstOrDefault(x => x.allOtherInventoryInst.cabinetId
                          == CabinetPower.Id);
                        if (OldOtherinventorydistance != null)
                        {
                            if (OldOtherinventorydistance.ReferenceOtherInventoryId != editCabinetPowerInstallationObject.OtherInventoryDistance.ReferenceOtherInventoryId)
                            {
                                var OtherInventoryDistance = _unitOfWork.OtherInventoryDistanceRepository.GetWhere(x => x.ReferenceOtherInventoryId
                                    == editCabinetPowerInstallationObject.OtherInventoryDistance.ReferenceOtherInventoryId && x.Distance == editCabinetPowerInstallationObject.OtherInventoryDistance.Distance
                                    && x.Azimuth == editCabinetPowerInstallationObject.OtherInventoryDistance.Azimuth).Select(x => x.ReferenceOtherInventoryId).ToList();
                                var OtherInSite = _unitOfWork.OtherInSiteRepository.GetWhere(x => OtherInventoryDistance.Any(y => y
                                == x.allOtherInventoryInstId) && !x.Dismantle && x.SiteCode.ToLower() == CabinetPowerInst.SiteCode.ToLower()).ToList();
                                if (OtherInSite.Count > 0)
                                {
                                    return new Response<GetForAddOtherInventoryInstallationObject>(false, null, null, "can not select the otherinventory reference on same distance and azimuth because found other otherinventory reference in same place", (int)Helpers.Constants.ApiReturnCode.fail);
                                }
                            }
                            Otherinventorydistance.Azimuth = editCabinetPowerInstallationObject.OtherInventoryDistance.Azimuth;
                            Otherinventorydistance.Distance = editCabinetPowerInstallationObject.OtherInventoryDistance.Distance;
                            Otherinventorydistance.ReferenceOtherInventoryId = editCabinetPowerInstallationObject.OtherInventoryDistance.ReferenceOtherInventoryId;
                            var TabelTLIotherInventoryDistance = _unitOfWork.TablesNamesRepository.GetWhereFirst(x => x.TableName == "TLIotherInventoryDistance").Id;
                            _unitOfWork.OtherInventoryDistanceRepository.UpdateWithHLogic(UserId,HistoryId, TabelTLIotherInventoryDistance, OldOtherinventorydistance, Otherinventorydistance);
                            _unitOfWork.SaveChanges();
                        }
                        var sortedIds = _unitOfWork.CivilWithLegsRepository.ProcessDynamicAttributes(editCabinetPowerInstallationObject, HistoryId);

                        if (editCabinetPowerInstallationObject.dynamicAttribute != null && editCabinetPowerInstallationObject.dynamicAttribute.Count > 0)
                        {
                            var sortedDynamicAttributes = editCabinetPowerInstallationObject.dynamicAttribute
                                .OrderBy(item => sortedIds.IndexOf(item.id))
                                .ToList();

                            foreach (var item in sortedDynamicAttributes)
                            {
                                var Message = _unitOfWork.CivilWithLegsRepository.EditCheckDynamicValidationAndDependence(item.id, item.value, CabinetPower.Id, HistoryId).Message;
                                if (Message != "Success")
                                {
                                    return new Response<GetForAddOtherInventoryInstallationObject>(true, null, null, Message, (int)Helpers.Constants.ApiReturnCode.fail);
                                }
                            }
                        }
                        await _unitOfWork.SaveChangesAsync();

                    }
                    else
                    {
                        return new Response<GetForAddOtherInventoryInstallationObject>(true, null, null, " The CabinetPower is not found", (int)ApiReturnCode.fail);
                    }


                    if (TaskId != null)
                    {
                        var Submit = _unitOfWork.SiteRepository.SubmitTaskByTLI(TaskId);
                        var result = Submit.Result;
                        if (result.result == true && result.errorMessage == null)
                        {
                            _unitOfWork.SaveChanges();
                            transaction.Complete();
                        }
                        else
                        {
                            transaction.Dispose();
                            return new Response<GetForAddOtherInventoryInstallationObject>(true, null, null, result.errorMessage.ToString(), (int)ApiReturnCode.fail);
                        }
                    }
                    else
                    {
                        _unitOfWork.SaveChanges();
                        transaction.Complete();
                    }
                    return new Response<GetForAddOtherInventoryInstallationObject>();
                }
                catch (Exception err)
                {
                    return new Response<GetForAddOtherInventoryInstallationObject>(true, null, null, err.Message, (int)ApiReturnCode.fail);
                }
            }
        }
        public async Task<Response<GetForAddOtherInventoryInstallationObject>> EditCabinetTelecomInstallation(EditCabinetTelecomInstallationObject editCabinetTelecomInstallationObject, string TableName, int? TaskId, int UserId, string connectionString,bool ExternalSys)
        {
            using (TransactionScope transaction = new TransactionScope())
            {
                try
                {

                    TLIsite OldSite = new TLIsite();
                    TLIsite  NewSite = new TLIsite();
                    TLItablesNames TableEntity = _unitOfWork.TablesNamesRepository.GetWhereFirst(x => x.TableName.ToLower() == TableName.ToLower());

                    TLIcabinet CabinetTelecom = _mapper.Map<TLIcabinet>(editCabinetTelecomInstallationObject.installationAttributes);

                    TLIotherInSite CabinetPowerInst =
                           _dbContext.TLIotherInSite.AsNoTracking()
                          .Include(x => x.allOtherInventoryInst).
                           ThenInclude(x => x.cabinet).Include(x => x.Site).FirstOrDefault(x => x.allOtherInventoryInst
                           .cabinetId == CabinetTelecom.Id
                           && !x.Dismantle);

                    if (CabinetPowerInst != null)
                    {

                        var CheckName = _dbContext.MV_CABINET_TELECOM_VIEW.FirstOrDefault(x =>
                        !x.Dismantle &&
                         x.Id != CabinetTelecom.Id &&
                         x.Name.ToLower() == CabinetTelecom.Name.ToLower() &&
                         x.SITECODE.ToLower() == CabinetPowerInst.SiteCode.ToLower());

                        if (CheckName != null)
                            return new Response<GetForAddOtherInventoryInstallationObject>(true, null, null, $"The name {CabinetTelecom.Name} is already exists", (int)ApiReturnCode.fail);
                     
                        if (CabinetPowerInst.ReservedSpace == true && editCabinetTelecomInstallationObject.OtherInSite.ReservedSpace == true)
                        {
                            if (CabinetTelecom.SpaceInstallation != CabinetPowerInst.allOtherInventoryInst.cabinet.SpaceInstallation)
                            {

                                 NewSite = _dbContext.TLIsite.FirstOrDefault(x => x.SiteCode.ToLower() ==
                                        CabinetPowerInst.SiteCode.ToLower());

                                NewSite.ReservedSpace = NewSite.ReservedSpace - CabinetPowerInst.allOtherInventoryInst.cabinet.SpaceInstallation;
                                NewSite.ReservedSpace = NewSite.ReservedSpace + CabinetTelecom.SpaceInstallation;
                                _unitOfWork.SiteRepository.UpdateSiteWithHistory(UserId, CabinetPowerInst.Site, NewSite);
                                _dbContext.SaveChanges();
                            }
                        }
                        else if (CabinetPowerInst.ReservedSpace == true && editCabinetTelecomInstallationObject.OtherInSite.ReservedSpace == false)
                        {


                             NewSite = _dbContext.TLIsite.FirstOrDefault(x => x.SiteCode.ToLower() ==
                                    CabinetPowerInst.SiteCode.ToLower());
                            NewSite.ReservedSpace = NewSite.ReservedSpace - CabinetPowerInst.allOtherInventoryInst.cabinet.SpaceInstallation;
                            _unitOfWork.SiteRepository.UpdateSiteWithHistory(UserId, CabinetPowerInst.Site, NewSite);
                            _dbContext.SaveChanges();
                        }
                        if (CabinetPowerInst.ReservedSpace == false && editCabinetTelecomInstallationObject.OtherInSite.ReservedSpace == true)
                        {


                            var CheckSpace = _unitOfWork.SiteRepository.CheckSpaces(UserId, CabinetPowerInst.SiteCode, TableEntity.TableName, editCabinetTelecomInstallationObject.CabinetTelecomType.CabinetTelecomLibraryId, CabinetTelecom.SpaceInstallation, null).Message;
                            if (CheckSpace != "Success")
                            {
                                return new Response<GetForAddOtherInventoryInstallationObject>(true, null, null, CheckSpace, (int)Helpers.Constants.ApiReturnCode.fail);
                            }

                        }
                        //string CheckGeneralValidation = CheckGeneralValidationFunctionEditVersion(CabinetPowerModel.DynamicInstAttsValue, TableName);

                        //if (!string.IsNullOrEmpty(CheckGeneralValidation))
                        //    return new Response<GetForAddOtherInventoryInstallationObject>(true, null, null, CheckGeneralValidation, (int)ApiReturnCode.fail);

                        //string CheckDependencyValidation = CheckDependencyValidationEditVersion(model, SiteCode, TableName);

                        //if (!string.IsNullOrEmpty(CheckDependencyValidation))
                        //    return new Response<GetForAddOtherInventoryInstallationObject>(true, null, null, CheckDependencyValidation, (int)ApiReturnCode.fail);

                        CabinetTelecom.CabinetTelecomLibraryId = editCabinetTelecomInstallationObject.CabinetTelecomType.CabinetTelecomLibraryId;
                        var HistoryId=_unitOfWork.CabinetRepository.UpdateWithHInstallation(UserId,null, CabinetPowerInst.allOtherInventoryInst.cabinet, CabinetTelecom, CabinetPowerInst.SiteCode,ExternalSys);
                        _unitOfWork.SaveChanges();
                        //----------------------------------------------------------------------------------//
                        //----------------------OtherOnSite-------------------------------------------------//
                        TLIotherInSite NewCabinetPowerInst =
                         _dbContext.TLIotherInSite
                        .Include(x => x.allOtherInventoryInst).
                         ThenInclude(x => x.cabinet).Include(x => x.Site).FirstOrDefault(x => x.allOtherInventoryInst
                         .cabinetId == CabinetTelecom.Id
                         && !x.Dismantle);

                        NewCabinetPowerInst.OtherInSiteStatus = editCabinetTelecomInstallationObject.OtherInSite.OtherInSiteStatus;
                        NewCabinetPowerInst.InstallationDate = editCabinetTelecomInstallationObject.OtherInSite.InstallationDate;
                        NewCabinetPowerInst.ReservedSpace = editCabinetTelecomInstallationObject.OtherInSite.ReservedSpace;
                        NewCabinetPowerInst.Dismantle = editCabinetTelecomInstallationObject.OtherInSite.Dismantle;
                        NewCabinetPowerInst.OtherInventoryStatus = editCabinetTelecomInstallationObject.OtherInSite.OtherInventoryStatus;
                        var TabelTLIotherInSite = _unitOfWork.TablesNamesRepository.GetWhereFirst(x => x.TableName == "TLIotherInSite").Id;
                        _unitOfWork.OtherInSiteRepository.UpdateWithHLogic(UserId,HistoryId, TabelTLIotherInSite, CabinetPowerInst, NewCabinetPowerInst);
                        _unitOfWork.SaveChanges();

                        //----------------------------------------------------------------------------------//
                        //----------------------OtherInventoryDistance-------------------------------------------------//

                        var Otherinventorydistance = _unitOfWork.OtherInventoryDistanceRepository.GetIncludeWhereFirst
                            (x => x.allOtherInventoryInst.cabinetId == CabinetTelecom.Id, x => x.allOtherInventoryInst);

                        var OldOtherinventorydistance = _unitOfWork.OtherInventoryDistanceRepository.GetAllAsQueryable().AsNoTracking()
                          .Include(x => x.allOtherInventoryInst).FirstOrDefault(x => x.allOtherInventoryInst.cabinetId
                          == CabinetTelecom.Id);
                        if (OldOtherinventorydistance != null)
                        {
                            if (OldOtherinventorydistance.ReferenceOtherInventoryId != editCabinetTelecomInstallationObject.OtherInventoryDistance.ReferenceOtherInventoryId)
                            {
                                var OtherInventoryDistance = _unitOfWork.OtherInventoryDistanceRepository.GetWhere(x => x.ReferenceOtherInventoryId
                                   == editCabinetTelecomInstallationObject.OtherInventoryDistance.ReferenceOtherInventoryId && x.Distance == editCabinetTelecomInstallationObject.OtherInventoryDistance.Distance
                                   && x.Azimuth == editCabinetTelecomInstallationObject.OtherInventoryDistance.Azimuth).Select(x => x.ReferenceOtherInventoryId).ToList();
                                var OtherInSite = _unitOfWork.OtherInSiteRepository.GetWhere(x => OtherInventoryDistance.Any(y => y
                                == x.allOtherInventoryInstId) && !x.Dismantle && x.SiteCode.ToLower() == CabinetPowerInst.SiteCode.ToLower()).ToList();
                                if (OtherInSite.Count > 0)
                                {
                                    return new Response<GetForAddOtherInventoryInstallationObject>(false, null, null, "can not select the otherinventory reference on same distance and azimuth because found other otherinventory reference in same place", (int)Helpers.Constants.ApiReturnCode.fail);
                                }
                            }
                            Otherinventorydistance.Azimuth = editCabinetTelecomInstallationObject.OtherInventoryDistance.Azimuth;
                            Otherinventorydistance.Distance = editCabinetTelecomInstallationObject.OtherInventoryDistance.Distance;
                            Otherinventorydistance.ReferenceOtherInventoryId = editCabinetTelecomInstallationObject.OtherInventoryDistance.ReferenceOtherInventoryId;
                            var TabelTLIotherInventoryDistance = _unitOfWork.TablesNamesRepository.GetWhereFirst(x => x.TableName == "TLIotherInventoryDistance").Id;
                            _unitOfWork.OtherInventoryDistanceRepository.UpdateWithHLogic(UserId,HistoryId, TabelTLIotherInventoryDistance, OldOtherinventorydistance, Otherinventorydistance);
                            _unitOfWork.SaveChanges();
                        }
                        var sortedIds = _unitOfWork.CivilWithLegsRepository.ProcessDynamicAttributes(editCabinetTelecomInstallationObject, HistoryId);

                        if (editCabinetTelecomInstallationObject.dynamicAttribute != null && editCabinetTelecomInstallationObject.dynamicAttribute.Count > 0)
                        {
                            var sortedDynamicAttributes = editCabinetTelecomInstallationObject.dynamicAttribute
                                .OrderBy(item => sortedIds.IndexOf(item.id))
                                .ToList();

                            foreach (var item in sortedDynamicAttributes)
                            {
                                var Message = _unitOfWork.CivilWithLegsRepository.EditCheckDynamicValidationAndDependence(item.id, item.value, CabinetTelecom.Id, HistoryId).Message;
                                if (Message != "Success")
                                {
                                    return new Response<GetForAddOtherInventoryInstallationObject>(true, null, null, Message, (int)Helpers.Constants.ApiReturnCode.fail);
                                }
                            }
                        }
                        await _unitOfWork.SaveChangesAsync();

                    }
                    else
                    {
                        return new Response<GetForAddOtherInventoryInstallationObject>(true, null, null, " The CabinetPower is not found", (int)ApiReturnCode.fail);
                    }


                    if (TaskId != null)
                    {
                        var Submit = _unitOfWork.SiteRepository.SubmitTaskByTLI(TaskId);
                        var result = Submit.Result;
                        if (result.result == true && result.errorMessage == null)
                        {
                            _unitOfWork.SaveChanges();
                            transaction.Complete();
                        }
                        else
                        {
                            transaction.Dispose();
                            return new Response<GetForAddOtherInventoryInstallationObject>(true, null, null, result.errorMessage.ToString(), (int)ApiReturnCode.fail);
                        }
                    }
                    else
                    {
                        _unitOfWork.SaveChanges();
                        transaction.Complete();
                    }
                    return new Response<GetForAddOtherInventoryInstallationObject>();
                }
                catch (Exception err)
                {
                    return new Response<GetForAddOtherInventoryInstallationObject>(true, null, null, err.Message, (int)ApiReturnCode.fail);
                }
            }
        }
        #region Helper Methods For UpdateSideArm Function..
        public string CheckDependencyValidationEditVersion(object Input, string SiteCode, string OtherInventoryType)
        {
            if (OtherInventoryType.ToLower() == TablesNames.TLIcabinet.ToString().ToLower())
            {
                string MainTableName = TablesNames.TLIcabinet.ToString();
                EditCabinetViewModel EditInstallationViewModel = _mapper.Map<EditCabinetViewModel>(Input);

                List<DynamicAttViewModel> DynamicAttributes = _mapper.Map<List<DynamicAttViewModel>>(_unitOfWork.DynamicAttRepository
                    .GetIncludeWhere(x => x.tablesNames.TableName.ToLower() == MainTableName.ToLower() && !x.disable
                        , x => x.tablesNames).ToList());

                foreach (DynamicAttViewModel DynamicAttribute in DynamicAttributes)
                {
                    TLIdependency DynamicAttributeMainDependency = _unitOfWork.DependencieRepository.GetIncludeWhereFirst(x => x.DynamicAttId == DynamicAttribute.Id &&
                        (x.ValueBoolean != null || x.ValueDateTime != null || x.ValueDouble != null || !string.IsNullOrEmpty(x.ValueString)) &&
                        x.OperationId != null, x => x.Operation);

                    if (DynamicAttributeMainDependency == null)
                        continue;

                    List<int> DependencyRows = _unitOfWork.DependencyRowRepository.GetWhere(x => x.DependencyId == DynamicAttributeMainDependency.Id)
                        .Select(x => x.RowId.Value).Distinct().ToList();

                    foreach (int RowId in DependencyRows)
                    {
                        List<TLIrule> Rules = _unitOfWork.RowRuleRepository.GetIncludeWhere(x => x.RowId == RowId, x => x.Rule, x => x.Rule.tablesNames,
                            x => x.Rule.Operation, x => x.Rule.dynamicAtt, x => x.Rule.attributeActivated).Select(x => x.Rule).ToList();

                        int CheckIfSuccessAllRules = 0;

                        foreach (TLIrule Rule in Rules)
                        {
                            string SDTableName = Rule.tablesNames.TableName;

                            string DataType = "";

                            string Operation = Rule.Operation.Name;
                            object OperationValue = new object();

                            if (Rule.OperationValueBoolean != null)
                            {
                                DataType = "Bool";
                                OperationValue = Rule.OperationValueBoolean;
                            }
                            else if (Rule.OperationValueDateTime != null)
                            {
                                DataType = "DateTime";
                                OperationValue = Rule.OperationValueDateTime;
                            }
                            else if (Rule.OperationValueDouble != null)
                            {
                                DataType = "Double";
                                OperationValue = Rule.OperationValueDouble;
                            }
                            else if (!string.IsNullOrEmpty(Rule.OperationValueString))
                            {
                                DataType = "String";
                                OperationValue = Rule.OperationValueString;
                            }

                            if (MainTableName.ToLower() == SDTableName.ToLower())
                            {
                                object InsertedValue = new object();

                                if (Rule.attributeActivatedId != null)
                                {
                                    string AttributeName = Rule.attributeActivated.Key;

                                    object TestValue = EditInstallationViewModel.GetType().GetProperties()
                                        .FirstOrDefault(x => x.Name.ToLower() == AttributeName.ToLower()).GetValue(EditInstallationViewModel, null);

                                    if (TestValue == null)
                                        break;

                                    if (Rule.OperationValueBoolean != null)
                                        InsertedValue = bool.Parse(TestValue.ToString());

                                    else if (Rule.OperationValueDateTime != null)
                                        InsertedValue = DateTime.Parse(TestValue.ToString());

                                    else if (Rule.OperationValueDouble != null)
                                        InsertedValue = double.Parse(TestValue.ToString());

                                    else if (!string.IsNullOrEmpty(Rule.OperationValueString))
                                        InsertedValue = TestValue.ToString();
                                }
                                else if (Rule.dynamicAttId != null)
                                {
                                    BaseInstAttView DynamicObject = EditInstallationViewModel.DynamicInstAttsValue
                                        .FirstOrDefault(x => x.Id == Rule.dynamicAttId.Value);

                                    if (DynamicObject == null)
                                        break;

                                    string DynamicAttributeDataType = _unitOfWork.DataTypeRepository.GetByID(DynamicObject.DataTypeId.Value).Name;

                                    if (DynamicAttributeDataType.ToLower() == "boolean".ToLower())
                                        InsertedValue = bool.Parse(DynamicObject.Value.ToString());

                                    else if (DynamicAttributeDataType.ToLower() == "string".ToLower())
                                        InsertedValue = DynamicObject.Value.ToString();

                                    else if (DynamicAttributeDataType.ToLower() == "int".ToLower() || DynamicAttributeDataType.ToLower() == "double".ToLower())
                                        InsertedValue = double.Parse(DynamicObject.Value.ToString());

                                    else if (DynamicAttributeDataType.ToLower() == "datetime".ToLower())
                                        InsertedValue = DateTime.Parse(DynamicObject.Value.ToString());
                                }

                                if (Operation == "==" ? InsertedValue.ToString().ToLower() == OperationValue.ToString().ToLower() :
                                    Operation == "!=" ? InsertedValue.ToString().ToLower() != OperationValue.ToString().ToLower() :
                                    Operation == ">" ? Comparer.DefaultInvariant.Compare(InsertedValue, OperationValue) == 1 :
                                    Operation == ">=" ? (Comparer.DefaultInvariant.Compare(InsertedValue, OperationValue) == 1 ||
                                        InsertedValue.ToString().ToLower() == OperationValue.ToString().ToLower()) :
                                    Operation == "<" ? Comparer.DefaultInvariant.Compare(InsertedValue, OperationValue) == -1 :
                                    Operation == "<=" ? (Comparer.DefaultInvariant.Compare(InsertedValue, OperationValue) == -1 ||
                                        InsertedValue.ToString().ToLower() == OperationValue.ToString().ToLower()) : false)
                                {
                                    CheckIfSuccessAllRules++;
                                }
                            }
                            else
                            {
                                List<object> TableRecords = new List<object>();
                                if (Rule.attributeActivatedId != null)
                                {
                                    string AttributeName = Rule.attributeActivated.Key;

                                    if (OperationValue != null)
                                        TableRecords = _mapper.Map<List<object>>(_dbContext.GetType().GetProperty(SDTableName)
                                            .GetValue(_dbContext, null)).Where(x => x.GetType().GetProperty(AttributeName).GetValue(x, null) != null ? (Operation == ">" ?
                                                (DataType.ToLower() == "DateTime".ToLower() ?
                                                    Comparer.DefaultInvariant.Compare(DateTime.Parse(x.GetType().GetProperty(AttributeName).GetValue(x, null).ToString()), OperationValue) == 1 :
                                                DataType.ToLower() == "Double".ToLower() ?
                                                    Comparer.DefaultInvariant.Compare(double.Parse(x.GetType().GetProperty(AttributeName).GetValue(x, null).ToString()), OperationValue) == 1 : false) :
                                            Operation == ">=" ?
                                                (DataType.ToLower() == "DateTime".ToLower() ?
                                                    (Comparer.DefaultInvariant.Compare(DateTime.Parse(x.GetType().GetProperty(AttributeName).GetValue(x, null).ToString()), OperationValue) == 1 ||
                                                        x.GetType().GetProperty(AttributeName).GetValue(x, null).ToString().ToLower() == OperationValue.ToString().ToLower()) :
                                                DataType.ToLower() == "Double".ToLower() ?
                                                    (Comparer.DefaultInvariant.Compare(double.Parse(x.GetType().GetProperty(AttributeName).GetValue(x, null).ToString()), OperationValue) == 1 ||
                                                        x.GetType().GetProperty(AttributeName).GetValue(x, null).ToString().ToLower() == OperationValue.ToString().ToLower()) : false) :
                                            Operation == "<" ?
                                                (DataType.ToLower() == "DateTime".ToLower() ?
                                                    Comparer.DefaultInvariant.Compare(DateTime.Parse(x.GetType().GetProperty(AttributeName).GetValue(x, null).ToString()), OperationValue) == -1 :
                                                DataType.ToLower() == "Double".ToLower() ?
                                                    Comparer.DefaultInvariant.Compare(double.Parse(x.GetType().GetProperty(AttributeName).GetValue(x, null).ToString()), OperationValue) == -1 : false) :
                                            Operation == "<=" ?
                                                (DataType.ToLower() == "DateTime".ToLower() ?
                                                    (Comparer.DefaultInvariant.Compare(DateTime.Parse(x.GetType().GetProperty(AttributeName).GetValue(x, null).ToString()), OperationValue) == -1 ||
                                                        x.GetType().GetProperty(AttributeName).GetValue(x, null).ToString().ToLower() == OperationValue.ToString().ToLower()) :
                                                DataType.ToLower() == "Double".ToLower() ?
                                                    (Comparer.DefaultInvariant.Compare(double.Parse(x.GetType().GetProperty(AttributeName).GetValue(x, null).ToString()), OperationValue) == -1 ||
                                                        x.GetType().GetProperty(AttributeName).GetValue(x, null).ToString().ToLower() == OperationValue.ToString().ToLower()) : false) :
                                            Operation == "==" ?
                                                x.GetType().GetProperty(AttributeName).GetValue(x, null).ToString().ToLower() == OperationValue.ToString().ToLower() :
                                            Operation == "!=" ?
                                                x.GetType().GetProperty(AttributeName).GetValue(x, null).ToString().ToLower() != OperationValue.ToString().ToLower() : false) : false).ToList();

                                }
                                else if (Rule.dynamicAttId != null)
                                {
                                    List<int> DynamicAttValuesInventoryIds = new List<int>();

                                    if (!DynamicAttribute.LibraryAtt)
                                    {
                                        DynamicAttValuesInventoryIds = _unitOfWork.DynamicAttInstValueRepository
                                            .GetWhere(x => (x.DynamicAttId == Rule.dynamicAttId.Value && !x.disable) &&
                                                (Operation == "==" ?
                                                    ((Rule.OperationValueBoolean != null ? x.ValueBoolean.ToString().ToLower() == Rule.OperationValueBoolean.ToString().ToLower() : false) ||
                                                    (Rule.OperationValueDateTime != null ? x.ValueDateTime.ToString().ToLower() == Rule.OperationValueDateTime.ToString().ToLower() : false) ||
                                                    (Rule.OperationValueDouble != null ? x.ValueDouble == Rule.OperationValueDouble : false) ||
                                                    (!string.IsNullOrEmpty(Rule.OperationValueString) ? x.ValueString.ToLower() == Rule.OperationValueString.ToLower() : false)) : false) ||

                                                (Operation == "!=" ?
                                                    ((Rule.OperationValueBoolean != null ? x.ValueBoolean.ToString().ToLower() != Rule.OperationValueBoolean.ToString().ToLower() : false) ||
                                                    (Rule.OperationValueDateTime != null ? x.ValueDateTime.ToString().ToLower() != Rule.OperationValueDateTime.ToString().ToLower() : false) ||
                                                    (Rule.OperationValueDouble != null ? x.ValueDouble != Rule.OperationValueDouble : false) ||
                                                    (!string.IsNullOrEmpty(Rule.OperationValueString) ? x.ValueString.ToLower() != Rule.OperationValueString.ToLower() : false)) : false) ||

                                                (Operation == ">" ?
                                                    ((Rule.OperationValueDateTime != null ? x.ValueDateTime > Rule.OperationValueDateTime : false) ||
                                                    (Rule.OperationValueDouble != null ? x.ValueDouble > Rule.OperationValueDouble : false)) : false) ||

                                                (Operation == ">=" ?
                                                    ((Rule.OperationValueDateTime != null ? x.ValueDateTime >= Rule.OperationValueDateTime : false) ||
                                                    (Rule.OperationValueDouble != null ? x.ValueDouble >= Rule.OperationValueDouble : false)) : false) ||

                                                (Operation == "<" ?
                                                    ((Rule.OperationValueDateTime != null ? x.ValueDateTime < Rule.OperationValueDateTime : false) ||
                                                    (Rule.OperationValueDouble != null ? x.ValueDouble < Rule.OperationValueDouble : false)) : false) ||

                                                (Operation == "<=" ?
                                                    ((Rule.OperationValueDateTime != null ? x.ValueDateTime <= Rule.OperationValueDateTime : false) ||
                                                    (Rule.OperationValueDouble != null ? x.ValueDouble <= Rule.OperationValueDouble : false)) : false)

                                                ).Select(x => x.InventoryId).ToList();
                                    }
                                    else
                                    {
                                        DynamicAttValuesInventoryIds = _unitOfWork.DynamicAttLibRepository
                                            .GetWhere(x => (x.DynamicAttId == Rule.dynamicAttId && !x.disable) &&
                                                (Operation == "==" ?
                                                    ((Rule.OperationValueBoolean != null ? x.ValueBoolean.ToString().ToLower() == Rule.OperationValueBoolean.ToString().ToLower() : false) ||
                                                    (Rule.OperationValueDateTime != null ? x.ValueDateTime.ToString().ToLower() == Rule.OperationValueDateTime.ToString().ToLower() : false) ||
                                                    (Rule.OperationValueDouble != null ? x.ValueDouble == Rule.OperationValueDouble : false) ||
                                                    (!string.IsNullOrEmpty(Rule.OperationValueString) ? x.ValueString.ToLower() == Rule.OperationValueString.ToLower() : false)) : false) ||

                                                (Operation == "!=" ?
                                                    ((Rule.OperationValueBoolean != null ? x.ValueBoolean.ToString().ToLower() != Rule.OperationValueBoolean.ToString().ToLower() : false) ||
                                                    (Rule.OperationValueDateTime != null ? x.ValueDateTime.ToString().ToLower() != Rule.OperationValueDateTime.ToString().ToLower() : false) ||
                                                    (Rule.OperationValueDouble != null ? x.ValueDouble != Rule.OperationValueDouble : false) ||
                                                    (!string.IsNullOrEmpty(Rule.OperationValueString) ? x.ValueString.ToLower() != Rule.OperationValueString.ToLower() : false)) : false) ||

                                                (Operation == ">" ?
                                                    ((Rule.OperationValueDateTime != null ? x.ValueDateTime > Rule.OperationValueDateTime : false) ||
                                                    (Rule.OperationValueDouble != null ? x.ValueDouble > Rule.OperationValueDouble : false)) : false) ||

                                                (Operation == ">=" ?
                                                    ((Rule.OperationValueDateTime != null ? x.ValueDateTime >= Rule.OperationValueDateTime : false) ||
                                                    (Rule.OperationValueDouble != null ? x.ValueDouble >= Rule.OperationValueDouble : false)) : false) ||

                                                (Operation == "<" ?
                                                    ((Rule.OperationValueDateTime != null ? x.ValueDateTime < Rule.OperationValueDateTime : false) ||
                                                    (Rule.OperationValueDouble != null ? x.ValueDouble < Rule.OperationValueDouble : false)) : false) ||

                                                (Operation == "<=" ?
                                                    ((Rule.OperationValueDateTime != null ? x.ValueDateTime <= Rule.OperationValueDateTime : false) ||
                                                    (Rule.OperationValueDouble != null ? x.ValueDouble <= Rule.OperationValueDouble : false)) : false)

                                                ).Select(x => x.InventoryId).ToList();
                                    }
                                    if (DynamicAttValuesInventoryIds != null ? DynamicAttValuesInventoryIds.Count() != 0 : false)
                                    {
                                        TableRecords = _mapper.Map<List<object>>(_dbContext.GetType()
                                            .GetProperty(SDTableName).GetValue(_dbContext, null))
                                                .Where(x => DynamicAttValuesInventoryIds.Contains(Convert.ToInt32(x.GetType().GetProperty("Id").GetValue(x, null)))).ToList();
                                    }
                                }

                                AddInstRuleViewModel AddInstRuleViewModel = new AddInstRuleViewModel();
                                if (Rule.dynamicAttId != null)
                                {
                                    AddInstRuleViewModel = new AddInstRuleViewModel
                                    {
                                        dynamicAttId = Rule.dynamicAttId,
                                        IsDynamic = true,
                                        OperationId = Rule.OperationId,
                                        OperationValueBoolean = Rule.OperationValueBoolean,
                                        OperationValueDateTime = Rule.OperationValueDateTime,
                                        OperationValueDouble = Rule.OperationValueDouble,
                                        OperationValueString = Rule.OperationValueString,
                                        TableName = Rule.tablesNames.TableName
                                    };
                                }
                                else if (Rule.attributeActivatedId != null)
                                {
                                    AddInstRuleViewModel = new AddInstRuleViewModel
                                    {
                                        attributeActivatedId = Rule.attributeActivatedId,
                                        IsDynamic = false,
                                        OperationId = Rule.OperationId,
                                        OperationValueBoolean = Rule.OperationValueBoolean,
                                        OperationValueDateTime = Rule.OperationValueDateTime,
                                        OperationValueDouble = Rule.OperationValueDouble,
                                        OperationValueString = Rule.OperationValueString,
                                        TableName = Rule.tablesNames.TableName
                                    };
                                }
                                List<object> RecordsIds = _mapper.Map<List<object>>(GetRecordsIds(MainTableName, AddInstRuleViewModel));

                                PathToCheckDependencyValidation Item = (PathToCheckDependencyValidation)Enum.Parse(typeof(PathToCheckDependencyValidation),
                                    MainTableName + SDTableName + "Goal");

                                List<string> Path = GetEnumDescription(Item).Split(" ").ToList();

                                object CheckId = new object();

                                if (Path.Count() > 1)
                                {
                                    object CivilLoads = EditInstallationViewModel.GetType().GetProperty(Path[0])
                                        .GetValue(EditInstallationViewModel, null);

                                    CheckId = CivilLoads.GetType().GetProperty(Path[1]).GetValue(CivilLoads, null) != null ?
                                        (int)CivilLoads.GetType().GetProperty(Path[1]).GetValue(CivilLoads, null) : new object();
                                }
                                else if (Path.Count() == 1 && Path[0].ToLower() == "sitecode")
                                {
                                    CheckId = SiteCode;
                                }
                                else if (Path.Count() == 1)
                                {
                                    if (EditInstallationViewModel.GetType().GetProperty(Path[0]).GetValue(EditInstallationViewModel, null) != null)
                                        CheckId = (int)EditInstallationViewModel.GetType().GetProperty(Path[0])
                                            .GetValue(EditInstallationViewModel, null);
                                }

                                if (RecordsIds.Exists(x => x.ToString().ToLower() == CheckId.ToString().ToLower()))
                                {
                                    CheckIfSuccessAllRules++;
                                }
                            }
                        }

                        if (Rules.Count() == CheckIfSuccessAllRules)
                        {
                            string DynamicAttributeName = "";
                            int DynamicAttributeId = _unitOfWork.DependencyRowRepository
                                .GetIncludeWhereFirst(x => x.RowId == RowId, x => x.Dependency).Dependency.DynamicAttId.Value;

                            BaseInstAttView InputDynamicAttribute = EditInstallationViewModel.DynamicInstAttsValue
                                .FirstOrDefault(x => x.Id == DynamicAttributeId);

                            if (InputDynamicAttribute == null)
                            {
                                DynamicAttributeName = _unitOfWork.DynamicAttRepository
                                    .GetWhereFirst(x => x.Id == DynamicAttributeId).Key;

                                return $"({DynamicAttributeName}) value can't be null";
                            }
                            else
                            {
                                object DependencyValidationValue = new object();
                                object InputDynamicValue = new object();

                                if (DynamicAttributeMainDependency.ValueBoolean != null)
                                {
                                    DependencyValidationValue = DynamicAttributeMainDependency.ValueBoolean;
                                    InputDynamicValue = bool.Parse(InputDynamicAttribute.Value.ToString());
                                }
                                else if (DynamicAttributeMainDependency.ValueDateTime != null)
                                {
                                    DependencyValidationValue = DynamicAttributeMainDependency.ValueDateTime;
                                    InputDynamicValue = DateTime.Parse(InputDynamicAttribute.Value.ToString());
                                }
                                else if (DynamicAttributeMainDependency.ValueDouble != null)
                                {
                                    DependencyValidationValue = DynamicAttributeMainDependency.ValueDouble;
                                    InputDynamicValue = double.Parse(InputDynamicAttribute.Value.ToString());
                                }
                                else if (!string.IsNullOrEmpty(DynamicAttributeMainDependency.ValueString))
                                {
                                    DependencyValidationValue = DynamicAttributeMainDependency.ValueString;
                                    InputDynamicValue = InputDynamicAttribute.Value.ToString();
                                }

                                string DependencyValidationOperation = DynamicAttributeMainDependency.Operation.Name;

                                if (!(DependencyValidationOperation == "==" ? InputDynamicValue.ToString().ToLower() == DependencyValidationValue.ToString().ToLower() :
                                    DependencyValidationOperation == "!=" ? InputDynamicValue.ToString().ToLower() != DependencyValidationValue.ToString().ToLower() :
                                    DependencyValidationOperation == ">" ? Comparer.DefaultInvariant.Compare(InputDynamicValue, DependencyValidationValue) == 1 :
                                    DependencyValidationOperation == ">=" ? (Comparer.DefaultInvariant.Compare(InputDynamicValue, DependencyValidationValue) == 1 ||
                                        InputDynamicValue.ToString().ToLower() == DependencyValidationValue.ToString().ToLower()) :
                                    DependencyValidationOperation == "<" ? Comparer.DefaultInvariant.Compare(InputDynamicValue, DependencyValidationValue) == -1 :
                                    DependencyValidationOperation == "<=" ? (Comparer.DefaultInvariant.Compare(InputDynamicValue, DependencyValidationValue) == -1 ||
                                        InputDynamicValue.ToString().ToLower() == DependencyValidationValue.ToString().ToLower()) : false))
                                {
                                    DynamicAttributeName = _unitOfWork.DynamicAttRepository
                                        .GetWhereFirst(x => x.Id == DynamicAttributeId).Key;

                                    string ReturnOperation = (DependencyValidationOperation == "==" ? "equal to" :
                                        (DependencyValidationOperation == "!=" ? "not equal to" :
                                        (DependencyValidationOperation == ">" ? "bigger than" :
                                        (DependencyValidationOperation == ">=" ? "bigger than or equal to" :
                                        (DependencyValidationOperation == "<" ? "smaller than" :
                                        (DependencyValidationOperation == "<=" ? "smaller than or equal to" : ""))))));

                                    return $"({DynamicAttributeName}) value must be {ReturnOperation} {DependencyValidationValue}";
                                }
                            }
                        }
                    }
                }
            }
            else if (OtherInventoryType.ToLower() == TablesNames.TLIsolar.ToString().ToLower())
            {
                string MainTableName = TablesNames.TLIsolar.ToString();
                EditSolarViewModel EditInstallationViewModel = _mapper.Map<EditSolarViewModel>(Input);

                List<DynamicAttViewModel> DynamicAttributes = _mapper.Map<List<DynamicAttViewModel>>(_unitOfWork.DynamicAttRepository
                    .GetIncludeWhere(x => x.tablesNames.TableName.ToLower() == MainTableName.ToLower() && !x.disable
                        , x => x.tablesNames).ToList());

                foreach (DynamicAttViewModel DynamicAttribute in DynamicAttributes)
                {
                    TLIdependency DynamicAttributeMainDependency = _unitOfWork.DependencieRepository.GetIncludeWhereFirst(x => x.DynamicAttId == DynamicAttribute.Id &&
                        (x.ValueBoolean != null || x.ValueDateTime != null || x.ValueDouble != null || !string.IsNullOrEmpty(x.ValueString)) &&
                        x.OperationId != null, x => x.Operation);

                    if (DynamicAttributeMainDependency == null)
                        continue;

                    List<int> DependencyRows = _unitOfWork.DependencyRowRepository.GetWhere(x => x.DependencyId == DynamicAttributeMainDependency.Id)
                        .Select(x => x.RowId.Value).Distinct().ToList();

                    foreach (int RowId in DependencyRows)
                    {
                        List<TLIrule> Rules = _unitOfWork.RowRuleRepository.GetIncludeWhere(x => x.RowId == RowId, x => x.Rule, x => x.Rule.tablesNames,
                            x => x.Rule.Operation, x => x.Rule.dynamicAtt, x => x.Rule.attributeActivated).Select(x => x.Rule).ToList();

                        int CheckIfSuccessAllRules = 0;

                        foreach (TLIrule Rule in Rules)
                        {
                            string SDTableName = Rule.tablesNames.TableName;

                            string DataType = "";

                            string Operation = Rule.Operation.Name;
                            object OperationValue = new object();

                            if (Rule.OperationValueBoolean != null)
                            {
                                DataType = "Bool";
                                OperationValue = Rule.OperationValueBoolean;
                            }
                            else if (Rule.OperationValueDateTime != null)
                            {
                                DataType = "DateTime";
                                OperationValue = Rule.OperationValueDateTime;
                            }
                            else if (Rule.OperationValueDouble != null)
                            {
                                DataType = "Double";
                                OperationValue = Rule.OperationValueDouble;
                            }
                            else if (!string.IsNullOrEmpty(Rule.OperationValueString))
                            {
                                DataType = "String";
                                OperationValue = Rule.OperationValueString;
                            }

                            if (MainTableName.ToLower() == SDTableName.ToLower())
                            {
                                object InsertedValue = new object();

                                if (Rule.attributeActivatedId != null)
                                {
                                    string AttributeName = Rule.attributeActivated.Key;

                                    object TestValue = EditInstallationViewModel.GetType().GetProperties()
                                        .FirstOrDefault(x => x.Name.ToLower() == AttributeName.ToLower()).GetValue(EditInstallationViewModel, null);

                                    if (TestValue == null)
                                        break;

                                    if (Rule.OperationValueBoolean != null)
                                        InsertedValue = bool.Parse(TestValue.ToString());

                                    else if (Rule.OperationValueDateTime != null)
                                        InsertedValue = DateTime.Parse(TestValue.ToString());

                                    else if (Rule.OperationValueDouble != null)
                                        InsertedValue = double.Parse(TestValue.ToString());

                                    else if (!string.IsNullOrEmpty(Rule.OperationValueString))
                                        InsertedValue = TestValue.ToString();
                                }
                                else if (Rule.dynamicAttId != null)
                                {
                                    BaseInstAttView DynamicObject = EditInstallationViewModel.DynamicInstAttsValue
                                        .FirstOrDefault(x => x.Id == Rule.dynamicAttId.Value);

                                    if (DynamicObject == null)
                                        break;

                                    string DynamicAttributeDataType = _unitOfWork.DataTypeRepository.GetByID(DynamicObject.DataTypeId.Value).Name;

                                    if (DynamicAttributeDataType.ToLower() == "boolean".ToLower())
                                        InsertedValue = bool.Parse(DynamicObject.Value.ToString());

                                    else if (DynamicAttributeDataType.ToLower() == "string".ToLower())
                                        InsertedValue = DynamicObject.Value.ToString();

                                    else if (DynamicAttributeDataType.ToLower() == "int".ToLower() || DynamicAttributeDataType.ToLower() == "double".ToLower())
                                        InsertedValue = double.Parse(DynamicObject.Value.ToString());

                                    else if (DynamicAttributeDataType.ToLower() == "datetime".ToLower())
                                        InsertedValue = DateTime.Parse(DynamicObject.Value.ToString());
                                }

                                if (Operation == "==" ? InsertedValue.ToString().ToLower() == OperationValue.ToString().ToLower() :
                                    Operation == "!=" ? InsertedValue.ToString().ToLower() != OperationValue.ToString().ToLower() :
                                    Operation == ">" ? Comparer.DefaultInvariant.Compare(InsertedValue, OperationValue) == 1 :
                                    Operation == ">=" ? (Comparer.DefaultInvariant.Compare(InsertedValue, OperationValue) == 1 ||
                                        InsertedValue.ToString().ToLower() == OperationValue.ToString().ToLower()) :
                                    Operation == "<" ? Comparer.DefaultInvariant.Compare(InsertedValue, OperationValue) == -1 :
                                    Operation == "<=" ? (Comparer.DefaultInvariant.Compare(InsertedValue, OperationValue) == -1 ||
                                        InsertedValue.ToString().ToLower() == OperationValue.ToString().ToLower()) : false)
                                {
                                    CheckIfSuccessAllRules++;
                                }
                            }
                            else
                            {
                                List<object> TableRecords = new List<object>();
                                if (Rule.attributeActivatedId != null)
                                {
                                    string AttributeName = Rule.attributeActivated.Key;

                                    if (OperationValue != null)
                                        TableRecords = _mapper.Map<List<object>>(_dbContext.GetType().GetProperty(SDTableName)
                                            .GetValue(_dbContext, null)).Where(x => x.GetType().GetProperty(AttributeName).GetValue(x, null) != null ? (Operation == ">" ?
                                                (DataType.ToLower() == "DateTime".ToLower() ?
                                                    Comparer.DefaultInvariant.Compare(DateTime.Parse(x.GetType().GetProperty(AttributeName).GetValue(x, null).ToString()), OperationValue) == 1 :
                                                DataType.ToLower() == "Double".ToLower() ?
                                                    Comparer.DefaultInvariant.Compare(double.Parse(x.GetType().GetProperty(AttributeName).GetValue(x, null).ToString()), OperationValue) == 1 : false) :
                                            Operation == ">=" ?
                                                (DataType.ToLower() == "DateTime".ToLower() ?
                                                    (Comparer.DefaultInvariant.Compare(DateTime.Parse(x.GetType().GetProperty(AttributeName).GetValue(x, null).ToString()), OperationValue) == 1 ||
                                                        x.GetType().GetProperty(AttributeName).GetValue(x, null).ToString().ToLower() == OperationValue.ToString().ToLower()) :
                                                DataType.ToLower() == "Double".ToLower() ?
                                                    (Comparer.DefaultInvariant.Compare(double.Parse(x.GetType().GetProperty(AttributeName).GetValue(x, null).ToString()), OperationValue) == 1 ||
                                                        x.GetType().GetProperty(AttributeName).GetValue(x, null).ToString().ToLower() == OperationValue.ToString().ToLower()) : false) :
                                            Operation == "<" ?
                                                (DataType.ToLower() == "DateTime".ToLower() ?
                                                    Comparer.DefaultInvariant.Compare(DateTime.Parse(x.GetType().GetProperty(AttributeName).GetValue(x, null).ToString()), OperationValue) == -1 :
                                                DataType.ToLower() == "Double".ToLower() ?
                                                    Comparer.DefaultInvariant.Compare(double.Parse(x.GetType().GetProperty(AttributeName).GetValue(x, null).ToString()), OperationValue) == -1 : false) :
                                            Operation == "<=" ?
                                                (DataType.ToLower() == "DateTime".ToLower() ?
                                                    (Comparer.DefaultInvariant.Compare(DateTime.Parse(x.GetType().GetProperty(AttributeName).GetValue(x, null).ToString()), OperationValue) == -1 ||
                                                        x.GetType().GetProperty(AttributeName).GetValue(x, null).ToString().ToLower() == OperationValue.ToString().ToLower()) :
                                                DataType.ToLower() == "Double".ToLower() ?
                                                    (Comparer.DefaultInvariant.Compare(double.Parse(x.GetType().GetProperty(AttributeName).GetValue(x, null).ToString()), OperationValue) == -1 ||
                                                        x.GetType().GetProperty(AttributeName).GetValue(x, null).ToString().ToLower() == OperationValue.ToString().ToLower()) : false) :
                                            Operation == "==" ?
                                                x.GetType().GetProperty(AttributeName).GetValue(x, null).ToString().ToLower() == OperationValue.ToString().ToLower() :
                                            Operation == "!=" ?
                                                x.GetType().GetProperty(AttributeName).GetValue(x, null).ToString().ToLower() != OperationValue.ToString().ToLower() : false) : false).ToList();

                                }
                                else if (Rule.dynamicAttId != null)
                                {
                                    List<int> DynamicAttValuesInventoryIds = new List<int>();

                                    if (!DynamicAttribute.LibraryAtt)
                                    {
                                        DynamicAttValuesInventoryIds = _unitOfWork.DynamicAttInstValueRepository
                                            .GetWhere(x => (x.DynamicAttId == Rule.dynamicAttId.Value && !x.disable) &&
                                                (Operation == "==" ?
                                                    ((Rule.OperationValueBoolean != null ? x.ValueBoolean.ToString().ToLower() == Rule.OperationValueBoolean.ToString().ToLower() : false) ||
                                                    (Rule.OperationValueDateTime != null ? x.ValueDateTime.ToString().ToLower() == Rule.OperationValueDateTime.ToString().ToLower() : false) ||
                                                    (Rule.OperationValueDouble != null ? x.ValueDouble == Rule.OperationValueDouble : false) ||
                                                    (!string.IsNullOrEmpty(Rule.OperationValueString) ? x.ValueString.ToLower() == Rule.OperationValueString.ToLower() : false)) : false) ||

                                                (Operation == "!=" ?
                                                    ((Rule.OperationValueBoolean != null ? x.ValueBoolean.ToString().ToLower() != Rule.OperationValueBoolean.ToString().ToLower() : false) ||
                                                    (Rule.OperationValueDateTime != null ? x.ValueDateTime.ToString().ToLower() != Rule.OperationValueDateTime.ToString().ToLower() : false) ||
                                                    (Rule.OperationValueDouble != null ? x.ValueDouble != Rule.OperationValueDouble : false) ||
                                                    (!string.IsNullOrEmpty(Rule.OperationValueString) ? x.ValueString.ToLower() != Rule.OperationValueString.ToLower() : false)) : false) ||

                                                (Operation == ">" ?
                                                    ((Rule.OperationValueDateTime != null ? x.ValueDateTime > Rule.OperationValueDateTime : false) ||
                                                    (Rule.OperationValueDouble != null ? x.ValueDouble > Rule.OperationValueDouble : false)) : false) ||

                                                (Operation == ">=" ?
                                                    ((Rule.OperationValueDateTime != null ? x.ValueDateTime >= Rule.OperationValueDateTime : false) ||
                                                    (Rule.OperationValueDouble != null ? x.ValueDouble >= Rule.OperationValueDouble : false)) : false) ||

                                                (Operation == "<" ?
                                                    ((Rule.OperationValueDateTime != null ? x.ValueDateTime < Rule.OperationValueDateTime : false) ||
                                                    (Rule.OperationValueDouble != null ? x.ValueDouble < Rule.OperationValueDouble : false)) : false) ||

                                                (Operation == "<=" ?
                                                    ((Rule.OperationValueDateTime != null ? x.ValueDateTime <= Rule.OperationValueDateTime : false) ||
                                                    (Rule.OperationValueDouble != null ? x.ValueDouble <= Rule.OperationValueDouble : false)) : false)

                                                ).Select(x => x.InventoryId).ToList();
                                    }
                                    else
                                    {
                                        DynamicAttValuesInventoryIds = _unitOfWork.DynamicAttLibRepository
                                            .GetWhere(x => (x.DynamicAttId == Rule.dynamicAttId && !x.disable) &&
                                                (Operation == "==" ?
                                                    ((Rule.OperationValueBoolean != null ? x.ValueBoolean.ToString().ToLower() == Rule.OperationValueBoolean.ToString().ToLower() : false) ||
                                                    (Rule.OperationValueDateTime != null ? x.ValueDateTime.ToString().ToLower() == Rule.OperationValueDateTime.ToString().ToLower() : false) ||
                                                    (Rule.OperationValueDouble != null ? x.ValueDouble == Rule.OperationValueDouble : false) ||
                                                    (!string.IsNullOrEmpty(Rule.OperationValueString) ? x.ValueString.ToLower() == Rule.OperationValueString.ToLower() : false)) : false) ||

                                                (Operation == "!=" ?
                                                    ((Rule.OperationValueBoolean != null ? x.ValueBoolean.ToString().ToLower() != Rule.OperationValueBoolean.ToString().ToLower() : false) ||
                                                    (Rule.OperationValueDateTime != null ? x.ValueDateTime.ToString().ToLower() != Rule.OperationValueDateTime.ToString().ToLower() : false) ||
                                                    (Rule.OperationValueDouble != null ? x.ValueDouble != Rule.OperationValueDouble : false) ||
                                                    (!string.IsNullOrEmpty(Rule.OperationValueString) ? x.ValueString.ToLower() != Rule.OperationValueString.ToLower() : false)) : false) ||

                                                (Operation == ">" ?
                                                    ((Rule.OperationValueDateTime != null ? x.ValueDateTime > Rule.OperationValueDateTime : false) ||
                                                    (Rule.OperationValueDouble != null ? x.ValueDouble > Rule.OperationValueDouble : false)) : false) ||

                                                (Operation == ">=" ?
                                                    ((Rule.OperationValueDateTime != null ? x.ValueDateTime >= Rule.OperationValueDateTime : false) ||
                                                    (Rule.OperationValueDouble != null ? x.ValueDouble >= Rule.OperationValueDouble : false)) : false) ||

                                                (Operation == "<" ?
                                                    ((Rule.OperationValueDateTime != null ? x.ValueDateTime < Rule.OperationValueDateTime : false) ||
                                                    (Rule.OperationValueDouble != null ? x.ValueDouble < Rule.OperationValueDouble : false)) : false) ||

                                                (Operation == "<=" ?
                                                    ((Rule.OperationValueDateTime != null ? x.ValueDateTime <= Rule.OperationValueDateTime : false) ||
                                                    (Rule.OperationValueDouble != null ? x.ValueDouble <= Rule.OperationValueDouble : false)) : false)

                                                ).Select(x => x.InventoryId).ToList();
                                    }
                                    if (DynamicAttValuesInventoryIds != null ? DynamicAttValuesInventoryIds.Count() != 0 : false)
                                    {
                                        TableRecords = _mapper.Map<List<object>>(_dbContext.GetType()
                                            .GetProperty(SDTableName).GetValue(_dbContext, null))
                                                .Where(x => DynamicAttValuesInventoryIds.Contains(Convert.ToInt32(x.GetType().GetProperty("Id").GetValue(x, null)))).ToList();
                                    }
                                }

                                AddInstRuleViewModel AddInstRuleViewModel = new AddInstRuleViewModel();
                                if (Rule.dynamicAttId != null)
                                {
                                    AddInstRuleViewModel = new AddInstRuleViewModel
                                    {
                                        dynamicAttId = Rule.dynamicAttId,
                                        IsDynamic = true,
                                        OperationId = Rule.OperationId,
                                        OperationValueBoolean = Rule.OperationValueBoolean,
                                        OperationValueDateTime = Rule.OperationValueDateTime,
                                        OperationValueDouble = Rule.OperationValueDouble,
                                        OperationValueString = Rule.OperationValueString,
                                        TableName = Rule.tablesNames.TableName
                                    };
                                }
                                else if (Rule.attributeActivatedId != null)
                                {
                                    AddInstRuleViewModel = new AddInstRuleViewModel
                                    {
                                        attributeActivatedId = Rule.attributeActivatedId,
                                        IsDynamic = false,
                                        OperationId = Rule.OperationId,
                                        OperationValueBoolean = Rule.OperationValueBoolean,
                                        OperationValueDateTime = Rule.OperationValueDateTime,
                                        OperationValueDouble = Rule.OperationValueDouble,
                                        OperationValueString = Rule.OperationValueString,
                                        TableName = Rule.tablesNames.TableName
                                    };
                                }
                                List<object> RecordsIds = _mapper.Map<List<object>>(GetRecordsIds(MainTableName, AddInstRuleViewModel));

                                PathToCheckDependencyValidation Item = (PathToCheckDependencyValidation)Enum.Parse(typeof(PathToCheckDependencyValidation),
                                    MainTableName + SDTableName + "Goal");

                                List<string> Path = GetEnumDescription(Item).Split(" ").ToList();

                                object CheckId = new object();

                                if (Path.Count() > 1)
                                {
                                    object CivilLoads = EditInstallationViewModel.GetType().GetProperty(Path[0])
                                        .GetValue(EditInstallationViewModel, null);

                                    CheckId = CivilLoads.GetType().GetProperty(Path[1]).GetValue(CivilLoads, null) != null ?
                                        (int)CivilLoads.GetType().GetProperty(Path[1]).GetValue(CivilLoads, null) : new object();
                                }
                                else if (Path.Count() == 1 && Path[0].ToLower() == "sitecode")
                                {
                                    CheckId = SiteCode;
                                }
                                else if (Path.Count() == 1)
                                {
                                    if (EditInstallationViewModel.GetType().GetProperty(Path[0]).GetValue(EditInstallationViewModel, null) != null)
                                        CheckId = (int)EditInstallationViewModel.GetType().GetProperty(Path[0])
                                            .GetValue(EditInstallationViewModel, null);
                                }

                                if (RecordsIds.Exists(x => x.ToString().ToLower() == CheckId.ToString().ToLower()))
                                {
                                    CheckIfSuccessAllRules++;
                                }
                            }
                        }

                        if (Rules.Count() == CheckIfSuccessAllRules)
                        {
                            string DynamicAttributeName = "";
                            int DynamicAttributeId = _unitOfWork.DependencyRowRepository
                                .GetIncludeWhereFirst(x => x.RowId == RowId, x => x.Dependency).Dependency.DynamicAttId.Value;

                            BaseInstAttView InputDynamicAttribute = EditInstallationViewModel.DynamicInstAttsValue
                                .FirstOrDefault(x => x.Id == DynamicAttributeId);

                            if (InputDynamicAttribute == null)
                            {
                                DynamicAttributeName = _unitOfWork.DynamicAttRepository
                                    .GetWhereFirst(x => x.Id == DynamicAttributeId).Key;

                                return $"({DynamicAttributeName}) value can't be null";
                            }
                            else
                            {
                                object DependencyValidationValue = new object();
                                object InputDynamicValue = new object();

                                if (DynamicAttributeMainDependency.ValueBoolean != null)
                                {
                                    DependencyValidationValue = DynamicAttributeMainDependency.ValueBoolean;
                                    InputDynamicValue = bool.Parse(InputDynamicAttribute.Value.ToString());
                                }
                                else if (DynamicAttributeMainDependency.ValueDateTime != null)
                                {
                                    DependencyValidationValue = DynamicAttributeMainDependency.ValueDateTime;
                                    InputDynamicValue = DateTime.Parse(InputDynamicAttribute.Value.ToString());
                                }
                                else if (DynamicAttributeMainDependency.ValueDouble != null)
                                {
                                    DependencyValidationValue = DynamicAttributeMainDependency.ValueDouble;
                                    InputDynamicValue = double.Parse(InputDynamicAttribute.Value.ToString());
                                }
                                else if (!string.IsNullOrEmpty(DynamicAttributeMainDependency.ValueString))
                                {
                                    DependencyValidationValue = DynamicAttributeMainDependency.ValueString;
                                    InputDynamicValue = InputDynamicAttribute.Value.ToString();
                                }

                                string DependencyValidationOperation = DynamicAttributeMainDependency.Operation.Name;

                                if (!(DependencyValidationOperation == "==" ? InputDynamicValue.ToString().ToLower() == DependencyValidationValue.ToString().ToLower() :
                                    DependencyValidationOperation == "!=" ? InputDynamicValue.ToString().ToLower() != DependencyValidationValue.ToString().ToLower() :
                                    DependencyValidationOperation == ">" ? Comparer.DefaultInvariant.Compare(InputDynamicValue, DependencyValidationValue) == 1 :
                                    DependencyValidationOperation == ">=" ? (Comparer.DefaultInvariant.Compare(InputDynamicValue, DependencyValidationValue) == 1 ||
                                        InputDynamicValue.ToString().ToLower() == DependencyValidationValue.ToString().ToLower()) :
                                    DependencyValidationOperation == "<" ? Comparer.DefaultInvariant.Compare(InputDynamicValue, DependencyValidationValue) == -1 :
                                    DependencyValidationOperation == "<=" ? (Comparer.DefaultInvariant.Compare(InputDynamicValue, DependencyValidationValue) == -1 ||
                                        InputDynamicValue.ToString().ToLower() == DependencyValidationValue.ToString().ToLower()) : false))
                                {
                                    DynamicAttributeName = _unitOfWork.DynamicAttRepository
                                        .GetWhereFirst(x => x.Id == DynamicAttributeId).Key;

                                    string ReturnOperation = (DependencyValidationOperation == "==" ? "equal to" :
                                        (DependencyValidationOperation == "!=" ? "not equal to" :
                                        (DependencyValidationOperation == ">" ? "bigger than" :
                                        (DependencyValidationOperation == ">=" ? "bigger than or equal to" :
                                        (DependencyValidationOperation == "<" ? "smaller than" :
                                        (DependencyValidationOperation == "<=" ? "smaller than or equal to" : ""))))));

                                    return $"({DynamicAttributeName}) value must be {ReturnOperation} {DependencyValidationValue}";
                                }
                            }
                        }
                    }
                }
            }
            else if (OtherInventoryType.ToLower() == TablesNames.TLIgenerator.ToString().ToLower())
            {
                string MainTableName = TablesNames.TLIgenerator.ToString();
                EditGeneratorViewModel EditInstallationViewModel = _mapper.Map<EditGeneratorViewModel>(Input);

                List<DynamicAttViewModel> DynamicAttributes = _mapper.Map<List<DynamicAttViewModel>>(_unitOfWork.DynamicAttRepository
                    .GetIncludeWhere(x => x.tablesNames.TableName.ToLower() == MainTableName.ToLower() && !x.disable
                        , x => x.tablesNames).ToList());

                foreach (DynamicAttViewModel DynamicAttribute in DynamicAttributes)
                {
                    TLIdependency DynamicAttributeMainDependency = _unitOfWork.DependencieRepository.GetIncludeWhereFirst(x => x.DynamicAttId == DynamicAttribute.Id &&
                        (x.ValueBoolean != null || x.ValueDateTime != null || x.ValueDouble != null || !string.IsNullOrEmpty(x.ValueString)) &&
                        x.OperationId != null, x => x.Operation);

                    if (DynamicAttributeMainDependency == null)
                        continue;

                    List<int> DependencyRows = _unitOfWork.DependencyRowRepository.GetWhere(x => x.DependencyId == DynamicAttributeMainDependency.Id)
                        .Select(x => x.RowId.Value).Distinct().ToList();

                    foreach (int RowId in DependencyRows)
                    {
                        List<TLIrule> Rules = _unitOfWork.RowRuleRepository.GetIncludeWhere(x => x.RowId == RowId, x => x.Rule, x => x.Rule.tablesNames,
                            x => x.Rule.Operation, x => x.Rule.dynamicAtt, x => x.Rule.attributeActivated).Select(x => x.Rule).ToList();

                        int CheckIfSuccessAllRules = 0;

                        foreach (TLIrule Rule in Rules)
                        {
                            string SDTableName = Rule.tablesNames.TableName;

                            string DataType = "";

                            string Operation = Rule.Operation.Name;
                            object OperationValue = new object();

                            if (Rule.OperationValueBoolean != null)
                            {
                                DataType = "Bool";
                                OperationValue = Rule.OperationValueBoolean;
                            }
                            else if (Rule.OperationValueDateTime != null)
                            {
                                DataType = "DateTime";
                                OperationValue = Rule.OperationValueDateTime;
                            }
                            else if (Rule.OperationValueDouble != null)
                            {
                                DataType = "Double";
                                OperationValue = Rule.OperationValueDouble;
                            }
                            else if (!string.IsNullOrEmpty(Rule.OperationValueString))
                            {
                                DataType = "String";
                                OperationValue = Rule.OperationValueString;
                            }

                            if (MainTableName.ToLower() == SDTableName.ToLower())
                            {
                                object InsertedValue = new object();

                                if (Rule.attributeActivatedId != null)
                                {
                                    string AttributeName = Rule.attributeActivated.Key;

                                    object TestValue = EditInstallationViewModel.GetType().GetProperties()
                                        .FirstOrDefault(x => x.Name.ToLower() == AttributeName.ToLower()).GetValue(EditInstallationViewModel, null);

                                    if (TestValue == null)
                                        break;

                                    if (Rule.OperationValueBoolean != null)
                                        InsertedValue = bool.Parse(TestValue.ToString());

                                    else if (Rule.OperationValueDateTime != null)
                                        InsertedValue = DateTime.Parse(TestValue.ToString());

                                    else if (Rule.OperationValueDouble != null)
                                        InsertedValue = double.Parse(TestValue.ToString());

                                    else if (!string.IsNullOrEmpty(Rule.OperationValueString))
                                        InsertedValue = TestValue.ToString();
                                }
                                else if (Rule.dynamicAttId != null)
                                {
                                    BaseInstAttView DynamicObject = EditInstallationViewModel.DynamicInstAttsValue
                                        .FirstOrDefault(x => x.Id == Rule.dynamicAttId.Value);

                                    if (DynamicObject == null)
                                        break;

                                    string DynamicAttributeDataType = _unitOfWork.DataTypeRepository.GetByID(DynamicObject.DataTypeId.Value).Name;

                                    if (DynamicAttributeDataType.ToLower() == "boolean".ToLower())
                                        InsertedValue = bool.Parse(DynamicObject.Value.ToString());

                                    else if (DynamicAttributeDataType.ToLower() == "string".ToLower())
                                        InsertedValue = DynamicObject.Value.ToString();

                                    else if (DynamicAttributeDataType.ToLower() == "int".ToLower() || DynamicAttributeDataType.ToLower() == "double".ToLower())
                                        InsertedValue = double.Parse(DynamicObject.Value.ToString());

                                    else if (DynamicAttributeDataType.ToLower() == "datetime".ToLower())
                                        InsertedValue = DateTime.Parse(DynamicObject.Value.ToString());
                                }

                                if (Operation == "==" ? InsertedValue.ToString().ToLower() == OperationValue.ToString().ToLower() :
                                    Operation == "!=" ? InsertedValue.ToString().ToLower() != OperationValue.ToString().ToLower() :
                                    Operation == ">" ? Comparer.DefaultInvariant.Compare(InsertedValue, OperationValue) == 1 :
                                    Operation == ">=" ? (Comparer.DefaultInvariant.Compare(InsertedValue, OperationValue) == 1 ||
                                        InsertedValue.ToString().ToLower() == OperationValue.ToString().ToLower()) :
                                    Operation == "<" ? Comparer.DefaultInvariant.Compare(InsertedValue, OperationValue) == -1 :
                                    Operation == "<=" ? (Comparer.DefaultInvariant.Compare(InsertedValue, OperationValue) == -1 ||
                                        InsertedValue.ToString().ToLower() == OperationValue.ToString().ToLower()) : false)
                                {
                                    CheckIfSuccessAllRules++;
                                }
                            }
                            else
                            {
                                List<object> TableRecords = new List<object>();
                                if (Rule.attributeActivatedId != null)
                                {
                                    string AttributeName = Rule.attributeActivated.Key;

                                    if (OperationValue != null)
                                        TableRecords = _mapper.Map<List<object>>(_dbContext.GetType().GetProperty(SDTableName)
                                            .GetValue(_dbContext, null)).Where(x => x.GetType().GetProperty(AttributeName).GetValue(x, null) != null ? (Operation == ">" ?
                                                (DataType.ToLower() == "DateTime".ToLower() ?
                                                    Comparer.DefaultInvariant.Compare(DateTime.Parse(x.GetType().GetProperty(AttributeName).GetValue(x, null).ToString()), OperationValue) == 1 :
                                                DataType.ToLower() == "Double".ToLower() ?
                                                    Comparer.DefaultInvariant.Compare(double.Parse(x.GetType().GetProperty(AttributeName).GetValue(x, null).ToString()), OperationValue) == 1 : false) :
                                            Operation == ">=" ?
                                                (DataType.ToLower() == "DateTime".ToLower() ?
                                                    (Comparer.DefaultInvariant.Compare(DateTime.Parse(x.GetType().GetProperty(AttributeName).GetValue(x, null).ToString()), OperationValue) == 1 ||
                                                        x.GetType().GetProperty(AttributeName).GetValue(x, null).ToString().ToLower() == OperationValue.ToString().ToLower()) :
                                                DataType.ToLower() == "Double".ToLower() ?
                                                    (Comparer.DefaultInvariant.Compare(double.Parse(x.GetType().GetProperty(AttributeName).GetValue(x, null).ToString()), OperationValue) == 1 ||
                                                        x.GetType().GetProperty(AttributeName).GetValue(x, null).ToString().ToLower() == OperationValue.ToString().ToLower()) : false) :
                                            Operation == "<" ?
                                                (DataType.ToLower() == "DateTime".ToLower() ?
                                                    Comparer.DefaultInvariant.Compare(DateTime.Parse(x.GetType().GetProperty(AttributeName).GetValue(x, null).ToString()), OperationValue) == -1 :
                                                DataType.ToLower() == "Double".ToLower() ?
                                                    Comparer.DefaultInvariant.Compare(double.Parse(x.GetType().GetProperty(AttributeName).GetValue(x, null).ToString()), OperationValue) == -1 : false) :
                                            Operation == "<=" ?
                                                (DataType.ToLower() == "DateTime".ToLower() ?
                                                    (Comparer.DefaultInvariant.Compare(DateTime.Parse(x.GetType().GetProperty(AttributeName).GetValue(x, null).ToString()), OperationValue) == -1 ||
                                                        x.GetType().GetProperty(AttributeName).GetValue(x, null).ToString().ToLower() == OperationValue.ToString().ToLower()) :
                                                DataType.ToLower() == "Double".ToLower() ?
                                                    (Comparer.DefaultInvariant.Compare(double.Parse(x.GetType().GetProperty(AttributeName).GetValue(x, null).ToString()), OperationValue) == -1 ||
                                                        x.GetType().GetProperty(AttributeName).GetValue(x, null).ToString().ToLower() == OperationValue.ToString().ToLower()) : false) :
                                            Operation == "==" ?
                                                x.GetType().GetProperty(AttributeName).GetValue(x, null).ToString().ToLower() == OperationValue.ToString().ToLower() :
                                            Operation == "!=" ?
                                                x.GetType().GetProperty(AttributeName).GetValue(x, null).ToString().ToLower() != OperationValue.ToString().ToLower() : false) : false).ToList();

                                }
                                else if (Rule.dynamicAttId != null)
                                {
                                    List<int> DynamicAttValuesInventoryIds = new List<int>();

                                    if (!DynamicAttribute.LibraryAtt)
                                    {
                                        DynamicAttValuesInventoryIds = _unitOfWork.DynamicAttInstValueRepository
                                            .GetWhere(x => (x.DynamicAttId == Rule.dynamicAttId.Value && !x.disable) &&
                                                (Operation == "==" ?
                                                    ((Rule.OperationValueBoolean != null ? x.ValueBoolean.ToString().ToLower() == Rule.OperationValueBoolean.ToString().ToLower() : false) ||
                                                    (Rule.OperationValueDateTime != null ? x.ValueDateTime.ToString().ToLower() == Rule.OperationValueDateTime.ToString().ToLower() : false) ||
                                                    (Rule.OperationValueDouble != null ? x.ValueDouble == Rule.OperationValueDouble : false) ||
                                                    (!string.IsNullOrEmpty(Rule.OperationValueString) ? x.ValueString.ToLower() == Rule.OperationValueString.ToLower() : false)) : false) ||

                                                (Operation == "!=" ?
                                                    ((Rule.OperationValueBoolean != null ? x.ValueBoolean.ToString().ToLower() != Rule.OperationValueBoolean.ToString().ToLower() : false) ||
                                                    (Rule.OperationValueDateTime != null ? x.ValueDateTime.ToString().ToLower() != Rule.OperationValueDateTime.ToString().ToLower() : false) ||
                                                    (Rule.OperationValueDouble != null ? x.ValueDouble != Rule.OperationValueDouble : false) ||
                                                    (!string.IsNullOrEmpty(Rule.OperationValueString) ? x.ValueString.ToLower() != Rule.OperationValueString.ToLower() : false)) : false) ||

                                                (Operation == ">" ?
                                                    ((Rule.OperationValueDateTime != null ? x.ValueDateTime > Rule.OperationValueDateTime : false) ||
                                                    (Rule.OperationValueDouble != null ? x.ValueDouble > Rule.OperationValueDouble : false)) : false) ||

                                                (Operation == ">=" ?
                                                    ((Rule.OperationValueDateTime != null ? x.ValueDateTime >= Rule.OperationValueDateTime : false) ||
                                                    (Rule.OperationValueDouble != null ? x.ValueDouble >= Rule.OperationValueDouble : false)) : false) ||

                                                (Operation == "<" ?
                                                    ((Rule.OperationValueDateTime != null ? x.ValueDateTime < Rule.OperationValueDateTime : false) ||
                                                    (Rule.OperationValueDouble != null ? x.ValueDouble < Rule.OperationValueDouble : false)) : false) ||

                                                (Operation == "<=" ?
                                                    ((Rule.OperationValueDateTime != null ? x.ValueDateTime <= Rule.OperationValueDateTime : false) ||
                                                    (Rule.OperationValueDouble != null ? x.ValueDouble <= Rule.OperationValueDouble : false)) : false)

                                                ).Select(x => x.InventoryId).ToList();
                                    }
                                    else
                                    {
                                        DynamicAttValuesInventoryIds = _unitOfWork.DynamicAttLibRepository
                                            .GetWhere(x => (x.DynamicAttId == Rule.dynamicAttId && !x.disable) &&
                                                (Operation == "==" ?
                                                    ((Rule.OperationValueBoolean != null ? x.ValueBoolean.ToString().ToLower() == Rule.OperationValueBoolean.ToString().ToLower() : false) ||
                                                    (Rule.OperationValueDateTime != null ? x.ValueDateTime.ToString().ToLower() == Rule.OperationValueDateTime.ToString().ToLower() : false) ||
                                                    (Rule.OperationValueDouble != null ? x.ValueDouble == Rule.OperationValueDouble : false) ||
                                                    (!string.IsNullOrEmpty(Rule.OperationValueString) ? x.ValueString.ToLower() == Rule.OperationValueString.ToLower() : false)) : false) ||

                                                (Operation == "!=" ?
                                                    ((Rule.OperationValueBoolean != null ? x.ValueBoolean.ToString().ToLower() != Rule.OperationValueBoolean.ToString().ToLower() : false) ||
                                                    (Rule.OperationValueDateTime != null ? x.ValueDateTime.ToString().ToLower() != Rule.OperationValueDateTime.ToString().ToLower() : false) ||
                                                    (Rule.OperationValueDouble != null ? x.ValueDouble != Rule.OperationValueDouble : false) ||
                                                    (!string.IsNullOrEmpty(Rule.OperationValueString) ? x.ValueString.ToLower() != Rule.OperationValueString.ToLower() : false)) : false) ||

                                                (Operation == ">" ?
                                                    ((Rule.OperationValueDateTime != null ? x.ValueDateTime > Rule.OperationValueDateTime : false) ||
                                                    (Rule.OperationValueDouble != null ? x.ValueDouble > Rule.OperationValueDouble : false)) : false) ||

                                                (Operation == ">=" ?
                                                    ((Rule.OperationValueDateTime != null ? x.ValueDateTime >= Rule.OperationValueDateTime : false) ||
                                                    (Rule.OperationValueDouble != null ? x.ValueDouble >= Rule.OperationValueDouble : false)) : false) ||

                                                (Operation == "<" ?
                                                    ((Rule.OperationValueDateTime != null ? x.ValueDateTime < Rule.OperationValueDateTime : false) ||
                                                    (Rule.OperationValueDouble != null ? x.ValueDouble < Rule.OperationValueDouble : false)) : false) ||

                                                (Operation == "<=" ?
                                                    ((Rule.OperationValueDateTime != null ? x.ValueDateTime <= Rule.OperationValueDateTime : false) ||
                                                    (Rule.OperationValueDouble != null ? x.ValueDouble <= Rule.OperationValueDouble : false)) : false)

                                                ).Select(x => x.InventoryId).ToList();
                                    }
                                    if (DynamicAttValuesInventoryIds != null ? DynamicAttValuesInventoryIds.Count() != 0 : false)
                                    {
                                        TableRecords = _mapper.Map<List<object>>(_dbContext.GetType()
                                            .GetProperty(SDTableName).GetValue(_dbContext, null))
                                                .Where(x => DynamicAttValuesInventoryIds.Contains(Convert.ToInt32(x.GetType().GetProperty("Id").GetValue(x, null)))).ToList();
                                    }
                                }

                                AddInstRuleViewModel AddInstRuleViewModel = new AddInstRuleViewModel();
                                if (Rule.dynamicAttId != null)
                                {
                                    AddInstRuleViewModel = new AddInstRuleViewModel
                                    {
                                        dynamicAttId = Rule.dynamicAttId,
                                        IsDynamic = true,
                                        OperationId = Rule.OperationId,
                                        OperationValueBoolean = Rule.OperationValueBoolean,
                                        OperationValueDateTime = Rule.OperationValueDateTime,
                                        OperationValueDouble = Rule.OperationValueDouble,
                                        OperationValueString = Rule.OperationValueString,
                                        TableName = Rule.tablesNames.TableName
                                    };
                                }
                                else if (Rule.attributeActivatedId != null)
                                {
                                    AddInstRuleViewModel = new AddInstRuleViewModel
                                    {
                                        attributeActivatedId = Rule.attributeActivatedId,
                                        IsDynamic = false,
                                        OperationId = Rule.OperationId,
                                        OperationValueBoolean = Rule.OperationValueBoolean,
                                        OperationValueDateTime = Rule.OperationValueDateTime,
                                        OperationValueDouble = Rule.OperationValueDouble,
                                        OperationValueString = Rule.OperationValueString,
                                        TableName = Rule.tablesNames.TableName
                                    };
                                }
                                List<object> RecordsIds = _mapper.Map<List<object>>(GetRecordsIds(MainTableName, AddInstRuleViewModel));

                                PathToCheckDependencyValidation Item = (PathToCheckDependencyValidation)Enum.Parse(typeof(PathToCheckDependencyValidation),
                                    MainTableName + SDTableName + "Goal");

                                List<string> Path = GetEnumDescription(Item).Split(" ").ToList();

                                object CheckId = new object();

                                if (Path.Count() > 1)
                                {
                                    object CivilLoads = EditInstallationViewModel.GetType().GetProperty(Path[0])
                                        .GetValue(EditInstallationViewModel, null);

                                    CheckId = CivilLoads.GetType().GetProperty(Path[1]).GetValue(CivilLoads, null) != null ?
                                        (int)CivilLoads.GetType().GetProperty(Path[1]).GetValue(CivilLoads, null) : new object();
                                }
                                else if (Path.Count() == 1 && Path[0].ToLower() == "sitecode")
                                {
                                    CheckId = SiteCode;
                                }
                                else if (Path.Count() == 1)
                                {
                                    if (EditInstallationViewModel.GetType().GetProperty(Path[0]).GetValue(EditInstallationViewModel, null) != null)
                                        CheckId = (int)EditInstallationViewModel.GetType().GetProperty(Path[0])
                                            .GetValue(EditInstallationViewModel, null);
                                }

                                if (RecordsIds.Exists(x => x.ToString().ToLower() == CheckId.ToString().ToLower()))
                                {
                                    CheckIfSuccessAllRules++;
                                }
                            }
                        }

                        if (Rules.Count() == CheckIfSuccessAllRules)
                        {
                            string DynamicAttributeName = "";
                            int DynamicAttributeId = _unitOfWork.DependencyRowRepository
                                .GetIncludeWhereFirst(x => x.RowId == RowId, x => x.Dependency).Dependency.DynamicAttId.Value;

                            BaseInstAttView InputDynamicAttribute = EditInstallationViewModel.DynamicInstAttsValue
                                .FirstOrDefault(x => x.Id == DynamicAttributeId);

                            if (InputDynamicAttribute == null)
                            {
                                DynamicAttributeName = _unitOfWork.DynamicAttRepository
                                    .GetWhereFirst(x => x.Id == DynamicAttributeId).Key;

                                return $"({DynamicAttributeName}) value can't be null";
                            }
                            else
                            {
                                object DependencyValidationValue = new object();
                                object InputDynamicValue = new object();

                                if (DynamicAttributeMainDependency.ValueBoolean != null)
                                {
                                    DependencyValidationValue = DynamicAttributeMainDependency.ValueBoolean;
                                    InputDynamicValue = bool.Parse(InputDynamicAttribute.Value.ToString());
                                }
                                else if (DynamicAttributeMainDependency.ValueDateTime != null)
                                {
                                    DependencyValidationValue = DynamicAttributeMainDependency.ValueDateTime;
                                    InputDynamicValue = DateTime.Parse(InputDynamicAttribute.Value.ToString());
                                }
                                else if (DynamicAttributeMainDependency.ValueDouble != null)
                                {
                                    DependencyValidationValue = DynamicAttributeMainDependency.ValueDouble;
                                    InputDynamicValue = double.Parse(InputDynamicAttribute.Value.ToString());
                                }
                                else if (!string.IsNullOrEmpty(DynamicAttributeMainDependency.ValueString))
                                {
                                    DependencyValidationValue = DynamicAttributeMainDependency.ValueString;
                                    InputDynamicValue = InputDynamicAttribute.Value.ToString();
                                }

                                string DependencyValidationOperation = DynamicAttributeMainDependency.Operation.Name;

                                if (!(DependencyValidationOperation == "==" ? InputDynamicValue.ToString().ToLower() == DependencyValidationValue.ToString().ToLower() :
                                    DependencyValidationOperation == "!=" ? InputDynamicValue.ToString().ToLower() != DependencyValidationValue.ToString().ToLower() :
                                    DependencyValidationOperation == ">" ? Comparer.DefaultInvariant.Compare(InputDynamicValue, DependencyValidationValue) == 1 :
                                    DependencyValidationOperation == ">=" ? (Comparer.DefaultInvariant.Compare(InputDynamicValue, DependencyValidationValue) == 1 ||
                                        InputDynamicValue.ToString().ToLower() == DependencyValidationValue.ToString().ToLower()) :
                                    DependencyValidationOperation == "<" ? Comparer.DefaultInvariant.Compare(InputDynamicValue, DependencyValidationValue) == -1 :
                                    DependencyValidationOperation == "<=" ? (Comparer.DefaultInvariant.Compare(InputDynamicValue, DependencyValidationValue) == -1 ||
                                        InputDynamicValue.ToString().ToLower() == DependencyValidationValue.ToString().ToLower()) : false))
                                {
                                    DynamicAttributeName = _unitOfWork.DynamicAttRepository
                                        .GetWhereFirst(x => x.Id == DynamicAttributeId).Key;

                                    string ReturnOperation = (DependencyValidationOperation == "==" ? "equal to" :
                                        (DependencyValidationOperation == "!=" ? "not equal to" :
                                        (DependencyValidationOperation == ">" ? "bigger than" :
                                        (DependencyValidationOperation == ">=" ? "bigger than or equal to" :
                                        (DependencyValidationOperation == "<" ? "smaller than" :
                                        (DependencyValidationOperation == "<=" ? "smaller than or equal to" : ""))))));

                                    return $"({DynamicAttributeName}) value must be {ReturnOperation} {DependencyValidationValue}";
                                }
                            }
                        }
                    }
                }
            }
            return string.Empty;
        }
        public string CheckGeneralValidationFunctionEditVersion(List<BaseInstAttView> TLIdynamicAttInstValue, string TableName)
        {
            List<DynamicAttViewModel> DynamicAttributes = _mapper.Map<List<DynamicAttViewModel>>(_unitOfWork.DynamicAttRepository
                .GetIncludeWhere(x => x.tablesNames.TableName.ToLower() == TableName.ToLower() && !x.disable, x => x.tablesNames).ToList());

            foreach (DynamicAttViewModel DynamicAttributeEntity in DynamicAttributes)
            {
                TLIvalidation Validation = _unitOfWork.ValidationRepository
                    .GetIncludeWhereFirst(x => x.DynamicAttId == DynamicAttributeEntity.Id, x => x.Operation, x => x.DynamicAtt);

                if (Validation != null)
                {
                    BaseInstAttView DynmaicAttributeValue = TLIdynamicAttInstValue.FirstOrDefault(x => x.Id == DynamicAttributeEntity.Id);

                    if (DynmaicAttributeValue == null)
                        return $"({Validation.DynamicAtt.Key}) value can't be null and must be inserted";

                    string OperationName = Validation.Operation.Name;

                    object InputDynamicValue = new object();
                    object ValidationValue = new object();

                    if (Validation.ValueBoolean != null)
                    {
                        ValidationValue = Validation.ValueBoolean;
                        InputDynamicValue = bool.Parse(DynmaicAttributeValue.Value.ToString());
                    }
                    else if (Validation.ValueDateTime != null)
                    {
                        ValidationValue = Validation.ValueDateTime;
                        InputDynamicValue = DateTime.Parse(DynmaicAttributeValue.Value.ToString());
                    }
                    else if (Validation.ValueDouble != null)
                    {
                        ValidationValue = Validation.ValueDouble;
                        InputDynamicValue = double.Parse(DynmaicAttributeValue.Value.ToString());
                    }
                    else if (!string.IsNullOrEmpty(Validation.ValueString))
                    {
                        ValidationValue = Validation.ValueString;
                        if (DynmaicAttributeValue.Value == null)
                            ValidationValue = "";
                        else
                            InputDynamicValue = DynmaicAttributeValue.Value.ToString();
                    }

                    if (!(OperationName == "==" ? InputDynamicValue.ToString().ToLower() == ValidationValue.ToString().ToLower() :
                        OperationName == "!=" ? InputDynamicValue.ToString().ToLower() != ValidationValue.ToString().ToLower() :
                        OperationName == ">" ? Comparer.DefaultInvariant.Compare(InputDynamicValue, ValidationValue) == 1 :
                        OperationName == ">=" ? (Comparer.DefaultInvariant.Compare(InputDynamicValue, ValidationValue) == 1 ||
                            InputDynamicValue.ToString().ToLower() == ValidationValue.ToString().ToLower()) :
                        OperationName == "<" ? Comparer.DefaultInvariant.Compare(InputDynamicValue, ValidationValue) == -1 :
                        OperationName == "<=" ? (Comparer.DefaultInvariant.Compare(InputDynamicValue, ValidationValue) == -1 ||
                            InputDynamicValue.ToString().ToLower() == ValidationValue.ToString().ToLower()) : false))
                    {
                        string DynamicAttributeName = _unitOfWork.DynamicAttRepository
                            .GetWhereFirst(x => x.Id == Validation.DynamicAttId).Key;

                        string ReturnOperation = (OperationName == "==" ? "equal to" :
                            (OperationName == "!=" ? "not equal to" :
                            (OperationName == ">" ? "bigger than" :
                            (OperationName == ">=" ? "bigger than or equal to" :
                            (OperationName == "<" ? "smaller than" :
                            (OperationName == "<=" ? "smaller than or equal to" : ""))))));

                        return $"({DynamicAttributeName}) value must be {ReturnOperation} {ValidationValue}";
                    }
                }
            }
            return string.Empty;
        }
        public Response<bool> DismantleOtherInventory(int UserId,string SiteCode, int OtherInventoryId, string OtherInventoryName, int? TaskId,string ConnectionString,bool ExternalSys)
        {
            using (TransactionScope scope = new TransactionScope())
            {
                try
                {
                    TLIhistory AddTablesHistory = new TLIhistory();
                    if (OtherInventoryName.ToLower() == OtherInventoryType.TLIsolar.ToString().ToLower())
                    {
                        var Solar = _unitOfWork.OtherInSiteRepository.GetIncludeWhereFirst(x => x.allOtherInventoryInst.solarId == OtherInventoryId && !x.Dismantle
                        && x.SiteCode.ToLower() == SiteCode.ToLower()
                        , x => x.allOtherInventoryInst, x => x.allOtherInventoryInst.solar, x => x.allOtherInventoryInst.cabinet,
                        x => x.allOtherInventoryInst.generator, x => x.Site);

                        if (Solar.ReservedSpace == true)
                        {
                            var OldSolar = _unitOfWork.OtherInSiteRepository.GetAllAsQueryable().AsNoTracking().Include(x => x.allOtherInventoryInst)
                                .Include(x => x.allOtherInventoryInst.solar).Include(x => x.allOtherInventoryInst.generator).
                                Include(x => x.allOtherInventoryInst.cabinet)
                                .Include(x => x.Site).FirstOrDefault(x => x.allOtherInventoryInst.solarId == OtherInventoryId
                                 && !x.Dismantle && x.SiteCode.ToLower() == SiteCode.ToLower());
                            Solar.Dismantle = true;
                            Solar.allOtherInventoryInst.solar.Name = Solar.allOtherInventoryInst.solar.Name + DateTime.Now;

                            if (ExternalSys == false)
                            {
                                AddTablesHistory.HistoryTypeId = _unitOfWork.HistoryTypeRepository.GetWhereFirst(x => x.Name == "Delete").Id;
                                AddTablesHistory.RecordId = OtherInventoryId.ToString();
                                AddTablesHistory.TablesNameId = _unitOfWork.TablesNamesRepository.GetWhereFirst(x => x.TableName == "TLIsolar").Id;
                                AddTablesHistory.UserId = UserId;
                                AddTablesHistory.SiteCode = SiteCode;
 


                                _dbContext.TLIhistory.Add(AddTablesHistory);
                                _unitOfWork.SaveChanges();
                            }
                            if (ExternalSys == true)
                            {
                                AddTablesHistory.HistoryTypeId = _unitOfWork.HistoryTypeRepository.GetWhereFirst(x => x.Name == "Delete").Id;
                                AddTablesHistory.RecordId = OtherInventoryId.ToString();
                                AddTablesHistory.TablesNameId = _unitOfWork.TablesNamesRepository.GetWhereFirst(x => x.TableName == "TLIsolar").Id;
                                AddTablesHistory.ExternalSysId = UserId;
                                AddTablesHistory.SiteCode = SiteCode;



                                _dbContext.TLIhistory.Add(AddTablesHistory);
                                _unitOfWork.SaveChanges();
                            }
                            var HistroryId = AddTablesHistory.Id;
                            
                            var TabelTLIotherInSite = _unitOfWork.TablesNamesRepository.GetWhereFirst(x => x.TableName == "TLIotherInSite").Id;
                            _unitOfWork.OtherInSiteRepository.UpdateWithHLogic(UserId, HistroryId, TabelTLIotherInSite, OldSolar, Solar);
                            _unitOfWork.SolarRepository.Update(Solar.allOtherInventoryInst.solar);
                            _unitOfWork.SaveChanges();
                            var OldSite = _unitOfWork.SiteRepository.GetAllAsQueryable().AsNoTracking().FirstOrDefault(x => x.SiteCode.ToLower() == Solar.SiteCode.ToLower());
                            Solar.Site.ReservedSpace = Solar.Site.ReservedSpace - Solar.allOtherInventoryInst.solar.SpaceInstallation;
                            _unitOfWork.SiteRepository.UpdateSiteWithHistory(UserId, OldSite, Solar.Site);
                            _unitOfWork.SaveChanges();
                            var ReferenceSolar = _unitOfWork.OtherInventoryDistanceRepository.GetWhere(x => x.ReferenceOtherInventoryId
                            == Solar.allOtherInventoryInstId);
                            foreach (var item in ReferenceSolar)
                            {
                                _unitOfWork.OtherInventoryDistanceRepository.RemoveItem(item);
                                _unitOfWork.OtherInventoryDistanceRepository.RemoveItemWithHistory(UserId, item);
                                _unitOfWork.SaveChanges();
                            }
                        }
                        else
                        {
                            var OldSolar = _unitOfWork.OtherInSiteRepository.GetAllAsQueryable().AsNoTracking().Include(x => x.allOtherInventoryInst)
                                .Include(x => x.allOtherInventoryInst.solar).Include(x => x.allOtherInventoryInst.generator).
                                Include(x => x.allOtherInventoryInst.cabinet)
                                .Include(x => x.Site).FirstOrDefault(x => x.allOtherInventoryInst.solarId == OtherInventoryId
                                && !x.Dismantle && x.SiteCode.ToLower() == SiteCode.ToLower());
                            Solar.Dismantle = true;
                            Solar.allOtherInventoryInst.solar.Name = Solar.allOtherInventoryInst.solar.Name + DateTime.Now;
                            

                            if (ExternalSys == false)
                            {

                                AddTablesHistory.HistoryTypeId = _unitOfWork.HistoryTypeRepository.GetWhereFirst(x => x.Name == "Delete").Id;
                                AddTablesHistory.RecordId = OtherInventoryId.ToString();
                                AddTablesHistory.TablesNameId = _unitOfWork.TablesNamesRepository.GetWhereFirst(x => x.TableName == "TLIsolar").Id;
                                AddTablesHistory.UserId = UserId;
                                AddTablesHistory.SiteCode = SiteCode;


                                _dbContext.TLIhistory.Add(AddTablesHistory);
                                _unitOfWork.SaveChanges();
                            }
                            if (ExternalSys == true)
                            {

                                AddTablesHistory.HistoryTypeId = _unitOfWork.HistoryTypeRepository.GetWhereFirst(x => x.Name == "Delete").Id;
                                AddTablesHistory.RecordId = OtherInventoryId.ToString();
                                AddTablesHistory.TablesNameId = _unitOfWork.TablesNamesRepository.GetWhereFirst(x => x.TableName == "TLIsolar").Id;
                                AddTablesHistory.ExternalSysId = UserId;
                                AddTablesHistory.SiteCode = SiteCode;


                                _dbContext.TLIhistory.Add(AddTablesHistory);
                                _unitOfWork.SaveChanges();
                            }
                            var HistroryId = AddTablesHistory.Id;

                            var TabelTLIotherInSite = _unitOfWork.TablesNamesRepository.GetWhereFirst(x => x.TableName == "TLIotherInSite").Id;
                            _unitOfWork.OtherInSiteRepository.UpdateWithHLogic(UserId, HistroryId, TabelTLIotherInSite, OldSolar, Solar);
                            _unitOfWork.SolarRepository.Update(Solar.allOtherInventoryInst.solar);
                            _unitOfWork.SaveChanges();
                         
                            var ReferenceSolar = _unitOfWork.OtherInventoryDistanceRepository.GetWhere(x => x.ReferenceOtherInventoryId
                          == Solar.allOtherInventoryInstId);
                            foreach (var item in ReferenceSolar)
                            {
                                _unitOfWork.OtherInventoryDistanceRepository.RemoveItem(item);
                                _unitOfWork.OtherInventoryDistanceRepository.RemoveItemWithHistory(UserId, item);
                                _unitOfWork.SaveChanges();
                            }
                        }
                    }
                    else if (OtherInventoryName.ToLower() == OtherInventoryType.TLIgenerator.ToString().ToLower())
                    {
                        var Generator = _unitOfWork.OtherInSiteRepository.GetIncludeWhereFirst(x => x.allOtherInventoryInst.generatorId == OtherInventoryId
                         && !x.Dismantle && x.SiteCode.ToLower() == SiteCode.ToLower()
                        , x => x.allOtherInventoryInst, x => x.allOtherInventoryInst.solar, x => x.allOtherInventoryInst.cabinet,
                        x => x.allOtherInventoryInst.generator, x => x.Site);

                        if (Generator.ReservedSpace == true)
                        {
                            var OldGenerator = _unitOfWork.OtherInSiteRepository.GetAllAsQueryable().AsNoTracking().Include(x => x.allOtherInventoryInst)
                                .Include(x => x.allOtherInventoryInst.solar).Include(x => x.allOtherInventoryInst.generator).
                                Include(x => x.allOtherInventoryInst.cabinet)
                                .Include(x => x.Site).FirstOrDefault(x => x.allOtherInventoryInst.generatorId == OtherInventoryId
                                 && !x.Dismantle && x.SiteCode.ToLower() == SiteCode.ToLower());
                            Generator.Dismantle = true;
                            Generator.allOtherInventoryInst.generator.Name = Generator.allOtherInventoryInst.generator.Name + DateTime.Now;
                            
                            if (ExternalSys == false)
                            {


                                AddTablesHistory.HistoryTypeId = _unitOfWork.HistoryTypeRepository.GetWhereFirst(x => x.Name == "Delete").Id;
                                AddTablesHistory.RecordId = OtherInventoryId.ToString();
                                AddTablesHistory.TablesNameId = _unitOfWork.TablesNamesRepository.GetWhereFirst(x => x.TableName == "TLIgenerator").Id;
                                AddTablesHistory.UserId = UserId;
                                AddTablesHistory.SiteCode = SiteCode;

                                _dbContext.TLIhistory.Add(AddTablesHistory);
                                _unitOfWork.SaveChanges();
                            }
                            if (ExternalSys == true)
                            {

                                AddTablesHistory.HistoryTypeId = _unitOfWork.HistoryTypeRepository.GetWhereFirst(x => x.Name == "Delete").Id;
                                AddTablesHistory.RecordId = OtherInventoryId.ToString();
                                AddTablesHistory.TablesNameId = _unitOfWork.TablesNamesRepository.GetWhereFirst(x => x.TableName == "TLIgenerator").Id;
                                AddTablesHistory.ExternalSysId = UserId;
                                AddTablesHistory.SiteCode = SiteCode;

                                _dbContext.TLIhistory.Add(AddTablesHistory);
                                _unitOfWork.SaveChanges();
                            }
                            var HistroryId = AddTablesHistory.Id;

                            var TabelTLIotherInSite = _unitOfWork.TablesNamesRepository.GetWhereFirst(x => x.TableName == "TLIotherInSite").Id;
                            _unitOfWork.OtherInSiteRepository.UpdateWithHLogic(UserId, HistroryId, TabelTLIotherInSite, OldGenerator, Generator);
                            _unitOfWork.GeneratorRepository.Update(Generator.allOtherInventoryInst.generator);
                            _unitOfWork.SaveChanges();
                         
                            var OldSite = _unitOfWork.SiteRepository.GetAllAsQueryable().AsNoTracking().FirstOrDefault(x => x.SiteCode.ToLower() == Generator.SiteCode.ToLower());
                            Generator.Site.ReservedSpace = Generator.Site.ReservedSpace - Generator.allOtherInventoryInst.generator.SpaceInstallation;
                            _unitOfWork.SiteRepository.UpdateSiteWithHistory(UserId, OldSite, Generator.Site);
                            _unitOfWork.SaveChanges();
                            var ReferenceSolar = _unitOfWork.OtherInventoryDistanceRepository.GetWhere(x => x.ReferenceOtherInventoryId
                           == Generator.allOtherInventoryInstId);
                            foreach (var item in ReferenceSolar)
                            {
                                _unitOfWork.OtherInventoryDistanceRepository.RemoveItem(item);
                                _unitOfWork.OtherInventoryDistanceRepository.RemoveItemWithHistory(UserId, item);
                                _unitOfWork.SaveChanges();
                            }
                        }
                        else
                        {
                            var OldGenerator = _unitOfWork.OtherInSiteRepository.GetAllAsQueryable().AsNoTracking().Include(x => x.allOtherInventoryInst)
                                .Include(x => x.allOtherInventoryInst.solar).Include(x => x.allOtherInventoryInst.generator).
                                Include(x => x.allOtherInventoryInst.cabinet)
                                .Include(x => x.Site).FirstOrDefault(x => x.allOtherInventoryInst.generatorId == OtherInventoryId
                                 && !x.Dismantle && x.SiteCode.ToLower() == SiteCode.ToLower());
                            Generator.Dismantle = true;
                            Generator.allOtherInventoryInst.generator.Name = Generator.allOtherInventoryInst.generator.Name + DateTime.Now;
                            if (ExternalSys == false)
                            {


                                AddTablesHistory.HistoryTypeId = _unitOfWork.HistoryTypeRepository.GetWhereFirst(x => x.Name == "Delete").Id;
                                AddTablesHistory.RecordId = OtherInventoryId.ToString();
                                AddTablesHistory.TablesNameId = _unitOfWork.TablesNamesRepository.GetWhereFirst(x => x.TableName == "TLIgenerator").Id;
                                AddTablesHistory.UserId = UserId;
                                AddTablesHistory.SiteCode = SiteCode;

                                _dbContext.TLIhistory.Add(AddTablesHistory);
                                _unitOfWork.SaveChanges();
                            }
                            if (ExternalSys == true)
                            {

                                AddTablesHistory.HistoryTypeId = _unitOfWork.HistoryTypeRepository.GetWhereFirst(x => x.Name == "Delete").Id;
                                AddTablesHistory.RecordId = OtherInventoryId.ToString();
                                AddTablesHistory.TablesNameId = _unitOfWork.TablesNamesRepository.GetWhereFirst(x => x.TableName == "TLIgenerator").Id;
                                AddTablesHistory.ExternalSysId = UserId;
                                AddTablesHistory.SiteCode = SiteCode;

                                _dbContext.TLIhistory.Add(AddTablesHistory);
                                _unitOfWork.SaveChanges();
                            }
                            var HistroryId = AddTablesHistory.Id;

                            var TabelTLIotherInSite = _unitOfWork.TablesNamesRepository.GetWhereFirst(x => x.TableName == "TLIotherInSite").Id;
                            _unitOfWork.OtherInSiteRepository.UpdateWithHLogic(UserId, HistroryId, TabelTLIotherInSite, OldGenerator, Generator);
                            _unitOfWork.GeneratorRepository.Update(Generator.allOtherInventoryInst.generator);
                            _unitOfWork.SaveChanges();
                          
                            var ReferenceSolar = _unitOfWork.OtherInventoryDistanceRepository.GetWhere(x => x.ReferenceOtherInventoryId
                        == Generator.allOtherInventoryInstId);
                            foreach (var item in ReferenceSolar)
                            {
                                _unitOfWork.OtherInventoryDistanceRepository.RemoveItem(item);
                                _unitOfWork.OtherInventoryDistanceRepository.RemoveItemWithHistory(UserId, item);
                                _unitOfWork.SaveChanges();
                            }
                        }
                    }
                    else if (OtherInventoryName.ToLower() == OtherInventoryType.TLIcabinet.ToString().ToLower())
                    {
                        var Cabinet = _unitOfWork.OtherInSiteRepository.GetIncludeWhereFirst(x => x.allOtherInventoryInst.cabinetId == OtherInventoryId
                         && !x.Dismantle && x.SiteCode.ToLower() == SiteCode.ToLower()
                        , x => x.allOtherInventoryInst, x => x.allOtherInventoryInst.solar, x => x.allOtherInventoryInst.cabinet,
                        x => x.allOtherInventoryInst.generator, x => x.Site);
                        var SolarReleationWithCabinet = _unitOfWork.OtherInSiteRepository.GetWhereAndInclude(x => x.allOtherInventoryInst.solar.CabinetId == OtherInventoryId
                         && !x.Dismantle && x.SiteCode.ToLower() == SiteCode.ToLower(), x => x.allOtherInventoryInst
                         , x => x.allOtherInventoryInst.solar, x => x.allOtherInventoryInst.generator, x => x.allOtherInventoryInst.cabinet);
                        if (SolarReleationWithCabinet != null && SolarReleationWithCabinet.Count > 0)
                            return new Response<bool>(false, false, null, "can not dismantle the cabinet becuase it relation with solar", (int)Helpers.Constants.ApiReturnCode.fail);
                        if (Cabinet.ReservedSpace == true)
                        {
                            var OldCabinet = _unitOfWork.OtherInSiteRepository.GetAllAsQueryable().AsNoTracking().Include(x => x.allOtherInventoryInst)
                                .Include(x => x.allOtherInventoryInst.solar).Include(x => x.allOtherInventoryInst.generator).
                                Include(x => x.allOtherInventoryInst.cabinet)
                                .Include(x => x.Site).FirstOrDefault(x => x.allOtherInventoryInst.cabinetId == OtherInventoryId
                                 && !x.Dismantle && x.SiteCode.ToLower() == SiteCode.ToLower());
                            Cabinet.Dismantle = true;
                            Cabinet.allOtherInventoryInst.cabinet.Name = Cabinet.allOtherInventoryInst.cabinet.Name + DateTime.Now;
                            if (ExternalSys == false)
                            {
                                AddTablesHistory.HistoryTypeId = _unitOfWork.HistoryTypeRepository.GetWhereFirst(x => x.Name == "Delete").Id;
                                AddTablesHistory.RecordId = OtherInventoryId.ToString();
                                AddTablesHistory.TablesNameId = _unitOfWork.TablesNamesRepository.GetWhereFirst(x => x.TableName == "TLIcabinet").Id;
                                AddTablesHistory.UserId = UserId;
                                AddTablesHistory.SiteCode = SiteCode;



                                _dbContext.TLIhistory.Add(AddTablesHistory);
                                _unitOfWork.SaveChanges();
                            }
                            if (ExternalSys == true)
                            {
                                AddTablesHistory.HistoryTypeId = _unitOfWork.HistoryTypeRepository.GetWhereFirst(x => x.Name == "Delete").Id;
                                AddTablesHistory.RecordId = OtherInventoryId.ToString();
                                AddTablesHistory.TablesNameId = _unitOfWork.TablesNamesRepository.GetWhereFirst(x => x.TableName == "TLIsolar").Id;
                                AddTablesHistory.ExternalSysId = UserId;
                                AddTablesHistory.SiteCode = SiteCode;



                                _dbContext.TLIhistory.Add(AddTablesHistory);
                                _unitOfWork.SaveChanges();
                            }
                            var HistroryId = AddTablesHistory.Id;

                            var TabelTLIotherInSite = _unitOfWork.TablesNamesRepository.GetWhereFirst(x => x.TableName == "TLIotherInSite").Id;
                            _unitOfWork.OtherInSiteRepository.UpdateWithHLogic(UserId, HistroryId, TabelTLIotherInSite, OldCabinet, Cabinet);
                            _unitOfWork.CabinetRepository.Update(Cabinet.allOtherInventoryInst.cabinet);
                            _unitOfWork.SaveChanges();
                          
                            var OldSite = _unitOfWork.SiteRepository.GetAllAsQueryable().AsNoTracking().FirstOrDefault(x => x.SiteCode.ToLower() == Cabinet.SiteCode.ToLower());
                            Cabinet.Site.ReservedSpace = Cabinet.Site.ReservedSpace - Cabinet.allOtherInventoryInst.cabinet.SpaceInstallation;
                            _unitOfWork.SiteRepository.UpdateSiteWithHistory(UserId, OldSite, Cabinet.Site);
                            _unitOfWork.SaveChanges();
                            var ReferenceSolar = _unitOfWork.OtherInventoryDistanceRepository.GetWhere(x => x.ReferenceOtherInventoryId
                            == Cabinet.allOtherInventoryInstId);
                            foreach (var item in ReferenceSolar)
                            {
                                _unitOfWork.OtherInventoryDistanceRepository.RemoveItem(item);
                                _unitOfWork.OtherInventoryDistanceRepository.RemoveItemWithHistory(UserId, item);
                                _unitOfWork.SaveChanges();
                            }
                        }
                        else
                        {
                            var OldCabinet = _unitOfWork.OtherInSiteRepository.GetAllAsQueryable().AsNoTracking().Include(x => x.allOtherInventoryInst)
                                .Include(x => x.allOtherInventoryInst.solar).Include(x => x.allOtherInventoryInst.generator).
                                Include(x => x.allOtherInventoryInst.cabinet)
                                .Include(x => x.Site).FirstOrDefault(x => x.allOtherInventoryInst.cabinetId == OtherInventoryId
                                 && !x.Dismantle && x.SiteCode.ToLower() == SiteCode.ToLower());
                            Cabinet.Dismantle = true;
                            Cabinet.allOtherInventoryInst.cabinet.Name = Cabinet.allOtherInventoryInst.cabinet.Name + DateTime.Now;
                            if (ExternalSys == false)
                            {
                                AddTablesHistory.HistoryTypeId = _unitOfWork.HistoryTypeRepository.GetWhereFirst(x => x.Name == "Delete").Id;
                                AddTablesHistory.RecordId = OtherInventoryId.ToString();
                                AddTablesHistory.TablesNameId = _unitOfWork.TablesNamesRepository.GetWhereFirst(x => x.TableName == "TLIcabinet").Id;
                                AddTablesHistory.UserId = UserId;
                                AddTablesHistory.SiteCode = SiteCode;



                                _dbContext.TLIhistory.Add(AddTablesHistory);
                                _unitOfWork.SaveChanges();
                            }
                            if (ExternalSys == true)
                            {
                                AddTablesHistory.HistoryTypeId = _unitOfWork.HistoryTypeRepository.GetWhereFirst(x => x.Name == "Delete").Id;
                                AddTablesHistory.RecordId = OtherInventoryId.ToString();
                                AddTablesHistory.TablesNameId = _unitOfWork.TablesNamesRepository.GetWhereFirst(x => x.TableName == "TLIsolar").Id;
                                AddTablesHistory.ExternalSysId = UserId;
                                AddTablesHistory.SiteCode = SiteCode;



                                _dbContext.TLIhistory.Add(AddTablesHistory);
                                _unitOfWork.SaveChanges();
                            }
                            var HistroryId = AddTablesHistory.Id;

                            var TabelTLIotherInSite = _unitOfWork.TablesNamesRepository.GetWhereFirst(x => x.TableName == "TLIotherInSite").Id;
                            _unitOfWork.OtherInSiteRepository.UpdateWithHLogic(UserId, HistroryId, TabelTLIotherInSite, OldCabinet, Cabinet);
                            _unitOfWork.CabinetRepository.Update(Cabinet.allOtherInventoryInst.cabinet);
                            _unitOfWork.SaveChanges();
                       
                            var ReferenceSolar = _unitOfWork.OtherInventoryDistanceRepository.GetWhere(x => x.ReferenceOtherInventoryId
                            == Cabinet.allOtherInventoryInstId);
                            foreach (var item in ReferenceSolar)
                            {
                                _unitOfWork.OtherInventoryDistanceRepository.RemoveItem(item);
                                _unitOfWork.OtherInventoryDistanceRepository.RemoveItemWithHistory(UserId, item);
                                _unitOfWork.SaveChanges();
                            }
                        }
                    }
                    var AttachFileRelated = _dbContext.TLIattachedFiles.Include(x => x.tablesName
                    ).Where(x => x.tablesName.TableName.ToLower() == OtherInventoryName.ToLower() &&
                    x.RecordId == OtherInventoryId);
                    _dbContext.RemoveRange(AttachFileRelated);
                    if (TaskId != null)
                    {
                        var Submit = _unitOfWork.SiteRepository.SubmitTaskByTLI(TaskId);
                        var result = Submit.Result;
                        if (result.result == true && result.errorMessage == null)
                        {
                            _unitOfWork.SaveChanges();
                            scope.Complete();
                        }
                        else
                        {
                            scope.Dispose();
                            return new Response<bool>(false, false, null, result.errorMessage.ToString(), (int)Helpers.Constants.ApiReturnCode.fail);
                        }
                    }
                    else
                    {
                        _unitOfWork.SaveChanges();
                        scope.Complete();
                    }
                    Task.Run(() => _unitOfWork.CivilWithLegsRepository.RefreshView(ConnectionString));
                    return new Response<bool>(true, true, null, null, (int)Helpers.Constants.ApiReturnCode.success);
                }
                catch (Exception er)
                {

                    return new Response<bool>(false, false, null, er.Message, (int)Helpers.Constants.ApiReturnCode.fail);
                }
            }

        }
        private void UpdateDynamicAttInstValue(List<BaseInstAttView> DynamicInstAttsValue, int InventoryId, int TableEntityId)
        {
            Parallel.ForEach(DynamicInstAttsValue, DynamicInstAttValue =>
            {
                var DAIV = _unitOfWork.DynamicAttInstValueRepository.GetWhereFirst(d => d.InventoryId == InventoryId && d.DynamicAttId == DynamicInstAttValue.Id && d.tablesNamesId == TableEntityId);
                if (DAIV.ValueBoolean != null)
                {
                    DAIV.ValueBoolean = (bool)DynamicInstAttValue.Value;
                    DAIV.Value = DynamicInstAttValue.Value.ToString();
                }
                else if (DAIV.ValueDateTime != null)
                {
                    DAIV.ValueDateTime = (DateTime)DynamicInstAttValue.Value;
                    DAIV.Value = DynamicInstAttValue.Value.ToString();
                }
                else if (DAIV.ValueDouble != null)
                {
                    DAIV.ValueDouble = (double)DynamicInstAttValue.Value;
                    DAIV.Value = DynamicInstAttValue.Value.ToString();
                }
                else if (!string.IsNullOrEmpty(DAIV.ValueString))
                {
                    DAIV.ValueString = (string)DynamicInstAttValue.Value;
                    DAIV.Value = DynamicInstAttValue.Value.ToString();
                }
                _unitOfWork.DynamicAttInstValueRepository.Update(DAIV);
            });
        }


        //    //Function accept 2 parameters 
        //    //First List of installation dynamic attribute values 
        //    //Second InventoryId 
        //    private void UpdateDynamicAttInstValue(List<BaseInstAttView> DynamicInstAttsValue, int InventoryId)
        //    {
        //        Parallel.ForEach(DynamicInstAttsValue, DynamicInstAttValue =>
        //        {
        //            var DAIV = _unitOfWork.DynamicAttInstValueRepository.GetWhereFirst(d => d.InventoryId == InventoryId && d.DynamicAttId == DynamicInstAttValue.Id);
        //            if (DAIV.ValueBoolean != null)
        //            {
        //                DAIV.ValueBoolean = (bool)DynamicInstAttValue.Value;
        //                DAIV.Value = DynamicInstAttValue.Value.ToString();
        //            }
        //            else if (DAIV.ValueDateTime != null)
        //            {
        //                DAIV.ValueDateTime = (DateTime)DynamicInstAttValue.Value;
        //                DAIV.Value = DynamicInstAttValue.Value.ToString();
        //            }
        //            else if (DAIV.ValueDouble != null)
        //            {
        //                DAIV.ValueDouble = (double)DynamicInstAttValue.Value;
        //                DAIV.Value = DynamicInstAttValue.Value.ToString();
        //            }
        //            else if (!string.IsNullOrEmpty(DAIV.ValueString))
        //            {
        //                DAIV.ValueString = (string)DynamicInstAttValue.Value;
        //                DAIV.Value = DynamicInstAttValue.Value.ToString();
        //            }
        //            _unitOfWork.DynamicAttInstValueRepository.Update(DAIV);
        //        });
        //    }
        //}
        #endregion
        #region GetById
        //Function take 2 parameters OtherInventoryInsId, TableName
        //First take table name Entity by TableName
        //specify the table i deal with
        //get record by Id
        //get activated attributes and values
        //get dynamic attributes by TableNameId
        public Response<GetForAddOtherInventoryInstallationObject> GetGenertorInstallationById(int GeneratorId, string TableName, int UserId, bool ExternalSys)
        {
            try
            {
                TLItablesNames TableNameEntity = _unitOfWork.TablesNamesRepository
                    .GetWhereFirst(c => c.TableName.ToLower() == TableName.ToLower());

                GetForAddOtherInventoryInstallationObject objectInst = new GetForAddOtherInventoryInstallationObject();


                TLIotherInSite GeneratorInst = _unitOfWork.OtherInSiteRepository
                    .GetIncludeWhereFirst(x => x.allOtherInventoryInst.generatorId == GeneratorId && !x.Dismantle, x => x.allOtherInventoryInst.generator.BaseGeneratorType);
                if (GeneratorInst != null)
                {
                    EditGeneratorLibraryAttributes GeneratorLibrary = _mapper.Map<EditGeneratorLibraryAttributes>(_unitOfWork.GeneratorLibraryRepository
                        .GetIncludeWhereFirst(x => x.Id == GeneratorInst.allOtherInventoryInst.generator.GeneratorLibraryId));

                    List<BaseInstAttViews> LibraryAttributes = _unitOfWork.AttributeActivatedRepository
                     .GetAttributeActivatedGetLibrary(Helpers.Constants.TablesNames.TLIgeneratorLibrary.ToString(), GeneratorLibrary, null).ToList();


                    List<BaseInstAttViews> LogisticalAttributes = _mapper.Map<List<BaseInstAttViews>>(_unitOfWork.LogistcalRepository
                            .GetLogisticals(Helpers.Constants.TablePartName.OtherInventory.ToString(), Helpers.Constants.TablesNames.TLIgeneratorLibrary.ToString(), GeneratorLibrary.Id).ToList());

                    LibraryAttributes.AddRange(LogisticalAttributes);

                    objectInst.LibraryAttribute = LibraryAttributes;

                    List<BaseInstAttViews> ListAttributesActivated = _unitOfWork.AttributeActivatedRepository
                        .GetInstAttributeActivatedGetForAdd(Helpers.Constants.TablesNames.TLIgenerator.ToString(), GeneratorInst.allOtherInventoryInst.generator, "GeneratorLibraryId").ToList();

                    BaseInstAttViews NameAttribute = ListAttributesActivated.FirstOrDefault(x => x.Key.ToLower() == "Name".ToLower());
                    if (NameAttribute != null)
                    {
                        BaseInstAttViews Swap = ListAttributesActivated[0];
                        ListAttributesActivated[ListAttributesActivated.IndexOf(NameAttribute)] = Swap;
                        ListAttributesActivated[0] = NameAttribute;
                        NameAttribute.Value = _dbContext.MV_GENERATOR_VIEW.FirstOrDefault(x => x.Id == GeneratorId)?.Name;
                    }

                    var foreignKeyAttributes = ListAttributesActivated.Select(FKitem =>
                    {
                        switch (FKitem.Label.ToLower())
                        {
                            case "basegeneratortype_name":
                                FKitem.Value = _mapper.Map<BaseGeneratorTypeViewModel>(GeneratorInst.allOtherInventoryInst.generator.BaseGeneratorType);
                                FKitem.Options = _mapper.Map<List<BaseGeneratorTypeViewModel>>(_unitOfWork.BaseGeneratorTypeRepository.GetWhere(x => !x.Deleted && !x.Disable).ToList());
                                break;
                            
                        }
                        return FKitem;
                    }).ToList();

                    ListAttributesActivated = ListAttributesActivated.Select(x =>
                    {
                        if (x.Label.ToLower() == "baseexisting" && (x.Value as bool?) == false)
                        {
                            ListAttributesActivated.ForEach(y =>
                            {
                                if (y.Label.ToLower() == "basegeneratortype_name")
                                {
                                    y.visible = false;
                                }
                            });
                        }
                        return x;
                    }).ToList();



                    objectInst.InstallationAttributes = ListAttributesActivated;

                    objectInst.DynamicAttribute = _unitOfWork.DynamicAttInstValueRepository.
                        GetDynamicInstAtt(TableNameEntity.Id, GeneratorId, null);

                    TLIallOtherInventoryInst allOtherInventoryInst = _unitOfWork.AllOtherInventoryInstRepository
                            .GetWhereFirst(x => x.generatorId == GeneratorId);
                    TLIotherInSite otherInSiteInfo = _dbContext.TLIotherInSite.FirstOrDefault(x => x.allOtherInventoryInstId == allOtherInventoryInst.Id);

                    List<BaseInstAttViews> otherInSiteAttributes = _unitOfWork.AttributeActivatedRepository
                         .GetAttributeActivatedGetForAdd(TablesNames.TLIotherInSite.ToString(), otherInSiteInfo, null, "allOtherInventoryInstId", "Id", "Dismantle", "SiteCode").ToList();

                    objectInst.OtherInSite = _mapper.Map<IEnumerable<BaseInstAttViews>>(otherInSiteAttributes);

                    TLIotherInventoryDistance otherInventoryDistance = _unitOfWork.OtherInventoryDistanceRepository.GetWhereFirst(x => x.allOtherInventoryInstId == allOtherInventoryInst.Id);

                    List<BaseInstAttViews> otherInventorytDistanceAttributes = _unitOfWork.AttributeActivatedRepository
                        .GetAttributeActivatedGetForAdd(TablesNames.TLIotherInventoryDistance.ToString(), otherInventoryDistance, null, "allOtherInventoryInstId", "Id", "Dismantle", "SiteCode").ToList();

                    string siteCode = _unitOfWork.OtherInSiteRepository
                     .GetIncludeWhereFirst(
                         x => x.allOtherInventoryInst.generator.Id == GeneratorId && !x.Dismantle && !x.allOtherInventoryInst.Draft,
                         x => x.allOtherInventoryInst, x => x.allOtherInventoryInst.generator, x => x.allOtherInventoryInst.solar, x => x.allOtherInventoryInst.cabinet
                     )?.SiteCode;
                    objectInst.OtherInventoryDistance = otherInventorytDistanceAttributes;
                    if (siteCode != null)
                    {
                        var listAttributes = objectInst.OtherInventoryDistance
                            .Where(attr => attr.DataType.ToLower() == "list" && attr.Key.ToLower() == "referenceotherinventoryid" )
                            .Select(attr =>
                            {
                                TLIotherInventoryDistance support = new TLIotherInventoryDistance();
                                var options = new List<SupportTypeImplementedViewModel>();
                                var Value = new SupportTypeImplementedViewModel();
                                if (otherInventoryDistance != null)
                                {
                                    support = _unitOfWork.OtherInventoryDistanceRepository
                                       .GetWhereFirst(x => x.allOtherInventoryInstId == otherInventoryDistance.allOtherInventoryInstId);
                                }
                                else
                                {
                                    support = null;
                                }
                                if (support != null && support.ReferenceOtherInventoryId != null)
                                {
                                    var supportReferenceAllCivilInst = _unitOfWork.AllOtherInventoryInstRepository
                                        .GetIncludeWhereFirst(x => x.Id == support.ReferenceOtherInventoryId,
                                            x => x.generator,
                                            x => x.solar,
                                            x => x.cabinet);
                                    object referencesValue = new object[0];
                                    if (supportReferenceAllCivilInst != null)
                                    {
                                        if (supportReferenceAllCivilInst.generatorId != null)
                                        {
                                            referencesValue = _dbContext.MV_GENERATOR_VIEW.FirstOrDefault(x => x.Id == supportReferenceAllCivilInst.generatorId)?.Name;
                                        }
                                        else if (supportReferenceAllCivilInst.solarId != null)
                                        {
                                            referencesValue = _dbContext.MV_SOLAR_VIEW.FirstOrDefault(x => x.Id == supportReferenceAllCivilInst.solarId)?.Name;
                                        }
                                        else if (supportReferenceAllCivilInst.cabinet.CabinetPowerLibraryId != null)
                                        {
                                            referencesValue = _dbContext.MV_CABINET_POWER_VIEW.FirstOrDefault(x => x.Id == supportReferenceAllCivilInst.cabinetId)?.Name;
                                        }
                                        else if (supportReferenceAllCivilInst.cabinet.CabinetTelecomLibraryId != null)
                                        {
                                            referencesValue = _dbContext.MV_CABINET_TELECOM_VIEW.FirstOrDefault(x => x.Id == supportReferenceAllCivilInst.cabinetId)?.Name;
                                        }

                                    }

                                    if (referencesValue != null && support.ReferenceOtherInventoryId != null && support.ReferenceOtherInventoryId.HasValue)
                                    {
                                        SupportTypeImplementedViewModel views = new SupportTypeImplementedViewModel()
                                        {
                                            Id = support.ReferenceOtherInventoryId.Value,
                                            Name = referencesValue.ToString()
                                        };
                                        attr.Value = views;
                                    }
                                    if (referencesValue == null)
                                    {
                                        attr.Value = null;
                                    }
                                    var AllOther = _unitOfWork.OtherInSiteRepository
                                            .GetIncludeWhere(x => !x.Dismantle && x.SiteCode == siteCode,
                                                x => x.allOtherInventoryInst,
                                                x => x.allOtherInventoryInst.generator,
                                                x => x.allOtherInventoryInst.solar,
                                                x => x.allOtherInventoryInst.cabinet)?.ToList();


                                    if (AllOther != null && AllOther.Any())
                                    {
                                        options = AllOther.SelectMany(item =>
                                        {
                                            var innerOptions = new List<SupportTypeImplementedViewModel>();

                                            if (item.allOtherInventoryInst.generator != null && item.allOtherInventoryInst.generatorId != GeneratorId)
                                            {
                                                SupportTypeImplementedViewModel civilWithLegsOption = new SupportTypeImplementedViewModel()
                                                {
                                                    Id = item.allOtherInventoryInst.Id,
                                                    Name = item.allOtherInventoryInst.generator?.Name
                                                };

                                                innerOptions.Add(civilWithLegsOption);
                                            }

                                            if (item.allOtherInventoryInst.solar != null)
                                            {
                                                SupportTypeImplementedViewModel civilWithoutLegOption = new SupportTypeImplementedViewModel()
                                                {
                                                    Id = item.allOtherInventoryInst.Id,
                                                    Name = item.allOtherInventoryInst.solar?.Name
                                                };
                                                innerOptions.Add(civilWithoutLegOption);
                                            }

                                            if (item.allOtherInventoryInst.cabinet != null )
                                            {
                                                SupportTypeImplementedViewModel civilNonSteelOption = new SupportTypeImplementedViewModel()
                                                {
                                                    Id = item.allOtherInventoryInst.Id,
                                                    Name = item.allOtherInventoryInst.cabinet?.Name
                                                };
                                                innerOptions.Add(civilNonSteelOption);
                                            }

                                            return innerOptions;
                                        }).ToList();
                                    }
                                }
                                else
                                {

                                    var allOther = _unitOfWork.OtherInSiteRepository
                                        .GetIncludeWhere(x => !x.Dismantle && x.SiteCode == siteCode,
                                            x => x.allOtherInventoryInst,
                                            x => x.allOtherInventoryInst.generator,
                                            x => x.allOtherInventoryInst.solar,
                                            x => x.allOtherInventoryInst.cabinet)?.ToList();
                                    options = allOther.SelectMany(item =>
                                    {
                                        var innerOptions = new List<SupportTypeImplementedViewModel>();

                                        if (item.allOtherInventoryInst.generator != null && item.allOtherInventoryInst.generatorId != GeneratorId)
                                        {
                                            SupportTypeImplementedViewModel civilWithLegsOption = new SupportTypeImplementedViewModel()
                                            {
                                                Id = item.allOtherInventoryInst.Id,
                                                Name = item.allOtherInventoryInst.generator?.Name
                                            };

                                            innerOptions.Add(civilWithLegsOption);
                                        }

                                        if (item.allOtherInventoryInst.solar != null)
                                        {
                                            SupportTypeImplementedViewModel civilWithoutLegOption = new SupportTypeImplementedViewModel()
                                            {
                                                Id = item.allOtherInventoryInst.Id,
                                                Name = item.allOtherInventoryInst.solar?.Name
                                            };
                                            innerOptions.Add(civilWithoutLegOption);
                                        }

                                        if (item.allOtherInventoryInst.cabinet != null)
                                        {
                                            SupportTypeImplementedViewModel civilNonSteelOption = new SupportTypeImplementedViewModel()
                                            {
                                                Id = item.allOtherInventoryInst.Id,
                                                Name = item.allOtherInventoryInst.cabinet?.Name
                                            };
                                            innerOptions.Add(civilNonSteelOption);
                                        }

                                        return innerOptions;
                                    }).ToList();
                                }
                                attr.Options = options;

                                return attr;
                            })
                            .ToList();
                    }
                    if (ExternalSys == true)
                    {
                        TLIhistory tLIhistory = new TLIhistory()
                        {
                            TablesNameId = TableNameEntity.Id,
                            ExternalSysId = UserId,
                            RecordId = GeneratorId.ToString(),
                            HistoryTypeId = 4,
                        };
                        _dbContext.TLIhistory.Add(tLIhistory);
                        _dbContext.SaveChanges();
                    }
                    return new Response<GetForAddOtherInventoryInstallationObject>(true, objectInst, null, null, (int)Helpers.Constants.ApiReturnCode.success);
                }
                else
                {
                    return new Response<GetForAddOtherInventoryInstallationObject>(false, null, null, "this civilwithlegs is not found ", (int)Helpers.Constants.ApiReturnCode.fail);
                }
            }

            catch (Exception err)
            {
                return new Response<GetForAddOtherInventoryInstallationObject>(false, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
        }
        public Response<GetForAddOtherInventoryInstallationObject> GetSolarInstallationById(int SolarId, string TableName,int UserId, bool ExternalSys)
        {
            try
            {
                TLItablesNames TableNameEntity = _unitOfWork.TablesNamesRepository
                    .GetWhereFirst(c => c.TableName.ToLower() == TableName.ToLower());

                GetForAddOtherInventoryInstallationObject objectInst = new GetForAddOtherInventoryInstallationObject();


                TLIotherInSite GeneratorInst = _unitOfWork.OtherInSiteRepository
                    .GetIncludeWhereFirst(x => x.allOtherInventoryInst.solarId == SolarId && !x.Dismantle, x => x.allOtherInventoryInst.solar.Cabinet);
                if (GeneratorInst != null)
                {
                    EditSolarLibraryAttributes SolarLibrary = _mapper.Map<EditSolarLibraryAttributes>(_unitOfWork.SolarLibraryRepository
                        .GetIncludeWhereFirst(x => x.Id == GeneratorInst.allOtherInventoryInst.solar.SolarLibraryId));

                    List<BaseInstAttViews> LibraryAttributes = _unitOfWork.AttributeActivatedRepository
                     .GetAttributeActivatedGetLibrary(Helpers.Constants.TablesNames.TLIsolarLibrary.ToString(), SolarLibrary, null).ToList();


                    List<BaseInstAttViews> LogisticalAttributes = _mapper.Map<List<BaseInstAttViews>>(_unitOfWork.LogistcalRepository
                            .GetLogisticals(Helpers.Constants.TablePartName.OtherInventory.ToString(), Helpers.Constants.TablesNames.TLIsolarLibrary.ToString(), SolarLibrary.Id).ToList());

                    LibraryAttributes.AddRange(LogisticalAttributes);

                    objectInst.LibraryAttribute = LibraryAttributes;

                    List<BaseInstAttViews> ListAttributesActivated = _unitOfWork.AttributeActivatedRepository
                        .GetInstAttributeActivatedGetForAdd(Helpers.Constants.TablesNames.TLIsolar.ToString(), GeneratorInst.allOtherInventoryInst.solar, "SolarLibraryId").ToList();

                    BaseInstAttViews NameAttribute = ListAttributesActivated.FirstOrDefault(x => x.Key.ToLower() == "Name".ToLower());
                    if (NameAttribute != null)
                    {
                        BaseInstAttViews Swap = ListAttributesActivated[0];
                        ListAttributesActivated[ListAttributesActivated.IndexOf(NameAttribute)] = Swap;
                        ListAttributesActivated[0] = NameAttribute;
                        NameAttribute.Value = _dbContext.MV_SOLAR_VIEW.FirstOrDefault(x => x.Id == SolarId)?.Name;
                    }

                    var foreignKeyAttributes = ListAttributesActivated.Select(FKitem =>
                    {
                        switch (FKitem.Label.ToLower())
                        {
                            case "cabinet_name":
                                FKitem.Value = _mapper.Map<BaseGeneratorTypeViewModel>(GeneratorInst.allOtherInventoryInst.solar.Cabinet);
                                FKitem.Options = _mapper.Map<List<BaseGeneratorTypeViewModel>>(_unitOfWork.
                                    OtherInSiteRepository.GetWhereAndInclude(x => !x.Dismantle && x.SiteCode.ToLower()== GeneratorInst.SiteCode.ToLower()
                                    && x.allOtherInventoryInst.cabinetId !=null && x.allOtherInventoryInst.cabinet.CabinetPowerLibraryId !=null,
                                    x=>x.allOtherInventoryInst,x=>x.allOtherInventoryInst.cabinet)
                                    .Select(x=>x.allOtherInventoryInst.cabinet).ToList());
                                break;

                        }
                        return FKitem;
                    }).ToList();

                  
                    objectInst.InstallationAttributes = ListAttributesActivated;

                    objectInst.DynamicAttribute = _unitOfWork.DynamicAttInstValueRepository.
                        GetDynamicInstAtt(TableNameEntity.Id, SolarId, null);

                    TLIallOtherInventoryInst allOtherInventoryInst = _unitOfWork.AllOtherInventoryInstRepository
                            .GetWhereFirst(x => x.solarId == SolarId);
                    TLIotherInSite otherInSiteInfo = _dbContext.TLIotherInSite.FirstOrDefault(x => x.allOtherInventoryInstId == allOtherInventoryInst.Id);

                    List<BaseInstAttViews> otherInSiteAttributes = _unitOfWork.AttributeActivatedRepository
                         .GetAttributeActivatedGetForAdd(TablesNames.TLIotherInSite.ToString(), otherInSiteInfo, null, "allOtherInventoryInstId", "Id", "Dismantle", "SiteCode").ToList();

                    objectInst.OtherInSite = _mapper.Map<IEnumerable<BaseInstAttViews>>(otherInSiteAttributes);

                    TLIotherInventoryDistance otherInventoryDistance = _unitOfWork.OtherInventoryDistanceRepository.GetWhereFirst(x => x.allOtherInventoryInstId == allOtherInventoryInst.Id);

                    List<BaseInstAttViews> otherInventorytDistanceAttributes = _unitOfWork.AttributeActivatedRepository
                        .GetAttributeActivatedGetForAdd(TablesNames.TLIotherInventoryDistance.ToString(), otherInventoryDistance, null, "allOtherInventoryInstId", "Id", "Dismantle", "SiteCode").ToList();

                    string siteCode = _unitOfWork.OtherInSiteRepository
                     .GetIncludeWhereFirst(
                         x => x.allOtherInventoryInst.solar.Id == SolarId && !x.Dismantle && !x.allOtherInventoryInst.Draft,
                         x => x.allOtherInventoryInst, 
                         x => x.allOtherInventoryInst.generator, 
                         x => x.allOtherInventoryInst.solar, x => x.allOtherInventoryInst.cabinet
                     )?.SiteCode;
                    objectInst.OtherInventoryDistance = otherInventorytDistanceAttributes;
                    if (siteCode != null)
                    {
                        var listAttributes = objectInst.OtherInventoryDistance
                            .Where(attr => attr.DataType.ToLower() == "list" && attr.Key.ToLower() == "referenceotherinventoryid" )
                            .Select(attr =>
                            {
                                TLIotherInventoryDistance support = new TLIotherInventoryDistance();
                                var options = new List<SupportTypeImplementedViewModel>();
                                var Value = new SupportTypeImplementedViewModel();
                                if (otherInventoryDistance != null)
                                {
                                    support = _unitOfWork.OtherInventoryDistanceRepository
                                       .GetWhereFirst(x => x.allOtherInventoryInstId == otherInventoryDistance.allOtherInventoryInstId);
                                }
                                else
                                {
                                    support = null;
                                }
                                if (support != null && support.ReferenceOtherInventoryId != null)
                                {
                                    var supportReferenceAllCivilInst = _unitOfWork.AllOtherInventoryInstRepository
                                        .GetIncludeWhereFirst(x => x.Id == support.ReferenceOtherInventoryId,
                                            x => x.generator,
                                            x => x.solar,
                                            x => x.cabinet);
                                    object referencesValue = new object[0];
                                    if (supportReferenceAllCivilInst != null)
                                    {
                                        if (supportReferenceAllCivilInst.generatorId != null)
                                        {
                                            referencesValue = _dbContext.MV_GENERATOR_VIEW.FirstOrDefault(x => x.Id == supportReferenceAllCivilInst.generatorId)?.Name;
                                        }
                                        else if (supportReferenceAllCivilInst.solarId != null)
                                        {
                                            referencesValue = _dbContext.MV_SOLAR_VIEW.FirstOrDefault(x => x.Id == supportReferenceAllCivilInst.solarId)?.Name;
                                        }
                                        else if (supportReferenceAllCivilInst.cabinet.CabinetPowerLibraryId != null)
                                        {
                                            referencesValue = _dbContext.MV_CABINET_POWER_VIEW.FirstOrDefault(x => x.Id == supportReferenceAllCivilInst.cabinetId)?.Name;
                                        }
                                        else if (supportReferenceAllCivilInst.cabinet.CabinetTelecomLibraryId != null)
                                        {
                                            referencesValue = _dbContext.MV_CABINET_TELECOM_VIEW.FirstOrDefault(x => x.Id == supportReferenceAllCivilInst.cabinetId)?.Name;
                                        }

                                    }

                                    if (referencesValue != null && support.ReferenceOtherInventoryId != null && support.ReferenceOtherInventoryId.HasValue)
                                    {
                                        SupportTypeImplementedViewModel views = new SupportTypeImplementedViewModel()
                                        {
                                            Id = support.ReferenceOtherInventoryId.Value,
                                            Name = referencesValue.ToString()
                                        };
                                        attr.Value = views;
                                    }
                                    if (referencesValue == null)
                                    {
                                        attr.Value = null;
                                    }
                                    var AllOther = _unitOfWork.OtherInSiteRepository
                                            .GetIncludeWhere(x => !x.Dismantle && x.SiteCode == siteCode,
                                                x => x.allOtherInventoryInst,
                                                x => x.allOtherInventoryInst.generator,
                                                x => x.allOtherInventoryInst.solar,
                                                x => x.allOtherInventoryInst.cabinet)?.ToList();


                                    if (AllOther != null && AllOther.Any())
                                    {
                                        options = AllOther.SelectMany(item =>
                                        {
                                            var innerOptions = new List<SupportTypeImplementedViewModel>();

                                            if (item.allOtherInventoryInst.generator != null)
                                            {
                                                SupportTypeImplementedViewModel civilWithLegsOption = new SupportTypeImplementedViewModel()
                                                {
                                                    Id = item.allOtherInventoryInst.Id,
                                                    Name = item.allOtherInventoryInst.generator?.Name
                                                };

                                                innerOptions.Add(civilWithLegsOption);
                                            }

                                            if (item.allOtherInventoryInst.solar != null && item.allOtherInventoryInst.solarId != SolarId)
                                            {
                                                SupportTypeImplementedViewModel civilWithoutLegOption = new SupportTypeImplementedViewModel()
                                                {
                                                    Id = item.allOtherInventoryInst.Id,
                                                    Name = item.allOtherInventoryInst.solar?.Name
                                                };
                                                innerOptions.Add(civilWithoutLegOption);
                                            }

                                            if (item.allOtherInventoryInst.cabinet != null )
                                            {
                                                SupportTypeImplementedViewModel civilNonSteelOption = new SupportTypeImplementedViewModel()
                                                {
                                                    Id = item.allOtherInventoryInst.Id,
                                                    Name = item.allOtherInventoryInst.cabinet?.Name
                                                };
                                                innerOptions.Add(civilNonSteelOption);
                                            }

                                            return innerOptions;
                                        }).ToList();
                                    }
                                }
                                else
                                {

                                    var allOther = _unitOfWork.OtherInSiteRepository
                                        .GetIncludeWhere(x => !x.Dismantle && x.SiteCode == siteCode,
                                            x => x.allOtherInventoryInst,
                                            x => x.allOtherInventoryInst.generator,
                                            x => x.allOtherInventoryInst.solar,
                                            x => x.allOtherInventoryInst.cabinet)?.ToList();
                                    options = allOther.SelectMany(item =>
                                    {
                                        var innerOptions = new List<SupportTypeImplementedViewModel>();

                                        if (item.allOtherInventoryInst.generator != null)
                                        {
                                            SupportTypeImplementedViewModel civilWithLegsOption = new SupportTypeImplementedViewModel()
                                            {
                                                Id = item.allOtherInventoryInst.Id,
                                                Name = item.allOtherInventoryInst.generator?.Name
                                            };

                                            innerOptions.Add(civilWithLegsOption);
                                        }

                                        if (item.allOtherInventoryInst.solar != null && item.allOtherInventoryInst.solarId != SolarId)
                                        {
                                            SupportTypeImplementedViewModel civilWithoutLegOption = new SupportTypeImplementedViewModel()
                                            {
                                                Id = item.allOtherInventoryInst.Id,
                                                Name = item.allOtherInventoryInst.solar?.Name
                                            };
                                            innerOptions.Add(civilWithoutLegOption);
                                        }

                                        if (item.allOtherInventoryInst.cabinet != null )
                                        {
                                            SupportTypeImplementedViewModel civilNonSteelOption = new SupportTypeImplementedViewModel()
                                            {
                                                Id = item.allOtherInventoryInst.Id,
                                                Name = item.allOtherInventoryInst.cabinet?.Name
                                            };
                                            innerOptions.Add(civilNonSteelOption);
                                        }

                                        return innerOptions;
                                    }).ToList();
                                }
                                attr.Options = options;

                                return attr;
                            })
                            .ToList();
                    }
                    if (ExternalSys == true)
                    {
                        TLIhistory tLIhistory = new TLIhistory()
                        {
                            TablesNameId = TableNameEntity.Id,
                            ExternalSysId = UserId,
                            RecordId=SolarId.ToString(),
                            HistoryTypeId = 4,
                        };
                        _dbContext.TLIhistory.Add(tLIhistory);
                        _dbContext.SaveChanges();
                    }
                    return new Response<GetForAddOtherInventoryInstallationObject>(true, objectInst, null, null, (int)Helpers.Constants.ApiReturnCode.success);
                }
                else
                {
                    return new Response<GetForAddOtherInventoryInstallationObject>(false, null, null, "this civilwithlegs is not found ", (int)Helpers.Constants.ApiReturnCode.fail);
                }
            }

            catch (Exception err)
            {
                return new Response<GetForAddOtherInventoryInstallationObject>(false, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
        }
        public Response<GetForAddOtherInventoryInstallationObject> GetCabinetTelecomInstallationById(int CabinetTelecomId, string TableName)
        {
            try
            {
                TLItablesNames TableNameEntity = _unitOfWork.TablesNamesRepository
                    .GetWhereFirst(c => c.TableName.ToLower() == TableName.ToLower());

                GetForAddOtherInventoryInstallationObject objectInst = new GetForAddOtherInventoryInstallationObject();


                TLIotherInSite CabinetPowerInst = _unitOfWork.OtherInSiteRepository
                    .GetIncludeWhereFirst(x => x.allOtherInventoryInst.cabinetId == CabinetTelecomId && !x.Dismantle, x => x.allOtherInventoryInst.cabinet.RenewableCabinetType);
                if (CabinetPowerInst != null)
                {
                    EditCabinetTelecomLibraryAttributes CabinetTelecomLibrary = _mapper.Map<EditCabinetTelecomLibraryAttributes>(_unitOfWork.CabinetTelecomLibraryRepository
                        .GetIncludeWhereFirst(x => x.Id == CabinetPowerInst.allOtherInventoryInst.cabinet.CabinetTelecomLibraryId));

                    List<BaseInstAttViews> LibraryAttributes = _unitOfWork.AttributeActivatedRepository
                     .GetAttributeActivatedGetLibrary(Helpers.Constants.TablesNames.TLIcabinetTelecomLibrary.ToString(), CabinetTelecomLibrary, null).ToList();

                    var telecomtype_name = LibraryAttributes.FirstOrDefault(item => item.Label.ToLower() == "telecomtype_name");
                    if (telecomtype_name != null)
                    {
                        telecomtype_name.Options = _mapper.Map<List<TelecomTypeViewModel>>(_unitOfWork.TelecomTypeRepository.GetWhere(x => !x.Deleted && !x.Disable).ToList());
                        telecomtype_name.Value = _unitOfWork.TelecomTypeRepository != null && CabinetTelecomLibrary.TelecomTypeId != 0 ?
                            _mapper.Map<TelecomTypeViewModel>(_unitOfWork.TelecomTypeRepository.GetWhereFirst(x => x.Id == CabinetTelecomLibrary.TelecomTypeId)) :
                            null;
                    }

                    List<BaseInstAttViews> LogisticalAttributes = _mapper.Map<List<BaseInstAttViews>>(_unitOfWork.LogistcalRepository
                            .GetLogisticals(Helpers.Constants.TablePartName.OtherInventory.ToString(), Helpers.Constants.TablesNames.TLIcabinetTelecomLibrary.ToString(), CabinetTelecomLibrary.Id).ToList());

                    LibraryAttributes.AddRange(LogisticalAttributes);

                    objectInst.LibraryAttribute = LibraryAttributes;

                    List<BaseInstAttViews> ListAttributesActivated = _unitOfWork.AttributeActivatedRepository
                        .GetInstAttributeActivatedGetForAdd(Helpers.Constants.TablesNames.TLIcabinet.ToString(), CabinetPowerInst.allOtherInventoryInst.cabinet, "CabinetPowerLibraryId", "CabinetTelecomLibraryId").ToList();

                    BaseInstAttViews NameAttribute = ListAttributesActivated.FirstOrDefault(x => x.Key.ToLower() == "Name".ToLower());
                    if (NameAttribute != null)
                    {
                        BaseInstAttViews Swap = ListAttributesActivated[0];
                        ListAttributesActivated[ListAttributesActivated.IndexOf(NameAttribute)] = Swap;
                        ListAttributesActivated[0] = NameAttribute;
                        NameAttribute.Value = _dbContext.MV_CABINET_TELECOM_VIEW.FirstOrDefault(x => x.Id == CabinetTelecomId)?.Name;
                    }

                    var foreignKeyAttributes = ListAttributesActivated.Select(FKitem =>
                    {
                        switch (FKitem.Label.ToLower())
                        {
                            case "renewablecabinettype_name":
                                FKitem.Value = _mapper.Map<RenewableCabinetTypeViewModel>(CabinetPowerInst.allOtherInventoryInst.cabinet.RenewableCabinetType);
                                FKitem.Options = _mapper.Map<List<RenewableCabinetTypeViewModel>>(_unitOfWork.RenewableCabinetTypeRepository.GetWhere(x => !x.Deleted && !x.Disable).ToList());
                                break;

                        }
                        return FKitem;
                    }).ToList();


                    objectInst.InstallationAttributes = ListAttributesActivated;

                    objectInst.DynamicAttribute = _unitOfWork.DynamicAttInstValueRepository.
                        GetDynamicInstAtt(TableNameEntity.Id, CabinetTelecomId, null);

                    TLIallOtherInventoryInst allOtherInventoryInst = _unitOfWork.AllOtherInventoryInstRepository
                            .GetWhereFirst(x => x.cabinetId == CabinetTelecomId);
                    TLIotherInSite otherInSiteInfo = _dbContext.TLIotherInSite.FirstOrDefault(x => x.allOtherInventoryInstId == allOtherInventoryInst.Id);

                    List<BaseInstAttViews> otherInSiteAttributes = _unitOfWork.AttributeActivatedRepository
                         .GetAttributeActivatedGetForAdd(TablesNames.TLIotherInSite.ToString(), otherInSiteInfo, null, "allOtherInventoryInstId", "Id", "Dismantle", "SiteCode").ToList();

                    objectInst.OtherInSite = _mapper.Map<IEnumerable<BaseInstAttViews>>(otherInSiteAttributes);

                    TLIotherInventoryDistance otherInventoryDistance = _unitOfWork.OtherInventoryDistanceRepository.GetWhereFirst(x => x.allOtherInventoryInstId == allOtherInventoryInst.Id);

                    List<BaseInstAttViews> otherInventorytDistanceAttributes = _unitOfWork.AttributeActivatedRepository
                        .GetAttributeActivatedGetForAdd(TablesNames.TLIotherInventoryDistance.ToString(), otherInventoryDistance, null, "allOtherInventoryInstId", "Id", "Dismantle", "SiteCode").ToList();

                    string siteCode = _unitOfWork.OtherInSiteRepository
                     .GetIncludeWhereFirst(
                         x => x.allOtherInventoryInst.cabinet.Id == CabinetTelecomId && !x.Dismantle && !x.allOtherInventoryInst.Draft,
                         x => x.allOtherInventoryInst, x => x.allOtherInventoryInst.generator, x => x.allOtherInventoryInst.solar, x => x.allOtherInventoryInst.cabinet
                     )?.SiteCode;
                    objectInst.OtherInventoryDistance = otherInventorytDistanceAttributes;
                    if (siteCode != null)
                    {
                        var listAttributes = objectInst.OtherInventoryDistance
                            .Where(attr => attr.DataType.ToLower() == "list" && attr.Key.ToLower() == "referenceotherinventoryid")
                            .Select(attr =>
                            {
                                TLIotherInventoryDistance support = new TLIotherInventoryDistance();
                                var options = new List<SupportTypeImplementedViewModel>();
                                var Value = new SupportTypeImplementedViewModel();
                                if (otherInventoryDistance != null)
                                {
                                    support = _unitOfWork.OtherInventoryDistanceRepository
                                       .GetWhereFirst(x => x.allOtherInventoryInstId == otherInventoryDistance.allOtherInventoryInstId);
                                }
                                else
                                {
                                    support = null;
                                }
                                if (support != null && support.ReferenceOtherInventoryId != null)
                                {
                                    var supportReferenceAllCivilInst = _unitOfWork.AllOtherInventoryInstRepository
                                        .GetIncludeWhereFirst(x => x.Id == support.ReferenceOtherInventoryId,
                                            x => x.generator,
                                            x => x.solar,
                                            x => x.cabinet);
                                    object referencesValue = new object[0];
                                    if (supportReferenceAllCivilInst != null)
                                    {
                                        if (supportReferenceAllCivilInst.generatorId != null)
                                        {
                                            referencesValue = _dbContext.MV_GENERATOR_VIEW.FirstOrDefault(x => x.Id == supportReferenceAllCivilInst.generatorId)?.Name;
                                        }
                                        else if (supportReferenceAllCivilInst.solarId != null)
                                        {
                                            referencesValue = _dbContext.MV_SOLAR_VIEW.FirstOrDefault(x => x.Id == supportReferenceAllCivilInst.solarId)?.Name;
                                        }
                                        else if (supportReferenceAllCivilInst.cabinet.CabinetPowerLibraryId != null)
                                        {
                                            referencesValue = _dbContext.MV_CABINET_POWER_VIEW.FirstOrDefault(x => x.Id == supportReferenceAllCivilInst.cabinetId)?.Name;
                                        }
                                        else if (supportReferenceAllCivilInst.cabinet.CabinetTelecomLibraryId != null)
                                        {
                                            referencesValue = _dbContext.MV_CABINET_TELECOM_VIEW.FirstOrDefault(x => x.Id == supportReferenceAllCivilInst.cabinetId)?.Name;
                                        }

                                    }

                                    if (referencesValue != null && support.ReferenceOtherInventoryId != null && support.ReferenceOtherInventoryId.HasValue)
                                    {
                                        SupportTypeImplementedViewModel views = new SupportTypeImplementedViewModel()
                                        {
                                            Id = support.ReferenceOtherInventoryId.Value,
                                            Name = referencesValue.ToString()
                                        };
                                        attr.Value = views;
                                    }
                                    if (referencesValue == null)
                                    {
                                        attr.Value = null;
                                    }
                                    var AllOther = _unitOfWork.OtherInSiteRepository
                                            .GetIncludeWhere(x => !x.Dismantle && x.SiteCode == siteCode,
                                                x => x.allOtherInventoryInst,
                                                x => x.allOtherInventoryInst.generator,
                                                x => x.allOtherInventoryInst.solar,
                                                x => x.allOtherInventoryInst.cabinet)?.ToList();


                                    if (AllOther != null && AllOther.Any())
                                    {
                                        options = AllOther.SelectMany(item =>
                                        {
                                            var innerOptions = new List<SupportTypeImplementedViewModel>();

                                            if (item.allOtherInventoryInst.generator != null)
                                            {
                                                SupportTypeImplementedViewModel civilWithLegsOption = new SupportTypeImplementedViewModel()
                                                {
                                                    Id = item.allOtherInventoryInst.Id,
                                                    Name = item.allOtherInventoryInst.generator?.Name
                                                };

                                                innerOptions.Add(civilWithLegsOption);
                                            }

                                            if (item.allOtherInventoryInst.solar != null)
                                            {
                                                SupportTypeImplementedViewModel civilWithoutLegOption = new SupportTypeImplementedViewModel()
                                                {
                                                    Id = item.allOtherInventoryInst.Id,
                                                    Name = item.allOtherInventoryInst.solar?.Name
                                                };
                                                innerOptions.Add(civilWithoutLegOption);
                                            }

                                            if (item.allOtherInventoryInst.cabinet != null && item.allOtherInventoryInst.cabinet.Id != CabinetTelecomId)
                                            {
                                                SupportTypeImplementedViewModel civilNonSteelOption = new SupportTypeImplementedViewModel()
                                                {
                                                    Id = item.allOtherInventoryInst.Id,
                                                    Name = item.allOtherInventoryInst.cabinet?.Name
                                                };
                                                innerOptions.Add(civilNonSteelOption);
                                            }

                                            return innerOptions;
                                        }).ToList();
                                    }
                                }
                                else
                                {

                                    var allOther = _unitOfWork.OtherInSiteRepository
                                        .GetIncludeWhere(x => !x.Dismantle && x.SiteCode == siteCode,
                                            x => x.allOtherInventoryInst,
                                            x => x.allOtherInventoryInst.generator,
                                            x => x.allOtherInventoryInst.solar,
                                            x => x.allOtherInventoryInst.cabinet)?.ToList();
                                    options = allOther.SelectMany(item =>
                                    {
                                        var innerOptions = new List<SupportTypeImplementedViewModel>();

                                        if (item.allOtherInventoryInst.generator != null)
                                        {
                                            SupportTypeImplementedViewModel civilWithLegsOption = new SupportTypeImplementedViewModel()
                                            {
                                                Id = item.allOtherInventoryInst.Id,
                                                Name = item.allOtherInventoryInst.generator?.Name
                                            };

                                            innerOptions.Add(civilWithLegsOption);
                                        }

                                        if (item.allOtherInventoryInst.solar != null)
                                        {
                                            SupportTypeImplementedViewModel civilWithoutLegOption = new SupportTypeImplementedViewModel()
                                            {
                                                Id = item.allOtherInventoryInst.Id,
                                                Name = item.allOtherInventoryInst.solar?.Name
                                            };
                                            innerOptions.Add(civilWithoutLegOption);
                                        }

                                        if (item.allOtherInventoryInst.cabinet != null && item.allOtherInventoryInst.cabinet.Id != CabinetTelecomId)
                                        {
                                            SupportTypeImplementedViewModel civilNonSteelOption = new SupportTypeImplementedViewModel()
                                            {
                                                Id = item.allOtherInventoryInst.Id,
                                                Name = item.allOtherInventoryInst.cabinet?.Name
                                            };
                                            innerOptions.Add(civilNonSteelOption);
                                        }

                                        return innerOptions;
                                    }).ToList();
                                }
                                attr.Options = options;

                                return attr;
                            })
                            .ToList();
                    }
                    return new Response<GetForAddOtherInventoryInstallationObject>(true, objectInst, null, null, (int)Helpers.Constants.ApiReturnCode.success);
                }
                else
                {
                    return new Response<GetForAddOtherInventoryInstallationObject>(false, null, null, "this civilwithlegs is not found ", (int)Helpers.Constants.ApiReturnCode.fail);
                }
            }

            catch (Exception err)
            {
                return new Response<GetForAddOtherInventoryInstallationObject>(false, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
        }
        public Response<GetForAddOtherInventoryInstallationObject> GetCabinetPowerInstallationById(int CabinetPowerId, string TableName)
        {
            try
            {
                TLItablesNames TableNameEntity = _unitOfWork.TablesNamesRepository
                    .GetWhereFirst(c => c.TableName.ToLower() == TableName.ToLower());

                GetForAddOtherInventoryInstallationObject objectInst = new GetForAddOtherInventoryInstallationObject();


                TLIotherInSite CabinetPowerInst = _unitOfWork.OtherInSiteRepository
                    .GetIncludeWhereFirst(x => x.allOtherInventoryInst.cabinetId == CabinetPowerId && !x.Dismantle, x => x.allOtherInventoryInst.cabinet.RenewableCabinetType);
                if (CabinetPowerInst != null)
                {
                    EditCabinetPowerLibraryAttributes CabinetPowerLibrary = _mapper.Map<EditCabinetPowerLibraryAttributes>(_unitOfWork.CabinetPowerLibraryRepository
                        .GetIncludeWhereFirst(x => x.Id == CabinetPowerInst.allOtherInventoryInst.cabinet.CabinetPowerLibraryId));

                    List<BaseInstAttViews> LibraryAttributes = _unitOfWork.AttributeActivatedRepository
                     .GetAttributeActivatedGetLibrary(Helpers.Constants.TablesNames.TLIcabinetPowerLibrary.ToString(), CabinetPowerLibrary, null).ToList();

                    var cabinetpowertype_name = LibraryAttributes.FirstOrDefault(item => item.Label.ToLower() == "cabinetpowertype_name");
                    if (cabinetpowertype_name != null)
                    {
                        cabinetpowertype_name.Options = _mapper.Map<List<CabinetPowerTypeViewModel>>(_unitOfWork.CabinetPowerTypeRepository.GetWhere(x => !x.Delete && !x.Disable).ToList());
                        cabinetpowertype_name.Value = _unitOfWork.CabinetPowerTypeRepository != null && CabinetPowerLibrary.CabinetPowerTypeId != 0 ?
                            _mapper.Map<CabinetPowerTypeViewModel>(_unitOfWork.CabinetPowerTypeRepository.GetWhereFirst(x => x.Id == CabinetPowerLibrary.CabinetPowerTypeId)) :
                            null;
                    }
                    var integratedwith = LibraryAttributes.FirstOrDefault(item => item.Label.ToLower() == "integratedwith");
                    if (integratedwith != null)
                    {
                        List<EnumOutPut> integratedwiths = new List<EnumOutPut>();
                        integratedwiths.Add(new EnumOutPut
                        {
                            Id = (int)IntegratedWith.Solar,
                            Name = IntegratedWith.Solar.ToString()
                        });
                        integratedwiths.Add(new EnumOutPut
                        {
                            Id = (int)IntegratedWith.Wind,
                            Name = IntegratedWith.Wind.ToString()
                        });

                        if (CabinetPowerLibrary.IntegratedWith != null)
                        {
                            integratedwith.Options = integratedwiths;
                            integratedwith.Value = integratedwiths.FirstOrDefault(shape => shape.Id == (int)CabinetPowerLibrary.IntegratedWith);
                        }
                        else
                        {
                            integratedwith.Options = integratedwiths;
                            integratedwith.Value = null;

                        }
                    }
                    LibraryAttributes = LibraryAttributes.Select(x =>
                    {
                        if (x.Label.ToLower() == "powerintegrated" && (x.Value as bool?) == false)
                        {
                            LibraryAttributes.ForEach(y =>
                            {
                                if (y.Label.ToLower() == "integratedwith")
                                {
                                    y.visible = false;
                                }
                            });
                        }
                        return x;
                    }).ToList();
                    List<BaseInstAttViews> LogisticalAttributes = _mapper.Map<List<BaseInstAttViews>>(_unitOfWork.LogistcalRepository
                            .GetLogisticals(Helpers.Constants.TablePartName.OtherInventory.ToString(), Helpers.Constants.TablesNames.TLIcabinetPowerLibrary.ToString(), CabinetPowerLibrary.Id).ToList());

                    LibraryAttributes.AddRange(LogisticalAttributes);

                    objectInst.LibraryAttribute = LibraryAttributes;

                    List<BaseInstAttViews> ListAttributesActivated = _unitOfWork.AttributeActivatedRepository
                        .GetInstAttributeActivatedGetForAdd(Helpers.Constants.TablesNames.TLIcabinet.ToString(), CabinetPowerInst.allOtherInventoryInst.cabinet, "CabinetPowerLibraryId", "CabinetTelecomLibraryId").ToList();

                    BaseInstAttViews NameAttribute = ListAttributesActivated.FirstOrDefault(x => x.Key.ToLower() == "Name".ToLower());
                    if (NameAttribute != null)
                    {
                        BaseInstAttViews Swap = ListAttributesActivated[0];
                        ListAttributesActivated[ListAttributesActivated.IndexOf(NameAttribute)] = Swap;
                        ListAttributesActivated[0] = NameAttribute;
                        NameAttribute.Value = _dbContext.MV_CABINET_POWER_VIEW.FirstOrDefault(x => x.Id == CabinetPowerId)?.Name;
                    }

                    var foreignKeyAttributes = ListAttributesActivated.Select(FKitem =>
                    {
                        switch (FKitem.Label.ToLower())
                        {
                            case "renewablecabinettype_name":
                                FKitem.Value = _mapper.Map<RenewableCabinetTypeViewModel>(CabinetPowerInst.allOtherInventoryInst.cabinet.RenewableCabinetType);
                                FKitem.Options = _mapper.Map<List<RenewableCabinetTypeViewModel>>(_unitOfWork.RenewableCabinetTypeRepository.GetWhere(x => !x.Deleted && !x.Disable).ToList());
                                break;
                            case "batterrytype_name":
                                FKitem.Value = _mapper.Map<BatterryTypeDto>(CabinetPowerInst.allOtherInventoryInst.cabinet.BatterryType);
                                FKitem.Options = _mapper.Map<List<BatterryTypeDto>>(_unitOfWork.BatteryTypeRepository.GetWhere(x => !x.Delete && !x.Disable).ToList());
                                break;

                        }
                        return FKitem;
                    }).ToList();

                  
                    objectInst.InstallationAttributes = ListAttributesActivated;

                    objectInst.DynamicAttribute = _unitOfWork.DynamicAttInstValueRepository.
                        GetDynamicInstAtt(TableNameEntity.Id, CabinetPowerId, null);

                    TLIallOtherInventoryInst allOtherInventoryInst = _unitOfWork.AllOtherInventoryInstRepository
                            .GetWhereFirst(x => x.cabinetId == CabinetPowerId);
                    TLIotherInSite otherInSiteInfo = _dbContext.TLIotherInSite.FirstOrDefault(x => x.allOtherInventoryInstId == allOtherInventoryInst.Id);

                    List<BaseInstAttViews> otherInSiteAttributes = _unitOfWork.AttributeActivatedRepository
                         .GetAttributeActivatedGetForAdd(TablesNames.TLIotherInSite.ToString(), otherInSiteInfo, null, "allOtherInventoryInstId", "Id", "Dismantle", "SiteCode").ToList();

                    objectInst.OtherInSite = _mapper.Map<IEnumerable<BaseInstAttViews>>(otherInSiteAttributes);

                    TLIotherInventoryDistance otherInventoryDistance = _unitOfWork.OtherInventoryDistanceRepository.GetWhereFirst(x => x.allOtherInventoryInstId == allOtherInventoryInst.Id);

                    List<BaseInstAttViews> otherInventorytDistanceAttributes = _unitOfWork.AttributeActivatedRepository
                        .GetAttributeActivatedGetForAdd(TablesNames.TLIotherInventoryDistance.ToString(), otherInventoryDistance, null, "allOtherInventoryInstId", "Id", "Dismantle", "SiteCode").ToList();

                    string siteCode = _unitOfWork.OtherInSiteRepository
                     .GetIncludeWhereFirst(
                         x => x.allOtherInventoryInst.cabinet.Id == CabinetPowerId && !x.Dismantle && !x.allOtherInventoryInst.Draft,
                         x => x.allOtherInventoryInst, x => x.allOtherInventoryInst.generator, x => x.allOtherInventoryInst.solar, x => x.allOtherInventoryInst.cabinet
                     )?.SiteCode;
                    objectInst.OtherInventoryDistance = otherInventorytDistanceAttributes;

                    if (siteCode != null)
                    {
                        var listAttributes = objectInst.OtherInventoryDistance
                            .Where(attr => attr.DataType.ToLower() == "list" && attr.Key.ToLower() == "referenceotherinventoryid")
                            .Select(attr =>
                            {
                                TLIotherInventoryDistance support = new TLIotherInventoryDistance();
                                var options = new List<SupportTypeImplementedViewModel>();
                                var Value = new SupportTypeImplementedViewModel();
                                if (otherInventoryDistance != null)
                                {
                                     support = _unitOfWork.OtherInventoryDistanceRepository
                                        .GetWhereFirst(x => x.allOtherInventoryInstId == otherInventoryDistance.allOtherInventoryInstId);
                                }
                                else
                                {
                                     support = null;
                                }
                                if (support != null )
                                {
                                    var supportReferenceAllCivilInst = _unitOfWork.AllOtherInventoryInstRepository
                                        .GetIncludeWhereFirst(x => x.Id == support.ReferenceOtherInventoryId,
                                            x => x.generator,
                                            x => x.solar,
                                            x => x.cabinet);
                                    object referencesValue = new object[0];
                                    if (supportReferenceAllCivilInst != null)
                                    {
                                        if (supportReferenceAllCivilInst.generatorId != null)
                                        {
                                            referencesValue = _dbContext.MV_GENERATOR_VIEW.FirstOrDefault(x => x.Id == supportReferenceAllCivilInst.generatorId)?.Name;
                                        }
                                        else if (supportReferenceAllCivilInst.solarId != null)
                                        {
                                            referencesValue = _dbContext.MV_SOLAR_VIEW.FirstOrDefault(x => x.Id == supportReferenceAllCivilInst.solarId)?.Name;
                                        }
                                        else if (supportReferenceAllCivilInst.cabinet.CabinetPowerLibraryId != null)
                                        {
                                            referencesValue = _dbContext.MV_CABINET_POWER_VIEW.FirstOrDefault(x => x.Id == supportReferenceAllCivilInst.cabinetId )?.Name;
                                        }
                                        else if (supportReferenceAllCivilInst.cabinet.CabinetTelecomLibraryId != null)
                                        {
                                            referencesValue = _dbContext.MV_CABINET_TELECOM_VIEW.FirstOrDefault(x => x.Id == supportReferenceAllCivilInst.cabinetId)?.Name;
                                        }
                                       
                                    }

                                    if (referencesValue != null && support.ReferenceOtherInventoryId != null && support.ReferenceOtherInventoryId.HasValue)
                                    {
                                        SupportTypeImplementedViewModel views = new SupportTypeImplementedViewModel()
                                        {
                                            Id = support.ReferenceOtherInventoryId.Value,
                                            Name = referencesValue.ToString()
                                        };
                                        attr.Value = views;
                                    }
                                    if (referencesValue == null)
                                    {
                                        attr.Value = null;
                                    }
                                    var AllOther = _unitOfWork.OtherInSiteRepository
                                            .GetIncludeWhere(x => !x.Dismantle && x.SiteCode == siteCode,
                                                x => x.allOtherInventoryInst,
                                                x => x.allOtherInventoryInst.generator,
                                                x => x.allOtherInventoryInst.solar,
                                                x => x.allOtherInventoryInst.cabinet)?.ToList();


                                    if (AllOther != null && AllOther.Any())
                                    {
                                        options = AllOther.SelectMany(item =>
                                        {
                                            var innerOptions = new List<SupportTypeImplementedViewModel>();

                                            if (item.allOtherInventoryInst.generator != null)
                                            {
                                                SupportTypeImplementedViewModel civilWithLegsOption = new SupportTypeImplementedViewModel()
                                                {
                                                    Id = item.allOtherInventoryInst.Id,
                                                    Name = item.allOtherInventoryInst.generator?.Name
                                                };

                                                innerOptions.Add(civilWithLegsOption);
                                            }

                                            if (item.allOtherInventoryInst.solar != null)
                                            {
                                                SupportTypeImplementedViewModel civilWithoutLegOption = new SupportTypeImplementedViewModel()
                                                {
                                                    Id = item.allOtherInventoryInst.Id,
                                                    Name = item.allOtherInventoryInst.solar?.Name
                                                };
                                                innerOptions.Add(civilWithoutLegOption);
                                            }

                                            if (item.allOtherInventoryInst.cabinet != null && item.allOtherInventoryInst.cabinet.Id != CabinetPowerId)
                                            {
                                                SupportTypeImplementedViewModel civilNonSteelOption = new SupportTypeImplementedViewModel()
                                                {
                                                    Id = item.allOtherInventoryInst.Id,
                                                    Name = item.allOtherInventoryInst.cabinet?.Name
                                                };
                                                innerOptions.Add(civilNonSteelOption);
                                            }

                                            return innerOptions;
                                        }).ToList();
                                    }
                                }
                                else
                                {
                                  
                                    var allOther = _unitOfWork.OtherInSiteRepository
                                        .GetIncludeWhere(x => !x.Dismantle && x.SiteCode == siteCode,
                                            x => x.allOtherInventoryInst,
                                            x => x.allOtherInventoryInst.generator,
                                            x => x.allOtherInventoryInst.solar,
                                            x => x.allOtherInventoryInst.cabinet)?.ToList();
                                    options = allOther.SelectMany(item =>
                                    {
                                        var innerOptions = new List<SupportTypeImplementedViewModel>();

                                        if (item.allOtherInventoryInst.generator != null)
                                        {
                                            SupportTypeImplementedViewModel civilWithLegsOption = new SupportTypeImplementedViewModel()
                                            {
                                                Id = item.allOtherInventoryInst.Id,
                                                Name = item.allOtherInventoryInst.generator?.Name
                                            };

                                            innerOptions.Add(civilWithLegsOption);
                                        }

                                        if (item.allOtherInventoryInst.solar != null)
                                        {
                                            SupportTypeImplementedViewModel civilWithoutLegOption = new SupportTypeImplementedViewModel()
                                            {
                                                Id = item.allOtherInventoryInst.Id,
                                                Name = item.allOtherInventoryInst.solar?.Name
                                            };
                                            innerOptions.Add(civilWithoutLegOption);
                                        }

                                        if (item.allOtherInventoryInst.cabinet != null && item.allOtherInventoryInst.cabinet.Id != CabinetPowerId)
                                        {
                                            SupportTypeImplementedViewModel civilNonSteelOption = new SupportTypeImplementedViewModel()
                                            {
                                                Id = item.allOtherInventoryInst.Id,
                                                Name = item.allOtherInventoryInst.cabinet?.Name
                                            };
                                            innerOptions.Add(civilNonSteelOption);
                                        }

                                        return innerOptions;
                                    }).ToList();
                                }
                                attr.Options = options;

                                return attr;
                            })
                            .ToList();
                    }

                    return new Response<GetForAddOtherInventoryInstallationObject>(true, objectInst, null, null, (int)Helpers.Constants.ApiReturnCode.success);
                }
                else
                {
                    return new Response<GetForAddOtherInventoryInstallationObject>(false, null, null, "this civilwithlegs is not found ", (int)Helpers.Constants.ApiReturnCode.fail);
                }
            }

            catch (Exception err)
            {
                return new Response<GetForAddOtherInventoryInstallationObject>(false, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
        }
        public Response<ObjectInstAtts> GetById(int OtherInventoryInsId, string TableName)
        {
            try
            {
                TLItablesNames TableNameEntity = _unitOfWork.TablesNamesRepository.GetWhereFirst(c => c.TableName == TableName);
                ObjectInstAtts objectInst = new ObjectInstAtts();
                if (OtherInventoryType.TLIcabinet.ToString() == TableName)
                {
                    TLIcabinet CabinetInst = _unitOfWork.CabinetRepository
                        .GetIncludeWhereFirst(x => x.Id == OtherInventoryInsId, x => x.CabinetPowerLibrary,
                            x => x.CabinetTelecomLibrary, x => x.RenewableCabinetType);
                    string LibraryAttributeName = "";
                    if (CabinetInst.CabinetPowerLibraryId != null)
                    {
                        LibraryAttributeName = "CabinetTelecomLibraryId";
                        CabinetPowerLibraryViewModel CabinetPowerLibrary = _mapper.Map<CabinetPowerLibraryViewModel>(_unitOfWork.CabinetPowerLibraryRepository
                            .GetIncludeWhereFirst(x => x.Id == CabinetInst.CabinetPowerLibraryId, x => x.CabinetPowerType));

                        List<BaseAttView> LibraryAttributes = _unitOfWork.AttributeActivatedRepository
                            .GetAttributeActivated(TablesNames.TLIcabinetPowerLibrary.ToString(), CabinetPowerLibrary, null).ToList();

                        foreach (BaseAttView LibraryAttribute in LibraryAttributes)
                        {
                            if (LibraryAttribute.DataType.ToLower() == "list")
                            {
                                LibraryAttribute.Value = CabinetPowerLibrary.GetType().GetProperties()
                                    .FirstOrDefault(x => x.Name.ToLower() == LibraryAttribute.Label.ToLower()).GetValue(CabinetPowerLibrary);
                            }
                        }
                        List<BaseAttView> LogisticalAttributes = _mapper.Map<List<BaseAttView>>(_unitOfWork.LogistcalRepository
                            .GetLogistical(Helpers.Constants.TablePartName.OtherInventory.ToString(), TablesNames.TLIcabinetPowerLibrary.ToString(), CabinetPowerLibrary.Id).ToList());

                        LibraryAttributes.AddRange(LogisticalAttributes);

                        objectInst.LibraryActivatedAttributes = LibraryAttributes;
                    }
                    else if (CabinetInst.CabinetTelecomLibraryId != null)
                    {

                        LibraryAttributeName = "CabinetPowerLibraryId";
                        CabinetTelecomLibraryViewModel CabinetTelecomLibrary = _mapper.Map<CabinetTelecomLibraryViewModel>(_unitOfWork.CabinetTelecomLibraryRepository
                            .GetIncludeWhereFirst(x => x.Id == CabinetInst.CabinetTelecomLibraryId, x => x.TelecomType));

                        List<BaseAttView> LibraryAttributes = _unitOfWork.AttributeActivatedRepository
                            .GetAttributeActivated(TablesNames.TLIcabinetTelecomLibrary.ToString(), CabinetTelecomLibrary, null).ToList();

                        foreach (BaseAttView LibraryAttribute in LibraryAttributes)
                        {
                            if (LibraryAttribute.DataType.ToLower() == "list")
                            {
                                LibraryAttribute.Value = CabinetTelecomLibrary.GetType().GetProperties()
                                    .FirstOrDefault(x => x.Name.ToLower() == LibraryAttribute.Label.ToLower()).GetValue(CabinetTelecomLibrary);
                            }
                        }
                        List<BaseAttView> LogisticalAttributes = _mapper.Map<List<BaseAttView>>(_unitOfWork.LogistcalRepository
                            .GetLogistical(Helpers.Constants.TablePartName.OtherInventory.ToString(), TablesNames.TLIcabinetTelecomLibrary.ToString(), CabinetTelecomLibrary.Id).ToList());

                        LibraryAttributes.AddRange(LogisticalAttributes);

                        objectInst.LibraryActivatedAttributes = LibraryAttributes;
                    }

                    List<BaseInstAttView> ListAttributesActivated = _unitOfWork.AttributeActivatedRepository
                        .GetInstAttributeActivated(OtherInventoryType.TLIcabinet.ToString(), CabinetInst, LibraryAttributeName).ToList();

                    BaseInstAttView NameAttribute = ListAttributesActivated.FirstOrDefault(x => x.Key.ToLower() == "Name".ToLower());
                    if (NameAttribute != null)
                    {
                        BaseInstAttView Swap = ListAttributesActivated[0];
                        ListAttributesActivated[ListAttributesActivated.IndexOf(NameAttribute)] = Swap;
                        ListAttributesActivated[0] = NameAttribute;
                    }
                    foreach (BaseInstAttView FKitem in ListAttributesActivated)
                    {
                        if (FKitem.Desc.ToLower() == "tlirenewablecabinettype")
                        {
                            if (CabinetInst.RenewableCabinetType == null)
                                FKitem.Value = "NA";

                            else
                            {
                                FKitem.Value = CabinetInst.RenewableCabinetType.Name;
                            }
                        }
                        else if (CabinetInst.CabinetPowerLibraryId != null)
                        {
                            if (FKitem.Desc.ToLower() == "tlicabinetpowerlibrary")
                            {
                                if (CabinetInst.CabinetPowerLibrary == null)
                                    FKitem.Value = "NA";

                                else
                                    FKitem.Value = CabinetInst.CabinetPowerLibrary.Model;
                            }
                        }
                        else if (CabinetInst.CabinetTelecomLibraryId != null)
                        {
                            if (FKitem.Desc.ToLower() == "tlicabinettelecomlibrary")
                            {
                                if (CabinetInst.CabinetTelecomLibrary == null)
                                    FKitem.Value = "NA";

                                else
                                    FKitem.Value = CabinetInst.CabinetTelecomLibrary.Model;
                            }
                        }
                    }

                    objectInst.AttributesActivated = ListAttributesActivated;
                    objectInst.DynamicAtts = _unitOfWork.DynamicAttInstValueRepository
                        .GetDynamicInstAtts(TableNameEntity.Id, OtherInventoryInsId, null);

                    List<KeyValuePair<string, List<DropDownListFilters>>> RelatedTables = _unitOfWork.CabinetRepository.GetRelatedTables();

                    string SiteCode = _unitOfWork.OtherInSiteRepository
                        .GetIncludeWhereFirst(x => x.allOtherInventoryInst.cabinet.Id == OtherInventoryInsId && !x.Dismantle && !x.allOtherInventoryInst.Draft,
                            x => x.allOtherInventoryInst, x => x.allOtherInventoryInst.cabinet).SiteCode;

                    List<KeyValuePair<string, List<DropDownListFilters>>> OtherInventoryDistanceRelatedTables = _unitOfWork.OtherInventoryDistanceRepository.CabientGetRelatedTables(SiteCode);

                    RelatedTables.AddRange(OtherInventoryDistanceRelatedTables);

                    DropDownListFilters ReferenceToDelete = RelatedTables.FirstOrDefault(x => x.Key.ToLower() == "ReferenceOtherInventoryId".ToLower())
                        .Value.FirstOrDefault(x => x.Id == OtherInventoryInsId);

                    if (ReferenceToDelete != null)
                        RelatedTables.FirstOrDefault(x => x.Key.ToLower() == "ReferenceOtherInventoryId".ToLower()).Value.Remove(ReferenceToDelete);

                    objectInst.RelatedTables = RelatedTables;

                    TLIallOtherInventoryInst allOtherInventoryInst = _dbContext.TLIallOtherInventoryInst.FirstOrDefault(x => x.cabinetId == OtherInventoryInsId);
                    TLIotherInSite otherInSiteInfo = _dbContext.TLIotherInSite.FirstOrDefault(x => x.allOtherInventoryInstId == allOtherInventoryInst.Id);

                    List<BaseAttView> otherInSiteAttributes = _unitOfWork.AttributeActivatedRepository
                         .GetAttributeActivated(TablesNames.TLIotherInSite.ToString(), otherInSiteInfo, null, "allOtherInventoryInstId", "Id", "Dismantle", "SiteCode").ToList();

                    objectInst.OtherInSite = _mapper.Map<IEnumerable<BaseInstAttView>>(otherInSiteAttributes);

                    TLIotherInventoryDistance otherInventoryDistance = _unitOfWork.OtherInventoryDistanceRepository.GetWhereFirst(x => x.allOtherInventoryInstId == allOtherInventoryInst.Id);

                    List<BaseAttView> otherInventorytDistanceAttributes = _unitOfWork.AttributeActivatedRepository
                        .GetAttributeActivated(TablesNames.TLIotherInventoryDistance.ToString(), otherInventoryDistance, null, "allOtherInventoryInstId", "Id", "Dismantle", "SiteCode").ToList();

                    int? ReferenceOtherInventoryInstId = null;

                    foreach (BaseAttView otherinventorytAttribute in otherInventorytDistanceAttributes)
                    {
                        if (otherinventorytAttribute.DataType.ToLower() == "list")
                        {
                            if (otherinventorytAttribute.Key.ToLower() == "referenceotherinventoryid" && otherInventoryDistance != null)
                            {
                                TLIallOtherInventoryInst SupportReferenceAllOtherInventory = _unitOfWork.AllOtherInventoryInstRepository
                                    .GetIncludeWhereFirst(x => x.Id == otherInventoryDistance.ReferenceOtherInventoryId,
                                        x => x.cabinet, x => x.solar, x => x.generator);

                                if (SupportReferenceAllOtherInventory != null)
                                {
                                    otherinventorytAttribute.Value = SupportReferenceAllOtherInventory.cabinetId != null ? SupportReferenceAllOtherInventory.cabinet.Name :
                                        (SupportReferenceAllOtherInventory.solarId != null ? SupportReferenceAllOtherInventory.solar.Name :
                                        SupportReferenceAllOtherInventory.generator.Name);

                                    ReferenceOtherInventoryInstId = SupportReferenceAllOtherInventory.Id;
                                }
                            }
                        }
                    }

                    objectInst.OtherInventoryDistance = _mapper.Map<IEnumerable<BaseInstAttView>>(otherInventorytDistanceAttributes);
                    if (ReferenceOtherInventoryInstId != null)
                        objectInst.OtherInventoryDistance.FirstOrDefault(x => x.Key.ToLower() == "ReferenceOtherInventoryId".ToLower()).Id = ReferenceOtherInventoryInstId.Value;
                }
                else if (OtherInventoryType.TLIsolar.ToString() == TableName)
                {
                    TLIsolar SolarInst = _unitOfWork.SolarRepository
                       .GetIncludeWhereFirst(x => x.Id == OtherInventoryInsId, x => x.SolarLibrary,
                           x => x.Cabinet);

                    SolarLibraryViewModel SolarLibrary = _mapper.Map<SolarLibraryViewModel>(_unitOfWork.SolarLibraryRepository
                       .GetIncludeWhereFirst(x => x.Id == SolarInst.SolarLibraryId, x => x.Capacity));

                    List<BaseAttView> LibraryAttributes = _unitOfWork.AttributeActivatedRepository
                        .GetAttributeActivated(TablesNames.TLIsolarLibrary.ToString(), SolarLibrary, null).ToList();

                    foreach (BaseAttView LibraryAttribute in LibraryAttributes)
                    {
                        if (LibraryAttribute.DataType.ToLower() == "list")
                        {
                            LibraryAttribute.Value = SolarLibrary.GetType().GetProperties()
                                .FirstOrDefault(x => x.Name.ToLower() == LibraryAttribute.Label.ToLower()).GetValue(SolarLibrary);
                        }
                    }
                    List<BaseAttView> LogisticalAttributes = _mapper.Map<List<BaseAttView>>(_unitOfWork.LogistcalRepository
                        .GetLogistical(Helpers.Constants.TablePartName.OtherInventory.ToString(), TablesNames.TLIsolarLibrary.ToString(), SolarLibrary.Id).ToList());

                    LibraryAttributes.AddRange(LogisticalAttributes);

                    objectInst.LibraryActivatedAttributes = LibraryAttributes;

                    List<BaseInstAttView> ListAttributesActivated = _unitOfWork.AttributeActivatedRepository
                        .GetInstAttributeActivated(OtherInventoryType.TLIsolar.ToString(), SolarInst).ToList();

                    BaseInstAttView NameAttribute = ListAttributesActivated.FirstOrDefault(x => x.Key.ToLower() == "Name".ToLower());
                    if (NameAttribute != null)
                    {
                        BaseInstAttView Swap = ListAttributesActivated[0];
                        ListAttributesActivated[ListAttributesActivated.IndexOf(NameAttribute)] = Swap;
                        ListAttributesActivated[0] = NameAttribute;
                    }
                    foreach (BaseInstAttView FKitem in ListAttributesActivated)
                    {
                        if (FKitem.Desc.ToLower() == "tlicabinet")
                        {
                            if (SolarInst.Cabinet == null)
                                FKitem.Value = "NA";

                            else
                                FKitem.Value = SolarInst.Cabinet.Name;
                        }
                        else if (FKitem.Desc.ToLower() == "tlisolarlibrary")
                        {
                            if (SolarInst.SolarLibrary == null)
                                FKitem.Value = "NA";

                            else
                                FKitem.Value = SolarInst.SolarLibrary.Model;
                        }
                    }

                    objectInst.AttributesActivated = ListAttributesActivated;
                    objectInst.DynamicAtts = _unitOfWork.DynamicAttInstValueRepository
                        .GetDynamicInstAtts(TableNameEntity.Id, OtherInventoryInsId, null);

                    string SiteCode = _unitOfWork.OtherInSiteRepository
                        .GetIncludeWhereFirst(x => x.allOtherInventoryInst.solar.Id == OtherInventoryInsId && !x.Dismantle && !x.allOtherInventoryInst.Draft,
                            x => x.allOtherInventoryInst, x => x.allOtherInventoryInst.solar).SiteCode;

                    List<KeyValuePair<string, List<DropDownListFilters>>> RelatedTables = _unitOfWork.SolarRepository.GetRelatedTables(SiteCode);

                    List<KeyValuePair<string, List<DropDownListFilters>>> OtherInventoryDistanceRelatedTables = _unitOfWork.OtherInventoryDistanceRepository.SolarGetRelatedTables(SiteCode);
                    RelatedTables.AddRange(OtherInventoryDistanceRelatedTables);

                    DropDownListFilters ReferenceToDelete = RelatedTables.FirstOrDefault(x => x.Key.ToLower() == "ReferenceOtherInventoryId".ToLower())
                        .Value.FirstOrDefault(x => x.Id == OtherInventoryInsId);

                    if (ReferenceToDelete != null)
                        RelatedTables.FirstOrDefault(x => x.Key.ToLower() == "ReferenceOtherInventoryId".ToLower()).Value.Remove(ReferenceToDelete);
                    objectInst.RelatedTables = RelatedTables;

                    TLIallOtherInventoryInst allOtherInventoryInst = _dbContext.TLIallOtherInventoryInst.FirstOrDefault(x => x.solarId == OtherInventoryInsId);
                    TLIotherInSite otherInSiteInfo = _dbContext.TLIotherInSite.FirstOrDefault(x => x.allOtherInventoryInstId == allOtherInventoryInst.Id);

                    List<BaseAttView> otherInSiteAttributes = _unitOfWork.AttributeActivatedRepository
                         .GetAttributeActivated(TablesNames.TLIotherInSite.ToString(), otherInSiteInfo, null, "allOtherInventoryInstId", "Id", "Dismantle", "SiteCode").ToList();

                    objectInst.OtherInSite = _mapper.Map<IEnumerable<BaseInstAttView>>(otherInSiteAttributes);

                    TLIotherInventoryDistance otherInventoryDistance = _unitOfWork.OtherInventoryDistanceRepository.GetWhereFirst(x => x.allOtherInventoryInstId == allOtherInventoryInst.Id);

                    List<BaseAttView> otherInventorytDistanceAttributes = _unitOfWork.AttributeActivatedRepository
                        .GetAttributeActivated(TablesNames.TLIotherInventoryDistance.ToString(), otherInventoryDistance, null, "allOtherInventoryInstId", "Id", "Dismantle", "SiteCode").ToList();

                    int? ReferenceOtherInventoryInstId = null;

                    foreach (BaseAttView otherinventorytAttribute in otherInventorytDistanceAttributes)
                    {
                        if (otherinventorytAttribute.DataType.ToLower() == "list")
                        {
                            if (otherinventorytAttribute.Key.ToLower() == "referenceotherinventoryid" && otherInventoryDistance != null)
                            {
                                TLIallOtherInventoryInst SupportReferenceAllOtherInventory = _unitOfWork.AllOtherInventoryInstRepository
                                    .GetIncludeWhereFirst(x => x.Id == otherInventoryDistance.ReferenceOtherInventoryId,
                                        x => x.cabinet, x => x.solar, x => x.generator);

                                if (SupportReferenceAllOtherInventory != null)
                                {
                                    otherinventorytAttribute.Value = SupportReferenceAllOtherInventory.cabinetId != null ? SupportReferenceAllOtherInventory.cabinet.Name :
                                        (SupportReferenceAllOtherInventory.solarId != null ? SupportReferenceAllOtherInventory.solar.Name :
                                        SupportReferenceAllOtherInventory.generator.Name);

                                    ReferenceOtherInventoryInstId = SupportReferenceAllOtherInventory.Id;
                                }
                            }
                        }
                    }

                    objectInst.OtherInventoryDistance = _mapper.Map<IEnumerable<BaseInstAttView>>(otherInventorytDistanceAttributes);
                    if (ReferenceOtherInventoryInstId != null)
                        objectInst.OtherInventoryDistance.FirstOrDefault(x => x.Key.ToLower() == "ReferenceOtherInventoryId".ToLower()).Id = ReferenceOtherInventoryInstId.Value;
                }
                else if (OtherInventoryType.TLIgenerator.ToString() == TableName)
                {
                    TLIgenerator GeneratorInst = _unitOfWork.GeneratorRepository
                       .GetIncludeWhereFirst(x => x.Id == OtherInventoryInsId, x => x.GeneratorLibrary,
                           x => x.BaseGeneratorType);

                    GeneratorLibraryViewModel GeneratorLibrary = _mapper.Map<GeneratorLibraryViewModel>(_unitOfWork
                     .GeneratorLibraryRepository.GetIncludeWhereFirst(x => x.Id == GeneratorInst.GeneratorLibraryId, x => x.Capacity));

                    List<BaseAttView> LibraryAttributes = _unitOfWork.AttributeActivatedRepository
                        .GetAttributeActivated(TablesNames.TLIgeneratorLibrary.ToString(), GeneratorLibrary, null).ToList();

                    foreach (BaseAttView LibraryAttribute in LibraryAttributes)
                    {
                        if (LibraryAttribute.DataType.ToLower() == "list")
                        {
                            LibraryAttribute.Value = GeneratorLibrary.GetType().GetProperties()
                                .FirstOrDefault(x => x.Name.ToLower() == LibraryAttribute.Label.ToLower()).GetValue(GeneratorLibrary);
                        }
                    }
                    List<BaseAttView> LogisticalAttributes = _mapper.Map<List<BaseAttView>>(_unitOfWork.LogistcalRepository
                      .GetLogistical(Helpers.Constants.TablePartName.OtherInventory.ToString(), TablesNames.TLIgeneratorLibrary.ToString(), GeneratorLibrary.Id).ToList());

                    LibraryAttributes.AddRange(LogisticalAttributes);

                    objectInst.LibraryActivatedAttributes = LibraryAttributes;

                    List<BaseInstAttView> ListAttributesActivated = _unitOfWork.AttributeActivatedRepository
                        .GetInstAttributeActivated(TablesNames.TLIgenerator.ToString(), GeneratorInst).ToList();

                    BaseInstAttView NameAttribute = ListAttributesActivated.FirstOrDefault(x => x.Key.ToLower() == "Name".ToLower());
                    if (NameAttribute != null)
                    {
                        BaseInstAttView Swap = ListAttributesActivated[0];
                        ListAttributesActivated[ListAttributesActivated.IndexOf(NameAttribute)] = Swap;
                        ListAttributesActivated[0] = NameAttribute;
                    }
                    foreach (BaseInstAttView FKitem in ListAttributesActivated)
                    {
                        if (FKitem.Desc.ToLower() == "tlibasegeneratortype")
                        {
                            if (GeneratorInst.BaseGeneratorType == null)
                                FKitem.Value = "NA";

                            else
                                FKitem.Value = GeneratorInst.BaseGeneratorType.Name;
                        }
                        if (FKitem.Desc.ToLower() == "tligeneratorlibrary")
                        {
                            if (GeneratorInst.GeneratorLibrary == null)
                                FKitem.Value = "NA";

                            else
                                FKitem.Value = GeneratorInst.GeneratorLibrary.Model;
                        }
                    }

                    objectInst.AttributesActivated = ListAttributesActivated;
                    objectInst.DynamicAtts = _unitOfWork.DynamicAttInstValueRepository
                        .GetDynamicInstAtts(TableNameEntity.Id, OtherInventoryInsId, null);

                    List<KeyValuePair<string, List<DropDownListFilters>>> RelatedTables = _unitOfWork.GeneratorRepository
                      .GetRelatedTables();

                    string SiteCode = _unitOfWork.OtherInSiteRepository
                        .GetIncludeWhereFirst(x => x.allOtherInventoryInst.generator.Id == OtherInventoryInsId && !x.Dismantle && !x.allOtherInventoryInst.Draft,
                            x => x.allOtherInventoryInst, x => x.allOtherInventoryInst.generator).SiteCode;

                    List<KeyValuePair<string, List<DropDownListFilters>>> OtherInventoryDistanceRelatedTables = _unitOfWork.OtherInventoryDistanceRepository.GeneratorGetRelatedTables(SiteCode);
                    RelatedTables.AddRange(OtherInventoryDistanceRelatedTables);

                    DropDownListFilters ReferenceToDelete = RelatedTables.FirstOrDefault(x => x.Key.ToLower() == "ReferenceOtherInventoryId".ToLower())
                        .Value.FirstOrDefault(x => x.Id == OtherInventoryInsId);

                    if (ReferenceToDelete != null)
                        RelatedTables.FirstOrDefault(x => x.Key.ToLower() == "ReferenceOtherInventoryId".ToLower()).Value.Remove(ReferenceToDelete);

                    objectInst.RelatedTables = RelatedTables;

                    TLIallOtherInventoryInst allOtherInventoryInst = _dbContext.TLIallOtherInventoryInst.FirstOrDefault(x => x.generatorId == OtherInventoryInsId);
                    TLIotherInSite otherInSiteInfo = _dbContext.TLIotherInSite.FirstOrDefault(x => x.allOtherInventoryInstId == allOtherInventoryInst.Id);

                    List<BaseAttView> otherInSiteAttributes = _unitOfWork.AttributeActivatedRepository
                         .GetAttributeActivated(TablesNames.TLIotherInSite.ToString(), otherInSiteInfo, null, "allOtherInventoryInstId", "Id", "Dismantle", "SiteCode").ToList();

                    objectInst.OtherInSite = _mapper.Map<IEnumerable<BaseInstAttView>>(otherInSiteAttributes);

                    TLIotherInventoryDistance otherInventoryDistance = _unitOfWork.OtherInventoryDistanceRepository.GetWhereFirst(x => x.allOtherInventoryInstId == allOtherInventoryInst.Id);

                    List<BaseAttView> otherInventorytDistanceAttributes = _unitOfWork.AttributeActivatedRepository
                        .GetAttributeActivated(TablesNames.TLIotherInventoryDistance.ToString(), otherInventoryDistance, null, "allOtherInventoryInstId", "Id", "Dismantle", "SiteCode").ToList();

                    int? ReferenceOtherInventoryInstId = null;

                    foreach (BaseAttView otherinventorytAttribute in otherInventorytDistanceAttributes)
                    {
                        if (otherinventorytAttribute.DataType.ToLower() == "list")
                        {
                            if (otherinventorytAttribute.Key.ToLower() == "referenceotherinventoryid" && otherInventoryDistance != null)
                            {
                                TLIallOtherInventoryInst SupportReferenceAllOtherInventory = _unitOfWork.AllOtherInventoryInstRepository
                                    .GetIncludeWhereFirst(x => x.Id == otherInventoryDistance.ReferenceOtherInventoryId,
                                        x => x.cabinet, x => x.solar, x => x.generator);

                                if (SupportReferenceAllOtherInventory != null)
                                {
                                    otherinventorytAttribute.Value = SupportReferenceAllOtherInventory.cabinetId != null ? SupportReferenceAllOtherInventory.cabinet.Name :
                                        (SupportReferenceAllOtherInventory.solarId != null ? SupportReferenceAllOtherInventory.solar.Name :
                                        SupportReferenceAllOtherInventory.generator.Name);

                                    ReferenceOtherInventoryInstId = SupportReferenceAllOtherInventory.Id;
                                }
                            }
                        }
                    }

                    objectInst.OtherInventoryDistance = _mapper.Map<IEnumerable<BaseInstAttView>>(otherInventorytDistanceAttributes);
                    if (ReferenceOtherInventoryInstId != null)
                        objectInst.OtherInventoryDistance.FirstOrDefault(x => x.Key.ToLower() == "ReferenceOtherInventoryId".ToLower()).Id = ReferenceOtherInventoryInstId.Value;
                }
                return new Response<ObjectInstAtts>(true, objectInst, null, null, (int)ApiReturnCode.success);
            }
            catch (Exception err)
            {
                return new Response<ObjectInstAtts>(true, null, null, err.Message, (int)ApiReturnCode.fail);
            }
        }
        #endregion
        # region Get Enabled Attributes Only With Dynamic Objects...
        public Response<ReturnWithFilters<object>> GetCabinetBySiteWithEnabledAtt(SiteBaseFilter BaseFilter, bool WithFilterData, CombineFilters CombineFilters, ParameterPagination parameterPagination, string LibraryType)
        {
            try
            {
                int Count = 0;
                List<object> OutPutList = new List<object>();
                ReturnWithFilters<object> CabinetTableDisplay = new ReturnWithFilters<object>();

                //
                // Get All OtherInSite To This BaseFilter + CombineFilters
                //
                List<TLIotherInSite> AllOtherInSiteRecords = GetOtherInSiteBySiteBaseFilter(BaseFilter, "Cabinet", CombineFilters, LibraryType);
                List<TLIotherInSite> OtherInSiteRecords = GetMaxInstallationDate(AllOtherInSiteRecords, "Cabinet");

                List<CabinetViewModel> Cabinets = _mapper.Map<List<CabinetViewModel>>(OtherInSiteRecords
                    .Select(x => x.allOtherInventoryInst.cabinet)).ToList();

                Count = Cabinets.Count();

                Cabinets = Cabinets.Skip((parameterPagination.PageNumber - 1) * parameterPagination.PageSize).
                    Take(parameterPagination.PageSize).ToList();

                List<TLIattributeViewManagment> DynamicAttributes = _mapper.Map<List<TLIattributeViewManagment>>(_unitOfWork.AttributeViewManagmentRepository.GetIncludeWhere(x =>
                    x.Enable && x.EditableManagmentView.View.ToLower() == EditableManamgmantViewNames.CabinetInstallation.ToString().ToLower() &&
                    x.DynamicAttId != null ? (x.DynamicAtt.tablesNames.TableName.ToLower() == TablesNames.TLIcabinet.ToString().ToLower() &&
                        !x.DynamicAtt.disable && !x.DynamicAtt.LibraryAtt) : false, x => x.EditableManagmentView, x => x.DynamicAtt,
                            x => x.DynamicAtt.tablesNames, x => x.DynamicAtt.DataType));

                List<TLIattributeViewManagment> AllAttributes = _unitOfWork.AttributeViewManagmentRepository.GetIncludeWhere(x =>
                    (x.Enable && x.EditableManagmentView.View == Helpers.Constants.EditableManamgmantViewNames.CabinetInstallation.ToString() &&
                    (LibraryType.ToLower() == "power" ?
                        !x.AttributeActivated.Key.ToLower().Contains("telecomlibrary") : !x.AttributeActivated.Key.ToLower().Contains("powerlibrary")) &&
                    (x.AttributeActivatedId != null ?
                        (x.AttributeActivated.Tabel == TablesNames.TLIcabinet.ToString() && x.AttributeActivated.enable) :
                        (!x.DynamicAtt.LibraryAtt && !x.DynamicAtt.disable && x.DynamicAtt.tablesNames.TableName == TablesNames.TLIcabinet.ToString()))) ||
                    (x.AttributeActivated != null ?
                        ((x.AttributeActivated.Key.ToLower() == "id" || x.AttributeActivated.Key.ToLower() == "active") && x.AttributeActivated.Tabel == TablesNames.TLIcabinet.ToString()) : false),
                       x => x.EditableManagmentView, x => x.EditableManagmentView.TLItablesNames1, x => x.EditableManagmentView.TLItablesNames2,
                       x => x.AttributeActivated, x => x.DynamicAtt, x => x.DynamicAtt.tablesNames, x => x.DynamicAtt.DataType).ToList();

                List<TLIattributeViewManagment> NotDateTimeInstallationAttributesViewModel = AllAttributes.Where(x =>
                    x.AttributeActivatedId != null ? (x.AttributeActivated.Key.ToLower() != "deleted" && x.AttributeActivated.DataType.ToLower() != "datetime") : false).ToList();

                List<TLIattributeViewManagment> NotDateTimeDynamicInstallationAttributesViewModel = DynamicAttributes.Where(x =>
                    x.DynamicAtt.DataType.Name.ToLower() != "datetime").ToList();

                List<TLIattributeViewManagment> DateTimeInstallationAttributesViewModel = AllAttributes.Where(x =>
                    x.AttributeActivatedId != null ? (x.AttributeActivated.Key.ToLower() != "deleted" && x.AttributeActivated.DataType.ToLower() == "datetime") : false).ToList();

                List<TLIattributeViewManagment> DateTimeDynamicInstallationAttributesViewModel = DynamicAttributes.Where(x =>
                    x.DynamicAtt.DataType.Name.ToLower() == "datetime").ToList();

                foreach (CabinetViewModel CabinetInstallationObject in Cabinets)
                {
                    dynamic DynamicCabinetInstallation = new ExpandoObject();

                    //
                    // Installation Object ViewModel...
                    //
                    if (NotDateTimeInstallationAttributesViewModel != null ? NotDateTimeInstallationAttributesViewModel.Count > 0 : false)
                    {
                        List<PropertyInfo> InstallationProps = typeof(CabinetViewModel).GetProperties().Where(x =>
                            x.PropertyType.GenericTypeArguments != null ?
                                (x.PropertyType.GenericTypeArguments.Count() > 0 ? x.PropertyType.GenericTypeArguments.FirstOrDefault().Name.ToLower() != "datetime" :
                                (x.PropertyType.Name.ToLower() != "datetime")) :
                            (x.PropertyType.Name.ToLower() != "datetime")).ToList();

                        foreach (PropertyInfo prop in InstallationProps)
                        {
                            if (prop.Name.ToLower().Contains("_name") &&
                                NotDateTimeInstallationAttributesViewModel.Select(x =>
                                    x.AttributeActivated.Label.ToLower()).Contains(prop.Name.ToLower()))
                            {
                                object ForeignKeyNamePropObject = prop.GetValue(CabinetInstallationObject, null);
                                ((IDictionary<String, Object>)DynamicCabinetInstallation).Add(new KeyValuePair<string, object>(prop.Name, ForeignKeyNamePropObject));
                            }
                            else if (NotDateTimeInstallationAttributesViewModel.Select(x =>
                                 x.AttributeActivated.Key.ToLower()).Contains(prop.Name.ToLower()) &&
                                !prop.Name.ToLower().Contains("_name") &&
                                (prop.Name.ToLower().Substring(Math.Max(0, prop.Name.Length - 2)) != "id" || prop.Name.ToLower() == "id"))
                            {
                                if (prop.Name.ToLower() != "id" && prop.Name.ToLower() != "active")
                                {
                                    TLIattributeViewManagment LabelName = AllAttributes.FirstOrDefault(x => ((x.AttributeActivated != null) ? x.AttributeActivated.Key == prop.Name : false) &&
                                        x.AttributeActivated.Tabel == TablesNames.TLIcabinet.ToString() &&
                                        x.Enable && x.AttributeActivated.DataType != "List" && x.Id != 0);

                                    if (LabelName != null)
                                    {
                                        object PropObject = prop.GetValue(CabinetInstallationObject, null);
                                        ((IDictionary<String, Object>)DynamicCabinetInstallation).Add(new KeyValuePair<string, object>(LabelName.AttributeActivated.Label, PropObject));
                                    }
                                }
                                else
                                {
                                    object PropObject = prop.GetValue(CabinetInstallationObject, null);
                                    ((IDictionary<String, Object>)DynamicCabinetInstallation).Add(new KeyValuePair<string, object>(prop.Name, PropObject));
                                }
                            }
                        }
                    }

                    //
                    // Installation Dynamic Attributes... (Not DateTime DataType Attribute)
                    //
                    if (NotDateTimeDynamicInstallationAttributesViewModel != null ? NotDateTimeDynamicInstallationAttributesViewModel.Count > 0 : false)
                    {
                        List<TLIdynamicAtt> NotDateTimeInstallationDynamicAttributes = _unitOfWork.DynamicAttRepository.GetIncludeWhere(x =>
                            !x.disable && x.tablesNames.TableName == TablesNames.TLIcabinet.ToString() &&
                            !x.LibraryAtt && x.DataType.Name.ToLower() != "datetime" &&
                            NotDateTimeDynamicInstallationAttributesViewModel.Select(y => y.DynamicAttId).Any(y => y == x.Id)
                                , x => x.tablesNames, x => x.DataType).ToList();

                        List<TLIdynamicAttInstValue> NotDateTimeDynamicAttInstValues = _unitOfWork.DynamicAttInstValueRepository.GetIncludeWhere(x =>
                            !x.DynamicAtt.LibraryAtt && !x.disable &&
                            x.InventoryId == CabinetInstallationObject.Id &&
                            NotDateTimeInstallationDynamicAttributes.AsEnumerable().Select(y => y.Key.ToLower()).Contains(x.DynamicAtt.Key.ToLower()) &&
                            x.tablesNames.TableName == TablesNames.TLIcabinet.ToString()
                                , x => x.DynamicAtt, x => x.tablesNames, x => x.DynamicAtt.DataType).ToList();

                        foreach (TLIdynamicAtt InstallationDynamicAtt in NotDateTimeInstallationDynamicAttributes)
                        {
                            TLIdynamicAttInstValue DynamicAttInstValue = NotDateTimeDynamicAttInstValues.FirstOrDefault(x =>
                                x.DynamicAtt.Key.ToLower() == InstallationDynamicAtt.Key.ToLower());

                            if (DynamicAttInstValue != null)
                            {
                                dynamic DynamicAttValue = new ExpandoObject();
                                if (DynamicAttInstValue.ValueString != null)
                                    DynamicAttValue = DynamicAttInstValue.ValueString;

                                else if (DynamicAttInstValue.ValueDouble != null)
                                    DynamicAttValue = DynamicAttInstValue.ValueDouble;

                                else if (DynamicAttInstValue.ValueDateTime != null)
                                    DynamicAttValue = DynamicAttInstValue.ValueDateTime;

                                else if (DynamicAttInstValue.ValueBoolean != null)
                                    DynamicAttValue = DynamicAttInstValue.ValueBoolean;

                                ((IDictionary<String, Object>)DynamicCabinetInstallation).Add(new KeyValuePair<string, object>(InstallationDynamicAtt.Key, DynamicAttValue));
                            }
                            else
                            {
                                ((IDictionary<String, Object>)DynamicCabinetInstallation).Add(new KeyValuePair<string, object>(InstallationDynamicAtt.Key, null));
                            }
                        }
                    }

                    //
                    // Installation Object ViewModel... (DateTime DataType Attribute)
                    //
                    dynamic DateTimeAttributes = new ExpandoObject();
                    if (DateTimeInstallationAttributesViewModel != null ? DateTimeInstallationAttributesViewModel.Count() > 0 : false)
                    {
                        List<PropertyInfo> DateTimeInstallationProps = typeof(CabinetViewModel).GetProperties().Where(x =>
                            x.PropertyType.GenericTypeArguments != null ?
                                (x.PropertyType.GenericTypeArguments.Count() > 0 ? x.PropertyType.GenericTypeArguments.FirstOrDefault().Name.ToLower() == "datetime" :
                                (x.PropertyType.Name.ToLower() == "datetime")) :
                            (x.PropertyType.Name.ToLower() == "datetime")).ToList();

                        foreach (PropertyInfo prop in DateTimeInstallationProps)
                        {
                            TLIattributeViewManagment LabelName = AllAttributes.FirstOrDefault(x => ((x.AttributeActivated != null) ? x.AttributeActivated.Key == prop.Name : false) &&
                                x.AttributeActivated.Tabel == TablesNames.TLIcabinet.ToString() &&
                                x.Enable && x.AttributeActivated.DataType != "List" && x.Id != 0);

                            if (LabelName != null)
                            {
                                object PropObject = prop.GetValue(CabinetInstallationObject, null);
                                ((IDictionary<String, Object>)DateTimeAttributes).Add(new KeyValuePair<string, object>(LabelName.AttributeActivated.Label, PropObject));
                            }
                        }
                    }

                    //
                    // Installation Dynamic Attributes... (DateTime DataType Attribute)
                    // 
                    if (DateTimeDynamicInstallationAttributesViewModel != null ? DateTimeDynamicInstallationAttributesViewModel.Count() > 0 : false)
                    {
                        List<TLIdynamicAtt> DateTimeInstallationDynamicAttributes = _unitOfWork.DynamicAttRepository.GetIncludeWhere(x =>
                           !x.disable && x.tablesNames.TableName == TablesNames.TLIcabinet.ToString() &&
                           !x.LibraryAtt && x.DataType.Name.ToLower() == "datetime" &&
                            DateTimeDynamicInstallationAttributesViewModel.Select(y => y.DynamicAttId).Any(y => y == x.Id), x => x.tablesNames).ToList();

                        List<TLIdynamicAttInstValue> DateTimeDynamicAttInstValues = _unitOfWork.DynamicAttInstValueRepository.GetIncludeWhere(x =>
                            x.InventoryId == CabinetInstallationObject.Id && !x.disable &&
                           !x.DynamicAtt.LibraryAtt &&
                            DateTimeInstallationDynamicAttributes.AsEnumerable().Select(y => y.Key.ToLower()).Contains(x.DynamicAtt.Key.ToLower()) &&
                            x.tablesNames.TableName == TablesNames.TLIcabinet.ToString()
                               , x => x.DynamicAtt, x => x.tablesNames, x => x.DynamicAtt.DataType).ToList();

                        foreach (TLIdynamicAtt InstallationDynamicAtt in DateTimeInstallationDynamicAttributes)
                        {
                            TLIdynamicAttInstValue DynamicAttInstallationValue = DateTimeDynamicAttInstValues.FirstOrDefault(x =>
                                x.DynamicAtt.Key.ToLower() == InstallationDynamicAtt.Key.ToLower());

                            if (DynamicAttInstallationValue != null)
                            {
                                dynamic DynamicAttValue = new ExpandoObject();
                                if (DynamicAttInstallationValue.ValueDateTime != null)
                                    DynamicAttValue = DynamicAttInstallationValue.ValueDateTime;

                                ((IDictionary<String, Object>)DateTimeAttributes).Add(new KeyValuePair<string, object>(InstallationDynamicAtt.Key, DynamicAttValue));
                            }
                            else
                            {
                                ((IDictionary<String, Object>)DateTimeAttributes).Add(new KeyValuePair<string, object>(InstallationDynamicAtt.Key, null));
                            }
                        }
                    }

                    ((IDictionary<String, Object>)DynamicCabinetInstallation).Add(new KeyValuePair<string, object>("DateTimeAttributes", DateTimeAttributes));

                    OutPutList.Add(DynamicCabinetInstallation);
                }
                CabinetTableDisplay.Model = OutPutList;

                if (WithFilterData)
                {
                    CabinetTableDisplay.filters = _unitOfWork.CabinetRepository.GetRelatedTables();
                }
                else
                {
                    CabinetTableDisplay.filters = null;
                }

                return new Response<ReturnWithFilters<object>>(true, CabinetTableDisplay, null, null, (int)ApiReturnCode.success, Count);
            }
            catch (Exception err)
            {
                return new Response<ReturnWithFilters<object>>(false, null, null, err.Message, (int)ApiReturnCode.fail);
            }
        }
        public Response<ReturnWithFilters<object>> GetSolarBySiteWithEnabledAtt(SiteBaseFilter BaseFilter, bool WithFilterData, CombineFilters CombineFilters, ParameterPagination parameterPagination)
        {
            try
            {
                int Count = 0;
                List<object> OutPutList = new List<object>();
                ReturnWithFilters<object> SolarTableDisplay = new ReturnWithFilters<object>();

                //
                // Get All OtherInSite To This BaseFilter + CombineFilters
                //
                List<TLIotherInSite> AllOtherInSiteRecords = GetOtherInSiteBySiteBaseFilter(BaseFilter, "Solar", CombineFilters, null);
                List<TLIotherInSite> OtherInSiteRecords = GetMaxInstallationDate(AllOtherInSiteRecords, "Solar");

                List<SolarViewModel> Solars = _mapper.Map<List<SolarViewModel>>(OtherInSiteRecords
                    .Select(x => x.allOtherInventoryInst.solar));

                Count = Solars.Count();

                Solars = Solars.Skip((parameterPagination.PageNumber - 1) * parameterPagination.PageSize).
                    Take(parameterPagination.PageSize).ToList();

                List<TLIattributeViewManagment> AllAttributes = _unitOfWork.AttributeViewManagmentRepository.GetIncludeWhere(x =>
                   (x.Enable && x.EditableManagmentView.View == Helpers.Constants.EditableManamgmantViewNames.SolarInstallation.ToString() &&
                   (x.AttributeActivatedId != null ?
                        (x.AttributeActivated.Tabel == TablesNames.TLIsolar.ToString() && x.AttributeActivated.enable) :
                        (!x.DynamicAtt.LibraryAtt && !x.DynamicAtt.disable && x.DynamicAtt.tablesNames.TableName == TablesNames.TLIsolar.ToString()))) ||
                    (x.AttributeActivated != null ?
                        ((x.AttributeActivated.Key.ToLower() == "id" || x.AttributeActivated.Key.ToLower() == "active") && x.AttributeActivated.Tabel == TablesNames.TLIsolar.ToString()) : false),
                       x => x.EditableManagmentView, x => x.EditableManagmentView.TLItablesNames1, x => x.EditableManagmentView.TLItablesNames2,
                       x => x.AttributeActivated, x => x.DynamicAtt, x => x.DynamicAtt.tablesNames, x => x.DynamicAtt.DataType).ToList();

                List<TLIattributeViewManagment> NotDateTimeInstallationAttributesViewModel = AllAttributes.Where(x =>
                    x.AttributeActivatedId != null ? (x.AttributeActivated.Key.ToLower() != "deleted" && x.AttributeActivated.DataType.ToLower() != "datetime") : false).ToList();

                List<TLIattributeViewManagment> NotDateTimeDynamicInstallationAttributesViewModel = AllAttributes.Where(x =>
                    x.DynamicAttId != null ? x.DynamicAtt.DataType.Name.ToLower() != "datetime" : false).ToList();

                List<TLIattributeViewManagment> DateTimeInstallationAttributesViewModel = AllAttributes.Where(x =>
                    x.AttributeActivatedId != null ? (x.AttributeActivated.Key.ToLower() != "deleted" && x.AttributeActivated.DataType.ToLower() == "datetime") : false).ToList();

                List<TLIattributeViewManagment> DateTimeDynamicInstallationAttributesViewModel = AllAttributes.Where(x =>
                    x.DynamicAttId != null ? x.DynamicAtt.DataType.Name.ToLower() == "datetime" : false).ToList();

                foreach (SolarViewModel SolarInstallationObject in Solars)
                {
                    dynamic DynamicSolarInstallation = new ExpandoObject();

                    //
                    // Installation Object ViewModel...
                    //
                    if (NotDateTimeInstallationAttributesViewModel != null ? NotDateTimeInstallationAttributesViewModel.Count > 0 : false)
                    {
                        List<PropertyInfo> InstallationProps = typeof(SolarViewModel).GetProperties().Where(x =>
                            x.PropertyType.GenericTypeArguments != null ?
                                (x.PropertyType.GenericTypeArguments.Count() > 0 ? x.PropertyType.GenericTypeArguments.FirstOrDefault().Name.ToLower() != "datetime" :
                                (x.PropertyType.Name.ToLower() != "datetime")) :
                            (x.PropertyType.Name.ToLower() != "datetime")).ToList();

                        foreach (PropertyInfo prop in InstallationProps)
                        {
                            if (prop.Name.ToLower().Contains("_name") &&
                                NotDateTimeInstallationAttributesViewModel.Select(x =>
                                    x.AttributeActivated.Label.ToLower()).Contains(prop.Name.ToLower()))
                            {
                                object ForeignKeyNamePropObject = prop.GetValue(SolarInstallationObject, null);
                                ((IDictionary<String, Object>)DynamicSolarInstallation).Add(new KeyValuePair<string, object>(prop.Name, ForeignKeyNamePropObject));
                            }
                            else if (NotDateTimeInstallationAttributesViewModel.Select(x =>
                                 x.AttributeActivated.Key.ToLower()).Contains(prop.Name.ToLower()) &&
                                !prop.Name.ToLower().Contains("_name") &&
                                (prop.Name.ToLower().Substring(Math.Max(0, prop.Name.Length - 2)) != "id" || prop.Name.ToLower() == "id"))
                            {
                                if (prop.Name.ToLower() != "id" && prop.Name.ToLower() != "active")
                                {
                                    TLIattributeViewManagment LabelName = AllAttributes.FirstOrDefault(x => ((x.AttributeActivated != null) ? x.AttributeActivated.Key == prop.Name : false) &&
                                        x.AttributeActivated.Tabel == TablesNames.TLIsolar.ToString() &&
                                        x.Enable && x.AttributeActivated.DataType != "List" && x.Id != 0);

                                    if (LabelName != null)
                                    {
                                        object PropObject = prop.GetValue(SolarInstallationObject, null);
                                        ((IDictionary<String, Object>)DynamicSolarInstallation).Add(new KeyValuePair<string, object>(LabelName.AttributeActivated.Label, PropObject));
                                    }
                                }
                                else
                                {
                                    object PropObject = prop.GetValue(SolarInstallationObject, null);
                                    ((IDictionary<String, Object>)DynamicSolarInstallation).Add(new KeyValuePair<string, object>(prop.Name, PropObject));
                                }
                            }
                        }
                    }

                    //
                    // Installation Dynamic Attributes... (Not DateTime DataType Attribute)
                    //
                    if (NotDateTimeDynamicInstallationAttributesViewModel != null ? NotDateTimeDynamicInstallationAttributesViewModel.Count > 0 : false)
                    {
                        List<TLIdynamicAtt> NotDateTimeInstallationDynamicAttributes = _unitOfWork.DynamicAttRepository.GetIncludeWhere(x =>
                            !x.disable && x.tablesNames.TableName == TablesNames.TLIsolar.ToString() &&
                            !x.LibraryAtt && x.DataType.Name.ToLower() != "datetime" &&
                            NotDateTimeDynamicInstallationAttributesViewModel.Select(y => y.DynamicAttId).Any(y => y == x.Id)
                                , x => x.tablesNames, x => x.DataType).ToList();

                        List<TLIdynamicAttInstValue> NotDateTimeDynamicAttInstValues = _unitOfWork.DynamicAttInstValueRepository.GetIncludeWhere(x =>
                            !x.DynamicAtt.LibraryAtt && !x.disable &&
                            x.InventoryId == SolarInstallationObject.Id &&
                            NotDateTimeInstallationDynamicAttributes.Select(y => y.Key.ToLower()).Contains(x.DynamicAtt.Key.ToLower()) &&
                            x.tablesNames.TableName == TablesNames.TLIsolar.ToString()
                                , x => x.DynamicAtt, x => x.tablesNames, x => x.DynamicAtt.DataType).ToList();

                        foreach (TLIdynamicAtt InstallationDynamicAtt in NotDateTimeInstallationDynamicAttributes)
                        {
                            TLIdynamicAttInstValue DynamicAttInstValue = NotDateTimeDynamicAttInstValues.FirstOrDefault(x =>
                                x.DynamicAtt.Key.ToLower() == InstallationDynamicAtt.Key.ToLower());

                            if (DynamicAttInstValue != null)
                            {
                                dynamic DynamicAttValue = new ExpandoObject();
                                if (DynamicAttInstValue.ValueString != null)
                                    DynamicAttValue = DynamicAttInstValue.ValueString;

                                else if (DynamicAttInstValue.ValueDouble != null)
                                    DynamicAttValue = DynamicAttInstValue.ValueDouble;

                                else if (DynamicAttInstValue.ValueDateTime != null)
                                    DynamicAttValue = DynamicAttInstValue.ValueDateTime;

                                else if (DynamicAttInstValue.ValueBoolean != null)
                                    DynamicAttValue = DynamicAttInstValue.ValueBoolean;

                                ((IDictionary<String, Object>)DynamicSolarInstallation).Add(new KeyValuePair<string, object>(InstallationDynamicAtt.Key, DynamicAttValue));
                            }
                            else
                            {
                                ((IDictionary<String, Object>)DynamicSolarInstallation).Add(new KeyValuePair<string, object>(InstallationDynamicAtt.Key, null));
                            }
                        }
                    }

                    //
                    // Installation Object ViewModel... (DateTime DataType Attribute)
                    //
                    dynamic DateTimeAttributes = new ExpandoObject();
                    if (DateTimeInstallationAttributesViewModel != null ? DateTimeInstallationAttributesViewModel.Count() > 0 : false)
                    {
                        List<PropertyInfo> DateTimeInstallationProps = typeof(SolarViewModel).GetProperties().Where(x =>
                            x.PropertyType.GenericTypeArguments != null ?
                                (x.PropertyType.GenericTypeArguments.Count() > 0 ? x.PropertyType.GenericTypeArguments.FirstOrDefault().Name.ToLower() == "datetime" :
                                (x.PropertyType.Name.ToLower() == "datetime")) :
                            (x.PropertyType.Name.ToLower() == "datetime")).ToList();

                        foreach (PropertyInfo prop in DateTimeInstallationProps)
                        {
                            TLIattributeViewManagment LabelName = AllAttributes.FirstOrDefault(x => ((x.AttributeActivated != null) ? x.AttributeActivated.Key == prop.Name : false) &&
                                x.AttributeActivated.Tabel == TablesNames.TLIsolar.ToString() &&
                                x.Enable && x.AttributeActivated.DataType != "List" && x.Id != 0);

                            if (LabelName != null)
                            {
                                object PropObject = prop.GetValue(SolarInstallationObject, null);
                                ((IDictionary<String, Object>)DateTimeAttributes).Add(new KeyValuePair<string, object>(LabelName.AttributeActivated.Label, PropObject));
                            }
                        }
                    }

                    //
                    // Installation Dynamic Attributes... (DateTime DataType Attribute)
                    // 
                    if (DateTimeDynamicInstallationAttributesViewModel != null ? DateTimeDynamicInstallationAttributesViewModel.Count() > 0 : false)
                    {
                        List<TLIdynamicAtt> DateTimeInstallationDynamicAttributes = _unitOfWork.DynamicAttRepository.GetIncludeWhere(x =>
                           !x.disable && x.tablesNames.TableName == TablesNames.TLIsolar.ToString() &&
                           !x.LibraryAtt && x.DataType.Name.ToLower() == "datetime" &&
                            DateTimeDynamicInstallationAttributesViewModel.Select(y => y.DynamicAttId).Any(y => y == x.Id), x => x.tablesNames).ToList();

                        List<TLIdynamicAttInstValue> DateTimeDynamicAttInstValues = _unitOfWork.DynamicAttInstValueRepository.GetIncludeWhere(x =>
                            x.InventoryId == SolarInstallationObject.Id && !x.disable &&
                           !x.DynamicAtt.LibraryAtt &&
                            DateTimeInstallationDynamicAttributes.Select(y => y.Key.ToLower()).Contains(x.DynamicAtt.Key.ToLower()) &&
                            x.tablesNames.TableName == TablesNames.TLIsolar.ToString()
                               , x => x.DynamicAtt, x => x.tablesNames, x => x.DynamicAtt.DataType).ToList();

                        foreach (TLIdynamicAtt InstallationDynamicAtt in DateTimeInstallationDynamicAttributes)
                        {
                            TLIdynamicAttInstValue DynamicAttInstallationValue = DateTimeDynamicAttInstValues.FirstOrDefault(x =>
                                x.DynamicAtt.Key.ToLower() == InstallationDynamicAtt.Key.ToLower());

                            if (DynamicAttInstallationValue != null)
                            {
                                dynamic DynamicAttValue = new ExpandoObject();
                                if (DynamicAttInstallationValue.ValueDateTime != null)
                                    DynamicAttValue = DynamicAttInstallationValue.ValueDateTime;

                                ((IDictionary<String, Object>)DateTimeAttributes).Add(new KeyValuePair<string, object>(InstallationDynamicAtt.Key, DynamicAttValue));
                            }
                            else
                            {
                                ((IDictionary<String, Object>)DateTimeAttributes).Add(new KeyValuePair<string, object>(InstallationDynamicAtt.Key, null));
                            }
                        }
                    }

                    ((IDictionary<String, Object>)DynamicSolarInstallation).Add(new KeyValuePair<string, object>("DateTimeAttributes", DateTimeAttributes));

                    OutPutList.Add(DynamicSolarInstallation);
                }
                SolarTableDisplay.Model = OutPutList;

                if (WithFilterData)
                {
                    SolarTableDisplay.filters = _unitOfWork.SolarRepository.GetRelatedTables(BaseFilter.SiteCode);
                }
                else
                {
                    SolarTableDisplay.filters = null;
                }

                return new Response<ReturnWithFilters<object>>(true, SolarTableDisplay, null, null, (int)ApiReturnCode.success, Count);
            }
            catch (Exception err)
            {
                return new Response<ReturnWithFilters<object>>(false, null, null, err.Message, (int)ApiReturnCode.fail);
            }
        }
        public Response<ReturnWithFilters<object>> GetGeneratorBySiteWithEnabledAtt(SiteBaseFilter BaseFilter, bool WithFilterData, CombineFilters CombineFilters, ParameterPagination parameterPagination)
        {
            try
            {
                int Count = 0;
                List<object> OutPutList = new List<object>();
                ReturnWithFilters<object> GeneratorTableDisplay = new ReturnWithFilters<object>();

                //
                // Get All OtherInSite To This BaseFilter + CombineFilters
                //
                List<TLIotherInSite> AllOtherInSiteRecords = GetOtherInSiteBySiteBaseFilter(BaseFilter, "Generator", CombineFilters, null);
                List<TLIotherInSite> OtherInSiteRecords = GetMaxInstallationDate(AllOtherInSiteRecords, "Generator");

                List<GeneratorViewModel> Generators = _mapper.Map<List<GeneratorViewModel>>(OtherInSiteRecords
                    .Select(x => x.allOtherInventoryInst.generator));

                Count = Generators.Count();

                Generators = Generators.Skip((parameterPagination.PageNumber - 1) * parameterPagination.PageSize).
                    Take(parameterPagination.PageSize).ToList();

                List<TLIattributeViewManagment> AllAttributes = _unitOfWork.AttributeViewManagmentRepository.GetIncludeWhere(x =>
                   (x.Enable && x.EditableManagmentView.View == Helpers.Constants.EditableManamgmantViewNames.GeneratorInstallation.ToString() &&
                   (x.AttributeActivatedId != null ?
                        (x.AttributeActivated.Tabel == TablesNames.TLIgenerator.ToString() && x.AttributeActivated.enable) :
                        (!x.DynamicAtt.LibraryAtt && !x.DynamicAtt.disable && x.DynamicAtt.tablesNames.TableName == TablesNames.TLIgenerator.ToString()))) ||
                    (x.AttributeActivated != null ?
                        ((x.AttributeActivated.Key.ToLower() == "id" || x.AttributeActivated.Key.ToLower() == "active") && x.AttributeActivated.Tabel == TablesNames.TLIgenerator.ToString()) : false),
                       x => x.EditableManagmentView, x => x.EditableManagmentView.TLItablesNames1, x => x.EditableManagmentView.TLItablesNames2,
                       x => x.AttributeActivated, x => x.DynamicAtt, x => x.DynamicAtt.tablesNames, x => x.DynamicAtt.DataType).ToList();

                List<TLIattributeViewManagment> NotDateTimeInstallationAttributesViewModel = AllAttributes.Where(x =>
                    x.AttributeActivatedId != null ? (x.AttributeActivated.Key.ToLower() != "deleted" && x.AttributeActivated.DataType.ToLower() != "datetime") : false).ToList();

                List<TLIattributeViewManagment> NotDateTimeDynamicInstallationAttributesViewModel = AllAttributes.Where(x =>
                    x.DynamicAttId != null ? x.DynamicAtt.DataType.Name.ToLower() != "datetime" : false).ToList();

                List<TLIattributeViewManagment> DateTimeInstallationAttributesViewModel = AllAttributes.Where(x =>
                    x.AttributeActivatedId != null ? (x.AttributeActivated.Key.ToLower() != "deleted" && x.AttributeActivated.DataType.ToLower() == "datetime") : false).ToList();

                List<TLIattributeViewManagment> DateTimeDynamicInstallationAttributesViewModel = AllAttributes.Where(x =>
                    x.DynamicAttId != null ? x.DynamicAtt.DataType.Name.ToLower() == "datetime" : false).ToList();

                foreach (GeneratorViewModel GeneratorInstallationObject in Generators)
                {
                    dynamic DynamicGeneratorInstallation = new ExpandoObject();

                    //
                    // Installation Object ViewModel...
                    //
                    if (NotDateTimeInstallationAttributesViewModel != null ? NotDateTimeInstallationAttributesViewModel.Count > 0 : false)
                    {
                        List<PropertyInfo> InstallationProps = typeof(GeneratorViewModel).GetProperties().Where(x =>
                            x.PropertyType.GenericTypeArguments != null ?
                                (x.PropertyType.GenericTypeArguments.Count() > 0 ? x.PropertyType.GenericTypeArguments.FirstOrDefault().Name.ToLower() != "datetime" :
                                (x.PropertyType.Name.ToLower() != "datetime")) :
                            (x.PropertyType.Name.ToLower() != "datetime")).ToList();

                        foreach (PropertyInfo prop in InstallationProps)
                        {
                            if (prop.Name.ToLower().Contains("_name") &&
                                NotDateTimeInstallationAttributesViewModel.Select(x =>
                                    x.AttributeActivated.Label.ToLower()).Contains(prop.Name.ToLower()))
                            {
                                object ForeignKeyNamePropObject = prop.GetValue(GeneratorInstallationObject, null);
                                ((IDictionary<String, Object>)DynamicGeneratorInstallation).Add(new KeyValuePair<string, object>(prop.Name, ForeignKeyNamePropObject));
                            }
                            else if (NotDateTimeInstallationAttributesViewModel.Select(x =>
                                 x.AttributeActivated.Key.ToLower()).Contains(prop.Name.ToLower()) &&
                                !prop.Name.ToLower().Contains("_name") &&
                                (prop.Name.ToLower().Substring(Math.Max(0, prop.Name.Length - 2)) != "id" || prop.Name.ToLower() == "id"))
                            {
                                if (prop.Name.ToLower() != "id" && prop.Name.ToLower() != "active")
                                {
                                    TLIattributeViewManagment LabelName = AllAttributes.FirstOrDefault(x => ((x.AttributeActivated != null) ? x.AttributeActivated.Key == prop.Name : false) &&
                                        x.AttributeActivated.Tabel == TablesNames.TLIgenerator.ToString() &&
                                        x.Enable && x.AttributeActivated.DataType != "List" && x.Id != 0);

                                    if (LabelName != null)
                                    {
                                        object PropObject = prop.GetValue(GeneratorInstallationObject, null);
                                        ((IDictionary<String, Object>)DynamicGeneratorInstallation).Add(new KeyValuePair<string, object>(LabelName.AttributeActivated.Label, PropObject));
                                    }
                                }
                                else
                                {
                                    object PropObject = prop.GetValue(GeneratorInstallationObject, null);
                                    ((IDictionary<String, Object>)DynamicGeneratorInstallation).Add(new KeyValuePair<string, object>(prop.Name, PropObject));
                                }
                            }
                        }
                    }

                    //
                    // Installation Dynamic Attributes... (Not DateTime DataType Attribute)
                    //
                    if (NotDateTimeDynamicInstallationAttributesViewModel != null ? NotDateTimeDynamicInstallationAttributesViewModel.Count > 0 : false)
                    {
                        List<TLIdynamicAtt> NotDateTimeInstallationDynamicAttributes = _unitOfWork.DynamicAttRepository.GetIncludeWhere(x =>
                            !x.disable && x.tablesNames.TableName == TablesNames.TLIgenerator.ToString() &&
                            !x.LibraryAtt && x.DataType.Name.ToLower() != "datetime" &&
                            NotDateTimeDynamicInstallationAttributesViewModel.Select(y => y.DynamicAttId).Any(y => y == x.Id)
                                , x => x.tablesNames, x => x.DataType).ToList();

                        List<TLIdynamicAttInstValue> NotDateTimeDynamicAttInstValues = _unitOfWork.DynamicAttInstValueRepository.GetIncludeWhere(x =>
                            !x.DynamicAtt.LibraryAtt && !x.disable &&
                            x.InventoryId == GeneratorInstallationObject.Id &&
                            NotDateTimeInstallationDynamicAttributes.Select(y => y.Key.ToLower()).Contains(x.DynamicAtt.Key.ToLower()) &&
                            x.tablesNames.TableName == TablesNames.TLIgenerator.ToString()
                                , x => x.DynamicAtt, x => x.tablesNames, x => x.DynamicAtt.DataType).ToList();

                        foreach (TLIdynamicAtt InstallationDynamicAtt in NotDateTimeInstallationDynamicAttributes)
                        {
                            TLIdynamicAttInstValue DynamicAttInstValue = NotDateTimeDynamicAttInstValues.FirstOrDefault(x =>
                                x.DynamicAtt.Key.ToLower() == InstallationDynamicAtt.Key.ToLower());

                            if (DynamicAttInstValue != null)
                            {
                                dynamic DynamicAttValue = new ExpandoObject();
                                if (DynamicAttInstValue.ValueString != null)
                                    DynamicAttValue = DynamicAttInstValue.ValueString;

                                else if (DynamicAttInstValue.ValueDouble != null)
                                    DynamicAttValue = DynamicAttInstValue.ValueDouble;

                                else if (DynamicAttInstValue.ValueDateTime != null)
                                    DynamicAttValue = DynamicAttInstValue.ValueDateTime;

                                else if (DynamicAttInstValue.ValueBoolean != null)
                                    DynamicAttValue = DynamicAttInstValue.ValueBoolean;

                                ((IDictionary<String, Object>)DynamicGeneratorInstallation).Add(new KeyValuePair<string, object>(InstallationDynamicAtt.Key, DynamicAttValue));
                            }
                            else
                            {
                                ((IDictionary<String, Object>)DynamicGeneratorInstallation).Add(new KeyValuePair<string, object>(InstallationDynamicAtt.Key, null));
                            }
                        }
                    }

                    //
                    // Installation Object ViewModel... (DateTime DataType Attribute)
                    //
                    dynamic DateTimeAttributes = new ExpandoObject();
                    if (DateTimeInstallationAttributesViewModel != null ? DateTimeInstallationAttributesViewModel.Count() > 0 : false)
                    {
                        List<PropertyInfo> DateTimeInstallationProps = typeof(GeneratorViewModel).GetProperties().Where(x =>
                            x.PropertyType.GenericTypeArguments != null ?
                                (x.PropertyType.GenericTypeArguments.Count() > 0 ? x.PropertyType.GenericTypeArguments.FirstOrDefault().Name.ToLower() == "datetime" :
                                (x.PropertyType.Name.ToLower() == "datetime")) :
                            (x.PropertyType.Name.ToLower() == "datetime")).ToList();

                        foreach (PropertyInfo prop in DateTimeInstallationProps)
                        {
                            TLIattributeViewManagment LabelName = AllAttributes.FirstOrDefault(x => ((x.AttributeActivated != null) ? x.AttributeActivated.Key == prop.Name : false) &&
                                x.AttributeActivated.Tabel == TablesNames.TLIgenerator.ToString() &&
                                x.Enable && x.AttributeActivated.DataType != "List" && x.Id != 0);

                            if (LabelName != null)
                            {
                                object PropObject = prop.GetValue(GeneratorInstallationObject, null);
                                ((IDictionary<String, Object>)DateTimeAttributes).Add(new KeyValuePair<string, object>(LabelName.AttributeActivated.Label, PropObject));
                            }
                        }
                    }

                    //
                    // Installation Dynamic Attributes... (DateTime DataType Attribute)
                    // 
                    if (DateTimeDynamicInstallationAttributesViewModel != null ? DateTimeDynamicInstallationAttributesViewModel.Count() > 0 : false)
                    {
                        List<TLIdynamicAtt> DateTimeInstallationDynamicAttributes = _unitOfWork.DynamicAttRepository.GetIncludeWhere(x =>
                           !x.disable && x.tablesNames.TableName == TablesNames.TLIgenerator.ToString() &&
                           !x.LibraryAtt && x.DataType.Name.ToLower() == "datetime" &&
                            DateTimeDynamicInstallationAttributesViewModel.Select(y => y.DynamicAttId).Any(y => y == x.Id), x => x.tablesNames).ToList();

                        List<TLIdynamicAttInstValue> DateTimeDynamicAttInstValues = _unitOfWork.DynamicAttInstValueRepository.GetIncludeWhere(x =>
                            x.InventoryId == GeneratorInstallationObject.Id && !x.disable &&
                           !x.DynamicAtt.LibraryAtt &&
                            DateTimeInstallationDynamicAttributes.Select(y => y.Key.ToLower()).Contains(x.DynamicAtt.Key.ToLower()) &&
                            x.tablesNames.TableName == TablesNames.TLIgenerator.ToString()
                               , x => x.DynamicAtt, x => x.tablesNames, x => x.DynamicAtt.DataType).ToList();

                        foreach (TLIdynamicAtt InstallationDynamicAtt in DateTimeInstallationDynamicAttributes)
                        {
                            TLIdynamicAttInstValue DynamicAttInstallationValue = DateTimeDynamicAttInstValues.FirstOrDefault(x =>
                                x.DynamicAtt.Key.ToLower() == InstallationDynamicAtt.Key.ToLower());

                            if (DynamicAttInstallationValue != null)
                            {
                                dynamic DynamicAttValue = new ExpandoObject();
                                if (DynamicAttInstallationValue.ValueDateTime != null)
                                    DynamicAttValue = DynamicAttInstallationValue.ValueDateTime;

                                ((IDictionary<String, Object>)DateTimeAttributes).Add(new KeyValuePair<string, object>(InstallationDynamicAtt.Key, DynamicAttValue));
                            }
                            else
                            {
                                ((IDictionary<String, Object>)DateTimeAttributes).Add(new KeyValuePair<string, object>(InstallationDynamicAtt.Key, null));
                            }
                        }
                    }

                    ((IDictionary<String, Object>)DynamicGeneratorInstallation).Add(new KeyValuePair<string, object>("DateTimeAttributes", DateTimeAttributes));

                    OutPutList.Add(DynamicGeneratorInstallation);
                }
                GeneratorTableDisplay.Model = OutPutList;

                if (WithFilterData)
                {
                    GeneratorTableDisplay.filters = _unitOfWork.GeneratorRepository.GetRelatedTables();
                }
                else
                {
                    GeneratorTableDisplay.filters = null;
                }

                return new Response<ReturnWithFilters<object>>(true, GeneratorTableDisplay, null, null, (int)ApiReturnCode.success, Count);
            }
            catch (Exception err)
            {
                return new Response<ReturnWithFilters<object>>(false, null, null, err.Message, (int)ApiReturnCode.fail);
            }
        }
        #endregion
        #region Helper Methods
        public List<TLIotherInSite> GetMaxInstallationDate(List<TLIotherInSite> Copy, string Kind)
        {
            var NewList = new List<TLIotherInSite>();
            if (Kind == "Cabinet")
            {
                foreach (var item in Copy)
                {
                    var CheckIfExist = NewList.FirstOrDefault(x => x.allOtherInventoryInst.cabinetId == item.allOtherInventoryInst.cabinetId);
                    if (CheckIfExist != null)
                    {
                        if (CheckIfExist.InstallationDate < item.InstallationDate)
                        {
                            NewList.Remove(CheckIfExist);
                            NewList.Add(item);
                        }
                    }
                    else
                        NewList.Add(item);
                }
            }
            else if (Kind == "Solar")
            {
                foreach (var item in Copy)
                {
                    var CheckIfExist = NewList.FirstOrDefault(x => x.allOtherInventoryInst.solarId == item.allOtherInventoryInst.solarId);
                    if (CheckIfExist != null)
                    {
                        if (CheckIfExist.InstallationDate < item.InstallationDate)
                        {
                            NewList.Remove(CheckIfExist);
                            NewList.Add(item);
                        }
                    }
                    else
                        NewList.Add(item);
                }
            }
            else if (Kind == "Generator")
            {
                foreach (var item in Copy)
                {
                    var CheckIfExist = NewList.FirstOrDefault(x => x.allOtherInventoryInst.generatorId == item.allOtherInventoryInst.generatorId);
                    if (CheckIfExist != null)
                    {
                        if (CheckIfExist.InstallationDate < item.InstallationDate)
                        {
                            NewList.Remove(CheckIfExist);
                            NewList.Add(item);
                        }
                    }
                    else
                        NewList.Add(item);
                }
            }
            return NewList;

        }
        public DynamicAttDto GetDynamicAttDto(TLIdynamicAttInstValue DynamicAttInstValueRecord, TLIdynamicAttLibValue DynamicAttLibRecord)
        {
            DynamicAttDto DynamicAttDto = new DynamicAttDto
            {
                DataType = new DataTypeViewModel(),
                DynamicAttInstValue = new DynamicAttInstValueViewModel(),
                DynamicAttLibValue = new DynamicAttLibValueViewMdodel()
            };

            if (DynamicAttLibRecord != null)
            {

                // Key
                DynamicAttDto.Key = DynamicAttLibRecord.DynamicAtt.Key;

                // DataType.Id + DataType.Name
                DynamicAttDto.DataType.Id = DynamicAttLibRecord.DynamicAtt.DataTypeId.Value;
                DynamicAttDto.DataType.Name = DynamicAttLibRecord.DynamicAtt.DataType.Name;

                // DynamicAttLibValue.Id
                DynamicAttDto.DynamicAttLibValue.Id = DynamicAttLibRecord.Id;

                // DynamicAttLibValue.Value
                DynamicAttDto.DynamicAttLibValue.Value = GetDynamicAttValue(null, DynamicAttLibRecord);

                // DynamicAttLibValue.DynamicAttId
                DynamicAttDto.DynamicAttLibValue.DynamicAttId = DynamicAttLibRecord.DynamicAttId;
                DynamicAttDto.DynamicAttInstValue = null;
            }
            else if (DynamicAttInstValueRecord != null)
            {

                // Key
                DynamicAttDto.Key = DynamicAttInstValueRecord.DynamicAtt.Key;

                // DataType.Id + DataType.Name
                DynamicAttDto.DataType.Id = DynamicAttInstValueRecord.DynamicAtt.DataTypeId.Value;
                DynamicAttDto.DataType.Name = DynamicAttInstValueRecord.DynamicAtt.DataType.Name;

                // DynamicAttInstValue.Id
                DynamicAttDto.DynamicAttInstValue.Id = DynamicAttInstValueRecord.Id;

                // DynamicAttInstValueRecord.ValueString
                DynamicAttDto.DynamicAttInstValue.Value = GetDynamicAttValue(DynamicAttInstValueRecord, null);

                // DynamicAttInstValue.DynamicAttId
                DynamicAttDto.DynamicAttInstValue.DynamicAttId = DynamicAttInstValueRecord.DynamicAttId;
                DynamicAttDto.DynamicAttLibValue = null;
            }
            return DynamicAttDto;

        }
        public string GetDynamicAttValue(TLIdynamicAttInstValue DynamicAttInstValueRecord, TLIdynamicAttLibValue DynamicAttLibRecord)
        {
            if (DynamicAttInstValueRecord != null)
            {
                if (DynamicAttInstValueRecord.ValueString != null) return DynamicAttInstValueRecord.ValueString;

                else if (DynamicAttInstValueRecord.ValueDouble != null) return DynamicAttInstValueRecord.ValueDouble.ToString();

                else if (DynamicAttInstValueRecord.ValueDateTime != null) return DynamicAttInstValueRecord.ValueDateTime.ToString();

                else if (DynamicAttInstValueRecord.ValueBoolean != null) return DynamicAttInstValueRecord.ValueBoolean.ToString();
            }
            else if (DynamicAttLibRecord != null)
            {
                if (DynamicAttLibRecord.ValueString != null)
                    return DynamicAttLibRecord.ValueString;

                else if (DynamicAttLibRecord.ValueDouble != null)
                    return DynamicAttLibRecord.ValueDouble.ToString();

                else if (DynamicAttLibRecord.ValueDateTime != null)
                    return DynamicAttLibRecord.ValueDateTime.ToString();

                else if (DynamicAttLibRecord.ValueBoolean != null)
                    return DynamicAttLibRecord.ValueBoolean.ToString();
            }
            return "";
        }
        public void GetInventoriesIdsFromDynamicAttributes(out List<int> DynamicInstValueListIds, List<TLIdynamicAtt> InstDynamicAttListIds, List<StringFilterObjectList> AttributeFilters)
        {
            try
            {
                List<StringFilterObjectList> DynamicInstAttributeFilters = AttributeFilters.Where(x =>
                    InstDynamicAttListIds.Select(y => y.Key.ToLower()).Contains(x.key.ToLower())).ToList();

                DynamicInstValueListIds = new List<int>();

                List<TLIdynamicAttInstValue> DynamicInstValueListObjects = _unitOfWork.DynamicAttInstValueRepository.GetIncludeWhere(x =>
                    InstDynamicAttListIds.Select(y => y.Id).Contains(x.DynamicAttId) && !x.disable).ToList();

                List<int> InventoriesIds = DynamicInstValueListObjects.Select(x => x.InventoryId).Distinct().ToList();

                foreach (int InventoryId in InventoriesIds)
                {
                    List<TLIdynamicAttInstValue> DynamicInstValueListInventories = DynamicInstValueListObjects.Where(x => x.InventoryId == InventoryId).ToList();

                    if (DynamicInstAttributeFilters.All(y => DynamicInstValueListInventories.Exists(x =>
                         (x.ValueBoolean != null) ?
                            (y.value.Any(z => x.ValueBoolean.ToString().ToLower().StartsWith(z.ToLower()))) :

                         (x.ValueDateTime != null ?
                            (y.value.Any(z => z.ToLower() == x.ValueDateTime.ToString().ToLower())) :

                         (x.ValueDouble != null ?
                            (y.value.Any(z => z.ToLower() == x.ValueDouble.ToString().ToLower())) :

                         (!string.IsNullOrEmpty(x.ValueString) ?
                            (y.value.Any(z => x.ValueString.ToLower().StartsWith(z.ToLower()))) : (false)))))))
                    {
                        DynamicInstValueListIds.Add(InventoryId);
                    }
                }
                return;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<TLIotherInSite> GetOtherInSiteBySiteBaseFilter(SiteBaseFilter BaseFilter, string Kind, CombineFilters CombineFilters, string LibraryType)
        {
            List<StringFilterObjectList> AttributeFilters = new List<StringFilterObjectList>();

            List<FilterObjectList> ObjectAttributeFilters = CombineFilters.filters;
            List<DateFilterViewModel> DateFilter = CombineFilters.DateFilter;

            List<AttributeActivatedViewModel> OtherInventoryInstallationAttribute = new List<AttributeActivatedViewModel>();

            if (Kind == "Cabinet")
            {
                if ((DateFilter != null ? DateFilter.Count() > 0 : false) ||
                    (ObjectAttributeFilters != null && ObjectAttributeFilters.Count > 0))
                {
                    OtherInventoryInstallationAttribute = _mapper.Map<List<AttributeActivatedViewModel>>(_unitOfWork.AttributeViewManagmentRepository.GetIncludeWhere(x =>
                        x.Enable && x.AttributeActivatedId != null &&
                        x.EditableManagmentView.View == Helpers.Constants.EditableManamgmantViewNames.CabinetInstallation.ToString() &&
                        x.EditableManagmentView.TLItablesNames1.TableName == TablesNames.TLIcabinet.ToString(),
                            x => x.AttributeActivated, x => x.EditableManagmentView, x => x.EditableManagmentView.TLItablesNames1,
                            x => x.EditableManagmentView.TLItablesNames2)
                    .Select(x => x.AttributeActivated).ToList());
                }
            }
            else if (Kind == "Solar")
            {
                if ((DateFilter != null ? DateFilter.Count() > 0 : false) ||
                    (ObjectAttributeFilters != null && ObjectAttributeFilters.Count > 0))
                {
                    OtherInventoryInstallationAttribute = _mapper.Map<List<AttributeActivatedViewModel>>(_unitOfWork.AttributeViewManagmentRepository.GetIncludeWhere(x =>
                        x.Enable && x.AttributeActivatedId != null &&
                        x.EditableManagmentView.View == Helpers.Constants.EditableManamgmantViewNames.SolarInstallation.ToString() &&
                        x.EditableManagmentView.TLItablesNames1.TableName == TablesNames.TLIsolar.ToString(),
                            x => x.AttributeActivated, x => x.EditableManagmentView, x => x.EditableManagmentView.TLItablesNames1,
                            x => x.EditableManagmentView.TLItablesNames2)
                    .Select(x => x.AttributeActivated).ToList());
                }
            }
            else if (Kind == "Generator")
            {
                if ((DateFilter != null ? DateFilter.Count() > 0 : false) ||
                    (ObjectAttributeFilters != null && ObjectAttributeFilters.Count > 0))
                {
                    OtherInventoryInstallationAttribute = _mapper.Map<List<AttributeActivatedViewModel>>(_unitOfWork.AttributeViewManagmentRepository.GetIncludeWhere(x =>
                        x.Enable && x.AttributeActivatedId != null &&
                        x.EditableManagmentView.View == Helpers.Constants.EditableManamgmantViewNames.GeneratorInstallation.ToString() &&
                        x.EditableManagmentView.TLItablesNames1.TableName == TablesNames.TLIgenerator.ToString(),
                            x => x.AttributeActivated, x => x.EditableManagmentView, x => x.EditableManagmentView.TLItablesNames1,
                            x => x.EditableManagmentView.TLItablesNames2)
                    .Select(x => x.AttributeActivated).ToList());
                }
            }

            if (ObjectAttributeFilters != null && ObjectAttributeFilters.Count > 0)
            {
                List<AttributeActivatedViewModel> NotDateOtherInventoryInstallationAttribute = OtherInventoryInstallationAttribute.Where(x =>
                    x.DataType.ToLower() != "datetime").ToList();

                foreach (FilterObjectList item in ObjectAttributeFilters)
                {
                    List<string> value = item.value.Select(x => x.ToString().ToLower()).ToList();

                    AttributeActivatedViewModel AttributeKey = NotDateOtherInventoryInstallationAttribute.FirstOrDefault(x =>
                        x.Label.ToLower() == item.key.ToLower());

                    string Key = "";
                    if (AttributeKey != null)
                        Key = AttributeKey.Key;

                    else
                        Key = item.key;

                    AttributeFilters.Add(new StringFilterObjectList
                    {
                        key = Key,
                        value = value
                    });
                }
            }

            //
            // DateTime Objects Filters..
            //
            List<DateFilterViewModel> AfterConvertDateFilters = new List<DateFilterViewModel>();
            if (DateFilter != null ? DateFilter.Count() > 0 : false)
            {
                List<AttributeActivatedViewModel> DateCivilInstallationAttribute = OtherInventoryInstallationAttribute.Where(x =>
                    x.DataType.ToLower() == "datetime").ToList();

                foreach (DateFilterViewModel item in DateFilter)
                {
                    DateTime DateFrom = Convert.ToDateTime(item.DateFrom);
                    DateTime DateTo = Convert.ToDateTime(item.DateTo);

                    if (DateFrom > DateTo)
                    {
                        DateTime Replacer = DateFrom;
                        DateFrom = DateTo;
                        DateTo = Replacer;
                    }

                    AttributeActivatedViewModel AttributeKey = DateCivilInstallationAttribute.FirstOrDefault(x =>
                        x.Label.ToLower() == item.key.ToLower());
                    string Key = "";

                    if (AttributeKey != null)
                        Key = AttributeKey.Key;
                    else
                        Key = item.key;

                    AfterConvertDateFilters.Add(new DateFilterViewModel
                    {
                        key = Key,
                        DateFrom = DateFrom,
                        DateTo = DateTo
                    });
                }
            }

            if (Kind == "Cabinet")
            {
                List<int> CabinetIds = new List<int>();
                List<int> WithoutDateFilterCabinetInstallation = new List<int>();
                List<int> WithDateFilterCabinetInstallation = new List<int>();

                if (AttributeFilters != null && AttributeFilters.Count > 0)
                {
                    //
                    // Installation Dynamic Attributes...
                    //
                    List<TLIdynamicAtt> InstDynamicAttListIds = _unitOfWork.DynamicAttRepository.GetIncludeWhere(x =>
                        AttributeFilters.AsEnumerable().Select(y => y.key.ToLower()).Contains(x.Key.ToLower()) &&
                        !x.LibraryAtt && !x.disable &&
                        x.tablesNames.TableName == TablesNames.TLIcabinet.ToString()
                            , x => x.tablesNames, x => x.DataType).ToList();

                    List<int> DynamicInstValueListIds = new List<int>();
                    bool DynamicInstExist = false;
                    if (InstDynamicAttListIds.Count() > 0)
                    {
                        DynamicInstExist = true;
                        GetInventoriesIdsFromDynamicAttributes(out DynamicInstValueListIds, InstDynamicAttListIds, AttributeFilters);
                    }

                    //
                    // Installation Attribute Activated...
                    //
                    bool AttrInstExist = typeof(CabinetViewModel).GetProperties().ToList().Exists(x =>
                        AttributeFilters.Where(y => y.key.ToLower() != "Id".ToLower()).Select(y =>
                            y.key.ToLower()).Contains(x.Name.ToLower()));

                    List<int> InstallationAttributeActivated = new List<int>();
                    if (AttrInstExist)
                    {
                        List<PropertyInfo> NotStringProps = typeof(CabinetViewModel).GetProperties().Where(x =>
                            x.PropertyType.Name.ToLower() != "string" &&
                            AttributeFilters.Select(y =>
                                y.key.ToLower()).Contains(x.Name.ToLower())).ToList();

                        List<PropertyInfo> StringProps = typeof(CabinetViewModel).GetProperties().Where(x =>
                            x.PropertyType.Name.ToLower() == "string" &&
                            AttributeFilters.Select(y =>
                                y.key.ToLower()).Contains(x.Name.ToLower())).ToList();

                        List<StringFilterObjectList> AttrInstAttributeFilters = AttributeFilters.Where(x =>
                            NotStringProps.Select(y => y.Name.ToLower()).Contains(x.key.ToLower()) ||
                            StringProps.Select(y => y.Name.ToLower()).Contains(x.key.ToLower())).ToList();

                        IEnumerable<TLIcabinet> Installations = _unitOfWork.CabinetRepository.GetAllWithoutCount();

                        foreach (StringFilterObjectList InstallationProp in AttrInstAttributeFilters)
                        {
                            if (StringProps.Select(x => x.Name.ToLower()).Contains(InstallationProp.key.ToLower()))
                            {
                                Installations = Installations.Where(x => StringProps.AsEnumerable().FirstOrDefault(y => (InstallationProp.key.ToLower() == y.Name.ToLower()) && (InstallationProp.value.AsEnumerable().FirstOrDefault(w =>
                                     y.GetValue(_mapper.Map<CabinetViewModel>(x), null) != null ? y.GetValue(_mapper.Map<CabinetViewModel>(x), null).ToString().ToLower().StartsWith(w.ToLower()) : false) != null)) != null).AsEnumerable();
                            }
                            else if (NotStringProps.Select(x => x.Name.ToLower()).Contains(InstallationProp.key.ToLower()))
                            {
                                Installations = Installations.Where(x => NotStringProps.AsEnumerable().FirstOrDefault(y => (InstallationProp.key.ToLower() == y.Name.ToLower()) && (y.GetValue(_mapper.Map<CabinetViewModel>(x), null) != null ?
                                    InstallationProp.value.AsEnumerable().Contains(y.GetValue(_mapper.Map<CabinetViewModel>(x), null).ToString().ToLower()) : false)) != null).AsEnumerable();
                            }
                        }

                        InstallationAttributeActivated = Installations.Select(x => x.Id).ToList();
                    }

                    //
                    // Installation (Attribute Activated + Dynamic) Attributes...
                    //
                    if (AttrInstExist && DynamicInstExist)
                    {
                        WithoutDateFilterCabinetInstallation = InstallationAttributeActivated.Intersect(DynamicInstValueListIds).ToList();
                    }
                    else if (AttrInstExist)
                    {
                        WithoutDateFilterCabinetInstallation = InstallationAttributeActivated;
                    }
                    else if (DynamicInstExist)
                    {
                        WithoutDateFilterCabinetInstallation = DynamicInstValueListIds;
                    }
                }

                if (DateFilter != null ? DateFilter.Count() > 0 : false)
                {
                    List<TLIdynamicAtt> DateTimeInstDynamicAttListIds = _unitOfWork.DynamicAttRepository.GetIncludeWhere(x =>
                        AfterConvertDateFilters.AsEnumerable().Select(y => y.key.ToLower()).Contains(x.Key.ToLower()) &&
                        !x.LibraryAtt && !x.disable &&
                        x.tablesNames.TableName == TablesNames.TLIcabinet.ToString()
                            , x => x.tablesNames, x => x.DataType).ToList();

                    List<int> DynamicInstValueListIds = new List<int>();
                    bool DynamicInstExist = false;

                    if (DateTimeInstDynamicAttListIds.Count > 0)
                    {
                        DynamicInstExist = true;
                        List<DateFilterViewModel> DynamicInstAttributeFilters = AfterConvertDateFilters.Where(x =>
                            DateTimeInstDynamicAttListIds.AsEnumerable().Select(y => y.Key.ToLower()).Contains(x.key.ToLower())).ToList();

                        DynamicInstValueListIds = new List<int>();

                        List<TLIdynamicAttInstValue> DynamicInstValueListObjects = _unitOfWork.DynamicAttInstValueRepository.GetIncludeWhere(x =>
                            DateTimeInstDynamicAttListIds.Select(y => y.Id).Any(y => y == x.DynamicAttId) && !x.disable).ToList();

                        List<int> InventoriesIds = DynamicInstValueListObjects.Select(x => x.InventoryId).Distinct().ToList();

                        foreach (int InventoryId in InventoriesIds)
                        {
                            List<TLIdynamicAttInstValue> DynamicInstValueListInventories = DynamicInstValueListObjects.Where(x =>
                                x.InventoryId == InventoryId).ToList();

                            if (DynamicInstAttributeFilters.All(y => DynamicInstValueListInventories.Select(x => x.ValueDateTime).Any(x =>
                                (x != null ?
                                    (x >= y.DateFrom && x <= y.DateTo) : (false)))))
                            {
                                DynamicInstValueListIds.Add(InventoryId);
                            }
                        }
                    }

                    //
                    // Installation Attribute Activated...
                    //
                    List<PropertyInfo> InstallationProps = typeof(CabinetViewModel).GetProperties().Where(x =>
                        AfterConvertDateFilters.Select(y =>
                            y.key.ToLower()).Contains(x.Name.ToLower())).ToList();

                    List<int> InstallationAttributeActivatedIds = new List<int>();
                    bool AttrInstExist = false;

                    if (InstallationProps != null)
                    {
                        AttrInstExist = true;

                        List<DateFilterViewModel> InstallationPropsAttributeFilters = AfterConvertDateFilters.Where(x =>
                            InstallationProps.Select(y => y.Name.ToLower()).Contains(x.key.ToLower())).ToList();

                        IEnumerable<TLIcabinet> Installations = _unitOfWork.CabinetRepository.GetAllWithoutCount();

                        foreach (DateFilterViewModel InstallationProp in InstallationPropsAttributeFilters)
                        {
                            Installations = Installations.Where(x => InstallationProps.Exists(y => (InstallationProp.key.ToLower() == y.Name.ToLower()) && ((y.GetValue(_mapper.Map<CabinetViewModel>(x), null) != null) ?
                                ((InstallationProp.DateFrom.Date <= Convert.ToDateTime(y.GetValue(_mapper.Map<CabinetViewModel>(x), null)).Date) &&
                                    (InstallationProp.DateTo.Date >= Convert.ToDateTime(y.GetValue(_mapper.Map<CabinetViewModel>(x), null)).Date)) : (false))));
                        }

                        InstallationAttributeActivatedIds = Installations.Select(x => x.Id).ToList();
                    }

                    //
                    // Installation (Attribute Activated + Dynamic) Attributes...
                    //
                    if (AttrInstExist && DynamicInstExist)
                    {
                        WithDateFilterCabinetInstallation = InstallationAttributeActivatedIds.Intersect(DynamicInstValueListIds).ToList();
                    }
                    else if (AttrInstExist)
                    {
                        WithDateFilterCabinetInstallation = InstallationAttributeActivatedIds;
                    }
                    else if (DynamicInstExist)
                    {
                        WithDateFilterCabinetInstallation = DynamicInstValueListIds;
                    }
                }
                if ((DateFilter != null ? DateFilter.Count() > 0 : false) ||
                    (AttributeFilters != null ? AttributeFilters.Count() > 0 : false))
                {
                    if ((AttributeFilters != null ? AttributeFilters.Count > 0 : false) &&
                        (AfterConvertDateFilters != null ? AfterConvertDateFilters.Count() > 0 : false))
                    {
                        CabinetIds = WithoutDateFilterCabinetInstallation.Intersect(WithDateFilterCabinetInstallation).ToList();
                    }
                    else if (AttributeFilters != null ? AttributeFilters.Count > 0 : false)
                    {
                        CabinetIds = WithoutDateFilterCabinetInstallation;
                    }
                    else if (AfterConvertDateFilters != null ? AfterConvertDateFilters.Count() > 0 : false)
                    {
                        CabinetIds = WithDateFilterCabinetInstallation;
                    }

                    if (LibraryType.ToLower() == "telecom")
                    {
                        return _unitOfWork.OtherInSiteRepository.GetIncludeWhere(x => (
                            (x.SiteCode == BaseFilter.SiteCode) &&
                            (!x.Dismantle) &&
                            (BaseFilter.ItemStatusId != null ? (
                                x.allOtherInventoryInst != null ? (
                                    x.allOtherInventoryInst.ItemStatusId != null ? (
                                        x.allOtherInventoryInst.ItemStatusId == BaseFilter.ItemStatusId)
                                    : false)
                                : false)
                            : true) &&
                            (BaseFilter.TicketId != null ? (
                                x.allOtherInventoryInst != null ? (
                                    x.allOtherInventoryInst.TicketId != null ? (
                                        x.allOtherInventoryInst.TicketId == BaseFilter.TicketId)
                                    : false)
                                : false)
                            : true) &&
                            (x.allOtherInventoryInst.cabinetId != null) &&
                            (x.allOtherInventoryInst.Draft == false) &&
                            CabinetIds.Contains(x.allOtherInventoryInst.cabinetId.Value) &&
                            x.allOtherInventoryInst.cabinet.CabinetTelecomLibraryId != null
                        ),
                        x => x.allOtherInventoryInst, x => x.allOtherInventoryInst.cabinet,
                        x => x.allOtherInventoryInst.cabinet.CabinetTelecomLibrary, x => x.allOtherInventoryInst.cabinet.RenewableCabinetType).ToList();
                    }
                    else if (LibraryType.ToLower() == "power")
                    {
                        return _unitOfWork.OtherInSiteRepository.GetIncludeWhere(x => (
                            (x.SiteCode == BaseFilter.SiteCode) &&
                            (!x.Dismantle) &&
                            (BaseFilter.ItemStatusId != null ? (
                                x.allOtherInventoryInst != null ? (
                                    x.allOtherInventoryInst.ItemStatusId != null ? (
                                        x.allOtherInventoryInst.ItemStatusId == BaseFilter.ItemStatusId)
                                    : false)
                                : false)
                            : true) &&
                            (BaseFilter.TicketId != null ? (
                                x.allOtherInventoryInst != null ? (
                                    x.allOtherInventoryInst.TicketId != null ? (
                                        x.allOtherInventoryInst.TicketId == BaseFilter.TicketId)
                                    : false)
                                : false)
                            : true) &&
                            (x.allOtherInventoryInst.cabinetId != null) &&
                            (x.allOtherInventoryInst.Draft == false) &&
                            CabinetIds.Contains(x.allOtherInventoryInst.cabinetId.Value) &&
                            x.allOtherInventoryInst.cabinet.CabinetPowerLibraryId != null
                        ),
                        x => x.allOtherInventoryInst, x => x.allOtherInventoryInst.cabinet, x => x.allOtherInventoryInst.cabinet.CabinetPowerLibrary,
                        x => x.allOtherInventoryInst.cabinet.RenewableCabinetType).ToList();
                    }
                }

                if (LibraryType.ToLower() == "telecom")
                {
                    return _unitOfWork.OtherInSiteRepository.GetIncludeWhere(x => (
                        (x.SiteCode == BaseFilter.SiteCode) &&
                        (!x.Dismantle) &&
                        (BaseFilter.ItemStatusId != null ? (
                            x.allOtherInventoryInst != null ? (
                                x.allOtherInventoryInst.ItemStatusId != null ? (
                                    x.allOtherInventoryInst.ItemStatusId == BaseFilter.ItemStatusId)
                                : false)
                            : false)
                        : true) &&
                        (BaseFilter.TicketId != null ? (
                            x.allOtherInventoryInst != null ? (
                                x.allOtherInventoryInst.TicketId != null ? (
                                    x.allOtherInventoryInst.TicketId == BaseFilter.TicketId)
                                : false)
                            : false)
                        : true) &&
                        (x.allOtherInventoryInst.cabinetId != null) &&
                        (x.allOtherInventoryInst.Draft == false) &&
                        x.allOtherInventoryInst.cabinet.CabinetTelecomLibraryId != null
                    ),
                    x => x.allOtherInventoryInst, x => x.allOtherInventoryInst.cabinet,
                    x => x.allOtherInventoryInst.cabinet.CabinetTelecomLibrary, x => x.allOtherInventoryInst.cabinet.RenewableCabinetType).ToList();
                }
                else if (LibraryType.ToLower() == "power")
                {
                    return _unitOfWork.OtherInSiteRepository.GetIncludeWhere(x => (
                        (x.SiteCode == BaseFilter.SiteCode) &&
                        (!x.Dismantle) &&
                        (BaseFilter.ItemStatusId != null ? (
                            x.allOtherInventoryInst != null ? (
                                x.allOtherInventoryInst.ItemStatusId != null ? (
                                    x.allOtherInventoryInst.ItemStatusId == BaseFilter.ItemStatusId)
                                : false)
                            : false)
                        : true) &&
                        (BaseFilter.TicketId != null ? (
                            x.allOtherInventoryInst != null ? (
                                x.allOtherInventoryInst.TicketId != null ? (
                                    x.allOtherInventoryInst.TicketId == BaseFilter.TicketId)
                                : false)
                            : false)
                        : true) &&
                        (x.allOtherInventoryInst.cabinetId != null) &&
                        (x.allOtherInventoryInst.Draft == false) &&
                        x.allOtherInventoryInst.cabinet.CabinetPowerLibraryId != null
                    ),
                    x => x.allOtherInventoryInst, x => x.allOtherInventoryInst.cabinet, x => x.allOtherInventoryInst.cabinet.CabinetPowerLibrary,
                    x => x.allOtherInventoryInst.cabinet.RenewableCabinetType).ToList();
                }
            }
            else if (Kind == "Solar")
            {
                List<int> SolarIds = new List<int>();
                List<int> WithoutDateFilterSolarInstallation = new List<int>();
                List<int> WithDateFilterSolarInstallation = new List<int>();

                if (AttributeFilters != null && AttributeFilters.Count > 0)
                {
                    //
                    // Installation Dynamic Attributes...
                    //
                    List<TLIdynamicAtt> InstDynamicAttListIds = _unitOfWork.DynamicAttRepository.GetIncludeWhere(x =>
                        AttributeFilters.AsEnumerable().Select(y => y.key.ToLower()).Contains(x.Key.ToLower()) &&
                        !x.LibraryAtt && !x.disable &&
                        x.tablesNames.TableName == TablesNames.TLIsolar.ToString()
                            , x => x.tablesNames, x => x.DataType).ToList();

                    List<int> DynamicInstValueListIds = new List<int>();
                    bool DynamicInstExist = false;
                    if (InstDynamicAttListIds.Count() > 0)
                    {
                        DynamicInstExist = true;
                        GetInventoriesIdsFromDynamicAttributes(out DynamicInstValueListIds, InstDynamicAttListIds, AttributeFilters);
                    }

                    //
                    // Installation Attribute Activated...
                    //
                    bool AttrInstExist = typeof(SolarViewModel).GetProperties().ToList().Exists(x =>
                        AttributeFilters.Where(y => y.key.ToLower() != "Id".ToLower()).Select(y =>
                            y.key.ToLower()).Contains(x.Name.ToLower()));

                    List<int> InstallationAttributeActivated = new List<int>();
                    if (AttrInstExist)
                    {
                        List<PropertyInfo> NotStringProps = typeof(SolarViewModel).GetProperties().Where(x =>
                            x.PropertyType.Name.ToLower() != "string" &&
                            AttributeFilters.Select(y =>
                                y.key.ToLower()).Contains(x.Name.ToLower())).ToList();

                        List<PropertyInfo> StringProps = typeof(SolarViewModel).GetProperties().Where(x =>
                            x.PropertyType.Name.ToLower() == "string" &&
                            AttributeFilters.Select(y =>
                                y.key.ToLower()).Contains(x.Name.ToLower())).ToList();

                        List<StringFilterObjectList> AttrInstAttributeFilters = AttributeFilters.Where(x =>
                            NotStringProps.Select(y => y.Name.ToLower()).Contains(x.key.ToLower()) ||
                            StringProps.Select(y => y.Name.ToLower()).Contains(x.key.ToLower())).ToList();

                        IEnumerable<TLIsolar> Installations = _unitOfWork.SolarRepository.GetAllWithoutCount();

                        foreach (StringFilterObjectList InstallationProp in AttrInstAttributeFilters)
                        {
                            if (StringProps.Select(x => x.Name.ToLower()).Contains(InstallationProp.key.ToLower()))
                            {
                                Installations = Installations.Where(x => StringProps.AsEnumerable().FirstOrDefault(y => (InstallationProp.key.ToLower() == y.Name.ToLower()) && (InstallationProp.value.AsEnumerable().FirstOrDefault(w =>
                                     y.GetValue(_mapper.Map<SolarViewModel>(x), null) != null ? y.GetValue(_mapper.Map<SolarViewModel>(x), null).ToString().ToLower().StartsWith(w.ToLower()) : false) != null)) != null).AsEnumerable();
                            }
                            else if (NotStringProps.Select(x => x.Name.ToLower()).Contains(InstallationProp.key.ToLower()))
                            {
                                Installations = Installations.Where(x => NotStringProps.AsEnumerable().FirstOrDefault(y => (InstallationProp.key.ToLower() == y.Name.ToLower()) && (y.GetValue(_mapper.Map<SolarViewModel>(x), null) != null ?
                                    InstallationProp.value.AsEnumerable().Contains(y.GetValue(_mapper.Map<SolarViewModel>(x), null).ToString().ToLower()) : false)) != null).AsEnumerable();
                            }
                        }

                        InstallationAttributeActivated = Installations.Select(x => x.Id).ToList();
                    }

                    //
                    // Installation (Attribute Activated + Dynamic) Attributes...
                    //
                    if (AttrInstExist && DynamicInstExist)
                    {
                        WithoutDateFilterSolarInstallation = InstallationAttributeActivated.Intersect(DynamicInstValueListIds).ToList();
                    }
                    else if (AttrInstExist)
                    {
                        WithoutDateFilterSolarInstallation = InstallationAttributeActivated;
                    }
                    else if (DynamicInstExist)
                    {
                        WithoutDateFilterSolarInstallation = DynamicInstValueListIds;
                    }
                }

                if (DateFilter != null ? DateFilter.Count() > 0 : false)
                {
                    List<TLIdynamicAtt> DateTimeInstDynamicAttListIds = _unitOfWork.DynamicAttRepository.GetIncludeWhere(x =>
                        AfterConvertDateFilters.AsEnumerable().Select(y => y.key.ToLower()).Contains(x.Key.ToLower()) &&
                        !x.LibraryAtt && !x.disable &&
                        x.tablesNames.TableName == TablesNames.TLIsolar.ToString()
                            , x => x.tablesNames, x => x.DataType).ToList();

                    List<int> DynamicInstValueListIds = new List<int>();
                    bool DynamicInstExist = false;

                    if (DateTimeInstDynamicAttListIds.Count > 0)
                    {
                        DynamicInstExist = true;
                        List<DateFilterViewModel> DynamicInstAttributeFilters = AfterConvertDateFilters.Where(x =>
                            DateTimeInstDynamicAttListIds.AsEnumerable().Select(y => y.Key.ToLower()).Contains(x.key.ToLower())).ToList();

                        DynamicInstValueListIds = new List<int>();

                        List<TLIdynamicAttInstValue> DynamicInstValueListObjects = _unitOfWork.DynamicAttInstValueRepository.GetIncludeWhere(x =>
                            DateTimeInstDynamicAttListIds.Select(y => y.Id).Any(y => y == x.DynamicAttId) && !x.disable).ToList();

                        List<int> InventoriesIds = DynamicInstValueListObjects.Select(x => x.InventoryId).Distinct().ToList();

                        foreach (int InventoryId in InventoriesIds)
                        {
                            List<TLIdynamicAttInstValue> DynamicInstValueListInventories = DynamicInstValueListObjects.Where(x =>
                                x.InventoryId == InventoryId).ToList();

                            if (DynamicInstAttributeFilters.All(y => DynamicInstValueListInventories.Select(x => x.ValueDateTime).Any(x =>
                                (x != null ?
                                    (x >= y.DateFrom && x <= y.DateTo) : (false)))))
                            {
                                DynamicInstValueListIds.Add(InventoryId);
                            }
                        }
                    }

                    //
                    // Installation Attribute Activated...
                    //
                    List<PropertyInfo> InstallationProps = typeof(SolarViewModel).GetProperties().Where(x =>
                        AfterConvertDateFilters.Select(y =>
                            y.key.ToLower()).Contains(x.Name.ToLower())).ToList();

                    List<int> InstallationAttributeActivatedIds = new List<int>();
                    bool AttrInstExist = false;

                    if (InstallationProps != null)
                    {
                        AttrInstExist = true;

                        List<DateFilterViewModel> InstallationPropsAttributeFilters = AfterConvertDateFilters.Where(x =>
                            InstallationProps.Select(y => y.Name.ToLower()).Contains(x.key.ToLower())).ToList();

                        IEnumerable<TLIsolar> Installations = _unitOfWork.SolarRepository.GetAllWithoutCount();

                        foreach (DateFilterViewModel InstallationProp in InstallationPropsAttributeFilters)
                        {
                            Installations = Installations.Where(x => InstallationProps.Exists(y => (InstallationProp.key.ToLower() == y.Name.ToLower()) && ((y.GetValue(_mapper.Map<SolarViewModel>(x), null) != null) ?
                                ((InstallationProp.DateFrom.Date <= Convert.ToDateTime(y.GetValue(_mapper.Map<SolarViewModel>(x), null)).Date) &&
                                    (InstallationProp.DateTo.Date >= Convert.ToDateTime(y.GetValue(_mapper.Map<SolarViewModel>(x), null)).Date)) : (false))));
                        }

                        InstallationAttributeActivatedIds = Installations.Select(x => x.Id).ToList();
                    }

                    //
                    // Installation (Attribute Activated + Dynamic) Attributes...
                    //
                    if (AttrInstExist && DynamicInstExist)
                    {
                        WithDateFilterSolarInstallation = InstallationAttributeActivatedIds.Intersect(DynamicInstValueListIds).ToList();
                    }
                    else if (AttrInstExist)
                    {
                        WithDateFilterSolarInstallation = InstallationAttributeActivatedIds;
                    }
                    else if (DynamicInstExist)
                    {
                        WithDateFilterSolarInstallation = DynamicInstValueListIds;
                    }
                }
                if ((DateFilter != null ? DateFilter.Count() > 0 : false) ||
                    (AttributeFilters != null ? AttributeFilters.Count() > 0 : false))
                {
                    if ((AttributeFilters != null ? AttributeFilters.Count > 0 : false) &&
                        (AfterConvertDateFilters != null ? AfterConvertDateFilters.Count() > 0 : false))
                    {
                        SolarIds = WithoutDateFilterSolarInstallation.Intersect(WithDateFilterSolarInstallation).ToList();
                    }
                    else if (AttributeFilters != null ? AttributeFilters.Count > 0 : false)
                    {
                        SolarIds = WithoutDateFilterSolarInstallation;
                    }
                    else if (AfterConvertDateFilters != null ? AfterConvertDateFilters.Count() > 0 : false)
                    {
                        SolarIds = WithDateFilterSolarInstallation;
                    }

                    return _unitOfWork.OtherInSiteRepository.GetIncludeWhere(x => (
                        (x.SiteCode == BaseFilter.SiteCode) &&
                        (!x.Dismantle) &&
                        (BaseFilter.ItemStatusId != null ? (
                            x.allOtherInventoryInst != null ? (
                                x.allOtherInventoryInst.ItemStatusId != null ? (
                                    x.allOtherInventoryInst.ItemStatusId == BaseFilter.ItemStatusId)
                                : false)
                            : false)
                        : true) &&
                        (BaseFilter.TicketId != null ? (
                            x.allOtherInventoryInst != null ? (
                                x.allOtherInventoryInst.TicketId != null ? (
                                    x.allOtherInventoryInst.TicketId == BaseFilter.TicketId)
                                : false)
                            : false)
                        : true) &&
                        (x.allOtherInventoryInst.solarId != null) &&
                        (x.allOtherInventoryInst.Draft == false) &&
                        SolarIds.Contains(x.allOtherInventoryInst.solarId.Value)
                    ),
                    x => x.allOtherInventoryInst, x => x.allOtherInventoryInst.solar, x => x.allOtherInventoryInst.solar.Cabinet,
                    x => x.allOtherInventoryInst.solar.SolarLibrary).ToList();
                }

                return _unitOfWork.OtherInSiteRepository.GetIncludeWhere(x => (
                    (x.SiteCode == BaseFilter.SiteCode) &&
                    (!x.Dismantle) &&
                    (BaseFilter.ItemStatusId != null ? (
                        x.allOtherInventoryInst != null ? (
                            x.allOtherInventoryInst.ItemStatusId != null ? (
                                x.allOtherInventoryInst.ItemStatusId == BaseFilter.ItemStatusId)
                            : false)
                        : false)
                    : true) &&
                    (BaseFilter.TicketId != null ? (
                        x.allOtherInventoryInst != null ? (
                            x.allOtherInventoryInst.TicketId != null ? (
                                x.allOtherInventoryInst.TicketId == BaseFilter.TicketId)
                            : false)
                        : false)
                    : true) &&
                    (x.allOtherInventoryInst.solarId != null) &&
                    (x.allOtherInventoryInst.Draft == false)
                ),
                x => x.allOtherInventoryInst, x => x.allOtherInventoryInst.solar, x => x.allOtherInventoryInst.solar.Cabinet,
                    x => x.allOtherInventoryInst.solar.SolarLibrary).ToList();
            }
            else if (Kind == "Generator")
            {
                List<int> GeneratorIds = new List<int>();
                List<int> WithoutDateFilterGeneratorInstallation = new List<int>();
                List<int> WithDateFilterGeneratorInstallation = new List<int>();

                if (AttributeFilters != null && AttributeFilters.Count > 0)
                {
                    //
                    // Installation Dynamic Attributes...
                    //
                    List<TLIdynamicAtt> InstDynamicAttListIds = _unitOfWork.DynamicAttRepository.GetIncludeWhere(x =>
                        AttributeFilters.AsEnumerable().Select(y => y.key.ToLower()).Contains(x.Key.ToLower()) &&
                        !x.LibraryAtt && !x.disable &&
                        x.tablesNames.TableName == TablesNames.TLIgenerator.ToString()
                            , x => x.tablesNames, x => x.DataType).ToList();

                    List<int> DynamicInstValueListIds = new List<int>();
                    bool DynamicInstExist = false;
                    if (InstDynamicAttListIds.Count() > 0)
                    {
                        DynamicInstExist = true;
                        GetInventoriesIdsFromDynamicAttributes(out DynamicInstValueListIds, InstDynamicAttListIds, AttributeFilters);
                    }

                    //
                    // Installation Attribute Activated...
                    //
                    bool AttrInstExist = typeof(GeneratorViewModel).GetProperties().ToList().Exists(x =>
                        AttributeFilters.Where(y => y.key.ToLower() != "Id".ToLower()).Select(y =>
                            y.key.ToLower()).Contains(x.Name.ToLower()));

                    List<int> InstallationAttributeActivated = new List<int>();
                    if (AttrInstExist)
                    {
                        List<PropertyInfo> NotStringProps = typeof(GeneratorViewModel).GetProperties().Where(x =>
                            x.PropertyType.Name.ToLower() != "string" &&
                            AttributeFilters.Select(y =>
                                y.key.ToLower()).Contains(x.Name.ToLower())).ToList();

                        List<PropertyInfo> StringProps = typeof(GeneratorViewModel).GetProperties().Where(x =>
                            x.PropertyType.Name.ToLower() == "string" &&
                            AttributeFilters.Select(y =>
                                y.key.ToLower()).Contains(x.Name.ToLower())).ToList();

                        List<StringFilterObjectList> AttrInstAttributeFilters = AttributeFilters.Where(x =>
                            NotStringProps.Select(y => y.Name.ToLower()).Contains(x.key.ToLower()) ||
                            StringProps.Select(y => y.Name.ToLower()).Contains(x.key.ToLower())).ToList();

                        IEnumerable<TLIgenerator> Installations = _unitOfWork.GeneratorRepository.GetAllWithoutCount();

                        foreach (StringFilterObjectList InstallationProp in AttrInstAttributeFilters)
                        {
                            if (StringProps.Select(x => x.Name.ToLower()).Contains(InstallationProp.key.ToLower()))
                            {
                                Installations = Installations.Where(x => StringProps.AsEnumerable().FirstOrDefault(y => (InstallationProp.key.ToLower() == y.Name.ToLower()) && (InstallationProp.value.AsEnumerable().FirstOrDefault(w =>
                                     y.GetValue(_mapper.Map<GeneratorViewModel>(x), null) != null ? y.GetValue(_mapper.Map<GeneratorViewModel>(x), null).ToString().ToLower().StartsWith(w.ToLower()) : false) != null)) != null).AsEnumerable();
                            }
                            else if (NotStringProps.Select(x => x.Name.ToLower()).Contains(InstallationProp.key.ToLower()))
                            {
                                Installations = Installations.Where(x => NotStringProps.AsEnumerable().FirstOrDefault(y => (InstallationProp.key.ToLower() == y.Name.ToLower()) && (y.GetValue(_mapper.Map<GeneratorViewModel>(x), null) != null ?
                                    InstallationProp.value.AsEnumerable().Contains(y.GetValue(_mapper.Map<GeneratorViewModel>(x), null).ToString().ToLower()) : false)) != null).AsEnumerable();
                            }
                        }

                        InstallationAttributeActivated = Installations.Select(x => x.Id).ToList();

                    }

                    //
                    // Installation (Attribute Activated + Dynamic) Attributes...
                    //
                    if (AttrInstExist && DynamicInstExist)
                    {
                        WithoutDateFilterGeneratorInstallation = InstallationAttributeActivated.Intersect(DynamicInstValueListIds).ToList();
                    }
                    else if (AttrInstExist)
                    {
                        WithoutDateFilterGeneratorInstallation = InstallationAttributeActivated;
                    }
                    else if (DynamicInstExist)
                    {
                        WithoutDateFilterGeneratorInstallation = DynamicInstValueListIds;
                    }
                }

                if (DateFilter != null ? DateFilter.Count() > 0 : false)
                {
                    List<TLIdynamicAtt> DateTimeInstDynamicAttListIds = _unitOfWork.DynamicAttRepository.GetIncludeWhere(x =>
                        AfterConvertDateFilters.AsEnumerable().Select(y => y.key.ToLower()).Contains(x.Key.ToLower()) &&
                        !x.LibraryAtt && !x.disable &&
                        x.tablesNames.TableName == TablesNames.TLIgenerator.ToString()
                            , x => x.tablesNames, x => x.DataType).ToList();

                    List<int> DynamicInstValueListIds = new List<int>();
                    bool DynamicInstExist = false;

                    if (DateTimeInstDynamicAttListIds.Count > 0)
                    {
                        DynamicInstExist = true;
                        List<DateFilterViewModel> DynamicInstAttributeFilters = AfterConvertDateFilters.Where(x =>
                            DateTimeInstDynamicAttListIds.AsEnumerable().Select(y => y.Key.ToLower()).Contains(x.key.ToLower())).ToList();

                        DynamicInstValueListIds = new List<int>();

                        List<TLIdynamicAttInstValue> DynamicInstValueListObjects = _unitOfWork.DynamicAttInstValueRepository.GetIncludeWhere(x =>
                            DateTimeInstDynamicAttListIds.Select(y => y.Id).Any(y => y == x.DynamicAttId) && !x.disable).ToList();

                        List<int> InventoriesIds = DynamicInstValueListObjects.Select(x => x.InventoryId).Distinct().ToList();

                        foreach (int InventoryId in InventoriesIds)
                        {
                            List<TLIdynamicAttInstValue> DynamicInstValueListInventories = DynamicInstValueListObjects.Where(x =>
                                x.InventoryId == InventoryId).ToList();

                            if (DynamicInstAttributeFilters.All(y => DynamicInstValueListInventories.Select(x => x.ValueDateTime).Any(x =>
                                (x != null ?
                                    (x >= y.DateFrom && x <= y.DateTo) : (false)))))
                            {
                                DynamicInstValueListIds.Add(InventoryId);
                            }
                        }
                    }

                    //
                    // Installation Attribute Activated...
                    //
                    List<PropertyInfo> InstallationProps = typeof(GeneratorViewModel).GetProperties().Where(x =>
                        AfterConvertDateFilters.Select(y =>
                            y.key.ToLower()).Contains(x.Name.ToLower())).ToList();

                    List<int> InstallationAttributeActivatedIds = new List<int>();
                    bool AttrInstExist = false;

                    if (InstallationProps != null)
                    {
                        AttrInstExist = true;

                        List<DateFilterViewModel> InstallationPropsAttributeFilters = AfterConvertDateFilters.Where(x =>
                            InstallationProps.Select(y => y.Name.ToLower()).Contains(x.key.ToLower())).ToList();

                        IEnumerable<TLIgenerator> Installations = _unitOfWork.GeneratorRepository.GetAllWithoutCount();

                        foreach (DateFilterViewModel InstallationProp in InstallationPropsAttributeFilters)
                        {
                            Installations = Installations.Where(x => InstallationProps.Exists(y => (InstallationProp.key.ToLower() == y.Name.ToLower()) && ((y.GetValue(_mapper.Map<GeneratorViewModel>(x), null) != null) ?
                                ((InstallationProp.DateFrom.Date <= Convert.ToDateTime(y.GetValue(_mapper.Map<GeneratorViewModel>(x), null)).Date) &&
                                    (InstallationProp.DateTo.Date >= Convert.ToDateTime(y.GetValue(_mapper.Map<GeneratorViewModel>(x), null)).Date)) : (false))));
                        }

                        InstallationAttributeActivatedIds = Installations.Select(x => x.Id).ToList();
                    }

                    //
                    // Installation (Attribute Activated + Dynamic) Attributes...
                    //
                    if (AttrInstExist && DynamicInstExist)
                    {
                        WithDateFilterGeneratorInstallation = InstallationAttributeActivatedIds.Intersect(DynamicInstValueListIds).ToList();
                    }
                    else if (AttrInstExist)
                    {
                        WithDateFilterGeneratorInstallation = InstallationAttributeActivatedIds;
                    }
                    else if (DynamicInstExist)
                    {
                        WithDateFilterGeneratorInstallation = DynamicInstValueListIds;
                    }
                }
                if ((DateFilter != null ? DateFilter.Count() > 0 : false) ||
                    (AttributeFilters != null ? AttributeFilters.Count() > 0 : false))
                {
                    if ((AttributeFilters != null ? AttributeFilters.Count > 0 : false) &&
                        (AfterConvertDateFilters != null ? AfterConvertDateFilters.Count() > 0 : false))
                    {
                        GeneratorIds = WithoutDateFilterGeneratorInstallation.Intersect(WithDateFilterGeneratorInstallation).ToList();
                    }
                    else if (AttributeFilters != null ? AttributeFilters.Count > 0 : false)
                    {
                        GeneratorIds = WithoutDateFilterGeneratorInstallation;
                    }
                    else if (AfterConvertDateFilters != null ? AfterConvertDateFilters.Count() > 0 : false)
                    {
                        GeneratorIds = WithDateFilterGeneratorInstallation;
                    }

                    return _unitOfWork.OtherInSiteRepository.GetIncludeWhere(x => (
                        (x.SiteCode == BaseFilter.SiteCode) &&
                        (!x.Dismantle) &&
                        (BaseFilter.ItemStatusId != null ? (
                            x.allOtherInventoryInst != null ? (
                                x.allOtherInventoryInst.ItemStatusId != null ? (
                                    x.allOtherInventoryInst.ItemStatusId == BaseFilter.ItemStatusId)
                                : false)
                            : false)
                        : true) &&
                        (BaseFilter.TicketId != null ? (
                            x.allOtherInventoryInst != null ? (
                                x.allOtherInventoryInst.TicketId != null ? (
                                    x.allOtherInventoryInst.TicketId == BaseFilter.TicketId)
                                : false)
                            : false)
                        : true) &&
                        (x.allOtherInventoryInst.generatorId != null) &&
                        (x.allOtherInventoryInst.Draft == false) &&
                        GeneratorIds.Contains(x.allOtherInventoryInst.generatorId.Value)
                    ),
                    x => x.allOtherInventoryInst, x => x.allOtherInventoryInst.generator, x => x.allOtherInventoryInst.generator.BaseGeneratorType,
                    x => x.allOtherInventoryInst.generator.GeneratorLibrary).ToList();
                }

                return _unitOfWork.OtherInSiteRepository.GetIncludeWhere(x => (
                    (x.SiteCode == BaseFilter.SiteCode) &&
                    (!x.Dismantle) &&
                    (BaseFilter.ItemStatusId != null ? (
                        x.allOtherInventoryInst != null ? (
                            x.allOtherInventoryInst.ItemStatusId != null ? (
                                x.allOtherInventoryInst.ItemStatusId == BaseFilter.ItemStatusId)
                            : false)
                        : false)
                    : true) &&
                    (BaseFilter.TicketId != null ? (
                        x.allOtherInventoryInst != null ? (
                            x.allOtherInventoryInst.TicketId != null ? (
                                x.allOtherInventoryInst.TicketId == BaseFilter.TicketId)
                            : false)
                        : false)
                    : true) &&
                    (x.allOtherInventoryInst.generatorId != null) &&
                    (x.allOtherInventoryInst.Draft == false)
                ),
                x => x.allOtherInventoryInst, x => x.allOtherInventoryInst.generator, x => x.allOtherInventoryInst.generator.BaseGeneratorType,
                    x => x.allOtherInventoryInst.generator.GeneratorLibrary).ToList();
            }
            return null;
        }
        public List<TLIdynamicAttLibValue> GetDynamicAttLibValue(TLIcabinet CabinetRecord, List<TLIattributeViewManagment> AttViewManagment)
        {
            return _unitOfWork.DynamicAttLibRepository.GetIncludeWhere(x =>
                AttViewManagment != null ? (
                    AttViewManagment.Exists(y => y.DynamicAttId == x.DynamicAttId)
                ) : true &&
                CabinetRecord.CabinetPowerLibraryId != null ? (
                    x.InventoryId == CabinetRecord.CabinetPowerLibraryId.Value &&
                    x.DynamicAtt.tablesNames.TableName == TablesNames.TLIcabinetPowerLibrary.ToString()
                ) : true &&
                CabinetRecord.CabinetTelecomLibraryId != null ? (
                    x.InventoryId == CabinetRecord.CabinetTelecomLibraryId.Value &&
                    x.DynamicAtt.tablesNames.TableName == TablesNames.TLIcabinetTelecomLibrary.ToString()
                ) : true &&
                x.DynamicAtt.disable == false &&
                x.DynamicAtt.LibraryAtt
            , x => x.DynamicAtt, x => x.tablesNames, x => x.DynamicAtt.DataType).ToList();
        }
        #endregion
        #region AddHistory
        //public void AddHistory (TicketAttributes ticketAtt, int allOtherInventoryId, string historyType)
        //{
        //    if (ticketAtt != null)
        //    {
        //        AddWorkflowHistoryViewModel workflowhistory = _mapper.Map<AddWorkflowHistoryViewModel>(ticketAtt);
        //        workflowhistory.RecordId = allOtherInventoryId;
        //        workflowhistory.TablesNameId = _unitOfWork.TablesNamesRepository.GetWhereFirst(x => x.TableName == "TLIallOtherInventoryInst").Id;
        //        workflowhistory.HistoryTypeId = _unitOfWork.HistoryTypeRepository.GetWhereSelectFirst(x => x.Name == historyType, x => new { x.Id }).Id;
        //        workflowhistory.UserId = 83;
        //        _unitOfWork.WorkflowHistoryRepository.AddWorkflowHistory(workflowhistory);
        //    }
        //    else
        //    {

        //        AddTablesHistoryViewModel history = new AddTablesHistoryViewModel();
        //        history.RecordId = allOtherInventoryId;
        //        history.TablesNameId = _unitOfWork.TablesNamesRepository.GetWhereFirst(x => x.TableName == "TLIallCivilInst").Id;
        //        history.HistoryTypeId = _unitOfWork.HistoryTypeRepository.GetWhereSelectFirst(x => x.Name == historyType, x => new { x.Id }).Id;
        //        history.UserId = 83;
        //        _unitOfWork.TablesHistoryRepository.AddTableHistory(history);

        //    }
        //}
        #endregion
    }
}