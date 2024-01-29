using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using TLIS_DAL;
using TLIS_DAL.Helper;
using TLIS_DAL.Helper.Filters;
using TLIS_DAL.Helpers;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModelBase;
using TLIS_DAL.ViewModels.AllLoadInstDTOs;
using TLIS_DAL.ViewModels.AreaDTOs;
using TLIS_DAL.ViewModels.CabinetDTOs;
using TLIS_DAL.ViewModels.CivilNonSteelDTOs;
using TLIS_DAL.ViewModels.CivilWithLegsDTOs;
using TLIS_DAL.ViewModels.CivilWithoutLegDTOs;
using TLIS_DAL.ViewModels.DataTypeDTOs;
using TLIS_DAL.ViewModels.DynamicAttInstValueDTOs;
using TLIS_DAL.ViewModels.DynamicAttLibValueDTOs;
using TLIS_DAL.ViewModels.GeneratorDTOs;
using TLIS_DAL.ViewModels.LoadOtherDTOs;
using TLIS_DAL.ViewModels.LoadPartDTOs;
using TLIS_DAL.ViewModels.MW_BUDTOs;
using TLIS_DAL.ViewModels.MW_BULibraryDTOs;
using TLIS_DAL.ViewModels.MW_DishDTOs;
using TLIS_DAL.ViewModels.MW_DishLbraryDTOs;
using TLIS_DAL.ViewModels.MW_ODUDTOs;
using TLIS_DAL.ViewModels.Mw_OtherDTOs;
using TLIS_DAL.ViewModels.MW_PortDTOs;
using TLIS_DAL.ViewModels.MW_RFUDTOs;
using TLIS_DAL.ViewModels.PartDTOs;
using TLIS_DAL.ViewModels.PowerDTOs;
using TLIS_DAL.ViewModels.RadioAntennaDTOs;
using TLIS_DAL.ViewModels.RadioAntennaLibraryDTOs;
using TLIS_DAL.ViewModels.RadioOtherDTOs;
using TLIS_DAL.ViewModels.RadioRRUDTOs;
using TLIS_DAL.ViewModels.RadioRRULibraryDTOs;
using TLIS_DAL.ViewModels.SideArmDTOs;
using TLIS_DAL.ViewModels.SiteDTOs;
using TLIS_DAL.ViewModels.SiteStatusDTOs;
using TLIS_DAL.ViewModels.SolarDTOs;
using TLIS_DAL.ViewModels.TablesNamesDTOs;
using TLIS_Repository.Base;
using TLIS_Service.IService;
using Nancy.Json;
using Microsoft.Extensions.Logging;
using TLIS_DAL.ViewModels.RegionDTOs;
using TLIS_DAL.ViewModels.AttributeActivatedDTOs;
using TLIS_DAL.ViewModels.AttachedFilesDTOs;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Engineering;
using TLIS_DAL.ViewModels.LocationTypeDTOs;
using Microsoft.AspNetCore.Mvc;
using System.Drawing;
using AutoMapper;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;
using System.Xml;
using Microsoft.AspNetCore.Components.Forms;
using LinqToExcel.Extensions;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Text.Json;
using Newtonsoft.Json;

namespace TLIS_Service.Services
{
    public class SiteService : ISiteService
    {
        IUnitOfWork _unitOfWork;
        IServiceCollection _services;
        ApplicationDbContext _context;
        private IMapper _mapper;
        ServiceProvider _serviceProvider;
        public static List<TLIsite> _MySites;
        public SiteService(IUnitOfWork unitOfWork, IServiceCollection services, ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _unitOfWork = unitOfWork;
            _services = services;
            _mapper = mapper;
        }
        public Response<AddSiteViewModel> AddSite(AddSiteViewModel AddSiteViewModel, int TaskId)
        {
            try
            {
                // Check Site Code If It's Already Exist in DB or Not..
                TLIsite CheckSiteCode = _unitOfWork.SiteRepository
                    .GetWhereFirst(x => x.SiteCode.ToLower() == AddSiteViewModel.SiteCode.ToLower());

                if (CheckSiteCode != null)
                    return new Response<AddSiteViewModel>(true, null, null, $"This site code {AddSiteViewModel.SiteCode} is already exist",
                        (int)Helpers.Constants.ApiReturnCode.fail);

                var CheckSiteName = _context.TLIsite.Where(x => x.SiteName.ToLower() == AddSiteViewModel.SiteName.ToLower()).FirstOrDefault();

                if (CheckSiteName != null)
                    return new Response<AddSiteViewModel>(true, null, null, $"This site name {AddSiteViewModel.SiteName} is already exist",
                        (int)Helpers.Constants.ApiReturnCode.fail);

                TLIsite NewSiteEntity = _mapper.Map<TLIsite>(AddSiteViewModel);
                _unitOfWork.SiteRepository.Add(NewSiteEntity);

                _MySites.Add(NewSiteEntity);

                _unitOfWork.SaveChanges();

                return new Response<AddSiteViewModel>(true, null, null, null, (int)Helpers.Constants.ApiReturnCode.success);
            }
            catch (Exception err)
            {
                return new Response<AddSiteViewModel>(false, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
        }
        public Response<EditSiteViewModel> EditSite(EditSiteViewModel EditSiteViewModel, int TaskId)
        {
            try
            {
                var CheckSiteName = _context.TLIsite.Where(x => x.SiteName == EditSiteViewModel.SiteName && x.SiteCode != EditSiteViewModel.SiteCode).FirstOrDefault();
                if (CheckSiteName != null)
                {
                    return new Response<EditSiteViewModel>(true, null, null, $"This site name {EditSiteViewModel.SiteName} is already exist",
                        (int)Helpers.Constants.ApiReturnCode.fail);
                }
                TLIsite Site = _mapper.Map<TLIsite>(EditSiteViewModel);
                _unitOfWork.SiteRepository.Update(Site);

                _MySites.Remove(_MySites.FirstOrDefault(x => x.SiteCode.ToLower() == EditSiteViewModel.SiteCode.ToLower()));
                _MySites.Add(Site);

                _unitOfWork.SaveChanges();

                return new Response<EditSiteViewModel>(true, null, null, null, (int)Helpers.Constants.ApiReturnCode.success);
            }
            catch (Exception err)
            {
                return new Response<EditSiteViewModel>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
        }
        public Response<List<AreaViewModel>> GetAllAreasForSiteOperation()
        {
            try
            {
                List<AreaViewModel> Areas = _mapper.Map<List<AreaViewModel>>(_unitOfWork.AreaRepository.GetAllWithoutCount().ToList());
                return new Response<List<AreaViewModel>>(true, Areas, null, null, (int)Helpers.Constants.ApiReturnCode.success);
            }
            catch (Exception err)
            {
                return new Response<List<AreaViewModel>>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
        }
        public Response<List<RegionViewModel>> GetAllRegionsForSiteOperation()
        {
            try
            {
                List<RegionViewModel> Regions = _mapper.Map<List<RegionViewModel>>(_unitOfWork.RegionRepository.GetAllWithoutCount().ToList());
                return new Response<List<RegionViewModel>>(true, Regions, null, null, (int)Helpers.Constants.ApiReturnCode.success);
            }
            catch (Exception err)
            {
                return new Response<List<RegionViewModel>>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
        }
        public Response<List<SiteStatusViewModel>> GetAllSiteStatusForSiteOperation()
        {
            try
            {
                List<SiteStatusViewModel> SiteStatus = _mapper.Map<List<SiteStatusViewModel>>(_unitOfWork.SiteStatusRepository
                    .GetWhere(x => !x.Deleted && x.Active).ToList());
                return new Response<List<SiteStatusViewModel>>(true, SiteStatus, null, null, (int)Helpers.Constants.ApiReturnCode.success);
            }
            catch (Exception err)
            {
                return new Response<List<SiteStatusViewModel>>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
        }
        public Response<List<LocationTypeViewModel>> GetAllLocationTypesForSiteOperation()
        {
            try
            {
                List<LocationTypeViewModel> LocationTypes = _mapper.Map<List<LocationTypeViewModel>>(_unitOfWork.LocationTypeRepository
                    .GetWhere(x => !x.Deleted && !x.Disable).ToList());
                return new Response<List<LocationTypeViewModel>>(true, LocationTypes, null, null, (int)Helpers.Constants.ApiReturnCode.success);
            }
            catch (Exception err)
            {
                return new Response<List<LocationTypeViewModel>>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
        }
        public Response<ListSiteStatusViewModel> AddSiteStatus(AddSiteStatusViewModel SiteStatus)
        {
            try
            {
                var site = _mapper.Map<TLIsiteStatus>(SiteStatus);
                _unitOfWork.SiteStatusRepository.Add(site);
                _unitOfWork.SaveChanges();
                return new Response<ListSiteStatusViewModel>(_mapper.Map<ListSiteStatusViewModel>(site));
            }
            catch (Exception err)
            {

                return new Response<ListSiteStatusViewModel>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
        }

        public Response<List<ListSiteStatusViewModel>> GetAllSiteStatus()//ParameterPagination parameterPagination, List<FilterObjectList> filters
        {
            try
            {
                int count = 0;
                var SiteStatusModel = _mapper.Map<List<ListSiteStatusViewModel>>(_unitOfWork.SiteStatusRepository.GetAllAsQueryable(out count).Where(x => x.Deleted == false).ToList());
                return new Response<List<ListSiteStatusViewModel>>(true, SiteStatusModel, null, null, (int)Helpers.Constants.ApiReturnCode.success, count);
            }
            catch (Exception err)
            {

                return new Response<List<ListSiteStatusViewModel>>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
        }

        public Response<ListSiteStatusViewModel> GetSiteStatusbyId(int SiteStatusId)
        {
            try
            {
                var SiteStatusModel = _mapper.Map<ListSiteStatusViewModel>(_unitOfWork.SiteStatusRepository.GetAllAsQueryable().Where(s => s.Id == SiteStatusId && s.Deleted == false).FirstOrDefault());
                return new Response<ListSiteStatusViewModel>(true, SiteStatusModel, null, null, (int)Helpers.Constants.ApiReturnCode.success);
            }
            catch (Exception err)
            {

                return new Response<ListSiteStatusViewModel>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
        }

        public Response<SiteViewModel> GetSiteMainSpaces(string SiteCode)
        {
            try
            {
                var Site = _context.TLIsite.FirstOrDefault(x => x.SiteCode == SiteCode);
                var GetSite = _mapper.Map<SiteViewModel>(Site);

                return new Response<SiteViewModel>(true, GetSite, null, null, (int)Helpers.Constants.ApiReturnCode.success);
            }
            catch (Exception err)
            {
                return new Response<SiteViewModel>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
        }


        public Response<ListSiteStatusViewModel> DeleteSiteStatus(int SiteStatusId)
        {
            try
            {
                var site = _unitOfWork.SiteStatusRepository.GetAllAsQueryable().Where(s => s.Id == SiteStatusId).FirstOrDefault();
                site.Deleted = true;
                site.DateDeleted = DateTime.Now;
                _unitOfWork.SiteStatusRepository.Update(site);
                _unitOfWork.SaveChanges();
                return new Response<ListSiteStatusViewModel>(true, null, null, null, (int)Helpers.Constants.ApiReturnCode.success);
            }
            catch (Exception err)
            {

                return new Response<ListSiteStatusViewModel>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
        }

        public Response<ListSiteStatusViewModel> UpdatSiteStatus(ListSiteStatusViewModel SiteStatus)
        {
            try
            {
                var site = _mapper.Map<TLIsiteStatus>(SiteStatus);
                _unitOfWork.SiteStatusRepository.UpdateItem(site);
                _unitOfWork.SaveChanges();
                return new Response<ListSiteStatusViewModel>(_mapper.Map<ListSiteStatusViewModel>(site));
            }
            catch (Exception err)
            {

                return new Response<ListSiteStatusViewModel>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
        }
        //Function return all sites depened on parameterPagination and filters
        //public Response<IEnumerable<SiteViewModel>> GetSites(ParameterPagination parameterPagination, List<FilterObjectList> filters = null)
        //{
        //    try
        //    {
        //        DateTime Start = DateTime.Now;
        //        List<string> locationtypes = new List<string>();
        //        List<SiteViewModel> Sites = new List<SiteViewModel>();
        //        if (filters != null ? filters.Count() > 0 : false)
        //        {
        //            var site = _context.TLIsite.ToList();
        //            foreach (var item in site)
        //            {
        //                if (item != null)
        //                {
        //                    string Regions = "";
        //                    string Locations = "";
        //                    if (item.RegionCode == null)
        //                    {
        //                        Regions = "";
        //                    }
        //                    else if (item.RegionCode != null)
        //                    {
        //                        Regions = _context.TLIregion.FirstOrDefault(x => x.RegionCode == item.RegionCode).RegionName;
        //                    }
        //                    if (item.LocationType == null)
        //                    {
        //                        Locations = "";
        //                    }
        //                    else if (item.LocationType != null)
        //                    {
        //                        Locations = _context.TLIlocationType.FirstOrDefault(x => x.Id.ToString() == item.LocationType).Name;
        //                    }
        //                    Sites.Add(new SiteViewModel()
        //                    {
        //                        SiteCode = item.SiteCode,
        //                        SiteName = item.SiteName,
        //                        Status = _context.TLIsiteStatus.FirstOrDefault(x => x.Id == item.siteStatusId).Name,
        //                        LocationHieght = item.LocationHieght,
        //                        Longitude = item.Longitude,
        //                        LocationType = Locations,
        //                        Latitude = item.Latitude,
        //                        CityName = item.Zone,
        //                        Area = _context.TLIarea.FirstOrDefault(x => x.Id == item.AreaId).AreaName,
        //                        Region = Regions,
        //                        ReservedSpace = item.ReservedSpace,
        //                        RentedSpace = item.RentedSpace,

        //                    });
        //                }
        //            }
        //            foreach (FilterObjectList Filter in filters)
        //            {
        //                if (typeof(SiteViewModel).GetProperties().FirstOrDefault(x => x.Name.ToLower() == Filter.key.ToLower())
        //                    .PropertyType.Name.ToLower() == "String".ToLower())
        //                {
        //                    Sites = Sites.Where(x => Filter.value.Any(FilterValue => x.GetType().GetProperties().FirstOrDefault(x => x.Name.ToLower() == Filter.key.ToLower())
        //                        .GetValue(x, null) != null ? x.GetType().GetProperties().FirstOrDefault(x => x.Name.ToLower() == Filter.key.ToLower())
        //                            .GetValue(x, null).ToString().ToLower().StartsWith(FilterValue.ToString().ToLower()) : false)).ToList();
        //                }
        //                else
        //                {
        //                    Sites = Sites.Where(x => Filter.value.Any(FilterValue => x.GetType().GetProperties().FirstOrDefault(x => x.Name.ToLower() == Filter.key.ToLower())
        //                        .GetValue(x, null) != null ? x.GetType().GetProperties().FirstOrDefault(x => x.Name.ToLower() == Filter.key.ToLower())
        //                            .GetValue(x, null).ToString().ToLower() == FilterValue.ToString().ToLower() : false)).ToList();
        //                }
        //            }

        //            int Count = Sites.Count();

        //            Sites = Sites.Skip((parameterPagination.PageNumber - 1) * parameterPagination.PageSize).
        //                Take(parameterPagination.PageSize).ToList();

        //            DateTime End = DateTime.Now;

        //            return new Response<IEnumerable<SiteViewModel>>(true, Sites, null, null, (int)Helpers.Constants.ApiReturnCode.success, (int)(End - Start).TotalSeconds);
        //        }
        //        else
        //        {
        //            var site = _context.TLIsite.ToList();
        //            foreach (var item in site)
        //            {
        //                if (item != null)
        //                {
        //                    string Regions = "";
        //                    string Locations = "";
        //                    if (item.RegionCode == null)
        //                    {
        //                        Regions = "";
        //                    }
        //                    else if (item.RegionCode != null)
        //                    {
        //                        Regions = _context.TLIregion.FirstOrDefault(x => x.RegionCode == item.RegionCode).RegionName;
        //                    }
        //                    if (item.LocationType == null)
        //                    {
        //                        Locations = "";
        //                    }
        //                    else if (item.LocationType != null)
        //                    {
        //                        Locations = _context.TLIlocationType.FirstOrDefault(x => x.Id.ToString() == item.LocationType).Name;
        //                    }
        //                    Sites.Add(new SiteViewModel()
        //                    {
        //                        SiteCode = item.SiteCode,
        //                        SiteName = item.SiteName,
        //                        Status = _context.TLIsiteStatus.FirstOrDefault(x => x.Id == item.siteStatusId).Name,
        //                        LocationHieght = item.LocationHieght,
        //                        Longitude = item.Longitude,
        //                        LocationType = Locations,
        //                        Latitude = item.Latitude,
        //                        CityName = item.Zone,
        //                        Area = _context.TLIarea.FirstOrDefault(x => x.Id == item.AreaId).AreaName,
        //                        Region = Regions,
        //                        ReservedSpace = item.ReservedSpace,
        //                        RentedSpace = item.RentedSpace,

        //                    });
        //                }
        //            }
        //            int count = Sites.Count();
        //            Sites = Sites.Skip((parameterPagination.PageNumber - 1) * parameterPagination.PageSize).
        //                Take(parameterPagination.PageSize).ToList();

        //            DateTime End = DateTime.Now;

        //            return new Response<IEnumerable<SiteViewModel>>(true, Sites, null, null, (int)Helpers.Constants.ApiReturnCode.success, (int)(End - Start).TotalSeconds);
        //        }
        //    }
        //    catch (Exception err)
        //    {
        //        return new Response<IEnumerable<SiteViewModel>>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
        //    }

        //}
        public Response<IEnumerable<SiteViewModelForGetAll>> GetSites(ParameterPagination parameterPagination, bool? isRefresh, bool? GetItemsCountOnEachSite, List<FilterObjectList> filters = null)
        {
            string[] ErrorMessagesWhenReturning = null;

        StartAgainWithRefresh:
            try
            {
                List<TLIlocationType> Locations = _context.TLIlocationType.AsNoTracking().ToList();

                List<string> UsedSitesInLoads = _context.TLIcivilLoads.AsNoTracking()
                    .Where(x => !string.IsNullOrEmpty(x.SiteCode))
                    .Select(x => x.SiteCode.ToLower()).Distinct().ToList();

                List<string> UsedSitesInCivils = _context.TLIcivilSiteDate.AsNoTracking()
                    .Where(x => !string.IsNullOrEmpty(x.SiteCode))
                    .Select(x => x.SiteCode.ToLower()).Distinct().ToList();

                UsedSitesInCivils.AddRange(UsedSitesInLoads);

                List<string> UsedSitesInOtherInventories = _context.TLIotherInSite.AsNoTracking()
                    .Where(x => !string.IsNullOrEmpty(x.SiteCode))
                    .Select(x => x.SiteCode.ToLower()).Distinct().ToList();

                UsedSitesInOtherInventories.AddRange(UsedSitesInCivils);

                List<string> AllUsedSites = UsedSitesInOtherInventories.Distinct().ToList();

                if (filters != null ? filters.Count() > 0 : false)
                {
                    IEnumerable<SiteViewModelForGetAll> SitesViewModels;

                    if (isRefresh != null ? isRefresh.Value : false)
                    {
                        _MySites = _context.TLIsite.AsNoTracking().Include(x => x.Area).Include(x => x.Region)
                                .Include(x => x.siteStatus).ToList();

                        SitesViewModels = _mapper.Map<IEnumerable<SiteViewModelForGetAll>>(_MySites);
                    }
                    else
                    {
                        _MySites.Count();
                        SitesViewModels = _mapper.Map<IEnumerable<SiteViewModelForGetAll>>(_MySites);
                    }

                    foreach (FilterObjectList filter in filters)
                    {
                        if (filter.key != "isUsed")
                        {
                            PropertyInfo Property = typeof(SiteViewModelForGetAll).GetProperties().FirstOrDefault(x => x.Name.ToLower() == filter.key.ToLower());

                            if (Property.PropertyType == typeof(string))
                            {
                                if (Property.Name.ToLower() == "LocationType".ToLower())
                                {
                                    SitesViewModels = SitesViewModels.Where(x => Property.GetValue(x) != null ?
                                        (Locations.Select(z => z.Id.ToString()).FirstOrDefault(y => y == Property.GetValue(x).ToString()) != null ?
                                            filter.value.Select(z => z.ToString().ToLower()).Any(z => Locations.FirstOrDefault(y => y.Id.ToString() == Property.GetValue(x).ToString()).Name.ToLower()
                                                .StartsWith(z)) : false) : false);
                                }
                                else
                                {
                                    SitesViewModels = SitesViewModels.Where(x => Property.GetValue(x) != null ?
                                        filter.value.Select(z => z.ToString().ToLower()).Any(z => Property.GetValue(x).ToString().ToLower().StartsWith(z)) : false).ToList();
                                }
                            }
                            else
                            {
                                SitesViewModels = SitesViewModels.Where(x => Property.GetValue(x) != null ?
                                    filter.value.Select(z => z.ToString().ToLower()).Any(z => z == Property.GetValue(x).ToString().ToLower()) : false).ToList();
                            }
                        }

                    }

                    var UsedFilter = filters.FirstOrDefault(x => x.key == "isUsed");
                    int Count = 0;
                    if (UsedFilter != null)
                    {
                        SitesViewModels = SitesViewModels.Where(x => AllUsedSites.Any(y => y.ToLower() == x.SiteCode.ToLower()).ToString().ToLower() == UsedFilter.ToString().ToLower());
                        Count = SitesViewModels.Count();
                        SitesViewModels = SitesViewModels.Skip((parameterPagination.PageNumber - 1) * parameterPagination.PageSize)
                        .Take(parameterPagination.PageSize);
                    }
                    else
                    {
                        SitesViewModels = SitesViewModels.Skip((parameterPagination.PageNumber - 1) * parameterPagination.PageSize)
                                                .Take(parameterPagination.PageSize);
                        Count = SitesViewModels.Count();
                    }

                    List<SiteViewModelForGetAll> ListForOutPutOnly = new List<SiteViewModelForGetAll>();

                    foreach (SiteViewModelForGetAll SitesViewModel in SitesViewModels)
                    {
                        string? LocationTypeInModel = _MySites.FirstOrDefault(x => x.SiteCode.ToLower() == SitesViewModel.SiteCode.ToLower())
                            .LocationType;

                        if (!string.IsNullOrEmpty(LocationTypeInModel))
                        {
                            TLIlocationType? CheckLocation = Locations.FirstOrDefault(x => x.Id.ToString() == LocationTypeInModel);

                            ListForOutPutOnly.Add(new SiteViewModelForGetAll()
                            {
                                SiteCode = SitesViewModel.SiteCode,
                                LocationType = CheckLocation != null ? CheckLocation.Name : "NA",
                                SiteName = SitesViewModel.SiteName,
                                Area = SitesViewModel.Area,
                                CityName = SitesViewModel.CityName,
                                Latitude = SitesViewModel.Latitude,
                                LocationHieght = SitesViewModel.LocationHieght,
                                Longitude = SitesViewModel.Longitude,
                                Region = SitesViewModel.Region,
                                RentedSpace = SitesViewModel.RentedSpace,
                                ReservedSpace = SitesViewModel.ReservedSpace,
                                Status = SitesViewModel.Status,
                                isUsed = AllUsedSites.Any(x => x.ToLower() == SitesViewModel.SiteCode.ToLower()),
                                ItemsOnSite = GetItemsCountOnEachSite != null ?
                                    (GetItemsCountOnEachSite.Value ? GetItemsOnSite(SitesViewModel.SiteCode).Data : null) : null
                            });
                        }
                        else
                        {
                            ListForOutPutOnly.Add(new SiteViewModelForGetAll()
                            {
                                SiteCode = SitesViewModel.SiteCode,
                                LocationType = "NA",
                                SiteName = SitesViewModel.SiteName,
                                Area = SitesViewModel.Area,
                                CityName = SitesViewModel.CityName,
                                Latitude = SitesViewModel.Latitude,
                                LocationHieght = SitesViewModel.LocationHieght,
                                Longitude = SitesViewModel.Longitude,
                                Region = SitesViewModel.Region,
                                RentedSpace = SitesViewModel.RentedSpace,
                                ReservedSpace = SitesViewModel.ReservedSpace,
                                Status = SitesViewModel.Status,
                                isUsed = AllUsedSites.Any(x => x.ToLower() == SitesViewModel.SiteCode.ToLower()),
                                ItemsOnSite = GetItemsCountOnEachSite != null ?
                                    (GetItemsCountOnEachSite.Value ? GetItemsOnSite(SitesViewModel.SiteCode).Data : null) : null
                            });
                        }
                    }

                    return new Response<IEnumerable<SiteViewModelForGetAll>>(true, ListForOutPutOnly, ErrorMessagesWhenReturning, null, (int)Helpers.Constants.ApiReturnCode.success, Count);
                }
                else
                {
                    IEnumerable<SiteViewModelForGetAll> SitesViewModels;

                    if (isRefresh != null ? isRefresh.Value : false)
                    {
                        _MySites = _context.TLIsite.AsNoTracking().Include(x => x.Area).Include(x => x.Region)
                                .Include(x => x.siteStatus).ToList();

                        SitesViewModels = _mapper.Map<IEnumerable<SiteViewModelForGetAll>>(_MySites);
                    }
                    else
                    {
                        _MySites.Count();
                        SitesViewModels = _mapper.Map<IEnumerable<SiteViewModelForGetAll>>(_MySites);
                    }



                    int Count = SitesViewModels.Count();
                    SitesViewModels = _mapper.Map<IEnumerable<SiteViewModelForGetAll>>(_MySites.Skip((parameterPagination.PageNumber - 1) * parameterPagination.PageSize)
                                                .Take(parameterPagination.PageSize));





                    List<SiteViewModelForGetAll> ListForOutPutOnly = new List<SiteViewModelForGetAll>();

                    foreach (SiteViewModelForGetAll SitesViewModel in SitesViewModels)
                    {
                        string? LocationTypeInModel = _MySites.FirstOrDefault(x => x.SiteCode.ToLower() == SitesViewModel.SiteCode.ToLower())
                            .LocationType;

                        if (!string.IsNullOrEmpty(LocationTypeInModel))
                        {
                            TLIlocationType? CheckLocation = Locations.FirstOrDefault(x => x.Id.ToString() == LocationTypeInModel);

                            ListForOutPutOnly.Add(new SiteViewModelForGetAll()
                            {
                                SiteCode = SitesViewModel.SiteCode,
                                LocationType = CheckLocation != null ? CheckLocation.Name : "NA",
                                SiteName = SitesViewModel.SiteName,
                                Area = SitesViewModel.Area,
                                CityName = SitesViewModel.CityName,
                                Latitude = SitesViewModel.Latitude,
                                LocationHieght = SitesViewModel.LocationHieght,
                                Longitude = SitesViewModel.Longitude,
                                Region = SitesViewModel.Region,
                                RentedSpace = SitesViewModel.RentedSpace,
                                ReservedSpace = SitesViewModel.ReservedSpace,
                                Status = SitesViewModel.Status,
                                isUsed = AllUsedSites.Any(x => x.ToLower() == SitesViewModel.SiteCode.ToLower()),
                                ItemsOnSite = GetItemsCountOnEachSite != null ?
                                    (GetItemsCountOnEachSite.Value ? GetItemsOnSite(SitesViewModel.SiteCode).Data : null) : null
                            });
                        }
                        else
                        {
                            ListForOutPutOnly.Add(new SiteViewModelForGetAll()
                            {
                                SiteCode = SitesViewModel.SiteCode,
                                LocationType = "NA",
                                SiteName = SitesViewModel.SiteName,
                                Area = SitesViewModel.Area,
                                CityName = SitesViewModel.CityName,
                                Latitude = SitesViewModel.Latitude,
                                LocationHieght = SitesViewModel.LocationHieght,
                                Longitude = SitesViewModel.Longitude,
                                Region = SitesViewModel.Region,
                                RentedSpace = SitesViewModel.RentedSpace,
                                ReservedSpace = SitesViewModel.ReservedSpace,
                                Status = SitesViewModel.Status,
                                isUsed = AllUsedSites.Any(x => x.ToLower() == SitesViewModel.SiteCode.ToLower()),
                                ItemsOnSite = GetItemsCountOnEachSite != null ?
                                    (GetItemsCountOnEachSite.Value ? GetItemsOnSite(SitesViewModel.SiteCode).Data : null) : null
                            });
                        }
                    }

                    return new Response<IEnumerable<SiteViewModelForGetAll>>(true, ListForOutPutOnly, ErrorMessagesWhenReturning, null, (int)Helpers.Constants.ApiReturnCode.success, Count);
                }
            }
            catch (Exception)
            {
                isRefresh = true;
                if (ErrorMessagesWhenReturning == null)
                {
                    ErrorMessagesWhenReturning = new string[]
                    {
                        "After Caching"
                    };
                    goto StartAgainWithRefresh;
                }

                return new Response<IEnumerable<SiteViewModelForGetAll>>(true, null, ErrorMessagesWhenReturning, null, (int)Helpers.Constants.ApiReturnCode.success, _MySites.Count());
            }
        }
        public List<SiteViewModel> GetAllSitesWithoutPaginationForWorkFlow()
        {
            return _mapper.Map<List<SiteViewModel>>(_unitOfWork.SiteRepository
                .GetIncludeWhere(x => true, x => x.Area, x => x.Region, x => x.siteStatus).ToList());
        }
        //Function return space details
        public Response<List<KeyValuePair<string, float>>> GetSpaceDetails(string SiteCode)
        {
            try
            {
                var Site = _unitOfWork.SiteRepository.GetAllAsQueryable().Where(s => s.SiteCode == SiteCode).FirstOrDefault();
                List<KeyValuePair<string, float>> SpaceDetails = new List<KeyValuePair<string, float>>();
                SpaceDetails.Add(new KeyValuePair<string, float>("RentedSpace", Site.RentedSpace));
                SpaceDetails.Add(new KeyValuePair<string, float>("ReservedSpace", Site.ReservedSpace));
                return new Response<List<KeyValuePair<string, float>>>(true, SpaceDetails, null, null, (int)Helpers.Constants.ApiReturnCode.success);
            }
            catch (Exception err)
            {

                return new Response<List<KeyValuePair<string, float>>>(false, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }


        }

        //Function update rented space
        public async Task<Response<SiteViewModel>> UpdateRentedSpace(string SiteCode, float RentedSpaceValue, int installationSpace)
        {
            try
            {
                var Site = _unitOfWork.SiteRepository.GetAllAsQueryable().Where(s => s.SiteCode == SiteCode).FirstOrDefault();
                Site.RentedSpace = RentedSpaceValue;

                _MySites.FirstOrDefault(x => x.SiteCode.ToLower() == SiteCode.ToLower()).RentedSpace = RentedSpaceValue;

                await _unitOfWork.SiteRepository.UpdateItem(Site);
                await _unitOfWork.SaveChangesAsync();
                return new Response<SiteViewModel>();
            }
            catch (Exception err)
            {

                return new Response<SiteViewModel>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.success);
            }

        }

        //Function check rented space if there is space update rented space
        public bool CheckRentedSpace(string SiteCode, float InstallationSpaceValue)
        {
            try
            {
                var Site = _unitOfWork.SiteRepository.GetAllAsQueryable().Where(s => s.SiteCode == SiteCode).FirstOrDefault();
                if ((Site.ReservedSpace + InstallationSpaceValue) <= Site.RentedSpace)
                {
                    //Site.RentedSpace += InstallationSpaceValue;
                    //await _unitOfWork.SiteRepository.UpdateItem(Site);
                    //await _unitOfWork.SaveChangesAsync();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception err)
            {

                return false;
            }
        }
        public Response<SiteCivilsViewModel> GetCivilsWithAllCivilInstIdsBySiteCode(string SiteCode, string CivilType)
        {
            try
            {
                if (CivilType.ToLower() == Helpers.Constants.TablesNames.TLIcivilWithLegs.ToString().ToLower())
                {
                    List<TLIallCivilInst> AllCivilInsts = _unitOfWork.CivilSiteDateRepository.GetIncludeWhere(x =>
                       !x.Dismantle && x.SiteCode == SiteCode && x.allCivilInst.civilWithLegsId != null &&
                       !string.IsNullOrEmpty(x.allCivilInst.civilWithLegs.Name),
                        x => x.allCivilInst, x => x.allCivilInst.civilWithLegs).AsQueryable().AsNoTracking().Select(x => x.allCivilInst).ToList();

                    List<CivilWithLegsViewModel> CivilWithLegs = _mapper.Map<List<CivilWithLegsViewModel>>(AllCivilInsts.Where(x => x.civilWithLegsId != null).Select(x => x.civilWithLegs).ToList());

                    foreach (CivilWithLegsViewModel CivilWithLeg in CivilWithLegs)
                    {
                        int AllCivilInstId = AllCivilInsts.FirstOrDefault(x => x.civilWithLegsId != null ? x.civilWithLegsId == CivilWithLeg.Id : false).Id;
                        CivilWithLeg.Id = AllCivilInstId;
                    }

                    return new Response<SiteCivilsViewModel>(true, new SiteCivilsViewModel
                    {
                        CivilWithLegs = CivilWithLegs
                    }, null, null, (int)Helpers.Constants.ApiReturnCode.success, CivilWithLegs.Count());
                }
                else if (CivilType.ToLower() == Helpers.Constants.TablesNames.TLIcivilWithoutLeg.ToString().ToLower())
                {
                    List<TLIallCivilInst> AllCivilInsts = _unitOfWork.CivilSiteDateRepository.GetIncludeWhere(x =>
                       !x.Dismantle && x.SiteCode == SiteCode && x.allCivilInst.civilWithoutLegId != null &&
                       !string.IsNullOrEmpty(x.allCivilInst.civilWithoutLeg.Name),
                        x => x.allCivilInst, x => x.allCivilInst.civilWithoutLeg, x => x.allCivilInst.civilWithoutLeg.CivilWithoutlegsLib).AsQueryable().AsNoTracking().Select(x => x.allCivilInst).ToList();

                    List<CivilWithoutLegViewModel> CivilWithoutLegs = _mapper.Map<List<CivilWithoutLegViewModel>>(AllCivilInsts.Where(x => x.civilWithoutLegId != null).Select(x => x.civilWithoutLeg).ToList());

                    foreach (CivilWithoutLegViewModel CivilWithoutLeg in CivilWithoutLegs)
                    {
                        int AllCivilInstId = AllCivilInsts.FirstOrDefault(x => x.civilWithoutLegId != null ? x.civilWithoutLegId == CivilWithoutLeg.Id : false).Id;
                        CivilWithoutLeg.Id = AllCivilInstId;
                    }

                    return new Response<SiteCivilsViewModel>(true, new SiteCivilsViewModel
                    {
                        civilWithoutLegs = CivilWithoutLegs
                    }, null, null, (int)Helpers.Constants.ApiReturnCode.success, CivilWithoutLegs.Count());
                }
                else if (CivilType.ToLower() == Helpers.Constants.TablesNames.TLIcivilNonSteel.ToString().ToLower())
                {
                    List<TLIallCivilInst> AllCivilInsts = _unitOfWork.CivilSiteDateRepository.GetIncludeWhere(x =>
                       !x.Dismantle && x.SiteCode == SiteCode && x.allCivilInst.civilNonSteelId != null &&
                       !string.IsNullOrEmpty(x.allCivilInst.civilNonSteel.Name),
                        x => x.allCivilInst, x => x.allCivilInst.civilNonSteel).AsQueryable().AsNoTracking().Select(x => x.allCivilInst).ToList();

                    List<CivilNonSteelViewModel> CivilNonSteels = _mapper.Map<List<CivilNonSteelViewModel>>(AllCivilInsts.Where(x => x.civilNonSteelId != null).Select(x => x.civilNonSteel).ToList());

                    foreach (CivilNonSteelViewModel CivilNonSteel in CivilNonSteels)
                    {
                        int AllCivilInstId = AllCivilInsts.FirstOrDefault(x => x.civilNonSteelId != null ? x.civilNonSteelId == CivilNonSteel.Id : false).Id;
                        CivilNonSteel.Id = AllCivilInstId;
                    }

                    return new Response<SiteCivilsViewModel>(true, new SiteCivilsViewModel
                    {
                        civilNonSteels = CivilNonSteels
                    }, null, null, (int)Helpers.Constants.ApiReturnCode.success, CivilNonSteels.Count());
                }

                return null;
            }
            catch (Exception err)
            {
                return new Response<SiteCivilsViewModel>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
        }
        //Function return all civils by siteCode
        public Response<SiteCivilsViewModel> GetCivilsBySiteCode(string SiteCode)
        {
            try
            {
                SiteCivilsViewModel siteCivils = new SiteCivilsViewModel();
                var SiteCivils = _unitOfWork.CivilSiteDateRepository.GetAllAsQueryable().Where(x => x.SiteCode == SiteCode).Select(x => x.allCivilInstId).ToList();
                var CivilInst = _unitOfWork.AllCivilInstRepository.GetAllAsQueryable().Where(x => SiteCivils.Contains(x.Id)).ToList();
                var CivilWithLegs = CivilInst.Where(x => x.civilWithLegsId != null && x.civilWithoutLegId == null && x.civilNonSteelId == null).Select(x => x.civilWithLegsId).ToList();
                var CivilWithoutLegs = CivilInst.Where(x => x.civilWithLegsId == null && x.civilWithoutLegId != null && x.civilNonSteelId == null).Select(x => x.civilWithoutLegId).ToList();
                var CivilNonSteel = CivilInst.Where(x => x.civilWithLegsId == null && x.civilWithoutLegId == null && x.civilNonSteelId != null).Select(x => x.civilNonSteelId).ToList();
                siteCivils.CivilWithLegs = _mapper.Map<List<CivilWithLegsViewModel>>(_unitOfWork.CivilWithLegsRepository.GetAllAsQueryable().Where(x => CivilWithLegs.Contains(x.Id)).ToList());
                siteCivils.civilWithoutLegs = _mapper.Map<List<CivilWithoutLegViewModel>>(_unitOfWork.CivilWithoutLegRepository.GetAllAsQueryable().Where(x => CivilWithoutLegs.Contains(x.Id)).ToList());
                siteCivils.civilNonSteels = _mapper.Map<List<CivilNonSteelViewModel>>(_unitOfWork.CivilNonSteelRepository.GetAllAsQueryable().Where(x => CivilNonSteel.Contains(x.Id)).ToList());
                return new Response<SiteCivilsViewModel>(true, siteCivils, null, null, (int)Helpers.Constants.ApiReturnCode.success);
            }
            catch (Exception err)
            {
                return new Response<SiteCivilsViewModel>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
        }

        public Response<SiteViewModel> GetSitebyId(string SiteCode)
        {
            try
            {
                SiteViewModel siteViewModel = new SiteViewModel();
                var siteInfo = _context.TLIsite.Include(x => x.Area).Include(x => x.Region).Include(x => x.siteStatus).
                    Where(x => x.SiteCode == SiteCode).FirstOrDefault();

                siteViewModel = new SiteViewModel()
                {
                    SiteCode = siteInfo.SiteCode,
                    SiteName = siteInfo.SiteName,
                    Status = _context.TLIsiteStatus.FirstOrDefault(x => x.Id == siteInfo.siteStatusId).Name,
                    LocationHieght = siteInfo.LocationHieght,
                    Longitude = siteInfo.Longitude,
                    LocationType = _context.TLIlocationType.FirstOrDefault(x => x.Id == Convert.ToInt64(siteInfo.LocationType)).Name,
                    Latitude = siteInfo.Latitude,
                    CityName = siteInfo.Zone,
                    Area = _context.TLIarea.FirstOrDefault(x => x.Id == siteInfo.AreaId).AreaName,
                    Region = _context.TLIregion.FirstOrDefault(x => x.RegionCode == siteInfo.RegionCode).RegionName,
                    ReservedSpace = siteInfo.ReservedSpace,
                    RentedSpace = siteInfo.RentedSpace,

                };
                if (siteInfo.SiteName == null)
                {
                    siteInfo.SiteName = "";
                }
                return new Response<SiteViewModel>(true, siteViewModel, null, null, (int)Helpers.Constants.ApiReturnCode.success);
            }
            catch (Exception err)
            {

                return new Response<SiteViewModel>(false, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
        }
        public async Task<Response<bool>> EditSitesMainSpaces(float RentedSpace, float ReservedSpace, string SiteCode)
        {
            try
            {
                TLIsite site = _unitOfWork.SiteRepository.GetWhereFirst(x => x.SiteCode == SiteCode);
                site.RentedSpace = RentedSpace;
                site.ReservedSpace = ReservedSpace;

                TLIsite OldSiteData = _MySites.FirstOrDefault(x => x.SiteCode.ToLower() == SiteCode.ToLower());
                _MySites.Remove(OldSiteData);
                _MySites.Add(site);

                _unitOfWork.SiteRepository.Update(site);
                _unitOfWork.SaveChanges();
                return new Response<bool>(true, true, null, null, (int)Helpers.Constants.ApiReturnCode.success);

            }
            catch (Exception err)
            {

                return new Response<bool>(false, false, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
        }
        public ImageResponse GetSitePhotosSlideshow(string SiteCode)
        {
            try
            {
                ImageResponse result = new ImageResponse();

                var AttachedFiles = _unitOfWork.AttachedFilesRepository.GetIncludeWhere(x => x.SiteCode == SiteCode && x.IsImg == true
                    , x => x.documenttype, x => x.tablesName).ToList();
                var Asstes = AttachedFiles.Select(x => new SiteAssets()
                {
                    previewImageSrc = x.Path,
                    thumbnailImageSrc = x.Path,
                    alt = x.Name,
                    tiltle = x.Name

                }).ToList();

                result.data = Asstes;
                return result;
                //var SitePhotosSlideshow = _mapper.Map<List<SitePhotosSlideshowViewModel>>(AttachedFiles);
                //  return new Response<List<SitePhotosSlideshowViewModel>>(true, SitePhotosSlideshow, null, null, (int)Helpers.Constants.ApiReturnCode.success, SitePhotosSlideshow.Count);
            }
            catch (Exception err)
            {
                //return new Response<List<SitePhotosSlideshowViewModel>> (true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);

                return null;
            }
        }

        //Function return sideArms related to specific site
        public Response<List<SideArmViewModel>> GetSideArmsBySiteCode(string SiteCode)
        {
            try
            {
                var SideArmsIds = _unitOfWork.CivilLoadsRepository.GetAllAsQueryable().Where(x => x.SiteCode == SiteCode).Select(x => x.sideArmId).ToList();
                var SideArms = _mapper.Map<List<SideArmViewModel>>(_unitOfWork.SideArmRepository.GetAllAsQueryable().Where(x => SideArmsIds.Contains(x.Id)));
                return new Response<List<SideArmViewModel>>(true, SideArms, null, null, (int)Helpers.Constants.ApiReturnCode.success);
            }
            catch (Exception err)
            {

                return new Response<List<SideArmViewModel>>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
        }

        //Function return other inventories by siteCode
        public Response<SiteOtherInventoriesViewModel> GetOtherInventoriesBySiteCode(string SiteCode)
        {
            try
            {
                SiteOtherInventoriesViewModel siteOtherInventories = new SiteOtherInventoriesViewModel();
                var AllOtherInventoriesInst = _unitOfWork.OtherInSiteRepository.GetAllAsQueryable().Where(x => x.SiteCode == SiteCode).Select(x => x.allOtherInventoryInstId).ToList();
                var OtherInventories = _unitOfWork.AllOtherInventoryInstRepository.GetAllAsQueryable().Where(x => AllOtherInventoriesInst.Contains(x.Id)).ToList();
                var Cabinets = OtherInventories.Where(x => x.cabinetId != null && x.generatorId == null && x.solarId == null).Select(x => x.cabinetId).ToList();
                var Generators = OtherInventories.Where(x => x.cabinetId == null && x.generatorId != null && x.solarId == null).Select(x => x.generatorId).ToList();
                var Solars = OtherInventories.Where(x => x.cabinetId == null && x.generatorId == null && x.solarId != null).Select(x => x.solarId).ToList();
                siteOtherInventories.cabinets = _mapper.Map<List<CabinetViewModel>>(_unitOfWork.CabinetRepository.GetAllAsQueryable().Where(x => Cabinets.Contains(x.Id)).ToList());
                siteOtherInventories.Generators = _mapper.Map<List<GeneratorViewModel>>(_unitOfWork.GeneratorRepository.GetAllAsQueryable().Where(x => Generators.Contains(x.Id)).ToList());
                siteOtherInventories.Solars = _mapper.Map<List<SolarViewModel>>(_unitOfWork.SolarRepository.GetIncludeWhere(x => Solars.Contains(x.Id), x => x.SolarLibrary).ToList());
                return new Response<SiteOtherInventoriesViewModel>(true, siteOtherInventories, null, null, (int)Helpers.Constants.ApiReturnCode.success);
            }
            catch (Exception err)
            {

                return new Response<SiteOtherInventoriesViewModel>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
        }

        //Function return all load by siteCode
        public Response<SiteLoadsViewModel> GetLoadsBySiteCode(string SiteCode)
        {
            try
            {
                SiteLoadsViewModel siteLoads = new SiteLoadsViewModel();
                var LoadsIds = _unitOfWork.CivilLoadsRepository.GetAllAsQueryable().Where(x => x.SiteCode == SiteCode).Select(x => x.allLoadInstId).ToList();
                var AllLoadsInst = _unitOfWork.AllLoadInstRepository.GetAllAsQueryable().Where(x => LoadsIds.Contains(x.Id)).ToList();
                var mwBUs = AllLoadsInst.Where(x => x.mwBUId != null && x.mwDishId == null && x.mwODUId == null && x.mwRFUId == null && x.mwOtherId == null && x.radioAntennaId == null && x.radioRRUId == null && x.radioOtherId == null && x.powerId == null && x.loadOtherId == null).Select(x => x.mwBUId).ToList();
                var mwDishs = AllLoadsInst.Where(x => x.mwBUId == null && x.mwDishId != null && x.mwODUId == null && x.mwRFUId == null && x.mwOtherId == null && x.radioAntennaId == null && x.radioRRUId == null && x.radioOtherId == null && x.powerId == null && x.loadOtherId == null).Select(x => x.mwDishId).ToList();
                var mwODUs = AllLoadsInst.Where(x => x.mwBUId == null && x.mwDishId == null && x.mwODUId != null && x.mwRFUId == null && x.mwOtherId == null && x.radioAntennaId == null && x.radioRRUId == null && x.radioOtherId == null && x.powerId == null && x.loadOtherId == null).Select(x => x.mwODUId).ToList();
                var mwRFUs = AllLoadsInst.Where(x => x.mwBUId == null && x.mwDishId == null && x.mwODUId == null && x.mwRFUId != null && x.mwOtherId == null && x.radioAntennaId == null && x.radioRRUId == null && x.radioOtherId == null && x.powerId == null && x.loadOtherId == null).Select(x => x.mwRFUId).ToList();
                var mwOthers = AllLoadsInst.Where(x => x.mwBUId == null && x.mwDishId == null && x.mwODUId == null && x.mwRFUId == null && x.mwOtherId != null && x.radioAntennaId == null && x.radioRRUId == null && x.radioOtherId == null && x.powerId == null && x.loadOtherId == null).Select(x => x.mwOtherId).ToList();
                var radioAntennas = AllLoadsInst.Where(x => x.mwBUId == null && x.mwDishId == null && x.mwODUId == null && x.mwRFUId == null && x.mwOtherId == null && x.radioAntennaId != null && x.radioRRUId == null && x.radioOtherId == null && x.powerId == null && x.loadOtherId == null).Select(x => x.radioAntennaId).ToList();
                var radioRRUs = AllLoadsInst.Where(x => x.mwBUId == null && x.mwDishId == null && x.mwODUId == null && x.mwRFUId == null && x.mwOtherId == null && x.radioAntennaId == null && x.radioRRUId != null && x.radioOtherId == null && x.powerId == null && x.loadOtherId == null).Select(x => x.radioRRUId).ToList();
                var radioOthers = AllLoadsInst.Where(x => x.mwBUId == null && x.mwDishId == null && x.mwODUId == null && x.mwRFUId == null && x.mwOtherId == null && x.radioAntennaId == null && x.radioRRUId == null && x.radioOtherId != null && x.powerId == null && x.loadOtherId == null).Select(x => x.radioOtherId).ToList();
                var powers = AllLoadsInst.Where(x => x.mwBUId == null && x.mwDishId == null && x.mwODUId == null && x.mwRFUId == null && x.mwOtherId == null && x.radioAntennaId == null && x.radioRRUId == null && x.radioOtherId == null && x.powerId != null && x.loadOtherId == null).Select(x => x.powerId).ToList();
                var loadOthers = AllLoadsInst.Where(x => x.mwBUId == null && x.mwDishId == null && x.mwODUId == null && x.mwRFUId == null && x.mwOtherId == null && x.radioAntennaId == null && x.radioRRUId == null && x.radioOtherId == null && x.powerId == null && x.loadOtherId != null).Select(x => x.loadOtherId).ToList();
                siteLoads.MW_BUs = _mapper.Map<List<MW_BUViewModel>>(_unitOfWork.MW_BURepository.GetAllAsQueryable().Where(x => mwBUs.Contains(x.Id)).ToList());
                siteLoads.MW_Dishes = _mapper.Map<List<MW_DishViewModel>>(_unitOfWork.MW_DishRepository.GetAllAsQueryable().Where(x => mwDishs.Contains(x.Id)).ToList());
                siteLoads.MW_ODUs = _mapper.Map<List<MW_ODUViewModel>>(_unitOfWork.MW_ODURepository.GetAllAsQueryable().Where(x => mwODUs.Contains(x.Id)).ToList());
                siteLoads.MW_RFUs = _mapper.Map<List<MW_RFUViewModel>>(_unitOfWork.MW_RFURepository.GetAllAsQueryable().Where(x => mwRFUs.Contains(x.Id)).ToList());
                siteLoads.MW_Others = _mapper.Map<List<Mw_OtherViewModel>>(_unitOfWork.Mw_OtherRepository.GetAllAsQueryable().Where(x => mwOthers.Contains(x.Id)).ToList());
                siteLoads.radioAntennas = _mapper.Map<List<RadioAntennaViewModel>>(_unitOfWork.RadioAntennaRepository.GetIncludeWhere(x => radioAntennas.Contains(x.Id), x => x.radioAntennaLibrary).ToList());
                siteLoads.radioRRUs = _mapper.Map<List<RadioRRUViewModel>>(_unitOfWork.RadioRRURepository.GetAllAsQueryable().Where(x => radioRRUs.Contains(x.Id)).ToList());
                siteLoads.radioOthers = _mapper.Map<List<RadioOtherViewModel>>(_unitOfWork.RadioOtherRepository.GetAllAsQueryable().Where(x => radioOthers.Contains(x.Id)).ToList());
                siteLoads.powers = _mapper.Map<List<PowerViewModel>>(_unitOfWork.PowerRepository.GetAllAsQueryable().Where(x => powers.Contains(x.Id)).ToList());
                siteLoads.loadOthers = _mapper.Map<List<LoadOtherViewModel>>(_unitOfWork.LoadOtherRepository.GetAllAsQueryable().Where(x => loadOthers.Contains(x.Id)).ToList());
                return new Response<SiteLoadsViewModel>(true, siteLoads, null, null, (int)Helpers.Constants.ApiReturnCode.success);
            }
            catch (Exception err)
            {
                return new Response<SiteLoadsViewModel>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
        }

        public Response<List<KeyValuePair<string, int>>> GetSteelCivil(string SiteCode)
        {
            try
            {
                var CivilsInSite = _unitOfWork.CivilSiteDateRepository.GetAllAsQueryable().Where(x => x.SiteCode == SiteCode && (x.allCivilInst.civilWithLegsId != null || x.allCivilInst.civilWithoutLegId != null)).Select(x => new KeyValuePair<string, int>(x.allCivilInst.civilWithLegsId != null ? x.allCivilInst.civilWithLegs.Name : x.allCivilInst.civilWithoutLeg.Name, x.allCivilInstId)).ToList();
                return new Response<List<KeyValuePair<string, int>>>(true, CivilsInSite, null, null, (int)Helpers.Constants.ApiReturnCode.success);
            }
            catch (Exception err)
            {

                return new Response<List<KeyValuePair<string, int>>>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
        }

        public Response<List<KeyValuePair<string, int>>> GetNonSteel(string SiteCode)
        {
            try
            {
                var NonSteelInSite = _unitOfWork.CivilSiteDateRepository.GetAllAsQueryable().Where(x => x.allCivilInst.civilNonSteelId != null && x.SiteCode == SiteCode).Select(x => new KeyValuePair<string, int>(x.allCivilInst.civilNonSteel.Name, x.allCivilInstId)).ToList();
                return new Response<List<KeyValuePair<string, int>>>(true, NonSteelInSite, null, null, (int)Helpers.Constants.ApiReturnCode.success);
            }
            catch (Exception err)
            {

                return new Response<List<KeyValuePair<string, int>>>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
        }



        public void test()
        {
            using (var con = new OracleConnection("User Id=TLIUSER;Password=TLIUSER;Data Source=192.168.1.50:1521/orcl.IDS.COM"))
            {
                //con.Open();
                //using (var tran = con.BeginTransaction())
                //{
                con.Open();
                OracleCommand OracleCommand = con.CreateCommand();
                OracleTransaction OracleTransaction;

                // Start a transaction.
                OracleTransaction = con.BeginTransaction();
                OracleCommand.Connection = con;
                OracleCommand.Transaction = OracleTransaction;

                try
                {
                    OracleCommand.CommandText = "Insert into \"TLIactor\"(\"Name\") VALUES('Test_1') RETURNING \"Id\" INTO :id";
                    OracleCommand.Parameters.Add(new OracleParameter
                    {
                        ParameterName = ":id",
                        OracleDbType = OracleDbType.Int16,
                        Direction = System.Data.ParameterDirection.Output
                    });
                    OracleCommand.ExecuteScalar();
                    var ActorId = OracleCommand.Parameters[":id"].Value;
                    //var ActorId = _unitOfWork.ActorRepository.GetAllAsQueryable().OrderBy(x => x.Id).LastOrDefault().Id;
                    OracleCommand.CommandText = $"Insert into \"TLIgroup\"(\"Name\",\"GroupType\",\"ActorId\") VALUES('Test_1', {1}, {ActorId})";
                    OracleCommand.ExecuteNonQuery();

                    // Attempt to commit the transaction.
                    OracleTransaction.Commit();
                    //Console.WriteLine("Both employees have been inserted in the database.");
                }
                catch (Exception ex)
                {
                    // Log the exception
                    try
                    {
                        //Rollback the transaction 
                        OracleTransaction.Rollback();
                    }
                    catch (Exception exception)
                    {
                        // This catch block will handle any errors that may have occurred	 
                        // on the server that would cause the rollback to fail.			 
                    }
                }
                //}
            }
        }

        public Response<LoadsViewModel> GetLoadsOnSite(LoadsOnSiteFilter BaseFilter, bool WithFilterData)
        {
            try
            {
                ReturnWithFilters<MW_DishLibInst> MW_DishesLibInstList = new ReturnWithFilters<MW_DishLibInst>();
                MW_DishLibInst MW_DishesLibInst = new MW_DishLibInst();

                ReturnWithFilters<MW_BULibInst> MW_BUsLibInstList = new ReturnWithFilters<MW_BULibInst>();
                MW_BULibInst MW_BUsLibInst = new MW_BULibInst();

                ReturnWithFilters<MW_ODULibInst> MW_ODUsLibInstList = new ReturnWithFilters<MW_ODULibInst>();
                MW_ODULibInst MW_ODUsLibInst = new MW_ODULibInst();

                ReturnWithFilters<MW_RFULibInst> MW_RFUsLibInstList = new ReturnWithFilters<MW_RFULibInst>();
                MW_RFULibInst MW_RFUsLibInst = new MW_RFULibInst();

                ReturnWithFilters<RadioAntennaLibInst> RadioAntennasLibInstList = new ReturnWithFilters<RadioAntennaLibInst>();
                RadioAntennaLibInst RadioAntennasLibInst = new RadioAntennaLibInst();

                ReturnWithFilters<RadioRRULibInst> RadioRRUSLibInstList = new ReturnWithFilters<RadioRRULibInst>();
                RadioRRULibInst RadioRRUSLibInst = new RadioRRULibInst();

                ReturnWithFilters<PowerLibInst> PowersLibInstList = new ReturnWithFilters<PowerLibInst>();
                PowerLibInst PowersLibInst = new PowerLibInst();

                TablesNamesViewModel TabelName = new TablesNamesViewModel();
                ListPartViewModel PartName = new ListPartViewModel();


                if (BaseFilter.TablesNameId != null)
                    TabelName = _unitOfWork.TablesNamesRepository.Get(BaseFilter.TablesNameId.Value);
                if (BaseFilter.PartId != null)
                    PartName = _unitOfWork.PartRepository.Get(BaseFilter.PartId.Value);

                // Get Civil Loads After Use The Filters From BaseFilter Object...
                List<TLIcivilLoads> CivilLoads = _unitOfWork.CivilLoadsRepository.GetIncludeWhere(x =>
                (x.SiteCode == BaseFilter.SiteCode) &&
                (BaseFilter.ItemStatusId != null ? (
                    x.allLoadInst != null ? (
                        x.allLoadInst.ItemStatusId != null ?
                            (x.allLoadInst.ItemStatusId == BaseFilter.ItemStatusId)
                        : true)
                    : true)
                : true) &&
                (BaseFilter.TicketId != null ? (
                    x.allLoadInst != null ? (
                        x.allLoadInst.TicketId != null ?
                            (x.allLoadInst.TicketId == BaseFilter.TicketId)
                        : true)
                    : true)
                : true) &&
                (BaseFilter.SideArmId != null ? (
                    x.sideArmId != null ?
                        (x.sideArmId == BaseFilter.SideArmId)
                    : true)
                : true) &&
                (BaseFilter.AllCivilId != null ?
                    (x.allCivilInstId == BaseFilter.AllCivilId)
                : true) &&
                (
                    (BaseFilter.TablesNameId != null ?
                        (TabelName.TableName == "TLImwRFU" ?
                            (x.allLoadInst.mwRFUId != null)
                        : true)
                    : true) ||
                    (BaseFilter.TablesNameId != null ?
                        (TabelName.TableName == "TLImwODU" ?
                            (x.allLoadInst.mwODUId != null)
                        : true)
                    : true) ||
                    (BaseFilter.TablesNameId != null ?
                        (TabelName.TableName == "TLImwDish" ?
                            (x.allLoadInst.mwDishId != null)
                        : true)
                    : true) ||
                    (BaseFilter.TablesNameId != null ?
                        (TabelName.TableName == "TLImwBU" ?
                            (x.allLoadInst.mwBUId != null)
                        : true)
                    : true) ||
                    (BaseFilter.TablesNameId != null ?
                        (TabelName.TableName == "TLIradioAntenna" ?
                            (x.allLoadInst.radioAntennaId != null)
                        : true)
                    : true) ||
                    (BaseFilter.TablesNameId != null ?
                        (TabelName.TableName == "TLIradioRRU" ?
                            (x.allLoadInst.radioRRUId != null)
                        : true)
                    : true) ||
                    (BaseFilter.TablesNameId != null ?
                        (TabelName.TableName == "TLIpower" ?
                            (x.allLoadInst.powerId != null)
                        : true)
                    : true)
                ) &&
                ((BaseFilter.TablesNameId == null ? (
                    BaseFilter.PartId != null ? (
                        PartName.Name == "MW" ? (
                            x.allLoadInst.mwRFUId != null)
                        : true)
                    : true)
                : true) ||
                (BaseFilter.TablesNameId == null ? (
                    BaseFilter.PartId != null ? (
                        PartName.Name == "MW" ? (
                            x.allLoadInst.mwDishId != null)
                        : true)
                    : true)
                : true) ||
                (BaseFilter.TablesNameId == null ? (
                    BaseFilter.PartId != null ? (
                        PartName.Name == "MW" ? (
                            x.allLoadInst.mwBUId != null)
                        : true)
                    : true)
                : true) ||
                (BaseFilter.TablesNameId == null ? (
                    BaseFilter.PartId != null ? (
                        PartName.Name == "Radio" ? (
                            x.allLoadInst.radioAntennaId != null)
                        : true)
                    : true)
                : true) ||
                (BaseFilter.TablesNameId == null ? (
                    BaseFilter.PartId != null ? (
                        PartName.Name == "Radio" ? (
                            x.allLoadInst.radioRRUId != null)
                        : true)
                    : true)
                : true) ||
                (BaseFilter.TablesNameId == null ? (
                    BaseFilter.PartId != null ? (
                        PartName.Name == "Other Inventory" ? (
                            x.allLoadInst.powerId != null)
                        : true)
                    : true)
                : true)
                ), x => x.allLoadInst).ToList();


                List<MW_RFULibInst> MW_RFUListToAddToModel = new List<MW_RFULibInst>();
                List<MW_ODULibInst> MW_ODUListToAddToModel = new List<MW_ODULibInst>();
                List<MW_DishLibInst> MW_DishListToAddToModel = new List<MW_DishLibInst>();
                List<MW_BULibInst> MW_BUListToAddToModel = new List<MW_BULibInst>();
                List<RadioAntennaLibInst> RadioAntennaListToAddToModel = new List<RadioAntennaLibInst>();
                List<RadioRRULibInst> RadioRRUListToAddToModel = new List<RadioRRULibInst>();
                List<PowerLibInst> PowerListToAddToModel = new List<PowerLibInst>();
                foreach (var CivilLoad in CivilLoads)
                {
                    if (CivilLoad.allLoadInstId != null)
                    {
                        TLIallLoadInst allLoadsInst = _unitOfWork.AllLoadInstRepository.GetIncludeWhere(x => x.Id == CivilLoad.allLoadInstId.Value, x => x.mwBU, x => x.mwDish,
                            x => x.mwODU, x => x.mwRFU).FirstOrDefault();

                        //
                        // Installation + Libraries + Dynamic Att...
                        //
                        if (allLoadsInst.mwRFUId != null)
                        {
                            List<DynamicAttDto> DynamicAttListCopy = new List<DynamicAttDto>();

                            // Installation Object + Dynamic Attributes For This Installation Object..
                            MW_RFUsLibInst.MW_RFU = _unitOfWork.MW_RFURepository.Get(allLoadsInst.mwRFUId.Value);
                            var DynamicAttInstValueRecords = _unitOfWork.DynamicAttInstValueRepository.GetIncludeWhere(x => (x.DynamicAtt.disable == false &&
                                x.DynamicAtt.tablesNames.TableName == "TLImwRFU" &&
                                x.InventoryId == MW_RFUsLibInst.MW_RFU.Id &&
                                !x.DynamicAtt.LibraryAtt), x => x.DynamicAtt, x => x.tablesNames, x => x.DynamicAtt.DataType).ToList();

                            foreach (var DynamicAttInstValueRecord in DynamicAttInstValueRecords)
                            {
                                var DynamicAttDtoObject = GetDynamicAttDto(DynamicAttInstValueRecord, null);
                                DynamicAttListCopy.Add(DynamicAttDtoObject);
                            }

                            // Library Object + Dynamic Attributes For This Library Object..
                            MW_RFUsLibInst.MW_RFULibrary = _unitOfWork.MW_RFULibraryRepository.Get(MW_RFUsLibInst.MW_RFU.MwRFULibraryId);

                            List<TLIdynamicAttLibValue> DynamicAttLibRecords = _unitOfWork.DynamicAttLibRepository.GetIncludeWhere(x => (
                                x.DynamicAtt.disable == false && x.DynamicAtt.tablesNames.TableName == "TLImwRFULibrary" &&
                                x.InventoryId == MW_RFUsLibInst.MW_RFU.MwRFULibraryId && x.DynamicAtt.LibraryAtt),
                                x => x.DynamicAtt, x => x.tablesNames, x => x.DynamicAtt.DataType).ToList();
                            foreach (var DynamicAttLibRecord in DynamicAttLibRecords)
                            {
                                var DynamicAttDtoObject = GetDynamicAttDto(null, DynamicAttLibRecord);
                                DynamicAttListCopy.Add(DynamicAttDtoObject);
                            }
                            MW_RFUsLibInst.DynamicAttList = DynamicAttListCopy;


                            // TicketId..
                            if (allLoadsInst.TicketId != null)
                            {
                                MW_RFUsLibInst.TicketId = allLoadsInst.TicketId;
                            }

                            // ItemStatusId + ItemStatus Object..
                            if (allLoadsInst.ItemStatusId != null)
                            {
                                MW_RFUsLibInst.ItemStatusId = allLoadsInst.ItemStatusId;
                                MW_RFUsLibInst.ItemStatus = _unitOfWork.ItemStatusRepository.GetByID(MW_RFUsLibInst.ItemStatusId.Value);
                            }
                            MW_RFUListToAddToModel.Add(MW_RFUsLibInst);
                        }
                        else if (allLoadsInst.mwODUId != null)
                        {
                            List<DynamicAttDto> DynamicAttListCopy = new List<DynamicAttDto>();

                            // Installation Object + Dynamic Attributes For This Installation Object..
                            MW_ODUsLibInst.MW_ODU = _unitOfWork.MW_ODURepository.Get(allLoadsInst.mwODUId.Value);
                            var DynamicAttInstValueRecords = _unitOfWork.DynamicAttInstValueRepository.GetIncludeWhere(x => (x.DynamicAtt.disable == false &&
                                x.DynamicAtt.tablesNames.TableName == "TLImwODU" &&
                                x.InventoryId == MW_ODUsLibInst.MW_ODU.Id &&
                                !x.DynamicAtt.LibraryAtt), x => x.DynamicAtt, x => x.tablesNames, x => x.DynamicAtt.DataType).ToList();
                            foreach (var DynamicAttInstValueRecord in DynamicAttInstValueRecords)
                            {
                                var DynamicAttDtoObject = GetDynamicAttDto(DynamicAttInstValueRecord, null);
                                DynamicAttListCopy.Add(DynamicAttDtoObject);
                            }

                            // Library Object + Dynamic Attributes For This Library Object..
                            MW_ODUsLibInst.MW_ODULibrary = _unitOfWork.MW_ODULibraryRepository.Get(MW_ODUsLibInst.MW_ODU.MwODULibraryId);
                            var DynamicAttLibRecords = _unitOfWork.DynamicAttLibRepository.GetIncludeWhere(x => (
                                x.DynamicAtt.disable == false && x.DynamicAtt.tablesNames.TableName == "TLImwODULibrary" &&
                                x.InventoryId == MW_ODUsLibInst.MW_ODU.MwODULibraryId && x.DynamicAtt.LibraryAtt),
                                x => x.DynamicAtt, x => x.tablesNames, x => x.DynamicAtt.DataType).ToList();
                            foreach (var DynamicAttLibRecord in DynamicAttLibRecords)
                            {
                                var DynamicAttDtoObject = GetDynamicAttDto(null, DynamicAttLibRecord);
                                DynamicAttListCopy.Add(DynamicAttDtoObject);
                            }
                            MW_ODUsLibInst.DynamicAttList = DynamicAttListCopy;

                            // TicketId..
                            if (allLoadsInst.TicketId != null)
                            {
                                MW_ODUsLibInst.TicketId = allLoadsInst.TicketId;
                            }

                            // ItemStatusId + ItemStatus Object..
                            if (allLoadsInst.ItemStatusId != null)
                            {
                                MW_ODUsLibInst.ItemStatusId = allLoadsInst.ItemStatusId;
                                MW_ODUsLibInst.ItemStatus = _unitOfWork.ItemStatusRepository.GetByID(MW_ODUsLibInst.ItemStatusId.Value);
                            }
                            MW_ODUListToAddToModel.Add(MW_ODUsLibInst);
                        }
                        else if (allLoadsInst.mwDishId != null)
                        {
                            List<DynamicAttDto> DynamicAttListCopy = new List<DynamicAttDto>();

                            // Installation Object + Dynamic Attributes For This Installation Object..
                            MW_DishesLibInst.MW_Dish = _unitOfWork.MW_DishRepository.Get(allLoadsInst.mwDishId.Value);
                            var DynamicAttInstValueRecords = _unitOfWork.DynamicAttInstValueRepository.GetIncludeWhere(x => (x.DynamicAtt.disable == false &&
                                x.DynamicAtt.tablesNames.TableName == "TLImwDish" &&
                                x.InventoryId == MW_DishesLibInst.MW_Dish.Id &&
                                !x.DynamicAtt.LibraryAtt), x => x.DynamicAtt, x => x.tablesNames, x => x.DynamicAtt.DataType).ToList();

                            foreach (var DynamicAttInstValueRecord in DynamicAttInstValueRecords)
                            {
                                var DynamicAttDtoObject = GetDynamicAttDto(DynamicAttInstValueRecord, null);
                                DynamicAttListCopy.Add(DynamicAttDtoObject);
                            }

                            // Library Object + Dynamic Attributes For This Library Object..
                            MW_DishesLibInst.MW_DishLibrary = _unitOfWork.MW_DishLibraryRepository.Get(MW_DishesLibInst.MW_Dish.MwDishLibraryId);
                            var DynamicAttLibRecords = _unitOfWork.DynamicAttLibRepository.GetIncludeWhere(x => (
                                x.DynamicAtt.disable == false && x.DynamicAtt.tablesNames.TableName == "TLImwDishLibrary" &&
                                x.InventoryId == MW_DishesLibInst.MW_Dish.MwDishLibraryId && x.DynamicAtt.LibraryAtt),
                                x => x.DynamicAtt, x => x.tablesNames, x => x.DynamicAtt.DataType).ToList();
                            foreach (var DynamicAttLibRecord in DynamicAttLibRecords)
                            {
                                var DynamicAttDtoObject = GetDynamicAttDto(null, DynamicAttLibRecord);
                                DynamicAttListCopy.Add(DynamicAttDtoObject);
                            }
                            MW_DishesLibInst.DynamicAttList = DynamicAttListCopy;


                            // TicketId..
                            if (allLoadsInst.TicketId != null)
                            {
                                MW_DishesLibInst.TicketId = allLoadsInst.TicketId;
                            }

                            // ItemStatusId + ItemStatus Object..
                            if (allLoadsInst.ItemStatusId != null)
                            {
                                MW_DishesLibInst.ItemStatusId = allLoadsInst.ItemStatusId;
                                MW_DishesLibInst.ItemStatus = _unitOfWork.ItemStatusRepository.GetByID(MW_DishesLibInst.ItemStatusId.Value);
                            }
                            MW_DishListToAddToModel.Add(MW_DishesLibInst);
                        }
                        else if (allLoadsInst.mwBUId != null)
                        {
                            List<DynamicAttDto> DynamicAttListCopy = new List<DynamicAttDto>();

                            // Installation Object + Dynamic Attributes For This Installation Object..
                            MW_BUsLibInst.MW_BU = _unitOfWork.MW_BURepository.Get(allLoadsInst.mwBUId.Value);
                            var DynamicAttInstValueRecords = _unitOfWork.DynamicAttInstValueRepository.GetIncludeWhere(x => (x.DynamicAtt.disable == false &&
                                x.DynamicAtt.tablesNames.TableName == "TLImwBU" &&
                                x.InventoryId == MW_BUsLibInst.MW_BU.Id &&
                                !x.DynamicAtt.LibraryAtt), x => x.DynamicAtt, x => x.tablesNames, x => x.DynamicAtt.DataType).ToList();

                            foreach (var DynamicAttInstValueRecord in DynamicAttInstValueRecords)
                            {
                                var DynamicAttDtoObject = GetDynamicAttDto(DynamicAttInstValueRecord, null);
                                DynamicAttListCopy.Add(DynamicAttDtoObject);
                            }

                            // Library Object + Dynamic Attributes For This Library Object..
                            MW_BUsLibInst.MW_BULibrary = _unitOfWork.MW_BULibraryRepository.Get(MW_BUsLibInst.MW_BU.MwBULibraryId);
                            var DynamicAttLibRecords = _unitOfWork.DynamicAttLibRepository.GetIncludeWhere(x => (
                                x.DynamicAtt.disable == false && x.DynamicAtt.tablesNames.TableName == "TLImwBULibrary" &&
                                x.InventoryId == MW_BUsLibInst.MW_BU.MwBULibraryId && x.DynamicAtt.LibraryAtt),
                                x => x.DynamicAtt, x => x.tablesNames, x => x.DynamicAtt.DataType).ToList();
                            foreach (var DynamicAttLibRecord in DynamicAttLibRecords)
                            {
                                var DynamicAttDtoObject = GetDynamicAttDto(null, DynamicAttLibRecord);
                                DynamicAttListCopy.Add(DynamicAttDtoObject);
                            }
                            MW_BUsLibInst.DynamicAttList = DynamicAttListCopy;


                            // TicketId..
                            if (allLoadsInst.TicketId != null)
                            {
                                MW_BUsLibInst.TicketId = allLoadsInst.TicketId;
                            }

                            // ItemStatusId + ItemStatus Object..
                            if (allLoadsInst.ItemStatusId != null)
                            {
                                MW_BUsLibInst.ItemStatusId = allLoadsInst.ItemStatusId;
                                MW_BUsLibInst.ItemStatus = _unitOfWork.ItemStatusRepository.GetByID(MW_BUsLibInst.ItemStatusId.Value);
                            }
                            MW_BUListToAddToModel.Add(MW_BUsLibInst);
                        }
                        else if (allLoadsInst.radioAntennaId != null)
                        {
                            List<DynamicAttDto> DynamicAttListCopy = new List<DynamicAttDto>();

                            // Installation Object + Dynamic Attributes For This Installation Object..
                            RadioAntennasLibInst.RadioAntenna = _unitOfWork.RadioAntennaRepository.Get(allLoadsInst.radioAntennaId.Value);
                            var DynamicAttInstValueRecords = _unitOfWork.DynamicAttInstValueRepository.GetIncludeWhere(x => (x.DynamicAtt.disable == false &&
                                x.DynamicAtt.tablesNames.TableName == "TLIradioAntenna" &&
                                x.InventoryId == RadioAntennasLibInst.RadioAntenna.Id &&
                                !x.DynamicAtt.LibraryAtt), x => x.DynamicAtt, x => x.tablesNames, x => x.DynamicAtt.DataType).ToList();

                            foreach (var DynamicAttInstValueRecord in DynamicAttInstValueRecords)
                            {
                                var DynamicAttDtoObject = GetDynamicAttDto(DynamicAttInstValueRecord, null);
                                DynamicAttListCopy.Add(DynamicAttDtoObject);
                            }

                            // Library Object + Dynamic Attributes For This Library Object..
                            RadioAntennasLibInst.RadioAntennaLibrary = _unitOfWork.RadioAntennaLibraryRepository.Get(RadioAntennasLibInst.RadioAntenna.radioAntennaLibraryId);
                            var DynamicAttLibRecords = _unitOfWork.DynamicAttLibRepository.GetIncludeWhere(x => (
                                x.DynamicAtt.disable == false && x.DynamicAtt.tablesNames.TableName == "TLIradioAntennaLibrary" &&
                                x.InventoryId == RadioAntennasLibInst.RadioAntenna.radioAntennaLibraryId && x.DynamicAtt.LibraryAtt),
                                x => x.DynamicAtt, x => x.tablesNames, x => x.DynamicAtt.DataType).ToList();
                            foreach (var DynamicAttLibRecord in DynamicAttLibRecords)
                            {
                                var DynamicAttDtoObject = GetDynamicAttDto(null, DynamicAttLibRecord);
                                DynamicAttListCopy.Add(DynamicAttDtoObject);
                            }
                            RadioAntennasLibInst.DynamicAttList = DynamicAttListCopy;


                            // TicketId..
                            if (allLoadsInst.TicketId != null)
                            {
                                RadioAntennasLibInst.TicketId = allLoadsInst.TicketId;
                            }

                            // ItemStatusId + ItemStatus Object..
                            if (allLoadsInst.ItemStatusId != null)
                            {
                                RadioAntennasLibInst.ItemStatusId = allLoadsInst.ItemStatusId;
                                RadioAntennasLibInst.ItemStatus = _unitOfWork.ItemStatusRepository.GetByID(RadioAntennasLibInst.ItemStatusId.Value);
                            }
                            RadioAntennaListToAddToModel.Add(RadioAntennasLibInst);
                        }
                        else if (allLoadsInst.radioRRUId != null)
                        {
                            List<DynamicAttDto> DynamicAttListCopy = new List<DynamicAttDto>();

                            // Installation Object + Dynamic Attributes For This Installation Object..
                            RadioRRUSLibInst.RadioRRU = _unitOfWork.RadioRRURepository.Get(allLoadsInst.radioRRUId.Value);
                            var DynamicAttInstValueRecords = _unitOfWork.DynamicAttInstValueRepository.GetIncludeWhere(x => (x.DynamicAtt.disable == false &&
                                x.DynamicAtt.tablesNames.TableName == "TLIradioRRU" &&
                                x.InventoryId == RadioRRUSLibInst.RadioRRU.Id &&
                                !x.DynamicAtt.LibraryAtt), x => x.DynamicAtt, x => x.tablesNames, x => x.DynamicAtt.DataType).ToList();

                            foreach (var DynamicAttInstValueRecord in DynamicAttInstValueRecords)
                            {
                                var DynamicAttDtoObject = GetDynamicAttDto(DynamicAttInstValueRecord, null);
                                DynamicAttListCopy.Add(DynamicAttDtoObject);
                            }

                            // Library Object + Dynamic Attributes For This Library Object..
                            RadioRRUSLibInst.RadioRRULibrary = _unitOfWork.RadioRRULibraryRepository.Get(RadioRRUSLibInst.RadioRRU.radioRRULibraryId);
                            var DynamicAttLibRecords = _unitOfWork.DynamicAttLibRepository.GetIncludeWhere(x => (
                                x.DynamicAtt.disable == false && x.DynamicAtt.tablesNames.TableName == "TLIradioRRULibrary" &&
                                x.InventoryId == RadioRRUSLibInst.RadioRRU.radioRRULibraryId && x.DynamicAtt.LibraryAtt),
                                x => x.DynamicAtt, x => x.tablesNames, x => x.DynamicAtt.DataType).ToList();
                            foreach (var DynamicAttLibRecord in DynamicAttLibRecords)
                            {
                                var DynamicAttDtoObject = GetDynamicAttDto(null, DynamicAttLibRecord);
                                DynamicAttListCopy.Add(DynamicAttDtoObject);
                            }
                            RadioRRUSLibInst.DynamicAttList = DynamicAttListCopy;


                            // TicketId..
                            if (allLoadsInst.TicketId != null)
                            {
                                RadioRRUSLibInst.TicketId = allLoadsInst.TicketId;
                            }

                            // ItemStatusId + ItemStatus Object..
                            if (allLoadsInst.ItemStatusId != null)
                            {
                                RadioRRUSLibInst.ItemStatusId = allLoadsInst.ItemStatusId;
                                RadioRRUSLibInst.ItemStatus = _unitOfWork.ItemStatusRepository.GetByID(RadioRRUSLibInst.ItemStatusId.Value);
                            }
                            RadioRRUListToAddToModel.Add(RadioRRUSLibInst);
                        }
                        else if (allLoadsInst.powerId != null)
                        {
                            List<DynamicAttDto> DynamicAttListCopy = new List<DynamicAttDto>();

                            // Installation Object + Dynamic Attributes For This Installation Object..
                            PowersLibInst.Power = _unitOfWork.PowerRepository.Get(allLoadsInst.powerId.Value);
                            var DynamicAttInstValueRecords = _unitOfWork.DynamicAttInstValueRepository.GetIncludeWhere(x => (x.DynamicAtt.disable == false &&
                                x.DynamicAtt.tablesNames.TableName == "TLIpower" &&
                                x.InventoryId == PowersLibInst.Power.Id &&
                                !x.DynamicAtt.LibraryAtt), x => x.DynamicAtt, x => x.tablesNames, x => x.DynamicAtt.DataType).ToList();

                            foreach (var DynamicAttInstValueRecord in DynamicAttInstValueRecords)
                            {
                                var DynamicAttDtoObject = GetDynamicAttDto(DynamicAttInstValueRecord, null);
                                DynamicAttListCopy.Add(DynamicAttDtoObject);
                            }

                            // Library Object + Dynamic Attributes For This Library Object..
                            PowersLibInst.PowerLibrary = _unitOfWork.PowerLibraryRepository.Get(PowersLibInst.Power.powerLibraryId);
                            var DynamicAttLibRecords = _unitOfWork.DynamicAttLibRepository.GetIncludeWhere(x => (
                                x.DynamicAtt.disable == false && x.DynamicAtt.tablesNames.TableName == "TLIpowerLibrary" &&
                                x.InventoryId == PowersLibInst.Power.powerLibraryId && x.DynamicAtt.LibraryAtt),
                                x => x.DynamicAtt, x => x.tablesNames, x => x.DynamicAtt.DataType).ToList();
                            foreach (var DynamicAttLibRecord in DynamicAttLibRecords)
                            {
                                var DynamicAttDtoObject = GetDynamicAttDto(null, DynamicAttLibRecord);
                                DynamicAttListCopy.Add(DynamicAttDtoObject);
                            }
                            PowersLibInst.DynamicAttList = DynamicAttListCopy;


                            // TicketId..
                            if (allLoadsInst.TicketId != null)
                            {
                                PowersLibInst.TicketId = allLoadsInst.TicketId;
                            }

                            // ItemStatusId + ItemStatus Object..
                            if (allLoadsInst.ItemStatusId != null)
                            {
                                PowersLibInst.ItemStatusId = allLoadsInst.ItemStatusId;
                                PowersLibInst.ItemStatus = _unitOfWork.ItemStatusRepository.GetByID(PowersLibInst.ItemStatusId.Value);
                            }
                            PowerListToAddToModel.Add(PowersLibInst);
                        }


                    }

                }

                MW_RFUsLibInstList.Model = MW_RFUListToAddToModel;
                MW_ODUsLibInstList.Model = MW_ODUListToAddToModel;
                MW_DishesLibInstList.Model = MW_DishListToAddToModel;
                MW_BUsLibInstList.Model = MW_BUListToAddToModel;
                RadioAntennasLibInstList.Model = RadioAntennaListToAddToModel;
                RadioRRUSLibInstList.Model = RadioRRUListToAddToModel;
                PowersLibInstList.Model = PowerListToAddToModel;

                // Add Filters...
                if (WithFilterData)
                {
                    MW_RFUsLibInstList.filters = _unitOfWork.MW_RFURepository.GetRelatedTables();
                    MW_ODUsLibInstList.filters = _unitOfWork.MW_ODURepository.GetRelatedTables(BaseFilter.SiteCode);
                    MW_DishesLibInstList.filters = _unitOfWork.MW_DishRepository.GetRelatedTables();
                    MW_BUsLibInstList.filters = _unitOfWork.MW_BURepository.GetRelatedTables(BaseFilter.SiteCode);
                    RadioAntennasLibInstList.filters = _unitOfWork.RadioAntennaRepository.GetRelatedTables();
                    RadioRRUSLibInstList.filters = _unitOfWork.RadioRRURepository.GetRelatedTables(BaseFilter.SiteCode);
                    PowersLibInstList.filters = _unitOfWork.PowerRepository.GetRelatedTables();
                }


                return new Response<LoadsViewModel>(true, new LoadsViewModel
                {
                    MW_Dishes = MW_DishesLibInstList,
                    MW_BUs = MW_BUsLibInstList,
                    MW_ODUs = MW_ODUsLibInstList,
                    MW_RFUs = MW_RFUsLibInstList,
                    RadioAntennas = RadioAntennasLibInstList,
                    RadioRRUS = RadioRRUSLibInstList,
                    Powers = PowersLibInstList
                }, null, null, (int)Helpers.Constants.ApiReturnCode.success,
                    MW_DishesLibInstList.Model.Count() +
                    MW_BUsLibInstList.Model.Count() +
                    MW_ODUsLibInstList.Model.Count() +
                    MW_RFUsLibInstList.Model.Count() +
                    RadioAntennasLibInstList.Model.Count() +
                    RadioRRUSLibInstList.Model.Count() +
                    PowersLibInstList.Model.Count());

            }
            catch (Exception err)
            {
                return new Response<LoadsViewModel>(false, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
        }

        #region Helper Method..
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
        #region Get Loads On Site Enabled Attributes Only With Dynamic Objects With 2 Filters...
        // 1. Get MW_Dish...
        public Response<ReturnWithFilters<object>> GetMW_DishOnSiteWithEnableAtt(LoadsOnSiteFilter BaseFilter, bool WithFilterData, CombineFilters CombineFilters, ParameterPagination parameterPagination, int? CivilId, string CivilType)
        {
            try
            {
                int Count = 0;
                List<FilterObjectList> ObjectAttributeFilters = CombineFilters.filters;
                List<DateFilterViewModel> DateFilter = CombineFilters.DateFilter;

                List<object> OutPutList = new List<object>();
                ReturnWithFilters<object> MW_DishesTableDisplay = new ReturnWithFilters<object>();

                List<TLIcivilLoads> CivilLoadsRecords = new List<TLIcivilLoads>();

                List<StringFilterObjectList> AttributeFilters = new List<StringFilterObjectList>();
                List<AttributeActivatedViewModel> MW_DishInstallationAttribute = new List<AttributeActivatedViewModel>();

                if ((DateFilter != null ? DateFilter.Count() > 0 : false) ||
                    (ObjectAttributeFilters != null && ObjectAttributeFilters.Count > 0))
                {
                    MW_DishInstallationAttribute = _mapper.Map<List<AttributeActivatedViewModel>>(_unitOfWork.AttributeViewManagmentRepository.GetIncludeWhere(x =>
                        x.Enable && x.AttributeActivatedId != null &&
                        x.EditableManagmentView.View == Helpers.Constants.EditableManamgmantViewNames.MW_DishInstallation.ToString() &&
                        x.EditableManagmentView.TLItablesNames1.TableName == Helpers.Constants.TablesNames.TLImwDish.ToString(),
                            x => x.AttributeActivated, x => x.EditableManagmentView, x => x.EditableManagmentView.TLItablesNames1,
                            x => x.EditableManagmentView.TLItablesNames2)
                    .Select(x => x.AttributeActivated).ToList());
                }

                if (ObjectAttributeFilters != null && ObjectAttributeFilters.Count > 0)
                {
                    List<AttributeActivatedViewModel> NotDateDateMW_DishInstallationAttribute = MW_DishInstallationAttribute.Where(x =>
                        x.DataType.ToLower() != "datetime").ToList();

                    foreach (FilterObjectList item in ObjectAttributeFilters)
                    {
                        List<string> value = item.value.Select(x => x.ToString().ToLower()).ToList();

                        AttributeActivatedViewModel AttributeKey = NotDateDateMW_DishInstallationAttribute.FirstOrDefault(x =>
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
                    List<AttributeActivatedViewModel> DateMW_DishInstallationAttribute = MW_DishInstallationAttribute.Where(x =>
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

                        AttributeActivatedViewModel AttributeKey = DateMW_DishInstallationAttribute.FirstOrDefault(x =>
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

                List<int> MW_DishIds = new List<int>();
                List<int> WithoutDateFilterMW_DishInstallation = new List<int>();
                List<int> WithDateFilterMW_DishInstallation = new List<int>();

                if (AttributeFilters != null && AttributeFilters.Count > 0)
                {
                    //
                    // Installation Dynamic Attributes...
                    //
                    List<TLIdynamicAtt> InstDynamicAttListIds = _unitOfWork.DynamicAttRepository.GetIncludeWhere(x =>
                        AttributeFilters.AsEnumerable().Select(y => y.key.ToLower()).Contains(x.Key.ToLower()) &&
                        !x.LibraryAtt && !x.disable &&
                        x.tablesNames.TableName == Helpers.Constants.TablesNames.TLImwDish.ToString()
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
                    bool AttrInstExist = typeof(MW_DishViewModel).GetProperties().ToList().Exists(x =>
                        AttributeFilters.Where(y => y.key.ToLower() != "Id".ToLower()).Select(y =>
                            y.key.ToLower()).Contains(x.Name.ToLower()));

                    List<int> InstallationAttributeActivated = new List<int>();
                    if (AttrInstExist)
                    {
                        List<PropertyInfo> NotStringProps = typeof(MW_DishViewModel).GetProperties().Where(x =>
                            x.PropertyType.Name.ToLower() != "string" &&
                                AttributeFilters.Select(y =>
                                    y.key.ToLower()).Contains(x.Name.ToLower())).ToList();

                        List<PropertyInfo> StringProps = typeof(MW_DishViewModel).GetProperties().Where(x =>
                            x.PropertyType.Name.ToLower() == "string" &&
                                AttributeFilters.Select(y =>
                                    y.key.ToLower()).Contains(x.Name.ToLower())).ToList();

                        List<StringFilterObjectList> AttrInstAttributeFilters = AttributeFilters.Where(x =>
                            NotStringProps.Select(y => y.Name.ToLower()).Contains(x.key.ToLower()) ||
                            StringProps.Select(y => y.Name.ToLower()).Contains(x.key.ToLower())).ToList();

                        //InstallationAttributeActivated = _unitOfWork.MW_DishRepository.GetWhere(x =>
                        //    AttrInstAttributeFilters.All(z =>
                        //    NotStringProps.Exists(y => (z.key.ToLower() == y.Name.ToLower()) && (y.GetValue(_mapper.Map<MW_DishViewModel>(x), null) != null ? z.value.Contains(y.GetValue(_mapper.Map<MW_DishViewModel>(x), null).ToString().ToLower()) : false)) ||
                        //    StringProps.Exists(y => (z.key.ToLower() == y.Name.ToLower()) && (z.value.Any(w =>
                        //            y.GetValue(_mapper.Map<MW_DishViewModel>(x), null) != null ? y.GetValue(_mapper.Map<MW_DishViewModel>(x), null).ToString().ToLower().StartsWith(w.ToLower()) : false))))
                        //).Select(i => i.Id).ToList();

                        IEnumerable<TLImwDish> Installations = _unitOfWork.MW_DishRepository.GetAllWithoutCount();

                        foreach (StringFilterObjectList InstallationProp in AttrInstAttributeFilters)
                        {
                            if (StringProps.Select(x => x.Name.ToLower()).Contains(InstallationProp.key.ToLower()))
                            {
                                Installations = Installations.Where(x => StringProps.AsEnumerable().FirstOrDefault(y => (InstallationProp.key.ToLower() == y.Name.ToLower()) && (InstallationProp.value.AsEnumerable().FirstOrDefault(w =>
                                     y.GetValue(_mapper.Map<MW_DishViewModel>(x), null) != null ? y.GetValue(_mapper.Map<MW_DishViewModel>(x), null).ToString().ToLower().StartsWith(w.ToLower()) : false) != null)) != null).AsEnumerable();
                            }
                            else if (NotStringProps.Select(x => x.Name.ToLower()).Contains(InstallationProp.key.ToLower()))
                            {
                                Installations = Installations.Where(x => NotStringProps.AsEnumerable().FirstOrDefault(y => (InstallationProp.key.ToLower() == y.Name.ToLower()) && (y.GetValue(_mapper.Map<MW_DishViewModel>(x), null) != null ?
                                    InstallationProp.value.AsEnumerable().Contains(y.GetValue(_mapper.Map<MW_DishViewModel>(x), null).ToString().ToLower()) : false)) != null).AsEnumerable();
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
                        WithoutDateFilterMW_DishInstallation = InstallationAttributeActivated.Intersect(DynamicInstValueListIds).ToList();
                    }
                    else if (AttrInstExist)
                    {
                        WithoutDateFilterMW_DishInstallation = InstallationAttributeActivated;
                    }
                    else if (DynamicInstExist)
                    {
                        WithoutDateFilterMW_DishInstallation = DynamicInstValueListIds;
                    }
                }

                if (DateFilter != null ? DateFilter.Count() > 0 : false)
                {
                    List<TLIdynamicAtt> DateTimeInstDynamicAttListIds = _unitOfWork.DynamicAttRepository.GetIncludeWhere(x =>
                        AfterConvertDateFilters.AsEnumerable().Select(y => y.key.ToLower()).Contains(x.Key.ToLower()) &&
                        !x.LibraryAtt && !x.disable &&
                        x.tablesNames.TableName == Helpers.Constants.TablesNames.TLImwDish.ToString()
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
                    List<PropertyInfo> InstallationProps = typeof(MW_DishViewModel).GetProperties().Where(x =>
                        AfterConvertDateFilters.Select(y =>
                            y.key.ToLower()).Contains(x.Name.ToLower())).ToList();

                    List<int> InstallationAttributeActivatedIds = new List<int>();
                    bool AttrInstExist = false;

                    if (InstallationProps != null)
                    {
                        AttrInstExist = true;

                        List<DateFilterViewModel> InstallationPropsAttributeFilters = AfterConvertDateFilters.Where(x =>
                            InstallationProps.Select(y => y.Name.ToLower()).Contains(x.key.ToLower())).ToList();

                        //InstallationAttributeActivatedIds = _unitOfWork.MW_DishRepository.GetWhere(x =>
                        //    InstallationPropsAttributeFilters.All(z =>
                        //        (InstallationProps.Exists(y => (z.key.ToLower() == y.Name.ToLower()) && ((y.GetValue(_mapper.Map<MW_DishViewModel>(x), null) != null) ?
                        //            ((z.DateFrom <= Convert.ToDateTime(y.GetValue(_mapper.Map<MW_DishViewModel>(x), null))) &&
                        //             (z.DateTo >= Convert.ToDateTime(y.GetValue(_mapper.Map<MW_DishViewModel>(x), null)))) : (false)))))
                        //).Select(i => i.Id).ToList();

                        IEnumerable<TLImwDish> Installations = _unitOfWork.MW_DishRepository.GetAllWithoutCount();

                        foreach (DateFilterViewModel InstallationProp in InstallationPropsAttributeFilters)
                        {
                            Installations = Installations.Where(x => InstallationProps.Exists(y => (InstallationProp.key.ToLower() == y.Name.ToLower()) && ((y.GetValue(_mapper.Map<MW_DishViewModel>(x), null) != null) ?
                                ((InstallationProp.DateFrom.Date <= Convert.ToDateTime(y.GetValue(_mapper.Map<MW_DishViewModel>(x), null)).Date) &&
                                    (InstallationProp.DateTo.Date >= Convert.ToDateTime(y.GetValue(_mapper.Map<MW_DishViewModel>(x), null)).Date)) : (false))));
                        }

                        InstallationAttributeActivatedIds = Installations.Select(x => x.Id).ToList();
                    }

                    //
                    // Installation (Attribute Activated + Dynamic) Attributes...
                    //
                    if (AttrInstExist && DynamicInstExist)
                    {
                        WithDateFilterMW_DishInstallation = InstallationAttributeActivatedIds.Intersect(DynamicInstValueListIds).ToList();
                    }
                    else if (AttrInstExist)
                    {
                        WithDateFilterMW_DishInstallation = InstallationAttributeActivatedIds;
                    }
                    else if (DynamicInstExist)
                    {
                        WithDateFilterMW_DishInstallation = DynamicInstValueListIds;
                    }
                }

                if ((DateFilter != null ? DateFilter.Count() > 0 : false) ||
                    (AttributeFilters != null ? AttributeFilters.Count() > 0 : false))
                {
                    if ((AttributeFilters != null ? AttributeFilters.Count > 0 : false) &&
                            (AfterConvertDateFilters != null ? AfterConvertDateFilters.Count() > 0 : false))
                    {
                        MW_DishIds = WithoutDateFilterMW_DishInstallation.Intersect(WithDateFilterMW_DishInstallation).ToList();
                    }
                    else if (AttributeFilters != null ? AttributeFilters.Count > 0 : false)
                    {
                        MW_DishIds = WithoutDateFilterMW_DishInstallation;
                    }
                    else if (AfterConvertDateFilters != null ? AfterConvertDateFilters.Count() > 0 : false)
                    {
                        MW_DishIds = WithDateFilterMW_DishInstallation;
                    }

                    CivilLoadsRecords = _unitOfWork.CivilLoadsRepository.GetIncludeWhere(x =>
                        (x.allLoadInstId != null ? x.allLoadInst.mwDishId != null : false) &&
                        (!x.Dismantle) &&
                            (x.SiteCode == BaseFilter.SiteCode) &&
                            (BaseFilter.ItemStatusId != null ? (
                                x.allLoadInst != null ? (
                                    x.allLoadInst.ItemStatusId != null ?
                                            (x.allLoadInst.ItemStatusId == BaseFilter.ItemStatusId)
                                    : false)
                                : false)
                            : true) &&
                            (BaseFilter.TicketId != null ? (
                                x.allLoadInst != null ? (
                                    x.allLoadInst.TicketId != null ?
                                            (x.allLoadInst.TicketId == BaseFilter.TicketId)
                                    : false)
                                : false)
                            : true) &&
                            (BaseFilter.AllCivilId != null ?
                                (x.allCivilInstId == BaseFilter.AllCivilId)
                            : true) &&

                            MW_DishIds.Contains(x.allLoadInst.mwDishId.Value),
                        x => x.allCivilInst, x => x.allLoadInst, x => x.allLoadInst.mwDish, x => x.allLoadInst.mwDish.InstallationPlace, x => x.allLoadInst.mwDish.MwDishLibrary,
                        x => x.allLoadInst.mwDish.owner, x => x.allLoadInst.mwDish.PolarityOnLocation, x => x.allLoadInst.mwDish.RepeaterType,
                        x => x.allLoadInst.mwDish.ItemConnectTo).ToList();
                }
                else
                {
                    CivilLoadsRecords = _unitOfWork.CivilLoadsRepository.GetIncludeWhere(x =>
                        (x.allLoadInstId != null ? x.allLoadInst.mwDishId != null : false) &&
                        (!x.Dismantle) &&
                        (x.SiteCode == BaseFilter.SiteCode) &&
                        (BaseFilter.ItemStatusId != null ? (
                            x.allLoadInst != null ? (
                                x.allLoadInst.ItemStatusId != null ?
                                        (x.allLoadInst.ItemStatusId == BaseFilter.ItemStatusId)
                                : false)
                            : false)
                        : true) &&
                        (BaseFilter.TicketId != null ? (
                            x.allLoadInst != null ? (
                                x.allLoadInst.TicketId != null ?
                                        (x.allLoadInst.TicketId == BaseFilter.TicketId)
                                : false)
                            : false)
                        : true) &&
                        (BaseFilter.AllCivilId != null ?
                            (x.allCivilInstId == BaseFilter.AllCivilId)
                        : true) &&
                        x.allLoadInst.mwDishId != null,

                    x => x.allCivilInst, x => x.allLoadInst, x => x.allLoadInst.mwDish, x => x.allLoadInst.mwDish.InstallationPlace, x => x.allLoadInst.mwDish.MwDishLibrary,
                    x => x.allLoadInst.mwDish.owner, x => x.allLoadInst.mwDish.PolarityOnLocation, x => x.allLoadInst.mwDish.RepeaterType,
                    x => x.allLoadInst.mwDish.ItemConnectTo).ToList();
                }
                // Delete Duplicated Objects Based On Installation Date...
                List<TLIcivilLoads> NewList = new List<TLIcivilLoads>();
                foreach (var item in CivilLoadsRecords)
                {
                    TLIcivilLoads CheckIfExist = NewList.FirstOrDefault(x => x.allLoadInst.mwDishId.Value == item.allLoadInst.mwDishId.Value);
                    if (CheckIfExist != null)
                    {
                        if (CheckIfExist.InstallationDate < item.InstallationDate)
                        {
                            NewList.Remove(CheckIfExist);
                            NewList.Add(item);
                        }
                    }
                    else
                    {
                        NewList.Add(item);
                    }
                }
                CivilLoadsRecords = NewList;

                if (CivilId != null)
                {
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

                    List<int> MW_DishesIds = _unitOfWork.CivilLoadsRepository.GetIncludeWhere(x => x.allCivilInstId == AllCivilInst.Id && !x.Dismantle &&
                        (x.allLoadInstId != null ? x.allLoadInst.mwDishId != null : false), x => x.allLoadInst).Select(x => x.allLoadInst.mwDishId.Value).Distinct().ToList();

                    CivilLoadsRecords = CivilLoadsRecords.Where(x => MW_DishesIds.Contains(x.allLoadInst.mwDishId.Value)).ToList();
                }

                if (BaseFilter.SideArmId != null)
                {
                    List<int> MW_DishesIds = _unitOfWork.CivilLoadsRepository.GetIncludeWhere(x => x.sideArmId.Value == BaseFilter.SideArmId.Value && !x.Dismantle &&
                        (x.allLoadInstId != null ? x.allLoadInst.mwDishId != null : false), x => x.allLoadInst).Select(x => x.allLoadInst.mwDishId.Value).Distinct().ToList();

                    CivilLoadsRecords = CivilLoadsRecords.Where(x => MW_DishesIds.Contains(x.allLoadInst.mwDishId.Value)).ToList();
                }

                Count = CivilLoadsRecords.Count();

                CivilLoadsRecords = CivilLoadsRecords.Skip((parameterPagination.PageNumber - 1) * parameterPagination.PageSize).
                    Take(parameterPagination.PageSize).ToList();

                List<MW_DishViewModel> Dishs = _mapper.Map<List<MW_DishViewModel>>(CivilLoadsRecords.Select(x => x.allLoadInst.mwDish).ToList());

                List<TLIattributeViewManagment> AllAttributes = _unitOfWork.AttributeViewManagmentRepository.GetIncludeWhere(x =>
                   (x.Enable && x.EditableManagmentView.View == Helpers.Constants.EditableManamgmantViewNames.MW_DishInstallation.ToString() &&
                   (x.AttributeActivatedId != null ?
                        (x.AttributeActivated.Tabel == Helpers.Constants.TablesNames.TLImwDish.ToString() && x.AttributeActivated.enable) :
                        (!x.DynamicAtt.LibraryAtt && !x.DynamicAtt.disable && x.DynamicAtt.tablesNames.TableName == Helpers.Constants.TablesNames.TLImwDish.ToString()))) ||
                    (x.AttributeActivated != null ?
                        ((x.AttributeActivated.Key.ToLower() == "id" || x.AttributeActivated.Key.ToLower() == "active") && x.AttributeActivated.Tabel == Helpers.Constants.TablesNames.TLImwDish.ToString()) : false),
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

                foreach (MW_DishViewModel DishsInstallationObject in Dishs)
                {
                    dynamic DynamiMW_DishInstallation = new ExpandoObject();

                    //
                    // Installation Object ViewModel...
                    //
                    if (NotDateTimeInstallationAttributesViewModel != null ? NotDateTimeInstallationAttributesViewModel.Count > 0 : false)
                    {
                        List<PropertyInfo> InstallationProps = typeof(MW_DishViewModel).GetProperties().Where(x =>
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
                                object ForeignKeyNamePropObject = prop.GetValue(DishsInstallationObject, null);
                                ((IDictionary<String, Object>)DynamiMW_DishInstallation).Add(new KeyValuePair<string, object>(prop.Name, ForeignKeyNamePropObject));
                            }
                            else if (NotDateTimeInstallationAttributesViewModel.Select(x =>
                                 x.AttributeActivated.Key.ToLower()).Contains(prop.Name.ToLower()) &&
                                !prop.Name.ToLower().Contains("_name") &&
                                (prop.Name.ToLower().Substring(Math.Max(0, prop.Name.Length - 2)) != "id" || prop.Name.ToLower() == "id"))
                            {
                                if (prop.Name.ToLower() != "id" && prop.Name.ToLower() != "active")
                                {
                                    TLIattributeViewManagment LabelName = AllAttributes.FirstOrDefault(x => ((x.AttributeActivated != null) ? x.AttributeActivated.Key == prop.Name : false) &&
                                        x.AttributeActivated.Tabel == Helpers.Constants.TablesNames.TLImwDish.ToString() &&
                                        x.Enable && x.AttributeActivated.DataType != "List" && x.Id != 0);

                                    if (LabelName != null)
                                    {
                                        object PropObject = prop.GetValue(DishsInstallationObject, null);
                                        ((IDictionary<String, Object>)DynamiMW_DishInstallation).Add(new KeyValuePair<string, object>(LabelName.AttributeActivated.Label, PropObject));
                                    }
                                }
                                else
                                {
                                    object PropObject = prop.GetValue(DishsInstallationObject, null);
                                    ((IDictionary<String, Object>)DynamiMW_DishInstallation).Add(new KeyValuePair<string, object>(prop.Name, PropObject));
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
                            !x.disable && x.tablesNames.TableName == Helpers.Constants.TablesNames.TLImwDish.ToString() &&
                            !x.LibraryAtt && x.DataType.Name.ToLower() != "datetime" &&
                            NotDateTimeDynamicInstallationAttributesViewModel.Select(y => y.DynamicAttId).Any(y => y == x.Id)
                                , x => x.tablesNames, x => x.DataType).ToList();

                        List<TLIdynamicAttInstValue> NotDateTimeDynamicAttInstValues = _unitOfWork.DynamicAttInstValueRepository.GetIncludeWhere(x =>
                            !x.DynamicAtt.LibraryAtt && !x.disable &&
                            x.InventoryId == DishsInstallationObject.Id &&
                            NotDateTimeInstallationDynamicAttributes.AsEnumerable().Select(y => y.Key.ToLower()).Contains(x.DynamicAtt.Key.ToLower()) &&
                            x.tablesNames.TableName == Helpers.Constants.TablesNames.TLImwDish.ToString()
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

                                ((IDictionary<String, Object>)DynamiMW_DishInstallation).Add(new KeyValuePair<string, object>(InstallationDynamicAtt.Key, DynamicAttValue));
                            }
                            else
                            {
                                ((IDictionary<String, Object>)DynamiMW_DishInstallation).Add(new KeyValuePair<string, object>(InstallationDynamicAtt.Key, null));
                            }
                        }
                    }

                    //
                    // Installation Object ViewModel... (DateTime DataType Attribute)
                    //
                    dynamic DateTimeAttributes = new ExpandoObject();
                    if (DateTimeInstallationAttributesViewModel != null ? DateTimeInstallationAttributesViewModel.Count() > 0 : false)
                    {
                        List<PropertyInfo> DateTimeInstallationProps = typeof(MW_DishViewModel).GetProperties().Where(x =>
                            x.PropertyType.GenericTypeArguments != null ?
                                (x.PropertyType.GenericTypeArguments.Count() > 0 ? x.PropertyType.GenericTypeArguments.FirstOrDefault().Name.ToLower() == "datetime" :
                                (x.PropertyType.Name.ToLower() == "datetime")) :
                            (x.PropertyType.Name.ToLower() == "datetime")).ToList();

                        foreach (PropertyInfo prop in DateTimeInstallationProps)
                        {
                            TLIattributeViewManagment LabelName = AllAttributes.FirstOrDefault(x => ((x.AttributeActivated != null) ? x.AttributeActivated.Key == prop.Name : false) &&
                                x.AttributeActivated.Tabel == Helpers.Constants.TablesNames.TLImwDish.ToString() &&
                                x.Enable && x.AttributeActivated.DataType != "List" && x.Id != 0);

                            if (LabelName != null)
                            {
                                object PropObject = prop.GetValue(DishsInstallationObject, null);
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
                           !x.disable && x.tablesNames.TableName == Helpers.Constants.TablesNames.TLImwDish.ToString() &&
                           !x.LibraryAtt && x.DataType.Name.ToLower() == "datetime" &&
                            DateTimeDynamicInstallationAttributesViewModel.Select(y => y.DynamicAttId).Any(y => y == x.Id), x => x.tablesNames).ToList();

                        List<TLIdynamicAttInstValue> DateTimeDynamicAttInstValues = _unitOfWork.DynamicAttInstValueRepository.GetIncludeWhere(x =>
                            x.InventoryId == DishsInstallationObject.Id && !x.disable &&
                           !x.DynamicAtt.LibraryAtt &&
                            DateTimeInstallationDynamicAttributes.AsEnumerable().Select(y => y.Key.ToLower()).Contains(x.DynamicAtt.Key.ToLower()) &&
                            x.tablesNames.TableName == Helpers.Constants.TablesNames.TLImwDish.ToString()
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

                    ((IDictionary<String, Object>)DynamiMW_DishInstallation).Add(new KeyValuePair<string, object>("DateTimeAttributes", DateTimeAttributes));

                    OutPutList.Add(DynamiMW_DishInstallation);
                }
                MW_DishesTableDisplay.Model = OutPutList;

                if (WithFilterData == true)
                {
                    MW_DishesTableDisplay.filters = _unitOfWork.MW_DishRepository.GetRelatedTables();
                }
                else
                {
                    MW_DishesTableDisplay.filters = null;
                }

                return new Response<ReturnWithFilters<object>>(true, MW_DishesTableDisplay, null, null, (int)Helpers.Constants.ApiReturnCode.success, Count);
            }
            catch (Exception err)
            {
                return new Response<ReturnWithFilters<object>>(false, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
        }

        // 2. Get MW_BU...
        public Response<ReturnWithFilters<object>> GetMW_BUOnSiteWithEnableAtt(LoadsOnSiteFilter BaseFilter, bool WithFilterData, CombineFilters CombineFilters, ParameterPagination parameterPagination, int? CivilId, string CivilType)
        {
            try
            {
                int Count = 0;

                List<FilterObjectList> ObjectAttributeFilters = CombineFilters.filters;
                List<DateFilterViewModel> DateFilter = CombineFilters.DateFilter;

                List<object> OutPutList = new List<object>();
                ReturnWithFilters<object> MW_BUesTableDisplay = new ReturnWithFilters<object>();

                List<TLIcivilLoads> CivilLoadsRecords = new List<TLIcivilLoads>();

                List<StringFilterObjectList> AttributeFilters = new List<StringFilterObjectList>();
                List<AttributeActivatedViewModel> MW_BUInstallationAttribute = new List<AttributeActivatedViewModel>();

                if ((DateFilter != null ? DateFilter.Count() > 0 : false) ||
                    (ObjectAttributeFilters != null && ObjectAttributeFilters.Count > 0))
                {
                    MW_BUInstallationAttribute = _mapper.Map<List<AttributeActivatedViewModel>>(_unitOfWork.AttributeViewManagmentRepository.GetIncludeWhere(x =>
                        x.Enable && x.AttributeActivatedId != null &&
                        x.EditableManagmentView.View == Helpers.Constants.EditableManamgmantViewNames.MW_BUInstallation.ToString() &&
                        x.EditableManagmentView.TLItablesNames1.TableName == Helpers.Constants.TablesNames.TLImwBU.ToString(),
                            x => x.AttributeActivated, x => x.EditableManagmentView, x => x.EditableManagmentView.TLItablesNames1,
                            x => x.EditableManagmentView.TLItablesNames2)
                    .Select(x => x.AttributeActivated).ToList());
                }

                if (ObjectAttributeFilters != null && ObjectAttributeFilters.Count > 0)
                {
                    List<AttributeActivatedViewModel> NotDateDateMW_BUInstallationAttribute = MW_BUInstallationAttribute.Where(x =>
                        x.DataType.ToLower() != "datetime").ToList();

                    foreach (FilterObjectList item in ObjectAttributeFilters)
                    {
                        List<string> value = item.value.Select(x => x.ToString().ToLower()).ToList();

                        AttributeActivatedViewModel AttributeKey = NotDateDateMW_BUInstallationAttribute.FirstOrDefault(x =>
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
                    List<AttributeActivatedViewModel> DateMW_BUInstallationAttribute = MW_BUInstallationAttribute.Where(x =>
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

                        AttributeActivatedViewModel AttributeKey = DateMW_BUInstallationAttribute.FirstOrDefault(x =>
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

                List<int> MW_BUIds = new List<int>();
                List<int> WithoutDateFilterMW_BUInstallation = new List<int>();
                List<int> WithDateFilterMW_BUInstallation = new List<int>();

                if (AttributeFilters != null && AttributeFilters.Count > 0)
                {
                    //
                    // Installation Dynamic Attributes...
                    //
                    List<TLIdynamicAtt> InstDynamicAttListIds = _unitOfWork.DynamicAttRepository.GetIncludeWhere(x =>
                        AttributeFilters.AsEnumerable().Select(y => y.key.ToLower()).Contains(x.Key.ToLower()) &&
                        !x.LibraryAtt && !x.disable &&
                        x.tablesNames.TableName == Helpers.Constants.TablesNames.TLImwBU.ToString()
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
                    bool AttrInstExist = typeof(MW_BUViewModel).GetProperties().ToList().Exists(x =>
                        AttributeFilters.Where(y => y.key.ToLower() != "Id".ToLower()).Select(y =>
                            y.key.ToLower()).Contains(x.Name.ToLower()));

                    List<int> InstallationAttributeActivated = new List<int>();
                    if (AttrInstExist)
                    {
                        List<PropertyInfo> NotStringProps = typeof(MW_BUViewModel).GetProperties().Where(x =>
                            x.PropertyType.Name.ToLower() != "string" &&
                                AttributeFilters.Select(y =>
                                    y.key.ToLower()).Contains(x.Name.ToLower())).ToList();

                        List<PropertyInfo> StringProps = typeof(MW_BUViewModel).GetProperties().Where(x =>
                            x.PropertyType.Name.ToLower() == "string" &&
                                AttributeFilters.Select(y =>
                                    y.key.ToLower()).Contains(x.Name.ToLower())).ToList();

                        List<StringFilterObjectList> AttrInstAttributeFilters = AttributeFilters.Where(x =>
                            NotStringProps.Select(y => y.Name.ToLower()).Contains(x.key.ToLower()) ||
                            StringProps.Select(y => y.Name.ToLower()).Contains(x.key.ToLower())).ToList();

                        //InstallationAttributeActivated = _unitOfWork.MW_BURepository.GetWhere(x =>
                        //    AttrInstAttributeFilters.All(z =>
                        //    NotStringProps.Exists(y => (z.key.ToLower() == y.Name.ToLower()) && (y.GetValue(_mapper.Map<MW_BUViewModel>(x), null) != null ? z.value.Contains(y.GetValue(_mapper.Map<MW_BUViewModel>(x), null).ToString().ToLower()) : false)) ||
                        //    StringProps.Exists(y => (z.key.ToLower() == y.Name.ToLower()) && (z.value.Any(w =>
                        //            y.GetValue(_mapper.Map<MW_BUViewModel>(x), null) != null ? y.GetValue(_mapper.Map<MW_BUViewModel>(x), null).ToString().ToLower().StartsWith(w.ToLower()) : false))))
                        //).Select(i => i.Id).ToList();

                        IEnumerable<TLImwBU> Installations = _unitOfWork.MW_BURepository.GetAllWithoutCount();

                        foreach (StringFilterObjectList InstallationProp in AttrInstAttributeFilters)
                        {
                            if (StringProps.Select(x => x.Name.ToLower()).Contains(InstallationProp.key.ToLower()))
                            {
                                Installations = Installations.Where(x => StringProps.AsEnumerable().FirstOrDefault(y => (InstallationProp.key.ToLower() == y.Name.ToLower()) && (InstallationProp.value.AsEnumerable().FirstOrDefault(w =>
                                     y.GetValue(_mapper.Map<MW_BUViewModel>(x), null) != null ? y.GetValue(_mapper.Map<MW_BUViewModel>(x), null).ToString().ToLower().StartsWith(w.ToLower()) : false) != null)) != null).AsEnumerable();
                            }
                            else if (NotStringProps.Select(x => x.Name.ToLower()).Contains(InstallationProp.key.ToLower()))
                            {
                                Installations = Installations.Where(x => NotStringProps.AsEnumerable().FirstOrDefault(y => (InstallationProp.key.ToLower() == y.Name.ToLower()) && (y.GetValue(_mapper.Map<MW_BUViewModel>(x), null) != null ?
                                    InstallationProp.value.AsEnumerable().Contains(y.GetValue(_mapper.Map<MW_BUViewModel>(x), null).ToString().ToLower()) : false)) != null).AsEnumerable();
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
                        WithoutDateFilterMW_BUInstallation = InstallationAttributeActivated.Intersect(DynamicInstValueListIds).ToList();
                    }
                    else if (AttrInstExist)
                    {
                        WithoutDateFilterMW_BUInstallation = InstallationAttributeActivated;
                    }
                    else if (DynamicInstExist)
                    {
                        WithoutDateFilterMW_BUInstallation = DynamicInstValueListIds;
                    }
                }

                if (DateFilter != null ? DateFilter.Count() > 0 : false)
                {
                    List<TLIdynamicAtt> DateTimeInstDynamicAttListIds = _unitOfWork.DynamicAttRepository.GetIncludeWhere(x =>
                        AfterConvertDateFilters.AsEnumerable().Select(y => y.key.ToLower()).Contains(x.Key.ToLower()) &&
                        !x.LibraryAtt && !x.disable &&
                        x.tablesNames.TableName == Helpers.Constants.TablesNames.TLImwBU.ToString()
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
                    List<PropertyInfo> InstallationProps = typeof(MW_BUViewModel).GetProperties().Where(x =>
                        AfterConvertDateFilters.Select(y =>
                            y.key.ToLower()).Contains(x.Name.ToLower())).ToList();

                    List<int> InstallationAttributeActivatedIds = new List<int>();
                    bool AttrInstExist = false;

                    if (InstallationProps != null)
                    {
                        AttrInstExist = true;

                        List<DateFilterViewModel> InstallationPropsAttributeFilters = AfterConvertDateFilters.Where(x =>
                            InstallationProps.Select(y => y.Name.ToLower()).Contains(x.key.ToLower())).ToList();

                        //InstallationAttributeActivatedIds = _unitOfWork.MW_BURepository.GetWhere(x =>
                        //    InstallationPropsAttributeFilters.All(z =>
                        //        (InstallationProps.Exists(y => (z.key.ToLower() == y.Name.ToLower()) && ((y.GetValue(_mapper.Map<MW_BUViewModel>(x), null) != null) ?
                        //            ((z.DateFrom <= Convert.ToDateTime(y.GetValue(_mapper.Map<MW_BUViewModel>(x), null))) &&
                        //             (z.DateTo >= Convert.ToDateTime(y.GetValue(_mapper.Map<MW_BUViewModel>(x), null)))) : (false)))))
                        //).Select(i => i.Id).ToList();

                        IEnumerable<TLImwBU> Installations = _unitOfWork.MW_BURepository.GetAllWithoutCount();

                        foreach (DateFilterViewModel InstallationProp in InstallationPropsAttributeFilters)
                        {
                            Installations = Installations.Where(x => InstallationProps.Exists(y => (InstallationProp.key.ToLower() == y.Name.ToLower()) && ((y.GetValue(_mapper.Map<MW_BUViewModel>(x), null) != null) ?
                                ((InstallationProp.DateFrom.Date >= Convert.ToDateTime(y.GetValue(_mapper.Map<MW_BUViewModel>(x), null)).Date) &&
                                    (InstallationProp.DateTo.Date <= Convert.ToDateTime(y.GetValue(_mapper.Map<MW_BUViewModel>(x), null)).Date)) : (false))));
                        }

                        InstallationAttributeActivatedIds = Installations.Select(x => x.Id).ToList();
                    }

                    //
                    // Installation (Attribute Activated + Dynamic) Attributes...
                    //
                    if (AttrInstExist && DynamicInstExist)
                    {
                        WithDateFilterMW_BUInstallation = InstallationAttributeActivatedIds.Intersect(DynamicInstValueListIds).ToList();
                    }
                    else if (AttrInstExist)
                    {
                        WithDateFilterMW_BUInstallation = InstallationAttributeActivatedIds;
                    }
                    else if (DynamicInstExist)
                    {
                        WithDateFilterMW_BUInstallation = DynamicInstValueListIds;
                    }
                }

                if ((DateFilter != null ? DateFilter.Count() > 0 : false) ||
                    (AttributeFilters != null ? AttributeFilters.Count() > 0 : false))
                {
                    if ((AttributeFilters != null ? AttributeFilters.Count > 0 : false) &&
                            (AfterConvertDateFilters != null ? AfterConvertDateFilters.Count() > 0 : false))
                    {
                        MW_BUIds = WithoutDateFilterMW_BUInstallation.Intersect(WithDateFilterMW_BUInstallation).ToList();
                    }
                    else if (AttributeFilters != null ? AttributeFilters.Count > 0 : false)
                    {
                        MW_BUIds = WithoutDateFilterMW_BUInstallation;
                    }
                    else if (AfterConvertDateFilters != null ? AfterConvertDateFilters.Count() > 0 : false)
                    {
                        MW_BUIds = WithDateFilterMW_BUInstallation;
                    }

                    CivilLoadsRecords = _unitOfWork.CivilLoadsRepository.GetIncludeWhere(x =>
                    (x.allLoadInstId != null ? x.allLoadInst.mwBUId != null : false) &&
                    (!x.Dismantle) &&
                        (x.SiteCode == BaseFilter.SiteCode) &&
                        (BaseFilter.ItemStatusId != null ? (
                            x.allLoadInst != null ? (
                                x.allLoadInst.ItemStatusId != null ?
                                        (x.allLoadInst.ItemStatusId == BaseFilter.ItemStatusId)
                                : false)
                            : false)
                        : true) &&
                        (BaseFilter.TicketId != null ? (
                            x.allLoadInst != null ? (
                                x.allLoadInst.TicketId != null ?
                                        (x.allLoadInst.TicketId == BaseFilter.TicketId)
                                : false)
                            : false)
                        : true) &&
                        (BaseFilter.AllCivilId != null ?
                            (x.allCivilInstId == BaseFilter.AllCivilId)
                        : true) &&

                        MW_BUIds.Contains(x.allLoadInst.mwBUId.Value),
                    x => x.allCivilInst, x => x.allLoadInst, x => x.allLoadInst.mwBU, x => x.allLoadInst.mwBU.InstallationPlace, x => x.allLoadInst.mwBU.MwBULibrary,
                    x => x.allLoadInst.mwBU.MainDish, x => x.allLoadInst.mwBU.Owner, x => x.allLoadInst.mwBU.baseBU).ToList();
                }
                else
                {
                    CivilLoadsRecords = _unitOfWork.CivilLoadsRepository.GetIncludeWhere(x =>
                       (x.allLoadInstId != null ? x.allLoadInst.mwBUId != null : false) &&
                       (!x.Dismantle) &&
                       (x.SiteCode == BaseFilter.SiteCode) &&
                       (BaseFilter.ItemStatusId != null ? (
                           x.allLoadInst != null ? (
                               x.allLoadInst.ItemStatusId != null ?
                                       (x.allLoadInst.ItemStatusId == BaseFilter.ItemStatusId)
                               : false)
                           : false)
                       : true) &&
                       (BaseFilter.TicketId != null ? (
                           x.allLoadInst != null ? (
                               x.allLoadInst.TicketId != null ?
                                       (x.allLoadInst.TicketId == BaseFilter.TicketId)
                               : false)
                           : false)
                       : true) &&
                       (BaseFilter.AllCivilId != null ?
                           (x.allCivilInstId == BaseFilter.AllCivilId)
                       : true),

                   x => x.allCivilInst, x => x.allLoadInst, x => x.allLoadInst.mwBU, x => x.allLoadInst.mwBU.InstallationPlace, x => x.allLoadInst.mwBU.MwBULibrary,
                   x => x.allLoadInst.mwBU.MainDish, x => x.allLoadInst.mwBU.Owner, x => x.allLoadInst.mwBU.baseBU).ToList();
                }

                // Delete Duplicated Objects Based On Installation Date...
                List<TLIcivilLoads> NewList = new List<TLIcivilLoads>();
                foreach (var item in CivilLoadsRecords)
                {
                    TLIcivilLoads CheckIfExist = NewList.FirstOrDefault(x => x.allLoadInst.mwBUId.Value == item.allLoadInst.mwBUId.Value);
                    if (CheckIfExist != null)
                    {
                        if (CheckIfExist.InstallationDate < item.InstallationDate)
                        {
                            NewList.Remove(CheckIfExist);
                            NewList.Add(item);
                        }
                    }
                    else
                    {
                        NewList.Add(item);
                    }
                }
                CivilLoadsRecords = NewList;

                if (CivilId != null)
                {
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

                    List<int> MW_BUesIds = _unitOfWork.CivilLoadsRepository.GetIncludeWhere(x => x.allCivilInstId == AllCivilInst.Id && !x.Dismantle &&
                        (x.allLoadInstId != null ? x.allLoadInst.mwBUId != null : false), x => x.allLoadInst).Select(x => x.allLoadInst.mwBUId.Value).Distinct().ToList();

                    CivilLoadsRecords = CivilLoadsRecords.Where(x => MW_BUesIds.Contains(x.allLoadInst.mwBUId.Value)).ToList();
                }

                if (BaseFilter.SideArmId != null)
                {
                    List<int> MW_BUesIds = _unitOfWork.CivilLoadsRepository.GetIncludeWhere(x => x.sideArmId == BaseFilter.SideArmId && !x.Dismantle &&
                        (x.allLoadInstId != null ? x.allLoadInst.mwBUId != null : false), x => x.allLoadInst).Select(x => x.allLoadInst.mwBUId.Value).Distinct().ToList();

                    CivilLoadsRecords = CivilLoadsRecords.Where(x => MW_BUesIds.Contains(x.allLoadInst.mwBUId.Value)).ToList();
                }

                Count = CivilLoadsRecords.Count();

                CivilLoadsRecords = CivilLoadsRecords.Skip((parameterPagination.PageNumber - 1) * parameterPagination.PageSize).
                     Take(parameterPagination.PageSize).ToList();

                List<MW_BUViewModel> BUs = _mapper.Map<List<MW_BUViewModel>>(CivilLoadsRecords.Select(x => x.allLoadInst.mwBU).ToList());

                foreach (var s in BUs)
                {
                    var bu = _context.TLImwDish.FirstOrDefault(x => x.Id == s.SdDishId);
                    s.SdDish_Name = bu != null ? bu.DishName : "NA";

                }




                List<TLIattributeViewManagment> AllAttributes = _unitOfWork.AttributeViewManagmentRepository.GetIncludeWhere(x =>
                   (x.Enable && x.EditableManagmentView.View == Helpers.Constants.EditableManamgmantViewNames.MW_BUInstallation.ToString() &&
                   (x.AttributeActivatedId != null ?
                        (x.AttributeActivated.Tabel == Helpers.Constants.TablesNames.TLImwBU.ToString() && x.AttributeActivated.enable) :
                        (!x.DynamicAtt.LibraryAtt && !x.DynamicAtt.disable && x.DynamicAtt.tablesNames.TableName == Helpers.Constants.TablesNames.TLImwBU.ToString()))) ||
                    (x.AttributeActivated != null ?
                        ((x.AttributeActivated.Key.ToLower() == "id" || x.AttributeActivated.Key.ToLower() == "active") && x.AttributeActivated.Tabel == Helpers.Constants.TablesNames.TLImwBU.ToString()) : false),
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


                foreach (MW_BUViewModel BUsInstallationObject in BUs)
                {
                    dynamic DynamiMW_BUInstallation = new ExpandoObject();

                    //
                    // Installation Object ViewModel...
                    //
                    if (NotDateTimeInstallationAttributesViewModel != null ? NotDateTimeInstallationAttributesViewModel.Count > 0 : false)
                    {
                        List<PropertyInfo> InstallationProps = typeof(MW_BUViewModel).GetProperties().Where(x =>
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
                                object ForeignKeyNamePropObject = prop.GetValue(BUsInstallationObject, null);
                                ((IDictionary<String, Object>)DynamiMW_BUInstallation).Add(new KeyValuePair<string, object>(prop.Name, ForeignKeyNamePropObject));
                            }
                            else if (NotDateTimeInstallationAttributesViewModel.Select(x =>
                                 x.AttributeActivated.Key.ToLower()).Contains(prop.Name.ToLower()) &&
                                !prop.Name.ToLower().Contains("_name") &&
                                (prop.Name.ToLower().Substring(Math.Max(0, prop.Name.Length - 2)) != "id" || prop.Name.ToLower() == "id"))
                            {
                                if (prop.Name.ToLower() != "id" && prop.Name.ToLower() != "active")
                                {
                                    TLIattributeViewManagment LabelName = AllAttributes.FirstOrDefault(x => ((x.AttributeActivated != null) ? x.AttributeActivated.Key == prop.Name : false) &&
                                        x.AttributeActivated.Tabel == Helpers.Constants.TablesNames.TLImwBU.ToString() &&
                                        x.Enable && x.AttributeActivated.DataType != "List" && x.Id != 0);

                                    if (LabelName != null)
                                    {
                                        object PropObject = prop.GetValue(BUsInstallationObject, null);
                                        ((IDictionary<String, Object>)DynamiMW_BUInstallation).Add(new KeyValuePair<string, object>(LabelName.AttributeActivated.Label, PropObject));
                                    }
                                }
                                else
                                {
                                    object PropObject = prop.GetValue(BUsInstallationObject, null);
                                    ((IDictionary<String, Object>)DynamiMW_BUInstallation).Add(new KeyValuePair<string, object>(prop.Name, PropObject));
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
                            !x.disable && x.tablesNames.TableName == Helpers.Constants.TablesNames.TLImwBU.ToString() &&
                            !x.LibraryAtt && x.DataType.Name.ToLower() != "datetime" &&
                            NotDateTimeDynamicInstallationAttributesViewModel.Select(y => y.DynamicAttId).Any(y => y == x.Id)
                                , x => x.tablesNames, x => x.DataType).ToList();

                        List<TLIdynamicAttInstValue> NotDateTimeDynamicAttInstValues = _unitOfWork.DynamicAttInstValueRepository.GetIncludeWhere(x =>
                            !x.DynamicAtt.LibraryAtt && !x.disable &&
                            x.InventoryId == BUsInstallationObject.Id &&
                            NotDateTimeInstallationDynamicAttributes.AsEnumerable().Select(y => y.Key.ToLower()).Contains(x.DynamicAtt.Key.ToLower()) &&
                            x.tablesNames.TableName == Helpers.Constants.TablesNames.TLImwBU.ToString()
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

                                ((IDictionary<String, Object>)DynamiMW_BUInstallation).Add(new KeyValuePair<string, object>(InstallationDynamicAtt.Key, DynamicAttValue));
                            }
                            else
                            {
                                ((IDictionary<String, Object>)DynamiMW_BUInstallation).Add(new KeyValuePair<string, object>(InstallationDynamicAtt.Key, null));
                            }
                        }
                    }

                    //
                    // Installation Object ViewModel... (DateTime DataType Attribute)
                    //
                    dynamic DateTimeAttributes = new ExpandoObject();
                    if (DateTimeInstallationAttributesViewModel != null ? DateTimeInstallationAttributesViewModel.Count() > 0 : false)
                    {
                        List<PropertyInfo> DateTimeInstallationProps = typeof(MW_BUViewModel).GetProperties().Where(x =>
                            x.PropertyType.GenericTypeArguments != null ?
                                (x.PropertyType.GenericTypeArguments.Count() > 0 ? x.PropertyType.GenericTypeArguments.FirstOrDefault().Name.ToLower() == "datetime" :
                                (x.PropertyType.Name.ToLower() == "datetime")) :
                            (x.PropertyType.Name.ToLower() == "datetime")).ToList();

                        foreach (PropertyInfo prop in DateTimeInstallationProps)
                        {
                            TLIattributeViewManagment LabelName = AllAttributes.FirstOrDefault(x => ((x.AttributeActivated != null) ? x.AttributeActivated.Key == prop.Name : false) &&
                                x.AttributeActivated.Tabel == Helpers.Constants.TablesNames.TLImwBU.ToString() &&
                                x.Enable && x.AttributeActivated.DataType != "List" && x.Id != 0);

                            if (LabelName != null)
                            {
                                object PropObject = prop.GetValue(BUsInstallationObject, null);
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
                           !x.disable && x.tablesNames.TableName == Helpers.Constants.TablesNames.TLImwBU.ToString() &&
                           !x.LibraryAtt && x.DataType.Name.ToLower() == "datetime" &&
                            DateTimeDynamicInstallationAttributesViewModel.Select(y => y.DynamicAttId).Any(y => y == x.Id), x => x.tablesNames).ToList();

                        List<TLIdynamicAttInstValue> DateTimeDynamicAttInstValues = _unitOfWork.DynamicAttInstValueRepository.GetIncludeWhere(x =>
                            x.InventoryId == BUsInstallationObject.Id && !x.disable &&
                           !x.DynamicAtt.LibraryAtt &&
                            DateTimeInstallationDynamicAttributes.AsEnumerable().Select(y => y.Key.ToLower()).Contains(x.DynamicAtt.Key.ToLower()) &&
                            x.tablesNames.TableName == Helpers.Constants.TablesNames.TLImwBU.ToString()
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

                    ((IDictionary<String, Object>)DynamiMW_BUInstallation).Add(new KeyValuePair<string, object>("DateTimeAttributes", DateTimeAttributes));

                    OutPutList.Add(DynamiMW_BUInstallation);
                }
                MW_BUesTableDisplay.Model = OutPutList;

                if (WithFilterData == true)
                {
                    MW_BUesTableDisplay.filters = _unitOfWork.MW_BURepository.GetRelatedTables(BaseFilter.SiteCode);
                }
                else
                {
                    MW_BUesTableDisplay.filters = null;
                }

                return new Response<ReturnWithFilters<object>>(true, MW_BUesTableDisplay, null, null, (int)Helpers.Constants.ApiReturnCode.success, Count);
            }
            catch (Exception err)
            {
                return new Response<ReturnWithFilters<object>>(false, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
        }

        // 3. Get MW_ODU...
        public Response<ReturnWithFilters<object>> GetMW_ODUOnSiteWithEnableAtt(LoadsOnSiteFilter BaseFilter, bool WithFilterData, CombineFilters CombineFilters, ParameterPagination parameterPagination, int? CivilId, string CivilType)
        {
            try
            {
                int Count = 0;
                List<FilterObjectList> ObjectAttributeFilters = CombineFilters.filters;
                List<DateFilterViewModel> DateFilter = CombineFilters.DateFilter;

                List<object> OutPutList = new List<object>();
                ReturnWithFilters<object> MW_ODUsTableDisplay = new ReturnWithFilters<object>();

                List<TLIcivilLoads> CivilLoadsRecords = new List<TLIcivilLoads>();

                List<StringFilterObjectList> AttributeFilters = new List<StringFilterObjectList>();
                List<AttributeActivatedViewModel> MW_ODUInstallationAttribute = new List<AttributeActivatedViewModel>();

                if ((DateFilter != null ? DateFilter.Count() > 0 : false) ||
                    (ObjectAttributeFilters != null && ObjectAttributeFilters.Count > 0))
                {
                    MW_ODUInstallationAttribute = _mapper.Map<List<AttributeActivatedViewModel>>(_unitOfWork.AttributeViewManagmentRepository.GetIncludeWhere(x =>
                        x.Enable && x.AttributeActivatedId != null &&
                        x.EditableManagmentView.View == Helpers.Constants.EditableManamgmantViewNames.MW_ODUInstallation.ToString() &&
                        x.EditableManagmentView.TLItablesNames1.TableName == Helpers.Constants.TablesNames.TLImwODU.ToString(),
                            x => x.AttributeActivated, x => x.EditableManagmentView, x => x.EditableManagmentView.TLItablesNames1,
                            x => x.EditableManagmentView.TLItablesNames2)
                    .Select(x => x.AttributeActivated).ToList());
                }

                if (ObjectAttributeFilters != null && ObjectAttributeFilters.Count > 0)
                {
                    List<AttributeActivatedViewModel> NotDateDateMW_ODUInstallationAttribute = MW_ODUInstallationAttribute.Where(x =>
                        x.DataType.ToLower() != "datetime").ToList();

                    foreach (FilterObjectList item in ObjectAttributeFilters)
                    {
                        List<string> value = item.value.Select(x => x.ToString().ToLower()).ToList();

                        AttributeActivatedViewModel AttributeKey = NotDateDateMW_ODUInstallationAttribute.FirstOrDefault(x =>
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
                    List<AttributeActivatedViewModel> DateMW_ODUInstallationAttribute = MW_ODUInstallationAttribute.Where(x =>
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

                        AttributeActivatedViewModel AttributeKey = DateMW_ODUInstallationAttribute.FirstOrDefault(x =>
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

                List<int> MW_ODUIds = new List<int>();
                List<int> WithoutDateFilterMW_ODUInstallation = new List<int>();
                List<int> WithDateFilterMW_ODUInstallation = new List<int>();

                if (AttributeFilters != null && AttributeFilters.Count > 0)
                {
                    //
                    // Installation Dynamic Attributes...
                    //
                    List<TLIdynamicAtt> InstDynamicAttListIds = _unitOfWork.DynamicAttRepository.GetIncludeWhere(x =>
                        AttributeFilters.AsEnumerable().Select(y => y.key.ToLower()).Contains(x.Key.ToLower()) &&
                        !x.LibraryAtt && !x.disable &&
                        x.tablesNames.TableName == Helpers.Constants.TablesNames.TLImwODU.ToString()
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
                    bool AttrInstExist = typeof(MW_ODUViewModel).GetProperties().ToList().Exists(x =>
                        AttributeFilters.Where(y => y.key.ToLower() != "Id".ToLower()).Select(y =>
                            y.key.ToLower()).Contains(x.Name.ToLower()));

                    List<int> InstallationAttributeActivated = new List<int>();
                    if (AttrInstExist)
                    {
                        List<PropertyInfo> NotStringProps = typeof(MW_ODUViewModel).GetProperties().Where(x =>
                            x.PropertyType.Name.ToLower() != "string" &&
                                AttributeFilters.Select(y =>
                                    y.key.ToLower()).Contains(x.Name.ToLower())).ToList();

                        List<PropertyInfo> StringProps = typeof(MW_ODUViewModel).GetProperties().Where(x =>
                            x.PropertyType.Name.ToLower() == "string" &&
                                AttributeFilters.Select(y =>
                                    y.key.ToLower()).Contains(x.Name.ToLower())).ToList();

                        List<StringFilterObjectList> AttrInstAttributeFilters = AttributeFilters.Where(x =>
                            NotStringProps.Select(y => y.Name.ToLower()).Contains(x.key.ToLower()) ||
                            StringProps.Select(y => y.Name.ToLower()).Contains(x.key.ToLower())).ToList();

                        IEnumerable<TLImwODU> Installations = _unitOfWork.MW_ODURepository.GetAllWithoutCount();

                        foreach (StringFilterObjectList InstallationProp in AttrInstAttributeFilters)
                        {
                            if (StringProps.Select(x => x.Name.ToLower()).Contains(InstallationProp.key.ToLower()))
                            {
                                Installations = Installations.Where(x => StringProps.AsEnumerable().FirstOrDefault(y => (InstallationProp.key.ToLower() == y.Name.ToLower()) && (InstallationProp.value.AsEnumerable().FirstOrDefault(w =>
                                     y.GetValue(_mapper.Map<MW_ODUViewModel>(x), null) != null ? y.GetValue(_mapper.Map<MW_ODUViewModel>(x), null).ToString().ToLower().StartsWith(w.ToLower()) : false) != null)) != null).AsEnumerable();
                            }
                            else if (NotStringProps.Select(x => x.Name.ToLower()).Contains(InstallationProp.key.ToLower()))
                            {
                                Installations = Installations.Where(x => NotStringProps.AsEnumerable().FirstOrDefault(y => (InstallationProp.key.ToLower() == y.Name.ToLower()) && (y.GetValue(_mapper.Map<MW_ODUViewModel>(x), null) != null ?
                                    InstallationProp.value.AsEnumerable().Contains(y.GetValue(_mapper.Map<MW_ODUViewModel>(x), null).ToString().ToLower()) : false)) != null).AsEnumerable();
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
                        WithoutDateFilterMW_ODUInstallation = InstallationAttributeActivated.Intersect(DynamicInstValueListIds).ToList();
                    }
                    else if (AttrInstExist)
                    {
                        WithoutDateFilterMW_ODUInstallation = InstallationAttributeActivated;
                    }
                    else if (DynamicInstExist)
                    {
                        WithoutDateFilterMW_ODUInstallation = DynamicInstValueListIds;
                    }
                }

                if (DateFilter != null ? DateFilter.Count() > 0 : false)
                {
                    List<TLIdynamicAtt> DateTimeInstDynamicAttListIds = _unitOfWork.DynamicAttRepository.GetIncludeWhere(x =>
                        AfterConvertDateFilters.AsEnumerable().Select(y => y.key.ToLower()).Contains(x.Key.ToLower()) &&
                        !x.LibraryAtt && !x.disable &&
                        x.tablesNames.TableName == Helpers.Constants.TablesNames.TLImwODU.ToString()
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
                    List<PropertyInfo> InstallationProps = typeof(MW_ODUViewModel).GetProperties().Where(x =>
                        AfterConvertDateFilters.Select(y =>
                            y.key.ToLower()).Contains(x.Name.ToLower())).ToList();

                    List<int> InstallationAttributeActivatedIds = new List<int>();
                    bool AttrInstExist = false;

                    if (InstallationProps != null)
                    {
                        AttrInstExist = true;

                        List<DateFilterViewModel> InstallationPropsAttributeFilters = AfterConvertDateFilters.Where(x =>
                            InstallationProps.Select(y => y.Name.ToLower()).Contains(x.key.ToLower())).ToList();

                        IEnumerable<TLImwODU> Installations = _unitOfWork.MW_ODURepository.GetAllWithoutCount();

                        foreach (DateFilterViewModel InstallationProp in InstallationPropsAttributeFilters)
                        {
                            Installations = Installations.Where(x => InstallationProps.Exists(y => (InstallationProp.key.ToLower() == y.Name.ToLower()) && ((y.GetValue(_mapper.Map<MW_ODUViewModel>(x), null) != null) ?
                                ((InstallationProp.DateFrom.Date <= Convert.ToDateTime(y.GetValue(_mapper.Map<MW_ODUViewModel>(x), null)).Date) &&
                                    (InstallationProp.DateTo.Date >= Convert.ToDateTime(y.GetValue(_mapper.Map<MW_ODUViewModel>(x), null)).Date)) : (false))));
                        }

                        InstallationAttributeActivatedIds = Installations.Select(x => x.Id).ToList();
                    }

                    //
                    // Installation (Attribute Activated + Dynamic) Attributes...
                    //
                    if (AttrInstExist && DynamicInstExist)
                    {
                        WithDateFilterMW_ODUInstallation = InstallationAttributeActivatedIds.Intersect(DynamicInstValueListIds).ToList();
                    }
                    else if (AttrInstExist)
                    {
                        WithDateFilterMW_ODUInstallation = InstallationAttributeActivatedIds;
                    }
                    else if (DynamicInstExist)
                    {
                        WithDateFilterMW_ODUInstallation = DynamicInstValueListIds;
                    }
                }

                if ((DateFilter != null ? DateFilter.Count() > 0 : false) ||
                    (AttributeFilters != null ? AttributeFilters.Count() > 0 : false))
                {
                    if ((AttributeFilters != null ? AttributeFilters.Count > 0 : false) &&
                            (AfterConvertDateFilters != null ? AfterConvertDateFilters.Count() > 0 : false))
                    {
                        MW_ODUIds = WithoutDateFilterMW_ODUInstallation.Intersect(WithDateFilterMW_ODUInstallation).ToList();
                    }
                    else if (AttributeFilters != null ? AttributeFilters.Count > 0 : false)
                    {
                        MW_ODUIds = WithoutDateFilterMW_ODUInstallation;
                    }
                    else if (AfterConvertDateFilters != null ? AfterConvertDateFilters.Count() > 0 : false)
                    {
                        MW_ODUIds = WithDateFilterMW_ODUInstallation;
                    }

                    CivilLoadsRecords = _unitOfWork.CivilLoadsRepository.GetIncludeWhere(x =>
                        (x.allLoadInstId != null ? x.allLoadInst.mwODUId != null : false) &&
                        (!x.Dismantle) &&
                        (x.SiteCode == BaseFilter.SiteCode) &&
                        (BaseFilter.ItemStatusId != null ? (
                            x.allLoadInst != null ? (
                                x.allLoadInst.ItemStatusId != null ?
                                        (x.allLoadInst.ItemStatusId == BaseFilter.ItemStatusId)
                                : false)
                            : false)
                        : true) &&
                        (BaseFilter.TicketId != null ? (
                            x.allLoadInst != null ? (
                                x.allLoadInst.TicketId != null ?
                                        (x.allLoadInst.TicketId == BaseFilter.TicketId)
                                : false)
                            : false)
                        : true) &&
                        (BaseFilter.AllCivilId != null ?
                            (x.allCivilInstId == BaseFilter.AllCivilId)
                        : true) &&

                        MW_ODUIds.Contains(x.allLoadInst.mwODUId.Value),
                    x => x.allCivilInst, x => x.allLoadInst, x => x.allLoadInst.mwODU, x => x.allLoadInst.mwODU.Mw_Dish, x => x.allLoadInst.mwODU.MwODULibrary,
                    x => x.allLoadInst.mwODU.Owner, x => x.allLoadInst.mwODU.OduInstallationType).ToList();
                }
                else
                {
                    CivilLoadsRecords = _unitOfWork.CivilLoadsRepository.GetIncludeWhere(x =>
                        (x.allLoadInstId != null ? x.allLoadInst.mwODUId != null : false) &&
                        (!x.Dismantle) &&
                        (x.SiteCode == BaseFilter.SiteCode) &&
                        (BaseFilter.ItemStatusId != null ? (
                            x.allLoadInst != null ? (
                                x.allLoadInst.ItemStatusId != null ?
                                        (x.allLoadInst.ItemStatusId == BaseFilter.ItemStatusId)
                                : false)
                            : false)
                        : true) &&
                        (BaseFilter.TicketId != null ? (
                            x.allLoadInst != null ? (
                                x.allLoadInst.TicketId != null ?
                                        (x.allLoadInst.TicketId == BaseFilter.TicketId)
                                : false)
                            : false)
                        : true) &&
                        (BaseFilter.AllCivilId != null ?
                            (x.allCivilInstId == BaseFilter.AllCivilId)
                        : true),

                    x => x.allCivilInst, x => x.allLoadInst, x => x.allLoadInst.mwODU, x => x.allLoadInst.mwODU.Mw_Dish, x => x.allLoadInst.mwODU.MwODULibrary,
                    x => x.allLoadInst.mwODU.Owner, x => x.allLoadInst.mwODU.OduInstallationType).ToList();
                }

                // Delete Duplicated Objects Based On Installation Date...
                List<TLIcivilLoads> NewList = new List<TLIcivilLoads>();
                foreach (var item in CivilLoadsRecords)
                {
                    TLIcivilLoads CheckIfExist = NewList.FirstOrDefault(x => x.allLoadInst.mwODUId.Value == item.allLoadInst.mwODUId.Value);
                    if (CheckIfExist != null)
                    {
                        if (CheckIfExist.InstallationDate < item.InstallationDate)
                        {
                            NewList.Remove(CheckIfExist);
                            NewList.Add(item);
                        }
                    }
                    else
                    {
                        NewList.Add(item);
                    }
                }
                CivilLoadsRecords = NewList;

                if (CivilId != null)
                {
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

                    List<int> MW_ODUesIds = _unitOfWork.CivilLoadsRepository.GetIncludeWhere(x => x.allCivilInstId == AllCivilInst.Id && !x.Dismantle &&
                        (x.allLoadInstId != null ? x.allLoadInst.mwODUId != null : false), x => x.allLoadInst).Select(x => x.allLoadInst.mwODUId.Value).Distinct().ToList();

                    CivilLoadsRecords = CivilLoadsRecords.Where(x => MW_ODUesIds.Contains(x.allLoadInst.mwODUId.Value)).ToList();
                }

                if (BaseFilter.SideArmId != null)
                {
                    List<int> MW_ODUesIds = _unitOfWork.CivilLoadsRepository.GetIncludeWhere(x => x.sideArmId == BaseFilter.SideArmId && !x.Dismantle &&
                        (x.allLoadInstId != null ? x.allLoadInst.mwODUId != null : false), x => x.allLoadInst).Select(x => x.allLoadInst.mwODUId.Value).Distinct().ToList();

                    CivilLoadsRecords = CivilLoadsRecords.Where(x => MW_ODUesIds.Contains(x.allLoadInst.mwODUId.Value)).ToList();
                }

                Count = CivilLoadsRecords.Count();

                CivilLoadsRecords = CivilLoadsRecords.Skip((parameterPagination.PageNumber - 1) * parameterPagination.PageSize).
                    Take(parameterPagination.PageSize).ToList();

                List<MW_ODUViewModel> ODUs = _mapper.Map<List<MW_ODUViewModel>>(CivilLoadsRecords.Select(x => x.allLoadInst.mwODU).ToList());

                List<TLIattributeViewManagment> AllAttributes = _unitOfWork.AttributeViewManagmentRepository.GetIncludeWhere(x =>
                   (x.Enable && x.EditableManagmentView.View == Helpers.Constants.EditableManamgmantViewNames.MW_ODUInstallation.ToString() &&
                   (x.AttributeActivatedId != null ?
                        (x.AttributeActivated.Tabel == Helpers.Constants.TablesNames.TLImwODU.ToString() && x.AttributeActivated.enable) :
                        (!x.DynamicAtt.LibraryAtt && !x.DynamicAtt.disable && x.DynamicAtt.tablesNames.TableName == Helpers.Constants.TablesNames.TLImwODU.ToString()))) ||
                    (x.AttributeActivated != null ?
                        ((x.AttributeActivated.Key.ToLower() == "id" || x.AttributeActivated.Key.ToLower() == "active") && x.AttributeActivated.Tabel == Helpers.Constants.TablesNames.TLImwODU.ToString()) : false),
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


                foreach (MW_ODUViewModel ODUsInstallationObject in ODUs)
                {
                    dynamic DynamiMW_ODUInstallation = new ExpandoObject();

                    //
                    // Installation Object ViewModel...
                    //
                    if (NotDateTimeInstallationAttributesViewModel != null ? NotDateTimeInstallationAttributesViewModel.Count > 0 : false)
                    {
                        List<PropertyInfo> InstallationProps = typeof(MW_ODUViewModel).GetProperties().Where(x =>
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
                                object ForeignKeyNamePropObject = prop.GetValue(ODUsInstallationObject, null);
                                ((IDictionary<String, Object>)DynamiMW_ODUInstallation).Add(new KeyValuePair<string, object>(prop.Name, ForeignKeyNamePropObject));
                            }
                            else if (NotDateTimeInstallationAttributesViewModel.Select(x =>
                                 x.AttributeActivated.Key.ToLower()).Contains(prop.Name.ToLower()) &&
                                !prop.Name.ToLower().Contains("_name") &&
                                (prop.Name.ToLower().Substring(Math.Max(0, prop.Name.Length - 2)) != "id" || prop.Name.ToLower() == "id"))
                            {
                                if (prop.Name.ToLower() == "ODUConnections".ToLower())
                                {
                                    TLIattributeViewManagment LabelName = AllAttributes.FirstOrDefault(x => ((x.AttributeActivated != null) ? x.AttributeActivated.Key == prop.Name : false) &&
                                        x.AttributeActivated.Tabel == Helpers.Constants.TablesNames.TLImwODU.ToString() &&
                                        x.Enable && x.AttributeActivated.DataType != "List" && x.Id != 0);

                                    if (LabelName != null)
                                    {
                                        if (!string.IsNullOrEmpty(prop.GetValue(ODUsInstallationObject, null).ToString()))
                                        {
                                            object PropObject = prop.GetValue(ODUsInstallationObject, null);
                                            ((IDictionary<String, Object>)DynamiMW_ODUInstallation).Add(new KeyValuePair<string, object>(LabelName.AttributeActivated.Label, PropObject.ToString()));
                                        }
                                        else
                                        {
                                            ((IDictionary<String, Object>)DynamiMW_ODUInstallation).Add(new KeyValuePair<string, object>(LabelName.AttributeActivated.Label, null));
                                        }
                                    }
                                }
                                else if (prop.Name.ToLower() != "id" && prop.Name.ToLower() != "active")
                                {
                                    TLIattributeViewManagment LabelName = AllAttributes.FirstOrDefault(x => ((x.AttributeActivated != null) ? x.AttributeActivated.Key == prop.Name : false) &&
                                        x.AttributeActivated.Tabel == Helpers.Constants.TablesNames.TLImwODU.ToString() &&
                                        x.Enable && x.AttributeActivated.DataType != "List" && x.Id != 0);

                                    if (LabelName != null)
                                    {
                                        object PropObject = prop.GetValue(ODUsInstallationObject, null);
                                        ((IDictionary<String, Object>)DynamiMW_ODUInstallation).Add(new KeyValuePair<string, object>(LabelName.AttributeActivated.Label, PropObject));
                                    }
                                }
                                else
                                {
                                    object PropObject = prop.GetValue(ODUsInstallationObject, null);
                                    ((IDictionary<String, Object>)DynamiMW_ODUInstallation).Add(new KeyValuePair<string, object>(prop.Name, PropObject));
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
                            !x.disable && x.tablesNames.TableName == Helpers.Constants.TablesNames.TLImwODU.ToString() &&
                            !x.LibraryAtt && x.DataType.Name.ToLower() != "datetime" &&
                            NotDateTimeDynamicInstallationAttributesViewModel.Select(y => y.DynamicAttId).Any(y => y == x.Id)
                                , x => x.tablesNames, x => x.DataType).ToList();

                        List<TLIdynamicAttInstValue> NotDateTimeDynamicAttInstValues = _unitOfWork.DynamicAttInstValueRepository.GetIncludeWhere(x =>
                            !x.DynamicAtt.LibraryAtt && !x.disable &&
                            x.InventoryId == ODUsInstallationObject.Id &&
                            NotDateTimeInstallationDynamicAttributes.AsEnumerable().Select(y => y.Key.ToLower()).Contains(x.DynamicAtt.Key.ToLower()) &&
                            x.tablesNames.TableName == Helpers.Constants.TablesNames.TLImwODU.ToString()
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

                                ((IDictionary<String, Object>)DynamiMW_ODUInstallation).Add(new KeyValuePair<string, object>(InstallationDynamicAtt.Key, DynamicAttValue));
                            }
                            else
                            {
                                ((IDictionary<String, Object>)DynamiMW_ODUInstallation).Add(new KeyValuePair<string, object>(InstallationDynamicAtt.Key, null));
                            }
                        }
                    }

                    //
                    // Installation Object ViewModel... (DateTime DataType Attribute)
                    //
                    dynamic DateTimeAttributes = new ExpandoObject();
                    if (DateTimeInstallationAttributesViewModel != null ? DateTimeInstallationAttributesViewModel.Count() > 0 : false)
                    {
                        List<PropertyInfo> DateTimeInstallationProps = typeof(MW_ODUViewModel).GetProperties().Where(x =>
                            x.PropertyType.GenericTypeArguments != null ?
                                (x.PropertyType.GenericTypeArguments.Count() > 0 ? x.PropertyType.GenericTypeArguments.FirstOrDefault().Name.ToLower() == "datetime" :
                                (x.PropertyType.Name.ToLower() == "datetime")) :
                            (x.PropertyType.Name.ToLower() == "datetime")).ToList();

                        foreach (PropertyInfo prop in DateTimeInstallationProps)
                        {
                            TLIattributeViewManagment LabelName = AllAttributes.FirstOrDefault(x => ((x.AttributeActivated != null) ? x.AttributeActivated.Key == prop.Name : false) &&
                                x.AttributeActivated.Tabel == Helpers.Constants.TablesNames.TLImwODU.ToString() &&
                                x.Enable && x.AttributeActivated.DataType != "List" && x.Id != 0);

                            if (LabelName != null)
                            {
                                object PropObject = prop.GetValue(ODUsInstallationObject, null);
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
                           !x.disable && x.tablesNames.TableName == Helpers.Constants.TablesNames.TLImwODU.ToString() &&
                           !x.LibraryAtt && x.DataType.Name.ToLower() == "datetime" &&
                            DateTimeDynamicInstallationAttributesViewModel.Select(y => y.DynamicAttId).Any(y => y == x.Id), x => x.tablesNames).ToList();

                        List<TLIdynamicAttInstValue> DateTimeDynamicAttInstValues = _unitOfWork.DynamicAttInstValueRepository.GetIncludeWhere(x =>
                            x.InventoryId == ODUsInstallationObject.Id && !x.disable &&
                           !x.DynamicAtt.LibraryAtt &&
                            DateTimeInstallationDynamicAttributes.AsEnumerable().Select(y => y.Key.ToLower()).Contains(x.DynamicAtt.Key.ToLower()) &&
                            x.tablesNames.TableName == Helpers.Constants.TablesNames.TLImwODU.ToString()
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

                    ((IDictionary<String, Object>)DynamiMW_ODUInstallation).Add(new KeyValuePair<string, object>("DateTimeAttributes", DateTimeAttributes));

                    OutPutList.Add(DynamiMW_ODUInstallation);
                }
                MW_ODUsTableDisplay.Model = OutPutList;

                if (WithFilterData == true)
                {
                    MW_ODUsTableDisplay.filters = _unitOfWork.MW_ODURepository.GetRelatedTables(BaseFilter.SiteCode);
                }
                else
                {
                    MW_ODUsTableDisplay.filters = null;
                }

                return new Response<ReturnWithFilters<object>>(true, MW_ODUsTableDisplay, null, null, (int)Helpers.Constants.ApiReturnCode.success, Count);
            }
            catch (Exception err)
            {
                return new Response<ReturnWithFilters<object>>(false, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
        }

        // 4. Get MW_RFU
        public Response<ReturnWithFilters<object>> GetMW_RFUOnSiteWithEnableAtt(LoadsOnSiteFilter BaseFilter, bool WithFilterData, CombineFilters CombineFilters, ParameterPagination parameterPagination, int? CivilId, string CivilType)
        {
            try
            {
                int Count = 0;

                List<FilterObjectList> ObjectAttributeFilters = CombineFilters.filters;
                List<DateFilterViewModel> DateFilter = CombineFilters.DateFilter;

                List<object> OutPutList = new List<object>();
                ReturnWithFilters<object> MW_RFUesTableDisplay = new ReturnWithFilters<object>();

                List<TLIcivilLoads> CivilLoadsRecords = new List<TLIcivilLoads>();

                List<StringFilterObjectList> AttributeFilters = new List<StringFilterObjectList>();
                List<AttributeActivatedViewModel> MW_RFUInstallationAttribute = new List<AttributeActivatedViewModel>();

                if ((DateFilter != null ? DateFilter.Count() > 0 : false) ||
                    (ObjectAttributeFilters != null && ObjectAttributeFilters.Count > 0))
                {
                    MW_RFUInstallationAttribute = _mapper.Map<List<AttributeActivatedViewModel>>(_unitOfWork.AttributeViewManagmentRepository.GetIncludeWhere(x =>
                        x.Enable && x.AttributeActivatedId != null &&
                        x.EditableManagmentView.View == Helpers.Constants.EditableManamgmantViewNames.MW_RFUInstallation.ToString() &&
                        x.EditableManagmentView.TLItablesNames1.TableName == Helpers.Constants.TablesNames.TLImwRFU.ToString(),
                            x => x.AttributeActivated, x => x.EditableManagmentView, x => x.EditableManagmentView.TLItablesNames1,
                            x => x.EditableManagmentView.TLItablesNames2)
                    .Select(x => x.AttributeActivated).ToList());
                }

                if (ObjectAttributeFilters != null && ObjectAttributeFilters.Count > 0)
                {
                    List<AttributeActivatedViewModel> NotDateDateMW_RFUInstallationAttribute = MW_RFUInstallationAttribute.Where(x =>
                        x.DataType.ToLower() != "datetime").ToList();

                    foreach (FilterObjectList item in ObjectAttributeFilters)
                    {
                        List<string> value = item.value.Select(x => x.ToString().ToLower()).ToList();

                        AttributeActivatedViewModel AttributeKey = NotDateDateMW_RFUInstallationAttribute.FirstOrDefault(x =>
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
                    List<AttributeActivatedViewModel> DateMW_RFUInstallationAttribute = MW_RFUInstallationAttribute.Where(x =>
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

                        AttributeActivatedViewModel AttributeKey = DateMW_RFUInstallationAttribute.FirstOrDefault(x =>
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

                List<int> MW_RFUIds = new List<int>();
                List<int> WithoutDateFilterMW_RFUInstallation = new List<int>();
                List<int> WithDateFilterMW_RFUInstallation = new List<int>();

                if (AttributeFilters != null && AttributeFilters.Count > 0)
                {
                    //
                    // Installation Dynamic Attributes...
                    //
                    List<TLIdynamicAtt> InstDynamicAttListIds = _unitOfWork.DynamicAttRepository.GetIncludeWhere(x =>
                        AttributeFilters.AsEnumerable().Select(y => y.key.ToLower()).Contains(x.Key.ToLower()) &&
                        !x.LibraryAtt && !x.disable &&
                        x.tablesNames.TableName == Helpers.Constants.TablesNames.TLImwRFU.ToString()
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
                    bool AttrInstExist = typeof(MW_RFUViewModel).GetProperties().ToList().Exists(x =>
                        AttributeFilters.Where(y => y.key.ToLower() != "Id".ToLower()).Select(y =>
                            y.key.ToLower()).Contains(x.Name.ToLower()));

                    List<int> InstallationAttributeActivated = new List<int>();
                    if (AttrInstExist)
                    {
                        List<PropertyInfo> NotStringProps = typeof(MW_RFUViewModel).GetProperties().Where(x =>
                            x.PropertyType.Name.ToLower() != "string" &&
                                AttributeFilters.Select(y =>
                                    y.key.ToLower()).Contains(x.Name.ToLower())).ToList();

                        List<PropertyInfo> StringProps = typeof(MW_RFUViewModel).GetProperties().Where(x =>
                            x.PropertyType.Name.ToLower() == "string" &&
                                AttributeFilters.Select(y =>
                                    y.key.ToLower()).Contains(x.Name.ToLower())).ToList();

                        List<StringFilterObjectList> AttrInstAttributeFilters = AttributeFilters.Where(x =>
                            NotStringProps.Select(y => y.Name.ToLower()).Contains(x.key.ToLower()) ||
                            StringProps.Select(y => y.Name.ToLower()).Contains(x.key.ToLower())).ToList();

                        IEnumerable<TLImwRFU> Installations = _unitOfWork.MW_RFURepository.GetAllWithoutCount();

                        foreach (StringFilterObjectList InstallationProp in AttrInstAttributeFilters)
                        {
                            if (StringProps.Select(x => x.Name.ToLower()).Contains(InstallationProp.key.ToLower()))
                            {
                                Installations = Installations.Where(x => StringProps.AsEnumerable().FirstOrDefault(y => (InstallationProp.key.ToLower() == y.Name.ToLower()) && (InstallationProp.value.AsEnumerable().FirstOrDefault(w =>
                                     y.GetValue(_mapper.Map<MW_RFUViewModel>(x), null) != null ? y.GetValue(_mapper.Map<MW_RFUViewModel>(x), null).ToString().ToLower().StartsWith(w.ToLower()) : false) != null)) != null).AsEnumerable();
                            }
                            else if (NotStringProps.Select(x => x.Name.ToLower()).Contains(InstallationProp.key.ToLower()))
                            {
                                Installations = Installations.Where(x => NotStringProps.AsEnumerable().FirstOrDefault(y => (InstallationProp.key.ToLower() == y.Name.ToLower()) && (y.GetValue(_mapper.Map<MW_RFUViewModel>(x), null) != null ?
                                    InstallationProp.value.AsEnumerable().Contains(y.GetValue(_mapper.Map<MW_RFUViewModel>(x), null).ToString().ToLower()) : false)) != null).AsEnumerable();
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
                        WithoutDateFilterMW_RFUInstallation = InstallationAttributeActivated.Intersect(DynamicInstValueListIds).ToList();
                    }
                    else if (AttrInstExist)
                    {
                        WithoutDateFilterMW_RFUInstallation = InstallationAttributeActivated;
                    }
                    else if (DynamicInstExist)
                    {
                        WithoutDateFilterMW_RFUInstallation = DynamicInstValueListIds;
                    }
                }

                if (DateFilter != null ? DateFilter.Count() > 0 : false)
                {
                    List<TLIdynamicAtt> DateTimeInstDynamicAttListIds = _unitOfWork.DynamicAttRepository.GetIncludeWhere(x =>
                        AfterConvertDateFilters.AsEnumerable().Select(y => y.key.ToLower()).Contains(x.Key.ToLower()) &&
                        !x.LibraryAtt && !x.disable &&
                        x.tablesNames.TableName == Helpers.Constants.TablesNames.TLImwRFU.ToString()
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
                    List<PropertyInfo> InstallationProps = typeof(MW_RFUViewModel).GetProperties().Where(x =>
                        AfterConvertDateFilters.Select(y =>
                            y.key.ToLower()).Contains(x.Name.ToLower())).ToList();

                    List<int> InstallationAttributeActivatedIds = new List<int>();
                    bool AttrInstExist = false;

                    if (InstallationProps != null)
                    {
                        AttrInstExist = true;

                        List<DateFilterViewModel> InstallationPropsAttributeFilters = AfterConvertDateFilters.Where(x =>
                            InstallationProps.Select(y => y.Name.ToLower()).Contains(x.key.ToLower())).ToList();

                        IEnumerable<TLImwRFU> Installations = _unitOfWork.MW_RFURepository.GetAllWithoutCount();

                        foreach (DateFilterViewModel InstallationProp in InstallationPropsAttributeFilters)
                        {
                            Installations = Installations.Where(x => InstallationProps.Exists(y => (InstallationProp.key.ToLower() == y.Name.ToLower()) && ((y.GetValue(_mapper.Map<MW_RFUViewModel>(x), null) != null) ?
                                ((InstallationProp.DateFrom.Date <= Convert.ToDateTime(y.GetValue(_mapper.Map<MW_RFUViewModel>(x), null)).Date) &&
                                    (InstallationProp.DateTo.Date >= Convert.ToDateTime(y.GetValue(_mapper.Map<MW_RFUViewModel>(x), null)).Date)) : (false))));
                        }

                        InstallationAttributeActivatedIds = Installations.Select(x => x.Id).ToList();
                    }

                    //
                    // Installation (Attribute Activated + Dynamic) Attributes...
                    //
                    if (AttrInstExist && DynamicInstExist)
                    {
                        WithDateFilterMW_RFUInstallation = InstallationAttributeActivatedIds.Intersect(DynamicInstValueListIds).ToList();
                    }
                    else if (AttrInstExist)
                    {
                        WithDateFilterMW_RFUInstallation = InstallationAttributeActivatedIds;
                    }
                    else if (DynamicInstExist)
                    {
                        WithDateFilterMW_RFUInstallation = DynamicInstValueListIds;
                    }
                }

                if ((DateFilter != null ? DateFilter.Count() > 0 : false) ||
                    (AttributeFilters != null ? AttributeFilters.Count() > 0 : false))
                {
                    if ((AttributeFilters != null ? AttributeFilters.Count > 0 : false) &&
                            (AfterConvertDateFilters != null ? AfterConvertDateFilters.Count() > 0 : false))
                    {
                        MW_RFUIds = WithoutDateFilterMW_RFUInstallation.Intersect(WithDateFilterMW_RFUInstallation).ToList();
                    }
                    else if (AttributeFilters != null ? AttributeFilters.Count > 0 : false)
                    {
                        MW_RFUIds = WithoutDateFilterMW_RFUInstallation;
                    }
                    else if (AfterConvertDateFilters != null ? AfterConvertDateFilters.Count() > 0 : false)
                    {
                        MW_RFUIds = WithDateFilterMW_RFUInstallation;
                    }

                    CivilLoadsRecords = _unitOfWork.CivilLoadsRepository.GetIncludeWhere(x =>
                        (x.allLoadInstId != null ? x.allLoadInst.mwRFUId != null : false) &&
                        (!x.Dismantle) &&
                        (x.SiteCode == BaseFilter.SiteCode) &&
                        (BaseFilter.ItemStatusId != null ? (
                            x.allLoadInst != null ? (
                                x.allLoadInst.ItemStatusId != null ?
                                        (x.allLoadInst.ItemStatusId == BaseFilter.ItemStatusId)
                                : false)
                            : false)
                        : true) &&
                        (BaseFilter.TicketId != null ? (
                            x.allLoadInst != null ? (
                                x.allLoadInst.TicketId != null ?
                                        (x.allLoadInst.TicketId == BaseFilter.TicketId)
                                : false)
                            : false)
                        : true) &&
                        (BaseFilter.AllCivilId != null ?
                            (x.allCivilInstId == BaseFilter.AllCivilId)
                        : true) &&

                        MW_RFUIds.Contains(x.allLoadInst.mwRFUId.Value),
                    x => x.allCivilInst, x => x.allLoadInst, x => x.allLoadInst.mwRFU, x => x.allLoadInst.mwRFU.MwPort, x => x.allLoadInst.mwRFU.MwRFULibrary,
                    x => x.allLoadInst.mwRFU.Owner).ToList();
                }
                else
                {
                    CivilLoadsRecords = _unitOfWork.CivilLoadsRepository.GetIncludeWhere(x =>
                        (x.allLoadInstId != null ? x.allLoadInst.mwRFUId != null : false) &&
                        (!x.Dismantle) &&
                        (x.SiteCode == BaseFilter.SiteCode) &&
                        (BaseFilter.ItemStatusId != null ? (
                            x.allLoadInst != null ? (
                                x.allLoadInst.ItemStatusId != null ?
                                        (x.allLoadInst.ItemStatusId == BaseFilter.ItemStatusId)
                                : false)
                            : false)
                        : true) &&
                        (BaseFilter.TicketId != null ? (
                            x.allLoadInst != null ? (
                                x.allLoadInst.TicketId != null ?
                                        (x.allLoadInst.TicketId == BaseFilter.TicketId)
                                : false)
                            : false)
                        : true) &&
                        (BaseFilter.AllCivilId != null ?
                            (x.allCivilInstId == BaseFilter.AllCivilId)
                        : true),

                    x => x.allCivilInst, x => x.allLoadInst, x => x.allLoadInst.mwRFU, x => x.allLoadInst.mwRFU.MwPort, x => x.allLoadInst.mwRFU.MwRFULibrary,
                    x => x.allLoadInst.mwRFU.Owner).ToList();
                }
                // Delete Duplicated Objects Based On Installation Date...
                List<TLIcivilLoads> NewList = new List<TLIcivilLoads>();
                foreach (var item in CivilLoadsRecords)
                {
                    TLIcivilLoads CheckIfExist = NewList.FirstOrDefault(x => x.allLoadInst.mwRFUId.Value == item.allLoadInst.mwRFUId.Value);
                    if (CheckIfExist != null)
                    {
                        if (CheckIfExist.InstallationDate < item.InstallationDate)
                        {
                            NewList.Remove(CheckIfExist);
                            NewList.Add(item);
                        }
                    }
                    else
                    {
                        NewList.Add(item);
                    }
                }
                CivilLoadsRecords = NewList;

                if (CivilId != null)
                {
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

                    List<int> MW_RFUesIds = _unitOfWork.CivilLoadsRepository.GetIncludeWhere(x => x.allCivilInstId == AllCivilInst.Id && !x.Dismantle &&
                        (x.allLoadInstId != null ? x.allLoadInst.mwRFUId != null : false), x => x.allLoadInst).Select(x => x.allLoadInst.mwRFUId.Value).Distinct().ToList();

                    CivilLoadsRecords = CivilLoadsRecords.Where(x => MW_RFUesIds.Contains(x.allLoadInst.mwRFUId.Value)).ToList();
                }

                if (BaseFilter.SideArmId != null)
                {
                    List<int> MW_RFUesIds = _unitOfWork.CivilLoadsRepository.GetIncludeWhere(x => x.sideArmId == BaseFilter.SideArmId && !x.Dismantle &&
                        (x.allLoadInstId != null ? x.allLoadInst.mwRFUId != null : false), x => x.allLoadInst).Select(x => x.allLoadInst.mwRFUId.Value).Distinct().ToList();

                    CivilLoadsRecords = CivilLoadsRecords.Where(x => MW_RFUesIds.Contains(x.allLoadInst.mwRFUId.Value)).ToList();
                }

                Count = CivilLoadsRecords.Count();

                CivilLoadsRecords = CivilLoadsRecords.Skip((parameterPagination.PageNumber - 1) * parameterPagination.PageSize).
                    Take(parameterPagination.PageSize).ToList();

                List<MW_RFUViewModel> RFUs = _mapper.Map<List<MW_RFUViewModel>>(CivilLoadsRecords.Select(x => x.allLoadInst.mwRFU).ToList());

                List<TLIattributeViewManagment> AllAttributes = _unitOfWork.AttributeViewManagmentRepository.GetIncludeWhere(x =>
                   (x.Enable && x.EditableManagmentView.View == Helpers.Constants.EditableManamgmantViewNames.MW_RFUInstallation.ToString() &&
                   (x.AttributeActivatedId != null ?
                        (x.AttributeActivated.Tabel == Helpers.Constants.TablesNames.TLImwRFU.ToString() && x.AttributeActivated.enable) :
                        (!x.DynamicAtt.LibraryAtt && !x.DynamicAtt.disable && x.DynamicAtt.tablesNames.TableName == Helpers.Constants.TablesNames.TLImwRFU.ToString()))) ||
                    (x.AttributeActivated != null ?
                        ((x.AttributeActivated.Key.ToLower() == "id" || x.AttributeActivated.Key.ToLower() == "active") && x.AttributeActivated.Tabel == Helpers.Constants.TablesNames.TLImwRFU.ToString()) : false),
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


                foreach (MW_RFUViewModel RFUsInstallationObject in RFUs)
                {
                    dynamic DynamiMW_RFUInstallation = new ExpandoObject();

                    //
                    // Installation Object ViewModel...
                    //
                    if (NotDateTimeInstallationAttributesViewModel != null ? NotDateTimeInstallationAttributesViewModel.Count > 0 : false)
                    {
                        List<PropertyInfo> InstallationProps = typeof(MW_RFUViewModel).GetProperties().Where(x =>
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
                                object ForeignKeyNamePropObject = prop.GetValue(RFUsInstallationObject, null);
                                ((IDictionary<String, Object>)DynamiMW_RFUInstallation).Add(new KeyValuePair<string, object>(prop.Name, ForeignKeyNamePropObject));
                            }
                            else if (NotDateTimeInstallationAttributesViewModel.Select(x =>
                                 x.AttributeActivated.Key.ToLower()).Contains(prop.Name.ToLower()) &&
                                !prop.Name.ToLower().Contains("_name") &&
                                (prop.Name.ToLower().Substring(Math.Max(0, prop.Name.Length - 2)) != "id" || prop.Name.ToLower() == "id"))
                            {
                                if (prop.Name.ToLower() != "id" && prop.Name.ToLower() != "active")
                                {
                                    TLIattributeViewManagment LabelName = AllAttributes.FirstOrDefault(x => ((x.AttributeActivated != null) ? x.AttributeActivated.Key == prop.Name : false) &&
                                        x.AttributeActivated.Tabel == Helpers.Constants.TablesNames.TLImwRFU.ToString() &&
                                        x.Enable && x.AttributeActivated.DataType != "List" && x.Id != 0);

                                    if (LabelName != null)
                                    {
                                        object PropObject = prop.GetValue(RFUsInstallationObject, null);
                                        ((IDictionary<String, Object>)DynamiMW_RFUInstallation).Add(new KeyValuePair<string, object>(LabelName.AttributeActivated.Label, PropObject));
                                    }
                                }
                                else
                                {
                                    object PropObject = prop.GetValue(RFUsInstallationObject, null);
                                    ((IDictionary<String, Object>)DynamiMW_RFUInstallation).Add(new KeyValuePair<string, object>(prop.Name, PropObject));
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
                            !x.disable && x.tablesNames.TableName == Helpers.Constants.TablesNames.TLImwRFU.ToString() &&
                            !x.LibraryAtt && x.DataType.Name.ToLower() != "datetime" &&
                            NotDateTimeDynamicInstallationAttributesViewModel.Select(y => y.DynamicAttId).Any(y => y == x.Id)
                                , x => x.tablesNames, x => x.DataType).ToList();

                        List<TLIdynamicAttInstValue> NotDateTimeDynamicAttInstValues = _unitOfWork.DynamicAttInstValueRepository.GetIncludeWhere(x =>
                            !x.DynamicAtt.LibraryAtt && !x.disable &&
                            x.InventoryId == RFUsInstallationObject.Id &&
                            NotDateTimeInstallationDynamicAttributes.AsEnumerable().Select(y => y.Key.ToLower()).Contains(x.DynamicAtt.Key.ToLower()) &&
                            x.tablesNames.TableName == Helpers.Constants.TablesNames.TLImwRFU.ToString()
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

                                ((IDictionary<String, Object>)DynamiMW_RFUInstallation).Add(new KeyValuePair<string, object>(InstallationDynamicAtt.Key, DynamicAttValue));
                            }
                            else
                            {
                                ((IDictionary<String, Object>)DynamiMW_RFUInstallation).Add(new KeyValuePair<string, object>(InstallationDynamicAtt.Key, null));
                            }
                        }
                    }

                    //
                    // Installation Object ViewModel... (DateTime DataType Attribute)
                    //
                    dynamic DateTimeAttributes = new ExpandoObject();
                    if (DateTimeInstallationAttributesViewModel != null ? DateTimeInstallationAttributesViewModel.Count() > 0 : false)
                    {
                        List<PropertyInfo> DateTimeInstallationProps = typeof(MW_RFUViewModel).GetProperties().Where(x =>
                            x.PropertyType.GenericTypeArguments != null ?
                                (x.PropertyType.GenericTypeArguments.Count() > 0 ? x.PropertyType.GenericTypeArguments.FirstOrDefault().Name.ToLower() == "datetime" :
                                (x.PropertyType.Name.ToLower() == "datetime")) :
                            (x.PropertyType.Name.ToLower() == "datetime")).ToList();

                        foreach (PropertyInfo prop in DateTimeInstallationProps)
                        {
                            TLIattributeViewManagment LabelName = AllAttributes.FirstOrDefault(x => ((x.AttributeActivated != null) ? x.AttributeActivated.Key == prop.Name : false) &&
                                x.AttributeActivated.Tabel == Helpers.Constants.TablesNames.TLImwRFU.ToString() &&
                                x.Enable && x.AttributeActivated.DataType != "List" && x.Id != 0);

                            if (LabelName != null)
                            {
                                object PropObject = prop.GetValue(RFUsInstallationObject, null);
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
                           !x.disable && x.tablesNames.TableName == Helpers.Constants.TablesNames.TLImwRFU.ToString() &&
                           !x.LibraryAtt && x.DataType.Name.ToLower() == "datetime" &&
                            DateTimeDynamicInstallationAttributesViewModel.Select(y => y.DynamicAttId).Any(y => y == x.Id), x => x.tablesNames).ToList();

                        List<TLIdynamicAttInstValue> DateTimeDynamicAttInstValues = _unitOfWork.DynamicAttInstValueRepository.GetIncludeWhere(x =>
                            x.InventoryId == RFUsInstallationObject.Id && !x.disable &&
                           !x.DynamicAtt.LibraryAtt &&
                            DateTimeInstallationDynamicAttributes.AsEnumerable().Select(y => y.Key.ToLower()).Contains(x.DynamicAtt.Key.ToLower()) &&
                            x.tablesNames.TableName == Helpers.Constants.TablesNames.TLImwRFU.ToString()
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

                    ((IDictionary<String, Object>)DynamiMW_RFUInstallation).Add(new KeyValuePair<string, object>("DateTimeAttributes", DateTimeAttributes));

                    OutPutList.Add(DynamiMW_RFUInstallation);
                }
                MW_RFUesTableDisplay.Model = OutPutList;

                if (WithFilterData == true)
                {
                    MW_RFUesTableDisplay.filters = _unitOfWork.MW_RFURepository.GetRelatedTables();
                }
                else
                {
                    MW_RFUesTableDisplay.filters = null;
                }

                return new Response<ReturnWithFilters<object>>(true, MW_RFUesTableDisplay, null, null, (int)Helpers.Constants.ApiReturnCode.success, Count);
            }
            catch (Exception err)
            {
                return new Response<ReturnWithFilters<object>>(false, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
        }

        // 5. Get MW_Other
        public Response<ReturnWithFilters<object>> GetMW_OtherOnSiteWithEnableAtt(LoadsOnSiteFilter BaseFilter, bool WithFilterData, CombineFilters CombineFilters, ParameterPagination parameterPagination, int? CivilId, string CivilType)
        {
            try
            {
                int Count = 0;

                List<FilterObjectList> ObjectAttributeFilters = CombineFilters.filters;
                List<DateFilterViewModel> DateFilter = CombineFilters.DateFilter;

                List<object> OutPutList = new List<object>();
                ReturnWithFilters<object> MW_OtheresTableDisplay = new ReturnWithFilters<object>();

                List<TLIcivilLoads> CivilLoadsRecords = new List<TLIcivilLoads>();

                List<StringFilterObjectList> AttributeFilters = new List<StringFilterObjectList>();
                List<AttributeActivatedViewModel> MW_OtherInstallationAttribute = new List<AttributeActivatedViewModel>();

                if ((DateFilter != null ? DateFilter.Count() > 0 : false) ||
                    (ObjectAttributeFilters != null && ObjectAttributeFilters.Count > 0))
                {
                    MW_OtherInstallationAttribute = _mapper.Map<List<AttributeActivatedViewModel>>(_unitOfWork.AttributeViewManagmentRepository.GetIncludeWhere(x =>
                        x.Enable && x.AttributeActivatedId != null &&
                        x.EditableManagmentView.View == Helpers.Constants.EditableManamgmantViewNames.OtherMWInstallation.ToString() &&
                        x.EditableManagmentView.TLItablesNames1.TableName == Helpers.Constants.TablesNames.TLImwOther.ToString(),
                            x => x.AttributeActivated, x => x.EditableManagmentView, x => x.EditableManagmentView.TLItablesNames1,
                            x => x.EditableManagmentView.TLItablesNames2)
                    .Select(x => x.AttributeActivated).ToList());
                }

                if (ObjectAttributeFilters != null && ObjectAttributeFilters.Count > 0)
                {
                    List<AttributeActivatedViewModel> NotDateDateMW_OtherInstallationAttribute = MW_OtherInstallationAttribute.Where(x =>
                        x.DataType.ToLower() != "datetime").ToList();

                    foreach (FilterObjectList item in ObjectAttributeFilters)
                    {
                        List<string> value = item.value.Select(x => x.ToString().ToLower()).ToList();

                        AttributeActivatedViewModel AttributeKey = NotDateDateMW_OtherInstallationAttribute.FirstOrDefault(x =>
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
                    List<AttributeActivatedViewModel> DateMW_OtherInstallationAttribute = MW_OtherInstallationAttribute.Where(x =>
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

                        AttributeActivatedViewModel AttributeKey = DateMW_OtherInstallationAttribute.FirstOrDefault(x =>
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

                List<int> MW_OtherIds = new List<int>();
                List<int> WithoutDateFilterMW_OtherInstallation = new List<int>();
                List<int> WithDateFilterMW_OtherInstallation = new List<int>();

                if (AttributeFilters != null && AttributeFilters.Count > 0)
                {
                    //
                    // Installation Dynamic Attributes...
                    //
                    List<TLIdynamicAtt> InstDynamicAttListIds = _unitOfWork.DynamicAttRepository.GetIncludeWhere(x =>
                        AttributeFilters.AsEnumerable().Select(y => y.key.ToLower()).Contains(x.Key.ToLower()) &&
                        !x.LibraryAtt && !x.disable &&
                        x.tablesNames.TableName == Helpers.Constants.TablesNames.TLImwOther.ToString()
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
                    bool AttrInstExist = typeof(Mw_OtherViewModel).GetProperties().ToList().Exists(x =>
                        AttributeFilters.Where(y => y.key.ToLower() != "Id".ToLower()).Select(y =>
                            y.key.ToLower()).Contains(x.Name.ToLower()));

                    List<int> InstallationAttributeActivated = new List<int>();
                    if (AttrInstExist)
                    {
                        List<PropertyInfo> NotStringProps = typeof(Mw_OtherViewModel).GetProperties().Where(x =>
                            x.PropertyType.Name.ToLower() != "string" &&
                                AttributeFilters.Select(y =>
                                    y.key.ToLower()).Contains(x.Name.ToLower())).ToList();

                        List<PropertyInfo> StringProps = typeof(Mw_OtherViewModel).GetProperties().Where(x =>
                            x.PropertyType.Name.ToLower() == "string" &&
                                AttributeFilters.Select(y =>
                                    y.key.ToLower()).Contains(x.Name.ToLower())).ToList();

                        List<StringFilterObjectList> AttrInstAttributeFilters = AttributeFilters.Where(x =>
                            NotStringProps.Select(y => y.Name.ToLower()).Contains(x.key.ToLower()) ||
                            StringProps.Select(y => y.Name.ToLower()).Contains(x.key.ToLower())).ToList();

                        IEnumerable<TLImwOther> Installations = _unitOfWork.Mw_OtherRepository.GetAllWithoutCount();

                        foreach (StringFilterObjectList InstallationProp in AttrInstAttributeFilters)
                        {
                            if (StringProps.Select(x => x.Name.ToLower()).Contains(InstallationProp.key.ToLower()))
                            {
                                Installations = Installations.Where(x => StringProps.AsEnumerable().FirstOrDefault(y => (InstallationProp.key.ToLower() == y.Name.ToLower()) && (InstallationProp.value.AsEnumerable().FirstOrDefault(w =>
                                     y.GetValue(_mapper.Map<Mw_OtherViewModel>(x), null) != null ? y.GetValue(_mapper.Map<Mw_OtherViewModel>(x), null).ToString().ToLower().StartsWith(w.ToLower()) : false) != null)) != null).AsEnumerable();
                            }
                            else if (NotStringProps.Select(x => x.Name.ToLower()).Contains(InstallationProp.key.ToLower()))
                            {
                                Installations = Installations.Where(x => NotStringProps.AsEnumerable().FirstOrDefault(y => (InstallationProp.key.ToLower() == y.Name.ToLower()) && (y.GetValue(_mapper.Map<Mw_OtherViewModel>(x), null) != null ?
                                    InstallationProp.value.AsEnumerable().Contains(y.GetValue(_mapper.Map<Mw_OtherViewModel>(x), null).ToString().ToLower()) : false)) != null).AsEnumerable();
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
                        WithoutDateFilterMW_OtherInstallation = InstallationAttributeActivated.Intersect(DynamicInstValueListIds).ToList();
                    }
                    else if (AttrInstExist)
                    {
                        WithoutDateFilterMW_OtherInstallation = InstallationAttributeActivated;
                    }
                    else if (DynamicInstExist)
                    {
                        WithoutDateFilterMW_OtherInstallation = DynamicInstValueListIds;
                    }
                }

                if (DateFilter != null ? DateFilter.Count() > 0 : false)
                {
                    List<TLIdynamicAtt> DateTimeInstDynamicAttListIds = _unitOfWork.DynamicAttRepository.GetIncludeWhere(x =>
                        AfterConvertDateFilters.AsEnumerable().Select(y => y.key.ToLower()).Contains(x.Key.ToLower()) &&
                        !x.LibraryAtt && !x.disable &&
                        x.tablesNames.TableName == Helpers.Constants.TablesNames.TLImwOther.ToString()
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
                    List<PropertyInfo> InstallationProps = typeof(Mw_OtherViewModel).GetProperties().Where(x =>
                        AfterConvertDateFilters.Select(y =>
                            y.key.ToLower()).Contains(x.Name.ToLower())).ToList();

                    List<int> InstallationAttributeActivatedIds = new List<int>();
                    bool AttrInstExist = false;

                    if (InstallationProps != null)
                    {
                        AttrInstExist = true;

                        List<DateFilterViewModel> InstallationPropsAttributeFilters = AfterConvertDateFilters.Where(x =>
                            InstallationProps.Select(y => y.Name.ToLower()).Contains(x.key.ToLower())).ToList();

                        IEnumerable<TLImwOther> Installations = _unitOfWork.Mw_OtherRepository.GetAllWithoutCount();

                        foreach (DateFilterViewModel InstallationProp in InstallationPropsAttributeFilters)
                        {
                            Installations = Installations.Where(x => InstallationProps.Exists(y => (InstallationProp.key.ToLower() == y.Name.ToLower()) && ((y.GetValue(_mapper.Map<Mw_OtherViewModel>(x), null) != null) ?
                                ((InstallationProp.DateFrom.Date <= Convert.ToDateTime(y.GetValue(_mapper.Map<Mw_OtherViewModel>(x), null)).Date) &&
                                    (InstallationProp.DateTo.Date >= Convert.ToDateTime(y.GetValue(_mapper.Map<Mw_OtherViewModel>(x), null)).Date)) : (false))));
                        }

                        InstallationAttributeActivatedIds = Installations.Select(x => x.Id).ToList();
                    }

                    //
                    // Installation (Attribute Activated + Dynamic) Attributes...
                    //
                    if (AttrInstExist && DynamicInstExist)
                    {
                        WithDateFilterMW_OtherInstallation = InstallationAttributeActivatedIds.Intersect(DynamicInstValueListIds).ToList();
                    }
                    else if (AttrInstExist)
                    {
                        WithDateFilterMW_OtherInstallation = InstallationAttributeActivatedIds;
                    }
                    else if (DynamicInstExist)
                    {
                        WithDateFilterMW_OtherInstallation = DynamicInstValueListIds;
                    }
                }

                if ((DateFilter != null ? DateFilter.Count() > 0 : false) ||
                    (AttributeFilters != null ? AttributeFilters.Count() > 0 : false))
                {
                    if ((AttributeFilters != null ? AttributeFilters.Count > 0 : false) &&
                            (AfterConvertDateFilters != null ? AfterConvertDateFilters.Count() > 0 : false))
                    {
                        MW_OtherIds = WithoutDateFilterMW_OtherInstallation.Intersect(WithDateFilterMW_OtherInstallation).ToList();
                    }
                    else if (AttributeFilters != null ? AttributeFilters.Count > 0 : false)
                    {
                        MW_OtherIds = WithoutDateFilterMW_OtherInstallation;
                    }
                    else if (AfterConvertDateFilters != null ? AfterConvertDateFilters.Count() > 0 : false)
                    {
                        MW_OtherIds = WithDateFilterMW_OtherInstallation;
                    }

                    CivilLoadsRecords = _unitOfWork.CivilLoadsRepository.GetIncludeWhere(x =>
                        (x.allLoadInstId != null ? x.allLoadInst.mwOtherId != null : false) &&
                        (!x.Dismantle) &&
                        (x.SiteCode == BaseFilter.SiteCode) &&
                        (BaseFilter.ItemStatusId != null ? (
                            x.allLoadInst != null ? (
                                x.allLoadInst.ItemStatusId != null ?
                                        (x.allLoadInst.ItemStatusId == BaseFilter.ItemStatusId)
                                : false)
                            : false)
                        : true) &&
                        (BaseFilter.TicketId != null ? (
                            x.allLoadInst != null ? (
                                x.allLoadInst.TicketId != null ?
                                        (x.allLoadInst.TicketId == BaseFilter.TicketId)
                                : false)
                            : false)
                        : true) &&
                        (BaseFilter.AllCivilId != null ?
                            (x.allCivilInstId == BaseFilter.AllCivilId)
                        : true) &&

                        MW_OtherIds.Contains(x.allLoadInst.mwOtherId.Value),
                    x => x.allCivilInst, x => x.allLoadInst, x => x.allLoadInst.mwOther, x => x.allLoadInst.mwOther.mwOtherLibrary,
                    x => x.allLoadInst.mwOther.InstallationPlace).ToList();
                }
                else
                {
                    CivilLoadsRecords = _unitOfWork.CivilLoadsRepository.GetIncludeWhere(x =>
                       (x.allLoadInstId != null ? x.allLoadInst.mwOtherId != null : false) &&
                       (!x.Dismantle) &&
                       (x.SiteCode == BaseFilter.SiteCode) &&
                       (BaseFilter.ItemStatusId != null ? (
                           x.allLoadInst != null ? (
                               x.allLoadInst.ItemStatusId != null ?
                                       (x.allLoadInst.ItemStatusId == BaseFilter.ItemStatusId)
                               : false)
                           : false)
                       : true) &&
                       (BaseFilter.TicketId != null ? (
                           x.allLoadInst != null ? (
                               x.allLoadInst.TicketId != null ?
                                       (x.allLoadInst.TicketId == BaseFilter.TicketId)
                               : false)
                           : false)
                       : true) &&
                       (BaseFilter.AllCivilId != null ?
                           (x.allCivilInstId == BaseFilter.AllCivilId)
                       : true),

                    x => x.allCivilInst, x => x.allLoadInst, x => x.allLoadInst.mwOther, x => x.allLoadInst.mwOther.mwOtherLibrary,
                    x => x.allLoadInst.mwOther.InstallationPlace).ToList();
                }

                // Delete Duplicated Objects Based On Installation Date...
                List<TLIcivilLoads> NewList = new List<TLIcivilLoads>();
                foreach (var item in CivilLoadsRecords)
                {
                    TLIcivilLoads CheckIfExist = NewList.FirstOrDefault(x => x.allLoadInst.mwOtherId.Value == item.allLoadInst.mwOtherId.Value);
                    if (CheckIfExist != null)
                    {
                        if (CheckIfExist.InstallationDate < item.InstallationDate)
                        {
                            NewList.Remove(CheckIfExist);
                            NewList.Add(item);
                        }
                    }
                    else
                    {
                        NewList.Add(item);
                    }
                }
                CivilLoadsRecords = NewList;

                if (CivilId != null)
                {
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

                    List<int> MW_OthersIds = _unitOfWork.CivilLoadsRepository.GetIncludeWhere(x => x.allCivilInstId == AllCivilInst.Id && !x.Dismantle &&
                        (x.allLoadInstId != null ? x.allLoadInst.mwOtherId != null : false), x => x.allLoadInst).Select(x => x.allLoadInst.mwOtherId.Value).Distinct().ToList();

                    CivilLoadsRecords = CivilLoadsRecords.Where(x => MW_OthersIds.Contains(x.allLoadInst.mwOtherId.Value)).ToList();
                }

                if (BaseFilter.SideArmId != null)
                {
                    List<int> MW_OthersIds = _unitOfWork.CivilLoadsRepository.GetIncludeWhere(x => x.sideArmId == BaseFilter.SideArmId && !x.Dismantle &&
                        (x.allLoadInstId != null ? x.allLoadInst.mwOtherId != null : false), x => x.allLoadInst).Select(x => x.allLoadInst.mwOtherId.Value).Distinct().ToList();

                    CivilLoadsRecords = CivilLoadsRecords.Where(x => MW_OthersIds.Contains(x.allLoadInst.mwOtherId.Value)).ToList();
                }

                Count = CivilLoadsRecords.Count();

                CivilLoadsRecords = CivilLoadsRecords.Skip((parameterPagination.PageNumber - 1) * parameterPagination.PageSize).
                    Take(parameterPagination.PageSize).ToList();

                List<Mw_OtherViewModel> Others = _mapper.Map<List<Mw_OtherViewModel>>(CivilLoadsRecords.Select(x => x.allLoadInst.mwOther).ToList());

                List<TLIattributeViewManagment> AllAttributes = _unitOfWork.AttributeViewManagmentRepository.GetIncludeWhere(x =>
                   (x.Enable && x.EditableManagmentView.View == Helpers.Constants.EditableManamgmantViewNames.OtherMWInstallation.ToString() &&
                   (x.AttributeActivatedId != null ?
                        (x.AttributeActivated.Tabel == Helpers.Constants.TablesNames.TLImwOther.ToString() && x.AttributeActivated.enable) :
                        (!x.DynamicAtt.LibraryAtt && !x.DynamicAtt.disable && x.DynamicAtt.tablesNames.TableName == Helpers.Constants.TablesNames.TLImwOther.ToString()))) ||
                    (x.AttributeActivated != null ?
                        ((x.AttributeActivated.Key.ToLower() == "id" || x.AttributeActivated.Key.ToLower() == "active") && x.AttributeActivated.Tabel == Helpers.Constants.TablesNames.TLImwOther.ToString()) : false),
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

                foreach (Mw_OtherViewModel OthersInstallationObject in Others)
                {
                    dynamic DynamiMW_OtherInstallation = new ExpandoObject();

                    //
                    // Installation Object ViewModel... (Not DateTime DataType Attribute)
                    //
                    if (NotDateTimeInstallationAttributesViewModel != null ? NotDateTimeInstallationAttributesViewModel.Count > 0 : false)
                    {
                        List<PropertyInfo> InstallationProps = typeof(Mw_OtherViewModel).GetProperties().Where(x =>
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
                                if (prop.Name.ToLower().StartsWith("InstallationPlace_".ToLower()))
                                {
                                    int MW_OtherViewModelId = (int)DynamiMW_OtherInstallation.Id;

                                    TLIcivilLoads? CivilLoads = _unitOfWork.CivilLoadsRepository.GetIncludeWhereFirst(x => !x.Dismantle && (x.allLoadInstId != null ?
                                        !x.allLoadInst.Draft && (x.allLoadInst.mwOtherId != null ? x.allLoadInst.mwOtherId == MW_OtherViewModelId : false) : false),
                                            x => x.allLoadInst, x => x.allLoadInst.mwOther);

                                    if (CivilLoads != null)
                                    {
                                        if (CivilLoads.sideArmId != null)
                                        {
                                            string InstallationPlaceName = _unitOfWork.InstallationPlaceRepository
                                                .GetWhereFirst(x => x.Name.ToLower().StartsWith("Side".ToLower())).Name;

                                            ((IDictionary<String, Object>)DynamiMW_OtherInstallation).Add(new KeyValuePair<string, object>(prop.Name, InstallationPlaceName));
                                        }
                                        else if (CivilLoads.legId != null)
                                        {
                                            string InstallationPlaceName = _unitOfWork.InstallationPlaceRepository
                                                .GetWhereFirst(x => x.Name.ToLower().StartsWith("Leg".ToLower())).Name;

                                            ((IDictionary<String, Object>)DynamiMW_OtherInstallation).Add(new KeyValuePair<string, object>(prop.Name, InstallationPlaceName));
                                        }
                                        else
                                        {
                                            string InstallationPlaceName = _unitOfWork.InstallationPlaceRepository
                                                .GetWhereFirst(x => x.Name.ToLower().StartsWith("Direct".ToLower())).Name;

                                            ((IDictionary<String, Object>)DynamiMW_OtherInstallation).Add(new KeyValuePair<string, object>(prop.Name, InstallationPlaceName));
                                        }
                                    }
                                }
                                else
                                {
                                    object ForeignKeyNamePropObject = prop.GetValue(OthersInstallationObject, null);
                                    ((IDictionary<String, Object>)DynamiMW_OtherInstallation).Add(new KeyValuePair<string, object>(prop.Name, ForeignKeyNamePropObject));
                                }
                            }
                            else if (NotDateTimeInstallationAttributesViewModel.Select(x =>
                                 x.AttributeActivated.Key.ToLower()).Contains(prop.Name.ToLower()) &&
                                !prop.Name.ToLower().Contains("_name") &&
                                (prop.Name.ToLower().Substring(Math.Max(0, prop.Name.Length - 2)) != "id" || prop.Name.ToLower() == "id"))
                            {
                                if (prop.Name.ToLower() != "id" && prop.Name.ToLower() != "active")
                                {
                                    TLIattributeViewManagment LabelName = AllAttributes.FirstOrDefault(x => ((x.AttributeActivated != null) ? x.AttributeActivated.Key == prop.Name : false) &&
                                        x.AttributeActivated.Tabel == Helpers.Constants.TablesNames.TLImwOther.ToString() &&
                                        x.Enable && x.AttributeActivated.DataType != "List" && x.Id != 0);

                                    if (LabelName != null)
                                    {
                                        object PropObject = prop.GetValue(OthersInstallationObject, null);
                                        ((IDictionary<String, Object>)DynamiMW_OtherInstallation).Add(new KeyValuePair<string, object>(LabelName.AttributeActivated.Label, PropObject));
                                    }
                                }
                                else
                                {
                                    object PropObject = prop.GetValue(OthersInstallationObject, null);
                                    ((IDictionary<String, Object>)DynamiMW_OtherInstallation).Add(new KeyValuePair<string, object>(prop.Name, PropObject));
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
                            !x.disable && x.tablesNames.TableName == Helpers.Constants.TablesNames.TLImwOther.ToString() &&
                            !x.LibraryAtt && x.DataType.Name.ToLower() != "datetime" &&
                            NotDateTimeDynamicInstallationAttributesViewModel.Select(y => y.DynamicAttId).Any(y => y == x.Id)
                                , x => x.tablesNames, x => x.DataType).ToList();

                        List<TLIdynamicAttInstValue> NotDateTimeDynamicAttInstValues = _unitOfWork.DynamicAttInstValueRepository.GetIncludeWhere(x =>
                            !x.DynamicAtt.LibraryAtt && !x.disable &&
                            x.InventoryId == OthersInstallationObject.Id &&
                            NotDateTimeInstallationDynamicAttributes.AsEnumerable().Select(y => y.Key.ToLower()).Contains(x.DynamicAtt.Key.ToLower()) &&
                            x.tablesNames.TableName == Helpers.Constants.TablesNames.TLImwOther.ToString()
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

                                ((IDictionary<String, Object>)DynamiMW_OtherInstallation).Add(new KeyValuePair<string, object>(InstallationDynamicAtt.Key, DynamicAttValue));
                            }
                            else
                            {
                                ((IDictionary<String, Object>)DynamiMW_OtherInstallation).Add(new KeyValuePair<string, object>(InstallationDynamicAtt.Key, null));
                            }
                        }
                    }

                    //
                    // Installation Object ViewModel... (DateTime DataType Attribute)
                    //
                    dynamic DateTimeAttributes = new ExpandoObject();
                    if (DateTimeInstallationAttributesViewModel != null ? DateTimeInstallationAttributesViewModel.Count() > 0 : false)
                    {
                        List<PropertyInfo> DateTimeInstallationProps = typeof(Mw_OtherViewModel).GetProperties().Where(x =>
                            x.PropertyType.GenericTypeArguments != null ?
                                (x.PropertyType.GenericTypeArguments.Count() > 0 ? x.PropertyType.GenericTypeArguments.FirstOrDefault().Name.ToLower() == "datetime" :
                                (x.PropertyType.Name.ToLower() == "datetime")) :
                            (x.PropertyType.Name.ToLower() == "datetime")).ToList();

                        foreach (PropertyInfo prop in DateTimeInstallationProps)
                        {
                            TLIattributeViewManagment LabelName = AllAttributes.FirstOrDefault(x => ((x.AttributeActivated != null) ? x.AttributeActivated.Key == prop.Name : false) &&
                                x.AttributeActivated.Tabel == Helpers.Constants.TablesNames.TLImwOther.ToString() &&
                                x.Enable && x.AttributeActivated.DataType != "List" && x.Id != 0);

                            if (LabelName != null)
                            {
                                object PropObject = prop.GetValue(OthersInstallationObject, null);
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
                           !x.disable && x.tablesNames.TableName == Helpers.Constants.TablesNames.TLImwOther.ToString() &&
                           !x.LibraryAtt && x.DataType.Name.ToLower() == "datetime" &&
                            DateTimeDynamicInstallationAttributesViewModel.Select(y => y.DynamicAttId).Any(y => y == x.Id), x => x.tablesNames).ToList();

                        List<TLIdynamicAttInstValue> DateTimeDynamicAttInstValues = _unitOfWork.DynamicAttInstValueRepository.GetIncludeWhere(x =>
                            x.InventoryId == OthersInstallationObject.Id && !x.disable &&
                           !x.DynamicAtt.LibraryAtt &&
                            DateTimeInstallationDynamicAttributes.AsEnumerable().Select(y => y.Key.ToLower()).Contains(x.DynamicAtt.Key.ToLower()) &&
                            x.tablesNames.TableName == Helpers.Constants.TablesNames.TLImwOther.ToString()
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

                    ((IDictionary<String, Object>)DynamiMW_OtherInstallation).Add(new KeyValuePair<string, object>("DateTimeAttributes", DateTimeAttributes));

                    OutPutList.Add(DynamiMW_OtherInstallation);
                }

                MW_OtheresTableDisplay.Model = OutPutList;

                if (WithFilterData == true)
                {
                    MW_OtheresTableDisplay.filters = _unitOfWork.Mw_OtherRepository.GetRelatedTables();
                }
                else
                {
                    MW_OtheresTableDisplay.filters = null;
                }

                return new Response<ReturnWithFilters<object>>(true, MW_OtheresTableDisplay, null, null, (int)Helpers.Constants.ApiReturnCode.success, Count);
            }
            catch (Exception err)
            {
                return new Response<ReturnWithFilters<object>>(false, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
        }

        // 6. Get RadioAntenna...
        public Response<ReturnWithFilters<object>> GetRadioAntennaOnSiteWithEnableAtt(LoadsOnSiteFilter BaseFilter, bool WithFilterData, CombineFilters CombineFilters, ParameterPagination parameterPagination, int? CivilId, string CivilType)
        {
            try
            {
                int Count = 0;
                List<FilterObjectList> ObjectAttributeFilters = CombineFilters.filters;
                List<DateFilterViewModel> DateFilter = CombineFilters.DateFilter;

                List<object> OutPutList = new List<object>();
                ReturnWithFilters<object> RadioAntennaesTableDisplay = new ReturnWithFilters<object>();

                List<TLIcivilLoads> CivilLoadsRecords = new List<TLIcivilLoads>();

                List<StringFilterObjectList> AttributeFilters = new List<StringFilterObjectList>();
                List<AttributeActivatedViewModel> RadioAntennaInstallationAttribute = new List<AttributeActivatedViewModel>();

                if ((DateFilter != null ? DateFilter.Count() > 0 : false) ||
                    (ObjectAttributeFilters != null && ObjectAttributeFilters.Count > 0))
                {
                    RadioAntennaInstallationAttribute = _mapper.Map<List<AttributeActivatedViewModel>>(_unitOfWork.AttributeViewManagmentRepository.GetIncludeWhere(x =>
                        x.Enable && x.AttributeActivatedId != null &&
                        x.EditableManagmentView.View == Helpers.Constants.EditableManamgmantViewNames.RadioAntennaInstallation.ToString() &&
                        x.EditableManagmentView.TLItablesNames1.TableName == Helpers.Constants.TablesNames.TLIradioAntenna.ToString(),
                            x => x.AttributeActivated, x => x.EditableManagmentView, x => x.EditableManagmentView.TLItablesNames1,
                            x => x.EditableManagmentView.TLItablesNames2)
                    .Select(x => x.AttributeActivated).ToList());
                }

                if (ObjectAttributeFilters != null && ObjectAttributeFilters.Count > 0)
                {
                    List<AttributeActivatedViewModel> NotDateDateRadioAntennaInstallationAttribute = RadioAntennaInstallationAttribute.Where(x =>
                        x.DataType.ToLower() != "datetime").ToList();

                    foreach (FilterObjectList item in ObjectAttributeFilters)
                    {
                        List<string> value = item.value.Select(x => x.ToString().ToLower()).ToList();

                        AttributeActivatedViewModel AttributeKey = NotDateDateRadioAntennaInstallationAttribute.FirstOrDefault(x =>
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
                    List<AttributeActivatedViewModel> DateRadioAntennaInstallationAttribute = RadioAntennaInstallationAttribute.Where(x =>
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

                        AttributeActivatedViewModel AttributeKey = DateRadioAntennaInstallationAttribute.FirstOrDefault(x =>
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

                List<int> RadioAntennaIds = new List<int>();
                List<int> WithoutDateFilterRadioAntennaInstallation = new List<int>();
                List<int> WithDateFilterRadioAntennaInstallation = new List<int>();

                if (AttributeFilters != null && AttributeFilters.Count > 0)
                {
                    //
                    // Installation Dynamic Attributes...
                    //
                    List<TLIdynamicAtt> InstDynamicAttListIds = _unitOfWork.DynamicAttRepository.GetIncludeWhere(x =>
                        AttributeFilters.AsEnumerable().Select(y => y.key.ToLower()).Contains(x.Key.ToLower()) &&
                        !x.LibraryAtt && !x.disable &&
                        x.tablesNames.TableName == Helpers.Constants.TablesNames.TLIradioAntenna.ToString()
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
                    bool AttrInstExist = typeof(RadioAntennaViewModel).GetProperties().ToList().Exists(x =>
                        AttributeFilters.Where(y => y.key.ToLower() != "Id".ToLower()).Select(y =>
                            y.key.ToLower()).Contains(x.Name.ToLower()));

                    List<int> InstallationAttributeActivated = new List<int>();
                    if (AttrInstExist)
                    {
                        List<PropertyInfo> NotStringProps = typeof(RadioAntennaViewModel).GetProperties().Where(x =>
                            x.PropertyType.Name.ToLower() != "string" &&
                                AttributeFilters.Select(y =>
                                    y.key.ToLower()).Contains(x.Name.ToLower())).ToList();

                        List<PropertyInfo> StringProps = typeof(RadioAntennaViewModel).GetProperties().Where(x =>
                            x.PropertyType.Name.ToLower() == "string" &&
                                AttributeFilters.Select(y =>
                                    y.key.ToLower()).Contains(x.Name.ToLower())).ToList();

                        List<StringFilterObjectList> AttrInstAttributeFilters = AttributeFilters.Where(x =>
                            NotStringProps.Select(y => y.Name.ToLower()).Contains(x.key.ToLower()) ||
                            StringProps.Select(y => y.Name.ToLower()).Contains(x.key.ToLower())).ToList();

                        IEnumerable<TLIradioAntenna> Installations = _unitOfWork.RadioAntennaRepository.GetAllWithoutCount();

                        foreach (StringFilterObjectList InstallationProp in AttrInstAttributeFilters)
                        {
                            if (StringProps.Select(x => x.Name.ToLower()).Contains(InstallationProp.key.ToLower()))
                            {
                                Installations = Installations.Where(x => StringProps.AsEnumerable().FirstOrDefault(y => (InstallationProp.key.ToLower() == y.Name.ToLower()) && (InstallationProp.value.AsEnumerable().FirstOrDefault(w =>
                                     y.GetValue(_mapper.Map<RadioAntennaViewModel>(x), null) != null ? y.GetValue(_mapper.Map<RadioAntennaViewModel>(x), null).ToString().ToLower().StartsWith(w.ToLower()) : false) != null)) != null).AsEnumerable();
                            }
                            else if (NotStringProps.Select(x => x.Name.ToLower()).Contains(InstallationProp.key.ToLower()))
                            {
                                Installations = Installations.Where(x => NotStringProps.AsEnumerable().FirstOrDefault(y => (InstallationProp.key.ToLower() == y.Name.ToLower()) && (y.GetValue(_mapper.Map<RadioAntennaViewModel>(x), null) != null ?
                                    InstallationProp.value.AsEnumerable().Contains(y.GetValue(_mapper.Map<RadioAntennaViewModel>(x), null).ToString().ToLower()) : false)) != null).AsEnumerable();
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
                        WithoutDateFilterRadioAntennaInstallation = InstallationAttributeActivated.Intersect(DynamicInstValueListIds).ToList();
                    }
                    else if (AttrInstExist)
                    {
                        WithoutDateFilterRadioAntennaInstallation = InstallationAttributeActivated;
                    }
                    else if (DynamicInstExist)
                    {
                        WithoutDateFilterRadioAntennaInstallation = DynamicInstValueListIds;
                    }
                }

                if (DateFilter != null ? DateFilter.Count() > 0 : false)
                {
                    List<TLIdynamicAtt> DateTimeInstDynamicAttListIds = _unitOfWork.DynamicAttRepository.GetIncludeWhere(x =>
                        AfterConvertDateFilters.AsEnumerable().Select(y => y.key.ToLower()).Contains(x.Key.ToLower()) &&
                        !x.LibraryAtt && !x.disable &&
                        x.tablesNames.TableName == Helpers.Constants.TablesNames.TLIradioAntenna.ToString()
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
                    List<PropertyInfo> InstallationProps = typeof(RadioAntennaViewModel).GetProperties().Where(x =>
                        AfterConvertDateFilters.Select(y =>
                            y.key.ToLower()).Contains(x.Name.ToLower())).ToList();

                    List<int> InstallationAttributeActivatedIds = new List<int>();
                    bool AttrInstExist = false;

                    if (InstallationProps != null)
                    {
                        AttrInstExist = true;

                        List<DateFilterViewModel> InstallationPropsAttributeFilters = AfterConvertDateFilters.Where(x =>
                            InstallationProps.Select(y => y.Name.ToLower()).Contains(x.key.ToLower())).ToList();

                        IEnumerable<TLIradioAntenna> Installations = _unitOfWork.RadioAntennaRepository.GetAllWithoutCount();

                        foreach (DateFilterViewModel InstallationProp in InstallationPropsAttributeFilters)
                        {
                            Installations = Installations.Where(x => InstallationProps.Exists(y => (InstallationProp.key.ToLower() == y.Name.ToLower()) && ((y.GetValue(_mapper.Map<RadioAntennaViewModel>(x), null) != null) ?
                                ((InstallationProp.DateFrom.Date <= Convert.ToDateTime(y.GetValue(_mapper.Map<RadioAntennaViewModel>(x), null)).Date) &&
                                    (InstallationProp.DateTo.Date >= Convert.ToDateTime(y.GetValue(_mapper.Map<RadioAntennaViewModel>(x), null)).Date)) : (false))));
                        }

                        InstallationAttributeActivatedIds = Installations.Select(x => x.Id).ToList();
                    }

                    //
                    // Installation (Attribute Activated + Dynamic) Attributes...
                    //
                    if (AttrInstExist && DynamicInstExist)
                    {
                        WithDateFilterRadioAntennaInstallation = InstallationAttributeActivatedIds.Intersect(DynamicInstValueListIds).ToList();
                    }
                    else if (AttrInstExist)
                    {
                        WithDateFilterRadioAntennaInstallation = InstallationAttributeActivatedIds;
                    }
                    else if (DynamicInstExist)
                    {
                        WithDateFilterRadioAntennaInstallation = DynamicInstValueListIds;
                    }
                }

                if ((DateFilter != null ? DateFilter.Count() > 0 : false) ||
                    (AttributeFilters != null ? AttributeFilters.Count() > 0 : false))
                {
                    if ((AttributeFilters != null ? AttributeFilters.Count > 0 : false) &&
                            (AfterConvertDateFilters != null ? AfterConvertDateFilters.Count() > 0 : false))
                    {
                        RadioAntennaIds = WithoutDateFilterRadioAntennaInstallation.Intersect(WithDateFilterRadioAntennaInstallation).ToList();
                    }
                    else if (AttributeFilters != null ? AttributeFilters.Count > 0 : false)
                    {
                        RadioAntennaIds = WithoutDateFilterRadioAntennaInstallation;
                    }
                    else if (AfterConvertDateFilters != null ? AfterConvertDateFilters.Count() > 0 : false)
                    {
                        RadioAntennaIds = WithDateFilterRadioAntennaInstallation;
                    }

                    CivilLoadsRecords = _unitOfWork.CivilLoadsRepository.GetIncludeWhere(x =>
                        (x.allLoadInstId != null ? x.allLoadInst.radioAntennaId != null : false) &&
                        (!x.Dismantle) &&
                        (x.SiteCode == BaseFilter.SiteCode) &&
                        (BaseFilter.ItemStatusId != null ? (
                            x.allLoadInst != null ? (
                                x.allLoadInst.ItemStatusId != null ?
                                        (x.allLoadInst.ItemStatusId == BaseFilter.ItemStatusId)
                                : false)
                            : false)
                        : true) &&
                        (BaseFilter.TicketId != null ? (
                            x.allLoadInst != null ? (
                                x.allLoadInst.TicketId != null ?
                                        (x.allLoadInst.TicketId == BaseFilter.TicketId)
                                : false)
                            : false)
                        : true) &&
                        (BaseFilter.AllCivilId != null ?
                            (x.allCivilInstId == BaseFilter.AllCivilId)
                        : true) &&

                        RadioAntennaIds.Contains(x.allLoadInst.radioAntennaId.Value),
                    x => x.allCivilInst, x => x.allLoadInst, x => x.allLoadInst.radioAntenna, x => x.allLoadInst.radioAntenna.installationPlace,
                    x => x.allLoadInst.radioAntenna.owner, x => x.allLoadInst.radioAntenna.radioAntennaLibrary).ToList();
                }
                else
                {
                    CivilLoadsRecords = _unitOfWork.CivilLoadsRepository.GetIncludeWhere(x =>
                       (x.allLoadInstId != null ? x.allLoadInst.radioAntennaId != null : false) &&
                       (!x.Dismantle) &&
                       (x.SiteCode == BaseFilter.SiteCode) &&
                       (BaseFilter.ItemStatusId != null ? (
                           x.allLoadInst != null ? (
                               x.allLoadInst.ItemStatusId != null ?
                                       (x.allLoadInst.ItemStatusId == BaseFilter.ItemStatusId)
                               : false)
                           : false)
                       : true) &&
                       (BaseFilter.TicketId != null ? (
                           x.allLoadInst != null ? (
                               x.allLoadInst.TicketId != null ?
                                       (x.allLoadInst.TicketId == BaseFilter.TicketId)
                               : false)
                           : false)
                       : true) &&
                       (BaseFilter.AllCivilId != null ?
                           (x.allCivilInstId == BaseFilter.AllCivilId)
                       : true),

                    x => x.allCivilInst, x => x.allLoadInst, x => x.allLoadInst.radioAntenna, x => x.allLoadInst.radioAntenna.installationPlace,
                    x => x.allLoadInst.radioAntenna.owner, x => x.allLoadInst.radioAntenna.radioAntennaLibrary).ToList();
                }

                // Delete Duplicated Objects Based On Installation Date...
                List<TLIcivilLoads> NewList = new List<TLIcivilLoads>();
                foreach (var item in CivilLoadsRecords)
                {
                    TLIcivilLoads CheckIfExist = NewList.FirstOrDefault(x => x.allLoadInst.radioAntennaId.Value == item.allLoadInst.radioAntennaId.Value);
                    if (CheckIfExist != null)
                    {
                        if (CheckIfExist.InstallationDate < item.InstallationDate)
                        {
                            NewList.Remove(CheckIfExist);
                            NewList.Add(item);
                        }
                    }
                    else
                    {
                        NewList.Add(item);
                    }
                }
                CivilLoadsRecords = NewList;

                if (CivilId != null)
                {
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

                    List<int> RadioAntennasIds = _unitOfWork.CivilLoadsRepository.GetIncludeWhere(x => x.allCivilInstId == AllCivilInst.Id && !x.Dismantle &&
                        (x.allLoadInstId != null ? x.allLoadInst.radioAntennaId != null : false), x => x.allLoadInst).Select(x => x.allLoadInst.radioAntennaId.Value).Distinct().ToList();

                    CivilLoadsRecords = CivilLoadsRecords.Where(x => RadioAntennasIds.Contains(x.allLoadInst.radioAntennaId.Value)).ToList();
                }

                if (BaseFilter.SideArmId != null)
                {
                    List<int> RadioAntennasIds = _unitOfWork.CivilLoadsRepository.GetIncludeWhere(x => x.sideArmId == BaseFilter.SideArmId && !x.Dismantle &&
                        (x.allLoadInstId != null ? x.allLoadInst.radioAntennaId != null : false), x => x.allLoadInst).Select(x => x.allLoadInst.radioAntennaId.Value).Distinct().ToList();

                    CivilLoadsRecords = CivilLoadsRecords.Where(x => RadioAntennasIds.Contains(x.allLoadInst.radioAntennaId.Value)).ToList();
                }

                Count = CivilLoadsRecords.Count();

                CivilLoadsRecords = CivilLoadsRecords.Skip((parameterPagination.PageNumber - 1) * parameterPagination.PageSize).
                    Take(parameterPagination.PageSize).ToList();

                List<RadioAntennaViewModel> Others = _mapper.Map<List<RadioAntennaViewModel>>(CivilLoadsRecords.Select(x => x.allLoadInst.radioAntenna).ToList());

                List<TLIattributeViewManagment> AllAttributes = _unitOfWork.AttributeViewManagmentRepository.GetIncludeWhere(x =>
                   (x.Enable && x.EditableManagmentView.View == Helpers.Constants.EditableManamgmantViewNames.RadioAntennaInstallation.ToString() &&
                   (x.AttributeActivatedId != null ?
                        (x.AttributeActivated.Tabel == Helpers.Constants.TablesNames.TLIradioAntenna.ToString() && x.AttributeActivated.enable) :
                        (!x.DynamicAtt.LibraryAtt && !x.DynamicAtt.disable && x.DynamicAtt.tablesNames.TableName == Helpers.Constants.TablesNames.TLIradioAntenna.ToString()))) ||
                    (x.AttributeActivated != null ?
                        ((x.AttributeActivated.Key.ToLower() == "id" || x.AttributeActivated.Key.ToLower() == "active") && x.AttributeActivated.Tabel == Helpers.Constants.TablesNames.TLIradioAntenna.ToString()) : false),
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


                foreach (RadioAntennaViewModel OthersInstallationObject in Others)
                {
                    dynamic DynamiRadioAntennaInstallation = new ExpandoObject();

                    //
                    // Installation Object ViewModel...
                    //
                    if (NotDateTimeInstallationAttributesViewModel != null ? NotDateTimeInstallationAttributesViewModel.Count > 0 : false)
                    {
                        List<PropertyInfo> InstallationProps = typeof(RadioAntennaViewModel).GetProperties().Where(x =>
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
                                object ForeignKeyNamePropObject = prop.GetValue(OthersInstallationObject, null);
                                ((IDictionary<String, Object>)DynamiRadioAntennaInstallation).Add(new KeyValuePair<string, object>(prop.Name, ForeignKeyNamePropObject));
                            }
                            else if (NotDateTimeInstallationAttributesViewModel.Select(x =>
                                 x.AttributeActivated.Key.ToLower()).Contains(prop.Name.ToLower()) &&
                                !prop.Name.ToLower().Contains("_name") &&
                                (prop.Name.ToLower().Substring(Math.Max(0, prop.Name.Length - 2)) != "id" || prop.Name.ToLower() == "id"))
                            {
                                if (prop.Name.ToLower() != "id" && prop.Name.ToLower() != "active")
                                {
                                    TLIattributeViewManagment LabelName = AllAttributes.FirstOrDefault(x => ((x.AttributeActivated != null) ? x.AttributeActivated.Key == prop.Name : false) &&
                                        x.AttributeActivated.Tabel == Helpers.Constants.TablesNames.TLIradioAntenna.ToString() &&
                                        x.Enable && x.AttributeActivated.DataType != "List" && x.Id != 0);

                                    if (LabelName != null)
                                    {
                                        object PropObject = prop.GetValue(OthersInstallationObject, null);
                                        ((IDictionary<String, Object>)DynamiRadioAntennaInstallation).Add(new KeyValuePair<string, object>(LabelName.AttributeActivated.Label, PropObject));
                                    }
                                }
                                else
                                {
                                    object PropObject = prop.GetValue(OthersInstallationObject, null);
                                    ((IDictionary<String, Object>)DynamiRadioAntennaInstallation).Add(new KeyValuePair<string, object>(prop.Name, PropObject));
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
                            !x.disable && x.tablesNames.TableName == Helpers.Constants.TablesNames.TLIradioAntenna.ToString() &&
                            !x.LibraryAtt && x.DataType.Name.ToLower() != "datetime" &&
                            NotDateTimeDynamicInstallationAttributesViewModel.Select(y => y.DynamicAttId).Any(y => y == x.Id)
                                , x => x.tablesNames, x => x.DataType).ToList();

                        List<TLIdynamicAttInstValue> NotDateTimeDynamicAttInstValues = _unitOfWork.DynamicAttInstValueRepository.GetIncludeWhere(x =>
                            !x.DynamicAtt.LibraryAtt && !x.disable &&
                            x.InventoryId == OthersInstallationObject.Id &&
                            NotDateTimeInstallationDynamicAttributes.AsEnumerable().Select(y => y.Key.ToLower()).Contains(x.DynamicAtt.Key.ToLower()) &&
                            x.tablesNames.TableName == Helpers.Constants.TablesNames.TLIradioAntenna.ToString()
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

                                ((IDictionary<String, Object>)DynamiRadioAntennaInstallation).Add(new KeyValuePair<string, object>(InstallationDynamicAtt.Key, DynamicAttValue));
                            }
                            else
                            {
                                ((IDictionary<String, Object>)DynamiRadioAntennaInstallation).Add(new KeyValuePair<string, object>(InstallationDynamicAtt.Key, null));
                            }
                        }
                    }

                    //
                    // Installation Object ViewModel... (DateTime DataType Attribute)
                    //
                    dynamic DateTimeAttributes = new ExpandoObject();
                    if (DateTimeInstallationAttributesViewModel != null ? DateTimeInstallationAttributesViewModel.Count() > 0 : false)
                    {
                        List<PropertyInfo> DateTimeInstallationProps = typeof(RadioAntennaViewModel).GetProperties().Where(x =>
                            x.PropertyType.GenericTypeArguments != null ?
                                (x.PropertyType.GenericTypeArguments.Count() > 0 ? x.PropertyType.GenericTypeArguments.FirstOrDefault().Name.ToLower() == "datetime" :
                                (x.PropertyType.Name.ToLower() == "datetime")) :
                            (x.PropertyType.Name.ToLower() == "datetime")).ToList();

                        foreach (PropertyInfo prop in DateTimeInstallationProps)
                        {
                            TLIattributeViewManagment LabelName = AllAttributes.FirstOrDefault(x => ((x.AttributeActivated != null) ? x.AttributeActivated.Key == prop.Name : false) &&
                                x.AttributeActivated.Tabel == Helpers.Constants.TablesNames.TLIradioAntenna.ToString() &&
                                x.Enable && x.AttributeActivated.DataType != "List" && x.Id != 0);

                            if (LabelName != null)
                            {
                                object PropObject = prop.GetValue(OthersInstallationObject, null);
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
                           !x.disable && x.tablesNames.TableName == Helpers.Constants.TablesNames.TLIradioAntenna.ToString() &&
                           !x.LibraryAtt && x.DataType.Name.ToLower() == "datetime" &&
                            DateTimeDynamicInstallationAttributesViewModel.Select(y => y.DynamicAttId).Any(y => y == x.Id), x => x.tablesNames).ToList();

                        List<TLIdynamicAttInstValue> DateTimeDynamicAttInstValues = _unitOfWork.DynamicAttInstValueRepository.GetIncludeWhere(x =>
                            x.InventoryId == OthersInstallationObject.Id && !x.disable &&
                           !x.DynamicAtt.LibraryAtt &&
                            DateTimeInstallationDynamicAttributes.AsEnumerable().Select(y => y.Key.ToLower()).Contains(x.DynamicAtt.Key.ToLower()) &&
                            x.tablesNames.TableName == Helpers.Constants.TablesNames.TLIradioAntenna.ToString()
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

                    ((IDictionary<String, Object>)DynamiRadioAntennaInstallation).Add(new KeyValuePair<string, object>("DateTimeAttributes", DateTimeAttributes));

                    OutPutList.Add(DynamiRadioAntennaInstallation);
                }
                RadioAntennaesTableDisplay.Model = OutPutList;

                if (WithFilterData == true)
                {
                    RadioAntennaesTableDisplay.filters = _unitOfWork.RadioAntennaRepository.GetRelatedTables();
                }
                else
                {
                    RadioAntennaesTableDisplay.filters = null;
                }

                return new Response<ReturnWithFilters<object>>(true, RadioAntennaesTableDisplay, null, null, (int)Helpers.Constants.ApiReturnCode.success, Count);
            }
            catch (Exception err)
            {
                return new Response<ReturnWithFilters<object>>(false, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
        }

        // 7. Get RadioRRU...
        public Response<ReturnWithFilters<object>> GetRadioRRUOnSiteWithEnableAtt(LoadsOnSiteFilter BaseFilter, bool WithFilterData, CombineFilters CombineFilters, ParameterPagination parameterPagination, int? CivilId, string CivilType)
        {
            try
            {
                int Count = 0;
                List<FilterObjectList> ObjectAttributeFilters = CombineFilters.filters;
                List<DateFilterViewModel> DateFilter = CombineFilters.DateFilter;

                List<object> OutPutList = new List<object>();
                ReturnWithFilters<object> RadioRRUesTableDisplay = new ReturnWithFilters<object>();

                List<TLIcivilLoads> CivilLoadsRecords = new List<TLIcivilLoads>();

                List<StringFilterObjectList> AttributeFilters = new List<StringFilterObjectList>();
                List<AttributeActivatedViewModel> RadioRRUInstallationAttribute = new List<AttributeActivatedViewModel>();

                if ((DateFilter != null ? DateFilter.Count() > 0 : false) ||
                    (ObjectAttributeFilters != null && ObjectAttributeFilters.Count > 0))
                {
                    RadioRRUInstallationAttribute = _mapper.Map<List<AttributeActivatedViewModel>>(_unitOfWork.AttributeViewManagmentRepository.GetIncludeWhere(x =>
                        x.Enable && x.AttributeActivatedId != null &&
                        x.EditableManagmentView.View == Helpers.Constants.EditableManamgmantViewNames.RadioRRUInstallation.ToString() &&
                        x.EditableManagmentView.TLItablesNames1.TableName == Helpers.Constants.TablesNames.TLIradioRRU.ToString(),
                            x => x.AttributeActivated, x => x.EditableManagmentView, x => x.EditableManagmentView.TLItablesNames1,
                            x => x.EditableManagmentView.TLItablesNames2)
                    .Select(x => x.AttributeActivated).ToList());
                }

                if (ObjectAttributeFilters != null && ObjectAttributeFilters.Count > 0)
                {
                    List<AttributeActivatedViewModel> NotDateDateRadioRRUInstallationAttribute = RadioRRUInstallationAttribute.Where(x =>
                        x.DataType.ToLower() != "datetime").ToList();

                    foreach (FilterObjectList item in ObjectAttributeFilters)
                    {
                        List<string> value = item.value.Select(x => x.ToString().ToLower()).ToList();

                        AttributeActivatedViewModel AttributeKey = NotDateDateRadioRRUInstallationAttribute.FirstOrDefault(x =>
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
                    List<AttributeActivatedViewModel> DateRadioRRUInstallationAttribute = RadioRRUInstallationAttribute.Where(x =>
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

                        AttributeActivatedViewModel AttributeKey = DateRadioRRUInstallationAttribute.FirstOrDefault(x =>
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

                List<int> RadioRRUIds = new List<int>();
                List<int> WithoutDateFilterRadioRRUInstallation = new List<int>();
                List<int> WithDateFilterRadioRRUInstallation = new List<int>();

                if (AttributeFilters != null && AttributeFilters.Count > 0)
                {
                    //
                    // Installation Dynamic Attributes...
                    //
                    List<TLIdynamicAtt> InstDynamicAttListIds = _unitOfWork.DynamicAttRepository.GetIncludeWhere(x =>
                        AttributeFilters.AsEnumerable().Select(y => y.key.ToLower()).Contains(x.Key.ToLower()) &&
                        !x.LibraryAtt && !x.disable &&
                        x.tablesNames.TableName == Helpers.Constants.TablesNames.TLIradioRRU.ToString()
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
                    bool AttrInstExist = typeof(RadioRRUViewModel).GetProperties().ToList().Exists(x =>
                        AttributeFilters.Where(y => y.key.ToLower() != "Id".ToLower()).Select(y =>
                            y.key.ToLower()).Contains(x.Name.ToLower()));

                    List<int> InstallationAttributeActivated = new List<int>();
                    if (AttrInstExist)
                    {
                        List<PropertyInfo> NotStringProps = typeof(RadioRRUViewModel).GetProperties().Where(x =>
                            x.PropertyType.Name.ToLower() != "string" &&
                                AttributeFilters.Select(y =>
                                    y.key.ToLower()).Contains(x.Name.ToLower())).ToList();

                        List<PropertyInfo> StringProps = typeof(RadioRRUViewModel).GetProperties().Where(x =>
                            x.PropertyType.Name.ToLower() == "string" &&
                                AttributeFilters.Select(y =>
                                    y.key.ToLower()).Contains(x.Name.ToLower())).ToList();

                        List<StringFilterObjectList> AttrInstAttributeFilters = AttributeFilters.Where(x =>
                            NotStringProps.Select(y => y.Name.ToLower()).Contains(x.key.ToLower()) ||
                            StringProps.Select(y => y.Name.ToLower()).Contains(x.key.ToLower())).ToList();

                        IEnumerable<TLIRadioRRU> Installations = _unitOfWork.RadioRRURepository.GetAllWithoutCount();

                        foreach (StringFilterObjectList InstallationProp in AttrInstAttributeFilters)
                        {
                            if (StringProps.Select(x => x.Name.ToLower()).Contains(InstallationProp.key.ToLower()))
                            {
                                Installations = Installations.Where(x => StringProps.AsEnumerable().FirstOrDefault(y => (InstallationProp.key.ToLower() == y.Name.ToLower()) && (InstallationProp.value.AsEnumerable().FirstOrDefault(w =>
                                     y.GetValue(_mapper.Map<RadioRRUViewModel>(x), null) != null ? y.GetValue(_mapper.Map<RadioRRUViewModel>(x), null).ToString().ToLower().StartsWith(w.ToLower()) : false) != null)) != null).AsEnumerable();
                            }
                            else if (NotStringProps.Select(x => x.Name.ToLower()).Contains(InstallationProp.key.ToLower()))
                            {
                                Installations = Installations.Where(x => NotStringProps.AsEnumerable().FirstOrDefault(y => (InstallationProp.key.ToLower() == y.Name.ToLower()) && (y.GetValue(_mapper.Map<RadioRRUViewModel>(x), null) != null ?
                                    InstallationProp.value.AsEnumerable().Contains(y.GetValue(_mapper.Map<RadioRRUViewModel>(x), null).ToString().ToLower()) : false)) != null).AsEnumerable();
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
                        WithoutDateFilterRadioRRUInstallation = InstallationAttributeActivated.Intersect(DynamicInstValueListIds).ToList();
                    }
                    else if (AttrInstExist)
                    {
                        WithoutDateFilterRadioRRUInstallation = InstallationAttributeActivated;
                    }
                    else if (DynamicInstExist)
                    {
                        WithoutDateFilterRadioRRUInstallation = DynamicInstValueListIds;
                    }
                }

                if (DateFilter != null ? DateFilter.Count() > 0 : false)
                {
                    List<TLIdynamicAtt> DateTimeInstDynamicAttListIds = _unitOfWork.DynamicAttRepository.GetIncludeWhere(x =>
                        AfterConvertDateFilters.AsEnumerable().Select(y => y.key.ToLower()).Contains(x.Key.ToLower()) &&
                        !x.LibraryAtt && !x.disable &&
                        x.tablesNames.TableName == Helpers.Constants.TablesNames.TLIradioRRU.ToString()
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
                    List<PropertyInfo> InstallationProps = typeof(RadioRRUViewModel).GetProperties().Where(x =>
                        AfterConvertDateFilters.Select(y =>
                            y.key.ToLower()).Contains(x.Name.ToLower())).ToList();

                    List<int> InstallationAttributeActivatedIds = new List<int>();
                    bool AttrInstExist = false;

                    if (InstallationProps != null)
                    {
                        AttrInstExist = true;

                        List<DateFilterViewModel> InstallationPropsAttributeFilters = AfterConvertDateFilters.Where(x =>
                            InstallationProps.Select(y => y.Name.ToLower()).Contains(x.key.ToLower())).ToList();

                        IEnumerable<TLIRadioRRU> Installations = _unitOfWork.RadioRRURepository.GetAllWithoutCount();

                        foreach (DateFilterViewModel InstallationProp in InstallationPropsAttributeFilters)
                        {
                            Installations = Installations.Where(x => InstallationProps.Exists(y => (InstallationProp.key.ToLower() == y.Name.ToLower()) && ((y.GetValue(_mapper.Map<RadioRRUViewModel>(x), null) != null) ?
                                ((InstallationProp.DateFrom.Date <= Convert.ToDateTime(y.GetValue(_mapper.Map<RadioRRUViewModel>(x), null)).Date) &&
                                    (InstallationProp.DateTo.Date >= Convert.ToDateTime(y.GetValue(_mapper.Map<RadioRRUViewModel>(x), null)).Date)) : (false))));
                        }

                        InstallationAttributeActivatedIds = Installations.Select(x => x.Id).ToList();
                    }

                    //
                    // Installation (Attribute Activated + Dynamic) Attributes...
                    //
                    if (AttrInstExist && DynamicInstExist)
                    {
                        WithDateFilterRadioRRUInstallation = InstallationAttributeActivatedIds.Intersect(DynamicInstValueListIds).ToList();
                    }
                    else if (AttrInstExist)
                    {
                        WithDateFilterRadioRRUInstallation = InstallationAttributeActivatedIds;
                    }
                    else if (DynamicInstExist)
                    {
                        WithDateFilterRadioRRUInstallation = DynamicInstValueListIds;
                    }
                }

                if ((DateFilter != null ? DateFilter.Count() > 0 : false) ||
                    (AttributeFilters != null ? AttributeFilters.Count() > 0 : false))
                {
                    if ((AttributeFilters != null ? AttributeFilters.Count > 0 : false) &&
                            (AfterConvertDateFilters != null ? AfterConvertDateFilters.Count() > 0 : false))
                    {
                        RadioRRUIds = WithoutDateFilterRadioRRUInstallation.Intersect(WithDateFilterRadioRRUInstallation).ToList();
                    }
                    else if (AttributeFilters != null ? AttributeFilters.Count > 0 : false)
                    {
                        RadioRRUIds = WithoutDateFilterRadioRRUInstallation;
                    }
                    else if (AfterConvertDateFilters != null ? AfterConvertDateFilters.Count() > 0 : false)
                    {
                        RadioRRUIds = WithDateFilterRadioRRUInstallation;
                    }

                    CivilLoadsRecords = _unitOfWork.CivilLoadsRepository.GetIncludeWhere(x =>
                        (x.allLoadInstId != null ? x.allLoadInst.radioRRUId != null : false) &&
                        (!x.Dismantle) &&
                        (x.SiteCode == BaseFilter.SiteCode) &&
                        (BaseFilter.ItemStatusId != null ? (
                            x.allLoadInst != null ? (
                                x.allLoadInst.ItemStatusId != null ?
                                        (x.allLoadInst.ItemStatusId == BaseFilter.ItemStatusId)
                                : false)
                            : false)
                        : true) &&
                        (BaseFilter.TicketId != null ? (
                            x.allLoadInst != null ? (
                                x.allLoadInst.TicketId != null ?
                                        (x.allLoadInst.TicketId == BaseFilter.TicketId)
                                : false)
                            : false)
                        : true) &&
                        (BaseFilter.AllCivilId != null ?
                            (x.allCivilInstId == BaseFilter.AllCivilId)
                        : true) &&

                        RadioRRUIds.Contains(x.allLoadInst.radioRRUId.Value),
                    x => x.allCivilInst, x => x.allLoadInst, x => x.allLoadInst.radioRRU, x => x.allLoadInst.radioRRU.installationPlace,
                    x => x.allLoadInst.radioRRU.owner, x => x.allLoadInst.radioRRU.radioRRULibrary, x => x.allLoadInst.radioRRU.radioAntenna).ToList();
                }
                else
                {
                    CivilLoadsRecords = _unitOfWork.CivilLoadsRepository.GetIncludeWhere(x =>
                       (x.allLoadInstId != null ? x.allLoadInst.radioRRUId != null : false) &&
                       (!x.Dismantle) &&
                       (x.SiteCode == BaseFilter.SiteCode) &&
                       (BaseFilter.ItemStatusId != null ? (
                           x.allLoadInst != null ? (
                               x.allLoadInst.ItemStatusId != null ?
                                       (x.allLoadInst.ItemStatusId == BaseFilter.ItemStatusId)
                               : false)
                           : false)
                       : true) &&
                       (BaseFilter.TicketId != null ? (
                           x.allLoadInst != null ? (
                               x.allLoadInst.TicketId != null ?
                                       (x.allLoadInst.TicketId == BaseFilter.TicketId)
                               : false)
                           : false)
                       : true) &&
                       (BaseFilter.AllCivilId != null ?
                           (x.allCivilInstId == BaseFilter.AllCivilId)
                       : true),

                    x => x.allCivilInst, x => x.allLoadInst, x => x.allLoadInst.radioRRU, x => x.allLoadInst.radioRRU.installationPlace,
                    x => x.allLoadInst.radioRRU.owner, x => x.allLoadInst.radioRRU.radioRRULibrary, x => x.allLoadInst.radioRRU.radioAntenna).ToList();
                }

                // Delete Duplicated Objects Based On Installation Date...
                List<TLIcivilLoads> NewList = new List<TLIcivilLoads>();
                foreach (var item in CivilLoadsRecords)
                {
                    TLIcivilLoads CheckIfExist = NewList.FirstOrDefault(x => x.allLoadInst.radioRRUId.Value == item.allLoadInst.radioRRUId.Value);
                    if (CheckIfExist != null)
                    {
                        if (CheckIfExist.InstallationDate < item.InstallationDate)
                        {
                            NewList.Remove(CheckIfExist);
                            NewList.Add(item);
                        }
                    }
                    else
                    {
                        NewList.Add(item);
                    }
                }
                CivilLoadsRecords = NewList;

                if (CivilId != null)
                {
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

                    List<int> RadioRRUsIds = _unitOfWork.CivilLoadsRepository.GetIncludeWhere(x => x.allCivilInstId == AllCivilInst.Id && !x.Dismantle &&
                        (x.allLoadInstId != null ? x.allLoadInst.radioRRUId != null : false), x => x.allLoadInst).Select(x => x.allLoadInst.radioRRUId.Value).Distinct().ToList();

                    CivilLoadsRecords = CivilLoadsRecords.Where(x => RadioRRUsIds.Contains(x.allLoadInst.radioRRUId.Value)).ToList();
                }

                if (BaseFilter.SideArmId != null)
                {
                    List<int> RadioRRUsIds = _unitOfWork.CivilLoadsRepository.GetIncludeWhere(x => x.sideArmId == BaseFilter.SideArmId && !x.Dismantle &&
                        (x.allLoadInstId != null ? x.allLoadInst.radioRRUId != null : false), x => x.allLoadInst).Select(x => x.allLoadInst.radioRRUId.Value).Distinct().ToList();

                    CivilLoadsRecords = CivilLoadsRecords.Where(x => RadioRRUsIds.Contains(x.allLoadInst.radioRRUId.Value)).ToList();
                }

                Count = CivilLoadsRecords.Count();

                CivilLoadsRecords = CivilLoadsRecords.Skip((parameterPagination.PageNumber - 1) * parameterPagination.PageSize).
                    Take(parameterPagination.PageSize).ToList();

                List<RadioRRUViewModel> Others = _mapper.Map<List<RadioRRUViewModel>>(CivilLoadsRecords.Select(x => x.allLoadInst.radioRRU).ToList());

                List<TLIattributeViewManagment> AllAttributes = _unitOfWork.AttributeViewManagmentRepository.GetIncludeWhere(x =>
                   (x.Enable && x.EditableManagmentView.View == Helpers.Constants.EditableManamgmantViewNames.RadioRRUInstallation.ToString() &&
                   (x.AttributeActivatedId != null ?
                        (x.AttributeActivated.Tabel == Helpers.Constants.TablesNames.TLIradioRRU.ToString() && x.AttributeActivated.enable) :
                        (!x.DynamicAtt.LibraryAtt && !x.DynamicAtt.disable && x.DynamicAtt.tablesNames.TableName == Helpers.Constants.TablesNames.TLIradioRRU.ToString()))) ||
                    (x.AttributeActivated != null ?
                        ((x.AttributeActivated.Key.ToLower() == "id" || x.AttributeActivated.Key.ToLower() == "active") && x.AttributeActivated.Tabel == Helpers.Constants.TablesNames.TLIradioRRU.ToString()) : false),
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


                foreach (RadioRRUViewModel OthersInstallationObject in Others)
                {
                    dynamic DynamiRadioRRUInstallation = new ExpandoObject();

                    //
                    // Installation Object ViewModel...
                    //
                    if (NotDateTimeInstallationAttributesViewModel != null ? NotDateTimeInstallationAttributesViewModel.Count > 0 : false)
                    {
                        List<PropertyInfo> InstallationProps = typeof(RadioRRUViewModel).GetProperties().Where(x =>
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
                                object ForeignKeyNamePropObject = prop.GetValue(OthersInstallationObject, null);
                                ((IDictionary<String, Object>)DynamiRadioRRUInstallation).Add(new KeyValuePair<string, object>(prop.Name, ForeignKeyNamePropObject));
                            }
                            else if (NotDateTimeInstallationAttributesViewModel.Select(x =>
                                 x.AttributeActivated.Key.ToLower()).Contains(prop.Name.ToLower()) &&
                                !prop.Name.ToLower().Contains("_name") &&
                                (prop.Name.ToLower().Substring(Math.Max(0, prop.Name.Length - 2)) != "id" || prop.Name.ToLower() == "id"))
                            {
                                if (prop.Name.ToLower() != "id" && prop.Name.ToLower() != "active")
                                {
                                    TLIattributeViewManagment LabelName = AllAttributes.FirstOrDefault(x => ((x.AttributeActivated != null) ? x.AttributeActivated.Key == prop.Name : false) &&
                                        x.AttributeActivated.Tabel == Helpers.Constants.TablesNames.TLIradioRRU.ToString() &&
                                        x.Enable && x.AttributeActivated.DataType != "List" && x.Id != 0);

                                    if (LabelName != null)
                                    {
                                        object PropObject = prop.GetValue(OthersInstallationObject, null);
                                        ((IDictionary<String, Object>)DynamiRadioRRUInstallation).Add(new KeyValuePair<string, object>(LabelName.AttributeActivated.Label, PropObject));
                                    }
                                }
                                else
                                {
                                    object PropObject = prop.GetValue(OthersInstallationObject, null);
                                    ((IDictionary<String, Object>)DynamiRadioRRUInstallation).Add(new KeyValuePair<string, object>(prop.Name, PropObject));
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
                            !x.disable && x.tablesNames.TableName == Helpers.Constants.TablesNames.TLIradioRRU.ToString() &&
                            !x.LibraryAtt && x.DataType.Name.ToLower() != "datetime" &&
                            NotDateTimeDynamicInstallationAttributesViewModel.Select(y => y.DynamicAttId).Any(y => y == x.Id)
                                , x => x.tablesNames, x => x.DataType).ToList();

                        List<TLIdynamicAttInstValue> NotDateTimeDynamicAttInstValues = _unitOfWork.DynamicAttInstValueRepository.GetIncludeWhere(x =>
                            !x.DynamicAtt.LibraryAtt && !x.disable &&
                            x.InventoryId == OthersInstallationObject.Id &&
                            NotDateTimeInstallationDynamicAttributes.AsEnumerable().Select(y => y.Key.ToLower()).Contains(x.DynamicAtt.Key.ToLower()) &&
                            x.tablesNames.TableName == Helpers.Constants.TablesNames.TLIradioRRU.ToString()
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

                                ((IDictionary<String, Object>)DynamiRadioRRUInstallation).Add(new KeyValuePair<string, object>(InstallationDynamicAtt.Key, DynamicAttValue));
                            }
                            else
                            {
                                ((IDictionary<String, Object>)DynamiRadioRRUInstallation).Add(new KeyValuePair<string, object>(InstallationDynamicAtt.Key, null));
                            }
                        }
                    }

                    //
                    // Installation Object ViewModel... (DateTime DataType Attribute)
                    //
                    dynamic DateTimeAttributes = new ExpandoObject();
                    if (DateTimeInstallationAttributesViewModel != null ? DateTimeInstallationAttributesViewModel.Count() > 0 : false)
                    {
                        List<PropertyInfo> DateTimeInstallationProps = typeof(RadioRRUViewModel).GetProperties().Where(x =>
                            x.PropertyType.GenericTypeArguments != null ?
                                (x.PropertyType.GenericTypeArguments.Count() > 0 ? x.PropertyType.GenericTypeArguments.FirstOrDefault().Name.ToLower() == "datetime" :
                                (x.PropertyType.Name.ToLower() == "datetime")) :
                            (x.PropertyType.Name.ToLower() == "datetime")).ToList();

                        foreach (PropertyInfo prop in DateTimeInstallationProps)
                        {
                            TLIattributeViewManagment LabelName = AllAttributes.FirstOrDefault(x => ((x.AttributeActivated != null) ? x.AttributeActivated.Key == prop.Name : false) &&
                                x.AttributeActivated.Tabel == Helpers.Constants.TablesNames.TLIradioRRU.ToString() &&
                                x.Enable && x.AttributeActivated.DataType != "List" && x.Id != 0);

                            if (LabelName != null)
                            {
                                object PropObject = prop.GetValue(OthersInstallationObject, null);
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
                           !x.disable && x.tablesNames.TableName == Helpers.Constants.TablesNames.TLIradioRRU.ToString() &&
                           !x.LibraryAtt && x.DataType.Name.ToLower() == "datetime" &&
                            DateTimeDynamicInstallationAttributesViewModel.Select(y => y.DynamicAttId).Any(y => y == x.Id), x => x.tablesNames).ToList();

                        List<TLIdynamicAttInstValue> DateTimeDynamicAttInstValues = _unitOfWork.DynamicAttInstValueRepository.GetIncludeWhere(x =>
                            x.InventoryId == OthersInstallationObject.Id && !x.disable &&
                           !x.DynamicAtt.LibraryAtt &&
                            DateTimeInstallationDynamicAttributes.AsEnumerable().Select(y => y.Key.ToLower()).Contains(x.DynamicAtt.Key.ToLower()) &&
                            x.tablesNames.TableName == Helpers.Constants.TablesNames.TLIradioRRU.ToString()
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

                    ((IDictionary<String, Object>)DynamiRadioRRUInstallation).Add(new KeyValuePair<string, object>("DateTimeAttributes", DateTimeAttributes));

                    OutPutList.Add(DynamiRadioRRUInstallation);
                }
                RadioRRUesTableDisplay.Model = OutPutList;

                if (WithFilterData == true)
                {
                    RadioRRUesTableDisplay.filters = _unitOfWork.RadioRRURepository.GetRelatedTables(BaseFilter.SiteCode);
                }
                else
                {
                    RadioRRUesTableDisplay.filters = null;
                }

                return new Response<ReturnWithFilters<object>>(true, RadioRRUesTableDisplay, null, null, (int)Helpers.Constants.ApiReturnCode.success, Count);
            }
            catch (Exception err)
            {
                return new Response<ReturnWithFilters<object>>(false, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
        }

        // 8. Get RadioOther...
        public Response<ReturnWithFilters<object>> GetRadioOtherOnSiteWithEnableAtt(LoadsOnSiteFilter BaseFilter, bool WithFilterData, CombineFilters CombineFilters, ParameterPagination parameterPagination, int? CivilId, string CivilType)
        {
            try
            {
                int Count = 0;
                List<FilterObjectList> ObjectAttributeFilters = CombineFilters.filters;
                List<DateFilterViewModel> DateFilter = CombineFilters.DateFilter;

                List<object> OutPutList = new List<object>();
                ReturnWithFilters<object> RadioOtheresTableDisplay = new ReturnWithFilters<object>();

                List<TLIcivilLoads> CivilLoadsRecords = new List<TLIcivilLoads>();

                List<StringFilterObjectList> AttributeFilters = new List<StringFilterObjectList>();
                List<AttributeActivatedViewModel> RadioOtherInstallationAttribute = new List<AttributeActivatedViewModel>();

                if ((DateFilter != null ? DateFilter.Count() > 0 : false) ||
                    (ObjectAttributeFilters != null && ObjectAttributeFilters.Count > 0))
                {
                    RadioOtherInstallationAttribute = _mapper.Map<List<AttributeActivatedViewModel>>(_unitOfWork.AttributeViewManagmentRepository.GetIncludeWhere(x =>
                        x.Enable && x.AttributeActivatedId != null &&
                        x.EditableManagmentView.View == Helpers.Constants.EditableManamgmantViewNames.RadioOtherInstallation.ToString() &&
                        x.EditableManagmentView.TLItablesNames1.TableName == Helpers.Constants.TablesNames.TLIradioOther.ToString(),
                            x => x.AttributeActivated, x => x.EditableManagmentView, x => x.EditableManagmentView.TLItablesNames1,
                            x => x.EditableManagmentView.TLItablesNames2)
                    .Select(x => x.AttributeActivated).ToList());
                }

                if (ObjectAttributeFilters != null && ObjectAttributeFilters.Count > 0)
                {
                    List<AttributeActivatedViewModel> NotDateDateRadioOtherInstallationAttribute = RadioOtherInstallationAttribute.Where(x =>
                        x.DataType.ToLower() != "datetime").ToList();

                    foreach (FilterObjectList item in ObjectAttributeFilters)
                    {
                        List<string> value = item.value.Select(x => x.ToString().ToLower()).ToList();

                        AttributeActivatedViewModel AttributeKey = NotDateDateRadioOtherInstallationAttribute.FirstOrDefault(x =>
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
                    List<AttributeActivatedViewModel> DateRadioOtherInstallationAttribute = RadioOtherInstallationAttribute.Where(x =>
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

                        AttributeActivatedViewModel AttributeKey = DateRadioOtherInstallationAttribute.FirstOrDefault(x =>
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

                List<int> RadioOtherIds = new List<int>();
                List<int> WithoutDateFilterRadioOtherInstallation = new List<int>();
                List<int> WithDateFilterRadioOtherInstallation = new List<int>();

                if (AttributeFilters != null && AttributeFilters.Count > 0)
                {
                    //
                    // Installation Dynamic Attributes...
                    //
                    List<TLIdynamicAtt> InstDynamicAttListIds = _unitOfWork.DynamicAttRepository.GetIncludeWhere(x =>
                        AttributeFilters.AsEnumerable().Select(y => y.key.ToLower()).Contains(x.Key.ToLower()) &&
                        !x.LibraryAtt && !x.disable &&
                        x.tablesNames.TableName == Helpers.Constants.TablesNames.TLIradioOther.ToString()
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
                    bool AttrInstExist = typeof(RadioOtherViewModel).GetProperties().ToList().Exists(x =>
                        AttributeFilters.Where(y => y.key.ToLower() != "Id".ToLower()).Select(y =>
                            y.key.ToLower()).Contains(x.Name.ToLower()));

                    List<int> InstallationAttributeActivated = new List<int>();
                    if (AttrInstExist)
                    {
                        List<PropertyInfo> NotStringProps = typeof(RadioOtherViewModel).GetProperties().Where(x =>
                            x.PropertyType.Name.ToLower() != "string" &&
                                AttributeFilters.Select(y =>
                                    y.key.ToLower()).Contains(x.Name.ToLower())).ToList();

                        List<PropertyInfo> StringProps = typeof(RadioOtherViewModel).GetProperties().Where(x =>
                            x.PropertyType.Name.ToLower() == "string" &&
                                AttributeFilters.Select(y =>
                                    y.key.ToLower()).Contains(x.Name.ToLower())).ToList();

                        List<StringFilterObjectList> AttrInstAttributeFilters = AttributeFilters.Where(x =>
                            NotStringProps.Select(y => y.Name.ToLower()).Contains(x.key.ToLower()) ||
                            StringProps.Select(y => y.Name.ToLower()).Contains(x.key.ToLower())).ToList();

                        IEnumerable<TLIradioOther> Installations = _unitOfWork.RadioOtherRepository.GetAllWithoutCount();

                        foreach (StringFilterObjectList InstallationProp in AttrInstAttributeFilters)
                        {
                            if (StringProps.Select(x => x.Name.ToLower()).Contains(InstallationProp.key.ToLower()))
                            {
                                Installations = Installations.Where(x => StringProps.AsEnumerable().FirstOrDefault(y => (InstallationProp.key.ToLower() == y.Name.ToLower()) && (InstallationProp.value.AsEnumerable().FirstOrDefault(w =>
                                     y.GetValue(_mapper.Map<RadioOtherViewModel>(x), null) != null ? y.GetValue(_mapper.Map<RadioOtherViewModel>(x), null).ToString().ToLower().StartsWith(w.ToLower()) : false) != null)) != null).AsEnumerable();
                            }
                            else if (NotStringProps.Select(x => x.Name.ToLower()).Contains(InstallationProp.key.ToLower()))
                            {
                                Installations = Installations.Where(x => NotStringProps.AsEnumerable().FirstOrDefault(y => (InstallationProp.key.ToLower() == y.Name.ToLower()) && (y.GetValue(_mapper.Map<RadioOtherViewModel>(x), null) != null ?
                                    InstallationProp.value.AsEnumerable().Contains(y.GetValue(_mapper.Map<RadioOtherViewModel>(x), null).ToString().ToLower()) : false)) != null).AsEnumerable();
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
                        WithoutDateFilterRadioOtherInstallation = InstallationAttributeActivated.Intersect(DynamicInstValueListIds).ToList();
                    }
                    else if (AttrInstExist)
                    {
                        WithoutDateFilterRadioOtherInstallation = InstallationAttributeActivated;
                    }
                    else if (DynamicInstExist)
                    {
                        WithoutDateFilterRadioOtherInstallation = DynamicInstValueListIds;
                    }
                }

                if (DateFilter != null ? DateFilter.Count() > 0 : false)
                {
                    List<TLIdynamicAtt> DateTimeInstDynamicAttListIds = _unitOfWork.DynamicAttRepository.GetIncludeWhere(x =>
                        AfterConvertDateFilters.AsEnumerable().Select(y => y.key.ToLower()).Contains(x.Key.ToLower()) &&
                        !x.LibraryAtt && !x.disable &&
                        x.tablesNames.TableName == Helpers.Constants.TablesNames.TLIradioOther.ToString()
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
                    List<PropertyInfo> InstallationProps = typeof(RadioOtherViewModel).GetProperties().Where(x =>
                        AfterConvertDateFilters.Select(y =>
                            y.key.ToLower()).Contains(x.Name.ToLower())).ToList();

                    List<int> InstallationAttributeActivatedIds = new List<int>();
                    bool AttrInstExist = false;

                    if (InstallationProps != null)
                    {
                        AttrInstExist = true;

                        List<DateFilterViewModel> InstallationPropsAttributeFilters = AfterConvertDateFilters.Where(x =>
                            InstallationProps.Select(y => y.Name.ToLower()).Contains(x.key.ToLower())).ToList();

                        IEnumerable<TLIradioOther> Installations = _unitOfWork.RadioOtherRepository.GetAllWithoutCount();

                        foreach (DateFilterViewModel InstallationProp in InstallationPropsAttributeFilters)
                        {
                            Installations = Installations.Where(x => InstallationProps.Exists(y => (InstallationProp.key.ToLower() == y.Name.ToLower()) && ((y.GetValue(_mapper.Map<RadioOtherViewModel>(x), null) != null) ?
                                ((InstallationProp.DateFrom.Date <= Convert.ToDateTime(y.GetValue(_mapper.Map<RadioOtherViewModel>(x), null)).Date) &&
                                    (InstallationProp.DateTo.Date >= Convert.ToDateTime(y.GetValue(_mapper.Map<RadioOtherViewModel>(x), null)).Date)) : (false))));
                        }

                        InstallationAttributeActivatedIds = Installations.Select(x => x.Id).ToList();
                    }

                    //
                    // Installation (Attribute Activated + Dynamic) Attributes...
                    //
                    if (AttrInstExist && DynamicInstExist)
                    {
                        WithDateFilterRadioOtherInstallation = InstallationAttributeActivatedIds.Intersect(DynamicInstValueListIds).ToList();
                    }
                    else if (AttrInstExist)
                    {
                        WithDateFilterRadioOtherInstallation = InstallationAttributeActivatedIds;
                    }
                    else if (DynamicInstExist)
                    {
                        WithDateFilterRadioOtherInstallation = DynamicInstValueListIds;
                    }
                }

                if ((DateFilter != null ? DateFilter.Count() > 0 : false) ||
                    (AttributeFilters != null ? AttributeFilters.Count() > 0 : false))
                {
                    if ((AttributeFilters != null ? AttributeFilters.Count > 0 : false) &&
                            (AfterConvertDateFilters != null ? AfterConvertDateFilters.Count() > 0 : false))
                    {
                        RadioOtherIds = WithoutDateFilterRadioOtherInstallation.Intersect(WithDateFilterRadioOtherInstallation).ToList();
                    }
                    else if (AttributeFilters != null ? AttributeFilters.Count > 0 : false)
                    {
                        RadioOtherIds = WithoutDateFilterRadioOtherInstallation;
                    }
                    else if (AfterConvertDateFilters != null ? AfterConvertDateFilters.Count() > 0 : false)
                    {
                        RadioOtherIds = WithDateFilterRadioOtherInstallation;
                    }

                    CivilLoadsRecords = _unitOfWork.CivilLoadsRepository.GetIncludeWhere(x =>
                        (x.allLoadInstId != null ? x.allLoadInst.radioOtherId != null : false) &&
                        (!x.Dismantle) &&
                        (x.SiteCode == BaseFilter.SiteCode) &&
                        (BaseFilter.ItemStatusId != null ? (
                            x.allLoadInst != null ? (
                                x.allLoadInst.ItemStatusId != null ?
                                        (x.allLoadInst.ItemStatusId == BaseFilter.ItemStatusId)
                                : false)
                            : false)
                        : true) &&
                        (BaseFilter.TicketId != null ? (
                            x.allLoadInst != null ? (
                                x.allLoadInst.TicketId != null ?
                                        (x.allLoadInst.TicketId == BaseFilter.TicketId)
                                : false)
                            : false)
                        : true) &&
                        (BaseFilter.AllCivilId != null ?
                            (x.allCivilInstId == BaseFilter.AllCivilId)
                        : true) &&

                        RadioOtherIds.Contains(x.allLoadInst.radioOtherId.Value),
                    x => x.allCivilInst, x => x.allLoadInst, x => x.allLoadInst.radioOther, x => x.allLoadInst.radioOther.installationPlace,
                    x => x.allLoadInst.radioOther.owner, x => x.allLoadInst.radioOther.radioOtherLibrary).ToList();
                }
                else
                {
                    CivilLoadsRecords = _unitOfWork.CivilLoadsRepository.GetIncludeWhere(x =>
                       (x.allLoadInstId != null ? x.allLoadInst.radioOtherId != null : false) &&
                       (!x.Dismantle) &&
                       (x.SiteCode == BaseFilter.SiteCode) &&
                       (BaseFilter.ItemStatusId != null ? (
                           x.allLoadInst != null ? (
                               x.allLoadInst.ItemStatusId != null ?
                                       (x.allLoadInst.ItemStatusId == BaseFilter.ItemStatusId)
                               : false)
                           : false)
                       : true) &&
                       (BaseFilter.TicketId != null ? (
                           x.allLoadInst != null ? (
                               x.allLoadInst.TicketId != null ?
                                       (x.allLoadInst.TicketId == BaseFilter.TicketId)
                               : false)
                           : false)
                       : true) &&
                       (BaseFilter.AllCivilId != null ?
                           (x.allCivilInstId == BaseFilter.AllCivilId)
                       : true),

                   x => x.allCivilInst, x => x.allLoadInst, x => x.allLoadInst.radioOther, x => x.allLoadInst.radioOther.installationPlace,
                   x => x.allLoadInst.radioOther.owner, x => x.allLoadInst.radioOther.radioOtherLibrary).ToList();
                }

                // Delete Duplicated Objects Based On Installation Date...
                List<TLIcivilLoads> NewList = new List<TLIcivilLoads>();
                foreach (var item in CivilLoadsRecords)
                {
                    TLIcivilLoads CheckIfExist = NewList.FirstOrDefault(x => x.allLoadInst.radioOtherId.Value == item.allLoadInst.radioOtherId.Value);
                    if (CheckIfExist != null)
                    {
                        if (CheckIfExist.InstallationDate < item.InstallationDate)
                        {
                            NewList.Remove(CheckIfExist);
                            NewList.Add(item);
                        }
                    }
                    else
                    {
                        NewList.Add(item);
                    }
                }
                CivilLoadsRecords = NewList;

                if (CivilId != null)
                {
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

                    List<int> RadioOthersIds = _unitOfWork.CivilLoadsRepository.GetIncludeWhere(x => x.allCivilInstId == AllCivilInst.Id && !x.Dismantle &&
                        (x.allLoadInstId != null ? x.allLoadInst.radioOtherId != null : false), x => x.allLoadInst).Select(x => x.allLoadInst.radioOtherId.Value).Distinct().ToList();

                    CivilLoadsRecords = CivilLoadsRecords.Where(x => RadioOthersIds.Contains(x.allLoadInst.radioOtherId.Value)).ToList();
                }

                if (BaseFilter.SideArmId != null)
                {
                    List<int> RadioOthersIds = _unitOfWork.CivilLoadsRepository.GetIncludeWhere(x => x.sideArmId == BaseFilter.SideArmId && !x.Dismantle &&
                        (x.allLoadInstId != null ? x.allLoadInst.radioOtherId != null : false), x => x.allLoadInst).Select(x => x.allLoadInst.radioOtherId.Value).Distinct().ToList();

                    CivilLoadsRecords = CivilLoadsRecords.Where(x => RadioOthersIds.Contains(x.allLoadInst.radioOtherId.Value)).ToList();
                }

                Count = CivilLoadsRecords.Count();

                CivilLoadsRecords = CivilLoadsRecords.Skip((parameterPagination.PageNumber - 1) * parameterPagination.PageSize).
                    Take(parameterPagination.PageSize).ToList();

                List<RadioOtherViewModel> Others = _mapper.Map<List<RadioOtherViewModel>>(CivilLoadsRecords.Select(x => x.allLoadInst.radioOther).ToList());

                List<TLIattributeViewManagment> AllAttributes = _unitOfWork.AttributeViewManagmentRepository.GetIncludeWhere(x =>
                   (x.Enable && x.EditableManagmentView.View == Helpers.Constants.EditableManamgmantViewNames.RadioOtherInstallation.ToString() &&
                   (x.AttributeActivatedId != null ?
                        (x.AttributeActivated.Tabel == Helpers.Constants.TablesNames.TLIradioOther.ToString() && x.AttributeActivated.enable) :
                        (!x.DynamicAtt.LibraryAtt && !x.DynamicAtt.disable && x.DynamicAtt.tablesNames.TableName == Helpers.Constants.TablesNames.TLIradioOther.ToString()))) ||
                    (x.AttributeActivated != null ?
                        ((x.AttributeActivated.Key.ToLower() == "id" || x.AttributeActivated.Key.ToLower() == "active") && x.AttributeActivated.Tabel == Helpers.Constants.TablesNames.TLIradioOther.ToString()) : false),
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


                foreach (RadioOtherViewModel OthersInstallationObject in Others)
                {
                    dynamic DynamiRadioOtherInstallation = new ExpandoObject();

                    //
                    // Installation Object ViewModel...
                    //
                    if (NotDateTimeInstallationAttributesViewModel != null ? NotDateTimeInstallationAttributesViewModel.Count > 0 : false)
                    {
                        List<PropertyInfo> InstallationProps = typeof(RadioOtherViewModel).GetProperties().Where(x =>
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
                                object ForeignKeyNamePropObject = prop.GetValue(OthersInstallationObject, null);
                                ((IDictionary<String, Object>)DynamiRadioOtherInstallation).Add(new KeyValuePair<string, object>(prop.Name, ForeignKeyNamePropObject));
                            }
                            else if (NotDateTimeInstallationAttributesViewModel.Select(x =>
                                 x.AttributeActivated.Key.ToLower()).Contains(prop.Name.ToLower()) &&
                                !prop.Name.ToLower().Contains("_name") &&
                                (prop.Name.ToLower().Substring(Math.Max(0, prop.Name.Length - 2)) != "id" || prop.Name.ToLower() == "id"))
                            {
                                if (prop.Name.ToLower() != "id" && prop.Name.ToLower() != "active")
                                {
                                    TLIattributeViewManagment LabelName = AllAttributes.FirstOrDefault(x => ((x.AttributeActivated != null) ? x.AttributeActivated.Key == prop.Name : false) &&
                                        x.AttributeActivated.Tabel == Helpers.Constants.TablesNames.TLIradioOther.ToString() &&
                                        x.Enable && x.AttributeActivated.DataType != "List" && x.Id != 0);

                                    if (LabelName != null)
                                    {
                                        object PropObject = prop.GetValue(OthersInstallationObject, null);
                                        ((IDictionary<String, Object>)DynamiRadioOtherInstallation).Add(new KeyValuePair<string, object>(LabelName.AttributeActivated.Label, PropObject));
                                    }
                                }
                                else
                                {
                                    object PropObject = prop.GetValue(OthersInstallationObject, null);
                                    ((IDictionary<String, Object>)DynamiRadioOtherInstallation).Add(new KeyValuePair<string, object>(prop.Name, PropObject));
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
                            !x.disable && x.tablesNames.TableName == Helpers.Constants.TablesNames.TLIradioOther.ToString() &&
                            !x.LibraryAtt && x.DataType.Name.ToLower() != "datetime" &&
                            NotDateTimeDynamicInstallationAttributesViewModel.Select(y => y.DynamicAttId).Any(y => y == x.Id)
                                , x => x.tablesNames, x => x.DataType).ToList();

                        List<TLIdynamicAttInstValue> NotDateTimeDynamicAttInstValues = _unitOfWork.DynamicAttInstValueRepository.GetIncludeWhere(x =>
                            !x.DynamicAtt.LibraryAtt && !x.disable &&
                            x.InventoryId == OthersInstallationObject.Id &&
                            NotDateTimeInstallationDynamicAttributes.AsEnumerable().Select(y => y.Key.ToLower()).Contains(x.DynamicAtt.Key.ToLower()) &&
                            x.tablesNames.TableName == Helpers.Constants.TablesNames.TLIradioOther.ToString()
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

                                ((IDictionary<String, Object>)DynamiRadioOtherInstallation).Add(new KeyValuePair<string, object>(InstallationDynamicAtt.Key, DynamicAttValue));
                            }
                            else
                            {
                                ((IDictionary<String, Object>)DynamiRadioOtherInstallation).Add(new KeyValuePair<string, object>(InstallationDynamicAtt.Key, null));
                            }
                        }
                    }

                    //
                    // Installation Object ViewModel... (DateTime DataType Attribute)
                    //
                    dynamic DateTimeAttributes = new ExpandoObject();
                    if (DateTimeInstallationAttributesViewModel != null ? DateTimeInstallationAttributesViewModel.Count() > 0 : false)
                    {
                        List<PropertyInfo> DateTimeInstallationProps = typeof(RadioOtherViewModel).GetProperties().Where(x =>
                            x.PropertyType.GenericTypeArguments != null ?
                                (x.PropertyType.GenericTypeArguments.Count() > 0 ? x.PropertyType.GenericTypeArguments.FirstOrDefault().Name.ToLower() == "datetime" :
                                (x.PropertyType.Name.ToLower() == "datetime")) :
                            (x.PropertyType.Name.ToLower() == "datetime")).ToList();

                        foreach (PropertyInfo prop in DateTimeInstallationProps)
                        {
                            TLIattributeViewManagment LabelName = AllAttributes.FirstOrDefault(x => ((x.AttributeActivated != null) ? x.AttributeActivated.Key == prop.Name : false) &&
                                x.AttributeActivated.Tabel == Helpers.Constants.TablesNames.TLIradioOther.ToString() &&
                                x.Enable && x.AttributeActivated.DataType != "List" && x.Id != 0);

                            if (LabelName != null)
                            {
                                object PropObject = prop.GetValue(OthersInstallationObject, null);
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
                           !x.disable && x.tablesNames.TableName == Helpers.Constants.TablesNames.TLIradioOther.ToString() &&
                           !x.LibraryAtt && x.DataType.Name.ToLower() == "datetime" &&
                            DateTimeDynamicInstallationAttributesViewModel.Select(y => y.DynamicAttId).Any(y => y == x.Id), x => x.tablesNames).ToList();

                        List<TLIdynamicAttInstValue> DateTimeDynamicAttInstValues = _unitOfWork.DynamicAttInstValueRepository.GetIncludeWhere(x =>
                            x.InventoryId == OthersInstallationObject.Id && !x.disable &&
                           !x.DynamicAtt.LibraryAtt &&
                            DateTimeInstallationDynamicAttributes.AsEnumerable().Select(y => y.Key.ToLower()).Contains(x.DynamicAtt.Key.ToLower()) &&
                            x.tablesNames.TableName == Helpers.Constants.TablesNames.TLIradioOther.ToString()
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

                    ((IDictionary<String, Object>)DynamiRadioOtherInstallation).Add(new KeyValuePair<string, object>("DateTimeAttributes", DateTimeAttributes));

                    OutPutList.Add(DynamiRadioOtherInstallation);
                }
                RadioOtheresTableDisplay.Model = OutPutList;

                if (WithFilterData == true)
                {
                    RadioOtheresTableDisplay.filters = _unitOfWork.RadioOtherRepository.GetRelatedTables();
                }
                else
                {
                    RadioOtheresTableDisplay.filters = null;
                }

                return new Response<ReturnWithFilters<object>>(true, RadioOtheresTableDisplay, null, null, (int)Helpers.Constants.ApiReturnCode.success, Count);
            }
            catch (Exception err)
            {
                return new Response<ReturnWithFilters<object>>(false, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
        }

        // 9. Get Power...
        public Response<ReturnWithFilters<object>> GetPowerOnSiteWithEnableAtt(LoadsOnSiteFilter BaseFilter, bool WithFilterData, CombineFilters CombineFilters, ParameterPagination parameterPagination, int? CivilId, string CivilType)
        {
            try
            {
                int Count = 0;
                List<FilterObjectList> ObjectAttributeFilters = CombineFilters.filters;
                List<DateFilterViewModel> DateFilter = CombineFilters.DateFilter;

                List<object> OutPutList = new List<object>();
                ReturnWithFilters<object> PoweresTableDisplay = new ReturnWithFilters<object>();

                List<TLIcivilLoads> CivilLoadsRecords = new List<TLIcivilLoads>();

                List<StringFilterObjectList> AttributeFilters = new List<StringFilterObjectList>();
                List<AttributeActivatedViewModel> PowerInstallationAttribute = new List<AttributeActivatedViewModel>();

                if ((DateFilter != null ? DateFilter.Count() > 0 : false) ||
                    (ObjectAttributeFilters != null && ObjectAttributeFilters.Count > 0))
                {
                    PowerInstallationAttribute = _mapper.Map<List<AttributeActivatedViewModel>>(_unitOfWork.AttributeViewManagmentRepository.GetIncludeWhere(x =>
                        x.Enable && x.AttributeActivatedId != null &&
                        x.EditableManagmentView.View == Helpers.Constants.EditableManamgmantViewNames.PowerInstallation.ToString() &&
                        x.EditableManagmentView.TLItablesNames1.TableName == Helpers.Constants.TablesNames.TLIpower.ToString(),
                            x => x.AttributeActivated, x => x.EditableManagmentView, x => x.EditableManagmentView.TLItablesNames1,
                            x => x.EditableManagmentView.TLItablesNames2)
                    .Select(x => x.AttributeActivated).ToList());
                }

                if (ObjectAttributeFilters != null && ObjectAttributeFilters.Count > 0)
                {
                    List<AttributeActivatedViewModel> NotDateDatePowerInstallationAttribute = PowerInstallationAttribute.Where(x =>
                        x.DataType.ToLower() != "datetime").ToList();

                    foreach (FilterObjectList item in ObjectAttributeFilters)
                    {
                        List<string> value = item.value.Select(x => x.ToString().ToLower()).ToList();

                        AttributeActivatedViewModel AttributeKey = NotDateDatePowerInstallationAttribute.FirstOrDefault(x =>
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
                    List<AttributeActivatedViewModel> DatePowerInstallationAttribute = PowerInstallationAttribute.Where(x =>
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

                        AttributeActivatedViewModel AttributeKey = DatePowerInstallationAttribute.FirstOrDefault(x =>
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

                List<int> powerIds = new List<int>();
                List<int> WithoutDateFilterPowerInstallation = new List<int>();
                List<int> WithDateFilterPowerInstallation = new List<int>();

                if (AttributeFilters != null && AttributeFilters.Count > 0)
                {
                    //
                    // Installation Dynamic Attributes...
                    //
                    List<TLIdynamicAtt> InstDynamicAttListIds = _unitOfWork.DynamicAttRepository.GetIncludeWhere(x =>
                        AttributeFilters.AsEnumerable().Select(y => y.key.ToLower()).Contains(x.Key.ToLower()) &&
                        !x.LibraryAtt && !x.disable &&
                        x.tablesNames.TableName == Helpers.Constants.TablesNames.TLIpower.ToString()
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
                    bool AttrInstExist = typeof(PowerViewModel).GetProperties().ToList().Exists(x =>
                        AttributeFilters.Where(y => y.key.ToLower() != "Id".ToLower()).Select(y =>
                            y.key.ToLower()).Contains(x.Name.ToLower()));

                    List<int> InstallationAttributeActivated = new List<int>();
                    if (AttrInstExist)
                    {
                        List<PropertyInfo> NotStringProps = typeof(PowerViewModel).GetProperties().Where(x =>
                            x.PropertyType.Name.ToLower() != "string" &&
                                AttributeFilters.Select(y =>
                                    y.key.ToLower()).Contains(x.Name.ToLower())).ToList();

                        List<PropertyInfo> StringProps = typeof(PowerViewModel).GetProperties().Where(x =>
                            x.PropertyType.Name.ToLower() == "string" &&
                                AttributeFilters.Select(y =>
                                    y.key.ToLower()).Contains(x.Name.ToLower())).ToList();

                        List<StringFilterObjectList> AttrInstAttributeFilters = AttributeFilters.Where(x =>
                            NotStringProps.Select(y => y.Name.ToLower()).Contains(x.key.ToLower()) ||
                            StringProps.Select(y => y.Name.ToLower()).Contains(x.key.ToLower())).ToList();

                        //InstallationAttributeActivated = _unitOfWork.PowerRepository.GetWhere(x =>
                        //    AttrInstAttributeFilters.All(z =>
                        //    NotStringProps.Exists(y => (z.key.ToLower() == y.Name.ToLower()) && (y.GetValue(_mapper.Map<PowerViewModel>(x), null) != null ? z.value.Contains(y.GetValue(_mapper.Map<PowerViewModel>(x), null).ToString().ToLower()) : false)) ||
                        //    StringProps.Exists(y => (z.key.ToLower() == y.Name.ToLower()) && (z.value.Any(w =>
                        //            y.GetValue(_mapper.Map<PowerViewModel>(x), null) != null ? y.GetValue(_mapper.Map<PowerViewModel>(x), null).ToString().ToLower().StartsWith(w.ToLower()) : false))))
                        //).Select(i => i.Id).ToList();

                        IEnumerable<TLIpower> Installations = _unitOfWork.PowerRepository.GetAllWithoutCount();

                        foreach (StringFilterObjectList InstallationProp in AttrInstAttributeFilters)
                        {
                            if (StringProps.Select(x => x.Name.ToLower()).Contains(InstallationProp.key.ToLower()))
                            {
                                Installations = Installations.Where(x => StringProps.AsEnumerable().FirstOrDefault(y => (InstallationProp.key.ToLower() == y.Name.ToLower()) && (InstallationProp.value.AsEnumerable().FirstOrDefault(w =>
                                     y.GetValue(_mapper.Map<PowerViewModel>(x), null) != null ? y.GetValue(_mapper.Map<PowerViewModel>(x), null).ToString().ToLower().StartsWith(w.ToLower()) : false) != null)) != null).AsEnumerable();
                            }
                            else if (NotStringProps.Select(x => x.Name.ToLower()).Contains(InstallationProp.key.ToLower()))
                            {
                                Installations = Installations.Where(x => NotStringProps.AsEnumerable().FirstOrDefault(y => (InstallationProp.key.ToLower() == y.Name.ToLower()) && (y.GetValue(_mapper.Map<PowerViewModel>(x), null) != null ?
                                    InstallationProp.value.AsEnumerable().Contains(y.GetValue(_mapper.Map<PowerViewModel>(x), null).ToString().ToLower()) : false)) != null).AsEnumerable();
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
                        WithoutDateFilterPowerInstallation = InstallationAttributeActivated.Intersect(DynamicInstValueListIds).ToList();
                    }
                    else if (AttrInstExist)
                    {
                        WithoutDateFilterPowerInstallation = InstallationAttributeActivated;
                    }
                    else if (DynamicInstExist)
                    {
                        WithoutDateFilterPowerInstallation = DynamicInstValueListIds;
                    }
                }

                if (DateFilter != null ? DateFilter.Count() > 0 : false)
                {
                    List<TLIdynamicAtt> DateTimeInstDynamicAttListIds = _unitOfWork.DynamicAttRepository.GetIncludeWhere(x =>
                        AfterConvertDateFilters.AsEnumerable().Select(y => y.key.ToLower()).Contains(x.Key.ToLower()) &&
                        !x.LibraryAtt && !x.disable &&
                        x.tablesNames.TableName == Helpers.Constants.TablesNames.TLIpower.ToString()
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
                    List<PropertyInfo> InstallationProps = typeof(PowerViewModel).GetProperties().Where(x =>
                        AfterConvertDateFilters.Select(y =>
                            y.key.ToLower()).Contains(x.Name.ToLower())).ToList();

                    List<int> InstallationAttributeActivatedIds = new List<int>();
                    bool AttrInstExist = false;

                    if (InstallationProps != null)
                    {
                        AttrInstExist = true;

                        List<DateFilterViewModel> InstallationPropsAttributeFilters = AfterConvertDateFilters.Where(x =>
                            InstallationProps.Select(y => y.Name.ToLower()).Contains(x.key.ToLower())).ToList();

                        //InstallationAttributeActivatedIds = _unitOfWork.PowerRepository.GetWhere(x =>
                        //    InstallationPropsAttributeFilters.All(z =>
                        //        (InstallationProps.Exists(y => (z.key.ToLower() == y.Name.ToLower()) && ((y.GetValue(_mapper.Map<PowerViewModel>(x), null) != null) ?
                        //            ((z.DateFrom <= Convert.ToDateTime(y.GetValue(_mapper.Map<PowerViewModel>(x), null))) &&
                        //             (z.DateTo >= Convert.ToDateTime(y.GetValue(_mapper.Map<PowerViewModel>(x), null)))) : (false)))))
                        //).Select(i => i.Id).ToList();

                        IEnumerable<TLIpower> Installations = _unitOfWork.PowerRepository.GetAllWithoutCount();

                        foreach (DateFilterViewModel InstallationProp in InstallationPropsAttributeFilters)
                        {
                            Installations = Installations.Where(x => InstallationProps.Exists(y => (InstallationProp.key.ToLower() == y.Name.ToLower()) && ((y.GetValue(_mapper.Map<PowerViewModel>(x), null) != null) ?
                                ((InstallationProp.DateFrom.Date <= Convert.ToDateTime(y.GetValue(_mapper.Map<PowerViewModel>(x), null)).Date) &&
                                    (InstallationProp.DateTo.Date >= Convert.ToDateTime(y.GetValue(_mapper.Map<PowerViewModel>(x), null)).Date)) : (false))));
                        }

                        InstallationAttributeActivatedIds = Installations.Select(x => x.Id).ToList();
                    }

                    //
                    // Installation (Attribute Activated + Dynamic) Attributes...
                    //
                    if (AttrInstExist && DynamicInstExist)
                    {
                        WithDateFilterPowerInstallation = InstallationAttributeActivatedIds.Intersect(DynamicInstValueListIds).ToList();
                    }
                    else if (AttrInstExist)
                    {
                        WithDateFilterPowerInstallation = InstallationAttributeActivatedIds;
                    }
                    else if (DynamicInstExist)
                    {
                        WithDateFilterPowerInstallation = DynamicInstValueListIds;
                    }
                }

                if ((DateFilter != null ? DateFilter.Count() > 0 : false) ||
                    (AttributeFilters != null ? AttributeFilters.Count() > 0 : false))
                {
                    if ((AttributeFilters != null ? AttributeFilters.Count > 0 : false) &&
                            (AfterConvertDateFilters != null ? AfterConvertDateFilters.Count() > 0 : false))
                    {
                        powerIds = WithoutDateFilterPowerInstallation.Intersect(WithDateFilterPowerInstallation).ToList();
                    }
                    else if (AttributeFilters != null ? AttributeFilters.Count > 0 : false)
                    {
                        powerIds = WithoutDateFilterPowerInstallation;
                    }
                    else if (AfterConvertDateFilters != null ? AfterConvertDateFilters.Count() > 0 : false)
                    {
                        powerIds = WithDateFilterPowerInstallation;
                    }

                    CivilLoadsRecords = _unitOfWork.CivilLoadsRepository.GetIncludeWhere(x =>
                        (x.allLoadInstId != null ? x.allLoadInst.powerId != null : false) &&
                        (!x.Dismantle) &&
                        (x.SiteCode == BaseFilter.SiteCode) &&
                        (BaseFilter.ItemStatusId != null ? (
                            x.allLoadInst != null ? (
                                x.allLoadInst.ItemStatusId != null ?
                                        (x.allLoadInst.ItemStatusId == BaseFilter.ItemStatusId)
                                : false)
                            : false)
                        : true) &&
                        (BaseFilter.TicketId != null ? (
                            x.allLoadInst != null ? (
                                x.allLoadInst.TicketId != null ?
                                        (x.allLoadInst.TicketId == BaseFilter.TicketId)
                                : false)
                            : false)
                        : true) &&
                        (BaseFilter.AllCivilId != null ?
                            (x.allCivilInstId == BaseFilter.AllCivilId)
                        : true) &&
                        powerIds.Contains(x.allLoadInst.powerId.Value),
                    x => x.allCivilInst, x => x.allLoadInst, x => x.allLoadInst.power, x => x.allLoadInst.power.installationPlace,
                    x => x.allLoadInst.power.owner, x => x.allLoadInst.power.powerLibrary, x => x.allLoadInst.power.powerType).ToList();
                }
                else
                {
                    CivilLoadsRecords = _unitOfWork.CivilLoadsRepository.GetIncludeWhere(x =>
                       (x.allLoadInstId != null ? x.allLoadInst.powerId != null : false) &&
                       (!x.Dismantle) &&
                       (x.SiteCode == BaseFilter.SiteCode) &&
                       (BaseFilter.ItemStatusId != null ? (
                           x.allLoadInst != null ? (
                               x.allLoadInst.ItemStatusId != null ?
                                       (x.allLoadInst.ItemStatusId == BaseFilter.ItemStatusId)
                               : false)
                           : false)
                       : true) &&
                       (BaseFilter.TicketId != null ? (
                           x.allLoadInst != null ? (
                               x.allLoadInst.TicketId != null ?
                                       (x.allLoadInst.TicketId == BaseFilter.TicketId)
                               : false)
                           : false)
                       : true) &&
                       (BaseFilter.AllCivilId != null ?
                           (x.allCivilInstId == BaseFilter.AllCivilId)
                       : true),

                    x => x.allCivilInst, x => x.allLoadInst, x => x.allLoadInst.power, x => x.allLoadInst.power.installationPlace,
                    x => x.allLoadInst.power.owner, x => x.allLoadInst.power.powerLibrary, x => x.allLoadInst.power.powerType).ToList();
                }

                // Delete Duplicated Objects Based On Installation Date...
                List<TLIcivilLoads> NewList = new List<TLIcivilLoads>();
                foreach (var item in CivilLoadsRecords)
                {
                    TLIcivilLoads CheckIfExist = NewList.FirstOrDefault(x => x.allLoadInst.powerId.Value == item.allLoadInst.powerId.Value);
                    if (CheckIfExist != null)
                    {
                        if (CheckIfExist.InstallationDate < item.InstallationDate)
                        {
                            NewList.Remove(CheckIfExist);
                            NewList.Add(item);
                        }
                    }
                    else
                    {
                        NewList.Add(item);
                    }
                }
                CivilLoadsRecords = NewList;

                if (CivilId != null)
                {
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

                    List<int> PowerIds = _unitOfWork.CivilLoadsRepository.GetIncludeWhere(x => x.allCivilInstId == AllCivilInst.Id && !x.Dismantle &&
                        (x.allLoadInstId != null ? x.allLoadInst.powerId != null : false), x => x.allLoadInst).Select(x => x.allLoadInst.powerId.Value).Distinct().ToList();

                    CivilLoadsRecords = CivilLoadsRecords.Where(x => PowerIds.Contains(x.allLoadInst.powerId.Value)).ToList();
                }

                if (BaseFilter.SideArmId != null)
                {
                    List<int> PowerIds = _unitOfWork.CivilLoadsRepository.GetIncludeWhere(x => x.sideArmId == BaseFilter.SideArmId && !x.Dismantle &&
                        (x.allLoadInstId != null ? x.allLoadInst.powerId != null : false), x => x.allLoadInst).Select(x => x.allLoadInst.powerId.Value).Distinct().ToList();

                    CivilLoadsRecords = CivilLoadsRecords.Where(x => PowerIds.Contains(x.allLoadInst.powerId.Value)).ToList();
                }

                Count = CivilLoadsRecords.Count();

                CivilLoadsRecords = CivilLoadsRecords.Skip((parameterPagination.PageNumber - 1) * parameterPagination.PageSize).
                    Take(parameterPagination.PageSize).ToList();

                List<PowerViewModel> Others = _mapper.Map<List<PowerViewModel>>(CivilLoadsRecords.Select(x => x.allLoadInst.power).ToList());

                List<TLIattributeViewManagment> AllAttributes = _unitOfWork.AttributeViewManagmentRepository.GetIncludeWhere(x =>
                   (x.Enable && x.EditableManagmentView.View == Helpers.Constants.EditableManamgmantViewNames.PowerInstallation.ToString() &&
                   (x.AttributeActivatedId != null ?
                        (x.AttributeActivated.Tabel == Helpers.Constants.TablesNames.TLIpower.ToString() && x.AttributeActivated.enable) :
                        (!x.DynamicAtt.LibraryAtt && !x.DynamicAtt.disable && x.DynamicAtt.tablesNames.TableName == Helpers.Constants.TablesNames.TLIpower.ToString()))) ||
                    (x.AttributeActivated != null ?
                        ((x.AttributeActivated.Key.ToLower() == "id" || x.AttributeActivated.Key.ToLower() == "active") && x.AttributeActivated.Tabel == Helpers.Constants.TablesNames.TLIpower.ToString()) : false),
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


                foreach (PowerViewModel OthersInstallationObject in Others)
                {
                    dynamic DynamiPowerInstallation = new ExpandoObject();

                    //
                    // Installation Object ViewModel...
                    //
                    if (NotDateTimeInstallationAttributesViewModel != null ? NotDateTimeInstallationAttributesViewModel.Count > 0 : false)
                    {
                        List<PropertyInfo> InstallationProps = typeof(PowerViewModel).GetProperties().Where(x =>
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
                                object ForeignKeyNamePropObject = prop.GetValue(OthersInstallationObject, null);
                                ((IDictionary<String, Object>)DynamiPowerInstallation).Add(new KeyValuePair<string, object>(prop.Name, ForeignKeyNamePropObject));
                            }
                            else if (NotDateTimeInstallationAttributesViewModel.Select(x =>
                                 x.AttributeActivated.Key.ToLower()).Contains(prop.Name.ToLower()) &&
                                !prop.Name.ToLower().Contains("_name") &&
                                (prop.Name.ToLower().Substring(Math.Max(0, prop.Name.Length - 2)) != "id" || prop.Name.ToLower() == "id"))
                            {
                                if (prop.Name.ToLower() != "id" && prop.Name.ToLower() != "active")
                                {
                                    TLIattributeViewManagment LabelName = AllAttributes.FirstOrDefault(x => ((x.AttributeActivated != null) ? x.AttributeActivated.Key == prop.Name : false) &&
                                        x.AttributeActivated.Tabel == Helpers.Constants.TablesNames.TLIpower.ToString() &&
                                        x.Enable && x.AttributeActivated.DataType != "List" && x.Id != 0);

                                    if (LabelName != null)
                                    {
                                        object PropObject = prop.GetValue(OthersInstallationObject, null);
                                        ((IDictionary<String, Object>)DynamiPowerInstallation).Add(new KeyValuePair<string, object>(LabelName.AttributeActivated.Label, PropObject));
                                    }
                                }
                                else
                                {
                                    object PropObject = prop.GetValue(OthersInstallationObject, null);
                                    ((IDictionary<String, Object>)DynamiPowerInstallation).Add(new KeyValuePair<string, object>(prop.Name, PropObject));
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
                            !x.disable && x.tablesNames.TableName == Helpers.Constants.TablesNames.TLIpower.ToString() &&
                            !x.LibraryAtt && x.DataType.Name.ToLower() != "datetime" &&
                            NotDateTimeDynamicInstallationAttributesViewModel.Select(y => y.DynamicAttId).Any(y => y == x.Id)
                                , x => x.tablesNames, x => x.DataType).ToList();

                        List<TLIdynamicAttInstValue> NotDateTimeDynamicAttInstValues = _unitOfWork.DynamicAttInstValueRepository.GetIncludeWhere(x =>
                            !x.DynamicAtt.LibraryAtt && !x.disable &&
                            x.InventoryId == OthersInstallationObject.Id &&
                            NotDateTimeInstallationDynamicAttributes.AsEnumerable().Select(y => y.Key.ToLower()).Contains(x.DynamicAtt.Key.ToLower()) &&
                            x.tablesNames.TableName == Helpers.Constants.TablesNames.TLIpower.ToString()
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

                                ((IDictionary<String, Object>)DynamiPowerInstallation).Add(new KeyValuePair<string, object>(InstallationDynamicAtt.Key, DynamicAttValue));
                            }
                            else
                            {
                                ((IDictionary<String, Object>)DynamiPowerInstallation).Add(new KeyValuePair<string, object>(InstallationDynamicAtt.Key, null));
                            }
                        }
                    }

                    //
                    // Installation Object ViewModel... (DateTime DataType Attribute)
                    //
                    dynamic DateTimeAttributes = new ExpandoObject();
                    if (DateTimeInstallationAttributesViewModel != null ? DateTimeInstallationAttributesViewModel.Count() > 0 : false)
                    {
                        List<PropertyInfo> DateTimeInstallationProps = typeof(PowerViewModel).GetProperties().Where(x =>
                            x.PropertyType.GenericTypeArguments != null ?
                                (x.PropertyType.GenericTypeArguments.Count() > 0 ? x.PropertyType.GenericTypeArguments.FirstOrDefault().Name.ToLower() == "datetime" :
                                (x.PropertyType.Name.ToLower() == "datetime")) :
                            (x.PropertyType.Name.ToLower() == "datetime")).ToList();

                        foreach (PropertyInfo prop in DateTimeInstallationProps)
                        {
                            TLIattributeViewManagment LabelName = AllAttributes.FirstOrDefault(x => ((x.AttributeActivated != null) ? x.AttributeActivated.Key == prop.Name : false) &&
                                x.AttributeActivated.Tabel == Helpers.Constants.TablesNames.TLIpower.ToString() &&
                                x.Enable && x.AttributeActivated.DataType != "List" && x.Id != 0);

                            if (LabelName != null)
                            {
                                object PropObject = prop.GetValue(OthersInstallationObject, null);
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
                           !x.disable && x.tablesNames.TableName == Helpers.Constants.TablesNames.TLIpower.ToString() &&
                           !x.LibraryAtt && x.DataType.Name.ToLower() == "datetime" &&
                            DateTimeDynamicInstallationAttributesViewModel.Select(y => y.DynamicAttId).Any(y => y == x.Id), x => x.tablesNames).ToList();

                        List<TLIdynamicAttInstValue> DateTimeDynamicAttInstValues = _unitOfWork.DynamicAttInstValueRepository.GetIncludeWhere(x =>
                            x.InventoryId == OthersInstallationObject.Id && !x.disable &&
                           !x.DynamicAtt.LibraryAtt &&
                            DateTimeInstallationDynamicAttributes.AsEnumerable().Select(y => y.Key.ToLower()).Contains(x.DynamicAtt.Key.ToLower()) &&
                            x.tablesNames.TableName == Helpers.Constants.TablesNames.TLIpower.ToString()
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

                    ((IDictionary<String, Object>)DynamiPowerInstallation).Add(new KeyValuePair<string, object>("DateTimeAttributes", DateTimeAttributes));

                    OutPutList.Add(DynamiPowerInstallation);
                }
                PoweresTableDisplay.Model = OutPutList;

                if (WithFilterData == true)
                {
                    PoweresTableDisplay.filters = _unitOfWork.PowerRepository.GetRelatedTables();
                }
                else
                {
                    PoweresTableDisplay.filters = null;
                }

                return new Response<ReturnWithFilters<object>>(true, PoweresTableDisplay, null, null, (int)Helpers.Constants.ApiReturnCode.success, Count);
            }
            catch (Exception err)
            {
                return new Response<ReturnWithFilters<object>>(false, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
        }
        #endregion
        #region Helper Methods..
        public DynamicAttDto GetDynamicAttDto(TLIdynamicAttInstValue DynamicAttInstValueRecord, TLIdynamicAttLibValue DynamicAttLibRecord)
        {
            DynamicAttDto DynamicAttDto = new DynamicAttDto
            {
                DataType = new DataTypeViewModel(),
                DynamicAttInstValue = new DynamicAttInstValueViewModel(),
                DynamicAttLibValue = new DynamicAttLibValueViewMdodel()
            };
            if (DynamicAttInstValueRecord != null)
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
            else if (DynamicAttLibRecord != null)
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
        #endregion

        // string Baseurl = "https://localhost:44311";
        public async Task<string> GetSMIS_Site(string UserName, string Password, string ViewName, string Paramater, string RowContent)
        {
            try
            {
                ServiceProvider serviceProvider = _services.BuildServiceProvider();
                IConfiguration Configuration = serviceProvider.GetService<IConfiguration>();

                HttpWebRequest Request = !string.IsNullOrEmpty(Paramater) ?
                    (HttpWebRequest)WebRequest.Create(Configuration["SMIS_API_URL"] + $"{UserName}/{Password}/{ViewName}/'{Paramater}'") :
                    (HttpWebRequest)WebRequest.Create(Configuration["SMIS_API_URL"] + $"{UserName}/{Password}/{ViewName}");

                Request.Method = "POST";

                if (!string.IsNullOrEmpty(RowContent))
                {
                    Request.ContentType = "text/plain";

                    ASCIIEncoding encoding = new ASCIIEncoding();
                    byte[] BodyText = encoding.GetBytes(RowContent);

                    Stream NewStream = Request.GetRequestStream();
                    NewStream.Write(BodyText, 0, BodyText.Length);
                    Request.ContentLength = BodyText.Length;
                }

                string SMIS_Response = "";
                using (WebResponse WebResponse = Request.GetResponse())
                {
                    using (StreamReader Reader = new StreamReader(WebResponse.GetResponseStream()))
                    {
                        SMIS_Response = Reader.ReadToEnd();
                    }
                }

                List<SiteDataFromOutsiderApiViewModel> SiteViewModelLists = new JavaScriptSerializer().Deserialize<List<SiteDataFromOutsiderApiViewModel>>(SMIS_Response);

                using (TransactionScope transaction = new TransactionScope())
                {
                    foreach (SiteDataFromOutsiderApiViewModel item in SiteViewModelLists)
                    {
                        TLIsite CheckSiteCodeIfExist = _unitOfWork.SiteRepository
                            .GetWhereFirst(x => x.SiteCode.ToLower() == item.Sitecode.ToLower() ||
                                x.SiteName.ToLower() == item.Sitename.ToLower());

                        if (CheckSiteCodeIfExist != null)
                        {
                            CheckSiteCodeIfExist.SiteCode = item.Sitecode;
                            CheckSiteCodeIfExist.SiteName = item.Sitename;
                            CheckSiteCodeIfExist.LocationType = item.LocationType;
                            CheckSiteCodeIfExist.Latitude = item.Latitude;
                            CheckSiteCodeIfExist.Longitude = item.Longtude;
                            CheckSiteCodeIfExist.Zone = item.Zone;
                            CheckSiteCodeIfExist.SubArea = item.Subarea;
                            CheckSiteCodeIfExist.STATUS_DATE = item.Statusdate;
                            CheckSiteCodeIfExist.CREATE_DATE = item.Createddate;
                            CheckSiteCodeIfExist.LocationHieght = item.LocationHieght;

                            TLIarea CheckAreaIfExist = _unitOfWork.AreaRepository
                                .GetWhereFirst(x => x.AreaName.ToLower() == item.Area.ToLower());

                            if (CheckAreaIfExist == null)
                            {
                                TLIarea AddNewArea = new TLIarea
                                {
                                    AreaName = item.Area
                                };
                                await _unitOfWork.AreaRepository.AddAsync(AddNewArea);
                                await _unitOfWork.SaveChangesAsync();

                                CheckSiteCodeIfExist.AreaId = AddNewArea.Id;
                            }
                            else
                            {
                                CheckSiteCodeIfExist.AreaId = CheckAreaIfExist.Id;
                            }

                            TLIregion CheckRegonIfExist = await _context.TLIregion
                                .FirstOrDefaultAsync(x => x.RegionCode.ToLower() == item.Rejoncode.ToLower());

                            if (CheckRegonIfExist == null)
                            {
                                await _context.TLIregion.AddAsync(new TLIregion { RegionCode = item.Rejoncode });
                                await _context.SaveChangesAsync();

                                CheckSiteCodeIfExist.RegionCode = item.Rejoncode;
                            }
                            else
                            {
                                CheckSiteCodeIfExist.RegionCode = CheckRegonIfExist.RegionCode;
                            }

                            // Mostafa Al-Homsi Answered That this Column's Value Will Always Be (NULL)..
                            TLIsiteStatus CheckSiteStatusIfExist = _unitOfWork.SiteStatusRepository.GetWhereFirst(x => x.Id == 0);

                            if (CheckSiteStatusIfExist == null)
                            {
                                TLIsiteStatus AddNewSiteStatus = new TLIsiteStatus
                                {
                                    Id = 0,
                                    Name = "NA",
                                    Active = true,
                                    DateDeleted = null,
                                    Deleted = false
                                };
                                await _unitOfWork.SiteStatusRepository.AddAsync(AddNewSiteStatus);
                                await _unitOfWork.SaveChangesAsync();

                                CheckSiteCodeIfExist.siteStatusId = AddNewSiteStatus.Id;
                            }
                            else
                            {
                                CheckSiteCodeIfExist.siteStatusId = CheckSiteStatusIfExist.Id;
                            }

                            await _unitOfWork.SaveChangesAsync();
                        }
                        else
                        {
                            TLIsite NewSiteToAdd = new TLIsite();

                            NewSiteToAdd.SiteCode = item.Sitecode;
                            TLIregion CheckRegonIfExist = await _context.TLIregion
                                .FirstOrDefaultAsync(x => x.RegionCode.ToLower() == item.Rejoncode.ToLower());

                            if (CheckRegonIfExist == null)
                            {
                                await _context.TLIregion.AddAsync(new TLIregion { RegionCode = item.Rejoncode });
                                await _context.SaveChangesAsync();

                                NewSiteToAdd.RegionCode = item.Rejoncode;
                            }
                            else
                            {
                                NewSiteToAdd.RegionCode = CheckRegonIfExist.RegionCode;
                            }

                            TLIarea CheckAreaIfExist = _unitOfWork.AreaRepository
                                .GetWhereFirst(x => x.AreaName.ToLower() == item.Area.ToLower());

                            if (CheckAreaIfExist == null)
                            {
                                TLIarea AddNewArea = new TLIarea
                                {
                                    AreaName = item.Area
                                };
                                await _unitOfWork.AreaRepository.AddAsync(AddNewArea);
                                await _unitOfWork.SaveChangesAsync();

                                NewSiteToAdd.AreaId = AddNewArea.Id;
                            }
                            else
                            {
                                NewSiteToAdd.AreaId = CheckAreaIfExist.Id;
                            }

                            // Mostafa Al-Homsi Answered That this Column's Value Will Always Be (NULL)..
                            TLIsiteStatus CheckSiteStatusIfExist = _unitOfWork.SiteStatusRepository.GetWhereFirst(x => x.Id == 0);

                            if (CheckSiteStatusIfExist == null)
                            {
                                TLIsiteStatus AddNewSiteStatus = new TLIsiteStatus
                                {
                                    Id = 0,
                                    Name = "NA",
                                    Active = true,
                                    DateDeleted = null,
                                    Deleted = false
                                };
                                await _unitOfWork.SiteStatusRepository.AddAsync(AddNewSiteStatus);
                                await _unitOfWork.SaveChangesAsync();

                                NewSiteToAdd.siteStatusId = AddNewSiteStatus.Id;
                            }
                            else
                            {
                                NewSiteToAdd.siteStatusId = CheckSiteStatusIfExist.Id;
                            }

                            NewSiteToAdd.Latitude = item.Latitude;
                            NewSiteToAdd.Longitude = item.Longtude;
                            NewSiteToAdd.Zone = item.Zone;
                            NewSiteToAdd.SubArea = item.Subarea;
                            NewSiteToAdd.STATUS_DATE = item.Statusdate;
                            NewSiteToAdd.CREATE_DATE = item.Createddate;
                            NewSiteToAdd.LocationHieght = item.LocationHieght;
                            NewSiteToAdd.LocationType = item.LocationType;

                            await _unitOfWork.SiteRepository.AddAsync(NewSiteToAdd);
                            await _unitOfWork.SaveChangesAsync();
                        }
                    }

                    _MySites = _context.TLIsite.AsNoTracking()
                        .Include(x => x.siteStatus).Include(x => x.Region).Include(x => x.AreaId).ToList();

                    transaction.Complete();
                }
                return "No Error Found";
            }
            catch (Exception err)
            {
                return err.Message;
            }
        }
        public Response<List<RegionViewModel>> GetAllRegion()
        {
            try
            {
                int count = 0;
                var RegionModel = _mapper.Map<List<RegionViewModel>>(_unitOfWork.RegionRepository.GetAllAsQueryable());
                return new Response<List<RegionViewModel>>(true, RegionModel, null, null, (int)Helpers.Constants.ApiReturnCode.success, count);
            }
            catch (Exception err)
            {

                return new Response<List<RegionViewModel>>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
        }
        public Response<List<AreaViewModel>> GetAllArea()
        {
            try
            {
                int count = 0;
                var AreaModel = _mapper.Map<List<AreaViewModel>>(_unitOfWork.AreaRepository.GetAllAsQueryable());
                return new Response<List<AreaViewModel>>(true, AreaModel, null, null, (int)Helpers.Constants.ApiReturnCode.success, count);
            }
            catch (Exception err)
            {

                return new Response<List<AreaViewModel>>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
        }
        public Response<SiteViewModel> DisplaySiteDetailsBySiteCode(string SiteCode)
        {
            try
            {
                var Site = _unitOfWork.SiteRepository.GetIncludeWhereFirst(x => x.SiteCode == SiteCode, s => s.Area, a => a.Region, d => d.siteStatus);
                var site = _mapper.Map<SiteViewModel>(Site);
                return new Response<SiteViewModel>(site);
            }
            catch (Exception err)
            {

                return new Response<SiteViewModel>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.success);
            }
        }
        public Response<List<GetAllsiteOnMultiRegion>> GetAllsiteonMultiRegion(List<RegionForSiteViewModel> Region)
        {
            try
            {
                List<GetAllsiteOnMultiRegion> finelres = new List<GetAllsiteOnMultiRegion>();


                foreach (var item in Region)
                {
                    GetAllsiteOnMultiRegion RegionRes = new GetAllsiteOnMultiRegion();
                    var res = _unitOfWork.RegionRepository.GetWhereFirst(x => x.RegionName == item.RegionName);
                    if (res != null)
                    {
                        var result = _mapper.Map<List<SiteForGetViewModel>>(_unitOfWork.SiteRepository.GetWhere(x => x.RegionCode == res.RegionCode)).ToList();
                        RegionRes.siteForGetViewModels = result;

                    }
                    RegionRes.RegionCode = res.RegionCode;
                    RegionRes.RegionName = res.RegionName;
                    finelres.Add(RegionRes);
                }
                return new Response<List<GetAllsiteOnMultiRegion>>(true, finelres, null, null, (int)Helpers.Constants.ApiReturnCode.success);
            }
            catch (Exception err)
            {

                return new Response<List<GetAllsiteOnMultiRegion>>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);

            }
        }
        public Response<List<GetAllsiteOnMultiAreaViewModel>> GetAllsiteonMultiArea(List<AreaForSiteViewModel> Area)
        {
            try
            {
                List<GetAllsiteOnMultiAreaViewModel> finelres = new List<GetAllsiteOnMultiAreaViewModel>();


                foreach (var item in Area)
                {
                    GetAllsiteOnMultiAreaViewModel AreaRes = new GetAllsiteOnMultiAreaViewModel();
                    var res = _unitOfWork.AreaRepository.GetWhereFirst(x => x.AreaName == item.AreaName);
                    if (res != null)
                    {
                        var result = _mapper.Map<List<SiteForGetViewModel>>(_unitOfWork.SiteRepository.GetWhere(x => x.AreaId == res.Id)).ToList();
                        AreaRes.siteForGetViewModels = result;

                    }
                    AreaRes.Id = res.Id;
                    AreaRes.AreaName = res.AreaName;
                    finelres.Add(AreaRes);
                }
                return new Response<List<GetAllsiteOnMultiAreaViewModel>>(true, finelres, null, null, (int)Helpers.Constants.ApiReturnCode.success);
            }
            catch (Exception err)
            {

                return new Response<List<GetAllsiteOnMultiAreaViewModel>>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);

            }
        }
        public async Task<List<GetSiteNameBySitCode>> GetSiteNameBySitCode(List<SiteCodeForW_F> SiteCode)
        {
            List<GetSiteNameBySitCode> Sites = new List<GetSiteNameBySitCode>();
            foreach (var item in SiteCode)
            {

                var res = _unitOfWork.SiteRepository.GetWhereFirst(x => x.SiteCode == item.SiteCode);
                var result = _mapper.Map<GetSiteNameBySitCode>(res);
                Sites.Add(result);
            }
            return Sites;
        }
        public Response<UsedSitesViewModel> GetUsedSitesCount()
        {
            if (_MySites == null)
                _MySites = _context.TLIsite
                    .AsNoTracking().Include(x => x.Area).Include(x => x.Region)
                    .Include(x => x.siteStatus).ToList();

            List<string> UsedSitesInLoads = _context.TLIcivilLoads.AsNoTracking().Select(x => x.SiteCode.ToLower()).Distinct().ToList();
            List<string> UsedSitesInCivils = _context.TLIcivilSiteDate.AsNoTracking().Select(x => x.SiteCode.ToLower()).Distinct().ToList();
            List<string> UsedSitesInOtherInventories = _context.TLIotherInSite.AsNoTracking().Select(x => x.SiteCode.ToLower()).Distinct().ToList();

            List<string> all = UsedSitesInLoads.Concat(UsedSitesInCivils).Concat(UsedSitesInOtherInventories).Distinct().ToList();

            int UsedSitesCount = _MySites.Select(x => x.SiteCode.ToLower()).Where(all.Contains).ToList().Count();
            int AllSitesCount = _MySites.Count();
            int UnUsedSitesCount = AllSitesCount - UsedSitesCount;

            UsedSitesViewModel OutPut = new UsedSitesViewModel()
            {
                UsedSitesCount = UsedSitesCount,
                UnUsedSitesCount = UnUsedSitesCount,
                AllSitesCount = AllSitesCount
            };

            return new Response<UsedSitesViewModel>(true, OutPut, null, null, (int)Helpers.Constants.ApiReturnCode.success);
        }
        public Response<ItemsOnSite> GetItemsOnSite(string SiteCode)
        {
            ItemsOnSite OutPut = new ItemsOnSite();

            List<TLIcivilLoads> UsedSitesInLoads = _unitOfWork.CivilLoadsRepository.GetWhereAndInclude(x => x.SiteCode.ToLower() == SiteCode.ToLower() && !x.Dismantle
            && x.allLoadInstId != null, x => x.allLoadInst, x => x.sideArm).ToList();

            OutPut.PowerCount = UsedSitesInLoads.Where(x => x.allLoadInst.Draft == false && x.allLoadInst.powerId != null).Count();
            OutPut.MW_RFUCount = UsedSitesInLoads.Where(x => x.allLoadInst.Draft == false && x.allLoadInst.mwRFUId != null).Count();
            OutPut.MW_BUCount = UsedSitesInLoads.Where(x => x.allLoadInst.Draft == false && x.allLoadInst.mwBUId != null).Count();
            OutPut.MW_DishCount = UsedSitesInLoads.Where(x => x.allLoadInst.Draft == false && x.allLoadInst.mwDishId != null).Count();
            OutPut.MW_ODUCount = UsedSitesInLoads.Where(x => x.allLoadInst.Draft == false && x.allLoadInst.mwODUId != null).Count();
            OutPut.MW_OtherCount = UsedSitesInLoads.Where(x => x.allLoadInst.Draft == false && x.allLoadInst.mwOtherId != null).Count();
            OutPut.RadioAntennaCount = UsedSitesInLoads.Where(x => x.allLoadInst.Draft == false && x.allLoadInst.radioAntennaId != null).Count();
            OutPut.RadioRRUCount = UsedSitesInLoads.Where(x => x.allLoadInst.Draft == false && x.allLoadInst.radioRRUId != null).Count();
            OutPut.RadioOtherCount = UsedSitesInLoads.Where(x => x.allLoadInst.Draft == false && x.allLoadInst.radioOtherId != null).Count();
            OutPut.LoadOtherCount = UsedSitesInLoads.Where(x => x.allLoadInst.Draft == false && x.allLoadInst.loadOtherId != null).Count();
            OutPut.SideArmCount = UsedSitesInLoads.Where(x => x.sideArmId != null && x.sideArm.Draft == false).Count();

            List<TLIallCivilInst> UsedSitesInCivils = _unitOfWork.CivilSiteDateRepository.GetWhereAndInclude(x => x.SiteCode.ToLower() == SiteCode.ToLower()
            && !x.Dismantle && x.allCivilInst.Draft == false, x => x.allCivilInst, x => x.allCivilInst.civilWithLegs, x => x.allCivilInst.civilWithoutLeg,
            x => x.allCivilInst.civilNonSteel, x => x.allCivilInst.civilWithoutLeg.CivilWithoutlegsLib, x => x.allCivilInst.civilWithoutLeg.CivilWithoutlegsLib.CivilWithoutLegCategory).Select(x => x.allCivilInst).ToList();

            int xxx = UsedSitesInCivils.Count();

            OutPut.SteelWithLegsCount = UsedSitesInCivils.Where(x => x.civilWithLegsId != null).Count();
            OutPut.SteelWithoutLegs_MastCount = UsedSitesInCivils.Where(x => x.civilWithoutLegId != null ?
               (x.civilWithoutLeg.CivilWithoutlegsLib.CivilWithoutLegCategoryId != null ?
                x.civilWithoutLeg.CivilWithoutlegsLib.CivilWithoutLegCategory.Name.ToLower() == Helpers.Constants.CivilWithoutLegCategories.Mast.ToString().ToLower() : false) : false).Count();
            OutPut.SteelWithoutLegs_MonopoleCount = UsedSitesInCivils.Where(x => x.civilWithoutLegId != null ?
               (x.civilWithoutLeg.CivilWithoutlegsLib.CivilWithoutLegCategoryId != null ?
                x.civilWithoutLeg.CivilWithoutlegsLib.CivilWithoutLegCategory.Name.ToLower() == Helpers.Constants.CivilWithoutLegCategories.Monopole.ToString().ToLower() : false) : false).Count();
            OutPut.SteelWithoutLegs_CapsuleCount = UsedSitesInCivils.Where(x => x.civilWithoutLegId != null ?
               (x.civilWithoutLeg.CivilWithoutlegsLib.CivilWithoutLegCategoryId != null ?
                x.civilWithoutLeg.CivilWithoutlegsLib.CivilWithoutLegCategory.Name.ToLower() == Helpers.Constants.CivilWithoutLegCategories.Capsule.ToString().ToLower() : false) : false).Count();
            OutPut.NonSteelCount = UsedSitesInCivils.Where(x => x.civilNonSteelId != null).Count();

            List<TLIallOtherInventoryInst> UsedSitesInOtherInventories = _unitOfWork.OtherInSiteRepository.GetWhereAndInclude(x => x.SiteCode.ToLower() == SiteCode.ToLower()
            && !x.Dismantle && x.allOtherInventoryInst.Draft == false, x => x.allOtherInventoryInst, x => x.allOtherInventoryInst.cabinet
            , x => x.allOtherInventoryInst.solar, x => x.allOtherInventoryInst.generator).Select(x => x.allOtherInventoryInst).ToList();



            OutPut.CabinetPowerCount = UsedSitesInOtherInventories.Where(x => x.cabinetId != null ?
                x.cabinet.CabinetPowerLibraryId != null : false).Count();
            OutPut.CabinetTelecomCount = UsedSitesInOtherInventories.Where(x => x.cabinetId != null ?
                x.cabinet.CabinetTelecomLibraryId != null : false).Count();
            OutPut.SolarCount = UsedSitesInOtherInventories.Where(x => x.solarId != null).Count();
            OutPut.GeneratorCount = UsedSitesInOtherInventories.Where(x => x.generatorId != null).Count();

            return new Response<ItemsOnSite>(true, OutPut, null, null, (int)Helpers.Constants.ApiReturnCode.success);
        }
        private readonly string _connectionString;
        public List<dynamic> ExecuteStoredProcedureAndQueryDynamicView(string storedProcedureName, string dynamicViewName, string ConnectionString)
        {
            using (var connection = new OracleConnection(ConnectionString))
            {
                connection.Open();
                // Execute Stored Procedure
                using (OracleCommand procedureCommand = new OracleCommand(storedProcedureName, connection))
                {
                    procedureCommand.CommandType = CommandType.StoredProcedure;
                    procedureCommand.ExecuteNonQuery();
                }

                // Query Dynamic View
                string sqlQuery = $"SELECT * FROM {dynamicViewName}";
                using (OracleCommand queryCommand = new OracleCommand(sqlQuery, connection))
                {
                    using (OracleDataReader reader = queryCommand.ExecuteReader())
                    {
                        List<dynamic> result = new List<dynamic>();

                        while (reader.Read())
                        {
                            dynamic dynamicResult = new ExpandoObject();
                            var properties = (IDictionary<string, object>)dynamicResult;

                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                properties[reader.GetName(i)] = reader[i];
                            }

                            result.Add(dynamicResult);
                        }

                        return result;
                    }
                }
            }
        }

        private static readonly HttpClient _httpClient = new HttpClient();

        public async Task<SumbitTaskByTLI> SumbitTaskByTLI(int TaskId)
        {
            string apiUrl = "http://192.168.1.50:9085/api/TicketManagement/SumbitTaskByTLI";

            int maxRetries = 10; // Number of retries
            int retryDelayMilliseconds = 600000; // 10 minutes in milliseconds

            for (int retryCount = 0; retryCount < maxRetries; retryCount++)
            {
                try
                {
                    var jsonPayload = new { Id = TaskId };
                    var content = new StringContent(System.Text.Json.JsonSerializer.Serialize(jsonPayload), Encoding.UTF8, "application/json");

                    using (HttpResponseMessage response = await _httpClient.PostAsync(apiUrl, content))
                    {
                        if (response.IsSuccessStatusCode)
                        {
                            string responseBody = await response.Content.ReadAsStringAsync();

                            if (!string.IsNullOrEmpty(responseBody))
                            {
                                var rootObject = System.Text.Json.JsonSerializer.Deserialize<SumbitTaskByTLI>(responseBody, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                                var res = _mapper.Map<SumbitTaskByTLI>(rootObject);
                                return res;
                            }
                        }
                        else
                        {
                            Console.WriteLine($"API request failed with status code: {response.StatusCode}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred: {ex.Message}");
                    // Log the exception details if necessary
                }

                // Wait for the specified delay before retrying
                await Task.Delay(retryDelayMilliseconds);
            }

            return null;
        }

    }
  
    
}



