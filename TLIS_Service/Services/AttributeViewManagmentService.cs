using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using TLIS_DAL.Helper;
using TLIS_DAL.Helper.Filters;
using TLIS_DAL.Helpers;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModels.AttActivatedCategoryDTOs;
using TLIS_DAL.ViewModels.AttributeActivatedDTOs;
using TLIS_DAL.ViewModels.AttributeViewManagmentDTOs;
using TLIS_DAL.ViewModels.TablesNamesDTOs;
using TLIS_Repository.Base;
using TLIS_Repository.Helpers;
using TLIS_Service.IService;

namespace TLIS_Service.Services
{
    public class AttributeViewManagmentService: IAttributeViewManagmentService
    {
        IUnitOfWork _unitOfWork;
        IServiceCollection _services;
        ServiceProvider _serviceProvider;
        private IMapper _mapper;
        public AttributeViewManagmentService(IUnitOfWork unitOfWork, IServiceCollection services, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _services = services;
            _serviceProvider = _services.BuildServiceProvider();
            _mapper = mapper;
        }

        //public async Task<Response<List<AttributeViewManagmentViewModel>>> GetAllAttributes()
        //{
        //    try
        //    {
        //        List<AttributeViewManagmentViewModel> OutPutList = new List<AttributeViewManagmentViewModel>();

        //        int count = 0;

        //        List<TLIeditableManagmentView> EditableManagmentViewData = _unitOfWork.EditableManagmentViewRepository
        //            .GetAllIncludeMultiple(null, null, out count, null)
        //            .ToList();

        //        using (TransactionScope transaction = new TransactionScope())
        //        {
        //            foreach (var EditableManagmentViewRecord in EditableManagmentViewData)
        //            {
        //                TablesNamesViewModel Table1 = _unitOfWork.TablesNamesRepository.Get(EditableManagmentViewRecord.TLItablesNames1Id);

        //                TablesNamesViewModel Table2 = _mapper.Map<TablesNamesViewModel>(_unitOfWork.TablesNamesRepository.GetWhereFirst(x =>
        //                    EditableManagmentViewRecord.TLItablesNames2Id != null ? (x.Id == EditableManagmentViewRecord.TLItablesNames2Id.Value) : false));

        //                int CategoryId = 0;
        //                List<TLIattributeActivated> AttributeActivatedList = new List<TLIattributeActivated>();
        //                if (EditableManagmentViewRecord.CivilWithoutLegCategoryId != null)
        //                {
        //                    CategoryId = EditableManagmentViewRecord.CivilWithoutLegCategoryId.Value;
        //                    List<TLIattActivatedCategory> AttributeActivatedCategories = _unitOfWork.AttActivatedCategoryRepository.GetIncludeWhere(x => 
        //                        x.civilWithoutLegCategoryId == CategoryId && x.enable && 
        //                        (x.attributeActivated.Tabel == Table1.TableName || x.attributeActivated.Tabel == Table2.TableName), x => x.attributeActivated).ToList();
        //                    foreach (TLIattActivatedCategory AttributeActivatedCategory in AttributeActivatedCategories)
        //                    {
        //                        TLIattributeViewManagment AttributeViewManagment = new TLIattributeViewManagment();
        //                        TLIattributeViewManagment AttributeActivatedCheck = _unitOfWork.AttributeViewManagmentRepository.GetIncludeWhereFirst(x =>
        //                            x.AttributeActivatedId != null &&
        //                            x.AttributeActivatedId == AttributeActivatedCategory.attributeActivatedId, x => x.AttributeActivated);
        //                        if (AttributeActivatedCheck == null)
        //                        {
        //                            AttributeViewManagment.AttributeActivatedId = AttributeActivatedCategory.attributeActivatedId;
        //                            AttributeViewManagment.Enable = true;
        //                            AttributeViewManagment.EditableManagmentViewId = EditableManagmentViewRecord.Id;

        //                            await _unitOfWork.AttributeViewManagmentRepository.AddAsync(AttributeViewManagment);
        //                            await _unitOfWork.SaveChangesAsync();

        //                            string Key = _unitOfWork.AttributeActivatedRepository.GetByID(AttributeViewManagment.AttributeActivatedId.Value).Key;

