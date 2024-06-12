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
using System.Collections;
using System.Globalization;
using TLIS_DAL.Helper;
using TLIS_DAL.Helper.Filters;
using TLIS_DAL.Helpers;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModelBase;
using TLIS_DAL.ViewModels.CabinetDTOs;
using TLIS_DAL.ViewModels.DataTypeDTOs;
using TLIS_DAL.ViewModels.DynamicAttDTOs;
using TLIS_DAL.ViewModels.DynamicAttInstValueDTOs;
using TLIS_DAL.ViewModels.DynamicAttLibValueDTOs;
using TLIS_DAL.ViewModels.LoadOtherLibraryDTOs;
using TLIS_DAL.ViewModels.RadioAntennaLibraryDTOs;
using TLIS_DAL.ViewModels.RadioOtherLibraryDTOs;
using TLIS_DAL.ViewModels.RadioRRULibraryDTOs;
using TLIS_DAL.ViewModels.TablesHistoryDTOs;
using TLIS_Repository.Base;
using TLIS_Service.IService;
using static TLIS_Repository.Helpers.Constants;
using TLIS_DAL.ViewModels.LogisticalDTOs;
using AutoMapper;
using TLIS_DAL.ViewModels.PowerDTOs;
using TLIS_DAL.ViewModels.CivilWithLegLibraryDTOs;
using Org.BouncyCastle.Asn1.Cms;
using TLIS_DAL.ViewModels.CivilWithLegsDTOs;
using TLIS_DAL.ViewModels.AsTypeDTOs;
using TLIS_DAL.ViewModels.PolarityTypeDTOs;
using TLIS_DAL.ViewModels.MW_DishLbraryDTOs;
using TLIS_DAL;
using Nancy.Extensions;

