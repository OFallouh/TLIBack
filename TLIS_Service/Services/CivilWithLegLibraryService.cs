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
using TLIS_DAL.ViewModels.AttributeActivatedDTOs;
using TLIS_DAL.ViewModels.CivilWithLegDTOs;
using TLIS_DAL.ViewModels.CivilWithLegLibraryDTOs;
using TLIS_DAL.ViewModels.DynamicAttDTOs;
using TLIS_DAL.ViewModels.DynamicAttInstValueDTOs;
using TLIS_DAL.ViewModels.DynamicAttLibValueDTOs;
using TLIS_Repository.Base;
using TLIS_Service.IService;

namespace TLIS_Service.Services
{
   public class CivilWithLegLibraryService : ICivilWithLegLibraryService
    {
        IUnitOfWork _unitOfWork;
        IServiceCollection _services;
        private IMapper _mapper;
        public CivilWithLegLibraryService(IUnitOfWork unitOfWork, IServiceCollection services,IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _services = services;
            ServiceProvider serviceProvider = _services.BuildServiceProvider();
            _mapper = mapper;
        }
        public Response<AllItemAttributes> GetById(int Id)
        {
            try
            {
                AllItemAttributes attributes = new AllItemAttributes();
                TLIcivilWithLegLibrary CivilWithLeg = _unitOfWork.CivilWithLegLibraryRepository.GetByID(Id);
               
                var AttributesActivated = _unitOfWork.AttributeActivatedRepository.GetWhere(x=>x.Tabel == "TLIcivilWithLegLibrary").ToList();
                List<BaseAttView> baseAtts = new List<BaseAttView>();
                Parallel.ForEach(AttributesActivated, AttributeActivated =>
                {
                    var value = CivilWithLeg.GetType().GetProperty(AttributeActivated.Key).GetValue(CivilWithLeg);
                    baseAtts.Add(new BaseAttView { Key = AttributeActivated.Key, Value = value.ToString(), Desc = AttributeActivated.Description, Label = AttributeActivated.Label, enable = AttributeActivated.enable });
                });
                attributes.AttributesActivated = baseAtts;
                List<FilterExperssion> filterExperssions = new List<FilterExperssion>();
                List<string> IdValues = new List<string>();
                IdValues.Add(Id.ToString());
                //List<string> CivilTypeIdValues = new List<string>();
                //CivilTypeIdValues.Add("1");
                //filterExperssions.Add(new FilterExperssion { propertyName = "CiviId", comparison = "==", value = IdValues });
                //filterExperssions.Add(new FilterExperssion { propertyName = "LibraryAtt", comparison = "==", value = CivilTypeIdValues });
                //var dynamicAtts = _unitOfWork.DynamicAttRepository.WhereFilters(filterExperssions).ToList();
                var dynamicAtts = _unitOfWork.DynamicAttRepository.GetWhere(x => x.LibraryAtt == true).ToList();
                List<DynamicAttLibViewModel> dynamicAttsViewModel = new List<DynamicAttLibViewModel>();
                Parallel.ForEach(dynamicAtts, dynamicAtt =>
                {
                    dynamicAttsViewModel.Add(new DynamicAttLibViewModel { Id = dynamicAtt.Id, DataTypeId = dynamicAtt.DataTypeId, Key = dynamicAtt.Key, Value = null });
                });
                //CivilTypeIdValues.Remove("1");
                //CivilTypeIdValues.Add("0");
                //List<KeyValuePair<DynamicAttViewModel,string>> test = new List<KeyValuePair<DynamicAttViewModel,string>>();
                Parallel.ForEach(dynamicAttsViewModel, dynamicAttViewModel =>
                {
                   // var DynamicAttLibValue = _unitOfWork.DynamicAttLibRepository.Where("DynamicAttId", "==", dynamicAttViewModel.Id.ToString()).FirstOrDefault();
                    var DynamicAttLibValue = _unitOfWork.DynamicAttLibRepository.GetWhereFirst(x=>x.DynamicAttId== dynamicAttViewModel.Id);
                    //dynamicAttViewModel.Value = DynamicAttLibValue;
                    //test.Add(KeyValuePair.Create<DynamicAttViewModel, string>(dynamicAttViewModel, DynamicAttInstValue));
                });
                attributes.DynamicAtts = dynamicAttsViewModel;
                //List<FilterExperssion> filterExperssions_1 = new List<FilterExperssion>();
                //filterExperssions_1.Add(new FilterExperssion { propertyName = "CiviId", comparison = "==", value = IdValues });
                //filterExperssions_1.Add(new FilterExperssion { propertyName = "LibraryAtt", comparison = "==", value = CivilTypeIdValues });
                //var InstAtts = _unitOfWork.DynamicAttRepository.WhereFilters(filterExperssions_1).ToList();
                var InstAtts = _unitOfWork.DynamicAttRepository.GetWhere(x=>x.LibraryAtt==false).ToList();
                List<DynaminAttInstViewModel> dynamicinstViewModel = _mapper.Map<List<DynaminAttInstViewModel>>(InstAtts);
                //foreach (var InstAtt in InstAtts)
                //{
                //    dynamicinstViewModel.Add(new DynaminAttInstViewModel { Id = InstAtt.Id, Key = InstAtt.Key , Validations = null, Dependencies = null });
                //}
                attributes.DynamicAttInst = dynamicinstViewModel;
                return new Response<AllItemAttributes>(true, attributes, null, null, (int)Helpers.Constants.ApiReturnCode.success);
            }
            catch(Exception err)
            {
                
                return new Response<AllItemAttributes>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);

            }
        }

