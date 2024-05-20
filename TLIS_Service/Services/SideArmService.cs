using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Collections;
using System.Threading.Tasks;
using System.Transactions;
using TLIS_DAL;
using TLIS_DAL.Helper;
using TLIS_DAL.Helper.Filters;
using TLIS_DAL.Helpers;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModelBase;
using TLIS_DAL.ViewModels.AttributeActivatedDTOs;
using TLIS_DAL.ViewModels.CabinetDTOs;
using TLIS_DAL.ViewModels.CivilLoadsDTOs;
using TLIS_DAL.ViewModels.DataTypeDTOs;
using TLIS_DAL.ViewModels.DynamicAttDTOs;
using TLIS_DAL.ViewModels.DynamicAttInstValueDTOs;
using TLIS_DAL.ViewModels.DynamicAttLibValueDTOs;
using TLIS_DAL.ViewModels.ItemStatusDTOs;
using TLIS_DAL.ViewModels.OwnerDTOs;
using TLIS_DAL.ViewModels.RuleDTOs;
using TLIS_DAL.ViewModels.SideArmDTOs;
using TLIS_DAL.ViewModels.SideArmInstallationPlaceDTOs;
using TLIS_DAL.ViewModels.SideArmLibraryDTOs;
using TLIS_DAL.ViewModels.SideArmTypeDTOs;
using TLIS_DAL.ViewModels.TicketDTOs;
using TLIS_Repository.Base;
using TLIS_Service.IService;
using static TLIS_Service.Helpers.Constants;
using TLIS_DAL.ViewModels.LegDTOs;
using TLIS_DAL.ViewModels.PowerDTOs;
using TLIS_DAL.ViewModels.LoadOtherDTOs;
using TLIS_DAL.ViewModels.GeneratorDTOs;
using TLIS_DAL.ViewModels.CivilWithLegsDTOs;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Engineering;
using TLIS_DAL.ViewModels.LocationTypeDTOs;
using TLIS_DAL.ViewModels.SupportTypeImplementedDTOs;
using System.Data;
using TLIS_DAL.ViewModels.CivilWithLegLibraryDTOs;
using TLIS_DAL.ViewModels;
using TLIS_DAL.ViewModels.SectionsLegTypeDTOs;
using TLIS_DAL.ViewModels.StructureTypeDTOs;
using TLIS_DAL.ViewModels.SupportTypeDesignedDTOs;
using static TLIS_DAL.ViewModels.CivilWithLegLibraryDTOs.EditCivilWithLegsLibraryObject;
using static TLIS_DAL.ViewModels.SideArmLibraryDTOs.EditSideArmLibraryObject;
using TLIS_DAL.ViewModels.CivilWithoutLegDTOs;
using TLIS_DAL.ViewModels.CivilNonSteelDTOs;
using static TLIS_DAL.ViewModels.SideArmDTOs.EditSidearmInstallationObject;

namespace TLIS_Service.Services
{
    public class SideArmService : ISideArmService
    {
        IUnitOfWork _unitOfWork;
        IServiceCollection _services;
        private readonly ApplicationDbContext _dbContext;
        private IMapper _mapper;
        //public static List<TLIcivilLoads> _SideArmCivilLoads; 
        //public static List<TLIattributeActivated> _AttributeActivated; 
        //public static List<TLIdynamicAtt> _DynamicAttributes; 
        //public static List<TLIdynamicAttInstValue> _DynamicAttributesInstallationValue; 

        public SideArmService(IUnitOfWork unitOfWork, IServiceCollection services, ApplicationDbContext context, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _services = services;
            _dbContext = context;
            _mapper = mapper;
        }
        //public Response<AllItemAttributes> AddSideArm(AddSideArmLibraryObject SideArmViewModel, string SiteCode, string ConnectionString, int? TaskId,int UserId)
        //{
        //    //using (TransactionScope transaction = new TransactionScope())
        //    //{
        //    //    try
        //    //    {
        //    //        TLItablesNames TableNameEntity = _unitOfWork.TablesNamesRepository.GetWhereFirst(x => x.TableName == TablesNames.TLIsideArm.ToString());
        //    //        TLIsideArm SideArm = _mapper.Map<TLIsideArm>(SideArmViewModel);
        //    //        SideArm.Active = true;


        //    //        //string CheckDependencyValidation = CheckDependencyValidationForSideArm(SideArmViewModel, SiteCode);

        //    //        //if (!string.IsNullOrEmpty(CheckDependencyValidation))
        //    //        //    return new Response<AllItemAttributes>(true, null, null, CheckDependencyValidation, (int)ApiReturnCode.fail);

        //    //        //string CheckGeneralValidation = CheckGeneralValidationFunction(SideArmViewModel.dynamicAttribute, TableNameEntity.TableName);

        //    //        //if (!string.IsNullOrEmpty(CheckGeneralValidation))
        //    //        //    return new Response<AllItemAttributes>(true, null, null, CheckGeneralValidation, (int)ApiReturnCode.fail);


        //    //        TLIsideArmInstallationPlace InstallationPlaceEntity = _unitOfWork.SideArmInstallationPlaceRepository
        //    //            .GetByID(SideArm.sideArmInstallationPlaceId);

        //    //        TLIsideArmType sideArmTypeEntity = _unitOfWork.SideArmTypeRepository.GetByID(SideArm.sideArmTypeId);

        //    //        if ((InstallationPlaceEntity.Name.ToLower() == SideArmInstallationPlace.Leg.ToString().ToLower() &&
        //    //            sideArmTypeEntity.Name.ToLower() == SideArmTypes.Normal.ToString().ToLower()) ?
        //    //                ((SideArmViewModel.TLIcivilLoads.legId == null && SideArmViewModel.TLIcivilLoads.Leg2Id == null) || (SideArmViewModel.TLIcivilLoads.Leg2Id != null && SideArmViewModel.TLIcivilLoads.legId != null)) : false)
        //    //        {
        //    //            return new Response<AllItemAttributes>(false, null, null,
        //    //                "Number of legs must be equal to 1 leg only when the installation place is leg and side arm type is normal", (int)ApiReturnCode.fail);
        //    //        }
        //    //        else if (InstallationPlaceEntity.Name.ToLower() == SideArmInstallationPlace.Leg.ToString().ToLower() &&
        //    //            sideArmTypeEntity.Name.ToLower() == SideArmTypes.Special.ToString().ToLower() ?
        //    //                (SideArmViewModel.TLIcivilLoads.legId == null || SideArmViewModel.TLIcivilLoads.Leg2Id == null) : false)
        //    //        {
        //    //            return new Response<AllItemAttributes>(false, null, null,
        //    //                "Number of legs must be equal to 2 leg only when the installation place is leg and side arm type is special", (int)ApiReturnCode.fail);
        //    //        }
        //    //        if (SideArm.Azimuth <= 0)
        //    //        {
        //    //            return new Response<AllItemAttributes>(false, null, null,
        //    //                "Azimuth must bigger from zero", (int)ApiReturnCode.fail);

        //    //        }
        //    //        if (SideArm.HeightBase <= 0)
        //    //        {
        //    //            return new Response<AllItemAttributes>(false, null, null,
        //    //                "HeightBase must bigger from zero", (int)ApiReturnCode.fail);
        //    //        }
        //    //        var civilload = _dbContext.TLIallCivilInst.Where(x => x.Id == SideArmViewModel.TLIcivilLoads.allCivilInstId).Include(x => x.civilNonSteel).Include(x => x.civilWithLegs).Include(x => x.civilWithoutLeg).FirstOrDefault();
        //    //        if (civilload.civilWithLegsId != null && sideArmTypeEntity.Name.ToLower() == SideArmTypes.Normal.ToString().ToLower())
        //    //        {
        //    //            var civilwithlegname = _unitOfWork.CivilWithLegsRepository.GetWhereFirst(x => x.Id == civilload.civilWithLegsId).Name;
        //    //            var LegName = _unitOfWork.LegRepository.GetWhereFirst(x => x.Id == SideArmViewModel.TLIcivilLoads.legId || x.Id == SideArmViewModel.TLIcivilLoads.Leg2Id).CiviLegName;
        //    //            SideArm.Name = civilwithlegname + LegName + SideArmViewModel.installationAttributes.HeightBase + SideArmViewModel.installationAttributes.Azimuth;
        //    //        }
        //    //        if (civilload.civilWithLegsId != null && sideArmTypeEntity.Name.ToLower() == SideArmTypes.Special.ToString().ToLower())
        //    //        {
        //    //            var civilwithlegname = _unitOfWork.CivilWithLegsRepository.GetWhereFirst(x => x.Id == civilload.civilWithLegsId).Name;
        //    //            var LegName = _unitOfWork.LegRepository.GetWhereFirst(x => x.Id == SideArmViewModel.TLIcivilLoads.legId).CiviLegName;
        //    //            var LegName2 = _unitOfWork.LegRepository.GetWhereFirst(x => x.Id == SideArmViewModel.TLIcivilLoads.Leg2Id).CiviLegName;
        //    //            SideArm.Name = civilwithlegname + LegName + LegName2 + SideArmViewModel.installationAttributes.HeightBase + SideArmViewModel.installationAttributes.Azimuth;
        //    //        }
        //    //        if (civilload.civilWithoutLegId != null && sideArmTypeEntity.Name.ToLower() == SideArmTypes.Normal.ToString().ToLower())
        //    //        {
        //    //            var civilwithlegname = _unitOfWork.CivilWithoutLegRepository.GetWhereFirst(x => x.Id == civilload.civilWithoutLegId).Name;
        //    //            SideArm.Name = civilwithlegname + SideArmViewModel.installationAttributes.HeightBase + SideArmViewModel.installationAttributes.Azimuth;
        //    //        }
        //    //        if (civilload.civilNonSteelId != null && sideArmTypeEntity.Name.ToLower() == SideArmTypes.Normal.ToString().ToLower())
        //    //        {
        //    //            var civilwithlegname = _unitOfWork.CivilNonSteelRepository.GetWhereFirst(x => x.Id == civilload.civilNonSteelId).Name;
        //    //            SideArm.Name = civilwithlegname + SideArmViewModel.installationAttributes.HeightBase + SideArmViewModel.installationAttributes.Azimuth;
        //    //        }
        //    //        TLIcivilLoads CheckName = _unitOfWork.CivilLoadsRepository.GetIncludeWhereFirst(x => x.sideArmId != null ?
        //    //            (x.sideArm.Name.ToLower() == SideArm.Name.ToLower() && !x.sideArm.Draft && !x.Dismantle &&
        //    //                x.SiteCode.ToLower() == SiteCode.ToLower()) : false,
        //    //                x => x.sideArm);

        //    //        if (CheckName != null)
        //    //            return new Response<AllItemAttributes>(false, null, null, $"This name {SideArm.Name} is already exists", (int)ApiReturnCode.fail);

        //    //        _unitOfWork.SideArmRepository.AddWithHistory(UserId, SideArm);
        //    //        _unitOfWork.SaveChanges();

        //    //        TLIcivilLoads civilLoad = new TLIcivilLoads
        //    //        {
        //    //            InstallationDate = SideArmViewModel.TLIcivilLoads.InstallationDate,
        //    //            allCivilInstId = SideArmViewModel.TLIcivilLoads.allCivilInstId,
        //    //            civilSteelSupportCategoryId = SideArmViewModel.TLIcivilLoads.civilSteelSupportCategoryId,
        //    //            ItemOnCivilStatus = SideArmViewModel.TLIcivilLoads.ItemOnCivilStatus,
        //    //            ItemStatus = SideArmViewModel.TLIcivilLoads.ItemStatus,
        //    //            ReservedSpace = SideArmViewModel.TLIcivilLoads.ReservedSpace,
        //    //            sideArmId = SideArm.Id,
        //    //            SiteCode = SiteCode,
        //    //            legId = SideArmViewModel.TLIcivilLoads.legId,
        //    //            Leg2Id = SideArmViewModel.TLIcivilLoads.Leg2Id
        //    //        };

        //    //        _unitOfWork.CivilLoadsRepository.AddWithHistory(UserId, civilLoad);
        //    //        _unitOfWork.SaveChangesAsync();



