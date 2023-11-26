using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
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
using TLIS_DAL.ViewModels.AllCivilInstDTOs;
using TLIS_DAL.ViewModels.AllLoadInstDTOs;
using TLIS_DAL.ViewModels.AttActivatedCategoryDTOs;
using TLIS_DAL.ViewModels.AttributeActivatedDTOs;
using TLIS_DAL.ViewModels.AttributeViewManagmentDTOs;
using TLIS_DAL.ViewModels.BaseCivilWithLegsTypeDTOs;
using TLIS_DAL.ViewModels.BaseTypeDTOs;
using TLIS_DAL.ViewModels.CabinetDTOs;
using TLIS_DAL.ViewModels.CivilLoadsDTOs;
using TLIS_DAL.ViewModels.CivilNonSteelDTOs;
using TLIS_DAL.ViewModels.CivilWithLegDTOs;
using TLIS_DAL.ViewModels.CivilWithLegsDTOs;
using TLIS_DAL.ViewModels.CivilWithoutLegDTOs;
using TLIS_DAL.ViewModels.DataTypeDTOs;
using TLIS_DAL.ViewModels.DismantleDto;
using TLIS_DAL.ViewModels.DynamicAttDTOs;
using TLIS_DAL.ViewModels.DynamicAttInstValueDTOs;
using TLIS_DAL.ViewModels.DynamicAttLibValueDTOs;
using TLIS_DAL.ViewModels.EnforcmentCategoryDTOs;
using TLIS_DAL.ViewModels.GuyLineTypeDTOs;
using TLIS_DAL.ViewModels.LoadOtherDTOs;
using TLIS_DAL.ViewModels.LocationTypeDTOs;
using TLIS_DAL.ViewModels.LogicalOperationDTOs;
using TLIS_DAL.ViewModels.MW_BUDTOs;
using TLIS_DAL.ViewModels.MW_DishDTOs;
using TLIS_DAL.ViewModels.MW_ODUDTOs;
using TLIS_DAL.ViewModels.Mw_OtherDTOs;
using TLIS_DAL.ViewModels.MW_RFUDTOs;
using TLIS_DAL.ViewModels.OwnerDTOs;
using TLIS_DAL.ViewModels.PowerDTOs;
using TLIS_DAL.ViewModels.RadioAntennaDTOs;
using TLIS_DAL.ViewModels.RadioOtherDTOs;
using TLIS_DAL.ViewModels.RadioRRUDTOs;
using TLIS_DAL.ViewModels.RuleDTOs;
using TLIS_DAL.ViewModels.SideArmDTOs;
using TLIS_DAL.ViewModels.SubTypeDTOs;
using TLIS_DAL.ViewModels.SupportTypeImplementedDTOs;
using TLIS_DAL.ViewModels.TablesHistoryDTOs;
using TLIS_DAL.ViewModels.WorkflowHistoryDTOs;
using TLIS_Repository.Base;
using TLIS_Service.IService;
using static TLIS_Repository.Helpers.Constants;
using static TLIS_Service.Helpers.Constants;
using TablesNames = TLIS_Repository.Helpers.Constants.TablesNames;
using System.Runtime.CompilerServices;
using TLIS_DAL.ViewModels.LegDTOs;
using AutoMapper;
using TLIS_DAL.ViewModels.SideArmLibraryDTOs;
using Remotion.Utilities;