        public async Task<Response<ReturnWithFilters<CivilWithLegLibraryViewModel>>> getCivilWithLegLibrariesAsync(List<FilterObjectList> filters, bool WithFilterData, ParameterPagination parameters)
        {
            try
            {
                IEnumerable<TLIcivilWithLegLibrary> CivilWithLegLibrariesList;
                if (filters.Count != 0)
                {
                    //List<FilterExperssion> filterExperssions = new List<FilterExperssion>();
                    //foreach (var filter in filters)
                    //{
                    //    filterExperssions.Add(new FilterExperssion { propertyName = filter.key, comparison = "==", value = filter.value });
                    //}
                    CivilWithLegLibrariesList = await _unitOfWork.CivilWithLegLibraryRepository.GetAllAsync(parameters, filters);
                }
                else
                {

                    CivilWithLegLibrariesList = await _unitOfWork.CivilWithLegLibraryRepository.GetAllAsync(parameters);
                }


                
                var FilteredCivilWithLegLibrariesModel = _mapper.Map<IEnumerable<CivilWithLegLibraryViewModel>>(CivilWithLegLibrariesList);
                ReturnWithFilters<CivilWithLegLibraryViewModel> CivilWithleg = new ReturnWithFilters<CivilWithLegLibraryViewModel>();
                CivilWithleg.Model = FilteredCivilWithLegLibrariesModel.ToList();
                if (WithFilterData.Equals(true))
                {
                    CivilWithleg.filters = _unitOfWork.CivilWithLegLibraryRepository.GetRelatedTables();
                }
                return new Response<ReturnWithFilters<CivilWithLegLibraryViewModel>>(true, CivilWithleg, null, null, (int)Helpers.Constants.ApiReturnCode.success);
            }
            catch(Exception err)
            {
                
                return new Response<ReturnWithFilters<CivilWithLegLibraryViewModel>>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
        }
        public Response<CivilWithLegLibraryViewModel> AddCivilWithLegLibrary(AddCivilWithLegLibraryViewModel CivilWithLegLibrary = null, List<AddDynamicLibAttValueViewModel> DynamicLibAttsValue = null, List<AddDynamicAttInstValueViewModel> DynamicAttsInstValue = null)
        {
            try
            {
                TLIcivilWithLegLibrary CivilWithLegEntites = _mapper.Map<TLIcivilWithLegLibrary>(CivilWithLegLibrary);
                _unitOfWork.CivilWithLegLibraryRepository.Add(CivilWithLegEntites);
                if (DynamicLibAttsValue.Count > 0)
                {
                    Parallel.ForEach(DynamicLibAttsValue, DynamicLibAttValue =>
                    {
                        var dynamicAttEntity = _mapper.Map<TLIdynamicAtt>(DynamicLibAttsValue);
                        _unitOfWork.DynamicAttRepository.Add(dynamicAttEntity);
                        var dynamicAttLibValueEntites = _mapper.Map<TLIdynamicAttLibValue>(DynamicLibAttValue);
                        dynamicAttLibValueEntites.DynamicAttId = dynamicAttEntity.Id;
                        _unitOfWork.DynamicAttLibRepository.Add(dynamicAttLibValueEntites);
                    });
                }
                if(DynamicAttsInstValue.Count > 0)
                {
                    var dynamicAttEntites = _mapper.Map<List<TLIdynamicAtt>>(DynamicAttsInstValue);
                    _unitOfWork.DynamicAttRepository.AddRangeAsync(dynamicAttEntites);
                }
                _unitOfWork.SaveChanges();
                return new Response<CivilWithLegLibraryViewModel>(true, null, null, null, (int)Helpers.Constants.ApiReturnCode.success);

            }
            catch(Exception err)
            {
                
                return new Response<CivilWithLegLibraryViewModel>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
        }

        public async Task EditCivilWithLegLibrary(EditCivilWithLegLibraryViewModels EditCivilWithLegLibraryViewModel)
        {
            TLIcivilWithLegLibrary CivilWithLegEntites = _mapper.Map<TLIcivilWithLegLibrary>(EditCivilWithLegLibraryViewModel);

            _unitOfWork.CivilWithLegLibraryRepository.Update(CivilWithLegEntites);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<Response<CivilWithLegLibraryViewModel>> Disable(int Id)
        {
            try
            {
                var CivilWithLeg = _unitOfWork.CivilWithLegLibraryRepository.GetByID(Id);
                var TableNameId = _unitOfWork.TablesNamesRepository.GetWhereSelectFirst(x => x.TableName == Helpers.Constants.CivilType.TLIcivilWithLegLibrary.ToString(),x=>new { x.Id}).Id;
                CivilWithLeg.Active = false;
                _unitOfWork.CivilWithLegLibraryRepository.Update(CivilWithLeg);
              //  await _unitOfWork.SaveChangesAsync();
                var DynamicAtts = _unitOfWork.DynamicAttRepository.GetWhere(x => x.LibraryAtt == true && x.tablesNamesId == TableNameId).ToList();
                Parallel.ForEach(DynamicAtts, DynamicAtt =>
                {
                    DynamicAtt.disable = true;
                    _unitOfWork.DynamicAttRepository.Update(DynamicAtt);
                });
                var DynamicLibAttValues = _unitOfWork.DynamicAttLibRepository.GetWhere(x => x.tablesNamesId == TableNameId && x.InventoryId == Id).ToList();
                Parallel.ForEach(DynamicLibAttValues, DynamicLibAttValue =>
                {
                    DynamicLibAttValue.disable = true;
                    _unitOfWork.DynamicAttLibRepository.Update(DynamicLibAttValue);
                });

                //Disable Installation att 
                await _unitOfWork.SaveChangesAsync();
                return new Response<CivilWithLegLibraryViewModel>(true, null, null, null, (int)Helpers.Constants.ApiReturnCode.success);
            }
            catch (Exception err)
            {
                
                return new Response<CivilWithLegLibraryViewModel>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
            

        }
    }
}