        //    //        if (SideArmViewModel.dynamicAttribute != null ? SideArmViewModel.dynamicAttribute.Count > 0 : false)
        //    //        {
        //    //            foreach (var DynamicAttInstValue in SideArmViewModel.dynamicAttribute)
        //    //            {
        //    //                _unitOfWork.DynamicAttInstValueRepository.AddDdynamicAttributeInstallation(UserId, DynamicAttInstValue, TableNameEntity.Id, SideArm.Id);
        //    //            }
        //    //        }
        //    //        if (TaskId != null)
        //    //        {
        //    //            var Submit = _unitOfWork.SiteRepository.SubmitTaskByTLI(TaskId);
        //    //            var result = Submit.Result;
        //    //            if (result.result == true && result.errorMessage == null)
        //    //            {
        //    //                _unitOfWork.SaveChanges();
        //    //                transaction.Complete();
        //    //            }
        //    //            else
        //    //            {
        //    //                transaction.Dispose();
        //    //                return new Response<AllItemAttributes>(false, null, null, result.errorMessage.ToString(), (int)ApiReturnCode.fail);
        //    //            }
        //    //        }
        //    //        else
        //    //        {
        //    //            _unitOfWork.SaveChanges();
        //    //            transaction.Complete();
        //    //        }
        //    //        return new Response<AllItemAttributes>();
        //    //    }
        //    //    catch (Exception err)
        //    //    {
        //    //        return new Response<AllItemAttributes>(false, null, null, err.Message, (int)ApiReturnCode.fail);
        //    //    }
        //    //}
        //    return new Response<AllItemAttributes>();
        //}
        public Response<GetForAddCivilLoadObject> GetAttForAdd(int LibraryId)
        {
            try
            {
                TLItablesNames TableNameEntity = _unitOfWork.TablesNamesRepository.GetWhereFirst(c =>
                    c.TableName == TablesNames.TLIsideArm.ToString());

                GetForAddCivilLoadObject objectInst = new GetForAddCivilLoadObject();

                TLIsideArmLibrary sideArmLibrary = _unitOfWork.SideArmLibraryRepository.GetWhereFirst(x=>x.Id==LibraryId && x.Active && !x.Deleted);
                if (sideArmLibrary != null)
                {
                    List<BaseInstAttViews> LibraryAttributeActivated = _unitOfWork.AttributeActivatedRepository.
                        GetAttributeActivatedGetForAdd(TablesNames.TLIsideArmLibrary.ToString(), sideArmLibrary).ToList();

                    List<BaseInstAttViews> AddToLibraryAttributesActivated = _mapper.Map<List<BaseInstAttViews>>(_unitOfWork.LogistcalRepository
                        .GetLogisticals(TablePartName.SideArm.ToString(), TablesNames.TLIsideArmLibrary.ToString(), sideArmLibrary.Id).ToList());

                    LibraryAttributeActivated.AddRange(AddToLibraryAttributesActivated);

                    objectInst.LibraryAttribute = LibraryAttributeActivated;

                    List<BaseInstAttViews> ListAttributesActivated = _unitOfWork.AttributeActivatedRepository.
                        GetInstAttributeActivatedGetForAdd(TablesNames.TLIsideArm.ToString(), null, "Name", "sideArmLibraryId", "ItemStatusId", "TicketId", "sideArmInstallationPlaceId", "sideArmTypeId").ToList();

                    BaseInstAttViews NameAttribute = ListAttributesActivated.FirstOrDefault(x => x.Key.ToLower() == "Name".ToLower());
                    if (NameAttribute != null)
                    {
                        BaseInstAttViews Swap = ListAttributesActivated[0];
                        ListAttributesActivated[ListAttributesActivated.IndexOf(NameAttribute)] = Swap;
                        ListAttributesActivated[0] = NameAttribute;
                    }
                    Dictionary<string, Func<IEnumerable<object>>> repositoryMethods = new Dictionary<string, Func<IEnumerable<object>>>
                {
                    { "owner_name", () => _mapper.Map<List<OwnerViewModel>>(_unitOfWork.OwnerRepository.GetWhere(x => !x.Deleted && !x.Disable).ToList()) },
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

                    objectInst.dynamicAttribute = DynamicAttributesWithoutValue;

                    objectInst.CivilLoads = _unitOfWork.AttributeActivatedRepository
                        .GetInstAttributeActivatedGetForAdd(TablesNames.TLIcivilLoads.ToString(), null, null, "allLoadInstId", "Dismantle", "SiteCode", "legId",
                            "Leg2Id", "sideArmId", "allCivilInstId", "civilSteelSupportCategoryId").ToList();

                    return new Response<GetForAddCivilLoadObject>(true, objectInst, null, null, (int)ApiReturnCode.success);

                }
                else
                {
                    return new Response<GetForAddCivilLoadObject>(false, null, null, "this sidearmlibrary is not found", (int)ApiReturnCode.success);
                }
            }
            catch (Exception err)
            {
                return new Response<GetForAddCivilLoadObject>(false, null, null, err.Message, (int)ApiReturnCode.fail);
            }
        }
        public Response<bool> DismantleSideArm(string SiteCode, int sideArmId, int? TaskId)
        {
            using (TransactionScope transactionScope = new TransactionScope())
            {
                try
                {
                    var civilLoads = _dbContext.TLIcivilLoads.Where(x => x.sideArmId == sideArmId && x.SiteCode == SiteCode && x.Dismantle == false).ToList();
                    foreach (var sidearm in civilLoads)
                    {
                        sidearm.Dismantle = true;
                    }
                    _dbContext.SaveChanges();
                    if (TaskId != null)
                    {
                        var Submit = _unitOfWork.SiteRepository.SubmitTaskByTLI(TaskId);
                        var result = Submit.Result;
                        if (result.result == true && result.errorMessage == null)
                        {
                            _unitOfWork.SaveChanges();
                            transactionScope.Complete();
                        }
                        else
                        {
                            transactionScope.Dispose();
                            return new Response<bool>(false, false, null, result.errorMessage.ToString(), (int)Helpers.Constants.ApiReturnCode.fail);
                        }
                    }
                    else
                    {
                        _unitOfWork.SaveChanges();
                        transactionScope.Complete();
                    }
                    return new Response<bool>(true, true, null, null, (int)Helpers.Constants.ApiReturnCode.success);
                }
                catch (Exception er)
                {

                    return new Response<bool>(false, false, null, er.Message, (int)Helpers.Constants.ApiReturnCode.fail);
                }
            }
        }
        //Function take 3 parameters
        //Function return all sideArms depened on filters, parameters
        //If WithFilterData is true return get related tables
        public Response<ReturnWithFilters<SideArmDisplayedOnTableViewModel>> getSideArms(CivilLoadsFilter BaseFilter, bool WithFilterData, List<FilterObjectList> filters)
        {
            try
            {
                List<SideArmDisplayedOnTableViewModel> OutPutListWithDuplicate = new List<SideArmDisplayedOnTableViewModel>();
                ReturnWithFilters<SideArmDisplayedOnTableViewModel> SideArmTableDisplay = new ReturnWithFilters<SideArmDisplayedOnTableViewModel>();
                int count = 0;

                List<TLIcivilLoads> AllCivilLoadsRecords = GetCivilLoadsWithConditions(BaseFilter, null);
                List<TLIcivilLoads> CivilLoadsRecords = GetMaxInstallationDate(AllCivilLoadsRecords);

                foreach (TLIcivilLoads item in CivilLoadsRecords)
                {
                    SideArmDisplayedOnTableViewModel OutPut = new SideArmDisplayedOnTableViewModel();
                    SideArmViewModel SideArm = _unitOfWork.SideArmRepository.Get(item.sideArmId.Value);

                    TLIsideArmLibrary TLISideArmLib = _unitOfWork.SideArmLibraryRepository.GetIncludeWhereFirst(x => x.Id == item.sideArm.sideArmLibraryId, x => x.sideArms);
                    SideArmLibraryViewModel SideArmLibViewModel = _mapper.Map<SideArmLibraryViewModel>(TLISideArmLib);

                    var DynamicAttInstValueRecords = _unitOfWork.DynamicAttInstValueRepository.GetIncludeWhere(x =>
                        (x.DynamicAtt.disable == false &&
                        x.DynamicAtt.tablesNames.TableName == "TLIsideArm" &&
                        x.InventoryId == SideArm.Id &&
                        !x.DynamicAtt.LibraryAtt), x => x.DynamicAtt, x => x.tablesNames, x => x.DynamicAtt.DataType).ToList();

                    List<DynamicAttDto> DynamicAttListCopy = new List<DynamicAttDto>();

                    // 
                    // Add All The Dynamic Attributes Installation Value To The AddToDynamicAttList List...
                    //
                    foreach (var DynamicAttInstValueRecord in DynamicAttInstValueRecords)
                    {
                        var CivilNonSteelDynamicAttDto = GetDynamicAttDto(DynamicAttInstValueRecord, null);
                        DynamicAttListCopy.Add(CivilNonSteelDynamicAttDto);
                    }

                    //
                    // Library
                    // 
                    var DynamicAttLibRecords = _unitOfWork.DynamicAttLibRepository.GetIncludeWhere(x =>
                        (x.DynamicAtt.disable == false &&
                        x.DynamicAtt.tablesNames.TableName == "TLIsideArmLibrary" &&
                        x.InventoryId == SideArm.sideArmLibraryId &&
                        x.DynamicAtt.LibraryAtt), x => x.DynamicAtt, x => x.tablesNames, x => x.DynamicAtt.DataType).ToList();

                    foreach (var DynamicAttLibRecord in DynamicAttLibRecords)
                    {
                        var CivilNonSteelDynamicAttDto = GetDynamicAttDto(null, DynamicAttLibRecord);
                        DynamicAttListCopy.Add(CivilNonSteelDynamicAttDto);
                    }
                    OutPut.DynamicAttList = DynamicAttListCopy;
                    OutPut.SideArm = SideArm;
                    OutPut.SideArmLib = SideArmLibViewModel;
                    OutPutListWithDuplicate.Add(OutPut);
                }
                var OutPutList = OutPutListWithDuplicate.GroupBy(x => x.SideArm.Id).Select(x => x.First()).ToList();

                // var SideArms = _unitOfWork.SideArmRepository.GetAllIncludeMultiple(parameters, filters, out count, s => s.owner, s => s.sideArmInstallationPlace, s => s.sideArmLibrary).ToList();
                // var SideArmsViewModel = _mapper.Map<List<SideArmViewModel>>(SideArms);
                SideArmTableDisplay.Model = OutPutList;
                if (WithFilterData == true)
                {
                    var Owners = _unitOfWork.OwnerRepository.GetAllWithoutCount();
                    var OwnersFilters = _mapper.Map<List<DropDownListFilters>>(Owners);
                    SideArmTableDisplay.filters = _unitOfWork.SideArmRepository.GetRelatedTables();
                }
                else
                {
                    SideArmTableDisplay.filters = null;
                }
                return new Response<ReturnWithFilters<SideArmDisplayedOnTableViewModel>>(true, SideArmTableDisplay, null, null, (int)ApiReturnCode.success, count);
            }
            catch (Exception err)
            {

                return new Response<ReturnWithFilters<SideArmDisplayedOnTableViewModel>>(true, null, null, err.Message, (int)ApiReturnCode.fail);
            }
        }
        public Response<ReturnWithFilters<object>> GetSideArmsWithEnabledAtt(CivilLoadsFilter BaseFilter, bool WithFilterData, ParameterPagination parameterPagination, CombineFilters CombineFilters, int? CivilId, string CivilType)
        {
            try
            {
                int Count = 0;
                List<object> OutPutList = new List<object>();
                ReturnWithFilters<object> SideArmTableDisplay = new ReturnWithFilters<object>();

                List<TLIcivilLoads> AllCivilLoadsRecords = GetCivilLoadsWithConditions(BaseFilter, CombineFilters);
                List<TLIcivilLoads> CivilLoadsRecords = GetMaxInstallationDate(AllCivilLoadsRecords);

                List<SideArmViewModel> SideArms = _mapper.Map<List<SideArmViewModel>>(CivilLoadsRecords.Select(x =>
                    x.sideArm).ToList());

                if (CivilId != null)
                {
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

                    List<int> SideArmIds = _unitOfWork.CivilLoadsRepository.GetIncludeWhere(x => x.allCivilInstId == AllCivilInst.Id && !x.Dismantle &&
                        x.sideArmId != null, x => x.sideArm).Select(x => x.sideArmId.Value).Distinct().ToList();

                    SideArms = SideArms.Where(x => SideArmIds.Contains(_mapper.Map<SideArmViewModel>(x).Id)).ToList();
                }

                Count = SideArms.Count();

                SideArms = SideArms.Skip((parameterPagination.PageNumber - 1) * parameterPagination.PageSize).
                    Take(parameterPagination.PageSize).ToList();

                List<TLIattributeViewManagment> AllAttributes = _unitOfWork.AttributeViewManagmentRepository.GetIncludeWhere(x =>
                   (x.Enable && x.EditableManagmentView.View == EditableManamgmantViewNames.SideArmInstallation.ToString() &&
                   (x.AttributeActivatedId != null ?
                        (x.AttributeActivated.Tabel == TablesNames.TLIsideArm.ToString() && x.AttributeActivated.enable &&
                         x.AttributeActivated.Key.ToLower() != "Draft".ToLower() && x.AttributeActivated.Key.ToLower() != "TicketId".ToLower() &&
                         x.AttributeActivated.Key.ToLower() != "ItemStatusId".ToLower() && x.AttributeActivated.Key.ToLower() != "ReservedSpace".ToLower() &&
                         x.AttributeActivated.Key.ToLower() != "Active".ToLower()) :
                        (!x.DynamicAtt.LibraryAtt && !x.DynamicAtt.disable && x.DynamicAtt.tablesNames.TableName == TablesNames.TLIsideArm.ToString()))) ||
                    (x.AttributeActivated != null ?
                        ((x.AttributeActivated.Key.ToLower() == "id" || x.AttributeActivated.Key.ToLower() == "active") && x.AttributeActivated.Tabel == TablesNames.TLIsideArm.ToString()) : false),
                       x => x.EditableManagmentView, x => x.EditableManagmentView.TLItablesNames1, x => x.EditableManagmentView.TLItablesNames2,
                       x => x.AttributeActivated, x => x.DynamicAtt, x => x.DynamicAtt.tablesNames, x => x.DynamicAtt.DataType).ToList();

                List<TLIattributeViewManagment> NotDateTimeInstallationAttributesViewModel = AllAttributes.Where(x =>
                    x.AttributeActivatedId != null ? (x.AttributeActivated.Key.ToLower() != "deleted" && x.AttributeActivated.DataType.ToLower() != "datetime" &&
                    x.AttributeActivated.Key.ToLower() != "Active".ToLower()) : false).ToList();

                List<TLIattributeViewManagment> NotDateTimeDynamicInstallationAttributesViewModel = AllAttributes.Where(x =>
                    x.DynamicAttId != null ? x.DynamicAtt.DataType.Name.ToLower() != "datetime" : false).ToList();

                List<TLIattributeViewManagment> DateTimeInstallationAttributesViewModel = AllAttributes.Where(x =>
                    x.AttributeActivatedId != null ? (x.AttributeActivated.Key.ToLower() != "deleted" && x.AttributeActivated.DataType.ToLower() == "datetime") : false).ToList();

                List<TLIattributeViewManagment> DateTimeDynamicInstallationAttributesViewModel = AllAttributes.Where(x =>
                    x.DynamicAttId != null ? x.DynamicAtt.DataType.Name.ToLower() == "datetime" : false).ToList();

                foreach (SideArmViewModel SideArmInstallationObject in SideArms)
                {
                    dynamic DynamiSideArmInstallation = new ExpandoObject();

                    //
                    // Installation Object ViewModel...
                    //
                    if (NotDateTimeInstallationAttributesViewModel != null ? NotDateTimeInstallationAttributesViewModel.Count > 0 : false)
                    {
                        List<PropertyInfo> InstallationProps = typeof(SideArmViewModel).GetProperties().Where(x =>
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
                                object ForeignKeyNamePropObject = prop.GetValue(SideArmInstallationObject, null);
                                ((IDictionary<String, Object>)DynamiSideArmInstallation).Add(new KeyValuePair<string, object>(prop.Name, ForeignKeyNamePropObject));
                            }
                            else if (NotDateTimeInstallationAttributesViewModel.Select(x =>
                                 x.AttributeActivated.Key.ToLower()).Contains(prop.Name.ToLower()) &&
                                !prop.Name.ToLower().Contains("_name") &&
                                (prop.Name.ToLower().Substring(Math.Max(0, prop.Name.Length - 2)) != "id" || prop.Name.ToLower() == "id"))
                            {
                                if (prop.Name.ToLower() != "id" && prop.Name.ToLower() != "active")
                                {
                                    TLIattributeViewManagment LabelName = AllAttributes.FirstOrDefault(x => ((x.AttributeActivated != null) ? x.AttributeActivated.Key == prop.Name : false) &&
                                        x.AttributeActivated.Tabel == TablesNames.TLIsideArm.ToString() &&
                                        x.Enable && x.AttributeActivated.DataType != "List" && x.Id != 0);

                                    if (LabelName != null)
                                    {
                                        object PropObject = prop.GetValue(SideArmInstallationObject, null);
                                        ((IDictionary<String, Object>)DynamiSideArmInstallation).Add(new KeyValuePair<string, object>(LabelName.AttributeActivated.Label, PropObject));
                                    }
                                }
                                else
                                {
                                    object PropObject = prop.GetValue(SideArmInstallationObject, null);
                                    ((IDictionary<String, Object>)DynamiSideArmInstallation).Add(new KeyValuePair<string, object>(prop.Name, PropObject));
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
                            !x.disable && x.tablesNames.TableName == TablesNames.TLIsideArm.ToString() &&
                            !x.LibraryAtt && x.DataType.Name.ToLower() != "datetime" &&
                            NotDateTimeDynamicInstallationAttributesViewModel.Select(y => y.DynamicAttId).Any(y => y == x.Id)
                                , x => x.tablesNames, x => x.DataType).ToList();

                        List<TLIdynamicAttInstValue> NotDateTimeDynamicAttInstValues = _unitOfWork.DynamicAttInstValueRepository.GetIncludeWhere(x =>
                            !x.DynamicAtt.LibraryAtt && !x.disable &&
                            x.InventoryId == SideArmInstallationObject.Id &&
                            NotDateTimeInstallationDynamicAttributes.AsEnumerable().Select(y => y.Key.ToLower()).Contains(x.DynamicAtt.Key.ToLower()) &&
                            x.tablesNames.TableName == TablesNames.TLIsideArm.ToString()
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

                                ((IDictionary<String, Object>)DynamiSideArmInstallation).Add(new KeyValuePair<string, object>(InstallationDynamicAtt.Key, DynamicAttValue));
                            }
                            else
                            {
                                ((IDictionary<String, Object>)DynamiSideArmInstallation).Add(new KeyValuePair<string, object>(InstallationDynamicAtt.Key, null));
                            }
                        }
                    }

                    //
                    // Installation Object ViewModel... (DateTime DataType Attribute)
                    //
                    dynamic DateTimeAttributes = new ExpandoObject();
                    if (DateTimeInstallationAttributesViewModel != null ? DateTimeInstallationAttributesViewModel.Count() > 0 : false)
                    {
                        List<PropertyInfo> DateTimeInstallationProps = typeof(SideArmViewModel).GetProperties().Where(x =>
                            x.PropertyType.GenericTypeArguments != null ?
                                (x.PropertyType.GenericTypeArguments.Count() > 0 ? x.PropertyType.GenericTypeArguments.FirstOrDefault().Name.ToLower() == "datetime" :
                                (x.PropertyType.Name.ToLower() == "datetime")) :
                            (x.PropertyType.Name.ToLower() == "datetime")).ToList();

                        foreach (PropertyInfo prop in DateTimeInstallationProps)
                        {
                            TLIattributeViewManagment LabelName = AllAttributes.FirstOrDefault(x => ((x.AttributeActivated != null) ? x.AttributeActivated.Key == prop.Name : false) &&
                                x.AttributeActivated.Tabel == TablesNames.TLIsideArm.ToString() &&
                                x.Enable && x.AttributeActivated.DataType != "List" && x.Id != 0);

                            if (LabelName != null)
                            {
                                object PropObject = prop.GetValue(SideArmInstallationObject, null);
                                ((IDictionary<String, Object>)DateTimeAttributes).Add(new KeyValuePair<string, object>(LabelName.AttributeActivated.Label, PropObject));
                            }
                        }
                    }

                    //
                    // Installation Dynamic Attributes... (DateTime DataType Attribute)
                    // 
                    if (DateTimeDynamicInstallationAttributesViewModel != null ? DateTimeDynamicInstallationAttributesViewModel.Count > 0 : false)
                    {
                        List<TLIdynamicAtt> DateTimeInstallationDynamicAttributes = _unitOfWork.DynamicAttRepository.GetIncludeWhere(x =>
                           !x.disable && x.tablesNames.TableName == TablesNames.TLIsideArm.ToString() &&
                           !x.LibraryAtt && x.DataType.Name.ToLower() == "datetime" &&
                            DateTimeDynamicInstallationAttributesViewModel.Select(y => y.DynamicAttId).Any(y => y == x.Id), x => x.tablesNames).ToList();

                        List<TLIdynamicAttInstValue> DateTimeDynamicAttInstValues = _unitOfWork.DynamicAttInstValueRepository.GetIncludeWhere(x =>
                            x.InventoryId == SideArmInstallationObject.Id && !x.disable &&
                           !x.DynamicAtt.LibraryAtt &&
                            DateTimeInstallationDynamicAttributes.AsEnumerable().Select(y => y.Key.ToLower()).Contains(x.DynamicAtt.Key.ToLower()) &&
                            x.tablesNames.TableName == TablesNames.TLIsideArm.ToString()
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
                    ((IDictionary<String, Object>)DynamiSideArmInstallation).Add(new KeyValuePair<string, object>("DateTimeAttributes", DateTimeAttributes));

                    OutPutList.Add(DynamiSideArmInstallation);
                }
                SideArmTableDisplay.Model = OutPutList;


                if (WithFilterData == true)
                {
                    SideArmTableDisplay.filters = _unitOfWork.SideArmRepository.GetRelatedTables();
                }
                else
                {
                    SideArmTableDisplay.filters = null;
                }

                return new Response<ReturnWithFilters<object>>(true, SideArmTableDisplay, null, null, (int)ApiReturnCode.success, Count);
            }
            catch (Exception err)
            {
                return new Response<ReturnWithFilters<object>>(true, null, null, err.Message, (int)ApiReturnCode.fail);
            }
        }

        public Response<ObjectInstAttsForSideArm> GetSideArmById(int SideArmId, string TableName)
        {
            try
            {
                var TableNameEntity = _unitOfWork.TablesNamesRepository.GetWhereFirst(c => c.TableName == TableName);
                ObjectInstAttsForSideArm objectInst = new ObjectInstAttsForSideArm();

                TLIsideArm side = _unitOfWork.SideArmRepository
                    .GetIncludeWhereFirst(x => x.Id == SideArmId, x => x.owner, x => x.sideArmInstallationPlace, x => x.sideArmType, x => x.sideArmLibrary, x => x.ItemStatus, x => x.Ticket);

                SideArmLibraryViewModel sideArmLibrary = _mapper.Map<SideArmLibraryViewModel>(_unitOfWork.SideArmLibraryRepository
                    .GetByID(side.sideArmLibraryId));

                List<BaseAttView> LibraryAttributes = _unitOfWork.AttributeActivatedRepository
                    .GetAttributeActivated(TablesNames.TLIsideArmLibrary.ToString(), sideArmLibrary, null).ToList();

                foreach (BaseAttView LibraryAttribute in LibraryAttributes)
                {
                    if (LibraryAttribute.DataType.ToLower() == "list")
                    {
                        LibraryAttribute.Value = sideArmLibrary.GetType().GetProperties()
                            .FirstOrDefault(x => x.Name.ToLower() == LibraryAttribute.Label.ToLower()).GetValue(sideArmLibrary);
                    }
                }

                List<BaseAttView> LibraryLogisticalAttributes = _mapper.Map<List<BaseAttView>>(_unitOfWork.LogistcalRepository
                     .GetLogistical("SideArm", TablesNames.TLIsideArmLibrary.ToString(), sideArmLibrary.Id).ToList());

                LibraryAttributes.AddRange(LibraryLogisticalAttributes);
                objectInst.LibraryActivatedAttributes = LibraryAttributes;

                List<BaseInstAttView> ListAttributesActivated = _unitOfWork.AttributeActivatedRepository
                    .GetInstAttributeActivated(TablesNames.TLIsideArm.ToString(), side, "TicketId", "Draft", /*"EquivalentSpace",*/
                        "HBA", "ItemStatusId", "ReservedSpace", "sideArmInstallationPlaceId", "sideArmTypeId").ToList();

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
                        if (side.owner == null)
                            FKitem.Value = "NA";

                        else
                            FKitem.Value = side.owner.OwnerName;
                    }
                    else if (FKitem.Desc.ToLower() == "tlisidearminstallationplace")
                    {
                        if (side.sideArmInstallationPlace == null)
                            FKitem.Value = "NA";

                        else
                            FKitem.Value = side.sideArmInstallationPlace.Name;
                    }
                    else if (FKitem.Desc.ToLower() == "tlisidearmtype")
                    {
                        if (side.sideArmType == null)
                            FKitem.Value = "NA";

                        else
                            FKitem.Value = side.sideArmType.Name;
                    }
                    else if (FKitem.Desc.ToLower() == "tliitemstatus")
                    {
                        if (side.ItemStatus == null)
                            FKitem.Value = "NA";

                        else
                            FKitem.Value = side.ItemStatus.Name;
                    }
                    else if (FKitem.Desc.ToLower() == "tlisidearmlibrary")
                    {
                        if (side.sideArmLibrary == null)
                            FKitem.Value = "NA";

                        else
                            FKitem.Value = side.sideArmLibrary.Model;
                    }
                    else if (FKitem.Desc.ToLower() == "tliticket")
                        FKitem.Value = _mapper.Map<List<ListTicketViewModel>>(_unitOfWork.TicketRepository.GetAllWithoutCount().ToList());
                }

                objectInst.AttributesActivated = ListAttributesActivated;
                objectInst.DynamicAtts = _unitOfWork.DynamicAttInstValueRepository
                    .GetDynamicInstAtts(TableNameEntity.Id, SideArmId, null);

                TLIcivilLoads CivilLoads = _unitOfWork.CivilLoadsRepository.GetIncludeWhereFirst(x => x.sideArmId == SideArmId && x.allLoadInst == null,
                    x => x.sideArm, x => x.site, x => x.leg, x => x.allCivilInst,
                    x => x.allCivilInst.civilWithLegs, x => x.allCivilInst.civilWithoutLeg, x => x.allCivilInst.civilNonSteel, x => x.civilSteelSupportCategory);

                if (CivilLoads != null)
                {
                    List<KeyValuePair<string, List<DropDownListFilters>>> sidearmRelatedTables = _unitOfWork.SideArmRepository
                        .GetRelatedTables();

                    List<KeyValuePair<string, List<DropDownListFilters>>> CivilLoadsRelatedTables = _unitOfWork.CivilLoadsRepository
                        .GetRelatedTables(CivilLoads.SiteCode);

                    sidearmRelatedTables.AddRange(CivilLoadsRelatedTables);

                    if (CivilLoads.allCivilInst.civilWithLegsId != null)
                    {
                        List<TLIleg> LegsForCivilWithLegLibrary = _unitOfWork.LegRepository
                            .GetWhere(x => x.CivilWithLegInstId == CivilLoads.allCivilInst.civilWithLegsId).ToList();

                        List<DropDownListFilters> LegIds = _mapper.Map<List<DropDownListFilters>>(LegsForCivilWithLegLibrary);

                        sidearmRelatedTables.Add(new KeyValuePair<string, List<DropDownListFilters>>("Leg1Id", LegIds));

                        List<TLIleg> Legs2ForCivilWithLegLibrary = LegsForCivilWithLegLibrary.Except(LegsForCivilWithLegLibrary
                            .Where(x => x.Id == CivilLoads.legId)).ToList();

                        List<DropDownListFilters> Leg2Ids = _mapper.Map<List<DropDownListFilters>>(Legs2ForCivilWithLegLibrary);

                        sidearmRelatedTables.Add(new KeyValuePair<string, List<DropDownListFilters>>("Leg2Id", Leg2Ids));
                    }

                    objectInst.RelatedTables = sidearmRelatedTables;

                    List<BaseAttView> LoadInstAttributes = _unitOfWork.AttributeActivatedRepository
                        .GetAttributeActivated(TablesNames.TLIcivilLoads.ToString(), CivilLoads, null, "allLoadInstId",
                            "Dismantle", "SiteCode", "civilSteelSupportCategoryId", "legId", "Leg2Id", "sideArmId", "allCivilInstId").ToList();

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
                                FKitem.Value = "NA";

                            else
                                FKitem.Value = CivilLoads.leg.CiviLegName;
                        }
                        else if (FKitem.Desc.ToLower() == "tlicivilsteelsupportcategory")
                        {
                            if (CivilLoads.civilSteelSupportCategory == null)
                                FKitem.Value = "NA";

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
                    }
                    objectInst.CivilLoads = _mapper.Map<IEnumerable<BaseInstAttView>>(LoadInstAttributes);
                }

                List<BaseInstAttView> xxx = new List<BaseInstAttView>();

                TLIallCivilInst AllCivilInst = _unitOfWork.CivilLoadsRepository
                    .GetIncludeWhereFirst(x => x.sideArmId == SideArmId && !x.Dismantle && x.allLoadInstId == null, x => x.allCivilInst).allCivilInst;
                if (AllCivilInst.civilWithLegsId != null)
                {
                    xxx.Add(new BaseInstAttView
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
                        Value = "Civil With Legs"
                    });
                    xxx.Add(new BaseInstAttView
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
                    xxx.Add(new BaseInstAttView
                    {
                        AutoFill = false,
                        DataType = "List",
                        DataTypeId = null,
                        Desc = "Select side arm type",
                        enable = true,
                        Id = side.sideArmTypeId,
                        Key = "Select side arm type",
                        Label = "Select side arm type",
                        Manage = false,
                        Required = false,
                        Value = side.sideArmType.Name
                    });
                    if (side.sideArmType.Name.ToLower() == SideArmTypes.Special.ToString().ToLower())
                    {
                        xxx.Add(new BaseInstAttView
                        {
                            AutoFill = false,
                            DataType = "List",
                            DataTypeId = null,
                            Desc = "Select side arm installtion type",
                            enable = true,
                            Id = side.sideArmInstallationPlaceId != null ? side.sideArmInstallationPlaceId : 0,
                            Key = "Select side arm installtion type",
                            Label = "Select side arm installtion type",
                            Manage = false,
                            Required = false,
                            Value = side.sideArmInstallationPlace.Name
                        });
                        xxx.Add(new BaseInstAttView
                        {
                            AutoFill = false,
                            DataType = "List",
                            DataTypeId = null,
                            Desc = "Select two legs",
                            enable = true,
                            Id = side.sideArmInstallationPlaceId != null ? side.sideArmInstallationPlaceId : 0,
                            Key = "Select two legs",
                            Label = "Select two legs",
                            Manage = false,
                            Required = false,
                            Value = CivilLoads.Leg2Id != 0 ? CivilLoads.leg.CiviLegName + "," + _unitOfWork.LegRepository.GetByID(CivilLoads.Leg2Id.Value).CiviLegName :
                                CivilLoads.leg.CiviLegName
                        });
                    }
                    else
                    {
                        xxx.Add(new BaseInstAttView
                        {
                            AutoFill = false,
                            DataType = "List",
                            DataTypeId = null,
                            Desc = "Select side arm installtion type",
                            enable = true,
                            Id = side.sideArmInstallationPlaceId != null ? side.sideArmInstallationPlaceId : 0,
                            Key = "Select side arm installtion type",
                            Label = "Select side arm installtion type",
                            Manage = false,
                            Required = false,
                            Value = side.sideArmInstallationPlace.Name
                        });
                        xxx.Add(new BaseInstAttView
                        {
                            AutoFill = false,
                            DataType = "List",
                            DataTypeId = null,
                            Desc = "Select leg",
                            enable = true,
                            Id = CivilLoads.legId.Value,
                            Key = "Select leg",
                            Label = "Select leg",
                            Manage = false,
                            Required = false,
                            Value = CivilLoads.leg.CiviLegName
                        });
                    }
                }
                else if (AllCivilInst.civilWithoutLegId != null)
                {
                    xxx.Add(new BaseInstAttView
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
                    xxx.Add(new BaseInstAttView
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
                    xxx.Add(new BaseInstAttView
                    {
                        AutoFill = false,
                        DataType = "List",
                        DataTypeId = null,
                        Desc = "Select side arm type",
                        enable = true,
                        Id = side.sideArmTypeId,
                        Key = "Select side arm type",
                        Label = "Select side arm type",
                        Manage = false,
                        Required = false,
                        Value = side.sideArmType.Name
                    });
                    xxx.Add(new BaseInstAttView
                    {
                        AutoFill = false,
                        DataType = "List",
                        DataTypeId = null,
                        Desc = "Select side arm installtion type",
                        enable = true,
                        Id = side.sideArmInstallationPlaceId != null ? side.sideArmInstallationPlaceId : 0,
                        Key = "Select side arm installtion type",
                        Label = "Select side arm installtion type",
                        Manage = false,
                        Required = false,
                        Value = side.sideArmInstallationPlace.Name
                    });
                }
                else if (AllCivilInst.civilNonSteelId != null)
                {
                    xxx.Add(new BaseInstAttView
                    {
                        AutoFill = false,
                        DataType = "List",
                        DataTypeId = null,
                        Desc = "",
                        enable = true,
                        Id = -1,
                        Key = "Select civil support type",
                        Label = "Select civil support type",
                        Manage = false,
                        Required = false,
                        Value = "Civil non steel"
                    });
                    xxx.Add(new BaseInstAttView
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
                    xxx.Add(new BaseInstAttView
                    {
                        AutoFill = false,
                        DataType = "List",
                        DataTypeId = null,
                        Desc = "Select side arm type",
                        enable = true,
                        Id = side.sideArmTypeId,
                        Key = "Select side arm type",
                        Label = "Select side arm type",
                        Manage = false,
                        Required = false,
                        Value = side.sideArmType.Name
                    });
                    xxx.Add(new BaseInstAttView
                    {
                        AutoFill = false,
                        DataType = "List",
                        DataTypeId = null,
                        Desc = "Select side arm installtion type",
                        enable = true,
                        Id = side.sideArmInstallationPlaceId != null ? side.sideArmInstallationPlaceId : 0,
                        Key = "Select side arm installtion type",
                        Label = "Select side arm installtion type",
                        Manage = false,
                        Required = false,
                        Value = side.sideArmInstallationPlace.Name
                    });
                }
                objectInst.SideArmInstallationInfo = xxx;
                return new Response<ObjectInstAttsForSideArm>(true, objectInst, null, null, (int)ApiReturnCode.success);
            }
            catch (Exception err)
            {
                return new Response<ObjectInstAttsForSideArm>(true, null, null, err.Message, (int)ApiReturnCode.fail);
            }
        }

        public Response<GetEnableAttribute> GetSideArmInstallationWithEnableAtt(string SiteCode, string ConnectionString)
        {
            using (var connection = new OracleConnection(ConnectionString))
            {
                try
                {
                    GetEnableAttribute getEnableAttribute = new GetEnableAttribute();
                    connection.Open();
                    string storedProcedureName = "CREATE_DYNAMIC_PIVOT_SIDEARM ";
                    using (OracleCommand procedureCommand = new OracleCommand(storedProcedureName, connection))
                    {
                        procedureCommand.CommandType = CommandType.StoredProcedure;
                        procedureCommand.ExecuteNonQuery();
                    }
                    var attActivated = _dbContext.TLIattributeViewManagment.Include(x => x.EditableManagmentView).Include(x => x.AttributeActivated)
                        .Include(x => x.DynamicAtt).Where(x => x.Enable && x.EditableManagmentView.View == "SideArmInstallation" &&
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
                    propertyNamesStatic.Add("CIVILNAME");
                    propertyNamesStatic.Add("CIVILID");
                    propertyNamesStatic.Add("FIRST_LEG");
                    propertyNamesStatic.Add("FIRST_LEG_ID");
                    propertyNamesStatic.Add("SECOND_LEG");
                    propertyNamesStatic.Add("SECOND_LEG_ID");
             

                    if (propertyNamesDynamic.Count == 0)
                    {
                    
                        var query = _dbContext.SIDEARM_VIEW.Where(x => x.SITECODE.ToLower() == SiteCode.ToLower()).AsEnumerable()
                        .Select(item => _unitOfWork.CivilWithLegsRepository.BuildDynamicSelect(item, null, propertyNamesStatic, propertyNamesDynamic));
                        int count = query.Count();
                        getEnableAttribute.Model = query;
                        return new Response<GetEnableAttribute>(true, getEnableAttribute, null, "Success", (int)Helpers.Constants.ApiReturnCode.success, count);
                    }
                    else
                    {
                        var query = _dbContext.SIDEARM_VIEW.Where(x => x.SITECODE.ToLower() == SiteCode.ToLower()).AsEnumerable()
                       .GroupBy(x => new
                       {
                           SITECODE = x.SITECODE,
                           Id = x.Id,
                           Name = x.Name,
                           CIVILNAME = x.CIVILNAME,
                           CIVILID = x.CIVILID,
                           FIRST_LEG = x.FIRST_LEG,
                           SECOND_LEG = x.SECOND_LEG,
                           Notes = x.Notes,
                           HeightBase = x.HeightBase,
                           Azimuth = x.Azimuth,
                           ReservedSpace = x.ReservedSpace,
                           Active = x.Active,
                           VisibleStatus = x.VisibleStatus,
                           SpaceInstallation = x.SpaceInstallation,
                           SIDEARMLIBRARY = x.SIDEARMLIBRARY,
                           SIDEARMINSTALLATIONPLACE = x.SIDEARMINSTALLATIONPLACE,
                           OWNER = x.OWNER,
                           SIDEARMTYPE = x.SIDEARMTYPE,
                           Draft = x.Draft,
                           CenterHigh = x.CenterHigh,
                           HBA = x.HBA,
                           HieghFromLand = x.HieghFromLand,
                           EquivalentSpace = x.EquivalentSpace,
                           FIRST_LEG_ID=x.FIRST_LEG_ID,
                           SECOND_LEG_ID=x.SECOND_LEG_ID,
                        

                       })
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

        #region Helper Methods...
        public List<TLIcivilLoads> GetMaxInstallationDate(List<TLIcivilLoads> Copy)
        {
            var y = new List<TLIcivilLoads>();
            foreach (var item in Copy)
            {
                var CheckIfExist = y.FirstOrDefault(x => x.sideArmId == item.sideArmId);
                if (CheckIfExist != null)
                {
                    if (CheckIfExist.InstallationDate < item.InstallationDate)
                    {
                        y.Remove(CheckIfExist);
                        y.Add(item);
                    }
                }
                else
                    y.Add(item);
            }
            return y;
        }
        #region Helper Methods ...
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
        #endregion
        public List<TLIcivilLoads> GetCivilLoadsWithConditions(CivilLoadsFilter BaseFilter, CombineFilters CombineFilters)
        {
            List<FilterObjectList> ObjectAttributeFilters = CombineFilters.filters;
            List<DateFilterViewModel> DateFilter = CombineFilters.DateFilter;

            List<StringFilterObjectList> AttributeFilters = new List<StringFilterObjectList>();
            List<AttributeActivatedViewModel> SideArmInstallationAttribute = new List<AttributeActivatedViewModel>();

            if ((DateFilter != null ? DateFilter.Count() > 0 : false) ||
                (ObjectAttributeFilters != null && ObjectAttributeFilters.Count > 0))
            {
                SideArmInstallationAttribute = _mapper.Map<List<AttributeActivatedViewModel>>(_unitOfWork.AttributeViewManagmentRepository.GetIncludeWhere(x =>
                    x.Enable && x.AttributeActivatedId != null &&
                    x.EditableManagmentView.View == Helpers.Constants.EditableManamgmantViewNames.SideArmInstallation.ToString() &&
                    x.EditableManagmentView.TLItablesNames1.TableName == TablesNames.TLIsideArm.ToString(),
                        x => x.AttributeActivated, x => x.EditableManagmentView, x => x.EditableManagmentView.TLItablesNames1,
                        x => x.EditableManagmentView.TLItablesNames2)
                .Select(x => x.AttributeActivated).ToList());
            }

            if (ObjectAttributeFilters != null && ObjectAttributeFilters.Count > 0)
            {
                List<AttributeActivatedViewModel> NotDateDateSideArmInstallationAttribute = SideArmInstallationAttribute.Where(x =>
                    x.DataType.ToLower() != "datetime").ToList();

                foreach (FilterObjectList item in ObjectAttributeFilters)
                {
                    List<string> value = item.value.Select(x => x.ToString().ToLower()).ToList();

                    AttributeActivatedViewModel AttributeKey = NotDateDateSideArmInstallationAttribute.FirstOrDefault(x =>
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
                List<AttributeActivatedViewModel> DateSideArmInstallationAttribute = SideArmInstallationAttribute.Where(x =>
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

                    AttributeActivatedViewModel AttributeKey = DateSideArmInstallationAttribute.FirstOrDefault(x =>
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

            List<int> SideArmsIds = new List<int>();
            List<int> WithoutDateFilterSideArmInstallation = new List<int>();
            List<int> WithDateFilterSideArmInstallation = new List<int>();

            if (AttributeFilters != null && AttributeFilters.Count > 0)
            {
                //
                // Installation Dynamic Attributes...
                //
                List<TLIdynamicAtt> InstDynamicAttListIds = _unitOfWork.DynamicAttRepository.GetIncludeWhere(x =>
                    AttributeFilters.AsEnumerable().Select(y => y.key.ToLower()).Contains(x.Key.ToLower()) &&
                    !x.LibraryAtt && !x.disable &&
                    x.tablesNames.TableName == TablesNames.TLIsideArm.ToString()
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
                bool AttrInstExist = typeof(SideArmViewModel).GetProperties().ToList().Exists(x =>
                    AttributeFilters.Where(y => y.key.ToLower() != "Id".ToLower()).Select(y =>
                        y.key.ToLower()).Contains(x.Name.ToLower()));

                List<int> InstallationAttributeActivated = new List<int>();
                if (AttrInstExist)
                {
                    List<PropertyInfo> NotStringProps = typeof(SideArmViewModel).GetProperties().Where(x =>
                        x.PropertyType.Name.ToLower() != "string" &&
                            AttributeFilters.Select(y =>
                                y.key.ToLower()).Contains(x.Name.ToLower())).ToList();

                    List<PropertyInfo> StringProps = typeof(SideArmViewModel).GetProperties().Where(x =>
                        x.PropertyType.Name.ToLower() == "string" &&
                            AttributeFilters.Select(y =>
                                y.key.ToLower()).Contains(x.Name.ToLower())).ToList();

                    List<StringFilterObjectList> AttrInstAttributeFilters = AttributeFilters.Where(x =>
                        NotStringProps.Select(y => y.Name.ToLower()).Contains(x.key.ToLower()) ||
                        StringProps.Select(y => y.Name.ToLower()).Contains(x.key.ToLower())).ToList();

                    IEnumerable<TLIsideArm> Installations = _unitOfWork.SideArmRepository.GetAllWithoutCount();

                    foreach (StringFilterObjectList InstallationProp in AttrInstAttributeFilters)
                    {
                        if (StringProps.Select(x => x.Name.ToLower()).Contains(InstallationProp.key.ToLower()))
                        {
                            Installations = Installations.Where(x => StringProps.AsEnumerable().FirstOrDefault(y => (InstallationProp.key.ToLower() == y.Name.ToLower()) && (InstallationProp.value.AsEnumerable().FirstOrDefault(w =>
                                 y.GetValue(_mapper.Map<SideArmViewModel>(x), null) != null ? y.GetValue(_mapper.Map<SideArmViewModel>(x), null).ToString().ToLower().StartsWith(w.ToLower()) : false) != null)) != null).AsEnumerable();
                        }
                        else if (NotStringProps.Select(x => x.Name.ToLower()).Contains(InstallationProp.key.ToLower()))
                        {
                            Installations = Installations.Where(x => NotStringProps.AsEnumerable().FirstOrDefault(y => (InstallationProp.key.ToLower() == y.Name.ToLower()) && (y.GetValue(_mapper.Map<SideArmViewModel>(x), null) != null ?
                                InstallationProp.value.AsEnumerable().Contains(y.GetValue(_mapper.Map<SideArmViewModel>(x), null).ToString().ToLower()) : false)) != null).AsEnumerable();
                        }
                    }

                    InstallationAttributeActivated = Installations.Select(x => x.Id).ToList();
                }

                //
                // Installation (Attribute Activated + Dynamic) Attributes...
                //
                List<int> IntersectInstallationIds = new List<int>();
                if (AttrInstExist && DynamicInstExist)
                {
                    WithoutDateFilterSideArmInstallation = InstallationAttributeActivated.Intersect(DynamicInstValueListIds).ToList();
                }
                else if (AttrInstExist)
                {
                    WithoutDateFilterSideArmInstallation = InstallationAttributeActivated;
                }
                else if (DynamicInstExist)
                {
                    WithoutDateFilterSideArmInstallation = DynamicInstValueListIds;
                }
            }

            if (DateFilter != null ? DateFilter.Count() > 0 : false)
            {
                List<TLIdynamicAtt> DateTimeInstDynamicAttListIds = _unitOfWork.DynamicAttRepository.GetIncludeWhere(x =>
                    AfterConvertDateFilters.AsEnumerable().Select(y => y.key.ToLower()).Contains(x.Key.ToLower()) &&
                    !x.LibraryAtt && !x.disable &&
                    x.tablesNames.TableName == TablesNames.TLIsideArm.ToString()
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
                List<PropertyInfo> InstallationProps = typeof(SideArmViewModel).GetProperties().Where(x =>
                    AfterConvertDateFilters.Select(y =>
                        y.key.ToLower()).Contains(x.Name.ToLower())).ToList();

                List<int> InstallationAttributeActivatedIds = new List<int>();
                bool AttrInstExist = false;

                if (InstallationProps != null)
                {
                    AttrInstExist = true;

                    List<DateFilterViewModel> InstallationPropsAttributeFilters = AfterConvertDateFilters.Where(x =>
                        InstallationProps.Select(y => y.Name.ToLower()).Contains(x.key.ToLower())).ToList();

                    IEnumerable<TLIsideArm> Installations = _unitOfWork.SideArmRepository.GetAllWithoutCount();

                    foreach (DateFilterViewModel InstallationProp in InstallationPropsAttributeFilters)
                    {
                        Installations = Installations.Where(x => InstallationProps.Exists(y => (InstallationProp.key.ToLower() == y.Name.ToLower()) && ((y.GetValue(_mapper.Map<SideArmViewModel>(x), null) != null) ?
                            ((InstallationProp.DateFrom.Date <= Convert.ToDateTime(y.GetValue(_mapper.Map<SideArmViewModel>(x), null)).Date) &&
                                (InstallationProp.DateTo.Date >= Convert.ToDateTime(y.GetValue(_mapper.Map<SideArmViewModel>(x), null)).Date)) : (false))));
                    }

                    InstallationAttributeActivatedIds = Installations.Select(x => x.Id).ToList();
                }

                //
                // Installation (Attribute Activated + Dynamic) Attributes...
                //
                if (AttrInstExist && DynamicInstExist)
                {
                    WithDateFilterSideArmInstallation = InstallationAttributeActivatedIds.Intersect(DynamicInstValueListIds).ToList();
                }
                else if (AttrInstExist)
                {
                    WithDateFilterSideArmInstallation = InstallationAttributeActivatedIds;
                }
                else if (DynamicInstExist)
                {
                    WithDateFilterSideArmInstallation = DynamicInstValueListIds;
                }
            }

            if ((DateFilter != null ? DateFilter.Count() > 0 : false) ||
                (AttributeFilters != null ? AttributeFilters.Count() > 0 : false))
            {
                if ((AttributeFilters != null ? AttributeFilters.Count > 0 : false) &&
                        (AfterConvertDateFilters != null ? AfterConvertDateFilters.Count() > 0 : false))
                {
                    SideArmsIds = WithoutDateFilterSideArmInstallation.Intersect(WithDateFilterSideArmInstallation).ToList();
                }
                else if (AttributeFilters != null ? AttributeFilters.Count > 0 : false)
                {
                    SideArmsIds = WithoutDateFilterSideArmInstallation;
                }
                else if (AfterConvertDateFilters != null ? AfterConvertDateFilters.Count() > 0 : false)
                {
                    SideArmsIds = WithDateFilterSideArmInstallation;
                }

                return _unitOfWork.CivilLoadsRepository.GetIncludeWhere(x =>
                    (x.SiteCode == BaseFilter.SiteCode) &&
                    (!x.Dismantle) &&
                    (BaseFilter.ItemStatusId != null ? (
                        x.sideArm.ItemStatusId != null ? (
                            x.sideArm.ItemStatusId == BaseFilter.ItemStatusId)
                        : false)
                    : true) &&
                    (BaseFilter.TicketId != null ? (
                        x.sideArm.TicketId != null ? (
                            x.sideArm.TicketId == BaseFilter.TicketId)
                        : false)
                    : true) &&
                    (BaseFilter.CivilId != null ? (
                        x.allCivilInstId == BaseFilter.CivilId)
                    : true) &&
                    (x.sideArmId != null) &&
                    (SideArmsIds.Contains(x.sideArmId.Value)),
                  x => x.sideArm, x => x.sideArm.ItemStatus, x => x.sideArm.owner, x => x.sideArm.sideArmInstallationPlace, x => x.sideArm.sideArmLibrary,
                  x => x.sideArm.sideArmType, x => x.sideArm.Ticket).ToList();
            }

            return _unitOfWork.CivilLoadsRepository.GetIncludeWhere(x =>
                (x.SiteCode == BaseFilter.SiteCode) &&
                (!x.Dismantle) &&
                (BaseFilter.ItemStatusId != null ? (
                    x.sideArm.ItemStatusId != null ? (
                        x.sideArm.ItemStatusId == BaseFilter.ItemStatusId)
                    : false)
                : true) &&
                (BaseFilter.TicketId != null ? (
                    x.sideArm.TicketId != null ? (
                        x.sideArm.TicketId == BaseFilter.TicketId)
                    : false)
                : true) &&
                (BaseFilter.CivilId != null ? (
                    x.allCivilInstId == BaseFilter.CivilId)
                : true) &&
                (x.sideArmId != null),
                x => x.sideArm, x => x.sideArm.ItemStatus, x => x.sideArm.owner, x => x.sideArm.sideArmInstallationPlace, x => x.sideArm.sideArmLibrary,
                x => x.sideArm.sideArmType, x => x.sideArm.Ticket).ToList();
        }
        public DynamicAttDto GetDynamicAttDto(TLIdynamicAttInstValue DynamicAttInstValueRecord, TLIdynamicAttLibValue DynamicAttLibRecord)
        {
            var CivilDynamicAttDto = new DynamicAttDto
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
                CivilDynamicAttDto.DynamicAttLibValue.Value = GetDynamicAttValue(DynamicAttInstValueRecord, null);

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
        //Function take 1 parameter
        //get record by Id
        //get activated attributes with values
        //get table name Entity by table name
        //get dynamic attributes
        public Response<GetForAddLoadObject> GetById(int Id)
        {
            try
            {
              
                GetForAddLoadObject attributes = new GetForAddLoadObject();
                List<BaseInstAttViews> Config = new List<BaseInstAttViews>();
                List<BaseInstAttViews> Civilloads = new List<BaseInstAttViews>();
                TLIcivilLoads sideArm = _unitOfWork.CivilLoadsRepository
                    .GetIncludeWhereFirst(x => x.sideArmId ==Id && !x.Dismantle , 
                    x => x.sideArm.owner, x => x.sideArm.sideArmType, x => x.sideArm.sideArmInstallationPlace,
                    x => x.sideArm.sideArmInstallationPlace,x=>x.sideArm.sideArmLibrary);
                if (sideArm != null)
                {
                    EditCivilSideArmlLibraryAttributes SideArmLibrary = _mapper.Map<EditCivilSideArmlLibraryAttributes>(sideArm.sideArm.sideArmLibrary);
                    List<BaseInstAttViews> LibraryAttributes = _unitOfWork.AttributeActivatedRepository
                         .GetAttributeActivatedGetLibrary(Helpers.Constants.TablesNames.TLIsideArmLibrary.ToString(), SideArmLibrary, null).ToList();

                    List<BaseInstAttViews> LogisticalAttributes = _mapper.Map<List<BaseInstAttViews>>(_unitOfWork.LogistcalRepository
                            .GetLogisticalsNonSteel(Helpers.Constants.TablePartName.SideArm.ToString(), Helpers.Constants.TablesNames.TLIsideArmLibrary.ToString(), SideArmLibrary.Id).ToList());

                    LibraryAttributes.AddRange(LogisticalAttributes);

                    attributes.LibraryAttribute = LibraryAttributes;
                    attributes.InstallationAttributes = _unitOfWork.AttributeActivatedRepository.GetInstAttributeActivatedGetForAdd(TablesNames.TLIsideArm.ToString(), sideArm.sideArm, null, "sideArmLibraryId", "ItemStatusId", "TicketId");
                    var foreignKeyAttributes = attributes.InstallationAttributes.Select(FKitem =>
                    {
                        switch (FKitem.Label.ToLower())
                        {

                            case "owner_name":
                                FKitem.Value = _mapper.Map<OwnerViewModel>(sideArm.sideArm.owner);
                                FKitem.Options = _mapper.Map<List<OwnerViewModel>>(_unitOfWork.OwnerRepository.GetWhere(x => !x.Deleted && !x.Disable).ToList());
                                break;
                        }
                        return FKitem;
                    }).ToList();

                    BaseInstAttViews NameAttribute = attributes.InstallationAttributes.FirstOrDefault(x => x.Key.ToLower() == "Name".ToLower());
                    if (NameAttribute != null)
                    {
                        NameAttribute.Value = _dbContext.SIDEARM_VIEW.FirstOrDefault(x => x.Id == Id)?.Name;
                    }
                    var selectedAttributes = attributes.InstallationAttributes
                   .Where(x => new[] { "sidearminstallationplace_name", "sidearmtype_name" }
                                .Contains(x.Label.ToLower()))
                   .ToList();

                 
                    var foreignKeyAttribute = selectedAttributes.Select(FKitem =>
                    {
                        switch (FKitem.Label.ToLower())
                        {
                            case "sidearminstallationplace_name":
                                FKitem.Value = _mapper.Map<SectionsLegTypeViewModel>(sideArm?.sideArm?.sideArmInstallationPlace);
                                FKitem.Options = _mapper.Map<List<SectionsLegTypeViewModel>>(_unitOfWork.SideArmInstallationPlaceRepository
                                    .GetWhere(x=>x.Id== sideArm.sideArm.sideArmInstallationPlaceId));
                                break;

                            case "sidearmtype_name":
                                FKitem.Value = _mapper.Map<SectionsLegTypeViewModel>(sideArm?.sideArm?.sideArmType);
                                FKitem.Options = _mapper.Map<List<SectionsLegTypeViewModel>>(_unitOfWork.SideArmTypeRepository
                                    .GetWhere(x => x.Id == sideArm.sideArm.sideArmTypeId));
                                break;
                        }
                        return FKitem;
                    }).ToList();


                    Config.AddRange(foreignKeyAttribute);

                    var CivilLoad = _unitOfWork.CivilLoadsRepository.GetWhereFirst(x => x.sideArmId == Id && !x.Dismantle);
                    if (CivilLoad != null)
                    {
                        var AllCivilInst = _unitOfWork.AllCivilInstRepository.GetIncludeWhereFirst(x => x.Id == CivilLoad.allCivilInstId, x => x.civilNonSteel
                        , x => x.civilWithLegs, x => x.civilWithoutLeg);
                        if (AllCivilInst != null)
                        {
                            BaseInstAttViews baseInstAttViews = new BaseInstAttViews();
                            if (AllCivilInst.civilWithoutLegId != null)
                            {
                                baseInstAttViews.Key = "civilWithoutLegId";
                                baseInstAttViews.Label = "civilWithoutLeg_name";
                                baseInstAttViews.Value = _mapper.Map<SectionsLegTypeViewModel>(AllCivilInst.civilWithoutLeg);
                                baseInstAttViews.Options = _mapper.Map<List<SectionsLegTypeViewModel>>(_unitOfWork.CivilWithoutLegRepository
                                     .GetWhere(x => x.Id == AllCivilInst.civilWithoutLegId));
                                baseInstAttViews.DataType = "List";
                                Config.Add(baseInstAttViews);
                            }
                            else if (AllCivilInst.civilNonSteelId != null)
                            {
                                baseInstAttViews.Key = "civilNonSteelId";
                                baseInstAttViews.Label = "civilNonSteel_name";
                                baseInstAttViews.Value = _mapper.Map<SectionsLegTypeViewModel>(AllCivilInst.civilNonSteel);
                                baseInstAttViews.Options = _mapper.Map<List<SectionsLegTypeViewModel>>(_unitOfWork.CivilNonSteelRepository
                                       .GetWhere(x => x.Id == AllCivilInst.civilNonSteelId));
                                baseInstAttViews.DataType = "List";
                                Config.Add(baseInstAttViews);
                            }
                            if (AllCivilInst.civilWithLegsId != null)
                            {
                                baseInstAttViews.Key = "civilWithLegId";
                                baseInstAttViews.Label = "civilWithLeg_name";
                                baseInstAttViews.Value = _mapper.Map<SectionsLegTypeViewModel>(AllCivilInst.civilWithLegs);
                                baseInstAttViews.Options = _mapper.Map<List<SectionsLegTypeViewModel>>(_unitOfWork.CivilWithLegsRepository
                                    .GetWhere(x => x.Id == AllCivilInst.civilWithLegsId));
                                baseInstAttViews.DataType = "List";
                                Config.Add(baseInstAttViews);
                            }
                        }
                        if ((CivilLoad.legId != 0 && CivilLoad.legId != null) || (CivilLoad.Leg2Id != 0 && CivilLoad.Leg2Id != null))
                        {
                        
                            var Leg1 = _unitOfWork.LegRepository.GetWhereFirst(x => x.Id == CivilLoad.legId);
                            if (Leg1 != null)
                            {
                                BaseInstAttViews baseInstAttViews = new BaseInstAttViews();
                                baseInstAttViews.Key = "legId";
                                baseInstAttViews.Value = _mapper.Map<SectionsLegTypeViewModel>(Leg1);
                                baseInstAttViews.Label = "leg_name";
                                baseInstAttViews.Options = _mapper.Map<List<SectionsLegTypeViewModel>>(_unitOfWork.LegRepository
                                   .GetWhere(x => x.Id == CivilLoad.legId));
                                baseInstAttViews.DataType = "list";
                                Config.Add(baseInstAttViews);
                            }
                            var Leg2 = _unitOfWork.LegRepository.GetWhereFirst(x => x.Id == CivilLoad.Leg2Id);
                            if (Leg2 != null)
                            {
                                BaseInstAttViews baseInstAttViews = new BaseInstAttViews();
                                baseInstAttViews.Key = "leg2Id";
                                baseInstAttViews.Label = "leg_name";
                                baseInstAttViews.Value = _mapper.Map<SectionsLegTypeViewModel>(Leg2);
                                baseInstAttViews.Options = _mapper.Map<List<SectionsLegTypeViewModel>>(_unitOfWork.LegRepository
                               .GetWhere(x => x.Id == CivilLoad.Leg2Id));
                                baseInstAttViews.DataType = "list";
                                Config.Add(baseInstAttViews);
                            }
                        }
                        attributes.installationConfig = Config;
                        var InstallationDate = new BaseInstAttViews()
                        {
                            Key = "InstallationDate",
                            Value = CivilLoad.InstallationDate,
                            DataType = "datetime",
                            Label = "InstallationDate",


                        };
                        Civilloads.Add(InstallationDate);
                        var ItemOnCivilStatus = new BaseInstAttViews()
                        {
                            Key = "ItemOnCivilStatus",
                            Value = CivilLoad.ItemOnCivilStatus,
                            DataType = "string",
                            Label = "ItemOnCivilStatus",


                        };
                        Civilloads.Add(ItemOnCivilStatus);
                        var ItemStatus = new BaseInstAttViews()
                        {
                            Key = "ItemStatus",
                            Value = CivilLoad.InstallationDate,
                            DataType = "string",
                            Label = "ItemStatus",


                        };
                        Civilloads.Add(ItemStatus);
                        var ReservedSpace = new BaseInstAttViews()
                        {
                            Key = "ReservedSpace",
                            Value = CivilLoad.ReservedSpace,
                            DataType = "bool",
                            Label = "ReservedSpace",

                        };
                        Civilloads.Add(ReservedSpace);
                       
                    }
                    attributes.CivilLoads = Civilloads;
                    attributes.InstallationAttributes = attributes.InstallationAttributes.Except(selectedAttributes).ToList();
                    var TableNameEntity = _unitOfWork.TablesNamesRepository.GetWhereFirst(X => X.TableName == TablesNames.TLIsideArm.ToString());

                    attributes.dynamicAttribute = null;
                    attributes.dynamicAttribute = _mapper.Map<List<DynaminAttInstViewModel>>(_unitOfWork.DynamicAttRepository.GetWhere(x => x.tablesNamesId == TableNameEntity.Id && x.LibraryAtt == false).ToList());


                    return new Response<GetForAddLoadObject>(true, attributes, null, null, (int)ApiReturnCode.success);

                }
                else
                {
                    return new Response<GetForAddLoadObject>(false, null, null, "this id is not found", (int)ApiReturnCode.fail);
                }
            }

            catch (Exception err)
            {

                return new Response<GetForAddLoadObject>(false, null, null, err.Message, (int)ApiReturnCode.fail);
            }

        }
        //Function take 1 parameter
        //get table name Entity by table name
        //map ViewModel to Entity
        //get record by Id
        //check business rules
        //add Entity
        //add dynamic attributes
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
        public string CheckDependencyValidationForSideArm(object Input, string SiteCode)
        {
            string MainTableName = TablesNames.TLIsideArm.ToString();
            AddSideArmViewModel AddInstallationViewModel = _mapper.Map<AddSideArmViewModel>(Input);

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
       
        //Function take 1 parameter
        //map ViewModel to Entity
        //update Entity
        public async Task<Response<EditSidearmInstallationObject>> UpdateSideArm(EditSidearmInstallationObject SideArmViewModel, int? TaskId,int UserId)
        {
            using (TransactionScope transactionScope = new TransactionScope())
            {
                try
                {
                    TLIcivilLoads CivilLoads = _unitOfWork.CivilLoadsRepository.GetIncludeWhereFirst(x => x.sideArmId == SideArmViewModel.installationAttributes.Id 
                    && !x.Dismantle,x=>x.allCivilInst,x=>x.allCivilInst.civilNonSteel,x=>x.allCivilInst.civilWithLegs
                    ,x=>x.allCivilInst.civilWithoutLeg);
                    if (CivilLoads != null)
                    {
                        TLIsideArm SideArm = _mapper.Map<TLIsideArm>(SideArmViewModel.installationAttributes);
                        TLIsideArm SideArmInst = _unitOfWork.SideArmRepository.GetAllAsQueryable().AsNoTracking().FirstOrDefault(x => x.Id == SideArm.Id);
                        
                        var SiteCode = CivilLoads.SiteCode ?? " ";
                        if (SideArmViewModel.installationConfig.civilSteelType == 0)
                        {
                            if (SideArmViewModel.installationConfig.civilWithLegId != 0 || SideArmViewModel.installationConfig.civilWithLegId != null)
                            {
                                if (SideArmViewModel.installationConfig.sideArmTypeId == 1)
                                {
                                    if (SideArmViewModel.installationConfig.installationPlaceId == 1)
                                    {
                                        if (SideArmViewModel.installationConfig.legId.Count == 1)
                                        {
                                            if (SideArm.Azimuth <= 0)
                                            {
                                                return new Response<EditSidearmInstallationObject>(false, null, null,
                                                    "Azimuth must bigger from zero", (int)ApiReturnCode.fail);

                                            }
                                            if (SideArm.HeightBase <= 0)
                                            {
                                                return new Response<EditSidearmInstallationObject>(false, null, null,
                                                    "HeightBase must bigger from zero", (int)ApiReturnCode.fail);
                                            }
                                            var AzimuthandAndHeightBase = _dbContext.SIDEARM_VIEW.Where(x => x.CIVILID == SideArmViewModel.installationConfig.civilWithLegId.ToString()
                                            && x.SITECODE.ToLower() == SiteCode.ToLower() && x.SIDEARMTYPE.ToLower() == "normal" && x.FIRST_LEG_ID == SideArmViewModel.installationConfig.legId[0]
                                            && x.Azimuth == SideArmViewModel.installationAttributes.Azimuth && x.HeightBase == SideArmViewModel.installationAttributes.HeightBase && x.Id !=SideArm.Id).ToList();
                                            if (AzimuthandAndHeightBase != null || AzimuthandAndHeightBase.Count() > 0)
                                            {
                                                return new Response<EditSidearmInstallationObject>(false, null, null, "can not installed this sidearm on azimuth and heightbase selected because found other sidearm in same azimuth and heightbase", (int)ApiReturnCode.fail);
                                            }
                                            var civilwithlegname = _unitOfWork.CivilSiteDateRepository.GetIncludeWhereFirst(x => x.allCivilInst.civilWithLegsId == SideArmViewModel.installationConfig.civilWithLegId && x.SiteCode.ToLower() == SiteCode.ToLower() && !x.Dismantle
                                            , x => x.allCivilInst, x => x.allCivilInst.civilWithLegs
                                              , x => x.allCivilInst.civilWithoutLeg, x => x.allCivilInst.civilNonSteel);
                                            if (civilwithlegname != null)
                                            {
                                                var LegName = _unitOfWork.LegRepository.GetWhereFirst(x => x.Id == SideArmViewModel.installationConfig.legId[0])?.CiviLegName;
                                                if (civilwithlegname != null && LegName != null)
                                                {
                                                    SideArm.Name = civilwithlegname.allCivilInst.civilWithLegs.Name + LegName + SideArmViewModel.installationAttributes.HeightBase + SideArmViewModel.installationAttributes.Azimuth;

                                                }

                                                var CheckName = _dbContext.SIDEARM_VIEW.FirstOrDefault(x => !x.Dismantle &&
                                                     (x.Id != null ? x.Name.ToLower() == SideArm.Name.ToLower() : false
                                                        && x.SITECODE.ToLower() == SiteCode.ToLower()));

                                                if (CheckName != null)
                                                    return new Response<EditSidearmInstallationObject>(false, null, null, $"The name {SideArm.Name} is already exists", (int)Helpers.Constants.ApiReturnCode.fail);

                                                //string CheckDependencyValidation = CheckDependencyValidationForSideArm(SideArmViewModel, SiteCode);

                                                //if (!string.IsNullOrEmpty(CheckDependencyValidation))
                                                //    return new Response<AllItemAttributes>(true, null, null, CheckDependencyValidation, (int)ApiReturnCode.fail);

                                                //string CheckGeneralValidation = CheckGeneralValidationFunction(SideArmViewModel.dynamicAttribute, TableNameEntity.TableName);

                                                //if (!string.IsNullOrEmpty(CheckGeneralValidation))
                                                //    return new Response<AllItemAttributes>(true, null, null, CheckGeneralValidation, (int)ApiReturnCode.fail);

                                               
                                            }
                                            else
                                            {
                                                return new Response<EditSidearmInstallationObject>(false, null, null,
                                                                                          " this civil in not found  ", (int)ApiReturnCode.fail);
                                            }


                                        }
                                        else
                                        {
                                            return new Response<EditSidearmInstallationObject>(false, null, null,
                                            "must selected one leg  ", (int)ApiReturnCode.fail);
                                        }
                                    }
                                    else
                                    {
                                        return new Response<EditSidearmInstallationObject>(false, null, null,
                                                "installation place must be leg  ", (int)ApiReturnCode.fail);
                                    }
                                }
                                else if (SideArmViewModel.installationConfig.sideArmTypeId == 2)
                                {
                                    if (SideArmViewModel.installationConfig.installationPlaceId == 1 || SideArmViewModel.installationConfig.installationPlaceId == 2)
                                    {
                                        if (SideArmViewModel.installationConfig.legId.Count == 2)
                                        {
                                            if (SideArm.Azimuth <= 0)
                                            {
                                                return new Response<EditSidearmInstallationObject>(false, null, null,
                                                    "Azimuth must bigger from zero", (int)ApiReturnCode.fail);

                                            }
                                            if (SideArm.HeightBase <= 0)
                                            {
                                                return new Response<EditSidearmInstallationObject>(false, null, null,
                                                    "HeightBase must bigger from zero", (int)ApiReturnCode.fail);
                                            }
                                            var AzimuthandAndHeightBase = _dbContext.SIDEARM_VIEW.Where(x => x.CIVILID == SideArmViewModel.installationConfig.civilWithLegId.ToString()
                                            && x.SITECODE.ToLower() == SiteCode.ToLower() && x.SIDEARMTYPE.ToLower() == "normal" && x.FIRST_LEG_ID == SideArmViewModel.installationConfig.legId[0]
                                            && x.Azimuth == SideArmViewModel.installationAttributes.Azimuth && x.HeightBase == SideArmViewModel.installationAttributes.HeightBase && x.Id != SideArm.Id).ToList();
                                            if (AzimuthandAndHeightBase != null || AzimuthandAndHeightBase.Count() > 0)
                                            {
                                                return new Response<EditSidearmInstallationObject>(false, null, null, "can not installed this sidearm on azimuth and heightbase selected because found other sidearm in same azimuth and heightbase", (int)ApiReturnCode.fail);
                                            }
                                            var civilwithlegname = _unitOfWork.CivilSiteDateRepository.GetIncludeWhereFirst(x => x.allCivilInst.civilWithLegsId
                                            == SideArmViewModel.installationConfig.civilWithLegId && x.SiteCode.ToLower() == SiteCode.ToLower() && !x.Dismantle, x => x.allCivilInst, x => x.allCivilInst.civilWithLegs
                                            , x => x.allCivilInst.civilWithoutLeg, x => x.allCivilInst.civilNonSteel);
                                            if (civilwithlegname != null)
                                            {
                                                var LegName = _unitOfWork.LegRepository.GetWhereFirst(x => x.Id == SideArmViewModel.installationConfig.legId[0])?.CiviLegName;
                                                var LegName2 = _unitOfWork.LegRepository.GetWhereFirst(x => x.Id == SideArmViewModel.installationConfig.legId[1])?.CiviLegName;
                                                if (LegName != null && LegName2 != null && civilwithlegname != null)
                                                {
                                                    SideArm.Name = civilwithlegname.allCivilInst.civilWithLegs.Name + LegName + LegName2 + SideArmViewModel.installationAttributes.HeightBase + SideArmViewModel.installationAttributes.Azimuth;

                                                }
                                                var CheckName = _dbContext.SIDEARM_VIEW.FirstOrDefault(x => !x.Dismantle &&
                                                             (x.Id != null ? x.Name.ToLower() == SideArm.Name.ToLower() : false
                                                                && x.SITECODE.ToLower() == SiteCode.ToLower()));

                                                if (CheckName != null)
                                                    return new Response<EditSidearmInstallationObject>(false, null, null, $"The name {SideArm.Name} is already exists", (int)Helpers.Constants.ApiReturnCode.fail);

                                                //string CheckDependencyValidation = CheckDependencyValidationForSideArm(SideArmViewModel, SiteCode);

                                                //if (!string.IsNullOrEmpty(CheckDependencyValidation))
                                                //    return new Response<AllItemAttributes>(true, null, null, CheckDependencyValidation, (int)ApiReturnCode.fail);

                                                //string CheckGeneralValidation = CheckGeneralValidationFunction(SideArmViewModel.dynamicAttribute, TableNameEntity.TableName);

                                                //if (!string.IsNullOrEmpty(CheckGeneralValidation))
                                                //    return new Response<AllItemAttributes>(true, null, null, CheckGeneralValidation, (int)ApiReturnCode.fail);
                                                
                                            }
                                            else
                                            {
                                                return new Response<EditSidearmInstallationObject>(false, null, null,
                                                                                          " this civil in not found  ", (int)ApiReturnCode.fail);
                                            }
                                        }
                                        else
                                        {
                                            return new Response<EditSidearmInstallationObject>(false, null, null,
                                            "must selected tow legs  ", (int)ApiReturnCode.fail);
                                        }

                                    }
                                    else
                                    {
                                        return new Response<EditSidearmInstallationObject>(false, null, null,
                                                "installation place must be leg or bracing ", (int)ApiReturnCode.fail);
                                    }
                                }

                            }
                            else
                            {
                                return new Response<EditSidearmInstallationObject>(false, null, null,
                               "must selected civilwithleg support item  ", (int)ApiReturnCode.fail);
                            }
                        }
                        else if (SideArmViewModel.installationConfig.civilSteelType == 1)
                        {
                            if (SideArmViewModel.installationConfig.civilWithoutLegId != 0 || SideArmViewModel.installationConfig.civilWithoutLegId != null)
                            {
                                if (SideArmViewModel.installationConfig.sideArmTypeId == 1)
                                {

                                    if (SideArmViewModel.installationConfig.installationPlaceId == 3 || SideArmViewModel.installationConfig.installationPlaceId == 4 ||
                                        SideArmViewModel.installationConfig.installationPlaceId == 5)
                                    {
                                        if (SideArm.Azimuth <= 0)
                                        {
                                            return new Response<EditSidearmInstallationObject>(false, null, null,
                                                "Azimuth must bigger from zero", (int)ApiReturnCode.fail);

                                        }
                                        if (SideArm.HeightBase <= 0)
                                        {
                                            return new Response<EditSidearmInstallationObject>(false, null, null,
                                                "HeightBase must bigger from zero", (int)ApiReturnCode.fail);
                                        }
                                        var AzimuthandAndHeightBase = _dbContext.SIDEARM_VIEW.Where(x => x.CIVILID == SideArmViewModel.installationConfig.civilWithLegId.ToString()
                                             && x.SITECODE.ToLower() == SiteCode.ToLower() && x.SIDEARMTYPE.ToLower() == "normal" && x.FIRST_LEG_ID == SideArmViewModel.installationConfig.legId[0]
                                             && x.Azimuth == SideArmViewModel.installationAttributes.Azimuth && x.HeightBase == SideArmViewModel.installationAttributes.HeightBase && x.Id != SideArm.Id).ToList();
                                        if (AzimuthandAndHeightBase != null || AzimuthandAndHeightBase.Count() > 0)
                                        {
                                            return new Response<EditSidearmInstallationObject>(false, null, null, "can not installed this sidearm on azimuth and heightbase selected because found other sidearm in same azimuth and heightbase", (int)ApiReturnCode.fail);
                                        }
                                        var HeightBase = _dbContext.SIDEARM_VIEW.Where(x => x.CIVILID == SideArmViewModel.installationConfig.civilWithoutLegId.ToString()
                                        && x.SITECODE.ToLower() == SiteCode.ToLower() && x.SIDEARMTYPE.ToLower() == "normal"
                                        && x.Azimuth == SideArmViewModel.installationAttributes.HeightBase).ToList();
                                        if (HeightBase != null || HeightBase.Count() > 0)
                                        {
                                            return new Response<EditSidearmInstallationObject>(false, null, null, "can not installed this sidearm on heightbase selected because found other sidearm in same heightbase", (int)ApiReturnCode.fail);
                                        }
                                        var civilwithlegname = _unitOfWork.CivilSiteDateRepository.GetIncludeWhereFirst(x => x.allCivilInst.civilWithoutLegId == SideArmViewModel.installationConfig.civilWithoutLegId && x.SiteCode.ToLower() == SiteCode.ToLower() && !x.Dismantle
                                        , x => x.allCivilInst, x => x.allCivilInst.civilWithLegs,
                                        x => x.allCivilInst.civilWithoutLeg, x => x.allCivilInst.civilNonSteel);
                                        if (civilwithlegname != null)
                                        {

                                            SideArm.Name = civilwithlegname.allCivilInst.civilWithoutLeg.Name + SideArmViewModel.installationAttributes.HeightBase + SideArmViewModel.installationAttributes.Azimuth;

                                            var CheckName = _dbContext.SIDEARM_VIEW.FirstOrDefault(x => !x.Dismantle &&
                                                    (x.Id != null ? x.Name.ToLower() == SideArm.Name.ToLower() : false
                                                       && x.SITECODE.ToLower() == SiteCode.ToLower()));

                                            if (CheckName != null)
                                                return new Response<EditSidearmInstallationObject>(false, null, null, $"The name {SideArm.Name} is already exists", (int)Helpers.Constants.ApiReturnCode.fail);

                                            //string CheckDependencyValidation = CheckDependencyValidationForSideArm(SideArmViewModel, SiteCode);

                                            //if (!string.IsNullOrEmpty(CheckDependencyValidation))
                                            //    return new Response<AllItemAttributes>(true, null, null, CheckDependencyValidation, (int)ApiReturnCode.fail);

                                            //string CheckGeneralValidation = CheckGeneralValidationFunction(SideArmViewModel.dynamicAttribute, TableNameEntity.TableName);

                                            //if (!string.IsNullOrEmpty(CheckGeneralValidation))
                                            //    return new Response<AllItemAttributes>(true, null, null, CheckGeneralValidation, (int)ApiReturnCode.fail);

                                           
                                        }
                                        else
                                        {
                                            return new Response<EditSidearmInstallationObject>(false, null, null,
                                                                                      " this civil in not found  ", (int)ApiReturnCode.fail);
                                        }
                                    }
                                    else
                                    {
                                        return new Response<EditSidearmInstallationObject>(false, null, null,
                                        "installation place must be SupportRole or Supportfence or AboveSupport", (int)ApiReturnCode.fail);
                                    }


                                }
                                else
                                {
                                    return new Response<EditSidearmInstallationObject>(false, null, null,
                                  "can not selected installation type other normal ", (int)ApiReturnCode.fail);

                                }
                            }
                            else
                            {
                                return new Response<EditSidearmInstallationObject>(false, null, null,
                               "must selected civilwithout support item  ", (int)ApiReturnCode.fail);
                            }
                        }
                        else if (SideArmViewModel.installationConfig.civilSteelType == 2)
                        {
                            if (SideArmViewModel.installationConfig.civilNonSteelId != 0 || SideArmViewModel.installationConfig.civilWithLegId != null)
                            {
                                if (SideArmViewModel.installationConfig.sideArmTypeId == 1)
                                {
                                    if (SideArmViewModel.installationConfig.installationPlaceId == 6)
                                    {
                                        if (SideArm.Azimuth <= 0)
                                        {
                                            return new Response<EditSidearmInstallationObject>(false, null, null,
                                                "Azimuth must bigger from zero", (int)ApiReturnCode.fail);

                                        }
                                        if (SideArm.HeightBase <= 0)
                                        {
                                            return new Response<EditSidearmInstallationObject>(false, null, null,
                                                "HeightBase must bigger from zero", (int)ApiReturnCode.fail);
                                        }
                                        var civilwithlegname = _unitOfWork.CivilSiteDateRepository.GetIncludeWhereFirst(x => x.allCivilInst.civilNonSteelId == SideArmViewModel.installationConfig.civilNonSteelId && x.SiteCode.ToLower() == SiteCode.ToLower() && !x.Dismantle
                                        , x => x.allCivilInst, x => x.allCivilInst.civilWithLegs,
                                        x => x.allCivilInst.civilWithoutLeg, x => x.allCivilInst.civilNonSteel);
                                        if (civilwithlegname != null)
                                        {
                                            SideArm.Name = civilwithlegname.allCivilInst.civilNonSteel.Name + SideArmViewModel.installationAttributes.HeightBase + SideArmViewModel.installationAttributes.Azimuth;

                                            var CheckName = _dbContext.SIDEARM_VIEW.FirstOrDefault(x => !x.Dismantle &&
                                                             (x.Id != null ? x.Name.ToLower() == SideArm.Name.ToLower() : false
                                                                && x.SITECODE.ToLower() == SiteCode.ToLower()));

                                            if (CheckName != null)
                                                return new Response<EditSidearmInstallationObject>(false, null, null, $"The name {SideArm.Name} is already exists", (int)Helpers.Constants.ApiReturnCode.fail);

                                            //string CheckDependencyValidation = CheckDependencyValidationForSideArm(SideArmViewModel, SiteCode);

                                            //if (!string.IsNullOrEmpty(CheckDependencyValidation))
                                            //    return new Response<AllItemAttributes>(true, null, null, CheckDependencyValidation, (int)ApiReturnCode.fail);

                                            //string CheckGeneralValidation = CheckGeneralValidationFunction(SideArmViewModel.dynamicAttribute, TableNameEntity.TableName);

                                            //if (!string.IsNullOrEmpty(CheckGeneralValidation))
                                            //    return new Response<AllItemAttributes>(true, null, null, CheckGeneralValidation, (int)ApiReturnCode.fail);

                                            
                                        }
                                        else
                                        {
                                            return new Response<EditSidearmInstallationObject>(false, null, null,
                                                                                      " this civil in not found  ", (int)ApiReturnCode.fail);
                                        }
                                    }
                                    else
                                    {
                                        return new Response<EditSidearmInstallationObject>(false, null, null,
                                        "installation place musst be wall sidearm", (int)ApiReturnCode.fail);

                                    }
                                }

                                else
                                {
                                    return new Response<EditSidearmInstallationObject>(false, null, null,
                                    "can not selected installation type other normal ", (int)ApiReturnCode.fail);

                                }
                            }

                            else
                            {
                                return new Response<EditSidearmInstallationObject>(false, null, null,
                                "must selected civilNonSteel support item  ", (int)ApiReturnCode.fail);
                            }
                        }
                        //string CheckGeneralValidation = CheckGeneralValidationFunctionEditVersion(SideArmViewModel.DynamicInstAttsValue, TablesNames.TLIsideArm.ToString());

                        //if (!string.IsNullOrEmpty(CheckGeneralValidation))
                        //    return new Response<AllItemAttributes>(true, null, null, CheckGeneralValidation, (int)ApiReturnCode.fail);

                        //string CheckDependencyValidation = CheckDependencyValidationEditVersion(SideArmViewModel, SiteCode);

                        //if (!string.IsNullOrEmpty(CheckDependencyValidation))
                        //    return new Response<AllItemAttributes>(true, null, null, CheckDependencyValidation, (int)ApiReturnCode.fail);
                        SideArm.sideArmInstallationPlaceId = SideArmInst.sideArmInstallationPlaceId;
                        SideArm.sideArmTypeId = SideArmInst.sideArmTypeId;
                        SideArm.sideArmLibraryId = SideArmViewModel.civilType.sideArmLibraryId;
                        _unitOfWork.SideArmRepository.UpdateWithHistory(UserId, SideArmInst, SideArm);
                        _unitOfWork.SaveChanges();

                        if(SideArmViewModel.CivilLoads != null)
                        {
                            CivilLoads.ItemOnCivilStatus = SideArmViewModel.CivilLoads?.ItemOnCivilStatus;
                            CivilLoads.InstallationDate = SideArmViewModel.CivilLoads.InstallationDate;
                            CivilLoads.ItemStatus = SideArmViewModel.CivilLoads?.ItemStatus;
                            CivilLoads.ReservedSpace = SideArmViewModel.CivilLoads.ReservedSpace;
                            CivilLoads.Dismantle =false;

                        }
                        int TableNameId = _unitOfWork.TablesNamesRepository.GetWhereFirst(x => x.TableName == TablesNames.TLIsideArm.ToString()).Id;
                        if (SideArmViewModel.dynamicAttribute != null ? SideArmViewModel.dynamicAttribute.Count() > 0 : false)
                            _unitOfWork.DynamicAttInstValueRepository.UpdateDynamicValues(UserId, SideArmViewModel.dynamicAttribute, TableNameId, SideArm.Id);
                        if (TaskId != null)
                        {
                            var Submit = _unitOfWork.SiteRepository.SubmitTaskByTLI(TaskId);
                            var result = Submit.Result;
                            if (result.result == true && result.errorMessage == null)
                            {
                                _unitOfWork.SaveChanges();
                                transactionScope.Complete();
                            }
                            else
                            {
                                transactionScope.Dispose();
                                return new Response<EditSidearmInstallationObject>(false, null, null, result.errorMessage.ToString(), (int)ApiReturnCode.fail);
                            }
                        }
                        else
                        {
                            _unitOfWork.SaveChanges();
                            transactionScope.Complete();
                        }
                        return new Response<EditSidearmInstallationObject>();
                    }
                    else
                    {
                        return new Response<EditSidearmInstallationObject>(false, null, null, "This sidearm is not found", (int)ApiReturnCode.fail);
                    }
                }
               
                catch (Exception err)
                {
                    return new Response<EditSidearmInstallationObject>(false, null, null, err.Message, (int)ApiReturnCode.fail);
                }
            }
        }
        #region Helper Methods For UpdateSideArm Function..
        public string CheckDependencyValidationEditVersion(object Input, string SiteCode)
        {
            string MainTableName = TablesNames.TLIsideArm.ToString();
            EditSideArmViewModel EditInstallationViewModel = _mapper.Map<EditSideArmViewModel>(Input);

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
        public async Task<Response<IEnumerable<SideArmInstallationPlaceViewModel>>> GetSideArmInstallationPlace(int civilInstallationPlaceType)
        {
            try
            {
                var SideArmInstallationPlace = _unitOfWork.SideArmInstallationPlaceRepository.GetWhere(x => x.CivilInstallationPlaceType == civilInstallationPlaceType).ToList();
                var SideArmInstallationPlaceViewModel = _mapper.Map<IEnumerable<SideArmInstallationPlaceViewModel>>(SideArmInstallationPlace);
                return new Response<IEnumerable<SideArmInstallationPlaceViewModel>>(SideArmInstallationPlaceViewModel);
            }
            catch (Exception err)
            {
                return new Response<IEnumerable<SideArmInstallationPlaceViewModel>>(true, null, null, err.Message, (int)ApiReturnCode.fail);
            }
        }

        public async Task<Response<TLIsideArmInstallationPlace>> AddSideArmInstallationPlace(AddSideArmInstallationPlaceViewModel SideArmInstallationPlace)
        {
            try
            {
                TLIsideArmInstallationPlace SideInstallationPlace = _mapper.Map<TLIsideArmInstallationPlace>(SideArmInstallationPlace);
                await _unitOfWork.SideArmInstallationPlaceRepository.AddAsync(SideInstallationPlace);
                await _unitOfWork.SaveChangesAsync();
                //   return new Response<TLIsideArmInstallationPlace>();
                return new Response<TLIsideArmInstallationPlace>(true, SideInstallationPlace, null, null, (int)ApiReturnCode.success);

            }
            catch (Exception err)
            {

                return new Response<TLIsideArmInstallationPlace>(true, null, null, err.Message, (int)ApiReturnCode.fail);

            }
        }

        public async Task<Response<TLIsideArmInstallationPlace>> UpdateSideArmInstallationPlace(EditSideArmInstallationPlaceViewModel SideArmInstallationPlaceViewModel)
        {
            try
            {
                TLIsideArmInstallationPlace SideArmInstallationPlace = _mapper.Map<TLIsideArmInstallationPlace>(SideArmInstallationPlaceViewModel);
                _unitOfWork.SideArmInstallationPlaceRepository.Update(SideArmInstallationPlace);
                await _unitOfWork.SaveChangesAsync();
                return new Response<TLIsideArmInstallationPlace>();
            }
            catch (Exception err)
            {

                return new Response<TLIsideArmInstallationPlace>(true, null, null, err.Message, (int)ApiReturnCode.fail);
            }
        }
        //Function take 3 parameters
        //loop on each dynamic installation attribute value and add it
        private void AddDynamicAttributes(List<AddDynamicAttInstValueViewModel> addDynamicAttsInst, int TablesNamesId, int InventoryId)
        {
            Parallel.ForEach(addDynamicAttsInst, addDynamicAttInst =>
            {
                TLIdynamicAttInstValue attInstValue = new TLIdynamicAttInstValue();
                attInstValue.InventoryId = InventoryId;
                attInstValue.tablesNamesId = TablesNamesId;
                attInstValue.DynamicAttId = addDynamicAttInst.DynamicAttId;
                attInstValue.disable = false;
                //attInstValue.Value = addDynamicAttInst.Value;
                _unitOfWork.DynamicAttInstValueRepository.Add(attInstValue);
                _unitOfWork.SaveChanges();
            });
        }
        //Function take 2 parameters
        //loop update each dynamic installation attribute value 
        private void UpdateDynamicAttInstValue(List<BaseInstAttView> DynamicInstAttsValue, int Id)
        {
            Parallel.ForEach(DynamicInstAttsValue, DynamicInstAttValue =>
            {
                var DAIV = _unitOfWork.DynamicAttInstValueRepository.GetWhereFirst(d => d.InventoryId == Id && d.DynamicAttId == DynamicInstAttValue.Id);
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
                    DAIV.ValueString = DynamicInstAttValue.Value.ToString();
                    DAIV.Value = DynamicInstAttValue.Value.ToString();
                }
                _unitOfWork.DynamicAttInstValueRepository.Update(DAIV);
            });
        }
        //Function take 2 parameters
        //get table name by table name
        //get library record by LibraryID
        //get library activated attributes with values
        //get activated attributes
        //get dynamic attributes
        //get related tables

        public Response<List<KeyValuePair<string, int>>> getSideArmsForAdd(string SiteCode, int CivilId, int? LegId, int? MinHeight, int? MaxHeight, int? NumberOfLoadsOnSideArm, int? MinAzimuth, int? MaxAzimuth)
        {
            try
            {
                int count = 0;
                int count_1 = 0;
                List<FilterObject> conditions = new List<FilterObject>();
                List<KeyValuePair<string, int>> Results = new List<KeyValuePair<string, int>>();
                List<int> Ids = null;
                if (string.IsNullOrEmpty(SiteCode) == false && CivilId != 0)
                {
                    conditions.Add(new FilterObject("civilLoads.SiteCode", SiteCode));
                    conditions.Add(new FilterObject("civilLoads.allCivilInstId", CivilId));
                    conditions.Add(new FilterObject("civilLoads.sideArmId", 0, ">"));
                }
                if (LegId != null)
                {
                    conditions.Add(new FilterObject("legId", (int)LegId));
                }
                if (MinHeight != null && MaxHeight != null)
                {
                    conditions.Add(new FilterObject("civilLoads.sideArm.Height", MinHeight, ">="));
                    conditions.Add(new FilterObject("civilLoads.sideArm.Height", MaxHeight, "<="));
                }
                if (MinAzimuth != null && MaxAzimuth != null)
                {
                    conditions.Add(new FilterObject("civilLoads.sideArm.Azimuth", MinAzimuth, ">="));
                    conditions.Add(new FilterObject("civilLoads.sideArm.Azimuth", MaxAzimuth, "<="));
                }
                if (NumberOfLoadsOnSideArm != null)
                {
                    List<FilterObject> condition_1 = new List<FilterObject>();
                    condition_1.Add(new FilterObject("sideArmId", 0, ">"));
                    condition_1.Add(new FilterObject("allLoadInstId", 0, ">"));
                    Ids = _unitOfWork.CivilLoadsRepository.GetAllIncludeMultipleWithCondition(null, null, condition_1, out count_1, null).GroupBy(x => x.sideArmId).Select(x => new { Id = x.Key, Count = x.Count() }).Where(x => x.Count == NumberOfLoadsOnSideArm).Select(x => (int)x.Id).ToList();
                }
                Results = _unitOfWork.CivilLoadLegsRepository.GetAllIncludeMultipleWithCondition(null, null, conditions, out count, null).Where(x => Ids.Contains((int)x.civilLoads.sideArmId)).Select(x => new KeyValuePair<string, int>(x.civilLoads.sideArm.Name, (int)x.civilLoads.sideArmId)).ToList();
                return new Response<List<KeyValuePair<string, int>>>(true, Results, null, null, (int)ApiReturnCode.success);
            }
            catch (Exception err)
            {

                return new Response<List<KeyValuePair<string, int>>>(true, null, null, null, (int)ApiReturnCode.fail);
            }
        }

        public Response<IEnumerable<SideArmTypeViewModel>> GetSideArmType()
        {
            try
            {
                IEnumerable<SideArmTypeViewModel> SideArmType = _mapper.Map<List<SideArmTypeViewModel>>(_unitOfWork.SideArmTypeRepository.GetAll(out int count));

                return new Response<IEnumerable<SideArmTypeViewModel>>(true, SideArmType, null, null, (int)ApiReturnCode.success);
            }

            catch (Exception err)
            {

                return new Response<IEnumerable<SideArmTypeViewModel>>(true, null, null, err.Message, (int)ApiReturnCode.fail);
            }
        }
        public Response<List<SideArmInstallationPlaceViewModel>> GetSideArmInstallationPlace(string CivilType, int SideArmTypeId)
        {
            try
            {
                List<SideArmInstallationPlaceViewModel> SideArmInstallationPlaces = new List<SideArmInstallationPlaceViewModel>();

                string SideArmType = _unitOfWork.SideArmTypeRepository.GetByID(SideArmTypeId).Name;

                if (CivilType.ToLower() == TablesNames.TLIcivilWithLegs.ToString().ToLower() &&
                    SideArmType.ToLower() == SideArmTypes.Normal.ToString().ToLower())
                {
                    SideArmInstallationPlaces = _mapper.Map<List<SideArmInstallationPlaceViewModel>>(_unitOfWork.SideArmInstallationPlaceRepository
                        .GetWhere(x => !x.Deleted && !x.Disable && x.Name.ToLower() == "leg").ToList());
                }
                else if (CivilType.ToLower() == TablesNames.TLIcivilWithLegs.ToString().ToLower() &&
                    SideArmType.ToLower() == SideArmTypes.Special.ToString().ToLower())
                {
                    SideArmInstallationPlaces = _mapper.Map<List<SideArmInstallationPlaceViewModel>>(_unitOfWork.SideArmInstallationPlaceRepository
                        .GetWhere(x => !x.Deleted && !x.Disable && (x.Name.ToLower() == "leg" || x.Name.ToLower() == "bracing")).ToList());
                }
                else if (CivilType.ToLower() == TablesNames.TLIcivilWithoutLeg.ToString().ToLower())
                {
                    SideArmInstallationPlaces = _mapper.Map<List<SideArmInstallationPlaceViewModel>>(_unitOfWork.SideArmInstallationPlaceRepository
                        .GetWhere(x => !x.Deleted && !x.Disable &&
                        (x.Name.ToLower() == "supportpole" || x.Name.ToLower() == "supportfence" || x.Name.ToLower() == "mastabovesupport")).ToList());
                }
                else if (CivilType.ToLower() == TablesNames.TLIcivilNonSteel.ToString().ToLower())
                {
                    SideArmInstallationPlaces = _mapper.Map<List<SideArmInstallationPlaceViewModel>>(_unitOfWork.SideArmInstallationPlaceRepository
                        .GetWhere(x => !x.Deleted && !x.Disable && x.Name.ToLower() == "wallsidearm").ToList());
                }
                return new Response<List<SideArmInstallationPlaceViewModel>>(true, SideArmInstallationPlaces, null, null, (int)ApiReturnCode.success, SideArmInstallationPlaces.Count());
            }
            catch (Exception err)
            {
                return new Response<List<SideArmInstallationPlaceViewModel>>(true, null, null, err.Message, (int)ApiReturnCode.fail);
            }
        }
        public Response<List<SideArmTypeViewModel>> GetSideArmTypes(string tablename)
        {
            try
            {
                List<SideArmTypeViewModel> SideArmTypes = null;
                if (tablename.ToLower() == TablesNames.TLIcivilWithLegs.ToString().ToLower())
                {
                    List<string> typesid = new List<string>() { "Normal", "Special" };
                    SideArmTypes = _mapper.Map<List<SideArmTypeViewModel>>(_unitOfWork.SideArmTypeRepository
                     .GetWhere(x => !x.Deleted && !x.Disable && typesid.Contains(x.Name)).ToList());
                }

                else
                {
                    SideArmTypes = _mapper.Map<List<SideArmTypeViewModel>>(_unitOfWork.SideArmTypeRepository
                    .GetWhere(x => !x.Deleted && !x.Disable && x.Name == "Normal").ToList());
                }



                return new Response<List<SideArmTypeViewModel>>(true, SideArmTypes, null, null, (int)ApiReturnCode.success);
            }
            catch (Exception err)
            {
                return new Response<List<SideArmTypeViewModel>>(true, null, null, err.Message, (int)ApiReturnCode.fail);
            }
        }
        public Response<List<SideArmViewModel>> GetSideArmsByAllCivilInstId(int AllCivilInstId)
        {
            try
            {
                List<SideArmViewModel> SideArmsViewModels = _mapper.Map<List<SideArmViewModel>>(_unitOfWork.CivilLoadsRepository
                    .GetIncludeWhere(x => x.allCivilInstId == AllCivilInstId && x.sideArmId != null && !x.Dismantle && x.allLoadInstId == null, x => x.sideArm).Select(x => x.sideArm).Distinct().ToList());

                return new Response<List<SideArmViewModel>>(true, SideArmsViewModels, null, null, (int)ApiReturnCode.success, SideArmsViewModels.Count());
            }
            catch (Exception err)
            {
                return new Response<List<SideArmViewModel>>(true, null, null, err.Message, (int)ApiReturnCode.fail);
            }
        }
        public Response<List<SideArmViewModel>> GetSideArmsByFilters(int AllCivilInstId, float? MaxAzimuth, float? MinAzimuth, float? MaxHeightBase, float? MinHeightBase)
        {
            try
            {
                List<SideArmViewModel> SideArmsViewModels = _mapper.Map<List<SideArmViewModel>>(_unitOfWork.CivilLoadsRepository.GetIncludeWhere(x =>
                    (x.allCivilInstId == AllCivilInstId) && (x.sideArmId != null) &&
                    (MaxAzimuth != null ? (x.sideArm.Azimuth != null ? x.sideArm.Azimuth <= MaxAzimuth.Value : false) : true) &&
                    (MinAzimuth != null ? (x.sideArm.Azimuth != null ? x.sideArm.Azimuth >= MinAzimuth.Value : false) : true) &&
                    (MaxHeightBase != null ? (x.sideArm.HeightBase != null ? x.sideArm.HeightBase <= MaxHeightBase : false) : true) &&
                    (MinHeightBase != null ? (x.sideArm.HeightBase != null ? x.sideArm.HeightBase >= MinHeightBase : false) : true), x => x.sideArm)
                        .Select(x => x.sideArm).ToList());

                return new Response<List<SideArmViewModel>>(true, SideArmsViewModels, null, null, (int)ApiReturnCode.success);
            }
            catch (Exception err)
            {
                return new Response<List<SideArmViewModel>>(true, null, null, err.Message, (int)ApiReturnCode.fail);
            }
        }

        public Response<SideArmViewDto> AddSideArm(SideArmViewDto addSideArms, string SiteCode,int? TaskId, int UserId)
        {
            using (TransactionScope transaction = new TransactionScope())
            {
                try
                {
                    var SideArmLibrary = _unitOfWork.SideArmLibraryRepository.GetWhereFirst(x => x.Id == addSideArms.installationConfig.sideArmLibraryId
                    && x.Active);
                    if (SideArmLibrary != null)
                    {
                        TLItablesNames TableNameEntity = _unitOfWork.TablesNamesRepository.GetWhereFirst(x => x.TableName == TablesNames.TLIsideArm.ToString());
                        TLIsideArm SideArm = _mapper.Map<TLIsideArm>(addSideArms.installationAttributes);
                        SideArm.Active = true;
                        if (addSideArms.installationConfig.civilSteelType == 0)
                        {
                            if (addSideArms.installationConfig.civilWithLegId != 0 || addSideArms.installationConfig.civilWithLegId != null)
                            {
                                if (addSideArms.installationConfig.sideArmTypeId == 1)
                                {
                                    if (addSideArms.installationConfig.installationPlaceId == 1)
                                    {
                                        if (addSideArms.installationConfig.legId.Count == 1)
                                        {
                                            if (SideArm.Azimuth <= 0)
                                            {
                                                return new Response<SideArmViewDto>(false, null, null,
                                                    "Azimuth must bigger from zero", (int)ApiReturnCode.fail);

                                            }
                                            if (SideArm.HeightBase <= 0)
                                            {
                                                return new Response<SideArmViewDto>(false, null, null,
                                                    "HeightBase must bigger from zero", (int)ApiReturnCode.fail);
                                            }
                                            var AzimuthandAndHeightBase = _dbContext.SIDEARM_VIEW.Where(x => x.CIVILID == addSideArms.installationConfig.civilWithLegId.ToString()
                                            && x.SITECODE.ToLower() == SiteCode.ToLower() && x.SIDEARMTYPE.ToLower() == "normal" && x.FIRST_LEG_ID == addSideArms.installationConfig.legId[0]
                                            && x.Azimuth == addSideArms.installationAttributes.Azimuth && x.HeightBase == addSideArms.installationAttributes.HeightBase).ToList();
                                            if (AzimuthandAndHeightBase != null && AzimuthandAndHeightBase.Count() > 0)
                                            {
                                                return new Response<SideArmViewDto>(false, null, null, "can not installed this sidearm on azimuth and heightbase selected because found other sidearm in same azimuth and heightbase", (int)ApiReturnCode.fail);
                                            }
                                            var civilwithlegname = _unitOfWork.CivilSiteDateRepository.GetIncludeWhereFirst(x => x.allCivilInst.civilWithLegsId == addSideArms.installationConfig.civilWithLegId && x.SiteCode.ToLower() == SiteCode.ToLower() && !x.Dismantle
                                            , x => x.allCivilInst, x => x.allCivilInst.civilWithLegs
                                              , x => x.allCivilInst.civilWithoutLeg, x => x.allCivilInst.civilNonSteel);
                                            if (civilwithlegname != null)
                                            {
                                                var LegName = _unitOfWork.LegRepository.GetWhereFirst(x => x.Id == addSideArms.installationConfig.legId[0])?.CiviLegName;
                                                if (civilwithlegname != null && LegName != null)
                                                {
                                                    SideArm.Name = civilwithlegname.allCivilInst.civilWithLegs.Name + LegName + addSideArms.installationAttributes.HeightBase + addSideArms.installationAttributes.Azimuth;

                                                }

                                                var CheckName = _dbContext.SIDEARM_VIEW.FirstOrDefault(x => !x.Dismantle &&
                                                     (x.Id != null ? x.Name.ToLower() == SideArm.Name.ToLower() : false
                                                        && x.SITECODE.ToLower() == SiteCode.ToLower()));

                                                if (CheckName != null)
                                                    return new Response<SideArmViewDto>(false, null, null, $"The name {SideArm.Name} is already exists", (int)Helpers.Constants.ApiReturnCode.fail);

                                                //string CheckDependencyValidation = CheckDependencyValidationForSideArm(addSideArms, SiteCode);

                                                //if (!string.IsNullOrEmpty(CheckDependencyValidation))
                                                //    return new Response<AllItemAttributes>(true, null, null, CheckDependencyValidation, (int)ApiReturnCode.fail);

                                                //string CheckGeneralValidation = CheckGeneralValidationFunction(addSideArms.dynamicAttribute, TableNameEntity.TableName);

                                                //if (!string.IsNullOrEmpty(CheckGeneralValidation))
                                                //    return new Response<AllItemAttributes>(true, null, null, CheckGeneralValidation, (int)ApiReturnCode.fail);

                                                SideArm.sideArmInstallationPlaceId = addSideArms.installationConfig.installationPlaceId;
                                                SideArm.sideArmLibraryId = addSideArms.installationConfig.sideArmLibraryId;
                                                SideArm.sideArmTypeId = addSideArms.installationConfig.sideArmTypeId;
                                                _unitOfWork.SideArmRepository.AddWithHistory(UserId, SideArm);
                                                _unitOfWork.SaveChanges();

                                                if (addSideArms != null && addSideArms.civilLoads != null && addSideArms.installationConfig != null)
                                                {
                                                    TLIcivilLoads civilLoad = new TLIcivilLoads
                                                    {
                                                        InstallationDate = addSideArms.civilLoads.InstallationDate,
                                                        allCivilInstId = civilwithlegname.allCivilInst.Id,
                                                        ItemOnCivilStatus = addSideArms.civilLoads?.ItemOnCivilStatus,
                                                        ItemStatus = addSideArms.civilLoads?.ItemStatus,
                                                        ReservedSpace = addSideArms.civilLoads.ReservedSpace,
                                                        sideArmId = SideArm.Id,
                                                        Dismantle = false,
                                                        SiteCode = SiteCode,
                                                        legId = addSideArms.installationConfig.legId[0]
                                                    };
                                                    civilLoad.allLoadInstId = null;

                                                    _unitOfWork.CivilLoadsRepository.AddWithHistory(UserId, civilLoad);
                                                    _unitOfWork.SaveChangesAsync();
                                                }
                                                if (addSideArms.dynamicAttribute != null ? addSideArms.dynamicAttribute.Count > 0 : false)
                                                {
                                                    foreach (var DynamicAttInstValue in addSideArms.dynamicAttribute)
                                                    {
                                                        _unitOfWork.DynamicAttInstValueRepository.AddDdynamicAttributeInstallation(UserId, DynamicAttInstValue, TableNameEntity.Id, SideArm.Id);
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                return new Response<SideArmViewDto>(false, null, null,
                                                                                          " this civil in not found  ", (int)ApiReturnCode.fail);
                                            }


                                        }
                                        else
                                        {
                                            return new Response<SideArmViewDto>(false, null, null,
                                            "must selected one leg  ", (int)ApiReturnCode.fail);
                                        }
                                    }
                                    else
                                    {
                                        return new Response<SideArmViewDto>(false, null, null,
                                                "installation place must be leg  ", (int)ApiReturnCode.fail);
                                    }
                                }
                                else if (addSideArms.installationConfig.sideArmTypeId == 2)
                                {
                                    if (addSideArms.installationConfig.installationPlaceId == 1 || addSideArms.installationConfig.installationPlaceId == 2)
                                    {
                                        if (addSideArms.installationConfig.legId.Count == 2)
                                        {
                                            if (SideArm.Azimuth <= 0)
                                            {
                                                return new Response<SideArmViewDto>(false, null, null,
                                                    "Azimuth must bigger from zero", (int)ApiReturnCode.fail);

                                            }
                                            if (SideArm.HeightBase <= 0)
                                            {
                                                return new Response<SideArmViewDto>(false, null, null,
                                                    "HeightBase must bigger from zero", (int)ApiReturnCode.fail);
                                            }
                                            var AzimuthandAndHeightBase = _dbContext.SIDEARM_VIEW.Where(x => x.CIVILID == addSideArms.installationConfig.civilWithLegId.ToString()
                                            && x.SITECODE.ToLower() == SiteCode.ToLower() && x.SIDEARMTYPE.ToLower() == "normal" && x.FIRST_LEG_ID == addSideArms.installationConfig.legId[0]
                                            && x.Azimuth == addSideArms.installationAttributes.Azimuth && x.HeightBase == addSideArms.installationAttributes.HeightBase).ToList();
                                            if (AzimuthandAndHeightBase != null && AzimuthandAndHeightBase.Count() > 0)
                                            {
                                                return new Response<SideArmViewDto>(false, null, null, "can not installed this sidearm on azimuth and heightbase selected because found other sidearm in same azimuth and heightbase", (int)ApiReturnCode.fail);
                                            }
                                            var civilwithlegname = _unitOfWork.CivilSiteDateRepository.GetIncludeWhereFirst(x => x.allCivilInst.civilWithLegsId
                                            == addSideArms.installationConfig.civilWithLegId && x.SiteCode.ToLower() == SiteCode.ToLower() && !x.Dismantle, x => x.allCivilInst, x => x.allCivilInst.civilWithLegs
                                            , x => x.allCivilInst.civilWithoutLeg, x => x.allCivilInst.civilNonSteel);
                                            if (civilwithlegname != null)
                                            {
                                                var LegName = _unitOfWork.LegRepository.GetWhereFirst(x => x.Id == addSideArms.installationConfig.legId[0])?.CiviLegName;
                                                var LegName2 = _unitOfWork.LegRepository.GetWhereFirst(x => x.Id == addSideArms.installationConfig.legId[1])?.CiviLegName;
                                                if (LegName != null && LegName2 != null && civilwithlegname != null)
                                                {
                                                    SideArm.Name = civilwithlegname.allCivilInst.civilWithLegs.Name + LegName + LegName2 + addSideArms.installationAttributes.HeightBase + addSideArms.installationAttributes.Azimuth;

                                                }
                                                var CheckName = _dbContext.SIDEARM_VIEW.FirstOrDefault(x => !x.Dismantle &&
                                                             (x.Id != null ? x.Name.ToLower() == SideArm.Name.ToLower() : false
                                                                && x.SITECODE.ToLower() == SiteCode.ToLower()));

                                                if (CheckName != null)
                                                    return new Response<SideArmViewDto>(false, null, null, $"The name {SideArm.Name} is already exists", (int)Helpers.Constants.ApiReturnCode.fail);

                                                //string CheckDependencyValidation = CheckDependencyValidationForSideArm(addSideArms, SiteCode);

                                                //if (!string.IsNullOrEmpty(CheckDependencyValidation))
                                                //    return new Response<AllItemAttributes>(true, null, null, CheckDependencyValidation, (int)ApiReturnCode.fail);

                                                //string CheckGeneralValidation = CheckGeneralValidationFunction(addSideArms.dynamicAttribute, TableNameEntity.TableName);

                                                //if (!string.IsNullOrEmpty(CheckGeneralValidation))
                                                //    return new Response<AllItemAttributes>(true, null, null, CheckGeneralValidation, (int)ApiReturnCode.fail);

                                                SideArm.sideArmInstallationPlaceId = addSideArms.installationConfig.installationPlaceId;
                                                SideArm.sideArmLibraryId = addSideArms.installationConfig.sideArmLibraryId;
                                                SideArm.sideArmTypeId = addSideArms.installationConfig.sideArmTypeId;
                                                _unitOfWork.SideArmRepository.AddWithHistory(UserId, SideArm);
                                                _unitOfWork.SaveChanges();

                                                if (addSideArms != null && addSideArms.civilLoads != null && addSideArms.installationConfig != null)
                                                {
                                                    TLIcivilLoads civilLoad = new TLIcivilLoads
                                                    {
                                                        InstallationDate = addSideArms.civilLoads.InstallationDate,
                                                        allCivilInstId = civilwithlegname.allCivilInst.Id,
                                                        ItemOnCivilStatus = addSideArms.civilLoads?.ItemOnCivilStatus,
                                                        ItemStatus = addSideArms.civilLoads?.ItemStatus,
                                                        ReservedSpace = addSideArms.civilLoads.ReservedSpace,
                                                        sideArmId = SideArm.Id,
                                                        SiteCode = SiteCode,
                                                        Dismantle = false,
                                                        legId = addSideArms.installationConfig.legId[0],
                                                        Leg2Id = addSideArms.installationConfig.legId[1],
                                                 
                                                    };
                                                    civilLoad.allLoadInstId = null;

                                                    _unitOfWork.CivilLoadsRepository.AddWithHistory(UserId, civilLoad);
                                                    _unitOfWork.SaveChangesAsync();
                                                }
                                                if (addSideArms.dynamicAttribute != null ? addSideArms.dynamicAttribute.Count > 0 : false)
                                                {
                                                    foreach (var DynamicAttInstValue in addSideArms.dynamicAttribute)
                                                    {
                                                        _unitOfWork.DynamicAttInstValueRepository.AddDdynamicAttributeInstallation(UserId, DynamicAttInstValue, TableNameEntity.Id, SideArm.Id);
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                return new Response<SideArmViewDto>(false, null, null,
                                                                                          " this civil in not found  ", (int)ApiReturnCode.fail);
                                            }
                                        }
                                        else
                                        {
                                            return new Response<SideArmViewDto>(false, null, null,
                                            "must selected tow legs  ", (int)ApiReturnCode.fail);
                                        }

                                    }
                                    else
                                    {
                                        return new Response<SideArmViewDto>(false, null, null,
                                                "installation place must be leg or bracing ", (int)ApiReturnCode.fail);
                                    }
                                }
                                
                            }
                            else
                            {
                                return new Response<SideArmViewDto>(false, null, null,
                               "must selected civilwithleg support item  ", (int)ApiReturnCode.fail);
                            }
                        }
                        else if (addSideArms.installationConfig.civilSteelType == 1)
                        {
                            if (addSideArms.installationConfig.civilWithoutLegId != 0 || addSideArms.installationConfig.civilWithoutLegId != null)
                            {
                                if (addSideArms.installationConfig.sideArmTypeId == 1)
                                {

                                    if (addSideArms.installationConfig.installationPlaceId == 3 || addSideArms.installationConfig.installationPlaceId == 4||
                                        addSideArms.installationConfig.installationPlaceId == 5)
                                    {
                                        if (SideArm.Azimuth <= 0)
                                        {
                                            return new Response<SideArmViewDto>(false, null, null,
                                                "Azimuth must bigger from zero", (int)ApiReturnCode.fail);

                                        }
                                        if (SideArm.HeightBase <= 0)
                                        {
                                            return new Response<SideArmViewDto>(false, null, null,
                                                "HeightBase must bigger from zero", (int)ApiReturnCode.fail);
                                        }
                                        var AzimuthandAndHeightBase = _dbContext.SIDEARM_VIEW.Where(x => x.CIVILID == addSideArms.installationConfig.civilWithLegId.ToString()
                                            && x.SITECODE.ToLower() == SiteCode.ToLower() && x.SIDEARMTYPE.ToLower() == "normal" && x.FIRST_LEG_ID == addSideArms.installationConfig.legId[0]
                                            && x.Azimuth == addSideArms.installationAttributes.Azimuth && x.HeightBase == addSideArms.installationAttributes.HeightBase).ToList();
                                        if (AzimuthandAndHeightBase != null && AzimuthandAndHeightBase.Count() > 0)
                                        {
                                            return new Response<SideArmViewDto>(false, null, null, "can not installed this sidearm on azimuth and heightbase selected because found other sidearm in same azimuth and heightbase", (int)ApiReturnCode.fail);
                                        }
                                        var civilwithlegname = _unitOfWork.CivilSiteDateRepository.GetIncludeWhereFirst(x => x.allCivilInst.civilWithoutLegId == addSideArms.installationConfig.civilWithoutLegId && x.SiteCode.ToLower() == SiteCode.ToLower() && !x.Dismantle
                                        , x => x.allCivilInst, x => x.allCivilInst.civilWithLegs,
                                        x => x.allCivilInst.civilWithoutLeg, x => x.allCivilInst.civilNonSteel);
                                        if (civilwithlegname != null)
                                        {

                                            SideArm.Name = civilwithlegname.allCivilInst.civilWithoutLeg.Name + addSideArms.installationAttributes.HeightBase + addSideArms.installationAttributes.Azimuth;

                                            var CheckName = _dbContext.SIDEARM_VIEW.FirstOrDefault(x => !x.Dismantle &&
                                                    (x.Id != null ? x.Name.ToLower() == SideArm.Name.ToLower() : false
                                                       && x.SITECODE.ToLower() == SiteCode.ToLower()));

                                            if (CheckName != null)
                                                return new Response<SideArmViewDto>(false, null, null, $"The name {SideArm.Name} is already exists", (int)Helpers.Constants.ApiReturnCode.fail);

                                            //string CheckDependencyValidation = CheckDependencyValidationForSideArm(addSideArms, SiteCode);

                                            //if (!string.IsNullOrEmpty(CheckDependencyValidation))
                                            //    return new Response<AllItemAttributes>(true, null, null, CheckDependencyValidation, (int)ApiReturnCode.fail);

                                            //string CheckGeneralValidation = CheckGeneralValidationFunction(addSideArms.dynamicAttribute, TableNameEntity.TableName);

                                            //if (!string.IsNullOrEmpty(CheckGeneralValidation))
                                            //    return new Response<AllItemAttributes>(true, null, null, CheckGeneralValidation, (int)ApiReturnCode.fail);

                                            SideArm.sideArmInstallationPlaceId = addSideArms.installationConfig.installationPlaceId;
                                            SideArm.sideArmLibraryId = addSideArms.installationConfig.sideArmLibraryId;
                                            SideArm.sideArmTypeId = addSideArms.installationConfig.sideArmTypeId;
                                            _unitOfWork.SideArmRepository.AddWithHistory(UserId, SideArm);
                                            _unitOfWork.SaveChanges();

                                            if (addSideArms != null && addSideArms.civilLoads != null && addSideArms.installationConfig != null)
                                            {
                                                TLIcivilLoads civilLoad = new TLIcivilLoads
                                                {
                                                    InstallationDate = addSideArms.civilLoads.InstallationDate,
                                                    allCivilInstId = civilwithlegname.allCivilInst.Id,

                                                    ItemOnCivilStatus = addSideArms.civilLoads?.ItemOnCivilStatus,
                                                    ItemStatus = addSideArms.civilLoads?.ItemStatus,
                                                    ReservedSpace = addSideArms.civilLoads.ReservedSpace,
                                                    Dismantle = false,
                                                    sideArmId = SideArm.Id,
                                                    SiteCode = SiteCode
                                                };

                                                if (addSideArms.installationConfig.legId != null && addSideArms.installationConfig.legId.Count > 0)
                                                {
                                                    civilLoad.legId = addSideArms.installationConfig.legId[0];

                                                    if (addSideArms.installationConfig.legId.Count > 1)
                                                    {
                                                        civilLoad.Leg2Id = addSideArms.installationConfig.legId[1];
                                                    }
                                                    else
                                                    {
                                                        civilLoad.Leg2Id = null;
                                                    }
                                                }
                                                else
                                                {
                                                    civilLoad.legId = null;
                                                    civilLoad.Leg2Id = null;
                                                }

                                                civilLoad.allLoadInstId = null;

                                                _unitOfWork.CivilLoadsRepository.AddWithHistory(UserId, civilLoad);
                                                _unitOfWork.SaveChangesAsync();
                                            }


                                            if (addSideArms.dynamicAttribute != null ? addSideArms.dynamicAttribute.Count > 0 : false)
                                            {
                                                foreach (var DynamicAttInstValue in addSideArms.dynamicAttribute)
                                                {
                                                    _unitOfWork.DynamicAttInstValueRepository.AddDdynamicAttributeInstallation(UserId, DynamicAttInstValue, TableNameEntity.Id, SideArm.Id);
                                                }
                                            }
                                        }
                                        else
                                        {
                                            return new Response<SideArmViewDto>(false, null, null,
                                                                                      " this civil in not found  ", (int)ApiReturnCode.fail);
                                        }
                                    }
                                    else
                                    {
                                        return new Response<SideArmViewDto>(false, null, null,
                                        "installation place must be SupportRole or Supportfence or AboveSupport", (int)ApiReturnCode.fail);
                                    }


                                }
                                else
                                {
                                    return new Response<SideArmViewDto>(false, null, null,
                                  "can not selected installation type other normal ", (int)ApiReturnCode.fail);

                                }
                            }
                            else
                            {
                                return new Response<SideArmViewDto>(false, null, null,
                               "must selected civilwithout support item  ", (int)ApiReturnCode.fail);
                            }
                        }
                        else if (addSideArms.installationConfig.civilSteelType == 2)
                        {
                            if (addSideArms.installationConfig.civilNonSteelId != 0 || addSideArms.installationConfig.civilWithLegId != null)
                            {
                                if (addSideArms.installationConfig.sideArmTypeId == 1)
                                {
                                    if (addSideArms.installationConfig.installationPlaceId == 6)
                                    {
                                        if (SideArm.Azimuth <= 0)
                                        {
                                            return new Response<SideArmViewDto>(false, null, null,
                                                "Azimuth must bigger from zero", (int)ApiReturnCode.fail);

                                        }
                                        if (SideArm.HeightBase <= 0)
                                        {
                                            return new Response<SideArmViewDto>(false, null, null,
                                                "HeightBase must bigger from zero", (int)ApiReturnCode.fail);
                                        }
                                        var AzimuthandAndHeightBase = _dbContext.SIDEARM_VIEW.Where(x => x.CIVILID == addSideArms.installationConfig.civilWithLegId.ToString()
                                           && x.SITECODE.ToLower() == SiteCode.ToLower() && x.SIDEARMTYPE.ToLower() == "normal" && x.FIRST_LEG_ID == addSideArms.installationConfig.legId[0]
                                           && x.Azimuth == addSideArms.installationAttributes.Azimuth && x.HeightBase == addSideArms.installationAttributes.HeightBase).ToList();
                                        if (AzimuthandAndHeightBase != null && AzimuthandAndHeightBase.Count() > 0)
                                        {
                                            return new Response<SideArmViewDto>(false, null, null, "can not installed this sidearm on azimuth and heightbase selected because found other sidearm in same azimuth and heightbase", (int)ApiReturnCode.fail);
                                        }
                                        var civilwithlegname = _unitOfWork.CivilSiteDateRepository.GetIncludeWhereFirst(x => x.allCivilInst.civilNonSteelId == addSideArms.installationConfig.civilNonSteelId && x.SiteCode.ToLower() == SiteCode.ToLower() && !x.Dismantle
                                        , x => x.allCivilInst, x => x.allCivilInst.civilWithLegs,
                                        x => x.allCivilInst.civilWithoutLeg, x => x.allCivilInst.civilNonSteel);
                                        if (civilwithlegname != null)
                                        {
                                            SideArm.Name = civilwithlegname.allCivilInst.civilNonSteel.Name + addSideArms.installationAttributes.HeightBase + addSideArms.installationAttributes.Azimuth;

                                            var CheckName = _dbContext.SIDEARM_VIEW.FirstOrDefault(x => !x.Dismantle &&
                                                             (x.Id != null ? x.Name.ToLower() == SideArm.Name.ToLower() : false
                                                                && x.SITECODE.ToLower() == SiteCode.ToLower()));

                                            if (CheckName != null)
                                                return new Response<SideArmViewDto>(false, null, null, $"The name {SideArm.Name} is already exists", (int)Helpers.Constants.ApiReturnCode.fail);

                                            //string CheckDependencyValidation = CheckDependencyValidationForSideArm(addSideArms, SiteCode);

                                            //if (!string.IsNullOrEmpty(CheckDependencyValidation))
                                            //    return new Response<AllItemAttributes>(true, null, null, CheckDependencyValidation, (int)ApiReturnCode.fail);

                                            //string CheckGeneralValidation = CheckGeneralValidationFunction(addSideArms.dynamicAttribute, TableNameEntity.TableName);

                                            //if (!string.IsNullOrEmpty(CheckGeneralValidation))
                                            //    return new Response<AllItemAttributes>(true, null, null, CheckGeneralValidation, (int)ApiReturnCode.fail);

                                            
                                            SideArm.sideArmInstallationPlaceId = addSideArms.installationConfig.installationPlaceId;
                                            SideArm.sideArmLibraryId = addSideArms.installationConfig.sideArmLibraryId;
                                            SideArm.sideArmTypeId = addSideArms.installationConfig.sideArmTypeId;
                                            _unitOfWork.SideArmRepository.AddWithHistory(UserId, SideArm);
                                            _unitOfWork.SaveChanges();

                                            if (addSideArms != null && addSideArms.civilLoads != null && addSideArms.installationConfig != null)
                                            {
                                                TLIcivilLoads civilLoad = new TLIcivilLoads
                                                {
                                                    InstallationDate = addSideArms.civilLoads.InstallationDate,
                                                    allCivilInstId = civilwithlegname.allCivilInst.Id, 
                                                    ItemOnCivilStatus = addSideArms.civilLoads?.ItemOnCivilStatus,
                                                    ItemStatus = addSideArms.civilLoads?.ItemStatus,
                                                    ReservedSpace = addSideArms.civilLoads.ReservedSpace,
                                                    sideArmId = SideArm.Id,
                                                    Dismantle = false,
                                                    SiteCode = SiteCode,
                                                  
                                                };

                                                if (addSideArms.installationConfig.legId != null && addSideArms.installationConfig.legId.Count > 0)
                                                {
                                                    civilLoad.legId = addSideArms.installationConfig.legId[0];

                                                    if (addSideArms.installationConfig.legId.Count > 1)
                                                    {
                                                        civilLoad.Leg2Id = addSideArms.installationConfig.legId[1];
                                                    }
                                                    else
                                                    {
                                                        civilLoad.Leg2Id = null;
                                                    }
                                                }
                                                else
                                                {
                                                    civilLoad.legId = null;
                                                    civilLoad.Leg2Id = null;
                                                }

                                                civilLoad.allLoadInstId = null;

                                                _unitOfWork.CivilLoadsRepository.AddWithHistory(UserId, civilLoad);
                                                _unitOfWork.SaveChangesAsync();
                                            }
                                            if (addSideArms.dynamicAttribute != null ? addSideArms.dynamicAttribute.Count > 0 : false)
                                            {
                                                foreach (var DynamicAttInstValue in addSideArms.dynamicAttribute)
                                                {
                                                    _unitOfWork.DynamicAttInstValueRepository.AddDdynamicAttributeInstallation(UserId, DynamicAttInstValue, TableNameEntity.Id, SideArm.Id);
                                                }
                                            }
                                        }
                                        else
                                        {
                                            return new Response<SideArmViewDto>(false, null, null,
                                                                                      " this civil in not found  ", (int)ApiReturnCode.fail);
                                        }
                                    }
                                    else
                                    {
                                        return new Response<SideArmViewDto>(false, null, null,
                                        "installation place musst be wall sidearm", (int)ApiReturnCode.fail);

                                    }
                                }

                                else
                                {
                                    return new Response<SideArmViewDto>(false, null, null,
                                    "can not selected installation type other normal ", (int)ApiReturnCode.fail);

                                }
                            }
                            else
                            {
                                return new Response<SideArmViewDto>(false, null, null,
                                "must selected civilNonSteel support item  ", (int)ApiReturnCode.fail);
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
                                return new Response<SideArmViewDto>(false, null, null, result.errorMessage.ToString(), (int)ApiReturnCode.fail);
                            }
                        }
                        else
                        {
                            _unitOfWork.SaveChanges();
                            transaction.Complete();
                        }
                        return new Response<SideArmViewDto>();
                    }
                    else
                    {
                        return new Response<SideArmViewDto>(false, null, null,
                               "It is not possible to select a sidearmlibrary  that does not exist or is in a disable state", (int)ApiReturnCode.fail);
                    }
                }
                catch (Exception err)
                {
                    return new Response<SideArmViewDto>(false, null, null, err.Message, (int)ApiReturnCode.fail);
                }
            }
                    
        }
        
    }
}
