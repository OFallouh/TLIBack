using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using TLIS_DAL;
using TLIS_DAL.Helper;
using System.Collections;
using TLIS_DAL.Helper.Filters;
using TLIS_DAL.Helpers;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModelBase;
using TLIS_DAL.ViewModels;
using TLIS_DAL.ViewModels.BaseBUDTOs;
using TLIS_DAL.ViewModels.CivilWithLegsDTOs;
using TLIS_DAL.ViewModels.DynamicAttDTOs;
using TLIS_DAL.ViewModels.DynamicAttInstValueDTOs;
using TLIS_DAL.ViewModels.ItemConnectToDTOs;
using TLIS_DAL.ViewModels.MW_BUDTOs;
using TLIS_DAL.ViewModels.MW_BULibraryDTOs;
using TLIS_DAL.ViewModels.MW_DishDTOs;
using TLIS_DAL.ViewModels.MW_DishLbraryDTOs;
using TLIS_DAL.ViewModels.MW_ODUDTOs;
using TLIS_DAL.ViewModels.Mw_OtherDTOs;
using TLIS_DAL.ViewModels.MW_OtherLibraryDTOs;
using TLIS_DAL.ViewModels.MW_PortDTOs;
using TLIS_DAL.ViewModels.MW_RFUDTOs;
using TLIS_DAL.ViewModels.OduInstallationTypeDTOs;
using TLIS_DAL.ViewModels.OwnerDTOs;
using TLIS_DAL.ViewModels.PolarityOnLocationDTOs;
using TLIS_DAL.ViewModels.RepeaterTypeDTOs;
using TLIS_DAL.ViewModels.RuleDTOs;
using TLIS_DAL.ViewModels.TablesHistoryDTOs;
using TLIS_DAL.ViewModels.WorkflowHistoryDTOs;
using TLIS_Repository.Base;
using TLIS_Service.IService;
using static TLIS_Service.Helpers.Constants;
using TLIS_DAL.ViewModels.SideArmDTOs;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Database;
using TLIS_DAL.ViewModels.CivilLoadsDTOs;
using AutoMapper;