        //                            AttributeViewManagmentViewModel NewObjectToAdd = new AttributeViewManagmentViewModel
        //                            {
        //                                Key = Key,
        //                                Enable = AttributeViewManagment.Enable,
        //                                AttributeActivatedId = AttributeViewManagment.AttributeActivatedId,
        //                                EditableManagmentViewId = AttributeViewManagment.EditableManagmentViewId
        //                            };
        //                            OutPutList.Add(NewObjectToAdd);
        //                        }
        //                        else
        //                        {
        //                            AttributeViewManagmentViewModel NewObjectToAdd = new AttributeViewManagmentViewModel
        //                            {
        //                                Key = AttributeActivatedCheck.AttributeActivated.Key,
        //                                Enable = AttributeActivatedCheck.Enable,
        //                                AttributeActivatedId = AttributeActivatedCheck.AttributeActivatedId,
        //                                EditableManagmentViewId = AttributeActivatedCheck.EditableManagmentViewId
        //                            };
        //                            OutPutList.Add(NewObjectToAdd);
        //                        }
        //                    }
        //                }
        //                else
        //                {
        //                    AttributeActivatedList = _unitOfWork.AttributeActivatedRepository.GetWhere(x =>
        //                        (x.Tabel == Table1.TableName ||
        //                         x.Tabel == Table2.TableName) && x.enable).ToList();
        //                    foreach (TLIattributeActivated AttActivatedMatch in AttributeActivatedList)
        //                    {
        //                        TLIattributeViewManagment AttributeViewManagment = new TLIattributeViewManagment();

        //                        TLIattributeViewManagment AttributeActivatedCheck = _unitOfWork.AttributeViewManagmentRepository.GetIncludeWhereFirst(x =>
        //                            x.AttributeActivatedId != null &&
        //                            x.AttributeActivatedId == AttActivatedMatch.Id, x => x.AttributeActivated);

        //                        if (AttributeActivatedCheck == null)
        //                        {
        //                            AttributeViewManagment.AttributeActivatedId = AttActivatedMatch.Id;
        //                            AttributeViewManagment.Enable = true;
        //                            AttributeViewManagment.EditableManagmentViewId = EditableManagmentViewRecord.Id;

        //                            await _unitOfWork.AttributeViewManagmentRepository.AddAsync(AttributeViewManagment);
        //                            await _unitOfWork.SaveChangesAsync();

        //                            string Key = _unitOfWork.AttributeActivatedRepository.GetByID(AttributeViewManagment.AttributeActivatedId.Value).Key;

        //                            AttributeViewManagmentViewModel NewObjectToAdd = new AttributeViewManagmentViewModel
        //                            {
        //                                Key = Key,
        //                                Enable = AttributeViewManagment.Enable,
        //                                AttributeActivatedId = AttributeViewManagment.AttributeActivatedId,
        //                                EditableManagmentViewId = AttributeViewManagment.EditableManagmentViewId
        //                            };
        //                            OutPutList.Add(NewObjectToAdd);
        //                        }
        //                        else
        //                        {
        //                            AttributeViewManagmentViewModel NewObjectToAdd = new AttributeViewManagmentViewModel
        //                            {
        //                                Key = AttributeActivatedCheck.AttributeActivated.Key,
        //                                Enable = AttributeActivatedCheck.Enable,
        //                                AttributeActivatedId = AttributeActivatedCheck.AttributeActivatedId,
        //                                EditableManagmentViewId = AttributeActivatedCheck.EditableManagmentViewId
        //                            };
        //                            OutPutList.Add(NewObjectToAdd);
        //                        }
        //                    }
        //                    // Add The Dynamic Attribute Data To Attribute View Managment Table...
        //                    List<TLIdynamicAtt> DynamicAttributesList = _unitOfWork.DynamicAttRepository.GetWhere(x =>
        //                        (x.tablesNamesId == Table1.Id ||
        //                         x.tablesNamesId == Table2.Id) && !x.disable)
        //                        .ToList();

        //                    foreach (TLIdynamicAtt DynamicAttribute in DynamicAttributesList)
        //                    {
        //                        TLIattributeViewManagment AttributeViewManagment = new TLIattributeViewManagment();

        //                        TLIattributeViewManagment AttributeActivatedCheck = _unitOfWork.AttributeViewManagmentRepository.GetIncludeWhereFirst(x =>
        //                            x.DynamicAttId != null &&
        //                            x.DynamicAttId == DynamicAttribute.Id, x => x.DynamicAtt);