namespace TLIS_Service.Services
{
    public class CivilInstService : ICivilInstService
    {
        IUnitOfWork _unitOfWork;
        IServiceCollection _services;
        private readonly ApplicationDbContext _dbContext;
        private IMapper _mapper;
        public CivilInstService(IUnitOfWork unitOfWork, IServiceCollection services, ApplicationDbContext context,IMapper mapper)
        {
            _dbContext = context;
            _unitOfWork = unitOfWork;
            _services = services;
            _mapper = mapper;
        }
        public Response<LoadsOnSideArm> GetLoadsOnSideArm(int SideArmId)
        {
            try
            {
                LoadsOnSideArm OutPut = new LoadsOnSideArm();

                List<TLIcivilLoads> CivilLoads = _unitOfWork.CivilLoadsRepository.GetIncludeWhere(x=>!x.Dismantle && x.sideArmId == SideArmId && x.allLoadInstId != null, x => x.allLoadInst,
                    x => x.allLoadInst.mwBU, x => x.allLoadInst.loadOther, x => x.allLoadInst.mwDish, x => x.allLoadInst.mwODU, x => x.allLoadInst.mwOther,
                    x => x.allLoadInst.mwRFU, x => x.allLoadInst.power, x => x.allLoadInst.radioAntenna, x => x.allLoadInst.radioOther, x => x.allLoadInst.radioRRU).ToList();

                OutPut.MW_ODUs = _mapper.Map<List<MW_ODUViewModel>>(CivilLoads.Where(x => x.allLoadInst.mwODUId != null)
                    .Select(x => x.allLoadInst.mwODU).Distinct().ToList());

                OutPut.MW_Dishes = _mapper.Map<List<MW_DishViewModel>>(CivilLoads.Where(x => x.allLoadInst.mwDishId != null)
                    .Select(x => x.allLoadInst.mwDish).Distinct().ToList());

                OutPut.MW_RFUs = _mapper.Map<List<MW_RFUViewModel>>(CivilLoads.Where(x => x.allLoadInst.mwRFUId != null)
                    .Select(x => x.allLoadInst.mwRFU).Distinct().ToList());

                OutPut.MW_BUs = _mapper.Map<List<MW_BUViewModel>>(CivilLoads.Where(x => x.allLoadInst.mwBUId != null)
                    .Select(x => x.allLoadInst.mwBU).Distinct().ToList());

                OutPut.MW_Others = _mapper.Map<List<Mw_OtherViewModel>>(CivilLoads.Where(x => x.allLoadInst.mwOtherId != null)
                    .Select(x => x.allLoadInst.mwOther).Distinct().ToList());

                OutPut.RadioAntennas = _mapper.Map<List<RadioAntennaViewModel>>(CivilLoads.Where(x => x.allLoadInst.radioAntennaId != null)
                    .Select(x => x.allLoadInst.radioAntenna).Distinct().ToList());

                OutPut.RadioRRUs = _mapper.Map<List<RadioRRUViewModel>>(CivilLoads.Where(x => x.allLoadInst.radioRRUId != null)
                    .Select(x => x.allLoadInst.radioRRU).Distinct().ToList());

                OutPut.RadioOthers = _mapper.Map<List<RadioOtherViewModel>>(CivilLoads.Where(x => x.allLoadInst.radioOtherId != null)
                    .Select(x => x.allLoadInst.radioOther).Distinct().ToList());

                OutPut.Powers = _mapper.Map<List<PowerViewModel>>(CivilLoads.Where(x => x.allLoadInst.powerId != null)
                    .Select(x => x.allLoadInst.power).Distinct().ToList());

                OutPut.LoadOthers = _mapper.Map<List<LoadOtherViewModel>>(CivilLoads.Where(x => x.allLoadInst.loadOtherId != null)
                    .Select(x => x.allLoadInst.loadOther).Distinct().ToList());

                return new Response<LoadsOnSideArm>(true, OutPut, null, null, (int)Helpers.Constants.ApiReturnCode.success);
            }
            catch (Exception err)
            {
                return new Response<LoadsOnSideArm>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
        }
        public Response<CivilLoads> GetLoadsAndSideArmsForCivil(int CivilId, string CivilType)
        {
            try
            {
                CivilLoads OutPut = new CivilLoads();
                TLIallCivilInst AllCivilInst = new TLIallCivilInst();

                if (CivilType.ToLower() == Helpers.Constants.TablesNames.TLIcivilWithLegs.ToString().ToLower())
                {
                    AllCivilInst = _unitOfWork.AllCivilInstRepository.GetWhereFirst(x => x.civilWithLegsId == CivilId);
                }
                else if (CivilType.ToLower() == Helpers.Constants.TablesNames.TLIcivilWithoutLeg.ToString().ToLower())
                {
                    AllCivilInst = _unitOfWork.AllCivilInstRepository.GetWhereFirst(x => x.civilWithoutLegId == CivilId);
                }
                else if (CivilType.ToLower() == Helpers.Constants.TablesNames.TLIcivilNonSteel.ToString().ToLower())
                {
                    AllCivilInst = _unitOfWork.AllCivilInstRepository.GetWhereFirst(x => x.civilNonSteelId == CivilId);
                }

                List<TLIcivilLoads> CivilSiteDates = _unitOfWork.CivilLoadsRepository.GetIncludeWhere(x => x.allCivilInstId == AllCivilInst.Id && !x.Dismantle,
                    x => x.sideArm, x => x.allLoadInst, x => x.allLoadInst.mwBU, x => x.allLoadInst.mwDish, x => x.allLoadInst.mwODU, x => x.allLoadInst.mwOther, x => x.allLoadInst.mwRFU,
                    x => x.allLoadInst.power, x => x.allLoadInst.radioAntenna, x => x.allLoadInst.radioOther, x => x.allLoadInst.radioRRU, x => x.allLoadInst.loadOther).ToList();

                OutPut.SideArms = _mapper.Map<List<SideArmViewModel>>(CivilSiteDates.Where(x => x.sideArmId != null && x.allLoadInstId == null)
                    .Select(x => x.sideArm).Distinct().ToList());

                OutPut.MW_ODUs = _mapper.Map<List<MW_ODUViewModel>>(CivilSiteDates.Where(x => x.allLoadInstId != null ? x.allLoadInst.mwODUId != null : false)
                    .Select(x => x.allLoadInst.mwODU).Distinct().ToList());

                OutPut.MW_Dishes = _mapper.Map<List<MW_DishViewModel>>(CivilSiteDates.Where(x => x.allLoadInstId != null ? x.allLoadInst.mwDishId != null : false)
                    .Select(x => x.allLoadInst.mwDish).Distinct().ToList());

                OutPut.MW_RFUs = _mapper.Map<List<MW_RFUViewModel>>(CivilSiteDates.Where(x => x.allLoadInstId != null ? x.allLoadInst.mwRFUId != null : false)
                    .Select(x => x.allLoadInst.mwRFU).Distinct().ToList());

                OutPut.MW_BUs = _mapper.Map<List<MW_BUViewModel>>(CivilSiteDates.Where(x => x.allLoadInstId != null ? x.allLoadInst.mwBUId != null : false)
                    .Select(x => x.allLoadInst.mwBU).Distinct().ToList());

                OutPut.MW_Others = _mapper.Map<List<Mw_OtherViewModel>>(CivilSiteDates.Where(x => x.allLoadInstId != null ? x.allLoadInst.mwOtherId != null : false)
                    .Select(x => x.allLoadInst.mwOther).Distinct().ToList());

                OutPut.RadioAntennas = _mapper.Map<List<RadioAntennaViewModel>>(CivilSiteDates.Where(x => x.allLoadInstId != null ? x.allLoadInst.radioAntennaId != null : false)
                    .Select(x => x.allLoadInst.radioAntenna).Distinct().ToList());

                OutPut.RadioRRUs = _mapper.Map<List<RadioRRUViewModel>>(CivilSiteDates.Where(x => x.allLoadInstId != null ? x.allLoadInst.radioRRUId != null : false)
                    .Select(x => x.allLoadInst.radioRRU).Distinct().ToList());

                OutPut.RadioOthers = _mapper.Map<List<RadioOtherViewModel>>(CivilSiteDates.Where(x => x.allLoadInstId != null ? x.allLoadInst.radioOtherId != null : false)
                    .Select(x => x.allLoadInst.radioOther).Distinct().ToList());

                OutPut.Powers = _mapper.Map<List<PowerViewModel>>(CivilSiteDates.Where(x => x.allLoadInstId != null ? x.allLoadInst.powerId != null : false)
                    .Select(x => x.allLoadInst.power).Distinct().ToList());

                OutPut.LoadOthers = _mapper.Map<List<LoadOtherViewModel>>(CivilSiteDates.Where(x => x.allLoadInstId != null ? x.allLoadInst.loadOtherId != null : false)
                    .Select(x => x.allLoadInst.loadOther).Distinct().ToList());

                return new Response<CivilLoads>(true, OutPut, null, null, (int)Helpers.Constants.ApiReturnCode.success);
            }
            catch (Exception err)
            {
                return new Response<CivilLoads>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
        }

        // GetAttForAdd return data needed to add record of any type of civil depened on TableName
        //This Function take 
        //TableName to check type of civil and get Id of this TableName from TLItablesNames and get DynamicAtts depened on TableName Id
        //CivilLibraryId to return activatedatts depened civil type ex: if type is TLIcivilWithLegs the library is TLIcivilWithLegLibrary
        //CategoryId this prarmeter accept null because TLIcivilWithoutLeg has more than one category (Mast - Monopole) and depened on category we can get activatedatts and dynamic atts
        //Get All civils in that SiteCode
        //That Function return ObjectInstAtts object consist of List Library Activated Attributes with values, List Attributes Activated, List Dynamic Atts, List of related tables(foreign keys in specific table), List civils of related to specific site
        public Response<ObjectInst> GetAttForAdd(string TableName, int CivilLibraryId, int? CategoryId, string SiteCode)
        {
            try
            {
                TLItablesNames TableNameEntity = _unitOfWork.TablesNamesRepository
                    .GetWhereFirst(c => c.TableName.ToLower() == TableName.ToLower());

                ObjectInst objectInst = new ObjectInst();

                if (Helpers.Constants.CivilType.TLIcivilWithLegs.ToString() == TableName)
                {
                    CivilWithLegLibraryViewModel CivilWithLegLibrary = _mapper.Map<CivilWithLegLibraryViewModel>(_unitOfWork.CivilWithLegLibraryRepository
                        .GetIncludeWhereFirst(x => x.Id == CivilLibraryId, x => x.civilSteelSupportCategory, x => x.sectionsLegType,
                            x => x.structureType, x => x.supportTypeDesigned));

                    List<BaseAttView> LibraryAttributes = _unitOfWork.AttributeActivatedRepository
                        .GetAttributeActivated(Helpers.Constants.TablesNames.TLIcivilWithLegLibrary.ToString(), CivilWithLegLibrary, null).ToList();

                    foreach (BaseAttView LibraryAttribute in LibraryAttributes)
                    {
                        if (LibraryAttribute.DataType.ToLower() == "list")
                        {
                            LibraryAttribute.Value = CivilWithLegLibrary.GetType().GetProperties()
                                .FirstOrDefault(x => x.Name.ToLower() == LibraryAttribute.Label.ToLower()).GetValue(CivilWithLegLibrary);
                        }
                    }

                    List<BaseAttView> LibraryLogisticalAttributes = _mapper.Map<List<BaseAttView>>(_unitOfWork.LogistcalRepository
                        .GetLogistical(Helpers.Constants.TablePartName.CivilSupport.ToString(), Helpers.Constants.TablesNames.TLIcivilWithLegLibrary.ToString(), CivilWithLegLibrary.Id).ToList());

                    LibraryAttributes.AddRange(LibraryLogisticalAttributes);

                    objectInst.LibraryActivatedAttributes = LibraryAttributes;

                    List<BaseInstAttView> ListAttributesActivated = _unitOfWork.AttributeActivatedRepository
                        .GetInstAttributeActivated(TableName, null, "Name", "CivilWithLegsLibId", "CurrentLoads").ToList();

                    BaseInstAttView NameAttribute = ListAttributesActivated.FirstOrDefault(x => x.Key.ToLower() == "Name".ToLower());
                    if (NameAttribute != null)
                    {
                        BaseInstAttView Swap = ListAttributesActivated[0];
                        ListAttributesActivated[ListAttributesActivated.IndexOf(NameAttribute)] = Swap;
                        ListAttributesActivated[0] = NameAttribute;
                    }
                    foreach (BaseInstAttView FKitem in ListAttributesActivated)
                    {
                        if (FKitem.Desc.ToLower() == "tlilocationtype")
                            FKitem.Value = _mapper.Map<List<LocationTypeViewModel>>(_unitOfWork.LocationTypeRepository.GetWhere(x => !x.Deleted && !x.Disable).ToList());

                        else if (FKitem.Desc.ToLower() == "tlibasetype")
                            FKitem.Value = _mapper.Map<List<BaseTypeViewModel>>(_unitOfWork.BaseTypeRepository.GetWhere(x => !x.Deleted && !x.Disable).ToList());

                        else if (FKitem.Desc.ToLower() == "tliowner")
                            FKitem.Value = _mapper.Map<List<OwnerViewModel>>(_unitOfWork.OwnerRepository.GetWhere(x => !x.Deleted && !x.Disable).ToList());

                        else if (FKitem.Desc.ToLower() == "tlibasecivilwithlegstype")
                            FKitem.Value = _mapper.Map<List<BaseCivilWithLegsTypeViewModel>>(_unitOfWork.BaseCivilWithLegsTypeRepository.GetWhere(x => !x.Deleted && !x.Disable).ToList());

                        else if (FKitem.Desc.ToLower() == "tliguylinetype")
                            FKitem.Value = _mapper.Map<List<GuyLineTypeViewModel>>(_unitOfWork.GuyLineTypeRepository.GetWhere(x => !x.Deleted && !x.Disable).ToList());

                        else if (FKitem.Desc.ToLower() == "tlisupporttypeimplemented")
                            FKitem.Value = _mapper.Map<List<SupportTypeImplementedViewModel>>(_unitOfWork.SupportTypeImplementedRepository.GetWhere(x => !x.Deleted && !x.Disable).ToList());

                        else if (FKitem.Desc.ToLower() == "tlienforcmentcategory")
                            FKitem.Value = _mapper.Map<List<EnforcmentCategoryViewModel>>(_unitOfWork.EnforcmentCategoryRepository.GetWhere(x => !x.Deleted && !x.Disable).ToList());

                        else if (FKitem.Key.ToLower() == "StructureType".ToLower())
                        {
                            List<EnumOutPut> StructureTypeCompatibleWithDesigns = new List<EnumOutPut>();
                            StructureTypeCompatibleWithDesigns.Add(new EnumOutPut
                            {
                                Id = (int)StructureTypeCompatibleWithDesign.Yes,
                                Name = StructureTypeCompatibleWithDesign.Yes.ToString()
                            });
                            StructureTypeCompatibleWithDesigns.Add(new EnumOutPut
                            {
                                Id = (int)StructureTypeCompatibleWithDesign.No,
                                Name = StructureTypeCompatibleWithDesign.No.ToString()
                            });
                            FKitem.Value = StructureTypeCompatibleWithDesigns;
                        }
                        else if (FKitem.Key.ToLower() == "SectionsLegType".ToLower())
                        {
                            List<EnumOutPut> SectionsLegType = new List<EnumOutPut>();
                            SectionsLegType.Add(new EnumOutPut
                            {
                                Id = (int)SectionsLegTypeCompatibleWithDesign.Yes,
                                Name = SectionsLegTypeCompatibleWithDesign.Yes.ToString()
                            });
                            SectionsLegType.Add(new EnumOutPut
                            {
                                Id = (int)SectionsLegTypeCompatibleWithDesign.No,
                                Name = SectionsLegTypeCompatibleWithDesign.No.ToString()
                            });
                            FKitem.Value = SectionsLegType;
                        }
                    }

                    objectInst.AttributesActivated = ListAttributesActivated;

                    IEnumerable<DynaminAttInstViewModel> DynamicAttributesWithoutValue = _unitOfWork.DynamicAttRepository
                        .GetDynamicInstAtts(TableNameEntity.Id, CategoryId);

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

                    objectInst.CivilSiteDate = _unitOfWork.AttributeActivatedRepository
                        .GetInstAttributeActivated(Helpers.Constants.TablesNames.TLIcivilSiteDate.ToString(), null, "allCivilInstId", "Dismantle", "SiteCode");

                    objectInst.CivilSupportDistance = _unitOfWork.AttributeActivatedRepository
                        .GetInstAttributeActivated(Helpers.Constants.TablesNames.TLIcivilSupportDistance.ToString(), null, "CivilInstId", "SiteCode");

                    objectInst.Leg = _unitOfWork.AttributeActivatedRepository
                    .GetInstAttributeActivated(Helpers.Constants.TablesNames.TLIleg.ToString(), null, "CiviLegName", "CivilWithLegInstId");

                    List<KeyValuePair<string, List<DropDownListFilters>>> RelatedTables = new List<KeyValuePair<string, List<DropDownListFilters>>>();
                    List<DropDownListFilters> RelatedToSite = GetRelatedToSite(SiteCode, TableName);
                    RelatedTables.Add(new KeyValuePair<string, List<DropDownListFilters>>("ReferenceCivilId", RelatedToSite));

                    BaseInstAttView ReferenceCivilIdObjectInCivilSupportDistance = objectInst.CivilSupportDistance
                        .FirstOrDefault(x => x.Key.ToLower() == "ReferenceCivilId".ToLower());

                    if (ReferenceCivilIdObjectInCivilSupportDistance != null)
                        objectInst.CivilSupportDistance.FirstOrDefault(x => x.Key.ToLower() == "ReferenceCivilId".ToLower()).Value = RelatedToSite;

                    objectInst.RelatedTables = RelatedTables;
                }
                else if (Helpers.Constants.CivilType.TLIcivilWithoutLeg.ToString() == TableName)
                {
                    CivilWithoutLegLibraryViewModel CivilWithoutLegLibrary = _mapper.Map<CivilWithoutLegLibraryViewModel>(_unitOfWork.CivilWithoutLegLibraryRepository
                        .GetIncludeWhereFirst(x => x.Id == CivilLibraryId, x => x.CivilSteelSupportCategory, x => x.CivilWithoutLegCategory,
                        x => x.InstallationCivilwithoutLegsType, x => x.structureType));

                    List<BaseAttView> LibraryAttributes = _unitOfWork.AttributeActivatedRepository
                        .GetAttributeActivated(Helpers.Constants.TablesNames.TLIcivilWithoutLegLibrary.ToString(), CivilWithoutLegLibrary, null).ToList();

                    foreach (BaseAttView LibraryAttribute in LibraryAttributes)
                    {
                        if (LibraryAttribute.DataType.ToLower() == "list")
                        {
                            LibraryAttribute.Value = CivilWithoutLegLibrary.GetType().GetProperties()
                                .FirstOrDefault(x => x.Name.ToLower() == LibraryAttribute.Label.ToLower()).GetValue(CivilWithoutLegLibrary);
                        }
                    }

                    List<BaseAttView> AddToLibraryAttributesActivated = _mapper.Map<List<BaseAttView>>(_unitOfWork.LogistcalRepository
                        .GetLogistical(Helpers.Constants.TablePartName.CivilSupport.ToString(), Helpers.Constants.TablesNames.TLIcivilWithoutLegLibrary.ToString(), CivilWithoutLegLibrary.Id).ToList());

                    LibraryAttributes.AddRange(AddToLibraryAttributesActivated);

                    objectInst.LibraryActivatedAttributes = LibraryAttributes;

                    List<BaseInstAttView> ListAttributesActivated = _unitOfWork.AttributeActivatedRepository.
                        GetInstAttributeActivatedForCivilWithoutLeg(CategoryId, null, "CivilWithoutlegsLibId", "CurrentLoads").ToList();

                    BaseInstAttView NameAttribute = ListAttributesActivated.FirstOrDefault(x => x.Key.ToLower() == "Name".ToLower());
                    if (NameAttribute != null)
                    {
                        BaseInstAttView Swap = ListAttributesActivated[0];
                        ListAttributesActivated[ListAttributesActivated.IndexOf(NameAttribute)] = Swap;
                        ListAttributesActivated[0] = NameAttribute;
                       
                    }
                    if (CategoryId == 1)
                    {
                        ListAttributesActivated.Remove(NameAttribute);
                    }
                    foreach (BaseInstAttView FKitem in ListAttributesActivated)
                    {
                        if (FKitem.Desc.ToLower() == "tliowner")
                            FKitem.Value = _mapper.Map<List<OwnerViewModel>>(_unitOfWork.OwnerRepository.GetWhere(x => !x.Deleted && !x.Disable).ToList());

                        else if (FKitem.Desc.ToLower() == "tlisubtype")
                            FKitem.Value = _mapper.Map<List<SubTypeViewModel>>(_unitOfWork.SubTypeRepository.GetWhere(x => !x.Delete && !x.Disable).ToList());

                        else if (FKitem.Key.ToLower() == "equipmentslocation")
                        {
                            List<EnumOutPut> equipmentslocation = new List<EnumOutPut>();
                            equipmentslocation.Add(new EnumOutPut
                            {
                                Id = (int)EquipmentsLocation.Body,
                                Name = EquipmentsLocation.Body.ToString()
                            });
                            equipmentslocation.Add(new EnumOutPut
                            {
                                Id = (int)EquipmentsLocation.Platform,
                                Name = EquipmentsLocation.Platform.ToString()
                            });
                            equipmentslocation.Add(new EnumOutPut
                            {
                                Id = (int)EquipmentsLocation.Together,
                                Name = EquipmentsLocation.Together.ToString()
                            });
                            FKitem.Value = equipmentslocation;
                        }
                        else if (FKitem.Key.ToLower() == "availabilityofworkplatforms")
                        {
                            List<EnumOutPut> availabilityOfWorkPlatforms = new List<EnumOutPut>();
                            availabilityOfWorkPlatforms.Add(new EnumOutPut
                            {
                                Id = (int)AvailabilityOfWorkPlatforms.No,
                                Name = AvailabilityOfWorkPlatforms.No.ToString()
                            });
                            availabilityOfWorkPlatforms.Add(new EnumOutPut
                            {
                                Id = (int)AvailabilityOfWorkPlatforms.Yes,
                                Name = AvailabilityOfWorkPlatforms.Yes.ToString()
                            });
                            FKitem.Value = availabilityOfWorkPlatforms;
                        }
                        else if (FKitem.Key.ToLower() == "laddersteps")
                        {
                            List<EnumOutPut> ladderSteps = new List<EnumOutPut>();
                            ladderSteps.Add(new EnumOutPut
                            {
                                Id = (int)LadderSteps.Ladder,
                                Name = LadderSteps.Ladder.ToString()
                            });
                            ladderSteps.Add(new EnumOutPut
                            {
                                Id = (int)LadderSteps.Steps,
                                Name = LadderSteps.Steps.ToString()
                            });
                            FKitem.Value = ladderSteps;
                        }
                        else if (FKitem.Key.ToLower() == "reinforced")
                        {
                            List<EnumOutPut> reinforced = new List<EnumOutPut>();
                            reinforced.Add(new EnumOutPut
                            {
                                Id = (int)Reinforced.Yes,
                                Name = Reinforced.Yes.ToString()
                            });
                            reinforced.Add(new EnumOutPut
                            {
                                Id = (int)Reinforced.No,
                                Name = Reinforced.No.ToString()
                            });
                            FKitem.Value = reinforced;
                        }
                    }

                      objectInst.AttributesActivated = ListAttributesActivated;
                    
                    objectInst.CivilSiteDate = _unitOfWork.AttributeActivatedRepository.GetInstAttributeActivated(Helpers.Constants.TablesNames.TLIcivilSiteDate.ToString(), null, "allCivilInstId", "Dismantle", "SiteCode");
                    objectInst.CivilSupportDistance = _unitOfWork.AttributeActivatedRepository.GetInstAttributeActivated(Helpers.Constants.TablesNames.TLIcivilSupportDistance.ToString(), null, "CivilInstId", "SiteCode");

                    IEnumerable<DynaminAttInstViewModel> DynamicAttributesWithoutValue = _unitOfWork.DynamicAttRepository
                        .GetDynamicInstAtts(TableNameEntity.Id, CategoryId);

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
                    var RelatedToSite = GetRelatedToSite(SiteCode, TableName);
                    RelatedTables.Add(new KeyValuePair<string, List<DropDownListFilters>>("ReferenceCivilId", RelatedToSite));
                    objectInst.RelatedTables = RelatedTables;
                }
                else if (Helpers.Constants.CivilType.TLIcivilNonSteel.ToString() == TableName)
                {
                    CivilNonSteelLibraryViewModel CivilNonSteelLibrary = _mapper.Map<CivilNonSteelLibraryViewModel>(_unitOfWork.CivilNonSteelLibraryRepository
                        .GetIncludeWhereFirst(x => x.Id == CivilLibraryId, x => x.civilNonSteelType));

                    List<BaseAttView> LibraryAttributes = _unitOfWork.AttributeActivatedRepository
                        .GetAttributeActivated(Helpers.Constants.TablesNames.TLIcivilNonSteelLibrary.ToString(), CivilNonSteelLibrary, null, "CurrentLoads").ToList();

                    foreach (BaseAttView LibraryAttribute in LibraryAttributes)
                    {
                        if (LibraryAttribute.DataType.ToLower() == "list")
                        {
                            LibraryAttribute.Value = CivilNonSteelLibrary.GetType().GetProperties()
                                .FirstOrDefault(x => x.Name.ToLower() == LibraryAttribute.Label.ToLower()).GetValue(CivilNonSteelLibrary);
                        }
                    }

                    List<BaseAttView> AddToLibraryAttributesActivated = _mapper.Map<List<BaseAttView>>(_unitOfWork.LogistcalRepository
                        .GetLogistical(Helpers.Constants.TablePartName.CivilSupport.ToString(), Helpers.Constants.TablesNames.TLIcivilNonSteelLibrary.ToString(), CivilNonSteelLibrary.Id).ToList());

                    LibraryAttributes.AddRange(AddToLibraryAttributesActivated);

                    objectInst.LibraryActivatedAttributes = LibraryAttributes;

                    List<BaseInstAttView> ListAttributesActivated = _unitOfWork.AttributeActivatedRepository.
                        GetInstAttributeActivated(TableName, null, "CivilNonSteelLibraryId", "CurrentLoads").ToList();

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
                            FKitem.Value = _mapper.Map<List<OwnerViewModel>>(_unitOfWork.OwnerRepository.GetWhere(x => !x.Deleted && !x.Disable).ToList());

                        else if (FKitem.Desc.ToLower() == "tlisupporttypeimplemented")
                            FKitem.Value = _mapper.Map<List<SupportTypeImplementedViewModel>>(_unitOfWork.SupportTypeImplementedRepository.GetWhere(x => !x.Deleted && !x.Disable).ToList());

                        else if (FKitem.Desc.ToLower() == "tlilocationtype")
                            FKitem.Value = _mapper.Map<List<LocationTypeViewModel>>(_unitOfWork.LocationTypeRepository.GetWhere(x => !x.Deleted && !x.Disable).ToList());
                    }

                    objectInst.AttributesActivated = ListAttributesActivated;
                    objectInst.CivilSiteDate = _unitOfWork.AttributeActivatedRepository.GetInstAttributeActivated(Helpers.Constants.TablesNames.TLIcivilSiteDate.ToString(), null, "allCivilInstId", "Dismantle", "SiteCode");
                    objectInst.CivilSupportDistance = _unitOfWork.AttributeActivatedRepository.GetInstAttributeActivated(Helpers.Constants.TablesNames.TLIcivilSupportDistance.ToString(), null, "CivilInstId", "SiteCode");
                    IEnumerable<DynaminAttInstViewModel> DynamicAttributesWithoutValue = _unitOfWork.DynamicAttRepository
                        .GetDynamicInstAtts(TableNameEntity.Id, CategoryId);

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
                    List<DropDownListFilters> RelatedToSite = GetRelatedToSite(SiteCode, TableName);
                    RelatedTables.Add(new KeyValuePair<string, List<DropDownListFilters>>("ReferenceCivilId", RelatedToSite));
                    objectInst.RelatedTables = RelatedTables;
                }
                return new Response<ObjectInst>(true, objectInst, null, null, (int)Helpers.Constants.ApiReturnCode.success);
            }
            catch (Exception err)
            {
                return new Response<ObjectInst>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
        } 
        //Return all civil in this site
        private List<DropDownListFilters> GetRelatedToSite(string SiteCode, string TableName)
        {
            //List<DropDownListFilters> values = new List<DropDownListFilters>();

            List<DropDownListFilters> CivilInstallations = _unitOfWork.CivilSiteDateRepository.GetIncludeWhere(x =>
                x.SiteCode == SiteCode && !x.Dismantle, x => x.allCivilInst, x => x.allCivilInst.civilWithLegs,
                    x => x.allCivilInst.civilWithoutLeg, x => x.allCivilInst.civilNonSteel)
                .Select(x => new DropDownListFilters()
                {
                    Id = x.allCivilInstId,
                    Deleted = false,
                    Disable = false,
                    Value = x.allCivilInst.civilWithLegsId != null ? x.allCivilInst.civilWithLegs.Name : 
                        (x.allCivilInst.civilWithoutLegId != null ? x.allCivilInst.civilWithoutLeg.Name : x.allCivilInst.civilNonSteel.Name)
                }).ToList();

            //for (int i = 0; i < CivilInstallations.Count; i++)
            //{
            //    TLIallCivilInst Civil = _unitOfWork.AllCivilInstRepository
            //        .GetWhereFirst(x => x.Id == CivilInstallations[i]);

            //    if (Civil.civilWithLegsId != null)
            //    {
            //        Civil.civilWithLegs = _unitOfWork.CivilSiteDateRepository.GetIncludeWhereFirst(x => !x.Dismantle && x.SiteCode.ToLower() == SiteCode.ToLower() &&
            //            (x.allCivilInst.civilWithLegsId != null ? x.allCivilInst.civilWithLegsId == Civil.civilWithLegsId.Value : false) &&
            //            !x.allCivilInst.Draft, x => x.allCivilInst, x => x.allCivilInst.civilWithLegs).allCivilInst.civilWithLegs;

            //        values.Add(new DropDownListFilters(Civil.civilWithLegs.Id, Civil.civilWithLegs.Name));
            //    }
            //    else if (Civil.civilWithoutLegId != null)
            //    {
            //        Civil.civilWithoutLeg = _unitOfWork.CivilSiteDateRepository.GetIncludeWhereFirst(x => !x.Dismantle && x.SiteCode.ToLower() == SiteCode.ToLower() &&
            //            (x.allCivilInst.civilWithoutLegId != null ? x.allCivilInst.civilWithoutLegId == Civil.civilWithoutLegId.Value : false) &&
            //            !x.allCivilInst.Draft, x => x.allCivilInst, x => x.allCivilInst.civilWithoutLeg).allCivilInst.civilWithoutLeg;

            //        values.Add(new DropDownListFilters(Civil.civilWithoutLeg.Id, Civil.civilWithoutLeg.Name));
            //    }
            //    else if (Civil.civilNonSteelId != null)
            //    {
            //        Civil.civilNonSteel = _unitOfWork.CivilSiteDateRepository.GetIncludeWhereFirst(x => !x.Dismantle && x.SiteCode.ToLower() == SiteCode.ToLower() &&
            //            (x.allCivilInst.civilNonSteelId != null ? x.allCivilInst.civilNonSteelId == Civil.civilNonSteelId.Value : false) &&
            //            !x.allCivilInst.Draft, x => x.allCivilInst, x => x.allCivilInst.civilNonSteel).allCivilInst.civilNonSteel;

            //        values.Add(new DropDownListFilters(Civil.civilNonSteel.Id, Civil.civilNonSteel.Name));
            //    }
            //}
            //values.Add(new DropDownListFilters
            //{
            //    Id = 0,
            //    Value = "NA",
            //    Deleted = false,
            //    Disable = false

            //});
            return CivilInstallations;
        }
        //This Function accept two parameters
        //CivilInstallationViewModel object that accept any kind of ViewModel
        //TableName to specify any table i deal with 
        #region Helper Methods..

        private List<DropDownListFilters> GetRelatedToSiteToEdit(string SiteCode, string TableName , int CivilInsId)
        {
            List<DropDownListFilters> values = new List<DropDownListFilters>();

            List<int> CivilInstallations = _unitOfWork.CivilSiteDateRepository.GetWhere(x =>
                x.SiteCode == SiteCode && !x.Dismantle).Select(x => x.allCivilInstId).ToList();

            for (int i = 0; i < CivilInstallations.Count; i++)
            {
                TLIallCivilInst Civil = _unitOfWork.AllCivilInstRepository
                    .GetWhereFirst(x => x.Id == CivilInstallations[i]);

                if (Civil.civilWithLegsId != null)
                {
                    Civil.civilWithLegs = _unitOfWork.CivilWithLegsRepository.GetWhereFirst(x => x.Id == Civil.civilWithLegsId && x.Id != CivilInsId);
                    if (Civil.civilWithLegs != null)
                    {
                        values.Add(new DropDownListFilters(Civil.civilWithLegs.Id, Civil.civilWithLegs.Name));
                    }
                }
                else if (Civil.civilWithoutLegId != null)
                {
                    Civil.civilWithoutLeg = _unitOfWork.CivilWithoutLegRepository.GetWhereFirst(x => x.Id == Civil.civilWithoutLegId && x.Id != CivilInsId);
                    if (Civil.civilWithoutLeg != null)
                    {
                        values.Add(new DropDownListFilters(Civil.civilWithoutLeg.Id, Civil.civilWithoutLeg.Name));
                    }
                }
                else if (Civil.civilNonSteelId != null)
                {
                    Civil.civilNonSteel = _unitOfWork.CivilNonSteelRepository.GetWhereFirst(x => x.Id == Civil.civilNonSteelId && x.Id != CivilInsId);
                    if (Civil.civilNonSteel != null)
                    {
                        values.Add(new DropDownListFilters(Civil.civilNonSteel.Id, Civil.civilNonSteel.Name));
                    }
                }
            }
            values.Add(new DropDownListFilters
            {
                Id = 0,
                Value = "NA",
                Deleted = false,
                Disable = false

            });
            return values;
        }
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
        public string CheckDependencyValidationForCivilTypes(object Input, string CivilType, string SiteCode, int? CategoryId)
        {
            if (CivilType.ToLower() == Helpers.Constants.TablesNames.TLIcivilWithLegs.ToString().ToLower())
            {
                string MainTableName = Helpers.Constants.TablesNames.TLIcivilWithLegs.ToString();
                AddCivilWithLegsViewModel AddCivilInstallationViewModel = _mapper.Map<AddCivilWithLegsViewModel>(Input);

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

                                    object TestValue = AddCivilInstallationViewModel.GetType().GetProperties()
                                        .FirstOrDefault(x => x.Name.ToLower() == AttributeName.ToLower()).GetValue(AddCivilInstallationViewModel, null);

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
                                    AddDynamicAttInstValueViewModel DynamicObject = AddCivilInstallationViewModel.TLIdynamicAttInstValue
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
                                    object CivilLoads = AddCivilInstallationViewModel.GetType().GetProperty(Path[0])
                                        .GetValue(AddCivilInstallationViewModel, null);

                                    CheckId = CivilLoads.GetType().GetProperty(Path[1]).GetValue(CivilLoads, null) != null ?
                                        (int)CivilLoads.GetType().GetProperty(Path[1]).GetValue(CivilLoads, null) : new object();
                                }
                                else if (Path.Count() == 1 && Path[0].ToLower() == "sitecode")
                                {
                                    CheckId = SiteCode;
                                }
                                else if (Path.Count() == 1)
                                {
                                    if (AddCivilInstallationViewModel.GetType().GetProperty(Path[0]).GetValue(AddCivilInstallationViewModel, null) != null)
                                        CheckId = (int)AddCivilInstallationViewModel.GetType().GetProperty(Path[0])
                                            .GetValue(AddCivilInstallationViewModel, null);
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

                            AddDynamicAttInstValueViewModel InputDynamicAttribute = AddCivilInstallationViewModel.TLIdynamicAttInstValue
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
            else if (CivilType.ToLower() == Helpers.Constants.TablesNames.TLIcivilWithoutLeg.ToString().ToLower())
            {
                string MainTableName = Helpers.Constants.TablesNames.TLIcivilWithoutLeg.ToString();
                AddCivilWithoutLegViewModel AddCivilInstallationViewModel = _mapper.Map<AddCivilWithoutLegViewModel>(Input);

                List<DynamicAttViewModel> DynamicAttributes = _mapper.Map<List<DynamicAttViewModel>>(_unitOfWork.DynamicAttRepository
                    .GetIncludeWhere(x => x.tablesNames.TableName.ToLower() == MainTableName.ToLower() && !x.disable &&
                        ((x.CivilWithoutLegCategoryId != null && CategoryId != null) ? x.CivilWithoutLegCategoryId == CategoryId : false), x => x.tablesNames).ToList());

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

                                    object TestValue = AddCivilInstallationViewModel.GetType().GetProperties()
                                        .FirstOrDefault(x => x.Name.ToLower() == AttributeName.ToLower()).GetValue(AddCivilInstallationViewModel, null);

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
                                    AddDynamicAttInstValueViewModel DynamicObject = AddCivilInstallationViewModel.TLIdynamicAttInstValue
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
                                    object CivilLoads = AddCivilInstallationViewModel.GetType().GetProperty(Path[0])
                                        .GetValue(AddCivilInstallationViewModel, null);

                                    CheckId = CivilLoads.GetType().GetProperty(Path[1]).GetValue(CivilLoads, null) != null ?
                                        (int)CivilLoads.GetType().GetProperty(Path[1]).GetValue(CivilLoads, null) : new object();
                                }
                                else if (Path.Count() == 1 && Path[0].ToLower() == "sitecode")
                                {
                                    CheckId = SiteCode;
                                }
                                else if (Path.Count() == 1)
                                {
                                    if (AddCivilInstallationViewModel.GetType().GetProperty(Path[0]).GetValue(AddCivilInstallationViewModel, null) != null)
                                        CheckId = (int)AddCivilInstallationViewModel.GetType().GetProperty(Path[0])
                                            .GetValue(AddCivilInstallationViewModel, null);
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

                            AddDynamicAttInstValueViewModel InputDynamicAttribute = AddCivilInstallationViewModel.TLIdynamicAttInstValue
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
            else if (CivilType.ToLower() == Helpers.Constants.TablesNames.TLIcivilNonSteel.ToString().ToLower())
            {
                string MainTableName = Helpers.Constants.TablesNames.TLIcivilNonSteel.ToString();
                AddCivilNonSteelViewModel AddCivilInstallationViewModel = _mapper.Map<AddCivilNonSteelViewModel>(Input);

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

                                    object TestValue = AddCivilInstallationViewModel.GetType().GetProperties()
                                        .FirstOrDefault(x => x.Name.ToLower() == AttributeName.ToLower()).GetValue(AddCivilInstallationViewModel, null);

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
                                    AddDynamicAttInstValueViewModel DynamicObject = AddCivilInstallationViewModel.TLIdynamicAttInstValue
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
                                    object CivilLoads = AddCivilInstallationViewModel.GetType().GetProperty(Path[0])
                                        .GetValue(AddCivilInstallationViewModel, null);

                                    CheckId = CivilLoads.GetType().GetProperty(Path[1]).GetValue(CivilLoads, null) != null ?
                                        (int)CivilLoads.GetType().GetProperty(Path[1]).GetValue(CivilLoads, null) : new object();
                                }
                                else if (Path.Count() == 1 && Path[0].ToLower() == "sitecode")
                                {
                                    CheckId = SiteCode;
                                }
                                else if (Path.Count() == 1)
                                {
                                    if (AddCivilInstallationViewModel.GetType().GetProperty(Path[0]).GetValue(AddCivilInstallationViewModel, null) != null)
                                        CheckId = (int)AddCivilInstallationViewModel.GetType().GetProperty(Path[0])
                                            .GetValue(AddCivilInstallationViewModel, null);
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

                            AddDynamicAttInstValueViewModel InputDynamicAttribute = AddCivilInstallationViewModel.TLIdynamicAttInstValue
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
        public string CheckGeneralValidationFunction(List<AddDynamicAttInstValueViewModel> TLIdynamicAttInstValue, string TableName, int? catid = null)
        {
            List<DynamicAttViewModel> DynamicAttributes = null;

            if (catid != null)
            {
                DynamicAttributes = _mapper.Map<List<DynamicAttViewModel>>(_unitOfWork.DynamicAttRepository
                   .GetIncludeWhere(x => x.tablesNames.TableName.ToLower() == TableName.ToLower() && !x.disable && x.CivilWithoutLegCategoryId == catid
                       , x => x.tablesNames).ToList());
            }
            else
            {
                DynamicAttributes = _mapper.Map<List<DynamicAttViewModel>>(_unitOfWork.DynamicAttRepository
                   .GetIncludeWhere(x => x.tablesNames.TableName.ToLower() == TableName.ToLower() && !x.disable
                       , x => x.tablesNames).ToList());
            }

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
        public Response<ObjectInstAtts> AddCivilInstallation(object CivilInstallationViewModel, string TableName, string SiteCode, string connectionString)
        {
            //using (TransactionScope transaction = new TransactionScope())
            //{
            int allCivilInstId = 0;

            using (TransactionScope transaction = new TransactionScope())
            {
                try
                {
                    string ErrorMessage = string.Empty;
                    var TableNameEntity = _unitOfWork.TablesNamesRepository.GetWhereFirst(c => c.TableName == TableName);
                    if (Helpers.Constants.CivilType.TLIcivilWithLegs.ToString() == TableName)
                    {
                        //Add Civil With Legs
                        //Map to AddCivilWithLegsViewModel to ViewModel
                        AddCivilWithLegsViewModel addCivilWithLegs = _mapper.Map<AddCivilWithLegsViewModel>(CivilInstallationViewModel);
                        if (addCivilWithLegs.ticketAtt != null ? addCivilWithLegs.ticketAtt.TicketId == 0 : false)
                            addCivilWithLegs.ticketAtt = null;
                        //Map to TLIcivilWithLegs Entity
                        TLIcivilWithLegs civilWithLegs = _mapper.Map<TLIcivilWithLegs>(addCivilWithLegs);

                        if (addCivilWithLegs.TLIcivilSiteDate.ReservedSpace == true)
                        {
                            var CheckSpace = _unitOfWork.SiteRepository.CheckSpace(SiteCode, TableName, addCivilWithLegs.CivilWithLegsLibId, addCivilWithLegs.SpaceInstallation, null).Message;
                            if (CheckSpace != "Success")
                            {
                                return new Response<ObjectInstAtts>(true, null, null, CheckSpace, (int)Helpers.Constants.ApiReturnCode.fail);
                            }
                        }
                        //Check Validations
                        string CheckGeneralValidation = CheckGeneralValidationFunction(addCivilWithLegs.TLIdynamicAttInstValue, TableName);

                        if (!string.IsNullOrEmpty(CheckGeneralValidation))
                            return new Response<ObjectInstAtts>(true, null, null, CheckGeneralValidation, (int)Helpers.Constants.ApiReturnCode.fail);

                        string CheckDependencyValidation = CheckDependencyValidationForCivilTypes(CivilInstallationViewModel, TableName, SiteCode, null);

                        if (!string.IsNullOrEmpty(CheckDependencyValidation))
                            return new Response<ObjectInstAtts>(true, null, null, CheckDependencyValidation, (int)Helpers.Constants.ApiReturnCode.fail);
                        var civilwithleglibrary = _dbContext.TLIcivilWithLegLibrary.Where(x => x.Id == addCivilWithLegs.CivilWithLegsLibId).FirstOrDefault();
                        var sitename = _dbContext.TLIsite.Where(x => x.SiteCode == SiteCode).FirstOrDefault();
                        var ownername = _dbContext.TLIowner.Where(x => x.Id == addCivilWithLegs.OwnerId).FirstOrDefault();
                        civilWithLegs.Name = sitename.SiteName + civilwithleglibrary.Model + ownername.OwnerName + addCivilWithLegs.HeightImplemented;
                        //Check name if already exists in database then return message that the name is already exists
                        TLIcivilSiteDate CheckName = _unitOfWork.CivilSiteDateRepository.GetIncludeWhereFirst(x => !x.Dismantle && !x.allCivilInst.Draft &&
                            (x.allCivilInst.civilWithLegsId != null ? x.allCivilInst.civilWithLegs.Name.ToLower() == civilWithLegs.Name.ToLower() : false &&
                                x.SiteCode.ToLower() == SiteCode.ToLower()), x => x.allCivilInst, x => x.allCivilInst.civilWithLegs);

                        if (CheckName != null)
                            return new Response<ObjectInstAtts>(true, null, null, $"The name {civilWithLegs.Name} is already exists", (int)Helpers.Constants.ApiReturnCode.fail);
                        //TLIcivilSiteDate CheckName = _unitOfWork.CivilSiteDateRepository.GetIncludeWhereFirst(x => !x.Dismantle && !x.allCivilInst.Draft &&
                        //    (x.allCivilInst.civilWithLegsId != null ? x.allCivilInst.civilWithLegs.Name.ToLower() == civilWithLegs.Name.ToLower() : false), x => x.allCivilInst, x => x.allCivilInst.civilWithLegs);
                        //if (CheckName != null)
                        //    return new Response<ObjectInstAtts>(true, null, null, $"The name {civilWithLegs.Name} is already exists", (int)Helpers.Constants.ApiReturnCode.fail);

                        //Add Entity
                        civilWithLegs.Legs = null;
                        _unitOfWork.CivilWithLegsRepository.AddWithHistory(Helpers.LogFilterAttribute.UserId, civilWithLegs);
                        _unitOfWork.SaveChanges();

                        //Add to Civil_Site_Date if there is free space
                        TLIallCivilInst allCivilInst = new TLIallCivilInst();
                        allCivilInst.civilWithLegsId = civilWithLegs.Id;
                        allCivilInst.Draft = false;

                        //if (addCivilWithLegs.ticketAtt != null)
                        //{
                        //    allCivilInst.Draft = true;
                        //    allCivilInst.TicketId = addCivilWithLegs.ticketAtt.TicketId;
                        //    allCivilInst.ItemStatusId = addCivilWithLegs.ticketAtt.ItemsStatusId;
                        //}

                        _unitOfWork.AllCivilInstRepository.Add(allCivilInst);
                        _unitOfWork.SaveChanges();
                        allCivilInstId = allCivilInst.Id;

                        //Check if Site object not null and SiteCode not null or empty then add civilSiteDate
                        if (addCivilWithLegs.TLIcivilSiteDate != null && string.IsNullOrEmpty(SiteCode) == false)
                        {
                            TLIcivilSiteDate civilSiteDate = new TLIcivilSiteDate();
                            civilSiteDate.SiteCode = SiteCode;
                            civilSiteDate.allCivilInstId = allCivilInst.Id;
                            civilSiteDate.InstallationDate = addCivilWithLegs.TLIcivilSiteDate.InstallationDate;
                            civilSiteDate.LongitudinalSpindleLengthm = addCivilWithLegs.TLIcivilSiteDate.LongitudinalSpindleLengthm;
                            civilSiteDate.HorizontalSpindleLengthm = addCivilWithLegs.TLIcivilSiteDate.HorizontalSpindleLengthm;
                            civilSiteDate.ReservedSpace = addCivilWithLegs.TLIcivilSiteDate.ReservedSpace;
                            _unitOfWork.CivilSiteDateRepository.Add(civilSiteDate);
                            _unitOfWork.SaveChanges();
                        }

                        //Get all civils in one site if there is no one then we can't add civil reference
                        List<TLIcivilSiteDate> SiteCivils = _unitOfWork.CivilSiteDateRepository.GetWhere(x => x.SiteCode == SiteCode && x.allCivilInst.civilWithLegsId != null && x.allCivilInst.civilWithoutLegId == null && x.allCivilInst.civilNonSteelId == null).ToList();

                        //if CivilSupportDistance object not null and there is civils in site then add to TLIcivilSupportDistance table
                        if (addCivilWithLegs.TLIcivilSupportDistance != null)
                        {
                            TLIcivilSupportDistance civilSupportDistance = new TLIcivilSupportDistance();
                            civilSupportDistance.Distance = addCivilWithLegs.TLIcivilSupportDistance.Distance;
                            civilSupportDistance.Azimuth = addCivilWithLegs.TLIcivilSupportDistance.Azimuth;
                            civilSupportDistance.SiteCode = SiteCode;
                            civilSupportDistance.ReferenceCivilId = addCivilWithLegs.TLIcivilSupportDistance.ReferenceCivilId;
                            civilSupportDistance.CivilInstId = allCivilInst.Id;
                            _unitOfWork.CivilSupportDistanceRepository.Add(civilSupportDistance);
                            _unitOfWork.SaveChanges();
                        }
                       
                        if (addCivilWithLegs.Legs != null)
                        {
                            List<TLIleg> addLegViewModels = new List<TLIleg>();
                            foreach (var item in addCivilWithLegs.Legs)
                            {
                                //addLegViewModel.Id =
                                //addLegViewModel.CiviLegName = civilWithLegs.Name + item.LegLetter;
                                //addLegViewModel.LegLetter = item.LegLetter;
                                //addLegViewModel.CivilWithLegInstId = civilWithLegs.Id;
                                //addLegViewModel.LegAzimuth = item.LegAzimuth;
                                //addLegViewModel.Notes = item.Notes;
                                //addLegViewModels.Add(addLegViewModel);
                                //_unitOfWork.LegRepository.Add(addLegViewModel);
                                //_unitOfWork.SaveChanges();
                                TLIleg tLIleg = new TLIleg()
                                {
                                    CiviLegName = civilWithLegs.Name + item.LegLetter,
                                    LegLetter = item.LegLetter,
                                    CivilWithLegInstId = civilWithLegs.Id,
                                    Notes = item.Notes,
                                    LegAzimuth = item.LegAzimuth,

                                };

                               addLegViewModels.Add(tLIleg);
                               _unitOfWork.LegRepository.Add(tLIleg);
                               _unitOfWork.SaveChanges();
                            }
                        }
                        //Update Reserved Space in TLIsite if ReservedSpace is true
                        //if (addCivilWithLegs.TLIcivilSiteDate.ReservedSpace == true)
                        //{
                        //    _unitOfWork.SiteRepository.UpdateReservedSpace(SiteCode, addCivilWithLegs.SpaceInstallation);
                        //}
                        //Add Dynamic Attributes
                        foreach (var addDynamicAttsInstValue in addCivilWithLegs.TLIdynamicAttInstValue)
                        {
                            _unitOfWork.DynamicAttInstValueRepository.AddDynamicInstAtts(addDynamicAttsInstValue, TableNameEntity.Id, civilWithLegs.Id);
                        }
                    }
                    else if (Helpers.Constants.CivilType.TLIcivilWithoutLeg.ToString() == TableName)
                    {
                        //Add Civil Without Legs
                        //Map object To ViewModel
                        AddCivilWithoutLegViewModel AddCivilWithoutLeg = _mapper.Map<AddCivilWithoutLegViewModel>(CivilInstallationViewModel);
                        if (AddCivilWithoutLeg.ticketAtt != null ? AddCivilWithoutLeg.ticketAtt.TicketId == 0 : false)
                            AddCivilWithoutLeg.ticketAtt = null;
                        //View Model To Entity
                        TLIcivilWithoutLeg CivilWithoutLeg = _mapper.Map<TLIcivilWithoutLeg>(AddCivilWithoutLeg);
                        if (AddCivilWithoutLeg.TLIcivilSiteDate.ReservedSpace == true)
                        {
                            var CheckSpace = _unitOfWork.SiteRepository.CheckSpace(SiteCode, TableName, AddCivilWithoutLeg.CivilWithoutlegsLibId, AddCivilWithoutLeg.SpaceInstallation, null).Message;
                            if (CheckSpace != "Success")
                            {
                                return new Response<ObjectInstAtts>(true, null, null, CheckSpace, (int)Helpers.Constants.ApiReturnCode.fail);
                            }
                        }
                        var CivilWithoutLegLibrary = _unitOfWork.CivilWithoutLegLibraryRepository.GetByID(CivilWithoutLeg.CivilWithoutlegsLibId);

                        //Check Validations
                        string CheckDependencyValidation = CheckDependencyValidationForCivilTypes(CivilInstallationViewModel, TableName, SiteCode, CivilWithoutLegLibrary.CivilWithoutLegCategoryId);

                        if (!string.IsNullOrEmpty(CheckDependencyValidation))
                            return new Response<ObjectInstAtts>(true, null, null, CheckDependencyValidation, (int)Helpers.Constants.ApiReturnCode.fail);

                        string CheckGeneralValidation = CheckGeneralValidationFunction(AddCivilWithoutLeg.TLIdynamicAttInstValue, TableName, CivilWithoutLegLibrary.CivilWithoutLegCategoryId.Value);
                        if (!string.IsNullOrEmpty(CheckGeneralValidation))
                            return new Response<ObjectInstAtts>(true, null, null, CheckGeneralValidation, (int)Helpers.Constants.ApiReturnCode.fail);

                        bool test = true;
                        if (test == true)
                        {
                            //Start Business Rules
                            var CivilSteelSupportCategory = _unitOfWork.CivilWithoutLegCategoryRepository.GetWhereFirst(x=>x.Id==CivilWithoutLegLibrary.CivilWithoutLegCategoryId).Name;
                            if (CivilSteelSupportCategory.ToLower() == "mast")
                            {
                                var Site = _unitOfWork.SiteRepository.GetWhereFirst(x => x.SiteCode == SiteCode);
                                var Owner = _unitOfWork.OwnerRepository.GetByID((int)AddCivilWithoutLeg.OwnerId);
                                CivilWithoutLeg.Name = Site.SiteName + " " + CivilWithoutLegLibrary.Model + " " + Owner.OwnerName + " " + AddCivilWithoutLeg.HeightImplemented;

                                if (CivilWithoutLeg.HeightImplemented < 6)
                                {
                                    CivilWithoutLeg.NumberOfCivilParts = 1;
                                    CivilWithoutLeg.BottomPartLengthm = CivilWithoutLeg.HeightImplemented;
                                }
                                else
                                {
                                    CivilWithoutLeg.NumberOfCivilParts = 2;
                                    CivilWithoutLeg.UpperPartLengthm = CivilWithoutLeg.HeightImplemented - CivilWithoutLeg.BottomPartLengthm;
                                }
                                if (CivilWithoutLeg.NumberOfCivilParts == 2)
                                {
                                    if (CivilWithoutLeg.UpperPartDiameterm == null)
                                    {
                                        return new Response<ObjectInstAtts>(true, null, null, "Upper part diameterm shouldn't be null.", (int)Helpers.Constants.ApiReturnCode.fail);
                                    }
                                }
                                if (CivilWithoutLegLibrary.Model.ToLower().Contains("located"))
                                {
                                    if (CivilWithoutLeg.BasePlateLengthcm == 0 || CivilWithoutLeg.BasePlateThicknesscm == 0 || CivilWithoutLeg.BasePlateWidthcm == 0)
                                    {
                                        return new Response<ObjectInstAtts>(true, null, null, "Base Plate Length and Base Plate Thickness and Base Plate Width Shouldn't be null if Model Contains located", (int)Helpers.Constants.ApiReturnCode.fail);
                                    }
                                }
                                if (CivilWithoutLegLibrary.Model.ToLower().Contains("anchored"))
                                {
                                    if (CivilWithoutLeg.SpindlesBasePlateLengthcm == 0 || CivilWithoutLeg.SpindlesBasePlateWidthcm == 0)
                                    {
                                        return new Response<ObjectInstAtts>(true, null, null, "Spindles Base Plate Length and Spindles Base Plate Width Shouldn't be null if Model Contains anchored", (int)Helpers.Constants.ApiReturnCode.fail);
                                    }
                                }
                            }
                            //End Business Rules
                            //Add CivilWithoutLeg Entity
                            //Check Name if already exists in database 
                            //if true return error message that the name is already exists
                            TLIcivilSiteDate CheckName = _unitOfWork.CivilSiteDateRepository.GetIncludeWhereFirst(x => !x.Dismantle && !x.allCivilInst.Draft &&
                                (x.allCivilInst.civilWithoutLegId != null ? x.allCivilInst.civilWithoutLeg.Name.ToLower() == CivilWithoutLeg.Name.ToLower() : false) &&
                                x.allCivilInst.civilWithoutLeg.CivilWithoutlegsLib.CivilWithoutLegCategoryId == CivilWithoutLegLibrary.CivilWithoutLegCategoryId &&
                                x.SiteCode.ToLower() == SiteCode.ToLower(),
                                    x => x.allCivilInst, x => x.allCivilInst.civilWithoutLeg, x => x.allCivilInst.civilWithoutLeg.CivilWithoutlegsLib);

                            if (CheckName != null)
                                return new Response<ObjectInstAtts>(true, null, null, $"The name {CivilWithoutLeg.Name} is already exists", (int)Helpers.Constants.ApiReturnCode.fail);

                            _unitOfWork.CivilWithoutLegRepository.AddWithHistory(Helpers.LogFilterAttribute.UserId, CivilWithoutLeg);
                            _unitOfWork.SaveChanges();
                            //Add to Civil_Site_Date if there is free space
                            TLIallCivilInst allCivilInst = new TLIallCivilInst();
                            allCivilInst.civilWithoutLegId = CivilWithoutLeg.Id;
                            allCivilInst.Draft = false;
                            //if (AddCivilWithoutLeg.ticketAtt != null)
                            //{
                            //    allCivilInst.Draft = true;
                            //    allCivilInst.TicketId = AddCivilWithoutLeg.ticketAtt.TicketId;
                            //    allCivilInst.ItemStatusId = AddCivilWithoutLeg.ticketAtt.ItemsStatusId;
                            //}
                            _unitOfWork.AllCivilInstRepository.Add(allCivilInst);
                            _unitOfWork.SaveChanges();
                            allCivilInstId = allCivilInst.Id;
                            //Add civilSiteDate if site object is not null
                            if (AddCivilWithoutLeg.TLIcivilSiteDate != null)
                            {
                                TLIcivilSiteDate civilSiteDate = new TLIcivilSiteDate();
                                civilSiteDate.SiteCode = SiteCode;
                                civilSiteDate.allCivilInstId = allCivilInst.Id;
                                civilSiteDate.InstallationDate = AddCivilWithoutLeg.TLIcivilSiteDate.InstallationDate;
                                civilSiteDate.LongitudinalSpindleLengthm = AddCivilWithoutLeg.TLIcivilSiteDate.LongitudinalSpindleLengthm;
                                civilSiteDate.HorizontalSpindleLengthm = AddCivilWithoutLeg.TLIcivilSiteDate.HorizontalSpindleLengthm;
                                civilSiteDate.ReservedSpace = AddCivilWithoutLeg.TLIcivilSiteDate.ReservedSpace;
                                _unitOfWork.CivilSiteDateRepository.Add(civilSiteDate);
                                _unitOfWork.SaveChanges();
                            }
                            //Check if there are other civils in that site
                            var SiteCivils = _unitOfWork.CivilSiteDateRepository.GetWhere(x => x.SiteCode == SiteCode && x.allCivilInst.civilWithLegsId == null && x.allCivilInst.civilWithoutLegId != null && x.allCivilInst.civilNonSteelId == null).ToList();
                            //check if there are other civils in that site and CivilSupportDistance not null
                            if (AddCivilWithoutLeg.TLIcivilSupportDistance != null)
                            {
                                TLIcivilSupportDistance civilSupportDistance = new TLIcivilSupportDistance();
                                civilSupportDistance.Distance = AddCivilWithoutLeg.TLIcivilSupportDistance.Distance;
                                civilSupportDistance.Azimuth = AddCivilWithoutLeg.TLIcivilSupportDistance.Azimuth;
                                civilSupportDistance.SiteCode = SiteCode;
                                civilSupportDistance.ReferenceCivilId = AddCivilWithoutLeg.TLIcivilSupportDistance.ReferenceCivilId;
                                civilSupportDistance.CivilInstId = allCivilInst.Id;
                                _unitOfWork.CivilSupportDistanceRepository.Add(civilSupportDistance);
                                _unitOfWork.SaveChanges();
                            }
                            //Update ReservedSpace in site table if ReservedSpace is true
                            //if (AddCivilWithoutLeg.TLIcivilSiteDate.ReservedSpace == true)
                            //{
                            //    _unitOfWork.SiteRepository.UpdateReservedSpace(SiteCode, AddCivilWithoutLeg.SpaceInstallation);
                            //}
                            //Add Dynamic Attributes
                            if (AddCivilWithoutLeg.TLIdynamicAttInstValue.Count > 0)
                            {
                                foreach (var addDynamicAttsInstValue in AddCivilWithoutLeg.TLIdynamicAttInstValue)
                                {
                                    _unitOfWork.DynamicAttInstValueRepository.AddDynamicInstAtts(addDynamicAttsInstValue, TableNameEntity.Id, CivilWithoutLeg.Id);
                                }
                            }
                            //AddCivilHistory(AddCivilWithoutLeg.ticketAtt, allCivilInstId, "Insert");
                        }
                        else
                        {
                            return new Response<ObjectInstAtts>(true, null, null, ErrorMessage, (int)Helpers.Constants.ApiReturnCode.fail);
                        }
                    }
                    else if (Helpers.Constants.CivilType.TLIcivilNonSteel.ToString() == TableName)
                    {
                        //Add Civil Non Steel
                        //Map object to ViewModel
                        AddCivilNonSteelViewModel AddCivilNonSteel = _mapper.Map<AddCivilNonSteelViewModel>(CivilInstallationViewModel);
                        if (AddCivilNonSteel.ticketAtt != null ? AddCivilNonSteel.ticketAtt.TicketId == 0 : false)
                            AddCivilNonSteel.ticketAtt = null;
                        //Map ViewModel to Entity
                        TLIcivilNonSteel CivilNonSteel = _mapper.Map<TLIcivilNonSteel>(AddCivilNonSteel);
                        if (AddCivilNonSteel.TLIcivilSiteDate.ReservedSpace == true)
                        {
                            var CheckSpace = _unitOfWork.SiteRepository.CheckSpace(SiteCode, TableName, AddCivilNonSteel.CivilNonSteelLibraryId, AddCivilNonSteel.SpaceInstallation, null).Message;
                            if (CheckSpace != "Success")
                            {
                                return new Response<ObjectInstAtts>(true, null, null, CheckSpace, (int)Helpers.Constants.ApiReturnCode.fail);
                            }
                        }
                        //Check Validations
                        string CheckDependencyValidation = CheckDependencyValidationForCivilTypes(CivilInstallationViewModel, TableName, SiteCode, null);

                        if (!string.IsNullOrEmpty(CheckDependencyValidation))
                            return new Response<ObjectInstAtts>(true, null, null, CheckDependencyValidation, (int)Helpers.Constants.ApiReturnCode.fail);

                        string CheckGeneralValidation = CheckGeneralValidationFunction(AddCivilNonSteel.TLIdynamicAttInstValue, TableName);

                        if (!string.IsNullOrEmpty(CheckGeneralValidation))
                            return new Response<ObjectInstAtts>(true, null, null, CheckGeneralValidation, (int)Helpers.Constants.ApiReturnCode.fail);

                        bool test = true;
                        if (test == true)
                        {
                            //Check if there is other recodes has the same name
                            TLIcivilSiteDate CheckName = _unitOfWork.CivilSiteDateRepository.GetIncludeWhereFirst(x => !x.Dismantle && !x.allCivilInst.Draft &&
                                (x.allCivilInst.civilNonSteelId != null ? x.allCivilInst.civilNonSteel.Name.ToLower() == CivilNonSteel.Name.ToLower() : false &&
                                x.SiteCode.ToLower() == SiteCode.ToLower()),
                                x => x.allCivilInst, x => x.allCivilInst.civilNonSteel);

                            //if CheckName is true then return error message that the name is already exists
                            if (CheckName != null)
                            {
                                return new Response<ObjectInstAtts>(true, null, null, $"The name {CivilNonSteel.Name} is already exists", (int)Helpers.Constants.ApiReturnCode.fail);
                            }
                            //add Entity to database
                            _unitOfWork.CivilNonSteelRepository.AddWithHistory(Helpers.LogFilterAttribute.UserId, CivilNonSteel);
                            _unitOfWork.SaveChanges();
                            //Add to Civil_Site_Date if there is free space
                            TLIallCivilInst allCivilInst = new TLIallCivilInst();
                            allCivilInst.civilNonSteelId = CivilNonSteel.Id;
                            if (AddCivilNonSteel.ticketAtt != null)
                            {
                                allCivilInst.Draft = false;
                                allCivilInst.TicketId = AddCivilNonSteel.ticketAtt.TicketId;
                                allCivilInst.ItemStatusId = AddCivilNonSteel.ticketAtt.ItemsStatusId;
                            }
                            _unitOfWork.AllCivilInstRepository.Add(allCivilInst);
                            _unitOfWork.SaveChanges();
                            allCivilInstId = allCivilInst.Id;
                            //if Site object is not null then add record to TLIcivilSiteDate table 
                            if (AddCivilNonSteel.TLIcivilSiteDate != null)
                            {
                                TLIcivilSiteDate civilSiteDate = new TLIcivilSiteDate();
                                civilSiteDate.SiteCode = SiteCode;
                                civilSiteDate.allCivilInstId = allCivilInst.Id;
                                civilSiteDate.InstallationDate = AddCivilNonSteel.TLIcivilSiteDate.InstallationDate;
                                civilSiteDate.LongitudinalSpindleLengthm = AddCivilNonSteel.TLIcivilSiteDate.LongitudinalSpindleLengthm;
                                civilSiteDate.HorizontalSpindleLengthm = AddCivilNonSteel.TLIcivilSiteDate.HorizontalSpindleLengthm;
                                civilSiteDate.ReservedSpace = AddCivilNonSteel.TLIcivilSiteDate.ReservedSpace;
                                _unitOfWork.CivilSiteDateRepository.Add(civilSiteDate);
                                _unitOfWork.SaveChanges();

                            }
                            //Check if there are other civils in that site
                            var SiteCivils = _unitOfWork.CivilSiteDateRepository.GetWhere(x =>
                                x.SiteCode == SiteCode && x.allCivilInst.civilNonSteelId != null).ToList();
                            //check if there are other civils in that site and CivilSupportDistance not null
                            if (SiteCivils.Count > 0 && AddCivilNonSteel.CivilSupportDistance != null)
                            {
                                TLIcivilSupportDistance civilSupportDistance = new TLIcivilSupportDistance();
                                civilSupportDistance.Distance = AddCivilNonSteel.CivilSupportDistance.Distance;
                                civilSupportDistance.Azimuth = AddCivilNonSteel.CivilSupportDistance.Azimuth;
                                civilSupportDistance.SiteCode = SiteCode;
                                civilSupportDistance.ReferenceCivilId = AddCivilNonSteel.CivilSupportDistance.ReferenceCivilId;
                                civilSupportDistance.CivilInstId = allCivilInst.Id;
                                _unitOfWork.CivilSupportDistanceRepository.Add(civilSupportDistance);
                                _unitOfWork.SaveChanges();
                            }
                            //if ReservedSpace is true then update ReservedSpace in Sited table
                            //if (AddCivilNonSteel.TLIcivilSiteDate.ReservedSpace == true)
                            //{
                            //    _unitOfWork.SiteRepository.UpdateReservedSpace(SiteCode, AddCivilNonSteel.SpaceInstallation);
                            //}
                            //Add Dynamic Attributes
                            if (AddCivilNonSteel.TLIdynamicAttInstValue.Count > 0)
                            {
                                foreach (var addDynamicAttsInstValue in AddCivilNonSteel.TLIdynamicAttInstValue)
                                {
                                    _unitOfWork.DynamicAttInstValueRepository.AddDynamicInstAtts(addDynamicAttsInstValue, TableNameEntity.Id, CivilNonSteel.Id);
                                }
                            }
                            //AddCivilHistory(AddCivilNonSteel.ticketAtt, allCivilInstId, "Insert");
                        }
                        else
                        {
                            return new Response<ObjectInstAtts>(true, null, null, ErrorMessage, (int)Helpers.Constants.ApiReturnCode.fail);
                        }
                    }
                    transaction.Complete();
                    return new Response<ObjectInstAtts>();
                }
                catch (Exception err)
                {
                    return new Response<ObjectInstAtts>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
                }
            }
            //}
        }
        //Function accept two Parameters 
        //First CivilInstallationViewModel object ViewModel of civil type
        //Second CivilType to specify any civil type deal with
        public async Task<Response<ObjectInstAtts>> EditCivilInstallation(object CivilInstallationViewModel, string CivilType)
        {
            using (TransactionScope transaction = new TransactionScope())
            {
             
                try
                {
                    int cid = 0;

                    int TableNameId = 0;
                    if (Helpers.Constants.CivilType.TLIcivilWithLegs.ToString() == CivilType)
                    {
                        TableNameId = _unitOfWork.TablesNamesRepository.GetWhereSelectFirst(x => x.TableName == "TLIcivilWithLegs", x => new { x.Id }).Id;
                        //float SiteCode = 0;
                        //Map from object to ViewModel
                        EditCivilWithLegsViewModel civilWithLegs = _mapper.Map<EditCivilWithLegsViewModel>(CivilInstallationViewModel);
                        //Map from ViewModel to Entity
                        TLIcivilWithLegs civilWithLegsEntity = _mapper.Map<TLIcivilWithLegs>(civilWithLegs);
                        //Check if there is any recorde have the same name and different Id
                        //if yes return true
                        //else return false
                        var CivilWithLegInst = _unitOfWork.CivilWithLegsRepository.GetAllAsQueryable().AsNoTracking().FirstOrDefault(x => x.Id == civilWithLegs.Id);
                        
                        string SiteCode = _unitOfWork.CivilSiteDateRepository.GetIncludeWhereFirst(x => x.allCivilInst != null ?
                         ((x.allCivilInst.civilWithLegsId != null ? x.allCivilInst.civilWithLegsId == civilWithLegs.Id : false) &&
                             !x.Dismantle && !x.allCivilInst.Draft) : false, x => x.allCivilInst).SiteCode;

                        TLIcivilSiteDate CheckName = _unitOfWork.CivilSiteDateRepository.GetWhereAndInclude(x => x.allCivilInst.civilWithLegs.Id != civilWithLegsEntity.Id &&
                        !x.Dismantle && !x.allCivilInst.Draft &&
                            (x.allCivilInst.civilWithLegsId != null ? x.allCivilInst.civilWithLegs.Name.ToLower() == civilWithLegs.Name.ToLower() : false
                            &&
                            x.SiteCode.ToLower() == SiteCode.ToLower()),
                            x => x.allCivilInst, x => x.allCivilInst.civilWithLegs).FirstOrDefault();

                        //if CheckName is true then return error message that the name is already exists 
                        if (CheckName != null)
                            return new Response<ObjectInstAtts>(true, null, null, $"The Name {civilWithLegsEntity.Name} is already exists", (int)Helpers.Constants.ApiReturnCode.fail);

                        if (civilWithLegsEntity.HeightImplemented != CivilWithLegInst.HeightImplemented && civilWithLegs.TLIcivilSiteDate.ReservedSpace == true)
                        {
                            var allcivilinst = _dbContext.TLIallCivilInst.Where(x => x.civilWithLegsId == civilWithLegsEntity.Id).Select(x => x.Id).FirstOrDefault();
                            var civilloads = _dbContext.TLIcivilLoads.Where(x => x.allCivilInstId == allcivilinst && x.ReservedSpace == true && x.Dismantle == false).Select(x => x.allLoadInstId).ToList();
                            civilWithLegsEntity.CurrentLoads = 0;
                            foreach (var civilload in civilloads)
                            {
                                var allloadinst = _dbContext.TLIallLoadInst.Where(x => x.Id == civilload).Include(x => x.mwBU).Include(x => x.mwDish).Include(x => x.mwODU).
                                    Include(x => x.mwOther).Include(x => x.mwRFU).Include(x => x.radioAntenna).Include(x => x.radioRRU).Include(x => x.radioOther).
                                    Include(x => x.power).Include(x => x.loadOther).FirstOrDefault();
                                if (allloadinst.mwBUId != null)
                                {
                                    TLImwBU tLImwBU = allloadinst.mwBU;
                                    tLImwBU.EquivalentSpace = tLImwBU.SpaceInstallation * (tLImwBU.CenterHigh / (float)civilWithLegsEntity.HeightImplemented);
                                    _dbContext.TLImwBU.Update(tLImwBU);
                                    civilWithLegsEntity.CurrentLoads = civilWithLegsEntity.CurrentLoads + tLImwBU.EquivalentSpace;
                                }
                                if (allloadinst.mwRFUId != null)
                                {
                                    TLImwRFU tLImwRFU = allloadinst.mwRFU;
                                    tLImwRFU.EquivalentSpace = tLImwRFU.SpaceInstallation * (tLImwRFU.CenterHigh / (float)civilWithLegsEntity.HeightImplemented);
                                    _dbContext.TLImwRFU.Update(tLImwRFU);
                                    civilWithLegsEntity.CurrentLoads = civilWithLegsEntity.CurrentLoads + tLImwRFU.EquivalentSpace;
                                }
                                if (allloadinst.mwDishId != null)
                                {
                                    TLImwDish tLImwDish = allloadinst.mwDish;
                                    tLImwDish.EquivalentSpace = tLImwDish.SpaceInstallation * (tLImwDish.CenterHigh / (float)civilWithLegsEntity.HeightImplemented);
                                    _dbContext.TLImwDish.Update(tLImwDish);
                                    civilWithLegsEntity.CurrentLoads = civilWithLegsEntity.CurrentLoads + tLImwDish.EquivalentSpace;
                                }
                                if (allloadinst.mwODUId != null)
                                {
                                    TLImwODU tLImwODU = allloadinst.mwODU;
                                    tLImwODU.EquivalentSpace = tLImwODU.SpaceInstallation * (tLImwODU.CenterHigh / (float)civilWithLegsEntity.HeightImplemented);
                                    _dbContext.TLImwODU.Update(tLImwODU);
                                    civilWithLegsEntity.CurrentLoads = civilWithLegsEntity.CurrentLoads + tLImwODU.EquivalentSpace;
                                }
                                if (allloadinst.mwOtherId != null)
                                {
                                    TLImwOther tLImwOther = allloadinst.mwOther;
                                    tLImwOther.EquivalentSpace = tLImwOther.Spaceinstallation * (tLImwOther.CenterHigh / (float)civilWithLegsEntity.HeightImplemented);
                                    _dbContext.TLImwOther.Update(tLImwOther);
                                    civilWithLegsEntity.CurrentLoads = civilWithLegsEntity.CurrentLoads + tLImwOther.EquivalentSpace;
                                }
                                if (allloadinst.radioAntennaId != null)
                                {
                                    TLIradioAntenna tLIradioAntenna = allloadinst.radioAntenna;
                                    tLIradioAntenna.EquivalentSpace = tLIradioAntenna.SpaceInstallation * (tLIradioAntenna.CenterHigh / (float)civilWithLegsEntity.HeightImplemented);
                                    _dbContext.TLIradioAntenna.Update(tLIradioAntenna);
                                    civilWithLegsEntity.CurrentLoads = civilWithLegsEntity.CurrentLoads + tLIradioAntenna.EquivalentSpace;
                                }
                                if (allloadinst.radioRRUId != null)
                                {
                                    TLIRadioRRU tLIRadioRRU = allloadinst.radioRRU;
                                    tLIRadioRRU.EquivalentSpace = tLIRadioRRU.SpaceInstallation * (tLIRadioRRU.CenterHigh / (float)civilWithLegsEntity.HeightImplemented);
                                    _dbContext.TLIRadioRRU.Update(tLIRadioRRU);
                                    civilWithLegsEntity.CurrentLoads = civilWithLegsEntity.CurrentLoads + tLIRadioRRU.EquivalentSpace;
                                }
                                if (allloadinst.radioOtherId != null)
                                {
                                    TLIradioOther tLIradioOther = allloadinst.radioOther;
                                    tLIradioOther.EquivalentSpace = tLIradioOther.Spaceinstallation * (tLIradioOther.CenterHigh / (float)civilWithLegsEntity.HeightImplemented);
                                    _dbContext.TLIradioOther.Update(tLIradioOther);
                                    civilWithLegsEntity.CurrentLoads = civilWithLegsEntity.CurrentLoads + tLIradioOther.EquivalentSpace;
                                }
                                if (allloadinst.powerId != null)
                                {
                                    TLIpower tLIpower = allloadinst.power;
                                    tLIpower.EquivalentSpace = tLIpower.SpaceInstallation * (tLIpower.CenterHigh / (float)civilWithLegsEntity.HeightImplemented);
                                    _dbContext.TLIpower.Update(tLIpower);
                                    civilWithLegsEntity.CurrentLoads = civilWithLegsEntity.CurrentLoads + tLIpower.EquivalentSpace;
                                }
                                if (allloadinst.loadOtherId != null)
                                {
                                    TLIloadOther tLIloadOther = allloadinst.loadOther;
                                    tLIloadOther.EquivalentSpace = tLIloadOther.SpaceInstallation * (tLIloadOther.CenterHigh / (float)civilWithLegsEntity.HeightImplemented);
                                    _dbContext.TLIloadOther.Update(tLIloadOther);
                                    civilWithLegsEntity.CurrentLoads = civilWithLegsEntity.CurrentLoads + tLIloadOther.EquivalentSpace;
                                }

                            }

                        }
                        if (civilWithLegsEntity.SpaceInstallation != CivilWithLegInst.SpaceInstallation && civilWithLegs.TLIcivilSiteDate.ReservedSpace == true)
                        {
                            var allcivil = _dbContext.TLIallCivilInst.Where(x => x.civilWithLegsId == CivilWithLegInst.Id).Select(x => x.Id).FirstOrDefault();
                            var sitescode = _dbContext.TLIcivilSiteDate.Where(x => x.allCivilInstId == allcivil).Select(x => x.SiteCode).FirstOrDefault();
                            var Site = _dbContext.TLIsite.Where(x => x.SiteCode == sitescode).FirstOrDefault();
                            Site.ReservedSpace = Site.ReservedSpace - CivilWithLegInst.SpaceInstallation;
                            Site.ReservedSpace = Site.ReservedSpace + civilWithLegsEntity.SpaceInstallation;
                            _dbContext.SaveChanges();
                        }
                        string CheckGeneralValidationFunction = CheckGeneralValidationFunctionEditVersion(civilWithLegs.DynamicInstAttsValue, CivilType, null);

                        if (!string.IsNullOrEmpty(CheckGeneralValidationFunction))
                            return new Response<ObjectInstAtts>(true, null, null, CheckGeneralValidationFunction, (int)Helpers.Constants.ApiReturnCode.fail);

                        string CheckDependencyValidation = CheckDependencyValidationForCivilTypesEditVersion(CivilInstallationViewModel, CivilType, SiteCode, null);

                        if (!string.IsNullOrEmpty(CheckDependencyValidation))
                            return new Response<ObjectInstAtts>(true, null, null, CheckDependencyValidation, (int)Helpers.Constants.ApiReturnCode.fail);
                        TLIcivilWithLegs civilWithLegsEntity1 = _mapper.Map<TLIcivilWithLegs>(civilWithLegs);
                        //Check if there is any recorde have the same name and different Id
                        //if yes return true
                        //else return false
                        //if CheckName is false then update civilWithLegsEntity
                        _unitOfWork.CivilWithLegsRepository.UpdateWithHistory(Helpers.LogFilterAttribute.UserId, CivilWithLegInst, civilWithLegsEntity1);
                        var civilsitedate = _unitOfWork.CivilSiteDateRepository.GetIncludeWhereFirst(x => x.allCivilInst.civilWithLegsId == civilWithLegs.Id);
                        civilsitedate.LongitudinalSpindleLengthm = civilWithLegs.TLIcivilSiteDate.LongitudinalSpindleLengthm;
                        civilsitedate.HorizontalSpindleLengthm = civilWithLegs.TLIcivilSiteDate.HorizontalSpindleLengthm;
                        civilsitedate.ReservedSpace = civilWithLegs.TLIcivilSiteDate.ReservedSpace;
                        civilsitedate.Dismantle = civilWithLegs.TLIcivilSiteDate.Dismantle;
                        civilsitedate.InstallationDate = civilWithLegs.TLIcivilSiteDate.InstallationDate;
                        _unitOfWork.SaveChanges();
                        var allcivilinstId = _unitOfWork.AllCivilInstRepository.GetWhereFirst(x => x.civilWithLegsId == civilWithLegs.Id).Id;
                        var civilsupportdistance = _unitOfWork.CivilSupportDistanceRepository.GetWhereFirst(x => x.CivilInstId == allcivilinstId);
                        civilsupportdistance.Azimuth = civilWithLegs.TLIcivilSupportDistance.Azimuth;
                        civilsupportdistance.Distance = civilWithLegs.TLIcivilSupportDistance.Distance;
                        civilsupportdistance.ReferenceCivilId = civilWithLegs.TLIcivilSupportDistance.ReferenceCivilId;
                        _unitOfWork.SaveChanges();
                        if (civilWithLegs.DynamicInstAttsValue.Count > 0)
                        {
                            _unitOfWork.DynamicAttInstValueRepository.UpdateDynamicValue(civilWithLegs.DynamicInstAttsValue, TableNameId, civilWithLegsEntity.Id);
                        }

                        //List<TLIleg> Legs = _mapper.Map<List<TLIleg>>(civilWithLegs.Legs);
                        //_unitOfWork.LegRepository.UpdateRange(Legs);
                        //_unitOfWork.SaveChanges();

                        await _unitOfWork.SaveChangesAsync();
                    }
                    else if (Helpers.Constants.CivilType.TLIcivilWithoutLeg.ToString() == CivilType)
                    {
                        TableNameId = _unitOfWork.TablesNamesRepository.GetWhereSelectFirst(x => x.TableName == "TLIcivilWithoutLeg", x => new { x.Id }).Id;

                        //Map from object to ViewModel
                        EditCivilWithoutLegViewModel civilWithoutLegs = _mapper.Map<EditCivilWithoutLegViewModel>(CivilInstallationViewModel);
                        //Map from ViewModel to Entity
                        TLIcivilWithoutLeg civilWithoutLegsEntity = _mapper.Map<TLIcivilWithoutLeg>(civilWithoutLegs);
                        //Check if there is any recorde have the same name and different Id 
                        //if yes return true
                        //else return false

                        TLIcivilWithoutLeg CivilWithOutLegInst = _unitOfWork.CivilWithoutLegRepository.GetAllAsQueryable()
                            .AsNoTracking().FirstOrDefault(x => x.Id == civilWithoutLegs.Id);

                        TLIcivilWithoutLegLibrary CheckCivilWithoutLegLibrary = _unitOfWork.CivilWithoutLegLibraryRepository
                            .GetByID(civilWithoutLegs.CivilWithoutlegsLibId);

                        string SiteCode = _unitOfWork.CivilSiteDateRepository.GetIncludeWhereFirst(x => x.allCivilInst != null ?
                            ((x.allCivilInst.civilWithoutLegId != null ? x.allCivilInst.civilWithoutLegId == civilWithoutLegs.Id : false) &&
                                !x.Dismantle && !x.allCivilInst.Draft) : false, x => x.allCivilInst).SiteCode;

                        TLIcivilSiteDate CheckName = _unitOfWork.CivilSiteDateRepository.GetWhereAndInclude(x => x.allCivilInst.civilWithoutLeg.Id != civilWithoutLegsEntity.Id &&
                        !x.Dismantle && !x.allCivilInst.Draft &&
                            (x.allCivilInst.civilWithoutLegId != null ? x.allCivilInst.civilWithoutLeg.Name.ToLower() == civilWithoutLegs.Name.ToLower() : false
                            &&
                            x.SiteCode.ToLower() == SiteCode.ToLower()),
                            x => x.allCivilInst, x => x.allCivilInst.civilWithoutLeg,x=>x.allCivilInst.civilWithoutLeg.CivilWithoutlegsLib).FirstOrDefault();

                        //if CheckName is true then return error message that the name is already exists 
                        if (CheckName != null)
                        {
                            if (CheckName.allCivilInst.civilWithoutLeg.CivilWithoutlegsLib.CivilWithoutLegCategoryId == CheckCivilWithoutLegLibrary.CivilWithoutLegCategoryId)
                                return new Response<ObjectInstAtts>(true, null, null, $"The Name {civilWithoutLegsEntity.Name} is already exists", (int)Helpers.Constants.ApiReturnCode.fail);
                        }
                        if (civilWithoutLegsEntity.HeightImplemented != CivilWithOutLegInst.HeightImplemented && civilWithoutLegs.TLIcivilSiteDate.ReservedSpace == true)
                        {
                            //TLIallCivilInst allcivilinst = _unitOfWork.AllCivilInstRepository.GetWhereFirst(x => x.civilWithoutLegId != null ? 
                            //    x.civilWithoutLegId.Value == civilWithoutLegsEntity.Id : false);

                            var allcivil = _dbContext.TLIallCivilInst.ToList();

                            foreach (var s in allcivil)
                            {
                                if (s.civilWithoutLegId == civilWithoutLegsEntity.Id)
                                {
                                    cid = s.Id;
                                }
                            }


                            var civilloads = _dbContext.TLIcivilLoads.Where(x => x.allCivilInstId == cid && x.Dismantle == false).Select(x => x.allLoadInstId).ToList();
                            civilWithoutLegsEntity.CurrentLoads = 0;
                            foreach (var civilload in civilloads)
                            {
                                var allloadinst = _dbContext.TLIallLoadInst.Where(x => x.Id == civilload).Include(x => x.mwBU).Include(x => x.mwDish).Include(x => x.mwODU).
                                    Include(x => x.mwOther).Include(x => x.mwRFU).Include(x => x.radioAntenna).Include(x => x.radioRRU).Include(x => x.radioOther).
                                    Include(x => x.power).Include(x => x.loadOther).FirstOrDefault();
                                if (allloadinst.mwBUId != null)
                                {
                                    TLImwBU tLImwBU = allloadinst.mwBU;
                                    tLImwBU.EquivalentSpace = tLImwBU.SpaceInstallation * (tLImwBU.CenterHigh / (float)civilWithoutLegsEntity.HeightImplemented);
                                    _dbContext.TLImwBU.Update(tLImwBU);
                                    civilWithoutLegsEntity.CurrentLoads = civilWithoutLegsEntity.CurrentLoads + tLImwBU.EquivalentSpace;
                                }
                                if (allloadinst.mwRFUId != null)
                                {
                                    TLImwRFU tLImwRFU = allloadinst.mwRFU;
                                    tLImwRFU.EquivalentSpace = tLImwRFU.SpaceInstallation * (tLImwRFU.CenterHigh / (float)civilWithoutLegsEntity.HeightImplemented);
                                    _dbContext.TLImwRFU.Update(tLImwRFU);
                                    civilWithoutLegsEntity.CurrentLoads = civilWithoutLegsEntity.CurrentLoads + tLImwRFU.EquivalentSpace;
                                }
                                if (allloadinst.mwDishId != null)
                                {
                                    TLImwDish tLImwDish = allloadinst.mwDish;
                                    tLImwDish.EquivalentSpace = tLImwDish.SpaceInstallation * (tLImwDish.CenterHigh / (float)civilWithoutLegsEntity.HeightImplemented);
                                    _dbContext.TLImwDish.Update(tLImwDish);
                                    civilWithoutLegsEntity.CurrentLoads = civilWithoutLegsEntity.CurrentLoads + tLImwDish.EquivalentSpace;
                                }
                                if (allloadinst.mwODUId != null)
                                {
                                    TLImwODU tLImwODU = allloadinst.mwODU;
                                    tLImwODU.EquivalentSpace = tLImwODU.SpaceInstallation * (tLImwODU.CenterHigh / (float)civilWithoutLegsEntity.HeightImplemented);
                                    _dbContext.TLImwODU.Update(tLImwODU);
                                    civilWithoutLegsEntity.CurrentLoads = civilWithoutLegsEntity.CurrentLoads + tLImwODU.EquivalentSpace;
                                }
                                if (allloadinst.mwOtherId != null)
                                {
                                    TLImwOther tLImwOther = allloadinst.mwOther;
                                    tLImwOther.EquivalentSpace = tLImwOther.Spaceinstallation * (tLImwOther.CenterHigh / (float)civilWithoutLegsEntity.HeightImplemented);
                                    _dbContext.TLImwOther.Update(tLImwOther);
                                    civilWithoutLegsEntity.CurrentLoads = civilWithoutLegsEntity.CurrentLoads + tLImwOther.EquivalentSpace;
                                }
                                if (allloadinst.radioAntennaId != null)
                                {
                                    TLIradioAntenna tLIradioAntenna = allloadinst.radioAntenna;
                                    tLIradioAntenna.EquivalentSpace = tLIradioAntenna.SpaceInstallation * (tLIradioAntenna.CenterHigh / (float)civilWithoutLegsEntity.HeightImplemented);
                                    _dbContext.TLIradioAntenna.Update(tLIradioAntenna);
                                    civilWithoutLegsEntity.CurrentLoads = civilWithoutLegsEntity.CurrentLoads + tLIradioAntenna.EquivalentSpace;
                                }
                                if (allloadinst.radioRRUId != null)
                                {
                                    TLIRadioRRU tLIRadioRRU = allloadinst.radioRRU;
                                    tLIRadioRRU.EquivalentSpace = tLIRadioRRU.SpaceInstallation * (tLIRadioRRU.CenterHigh / (float)civilWithoutLegsEntity.HeightImplemented);
                                    _dbContext.TLIRadioRRU.Update(tLIRadioRRU);
                                    civilWithoutLegsEntity.CurrentLoads = civilWithoutLegsEntity.CurrentLoads + tLIRadioRRU.EquivalentSpace;
                                }
                                if (allloadinst.radioOtherId != null)
                                {
                                    TLIradioOther tLIradioOther = allloadinst.radioOther;
                                    tLIradioOther.EquivalentSpace = tLIradioOther.Spaceinstallation * (tLIradioOther.CenterHigh / (float)civilWithoutLegsEntity.HeightImplemented);
                                    _dbContext.TLIradioOther.Update(tLIradioOther);
                                    civilWithoutLegsEntity.CurrentLoads = civilWithoutLegsEntity.CurrentLoads + tLIradioOther.EquivalentSpace;
                                }
                                if (allloadinst.powerId != null)
                                {
                                    TLIpower tLIpower = allloadinst.power;
                                    tLIpower.EquivalentSpace = tLIpower.SpaceInstallation * (tLIpower.CenterHigh / (float)civilWithoutLegsEntity.HeightImplemented);
                                    _dbContext.TLIpower.Update(tLIpower);
                                    civilWithoutLegsEntity.CurrentLoads = civilWithoutLegsEntity.CurrentLoads + tLIpower.EquivalentSpace;
                                }
                                if (allloadinst.loadOtherId != null)
                                {
                                    TLIloadOther tLIloadOther = allloadinst.loadOther;
                                    tLIloadOther.EquivalentSpace = tLIloadOther.SpaceInstallation * (tLIloadOther.CenterHigh / (float)civilWithoutLegsEntity.HeightImplemented);
                                    _dbContext.TLIloadOther.Update(tLIloadOther);
                                    civilWithoutLegsEntity.CurrentLoads = civilWithoutLegsEntity.CurrentLoads + tLIloadOther.EquivalentSpace;
                                }

                            }

                        }
                        if (civilWithoutLegsEntity.SpaceInstallation != CivilWithOutLegInst.SpaceInstallation && civilWithoutLegs.TLIcivilSiteDate.ReservedSpace == true)
                        {
                            //var allcivil = _dbContext.TLIallCivilInst.Where(x => x.civilWithoutLegId == cid).Select(x => x.Id).FirstOrDefault();
                            var allcivil = _dbContext.TLIallCivilInst.ToList();

                            foreach (var s in allcivil)
                            {
                                if (s.civilWithoutLegId == civilWithoutLegsEntity.Id)
                                {
                                    cid = s.Id;
                                }
                            }
                            var sitescode = _dbContext.TLIcivilSiteDate.Where(x => x.allCivilInstId == cid).Select(x => x.SiteCode).FirstOrDefault();
                            var Site = _dbContext.TLIsite.Where(x => x.SiteCode == sitescode).FirstOrDefault();
                            Site.ReservedSpace = Site.ReservedSpace - CivilWithOutLegInst.SpaceInstallation;
                            Site.ReservedSpace = Site.ReservedSpace + civilWithoutLegsEntity.SpaceInstallation;
                            _dbContext.SaveChanges();
                        }
                        TLIcivilWithoutLegLibrary CivilWithoutLegLibrary = _unitOfWork.CivilWithoutLegLibraryRepository.GetByID(civilWithoutLegs.CivilWithoutlegsLibId);

                        string CheckGeneralValidationFunction = CheckGeneralValidationFunctionEditVersion(civilWithoutLegs.DynamicInstAttsValue, CivilType, CivilWithoutLegLibrary.CivilWithoutLegCategoryId);

                        if (!string.IsNullOrEmpty(CheckGeneralValidationFunction))
                            return new Response<ObjectInstAtts>(true, null, null, CheckGeneralValidationFunction, (int)Helpers.Constants.ApiReturnCode.fail);

                        string CheckDependencyValidation = CheckDependencyValidationForCivilTypesEditVersion(CivilInstallationViewModel, CivilType, SiteCode, CivilWithoutLegLibrary.CivilWithoutLegCategoryId);

                        if (!string.IsNullOrEmpty(CheckDependencyValidation))
                            return new Response<ObjectInstAtts>(true, null, null, CheckDependencyValidation, (int)Helpers.Constants.ApiReturnCode.fail);

                        //if CheckName is false then update civilWithoutLegsEntity
                        _unitOfWork.CivilWithoutLegRepository.UpdateWithHistory(Helpers.LogFilterAttribute.UserId, CivilWithOutLegInst, civilWithoutLegsEntity);
                        var civilsitedate = _unitOfWork.CivilSiteDateRepository.GetIncludeWhereFirst(x => x.allCivilInst.civilWithoutLegId == civilWithoutLegs.Id);
                        civilsitedate.LongitudinalSpindleLengthm = civilWithoutLegs.TLIcivilSiteDate.LongitudinalSpindleLengthm;
                        civilsitedate.HorizontalSpindleLengthm = civilWithoutLegs.TLIcivilSiteDate.HorizontalSpindleLengthm;
                        civilsitedate.ReservedSpace = civilWithoutLegs.TLIcivilSiteDate.ReservedSpace;
                        civilsitedate.Dismantle = civilWithoutLegs.TLIcivilSiteDate.Dismantle;
                        civilsitedate.InstallationDate = civilWithoutLegs.TLIcivilSiteDate.InstallationDate;
                        _unitOfWork.SaveChanges();
                        var allcivilinstId = _unitOfWork.AllCivilInstRepository.GetWhereFirst(x => x.civilWithoutLegId == civilWithoutLegs.Id).Id;
                        var civilsupportdistance = _unitOfWork.CivilSupportDistanceRepository.GetWhereFirst(x => x.CivilInstId == allcivilinstId);
                        civilsupportdistance.Azimuth = civilsupportdistance != null ? civilWithoutLegs.TLIcivilSupportDistance.Azimuth : 0;
                        civilsupportdistance.Distance = civilsupportdistance != null ? civilWithoutLegs.TLIcivilSupportDistance.Distance : 0;
                        civilsupportdistance.ReferenceCivilId = civilsupportdistance != null ? civilWithoutLegs.TLIcivilSupportDistance.ReferenceCivilId : 0;
                        _unitOfWork.SaveChanges();
                        if (civilWithoutLegs.DynamicInstAttsValue.Count > 0)
                        {
                            _unitOfWork.DynamicAttInstValueRepository.UpdateDynamicValue(civilWithoutLegs.DynamicInstAttsValue, TableNameId, civilWithoutLegsEntity.Id);
                        }
                        await _unitOfWork.SaveChangesAsync();
                    }
                    else if (Helpers.Constants.CivilType.TLIcivilNonSteel.ToString() == CivilType)
                    {
                        TableNameId = _unitOfWork.TablesNamesRepository.GetWhereSelectFirst(x => x.TableName == "TLIcivilNonSteel", x => new { x.Id }).Id;

                        //Map from object to ViewModel
                        EditCivilNonSteelViewModel civilNonSteel = _mapper.Map<EditCivilNonSteelViewModel>(CivilInstallationViewModel);
                        //Map from ViewModel to Entity
                        TLIcivilNonSteel civilNonSteelEntity = _mapper.Map<TLIcivilNonSteel>(civilNonSteel);
                        //Check if there is any recorde have the same name and different Id 
                        //if yes return true
                        //else return false
                        var CivilNonSteelInst = _unitOfWork.CivilNonSteelRepository
                            .GetAllAsQueryable().AsNoTracking().FirstOrDefault(x => x.Id == civilNonSteel.Id);

                        string SiteCode = _unitOfWork.CivilSiteDateRepository.GetIncludeWhereFirst(x => x.allCivilInst != null ?
                            ((x.allCivilInst.civilNonSteelId != null ? x.allCivilInst.civilNonSteelId == civilNonSteel.Id : false) &&
                                !x.Dismantle && !x.allCivilInst.Draft) : false, x => x.allCivilInst).SiteCode;

                        TLIcivilSiteDate CheckName = _unitOfWork.CivilSiteDateRepository.GetWhereAndInclude(x => x.allCivilInst.civilNonSteel.Id != civilNonSteelEntity.Id &&
                        !x.Dismantle && !x.allCivilInst.Draft &&
                            (x.allCivilInst.civilNonSteelId != null ? x.allCivilInst.civilNonSteel.Name.ToLower() == civilNonSteel.Name.ToLower() : false
                            && 
                            x.SiteCode.ToLower() == SiteCode.ToLower()),
                            x => x.allCivilInst, x => x.allCivilInst.civilNonSteel).FirstOrDefault();

                        //if CheckName is true then return error message that the name is already exists
                        if (CheckName != null)
                            return new Response<ObjectInstAtts>(true, null, null, $"The Name {civilNonSteelEntity.Name} is already exists", (int)Helpers.Constants.ApiReturnCode.fail);

                        string CheckGeneralValidationFunction = CheckGeneralValidationFunctionEditVersion(civilNonSteel.DynamicInstAttsValue, CivilType, null);

                        if (!string.IsNullOrEmpty(CheckGeneralValidationFunction))
                            return new Response<ObjectInstAtts>(true, null, null, CheckGeneralValidationFunction, (int)Helpers.Constants.ApiReturnCode.fail);

                        string CheckDependencyValidation = CheckDependencyValidationForCivilTypesEditVersion(CivilInstallationViewModel, CivilType, SiteCode, null);

                        if (!string.IsNullOrEmpty(CheckDependencyValidation))
                            return new Response<ObjectInstAtts>(true, null, null, CheckDependencyValidation, (int)Helpers.Constants.ApiReturnCode.fail);

                        //if CheckName is false then update civilNonSteelEntity
                        _unitOfWork.CivilNonSteelRepository.UpdateWithHistory(Helpers.LogFilterAttribute.UserId, CivilNonSteelInst, civilNonSteelEntity);
                        var civilsitedate = _unitOfWork.CivilSiteDateRepository.GetIncludeWhereFirst(x => x.allCivilInst.civilNonSteelId == civilNonSteel.Id);
                        civilsitedate.LongitudinalSpindleLengthm = civilNonSteel.TLIcivilSiteDate.LongitudinalSpindleLengthm;
                        civilsitedate.HorizontalSpindleLengthm = civilNonSteel.TLIcivilSiteDate.HorizontalSpindleLengthm;
                        civilsitedate.ReservedSpace = civilNonSteel.TLIcivilSiteDate.ReservedSpace;
                        civilsitedate.Dismantle = civilNonSteel.TLIcivilSiteDate.Dismantle;
                        civilsitedate.InstallationDate = civilNonSteel.TLIcivilSiteDate.InstallationDate;
                        _unitOfWork.SaveChanges();
                        var allcivilinstId = _unitOfWork.AllCivilInstRepository.GetWhereFirst(x => x.civilNonSteelId == civilNonSteel.Id).Id;
                        var civilsupportdistance = _unitOfWork.CivilSupportDistanceRepository.GetWhereFirst(x => x.CivilInstId == allcivilinstId);
                        civilsupportdistance.Azimuth = civilNonSteel.TLIcivilSupportDistance.Azimuth;
                        civilsupportdistance.Distance = civilNonSteel.TLIcivilSupportDistance.Distance;
                        civilsupportdistance.ReferenceCivilId = civilNonSteel.TLIcivilSupportDistance.ReferenceCivilId;

                        _unitOfWork.SaveChanges();
                        if (civilNonSteel.DynamicInstAttsValue.Count > 0)
                        {
                            _unitOfWork.DynamicAttInstValueRepository.UpdateDynamicValue(civilNonSteel.DynamicInstAttsValue, TableNameId, civilNonSteelEntity.Id);
                        }
                        await _unitOfWork.SaveChangesAsync();
                    }
                    transaction.Complete();
                    return new Response<ObjectInstAtts>();
                }
                catch (Exception err)
                {
                    return new Response<ObjectInstAtts>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
                }
            }
        }
        #region Helper Methods For EditCivilInstallation Function..
        public string CheckDependencyValidationForCivilTypesEditVersion(object Input, string CivilType, string SiteCode, int? CategoryId)
        {
            if (CivilType.ToLower() == Helpers.Constants.TablesNames.TLIcivilWithLegs.ToString().ToLower())
            {
                string MainTableName = Helpers.Constants.TablesNames.TLIcivilWithLegs.ToString();
                EditCivilWithLegsViewModel EditCivilInstallationViewModel = _mapper.Map<EditCivilWithLegsViewModel>(Input);

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

                                    object TestValue = EditCivilInstallationViewModel.GetType().GetProperties()
                                        .FirstOrDefault(x => x.Name.ToLower() == AttributeName.ToLower()).GetValue(EditCivilInstallationViewModel, null);

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
                                    BaseInstAttView DynamicObject = EditCivilInstallationViewModel.DynamicInstAttsValue
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
                                    object CivilLoads = EditCivilInstallationViewModel.GetType().GetProperty(Path[0])
                                        .GetValue(EditCivilInstallationViewModel, null);

                                    CheckId = CivilLoads.GetType().GetProperty(Path[1]).GetValue(CivilLoads, null) != null ?
                                        (int)CivilLoads.GetType().GetProperty(Path[1]).GetValue(CivilLoads, null) : new object();
                                }
                                else if (Path.Count() == 1 && Path[0].ToLower() == "sitecode")
                                {
                                    CheckId = SiteCode;
                                }
                                else if (Path.Count() == 1)
                                {
                                    if (EditCivilInstallationViewModel.GetType().GetProperty(Path[0]).GetValue(EditCivilInstallationViewModel, null) != null)
                                        CheckId = (int)EditCivilInstallationViewModel.GetType().GetProperty(Path[0])
                                            .GetValue(EditCivilInstallationViewModel, null);
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

                            BaseInstAttView InputDynamicAttribute = EditCivilInstallationViewModel.DynamicInstAttsValue
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
            else if (CivilType.ToLower() == Helpers.Constants.TablesNames.TLIcivilWithoutLeg.ToString().ToLower())
            {
                string MainTableName = Helpers.Constants.TablesNames.TLIcivilWithoutLeg.ToString();
                EditCivilWithoutLegViewModel EditCivilInstallationViewModel = _mapper.Map<EditCivilWithoutLegViewModel>(Input);

                List<DynamicAttViewModel> DynamicAttributes = _mapper.Map<List<DynamicAttViewModel>>(_unitOfWork.DynamicAttRepository
                    .GetIncludeWhere(x => x.tablesNames.TableName.ToLower() == MainTableName.ToLower() && !x.disable &&
                        ((x.CivilWithoutLegCategoryId != null && CategoryId != null) ? x.CivilWithoutLegCategoryId == CategoryId : false), x => x.tablesNames).ToList());

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

                                    object TestValue = EditCivilInstallationViewModel.GetType().GetProperties()
                                        .FirstOrDefault(x => x.Name.ToLower() == AttributeName.ToLower()).GetValue(EditCivilInstallationViewModel, null);

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
                                    BaseInstAttView DynamicObject = EditCivilInstallationViewModel.DynamicInstAttsValue
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
                                    object CivilLoads = EditCivilInstallationViewModel.GetType().GetProperty(Path[0])
                                        .GetValue(EditCivilInstallationViewModel, null);

                                    CheckId = CivilLoads.GetType().GetProperty(Path[1]).GetValue(CivilLoads, null) != null ?
                                        (int)CivilLoads.GetType().GetProperty(Path[1]).GetValue(CivilLoads, null) : new object();
                                }
                                else if (Path.Count() == 1 && Path[0].ToLower() == "sitecode")
                                {
                                    CheckId = SiteCode;
                                }
                                else if (Path.Count() == 1)
                                {
                                    if (EditCivilInstallationViewModel.GetType().GetProperty(Path[0]).GetValue(EditCivilInstallationViewModel, null) != null)
                                        CheckId = (int)EditCivilInstallationViewModel.GetType().GetProperty(Path[0])
                                            .GetValue(EditCivilInstallationViewModel, null);
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

                            BaseInstAttView InputDynamicAttribute = EditCivilInstallationViewModel.DynamicInstAttsValue
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
            else if (CivilType.ToLower() == Helpers.Constants.TablesNames.TLIcivilNonSteel.ToString().ToLower())
            {
                string MainTableName = Helpers.Constants.TablesNames.TLIcivilNonSteel.ToString();
                EditCivilNonSteelViewModel EditCivilInstallationViewModel = _mapper.Map<EditCivilNonSteelViewModel>(Input);

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

                                    object TestValue = EditCivilInstallationViewModel.GetType().GetProperties()
                                        .FirstOrDefault(x => x.Name.ToLower() == AttributeName.ToLower()).GetValue(EditCivilInstallationViewModel, null);

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
                                    BaseInstAttView DynamicObject = EditCivilInstallationViewModel.DynamicInstAttsValue
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
                                    object CivilLoads = EditCivilInstallationViewModel.GetType().GetProperty(Path[0])
                                        .GetValue(EditCivilInstallationViewModel, null);

                                    CheckId = CivilLoads.GetType().GetProperty(Path[1]).GetValue(CivilLoads, null) != null ?
                                        (int)CivilLoads.GetType().GetProperty(Path[1]).GetValue(CivilLoads, null) : new object();
                                }
                                else if (Path.Count() == 1 && Path[0].ToLower() == "sitecode")
                                {
                                    CheckId = SiteCode;
                                }
                                else if (Path.Count() == 1)
                                {
                                    if (EditCivilInstallationViewModel.GetType().GetProperty(Path[0]).GetValue(EditCivilInstallationViewModel, null) != null)
                                        CheckId = (int)EditCivilInstallationViewModel.GetType().GetProperty(Path[0])
                                            .GetValue(EditCivilInstallationViewModel, null);
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

                            BaseInstAttView InputDynamicAttribute = EditCivilInstallationViewModel.DynamicInstAttsValue
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
        public string CheckGeneralValidationFunctionEditVersion(List<BaseInstAttView> TLIdynamicAttInstValue, string TableName, int? CivilWithoutLegCategoryId = null)
        {
            List<DynamicAttViewModel> DynamicAttributes = null;

            if (CivilWithoutLegCategoryId != null)
                DynamicAttributes = _mapper.Map<List<DynamicAttViewModel>>(_unitOfWork.DynamicAttRepository
                   .GetIncludeWhere(x => x.tablesNames.TableName.ToLower() == TableName.ToLower() && !x.disable && x.CivilWithoutLegCategoryId == CivilWithoutLegCategoryId, x => x.tablesNames).ToList());

            else
                DynamicAttributes = _mapper.Map<List<DynamicAttViewModel>>(_unitOfWork.DynamicAttRepository
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
        //Function accept two parameters 
        //First CivilInsId get record by Id depened on TableName
        //Second TableName get TableName Id from TLItablesNames, get from AttributeAtts by TableName, get DynamicAtts by TableName Id
        //This Function return ObjectInstAtts List of InstActivatedAtts, List of DynamicAtts and List of RelatedTables
        public Response<ObjectInstAtts> GetById(int CivilInsId, string TableName)
        {
            try
            {
                TLItablesNames TableNameEntity = _unitOfWork.TablesNamesRepository
                    .GetWhereFirst(c => c.TableName.ToLower() == TableName.ToLower());

                ObjectInstAtts objectInst = new ObjectInstAtts();

                if (Helpers.Constants.CivilType.TLIcivilWithLegs.ToString() == TableName)
                {
                    TLIcivilWithLegs CivilWithLegsInst = _unitOfWork.CivilWithLegsRepository
                        .GetIncludeWhereFirst(x => x.Id == CivilInsId, x => x.BaseCivilWithLegType,
                            x => x.CivilWithLegsLib, x => x.enforcmentCategory, x => x.baseType,
                            x => x.GuyLineType, x => x.locationType, x => x.Owner, x => x.SupportTypeImplemented);

                    CivilWithLegLibraryViewModel CivilWithLegLibrary = _mapper.Map<CivilWithLegLibraryViewModel>(_unitOfWork.CivilWithLegLibraryRepository
                        .GetIncludeWhereFirst(x => x.Id == CivilWithLegsInst.CivilWithLegsLibId, x => x.civilSteelSupportCategory,
                            x => x.sectionsLegType, x => x.structureType, x => x.supportTypeDesigned));

                    List<BaseAttView> LibraryAttributes = _unitOfWork.AttributeActivatedRepository
                        .GetAttributeActivated(Helpers.Constants.TablesNames.TLIcivilWithLegLibrary.ToString(), CivilWithLegLibrary, null).ToList();

                    foreach (BaseAttView LibraryAttribute in LibraryAttributes)
                    {
                        if (LibraryAttribute.DataType.ToLower() == "list")
                        {
                            LibraryAttribute.Value = CivilWithLegLibrary.GetType().GetProperties()
                                .FirstOrDefault(x => x.Name.ToLower() == LibraryAttribute.Label.ToLower()).GetValue(CivilWithLegLibrary);
                        }
                    }

                    List<BaseAttView> LogisticalAttributes = _mapper.Map<List<BaseAttView>>(_unitOfWork.LogistcalRepository
                        .GetLogistical(Helpers.Constants.TablePartName.CivilSupport.ToString(), Helpers.Constants.TablesNames.TLIcivilWithLegLibrary.ToString(), CivilWithLegLibrary.Id).ToList());

                    LibraryAttributes.AddRange(LogisticalAttributes);

                    objectInst.LibraryActivatedAttributes = LibraryAttributes;

                    List<BaseInstAttView> ListAttributesActivated = _unitOfWork.AttributeActivatedRepository
                        .GetInstAttributeActivated(Helpers.Constants.TablesNames.TLIcivilWithLegs.ToString(), CivilWithLegsInst).ToList();

                    BaseInstAttView NameAttribute = ListAttributesActivated.FirstOrDefault(x => x.Key.ToLower() == "Name".ToLower());
                    if (NameAttribute != null)
                    {
                        BaseInstAttView Swap = ListAttributesActivated[0];
                        ListAttributesActivated[ListAttributesActivated.IndexOf(NameAttribute)] = Swap;
                        ListAttributesActivated[0] = NameAttribute;
                    }
                    foreach (BaseInstAttView FKitem in ListAttributesActivated)
                    {
                        if (FKitem.Desc.ToLower() == "tlilocationtype")
                        {
                            if (CivilWithLegsInst.locationType == null)
                                FKitem.Value = "NA";

                            else
                                FKitem.Value = CivilWithLegsInst.locationType.Name;
                        }
                        else if (FKitem.Desc.ToLower() == "tlibasetype")
                        {
                            if (CivilWithLegsInst.baseType == null)
                                FKitem.Value = "NA";

                            else
                                FKitem.Value = CivilWithLegsInst.baseType.Name;
                        }
                        else if (FKitem.Desc.ToLower() == "tliowner")
                        {
                            if (CivilWithLegsInst.Owner == null)
                                FKitem.Value = "NA";

                            else
                                FKitem.Value = CivilWithLegsInst.Owner.OwnerName;
                        }
                        else if (FKitem.Desc.ToLower() == "tlibasecivilwithlegstype")
                        {
                            if (CivilWithLegsInst.BaseCivilWithLegType == null)
                                FKitem.Value = "NA";

                            else
                                FKitem.Value = CivilWithLegsInst.BaseCivilWithLegType.Name;
                        }
                        else if (FKitem.Desc.ToLower() == "tliguylinetype")
                        {
                            if (CivilWithLegsInst.GuyLineType == null)
                                FKitem.Value = "NA";

                            else
                                FKitem.Value = CivilWithLegsInst.GuyLineType.Name;
                        }
                        else if (FKitem.Desc.ToLower() == "tlisupporttypeimplemented")
                        {
                            if (CivilWithLegsInst.SupportTypeImplemented == null)
                                FKitem.Value = "NA";

                            else
                                FKitem.Value = CivilWithLegsInst.SupportTypeImplemented.Name;
                        }
                        else if (FKitem.Desc.ToLower() == "tlienforcmentcategory")
                        {
                            if (CivilWithLegsInst.enforcmentCategory == null)
                                FKitem.Value = "NA";

                            else
                                FKitem.Value = CivilWithLegsInst.enforcmentCategory.Name;
                        }
                        else if (FKitem.Desc.ToLower() == "tlicivilwithleglibrary")
                        {
                            if (CivilWithLegsInst.CivilWithLegsLib == null)
                                FKitem.Value = "NA";

                            else
                                FKitem.Value = CivilWithLegsInst.CivilWithLegsLib.Model;
                        }
                        else if (FKitem.Key.ToLower() == "StructureType".ToLower())
                        {
                            FKitem.Value = CivilWithLegsInst.StructureType.ToString();
                        }
                        else if (FKitem.Key.ToLower() == "SectionsLegType".ToLower())
                        {
                            FKitem.Value = CivilWithLegsInst.SectionsLegType.ToString();
                        }
                    }

                    objectInst.AttributesActivated = ListAttributesActivated;

                    objectInst.DynamicAtts = _unitOfWork.DynamicAttInstValueRepository.
                        GetDynamicInstAtts(TableNameEntity.Id, CivilInsId, null);

                    List<KeyValuePair<string, List<DropDownListFilters>>> RelatedTables = _unitOfWork.CivilWithLegsRepository
                        .GetRelatedTables();

                    string SiteCode = _unitOfWork.CivilSiteDateRepository
                        .GetIncludeWhereFirst(x => x.allCivilInst.civilWithLegs.Id == CivilInsId && !x.Dismantle && !x.allCivilInst.Draft,
                            x => x.allCivilInst, x => x.allCivilInst.civilWithLegs).SiteCode;

                    List<DropDownListFilters> RelatedToSite = GetRelatedToSiteToEdit(SiteCode, TableName , CivilInsId);

                    DropDownListFilters EditRecordReferenceToDelete = RelatedToSite.FirstOrDefault(x => x.Id == CivilInsId);

                    if (EditRecordReferenceToDelete != null)
                        RelatedToSite.Remove(EditRecordReferenceToDelete);

                    RelatedTables.Add(new KeyValuePair<string, List<DropDownListFilters>>("ReferenceCivilId", RelatedToSite));
                    objectInst.RelatedTables = RelatedTables;

                    TLIallCivilInst AllCivilInst = _unitOfWork.AllCivilInstRepository
                        .GetWhereFirst(x => x.civilWithLegsId == CivilInsId);

                    TLIcivilSiteDate CivilSiteDateInfo = _unitOfWork.CivilSiteDateRepository
                        .GetWhereFirst(x => x.allCivilInstId == AllCivilInst.Id);

                    List<BaseAttView> CivilSiteDateAttributes = _unitOfWork.AttributeActivatedRepository
                        .GetAttributeActivated(Helpers.Constants.TablesNames.TLIcivilSiteDate.ToString(), CivilSiteDateInfo, null, "allCivilInstId", "Id", "SiteCode", "Dismantle").ToList();

                    objectInst.CivilSiteDate = _mapper.Map<List<BaseInstAttView>>(CivilSiteDateAttributes);

                    TLIcivilSupportDistance civilSupportDistance = _unitOfWork.CivilSupportDistanceRepository
                        .GetWhereFirst(x => x.CivilInstId == AllCivilInst.Id);

                    List<BaseAttView> CivilsupportAttributes = _unitOfWork.AttributeActivatedRepository
                        .GetAttributeActivated(Helpers.Constants.TablesNames.TLIcivilSupportDistance.ToString(), civilSupportDistance, null, "CivilInstId", "Id", "SiteCode").ToList();

                    int? ReferenceCivilInstId = null;

                    foreach (BaseAttView CivilsupportAttribute in CivilsupportAttributes)
                    {
                        if (CivilsupportAttribute.DataType.ToLower() == "list")
                        {

                            if (CivilsupportAttribute.Key.ToLower() == "referencecivilid" && civilSupportDistance != null)
                            {
                                if (civilSupportDistance.ReferenceCivilId == 0)
                                {
                                    CivilsupportAttribute.Value = "NA";
                                }
                                else
                                {
                                    var support = _unitOfWork.CivilSupportDistanceRepository
                                        .GetIncludeWhereFirst(x => x.CivilInstId == civilSupportDistance.CivilInstId);

                                    TLIallCivilInst SupportReferenceAllCivilInst = _unitOfWork.AllCivilInstRepository
                                        .GetIncludeWhereFirst(x => x.Id == support.ReferenceCivilId, x => x.civilWithLegs, x => x.civilWithoutLeg, x => x.civilNonSteel);

                                    CivilsupportAttribute.Value = SupportReferenceAllCivilInst.civilWithLegsId != null ? SupportReferenceAllCivilInst.civilWithLegs.Name :
                                        (SupportReferenceAllCivilInst.civilWithoutLegId != null ? SupportReferenceAllCivilInst.civilWithoutLeg.Name :
                                        SupportReferenceAllCivilInst.civilNonSteel.Name);

                                    ReferenceCivilInstId = support.ReferenceCivilId;
                                }
                            }
                        }
                    }

                    objectInst.CivilSupportDistance = _mapper.Map<List<BaseInstAttView>>(CivilsupportAttributes);
                    if (ReferenceCivilInstId != null)
                    {
                        objectInst.CivilSupportDistance.FirstOrDefault(x => x.Key.ToLower() == "ReferenceCivilId".ToLower()).Id = ReferenceCivilInstId.Value;
                    }
                }
                else if (Helpers.Constants.CivilType.TLIcivilWithoutLeg.ToString() == TableName)
                {
                    TLIcivilWithoutLeg CivilWithoutLegInst = _unitOfWork.CivilWithoutLegRepository
                        .GetIncludeWhereFirst(x => x.Id == CivilInsId, x => x.CivilWithoutlegsLib, x => x.Owner, x => x.subType);

                    CivilWithoutLegLibraryViewModel CivilWithoutLegLibrary = _mapper.Map<CivilWithoutLegLibraryViewModel>(_unitOfWork.CivilWithoutLegLibraryRepository
                        .GetIncludeWhereFirst(x => x.Id == CivilWithoutLegInst.CivilWithoutlegsLibId, x => x.CivilSteelSupportCategory,
                            x => x.CivilWithoutLegCategory, x => x.InstallationCivilwithoutLegsType, x => x.structureType));

                    List<BaseAttView> LibraryAttributes = _unitOfWork.AttributeActivatedRepository
                        .GetAttributeActivated(Helpers.Constants.TablesNames.TLIcivilWithoutLegLibrary.ToString(),
                            CivilWithoutLegLibrary, CivilWithoutLegLibrary.CivilWithoutLegCategoryId).ToList();

                    foreach (BaseAttView LibraryAttribute in LibraryAttributes)
                    {
                        if (LibraryAttribute.DataType.ToLower() == "list")
                        {
                            LibraryAttribute.Value = CivilWithoutLegLibrary.GetType().GetProperties()
                                .FirstOrDefault(x => x.Name.ToLower() == LibraryAttribute.Label.ToLower()).GetValue(CivilWithoutLegLibrary);
                        }
                    }

                    List<BaseAttView> LogisticalAttributes = _mapper.Map<List<BaseAttView>>(_unitOfWork.LogistcalRepository
                        .GetLogistical(Helpers.Constants.TablePartName.CivilSupport.ToString(), Helpers.Constants.TablesNames.TLIcivilWithoutLegLibrary.ToString(), CivilWithoutLegLibrary.Id).ToList());

                    LibraryAttributes.AddRange(LogisticalAttributes);
                    objectInst.LibraryActivatedAttributes = LibraryAttributes;

                    List<BaseInstAttView> ListAttributesActivated = _unitOfWork.AttributeActivatedRepository
                        .GetInstAttributeActivatedForCivilWithoutLeg(CivilWithoutLegLibrary.CivilWithoutLegCategoryId, CivilWithoutLegInst).ToList();

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
                            if (CivilWithoutLegInst.Owner == null)
                                FKitem.Value = "NA";

                            else
                            {
                                FKitem.Value = CivilWithoutLegInst.Owner.OwnerName;
                            }
                        }
                        else if (FKitem.Desc.ToLower() == "tlisubtype")
                        {
                            if (CivilWithoutLegInst.subType == null)
                                FKitem.Value = "NA";

                            else
                                FKitem.Value = CivilWithoutLegInst.subType.Name;
                        }

                        if (FKitem.Desc.ToLower() == "tlicivilwithoutleglibrary")
                        {
                            if (CivilWithoutLegInst.CivilWithoutlegsLib == null)
                                FKitem.Value = "NA";
                            else
                                FKitem.Value = CivilWithoutLegInst.CivilWithoutlegsLib.Model;
                        }
                    }

                    objectInst.AttributesActivated = ListAttributesActivated;

                    objectInst.DynamicAtts = _unitOfWork.DynamicAttInstValueRepository
                        .GetDynamicInstAtts(TableNameEntity.Id, CivilInsId, CivilWithoutLegLibrary.CivilWithoutLegCategoryId);

                    List<KeyValuePair<string, List<DropDownListFilters>>> RelatedTables = _unitOfWork.CivilWithoutLegRepository
                        .GetRelatedTables();

                    string SiteCode = _unitOfWork.CivilSiteDateRepository
                        .GetIncludeWhereFirst(x => x.allCivilInst.civilWithoutLeg.Id == CivilInsId && !x.Dismantle && !x.allCivilInst.Draft,
                            x => x.allCivilInst, x => x.allCivilInst.civilWithoutLeg).SiteCode;

                    List<DropDownListFilters> RelatedToSite = GetRelatedToSiteToEdit(SiteCode, TableName, CivilInsId);

                    DropDownListFilters EditRecordReferenceToDelete = RelatedToSite.FirstOrDefault(x => x.Id == CivilInsId);

                    if (EditRecordReferenceToDelete != null)
                        RelatedToSite.Remove(EditRecordReferenceToDelete);

                    RelatedTables.Add(new KeyValuePair<string, List<DropDownListFilters>>("ReferenceCivilId", RelatedToSite));
                    objectInst.RelatedTables = RelatedTables;

                    TLIallCivilInst AllCivilInst = _unitOfWork.AllCivilInstRepository
                        .GetWhereFirst(x => x.civilWithoutLegId == CivilInsId);

                    TLIcivilSiteDate CivilSiteDateInfo = _unitOfWork.CivilSiteDateRepository
                        .GetWhereFirst(x => x.allCivilInstId == AllCivilInst.Id);

                    List<BaseAttView> CivilSiteDateAttributes = _unitOfWork.AttributeActivatedRepository
                         .GetAttributeActivated(Helpers.Constants.TablesNames.TLIcivilSiteDate.ToString(), CivilSiteDateInfo, null, "allCivilInstId", "Id", "SiteCode", "Dismantle").ToList();

                    objectInst.CivilSiteDate = _mapper.Map<List<BaseInstAttView>>(CivilSiteDateAttributes);

                    TLIcivilSupportDistance civilSupportDistance = _unitOfWork.CivilSupportDistanceRepository
                        .GetWhereFirst(x => x.CivilInstId == AllCivilInst.Id);

                    List<BaseAttView> CivilsupportAttributes = _unitOfWork.AttributeActivatedRepository
                        .GetAttributeActivated(Helpers.Constants.TablesNames.TLIcivilSupportDistance.ToString(), civilSupportDistance, null, "CivilInstId", "Id", "SiteCode").ToList();

                    int? ReferenceCivilInstId = null;

                    foreach (BaseAttView CivilsupportAttribute in CivilsupportAttributes)
                    {
                        if (CivilsupportAttribute.DataType.ToLower() == "list")
                        {
                            if (CivilsupportAttribute.Key.ToLower() == "referencecivilid" && civilSupportDistance != null)
                            {
                                if (civilSupportDistance.ReferenceCivilId == 0)
                                {
                                    CivilsupportAttribute.Value = "NA";
                                }
                                else
                                {
                                    
                                    var support = _unitOfWork.CivilSupportDistanceRepository
                                        .GetIncludeWhereFirst(x => x.CivilInstId == civilSupportDistance.CivilInstId);
                                    if (support != null) {

                                        TLIallCivilInst SupportReferenceAllCivilInst = _unitOfWork.AllCivilInstRepository
                                        .GetIncludeWhereFirst(x => x.Id == support.ReferenceCivilId, x => x.civilWithLegs, x => x.civilWithoutLeg, x => x.civilNonSteel);
                                        if (SupportReferenceAllCivilInst != null) 
                                        {
                                            CivilsupportAttribute.Value = SupportReferenceAllCivilInst.civilWithLegsId != null ? SupportReferenceAllCivilInst.civilWithLegs.Name :
                                            (SupportReferenceAllCivilInst.civilWithoutLegId != null ? SupportReferenceAllCivilInst.civilWithoutLeg.Name :
                                             SupportReferenceAllCivilInst.civilNonSteel.Name);
                                        }
                                   
                                        ReferenceCivilInstId = support.ReferenceCivilId;
                                    }
                                }
                            }
                        }
                    }

                    objectInst.CivilSupportDistance = _mapper.Map<List<BaseInstAttView>>(CivilsupportAttributes);
                    if (ReferenceCivilInstId != null)
                    {
                        objectInst.CivilSupportDistance.FirstOrDefault(x => x.Key.ToLower() == "ReferenceCivilId".ToLower()).Id = ReferenceCivilInstId.Value;
                    }
                }
                else if (Helpers.Constants.CivilType.TLIcivilNonSteel.ToString() == TableName)
                {
                    TLIcivilNonSteel CivilNonSteelInst = _unitOfWork.CivilNonSteelRepository
                        .GetIncludeWhereFirst(x => x.Id == CivilInsId, x => x.CivilNonsteelLibrary, x => x.locationType,
                            x => x.owner, x => x.supportTypeImplemented);

                    CivilNonSteelLibraryViewModel CivilNonSteelLibrary = _mapper.Map<CivilNonSteelLibraryViewModel>(_unitOfWork.CivilNonSteelLibraryRepository
                       .GetIncludeWhereFirst(x => x.Id == CivilNonSteelInst.CivilNonSteelLibraryId, x => x.civilNonSteelType));

                    List<BaseAttView> LibraryAttributes = _unitOfWork.AttributeActivatedRepository
                        .GetAttributeActivated(Helpers.Constants.TablesNames.TLIcivilNonSteelLibrary.ToString(), CivilNonSteelLibrary, null).ToList();

                    foreach (BaseAttView LibraryAttribute in LibraryAttributes)
                    {
                        if (LibraryAttribute.DataType.ToLower() == "list")
                        {
                            LibraryAttribute.Value = CivilNonSteelLibrary.GetType().GetProperties()
                                .FirstOrDefault(x => x.Name.ToLower() == LibraryAttribute.Label.ToLower()).GetValue(CivilNonSteelLibrary);
                        }
                    }

                    List<BaseAttView> LogisticalAttributes = _mapper.Map<List<BaseAttView>>(_unitOfWork.LogistcalRepository
                        .GetLogistical(Helpers.Constants.TablePartName.CivilSupport.ToString(), Helpers.Constants.TablesNames.TLIcivilNonSteelLibrary.ToString(), CivilNonSteelLibrary.Id).ToList());

                    LibraryAttributes.AddRange(LogisticalAttributes);

                    objectInst.LibraryActivatedAttributes = LibraryAttributes;

                    List<BaseInstAttView> ListAttributesActivated = _unitOfWork.AttributeActivatedRepository
                        .GetInstAttributeActivated(Helpers.Constants.TablesNames.TLIcivilNonSteel.ToString(), CivilNonSteelInst/*, "CurrentLoads"*/).ToList();

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
                            if (CivilNonSteelInst.owner == null)
                                FKitem.Value = "NA";

                            else
                                FKitem.Value = CivilNonSteelInst.owner.OwnerName;
                        }
                        else if (FKitem.Desc.ToLower() == "tlisupporttypeimplemented")
                        {
                            if (CivilNonSteelInst.supportTypeImplemented == null)
                                FKitem.Value = "NA";

                            else
                                FKitem.Value = CivilNonSteelInst.supportTypeImplemented.Name;
                        }
                        else if (FKitem.Desc.ToLower() == "tlilocationtype")
                        {
                            if (CivilNonSteelInst.locationType == null)
                                FKitem.Value = "NA";

                            else
                                FKitem.Value = CivilNonSteelInst.locationType.Name;
                        }
                        else if (FKitem.Desc.ToLower() == "tlicivilnonsteellibrary")
                        {
                            if (CivilNonSteelInst.CivilNonsteelLibrary == null)
                                FKitem.Value = "NA";

                            else
                                FKitem.Value = CivilNonSteelInst.CivilNonsteelLibrary.Model;
                        }
                    }

                    objectInst.AttributesActivated = ListAttributesActivated;

                    objectInst.DynamicAtts = _unitOfWork.DynamicAttInstValueRepository
                        .GetDynamicInstAtts(TableNameEntity.Id, CivilInsId, null);

                    List<KeyValuePair<string, List<DropDownListFilters>>> RelatedTables = _unitOfWork.CivilNonSteelRepository
                        .GetRelatedTables();

                    string SiteCode = _unitOfWork.CivilSiteDateRepository
                        .GetIncludeWhereFirst(x => x.allCivilInst.civilNonSteelId == CivilInsId && !x.Dismantle && !x.allCivilInst.Draft,
                            x => x.allCivilInst, x => x.allCivilInst.civilNonSteel).SiteCode;

                    List<DropDownListFilters> RelatedToSite = GetRelatedToSiteToEdit(SiteCode, TableName, CivilInsId);

                    DropDownListFilters EditRecordReferenceToDelete = RelatedToSite.FirstOrDefault(x => x.Id == CivilInsId);

                    if (EditRecordReferenceToDelete != null)
                        RelatedToSite.Remove(EditRecordReferenceToDelete);

                    RelatedTables.Add(new KeyValuePair<string, List<DropDownListFilters>>("ReferenceCivilId", RelatedToSite));
                    objectInst.RelatedTables = RelatedTables;

                    TLIallCivilInst AllCivilInst = _unitOfWork.AllCivilInstRepository
                        .GetWhereFirst(x => x.civilNonSteelId == CivilInsId);

                    TLIcivilSiteDate CivilSiteDateInfo = _unitOfWork.CivilSiteDateRepository
                        .GetWhereFirst(x => x.allCivilInstId == AllCivilInst.Id);

                    List<BaseAttView> CivilSiteDateAttributes = _unitOfWork.AttributeActivatedRepository
                        .GetAttributeActivated(Helpers.Constants.TablesNames.TLIcivilSiteDate.ToString(), CivilSiteDateInfo, null, "allCivilInstId", "Id", "SiteCode", "Dismantle").ToList();

                    objectInst.CivilSiteDate = _mapper.Map<List<BaseInstAttView>>(CivilSiteDateAttributes);

                    TLIcivilSupportDistance civilSupportDistance = _unitOfWork.CivilSupportDistanceRepository
                        .GetWhereFirst(x => x.CivilInstId == AllCivilInst.Id);

                    List<BaseAttView> CivilsupportAttributes = _unitOfWork.AttributeActivatedRepository
                        .GetAttributeActivated(Helpers.Constants.TablesNames.TLIcivilSupportDistance.ToString(), civilSupportDistance, null, "CivilInstId", "Id", "SiteCode").ToList();

                    int? ReferenceCivilInstId = null;

                    foreach (BaseAttView CivilsupportAttribute in CivilsupportAttributes)
                    {
                        if (CivilsupportAttribute.DataType.ToLower() == "list")
                        {
                            if (CivilsupportAttribute.Key.ToLower() == "referencecivilid" && civilSupportDistance != null)
                            {
                                if (civilSupportDistance.ReferenceCivilId == 0)
                                {
                                    CivilsupportAttribute.Value = "NA";
                                }
                                else
                                {
                                    var support = _unitOfWork.CivilSupportDistanceRepository
                                        .GetIncludeWhereFirst(x => x.CivilInstId == civilSupportDistance.CivilInstId);

                                    TLIallCivilInst SupportReferenceAllCivilInst = _unitOfWork.AllCivilInstRepository
                                        .GetIncludeWhereFirst(x => x.Id == support.ReferenceCivilId, x => x.civilWithLegs, x => x.civilWithoutLeg, x => x.civilNonSteel);

                                    CivilsupportAttribute.Value = SupportReferenceAllCivilInst.civilWithLegsId != null ? SupportReferenceAllCivilInst.civilWithLegs.Name :
                                        (SupportReferenceAllCivilInst.civilWithoutLegId != null ? SupportReferenceAllCivilInst.civilWithoutLeg.Name :
                                        SupportReferenceAllCivilInst.civilNonSteel.Name);

                                    ReferenceCivilInstId = support.ReferenceCivilId;
                                }

                            }
                        }
                    }

                    objectInst.CivilSupportDistance = _mapper.Map<List<BaseInstAttView>>(CivilsupportAttributes);
                    if (ReferenceCivilInstId != null)
                    {
                        objectInst.CivilSupportDistance.FirstOrDefault(x => x.Key.ToLower() == "ReferenceCivilId".ToLower()).Id = ReferenceCivilInstId.Value;
                    }
                }
                return new Response<ObjectInstAtts>(true, objectInst, null, null, (int)Helpers.Constants.ApiReturnCode.success);
            }
            catch (Exception err)
            {
                return new Response<ObjectInstAtts>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
        }
        //Function accept 2 parameters 
        //First List of installation dynamic attribute values 
        //Second InventoryId 
        private void UpdateDynamicAttInstValue(List<BaseInstAttView> DynamicInstAttsValue, int InventoryId)
        {
            Parallel.ForEach(DynamicInstAttsValue, DynamicInstAttValue =>
            {
                var DAIV = _unitOfWork.DynamicAttInstValueRepository.GetWhereFirst(d => d.InventoryId == InventoryId && d.DynamicAttId == DynamicInstAttValue.Id);
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
        #region Get Enabled Attributes Only With Dynamic Objects...
        public Response<ReturnWithFilters<object>> GetCivilWithLegsWithEnableAtt(SiteBaseFilter BaseFilter, bool WithFilterData, CombineFilters CombineFilters, ParameterPagination parameterPagination)
        {
            try
            {
                int Count = 0;
                List<object> OutPutList = new List<object>();
                ReturnWithFilters<object> CivilTableDisplay = new ReturnWithFilters<object>();

                //
                // Get All CivilSiteDateRecords To This BaseFilter
                //
                List<TLIcivilSiteDate> AllCivilSiteDateRecords = GetCivilSiteDateBySiteBaseFilter(BaseFilter, "CivilWithLegs", CombineFilters, null);

                List<TLIcivilSiteDate> CivilSiteDateRecords = GetMaxInstallationDate(out Count, AllCivilSiteDateRecords, "CivilWithLegs", parameterPagination);

                List<CivilWithLegsViewModel> Civils = _mapper.Map<List<CivilWithLegsViewModel>>(CivilSiteDateRecords.Select(x =>
                    x.allCivilInst.civilWithLegs).ToList());

                List<TLIattributeViewManagment> AllAttributes = _unitOfWork.AttributeViewManagmentRepository.GetIncludeWhere(x =>
                   (x.Enable && x.EditableManagmentView.View == Helpers.Constants.EditableManamgmantViewNames.CivilWithLegInstallation.ToString() &&
                   (x.AttributeActivatedId != null ?
                        (x.AttributeActivated.Tabel == Helpers.Constants.TablesNames.TLIcivilWithLegs.ToString() && x.AttributeActivated.enable) :
                        (!x.DynamicAtt.LibraryAtt && !x.DynamicAtt.disable && x.DynamicAtt.tablesNames.TableName == Helpers.Constants.TablesNames.TLIcivilWithLegs.ToString()))) ||
                    (x.AttributeActivated != null ?
                        ((x.AttributeActivated.Key.ToLower() == "id" || x.AttributeActivated.Key.ToLower() == "active") && x.AttributeActivated.Tabel == Helpers.Constants.TablesNames.TLIcivilWithLegs.ToString()) : false),
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

                foreach (CivilWithLegsViewModel CivilWithLegInstallationObject in Civils)
                {
                    dynamic DynamicCivilWithLegInstallation = new ExpandoObject();

                    //
                    // Installation Object ViewModel...
                    //
                    if (NotDateTimeInstallationAttributesViewModel != null ? NotDateTimeInstallationAttributesViewModel.Count > 0 : false)
                    {
                        List<PropertyInfo> InstallationProps = typeof(CivilWithLegsViewModel).GetProperties().Where(x =>
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
                                object ForeignKeyNamePropObject = prop.GetValue(CivilWithLegInstallationObject, null);
                                ((IDictionary<String, Object>)DynamicCivilWithLegInstallation).Add(new KeyValuePair<string, object>(prop.Name, ForeignKeyNamePropObject));
                            }
                            else if (NotDateTimeInstallationAttributesViewModel.Select(x =>
                                 x.AttributeActivated.Key.ToLower()).Contains(prop.Name.ToLower()) &&
                                !prop.Name.ToLower().Contains("_name") &&
                                (prop.Name.ToLower().Substring(Math.Max(0, prop.Name.Length - 2)) != "id" || prop.Name.ToLower() == "id"))
                            {
                                if (prop.Name.ToLower() == "StructureType".ToLower() ||
                                    prop.Name.ToLower() == "SectionsLegType".ToLower())
                                {
                                    TLIattributeViewManagment LabelName = AllAttributes.FirstOrDefault(x => ((x.AttributeActivated != null) ? x.AttributeActivated.Key == prop.Name : false) &&
                                        x.AttributeActivated.Tabel == Helpers.Constants.TablesNames.TLIcivilWithLegs.ToString() &&
                                        x.Enable && x.AttributeActivated.DataType != "List" && x.Id != 0);

                                    if (LabelName != null)
                                    {
                                        if (!string.IsNullOrEmpty(prop.GetValue(CivilWithLegInstallationObject, null).ToString()))
                                        {
                                            object PropObject = prop.GetValue(CivilWithLegInstallationObject, null);
                                            ((IDictionary<String, Object>)DynamicCivilWithLegInstallation).Add(new KeyValuePair<string, object>(LabelName.AttributeActivated.Label, PropObject.ToString()));
                                        }
                                        else
                                        {
                                            ((IDictionary<String, Object>)DynamicCivilWithLegInstallation).Add(new KeyValuePair<string, object>(LabelName.AttributeActivated.Label, null));
                                        }
                                    }
                                }
                                else if (prop.Name.ToLower() != "id" && prop.Name.ToLower() != "active")
                                {
                                    TLIattributeViewManagment LabelName = AllAttributes.FirstOrDefault(x => ((x.AttributeActivated != null) ? x.AttributeActivated.Key == prop.Name : false) &&
                                        x.AttributeActivated.Tabel == Helpers.Constants.TablesNames.TLIcivilWithLegs.ToString() &&
                                        x.Enable && x.AttributeActivated.DataType != "List" && x.Id != 0);

                                    if (LabelName != null)
                                    {
                                        object PropObject = prop.GetValue(CivilWithLegInstallationObject, null);
                                        ((IDictionary<String, Object>)DynamicCivilWithLegInstallation).Add(new KeyValuePair<string, object>(LabelName.AttributeActivated.Label, PropObject));
                                    }
                                }
                                else
                                {
                                    object PropObject = prop.GetValue(CivilWithLegInstallationObject, null);
                                    ((IDictionary<String, Object>)DynamicCivilWithLegInstallation).Add(new KeyValuePair<string, object>(prop.Name, PropObject));
                                }
                            }
                        }
                    }

                    //
                    // Installation Dynamic Attributes... (Not DateTime DataType Attribute)
                    //
                    if (NotDateTimeDynamicInstallationAttributesViewModel != null ? NotDateTimeDynamicInstallationAttributesViewModel.Count > 0 : false)
                    {
                        var temp = NotDateTimeDynamicInstallationAttributesViewModel.Select(x => x.DynamicAttId).ToList();
                        List<TLIdynamicAtt> NotDateTimeInstallationDynamicAttributes = _unitOfWork.DynamicAttRepository
                            .GetIncludeWhere(x =>
                                !x.disable &&
                                x.tablesNames.TableName == Helpers.Constants.TablesNames.TLIcivilWithLegs.ToString() &&
                                !x.LibraryAtt &&
                                x.DataType.Name.ToLower() != "datetime" &&
                                temp.Any(y => y == x.Id)
                            , x => x.tablesNames, x => x.DataType)
                            .ToList();

                        var tempno = NotDateTimeInstallationDynamicAttributes.Select(x => x.Key.ToLower()).ToList();
                        List<TLIdynamicAttInstValue> NotDateTimeDynamicAttInstValues = _unitOfWork.DynamicAttInstValueRepository.GetIncludeWhere(x =>
                            !x.DynamicAtt.LibraryAtt && !x.disable &&
                            x.InventoryId == CivilWithLegInstallationObject.Id &&
                            tempno.Any(y => y == x.DynamicAtt.Key.ToLower()) &&
                            x.tablesNames.TableName == Helpers.Constants.TablesNames.TLIcivilWithLegs.ToString()
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

                                ((IDictionary<String, Object>)DynamicCivilWithLegInstallation).Add(new KeyValuePair<string, object>(InstallationDynamicAtt.Key, DynamicAttValue));
                            }
                            else
                            {
                                ((IDictionary<String, Object>)DynamicCivilWithLegInstallation).Add(new KeyValuePair<string, object>(InstallationDynamicAtt.Key, null));
                            }
                        }
                    }

                    //
                    // Installation Object ViewModel... (DateTime DataType Attribute)
                    //
                    dynamic DateTimeAttributes = new ExpandoObject();
                    if (DateTimeInstallationAttributesViewModel != null ? DateTimeInstallationAttributesViewModel.Count() > 0 : false)
                    {
                        List<PropertyInfo> DateTimeInstallationProps = typeof(CivilWithLegsViewModel).GetProperties().Where(x =>
                            x.PropertyType.GenericTypeArguments != null ?
                                (x.PropertyType.GenericTypeArguments.Count() > 0 ? x.PropertyType.GenericTypeArguments.FirstOrDefault().Name.ToLower() == "datetime" :
                                (x.PropertyType.Name.ToLower() == "datetime")) :
                            (x.PropertyType.Name.ToLower() == "datetime")).ToList();

                        foreach (PropertyInfo prop in DateTimeInstallationProps)
                        {
                            TLIattributeViewManagment LabelName = AllAttributes.FirstOrDefault(x => ((x.AttributeActivated != null) ? x.AttributeActivated.Key == prop.Name : false) &&
                                x.AttributeActivated.Tabel == Helpers.Constants.TablesNames.TLIcivilWithLegs.ToString() &&
                                x.Enable && x.AttributeActivated.DataType != "List" && x.Id != 0);

                            if (LabelName != null)
                            {
                                object PropObject = prop.GetValue(CivilWithLegInstallationObject, null);
                                ((IDictionary<String, Object>)DateTimeAttributes).Add(new KeyValuePair<string, object>(LabelName.AttributeActivated.Label, PropObject));
                            }
                        }
                    }

                    //
                    // Installation Dynamic Attributes... (DateTime DataType Attribute)
                    // 
                    if (DateTimeDynamicInstallationAttributesViewModel != null ? DateTimeDynamicInstallationAttributesViewModel.Count() > 0 : false)
                    {
                        var temp = DateTimeDynamicInstallationAttributesViewModel.Select(x => x.DynamicAttId).ToList();
                        List<TLIdynamicAtt> DateTimeInstallationDynamicAttributes = _unitOfWork.DynamicAttRepository.GetIncludeWhere(x =>
                           !x.disable && x.tablesNames.TableName == Helpers.Constants.TablesNames.TLIcivilWithLegs.ToString() &&
                           !x.LibraryAtt && x.DataType.Name.ToLower() == "datetime" &&
                            temp.Any(y => y == x.Id), x => x.tablesNames).ToList();

                        var tempno = DateTimeInstallationDynamicAttributes.Select(x => x.Key.ToLower()).ToList();
                        List<TLIdynamicAttInstValue> DateTimeDynamicAttInstValues = _unitOfWork.DynamicAttInstValueRepository.GetIncludeWhere(x =>
                            x.InventoryId == CivilWithLegInstallationObject.Id && !x.disable &&
                           !x.DynamicAtt.LibraryAtt &&
                            tempno.Any(y => y == x.DynamicAtt.Key.ToLower()) &&
                            x.tablesNames.TableName == Helpers.Constants.TablesNames.TLIcivilWithLegs.ToString()
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

                    ((IDictionary<String, Object>)DynamicCivilWithLegInstallation).Add(new KeyValuePair<string, object>("DateTimeAttributes", DateTimeAttributes));

                    OutPutList.Add(DynamicCivilWithLegInstallation);
                }

                CivilTableDisplay.Model = OutPutList;

                if (WithFilterData)
                {
                    CivilTableDisplay.filters = _unitOfWork.CivilWithLegsRepository.GetRelatedTables();
                }
                else
                {
                    CivilTableDisplay.filters = null;
                }

                return new Response<ReturnWithFilters<object>>(true, CivilTableDisplay, null, null, (int)Helpers.Constants.ApiReturnCode.success, Count);
            }
            catch (Exception err)
            {
                return new Response<ReturnWithFilters<object>>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
        }
        public Response<ReturnWithFilters<object>> GetCivilWithoutLegWithEnableAtt(SiteBaseFilter BaseFilter, bool WithFilterData, CombineFilters CombineFilters, ParameterPagination parameterPagination, int CategoryId)
        {
            try
            {
                int Count = 0;
                List<object> OutPutList = new List<object>();
                ReturnWithFilters<object> CivilTableDisplay = new ReturnWithFilters<object>();

                List<FilterObjectList> AttributeFilters = CombineFilters.filters;
                List<DateFilterViewModel> DateFilter = CombineFilters.DateFilter;

                //
                // Get All CivilSiteDateRecords To This BaseFilter
                //
                List<TLIcivilSiteDate> AllCivilSiteDateRecords = GetCivilSiteDateBySiteBaseFilter(BaseFilter, "CivilWithoutLegs", CombineFilters, CategoryId);

                List<TLIcivilSiteDate> CivilSiteDateRecords = GetMaxInstallationDate(out Count, AllCivilSiteDateRecords, "CivilWithoutLegs", parameterPagination);

                List<CivilWithoutLegViewModel> Civils = _mapper.Map<List<CivilWithoutLegViewModel>>(CivilSiteDateRecords.Select(x =>
                    x.allCivilInst.civilWithoutLeg).ToList());

                List<AttActivatedCategoryViewModel> AttributeActivatedCategories = _mapper.Map<List<AttActivatedCategoryViewModel>>(_unitOfWork.AttActivatedCategoryRepository.GetIncludeWhere(x =>
                    (x.civilWithoutLegCategoryId.Value == CategoryId) &&
                    (x.enable) &&
                    (x.attributeActivated != null ?
                        ((x.attributeActivated.enable && x.attributeActivated.Tabel == Helpers.Constants.TablesNames.TLIcivilWithoutLeg.ToString()) ||
                        ((x.attributeActivated.Key.ToLower() == "id" || x.attributeActivated.Key.ToLower() == "active") && x.attributeActivated.Tabel == Helpers.Constants.TablesNames.TLIcivilWithoutLeg.ToString())) : false)
                    , x => x.attributeActivated).ToList());

                string CategoryName = _unitOfWork.CivilWithoutLegCategoryRepository.GetByID(CategoryId).Name;
                string EditableMangmentCategoryViewName = "";
                if (CategoryName == "Mast")
                    EditableMangmentCategoryViewName = Helpers.Constants.EditableManamgmantViewNames.CivilWithoutLegInstallationMast.ToString();
                else if (CategoryName == "Capsule")
                    EditableMangmentCategoryViewName = Helpers.Constants.EditableManamgmantViewNames.CivilWithoutLegInstallationCapsule.ToString();
                else if (CategoryName == "Monopole")
                    EditableMangmentCategoryViewName = Helpers.Constants.EditableManamgmantViewNames.CivilWithoutLegInstallationMonopole.ToString();
                var tempAtt = AttributeActivatedCategories.Select(x => x.attributeActivatedId).ToList();
                List<TLIattributeViewManagment> AllAttributes = _unitOfWork.AttributeViewManagmentRepository.GetIncludeWhere(x =>
                   (x.Enable && x.EditableManagmentView.View == EditableMangmentCategoryViewName &&
                   (x.AttributeActivatedId != null ?
                        (x.AttributeActivated.Tabel == Helpers.Constants.TablesNames.TLIcivilWithoutLeg.ToString() && x.AttributeActivated.enable &&
                            tempAtt.Any(y => y == x.AttributeActivatedId)) :
                        (!x.DynamicAtt.LibraryAtt && !x.DynamicAtt.disable && x.DynamicAtt.tablesNames.TableName == Helpers.Constants.TablesNames.TLIcivilWithoutLeg.ToString()))) ||
                    (x.AttributeActivated != null ?
                        ((x.AttributeActivated.Key.ToLower() == "id" || x.AttributeActivated.Key.ToLower() == "active") && x.AttributeActivated.Tabel == Helpers.Constants.TablesNames.TLIcivilWithoutLeg.ToString()) : false),
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

                foreach (CivilWithoutLegViewModel CivilWithoutLegInstallationObject in Civils)
                {
                    dynamic DynamicCivilWithoutLegInstallation = new ExpandoObject();

                    //
                    // Installation Object ViewModel...
                    //
                    if (NotDateTimeInstallationAttributesViewModel != null ? NotDateTimeInstallationAttributesViewModel.Count > 0 : false)
                    {
                        List<PropertyInfo> InstallationProps = typeof(CivilWithoutLegViewModel).GetProperties().Where(x =>
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
                                object ForeignKeyNamePropObject = prop.GetValue(CivilWithoutLegInstallationObject, null);
                                ((IDictionary<String, Object>)DynamicCivilWithoutLegInstallation).Add(new KeyValuePair<string, object>(prop.Name, ForeignKeyNamePropObject));
                            }
                            else if (NotDateTimeInstallationAttributesViewModel.Select(x =>
                                 x.AttributeActivated.Key.ToLower()).Contains(prop.Name.ToLower()) &&
                                !prop.Name.ToLower().Contains("_name") &&
                                (prop.Name.ToLower().Substring(Math.Max(0, prop.Name.Length - 2)) != "id" || prop.Name.ToLower() == "id"))
                            {
                                if (prop.Name.ToLower() == "ladderSteps".ToLower() ||
                                    prop.Name.ToLower() == "availabilityOfWorkPlatforms".ToLower() ||
                                    prop.Name.ToLower() == "reinforced".ToLower() ||
                                    prop.Name.ToLower() == "equipmentsLocation".ToLower())
                                {
                                    TLIattributeViewManagment LabelName = AllAttributes.FirstOrDefault(x => ((x.AttributeActivated != null) ? x.AttributeActivated.Key == prop.Name : false) &&
                                        x.AttributeActivated.Tabel == Helpers.Constants.TablesNames.TLIcivilWithoutLeg.ToString() &&
                                        x.Enable && x.Id != 0);

                                    if (LabelName != null)
                                    {
                                        if (!string.IsNullOrEmpty(prop.GetValue(CivilWithoutLegInstallationObject, null).ToString()))
                                        {
                                            object PropObject = prop.GetValue(CivilWithoutLegInstallationObject, null);
                                            ((IDictionary<String, Object>)DynamicCivilWithoutLegInstallation).Add(new KeyValuePair<string, object>(LabelName.AttributeActivated.Label, PropObject.ToString()));
                                        }
                                        else
                                        {
                                            ((IDictionary<String, Object>)DynamicCivilWithoutLegInstallation).Add(new KeyValuePair<string, object>(LabelName.AttributeActivated.Label, null));
                                        }
                                    }
                                }
                                else if (prop.Name.ToLower() != "id" && prop.Name.ToLower() != "active")
                                {
                                    TLIattributeViewManagment LabelName = AllAttributes.FirstOrDefault(x => ((x.AttributeActivated != null) ? x.AttributeActivated.Key == prop.Name : false) &&
                                        x.AttributeActivated.Tabel == Helpers.Constants.TablesNames.TLIcivilWithoutLeg.ToString() &&
                                        x.Enable && x.AttributeActivated.DataType != "List" && x.Id != 0);

                                    if (LabelName != null)
                                    {
                                        object PropObject = prop.GetValue(CivilWithoutLegInstallationObject, null);
                                        ((IDictionary<String, Object>)DynamicCivilWithoutLegInstallation).Add(new KeyValuePair<string, object>(LabelName.AttributeActivated.Label, PropObject));
                                    }
                                }
                                else
                                {
                                    object PropObject = prop.GetValue(CivilWithoutLegInstallationObject, null);
                                    ((IDictionary<String, Object>)DynamicCivilWithoutLegInstallation).Add(new KeyValuePair<string, object>(prop.Name, PropObject));
                                }
                            }
                        }
                    }

                    //
                    // Installation Dynamic Attributes... (Not DateTime DataType Attribute)
                    //
                    if (NotDateTimeDynamicInstallationAttributesViewModel != null ? NotDateTimeDynamicInstallationAttributesViewModel.Count > 0 : false)
                    {
                        var temp = NotDateTimeDynamicInstallationAttributesViewModel.Select(x => x.DynamicAttId).ToList();
                        List<TLIdynamicAtt> NotDateTimeInstallationDynamicAttributes = _unitOfWork.DynamicAttRepository.GetIncludeWhere(x =>
                            !x.disable && x.tablesNames.TableName == Helpers.Constants.TablesNames.TLIcivilWithoutLeg.ToString() &&
                            !x.LibraryAtt && x.DataType.Name.ToLower() != "datetime" &&
                            temp.Any(y => y == x.Id)
                                , x => x.tablesNames, x => x.DataType).ToList();
                        var tempno = NotDateTimeInstallationDynamicAttributes.Select(x => x.Key.ToLower()).ToList();
                        List<TLIdynamicAttInstValue> NotDateTimeDynamicAttInstValues = _unitOfWork.DynamicAttInstValueRepository.GetIncludeWhere(x =>
                            !x.DynamicAtt.LibraryAtt && !x.disable &&
                            x.InventoryId == CivilWithoutLegInstallationObject.Id &&
                            tempno.Any(y => y == x.DynamicAtt.Key.ToLower()) &&
                            x.tablesNames.TableName == Helpers.Constants.TablesNames.TLIcivilWithoutLeg.ToString()
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

                                ((IDictionary<String, Object>)DynamicCivilWithoutLegInstallation).Add(new KeyValuePair<string, object>(InstallationDynamicAtt.Key, DynamicAttValue));
                            }
                            else
                            {
                                ((IDictionary<String, Object>)DynamicCivilWithoutLegInstallation).Add(new KeyValuePair<string, object>(InstallationDynamicAtt.Key, null));
                            }
                        }
                    }

                    //
                    // Installation Object ViewModel... (DateTime DataType Attribute)
                    //
                    dynamic DateTimeAttributes = new ExpandoObject();
                    if (DateTimeInstallationAttributesViewModel != null ? DateTimeInstallationAttributesViewModel.Count() > 0 : false)
                    {
                        List<PropertyInfo> DateTimeInstallationProps = typeof(CivilWithoutLegViewModel).GetProperties().Where(x =>
                            x.PropertyType.GenericTypeArguments != null ?
                                (x.PropertyType.GenericTypeArguments.Count() > 0 ? x.PropertyType.GenericTypeArguments.FirstOrDefault().Name == "datetime" :
                                (x.PropertyType.Name.ToLower() == "datetime")) :
                            (x.PropertyType.Name.ToLower() == "datetime")).ToList();

                        foreach (PropertyInfo prop in DateTimeInstallationProps)
                        {
                            TLIattributeViewManagment LabelName = AllAttributes.FirstOrDefault(x => ((x.AttributeActivated != null) ? x.AttributeActivated.Key == prop.Name : false) &&
                                x.AttributeActivated.Tabel == Helpers.Constants.TablesNames.TLIcivilWithoutLeg.ToString() &&
                                x.Enable && x.AttributeActivated.DataType != "List" && x.Id != 0);

                            if (LabelName != null)
                            {
                                object PropObject = prop.GetValue(CivilWithoutLegInstallationObject, null);
                                ((IDictionary<String, Object>)DateTimeAttributes).Add(new KeyValuePair<string, object>(LabelName.AttributeActivated.Label, PropObject));
                            }
                        }
                    }

                    //
                    // Installation Dynamic Attributes... (DateTime DataType Attribute)
                    //
                    if (DateTimeDynamicInstallationAttributesViewModel != null ? DateTimeDynamicInstallationAttributesViewModel.Count() > 0 : false)
                    {
                        var temp = DateTimeDynamicInstallationAttributesViewModel.Select(x => x.DynamicAttId).ToList();
                        List<TLIdynamicAtt> DateTimeInstallationDynamicAttributes = _unitOfWork.DynamicAttRepository.GetIncludeWhere(x =>
                           !x.disable && x.tablesNames.TableName == Helpers.Constants.TablesNames.TLIcivilWithoutLeg.ToString() &&
                           !x.LibraryAtt && x.DataType.Name.ToLower() == "datetime" &&
                            temp.Any(y => y == x.Id), x => x.tablesNames).ToList();

                        var tempno = DateTimeInstallationDynamicAttributes.Select(x => x.Key.ToLower()).ToList();
                        List<TLIdynamicAttInstValue> DateTimeDynamicAttInstValues = _unitOfWork.DynamicAttInstValueRepository.GetIncludeWhere(x =>
                            x.InventoryId == CivilWithoutLegInstallationObject.Id && !x.disable &&
                           !x.DynamicAtt.LibraryAtt &&
                            tempno.Any(y => y == x.DynamicAtt.Key.ToLower()) &&
                            x.tablesNames.TableName == Helpers.Constants.TablesNames.TLIcivilWithoutLeg.ToString()
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

                    ((IDictionary<String, Object>)DynamicCivilWithoutLegInstallation).Add(new KeyValuePair<string, object>("DateTimeAttributes", DateTimeAttributes));

                    OutPutList.Add(DynamicCivilWithoutLegInstallation);
                }

                CivilTableDisplay.Model = OutPutList;

                if (WithFilterData == true)
                {
                    CivilTableDisplay.filters = _unitOfWork.CivilWithoutLegRepository.GetRelatedTables();
                }
                else
                {
                    CivilTableDisplay.filters = null;
                }

                return new Response<ReturnWithFilters<object>>(true, CivilTableDisplay, null, null, (int)Helpers.Constants.ApiReturnCode.success, Count);
            }
            catch (Exception err)
            {
                return new Response<ReturnWithFilters<object>>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
        }
        public Response<ReturnWithFilters<object>> GetCivilNonSteelWithEnableAtt(SiteBaseFilter BaseFilter, bool WithFilterData, CombineFilters CombineFilters, ParameterPagination parameterPagination)
        {
            try
            {
                int Count = 0;
                List<object> OutPutList = new List<object>();
                ReturnWithFilters<object> CivilTableDisplay = new ReturnWithFilters<object>();

                List<FilterObjectList> AttributeFilters = CombineFilters.filters;
                List<DateFilterViewModel> DateFilter = CombineFilters.DateFilter;

                //
                // Get All CivilSiteDateRecords To This BaseFilter
                //
                List<TLIcivilSiteDate> AllCivilSiteDateRecords = GetCivilSiteDateBySiteBaseFilter(BaseFilter, "CivilNonSteel", CombineFilters, null);

                List<TLIcivilSiteDate> CivilSiteDateRecords = GetMaxInstallationDate(out Count, AllCivilSiteDateRecords, "CivilNonSteel", parameterPagination);

                List<CivilNonSteelViewModel> Civils = _mapper.Map<List<CivilNonSteelViewModel>>(CivilSiteDateRecords.Select(x =>
                    x.allCivilInst.civilNonSteel).ToList());

                List<TLIattributeViewManagment> AllAttributes = _unitOfWork.AttributeViewManagmentRepository.GetIncludeWhere(x =>
                   (x.Enable && x.EditableManagmentView.View == Helpers.Constants.EditableManamgmantViewNames.CivilNonSteelInstallation.ToString() &&
                   (x.AttributeActivatedId != null ?
                        (x.AttributeActivated.Tabel == Helpers.Constants.TablesNames.TLIcivilNonSteel.ToString() && x.AttributeActivated.enable) :
                        (!x.DynamicAtt.LibraryAtt && !x.DynamicAtt.disable && x.DynamicAtt.tablesNames.TableName == Helpers.Constants.TablesNames.TLIcivilNonSteel.ToString()))) ||
                    (x.AttributeActivated != null ?
                        ((x.AttributeActivated.Key.ToLower() == "id" || x.AttributeActivated.Key.ToLower() == "active") && x.AttributeActivated.Tabel == Helpers.Constants.TablesNames.TLIcivilNonSteel.ToString()) : false),
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

                foreach (CivilNonSteelViewModel CivilNonSteelInstallationObject in Civils)
                {
                    dynamic DynamicCivilNonSteelInstallation = new ExpandoObject();

                    //
                    // Installation Object ViewModel...
                    //
                    if (NotDateTimeInstallationAttributesViewModel != null ? NotDateTimeInstallationAttributesViewModel.Count > 0 : false)
                    {
                        List<PropertyInfo> InstallationProps = typeof(CivilNonSteelViewModel).GetProperties().Where(x =>
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
                                object ForeignKeyNamePropObject = prop.GetValue(CivilNonSteelInstallationObject, null);
                                ((IDictionary<String, Object>)DynamicCivilNonSteelInstallation).Add(new KeyValuePair<string, object>(prop.Name, ForeignKeyNamePropObject));
                            }
                            else if (NotDateTimeInstallationAttributesViewModel.Select(x =>
                                 x.AttributeActivated.Key.ToLower()).Contains(prop.Name.ToLower()) &&
                                !prop.Name.ToLower().Contains("_name") &&
                                (prop.Name.ToLower().Substring(Math.Max(0, prop.Name.Length - 2)) != "id" || prop.Name.ToLower() == "id"))
                            {
                                if (prop.Name.ToLower() != "id" && prop.Name.ToLower() != "active")
                                {
                                    TLIattributeViewManagment LabelName = AllAttributes.FirstOrDefault(x => ((x.AttributeActivated != null) ? x.AttributeActivated.Key == prop.Name : false) &&
                                        x.AttributeActivated.Tabel == Helpers.Constants.TablesNames.TLIcivilNonSteel.ToString() &&
                                        x.Enable && x.AttributeActivated.DataType != "List" && x.Id != 0);

                                    if (LabelName != null)
                                    {
                                        object PropObject = prop.GetValue(CivilNonSteelInstallationObject, null);
                                        ((IDictionary<String, Object>)DynamicCivilNonSteelInstallation).Add(new KeyValuePair<string, object>(LabelName.AttributeActivated.Label, PropObject));
                                    }
                                }
                                else
                                {
                                    object PropObject = prop.GetValue(CivilNonSteelInstallationObject, null);
                                    ((IDictionary<String, Object>)DynamicCivilNonSteelInstallation).Add(new KeyValuePair<string, object>(prop.Name, PropObject));
                                }
                            }
                        }
                    }

                    //
                    // Installation Dynamic Attributes... (Not DateTime DataType Attribute)
                    //
                    if (NotDateTimeDynamicInstallationAttributesViewModel != null ? NotDateTimeDynamicInstallationAttributesViewModel.Count > 0 : false)
                    {
                        var temp = NotDateTimeDynamicInstallationAttributesViewModel.Select(x => x.DynamicAttId).ToList();
                        List<TLIdynamicAtt> NotDateTimeInstallationDynamicAttributes = _unitOfWork.DynamicAttRepository.GetIncludeWhere(x =>
                            !x.disable && x.tablesNames.TableName == Helpers.Constants.TablesNames.TLIcivilNonSteel.ToString() &&
                            !x.LibraryAtt && x.DataType.Name.ToLower() != "datetime" &&
                            temp.Any(y => y == x.Id)
                                , x => x.tablesNames, x => x.DataType).ToList();
                        var tempno = NotDateTimeInstallationDynamicAttributes.Select(x => x.Key.ToLower()).ToList();
                        List<TLIdynamicAttInstValue> NotDateTimeDynamicAttInstValues = _unitOfWork.DynamicAttInstValueRepository.GetIncludeWhere(x =>
                            !x.DynamicAtt.LibraryAtt && !x.disable &&
                            x.InventoryId == CivilNonSteelInstallationObject.Id &&
                            tempno.Any(y => y== x.DynamicAtt.Key.ToLower()) &&
                            x.tablesNames.TableName == Helpers.Constants.TablesNames.TLIcivilNonSteel.ToString()
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

                                ((IDictionary<String, Object>)DynamicCivilNonSteelInstallation).Add(new KeyValuePair<string, object>(InstallationDynamicAtt.Key, DynamicAttValue));
                            }
                            else
                            {
                                ((IDictionary<String, Object>)DynamicCivilNonSteelInstallation).Add(new KeyValuePair<string, object>(InstallationDynamicAtt.Key, null));
                            }
                        }
                    }

                    //
                    // Installation Object ViewModel... (DateTime DataType Attribute)
                    //
                    dynamic DateTimeAttributes = new ExpandoObject();
                    if (DateTimeInstallationAttributesViewModel != null ? DateTimeInstallationAttributesViewModel.Count() > 0 : false)
                    {
                        List<PropertyInfo> DateTimeInstallationProps = typeof(CivilNonSteelViewModel).GetProperties().Where(x =>
                            x.PropertyType.GenericTypeArguments != null ?
                                (x.PropertyType.GenericTypeArguments.Count() > 0 ? x.PropertyType.GenericTypeArguments.FirstOrDefault().Name == "datetime" :
                                (x.PropertyType.Name.ToLower() == "datetime")) :
                            (x.PropertyType.Name.ToLower() == "datetime")).ToList();

                        foreach (PropertyInfo prop in DateTimeInstallationProps)
                        {
                            TLIattributeViewManagment LabelName = AllAttributes.FirstOrDefault(x => ((x.AttributeActivated != null) ? x.AttributeActivated.Key == prop.Name : false) &&
                                x.AttributeActivated.Tabel == Helpers.Constants.TablesNames.TLIcivilNonSteel.ToString() &&
                                x.Enable && x.AttributeActivated.DataType != "List" && x.Id != 0);

                            if (LabelName != null)
                            {
                                object PropObject = prop.GetValue(CivilNonSteelInstallationObject, null);
                                ((IDictionary<String, Object>)DateTimeAttributes).Add(new KeyValuePair<string, object>(LabelName.AttributeActivated.Label, PropObject));
                            }
                        }
                    }

                    //
                    // Library Dynamic Attributes... (DateTime DataType Attribute)
                    // 
                    if (DateTimeDynamicInstallationAttributesViewModel != null ? DateTimeDynamicInstallationAttributesViewModel.Count() > 0 : false)
                    {
                        var temp = DateTimeDynamicInstallationAttributesViewModel.Select(x => x.DynamicAttId).ToList();
                        List<TLIdynamicAtt> DateTimeInstallationDynamicAttributes = _unitOfWork.DynamicAttRepository.GetIncludeWhere(x =>
                           !x.disable && x.tablesNames.TableName == Helpers.Constants.TablesNames.TLIcivilNonSteel.ToString() &&
                           !x.LibraryAtt && x.DataType.Name.ToLower() == "datetime" &&
                            temp.Any(y => y == x.Id), x => x.tablesNames).ToList();
                        var tempno = DateTimeInstallationDynamicAttributes.Select(x => x.Key.ToLower()).ToList();
                        List<TLIdynamicAttInstValue> DateTimeDynamicAttInstValues = _unitOfWork.DynamicAttInstValueRepository.GetIncludeWhere(x =>
                            x.InventoryId == CivilNonSteelInstallationObject.Id && !x.disable &&
                           !x.DynamicAtt.LibraryAtt &&
                            tempno.Any(y => y == x.DynamicAtt.Key.ToLower()) &&
                            x.tablesNames.TableName == Helpers.Constants.TablesNames.TLIcivilNonSteel.ToString()
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

                    ((IDictionary<String, Object>)DynamicCivilNonSteelInstallation).Add(new KeyValuePair<string, object>("DateTimeAttributes", DateTimeAttributes));

                    OutPutList.Add(DynamicCivilNonSteelInstallation);
                }

                CivilTableDisplay.Model = OutPutList;

                if (WithFilterData == true)
                {
                    CivilTableDisplay.filters = _unitOfWork.CivilNonSteelRepository.GetRelatedTables();
                }
                else
                {
                    CivilTableDisplay.filters = null;
                }

                return new Response<ReturnWithFilters<object>>(true, CivilTableDisplay, null, null, (int)Helpers.Constants.ApiReturnCode.success, Count);
            }
            catch (Exception err)
            {
                return new Response<ReturnWithFilters<object>>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
        }
        #endregion
        #region Get All Civils Installations And Libraries Enabled Attributes...
        public Response<AllCivilsViewModel> GetAllCivils(SiteBaseFilter BaseFilter, bool WithFilterData, ParameterPagination parameterPagination)
        {
            try
            {
                AllCivilsViewModel MainOutPut = new AllCivilsViewModel();
                int count = 0;

                MainOutPut.CivilWithLegs = GetCivilWithLegsWithEnableAtt(BaseFilter, WithFilterData, null, parameterPagination).Data;
                MainOutPut.CivilWithoutLeg = GetCivilWithoutLegWithEnableAtt(BaseFilter, WithFilterData, null, parameterPagination, 0).Data;
                MainOutPut.CivilNonSteel = GetCivilNonSteelWithEnableAtt(BaseFilter, WithFilterData, null, parameterPagination).Data;

                return new Response<AllCivilsViewModel>(true, MainOutPut, null, null, (int)Helpers.Constants.ApiReturnCode.success, count);
            }
            catch (Exception err)
            {
                return new Response<AllCivilsViewModel>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
        }
        #endregion
        #region Helper Methods..
        public List<TLIcivilSiteDate> GetMaxInstallationDate(out int Count, List<TLIcivilSiteDate> Copy, string Kind, ParameterPagination parameterPagination)
        {
            List<TLIcivilSiteDate> AfterDeleteDuplicate = new List<TLIcivilSiteDate>();
            if (Kind == "CivilWithLegs")
            {
                foreach (TLIcivilSiteDate item in Copy)
                {
                    TLIcivilSiteDate CheckIfExist = AfterDeleteDuplicate.FirstOrDefault(x => x.allCivilInst.civilWithLegsId == item.allCivilInst.civilWithLegsId);
                    if (CheckIfExist != null)
                    {
                        if (CheckIfExist.InstallationDate < item.InstallationDate)
                        {
                            AfterDeleteDuplicate.Remove(CheckIfExist);
                            AfterDeleteDuplicate.Add(item);
                        }
                    }
                    else
                        AfterDeleteDuplicate.Add(item);
                }
            }
            else if (Kind == "CivilWithoutLegs")
            {
                foreach (TLIcivilSiteDate item in Copy)
                {
                    TLIcivilSiteDate CheckIfExist = AfterDeleteDuplicate.FirstOrDefault(x => x.allCivilInst.civilWithoutLegId == item.allCivilInst.civilWithoutLegId);
                    if (CheckIfExist != null)
                    {
                        if (CheckIfExist.InstallationDate < item.InstallationDate)
                        {
                            AfterDeleteDuplicate.Remove(CheckIfExist);
                            AfterDeleteDuplicate.Add(item);
                        }
                    }
                    else
                        AfterDeleteDuplicate.Add(item);
                }
            }
            else if (Kind == "CivilNonSteel")
            {
                foreach (TLIcivilSiteDate item in Copy)
                {
                    TLIcivilSiteDate CheckIfExist = AfterDeleteDuplicate.FirstOrDefault(x => x.allCivilInst.civilNonSteelId == item.allCivilInst.civilNonSteelId);
                    if (CheckIfExist != null)
                    {
                        if (CheckIfExist.InstallationDate < item.InstallationDate)
                        {
                            AfterDeleteDuplicate.Remove(CheckIfExist);
                            AfterDeleteDuplicate.Add(item);
                        }
                    }
                    else
                        AfterDeleteDuplicate.Add(item);
                }
            }
            Count = AfterDeleteDuplicate.Count();


            if (parameterPagination == null)
            {
                return AfterDeleteDuplicate.Skip((0) * 100).Take(100).ToList();

            }

            return AfterDeleteDuplicate.Skip((parameterPagination.PageNumber - 1) * parameterPagination.PageSize).
                Take(parameterPagination.PageSize).ToList();

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
        public List<TLIcivilSiteDate> GetCivilSiteDateBySiteBaseFilter(SiteBaseFilter BaseFilter, string Kind, CombineFilters CombineFilters, int? CategoryId)
        {
            List<StringFilterObjectList> AttributeFilters = new List<StringFilterObjectList>();

            List<AttributeActivatedViewModel> CivilInstallationAttribute = new List<AttributeActivatedViewModel>();

            List<FilterObjectList> ObjectAttributeFilters = CombineFilters.filters;
            List<DateFilterViewModel> DateFilter = CombineFilters.DateFilter;

            if (Kind == "CivilWithLegs")
            {
                if ((DateFilter != null ? DateFilter.Count() > 0 : false) ||
                    (ObjectAttributeFilters != null && ObjectAttributeFilters.Count > 0))
                {
                    CivilInstallationAttribute = _mapper.Map<List<AttributeActivatedViewModel>>(_unitOfWork.AttributeViewManagmentRepository.GetIncludeWhere(x =>
                        x.Enable && x.AttributeActivatedId != null &&
                        x.EditableManagmentView.View == Helpers.Constants.EditableManamgmantViewNames.CivilWithLegInstallation.ToString() &&
                        x.EditableManagmentView.TLItablesNames1.TableName == Helpers.Constants.TablesNames.TLIcivilWithLegs.ToString(),
                            x => x.AttributeActivated, x => x.EditableManagmentView, x => x.EditableManagmentView.TLItablesNames1,
                            x => x.EditableManagmentView.TLItablesNames2)
                    .Select(x => x.AttributeActivated).ToList());
                }
            }
            else if (Kind == "CivilWithoutLegs")
            {
                if ((DateFilter != null ? DateFilter.Count() > 0 : false) ||
                    (ObjectAttributeFilters != null && ObjectAttributeFilters.Count > 0))
                {
                    List<AttActivatedCategoryViewModel> CivilWithoutLegCatgeoryData = _mapper.Map<List<AttActivatedCategoryViewModel>>(_unitOfWork.AttActivatedCategoryRepository
                        .GetWhere(x => x.civilWithoutLegCategoryId == CategoryId && x.enable && !x.IsLibrary).ToList());

                    string CategoryName = _unitOfWork.CivilWithoutLegCategoryRepository.GetByID(CategoryId.Value).Name;
                    string EditableMangmentCategoryViewName = "";
                    if (CategoryName == "Mast")
                        EditableMangmentCategoryViewName = Helpers.Constants.EditableManamgmantViewNames.CivilWithoutLegInstallationMast.ToString();
                    else if (CategoryName == "Capsule")
                        EditableMangmentCategoryViewName = Helpers.Constants.EditableManamgmantViewNames.CivilWithoutLegInstallationCapsule.ToString();
                    else if (CategoryName == "Monopole")
                        EditableMangmentCategoryViewName = Helpers.Constants.EditableManamgmantViewNames.CivilWithoutLegInstallationMonopole.ToString();

                    CivilInstallationAttribute = _mapper.Map<List<AttributeActivatedViewModel>>(_unitOfWork.AttributeViewManagmentRepository.GetIncludeWhere(x =>
                        x.Enable && x.AttributeActivatedId != null &&
                        x.EditableManagmentView.View == EditableMangmentCategoryViewName,
                            x => x.AttributeActivated, x => x.EditableManagmentView, x => x.EditableManagmentView.TLItablesNames1,
                            x => x.EditableManagmentView.TLItablesNames2).Select(x => x.AttributeActivated).ToList());

                    foreach (AttributeActivatedViewModel CivilWithoutLegAttribute in CivilInstallationAttribute)
                    {
                        AttActivatedCategoryViewModel AttActivatedCategory = CivilWithoutLegCatgeoryData
                            .FirstOrDefault(x => x.attributeActivatedId == CivilWithoutLegAttribute.Id);

                        CivilWithoutLegAttribute.Required = AttActivatedCategory.Required;
                        CivilWithoutLegAttribute.Label = AttActivatedCategory.Label;
                        CivilWithoutLegAttribute.Description = AttActivatedCategory.Description;
                        CivilWithoutLegAttribute.enable = AttActivatedCategory.enable;
                    }
                }
            }
            else if (Kind == "CivilNonSteel")
            {
                if ((DateFilter != null ? DateFilter.Count() > 0 : false) ||
                    (ObjectAttributeFilters != null && ObjectAttributeFilters.Count > 0))
                {
                    CivilInstallationAttribute = _mapper.Map<List<AttributeActivatedViewModel>>(_unitOfWork.AttributeViewManagmentRepository.GetIncludeWhere(x =>
                        x.Enable && x.AttributeActivatedId != null &&
                        x.EditableManagmentView.View == Helpers.Constants.EditableManamgmantViewNames.CivilNonSteelInstallation.ToString(),
                            x => x.AttributeActivated, x => x.EditableManagmentView, x => x.EditableManagmentView.TLItablesNames1,
                            x => x.EditableManagmentView.TLItablesNames2).Select(x => x.AttributeActivated).ToList());
                }
            }

            if (ObjectAttributeFilters != null && ObjectAttributeFilters.Count > 0)
            {
                List<AttributeActivatedViewModel> NotDateCivilInstallationAttribute = CivilInstallationAttribute.Where(x =>
                    x.DataType.ToLower() != "datetime").ToList();

                foreach (FilterObjectList item in ObjectAttributeFilters)
                {
                    List<string> value = item.value.Select(x => x.ToString().ToLower()).ToList();

                    AttributeActivatedViewModel AttributeKey = NotDateCivilInstallationAttribute.FirstOrDefault(x =>
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
                List<AttributeActivatedViewModel> DateCivilInstallationAttribute = CivilInstallationAttribute.Where(x =>
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

            if (Kind == "CivilWithLegs")
            {
                List<int> CivilWithLegsIds = new List<int>();
                List<int> WithoutDateFilterCivilWithLegsInstallation = new List<int>();
                List<int> WithDateFilterCivilWithLegsInstallation = new List<int>();

                if (AttributeFilters != null ? AttributeFilters.Count() > 0 : false)
                {
                    //
                    // Installation Dynamic Attributes...
                    //
                    List<TLIdynamicAtt> InstDynamicAttListIds = _unitOfWork.DynamicAttRepository.GetIncludeWhere(x =>
                        AttributeFilters.AsEnumerable().Select(y => y.key.ToLower()).Contains(x.Key.ToLower()) &&
                        !x.LibraryAtt && !x.disable &&
                        x.tablesNames.TableName == Helpers.Constants.TablesNames.TLIcivilWithLegs.ToString()
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
                    bool AttrInstExist = typeof(CivilWithLegsViewModel).GetProperties().ToList().Exists(x =>
                        AttributeFilters.Where(y => y.key.ToLower() != "Id".ToLower()).Select(y =>
                            y.key.ToLower()).Contains(x.Name.ToLower()));

                    List<int> InstallationAttributeActivated = new List<int>();

                    if (AttrInstExist)
                    {
                        List<PropertyInfo> NotStringProps = typeof(CivilWithLegsViewModel).GetProperties().Where(x =>
                            x.PropertyType.Name.ToLower() != "string" &&
                            AttributeFilters.Select(y =>
                                y.key.ToLower()).Contains(x.Name.ToLower())).ToList();

                        List<PropertyInfo> StringProps = typeof(CivilWithLegsViewModel).GetProperties().Where(x =>
                            x.PropertyType.Name.ToLower() == "string" &&
                            AttributeFilters.Select(y =>
                                y.key.ToLower()).Contains(x.Name.ToLower())).ToList();

                        List<StringFilterObjectList> AttrInstAttributeFilters = AttributeFilters.Where(x =>
                            NotStringProps.Select(y => y.Name.ToLower()).Contains(x.key.ToLower()) ||
                            StringProps.Select(y => y.Name.ToLower()).Contains(x.key.ToLower())).ToList();

                        //InstallationAttributeActivated = _unitOfWork.CivilWithLegsRepository.GetWhere(x =>
                        //     AttrInstAttributeFilters.All(z =>
                        //        NotStringProps.Exists(y => (z.key.ToLower() == y.Name.ToLower()) && (y.GetValue(_mapper.Map<CivilWithLegsViewModel>(x), null) != null ? z.value.Contains(y.GetValue(_mapper.Map<CivilWithLegsViewModel>(x), null).ToString().ToLower()) : false)) ||
                        //        StringProps.Exists(y => (z.key.ToLower() == y.Name.ToLower()) && (z.value.Any(w =>
                        //             y.GetValue(_mapper.Map<CivilWithLegsViewModel>(x), null) != null ? y.GetValue(_mapper.Map<CivilWithLegsViewModel>(x), null).ToString().ToLower().StartsWith(w.ToLower()) : false))))
                        // ).Select(i => i.Id).ToList();

                        IEnumerable<TLIcivilWithLegs> Installations = _unitOfWork.CivilWithLegsRepository.GetAllWithoutCount();

                        foreach (StringFilterObjectList InstallationProp in AttrInstAttributeFilters)
                        {
                            if (StringProps.Select(x => x.Name.ToLower()).Contains(InstallationProp.key.ToLower()))
                            {
                                Installations = Installations.Where(x => StringProps.AsEnumerable().FirstOrDefault(y => (InstallationProp.key.ToLower() == y.Name.ToLower()) && (InstallationProp.value.AsEnumerable().FirstOrDefault(w =>
                                     y.GetValue(_mapper.Map<CivilWithLegsViewModel>(x), null) != null ? y.GetValue(_mapper.Map<CivilWithLegsViewModel>(x), null).ToString().ToLower().StartsWith(w.ToLower()) : false) != null)) != null).AsEnumerable();
                            }
                            else if (NotStringProps.Select(x => x.Name.ToLower()).Contains(InstallationProp.key.ToLower()))
                            {
                                Installations = Installations.Where(x => NotStringProps.AsEnumerable().FirstOrDefault(y => (InstallationProp.key.ToLower() == y.Name.ToLower()) && (y.GetValue(_mapper.Map<CivilWithLegsViewModel>(x), null) != null ?
                                    InstallationProp.value.AsEnumerable().Contains(y.GetValue(_mapper.Map<CivilWithLegsViewModel>(x), null).ToString().ToLower()) : false)) != null).AsEnumerable();
                            }
                        }

                        InstallationAttributeActivated = Installations.Select(x => x.Id).ToList();
                    }

                    //
                    // Installation (Attribute Activated + Dynamic) Attributes...
                    //
                    if (AttrInstExist && DynamicInstExist)
                    {
                        WithoutDateFilterCivilWithLegsInstallation = InstallationAttributeActivated.Intersect(DynamicInstValueListIds).ToList();
                    }
                    else if (AttrInstExist)
                    {
                        WithoutDateFilterCivilWithLegsInstallation = InstallationAttributeActivated;
                    }
                    else if (DynamicInstExist)
                    {
                        WithoutDateFilterCivilWithLegsInstallation = DynamicInstValueListIds;
                    }
                }

                if (DateFilter != null ? DateFilter.Count() > 0 : false)
                {
                    List<TLIdynamicAtt> DateTimeInstDynamicAttListIds = _unitOfWork.DynamicAttRepository.GetIncludeWhere(x =>
                        AfterConvertDateFilters.AsEnumerable().Select(y => y.key.ToLower()).Contains(x.Key.ToLower()) &&
                        !x.LibraryAtt && !x.disable &&
                        x.tablesNames.TableName == Helpers.Constants.TablesNames.TLIcivilWithLegs.ToString()
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
                    List<PropertyInfo> InstallationProps = typeof(CivilWithLegsViewModel).GetProperties().Where(x =>
                        AfterConvertDateFilters.Select(y =>
                            y.key.ToLower()).Contains(x.Name.ToLower())).ToList();

                    List<int> InstallationAttributeActivatedIds = new List<int>();
                    bool AttrInstExist = false;

                    if (InstallationProps != null)
                    {
                        AttrInstExist = true;

                        List<DateFilterViewModel> InstallationPropsAttributeFilters = AfterConvertDateFilters.Where(x =>
                            InstallationProps.Select(y => y.Name.ToLower()).Contains(x.key.ToLower())).ToList();

                        IEnumerable<TLIcivilWithLegs> Installations = _unitOfWork.CivilWithLegsRepository.GetAllWithoutCount();

                        foreach (DateFilterViewModel InstallationProp in InstallationPropsAttributeFilters)
                        {
                            Installations = Installations.Where(x => InstallationProps.Exists(y => (InstallationProp.key.ToLower() == y.Name.ToLower()) && ((y.GetValue(_mapper.Map<CivilWithLegsViewModel>(x), null) != null) ?
                                ((InstallationProp.DateFrom.Date <= Convert.ToDateTime(y.GetValue(_mapper.Map<CivilWithLegsViewModel>(x), null)).Date) &&
                                (InstallationProp.DateTo.Date >= Convert.ToDateTime(y.GetValue(_mapper.Map<CivilWithLegsViewModel>(x), null)).Date)) : (false))));
                        }

                        InstallationAttributeActivatedIds = Installations.Select(x => x.Id).ToList();
                    }

                    //
                    // Installation (Attribute Activated + Dynamic) Attributes...
                    //
                    if (AttrInstExist && DynamicInstExist)
                    {
                        WithDateFilterCivilWithLegsInstallation = InstallationAttributeActivatedIds.Intersect(DynamicInstValueListIds).ToList();
                    }
                    else if (AttrInstExist)
                    {
                        WithDateFilterCivilWithLegsInstallation = InstallationAttributeActivatedIds;
                    }
                    else if (DynamicInstExist)
                    {
                        WithDateFilterCivilWithLegsInstallation = DynamicInstValueListIds;
                    }
                }

                if ((DateFilter != null ? DateFilter.Count() > 0 : false) ||
                    (AttributeFilters != null ? AttributeFilters.Count() > 0 : false))
                {
                    if ((AttributeFilters != null ? AttributeFilters.Count > 0 : false) &&
                        (AfterConvertDateFilters != null ? AfterConvertDateFilters.Count() > 0 : false))
                    {
                        CivilWithLegsIds = WithoutDateFilterCivilWithLegsInstallation.Intersect(WithDateFilterCivilWithLegsInstallation).ToList();
                    }
                    else if (AttributeFilters != null ? AttributeFilters.Count > 0 : false)
                    {
                        CivilWithLegsIds = WithoutDateFilterCivilWithLegsInstallation;
                    }
                    else if (AfterConvertDateFilters != null ? AfterConvertDateFilters.Count() > 0 : false)
                    {
                        CivilWithLegsIds = WithDateFilterCivilWithLegsInstallation;
                    }

                    return _unitOfWork.CivilSiteDateRepository.GetIncludeWhere(x => (
                        (x.SiteCode == BaseFilter.SiteCode) &&
                        (!x.Dismantle) &&
                        (BaseFilter.ItemStatusId != null ? (
                            x.allCivilInst != null ? (
                                x.allCivilInst.ItemStatusId != null ? (
                                    x.allCivilInst.ItemStatusId == BaseFilter.ItemStatusId)
                                : false)
                            : false)
                        : true) &&
                        (BaseFilter.TicketId != null ? (
                            x.allCivilInst != null ? (
                                x.allCivilInst.TicketId != null ? (
                                    x.allCivilInst.TicketId == BaseFilter.TicketId)
                                : false)
                            : false)
                        : true) &&
                        (x.allCivilInst.civilWithLegsId != null) &&
                        (x.allCivilInst.Draft == false) &&

                        CivilWithLegsIds.Contains(x.allCivilInst.civilWithLegsId.Value)
                    ), x => x.allCivilInst, x => x.allCivilInst.civilWithLegs, x => x.allCivilInst.civilWithLegs.BaseCivilWithLegType, x => x.allCivilInst.civilWithLegs.baseType,
                       x => x.allCivilInst.civilWithLegs.CivilWithLegsLib, x => x.allCivilInst.civilWithLegs.enforcmentCategory, x => x.allCivilInst.civilWithLegs.GuyLineType,
                       x => x.allCivilInst.civilWithLegs.locationType, x => x.allCivilInst.civilWithLegs.Owner, x => x.allCivilInst.civilWithLegs.SupportTypeImplemented).ToList();
                }

                return _unitOfWork.CivilSiteDateRepository.GetIncludeWhere(x => (
                    (x.SiteCode == BaseFilter.SiteCode) &&
                    (!x.Dismantle) &&
                    (BaseFilter.ItemStatusId != null ? (
                        x.allCivilInst != null ? (
                            x.allCivilInst.ItemStatusId != null ? (
                                x.allCivilInst.ItemStatusId == BaseFilter.ItemStatusId)
                            : false)
                        : false)
                    : true) &&
                    (BaseFilter.TicketId != null ? (
                        x.allCivilInst != null ? (
                            x.allCivilInst.TicketId != null ? (
                                x.allCivilInst.TicketId == BaseFilter.TicketId)
                            : false)
                        : false)
                    : true) &&
                    (x.allCivilInst.civilWithLegsId != null) &&
                    (x.allCivilInst.Draft == false)

                ), x => x.allCivilInst, x => x.allCivilInst.civilWithLegs, x => x.allCivilInst.civilWithLegs.BaseCivilWithLegType, x => x.allCivilInst.civilWithLegs.baseType,
                    x => x.allCivilInst.civilWithLegs.CivilWithLegsLib, x => x.allCivilInst.civilWithLegs.enforcmentCategory, x => x.allCivilInst.civilWithLegs.GuyLineType,
                    x => x.allCivilInst.civilWithLegs.locationType, x => x.allCivilInst.civilWithLegs.Owner, x => x.allCivilInst.civilWithLegs.SupportTypeImplemented).ToList();
            }
            else if (Kind == "CivilWithoutLegs")
            {
                List<int> CivilWithoutLegIds = new List<int>();
                List<int> WithoutDateFilterCivilWithoutLegInstallation = new List<int>();
                List<int> WithDateFilterCivilWithoutLegInstallation = new List<int>();

                if (AttributeFilters != null && AttributeFilters.Count > 0)
                {
                    //
                    // Installation Dynamic Attributes...
                    //
                    List<TLIdynamicAtt> InstDynamicAttListIds = _unitOfWork.DynamicAttRepository.GetIncludeWhere(x =>
                        AttributeFilters.AsEnumerable().Select(y => y.key.ToLower()).Contains(x.Key.ToLower()) &&
                        !x.LibraryAtt && !x.disable &&
                        x.tablesNames.TableName == Helpers.Constants.TablesNames.TLIcivilWithoutLeg.ToString()
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
                    bool AttrInstExist = typeof(CivilWithoutLegViewModel).GetProperties().ToList().Exists(x =>
                        AttributeFilters.Where(y => y.key.ToLower() != "Id".ToLower()).Select(y =>
                            y.key.ToLower()).Contains(x.Name.ToLower()));

                    List<int> InstallationAttributeActivated = new List<int>();
                    if (AttrInstExist)
                    {
                        List<PropertyInfo> NotStringProps = typeof(CivilWithoutLegViewModel).GetProperties().Where(x =>
                            x.PropertyType.Name.ToLower() != "string" &&
                            AttributeFilters.Select(y =>
                                y.key.ToLower()).Contains(x.Name.ToLower())).ToList();

                        List<PropertyInfo> StringProps = typeof(CivilWithoutLegViewModel).GetProperties().Where(x =>
                            x.PropertyType.Name.ToLower() == "string" &&
                            AttributeFilters.Select(y =>
                                y.key.ToLower()).Contains(x.Name.ToLower())).ToList();

                        List<StringFilterObjectList> AttrInstAttributeFilters = AttributeFilters.Where(x =>
                            NotStringProps.Select(y => y.Name.ToLower()).Contains(x.key.ToLower()) ||
                            StringProps.Select(y => y.Name.ToLower()).Contains(x.key.ToLower())).ToList();

                        //InstallationAttributeActivated = _unitOfWork.CivilWithoutLegRepository.GetWhere(x =>
                        //     AttrInstAttributeFilters.All(z =>
                        //        NotStringProps.Exists(y => (z.key.ToLower() == y.Name.ToLower()) && (y.GetValue(_mapper.Map<CivilWithoutLegViewModel>(x), null) != null ? z.value.Contains(y.GetValue(_mapper.Map<CivilWithoutLegViewModel>(x), null).ToString().ToLower()) : false)) ||
                        //        StringProps.Exists(y => (z.key.ToLower() == y.Name.ToLower()) && (z.value.Any(w =>
                        //             y.GetValue(_mapper.Map<CivilWithoutLegViewModel>(x), null) != null ? y.GetValue(_mapper.Map<CivilWithoutLegViewModel>(x), null).ToString().ToLower().StartsWith(w.ToLower()) : false))))
                        // ).Select(i => i.Id).ToList();

                        IEnumerable<TLIcivilWithoutLeg> Installations = _unitOfWork.CivilWithoutLegRepository.GetAllWithoutCount();

                        foreach (StringFilterObjectList InstallationProp in AttrInstAttributeFilters)
                        {
                            if (StringProps.Select(x => x.Name.ToLower()).Contains(InstallationProp.key.ToLower()))
                            {
                                Installations = Installations.Where(x => StringProps.AsEnumerable().FirstOrDefault(y => (InstallationProp.key.ToLower() == y.Name.ToLower()) && (InstallationProp.value.AsEnumerable().FirstOrDefault(w =>
                                     y.GetValue(_mapper.Map<CivilWithoutLegViewModel>(x), null) != null ? y.GetValue(_mapper.Map<CivilWithoutLegViewModel>(x), null).ToString().ToLower().StartsWith(w.ToLower()) : false) != null)) != null).AsEnumerable();
                            }
                            else if (NotStringProps.Select(x => x.Name.ToLower()).Contains(InstallationProp.key.ToLower()))
                            {
                                Installations = Installations.Where(x => NotStringProps.AsEnumerable().FirstOrDefault(y => (InstallationProp.key.ToLower() == y.Name.ToLower()) && (y.GetValue(_mapper.Map<CivilWithoutLegViewModel>(x), null) != null ?
                                    InstallationProp.value.AsEnumerable().Contains(y.GetValue(_mapper.Map<CivilWithoutLegViewModel>(x), null).ToString().ToLower()) : false)) != null).AsEnumerable();
                            }
                        }

                        InstallationAttributeActivated = Installations.Select(x => x.Id).ToList();
                    }

                    //
                    // Installation (Attribute Activated + Dynamic) Attributes...
                    //
                    if (AttrInstExist && DynamicInstExist)
                    {
                        WithoutDateFilterCivilWithoutLegInstallation = InstallationAttributeActivated.Intersect(DynamicInstValueListIds).ToList();
                    }
                    else if (AttrInstExist)
                    {
                        WithoutDateFilterCivilWithoutLegInstallation = InstallationAttributeActivated;
                    }
                    else if (DynamicInstExist)
                    {
                        WithoutDateFilterCivilWithoutLegInstallation = DynamicInstValueListIds;
                    }
                }

                if (DateFilter != null ? DateFilter.Count() > 0 : false)
                {
                    List<TLIdynamicAtt> DateTimeInstDynamicAttListIds = _unitOfWork.DynamicAttRepository.GetIncludeWhere(x =>
                        AfterConvertDateFilters.AsEnumerable().Select(y => y.key.ToLower()).Contains(x.Key.ToLower()) &&
                        !x.LibraryAtt && !x.disable &&
                        x.tablesNames.TableName == Helpers.Constants.TablesNames.TLIcivilWithoutLeg.ToString()
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
                    List<PropertyInfo> InstallationProps = typeof(CivilWithoutLegViewModel).GetProperties().Where(x =>
                        AfterConvertDateFilters.Select(y =>
                            y.key.ToLower()).Contains(x.Name.ToLower())).ToList();

                    List<int> InstallationAttributeActivatedIds = new List<int>();
                    bool AttrInstExist = false;

                    if (InstallationProps != null)
                    {
                        AttrInstExist = true;

                        List<DateFilterViewModel> InstallationPropsAttributeFilters = AfterConvertDateFilters.Where(x =>
                            InstallationProps.Select(y => y.Name.ToLower()).Contains(x.key.ToLower())).ToList();

                        IEnumerable<TLIcivilWithoutLeg> Installations = _unitOfWork.CivilWithoutLegRepository.GetAllWithoutCount();

                        foreach (DateFilterViewModel InstallationProp in InstallationPropsAttributeFilters)
                        {
                            Installations = Installations.Where(x => InstallationProps.Exists(y => (InstallationProp.key.ToLower() == y.Name.ToLower()) && ((y.GetValue(_mapper.Map<CivilWithoutLegViewModel>(x), null) != null) ?
                                ((InstallationProp.DateFrom.Date <= Convert.ToDateTime(y.GetValue(_mapper.Map<CivilWithoutLegViewModel>(x), null)).Date) &&
                                    (InstallationProp.DateTo.Date >= Convert.ToDateTime(y.GetValue(_mapper.Map<CivilWithoutLegViewModel>(x), null)).Date)) : (false))));
                        }

                        InstallationAttributeActivatedIds = Installations.Select(x => x.Id).ToList();
                    }

                    //
                    // Installation (Attribute Activated + Dynamic) Attributes...
                    //
                    if (AttrInstExist && DynamicInstExist)
                    {
                        WithDateFilterCivilWithoutLegInstallation = InstallationAttributeActivatedIds.Intersect(DynamicInstValueListIds).ToList();
                    }
                    else if (AttrInstExist)
                    {
                        WithDateFilterCivilWithoutLegInstallation = InstallationAttributeActivatedIds;
                    }
                    else if (DynamicInstExist)
                    {
                        WithDateFilterCivilWithoutLegInstallation = DynamicInstValueListIds;
                    }
                }

                if ((DateFilter != null ? DateFilter.Count() > 0 : false) ||
                    (AttributeFilters != null ? AttributeFilters.Count() > 0 : false))
                {
                    if ((AttributeFilters != null ? AttributeFilters.Count > 0 : false) &&
                        (AfterConvertDateFilters != null ? AfterConvertDateFilters.Count() > 0 : false))
                    {
                        CivilWithoutLegIds = WithoutDateFilterCivilWithoutLegInstallation.Intersect(WithDateFilterCivilWithoutLegInstallation).ToList();
                    }
                    else if (AttributeFilters != null ? AttributeFilters.Count > 0 : false)
                    {
                        CivilWithoutLegIds = WithoutDateFilterCivilWithoutLegInstallation;
                    }
                    else if (AfterConvertDateFilters != null ? AfterConvertDateFilters.Count() > 0 : false)
                    {
                        CivilWithoutLegIds = WithDateFilterCivilWithoutLegInstallation;
                    }

                    return _unitOfWork.CivilSiteDateRepository.GetIncludeWhere(x => (
                        (x.SiteCode == BaseFilter.SiteCode) &&
                        (!x.Dismantle) &&
                        (x.allCivilInst.civilWithoutLeg.CivilWithoutlegsLib.CivilWithoutLegCategoryId == CategoryId) &&
                        (BaseFilter.ItemStatusId != null ? (
                            x.allCivilInst != null ? (
                                x.allCivilInst.ItemStatusId != null ? (
                                    x.allCivilInst.ItemStatusId == BaseFilter.ItemStatusId)
                                : false)
                            : false)
                        : true) &&
                        (BaseFilter.TicketId != null ? (
                            x.allCivilInst != null ? (
                                x.allCivilInst.TicketId != null ? (
                                    x.allCivilInst.TicketId == BaseFilter.TicketId)
                                : false)
                            : false)
                        : true) &&
                        (x.allCivilInst.civilWithoutLegId != null) &&
                        (x.allCivilInst.Draft == false) &&

                        CivilWithoutLegIds.Contains(x.allCivilInst.civilWithoutLegId.Value)
                    ), x => x.allCivilInst, x => x.allCivilInst.civilWithoutLeg, x => x.allCivilInst.civilWithoutLeg.CivilWithoutlegsLib,
                       x => x.allCivilInst.civilWithoutLeg.Owner, x => x.allCivilInst.civilWithoutLeg.subType).ToList();
                }

                return _unitOfWork.CivilSiteDateRepository.GetIncludeWhere(x => (
                    (x.allCivilInst.civilWithoutLeg.CivilWithoutlegsLib.CivilWithoutLegCategoryId == CategoryId) &&
                    (x.SiteCode == BaseFilter.SiteCode) &&
                    (!x.Dismantle) &&
                    (BaseFilter.ItemStatusId != null ? (
                        x.allCivilInst != null ? (
                            x.allCivilInst.ItemStatusId != null ? (
                               x.allCivilInst.ItemStatusId == BaseFilter.ItemStatusId)
                            : false)
                        : false)
                    : true) &&
                    (BaseFilter.TicketId != null ? (
                        x.allCivilInst != null ? (
                           x.allCivilInst.TicketId != null ? (
                                x.allCivilInst.TicketId == BaseFilter.TicketId)
                            : false)
                        : false)
                    : true) &&
                    (x.allCivilInst.civilWithoutLegId != null) &&
                    (x.allCivilInst.Draft == false)

                ), x => x.allCivilInst, x => x.allCivilInst.civilWithoutLeg, x => x.allCivilInst.civilWithoutLeg.CivilWithoutlegsLib,
                    x => x.allCivilInst.civilWithoutLeg.Owner, x => x.allCivilInst.civilWithoutLeg.subType).ToList();
            }
            else if (Kind == "CivilNonSteel")
            {
                List<int> CivilNonSteelIds = new List<int>();
                List<int> WithoutDateFilterCivilNonSteelInstallation = new List<int>();
                List<int> WithDateFilterCivilNonSteelInstallation = new List<int>();

                if (AttributeFilters != null && AttributeFilters.Count > 0)
                {
                    //
                    // Installation Dynamic Attributes...
                    //
                    List<TLIdynamicAtt> InstDynamicAttListIds = _unitOfWork.DynamicAttRepository.GetIncludeWhere(x =>
                        AttributeFilters.AsEnumerable().Select(y => y.key.ToLower()).Contains(x.Key.ToLower()) &&
                        !x.LibraryAtt && !x.disable &&
                        x.tablesNames.TableName == Helpers.Constants.TablesNames.TLIcivilNonSteel.ToString()
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
                    bool AttrInstExist = typeof(CivilNonSteelViewModel).GetProperties().ToList().Exists(x =>
                        AttributeFilters.Where(y => y.key.ToLower() != "Id".ToLower()).Select(y =>
                            y.key.ToLower()).Contains(x.Name.ToLower()));

                    List<int> InstallationAttributeActivated = new List<int>();
                    if (AttrInstExist)
                    {
                        List<PropertyInfo> NotStringProps = typeof(CivilNonSteelViewModel).GetProperties().Where(x =>
                            x.PropertyType.Name.ToLower() != "string" &&
                            AttributeFilters.Select(y =>
                                y.key.ToLower()).Contains(x.Name.ToLower())).ToList();

                        List<PropertyInfo> StringProps = typeof(CivilNonSteelViewModel).GetProperties().Where(x =>
                            x.PropertyType.Name.ToLower() == "string" &&
                            AttributeFilters.Select(y =>
                                y.key.ToLower()).Contains(x.Name.ToLower())).ToList();

                        List<StringFilterObjectList> AttrInstAttributeFilters = AttributeFilters.Where(x =>
                            NotStringProps.Select(y => y.Name.ToLower()).Contains(x.key.ToLower()) ||
                            StringProps.Select(y => y.Name.ToLower()).Contains(x.key.ToLower())).ToList();

                        //InstallationAttributeActivated = _unitOfWork.CivilNonSteelRepository.GetWhere(x =>
                        //     AttrInstAttributeFilters.All(z =>
                        //        NotStringProps.Exists(y => (z.key.ToLower() == y.Name.ToLower()) && (y.GetValue(_mapper.Map<CivilNonSteelViewModel>(x), null) != null ? z.value.Contains(y.GetValue(_mapper.Map<CivilNonSteelViewModel>(x), null).ToString().ToLower()) : false)) ||
                        //        StringProps.Exists(y => (z.key.ToLower() == y.Name.ToLower()) && (z.value.Any(w =>
                        //             y.GetValue(_mapper.Map<CivilNonSteelViewModel>(x), null) != null ? y.GetValue(_mapper.Map<CivilNonSteelViewModel>(x), null).ToString().ToLower().StartsWith(w.ToLower()) : false))))
                        // ).Select(i => i.Id).ToList();

                        IEnumerable<TLIcivilNonSteel> Installations = _unitOfWork.CivilNonSteelRepository.GetAllWithoutCount();

                        foreach (StringFilterObjectList InstallationProp in AttrInstAttributeFilters)
                        {
                            if (StringProps.Select(x => x.Name.ToLower()).Contains(InstallationProp.key.ToLower()))
                            {
                                Installations = Installations.Where(x => StringProps.AsEnumerable().FirstOrDefault(y => (InstallationProp.key.ToLower() == y.Name.ToLower()) && (InstallationProp.value.AsEnumerable().FirstOrDefault(w =>
                                     y.GetValue(_mapper.Map<CivilNonSteelViewModel>(x), null) != null ? y.GetValue(_mapper.Map<CivilNonSteelViewModel>(x), null).ToString().ToLower().StartsWith(w.ToLower()) : false) != null)) != null).AsEnumerable();
                            }
                            else if (NotStringProps.Select(x => x.Name.ToLower()).Contains(InstallationProp.key.ToLower()))
                            {
                                Installations = Installations.Where(x => NotStringProps.AsEnumerable().FirstOrDefault(y => (InstallationProp.key.ToLower() == y.Name.ToLower()) && (y.GetValue(_mapper.Map<CivilNonSteelViewModel>(x), null) != null ?
                                    InstallationProp.value.AsEnumerable().Contains(y.GetValue(_mapper.Map<CivilNonSteelViewModel>(x), null).ToString().ToLower()) : false)) != null).AsEnumerable();
                            }
                        }

                        InstallationAttributeActivated = Installations.Select(x => x.Id).ToList();
                    }

                    //
                    // Installation (Attribute Activated + Dynamic) Attributes...
                    //
                    if (AttrInstExist && DynamicInstExist)
                    {
                        WithoutDateFilterCivilNonSteelInstallation = InstallationAttributeActivated.Intersect(DynamicInstValueListIds).ToList();
                    }
                    else if (AttrInstExist)
                    {
                        WithoutDateFilterCivilNonSteelInstallation = InstallationAttributeActivated;
                    }
                    else if (DynamicInstExist)
                    {
                        WithoutDateFilterCivilNonSteelInstallation = DynamicInstValueListIds;
                    }
                }

                if (DateFilter != null ? DateFilter.Count() > 0 : false)
                {
                    List<TLIdynamicAtt> DateTimeInstDynamicAttListIds = _unitOfWork.DynamicAttRepository.GetIncludeWhere(x =>
                        AfterConvertDateFilters.AsEnumerable().Select(y => y.key.ToLower()).Contains(x.Key.ToLower()) &&
                        !x.LibraryAtt && !x.disable &&
                        x.tablesNames.TableName == Helpers.Constants.TablesNames.TLIcivilNonSteel.ToString()
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
                    List<PropertyInfo> InstallationProps = typeof(CivilNonSteelViewModel).GetProperties().Where(x =>
                        AfterConvertDateFilters.Select(y =>
                            y.key.ToLower()).Contains(x.Name.ToLower())).ToList();

                    List<int> InstallationAttributeActivatedIds = new List<int>();
                    bool AttrInstExist = false;

                    if (InstallationProps != null)
                    {
                        AttrInstExist = true;

                        List<DateFilterViewModel> InstallationPropsAttributeFilters = AfterConvertDateFilters.Where(x =>
                            InstallationProps.Select(y => y.Name.ToLower()).Contains(x.key.ToLower())).ToList();

                        IEnumerable<TLIcivilNonSteel> Installations = _unitOfWork.CivilNonSteelRepository.GetAllWithoutCount();

                        foreach (DateFilterViewModel InstallationProp in InstallationPropsAttributeFilters)
                        {
                            Installations = Installations.Where(x => InstallationProps.Exists(y => (InstallationProp.key.ToLower() == y.Name.ToLower()) && ((y.GetValue(_mapper.Map<CivilNonSteelViewModel>(x), null) != null) ?
                                ((InstallationProp.DateFrom.Date <= Convert.ToDateTime(y.GetValue(_mapper.Map<CivilNonSteelViewModel>(x), null)).Date) &&
                                    (InstallationProp.DateTo.Date >= Convert.ToDateTime(y.GetValue(_mapper.Map<CivilNonSteelViewModel>(x), null)).Date)) : (false))));
                        }

                        InstallationAttributeActivatedIds = Installations.Select(x => x.Id).ToList();
                    }

                    //
                    // Installation (Attribute Activated + Dynamic) Attributes...
                    //
                    if (AttrInstExist && DynamicInstExist)
                    {
                        WithDateFilterCivilNonSteelInstallation = InstallationAttributeActivatedIds.Intersect(DynamicInstValueListIds).ToList();
                    }
                    else if (AttrInstExist)
                    {
                        WithDateFilterCivilNonSteelInstallation = InstallationAttributeActivatedIds;
                    }
                    else if (DynamicInstExist)
                    {
                        WithDateFilterCivilNonSteelInstallation = DynamicInstValueListIds;
                    }
                }

                if ((DateFilter != null ? DateFilter.Count() > 0 : false) ||
                    (AttributeFilters != null ? AttributeFilters.Count() > 0 : false))
                {
                    if ((AttributeFilters != null ? AttributeFilters.Count > 0 : false) &&
                        (AfterConvertDateFilters != null ? AfterConvertDateFilters.Count() > 0 : false))
                    {
                        CivilNonSteelIds = WithoutDateFilterCivilNonSteelInstallation.Intersect(WithDateFilterCivilNonSteelInstallation).ToList();
                    }
                    else if (AttributeFilters != null ? AttributeFilters.Count > 0 : false)
                    {
                        CivilNonSteelIds = WithoutDateFilterCivilNonSteelInstallation;
                    }
                    else if (AfterConvertDateFilters != null ? AfterConvertDateFilters.Count() > 0 : false)
                    {
                        CivilNonSteelIds = WithDateFilterCivilNonSteelInstallation;
                    }

                    return _unitOfWork.CivilSiteDateRepository.GetIncludeWhere(x => (
                        (x.SiteCode == BaseFilter.SiteCode) &&
                        (!x.Dismantle) &&
                        (BaseFilter.ItemStatusId != null ? (
                            x.allCivilInst != null ? (
                                x.allCivilInst.ItemStatusId != null ? (
                                    x.allCivilInst.ItemStatusId == BaseFilter.ItemStatusId)
                                : false)
                            : false)
                        : true) &&
                        (BaseFilter.TicketId != null ? (
                            x.allCivilInst != null ? (
                                x.allCivilInst.TicketId != null ? (
                                    x.allCivilInst.TicketId == BaseFilter.TicketId)
                                : false)
                            : false)
                        : true) &&
                        (x.allCivilInst.civilNonSteelId != null) &&
                        (x.allCivilInst.Draft == false) &&

                        CivilNonSteelIds.Contains(x.allCivilInst.civilNonSteelId.Value)
                    ), x => x.allCivilInst, x => x.allCivilInst.civilNonSteel, x => x.allCivilInst.civilNonSteel.CivilNonsteelLibrary,
                       x => x.allCivilInst.civilNonSteel.locationType,
                       x => x.allCivilInst.civilNonSteel.owner, x => x.allCivilInst.civilNonSteel.supportTypeImplemented).ToList();
                }

                return _unitOfWork.CivilSiteDateRepository.GetIncludeWhere(x => (
                    (x.SiteCode == BaseFilter.SiteCode) &&
                    (!x.Dismantle) &&
                    (BaseFilter.ItemStatusId != null ? (
                        x.allCivilInst != null ? (
                            x.allCivilInst.ItemStatusId != null ? (
                                x.allCivilInst.ItemStatusId == BaseFilter.ItemStatusId)
                            : false)
                        : false)
                    : true) &&
                    (BaseFilter.TicketId != null ? (
                        x.allCivilInst != null ? (
                            x.allCivilInst.TicketId != null ? (
                                x.allCivilInst.TicketId == BaseFilter.TicketId)
                            : false)
                        : false)
                    : true) &&
                    (x.allCivilInst.civilNonSteelId != null) &&
                    (x.allCivilInst.Draft == false)

                ), x => x.allCivilInst, x => x.allCivilInst.civilNonSteel, x => x.allCivilInst.civilNonSteel.CivilNonsteelLibrary,
                    x => x.allCivilInst.civilNonSteel.locationType,
                    x => x.allCivilInst.civilNonSteel.owner, x => x.allCivilInst.civilNonSteel.supportTypeImplemented).ToList();
            }

            return null;
        }
        public DynamicAttDto GetDynamicAttDto(TLIdynamicAttInstValue DynamicAttInstValueRecord, TLIdynamicAttLibValue DynamicAttLibRecord)
        {
            DynamicAttDto CivilDynamicAttDto = new DynamicAttDto
            {
                DataType = new DataTypeViewModel(),
                DynamicAttInstValue = new DynamicAttInstValueViewModel(),
                DynamicAttLibValue = new DynamicAttLibValueViewMdodel()
            };
            if (DynamicAttInstValueRecord != null)
            {
                // Key
                CivilDynamicAttDto.Key = DynamicAttInstValueRecord.DynamicAtt.Key;

                // DataType.Id + DataType.Name
                CivilDynamicAttDto.DataType.Id = DynamicAttInstValueRecord.DynamicAtt.DataTypeId.Value;
                CivilDynamicAttDto.DataType.Name = DynamicAttInstValueRecord.DynamicAtt.DataType.Name;

                // DynamicAttInstValue.Id
                CivilDynamicAttDto.DynamicAttInstValue.Id = DynamicAttInstValueRecord.Id;

                // DynamicAttInstValueRecord.ValueString
                CivilDynamicAttDto.DynamicAttInstValue.Value = GetDynamicAttValue(DynamicAttInstValueRecord, null);

                // DynamicAttInstValue.DynamicAttId
                CivilDynamicAttDto.DynamicAttInstValue.DynamicAttId = DynamicAttInstValueRecord.DynamicAttId;

                CivilDynamicAttDto.DynamicAttLibValue = null;
            }
            else if (DynamicAttLibRecord != null)
            {
                // Key
                CivilDynamicAttDto.Key = DynamicAttLibRecord.DynamicAtt.Key;

                // DataType.Id + DataType.Name
                CivilDynamicAttDto.DataType.Id = DynamicAttLibRecord.DynamicAtt.DataTypeId.Value;
                CivilDynamicAttDto.DataType.Name = DynamicAttLibRecord.DynamicAtt.DataType.Name;

                // DynamicAttLibValue.Id
                CivilDynamicAttDto.DynamicAttLibValue.Id = DynamicAttLibRecord.Id;

                // DynamicAttLibValue.Value
                CivilDynamicAttDto.DynamicAttLibValue.Value = GetDynamicAttValue(null, DynamicAttLibRecord);

                // DynamicAttLibValue.DynamicAttId
                CivilDynamicAttDto.DynamicAttLibValue.DynamicAttId = DynamicAttLibRecord.DynamicAttId;
                CivilDynamicAttDto.DynamicAttInstValue = null;
            }
            return CivilDynamicAttDto;
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
        #endregion

        public Response<List<ListOfCivilLoadDto>> GetAllCivilLoad(string SearchName, ParameterPagination parameters)
        {
            try
            {

                int count = 0;
                var query = _dbContext.TLIcivilLoads.Where(x => x.sideArmId != null && x.Dismantle == false).Include(x => x.site).Include(x => x.allLoadInst).Include(y => y.sideArm).Include(z => z.allCivilInst).ToList();
                var query2 = _dbContext.TLIcivilLoads.Where(x => x.sideArmId == null && x.Dismantle == false).Include(x => x.site).Include(x => x.allLoadInst).Include(z => z.allCivilInst).ToList();
                List<ListOfCivilLoadDto> result = new List<ListOfCivilLoadDto>();
                List<int> ExcludeList = new List<int>();
                List<int> ExcludeList2 = new List<int>();

                foreach (var q in query)
                {
                    if (!ExcludeList.Contains(q.allCivilInstId))
                    {

                        ExcludeList.Add(q.allCivilInstId);
                        ListOfCivilLoadDto item = new ListOfCivilLoadDto();
                        item.SiteCode = q.site != null ? q.site.SiteCode : "Not Assign to Site";
                        item.SiteName = q.site != null ? q.site.SiteName : "Not Assign to Site";
                        if (q.allCivilInst.civilWithLegsId != null)
                        {
                            var Civil = _dbContext.TLIcivilWithLegs.FirstOrDefault(x => x.Id == q.allCivilInst.civilWithLegsId);
                            item.CivilName = Civil.Name;
                            item.CivilId = q.allCivilInstId;
                        }
                        else if (q.allCivilInst.civilWithoutLegId != null)
                        {
                            var Civil = _dbContext.TLIcivilWithoutLeg.FirstOrDefault(x => x.Id == q.allCivilInst.civilWithoutLegId);
                            item.CivilName = Civil.Name;
                            item.CivilId = q.allCivilInstId;

                        }
                        else
                        {
                            var Civil = _dbContext.TLIcivilNonSteel.FirstOrDefault(x => x.Id == q.allCivilInst.civilNonSteelId);
                            item.CivilName = Civil.Name;
                            item.CivilId = q.allCivilInstId;

                        }


                        var LoadOnSide = GetLoadForSideArm((int)q.sideArmId, q.allCivilInstId);
                        item.CivilLoadWithSideArm = LoadOnSide;




                        var LoadOnCivil = GetLoadWithoutSideArm(q.allCivilInstId);
                        item.LoadDirOnCivil = LoadOnCivil;

                        result.Add(item);
                    }
                }

                //Civil onlay have load without any side arm
                foreach (var q in query2)
                {
                    if (!ExcludeList2.Contains(q.allCivilInstId) && !ExcludeList.Contains(q.allCivilInstId))
                    {
                        ExcludeList2.Add(q.allCivilInstId);
                        ListOfCivilLoadDto item = new ListOfCivilLoadDto();
                        item.SiteCode = q.site != null ? q.site.SiteCode : "Not Assign to Site";
                        item.SiteName = q.site != null ? q.site.SiteName : "Not Assign to Site";
                        if (q.allCivilInst.civilWithLegsId != null)
                        {
                            var Civil = _dbContext.TLIcivilWithLegs.FirstOrDefault(x => x.Id == q.allCivilInst.civilWithLegsId);
                            item.CivilName = Civil.Name;
                            item.CivilId = q.allCivilInstId;
                        }
                        else if (q.allCivilInst.civilWithoutLegId != null)
                        {
                            var Civil = _dbContext.TLIcivilWithoutLeg.FirstOrDefault(x => x.Id == q.allCivilInst.civilWithoutLegId);
                            item.CivilName = Civil.Name;
                            item.CivilId = q.allCivilInstId;

                        }
                        else
                        {
                            var Civil = _dbContext.TLIcivilNonSteel.FirstOrDefault(x => x.Id == q.allCivilInst.civilNonSteelId);
                            item.CivilName = Civil.Name;
                            item.CivilId = q.allCivilInstId;

                        }

                        var LoadOnCivil = GetLoadWithoutSideArm(q.allCivilInstId);
                        item.LoadDirOnCivil = LoadOnCivil;

                        result.Add(item);
                    }
                }
                count = result.Count();
                //Filter On CivilName or SiteCode
                if (SearchName != null)
                {
                    result = result.Where(x => x.CivilName.ToLower().StartsWith(SearchName.ToLower()) || x.SiteCode.ToLower().StartsWith(SearchName.ToLower())).ToList();
                }

                if (parameters.PageNumber != 0 && parameters.PageSize != 0)
                {
                    result = result.Skip((parameters.PageNumber - 1) * parameters.PageSize).Take(parameters.PageSize).ToList();

                }


                return new Response<List<ListOfCivilLoadDto>>(true, result, null, null, (int)Helpers.Constants.ApiReturnCode.success, count);
            }
            catch (Exception ex)
            {
                return new Response<List<ListOfCivilLoadDto>>(false, null, null, ex.Message, (int)Helpers.Constants.ApiReturnCode.fail);

            }
        }

        public List<LoadOnSideArm> GetLoadForSideArm(int sidearmid, int civilid)
        {
            var sideArm = _dbContext.TLIsideArm.FirstOrDefault(x => x.Id == sidearmid);
            var loadsid = _dbContext.TLIcivilLoads.Where(x => x.sideArmId == sidearmid && x.allCivilInstId == civilid && x.Dismantle == false).Select(y => y.allLoadInstId).ToList();
            List<LoadOnSideArm> FinalResult = new List<LoadOnSideArm>();
            List<LoadOnCivil> listOfload = new List<LoadOnCivil>();
            LoadOnSideArm loadOnSideArm = new LoadOnSideArm();

            loadOnSideArm.SideArmId = sidearmid;
            loadOnSideArm.SideArmName = sideArm.Name;
            foreach (var lid in loadsid)
            {
                var resultLoad = _dbContext.TLIallLoadInst.FirstOrDefault(x => x.Id == lid && x.Active == true);

                LoadOnCivil loadOnCivil = new LoadOnCivil();
                if (resultLoad != null)
                {
                    string loadName = null;

                    var keyName = GetKeyName(resultLoad);
                    if (keyName == "mwBUId")
                    {
                        loadName = _dbContext.TLImwBU.Where(x => x.Id == resultLoad.mwBUId).Select(x => x.Name).FirstOrDefault();
                    }
                    else if (keyName == "mwDishId")
                    {
                        loadName = _dbContext.TLImwDish.Where(x => x.Id == resultLoad.mwDishId).Select(x => x.DishName).FirstOrDefault();
                    }
                    else if (keyName == "mwODUId")
                    {
                        loadName = _dbContext.TLImwODU.Where(x => x.Id == resultLoad.mwODUId).Select(x => x.Name).FirstOrDefault();
                    }
                    else if (keyName == "mwRFUId")
                    {
                        loadName = _dbContext.TLImwRFU.Where(x => x.Id == resultLoad.mwRFUId).Select(x => x.Name).FirstOrDefault();
                    }
                    else if (keyName == "mwOtherId")
                    {
                        loadName = _dbContext.TLImwOther.Where(x => x.Id == resultLoad.mwOtherId).Select(x => x.Name).FirstOrDefault();
                    }
                    else if (keyName == "radioAntennaId")
                    {
                        loadName = _dbContext.TLIradioAntenna.Where(x => x.Id == resultLoad.radioAntennaId).Select(x => x.Name).FirstOrDefault();
                    }
                    else if (keyName == "radioRRUId")
                    {
                        loadName = _dbContext.TLIRadioRRU.Where(x => x.Id == resultLoad.radioRRUId).Select(x => x.Name).FirstOrDefault();
                    }
                    else if (keyName == "radioOtherId")
                    {
                        loadName = _dbContext.TLIradioOther.Where(x => x.Id == resultLoad.radioOtherId).Select(x => x.Name).FirstOrDefault();
                    }
                    else if (keyName == "powerId")
                    {
                        loadName = _dbContext.TLIpower.Where(x => x.Id == resultLoad.powerId).Select(x => x.Name).FirstOrDefault();
                    }
                    else if (keyName == "loadOtherId")
                    {
                        loadName = _dbContext.TLIloadOther.Where(x => x.Id == resultLoad.loadOtherId).Select(x => x.Name).FirstOrDefault();
                    }

                    loadOnCivil.LoadId = (int)lid;
                    loadOnCivil.LoadName = loadName;
                    listOfload.Add(loadOnCivil);
                    loadOnSideArm.LoadRelatedSide = listOfload;

                }

            }
            FinalResult.Add(loadOnSideArm);

            return FinalResult;


        }
        //Load without side arm
        public List<LoadOnCivil> GetLoadWithoutSideArm(int civilid)
        {
            var loadsid = _dbContext.TLIcivilLoads.Where(x => x.sideArmId == null && x.allCivilInstId == civilid && x.Dismantle == false).Select(y => y.allLoadInstId).ToList();
            List<LoadOnCivil> listOfload = new List<LoadOnCivil>();

            foreach (var lid in loadsid)
            {
                string keyName = null;
                LoadOnCivil loadOnCivil = new LoadOnCivil();
                /*  var result = db.TLIallLoadInst.Where(x => x.Id == lid && x.Active == true).AsEnumerable()
                      .FirstOrDefault(a => a.GetType().GetProperties().All(pi => pi.GetValue(a) != null));*/
                var result = _dbContext.TLIallLoadInst.FirstOrDefault(x => x.Id == lid && x.Active == true);

                if (result != null)
                {
                    string loadName = null;
                    keyName = GetKeyName(result);
                    if (keyName == "mwBUId")
                    {
                        loadName = _dbContext.TLImwBU.Where(x => x.Id == result.mwBUId).Select(x => x.Name).FirstOrDefault();
                    }
                    else if (keyName == "mwDishId")
                    {
                        loadName = _dbContext.TLImwDish.Where(x => x.Id == result.mwDishId).Select(x => x.DishName).FirstOrDefault();
                    }
                    else if (keyName == "mwODUId")
                    {
                        loadName = _dbContext.TLImwODU.Where(x => x.Id == result.mwODUId).Select(x => x.Name).FirstOrDefault();
                    }
                    else if (keyName == "mwRFUId")
                    {
                        loadName = _dbContext.TLImwRFU.Where(x => x.Id == result.mwRFUId).Select(x => x.Name).FirstOrDefault();
                    }
                    else if (keyName == "mwOtherId")
                    {
                        loadName = _dbContext.TLImwOther.Where(x => x.Id == result.mwOtherId).Select(x => x.Name).FirstOrDefault();
                    }
                    else if (keyName == "radioAntennaId")
                    {
                        loadName = _dbContext.TLIradioAntenna.Where(x => x.Id == result.radioAntennaId).Select(x => x.Name).FirstOrDefault();
                    }
                    else if (keyName == "radioRRUId")
                    {
                        loadName = _dbContext.TLIRadioRRU.Where(x => x.Id == result.radioRRUId).Select(x => x.Name).FirstOrDefault();
                    }
                    else if (keyName == "radioOtherId")
                    {
                        loadName = _dbContext.TLIradioOther.Where(x => x.Id == result.radioOtherId).Select(x => x.Name).FirstOrDefault();
                    }
                    else if (keyName == "powerId")
                    {
                        loadName = _dbContext.TLIpower.Where(x => x.Id == result.powerId).Select(x => x.Name).FirstOrDefault();
                    }
                    else if (keyName == "loadOtherId")
                    {
                        loadName = _dbContext.TLIloadOther.Where(x => x.Id == result.loadOtherId).Select(x => x.Name).FirstOrDefault();
                    }

                    loadOnCivil.LoadId = (int)lid;
                    loadOnCivil.LoadName = loadName;
                    listOfload.Add(loadOnCivil);
                }


            }

            return listOfload;


        }

        public string GetKeyName(TLIallLoadInst m)
        {
            string keyName = null;
            if (m.mwBUId != null)
            {
                keyName = "mwBUId";
            }
            else if (m.mwDishId != null)
            {
                keyName = "mwDishId";
            }
            else if (m.mwODUId != null)
            {
                keyName = "mwODUId";
            }
            else if (m.mwRFUId != null)
            {
                keyName = "mwRFUId";
            }
            else if (m.mwOtherId != null)
            {
                keyName = "mwOtherId";
            }
            else if (m.radioAntennaId != null)
            {
                keyName = "radioAntennaId";
            }
            else if (m.radioRRUId != null)
            {
                keyName = "radioRRUId";
            }
            else if (m.radioOtherId != null)
            {
                keyName = "radioOtherId";
            }
            else if (m.powerId != null)
            {
                keyName = "powerId";
            }
            else if (m.loadOtherId != null)
            {
                keyName = "loadOtherId";
            }
            return keyName;

        }

        //public Response<bool> CivilDismantle(DismantleBinding dis)
        //{
        //    try
        //    {

        //        if (dis.DismantleAll == true)
        //        {
        //            var AllLoad = _dbContext.TLIcivilLoads.Where(x => x.allCivilInstId == dis.CivilId && x.Dismantle == false && x.SiteCode == dis.SiteCode).Include(x => x.allCivilInst).ToList();
        //            foreach (var s in AllLoad)
        //            {
        //                s.Dismantle = true;
        //                _dbContext.Entry(s).State = EntityState.Modified;
        //                _dbContext.SaveChanges();
        //            }

        //            if (dis.RecalculatedReservedSpace == true)
        //            {
        //                var UpdateFreeSiteSpace = ReCalcualateSiteFreeSpace(dis.CivilId, dis.SiteCode, 0);
        //            }

        //            //Dsmantle related load on civil
        //            var RelatedLoadStatus = CheckRelatedLoad(dis.SiteCode, dis.CivilId, 0, 0);
        //        }

        //        else
        //        {
        //            if (dis.SidearmIds.Count != 0)
        //            {
        //                foreach (var s in dis.SidearmIds)
        //                {
        //                    var sideArms = _dbContext.TLIcivilLoads.Where(x => x.sideArmId == s && x.SiteCode == dis.SiteCode && x.Dismantle == false).ToList();
        //                    foreach (var v in sideArms)
        //                    {
        //                        v.Dismantle = true;
        //                        _dbContext.Entry(v).State = EntityState.Modified;
        //                        _dbContext.SaveChanges();
        //                        if (v.allLoadInstId != null)
        //                        {
        //                            var RelatedLoadStatus = CheckRelatedLoad(dis.SiteCode, dis.CivilId, (int)v.sideArmId, (int)v.allLoadInstId);

        //                        }


        //                    }


        //                }
        //                //Dsmantle related load on civil
        //            }

        //            if (dis.Loadids.Count != 0)
        //            {
        //                foreach (var t in dis.Loadids)
        //                {
        //                    var loads = _dbContext.TLIcivilLoads.FirstOrDefault(x => x.allLoadInstId == t && x.SiteCode == dis.SiteCode && x.Dismantle == false && x.allCivilInstId == dis.CivilId);
        //                    loads.allLoadInstId = null;
        //                    _dbContext.Entry(loads).State = EntityState.Modified;
        //                    _dbContext.SaveChanges();
        //                    var RelatedLoadStatus = CheckRelatedLoad(dis.SiteCode, dis.CivilId, 0, t);

        //                }
        //            }


        //        }
        //        return new Response<bool>(true, true, null, null, (int)Helpers.Constants.ApiReturnCode.success);

        //    }

        //    catch (Exception er)
        //    {
        //        return new Response<bool>(false, false, null, er.Message, (int)Helpers.Constants.ApiReturnCode.fail);

        //    }



        //}

        public bool ReCalcualateSiteFreeSpace(int civilid, string sitecode, int loadid)
        {
            float FreeSpace = 0;

            if (loadid == 0)
            {

                var CivilLoads = _dbContext.TLIcivilLoads.Include(x => x.allCivilInst).FirstOrDefault(x => x.allCivilInstId == civilid && x.Dismantle == true && x.SiteCode == sitecode);
                // ReCalculate Site Reserved Space 

                if (CivilLoads.allCivilInst.civilWithLegsId != null)
                {
                    var civilwithleg = _dbContext.TLIcivilWithLegs.FirstOrDefault(x => x.Id == CivilLoads.allCivilInst.civilWithLegsId);
                    FreeSpace += civilwithleg.SpaceInstallation;

                }
                else if (CivilLoads.allCivilInst.civilWithoutLegId != null)
                {
                    var civilwithoutleg = _dbContext.TLIcivilWithoutLeg.FirstOrDefault(x => x.Id == CivilLoads.allCivilInst.civilWithoutLegId);
                    FreeSpace += civilwithoutleg.SpaceInstallation;

                }
                else
                {
                    var civilNonSteel = _dbContext.TLIcivilNonSteel.FirstOrDefault(x => x.Id == CivilLoads.allCivilInst.civilNonSteelId);
                    FreeSpace += civilNonSteel.SpaceInstallation;

                }

            }
            else if (civilid == 0 && loadid != 0)
            {
                var otherLoad = _dbContext.TLIotherInSite.Where(x => x.SiteCode == sitecode && x.allOtherInventoryInstId == loadid && x.Dismantle == true)
                    .Include(x => x.allOtherInventoryInst).FirstOrDefault();

                if (otherLoad.allOtherInventoryInst.cabinetId != null)
                {
                    var cab = _dbContext.TLIcabinet.FirstOrDefault(x => x.Id == otherLoad.allOtherInventoryInst.cabinetId);
                    FreeSpace += cab.SpaceInstallation;

                }

                else if (otherLoad.allOtherInventoryInst.solarId != null)
                {
                    var solar = _dbContext.TLIsolar.FirstOrDefault(x => x.Id == otherLoad.allOtherInventoryInst.solarId);
                    FreeSpace += solar.SpaceInstallation;
                }

                else
                {
                    var generator = _dbContext.TLIgenerator.FirstOrDefault(x => x.Id == otherLoad.allOtherInventoryInst.generatorId);
                    FreeSpace += generator.SpaceInstallation;
                }

            }
            var Site = _dbContext.TLIsite.FirstOrDefault(x => x.SiteCode == sitecode);

            Site.ReservedSpace -= FreeSpace;

            _dbContext.Entry(Site).State = EntityState.Modified;
            _dbContext.SaveChanges();

            return true;



        }

        public bool CheckRelatedLoad(string sitecode, int civilid, int sidearm, int loadid)
        {
            List<TLIcivilLoads> LstLoadOnCivil = null;
            if (sidearm != 0 && loadid != 0)
            {
                LstLoadOnCivil = _dbContext.TLIcivilLoads.Where(x => x.allCivilInstId == civilid && x.Dismantle == true && x.SiteCode == sitecode && x.sideArmId == sidearm && x.allLoadInstId == loadid)
                               .Include(y => y.allLoadInst).ToList();
            }

            else if (sidearm == 0 && loadid != 0)
            {
                LstLoadOnCivil = _dbContext.TLIcivilLoads.Where(x => x.allCivilInstId == civilid && x.SiteCode == sitecode && x.allLoadInstId == loadid)
                                .Include(y => y.allLoadInst).ToList();
            }

            else
            {
                LstLoadOnCivil = _dbContext.TLIcivilLoads.Where(x => x.allCivilInstId == civilid && x.Dismantle == true && x.SiteCode == sitecode)
              .Include(y => y.allLoadInst).ToList();
            }

            string LoadType = null;
            foreach (var l in LstLoadOnCivil)
            {
                if (l.allLoadInst != null)
                {
                    LoadType = GetKeyName(l.allLoadInst);

                    //Dismantle dish related with BU AND ODU
                    if (LoadType == "mwDishId")
                    {
                        var Bu = _dbContext.TLImwBU.Where(x => x.MainDishId == l.allLoadInst.mwDishId || x.SdDishId == l.allLoadInst.mwDishId).ToList();
                        foreach (var b in Bu)
                        {
                            b.MainDishId = null;
                            b.SdDishId = null;
                            _dbContext.SaveChanges();

                        }
                        var Odu = _dbContext.TLImwODU.Where(x => x.Mw_DishId == l.allLoadInst.mwDishId).ToList();
                        foreach (var o in Odu)
                        {
                            o.Mw_DishId = null;
                            _dbContext.SaveChanges();
                        }

                    }

                    else if (LoadType == "mwBUId")
                    {
                        var Bu = _dbContext.TLImwBU.FirstOrDefault(x => x.Id == l.allLoadInst.mwBUId);
                        Bu.MainDishId = null;
                        Bu.SdDishId = null;
                        _dbContext.SaveChanges();
                    }
                    else if (LoadType == "radioAntennaId")
                    {
                        var RadioRRU = _dbContext.TLIRadioRRU.Where(x => x.radioAntennaId == l.allLoadInst.radioAntennaId).ToList();
                        foreach (var r in RadioRRU)
                        {
                            r.radioAntennaId = null;
                            _dbContext.SaveChanges();
                        }


                    }

                    else if (LoadType == "radioRRUId")
                    {
                        var RadioRRU = _dbContext.TLIRadioRRU.FirstOrDefault(x => x.Id == l.allLoadInst.radioRRUId);
                        RadioRRU.radioAntennaId = null;
                        _dbContext.SaveChanges();
                    }
                }
            }
            return true;
        }

        public string GetOtherLoadKey(int loadid)
        {
            var OtherLoad = _dbContext.TLIallOtherInventoryInst.FirstOrDefault(x => x.Id == loadid);
            string KeyName = null;
            if (OtherLoad.cabinetId != null)
            {
                KeyName = "cabinetId";
            }
            else if (OtherLoad.solarId != null)
            {
                KeyName = "solarId";

            }

            else
            {
                KeyName = "generatorId";

            }

            return KeyName;
        }

        ////public Response<bool> OtherLoadDismantale(string sitecode, int loadid)
        ////{
        ////    try
        ////    {

        ////        var Load = _dbContext.TLIotherInSite.Where(x => x.SiteCode == sitecode && x.allOtherInventoryInstId == loadid && x.Dismantle == false)
        ////            .Include(x => x.allOtherInventoryInst).FirstOrDefault();

        ////        if (Load != null)
        ////        {
        ////            var LoadName = GetOtherLoadKey(Load.allOtherInventoryInstId);

        ////            if (LoadName != "cabinetId")
        ////            {
        ////                Load.Dismantle = true;
        ////                _dbContext.Entry(Load).State = EntityState.Modified;
        ////                _dbContext.SaveChanges();
        ////            }

        ////            else
        ////            {
        ////                var LoadInventory = _dbContext.TLIallOtherInventoryInst.FirstOrDefault(x => x.Id == Load.allOtherInventoryInstId);
        ////                if (LoadInventory != null)
        ////                {
        ////                    var cab = _dbContext.TLIcabinet.FirstOrDefault(x => x.Id == LoadInventory.cabinetId);
        ////                    if (cab.CabinetPowerLibraryId == null)
        ////                    {
        ////                        Load.Dismantle = true;
        ////                        _dbContext.Entry(Load).State = EntityState.Modified;
        ////                        _dbContext.SaveChanges();
        ////                    }
        ////                    else
        ////                    {
        ////                        Load.Dismantle = true;
        ////                        _dbContext.Entry(Load).State = EntityState.Modified;
        ////                        _dbContext.SaveChanges();
        ////                        //remove relation from solar or wind with cab
        ////                        var solar = _dbContext.TLIsolar.Where(x => x.CabinetId == cab.Id).ToList();
        ////                        foreach (var s in solar)
        ////                        {
        ////                            s.CabinetId = null;
        ////                            _dbContext.Entry(s).State = EntityState.Modified;
        ////                            _dbContext.SaveChanges();
        ////                        }
        ////                    }
        ////                }

        ////            }
        ////        }
        ////        var RecalcSiteSpac = ReCalcualateSiteFreeSpace(0, sitecode, loadid);
        ////        return new Response<bool>(true, true, null, null, (int)Helpers.Constants.ApiReturnCode.success);

        ////    }
        //    catch (Exception ex)
        //    {
        //        return new Response<bool>(false, false, null, ex.Message, (int)Helpers.Constants.ApiReturnCode.fail);

        //    }
        //}
        public Response<CivilLoads> GetRelationshipBetweenloads(int loadid, string Loadname)
        {
            try
            {
                CivilLoads OutPut = new CivilLoads();

                if (Loadname == Helpers.Constants.TablesNames.TLImwBU.ToString())
                {
                    var mwport = _dbContext.TLImwPort.Where(x => x.MwBUId == loadid).Select(x => x.Id).FirstOrDefault();

                    OutPut.MW_RFUs = _mapper.Map<List<MW_RFUViewModel>>(_dbContext.TLImwRFU.Where
                                       (x => x.MwPortId == mwport).Distinct().ToList());
                }
                if (Loadname == Helpers.Constants.TablesNames.TLImwDish.ToString())
                {
                    OutPut.MW_ODUs = _mapper.Map<List<MW_ODUViewModel>>(_dbContext.TLImwODU.Where
                                           (x => x.Mw_DishId == loadid).Distinct().ToList());
                    OutPut.MW_BUs = _mapper.Map<List<MW_BUViewModel>>(_dbContext.TLImwBU.Where
                        (x => x.MainDishId == loadid).Distinct().ToList());
                }
                if (Loadname == Helpers.Constants.TablesNames.TLIradioAntenna.ToString())
                {
                    OutPut.RadioRRUs = _mapper.Map<List<RadioRRUViewModel>>(_dbContext.TLIRadioRRU.Where
                                           (x => x.radioAntennaId == loadid).Distinct().ToList());
                }
                return new Response<CivilLoads>(true, OutPut, null, null, (int)Helpers.Constants.ApiReturnCode.success);
            }
            catch (Exception err)
            {
                return new Response<CivilLoads>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
        }
        public Response<List<LibraryDataDto>> GetLibraryAttForInstallations(string InstTableName, int? CatId)
        {
            try
            {
                List<LibraryDataDto> result = new List<LibraryDataDto>();
                if (InstTableName == "TLIsideArm")
                {
                    result = _mapper.Map<List<LibraryDataDto>>(_dbContext.TLIsideArmLibrary.Where(x => x.Deleted == false && x.Active == true)).ToList();
                }
                else if (InstTableName == "TLIgenerator")
                {
                    result = _mapper.Map<List<LibraryDataDto>>(_dbContext.TLIgeneratorLibrary.Where(x => x.Deleted == false && x.Active == true)).ToList();
                }
                else if (InstTableName == "TLImwBU")
                {
                    result = _mapper.Map<List<LibraryDataDto>>(_dbContext.TLImwBULibrary.Where(x => x.Deleted == false && x.Active == true)).ToList();

                }
                else if (InstTableName == "TLImwDish")
                {
                    result = _mapper.Map<List<LibraryDataDto>>(_dbContext.TLImwDishLibrary.Where(x => x.Deleted == false && x.Active == true)).ToList();
                }
                else if (InstTableName == "TLImwOther")
                {
                    result = _mapper.Map<List<LibraryDataDto>>(_dbContext.TLImwOtherLibrary.Where(x => x.Deleted == false && x.Active == true)).ToList();

                }
                else if (InstTableName == "TLImwODU")
                {
                    result = _mapper.Map<List<LibraryDataDto>>(_dbContext.TLImwODULibrary.Where(x => x.Deleted == false && x.Active == true)).ToList();

                }
                else if (InstTableName == "TLIcivilWithoutLeg")
                {
                    if (CatId != null && CatId != 0)
                    {
                        result = _mapper.Map<List<LibraryDataDto>>(_dbContext.TLIcivilWithoutLegLibrary.Where(x => x.Deleted == false && x.Active == true && x.CivilWithoutLegCategoryId == CatId)).ToList();

                    }
                    else
                    {
                        result = _mapper.Map<List<LibraryDataDto>>(_dbContext.TLIcivilWithoutLegLibrary.Where(x => x.Deleted == false && x.Active == true)).ToList();

                    }

                }
                else if (InstTableName == "TLIcabinetPower")
                {
                    result = _mapper.Map<List<LibraryDataDto>>(_dbContext.TLIcabinetPowerLibrary.Where(x => x.Deleted == false && x.Active == true)).ToList();

                }
                else if (InstTableName == "TLIcivilWithLegs")
                {
                    result = _mapper.Map<List<LibraryDataDto>>(_dbContext.TLIcivilWithLegLibrary.Where(x => x.Deleted == false && x.Active == true)).ToList();

                }
                else if (InstTableName == "TLIradioAntenna")
                {
                    result = _mapper.Map<List<LibraryDataDto>>(_dbContext.TLIradioAntennaLibrary.Where(x => x.Deleted == false && x.Active == true)).ToList();

                }
                else if (InstTableName == "TLIloadOther")
                {
                    result = _mapper.Map<List<LibraryDataDto>>(_dbContext.TLIloadOtherLibrary.Where(x => x.Deleted == false && x.Active == true)).ToList();

                }
                else if (InstTableName == "TLIcivilNonSteel")
                {
                    result = _mapper.Map<List<LibraryDataDto>>(_dbContext.TLIcivilNonSteelLibrary.Where(x => x.Deleted == false && x.Active == true)).ToList();

                }
                else if (InstTableName == "TLIpower")
                {
                    result = _mapper.Map<List<LibraryDataDto>>(_dbContext.TLIpowerLibrary.Where(x => x.Deleted == false && x.Active == true)).ToList();

                }
                else if (InstTableName == "TLImwRFU")
                {
                    result = _mapper.Map<List<LibraryDataDto>>(_dbContext.TLImwRFULibrary.Where(x => x.Deleted == false && x.Active == true)).ToList();

                }
                else if (InstTableName == "TLIcabinetTelecom")
                {
                    result = _mapper.Map<List<LibraryDataDto>>(_dbContext.TLIcabinetTelecomLibrary.Where(x => x.Deleted == false && x.Active == true)).ToList();

                }
                else if (InstTableName == "TLIradioOther")
                {
                    result = _mapper.Map<List<LibraryDataDto>>(_dbContext.TLIradioOtherLibrary.Where(x => x.Deleted == false && x.Active == true)).ToList();

                }
                else if (InstTableName == "TLIradioRRU")
                {
                    result = _mapper.Map<List<LibraryDataDto>>(_dbContext.TLIradioRRULibrary.Where(x => x.Deleted == false && x.Active == true)).ToList();

                }
                else if (InstTableName == "TLIsolar")
                {
                    result = _mapper.Map<List<LibraryDataDto>>(_dbContext.TLIsolarLibrary.Where(x => x.Deleted == false && x.Active == true)).ToList();

                }

                return new Response<List<LibraryDataDto>>(true, result, null, null, (int)Helpers.Constants.ApiReturnCode.success, result.Count);
            }
            catch (Exception ex)
            {
                return new Response<List<LibraryDataDto>>(false, null, null, ex.Message, (int)Helpers.Constants.ApiReturnCode.fail);

            }
        }
        public Response<bool> CheckLoadsBeforDismantle(string TableName, int loadId)
        {
            try
            {
                if (TableName == Helpers.Constants.TablesNames.TLImwBU.ToString())
                {
                    var RFU = _dbContext.TLImwPort.Where(x => x.MwBUId == loadId).Select(x => x.Id).ToList();
                    foreach (var item in RFU)
                    {
                        var BU = _dbContext.TLImwRFU.Where(x => x.MwPortId == item).ToList();
                        if (BU.Count > 0)
                        {
                            return new Response<bool>(true, false, null, "will dismantle the cascaded BU and the RFUs installed on it", (int)Helpers.Constants.ApiReturnCode.fail);
                        }
                    }
                }

                else if (TableName == Helpers.Constants.TablesNames.TLImwDish.ToString())
                {
                    var BU = _dbContext.TLImwBU.Where(x => x.MainDishId == loadId).ToList();
                    var ODU = _dbContext.TLImwODU.Where(x => x.Mw_DishId == loadId).ToList();
                    if (ODU.Count > 0)
                    {
                        return new Response<bool>(true, false, null, "it’s better to dismantle the ODU as it will be useless", (int)Helpers.Constants.ApiReturnCode.fail);
                    }
                    if (BU.Count > 0)
                    {
                        return new Response<bool>(true, false, null, "the MWDish is Main Dish to MwBU must delete relation And Dismantle Or Change Relation  ", (int)Helpers.Constants.ApiReturnCode.fail);
                    }
                }

                else if (TableName == Helpers.Constants.TablesNames.TLIradioAntenna.ToString())
                {
                    var RadioRRU = _dbContext.TLIRadioRRU.Where(x => x.radioAntennaId == loadId).ToList();
                    if (RadioRRU.Count > 0)
                    {
                        return new Response<bool>(true, false, null, "the RadioAntenna is Related  to RadioRRU must delete relation And Dismantle Or Change Relation  ", (int)Helpers.Constants.ApiReturnCode.fail);
                    }
                }

                return new Response<bool>(true, true, null, null, (int)Helpers.Constants.ApiReturnCode.success);
            }
            catch (Exception er)
            {

                return new Response<bool>(false, false, null, er.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
        }
        public Response<bool> DismantleCivil(string SiteCode, int CivilId, string CivilName)
        {
            try
            {
                var allcivil = _dbContext.TLIallCivilInst.Where(x => x.civilWithLegsId == CivilId || x.civilWithoutLegId == CivilId || x.civilNonSteelId == CivilId).Include(x => x.civilWithLegs).Include(x => x.civilWithoutLeg).Include(x => x.civilNonSteel).ToList();
                double? Freespace = 0;
                foreach (var item in allcivil)
                {
                    if (item.civilWithLegsId != null && CivilName == Helpers.Constants.TablesNames.TLIcivilWithLegs.ToString())
                    {

                        TLIcivilWithLegs TLIcivilWithLegs = item.civilWithLegs;
                        var civilSiteDate = _dbContext.TLIcivilSiteDate.Where(x => x.allCivilInstId == item.Id && x.SiteCode == SiteCode && x.ReservedSpace == true && x.Dismantle == false).ToList();
                        foreach (var civil in civilSiteDate)
                        {
                            civil.Dismantle = true;
                            Freespace += TLIcivilWithLegs.SpaceInstallation;

                        }
                        var civilSiteDate1 = _dbContext.TLIcivilSiteDate.Where(x => x.allCivilInstId == item.Id && x.SiteCode == SiteCode && x.ReservedSpace == false && x.Dismantle == false).ToList();
                        foreach (var civil in civilSiteDate1)
                        {
                            civil.Dismantle = true;

                        }
                        var allcivilload = _dbContext.TLIcivilLoads.Where(x => x.allCivilInstId == item.Id && x.SiteCode == SiteCode && x.Dismantle == false).ToList();
                        foreach (var tLIcivilLoads in allcivilload)
                        {
                            tLIcivilLoads.Dismantle = true;

                        }

                    }
                    else if (item.civilWithoutLegId != null && CivilName == Helpers.Constants.TablesNames.TLIcivilWithoutLeg.ToString())
                    {

                        TLIcivilWithoutLeg TLIcivilWithoutLeg = item.civilWithoutLeg;
                        var civilSiteDate = _dbContext.TLIcivilSiteDate.Where(x => x.allCivilInstId == item.Id && x.SiteCode == SiteCode && x.ReservedSpace == true && x.Dismantle == false).ToList();
                        foreach (var civil in civilSiteDate)
                        {
                            civil.Dismantle = true;
                            Freespace += TLIcivilWithoutLeg.SpaceInstallation;
                        }
                        var civilSiteDate1 = _dbContext.TLIcivilSiteDate.Where(x => x.allCivilInstId == item.Id && x.SiteCode == SiteCode && x.ReservedSpace == false && x.Dismantle == false).ToList();
                        foreach (var civil in civilSiteDate1)
                        {
                            civil.Dismantle = true;
                        }
                        var allcivilload = _dbContext.TLIcivilLoads.Where(x => x.allCivilInstId == item.Id && x.SiteCode == SiteCode && x.Dismantle == false).ToList();
                        foreach (var tLIcivilLoads in allcivilload)
                        {
                            tLIcivilLoads.Dismantle = true;

                        }

                    }
                    else if (item.civilNonSteelId != null && CivilName == Helpers.Constants.TablesNames.TLIcivilNonSteel.ToString())
                    {

                        TLIcivilNonSteel TLIcivilNonSteel = item.civilNonSteel;
                        var civilSiteDate = _dbContext.TLIcivilSiteDate.Where(x => x.allCivilInstId == item.Id && x.SiteCode == SiteCode && x.Dismantle == false).ToList();
                        foreach (var civil in civilSiteDate)
                        {
                            civil.Dismantle = true;
                            Freespace += TLIcivilNonSteel.SpaceInstallation;
                        }
                        var civilSiteDate1 = _dbContext.TLIcivilSiteDate.Where(x => x.allCivilInstId == item.Id && x.SiteCode == SiteCode && x.Dismantle == false).ToList();
                        foreach (var civil in civilSiteDate1)
                        {
                            civil.Dismantle = true;
                        }
                        var allcivilload = _dbContext.TLIcivilLoads.Where(x => x.allCivilInstId == item.Id && x.SiteCode == SiteCode && x.Dismantle == false).ToList();
                        foreach (var tLIcivilLoads in allcivilload)
                        {
                            tLIcivilLoads.Dismantle = true;
                        }

                    }

                }
                var Site = _dbContext.TLIsite.FirstOrDefault(x => x.SiteCode == SiteCode);
                Site.ReservedSpace -= (float)Freespace;
                _dbContext.Entry(Site).State = EntityState.Modified;
                _dbContext.SaveChanges();
                return new Response<bool>(true, true, null, null, (int)Helpers.Constants.ApiReturnCode.success);
            }
            catch (Exception er)
            {

                return new Response<bool>(false, false, null, er.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
        }
        public Response<List<LogicalOperationViewModel>> GetlogicalOperation()
        {
            try
            {
                var logical = _unitOfWork.LogicalOperationRepository.GetAllAsQueryable().ToList();
                var Logical = _mapper.Map<List<LogicalOperationViewModel>>(logical);
                return new Response<List<LogicalOperationViewModel>>(true, Logical, null, null, (int)Helpers.Constants.ApiReturnCode.success);

            }
            catch (Exception err)
            {

                return new Response<List<LogicalOperationViewModel>>(false, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
        }
        public Response<SideArmAndLoadsOnCivil> CheckFilterSideArm_LoadsOnCivils(int CivilId, string CivilType)
        {
            SideArmAndLoadsOnCivil OutPutData = new SideArmAndLoadsOnCivil();

            string OutPutMessage = "";

            TLIallCivilInst AllCivilInst = new TLIallCivilInst();

            if (CivilType.ToLower() == TablesNames.TLIcivilWithLegs.ToString().ToLower())
            {
                AllCivilInst = _unitOfWork.AllCivilInstRepository.GetWhereFirst(x => x.civilWithLegsId == CivilId);
            }
            else if (CivilType.ToLower() == TablesNames.TLIcivilWithoutLeg.ToString().ToLower())
            {
                AllCivilInst = _unitOfWork.AllCivilInstRepository.GetWhereFirst(x => x.civilWithoutLegId == CivilId);
            }
            else if (CivilType.ToLower() == TablesNames.TLIcivilNonSteel.ToString().ToLower())
            {
                AllCivilInst = _unitOfWork.AllCivilInstRepository.GetWhereFirst(x => x.civilNonSteelId == CivilId);
            }

            // Side Arms..
            OutPutData.SideArmsCount = _unitOfWork.CivilLoadsRepository.GetIncludeWhere(x => x.allCivilInstId == AllCivilInst.Id && !x.Dismantle &&
                x.sideArmId != null ? !x.sideArm.Draft : false, x => x.sideArm).Select(x => x.sideArmId.Value).Distinct().ToList().Count();

            // MWs..
            OutPutData.MW_DishesCount = _unitOfWork.CivilLoadsRepository.GetIncludeWhere(x => x.allCivilInstId == AllCivilInst.Id && !x.Dismantle &&
                x.allLoadInstId != null ? (x.allLoadInst.mwDishId != null && !x.allLoadInst.Draft) : false,
                    x => x.allLoadInst).Select(x => x.allLoadInst.mwDishId.Value).Distinct().ToList().Count();

            OutPutData.MW_BUsCount = _unitOfWork.CivilLoadsRepository.GetIncludeWhere(x => x.allCivilInstId == AllCivilInst.Id && !x.Dismantle &&
                x.allLoadInstId != null ? (x.allLoadInst.mwBUId != null && !x.allLoadInst.Draft) : false,
                    x => x.allLoadInst).Select(x => x.allLoadInst.mwBUId.Value).Distinct().ToList().Count();

            OutPutData.MW_ODUsCount = _unitOfWork.CivilLoadsRepository.GetIncludeWhere(x => x.allCivilInstId == AllCivilInst.Id && !x.Dismantle &&
                x.allLoadInstId != null ? (x.allLoadInst.mwODUId != null && !x.allLoadInst.Draft) : false,
                    x => x.allLoadInst).Select(x => x.allLoadInst.mwODUId.Value).Distinct().ToList().Count();

            OutPutData.MW_RFUsCount = _unitOfWork.CivilLoadsRepository.GetIncludeWhere(x => x.allCivilInstId == AllCivilInst.Id && !x.Dismantle &&
                x.allLoadInstId != null ? (x.allLoadInst.mwRFUId != null && !x.allLoadInst.Draft) : false,
                    x => x.allLoadInst).Select(x => x.allLoadInst.mwRFUId.Value).Distinct().ToList().Count();

            OutPutData.OtherMWsCount = _unitOfWork.CivilLoadsRepository.GetIncludeWhere(x => x.allCivilInstId == AllCivilInst.Id && !x.Dismantle &&
                x.allLoadInstId != null ? (x.allLoadInst.mwOtherId != null && !x.allLoadInst.Draft) : false,
                    x => x.allLoadInst).Select(x => x.allLoadInst.mwOtherId.Value).Distinct().ToList().Count();

            // Radios..
            OutPutData.RadioAntennasCount = _unitOfWork.CivilLoadsRepository.GetIncludeWhere(x => x.allCivilInstId == AllCivilInst.Id && !x.Dismantle &&
                x.allLoadInstId != null ? (x.allLoadInst.radioAntennaId != null && !x.allLoadInst.Draft) : false,
                    x => x.allLoadInst).Select(x => x.allLoadInst.radioAntennaId.Value).Distinct().ToList().Count();

            OutPutData.RadioRRUsCount = _unitOfWork.CivilLoadsRepository.GetIncludeWhere(x => x.allCivilInstId == AllCivilInst.Id && !x.Dismantle &&
                x.allLoadInstId != null ? (x.allLoadInst.radioRRUId != null && !x.allLoadInst.Draft) : false,
                    x => x.allLoadInst).Select(x => x.allLoadInst.radioRRUId.Value).Distinct().ToList().Count();

            OutPutData.OtherRadiosCount = _unitOfWork.CivilLoadsRepository.GetIncludeWhere(x => x.allCivilInstId == AllCivilInst.Id && !x.Dismantle &&
                x.allLoadInstId != null ? (x.allLoadInst.radioOtherId != null && !x.allLoadInst.Draft) : false,
                    x => x.allLoadInst).Select(x => x.allLoadInst.radioOtherId.Value).Distinct().ToList().Count();

            // Power..
            OutPutData.PowersCount = _unitOfWork.CivilLoadsRepository.GetIncludeWhere(x => x.allCivilInstId == AllCivilInst.Id && !x.Dismantle &&
                x.allLoadInstId != null ? (x.allLoadInst.powerId != null && !x.allLoadInst.Draft) : false,
                    x => x.allLoadInst).Select(x => x.allLoadInst.powerId.Value).Distinct().ToList().Count();

            // Load Other..
            OutPutData.OtherLoadsCount = _unitOfWork.CivilLoadsRepository.GetIncludeWhere(x => x.allCivilInstId == AllCivilInst.Id && !x.Dismantle &&
                x.allLoadInstId != null ? (x.allLoadInst.loadOtherId != null && !x.allLoadInst.Draft) : false,
                    x => x.allLoadInst).Select(x => x.allLoadInst.loadOtherId.Value).Distinct().ToList().Count();

            return new Response<SideArmAndLoadsOnCivil>(true, OutPutData, null, null, (int)Helpers.Constants.ApiReturnCode.success);
        }
        public Response<LoadsCountOnSideArm> CheckLoadsOnSideArm(int SideArmId)
        {
            LoadsCountOnSideArm OutPutData = new LoadsCountOnSideArm();

            // MWs..
            OutPutData.MW_DishesCount = _unitOfWork.CivilLoadsRepository.GetIncludeWhere(x => (x.sideArmId != null ? x.sideArmId == SideArmId : false) &&
                !x.Dismantle &&
                x.allLoadInstId != null ? (x.allLoadInst.mwDishId != null && !x.allLoadInst.Draft) : false,
                    x => x.allLoadInst).Select(x => x.allLoadInst.mwDishId.Value).Distinct().ToList().Count();

            OutPutData.MW_BUsCount = _unitOfWork.CivilLoadsRepository.GetIncludeWhere(x => (x.sideArmId != null ? x.sideArmId == SideArmId : false) &&
                !x.Dismantle &&
                x.allLoadInstId != null ? (x.allLoadInst.mwBUId != null && !x.allLoadInst.Draft) : false,
                    x => x.allLoadInst).Select(x => x.allLoadInst.mwBUId.Value).Distinct().ToList().Count();

            OutPutData.MW_ODUsCount = _unitOfWork.CivilLoadsRepository.GetIncludeWhere(x => (x.sideArmId != null ? x.sideArmId == SideArmId : false) &&
                !x.Dismantle &&
                x.allLoadInstId != null ? (x.allLoadInst.mwODUId != null && !x.allLoadInst.Draft) : false,
                    x => x.allLoadInst).Select(x => x.allLoadInst.mwODUId.Value).Distinct().ToList().Count();

            OutPutData.MW_RFUsCount = _unitOfWork.CivilLoadsRepository.GetIncludeWhere(x => (x.sideArmId != null ? x.sideArmId == SideArmId : false) &&
                !x.Dismantle &&
                x.allLoadInstId != null ? (x.allLoadInst.mwRFUId != null && !x.allLoadInst.Draft) : false,
                    x => x.allLoadInst).Select(x => x.allLoadInst.mwRFUId.Value).Distinct().ToList().Count();

            OutPutData.OtherMWsCount = _unitOfWork.CivilLoadsRepository.GetIncludeWhere(x => (x.sideArmId != null ? x.sideArmId == SideArmId : false) &&
                !x.Dismantle &&
                x.allLoadInstId != null ? (x.allLoadInst.mwOtherId != null && !x.allLoadInst.Draft) : false,
                    x => x.allLoadInst).Select(x => x.allLoadInst.mwOtherId.Value).Distinct().ToList().Count();

            // Radios..
            OutPutData.RadioAntennasCount = _unitOfWork.CivilLoadsRepository.GetIncludeWhere(x => (x.sideArmId != null ? x.sideArmId == SideArmId : false) &&
                !x.Dismantle &&
                x.allLoadInstId != null ? (x.allLoadInst.radioAntennaId != null && !x.allLoadInst.Draft) : false,
                    x => x.allLoadInst).Select(x => x.allLoadInst.radioAntennaId.Value).Distinct().ToList().Count();

            OutPutData.RadioRRUsCount = _unitOfWork.CivilLoadsRepository.GetIncludeWhere(x => (x.sideArmId != null ? x.sideArmId == SideArmId : false) &&
                !x.Dismantle &&
                x.allLoadInstId != null ? (x.allLoadInst.radioRRUId != null && !x.allLoadInst.Draft) : false,
                    x => x.allLoadInst).Select(x => x.allLoadInst.radioRRUId.Value).Distinct().ToList().Count();

            OutPutData.OtherRadiosCount = _unitOfWork.CivilLoadsRepository.GetIncludeWhere(x => (x.sideArmId != null ? x.sideArmId == SideArmId : false) &&
                !x.Dismantle &&
                x.allLoadInstId != null ? (x.allLoadInst.radioOtherId != null && !x.allLoadInst.Draft) : false,
                    x => x.allLoadInst).Select(x => x.allLoadInst.radioOtherId.Value).Distinct().ToList().Count();

            // Power..
            OutPutData.PowersCount = _unitOfWork.CivilLoadsRepository.GetIncludeWhere(x => (x.sideArmId != null ? x.sideArmId == SideArmId : false) &&
                !x.Dismantle &&
                x.allLoadInstId != null ? (x.allLoadInst.powerId != null && !x.allLoadInst.Draft) : false,
                    x => x.allLoadInst).Select(x => x.allLoadInst.powerId.Value).Distinct().ToList().Count();

            // Load Other..
            OutPutData.OtherLoadsCount = _unitOfWork.CivilLoadsRepository.GetIncludeWhere(x => (x.sideArmId != null ? x.sideArmId == SideArmId : false) &&
                !x.Dismantle &&
                x.allLoadInstId != null ? (x.allLoadInst.loadOtherId != null && !x.allLoadInst.Draft) : false,
                    x => x.allLoadInst).Select(x => x.allLoadInst.loadOtherId.Value).Distinct().ToList().Count();

            return new Response<LoadsCountOnSideArm>(true, OutPutData, null, null, (int)Helpers.Constants.ApiReturnCode.success);
        }
    }
}
