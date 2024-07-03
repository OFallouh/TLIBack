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
using System.Transactions;
using TLIS_DAL.Helper;
using TLIS_DAL.Helper.Filters;
using TLIS_DAL.Helpers;
using System.Collections;
using System.Globalization;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModelBase;
using TLIS_DAL.ViewModels.CabinetDTOs;
using TLIS_DAL.ViewModels.DataTypeDTOs;
using TLIS_DAL.ViewModels.DynamicAttDTOs;
using TLIS_DAL.ViewModels.DynamicAttInstValueDTOs;
using TLIS_DAL.ViewModels.DynamicAttLibValueDTOs;
using TLIS_DAL.ViewModels.PowerDTOs;
using TLIS_DAL.ViewModels.TablesHistoryDTOs;
using TLIS_Repository.Base;
using TLIS_Service.IService;
using static TLIS_Service.Helpers.Constants;
using TLIS_DAL.ViewModels.LogisticalDTOs;
using AutoMapper;
using TLIS_DAL.ViewModels.MW_RFUDTOs;
using TLIS_DAL.ViewModels.CivilWithLegLibraryDTOs;
using TLIS_DAL.ViewModels.PowerLibraryDTOs;
using TLIS_DAL.ViewModels.CivilWithLegsDTOs;
using TLIS_DAL.ViewModels.AsTypeDTOs;
using TLIS_DAL.ViewModels.PolarityTypeDTOs;
using TLIS_DAL.ViewModels.MW_DishLbraryDTOs;
using Org.BouncyCastle.Asn1.Cms;
using TLIS_DAL.ViewModels.ParityDTOs;
using TLIS_DAL;
using TLIS_DAL.ViewModels.MW_ODULibraryDTOs;