namespace TLIS_Service.Services
{
    public class RadioLibraryService : IRadioLibraryService
    {
        private readonly IUnitOfWork _unitOfWork;
        IServiceCollection _services;
        private IMapper _mapper;
        ApplicationDbContext db;
        public RadioLibraryService(IUnitOfWork unitOfWork, IServiceCollection services, IMapper mapper, ApplicationDbContext _context)
        {
            _unitOfWork = unitOfWork;
            _services = services;
            ServiceProvider serviceProvider = _services.BuildServiceProvider();
            _mapper = mapper;
            db = _context;
        }
        //Function return all OtherRadioLibrary depened on parameters and filters
        public Response<ReturnWithFilters<RadioOtherLibraryViewModel>> GetOtherRadioLibraries(ParameterPagination parameters, List<FilterObjectList> filters = null)
        {
            try
            {
                int count = 0;
                ReturnWithFilters<RadioOtherLibraryViewModel> Response = new ReturnWithFilters<RadioOtherLibraryViewModel>();
                List<FilterObject> condition = new List<FilterObject>();
                condition.Add(new FilterObject("Active", true));
                condition.Add(new FilterObject("Deleted", false));
                var OtherRadioLibraries = _unitOfWork.RadioOtherLibraryRepository.GetAllIncludeMultipleWithCondition(parameters, filters, condition, out count, null).OrderBy(x => x.Id).ToList();
                var OtherRadioLibrariesModels = _mapper.Map<List<RadioOtherLibraryViewModel>>(OtherRadioLibraries);
                Response.Model = OtherRadioLibrariesModels;
                Response.filters = null;
                return new Response<ReturnWithFilters<RadioOtherLibraryViewModel>>(true, Response, null, null, (int)Helpers.Constants.ApiReturnCode.success, count);
            }
            catch (Exception err)
            {

                return new Response<ReturnWithFilters<RadioOtherLibraryViewModel>>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
        }
        //Function return all RadioAntennaLibrary depened on parameters and filters
        public Response<ReturnWithFilters<RadioAntennaLibraryViewModel>> GetRadioAntennaLibraries(ParameterPagination parameters, List<FilterObjectList> filters = null)
        {
            try
            {
                int count = 0;
                ReturnWithFilters<RadioAntennaLibraryViewModel> Response = new ReturnWithFilters<RadioAntennaLibraryViewModel>();
                List<FilterObject> condition = new List<FilterObject>();
                condition.Add(new FilterObject("Active", true));
                condition.Add(new FilterObject("Deleted", false));
                var RadioAntennaLibraries = _unitOfWork.RadioAntennaLibraryRepository.GetAllIncludeMultipleWithCondition(parameters, filters, condition, out count, null).OrderBy(x => x.Id).ToList();
                var RadioAntennaLibrariesModels = _mapper.Map<List<RadioAntennaLibraryViewModel>>(RadioAntennaLibraries);
                Response.Model = RadioAntennaLibrariesModels;
                Response.filters = null;
                return new Response<ReturnWithFilters<RadioAntennaLibraryViewModel>>(true, Response, null, null, (int)Helpers.Constants.ApiReturnCode.success, count);
            }
            catch (Exception err)
            {

                return new Response<ReturnWithFilters<RadioAntennaLibraryViewModel>>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
        }
        //Function return all RadioRRULibrary depened on parameters and filters
        public Response<ReturnWithFilters<RadioRRULibraryViewModel>> GetRadioRRULibraries(ParameterPagination parameters, List<FilterObjectList> filters = null)
        {
            try
            {
                int count = 0;
                ReturnWithFilters<RadioRRULibraryViewModel> Response = new ReturnWithFilters<RadioRRULibraryViewModel>();
                List<FilterObject> condition = new List<FilterObject>();
                condition.Add(new FilterObject("Active", true));
                condition.Add(new FilterObject("Deleted", false));
                var RadioRRULibraries = _unitOfWork.RadioRRULibraryRepository.GetAllIncludeMultipleWithCondition(parameters, filters, condition, out count, null).OrderBy(x => x.Id).ToList();
                var RadioRRULibrariesModels = _mapper.Map<List<RadioRRULibraryViewModel>>(RadioRRULibraries);
                Response.Model = RadioRRULibrariesModels;
                Response.filters = null;
                return new Response<ReturnWithFilters<RadioRRULibraryViewModel>>(true, Response, null, null, (int)Helpers.Constants.ApiReturnCode.success, count);
            }
            catch (Exception err)
            {

                return new Response<ReturnWithFilters<RadioRRULibraryViewModel>>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
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
        public Response<ReturnWithFilters<object>> GetRadioAntennaLibrariesWithEnabledAttribute(CombineFilters CombineFilters, ParameterPagination parameterPagination)
        {
            try
            {
                List<FilterObjectList> ObjectAttributeFilters = CombineFilters.filters;
                List<DateFilterViewModel> DateFilter = CombineFilters.DateFilter;
                int Count = 0;
                List<object> OutPutList = new List<object>();
                ReturnWithFilters<object> RadioAntennaTableDisplay = new ReturnWithFilters<object>();

                List<StringFilterObjectList> AttributeFilters = new List<StringFilterObjectList>();

                List<RadioAntennaLibraryViewModel> RadioAntennaLibraries = new List<RadioAntennaLibraryViewModel>();
                List<RadioAntennaLibraryViewModel> WithoutDateFilterRadioAntennaLibraries = new List<RadioAntennaLibraryViewModel>();
                List<RadioAntennaLibraryViewModel> WithDateFilterRadioAntennaLibraries = new List<RadioAntennaLibraryViewModel>();

                List<TLIattributeActivated> RadioAntennaLibraryAttribute = new List<TLIattributeActivated>();
                if ((DateFilter != null ? DateFilter.Count() > 0 : false) ||
                    (ObjectAttributeFilters != null && ObjectAttributeFilters.Count > 0))
                {
                    RadioAntennaLibraryAttribute = _unitOfWork.AttributeViewManagmentRepository.GetIncludeWhere(x =>
                        x.Enable && x.AttributeActivatedId != null &&
                        x.AttributeActivated.DataType.ToLower() != "datetime" &&
                        x.EditableManagmentView.View == Helpers.Constants.EditableManamgmantViewNames.RadioAntennaLibrary.ToString() &&
                        x.EditableManagmentView.TLItablesNames1.TableName == Helpers.Constants.TablesNames.TLIradioAntennaLibrary.ToString(),
                            x => x.AttributeActivated, x => x.EditableManagmentView, x => x.EditableManagmentView.TLItablesNames1)
                    .Select(x => x.AttributeActivated).ToList();
                }

                if (ObjectAttributeFilters != null && ObjectAttributeFilters.Count > 0)
                {
                    List<TLIattributeActivated> NotDateDateRadioAntennaLibraryAttribute = RadioAntennaLibraryAttribute.Where(x =>
                        x.DataType.ToLower() != "datetime").ToList();

                    foreach (FilterObjectList item in ObjectAttributeFilters)
                    {
                        List<string> value = item.value.Select(x => x.ToString().ToLower()).ToList();

                        TLIattributeActivated AttributeKey = NotDateDateRadioAntennaLibraryAttribute.FirstOrDefault(x =>
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
                        x.tablesNames.TableName == Helpers.Constants.TablesNames.TLIradioAntennaLibrary.ToString(), x => x.tablesNames, x => x.DataType).ToList();

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
                    bool AttrLibExist = typeof(RadioAntennaLibraryViewModel).GetProperties().ToList().Exists(x =>
                        AttributeFilters.Where(y => y.key.ToLower() != "Id".ToLower()).Select(y =>
                            y.key.ToLower()).Contains(x.Name.ToLower()));

                    List<int> LibraryAttributeActivatedIds = new List<int>();

                    if (AttrLibExist)
                    {
                        List<PropertyInfo> NonStringLibraryProps = typeof(RadioAntennaLibraryViewModel).GetProperties().Where(x =>
                            x.PropertyType.Name.ToLower() != "string" &&
                            AttributeFilters.Select(y =>
                                y.key.ToLower()).Contains(x.Name.ToLower())).ToList();

                        List<PropertyInfo> StringLibraryProps = typeof(RadioAntennaLibraryViewModel).GetProperties().Where(x =>
                            x.PropertyType.Name.ToLower() == "string" &&
                            AttributeFilters.Select(y =>
                                y.key.ToLower()).Contains(x.Name.ToLower())).ToList();

                        List<StringFilterObjectList> LibraryPropsAttributeFilters = AttributeFilters.Where(x =>
                            NonStringLibraryProps.Select(y => y.Name.ToLower()).Contains(x.key.ToLower()) ||
                            StringLibraryProps.Select(y => y.Name.ToLower()).Contains(x.key.ToLower())).ToList();

                        IEnumerable<TLIradioAntennaLibrary> Libraries = _unitOfWork.RadioAntennaLibraryRepository.GetWhere(x => !x.Deleted).AsEnumerable();

                        foreach (StringFilterObjectList LibraryProp in LibraryPropsAttributeFilters)
                        {
                            if (StringLibraryProps.Select(x => x.Name.ToLower()).Contains(LibraryProp.key.ToLower()))
                            {
                                Libraries = Libraries.Where(x => StringLibraryProps.AsEnumerable().FirstOrDefault(y => (LibraryProp.key.ToLower() == y.Name.ToLower()) && (LibraryProp.value.AsEnumerable().FirstOrDefault(w =>
                                     y.GetValue(_mapper.Map<RadioAntennaLibraryViewModel>(x), null) != null ? y.GetValue(_mapper.Map<RadioAntennaLibraryViewModel>(x), null).ToString().ToLower().StartsWith(w.ToLower()) : false) != null)) != null).AsEnumerable();
                            }
                            else if (NonStringLibraryProps.Select(x => x.Name.ToLower()).Contains(LibraryProp.key.ToLower()))
                            {
                                Libraries = Libraries.Where(x => NonStringLibraryProps.AsEnumerable().FirstOrDefault(y => (LibraryProp.key.ToLower() == y.Name.ToLower()) && (y.GetValue(_mapper.Map<RadioAntennaLibraryViewModel>(x), null) != null ?
                                    LibraryProp.value.AsEnumerable().Contains(y.GetValue(_mapper.Map<RadioAntennaLibraryViewModel>(x), null).ToString().ToLower()) : false)) != null).AsEnumerable();
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

                    WithoutDateFilterRadioAntennaLibraries = _mapper.Map<List<RadioAntennaLibraryViewModel>>(_unitOfWork.RadioAntennaLibraryRepository.GetWhere(x =>
                        x.Id > 0 && IntersectLibraryIds.Contains(x.Id) && !x.Deleted).ToList());
                }

                //
                // DateTime Objects Filters..
                //
                List<DateFilterViewModel> AfterConvertDateFilters = new List<DateFilterViewModel>();
                if (DateFilter != null ? DateFilter.Count() > 0 : false)
                {
                    List<TLIattributeActivated> DateRadioAntennaLibraryAttribute = RadioAntennaLibraryAttribute.Where(x =>
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

                        TLIattributeActivated AttributeKey = DateRadioAntennaLibraryAttribute.FirstOrDefault(x =>
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
                        x.tablesNames.TableName == Helpers.Constants.TablesNames.TLIradioAntennaLibrary.ToString(), x => x.tablesNames).ToList();

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
                    List<PropertyInfo> LibraryProps = typeof(RadioAntennaLibraryViewModel).GetProperties().Where(x =>
                        AfterConvertDateFilters.Select(y =>
                            y.key.ToLower()).Contains(x.Name.ToLower())).ToList();

                    List<int> LibraryAttributeActivatedIds = new List<int>();
                    bool AttrLibExist = false;

                    if (LibraryProps != null)
                    {
                        AttrLibExist = true;

                        List<DateFilterViewModel> LibraryPropsAttributeFilters = AfterConvertDateFilters.Where(x =>
                            LibraryProps.Select(y => y.Name.ToLower()).Contains(x.key.ToLower())).ToList();

                        IEnumerable<TLIradioAntennaLibrary> Libraries = _unitOfWork.RadioAntennaLibraryRepository.GetWhere(x => !x.Deleted).AsEnumerable();

                        foreach (DateFilterViewModel LibraryProp in LibraryPropsAttributeFilters)
                        {
                            Libraries = Libraries.Where(x => LibraryProps.Exists(y => (LibraryProp.key.ToLower() == y.Name.ToLower()) && ((y.GetValue(_mapper.Map<RadioAntennaLibraryViewModel>(x), null) != null) ?
                                ((LibraryProp.DateFrom >= Convert.ToDateTime(y.GetValue(_mapper.Map<RadioAntennaLibraryViewModel>(x), null))) &&
                                    (LibraryProp.DateTo <= Convert.ToDateTime(y.GetValue(_mapper.Map<RadioAntennaLibraryViewModel>(x), null)))) : (false))));
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

                    WithDateFilterRadioAntennaLibraries = _mapper.Map<List<RadioAntennaLibraryViewModel>>(_unitOfWork.RadioAntennaLibraryRepository.GetWhere(x =>
                        x.Id > 0 && IntersectLibraryIds.Contains(x.Id) && !x.Deleted).ToList());
                }

                //
                // Intersect Between WithoutDateFilterRadioAntennaLibraries + WithDateFilterRadioAntennaLibraries To Get The Records That Meet The Filters (DateFilters + AttributeFilters)
                //
                if ((AttributeFilters != null ? AttributeFilters.Count() == 0 : true) &&
                    (AfterConvertDateFilters != null ? AfterConvertDateFilters.Count() == 0 : true))
                {
                    RadioAntennaLibraries = _mapper.Map<List<RadioAntennaLibraryViewModel>>(_unitOfWork.RadioAntennaLibraryRepository.GetWhere(x =>
                        x.Id > 0 && !x.Deleted).ToList());
                }
                else if ((AttributeFilters != null ? AttributeFilters.Count > 0 : false) &&
                        (AfterConvertDateFilters != null ? AfterConvertDateFilters.Count() > 0 : false))
                {
                    List<int> RadioAntennaIds = WithoutDateFilterRadioAntennaLibraries.Select(x => x.Id).Intersect(WithDateFilterRadioAntennaLibraries.Select(x => x.Id)).ToList();
                    RadioAntennaLibraries = _mapper.Map<List<RadioAntennaLibraryViewModel>>(_unitOfWork.RadioAntennaLibraryRepository.GetWhere(x =>
                        RadioAntennaIds.Contains(x.Id)).ToList());
                }
                else if (AttributeFilters != null ? AttributeFilters.Count > 0 : false)
                {
                    RadioAntennaLibraries = WithoutDateFilterRadioAntennaLibraries;
                }
                else if (AfterConvertDateFilters != null ? AfterConvertDateFilters.Count() > 0 : false)
                {
                    RadioAntennaLibraries = WithDateFilterRadioAntennaLibraries;
                }

                Count = RadioAntennaLibraries.Count();

                RadioAntennaLibraries = RadioAntennaLibraries.Skip((parameterPagination.PageNumber - 1) * parameterPagination.PageSize).
                    Take(parameterPagination.PageSize).ToList();

                List<TLIattributeViewManagment> AllAttributes = _unitOfWork.AttributeViewManagmentRepository.GetIncludeWhere(x =>
                   (x.Enable && x.EditableManagmentView.View == Helpers.Constants.EditableManamgmantViewNames.RadioAntennaLibrary.ToString() &&
                   (x.AttributeActivatedId != null ?
                        (x.AttributeActivated.Tabel == Helpers.Constants.TablesNames.TLIradioAntennaLibrary.ToString() && x.AttributeActivated.enable) :
                        (x.DynamicAtt.LibraryAtt && !x.DynamicAtt.disable && x.DynamicAtt.tablesNames.TableName == Helpers.Constants.TablesNames.TLIradioAntennaLibrary.ToString()))) ||
                    (x.AttributeActivated != null ?
                        ((x.AttributeActivated.Key.ToLower() == "id" || x.AttributeActivated.Key.ToLower() == "active") && x.AttributeActivated.Tabel == Helpers.Constants.TablesNames.TLIradioAntennaLibrary.ToString()) : false),
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

                foreach (RadioAntennaLibraryViewModel RadioAntennaLibraryViewModel in RadioAntennaLibraries)
                {
                    dynamic DynamicRadioAntennaLibrary = new ExpandoObject();

                    //
                    // Library Object ViewModel... (Not DateTime DataType Attribute)
                    //
                    if (NotDateTimeLibraryAttributesViewModel != null ? NotDateTimeLibraryAttributesViewModel.Count > 0 : false)
                    {
                        List<PropertyInfo> LibraryProps = typeof(RadioAntennaLibraryViewModel).GetProperties().Where(x =>
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
                                object ForeignKeyNamePropObject = prop.GetValue(RadioAntennaLibraryViewModel, null);
                                ((IDictionary<String, Object>)DynamicRadioAntennaLibrary).Add(new KeyValuePair<string, object>(prop.Name, ForeignKeyNamePropObject));
                            }
                            else if (NotDateTimeLibraryAttributesViewModel.Select(x =>
                                 x.AttributeActivated.Key.ToLower()).Contains(prop.Name.ToLower()) &&
                                !prop.Name.ToLower().Contains("_name") &&
                                (prop.Name.ToLower().Substring(Math.Max(0, prop.Name.Length - 2)) != "id" || prop.Name.ToLower() == "id"))
                            {
                                if (prop.Name.ToLower() != "id" && prop.Name.ToLower() != "active")
                                {
                                    TLIattributeViewManagment LabelName = AllAttributes.FirstOrDefault(x => ((x.AttributeActivated != null) ? x.AttributeActivated.Key == prop.Name : false) &&
                                        x.AttributeActivated.Tabel == Helpers.Constants.TablesNames.TLIradioAntennaLibrary.ToString() &&
                                        x.Enable && x.AttributeActivated.DataType != "List" && x.Id != 0);

                                    if (LabelName != null)
                                    {
                                        object PropObject = prop.GetValue(RadioAntennaLibraryViewModel, null);
                                        ((IDictionary<String, Object>)DynamicRadioAntennaLibrary).Add(new KeyValuePair<string, object>(LabelName.AttributeActivated.Label, PropObject));
                                    }
                                }
                                else
                                {
                                    object PropObject = prop.GetValue(RadioAntennaLibraryViewModel, null);
                                    ((IDictionary<String, Object>)DynamicRadioAntennaLibrary).Add(new KeyValuePair<string, object>(prop.Name, PropObject));
                                }
                            }
                        }
                    }

                    //
                    // Library Dynamic Attributes... (Not DateTime DataType Attribute)
                    // 
                    List<TLIdynamicAtt> NotDateTimeLibraryDynamicAttributes = _unitOfWork.DynamicAttRepository.GetIncludeWhere(x =>
                       !x.disable && x.tablesNames.TableName == Helpers.Constants.TablesNames.TLIradioAntennaLibrary.ToString() &&
                        x.LibraryAtt && x.DataType.Name.ToLower() != "datetime" &&
                        NotDateTimeDynamicLibraryAttributesViewModel.AsEnumerable().Select(y => y.DynamicAttId).Contains(x.Id), x => x.tablesNames, x => x.DataType).ToList();

                    foreach (var LibraryDynamicAtt in NotDateTimeLibraryDynamicAttributes)
                    {
                        TLIdynamicAttLibValue DynamicAttLibValue = _unitOfWork.DynamicAttLibRepository.GetIncludeWhereFirst(x =>
                            x.DynamicAttId == LibraryDynamicAtt.Id &&
                            x.InventoryId == RadioAntennaLibraryViewModel.Id && !x.disable &&
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

                            ((IDictionary<String, Object>)DynamicRadioAntennaLibrary).Add(new KeyValuePair<string, object>(LibraryDynamicAtt.Key, DynamicAttValue));
                        }
                        else
                        {
                            ((IDictionary<String, Object>)DynamicRadioAntennaLibrary).Add(new KeyValuePair<string, object>(LibraryDynamicAtt.Key, null));
                        }
                    }

                    //
                    // Library Object ViewModel... (DateTime DataType Attribute)
                    //
                    dynamic DateTimeAttributes = new ExpandoObject();
                    if (DateTimeLibraryAttributesViewModel != null ? DateTimeLibraryAttributesViewModel.Count() > 0 : false)
                    {
                        List<PropertyInfo> DateTimeLibraryProps = typeof(RadioAntennaLibraryViewModel).GetProperties().Where(x =>
                            x.PropertyType.GenericTypeArguments != null ?
                                (x.PropertyType.GenericTypeArguments.Count() > 0 ? x.PropertyType.GenericTypeArguments.FirstOrDefault().Name == "datetime" :
                                (x.PropertyType.Name.ToLower() == "datetime")) :
                            (x.PropertyType.Name.ToLower() == "datetime")).ToList();

                        foreach (PropertyInfo prop in DateTimeLibraryProps)
                        {
                            TLIattributeViewManagment LabelName = AllAttributes.FirstOrDefault(x => ((x.AttributeActivated != null) ? x.AttributeActivated.Key == prop.Name : false) &&
                                x.AttributeActivated.Tabel == Helpers.Constants.TablesNames.TLIradioAntennaLibrary.ToString() &&
                                x.Enable && x.AttributeActivated.DataType != "List" && x.Id != 0);

                            if (LabelName != null)
                            {
                                object PropObject = prop.GetValue(RadioAntennaLibraryViewModel, null);
                                ((IDictionary<String, Object>)DateTimeAttributes).Add(new KeyValuePair<string, object>(LabelName.AttributeActivated.Label, PropObject));
                            }
                        }
                    }

                    //
                    // Library Dynamic Attributes... (DateTime DataType Attribute)
                    // 
                    List<TLIdynamicAtt> LibraryDynamicAttributes = _unitOfWork.DynamicAttRepository.GetIncludeWhere(x =>
                       !x.disable && x.tablesNames.TableName == Helpers.Constants.TablesNames.TLIradioAntennaLibrary.ToString() &&
                        x.LibraryAtt && x.DataType.Name.ToLower() == "datetime" &&
                        DateTimeDynamicLibraryAttributesViewModel.AsEnumerable().Select(y => y.DynamicAttId).Contains(x.Id), x => x.tablesNames).ToList();

                    foreach (TLIdynamicAtt LibraryDynamicAtt in LibraryDynamicAttributes)
                    {
                        TLIdynamicAttLibValue DynamicAttLibValue = _unitOfWork.DynamicAttLibRepository.GetIncludeWhereFirst(x =>
                            x.DynamicAttId == LibraryDynamicAtt.Id &&
                            x.InventoryId == RadioAntennaLibraryViewModel.Id && !x.disable &&
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

                    ((IDictionary<String, Object>)DynamicRadioAntennaLibrary).Add(new KeyValuePair<string, object>("DateTimeAttributes", DateTimeAttributes));

                    OutPutList.Add(DynamicRadioAntennaLibrary);
                }

                RadioAntennaTableDisplay.Model = OutPutList;

                RadioAntennaTableDisplay.filters = _unitOfWork.RadioAntennaLibraryRepository.GetRelatedTables();

                return new Response<ReturnWithFilters<object>>(true, RadioAntennaTableDisplay, null, null, (int)Helpers.Constants.ApiReturnCode.success, Count);
            }
            catch (Exception err)
            {
                return new Response<ReturnWithFilters<object>>(false, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
        }
        public Response<ReturnWithFilters<object>> GetRadioRRULibrariesWithEnabledAttribute(CombineFilters CombineFilters, ParameterPagination parameterPagination)
        {
            try
            {
                List<FilterObjectList> ObjectAttributeFilters = CombineFilters.filters;
                List<DateFilterViewModel> DateFilter = CombineFilters.DateFilter;
                int Count = 0;
                List<object> OutPutList = new List<object>();
                ReturnWithFilters<object> RadioRRUTableDisplay = new ReturnWithFilters<object>();

                List<StringFilterObjectList> AttributeFilters = new List<StringFilterObjectList>();

                List<RadioRRULibraryViewModel> RadioRRULibraries = new List<RadioRRULibraryViewModel>();
                List<RadioRRULibraryViewModel> WithoutDateFilterRadioRRULibraries = new List<RadioRRULibraryViewModel>();
                List<RadioRRULibraryViewModel> WithDateFilterRadioRRULibraries = new List<RadioRRULibraryViewModel>();

                List<TLIattributeActivated> RadioRRULibraryAttribute = new List<TLIattributeActivated>();
                if ((DateFilter != null ? DateFilter.Count() > 0 : false) ||
                    (ObjectAttributeFilters != null && ObjectAttributeFilters.Count > 0))
                {
                    RadioRRULibraryAttribute = _unitOfWork.AttributeViewManagmentRepository.GetIncludeWhere(x =>
                        x.Enable && x.AttributeActivatedId != null &&
                        x.AttributeActivated.DataType.ToLower() != "datetime" &&
                        x.EditableManagmentView.View == Helpers.Constants.EditableManamgmantViewNames.RadioRRULibrary.ToString() &&
                        x.EditableManagmentView.TLItablesNames1.TableName == Helpers.Constants.TablesNames.TLIradioRRULibrary.ToString(),
                            x => x.AttributeActivated, x => x.EditableManagmentView, x => x.EditableManagmentView.TLItablesNames1)
                    .Select(x => x.AttributeActivated).ToList();
                }

                if (ObjectAttributeFilters != null && ObjectAttributeFilters.Count > 0)
                {
                    List<TLIattributeActivated> NotDateDateRadioRRULibraryAttribute = RadioRRULibraryAttribute.Where(x =>
                        x.DataType.ToLower() != "datetime").ToList();

                    foreach (FilterObjectList item in ObjectAttributeFilters)
                    {
                        List<string> value = item.value.Select(x => x.ToString().ToLower()).ToList();

                        TLIattributeActivated AttributeKey = NotDateDateRadioRRULibraryAttribute.FirstOrDefault(x =>
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
                        x.tablesNames.TableName == Helpers.Constants.TablesNames.TLIradioRRULibrary.ToString(), x => x.tablesNames, x => x.DataType).ToList();

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
                    bool AttrLibExist = typeof(RadioRRULibraryViewModel).GetProperties().ToList().Exists(x =>
                        AttributeFilters.Where(y => y.key.ToLower() != "Id".ToLower()).Select(y =>
                            y.key.ToLower()).Contains(x.Name.ToLower()));

                    List<int> LibraryAttributeActivatedIds = new List<int>();

                    if (AttrLibExist)
                    {
                        List<PropertyInfo> NonStringLibraryProps = typeof(RadioRRULibraryViewModel).GetProperties().Where(x =>
                            x.PropertyType.Name.ToLower() != "string" &&
                            AttributeFilters.Select(y =>
                                y.key.ToLower()).Contains(x.Name.ToLower())).ToList();

                        List<PropertyInfo> StringLibraryProps = typeof(RadioRRULibraryViewModel).GetProperties().Where(x =>
                            x.PropertyType.Name.ToLower() == "string" &&
                            AttributeFilters.Select(y =>
                                y.key.ToLower()).Contains(x.Name.ToLower())).ToList();

                        List<StringFilterObjectList> LibraryPropsAttributeFilters = AttributeFilters.Where(x =>
                            NonStringLibraryProps.Select(y => y.Name.ToLower()).Contains(x.key.ToLower()) ||
                            StringLibraryProps.Select(y => y.Name.ToLower()).Contains(x.key.ToLower())).ToList();

                        IEnumerable<TLIradioRRULibrary> Libraries = _unitOfWork.RadioRRULibraryRepository.GetWhere(x => !x.Deleted).AsEnumerable();

                        foreach (StringFilterObjectList LibraryProp in LibraryPropsAttributeFilters)
                        {
                            if (StringLibraryProps.Select(x => x.Name.ToLower()).Contains(LibraryProp.key.ToLower()))
                            {
                                Libraries = Libraries.Where(x => StringLibraryProps.AsEnumerable().FirstOrDefault(y => (LibraryProp.key.ToLower() == y.Name.ToLower()) && (LibraryProp.value.AsEnumerable().FirstOrDefault(w =>
                                     y.GetValue(_mapper.Map<RadioRRULibraryViewModel>(x), null) != null ? y.GetValue(_mapper.Map<RadioRRULibraryViewModel>(x), null).ToString().ToLower().StartsWith(w.ToLower()) : false) != null)) != null).AsEnumerable();
                            }
                            else if (NonStringLibraryProps.Select(x => x.Name.ToLower()).Contains(LibraryProp.key.ToLower()))
                            {
                                Libraries = Libraries.Where(x => NonStringLibraryProps.AsEnumerable().FirstOrDefault(y => (LibraryProp.key.ToLower() == y.Name.ToLower()) && (y.GetValue(_mapper.Map<RadioRRULibraryViewModel>(x), null) != null ?
                                    LibraryProp.value.AsEnumerable().Contains(y.GetValue(_mapper.Map<RadioRRULibraryViewModel>(x), null).ToString().ToLower()) : false)) != null).AsEnumerable();
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

                    WithoutDateFilterRadioRRULibraries = _mapper.Map<List<RadioRRULibraryViewModel>>(_unitOfWork.RadioRRULibraryRepository.GetWhere(x =>
                        x.Id > 0 && IntersectLibraryIds.Contains(x.Id) && !x.Deleted).ToList());
                }

                //
                // DateTime Objects Filters..
                //
                List<DateFilterViewModel> AfterConvertDateFilters = new List<DateFilterViewModel>();
                if (DateFilter != null ? DateFilter.Count() > 0 : false)
                {
                    List<TLIattributeActivated> DateRadioRRULibraryAttribute = RadioRRULibraryAttribute.Where(x =>
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

                        TLIattributeActivated AttributeKey = DateRadioRRULibraryAttribute.FirstOrDefault(x =>
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
                        x.tablesNames.TableName == Helpers.Constants.TablesNames.TLIradioRRULibrary.ToString(), x => x.tablesNames).ToList();

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
                    List<PropertyInfo> LibraryProps = typeof(RadioRRULibraryViewModel).GetProperties().Where(x =>
                        AfterConvertDateFilters.Select(y =>
                            y.key.ToLower()).Contains(x.Name.ToLower())).ToList();

                    List<int> LibraryAttributeActivatedIds = new List<int>();
                    bool AttrLibExist = false;

                    if (LibraryProps != null)
                    {
                        AttrLibExist = true;

                        List<DateFilterViewModel> LibraryPropsAttributeFilters = AfterConvertDateFilters.Where(x =>
                            LibraryProps.Select(y => y.Name.ToLower()).Contains(x.key.ToLower())).ToList();

                        IEnumerable<TLIradioRRULibrary> Libraries = _unitOfWork.RadioRRULibraryRepository.GetWhere(x => !x.Deleted).AsEnumerable();

                        foreach (DateFilterViewModel LibraryProp in LibraryPropsAttributeFilters)
                        {
                            Libraries = Libraries.Where(x => LibraryProps.Exists(y => (LibraryProp.key.ToLower() == y.Name.ToLower()) && ((y.GetValue(_mapper.Map<RadioRRULibraryViewModel>(x), null) != null) ?
                                ((LibraryProp.DateFrom >= Convert.ToDateTime(y.GetValue(_mapper.Map<RadioRRULibraryViewModel>(x), null))) &&
                                    (LibraryProp.DateTo <= Convert.ToDateTime(y.GetValue(_mapper.Map<RadioRRULibraryViewModel>(x), null)))) : (false))));
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

                    WithDateFilterRadioRRULibraries = _mapper.Map<List<RadioRRULibraryViewModel>>(_unitOfWork.RadioRRULibraryRepository.GetWhere(x =>
                        x.Id > 0 && IntersectLibraryIds.Contains(x.Id) && !x.Deleted).ToList());
                }

                //
                // Intersect Between WithoutDateFilterRadioRRULibraries + WithDateFilterRadioRRULibraries To Get The Records That Meet The Filters (DateFilters + AttributeFilters)
                //
                if ((AttributeFilters != null ? AttributeFilters.Count() == 0 : true) &&
                    (AfterConvertDateFilters != null ? AfterConvertDateFilters.Count() == 0 : true))
                {
                    RadioRRULibraries = _mapper.Map<List<RadioRRULibraryViewModel>>(_unitOfWork.RadioRRULibraryRepository.GetWhere(x =>
                        x.Id > 0 && !x.Deleted).ToList());
                }
                else if ((AttributeFilters != null ? AttributeFilters.Count > 0 : false) &&
                        (AfterConvertDateFilters != null ? AfterConvertDateFilters.Count() > 0 : false))
                {
                    List<int> RadioRRUIds = WithoutDateFilterRadioRRULibraries.Select(x => x.Id).Intersect(WithDateFilterRadioRRULibraries.Select(x => x.Id)).ToList();
                    RadioRRULibraries = _mapper.Map<List<RadioRRULibraryViewModel>>(_unitOfWork.RadioRRULibraryRepository.GetWhere(x =>
                        RadioRRUIds.Contains(x.Id)).ToList());
                }
                else if (AttributeFilters != null ? AttributeFilters.Count > 0 : false)
                {
                    RadioRRULibraries = WithoutDateFilterRadioRRULibraries;
                }
                else if (AfterConvertDateFilters != null ? AfterConvertDateFilters.Count() > 0 : false)
                {
                    RadioRRULibraries = WithDateFilterRadioRRULibraries;
                }

                Count = RadioRRULibraries.Count();

                RadioRRULibraries = RadioRRULibraries.Skip((parameterPagination.PageNumber - 1) * parameterPagination.PageSize).
                    Take(parameterPagination.PageSize).ToList();

                List<TLIattributeViewManagment> AllAttributes = _unitOfWork.AttributeViewManagmentRepository.GetIncludeWhere(x =>
                   (x.Enable && x.EditableManagmentView.View == Helpers.Constants.EditableManamgmantViewNames.RadioRRULibrary.ToString() &&
                   (x.AttributeActivatedId != null ?
                        (x.AttributeActivated.Tabel == Helpers.Constants.TablesNames.TLIradioRRULibrary.ToString() && x.AttributeActivated.enable) :
                        (x.DynamicAtt.LibraryAtt && !x.DynamicAtt.disable && x.DynamicAtt.tablesNames.TableName == Helpers.Constants.TablesNames.TLIradioRRULibrary.ToString()))) ||
                    (x.AttributeActivated != null ?
                        ((x.AttributeActivated.Key.ToLower() == "id" || x.AttributeActivated.Key.ToLower() == "active") && x.AttributeActivated.Tabel == Helpers.Constants.TablesNames.TLIradioRRULibrary.ToString()) : false),
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

                foreach (RadioRRULibraryViewModel RadioRRULibraryViewModel in RadioRRULibraries)
                {
                    dynamic DynamicRadioRRULibrary = new ExpandoObject();

                    //
                    // Library Object ViewModel... (Not DateTime DataType Attribute)
                    //
                    if (NotDateTimeLibraryAttributesViewModel != null ? NotDateTimeLibraryAttributesViewModel.Count > 0 : false)
                    {
                        List<PropertyInfo> LibraryProps = typeof(RadioRRULibraryViewModel).GetProperties().Where(x =>
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
                                object ForeignKeyNamePropObject = prop.GetValue(RadioRRULibraryViewModel, null);
                                ((IDictionary<String, Object>)DynamicRadioRRULibrary).Add(new KeyValuePair<string, object>(prop.Name, ForeignKeyNamePropObject));
                            }
                            else if (NotDateTimeLibraryAttributesViewModel.Select(x =>
                                 x.AttributeActivated.Key.ToLower()).Contains(prop.Name.ToLower()) &&
                                !prop.Name.ToLower().Contains("_name") &&
                                (prop.Name.ToLower().Substring(Math.Max(0, prop.Name.Length - 2)) != "id" || prop.Name.ToLower() == "id"))
                            {
                                if (prop.Name.ToLower() != "id" && prop.Name.ToLower() != "active")
                                {
                                    TLIattributeViewManagment LabelName = AllAttributes.FirstOrDefault(x => ((x.AttributeActivated != null) ? x.AttributeActivated.Key == prop.Name : false) &&
                                        x.AttributeActivated.Tabel == Helpers.Constants.TablesNames.TLIradioRRULibrary.ToString() &&
                                        x.Enable && x.AttributeActivated.DataType != "List" && x.Id != 0);

                                    if (LabelName != null)
                                    {
                                        object PropObject = prop.GetValue(RadioRRULibraryViewModel, null);
                                        ((IDictionary<String, Object>)DynamicRadioRRULibrary).Add(new KeyValuePair<string, object>(LabelName.AttributeActivated.Label, PropObject));
                                    }
                                }
                                else
                                {
                                    object PropObject = prop.GetValue(RadioRRULibraryViewModel, null);
                                    ((IDictionary<String, Object>)DynamicRadioRRULibrary).Add(new KeyValuePair<string, object>(prop.Name, PropObject));
                                }
                            }
                        }
                    }

                    //
                    // Library Dynamic Attributes... (Not DateTime DataType Attribute)
                    // 
                    List<TLIdynamicAtt> NotDateTimeLibraryDynamicAttributes = _unitOfWork.DynamicAttRepository.GetIncludeWhere(x =>
                       !x.disable && x.tablesNames.TableName == Helpers.Constants.TablesNames.TLIradioRRULibrary.ToString() &&
                        x.LibraryAtt && x.DataType.Name.ToLower() != "datetime" &&
                        NotDateTimeDynamicLibraryAttributesViewModel.AsEnumerable().Select(y => y.DynamicAttId).Contains(x.Id), x => x.tablesNames, x => x.DataType).ToList();

                    foreach (var LibraryDynamicAtt in NotDateTimeLibraryDynamicAttributes)
                    {
                        TLIdynamicAttLibValue DynamicAttLibValue = _unitOfWork.DynamicAttLibRepository.GetIncludeWhereFirst(x =>
                            x.DynamicAttId == LibraryDynamicAtt.Id &&
                            x.InventoryId == RadioRRULibraryViewModel.Id && !x.disable &&
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

                            ((IDictionary<String, Object>)DynamicRadioRRULibrary).Add(new KeyValuePair<string, object>(LibraryDynamicAtt.Key, DynamicAttValue));
                        }
                        else
                        {
                            ((IDictionary<String, Object>)DynamicRadioRRULibrary).Add(new KeyValuePair<string, object>(LibraryDynamicAtt.Key, null));
                        }
                    }

                    //
                    // Library Object ViewModel... (DateTime DataType Attribute)
                    //
                    dynamic DateTimeAttributes = new ExpandoObject();
                    if (DateTimeLibraryAttributesViewModel != null ? DateTimeLibraryAttributesViewModel.Count() > 0 : false)
                    {
                        List<PropertyInfo> DateTimeLibraryProps = typeof(RadioRRULibraryViewModel).GetProperties().Where(x =>
                            x.PropertyType.GenericTypeArguments != null ?
                                (x.PropertyType.GenericTypeArguments.Count() > 0 ? x.PropertyType.GenericTypeArguments.FirstOrDefault().Name == "datetime" :
                                (x.PropertyType.Name.ToLower() == "datetime")) :
                            (x.PropertyType.Name.ToLower() == "datetime")).ToList();

                        foreach (PropertyInfo prop in DateTimeLibraryProps)
                        {
                            TLIattributeViewManagment LabelName = AllAttributes.FirstOrDefault(x => ((x.AttributeActivated != null) ? x.AttributeActivated.Key == prop.Name : false) &&
                                x.AttributeActivated.Tabel == Helpers.Constants.TablesNames.TLIradioRRULibrary.ToString() &&
                                x.Enable && x.AttributeActivated.DataType != "List" && x.Id != 0);

                            if (LabelName != null)
                            {
                                object PropObject = prop.GetValue(RadioRRULibraryViewModel, null);
                                ((IDictionary<String, Object>)DateTimeAttributes).Add(new KeyValuePair<string, object>(LabelName.AttributeActivated.Label, PropObject));
                            }
                        }
                    }

                    //
                    // Library Dynamic Attributes... (DateTime DataType Attribute)
                    // 
                    List<TLIdynamicAtt> LibraryDynamicAttributes = _unitOfWork.DynamicAttRepository.GetIncludeWhere(x =>
                       !x.disable && x.tablesNames.TableName == Helpers.Constants.TablesNames.TLIradioRRULibrary.ToString() &&
                        x.LibraryAtt && x.DataType.Name.ToLower() == "datetime" &&
                        DateTimeDynamicLibraryAttributesViewModel.AsEnumerable().Select(y => y.DynamicAttId).Contains(x.Id), x => x.tablesNames).ToList();

                    foreach (TLIdynamicAtt LibraryDynamicAtt in LibraryDynamicAttributes)
                    {
                        TLIdynamicAttLibValue DynamicAttLibValue = _unitOfWork.DynamicAttLibRepository.GetIncludeWhereFirst(x =>
                            x.DynamicAttId == LibraryDynamicAtt.Id &&
                            x.InventoryId == RadioRRULibraryViewModel.Id && !x.disable &&
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

                    ((IDictionary<String, Object>)DynamicRadioRRULibrary).Add(new KeyValuePair<string, object>("DateTimeAttributes", DateTimeAttributes));

                    OutPutList.Add(DynamicRadioRRULibrary);
                }

                RadioRRUTableDisplay.Model = OutPutList;
                RadioRRUTableDisplay.filters = _unitOfWork.RadioRRULibraryRepository.GetRelatedTables();

                return new Response<ReturnWithFilters<object>>(true, RadioRRUTableDisplay, null, null, (int)Helpers.Constants.ApiReturnCode.success, Count);
            }
            catch (Exception err)
            {
                return new Response<ReturnWithFilters<object>>(false, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
        }
        public Response<ReturnWithFilters<object>> GetRadioOtherLibrariesWithEnabledAttribute(CombineFilters CombineFilters, ParameterPagination parameterPagination)
        {
            try
            {
                List<FilterObjectList> ObjectAttributeFilters = CombineFilters.filters;
                List<DateFilterViewModel> DateFilter = CombineFilters.DateFilter;
                int Count = 0;
                List<object> OutPutList = new List<object>();
                ReturnWithFilters<object> RadioOtherTableDisplay = new ReturnWithFilters<object>();

                List<StringFilterObjectList> AttributeFilters = new List<StringFilterObjectList>();

                List<RadioOtherLibraryViewModel> RadioOtherLibraries = new List<RadioOtherLibraryViewModel>();
                List<RadioOtherLibraryViewModel> WithoutDateFilterRadioOtherLibraries = new List<RadioOtherLibraryViewModel>();
                List<RadioOtherLibraryViewModel> WithDateFilterRadioOtherLibraries = new List<RadioOtherLibraryViewModel>();

                List<TLIattributeActivated> RadioOtherLibraryAttribute = new List<TLIattributeActivated>();
                if ((DateFilter != null ? DateFilter.Count() > 0 : false) ||
                    (ObjectAttributeFilters != null && ObjectAttributeFilters.Count > 0))
                {
                    RadioOtherLibraryAttribute = _unitOfWork.AttributeViewManagmentRepository.GetIncludeWhere(x =>
                        x.Enable && x.AttributeActivatedId != null &&
                        x.AttributeActivated.DataType.ToLower() != "datetime" &&
                        x.EditableManagmentView.View == Helpers.Constants.EditableManamgmantViewNames.RadioOtherLibrary.ToString() &&
                        x.EditableManagmentView.TLItablesNames1.TableName == Helpers.Constants.TablesNames.TLIradioOtherLibrary.ToString(),
                            x => x.AttributeActivated, x => x.EditableManagmentView, x => x.EditableManagmentView.TLItablesNames1)
                    .Select(x => x.AttributeActivated).ToList();
                }

                if (ObjectAttributeFilters != null && ObjectAttributeFilters.Count > 0)
                {
                    List<TLIattributeActivated> NotDateDateRadioOtherLibraryAttribute = RadioOtherLibraryAttribute.Where(x =>
                        x.DataType.ToLower() != "datetime").ToList();

                    foreach (FilterObjectList item in ObjectAttributeFilters)
                    {
                        List<string> value = item.value.Select(x => x.ToString().ToLower()).ToList();

                        TLIattributeActivated AttributeKey = NotDateDateRadioOtherLibraryAttribute.FirstOrDefault(x =>
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
                        x.tablesNames.TableName == Helpers.Constants.TablesNames.TLIradioOtherLibrary.ToString(), x => x.tablesNames, x => x.DataType).ToList();

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
                    bool AttrLibExist = typeof(RadioOtherLibraryViewModel).GetProperties().ToList().Exists(x =>
                        AttributeFilters.Where(y => y.key.ToLower() != "Id".ToLower()).Select(y =>
                            y.key.ToLower()).Contains(x.Name.ToLower()));

                    List<int> LibraryAttributeActivatedIds = new List<int>();

                    if (AttrLibExist)
                    {
                        List<PropertyInfo> NonStringLibraryProps = typeof(RadioOtherLibraryViewModel).GetProperties().Where(x =>
                            x.PropertyType.Name.ToLower() != "string" &&
                            AttributeFilters.Select(y =>
                                y.key.ToLower()).Contains(x.Name.ToLower())).ToList();

                        List<PropertyInfo> StringLibraryProps = typeof(RadioOtherLibraryViewModel).GetProperties().Where(x =>
                            x.PropertyType.Name.ToLower() == "string" &&
                            AttributeFilters.Select(y =>
                                y.key.ToLower()).Contains(x.Name.ToLower())).ToList();

                        List<StringFilterObjectList> LibraryPropsAttributeFilters = AttributeFilters.Where(x =>
                            NonStringLibraryProps.Select(y => y.Name.ToLower()).Contains(x.key.ToLower()) ||
                            StringLibraryProps.Select(y => y.Name.ToLower()).Contains(x.key.ToLower())).ToList();

                        IEnumerable<TLIradioOtherLibrary> Libraries = _unitOfWork.RadioOtherLibraryRepository.GetWhere(x => !x.Deleted).AsEnumerable();

                        foreach (StringFilterObjectList LibraryProp in LibraryPropsAttributeFilters)
                        {
                            if (StringLibraryProps.Select(x => x.Name.ToLower()).Contains(LibraryProp.key.ToLower()))
                            {
                                Libraries = Libraries.Where(x => StringLibraryProps.AsEnumerable().FirstOrDefault(y => (LibraryProp.key.ToLower() == y.Name.ToLower()) && (LibraryProp.value.AsEnumerable().FirstOrDefault(w =>
                                     y.GetValue(_mapper.Map<RadioOtherLibraryViewModel>(x), null) != null ? y.GetValue(_mapper.Map<RadioOtherLibraryViewModel>(x), null).ToString().ToLower().StartsWith(w.ToLower()) : false) != null)) != null).AsEnumerable();
                            }
                            else if (NonStringLibraryProps.Select(x => x.Name.ToLower()).Contains(LibraryProp.key.ToLower()))
                            {
                                Libraries = Libraries.Where(x => NonStringLibraryProps.AsEnumerable().FirstOrDefault(y => (LibraryProp.key.ToLower() == y.Name.ToLower()) && (y.GetValue(_mapper.Map<RadioOtherLibraryViewModel>(x), null) != null ?
                                    LibraryProp.value.AsEnumerable().Contains(y.GetValue(_mapper.Map<RadioOtherLibraryViewModel>(x), null).ToString().ToLower()) : false)) != null).AsEnumerable();
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

                    WithoutDateFilterRadioOtherLibraries = _mapper.Map<List<RadioOtherLibraryViewModel>>(_unitOfWork.RadioOtherLibraryRepository.GetWhere(x =>
                        x.Id > 0 && IntersectLibraryIds.Contains(x.Id) && !x.Deleted).ToList());
                }

                //
                // DateTime Objects Filters..
                //
                List<DateFilterViewModel> AfterConvertDateFilters = new List<DateFilterViewModel>();
                if (DateFilter != null ? DateFilter.Count() > 0 : false)
                {
                    List<TLIattributeActivated> DateRadioOtherLibraryAttribute = RadioOtherLibraryAttribute.Where(x =>
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

                        TLIattributeActivated AttributeKey = DateRadioOtherLibraryAttribute.FirstOrDefault(x =>
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
                        x.tablesNames.TableName == Helpers.Constants.TablesNames.TLIradioOtherLibrary.ToString(), x => x.tablesNames).ToList();

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
                    List<PropertyInfo> LibraryProps = typeof(RadioOtherLibraryViewModel).GetProperties().Where(x =>
                        AfterConvertDateFilters.Select(y =>
                            y.key.ToLower()).Contains(x.Name.ToLower())).ToList();

                    List<int> LibraryAttributeActivatedIds = new List<int>();
                    bool AttrLibExist = false;

                    if (LibraryProps != null)
                    {
                        AttrLibExist = true;

                        List<DateFilterViewModel> LibraryPropsAttributeFilters = AfterConvertDateFilters.Where(x =>
                            LibraryProps.Select(y => y.Name.ToLower()).Contains(x.key.ToLower())).ToList();

                        IEnumerable<TLIradioOtherLibrary> Libraries = _unitOfWork.RadioOtherLibraryRepository.GetWhere(x => !x.Deleted).AsEnumerable();

                        foreach (DateFilterViewModel LibraryProp in LibraryPropsAttributeFilters)
                        {
                            Libraries = Libraries.Where(x => LibraryProps.Exists(y => (LibraryProp.key.ToLower() == y.Name.ToLower()) && ((y.GetValue(_mapper.Map<RadioOtherLibraryViewModel>(x), null) != null) ?
                                ((LibraryProp.DateFrom >= Convert.ToDateTime(y.GetValue(_mapper.Map<RadioOtherLibraryViewModel>(x), null))) &&
                                    (LibraryProp.DateTo <= Convert.ToDateTime(y.GetValue(_mapper.Map<RadioOtherLibraryViewModel>(x), null)))) : (false))));
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

                    WithDateFilterRadioOtherLibraries = _mapper.Map<List<RadioOtherLibraryViewModel>>(_unitOfWork.RadioOtherLibraryRepository.GetWhere(x =>
                        x.Id > 0 && IntersectLibraryIds.Contains(x.Id) && !x.Deleted).ToList());
                }

                //
                // Intersect Between WithoutDateFilterRadioOtherLibraries + WithDateFilterRadioOtherLibraries To Get The Records That Meet The Filters (DateFilters + AttributeFilters)
                //
                if ((AttributeFilters != null ? AttributeFilters.Count() == 0 : true) &&
                    (AfterConvertDateFilters != null ? AfterConvertDateFilters.Count() == 0 : true))
                {
                    RadioOtherLibraries = _mapper.Map<List<RadioOtherLibraryViewModel>>(_unitOfWork.RadioOtherLibraryRepository.GetWhere(x =>
                        x.Id > 0 && !x.Deleted).ToList());
                }
                else if ((AttributeFilters != null ? AttributeFilters.Count > 0 : false) &&
                        (AfterConvertDateFilters != null ? AfterConvertDateFilters.Count() > 0 : false))
                {
                    List<int> RadioOtherIds = WithoutDateFilterRadioOtherLibraries.Select(x => x.Id).Intersect(WithDateFilterRadioOtherLibraries.Select(x => x.Id)).ToList();
                    RadioOtherLibraries = _mapper.Map<List<RadioOtherLibraryViewModel>>(_unitOfWork.RadioOtherLibraryRepository.GetWhere(x =>
                        RadioOtherIds.Contains(x.Id)).ToList());
                }
                else if (AttributeFilters != null ? AttributeFilters.Count > 0 : false)
                {
                    RadioOtherLibraries = WithoutDateFilterRadioOtherLibraries;
                }
                else if (AfterConvertDateFilters != null ? AfterConvertDateFilters.Count() > 0 : false)
                {
                    RadioOtherLibraries = WithDateFilterRadioOtherLibraries;
                }

                Count = RadioOtherLibraries.Count();

                RadioOtherLibraries = RadioOtherLibraries.Skip((parameterPagination.PageNumber - 1) * parameterPagination.PageSize).
                    Take(parameterPagination.PageSize).ToList();

                List<TLIattributeViewManagment> AllAttributes = _unitOfWork.AttributeViewManagmentRepository.GetIncludeWhere(x =>
                   (x.Enable && x.EditableManagmentView.View == Helpers.Constants.EditableManamgmantViewNames.RadioOtherLibrary.ToString() &&
                   (x.AttributeActivatedId != null ?
                        (x.AttributeActivated.Tabel == Helpers.Constants.TablesNames.TLIradioOtherLibrary.ToString() && x.AttributeActivated.enable) :
                        (x.DynamicAtt.LibraryAtt && !x.DynamicAtt.disable && x.DynamicAtt.tablesNames.TableName == Helpers.Constants.TablesNames.TLIradioOtherLibrary.ToString()))) ||
                    (x.AttributeActivated != null ?
                        ((x.AttributeActivated.Key.ToLower() == "id" || x.AttributeActivated.Key.ToLower() == "active") && x.AttributeActivated.Tabel == Helpers.Constants.TablesNames.TLIradioOtherLibrary.ToString()) : false),
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

                foreach (RadioOtherLibraryViewModel RadioOtherLibraryViewModel in RadioOtherLibraries)
                {
                    dynamic DynamicRadioOtherLibrary = new ExpandoObject();

                    //
                    // Library Object ViewModel... (Not DateTime DataType Attribute)
                    //
                    if (NotDateTimeLibraryAttributesViewModel != null ? NotDateTimeLibraryAttributesViewModel.Count > 0 : false)
                    {
                        List<PropertyInfo> LibraryProps = typeof(RadioOtherLibraryViewModel).GetProperties().Where(x =>
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
                                object ForeignKeyNamePropObject = prop.GetValue(RadioOtherLibraryViewModel, null);
                                ((IDictionary<String, Object>)DynamicRadioOtherLibrary).Add(new KeyValuePair<string, object>(prop.Name, ForeignKeyNamePropObject));
                            }
                            else if (NotDateTimeLibraryAttributesViewModel.Select(x =>
                                 x.AttributeActivated.Key.ToLower()).Contains(prop.Name.ToLower()) &&
                                !prop.Name.ToLower().Contains("_name") &&
                                (prop.Name.ToLower().Substring(Math.Max(0, prop.Name.Length - 2)) != "id" || prop.Name.ToLower() == "id"))
                            {
                                if (prop.Name.ToLower() != "id" && prop.Name.ToLower() != "active")
                                {
                                    TLIattributeViewManagment LabelName = AllAttributes.FirstOrDefault(x => ((x.AttributeActivated != null) ? x.AttributeActivated.Key == prop.Name : false) &&
                                        x.AttributeActivated.Tabel == Helpers.Constants.TablesNames.TLIradioOtherLibrary.ToString() &&
                                        x.Enable && x.AttributeActivated.DataType != "List" && x.Id != 0);

                                    if (LabelName != null)
                                    {
                                        object PropObject = prop.GetValue(RadioOtherLibraryViewModel, null);
                                        ((IDictionary<String, Object>)DynamicRadioOtherLibrary).Add(new KeyValuePair<string, object>(LabelName.AttributeActivated.Label, PropObject));
                                    }
                                }
                                else
                                {
                                    object PropObject = prop.GetValue(RadioOtherLibraryViewModel, null);
                                    ((IDictionary<String, Object>)DynamicRadioOtherLibrary).Add(new KeyValuePair<string, object>(prop.Name, PropObject));
                                }
                            }
                        }
                    }

                    //
                    // Library Dynamic Attributes... (Not DateTime DataType Attribute)
                    // 
                    List<TLIdynamicAtt> NotDateTimeLibraryDynamicAttributes = _unitOfWork.DynamicAttRepository.GetIncludeWhere(x =>
                       !x.disable && x.tablesNames.TableName == Helpers.Constants.TablesNames.TLIradioOtherLibrary.ToString() &&
                        x.LibraryAtt && x.DataType.Name.ToLower() != "datetime" &&
                        NotDateTimeDynamicLibraryAttributesViewModel.AsEnumerable().Select(y => y.DynamicAttId).Contains(x.Id), x => x.tablesNames, x => x.DataType).ToList();

                    foreach (var LibraryDynamicAtt in NotDateTimeLibraryDynamicAttributes)
                    {
                        TLIdynamicAttLibValue DynamicAttLibValue = _unitOfWork.DynamicAttLibRepository.GetIncludeWhereFirst(x =>
                            x.DynamicAttId == LibraryDynamicAtt.Id &&
                            x.InventoryId == RadioOtherLibraryViewModel.Id && !x.disable &&
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

                            ((IDictionary<String, Object>)DynamicRadioOtherLibrary).Add(new KeyValuePair<string, object>(LibraryDynamicAtt.Key, DynamicAttValue));
                        }
                        else
                        {
                            ((IDictionary<String, Object>)DynamicRadioOtherLibrary).Add(new KeyValuePair<string, object>(LibraryDynamicAtt.Key, null));
                        }
                    }

                    //
                    // Library Object ViewModel... (DateTime DataType Attribute)
                    //
                    dynamic DateTimeAttributes = new ExpandoObject();
                    if (DateTimeLibraryAttributesViewModel != null ? DateTimeLibraryAttributesViewModel.Count() > 0 : false)
                    {
                        List<PropertyInfo> DateTimeLibraryProps = typeof(RadioOtherLibraryViewModel).GetProperties().Where(x =>
                            x.PropertyType.GenericTypeArguments != null ?
                                (x.PropertyType.GenericTypeArguments.Count() > 0 ? x.PropertyType.GenericTypeArguments.FirstOrDefault().Name == "datetime" :
                                (x.PropertyType.Name.ToLower() == "datetime")) :
                            (x.PropertyType.Name.ToLower() == "datetime")).ToList();

                        foreach (PropertyInfo prop in DateTimeLibraryProps)
                        {
                            TLIattributeViewManagment LabelName = AllAttributes.FirstOrDefault(x => ((x.AttributeActivated != null) ? x.AttributeActivated.Key == prop.Name : false) &&
                                x.AttributeActivated.Tabel == Helpers.Constants.TablesNames.TLIradioOtherLibrary.ToString() &&
                                x.Enable && x.AttributeActivated.DataType != "List" && x.Id != 0);

                            if (LabelName != null)
                            {
                                object PropObject = prop.GetValue(RadioOtherLibraryViewModel, null);
                                ((IDictionary<String, Object>)DateTimeAttributes).Add(new KeyValuePair<string, object>(LabelName.AttributeActivated.Label, PropObject));
                            }
                        }
                    }

                    //
                    // Library Dynamic Attributes... (DateTime DataType Attribute)
                    // 
                    List<TLIdynamicAtt> LibraryDynamicAttributes = _unitOfWork.DynamicAttRepository.GetIncludeWhere(x =>
                       !x.disable && x.tablesNames.TableName == Helpers.Constants.TablesNames.TLIradioOtherLibrary.ToString() &&
                        x.LibraryAtt && x.DataType.Name.ToLower() == "datetime" &&
                        DateTimeDynamicLibraryAttributesViewModel.AsEnumerable().Select(y => y.DynamicAttId).Contains(x.Id), x => x.tablesNames).ToList();

                    foreach (TLIdynamicAtt LibraryDynamicAtt in LibraryDynamicAttributes)
                    {
                        TLIdynamicAttLibValue DynamicAttLibValue = _unitOfWork.DynamicAttLibRepository.GetIncludeWhereFirst(x =>
                            x.DynamicAttId == LibraryDynamicAtt.Id &&
                            x.InventoryId == RadioOtherLibraryViewModel.Id && !x.disable &&
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

                    ((IDictionary<String, Object>)DynamicRadioOtherLibrary).Add(new KeyValuePair<string, object>("DateTimeAttributes", DateTimeAttributes));

                    OutPutList.Add(DynamicRadioOtherLibrary);
                }

                RadioOtherTableDisplay.Model = OutPutList;
                RadioOtherTableDisplay.filters = _unitOfWork.RadioOtherLibraryRepository.GetRelatedTables();

                return new Response<ReturnWithFilters<object>>(true, RadioOtherTableDisplay, null, null, (int)Helpers.Constants.ApiReturnCode.success, Count);
            }
            catch (Exception err)
            {
                return new Response<ReturnWithFilters<object>>(false, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
        }
        #endregion
        //Function take 2 parameters
        //get table name Entity by TableName
        //specify the table i deal with
        //get record by Id
        //get activated attributes 
        //get dynamic attributes
        public Response<GetForAddCivilLibrarybject> GetById(int Id, string TableName)
        {
            try
            {
                GetForAddCivilLibrarybject attributes = new GetForAddCivilLibrarybject();

                TLItablesNames TableNameEntity = _unitOfWork.TablesNamesRepository.GetWhereFirst(c =>
                    c.TableName == TableName);

                List<BaseAttView> ListAttributesActivated = new List<BaseAttView>();

                if (Helpers.Constants.LoadSubType.TLIradioAntennaLibrary.ToString() == TableName)
                {
                    TLIradioAntennaLibrary RadioAntennaLibrary = _unitOfWork.RadioAntennaLibraryRepository.GetIncludeWhereFirst(x =>
                        x.Id == Id && !x.Deleted && x.Active);
                    if (RadioAntennaLibrary != null)
                    {
                        List<BaseInstAttViews> listofAttributesActivated = _unitOfWork.AttributeActivatedRepository.GetAttributeActivatedGetForAdd(TableName, RadioAntennaLibrary, null).ToList();

                        attributes.LogisticalItems = _unitOfWork.LogistcalRepository.GetLogisticalsNonSteel(Helpers.Constants.TablePartName.Radio.ToString(), TableName, Id);
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
                            NameAttribute.Value = db.MV_RADIO_ANTENNA_LIBRARY_VIEW.FirstOrDefault(x => x.Id == Id)?.Model;
                        }
                    }
                    else
                    {
                        return new Response<GetForAddCivilLibrarybject>(false, null, null, "this RadioAntenna is not found", (int)Helpers.Constants.ApiReturnCode.fail);
                    }
                }
                else if (Helpers.Constants.LoadSubType.TLIradioRRULibrary.ToString() == TableName)
                {
                    TLIradioAntennaLibrary RadioRRULibrary = _unitOfWork.RadioAntennaLibraryRepository.GetIncludeWhereFirst(x =>
                      x.Id == Id && !x.Deleted && x.Active);
                    if (RadioRRULibrary != null)
                    {
                        List<BaseInstAttViews> listofAttributesActivated = _unitOfWork.AttributeActivatedRepository.GetAttributeActivatedGetForAdd(TableName, RadioRRULibrary, null).ToList();

                        attributes.LogisticalItems = _unitOfWork.LogistcalRepository.GetLogisticalsNonSteel(Helpers.Constants.TablePartName.Radio.ToString(), TableName, Id);
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
                            NameAttribute.Value = db.MV_RADIO_RRU_LIBRARY_VIEW.FirstOrDefault(x => x.Id == Id)?.Model;
                        }
                    }
                    else
                    {
                        return new Response<GetForAddCivilLibrarybject>(false, null, null, "this RadioRRU is not found", (int)Helpers.Constants.ApiReturnCode.fail);
                    }
                }
                //else if (Helpers.Constants.LoadSubType.TLIradioOtherLibrary.ToString() == TableName)
                //{
                //    TLIradioOtherLibrary RadioOtherLibrary = _unitOfWork.RadioOtherLibraryRepository.GetWhereFirst(x =>
                //        x.Id == Id);

                //    ListAttributesActivated = _unitOfWork.AttributeActivatedRepository.GetAttributeActivated(TableName, RadioOtherLibrary, null).ToList();
                //}
                return new Response<GetForAddCivilLibrarybject>(true, attributes, null, null, (int)Helpers.Constants.ApiReturnCode.success);
            }
            catch (Exception err)
            {
                return new Response<GetForAddCivilLibrarybject>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
        }
        public Response<GetEnableAttribute> GetRadioAntennaLibrariesEnabledAtt(string ConnectionString)
        {
            using (var connection = new OracleConnection(ConnectionString))
            {
                try
                {
                    GetEnableAttribute getEnableAttribute = new GetEnableAttribute();
                    connection.Open();
                
                    var attActivated = db.TLIattributeViewManagment
                        .Include(x => x.EditableManagmentView)
                        .Include(x => x.AttributeActivated)
                        .Include(x => x.DynamicAtt)
                        .Where(x => x.Enable && x.EditableManagmentView.View == "RadioAntennaLibrary"
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
                        var query = db.MV_RADIO_ANTENNA_LIBRARY_VIEW.Where(x => !x.Deleted).AsEnumerable()
                       .Select(item => _unitOfWork.CivilWithLegsRepository.BuildDynamicSelect(item, null, propertyNamesStatic, propertyNamesDynamic));
                        int count = query.Count();

                        getEnableAttribute.Model = query;
                        return new Response<GetEnableAttribute>(true, getEnableAttribute, null, "Success", (int)Helpers.Constants.ApiReturnCode.success, count);
                    }
                    else
                    {
                        var query = db.MV_RADIO_ANTENNA_LIBRARY_VIEW.Where(x => !x.Deleted).AsEnumerable()
                    .GroupBy(x => new
                    {
                        Id = x.Id,
                        Model = x.Model,
                        Notes = x.Notes,
                        FrequencyBand = x.FrequencyBand,
                        Weight = x.Weight,
                        Width = x.Width,
                        Depth = x.Depth,
                        Active = x.Active,
                        Deleted = x.Deleted,
                        Length = x.Length,
                        SpaceLibrary = x.SpaceLibrary,          

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
        //Function take 2 parameters
        //specify the table i deal with
        //map object to ViewModel
        //map ViewModel to Entity
        //check the validation
        //add Entity
        //add dynamic attributes values
        public Response<AllItemAttributes> AddRadioLibrary(string TableName, object RadioLibraryViewModel, string connectionString,int UserId)
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
                            TLItablesNames TableNameEntity = _unitOfWork.TablesNamesRepository.GetWhereFirst(l => l.TableName.ToLower() == TableName.ToLower());
                            if (Helpers.Constants.LoadSubType.TLIradioAntennaLibrary.ToString().ToLower() == TableName.ToLower())
                            {
                                AddRadioAntennaLibraryObject addRadioAntenna = _mapper.Map<AddRadioAntennaLibraryObject>(RadioLibraryViewModel);
                                TLIradioAntennaLibrary radioAntennaLibrary = _mapper.Map<TLIradioAntennaLibrary>(addRadioAntenna.AttributesActivatedLibrary);
                                string CheckDependencyValidation = CheckDependencyValidationForRadioTypes(RadioLibraryViewModel, TableName);

                                if (!string.IsNullOrEmpty(CheckDependencyValidation))
                                    return new Response<AllItemAttributes>(true, null, null, CheckDependencyValidation, (int)ApiReturnCode.fail);

                                string CheckGeneralValidation = CheckGeneralValidationFunctionLib(addRadioAntenna.DynamicAttributes, TableNameEntity.TableName);

                                if (!string.IsNullOrEmpty(CheckGeneralValidation))
                                    return new Response<AllItemAttributes>(true, null, null, CheckGeneralValidation, (int)ApiReturnCode.fail);

                                var CheckModel = _unitOfWork.RadioAntennaLibraryRepository
                                    .GetWhereFirst(x => x.Model == radioAntennaLibrary.Model && !x.Deleted);

                                if (CheckModel != null)
                                  return new Response<AllItemAttributes>(true, null, null, $"This model {radioAntennaLibrary.Model} is already exists", (int)Helpers.Constants.ApiReturnCode.fail);                              
                                
                                _unitOfWork.RadioAntennaLibraryRepository.AddWithHistory(UserId, radioAntennaLibrary);
                                _unitOfWork.SaveChanges();
                               
                                dynamic LogisticalItemIds = new ExpandoObject();
                                LogisticalItemIds = addRadioAntenna.LogisticalItems;
                                AddLogisticalItemWithRadio(LogisticalItemIds, radioAntennaLibrary, TableNameEntity.Id);
                                if (addRadioAntenna.DynamicAttributes.Count > 0)
                                {
                                    _unitOfWork.DynamicAttLibRepository.AddDynamicLibAtt(UserId, addRadioAntenna.DynamicAttributes, TableNameEntity.Id, radioAntennaLibrary.Id,connectionString);
                                }
                                _unitOfWork.TablesHistoryRepository.AddHistory(radioAntennaLibrary.Id, Helpers.Constants.HistoryType.Add.ToString().ToLower(), TablesNames.TLImwDishLibrary.ToString().ToLower());
                            }
                            else if (Helpers.Constants.LoadSubType.TLIradioOtherLibrary.ToString().ToLower() == TableName.ToLower())
                            {
                                AddRadioOtherLibraryObject addRadioOther = _mapper.Map<AddRadioOtherLibraryObject>(RadioLibraryViewModel);
                                TLIradioOtherLibrary radioOther = _mapper.Map<TLIradioOtherLibrary>(addRadioOther.LibraryAttribute);
                              
                                string CheckDependencyValidation = CheckDependencyValidationForRadioTypes(RadioLibraryViewModel, TableName);

                                if (!string.IsNullOrEmpty(CheckDependencyValidation))
                                    return new Response<AllItemAttributes>(true, null, null, CheckDependencyValidation, (int)ApiReturnCode.fail);

                                string CheckGeneralValidation = CheckGeneralValidationFunctionLib(addRadioOther.dynamicAttribute, TableNameEntity.TableName);

                                if (!string.IsNullOrEmpty(CheckGeneralValidation))
                                    return new Response<AllItemAttributes>(true, null, null, CheckGeneralValidation, (int)ApiReturnCode.fail);
                             
                                var CheckModel = _unitOfWork.RadioOtherLibraryRepository.GetWhereFirst(x => x.Model == radioOther.Model && !x.Deleted);
                                if (CheckModel != null)
                                return new Response<AllItemAttributes>(true, null, null, $"This model {radioOther.Model} is already exists", (int)Helpers.Constants.ApiReturnCode.fail);
                                             
                                _unitOfWork.RadioOtherLibraryRepository.AddWithHistory(Helpers.LogFilterAttribute.UserId, radioOther);
                                _unitOfWork.SaveChanges();

                                dynamic LogisticalItemIds = new ExpandoObject();
                                LogisticalItemIds = addRadioOther.LogisticalItems;

                                AddLogisticalItemWithRadio(LogisticalItemIds, radioOther, TableNameEntity.Id);

                                //if (addRadioOther.dynamicAttribute.Count > 0)
                                //{
                                //    _unitOfWork.DynamicAttLibRepository.AddDynamicLibAtt(addRadioOther.dynamicAttribute, TableNameEntity.Id, radioOther.Id);
                                //}
                                 //   _unitOfWork.TablesHistoryRepository.AddHistory(radioOther.Id, "Add", "TLIradioOtherLibrary");
                               
                            }
                            //else if (Helpers.Constants.LoadSubType.TLIradioRRULibrary.ToString().ToLower() == TableName.ToLower())
                            //{
                            //    AddRadioRRULibraryObject addRadioRRULibrary = _mapper.Map<AddRadioRRULibraryObject>(RadioLibraryViewModel);
                            //    TLIradioRRULibrary radioRRULibrary = _mapper.Map<TLIradioRRULibrary>(addRadioRRULibrary.LibraryAttribute);
                            //    if (radioRRULibrary.L_W_H_cm3 == null || radioRRULibrary.L_W_H_cm3 == "")
                            //    {
                            //        radioRRULibrary.L_W_H_cm3 = radioRRULibrary.Length + "_" + radioRRULibrary.Width + "_" + radioRRULibrary.Height;
                            //    }
                         
                            //    string CheckDependencyValidation = CheckDependencyValidationForRadioTypes(RadioLibraryViewModel, TableName);

                            //    if (!string.IsNullOrEmpty(CheckDependencyValidation))
                            //        return new Response<AllItemAttributes>(true, null, null, CheckDependencyValidation, (int)ApiReturnCode.fail);

                            //    string CheckGeneralValidation = CheckGeneralValidationFunctionLib(addRadioRRULibrary.dynamicAttribute, TableNameEntity.TableName);

                            //    if (!string.IsNullOrEmpty(CheckGeneralValidation))
                            //        return new Response<AllItemAttributes>(true, null, null, CheckGeneralValidation, (int)ApiReturnCode.fail);
                               
                            //    var CheckModel = _unitOfWork.RadioRRULibraryRepository.GetWhereFirst(x => x.Model == radioRRULibrary.Model && !x.Deleted);
                            //    if (CheckModel != null)
                            //    return new Response<AllItemAttributes>(true, null, null, $"This model {radioRRULibrary.Model} is already exists", (int)Helpers.Constants.ApiReturnCode.fail);
                                            
                            //    _unitOfWork.RadioRRULibraryRepository.AddWithHistory(Helpers.LogFilterAttribute.UserId, radioRRULibrary);
                            //    _unitOfWork.SaveChanges();

                            //    dynamic LogisticalItemIds = new ExpandoObject();
                            //    LogisticalItemIds = addRadioRRULibrary.LogisticalItems;

                            //    AddLogisticalItemWithRadio(LogisticalItemIds, radioRRULibrary, TableNameEntity.Id);

                            //    //if (addRadioRRULibrary.dynamicAttribute.Count > 0)
                            //    //{
                            //    //    _unitOfWork.DynamicAttLibRepository.AddDynamicLibAtt(addRadioRRULibrary.dynamicAttribute, TableNameEntity.Id, radioRRULibrary.Id);
                            //    //}
                            //    //_unitOfWork.TablesHistoryRepository.AddHistory(radioRRULibrary.Id, "Add", "TLIradioRRULibrary");
                                
                            //}
                            transaction.Complete();
                            tran.Commit();
                            return new Response<AllItemAttributes>();
                        }
                        catch (Exception err)
                        {
                            tran.Rollback();
                            return new Response<AllItemAttributes>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
                        }
                    }
                }
            }

        }
        public Response<AddRadioAntennaLibraryObject> AddRadioAntennaLibrary(string TableName, AddRadioAntennaLibraryObject RadioLibraryViewModel, string connectionString, int UserId)
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
                            TLItablesNames TableNameEntity = _unitOfWork.TablesNamesRepository.GetWhereFirst(l => l.TableName.ToLower() == TableName.ToLower());
                          
                            TLIradioAntennaLibrary radioAntennaLibrary = _mapper.Map<TLIradioAntennaLibrary>(RadioLibraryViewModel.AttributesActivatedLibrary);
                            //string CheckDependencyValidation = CheckDependencyValidationForRadioTypes(RadioLibraryViewModel, TableName);

                            //if (!string.IsNullOrEmpty(CheckDependencyValidation))
                            //    return new Response<AddRadioAntennaLibraryObject>(true, null, null, CheckDependencyValidation, (int)ApiReturnCode.fail);

                            //string CheckGeneralValidation = CheckGeneralValidationFunctionLib(RadioLibraryViewModel.DynamicAttributes, TableNameEntity.TableName);

                            //if (!string.IsNullOrEmpty(CheckGeneralValidation))
                            //    return new Response<AddRadioAntennaLibraryObject>(true, null, null, CheckGeneralValidation, (int)ApiReturnCode.fail);

                            var CheckModel = _unitOfWork.RadioAntennaLibraryRepository
                                .GetWhereFirst(x => x.Model == radioAntennaLibrary.Model && !x.Deleted);

                            if (CheckModel != null)
                                return new Response<AddRadioAntennaLibraryObject>(true, null, null, $"This model {radioAntennaLibrary.Model} is already exists", (int)Helpers.Constants.ApiReturnCode.fail);

                            _unitOfWork.RadioAntennaLibraryRepository.AddWithHistory(UserId, radioAntennaLibrary);
                            _unitOfWork.SaveChanges();

                            dynamic LogisticalItemIds = new ExpandoObject();
                            LogisticalItemIds = RadioLibraryViewModel.LogisticalItems;
                            AddLogisticalItemWithCivil(UserId,LogisticalItemIds, radioAntennaLibrary, TableNameEntity.Id);

                            if (RadioLibraryViewModel.DynamicAttributes.Count > 0)
                            {
                                _unitOfWork.DynamicAttLibRepository.AddDynamicLibAtt(UserId, RadioLibraryViewModel.DynamicAttributes, TableNameEntity.Id, radioAntennaLibrary.Id, connectionString);
                            }
                            _unitOfWork.TablesHistoryRepository.AddHistory(radioAntennaLibrary.Id, Helpers.Constants.HistoryType.Add.ToString().ToLower(), TablesNames.TLIradioAntennaLibrary.ToString().ToLower());
                            
                          
                            transaction.Complete();
                            tran.Commit();
                           
                            Task.Run(() => _unitOfWork.CivilWithLegsRepository.RefreshView(connectionString, "MV_RADIO_ANTENNA_LIBRARY_VIEW"));
                        
                            return new Response<AddRadioAntennaLibraryObject>();
                        }
                        catch (Exception err)
                        {
                            tran.Rollback();
                            return new Response<AddRadioAntennaLibraryObject>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
                        }
                    }
                }
            }

        }
        public Response<AddRadioRRULibraryObject> AddRadioRRULibrary(string TableName, AddRadioRRULibraryObject RadioLibraryViewModel, string connectionString, int UserId)
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
                            TLItablesNames TableNameEntity = _unitOfWork.TablesNamesRepository.GetWhereFirst(l => l.TableName.ToLower() == TableName.ToLower());

                            TLIradioRRULibrary radioRRULibrary = _mapper.Map<TLIradioRRULibrary>(RadioLibraryViewModel.AttributesActivatedLibrary);
                            //string CheckDependencyValidation = CheckDependencyValidationForRadioTypes(RadioLibraryViewModel, TableName);

                            //if (!string.IsNullOrEmpty(CheckDependencyValidation))
                            //    return new Response<AddRadioAntennaLibraryObject>(true, null, null, CheckDependencyValidation, (int)ApiReturnCode.fail);

                            //string CheckGeneralValidation = CheckGeneralValidationFunctionLib(RadioLibraryViewModel.DynamicAttributes, TableNameEntity.TableName);

                            //if (!string.IsNullOrEmpty(CheckGeneralValidation))
                            //    return new Response<AddRadioAntennaLibraryObject>(true, null, null, CheckGeneralValidation, (int)ApiReturnCode.fail);

                            var CheckModel = _unitOfWork.RadioAntennaLibraryRepository
                                .GetWhereFirst(x => x.Model == radioRRULibrary.Model && !x.Deleted);

                            if (CheckModel != null)
                                return new Response<AddRadioRRULibraryObject>(true, null, null, $"This model {radioRRULibrary.Model} is already exists", (int)Helpers.Constants.ApiReturnCode.fail);

                            _unitOfWork.RadioRRULibraryRepository.AddWithHistory(UserId, radioRRULibrary);
                            _unitOfWork.SaveChanges();

                            dynamic LogisticalItemIds = new ExpandoObject();
                            LogisticalItemIds = RadioLibraryViewModel.LogisticalItems;
                            AddLogisticalItemWithCivil(UserId, LogisticalItemIds, radioRRULibrary, TableNameEntity.Id);

                            if (RadioLibraryViewModel.DynamicAttributes.Count > 0)
                            {
                                _unitOfWork.DynamicAttLibRepository.AddDynamicLibAtt(UserId, RadioLibraryViewModel.DynamicAttributes, TableNameEntity.Id, radioRRULibrary.Id, connectionString);
                            }
                            _unitOfWork.TablesHistoryRepository.AddHistory(radioRRULibrary.Id, Helpers.Constants.HistoryType.Add.ToString().ToLower(), TablesNames.TLIradioRRULibrary.ToString().ToLower());


                            transaction.Complete();
                            tran.Commit();

                            Task.Run(() => _unitOfWork.CivilWithLegsRepository.RefreshView(connectionString, "MV_RADIO_RRU_LIBRARY_VIEW"));

                            return new Response<AddRadioRRULibraryObject>();
                        }
                        catch (Exception err)
                        {
                            tran.Rollback();
                            return new Response<AddRadioRRULibraryObject>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
                        }
                    }
                }
            }

        }
        public Response<GetEnableAttribute> GetRadioRRULibrariesEnabledAtt(string ConnectionString)
        {
            using (var connection = new OracleConnection(ConnectionString))
            {
                try
                {
                    GetEnableAttribute getEnableAttribute = new GetEnableAttribute();
                    connection.Open();

                    var attActivated = db.TLIattributeViewManagment
                        .Include(x => x.EditableManagmentView)
                        .Include(x => x.AttributeActivated)
                        .Include(x => x.DynamicAtt)
                        .Where(x => x.Enable && x.EditableManagmentView.View == "RadioRRULibrary"
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
                        var query = db.MV_RADIO_RRU_LIBRARY_VIEW.Where(x => !x.Deleted).AsEnumerable()
                       .Select(item => _unitOfWork.CivilWithLegsRepository.BuildDynamicSelect(item, null, propertyNamesStatic, propertyNamesDynamic));
                        int count = query.Count();

                        getEnableAttribute.Model = query;
                        return new Response<GetEnableAttribute>(true, getEnableAttribute, null, "Success", (int)Helpers.Constants.ApiReturnCode.success, count);
                    }
                    else
                    {
                        var query = db.MV_RADIO_RRU_LIBRARY_VIEW.Where(x => !x.Deleted).AsEnumerable()
                    .GroupBy(x => new
                    {
                        Id = x.Id,
                        Model = x.Model,
                        Notes = x.Notes,
                        Height = x.Height,
                        Weight = x.Weight,
                        Width = x.Width,
                        Depth = x.Depth,
                        Active = x.Active,
                        Deleted = x.Deleted,
                        Length = x.Length,
                        Type = x.Type,
                        Band = x.Band,
                        ChannelBandwidth = x.ChannelBandwidth,
                        L_W_H_cm3 = x.L_W_H_cm3,

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
        #region Helper Methods
        public string CheckDependencyValidationForRadioTypes(object Input, string RadioType)
        {
            if (RadioType.ToLower() == TablesNames.TLIradioRRULibrary.ToString().ToLower())
            {
                AddRadioRRULibraryViewModel AddRadioLibraryViewModel = _mapper.Map<AddRadioRRULibraryViewModel>(Input);

                List<DynamicAttViewModel> DynamicAttributes = _mapper.Map<List<DynamicAttViewModel>>(_unitOfWork.DynamicAttRepository
                    .GetIncludeWhere(x => x.tablesNames.TableName.ToLower() == RadioType.ToLower() && !x.disable
                        , x => x.tablesNames).ToList());

                foreach (DynamicAttViewModel DynamicAttribute in DynamicAttributes)
                {
                    TLIdependency Dependency = _unitOfWork.DependencieRepository.GetIncludeWhereFirst(x => x.DynamicAttId == DynamicAttribute.Id &&
                        x.OperationId != null && (x.ValueBoolean != null || x.ValueDateTime != null || x.ValueDouble != null || !string.IsNullOrEmpty(x.ValueString)),
                            x => x.Operation, x => x.DynamicAtt);

                    if (Dependency != null)
                    {
                        AddDynamicLibAttValueViewModel InsertedDynamicAttributeValue = AddRadioLibraryViewModel.TLIdynamicAttLibValue
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

                                    InsertedValue = AddRadioLibraryViewModel.GetType().GetProperties()
                                        .FirstOrDefault(x => x.Name.ToLower() == AttributeName.ToLower()).GetValue(AddRadioLibraryViewModel, null);

                                    if (InsertedValue == null)
                                        break;
                                }
                                else if (Rule.dynamicAttId != null)
                                {
                                    AddDynamicLibAttValueViewModel DynamicObject = AddRadioLibraryViewModel.TLIdynamicAttLibValue
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
            }
            else if (RadioType.ToLower() == TablesNames.TLIradioAntennaLibrary.ToString().ToLower())
            {
                AddRadioAntennaLibraryViewModel AddRadioLibraryViewModel = _mapper.Map<AddRadioAntennaLibraryViewModel>(Input);

                List<DynamicAttViewModel> DynamicAttributes = _mapper.Map<List<DynamicAttViewModel>>(_unitOfWork.DynamicAttRepository
                    .GetIncludeWhere(x => x.tablesNames.TableName.ToLower() == RadioType.ToLower() && !x.disable
                        , x => x.tablesNames).ToList());

                foreach (DynamicAttViewModel DynamicAttribute in DynamicAttributes)
                {
                    TLIdependency Dependency = _unitOfWork.DependencieRepository.GetIncludeWhereFirst(x => x.DynamicAttId == DynamicAttribute.Id &&
                        x.OperationId != null && (x.ValueBoolean != null || x.ValueDateTime != null || x.ValueDouble != null || !string.IsNullOrEmpty(x.ValueString)),
                            x => x.Operation, x => x.DynamicAtt);

                    if (Dependency != null)
                    {
                        AddDynamicLibAttValueViewModel InsertedDynamicAttributeValue = AddRadioLibraryViewModel.TLIdynamicAttLibValue
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

                                    InsertedValue = AddRadioLibraryViewModel.GetType().GetProperties()
                                        .FirstOrDefault(x => x.Name.ToLower() == AttributeName.ToLower()).GetValue(AddRadioLibraryViewModel, null);
                                }
                                else if (Rule.dynamicAttId != null)
                                {
                                    AddDynamicLibAttValueViewModel DynamicObject = AddRadioLibraryViewModel.TLIdynamicAttLibValue
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
            }
            else if (RadioType.ToLower() == TablesNames.TLIradioOtherLibrary.ToString().ToLower())
            {
                AddRadioOtherLibraryViewModel AddRadioLibraryViewModel = _mapper.Map<AddRadioOtherLibraryViewModel>(Input);

                List<DynamicAttViewModel> DynamicAttributes = _mapper.Map<List<DynamicAttViewModel>>(_unitOfWork.DynamicAttRepository
                    .GetIncludeWhere(x => x.tablesNames.TableName.ToLower() == RadioType.ToLower() && !x.disable
                        , x => x.tablesNames).ToList());

                foreach (DynamicAttViewModel DynamicAttribute in DynamicAttributes)
                {
                    TLIdependency Dependency = _unitOfWork.DependencieRepository.GetIncludeWhereFirst(x => x.DynamicAttId == DynamicAttribute.Id &&
                        x.OperationId != null && (x.ValueBoolean != null || x.ValueDateTime != null || x.ValueDouble != null || !string.IsNullOrEmpty(x.ValueString)),
                            x => x.Operation, x => x.DynamicAtt);

                    if (Dependency != null)
                    {
                        AddDynamicLibAttValueViewModel InsertedDynamicAttributeValue = AddRadioLibraryViewModel.TLIdynamicAttLibValue
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

                                    InsertedValue = AddRadioLibraryViewModel.GetType().GetProperties()
                                        .FirstOrDefault(x => x.Name.ToLower() == AttributeName.ToLower()).GetValue(AddRadioLibraryViewModel, null);
                                }
                                else if (Rule.dynamicAttId != null)
                                {
                                    AddDynamicLibAttValueViewModel DynamicObject = AddRadioLibraryViewModel.TLIdynamicAttLibValue
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
        public void AddLogisticalItemWithRadio(dynamic LogisticalItemIds, dynamic RadioEntity, int TableNameEntityId)
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
                                RecordId = RadioEntity.Id,
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
                                RecordId = RadioEntity.Id,
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
                                RecordId = RadioEntity.Id,
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
                                RecordId = RadioEntity.Id,
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
        public void AddLogisticalItemWithCivil(int UserId, dynamic LogisticalItemIds, dynamic CivilEntity, int TableNameEntityId)
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
                                    RecordId = CivilEntity.Id,
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
                                    RecordId = CivilEntity.Id,
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
                                    RecordId = CivilEntity.Id,
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
                                    RecordId = CivilEntity.Id,
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
                                    RecordId = CivilEntity.Id,
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
                                    RecordId = CivilEntity.Id,
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
        #endregion
        //Function take 2 parameters
        //get table name Entity by TableName
        //specify the table i deal with
        //map object to ViewModel
        //map ViewModel to Entity
        //check validation
        //update Entity
        //Update dynamic attributes values
        //public async Task<Response<AllItemAttributes>> EditRadioLibrary(string TableName, object RadioLibraryViewModel)
        //{
        //    using (TransactionScope transaction =
        //        new TransactionScope(TransactionScopeOption.Required,
        //                           new System.TimeSpan(0, 15, 0)))
        //    {
        //        try
        //        {
        //            int resultId = 0;
        //            var TableNameEntity = _unitOfWork.TablesNamesRepository.GetWhereFirst(l => l.TableName.ToLower() == TableName.ToLower());
        //            if (Helpers.Constants.LoadSubType.TLIradioAntennaLibrary.ToString().ToLower() == TableName.ToLower())
        //            {
        //                EditRadioAntennaLibraryViewModel editRadioAntennaLibrary = _mapper.Map<EditRadioAntennaLibraryViewModel>(RadioLibraryViewModel);
        //                TLIradioAntennaLibrary radioAntennaLibrary = _mapper.Map<TLIradioAntennaLibrary>(editRadioAntennaLibrary);
        //                var RadioAntenna = _unitOfWork.RadioAntennaLibraryRepository.GetAllAsQueryable().AsNoTracking().FirstOrDefault(x => x.Id == editRadioAntennaLibrary.Id);
        //                radioAntennaLibrary.Active = RadioAntenna.Active;
        //                radioAntennaLibrary.Deleted = RadioAntenna.Deleted;
        //                var CheckModel = _unitOfWork.RadioAntennaLibraryRepository.GetWhereFirst(x => x.Model.ToLower() == radioAntennaLibrary.Model.ToLower() &&
        //                    x.Id != radioAntennaLibrary.Id && !x.Deleted);
        //                if (CheckModel != null)
        //                {
        //                    return new Response<AllItemAttributes>(true, null, null, $"This model {radioAntennaLibrary.Model} is already exists", (int)Helpers.Constants.ApiReturnCode.fail);
        //                }

        //                //var testUpdate = _unitOfWork.TablesHistoryRepository.CheckUpdateObject(RadioAntenna, radioAntennaLibrary);
        //                //if (testUpdate.Details.Count != 0)
        //                //{
        //                _unitOfWork.RadioAntennaLibraryRepository.UpdateWithHistory(Helpers.LogFilterAttribute.UserId, RadioAntenna, radioAntennaLibrary);
        //                // resultId = _unitOfWork.TablesHistoryRepository.AddHistoryForEdit(radioAntennaLibrary.Id, TableNameEntity.Id, "Update", testUpdate.Details.ToList());
        //                await _unitOfWork.SaveChangesAsync();
        //                //  }

        //                string CheckDependency = CheckDependencyValidationEditApiVersion(RadioLibraryViewModel, TableName);
        //                if (!string.IsNullOrEmpty(CheckDependency))
        //                {
        //                    return new Response<AllItemAttributes>(true, null, null, CheckDependency, (int)Helpers.Constants.ApiReturnCode.fail);
        //                }

        //                string CheckValidation = CheckGeneralValidationFunctionEditApiVersion(editRadioAntennaLibrary.DynamicAtts, TableNameEntity.TableName);
        //                if (!string.IsNullOrEmpty(CheckValidation))
        //                {
        //                    return new Response<AllItemAttributes>(true, null, null, CheckValidation, (int)Helpers.Constants.ApiReturnCode.fail);
        //                }

        //                dynamic LogisticalItemIds = new ExpandoObject();
        //                LogisticalItemIds = RadioLibraryViewModel;

        //                AddLogisticalViewModel OldLogisticalItemIds = new AddLogisticalViewModel();

        //                var CheckVendorId = _unitOfWork.LogisticalitemRepository
        //                    .GetIncludeWhereFirst(x => x.logistical.logisticalType.Name.ToLower() == Helpers.Constants.LogisticalType.Vendor.ToString().ToLower() &&
        //                        x.IsLib && x.tablesNamesId == TableNameEntity.Id && x.RecordId == radioAntennaLibrary.Id, x => x.logistical,
        //                            x => x.logistical.logisticalType);

        //                if (CheckVendorId != null)
        //                    OldLogisticalItemIds.VendorId = CheckVendorId.logisticalId;

        //                else
        //                    OldLogisticalItemIds.VendorId = 0;

        //                var CheckSupplierId = _unitOfWork.LogisticalitemRepository
        //                    .GetIncludeWhereFirst(x => x.logistical.logisticalType.Name.ToLower() == Helpers.Constants.LogisticalType.Supplier.ToString().ToLower() &&
        //                        x.IsLib && x.tablesNamesId == TableNameEntity.Id && x.RecordId == radioAntennaLibrary.Id, x => x.logistical,
        //                            x => x.logistical.logisticalType);

        //                if (CheckSupplierId != null)
        //                    OldLogisticalItemIds.SupplierId = CheckSupplierId.logisticalId;

        //                else
        //                    OldLogisticalItemIds.SupplierId = 0;

        //                var CheckDesignerId = _unitOfWork.LogisticalitemRepository
        //                    .GetIncludeWhereFirst(x => x.logistical.logisticalType.Name.ToLower() == Helpers.Constants.LogisticalType.Designer.ToString().ToLower() &&
        //                        x.IsLib && x.tablesNamesId == TableNameEntity.Id && x.RecordId == radioAntennaLibrary.Id, x => x.logistical,
        //                            x => x.logistical.logisticalType);

        //                if (CheckDesignerId != null)
        //                    OldLogisticalItemIds.DesignerId = CheckDesignerId.logisticalId;

        //                else
        //                    OldLogisticalItemIds.DesignerId = 0;

        //                var CheckManufacturerId = _unitOfWork.LogisticalitemRepository
        //                    .GetIncludeWhereFirst(x => x.logistical.logisticalType.Name.ToLower() == Helpers.Constants.LogisticalType.Manufacturer.ToString().ToLower() &&
        //                        x.IsLib && x.tablesNamesId == TableNameEntity.Id && x.RecordId == radioAntennaLibrary.Id, x => x.logistical,
        //                            x => x.logistical.logisticalType);

        //                if (CheckManufacturerId != null)
        //                    OldLogisticalItemIds.ManufacturerId = CheckManufacturerId.logisticalId;

        //                else
        //                    OldLogisticalItemIds.ManufacturerId = 0;

        //                EditLogisticalItem(LogisticalItemIds, radioAntennaLibrary, TableNameEntity.Id, OldLogisticalItemIds);

        //                if (editRadioAntennaLibrary.DynamicAtts != null ? editRadioAntennaLibrary.DynamicAtts.Count > 0 : false)
        //                {
        //                    _unitOfWork.DynamicAttLibRepository.UpdateDynamicLibAttsWithHistory(editRadioAntennaLibrary.DynamicAtts, TableNameEntity.Id, radioAntennaLibrary.Id, Helpers.LogFilterAttribute.UserId, resultId, RadioAntenna.Id);
        //                }
        //            }
        //            else if (Helpers.Constants.LoadSubType.TLIradioOtherLibrary.ToString().ToLower() == TableName.ToLower())
        //            {
        //                EditRadioOtherLibraryViewModel editRadioOther = _mapper.Map<EditRadioOtherLibraryViewModel>(RadioLibraryViewModel);
        //                TLIradioOtherLibrary radioOther = _mapper.Map<TLIradioOtherLibrary>(editRadioOther);
        //                var Other = _unitOfWork.RadioOtherLibraryRepository.GetAllAsQueryable().AsNoTracking().FirstOrDefault(x => x.Id == editRadioOther.Id); ;

        //                radioOther.Active = Other.Active;
        //                radioOther.Deleted = Other.Deleted;
        //                var CheckModel = _unitOfWork.RadioOtherLibraryRepository.GetWhereFirst(x => x.Model.ToLower() == radioOther.Model.ToLower() &&
        //                    x.Id != radioOther.Id && !x.Deleted);
        //                if (CheckModel != null)
        //                {
        //                    return new Response<AllItemAttributes>(true, null, null, $"This model {radioOther.Model} is already exists", (int)Helpers.Constants.ApiReturnCode.fail);
        //                }

        //                _unitOfWork.RadioOtherLibraryRepository.UpdateWithHistory(Helpers.LogFilterAttribute.UserId, Other, radioOther);
        //                //if (testUpdate.Details.Count != 0)
        //                //{
        //                //    _unitOfWork.RadioOtherLibraryRepository.Update((TLIradioOtherLibrary)testUpdate.original);
        //                //   // resultId = _unitOfWork.TablesHistoryRepository.AddHistoryForEdit(radioOther.Id, TableNameEntity.Id, "Update", testUpdate.Details.ToList());
        //                //    await _unitOfWork.SaveChangesAsync();
        //                //}

        //                string CheckDependency = CheckDependencyValidationEditApiVersion(RadioLibraryViewModel, TableName);
        //                if (!string.IsNullOrEmpty(CheckDependency))
        //                {
        //                    return new Response<AllItemAttributes>(true, null, null, CheckDependency, (int)Helpers.Constants.ApiReturnCode.fail);
        //                }

        //                string CheckValidation = CheckGeneralValidationFunctionEditApiVersion(editRadioOther.DynamicAtts, TableNameEntity.TableName);
        //                if (!string.IsNullOrEmpty(CheckValidation))
        //                {
        //                    return new Response<AllItemAttributes>(true, null, null, CheckValidation, (int)Helpers.Constants.ApiReturnCode.fail);
        //                }

        //                dynamic LogisticalItemIds = new ExpandoObject();
        //                LogisticalItemIds = RadioLibraryViewModel;

        //                AddLogisticalViewModel OldLogisticalItemIds = new AddLogisticalViewModel();

        //                var CheckVendorId = _unitOfWork.LogisticalitemRepository
        //                    .GetIncludeWhereFirst(x => x.logistical.logisticalType.Name.ToLower() == Helpers.Constants.LogisticalType.Vendor.ToString().ToLower() &&
        //                        x.IsLib && x.tablesNamesId == TableNameEntity.Id && x.RecordId == radioOther.Id, x => x.logistical,
        //                            x => x.logistical.logisticalType);

        //                if (CheckVendorId != null)
        //                    OldLogisticalItemIds.VendorId = CheckVendorId.logisticalId;

        //                else
        //                    OldLogisticalItemIds.VendorId = 0;

        //                var CheckSupplierId = _unitOfWork.LogisticalitemRepository
        //                    .GetIncludeWhereFirst(x => x.logistical.logisticalType.Name.ToLower() == Helpers.Constants.LogisticalType.Supplier.ToString().ToLower() &&
        //                        x.IsLib && x.tablesNamesId == TableNameEntity.Id && x.RecordId == radioOther.Id, x => x.logistical,
        //                            x => x.logistical.logisticalType);

        //                if (CheckSupplierId != null)
        //                    OldLogisticalItemIds.SupplierId = CheckSupplierId.logisticalId;

        //                else
        //                    OldLogisticalItemIds.SupplierId = 0;

        //                var CheckDesignerId = _unitOfWork.LogisticalitemRepository
        //                    .GetIncludeWhereFirst(x => x.logistical.logisticalType.Name.ToLower() == Helpers.Constants.LogisticalType.Designer.ToString().ToLower() &&
        //                        x.IsLib && x.tablesNamesId == TableNameEntity.Id && x.RecordId == radioOther.Id, x => x.logistical,
        //                            x => x.logistical.logisticalType);

        //                if (CheckDesignerId != null)
        //                    OldLogisticalItemIds.DesignerId = CheckDesignerId.logisticalId;

        //                else
        //                    OldLogisticalItemIds.DesignerId = 0;

        //                var CheckManufacturerId = _unitOfWork.LogisticalitemRepository
        //                    .GetIncludeWhereFirst(x => x.logistical.logisticalType.Name.ToLower() == Helpers.Constants.LogisticalType.Manufacturer.ToString().ToLower() &&
        //                        x.IsLib && x.tablesNamesId == TableNameEntity.Id && x.RecordId == radioOther.Id, x => x.logistical,
        //                            x => x.logistical.logisticalType);

        //                if (CheckManufacturerId != null)
        //                    OldLogisticalItemIds.ManufacturerId = CheckManufacturerId.logisticalId;

        //                else
        //                    OldLogisticalItemIds.ManufacturerId = 0;

        //                EditLogisticalItem(LogisticalItemIds, radioOther, TableNameEntity.Id, OldLogisticalItemIds);

        //                if (editRadioOther.DynamicAtts != null ? editRadioOther.DynamicAtts.Count > 0 : false)
        //                {
        //                    _unitOfWork.DynamicAttLibRepository.UpdateDynamicLibAttsWithHistory(editRadioOther.DynamicAtts, TableNameEntity.Id, radioOther.Id, Helpers.LogFilterAttribute.UserId, resultId, Other.Id);
        //                }
        //                //_unitOfWork.RadioOtherLibraryRepository.Update(radioOther);
        //                //_unitOfWork.DynamicAttLibRepository.UpdateDynamicLibAttsWithHistory(editRadioOther.DynamicAtts, TableNameEntity.Id, radioOther.Id);
        //                //await _unitOfWork.SaveChangesAsync();
        //            }
        //            else if (Helpers.Constants.LoadSubType.TLIradioRRULibrary.ToString().ToLower() == TableName.ToLower())
        //            {
        //                EditRadioRRULibraryViewModel editRadioRRULibrary = _mapper.Map<EditRadioRRULibraryViewModel>(RadioLibraryViewModel);
        //                TLIradioRRULibrary radioRRULibrary = _mapper.Map<TLIradioRRULibrary>(editRadioRRULibrary);
        //                if (radioRRULibrary.L_W_H_cm3 == null || radioRRULibrary.L_W_H_cm3 == "")
        //                {
        //                    radioRRULibrary.L_W_H_cm3 = radioRRULibrary.Length + "_" + radioRRULibrary.Width + "_" + radioRRULibrary.Height;
        //                }
        //                var RadioRRU = _unitOfWork.RadioRRULibraryRepository.GetAllAsQueryable().AsNoTracking().FirstOrDefault(x => x.Id == editRadioRRULibrary.Id);

        //                radioRRULibrary.Active = RadioRRU.Active;
        //                radioRRULibrary.Deleted = RadioRRU.Deleted;
        //                var CheckModel = _unitOfWork.RadioRRULibraryRepository.GetWhereFirst(x => x.Model.ToLower() == radioRRULibrary.Model.ToLower() &&
        //                    x.Id != radioRRULibrary.Id && !x.Deleted);
        //                if (CheckModel != null)
        //                {
        //                    return new Response<AllItemAttributes>(true, null, null, $"This model {radioRRULibrary.Model} is already exists", (int)Helpers.Constants.ApiReturnCode.fail);
        //                }
        //                _unitOfWork.RadioRRULibraryRepository.UpdateWithHistory(Helpers.LogFilterAttribute.UserId, RadioRRU, radioRRULibrary);
        //                //if (testUpdate.Details.Count != 0)
        //                //{
        //                //    _unitOfWork.RadioRRULibraryRepository.Update((TLIradioRRULibrary)testUpdate.original);
        //                //   // resultId = _unitOfWork.TablesHistoryRepository.AddHistoryForEdit(radioRRULibrary.Id, TableNameEntity.Id, "Update", testUpdate.Details.ToList());
        //                //    await _unitOfWork.SaveChangesAsync();
        //                //}

        //                string CheckDependency = CheckDependencyValidationEditApiVersion(RadioLibraryViewModel, TableName);
        //                if (!string.IsNullOrEmpty(CheckDependency))
        //                {
        //                    return new Response<AllItemAttributes>(true, null, null, CheckDependency, (int)Helpers.Constants.ApiReturnCode.fail);
        //                }

        //                string CheckValidation = CheckGeneralValidationFunctionEditApiVersion(editRadioRRULibrary.DynamicAtts, TableNameEntity.TableName);
        //                if (!string.IsNullOrEmpty(CheckValidation))
        //                {
        //                    return new Response<AllItemAttributes>(true, null, null, CheckValidation, (int)Helpers.Constants.ApiReturnCode.fail);
        //                }

        //                dynamic LogisticalItemIds = new ExpandoObject();
        //                LogisticalItemIds = RadioLibraryViewModel;

        //                AddLogisticalViewModel OldLogisticalItemIds = new AddLogisticalViewModel();

        //                var CheckVendorId = _unitOfWork.LogisticalitemRepository
        //                    .GetIncludeWhereFirst(x => x.logistical.logisticalType.Name.ToLower() == Helpers.Constants.LogisticalType.Vendor.ToString().ToLower() &&
        //                        x.IsLib && x.tablesNamesId == TableNameEntity.Id && x.RecordId == radioRRULibrary.Id, x => x.logistical,
        //                            x => x.logistical.logisticalType);

        //                if (CheckVendorId != null)
        //                    OldLogisticalItemIds.VendorId = CheckVendorId.logisticalId;

        //                else
        //                    OldLogisticalItemIds.VendorId = 0;

        //                var CheckSupplierId = _unitOfWork.LogisticalitemRepository
        //                    .GetIncludeWhereFirst(x => x.logistical.logisticalType.Name.ToLower() == Helpers.Constants.LogisticalType.Supplier.ToString().ToLower() &&
        //                        x.IsLib && x.tablesNamesId == TableNameEntity.Id && x.RecordId == radioRRULibrary.Id, x => x.logistical,
        //                            x => x.logistical.logisticalType);

        //                if (CheckSupplierId != null)
        //                    OldLogisticalItemIds.SupplierId = CheckSupplierId.logisticalId;

        //                else
        //                    OldLogisticalItemIds.SupplierId = 0;

        //                var CheckDesignerId = _unitOfWork.LogisticalitemRepository
        //                    .GetIncludeWhereFirst(x => x.logistical.logisticalType.Name.ToLower() == Helpers.Constants.LogisticalType.Designer.ToString().ToLower() &&
        //                        x.IsLib && x.tablesNamesId == TableNameEntity.Id && x.RecordId == radioRRULibrary.Id, x => x.logistical,
        //                            x => x.logistical.logisticalType);

        //                if (CheckDesignerId != null)
        //                    OldLogisticalItemIds.DesignerId = CheckDesignerId.logisticalId;

        //                else
        //                    OldLogisticalItemIds.DesignerId = 0;

        //                var CheckManufacturerId = _unitOfWork.LogisticalitemRepository
        //                    .GetIncludeWhereFirst(x => x.logistical.logisticalType.Name.ToLower() == Helpers.Constants.LogisticalType.Manufacturer.ToString().ToLower() &&
        //                        x.IsLib && x.tablesNamesId == TableNameEntity.Id && x.RecordId == radioRRULibrary.Id, x => x.logistical,
        //                            x => x.logistical.logisticalType);

        //                if (CheckManufacturerId != null)
        //                    OldLogisticalItemIds.ManufacturerId = CheckManufacturerId.logisticalId;

        //                else
        //                    OldLogisticalItemIds.ManufacturerId = 0;

        //                EditLogisticalItem(LogisticalItemIds, radioRRULibrary, TableNameEntity.Id, OldLogisticalItemIds);

        //                if (editRadioRRULibrary.DynamicAtts != null ? editRadioRRULibrary.DynamicAtts.Count > 0 : false)
        //                {
        //                    _unitOfWork.DynamicAttLibRepository.UpdateDynamicLibAttsWithHistory(editRadioRRULibrary.DynamicAtts, TableNameEntity.Id, radioRRULibrary.Id, Helpers.LogFilterAttribute.UserId, resultId, RadioRRU.Id);
        //                }
        //            }
        //            transaction.Complete();
        //            return new Response<AllItemAttributes>();
        //        }
        //        catch (Exception err)
        //        {
        //            return new Response<AllItemAttributes>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
        //        }
        //    }
        //}
        public async Task<Response<EditRadioAntennaLibraryObject>> EditRadioAntennaLibrary(string TableName, EditRadioAntennaLibraryObject RadioLibraryViewModel,int UserId,string connectionString)
        {
            using (TransactionScope transaction =
                new TransactionScope(TransactionScopeOption.Required,
                                   new System.TimeSpan(0, 15, 0)))
            {
                try
                {
                    int resultId = 0;
                    var TableNameEntity = _unitOfWork.TablesNamesRepository.GetWhereFirst(l => l.TableName.ToLower() == TableName.ToLower());

                        TLIradioAntennaLibrary radioAntennaLibrary = _mapper.Map<TLIradioAntennaLibrary>(RadioLibraryViewModel.AttributesActivatedLibrary);
                        var OldRadioAntenna = _unitOfWork.RadioAntennaLibraryRepository.GetAllAsQueryable().AsNoTracking().FirstOrDefault(x => x.Id == radioAntennaLibrary.Id);
                        var CheckModel = _unitOfWork.RadioAntennaLibraryRepository.GetWhereFirst(x => x.Model.ToLower() == radioAntennaLibrary.Model.ToLower() &&
                            x.Id != radioAntennaLibrary.Id && !x.Deleted);
                        if (CheckModel != null)
                         return new Response<EditRadioAntennaLibraryObject>(true, null, null, $"This model {radioAntennaLibrary.Model} is already exists", (int)Helpers.Constants.ApiReturnCode.fail);
                        
                        _unitOfWork.RadioAntennaLibraryRepository.UpdateWithHistory(UserId, OldRadioAntenna, radioAntennaLibrary);
                        await _unitOfWork.SaveChangesAsync();
                        //string CheckDependency = CheckDependencyValidationEditApiVersion(RadioLibraryViewModel, TableName);
                        //if (!string.IsNullOrEmpty(CheckDependency))
                        //{
                        //    return new Response<AllItemAttributes>(true, null, null, CheckDependency, (int)Helpers.Constants.ApiReturnCode.fail);
                        //}

                        //string CheckValidation = CheckGeneralValidationFunctionEditApiVersion(RadioLibraryViewModel.DynamicAttributes, TableNameEntity.TableName);
                        //if (!string.IsNullOrEmpty(CheckValidation))
                        //{
                        //    return new Response<AllItemAttributes>(true, null, null, CheckValidation, (int)Helpers.Constants.ApiReturnCode.fail);
                        //}

                        dynamic LogisticalItemIds = new ExpandoObject();
                        LogisticalItemIds = RadioLibraryViewModel;

                        AddLogisticalViewModel OldLogisticalItemIds = new AddLogisticalViewModel();

                        var CheckVendorId = _unitOfWork.LogisticalitemRepository
                            .GetIncludeWhereFirst(x => x.logistical.logisticalType.Name.ToLower() == Helpers.Constants.LogisticalType.Vendor.ToString().ToLower() &&
                                x.IsLib && x.tablesNamesId == TableNameEntity.Id && x.RecordId == radioAntennaLibrary.Id, x => x.logistical,
                                    x => x.logistical.logisticalType);

                    if (CheckVendorId != null)
                        OldLogisticalItemIds.Vendor = Convert.ToInt32(CheckVendorId.logisticalId);

                    var CheckSupplierId = _unitOfWork.LogisticalitemRepository
                        .GetIncludeWhereFirst(x => x.logistical.logisticalType.Name.ToLower() == Helpers.Constants.LogisticalType.Supplier.ToString().ToLower() &&
                            x.IsLib && x.tablesNamesId == TableNameEntity.Id && x.RecordId == radioAntennaLibrary.Id, x => x.logistical,
                                x => x.logistical.logisticalType);

                    if (CheckSupplierId != null)
                        OldLogisticalItemIds.Supplier = CheckSupplierId.logisticalId;

                    var CheckDesignerId = _unitOfWork.LogisticalitemRepository
                        .GetIncludeWhereFirst(x => x.logistical.logisticalType.Name.ToLower() == Helpers.Constants.LogisticalType.Designer.ToString().ToLower() &&
                            x.IsLib && x.tablesNamesId == TableNameEntity.Id && x.RecordId == radioAntennaLibrary.Id, x => x.logistical,
                                x => x.logistical.logisticalType);

                    if (CheckDesignerId != null)
                        OldLogisticalItemIds.Designer = CheckDesignerId.logisticalId;


                    var CheckManufacturerId = _unitOfWork.LogisticalitemRepository
                        .GetIncludeWhereFirst(x => x.logistical.logisticalType.Name.ToLower() == Helpers.Constants.LogisticalType.Manufacturer.ToString().ToLower() &&
                            x.IsLib && x.tablesNamesId == TableNameEntity.Id && x.RecordId == radioAntennaLibrary.Id, x => x.logistical,
                                x => x.logistical.logisticalType);

                    if (CheckManufacturerId != null)
                        OldLogisticalItemIds.Manufacturer = CheckManufacturerId.logisticalId;


                    var CheckContractorId = _unitOfWork.LogisticalitemRepository
                 .GetIncludeWhereFirst(x => x.logistical.logisticalType.Name.ToLower() == Helpers.Constants.LogisticalType.Contractor.ToString().ToLower() &&
                     x.IsLib && x.tablesNamesId == TableNameEntity.Id && x.RecordId == radioAntennaLibrary.Id, x => x.logistical,
                         x => x.logistical.logisticalType);

                    if (CheckContractorId != null)
                        OldLogisticalItemIds.Contractor = CheckContractorId.logisticalId;


                    var CheckConsultantId = _unitOfWork.LogisticalitemRepository
                       .GetIncludeWhereFirst(x => x.logistical.logisticalType.Name.ToLower() == Helpers.Constants.LogisticalType.Consultant.ToString().ToLower() &&
                           x.IsLib && x.tablesNamesId == TableNameEntity.Id && x.RecordId == radioAntennaLibrary.Id, x => x.logistical,
                               x => x.logistical.logisticalType);

                    if (CheckConsultantId != null)
                        OldLogisticalItemIds.Consultant = CheckConsultantId.logisticalId;


                    EditLogisticalItemss(UserId, RadioLibraryViewModel.LogisticalItems, radioAntennaLibrary, TableNameEntity.Id, OldLogisticalItemIds);

                    if (RadioLibraryViewModel.DynamicAttributes != null ? RadioLibraryViewModel.DynamicAttributes.Count > 0 : false)
                    {
                        _unitOfWork.DynamicAttLibRepository.UpdateDynamicLibAttsWithHistorys(RadioLibraryViewModel.DynamicAttributes, connectionString ,TableNameEntity.Id, radioAntennaLibrary.Id, UserId, resultId, radioAntennaLibrary.Id);
                    }

                    await _unitOfWork.SaveChangesAsync();

                    transaction.Complete();
                    Task.Run(() => _unitOfWork.CivilWithLegsRepository.RefreshView(connectionString, "MV_RADIO_ANTENNA_LIBRARY_VIEW"));
                    return new Response<EditRadioAntennaLibraryObject>();
                }
                catch (Exception err)
                {
                    return new Response<EditRadioAntennaLibraryObject>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
                }
            }
        }
        public async Task<Response<EditRadioRRULibraryObject>> EditRadioRRULibrary(string TableName, EditRadioRRULibraryObject RadioLibraryViewModel, int UserId, string connectionString)
        {
            using (TransactionScope transaction =
                new TransactionScope(TransactionScopeOption.Required,
                                   new System.TimeSpan(0, 15, 0)))
            {
                try
                {
                    int resultId = 0;
                    var TableNameEntity = _unitOfWork.TablesNamesRepository.GetWhereFirst(l => l.TableName.ToLower() == TableName.ToLower());

                    TLIradioRRULibrary radioRRULibrary = _mapper.Map<TLIradioRRULibrary>(RadioLibraryViewModel.AttributesActivatedLibrary);
                    var OldRadioRRU= _unitOfWork.RadioRRULibraryRepository.GetAllAsQueryable().AsNoTracking().FirstOrDefault(x => x.Id == radioRRULibrary.Id);

                    var CheckModel = _unitOfWork.RadioRRULibraryRepository.GetWhereFirst(x => x.Model.ToLower() == radioRRULibrary.Model.ToLower() &&
                        x.Id != radioRRULibrary.Id && !x.Deleted);
                    if (CheckModel != null)
                        return new Response<EditRadioRRULibraryObject>(true, null, null, $"This model {radioRRULibrary.Model} is already exists", (int)Helpers.Constants.ApiReturnCode.fail);

                    _unitOfWork.RadioRRULibraryRepository.UpdateWithHistory(UserId, OldRadioRRU, radioRRULibrary);
                    await _unitOfWork.SaveChangesAsync();
                    //string CheckDependency = CheckDependencyValidationEditApiVersion(RadioLibraryViewModel, TableName);
                    //if (!string.IsNullOrEmpty(CheckDependency))
                    //{
                    //    return new Response<AllItemAttributes>(true, null, null, CheckDependency, (int)Helpers.Constants.ApiReturnCode.fail);
                    //}

                    //string CheckValidation = CheckGeneralValidationFunctionEditApiVersion(RadioLibraryViewModel.DynamicAttributes, TableNameEntity.TableName);
                    //if (!string.IsNullOrEmpty(CheckValidation))
                    //{
                    //    return new Response<AllItemAttributes>(true, null, null, CheckValidation, (int)Helpers.Constants.ApiReturnCode.fail);
                    //}

                    dynamic LogisticalItemIds = new ExpandoObject();
                    LogisticalItemIds = RadioLibraryViewModel;

                    AddLogisticalViewModel OldLogisticalItemIds = new AddLogisticalViewModel();

                    var CheckVendorId = _unitOfWork.LogisticalitemRepository
                        .GetIncludeWhereFirst(x => x.logistical.logisticalType.Name.ToLower() == Helpers.Constants.LogisticalType.Vendor.ToString().ToLower() &&
                            x.IsLib && x.tablesNamesId == TableNameEntity.Id && x.RecordId == radioRRULibrary.Id, x => x.logistical,
                                x => x.logistical.logisticalType);

                    if (CheckVendorId != null)
                        OldLogisticalItemIds.Vendor = Convert.ToInt32(CheckVendorId.logisticalId);

                    var CheckSupplierId = _unitOfWork.LogisticalitemRepository
                        .GetIncludeWhereFirst(x => x.logistical.logisticalType.Name.ToLower() == Helpers.Constants.LogisticalType.Supplier.ToString().ToLower() &&
                            x.IsLib && x.tablesNamesId == TableNameEntity.Id && x.RecordId == radioRRULibrary.Id, x => x.logistical,
                                x => x.logistical.logisticalType);

                    if (CheckSupplierId != null)
                        OldLogisticalItemIds.Supplier = CheckSupplierId.logisticalId;

                    var CheckDesignerId = _unitOfWork.LogisticalitemRepository
                        .GetIncludeWhereFirst(x => x.logistical.logisticalType.Name.ToLower() == Helpers.Constants.LogisticalType.Designer.ToString().ToLower() &&
                            x.IsLib && x.tablesNamesId == TableNameEntity.Id && x.RecordId == radioRRULibrary.Id, x => x.logistical,
                                x => x.logistical.logisticalType);

                    if (CheckDesignerId != null)
                        OldLogisticalItemIds.Designer = CheckDesignerId.logisticalId;


                    var CheckManufacturerId = _unitOfWork.LogisticalitemRepository
                        .GetIncludeWhereFirst(x => x.logistical.logisticalType.Name.ToLower() == Helpers.Constants.LogisticalType.Manufacturer.ToString().ToLower() &&
                            x.IsLib && x.tablesNamesId == TableNameEntity.Id && x.RecordId == radioRRULibrary.Id, x => x.logistical,
                                x => x.logistical.logisticalType);

                    if (CheckManufacturerId != null)
                        OldLogisticalItemIds.Manufacturer = CheckManufacturerId.logisticalId;


                    var CheckContractorId = _unitOfWork.LogisticalitemRepository
                 .GetIncludeWhereFirst(x => x.logistical.logisticalType.Name.ToLower() == Helpers.Constants.LogisticalType.Contractor.ToString().ToLower() &&
                     x.IsLib && x.tablesNamesId == TableNameEntity.Id && x.RecordId == radioRRULibrary.Id, x => x.logistical,
                         x => x.logistical.logisticalType);

                    if (CheckContractorId != null)
                        OldLogisticalItemIds.Contractor = CheckContractorId.logisticalId;


                    var CheckConsultantId = _unitOfWork.LogisticalitemRepository
                       .GetIncludeWhereFirst(x => x.logistical.logisticalType.Name.ToLower() == Helpers.Constants.LogisticalType.Consultant.ToString().ToLower() &&
                           x.IsLib && x.tablesNamesId == TableNameEntity.Id && x.RecordId == radioRRULibrary.Id, x => x.logistical,
                               x => x.logistical.logisticalType);

                    if (CheckConsultantId != null)
                        OldLogisticalItemIds.Consultant = CheckConsultantId.logisticalId;


                    EditLogisticalItemss(UserId, RadioLibraryViewModel.LogisticalItems, radioRRULibrary, TableNameEntity.Id, OldLogisticalItemIds);

                    if (RadioLibraryViewModel.DynamicAttributes != null ? RadioLibraryViewModel.DynamicAttributes.Count > 0 : false)
                    {
                        _unitOfWork.DynamicAttLibRepository.UpdateDynamicLibAttsWithHistorys(RadioLibraryViewModel.DynamicAttributes, connectionString, TableNameEntity.Id, radioRRULibrary.Id, UserId, resultId, radioRRULibrary.Id);
                    }

                    await _unitOfWork.SaveChangesAsync();

                    transaction.Complete();
                    Task.Run(() => _unitOfWork.CivilWithLegsRepository.RefreshView(connectionString, "MV_RADIO_RRU_LIBRARY_VIEW"));
                    return new Response<EditRadioRRULibraryObject>();
                }
                catch (Exception err)
                {
                    return new Response<EditRadioRRULibraryObject>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
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
        public string CheckDependencyValidationEditApiVersion(object Input, string RadioType)
        {
            if (RadioType.ToLower() == Helpers.Constants.TablesNames.TLIradioAntennaLibrary.ToString().ToLower())
            {
                EditRadioAntennaLibraryViewModel EditRadioAntennaLibraryViewModel = _mapper.Map<EditRadioAntennaLibraryViewModel>(Input);

                List<DynamicAttViewModel> DynamicAttributes = _mapper.Map<List<DynamicAttViewModel>>(_unitOfWork.DynamicAttRepository
                    .GetIncludeWhere(x => x.tablesNames.TableName.ToLower() == RadioType.ToLower() && !x.disable
                        , x => x.tablesNames).ToList());

                foreach (DynamicAttViewModel DynamicAttribute in DynamicAttributes)
                {
                    TLIdependency Dependency = _unitOfWork.DependencieRepository.GetIncludeWhereFirst(x => x.DynamicAttId == DynamicAttribute.Id &&
                        x.OperationId != null && (x.ValueBoolean != null || x.ValueDateTime != null || x.ValueDouble != null || !string.IsNullOrEmpty(x.ValueString)),
                            x => x.Operation, x => x.DynamicAtt);

                    if (Dependency != null)
                    {
                        DynamicAttLibViewModel InsertedDynamicAttributeValue = EditRadioAntennaLibraryViewModel.DynamicAtts
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

                                    InsertedValue = EditRadioAntennaLibraryViewModel.GetType().GetProperties()
                                        .FirstOrDefault(x => x.Name.ToLower() == AttributeName.ToLower()).GetValue(EditRadioAntennaLibraryViewModel, null);
                                }
                                else if (Rule.dynamicAttId != null)
                                {
                                    DynamicAttLibViewModel DynamicObject = EditRadioAntennaLibraryViewModel.DynamicAtts
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
            }
            else if (RadioType.ToLower() == Helpers.Constants.TablesNames.TLIradioRRULibrary.ToString().ToLower())
            {
                EditRadioRRULibraryViewModel EditRadioRRULibraryViewModel = _mapper.Map<EditRadioRRULibraryViewModel>(Input);

                List<DynamicAttViewModel> DynamicAttributes = _mapper.Map<List<DynamicAttViewModel>>(_unitOfWork.DynamicAttRepository
                    .GetIncludeWhere(x => x.tablesNames.TableName.ToLower() == RadioType.ToLower() && !x.disable
                        , x => x.tablesNames).ToList());

                foreach (DynamicAttViewModel DynamicAttribute in DynamicAttributes)
                {
                    TLIdependency Dependency = _unitOfWork.DependencieRepository.GetIncludeWhereFirst(x => x.DynamicAttId == DynamicAttribute.Id &&
                        x.OperationId != null && (x.ValueBoolean != null || x.ValueDateTime != null || x.ValueDouble != null || !string.IsNullOrEmpty(x.ValueString)),
                            x => x.Operation, x => x.DynamicAtt);

                    if (Dependency != null)
                    {
                        DynamicAttLibViewModel InsertedDynamicAttributeValue = EditRadioRRULibraryViewModel.DynamicAtts
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

                                    InsertedValue = EditRadioRRULibraryViewModel.GetType().GetProperties()
                                        .FirstOrDefault(x => x.Name.ToLower() == AttributeName.ToLower()).GetValue(EditRadioRRULibraryViewModel, null);
                                }
                                else if (Rule.dynamicAttId != null)
                                {
                                    DynamicAttLibViewModel DynamicObject = EditRadioRRULibraryViewModel.DynamicAtts
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
            }
            else if (RadioType.ToLower() == Helpers.Constants.TablesNames.TLIradioOtherLibrary.ToString().ToLower())
            {
                EditRadioOtherLibraryViewModel EditRadioOtherLibraryViewModel = _mapper.Map<EditRadioOtherLibraryViewModel>(Input);

                List<DynamicAttViewModel> DynamicAttributes = _mapper.Map<List<DynamicAttViewModel>>(_unitOfWork.DynamicAttRepository
                    .GetIncludeWhere(x => x.tablesNames.TableName.ToLower() == RadioType.ToLower() && !x.disable
                        , x => x.tablesNames).ToList());

                foreach (DynamicAttViewModel DynamicAttribute in DynamicAttributes)
                {
                    TLIdependency Dependency = _unitOfWork.DependencieRepository.GetIncludeWhereFirst(x => x.DynamicAttId == DynamicAttribute.Id &&
                        x.OperationId != null && (x.ValueBoolean != null || x.ValueDateTime != null || x.ValueDouble != null || !string.IsNullOrEmpty(x.ValueString)),
                            x => x.Operation, x => x.DynamicAtt);

                    if (Dependency != null)
                    {
                        DynamicAttLibViewModel InsertedDynamicAttributeValue = EditRadioOtherLibraryViewModel.DynamicAtts
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

                                    InsertedValue = EditRadioOtherLibraryViewModel.GetType().GetProperties()
                                        .FirstOrDefault(x => x.Name.ToLower() == AttributeName.ToLower()).GetValue(EditRadioOtherLibraryViewModel, null);
                                }
                                else if (Rule.dynamicAttId != null)
                                {
                                    DynamicAttLibViewModel DynamicObject = EditRadioOtherLibraryViewModel.DynamicAtts
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
        //Function take 2 parameters
        //get table name Entity by TableName
        //specify the table i deal with
        //get record by Id
        //enable or disable the record depened on record status
        //update Entity
        public async Task<Response<AllItemAttributes>> DisableRadioLibrary(string TableName, int Id,int UserId,string connectionString)
        {
            using (TransactionScope transaction = new TransactionScope())
            {
                try
                {
                    var TableNameEntity = _unitOfWork.TablesNamesRepository.GetWhereFirst(l => l.TableName == TableName);
                    if (Helpers.Constants.LoadSubType.TLIradioAntennaLibrary.ToString() == TableName)
                    {
                        var CivilLoad = _unitOfWork.CivilLoadsRepository.GetIncludeWhere(x => x.allLoadInstId != null && !x.Dismantle &&
                       x.allLoadInst.radioAntenna.radioAntennaLibraryId == Id, x => x.allLoadInst, x => x.allLoadInst.radioAntenna).ToList();
                        var OldRadioAntennaLibrary = _unitOfWork.RadioAntennaLibraryRepository.GetAllAsQueryable().AsNoTracking().FirstOrDefault(x => x.Id == Id);
                        if (CivilLoad != null && CivilLoad.Count > 0)
                            return new Response<AllItemAttributes>(false, null, null, "Can not delete this item because is used", (int)Helpers.Constants.ApiReturnCode.fail);
                        var NewRadioAntennaEntity = _unitOfWork.RadioAntennaLibraryRepository.GetByID(Id);
                        NewRadioAntennaEntity.Active = !(NewRadioAntennaEntity.Active);
                        _unitOfWork.RadioAntennaLibraryRepository.UpdateWithHistory(UserId, OldRadioAntennaLibrary, NewRadioAntennaEntity);
                        await _unitOfWork.SaveChangesAsync();
                    }
                    else if (Helpers.Constants.LoadSubType.TLIradioOtherLibrary.ToString() == TableName)
                    {
                        var CivilLoad = _unitOfWork.CivilLoadsRepository.GetIncludeWhere(x => x.allLoadInstId != null && !x.Dismantle &&
                        x.allLoadInst.radioOther.radioOtherLibraryId == Id, x => x.allLoadInst, x => x.allLoadInst.radioOther).ToList();
                        var OldRadioOtherLibrary = _unitOfWork.RadioOtherLibraryRepository.GetAllAsQueryable().AsNoTracking().FirstOrDefault(x => x.Id == Id);
                        if (CivilLoad != null && CivilLoad.Count > 0)
                            return new Response<AllItemAttributes>(false, null, null, "Can not delete this item because is used", (int)Helpers.Constants.ApiReturnCode.fail);

                        TLIradioOtherLibrary NewRadioOtherLibrary = _unitOfWork.RadioOtherLibraryRepository.GetByID(Id);
                      
                        NewRadioOtherLibrary.Active = !(NewRadioOtherLibrary.Active);
                        _unitOfWork.RadioOtherLibraryRepository.UpdateWithHistory(UserId, OldRadioOtherLibrary, NewRadioOtherLibrary);
                        await _unitOfWork.SaveChangesAsync();
                    }
                    else if (Helpers.Constants.LoadSubType.TLIradioRRULibrary.ToString() == TableName)
                    {
                        var CivilLoad = _unitOfWork.CivilLoadsRepository.GetIncludeWhere(x => x.allLoadInstId != null && !x.Dismantle &&
                         x.allLoadInst.radioRRU.radioRRULibraryId == Id, x => x.allLoadInst, x => x.allLoadInst.radioRRU).ToList();
                        var OldRadioRRULibrary = _unitOfWork.RadioRRULibraryRepository.GetAllAsQueryable().AsNoTracking().FirstOrDefault(x => x.Id == Id);
                        if (CivilLoad != null && CivilLoad.Count > 0)
                            return new Response<AllItemAttributes>(false, null, null, "Can not delete this item because is used", (int)Helpers.Constants.ApiReturnCode.fail);
                        TLIradioRRULibrary NewRadioRRULibrary = _unitOfWork.RadioRRULibraryRepository.GetByID(Id);
                 
                        NewRadioRRULibrary.Active = !(NewRadioRRULibrary.Active);
                        _unitOfWork.RadioRRULibraryRepository.UpdateWithHistory(UserId, OldRadioRRULibrary, NewRadioRRULibrary);
                        await _unitOfWork.SaveChangesAsync();
                    }
                    transaction.Complete();
                    if (Helpers.Constants.LoadSubType.TLIradioAntennaLibrary.ToString() == TableName)
                    {
                        Task.Run(() => _unitOfWork.CivilWithLegsRepository.RefreshView(connectionString, "MV_RADIO_ANTENNA_LIBRARY_VIEW"));
                    }
                    else if (Helpers.Constants.LoadSubType.TLIradioOtherLibrary.ToString() == TableName)
                    {
                        //  Task.Run(() => _unitOfWork.CivilWithLegsRepository.RefreshView(connectionString, "MV_MWDISH_LIBRARY_VIEW"));
                    }
                    else if (Helpers.Constants.LoadSubType.TLIradioRRULibrary.ToString() == TableName)
                    {
                          Task.Run(() => _unitOfWork.CivilWithLegsRepository.RefreshView(connectionString, "MV_RADIO_RRU_LIBRARY_VIEW"));
                    }
                    return new Response<AllItemAttributes>();
                }
                catch (Exception err)
                {

                    return new Response<AllItemAttributes>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
                }
            }

        }
        //Function table name Entity by TableName
        //specify the table i deal with
        //get activated attributes
        //get dynamic attributes
        public Response<GetForAddCivilLibrarybject> GetForAdd(string TableName)
        {
            try
            {
                GetForAddCivilLibrarybject attributes = new GetForAddCivilLibrarybject();
                var TableNameEntity = _unitOfWork.TablesNamesRepository.GetWhereFirst(l => l.TableName == TableName);
                if (Helpers.Constants.LoadSubType.TLIradioAntennaLibrary.ToString() == TableName)
                {
                    var ListAttributesActivated = _unitOfWork.AttributeActivatedRepository.GetAttributeActivatedGetForAdd(TablesNames.TLIradioAntennaLibrary.ToString(), null, null);
                    var LogisticalItems = _unitOfWork.LogistcalRepository.GetLogisticalLibrary("Radio");
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

                }
                else if (Helpers.Constants.LoadSubType.TLIradioRRULibrary.ToString() == TableName)
                {
                    var ListAttributesActivated = _unitOfWork.AttributeActivatedRepository.GetAttributeActivatedGetForAdd(TablesNames.TLIradioRRULibrary.ToString(), null, null);
                    var LogisticalItems = _unitOfWork.LogistcalRepository.GetLogisticalLibrary("Radio");
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

                }
                else if (Helpers.Constants.LoadSubType.TLIradioOtherLibrary.ToString() == TableName)
                {
                    var ListAttributesActivated = _unitOfWork.AttributeActivatedRepository.GetAttributeActivatedGetForAdd(Helpers.Constants.TablesNames.TLIradioOther.ToString(), null, null).ToList();
                    ListAttributesActivated.AddRange(_unitOfWork.LogistcalRepository.GetLogisticalLibrary("Radio"));
                    attributes.AttributesActivatedLibrary = ListAttributesActivated;
                    attributes.DynamicAttributes = _unitOfWork.DynamicAttRepository.GetDynamicLibAtt(TableNameEntity.Id, null);
                  
                }
                return new Response<GetForAddCivilLibrarybject>(true, attributes, null, null, (int)Helpers.Constants.ApiReturnCode.success);
            }
            catch (Exception err)
            {

                return new Response<GetForAddCivilLibrarybject>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
        }
        //Function take 2 parameters
        //get table name Entity depened on table name
        //specify the table i deal with
        //get record by Id
        //set Deleted is true
        //update Entity
        //update dynamic attributes
        public async Task<Response<AllItemAttributes>> DeletedRadioLibrary(string TableName, int Id,int UserId,string connectionString)
        {
            using (TransactionScope transaction = new TransactionScope())
            {
                try
                {
                    var TableNameEntity = _unitOfWork.TablesNamesRepository.GetWhereFirst(l => l.TableName == TableName);
                    if (Helpers.Constants.LoadSubType.TLIradioAntennaLibrary.ToString() == TableName)
                    {
                        var CivilLoad = _unitOfWork.CivilLoadsRepository.GetIncludeWhere(x => x.allLoadInstId != null &&!x.Dismantle&&
                        x.allLoadInst.radioAntenna.radioAntennaLibraryId == Id, x => x.allLoadInst, x => x.allLoadInst.radioAntenna).ToList();
                        var OldRadioAntennaLibrary = _unitOfWork.RadioAntennaLibraryRepository.GetAllAsQueryable().AsNoTracking().FirstOrDefault(x => x.Id == Id);
                        if (CivilLoad != null && CivilLoad.Count > 0)
                            return new Response<AllItemAttributes>(false, null, null, "Can not delete this item because is used", (int)Helpers.Constants.ApiReturnCode.fail);

                        var NewRadioAntennaEntity = _unitOfWork.RadioAntennaLibraryRepository.GetByID(Id);
                        NewRadioAntennaEntity.Deleted = true;
                        NewRadioAntennaEntity.Model = NewRadioAntennaEntity.Model + "_" + DateTime.Now.ToString();
                        _unitOfWork.RadioAntennaLibraryRepository.UpdateWithHistory(UserId, OldRadioAntennaLibrary, NewRadioAntennaEntity);
                        DisableDynamicAttLibValues(TableNameEntity.Id, Id, UserId);
                        await _unitOfWork.SaveChangesAsync();
                        AddHistory(NewRadioAntennaEntity.Id, Helpers.Constants.HistoryType.Delete.ToString(), Helpers.Constants.TablesNames.TLIradioAntennaLibrary.ToString());
                    }
                    else if (Helpers.Constants.LoadSubType.TLIradioOtherLibrary.ToString() == TableName)
                    {
                        var CivilLoad = _unitOfWork.CivilLoadsRepository.GetIncludeWhere(x => x.allLoadInstId != null && !x.Dismantle &&
                        x.allLoadInst.radioOther.radioOtherLibraryId == Id, x => x.allLoadInst, x => x.allLoadInst.radioOther).ToList();
                        var OldRadioOtherLibrary = _unitOfWork.RadioOtherLibraryRepository.GetAllAsQueryable().AsNoTracking().FirstOrDefault(x => x.Id == Id);
                        if (CivilLoad != null && CivilLoad.Count > 0)
                            return new Response<AllItemAttributes>(false, null, null, "Can not delete this item because is used", (int)Helpers.Constants.ApiReturnCode.fail);

                        TLIradioOtherLibrary NewRadioOtherLibrary = _unitOfWork.RadioOtherLibraryRepository.GetByID(Id);
                        NewRadioOtherLibrary.Deleted = true;
                        NewRadioOtherLibrary.Model = NewRadioOtherLibrary.Model + "_" + DateTime.Now.ToString();
                        _unitOfWork.RadioOtherLibraryRepository.UpdateWithHistory(UserId, OldRadioOtherLibrary, NewRadioOtherLibrary);
                        DisableDynamicAttLibValues(TableNameEntity.Id, Id, UserId);
                        await _unitOfWork.SaveChangesAsync();
                        AddHistory(NewRadioOtherLibrary.Id, Helpers.Constants.HistoryType.Delete.ToString(), Helpers.Constants.LoadSubType.TLIradioOtherLibrary.ToString());
                    }
                    else if (Helpers.Constants.LoadSubType.TLIradioRRULibrary.ToString() == TableName)
                    {
                        var CivilLoad = _unitOfWork.CivilLoadsRepository.GetIncludeWhere(x => x.allLoadInstId != null && !x.Dismantle &&
                          x.allLoadInst.radioRRU.radioRRULibraryId == Id, x => x.allLoadInst, x => x.allLoadInst.radioRRU).ToList();
                        var OldRadioRRULibrary = _unitOfWork.RadioRRULibraryRepository.GetAllAsQueryable().AsNoTracking().FirstOrDefault(x => x.Id == Id);
                        if (CivilLoad != null && CivilLoad.Count > 0)
                            return new Response<AllItemAttributes>(false, null, null, "Can not delete this item because is used", (int)Helpers.Constants.ApiReturnCode.fail);
                        TLIradioRRULibrary NewRadioRRULibrary = _unitOfWork.RadioRRULibraryRepository.GetByID(Id);
                        NewRadioRRULibrary.Deleted = true;
                        NewRadioRRULibrary.Model = NewRadioRRULibrary.Model + "_" + DateTime.Now.ToString();
                        _unitOfWork.RadioRRULibraryRepository.UpdateWithHistory(UserId, OldRadioRRULibrary, NewRadioRRULibrary);
                        DisableDynamicAttLibValues(TableNameEntity.Id, Id, UserId);
                        await _unitOfWork.SaveChangesAsync();
                        AddHistory(NewRadioRRULibrary.Id, Helpers.Constants.HistoryType.Delete.ToString(), Helpers.Constants.TablesNames.TLIradioRRULibrary.ToString());
                    }
                    transaction.Complete();
                    if (Helpers.Constants.LoadSubType.TLIradioAntennaLibrary.ToString() == TableName)
                    {
                        Task.Run(() => _unitOfWork.CivilWithLegsRepository.RefreshView(connectionString, "MV_RADIO_ANTENNA_LIBRARY_VIEW"));
                    }
                    else if (Helpers.Constants.LoadSubType.TLIradioOtherLibrary.ToString() == TableName)
                    {
                      //  Task.Run(() => _unitOfWork.CivilWithLegsRepository.RefreshView(connectionString, "MV_MWDISH_LIBRARY_VIEW"));
                    }
                    else if (Helpers.Constants.LoadSubType.TLIradioRRULibrary.ToString() == TableName)
                    {
                       Task.Run(() => _unitOfWork.CivilWithLegsRepository.RefreshView(connectionString, "MV_RADIO_RRU_LIBRARY_VIEW"));
                    }
                
                    return new Response<AllItemAttributes>();
                }
                catch (Exception err)
                {

                    return new Response<AllItemAttributes>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
                }
            }
        }
        private void DisableDynamicAttLibValues(int TableNameId, int Id, int UserId)
        {
            var DynamiAttLibValues = db.TLIdynamicAttLibValue
                .Where(d => d.InventoryId == Id && d.tablesNamesId == TableNameId)
                .ToList();
            foreach (var DynamiAttLibValue in DynamiAttLibValues)
            {
                var OldDynamiAttLibValues = _unitOfWork.DynamicAttLibValueRepository.GetAllAsQueryable().AsNoTracking()
                .FirstOrDefault(d => d.Id == DynamiAttLibValue.Id);
                DynamiAttLibValue.disable = true;
                _unitOfWork.DynamicAttLibValueRepository.UpdateWithHistory(UserId, OldDynamiAttLibValues, DynamiAttLibValue);
            }
        }
        #region Add History
        public void AddHistory(int Radio_lib_id, string historyType, string TableName)
        {

            AddTablesHistoryViewModel history = new AddTablesHistoryViewModel();
            history.RecordId = Radio_lib_id;
            history.TablesNameId = _unitOfWork.TablesNamesRepository.GetWhereFirst(x => x.TableName == TableName).Id;
            history.HistoryTypeId = _unitOfWork.HistoryTypeRepository.GetWhereSelectFirst(x => x.Name == historyType, x => new { x.Id }).Id;
            history.UserId = 83;
            _unitOfWork.TablesHistoryRepository.AddTableHistory(history);

        }
        #endregion
        #region AddHistoryForEdit
        public int AddHistoryForEdit(int RecordId, int TableNameid, string HistoryType, List<TLIhistoryDetails> details)
        {
            AddTablesHistoryViewModel history = new AddTablesHistoryViewModel();
            history.RecordId = RecordId;
            history.TablesNameId = TableNameid;//_unitOfWork.TablesNamesRepository.GetWhereFirst(x => x.Id == TableNameid).Id;
            history.HistoryTypeId = _unitOfWork.HistoryTypeRepository.GetWhereSelectFirst(x => x.Name == HistoryType, x => new { x.Id }).Id;
            history.UserId = 83;
            int? TableHistoryId = null;
            var CheckTableHistory = _unitOfWork.TablesHistoryRepository.GetWhereFirst(x => x.HistoryType.Name == HistoryType && x.RecordId == RecordId.ToString() && x.TablesNameId == TableNameid);
            if (CheckTableHistory != null)
            {
                var TableHistory = _unitOfWork.TablesHistoryRepository.GetWhereAndSelect(x => x.HistoryType.Name == HistoryType && x.RecordId == RecordId.ToString() && x.TablesNameId == TableNameid, x => new { x.Id }).ToList().Max(x => x.Id);
                if (TableHistory != null)
                    TableHistoryId = TableHistory;
                if (TableHistoryId != null)
                {
                    history.PreviousHistoryId = TableHistoryId;
                }
            }

            int HistoryId = _unitOfWork.TablesHistoryRepository.AddTableHistory(history, details);
            _unitOfWork.SaveChangesAsync();
            return HistoryId;
        }

        #endregion
    }
}
