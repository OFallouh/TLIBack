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
using Swashbuckle.AspNetCore.SwaggerUI;
using static TLIS_Repository.Repositories.SiteRepository;
using TLIS_DAL.ViewModels.CivilLoadsDTOs;
using TLIS_DAL.ViewModels.CivilWithLegLibraryDTOs;
using OfficeOpenXml;
using ClosedXML.Excel;
using static TLIS_Service.Helpers.Constants;
using MimeKit.IO.Filters;
using static TLIS_Service.Services.SiteService;
using Newtonsoft.Json;
using System.Globalization;
using DocumentFormat.OpenXml.Drawing.Charts;
using Oracle.ManagedDataAccess.Types;



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
        IServiceProvider Services;
        private readonly IConfiguration _configuration;
        public SiteService(IUnitOfWork unitOfWork, IServiceCollection services, ApplicationDbContext context, IMapper mapper, IServiceProvider serviceو, IConfiguration configuration)
        {
            _context = context;
            _unitOfWork = unitOfWork;
            _services = services;
            _mapper = mapper;
            _configuration = configuration;
        }
        public Response<AddSiteViewModel> AddSite(AddSiteViewModel AddSiteViewModel, int? TaskId, int UserId)
        {
            using (TransactionScope transaction = new TransactionScope())
            {
                try
                {
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
                    _unitOfWork.SiteRepository.AddWithHSite(UserId, null, NewSiteEntity);
                    if (TaskId != null)
                    {
                        var Submit = _unitOfWork.SiteRepository.SubmitTaskByTLI(TaskId);
                        var result = Submit.Result;
                        if (result.result == true && result.errorMessage == null)
                        {
                            var AreaName = _context.TLIarea.FirstOrDefault(x => x.Id == NewSiteEntity.AreaId)?.AreaName;
                            EditTicketInfoBinding editTicketInfoBinding = new EditTicketInfoBinding()
                            {
                                TaskId = TaskId,
                                SiteCode = NewSiteEntity.SiteCode,
                                RegionName = NewSiteEntity.RegionCode,
                                CityName = NewSiteEntity.Zone,
                                AreaName = AreaName

                            };
                            var SubmitTicketInfo = _unitOfWork.SiteRepository.EditTicketInfoByTLI(editTicketInfoBinding);
                            if (SubmitTicketInfo.Result.result == true && SubmitTicketInfo.Result.errorMessage == null)
                            {
                                _unitOfWork.SaveChanges();
                                transaction.Complete();
                            }
                        }
                        else
                        {
                            transaction.Dispose();
                            return new Response<AddSiteViewModel>(false, null, null, result.errorMessage.ToString(), (int)Helpers.Constants.ApiReturnCode.fail);
                        }
                    }
                    else
                    {
                        _unitOfWork.SaveChanges();
                        transaction.Complete();
                    }

                    return new Response<AddSiteViewModel>(true, null, null, null, (int)Helpers.Constants.ApiReturnCode.success);
                }
                catch (Exception err)
                {
                    return new Response<AddSiteViewModel>(false, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
                }
            }
        }
        public Response<EditSiteViewModel> EditSite(EditSiteViewModel EditSiteViewModel, int? TaskId, int UserId)
        {
            using (TransactionScope transaction = new TransactionScope())
            {
                try
                {
                    var CheckSiteName = _context.TLIsite.FirstOrDefault(x => x.SiteName == EditSiteViewModel.SiteName
                    && x.SiteCode != EditSiteViewModel.SiteCode);

                    if (CheckSiteName != null)
                    {
                        return new Response<EditSiteViewModel>(true, null, null, $"This site name {EditSiteViewModel.SiteName} is already exist",
                            (int)Helpers.Constants.ApiReturnCode.fail);
                    }
                    var OldSiteInfo = _unitOfWork.SiteRepository.GetAllAsQueryable().AsNoTracking().FirstOrDefault
                        (x => x.SiteCode == EditSiteViewModel.SiteCode);
                    TLIsite Site = _mapper.Map<TLIsite>(EditSiteViewModel);
                    if(Site.RentedSpace < Site.ReservedSpace)
                        return new Response<EditSiteViewModel>(true, null, null, $"can not to be resered space bigger than rented space",
                            (int)Helpers.Constants.ApiReturnCode.fail);

                    _unitOfWork.SiteRepository.UpdateWithHInstallationSite(UserId, null, OldSiteInfo, Site, EditSiteViewModel.SiteCode);

                    _MySites.Remove(_MySites.FirstOrDefault(x => x.SiteCode.ToLower() == EditSiteViewModel.SiteCode.ToLower()));
                    _MySites.Add(Site);
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
                            return new Response<EditSiteViewModel>(true, null, null, result.errorMessage.ToString(), (int)Helpers.Constants.ApiReturnCode.fail);
                        }
                    }
                    else
                    {
                        _unitOfWork.SaveChanges();
                        transaction.Complete();
                    }
                    return new Response<EditSiteViewModel>(true, null, null, null, (int)Helpers.Constants.ApiReturnCode.success);
                }
                catch (Exception err)
                {
                    return new Response<EditSiteViewModel>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
                }
            }
        }
        public Response<SiteDetailsObject> EditSiteDetalis(SiteDetailsObject siteDetailsObject, int? TaskId, int UserId)
        {
            using (TransactionScope transaction = new TransactionScope())
            {
                try
                {
                    var OldSiteSelected = _context.TLIsite.AsNoTracking().FirstOrDefault(x => x.SiteCode.ToLower() == siteDetailsObject.SiteCode.ToLower());
                    var SiteSelected = _context.TLIsite.FirstOrDefault(x => x.SiteCode.ToLower() == siteDetailsObject.SiteCode.ToLower());
                    if (SiteSelected != null)
                    {
                        if (siteDetailsObject.PlanType != null)
                        {
                            var planTypeValues = siteDetailsObject.PlanType.Select(pt => ((int)pt).ToString());
                            SiteSelected.PlanType = string.Join(",", planTypeValues);


                            foreach (var item in siteDetailsObject.PlanType)
                            {
                                if (item == Enums.PlanType.CollectData)
                                {
                                    SiteSelected.PlanStatusCollectData = (int?)siteDetailsObject.CollectData.PlanStatus;
                                    SiteSelected.pendingTypeCollectData = (int?)siteDetailsObject.CollectData.PendingType;
                                    SiteSelected.MWValidationRemarkCollectData = siteDetailsObject.CollectData.MwValidationRemark;
                                    SiteSelected.MWValidationStatusCollectDate = (int?)siteDetailsObject.CollectData.MwValidationStatus;
                                    SiteSelected.RadioVStatusCollectData = (int?)siteDetailsObject.CollectData.RadioValidationStatus;
                                    SiteSelected.RadioVRemarkCollectData = siteDetailsObject.CollectData.RadioValidationRemark;
                                    SiteSelected.PowerVStatusCollectData = (int?)siteDetailsObject.CollectData.PowerValidationStatus;
                                    SiteSelected.PowerVRemarkCollectData = siteDetailsObject.CollectData.PowerValidationRemark;
                                }
                                else if (item == Enums.PlanType.MWMD)
                                {
                                    SiteSelected.MdTypeMWMd = (int?)siteDetailsObject.MWMd.MdType;
                                    SiteSelected.DescriptionMWMd = siteDetailsObject.MWMd.Description;
                                    SiteSelected.PlanStatusMWMd = (int?)siteDetailsObject.MWMd.PlanStatus;
                                    SiteSelected.pendingTypeMWMd = (int?)siteDetailsObject.MWMd.PendingType;
                                    SiteSelected.MWValidationStatusMWMd = (int?)siteDetailsObject.MWMd.MwValidationStatus;
                                    SiteSelected.MWValidationRemarkMWMd = siteDetailsObject.MWMd.MwValidationRemark;
                                }
                                else if (item == Enums.PlanType.RadioMD)
                                {
                                    SiteSelected.MdTypeRadioMd = (int?)siteDetailsObject.RadioMd.MdType;
                                    SiteSelected.DescriptionRadioMd = siteDetailsObject.RadioMd.Description;
                                    SiteSelected.PlanStatusRadioMd = (int?)siteDetailsObject.RadioMd.PlanStatus;
                                    SiteSelected.pendingTypeRadioMd = (int?)siteDetailsObject.RadioMd.PendingType;
                                    SiteSelected.RadioVStatusRadioMd = (int?)siteDetailsObject.RadioMd.RadioValidationStatus;
                                    SiteSelected.RadioVRemarkRadioMd = siteDetailsObject.RadioMd.RadioValidationRemark;
                                }
                                else if (item == Enums.PlanType.PowerMD)
                                {
                                    SiteSelected.MdTypePowerMd = (int?)siteDetailsObject.PowerMd.MdType;
                                    SiteSelected.DescriptionPowerMd = siteDetailsObject.PowerMd.Description;
                                    SiteSelected.PlanStatusPowerMd = (int?)siteDetailsObject.PowerMd.PlanStatus;
                                    SiteSelected.pendingTypePowerMd = (int?)siteDetailsObject.PowerMd.PendingType;
                                    SiteSelected.PowerVStatusPowerMd = (int?)siteDetailsObject.PowerMd.PowerValidationStatus;
                                    SiteSelected.PowerVRemarkPowerMd = siteDetailsObject.PowerMd.PowerValidationRemark;
                                }
                                _unitOfWork.SiteRepository.UpdateWithHInstallationSite(UserId, null, OldSiteSelected, SiteSelected, siteDetailsObject.SiteCode);
                                _unitOfWork.SaveChanges();

                            }
                        }
                        if (siteDetailsObject.PlanType == null)
                        {
                            SiteSelected.PlanStatusCollectData = (int?)siteDetailsObject.CollectData.PlanStatus;
                            SiteSelected.PlanType = null;
                            SiteSelected.pendingTypeCollectData = (int?)siteDetailsObject.CollectData.PendingType;
                            SiteSelected.MWValidationRemarkCollectData = siteDetailsObject.CollectData.MwValidationRemark;
                            SiteSelected.MWValidationStatusCollectDate = (int?)siteDetailsObject.CollectData.MwValidationStatus;
                            SiteSelected.RadioVStatusCollectData = (int?)siteDetailsObject.CollectData.RadioValidationStatus;
                            SiteSelected.RadioVRemarkCollectData = siteDetailsObject.CollectData.RadioValidationRemark;
                            SiteSelected.PowerVStatusCollectData = (int?)siteDetailsObject.CollectData.PowerValidationStatus;
                            SiteSelected.PowerVRemarkCollectData = siteDetailsObject.CollectData.PowerValidationRemark;
                            SiteSelected.MdTypeMWMd = (int?)siteDetailsObject.MWMd.MdType;
                            SiteSelected.DescriptionMWMd = siteDetailsObject.MWMd.Description;
                            SiteSelected.PlanStatusMWMd = (int?)siteDetailsObject.MWMd.PlanStatus;
                            SiteSelected.pendingTypeMWMd = (int?)siteDetailsObject.MWMd.PendingType;
                            SiteSelected.MWValidationStatusMWMd = (int?)siteDetailsObject.MWMd.MwValidationStatus;
                            SiteSelected.MWValidationRemarkMWMd = siteDetailsObject.MWMd.MwValidationRemark;
                            SiteSelected.MdTypeRadioMd = (int?)siteDetailsObject.RadioMd.MdType;
                            SiteSelected.DescriptionRadioMd = siteDetailsObject.RadioMd.Description;
                            SiteSelected.PlanStatusRadioMd = (int?)siteDetailsObject.RadioMd.PlanStatus;
                            SiteSelected.pendingTypeRadioMd = (int?)siteDetailsObject.RadioMd.PendingType;
                            SiteSelected.RadioVStatusRadioMd = (int?)siteDetailsObject.RadioMd.RadioValidationStatus;
                            SiteSelected.RadioVRemarkRadioMd = siteDetailsObject.RadioMd.RadioValidationRemark;
                            SiteSelected.MdTypePowerMd = (int?)siteDetailsObject.PowerMd.MdType;
                            SiteSelected.DescriptionPowerMd = siteDetailsObject.PowerMd.Description;
                            SiteSelected.PlanStatusPowerMd = (int?)siteDetailsObject.PowerMd.PlanStatus;
                            SiteSelected.pendingTypePowerMd = (int?)siteDetailsObject.PowerMd.PendingType;
                            SiteSelected.PowerVStatusPowerMd = (int?)siteDetailsObject.PowerMd.PowerValidationStatus;
                            SiteSelected.PowerVRemarkPowerMd = siteDetailsObject.PowerMd.PowerValidationRemark;
                            _unitOfWork.SiteRepository.UpdateWithHInstallationSite(UserId, null, OldSiteSelected, SiteSelected, siteDetailsObject.SiteCode);
                            _unitOfWork.SaveChanges();
                        }
                      
                     
                    }
                    else
                    {
                        return new Response<SiteDetailsObject>(true, null, null, "This Site Is Not Found", (int)Helpers.Constants.ApiReturnCode.fail);
                    }
                    transaction.Complete();
                    return new Response<SiteDetailsObject>(true, null, null, null, (int)Helpers.Constants.ApiReturnCode.success);
                }
                catch (Exception err)
                {
                    return new Response<SiteDetailsObject>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
                }
            }
        }
        public Response<SiteDetailsObject> GetSiteDetails(string siteCode)
        {
            try
            {
                var siteSelected = _context.TLIsite.FirstOrDefault(x => x.SiteCode.ToLower() == siteCode.ToLower());

                if (siteSelected == null)
                    return new Response<SiteDetailsObject>(true, null, null, "This Site Is Not Found", (int)Helpers.Constants.ApiReturnCode.fail);

                var siteDetailsObject = new SiteDetailsObject
                {
                    SiteCode = siteSelected.SiteCode,
                    PlanType = siteSelected.PlanType?.Split(',').Select(pt => (Enums.PlanType)Enum.Parse(typeof(Enums.PlanType), pt)).ToList() ?? new List<Enums.PlanType>(),
                    CollectData = siteSelected.PlanStatusCollectData.HasValue || siteSelected.pendingTypeCollectData.HasValue ||
                                  siteSelected.MWValidationStatusCollectDate.HasValue || !string.IsNullOrEmpty(siteSelected.MWValidationRemarkCollectData) ||
                                  siteSelected.RadioVStatusCollectData.HasValue || !string.IsNullOrEmpty(siteSelected.RadioVRemarkCollectData) ||
                                  siteSelected.PowerVStatusCollectData.HasValue || !string.IsNullOrEmpty(siteSelected.PowerVRemarkCollectData)
                                  ? new CollectData
                                  {
                                      PlanStatus = siteSelected.PlanStatusCollectData.HasValue ? (Enums.CollectionDataPlanStatus?)siteSelected.PlanStatusCollectData : null,
                                      PendingType = siteSelected.pendingTypeCollectData.HasValue ? (Enums.CollectionDataPendingType?)siteSelected.pendingTypeCollectData : null,
                                      MwValidationStatus = siteSelected.MWValidationStatusCollectDate.HasValue ? (Enums.ValidationStatus?)siteSelected.MWValidationStatusCollectDate : null,
                                      MwValidationRemark = siteSelected.MWValidationRemarkCollectData,
                                      RadioValidationStatus = siteSelected.RadioVStatusCollectData.HasValue ? (Enums.RadioValidationStatus?)siteSelected.RadioVStatusCollectData : null,
                                      RadioValidationRemark = siteSelected.RadioVRemarkCollectData,
                                      PowerValidationStatus = siteSelected.PowerVStatusCollectData.HasValue ? (Enums.ValidationStatus?)siteSelected.PowerVStatusCollectData : null,
                                      PowerValidationRemark = siteSelected.PowerVRemarkCollectData
                                  } : null,
                    MWMd = siteSelected.MdTypeMWMd.HasValue || !string.IsNullOrEmpty(siteSelected.DescriptionMWMd) ||
                           siteSelected.PlanStatusMWMd.HasValue || siteSelected.pendingTypeMWMd.HasValue ||
                           siteSelected.MWValidationStatusMWMd.HasValue || !string.IsNullOrEmpty(siteSelected.MWValidationRemarkMWMd)
                           ? new MWMd
                           {
                               MdType = siteSelected.MdTypeMWMd.HasValue ? (Enums.MDType?)siteSelected.MdTypeMWMd : null,
                               Description = siteSelected.DescriptionMWMd,
                               PlanStatus = siteSelected.PlanStatusMWMd.HasValue ? (Enums.MWMDPlanStatus?)siteSelected.PlanStatusMWMd : null,
                               PendingType = siteSelected.pendingTypeMWMd.HasValue ? (Enums.MWMDPendingType?)siteSelected.pendingTypeMWMd : null,
                               MwValidationStatus = siteSelected.MWValidationStatusMWMd.HasValue ? (Enums.MWValidationStatus?)siteSelected.MWValidationStatusMWMd : null,
                               MwValidationRemark = siteSelected.MWValidationRemarkMWMd
                           } : null,
                    RadioMd = siteSelected.MdTypeRadioMd.HasValue || !string.IsNullOrEmpty(siteSelected.DescriptionRadioMd) ||
                              siteSelected.PlanStatusRadioMd.HasValue || siteSelected.pendingTypeRadioMd.HasValue ||
                              siteSelected.RadioVStatusRadioMd.HasValue || !string.IsNullOrEmpty(siteSelected.RadioVRemarkRadioMd)
                              ? new RadioMd
                              {
                                  MdType = siteSelected.MdTypeRadioMd.HasValue ? (Enums.MDType?)siteSelected.MdTypeRadioMd : null,
                                  Description = siteSelected.DescriptionRadioMd,
                                  PlanStatus = siteSelected.PlanStatusRadioMd.HasValue ? (Enums.OtherMDPlanStatus?)siteSelected.PlanStatusRadioMd : null,
                                  PendingType = siteSelected.pendingTypeRadioMd.HasValue ? (Enums.OtherMDPendingType?)siteSelected.pendingTypeRadioMd : null,
                                  RadioValidationStatus = siteSelected.RadioVStatusRadioMd.HasValue ? (Enums.OtherValidationStatus?)siteSelected.RadioVStatusRadioMd : null,
                                  RadioValidationRemark = siteSelected.RadioVRemarkRadioMd
                              } : null,
                    PowerMd = siteSelected.MdTypePowerMd.HasValue || !string.IsNullOrEmpty(siteSelected.DescriptionPowerMd) ||
                              siteSelected.PlanStatusPowerMd.HasValue || siteSelected.pendingTypePowerMd.HasValue ||
                              siteSelected.PowerVStatusPowerMd.HasValue || !string.IsNullOrEmpty(siteSelected.PowerVRemarkPowerMd)
                              ? new PowerMd
                              {
                                  MdType = siteSelected.MdTypePowerMd.HasValue ? (Enums.MDType?)siteSelected.MdTypePowerMd : null,
                                  Description = siteSelected.DescriptionPowerMd,
                                  PlanStatus = siteSelected.PlanStatusPowerMd.HasValue ? (Enums.OtherMDPlanStatus?)siteSelected.PlanStatusPowerMd : null,
                                  PendingType = siteSelected.pendingTypePowerMd.HasValue ? (Enums.OtherMDPendingType?)siteSelected.pendingTypePowerMd : null,
                                  PowerValidationStatus = siteSelected.PowerVStatusPowerMd.HasValue ? (Enums.OtherValidationStatus?)siteSelected.PowerVStatusPowerMd : null,
                                  PowerValidationRemark = siteSelected.PowerVRemarkPowerMd
                              } : null
                };

                if (siteDetailsObject == null ||
                    (siteDetailsObject.PlanType?.Count == 0 &&
                     siteDetailsObject.CollectData == null &&
                     siteDetailsObject.MWMd == null &&
                     siteDetailsObject.RadioMd == null &&
                     siteDetailsObject.PowerMd == null))
                {
                    return new Response<SiteDetailsObject>(true, null, null, "No relevant data found", (int)Helpers.Constants.ApiReturnCode.success);
                }

                return new Response<SiteDetailsObject>(true, siteDetailsObject, null, null, (int)Helpers.Constants.ApiReturnCode.success);
            }
            catch (Exception err)
            {
                return new Response<SiteDetailsObject>(false, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
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
        public Response<IEnumerable<SiteViewModelForGetAll>> GetSites(int? UserId, string UserName, bool? isRefresh, bool? GetItemsCountOnEachSite,bool ExternalSys, FilterRequest filterRequest = null)
        {
            string[] ErrorMessagesWhenReturning = null;

        StartAgainWithRefresh:
            try
            {
                if (UserId == null)
                {
                    UserId = _context.TLIexternalSys.FirstOrDefault(x => x.UserName.ToLower() == UserName.ToLower()).Id;
                }

                List<TLIlocationType> Locations = _context.TLIlocationType.AsNoTracking().ToList();

                List<string> UsedSitesInLoads = _context.TLIcivilLoads.AsNoTracking()
                    .Where(x => !string.IsNullOrEmpty(x.SiteCode) && !x.Dismantle)
                    .Select(x => x.SiteCode.ToLower()).Distinct().ToList();

                List<string> UsedSitesInCivils = _context.TLIcivilSiteDate.AsNoTracking()
                    .Where(x => !string.IsNullOrEmpty(x.SiteCode) && !x.Dismantle)
                    .Select(x => x.SiteCode.ToLower()).Distinct().ToList();

                UsedSitesInCivils.AddRange(UsedSitesInLoads);

                List<string> UsedSitesInOtherInventories = _context.TLIotherInSite.AsNoTracking()
                    .Where(x => !string.IsNullOrEmpty(x.SiteCode) && !x.Dismantle)
                    .Select(x => x.SiteCode.ToLower()).Distinct().ToList();

                UsedSitesInOtherInventories.AddRange(UsedSitesInCivils);

                List<string> AllUsedSites = UsedSitesInOtherInventories.Distinct().ToList();

                IEnumerable<SiteViewModelForGetAll> SitesViewModels;

                if (isRefresh == true)
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
                foreach (var itemSitesViewModels in SitesViewModels)
                {
                    string? LocationTypeInModel = _MySites.FirstOrDefault(x => x.SiteCode.ToLower() == itemSitesViewModels.SiteCode.ToLower())
                        .LocationType;

                    if (!string.IsNullOrEmpty(LocationTypeInModel))
                    {
                        TLIlocationType? CheckLocation = Locations.FirstOrDefault(x => x.Id.ToString() == LocationTypeInModel);


                        itemSitesViewModels.SiteCode = itemSitesViewModels.SiteCode;
                        itemSitesViewModels.LocationType = CheckLocation != null ? CheckLocation.Name : null;
                        itemSitesViewModels.SiteName = itemSitesViewModels.SiteName;
                        itemSitesViewModels.Area = itemSitesViewModels.Area;
                        itemSitesViewModels.CityName = itemSitesViewModels.CityName;
                        itemSitesViewModels.Latitude = itemSitesViewModels.Latitude;
                        itemSitesViewModels.LocationHieght = itemSitesViewModels.LocationHieght;
                        itemSitesViewModels.Longitude = itemSitesViewModels.Longitude;
                        itemSitesViewModels.Region = itemSitesViewModels.Region;
                        itemSitesViewModels.RentedSpace = itemSitesViewModels.RentedSpace;
                        itemSitesViewModels.ReservedSpace = itemSitesViewModels.ReservedSpace;
                        itemSitesViewModels.Status = itemSitesViewModels.Status;
                        itemSitesViewModels.SiteVisiteDate = itemSitesViewModels.SiteVisiteDate;
                        itemSitesViewModels.isUsed = AllUsedSites.Any(x => x.ToLower() == itemSitesViewModels.SiteCode.ToLower());
                    }
                    else
                    {
                        itemSitesViewModels.SiteCode = itemSitesViewModels.SiteCode;
                        itemSitesViewModels.LocationType = null;
                        itemSitesViewModels.SiteName = itemSitesViewModels.SiteName;
                        itemSitesViewModels.Area = itemSitesViewModels.Area;
                        itemSitesViewModels.CityName = itemSitesViewModels.CityName;
                        itemSitesViewModels.Latitude = itemSitesViewModels.Latitude;
                        itemSitesViewModels.LocationHieght = itemSitesViewModels.LocationHieght;
                        itemSitesViewModels.Longitude = itemSitesViewModels.Longitude;
                        itemSitesViewModels.Region = itemSitesViewModels.Region;
                        itemSitesViewModels.RentedSpace = itemSitesViewModels.RentedSpace;
                        itemSitesViewModels.ReservedSpace = itemSitesViewModels.ReservedSpace;
                        itemSitesViewModels.Status = itemSitesViewModels.Status;
                        itemSitesViewModels.SiteVisiteDate = itemSitesViewModels.SiteVisiteDate;
                        itemSitesViewModels.isUsed = AllUsedSites.Any(x => x.ToLower() == itemSitesViewModels.SiteCode.ToLower());
                    }
                }

                if (filterRequest != null && filterRequest.Filters != null && filterRequest.Filters.Count > 0)
                {
                    foreach (var filter in filterRequest.Filters)
                    {
                        string field = filter.Key;

                        // Serialize filter.Value (Filter object) to JSON string
                        string filterJsonString = System.Text.Json.JsonSerializer.Serialize(filter.Value);

                        // Parse the JSON string to JsonElement
                        JsonElement filterValue = System.Text.Json. JsonSerializer.Deserialize<JsonElement>(filterJsonString);

                        // Check if Value is null or empty and skip if it is
                        if (filterValue.TryGetProperty("Value", out JsonElement valueElement))
                        {
                            // تحقق من إذا كانت القيمة Null أو فارغة بناءً على نوع القيمة
                            if (valueElement.ValueKind == JsonValueKind.Null ||
                                (valueElement.ValueKind == JsonValueKind.String && string.IsNullOrEmpty(valueElement.GetString())))
                            {
                                continue; // تخطي الفلتر إذا كانت القيمة Null أو فارغة
                            }
                        }


                        if (filterValue.TryGetProperty("MatchMode", out JsonElement matchModeElement))
                        {
                            string matchMode = matchModeElement.GetString();
                            JsonElement value = filterValue.GetProperty("Value");

                            PropertyInfo property = typeof(SiteViewModelForGetAll).GetProperty(field, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

                            if (property != null)
                            {
                                // تحديد النوع الصحيح بناءً على قيمة filter
                                var propertyValue = property.GetValue(SitesViewModels.FirstOrDefault());
                                bool? boolValue = null;
                                string stringValue = null;
                                decimal? decimalValue = null;
                                DateTime? dateTimeValue = null;

                                if (value.ValueKind == JsonValueKind.String)
                                {
                                    stringValue = value.GetString();
                                }
                                else if (value.ValueKind == JsonValueKind.True || value.ValueKind == JsonValueKind.False)
                                {
                                    boolValue = value.GetBoolean();
                                }
                                else if (value.ValueKind == JsonValueKind.Number)
                                {
                                    decimalValue = value.GetDecimal();
                                }
                                else if (value.ValueKind == JsonValueKind.String && DateTime.TryParse(value.GetString(), out DateTime tempDate))
                                {
                                    dateTimeValue = tempDate;
                                }

                                string filterValueLower = stringValue?.ToLower();
                                switch (matchMode)
                                {
                                    case "equals":
                                        if (stringValue != null)
                                        {
                                            SitesViewModels = SitesViewModels.Where(x => property.GetValue(x)?.ToString().ToLower() == filterValueLower);
                                        }
                                        else if (boolValue.HasValue)
                                        {
                                            SitesViewModels = SitesViewModels.Where(x => (bool?)property.GetValue(x) == boolValue.Value);
                                        }
                                        else if (decimalValue.HasValue)
                                        {
                                            SitesViewModels = SitesViewModels.Where(x => decimal.TryParse(property.GetValue(x)?.ToString(), out decimal propValue) && propValue == decimalValue.Value);
                                        }
                                        else if (dateTimeValue.HasValue)
                                        {
                                            SitesViewModels = SitesViewModels.Where(x => DateTime.TryParse(property.GetValue(x)?.ToString(), out DateTime propDate) && propDate.Date == dateTimeValue.Value.Date);
                                        }
                                        break;

                                    case "notequals":
                                        if (stringValue != null)
                                        {
                                            SitesViewModels = SitesViewModels.Where(x => property.GetValue(x)?.ToString().ToLower() != filterValueLower);
                                        }
                                        else if (boolValue.HasValue)
                                        {
                                            SitesViewModels = SitesViewModels.Where(x => (bool?)property.GetValue(x) != boolValue.Value);
                                        }
                                        else if (decimalValue.HasValue)
                                        {
                                            SitesViewModels = SitesViewModels.Where(x => decimal.TryParse(property.GetValue(x)?.ToString(), out decimal propValue) && propValue != decimalValue.Value);
                                        }
                                        else if (dateTimeValue.HasValue)
                                        {
                                            SitesViewModels = SitesViewModels.Where(x => DateTime.TryParse(property.GetValue(x)?.ToString(), out DateTime propDate) && propDate.Date != dateTimeValue.Value.Date);
                                        }
                                        break;

                                    case "startsWith":
                                        if (stringValue != null)
                                        {
                                            SitesViewModels = SitesViewModels.Where(x => property.GetValue(x)?.ToString().ToLower().StartsWith(filterValueLower ?? "") ?? false);
                                        }
                                        else if (boolValue.HasValue)
                                        {
                                            SitesViewModels = SitesViewModels.Where(x => (bool?)property.GetValue(x) == boolValue.Value);
                                        }
                                        break;

                                    case "contains":
                                        if (stringValue != null)
                                        {
                                            SitesViewModels = SitesViewModels.Where(x => property.GetValue(x)?.ToString().ToLower().Contains(filterValueLower ?? "") ?? false);
                                        }
                                        else if (boolValue.HasValue)
                                        {
                                            SitesViewModels = SitesViewModels.Where(x => (bool?)property.GetValue(x) == boolValue.Value);
                                        }
                                        break;

                                    case "notcontains":
                                        if (stringValue != null)
                                        {
                                            SitesViewModels = SitesViewModels.Where(x => !(property.GetValue(x)?.ToString().ToLower().Contains(filterValueLower ?? "") ?? true));
                                        }
                                        else if (boolValue.HasValue)
                                        {
                                            SitesViewModels = SitesViewModels.Where(x => (bool?)property.GetValue(x) != boolValue.Value);
                                        }
                                        break;

                                    case "lt":
                                        if (decimalValue.HasValue)
                                        {
                                            SitesViewModels = SitesViewModels.Where(x => decimal.TryParse(property.GetValue(x)?.ToString(), out decimal propValue) && propValue < decimalValue.Value);
                                        }
                                        break;

                                    case "lte":
                                        if (decimalValue.HasValue)
                                        {
                                            SitesViewModels = SitesViewModels.Where(x => decimal.TryParse(property.GetValue(x)?.ToString(), out decimal propValue) && propValue <= decimalValue.Value);
                                        }
                                        break;

                                    case "gt":
                                        if (decimalValue.HasValue)
                                        {
                                            SitesViewModels = SitesViewModels.Where(x => decimal.TryParse(property.GetValue(x)?.ToString(), out decimal propValue) && propValue > decimalValue.Value);
                                        }
                                        break;

                                    case "gte":
                                        if (decimalValue.HasValue)
                                        {
                                            SitesViewModels = SitesViewModels.Where(x => decimal.TryParse(property.GetValue(x)?.ToString(), out decimal propValue) && propValue >= decimalValue.Value);
                                        }
                                        break;

                                    case "is":
                                        if (dateTimeValue.HasValue)
                                        {
                                            SitesViewModels = SitesViewModels.Where(x => DateTime.TryParse(property.GetValue(x)?.ToString(), out DateTime propDate) && propDate.Date == dateTimeValue.Value.Date);
                                        }
                                        break;

                                    case "isNot":
                                        if (dateTimeValue.HasValue)
                                        {
                                            SitesViewModels = SitesViewModels.Where(x => DateTime.TryParse(property.GetValue(x)?.ToString(), out DateTime propDate) && propDate.Date != dateTimeValue.Value.Date);
                                        }
                                        break;

                                    case "before":
                                        if (dateTimeValue.HasValue)
                                        {
                                            SitesViewModels = SitesViewModels.Where(x => DateTime.TryParse(property.GetValue(x)?.ToString(), out DateTime propDate) && propDate.Date < dateTimeValue.Value.Date);
                                        }
                                        break;

                                    case "after":
                                        if (dateTimeValue.HasValue)
                                        {
                                            SitesViewModels = SitesViewModels.Where(x => DateTime.TryParse(property.GetValue(x)?.ToString(), out DateTime propDate) && propDate.Date > dateTimeValue.Value.Date);
                                        }
                                        break;

                                    default:
                                        throw new InvalidOperationException($"Filter match mode '{matchMode}' is not supported.");
                                }




                            }
                        }
                    }
                }
                

                int count = SitesViewModels.Count();


                // التحقق من MultiSortMeta إذا كانت موجودة
                // التحقق من وجود إعدادات الفرز في MultiSortMeta
                if (filterRequest?.MultiSortMeta?.Count > 0)
                {
                    // تطبيق الفرز الديناميكي بناءً على الحقول المحددة في MultiSortMeta
                    IOrderedEnumerable<SiteViewModelForGetAll> orderedSites = null;

                    foreach (var sortMeta in filterRequest.MultiSortMeta)
                    {
                        var property = typeof(SiteViewModelForGetAll).GetProperty(sortMeta.Field, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

                        if (property != null)
                        {
                            // إذا كانت القائمة غير مرتبة بعد
                            if (orderedSites == null)
                            {
                                orderedSites = sortMeta.Order == 1
                                    ? SitesViewModels.OrderBy(x => property.GetValue(x))
                                    : SitesViewModels.OrderByDescending(x => property.GetValue(x));
                            }
                            else
                            {
                                // إذا كانت القائمة مرتبة مسبقًا، نضيف ترتيبًا ثانويًا
                                orderedSites = sortMeta.Order == 1
                                    ? orderedSites.ThenBy(x => property.GetValue(x))
                                    : orderedSites.ThenByDescending(x => property.GetValue(x));
                            }
                        }
                    }

                    // تحديث النتائج بالترتيب النهائي
                    if (orderedSites != null)
                    {
                        SitesViewModels = orderedSites.ToList();
                    }
                }


                // تابع الكود لبناء النتيجة النهائية كما هو موضح سابقًا

                int skipCount = filterRequest?.First ?? 0;
                int takeCount = filterRequest?.Rows ?? int.MaxValue;

                SitesViewModels = SitesViewModels.Skip(skipCount).Take(takeCount);

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
                            LocationType = CheckLocation != null ? CheckLocation.Name : null,
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
                            SiteVisiteDate= SitesViewModel.SiteVisiteDate,
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
                            LocationType =null,
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
                            SiteVisiteDate = SitesViewModel.SiteVisiteDate,
                            isUsed = AllUsedSites.Any(x => x.ToLower() == SitesViewModel.SiteCode.ToLower()),
                            ItemsOnSite = GetItemsCountOnEachSite != null ?
                                (GetItemsCountOnEachSite.Value ? GetItemsOnSite(SitesViewModel.SiteCode).Data : null) : null
                        });
                    }
                }
                TLItablesNames TableNameEntity = _unitOfWork.TablesNamesRepository
                  .GetWhereFirst(c => c.TableName== "TLIsite");
                if (ExternalSys == true)
                {
                    TLIhistory tLIhistory = new TLIhistory()
                    {
                        TablesNameId = TableNameEntity.Id,
                        ExternalSysId = UserId,
                        HistoryTypeId = 4,
                    };
                    _context.TLIhistory.Add(tLIhistory);
                    _context.SaveChanges();
                }
                return new Response<IEnumerable<SiteViewModelForGetAll>>(true, ListForOutPutOnly, ErrorMessagesWhenReturning, null, (int)Helpers.Constants.ApiReturnCode.success, count);
            }
            catch (Exception er)
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

                return new Response<IEnumerable<SiteViewModelForGetAll>>(false, null, ErrorMessagesWhenReturning, null, (int)Helpers.Constants.ApiReturnCode.NeedUpdate);
            }
        }


        public Response<IEnumerable<SiteViewModelForGetAll>> GetSiteIntegration(int? UserId, string UserName , bool? isRefresh, bool? GetItemsCountOnEachSite)
        {
            string[] ErrorMessagesWhenReturning = null;

        StartAgainWithRefresh:
            try
            {
                if (UserId == null)
                {
                    UserId = _context.TLIexternalSys.FirstOrDefault(x => x.UserName.ToLower() == UserName.ToLower()).Id;
                }
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
                    SitesViewModels = _mapper.Map<IEnumerable<SiteViewModelForGetAll>>(_MySites);





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
                
                var TabelNameId = _context.TLItablesNames.FirstOrDefault(x => x.TableName == "TLIsite").Id;
                TLIhistory tLIhistory = new TLIhistory()
                {
                    TablesNameId = TabelNameId,
                    ExternalSysId = UserId,
                    HistoryTypeId = 4,
                };
                _context.TLIhistory.Add(tLIhistory);
                _context.SaveChanges();
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
        public Response<IEnumerable<SiteDTO>> GetAllSitesWithoutPagination(bool? GetItemsCountOnEachSite)
        {
            string[] ErrorMessagesWhenReturning = null;
            bool isRefresh = true;
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



                IEnumerable<SiteDTO> SitesViewModels;

                if (isRefresh != null ? isRefresh : false)
                {
                    _MySites = _context.TLIsite.AsNoTracking().Include(x => x.Area).Include(x => x.Region)
                            .Include(x => x.siteStatus).ToList();

                    SitesViewModels = _mapper.Map<IEnumerable<SiteDTO>>(_MySites);
                }
                else
                {
                    _MySites.Count();
                    SitesViewModels = _mapper.Map<IEnumerable<SiteDTO>>(_MySites);
                }

                List<SiteDTO> ListForOutPutOnly = new List<SiteDTO>();

                foreach (SiteDTO SitesViewModel in SitesViewModels)
                {
                    string? LocationTypeInModel = _MySites.FirstOrDefault(x => x.SiteCode.ToLower() == SitesViewModel.SiteCode.ToLower())
                        .LocationType;

                    if (!string.IsNullOrEmpty(LocationTypeInModel))
                    {
                        TLIlocationType? CheckLocation = Locations.FirstOrDefault(x => x.Id.ToString() == LocationTypeInModel);

                        ListForOutPutOnly.Add(new SiteDTO()
                        {
                            SiteCode = SitesViewModel.SiteCode,

                            SiteName = SitesViewModel.SiteName,

                            isUsed = AllUsedSites.Any(x => x.ToLower() == SitesViewModel.SiteCode.ToLower()),

                        });
                    }
                    else
                    {
                        ListForOutPutOnly.Add(new SiteDTO()
                        {
                            SiteCode = SitesViewModel.SiteCode,

                            SiteName = SitesViewModel.SiteName,

                            isUsed = AllUsedSites.Any(x => x.ToLower() == SitesViewModel.SiteCode.ToLower()),

                        });
                    }
                }

                return new Response<IEnumerable<SiteDTO>>(true, ListForOutPutOnly, ErrorMessagesWhenReturning, null, (int)Helpers.Constants.ApiReturnCode.success);
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

                return new Response<IEnumerable<SiteDTO>>(true, null, ErrorMessagesWhenReturning, null, (int)Helpers.Constants.ApiReturnCode.success, _MySites.Count());
            }
        }
        public class FilterObjectLists
        {
            public string Key { get; set; }               // اسم الخاصية التي سيتم تطبيق الفلترة عليها
            public List<object> Value { get; set; }       // القيم التي سيتم مطابقتها
            public string MatchMode { get; set; }         // وضع الفلترة (مثل contains, startsWith, equals، إلخ.)
        }

        public Response<IEnumerable<SiteViewModel>> GetAllSites(string ConnectionString, ParameterPagination parameterPagination, List<FilterObjectLists> filters = null)
        {
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
                int totalCount = 0;
                IQueryable<TLIsite> sitesQuery = _context.TLIsite.Include(x => x.Area).Include(x => x.Region).Include(x => x.siteStatus).OrderByDescending(x => x.SiteName);

                // تطبيق الفلاتر الجديدة
                if (filters != null && filters.Any())
                {
                    foreach (FilterObjectLists filter in filters)
                    {
                        PropertyInfo? FilterKeyProperty = typeof(SiteViewModel).GetProperty(filter.Key);

                        if (FilterKeyProperty != null)
                        {
                            foreach (var filterValue in filter.Value)
                            {
                                var matchMode = filter.MatchMode;
                                var filterStringValue = filterValue.ToString()?.ToLower();

                                // تطبيق matchMode بناءً على نوع الفلتر
                                if (FilterKeyProperty.PropertyType == typeof(string))
                                {
                                    switch (matchMode)
                                    {
                                        case "contains":
                                            sitesQuery = sitesQuery.Where(x => EF.Property<string>(x, FilterKeyProperty.Name).ToLower().Contains(filterStringValue));
                                            break;
                                        case "startsWith":
                                            sitesQuery = sitesQuery.Where(x => EF.Property<string>(x, FilterKeyProperty.Name).ToLower().StartsWith(filterStringValue));
                                            break;
                                        case "endsWith":
                                            sitesQuery = sitesQuery.Where(x => EF.Property<string>(x, FilterKeyProperty.Name).ToLower().EndsWith(filterStringValue));
                                            break;
                                        case "equals":
                                            sitesQuery = sitesQuery.Where(x => EF.Property<string>(x, FilterKeyProperty.Name).ToLower() == filterStringValue);
                                            break;
                                        case "notEquals":
                                            sitesQuery = sitesQuery.Where(x => EF.Property<string>(x, FilterKeyProperty.Name).ToLower() != filterStringValue);
                                            break;
                                        default:
                                            break;
                                    }
                                }
                                else if (FilterKeyProperty.PropertyType == typeof(int?) || FilterKeyProperty.PropertyType == typeof(int))
                                {
                                    int filterIntValue = int.Parse(filterStringValue);
                                    switch (matchMode)
                                    {
                                        case "equals":
                                            sitesQuery = sitesQuery.Where(x => EF.Property<int>(x, FilterKeyProperty.Name) == filterIntValue);
                                            break;
                                        case "notEquals":
                                            sitesQuery = sitesQuery.Where(x => EF.Property<int>(x, FilterKeyProperty.Name) != filterIntValue);
                                            break;
                                        case "lessThan":
                                            sitesQuery = sitesQuery.Where(x => EF.Property<int>(x, FilterKeyProperty.Name) < filterIntValue);
                                            break;
                                        case "greaterThan":
                                            sitesQuery = sitesQuery.Where(x => EF.Property<int>(x, FilterKeyProperty.Name) > filterIntValue);
                                            break;
                                        case "lessThanOrEqualTo":
                                            sitesQuery = sitesQuery.Where(x => EF.Property<int>(x, FilterKeyProperty.Name) <= filterIntValue);
                                            break;
                                        case "greaterThanOrEqualTo":
                                            sitesQuery = sitesQuery.Where(x => EF.Property<int>(x, FilterKeyProperty.Name) >= filterIntValue);
                                            break;
                                        default:
                                            break;
                                    }
                                }
                            }
                        }
                    }
                }

                totalCount = sitesQuery.Count();

                sitesQuery = sitesQuery.Skip((parameterPagination.PageNumber - 1) * parameterPagination.PageSize)
                    .Take(parameterPagination.PageSize);

                var sites = sitesQuery.ToList();
                var sitesViewModels = _mapper.Map<IEnumerable<SiteViewModel>>(sites);

                return new Response<IEnumerable<SiteViewModel>>(true, sitesViewModels, null, null, (int)Helpers.Constants.ApiReturnCode.success, totalCount);
            }
            catch (Exception err)
            {
                return new Response<IEnumerable<SiteViewModel>>(false, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
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
                var siteInfo = _context.TLIsite.Include(x => x.Area)
                                                .Include(x => x.Region)
                                                .Include(x => x.siteStatus)
                                                .FirstOrDefault(x => x.SiteCode == SiteCode);
              
                if (siteInfo != null)
                {
                    siteViewModel = new SiteViewModel()
                    {
                        SiteCode = siteInfo.SiteCode,
                        SiteName = siteInfo.SiteName ?? "",
                        siteStatusId = _context.TLIsiteStatus.FirstOrDefault(x => x.Id == siteInfo.siteStatusId).Id ,
                        LocationHieght = siteInfo.LocationHieght,
                        Longitude = siteInfo.Longitude,


                        LocationType = int.TryParse(siteInfo.LocationType, out int locationTypeId)
                          ? (int?)_context.TLIlocationType.FirstOrDefault(x => x.Id == locationTypeId)?.Id
                          : null,

                        Latitude = siteInfo.Latitude,
                        Zone = siteInfo.Zone,
                        AreaId = _context.TLIarea.FirstOrDefault(x => x.Id == siteInfo.AreaId).Id ,
                        RegionCode = _context.TLIregion.FirstOrDefault(x => x.RegionCode == siteInfo.RegionCode)?.RegionCode ?? null,
                        ReservedSpace = siteInfo.ReservedSpace,
                        RentedSpace = siteInfo.RentedSpace,
                        SubArea = siteInfo.SubArea,
                        SiteVisiteDate= siteInfo.SiteVisiteDate
                    };
                }


                return new Response<SiteViewModel>(true, siteViewModel, null, null, (int)Helpers.Constants.ApiReturnCode.success);
            }
            catch (Exception err)
            {
                return new Response<SiteViewModel>(false, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
        }
        public async Task<Response<bool>> EditSitesMainSpaces(float RentedSpace, float ReservedSpace, string SiteCode, int UserId)
        {
            try
            {
                TLIsite site = _unitOfWork.SiteRepository.GetWhereFirst(x => x.SiteCode.ToLower() == SiteCode.ToLower());
                TLIsite Oldsite = _unitOfWork.SiteRepository.GetAllAsQueryable().AsNoTracking().FirstOrDefault(x => x.SiteCode.ToLower() == SiteCode.ToLower());
                if (ReservedSpace != site.ReservedSpace)
                    return new Response<bool>(false, true, null, "You cannot modify the value of reservedspace ", (int)Helpers.Constants.ApiReturnCode.fail);

                if (site.RentedSpace < site.ReservedSpace)
                    return new Response<bool>(false, true, null, "RentedSpace can not be smaller of ReservedSpace", (int)Helpers.Constants.ApiReturnCode.fail);

                else
                {
                    site.RentedSpace = RentedSpace;
                    site.ReservedSpace = ReservedSpace;
                    TLIsite OldSiteData = _MySites.FirstOrDefault(x => x.SiteCode.ToLower() == SiteCode.ToLower());
                    _MySites.Remove(OldSiteData);
                    _MySites.Add(site);
                    _unitOfWork.SiteRepository.UpdateWithHInstallationSite(UserId, null, Oldsite, site, SiteCode);
                    _unitOfWork.SaveChanges();
                }
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
        //public async Task<string> GetSMIS_Site(string UserName, string Password, string ViewName, string Paramater, string RowContent)
        //{
        //    try
        //    {
        //        ServiceProvider serviceProvider = _services.BuildServiceProvider();
        //        IConfiguration Configuration = serviceProvider.GetService<IConfiguration>();
        //        HttpWebRequest Request = !string.IsNullOrEmpty(Paramater) ?
        //            (HttpWebRequest)WebRequest.Create(Configuration["SMIS_API_URL"] + $"{UserName}/{Password}/{ViewName}/'{Paramater}'") :
        //            (HttpWebRequest)WebRequest.Create(Configuration["SMIS_API_URL"] + $"{UserName}/{Password}/{ViewName}");

        //        Request.Method = "GET";


        //        if (!string.IsNullOrEmpty(RowContent))
        //        {
        //            Request.ContentType = "text/plain";

        //            ASCIIEncoding encoding = new ASCIIEncoding();
        //            byte[] BodyText = encoding.GetBytes(RowContent);

        //            Stream NewStream = Request.GetRequestStream();
        //            NewStream.Write(BodyText, 0, BodyText.Length);
        //            Request.ContentLength = BodyText.Length;
        //        }

        //        string SMIS_Response = "";
        //        using (WebResponse WebResponse = Request.GetResponse())
        //        {
        //            using (StreamReader Reader = new StreamReader(WebResponse.GetResponseStream()))
        //            {
        //                SMIS_Response = Reader.ReadToEnd();
        //            }
        //        }

        //        // تعديل هذا الجزء من الكود
        //        List<SiteDataFromOutsiderApiViewModel> SiteViewModelLists =
        //            JsonConvert.DeserializeObject<List<SiteDataFromOutsiderApiViewModel>>(SMIS_Response);
        //        using (TransactionScope transaction = new TransactionScope())
        //        {
        //            foreach (SiteDataFromOutsiderApiViewModel item in SiteViewModelLists)
        //            {
        //                TLIsite CheckSiteCodeIfExist = _unitOfWork.SiteRepository
        //                    .GetWhereFirst(x => x.SiteCode.ToLower() == item.Sitecode.ToLower() ||
        //                        x.SiteName.ToLower() == item.Sitename.ToLower());

        //                if (CheckSiteCodeIfExist != null)
        //                {
        //                    CheckSiteCodeIfExist.SiteCode = item.Sitecode;
        //                    CheckSiteCodeIfExist.SiteName = item.Sitename;
        //                    CheckSiteCodeIfExist.LocationType = item.LocationType;
        //                    CheckSiteCodeIfExist.Latitude = item.Latitude;
        //                    CheckSiteCodeIfExist.Longitude = item.Longitude;
        //                    CheckSiteCodeIfExist.Zone = item.Zone;
        //                    CheckSiteCodeIfExist.SubArea = item.Subarea;
        //                    CheckSiteCodeIfExist.STATUS_DATE = item.Statusdate;
        //                    CheckSiteCodeIfExist.CREATE_DATE = item.Createddate;
        //                    CheckSiteCodeIfExist.LocationHieght = item.LocationHieght;
        //                    CheckSiteCodeIfExist.LocationType = item.LocationType;
        //                    CheckSiteCodeIfExist.RentedSpace = item.RentedSpace;
        //                    CheckSiteCodeIfExist.ReservedSpace = item.ReservedSpace;
        //                    CheckSiteCodeIfExist.SiteVisiteDate = item.SiteVisiteDate;
        //                    TLIarea CheckAreaIfExist = _unitOfWork.AreaRepository
        //                        .GetWhereFirst(x => x.AreaName.ToLower() == item.Area.ToLower());

        //                    if (CheckAreaIfExist == null)
        //                    {
        //                        TLIarea AddNewArea = new TLIarea
        //                        {
        //                            AreaName = item.Area
        //                        };
        //                        await _unitOfWork.AreaRepository.AddAsync(AddNewArea);
        //                        await _unitOfWork.SaveChangesAsync();

        //                        CheckSiteCodeIfExist.AreaId = AddNewArea.Id;
        //                    }
        //                    else
        //                    {
        //                        CheckSiteCodeIfExist.AreaId = CheckAreaIfExist.Id;
        //                    }

        //                    TLIregion CheckRegonIfExist = await _context.TLIregion
        //                        .FirstOrDefaultAsync(x => x.RegionCode.ToLower() == item.RegionCode.ToLower());

        //                    if (CheckRegonIfExist == null)
        //                    {
        //                        await _context.TLIregion.AddAsync(new TLIregion { RegionCode = item.RegionCode });
        //                        await _context.SaveChangesAsync();

        //                        CheckSiteCodeIfExist.RegionCode = item.RegionCode;
        //                    }
        //                    else
        //                    {
        //                        CheckSiteCodeIfExist.RegionCode = CheckRegonIfExist.RegionCode;
        //                    }

        //                    TLIsiteStatus ChecksiteStatusIfExist = _unitOfWork.SiteStatusRepository
        //                       .GetWhereFirst(x => x.Name.ToLower() == item.siteStatus.ToLower());

        //                    if (ChecksiteStatusIfExist == null)
        //                    {
        //                        TLIsiteStatus AddNewsiteStatus = new TLIsiteStatus
        //                        {
        //                            Name = item.siteStatus
        //                        };
        //                        await _unitOfWork.SiteStatusRepository.AddAsync(AddNewsiteStatus);
        //                        await _unitOfWork.SaveChangesAsync();

        //                        CheckSiteCodeIfExist.siteStatusId = AddNewsiteStatus.Id;
        //                    }
        //                    else
        //                    {
        //                        CheckSiteCodeIfExist.siteStatusId = ChecksiteStatusIfExist.Id;
        //                    }

        //                    TLIlocationType ChecklocationTypeIfExist = _unitOfWork.LocationTypeRepository
        //                      .GetWhereFirst(x => x.Name.ToLower() == item.LocationType.ToLower());

        //                    if (ChecklocationTypeIfExist == null)
        //                    {
        //                        TLIlocationType AddNewTLIlocationType = new TLIlocationType
        //                        {
        //                            Name = item.LocationType
        //                        };
        //                        await _unitOfWork.LocationTypeRepository.AddAsync(AddNewTLIlocationType);
        //                        await _unitOfWork.SaveChangesAsync();

        //                        CheckSiteCodeIfExist.LocationType = AddNewTLIlocationType.Id.ToString();
        //                    }
        //                    else
        //                    {
        //                        CheckSiteCodeIfExist.LocationType = ChecklocationTypeIfExist.Id.ToString();
        //                    }

        //                    await _unitOfWork.SaveChangesAsync();
        //                }
        //                else
        //                {
        //                    TLIsite NewSiteToAdd = new TLIsite();

        //                    NewSiteToAdd.SiteCode = item.Sitecode;
        //                    TLIregion CheckRegonIfExist = await _context.TLIregion
        //                        .FirstOrDefaultAsync(x => x.RegionCode.ToLower() == item.RegionCode.ToLower());

        //                    if (CheckRegonIfExist == null)
        //                    {
        //                        await _context.TLIregion.AddAsync(new TLIregion { RegionCode = item.RegionCode });
        //                        await _context.SaveChangesAsync();

        //                        NewSiteToAdd.RegionCode = item.RegionCode;
        //                    }
        //                    else
        //                    {
        //                        NewSiteToAdd.RegionCode = CheckRegonIfExist.RegionCode;
        //                    }

        //                    TLIarea CheckAreaIfExist = _unitOfWork.AreaRepository
        //                        .GetWhereFirst(x => x.AreaName.ToLower() == item.Area.ToLower());

        //                    if (CheckAreaIfExist == null)
        //                    {
        //                        TLIarea AddNewArea = new TLIarea
        //                        {
        //                            AreaName = item.Area
        //                        };
        //                        await _unitOfWork.AreaRepository.AddAsync(AddNewArea);
        //                        await _unitOfWork.SaveChangesAsync();

        //                        NewSiteToAdd.AreaId = AddNewArea.Id;
        //                    }
        //                    else
        //                    {
        //                        NewSiteToAdd.AreaId = CheckAreaIfExist.Id;
        //                    }



        //                    TLIsiteStatus ChecksiteStatusIfExist = _unitOfWork.SiteStatusRepository
        //                       .GetWhereFirst(x => x.Name.ToLower() == item.siteStatus.ToLower());

        //                    if (ChecksiteStatusIfExist == null)
        //                    {
        //                        TLIsiteStatus AddNewsiteStatus = new TLIsiteStatus
        //                        {
        //                            Name = item.siteStatus
        //                        };
        //                        await _unitOfWork.SiteStatusRepository.AddAsync(AddNewsiteStatus);
        //                        await _unitOfWork.SaveChangesAsync();

        //                        NewSiteToAdd.siteStatusId = AddNewsiteStatus.Id;
        //                    }
        //                    else
        //                    {
        //                        NewSiteToAdd.siteStatusId = ChecksiteStatusIfExist.Id;
        //                    }

        //                    TLIlocationType ChecklocationTypeIfExist = _unitOfWork.LocationTypeRepository
        //                      .GetWhereFirst(x => x.Name.ToLower() == item.LocationType.ToLower());

        //                    if (ChecklocationTypeIfExist == null)
        //                    {
        //                        TLIlocationType AddNewTLIlocationType = new TLIlocationType
        //                        {
        //                            Name = item.LocationType
        //                        };
        //                        await _unitOfWork.LocationTypeRepository.AddAsync(AddNewTLIlocationType);
        //                        await _unitOfWork.SaveChangesAsync();

        //                        NewSiteToAdd.LocationType = AddNewTLIlocationType.Id.ToString();
        //                    }
        //                    else
        //                    {
        //                        NewSiteToAdd.LocationType = ChecklocationTypeIfExist.Id.ToString();
        //                    }
        //                    NewSiteToAdd.Latitude = item.Latitude;
        //                    NewSiteToAdd.Longitude = item.Longitude;
        //                    NewSiteToAdd.Zone = item.Zone;
        //                    NewSiteToAdd.SubArea = item.Subarea;
        //                    NewSiteToAdd.STATUS_DATE = item.Statusdate;
        //                    NewSiteToAdd.CREATE_DATE = item.Createddate;
        //                    NewSiteToAdd.LocationHieght = item.LocationHieght;
        //                    NewSiteToAdd.LocationType = item.LocationType;
        //                    NewSiteToAdd.RentedSpace = item.RentedSpace;
        //                    NewSiteToAdd.ReservedSpace = item.ReservedSpace;
        //                    NewSiteToAdd.SiteVisiteDate = item.SiteVisiteDate;

        //                    await _unitOfWork.SiteRepository.AddAsync(NewSiteToAdd);
        //                    await _unitOfWork.SaveChangesAsync();
        //                }
        //            }

        //            _MySites = _context.TLIsite.AsNoTracking()
        //                .Include(x => x.siteStatus).Include(x => x.Region).Include(x => x.AreaId).ToList();

        //            transaction.Complete();
        //        }
        //        return "No Error Found";
        //    }
        //    catch (Exception err)
        //    {
        //        return err.Message;
        //    }
        //}
        //public async Task<string> GetSMIS_Site(string UserName, string Password, string ViewName, string Paramater, string RowContent)
        //{
        //    try
        //    {
        //        var serviceProvider = _services.BuildServiceProvider();
        //        var configuration = serviceProvider.GetService<IConfiguration>();
        //        string apiUrl = configuration["SMIS_API_URL"] + $"{UserName}/{Password}/{ViewName}";

        //        if (!string.IsNullOrEmpty(Paramater))
        //            apiUrl += $"/'{Paramater}'";

        //        var request = (HttpWebRequest)WebRequest.Create(apiUrl);
        //        request.Method = "GET";

        //        if (!string.IsNullOrEmpty(RowContent))
        //        {
        //            request.ContentType = "text/plain";
        //            byte[] bodyText = Encoding.ASCII.GetBytes(RowContent);
        //            using (var newStream = request.GetRequestStream())
        //            {
        //                newStream.Write(bodyText, 0, bodyText.Length);
        //                request.ContentLength = bodyText.Length;
        //            }
        //        }

        //        string responseText;
        //        using (var webResponse = request.GetResponse())
        //        using (var reader = new StreamReader(webResponse.GetResponseStream()))
        //        {
        //            responseText = reader.ReadToEnd();
        //        }

        //        var siteDataList = JsonConvert.DeserializeObject<List<SiteDataFromOutsiderApiViewModel>>(responseText);

        //        using (var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
        //        {
        //            foreach (var item in siteDataList)
        //            {
        //                var existingSite = _unitOfWork.SiteRepository.GetWhereFirst(x => x.SiteCode == item.Sitecode || x.SiteName == item.Sitename);

        //                if (existingSite != null)
        //                {
        //                    await UpdateSiteAsync(existingSite, item);
        //                }
        //                else
        //                {
        //                    await AddNewSiteAsync(item);
        //                }
        //            }
        //            transaction.Complete();
        //        }
        //        return "تمت العملية بنجاح";
        //    }
        //    catch (Exception ex)
        //    {
        //        return ex.Message;
        //    }
        //}
        //public async Task<string> GetSMIS_Site(string UserName, string Password, string ViewName, string Paramater, string RowContent)
        //{
        //    try
        //    {
        //        var serviceProvider = _services.BuildServiceProvider();
        //        var configuration = serviceProvider.GetService<IConfiguration>();
        //        string apiUrl = configuration["SMIS_API_URL"]; // رابط الـ API

        //        if (string.IsNullOrEmpty(apiUrl))
        //        {
        //            return "رابط الـ API غير موجود أو غير صحيح.";
        //        }

        //        // تكوين الطلب
        //        HttpWebRequest request = !string.IsNullOrEmpty(Paramater)
        //            ? (HttpWebRequest)WebRequest.Create($"{apiUrl}{UserName}/{Password}/{ViewName}/'{Paramater}'")
        //            : (HttpWebRequest)WebRequest.Create($"{apiUrl}{UserName}/{Password}/{ViewName}");

        //        request.Method = "GET";

        //        if (!string.IsNullOrEmpty(RowContent))
        //        {
        //            request.ContentType = "text/plain";
        //            ASCIIEncoding encoding = new ASCIIEncoding();
        //            byte[] bodyText = encoding.GetBytes(RowContent);

        //            using (Stream newStream = request.GetRequestStream())
        //            {
        //                newStream.Write(bodyText, 0, bodyText.Length);
        //            }
        //            request.ContentLength = bodyText.Length;
        //        }

        //        // الحصول على الاستجابة من API
        //        string smisResponse;
        //        using (WebResponse webResponse = await request.GetResponseAsync())
        //        {
        //            using (StreamReader reader = new StreamReader(webResponse.GetResponseStream()))
        //            {
        //                smisResponse = await reader.ReadToEndAsync();
        //            }
        //        }

        //        // تحويل النص إلى قائمة من الكائنات
        //        var siteDataList = JsonConvert.DeserializeObject<List<SiteDataFromOutsiderApiViewModel>>(smisResponse);

        //        int batchSize = 100; // حجم الدفعة
        //        var transactionOptions = new TransactionOptions
        //        {
        //            IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted, // تحديد المساحة بشكل صريح
        //            Timeout = TimeSpan.FromMinutes(10) // زيادة المهلة
        //        };

        //        for (int i = 0; i < siteDataList.Count; i += batchSize)
        //        {
        //            var batch = siteDataList.Skip(i).Take(batchSize).ToList();

        //            using (var transaction = new TransactionScope(TransactionScopeOption.Required, transactionOptions, TransactionScopeAsyncFlowOption.Enabled))
        //            {
        //                foreach (var item in batch)
        //                {
        //                    try
        //                    {
        //                        int areaId = await GetAreaIdAsync(item.Area);
        //                        string regionCode = await GetRegionCodeAsync(item.RegionCode);
        //                        int siteStatusId = await GetSiteStatusIdAsync(item.siteStatus);
        //                        string locationTypeId = await GetLocationTypeIdAsync(item.LocationType);

        //                        var existingSite = _unitOfWork.SiteRepository.GetWhereFirst(x => x.SiteCode == item.Sitecode || x.SiteName == item.Sitename);

        //                        if (existingSite != null)
        //                        {
        //                            existingSite.SiteCode = item.Sitecode;
        //                            existingSite.SiteName = item.Sitename;
        //                            existingSite.LocationType = locationTypeId;
        //                            existingSite.Latitude = item.Latitude;
        //                            existingSite.Longitude = item.Longitude;
        //                            existingSite.Zone = item.Zone;
        //                            existingSite.SubArea = item.Subarea;
        //                            existingSite.STATUS_DATE = item.Statusdate;
        //                            existingSite.CREATE_DATE = item.Createddate;
        //                            existingSite.LocationHieght = item.LocationHieght ?? 0;
        //                            existingSite.AreaId = areaId;
        //                            existingSite.RegionCode = regionCode;

        //                            await _unitOfWork.SaveChangesAsync();
        //                        }
        //                        else
        //                        {
        //                            var newSite = new TLIsite
        //                            {
        //                                SiteCode = item.Sitecode,
        //                                SiteName = item.Sitename,
        //                                Latitude = item.Latitude,
        //                                Longitude = item.Longitude,
        //                                Zone = item.Zone,
        //                                SubArea = item.Subarea,
        //                                STATUS_DATE = item.Statusdate,
        //                                CREATE_DATE = item.Createddate,
        //                                LocationHieght = item.LocationHieght ?? 0,
        //                                RentedSpace = 0,
        //                                ReservedSpace = 0,
        //                                SiteVisiteDate = DateTime.Now,
        //                                AreaId = areaId,
        //                                RegionCode = regionCode,
        //                                siteStatusId = siteStatusId,
        //                                LocationType = locationTypeId
        //                            };

        //                            await _unitOfWork.SiteRepository.AddAsync(newSite);
        //                            await _unitOfWork.SaveChangesAsync();
        //                        }
        //                    }
        //                    catch (Exception ex)
        //                    {
        //                        // تسجيل الخطأ ومتابعة معالجة باقي السجلات
        //                        Console.WriteLine($"Error processing site {item.Sitecode}: {ex.Message}");
        //                    }
        //                }

        //                transaction.Complete();
        //            }
        //        }

        //        return "تمت العملية بنجاح";
        //    }
        //    catch (Exception ex)
        //    {
        //        return ex.Message;
        //    }
        //}



        public async Task<string> GetSMIS_Site(string UserName, string Password, string ViewName, string Paramater, string RowContent)
        {
            try
            {
                var serviceProvider = _services.BuildServiceProvider();
                var configuration = serviceProvider.GetService<IConfiguration>();
                string apiUrl = configuration["SMIS_API_URL"]; // رابط الـ API

                if (string.IsNullOrEmpty(apiUrl))
                {
                    return "رابط الـ API غير موجود أو غير صحيح.";
                }

                // تكوين الطلب
                HttpWebRequest request = !string.IsNullOrEmpty(Paramater)
                    ? (HttpWebRequest)WebRequest.Create($"{apiUrl}{UserName}/{Password}/{ViewName}/'{Paramater}'")
                    : (HttpWebRequest)WebRequest.Create($"{apiUrl}{UserName}/{Password}/{ViewName}");

                request.Method = "GET";

                if (!string.IsNullOrEmpty(RowContent))
                {
                    request.ContentType = "text/plain";
                    ASCIIEncoding encoding = new ASCIIEncoding();
                    byte[] bodyText = encoding.GetBytes(RowContent);

                    using (Stream newStream = request.GetRequestStream())
                    {
                        newStream.Write(bodyText, 0, bodyText.Length);
                    }
                    request.ContentLength = bodyText.Length;
                }

                // الحصول على الاستجابة من API
                string smisResponse;
                using (WebResponse webResponse = await request.GetResponseAsync())
                {
                    using (StreamReader reader = new StreamReader(webResponse.GetResponseStream()))
                    {
                        smisResponse = await reader.ReadToEndAsync();
                    }
                }

                // تحويل النص إلى قائمة من الكائنات
                var siteDataList = JsonConvert.DeserializeObject<List<SiteDataFromOutsiderApiViewModel>>(smisResponse);

                //تقسيم البيانات إلى دفعات
                int batchSize = 2500; // عدد العناصر في كل دفعة
                int totalBatches = (int)Math.Ceiling((double)siteDataList.Count / batchSize);

                for (int batchIndex = 0; batchIndex < totalBatches; batchIndex++)
                {
                    var batch = siteDataList.Skip(batchIndex * batchSize).Take(batchSize).ToList();

                    using (var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                    {
                        foreach (var item in batch)
                        {
                            int areaId = await GetAreaIdAsync(item.Area);
                            string regionCode = await GetRegionCodeAsync(item.RegionCode);
                            int siteStatusId = await GetSiteStatusIdAsync(item.siteStatus);
                            string locationTypeId = await GetLocationTypeIdAsync(item.LocationType);

                            var existingSite = _unitOfWork.SiteRepository.GetWhereFirst(x => x.SiteCode == item.Sitecode || x.SiteName == item.Sitename);

                            if (existingSite != null)
                            {
                                // تحديث الكيان الحالي
                                existingSite.SiteCode = item.Sitecode;
                                existingSite.SiteName = item.Sitename;
                                existingSite.LocationType = locationTypeId;
                                existingSite.Latitude = item.Latitude;
                                existingSite.Longitude = item.Longitude;
                                existingSite.Zone = item.Zone;
                                existingSite.SubArea = item.Subarea;
                                existingSite.STATUS_DATE = item.Statusdate;
                                existingSite.CREATE_DATE = item.Createddate;
                                existingSite.LocationHieght = item.LocationHieght ?? 0;
                                existingSite.AreaId = areaId;
                                existingSite.RegionCode = regionCode;
                                existingSite.siteStatusId = siteStatusId;

                                await _unitOfWork.SaveChangesAsync();
                            }
                            else
                            {
                                // إضافة كيان جديد
                                var newSite = new TLIsite
                                {
                                    SiteCode = item.Sitecode,
                                    SiteName = item.Sitename,
                                    Latitude = item.Latitude,
                                    Longitude = item.Longitude,
                                    Zone = item.Zone,
                                    SubArea = item.Subarea,
                                    STATUS_DATE = item.Statusdate,
                                    CREATE_DATE = item.Createddate,
                                    LocationHieght = item.LocationHieght ?? 0,
                                    RentedSpace = 0,
                                    ReservedSpace = 0,
                                    SiteVisiteDate = DateTime.Now,
                                    AreaId = areaId,
                                    RegionCode = regionCode,
                                    siteStatusId = siteStatusId,
                                    LocationType = locationTypeId
                                };

                                await _unitOfWork.SiteRepository.AddAsync(newSite);
                                await _unitOfWork.SaveChangesAsync();
                            }
                        }

                        transaction.Complete();
                    }
                }

                return "تمت العملية بنجاح";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        private async Task UpdateSiteAsync(TLIsite site, SiteDataFromOutsiderApiViewModel item)
        {
            site.SiteCode = item.Sitecode;
            site.SiteName = item.Sitename;
            site.LocationType = await GetLocationTypeIdAsync(item.LocationType);
            site.Latitude = item.Latitude;
            site.Longitude = item.Longitude;
            site.Zone = item.Zone;
            site.SubArea = item.Subarea;
            site.STATUS_DATE = item.Statusdate;
            site.CREATE_DATE = item.Createddate;
            site.LocationHieght = item.LocationHieght ?? 0;
            site.AreaId = await GetAreaIdAsync(item.Area);
            site.RegionCode = await GetRegionCodeAsync(item.RegionCode);
            site.siteStatusId = await GetSiteStatusIdAsync(item.siteStatus);

            await _unitOfWork.SaveChangesAsync();
        }

        private async Task AddNewSiteAsync(SiteDataFromOutsiderApiViewModel item)
        {
            var newSite = new TLIsite
            {
                SiteCode = item.Sitecode,
                SiteName = item.Sitename,
                Latitude = item.Latitude,
                Longitude = item.Longitude,
                Zone = item.Zone,
                SubArea = item.Subarea,
                STATUS_DATE = item.Statusdate,
                CREATE_DATE = item.Createddate,
                LocationHieght = item.LocationHieght ?? 0,
                RentedSpace = 0,
                ReservedSpace = 0,
                SiteVisiteDate = DateTime.Now,
                AreaId = await GetAreaIdAsync(item.Area),
                RegionCode = await GetRegionCodeAsync(item.RegionCode),
                siteStatusId = await GetSiteStatusIdAsync(item.siteStatus),
                LocationType = await GetLocationTypeIdAsync(item.LocationType)
            };

            await _unitOfWork.SiteRepository.AddAsync(newSite);
            await _unitOfWork.SaveChangesAsync();
        }

        private async Task<int> GetAreaIdAsync(string areaName)
        {
            var existingArea = await _context.TLIarea.FirstOrDefaultAsync(x => x.AreaName == areaName);
            if (existingArea != null) return existingArea.Id;

            var newArea = new TLIarea { AreaName = areaName };
            await _unitOfWork.AreaRepository.AddAsync(newArea);
            await _unitOfWork.SaveChangesAsync();
            return newArea.Id;
        }

        private async Task<string> GetRegionCodeAsync(string regionCode)
        {
            var existingRegion = await _context.TLIregion.FirstOrDefaultAsync(x => x.RegionCode == regionCode);
            if (existingRegion != null) return existingRegion.RegionCode;

            var newRegion = new TLIregion { RegionCode = regionCode };
            await _context.TLIregion.AddAsync(newRegion);
            await _context.SaveChangesAsync();
            return newRegion.RegionCode;
        }

        private async Task<int> GetSiteStatusIdAsync(string siteStatus)
        {
            var existingStatus = await _context.TLIsiteStatus.FirstOrDefaultAsync(x => x.Name == "On Air");
            if (existingStatus != null) return existingStatus.Id;

            var newStatus = new TLIsiteStatus { Name = "On Air" };
            await _unitOfWork.SiteStatusRepository.AddAsync(newStatus);
            await _unitOfWork.SaveChangesAsync();
            return newStatus.Id;
        }

        private async Task<string> GetLocationTypeIdAsync(string locationType)
        {
            var existingLocationType = await _context.TLIlocationType.FirstOrDefaultAsync(x => x.Name == locationType);
            if (existingLocationType != null) return existingLocationType.Id.ToString();

            var newLocationType = new TLIlocationType { Name = locationType };
            await _unitOfWork.LocationTypeRepository.AddAsync(newLocationType);
            await _unitOfWork.SaveChangesAsync();
            return newLocationType.Id.ToString();
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

            List<TLIcivilLoads> UsedSitesInLoads = _unitOfWork.CivilLoadsRepository
            .GetWhereAndInclude(x => x.SiteCode.ToLower() == SiteCode.ToLower() && !x.Dismantle
                && x.allLoadInstId != null, x => x.allLoadInst, x => x.sideArm)
            .AsQueryable()
            .ToList();
            OutPut.PowerCount = UsedSitesInLoads.Count(x => !x.allLoadInst.Draft && x.allLoadInst.powerId != null);
            OutPut.PowerCount = UsedSitesInLoads.Count(x => x.allLoadInst.Draft == false && x.allLoadInst.powerId != null);
            OutPut.MW_RFUCount = UsedSitesInLoads.Count(x => x.allLoadInst.Draft == false && x.allLoadInst.mwRFUId != null);
            OutPut.MW_BUCount = UsedSitesInLoads.Count(x => x.allLoadInst.Draft == false && x.allLoadInst.mwBUId != null);
            OutPut.MW_DishCount = UsedSitesInLoads.Count(x => x.allLoadInst.Draft == false && x.allLoadInst.mwDishId != null);
            OutPut.MW_ODUCount = UsedSitesInLoads.Count(x => x.allLoadInst.Draft == false && x.allLoadInst.mwODUId != null);
            OutPut.MW_OtherCount = UsedSitesInLoads.Count(x => x.allLoadInst.Draft == false && x.allLoadInst.mwOtherId != null);
            OutPut.RadioAntennaCount = UsedSitesInLoads.Count(x => x.allLoadInst.Draft == false && x.allLoadInst.radioAntennaId != null);
            OutPut.RadioRRUCount = UsedSitesInLoads.Count(x => x.allLoadInst.Draft == false && x.allLoadInst.radioRRUId != null);
            OutPut.RadioOtherCount = UsedSitesInLoads.Count(x => x.allLoadInst.Draft == false && x.allLoadInst.radioOtherId != null);
            OutPut.LoadOtherCount = UsedSitesInLoads.Count(x => x.allLoadInst.Draft == false && x.allLoadInst.loadOtherId != null);
            OutPut.SideArmCount = UsedSitesInLoads.Count(x => x.sideArmId != null && x.sideArm.Draft == false);


            IQueryable<TLIallCivilInst> UsedSitesInCivils = _unitOfWork.CivilSiteDateRepository
              .GetWhereAndInclude(x => x.SiteCode.ToLower() == SiteCode.ToLower()
                  && !x.Dismantle && x.allCivilInst.Draft == false, x => x.allCivilInst, x => x.allCivilInst.civilWithLegs,
                  x => x.allCivilInst.civilWithoutLeg, x => x.allCivilInst.civilNonSteel,
                  x => x.allCivilInst.civilWithoutLeg.CivilWithoutlegsLib,
                  x => x.allCivilInst.civilWithoutLeg.CivilWithoutlegsLib.CivilWithoutLegCategory)
              .Select(x => x.allCivilInst)
              .AsQueryable();

            int xxx = UsedSitesInCivils.Count();
            OutPut.SteelWithLegsCount = UsedSitesInCivils.Count(x => x.civilWithLegsId != null);
            OutPut.SteelWithoutLegs_MastCount = UsedSitesInCivils.Count(x => x.civilWithoutLegId != null ?
               (x.civilWithoutLeg.CivilWithoutlegsLib.CivilWithoutLegCategoryId != null ?
                x.civilWithoutLeg.CivilWithoutlegsLib.CivilWithoutLegCategory.Name.ToLower() == Helpers.Constants.CivilWithoutLegCategories.Mast.ToString().ToLower() : false) : false);
            OutPut.SteelWithoutLegs_MonopoleCount = UsedSitesInCivils.Count(x => x.civilWithoutLegId != null ?
               (x.civilWithoutLeg.CivilWithoutlegsLib.CivilWithoutLegCategoryId != null ?
                x.civilWithoutLeg.CivilWithoutlegsLib.CivilWithoutLegCategory.Name.ToLower() == Helpers.Constants.CivilWithoutLegCategories.Monopole.ToString().ToLower() : false) : false);
            OutPut.SteelWithoutLegs_CapsuleCount = UsedSitesInCivils.Count(x => x.civilWithoutLegId != null ?
               (x.civilWithoutLeg.CivilWithoutlegsLib.CivilWithoutLegCategoryId != null ?
                x.civilWithoutLeg.CivilWithoutlegsLib.CivilWithoutLegCategory.Name.ToLower() == Helpers.Constants.CivilWithoutLegCategories.Capsule.ToString().ToLower() : false) : false);
            OutPut.NonSteelCount = UsedSitesInCivils.Count(x => x.civilNonSteelId != null);

            IQueryable<TLIallOtherInventoryInst> UsedSitesInOtherInventories = _unitOfWork.OtherInSiteRepository
              .GetWhereAndInclude(x => x.SiteCode.ToLower() == SiteCode.ToLower()
                  && !x.Dismantle && x.allOtherInventoryInst.Draft == false, x => x.allOtherInventoryInst,
                  x => x.allOtherInventoryInst.cabinet, x => x.allOtherInventoryInst.solar, x => x.allOtherInventoryInst.generator)
              .Select(x => x.allOtherInventoryInst)
              .AsQueryable();

            OutPut.CabinetPowerCount = UsedSitesInOtherInventories.Count(x => x.cabinetId != null ?
                x.cabinet.CabinetPowerLibraryId != null : false);
            OutPut.CabinetTelecomCount = UsedSitesInOtherInventories.Count(x => x.cabinetId != null ?
                x.cabinet.CabinetTelecomLibraryId != null : false);
            OutPut.SolarCount = UsedSitesInOtherInventories.Count(x => x.solarId != null);
            OutPut.GeneratorCount = UsedSitesInOtherInventories.Count(x => x.generatorId != null);

            return new Response<ItemsOnSite>(true, OutPut, null, null, (int)Helpers.Constants.ApiReturnCode.success);
        }
        private readonly string _connectionString;
        public List<dynamic> ExecuteStoredProcedureAndQueryDynamicView(string ConnectionString, string sitecode, string encodedFilter,int? operationCounter)
        {
            using (var connection = new OracleConnection(ConnectionString))
            {
                connection.Open();

                List<dynamic> result = new List<dynamic>();

                switch (operationCounter)
                {
                    case 1:
                     
                        string sqlQuery1 = @"select sum(WITHLEGS) WITHLEGS, sum(MAST) MAST, sum(MONOPOLE) MONOPOLE, 
                                  sum(CAPSULE) CAPSULE, sum(NONSTEEL) NONSTEEL, sum(SIDEARM) SIDEARM, 
                                  sum(MWDISH) MWDISH, sum(MWODU) MWODU, sum(MWBU) MWBU, sum(MWRFU) MWRFU, 
                                  sum(MWOTHER) MWOTHER, sum(RADIO_ANTENNA) RADIO_ANTENNA, sum(RADIO_RRU) RADIO_RRU, 
                                  sum(RADIO_OTHER) RADIO_OTHER, sum(POWER) POWER, sum(LOAD_OTHER) LOAD_OTHER, 
                                  sum(GENERATOR) GENERATOR, sum(SOLAR) SOLAR, sum(CABINET_POWER) CABINET_POWER, 
                                  sum(CABINET_TELECOM) CABINET_TELECOM 
                                  from COUNT_SITE";
                        using (OracleCommand queryCommand1 = new OracleCommand(sqlQuery1, connection))
                        {
                            using (OracleDataReader reader1 = queryCommand1.ExecuteReader())
                            {
                                while (reader1.Read())
                                {
                                    dynamic dynamicResult = new ExpandoObject();
                                    var properties = (IDictionary<string, object>)dynamicResult;

                                    for (int i = 0; i < reader1.FieldCount; i++)
                                    {
                                        properties[reader1.GetName(i)] = reader1[i];
                                    }

                                    result.Add(dynamicResult);
                                }
                            }
                        }
                        break;

                    case 2:
                       
                        string sqlQuery2 = @"select sitecode, sitename, 
                                      case when WITHLEGS = 0 and SIDEARM = 0 and RADIO_RRU = 0 and RADIO_ANTENNA = 0 
                                           and POWER = 0 and NONSTEEL = 0 and MWODU = 0 and MWDISH = 0 
                                           and MONOPOLE = 0 and MAST = 0 and GENERATOR = 0 and CAPSULE = 0 
                                           then 0 else 1 end as isused 
                                      from count_site";
                        using (OracleCommand queryCommand2 = new OracleCommand(sqlQuery2, connection))
                        {
                            using (OracleDataReader reader2 = queryCommand2.ExecuteReader())
                            {
                                while (reader2.Read())
                                {
                                    dynamic dynamicResult = new ExpandoObject();
                                    var properties = (IDictionary<string, object>)dynamicResult;

                                    for (int i = 0; i < reader2.FieldCount; i++)
                                    {
                                        properties[reader2.GetName(i)] = reader2[i];
                                    }

                                    result.Add(dynamicResult);
                                }
                            }
                        }
                        break;

                    case 3:
                     
                        string sqlQuery3 = @"select * from count_site where sitecode = :sitecode";
                        using (OracleCommand queryCommand3 = new OracleCommand(sqlQuery3, connection))
                        {
                            queryCommand3.Parameters.Add(new OracleParameter("sitecode", sitecode));

                            using (OracleDataReader reader3 = queryCommand3.ExecuteReader())
                            {
                                while (reader3.Read())
                                {
                                    dynamic dynamicResult = new ExpandoObject();
                                    var properties = (IDictionary<string, object>)dynamicResult;

                                    for (int i = 0; i < reader3.FieldCount; i++)
                                    {
                                        properties[reader3.GetName(i)] = reader3[i];
                                    }

                                    result.Add(dynamicResult);
                                }
                            }
                        }
                        break;

                    case 4:
                     
                        string sqlQuery4 = @"select * 
                                     from TREE_SITES
                                     where lower(SITENAME) like '%' || :encodedFilter || '%' 
                                     or lower(SITECODE) like '%' || :encodedFilter || '%' 
                                     or lower(WITHLEG_NAME) like '%' || :encodedFilter || '%' 
                                     or lower(FIRST_LEG_NAME) like '%' || :encodedFilter || '%' 
                                     or lower(WITHOUTLEG_NAME) like '%' || :encodedFilter || '%' 
                                     or lower(NONSTEEL_NAME) like '%' || :encodedFilter || '%' 
                                     or lower(FIRST_SIDEARM_NAME) like '%' || :encodedFilter || '%' 
                                     or lower(MWDISH_NAME) like '%' || :encodedFilter || '%' 
                                     or lower(MWODU_NAME) like '%' || :encodedFilter || '%' 
                                     or lower(RADIO_RRU_NAME) like '%' || :encodedFilter || '%' 
                                     or lower(RADIO_ANTENNA_NAME) like '%' || :encodedFilter || '%' 
                                     or lower(POWER_NAME) like '%' || :encodedFilter || '%'";
                        using (OracleCommand queryCommand4 = new OracleCommand(sqlQuery4, connection))
                        {
                            queryCommand4.Parameters.Add(new OracleParameter("encodedFilter", encodedFilter.ToLower()));

                            using (OracleDataReader reader4 = queryCommand4.ExecuteReader())
                            {
                                while (reader4.Read())
                                {
                                    dynamic dynamicResult = new ExpandoObject();
                                    var properties = (IDictionary<string, object>)dynamicResult;

                                    for (int i = 0; i < reader4.FieldCount; i++)
                                    {
                                        properties[reader4.GetName(i)] = reader4[i];
                                    }

                                    result.Add(dynamicResult);
                                }
                            }
                        }
                        break;

                    case 5:
                     
                        string sqlQuery5 = @"select * from TREE_SITES where sitecode = :sitecode";
                        using (OracleCommand queryCommand5 = new OracleCommand(sqlQuery5, connection))
                        {
                            queryCommand5.Parameters.Add(new OracleParameter("sitecode", sitecode));

                            using (OracleDataReader reader5 = queryCommand5.ExecuteReader())
                            {
                                while (reader5.Read())
                                {
                                    dynamic dynamicResult = new ExpandoObject();
                                    var properties = (IDictionary<string, object>)dynamicResult;

                                    for (int i = 0; i < reader5.FieldCount; i++)
                                    {
                                        properties[reader5.GetName(i)] = reader5[i];
                                    }

                                    result.Add(dynamicResult);
                                }
                            }
                        }
                        break;
                }

                return result;
            }
        }
        public class GetHistoryRequest
        {
           
            public int First { get; set; }
            public int Rows { get; set; }
            public int SortOrder { get; set; }
            public Dictionary<string, dynamic> Filters { get; set; }
            public List<SortMeta> MultiSortMeta { get; set; }
        }

        public class FilterRequest
        {
            public int? First { get; set; } 
            public int? Rows { get; set; } 
            public int? SortOrder { get; set; } 
            public Dictionary<string, Filter> Filters { get; set; }
            public List<SortMeta> MultiSortMeta { get; set; } 
        }

        public class Filter
        {
            public dynamic Value { get; set; }
            public string MatchMode { get; set; } 
        }

        public class SortMeta
        {
            public string Field { get; set; } 
            public int? Order { get; set; } // ترتيب الفرز
        }
        public class FilterMatchMode
        {
            public const string STARTS_WITH = "startsWith";
            public const string CONTAINS = "contains";
            public const string NOT_CONTAINS = "notContains";
            public const string ENDS_WITH = "endsWith";
            public const string EQUALS = "equals";
            public const string NOT_EQUALS = "notEquals";
            public const string LESS_THAN = "lessThan";
            public const string LESS_THAN_OR_EQUAL_TO = "lessThanOrEqualTo";
            public const string GREATER_THAN = "greaterThan";
            public const string GREATER_THAN_OR_EQUAL_TO = "greaterThanOrEqualTo";
            public const string DATE_IS = "dateIs";
            public const string DATE_IS_NOT = "dateIsNot";
            public const string DATE_BEFORE = "dateBefore";
            public const string DATE_AFTER = "dateAfter";
        }

        // تعريف الفلاتر
        

        public Response<List<dynamic>> GetHistory(string TabelName, string? BaseId, string SiteCode, int? UserId, int? ExternalSysId, string ConnectionString
            , int first, int rows, int sortOrder, Dictionary<string, dynamic> filters, List<SortMeta> multiSortMeta)
        {
            using (var connection = new OracleConnection(ConnectionString))
            {
                connection.Open();

                List<dynamic> result = new List<dynamic>();          
                string sqlQuery = null;
                List<string> filterConditions = new List<string>();

                if (!string.IsNullOrEmpty(TabelName) && !string.IsNullOrEmpty(BaseId) && string.IsNullOrEmpty(SiteCode) && UserId == null && ExternalSysId == null)
                {
                    sqlQuery = @"select * from HISTORY_VIEW where BASE_TABLE = :TabelName AND BASE_RECORD_ID = :BaseId";
                    using (OracleCommand queryCommand5 = new OracleCommand(sqlQuery, connection))
                    {
                        queryCommand5.Parameters.Add(new OracleParameter("TabelName", TabelName));
                   
                        queryCommand5.Parameters.Add(new OracleParameter("BaseId", BaseId));
                    

                        using (OracleDataReader reader5 = queryCommand5.ExecuteReader())
                        {
                            while (reader5.Read())
                            {
                                dynamic dynamicResult = new ExpandoObject();
                                var properties = (IDictionary<string, object>)dynamicResult;

                                for (int i = 0; i < reader5.FieldCount; i++)
                                {
                                    properties[reader5.GetName(i)] = reader5[i];
                                }

                                result.Add(dynamicResult);
                            }
                        }
                    }
                }
                else if (!string.IsNullOrEmpty(TabelName) && TabelName.ToLower() == "tlisite" && !string.IsNullOrEmpty(SiteCode) && BaseId == null && UserId == null && ExternalSysId == null)
                {
                    sqlQuery = @"select * from HISTORY_VIEW where BASE_TABLE = :TabelName AND SITECODE = :SiteCode";
                    using (OracleCommand queryCommand5 = new OracleCommand(sqlQuery, connection))
                    {
                        queryCommand5.Parameters.Add(new OracleParameter("TabelName", TabelName));
                        queryCommand5.Parameters.Add(new OracleParameter("SiteCode", SiteCode));
         
                      

                        using (OracleDataReader reader5 = queryCommand5.ExecuteReader())
                        {
                            while (reader5.Read())
                            {
                                dynamic dynamicResult = new ExpandoObject();
                                var properties = (IDictionary<string, object>)dynamicResult;

                                for (int i = 0; i < reader5.FieldCount; i++)
                                {
                                    properties[reader5.GetName(i)] = reader5[i];
                                }

                                result.Add(dynamicResult);
                            }
                        }
                    }
                }
                else if (UserId != null && string.IsNullOrEmpty(TabelName) && string.IsNullOrEmpty(SiteCode) && BaseId == null && ExternalSysId == null)
                {
                    sqlQuery = @"select * from HISTORY_VIEW where USER_ID = :UserId";
                    using (OracleCommand queryCommand5 = new OracleCommand(sqlQuery, connection))
                    {
                     
                        queryCommand5.Parameters.Add(new OracleParameter("UserId", UserId));

                        using (OracleDataReader reader5 = queryCommand5.ExecuteReader())
                        {
                            while (reader5.Read())
                            {
                                dynamic dynamicResult = new ExpandoObject();
                                var properties = (IDictionary<string, object>)dynamicResult;

                                for (int i = 0; i < reader5.FieldCount; i++)
                                {
                                    properties[reader5.GetName(i)] = reader5[i];
                                }

                                result.Add(dynamicResult);
                            }
                        }
                    }
                }
                else if (ExternalSysId != null && string.IsNullOrEmpty(TabelName) && string.IsNullOrEmpty(SiteCode) && BaseId == null && UserId == null)
                {
                    sqlQuery = @"select * from HISTORY_VIEW where SYS_ID = :ExternalSysId";
                    using (OracleCommand queryCommand5 = new OracleCommand(sqlQuery, connection))
                    {
                        
                        queryCommand5.Parameters.Add(new OracleParameter("ExternalSysId", ExternalSysId));
                    

                        using (OracleDataReader reader5 = queryCommand5.ExecuteReader())
                        {
                            while (reader5.Read())
                            {
                                dynamic dynamicResult = new ExpandoObject();
                                var properties = (IDictionary<string, object>)dynamicResult;

                                for (int i = 0; i < reader5.FieldCount; i++)
                                {
                                    properties[reader5.GetName(i)] = reader5[i];
                                }

                                result.Add(dynamicResult);
                            }
                        }
                    }
                }
                else if (!string.IsNullOrEmpty(TabelName) && TabelName.ToLower() != "tlisite" && !string.IsNullOrEmpty(SiteCode) && UserId == null && ExternalSysId == null && BaseId == null)
                {
                    sqlQuery = @"select * from HISTORY_VIEW where BASE_TABLE = :TabelName AND SITECODE = :SiteCode";
                    using (OracleCommand queryCommand5 = new OracleCommand(sqlQuery, connection))
                    {
                     
                        queryCommand5.Parameters.Add(new OracleParameter("SiteCode", SiteCode));
                        queryCommand5.Parameters.Add(new OracleParameter("TabelName", TabelName));


                        using (OracleDataReader reader5 = queryCommand5.ExecuteReader())
                        {
                            while (reader5.Read())
                            {
                                dynamic dynamicResult = new ExpandoObject();
                                var properties = (IDictionary<string, object>)dynamicResult;

                                for (int i = 0; i < reader5.FieldCount; i++)
                                {
                                    properties[reader5.GetName(i)] = reader5[i];
                                }

                                result.Add(dynamicResult);
                            }
                        }
                    }
                }
                else if (string.IsNullOrEmpty(TabelName) && !string.IsNullOrEmpty(SiteCode) && UserId == null && ExternalSysId == null && BaseId == null)
                {
                    sqlQuery = @"select * from HISTORY_VIEW where SITECODE = :SiteCode";
                    using (OracleCommand queryCommand5 = new OracleCommand(sqlQuery, connection))
                    {

                        queryCommand5.Parameters.Add(new OracleParameter("SiteCode", SiteCode));
                    


                        using (OracleDataReader reader5 = queryCommand5.ExecuteReader())
                        {
                            while (reader5.Read())
                            {
                                dynamic dynamicResult = new ExpandoObject();
                                var properties = (IDictionary<string, object>)dynamicResult;

                                for (int i = 0; i < reader5.FieldCount; i++)
                                {
                                    properties[reader5.GetName(i)] = reader5[i];
                                }

                                result.Add(dynamicResult);
                            }
                        }
                    }
                }
                else if (!string.IsNullOrEmpty(TabelName) && TabelName.ToLower() != "tlisite" && !string.IsNullOrEmpty(SiteCode) && !string.IsNullOrEmpty(BaseId) && UserId == null && ExternalSysId == null)
                {
                    sqlQuery = @"select * from HISTORY_VIEW where BASE_TABLE = :TabelName AND SITECODE = :SiteCode AND BASE_RECORD_ID = :BaseId";
                    using (OracleCommand queryCommand5 = new OracleCommand(sqlQuery, connection))
                    {
                        queryCommand5.Parameters.Add(new OracleParameter("TabelName", TabelName));
                        queryCommand5.Parameters.Add(new OracleParameter("SiteCode", SiteCode));
                        queryCommand5.Parameters.Add(new OracleParameter("BaseId", BaseId));
                       

                        using (OracleDataReader reader5 = queryCommand5.ExecuteReader())
                        {
                            while (reader5.Read())
                            {
                                dynamic dynamicResult = new ExpandoObject();
                                var properties = (IDictionary<string, object>)dynamicResult;

                                for (int i = 0; i < reader5.FieldCount; i++)
                                {
                                    properties[reader5.GetName(i)] = reader5[i];
                                }

                                result.Add(dynamicResult);
                            }
                        }
                    }
                }
                else if (string.IsNullOrEmpty(TabelName) && string.IsNullOrEmpty(SiteCode) && string.IsNullOrEmpty(BaseId) && UserId == null && ExternalSysId == null)
                {
                    var filterMatchModeOptions = new
                    {
                        text = new[]
                        {
            FilterMatchMode.STARTS_WITH,
            FilterMatchMode.CONTAINS,
            FilterMatchMode.NOT_CONTAINS,
            FilterMatchMode.ENDS_WITH,
            FilterMatchMode.EQUALS,
            FilterMatchMode.NOT_EQUALS
        },
                        numeric = new[]
                        {
            FilterMatchMode.EQUALS,
            FilterMatchMode.NOT_EQUALS,
            FilterMatchMode.LESS_THAN,
            FilterMatchMode.LESS_THAN_OR_EQUAL_TO,
            FilterMatchMode.GREATER_THAN,
            FilterMatchMode.GREATER_THAN_OR_EQUAL_TO
        },
                        date = new[]
                        {
            FilterMatchMode.DATE_IS,
            FilterMatchMode.DATE_IS_NOT,
            FilterMatchMode.DATE_BEFORE,
            FilterMatchMode.DATE_AFTER
        }
                    };

                    sqlQuery = @"SELECT * FROM HISTORY_VIEW";

                    if (filters != null)
                    {
                        foreach (var filter in filters)
                        {
                            string field = filter.Key;
                            JsonElement filterValue = (JsonElement)filter.Value;
                            string matchMode = filterValue.GetProperty("matchMode").GetString();
                            JsonElement valueElement = filterValue.GetProperty("value");
                            object value;

                            // تحويل التاريخ إذا كان النوع هو string ويمثل تاريخًا
                            if (valueElement.ValueKind == JsonValueKind.String && DateTime.TryParse(valueElement.GetString(), out DateTime parsedDate))
                            {
                                value = parsedDate.ToString("yyyy-MM-dd"); // تحويل إلى الشكل YYYY-MM-DD
                            }
                            else
                            {
                                switch (valueElement.ValueKind)
                                {
                                    case JsonValueKind.String:
                                        value = valueElement.GetString();
                                        break;
                                    case JsonValueKind.Number:
                                        value = valueElement.GetDouble();
                                        break;
                                    case JsonValueKind.True:
                                    case JsonValueKind.False:
                                        value = valueElement.GetBoolean();
                                        break;
                                    case JsonValueKind.Null:
                                        value = null;
                                        break;
                                    default:
                                        value = null;
                                        break;
                                }
                            }

                            if (value != null)
                            {
                                switch (matchMode)
                                {
                                    case FilterMatchMode.STARTS_WITH:
                                    case FilterMatchMode.CONTAINS:
                                    case FilterMatchMode.NOT_CONTAINS:
                                    case FilterMatchMode.ENDS_WITH:
                                        filterConditions.Add($"\"{field}\" LIKE :{field}");
                                        break;
                                    case FilterMatchMode.EQUALS:
                                        filterConditions.Add($"\"{field}\" = :{field}");
                                        break;
                                    case FilterMatchMode.NOT_EQUALS:
                                        filterConditions.Add($"\"{field}\" <> :{field}");
                                        break;
                                    case FilterMatchMode.LESS_THAN:
                                        filterConditions.Add($"\"{field}\" < :{field}");
                                        break;
                                    case FilterMatchMode.LESS_THAN_OR_EQUAL_TO:
                                        filterConditions.Add($"\"{field}\" <= :{field}");
                                        break;
                                    case FilterMatchMode.GREATER_THAN:
                                        filterConditions.Add($"\"{field}\" > :{field}");
                                        break;
                                    case FilterMatchMode.GREATER_THAN_OR_EQUAL_TO:
                                        filterConditions.Add($"\"{field}\" >= :{field}");
                                        break;
                                    case FilterMatchMode.DATE_IS:
                                        filterConditions.Add($"TRUNC(\"{field}\") = TRUNC(TO_DATE(:{field}, 'YYYY-MM-DD'))");
                                        break;
                                    case FilterMatchMode.DATE_IS_NOT:
                                        filterConditions.Add($"TRUNC(\"{field}\") <> TRUNC(TO_DATE(:{field}, 'YYYY-MM-DD'))");
                                        break;
                                    case FilterMatchMode.DATE_BEFORE:
                                        filterConditions.Add($"TRUNC(\"{field}\") < TRUNC(TO_DATE(:{field}, 'YYYY-MM-DD'))");
                                        break;
                                    case FilterMatchMode.DATE_AFTER:
                                        filterConditions.Add($"TRUNC(\"{field}\") > TRUNC(TO_DATE(:{field}, 'YYYY-MM-DD'))");
                                        break;
                                }
                            }
                        }
                    }

                    // دمج شروط الفلترة في الاستعلام
                    if (filterConditions.Count > 0)
                    {
                        sqlQuery += " WHERE " + string.Join(" AND ", filterConditions);
                    }

                    // حساب العدد فقط بعد تطبيق الفلاتر
                    string countQuery = $"SELECT COUNT(*) FROM HISTORY_VIEW {(filterConditions.Count > 0 ? " WHERE " + string.Join(" AND ", filterConditions) : "")}";

                    int totalCount = 0;
                    using (OracleCommand countCommand = new OracleCommand(countQuery, connection))
                    {
                        // إضافة معلمات الفلترة
                        if (filters != null)
                        {
                            foreach (var filter in filters)
                            {
                                string field = filter.Key;
                                JsonElement filterValue = (JsonElement)filter.Value;
                                string matchMode = filterValue.GetProperty("matchMode").GetString();
                                JsonElement valueElement = filterValue.GetProperty("value");
                                object value;

                                // تحويل التاريخ إذا كان النوع هو string ويمثل تاريخًا
                                if (valueElement.ValueKind == JsonValueKind.String && DateTime.TryParse(valueElement.GetString(), out DateTime parsedDate))
                                {
                                    value = parsedDate.ToString("yyyy-MM-dd"); // تحويل إلى الشكل YYYY-MM-DD
                                }
                                else
                                {
                                    switch (valueElement.ValueKind)
                                    {
                                        case JsonValueKind.String:
                                            value = valueElement.GetString();
                                            break;
                                        case JsonValueKind.Number:
                                            value = valueElement.GetDouble();
                                            break;
                                        case JsonValueKind.True:
                                        case JsonValueKind.False:
                                            value = valueElement.GetBoolean();
                                            break;
                                        case JsonValueKind.Null:
                                            value = null;
                                            break;
                                        default:
                                            value = null;
                                            break;
                                    }
                                }

                                // إضافة المعاملات لاستعلام العدد
                                if (value != null)
                                {
                                    countCommand.Parameters.Add(new OracleParameter(field, matchMode == FilterMatchMode.CONTAINS ? "%" + value + "%" : value));
                                }
                            }
                        }

                        totalCount = Convert.ToInt32(countCommand.ExecuteScalar());
                    }

                    // بعد حساب العدد، إضافة شروط الترتيب والصفحات للاستعلام الأساسي
                    if (multiSortMeta != null && multiSortMeta.Count > 0)
                    {
                        List<string> sortConditions = new List<string>();
                        foreach (var sort in multiSortMeta)
                        {
                            sortConditions.Add($"\"{sort.Field}\" {(sort.Order == 1 ? "ASC" : "DESC")}");
                        }
                        sqlQuery += " ORDER BY " + string.Join(", ", sortConditions);
                    }

                    // إضافة الصفحات
                    sqlQuery += $" OFFSET {first} ROWS FETCH NEXT {rows} ROWS ONLY";

                    // تنفيذ الاستعلام الأساسي لجلب البيانات
                    using (OracleCommand queryCommand = new OracleCommand(sqlQuery, connection))
                    {
                        if (!string.IsNullOrEmpty(SiteCode))
                        {
                            queryCommand.Parameters.Add(new OracleParameter("SiteCode", SiteCode));
                        }

                        // إضافة معلمات الفلترة
                        if (filters != null)
                        {
                            foreach (var filter in filters)
                            {
                                string field = filter.Key;
                                JsonElement filterValue = (JsonElement)filter.Value;
                                string matchMode = filterValue.GetProperty("matchMode").GetString();
                                JsonElement valueElement;

                                if (filterValue.TryGetProperty("value", out valueElement))
                                {
                                    object value;

                                    // تحويل التاريخ إذا كان النوع هو string ويمثل تاريخًا
                                    if (valueElement.ValueKind == JsonValueKind.String && DateTime.TryParse(valueElement.GetString(), out DateTime parsedDate))
                                    {
                                        value = parsedDate.ToString("yyyy-MM-dd"); // تحويل إلى الشكل YYYY-MM-DD
                                    }
                                    else
                                    {
                                        switch (valueElement.ValueKind)
                                        {
                                            case JsonValueKind.String:
                                                value = valueElement.GetString();
                                                break;
                                            case JsonValueKind.Number:
                                                value = valueElement.GetDouble();
                                                break;
                                            case JsonValueKind.True:
                                            case JsonValueKind.False:
                                                value = valueElement.GetBoolean();
                                                break;
                                            case JsonValueKind.Null:
                                                value = null;
                                                break;
                                            default:
                                                value = null;
                                                break;
                                        }
                                    }

                                    if (value != null)
                                    {
                                        queryCommand.Parameters.Add(new OracleParameter(field, matchMode == FilterMatchMode.CONTAINS ? "%" + value + "%" : value));
                                    }
                                }
                            }
                        }

                        using (OracleDataReader reader = queryCommand.ExecuteReader())
                        {
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
                        }
                    }

                    return new Response<List<dynamic>>(true, result, null, null, (int)Helpers.Constants.ApiReturnCode.success, totalCount);
                }

                return new Response<List<dynamic>>(true, result, null, null, (int)Helpers.Constants.ApiReturnCode.success);

            }
        }



        public Response<SiteInfo> GetSiteInfo(string SiteCode)
        {
            var SiteInfo = _context.TLIsite.Include(x=>x.Area).Include(x=>x.Region).FirstOrDefault(x => x.SiteCode == SiteCode);
            if (SiteInfo != null)
            {
                SiteInfo site = new SiteInfo
                {
                    CityName = SiteInfo.Zone,
                    RegionCode = SiteInfo.RegionCode,
                    SubArea = SiteInfo.Area.AreaName
                };
                return new Response<SiteInfo>(true, site, null, null, (int)Helpers.Constants.ApiReturnCode.success);
            }
            else
            {
                return new Response<SiteInfo>(false, null, null, "This SiteCode Is Not Found", (int)Helpers.Constants.ApiReturnCode.fail);
            }
        }
        public Response<List<RecalculatSpaceOnSite>> RecalculateSite()
        {
            try
            {
                List<RecalculatSpaceOnSite> RecalculatSpaceOnSites = new List<RecalculatSpaceOnSite>();
                var SiteInfo = _context.TLIsite.ToList();
                foreach (var item in SiteInfo)
                {
                    if (item.RentedSpace == 0)
                    {
                        RecalculatSpaceOnSite recalculat = new RecalculatSpaceOnSite()
                        {
                            AttributeName = "RentedSpace",
                            ItemOnSiteType = "Site",
                            ItemOnSiteName = " ",
                            SiteName = item.SiteName,
                            Type = "Site",
                           
                        };
                        RecalculatSpaceOnSites.Add(recalculat);
                    }
                    item.ReservedSpace = 0;
                    List<TLIcivilSiteDate> CivilOnSite = _unitOfWork.CivilSiteDateRepository.GetWhereAndInclude
                    (x => x.SiteCode.ToLower() == item.SiteCode.ToLower() && !x.Dismantle && x.ReservedSpace
                    , x => x.allCivilInst, x => x.allCivilInst.civilNonSteel, x => x.allCivilInst.civilWithLegs,
                    x => x.allCivilInst.civilWithoutLeg).ToList();
                    foreach (var CivilOnSiteitem in CivilOnSite)
                    {
                        if (CivilOnSiteitem.allCivilInst.civilWithLegsId != null)
                        {
                            if (CivilOnSiteitem.allCivilInst.civilWithLegs.SpaceInstallation == 0)
                            {
                                RecalculatSpaceOnSite recalculat = new RecalculatSpaceOnSite()
                                {
                                    AttributeName = "SpaceInstallation",
                                    ItemOnSiteType = "civilWithLegs",
                                    ItemOnSiteName = CivilOnSiteitem.allCivilInst.civilWithLegs.Name,
                                    SiteName= item.SiteName,
                                    Type = "Installation",
                                    ReservedSpaceInCivil = CivilOnSiteitem.ReservedSpace
                                };
                                RecalculatSpaceOnSites.Add(recalculat);
                            }
                            else
                            {
                                item.ReservedSpace = item.ReservedSpace + CivilOnSiteitem.allCivilInst.civilWithLegs.SpaceInstallation;
                            }
                        }
                        else if (CivilOnSiteitem.allCivilInst.civilWithoutLegId != null)
                        {
                            if (CivilOnSiteitem.allCivilInst.civilWithoutLeg.SpaceInstallation == 0)
                            {
                                RecalculatSpaceOnSite recalculat = new RecalculatSpaceOnSite()
                                {
                                    AttributeName = "SpaceInstallation",
                                    ItemOnSiteType = _context.MV_CIVIL_WITHOUTLEGS_VIEW.FirstOrDefault(x=>x.Id== CivilOnSiteitem.allCivilInst.civilWithoutLeg.Id)?.CIVILWITHOUTLEGCATEGORY,
                                    ItemOnSiteName = CivilOnSiteitem.allCivilInst.civilWithoutLeg.Name,
                                    SiteName = item.SiteName,
                                    Type = "Installation",
                                    ReservedSpaceInCivil = CivilOnSiteitem.ReservedSpace
                                };
                                RecalculatSpaceOnSites.Add(recalculat);
                            }
                            else
                            {
                                item.ReservedSpace = item.ReservedSpace + CivilOnSiteitem.allCivilInst.civilWithoutLeg.SpaceInstallation;
                            }
                        }
                        else if (CivilOnSiteitem.allCivilInst.civilNonSteelId != null)
                        {
                            if (CivilOnSiteitem.allCivilInst.civilNonSteel.SpaceInstallation == 0)
                            {
                                RecalculatSpaceOnSite recalculat = new RecalculatSpaceOnSite()
                                {
                                    AttributeName = "SpaceInstallation",
                                    ItemOnSiteType = "civilNonSteel",
                                    ItemOnSiteName = CivilOnSiteitem.allCivilInst.civilNonSteel.Name,
                                    SiteName = item.SiteName,
                                    Type = "Installation",
                                    ReservedSpaceInCivil = CivilOnSiteitem.ReservedSpace
                                };
                                RecalculatSpaceOnSites.Add(recalculat);
                            }
                            else
                            {
                                item.ReservedSpace = item.ReservedSpace + CivilOnSiteitem.allCivilInst.civilNonSteel.SpaceInstallation;
                            }
                        }
                    }
                    List<TLIotherInSite> OtherOnSite = _unitOfWork.OtherInSiteRepository
                    .GetWhereAndInclude(x => x.SiteCode.ToLower() == item.SiteCode.ToLower()
                    && !x.Dismantle && x.ReservedSpace == true, x => x.allOtherInventoryInst, x => x.allOtherInventoryInst.cabinet
                    , x => x.allOtherInventoryInst.solar, x => x.allOtherInventoryInst.generator).ToList();
                    foreach (var OtherOnSiteitem in OtherOnSite)
                    {
                        if (OtherOnSiteitem.allOtherInventoryInst.generatorId != null)
                        {
                            if (OtherOnSiteitem.allOtherInventoryInst.generator.SpaceInstallation == 0)
                            {
                                RecalculatSpaceOnSite recalculat = new RecalculatSpaceOnSite()
                                {
                                    AttributeName = "SpaceInstallation",
                                    ItemOnSiteType = "generator",
                                    ItemOnSiteName = OtherOnSiteitem.allOtherInventoryInst.generator.Name,
                                    SiteName = item.SiteName,
                                    Type = "Installation",
                                    ReservedSpaceInCivil = OtherOnSiteitem.ReservedSpace
                                };
                                RecalculatSpaceOnSites.Add(recalculat);
                            }
                            else
                            {
                                item.ReservedSpace = item.ReservedSpace + OtherOnSiteitem.allOtherInventoryInst.generator.SpaceInstallation;
                            }
                        }
                        else if (OtherOnSiteitem.allOtherInventoryInst.cabinetId != null)
                        {
                            if (OtherOnSiteitem.allOtherInventoryInst.cabinet.SpaceInstallation == 0)
                            {
                                RecalculatSpaceOnSite recalculat = new RecalculatSpaceOnSite()
                                {
                                    AttributeName = "SpaceInstallation",
                                    ItemOnSiteType = "cabinet",
                                    ItemOnSiteName = OtherOnSiteitem.allOtherInventoryInst.cabinet.Name,
                                    SiteName = item.SiteName,
                                    Type = "Installation",
                                    ReservedSpaceInCivil = OtherOnSiteitem.ReservedSpace
                                };
                                RecalculatSpaceOnSites.Add(recalculat);
                            }
                            else
                            {
                                item.ReservedSpace = item.ReservedSpace + OtherOnSiteitem.allOtherInventoryInst.cabinet.SpaceInstallation;
                            }
                        }
                        else if (OtherOnSiteitem.allOtherInventoryInst.solarId != null)
                        {
                            if (OtherOnSiteitem.allOtherInventoryInst.solar.SpaceInstallation == 0)
                            {
                                RecalculatSpaceOnSite recalculat = new RecalculatSpaceOnSite()
                                {
                                    AttributeName = "SpaceInstallation",
                                    ItemOnSiteType = "solar",
                                    ItemOnSiteName = OtherOnSiteitem.allOtherInventoryInst.solar.Name,
                                    SiteName = item.SiteName,
                                    Type = "Installation",
                                    ReservedSpaceInCivil = OtherOnSiteitem.ReservedSpace
                                };
                                RecalculatSpaceOnSites.Add(recalculat);
                            }
                            else
                            {
                                item.ReservedSpace = item.ReservedSpace + OtherOnSiteitem.allOtherInventoryInst.solar.SpaceInstallation;
                            }
                        }
                    }
                    _unitOfWork.SiteRepository.Update(item);
                    _unitOfWork.SaveChanges();

                }

                return new Response<List<RecalculatSpaceOnSite>>(true, RecalculatSpaceOnSites, null, null, (int)Helpers.Constants.ApiReturnCode.success);
            }
            catch (Exception er)
            {

                return new Response<List<RecalculatSpaceOnSite>>(true, null, null, er.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
            
        }
        public Response<string> GetConfigurationTables(string SiteCode, string TableNameInstallation, int? CategoryId, string ConnectionString)
        {
            using (var connection = new OracleConnection(ConnectionString))
            {
               
                if (Helpers.Constants.CivilType.TLIcivilWithLegs.ToString() == TableNameInstallation)
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
                        var attActivated = _context.TLIattributeViewManagment
                            .Include(x => x.EditableManagmentView)
                            .Include(x => x.AttributeActivated)
                            .Include(x => x.DynamicAtt)
                            .Where(x => x.Enable && x.EditableManagmentView.View == "CivilWithLegInstallation" &&
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
                                var query = _context.MV_CIVIL_WITHLEGS_VIEW.Where(x =>
                                !x.Dismantle).AsEnumerable()
                                .Select(item => _unitOfWork.CivilWithLegsRepository.BuildDynamicSelect(item, null, propertyNamesStatic, propertyNamesDynamic));
                                int count = query.Count();
                                getEnableAttribute.Model = query;
                                string excelFilePath = ExportToExcel(query, "TLIcivilWithLegs");

                                return new Response<string>(false, excelFilePath, null, null, (int)Helpers.Constants.ApiReturnCode.success);
                            }
                            else
                            {
                                var query = _context.MV_CIVIL_WITHLEGS_VIEW.Where(x =>
                                  !x.Dismantle).AsEnumerable()
                            .GroupBy(x => new
                            {

                                Id = x.Id,
                                Name = x.Name,
                                SITECODE = x.SITECODE,
                                WindMaxLoadm2 = x.WindMaxLoadm2,
                                LocationHeight = x.LocationHeight,
                                PoType = x.PoType,
                                PoNo = x.PoNo,
                                PoDate = x.PoDate,
                                HeightImplemented = x.HeightImplemented,
                                BuildingMaxLoad = x.BuildingMaxLoad,
                                SupportMaxLoadAfterInforcement = x.SupportMaxLoadAfterInforcement,
                                CurrentLoads = x.CurrentLoads,
                                warningpercentageloads = x.warningpercentageloads,
                                VisiableStatus = x.VisiableStatus,
                                VerticalMeasured = x.VerticalMeasured,
                                OtherBaseType = x.OtherBaseType,
                                IsEnforeced = x.IsEnforeced,
                                H2height = x.H2height,
                                HeightBase = x.HeightBase,
                                DimensionsLeg = x.DimensionsLeg,
                                DiagonalMemberSection = x.DiagonalMemberSection,
                                DiagonalMemberDimensions = x.DiagonalMemberDimensions,
                                BoltHoles = x.BoltHoles,
                                BasePlatethickness = x.BasePlatethickness,
                                BasePlateShape = x.BasePlateShape,
                                BasePlateDimensions = x.BasePlateDimensions,
                                BaseNote = x.BaseNote,
                                LOCATIONTYPE = x.LOCATIONTYPE,
                                BASETYPE = x.BASETYPE,
                                VerticalMeasurement = x.VerticalMeasurement,
                                SteelCrossSection = x.SteelCrossSection,
                                DiagonalMemberPrefix = x.DiagonalMemberPrefix,
                                EnforcementHeightBase = x.EnforcementHeightBase,
                                Enforcementlevel = x.Enforcementlevel,
                                StructureType = x.StructureType,
                                SectionsLegType = x.SectionsLegType,
                                TotalHeight = x.TotalHeight,
                                SpaceInstallation = x.SpaceInstallation,
                                OWNER = x.OWNER,
                                CIVILWITHLEGSLIB = x.CIVILWITHLEGSLIB,
                                GUYLINETYPE = x.GUYLINETYPE,
                                CIVILWITHLEGSTYPE = x.BASECIVILWITHLEGTYPE,
                                SUPPORTTYPEIMPLEMENTED = x.SUPPORTTYPEIMPLEMENTED,
                                BaseCivilWithLegType = x.BASECIVILWITHLEGTYPE,
                                Support_Limited_Load = x.Support_Limited_Load,
                                ENFORCMENTCATEGORY = x.ENFORCMENTCATEGORY,
                                Remark = x.Remark,
                                Dismantle = x.Dismantle,
                            }).OrderBy(x => x.Key.Name)
                            .Select(x => new { key = x.Key, value = x.ToDictionary(z => z.Key, z => z.INPUTVALUE) })
                            .Select(item => _unitOfWork.CivilWithLegsRepository.BuildDynamicSelect(item.key, item.value, propertyNamesStatic, propertyNamesDynamic));
                                int count = query.Count();
                                getEnableAttribute.Model = query;
                                string excelFilePath = ExportToExcel(query, "TLIcivilWithLegs");

                                return new Response<string>(false, excelFilePath, null, null, (int)Helpers.Constants.ApiReturnCode.success);
                            }
                        }
                        if (propertyNamesDynamic.Count == 0)
                        {
                            var query = _context.MV_CIVIL_WITHLEGS_VIEW.Where(x => x.SITECODE.ToLower() == SiteCode.ToLower()
                            && !x.Dismantle).AsEnumerable()
                        .Select(item => _unitOfWork.CivilWithLegsRepository.BuildDynamicSelect(item, null, propertyNamesStatic, propertyNamesDynamic));
                            int count = query.Count();

                            getEnableAttribute.Model = query;

                            string excelFilePath = ExportToExcel(query, "TLIcivilWithLegs");

                            return new Response<string>(false, excelFilePath, null, null, (int)Helpers.Constants.ApiReturnCode.success);
                        }
                        else
                        {
                            var query = _context.MV_CIVIL_WITHLEGS_VIEW.Where(x => x.SITECODE.ToLower() == SiteCode.ToLower()
                             && !x.Dismantle).AsEnumerable()
                        .GroupBy(x => new
                        {

                            Id = x.Id,
                            Name = x.Name,
                            SITECODE = x.SITECODE,
                            WindMaxLoadm2 = x.WindMaxLoadm2,
                            LocationHeight = x.LocationHeight,
                            PoType = x.PoType,
                            PoNo = x.PoNo,
                            PoDate = x.PoDate,
                            HeightImplemented = x.HeightImplemented,
                            BuildingMaxLoad = x.BuildingMaxLoad,
                            SupportMaxLoadAfterInforcement = x.SupportMaxLoadAfterInforcement,
                            CurrentLoads = x.CurrentLoads,
                            warningpercentageloads = x.warningpercentageloads,
                            VisiableStatus = x.VisiableStatus,
                            VerticalMeasured = x.VerticalMeasured,
                            OtherBaseType = x.OtherBaseType,
                            IsEnforeced = x.IsEnforeced,
                            H2height = x.H2height,
                            HeightBase = x.HeightBase,
                            DimensionsLeg = x.DimensionsLeg,
                            DiagonalMemberSection = x.DiagonalMemberSection,
                            DiagonalMemberDimensions = x.DiagonalMemberDimensions,
                            BoltHoles = x.BoltHoles,
                            BasePlatethickness = x.BasePlatethickness,
                            BasePlateShape = x.BasePlateShape,
                            BasePlateDimensions = x.BasePlateDimensions,
                            BaseNote = x.BaseNote,
                            LOCATIONTYPE = x.LOCATIONTYPE,
                            BASETYPE = x.BASETYPE,
                            VerticalMeasurement = x.VerticalMeasurement,
                            SteelCrossSection = x.SteelCrossSection,
                            DiagonalMemberPrefix = x.DiagonalMemberPrefix,
                            EnforcementHeightBase = x.EnforcementHeightBase,
                            Enforcementlevel = x.Enforcementlevel,
                            StructureType = x.StructureType,
                            SectionsLegType = x.SectionsLegType,
                            TotalHeight = x.TotalHeight,
                            SpaceInstallation = x.SpaceInstallation,
                            OWNER = x.OWNER,
                            CIVILWITHLEGSLIB = x.CIVILWITHLEGSLIB,
                            GUYLINETYPE = x.GUYLINETYPE,
                            CIVILWITHLEGSTYPE = x.BASECIVILWITHLEGTYPE,
                            SUPPORTTYPEIMPLEMENTED = x.SUPPORTTYPEIMPLEMENTED,
                            BaseCivilWithLegType = x.BASECIVILWITHLEGTYPE,
                            Support_Limited_Load = x.Support_Limited_Load,
                            ENFORCMENTCATEGORY = x.ENFORCMENTCATEGORY,
                            Remark = x.Remark,
                            Dismantle = x.Dismantle,
                        }).OrderBy(x => x.Key.Name)
                        .Select(x => new { key = x.Key, value = x.ToDictionary(z => z.Key, z => z.INPUTVALUE) })
                        .Select(item => _unitOfWork.CivilWithLegsRepository.BuildDynamicSelect(item.key, item.value, propertyNamesStatic, propertyNamesDynamic));
                            int count = query.Count();
                            getEnableAttribute.Model = query;
                            string excelFilePath = ExportToExcel(query, "TLIcivilWithLegs");

                            return new Response<string>(false, excelFilePath, null, null, (int)Helpers.Constants.ApiReturnCode.success);

                        }

                    }
                    catch (Exception err)
                    {
                        return new Response<string>(false, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
                    }
                }
                else if (Helpers.Constants.CivilType.TLIcivilWithoutLeg.ToString() == TableNameInstallation && CategoryId == 1)
                {
                    try
                    {

                        GetEnableAttribute getEnableAttribute = new GetEnableAttribute();
                        connection.Open();
                        //string storedProcedureName = "CREATE_DYNAMIC_PIVOT_WITHOUTLEG";
                        //using (OracleCommand procedureCommand = new OracleCommand(storedProcedureName, connection))
                        //{
                        //    procedureCommand.CommandType = CommandType.StoredProcedure;
                        //    procedureCommand.ExecuteNonQuery();
                        //}
                        var attActivated = _context.TLIattributeViewManagment
                            .Include(x => x.EditableManagmentView)
                            .Include(x => x.AttributeActivated)
                            .Include(x => x.DynamicAtt)
                            .Where(x => x.Enable && x.EditableManagmentView.View == "CivilWithoutLegInstallationMast" &&
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

                                var query = _context.MV_CIVIL_WITHOUTLEGS_VIEW
                                .Where(x =>
                                 x.CIVILWITHOUTLEGCATEGORY.ToLower() == "mast" && !x.Dismantle)
                                .AsEnumerable().Select(x =>

                                     new
                                     {
                                         HieghFromLand = x.HieghFromLand,
                                         EquivalentSpace = x.EquivalentSpace,
                                         Support_Limited_Load = x.Support_Limited_Load,
                                         SITECODE = x.SITECODE,
                                         Id = x.Id,
                                         Name = x.Name,
                                         UpperPartLengthm = x.UpperPartLengthm,
                                         UpperPartDiameterm = x.UpperPartDiameterm,
                                         SpindlesBasePlateLengthcm = x.SpindlesBasePlateLengthcm,
                                         SpindlesBasePlateWidthcm = x.SpindlesBasePlateWidthcm,
                                         ConcreteBaseWidthm = x.ConcreteBaseWidthm,
                                         SpinBasePlateAnchorDiametercm = x.SpinBasePlateAnchorDiametercm,
                                         NumberOfCivilParts = x.NumberOfCivilParts,
                                         NumberOfLongitudinalSpindles = x.NumberOfLongitudinalSpindles,
                                         NumberOfhorizontalSpindle = x.NumberOfhorizontalSpindle,
                                         CivilLengthAboveEndOfSpindles = x.CivilLengthAboveEndOfSpindles,
                                         CivilBaseLevelFromGround = x.CivilBaseLevelFromGround,
                                         LongitudinalSpinDiameterrmm = x.LongitudinalSpinDiameterrmm,
                                         HorizontalSpindlesHBAm = x.HorizontalSpindlesHBAm,
                                         HorizontalSpindleDiametermm = x.HorizontalSpindleDiametermm,
                                         FlangeThicknesscm = x.FlangeThicknesscm,
                                         FlangeDiametercm = x.FlangeDiametercm,
                                         FlangeBoltsDiametermm = x.FlangeBoltsDiametermm,
                                         ConcreteBaseThicknessm = x.ConcreteBaseThicknessm,
                                         ConcreteBaseLengthm = x.ConcreteBaseLengthm,
                                         Civil_Remarks = x.Civil_Remarks,
                                         BottomPartLengthm = x.BottomPartLengthm,
                                         BottomPartDiameterm = x.BottomPartDiameterm,
                                         BasePlateWidthcm = x.BasePlateWidthcm,
                                         BasePlateThicknesscm = x.BasePlateThicknesscm,
                                         BasePlateLengthcm = x.BasePlateLengthcm,
                                         BPlateBoltsAnchorDiametermm = x.BPlateBoltsAnchorDiametermm,
                                         BaseBeamSectionmm = x.BaseBeamSectionmm,
                                         WindMaxLoadm2 = x.WindMaxLoadm2,
                                         Location_Height = x.Location_Height,
                                         PoType = x.PoType,
                                         PoNo = x.PoNo,
                                         PoDate = x.PoDate,
                                         HeightImplemented = x.HeightImplemented,
                                         BuildingMaxLoad = x.BuildingMaxLoad,
                                         SupportMaxLoadAfterInforcement = x.SupportMaxLoadAfterInforcement,
                                         CurrentLoads = x.CurrentLoads,
                                         WarningPercentageLoads = x.WarningPercentageLoads,
                                         Visiable_Status = x.Visiable_Status,
                                         SpaceInstallation = x.SpaceInstallation,
                                         CIVILWITHOUTLEGSLIB = x.CIVILWITHOUTLEGSLIB,
                                         OWNER = x.OWNER,
                                         SUBTYPE = x.SUBTYPE,
                                         HeightBase = x.HeightBase,
                                         BuildingHeightH3 = x.BuildingHeightH3,
                                         reinforced = x.reinforced,
                                         ladderSteps = x.ladderSteps,
                                         availabilityOfWorkPlatforms = x.availabilityOfWorkPlatforms,
                                         equipmentsLocation = x.equipmentsLocation,
                                         CenterHigh = x.CenterHigh,
                                         HBA = x.HBA,
                                         CIVILWITHOUTLEGCATEGORY = x.CIVILWITHOUTLEGCATEGORY,
                                         Dismantle = x.Dismantle

                                     }).Distinct().Select(item => _unitOfWork.CivilWithLegsRepository.BuildDynamicSelect(item, null, propertyNamesStatic, propertyNamesDynamic))
                              ;
                                int count = query.Count();

                                getEnableAttribute.Model = query;
                                string excelFilePath = ExportToExcel(query, "TLIcivilWithoutLegMast");
                                
                                return new Response<string>(false, excelFilePath, null, null, (int)Helpers.Constants.ApiReturnCode.success);
                            }
                            else
                            {
                                var query = _context.MV_CIVIL_WITHOUTLEGS_VIEW.Where(x =>
                                 x.CIVILWITHOUTLEGCATEGORY.ToLower() == "mast" && !x.Dismantle).Select(x =>

                                     new
                                     {
                                         HieghFromLand = x.HieghFromLand,
                                         EquivalentSpace = x.EquivalentSpace,
                                         Support_Limited_Load = x.Support_Limited_Load,
                                         SITECODE = x.SITECODE,
                                         Id = x.Id,
                                         Name = x.Name,
                                         UpperPartLengthm = x.UpperPartLengthm,
                                         UpperPartDiameterm = x.UpperPartDiameterm,
                                         SpindlesBasePlateLengthcm = x.SpindlesBasePlateLengthcm,
                                         SpindlesBasePlateWidthcm = x.SpindlesBasePlateWidthcm,
                                         ConcreteBaseWidthm = x.ConcreteBaseWidthm,
                                         SpinBasePlateAnchorDiametercm = x.SpinBasePlateAnchorDiametercm,
                                         NumberOfCivilParts = x.NumberOfCivilParts,
                                         NumberOfLongitudinalSpindles = x.NumberOfLongitudinalSpindles,
                                         NumberOfhorizontalSpindle = x.NumberOfhorizontalSpindle,
                                         CivilLengthAboveEndOfSpindles = x.CivilLengthAboveEndOfSpindles,
                                         CivilBaseLevelFromGround = x.CivilBaseLevelFromGround,
                                         LongitudinalSpinDiameterrmm = x.LongitudinalSpinDiameterrmm,
                                         HorizontalSpindlesHBAm = x.HorizontalSpindlesHBAm,
                                         HorizontalSpindleDiametermm = x.HorizontalSpindleDiametermm,
                                         FlangeThicknesscm = x.FlangeThicknesscm,
                                         FlangeDiametercm = x.FlangeDiametercm,
                                         FlangeBoltsDiametermm = x.FlangeBoltsDiametermm,
                                         ConcreteBaseThicknessm = x.ConcreteBaseThicknessm,
                                         ConcreteBaseLengthm = x.ConcreteBaseLengthm,
                                         Civil_Remarks = x.Civil_Remarks,
                                         BottomPartLengthm = x.BottomPartLengthm,
                                         BottomPartDiameterm = x.BottomPartDiameterm,
                                         BasePlateWidthcm = x.BasePlateWidthcm,
                                         BasePlateThicknesscm = x.BasePlateThicknesscm,
                                         BasePlateLengthcm = x.BasePlateLengthcm,
                                         BPlateBoltsAnchorDiametermm = x.BPlateBoltsAnchorDiametermm,
                                         BaseBeamSectionmm = x.BaseBeamSectionmm,
                                         WindMaxLoadm2 = x.WindMaxLoadm2,
                                         Location_Height = x.Location_Height,
                                         PoType = x.PoType,
                                         PoNo = x.PoNo,
                                         PoDate = x.PoDate,
                                         HeightImplemented = x.HeightImplemented,
                                         BuildingMaxLoad = x.BuildingMaxLoad,
                                         SupportMaxLoadAfterInforcement = x.SupportMaxLoadAfterInforcement,
                                         CurrentLoads = x.CurrentLoads,
                                         WarningPercentageLoads = x.WarningPercentageLoads,
                                         Visiable_Status = x.Visiable_Status,
                                         SpaceInstallation = x.SpaceInstallation,
                                         CIVILWITHOUTLEGSLIB = x.CIVILWITHOUTLEGSLIB,
                                         OWNER = x.OWNER,
                                         SUBTYPE = x.SUBTYPE,
                                         HeightBase = x.HeightBase,
                                         BuildingHeightH3 = x.BuildingHeightH3,
                                         reinforced = x.reinforced,
                                         ladderSteps = x.ladderSteps,
                                         availabilityOfWorkPlatforms = x.availabilityOfWorkPlatforms,
                                         equipmentsLocation = x.equipmentsLocation,
                                         CenterHigh = x.CenterHigh,
                                         HBA = x.HBA,
                                         INPUTVALUE = x.INPUTVALUE,
                                         Key = x.Key,
                                         CIVILWITHOUTLEGCATEGORY = x.CIVILWITHOUTLEGCATEGORY,
                                         Dismantle = x.Dismantle

                                     }).AsEnumerable()
                            .GroupBy(x => new
                            {
                                HieghFromLand = x.HieghFromLand,
                                EquivalentSpace = x.EquivalentSpace,
                                Support_Limited_Load = x.Support_Limited_Load,
                                SITECODE = x.SITECODE,
                                Id = x.Id,
                                Name = x.Name,
                                UpperPartLengthm = x.UpperPartLengthm,
                                UpperPartDiameterm = x.UpperPartDiameterm,
                                SpindlesBasePlateLengthcm = x.SpindlesBasePlateLengthcm,
                                SpindlesBasePlateWidthcm = x.SpindlesBasePlateWidthcm,
                                ConcreteBaseWidthm = x.ConcreteBaseWidthm,
                                SpinBasePlateAnchorDiametercm = x.SpinBasePlateAnchorDiametercm,
                                NumberOfCivilParts = x.NumberOfCivilParts,
                                NumberOfLongitudinalSpindles = x.NumberOfLongitudinalSpindles,
                                NumberOfhorizontalSpindle = x.NumberOfhorizontalSpindle,
                                CivilLengthAboveEndOfSpindles = x.CivilLengthAboveEndOfSpindles,
                                CivilBaseLevelFromGround = x.CivilBaseLevelFromGround,
                                LongitudinalSpinDiameterrmm = x.LongitudinalSpinDiameterrmm,
                                HorizontalSpindlesHBAm = x.HorizontalSpindlesHBAm,
                                HorizontalSpindleDiametermm = x.HorizontalSpindleDiametermm,
                                FlangeThicknesscm = x.FlangeThicknesscm,
                                FlangeDiametercm = x.FlangeDiametercm,
                                FlangeBoltsDiametermm = x.FlangeBoltsDiametermm,
                                ConcreteBaseThicknessm = x.ConcreteBaseThicknessm,
                                ConcreteBaseLengthm = x.ConcreteBaseLengthm,
                                Civil_Remarks = x.Civil_Remarks,
                                BottomPartLengthm = x.BottomPartLengthm,
                                BottomPartDiameterm = x.BottomPartDiameterm,
                                BasePlateWidthcm = x.BasePlateWidthcm,
                                BasePlateThicknesscm = x.BasePlateThicknesscm,
                                BasePlateLengthcm = x.BasePlateLengthcm,
                                BPlateBoltsAnchorDiametermm = x.BPlateBoltsAnchorDiametermm,
                                BaseBeamSectionmm = x.BaseBeamSectionmm,
                                WindMaxLoadm2 = x.WindMaxLoadm2,
                                Location_Height = x.Location_Height,
                                PoType = x.PoType,
                                PoNo = x.PoNo,
                                PoDate = x.PoDate,
                                HeightImplemented = x.HeightImplemented,
                                BuildingMaxLoad = x.BuildingMaxLoad,
                                SupportMaxLoadAfterInforcement = x.SupportMaxLoadAfterInforcement,
                                CurrentLoads = x.CurrentLoads,
                                WarningPercentageLoads = x.WarningPercentageLoads,
                                Visiable_Status = x.Visiable_Status,
                                SpaceInstallation = x.SpaceInstallation,
                                CIVILWITHOUTLEGSLIB = x.CIVILWITHOUTLEGSLIB,
                                OWNER = x.OWNER,
                                SUBTYPE = x.SUBTYPE,
                                HeightBase = x.HeightBase,
                                BuildingHeightH3 = x.BuildingHeightH3,
                                reinforced = x.reinforced,
                                ladderSteps = x.ladderSteps,
                                availabilityOfWorkPlatforms = x.availabilityOfWorkPlatforms,
                                equipmentsLocation = x.equipmentsLocation,
                                CenterHigh = x.CenterHigh,
                                HBA = x.HBA,
                                CIVILWITHOUTLEGCATEGORY = x.CIVILWITHOUTLEGCATEGORY,
                                Dismantle = x.Dismantle
                            }).OrderBy(x => x.Key.Name)
                            .Select(x =>
                                new { key = x.Key, value = x.ToDictionary(z => z.Key, z => z.INPUTVALUE) }
                            )
                            .Select(item => _unitOfWork.CivilWithLegsRepository.BuildDynamicSelect(item.key, item.value, propertyNamesStatic, propertyNamesDynamic));
                                int count = query.Count();

                                getEnableAttribute.Model = query;
                                string excelFilePath = ExportToExcel(query, "TLIcivilWithoutLegMast");

                                return new Response<string>(false, excelFilePath, null, null, (int)Helpers.Constants.ApiReturnCode.success);
                            }
                        }
                        if (propertyNamesDynamic.Count == 0)
                        {

                            var query = _context.MV_CIVIL_WITHOUTLEGS_VIEW
                            .Where(x => x.SITECODE.ToLower() == SiteCode.ToLower() &&
                             x.CIVILWITHOUTLEGCATEGORY.ToLower() == "mast" && !x.Dismantle)
                            .AsEnumerable().Select(x =>

                                 new
                                 {
                                     HieghFromLand = x.HieghFromLand,
                                     EquivalentSpace = x.EquivalentSpace,
                                     Support_Limited_Load = x.Support_Limited_Load,
                                     SITECODE = x.SITECODE,
                                     Id = x.Id,
                                     Name = x.Name,
                                     UpperPartLengthm = x.UpperPartLengthm,
                                     UpperPartDiameterm = x.UpperPartDiameterm,
                                     SpindlesBasePlateLengthcm = x.SpindlesBasePlateLengthcm,
                                     SpindlesBasePlateWidthcm = x.SpindlesBasePlateWidthcm,
                                     ConcreteBaseWidthm = x.ConcreteBaseWidthm,
                                     SpinBasePlateAnchorDiametercm = x.SpinBasePlateAnchorDiametercm,
                                     NumberOfCivilParts = x.NumberOfCivilParts,
                                     NumberOfLongitudinalSpindles = x.NumberOfLongitudinalSpindles,
                                     NumberOfhorizontalSpindle = x.NumberOfhorizontalSpindle,
                                     CivilLengthAboveEndOfSpindles = x.CivilLengthAboveEndOfSpindles,
                                     CivilBaseLevelFromGround = x.CivilBaseLevelFromGround,
                                     LongitudinalSpinDiameterrmm = x.LongitudinalSpinDiameterrmm,
                                     HorizontalSpindlesHBAm = x.HorizontalSpindlesHBAm,
                                     HorizontalSpindleDiametermm = x.HorizontalSpindleDiametermm,
                                     FlangeThicknesscm = x.FlangeThicknesscm,
                                     FlangeDiametercm = x.FlangeDiametercm,
                                     FlangeBoltsDiametermm = x.FlangeBoltsDiametermm,
                                     ConcreteBaseThicknessm = x.ConcreteBaseThicknessm,
                                     ConcreteBaseLengthm = x.ConcreteBaseLengthm,
                                     Civil_Remarks = x.Civil_Remarks,
                                     BottomPartLengthm = x.BottomPartLengthm,
                                     BottomPartDiameterm = x.BottomPartDiameterm,
                                     BasePlateWidthcm = x.BasePlateWidthcm,
                                     BasePlateThicknesscm = x.BasePlateThicknesscm,
                                     BasePlateLengthcm = x.BasePlateLengthcm,
                                     BPlateBoltsAnchorDiametermm = x.BPlateBoltsAnchorDiametermm,
                                     BaseBeamSectionmm = x.BaseBeamSectionmm,
                                     WindMaxLoadm2 = x.WindMaxLoadm2,
                                     Location_Height = x.Location_Height,
                                     PoType = x.PoType,
                                     PoNo = x.PoNo,
                                     PoDate = x.PoDate,
                                     HeightImplemented = x.HeightImplemented,
                                     BuildingMaxLoad = x.BuildingMaxLoad,
                                     SupportMaxLoadAfterInforcement = x.SupportMaxLoadAfterInforcement,
                                     CurrentLoads = x.CurrentLoads,
                                     WarningPercentageLoads = x.WarningPercentageLoads,
                                     Visiable_Status = x.Visiable_Status,
                                     SpaceInstallation = x.SpaceInstallation,
                                     CIVILWITHOUTLEGSLIB = x.CIVILWITHOUTLEGSLIB,
                                     OWNER = x.OWNER,
                                     SUBTYPE = x.SUBTYPE,
                                     HeightBase = x.HeightBase,
                                     BuildingHeightH3 = x.BuildingHeightH3,
                                     reinforced = x.reinforced,
                                     ladderSteps = x.ladderSteps,
                                     availabilityOfWorkPlatforms = x.availabilityOfWorkPlatforms,
                                     equipmentsLocation = x.equipmentsLocation,
                                     CenterHigh = x.CenterHigh,
                                     HBA = x.HBA,
                                     CIVILWITHOUTLEGCATEGORY = x.CIVILWITHOUTLEGCATEGORY,
                                     Dismantle = x.Dismantle

                                 }).Distinct().Select(item => _unitOfWork.CivilWithLegsRepository.BuildDynamicSelect(item, null, propertyNamesStatic, propertyNamesDynamic))
                          ;
                            int count = query.Count();

                            getEnableAttribute.Model = query;
                            string excelFilePath = ExportToExcel(query, "TLIcivilWithoutLegMast");

                            return new Response<string>(false, excelFilePath, null, null, (int)Helpers.Constants.ApiReturnCode.success);
                        }
                        else
                        {
                            var query = _context.MV_CIVIL_WITHOUTLEGS_VIEW.Where(x => x.SITECODE.ToLower() == SiteCode.ToLower() &&
                             x.CIVILWITHOUTLEGCATEGORY.ToLower() == "mast" && !x.Dismantle).Select(x =>

                                 new
                                 {
                                     HieghFromLand = x.HieghFromLand,
                                     EquivalentSpace = x.EquivalentSpace,
                                     Support_Limited_Load = x.Support_Limited_Load,
                                     SITECODE = x.SITECODE,
                                     Id = x.Id,
                                     Name = x.Name,
                                     UpperPartLengthm = x.UpperPartLengthm,
                                     UpperPartDiameterm = x.UpperPartDiameterm,
                                     SpindlesBasePlateLengthcm = x.SpindlesBasePlateLengthcm,
                                     SpindlesBasePlateWidthcm = x.SpindlesBasePlateWidthcm,
                                     ConcreteBaseWidthm = x.ConcreteBaseWidthm,
                                     SpinBasePlateAnchorDiametercm = x.SpinBasePlateAnchorDiametercm,
                                     NumberOfCivilParts = x.NumberOfCivilParts,
                                     NumberOfLongitudinalSpindles = x.NumberOfLongitudinalSpindles,
                                     NumberOfhorizontalSpindle = x.NumberOfhorizontalSpindle,
                                     CivilLengthAboveEndOfSpindles = x.CivilLengthAboveEndOfSpindles,
                                     CivilBaseLevelFromGround = x.CivilBaseLevelFromGround,
                                     LongitudinalSpinDiameterrmm = x.LongitudinalSpinDiameterrmm,
                                     HorizontalSpindlesHBAm = x.HorizontalSpindlesHBAm,
                                     HorizontalSpindleDiametermm = x.HorizontalSpindleDiametermm,
                                     FlangeThicknesscm = x.FlangeThicknesscm,
                                     FlangeDiametercm = x.FlangeDiametercm,
                                     FlangeBoltsDiametermm = x.FlangeBoltsDiametermm,
                                     ConcreteBaseThicknessm = x.ConcreteBaseThicknessm,
                                     ConcreteBaseLengthm = x.ConcreteBaseLengthm,
                                     Civil_Remarks = x.Civil_Remarks,
                                     BottomPartLengthm = x.BottomPartLengthm,
                                     BottomPartDiameterm = x.BottomPartDiameterm,
                                     BasePlateWidthcm = x.BasePlateWidthcm,
                                     BasePlateThicknesscm = x.BasePlateThicknesscm,
                                     BasePlateLengthcm = x.BasePlateLengthcm,
                                     BPlateBoltsAnchorDiametermm = x.BPlateBoltsAnchorDiametermm,
                                     BaseBeamSectionmm = x.BaseBeamSectionmm,
                                     WindMaxLoadm2 = x.WindMaxLoadm2,
                                     Location_Height = x.Location_Height,
                                     PoType = x.PoType,
                                     PoNo = x.PoNo,
                                     PoDate = x.PoDate,
                                     HeightImplemented = x.HeightImplemented,
                                     BuildingMaxLoad = x.BuildingMaxLoad,
                                     SupportMaxLoadAfterInforcement = x.SupportMaxLoadAfterInforcement,
                                     CurrentLoads = x.CurrentLoads,
                                     WarningPercentageLoads = x.WarningPercentageLoads,
                                     Visiable_Status = x.Visiable_Status,
                                     SpaceInstallation = x.SpaceInstallation,
                                     CIVILWITHOUTLEGSLIB = x.CIVILWITHOUTLEGSLIB,
                                     OWNER = x.OWNER,
                                     SUBTYPE = x.SUBTYPE,
                                     HeightBase = x.HeightBase,
                                     BuildingHeightH3 = x.BuildingHeightH3,
                                     reinforced = x.reinforced,
                                     ladderSteps = x.ladderSteps,
                                     availabilityOfWorkPlatforms = x.availabilityOfWorkPlatforms,
                                     equipmentsLocation = x.equipmentsLocation,
                                     CenterHigh = x.CenterHigh,
                                     HBA = x.HBA,
                                     INPUTVALUE = x.INPUTVALUE,
                                     Key = x.Key,
                                     CIVILWITHOUTLEGCATEGORY = x.CIVILWITHOUTLEGCATEGORY,
                                     Dismantle = x.Dismantle

                                 }).AsEnumerable()
                        .GroupBy(x => new
                        {
                            HieghFromLand = x.HieghFromLand,
                            EquivalentSpace = x.EquivalentSpace,
                            Support_Limited_Load = x.Support_Limited_Load,
                            SITECODE = x.SITECODE,
                            Id = x.Id,
                            Name = x.Name,
                            UpperPartLengthm = x.UpperPartLengthm,
                            UpperPartDiameterm = x.UpperPartDiameterm,
                            SpindlesBasePlateLengthcm = x.SpindlesBasePlateLengthcm,
                            SpindlesBasePlateWidthcm = x.SpindlesBasePlateWidthcm,
                            ConcreteBaseWidthm = x.ConcreteBaseWidthm,
                            SpinBasePlateAnchorDiametercm = x.SpinBasePlateAnchorDiametercm,
                            NumberOfCivilParts = x.NumberOfCivilParts,
                            NumberOfLongitudinalSpindles = x.NumberOfLongitudinalSpindles,
                            NumberOfhorizontalSpindle = x.NumberOfhorizontalSpindle,
                            CivilLengthAboveEndOfSpindles = x.CivilLengthAboveEndOfSpindles,
                            CivilBaseLevelFromGround = x.CivilBaseLevelFromGround,
                            LongitudinalSpinDiameterrmm = x.LongitudinalSpinDiameterrmm,
                            HorizontalSpindlesHBAm = x.HorizontalSpindlesHBAm,
                            HorizontalSpindleDiametermm = x.HorizontalSpindleDiametermm,
                            FlangeThicknesscm = x.FlangeThicknesscm,
                            FlangeDiametercm = x.FlangeDiametercm,
                            FlangeBoltsDiametermm = x.FlangeBoltsDiametermm,
                            ConcreteBaseThicknessm = x.ConcreteBaseThicknessm,
                            ConcreteBaseLengthm = x.ConcreteBaseLengthm,
                            Civil_Remarks = x.Civil_Remarks,
                            BottomPartLengthm = x.BottomPartLengthm,
                            BottomPartDiameterm = x.BottomPartDiameterm,
                            BasePlateWidthcm = x.BasePlateWidthcm,
                            BasePlateThicknesscm = x.BasePlateThicknesscm,
                            BasePlateLengthcm = x.BasePlateLengthcm,
                            BPlateBoltsAnchorDiametermm = x.BPlateBoltsAnchorDiametermm,
                            BaseBeamSectionmm = x.BaseBeamSectionmm,
                            WindMaxLoadm2 = x.WindMaxLoadm2,
                            Location_Height = x.Location_Height,
                            PoType = x.PoType,
                            PoNo = x.PoNo,
                            PoDate = x.PoDate,
                            HeightImplemented = x.HeightImplemented,
                            BuildingMaxLoad = x.BuildingMaxLoad,
                            SupportMaxLoadAfterInforcement = x.SupportMaxLoadAfterInforcement,
                            CurrentLoads = x.CurrentLoads,
                            WarningPercentageLoads = x.WarningPercentageLoads,
                            Visiable_Status = x.Visiable_Status,
                            SpaceInstallation = x.SpaceInstallation,
                            CIVILWITHOUTLEGSLIB = x.CIVILWITHOUTLEGSLIB,
                            OWNER = x.OWNER,
                            SUBTYPE = x.SUBTYPE,
                            HeightBase = x.HeightBase,
                            BuildingHeightH3 = x.BuildingHeightH3,
                            reinforced = x.reinforced,
                            ladderSteps = x.ladderSteps,
                            availabilityOfWorkPlatforms = x.availabilityOfWorkPlatforms,
                            equipmentsLocation = x.equipmentsLocation,
                            CenterHigh = x.CenterHigh,
                            HBA = x.HBA,
                            CIVILWITHOUTLEGCATEGORY = x.CIVILWITHOUTLEGCATEGORY,
                            Dismantle = x.Dismantle
                        }).OrderBy(x => x.Key.Name)
                        .Select(x =>
                            new { key = x.Key, value = x.ToDictionary(z => z.Key, z => z.INPUTVALUE) }
                        )
                        .Select(item => _unitOfWork.CivilWithLegsRepository.BuildDynamicSelect(item.key, item.value, propertyNamesStatic, propertyNamesDynamic));
                            int count = query.Count();

                            getEnableAttribute.Model = query;
                            string excelFilePath = ExportToExcel(query, "TLIcivilWithoutLegMast");

                            return new Response<string>(false, excelFilePath, null, null, (int)Helpers.Constants.ApiReturnCode.success);
                        }

                    }
                    catch (Exception err)
                    {
                        return new Response<string>(false, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
                    }
                }
                else if (Helpers.Constants.CivilType.TLIcivilWithoutLeg.ToString() == TableNameInstallation && CategoryId == 2)
                {
                    try
                    {

                        GetEnableAttribute getEnableAttribute = new GetEnableAttribute();
                        connection.Open();
                        //string storedProcedureName = "CREATE_DYNAMIC_PIVOT_WITHOUTLEG";
                        //using (OracleCommand procedureCommand = new OracleCommand(storedProcedureName, connection))
                        //{
                        //    procedureCommand.CommandType = CommandType.StoredProcedure;
                        //    procedureCommand.ExecuteNonQuery();
                        //}
                        var attActivated = _context.TLIattributeViewManagment
                            .Include(x => x.EditableManagmentView)
                            .Include(x => x.AttributeActivated)
                            .Include(x => x.DynamicAtt)
                            .Where(x => x.Enable && x.EditableManagmentView.View == "CivilWithoutLegInstallationCapsule" &&
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

                                var query = _context.MV_CIVIL_WITHOUTLEGS_VIEW
                                .Where(x =>
                                 x.CIVILWITHOUTLEGCATEGORY.ToLower() == "capsule" && !x.Dismantle)
                                .AsEnumerable().Select(x =>

                                     new
                                     {
                                         HieghFromLand = x.HieghFromLand,
                                         EquivalentSpace = x.EquivalentSpace,
                                         Support_Limited_Load = x.Support_Limited_Load,
                                         SITECODE = x.SITECODE,
                                         Id = x.Id,
                                         Name = x.Name,
                                         UpperPartLengthm = x.UpperPartLengthm,
                                         UpperPartDiameterm = x.UpperPartDiameterm,
                                         SpindlesBasePlateLengthcm = x.SpindlesBasePlateLengthcm,
                                         SpindlesBasePlateWidthcm = x.SpindlesBasePlateWidthcm,
                                         ConcreteBaseWidthm = x.ConcreteBaseWidthm,
                                         SpinBasePlateAnchorDiametercm = x.SpinBasePlateAnchorDiametercm,
                                         NumberOfCivilParts = x.NumberOfCivilParts,
                                         NumberOfLongitudinalSpindles = x.NumberOfLongitudinalSpindles,
                                         NumberOfhorizontalSpindle = x.NumberOfhorizontalSpindle,
                                         CivilLengthAboveEndOfSpindles = x.CivilLengthAboveEndOfSpindles,
                                         CivilBaseLevelFromGround = x.CivilBaseLevelFromGround,
                                         LongitudinalSpinDiameterrmm = x.LongitudinalSpinDiameterrmm,
                                         HorizontalSpindlesHBAm = x.HorizontalSpindlesHBAm,
                                         HorizontalSpindleDiametermm = x.HorizontalSpindleDiametermm,
                                         FlangeThicknesscm = x.FlangeThicknesscm,
                                         FlangeDiametercm = x.FlangeDiametercm,
                                         FlangeBoltsDiametermm = x.FlangeBoltsDiametermm,
                                         ConcreteBaseThicknessm = x.ConcreteBaseThicknessm,
                                         ConcreteBaseLengthm = x.ConcreteBaseLengthm,
                                         Civil_Remarks = x.Civil_Remarks,
                                         BottomPartLengthm = x.BottomPartLengthm,
                                         BottomPartDiameterm = x.BottomPartDiameterm,
                                         BasePlateWidthcm = x.BasePlateWidthcm,
                                         BasePlateThicknesscm = x.BasePlateThicknesscm,
                                         BasePlateLengthcm = x.BasePlateLengthcm,
                                         BPlateBoltsAnchorDiametermm = x.BPlateBoltsAnchorDiametermm,
                                         BaseBeamSectionmm = x.BaseBeamSectionmm,
                                         WindMaxLoadm2 = x.WindMaxLoadm2,
                                         Location_Height = x.Location_Height,
                                         PoType = x.PoType,
                                         PoNo = x.PoNo,
                                         PoDate = x.PoDate,
                                         HeightImplemented = x.HeightImplemented,
                                         BuildingMaxLoad = x.BuildingMaxLoad,
                                         SupportMaxLoadAfterInforcement = x.SupportMaxLoadAfterInforcement,
                                         CurrentLoads = x.CurrentLoads,
                                         WarningPercentageLoads = x.WarningPercentageLoads,
                                         Visiable_Status = x.Visiable_Status,
                                         SpaceInstallation = x.SpaceInstallation,
                                         CIVILWITHOUTLEGSLIB = x.CIVILWITHOUTLEGSLIB,
                                         OWNER = x.OWNER,
                                         SUBTYPE = x.SUBTYPE,
                                         HeightBase = x.HeightBase,
                                         BuildingHeightH3 = x.BuildingHeightH3,
                                         reinforced = x.reinforced,
                                         ladderSteps = x.ladderSteps,
                                         availabilityOfWorkPlatforms = x.availabilityOfWorkPlatforms,
                                         equipmentsLocation = x.equipmentsLocation,
                                         CenterHigh = x.CenterHigh,
                                         HBA = x.HBA,
                                         CIVILWITHOUTLEGCATEGORY = x.CIVILWITHOUTLEGCATEGORY,
                                         Dismantle = x.Dismantle

                                     }).Distinct().Select(item => _unitOfWork.CivilWithLegsRepository.BuildDynamicSelect(item, null, propertyNamesStatic, propertyNamesDynamic))
                              ;
                                int count = query.Count();

                                getEnableAttribute.Model = query;
                                string excelFilePath = ExportToExcel(query, "TLIcivilWithoutLegCapsule");

                                return new Response<string>(false, excelFilePath, null, null, (int)Helpers.Constants.ApiReturnCode.success);
                            }
                            else
                            {
                                var query = _context.MV_CIVIL_WITHOUTLEGS_VIEW.Where(x =>
                                 x.CIVILWITHOUTLEGCATEGORY.ToLower() == "capsule" && !x.Dismantle).Select(x =>

                                     new
                                     {
                                         HieghFromLand = x.HieghFromLand,
                                         EquivalentSpace = x.EquivalentSpace,
                                         Support_Limited_Load = x.Support_Limited_Load,
                                         SITECODE = x.SITECODE,
                                         Id = x.Id,
                                         Name = x.Name,
                                         UpperPartLengthm = x.UpperPartLengthm,
                                         UpperPartDiameterm = x.UpperPartDiameterm,
                                         SpindlesBasePlateLengthcm = x.SpindlesBasePlateLengthcm,
                                         SpindlesBasePlateWidthcm = x.SpindlesBasePlateWidthcm,
                                         ConcreteBaseWidthm = x.ConcreteBaseWidthm,
                                         SpinBasePlateAnchorDiametercm = x.SpinBasePlateAnchorDiametercm,
                                         NumberOfCivilParts = x.NumberOfCivilParts,
                                         NumberOfLongitudinalSpindles = x.NumberOfLongitudinalSpindles,
                                         NumberOfhorizontalSpindle = x.NumberOfhorizontalSpindle,
                                         CivilLengthAboveEndOfSpindles = x.CivilLengthAboveEndOfSpindles,
                                         CivilBaseLevelFromGround = x.CivilBaseLevelFromGround,
                                         LongitudinalSpinDiameterrmm = x.LongitudinalSpinDiameterrmm,
                                         HorizontalSpindlesHBAm = x.HorizontalSpindlesHBAm,
                                         HorizontalSpindleDiametermm = x.HorizontalSpindleDiametermm,
                                         FlangeThicknesscm = x.FlangeThicknesscm,
                                         FlangeDiametercm = x.FlangeDiametercm,
                                         FlangeBoltsDiametermm = x.FlangeBoltsDiametermm,
                                         ConcreteBaseThicknessm = x.ConcreteBaseThicknessm,
                                         ConcreteBaseLengthm = x.ConcreteBaseLengthm,
                                         Civil_Remarks = x.Civil_Remarks,
                                         BottomPartLengthm = x.BottomPartLengthm,
                                         BottomPartDiameterm = x.BottomPartDiameterm,
                                         BasePlateWidthcm = x.BasePlateWidthcm,
                                         BasePlateThicknesscm = x.BasePlateThicknesscm,
                                         BasePlateLengthcm = x.BasePlateLengthcm,
                                         BPlateBoltsAnchorDiametermm = x.BPlateBoltsAnchorDiametermm,
                                         BaseBeamSectionmm = x.BaseBeamSectionmm,
                                         WindMaxLoadm2 = x.WindMaxLoadm2,
                                         Location_Height = x.Location_Height,
                                         PoType = x.PoType,
                                         PoNo = x.PoNo,
                                         PoDate = x.PoDate,
                                         HeightImplemented = x.HeightImplemented,
                                         BuildingMaxLoad = x.BuildingMaxLoad,
                                         SupportMaxLoadAfterInforcement = x.SupportMaxLoadAfterInforcement,
                                         CurrentLoads = x.CurrentLoads,
                                         WarningPercentageLoads = x.WarningPercentageLoads,
                                         Visiable_Status = x.Visiable_Status,
                                         SpaceInstallation = x.SpaceInstallation,
                                         CIVILWITHOUTLEGSLIB = x.CIVILWITHOUTLEGSLIB,
                                         OWNER = x.OWNER,
                                         SUBTYPE = x.SUBTYPE,
                                         HeightBase = x.HeightBase,
                                         BuildingHeightH3 = x.BuildingHeightH3,
                                         reinforced = x.reinforced,
                                         ladderSteps = x.ladderSteps,
                                         availabilityOfWorkPlatforms = x.availabilityOfWorkPlatforms,
                                         equipmentsLocation = x.equipmentsLocation,
                                         CenterHigh = x.CenterHigh,
                                         HBA = x.HBA,
                                         INPUTVALUE = x.INPUTVALUE,
                                         Key = x.Key,
                                         CIVILWITHOUTLEGCATEGORY = x.CIVILWITHOUTLEGCATEGORY,
                                         Dismantle = x.Dismantle

                                     }).AsEnumerable()
                            .GroupBy(x => new
                            {
                                HieghFromLand = x.HieghFromLand,
                                EquivalentSpace = x.EquivalentSpace,
                                Support_Limited_Load = x.Support_Limited_Load,
                                SITECODE = x.SITECODE,
                                Id = x.Id,
                                Name = x.Name,
                                UpperPartLengthm = x.UpperPartLengthm,
                                UpperPartDiameterm = x.UpperPartDiameterm,
                                SpindlesBasePlateLengthcm = x.SpindlesBasePlateLengthcm,
                                SpindlesBasePlateWidthcm = x.SpindlesBasePlateWidthcm,
                                ConcreteBaseWidthm = x.ConcreteBaseWidthm,
                                SpinBasePlateAnchorDiametercm = x.SpinBasePlateAnchorDiametercm,
                                NumberOfCivilParts = x.NumberOfCivilParts,
                                NumberOfLongitudinalSpindles = x.NumberOfLongitudinalSpindles,
                                NumberOfhorizontalSpindle = x.NumberOfhorizontalSpindle,
                                CivilLengthAboveEndOfSpindles = x.CivilLengthAboveEndOfSpindles,
                                CivilBaseLevelFromGround = x.CivilBaseLevelFromGround,
                                LongitudinalSpinDiameterrmm = x.LongitudinalSpinDiameterrmm,
                                HorizontalSpindlesHBAm = x.HorizontalSpindlesHBAm,
                                HorizontalSpindleDiametermm = x.HorizontalSpindleDiametermm,
                                FlangeThicknesscm = x.FlangeThicknesscm,
                                FlangeDiametercm = x.FlangeDiametercm,
                                FlangeBoltsDiametermm = x.FlangeBoltsDiametermm,
                                ConcreteBaseThicknessm = x.ConcreteBaseThicknessm,
                                ConcreteBaseLengthm = x.ConcreteBaseLengthm,
                                Civil_Remarks = x.Civil_Remarks,
                                BottomPartLengthm = x.BottomPartLengthm,
                                BottomPartDiameterm = x.BottomPartDiameterm,
                                BasePlateWidthcm = x.BasePlateWidthcm,
                                BasePlateThicknesscm = x.BasePlateThicknesscm,
                                BasePlateLengthcm = x.BasePlateLengthcm,
                                BPlateBoltsAnchorDiametermm = x.BPlateBoltsAnchorDiametermm,
                                BaseBeamSectionmm = x.BaseBeamSectionmm,
                                WindMaxLoadm2 = x.WindMaxLoadm2,
                                Location_Height = x.Location_Height,
                                PoType = x.PoType,
                                PoNo = x.PoNo,
                                PoDate = x.PoDate,
                                HeightImplemented = x.HeightImplemented,
                                BuildingMaxLoad = x.BuildingMaxLoad,
                                SupportMaxLoadAfterInforcement = x.SupportMaxLoadAfterInforcement,
                                CurrentLoads = x.CurrentLoads,
                                WarningPercentageLoads = x.WarningPercentageLoads,
                                Visiable_Status = x.Visiable_Status,
                                SpaceInstallation = x.SpaceInstallation,
                                CIVILWITHOUTLEGSLIB = x.CIVILWITHOUTLEGSLIB,
                                OWNER = x.OWNER,
                                SUBTYPE = x.SUBTYPE,
                                HeightBase = x.HeightBase,
                                BuildingHeightH3 = x.BuildingHeightH3,
                                reinforced = x.reinforced,
                                ladderSteps = x.ladderSteps,
                                availabilityOfWorkPlatforms = x.availabilityOfWorkPlatforms,
                                equipmentsLocation = x.equipmentsLocation,
                                CenterHigh = x.CenterHigh,
                                HBA = x.HBA,
                                CIVILWITHOUTLEGCATEGORY = x.CIVILWITHOUTLEGCATEGORY,
                                Dismantle = x.Dismantle
                            }).OrderBy(x => x.Key.Name)
                            .Select(x =>
                                new { key = x.Key, value = x.ToDictionary(z => z.Key, z => z.INPUTVALUE) }
                            )
                            .Select(item => _unitOfWork.CivilWithLegsRepository.BuildDynamicSelect(item.key, item.value, propertyNamesStatic, propertyNamesDynamic));
                                int count = query.Count();

                                getEnableAttribute.Model = query;
                                string excelFilePath = ExportToExcel(query, "TLIcivilWithoutLegCapsule");

                                return new Response<string>(false, excelFilePath, null, null, (int)Helpers.Constants.ApiReturnCode.success);
                            }
                            if (propertyNamesDynamic.Count == 0)
                            {

                                var query = _context.MV_CIVIL_WITHOUTLEGS_VIEW
                                .Where(x => x.SITECODE.ToLower() == SiteCode.ToLower() &&
                                 x.CIVILWITHOUTLEGCATEGORY.ToLower() == "capsule" && !x.Dismantle)
                                .AsEnumerable().Select(x =>

                                     new
                                     {
                                         HieghFromLand = x.HieghFromLand,
                                         EquivalentSpace = x.EquivalentSpace,
                                         Support_Limited_Load = x.Support_Limited_Load,
                                         SITECODE = x.SITECODE,
                                         Id = x.Id,
                                         Name = x.Name,
                                         UpperPartLengthm = x.UpperPartLengthm,
                                         UpperPartDiameterm = x.UpperPartDiameterm,
                                         SpindlesBasePlateLengthcm = x.SpindlesBasePlateLengthcm,
                                         SpindlesBasePlateWidthcm = x.SpindlesBasePlateWidthcm,
                                         ConcreteBaseWidthm = x.ConcreteBaseWidthm,
                                         SpinBasePlateAnchorDiametercm = x.SpinBasePlateAnchorDiametercm,
                                         NumberOfCivilParts = x.NumberOfCivilParts,
                                         NumberOfLongitudinalSpindles = x.NumberOfLongitudinalSpindles,
                                         NumberOfhorizontalSpindle = x.NumberOfhorizontalSpindle,
                                         CivilLengthAboveEndOfSpindles = x.CivilLengthAboveEndOfSpindles,
                                         CivilBaseLevelFromGround = x.CivilBaseLevelFromGround,
                                         LongitudinalSpinDiameterrmm = x.LongitudinalSpinDiameterrmm,
                                         HorizontalSpindlesHBAm = x.HorizontalSpindlesHBAm,
                                         HorizontalSpindleDiametermm = x.HorizontalSpindleDiametermm,
                                         FlangeThicknesscm = x.FlangeThicknesscm,
                                         FlangeDiametercm = x.FlangeDiametercm,
                                         FlangeBoltsDiametermm = x.FlangeBoltsDiametermm,
                                         ConcreteBaseThicknessm = x.ConcreteBaseThicknessm,
                                         ConcreteBaseLengthm = x.ConcreteBaseLengthm,
                                         Civil_Remarks = x.Civil_Remarks,
                                         BottomPartLengthm = x.BottomPartLengthm,
                                         BottomPartDiameterm = x.BottomPartDiameterm,
                                         BasePlateWidthcm = x.BasePlateWidthcm,
                                         BasePlateThicknesscm = x.BasePlateThicknesscm,
                                         BasePlateLengthcm = x.BasePlateLengthcm,
                                         BPlateBoltsAnchorDiametermm = x.BPlateBoltsAnchorDiametermm,
                                         BaseBeamSectionmm = x.BaseBeamSectionmm,
                                         WindMaxLoadm2 = x.WindMaxLoadm2,
                                         Location_Height = x.Location_Height,
                                         PoType = x.PoType,
                                         PoNo = x.PoNo,
                                         PoDate = x.PoDate,
                                         HeightImplemented = x.HeightImplemented,
                                         BuildingMaxLoad = x.BuildingMaxLoad,
                                         SupportMaxLoadAfterInforcement = x.SupportMaxLoadAfterInforcement,
                                         CurrentLoads = x.CurrentLoads,
                                         WarningPercentageLoads = x.WarningPercentageLoads,
                                         Visiable_Status = x.Visiable_Status,
                                         SpaceInstallation = x.SpaceInstallation,
                                         CIVILWITHOUTLEGSLIB = x.CIVILWITHOUTLEGSLIB,
                                         OWNER = x.OWNER,
                                         SUBTYPE = x.SUBTYPE,
                                         HeightBase = x.HeightBase,
                                         BuildingHeightH3 = x.BuildingHeightH3,
                                         reinforced = x.reinforced,
                                         ladderSteps = x.ladderSteps,
                                         availabilityOfWorkPlatforms = x.availabilityOfWorkPlatforms,
                                         equipmentsLocation = x.equipmentsLocation,
                                         CenterHigh = x.CenterHigh,
                                         HBA = x.HBA,
                                         CIVILWITHOUTLEGCATEGORY = x.CIVILWITHOUTLEGCATEGORY,
                                         Dismantle = x.Dismantle

                                     }).Distinct().Select(item => _unitOfWork.CivilWithLegsRepository.BuildDynamicSelect(item, null, propertyNamesStatic, propertyNamesDynamic))
                              ;
                                int count = query.Count();

                                getEnableAttribute.Model = query;
                                string excelFilePath = ExportToExcel(query, "TLIcivilWithoutLegCapsule");

                                return new Response<string>(false, excelFilePath, null, null, (int)Helpers.Constants.ApiReturnCode.success);
                            }
                            else
                            {
                                var query = _context.MV_CIVIL_WITHOUTLEGS_VIEW.Where(x => x.SITECODE.ToLower() == SiteCode.ToLower() &&
                                 x.CIVILWITHOUTLEGCATEGORY.ToLower() == "capsule" && !x.Dismantle).Select(x =>

                                     new
                                     {
                                         HieghFromLand = x.HieghFromLand,
                                         EquivalentSpace = x.EquivalentSpace,
                                         Support_Limited_Load = x.Support_Limited_Load,
                                         SITECODE = x.SITECODE,
                                         Id = x.Id,
                                         Name = x.Name,
                                         UpperPartLengthm = x.UpperPartLengthm,
                                         UpperPartDiameterm = x.UpperPartDiameterm,
                                         SpindlesBasePlateLengthcm = x.SpindlesBasePlateLengthcm,
                                         SpindlesBasePlateWidthcm = x.SpindlesBasePlateWidthcm,
                                         ConcreteBaseWidthm = x.ConcreteBaseWidthm,
                                         SpinBasePlateAnchorDiametercm = x.SpinBasePlateAnchorDiametercm,
                                         NumberOfCivilParts = x.NumberOfCivilParts,
                                         NumberOfLongitudinalSpindles = x.NumberOfLongitudinalSpindles,
                                         NumberOfhorizontalSpindle = x.NumberOfhorizontalSpindle,
                                         CivilLengthAboveEndOfSpindles = x.CivilLengthAboveEndOfSpindles,
                                         CivilBaseLevelFromGround = x.CivilBaseLevelFromGround,
                                         LongitudinalSpinDiameterrmm = x.LongitudinalSpinDiameterrmm,
                                         HorizontalSpindlesHBAm = x.HorizontalSpindlesHBAm,
                                         HorizontalSpindleDiametermm = x.HorizontalSpindleDiametermm,
                                         FlangeThicknesscm = x.FlangeThicknesscm,
                                         FlangeDiametercm = x.FlangeDiametercm,
                                         FlangeBoltsDiametermm = x.FlangeBoltsDiametermm,
                                         ConcreteBaseThicknessm = x.ConcreteBaseThicknessm,
                                         ConcreteBaseLengthm = x.ConcreteBaseLengthm,
                                         Civil_Remarks = x.Civil_Remarks,
                                         BottomPartLengthm = x.BottomPartLengthm,
                                         BottomPartDiameterm = x.BottomPartDiameterm,
                                         BasePlateWidthcm = x.BasePlateWidthcm,
                                         BasePlateThicknesscm = x.BasePlateThicknesscm,
                                         BasePlateLengthcm = x.BasePlateLengthcm,
                                         BPlateBoltsAnchorDiametermm = x.BPlateBoltsAnchorDiametermm,
                                         BaseBeamSectionmm = x.BaseBeamSectionmm,
                                         WindMaxLoadm2 = x.WindMaxLoadm2,
                                         Location_Height = x.Location_Height,
                                         PoType = x.PoType,
                                         PoNo = x.PoNo,
                                         PoDate = x.PoDate,
                                         HeightImplemented = x.HeightImplemented,
                                         BuildingMaxLoad = x.BuildingMaxLoad,
                                         SupportMaxLoadAfterInforcement = x.SupportMaxLoadAfterInforcement,
                                         CurrentLoads = x.CurrentLoads,
                                         WarningPercentageLoads = x.WarningPercentageLoads,
                                         Visiable_Status = x.Visiable_Status,
                                         SpaceInstallation = x.SpaceInstallation,
                                         CIVILWITHOUTLEGSLIB = x.CIVILWITHOUTLEGSLIB,
                                         OWNER = x.OWNER,
                                         SUBTYPE = x.SUBTYPE,
                                         HeightBase = x.HeightBase,
                                         BuildingHeightH3 = x.BuildingHeightH3,
                                         reinforced = x.reinforced,
                                         ladderSteps = x.ladderSteps,
                                         availabilityOfWorkPlatforms = x.availabilityOfWorkPlatforms,
                                         equipmentsLocation = x.equipmentsLocation,
                                         CenterHigh = x.CenterHigh,
                                         HBA = x.HBA,
                                         INPUTVALUE = x.INPUTVALUE,
                                         Key = x.Key,
                                         CIVILWITHOUTLEGCATEGORY = x.CIVILWITHOUTLEGCATEGORY,
                                         Dismantle = x.Dismantle

                                     }).AsEnumerable()
                            .GroupBy(x => new
                            {
                                HieghFromLand = x.HieghFromLand,
                                EquivalentSpace = x.EquivalentSpace,
                                Support_Limited_Load = x.Support_Limited_Load,
                                SITECODE = x.SITECODE,
                                Id = x.Id,
                                Name = x.Name,
                                UpperPartLengthm = x.UpperPartLengthm,
                                UpperPartDiameterm = x.UpperPartDiameterm,
                                SpindlesBasePlateLengthcm = x.SpindlesBasePlateLengthcm,
                                SpindlesBasePlateWidthcm = x.SpindlesBasePlateWidthcm,
                                ConcreteBaseWidthm = x.ConcreteBaseWidthm,
                                SpinBasePlateAnchorDiametercm = x.SpinBasePlateAnchorDiametercm,
                                NumberOfCivilParts = x.NumberOfCivilParts,
                                NumberOfLongitudinalSpindles = x.NumberOfLongitudinalSpindles,
                                NumberOfhorizontalSpindle = x.NumberOfhorizontalSpindle,
                                CivilLengthAboveEndOfSpindles = x.CivilLengthAboveEndOfSpindles,
                                CivilBaseLevelFromGround = x.CivilBaseLevelFromGround,
                                LongitudinalSpinDiameterrmm = x.LongitudinalSpinDiameterrmm,
                                HorizontalSpindlesHBAm = x.HorizontalSpindlesHBAm,
                                HorizontalSpindleDiametermm = x.HorizontalSpindleDiametermm,
                                FlangeThicknesscm = x.FlangeThicknesscm,
                                FlangeDiametercm = x.FlangeDiametercm,
                                FlangeBoltsDiametermm = x.FlangeBoltsDiametermm,
                                ConcreteBaseThicknessm = x.ConcreteBaseThicknessm,
                                ConcreteBaseLengthm = x.ConcreteBaseLengthm,
                                Civil_Remarks = x.Civil_Remarks,
                                BottomPartLengthm = x.BottomPartLengthm,
                                BottomPartDiameterm = x.BottomPartDiameterm,
                                BasePlateWidthcm = x.BasePlateWidthcm,
                                BasePlateThicknesscm = x.BasePlateThicknesscm,
                                BasePlateLengthcm = x.BasePlateLengthcm,
                                BPlateBoltsAnchorDiametermm = x.BPlateBoltsAnchorDiametermm,
                                BaseBeamSectionmm = x.BaseBeamSectionmm,
                                WindMaxLoadm2 = x.WindMaxLoadm2,
                                Location_Height = x.Location_Height,
                                PoType = x.PoType,
                                PoNo = x.PoNo,
                                PoDate = x.PoDate,
                                HeightImplemented = x.HeightImplemented,
                                BuildingMaxLoad = x.BuildingMaxLoad,
                                SupportMaxLoadAfterInforcement = x.SupportMaxLoadAfterInforcement,
                                CurrentLoads = x.CurrentLoads,
                                WarningPercentageLoads = x.WarningPercentageLoads,
                                Visiable_Status = x.Visiable_Status,
                                SpaceInstallation = x.SpaceInstallation,
                                CIVILWITHOUTLEGSLIB = x.CIVILWITHOUTLEGSLIB,
                                OWNER = x.OWNER,
                                SUBTYPE = x.SUBTYPE,
                                HeightBase = x.HeightBase,
                                BuildingHeightH3 = x.BuildingHeightH3,
                                reinforced = x.reinforced,
                                ladderSteps = x.ladderSteps,
                                availabilityOfWorkPlatforms = x.availabilityOfWorkPlatforms,
                                equipmentsLocation = x.equipmentsLocation,
                                CenterHigh = x.CenterHigh,
                                HBA = x.HBA,
                                CIVILWITHOUTLEGCATEGORY = x.CIVILWITHOUTLEGCATEGORY,
                                Dismantle = x.Dismantle
                            }).OrderBy(x => x.Key.Name)
                            .Select(x =>
                                new { key = x.Key, value = x.ToDictionary(z => z.Key, z => z.INPUTVALUE) }
                            )
                            .Select(item => _unitOfWork.CivilWithLegsRepository.BuildDynamicSelect(item.key, item.value, propertyNamesStatic, propertyNamesDynamic));
                                int count = query.Count();

                                getEnableAttribute.Model = query;
                                string excelFilePath = ExportToExcel(query, "TLIcivilWithoutLegCapsule");

                                return new Response<string>(false, excelFilePath, null, null, (int)Helpers.Constants.ApiReturnCode.success);
                            }
                        }
                    }
                    catch (Exception err)
                    {
                        return new Response<string>(false, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
                    }
                }
                else if (Helpers.Constants.CivilType.TLIcivilWithoutLeg.ToString() == TableNameInstallation && CategoryId == 3)
                {
                    try
                    {

                        GetEnableAttribute getEnableAttribute = new GetEnableAttribute();
                        connection.Open();
                        //string storedProcedureName = "CREATE_DYNAMIC_PIVOT_WITHOUTLEG";
                        //using (OracleCommand procedureCommand = new OracleCommand(storedProcedureName, connection))
                        //{
                        //    procedureCommand.CommandType = CommandType.StoredProcedure;
                        //    procedureCommand.ExecuteNonQuery();
                        //}
                        var attActivated = _context.TLIattributeViewManagment
                            .Include(x => x.EditableManagmentView)
                            .Include(x => x.AttributeActivated)
                            .Include(x => x.DynamicAtt)
                            .Where(x => x.Enable && x.EditableManagmentView.View == "CivilWithoutLegInstallationMonopole" &&
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

                                var query = _context.MV_CIVIL_WITHOUTLEGS_VIEW
                                .Where(x =>
                                 x.CIVILWITHOUTLEGCATEGORY.ToLower() == "monopole" && !x.Dismantle)
                                .AsEnumerable().Select(x =>

                                     new
                                     {
                                         HieghFromLand = x.HieghFromLand,
                                         EquivalentSpace = x.EquivalentSpace,
                                         Support_Limited_Load = x.Support_Limited_Load,
                                         SITECODE = x.SITECODE,
                                         Id = x.Id,
                                         Name = x.Name,
                                         UpperPartLengthm = x.UpperPartLengthm,
                                         UpperPartDiameterm = x.UpperPartDiameterm,
                                         SpindlesBasePlateLengthcm = x.SpindlesBasePlateLengthcm,
                                         SpindlesBasePlateWidthcm = x.SpindlesBasePlateWidthcm,
                                         ConcreteBaseWidthm = x.ConcreteBaseWidthm,
                                         SpinBasePlateAnchorDiametercm = x.SpinBasePlateAnchorDiametercm,
                                         NumberOfCivilParts = x.NumberOfCivilParts,
                                         NumberOfLongitudinalSpindles = x.NumberOfLongitudinalSpindles,
                                         NumberOfhorizontalSpindle = x.NumberOfhorizontalSpindle,
                                         CivilLengthAboveEndOfSpindles = x.CivilLengthAboveEndOfSpindles,
                                         CivilBaseLevelFromGround = x.CivilBaseLevelFromGround,
                                         LongitudinalSpinDiameterrmm = x.LongitudinalSpinDiameterrmm,
                                         HorizontalSpindlesHBAm = x.HorizontalSpindlesHBAm,
                                         HorizontalSpindleDiametermm = x.HorizontalSpindleDiametermm,
                                         FlangeThicknesscm = x.FlangeThicknesscm,
                                         FlangeDiametercm = x.FlangeDiametercm,
                                         FlangeBoltsDiametermm = x.FlangeBoltsDiametermm,
                                         ConcreteBaseThicknessm = x.ConcreteBaseThicknessm,
                                         ConcreteBaseLengthm = x.ConcreteBaseLengthm,
                                         Civil_Remarks = x.Civil_Remarks,
                                         BottomPartLengthm = x.BottomPartLengthm,
                                         BottomPartDiameterm = x.BottomPartDiameterm,
                                         BasePlateWidthcm = x.BasePlateWidthcm,
                                         BasePlateThicknesscm = x.BasePlateThicknesscm,
                                         BasePlateLengthcm = x.BasePlateLengthcm,
                                         BPlateBoltsAnchorDiametermm = x.BPlateBoltsAnchorDiametermm,
                                         BaseBeamSectionmm = x.BaseBeamSectionmm,
                                         WindMaxLoadm2 = x.WindMaxLoadm2,
                                         Location_Height = x.Location_Height,
                                         PoType = x.PoType,
                                         PoNo = x.PoNo,
                                         PoDate = x.PoDate,
                                         HeightImplemented = x.HeightImplemented,
                                         BuildingMaxLoad = x.BuildingMaxLoad,
                                         SupportMaxLoadAfterInforcement = x.SupportMaxLoadAfterInforcement,
                                         CurrentLoads = x.CurrentLoads,
                                         WarningPercentageLoads = x.WarningPercentageLoads,
                                         Visiable_Status = x.Visiable_Status,
                                         SpaceInstallation = x.SpaceInstallation,
                                         CIVILWITHOUTLEGSLIB = x.CIVILWITHOUTLEGSLIB,
                                         OWNER = x.OWNER,
                                         SUBTYPE = x.SUBTYPE,
                                         HeightBase = x.HeightBase,
                                         BuildingHeightH3 = x.BuildingHeightH3,
                                         reinforced = x.reinforced,
                                         ladderSteps = x.ladderSteps,
                                         availabilityOfWorkPlatforms = x.availabilityOfWorkPlatforms,
                                         equipmentsLocation = x.equipmentsLocation,
                                         CenterHigh = x.CenterHigh,
                                         HBA = x.HBA,
                                         CIVILWITHOUTLEGCATEGORY = x.CIVILWITHOUTLEGCATEGORY,
                                         Dismantle = x.Dismantle

                                     }).Distinct().Select(item => _unitOfWork.CivilWithLegsRepository.BuildDynamicSelect(item, null, propertyNamesStatic, propertyNamesDynamic))
                              ;
                                int count = query.Count();

                                getEnableAttribute.Model = query;
                                string excelFilePath = ExportToExcel(query, "TLIcivilWithoutLegMonople");

                                return new Response<string>(false, excelFilePath, null, null, (int)Helpers.Constants.ApiReturnCode.success);
                            }
                            else
                            {
                                var query = _context.MV_CIVIL_WITHOUTLEGS_VIEW.Where(x =>
                                 x.CIVILWITHOUTLEGCATEGORY.ToLower() == "monopole" && !x.Dismantle).Select(x =>

                                     new
                                     {
                                         HieghFromLand = x.HieghFromLand,
                                         EquivalentSpace = x.EquivalentSpace,
                                         Support_Limited_Load = x.Support_Limited_Load,
                                         SITECODE = x.SITECODE,
                                         Id = x.Id,
                                         Name = x.Name,
                                         UpperPartLengthm = x.UpperPartLengthm,
                                         UpperPartDiameterm = x.UpperPartDiameterm,
                                         SpindlesBasePlateLengthcm = x.SpindlesBasePlateLengthcm,
                                         SpindlesBasePlateWidthcm = x.SpindlesBasePlateWidthcm,
                                         ConcreteBaseWidthm = x.ConcreteBaseWidthm,
                                         SpinBasePlateAnchorDiametercm = x.SpinBasePlateAnchorDiametercm,
                                         NumberOfCivilParts = x.NumberOfCivilParts,
                                         NumberOfLongitudinalSpindles = x.NumberOfLongitudinalSpindles,
                                         NumberOfhorizontalSpindle = x.NumberOfhorizontalSpindle,
                                         CivilLengthAboveEndOfSpindles = x.CivilLengthAboveEndOfSpindles,
                                         CivilBaseLevelFromGround = x.CivilBaseLevelFromGround,
                                         LongitudinalSpinDiameterrmm = x.LongitudinalSpinDiameterrmm,
                                         HorizontalSpindlesHBAm = x.HorizontalSpindlesHBAm,
                                         HorizontalSpindleDiametermm = x.HorizontalSpindleDiametermm,
                                         FlangeThicknesscm = x.FlangeThicknesscm,
                                         FlangeDiametercm = x.FlangeDiametercm,
                                         FlangeBoltsDiametermm = x.FlangeBoltsDiametermm,
                                         ConcreteBaseThicknessm = x.ConcreteBaseThicknessm,
                                         ConcreteBaseLengthm = x.ConcreteBaseLengthm,
                                         Civil_Remarks = x.Civil_Remarks,
                                         BottomPartLengthm = x.BottomPartLengthm,
                                         BottomPartDiameterm = x.BottomPartDiameterm,
                                         BasePlateWidthcm = x.BasePlateWidthcm,
                                         BasePlateThicknesscm = x.BasePlateThicknesscm,
                                         BasePlateLengthcm = x.BasePlateLengthcm,
                                         BPlateBoltsAnchorDiametermm = x.BPlateBoltsAnchorDiametermm,
                                         BaseBeamSectionmm = x.BaseBeamSectionmm,
                                         WindMaxLoadm2 = x.WindMaxLoadm2,
                                         Location_Height = x.Location_Height,
                                         PoType = x.PoType,
                                         PoNo = x.PoNo,
                                         PoDate = x.PoDate,
                                         HeightImplemented = x.HeightImplemented,
                                         BuildingMaxLoad = x.BuildingMaxLoad,
                                         SupportMaxLoadAfterInforcement = x.SupportMaxLoadAfterInforcement,
                                         CurrentLoads = x.CurrentLoads,
                                         WarningPercentageLoads = x.WarningPercentageLoads,
                                         Visiable_Status = x.Visiable_Status,
                                         SpaceInstallation = x.SpaceInstallation,
                                         CIVILWITHOUTLEGSLIB = x.CIVILWITHOUTLEGSLIB,
                                         OWNER = x.OWNER,
                                         SUBTYPE = x.SUBTYPE,
                                         HeightBase = x.HeightBase,
                                         BuildingHeightH3 = x.BuildingHeightH3,
                                         reinforced = x.reinforced,
                                         ladderSteps = x.ladderSteps,
                                         availabilityOfWorkPlatforms = x.availabilityOfWorkPlatforms,
                                         equipmentsLocation = x.equipmentsLocation,
                                         CenterHigh = x.CenterHigh,
                                         HBA = x.HBA,
                                         INPUTVALUE = x.INPUTVALUE,
                                         Key = x.Key,
                                         CIVILWITHOUTLEGCATEGORY = x.CIVILWITHOUTLEGCATEGORY,
                                         Dismantle = x.Dismantle

                                     }).AsEnumerable()
                            .GroupBy(x => new
                            {
                                HieghFromLand = x.HieghFromLand,
                                EquivalentSpace = x.EquivalentSpace,
                                Support_Limited_Load = x.Support_Limited_Load,
                                SITECODE = x.SITECODE,
                                Id = x.Id,
                                Name = x.Name,
                                UpperPartLengthm = x.UpperPartLengthm,
                                UpperPartDiameterm = x.UpperPartDiameterm,
                                SpindlesBasePlateLengthcm = x.SpindlesBasePlateLengthcm,
                                SpindlesBasePlateWidthcm = x.SpindlesBasePlateWidthcm,
                                ConcreteBaseWidthm = x.ConcreteBaseWidthm,
                                SpinBasePlateAnchorDiametercm = x.SpinBasePlateAnchorDiametercm,
                                NumberOfCivilParts = x.NumberOfCivilParts,
                                NumberOfLongitudinalSpindles = x.NumberOfLongitudinalSpindles,
                                NumberOfhorizontalSpindle = x.NumberOfhorizontalSpindle,
                                CivilLengthAboveEndOfSpindles = x.CivilLengthAboveEndOfSpindles,
                                CivilBaseLevelFromGround = x.CivilBaseLevelFromGround,
                                LongitudinalSpinDiameterrmm = x.LongitudinalSpinDiameterrmm,
                                HorizontalSpindlesHBAm = x.HorizontalSpindlesHBAm,
                                HorizontalSpindleDiametermm = x.HorizontalSpindleDiametermm,
                                FlangeThicknesscm = x.FlangeThicknesscm,
                                FlangeDiametercm = x.FlangeDiametercm,
                                FlangeBoltsDiametermm = x.FlangeBoltsDiametermm,
                                ConcreteBaseThicknessm = x.ConcreteBaseThicknessm,
                                ConcreteBaseLengthm = x.ConcreteBaseLengthm,
                                Civil_Remarks = x.Civil_Remarks,
                                BottomPartLengthm = x.BottomPartLengthm,
                                BottomPartDiameterm = x.BottomPartDiameterm,
                                BasePlateWidthcm = x.BasePlateWidthcm,
                                BasePlateThicknesscm = x.BasePlateThicknesscm,
                                BasePlateLengthcm = x.BasePlateLengthcm,
                                BPlateBoltsAnchorDiametermm = x.BPlateBoltsAnchorDiametermm,
                                BaseBeamSectionmm = x.BaseBeamSectionmm,
                                WindMaxLoadm2 = x.WindMaxLoadm2,
                                Location_Height = x.Location_Height,
                                PoType = x.PoType,
                                PoNo = x.PoNo,
                                PoDate = x.PoDate,
                                HeightImplemented = x.HeightImplemented,
                                BuildingMaxLoad = x.BuildingMaxLoad,
                                SupportMaxLoadAfterInforcement = x.SupportMaxLoadAfterInforcement,
                                CurrentLoads = x.CurrentLoads,
                                WarningPercentageLoads = x.WarningPercentageLoads,
                                Visiable_Status = x.Visiable_Status,
                                SpaceInstallation = x.SpaceInstallation,
                                CIVILWITHOUTLEGSLIB = x.CIVILWITHOUTLEGSLIB,
                                OWNER = x.OWNER,
                                SUBTYPE = x.SUBTYPE,
                                HeightBase = x.HeightBase,
                                BuildingHeightH3 = x.BuildingHeightH3,
                                reinforced = x.reinforced,
                                ladderSteps = x.ladderSteps,
                                availabilityOfWorkPlatforms = x.availabilityOfWorkPlatforms,
                                equipmentsLocation = x.equipmentsLocation,
                                CenterHigh = x.CenterHigh,
                                HBA = x.HBA,
                                CIVILWITHOUTLEGCATEGORY = x.CIVILWITHOUTLEGCATEGORY,
                                Dismantle = x.Dismantle
                            }).OrderBy(x => x.Key.Name)
                            .Select(x =>
                                new { key = x.Key, value = x.ToDictionary(z => z.Key, z => z.INPUTVALUE) }
                            )
                            .Select(item => _unitOfWork.CivilWithLegsRepository.BuildDynamicSelect(item.key, item.value, propertyNamesStatic, propertyNamesDynamic));
                                int count = query.Count();

                                getEnableAttribute.Model = query;
                                string excelFilePath = ExportToExcel(query, "TLIcivilWithoutLegMonople");

                                return new Response<string>(false, excelFilePath, null, null, (int)Helpers.Constants.ApiReturnCode.success);
                            }
                        }
                        if (propertyNamesDynamic.Count == 0)
                        {

                            var query = _context.MV_CIVIL_WITHOUTLEGS_VIEW
                            .Where(x => x.SITECODE.ToLower() == SiteCode.ToLower() &&
                             x.CIVILWITHOUTLEGCATEGORY.ToLower() == "monopole" && !x.Dismantle)
                            .AsEnumerable().Select(x =>

                                 new
                                 {
                                     HieghFromLand = x.HieghFromLand,
                                     EquivalentSpace = x.EquivalentSpace,
                                     Support_Limited_Load = x.Support_Limited_Load,
                                     SITECODE = x.SITECODE,
                                     Id = x.Id,
                                     Name = x.Name,
                                     UpperPartLengthm = x.UpperPartLengthm,
                                     UpperPartDiameterm = x.UpperPartDiameterm,
                                     SpindlesBasePlateLengthcm = x.SpindlesBasePlateLengthcm,
                                     SpindlesBasePlateWidthcm = x.SpindlesBasePlateWidthcm,
                                     ConcreteBaseWidthm = x.ConcreteBaseWidthm,
                                     SpinBasePlateAnchorDiametercm = x.SpinBasePlateAnchorDiametercm,
                                     NumberOfCivilParts = x.NumberOfCivilParts,
                                     NumberOfLongitudinalSpindles = x.NumberOfLongitudinalSpindles,
                                     NumberOfhorizontalSpindle = x.NumberOfhorizontalSpindle,
                                     CivilLengthAboveEndOfSpindles = x.CivilLengthAboveEndOfSpindles,
                                     CivilBaseLevelFromGround = x.CivilBaseLevelFromGround,
                                     LongitudinalSpinDiameterrmm = x.LongitudinalSpinDiameterrmm,
                                     HorizontalSpindlesHBAm = x.HorizontalSpindlesHBAm,
                                     HorizontalSpindleDiametermm = x.HorizontalSpindleDiametermm,
                                     FlangeThicknesscm = x.FlangeThicknesscm,
                                     FlangeDiametercm = x.FlangeDiametercm,
                                     FlangeBoltsDiametermm = x.FlangeBoltsDiametermm,
                                     ConcreteBaseThicknessm = x.ConcreteBaseThicknessm,
                                     ConcreteBaseLengthm = x.ConcreteBaseLengthm,
                                     Civil_Remarks = x.Civil_Remarks,
                                     BottomPartLengthm = x.BottomPartLengthm,
                                     BottomPartDiameterm = x.BottomPartDiameterm,
                                     BasePlateWidthcm = x.BasePlateWidthcm,
                                     BasePlateThicknesscm = x.BasePlateThicknesscm,
                                     BasePlateLengthcm = x.BasePlateLengthcm,
                                     BPlateBoltsAnchorDiametermm = x.BPlateBoltsAnchorDiametermm,
                                     BaseBeamSectionmm = x.BaseBeamSectionmm,
                                     WindMaxLoadm2 = x.WindMaxLoadm2,
                                     Location_Height = x.Location_Height,
                                     PoType = x.PoType,
                                     PoNo = x.PoNo,
                                     PoDate = x.PoDate,
                                     HeightImplemented = x.HeightImplemented,
                                     BuildingMaxLoad = x.BuildingMaxLoad,
                                     SupportMaxLoadAfterInforcement = x.SupportMaxLoadAfterInforcement,
                                     CurrentLoads = x.CurrentLoads,
                                     WarningPercentageLoads = x.WarningPercentageLoads,
                                     Visiable_Status = x.Visiable_Status,
                                     SpaceInstallation = x.SpaceInstallation,
                                     CIVILWITHOUTLEGSLIB = x.CIVILWITHOUTLEGSLIB,
                                     OWNER = x.OWNER,
                                     SUBTYPE = x.SUBTYPE,
                                     HeightBase = x.HeightBase,
                                     BuildingHeightH3 = x.BuildingHeightH3,
                                     reinforced = x.reinforced,
                                     ladderSteps = x.ladderSteps,
                                     availabilityOfWorkPlatforms = x.availabilityOfWorkPlatforms,
                                     equipmentsLocation = x.equipmentsLocation,
                                     CenterHigh = x.CenterHigh,
                                     HBA = x.HBA,
                                     CIVILWITHOUTLEGCATEGORY = x.CIVILWITHOUTLEGCATEGORY,
                                     Dismantle = x.Dismantle

                                 }).Distinct().Select(item => _unitOfWork.CivilWithLegsRepository.BuildDynamicSelect(item, null, propertyNamesStatic, propertyNamesDynamic))
                          ;
                            int count = query.Count();

                            getEnableAttribute.Model = query;
                            string excelFilePath = ExportToExcel(query, "TLIcivilWithoutLegMonople");

                            return new Response<string>(false, excelFilePath, null, null, (int)Helpers.Constants.ApiReturnCode.success);
                        }
                        else
                        {
                            var query = _context.MV_CIVIL_WITHOUTLEGS_VIEW.Where(x => x.SITECODE.ToLower() == SiteCode.ToLower() &&
                             x.CIVILWITHOUTLEGCATEGORY.ToLower() == "monopole" && !x.Dismantle).Select(x =>

                                 new
                                 {
                                     HieghFromLand = x.HieghFromLand,
                                     EquivalentSpace = x.EquivalentSpace,
                                     Support_Limited_Load = x.Support_Limited_Load,
                                     SITECODE = x.SITECODE,
                                     Id = x.Id,
                                     Name = x.Name,
                                     UpperPartLengthm = x.UpperPartLengthm,
                                     UpperPartDiameterm = x.UpperPartDiameterm,
                                     SpindlesBasePlateLengthcm = x.SpindlesBasePlateLengthcm,
                                     SpindlesBasePlateWidthcm = x.SpindlesBasePlateWidthcm,
                                     ConcreteBaseWidthm = x.ConcreteBaseWidthm,
                                     SpinBasePlateAnchorDiametercm = x.SpinBasePlateAnchorDiametercm,
                                     NumberOfCivilParts = x.NumberOfCivilParts,
                                     NumberOfLongitudinalSpindles = x.NumberOfLongitudinalSpindles,
                                     NumberOfhorizontalSpindle = x.NumberOfhorizontalSpindle,
                                     CivilLengthAboveEndOfSpindles = x.CivilLengthAboveEndOfSpindles,
                                     CivilBaseLevelFromGround = x.CivilBaseLevelFromGround,
                                     LongitudinalSpinDiameterrmm = x.LongitudinalSpinDiameterrmm,
                                     HorizontalSpindlesHBAm = x.HorizontalSpindlesHBAm,
                                     HorizontalSpindleDiametermm = x.HorizontalSpindleDiametermm,
                                     FlangeThicknesscm = x.FlangeThicknesscm,
                                     FlangeDiametercm = x.FlangeDiametercm,
                                     FlangeBoltsDiametermm = x.FlangeBoltsDiametermm,
                                     ConcreteBaseThicknessm = x.ConcreteBaseThicknessm,
                                     ConcreteBaseLengthm = x.ConcreteBaseLengthm,
                                     Civil_Remarks = x.Civil_Remarks,
                                     BottomPartLengthm = x.BottomPartLengthm,
                                     BottomPartDiameterm = x.BottomPartDiameterm,
                                     BasePlateWidthcm = x.BasePlateWidthcm,
                                     BasePlateThicknesscm = x.BasePlateThicknesscm,
                                     BasePlateLengthcm = x.BasePlateLengthcm,
                                     BPlateBoltsAnchorDiametermm = x.BPlateBoltsAnchorDiametermm,
                                     BaseBeamSectionmm = x.BaseBeamSectionmm,
                                     WindMaxLoadm2 = x.WindMaxLoadm2,
                                     Location_Height = x.Location_Height,
                                     PoType = x.PoType,
                                     PoNo = x.PoNo,
                                     PoDate = x.PoDate,
                                     HeightImplemented = x.HeightImplemented,
                                     BuildingMaxLoad = x.BuildingMaxLoad,
                                     SupportMaxLoadAfterInforcement = x.SupportMaxLoadAfterInforcement,
                                     CurrentLoads = x.CurrentLoads,
                                     WarningPercentageLoads = x.WarningPercentageLoads,
                                     Visiable_Status = x.Visiable_Status,
                                     SpaceInstallation = x.SpaceInstallation,
                                     CIVILWITHOUTLEGSLIB = x.CIVILWITHOUTLEGSLIB,
                                     OWNER = x.OWNER,
                                     SUBTYPE = x.SUBTYPE,
                                     HeightBase = x.HeightBase,
                                     BuildingHeightH3 = x.BuildingHeightH3,
                                     reinforced = x.reinforced,
                                     ladderSteps = x.ladderSteps,
                                     availabilityOfWorkPlatforms = x.availabilityOfWorkPlatforms,
                                     equipmentsLocation = x.equipmentsLocation,
                                     CenterHigh = x.CenterHigh,
                                     HBA = x.HBA,
                                     INPUTVALUE = x.INPUTVALUE,
                                     Key = x.Key,
                                     CIVILWITHOUTLEGCATEGORY = x.CIVILWITHOUTLEGCATEGORY,
                                     Dismantle = x.Dismantle

                                 }).AsEnumerable()
                        .GroupBy(x => new
                        {
                            HieghFromLand = x.HieghFromLand,
                            EquivalentSpace = x.EquivalentSpace,
                            Support_Limited_Load = x.Support_Limited_Load,
                            SITECODE = x.SITECODE,
                            Id = x.Id,
                            Name = x.Name,
                            UpperPartLengthm = x.UpperPartLengthm,
                            UpperPartDiameterm = x.UpperPartDiameterm,
                            SpindlesBasePlateLengthcm = x.SpindlesBasePlateLengthcm,
                            SpindlesBasePlateWidthcm = x.SpindlesBasePlateWidthcm,
                            ConcreteBaseWidthm = x.ConcreteBaseWidthm,
                            SpinBasePlateAnchorDiametercm = x.SpinBasePlateAnchorDiametercm,
                            NumberOfCivilParts = x.NumberOfCivilParts,
                            NumberOfLongitudinalSpindles = x.NumberOfLongitudinalSpindles,
                            NumberOfhorizontalSpindle = x.NumberOfhorizontalSpindle,
                            CivilLengthAboveEndOfSpindles = x.CivilLengthAboveEndOfSpindles,
                            CivilBaseLevelFromGround = x.CivilBaseLevelFromGround,
                            LongitudinalSpinDiameterrmm = x.LongitudinalSpinDiameterrmm,
                            HorizontalSpindlesHBAm = x.HorizontalSpindlesHBAm,
                            HorizontalSpindleDiametermm = x.HorizontalSpindleDiametermm,
                            FlangeThicknesscm = x.FlangeThicknesscm,
                            FlangeDiametercm = x.FlangeDiametercm,
                            FlangeBoltsDiametermm = x.FlangeBoltsDiametermm,
                            ConcreteBaseThicknessm = x.ConcreteBaseThicknessm,
                            ConcreteBaseLengthm = x.ConcreteBaseLengthm,
                            Civil_Remarks = x.Civil_Remarks,
                            BottomPartLengthm = x.BottomPartLengthm,
                            BottomPartDiameterm = x.BottomPartDiameterm,
                            BasePlateWidthcm = x.BasePlateWidthcm,
                            BasePlateThicknesscm = x.BasePlateThicknesscm,
                            BasePlateLengthcm = x.BasePlateLengthcm,
                            BPlateBoltsAnchorDiametermm = x.BPlateBoltsAnchorDiametermm,
                            BaseBeamSectionmm = x.BaseBeamSectionmm,
                            WindMaxLoadm2 = x.WindMaxLoadm2,
                            Location_Height = x.Location_Height,
                            PoType = x.PoType,
                            PoNo = x.PoNo,
                            PoDate = x.PoDate,
                            HeightImplemented = x.HeightImplemented,
                            BuildingMaxLoad = x.BuildingMaxLoad,
                            SupportMaxLoadAfterInforcement = x.SupportMaxLoadAfterInforcement,
                            CurrentLoads = x.CurrentLoads,
                            WarningPercentageLoads = x.WarningPercentageLoads,
                            Visiable_Status = x.Visiable_Status,
                            SpaceInstallation = x.SpaceInstallation,
                            CIVILWITHOUTLEGSLIB = x.CIVILWITHOUTLEGSLIB,
                            OWNER = x.OWNER,
                            SUBTYPE = x.SUBTYPE,
                            HeightBase = x.HeightBase,
                            BuildingHeightH3 = x.BuildingHeightH3,
                            reinforced = x.reinforced,
                            ladderSteps = x.ladderSteps,
                            availabilityOfWorkPlatforms = x.availabilityOfWorkPlatforms,
                            equipmentsLocation = x.equipmentsLocation,
                            CenterHigh = x.CenterHigh,
                            HBA = x.HBA,
                            CIVILWITHOUTLEGCATEGORY = x.CIVILWITHOUTLEGCATEGORY,
                            Dismantle = x.Dismantle
                        }).OrderBy(x => x.Key.Name)
                        .Select(x =>
                            new { key = x.Key, value = x.ToDictionary(z => z.Key, z => z.INPUTVALUE) }
                        )
                        .Select(item => _unitOfWork.CivilWithLegsRepository.BuildDynamicSelect(item.key, item.value, propertyNamesStatic, propertyNamesDynamic));
                            int count = query.Count();

                            getEnableAttribute.Model = query;
                            string excelFilePath = ExportToExcel(query, "TLIcivilWithoutLegMonople");

                            return new Response<string>(false, excelFilePath, null, null, (int)Helpers.Constants.ApiReturnCode.success);
                        }


                    }
                    catch (Exception err)
                    {
                        return new Response<string>(false, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
                    }
                }
                else if (Helpers.Constants.CivilType.TLIcivilNonSteel.ToString() == TableNameInstallation)
                {
                    try
                    {

                        GetEnableAttribute getEnableAttribute = new GetEnableAttribute();
                        connection.Open();
                        //string storedProcedureName = "CREATE_DYNAMIC_PIVOT_NONSTEEL ";
                        //using (OracleCommand procedureCommand = new OracleCommand(storedProcedureName, connection))
                        //{
                        //    procedureCommand.CommandType = CommandType.StoredProcedure;
                        //    procedureCommand.ExecuteNonQuery();
                        //}
                        var attActivated = _context.TLIattributeViewManagment.Include(x => x.EditableManagmentView).Include(x => x.AttributeActivated)
                            .Include(x => x.DynamicAtt).Where(x => x.Enable && x.EditableManagmentView.View == "CivilNonSteelInstallation" &&
                            ((x.AttributeActivatedId != null && x.AttributeActivated.enable) || (x.DynamicAttId != null && !x.DynamicAtt.disable)))
                            .Select(x => new { attribute = x.AttributeActivated.Key, dynamic = x.DynamicAtt.Key, dataType = x.DynamicAtt != null ? x.DynamicAtt.DataType.Name.ToString() : x.AttributeActivated.DataType.ToString() })
                            .OrderByDescending(x => x.attribute.ToLower().StartsWith("name"))
                                .ThenBy(x => x.attribute == null)
                                .ThenBy(x => x.attribute)
                                .ToList(); ;
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
                                var query = _context.MV_CIVIL_NONSTEEL_VIEW.Where(x => !x.Dismantle).AsEnumerable()
                              .Select(item => _unitOfWork.CivilWithLegsRepository.BuildDynamicSelect(item, null, propertyNamesStatic, propertyNamesDynamic));
                                int count = query.Count();
                                getEnableAttribute.Model = query;
                                string excelFilePath = ExportToExcel(query, "TLIcivilNonSteel");

                                return new Response<string>(false, excelFilePath, null, null, (int)Helpers.Constants.ApiReturnCode.success);

                            }
                            else
                            {
                                var query = _context.MV_CIVIL_NONSTEEL_VIEW.Where(x =>
                                   !x.Dismantle).AsEnumerable()
                                .GroupBy(x => new
                                {
                                    SITECODE = x.SITECODE,
                                    Id = x.Id,
                                    Name = x.Name,
                                    CurrentLoads = x.CurrentLoads,
                                    SpaceInstallation = x.SpaceInstallation,
                                    CIVILNONSTEELLIBRARY = x.CIVILNONSTEELLIBRARY,
                                    OWNER = x.OWNER,
                                    SUPPORTTYPEIMPLEMENTED = x.SUPPORTTYPEIMPLEMENTED,
                                    LOCATIONTYPE = x.LOCATIONTYPE,
                                    locationHeight = x.locationHeight,
                                    BuildingMaxLoad = x.BuildingMaxLoad,
                                    CivilSupportCurrentLoad = x.CivilSupportCurrentLoad,
                                    H2Height = x.H2Height,
                                    CenterHigh = x.CenterHigh,
                                    HBA = x.HBA,
                                    HieghFromLand = x.HieghFromLand,
                                    EquivalentSpace = x.EquivalentSpace,
                                    Support_Limited_Load = x.Support_Limited_Load,
                                })
                                .Select(x => new { key = x.Key, value = x.ToDictionary(z => z.Key, z => z.INPUTVALUE) })
                                .Select(item => _unitOfWork.CivilWithLegsRepository.BuildDynamicSelect(item.key, item.value, propertyNamesStatic, propertyNamesDynamic));

                                int count = query.Count();

                                getEnableAttribute.Model = query;
                                string excelFilePath = ExportToExcel(query, "TLIcivilNonSteel");

                                return new Response<string>(false, excelFilePath, null, null, (int)Helpers.Constants.ApiReturnCode.success);

                            }
                        }
                        if (propertyNamesDynamic.Count == 0)
                        {

                            var query = _context.MV_CIVIL_NONSTEEL_VIEW.Where(x => x.SITECODE.ToLower() == SiteCode.ToLower()
                             && !x.Dismantle).AsEnumerable()
                            .Select(item => _unitOfWork.CivilWithLegsRepository.BuildDynamicSelect(item, null, propertyNamesStatic, propertyNamesDynamic));
                            int count = query.Count();
                            getEnableAttribute.Model = query;
                            string excelFilePath = ExportToExcel(query, "TLIcivilNonSteel");

                            return new Response<string>(false, excelFilePath, null, null, (int)Helpers.Constants.ApiReturnCode.success);

                        }
                        else
                        {
                            var query = _context.MV_CIVIL_NONSTEEL_VIEW.Where(x => x.SITECODE.ToLower() == SiteCode.ToLower()
                             && !x.Dismantle).AsEnumerable()
                           .GroupBy(x => new
                           {
                               SITECODE = x.SITECODE,
                               Id = x.Id,
                               Name = x.Name,
                               CurrentLoads = x.CurrentLoads,
                               SpaceInstallation = x.SpaceInstallation,
                               CIVILNONSTEELLIBRARY = x.CIVILNONSTEELLIBRARY,
                               OWNER = x.OWNER,
                               SUPPORTTYPEIMPLEMENTED = x.SUPPORTTYPEIMPLEMENTED,
                               LOCATIONTYPE = x.LOCATIONTYPE,
                               locationHeight = x.locationHeight,
                               BuildingMaxLoad = x.BuildingMaxLoad,
                               CivilSupportCurrentLoad = x.CivilSupportCurrentLoad,
                               H2Height = x.H2Height,
                               CenterHigh = x.CenterHigh,
                               HBA = x.HBA,
                               HieghFromLand = x.HieghFromLand,
                               EquivalentSpace = x.EquivalentSpace,
                               Support_Limited_Load = x.Support_Limited_Load,
                           })
                           .Select(x => new { key = x.Key, value = x.ToDictionary(z => z.Key, z => z.INPUTVALUE) })
                           .Select(item => _unitOfWork.CivilWithLegsRepository.BuildDynamicSelect(item.key, item.value, propertyNamesStatic, propertyNamesDynamic));

                            int count = query.Count();

                            getEnableAttribute.Model = query;


                            string excelFilePath = ExportToExcel(query, "TLIcivilNonSteel");

                            return new Response<string>(false, excelFilePath, null, null, (int)Helpers.Constants.ApiReturnCode.success);

                        }

                    }
                    catch (Exception err)
                    {
                        return new Response<string>(false, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
                    }
                }
                else if (Helpers.Constants.LoadSubType.TLIsideArm.ToString() == TableNameInstallation)
                {
                    try
                    {
                        GetEnableAttribute getEnableAttribute = new GetEnableAttribute();
                        connection.Open();
                        //string storedProcedureName = "CREATE_DYNAMIC_PIVOT_SIDEARM ";
                        //using (OracleCommand procedureCommand = new OracleCommand(storedProcedureName, connection))
                        //{
                        //    procedureCommand.CommandType = CommandType.StoredProcedure;
                        //    procedureCommand.ExecuteNonQuery();
                        //}
                        var attActivated = _context.TLIattributeViewManagment.Include(x => x.EditableManagmentView).Include(x => x.AttributeActivated)
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
                        propertyNamesStatic.Add("SITECODE");
                        propertyNamesStatic.Add("CIVILNAME");
                        propertyNamesStatic.Add("CIVILID");
                        propertyNamesStatic.Add("FIRST_LEG");
                        propertyNamesStatic.Add("FIRST_LEG_ID");
                        propertyNamesStatic.Add("SECOND_LEG");
                        propertyNamesStatic.Add("SECOND_LEG_ID");
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

                                var query = _context.MV_SIDEARM_VIEW.Where(x => !x.Dismantle).AsEnumerable()
                                .Select(item => _unitOfWork.CivilWithLegsRepository.BuildDynamicSelect(item, null, propertyNamesStatic, propertyNamesDynamic));
                                int count = query.Count();
                                getEnableAttribute.Model = query;
                                string excelFilePath = ExportToExcel(query, "TLIsideArm");

                                return new Response<string>(false, excelFilePath, null, null, (int)Helpers.Constants.ApiReturnCode.success);

                            }
                            else
                            {
                                var query = _context.MV_SIDEARM_VIEW.Where(x => !x.Dismantle).AsEnumerable()
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
                                   FIRST_LEG_ID = x.FIRST_LEG_ID,
                                   SECOND_LEG_ID = x.SECOND_LEG_ID,
                                   ALLCIVIL_ID = x.ALLCIVIL_ID,
                                   Dismantle = x.Dismantle

                               })
                               .Select(x => new { key = x.Key, value = x.ToDictionary(z => z.Key, z => z.INPUTVALUE) })
                               .Select(item => _unitOfWork.CivilWithLegsRepository.BuildDynamicSelect(item.key, item.value, propertyNamesStatic, propertyNamesDynamic));

                                int count = query.Count();

                                getEnableAttribute.Model = query;
                                string excelFilePath = ExportToExcel(query, "TLIsideArm");

                                return new Response<string>(false, excelFilePath, null, null, (int)Helpers.Constants.ApiReturnCode.success);
                            }
                        }
                        if (propertyNamesDynamic.Count == 0)
                        {

                            var query = _context.MV_SIDEARM_VIEW.Where(x => x.SITECODE.ToLower() == SiteCode.ToLower() && !x.Dismantle).AsEnumerable()
                            .Select(item => _unitOfWork.CivilWithLegsRepository.BuildDynamicSelect(item, null, propertyNamesStatic, propertyNamesDynamic));
                            int count = query.Count();
                            getEnableAttribute.Model = query;
                            string excelFilePath = ExportToExcel(query, "TLIsideArm");

                            return new Response<string>(false, excelFilePath, null, null, (int)Helpers.Constants.ApiReturnCode.success);
                        }
                        else
                        {
                            var query = _context.MV_SIDEARM_VIEW.Where(x => x.SITECODE.ToLower() == SiteCode.ToLower() && !x.Dismantle).AsEnumerable()
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
                               FIRST_LEG_ID = x.FIRST_LEG_ID,
                               SECOND_LEG_ID = x.SECOND_LEG_ID,
                               ALLCIVIL_ID = x.ALLCIVIL_ID,
                               Dismantle = x.Dismantle

                           })
                           .Select(x => new { key = x.Key, value = x.ToDictionary(z => z.Key, z => z.INPUTVALUE) })
                           .Select(item => _unitOfWork.CivilWithLegsRepository.BuildDynamicSelect(item.key, item.value, propertyNamesStatic, propertyNamesDynamic));

                            int count = query.Count();

                            getEnableAttribute.Model = query;
                            string excelFilePath = ExportToExcel(query, "TLIsideArm");

                            return new Response<string>(false, excelFilePath, null, null, (int)Helpers.Constants.ApiReturnCode.success);
                        }
                    }
                    catch (Exception err)
                    {
                        return new Response<string>(false, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
                    }
                }
                else if (Helpers.Constants.LoadSubType.TLIloadOther.ToString() == TableNameInstallation)
                {
                    try
                    {
                        GetEnableAttribute getEnableAttribute = new GetEnableAttribute();
                        connection.Open();
                        //string storedProcedureName = "CREATE_DYNAMIC_PIVOT_MWDISH";
                        //using (OracleCommand procedureCommand = new OracleCommand(storedProcedureName, connection))
                        //{
                        //    procedureCommand.CommandType = CommandType.StoredProcedure;
                        //    procedureCommand.ExecuteNonQuery();
                        //}
                        var attActivated = _context.TLIattributeViewManagment.Include(x => x.EditableManagmentView).Include(x => x.AttributeActivated)
                            .Include(x => x.DynamicAtt).Where(x => x.Enable && x.EditableManagmentView.View == "OtherLoadInstallation" &&
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
                        propertyNamesStatic.Add("SiteCode");
                        propertyNamesStatic.Add("LEG_NAME");
                        propertyNamesStatic.Add("CIVILNAME");
                        propertyNamesStatic.Add("CIVIL_ID");
                        propertyNamesStatic.Add("SIDEARMNAME");
                        propertyNamesStatic.Add("SIDEARM_ID");
                        propertyNamesStatic.Add("ALLCIVILINST_ID");
                        propertyNamesStatic.Add("LEG_ID");

                        if (SiteCode == null)
                        {
                            if (propertyNamesDynamic.Count == 0)
                            {
                                var query = _context.MV_LOAD_OTHER_VIEW.Where(x => !x.Dismantle).AsEnumerable()
                               .Select(item => _unitOfWork.CivilWithLegsRepository.BuildDynamicSelect(item, null, propertyNamesStatic, propertyNamesDynamic));
                                int count = query.Count();
                                getEnableAttribute.Model = query;
                                string excelFilePath = ExportToExcel(query, "TLIloadOther");

                                return new Response<string>(false, excelFilePath, null, null, (int)Helpers.Constants.ApiReturnCode.success);
                            }
                            else
                            {
                                var query = _context.MV_LOAD_OTHER_VIEW.Where(x => !x.Dismantle).AsEnumerable()
                               .GroupBy(x => new
                               {
                                   SiteCode = x.SiteCode,
                                   Id = x.Id,
                                   Name = x.Name,
                                   Azimuth = x.Azimuth,
                                   Notes = x.Notes,
                                   HeightBase = x.HeightBase,
                                   HeightLand = x.HeightLand,
                                   SerialNumber = x.SerialNumber,
                                   HieghFromLand = x.HieghFromLand,
                                   SpaceInstallation = x.SpaceInstallation,
                                   LOADOTHERLIBRARY = x.LOADOTHERLIBRARY,
                                   INSTALLATIONPLACE = x.INSTALLATIONPLACE,
                                   CenterHigh = x.CenterHigh,
                                   HBA = x.HBA,
                                   EquivalentSpace = x.EquivalentSpace,
                                   Dismantle = x.Dismantle,
                                   LEG_NAME = x.LEG_NAME,
                                   CIVILNAME = x.CIVILNAME,
                                   CIVIL_ID = x.CIVIL_ID,
                                   SIDEARMNAME = x.SIDEARMNAME,
                                   SIDEARM_ID = x.SIDEARM_ID,
                                   ALLCIVILINST_ID = x.ALLCIVILINST_ID,
                                   LEG_ID = x.LEG_ID,
                                   SideArmSec_Name = x.SideArmSec_Name,
                                   SideArmSec_Id = x.SideArmSec_Id


                               })
                               .Select(x => new { key = x.Key, value = x.ToDictionary(z => z.Key, z => z.INPUTVALUE) })
                               .Select(item => _unitOfWork.CivilWithLegsRepository.BuildDynamicSelect(item.key, item.value, propertyNamesStatic, propertyNamesDynamic));

                                int count = query.Count();

                                getEnableAttribute.Model = query;
                                string excelFilePath = ExportToExcel(query, "TLIloadOther");

                                return new Response<string>(false, excelFilePath, null, null, (int)Helpers.Constants.ApiReturnCode.success);
                            }
                        }
                        if (propertyNamesDynamic.Count == 0)
                        {
                            var query = _context.MV_LOAD_OTHER_VIEW.Where(x => x.SiteCode.ToLower() == SiteCode.ToLower() && !x.Dismantle).AsEnumerable()
                            .Select(item => _unitOfWork.CivilWithLegsRepository.BuildDynamicSelect(item, null, propertyNamesStatic, propertyNamesDynamic));
                            int count = query.Count();
                            getEnableAttribute.Model = query;
                            string excelFilePath = ExportToExcel(query, "TLIloadOther");

                            return new Response<string>(false, excelFilePath, null, null, (int)Helpers.Constants.ApiReturnCode.success);
                        }
                        else
                        {
                            var query = _context.MV_LOAD_OTHER_VIEW.Where(x => x.SiteCode.ToLower() == SiteCode.ToLower() && !x.Dismantle).AsEnumerable()
                           .GroupBy(x => new
                           {
                               SiteCode = x.SiteCode,
                               Id = x.Id,
                               Name = x.Name,
                               Azimuth = x.Azimuth,
                               Notes = x.Notes,
                               HeightBase = x.HeightBase,
                               HeightLand = x.HeightLand,
                               SerialNumber = x.SerialNumber,
                               HieghFromLand = x.HieghFromLand,
                               SpaceInstallation = x.SpaceInstallation,
                               LOADOTHERLIBRARY = x.LOADOTHERLIBRARY,
                               INSTALLATIONPLACE = x.INSTALLATIONPLACE,
                               CenterHigh = x.CenterHigh,
                               HBA = x.HBA,
                               EquivalentSpace = x.EquivalentSpace,
                               Dismantle = x.Dismantle,
                               LEG_NAME = x.LEG_NAME,
                               CIVILNAME = x.CIVILNAME,
                               CIVIL_ID = x.CIVIL_ID,
                               SIDEARMNAME = x.SIDEARMNAME,
                               SIDEARM_ID = x.SIDEARM_ID,
                               ALLCIVILINST_ID = x.ALLCIVILINST_ID,
                               LEG_ID = x.LEG_ID,
                               SideArmSec_Name = x.SideArmSec_Name,
                               SideArmSec_Id = x.SideArmSec_Id


                           })
                           .Select(x => new { key = x.Key, value = x.ToDictionary(z => z.Key, z => z.INPUTVALUE) })
                           .Select(item => _unitOfWork.CivilWithLegsRepository.BuildDynamicSelect(item.key, item.value, propertyNamesStatic, propertyNamesDynamic));

                            int count = query.Count();

                            getEnableAttribute.Model = query;
                            string excelFilePath = ExportToExcel(query, "TLIloadOther");

                            return new Response<string>(false, excelFilePath, null, null, (int)Helpers.Constants.ApiReturnCode.success);
                        }
                    }
                    catch (Exception err)
                    {
                        return new Response<string>(false, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
                    }
                }
                else if (Helpers.Constants.LoadSubType.TLIpower.ToString() == TableNameInstallation)
                {
                    try
                    {
                        GetEnableAttribute getEnableAttribute = new GetEnableAttribute();
                        connection.Open();
                        var attActivated = _context.TLIattributeViewManagment.Include(x => x.EditableManagmentView).Include(x => x.AttributeActivated)
                            .Include(x => x.DynamicAtt).Where(x => x.Enable && x.EditableManagmentView.View == "PowerInstallation" &&
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
                        propertyNamesStatic.Add("CIVIL_ID");
                        propertyNamesStatic.Add("SIDEARMNAME");
                        propertyNamesStatic.Add("SIDEARM_ID");
                        propertyNamesStatic.Add("ALLCIVILINST_ID");
                        propertyNamesStatic.Add("LEGID");
                        propertyNamesStatic.Add("LEGNAME");
                        propertyNamesStatic.Add("SiteCode");
                        if (SiteCode == null)
                        {
                            if (propertyNamesDynamic.Count == 0)
                            {
                                var query = _context.MV_POWER_VIEW.Where(x => !x.Dismantle).AsEnumerable()
                                .Select(item => _unitOfWork.CivilWithLegsRepository.BuildDynamicSelect(item, null, propertyNamesStatic, propertyNamesDynamic));
                                int count = query.Count();
                                getEnableAttribute.Model = query;
                                string excelFilePath = ExportToExcel(query, "TLIpower");

                                return new Response<string>(false, excelFilePath, null, null, (int)Helpers.Constants.ApiReturnCode.success);
                            }
                            else
                            {
                                var query = _context.MV_POWER_VIEW.Where(x => !x.Dismantle).AsEnumerable()
                               .GroupBy(x => new
                               {
                                   SiteCode = x.SiteCode,
                                   Id = x.Id,
                                   Name = x.Name,
                                   Azimuth = x.Azimuth,
                                   Notes = x.Notes,
                                   VisibleStatus = x.VisibleStatus,
                                   POWERTYPE = x.POWERTYPE,
                                   SerialNumber = x.SerialNumber,
                                   HBA = x.HBA,
                                   SpaceInstallation = x.SpaceInstallation,
                                   HeightBase = x.HeightBase,
                                   HeightLand = x.HeightLand,
                                   INSTALLATIONPLACE = x.INSTALLATIONPLACE,
                                   POWERLIBRARY = x.POWERLIBRARY,
                                   Dismantle = x.Dismantle,
                                   CenterHigh = x.CenterHigh,
                                   HieghFromLand = x.HieghFromLand,
                                   EquivalentSpace = x.EquivalentSpace,
                                   LEGNAME = x.LEGNAME,
                                   CIVILNAME = x.CIVILNAME,
                                   CIVIL_ID = x.CIVIL_ID,
                                   SIDEARMNAME = x.SIDEARMNAME,
                                   SIDEARM_ID = x.SIDEARM_ID,
                                   ALLCIVILINST_ID = x.ALLCIVILINST_ID,
                                   LEGID = x.LEGID,
                                   OWNER = x.OWNER,
                                   Height = x.Height


                               })
                               .Select(x => new { key = x.Key, value = x.ToDictionary(z => z.Key, z => z.INPUTVALUE) })
                               .Select(item => _unitOfWork.CivilWithLegsRepository.BuildDynamicSelect(item.key, item.value, propertyNamesStatic, propertyNamesDynamic));

                                int count = query.Count();

                                getEnableAttribute.Model = query;
                                string excelFilePath = ExportToExcel(query, "TLIpower");

                                return new Response<string>(false, excelFilePath, null, null, (int)Helpers.Constants.ApiReturnCode.success);

                            }
                        }
                        if (propertyNamesDynamic.Count == 0)
                        {
                            var query = _context.MV_POWER_VIEW.Where(x => x.SiteCode.ToLower() == SiteCode.ToLower() && !x.Dismantle).AsEnumerable()
                            .Select(item => _unitOfWork.CivilWithLegsRepository.BuildDynamicSelect(item, null, propertyNamesStatic, propertyNamesDynamic));
                            int count = query.Count();
                            getEnableAttribute.Model = query;
                            string excelFilePath = ExportToExcel(query, "TLIpower");

                            return new Response<string>(false, excelFilePath, null, null, (int)Helpers.Constants.ApiReturnCode.success);
                        }
                        else
                        {
                            var query = _context.MV_POWER_VIEW.Where(x => x.SiteCode.ToLower() == SiteCode.ToLower() && !x.Dismantle).AsEnumerable()
                           .GroupBy(x => new
                           {
                               SiteCode = x.SiteCode,
                               Id = x.Id,
                               Name = x.Name,
                               Azimuth = x.Azimuth,
                               Notes = x.Notes,
                               VisibleStatus = x.VisibleStatus,
                               POWERTYPE = x.POWERTYPE,
                               SerialNumber = x.SerialNumber,
                               HBA = x.HBA,
                               SpaceInstallation = x.SpaceInstallation,
                               HeightBase = x.HeightBase,
                               HeightLand = x.HeightLand,
                               INSTALLATIONPLACE = x.INSTALLATIONPLACE,
                               POWERLIBRARY = x.POWERLIBRARY,
                               Dismantle = x.Dismantle,
                               CenterHigh = x.CenterHigh,
                               HieghFromLand = x.HieghFromLand,
                               EquivalentSpace = x.EquivalentSpace,
                               LEGNAME = x.LEGNAME,
                               CIVILNAME = x.CIVILNAME,
                               CIVIL_ID = x.CIVIL_ID,
                               SIDEARMNAME = x.SIDEARMNAME,
                               SIDEARM_ID = x.SIDEARM_ID,
                               ALLCIVILINST_ID = x.ALLCIVILINST_ID,
                               LEGID = x.LEGID,
                               OWNER = x.OWNER,
                               Height = x.Height


                           })
                           .Select(x => new { key = x.Key, value = x.ToDictionary(z => z.Key, z => z.INPUTVALUE) })
                           .Select(item => _unitOfWork.CivilWithLegsRepository.BuildDynamicSelect(item.key, item.value, propertyNamesStatic, propertyNamesDynamic));

                            int count = query.Count();

                            getEnableAttribute.Model = query;
                            string excelFilePath = ExportToExcel(query, "TLIpower");

                            return new Response<string>(false, excelFilePath, null, null, (int)Helpers.Constants.ApiReturnCode.success);
                        }
                    }
                    catch (Exception err)
                    {
                        return new Response<string>(false, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
                    }
                }
                else if (Helpers.Constants.LoadSubType.TLImwBU.ToString() == TableNameInstallation)
                {
                    try
                    {

                        GetEnableAttribute getEnableAttribute = new GetEnableAttribute();
                        connection.Open();
                        //string storedProcedureName = "CREATE_DYNAMIC_PIVOT_MWODU";
                        //using (OracleCommand procedureCommand = new OracleCommand(storedProcedureName, connection))
                        //{
                        //    procedureCommand.CommandType = CommandType.StoredProcedure;
                        //    procedureCommand.ExecuteNonQuery();
                        //}
                        var attActivated = _context.TLIattributeViewManagment.Include(x => x.EditableManagmentView).Include(x => x.AttributeActivated)
                            .Include(x => x.DynamicAtt).Where(x => x.Enable && x.EditableManagmentView.View == "MW_BUInstallation" &&
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
                        propertyNamesStatic.Add("SiteCode");
                        propertyNamesStatic.Add("SIDEARMNAME");
                        propertyNamesStatic.Add("CIVILNAME");
                        propertyNamesStatic.Add("SIDEARM_ID");
                        propertyNamesStatic.Add("CIVIL_ID");
                        propertyNamesStatic.Add("ALLCIVILINST_ID");
                        propertyNamesStatic.Add("LEG_ID");
                        propertyNamesStatic.Add("LEG_NAME");
                        propertyNamesStatic.Add("SideArmSec_Id");
                        propertyNamesStatic.Add("SideArmSec_Name");
                        if (SiteCode == null)
                        {
                            if (propertyNamesDynamic.Count == 0)
                            {
                                var query = _context.MV_MWBU_VIEW.Where(x => !x.Dismantle).AsEnumerable()
                                .Select(item => _unitOfWork.CivilWithLegsRepository.BuildDynamicSelect(item, null, propertyNamesStatic, propertyNamesDynamic));
                                int count = query.Count();
                                getEnableAttribute.Model = query;
                                string excelFilePath = ExportToExcel(query, "TLImwBU");

                                return new Response<string>(false, excelFilePath, null, null, (int)Helpers.Constants.ApiReturnCode.success);
                            }
                            else
                            {
                                var query = _context.MV_MWBU_VIEW.Where(x => !x.Dismantle).AsEnumerable()
                               .GroupBy(x => new
                               {
                                   SiteCode = x.SiteCode,
                                   Id = x.Id,
                                   Name = x.Name,
                                   Serial_Number = x.Serial_Number,
                                   Notes = x.Notes,
                                   Height = x.Height,
                                   BUNumber = x.BUNumber,
                                   Visiable_Status = x.Visiable_Status,
                                   SpaceInstallation = x.SpaceInstallation,
                                   OWNER = x.OWNER,
                                   BASEBU = x.BASEBU,
                                   MWBULIBRARY = x.MWBULIBRARY,
                                   MAINDISH = x.MAINDISH,
                                   CenterHigh = x.CenterHigh,
                                   HBA = x.HBA,
                                   HieghFromLand = x.HieghFromLand,
                                   EquivalentSpace = x.EquivalentSpace,
                                   Azimuth = x.Azimuth,
                                   SDDISH = x.SDDISH,
                                   CIVILNAME = x.CIVILNAME,
                                   CIVIL_ID = x.CIVIL_ID,
                                   SIDEARMNAME = x.SIDEARMNAME,
                                   Dismantle = x.Dismantle,
                                   PORTCASCADE = x.PORTCASCADE,
                                   SideArmSec_Name = x.SideArmSec_Name,
                                   LEG_NAME = x.LEG_NAME,
                                   ALLCIVILINST_ID = x.ALLCIVILINST_ID,
                                   ALLLOAD_ID = x.ALLLOAD_ID,
                                   LEG_ID = x.LEG_ID,
                                   INSTALLATIONPLACE = x.INSTALLATIONPLACE,
                                   SIDEARM_ID = x.SIDEARM_ID,
                                   SideArmSec_Id = x.SideArmSec_Id
                               })
                               .Select(x => new { key = x.Key, value = x.ToDictionary(z => z.Key, z => z.INPUTVALUE) })
                               .Select(item => _unitOfWork.CivilWithLegsRepository.BuildDynamicSelect(item.key, item.value, propertyNamesStatic, propertyNamesDynamic));

                                int count = query.Count();

                                getEnableAttribute.Model = query;
                                string excelFilePath = ExportToExcel(query, "TLImwBU");

                                return new Response<string>(false, excelFilePath, null, null, (int)Helpers.Constants.ApiReturnCode.success);
                            }
                        }
                        if (propertyNamesDynamic.Count == 0)
                        {
                            var query = _context.MV_MWBU_VIEW.Where(x => x.SiteCode.ToLower() == SiteCode.ToLower() && !x.Dismantle).AsEnumerable()
                            .Select(item => _unitOfWork.CivilWithLegsRepository.BuildDynamicSelect(item, null, propertyNamesStatic, propertyNamesDynamic));
                            int count = query.Count();
                            getEnableAttribute.Model = query;
                            string excelFilePath = ExportToExcel(query, "TLImwBU");

                            return new Response<string>(false, excelFilePath, null, null, (int)Helpers.Constants.ApiReturnCode.success);
                        }
                        else
                        {
                            var query = _context.MV_MWBU_VIEW.Where(x => x.SiteCode.ToLower() == SiteCode.ToLower() && !x.Dismantle).AsEnumerable()
                           .GroupBy(x => new
                           {
                               SiteCode = x.SiteCode,
                               Id = x.Id,
                               Name = x.Name,
                               Serial_Number = x.Serial_Number,
                               Notes = x.Notes,
                               Height = x.Height,
                               BUNumber = x.BUNumber,
                               Visiable_Status = x.Visiable_Status,
                               SpaceInstallation = x.SpaceInstallation,
                               OWNER = x.OWNER,
                               BASEBU = x.BASEBU,
                               MWBULIBRARY = x.MWBULIBRARY,
                               MAINDISH = x.MAINDISH,
                               CenterHigh = x.CenterHigh,
                               HBA = x.HBA,
                               HieghFromLand = x.HieghFromLand,
                               EquivalentSpace = x.EquivalentSpace,
                               Azimuth = x.Azimuth,
                               SDDISH = x.SDDISH,
                               CIVILNAME = x.CIVILNAME,
                               CIVIL_ID = x.CIVIL_ID,
                               SIDEARMNAME = x.SIDEARMNAME,
                               Dismantle = x.Dismantle,
                               PORTCASCADE = x.PORTCASCADE,
                               SideArmSec_Name = x.SideArmSec_Name,
                               LEG_NAME = x.LEG_NAME,
                               ALLCIVILINST_ID = x.ALLCIVILINST_ID,
                               ALLLOAD_ID = x.ALLLOAD_ID,
                               LEG_ID = x.LEG_ID,
                               INSTALLATIONPLACE = x.INSTALLATIONPLACE,
                               SIDEARM_ID = x.SIDEARM_ID,
                               SideArmSec_Id = x.SideArmSec_Id

                           })
                           .Select(x => new { key = x.Key, value = x.ToDictionary(z => z.Key, z => z.INPUTVALUE) })
                           .Select(item => _unitOfWork.CivilWithLegsRepository.BuildDynamicSelect(item.key, item.value, propertyNamesStatic, propertyNamesDynamic));

                            int count = query.Count();

                            getEnableAttribute.Model = query;


                            string excelFilePath = ExportToExcel(query, "TLImwBU");

                            return new Response<string>(false, excelFilePath, null, null, (int)Helpers.Constants.ApiReturnCode.success);
                        }

                    }
                    catch (Exception err)
                    {
                        return new Response<string>(false, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
                    }
                }
                else if (Helpers.Constants.LoadSubType.TLImwRFU.ToString() == TableNameInstallation)
                {
                    try
                    {

                        GetEnableAttribute getEnableAttribute = new GetEnableAttribute();
                        connection.Open();
                        //string storedProcedureName = "CREATE_DYNAMIC_PIVOT_MWDISH";
                        //using (OracleCommand procedureCommand = new OracleCommand(storedProcedureName, connection))
                        //{
                        //    procedureCommand.CommandType = CommandType.StoredProcedure;
                        //    procedureCommand.ExecuteNonQuery();
                        //}
                        var attActivated = _context.TLIattributeViewManagment.Include(x => x.EditableManagmentView).Include(x => x.AttributeActivated)
                            .Include(x => x.DynamicAtt).Where(x => x.Enable && x.EditableManagmentView.View == "MW_RFUInstallation" &&
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
                        propertyNamesStatic.Add("SiteCode");
                        propertyNamesStatic.Add("LEG_NAME");
                        propertyNamesStatic.Add("CIVILNAME");
                        propertyNamesStatic.Add("CIVIL_ID");
                        propertyNamesStatic.Add("SIDEARMNAME");
                        propertyNamesStatic.Add("SIDEARM_ID");
                        propertyNamesStatic.Add("ALLCIVILINST_ID");
                        propertyNamesStatic.Add("LEG_ID");
                        if (SiteCode == null)
                        {
                            if (propertyNamesDynamic.Count == 0)
                            {
                                var query = _context.MV_MWRFU_VIEW.Where(x => !x.Dismantle).AsEnumerable()
                                .Select(item => _unitOfWork.CivilWithLegsRepository.BuildDynamicSelect(item, null, propertyNamesStatic, propertyNamesDynamic));
                                int count = query.Count();
                                getEnableAttribute.Model = query;
                                string excelFilePath = ExportToExcel(query, "TLImwRFU");

                                return new Response<string>(false, excelFilePath, null, null, (int)Helpers.Constants.ApiReturnCode.success);
                            }
                            else
                            {
                                var query = _context.MV_MWRFU_VIEW.Where(x => !x.Dismantle).AsEnumerable()
                               .GroupBy(x => new
                               {
                                   SiteCode = x.SiteCode,
                                   Id = x.Id,
                                   Name = x.Name,
                                   Azimuth = x.Azimuth,
                                   heightBase = x.heightBase,
                                   Note = x.Note,
                                   OWNER = x.OWNER,
                                   SerialNumber = x.SerialNumber,
                                   SpaceInstallation = x.SpaceInstallation,
                                   MWRFULIBRARY = x.MWRFULIBRARY,
                                   HieghFromLand = x.HieghFromLand,
                                   MWPORT = x.MWPORT,
                                   CenterHigh = x.CenterHigh,
                                   HBA = x.HBA,
                                   EquivalentSpace = x.EquivalentSpace,
                                   Dismantle = x.Dismantle,
                                   LEG_NAME = x.LEG_NAME,
                                   CIVILNAME = x.CIVILNAME,
                                   CIVIL_ID = x.CIVIL_ID,
                                   SIDEARMNAME = x.SIDEARMNAME,
                                   SIDEARM_ID = x.SIDEARM_ID,
                                   ALLCIVILINST_ID = x.ALLCIVILINST_ID,
                                   LEG_ID = x.LEG_ID,
                                   SideArmSec_Name = x.SideArmSec_Name,
                                   SideArmSec_Id = x.SideArmSec_Id

                               })
                               .Select(x => new { key = x.Key, value = x.ToDictionary(z => z.Key, z => z.INPUTVALUE) })
                               .Select(item => _unitOfWork.CivilWithLegsRepository.BuildDynamicSelect(item.key, item.value, propertyNamesStatic, propertyNamesDynamic));

                                int count = query.Count();

                                getEnableAttribute.Model = query;
                                string excelFilePath = ExportToExcel(query, "TLImwRFU");

                                return new Response<string>(false, excelFilePath, null, null, (int)Helpers.Constants.ApiReturnCode.success);
                            }
                        }
                        if (propertyNamesDynamic.Count == 0)
                        {
                            var query = _context.MV_MWRFU_VIEW.Where(x => x.SiteCode.ToLower() == SiteCode.ToLower() && !x.Dismantle).AsEnumerable()
                            .Select(item => _unitOfWork.CivilWithLegsRepository.BuildDynamicSelect(item, null, propertyNamesStatic, propertyNamesDynamic));
                            int count = query.Count();
                            getEnableAttribute.Model = query;
                            string excelFilePath = ExportToExcel(query, "TLImwRFU");

                            return new Response<string>(false, excelFilePath, null, null, (int)Helpers.Constants.ApiReturnCode.success);
                        }
                        else
                        {
                            var query = _context.MV_MWRFU_VIEW.Where(x => x.SiteCode.ToLower() == SiteCode.ToLower() && !x.Dismantle).AsEnumerable()
                           .GroupBy(x => new
                           {
                               SiteCode = x.SiteCode,
                               Id = x.Id,
                               Name = x.Name,
                               Azimuth = x.Azimuth,
                               heightBase = x.heightBase,
                               Note = x.Note,
                               OWNER = x.OWNER,
                               SerialNumber = x.SerialNumber,
                               SpaceInstallation = x.SpaceInstallation,
                               MWRFULIBRARY = x.MWRFULIBRARY,
                               HieghFromLand = x.HieghFromLand,
                               MWPORT = x.MWPORT,
                               CenterHigh = x.CenterHigh,
                               HBA = x.HBA,
                               EquivalentSpace = x.EquivalentSpace,
                               Dismantle = x.Dismantle,
                               LEG_NAME = x.LEG_NAME,
                               CIVILNAME = x.CIVILNAME,
                               CIVIL_ID = x.CIVIL_ID,
                               SIDEARMNAME = x.SIDEARMNAME,
                               SIDEARM_ID = x.SIDEARM_ID,
                               ALLCIVILINST_ID = x.ALLCIVILINST_ID,
                               LEG_ID = x.LEG_ID,
                               SideArmSec_Name = x.SideArmSec_Name,
                               SideArmSec_Id = x.SideArmSec_Id

                           })
                           .Select(x => new { key = x.Key, value = x.ToDictionary(z => z.Key, z => z.INPUTVALUE) })
                           .Select(item => _unitOfWork.CivilWithLegsRepository.BuildDynamicSelect(item.key, item.value, propertyNamesStatic, propertyNamesDynamic));

                            int count = query.Count();

                            getEnableAttribute.Model = query;

                            string excelFilePath = ExportToExcel(query, "TLImwRFU");

                            return new Response<string>(false, excelFilePath, null, null, (int)Helpers.Constants.ApiReturnCode.success);
                        }

                    }
                    catch (Exception err)
                    {
                        return new Response<string>(false, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
                    }
                }
                else if (Helpers.Constants.LoadSubType.TLImwODU.ToString() == TableNameInstallation)
                {
                    try
                    {

                        GetEnableAttribute getEnableAttribute = new GetEnableAttribute();
                        connection.Open();
                        //string storedProcedureName = "CREATE_DYNAMIC_PIVOT_MWODU";
                        //using (OracleCommand procedureCommand = new OracleCommand(storedProcedureName, connection))
                        //{
                        //    procedureCommand.CommandType = CommandType.StoredProcedure;
                        //    procedureCommand.ExecuteNonQuery();
                        //}
                        var attActivated = _context.TLIattributeViewManagment.Include(x => x.EditableManagmentView).Include(x => x.AttributeActivated)
                            .Include(x => x.DynamicAtt).Where(x => x.Enable && x.EditableManagmentView.View == "MW_ODUInstallation" &&
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
                        propertyNamesStatic.Add("SiteCode");
                        propertyNamesStatic.Add("SIDEARMNAME");
                        propertyNamesStatic.Add("CIVILNAME");
                        propertyNamesStatic.Add("SIDEARMID");
                        propertyNamesStatic.Add("CIVIL_ID");
                        propertyNamesStatic.Add("ALLCIVILID");
                        propertyNamesStatic.Add("MW_DISH_ID");
                        if (SiteCode == null)
                        {
                            if (propertyNamesDynamic.Count == 0)
                            {
                                var query = _context.MV_MWODU_VIEW.Where(x => !x.Dismantle).AsEnumerable()
                                .Select(item => _unitOfWork.CivilWithLegsRepository.BuildDynamicSelect(item, null, propertyNamesStatic, propertyNamesDynamic));
                                int count = query.Count();
                                getEnableAttribute.Model = query;
                                string excelFilePath = ExportToExcel(query, "TLImwODU");

                                return new Response<string>(false, excelFilePath, null, null, (int)Helpers.Constants.ApiReturnCode.success);
                            }
                            else
                            {
                                var query = _context.MV_MWODU_VIEW.Where(x => !x.Dismantle).AsEnumerable()
                               .GroupBy(x => new
                               {
                                   SiteCode = x.SiteCode,
                                   Id = x.Id,
                                   Name = x.Name,
                                   Serial_Number = x.Serial_Number,
                                   Notes = x.Notes,
                                   Height = x.Height,
                                   ODUConnections = x.ODUConnections,
                                   Visiable_Status = x.Visiable_Status,
                                   SpaceInstallation = x.SpaceInstallation,
                                   OWNER = x.OWNER,
                                   MW_DISH = x.MW_DISH,
                                   ODUINSTALLATIONTYPE = x.ODUINSTALLATIONTYPE,
                                   MWODULIBRARY = x.MWODULIBRARY,
                                   CenterHigh = x.CenterHigh,
                                   HBA = x.HBA,
                                   HieghFromLand = x.HieghFromLand,
                                   EquivalentSpace = x.EquivalentSpace,
                                   Azimuth = x.Azimuth,
                                   SIDEARMID = x.SIDEARMID,
                                   CIVILNAME = x.CIVILNAME,
                                   CIVIL_ID = x.CIVIL_ID,
                                   SIDEARMNAME = x.SIDEARMNAME,
                                   Dismantle = x.Dismantle,
                                   ALLCIVILID = x.ALLCIVILID,
                                   MW_DISH_ID = x.MW_DISH_ID,

                               })
                               .Select(x => new { key = x.Key, value = x.ToDictionary(z => z.Key, z => z.INPUTVALUE) })
                               .Select(item => _unitOfWork.CivilWithLegsRepository.BuildDynamicSelect(item.key, item.value, propertyNamesStatic, propertyNamesDynamic));

                                int count = query.Count();

                                getEnableAttribute.Model = query;
                                string excelFilePath = ExportToExcel(query, "TLImwODU");

                                return new Response<string>(false, excelFilePath, null, null, (int)Helpers.Constants.ApiReturnCode.success);
                            }
                        }
                        if (propertyNamesDynamic.Count == 0)
                        {
                            var query = _context.MV_MWODU_VIEW.Where(x => x.SiteCode.ToLower() == SiteCode.ToLower() && !x.Dismantle).AsEnumerable()
                            .Select(item => _unitOfWork.CivilWithLegsRepository.BuildDynamicSelect(item, null, propertyNamesStatic, propertyNamesDynamic));
                            int count = query.Count();
                            getEnableAttribute.Model = query;
                            string excelFilePath = ExportToExcel(query, "TLImwODU");

                            return new Response<string>(false, excelFilePath, null, null, (int)Helpers.Constants.ApiReturnCode.success);
                        }
                        else
                        {
                            var query = _context.MV_MWODU_VIEW.Where(x => x.SiteCode.ToLower() == SiteCode.ToLower() && !x.Dismantle).AsEnumerable()
                           .GroupBy(x => new
                           {
                               SiteCode = x.SiteCode,
                               Id = x.Id,
                               Name = x.Name,
                               Serial_Number = x.Serial_Number,
                               Notes = x.Notes,
                               Height = x.Height,
                               ODUConnections = x.ODUConnections,
                               Visiable_Status = x.Visiable_Status,
                               SpaceInstallation = x.SpaceInstallation,
                               OWNER = x.OWNER,
                               MW_DISH = x.MW_DISH,
                               ODUINSTALLATIONTYPE = x.ODUINSTALLATIONTYPE,
                               MWODULIBRARY = x.MWODULIBRARY,
                               CenterHigh = x.CenterHigh,
                               HBA = x.HBA,
                               HieghFromLand = x.HieghFromLand,
                               EquivalentSpace = x.EquivalentSpace,
                               Azimuth = x.Azimuth,
                               SIDEARMID = x.SIDEARMID,
                               CIVILNAME = x.CIVILNAME,
                               CIVIL_ID = x.CIVIL_ID,
                               SIDEARMNAME = x.SIDEARMNAME,
                               Dismantle = x.Dismantle,
                               ALLCIVILID = x.ALLCIVILID,
                               MW_DISH_ID = x.MW_DISH_ID,

                           })
                           .Select(x => new { key = x.Key, value = x.ToDictionary(z => z.Key, z => z.INPUTVALUE) })
                           .Select(item => _unitOfWork.CivilWithLegsRepository.BuildDynamicSelect(item.key, item.value, propertyNamesStatic, propertyNamesDynamic));

                            int count = query.Count();

                            getEnableAttribute.Model = query;


                            string excelFilePath = ExportToExcel(query, "TLImwODU");

                            return new Response<string>(false, excelFilePath, null, null, (int)Helpers.Constants.ApiReturnCode.success);
                        }

                    }
                    catch (Exception err)
                    {
                        return new Response<string>(false, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
                    }
                }
                else if (Helpers.Constants.LoadSubType.TLImwDish.ToString() == TableNameInstallation)
                {
                    try
                    {

                        GetEnableAttribute getEnableAttribute = new GetEnableAttribute();
                        connection.Open();
                        //string storedProcedureName = "CREATE_DYNAMIC_PIVOT_MWDISH";
                        //using (OracleCommand procedureCommand = new OracleCommand(storedProcedureName, connection))
                        //{
                        //    procedureCommand.CommandType = CommandType.StoredProcedure;
                        //    procedureCommand.ExecuteNonQuery();
                        //}
                        var attActivated = _context.TLIattributeViewManagment.Include(x => x.EditableManagmentView).Include(x => x.AttributeActivated)
                            .Include(x => x.DynamicAtt).Where(x => x.Enable && x.EditableManagmentView.View == "MW_DishInstallation" &&
                            ((x.AttributeActivatedId != null && x.AttributeActivated.enable) || (x.DynamicAttId != null && !x.DynamicAtt.disable)))
                            .Select(x => new { attribute = x.AttributeActivated.Key, dynamic = x.DynamicAtt.Key, dataType = x.DynamicAtt != null ? x.DynamicAtt.DataType.Name.ToString() : x.AttributeActivated.DataType.ToString() })
                          .OrderByDescending(x => x.attribute.ToLower().StartsWith("dishname"))
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
                        propertyNamesStatic.Add("SiteCode");
                        propertyNamesStatic.Add("LEG_NAME");
                        propertyNamesStatic.Add("CIVILNAME");
                        propertyNamesStatic.Add("CIVIL_ID");
                        propertyNamesStatic.Add("SIDEARMNAME");
                        propertyNamesStatic.Add("SIDEARM_ID");
                        propertyNamesStatic.Add("ALLCIVILINST_ID");
                        propertyNamesStatic.Add("LEG_ID");
                        propertyNamesStatic.Add("ODU_COUNT");
                        propertyNamesStatic.Add("POLARITYTYPE");
                        propertyNamesStatic.Add("SideArmSec_Name");
                        propertyNamesStatic.Add("SideArmSec_Id");
                        if (SiteCode == null)
                        {
                            if (propertyNamesDynamic.Count == 0)
                            {
                                var query = _context.MV_MWDISH_VIEW.Where(x => !x.Dismantle).AsEnumerable()
                                .Select(item => _unitOfWork.CivilWithLegsRepository.BuildDynamicSelect(item, null, propertyNamesStatic, propertyNamesDynamic));
                                int count = query.Count();
                                getEnableAttribute.Model = query;
                                string excelFilePath = ExportToExcel(query, "TLImwDish");

                                return new Response<string>(false, excelFilePath, null, null, (int)Helpers.Constants.ApiReturnCode.success);
                            }
                            else
                            {
                                var query = _context.MV_MWDISH_VIEW.Where(x => !x.Dismantle).AsEnumerable()
                               .GroupBy(x => new
                               {
                                   SiteCode = x.SiteCode,
                                   Id = x.Id,
                                   DishName = x.DishName,
                                   Azimuth = x.Azimuth,
                                   Notes = x.Notes,
                                   Far_End_Site_Code = x.Far_End_Site_Code,
                                   HBA_Surface = x.HBA_Surface,
                                   Serial_Number = x.Serial_Number,
                                   MW_LINK = x.MW_LINK,
                                   Visiable_Status = x.Visiable_Status,
                                   SpaceInstallation = x.SpaceInstallation,
                                   HeightBase = x.HeightBase,
                                   HeightLand = x.HeightLand,
                                   Temp = x.Temp,
                                   OWNER = x.OWNER,
                                   REPEATERTYPE = x.REPEATERTYPE,
                                   POLARITYONLOCATION = x.POLARITYONLOCATION,
                                   ITEMCONNECTTO = x.ITEMCONNECTTO,
                                   MWDISHLIBRARY = x.MWDISHLIBRARY,
                                   INSTALLATIONPLACE = x.INSTALLATIONPLACE,
                                   CenterHigh = x.CenterHigh,
                                   HBA = x.HBA,
                                   HieghFromLand = x.HieghFromLand,
                                   EquivalentSpace = x.EquivalentSpace,
                                   Dismantle = x.Dismantle,
                                   LEG_NAME = x.LEG_NAME,
                                   CIVILNAME = x.CIVILNAME,
                                   CIVIL_ID = x.CIVIL_ID,
                                   SIDEARMNAME = x.SIDEARMNAME,
                                   SIDEARM_ID = x.SIDEARM_ID,
                                   ALLCIVILINST_ID = x.ALLCIVILINST_ID,
                                   LEG_ID = x.LEG_ID,
                                   ODU_COUNT = x.ODU_COUNT,
                                   POLARITYTYPE = x.POLARITYTYPE,
                                   SideArmSec_Name = x.SideArmSec_Name,
                                   SideArmSec_Id = x.SideArmSec_Id

                               })
                               .Select(x => new { key = x.Key, value = x.ToDictionary(z => z.Key, z => z.INPUTVALUE) })
                               .Select(item => _unitOfWork.CivilWithLegsRepository.BuildDynamicSelect(item.key, item.value, propertyNamesStatic, propertyNamesDynamic));

                                int count = query.Count();

                                getEnableAttribute.Model = query;
                                string excelFilePath = ExportToExcel(query, "TLImwDish");

                                return new Response<string>(false, excelFilePath, null, null, (int)Helpers.Constants.ApiReturnCode.success);
                            }
                        }
                        if (propertyNamesDynamic.Count == 0)
                        {
                            var query = _context.MV_MWDISH_VIEW.Where(x => x.SiteCode.ToLower() == SiteCode.ToLower() && !x.Dismantle).AsEnumerable()
                            .Select(item => _unitOfWork.CivilWithLegsRepository.BuildDynamicSelect(item, null, propertyNamesStatic, propertyNamesDynamic));
                            int count = query.Count();
                            getEnableAttribute.Model = query; string excelFilePath = ExportToExcel(query, "TLImwDish");

                            return new Response<string>(false, excelFilePath, null, null, (int)Helpers.Constants.ApiReturnCode.success); 
                        }
                        else
                        {
                            var query = _context.MV_MWDISH_VIEW.Where(x => x.SiteCode.ToLower() == SiteCode.ToLower() && !x.Dismantle).AsEnumerable()
                           .GroupBy(x => new
                           {
                               SiteCode = x.SiteCode,
                               Id = x.Id,
                               DishName = x.DishName,
                               Azimuth = x.Azimuth,
                               Notes = x.Notes,
                               Far_End_Site_Code = x.Far_End_Site_Code,
                               HBA_Surface = x.HBA_Surface,
                               Serial_Number = x.Serial_Number,
                               MW_LINK = x.MW_LINK,
                               Visiable_Status = x.Visiable_Status,
                               SpaceInstallation = x.SpaceInstallation,
                               HeightBase = x.HeightBase,
                               HeightLand = x.HeightLand,
                               Temp = x.Temp,
                               OWNER = x.OWNER,
                               REPEATERTYPE = x.REPEATERTYPE,
                               POLARITYONLOCATION = x.POLARITYONLOCATION,
                               ITEMCONNECTTO = x.ITEMCONNECTTO,
                               MWDISHLIBRARY = x.MWDISHLIBRARY,
                               INSTALLATIONPLACE = x.INSTALLATIONPLACE,
                               CenterHigh = x.CenterHigh,
                               HBA = x.HBA,
                               HieghFromLand = x.HieghFromLand,
                               EquivalentSpace = x.EquivalentSpace,
                               Dismantle = x.Dismantle,
                               LEG_NAME = x.LEG_NAME,
                               CIVILNAME = x.CIVILNAME,
                               CIVIL_ID = x.CIVIL_ID,
                               SIDEARMNAME = x.SIDEARMNAME,
                               SIDEARM_ID = x.SIDEARM_ID,
                               ALLCIVILINST_ID = x.ALLCIVILINST_ID,
                               LEG_ID = x.LEG_ID,
                               ODU_COUNT = x.ODU_COUNT,
                               POLARITYTYPE = x.POLARITYTYPE,
                               SideArmSec_Name = x.SideArmSec_Name,
                               SideArmSec_Id = x.SideArmSec_Id

                           })
                           .Select(x => new { key = x.Key, value = x.ToDictionary(z => z.Key, z => z.INPUTVALUE) })
                           .Select(item => _unitOfWork.CivilWithLegsRepository.BuildDynamicSelect(item.key, item.value, propertyNamesStatic, propertyNamesDynamic));

                            int count = query.Count();

                            getEnableAttribute.Model = query;

                            string excelFilePath = ExportToExcel(query, "TLImwDish");

                            return new Response<string>(false, excelFilePath, null, null, (int)Helpers.Constants.ApiReturnCode.success);
                        }

                    }
                    catch (Exception err)
                    {
                        return new Response<string>(false, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
                    }
                }
                else if (Helpers.Constants.LoadSubType.TLImwOther.ToString() == TableNameInstallation)
                {
                    try
                    {

                        GetEnableAttribute getEnableAttribute = new GetEnableAttribute();
                        connection.Open();
                        //string storedProcedureName = "CREATE_DYNAMIC_PIVOT_MWDISH";
                        //using (OracleCommand procedureCommand = new OracleCommand(storedProcedureName, connection))
                        //{
                        //    procedureCommand.CommandType = CommandType.StoredProcedure;
                        //    procedureCommand.ExecuteNonQuery();
                        //}
                        var attActivated = _context.TLIattributeViewManagment.Include(x => x.EditableManagmentView).Include(x => x.AttributeActivated)
                            .Include(x => x.DynamicAtt).Where(x => x.Enable && x.EditableManagmentView.View == "OtherMWInstallation" &&
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
                        propertyNamesStatic.Add("SiteCode");
                        propertyNamesStatic.Add("LEG_NAME");
                        propertyNamesStatic.Add("CIVILNAME");
                        propertyNamesStatic.Add("CIVIL_ID");
                        propertyNamesStatic.Add("SIDEARMNAME");
                        propertyNamesStatic.Add("SIDEARM_ID");
                        propertyNamesStatic.Add("ALLCIVILINST_ID");
                        propertyNamesStatic.Add("LEG_ID");
                        if (SiteCode == null)
                        {
                            if (propertyNamesDynamic.Count == 0)
                            {
                                var query = _context.MV_MWOTHER_VIEW.Where(x => !x.Dismantle).AsEnumerable()
                                .Select(item => _unitOfWork.CivilWithLegsRepository.BuildDynamicSelect(item, null, propertyNamesStatic, propertyNamesDynamic));
                                int count = query.Count();
                                getEnableAttribute.Model = query;
                                string excelFilePath = ExportToExcel(query, "TLImwOther");

                                return new Response<string>(false, excelFilePath, null, null, (int)Helpers.Constants.ApiReturnCode.success);
                            }
                            else
                            {
                                var query = _context.MV_MWOTHER_VIEW.Where(x => !x.Dismantle).AsEnumerable()
                               .GroupBy(x => new
                               {
                                   SiteCode = x.SiteCode,
                                   Id = x.Id,
                                   Name = x.Name,
                                   Azimuth = x.Azimuth,
                                   Notes = x.Notes,
                                   HeightBase = x.HeightBase,
                                   HeightLand = x.HeightLand,
                                   VisibleStatus = x.VisibleStatus,
                                   SerialNumber = x.SerialNumber,
                                   HieghFromLand = x.HieghFromLand,
                                   Spaceinstallation = x.Spaceinstallation,
                                   MWOTHERLIBRARY = x.MWOTHERLIBRARY,
                                   INSTALLATIONPLACE = x.INSTALLATIONPLACE,
                                   CenterHigh = x.CenterHigh,
                                   HBA = x.HBA,
                                   EquivalentSpace = x.EquivalentSpace,
                                   Dismantle = x.Dismantle,
                                   LEG_NAME = x.LEG_NAME,
                                   CIVILNAME = x.CIVILNAME,
                                   CIVIL_ID = x.CIVIL_ID,
                                   SIDEARMNAME = x.SIDEARMNAME,
                                   SIDEARM_ID = x.SIDEARM_ID,
                                   ALLCIVILINST_ID = x.ALLCIVILINST_ID,
                                   LEG_ID = x.LEG_ID,
                                   SideArmSec_Name = x.SideArmSec_Name,
                                   SideArmSec_Id = x.SideArmSec_Id

                               })
                               .Select(x => new { key = x.Key, value = x.ToDictionary(z => z.Key, z => z.INPUTVALUE) })
                               .Select(item => _unitOfWork.CivilWithLegsRepository.BuildDynamicSelect(item.key, item.value, propertyNamesStatic, propertyNamesDynamic));

                                int count = query.Count();

                                getEnableAttribute.Model = query;
                                string excelFilePath = ExportToExcel(query, "TLImwOther");

                                return new Response<string>(false, excelFilePath, null, null, (int)Helpers.Constants.ApiReturnCode.success);
                            }
                        }
                        if (propertyNamesDynamic.Count == 0)
                        {
                            var query = _context.MV_MWOTHER_VIEW.Where(x => x.SiteCode.ToLower() == SiteCode.ToLower() && !x.Dismantle).AsEnumerable()
                            .Select(item => _unitOfWork.CivilWithLegsRepository.BuildDynamicSelect(item, null, propertyNamesStatic, propertyNamesDynamic));
                            int count = query.Count();
                            getEnableAttribute.Model = query;
                            string excelFilePath = ExportToExcel(query, "TLImwOther");

                            return new Response<string>(false, excelFilePath, null, null, (int)Helpers.Constants.ApiReturnCode.success);
                        }
                        else
                        {
                            var query = _context.MV_MWOTHER_VIEW.Where(x => x.SiteCode.ToLower() == SiteCode.ToLower() && !x.Dismantle).AsEnumerable()
                           .GroupBy(x => new
                           {
                               SiteCode = x.SiteCode,
                               Id = x.Id,
                               Name = x.Name,
                               Azimuth = x.Azimuth,
                               Notes = x.Notes,
                               HeightBase = x.HeightBase,
                               HeightLand = x.HeightLand,
                               VisibleStatus = x.VisibleStatus,
                               SerialNumber = x.SerialNumber,
                               HieghFromLand = x.HieghFromLand,
                               Spaceinstallation = x.Spaceinstallation,
                               MWOTHERLIBRARY = x.MWOTHERLIBRARY,
                               INSTALLATIONPLACE = x.INSTALLATIONPLACE,
                               CenterHigh = x.CenterHigh,
                               HBA = x.HBA,
                               EquivalentSpace = x.EquivalentSpace,
                               Dismantle = x.Dismantle,
                               LEG_NAME = x.LEG_NAME,
                               CIVILNAME = x.CIVILNAME,
                               CIVIL_ID = x.CIVIL_ID,
                               SIDEARMNAME = x.SIDEARMNAME,
                               SIDEARM_ID = x.SIDEARM_ID,
                               ALLCIVILINST_ID = x.ALLCIVILINST_ID,
                               LEG_ID = x.LEG_ID,
                               SideArmSec_Name = x.SideArmSec_Name,
                               SideArmSec_Id = x.SideArmSec_Id

                           })
                           .Select(x => new { key = x.Key, value = x.ToDictionary(z => z.Key, z => z.INPUTVALUE) })
                           .Select(item => _unitOfWork.CivilWithLegsRepository.BuildDynamicSelect(item.key, item.value, propertyNamesStatic, propertyNamesDynamic));

                            int count = query.Count();

                            getEnableAttribute.Model = query;

                            string excelFilePath = ExportToExcel(query, "TLImwOther");

                            return new Response<string>(false, excelFilePath, null, null, (int)Helpers.Constants.ApiReturnCode.success);
                        }

                    }
                    catch (Exception err)
                    {
                        return new Response<string>(false, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
                    }
                }
                else if (Helpers.Constants.LoadSubType.TLIradioAntenna.ToString() == TableNameInstallation)
                {
                    try
                    {
                        GetEnableAttribute getEnableAttribute = new GetEnableAttribute();
                        connection.Open();
                        //string storedProcedureName = "CREATE_DYNAMIC_PIVOT_MWDISH";
                        //using (OracleCommand procedureCommand = new OracleCommand(storedProcedureName, connection))
                        //{
                        //    procedureCommand.CommandType = CommandType.StoredProcedure;
                        //    procedureCommand.ExecuteNonQuery();
                        //}
                        var attActivated = _context.TLIattributeViewManagment.Include(x => x.EditableManagmentView).Include(x => x.AttributeActivated)
                            .Include(x => x.DynamicAtt).Where(x => x.Enable && x.EditableManagmentView.View == "RadioAntennaInstallation" &&
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
                        propertyNamesStatic.Add("CIVIL_ID");
                        propertyNamesStatic.Add("SIDEARMNAME");
                        propertyNamesStatic.Add("SIDEARM_ID");
                        propertyNamesStatic.Add("ALLCIVILINST_ID");
                        propertyNamesStatic.Add("LEGID");
                        propertyNamesStatic.Add("LEGNAME");
                        propertyNamesStatic.Add("SiteCode");
                        if (SiteCode == null)
                        {
                            if (propertyNamesDynamic.Count == 0)
                            {
                                var query = _context.MV_RADIO_ANTENNA_VIEW.Where(x => !x.Dismantle).AsEnumerable()
                               .Select(item => _unitOfWork.CivilWithLegsRepository.BuildDynamicSelect(item, null, propertyNamesStatic, propertyNamesDynamic));
                                int count = query.Count();
                                getEnableAttribute.Model = query;
                                string excelFilePath = ExportToExcel(query, "TLIradioAntenna");

                                return new Response<string>(false, excelFilePath, null, null, (int)Helpers.Constants.ApiReturnCode.success);
                            }
                            else
                            {
                                var query = _context.MV_RADIO_ANTENNA_VIEW.Where(x => !x.Dismantle).AsEnumerable()
                               .GroupBy(x => new
                               {
                                   SiteCode = x.SiteCode,
                                   Id = x.Id,
                                   Name = x.Name,
                                   Azimuth = x.Azimuth,
                                   Notes = x.Notes,
                                   VisibleStatus = x.VisibleStatus,
                                   MechanicalTilt = x.MechanicalTilt,
                                   ElectricalTilt = x.ElectricalTilt,
                                   SerialNumber = x.SerialNumber,
                                   HBASurface = x.HBASurface,
                                   SpaceInstallation = x.SpaceInstallation,
                                   HeightBase = x.HeightBase,
                                   HeightLand = x.HeightLand,
                                   INSTALLATIONPLACE = x.INSTALLATIONPLACE,
                                   RADIOANTENNALIBRARY = x.RADIOANTENNALIBRARY,
                                   Dismantle = x.Dismantle,
                                   CenterHigh = x.CenterHigh,
                                   HBA = x.HBA,
                                   HieghFromLand = x.HieghFromLand,
                                   EquivalentSpace = x.EquivalentSpace,
                                   LEGNAME = x.LEGNAME,
                                   CIVILNAME = x.CIVILNAME,
                                   CIVIL_ID = x.CIVIL_ID,
                                   SIDEARMNAME = x.SIDEARMNAME,
                                   SIDEARM_ID = x.SIDEARM_ID,
                                   ALLCIVILINST_ID = x.ALLCIVILINST_ID,
                                   LEGID = x.LEGID,
                                   OWNER = x.OWNER


                               })
                               .Select(x => new { key = x.Key, value = x.ToDictionary(z => z.Key, z => z.INPUTVALUE) })
                               .Select(item => _unitOfWork.CivilWithLegsRepository.BuildDynamicSelect(item.key, item.value, propertyNamesStatic, propertyNamesDynamic));

                                int count = query.Count();

                                getEnableAttribute.Model = query;
                                string excelFilePath = ExportToExcel(query, "TLIradioAntenna");

                                return new Response<string>(false, excelFilePath, null, null, (int)Helpers.Constants.ApiReturnCode.success);
                            }
                        }
                        if (propertyNamesDynamic.Count == 0)
                        {
                            var query = _context.MV_RADIO_ANTENNA_VIEW.Where(x => x.SiteCode.ToLower() == SiteCode.ToLower() && !x.Dismantle).AsEnumerable()
                            .Select(item => _unitOfWork.CivilWithLegsRepository.BuildDynamicSelect(item, null, propertyNamesStatic, propertyNamesDynamic));
                            int count = query.Count();
                            getEnableAttribute.Model = query;
                            string excelFilePath = ExportToExcel(query, "TLIradioAntenna");

                            return new Response<string>(false, excelFilePath, null, null, (int)Helpers.Constants.ApiReturnCode.success);
                        }
                        else
                        {
                            var query = _context.MV_RADIO_ANTENNA_VIEW.Where(x => x.SiteCode.ToLower() == SiteCode.ToLower() && !x.Dismantle).AsEnumerable()
                           .GroupBy(x => new
                           {
                               SiteCode = x.SiteCode,
                               Id = x.Id,
                               Name = x.Name,
                               Azimuth = x.Azimuth,
                               Notes = x.Notes,
                               VisibleStatus = x.VisibleStatus,
                               MechanicalTilt = x.MechanicalTilt,
                               ElectricalTilt = x.ElectricalTilt,
                               SerialNumber = x.SerialNumber,
                               HBASurface = x.HBASurface,
                               SpaceInstallation = x.SpaceInstallation,
                               HeightBase = x.HeightBase,
                               HeightLand = x.HeightLand,
                               INSTALLATIONPLACE = x.INSTALLATIONPLACE,
                               RADIOANTENNALIBRARY = x.RADIOANTENNALIBRARY,
                               Dismantle = x.Dismantle,
                               CenterHigh = x.CenterHigh,
                               HBA = x.HBA,
                               HieghFromLand = x.HieghFromLand,
                               EquivalentSpace = x.EquivalentSpace,
                               LEGNAME = x.LEGNAME,
                               CIVILNAME = x.CIVILNAME,
                               CIVIL_ID = x.CIVIL_ID,
                               SIDEARMNAME = x.SIDEARMNAME,
                               SIDEARM_ID = x.SIDEARM_ID,
                               ALLCIVILINST_ID = x.ALLCIVILINST_ID,
                               LEGID = x.LEGID,
                               OWNER = x.OWNER


                           })
                           .Select(x => new { key = x.Key, value = x.ToDictionary(z => z.Key, z => z.INPUTVALUE) })
                           .Select(item => _unitOfWork.CivilWithLegsRepository.BuildDynamicSelect(item.key, item.value, propertyNamesStatic, propertyNamesDynamic));

                            int count = query.Count();

                            getEnableAttribute.Model = query;
                            string excelFilePath = ExportToExcel(query, "TLIradioAntenna");

                            return new Response<string>(false, excelFilePath, null, null, (int)Helpers.Constants.ApiReturnCode.success);
                        }
                    }
                    catch (Exception err)
                    {
                        return new Response<string>(false, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
                    }
                }
                else if (Helpers.Constants.LoadSubType.TLIradioRRU.ToString() == TableNameInstallation)
                {
                    try
                    {
                        GetEnableAttribute getEnableAttribute = new GetEnableAttribute();
                        connection.Open();
                        //string storedProcedureName = "CREATE_DYNAMIC_PIVOT_MWDISH";
                        //using (OracleCommand procedureCommand = new OracleCommand(storedProcedureName, connection))
                        //{
                        //    procedureCommand.CommandType = CommandType.StoredProcedure;
                        //    procedureCommand.ExecuteNonQuery();
                        //}
                        var attActivated = _context.TLIattributeViewManagment.Include(x => x.EditableManagmentView).Include(x => x.AttributeActivated)
                            .Include(x => x.DynamicAtt).Where(x => x.Enable && x.EditableManagmentView.View == "RadioRRUInstallation" &&
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
                        propertyNamesStatic.Add("CIVIL_ID");
                        propertyNamesStatic.Add("SIDEARMNAME");
                        propertyNamesStatic.Add("SIDEARM_ID");
                        propertyNamesStatic.Add("ALLCIVILINST_ID");
                        propertyNamesStatic.Add("LEGID");
                        propertyNamesStatic.Add("LEGNAME");
                        propertyNamesStatic.Add("SiteCode");
                        if (SiteCode == null)
                        {
                            if (propertyNamesDynamic.Count == 0)
                            {
                                var query = _context.MV_RADIO_RRU_VIEW.Where(x => !x.Dismantle).AsEnumerable()
                                .Select(item => _unitOfWork.CivilWithLegsRepository.BuildDynamicSelect(item, null, propertyNamesStatic, propertyNamesDynamic));
                                int count = query.Count();
                                getEnableAttribute.Model = query;
                                string excelFilePath = ExportToExcel(query, "TLIradioRRU");

                                return new Response<string>(false, excelFilePath, null, null, (int)Helpers.Constants.ApiReturnCode.success);
                            }
                            else
                            {
                                var query = _context.MV_RADIO_RRU_VIEW.Where(x => !x.Dismantle).AsEnumerable()
                               .GroupBy(x => new
                               {
                                   SiteCode = x.SiteCode,
                                   Id = x.Id,
                                   Name = x.Name,
                                   Azimuth = x.Azimuth,
                                   Notes = x.Notes,
                                   VisibleStatus = x.VisibleStatus,
                                   SerialNumber = x.SerialNumber,
                                   HBA = x.HBA,
                                   SpaceInstallation = x.SpaceInstallation,
                                   HeightBase = x.HeightBase,
                                   HeightLand = x.HeightLand,
                                   INSTALLATIONPLACE = x.INSTALLATIONPLACE,
                                   RADIORRULIBRARY = x.RADIORRULIBRARY,
                                   RADIOANTENNA = x.RADIOANTENNA,
                                   Dismantle = x.Dismantle,
                                   CenterHigh = x.CenterHigh,
                                   HieghFromLand = x.HieghFromLand,
                                   EquivalentSpace = x.EquivalentSpace,
                                   LEGNAME = x.LEGNAME,
                                   CIVILNAME = x.CIVILNAME,
                                   CIVIL_ID = x.CIVIL_ID,
                                   SIDEARMNAME = x.SIDEARMNAME,
                                   SIDEARM_ID = x.SIDEARM_ID,
                                   ALLCIVILINST_ID = x.ALLCIVILINST_ID,
                                   LEGID = x.LEGID,
                                   OWNER = x.OWNER


                               })
                               .Select(x => new { key = x.Key, value = x.ToDictionary(z => z.Key, z => z.INPUTVALUE) })
                               .Select(item => _unitOfWork.CivilWithLegsRepository.BuildDynamicSelect(item.key, item.value, propertyNamesStatic, propertyNamesDynamic));

                                int count = query.Count();

                                getEnableAttribute.Model = query;
                                string excelFilePath = ExportToExcel(query, "TLIradioRRU");

                                return new Response<string>(false, excelFilePath, null, null, (int)Helpers.Constants.ApiReturnCode.success);

                            }
                        }
                        if (propertyNamesDynamic.Count == 0)
                        {
                            var query = _context.MV_RADIO_RRU_VIEW.Where(x => x.SiteCode.ToLower() == SiteCode.ToLower() && !x.Dismantle).AsEnumerable()
                            .Select(item => _unitOfWork.CivilWithLegsRepository.BuildDynamicSelect(item, null, propertyNamesStatic, propertyNamesDynamic));
                            int count = query.Count();
                            getEnableAttribute.Model = query;
                            string excelFilePath = ExportToExcel(query, "TLIradioRRU");

                            return new Response<string>(false, excelFilePath, null, null, (int)Helpers.Constants.ApiReturnCode.success);
                        }
                        else
                        {
                            var query = _context.MV_RADIO_RRU_VIEW.Where(x => x.SiteCode.ToLower() == SiteCode.ToLower() && !x.Dismantle).AsEnumerable()
                           .GroupBy(x => new
                           {
                               SiteCode = x.SiteCode,
                               Id = x.Id,
                               Name = x.Name,
                               Azimuth = x.Azimuth,
                               Notes = x.Notes,
                               VisibleStatus = x.VisibleStatus,
                               SerialNumber = x.SerialNumber,
                               HBA = x.HBA,
                               SpaceInstallation = x.SpaceInstallation,
                               HeightBase = x.HeightBase,
                               HeightLand = x.HeightLand,
                               INSTALLATIONPLACE = x.INSTALLATIONPLACE,
                               RADIORRULIBRARY = x.RADIORRULIBRARY,
                               RADIOANTENNA = x.RADIOANTENNA,
                               Dismantle = x.Dismantle,
                               CenterHigh = x.CenterHigh,
                               HieghFromLand = x.HieghFromLand,
                               EquivalentSpace = x.EquivalentSpace,
                               LEGNAME = x.LEGNAME,
                               CIVILNAME = x.CIVILNAME,
                               CIVIL_ID = x.CIVIL_ID,
                               SIDEARMNAME = x.SIDEARMNAME,
                               SIDEARM_ID = x.SIDEARM_ID,
                               ALLCIVILINST_ID = x.ALLCIVILINST_ID,
                               LEGID = x.LEGID,
                               OWNER = x.OWNER


                           })
                           .Select(x => new { key = x.Key, value = x.ToDictionary(z => z.Key, z => z.INPUTVALUE) })
                           .Select(item => _unitOfWork.CivilWithLegsRepository.BuildDynamicSelect(item.key, item.value, propertyNamesStatic, propertyNamesDynamic));

                            int count = query.Count();

                            getEnableAttribute.Model = query;
                            string excelFilePath = ExportToExcel(query, "TLIradioRRU");

                            return new Response<string>(false, excelFilePath, null, null, (int)Helpers.Constants.ApiReturnCode.success);
                        }
                    }
                    catch (Exception err)
                    {
                        return new Response<string>(false, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
                    }
                }
                else if (Helpers.Constants.LoadSubType.TLIradioOther.ToString() == TableNameInstallation)
                {
                    try
                    {
                        GetEnableAttribute getEnableAttribute = new GetEnableAttribute();
                        connection.Open();
                        //string storedProcedureName = "CREATE_DYNAMIC_PIVOT_MWDISH";
                        //using (OracleCommand procedureCommand = new OracleCommand(storedProcedureName, connection))
                        //{
                        //    procedureCommand.CommandType = CommandType.StoredProcedure;
                        //    procedureCommand.ExecuteNonQuery();
                        //}
                        var attActivated = _context.TLIattributeViewManagment.Include(x => x.EditableManagmentView).Include(x => x.AttributeActivated)
                            .Include(x => x.DynamicAtt).Where(x => x.Enable && x.EditableManagmentView.View == "RadioOtherInstallation" &&
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
                        propertyNamesStatic.Add("SiteCode");
                        propertyNamesStatic.Add("LEG_NAME");
                        propertyNamesStatic.Add("CIVILNAME");
                        propertyNamesStatic.Add("CIVIL_ID");
                        propertyNamesStatic.Add("SIDEARMNAME");
                        propertyNamesStatic.Add("SIDEARM_ID");
                        propertyNamesStatic.Add("ALLCIVILINST_ID");
                        propertyNamesStatic.Add("LEG_ID");
                        if (SiteCode == null)
                        {
                            if (propertyNamesDynamic.Count == 0)
                            {
                                var query = _context.MV_RADIO_OTHER_VIEW.Where(x => !x.Dismantle).AsEnumerable()
                               .Select(item => _unitOfWork.CivilWithLegsRepository.BuildDynamicSelect(item, null, propertyNamesStatic, propertyNamesDynamic));
                                int count = query.Count();
                                getEnableAttribute.Model = query;
                                string excelFilePath = ExportToExcel(query, "TLIradioOther");

                                return new Response<string>(false, excelFilePath, null, null, (int)Helpers.Constants.ApiReturnCode.success);
                            }
                            else
                            {
                                var query = _context.MV_RADIO_OTHER_VIEW.Where(x => !x.Dismantle).AsEnumerable()
                               .GroupBy(x => new
                               {
                                   SiteCode = x.SiteCode,
                                   Id = x.Id,
                                   Name = x.Name,
                                   Azimuth = x.Azimuth,
                                   Notes = x.Notes,
                                   HeightBase = x.HeightBase,
                                   HeightLand = x.HeightLand,
                                   OWNER = x.OWNER,
                                   SerialNumber = x.SerialNumber,
                                   HieghFromLand = x.HieghFromLand,
                                   Spaceinstallation = x.Spaceinstallation,
                                   RADIOOTHERLIBRARY = x.RADIOOTHERLIBRARY,
                                   INSTALLATIONPLACE = x.INSTALLATIONPLACE,
                                   CenterHigh = x.CenterHigh,
                                   HBA = x.HBA,
                                   EquivalentSpace = x.EquivalentSpace,
                                   Dismantle = x.Dismantle,
                                   LEG_NAME = x.LEG_NAME,
                                   CIVILNAME = x.CIVILNAME,
                                   CIVIL_ID = x.CIVIL_ID,
                                   SIDEARMNAME = x.SIDEARMNAME,
                                   SIDEARM_ID = x.SIDEARM_ID,
                                   ALLCIVILINST_ID = x.ALLCIVILINST_ID,
                                   LEG_ID = x.LEG_ID,
                                   SideArmSec_Name = x.SideArmSec_Name,
                                   SideArmSec_Id = x.SideArmSec_Id


                               })
                               .Select(x => new { key = x.Key, value = x.ToDictionary(z => z.Key, z => z.INPUTVALUE) })
                               .Select(item => _unitOfWork.CivilWithLegsRepository.BuildDynamicSelect(item.key, item.value, propertyNamesStatic, propertyNamesDynamic));

                                int count = query.Count();

                                getEnableAttribute.Model = query;
                                string excelFilePath = ExportToExcel(query, "TLIradioOther");

                                return new Response<string>(false, excelFilePath, null, null, (int)Helpers.Constants.ApiReturnCode.success);
                            }
                        }
                        if (propertyNamesDynamic.Count == 0)
                        {
                            var query = _context.MV_RADIO_OTHER_VIEW.Where(x => x.SiteCode.ToLower() == SiteCode.ToLower() && !x.Dismantle).AsEnumerable()
                            .Select(item => _unitOfWork.CivilWithLegsRepository.BuildDynamicSelect(item, null, propertyNamesStatic, propertyNamesDynamic));
                            int count = query.Count();
                            getEnableAttribute.Model = query;
                            string excelFilePath = ExportToExcel(query, "TLIradioOther");

                            return new Response<string>(false, excelFilePath, null, null, (int)Helpers.Constants.ApiReturnCode.success);
                        }
                        else
                        {
                            var query = _context.MV_RADIO_OTHER_VIEW.Where(x => x.SiteCode.ToLower() == SiteCode.ToLower() && !x.Dismantle).AsEnumerable()
                           .GroupBy(x => new
                           {
                               SiteCode = x.SiteCode,
                               Id = x.Id,
                               Name = x.Name,
                               Azimuth = x.Azimuth,
                               Notes = x.Notes,
                               HeightBase = x.HeightBase,
                               HeightLand = x.HeightLand,
                               OWNER = x.OWNER,
                               SerialNumber = x.SerialNumber,
                               HieghFromLand = x.HieghFromLand,
                               Spaceinstallation = x.Spaceinstallation,
                               RADIOOTHERLIBRARY = x.RADIOOTHERLIBRARY,
                               INSTALLATIONPLACE = x.INSTALLATIONPLACE,
                               CenterHigh = x.CenterHigh,
                               HBA = x.HBA,
                               EquivalentSpace = x.EquivalentSpace,
                               Dismantle = x.Dismantle,
                               LEG_NAME = x.LEG_NAME,
                               CIVILNAME = x.CIVILNAME,
                               CIVIL_ID = x.CIVIL_ID,
                               SIDEARMNAME = x.SIDEARMNAME,
                               SIDEARM_ID = x.SIDEARM_ID,
                               ALLCIVILINST_ID = x.ALLCIVILINST_ID,
                               LEG_ID = x.LEG_ID,
                               SideArmSec_Name = x.SideArmSec_Name,
                               SideArmSec_Id = x.SideArmSec_Id


                           })
                           .Select(x => new { key = x.Key, value = x.ToDictionary(z => z.Key, z => z.INPUTVALUE) })
                           .Select(item => _unitOfWork.CivilWithLegsRepository.BuildDynamicSelect(item.key, item.value, propertyNamesStatic, propertyNamesDynamic));

                            int count = query.Count();

                            getEnableAttribute.Model = query;
                            string excelFilePath = ExportToExcel(query, "TLIradioOther");

                            return new Response<string>(false, excelFilePath, null, null, (int)Helpers.Constants.ApiReturnCode.success);
                        }
                    }
                    catch (Exception err)
                    {
                        return new Response<string>(false, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
                    }
                }
                else if (Helpers.Constants.OtherInventoryType.TLIcabinetPower.ToString() == TableNameInstallation)
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
                        var attActivated = _context.TLIattributeViewManagment
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
                        propertyNamesStatic.Remove("CabinetTelecomLibrary");

                        if (SiteCode == null)
                        {
                            if (propertyNamesDynamic.Count == 0)
                            {
                                var query = _context.MV_CABINET_POWER_VIEW.Where(x =>
                                !x.Dismantle).AsEnumerable()
                                .Select(item => _unitOfWork.CivilWithLegsRepository.BuildDynamicSelect(item, null, propertyNamesStatic, propertyNamesDynamic));
                                int count = query.Count();

                                getEnableAttribute.Model = query;
                                string excelFilePath = ExportToExcel(query, "TLIcabinetPower");

                                return new Response<string>(false, excelFilePath, null, null, (int)Helpers.Constants.ApiReturnCode.success);
                            }
                            else
                            {
                                var query = _context.MV_CABINET_POWER_VIEW.Where(x =>
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
                                string excelFilePath = ExportToExcel(query, "TLIcabinetPower");

                                return new Response<string>(false, excelFilePath, null, null, (int)Helpers.Constants.ApiReturnCode.success);
                            }
                        }
                        if (propertyNamesDynamic.Count == 0)
                        {
                            var query = _context.MV_CABINET_POWER_VIEW.Where(x => x.SITECODE.ToLower() == SiteCode.ToLower()
                            && !x.Dismantle).AsEnumerable()
                        .Select(item => _unitOfWork.CivilWithLegsRepository.BuildDynamicSelect(item, null, propertyNamesStatic, propertyNamesDynamic));
                            int count = query.Count();

                            getEnableAttribute.Model = query;
                            string excelFilePath = ExportToExcel(query, "TLIcabinetPower");

                            return new Response<string>(false, excelFilePath, null, null, (int)Helpers.Constants.ApiReturnCode.success);
                        }
                        else
                        {
                            var query = _context.MV_CABINET_POWER_VIEW.Where(x => x.SITECODE.ToLower() == SiteCode.ToLower()
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
                            string excelFilePath = ExportToExcel(query, "TLIcabinetPower");

                            return new Response<string>(false, excelFilePath, null, null, (int)Helpers.Constants.ApiReturnCode.success);
                        }

                    }
                    catch (Exception err)
                    {
                        return new Response<string>(false, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
                    }
                }
                else if (Helpers.Constants.OtherInventoryType.TLIcabinetTelecom.ToString() == TableNameInstallation)
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
                        var attActivated = _context.TLIattributeViewManagment
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
                                var query = _context.MV_CABINET_TELECOM_VIEW.Where(x =>
                                !x.Dismantle).AsEnumerable()
                                .Select(item => _unitOfWork.CivilWithLegsRepository.BuildDynamicSelect(item, null, propertyNamesStatic, propertyNamesDynamic));
                                int count = query.Count();

                                getEnableAttribute.Model = query;
                                string excelFilePath = ExportToExcel(query, "TLIcabinetTelecom");

                                return new Response<string>(false, excelFilePath, null, null, (int)Helpers.Constants.ApiReturnCode.success);
                            }
                            else
                            {
                                var query = _context.MV_CABINET_TELECOM_VIEW.Where(x =>
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
                                string excelFilePath = ExportToExcel(query, "TLIcabinetTelecom");

                                return new Response<string>(false, excelFilePath, null, null, (int)Helpers.Constants.ApiReturnCode.success);
                            }
                        }
                        if (propertyNamesDynamic.Count == 0)
                        {
                            var query = _context.MV_CABINET_TELECOM_VIEW.Where(x => x.SITECODE.ToLower() == SiteCode.ToLower()
                            && !x.Dismantle).AsEnumerable()
                        .Select(item => _unitOfWork.CivilWithLegsRepository.BuildDynamicSelect(item, null, propertyNamesStatic, propertyNamesDynamic));
                            int count = query.Count();

                            getEnableAttribute.Model = query;
                            string excelFilePath = ExportToExcel(query, "TLIcabinetTelecom");

                            return new Response<string>(false, excelFilePath, null, null, (int)Helpers.Constants.ApiReturnCode.success);
                        }
                        else
                        {
                            var query = _context.MV_CABINET_TELECOM_VIEW.Where(x => x.SITECODE.ToLower() == SiteCode.ToLower()
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
                            string excelFilePath = ExportToExcel(query, "TLIcabinetTelecom");

                            return new Response<string>(false, excelFilePath, null, null, (int)Helpers.Constants.ApiReturnCode.success);
                        }

                    }
                    catch (Exception err)
                    {
                        return new Response<string>(false, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
                    }
                }
                else if (Helpers.Constants.OtherInventoryType.TLIsolar.ToString() == TableNameInstallation)
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
                        var attActivated = _context.TLIattributeViewManagment
                            .Include(x => x.EditableManagmentView)
                            .Include(x => x.AttributeActivated)
                            .Include(x => x.DynamicAtt)
                            .Where(x => x.Enable && x.EditableManagmentView.View == "SolarInstallation" &&
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
                                var query = _context.MV_SOLAR_VIEW.Where(x =>
                                !x.Dismantle).AsEnumerable()
                                .Select(item => _unitOfWork.CivilWithLegsRepository.BuildDynamicSelect(item, null, propertyNamesStatic, propertyNamesDynamic));
                                int count = query.Count();

                                getEnableAttribute.Model = query;
                                string excelFilePath = ExportToExcel(query, "TLIsolar");

                                return new Response<string>(false, excelFilePath, null, null, (int)Helpers.Constants.ApiReturnCode.success);
                            }
                            else
                            {
                                var query = _context.MV_SOLAR_VIEW.Where(x =>
                                  !x.Dismantle).AsEnumerable()
                            .GroupBy(x => new
                            {
                                Id = x.Id,
                                Name = x.Name,
                                SITECODE = x.SITECODE,
                                PVPanelBrandAndWattage = x.PVPanelBrandAndWattage,
                                PVArrayAzimuth = x.PVArrayAzimuth,
                                PVArrayAngel = x.PVArrayAngel,
                                SpaceInstallation = x.SpaceInstallation,
                                VisibleStatus = x.VisibleStatus,
                                Prefix = x.Prefix,
                                PowerLossRatio = x.PowerLossRatio,
                                NumberOfSSU = x.NumberOfSSU,
                                NumberOfLightingRod = x.NumberOfLightingRod,
                                NumberOfInstallPVs = x.NumberOfInstallPVs,
                                LocationDescription = x.LocationDescription,
                                ExtenstionDimension = x.ExtenstionDimension,
                                Extension = x.Extension,
                                SOLARLIBRARY = x.SOLARLIBRARY,
                                CABINET = x.CABINET,
                                Dismantle = x.Dismantle,

                            }).OrderBy(x => x.Key.Name)
                            .Select(x => new { key = x.Key, value = x.ToDictionary(z => z.Key, z => z.INPUTVALUE) })
                            .Select(item => _unitOfWork.CivilWithLegsRepository.BuildDynamicSelect(item.key, item.value, propertyNamesStatic, propertyNamesDynamic));
                                int count = query.Count();
                                getEnableAttribute.Model = query;
                                string excelFilePath = ExportToExcel(query, "TLIsolar");

                                return new Response<string>(false, excelFilePath, null, null, (int)Helpers.Constants.ApiReturnCode.success);
                            }
                        }
                        if (propertyNamesDynamic.Count == 0)
                        {
                            var query = _context.MV_SOLAR_VIEW.Where(x => x.SITECODE.ToLower() == SiteCode.ToLower()
                            && !x.Dismantle).AsEnumerable()
                        .Select(item => _unitOfWork.CivilWithLegsRepository.BuildDynamicSelect(item, null, propertyNamesStatic, propertyNamesDynamic));
                            int count = query.Count();

                            getEnableAttribute.Model = query;
                            string excelFilePath = ExportToExcel(query, "TLIsolar");

                            return new Response<string>(false, excelFilePath, null, null, (int)Helpers.Constants.ApiReturnCode.success);
                        }
                        else
                        {
                            var query = _context.MV_SOLAR_VIEW.Where(x => x.SITECODE.ToLower() == SiteCode.ToLower()
                             && !x.Dismantle).AsEnumerable()
                        .GroupBy(x => new
                        {
                            Id = x.Id,
                            Name = x.Name,
                            SITECODE = x.SITECODE,
                            PVPanelBrandAndWattage = x.PVPanelBrandAndWattage,
                            PVArrayAzimuth = x.PVArrayAzimuth,
                            PVArrayAngel = x.PVArrayAngel,
                            SpaceInstallation = x.SpaceInstallation,
                            VisibleStatus = x.VisibleStatus,
                            Prefix = x.Prefix,
                            PowerLossRatio = x.PowerLossRatio,
                            NumberOfSSU = x.NumberOfSSU,
                            NumberOfLightingRod = x.NumberOfLightingRod,
                            NumberOfInstallPVs = x.NumberOfInstallPVs,
                            LocationDescription = x.LocationDescription,
                            ExtenstionDimension = x.ExtenstionDimension,
                            Extension = x.Extension,
                            SOLARLIBRARY = x.SOLARLIBRARY,
                            CABINET = x.CABINET,
                            Dismantle = x.Dismantle,

                        }).OrderBy(x => x.Key.Name)
                        .Select(x => new { key = x.Key, value = x.ToDictionary(z => z.Key, z => z.INPUTVALUE) })
                        .Select(item => _unitOfWork.CivilWithLegsRepository.BuildDynamicSelect(item.key, item.value, propertyNamesStatic, propertyNamesDynamic));
                            int count = query.Count();
                            getEnableAttribute.Model = query;
                            string excelFilePath = ExportToExcel(query, "TLIsolar");

                            return new Response<string>(false, excelFilePath, null, null, (int)Helpers.Constants.ApiReturnCode.success);
                        }

                    }
                    catch (Exception err)
                    {
                        return new Response<string>(false, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
                    }
                }
                else if (Helpers.Constants.OtherInventoryType.TLIgenerator.ToString() == TableNameInstallation)
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
                        var attActivated = _context.TLIattributeViewManagment
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
                                var query = _context.MV_GENERATOR_VIEW.Where(x =>
                                !x.Dismantle).AsEnumerable()
                                .Select(item => _unitOfWork.CivilWithLegsRepository.BuildDynamicSelect(item, null, propertyNamesStatic, propertyNamesDynamic));
                                int count = query.Count();

                                getEnableAttribute.Model = query;
                                string excelFilePath = ExportToExcel(query, "TLIgenerator");

                                return new Response<string>(false, excelFilePath, null, null, (int)Helpers.Constants.ApiReturnCode.success);
                            }
                            else
                            {
                                var query = _context.MV_GENERATOR_VIEW.Where(x =>
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
                                string excelFilePath = ExportToExcel(query, "TLIgenerator");

                                return new Response<string>(false, excelFilePath, null, null, (int)Helpers.Constants.ApiReturnCode.success);
                            }
                        }
                        if (propertyNamesDynamic.Count == 0)
                        {
                            var query = _context.MV_GENERATOR_VIEW.Where(x => x.SITECODE.ToLower() == SiteCode.ToLower()
                            && !x.Dismantle).AsEnumerable()
                        .Select(item => _unitOfWork.CivilWithLegsRepository.BuildDynamicSelect(item, null, propertyNamesStatic, propertyNamesDynamic));
                            int count = query.Count();

                            getEnableAttribute.Model = query;
                            string excelFilePath = ExportToExcel(query, "TLIgenerator");

                            return new Response<string>(false, excelFilePath, null, null, (int)Helpers.Constants.ApiReturnCode.success);
                        }
                        else
                        {
                            var query = _context.MV_GENERATOR_VIEW.Where(x => x.SITECODE.ToLower() == SiteCode.ToLower()
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
                            string excelFilePath = ExportToExcel(query, "TLIgenerator");

                            return new Response<string>(false, excelFilePath, null, null, (int)Helpers.Constants.ApiReturnCode.success);
                        }

                    }
                    catch (Exception err)
                    {
                        return new Response<string>(false, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
                    }
                }

                return new Response<string>(false, null, null, null, (int)Helpers.Constants.ApiReturnCode.success);
            }
        }


        public string ExportToExcel(IEnumerable<IDictionary<string, object>> data, string TableName)
        {
            var folderPath = _configuration["StoreFiles"];
            string downloadFolder = Path.Combine(folderPath, "GenerateFiles");

            if (!Directory.Exists(downloadFolder))
            {
                Directory.CreateDirectory(downloadFolder);
            }

            var filePath = Path.Combine(downloadFolder, TableName + ".xlsx");
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Sheet1");

                if (data.Any())
                {
                   
                    var headers = data.First().Keys.ToList();
                    var formattedHeaders = headers.Select(header => FormatHeader(header)).ToList();

                    for (int i = 0; i < formattedHeaders.Count; i++)
                    {
                        worksheet.Cell(1, i + 1).Value = formattedHeaders[i];
                    }

                    
                    int row = 2;
                    foreach (var rowData in data)
                    {
                        for (int col = 0; col < formattedHeaders.Count; col++)
                        {
                            var header = headers[col];
                            var value = rowData.ContainsKey(header) ? rowData[header]?.ToString() : string.Empty;
                            worksheet.Cell(row, col + 1).Value = value;
                        }
                        row++;
                    }
                }
                else
                {
                    worksheet.Cell(1, 1).Value = "No data available to export.";
                }

                workbook.SaveAs(filePath);
            }
            
            return filePath;
        }


        private string FormatHeader(string header)
        {
            if (string.IsNullOrWhiteSpace(header))
            {
                return header;
            }

            var formattedHeader = new StringBuilder();
            bool previousWasUpper = false;

            foreach (var ch in header)
            {
                if (ch == '_')
                {
                    formattedHeader.Append(' ');
                }
                else if (char.IsUpper(ch))
                {
                    if (previousWasUpper)
                    {
                        formattedHeader.Append(ch);
                    }
                    else
                    {
                        if (formattedHeader.Length > 0)
                        {
                            formattedHeader.Append(' ');
                        }
                        formattedHeader.Append(ch);
                    }
                    previousWasUpper = true;
                }
                else
                {
                    formattedHeader.Append(ch);
                    previousWasUpper = false;
                }
            }

            return formattedHeader.ToString();
        }

        public Response<IEnumerable<TLIlogUsersActionsViewModel>> GetLogsWithPaginationAndSorting(FilterRequest filterRequest)
        {
            try
            {
                var ConnectionString = _configuration["ConnectionStrings:ActiveConnection"];
                string filtersJson = null;
                if (filterRequest?.Filters != null && filterRequest.Filters.Any())
                {
                    filtersJson = System.Text.Json.JsonSerializer.Serialize(filterRequest.Filters);
                }

                var logs = new List<TLIlogUsersActions>();
                var logsViewModel = new List<TLIlogUsersActionsViewModel>();

                int totalCount = 0;

                using (var connection = new OracleConnection(ConnectionString))
                {
                    connection.Open();

                    using (var command = new OracleCommand("GetLogsByFilterPaginationAndSorting", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        // إعداد المعاملات
                        var resultParam = new OracleParameter("result", OracleDbType.RefCursor)
                        {
                            Direction = ParameterDirection.Output
                        };
                        command.Parameters.Add(resultParam);

                        var totalCountParam = new OracleParameter("total_count", OracleDbType.Int32)
                        {
                            Direction = ParameterDirection.Output
                        };
                        command.Parameters.Add(totalCountParam);

                        command.Parameters.Add("filters", OracleDbType.Clob).Value = filtersJson ?? (object)DBNull.Value;
                        command.Parameters.Add("first", OracleDbType.Int32).Value = filterRequest.First ?? 0;
                        command.Parameters.Add("rows", OracleDbType.Int32).Value = filterRequest.Rows ?? 10;
                        command.Parameters.Add("sort_field", OracleDbType.Varchar2).Value = filterRequest.MultiSortMeta?.FirstOrDefault()?.Field ?? "Id";
                        command.Parameters.Add("sort_order", OracleDbType.Int32).Value = filterRequest.MultiSortMeta?.FirstOrDefault()?.Order ?? 1;

                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var log = new TLIlogUsersActions
                                {
                                    Id = Convert.ToInt32(reader["Id"]),
                                    Date = Convert.ToDateTime(reader["Date"]),
                                    UserId = Convert.ToInt32(reader["UserId"]),
                                    ControllerName = reader["ControllerName"]?.ToString(),
                                    FunctionName = reader["FunctionName"]?.ToString(),
                                    BodyParameters = reader["BodyParameters"]?.ToString(),
                                    HeaderParameters = reader["HeaderParameters"]?.ToString(),
                                    ResponseStatus = reader["ResponseStatus"]?.ToString(),
                                    Result = reader["Result"]?.ToString()
                                };

                                logs.Add(log);
                            }
                        }

                        totalCount = ((OracleDecimal)totalCountParam.Value).ToInt32();
                    }
                }

                foreach (var log in logs)
                {
                    using (var connection = new OracleConnection(ConnectionString))
                    {
                        connection.Open();
                        using (var command = new OracleCommand("SELECT \"UserName\" FROM \"TLIuser\" WHERE \"Id\" = :UserId", connection))
                        {
                            command.Parameters.Add(new OracleParameter("UserId", log.UserId));
                            var userName = command.ExecuteScalar()?.ToString();

                            var logViewModel = new TLIlogUsersActionsViewModel
                            {
                                Id = log.Id,
                                Date = log.Date,
                                UserName = userName,
                                ControllerName = log.ControllerName,
                                FunctionName = log.FunctionName,
                                BodyParameters = log.BodyParameters,
                                HeaderParameters = log.HeaderParameters,
                                ResponseStatus = log.ResponseStatus,
                                Result = log.Result
                            };

                            logsViewModel.Add(logViewModel);
                        }
                    }
                }

                return new Response<IEnumerable<TLIlogUsersActionsViewModel>>
                {
                    Data = logsViewModel,
                    Count = totalCount,
                    Succeeded = true
                };
            }
            catch (Exception ex)
            {
                return new Response<IEnumerable<TLIlogUsersActionsViewModel>>
                {
                    Data = null,
                    Succeeded = false,
                    Message = ex.Message
                };
            }
        }




        public Response<string> ClearAllHistory(string connectionString, string dateFrom, string dateTo)
        {
            const int batchSize = 10000; // حجم الدفعة
            try
            {
                DateTime parsedDateFrom, parsedDateTo;

                // تنسيق التاريخ
                string[] formats = { "yyyy-MM-dd", "dd-MMM-yy", "d-MMM-yy" };

                // التحقق من تنسيق DateFrom
                if (!DateTime.TryParseExact(dateFrom, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedDateFrom))
                {
                    return new Response<string>(false, null, null, $"Invalid DateFrom format. Received: {dateFrom}", (int)Helpers.Constants.ApiReturnCode.fail);
                }

                // التحقق من تنسيق DateTo
                if (!DateTime.TryParseExact(dateTo, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedDateTo))
                {
                    return new Response<string>(false, null, null, $"Invalid DateTo format. Received: {dateTo}", (int)Helpers.Constants.ApiReturnCode.fail);
                }

                // التأكد من ضبط الوقت على 12:00:00 AM
                parsedDateFrom = parsedDateFrom.Date;
                parsedDateTo = parsedDateTo.Date;

                using (var connection = new OracleConnection(connectionString))
                {
                    connection.Open();
                    using (var transaction = connection.BeginTransaction()) // بدء معاملة
                    {
                        try
                        {
                            using (var command = connection.CreateCommand())
                            {
                                command.Transaction = transaction;

                                // حذف البيانات من TLIhistoryDet على دفعات
                                while (true)
                                {
                                    command.CommandText = @"
                                DELETE FROM ""TLIhistoryDet""
                                WHERE ""HistoryId"" IN (
                                    SELECT ""Id""
                                    FROM ""TLIhistory""
                                    WHERE TRUNC(""HistoryDate"") BETWEEN TRUNC(:DateFrom) AND TRUNC(:DateTo)
                                )";

                                    // إعداد المعاملات
                                    command.Parameters.Clear();
                                    command.Parameters.Add(new OracleParameter("DateFrom", OracleDbType.Date) { Value = parsedDateFrom });
                                    command.Parameters.Add(new OracleParameter("DateTo", OracleDbType.Date) { Value = parsedDateTo });

                                    int deletedRows = command.ExecuteNonQuery();

                                    if (deletedRows == 0)
                                        break; // الخروج إذا لم يتم حذف أي صفوف
                                }

                                // حذف البيانات من TLIhistory على دفعات
                                while (true)
                                {
                                    command.CommandText = @"
                                DELETE FROM ""TLIhistory""
                                WHERE TRUNC(""HistoryDate"") BETWEEN TRUNC(:DateFrom) AND TRUNC(:DateTo)";

                                    // إعداد المعاملات
                                    command.Parameters.Clear();
                                    command.Parameters.Add(new OracleParameter("DateFrom", OracleDbType.Date) { Value = parsedDateFrom });
                                    command.Parameters.Add(new OracleParameter("DateTo", OracleDbType.Date) { Value = parsedDateTo });

                                    int deletedRows = command.ExecuteNonQuery();

                                    if (deletedRows == 0)
                                        break; // الخروج إذا لم يتم حذف أي صفوف
                                }
                            }

                            transaction.Commit(); // تأكيد المعاملة
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback(); // إلغاء المعاملة عند حدوث خطأ
                            throw new Exception($"Error during history deletion: {ex.Message}", ex);
                        }
                    }
                }

                return new Response<string>(true, "All history cleared successfully", null, null, (int)Helpers.Constants.ApiReturnCode.success);
            }
            catch (Exception err)
            {
                // تسجيل الخطأ أو معالجته
                return new Response<string>(false, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
        }

        public Response<string> ClearLogHistory(string connectionString, string dateFrom = null, string dateTo = null)
        {
            const int batchSize = 10000; // حجم الدفعة
            try
            {
                DateTime? parsedDateFrom = null, parsedDateTo = null;

                // التحقق من وجود الفلاتر والتأكد من صحتها
                if (!string.IsNullOrEmpty(dateFrom))
                {
                    string[] formats = { "yyyy-MM-dd", "dd-MMM-yy", "d-MMM-yy" };

                    // التحقق من تنسيق DateFrom
                    if (!DateTime.TryParseExact(dateFrom, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime tempDateFrom))
                    {
                        return new Response<string>(false, null, null, $"Invalid DateFrom format. Received: {dateFrom}", (int)Helpers.Constants.ApiReturnCode.fail);
                    }
                    parsedDateFrom = tempDateFrom.Date;  // حفظ التاريخ بدون الوقت
                }

                if (!string.IsNullOrEmpty(dateTo))
                {
                    string[] formats = { "yyyy-MM-dd", "dd-MMM-yy", "d-MMM-yy" };

                    // التحقق من تنسيق DateTo
                    if (!DateTime.TryParseExact(dateTo, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime tempDateTo))
                    {
                        return new Response<string>(false, null, null, $"Invalid DateTo format. Received: {dateTo}", (int)Helpers.Constants.ApiReturnCode.fail);
                    }
                    parsedDateTo = tempDateTo.Date;  // حفظ التاريخ بدون الوقت
                }

                using (var connection = new OracleConnection(connectionString))
                {
                    connection.Open();
                    using (var transaction = connection.BeginTransaction()) // بدء معاملة
                    {
                        try
                        {
                            using (var command = connection.CreateCommand())
                            {
                                command.Transaction = transaction;

                                // إذا كانت الفلاتر موجودة، حذف البيانات وفقًا للتواريخ
                                if (parsedDateFrom.HasValue && parsedDateTo.HasValue)
                                {
                                    command.CommandText = @"
                            DELETE FROM ""TLIlogUsersActions""
                            WHERE ""Date"" BETWEEN :DateFrom AND :DateTo";

                                    // إعداد المعاملات
                                    command.Parameters.Clear();
                                    command.Parameters.Add(new OracleParameter("DateFrom", OracleDbType.Date) { Value = parsedDateFrom.Value });
                                    command.Parameters.Add(new OracleParameter("DateTo", OracleDbType.Date) { Value = parsedDateTo.Value });
                                }
                                else
                                {
                                    // إذا لم تكن هناك فلاتر، حذف جميع السجلات
                                    command.CommandText = "DELETE FROM \"TLIlogUsersActions\"";
                                }

                                // تنفيذ الأمر
                                int deletedRows = command.ExecuteNonQuery();

                                if (deletedRows == 0)
                                {
                                    // لا يوجد سجلات تم حذفها
                                    return new Response<string>(false, null, null, "No records found to delete", (int)Helpers.Constants.ApiReturnCode.fail);
                                }
                            }

                            transaction.Commit(); // تأكيد المعاملة
                        }
                        catch (Exception)
                        {
                            transaction.Rollback(); // إلغاء المعاملة عند حدوث خطأ
                            throw;
                        }
                    }
                }

                return new Response<string>(true, "Log history cleared successfully", null, null, (int)Helpers.Constants.ApiReturnCode.success);
            }
            catch (Exception err)
            {
                // تسجيل الخطأ أو معالجته
                return new Response<string>(false, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
        }




    }

}




