        //                        if (AttributeActivatedCheck == null)
        //                        {
        //                            AttributeViewManagment.DynamicAttId = DynamicAttribute.Id;
        //                            AttributeViewManagment.Enable = true;
        //                            AttributeViewManagment.EditableManagmentViewId = EditableManagmentViewRecord.Id;

        //                            await _unitOfWork.AttributeViewManagmentRepository.AddAsync(AttributeViewManagment);
        //                            await _unitOfWork.SaveChangesAsync();

        //                            string key = _unitOfWork.AttributeActivatedRepository.GetByID(AttributeViewManagment.DynamicAttId.Value).Key;

        //                            AttributeViewManagmentViewModel NewObjectToAdd = new AttributeViewManagmentViewModel
        //                            {
        //                                Key = key,
        //                                Enable = AttributeViewManagment.Enable,
        //                                DynamicAttId = AttributeViewManagment.DynamicAttId,
        //                                EditableManagmentViewId = AttributeViewManagment.EditableManagmentViewId
        //                            };
        //                            OutPutList.Add(NewObjectToAdd);
        //                        }
        //                        else
        //                        {
        //                            AttributeViewManagmentViewModel NewObjectToAdd = new AttributeViewManagmentViewModel
        //                            {
        //                                Key = AttributeActivatedCheck.DynamicAtt.Key,
        //                                Enable = AttributeActivatedCheck.Enable,
        //                                DynamicAttId = AttributeActivatedCheck.DynamicAttId,
        //                                EditableManagmentViewId = AttributeActivatedCheck.EditableManagmentViewId
        //                            };
        //                            OutPutList.Add(NewObjectToAdd);
        //                        }
        //                    }
        //                }
        //            }
        //            transaction.Complete();
        //        }