namespace TLIS_Service.Services
{
    public class PowerLibraryService : IPowerLibraryService
    {
        private readonly IUnitOfWork _unitOfWork;
        IServiceCollection _services;
        private IMapper _mapper;
        ApplicationDbContext db;
        public PowerLibraryService(IUnitOfWork unitOfWork, IServiceCollection services,IMapper mapper, ApplicationDbContext _context)
        {
            _unitOfWork = unitOfWork;
            _services = services;
            _mapper = mapper;
            db = _context;
        }
        //Function take 2 parameters parameters, filters
        //Function return all records 
        public Response<ReturnWithFilters<PowerLibraryViewModel>> GetPowerLibraries(ParameterPagination parameters, List<FilterObjectList> filters = null)
        {
            try
            {
                int count = 0;
                ReturnWithFilters<PowerLibraryViewModel> Response = new ReturnWithFilters<PowerLibraryViewModel>();
                List<FilterObject> condition = new List<FilterObject>();
                condition.Add(new FilterObject("Active", true));
                condition.Add(new FilterObject("Deleted", false));
                var PowerLibraries = _unitOfWork.PowerLibraryRepository.GetAllIncludeMultipleWithCondition(parameters, filters, condition, out count).OrderBy(x => x.Id).ToList();
                var PowerLibrariesModels = _mapper.Map<List<PowerLibraryViewModel>>(PowerLibraries);
                Response.Model = PowerLibrariesModels;
                Response.filters = null;
                return new Response<ReturnWithFilters<PowerLibraryViewModel>>(true, Response, null, null, (int)Helpers.Constants.ApiReturnCode.success, count);
            }
            catch (Exception err)
            {

                return new Response<ReturnWithFilters<PowerLibraryViewModel>>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
        }

        #region Get Enabled Attributes Only With Dynamic Objects (Libraries Only)...
        #region Helper Methods
        public void GetInventoriesIdsFromDynamicAttributes(out List<int> DynamicLibValueListIds, List<TLIdynamicAtt> LibDynamicAttListIds, List<StringFilterObjectList> AttributeFilters)
        {
            try
            {
                List<StringFilterObjectList> DynamicLibAttributeFilters = AttributeFilters.Where(x =>
                    LibDynamicAttListIds.Select(y => y.Key.ToLower()).Contains(x.key.ToLower())).ToList();

                DynamicLibValueListIds = new List<int>();

                List<TLIdynamicAttLibValue> DynamicLibValueListObjects = _unitOfWork.DynamicAttLibRepository.GetIncludeWhere(x =>
                    LibDynamicAttListIds.Select(y => y.Id).Contains(x.DynamicAttId) && !x.disable).ToList();

                List<int> InventoriesIds = DynamicLibValueListObjects.Select(x => x.InventoryId).Distinct().ToList();

                foreach (int InventoryId in InventoriesIds)
                {
                    List<TLIdynamicAttLibValue> DynamicLibValueListInventories = DynamicLibValueListObjects.Where(x => x.InventoryId == InventoryId).ToList();

                    if (DynamicLibAttributeFilters.All(y => DynamicLibValueListInventories.Exists(x =>
                         (x.ValueBoolean != null) ?
                            (y.value.Any(z => x.ValueBoolean.ToString().ToLower().StartsWith(z.ToLower()))) :

                         (x.ValueDateTime != null ?
                            (y.value.Any(z => z.ToLower() == x.ValueDateTime.ToString().ToLower())) :

                         (x.ValueDouble != null ?
                            (y.value.Any(z => z.ToLower() == x.ValueDouble.ToString().ToLower())) :

                         (!string.IsNullOrEmpty(x.ValueString) ?
                            (y.value.Any(z => x.ValueString.ToLower().StartsWith(z.ToLower()))) : (false)))))))
                    {
                        DynamicLibValueListIds.Add(InventoryId);
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
        public Response<ReturnWithFilters<object>> GetPowerLibrariesWithEnableAttributes(CombineFilters CombineFilters, ParameterPagination parameterPagination)
        {
            try
            {
                List<FilterObjectList> ObjectAttributeFilters = CombineFilters.filters;
                List<DateFilterViewModel> DateFilter = CombineFilters.DateFilter;
                int Count = 0;
                List<object> OutPutList = new List<object>();
                ReturnWithFilters<object> PowerTableDisplay = new ReturnWithFilters<object>();

                List<StringFilterObjectList> AttributeFilters = new List<StringFilterObjectList>();

                List<PowerLibraryViewModel> PowersLibraries = new List<PowerLibraryViewModel>();
                List<PowerLibraryViewModel> WithoutDateFilterPowersLibraries = new List<PowerLibraryViewModel>();
                List<PowerLibraryViewModel> WithDateFilterPowersLibraries = new List<PowerLibraryViewModel>();

                List<TLIattributeActivated> PowerLibraryAttribute = new List<TLIattributeActivated>();
                if ((DateFilter != null ? DateFilter.Count() > 0 : false) ||
                    (ObjectAttributeFilters != null && ObjectAttributeFilters.Count > 0))
                {
                    PowerLibraryAttribute = _unitOfWork.AttributeViewManagmentRepository.GetIncludeWhere(x =>
                        x.Enable && x.AttributeActivatedId != null &&
                        x.AttributeActivated.DataType.ToLower() != "datetime" &&
                        x.EditableManagmentView.View == Helpers.Constants.EditableManamgmantViewNames.PowerLibrary.ToString() &&
                        x.EditableManagmentView.TLItablesNames1.TableName == Helpers.Constants.TablesNames.TLIpowerLibrary.ToString(),
                            x => x.AttributeActivated, x => x.EditableManagmentView, x => x.EditableManagmentView.TLItablesNames1)
                    .Select(x => x.AttributeActivated).ToList();
                }

                if (ObjectAttributeFilters != null && ObjectAttributeFilters.Count > 0)
                {
                    List<TLIattributeActivated> NotDateDatePowerLibraryAttribute = PowerLibraryAttribute.Where(x =>
                        x.DataType.ToLower() != "datetime").ToList();

                    foreach (FilterObjectList item in ObjectAttributeFilters)
                    {
                        List<string> value = item.value.Select(x => x.ToString().ToLower()).ToList();

                        TLIattributeActivated AttributeKey = NotDateDatePowerLibraryAttribute.FirstOrDefault(x =>
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
                if (AttributeFilters != null && AttributeFilters.Count > 0)
                {
                    //
                    // Library Dynamic Attributes...
                    //
                    List<TLIdynamicAtt> LibDynamicAttListIds = _unitOfWork.DynamicAttRepository.GetIncludeWhere(x =>
                        AttributeFilters.AsEnumerable().Select(y => y.key.ToLower()).Contains(x.Key.ToLower()) &&
                        x.LibraryAtt && !x.disable &&
                        x.tablesNames.TableName == Helpers.Constants.TablesNames.TLIpowerLibrary.ToString()
                            , x => x.tablesNames, x => x.DataType).ToList();

                    List<int> DynamicLibValueListIds = new List<int>();
                    bool DynamicLibExist = false;

                    if (LibDynamicAttListIds.Count > 0)
                    {
                        DynamicLibExist = true;
                        GetInventoriesIdsFromDynamicAttributes(out DynamicLibValueListIds, LibDynamicAttListIds, AttributeFilters);
                    }

                    //
                    // Library Attribute Activated...
                    //
                    bool AttrLibExist = typeof(PowerLibraryViewModel).GetProperties().ToList().Exists(x =>
                        AttributeFilters.Where(y => y.key.ToLower() != "Id".ToLower()).Select(y =>
                            y.key.ToLower()).Contains(x.Name.ToLower()));

                    List<int> LibraryAttributeActivatedIds = new List<int>();

                    if (AttrLibExist)
                    {
                        List<PropertyInfo> NonStringLibraryProps = typeof(PowerLibraryViewModel).GetProperties().Where(x =>
                            x.PropertyType.Name.ToLower() != "string" &&
                            AttributeFilters.Select(y =>
                                y.key.ToLower()).Contains(x.Name.ToLower())).ToList();

                        List<PropertyInfo> StringLibraryProps = typeof(PowerLibraryViewModel).GetProperties().Where(x =>
                            x.PropertyType.Name.ToLower() == "string" &&
                            AttributeFilters.Select(y =>
                                y.key.ToLower()).Contains(x.Name.ToLower())).ToList();

                        List<StringFilterObjectList> LibraryPropsAttributeFilters = AttributeFilters.Where(x =>
                            NonStringLibraryProps.Select(y => y.Name.ToLower()).Contains(x.key.ToLower()) ||
                            StringLibraryProps.Select(y => y.Name.ToLower()).Contains(x.key.ToLower())).ToList();

                        //LibraryAttributeActivatedIds = _unitOfWork.PowerLibraryRepository.GetWhere(x =>
                        //     LibraryPropsAttributeFilters.All(z =>
                        //        NonStringLibraryProps.Exists(y => (z.key.ToLower() == y.Name.ToLower()) && (y.GetValue(_mapper.Map<PowerLibraryViewModel>(x), null) != null ? z.value.Contains(y.GetValue(_mapper.Map<PowerLibraryViewModel>(x), null).ToString().ToLower()) : false)) ||
                        //        StringLibraryProps.Exists(y => (z.key.ToLower() == y.Name.ToLower()) && (z.value.Any(w =>
                        //             y.GetValue(_mapper.Map<PowerLibraryViewModel>(x), null) != null ? y.GetValue(_mapper.Map<PowerLibraryViewModel>(x), null).ToString().ToLower().StartsWith(w.ToLower()) : false))))
                        // ).Select(i => i.Id).ToList();

                        IEnumerable<TLIpowerLibrary> Libraries = _unitOfWork.PowerLibraryRepository.GetWhere(x => !x.Deleted).AsEnumerable();

                        foreach (StringFilterObjectList LibraryProp in LibraryPropsAttributeFilters)
                        {
                            if (StringLibraryProps.Select(x => x.Name.ToLower()).Contains(LibraryProp.key.ToLower()))
                            {
                                Libraries = Libraries.Where(x => StringLibraryProps.AsEnumerable().FirstOrDefault(y => (LibraryProp.key.ToLower() == y.Name.ToLower()) && (LibraryProp.value.AsEnumerable().FirstOrDefault(w =>
                                     y.GetValue(_mapper.Map<PowerLibraryViewModel>(x), null) != null ? y.GetValue(_mapper.Map<PowerLibraryViewModel>(x), null).ToString().ToLower().StartsWith(w.ToLower()) : false) != null)) != null).AsEnumerable();
                            }
                            else if (NonStringLibraryProps.Select(x => x.Name.ToLower()).Contains(LibraryProp.key.ToLower()))
                            {
                                Libraries = Libraries.Where(x => NonStringLibraryProps.AsEnumerable().FirstOrDefault(y => (LibraryProp.key.ToLower() == y.Name.ToLower()) && (y.GetValue(_mapper.Map<PowerLibraryViewModel>(x), null) != null ?
                                    LibraryProp.value.AsEnumerable().Contains(y.GetValue(_mapper.Map<PowerLibraryViewModel>(x), null).ToString().ToLower()) : false)) != null).AsEnumerable();
                            }
                        }

                        LibraryAttributeActivatedIds = Libraries.Select(x => x.Id).ToList();
                    }

                    //
                    // Library (Attribute Activated + Dynamic) Attributes...
                    //
                    List<int> IntersectLibraryIds = new List<int>();
                    if (AttrLibExist && DynamicLibExist)
                    {
                        IntersectLibraryIds = LibraryAttributeActivatedIds.Intersect(DynamicLibValueListIds).ToList();
                    }
                    else if (AttrLibExist)
                    {
                        IntersectLibraryIds = LibraryAttributeActivatedIds;
                    }
                    else if (DynamicLibExist)
                    {
                        IntersectLibraryIds = DynamicLibValueListIds;
                    }

                    WithoutDateFilterPowersLibraries = _mapper.Map<List<PowerLibraryViewModel>>(_unitOfWork.PowerLibraryRepository.GetIncludeWhere(x =>
                        x.Id > 0 && IntersectLibraryIds.Contains(x.Id) && !x.Deleted).ToList());
                }

                //
                // DateTime Objects Filters..
                //
                List<DateFilterViewModel> AfterConvertDateFilters = new List<DateFilterViewModel>();
                if (DateFilter != null ? DateFilter.Count() > 0 : false)
                {
                    List<TLIattributeActivated> DatePowerLibraryAttribute = PowerLibraryAttribute.Where(x =>
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

                        TLIattributeActivated AttributeKey = DatePowerLibraryAttribute.FirstOrDefault(x =>
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
                if (AfterConvertDateFilters != null ? AfterConvertDateFilters.Count() > 0 : false)
                {
                    //
                    // Library Dynamic Attributes...
                    //
                    List<TLIdynamicAtt> DateTimeLibDynamicAttListIds = _unitOfWork.DynamicAttRepository.GetIncludeWhere(x =>
                        AfterConvertDateFilters.AsEnumerable().Select(y => y.key.ToLower()).Contains(x.Key.ToLower()) &&
                        x.LibraryAtt && !x.disable &&
                        x.tablesNames.TableName == Helpers.Constants.TablesNames.TLIpowerLibrary.ToString(), x => x.tablesNames, x => x.DataType).ToList();

                    List<int> DynamicLibValueListIds = new List<int>();
                    bool DynamicLibExist = false;

                    if (DateTimeLibDynamicAttListIds.Count > 0)
                    {
                        DynamicLibExist = true;
                        List<DateFilterViewModel> DynamicLibAttributeFilters = AfterConvertDateFilters.Where(x =>
                            DateTimeLibDynamicAttListIds.AsEnumerable().Select(y => y.Key.ToLower()).Contains(x.key.ToLower())).ToList();

                        DynamicLibValueListIds = new List<int>();

                        List<TLIdynamicAttLibValue> DynamicLibValueListObjects = _unitOfWork.DynamicAttLibRepository.GetIncludeWhere(x =>
                            DateTimeLibDynamicAttListIds.Select(y => y.Id).Any(y => y == x.DynamicAttId) && !x.disable).ToList();

                        List<int> InventoriesIds = DynamicLibValueListObjects.Select(x => x.InventoryId).Distinct().ToList();

                        foreach (int InventoryId in InventoriesIds)
                        {
                            List<TLIdynamicAttLibValue> DynamicLibValueListInventories = DynamicLibValueListObjects.Where(x =>
                                x.InventoryId == InventoryId).ToList();

                            if (DynamicLibAttributeFilters.All(y => DynamicLibValueListInventories.Select(x => x.ValueDateTime).Any(x =>
                                 (x != null ?
                                    (x >= y.DateFrom && x <= y.DateTo) : (false)))))
                            {
                                DynamicLibValueListIds.Add(InventoryId);
                            }
                        }
                    }

                    //
                    // Library Attribute Activated...
                    //
                    List<PropertyInfo> LibraryProps = typeof(PowerLibraryViewModel).GetProperties().Where(x =>
                        AfterConvertDateFilters.Select(y =>
                            y.key.ToLower()).Contains(x.Name.ToLower())).ToList();

                    List<int> LibraryAttributeActivatedIds = new List<int>();
                    bool AttrLibExist = false;

                    if (LibraryProps != null)
                    {
                        AttrLibExist = true;

                        List<DateFilterViewModel> LibraryPropsAttributeFilters = AfterConvertDateFilters.Where(x =>
                            LibraryProps.Select(y => y.Name.ToLower()).Contains(x.key.ToLower())).ToList();

                        //LibraryAttributeActivatedIds = _unitOfWork.PowerLibraryRepository.GetIncludeWhere(x =>
                        //    LibraryPropsAttributeFilters.All(z =>
                        //        (LibraryProps.Exists(y => (z.key.ToLower() == y.Name.ToLower()) && ((y.GetValue(_mapper.Map<PowerLibraryViewModel>(x), null) != null) ?
                        //            ((z.DateFrom >= Convert.ToDateTime(y.GetValue(_mapper.Map<PowerLibraryViewModel>(x), null))) &&
                        //             (z.DateTo <= Convert.ToDateTime(y.GetValue(_mapper.Map<PowerLibraryViewModel>(x), null)))) : (false)))))
                        //).Select(i => i.Id).ToList();

                        IEnumerable<TLIpowerLibrary> Libraries = _unitOfWork.PowerLibraryRepository.GetWhere(x => !x.Deleted).AsEnumerable();

                        foreach (DateFilterViewModel LibraryProp in LibraryPropsAttributeFilters)
                        {
                            Libraries = Libraries.Where(x => LibraryProps.Exists(y => (LibraryProp.key.ToLower() == y.Name.ToLower()) && ((y.GetValue(_mapper.Map<PowerLibraryViewModel>(x), null) != null) ?
                                ((LibraryProp.DateFrom >= Convert.ToDateTime(y.GetValue(_mapper.Map<PowerLibraryViewModel>(x), null))) &&
                                    (LibraryProp.DateTo <= Convert.ToDateTime(y.GetValue(_mapper.Map<PowerLibraryViewModel>(x), null)))) : (false))));
                        }

                        LibraryAttributeActivatedIds = Libraries.Select(x => x.Id).ToList();
                    }

                    //
                    // Library (Attribute Activated + Dynamic) Attributes...
                    //
                    List<int> IntersectLibraryIds = new List<int>();
                    if (AttrLibExist && DynamicLibExist)
                    {
                        IntersectLibraryIds = LibraryAttributeActivatedIds.Intersect(DynamicLibValueListIds).ToList();
                    }
                    else if (AttrLibExist)
                    {
                        IntersectLibraryIds = LibraryAttributeActivatedIds;
                    }
                    else if (DynamicLibExist)
                    {
                        IntersectLibraryIds = DynamicLibValueListIds;
                    }

                    WithDateFilterPowersLibraries = _mapper.Map<List<PowerLibraryViewModel>>(_unitOfWork.PowerLibraryRepository.GetIncludeWhere(x =>
                        x.Id > 0 && IntersectLibraryIds.Contains(x.Id) && !x.Deleted).ToList());
                }

                //
                // Intersect Between WithoutDateFilterPowersLibraries + WithDateFilterPowersLibraries To Get The Records That Meet The Filters (DateFilters + AttributeFilters)
                //
                if ((AttributeFilters != null ? AttributeFilters.Count() == 0 : true) &&
                    (AfterConvertDateFilters != null ? AfterConvertDateFilters.Count() == 0 : true))
                {
                    PowersLibraries = _mapper.Map<List<PowerLibraryViewModel>>(_unitOfWork.PowerLibraryRepository.GetIncludeWhere(x =>
                        x.Id > 0 && !x.Deleted).ToList());
                }
                else if ((AttributeFilters != null ? AttributeFilters.Count > 0 : false) &&
                        (AfterConvertDateFilters != null ? AfterConvertDateFilters.Count() > 0 : false))
                {
                    List<int> PowerIds = WithoutDateFilterPowersLibraries.Select(x => x.Id).Intersect(WithDateFilterPowersLibraries.Select(x => x.Id)).ToList();
                    PowersLibraries = _mapper.Map<List<PowerLibraryViewModel>>(_unitOfWork.PowerLibraryRepository.GetWhere(x =>
                        PowerIds.Contains(x.Id)).ToList());
                }
                else if (AttributeFilters != null ? AttributeFilters.Count > 0 : false)
                {
                    PowersLibraries = WithoutDateFilterPowersLibraries;
                }
                else if (AfterConvertDateFilters != null ? AfterConvertDateFilters.Count() > 0 : false)
                {
                    PowersLibraries = WithDateFilterPowersLibraries;
                }

                Count = PowersLibraries.Count();

                PowersLibraries = PowersLibraries.Skip((parameterPagination.PageNumber - 1) * parameterPagination.PageSize).
                    Take(parameterPagination.PageSize).ToList();

                List<TLIattributeViewManagment> AllAttributes = _unitOfWork.AttributeViewManagmentRepository.GetIncludeWhere(x =>
                   (x.Enable && x.EditableManagmentView.View == Helpers.Constants.EditableManamgmantViewNames.PowerLibrary.ToString() &&
                   (x.AttributeActivatedId != null ?
                        (x.AttributeActivated.Tabel == Helpers.Constants.TablesNames.TLIpowerLibrary.ToString() && x.AttributeActivated.enable) :
                        (x.DynamicAtt.LibraryAtt && !x.DynamicAtt.disable && x.DynamicAtt.tablesNames.TableName == Helpers.Constants.TablesNames.TLIpowerLibrary.ToString()))) ||
                    (x.AttributeActivated != null ?
                        ((x.AttributeActivated.Key.ToLower() == "id" || x.AttributeActivated.Key.ToLower() == "active") && x.AttributeActivated.Tabel == Helpers.Constants.TablesNames.TLIpowerLibrary.ToString()) : false),
                       x => x.EditableManagmentView, x => x.EditableManagmentView.TLItablesNames1, x => x.EditableManagmentView.TLItablesNames2,
                       x => x.AttributeActivated, x => x.DynamicAtt, x => x.DynamicAtt.tablesNames, x => x.DynamicAtt.DataType).ToList();

                List<TLIattributeViewManagment> NotDateTimeLibraryAttributesViewModel = AllAttributes.Where(x =>
                    x.AttributeActivatedId != null ? (x.AttributeActivated.Key.ToLower() != "deleted" && x.AttributeActivated.DataType.ToLower() != "datetime") : false).ToList();

                List<TLIattributeViewManagment> NotDateTimeDynamicLibraryAttributesViewModel = AllAttributes.Where(x =>
                    x.DynamicAttId != null ? x.DynamicAtt.DataType.Name.ToLower() != "datetime" : false).ToList();

                List<TLIattributeViewManagment> DateTimeLibraryAttributesViewModel = AllAttributes.Where(x =>
                    x.AttributeActivatedId != null ? (x.AttributeActivated.Key.ToLower() != "deleted" && x.AttributeActivated.DataType.ToLower() == "datetime") : false).ToList();

                List<TLIattributeViewManagment> DateTimeDynamicLibraryAttributesViewModel = AllAttributes.Where(x =>
                    x.DynamicAttId != null ? x.DynamicAtt.DataType.Name.ToLower() == "datetime" : false).ToList();

                foreach (PowerLibraryViewModel PowerLibraryViewModel in PowersLibraries)
                {
                    dynamic DynamicPowerLibrary = new ExpandoObject();

                    //
                    // Library Object ViewModel... (Not DateTime DataType Attribute)
                    //
                    if (NotDateTimeLibraryAttributesViewModel != null ? NotDateTimeLibraryAttributesViewModel.Count > 0 : false)
                    {
                        List<PropertyInfo> LibraryProps = typeof(PowerLibraryViewModel).GetProperties().Where(x =>
                            x.PropertyType.GenericTypeArguments != null ?
                                (x.PropertyType.GenericTypeArguments.Count() > 0 ? x.PropertyType.GenericTypeArguments.FirstOrDefault().Name.ToLower() != "datetime" :
                                (x.PropertyType.Name.ToLower() != "datetime")) :
                            (x.PropertyType.Name.ToLower() != "datetime")).ToList();

                        foreach (PropertyInfo prop in LibraryProps)
                        {
                            if (prop.Name.ToLower().Contains("_name") &&
                                NotDateTimeLibraryAttributesViewModel.Select(x =>
                                    x.AttributeActivated.Label.ToLower()).Contains(prop.Name.ToLower()))
                            {
                                object ForeignKeyNamePropObject = prop.GetValue(PowerLibraryViewModel, null);
                                ((IDictionary<String, Object>)DynamicPowerLibrary).Add(new KeyValuePair<string, object>(prop.Name, ForeignKeyNamePropObject));
                            }
                            else if (NotDateTimeLibraryAttributesViewModel.Select(x =>
                                 x.AttributeActivated.Key.ToLower()).Contains(prop.Name.ToLower()) &&
                                !prop.Name.ToLower().Contains("_name") &&
                                (prop.Name.ToLower().Substring(Math.Max(0, prop.Name.Length - 2)) != "id" || prop.Name.ToLower() == "id"))
                            {
                                if (prop.Name.ToLower() != "id" && prop.Name.ToLower() != "active")
                                {
                                    TLIattributeViewManagment LabelName = AllAttributes.FirstOrDefault(x => ((x.AttributeActivated != null) ? x.AttributeActivated.Key == prop.Name : false) &&
                                        x.AttributeActivated.Tabel == Helpers.Constants.TablesNames.TLIpowerLibrary.ToString() &&
                                        x.Enable && x.AttributeActivated.DataType != "List" && x.Id != 0);

                                    if (LabelName != null)
                                    {
                                        object PropObject = prop.GetValue(PowerLibraryViewModel, null);
                                        ((IDictionary<String, Object>)DynamicPowerLibrary).Add(new KeyValuePair<string, object>(LabelName.AttributeActivated.Label, PropObject));
                                    }
                                }
                                else
                                {
                                    object PropObject = prop.GetValue(PowerLibraryViewModel, null);
                                    ((IDictionary<String, Object>)DynamicPowerLibrary).Add(new KeyValuePair<string, object>(prop.Name, PropObject));
                                }
                            }
                        }
                    }

                    //
                    // Library Dynamic Attributes... (Not DateTime DataType Attribute)
                    // 
                    var temp= NotDateTimeDynamicLibraryAttributesViewModel.Select(x=>x.DynamicAttId).ToList();
                    List<TLIdynamicAtt> NotDateTimeLibraryDynamicAttributes = _unitOfWork.DynamicAttRepository.GetIncludeWhere(x =>
                       !x.disable && x.tablesNames.TableName == Helpers.Constants.TablesNames.TLIpowerLibrary.ToString() &&
                        x.LibraryAtt && x.DataType.Name.ToLower() != "datetime" &&
                        temp.Any(y => y == x.Id), x => x.tablesNames, x => x.DataType).ToList();

                    foreach (var LibraryDynamicAtt in NotDateTimeLibraryDynamicAttributes)
                    {
                        TLIdynamicAttLibValue DynamicAttLibValue = _unitOfWork.DynamicAttLibRepository.GetIncludeWhereFirst(x =>
                            x.DynamicAttId == LibraryDynamicAtt.Id &&
                            x.InventoryId == PowerLibraryViewModel.Id && !x.disable &&
                            x.DynamicAtt.LibraryAtt &&
                            x.DynamicAtt.Key == LibraryDynamicAtt.Key,
                                x => x.DynamicAtt, x => x.tablesNames, x => x.DynamicAtt.DataType);

                        if (DynamicAttLibValue != null)
                        {
                            dynamic DynamicAttValue = new ExpandoObject();

                            if (DynamicAttLibValue.ValueString != null)
                                DynamicAttValue = DynamicAttLibValue.ValueString;

                            else if (DynamicAttLibValue.ValueDouble != null)
                                DynamicAttValue = DynamicAttLibValue.ValueDouble;

                            else if (DynamicAttLibValue.ValueDateTime != null)
                                DynamicAttValue = DynamicAttLibValue.ValueDateTime;

                            else if (DynamicAttLibValue.ValueBoolean != null)
                                DynamicAttValue = DynamicAttLibValue.ValueBoolean;

                            ((IDictionary<String, Object>)DynamicPowerLibrary).Add(new KeyValuePair<string, object>(LibraryDynamicAtt.Key, DynamicAttValue));
                        }
                        else
                        {
                            ((IDictionary<String, Object>)DynamicPowerLibrary).Add(new KeyValuePair<string, object>(LibraryDynamicAtt.Key, null));
                        }
                    }

                    //
                    // Library Object ViewModel... (DateTime DataType Attribute)
                    //
                    dynamic DateTimeAttributes = new ExpandoObject();
                    if (DateTimeLibraryAttributesViewModel != null ? DateTimeLibraryAttributesViewModel.Count() > 0 : false)
                    {
                        List<PropertyInfo> DateTimeLibraryProps = typeof(PowerLibraryViewModel).GetProperties().Where(x =>
                            x.PropertyType.GenericTypeArguments != null ?
                                (x.PropertyType.GenericTypeArguments.Count() > 0 ? x.PropertyType.GenericTypeArguments.FirstOrDefault().Name.ToLower() == "datetime" :
                                (x.PropertyType.Name.ToLower() == "datetime")) :
                            (x.PropertyType.Name.ToLower() == "datetime")).ToList();

                        foreach (PropertyInfo prop in DateTimeLibraryProps)
                        {
                            TLIattributeViewManagment LabelName = AllAttributes.FirstOrDefault(x => ((x.AttributeActivated != null) ? x.AttributeActivated.Key == prop.Name : false) &&
                                x.AttributeActivated.Tabel == Helpers.Constants.TablesNames.TLIpowerLibrary.ToString() &&
                                x.Enable && x.AttributeActivated.DataType != "List" && x.Id != 0);

                            if (LabelName != null)
                            {
                                object PropObject = prop.GetValue(PowerLibraryViewModel, null);
                                ((IDictionary<String, Object>)DateTimeAttributes).Add(new KeyValuePair<string, object>(LabelName.AttributeActivated.Label, PropObject));
                            }
                        }
                    }

                    //
                    // Library Dynamic Attributes... (DateTime DataType Attribute)
                    // 
                    temp= DateTimeDynamicLibraryAttributesViewModel.Select(x=>x.DynamicAttId).ToList();
                    List<TLIdynamicAtt> LibraryDynamicAttributes = _unitOfWork.DynamicAttRepository.GetIncludeWhere(x =>
                       !x.disable && x.tablesNames.TableName == Helpers.Constants.TablesNames.TLIpowerLibrary.ToString() &&
                        x.LibraryAtt && x.DataType.Name.ToLower() == "datetime" &&
                        temp.Any(y => y == x.Id), x => x.tablesNames).ToList();

                    foreach (TLIdynamicAtt LibraryDynamicAtt in LibraryDynamicAttributes)
                    {
                        TLIdynamicAttLibValue DynamicAttLibValue = _unitOfWork.DynamicAttLibRepository.GetIncludeWhereFirst(x =>
                            x.DynamicAttId == LibraryDynamicAtt.Id &&
                            x.InventoryId == PowerLibraryViewModel.Id && !x.disable &&
                            x.DynamicAtt.LibraryAtt &&
                            x.DynamicAtt.Key == LibraryDynamicAtt.Key,
                                x => x.DynamicAtt, x => x.tablesNames, x => x.DynamicAtt.DataType);

                        if (DynamicAttLibValue != null)
                        {
                            dynamic DynamicAttValue = new ExpandoObject();
                            if (DynamicAttLibValue.ValueDateTime != null)
                                DynamicAttValue = DynamicAttLibValue.ValueDateTime;

                            ((IDictionary<String, Object>)DateTimeAttributes).Add(new KeyValuePair<string, object>(LibraryDynamicAtt.Key, DynamicAttValue));
                        }
                        else
                        {
                            ((IDictionary<String, Object>)DateTimeAttributes).Add(new KeyValuePair<string, object>(LibraryDynamicAtt.Key, null));
                        }
                    }

                    ((IDictionary<String, Object>)DynamicPowerLibrary).Add(new KeyValuePair<string, object>("DateTimeAttributes", DateTimeAttributes));

                    OutPutList.Add(DynamicPowerLibrary);
                }

                PowerTableDisplay.Model = OutPutList;

                PowerTableDisplay.filters = _unitOfWork.PowerLibraryRepository.GetRelatedTables();

                return new Response<ReturnWithFilters<object>>(true, PowerTableDisplay, null, null, (int)Helpers.Constants.ApiReturnCode.success, Count);
            }
            catch (Exception err)
            {
                return new Response<ReturnWithFilters<object>>(false, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
        }
        #endregion
        //Function take 2 parameters Id, TableName
        //get table name Entity by TableName
        //get record by Id
        //get activated attributes and values
        //get dynamic attributes
        public Response<GetForAddCivilLibrarybject> GetById(int Id, string TableName)
        {
            try
            {
                GetForAddCivilLibrarybject attributes = new GetForAddCivilLibrarybject();
                TLItablesNames TableNameEntity = _unitOfWork.TablesNamesRepository.GetWhereFirst(c =>
                   c.TableName == "TLIpowerLibrary");
                TLIpowerLibrary PowerLibrary = _unitOfWork.PowerLibraryRepository.GetIncludeWhereFirst(x =>
                        x.Id == Id && x.Active && !x.Deleted);
                if (PowerLibrary != null)
                {
                    List<BaseInstAttViews> listofAttributesActivated = _unitOfWork.AttributeActivatedRepository.GetAttributeActivatedGetForAdd(TableName, PowerLibrary, null).ToList();
                    attributes.LogisticalItems = _unitOfWork.LogistcalRepository.GetLogisticals(Helpers.Constants.TablePartName.Power.ToString(), TableName, Id);
                    attributes.AttributesActivatedLibrary = listofAttributesActivated;
                    attributes.DynamicAttributes = _unitOfWork.DynamicAttLibRepository.GetDynamicLibAtt(TableNameEntity.Id, Id, null);
                    List<BaseInstAttViews> Test = attributes.AttributesActivatedLibrary.ToList();
                    BaseInstAttViews NameAttribute = Test.FirstOrDefault(x => x.Key.ToLower() == "Model".ToLower());
                    if (NameAttribute != null)
                    {
                        BaseInstAttViews Swap = Test.ToList()[0];
                        Test[Test.IndexOf(NameAttribute)] = Swap;
                        Test[0] = NameAttribute;
                        attributes.AttributesActivatedLibrary = Test;
                        NameAttribute.Value = db.MV_POWER_LIBRARY_VIEW.FirstOrDefault(x => x.Id == Id)?.Model;
                    }
                }
                else
                {
                    return new Response<GetForAddCivilLibrarybject>(false, null, null, "this Power is not  found", (int)Helpers.Constants.ApiReturnCode.fail);
                }
                return new Response<GetForAddCivilLibrarybject>(true, attributes, null, null, (int)Helpers.Constants.ApiReturnCode.success);
            }
            catch (Exception err)
            {
                return new Response<GetForAddCivilLibrarybject>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
        }
        //Function take 2 parameters TableName, PowerLibraryViewModel
        //get table name Entity by TableName
        //Map from ViewModel to Entity
        //Check validation
        //Add Entity
        //Add dynamic attributes
        public Response<AddPowerLibraryObject> AddPowerLibrary(int UserId, AddPowerLibraryObject PowerLibraryViewModel, string connectionString)
        {
            using (var con = new OracleConnection(connectionString))
            {
                con.Open();
                using (var tran = con.BeginTransaction())
                {
                    using (TransactionScope transaction = new TransactionScope())
                    {
                        try
                        {
                            string ErrorMessage = string.Empty;
                            var TableNameEntity = _unitOfWork.TablesNamesRepository.GetWhereFirst(l => l.TableName.ToLower() == LoadSubType.TLIpowerLibrary.ToString().ToLower());
                            TLIpowerLibrary PowerLibrary = _mapper.Map<TLIpowerLibrary>(PowerLibraryViewModel.AttributesActivatedLibrary);
                           
                            //string CheckDependencyValidation = CheckDependencyValidationForPower(PowerLibraryViewModel);

                            //if (!string.IsNullOrEmpty(CheckDependencyValidation))
                            //    return new Response<AddPowerLibraryObject>(true, null, null, CheckDependencyValidation, (int)ApiReturnCode.fail);

                            //string CheckGeneralValidation = CheckGeneralValidationFunctionLib(PowerLibraryViewModel.DynamicAttributes, TableNameEntity.TableName);

                            //if (!string.IsNullOrEmpty(CheckGeneralValidation))
                            //    return new Response<AddPowerLibraryObject>(true, null, null, CheckGeneralValidation, (int)ApiReturnCode.fail);
                          
                            var CheckModel = _unitOfWork.PowerLibraryRepository.GetWhereFirst(x => x.Model == PowerLibrary.Model && !x.Deleted);
                            if (CheckModel != null)
                            {
                                return new Response<AddPowerLibraryObject>(true, null, null, $"This model {PowerLibrary.Model} is already exists", (int)Helpers.Constants.ApiReturnCode.fail);
                            }
                          
                            _unitOfWork.PowerLibraryRepository.AddAsyncWithHistory(UserId, PowerLibrary);
                            _unitOfWork.SaveChanges();

                            dynamic LogisticalItemIds = new ExpandoObject();
                            LogisticalItemIds = PowerLibraryViewModel.LogisticalItems;

                            AddLogisticalItemWithCivil(UserId, LogisticalItemIds, PowerLibrary, TableNameEntity.Id);

                            if (PowerLibraryViewModel.DynamicAttributes.Count > 0)
                            {
                                _unitOfWork.DynamicAttLibRepository.AddDynamicLibAtt(UserId, PowerLibraryViewModel.DynamicAttributes, TableNameEntity.Id, PowerLibrary.Id, connectionString);
                            }
                            _unitOfWork.TablesHistoryRepository.AddHistory(PowerLibrary.Id, Helpers.Constants.HistoryType.Add.ToString().ToLower(), TablesNames.TLImwDishLibrary.ToString().ToLower());
                            transaction.Complete();
                            Task.Run(() => _unitOfWork.CivilWithLegsRepository.RefreshView(connectionString));
                            tran.Commit();
                            return new Response<AddPowerLibraryObject>();
                        }
                        catch (Exception err)
                        {

                            tran.Rollback();
                            return new Response<AddPowerLibraryObject>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
                        }
                    }
                }
            }
        }
        public Response<GetEnableAttribute> GetPowerLibrariesEnabledAtt(string ConnectionString)
        {
            using (var connection = new OracleConnection(ConnectionString))
            {
                try
                {
                    GetEnableAttribute getEnableAttribute = new GetEnableAttribute();
                    connection.Open();
                    //string storedProcedureName = "create_dynamic_pivot_withleg_library ";
                    //using (OracleCommand procedureCommand = new OracleCommand(storedProcedureName, connection))
                    //{
                    //    procedureCommand.CommandType = CommandType.StoredProcedure;
                    //    procedureCommand.ExecuteNonQuery();
                    //}
                    var attActivated = db.TLIattributeViewManagment
                        .Include(x => x.EditableManagmentView)
                        .Include(x => x.AttributeActivated)
                        .Include(x => x.DynamicAtt)
                        .Where(x => x.Enable && x.EditableManagmentView.View == "PowerLibrary"
                        && ((x.AttributeActivatedId != null && x.AttributeActivated.enable) || (x.DynamicAttId != null && !x.DynamicAtt.disable)))
                        .Select(x => new { attribute = x.AttributeActivated.Key, dynamic = x.DynamicAtt.Key, dataType = x.DynamicAtt != null ? x.DynamicAtt.DataType.Name.ToString() : x.AttributeActivated.DataType.ToString() })
                          .OrderByDescending(x => x.attribute.ToLower().StartsWith("model"))
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
                    if (propertyNamesDynamic.Count == 0)
                    {
                        var query = db.MV_POWER_LIBRARY_VIEW.Where(x => !x.Deleted).AsEnumerable()
                    .Select(item => _unitOfWork.CivilWithLegsRepository.BuildDynamicSelect(item, null, propertyNamesStatic, propertyNamesDynamic));
                        int count = query.Count();

                        getEnableAttribute.Model = query;
                        return new Response<GetEnableAttribute>(true, getEnableAttribute, null, "Success", (int)Helpers.Constants.ApiReturnCode.success, count);
                    }
                    else
                    {
                        var query = db.MV_POWER_LIBRARY_VIEW.Where(x => !x.Deleted).AsEnumerable()
                    .GroupBy(x => new
                    {
                        Id = x.Id,
                        Model = x.Model,
                        Note = x.Note,
                        FrequencyRange = x.FrequencyRange,
                        BandWidth = x.BandWidth,
                        ChannelBandWidth = x.ChannelBandWidth,
                        Type = x.Type,
                        Active = x.Active,
                        Deleted = x.Deleted,
                        Size = x.Size,
                        L_W_H = x.L_W_H,
                        Weight = x.Weight,
                        width = x.width,
                        Length = x.Length,
                        Height = x.Height,
                        Depth = x.Depth,
                        SpaceLibrary = x.SpaceLibrary
                 

                    }).OrderBy(x => x.Key.Model)
                    .Select(x => new { key = x.Key, value = x.ToDictionary(z => z.Key, z => z.INPUTVALUE) })
                    .Select(item => _unitOfWork.CivilWithLegsRepository.BuildDynamicSelect(item.key, item.value, propertyNamesStatic, propertyNamesDynamic));
                        int count = query.Count();

                        getEnableAttribute.Model = query;
                        return new Response<GetEnableAttribute>(true, getEnableAttribute, null, "Success", (int)Helpers.Constants.ApiReturnCode.success, count);
                    }

                }
                catch (Exception err)
                {
                    return new Response<GetEnableAttribute>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
                }
            }
        }
        public void AddLogisticalItemWithCivil(int UserId, dynamic LogisticalItemIds, dynamic MWLibraryEntity, int TableNameEntityId)
        {
            using (TransactionScope transaction = new TransactionScope())
            {
                try
                {
                    if (LogisticalItemIds != null)
                    {
                        if (LogisticalItemIds.Vendor != null && LogisticalItemIds.Vendor != 0)
                        {
                            TLIlogistical LogisticalObject = _unitOfWork.LogistcalRepository.GetByID(LogisticalItemIds.Vendor);
                            if (LogisticalObject != null)
                            {
                                TLIlogisticalitem NewLogisticalItem = new TLIlogisticalitem
                                {
                                    Name = "",
                                    IsLib = true,
                                    logisticalId = LogisticalObject.Id,
                                    RecordId = MWLibraryEntity.Id,
                                    tablesNamesId = TableNameEntityId
                                };
                                _unitOfWork.LogisticalitemRepository.AddAsyncWithHistory(UserId, NewLogisticalItem);
                                _unitOfWork.SaveChangesAsync();
                            }
                        }
                        if (LogisticalItemIds.Supplier != null && LogisticalItemIds.Supplier != 0)
                        {
                            TLIlogistical LogisticalObject = _unitOfWork.LogistcalRepository.GetByID(LogisticalItemIds.Supplier);
                            if (LogisticalObject != null)
                            {
                                TLIlogisticalitem NewLogisticalItem = new TLIlogisticalitem
                                {
                                    Name = "",
                                    IsLib = true,
                                    logisticalId = LogisticalObject.Id,
                                    RecordId = MWLibraryEntity.Id,
                                    tablesNamesId = TableNameEntityId
                                };
                                _unitOfWork.LogisticalitemRepository.AddAsyncWithHistory(UserId, NewLogisticalItem);
                                _unitOfWork.SaveChangesAsync();
                            }
                        }
                        if (LogisticalItemIds.Designer != null && LogisticalItemIds.Designer != 0)
                        {
                            TLIlogistical LogisticalObject = _unitOfWork.LogistcalRepository.GetByID(LogisticalItemIds.Designer);
                            if (LogisticalObject != null)
                            {
                                TLIlogisticalitem NewLogisticalItem = new TLIlogisticalitem
                                {
                                    Name = "",
                                    IsLib = true,
                                    logisticalId = LogisticalObject.Id,
                                    RecordId = MWLibraryEntity.Id,
                                    tablesNamesId = TableNameEntityId
                                };
                                _unitOfWork.LogisticalitemRepository.AddAsyncWithHistory(UserId, NewLogisticalItem);
                                _unitOfWork.SaveChangesAsync();
                            }
                        }
                        if (LogisticalItemIds.Manufacturer != null && LogisticalItemIds.Manufacturer != 0)
                        {
                            TLIlogistical LogisticalObject = _unitOfWork.LogistcalRepository.GetByID(LogisticalItemIds.Manufacturer);
                            if (LogisticalObject != null)
                            {
                                TLIlogisticalitem NewLogisticalItem = new TLIlogisticalitem
                                {
                                    Name = "",
                                    IsLib = true,
                                    logisticalId = LogisticalObject.Id,
                                    RecordId = MWLibraryEntity.Id,
                                    tablesNamesId = TableNameEntityId
                                };
                                _unitOfWork.LogisticalitemRepository.AddAsyncWithHistory(UserId, NewLogisticalItem);
                                _unitOfWork.SaveChangesAsync();
                            }
                        }
                        if (LogisticalItemIds.Contractor != null && LogisticalItemIds.Contractor != 0)
                        {
                            TLIlogistical LogisticalObject = _unitOfWork.LogistcalRepository.GetByID(LogisticalItemIds.Contractor);
                            if (LogisticalObject != null)
                            {
                                TLIlogisticalitem NewLogisticalItem = new TLIlogisticalitem
                                {
                                    Name = "",
                                    IsLib = true,
                                    logisticalId = LogisticalObject.Id,
                                    RecordId = MWLibraryEntity.Id,
                                    tablesNamesId = TableNameEntityId
                                };
                                _unitOfWork.LogisticalitemRepository.AddAsyncWithHistory(UserId, NewLogisticalItem);
                                _unitOfWork.SaveChangesAsync();
                            }

                        }
                        if (LogisticalItemIds.Consultant != null && LogisticalItemIds.Consultant != 0)
                        {
                            TLIlogistical LogisticalObject = _unitOfWork.LogistcalRepository.GetByID(LogisticalItemIds.Consultant);
                            if (LogisticalObject != null)
                            {
                                TLIlogisticalitem NewLogisticalItem = new TLIlogisticalitem
                                {
                                    Name = "",
                                    IsLib = true,
                                    logisticalId = LogisticalObject.Id,
                                    RecordId = MWLibraryEntity.Id,
                                    tablesNamesId = TableNameEntityId
                                };
                                _unitOfWork.LogisticalitemRepository.AddAsyncWithHistory(UserId, NewLogisticalItem);
                                _unitOfWork.SaveChangesAsync();
                            }
                        }
                    }

                    transaction.Complete();
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
        public async Task<Response<EditPowerLibraryObject>> EditPowerLibrary(int userId, EditPowerLibraryObject editPowerLibraryObject, string TableName, string connectionString)
        {
            using (TransactionScope transaction =
                new TransactionScope(TransactionScopeOption.Required,
                                   new System.TimeSpan(0, 15, 0)))
            {
                try
                {

                    int resultId = 0;

                    TLItablesNames TableNameEntity = _unitOfWork.TablesNamesRepository.GetWhereFirst(c => c.TableName == TableName);

                    TLIpowerLibrary PowerLibraryEntites = _mapper.Map<TLIpowerLibrary>(editPowerLibraryObject.AttributesActivatedLibrary);

                    TLIpowerLibrary PowerLegLib = _unitOfWork.PowerLibraryRepository.GetAllAsQueryable().AsNoTracking().FirstOrDefault(x => x.Id == PowerLibraryEntites.Id);


                    if (PowerLibraryEntites.SpaceLibrary == 0)
                    {
                        if (PowerLibraryEntites.Height <= 0)
                        {
                            return new Response<EditPowerLibraryObject>(false, null, null, "Height It must be greater than zero", (int)Helpers.Constants.ApiReturnCode.fail);
                        }
                        else
                        {
                            PowerLibraryEntites.SpaceLibrary = PowerLibraryEntites.Height * PowerLibraryEntites.width;
                        }

                    }
                    if (_unitOfWork.MW_DishLibraryRepository.GetWhereFirst(x => x.Model == PowerLibraryEntites.Model && x.Id != PowerLibraryEntites.Id && !x.Deleted) != null)
                    {
                        return new Response<EditPowerLibraryObject>(false, null, null, $"This model {PowerLibraryEntites.Model} is already exists", (int)Helpers.Constants.ApiReturnCode.fail);
                    }


                    _unitOfWork.PowerLibraryRepository.UpdateWithHistory(userId, PowerLegLib, PowerLibraryEntites);


                    //string CheckDependencyValidation = CheckDependencyValidationForCivilTypesEditApiVersions(editCivilWithLegsLibrary, TableName);
                    //if (!string.IsNullOrEmpty(CheckDependencyValidation))
                    //{
                    //    return new Response<EditCivilWithLegsLibraryObject>(true, null, null, CheckDependencyValidation, (int)Helpers.Constants.ApiReturnCode.fail);
                    //}

                    //string CheckGeneralValidation = CheckGeneralValidationFunctionEditApiVersions(editCivilWithLegsLibrary.dynamicAttributes, TableNameEntity.TableName);
                    //if (!string.IsNullOrEmpty(CheckGeneralValidation))
                    //{
                    //    return new Response<EditCivilWithLegsLibraryObject>(true, null, null, CheckGeneralValidation, (int)Helpers.Constants.ApiReturnCode.fail);
                    //}

                    AddLogisticalViewModel OldLogisticalItemIds = new AddLogisticalViewModel();

                    var CheckVendorId = _unitOfWork.LogisticalitemRepository
                        .GetIncludeWhereFirst(x => x.logistical.logisticalType.Name.ToLower() == Helpers.Constants.LogisticalType.Vendor.ToString().ToLower() &&
                            x.IsLib && x.tablesNamesId == TableNameEntity.Id && x.RecordId == PowerLibraryEntites.Id, x => x.logistical,
                                x => x.logistical.logisticalType);

                    if (CheckVendorId != null)
                        OldLogisticalItemIds.Vendor = Convert.ToInt32(CheckVendorId.logisticalId);

                    var CheckSupplierId = _unitOfWork.LogisticalitemRepository
                        .GetIncludeWhereFirst(x => x.logistical.logisticalType.Name.ToLower() == Helpers.Constants.LogisticalType.Supplier.ToString().ToLower() &&
                            x.IsLib && x.tablesNamesId == TableNameEntity.Id && x.RecordId == PowerLibraryEntites.Id, x => x.logistical,
                                x => x.logistical.logisticalType);

                    if (CheckSupplierId != null)
                        OldLogisticalItemIds.Supplier = CheckSupplierId.logisticalId;

                    var CheckDesignerId = _unitOfWork.LogisticalitemRepository
                        .GetIncludeWhereFirst(x => x.logistical.logisticalType.Name.ToLower() == Helpers.Constants.LogisticalType.Designer.ToString().ToLower() &&
                            x.IsLib && x.tablesNamesId == TableNameEntity.Id && x.RecordId == PowerLibraryEntites.Id, x => x.logistical,
                                x => x.logistical.logisticalType);

                    if (CheckDesignerId != null)
                        OldLogisticalItemIds.Designer = CheckDesignerId.logisticalId;


                    var CheckManufacturerId = _unitOfWork.LogisticalitemRepository
                        .GetIncludeWhereFirst(x => x.logistical.logisticalType.Name.ToLower() == Helpers.Constants.LogisticalType.Manufacturer.ToString().ToLower() &&
                            x.IsLib && x.tablesNamesId == TableNameEntity.Id && x.RecordId == PowerLibraryEntites.Id, x => x.logistical,
                                x => x.logistical.logisticalType);

                    if (CheckManufacturerId != null)
                        OldLogisticalItemIds.Manufacturer = CheckManufacturerId.logisticalId;


                    var CheckContractorId = _unitOfWork.LogisticalitemRepository
                 .GetIncludeWhereFirst(x => x.logistical.logisticalType.Name.ToLower() == Helpers.Constants.LogisticalType.Contractor.ToString().ToLower() &&
                     x.IsLib && x.tablesNamesId == TableNameEntity.Id && x.RecordId == PowerLibraryEntites.Id, x => x.logistical,
                         x => x.logistical.logisticalType);

                    if (CheckContractorId != null)
                        OldLogisticalItemIds.Contractor = CheckContractorId.logisticalId;


                    var CheckConsultantId = _unitOfWork.LogisticalitemRepository
                       .GetIncludeWhereFirst(x => x.logistical.logisticalType.Name.ToLower() == Helpers.Constants.LogisticalType.Consultant.ToString().ToLower() &&
                           x.IsLib && x.tablesNamesId == TableNameEntity.Id && x.RecordId == PowerLibraryEntites.Id, x => x.logistical,
                               x => x.logistical.logisticalType);

                    if (CheckConsultantId != null)
                        OldLogisticalItemIds.Consultant = CheckConsultantId.logisticalId;


                    EditLogisticalItemss(userId, editPowerLibraryObject.LogisticalItems, PowerLibraryEntites, TableNameEntity.Id, OldLogisticalItemIds);

                    if (editPowerLibraryObject.DynamicAttributes != null ? editPowerLibraryObject.DynamicAttributes.Count > 0 : false)
                    {
                        _unitOfWork.DynamicAttLibRepository.UpdateDynamicLibAttsWithHistorys(editPowerLibraryObject.DynamicAttributes, connectionString, TableNameEntity.Id, PowerLibraryEntites.Id, userId, resultId, PowerLegLib.Id);
                    }

                    await _unitOfWork.SaveChangesAsync();


                    transaction.Complete();
                    Task.Run(() => _unitOfWork.CivilWithLegsRepository.RefreshView(connectionString));
                    return new Response<EditPowerLibraryObject>(true, null, null, null, (int)Helpers.Constants.ApiReturnCode.success);
                }
                catch (Exception err)
                {
                    return new Response<EditPowerLibraryObject>(false, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
                }
            }

        }
        public void EditLogisticalItemss(int UserId, AddLogisticalViewModel LogisticalItemIds, dynamic MainEntity, int TableNameEntityId, AddLogisticalViewModel OldLogisticalItemIds)
        {
            using (TransactionScope transaction2 =
                new TransactionScope(TransactionScopeOption.Required,
                                   new System.TimeSpan(0, 15, 0)))
            {
                try
                {
                    if (LogisticalItemIds != null)
                    {
                        if (LogisticalItemIds.Vendor != null && LogisticalItemIds.Vendor != 0)
                        {
                            if (OldLogisticalItemIds.Vendor != null ? OldLogisticalItemIds.Vendor != 0 : false)
                            {
                                TLIlogistical OldLogisticalObject = _unitOfWork.LogistcalRepository
                                    .GetByID(OldLogisticalItemIds.Vendor);

                                int CivilId = MainEntity.Id;

                                var OldValueMWRFU = db.TLIlogisticalitem.AsNoTracking().FirstOrDefault(x => x.logisticalId == OldLogisticalObject.Id
                                && x.IsLib && x.RecordId == CivilId &&
                                        x.tablesNamesId == TableNameEntityId);

                                TLIlogisticalitem? LogisticalItem = _unitOfWork.LogisticalitemRepository
                                    .GetWhereFirst(x => x.logisticalId == OldLogisticalObject.Id && x.IsLib && x.RecordId == CivilId &&
                                        x.tablesNamesId == TableNameEntityId);

                                if (LogisticalItem != null)
                                {
                                    LogisticalItem.logisticalId = LogisticalItemIds.Vendor;

                                    _unitOfWork.LogisticalitemRepository.UpdateWithHistory(UserId, OldValueMWRFU, LogisticalItem);
                                    _unitOfWork.SaveChangesAsync();
                                }
                                else
                                {
                                    TLIlogisticalitem NewLogisticalItem = new TLIlogisticalitem()
                                    {
                                        IsLib = true,
                                        logisticalId = LogisticalItemIds.Vendor,
                                        Name = OldLogisticalObject.Name,
                                        RecordId = CivilId,
                                        tablesNamesId = TableNameEntityId
                                    };

                                    _unitOfWork.LogisticalitemRepository.AddWithHistory(UserId, NewLogisticalItem);
                                    _unitOfWork.SaveChangesAsync();
                                }
                            }
                            else
                            {
                                TLIlogistical LogisticalObject = _unitOfWork.LogistcalRepository
                                    .GetByID(LogisticalItemIds.Vendor);

                                TLIlogisticalitem NewLogisticalItem = new TLIlogisticalitem
                                {
                                    Name = "",
                                    IsLib = true,
                                    logisticalId = LogisticalObject.Id,
                                    RecordId = MainEntity.Id,
                                    tablesNamesId = TableNameEntityId
                                };
                                _unitOfWork.LogisticalitemRepository.AddAsyncWithHistory(UserId, NewLogisticalItem);
                                _unitOfWork.SaveChangesAsync();
                            }
                        }
                        if (LogisticalItemIds.Supplier != null && LogisticalItemIds.Supplier != 0)
                        {
                            if (OldLogisticalItemIds.Supplier != null ? OldLogisticalItemIds.Supplier != 0 : false)
                            {
                                TLIlogistical OldLogisticalObject = db.TLIlogistical
                                    .FirstOrDefault(x => x.Id == OldLogisticalItemIds.Supplier);

                                int CivilId = MainEntity.Id;

                                var OldValueMWRFU = db.TLIlogisticalitem.AsNoTracking().FirstOrDefault(x => x.logisticalId == OldLogisticalObject.Id
                                && x.IsLib && x.RecordId == CivilId &&
                                        x.tablesNamesId == TableNameEntityId);

                                TLIlogisticalitem? LogisticalItem = _unitOfWork.LogisticalitemRepository
                                    .GetWhereFirst(x => x.logisticalId == OldLogisticalObject.Id && x.IsLib && x.RecordId == CivilId &&
                                        x.tablesNamesId == TableNameEntityId);

                                if (LogisticalItem != null)
                                {
                                    LogisticalItem.logisticalId = LogisticalItemIds.Supplier;

                                    _unitOfWork.LogisticalitemRepository.UpdateWithHistory(UserId, OldValueMWRFU, LogisticalItem);
                                    _unitOfWork.SaveChangesAsync();
                                }
                                else
                                {
                                    TLIlogisticalitem NewLogisticalItem = new TLIlogisticalitem()
                                    {
                                        IsLib = true,
                                        logisticalId = LogisticalItemIds.Supplier,
                                        Name = OldLogisticalObject.Name,
                                        RecordId = CivilId,
                                        tablesNamesId = TableNameEntityId
                                    };

                                    _unitOfWork.LogisticalitemRepository.AddWithHistory(UserId, NewLogisticalItem);
                                    _unitOfWork.SaveChangesAsync();
                                }
                            }
                            else
                            {
                                TLIlogistical LogisticalObject = db.TLIlogistical.FirstOrDefault(x => x.Id == LogisticalItemIds.Supplier);
                                if (LogisticalObject != null)
                                {
                                    TLIlogisticalitem NewLogisticalItem = new TLIlogisticalitem
                                    {
                                        Name = "",
                                        IsLib = true,
                                        logisticalId = LogisticalObject.Id,
                                        RecordId = MainEntity.Id,
                                        tablesNamesId = TableNameEntityId
                                    };
                                    _unitOfWork.LogisticalitemRepository.AddAsyncWithHistory(UserId, NewLogisticalItem);
                                    _unitOfWork.SaveChangesAsync();
                                }
                            }
                        }
                        if (LogisticalItemIds.Designer != null && LogisticalItemIds.Designer != 0)
                        {
                            if (OldLogisticalItemIds.Designer != null ? OldLogisticalItemIds.Designer != 0 : false)
                            {
                                TLIlogistical OldLogisticalObject = db.TLIlogistical
                                    .FirstOrDefault(x => x.Id == OldLogisticalItemIds.Designer);

                                int CivilId = MainEntity.Id;

                                var OldValueMWRFU = db.TLIlogisticalitem.AsNoTracking().FirstOrDefault(x => x.logisticalId == OldLogisticalObject.Id
                                && x.IsLib && x.RecordId == CivilId &&
                                        x.tablesNamesId == TableNameEntityId);

                                TLIlogisticalitem? LogisticalItem = _unitOfWork.LogisticalitemRepository
                                    .GetWhereFirst(x => x.logisticalId == OldLogisticalObject.Id && x.IsLib && x.RecordId == CivilId &&
                                        x.tablesNamesId == TableNameEntityId);

                                if (LogisticalItem != null)
                                {
                                    LogisticalItem.logisticalId = LogisticalItemIds.Designer;

                                    _unitOfWork.LogisticalitemRepository.UpdateWithHistory(UserId, OldValueMWRFU, LogisticalItem);
                                    _unitOfWork.SaveChangesAsync();
                                }
                                else
                                {
                                    TLIlogisticalitem NewLogisticalItem = new TLIlogisticalitem()
                                    {
                                        IsLib = true,
                                        logisticalId = LogisticalItemIds.Designer,
                                        Name = OldLogisticalObject.Name,
                                        RecordId = CivilId,
                                        tablesNamesId = TableNameEntityId
                                    };

                                    _unitOfWork.LogisticalitemRepository.AddWithHistory(UserId, NewLogisticalItem);
                                    _unitOfWork.SaveChangesAsync();
                                }
                            }
                            else
                            {
                                TLIlogistical LogisticalObject = db.TLIlogistical
                                    .FirstOrDefault(x => x.Id == LogisticalItemIds.Designer);
                                if (LogisticalObject != null)
                                {
                                    TLIlogisticalitem NewLogisticalItem = new TLIlogisticalitem
                                    {
                                        Name = "",
                                        IsLib = true,
                                        logisticalId = LogisticalObject.Id,
                                        RecordId = MainEntity.Id,
                                        tablesNamesId = TableNameEntityId
                                    };
                                    _unitOfWork.LogisticalitemRepository.AddAsyncWithHistory(UserId, NewLogisticalItem);
                                    _unitOfWork.SaveChangesAsync();
                                }
                            }
                        }
                        if (LogisticalItemIds.Manufacturer != null && LogisticalItemIds.Manufacturer != 0)
                        {
                            if (OldLogisticalItemIds.Manufacturer != null ? OldLogisticalItemIds.Manufacturer != 0 : false)
                            {
                                TLIlogistical OldLogisticalObject = db.TLIlogistical
                                    .FirstOrDefault(x => x.Id == OldLogisticalItemIds.Manufacturer);

                                int CivilId = MainEntity.Id;

                                var OldValueMWRFU = db.TLIlogisticalitem.AsNoTracking().FirstOrDefault(x => x.logisticalId == OldLogisticalObject.Id &&
                                x.IsLib && x.RecordId == CivilId &&
                                        x.tablesNamesId == TableNameEntityId);

                                TLIlogisticalitem? LogisticalItem = _unitOfWork.LogisticalitemRepository
                                    .GetWhereFirst(x => x.logisticalId == OldLogisticalObject.Id && x.IsLib && x.RecordId == CivilId &&
                                        x.tablesNamesId == TableNameEntityId);

                                if (LogisticalItem != null)
                                {
                                    LogisticalItem.logisticalId = LogisticalItemIds.Manufacturer;

                                    _unitOfWork.LogisticalitemRepository.UpdateWithHistory(UserId, OldValueMWRFU, LogisticalItem);
                                    _unitOfWork.SaveChangesAsync();
                                }
                                else
                                {
                                    TLIlogisticalitem NewLogisticalItem = new TLIlogisticalitem()
                                    {
                                        IsLib = true,
                                        logisticalId = LogisticalItemIds.Manufacturer,
                                        Name = OldLogisticalObject.Name,
                                        RecordId = CivilId,
                                        tablesNamesId = TableNameEntityId
                                    };

                                    _unitOfWork.LogisticalitemRepository.AddWithHistory(UserId, NewLogisticalItem);
                                    _unitOfWork.SaveChangesAsync();
                                }
                            }
                            else
                            {
                                TLIlogistical LogisticalObject = db.TLIlogistical.FirstOrDefault(x => x.Id == LogisticalItemIds.Manufacturer);
                                if (LogisticalObject != null)
                                {
                                    TLIlogisticalitem NewLogisticalItem = new TLIlogisticalitem
                                    {
                                        Name = "",
                                        IsLib = true,
                                        logisticalId = LogisticalObject.Id,
                                        RecordId = MainEntity.Id,
                                        tablesNamesId = TableNameEntityId
                                    };
                                    _unitOfWork.LogisticalitemRepository.AddAsyncWithHistory(UserId, NewLogisticalItem);
                                    _unitOfWork.SaveChangesAsync();
                                }
                            }
                        }
                        if (LogisticalItemIds.Consultant != null && LogisticalItemIds.Consultant != 0)
                        {
                            if (OldLogisticalItemIds.Consultant != null ? OldLogisticalItemIds.Consultant != 0 : false)
                            {
                                TLIlogistical OldLogisticalObject = db.TLIlogistical
                                    .FirstOrDefault(x => x.Id == OldLogisticalItemIds.Consultant);

                                int CivilId = MainEntity.Id;

                                var OldValueMWRFU = db.TLIlogisticalitem.AsNoTracking().FirstOrDefault(x => x.logisticalId == OldLogisticalObject.Id &&
                                x.IsLib && x.RecordId == CivilId &&
                                        x.tablesNamesId == TableNameEntityId);

                                TLIlogisticalitem? LogisticalItem = _unitOfWork.LogisticalitemRepository
                                    .GetWhereFirst(x => x.logisticalId == OldLogisticalObject.Id && x.IsLib && x.RecordId == CivilId &&
                                        x.tablesNamesId == TableNameEntityId);

                                if (LogisticalItem != null)
                                {
                                    LogisticalItem.logisticalId = LogisticalItemIds.Consultant;

                                    _unitOfWork.LogisticalitemRepository.UpdateWithHistory(UserId, OldValueMWRFU, LogisticalItem);
                                    _unitOfWork.SaveChangesAsync();
                                }
                                else
                                {
                                    TLIlogisticalitem NewLogisticalItem = new TLIlogisticalitem()
                                    {
                                        IsLib = true,
                                        logisticalId = LogisticalItemIds.Manufacturer,
                                        Name = OldLogisticalObject.Name,
                                        RecordId = CivilId,
                                        tablesNamesId = TableNameEntityId
                                    };

                                    _unitOfWork.LogisticalitemRepository.AddWithHistory(UserId, NewLogisticalItem);
                                    _unitOfWork.SaveChangesAsync();
                                }
                            }
                            else
                            {
                                TLIlogistical LogisticalObject = db.TLIlogistical.FirstOrDefault(x => x.Id == LogisticalItemIds.Consultant);
                                if (LogisticalObject != null)
                                {
                                    TLIlogisticalitem NewLogisticalItem = new TLIlogisticalitem
                                    {
                                        Name = "",
                                        IsLib = true,
                                        logisticalId = LogisticalObject.Id,
                                        RecordId = MainEntity.Id,
                                        tablesNamesId = TableNameEntityId
                                    };
                                    _unitOfWork.LogisticalitemRepository.AddAsyncWithHistory(UserId, NewLogisticalItem);
                                    _unitOfWork.SaveChangesAsync();
                                }
                            }
                        }
                        if (LogisticalItemIds.Contractor != null && LogisticalItemIds.Contractor != 0)
                        {
                            if (OldLogisticalItemIds.Contractor != null ? OldLogisticalItemIds.Contractor != 0 : false)
                            {
                                TLIlogistical OldLogisticalObject = db.TLIlogistical
                                    .FirstOrDefault(x => x.Id == OldLogisticalItemIds.Contractor);

                                int CivilId = MainEntity.Id;

                                var OldValueMWRFU = db.TLIlogisticalitem.AsNoTracking().FirstOrDefault(x => x.logisticalId == OldLogisticalObject.Id &&
                                x.IsLib && x.RecordId == CivilId &&
                                        x.tablesNamesId == TableNameEntityId);

                                TLIlogisticalitem? LogisticalItem = _unitOfWork.LogisticalitemRepository
                                    .GetWhereFirst(x => x.logisticalId == OldLogisticalObject.Id && x.IsLib && x.RecordId == CivilId &&
                                        x.tablesNamesId == TableNameEntityId);

                                if (LogisticalItem != null)
                                {
                                    LogisticalItem.logisticalId = LogisticalItemIds.Contractor;

                                    _unitOfWork.LogisticalitemRepository.UpdateWithHistory(UserId, OldValueMWRFU, LogisticalItem);
                                    _unitOfWork.SaveChangesAsync();
                                }
                                else
                                {
                                    TLIlogisticalitem NewLogisticalItem = new TLIlogisticalitem()
                                    {
                                        IsLib = true,
                                        logisticalId = LogisticalItemIds.Manufacturer,
                                        Name = OldLogisticalObject.Name,
                                        RecordId = CivilId,
                                        tablesNamesId = TableNameEntityId
                                    };

                                    _unitOfWork.LogisticalitemRepository.AddWithHistory(UserId, NewLogisticalItem);
                                    _unitOfWork.SaveChangesAsync();
                                }
                            }
                            else
                            {
                                TLIlogistical LogisticalObject = db.TLIlogistical.FirstOrDefault(x => x.Id == LogisticalItemIds.Contractor);
                                if (LogisticalObject != null)
                                {
                                    TLIlogisticalitem NewLogisticalItem = new TLIlogisticalitem
                                    {
                                        Name = "",
                                        IsLib = true,
                                        logisticalId = LogisticalObject.Id,
                                        RecordId = MainEntity.Id,
                                        tablesNamesId = TableNameEntityId
                                    };
                                    _unitOfWork.LogisticalitemRepository.AddAsyncWithHistory(UserId, NewLogisticalItem);
                                    _unitOfWork.SaveChangesAsync();
                                }
                            }
                        }
                    }

                    transaction2.Complete();
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
        #region Helper Methods
        public string CheckDependencyValidationForPower(object Input)
        {
            AddPowerLibraryViewModel AddPowerViewModel = _mapper.Map<AddPowerLibraryViewModel>(Input);

            List<DynamicAttViewModel> DynamicAttributes = _mapper.Map<List<DynamicAttViewModel>>(_unitOfWork.DynamicAttRepository
                    .GetIncludeWhere(x => x.tablesNames.TableName.ToLower() == TablesNames.TLIpowerLibrary.ToString().ToLower() && !x.disable
                        , x => x.tablesNames).ToList());

            foreach (DynamicAttViewModel DynamicAttribute in DynamicAttributes)
            {
                TLIdependency Dependency = _unitOfWork.DependencieRepository.GetIncludeWhereFirst(x => x.DynamicAttId == DynamicAttribute.Id &&
                    x.OperationId != null && (x.ValueBoolean != null || x.ValueDateTime != null || x.ValueDouble != null || !string.IsNullOrEmpty(x.ValueString)),
                        x => x.Operation, x => x.DynamicAtt);

                if (Dependency != null)
                {
                    AddDynamicLibAttValueViewModel InsertedDynamicAttributeValue = AddPowerViewModel.TLIdynamicAttLibValue
                        .FirstOrDefault(x => x.DynamicAttId == DynamicAttribute.Id);

                    if (InsertedDynamicAttributeValue == null)
                        return $"({DynamicAttribute.Key}) value can't be null and must be inserted";

                    List<int> RowsIds = _unitOfWork.DependencyRowRepository.GetWhere(x => x.DependencyId == Dependency.Id && x.RowId != null).Select(x => x.RowId.Value).Distinct().ToList();

                    foreach (int RowId in RowsIds)
                    {
                        List<TLIrule> Rules = _unitOfWork.RowRuleRepository.GetIncludeWhere(x => x.RowId.Value == RowId, x => x.Rule, x => x.Rule.Operation, x => x.Rule.attributeActivated
                            , x => x.Rule.dynamicAtt).Select(x => x.Rule).Distinct().ToList();

                        int Succed = 0;

                        foreach (TLIrule Rule in Rules)
                        {
                            string RuleOperation = Rule.Operation.Name;
                            object RuleValue = new object();

                            if (Rule.OperationValueBoolean != null)
                                RuleValue = Rule.OperationValueBoolean;

                            else if (Rule.OperationValueDateTime != null)
                                RuleValue = Rule.OperationValueDateTime;

                            else if (Rule.OperationValueDouble != null)
                                RuleValue = Rule.OperationValueDouble;

                            else if (!string.IsNullOrEmpty(Rule.OperationValueString))
                                RuleValue = Rule.OperationValueString;

                            object InsertedValue = new object();

                            if (Rule.attributeActivatedId != null)
                            {
                                string AttributeName = Rule.attributeActivated.Key;

                                InsertedValue = AddPowerViewModel.GetType().GetProperties()
                                    .FirstOrDefault(x => x.Name.ToLower() == AttributeName.ToLower()).GetValue(AddPowerViewModel, null);
                            }
                            else if (Rule.dynamicAttId != null)
                            {
                                AddDynamicLibAttValueViewModel DynamicObject = AddPowerViewModel.TLIdynamicAttLibValue
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

                            if (InsertedValue == null)
                                break;

                            if (RuleOperation == "==" ? InsertedValue.ToString().ToLower() == RuleValue.ToString().ToLower() :
                                RuleOperation == "!=" ? InsertedValue.ToString().ToLower() != RuleValue.ToString().ToLower() :
                                RuleOperation == ">" ? Comparer.DefaultInvariant.Compare(InsertedValue, RuleValue) == 1 :
                                RuleOperation == ">=" ? (Comparer.DefaultInvariant.Compare(InsertedValue, RuleValue) == 1 ||
                                    InsertedValue.ToString().ToLower() == RuleValue.ToString().ToLower()) :
                                RuleOperation == "<" ? Comparer.DefaultInvariant.Compare(InsertedValue, RuleValue) == -1 :
                                RuleOperation == "<=" ? (Comparer.DefaultInvariant.Compare(InsertedValue, RuleValue) == -1 ||
                                    InsertedValue.ToString().ToLower() == RuleValue.ToString().ToLower()) : false)
                            {
                                Succed++;
                            }
                        }
                        if (Rules.Count() == Succed)
                        {
                            string DependencyValidationOperation = Dependency.Operation.Name;

                            object DependencyValdiationValue = Dependency.ValueBoolean != null ? Dependency.ValueBoolean :
                                Dependency.ValueDateTime != null ? Dependency.ValueDateTime :
                                Dependency.ValueDouble != null ? Dependency.ValueDouble :
                                !string.IsNullOrEmpty(Dependency.ValueString) ? Dependency.ValueString : null;

                            object InsertedDynamicAttributeValueAsObject = InsertedDynamicAttributeValue.ValueBoolean != null ? InsertedDynamicAttributeValue.ValueBoolean :
                                InsertedDynamicAttributeValue.ValueDateTime != null ? InsertedDynamicAttributeValue.ValueDateTime :
                                InsertedDynamicAttributeValue.ValueDouble != null ? InsertedDynamicAttributeValue.ValueDouble :
                                !string.IsNullOrEmpty(InsertedDynamicAttributeValue.ValueString) ? InsertedDynamicAttributeValue.ValueString : null;

                            if (Dependency.ValueDateTime != null && InsertedDynamicAttributeValue.ValueDateTime != null)
                            {
                                DateTime DependencyValdiationValueConverter = new DateTime(Dependency.ValueDateTime.Value.Year,
                                    Dependency.ValueDateTime.Value.Month, Dependency.ValueDateTime.Value.Day);

                                DependencyValdiationValue = DependencyValdiationValueConverter;

                                DateTime InsertedDynamicAttributeValueAsObjectConverter = new DateTime(InsertedDynamicAttributeValue.ValueDateTime.Value.Year,
                                    InsertedDynamicAttributeValue.ValueDateTime.Value.Month, InsertedDynamicAttributeValue.ValueDateTime.Value.Day);

                                InsertedDynamicAttributeValueAsObject = InsertedDynamicAttributeValueAsObjectConverter;
                            }

                            if (InsertedDynamicAttributeValueAsObject != null && DependencyValdiationValue != null)
                            {
                                if (!(DependencyValidationOperation == "==" ? InsertedDynamicAttributeValueAsObject.ToString().ToLower() == DependencyValdiationValue.ToString().ToLower() :
                                     DependencyValidationOperation == "!=" ? InsertedDynamicAttributeValueAsObject.ToString().ToLower() != DependencyValdiationValue.ToString().ToLower() :
                                     DependencyValidationOperation == ">" ? Comparer.DefaultInvariant.Compare(InsertedDynamicAttributeValueAsObject, DependencyValdiationValue) == 1 :
                                     DependencyValidationOperation == ">=" ? (InsertedDynamicAttributeValueAsObject.ToString().ToLower() == DependencyValdiationValue.ToString().ToLower() ||
                                         Comparer.DefaultInvariant.Compare(InsertedDynamicAttributeValueAsObject, DependencyValdiationValue) == 1) :
                                     DependencyValidationOperation == "<" ? Comparer.DefaultInvariant.Compare(InsertedDynamicAttributeValueAsObject, DependencyValdiationValue) == -1 :
                                     DependencyValidationOperation == "<=" ? (InsertedDynamicAttributeValueAsObject.ToString().ToLower() == DependencyValdiationValue.ToString().ToLower() ||
                                         Comparer.DefaultInvariant.Compare(InsertedDynamicAttributeValueAsObject, DependencyValdiationValue) == -1) : false))
                                {
                                    string ReturnOperation = (DependencyValidationOperation == "==" ? "Equal To" :
                                        (DependencyValidationOperation == "!=" ? "not equal to" :
                                        (DependencyValidationOperation == ">" ? "bigger than" :
                                        (DependencyValidationOperation == ">=" ? "bigger than or equal to" :
                                        (DependencyValidationOperation == "<" ? "smaller than" :
                                        (DependencyValidationOperation == "<=" ? "smaller than or equal to" : ""))))));

                                    return $"({Dependency.DynamicAtt.Key}) value must be {ReturnOperation} {DependencyValdiationValue}";
                                }
                            }
                        }
                    }
                }
            }

            return string.Empty;
        }
        public string CheckGeneralValidationFunction(List<AddDynamicLibAttValueViewModel> TLIdynamicAttLibValue, string TableName)
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
                    AddDynamicLibAttValueViewModel DynmaicAttributeValue = TLIdynamicAttLibValue.FirstOrDefault(x => x.DynamicAttId == DynamicAttributeEntity.Id);

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
        public string CheckGeneralValidationFunctionLib(List<AddDdynamicAttributeInstallationValueViewModel> TLIdynamicAttLibValue, string TableName)
        {
            List<DynamicAttViewModel> DynamicAttributes = _mapper.Map<List<DynamicAttViewModel>>(_unitOfWork.DynamicAttRepository
                .GetIncludeWhere(x => x.tablesNames.TableName.ToLower() == TableName.ToLower() && !x.disable
                    , x => x.tablesNames).ToList());


            var invalidValidation = DynamicAttributes.Select(DynamicAttributeEntity =>
            {
                var Validation = _unitOfWork.ValidationRepository
                    .GetIncludeWhereFirst(x => x.DynamicAttId == DynamicAttributeEntity.Id, x => x.Operation, x => x.DynamicAtt);

                if (Validation != null)
                {
                    var DynmaicAttributeValue = TLIdynamicAttLibValue.FirstOrDefault(x => x.id == DynamicAttributeEntity.Id);

                    if (DynmaicAttributeValue == null)
                        return $"({Validation.DynamicAtt.Key}) value can't be null and must be inserted";

                    var OperationName = Validation.Operation.Name;

                    var InputDynamicValue = DynmaicAttributeValue.value;
                    var ValidationValue = Validation.ValueBoolean ?? Validation.ValueDateTime ?? Validation.ValueDouble ?? (object)Validation.ValueString;

                    if (!(OperationName == "==" ? InputDynamicValue.ToString().ToLower() == ValidationValue.ToString().ToLower() :
                        OperationName == "!=" ? InputDynamicValue.ToString().ToLower() != ValidationValue.ToString().ToLower() :
                        OperationName == ">" ? Comparer.DefaultInvariant.Compare(InputDynamicValue, ValidationValue) == 1 :
                        OperationName == ">=" ? (Comparer.DefaultInvariant.Compare(InputDynamicValue, ValidationValue) == 1 ||
                            InputDynamicValue.ToString().ToLower() == ValidationValue.ToString().ToLower()) :
                        OperationName == "<" ? Comparer.DefaultInvariant.Compare(InputDynamicValue, ValidationValue) == -1 :
                        OperationName == "<=" ? (Comparer.DefaultInvariant.Compare(InputDynamicValue, ValidationValue) == -1 ||
                            InputDynamicValue.ToString().ToLower() == ValidationValue.ToString().ToLower()) : false))
                    {
                        var DynamicAttributeName = _unitOfWork.DynamicAttRepository
                            .GetWhereFirst(x => x.Id == Validation.DynamicAttId).Key;

                        var ReturnOperation = (OperationName == "==" ? "equal to" :
                            (OperationName == "!=" ? "not equal to" :
                            (OperationName == ">" ? "bigger than" :
                            (OperationName == ">=" ? "bigger than or equal to" :
                            (OperationName == "<" ? "smaller than" :
                            (OperationName == "<=" ? "smaller than or equal to" : ""))))));

                        return $"({DynamicAttributeName}) value must be {ReturnOperation} {ValidationValue}";
                    }
                }
                return null;
            }).FirstOrDefault(invalidValidation => invalidValidation != null);

            return invalidValidation ?? string.Empty;
        }
        public void AddLogisticalItemWithPower(dynamic LogisticalItemIds, dynamic PowerEntity, int TableNameEntityId)
        {
            using (TransactionScope transaction = new TransactionScope())
            {
                try
                {
                    if (LogisticalItemIds.LogisticalItems != null)
                    {
                        if (LogisticalItemIds.LogisticalItems.VendorId != null && LogisticalItemIds.LogisticalItems.VendorId != 0)
                        {
                            TLIlogistical LogisticalObject = _unitOfWork.LogistcalRepository.GetByID(LogisticalItemIds.LogisticalItems.VendorId);
                            TLIlogisticalitem NewLogisticalItem = new TLIlogisticalitem
                            {
                                Name = "",
                                IsLib = true,
                                logisticalId = LogisticalObject.Id,
                                RecordId = PowerEntity.Id,
                                tablesNamesId = TableNameEntityId
                            };
                            _unitOfWork.LogisticalitemRepository.AddAsync(NewLogisticalItem);
                            _unitOfWork.SaveChangesAsync();
                        }
                        if (LogisticalItemIds.LogisticalItems.SupplierId != null && LogisticalItemIds.LogisticalItems.SupplierId != 0)
                        {
                            TLIlogistical LogisticalObject = _unitOfWork.LogistcalRepository.GetByID(LogisticalItemIds.LogisticalItems.SupplierId);
                            TLIlogisticalitem NewLogisticalItem = new TLIlogisticalitem
                            {
                                Name = "",
                                IsLib = true,
                                logisticalId = LogisticalObject.Id,
                                RecordId = PowerEntity.Id,
                                tablesNamesId = TableNameEntityId
                            };
                            _unitOfWork.LogisticalitemRepository.AddAsync(NewLogisticalItem);
                            _unitOfWork.SaveChangesAsync();
                        }
                        if (LogisticalItemIds.LogisticalItems.DesignerId != null && LogisticalItemIds.LogisticalItems.DesignerId != 0)
                        {
                            TLIlogistical LogisticalObject = _unitOfWork.LogistcalRepository.GetByID(LogisticalItemIds.LogisticalItems.DesignerId);
                            TLIlogisticalitem NewLogisticalItem = new TLIlogisticalitem
                            {
                                Name = "",
                                IsLib = true,
                                logisticalId = LogisticalObject.Id,
                                RecordId = PowerEntity.Id,
                                tablesNamesId = TableNameEntityId
                            };
                            _unitOfWork.LogisticalitemRepository.AddAsync(NewLogisticalItem);
                            _unitOfWork.SaveChangesAsync();
                        }
                        if (LogisticalItemIds.LogisticalItems.ManufacturerId != null && LogisticalItemIds.LogisticalItems.ManufacturerId != 0)
                        {
                            TLIlogistical LogisticalObject = _unitOfWork.LogistcalRepository.GetByID(LogisticalItemIds.LogisticalItems.ManufacturerId);
                            TLIlogisticalitem NewLogisticalItem = new TLIlogisticalitem
                            {
                                Name = "",
                                IsLib = true,
                                logisticalId = LogisticalObject.Id,
                                RecordId = PowerEntity.Id,
                                tablesNamesId = TableNameEntityId
                            };
                            _unitOfWork.LogisticalitemRepository.AddAsync(NewLogisticalItem);
                            _unitOfWork.SaveChangesAsync();
                        }
                    }

                    transaction.Complete();
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
        #endregion
        //Function take 2 parameters TableName, PowerLibraryViewModel
        //get table name Entity by TableName
        //Map ViewModel to Entity
        //Check validation
        //Update validation
        //Update dynamic attributes
        //public async Task<Response<AllItemAttributes>> EditPowerLibrary(string TableName, EditPowerLibraryViewModel PowerLibraryViewModel)
        //{
        //    using (TransactionScope transaction2 =
        //        new TransactionScope(TransactionScopeOption.Required,
        //                           new System.TimeSpan(0, 15, 0)))
        //    {
        //        try
        //        {
        //            int resultId = 0;
        //            TLItablesNames TableNameEntity = _unitOfWork.TablesNamesRepository.GetWhereFirst(l => l.TableName.ToLower() == TableName.ToLower());
        //            TLIpowerLibrary PowerLibrary = _mapper.Map<TLIpowerLibrary>(PowerLibraryViewModel);
        //            var PowerLib = _unitOfWork.PowerLibraryRepository.GetAllAsQueryable().AsNoTracking().FirstOrDefault(x => x.Id == PowerLibraryViewModel.Id);

        //            PowerLibrary.Active = PowerLib.Active;
        //            PowerLibrary.Deleted = PowerLib.Deleted;
        //            var CheckModel = _unitOfWork.PowerLibraryRepository.Any(x => x.Model.ToLower() == PowerLibrary.Model.ToLower() &&
        //                x.Id != PowerLibrary.Id && !x.Deleted);
        //            if (CheckModel != false)
        //            {
        //                return new Response<AllItemAttributes>(true, null, null, $"This model {PowerLibrary.Model} is already exists", (int)Helpers.Constants.ApiReturnCode.fail);
        //            }

        //            _unitOfWork.PowerLibraryRepository.UpdateWithHistory(Helpers.LogFilterAttribute.UserId, PowerLib, PowerLibrary);
        //            //if (testUpdate.Details.Count != 0)
        //            //{
        //            //    _unitOfWork.PowerLibraryRepository.Update((TLIpowerLibrary)testUpdate.original);
        //            //    resultId = _unitOfWork.TablesHistoryRepository.AddHistoryForEdit(PowerLibrary.Id, TableNameEntity.Id, "Update", testUpdate.Details.ToList());
        //            //}

        //            string CheckDependecncy = CheckDependencyValidationEditApiVersion(PowerLibraryViewModel);
        //            if (!string.IsNullOrEmpty(CheckDependecncy))
        //            {
        //                return new Response<AllItemAttributes>(true, null, null, CheckDependecncy, (int)Helpers.Constants.ApiReturnCode.fail);
        //            }

        //            string CheckValidation = CheckGeneralValidationFunctionEditApiVersion(PowerLibraryViewModel.DynamicAtts, TableNameEntity.TableName);
        //            if (!string.IsNullOrEmpty(CheckValidation))
        //            {
        //                return new Response<AllItemAttributes>(true, null, null, CheckValidation, (int)Helpers.Constants.ApiReturnCode.fail);
        //            }

        //            dynamic LogisticalItemIds = new ExpandoObject();
        //            LogisticalItemIds = PowerLibraryViewModel;

        //            AddLogisticalViewModel OldLogisticalItemIds = new AddLogisticalViewModel();

        //            var CheckVendorId = _unitOfWork.LogisticalitemRepository
        //                .GetIncludeWhereFirst(x => x.logistical.logisticalType.Name.ToLower() == Helpers.Constants.LogisticalType.Vendor.ToString().ToLower() &&
        //                    x.IsLib && x.tablesNamesId == TableNameEntity.Id && x.RecordId == PowerLibrary.Id, x => x.logistical,
        //                        x => x.logistical.logisticalType);

        //            if (CheckVendorId != null)
        //                OldLogisticalItemIds.VendorId = CheckVendorId.logisticalId;

        //            else
        //                OldLogisticalItemIds.VendorId = 0;

        //            var CheckSupplierId = _unitOfWork.LogisticalitemRepository
        //                .GetIncludeWhereFirst(x => x.logistical.logisticalType.Name.ToLower() == Helpers.Constants.LogisticalType.Supplier.ToString().ToLower() &&
        //                    x.IsLib && x.tablesNamesId == TableNameEntity.Id && x.RecordId == PowerLibrary.Id, x => x.logistical,
        //                        x => x.logistical.logisticalType);

        //            if (CheckSupplierId != null)
        //                OldLogisticalItemIds.SupplierId = CheckSupplierId.logisticalId;

        //            else
        //                OldLogisticalItemIds.SupplierId = 0;

        //            var CheckDesignerId = _unitOfWork.LogisticalitemRepository
        //                .GetIncludeWhereFirst(x => x.logistical.logisticalType.Name.ToLower() == Helpers.Constants.LogisticalType.Designer.ToString().ToLower() &&
        //                    x.IsLib && x.tablesNamesId == TableNameEntity.Id && x.RecordId == PowerLibrary.Id, x => x.logistical,
        //                        x => x.logistical.logisticalType);

        //            if (CheckDesignerId != null)
        //                OldLogisticalItemIds.DesignerId = CheckDesignerId.logisticalId;

        //            else
        //                OldLogisticalItemIds.DesignerId = 0;

        //            var CheckManufacturerId = _unitOfWork.LogisticalitemRepository
        //                .GetIncludeWhereFirst(x => x.logistical.logisticalType.Name.ToLower() == Helpers.Constants.LogisticalType.Manufacturer.ToString().ToLower() &&
        //                    x.IsLib && x.tablesNamesId == TableNameEntity.Id && x.RecordId == PowerLibrary.Id, x => x.logistical,
        //                        x => x.logistical.logisticalType);

        //            if (CheckManufacturerId != null)
        //                OldLogisticalItemIds.ManufacturerId = CheckManufacturerId.logisticalId;

        //            else
        //                OldLogisticalItemIds.ManufacturerId = 0;

        //            EditLogisticalItem(LogisticalItemIds, PowerLibrary, TableNameEntity.Id, OldLogisticalItemIds);

        //            if (PowerLibraryViewModel.DynamicAtts != null ? PowerLibraryViewModel.DynamicAtts.Count > 0 : false)
        //            {
        //                _unitOfWork.DynamicAttLibRepository.UpdateDynamicLibAttsWithHistory(PowerLibraryViewModel.DynamicAtts, TableNameEntity.Id, PowerLibrary.Id, Helpers.LogFilterAttribute.UserId, resultId, PowerLib.Id);
        //            }

        //            await _unitOfWork.SaveChangesAsync();
        //            transaction2.Complete();
        //            return new Response<AllItemAttributes>();
        //        }
        //        catch (Exception err)
        //        {
        //            return new Response<AllItemAttributes>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
        //        }
        //    }
        //}
        #region Helper Methods..
        public void EditLogisticalItem(dynamic LogisticalItemIds, dynamic MainEntity, int TableNameEntityId, dynamic OldLogisticalItemIds)
        {
            using (TransactionScope transaction2 =
                new TransactionScope(TransactionScopeOption.Required,
                                   new System.TimeSpan(0, 15, 0)))
            {
                try
                {
                    if (LogisticalItemIds.LogisticalItems != null)
                    {
                        if (LogisticalItemIds.LogisticalItems.VendorId != null && LogisticalItemIds.LogisticalItems.VendorId != 0)
                        {
                            if (OldLogisticalItemIds.VendorId != null ? OldLogisticalItemIds.VendorId != 0 : false)
                            {
                                TLIlogistical OldLogisticalObject = _unitOfWork.LogistcalRepository
                                    .GetByID(OldLogisticalItemIds.VendorId);

                                int CivilId = MainEntity.Id;

                                TLIlogisticalitem? LogisticalItem = _unitOfWork.LogisticalitemRepository
                                    .GetWhereFirst(x => x.logisticalId == OldLogisticalObject.Id && x.IsLib && x.RecordId == CivilId &&
                                        x.tablesNamesId == TableNameEntityId);

                                if (LogisticalItem != null)
                                {
                                    LogisticalItem.logisticalId = LogisticalItemIds.LogisticalItems.VendorId;

                                    _unitOfWork.LogisticalitemRepository.Update(LogisticalItem);
                                    _unitOfWork.SaveChangesAsync();
                                }
                                else
                                {
                                    TLIlogisticalitem NewLogisticalItem = new TLIlogisticalitem()
                                    {
                                        IsLib = true,
                                        logisticalId = LogisticalItemIds.LogisticalItems.VendorId,
                                        Name = OldLogisticalObject.Name,
                                        RecordId = CivilId,
                                        tablesNamesId = TableNameEntityId
                                    };

                                    _unitOfWork.LogisticalitemRepository.Add(NewLogisticalItem);
                                    _unitOfWork.SaveChangesAsync();
                                }
                            }
                            else
                            {
                                TLIlogistical LogisticalObject = _unitOfWork.LogistcalRepository
                                    .GetByID(LogisticalItemIds.LogisticalItems.VendorId);

                                TLIlogisticalitem NewLogisticalItem = new TLIlogisticalitem
                                {
                                    Name = "",
                                    IsLib = true,
                                    logisticalId = LogisticalObject.Id,
                                    RecordId = MainEntity.Id,
                                    tablesNamesId = TableNameEntityId
                                };
                                _unitOfWork.LogisticalitemRepository.AddAsync(NewLogisticalItem);
                                _unitOfWork.SaveChangesAsync();
                            }
                        }
                        if (LogisticalItemIds.LogisticalItems.SupplierId != null && LogisticalItemIds.LogisticalItems.SupplierId != 0)
                        {
                            if (OldLogisticalItemIds.SupplierId != null ? OldLogisticalItemIds.SupplierId != 0 : false)
                            {
                                TLIlogistical OldLogisticalObject = _unitOfWork.LogistcalRepository
                                    .GetByID(OldLogisticalItemIds.SupplierId);

                                int CivilId = MainEntity.Id;

                                TLIlogisticalitem? LogisticalItem = _unitOfWork.LogisticalitemRepository
                                    .GetWhereFirst(x => x.logisticalId == OldLogisticalObject.Id && x.IsLib && x.RecordId == CivilId &&
                                        x.tablesNamesId == TableNameEntityId);

                                if (LogisticalItem != null)
                                {
                                    LogisticalItem.logisticalId = LogisticalItemIds.LogisticalItems.SupplierId;

                                    _unitOfWork.LogisticalitemRepository.Update(LogisticalItem);
                                    _unitOfWork.SaveChangesAsync();
                                }
                                else
                                {
                                    TLIlogisticalitem NewLogisticalItem = new TLIlogisticalitem()
                                    {
                                        IsLib = true,
                                        logisticalId = LogisticalItemIds.LogisticalItems.SupplierId,
                                        Name = OldLogisticalObject.Name,
                                        RecordId = CivilId,
                                        tablesNamesId = TableNameEntityId
                                    };

                                    _unitOfWork.LogisticalitemRepository.Add(NewLogisticalItem);
                                    _unitOfWork.SaveChangesAsync();
                                }
                            }
                            else
                            {
                                TLIlogistical LogisticalObject = _unitOfWork.LogistcalRepository
                                    .GetByID(LogisticalItemIds.LogisticalItems.SupplierId);

                                TLIlogisticalitem NewLogisticalItem = new TLIlogisticalitem
                                {
                                    Name = "",
                                    IsLib = true,
                                    logisticalId = LogisticalObject.Id,
                                    RecordId = MainEntity.Id,
                                    tablesNamesId = TableNameEntityId
                                };
                                _unitOfWork.LogisticalitemRepository.AddAsync(NewLogisticalItem);
                                _unitOfWork.SaveChangesAsync();
                            }
                        }
                        if (LogisticalItemIds.LogisticalItems.DesignerId != null && LogisticalItemIds.LogisticalItems.DesignerId != 0)
                        {
                            if (OldLogisticalItemIds.DesignerId != null ? OldLogisticalItemIds.DesignerId != 0 : false)
                            {
                                TLIlogistical OldLogisticalObject = _unitOfWork.LogistcalRepository
                                    .GetByID(OldLogisticalItemIds.DesignerId);

                                int CivilId = MainEntity.Id;

                                TLIlogisticalitem? LogisticalItem = _unitOfWork.LogisticalitemRepository
                                    .GetWhereFirst(x => x.logisticalId == OldLogisticalObject.Id && x.IsLib && x.RecordId == CivilId &&
                                        x.tablesNamesId == TableNameEntityId);

                                if (LogisticalItem != null)
                                {
                                    LogisticalItem.logisticalId = LogisticalItemIds.LogisticalItems.DesignerId;

                                    _unitOfWork.LogisticalitemRepository.Update(LogisticalItem);
                                    _unitOfWork.SaveChangesAsync();
                                }
                                else
                                {
                                    TLIlogisticalitem NewLogisticalItem = new TLIlogisticalitem()
                                    {
                                        IsLib = true,
                                        logisticalId = LogisticalItemIds.LogisticalItems.DesignerId,
                                        Name = OldLogisticalObject.Name,
                                        RecordId = CivilId,
                                        tablesNamesId = TableNameEntityId
                                    };

                                    _unitOfWork.LogisticalitemRepository.Add(NewLogisticalItem);
                                    _unitOfWork.SaveChangesAsync();
                                }
                            }
                            else
                            {
                                TLIlogistical LogisticalObject = _unitOfWork.LogistcalRepository
                                    .GetByID(LogisticalItemIds.LogisticalItems.DesignerId);

                                TLIlogisticalitem NewLogisticalItem = new TLIlogisticalitem
                                {
                                    Name = "",
                                    IsLib = true,
                                    logisticalId = LogisticalObject.Id,
                                    RecordId = MainEntity.Id,
                                    tablesNamesId = TableNameEntityId
                                };
                                _unitOfWork.LogisticalitemRepository.AddAsync(NewLogisticalItem);
                                _unitOfWork.SaveChangesAsync();
                            }
                        }
                        if (LogisticalItemIds.LogisticalItems.ManufacturerId != null && LogisticalItemIds.LogisticalItems.ManufacturerId != 0)
                        {
                            if (OldLogisticalItemIds.ManufacturerId != null ? OldLogisticalItemIds.ManufacturerId != 0 : false)
                            {
                                TLIlogistical OldLogisticalObject = _unitOfWork.LogistcalRepository
                                    .GetByID(OldLogisticalItemIds.ManufacturerId);

                                int CivilId = MainEntity.Id;

                                TLIlogisticalitem? LogisticalItem = _unitOfWork.LogisticalitemRepository
                                    .GetWhereFirst(x => x.logisticalId == OldLogisticalObject.Id && x.IsLib && x.RecordId == CivilId &&
                                        x.tablesNamesId == TableNameEntityId);

                                if (LogisticalItem != null)
                                {
                                    LogisticalItem.logisticalId = LogisticalItemIds.LogisticalItems.ManufacturerId;

                                    _unitOfWork.LogisticalitemRepository.Update(LogisticalItem);
                                    _unitOfWork.SaveChangesAsync();
                                }
                                else
                                {
                                    TLIlogisticalitem NewLogisticalItem = new TLIlogisticalitem()
                                    {
                                        IsLib = true,
                                        logisticalId = LogisticalItemIds.LogisticalItems.ManufacturerId,
                                        Name = OldLogisticalObject.Name,
                                        RecordId = CivilId,
                                        tablesNamesId = TableNameEntityId
                                    };

                                    _unitOfWork.LogisticalitemRepository.Add(NewLogisticalItem);
                                    _unitOfWork.SaveChangesAsync();
                                }
                            }
                            else
                            {
                                TLIlogistical LogisticalObject = _unitOfWork.LogistcalRepository
                                    .GetByID(LogisticalItemIds.LogisticalItems.ManufacturerId);

                                TLIlogisticalitem NewLogisticalItem = new TLIlogisticalitem
                                {
                                    Name = "",
                                    IsLib = true,
                                    logisticalId = LogisticalObject.Id,
                                    RecordId = MainEntity.Id,
                                    tablesNamesId = TableNameEntityId
                                };
                                _unitOfWork.LogisticalitemRepository.AddAsync(NewLogisticalItem);
                                _unitOfWork.SaveChangesAsync();
                            }
                        }
                    }

                    transaction2.Complete();
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
        public string CheckDependencyValidationEditApiVersion(object Input)
        {
            EditPowerLibraryViewModel EditPowerLibraryViewModel = _mapper.Map<EditPowerLibraryViewModel>(Input);

            List<DynamicAttViewModel> DynamicAttributes = _mapper.Map<List<DynamicAttViewModel>>(_unitOfWork.DynamicAttRepository
                .GetIncludeWhere(x => x.tablesNames.TableName.ToLower() == Helpers.Constants.TablesNames.TLIpowerLibrary.ToString().ToLower() && !x.disable
                    , x => x.tablesNames).ToList());

            foreach (DynamicAttViewModel DynamicAttribute in DynamicAttributes)
            {
                TLIdependency Dependency = _unitOfWork.DependencieRepository.GetIncludeWhereFirst(x => x.DynamicAttId == DynamicAttribute.Id &&
                    x.OperationId != null && (x.ValueBoolean != null || x.ValueDateTime != null || x.ValueDouble != null || !string.IsNullOrEmpty(x.ValueString)),
                        x => x.Operation, x => x.DynamicAtt);

                if (Dependency != null)
                {
                    DynamicAttLibViewModel InsertedDynamicAttributeValue = EditPowerLibraryViewModel.DynamicAtts
                        .FirstOrDefault(x => x.Key.ToLower() == DynamicAttribute.Key.ToLower());

                    if (InsertedDynamicAttributeValue == null)
                        return $"({DynamicAttribute.Key}) value can't be null and must be inserted";

                    List<int> RowsIds = _unitOfWork.DependencyRowRepository.GetWhere(x => x.DependencyId == Dependency.Id && x.RowId != null).Select(x => x.RowId.Value).Distinct().ToList();

                    foreach (int RowId in RowsIds)
                    {
                        List<TLIrule> Rules = _unitOfWork.RowRuleRepository.GetIncludeWhere(x => x.RowId.Value == RowId, x => x.Rule, x => x.Rule.Operation, x => x.Rule.attributeActivated
                            , x => x.Rule.dynamicAtt).Select(x => x.Rule).Distinct().ToList();

                        int Succed = 0;

                        foreach (TLIrule Rule in Rules)
                        {
                            string RuleOperation = Rule.Operation.Name;
                            object RuleValue = new object();

                            if (Rule.OperationValueBoolean != null)
                                RuleValue = Rule.OperationValueBoolean;

                            else if (Rule.OperationValueDateTime != null)
                                RuleValue = Rule.OperationValueDateTime;

                            else if (Rule.OperationValueDouble != null)
                                RuleValue = Rule.OperationValueDouble;

                            else if (!string.IsNullOrEmpty(Rule.OperationValueString))
                                RuleValue = Rule.OperationValueString;

                            object InsertedValue = new object();

                            if (Rule.attributeActivatedId != null)
                            {
                                string AttributeName = Rule.attributeActivated.Key;

                                InsertedValue = EditPowerLibraryViewModel.GetType().GetProperties()
                                    .FirstOrDefault(x => x.Name.ToLower() == AttributeName.ToLower()).GetValue(EditPowerLibraryViewModel, null);
                            }
                            else if (Rule.dynamicAttId != null)
                            {
                                DynamicAttLibViewModel DynamicObject = EditPowerLibraryViewModel.DynamicAtts
                                    .FirstOrDefault(x => x.Key.ToLower() == Rule.dynamicAtt.Key.ToLower());

                                if (DynamicObject == null)
                                    break;

                                InsertedValue = DynamicObject.Value;
                            }

                            if (InsertedValue == null)
                                break;

                            if (RuleOperation == "==" ? InsertedValue.ToString().ToLower() == RuleValue.ToString().ToLower() :
                                RuleOperation == "!=" ? InsertedValue.ToString().ToLower() != RuleValue.ToString().ToLower() :
                                RuleOperation == ">" ? Comparer.DefaultInvariant.Compare(InsertedValue, RuleValue) == 1 :
                                RuleOperation == ">=" ? (Comparer.DefaultInvariant.Compare(InsertedValue, RuleValue) == 1 ||
                                    InsertedValue.ToString().ToLower() == RuleValue.ToString().ToLower()) :
                                RuleOperation == "<" ? Comparer.DefaultInvariant.Compare(InsertedValue, RuleValue) == -1 :
                                RuleOperation == "<=" ? (Comparer.DefaultInvariant.Compare(InsertedValue, RuleValue) == -1 ||
                                    InsertedValue.ToString().ToLower() == RuleValue.ToString().ToLower()) : false)
                            {
                                Succed++;
                            }
                        }
                        if (Rules.Count() == Succed)
                        {
                            string DependencyValidationOperation = Dependency.Operation.Name;

                            object DependencyValdiationValue = Dependency.ValueBoolean != null ? Dependency.ValueBoolean :
                                Dependency.ValueDateTime != null ? Dependency.ValueDateTime :
                                Dependency.ValueDouble != null ? Dependency.ValueDouble :
                                !string.IsNullOrEmpty(Dependency.ValueString) ? Dependency.ValueString : null;

                            object InsertedDynamicAttributeValueAsObject = InsertedDynamicAttributeValue.Value;

                            if (Dependency.ValueDateTime != null)
                            {
                                DateTime DependencyValdiationValueConverter = new DateTime(Dependency.ValueDateTime.Value.Year,
                                    Dependency.ValueDateTime.Value.Month, Dependency.ValueDateTime.Value.Day);

                                DependencyValdiationValue = DependencyValdiationValueConverter;

                                DateTime InsertedDynamicAttributeValueAsObjectConverter = DateTime.Parse(InsertedDynamicAttributeValue.Value.ToString());

                                InsertedDynamicAttributeValueAsObject = InsertedDynamicAttributeValueAsObjectConverter;
                            }

                            if (InsertedDynamicAttributeValueAsObject != null && DependencyValdiationValue != null)
                            {
                                if (!(DependencyValidationOperation == "==" ? InsertedDynamicAttributeValueAsObject.ToString().ToLower() == DependencyValdiationValue.ToString().ToLower() :
                                     DependencyValidationOperation == "!=" ? InsertedDynamicAttributeValueAsObject.ToString().ToLower() != DependencyValdiationValue.ToString().ToLower() :
                                     DependencyValidationOperation == ">" ? Comparer.DefaultInvariant.Compare(InsertedDynamicAttributeValueAsObject, DependencyValdiationValue) == 1 :
                                     DependencyValidationOperation == ">=" ? (InsertedDynamicAttributeValueAsObject.ToString().ToLower() == DependencyValdiationValue.ToString().ToLower() ||
                                         Comparer.DefaultInvariant.Compare(InsertedDynamicAttributeValueAsObject, DependencyValdiationValue) == 1) :
                                     DependencyValidationOperation == "<" ? Comparer.DefaultInvariant.Compare(InsertedDynamicAttributeValueAsObject, DependencyValdiationValue) == -1 :
                                     DependencyValidationOperation == "<=" ? (InsertedDynamicAttributeValueAsObject.ToString().ToLower() == DependencyValdiationValue.ToString().ToLower() ||
                                         Comparer.DefaultInvariant.Compare(InsertedDynamicAttributeValueAsObject, DependencyValdiationValue) == -1) : false))
                                {
                                    string ReturnOperation = (DependencyValidationOperation == "==" ? "Equal To" :
                                        (DependencyValidationOperation == "!=" ? "not equal to" :
                                        (DependencyValidationOperation == ">" ? "bigger than" :
                                        (DependencyValidationOperation == ">=" ? "bigger than or equal to" :
                                        (DependencyValidationOperation == "<" ? "smaller than" :
                                        (DependencyValidationOperation == "<=" ? "smaller than or equal to" : ""))))));

                                    return $"({Dependency.DynamicAtt.Key}) value must be {ReturnOperation} {DependencyValdiationValue}";
                                }
                            }
                        }
                    }
                }
            }
            return string.Empty;
        }
        public string CheckGeneralValidationFunctionEditApiVersion(List<DynamicAttLibViewModel> TLIdynamicAttLibValue, string TableName)
        {
            List<DynamicAttViewModel> DynamicAttributes = _mapper.Map<List<DynamicAttViewModel>>(_unitOfWork.DynamicAttRepository
                .GetIncludeWhere(x => x.tablesNames.TableName.ToLower() == TableName.ToLower() && !x.disable
                    , x => x.tablesNames).ToList());

            foreach (DynamicAttViewModel DynamicAttribute in DynamicAttributes)
            {
                TLIvalidation Validation = _unitOfWork.ValidationRepository
                    .GetIncludeWhereFirst(x => x.DynamicAtt.Key.ToLower() == DynamicAttribute.Key.ToLower(), x => x.Operation, x => x.DynamicAtt);

                if (Validation != null)
                {
                    string OperationName = Validation.Operation.Name;

                    DynamicAttLibViewModel TestValue = TLIdynamicAttLibValue.FirstOrDefault(x => x.Id == DynamicAttribute.Id);

                    if (TestValue == null)
                        return $"({Validation.DynamicAtt.Key}) value can't be null and must be inserted";

                    object InputDynamicValue = TestValue.Value;

                    object ValidationValue = new object();

                    if (Validation.ValueBoolean != null)
                    {
                        ValidationValue = Validation.ValueBoolean;
                        InputDynamicValue = bool.Parse(TestValue.Value.ToString());
                    }

                    else if (Validation.ValueDateTime != null)
                    {
                        ValidationValue = Validation.ValueDateTime;
                        InputDynamicValue = DateTime.Parse(TestValue.Value.ToString());
                    }

                    else if (Validation.ValueDouble != null)
                    {
                        ValidationValue = Validation.ValueDouble;
                        InputDynamicValue = double.Parse(TestValue.Value.ToString());
                    }

                    else if (!string.IsNullOrEmpty(Validation.ValueString))
                    {
                        ValidationValue = Validation.ValueString;
                        InputDynamicValue = TestValue.Value.ToString();
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
        //Function take 2 parameters TableName, Id
        //get table name Entity by TableName
        //get record by Id
        //disable or enable record depened on record status
        //Update Entity
        public async Task<Response<AllItemAttributes>> DisablePowerLibrary(int UserId,string TableName, int Id,string connectionString)
        {
            try
            {
                var TableNameEntity = _unitOfWork.TablesNamesRepository.GetWhereFirst(l => l.TableName == TableName);
                var CivilLoad = _unitOfWork.CivilLoadsRepository.GetIncludeWhere(x => x.allLoadInstId != null && !x.Dismantle &&
                      x.allLoadInst.power.powerLibraryId == Id, x => x.allLoadInst, x => x.allLoadInst.power).ToList();
       
                if (CivilLoad != null && CivilLoad.Count > 0)
                    return new Response<AllItemAttributes>(false, null, null, "Can not change status this item because is used", (int)Helpers.Constants.ApiReturnCode.fail);

                var PowerEntity = _unitOfWork.PowerLibraryRepository.GetAllAsQueryable().AsNoTracking().FirstOrDefault(x => x.Id == Id);
                TLIpowerLibrary NewPowerLibrary = _unitOfWork.PowerLibraryRepository.GetAllAsQueryable().AsNoTracking().FirstOrDefault(x => x.Id == Id);
                NewPowerLibrary.Active = !(NewPowerLibrary.Active);
                _unitOfWork.PowerLibraryRepository.UpdateWithHistory(UserId, PowerEntity, NewPowerLibrary);
                await _unitOfWork.SaveChangesAsync();
                Task.Run(() => _unitOfWork.CivilWithLegsRepository.RefreshView(connectionString));
                return new Response<AllItemAttributes>();
            }
            catch (Exception err)
            {

                return new Response<AllItemAttributes>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
        }
        //Function take 1 parameter
        //get table name Entity
        //get activated attributes
        //get dynamic attributes
        public Response<GetForAddCivilLibrarybject> GetForAdd(string TableName)
        {
            try
            {
                GetForAddCivilLibrarybject attributes = new GetForAddCivilLibrarybject();
                var TableNameEntity = _unitOfWork.TablesNamesRepository.GetWhereFirst(l => l.TableName == "TLIpowerLibrary");
                var ListAttributesActivated = _unitOfWork.AttributeActivatedRepository.GetAttributeActivatedGetForAdd(TablesNames.TLIpowerLibrary.ToString(), null, null)
                   .ToList();
                var LogisticalItems = _unitOfWork.LogistcalRepository.GetLogisticalLibrary("Power");
                attributes.LogisticalItems = LogisticalItems;
                attributes.AttributesActivatedLibrary = ListAttributesActivated;
                IEnumerable<BaseInstAttViewDynamic> DynamicAttributesWithoutValue = _unitOfWork.DynamicAttRepository
                   .GetDynamicLibAtt(TableNameEntity.Id, null)
                   .Select(DynamicAttribute =>
                   {
                       TLIdynamicAtt DynamicAttributeEntity = _unitOfWork.DynamicAttRepository.GetByID(DynamicAttribute.Id);
                       if (!string.IsNullOrEmpty(DynamicAttributeEntity.DefaultValue))
                       {
                           switch (DynamicAttribute.DataType.ToLower())
                           {
                               case "string":
                                   DynamicAttribute.Value = DynamicAttributeEntity.DefaultValue;
                                   break;
                               case "int":
                                   DynamicAttribute.Value = int.Parse(DynamicAttributeEntity.DefaultValue);
                                   break;
                               case "double":
                                   DynamicAttribute.Value = double.Parse(DynamicAttributeEntity.DefaultValue);
                                   break;
                               case "bool":
                                   DynamicAttribute.Value = bool.Parse(DynamicAttributeEntity.DefaultValue);
                                   break;
                               case "datetime":
                                   DynamicAttribute.Value = DateTime.Parse(DynamicAttributeEntity.DefaultValue);
                                   break;
                           }
                       }
                       else
                       {
                           DynamicAttribute.Value = " ".Split(' ')[0];
                       }
                       return DynamicAttribute;
                   });
                attributes.DynamicAttributes = DynamicAttributesWithoutValue;
                return new Response<GetForAddCivilLibrarybject>(true, attributes, null, null, (int)Helpers.Constants.ApiReturnCode.success);
            }
            catch (Exception err)
            {

                return new Response<GetForAddCivilLibrarybject>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
        }
        //Function take 2 parameters
        //get table name Entity by TableName
        //get record by Id
        //set Deleted is true
        //Update record 
        //disable dynamic attributes related to that record
        public async Task<Response<AllItemAttributes>> DeletePowerLibrary(int UserId,string TableName, int Id, string connectionString)
        {
            try
            {
                var TableNameEntity = _unitOfWork.TablesNamesRepository.GetWhereFirst(l => l.TableName == TableName);
                var CivilLoad = _unitOfWork.CivilLoadsRepository.GetIncludeWhere(x => x.allLoadInstId != null && !x.Dismantle &&
                       x.allLoadInst.power.powerLibraryId == Id, x => x.allLoadInst, x => x.allLoadInst.power).ToList();

                if (CivilLoad != null && CivilLoad.Count > 0)
                    return new Response<AllItemAttributes>(false, null, null, "Can not delete this item because is used", (int)Helpers.Constants.ApiReturnCode.fail);

                var PowerEntity = _unitOfWork.PowerLibraryRepository.GetAllAsQueryable().AsNoTracking().FirstOrDefault(x => x.Id == Id);
                TLIpowerLibrary NewPowerLibrary = _unitOfWork.PowerLibraryRepository.GetAllAsQueryable().AsNoTracking().FirstOrDefault(x => x.Id == Id);
                NewPowerLibrary.Deleted = true;
                NewPowerLibrary.Model = NewPowerLibrary.Model + "_" + DateTime.Now.ToString();
                _unitOfWork.PowerLibraryRepository.UpdateWithHistory(UserId, PowerEntity, NewPowerLibrary);
                _unitOfWork.DynamicAttLibRepository.DisableDynamicAttLibValues(TableNameEntity.Id, Id);
                await _unitOfWork.SaveChangesAsync();
                Task.Run(() => _unitOfWork.CivilWithLegsRepository.RefreshView(connectionString));
                return new Response<AllItemAttributes>();
            }
            catch (Exception err)
            {

                return new Response<AllItemAttributes>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
        }
        //#region Add History
        //public void AddHistory(int Other_Inventory_Lib_Id, string historyType, string TableName)
        //{

        //    AddTablesHistoryViewModel history = new AddTablesHistoryViewModel();
        //    history.RecordId = Other_Inventory_Lib_Id;
        //    history.TablesNameId = _unitOfWork.TablesNamesRepository.GetWhereFirst(x => x.TableName == TableName).Id;
        //    history.HistoryTypeId = _unitOfWork.HistoryTypeRepository.GetWhereSelectFirst(x => x.Name == historyType, x => new { x.Id }).Id;
        //    history.UserId = 83;
        //    _unitOfWork.TablesHistoryRepository.AddTableHistory(history);

        //}
        //#endregion
    }
}