namespace TLIS_Service.Services
{
    public class MWInstService : IMWInstService
    {
        IUnitOfWork _unitOfWork;
        IServiceCollection _services;
        private readonly ApplicationDbContext _dbContext;
        private IMapper _mapper;
        public MWInstService(IUnitOfWork unitOfWork, IServiceCollection services, ApplicationDbContext dbContext, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _services = services;
            _dbContext = dbContext;
            _mapper = mapper;
        }
        //Function take 3 parameters TableName, LibraryID, SiteCode
        //First get tabe name Entity by TableName
        //Second specify the table i deal with depened on TableName
        //Get Library by Id 
        //Get library activated attributes with values from selected library
        //Get activated attributes for installation table
        //Get dynamic attributes for table name
        //Get related tables
        public Response<ObjectInstAtts> GetAttForAdd(string TableName, int LibraryID, string SiteCode)
        {
            try
            {
                TLItablesNames TableNameEntity = _unitOfWork.TablesNamesRepository.GetWhereFirst(x =>
                    x.TableName == TableName);

                ObjectInstAtts objectInst = new ObjectInstAtts();
                List<BaseInstAttView> ListAttributesActivated = new List<BaseInstAttView>();

                if (LoadSubType.TLImwBU.ToString() == TableName)
                {
                    MW_BULibraryViewModel mwBULibrary = _mapper.Map<MW_BULibraryViewModel>(_unitOfWork.MW_BULibraryRepository
                        .GetIncludeWhereFirst(x => x.Id == LibraryID, x => x.diversityType));

                    List<BaseAttView> LibraryAttributes = _unitOfWork.AttributeActivatedRepository
                        .GetAttributeActivated(TablesNames.TLImwBULibrary.ToString(), mwBULibrary, null).ToList();

                    foreach (BaseAttView LibraryAttribute in LibraryAttributes)
                    {
                        if (LibraryAttribute.DataType.ToLower() == "list")
                        {
                            LibraryAttribute.Value = mwBULibrary.GetType().GetProperties()
                                .FirstOrDefault(x => x.Name.ToLower() == LibraryAttribute.Label.ToLower()).GetValue(mwBULibrary);
                        }
                    }

                    List<BaseAttView> LogisticalAttributes = _mapper.Map<List<BaseAttView>>(_unitOfWork.LogistcalRepository
                        .GetLogistical(TablePartName.MW.ToString(), Helpers.Constants.TablesNames.TLImwBULibrary.ToString(), mwBULibrary.Id).ToList());

                    LibraryAttributes.AddRange(LogisticalAttributes);

                    objectInst.LibraryActivatedAttributes = LibraryAttributes;

                    ListAttributesActivated = _unitOfWork.AttributeActivatedRepository.
                        GetInstAttributeActivated(LoadSubType.TLImwBU.ToString(), null, "Name", "InstallationPlaceId", "MwBULibraryId" /*, "EquivalentSpace"*/).ToList();

                    BaseInstAttView NameAttribute = ListAttributesActivated.FirstOrDefault(x => x.Key.ToLower() == "Name".ToLower());
                    if (NameAttribute != null)
                    {
                        BaseInstAttView Swap = ListAttributesActivated[0];
                        ListAttributesActivated[ListAttributesActivated.IndexOf(NameAttribute)] = Swap;
                        ListAttributesActivated[0] = NameAttribute;
                    }
                    foreach (BaseInstAttView FKitem in ListAttributesActivated)
                    {
                        if (FKitem.Desc.ToLower() == "tlibasebu")
                            FKitem.Value = _mapper.Map<List<BaseBUViewModel>>(_unitOfWork.BaseBURepository.GetWhere(x => !x.Disable && !x.Deleted).ToList());

                        else if (FKitem.Desc.ToLower() == "tliowner")
                            FKitem.Value = _mapper.Map<List<OwnerViewModel>>(_unitOfWork.OwnerRepository.GetWhere(x => !x.Disable && !x.Deleted).ToList());

                        else if (FKitem.Desc.ToLower() == "tlimwdish")
                        {
                            var Dish = _unitOfWork.CivilLoadsRepository.GetIncludeWhere(x => x.SiteCode == SiteCode && !x.Dismantle && x.allLoadInstId != null, x => x.allLoadInst).Select(x => x.allLoadInst.mwDishId).ToList();

                            List<TLImwDish> mwdishlist = new List<TLImwDish>();
                            foreach (var item in Dish)
                            {
                                if (item != null)
                                {
                                    var dishname = _dbContext.TLImwDish.FirstOrDefault(x => x.Id == item);
                                    mwdishlist.Add(dishname);
                                }
                            }
                            FKitem.Value = _mapper.Map<List<MW_DishGetForAddViewModel>>(mwdishlist);

                        }
                        else if (FKitem.Desc.ToLower() == "tlimwport")
                            FKitem.Value = _mapper.Map<List<MW_PortViewModel>>(_unitOfWork.MW_PortRepository.GetWhere(x => x.Port_Type == 2).ToList());
                    }
                }
                else if (LoadSubType.TLImwODU.ToString() == TableName)
                {
                    MW_ODULibraryViewModel mwODULibrary = _mapper.Map<MW_ODULibraryViewModel>(_unitOfWork.MW_ODULibraryRepository
                        .GetIncludeWhereFirst(x => x.Id == LibraryID, x => x.parity));

                    List<BaseAttView> LibraryAttributes = _unitOfWork.AttributeActivatedRepository
                        .GetAttributeActivated(TablesNames.TLImwODULibrary.ToString(), mwODULibrary, null).ToList();

                    foreach (BaseAttView LibraryAttribute in LibraryAttributes)
                    {
                        if (LibraryAttribute.DataType.ToLower() == "list")
                        {
                            LibraryAttribute.Value = mwODULibrary.GetType().GetProperties()
                                .FirstOrDefault(x => x.Name.ToLower() == LibraryAttribute.Label.ToLower()).GetValue(mwODULibrary);
                        }
                    }

                    List<BaseAttView> LogisticalAttributes = _mapper.Map<List<BaseAttView>>(_unitOfWork.LogistcalRepository
                        .GetLogistical(TablePartName.MW.ToString(), Helpers.Constants.TablesNames.TLImwODULibrary.ToString(), mwODULibrary.Id).ToList());

                    LibraryAttributes.AddRange(LogisticalAttributes);

                    objectInst.LibraryActivatedAttributes = LibraryAttributes;

                    ListAttributesActivated = _unitOfWork.AttributeActivatedRepository.
                        GetInstAttributeActivated(LoadSubType.TLImwODU.ToString(), null, "Name", "MwODULibraryId", "OduInstallationTypeId"/*, "EquivalentSpace"*/).ToList();

                    BaseInstAttView NameAttribute = ListAttributesActivated.FirstOrDefault(x => x.Key.ToLower() == "Name".ToLower());
                    if (NameAttribute != null)
                    {
                        BaseInstAttView Swap = ListAttributesActivated[0];
                        ListAttributesActivated[ListAttributesActivated.IndexOf(NameAttribute)] = Swap;
                        ListAttributesActivated[0] = NameAttribute;
                    }
                    foreach (BaseInstAttView FKitem in ListAttributesActivated)
                    {
                        if (FKitem.Desc.ToLower() == "tliowner")
                            FKitem.Value = _mapper.Map<List<OwnerViewModel>>(_unitOfWork.OwnerRepository.GetWhere(x => !x.Disable && !x.Deleted).ToList());

                        else if (FKitem.Desc.ToLower() == "tlimwdish")
                        {
                            List<int> UsedDishesIds = _unitOfWork.MW_ODURepository.GetWhere(x => x.Mw_DishId != null).Select(x => x.Mw_DishId.Value).ToList();

                            List<MW_DishGetForAddViewModel> MW_Dishes = _mapper.Map<List<MW_DishGetForAddViewModel>>(_unitOfWork.CivilLoadsRepository
                                .GetIncludeWhere(x => !x.Dismantle &&
                                    (x.allLoadInstId != null ? x.allLoadInst.mwDishId != null : false) &&
                                    !UsedDishesIds.Contains(x.allLoadInst.mwDishId.Value), x => x.allLoadInst, x => x.allLoadInst.mwDish)
                                .Select(x => x.allLoadInst.mwDish).ToList());

                            FKitem.Value = _mapper.Map<List<MW_DishGetForAddViewModel>>(MW_Dishes);
                        }
                    }

                }
                else if (LoadSubType.TLImwRFU.ToString() == TableName)
                {
                    MW_RFULibraryViewModel mwRFULibrary = _mapper.Map<MW_RFULibraryViewModel>(_unitOfWork.MW_RFULibraryRepository
                        .GetIncludeWhereFirst(x => x.Id == LibraryID, x => x.boardType, x => x.diversityType));

                    List<BaseAttView> LibraryAttributes = _unitOfWork.AttributeActivatedRepository
                        .GetAttributeActivated(TablesNames.TLImwRFULibrary.ToString(), mwRFULibrary, null).ToList();

                    foreach (BaseAttView LibraryAttribute in LibraryAttributes)
                    {
                        if (LibraryAttribute.DataType.ToLower() == "list")
                        {
                            LibraryAttribute.Value = mwRFULibrary.GetType().GetProperties()
                                .FirstOrDefault(x => x.Name.ToLower() == LibraryAttribute.Label.ToLower()).GetValue(mwRFULibrary);
                        }
                    }

                    List<BaseAttView> LogisticalAttributes = _mapper.Map<List<BaseAttView>>(_unitOfWork.LogistcalRepository
                        .GetLogistical(TablePartName.MW.ToString(), Helpers.Constants.TablesNames.TLImwRFULibrary.ToString(), mwRFULibrary.Id).ToList());

                    LibraryAttributes.AddRange(LogisticalAttributes);

                    objectInst.LibraryActivatedAttributes = LibraryAttributes;

                    ListAttributesActivated = _unitOfWork.AttributeActivatedRepository.
                        GetInstAttributeActivated(LoadSubType.TLImwRFU.ToString(), null, "MwRFULibraryId", "MwPortId"/*, "EquivalentSpace"*/).ToList();

                    BaseInstAttView NameAttribute = ListAttributesActivated.FirstOrDefault(x => x.Key.ToLower() == "Name".ToLower());
                    if (NameAttribute != null)
                    {
                        BaseInstAttView Swap = ListAttributesActivated[0];
                        ListAttributesActivated[ListAttributesActivated.IndexOf(NameAttribute)] = Swap;
                        ListAttributesActivated[0] = NameAttribute;
                    }
                    foreach (BaseInstAttView FKitem in ListAttributesActivated)
                    {
                        if (FKitem.Desc.ToLower() == "tliowner")
                            FKitem.Value = _mapper.Map<List<OwnerViewModel>>(_unitOfWork.OwnerRepository.GetWhere(x => !x.Disable && !x.Deleted).ToList());
                    }
                }
                else if (LoadSubType.TLImwDish.ToString() == TableName)
                {
                    MW_DishLibraryViewModel mwDishLibrary = _mapper.Map<MW_DishLibraryViewModel>(_unitOfWork.MW_DishLibraryRepository
                        .GetIncludeWhereFirst(x => x.Id == LibraryID, x => x.asType, x => x.polarityType));

                    List<BaseAttView> LibraryAttributes = _unitOfWork.AttributeActivatedRepository
                        .GetAttributeActivated(TablesNames.TLImwDishLibrary.ToString(), mwDishLibrary, null).ToList();

                    foreach (BaseAttView LibraryAttribute in LibraryAttributes)
                    {
                        if (LibraryAttribute.DataType.ToLower() == "list")
                        {
                            LibraryAttribute.Value = mwDishLibrary.GetType().GetProperties()
                                .FirstOrDefault(x => x.Name.ToLower() == LibraryAttribute.Label.ToLower()).GetValue(mwDishLibrary);
                        }
                    }

                    List<BaseAttView> LogisticalAttributes = _mapper.Map<List<BaseAttView>>(_unitOfWork.LogistcalRepository
                        .GetLogistical(TablePartName.MW.ToString(), Helpers.Constants.TablesNames.TLImwDishLibrary.ToString(), mwDishLibrary.Id).ToList());

                    LibraryAttributes.AddRange(LogisticalAttributes);

                    objectInst.LibraryActivatedAttributes = LibraryAttributes;

                    ListAttributesActivated = _unitOfWork.AttributeActivatedRepository.
                        GetInstAttributeActivated(LoadSubType.TLImwDish.ToString(), null, "DishName", "InstallationPlaceId", "MwDishLibraryId"/*, "EquivalentSpace"*/).ToList();

                    BaseInstAttView NameAttribute = ListAttributesActivated.FirstOrDefault(x => x.Key.ToLower() == "DishName".ToLower());
                    if (NameAttribute != null)
                    {
                        BaseInstAttView Swap = ListAttributesActivated[0];
                        ListAttributesActivated[ListAttributesActivated.IndexOf(NameAttribute)] = Swap;
                        ListAttributesActivated[0] = NameAttribute;
                    }

                    foreach (BaseInstAttView FKitem in ListAttributesActivated)
                    {
                        if (FKitem.Desc.ToLower() == "tliitemconnectto")
                            FKitem.Value = _mapper.Map<List<ItemConnectToViewModel>>(_unitOfWork.ItemConnectToRepository.GetWhere(x => !x.Deleted && !x.Disable).ToList());

                        else if (FKitem.Desc.ToLower() == "tlipolarityonlocation")
                            FKitem.Value = _mapper.Map<List<PolarityOnLocationViewModel>>(_unitOfWork.PolarityOnLocationRepository.GetWhere(x => !x.Deleted && !x.Disable).ToList());

                        else if (FKitem.Desc.ToLower() == "tlirepeatertype")
                            FKitem.Value = _mapper.Map<List<RepeaterTypeViewModel>>(_unitOfWork.RepeaterTypeRepository.GetWhere(x => !x.Deleted && !x.Disable).ToList());

                        else if (FKitem.Desc.ToLower() == "tliowner")
                            FKitem.Value = _mapper.Map<List<OwnerViewModel>>(_unitOfWork.OwnerRepository.GetWhere(x => !x.Deleted && !x.Disable).ToList());
                    }
                }
                else if (LoadSubType.TLImwOther.ToString() == TableName)
                {
                    TLImwOtherLibrary mwOtherLibrary = _unitOfWork.MW_OtherLibraryRepository.GetByID(LibraryID);

                    List<BaseAttView> LibraryAttributes = _unitOfWork.AttributeActivatedRepository
                        .GetAttributeActivated(TablesNames.TLImwOtherLibrary.ToString(), mwOtherLibrary, null).ToList();

                    List<BaseAttView> LogisticalAttributes = _mapper.Map<List<BaseAttView>>(_unitOfWork.LogistcalRepository
                        .GetLogistical(TablePartName.MW.ToString(), Helpers.Constants.TablesNames.TLImwOtherLibrary.ToString(), mwOtherLibrary.Id).ToList());

                    LibraryAttributes.AddRange(LogisticalAttributes);

                    objectInst.LibraryActivatedAttributes = LibraryAttributes;

                    ListAttributesActivated = _unitOfWork.AttributeActivatedRepository.
                        GetInstAttributeActivated(LoadSubType.TLImwOther.ToString(), null, "mwOtherLibraryId", /*"EquivalentSpace",*/
                            "InstallationPlaceId").ToList();

                    BaseInstAttView NameAttribute = ListAttributesActivated.FirstOrDefault(x => x.Key.ToLower() == "Name".ToLower());
                    if (NameAttribute != null)
                    {
                        BaseInstAttView Swap = ListAttributesActivated[0];
                        ListAttributesActivated[ListAttributesActivated.IndexOf(NameAttribute)] = Swap;
                        ListAttributesActivated[0] = NameAttribute;
                    }
                }

                List<KeyValuePair<string, List<DropDownListFilters>>> RelatedTables = _unitOfWork.CivilLoadsRepository.GetRelatedTables(SiteCode);
                objectInst.RelatedTables = RelatedTables;

                objectInst.AttributesActivated = ListAttributesActivated;

                objectInst.CivilLoads = _unitOfWork.AttributeActivatedRepository.
                    GetInstAttributeActivated(TablesNames.TLIcivilLoads.ToString(), null, "allLoadInstId", "Dismantle", "SiteCode", "legId", "Leg2Id",
                        "sideArmId", "allCivilInstId", "civilSteelSupportCategoryId");

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

                return new Response<ObjectInstAtts>(objectInst);
            }
            catch (Exception err)
            {
                return new Response<ObjectInstAtts>(true, null, null, err.Message, (int)ApiReturnCode.fail);
            }
        }
        public Response<List<MW_PortViewModel>> GetPortCascadedByBUId(int BUId, int? MainBUId)
        {
            List<int> UsedPorts = _unitOfWork.MW_BURepository
                .GetWhere(x => x.PortCascadeId > 0 && (MainBUId != null ? x.Id != MainBUId : true)).Select(x => x.PortCascadeId).ToList();

            List<MW_PortViewModel> Ports = _mapper.Map<List<MW_PortViewModel>>(_unitOfWork.MW_PortRepository
                .GetIncludeWhere(x => x.MwBUId == BUId && !UsedPorts.Contains(x.Id), x => x.MwBU, x => x.MwBULibrary).ToList());

            return new Response<List<MW_PortViewModel>>(Ports);
        }
        //Function take 3 parameters MWInstallationViewModel, TableName, SiteCode
        //First get table name Entity depened on TableName
        //Second specify the table i deal with depened on TableName
        //Map MWInstallationViewModel obj to ViewModel
        //Map ViewModel to Entity
        //Apply bussiness rules
        //Add Entity to database
        //Add relation between load and civil to TLIcivilLoads table
        //Add Dynamic Inst attributes
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
        public string CheckDependencyValidationForMWTypes(object Input, string MWType, string SiteCode)
        {
            if (MWType.ToLower() == TablesNames.TLImwDish.ToString().ToLower())
            {
                string MainTableName = TablesNames.TLImwDish.ToString();
                AddMW_DishViewModel AddInstallationViewModel = _mapper.Map<AddMW_DishViewModel>(Input);

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
                        List<TLIrule> Rules = _unitOfWork.RowRuleRepository.GetIncludeWhere(x => x.RowId == RowId && x.Rule.OperationId != null, x => x.Rule, x => x.Rule.tablesNames,
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
            else if (MWType.ToLower() == TablesNames.TLImwBU.ToString().ToLower())
            {
                string MainTableName = TablesNames.TLImwBU.ToString();
                AddMW_BUViewModel AddInstallationViewModel = _mapper.Map<AddMW_BUViewModel>(Input);

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
                        List<TLIrule> Rules = _unitOfWork.RowRuleRepository.GetIncludeWhere(x => x.RowId == RowId && x.Rule.OperationId != null, x => x.Rule, x => x.Rule.tablesNames,
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
                                    if (CivilLoads.GetType().GetProperty(Path[1]).GetValue(CivilLoads, null) != null)
                                    {
                                        CheckId = CivilLoads.GetType().GetProperty(Path[1]).GetValue(CivilLoads, null) != null ?
                                            (int)CivilLoads.GetType().GetProperty(Path[1]).GetValue(CivilLoads, null) : new object();
                                    }

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
            else if (MWType.ToLower() == TablesNames.TLImwRFU.ToString().ToLower())
            {
                string MainTableName = TablesNames.TLImwRFU.ToString();
                AddMW_RFUViewModel AddInstallationViewModel = _mapper.Map<AddMW_RFUViewModel>(Input);

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
                        List<TLIrule> Rules = _unitOfWork.RowRuleRepository.GetIncludeWhere(x => x.RowId == RowId && x.Rule.OperationId != null, x => x.Rule, x => x.Rule.tablesNames,
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
                                    if (Path[1].ToLower() == "allLoadInstId".ToLower())
                                    {
                                        int MW_PortId = (int)AddInstallationViewModel.GetType().GetProperty("MwPortId")
                                            .GetValue(AddInstallationViewModel, null);

                                        int MW_BUId = _unitOfWork.MW_PortRepository.GetWhereFirst(x => x.Id == MW_PortId).MwBUId;

                                        CheckId = _unitOfWork.AllLoadInstRepository.GetWhereFirst(x => !x.Draft && x.mwBUId == MW_BUId).Id;
                                    }
                                    else
                                    {
                                        object CivilLoads = AddInstallationViewModel.GetType().GetProperty(Path[0])
                                            .GetValue(AddInstallationViewModel, null);

                                        CheckId = CivilLoads.GetType().GetProperty(Path[1]).GetValue(CivilLoads, null) != null ?
                                            (int)CivilLoads.GetType().GetProperty(Path[1]).GetValue(CivilLoads, null) : new object();
                                    }
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
            else if (MWType.ToLower() == TablesNames.TLImwODU.ToString().ToLower())
            {
                string MainTableName = TablesNames.TLImwODU.ToString();
                AddMW_ODUViewModel AddInstallationViewModel = _mapper.Map<AddMW_ODUViewModel>(Input);

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
                        List<TLIrule> Rules = _unitOfWork.RowRuleRepository.GetIncludeWhere(x => x.RowId == RowId && x.Rule.OperationId != null, x => x.Rule, x => x.Rule.tablesNames,
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
            else if (MWType.ToLower() == TablesNames.TLImwOther.ToString().ToLower())
            {
                string MainTableName = TablesNames.TLImwOther.ToString();
                AddMw_OtherViewModel AddInstallationViewModel = _mapper.Map<AddMw_OtherViewModel>(Input);

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
                        List<TLIrule> Rules = _unitOfWork.RowRuleRepository.GetIncludeWhere(x => x.RowId == RowId && x.Rule.OperationId != null, x => x.Rule, x => x.Rule.tablesNames,
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
        public Response<ObjectInstAtts> AddMWInstallation(object MWInstallationViewModel, string TableName, string SiteCode, string ConnectionString)
        {
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
                            var TableNameEntity = _unitOfWork.TablesNamesRepository.GetWhereFirst(x => x.TableName == TableName);
                            if (LoadSubType.TLImwODU.ToString() == TableName)
                            {
                                AddMW_ODUViewModel addMW_ODU = _mapper.Map<AddMW_ODUViewModel>(MWInstallationViewModel);
                                TLImwODU mwODU = _mapper.Map<TLImwODU>(addMW_ODU);
                                //Installation: 
                                //{(((( -Directly behind the dish, (Installation mode called “Direct Mount”). In other installation mode(called “Separate Mount”, the ODU is installed separately to the civil steel support using side arm))))}.
                                bool test = true;
                                string CheckDependencyValidation = CheckDependencyValidationForMWTypes(MWInstallationViewModel, TableName, SiteCode);

                                if (!string.IsNullOrEmpty(CheckDependencyValidation))
                                    return new Response<ObjectInstAtts>(true, null, null, CheckDependencyValidation, (int)ApiReturnCode.fail);

                                string CheckGeneralValidation = CheckGeneralValidationFunction(addMW_ODU.TLIdynamicAttInstValue, TableName);

                                if (!string.IsNullOrEmpty(CheckGeneralValidation))
                                    return new Response<ObjectInstAtts>(true, null, null, CheckGeneralValidation, (int)ApiReturnCode.fail);

                                if (test == true)
                                {
                                    //TLIcivilLoads CheckName = _unitOfWork.CivilLoadsRepository.GetIncludeWhereFirst(x => !x.Dismantle && (x.allLoadInstId != null ?
                                    //    !x.allLoadInst.Draft && (x.allLoadInst.mwODUId != null ? x.allLoadInst.mwODU.Name.ToLower() == mwODU.Name.ToLower() : false) : false),
                                    //        x => x.allLoadInst, x => x.allLoadInst.mwODU);
                                    //if (CheckName != null)
                                    //    return new Response<ObjectInstAtts>(true, null, null, $"This name {mwODU.Name} is already exists", (int)ApiReturnCode.fail); 
                                    var CheckSerialNumber = _unitOfWork.MW_ODURepository.GetWhereFirst(x => x.Serial_Number == mwODU.Serial_Number);
                                    if (CheckSerialNumber != null)
                                    {
                                        return new Response<ObjectInstAtts>(true, null, null, $"The SerialNumber {mwODU.Serial_Number} is already exists", (int)ApiReturnCode.fail);
                                    }
                                    TLIoduInstallationType OduInstallationType = _unitOfWork.OduInstallationTypeRepository.GetByID((int)addMW_ODU.OduInstallationTypeId);

                                    //if (addMW_ODU.OduInstallationTypeId != null)
                                    //{
                                    //    OduInstallationType = _unitOfWork.OduInstallationTypeRepository.GetByID((int)addMW_ODU.OduInstallationTypeId);
                                    //    if (OduInstallationType.Name.ToLower() != "sidearm")
                                    //    {
                                    //        return new Response<ObjectInstAtts>(true, null, null, "The odu installation place should be sidearm", (int)ApiReturnCode.fail);
                                    //    }
                                    //}

                                    if (OduInstallationType.Name.ToLower() == "sperate mount")
                                    {
                                        if (addMW_ODU.Height == null)
                                        {
                                            return new Response<ObjectInstAtts>(true, null, null, "The odu Height Can't Be Null ", (int)ApiReturnCode.fail);
                                        }
                                    }

                                    TLImwDish DishEntity = null;
                                    mwODU.Name = "";
                                    if (mwODU.Mw_DishId != null)
                                    {
                                        DishEntity = _unitOfWork.MW_DishRepository.GetByID((int)mwODU.Mw_DishId);
                                        mwODU.Name += DishEntity.DishName;
                                    }
                                    TLImwODULibrary ODULibraryEntity = null;
                                    if (mwODU.MwODULibraryId != null)
                                    {
                                        ODULibraryEntity = _unitOfWork.MW_ODULibraryRepository.GetByID((int)mwODU.MwODULibraryId);
                                        if (String.IsNullOrEmpty(mwODU.Name) == true)
                                        {
                                            mwODU.Name += ODULibraryEntity.Model;
                                        }
                                        else
                                        {
                                            mwODU.Name += " " + ODULibraryEntity.Model;
                                        }
                                    }
                                    TLIpolarityOnLocation PolarityOnLocationEntity = null;
                                    if (DishEntity.PolarityOnLocationId != null)
                                    {
                                        PolarityOnLocationEntity = _unitOfWork.PolarityOnLocationRepository.GetByID((int)DishEntity.PolarityOnLocationId);
                                        if (String.IsNullOrEmpty(mwODU.Name) == true)
                                        {
                                            mwODU.Name += PolarityOnLocationEntity.Name;
                                        }
                                        else
                                        {
                                            mwODU.Name += " " + PolarityOnLocationEntity.Name;
                                        }
                                    }
                                    TLIcivilLoads CheckName = _unitOfWork.CivilLoadsRepository.GetIncludeWhereFirst(x => !x.Dismantle && (x.allLoadInstId != null ?
                                       !x.allLoadInst.Draft && (x.allLoadInst.mwODUId != null ? x.allLoadInst.mwODU.Name.ToLower() == mwODU.Name.ToLower() : false) : false) &&
                                       x.SiteCode.ToLower() == SiteCode.ToLower(),
                                           x => x.allLoadInst, x => x.allLoadInst.mwODU);

                                    if (CheckName != null)
                                        return new Response<ObjectInstAtts>(true, null, null, $"This name {mwODU.Name} is already exists", (int)ApiReturnCode.fail);

                                    _unitOfWork.MW_ODURepository.Add(mwODU);
                                    _unitOfWork.SaveChanges();

                                    var Id = _unitOfWork.AllLoadInstRepository.AddAllLoadInst(LoadSubType.TLImwODU.ToString(), mwODU.Id);
                                    _unitOfWork.CivilLoadsRepository.AddCivilLoad(addMW_ODU.TLIcivilLoads, Id, SiteCode);
                                    if (addMW_ODU.TLIdynamicAttInstValue.Count > 0)
                                    {
                                        foreach (var DynamicAttInstValue in addMW_ODU.TLIdynamicAttInstValue)
                                        {
                                            _unitOfWork.DynamicAttInstValueRepository.AddDynamicInstAtts(DynamicAttInstValue, TableNameEntity.Id, mwODU.Id);
                                        }
                                    }
                                    //AddHistory(addMW_ODU.ticketAtt, Id, "Insert");
                                }
                                else
                                {
                                    return new Response<ObjectInstAtts>(true, null, null, ErrorMessage, (int)ApiReturnCode.fail);
                                }
                            }
                            else if (LoadSubType.TLImwBU.ToString() == TableName)
                            {
                                AddMW_BUViewModel addMW_BU = _mapper.Map<AddMW_BUViewModel>(MWInstallationViewModel);
                                TLImwBU mwBU = _mapper.Map<TLImwBU>(addMW_BU);
                                var Message = _unitOfWork.CivilWithLegsRepository.CheckAvailableSpaceOnCivil(addMW_BU.TLIcivilLoads.allCivilInstId).Message;
                                if (Message != "Success")
                                {
                                    return new Response<ObjectInstAtts>(true, null, null, Message, (int)ApiReturnCode.fail);
                                }
                                var mwBULibrary = _dbContext.TLImwBULibrary.Where(x => x.Id == addMW_BU.MwBULibraryId).AsNoTracking().FirstOrDefault();
                                if (mwBU.CenterHigh == 0 || mwBU.CenterHigh == null)
                                {
                                    mwBU.CenterHigh = mwBU.HBA + mwBULibrary.Length / 2;
                                }
                                var message = _unitOfWork.CivilWithLegsRepository.CheckloadsOnCivil(addMW_BU.TLIcivilLoads.allCivilInstId, 0, mwBU.Azimuth, mwBU.CenterHigh).Message;
                                if (message != "Success")
                                {
                                    return new Response<ObjectInstAtts>(true, null, null, message, (int)ApiReturnCode.fail);
                                }

                                if (addMW_BU.TLIcivilLoads.ReservedSpace == true && mwBU.SpaceInstallation == 0)
                                {
                                    mwBU.SpaceInstallation = mwBULibrary.SpaceLibrary;

                                    if (mwBULibrary.SpaceLibrary == 0)
                                    {
                                        mwBU.SpaceInstallation = mwBULibrary.Length * mwBULibrary.Width;
                                    }
                                }
                                if (addMW_BU.TLIcivilLoads.ReservedSpace == true && (addMW_BU.TLIcivilLoads.sideArmId == null || addMW_BU.TLIcivilLoads.sideArmId == 0))
                                {
                                    mwBU.EquivalentSpace = _unitOfWork.CivilWithLegsRepository.Checkspaceload(addMW_BU.TLIcivilLoads.allCivilInstId, TableName, mwBU.SpaceInstallation, mwBU.CenterHigh, addMW_BU.MwBULibraryId, addMW_BU.HBA).Data;
                                }
                                bool test = true;
                                string CheckDependencyValidation = CheckDependencyValidationForMWTypes(MWInstallationViewModel, TableName, SiteCode);

                                if (!string.IsNullOrEmpty(CheckDependencyValidation))
                                    return new Response<ObjectInstAtts>(true, null, null, CheckDependencyValidation, (int)ApiReturnCode.fail);

                                string CheckGeneralValidation = CheckGeneralValidationFunction(addMW_BU.TLIdynamicAttInstValue, TableName);

                                if (!string.IsNullOrEmpty(CheckGeneralValidation))
                                    return new Response<ObjectInstAtts>(true, null, null, CheckGeneralValidation, (int)ApiReturnCode.fail);

                                if (test == true)
                                {
                                    //TLIcivilLoads CheckName = _unitOfWork.CivilLoadsRepository.GetIncludeWhereFirst(x => !x.Dismantle && (x.allLoadInstId != null ?
                                    //    !x.allLoadInst.Draft && (x.allLoadInst.mwBUId != null ? x.allLoadInst.mwBU.Name.ToLower() == mwBU.Name.ToLower() : false) : false),
                                    //        x => x.allLoadInst, x => x.allLoadInst.mwBU);
                                    //if (CheckName != null)
                                    //    return new Response<ObjectInstAtts>(true, null, null, $"This name {mwBU.Name} is already exists", (int)ApiReturnCode.fail);

                                    var CheckSerialNumber = _unitOfWork.MW_BURepository.GetWhereFirst(x => x.Serial_Number == mwBU.Serial_Number);
                                    if (CheckSerialNumber != null)
                                    {
                                        return new Response<ObjectInstAtts>(true, null, null, $"The SerialNumber {mwBU.Serial_Number} is already exists", (int)ApiReturnCode.fail);
                                    }
                                    mwBU.Name = "";
                                    TLIsideArm SideArmEntity = null;
                                    if (addMW_BU.TLIcivilLoads.sideArmId != null)
                                    {
                                        //  SideArmEntity = _unitOfWork.SideArmRepository.GetByID((int)addMW_BU.TLIcivilLoads.sideArmId);
                                        SideArmEntity = _dbContext.TLIsideArm.Where(x => x.Id == (int)addMW_BU.TLIcivilLoads.sideArmId).AsNoTracking().FirstOrDefault();

                                        mwBU.Name = SideArmEntity.Name;
                                    }
                                    TLImwBULibrary BULibrary = null;
                                    if (mwBU.MwBULibraryId != null)
                                    {
                                        // BULibrary = _unitOfWork.MW_BULibraryRepository.GetByID((int)mwBU.MwBULibraryId);
                                        BULibrary = _dbContext.TLImwBULibrary.Where(x => x.Id == (int)mwBU.MwBULibraryId).AsNoTracking().FirstOrDefault();
                                        if (String.IsNullOrEmpty(mwBU.Name) == true)
                                        {
                                            mwBU.Name = BULibrary.Model + " " + mwBU.Height;
                                        }
                                        else
                                        {
                                            mwBU.Name += " " + BULibrary.Model + " " + mwBU.Height;
                                        }
                                    }
                                    TLIcivilLoads CheckName = _unitOfWork.CivilLoadsRepository.GetIncludeWhereFirst(x => !x.Dismantle && (x.allLoadInstId != null ?
                                       !x.allLoadInst.Draft && (x.allLoadInst.mwBUId != null ? x.allLoadInst.mwBU.Name.ToLower() == mwBU.Name.ToLower() : false) : false) &&
                                       x.SiteCode.ToLower() == SiteCode.ToLower(),
                                           x => x.allLoadInst, x => x.allLoadInst.mwBU);
                                    if (CheckName != null)
                                        return new Response<ObjectInstAtts>(true, null, null, $"This name {mwBU.Name} is already exists", (int)ApiReturnCode.fail);

                                    _unitOfWork.MW_BURepository.AddWithHistory(Helpers.LogFilterAttribute.UserId, mwBU);
                                    _unitOfWork.SaveChanges();
                                    int Id = _unitOfWork.AllLoadInstRepository.AddAllLoadInst(LoadSubType.TLImwBU.ToString(), mwBU.Id);
                                    _unitOfWork.CivilLoadsRepository.AddCivilLoad(addMW_BU.TLIcivilLoads, Id, SiteCode);
                                    if (addMW_BU.TLIdynamicAttInstValue.Count > 0)
                                    {
                                        foreach (var DynamicAttInstValue in addMW_BU.TLIdynamicAttInstValue)
                                        {
                                            _unitOfWork.DynamicAttInstValueRepository.AddDynamicInstAtts(DynamicAttInstValue, TableNameEntity.Id, mwBU.Id);
                                        }
                                    }
                                    //add ports to BU

                                    for (int i = 0; i <= 4; i++)
                                    {
                                        if (i != 4)
                                        {
                                            TLImwPort item = new TLImwPort();
                                            item.Port_Name = mwBU.Name + "_Port" + (i + 1);
                                            item.TX_Frequency = "100";
                                            item.MwBUId = mwBU.Id;
                                            item.MwBULibraryId = mwBU.MwBULibraryId;
                                            item.Port_Type = 1;
                                            _dbContext.TLImwPort.Add(item);
                                            _dbContext.SaveChanges();
                                        }
                                        else
                                        {
                                            TLImwPort item = new TLImwPort();
                                            item.Port_Name = mwBU.Name + "_Port" + (i + 1);
                                            item.TX_Frequency = "100";
                                            item.MwBUId = mwBU.Id;
                                            item.MwBULibraryId = mwBU.MwBULibraryId;
                                            item.Port_Type = 2;
                                            _dbContext.TLImwPort.Add(item);
                                            _dbContext.SaveChanges();
                                        }

                                    }
                                    //AddHistory(addMW_BU.ticketAtt, Id, "Insert");
                                }
                                else
                                {
                                    return new Response<ObjectInstAtts>(true, null, null, ErrorMessage, (int)ApiReturnCode.fail);
                                }
                            }
                            else if (LoadSubType.TLImwDish.ToString() == TableName)
                            {
                                AddMW_DishViewModel AddMW_Dish = _mapper.Map<AddMW_DishViewModel>(MWInstallationViewModel);

                                TLImwDish mwDish = _mapper.Map<TLImwDish>(AddMW_Dish);
                                var Message = _unitOfWork.CivilWithLegsRepository.CheckAvailableSpaceOnCivil(AddMW_Dish.TLIcivilLoads.allCivilInstId).Message;
                                if (Message != "Success")
                                {
                                    return new Response<ObjectInstAtts>(true, null, null, Message, (int)ApiReturnCode.fail);
                                }
                                var mwDishLibrary = _dbContext.TLImwDishLibrary.Where(x => x.Id == AddMW_Dish.MwDishLibraryId).AsNoTracking().FirstOrDefault();
                                if (mwDish.CenterHigh == 0 || mwDish.CenterHigh == null)
                                {
                                    mwDish.CenterHigh = mwDish.HBA + mwDishLibrary.Length / 2;
                                }
                                var message = _unitOfWork.CivilWithLegsRepository.CheckloadsOnCivil(AddMW_Dish.TLIcivilLoads.allCivilInstId, 0, mwDish.Azimuth, mwDish.CenterHigh).Message;
                                if (message != "Success")
                                {
                                    return new Response<ObjectInstAtts>(true, null, null, message, (int)ApiReturnCode.fail);
                                }

                                if (AddMW_Dish.TLIcivilLoads.ReservedSpace == true && mwDish.SpaceInstallation == 0)
                                {
                                    mwDish.SpaceInstallation = mwDishLibrary.SpaceLibrary;

                                    if (mwDishLibrary.SpaceLibrary == 0)
                                    {
                                        mwDish.SpaceInstallation = Convert.ToSingle(3.14) * (float)Math.Pow(mwDishLibrary.diameter / 2, 2);
                                    }
                                }
                                if (AddMW_Dish.TLIcivilLoads.ReservedSpace == true && (AddMW_Dish.TLIcivilLoads.sideArmId == null || AddMW_Dish.TLIcivilLoads.sideArmId == 0))
                                {
                                    mwDish.EquivalentSpace = _unitOfWork.CivilWithLegsRepository.Checkspaceload(AddMW_Dish.TLIcivilLoads.allCivilInstId, TableName, mwDish.SpaceInstallation, mwDish.CenterHigh, AddMW_Dish.MwDishLibraryId, AddMW_Dish.HBA).Data;
                                }
                                //TLIcivilLoads CheckName = _unitOfWork.CivilLoadsRepository.GetIncludeWhereFirst(x => !x.Dismantle && (x.allLoadInstId != null ?
                                //        !x.allLoadInst.Draft && (x.allLoadInst.mwDishId != null ? x.allLoadInst.mwDish.DishName.ToLower() == mwDish.DishName.ToLower() : false) : false),
                                //            x => x.allLoadInst, x => x.allLoadInst.mwDish);
                                //if (CheckName != null)
                                //    return new Response<ObjectInstAtts>(true, null, null, $"This name {mwDish.DishName} is already exists", (int)ApiReturnCode.fail);
                                string CheckDependencyValidation = CheckDependencyValidationForMWTypes(MWInstallationViewModel, TableName, SiteCode);

                                if (!string.IsNullOrEmpty(CheckDependencyValidation))
                                    return new Response<ObjectInstAtts>(true, null, null, CheckDependencyValidation, (int)ApiReturnCode.fail);

                                string CheckGeneralValidation = CheckGeneralValidationFunction(AddMW_Dish.TLIdynamicAttInstValue, TableName);

                                if (!string.IsNullOrEmpty(CheckGeneralValidation))
                                    return new Response<ObjectInstAtts>(true, null, null, CheckGeneralValidation, (int)ApiReturnCode.fail);

                                if (!string.IsNullOrEmpty(mwDish.Serial_Number))
                                {
                                    bool CheckSerialNumber = _unitOfWork.MW_DishRepository.Any(x => x.Serial_Number == mwDish.Serial_Number);
                                    if (CheckSerialNumber)
                                        return new Response<ObjectInstAtts>(true, null, null, $"The Serial Number {mwDish.Serial_Number} is already exists", (int)ApiReturnCode.fail);
                                }

                                bool CheckMW_LinkId = _unitOfWork.MW_DishRepository.Any(x => x.MW_LinkId == mwDish.MW_LinkId);
                                if (CheckMW_LinkId)
                                    return new Response<ObjectInstAtts>(true, null, null, $"The MW_LinkId {mwDish.MW_LinkId} is already exists", (int)ApiReturnCode.fail);

                                mwDish.DishName = AddMW_Dish.TLIcivilLoads.sideArmId != null ?
                                    $"{_unitOfWork.SideArmRepository.GetByID((int)AddMW_Dish.TLIcivilLoads.sideArmId).Name} {AddMW_Dish.HeightBase} {AddMW_Dish.Azimuth}" :
                                    AddMW_Dish.HeightBase + " " + AddMW_Dish.Azimuth;

                                TLImwDishLibrary DishLibrary = _unitOfWork.MW_DishLibraryRepository.GetByID(AddMW_Dish.MwDishLibraryId);
                                TLIcivilLoads CheckName = _unitOfWork.CivilLoadsRepository.GetIncludeWhereFirst(x => !x.Dismantle && (x.allLoadInstId != null ?
                                    !x.allLoadInst.Draft && (x.allLoadInst.mwDishId != null ? x.allLoadInst.mwDish.DishName.ToLower() == mwDish.DishName.ToLower() : false) : false) &&
                                        x.SiteCode.ToLower() == SiteCode.ToLower(),
                                        x => x.allLoadInst, x => x.allLoadInst.mwDish);
                                if (CheckName != null)
                                    return new Response<ObjectInstAtts>(true, null, null, $"This name {mwDish.DishName} is already exists", (int)ApiReturnCode.fail);

                                if (AddMW_Dish.InstallationPlaceId != null)
                                {
                                    TLIinstallationPlace InstallationPlaceEntity = _unitOfWork.InstallationPlaceRepository.GetByID(AddMW_Dish.InstallationPlaceId.Value);

                                    if (InstallationPlaceEntity.Name.ToLower() == "direct")
                                        if (AddMW_Dish.TLIcivilLoads.allCivilInstId == 0 || AddMW_Dish.TLIcivilLoads.sideArmId != null)
                                            return new Response<ObjectInstAtts>(true, null, null, "The dish if installateion place is direct then should be on civil without sideArm", (int)ApiReturnCode.fail);

                                        else if (InstallationPlaceEntity.Name.ToLower() == "sidearm")
                                            if (AddMW_Dish.TLIcivilLoads.allCivilInstId == 0 || AddMW_Dish.TLIcivilLoads.sideArmId == null)
                                                return new Response<ObjectInstAtts>(true, null, null, "The dish if installateion place is direct then should be on civil by sideArm", (int)ApiReturnCode.fail);
                                }
                                if (AddMW_Dish.ItemConnectToId != null)
                                {
                                    TLIitemConnectTo ConnectedToEntity = _unitOfWork.ItemConnectToRepository.GetByID(AddMW_Dish.ItemConnectToId.Value);
                                    if (ConnectedToEntity.Name.ToLower() == "farsitedish")
                                        if (string.IsNullOrEmpty(AddMW_Dish.Far_End_Site_Code))
                                            return new Response<ObjectInstAtts>(true, null, null, "Far Site Code Shouldn't be null if dish connected to FarSiteDish", (int)ApiReturnCode.fail);
                                        else if (ConnectedToEntity.Name.ToLower() == "repeater")
                                        {
                                            if (AddMW_Dish.RepeaterTypeId == null)
                                            {
                                                return new Response<ObjectInstAtts>(true, null, null, "if dish connected to repeater then repeater type shouldn't be null", (int)ApiReturnCode.fail);
                                            }
                                            else
                                            {
                                                TLIrepeaterType RepeaterTypeEntity = null;
                                                if (AddMW_Dish.RepeaterTypeId != null)
                                                {
                                                    RepeaterTypeEntity = _unitOfWork.RepeaterTypeRepository.GetByID((int)AddMW_Dish.RepeaterTypeId);
                                                    if (RepeaterTypeEntity.Name.ToLower() != "active" && RepeaterTypeEntity.Name.ToLower() != "passive")
                                                    {
                                                        return new Response<ObjectInstAtts>(true, null, null, "if dish connected to repeater then repeater type should be active or passive", (int)ApiReturnCode.fail);
                                                    }
                                                    if (RepeaterTypeEntity.Name.ToLower() == "active")
                                                    {
                                                        if (String.IsNullOrEmpty(AddMW_Dish.Far_End_Site_Code))
                                                        {
                                                            return new Response<ObjectInstAtts>(true, null, null, "Far Site Code Shouldn't be null if repeater type is active", (int)ApiReturnCode.fail);
                                                        }
                                                    }
                                                    else if (RepeaterTypeEntity.Name.ToLower() == "passive")
                                                    {
                                                        if (ConnectedToEntity.Name.ToLower() != "repeater" || RepeaterTypeEntity.Name.ToLower() != "passive")
                                                        {
                                                            return new Response<ObjectInstAtts>(true, null, null, "The dish should be connected to repeater and repeater type is passive", (int)ApiReturnCode.fail);
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                }
                                _unitOfWork.MW_DishRepository.AddWithHistory(Helpers.LogFilterAttribute.UserId, mwDish);
                                _unitOfWork.SaveChanges();
                                int Id = _unitOfWork.AllLoadInstRepository.AddAllLoadInst(LoadSubType.TLImwDish.ToString(), mwDish.Id);
                                _unitOfWork.CivilLoadsRepository.AddCivilLoad(AddMW_Dish.TLIcivilLoads, Id, SiteCode);
                                if (AddMW_Dish.TLIdynamicAttInstValue.Count > 0)
                                {
                                    foreach (var DynamicAttInstValue in AddMW_Dish.TLIdynamicAttInstValue)
                                    {
                                        _unitOfWork.DynamicAttInstValueRepository.AddDynamicInstAtts(DynamicAttInstValue, TableNameEntity.Id, mwDish.Id);
                                    }
                                }
                            }
                            else if (LoadSubType.TLImwRFU.ToString() == TableName)
                            {
                                AddMW_RFUViewModel AddMW_RFU = _mapper.Map<AddMW_RFUViewModel>(MWInstallationViewModel);
                                TLImwRFU mwRFU = _mapper.Map<TLImwRFU>(AddMW_RFU);
                                if (AddMW_RFU.MwPortId == null)
                                {
                                    var Message = _unitOfWork.CivilWithLegsRepository.CheckAvailableSpaceOnCivil(AddMW_RFU.TLIcivilLoads.allCivilInstId).Message;
                                    if (Message != "Success")
                                    {
                                        return new Response<ObjectInstAtts>(true, null, null, Message, (int)ApiReturnCode.fail);
                                    }
                                    var mwRFULibrary = _dbContext.TLImwRFULibrary.Where(x => x.Id == AddMW_RFU.MwRFULibraryId).FirstOrDefault();
                                    if (mwRFU.CenterHigh == 0 || mwRFU.CenterHigh == null)
                                    {
                                        mwRFU.CenterHigh = mwRFU.HBA + mwRFULibrary.Length / 2;
                                    }
                                    if (AddMW_RFU.TLIcivilLoads.ReservedSpace == true && mwRFU.SpaceInstallation == 0)
                                    {
                                        mwRFU.SpaceInstallation = mwRFULibrary.SpaceLibrary;

                                        if (mwRFULibrary.SpaceLibrary == 0)
                                        {
                                            mwRFU.SpaceInstallation = mwRFULibrary.Length * mwRFULibrary.Width;
                                        }
                                    }
                                    if (AddMW_RFU.TLIcivilLoads.ReservedSpace == true && (AddMW_RFU.TLIcivilLoads.sideArmId == null || AddMW_RFU.TLIcivilLoads.sideArmId == 0))
                                    {
                                        mwRFU.EquivalentSpace = _unitOfWork.CivilWithLegsRepository.Checkspaceload(AddMW_RFU.TLIcivilLoads.allCivilInstId, TableName, mwRFU.SpaceInstallation, mwRFU.CenterHigh, AddMW_RFU.MwRFULibraryId, AddMW_RFU.HBA).Data;
                                    }
                                }

                                bool test = false;
                                if (AddMW_RFU.TLIdynamicAttInstValue != null ? AddMW_RFU.TLIdynamicAttInstValue.Count > 0 : false)
                                {
                                    string CheckDependencyValidation = CheckDependencyValidationForMWTypes(MWInstallationViewModel, TableName, SiteCode);

                                    if (!string.IsNullOrEmpty(CheckDependencyValidation))
                                        return new Response<ObjectInstAtts>(true, null, null, CheckDependencyValidation, (int)ApiReturnCode.fail);

                                    string CheckGeneralValidation = CheckGeneralValidationFunction(AddMW_RFU.TLIdynamicAttInstValue, TableName);

                                    if (!string.IsNullOrEmpty(CheckGeneralValidation))
                                        return new Response<ObjectInstAtts>(true, null, null, CheckGeneralValidation, (int)ApiReturnCode.fail);

                                    test = true;
                                }
                                else
                                {
                                    test = true;
                                }
                                if (test == true)
                                {
                                    TLIcivilLoads CheckName = _unitOfWork.CivilLoadsRepository.GetIncludeWhereFirst(x => !x.Dismantle && (x.allLoadInstId != null ?
                                        !x.allLoadInst.Draft && (x.allLoadInst.mwRFUId != null ? x.allLoadInst.mwRFU.Name.ToLower() == mwRFU.Name.ToLower() : false) : false) &&
                                        x.SiteCode.ToLower() == SiteCode.ToLower(),
                                            x => x.allLoadInst, x => x.allLoadInst.mwRFU);
                                    if (CheckName != null)
                                        return new Response<ObjectInstAtts>(true, null, null, $"This name {mwRFU.Name} is already exists", (int)ApiReturnCode.fail);

                                    var CheckSerialNumber = _unitOfWork.MW_RFURepository.GetWhereFirst(x => x.SerialNumber == mwRFU.SerialNumber);
                                    if (CheckSerialNumber != null)
                                    {
                                        return new Response<ObjectInstAtts>(true, null, null, $"The SerialNumber {mwRFU.SerialNumber} is already exists", (int)ApiReturnCode.fail);
                                    }

                                    _unitOfWork.MW_RFURepository.AddWithHistory(Helpers.LogFilterAttribute.UserId, mwRFU);
                                    _unitOfWork.SaveChanges();
                                    int Id = _unitOfWork.AllLoadInstRepository.AddAllLoadInst(LoadSubType.TLImwRFU.ToString(), mwRFU.Id);
                                    _unitOfWork.CivilLoadsRepository.AddCivilLoad(AddMW_RFU.TLIcivilLoads, Id, SiteCode);
                                    if (AddMW_RFU.TLIdynamicAttInstValue.Count > 0)
                                    {
                                        foreach (var DynamicAttInstValue in AddMW_RFU.TLIdynamicAttInstValue)
                                        {
                                            _unitOfWork.DynamicAttInstValueRepository.AddDynamicInstAtts(DynamicAttInstValue, TableNameEntity.Id, mwRFU.Id);
                                        }
                                    }
                                }    //AddHistory(AddMW_RFU.ticketAtt, Id, "Insert");

                                else
                                {
                                    return new Response<ObjectInstAtts>(true, null, null, ErrorMessage, (int)ApiReturnCode.fail);
                                }

                            }
                            else if (LoadSubType.TLImwOther.ToString() == TableName)
                            {
                                AddMw_OtherViewModel AddMW_Other = _mapper.Map<AddMw_OtherViewModel>(MWInstallationViewModel);
                                TLImwOther mwOther = _mapper.Map<TLImwOther>(AddMW_Other);
                                var Message = _unitOfWork.CivilWithLegsRepository.CheckAvailableSpaceOnCivil(AddMW_Other.TLIcivilLoads.allCivilInstId).Message;
                                if (Message != "Success")
                                {
                                    return new Response<ObjectInstAtts>(true, null, null, Message, (int)ApiReturnCode.fail);
                                }
                                var mwOtherLibrary = _dbContext.TLImwOtherLibrary.Where(x => x.Id == AddMW_Other.mwOtherLibraryId).FirstOrDefault();
                                if (mwOther.CenterHigh == 0 || mwOther.CenterHigh == null)
                                {
                                    mwOther.CenterHigh = mwOther.HBA + mwOtherLibrary.Length / 2;
                                }
                                if (AddMW_Other.TLIcivilLoads.ReservedSpace == true && mwOther.Spaceinstallation == 0)
                                {
                                    mwOther.Spaceinstallation = mwOtherLibrary.SpaceLibrary;

                                    if (mwOtherLibrary.SpaceLibrary == 0)
                                    {
                                        mwOther.Spaceinstallation = mwOtherLibrary.Length * mwOtherLibrary.Width;
                                    }
                                }
                                if (AddMW_Other.TLIcivilLoads.ReservedSpace == true && (AddMW_Other.TLIcivilLoads.sideArmId == null || AddMW_Other.TLIcivilLoads.sideArmId == 0))
                                {
                                    mwOther.EquivalentSpace = _unitOfWork.CivilWithLegsRepository.Checkspaceload(AddMW_Other.TLIcivilLoads.allCivilInstId, TableName, mwOther.Spaceinstallation, mwOther.CenterHigh, AddMW_Other.mwOtherLibraryId, AddMW_Other.HBA).Data;
                                }
                                bool test = false;
                                if (AddMW_Other.TLIdynamicAttInstValue != null ? AddMW_Other.TLIdynamicAttInstValue.Count > 0 : false)
                                {
                                    string CheckDependencyValidation = CheckDependencyValidationForMWTypes(MWInstallationViewModel, TableName, SiteCode);

                                    if (!string.IsNullOrEmpty(CheckDependencyValidation))
                                        return new Response<ObjectInstAtts>(true, null, null, CheckDependencyValidation, (int)ApiReturnCode.fail);

                                    string CheckGeneralValidation = CheckGeneralValidationFunction(AddMW_Other.TLIdynamicAttInstValue, TableName);

                                    if (!string.IsNullOrEmpty(CheckGeneralValidation))
                                        return new Response<ObjectInstAtts>(true, null, null, CheckGeneralValidation, (int)ApiReturnCode.fail);

                                    test = true;
                                }
                                else
                                {
                                    test = true;
                                }
                                if (test == true)
                                {
                                    TLIcivilLoads CheckName = _unitOfWork.CivilLoadsRepository.GetIncludeWhereFirst(x => !x.Dismantle && (x.allLoadInstId != null ?
                                        !x.allLoadInst.Draft && (x.allLoadInst.mwOtherId != null ? x.allLoadInst.mwOther.Name.ToLower() == mwOther.Name.ToLower() : false) : false) &&
                                        x.SiteCode.ToLower() == SiteCode.ToLower(),
                                            x => x.allLoadInst, x => x.allLoadInst.mwOther);
                                    if (CheckName != null)
                                        return new Response<ObjectInstAtts>(true, null, null, $"This name {mwOther.Name} is already exists", (int)ApiReturnCode.fail);

                                    var CheckSerialNumber = _unitOfWork.Mw_OtherRepository.GetWhereFirst(x => x.SerialNumber == mwOther.SerialNumber);
                                    if (CheckSerialNumber != null)
                                    {
                                        return new Response<ObjectInstAtts>(true, null, null, $"The SerialNumber {mwOther.SerialNumber} is already exists", (int)ApiReturnCode.fail);
                                    }

                                    _unitOfWork.Mw_OtherRepository.AddWithHistory(Helpers.LogFilterAttribute.UserId, mwOther);
                                    _unitOfWork.SaveChanges();
                                    int Id = _unitOfWork.AllLoadInstRepository.AddAllLoadInst(LoadSubType.TLImwOther.ToString(), mwOther.Id);
                                    _unitOfWork.CivilLoadsRepository.AddCivilLoad(AddMW_Other.TLIcivilLoads, Id, SiteCode);
                                    if (AddMW_Other.TLIdynamicAttInstValue.Count > 0)
                                    {
                                        foreach (var DynamicAttInstValue in AddMW_Other.TLIdynamicAttInstValue)
                                        {
                                            _unitOfWork.DynamicAttInstValueRepository.AddDynamicInstAtts(DynamicAttInstValue, TableNameEntity.Id, mwOther.Id);
                                        }
                                    }

                                }
                                else
                                {
                                    return new Response<ObjectInstAtts>(true, null, null, ErrorMessage, (int)ApiReturnCode.fail);
                                }
                            }

                            transaction.Complete();
                            tran.Commit();
                            return new Response<ObjectInstAtts>();
                        }
                        catch (Exception err)
                        {

                            tran.Rollback();
                            return new Response<ObjectInstAtts>(true, null, null, err.Message, (int)ApiReturnCode.fail);
                        }
                    }
                }
            }

        }
        //Function take 3 parameters MWInstallationViewModel, TableName
        //First get table name Entity depened on TableName
        //Map MWInstallationViewModel object to ViewModel
        //Map ViewModel to Entity
        //Update entity
        //Update dynamic attributes
        public async Task<Response<ObjectInstAtts>> EditMWInstallation(object MWInstallationViewModel, string TableName)
        {
            try
            {
                int TableNameId = 0;
                if (LoadSubType.TLImwODU.ToString() == TableName)
                {
                    TableNameId = _unitOfWork.TablesNamesRepository.GetWhereFirst(x => x.TableName.ToLower() == TablesNames.TLImwODU.ToString().ToLower()).Id;
                    EditMW_ODUViewModel MW_ODUViewModel = _mapper.Map<EditMW_ODUViewModel>(MWInstallationViewModel);

                    TLIcivilLoads CivilLoads = _unitOfWork.CivilLoadsRepository.GetIncludeWhereFirst(x => !x.Dismantle && (x.allLoadInstId != null ?
                        x.allLoadInst.mwODUId == MW_ODUViewModel.Id : false), x => x.allLoadInst);

                    string SiteCode = "";

                    if (CivilLoads != null)
                        SiteCode = CivilLoads.SiteCode;

                    else
                        SiteCode = null;

                    TLIcivilLoads CheckName = _unitOfWork.CivilLoadsRepository.GetIncludeWhereFirst(x => !x.Dismantle && x.allLoadInst.mwODUId != MW_ODUViewModel.Id && (x.allLoadInstId != null ?
                        !x.allLoadInst.Draft && (x.allLoadInst.mwODUId != null ? x.allLoadInst.mwODU.Name.ToLower() == MW_ODUViewModel.Name.ToLower() : false) : false) &&
                        x.SiteCode.ToLower() == SiteCode.ToLower(),
                            x => x.allLoadInst, x => x.allLoadInst.mwODU);

                    if (CheckName != null)
                        return new Response<ObjectInstAtts>(true, null, null, $"This name [{MW_ODUViewModel.Name}] is already exists", (int)ApiReturnCode.fail);

                    string CheckGeneralValidation = CheckGeneralValidationFunctionEditVersion(MW_ODUViewModel.DynamicInstAttsValue, TableName);

                    if (!string.IsNullOrEmpty(CheckGeneralValidation))
                        return new Response<ObjectInstAtts>(true, null, null, CheckGeneralValidation, (int)ApiReturnCode.fail);

                    string CheckDependencyValidation = CheckDependencyValidationEditVersion(MWInstallationViewModel, SiteCode, TableName);

                    if (!string.IsNullOrEmpty(CheckDependencyValidation))
                        return new Response<ObjectInstAtts>(true, null, null, CheckDependencyValidation, (int)ApiReturnCode.fail);

                    TLImwODU mwODU = _mapper.Map<TLImwODU>(MW_ODUViewModel);
                    TLImwODU OldMW_ODUViewModel = _unitOfWork.MW_ODURepository.GetAllAsQueryable().AsNoTracking().FirstOrDefault(x => x.Id == MW_ODUViewModel.Id);

                    _unitOfWork.MW_ODURepository.UpdateWithHistory(Helpers.LogFilterAttribute.UserId, OldMW_ODUViewModel, mwODU);
                    var allloads = _unitOfWork.AllLoadInstRepository.GetWhereFirst(x => x.mwODUId == MW_ODUViewModel.Id).Id;
                    var civilloads = _unitOfWork.CivilLoadsRepository.GetWhereFirst(x => x.allLoadInstId == allloads);
                    CivilLoads.InstallationDate = MW_ODUViewModel.TLIcivilLoads.InstallationDate;
                    CivilLoads.ItemOnCivilStatus = MW_ODUViewModel.TLIcivilLoads.ItemOnCivilStatus;
                    CivilLoads.ItemStatus = MW_ODUViewModel.TLIcivilLoads.ItemStatus;
                    CivilLoads.ReservedSpace = MW_ODUViewModel.TLIcivilLoads.ReservedSpace;
                    CivilLoads.sideArmId = MW_ODUViewModel.TLIcivilLoads.sideArmId;
                    CivilLoads.allCivilInstId = MW_ODUViewModel.TLIcivilLoads.allCivilInstId;
                    CivilLoads.legId = MW_ODUViewModel.TLIcivilLoads.legId;
                    CivilLoads.Leg2Id = MW_ODUViewModel.TLIcivilLoads.Leg2Id;

                    _unitOfWork.SaveChanges();
                    if (MW_ODUViewModel.DynamicInstAttsValue != null ? MW_ODUViewModel.DynamicInstAttsValue.Count() > 0 : false)
                        _unitOfWork.DynamicAttInstValueRepository.UpdateDynamicValue(MW_ODUViewModel.DynamicInstAttsValue, TableNameId, mwODU.Id);

                    await _unitOfWork.SaveChangesAsync();
                }
                else if (LoadSubType.TLImwBU.ToString() == TableName)
                {
                    TableNameId = _unitOfWork.TablesNamesRepository.GetWhereFirst(x => x.TableName.ToLower() == TablesNames.TLImwBU.ToString().ToLower()).Id;
                    EditMW_BUViewModel MW_BUViewModel = _mapper.Map<EditMW_BUViewModel>(MWInstallationViewModel);

                    TLIcivilLoads CivilLoads = _unitOfWork.CivilLoadsRepository.GetIncludeWhereFirst(x => !x.Dismantle && (x.allLoadInstId != null ?
                        x.allLoadInst.mwBUId == MW_BUViewModel.Id : false), x => x.allLoadInst);

                    string SiteCode = "";

                    if (CivilLoads != null)
                        SiteCode = CivilLoads.SiteCode;

                    else
                        SiteCode = null;

                    TLIcivilLoads CheckName = _unitOfWork.CivilLoadsRepository.GetIncludeWhereFirst(x => !x.Dismantle && x.allLoadInst.mwBUId != MW_BUViewModel.Id && (x.allLoadInstId != null ?
                        !x.allLoadInst.Draft && (x.allLoadInst.mwBUId != null ? x.allLoadInst.mwBU.Name.ToLower() == MW_BUViewModel.Name.ToLower() : false) : false) &&
                        x.SiteCode.ToLower() == SiteCode.ToLower(),
                            x => x.allLoadInst, x => x.allLoadInst.mwBU);

                    if (CheckName != null)
                        return new Response<ObjectInstAtts>(true, null, null, $"This name [{MW_BUViewModel.Name}] is already exists", (int)ApiReturnCode.fail);

                    string CheckGeneralValidation = CheckGeneralValidationFunctionEditVersion(MW_BUViewModel.DynamicInstAttsValue, TableName);

                    if (!string.IsNullOrEmpty(CheckGeneralValidation))
                        return new Response<ObjectInstAtts>(true, null, null, CheckGeneralValidation, (int)ApiReturnCode.fail);

                    string CheckDependencyValidation = CheckDependencyValidationEditVersion(MWInstallationViewModel, SiteCode, TableName);

                    if (!string.IsNullOrEmpty(CheckDependencyValidation))
                        return new Response<ObjectInstAtts>(true, null, null, CheckDependencyValidation, (int)ApiReturnCode.fail);

                    TLImwBU mwBU = _mapper.Map<TLImwBU>(MW_BUViewModel);
                    var mw_BUInst = _unitOfWork.MW_BURepository.GetAllAsQueryable().AsNoTracking().FirstOrDefault(x => x.Id == MW_BUViewModel.Id);
                    if (mwBU.HBA == mw_BUInst.HBA && mwBU.CenterHigh == mw_BUInst.CenterHigh && mwBU.SpaceInstallation == mw_BUInst.SpaceInstallation && mwBU.Azimuth != mw_BUInst.Azimuth && MW_BUViewModel.TLIcivilLoads.ReservedSpace == true)
                    {
                        var message = _unitOfWork.CivilWithLegsRepository.CheckloadsOnCivil(MW_BUViewModel.TLIcivilLoads.allCivilInstId, 0, mwBU.Azimuth, mwBU.CenterHigh).Message;
                        if (message != "Success")
                        {
                            return new Response<ObjectInstAtts>(true, null, null, message, (int)ApiReturnCode.fail);
                        }
                    }
                    if (mwBU.HBA != mw_BUInst.HBA || mwBU.CenterHigh != mw_BUInst.CenterHigh || mwBU.SpaceInstallation != mw_BUInst.SpaceInstallation && MW_BUViewModel.TLIcivilLoads.ReservedSpace == true)
                    {
                        var mwBULibrary = _dbContext.TLImwBULibrary.Where(x => x.Id == mwBU.MwBULibraryId).FirstOrDefault();
                        if (mwBU.CenterHigh == 0 || mwBU.CenterHigh == null)
                        {
                            mwBU.CenterHigh = mwBU.HBA + mwBULibrary.Length / 2;
                        }
                        var message = _unitOfWork.CivilWithLegsRepository.CheckloadsOnCivil(MW_BUViewModel.TLIcivilLoads.allCivilInstId, 0, mwBU.Azimuth, mwBU.CenterHigh).Message;
                        if (message != "Success")
                        {
                            return new Response<ObjectInstAtts>(true, null, null, message, (int)ApiReturnCode.fail);
                        }
                        if (MW_BUViewModel.TLIcivilLoads.ReservedSpace == true && (MW_BUViewModel.TLIcivilLoads.sideArmId == null || MW_BUViewModel.TLIcivilLoads.sideArmId == 0))
                        {
                            mwBU.EquivalentSpace = _unitOfWork.CivilWithLegsRepository.Checkspaceload(MW_BUViewModel.TLIcivilLoads.allCivilInstId, TableName, mwBU.SpaceInstallation, mwBU.CenterHigh, mwBU.MwBULibraryId, mwBU.HBA).Data;
                        }
                    }
                    _unitOfWork.MW_BURepository.UpdateWithHistory(Helpers.LogFilterAttribute.UserId, mw_BUInst, mwBU);
                    var allloads = _unitOfWork.AllLoadInstRepository.GetWhereFirst(x => x.mwBUId == MW_BUViewModel.Id).Id;
                    var civilloads = _unitOfWork.CivilLoadsRepository.GetWhereFirst(x => x.allLoadInstId == allloads);
                    CivilLoads.InstallationDate = MW_BUViewModel.TLIcivilLoads.InstallationDate;
                    CivilLoads.ItemOnCivilStatus = MW_BUViewModel.TLIcivilLoads.ItemOnCivilStatus;
                    CivilLoads.ItemStatus = MW_BUViewModel.TLIcivilLoads.ItemStatus;
                    CivilLoads.ReservedSpace = MW_BUViewModel.TLIcivilLoads.ReservedSpace;
                    CivilLoads.sideArmId = MW_BUViewModel.TLIcivilLoads.sideArmId;
                    CivilLoads.allCivilInstId = MW_BUViewModel.TLIcivilLoads.allCivilInstId;
                    CivilLoads.legId = MW_BUViewModel.TLIcivilLoads.legId;
                    CivilLoads.Leg2Id = MW_BUViewModel.TLIcivilLoads.Leg2Id;

                    _unitOfWork.SaveChanges();
                    if (MW_BUViewModel.DynamicInstAttsValue != null ? MW_BUViewModel.DynamicInstAttsValue.Count > 0 : false)
                        _unitOfWork.DynamicAttInstValueRepository.UpdateDynamicValue(MW_BUViewModel.DynamicInstAttsValue, TableNameId, mwBU.Id);

                    await _unitOfWork.SaveChangesAsync();
                }
                else if (LoadSubType.TLImwDish.ToString() == TableName)
                {
                    TableNameId = _unitOfWork.TablesNamesRepository.GetWhereFirst(x => x.TableName.ToLower() == TablesNames.TLImwDish.ToString().ToLower()).Id;
                    EditMW_DishViewModel MW_DishViewModel = _mapper.Map<EditMW_DishViewModel>(MWInstallationViewModel);

                    TLIcivilLoads CivilLoads = _unitOfWork.CivilLoadsRepository.GetIncludeWhereFirst(x => !x.Dismantle && (x.allLoadInstId != null ?
                        x.allLoadInst.mwDishId == MW_DishViewModel.Id : false), x => x.allLoadInst);

                    string SiteCode = "";

                    if (CivilLoads != null)
                        SiteCode = CivilLoads.SiteCode;

                    else
                        SiteCode = null;

                    TLIcivilLoads CheckName = _unitOfWork.CivilLoadsRepository.GetIncludeWhereFirst(x => !x.Dismantle && x.allLoadInst.mwDishId != MW_DishViewModel.Id && (x.allLoadInstId != null ?
                        !x.allLoadInst.Draft && (x.allLoadInst.mwDishId != null ? x.allLoadInst.mwDish.DishName.ToLower() == MW_DishViewModel.DishName.ToLower() : false) : false) &&
                        x.SiteCode.ToLower() == SiteCode.ToLower(),
                            x => x.allLoadInst, x => x.allLoadInst.mwDish);

                    if (CheckName != null)
                        return new Response<ObjectInstAtts>(true, null, null, $"This name [{MW_DishViewModel.DishName}] is already exists", (int)ApiReturnCode.fail);

                    string CheckGeneralValidation = CheckGeneralValidationFunctionEditVersion(MW_DishViewModel.DynamicInstAttsValue, TableName);

                    if (!string.IsNullOrEmpty(CheckGeneralValidation))
                        return new Response<ObjectInstAtts>(true, null, null, CheckGeneralValidation, (int)ApiReturnCode.fail);

                    string CheckDependencyValidation = CheckDependencyValidationEditVersion(MWInstallationViewModel, SiteCode, TableName);

                    if (!string.IsNullOrEmpty(CheckDependencyValidation))
                        return new Response<ObjectInstAtts>(true, null, null, CheckDependencyValidation, (int)ApiReturnCode.fail);

                    TLImwDish mwDish = _mapper.Map<TLImwDish>(MW_DishViewModel);
                    TLImwDish OldMW_DishViewModel = _unitOfWork.MW_DishRepository.GetAllAsQueryable().AsNoTracking().FirstOrDefault(x => x.Id == MW_DishViewModel.Id);
                    if (mwDish.HBA == OldMW_DishViewModel.HBA && mwDish.CenterHigh == OldMW_DishViewModel.CenterHigh && mwDish.SpaceInstallation == OldMW_DishViewModel.SpaceInstallation && mwDish.Azimuth != OldMW_DishViewModel.Azimuth && MW_DishViewModel.TLIcivilLoads.ReservedSpace == true)
                    {
                        var message = _unitOfWork.CivilWithLegsRepository.CheckloadsOnCivil(MW_DishViewModel.TLIcivilLoads.allCivilInstId, 0, mwDish.Azimuth, mwDish.CenterHigh).Message;
                        if (message != "Success")
                        {
                            return new Response<ObjectInstAtts>(true, null, null, message, (int)ApiReturnCode.fail);
                        }
                    }
                    if (mwDish.HBA != OldMW_DishViewModel.HBA || mwDish.CenterHigh != OldMW_DishViewModel.CenterHigh || mwDish.SpaceInstallation != OldMW_DishViewModel.SpaceInstallation && MW_DishViewModel.TLIcivilLoads.ReservedSpace == true)
                    {
                        var mwDishLibrary = _dbContext.TLImwDishLibrary.Where(x => x.Id == mwDish.MwDishLibraryId).FirstOrDefault();
                        if (mwDish.CenterHigh == 0 || mwDish.CenterHigh == null)
                        {
                            mwDish.CenterHigh = mwDish.HBA + mwDishLibrary.Length / 2;
                        }
                        var message = _unitOfWork.CivilWithLegsRepository.CheckloadsOnCivil(MW_DishViewModel.TLIcivilLoads.allCivilInstId, 0, mwDish.Azimuth, mwDish.CenterHigh).Message;
                        if (message != "Success")
                        {
                            return new Response<ObjectInstAtts>(true, null, null, message, (int)ApiReturnCode.fail);
                        }
                        if (MW_DishViewModel.TLIcivilLoads.ReservedSpace == true && (MW_DishViewModel.TLIcivilLoads.sideArmId == null || MW_DishViewModel.TLIcivilLoads.sideArmId == 0))
                        {
                            mwDish.EquivalentSpace = _unitOfWork.CivilWithLegsRepository.Checkspaceload(MW_DishViewModel.TLIcivilLoads.allCivilInstId, TableName, mwDish.SpaceInstallation, mwDish.CenterHigh, mwDish.MwDishLibraryId, mwDish.HBA).Data;
                        }
                    }
                    _unitOfWork.MW_DishRepository.UpdateWithHistory(Helpers.LogFilterAttribute.UserId, OldMW_DishViewModel, mwDish);
                    var allloads = _unitOfWork.AllLoadInstRepository.GetWhereFirst(x => x.mwDishId == MW_DishViewModel.Id).Id;
                    var civilloads = _unitOfWork.CivilLoadsRepository.GetWhereFirst(x => x.allLoadInstId == allloads);
                    CivilLoads.InstallationDate = MW_DishViewModel.TLIcivilLoads.InstallationDate;
                    CivilLoads.ItemOnCivilStatus = MW_DishViewModel.TLIcivilLoads.ItemOnCivilStatus;
                    CivilLoads.ItemStatus = MW_DishViewModel.TLIcivilLoads.ItemStatus;
                    CivilLoads.ReservedSpace = MW_DishViewModel.TLIcivilLoads.ReservedSpace;
                    CivilLoads.sideArmId = MW_DishViewModel.TLIcivilLoads.sideArmId;
                    CivilLoads.allCivilInstId = MW_DishViewModel.TLIcivilLoads.allCivilInstId;
                    CivilLoads.legId = MW_DishViewModel.TLIcivilLoads.legId;
                    CivilLoads.Leg2Id = MW_DishViewModel.TLIcivilLoads.Leg2Id;

                    _unitOfWork.SaveChanges();
                    if (MW_DishViewModel.DynamicInstAttsValue != null ? MW_DishViewModel.DynamicInstAttsValue.Count > 0 : false)
                    {
                        _unitOfWork.DynamicAttInstValueRepository.UpdateDynamicValue(MW_DishViewModel.DynamicInstAttsValue, TableNameId, mwDish.Id);
                    }
                    await _unitOfWork.SaveChangesAsync();
                }
                else if (LoadSubType.TLImwRFU.ToString() == TableName)
                {
                    TableNameId = _unitOfWork.TablesNamesRepository.GetWhereFirst(x => x.TableName.ToLower() == TablesNames.TLImwRFU.ToString().ToLower()).Id;
                    EditMW_RFUViewModel MW_RFUViewModel = _mapper.Map<EditMW_RFUViewModel>(MWInstallationViewModel);

                    TLIcivilLoads CivilLoads = _unitOfWork.CivilLoadsRepository.GetIncludeWhereFirst(x => !x.Dismantle && (x.allLoadInstId != null ?
                        x.allLoadInst.mwRFUId == MW_RFUViewModel.Id : false), x => x.allLoadInst);

                    string SiteCode = "";

                    if (CivilLoads != null)
                        SiteCode = CivilLoads.SiteCode;

                    else
                        SiteCode = null;

                    TLIcivilLoads CheckName = _unitOfWork.CivilLoadsRepository.GetIncludeWhereFirst(x => !x.Dismantle && x.allLoadInst.mwRFUId != MW_RFUViewModel.Id && (x.allLoadInstId != null ?
                        !x.allLoadInst.Draft && (x.allLoadInst.mwRFUId != null ? x.allLoadInst.mwRFU.Name.ToLower() == MW_RFUViewModel.Name.ToLower() : false) : false) &&
                        x.SiteCode.ToLower() == SiteCode.ToLower(),
                            x => x.allLoadInst, x => x.allLoadInst.mwRFU);

                    if (CheckName != null)
                        return new Response<ObjectInstAtts>(true, null, null, $"This name [{MW_RFUViewModel.Name}] is already exists", (int)ApiReturnCode.fail);

                    string CheckGeneralValidation = CheckGeneralValidationFunctionEditVersion(MW_RFUViewModel.DynamicInstAttsValue, TableName);

                    if (!string.IsNullOrEmpty(CheckGeneralValidation))
                        return new Response<ObjectInstAtts>(true, null, null, CheckGeneralValidation, (int)ApiReturnCode.fail);

                    string CheckDependencyValidation = CheckDependencyValidationEditVersion(MWInstallationViewModel, SiteCode, TableName);

                    if (!string.IsNullOrEmpty(CheckDependencyValidation))
                        return new Response<ObjectInstAtts>(true, null, null, CheckDependencyValidation, (int)ApiReturnCode.fail);

                    TLImwRFU mwRFU = _mapper.Map<TLImwRFU>(MW_RFUViewModel);
                    TLImwRFU OldMW_RFUViewModel = _unitOfWork.MW_RFURepository.GetAllAsQueryable().AsNoTracking().FirstOrDefault(x => x.Id == MW_RFUViewModel.Id);
                    if (mwRFU.HBA == OldMW_RFUViewModel.HBA && mwRFU.CenterHigh == OldMW_RFUViewModel.CenterHigh && mwRFU.SpaceInstallation == OldMW_RFUViewModel.SpaceInstallation && mwRFU.Azimuth != OldMW_RFUViewModel.Azimuth && MW_RFUViewModel.TLIcivilLoads.ReservedSpace == true)
                    {
                        var message = _unitOfWork.CivilWithLegsRepository.CheckloadsOnCivil(MW_RFUViewModel.TLIcivilLoads.allCivilInstId, 0, mwRFU.Azimuth, mwRFU.CenterHigh).Message;
                        if (message != "Success")
                        {
                            return new Response<ObjectInstAtts>(true, null, null, message, (int)ApiReturnCode.fail);
                        }
                    }
                    if (mwRFU.HBA != OldMW_RFUViewModel.HBA || mwRFU.CenterHigh != OldMW_RFUViewModel.CenterHigh || mwRFU.SpaceInstallation != OldMW_RFUViewModel.SpaceInstallation && MW_RFUViewModel.TLIcivilLoads.ReservedSpace == true)
                    {
                        var mwRFULibrary = _dbContext.TLImwRFULibrary.Where(x => x.Id == mwRFU.MwRFULibraryId).FirstOrDefault();
                        if (mwRFU.CenterHigh == 0 || mwRFU.CenterHigh == null)
                        {
                            mwRFU.CenterHigh = mwRFU.HBA + mwRFULibrary.Length / 2;
                        }
                        var message = _unitOfWork.CivilWithLegsRepository.CheckloadsOnCivil(MW_RFUViewModel.TLIcivilLoads.allCivilInstId, 0, mwRFU.Azimuth, mwRFU.CenterHigh).Message;
                        if (message != "Success")
                        {
                            return new Response<ObjectInstAtts>(true, null, null, message, (int)ApiReturnCode.fail);
                        }
                        if (MW_RFUViewModel.TLIcivilLoads.ReservedSpace == true && (MW_RFUViewModel.TLIcivilLoads.sideArmId == null || MW_RFUViewModel.TLIcivilLoads.sideArmId == 0))
                        {
                            mwRFU.EquivalentSpace = _unitOfWork.CivilWithLegsRepository.Checkspaceload(MW_RFUViewModel.TLIcivilLoads.allCivilInstId, TableName, mwRFU.SpaceInstallation, mwRFU.CenterHigh, mwRFU.MwRFULibraryId, mwRFU.HBA).Data;
                        }
                    }
                    _unitOfWork.MW_RFURepository.UpdateWithHistory(Helpers.LogFilterAttribute.UserId, OldMW_RFUViewModel, mwRFU);
                    var allloads = _unitOfWork.AllLoadInstRepository.GetWhereFirst(x => x.mwRFUId == MW_RFUViewModel.Id).Id;
                    var civilloads = _unitOfWork.CivilLoadsRepository.GetWhereFirst(x => x.allLoadInstId == allloads);
                    CivilLoads.InstallationDate = MW_RFUViewModel.TLIcivilLoads.InstallationDate;
                    CivilLoads.ItemOnCivilStatus = MW_RFUViewModel.TLIcivilLoads.ItemOnCivilStatus;
                    CivilLoads.ItemStatus = MW_RFUViewModel.TLIcivilLoads.ItemStatus;
                    CivilLoads.ReservedSpace = MW_RFUViewModel.TLIcivilLoads.ReservedSpace;
                    CivilLoads.sideArmId = MW_RFUViewModel.TLIcivilLoads.sideArmId;
                    CivilLoads.allCivilInstId = MW_RFUViewModel.TLIcivilLoads.allCivilInstId;
                    CivilLoads.legId = MW_RFUViewModel.TLIcivilLoads.legId;
                    CivilLoads.Leg2Id = MW_RFUViewModel.TLIcivilLoads.Leg2Id;

                    _unitOfWork.SaveChanges();
                    if (MW_RFUViewModel.DynamicInstAttsValue.Count > 0)
                    {
                        _unitOfWork.DynamicAttInstValueRepository.UpdateDynamicValue(MW_RFUViewModel.DynamicInstAttsValue, TableNameId, mwRFU.Id);
                    }
                    await _unitOfWork.SaveChangesAsync();
                }
                else if (LoadSubType.TLImwOther.ToString() == TableName)
                {
                    TableNameId = _unitOfWork.TablesNamesRepository.GetWhereFirst(x => x.TableName.ToLower() == TablesNames.TLImwOther.ToString().ToLower()).Id;
                    EditMw_OtherViewModel Mw_OtherViewModel = _mapper.Map<EditMw_OtherViewModel>(MWInstallationViewModel);

                    TLIcivilLoads CivilLoads = _unitOfWork.CivilLoadsRepository.GetIncludeWhereFirst(x => !x.Dismantle && (x.allLoadInstId != null ?
                        x.allLoadInst.mwOtherId == Mw_OtherViewModel.Id : false), x => x.allLoadInst);

                    string SiteCode = "";

                    if (CivilLoads != null)
                        SiteCode = CivilLoads.SiteCode;

                    else
                        SiteCode = null;

                    TLIcivilLoads CheckName = _unitOfWork.CivilLoadsRepository.GetIncludeWhereFirst(x => !x.Dismantle && x.allLoadInst.mwOtherId != Mw_OtherViewModel.Id && (x.allLoadInstId != null ?
                        !x.allLoadInst.Draft && (x.allLoadInst.mwOtherId != null ? x.allLoadInst.mwOther.Name.ToLower() == Mw_OtherViewModel.Name.ToLower() : false) : false) &&
                        x.SiteCode.ToLower() == SiteCode.ToLower(),
                            x => x.allLoadInst, x => x.allLoadInst.mwOther);

                    if (CheckName != null)
                        return new Response<ObjectInstAtts>(true, null, null, $"This name [{Mw_OtherViewModel.Name}] is already exists", (int)ApiReturnCode.fail);

                    string CheckGeneralValidation = CheckGeneralValidationFunctionEditVersion(Mw_OtherViewModel.DynamicInstAttsValue, TableName);

                    if (!string.IsNullOrEmpty(CheckGeneralValidation))
                        return new Response<ObjectInstAtts>(true, null, null, CheckGeneralValidation, (int)ApiReturnCode.fail);

                    string CheckDependencyValidation = CheckDependencyValidationEditVersion(MWInstallationViewModel, SiteCode, TableName);

                    if (!string.IsNullOrEmpty(CheckDependencyValidation))
                        return new Response<ObjectInstAtts>(true, null, null, CheckDependencyValidation, (int)ApiReturnCode.fail);

                    TLImwOther mwOther = _mapper.Map<TLImwOther>(Mw_OtherViewModel);
                    TLImwOther OldMw_OtherViewModel = _unitOfWork.Mw_OtherRepository.GetAllAsQueryable().AsNoTracking().FirstOrDefault(x => x.Id == Mw_OtherViewModel.Id);
                    if (mwOther.HBA != OldMw_OtherViewModel.HBA || mwOther.CenterHigh != OldMw_OtherViewModel.CenterHigh || mwOther.Spaceinstallation != OldMw_OtherViewModel.Spaceinstallation && Mw_OtherViewModel.TLIcivilLoads.ReservedSpace == true)
                    {
                        var mwOtherLibrary = _dbContext.TLImwOtherLibrary.Where(x => x.Id == mwOther.mwOtherLibraryId).FirstOrDefault();
                        if (mwOther.CenterHigh == 0 || mwOther.CenterHigh == null)
                        {
                            mwOther.CenterHigh = mwOther.HBA + mwOtherLibrary.Length / 2;
                        }
                        if (Mw_OtherViewModel.TLIcivilLoads.ReservedSpace == true && (Mw_OtherViewModel.TLIcivilLoads.sideArmId == null || Mw_OtherViewModel.TLIcivilLoads.sideArmId == 0))
                        {
                            mwOther.EquivalentSpace = _unitOfWork.CivilWithLegsRepository.Checkspaceload(Mw_OtherViewModel.TLIcivilLoads.allCivilInstId, TableName, mwOther.Spaceinstallation, mwOther.CenterHigh, mwOther.mwOtherLibraryId, mwOther.HBA).Data;
                        }
                    }
                    _unitOfWork.Mw_OtherRepository.UpdateWithHistory(Helpers.LogFilterAttribute.UserId, OldMw_OtherViewModel, mwOther);
                    var allloads = _unitOfWork.AllLoadInstRepository.GetWhereFirst(x => x.mwOtherId == Mw_OtherViewModel.Id).Id;
                    var civilloads = _unitOfWork.CivilLoadsRepository.GetWhereFirst(x => x.allLoadInstId == allloads);
                    CivilLoads.InstallationDate = Mw_OtherViewModel.TLIcivilLoads.InstallationDate;
                    CivilLoads.ItemOnCivilStatus = Mw_OtherViewModel.TLIcivilLoads.ItemOnCivilStatus;
                    CivilLoads.ItemStatus = Mw_OtherViewModel.TLIcivilLoads.ItemStatus;
                    CivilLoads.ReservedSpace = Mw_OtherViewModel.TLIcivilLoads.ReservedSpace;
                    CivilLoads.sideArmId = Mw_OtherViewModel.TLIcivilLoads.sideArmId;
                    CivilLoads.allCivilInstId = Mw_OtherViewModel.TLIcivilLoads.allCivilInstId;
                    CivilLoads.legId = Mw_OtherViewModel.TLIcivilLoads.legId;
                    CivilLoads.Leg2Id = Mw_OtherViewModel.TLIcivilLoads.Leg2Id;

                    _unitOfWork.SaveChanges();
                    if (Mw_OtherViewModel.DynamicInstAttsValue.Count > 0)
                    {
                        _unitOfWork.DynamicAttInstValueRepository.UpdateDynamicValue(Mw_OtherViewModel.DynamicInstAttsValue, TableNameId, mwOther.Id);
                    }
                    await _unitOfWork.SaveChangesAsync();
                }
                return new Response<ObjectInstAtts>();
            }
            catch (Exception err)
            {
                return new Response<ObjectInstAtts>(true, null, null, err.Message, (int)ApiReturnCode.fail);
            }
        }
        #region Helper Methods For UpdateSideArm Function..
        public string CheckDependencyValidationEditVersion(object Input, string SiteCode, string MWName)
        {
            if (MWName.ToLower() == TablesNames.TLImwDish.ToString().ToLower())
            {
                string MainTableName = TablesNames.TLImwDish.ToString();
                EditMW_DishViewModel EditInstallationViewModel = _mapper.Map<EditMW_DishViewModel>(Input);
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
                        List<TLIrule> Rules = _unitOfWork.RowRuleRepository.GetIncludeWhere(x => x.RowId == RowId && x.Rule.OperationId != null, x => x.Rule, x => x.Rule.tablesNames,
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
            else if (MWName.ToLower() == TablesNames.TLImwBU.ToString().ToLower())
            {
                string MainTableName = TablesNames.TLImwBU.ToString();
                EditMW_BUViewModel EditInstallationViewModel = _mapper.Map<EditMW_BUViewModel>(Input);

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
                        List<TLIrule> Rules = _unitOfWork.RowRuleRepository.GetIncludeWhere(x => x.RowId == RowId && x.Rule.OperationId != null, x => x.Rule, x => x.Rule.tablesNames,
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
            else if (MWName.ToLower() == TablesNames.TLImwRFU.ToString().ToLower())
            {
                string MainTableName = TablesNames.TLImwRFU.ToString();
                EditMW_RFUViewModel EditInstallationViewModel = _mapper.Map<EditMW_RFUViewModel>(Input);

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
                        List<TLIrule> Rules = _unitOfWork.RowRuleRepository.GetIncludeWhere(x => x.RowId == RowId && x.Rule.OperationId != null, x => x.Rule, x => x.Rule.tablesNames,
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
            else if (MWName.ToLower() == TablesNames.TLImwODU.ToString().ToLower())
            {
                string MainTableName = TablesNames.TLImwODU.ToString();
                EditMW_ODUViewModel EditInstallationViewModel = _mapper.Map<EditMW_ODUViewModel>(Input);

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
                        List<TLIrule> Rules = _unitOfWork.RowRuleRepository.GetIncludeWhere(x => x.RowId == RowId && x.Rule.OperationId != null, x => x.Rule, x => x.Rule.tablesNames,
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
            else if (MWName.ToLower() == TablesNames.TLImwOther.ToString().ToLower())
            {
                string MainTableName = TablesNames.TLImwOther.ToString();
                EditMw_OtherViewModel EditInstallationViewModel = _mapper.Map<EditMw_OtherViewModel>(Input);

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
                        List<TLIrule> Rules = _unitOfWork.RowRuleRepository.GetIncludeWhere(x => x.RowId == RowId && x.Rule.OperationId != null, x => x.Rule, x => x.Rule.tablesNames,
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
        #endregion
        //Function take 2 parameters DynamicInstAttsValue, LoadInstId
        //Function get dynamic attribute by Id then update dynamic attribute
        //private void UpdateDynamicAttInstValue(List<BaseInstAttView> DynamicInstAttsValue, int LoadInstId)
        //{
        //    Parallel.ForEach(DynamicInstAttsValue, DynamicInstAttValue =>
        //   {
        //       var DAIV = _unitOfWork.DynamicAttInstValueRepository.GetWhereFirst( d => d.InventoryId == LoadInstId && d.DynamicAttId == DynamicInstAttValue.Id);
        //       if (DAIV.ValueBoolean != null)
        //       {
        //           DAIV.ValueBoolean = (bool)DynamicInstAttValue.Value;
        //           DAIV.Value = DynamicInstAttValue.Value.ToString();
        //       }
        //       else if (DAIV.ValueDateTime != null)
        //       {
        //           DAIV.ValueDateTime = (DateTime)DynamicInstAttValue.Value;
        //           DAIV.Value = DynamicInstAttValue.Value.ToString();
        //       }
        //       else if (DAIV.ValueDouble != null)
        //       {
        //           DAIV.ValueDouble = (double)DynamicInstAttValue.Value;
        //           DAIV.Value = DynamicInstAttValue.Value.ToString();
        //       }
        //       else if (!string.IsNullOrEmpty(DAIV.ValueString))
        //       {
        //           DAIV.ValueString = (string)DynamicInstAttValue.Value;
        //           DAIV.Value = DynamicInstAttValue.Value.ToString();
        //       }
        //       //DAIV.Value = DynamicInstAttValue.Value.ToString();
        //       _unitOfWork.DynamicAttInstValueRepository.Update(DAIV);
        //   });
        // }
        //Function take 3 parameters filters, WithFilterData, parameters
        //Function get all records depened on filters and parameters
        //Function check WithFilterData if true get RelatedTables 
        public Response<ReturnWithFilters<MW_ODUViewModel>> getMW_ODU(List<FilterObjectList> filters, bool WithFilterData, ParameterPagination parameters)
        {
            try
            {
                int count = 0;
                ReturnWithFilters<MW_ODUViewModel> MW_ODU = new ReturnWithFilters<MW_ODUViewModel>();
                MW_ODU.Model = _mapper.Map<List<MW_ODUViewModel>>(_unitOfWork.MW_ODURepository.GetAllIncludeMultiple(parameters, filters, out count, x => x.Mw_Dish, x => x.OduInstallationType, x => x.MwODULibrary).ToList());
                if (WithFilterData == true)
                {
                    // MW_ODU.filters = _unitOfWork.MW_ODURepository.GetRelatedTables();
                }
                else
                {
                    MW_ODU.filters = null;
                }
                return new Response<ReturnWithFilters<MW_ODUViewModel>>(true, MW_ODU, null, null, (int)ApiReturnCode.success, count);
            }
            catch (Exception err)
            {

                return new Response<ReturnWithFilters<MW_ODUViewModel>>(true, null, null, err.Message, (int)ApiReturnCode.fail);
            }
        }
        public Response<bool> DismantleLoads(string sitecode, int LoadId, string LoadName)
        {
            try
            {
                double? Freespace = 0;
                double? EquivalentSpace = 0;
                var allLoadInst = _dbContext.TLIallLoadInst.Where(x => x.mwBUId == LoadId || x.mwDishId == LoadId || x.mwODUId == LoadId || x.mwRFUId == LoadId || x.mwOtherId == LoadId || x.radioAntennaId == LoadId || x.radioRRUId == LoadId || x.radioOtherId == LoadId || x.powerId == LoadId || x.loadOtherId == LoadId)
                    .Include(x => x.mwBU).Include(x => x.mwDish).Include(x => x.mwODU)
                    .Include(x => x.mwRFU).Include(x => x.mwOther).Include(x => x.radioAntenna).Include(x => x.radioRRU).Include(x => x.radioOther).
                    Include(x => x.power).Include(x => x.loadOther).ToList();

                foreach (var item in allLoadInst)
                {
                    var civilload = _dbContext.TLIcivilLoads.Where(x => x.allLoadInstId == item.Id).Select(x => x.allCivilInstId).ToList();
                    foreach (var civilloadinst in civilload)
                    {
                        var allcivil = _dbContext.TLIallCivilInst.Where(x => x.Id == civilloadinst).Include(x => x.civilWithLegs).Include(x => x.civilWithoutLeg).Include(x => x.civilNonSteel).ToList();


                        foreach (var t in allcivil)
                        {
                            if (t.civilWithLegsId != null)
                            {
                                if (item.mwBUId != null && LoadName == TablesNames.TLImwBU.ToString())
                                {

                                    TLImwBU TLImwBU = item.mwBU;
                                    var PortCascadeId = _dbContext.TLImwBU.Where(x => x.Id == item.mwBUId).Select(x => x.PortCascadeId).FirstOrDefault();
                                    var PortCascade = _dbContext.TLImwPort.Where(x => x.Id == PortCascadeId).ToList();
                                    foreach (var Port in PortCascade)
                                    {
                                        var allload = _dbContext.TLIallLoadInst.Where(x => x.mwBUId == Port.MwBUId).Select(x => x.Id).FirstOrDefault();
                                        var Civilloads = _dbContext.TLIcivilLoads.Where(x => x.allLoadInstId == allload && x.allCivilInstId == t.Id && x.Dismantle == false).FirstOrDefault();
                                        if (Civilloads != null)
                                        {
                                            Civilloads.Dismantle = true;
                                            EquivalentSpace += 0;
                                            Port.MwBUId = 0;
                                            Port.MwBULibraryId = 0;
                                        }
                                    }
                                    var mwport = _dbContext.TLImwPort.Where(x => x.MwBUId == item.mwBUId).Select(x => x.Id).ToList();
                                    foreach (var port in mwport)
                                    {
                                        var mwrfu = _dbContext.TLImwRFU.Where(x => x.MwPortId == port).Select(x => x.Id).ToList();

                                        foreach (var rfu in mwrfu)
                                        {
                                            var allLoadRFU = _dbContext.TLIallLoadInst.Where(x => x.mwRFUId == rfu).Select(x => x.Id).ToList();
                                            foreach (var allLoad in allLoadRFU)
                                            {
                                                var MwRfu = _dbContext.TLIcivilLoads.Where(x => x.allLoadInstId == allLoad && x.Dismantle == false && x.allCivilInstId == t.Id).ToList();
                                                foreach (var MWRfU in MwRfu)
                                                {
                                                    MWRfU.Dismantle = true;
                                                    EquivalentSpace += 0;
                                                }

                                            }
                                        }

                                    }
                                    var civilLoads = _dbContext.TLIcivilLoads.Where(x => x.allLoadInstId == item.Id && x.Dismantle == false && x.SiteCode == sitecode && x.allCivilInstId == t.Id).ToList();
                                    foreach (var civilLoad in civilLoads)
                                    {
                                        civilLoad.Dismantle = true;
                                        if (civilLoad.sideArmId == null)
                                        {

                                            EquivalentSpace += 0;

                                        }
                                        else
                                        {
                                            EquivalentSpace += 0;
                                        }

                                    }
                                    TLIcivilWithLegs tLIcivilWithLegs = t.civilWithLegs;
                                    tLIcivilWithLegs.CurrentLoads = tLIcivilWithLegs.CurrentLoads - EquivalentSpace;
                                }
                                else if (item.mwDishId != null && LoadName == TablesNames.TLImwDish.ToString())
                                {
                                    TLImwDish TLImwDish = item.mwDish;
                                    var mwODU = _dbContext.TLImwODU.Where(x => x.Mw_DishId == item.mwDishId).Select(x => x.Id).ToList();
                                    foreach (var ODU in mwODU)
                                    {
                                        var allLoadinst = _dbContext.TLIallLoadInst.Where(x => x.mwODUId == ODU).Select(x => x.Id).ToList();
                                        foreach (var Load in allLoadinst)
                                        {
                                            var civil = _dbContext.TLIcivilLoads.Where(x => x.allLoadInstId == Load && x.Dismantle == false && x.allCivilInstId == t.Id).ToList();


                                            foreach (var civillload in civil)
                                            {
                                                civillload.Dismantle = true;
                                                if (civillload.sideArmId == null)
                                                {
                                                    EquivalentSpace += 0;
                                                }
                                                else
                                                {
                                                    EquivalentSpace += 0;
                                                }
                                            }
                                        }
                                    }
                                    var mwdish = _dbContext.TLIcivilLoads.Where(x => x.allLoadInstId == item.Id && x.ReservedSpace == true && x.Dismantle == false).ToList();
                                    foreach (var TlImwdish in mwdish)
                                    {
                                        TlImwdish.Dismantle = true;
                                        var Bu = _dbContext.TLImwBU.Where(x => x.MainDishId == item.mwDishId).ToList();
                                        foreach (var TLIBu in Bu)
                                        {
                                            TLIBu.MainDishId = null;
                                        }
                                        if (TlImwdish.sideArmId == null)
                                        {
                                            EquivalentSpace += TLImwDish.EquivalentSpace;
                                        }
                                        else
                                        {
                                            EquivalentSpace += 0;
                                        }

                                    }
                                    var TLImwdish = _dbContext.TLIcivilLoads.Where(x => x.allLoadInstId == item.Id && x.ReservedSpace == false && x.Dismantle == false).ToList();
                                    foreach (var TLmwdish in TLImwdish)
                                    {
                                        TLmwdish.Dismantle = true;
                                        var Bu = _dbContext.TLImwBU.Where(x => x.MainDishId == item.mwDishId).ToList();
                                        foreach (var TLIBu in Bu)
                                        {
                                            TLIBu.MainDishId = null;
                                        }
                                    }
                                    TLIcivilWithLegs tLIcivilWithLegs = t.civilWithLegs;
                                    tLIcivilWithLegs.CurrentLoads = tLIcivilWithLegs.CurrentLoads - EquivalentSpace;

                                }
                                else if (item.mwODUId != null && LoadName == TablesNames.TLImwODU.ToString())
                                {
                                    TLImwODU TLImwODU = item.mwODU;
                                    var MWODU = _dbContext.TLIcivilLoads.Where(x => x.allLoadInstId == item.Id && x.Dismantle == false && x.allCivilInstId == t.Id).ToList();
                                    foreach (var mwodu in MWODU)
                                    {
                                        mwodu.Dismantle = true;
                                        EquivalentSpace += 0;
                                    }
                                    TLIcivilWithLegs tLIcivilWithLegs = t.civilWithLegs;
                                    tLIcivilWithLegs.CurrentLoads = tLIcivilWithLegs.CurrentLoads - EquivalentSpace;

                                }
                                else if (item.mwRFUId != null && LoadName == TablesNames.TLImwRFU.ToString())
                                {
                                    TLImwRFU TLImwRFU = item.mwRFU;
                                    var MWRFU = _dbContext.TLIcivilLoads.Where(x => x.allLoadInstId == item.Id && x.Dismantle == false && x.allCivilInstId == t.Id).ToList();
                                    foreach (var mwrfu in MWRFU)
                                    {
                                        mwrfu.Dismantle = true;
                                        EquivalentSpace += 0;
                                    }
                                    TLIcivilWithLegs tLIcivilWithLegs = t.civilWithLegs;
                                    tLIcivilWithLegs.CurrentLoads = tLIcivilWithLegs.CurrentLoads - EquivalentSpace;
                                }
                                else if (item.mwOtherId != null && LoadName == TablesNames.TLImwOther.ToString())
                                {
                                    TLImwOther TLImwOther = item.mwOther;
                                    var MWOTHER = _dbContext.TLIcivilLoads.Where(x => x.allLoadInstId == item.Id && x.ReservedSpace == true && x.Dismantle == false && x.allCivilInstId == t.Id).ToList();
                                    foreach (var mwother in MWOTHER)
                                    {
                                        mwother.Dismantle = true;
                                        if (mwother.sideArmId == null)
                                        {
                                            EquivalentSpace += TLImwOther.EquivalentSpace;
                                        }
                                        else
                                        {
                                            EquivalentSpace += 0;
                                        }

                                    }
                                    var TLIMWOTHER = _dbContext.TLIcivilLoads.Where(x => x.allLoadInstId == item.Id && x.ReservedSpace == false && x.Dismantle == false && x.allCivilInstId == t.Id).ToList();
                                    foreach (var mwother in TLIMWOTHER)
                                    {
                                        mwother.Dismantle = true;
                                    }
                                    TLIcivilWithLegs tLIcivilWithLegs = t.civilWithLegs;
                                    tLIcivilWithLegs.CurrentLoads = tLIcivilWithLegs.CurrentLoads - EquivalentSpace;
                                }
                                else if (item.radioAntennaId != null && LoadName == TablesNames.TLIradioAntenna.ToString())
                                {
                                    TLIradioAntenna TLIradioAntenna = item.radioAntenna;
                                    var RADIOANTENNA = _dbContext.TLIcivilLoads.Where(x => x.allLoadInstId == item.Id && x.ReservedSpace == true && x.Dismantle == false && x.allCivilInstId == t.Id).ToList();
                                    foreach (var radioantenna in RADIOANTENNA)
                                    {
                                        radioantenna.Dismantle = true;
                                        var RadioRRu = _dbContext.TLIRadioRRU.Where(x => x.radioAntennaId == item.radioAntennaId).ToList();
                                        foreach (var radioRru in RadioRRu)
                                        {
                                            radioRru.radioAntennaId = null;
                                        }
                                        if (radioantenna.sideArmId == null)
                                        {
                                            EquivalentSpace += TLIradioAntenna.EquivalentSpace;
                                        }
                                        else
                                        {
                                            EquivalentSpace += 0;
                                        }

                                    }
                                    var TLIRADIOANTENNA = _dbContext.TLIcivilLoads.Where(x => x.allLoadInstId == item.Id && x.ReservedSpace == false && x.Dismantle == false && x.allCivilInstId == t.Id).ToList();
                                    foreach (var radioantenna in TLIRADIOANTENNA)
                                    {
                                        radioantenna.Dismantle = true;
                                        var RadioRRu = _dbContext.TLIRadioRRU.Where(x => x.radioAntennaId == item.radioAntennaId).ToList();
                                        foreach (var radioRru in RadioRRu)
                                        {
                                            radioRru.radioAntennaId = null;
                                        }
                                    }
                                    TLIcivilWithLegs tLIcivilWithLegs = t.civilWithLegs;
                                    tLIcivilWithLegs.CurrentLoads = tLIcivilWithLegs.CurrentLoads - EquivalentSpace;
                                }
                                else if (item.radioRRUId != null && LoadName == TablesNames.TLIradioRRU.ToString())
                                {

                                    TLIRadioRRU TLIRadioRRU = item.radioRRU;
                                    var RADIORRU = _dbContext.TLIcivilLoads.Where(x => x.allLoadInstId == item.Id && x.ReservedSpace == true && x.Dismantle == false && x.allCivilInstId == t.Id).ToList();
                                    foreach (var radiorru in RADIORRU)
                                    {
                                        radiorru.Dismantle = true;
                                        if (radiorru.sideArmId == null)
                                        {
                                            EquivalentSpace += TLIRadioRRU.EquivalentSpace;
                                        }
                                        else
                                        {
                                            EquivalentSpace += 0;
                                        }
                                    }
                                    var TLIRADIORRU = _dbContext.TLIcivilLoads.Where(x => x.allLoadInstId == item.Id && x.ReservedSpace == false && x.Dismantle == false && x.allCivilInstId == t.Id).ToList();
                                    foreach (var radio in TLIRADIORRU)
                                    {
                                        radio.Dismantle = true;
                                    }
                                    TLIcivilWithLegs tLIcivilWithLegs = t.civilWithLegs;
                                    tLIcivilWithLegs.CurrentLoads = tLIcivilWithLegs.CurrentLoads - EquivalentSpace;
                                }
                                else if (item.radioOtherId != null && LoadName == TablesNames.TLIradioOther.ToString())
                                {
                                    TLIradioOther TLIradioOther = item.radioOther;
                                    var RADIOOTHER = _dbContext.TLIcivilLoads.Where(x => x.allLoadInstId == item.Id && x.ReservedSpace == true && x.Dismantle == false && x.allCivilInstId == t.Id).ToList();
                                    foreach (var radioother in RADIOOTHER)
                                    {
                                        radioother.Dismantle = true;
                                        if (radioother.sideArmId == null)
                                        {
                                            EquivalentSpace += TLIradioOther.EquivalentSpace;
                                        }
                                        else
                                        {
                                            EquivalentSpace += 0;
                                        }
                                    }
                                    var TLIRADIOOTHER = _dbContext.TLIcivilLoads.Where(x => x.allLoadInstId == item.Id && x.ReservedSpace == false && x.Dismantle == false && x.allCivilInstId == t.Id).ToList();
                                    foreach (var radioother in TLIRADIOOTHER)
                                    {
                                        radioother.Dismantle = true;
                                    }
                                    TLIcivilWithLegs tLIcivilWithLegs = t.civilWithLegs;
                                    tLIcivilWithLegs.CurrentLoads = tLIcivilWithLegs.CurrentLoads - EquivalentSpace;
                                }
                                else if (item.powerId != null && LoadName == TablesNames.TLIpower.ToString())
                                {
                                    TLIpower TLIpower = item.power;
                                    var POWER = _dbContext.TLIcivilLoads.Where(x => x.allLoadInstId == item.Id && x.ReservedSpace == true && x.Dismantle == false && x.allCivilInstId == t.Id).ToList();
                                    foreach (var power in POWER)
                                    {
                                        power.Dismantle = true;
                                        if (power.sideArmId == null)
                                        {
                                            EquivalentSpace += TLIpower.EquivalentSpace;
                                        }
                                        else
                                        {
                                            EquivalentSpace += 0;
                                        }
                                    }
                                    var TLIPOWER = _dbContext.TLIcivilLoads.Where(x => x.allLoadInstId == item.Id && x.ReservedSpace == false && x.Dismantle == false && x.allCivilInstId == t.Id).ToList();
                                    foreach (var power in TLIPOWER)
                                    {
                                        power.Dismantle = true;
                                    }
                                    TLIcivilWithLegs tLIcivilWithLegs = t.civilWithLegs;
                                    tLIcivilWithLegs.CurrentLoads = tLIcivilWithLegs.CurrentLoads - EquivalentSpace;
                                }
                                else if (item.loadOtherId != null && LoadName == TablesNames.TLIloadOther.ToString())
                                {
                                    TLIloadOther tLIloadOther = item.loadOther;
                                    var LOADOTHER = _dbContext.TLIcivilLoads.Where(x => x.allLoadInstId == item.Id && x.ReservedSpace == true && x.Dismantle == false && x.allCivilInstId == t.Id).ToList();
                                    foreach (var loadother in LOADOTHER)
                                    {
                                        loadother.Dismantle = true;
                                        if (loadother.sideArmId == null)
                                        {
                                            EquivalentSpace += tLIloadOther.EquivalentSpace;
                                        }
                                        else
                                        {
                                            EquivalentSpace += 0;
                                        }
                                    }
                                    var TLILOADOTHER = _dbContext.TLIcivilLoads.Where(x => x.allLoadInstId == item.Id && x.ReservedSpace == false && x.Dismantle == false && x.allCivilInstId == t.Id).ToList();
                                    foreach (var loadother in TLILOADOTHER)
                                    {
                                        loadother.Dismantle = true;
                                    }
                                    TLIcivilWithLegs tLIcivilWithLegs = t.civilWithLegs;
                                    tLIcivilWithLegs.CurrentLoads = tLIcivilWithLegs.CurrentLoads - EquivalentSpace;
                                }
                                _dbContext.SaveChanges();
                            }
                            else if (t.civilWithoutLegId != null)
                            {

                                if (item.mwBUId != null && LoadName == TablesNames.TLImwBU.ToString())
                                {

                                    TLImwBU TLImwBU = item.mwBU;
                                    var PortCascadeId = _dbContext.TLImwBU.Where(x => x.Id == item.mwBUId).Select(x => x.PortCascadeId).FirstOrDefault();
                                    var PortCascade = _dbContext.TLImwPort.Where(x => x.Id == PortCascadeId).ToList();
                                    foreach (var Port in PortCascade)
                                    {
                                        var allload = _dbContext.TLIallLoadInst.Where(x => x.mwBUId == Port.MwBUId).Select(x => x.Id).FirstOrDefault();
                                        var Civilloads = _dbContext.TLIcivilLoads.Where(x => x.allLoadInstId == allload && x.allCivilInstId == t.Id && x.Dismantle == false).FirstOrDefault();
                                        if (Civilloads != null)
                                        {
                                            Civilloads.Dismantle = true;
                                            EquivalentSpace += 0;
                                            Port.MwBUId = 0;
                                            Port.MwBULibraryId = 0;
                                        }
                                    }
                                    var mwport = _dbContext.TLImwPort.Where(x => x.MwBUId == item.mwBUId).Select(x => x.Id).ToList();
                                    foreach (var port in mwport)
                                    {
                                        var mwrfu = _dbContext.TLImwRFU.Where(x => x.MwPortId == port).Select(x => x.Id).ToList();

                                        foreach (var rfu in mwrfu)
                                        {
                                            var allLoadRFU = _dbContext.TLIallLoadInst.Where(x => x.mwRFUId == rfu).Select(x => x.Id).ToList();
                                            foreach (var allLoad in allLoadRFU)
                                            {
                                                var MwRfu = _dbContext.TLIcivilLoads.Where(x => x.allLoadInstId == allLoad && x.Dismantle == false && x.allCivilInstId == t.Id).ToList();
                                                foreach (var MWRfU in MwRfu)
                                                {
                                                    MWRfU.Dismantle = true;
                                                    EquivalentSpace += 0;
                                                }

                                            }
                                        }

                                    }
                                    var civilLoads = _dbContext.TLIcivilLoads.Where(x => x.allLoadInstId == item.Id && x.Dismantle == false && x.SiteCode == sitecode && x.allCivilInstId == t.Id).ToList();
                                    foreach (var civilLoad in civilLoads)
                                    {
                                        civilLoad.Dismantle = true;
                                        if (civilLoad.sideArmId == null)
                                        {

                                            EquivalentSpace += 0;

                                        }
                                        else
                                        {
                                            EquivalentSpace += 0;
                                        }

                                    }
                                    TLIcivilWithoutLeg tLIcivilWithoutLeg = t.civilWithoutLeg;
                                    tLIcivilWithoutLeg.CurrentLoads = tLIcivilWithoutLeg.CurrentLoads - (float?)EquivalentSpace;
                                }
                                else if (item.mwDishId != null && LoadName == TablesNames.TLImwDish.ToString())
                                {
                                    TLImwDish TLImwDish = item.mwDish;
                                    var mwODU = _dbContext.TLImwODU.Where(x => x.Mw_DishId == item.mwDishId).Select(x => x.Id).ToList();
                                    foreach (var ODU in mwODU)
                                    {
                                        var allLoadinst = _dbContext.TLIallLoadInst.Where(x => x.mwODUId == ODU).Select(x => x.Id).ToList();
                                        foreach (var Load in allLoadinst)
                                        {
                                            var civil = _dbContext.TLIcivilLoads.Where(x => x.allLoadInstId == Load && x.Dismantle == false && x.allCivilInstId == t.Id).ToList();


                                            foreach (var civillload in civil)
                                            {
                                                civillload.Dismantle = true;
                                                if (civillload.sideArmId == null)
                                                {
                                                    EquivalentSpace += 0;
                                                }
                                                else
                                                {
                                                    EquivalentSpace += 0;
                                                }
                                            }
                                        }
                                    }
                                    var mwdish = _dbContext.TLIcivilLoads.Where(x => x.allLoadInstId == item.Id && x.ReservedSpace == true && x.Dismantle == false).ToList();
                                    foreach (var TLImwdish in mwdish)
                                    {
                                        TLImwdish.Dismantle = true;
                                        var Bu = _dbContext.TLImwBU.Where(x => x.MainDishId == item.mwDishId).ToList();
                                        foreach (var TLIBu in Bu)
                                        {
                                            TLIBu.MainDishId = null;
                                        }
                                        if (TLImwdish.sideArmId == null)
                                        {
                                            EquivalentSpace += TLImwDish.EquivalentSpace;
                                        }
                                        else
                                        {
                                            EquivalentSpace += 0;
                                        }

                                    }
                                    var tlimwdish = _dbContext.TLIcivilLoads.Where(x => x.allLoadInstId == item.Id && x.ReservedSpace == false && x.Dismantle == false).ToList();
                                    foreach (var MwDish in tlimwdish)
                                    {
                                        MwDish.Dismantle = true;
                                        var Bu = _dbContext.TLImwBU.Where(x => x.MainDishId == item.mwDishId).ToList();
                                        foreach (var TLIBu in Bu)
                                        {
                                            TLIBu.MainDishId = null;
                                        }
                                    }
                                    TLIcivilWithoutLeg tLIcivilWithoutLeg = t.civilWithoutLeg;
                                    tLIcivilWithoutLeg.CurrentLoads = tLIcivilWithoutLeg.CurrentLoads - (float?)EquivalentSpace;

                                }
                                else if (item.mwODUId != null && LoadName == TablesNames.TLImwODU.ToString())
                                {
                                    TLImwODU TLImwODU = item.mwODU;
                                    var MWODU = _dbContext.TLIcivilLoads.Where(x => x.allLoadInstId == item.Id && x.Dismantle == false && x.allCivilInstId == t.Id).ToList();
                                    foreach (var mwodu in MWODU)
                                    {
                                        mwodu.Dismantle = true;
                                        EquivalentSpace += 0;
                                    }
                                    TLIcivilWithoutLeg tLIcivilWithoutLeg = t.civilWithoutLeg;
                                    tLIcivilWithoutLeg.CurrentLoads = tLIcivilWithoutLeg.CurrentLoads - (float?)EquivalentSpace;

                                }
                                else if (item.mwRFUId != null && LoadName == TablesNames.TLImwRFU.ToString())
                                {
                                    TLImwRFU TLImwRFU = item.mwRFU;
                                    var MWRFU = _dbContext.TLIcivilLoads.Where(x => x.allLoadInstId == item.Id && x.Dismantle == false && x.allCivilInstId == t.Id).ToList();
                                    foreach (var mwrfu in MWRFU)
                                    {
                                        mwrfu.Dismantle = true;
                                        EquivalentSpace += 0;
                                    }
                                    TLIcivilWithoutLeg tLIcivilWithoutLeg = t.civilWithoutLeg;
                                    tLIcivilWithoutLeg.CurrentLoads = tLIcivilWithoutLeg.CurrentLoads - (float?)EquivalentSpace;
                                }
                                else if (item.mwOtherId != null && LoadName == TablesNames.TLImwOther.ToString())
                                {
                                    TLImwOther TLImwOther = item.mwOther;
                                    var MWOTHER = _dbContext.TLIcivilLoads.Where(x => x.allLoadInstId == item.Id && x.ReservedSpace == true && x.Dismantle == false && x.allCivilInstId == t.Id).ToList();
                                    foreach (var mwother in MWOTHER)
                                    {
                                        mwother.Dismantle = true;
                                        if (mwother.sideArmId == null)
                                        {
                                            EquivalentSpace += TLImwOther.EquivalentSpace;
                                        }
                                        else
                                        {
                                            EquivalentSpace += 0;
                                        }

                                    }
                                    var TLIMWOTHER = _dbContext.TLIcivilLoads.Where(x => x.allLoadInstId == item.Id && x.ReservedSpace == false && x.Dismantle == false && x.allCivilInstId == t.Id).ToList();
                                    foreach (var mwother in TLIMWOTHER)
                                    {
                                        mwother.Dismantle = true;
                                    }
                                    TLIcivilWithoutLeg tLIcivilWithoutLeg = t.civilWithoutLeg;
                                    tLIcivilWithoutLeg.CurrentLoads = tLIcivilWithoutLeg.CurrentLoads - (float?)EquivalentSpace;
                                }
                                else if (item.radioAntennaId != null && LoadName == TablesNames.TLIradioAntenna.ToString())
                                {
                                    TLIradioAntenna TLIradioAntenna = item.radioAntenna;
                                    var RADIOANTENNA = _dbContext.TLIcivilLoads.Where(x => x.allLoadInstId == item.Id && x.ReservedSpace == true && x.Dismantle == false && x.allCivilInstId == t.Id).ToList();
                                    foreach (var radioantenna in RADIOANTENNA)
                                    {
                                        radioantenna.Dismantle = true;
                                        var RadioRRu = _dbContext.TLIRadioRRU.Where(x => x.radioAntennaId == item.radioAntennaId).ToList();
                                        foreach (var TLIRadioRRu in RadioRRu)
                                        {
                                            TLIRadioRRu.radioAntennaId = null;
                                        }
                                        if (radioantenna.sideArmId == null)
                                        {
                                            EquivalentSpace += TLIradioAntenna.EquivalentSpace;
                                        }
                                        else
                                        {
                                            EquivalentSpace += 0;
                                        }

                                    }
                                    var TLIRADIOANTENNA = _dbContext.TLIcivilLoads.Where(x => x.allLoadInstId == item.Id && x.ReservedSpace == false && x.Dismantle == false && x.allCivilInstId == t.Id).ToList();
                                    foreach (var radioantenna in TLIRADIOANTENNA)
                                    {
                                        radioantenna.Dismantle = true;
                                        var RadioRRu = _dbContext.TLIRadioRRU.Where(x => x.radioAntennaId == item.radioAntennaId).ToList();
                                        foreach (var TLIRadioRRu in RadioRRu)
                                        {
                                            TLIRadioRRu.radioAntennaId = null;
                                        }
                                    }
                                    TLIcivilWithoutLeg tLIcivilWithoutLeg = t.civilWithoutLeg;
                                    tLIcivilWithoutLeg.CurrentLoads = tLIcivilWithoutLeg.CurrentLoads - (float?)EquivalentSpace;
                                }
                                else if (item.radioRRUId != null && LoadName == TablesNames.TLIradioRRU.ToString())
                                {

                                    TLIRadioRRU TLIRadioRRU = item.radioRRU;
                                    var RADIORRU = _dbContext.TLIcivilLoads.Where(x => x.allLoadInstId == item.Id && x.ReservedSpace == true && x.Dismantle == false && x.allCivilInstId == t.Id).ToList();
                                    foreach (var radiorru in RADIORRU)
                                    {
                                        radiorru.Dismantle = true;
                                        if (radiorru.sideArmId == null)
                                        {
                                            EquivalentSpace += TLIRadioRRU.EquivalentSpace;
                                        }
                                        else
                                        {
                                            EquivalentSpace += 0;
                                        }
                                    }
                                    var TLIRADIORRU = _dbContext.TLIcivilLoads.Where(x => x.allLoadInstId == item.Id && x.ReservedSpace == false && x.Dismantle == false && x.allCivilInstId == t.Id).ToList();
                                    foreach (var radiorru in TLIRADIORRU)
                                    {
                                        radiorru.Dismantle = true;
                                    }
                                    TLIcivilWithoutLeg tLIcivilWithoutLeg = t.civilWithoutLeg;
                                    tLIcivilWithoutLeg.CurrentLoads = tLIcivilWithoutLeg.CurrentLoads - (float?)EquivalentSpace;
                                }
                                else if (item.radioOtherId != null && LoadName == TablesNames.TLIradioOther.ToString())
                                {
                                    TLIradioOther TLIradioOther = item.radioOther;
                                    var RADIOOTHER = _dbContext.TLIcivilLoads.Where(x => x.allLoadInstId == item.Id && x.ReservedSpace == true && x.Dismantle == false && x.allCivilInstId == t.Id).ToList();
                                    foreach (var radioother in RADIOOTHER)
                                    {
                                        radioother.Dismantle = true;
                                        if (radioother.sideArmId == null)
                                        {
                                            EquivalentSpace += TLIradioOther.EquivalentSpace;
                                        }
                                        else
                                        {
                                            EquivalentSpace += 0;
                                        }
                                    }
                                    var TLIRADIOOTHER = _dbContext.TLIcivilLoads.Where(x => x.allLoadInstId == item.Id && x.ReservedSpace == false && x.Dismantle == false && x.allCivilInstId == t.Id).ToList();
                                    foreach (var radioother in TLIRADIOOTHER)
                                    {
                                        radioother.Dismantle = true;
                                    }
                                    TLIcivilWithoutLeg tLIcivilWithoutLeg = t.civilWithoutLeg;
                                    tLIcivilWithoutLeg.CurrentLoads = tLIcivilWithoutLeg.CurrentLoads - (float?)EquivalentSpace;
                                }
                                else if (item.powerId != null && LoadName == TablesNames.TLIpower.ToString())
                                {
                                    TLIpower TLIpower = item.power;
                                    var POWER = _dbContext.TLIcivilLoads.Where(x => x.allLoadInstId == item.Id && x.ReservedSpace == true && x.Dismantle == false && x.allCivilInstId == t.Id).ToList();
                                    foreach (var power in POWER)
                                    {
                                        power.Dismantle = true;
                                        if (power.sideArmId == null)
                                        {
                                            EquivalentSpace += TLIpower.EquivalentSpace;
                                        }
                                        else
                                        {
                                            EquivalentSpace += 0;
                                        }
                                    }
                                    var TLIPOWER = _dbContext.TLIcivilLoads.Where(x => x.allLoadInstId == item.Id && x.ReservedSpace == false && x.Dismantle == false && x.allCivilInstId == t.Id).ToList();
                                    foreach (var power in TLIPOWER)
                                    {
                                        power.Dismantle = true;
                                    }
                                    TLIcivilWithoutLeg tLIcivilWithoutLeg = t.civilWithoutLeg;
                                    tLIcivilWithoutLeg.CurrentLoads = tLIcivilWithoutLeg.CurrentLoads - (float?)EquivalentSpace;
                                }
                                else if (item.loadOtherId != null && LoadName == TablesNames.TLIloadOther.ToString())
                                {
                                    TLIloadOther tLIloadOther = item.loadOther;
                                    var LOADOTHER = _dbContext.TLIcivilLoads.Where(x => x.allLoadInstId == item.Id && x.ReservedSpace == true && x.Dismantle == false && x.allCivilInstId == t.Id).ToList();
                                    foreach (var loadother in LOADOTHER)
                                    {
                                        loadother.Dismantle = true;
                                        if (loadother.sideArmId == null)
                                        {
                                            EquivalentSpace += tLIloadOther.EquivalentSpace;
                                        }
                                        else
                                        {
                                            EquivalentSpace += 0;
                                        }
                                    }
                                    var TLILOADOTHER = _dbContext.TLIcivilLoads.Where(x => x.allLoadInstId == item.Id && x.ReservedSpace == false && x.Dismantle == false && x.allCivilInstId == t.Id).ToList();
                                    foreach (var loadother in TLILOADOTHER)
                                    {
                                        loadother.Dismantle = true;
                                    }
                                    TLIcivilWithoutLeg tLIcivilWithoutLeg = t.civilWithoutLeg;
                                    tLIcivilWithoutLeg.CurrentLoads = tLIcivilWithoutLeg.CurrentLoads - (float?)EquivalentSpace;
                                }
                                _dbContext.SaveChanges();
                            }
                            else if (t.civilNonSteelId != null)
                            {
                                if (item.mwBUId != null && LoadName == TablesNames.TLImwBU.ToString())
                                {

                                    TLImwBU TLImwBU = item.mwBU;
                                    var PortCascadeId = _dbContext.TLImwBU.Where(x => x.Id == item.mwBUId).Select(x => x.PortCascadeId).FirstOrDefault();
                                    var PortCascade = _dbContext.TLImwPort.Where(x => x.Id == PortCascadeId).ToList();
                                    foreach (var Port in PortCascade)
                                    {
                                        var allload = _dbContext.TLIallLoadInst.Where(x => x.mwBUId == Port.MwBUId).Select(x => x.Id).FirstOrDefault();
                                        var Civilloads = _dbContext.TLIcivilLoads.Where(x => x.allLoadInstId == allload && x.allCivilInstId == t.Id && x.Dismantle == false).FirstOrDefault();
                                        if (Civilloads != null)
                                        {
                                            Civilloads.Dismantle = true;
                                            EquivalentSpace += 0;
                                            Port.MwBUId = 0;
                                            Port.MwBULibraryId = 0;
                                        }
                                    }
                                    var mwport = _dbContext.TLImwPort.Where(x => x.MwBUId == item.mwBUId).Select(x => x.Id).ToList();
                                    foreach (var port in mwport)
                                    {
                                        var mwrfu = _dbContext.TLImwRFU.Where(x => x.MwPortId == port).Select(x => x.Id).ToList();

                                        foreach (var rfu in mwrfu)
                                        {
                                            var allLoadRFU = _dbContext.TLIallLoadInst.Where(x => x.mwRFUId == rfu).Select(x => x.Id).ToList();
                                            foreach (var allLoad in allLoadRFU)
                                            {
                                                var MwRfu = _dbContext.TLIcivilLoads.Where(x => x.allLoadInstId == allLoad && x.Dismantle == false && x.allCivilInstId == t.Id).ToList();
                                                foreach (var MWRfU in MwRfu)
                                                {
                                                    MWRfU.Dismantle = true;
                                                    EquivalentSpace += 0;
                                                }

                                            }
                                        }

                                    }
                                    var civilLoads = _dbContext.TLIcivilLoads.Where(x => x.allLoadInstId == item.Id && x.Dismantle == false && x.SiteCode == sitecode && x.allCivilInstId == t.Id).ToList();
                                    foreach (var civilLoad in civilLoads)
                                    {
                                        civilLoad.Dismantle = true;
                                        EquivalentSpace += 0;

                                    }
                                    TLIcivilNonSteel tLIcivilNonSteel = t.civilNonSteel;
                                    tLIcivilNonSteel.CurrentLoads = tLIcivilNonSteel.CurrentLoads - (double)EquivalentSpace;
                                }
                                else if (item.mwDishId != null && LoadName == TablesNames.TLImwDish.ToString())
                                {
                                    TLImwDish TLImwDish = item.mwDish;
                                    var mwODU = _dbContext.TLImwODU.Where(x => x.Mw_DishId == item.mwDishId).Select(x => x.Id).ToList();
                                    foreach (var ODU in mwODU)
                                    {
                                        var allLoadinst = _dbContext.TLIallLoadInst.Where(x => x.mwODUId == ODU).Select(x => x.Id).ToList();
                                        foreach (var Load in allLoadinst)
                                        {
                                            var civil = _dbContext.TLIcivilLoads.Where(x => x.allLoadInstId == Load && x.Dismantle == false && x.allCivilInstId == t.Id).ToList();


                                            foreach (var civillload in civil)
                                            {
                                                civillload.Dismantle = true;
                                                EquivalentSpace += 0;
                                            }
                                        }
                                    }
                                    var mwdish = _dbContext.TLIcivilLoads.Where(x => x.allLoadInstId == item.Id && x.Dismantle == false).ToList();
                                    foreach (var TLImwdish in mwdish)
                                    {
                                        TLImwdish.Dismantle = true;
                                        var Bu = _dbContext.TLImwBU.Where(x => x.MainDishId == item.mwDishId).ToList();
                                        foreach (var TLIBu in Bu)
                                        {
                                            TLIBu.MainDishId = null;
                                        }
                                        EquivalentSpace += 0;

                                    }
                                    TLIcivilNonSteel tLIcivilNonSteel = t.civilNonSteel;
                                    tLIcivilNonSteel.CurrentLoads = tLIcivilNonSteel.CurrentLoads - (double)EquivalentSpace;

                                }
                                else if (item.mwODUId != null && LoadName == TablesNames.TLImwODU.ToString())
                                {
                                    TLImwODU TLImwODU = item.mwODU;
                                    var MWODU = _dbContext.TLIcivilLoads.Where(x => x.allLoadInstId == item.Id && x.Dismantle == false && x.allCivilInstId == t.Id).ToList();
                                    foreach (var mwodu in MWODU)
                                    {
                                        mwodu.Dismantle = true;
                                        EquivalentSpace += 0;
                                    }
                                    TLIcivilNonSteel tLIcivilNonSteel = t.civilNonSteel;
                                    tLIcivilNonSteel.CurrentLoads = tLIcivilNonSteel.CurrentLoads - (double)EquivalentSpace;


                                }
                                else if (item.mwRFUId != null && LoadName == TablesNames.TLImwRFU.ToString())
                                {
                                    TLImwRFU TLImwRFU = item.mwRFU;
                                    var MWRFU = _dbContext.TLIcivilLoads.Where(x => x.allLoadInstId == item.Id && x.Dismantle == false && x.allCivilInstId == t.Id).ToList();
                                    foreach (var mwrfu in MWRFU)
                                    {
                                        mwrfu.Dismantle = true;
                                        EquivalentSpace += 0;
                                    }
                                    TLIcivilNonSteel tLIcivilNonSteel = t.civilNonSteel;
                                    tLIcivilNonSteel.CurrentLoads = tLIcivilNonSteel.CurrentLoads - (double)EquivalentSpace;

                                }
                                else if (item.mwOtherId != null && LoadName == TablesNames.TLImwOther.ToString())
                                {
                                    TLImwOther TLImwOther = item.mwOther;
                                    var MWOTHER = _dbContext.TLIcivilLoads.Where(x => x.allLoadInstId == item.Id && x.Dismantle == false && x.allCivilInstId == t.Id).ToList();
                                    foreach (var mwother in MWOTHER)
                                    {
                                        mwother.Dismantle = true;
                                        EquivalentSpace += 0;
                                    }
                                    TLIcivilNonSteel tLIcivilNonSteel = t.civilNonSteel;
                                    tLIcivilNonSteel.CurrentLoads = tLIcivilNonSteel.CurrentLoads - (double)EquivalentSpace;

                                }
                                else if (item.radioAntennaId != null && LoadName == TablesNames.TLIradioAntenna.ToString())
                                {
                                    TLIradioAntenna TLIradioAntenna = item.radioAntenna;
                                    var RADIOANTENNA = _dbContext.TLIcivilLoads.Where(x => x.allLoadInstId == item.Id && x.Dismantle == false && x.allCivilInstId == t.Id).ToList();
                                    foreach (var radioantenna in RADIOANTENNA)
                                    {
                                        radioantenna.Dismantle = true;
                                        var RadioRRu = _dbContext.TLIRadioRRU.Where(x => x.radioAntennaId == item.radioAntennaId).ToList();
                                        foreach (var radioRru in RadioRRu)
                                        {
                                            radioRru.radioAntennaId = null;
                                        }
                                        EquivalentSpace += 0;

                                    }
                                    TLIcivilNonSteel tLIcivilNonSteel = t.civilNonSteel;
                                    tLIcivilNonSteel.CurrentLoads = tLIcivilNonSteel.CurrentLoads - (double)EquivalentSpace;

                                }
                                else if (item.radioRRUId != null && LoadName == TablesNames.TLIradioRRU.ToString())
                                {

                                    TLIRadioRRU TLIRadioRRU = item.radioRRU;
                                    var RADIORRU = _dbContext.TLIcivilLoads.Where(x => x.allLoadInstId == item.Id && x.Dismantle == false && x.allCivilInstId == t.Id).ToList();
                                    foreach (var radiorru in RADIORRU)
                                    {
                                        radiorru.Dismantle = true;
                                        EquivalentSpace += 0;

                                    }
                                    TLIcivilNonSteel tLIcivilNonSteel = t.civilNonSteel;
                                    tLIcivilNonSteel.CurrentLoads = tLIcivilNonSteel.CurrentLoads - (double)EquivalentSpace;

                                }
                                else if (item.radioOtherId != null && LoadName == TablesNames.TLIradioOther.ToString())
                                {
                                    TLIradioOther TLIradioOther = item.radioOther;
                                    var RADIOOTHER = _dbContext.TLIcivilLoads.Where(x => x.allLoadInstId == item.Id && x.Dismantle == false && x.allCivilInstId == t.Id).ToList();
                                    foreach (var radioother in RADIOOTHER)
                                    {
                                        radioother.Dismantle = true;
                                        EquivalentSpace += 0;

                                    }
                                    TLIcivilNonSteel tLIcivilNonSteel = t.civilNonSteel;
                                    tLIcivilNonSteel.CurrentLoads = tLIcivilNonSteel.CurrentLoads - (double)EquivalentSpace;

                                }
                                else if (item.powerId != null && LoadName == TablesNames.TLIpower.ToString())
                                {
                                    TLIpower TLIpower = item.power;
                                    var POWER = _dbContext.TLIcivilLoads.Where(x => x.allLoadInstId == item.Id && x.Dismantle == false && x.allCivilInstId == t.Id).ToList();
                                    foreach (var power in POWER)
                                    {
                                        power.Dismantle = true;
                                        EquivalentSpace += 0;

                                    }
                                    TLIcivilNonSteel tLIcivilNonSteel = t.civilNonSteel;
                                    tLIcivilNonSteel.CurrentLoads = tLIcivilNonSteel.CurrentLoads - (double)EquivalentSpace;

                                }
                                else if (item.loadOtherId != null && LoadName == TablesNames.TLIloadOther.ToString())
                                {
                                    TLIloadOther tLIloadOther = item.loadOther;
                                    var LOADOTHER = _dbContext.TLIcivilLoads.Where(x => x.allLoadInstId == item.Id && x.Dismantle == false && x.allCivilInstId == t.Id).ToList();
                                    foreach (var loadother in LOADOTHER)
                                    {
                                        loadother.Dismantle = true;
                                        EquivalentSpace += 0;
                                    }
                                    TLIcivilNonSteel tLIcivilNonSteel = t.civilNonSteel;
                                    tLIcivilNonSteel.CurrentLoads = tLIcivilNonSteel.CurrentLoads - (double)EquivalentSpace;

                                }
                                _dbContext.SaveChanges();

                            }
                        }
                    }


                    _dbContext.SaveChanges();
                }
                return new Response<bool>(true, true, null, null, (int)Helpers.Constants.ApiReturnCode.success);
            }
            catch (Exception er)
            {

                return new Response<bool>(false, false, null, er.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
        }
        //Function take 3 parameters filters, WithFilterData, parameters
        //Function get all records depened on filters and parameters
        //Function check WithFilterData if true get RelatedTables 
        public Response<ReturnWithFilters<MW_BUViewModel>> getMW_BU(List<FilterObjectList> filters, bool WithFilterData, ParameterPagination parameters)
        {
            try
            {
                int count = 0;
                ReturnWithFilters<MW_BUViewModel> MW_BU = new ReturnWithFilters<MW_BUViewModel>();
                MW_BU.Model = _mapper.Map<List<MW_BUViewModel>>(_unitOfWork.MW_BURepository.GetAllIncludeMultiple(parameters, filters, out count, x => x.MwBULibrary, x => x.Owner, x => x.MainDish).ToList());
                if (WithFilterData == true)
                {
                    // MW_BU.filters = _unitOfWork.MW_BURepository.GetRelatedTables();
                }
                else
                {
                    MW_BU.filters = null;
                }
                return new Response<ReturnWithFilters<MW_BUViewModel>>(true, MW_BU, null, null, (int)ApiReturnCode.success, count);
            }
            catch (Exception err)
            {

                return new Response<ReturnWithFilters<MW_BUViewModel>>(true, null, null, err.Message, (int)ApiReturnCode.fail);
            }
        }
        //Function take 3 parameters filters, WithFilterData, parameters
        //Function get all records depened on filters and parameters
        //Function check WithFilterData if true get RelatedTables 
        public Response<ReturnWithFilters<MW_DishViewModel>> getMW_Dish(List<FilterObjectList> filters, bool WithFilterData, ParameterPagination parameters)
        {
            try
            {
                int count = 0;
                ReturnWithFilters<MW_DishViewModel> MW_Dish = new ReturnWithFilters<MW_DishViewModel>();
                MW_Dish.Model = _mapper.Map<List<MW_DishViewModel>>(_unitOfWork.MW_DishRepository.GetAllIncludeMultiple(parameters, filters, out count, x => x.RepeaterType, x => x.PolarityOnLocation, x => x.ItemConnectTo, x => x.MwDishLibrary, x => x.InstallationPlace).ToList());
                if (WithFilterData == true)
                {
                    MW_Dish.filters = _unitOfWork.MW_DishRepository.GetRelatedTables();
                }
                else
                {
                    MW_Dish.filters = null;
                }
                return new Response<ReturnWithFilters<MW_DishViewModel>>(true, MW_Dish, null, null, (int)ApiReturnCode.success, count);
            }
            catch (Exception err)
            {

                return new Response<ReturnWithFilters<MW_DishViewModel>>(true, null, null, err.Message, (int)ApiReturnCode.fail);
            }
        }
        //Function take 3 parameters filters, WithFilterData, parameters
        //Function get all records depened on filters and parameters
        //Function check WithFilterData if true get RelatedTables 
        public Response<ReturnWithFilters<MW_RFUViewModel>> getMW_RFU(List<FilterObjectList> filters, bool WithFilterData, ParameterPagination parameters)
        {
            try
            {
                int count = 0;
                ReturnWithFilters<MW_RFUViewModel> MW_RFU = new ReturnWithFilters<MW_RFUViewModel>();
                MW_RFU.Model = _mapper.Map<List<MW_RFUViewModel>>(_unitOfWork.MW_RFURepository.GetAllIncludeMultiple(parameters, filters, out count, x => x.MwRFULibrary, x => x.MwPort).ToList());
                if (WithFilterData == true)
                {
                    MW_RFU.filters = _unitOfWork.MW_RFURepository.GetRelatedTables();
                }
                else
                {
                    MW_RFU.filters = null;
                }
                return new Response<ReturnWithFilters<MW_RFUViewModel>>(true, MW_RFU, null, null, (int)ApiReturnCode.success, count);
            }
            catch (Exception err)
            {

                return new Response<ReturnWithFilters<MW_RFUViewModel>>(true, null, null, err.Message, (int)ApiReturnCode.fail);
            }
        }
        //Function take 2 parameters MWInsId, TableName
        //First get table name Entity by TableName
        //Second specify the table i deal with
        //Get the record by Id
        //Get ativated attributes with values 
        //Get dynamic attributes
        //get related tables
        public Response<ObjectInstAttsForSideArm> GetById(int MWInsId, string TableName)
        {
            try
            {
                TLItablesNames TableNameEntity = _unitOfWork.TablesNamesRepository.GetWhereFirst(c => c.TableName == TableName);
                ObjectInstAttsForSideArm objectInst = new ObjectInstAttsForSideArm();
                TLIallLoadInst AllLoadInst = new TLIallLoadInst();
                TLIcivilLoads CivilLoads = new TLIcivilLoads();
                List<BaseAttView> LoadInstAttributes = new List<BaseAttView>();

                TLIallCivilInst AllCivilInst = new TLIallCivilInst();
                List<BaseInstAttView> MWInstallationInfo = new List<BaseInstAttView>();

                if (LoadSubType.TLImwBU.ToString() == TableName)
                {
                    TLImwBU mw_BU = _unitOfWork.MW_BURepository
                        .GetIncludeWhereFirst(x => x.Id == MWInsId, x => x.baseBU,
                            x => x.Owner, x => x.InstallationPlace, x => x.MwBULibrary,
                            x => x.MainDish);

                    MW_BULibraryViewModel MwBuLibrary = _mapper.Map<MW_BULibraryViewModel>(_unitOfWork.MW_BULibraryRepository
                       .GetIncludeWhereFirst(x => x.Id == mw_BU.MwBULibraryId, x => x.diversityType));

                    List<BaseAttView> LibraryAttributes = _unitOfWork.AttributeActivatedRepository
                       .GetAttributeActivated(TablesNames.TLImwBULibrary.ToString(), MwBuLibrary, null).ToList();

                    foreach (BaseAttView LibraryAttribute in LibraryAttributes)
                    {
                        if (LibraryAttribute.DataType.ToLower() == "list")
                        {
                            LibraryAttribute.Value = MwBuLibrary.GetType().GetProperties()
                                .FirstOrDefault(x => x.Name.ToLower() == LibraryAttribute.Label.ToLower()).GetValue(MwBuLibrary);
                        }
                    }
                    List<BaseAttView> LogisticalAttributes = _mapper.Map<List<BaseAttView>>(_unitOfWork.LogistcalRepository
                        .GetLogistical(TablePartName.MW.ToString(), TablesNames.TLImwBULibrary.ToString(), MwBuLibrary.Id).ToList());

                    LibraryAttributes.AddRange(LogisticalAttributes);

                    objectInst.LibraryActivatedAttributes = LibraryAttributes;

                    List<BaseInstAttView> ListAttributesActivated = _unitOfWork.AttributeActivatedRepository
                        .GetInstAttributeActivated(LoadSubType.TLImwBU.ToString(), mw_BU,
                            "InstallationPlaceId").ToList();

                    BaseInstAttView NameAttribute = ListAttributesActivated.FirstOrDefault(x => x.Key.ToLower() == "Name".ToLower());
                    if (NameAttribute != null)
                    {
                        BaseInstAttView Swap = ListAttributesActivated[0];
                        ListAttributesActivated[ListAttributesActivated.IndexOf(NameAttribute)] = Swap;
                        ListAttributesActivated[0] = NameAttribute;
                    }
                    TLIcivilLoads CivilLoad = _unitOfWork.CivilLoadsRepository.GetIncludeWhereFirst(x => !x.Dismantle && (x.allLoadInstId != null ?
                        x.allLoadInst.mwBUId == MWInsId : false), x => x.allLoadInst);
                    foreach (BaseInstAttView FKitem in ListAttributesActivated)
                    {
                        if (FKitem.Desc.ToLower() == "tlibasebu")
                        {
                            if (mw_BU.baseBU == null)
                                FKitem.Value = _unitOfWork.BaseBURepository.GetWhereFirst(x => x.Id == 0).Name;

                            else
                                FKitem.Value = _unitOfWork.BaseBURepository.GetWhereFirst(x => x.Id == mw_BU.baseBU.Id && !x.Deleted && !x.Disable).Name;
                        }
                        else if (FKitem.Desc.ToLower() == "tliowner")
                        {
                            if (mw_BU.Owner == null)
                                FKitem.Value = _unitOfWork.OwnerRepository.GetWhereFirst(x => x.Id == 0).OwnerName;

                            else
                                FKitem.Value = _unitOfWork.OwnerRepository.GetWhereFirst(x => x.Id == mw_BU.Owner.Id && !x.Deleted && x.Disable == false).OwnerName;
                        }
                        else if (FKitem.Desc.ToLower() == "tlimwdish")
                        {
                            if (mw_BU.MainDish == null)
                                FKitem.Value = "NA";

                            else
                            {
                                FKitem.Value = _unitOfWork.MW_DishRepository.GetWhereFirst(x => x.Id == mw_BU.MainDish.Id).DishName;
                            }

                        }
                        else if (FKitem.Desc.ToLower() == "tlimwbulibrary")
                        {
                            if (mw_BU.MwBULibrary == null)
                                FKitem.Value = _unitOfWork.MW_BULibraryRepository.GetWhereFirst(x => x.Id == 0).Model;

                            else
                                FKitem.Value = _unitOfWork.MW_BULibraryRepository.GetWhereFirst(x => x.Id == mw_BU.MwBULibrary.Id && !x.Deleted && x.Active == true).Model;
                        }

                        else if (FKitem.Desc.ToLower() == "tlimwport")
                        {
                            if (mw_BU.PortCascadeId == null || mw_BU.PortCascadeId == 0)
                                FKitem.Value = "NA";

                            else
                            {
                                FKitem.Value = _unitOfWork.MW_PortRepository.GetWhereFirst(x => x.Id == mw_BU.PortCascadeId).Port_Name;
                                FKitem.Id = mw_BU.PortCascadeId;

                            }
                        }
                    }
                    objectInst.AttributesActivated = ListAttributesActivated;

                    objectInst.DynamicAtts = _unitOfWork.DynamicAttInstValueRepository
                        .GetDynamicInstAtts(TableNameEntity.Id, MWInsId, null);

                    AllLoadInst = _unitOfWork.AllLoadInstRepository.GetWhereFirst(x => x.mwBUId == MWInsId);

                    CivilLoads = _unitOfWork.CivilLoadsRepository
                        .GetIncludeWhereFirst(x => x.allLoadInstId == AllLoadInst.Id, x => x.sideArm, x => x.site, x => x.leg, x => x.allCivilInst,
                            x => x.allLoadInst.loadOther, x => x.allLoadInst.mwBU, x => x.allLoadInst.mwDish, x => x.allLoadInst.mwODU, x => x.allLoadInst.mwOther,
                            x => x.allLoadInst.mwRFU, x => x.allLoadInst.power, x => x.allLoadInst.radioAntenna, x => x.allLoadInst.radioOther, x => x.allLoadInst.radioRRU,
                            x => x.allCivilInst.civilWithLegs, x => x.allCivilInst.civilWithoutLeg, x => x.allCivilInst.civilNonSteel, x => x.civilSteelSupportCategory);

                    LoadInstAttributes = _unitOfWork.AttributeActivatedRepository
                        .GetAttributeActivated(TablesNames.TLIcivilLoads.ToString(), CivilLoads, null, "allLoadInstId",
                            "Dismantle", "SiteCode", "civilSteelSupportCategoryId", "legId", "Leg2Id",
                                "sideArmId", "allCivilInstId").ToList();

                    List<KeyValuePair<string, List<DropDownListFilters>>> CivilLoadsRelatedTables = _unitOfWork.CivilLoadsRepository
                        .GetRelatedTables(CivilLoads.SiteCode);

                    if (CivilLoads != null)
                    {
                        TLImwPort? CascadedBu_ForRelatedTables = _unitOfWork.MW_PortRepository
                            .GetIncludeWhereFirst(x => x.Id == mw_BU.PortCascadeId, x => x.MwBU);

                        List<KeyValuePair<string, List<DropDownListFilters>>> mwbuRelatedTables = new List<KeyValuePair<string, List<DropDownListFilters>>>();
                        if (CascadedBu_ForRelatedTables != null)
                            mwbuRelatedTables = _unitOfWork.MW_BURepository
                                .GetRelatedTablesForEdit(CivilLoads.SiteCode, CascadedBu_ForRelatedTables.MwBUId);

                        else
                            mwbuRelatedTables = _unitOfWork.MW_BURepository
                                .GetRelatedTablesForEdit(CivilLoads.SiteCode, null);

                        mwbuRelatedTables.AddRange(CivilLoadsRelatedTables);

                        if (CivilLoads.allCivilInst.civilWithLegsId != null)
                        {
                            List<TLIleg> LegsForCivilWithLegLibrary = _unitOfWork.LegRepository
                                .GetWhere(x => x.CivilWithLegInstId == CivilLoads.allCivilInst.civilWithLegsId).ToList();

                            List<DropDownListFilters> LegIds = _mapper.Map<List<DropDownListFilters>>(LegsForCivilWithLegLibrary);

                            mwbuRelatedTables.Add(new KeyValuePair<string, List<DropDownListFilters>>("Leg1Id", LegIds));

                            List<TLIleg> Legs2ForCivilWithLegLibrary = LegsForCivilWithLegLibrary.Except(LegsForCivilWithLegLibrary
                                .Where(x => x.Id == CivilLoads.legId)).ToList();

                            List<DropDownListFilters> Leg2Ids = _mapper.Map<List<DropDownListFilters>>(Legs2ForCivilWithLegLibrary);

                            mwbuRelatedTables.Add(new KeyValuePair<string, List<DropDownListFilters>>("Leg2Id", Leg2Ids));
                        }

                        objectInst.RelatedTables = mwbuRelatedTables;

                        AllCivilInst = _unitOfWork.CivilLoadsRepository
                            .GetIncludeWhereFirst(x => (x.allLoadInstId != null ? (x.allLoadInst.mwBUId != null ?
                                x.allLoadInst.mwBUId.Value == MWInsId : false) : false) && !x.Dismantle, x => x.allCivilInst, x => x.allLoadInst).allCivilInst;

                        if (AllCivilInst.civilWithLegsId != null)
                        {
                            MWInstallationInfo.Add(new BaseInstAttView
                            {
                                AutoFill = false,
                                DataType = "List",
                                DataTypeId = null,
                                Desc = "Select civil support type",
                                enable = true,
                                Id = -1,
                                Key = "Select civil support type",
                                Label = "Select civil support type",
                                Manage = false,
                                Required = false,
                                Value = "Civil with legs"
                            });
                            MWInstallationInfo.Add(new BaseInstAttView
                            {
                                AutoFill = false,
                                DataType = "List",
                                DataTypeId = null,
                                Desc = "Select civil with legs support items",
                                enable = true,
                                Id = AllCivilInst.Id,
                                Key = "Select civil with legs support items",
                                Label = "Select civil with legs support items",
                                Manage = false,
                                Required = false,
                                Value = _unitOfWork.CivilWithLegsRepository.GetByID(AllCivilInst.civilWithLegsId.Value).Name
                            });
                            if (CivilLoads.sideArmId != null)
                            {
                                MWInstallationInfo.Add(new BaseInstAttView
                                {
                                    AutoFill = false,
                                    DataType = "List",
                                    DataTypeId = null,
                                    Desc = "Select the installation place type",
                                    enable = true,
                                    Id = -1,
                                    Key = "Select the installation place type",
                                    Label = "Select the installation place type",
                                    Manage = false,
                                    Required = false,
                                    Value = "SideArm"
                                });
                                MWInstallationInfo.Add(new BaseInstAttView
                                {
                                    AutoFill = false,
                                    DataType = "List",
                                    DataTypeId = null,
                                    Desc = "Select the sidearm",
                                    enable = true,
                                    Id = CivilLoads.sideArmId.Value,
                                    Key = "Select the sidearm",
                                    Label = "Select the sidearm",
                                    Manage = false,
                                    Required = false,
                                    Value = CivilLoads.sideArm.Name
                                });
                            }
                            else if (CivilLoads.legId != null)
                            {
                                MWInstallationInfo.Add(new BaseInstAttView
                                {
                                    AutoFill = false,
                                    DataType = "List",
                                    DataTypeId = null,
                                    Desc = "Select the installation place type",
                                    enable = true,
                                    Id = -1,
                                    Key = "Select the installation place type",
                                    Label = "Select the installation place type",
                                    Manage = false,
                                    Required = false,
                                    Value = "Leg"
                                });
                                MWInstallationInfo.Add(new BaseInstAttView
                                {
                                    AutoFill = false,
                                    DataType = "List",
                                    DataTypeId = null,
                                    Desc = "Select the leg",
                                    enable = true,
                                    Id = CivilLoads.legId.Value,
                                    Key = "Select the leg",
                                    Label = "Select the leg",
                                    Manage = false,
                                    Required = false,
                                    Value = CivilLoads.leg.CiviLegName
                                });
                            }
                            else
                            {
                                MWInstallationInfo.Add(new BaseInstAttView
                                {
                                    AutoFill = false,
                                    DataType = "List",
                                    DataTypeId = null,
                                    Desc = "Select the installation place type",
                                    enable = true,
                                    Id = -1,
                                    Key = "Select the installation place type",
                                    Label = "Select the installation place type",
                                    Manage = false,
                                    Required = false,
                                    Value = "Direct"
                                });
                            }
                            if (mw_BU.PortCascadeId != 0)
                            {
                                MWInstallationInfo.Add(new BaseInstAttView
                                {
                                    AutoFill = false,
                                    DataType = "List",
                                    DataTypeId = null,
                                    Desc = "Select BU installtion type",
                                    enable = true,
                                    Id = -1,
                                    Key = "Select BU installtion type",
                                    Label = "Select BU installtion type",
                                    Manage = false,
                                    Required = false,
                                    Value = "Cascaded"
                                });

                                TLImwPort CascadedBu = _unitOfWork.MW_PortRepository.GetIncludeWhereFirst(x => x.Id == mw_BU.PortCascadeId, x => x.MwBU);

                                MWInstallationInfo.Add(new BaseInstAttView
                                {
                                    AutoFill = false,
                                    DataType = "List",
                                    DataTypeId = null,
                                    Desc = "Select the cascaded BU",
                                    enable = true,
                                    Id = CascadedBu.MwBUId,
                                    Key = "Select the cascaded BU",
                                    Label = "Select the cascaded BU",
                                    Manage = false,
                                    Required = false,
                                    Value = CascadedBu.MwBU.Name
                                });
                            }
                            else
                            {
                                MWInstallationInfo.Add(new BaseInstAttView
                                {
                                    AutoFill = false,
                                    DataType = "List",
                                    DataTypeId = null,
                                    Desc = "Select BU installtion type",
                                    enable = true,
                                    Id = -1,
                                    Key = "Select BU installtion type",
                                    Label = "Select BU installtion type",
                                    Manage = false,
                                    Required = false,
                                    Value = "Normal"
                                });
                            }
                        }
                        else if (AllCivilInst.civilWithoutLegId != null)
                        {
                            MWInstallationInfo.Add(new BaseInstAttView
                            {
                                AutoFill = false,
                                DataType = "List",
                                DataTypeId = null,
                                Desc = "Select civil support type",
                                enable = true,
                                Id = -1,
                                Key = "Select civil support type",
                                Label = "Select civil support type",
                                Manage = false,
                                Required = false,
                                Value = "Civil without legs"
                            });
                            MWInstallationInfo.Add(new BaseInstAttView
                            {
                                AutoFill = false,
                                DataType = "List",
                                DataTypeId = null,
                                Desc = "Select civil without legs support items",
                                enable = true,
                                Id = AllCivilInst.Id,
                                Key = "Select civil without legs support items",
                                Label = "Select civil without legs support items",
                                Manage = false,
                                Required = false,
                                Value = _unitOfWork.CivilWithoutLegRepository.GetByID(AllCivilInst.civilWithoutLegId.Value).Name
                            });
                            if (CivilLoads.sideArmId != null)
                            {
                                MWInstallationInfo.Add(new BaseInstAttView
                                {
                                    AutoFill = false,
                                    DataType = "List",
                                    DataTypeId = null,
                                    Desc = "Select the installation place type",
                                    enable = true,
                                    Id = -1,
                                    Key = "Select the installation place type",
                                    Label = "Select the installation place type",
                                    Manage = false,
                                    Required = false,
                                    Value = "SideArm"
                                });
                                MWInstallationInfo.Add(new BaseInstAttView
                                {
                                    AutoFill = false,
                                    DataType = "List",
                                    DataTypeId = null,
                                    Desc = "Select the sidearm",
                                    enable = true,
                                    Id = CivilLoads.sideArmId.Value,
                                    Key = "Select the sidearm",
                                    Label = "Select the sidearm",
                                    Manage = false,
                                    Required = false,
                                    Value = CivilLoads.sideArm.Name
                                });
                            }
                            else
                            {
                                MWInstallationInfo.Add(new BaseInstAttView
                                {
                                    AutoFill = false,
                                    DataType = "List",
                                    DataTypeId = null,
                                    Desc = "Select the installation place type",
                                    enable = true,
                                    Id = -1,
                                    Key = "Select the installation place type",
                                    Label = "Select the installation place type",
                                    Manage = false,
                                    Required = false,
                                    Value = "Direct"
                                });
                            }
                            if (mw_BU.PortCascadeId != 0)
                            {
                                MWInstallationInfo.Add(new BaseInstAttView
                                {
                                    AutoFill = false,
                                    DataType = "List",
                                    DataTypeId = null,
                                    Desc = "Select BU installtion type",
                                    enable = true,
                                    Id = -1,
                                    Key = "Select BU installtion type",
                                    Label = "Select BU installtion type",
                                    Manage = false,
                                    Required = false,
                                    Value = "Cascaded"
                                });

                                TLImwPort CascadedBU = _unitOfWork.MW_PortRepository
                                    .GetIncludeWhereFirst(x => x.Id == mw_BU.PortCascadeId, x => x.MwBU);

                                MWInstallationInfo.Add(new BaseInstAttView
                                {
                                    AutoFill = false,
                                    DataType = "List",
                                    DataTypeId = null,
                                    Desc = "Select the cascaded BU",
                                    enable = true,
                                    Id = CascadedBU.MwBUId,
                                    Key = "Select the cascaded BU",
                                    Label = "Select the cascaded BU",
                                    Manage = false,
                                    Required = false,
                                    Value = CascadedBU.MwBU.Name
                                });
                            }
                            else
                            {
                                MWInstallationInfo.Add(new BaseInstAttView
                                {
                                    AutoFill = false,
                                    DataType = "List",
                                    DataTypeId = null,
                                    Desc = "Select BU installtion type",
                                    enable = true,
                                    Id = -1,
                                    Key = "Select BU installtion type",
                                    Label = "Select BU installtion type",
                                    Manage = false,
                                    Required = false,
                                    Value = "Normal"
                                });

                            }
                        }
                        else if (AllCivilInst.civilNonSteelId != null)
                        {
                            MWInstallationInfo.Add(new BaseInstAttView
                            {
                                AutoFill = false,
                                DataType = "List",
                                DataTypeId = null,
                                Desc = "Select civil support type",
                                enable = true,
                                Id = -1,
                                Key = "Select civil support type",
                                Label = "Select civil support type",
                                Manage = false,
                                Required = false,
                                Value = "Civil non steel"
                            });
                            MWInstallationInfo.Add(new BaseInstAttView
                            {
                                AutoFill = false,
                                DataType = "List",
                                DataTypeId = null,
                                Desc = "Select civil non steel support items",
                                enable = true,
                                Id = AllCivilInst.Id,
                                Key = "Select civil non steel support items",
                                Label = "Select civil non steel support items",
                                Manage = false,
                                Required = false,
                                Value = _unitOfWork.CivilNonSteelRepository.GetByID(AllCivilInst.civilNonSteelId.Value).Name
                            });
                            if (CivilLoads.sideArmId != null)
                            {
                                MWInstallationInfo.Add(new BaseInstAttView
                                {
                                    AutoFill = false,
                                    DataType = "List",
                                    DataTypeId = null,
                                    Desc = "Select the installation place type",
                                    enable = true,
                                    Id = -1,
                                    Key = "Select the installation place type",
                                    Label = "Select the installation place type",
                                    Manage = false,
                                    Required = false,
                                    Value = "SideArm"
                                });
                                MWInstallationInfo.Add(new BaseInstAttView
                                {
                                    AutoFill = false,
                                    DataType = "List",
                                    DataTypeId = null,
                                    Desc = "Select the sidearm",
                                    enable = true,
                                    Id = CivilLoads.sideArmId.Value,
                                    Key = "Select the sidearm",
                                    Label = "Select the sidearm",
                                    Manage = false,
                                    Required = false,
                                    Value = CivilLoads.sideArm.Name
                                });
                            }
                            else
                            {
                                MWInstallationInfo.Add(new BaseInstAttView
                                {
                                    AutoFill = false,
                                    DataType = "List",
                                    DataTypeId = null,
                                    Desc = "Select the installation place type",
                                    enable = true,
                                    Id = -1,
                                    Key = "Select the installation place type",
                                    Label = "Select the installation place type",
                                    Manage = false,
                                    Required = false,
                                    Value = "Direct"
                                });
                            }
                            if (mw_BU.PortCascadeId != 0)
                            {
                                MWInstallationInfo.Add(new BaseInstAttView
                                {
                                    AutoFill = false,
                                    DataType = "List",
                                    DataTypeId = null,
                                    Desc = "Select BU installtion type",
                                    enable = true,
                                    Id = -1,
                                    Key = "Select BU installtion type",
                                    Label = "Select BU installtion type",
                                    Manage = false,
                                    Required = false,
                                    Value = "Cascaded"
                                });

                                TLImwPort CascadedBU = _unitOfWork.MW_PortRepository.GetIncludeWhereFirst(x => x.Id == mw_BU.PortCascadeId, x => x.MwBU);

                                MWInstallationInfo.Add(new BaseInstAttView
                                {
                                    AutoFill = false,
                                    DataType = "List",
                                    DataTypeId = null,
                                    Desc = "Select the cascaded BU",
                                    enable = true,
                                    Id = CascadedBU.MwBUId,
                                    Key = "Select the cascaded BU",
                                    Label = "Select the cascaded BU",
                                    Manage = false,
                                    Required = false,
                                    Value = CascadedBU.MwBU.Name
                                });
                            }
                            else
                            {
                                MWInstallationInfo.Add(new BaseInstAttView
                                {
                                    AutoFill = false,
                                    DataType = "List",
                                    DataTypeId = null,
                                    Desc = "Select BU installtion type",
                                    enable = true,
                                    Id = -1,
                                    Key = "Select BU installtion type",
                                    Label = "Select BU installtion type",
                                    Manage = false,
                                    Required = false,
                                    Value = "Normal"
                                });

                            }
                        }

                        MWInstallationInfo.Add(new BaseInstAttView
                        {
                            AutoFill = false,
                            DataType = "List",
                            DataTypeId = null,
                            Desc = "allLoadInstId",
                            enable = false,
                            Id = AllLoadInst.Id,
                            Key = "allLoadInstId",
                            Label = "allLoadInstId",
                            Manage = false,
                            Required = false,
                            Value = AllLoadInst.Id
                        });
                        objectInst.SideArmInstallationInfo = MWInstallationInfo;
                    }
                }
                else if (LoadSubType.TLImwODU.ToString() == TableName)
                {
                    TLImwODU MW_ODU = _unitOfWork.MW_ODURepository
                        .GetIncludeWhereFirst(x => x.Id == MWInsId, x => x.MwODULibrary, x => x.Mw_Dish, x => x.OduInstallationType, x => x.Owner);

                    MW_ODULibraryViewModel MwOduLibrary = _mapper.Map<MW_ODULibraryViewModel>(_unitOfWork.MW_ODULibraryRepository
                        .GetIncludeWhereFirst(x => x.Id == MW_ODU.MwODULibraryId, x => x.parity));

                    List<BaseAttView> LibraryAttributes = _unitOfWork.AttributeActivatedRepository
                       .GetAttributeActivated(TablesNames.TLImwODULibrary.ToString(), MwOduLibrary, null).ToList();

                    TLIcivilLoads CivilLoad = _unitOfWork.CivilLoadsRepository.GetIncludeWhereFirst(x => !x.Dismantle && (x.allLoadInstId != null ?
                      x.allLoadInst.mwBUId == MWInsId : false), x => x.allLoadInst);

                    foreach (BaseAttView LibraryAttribute in LibraryAttributes)
                    {
                        if (LibraryAttribute.DataType.ToLower() == "list")
                        {
                            LibraryAttribute.Value = MwOduLibrary.GetType().GetProperties()
                                .FirstOrDefault(x => x.Name.ToLower() == LibraryAttribute.Label.ToLower()).GetValue(MwOduLibrary);
                        }
                    }

                    List<BaseAttView> LogisticalAttributes = _mapper.Map<List<BaseAttView>>(_unitOfWork.LogistcalRepository
                        .GetLogistical(TablePartName.MW.ToString(), TablesNames.TLImwODULibrary.ToString(), MwOduLibrary.Id).ToList());

                    LibraryAttributes.AddRange(LogisticalAttributes);

                    objectInst.LibraryActivatedAttributes = LibraryAttributes;


                    List<BaseInstAttView> ListAttributesActivated = _unitOfWork.AttributeActivatedRepository
                        .GetInstAttributeActivated(LoadSubType.TLImwODU.ToString(), MW_ODU, "OduInstallationTypeId", "Mw_DishId").ToList();

                    BaseInstAttView NameAttribute = ListAttributesActivated.FirstOrDefault(x => x.Key.ToLower() == "Name".ToLower());
                    if (NameAttribute != null)
                    {
                        BaseInstAttView Swap = ListAttributesActivated[0];
                        ListAttributesActivated[ListAttributesActivated.IndexOf(NameAttribute)] = Swap;
                        ListAttributesActivated[0] = NameAttribute;
                    }
                    foreach (BaseInstAttView FKitem in ListAttributesActivated)
                    {
                        if (FKitem.Desc.ToLower() == "tliowner")
                        {
                            if (MW_ODU.Owner == null)
                                FKitem.Value = _unitOfWork.OwnerRepository.GetByID((int)FKitem.Value).OwnerName;
                            else
                                FKitem.Value = _unitOfWork.OwnerRepository.GetWhereFirst(x => x.Id == MW_ODU.Owner.Id && !x.Deleted && x.Disable == false).OwnerName;
                        }
                        else if (FKitem.Desc.ToLower() == "tlimwdish")
                        {
                            if (MW_ODU.Mw_Dish == null)
                                FKitem.Value = "NA";

                            else
                            {

                                FKitem.Value = _unitOfWork.MW_DishRepository.GetWhereFirst(x => x.Id == MW_ODU.Mw_Dish.Id).DishName; ;
                            }

                        }
                        else if (FKitem.Desc.ToLower() == "tlimwodulibrary")
                        {
                            if (MW_ODU.MwODULibrary == null)
                                FKitem.Value = _unitOfWork.MW_ODULibraryRepository.GetWhereFirst(x => x.Id == 0).Model;

                            else
                                FKitem.Value = _unitOfWork.MW_ODULibraryRepository.GetWhereFirst(x => x.Id == MW_ODU.MwODULibrary.Id && !x.Deleted && x.Active == true).Model;
                        }
                    }

                    objectInst.AttributesActivated = ListAttributesActivated;

                    objectInst.DynamicAtts = _unitOfWork.DynamicAttInstValueRepository
                        .GetDynamicInstAtts(TableNameEntity.Id, MWInsId, null);

                    AllLoadInst = _unitOfWork.AllLoadInstRepository.GetWhereFirst(x => x.mwODUId == MWInsId);

                    CivilLoads = _unitOfWork.CivilLoadsRepository
                        .GetIncludeWhereFirst(x => x.allLoadInstId == AllLoadInst.Id, x => x.sideArm, x => x.site, x => x.leg, x => x.allCivilInst,
                            x => x.allLoadInst.loadOther, x => x.allLoadInst.mwBU, x => x.allLoadInst.mwDish, x => x.allLoadInst.mwODU, x => x.allLoadInst.mwOther,
                            x => x.allLoadInst.mwRFU, x => x.allLoadInst.power, x => x.allLoadInst.radioAntenna, x => x.allLoadInst.radioOther, x => x.allLoadInst.radioRRU,
                            x => x.allCivilInst.civilWithLegs, x => x.allCivilInst.civilWithoutLeg, x => x.allCivilInst.civilNonSteel, x => x.civilSteelSupportCategory);

                    LoadInstAttributes = _unitOfWork.AttributeActivatedRepository
                        .GetAttributeActivated(TablesNames.TLIcivilLoads.ToString(), CivilLoads, null, "allLoadInstId",
                            "Dismantle", "SiteCode", "civilSteelSupportCategoryId", "legId", "Leg2Id",
                                "sideArmId", "allCivilInstId").ToList();

                    List<KeyValuePair<string, List<DropDownListFilters>>> CivilLoadsRelatedTables = _unitOfWork.CivilLoadsRepository
                        .GetRelatedTables(CivilLoads.SiteCode);
                    if (CivilLoads != null)
                    {
                        List<KeyValuePair<string, List<DropDownListFilters>>> mwoduRelatedTables = _unitOfWork.MW_ODURepository
                            .GetRelatedTablesForEdit(CivilLoads.SiteCode, CivilLoads.allCivilInstId);

                        mwoduRelatedTables.AddRange(CivilLoadsRelatedTables);

                        if (CivilLoads.allCivilInst.civilWithLegsId != null)
                        {
                            List<TLIleg> LegsForCivilWithLegLibrary = _unitOfWork.LegRepository
                                .GetWhere(x => x.CivilWithLegInstId == CivilLoads.allCivilInst.civilWithLegsId).ToList();

                            List<DropDownListFilters> LegIds = _mapper.Map<List<DropDownListFilters>>(LegsForCivilWithLegLibrary);

                            mwoduRelatedTables.Add(new KeyValuePair<string, List<DropDownListFilters>>("Leg1Id", LegIds));

                            List<TLIleg> Legs2ForCivilWithLegLibrary = LegsForCivilWithLegLibrary.Except(LegsForCivilWithLegLibrary
                                .Where(x => x.Id == CivilLoads.legId)).ToList();

                            List<DropDownListFilters> Leg2Ids = _mapper.Map<List<DropDownListFilters>>(Legs2ForCivilWithLegLibrary);

                            mwoduRelatedTables.Add(new KeyValuePair<string, List<DropDownListFilters>>("Leg2Id", Leg2Ids));
                        }

                        objectInst.RelatedTables = mwoduRelatedTables;

                        AllCivilInst = _unitOfWork.CivilLoadsRepository
                            .GetIncludeWhereFirst(x => (x.allLoadInstId != null ? (x.allLoadInst.mwODUId != null ?
                                x.allLoadInst.mwODUId.Value == MWInsId : false) : false) && !x.Dismantle, x => x.allCivilInst, x => x.allLoadInst).allCivilInst;

                        if (AllCivilInst.civilWithLegsId != null)
                        {
                            MWInstallationInfo.Add(new BaseInstAttView
                            {
                                AutoFill = false,
                                DataType = "List",
                                DataTypeId = null,
                                Desc = "Select civil support type",
                                enable = true,
                                Id = -1,
                                Key = "Select civil support type",
                                Label = "Select civil support type",
                                Manage = false,
                                Required = false,
                                Value = "Civil with legs"
                            });
                            MWInstallationInfo.Add(new BaseInstAttView
                            {
                                AutoFill = false,
                                DataType = "List",
                                DataTypeId = null,
                                Desc = "Select civil with legs support items",
                                enable = true,
                                Id = AllCivilInst.Id,
                                Key = "Select civil with legs support items",
                                Label = "Select civil with legs support items",
                                Manage = false,
                                Required = false,
                                Value = _unitOfWork.CivilWithLegsRepository.GetByID(AllCivilInst.civilWithLegsId.Value).Name
                            });

                            if (CivilLoads.sideArmId != null)
                            {
                                MWInstallationInfo.Add(new BaseInstAttView
                                {
                                    AutoFill = false,
                                    DataType = "List",
                                    DataTypeId = null,
                                    Desc = "Select the installation place type",
                                    enable = true,
                                    Id = MW_ODU.OduInstallationTypeId.Value,
                                    Key = "Select the installation place type",
                                    Label = "Select the installation place type",
                                    Manage = false,
                                    Required = false,
                                    Value = _unitOfWork.OduInstallationTypeRepository.GetWhereFirst(x => x.Id == MW_ODU.OduInstallationTypeId.Value).Name
                                });
                                MWInstallationInfo.Add(new BaseInstAttView
                                {
                                    AutoFill = false,
                                    DataType = "List",
                                    DataTypeId = null,
                                    Desc = "Select the sidearm",
                                    enable = true,
                                    Id = CivilLoads.sideArmId.Value,
                                    Key = "Select the sidearm",
                                    Label = "Select the sidearm",
                                    Manage = false,
                                    Required = false,
                                    Value = CivilLoads.sideArm.Name
                                });
                            }
                            else if (CivilLoads.legId != null)
                            {
                                MWInstallationInfo.Add(new BaseInstAttView
                                {
                                    AutoFill = false,
                                    DataType = "List",
                                    DataTypeId = null,
                                    Desc = "Select the installation place type",
                                    enable = true,
                                    Id = MW_ODU.OduInstallationTypeId.Value,
                                    Key = "Select the installation place type",
                                    Label = "Select the installation place type",
                                    Manage = false,
                                    Required = false,
                                    Value = _unitOfWork.OduInstallationTypeRepository.GetWhereFirst(x => x.Id == MW_ODU.OduInstallationTypeId.Value).Name
                                });
                                MWInstallationInfo.Add(new BaseInstAttView
                                {
                                    AutoFill = false,
                                    DataType = "List",
                                    DataTypeId = null,
                                    Desc = "Select the seperate place",
                                    enable = true,
                                    Id = -1,
                                    Key = "Select the seperate place",
                                    Label = "Select the seperate place",
                                    Manage = false,
                                    Required = false,
                                    Value = "Leg"
                                });
                                MWInstallationInfo.Add(new BaseInstAttView
                                {
                                    AutoFill = false,
                                    DataType = "List",
                                    DataTypeId = null,
                                    Desc = "Select the leg",
                                    enable = true,
                                    Id = CivilLoads.legId.Value,
                                    Key = "Select the leg",
                                    Label = "Select the leg",
                                    Manage = false,
                                    Required = false,
                                    Value = CivilLoads.leg.CiviLegName
                                });
                            }
                            else if (MW_ODU.Mw_DishId != null ? MW_ODU.Mw_DishId != 0 : false)
                            {
                                MWInstallationInfo.Add(new BaseInstAttView
                                {
                                    AutoFill = false,
                                    DataType = "List",
                                    DataTypeId = null,
                                    Desc = "Select the installation place type",
                                    enable = true,
                                    Id = MW_ODU.OduInstallationTypeId.Value,
                                    Key = "Select the installation place type",
                                    Label = "Select the installation place type",
                                    Manage = false,
                                    Required = false,
                                    Value = _unitOfWork.OduInstallationTypeRepository.GetWhereFirst(x => x.Id == MW_ODU.OduInstallationTypeId).Name
                                });
                                MWInstallationInfo.Add(new BaseInstAttView
                                {
                                    AutoFill = false,
                                    DataType = "List",
                                    DataTypeId = null,
                                    Desc = "Select the dish ",
                                    enable = true,
                                    Id = MW_ODU.Mw_DishId.Value,
                                    Key = "Select the dish",
                                    Label = "Select the dish",
                                    Manage = false,
                                    Required = false,
                                    Value = _unitOfWork.MW_DishRepository.GetWhereFirst(x => x.Id == MW_ODU.Mw_DishId.Value).DishName
                                });
                            }
                            else
                            {
                                MWInstallationInfo.Add(new BaseInstAttView
                                {
                                    AutoFill = false,
                                    DataType = "List",
                                    DataTypeId = null,
                                    Desc = "Select the installation place type",
                                    enable = true,
                                    Id = MW_ODU.OduInstallationTypeId.Value,
                                    Key = "Select the installation place type",
                                    Label = "Select the installation place type",
                                    Manage = false,
                                    Required = false,
                                    Value = _unitOfWork.OduInstallationTypeRepository.GetWhereFirst(x => x.Id == MW_ODU.OduInstallationTypeId).Name
                                });
                                MWInstallationInfo.Add(new BaseInstAttView
                                {
                                    AutoFill = false,
                                    DataType = "List",
                                    DataTypeId = null,
                                    Desc = "Select the seperate place",
                                    enable = true,
                                    Id = -1,
                                    Key = "Select the seperate place",
                                    Label = "Select the seperate place",
                                    Manage = false,
                                    Required = false,
                                    Value = "Direct"
                                });
                            }
                        }
                        else if (AllCivilInst.civilWithoutLegId != null)
                        {
                            MWInstallationInfo.Add(new BaseInstAttView
                            {
                                AutoFill = false,
                                DataType = "List",
                                DataTypeId = null,
                                Desc = "Select civil support type",
                                enable = true,
                                Id = -1,
                                Key = "Select civil support type",
                                Label = "Select civil support type",
                                Manage = false,
                                Required = false,
                                Value = "Civil without legs"
                            });
                            MWInstallationInfo.Add(new BaseInstAttView
                            {
                                AutoFill = false,
                                DataType = "List",
                                DataTypeId = null,
                                Desc = "Select civil without legs support items",
                                enable = true,
                                Id = AllCivilInst.Id,
                                Key = "Select civil without legs support items",
                                Label = "Select civil without legs support items",
                                Manage = false,
                                Required = false,
                                Value = _unitOfWork.CivilWithoutLegRepository.GetByID(AllCivilInst.civilWithoutLegId.Value).Name
                            });

                            if (CivilLoads.sideArmId != null)
                            {
                                MWInstallationInfo.Add(new BaseInstAttView
                                {
                                    AutoFill = false,
                                    DataType = "List",
                                    DataTypeId = null,
                                    Desc = "Select the installation place type",
                                    enable = true,
                                    Id = MW_ODU.OduInstallationTypeId.Value,
                                    Key = "Select the installation place type",
                                    Label = "Select the installation place type",
                                    Manage = false,
                                    Required = false,
                                    Value = _unitOfWork.OduInstallationTypeRepository.GetWhereFirst(x => x.Id == MW_ODU.OduInstallationTypeId.Value).Name
                                });
                                MWInstallationInfo.Add(new BaseInstAttView
                                {
                                    AutoFill = false,
                                    DataType = "List",
                                    DataTypeId = null,
                                    Desc = "Select the sidearm",
                                    enable = true,
                                    Id = CivilLoads.sideArmId.Value,
                                    Key = "Select the sidearm",
                                    Label = "Select the sidearm",
                                    Manage = false,
                                    Required = false,
                                    Value = CivilLoads.sideArm.Name
                                });
                            }
                            else if (MW_ODU.Mw_DishId != null ? MW_ODU.Mw_DishId != 0 : false)
                            {
                                MWInstallationInfo.Add(new BaseInstAttView
                                {
                                    AutoFill = false,
                                    DataType = "List",
                                    DataTypeId = null,
                                    Desc = "Select the installation place type",
                                    enable = true,
                                    Id = MW_ODU.OduInstallationTypeId.Value,
                                    Key = "Select the installation place type",
                                    Label = "Select the installation place type",
                                    Manage = false,
                                    Required = false,
                                    Value = _unitOfWork.OduInstallationTypeRepository.GetWhereFirst(x => x.Id == MW_ODU.OduInstallationTypeId.Value).Name
                                });
                                MWInstallationInfo.Add(new BaseInstAttView
                                {
                                    AutoFill = false,
                                    DataType = "List",
                                    DataTypeId = null,
                                    Desc = "Select the dish",
                                    enable = true,
                                    Id = MW_ODU.Mw_DishId.Value,
                                    Key = "Select the dish",
                                    Label = "Select the dish",
                                    Manage = false,
                                    Required = false,
                                    Value = _unitOfWork.MW_DishRepository.GetWhereFirst(x => x.Id == MW_ODU.Mw_DishId).DishName
                                });
                            }
                            else
                            {
                                MWInstallationInfo.Add(new BaseInstAttView
                                {
                                    AutoFill = false,
                                    DataType = "List",
                                    DataTypeId = null,
                                    Desc = "Select the installation place type",
                                    enable = true,
                                    Id = MW_ODU.OduInstallationTypeId.Value,
                                    Key = "Select the installation place type",
                                    Label = "Select the installation place type",
                                    Manage = false,
                                    Required = false,
                                    Value = _unitOfWork.OduInstallationTypeRepository.GetWhereFirst(x => x.Id == MW_ODU.OduInstallationTypeId).Name
                                });
                            }
                        }
                        else if (AllCivilInst.civilNonSteelId != null)
                        {
                            MWInstallationInfo.Add(new BaseInstAttView
                            {
                                AutoFill = false,
                                DataType = "List",
                                DataTypeId = null,
                                Desc = "Select civil support type",
                                enable = true,
                                Id = -1,
                                Key = "Select civil support type",
                                Label = "Select civil support type",
                                Manage = false,
                                Required = false,
                                Value = "Civil non steel"
                            });
                            MWInstallationInfo.Add(new BaseInstAttView
                            {
                                AutoFill = false,
                                DataType = "List",
                                DataTypeId = null,
                                Desc = "Select civil non steel support items",
                                enable = true,
                                Id = AllCivilInst.Id,
                                Key = "Select civil non steel support items",
                                Label = "Select civil non steel support items",
                                Manage = false,
                                Required = false,
                                Value = _unitOfWork.CivilNonSteelRepository.GetByID(AllCivilInst.civilNonSteelId.Value).Name
                            });

                            if (CivilLoads.sideArmId != null)
                            {
                                MWInstallationInfo.Add(new BaseInstAttView
                                {
                                    AutoFill = false,
                                    DataType = "List",
                                    DataTypeId = null,
                                    Desc = "Select the installation place type",
                                    enable = true,
                                    Id = MW_ODU.OduInstallationTypeId.Value,
                                    Key = "Select the installation place type",
                                    Label = "Select the installation place type",
                                    Manage = false,
                                    Required = false,
                                    Value = _unitOfWork.OduInstallationTypeRepository.GetWhereFirst(x => x.Id == MW_ODU.OduInstallationTypeId.Value).Name
                                });
                                MWInstallationInfo.Add(new BaseInstAttView
                                {
                                    AutoFill = false,
                                    DataType = "List",
                                    DataTypeId = null,
                                    Desc = "Select the sidearm",
                                    enable = true,
                                    Id = CivilLoads.sideArmId.Value,
                                    Key = "Select the sidearm",
                                    Label = "Select the sidearm",
                                    Manage = false,
                                    Required = false,
                                    Value = CivilLoads.sideArm.Name
                                });
                            }
                            else if (MW_ODU.Mw_DishId != null ? MW_ODU.Mw_DishId != 0 : false)
                            {
                                MWInstallationInfo.Add(new BaseInstAttView
                                {
                                    AutoFill = false,
                                    DataType = "List",
                                    DataTypeId = null,
                                    Desc = "Select the installation place type",
                                    enable = true,
                                    Id = MW_ODU.OduInstallationTypeId.Value,
                                    Key = "Select the installation place type",
                                    Label = "Select the installation place type",
                                    Manage = false,
                                    Required = false,
                                    Value = _unitOfWork.OduInstallationTypeRepository.GetWhereFirst(x => x.Id == MW_ODU.OduInstallationTypeId.Value).Name
                                });
                                MWInstallationInfo.Add(new BaseInstAttView
                                {
                                    AutoFill = false,
                                    DataType = "List",
                                    DataTypeId = null,
                                    Desc = "Select the dish",
                                    enable = true,
                                    Id = MW_ODU.Mw_DishId.Value,
                                    Key = "Select the dish",
                                    Label = "Select the dish",
                                    Manage = false,
                                    Required = false,
                                    Value = _unitOfWork.MW_DishRepository.GetWhereFirst(x => x.Id == MW_ODU.Mw_DishId).DishName
                                });
                            }
                            else
                            {
                                MWInstallationInfo.Add(new BaseInstAttView
                                {
                                    AutoFill = false,
                                    DataType = "List",
                                    DataTypeId = null,
                                    Desc = "Select the installation place type",
                                    enable = true,
                                    Id = MW_ODU.OduInstallationTypeId.Value,
                                    Key = "Select the installation place type",
                                    Label = "Select the installation place type",
                                    Manage = false,
                                    Required = false,
                                    Value = _unitOfWork.OduInstallationTypeRepository.GetWhereFirst(x => x.Id == MW_ODU.OduInstallationTypeId).Name
                                });
                            }
                        }

                        MWInstallationInfo.Add(new BaseInstAttView
                        {
                            AutoFill = false,
                            DataType = "List",
                            DataTypeId = null,
                            Desc = "allLoadInstId",
                            enable = false,
                            Id = AllLoadInst.Id,
                            Key = "allLoadInstId",
                            Label = "allLoadInstId",
                            Manage = false,
                            Required = false,
                            Value = AllLoadInst.Id
                        });
                        objectInst.SideArmInstallationInfo = MWInstallationInfo;
                    }
                }
                else if (LoadSubType.TLImwDish.ToString() == TableName)
                {
                    TLImwDish MW_Dish = _unitOfWork.MW_DishRepository
                        .GetIncludeWhereFirst(x => x.Id == MWInsId, x => x.MwDishLibrary,
                            x => x.RepeaterType, x => x.owner, x => x.PolarityOnLocation,
                            x => x.ItemConnectTo, x => x.InstallationPlace);

                    MW_DishLibraryViewModel MwdishLibrary = _mapper.Map<MW_DishLibraryViewModel>(_unitOfWork.MW_DishLibraryRepository
                        .GetIncludeWhereFirst(x => x.Id == MW_Dish.MwDishLibraryId, x => x.polarityType, x => x.asType));

                    List<BaseAttView> LibraryAttributes = _unitOfWork.AttributeActivatedRepository
                       .GetAttributeActivated(TablesNames.TLImwDishLibrary.ToString(), MwdishLibrary, null).ToList();

                    foreach (BaseAttView LibraryAttribute in LibraryAttributes)
                    {
                        if (LibraryAttribute.DataType.ToLower() == "list")
                        {
                            LibraryAttribute.Value = MwdishLibrary.GetType().GetProperties()
                                .FirstOrDefault(x => x.Name.ToLower() == LibraryAttribute.Label.ToLower()).GetValue(MwdishLibrary);
                        }
                    }

                    List<BaseAttView> LogisticalAttributes = _mapper.Map<List<BaseAttView>>(_unitOfWork.LogistcalRepository
                       .GetLogistical(TablePartName.MW.ToString(), TablesNames.TLImwDishLibrary.ToString(), MwdishLibrary.Id).ToList());

                    LibraryAttributes.AddRange(LogisticalAttributes);

                    objectInst.LibraryActivatedAttributes = LibraryAttributes;

                    List<BaseInstAttView> ListAttributesActivated = _unitOfWork.AttributeActivatedRepository
                        .GetInstAttributeActivated(TablesNames.TLImwDish.ToString(), MW_Dish,
                            "InstallationPlaceId").ToList();

                    BaseInstAttView NameAttribute = ListAttributesActivated.FirstOrDefault(x => x.Key.ToLower() == "DishName".ToLower());
                    if (NameAttribute != null)
                    {
                        BaseInstAttView Swap = ListAttributesActivated[0];
                        ListAttributesActivated[ListAttributesActivated.IndexOf(NameAttribute)] = Swap;
                        ListAttributesActivated[0] = NameAttribute;
                    }
                    foreach (BaseInstAttView FKitem in ListAttributesActivated)
                    {
                        if (FKitem.Desc.ToLower() == "tliowner")
                        {
                            if (MW_Dish.owner == null)
                                FKitem.Value = _unitOfWork.OwnerRepository.GetWhereFirst(x => x.Id == 0).OwnerName;

                            else
                                FKitem.Value = _unitOfWork.OwnerRepository.GetWhereFirst(x => x.Id == MW_Dish.owner.Id && !x.Deleted && x.Disable == false).OwnerName;
                        }
                        else if (FKitem.Desc.ToLower() == "tlirepeatertype")
                        {
                            if (MW_Dish.RepeaterType == null)
                                FKitem.Value = _unitOfWork.RepeaterTypeRepository.GetWhereFirst(x => x.Id == 0).Name;

                            else
                                FKitem.Value = _unitOfWork.RepeaterTypeRepository.GetWhereFirst(x => x.Id == MW_Dish.RepeaterType.Id && !x.Deleted && x.Disable == false).Name;
                        }
                        else if (FKitem.Desc.ToLower() == "tlipolarityonlocation")
                        {
                            if (MW_Dish.PolarityOnLocation == null)
                                FKitem.Value = _unitOfWork.PolarityOnLocationRepository.GetWhereFirst(x => x.Id == 0).Name;

                            else
                                FKitem.Value = _unitOfWork.PolarityOnLocationRepository.GetWhereFirst(x => x.Id == MW_Dish.PolarityOnLocation.Id && !x.Deleted && x.Disable == false).Name;
                        }
                        else if (FKitem.Desc.ToLower() == "tliitemconnectto")
                        {
                            if (MW_Dish.ItemConnectTo == null)
                                FKitem.Value = _unitOfWork.ItemConnectToRepository.GetWhereFirst(x => x.Id == 0).Name;

                            else
                                FKitem.Value = _unitOfWork.ItemConnectToRepository.GetWhereFirst(x => x.Id == MW_Dish.ItemConnectTo.Id && !x.Deleted && x.Disable == false).Name;
                        }
                        else if (FKitem.Desc.ToLower() == "tlimwdishlibrary")
                        {
                            if (MW_Dish.MwDishLibrary == null)
                                FKitem.Value = _unitOfWork.MW_DishLibraryRepository.GetWhereFirst(x => x.Id == 0).Model;

                            else
                                FKitem.Value = _unitOfWork.MW_DishLibraryRepository.GetWhereFirst(x => x.Id == MW_Dish.MwDishLibrary.Id && !x.Deleted && x.Active == true).Model;
                        }
                    }

                    objectInst.AttributesActivated = ListAttributesActivated;

                    objectInst.DynamicAtts = _unitOfWork.DynamicAttInstValueRepository.
                        GetDynamicInstAtts(TableNameEntity.Id, MWInsId, null);

                    AllLoadInst = _unitOfWork.AllLoadInstRepository.GetWhereFirst(x => x.mwDishId == MWInsId);

                    CivilLoads = _unitOfWork.CivilLoadsRepository
                       .GetIncludeWhereFirst(x => x.allLoadInstId == AllLoadInst.Id, x => x.sideArm, x => x.site, x => x.leg, x => x.allCivilInst,
                           x => x.allLoadInst.loadOther, x => x.allLoadInst.mwBU, x => x.allLoadInst.mwDish, x => x.allLoadInst.mwODU, x => x.allLoadInst.mwOther,
                           x => x.allLoadInst.mwRFU, x => x.allLoadInst.power, x => x.allLoadInst.radioAntenna, x => x.allLoadInst.radioOther, x => x.allLoadInst.radioRRU,
                           x => x.allCivilInst.civilWithLegs, x => x.allCivilInst.civilWithoutLeg, x => x.allCivilInst.civilNonSteel, x => x.civilSteelSupportCategory);

                    LoadInstAttributes = _unitOfWork.AttributeActivatedRepository
                        .GetAttributeActivated(TablesNames.TLIcivilLoads.ToString(), CivilLoads, null, "allLoadInstId",
                            "Dismantle", "SiteCode", "civilSteelSupportCategoryId", "legId", "Leg2Id",
                                "sideArmId", "allCivilInstId").ToList();

                    List<KeyValuePair<string, List<DropDownListFilters>>> CivilLoadsRelatedTables = _unitOfWork.CivilLoadsRepository
                        .GetRelatedTables(CivilLoads.SiteCode);
                    if (CivilLoads != null)
                    {
                        List<KeyValuePair<string, List<DropDownListFilters>>> mwdishRelatedTables = _unitOfWork.MW_DishRepository
                            .GetRelatedTables();
                        mwdishRelatedTables.AddRange(CivilLoadsRelatedTables);

                        if (CivilLoads.allCivilInst.civilWithLegsId != null)
                        {
                            List<TLIleg> LegsForCivilWithLegLibrary = _unitOfWork.LegRepository
                                .GetWhere(x => x.CivilWithLegInstId == CivilLoads.allCivilInst.civilWithLegsId).ToList();

                            List<DropDownListFilters> LegIds = _mapper.Map<List<DropDownListFilters>>(LegsForCivilWithLegLibrary);

                            mwdishRelatedTables.Add(new KeyValuePair<string, List<DropDownListFilters>>("Leg1Id", LegIds));

                            List<TLIleg> Legs2ForCivilWithLegLibrary = LegsForCivilWithLegLibrary.Except(LegsForCivilWithLegLibrary
                                .Where(x => x.Id == CivilLoads.legId)).ToList();

                            List<DropDownListFilters> Leg2Ids = _mapper.Map<List<DropDownListFilters>>(Legs2ForCivilWithLegLibrary);

                            mwdishRelatedTables.Add(new KeyValuePair<string, List<DropDownListFilters>>("Leg2Id", Leg2Ids));
                        }

                        objectInst.RelatedTables = mwdishRelatedTables;

                        AllCivilInst = _unitOfWork.CivilLoadsRepository
                           .GetIncludeWhereFirst(x => (x.allLoadInstId != null ? (x.allLoadInst.mwDishId != null ?
                               x.allLoadInst.mwDishId.Value == MWInsId : false) : false) && !x.Dismantle, x => x.allCivilInst, x => x.allLoadInst).allCivilInst;

                        if (AllCivilInst.civilWithLegsId != null)
                        {
                            MWInstallationInfo.Add(new BaseInstAttView
                            {
                                AutoFill = false,
                                DataType = "List",
                                DataTypeId = null,
                                Desc = "Select civil support type",
                                enable = true,
                                Id = -1,
                                Key = "Select civil support type",
                                Label = "Select civil support type",
                                Manage = false,
                                Required = false,
                                Value = "Civil with legs"
                            });
                            MWInstallationInfo.Add(new BaseInstAttView
                            {
                                AutoFill = false,
                                DataType = "List",
                                DataTypeId = null,
                                Desc = "Select civil with legs support items",
                                enable = true,
                                Id = AllCivilInst.Id,
                                Key = "Select civil with legs support items",
                                Label = "Select civil with legs support items",
                                Manage = false,
                                Required = false,
                                Value = _unitOfWork.CivilWithLegsRepository.GetByID(AllCivilInst.civilWithLegsId.Value).Name
                            });

                            if (CivilLoads.sideArmId != null)
                            {
                                MWInstallationInfo.Add(new BaseInstAttView
                                {
                                    AutoFill = false,
                                    DataType = "List",
                                    DataTypeId = null,
                                    Desc = "Select the installation place type",
                                    enable = true,
                                    Id = -1,
                                    Key = "Select the installation place type",
                                    Label = "Select the installation place type",
                                    Manage = false,
                                    Required = false,
                                    Value = "SideArm"
                                });
                                MWInstallationInfo.Add(new BaseInstAttView
                                {
                                    AutoFill = false,
                                    DataType = "List",
                                    DataTypeId = null,
                                    Desc = "Select the sidearm",
                                    enable = true,
                                    Id = CivilLoads.sideArmId.Value,
                                    Key = "Select the sidearm",
                                    Label = "Select the sidearm",
                                    Manage = false,
                                    Required = false,
                                    Value = CivilLoads.sideArm.Name
                                });
                            }
                            else if (CivilLoads.legId != null)
                            {
                                MWInstallationInfo.Add(new BaseInstAttView
                                {
                                    AutoFill = false,
                                    DataType = "List",
                                    DataTypeId = null,
                                    Desc = "Select the installation place type",
                                    enable = true,
                                    Id = -1,
                                    Key = "Select the installation place type",
                                    Label = "Select the installation place type",
                                    Manage = false,
                                    Required = false,
                                    Value = "Leg"
                                });
                                MWInstallationInfo.Add(new BaseInstAttView
                                {
                                    AutoFill = false,
                                    DataType = "List",
                                    DataTypeId = null,
                                    Desc = "Select the leg",
                                    enable = true,
                                    Id = CivilLoads.legId.Value,
                                    Key = "Select the leg",
                                    Label = "Select the leg",
                                    Manage = false,
                                    Required = false,
                                    Value = CivilLoads.leg.CiviLegName
                                });
                            }
                            else
                            {
                                MWInstallationInfo.Add(new BaseInstAttView
                                {
                                    AutoFill = false,
                                    DataType = "List",
                                    DataTypeId = null,
                                    Desc = "Select the installation place type",
                                    enable = true,
                                    Id = -1,
                                    Key = "Select the installation place type",
                                    Label = "Select the installation place type",
                                    Manage = false,
                                    Required = false,
                                    Value = "Direct"
                                });
                            }
                        }
                        else if (AllCivilInst.civilWithoutLegId != null)
                        {
                            MWInstallationInfo.Add(new BaseInstAttView
                            {
                                AutoFill = false,
                                DataType = "List",
                                DataTypeId = null,
                                Desc = "Select civil support type",
                                enable = true,
                                Id = -1,
                                Key = "Select civil support type",
                                Label = "Select civil support type",
                                Manage = false,
                                Required = false,
                                Value = "Civil without legs"
                            });
                            MWInstallationInfo.Add(new BaseInstAttView
                            {
                                AutoFill = false,
                                DataType = "List",
                                DataTypeId = null,
                                Desc = "Select civil without legs support items",
                                enable = true,
                                Id = AllCivilInst.Id,
                                Key = "Select civil without legs support items",
                                Label = "Select civil without legs support items",
                                Manage = false,
                                Required = false,
                                Value = _unitOfWork.CivilWithoutLegRepository.GetByID(AllCivilInst.civilWithoutLegId.Value).Name
                            });

                            if (CivilLoads.sideArmId != null)
                            {
                                MWInstallationInfo.Add(new BaseInstAttView
                                {
                                    AutoFill = false,
                                    DataType = "List",
                                    DataTypeId = null,
                                    Desc = "Select the installation place type",
                                    enable = true,
                                    Id = -1,
                                    Key = "Select the installation place type",
                                    Label = "Select the installation place type",
                                    Manage = false,
                                    Required = false,
                                    Value = "SideArm"
                                });
                                MWInstallationInfo.Add(new BaseInstAttView
                                {
                                    AutoFill = false,
                                    DataType = "List",
                                    DataTypeId = null,
                                    Desc = "Select the sidearm",
                                    enable = true,
                                    Id = CivilLoads.sideArmId.Value,
                                    Key = "Select the sidearm",
                                    Label = "Select the sidearm",
                                    Manage = false,
                                    Required = false,
                                    Value = CivilLoads.sideArm.Name
                                });
                            }
                            else
                            {
                                MWInstallationInfo.Add(new BaseInstAttView
                                {
                                    AutoFill = false,
                                    DataType = "List",
                                    DataTypeId = null,
                                    Desc = "Select the installation place type",
                                    enable = true,
                                    Id = -1,
                                    Key = "Select the installation place type",
                                    Label = "Select the installation place type",
                                    Manage = false,
                                    Required = false,
                                    Value = "Direct"
                                });
                            }
                        }
                        else if (AllCivilInst.civilNonSteelId != null)
                        {
                            MWInstallationInfo.Add(new BaseInstAttView
                            {
                                AutoFill = false,
                                DataType = "List",
                                DataTypeId = null,
                                Desc = "Select civil support type",
                                enable = true,
                                Id = -1,
                                Key = "Select civil support type",
                                Label = "Select civil support type",
                                Manage = false,
                                Required = false,
                                Value = "Civil non steel"
                            });
                            MWInstallationInfo.Add(new BaseInstAttView
                            {
                                AutoFill = false,
                                DataType = "List",
                                DataTypeId = null,
                                Desc = "Select civil non steel support items",
                                enable = true,
                                Id = AllCivilInst.Id,
                                Key = "Select civil non steel support items",
                                Label = "Select civil non steel support items",
                                Manage = false,
                                Required = false,
                                Value = _unitOfWork.CivilNonSteelRepository.GetByID(AllCivilInst.civilNonSteelId.Value).Name
                            });

                            if (CivilLoads.sideArmId != null)
                            {
                                MWInstallationInfo.Add(new BaseInstAttView
                                {
                                    AutoFill = false,
                                    DataType = "List",
                                    DataTypeId = null,
                                    Desc = "Select the installation place type",
                                    enable = true,
                                    Id = -1,
                                    Key = "Select the installation place type",
                                    Label = "Select the installation place type",
                                    Manage = false,
                                    Required = false,
                                    Value = "SideArm"
                                });
                                MWInstallationInfo.Add(new BaseInstAttView
                                {
                                    AutoFill = false,
                                    DataType = "List",
                                    DataTypeId = null,
                                    Desc = "Select the sidearm",
                                    enable = true,
                                    Id = CivilLoads.sideArmId.Value,
                                    Key = "Select the sidearm",
                                    Label = "Select the sidearm",
                                    Manage = false,
                                    Required = false,
                                    Value = CivilLoads.sideArm.Name
                                });
                            }
                            else
                            {
                                MWInstallationInfo.Add(new BaseInstAttView
                                {
                                    AutoFill = false,
                                    DataType = "List",
                                    DataTypeId = null,
                                    Desc = "Select the installation place type",
                                    enable = true,
                                    Id = -1,
                                    Key = "Select the installation place type",
                                    Label = "Select the installation place type",
                                    Manage = false,
                                    Required = false,
                                    Value = "Direct"
                                });
                            }
                        }

                        MWInstallationInfo.Add(new BaseInstAttView
                        {
                            AutoFill = false,
                            DataType = "List",
                            DataTypeId = null,
                            Desc = "allLoadInstId",
                            enable = false,
                            Id = AllLoadInst.Id,
                            Key = "allLoadInstId",
                            Label = "allLoadInstId",
                            Manage = false,
                            Required = false,
                            Value = AllLoadInst.Id
                        });
                        objectInst.SideArmInstallationInfo = MWInstallationInfo;
                    }
                }
                else if (LoadSubType.TLImwRFU.ToString() == TableName)
                {
                    TLImwRFU MW_RFU = _unitOfWork.MW_RFURepository
                        .GetIncludeWhereFirst(x => x.Id == MWInsId, x => x.Owner,
                            x => x.MwPort, x => x.MwRFULibrary, x => x.MwPort.MwBU);

                    MW_RFULibraryViewModel MwRfuLibrary = _mapper.Map<MW_RFULibraryViewModel>(_unitOfWork.MW_RFULibraryRepository
                      .GetIncludeWhereFirst(x => x.Id == MW_RFU.MwRFULibraryId, x => x.diversityType, x => x.boardType));

                    List<BaseAttView> LibraryAttributes = _unitOfWork.AttributeActivatedRepository
                       .GetAttributeActivated(TablesNames.TLImwRFULibrary.ToString(), MwRfuLibrary, null).ToList();

                    foreach (BaseAttView LibraryAttribute in LibraryAttributes)
                    {
                        if (LibraryAttribute.DataType.ToLower() == "list")
                        {
                            LibraryAttribute.Value = MwRfuLibrary.GetType().GetProperties()
                                .FirstOrDefault(x => x.Name.ToLower() == LibraryAttribute.Label.ToLower()).GetValue(MwRfuLibrary);
                        }
                    }

                    List<BaseAttView> LogisticalAttributes = _mapper.Map<List<BaseAttView>>(_unitOfWork.LogistcalRepository
                        .GetLogistical(TablePartName.MW.ToString(), TablesNames.TLImwRFULibrary.ToString(), MwRfuLibrary.Id).ToList());

                    LibraryAttributes.AddRange(LogisticalAttributes);

                    objectInst.LibraryActivatedAttributes = LibraryAttributes;

                    List<BaseInstAttView> ListAttributesActivated = _unitOfWork.AttributeActivatedRepository
                        .GetInstAttributeActivated(LoadSubType.TLImwRFU.ToString(), MW_RFU).ToList();

                    BaseInstAttView NameAttribute = ListAttributesActivated.FirstOrDefault(x => x.Key.ToLower() == "Name".ToLower());
                    if (NameAttribute != null)
                    {
                        BaseInstAttView Swap = ListAttributesActivated[0];
                        ListAttributesActivated[ListAttributesActivated.IndexOf(NameAttribute)] = Swap;
                        ListAttributesActivated[0] = NameAttribute;
                    }
                    foreach (BaseInstAttView FKitem in ListAttributesActivated)
                    {
                        if (FKitem.Desc.ToLower() == "tliowner")
                        {

                            if (MW_RFU.Owner == null)
                                FKitem.Value = _unitOfWork.OwnerRepository.GetWhereFirst(x => x.Id == 0).OwnerName;

                            else
                                FKitem.Value = _unitOfWork.OwnerRepository.GetWhereFirst(x => x.Id == MW_RFU.Owner.Id && !x.Deleted && x.Disable == false).OwnerName;
                        }
                        else if (FKitem.Desc.ToLower() == "tlimwport")
                        {
                            if (MW_RFU.MwPort == null)
                                FKitem.Value = _unitOfWork.MW_PortRepository.GetWhereFirst(x => x.Id == 0).Port_Name;

                            else
                                FKitem.Value = MW_RFU.MwPort.Port_Name;
                        }
                        else if (FKitem.Desc.ToLower() == "tlimwrfulibrary")
                        {
                            if (MW_RFU.MwRFULibrary == null)
                                FKitem.Value = _unitOfWork.MW_RFULibraryRepository.GetWhereFirst(x => x.Id == 0).Model;

                            else
                                FKitem.Value = _unitOfWork.MW_RFULibraryRepository.GetWhereFirst(x => x.Id == MW_RFU.MwRFULibrary.Id && !x.Deleted && x.Active == true).Model;
                        }
                    }
                    objectInst.AttributesActivated = ListAttributesActivated;

                    objectInst.DynamicAtts = _unitOfWork.DynamicAttInstValueRepository
                       .GetDynamicInstAtts(TableNameEntity.Id, MWInsId, null);

                    AllLoadInst = _unitOfWork.AllLoadInstRepository.GetWhereFirst(x => x.mwRFUId == MWInsId);

                    CivilLoads = _unitOfWork.CivilLoadsRepository
                        .GetIncludeWhereFirst(x => x.allLoadInstId == AllLoadInst.Id, x => x.sideArm, x => x.site, x => x.leg, x => x.allCivilInst,
                            x => x.allLoadInst.loadOther, x => x.allLoadInst.mwBU, x => x.allLoadInst.mwDish, x => x.allLoadInst.mwODU, x => x.allLoadInst.mwOther,
                            x => x.allLoadInst.mwRFU, x => x.allLoadInst.power, x => x.allLoadInst.radioAntenna, x => x.allLoadInst.radioOther, x => x.allLoadInst.radioRRU,
                            x => x.allCivilInst.civilWithLegs, x => x.allCivilInst.civilWithoutLeg, x => x.allCivilInst.civilNonSteel, x => x.civilSteelSupportCategory);

                    LoadInstAttributes = _unitOfWork.AttributeActivatedRepository
                        .GetAttributeActivated(TablesNames.TLIcivilLoads.ToString(), CivilLoads, null, "allLoadInstId",
                            "Dismantle", "SiteCode", "civilSteelSupportCategoryId", "legId", "Leg2Id",
                                "sideArmId", "allCivilInstId").ToList();

                    List<KeyValuePair<string, List<DropDownListFilters>>> CivilLoadsRelatedTables = _unitOfWork.CivilLoadsRepository
                        .GetRelatedTables(CivilLoads.SiteCode);
                    if (CivilLoads != null)
                    {
                        List<KeyValuePair<string, List<DropDownListFilters>>> mwrfuRelatedTables = _unitOfWork.MW_RFURepository
                            .GetRelatedTables();
                        mwrfuRelatedTables.AddRange(CivilLoadsRelatedTables);

                        if (CivilLoads.allCivilInst.civilWithLegsId != null)
                        {
                            List<TLIleg> LegsForCivilWithLegLibrary = _unitOfWork.LegRepository
                                .GetWhere(x => x.CivilWithLegInstId == CivilLoads.allCivilInst.civilWithLegsId).ToList();

                            List<DropDownListFilters> LegIds = _mapper.Map<List<DropDownListFilters>>(LegsForCivilWithLegLibrary);

                            mwrfuRelatedTables.Add(new KeyValuePair<string, List<DropDownListFilters>>("Leg1Id", LegIds));

                            List<TLIleg> Legs2ForCivilWithLegLibrary = LegsForCivilWithLegLibrary.Except(LegsForCivilWithLegLibrary
                                .Where(x => x.Id == CivilLoads.legId)).ToList();

                            List<DropDownListFilters> Leg2Ids = _mapper.Map<List<DropDownListFilters>>(Legs2ForCivilWithLegLibrary);

                            mwrfuRelatedTables.Add(new KeyValuePair<string, List<DropDownListFilters>>("Leg2Id", Leg2Ids));
                        }

                        objectInst.RelatedTables = mwrfuRelatedTables;

                        AllCivilInst = _unitOfWork.CivilLoadsRepository
                           .GetIncludeWhereFirst(x => (x.allLoadInstId != null ? (x.allLoadInst.mwRFUId != null ?
                               x.allLoadInst.mwRFUId.Value == MWInsId : false) : false) && !x.Dismantle, x => x.allCivilInst, x => x.allLoadInst).allCivilInst;

                        if (AllCivilInst.civilWithLegsId != null)
                        {
                            MWInstallationInfo.Add(new BaseInstAttView
                            {
                                AutoFill = false,
                                DataType = "List",
                                DataTypeId = null,
                                Desc = "Select civil support type",
                                enable = true,
                                Id = -1,
                                Key = "Select civil support type",
                                Label = "Select civil support type",
                                Manage = false,
                                Required = false,
                                Value = "Civil with legs"
                            });
                            MWInstallationInfo.Add(new BaseInstAttView
                            {
                                AutoFill = false,
                                DataType = "List",
                                DataTypeId = null,
                                Desc = "Select civil with legs support items",
                                enable = true,
                                Id = AllCivilInst.Id,
                                Key = "Select civil with legs support items",
                                Label = "Select civil with legs support items",
                                Manage = false,
                                Required = false,
                                Value = _unitOfWork.CivilWithLegsRepository.GetByID(AllCivilInst.civilWithLegsId.Value).Name
                            });
                            MWInstallationInfo.Add(new BaseInstAttView
                            {
                                AutoFill = false,
                                DataType = "List",
                                DataTypeId = null,
                                Desc = "Select the BU",
                                enable = true,
                                Id = MW_RFU.MwPort.MwBUId,
                                Key = "Select the BU",
                                Label = "Select the BU",
                                Manage = false,
                                Required = false,
                                Value = MW_RFU.MwPort.MwBU.Name
                            });
                            MWInstallationInfo.Add(new BaseInstAttView
                            {
                                AutoFill = false,
                                DataType = "List",
                                DataTypeId = null,
                                Desc = "Select the port of BU",
                                enable = true,
                                Id = MW_RFU.MwPortId.Value,
                                Key = "Select the port of BU",
                                Label = "Select the port of BU",
                                Manage = false,
                                Required = false,
                                Value = MW_RFU.MwPort.Port_Name
                            });
                        }
                        else if (AllCivilInst.civilWithoutLegId != null)
                        {
                            MWInstallationInfo.Add(new BaseInstAttView
                            {
                                AutoFill = false,
                                DataType = "List",
                                DataTypeId = null,
                                Desc = "Select civil support type",
                                enable = true,
                                Id = -1,
                                Key = "Select civil support type",
                                Label = "Select civil support type",
                                Manage = false,
                                Required = false,
                                Value = "Civil without Legs"
                            });
                            MWInstallationInfo.Add(new BaseInstAttView
                            {
                                AutoFill = false,
                                DataType = "List",
                                DataTypeId = null,
                                Desc = "Select civil without legs support items",
                                enable = true,
                                Id = AllCivilInst.Id,
                                Key = "Select civil without legs support items",
                                Label = "Select civil without legs support items",
                                Manage = false,
                                Required = false,
                                Value = _unitOfWork.CivilWithoutLegRepository.GetByID(AllCivilInst.civilWithoutLegId.Value).Name
                            });
                            MWInstallationInfo.Add(new BaseInstAttView
                            {
                                AutoFill = false,
                                DataType = "List",
                                DataTypeId = null,
                                Desc = "Select the BU",
                                enable = true,
                                Id = MW_RFU.MwPort.MwBUId,
                                Key = "Select the BU",
                                Label = "Select the BU",
                                Manage = false,
                                Required = false,
                                Value = MW_RFU.MwPort.MwBU.Name
                            });
                            MWInstallationInfo.Add(new BaseInstAttView
                            {
                                AutoFill = false,
                                DataType = "List",
                                DataTypeId = null,
                                Desc = "Select the port of BU",
                                enable = true,
                                Id = MW_RFU.MwPortId.Value,
                                Key = "Select the port of BU",
                                Label = "Select the port of BU",
                                Manage = false,
                                Required = false,
                                Value = MW_RFU.MwPort.Port_Name
                            });
                        }
                        else if (AllCivilInst.civilNonSteelId != null)
                        {
                            MWInstallationInfo.Add(new BaseInstAttView
                            {
                                AutoFill = false,
                                DataType = "List",
                                DataTypeId = null,
                                Desc = "Select civil support type",
                                enable = true,
                                Id = -1,
                                Key = "Select civil support type",
                                Label = "Select civil support type",
                                Manage = false,
                                Required = false,
                                Value = "Civil non steel"
                            });
                            MWInstallationInfo.Add(new BaseInstAttView
                            {
                                AutoFill = false,
                                DataType = "List",
                                DataTypeId = null,
                                Desc = "Select civil without legs support items",
                                enable = true,
                                Id = AllCivilInst.Id,
                                Key = "Select civil without legs support items",
                                Label = "Select civil without legs support items",
                                Manage = false,
                                Required = false,
                                Value = _unitOfWork.CivilNonSteelRepository.GetByID(AllCivilInst.civilNonSteelId.Value).Name
                            });
                            MWInstallationInfo.Add(new BaseInstAttView
                            {
                                AutoFill = false,
                                DataType = "List",
                                DataTypeId = null,
                                Desc = "Select the BU",
                                enable = true,
                                Id = MW_RFU.MwPort.MwBUId,
                                Key = "Select the BU",
                                Label = "Select the BU",
                                Manage = false,
                                Required = false,
                                Value = MW_RFU.MwPort.MwBU.Name
                            });
                            MWInstallationInfo.Add(new BaseInstAttView
                            {
                                AutoFill = false,
                                DataType = "List",
                                DataTypeId = null,
                                Desc = "Select the port of BU",
                                enable = true,
                                Id = MW_RFU.MwPortId.Value,
                                Key = "Select the port of BU",
                                Label = "Select the port of BU",
                                Manage = false,
                                Required = false,
                                Value = MW_RFU.MwPort.Port_Name
                            });
                        }
                        MWInstallationInfo.Add(new BaseInstAttView
                        {
                            AutoFill = false,
                            DataType = "List",
                            DataTypeId = null,
                            Desc = "allLoadInstId",
                            enable = false,
                            Id = AllLoadInst.Id,
                            Key = "allLoadInstId",
                            Label = "allLoadInstId",
                            Manage = false,
                            Required = false,
                            Value = AllLoadInst.Id
                        });

                        objectInst.SideArmInstallationInfo = MWInstallationInfo;

                    }
                }
                else if (LoadSubType.TLImwOther.ToString() == TableName)
                {
                    TLImwOther mwOther = _unitOfWork.Mw_OtherRepository
                        .GetIncludeWhereFirst(x => x.Id == MWInsId, x => x.mwOtherLibrary);

                    MW_OtherLibraryViewModel MwOtherLibrary = _mapper.Map<MW_OtherLibraryViewModel>(_unitOfWork.MW_OtherLibraryRepository
                        .GetIncludeWhereFirst(x => x.Id == mwOther.mwOtherLibraryId));

                    List<BaseAttView> LibraryAttributes = _unitOfWork.AttributeActivatedRepository
                       .GetAttributeActivated(TablesNames.TLImwOtherLibrary.ToString(), MwOtherLibrary, null).ToList();

                    foreach (BaseAttView LibraryAttribute in LibraryAttributes)
                    {
                        if (LibraryAttribute.DataType.ToLower() == "list")
                        {
                            LibraryAttribute.Value = MwOtherLibrary.GetType().GetProperties()
                                .FirstOrDefault(x => x.Name.ToLower() == LibraryAttribute.Label.ToLower()).GetValue(MwOtherLibrary);
                        }
                    }
                    List<BaseAttView> LogisticalAttributes = _mapper.Map<List<BaseAttView>>(_unitOfWork.LogistcalRepository
                       .GetLogistical(TablePartName.MW.ToString(), TablesNames.TLImwOtherLibrary.ToString(), MwOtherLibrary.Id).ToList());

                    LibraryAttributes.AddRange(LogisticalAttributes);
                    objectInst.LibraryActivatedAttributes = LibraryAttributes;

                    List<BaseInstAttView> ListAttributesActivated = _unitOfWork.AttributeActivatedRepository
                        .GetInstAttributeActivated(LoadSubType.TLImwOther.ToString(), mwOther, "InstallationPlaceId").ToList();

                    BaseInstAttView NameAttribute = ListAttributesActivated.FirstOrDefault(x => x.Key.ToLower() == "Name".ToLower());
                    if (NameAttribute != null)
                    {
                        BaseInstAttView Swap = ListAttributesActivated[0];
                        ListAttributesActivated[ListAttributesActivated.IndexOf(NameAttribute)] = Swap;
                        ListAttributesActivated[0] = NameAttribute;
                    }
                    foreach (BaseInstAttView FKitem in ListAttributesActivated)
                    {
                        if (FKitem.Desc.ToLower() == "tlimwotherlibrary")
                        {
                            if (mwOther.mwOtherLibrary == null)
                                FKitem.Value = _unitOfWork.MW_OtherLibraryRepository.GetWhereFirst(x => x.Id == 0).Model;

                            else
                                FKitem.Value = _unitOfWork.MW_OtherLibraryRepository.GetWhereFirst(x => x.Id == mwOther.mwOtherLibrary.Id && !x.Deleted && x.Active == true).Model;
                        }
                    }
                    objectInst.AttributesActivated = ListAttributesActivated;

                    objectInst.DynamicAtts = _unitOfWork.DynamicAttInstValueRepository
                       .GetDynamicInstAtts(TableNameEntity.Id, MWInsId, null);

                    AllLoadInst = _unitOfWork.AllLoadInstRepository.GetWhereFirst(x => x.mwOtherId == MWInsId);

                    CivilLoads = _unitOfWork.CivilLoadsRepository
                        .GetIncludeWhereFirst(x => x.allLoadInstId == AllLoadInst.Id, x => x.sideArm, x => x.site, x => x.leg, x => x.allCivilInst,
                            x => x.allLoadInst.loadOther, x => x.allLoadInst.mwBU, x => x.allLoadInst.mwDish, x => x.allLoadInst.mwODU, x => x.allLoadInst.mwOther,
                            x => x.allLoadInst.mwRFU, x => x.allLoadInst.power, x => x.allLoadInst.radioAntenna, x => x.allLoadInst.radioOther, x => x.allLoadInst.radioRRU,
                            x => x.allCivilInst.civilWithLegs, x => x.allCivilInst.civilWithoutLeg, x => x.allCivilInst.civilNonSteel, x => x.civilSteelSupportCategory);

                    LoadInstAttributes = _unitOfWork.AttributeActivatedRepository
                        .GetAttributeActivated(TablesNames.TLIcivilLoads.ToString(), CivilLoads, null, "allLoadInstId",
                            "Dismantle", "SiteCode", "civilSteelSupportCategoryId", "legId", "Leg2Id",
                                "sideArmId", "allCivilInstId").ToList();

                    List<KeyValuePair<string, List<DropDownListFilters>>> CivilLoadsRelatedTables = _unitOfWork.CivilLoadsRepository
                        .GetRelatedTables(CivilLoads.SiteCode);
                    if (CivilLoads != null)
                    {
                        List<KeyValuePair<string, List<DropDownListFilters>>> mwotherRelatedTables = _unitOfWork.Mw_OtherRepository
                            .GetRelatedTables();
                        mwotherRelatedTables.AddRange(CivilLoadsRelatedTables);

                        if (CivilLoads.allCivilInst.civilWithLegsId != null)
                        {
                            List<TLIleg> LegsForCivilWithLegLibrary = _unitOfWork.LegRepository
                                .GetWhere(x => x.CivilWithLegInstId == CivilLoads.allCivilInst.civilWithLegsId).ToList();

                            List<DropDownListFilters> LegIds = _mapper.Map<List<DropDownListFilters>>(LegsForCivilWithLegLibrary);

                            mwotherRelatedTables.Add(new KeyValuePair<string, List<DropDownListFilters>>("Leg1Id", LegIds));

                            List<TLIleg> Legs2ForCivilWithLegLibrary = LegsForCivilWithLegLibrary.Except(LegsForCivilWithLegLibrary
                                .Where(x => x.Id == CivilLoads.legId)).ToList();

                            List<DropDownListFilters> Leg2Ids = _mapper.Map<List<DropDownListFilters>>(Legs2ForCivilWithLegLibrary);

                            mwotherRelatedTables.Add(new KeyValuePair<string, List<DropDownListFilters>>("Leg2Id", Leg2Ids));
                        }

                        objectInst.RelatedTables = mwotherRelatedTables;

                        AllCivilInst = _unitOfWork.CivilLoadsRepository
                           .GetIncludeWhereFirst(x => (x.allLoadInstId != null ? (x.allLoadInst.mwOtherId != null ?
                               x.allLoadInst.mwOtherId.Value == MWInsId : false) : false) && !x.Dismantle, x => x.allCivilInst, x => x.allLoadInst).allCivilInst;

                        if (AllCivilInst.civilWithLegsId != null)
                        {
                            MWInstallationInfo.Add(new BaseInstAttView
                            {
                                AutoFill = false,
                                DataType = "List",
                                DataTypeId = null,
                                Desc = "Select civil support type",
                                enable = true,
                                Id = -1,
                                Key = "Select civil support type",
                                Label = "Select civil support type",
                                Manage = false,
                                Required = false,
                                Value = "Civil with legs"
                            });
                            MWInstallationInfo.Add(new BaseInstAttView
                            {
                                AutoFill = false,
                                DataType = "List",
                                DataTypeId = null,
                                Desc = "Select civil with legs support items",
                                enable = true,
                                Id = AllCivilInst.Id,
                                Key = "Select civil with legs support items",
                                Label = "Select civil with legs support items",
                                Manage = false,
                                Required = false,
                                Value = _unitOfWork.CivilWithLegsRepository.GetByID(AllCivilInst.civilWithLegsId.Value).Name
                            });

                            if (CivilLoads.sideArmId != null)
                            {
                                MWInstallationInfo.Add(new BaseInstAttView
                                {
                                    AutoFill = false,
                                    DataType = "List",
                                    DataTypeId = null,
                                    Desc = "Select the installation place type",
                                    enable = true,
                                    Id = -1,
                                    Key = "Select the installation place type",
                                    Label = "Select the installation place type",
                                    Manage = false,
                                    Required = false,
                                    Value = "SideArm"
                                });
                                MWInstallationInfo.Add(new BaseInstAttView
                                {
                                    AutoFill = false,
                                    DataType = "List",
                                    DataTypeId = null,
                                    Desc = "Select the sidearm",
                                    enable = true,
                                    Id = CivilLoads.sideArmId.Value,
                                    Key = "Select the sidearm",
                                    Label = "Select the sidearm",
                                    Manage = false,
                                    Required = false,
                                    Value = CivilLoads.sideArm.Name
                                });
                            }
                            else if (CivilLoads.legId != null)
                            {
                                MWInstallationInfo.Add(new BaseInstAttView
                                {
                                    AutoFill = false,
                                    DataType = "List",
                                    DataTypeId = null,
                                    Desc = "Select the installation place type",
                                    enable = true,
                                    Id = -1,
                                    Key = "Select the installation place type",
                                    Label = "Select the installation place type",
                                    Manage = false,
                                    Required = false,
                                    Value = "Leg"
                                });
                                MWInstallationInfo.Add(new BaseInstAttView
                                {
                                    AutoFill = false,
                                    DataType = "List",
                                    DataTypeId = null,
                                    Desc = "Select the leg",
                                    enable = true,
                                    Id = CivilLoads.legId.Value,
                                    Key = "Select the leg",
                                    Label = "Select the leg",
                                    Manage = false,
                                    Required = false,
                                    Value = CivilLoads.leg.CiviLegName
                                });
                            }
                            else
                            {
                                MWInstallationInfo.Add(new BaseInstAttView
                                {
                                    AutoFill = false,
                                    DataType = "List",
                                    DataTypeId = null,
                                    Desc = "Select the installation place type",
                                    enable = true,
                                    Id = -1,
                                    Key = "Select the installation place type",
                                    Label = "Select the installation place type",
                                    Manage = false,
                                    Required = false,
                                    Value = "Direct"
                                });
                            }
                        }
                        else if (AllCivilInst.civilWithoutLegId != null)
                        {
                            MWInstallationInfo.Add(new BaseInstAttView
                            {
                                AutoFill = false,
                                DataType = "List",
                                DataTypeId = null,
                                Desc = "Select civil support type",
                                enable = true,
                                Id = -1,
                                Key = "Select civil support type",
                                Label = "Select civil support type",
                                Manage = false,
                                Required = false,
                                Value = "Civil without legs"
                            });
                            MWInstallationInfo.Add(new BaseInstAttView
                            {
                                AutoFill = false,
                                DataType = "List",
                                DataTypeId = null,
                                Desc = "Select civil without legs support items",
                                enable = true,
                                Id = AllCivilInst.Id,
                                Key = "Select civil without legs support items",
                                Label = "Select civil without legs support items",
                                Manage = false,
                                Required = false,
                                Value = _unitOfWork.CivilWithoutLegRepository.GetByID(AllCivilInst.civilWithoutLegId.Value).Name
                            });

                            if (CivilLoads.sideArmId != null)
                            {
                                MWInstallationInfo.Add(new BaseInstAttView
                                {
                                    AutoFill = false,
                                    DataType = "List",
                                    DataTypeId = null,
                                    Desc = "Select the installation place type",
                                    enable = true,
                                    Id = -1,
                                    Key = "Select the installation place type",
                                    Label = "Select the installation place type",
                                    Manage = false,
                                    Required = false,
                                    Value = "SideArm"
                                });
                                MWInstallationInfo.Add(new BaseInstAttView
                                {
                                    AutoFill = false,
                                    DataType = "List",
                                    DataTypeId = null,
                                    Desc = "Select the sidearm",
                                    enable = true,
                                    Id = CivilLoads.sideArmId.Value,
                                    Key = "Select the sidearm",
                                    Label = "Select the sidearm",
                                    Manage = false,
                                    Required = false,
                                    Value = CivilLoads.sideArm.Name
                                });
                            }
                            else
                            {
                                MWInstallationInfo.Add(new BaseInstAttView
                                {
                                    AutoFill = false,
                                    DataType = "List",
                                    DataTypeId = null,
                                    Desc = "Select the installation place type",
                                    enable = true,
                                    Id = -1,
                                    Key = "Select the installation place type",
                                    Label = "Select the installation place type",
                                    Manage = false,
                                    Required = false,
                                    Value = "Direct"
                                });
                            }
                        }
                        else if (AllCivilInst.civilNonSteelId != null)
                        {
                            MWInstallationInfo.Add(new BaseInstAttView
                            {
                                AutoFill = false,
                                DataType = "List",
                                DataTypeId = null,
                                Desc = "Select civil support type",
                                enable = true,
                                Id = -1,
                                Key = "Select civil support type",
                                Label = "Select civil support type",
                                Manage = false,
                                Required = false,
                                Value = "Civil non steel"
                            });
                            MWInstallationInfo.Add(new BaseInstAttView
                            {
                                AutoFill = false,
                                DataType = "List",
                                DataTypeId = null,
                                Desc = "Select civil non steel support items",
                                enable = true,
                                Id = AllCivilInst.Id,
                                Key = "Select civil non steel support items",
                                Label = "Select civil non steel support items",
                                Manage = false,
                                Required = false,
                                Value = _unitOfWork.CivilNonSteelRepository.GetByID(AllCivilInst.civilNonSteelId.Value).Name
                            });

                            if (CivilLoads.sideArmId != null)
                            {
                                MWInstallationInfo.Add(new BaseInstAttView
                                {
                                    AutoFill = false,
                                    DataType = "List",
                                    DataTypeId = null,
                                    Desc = "Select the installation place type",
                                    enable = true,
                                    Id = -1,
                                    Key = "Select the installation place type",
                                    Label = "Select the installation place type",
                                    Manage = false,
                                    Required = false,
                                    Value = "SideArm"
                                });
                                MWInstallationInfo.Add(new BaseInstAttView
                                {
                                    AutoFill = false,
                                    DataType = "List",
                                    DataTypeId = null,
                                    Desc = "Select the sidearm",
                                    enable = true,
                                    Id = CivilLoads.sideArmId.Value,
                                    Key = "Select the sidearm",
                                    Label = "Select the sidearm",
                                    Manage = false,
                                    Required = false,
                                    Value = CivilLoads.sideArm.Name
                                });
                            }
                            else
                            {
                                MWInstallationInfo.Add(new BaseInstAttView
                                {
                                    AutoFill = false,
                                    DataType = "List",
                                    DataTypeId = null,
                                    Desc = "Select the installation place type",
                                    enable = true,
                                    Id = -1,
                                    Key = "Select the installation place type",
                                    Label = "Select the installation place type",
                                    Manage = false,
                                    Required = false,
                                    Value = "Direct"
                                });
                            }
                        }

                        MWInstallationInfo.Add(new BaseInstAttView
                        {
                            AutoFill = false,
                            DataType = "List",
                            DataTypeId = null,
                            Desc = "allLoadInstId",
                            enable = false,
                            Id = AllLoadInst.Id,
                            Key = "allLoadInstId",
                            Label = "allLoadInstId",
                            Manage = false,
                            Required = false,
                            Value = AllLoadInst.Id
                        });
                        objectInst.SideArmInstallationInfo = MWInstallationInfo;
                    }
                }
                if (CivilLoads != null)
                {
                    foreach (BaseAttView FKitem in LoadInstAttributes)
                    {
                        if (FKitem.Desc.ToLower() == "tlisidearm")
                        {
                            if (CivilLoads.sideArm == null)
                                FKitem.Value = "NA";

                            else
                                FKitem.Value = CivilLoads.sideArm.Name;
                        }

                        else if (FKitem.Desc.ToLower() == "tlileg")
                        {
                            if (CivilLoads.leg == null)
                                FKitem.Value = _unitOfWork.LegRepository.GetWhereFirst(x => x.Id == 0).CiviLegName;

                            else
                                FKitem.Value = CivilLoads.leg.CiviLegName;
                        }

                        if (FKitem.Desc.ToLower() == "tlicivilsteelsupportcategory")
                        {
                            if (CivilLoads.civilSteelSupportCategory == null)
                                FKitem.Value = _unitOfWork.CivilSteelSupportCategoryRepository.GetWhereFirst(x => x.Id == 0).Name;

                            else
                                FKitem.Value = CivilLoads.civilSteelSupportCategory.Name;
                        }
                        else if (FKitem.Desc.ToLower() == "tlisite")
                        {
                            FKitem.Value = CivilLoads.site.SiteName;
                        }

                        else if (FKitem.Desc.ToLower() == "tliallcivilinst")
                        {
                            if (CivilLoads.allCivilInst.civilWithLegsId != null)
                            {
                                FKitem.Value = CivilLoads.allCivilInst.civilWithLegs.Name;
                            }
                            else if (CivilLoads.allCivilInst.civilWithoutLegId != null)
                            {
                                FKitem.Value = CivilLoads.allCivilInst.civilWithoutLeg.Name;
                            }
                            else if (CivilLoads.allCivilInst.civilNonSteelId != null)
                            {
                                FKitem.Value = CivilLoads.allCivilInst.civilNonSteel.Name;
                            }
                        }
                        else if (FKitem.Desc.ToLower() == "tliallloadinst")
                        {
                            if (CivilLoads.allLoadInst.power != null)
                            {
                                FKitem.Value = CivilLoads.allLoadInst.power.Name;
                            }
                            else if (CivilLoads.allLoadInst.loadOther != null)
                            {
                                FKitem.Value = CivilLoads.allLoadInst.loadOther.Name;
                            }
                            else if (CivilLoads.allLoadInst.mwBU != null)
                            {
                                FKitem.Value = CivilLoads.allLoadInst.mwBU.Name;
                            }
                            else if (CivilLoads.allLoadInst.mwDish != null)
                            {
                                FKitem.Value = CivilLoads.allLoadInst.mwDish.DishName;
                            }
                            else if (CivilLoads.allLoadInst.mwODU != null)
                            {
                                FKitem.Value = CivilLoads.allLoadInst.mwODU.Name;
                            }
                            else if (CivilLoads.allLoadInst.mwOther != null)
                            {
                                FKitem.Value = CivilLoads.allLoadInst.mwOther.Name;
                            }
                            else if (CivilLoads.allLoadInst.mwRFU != null)
                            {
                                FKitem.Value = CivilLoads.allLoadInst.mwRFU.Name;
                            }
                            else if (CivilLoads.allLoadInst.radioAntenna != null)
                            {
                                FKitem.Value = CivilLoads.allLoadInst.radioAntenna.Name;
                            }
                            else if (CivilLoads.allLoadInst.radioRRU != null)
                            {
                                FKitem.Value = CivilLoads.allLoadInst.radioRRU.Name;
                            }
                            else if (CivilLoads.allLoadInst.radioOther != null)
                            {
                                FKitem.Value = CivilLoads.allLoadInst.radioOther.Name;
                            }
                        }
                    }

                    objectInst.CivilLoads = _mapper.Map<IEnumerable<BaseInstAttView>>(LoadInstAttributes);
                }

                return new Response<ObjectInstAttsForSideArm>(true, objectInst, null, null, (int)ApiReturnCode.success);
            }
            catch (Exception err)
            {
                return new Response<ObjectInstAttsForSideArm>(true, null, null, err.Message, (int)ApiReturnCode.fail);
            }
        }
        #region AddHistory
        //public void AddHistory(TicketAttributes ticketAtt, int allLoadInstId, string historyType)
        //{
        //    if (ticketAtt != null)
        //    {

        //        AddWorkflowHistoryViewModel workflowhistory = _mapper.Map<AddWorkflowHistoryViewModel>(ticketAtt);
        //        workflowhistory.RecordId = allLoadInstId;
        //        workflowhistory.TablesNameId = _unitOfWork.TablesNamesRepository.GetWhereFirst(x => x.TableName == "TLIallLoadInst").Id;
        //        workflowhistory.HistoryTypeId = _unitOfWork.HistoryTypeRepository.GetWhereSelectFirst(x => x.Name == historyType, x => new { x.Id }).Id;
        //        workflowhistory.UserId = 83;
        //        _unitOfWork.WorkflowHistoryRepository.AddWorkflowHistory(workflowhistory);
        //    }
        //    else
        //    {
        //        AddTablesHistoryViewModel history = new AddTablesHistoryViewModel();
        //        history.RecordId = allLoadInstId;
        //        history.TablesNameId = _unitOfWork.TablesNamesRepository.GetWhereFirst(x => x.TableName == "TLIallLoadInst").Id;
        //        history.HistoryTypeId = _unitOfWork.HistoryTypeRepository.GetWhereSelectFirst(x => x.Name == historyType, x => new { x.Id }).Id;
        //        history.UserId = 83;
        //        _unitOfWork.TablesHistoryRepository.AddTableHistory(history);
        //    }
        //}
        #endregion

        public Response<List<InstallationPlaceViewModel>> GetInstallationPlaces(string TableName, string LoadType)
        {
            try
            {
                List<InstallationPlaceViewModel> InstallationPlaces = _mapper.Map<List<InstallationPlaceViewModel>>(_unitOfWork.InstallationPlaceRepository
                    .GetWhere(x => x.Name != "NA" && x.Id != 0 && !x.Deleted && x.Disable == false).ToList());

                if (TableName.ToLower() == TablesNames.TLIcivilWithoutLeg.ToString().ToLower() ||
                    TableName.ToLower() == TablesNames.TLIcivilNonSteel.ToString().ToLower())
                {
                    InstallationPlaceViewModel LegInstallationPlace = InstallationPlaces.FirstOrDefault(x => x.Name.ToLower() == "Leg".ToLower());

                    if (LegInstallationPlace != null)
                        InstallationPlaces.Remove(LegInstallationPlace);
                }
                if (!string.IsNullOrEmpty(LoadType) ? LoadType.ToLower() == TablesNames.TLIpower.ToString().ToLower() : false)
                {
                    InstallationPlaceViewModel LegInstallationPlace = InstallationPlaces.FirstOrDefault(x => x.Name.ToLower() == "Direct".ToLower());

                    if (LegInstallationPlace != null)
                        InstallationPlaces.Remove(LegInstallationPlace);
                }

                return new Response<List<InstallationPlaceViewModel>>(true, InstallationPlaces, null, null, (int)ApiReturnCode.success);
            }
            catch (Exception err)
            {
                return new Response<List<InstallationPlaceViewModel>>(true, null, null, err.Message, (int)ApiReturnCode.fail);
            }
        }
        public Response<List<InstallationPlaceViewModel>> GetInstallationType(string TableName)
        {
            try
            {
                List<InstallationPlaceViewModel> InstallationPlaces = _mapper.Map<List<InstallationPlaceViewModel>>(_unitOfWork.OduInstallationTypeRepository
                    .GetWhere(x => x.Name != "NA" && x.Id != 0 && !x.Deleted && !x.Disable).ToList());

                if (TableName.ToLower() == TablesNames.TLIcivilWithoutLeg.ToString().ToLower() ||
                    TableName.ToLower() == TablesNames.TLIcivilNonSteel.ToString().ToLower())
                {
                    InstallationPlaceViewModel LegInstallationPlace = InstallationPlaces.FirstOrDefault(x => x.Name.ToLower() == "Leg".ToLower());

                    if (LegInstallationPlace != null)
                        InstallationPlaces.Remove(LegInstallationPlace);
                }

                return new Response<List<InstallationPlaceViewModel>>(true, InstallationPlaces, null, null, (int)ApiReturnCode.success);
            }
            catch (Exception err)
            {
                return new Response<List<InstallationPlaceViewModel>>(true, null, null, err.Message, (int)ApiReturnCode.fail);
            }
        }
        public Response<List<MW_PortViewModel>> GetMW_PortsForMW_RFUInstallation(int AllCivilInstId)
        {
            try
            {
                // installed BUs on the selected civil..
                List<int> MW_BUInstallationIds = _unitOfWork.CivilLoadsRepository.GetIncludeWhere(x => x.allCivilInstId == AllCivilInstId &&
                    (x.allLoadInstId != null ? x.allLoadInst.mwBUId != null : false), x => x.allLoadInst).Select(x => x.allLoadInst.mwBUId.Value).ToList();

                List<MW_PortViewModel> MW_PortsOnCivil = _mapper.Map<List<MW_PortViewModel>>(_unitOfWork.MW_PortRepository
                    .GetWhere(x => MW_BUInstallationIds.Contains(x.MwBUId)).ToList());



                return new Response<List<MW_PortViewModel>>(true, MW_PortsOnCivil, null, null, (int)ApiReturnCode.success);
            }
            catch (Exception err)
            {
                return new Response<List<MW_PortViewModel>>(true, null, null, err.Message, (int)ApiReturnCode.fail);
            }
        }
        public Response<List<MW_BULibraryViewModel>> GetMW_BULibrariesForMW_BUInstallation()
        {
            try
            {
                // Get All The MW_BU Libraries That've Been Used As A Port..
                List<int> MW_BULibraryIds = _unitOfWork.MW_PortRepository.GetAllWithoutCount().Select(x => x.MwBULibraryId).ToList();

                // Filter The MW_BU Libraries Depending On The Used Libraries in MW_Port Table..
                List<MW_BULibraryViewModel> MW_BULibraries = _mapper.Map<List<MW_BULibraryViewModel>>(_unitOfWork.MW_BULibraryRepository
                    .GetWhere(x => !x.Deleted && x.Active && !MW_BULibraryIds.Contains(x.Id)).ToList());

                return new Response<List<MW_BULibraryViewModel>>(true, MW_BULibraries, null, null, (int)ApiReturnCode.success);
            }
            catch (Exception err)
            {
                return new Response<List<MW_BULibraryViewModel>>(true, null, null, err.Message, (int)ApiReturnCode.fail);
            }
        }


        public Response<List<MW_Free_BUInstDto>> GetMw_Free_BuInst(int AllCivilInstId)
        {
            try
            {
                var CiviBUs = _dbContext.TLIcivilLoads.Include(x => x.allLoadInst)
                    .ThenInclude(x => x.mwBU).Where(x => x.allCivilInstId == AllCivilInstId && x.allLoadInst.mwBUId != null && !x.Dismantle &&
                        !x.allLoadInst.Draft).Select(x => x.allLoadInst.mwBUId).ToList();

                if (CiviBUs == null || CiviBUs.Count == 0)
                {
                    return new Response<List<MW_Free_BUInstDto>>(false, null, null, "Not found any BU on this civil", (int)ApiReturnCode.success);
                }

                var AllPorts = _dbContext.TLImwPort.Where(x => CiviBUs.Contains(x.MwBUId)).Select(x => x.Id).ToList();

                var UsedPortsOnRFu = _dbContext.TLImwRFU.Include(x => x.MwRFULibrary).Where(x => !x.MwRFULibrary.Deleted && x.MwRFULibrary.Active)
                    .Select(x => x.MwPortId).Distinct().ToList();

                var FreePorts = AllPorts.Where(p => UsedPortsOnRFu.All(p2 => p != p2)).ToList();

                var AvaBUs = _dbContext.TLImwPort.Include(x => x.MwBU).Where(x => FreePorts.Contains(x.Id))
                    .Select(x => x.MwBUId).Distinct().ToList();

                var BUs = _dbContext.TLImwBU.Where(x => AvaBUs.Contains(x.Id)).AsQueryable();

                var AvailableBus = BUs.Select(x => new MW_Free_BUInstDto()
                {
                    BuInstId = x.Id,
                    BuInstName = x.Name
                }).ToList();

                return new Response<List<MW_Free_BUInstDto>>(true, AvailableBus, null, null, (int)ApiReturnCode.success, AvailableBus.Count());
            }
            catch (Exception ex)
            {
                return new Response<List<MW_Free_BUInstDto>>(false, null, null, ex.Message, (int)ApiReturnCode.fail);

            }
        }



        public Response<List<MW_PortViewModel>> GetFreePortOnBU(int BUid)
        {
            try
            {
                var AllPorts = _dbContext.TLImwPort.Where(x => x.MwBUId == BUid).Select(x => x.Id).ToList();
                var AllUsedPorts = _dbContext.TLImwRFU.Include(x => x.MwRFULibrary).Where(x => x.MwRFULibrary.Active == true && x.MwRFULibrary.Deleted == false).Select(x => x.MwPortId).ToList();
                var FreePorts = AllPorts.Where(p => AllUsedPorts.All(p2 => p != p2)).ToList();

                var AvailablePorts = _dbContext.TLImwPort.Where(x => FreePorts.Contains(x.Id)).Select(x => new MW_PortViewModel
                {
                    Id = x.Id,
                    Name = x.Port_Name,
                    MwBUId = x.MwBUId,
                    TX_Frequency = x.TX_Frequency

                }).ToList();
                return new Response<List<MW_PortViewModel>>(true, AvailablePorts, null, null, (int)ApiReturnCode.success);
            }

            catch (Exception ex)
            {
                return new Response<List<MW_PortViewModel>>(false, null, null, ex.Message, (int)ApiReturnCode.fail);

            }
        }


        public Response<List<MW_Free_BUInstDto>> GetMw_Free_Cascade_BuInst(int AllCivilInstId)
        {
            try
            {

                List<int> CiviBUs = _dbContext.TLIcivilLoads.Include(x => x.allLoadInst).ThenInclude(x => x.mwBU)
                    .Where(x => x.allCivilInstId == AllCivilInstId && (x.allLoadInst != null ? x.allLoadInst.mwBUId != null : false))
                    .Select(x => x.allLoadInst.mwBUId.Value).ToList();

                if (CiviBUs == null ? true : CiviBUs.Count() == 0)
                    return new Response<List<MW_Free_BUInstDto>>(false, null, null, "Not found any BU on this civil", (int)ApiReturnCode.success);

                List<int> AllPorts = _dbContext.TLImwPort.Where(x => CiviBUs.Contains(x.MwBUId) && x.Port_Type == 2).Select(x => x.Id).ToList();

                List<int> UsedPortsOnBu = _dbContext.TLImwBU.Include(x => x.MwBULibrary)
                    .Where(x => !x.MwBULibrary.Deleted && x.MwBULibrary.Active && x.PortCascadeId != 0).Select(x => x.PortCascadeId).Distinct().ToList();

                List<int> FreePorts = AllPorts.Where(p => UsedPortsOnBu.All(p2 => p != p2)).ToList();

                List<int> AvaBUs = _dbContext.TLImwPort.Include(x => x.MwBU).Where(x => FreePorts.Contains(x.Id)).Select(x => x.MwBUId).Distinct().ToList();

                if (AvaBUs != null ? AvaBUs.Count() > 0 : false)
                {
                    int Fport = 0;
                    var BUs = _dbContext.TLImwBU.Where(x => AvaBUs.Contains(x.Id)).ToList();
                    List<MW_Free_BUInstDto> res = new List<MW_Free_BUInstDto>();
                    //MW_Free_BUInstDto
                    foreach (var s in BUs)
                    {
                        MW_Free_BUInstDto item = new MW_Free_BUInstDto();

                        int casPort = _dbContext.TLImwPort.FirstOrDefault(x => x.MwBUId == s.Id && x.Port_Type == 2).Id;
                        item.CascadePortId = casPort != null ? casPort : 0;
                        item.BuInstId = s.Id;
                        item.BuInstName = s.Name;
                        res.Add(item);

                    }


                    return new Response<List<MW_Free_BUInstDto>>(true, res, null, null, (int)ApiReturnCode.success);
                }
                else
                {
                    return new Response<List<MW_Free_BUInstDto>>(false, null, null, "Not Found any avilable Cascade BU", (int)ApiReturnCode.fail);
                }
            }
            catch (Exception ex)
            {
                return new Response<List<MW_Free_BUInstDto>>(false, null, null, ex.Message, (int)ApiReturnCode.fail);
            }
        }
        public Response<List<MW_DishGetForAddViewModel>> GetFreeDishesForMW_ODU(int AllCivilInstId)
        {
            try
            {
                List<int> UsedDishesIds = _unitOfWork.MW_ODURepository.GetWhere(x => x.Mw_DishId != null).Select(x => x.Mw_DishId.Value).ToList();

                List<MW_DishGetForAddViewModel> MW_Dishes = _mapper.Map<List<MW_DishGetForAddViewModel>>(_unitOfWork.CivilLoadsRepository
                    .GetIncludeWhere(x => !x.Dismantle && x.allCivilInstId == AllCivilInstId &&
                        (x.allLoadInstId != null ? x.allLoadInst.mwDishId != null : false) &&
                        !UsedDishesIds.Contains(x.allLoadInst.mwDishId.Value), x => x.allLoadInst, x => x.allLoadInst.mwDish)
                    .Select(x => x.allLoadInst.mwDish).ToList());

                return new Response<List<MW_DishGetForAddViewModel>>(true, MW_Dishes, null, null, (int)ApiReturnCode.success);
            }
            catch (Exception err)
            {
                return new Response<List<MW_DishGetForAddViewModel>>(false, null, null, err.Message, (int)ApiReturnCode.fail);
            }
        }
        public Response<ObjectInstAtts> GetAttForAddForMW_ODUOnly(string TableName, int LibraryID, string SiteCode, int AllCivilInstId)
        {
            try
            {
                TLItablesNames TableNameEntity = _unitOfWork.TablesNamesRepository.GetWhereFirst(x =>
                    x.TableName.ToLower() == TableName.ToLower());

                ObjectInstAtts objectInst = new ObjectInstAtts();
                List<BaseInstAttView> ListAttributesActivated = new List<BaseInstAttView>();

                MW_ODULibraryViewModel mwODULibrary = _mapper.Map<MW_ODULibraryViewModel>(_unitOfWork.MW_ODULibraryRepository
                    .GetIncludeWhereFirst(x => x.Id == LibraryID, x => x.parity));

                List<BaseAttView> LibraryAttributes = _unitOfWork.AttributeActivatedRepository
                    .GetAttributeActivated(TablesNames.TLImwODULibrary.ToString(), mwODULibrary, null).ToList();

                foreach (BaseAttView LibraryAttribute in LibraryAttributes)
                {
                    if (LibraryAttribute.DataType.ToLower() == "list")
                    {
                        LibraryAttribute.Value = mwODULibrary.GetType().GetProperties()
                            .FirstOrDefault(x => x.Name.ToLower() == LibraryAttribute.Label.ToLower()).GetValue(mwODULibrary);
                    }
                }

                List<BaseAttView> LogisticalAttributes = _mapper.Map<List<BaseAttView>>(_unitOfWork.LogistcalRepository
                    .GetLogistical(TablePartName.MW.ToString(), Helpers.Constants.TablesNames.TLImwODULibrary.ToString(), mwODULibrary.Id).ToList());

                LibraryAttributes.AddRange(LogisticalAttributes);

                objectInst.LibraryActivatedAttributes = LibraryAttributes;

                ListAttributesActivated = _unitOfWork.AttributeActivatedRepository.
                    GetInstAttributeActivated(LoadSubType.TLImwODU.ToString(), null, "Name", "MwODULibraryId", "OduInstallationTypeId", "Mw_DishId"/*, "EquivalentSpace"*/).ToList();

                BaseInstAttView NameAttribute = ListAttributesActivated.FirstOrDefault(x => x.Key.ToLower() == "Name".ToLower());
                if (NameAttribute != null)
                {
                    BaseInstAttView Swap = ListAttributesActivated[0];
                    ListAttributesActivated[ListAttributesActivated.IndexOf(NameAttribute)] = Swap;
                    ListAttributesActivated[0] = NameAttribute;
                }
                foreach (BaseInstAttView FKitem in ListAttributesActivated)
                {
                    if (FKitem.Desc.ToLower() == "tliowner")
                        FKitem.Value = _mapper.Map<List<OwnerViewModel>>(_unitOfWork.OwnerRepository.GetWhere(x => !x.Disable && !x.Deleted).ToList());

                    else if (FKitem.Desc.ToLower() == "tlimwdish")
                    {
                        List<int> UsedDishesIds = _unitOfWork.MW_ODURepository.GetWhere(x => x.Mw_DishId != null).Select(x => x.Mw_DishId.Value).ToList();

                        List<MW_DishGetForAddViewModel> MW_Dishes = _mapper.Map<List<MW_DishGetForAddViewModel>>(_unitOfWork.CivilLoadsRepository
                            .GetIncludeWhere(x => !x.Dismantle && x.allCivilInstId == AllCivilInstId &&
                                (x.allLoadInstId != null ? x.allLoadInst.mwDishId != null : false) &&
                                !UsedDishesIds.Contains(x.allLoadInst.mwDishId.Value), x => x.allLoadInst, x => x.allLoadInst.mwDish)
                            .Select(x => x.allLoadInst.mwDish).ToList());

                        FKitem.Value = _mapper.Map<List<MW_DishGetForAddViewModel>>(MW_Dishes);
                    }
                }

                List<KeyValuePair<string, List<DropDownListFilters>>> RelatedTables = _unitOfWork.CivilLoadsRepository.GetRelatedTables(SiteCode);
                objectInst.RelatedTables = RelatedTables;

                objectInst.AttributesActivated = ListAttributesActivated;

                objectInst.CivilLoads = _unitOfWork.AttributeActivatedRepository.
                    GetInstAttributeActivated(TablesNames.TLIcivilLoads.ToString(), null, "allLoadInstId", "Dismantle", "SiteCode", "legId", "Leg2Id",
                        "sideArmId", "allCivilInstId", "civilSteelSupportCategoryId");

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

                return new Response<ObjectInstAtts>(objectInst);
            }
            catch (Exception err)
            {
                return new Response<ObjectInstAtts>(true, null, null, err.Message, (int)ApiReturnCode.fail);
            }
        }
    }
}