        //        return new Response<List<AttributeViewManagmentViewModel>>(true, OutPutList, null, null, (int)Helpers.Constants.ApiReturnCode.success);
        //    }
        //    catch (Exception err)
        //    {
        //        return new Response<List<AttributeViewManagmentViewModel>>(false, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
        //    }
        //}
        public Response<List<AttributeViewManagmentViewModel>> GetAttributesForViewWithoutPagination(string ViewName, string Search)
        {
            try
            {
                int Count = 0;
                TLIeditableManagmentView EditableManagmentViewData = _unitOfWork.EditableManagmentViewRepository
                    .GetWhereFirst(x => x.View == ViewName);

                if (EditableManagmentViewData == null)
                {
                    return new Response<List<AttributeViewManagmentViewModel>>(true, null, null, $"This View: {ViewName} Doesn't Exist", (int)Helpers.Constants.ApiReturnCode.fail);
                }

                int EditableManagmentViewDataId = EditableManagmentViewData.Id;

                List<AttributeViewManagmentViewModel> AllEnableAttributes = new List<AttributeViewManagmentViewModel>();

                if (EditableManagmentViewData.CivilWithoutLegCategoryId == null)
                {
                    if (!string.IsNullOrEmpty(Search))
                    {
                        AllEnableAttributes = _mapper.Map<List<AttributeViewManagmentViewModel>>(_unitOfWork.AttributeViewManagmentRepository.GetIncludeWhere(x =>
                            (x.EditableManagmentViewId == EditableManagmentViewDataId) &&
                            (x.DynamicAttId != null ?
                                (x.DynamicAtt.Key.ToLower().StartsWith(Search.ToLower()) && !x.DynamicAtt.disable)
                                :
                                (x.AttributeActivated.Key.ToLower().StartsWith(Search.ToLower()) && x.AttributeActivated.enable && !x.AttributeActivated.Key.Contains("Id")
                                && x.AttributeActivated.Key.ToLower() != "active" && x.AttributeActivated.Key.ToLower() != "deleted"))

                            , x => x.AttributeActivated, x => x.DynamicAtt).ToList());
                    }
                    else
                    {
                        AllEnableAttributes = _mapper.Map<List<AttributeViewManagmentViewModel>>(_unitOfWork.AttributeViewManagmentRepository.GetIncludeWhere(x =>
                            x.EditableManagmentViewId == EditableManagmentViewDataId &&
                            ((x.DynamicAttId != null) ? (!x.DynamicAtt.disable) : (x.AttributeActivated.enable && !x.AttributeActivated.Key.Contains("Id") && x.AttributeActivated.Key.ToLower() != "active" && x.AttributeActivated.Key.ToLower() != "deleted"))
                            , x => x.AttributeActivated, x => x.DynamicAtt).ToList());
                    }
                }
                else
                {
                    if (string.IsNullOrEmpty(Search))
                    {
                        List<AttributeViewManagmentViewModel> DynamicAttributes = _mapper.Map<List<AttributeViewManagmentViewModel>>(_unitOfWork.AttributeViewManagmentRepository
                            .GetIncludeWhere(x => x.EditableManagmentViewId == EditableManagmentViewData.Id &&
                                (x.DynamicAttId != null ? x.DynamicAtt.CivilWithoutLegCategoryId == EditableManagmentViewData.CivilWithoutLegCategoryId : false),
                                x => x.DynamicAtt)
                            .ToList());

                        List<AttActivatedCategoryViewModel> StaticAttributesInCategory = new List<AttActivatedCategoryViewModel>();
                        if (ViewName.ToLower() == Helpers.Constants.EditableManamgmantViewNames.CivilWithoutLegsLibraryCapsule.ToString().ToLower() ||
                            ViewName.ToLower() == Helpers.Constants.EditableManamgmantViewNames.CivilWithoutLegsLibraryMast.ToString().ToLower() ||
                            ViewName.ToLower() == Helpers.Constants.EditableManamgmantViewNames.CivilWithoutLegsLibraryMonopole.ToString().ToLower())
                        {
                            StaticAttributesInCategory = _mapper.Map<List<AttActivatedCategoryViewModel>>(_unitOfWork.AttActivatedCategoryRepository
                                .GetIncludeWhere(x => x.civilWithoutLegCategoryId == EditableManagmentViewData.CivilWithoutLegCategoryId && x.IsLibrary && x.enable &&
                                    !x.attributeActivated.Key.Contains("Id") && x.attributeActivated.Key.ToLower() != "active" && x.attributeActivated.Key.ToLower() != "deleted",
                                        x => x.attributeActivated)
                                .ToList());
                        }
                        else if (ViewName.ToLower() == Helpers.Constants.EditableManamgmantViewNames.CivilWithoutLegInstallationCapsule.ToString().ToLower() ||
                                 ViewName.ToLower() == Helpers.Constants.EditableManamgmantViewNames.CivilWithoutLegInstallationMast.ToString().ToLower() ||
                                 ViewName.ToLower() == Helpers.Constants.EditableManamgmantViewNames.CivilWithoutLegInstallationMonopole.ToString().ToLower())
                        {
                            StaticAttributesInCategory = _mapper.Map<List<AttActivatedCategoryViewModel>>(_unitOfWork.AttActivatedCategoryRepository
                                .GetIncludeWhere(x => x.civilWithoutLegCategoryId == EditableManagmentViewData.CivilWithoutLegCategoryId && !x.IsLibrary && x.enable &&
                                    !x.attributeActivated.Key.Contains("Id") && x.attributeActivated.Key.ToLower() != "active" && x.attributeActivated.Key.ToLower() != "deleted",
                                        x => x.attributeActivated)
                                .ToList());
                        }
                        List<AttributeViewManagmentViewModel> StaticAttributesInView = _mapper.Map<List<AttributeViewManagmentViewModel>>(_unitOfWork.AttributeViewManagmentRepository
                            .GetWhere(x => x.EditableManagmentViewId == EditableManagmentViewData.Id &&
                                x.AttributeActivated != null && StaticAttributesInCategory.Exists(y => y.attributeActivatedId == x.AttributeActivatedId))
                            .ToList());

                        StaticAttributesInView.AddRange(DynamicAttributes);
                        AllEnableAttributes = StaticAttributesInView;
                    }
                    else
                    {
                        List<AttributeViewManagmentViewModel> DynamicAttributes = _mapper.Map<List<AttributeViewManagmentViewModel>>(_unitOfWork.AttributeViewManagmentRepository
                            .GetIncludeWhere(x => x.EditableManagmentViewId == EditableManagmentViewData.Id &&
                                (x.DynamicAttId != null ?
                                    (x.DynamicAtt.CivilWithoutLegCategoryId == EditableManagmentViewData.CivilWithoutLegCategoryId &&
                                     x.DynamicAtt.Key.ToLower().StartsWith(Search.ToLower())) : false), x => x.DynamicAtt)
                            .ToList());

                        List<AttActivatedCategoryViewModel> StaticAttributesInCategory = new List<AttActivatedCategoryViewModel>();
                        if (ViewName.ToLower() == Helpers.Constants.EditableManamgmantViewNames.CivilWithoutLegsLibraryCapsule.ToString().ToLower() ||
                            ViewName.ToLower() == Helpers.Constants.EditableManamgmantViewNames.CivilWithoutLegsLibraryMast.ToString().ToLower() ||
                            ViewName.ToLower() == Helpers.Constants.EditableManamgmantViewNames.CivilWithoutLegsLibraryMonopole.ToString().ToLower())
                        {
                            StaticAttributesInCategory = _mapper.Map<List<AttActivatedCategoryViewModel>>(_unitOfWork.AttActivatedCategoryRepository
                                .GetIncludeWhere(x => x.civilWithoutLegCategoryId == EditableManagmentViewData.CivilWithoutLegCategoryId && x.IsLibrary && x.enable &&
                                    !x.attributeActivated.Key.Contains("Id") && x.attributeActivated.Key.ToLower() != "active" && x.attributeActivated.Key.ToLower() != "deleted" &&
                                     x.attributeActivated.Key.ToLower().StartsWith(Search.ToLower()),
                                        x => x.attributeActivated)
                                .ToList());
                        }
                        else if (ViewName.ToLower() == Helpers.Constants.EditableManamgmantViewNames.CivilWithoutLegInstallationCapsule.ToString().ToLower() ||
                                 ViewName.ToLower() == Helpers.Constants.EditableManamgmantViewNames.CivilWithoutLegInstallationMast.ToString().ToLower() ||
                                 ViewName.ToLower() == Helpers.Constants.EditableManamgmantViewNames.CivilWithoutLegInstallationMonopole.ToString().ToLower())
                        {
                            StaticAttributesInCategory = _mapper.Map<List<AttActivatedCategoryViewModel>>(_unitOfWork.AttActivatedCategoryRepository
                               .GetIncludeWhere(x => x.civilWithoutLegCategoryId == EditableManagmentViewData.CivilWithoutLegCategoryId && !x.IsLibrary && x.enable &&
                                   !x.attributeActivated.Key.Contains("Id") && x.attributeActivated.Key.ToLower() != "active" && x.attributeActivated.Key.ToLower() != "deleted" &&
                                    x.attributeActivated.Key.ToLower().StartsWith(Search.ToLower()),
                                       x => x.attributeActivated)
                               .ToList());
                        }
                        List<AttributeViewManagmentViewModel> StaticAttributesInView = _mapper.Map<List<AttributeViewManagmentViewModel>>(_unitOfWork.AttributeViewManagmentRepository
                            .GetWhere(x => x.EditableManagmentViewId == EditableManagmentViewData.Id &&
                                x.AttributeActivated != null && StaticAttributesInCategory.Exists(y => y.attributeActivatedId == x.AttributeActivatedId))
                            .ToList());

                        StaticAttributesInView.AddRange(DynamicAttributes);
                        AllEnableAttributes = StaticAttributesInView;
                    }
                }

                Count = AllEnableAttributes.Count();

                return new Response<List<AttributeViewManagmentViewModel>>(true, AllEnableAttributes, null, null, (int)Helpers.Constants.ApiReturnCode.success, Count);
            }
            catch (Exception err)
            {
                return new Response<List<AttributeViewManagmentViewModel>>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
        }
        public Response<List<AttributeViewManagmentViewModel>> GetAttributesForView(string ViewName, ParameterPagination parameterPagination, string Search)
        {
            try
            {
                int Count = 0;
                TLIeditableManagmentView EditableManagmentViewData = _unitOfWork.EditableManagmentViewRepository
                    .GetWhereFirst(x => x.View == ViewName);
                
                if(EditableManagmentViewData == null)
                {
                    return new Response<List<AttributeViewManagmentViewModel>>(true, null, null, $"This View: {ViewName} Doesn't Exist", (int)Helpers.Constants.ApiReturnCode.fail);
                }

                int EditableManagmentViewDataId = EditableManagmentViewData.Id;

                List<AttributeViewManagmentViewModel> AllEnableAttributes = new List<AttributeViewManagmentViewModel>();

                if (EditableManagmentViewData.CivilWithoutLegCategoryId == null)
                {
                    if (!string.IsNullOrEmpty(Search))
                    {
                        AllEnableAttributes = _mapper.Map<List<AttributeViewManagmentViewModel>>(_unitOfWork.AttributeViewManagmentRepository.GetIncludeWhere(x =>
                            (x.EditableManagmentViewId == EditableManagmentViewDataId) &&
                            (x.DynamicAttId != null ?
                                (x.DynamicAtt.Key.ToLower().StartsWith(Search.ToLower()) && !x.DynamicAtt.disable)
                                :
                                (x.AttributeActivated.Key.ToLower().StartsWith(Search.ToLower()) && x.AttributeActivated.enable && !x.AttributeActivated.Key.Contains("Id")
                                && x.AttributeActivated.Key.ToLower() != "active" && x.AttributeActivated.Key.ToLower() != "deleted"))

                            , x => x.AttributeActivated, x => x.DynamicAtt).ToList());
                    }
                    else
                    {
                        AllEnableAttributes = _mapper.Map<List<AttributeViewManagmentViewModel>>(_unitOfWork.AttributeViewManagmentRepository.GetIncludeWhere(x =>
                            x.EditableManagmentViewId == EditableManagmentViewDataId &&
                            ((x.DynamicAttId != null) ? (!x.DynamicAtt.disable) : (x.AttributeActivated.enable && !x.AttributeActivated.Key.Contains("Id") && x.AttributeActivated.Key.ToLower() != "active" && x.AttributeActivated.Key.ToLower() != "deleted"))
                            , x => x.AttributeActivated, x => x.DynamicAtt).ToList());
                    }
                }
                else
                {
                    if (string.IsNullOrEmpty(Search))
                    {
                        List<AttributeViewManagmentViewModel> DynamicAttributes = _mapper.Map<List<AttributeViewManagmentViewModel>>(_unitOfWork.AttributeViewManagmentRepository
                            .GetIncludeWhere(x => x.EditableManagmentViewId == EditableManagmentViewData.Id &&
                                (x.DynamicAttId != null ? x.DynamicAtt.CivilWithoutLegCategoryId == EditableManagmentViewData.CivilWithoutLegCategoryId : false),
                                x => x.DynamicAtt)
                            .ToList());

                        List<AttActivatedCategoryViewModel> StaticAttributesInCategory = new List<AttActivatedCategoryViewModel>();
                        if (ViewName.ToLower() == Helpers.Constants.EditableManamgmantViewNames.CivilWithoutLegsLibraryCapsule.ToString().ToLower() ||
                            ViewName.ToLower() == Helpers.Constants.EditableManamgmantViewNames.CivilWithoutLegsLibraryMast.ToString().ToLower() ||
                            ViewName.ToLower() == Helpers.Constants.EditableManamgmantViewNames.CivilWithoutLegsLibraryMonopole.ToString().ToLower())
                        {
                            StaticAttributesInCategory = _mapper.Map<List<AttActivatedCategoryViewModel>>(_unitOfWork.AttActivatedCategoryRepository
                                .GetIncludeWhere(x => x.civilWithoutLegCategoryId == EditableManagmentViewData.CivilWithoutLegCategoryId && x.IsLibrary && x.enable &&
                                    !x.attributeActivated.Key.Contains("Id") && x.attributeActivated.Key.ToLower() != "active" && x.attributeActivated.Key.ToLower() != "deleted",
                                        x => x.attributeActivated)
                                .ToList());
                        }
                        else if (ViewName.ToLower() == Helpers.Constants.EditableManamgmantViewNames.CivilWithoutLegInstallationCapsule.ToString().ToLower() ||
                                 ViewName.ToLower() == Helpers.Constants.EditableManamgmantViewNames.CivilWithoutLegInstallationMast.ToString().ToLower() ||
                                 ViewName.ToLower() == Helpers.Constants.EditableManamgmantViewNames.CivilWithoutLegInstallationMonopole.ToString().ToLower())
                        {
                            StaticAttributesInCategory = _mapper.Map<List<AttActivatedCategoryViewModel>>(_unitOfWork.AttActivatedCategoryRepository
                                .GetIncludeWhere(x => x.civilWithoutLegCategoryId == EditableManagmentViewData.CivilWithoutLegCategoryId && !x.IsLibrary && x.enable &&
                                    !x.attributeActivated.Key.Contains("Id") && x.attributeActivated.Key.ToLower() != "active" && x.attributeActivated.Key.ToLower() != "deleted",
                                        x => x.attributeActivated)
                                .ToList());
                        }
                        var temp = StaticAttributesInCategory.Select(x => x.attributeActivatedId).ToList();
                        List<AttributeViewManagmentViewModel> StaticAttributesInView = _mapper.Map<List<AttributeViewManagmentViewModel>>(_unitOfWork.AttributeViewManagmentRepository
                            .GetWhere(x => x.EditableManagmentViewId == EditableManagmentViewData.Id && x.AttributeActivated != null && temp.Any(y => y == x.AttributeActivatedId)).ToList());

                        //List<AttributeViewManagmentViewModel> StaticAttributesInView = _mapper.Map<List<AttributeViewManagmentViewModel>>(_unitOfWork.AttributeViewManagmentRepository
                        //    .GetWhere(x => x.EditableManagmentViewId == EditableManagmentViewData.Id &&
                        //        x.AttributeActivated != null && StaticAttributesInCategory.Exists(y => y.attributeActivatedId == x.AttributeActivatedId))
                        //    .ToList());

                        StaticAttributesInView.AddRange(DynamicAttributes);
                        AllEnableAttributes = StaticAttributesInView;
                    }
                    else
                    {
                        List<AttributeViewManagmentViewModel> DynamicAttributes = _mapper.Map<List<AttributeViewManagmentViewModel>>(_unitOfWork.AttributeViewManagmentRepository
                            .GetIncludeWhere(x => x.EditableManagmentViewId == EditableManagmentViewData.Id &&
                                (x.DynamicAttId != null ? 
                                    (x.DynamicAtt.CivilWithoutLegCategoryId == EditableManagmentViewData.CivilWithoutLegCategoryId && 
                                     x.DynamicAtt.Key.ToLower().StartsWith(Search.ToLower())) : false), x => x.DynamicAtt)
                            .ToList());

                        List<AttActivatedCategoryViewModel> StaticAttributesInCategory = new List<AttActivatedCategoryViewModel>();
                        if (ViewName.ToLower() == Helpers.Constants.EditableManamgmantViewNames.CivilWithoutLegsLibraryCapsule.ToString().ToLower() ||
                            ViewName.ToLower() == Helpers.Constants.EditableManamgmantViewNames.CivilWithoutLegsLibraryMast.ToString().ToLower() ||
                            ViewName.ToLower() == Helpers.Constants.EditableManamgmantViewNames.CivilWithoutLegsLibraryMonopole.ToString().ToLower())
                        {
                            StaticAttributesInCategory = _mapper.Map<List<AttActivatedCategoryViewModel>>(_unitOfWork.AttActivatedCategoryRepository
                                .GetIncludeWhere(x => x.civilWithoutLegCategoryId == EditableManagmentViewData.CivilWithoutLegCategoryId && x.IsLibrary && x.enable &&
                                    !x.attributeActivated.Key.Contains("Id") && x.attributeActivated.Key.ToLower() != "active" && x.attributeActivated.Key.ToLower() != "deleted" &&
                                     x.attributeActivated.Key.ToLower().StartsWith(Search.ToLower()),
                                        x => x.attributeActivated)
                                .ToList());
                        }
                        else if (ViewName.ToLower() == Helpers.Constants.EditableManamgmantViewNames.CivilWithoutLegInstallationCapsule.ToString().ToLower() ||
                                 ViewName.ToLower() == Helpers.Constants.EditableManamgmantViewNames.CivilWithoutLegInstallationMast.ToString().ToLower() ||
                                 ViewName.ToLower() == Helpers.Constants.EditableManamgmantViewNames.CivilWithoutLegInstallationMonopole.ToString().ToLower())
                        {
                            StaticAttributesInCategory = _mapper.Map<List<AttActivatedCategoryViewModel>>(_unitOfWork.AttActivatedCategoryRepository
                               .GetIncludeWhere(x => x.civilWithoutLegCategoryId == EditableManagmentViewData.CivilWithoutLegCategoryId && !x.IsLibrary && x.enable &&
                                   !x.attributeActivated.Key.Contains("Id") && x.attributeActivated.Key.ToLower() != "active" && x.attributeActivated.Key.ToLower() != "deleted" &&
                                    x.attributeActivated.Key.ToLower().StartsWith(Search.ToLower()),
                                       x => x.attributeActivated)
                               .ToList());
                        }
                        List<AttributeViewManagmentViewModel> StaticAttributesInView = _mapper.Map<List<AttributeViewManagmentViewModel>>(_unitOfWork.AttributeViewManagmentRepository
                            .GetWhere(x => x.EditableManagmentViewId == EditableManagmentViewData.Id &&
                                x.AttributeActivated != null && StaticAttributesInCategory.Select(y => y.attributeActivatedId).Contains(x.AttributeActivatedId))
                            .ToList());

                        StaticAttributesInView.AddRange(DynamicAttributes);
                        AllEnableAttributes = StaticAttributesInView;
                    }
                }

                AttributeViewManagmentViewModel NameAttribute = AllEnableAttributes.FirstOrDefault(x => x.Key.ToLower() == "Name".ToLower() ||
                    x.Key.ToLower() == "DishName".ToLower());

                if (NameAttribute != null)
                {
                    AttributeViewManagmentViewModel Swap = AllEnableAttributes[0];
                    AllEnableAttributes[AllEnableAttributes.IndexOf(NameAttribute)] = Swap;
                    AllEnableAttributes[0] = NameAttribute;
                }

                Count = AllEnableAttributes.Count();

                List<AttributeViewManagmentViewModel> AttributeViewManagmentList = AllEnableAttributes.Skip((parameterPagination.PageNumber - 1) * parameterPagination.PageSize)
                    .Take(parameterPagination.PageSize).ToList();

                return new Response<List<AttributeViewManagmentViewModel>>(true, AttributeViewManagmentList, null, null, (int)Helpers.Constants.ApiReturnCode.success, Count);
            }
            catch (Exception err)
            {
                return new Response<List<AttributeViewManagmentViewModel>>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
        }
        public async Task<Response<AttributeViewManagmentViewModel>> UpdateAttributeStatus(int AttributeViewManagmentId)
        {
            try
            {
                TLIattributeViewManagment AttributeViewManagment = _unitOfWork.AttributeViewManagmentRepository.GetWhereFirst(x => 
                    x.Id == AttributeViewManagmentId);

                if (AttributeViewManagment == null)
                {
                    return new Response<AttributeViewManagmentViewModel>(true, null, null, $"This Id: {AttributeViewManagmentId} is Not Exists",
                        (int)Helpers.Constants.ApiReturnCode.fail);
                }
                //if(AttributeViewManagment.AttributeActivatedId != null ? (
                //    !AttributeViewManagment.AttributeActivated.Manage ? (
                //        !AttributeViewManagment.AttributeActivated.Required || !AttributeViewManagment.AttributeActivated.enable) 
                //    :(false))
                //: (false))
                //{
                //    return new Response<AttributeViewManagmentViewModel>(true, null, null, "you can Not edit required or enable for attributes related to space calculation", (int)Helpers.Constants.ApiReturnCode.fail);
                //}
                AttributeViewManagment.Enable = !(AttributeViewManagment.Enable);

                _unitOfWork.AttributeViewManagmentRepository.Update(AttributeViewManagment);
                await _unitOfWork.SaveChangesAsync();
                AttributeViewManagmentViewModel OutPutViewModel = _mapper.Map<AttributeViewManagmentViewModel>(AttributeViewManagment);
                if(OutPutViewModel.DynamicAttId != null)
                {
                    OutPutViewModel.Key = _unitOfWork.DynamicAttRepository.GetWhereFirst(x => x.Id == OutPutViewModel.DynamicAttId).Key;
                }
                else
                {
                    OutPutViewModel.Key = _unitOfWork.AttributeActivatedRepository.GetWhereFirst(x => x.Id == OutPutViewModel.AttributeActivatedId).Key;
                }
                return new Response<AttributeViewManagmentViewModel>(true, OutPutViewModel, null, null, (int)Helpers.Constants.ApiReturnCode.success);
            }
            catch (Exception err)
            {
                return new Response<AttributeViewManagmentViewModel>(true, null, null, err.Message,
                    (int)Helpers.Constants.ApiReturnCode.fail);
            }
        }
    }
}
