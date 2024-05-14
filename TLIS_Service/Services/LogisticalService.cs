using AutoMapper;
using Castle.Core.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Engineering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TLIS_DAL;
using TLIS_DAL.Helper;
using TLIS_DAL.Helpers;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModelBase;
using TLIS_DAL.ViewModels.LogisticalDTOs;
using TLIS_Repository.Base;
using TLIS_Service.IService;

namespace TLIS_Service.Services
{
    public class LogisticalService : ILogisticalService
    {
        IUnitOfWork _unitOfWork;
        private IMapper _mapper;
        private readonly ApplicationDbContext _dbContext;
        public LogisticalService(IUnitOfWork unitOfWork, IMapper mapper, ApplicationDbContext dbContext)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _dbContext = dbContext;
        }
        public Response<MainLogisticalViewModel> GetById(int LogisticalId)
        {
            MainLogisticalViewModel Logistical = _mapper.Map<MainLogisticalViewModel>(_unitOfWork.LogistcalRepository.
                GetIncludeWhereFirst(x => x.Id == LogisticalId, x => x.logisticalType, x => x.tablePartName));

            return new Response<MainLogisticalViewModel>(true, Logistical, null, null, (int)Helpers.Constants.ApiReturnCode.success);
        }
        public Response<List<MainLogisticalViewModel>> GetLogisticalByTypeOrPart(string TablePartName, string LogisticalType, string Search, ParameterPagination parameterPagination)
        {
            List<MainLogisticalViewModel> Result = _mapper.Map<List<MainLogisticalViewModel>>(_unitOfWork.LogistcalRepository.GetIncludeWhere(x =>
                x.tablePartName.PartName.ToLower() == TablePartName.ToLower() &&
                x.logisticalType.Name.ToLower() == LogisticalType.ToLower() &&
                !x.Deleted, x => x.tablePartName, x => x.logisticalType).ToList());

            if (!string.IsNullOrEmpty(Search))
                Result = Result.Where(x => x.Name.ToLower().StartsWith(Search.ToLower())).ToList();

            int Count = Result.Count;

            Result = Result.Skip((parameterPagination.PageNumber - 1) * parameterPagination.PageSize).
                    Take(parameterPagination.PageSize).ToList();

            return new Response<List<MainLogisticalViewModel>>(true, Result, null, null, (int)Helpers.Constants.ApiReturnCode.success, Count);
        }
        public Response<bool> AddLogistical(AddNewLogistical NewLogistical)
        {
            int TablePartNameId = _unitOfWork.TablePartNameRepository
                .GetWhereFirst(x => x.PartName.ToLower() == NewLogistical.TablePartName.ToLower()).Id;

            TLIlogistical CheckLogistical = _unitOfWork.LogistcalRepository
                .GetWhereFirst(x => x.Name.ToLower() == NewLogistical.Name.ToLower() && !x.Deleted &&
                    x.tablePartNameId == TablePartNameId && x.logisticalTypeId == NewLogistical.LogisticalTypeId);

            if (CheckLogistical != null)
                return new Response<bool>(true, false, null, "This logistical is already exist", (int)Helpers.Constants.ApiReturnCode.fail);

            TLIlogistical Logistical = new TLIlogistical
            {
                Description = NewLogistical.Description,
                tablePartNameId = TablePartNameId,
                Active = true,
                Deleted = false,
                logisticalTypeId = NewLogistical.LogisticalTypeId,
                Name = NewLogistical.Name
            };

            _unitOfWork.LogistcalRepository.Add(Logistical);
            _unitOfWork.SaveChanges();

            return new Response<bool>(true, true, null, null, (int)Helpers.Constants.ApiReturnCode.success);
        }
        public Response<List<TableAffected>> DeleteLogistical(int LogisticalId)
        {
            try
            {
                TLIlogistical Logistical = _unitOfWork.LogistcalRepository
               .GetIncludeWhereFirst(x => x.Id == LogisticalId, x => x.tablePartName);
                if (Logistical != null)
                {
                    List<TLIlogisticalitem> AffectedLogisticalItems = _unitOfWork.LogisticalitemRepository
                        .GetIncludeWhere(x => x.logisticalId == LogisticalId, x => x.tablesNames).ToList();

                    if (AffectedLogisticalItems.Count != 0)
                    {
                        List<TableAffected> ListOfResponse = new List<TableAffected>();

                        if (Logistical.tablePartName.PartName.ToLower() == Helpers.Constants.TablePartName.CivilSupport.ToString().ToLower())
                        {
                            // CivilWithLeg..
                            List<TLIlogisticalitem> CivilWithLegLibraryLogisticalItems = AffectedLogisticalItems
                                .Where(x => x.tablesNames.TableName.ToLower() == Helpers.Constants.TablesNames.TLIcivilWithLegLibrary.ToString().ToLower()).ToList();

                            if (CivilWithLegLibraryLogisticalItems.Count != 0)
                            {
                                List<TLIcivilWithLegLibrary> CivilWithLegLibraries = _unitOfWork.CivilWithLegLibraryRepository
                                    .GetWhere(x => CivilWithLegLibraryLogisticalItems.Select(x => x.RecordId).Contains(x.Id) && !x.Deleted).ToList();

                                ListOfResponse.Add(new TableAffected()
                                {
                                    TableName = "Civil Steel Support With Legs Library",
                                    isLibrary = true,
                                    RecordsAffected = CivilWithLegLibraries.Select(x => new RecordAffected
                                    {
                                        RecordName = x.Model,
                                        SiteCode = null
                                    }).ToList()
                                });
                            }

                            // CivilWithoutLeg..
                            List<TLIlogisticalitem> CivilWithoutLegLibraryLogisticalItems = AffectedLogisticalItems
                                .Where(x => x.tablesNames.TableName.ToLower() == Helpers.Constants.TablesNames.TLIcivilWithoutLegLibrary.ToString().ToLower()).ToList();

                            if (CivilWithoutLegLibraryLogisticalItems.Count != 0)
                            {
                                List<TLIcivilWithoutLegLibrary> CivilWithoutLegLibraries = _unitOfWork.CivilWithoutLegLibraryRepository
                                    .GetWhere(x => CivilWithoutLegLibraryLogisticalItems.Select(x => x.RecordId).Contains(x.Id) && !x.Deleted).ToList();

                                ListOfResponse.Add(new TableAffected()
                                {
                                    TableName = "Civil Steel Support Without Legs Library",
                                    isLibrary = true,
                                    RecordsAffected = CivilWithoutLegLibraries.Select(x => new RecordAffected
                                    {
                                        RecordName = x.Model,
                                        SiteCode = null
                                    }).ToList()
                                });
                            }

                            // CivilNonSteel..
                            List<TLIlogisticalitem> CivilNonSteelLibraryLogisticalItems = AffectedLogisticalItems
                                .Where(x => x.tablesNames.TableName.ToLower() == Helpers.Constants.TablesNames.TLIcivilNonSteelLibrary.ToString().ToLower()).ToList();

                            if (CivilNonSteelLibraryLogisticalItems.Count != 0)
                            {
                                List<TLIcivilNonSteelLibrary> CivilNonSteelLibraries = _unitOfWork.CivilNonSteelLibraryRepository
                                    .GetWhere(x => CivilNonSteelLibraryLogisticalItems.Select(x => x.RecordId).Contains(x.Id) && !x.Deleted).ToList();

                                ListOfResponse.Add(new TableAffected()
                                {
                                    TableName = "Civil Non Steel Library",
                                    isLibrary = true,
                                    RecordsAffected = CivilNonSteelLibraries.Select(x => new RecordAffected
                                    {
                                        RecordName = x.Model,
                                        SiteCode = null
                                    }).ToList()
                                });
                            }
                        }
                        else if (Logistical.tablePartName.PartName.ToLower() == Helpers.Constants.TablePartName.SideArm.ToString().ToLower())
                        {
                            List<TLIlogisticalitem> SideArmLibraryLogisticalItems = AffectedLogisticalItems
                                .Where(x => x.tablesNames.TableName.ToLower() == Helpers.Constants.TablesNames.TLIsideArmLibrary.ToString().ToLower()).ToList();

                            if (SideArmLibraryLogisticalItems.Count != 0)
                            {
                                List<TLIsideArmLibrary> SideArmLibraries = _unitOfWork.SideArmLibraryRepository
                                    .GetWhere(x => SideArmLibraryLogisticalItems.Select(x => x.RecordId).Contains(x.Id) && !x.Deleted).ToList();

                                ListOfResponse.Add(new TableAffected()
                                {
                                    TableName = "Side Arm Library",
                                    isLibrary = true,
                                    RecordsAffected = SideArmLibraries.Select(x => new RecordAffected
                                    {
                                        RecordName = x.Model,
                                        SiteCode = null
                                    }).ToList()
                                });
                            }
                        }
                        else if (Logistical.tablePartName.PartName.ToLower() == Helpers.Constants.TablePartName.MW.ToString().ToLower())
                        {
                            // MW_RFU..
                            List<TLIlogisticalitem> MW_RFULibraryLogisticalItems = AffectedLogisticalItems
                                .Where(x => x.tablesNames.TableName.ToLower() == Helpers.Constants.TablesNames.TLImwRFULibrary.ToString().ToLower()).ToList();

                            if (MW_RFULibraryLogisticalItems.Count != 0)
                            {
                                List<TLImwRFULibrary> MW_RFULibraries = _unitOfWork.MW_RFULibraryRepository
                                    .GetWhere(x => MW_RFULibraryLogisticalItems.Select(x => x.RecordId).Contains(x.Id) && !x.Deleted).ToList();

                                ListOfResponse.Add(new TableAffected()
                                {
                                    TableName = "MW_RFU Library",
                                    isLibrary = true,
                                    RecordsAffected = MW_RFULibraries.Select(x => new RecordAffected
                                    {
                                        RecordName = x.Model,
                                        SiteCode = null
                                    }).ToList()
                                });
                            }

                            // MW_Dish..
                            List<TLIlogisticalitem> MW_DishLibraryLogisticalItems = AffectedLogisticalItems
                                .Where(x => x.tablesNames.TableName.ToLower() == Helpers.Constants.TablesNames.TLImwDishLibrary.ToString().ToLower()).ToList();

                            if (MW_DishLibraryLogisticalItems.Count != 0)
                            {
                                List<TLImwDishLibrary> MW_DishLibraries = _unitOfWork.MW_DishLibraryRepository
                                    .GetWhere(x => MW_DishLibraryLogisticalItems.Select(x => x.RecordId).Contains(x.Id) && !x.Deleted).ToList();

                                ListOfResponse.Add(new TableAffected()
                                {
                                    TableName = "MW_Dish Library",
                                    isLibrary = true,
                                    RecordsAffected = MW_DishLibraries.Select(x => new RecordAffected
                                    {
                                        RecordName = x.Model,
                                        SiteCode = null
                                    }).ToList()
                                });
                            }

                            // MW_BU..
                            List<TLIlogisticalitem> MW_BULibraryLogisticalItems = AffectedLogisticalItems
                                .Where(x => x.tablesNames.TableName.ToLower() == Helpers.Constants.TablesNames.TLImwBULibrary.ToString().ToLower()).ToList();

                            if (MW_BULibraryLogisticalItems.Count != 0)
                            {
                                List<TLImwBULibrary> MW_BULibraries = _unitOfWork.MW_BULibraryRepository
                                    .GetWhere(x => MW_BULibraryLogisticalItems.Select(x => x.RecordId).Contains(x.Id) && !x.Deleted).ToList();

                                ListOfResponse.Add(new TableAffected()
                                {
                                    TableName = "MW_BU Library",
                                    isLibrary = true,
                                    RecordsAffected = MW_BULibraries.Select(x => new RecordAffected
                                    {
                                        RecordName = x.Model,
                                        SiteCode = null
                                    }).ToList()
                                });
                            }

                            // MW_ODU..
                            List<TLIlogisticalitem> MW_ODULibraryLogisticalItems = AffectedLogisticalItems
                                .Where(x => x.tablesNames.TableName.ToLower() == Helpers.Constants.TablesNames.TLImwODULibrary.ToString().ToLower()).ToList();

                            if (MW_ODULibraryLogisticalItems.Count != 0)
                            {
                                List<TLImwODULibrary> MW_ODULibraries = _unitOfWork.MW_ODULibraryRepository
                                    .GetWhere(x => MW_ODULibraryLogisticalItems.Select(x => x.RecordId).Contains(x.Id) && !x.Deleted).ToList();

                                ListOfResponse.Add(new TableAffected()
                                {
                                    TableName = "MW_ODU Library",
                                    isLibrary = true,
                                    RecordsAffected = MW_ODULibraries.Select(x => new RecordAffected
                                    {
                                        RecordName = x.Model,
                                        SiteCode = null
                                    }).ToList()
                                });
                            }

                            // MW_Other..
                            List<TLIlogisticalitem> MW_OtherLibraryLogisticalItems = AffectedLogisticalItems
                                .Where(x => x.tablesNames.TableName.ToLower() == Helpers.Constants.TablesNames.TLImwOtherLibrary.ToString().ToLower()).ToList();

                            if (MW_OtherLibraryLogisticalItems.Count != 0)
                            {
                                List<TLImwOtherLibrary> MW_OtherLibraries = _unitOfWork.MW_OtherLibraryRepository
                                    .GetWhere(x => MW_OtherLibraryLogisticalItems.Select(x => x.RecordId).Contains(x.Id) && !x.Deleted).ToList();

                                ListOfResponse.Add(new TableAffected()
                                {
                                    TableName = "MW_Other Library",
                                    isLibrary = true,
                                    RecordsAffected = MW_OtherLibraries.Select(x => new RecordAffected
                                    {
                                        RecordName = x.Model,
                                        SiteCode = null
                                    }).ToList()
                                });
                            }
                        }
                        else if (Logistical.tablePartName.PartName.ToLower() == Helpers.Constants.TablePartName.Radio.ToString().ToLower())
                        {
                            // RadioAntenna..
                            List<TLIlogisticalitem> RadioAntennaLibraryLogisticalItems = AffectedLogisticalItems
                                .Where(x => x.tablesNames.TableName.ToLower() == Helpers.Constants.TablesNames.TLIradioAntennaLibrary.ToString().ToLower()).ToList();

                            if (RadioAntennaLibraryLogisticalItems.Count != 0)
                            {
                                List<TLIradioAntennaLibrary> RadioAntennaLibraries = _unitOfWork.RadioAntennaLibraryRepository
                                    .GetWhere(x => RadioAntennaLibraryLogisticalItems.Select(x => x.RecordId).Contains(x.Id) && !x.Deleted).ToList();

                                ListOfResponse.Add(new TableAffected()
                                {
                                    TableName = "Radio Antenna Library",
                                    isLibrary = true,
                                    RecordsAffected = RadioAntennaLibraries.Select(x => new RecordAffected
                                    {
                                        RecordName = x.Model,
                                        SiteCode = null
                                    }).ToList()
                                });
                            }

                            // RadioRRU..
                            List<TLIlogisticalitem> RadioRRULibraryLogisticalItems = AffectedLogisticalItems
                                .Where(x => x.tablesNames.TableName.ToLower() == Helpers.Constants.TablesNames.TLIradioRRULibrary.ToString().ToLower()).ToList();

                            if (RadioRRULibraryLogisticalItems.Count != 0)
                            {
                                List<TLIradioRRULibrary> RadioRRULibraries = _unitOfWork.RadioRRULibraryRepository
                                    .GetWhere(x => RadioRRULibraryLogisticalItems.Select(x => x.RecordId).Contains(x.Id) && !x.Deleted).ToList();

                                ListOfResponse.Add(new TableAffected()
                                {
                                    TableName = "Radio RRU Library",
                                    isLibrary = true,
                                    RecordsAffected = RadioRRULibraries.Select(x => new RecordAffected
                                    {
                                        RecordName = x.Model,
                                        SiteCode = null
                                    }).ToList()
                                });
                            }

                            // RadioOther..
                            List<TLIlogisticalitem> RadioOtherLibraryLogisticalItems = AffectedLogisticalItems
                                .Where(x => x.tablesNames.TableName.ToLower() == Helpers.Constants.TablesNames.TLIradioOtherLibrary.ToString().ToLower()).ToList();

                            if (RadioOtherLibraryLogisticalItems.Count != 0)
                            {
                                List<TLIradioOtherLibrary> RadioOtherLibraries = _unitOfWork.RadioOtherLibraryRepository
                                    .GetWhere(x => RadioOtherLibraryLogisticalItems.Select(x => x.RecordId).Contains(x.Id) && !x.Deleted).ToList();

                                ListOfResponse.Add(new TableAffected()
                                {
                                    TableName = "Radio Other Library",
                                    isLibrary = true,
                                    RecordsAffected = RadioOtherLibraries.Select(x => new RecordAffected
                                    {
                                        RecordName = x.Model,
                                        SiteCode = null
                                    }).ToList()
                                });
                            }
                        }
                        else if (Logistical.tablePartName.PartName.ToLower() == Helpers.Constants.TablePartName.Power.ToString().ToLower())
                        {
                            // Power..
                            List<TLIlogisticalitem> PowerLibraryLogisticalItems = AffectedLogisticalItems
                                .Where(x => x.tablesNames.TableName.ToLower() == Helpers.Constants.TablesNames.TLIpowerLibrary.ToString().ToLower()).ToList();

                            if (PowerLibraryLogisticalItems.Count != 0)
                            {
                                List<TLIpowerLibrary> PowerLibraries = _unitOfWork.PowerLibraryRepository
                                    .GetWhere(x => PowerLibraryLogisticalItems.Select(x => x.RecordId).Contains(x.Id) && !x.Deleted).ToList();

                                ListOfResponse.Add(new TableAffected()
                                {
                                    TableName = "Power Load Library",
                                    isLibrary = true,
                                    RecordsAffected = PowerLibraries.Select(x => new RecordAffected
                                    {
                                        RecordName = x.Model,
                                        SiteCode = null
                                    }).ToList()
                                });
                            }

                        }
                        else if (Logistical.tablePartName.PartName.ToLower() == Helpers.Constants.TablePartName.LoadOther.ToString().ToLower())
                        {
                            // LoadOther..
                            List<TLIlogisticalitem> LoadOtherLibraryLogisticalItems = AffectedLogisticalItems
                                .Where(x => x.tablesNames.TableName.ToLower() == Helpers.Constants.TablesNames.TLIloadOtherLibrary.ToString().ToLower()).ToList();

                            if (LoadOtherLibraryLogisticalItems.Count != 0)
                            {
                                List<TLIloadOtherLibrary> LoadOtherLibraries = _unitOfWork.LoadOtherLibraryRepository
                                    .GetWhere(x => LoadOtherLibraryLogisticalItems.Select(x => x.RecordId).Contains(x.Id) && !x.Deleted).ToList();

                                ListOfResponse.Add(new TableAffected()
                                {
                                    TableName = "Load Other Library",
                                    isLibrary = true,
                                    RecordsAffected = LoadOtherLibraries.Select(x => new RecordAffected
                                    {
                                        RecordName = x.Model,
                                        SiteCode = null
                                    }).ToList()
                                });
                            }
                        }
                        else if (Logistical.tablePartName.PartName.ToLower() == Helpers.Constants.TablePartName.OtherInventory.ToString().ToLower())
                        {
                            // CabinetPower..
                            List<TLIlogisticalitem> CabinetPowerLibraryLogisticalItems = AffectedLogisticalItems
                                .Where(x => x.tablesNames.TableName.ToLower() == Helpers.Constants.TablesNames.TLIcabinetPowerLibrary.ToString().ToLower()).ToList();

                            if (CabinetPowerLibraryLogisticalItems.Count != 0)
                            {
                                List<TLIcabinetPowerLibrary> CabinetPowerLibraries = _unitOfWork.CabinetPowerLibraryRepository
                                    .GetWhere(x => CabinetPowerLibraryLogisticalItems.Select(x => x.RecordId).Contains(x.Id) && !x.Deleted).ToList();

                                ListOfResponse.Add(new TableAffected()
                                {
                                    TableName = "Cabinet Power Library",
                                    isLibrary = true,
                                    RecordsAffected = CabinetPowerLibraries.Select(x => new RecordAffected
                                    {
                                        RecordName = x.Model,
                                        SiteCode = null
                                    }).ToList()
                                });
                            }

                            // CabinetTelecom..
                            List<TLIlogisticalitem> CabinetTelecomLibraryLogisticalItems = AffectedLogisticalItems
                                .Where(x => x.tablesNames.TableName.ToLower() == Helpers.Constants.TablesNames.TLIcabinetTelecomLibrary.ToString().ToLower()).ToList();

                            if (CabinetTelecomLibraryLogisticalItems.Count != 0)
                            {
                                List<TLIcabinetTelecomLibrary> CabinetTelecomLibraries = _unitOfWork.CabinetTelecomLibraryRepository
                                    .GetWhere(x => CabinetTelecomLibraryLogisticalItems.Select(x => x.RecordId).Contains(x.Id) && !x.Deleted).ToList();

                                ListOfResponse.Add(new TableAffected()
                                {
                                    TableName = "Cabinet Telecom Library",
                                    isLibrary = true,
                                    RecordsAffected = CabinetTelecomLibraries.Select(x => new RecordAffected
                                    {
                                        RecordName = x.Model,
                                        SiteCode = null
                                    }).ToList()
                                });
                            }

                            // Solar..
                            List<TLIlogisticalitem> SolarLibraryLogisticalItems = AffectedLogisticalItems
                                .Where(x => x.tablesNames.TableName.ToLower() == Helpers.Constants.TablesNames.TLIsolarLibrary.ToString().ToLower()).ToList();

                            if (SolarLibraryLogisticalItems.Count != 0)
                            {
                                List<TLIsolarLibrary> SolarLibraries = _unitOfWork.SolarLibraryRepository
                                    .GetWhere(x => SolarLibraryLogisticalItems.Select(x => x.RecordId).Contains(x.Id) && !x.Deleted).ToList();

                                ListOfResponse.Add(new TableAffected()
                                {
                                    TableName = "Solar Library",
                                    isLibrary = true,
                                    RecordsAffected = SolarLibraries.Select(x => new RecordAffected
                                    {
                                        RecordName = x.Model,
                                        SiteCode = null
                                    }).ToList()
                                });
                            }

                            // Generator..
                            List<TLIlogisticalitem> GeneratorLibraryLogisticalItems = AffectedLogisticalItems
                                .Where(x => x.tablesNames.TableName.ToLower() == Helpers.Constants.TablesNames.TLIgeneratorLibrary.ToString().ToLower()).ToList();

                            if (GeneratorLibraryLogisticalItems.Count != 0)
                            {
                                List<TLIgeneratorLibrary> GeneratorLibraries = _unitOfWork.GeneratorLibraryRepository
                                    .GetWhere(x => GeneratorLibraryLogisticalItems.Select(x => x.RecordId).Contains(x.Id) && !x.Deleted).ToList();

                                ListOfResponse.Add(new TableAffected()
                                {
                                    TableName = "Generator Library",
                                    isLibrary = true,
                                    RecordsAffected = GeneratorLibraries.Select(x => new RecordAffected
                                    {
                                        RecordName = x.Model,
                                        SiteCode = null
                                    }).ToList()
                                });
                            }
                        }

                        if (ListOfResponse.Count != 0)
                            return new Response<List<TableAffected>>(true, ListOfResponse, null, null, (int)Helpers.Constants.ApiReturnCode.success);
                        else
                        {
                            Logistical.Deleted = (true);
                            _unitOfWork.SaveChanges();
                        }
                    }
                    else
                    {
                        Logistical.Deleted = (true);
                        _unitOfWork.SaveChanges();
                    }
                }
                else
                {
                    return new Response<List<TableAffected>>(true, null, null, $"The LogisticalId Is Not Found", (int)Helpers.Constants.ApiReturnCode.fail);
                }
                return new Response<List<TableAffected>>(true, null, null, null, (int)Helpers.Constants.ApiReturnCode.success);
            }
            catch (Exception err)
            {

                return new Response<List<TableAffected>>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
        }
        public Response<List<TableAffected>> DisableLogistical(int LogisticalId)
        {
            try
            {
                TLIlogistical Logistical = _unitOfWork.LogistcalRepository
               .GetIncludeWhereFirst(x => x.Id == LogisticalId, x => x.tablePartName);
                if (Logistical !=null)
                {
                    List<TLIlogisticalitem> AffectedLogisticalItems = _unitOfWork.LogisticalitemRepository
                        .GetIncludeWhere(x => x.logisticalId == LogisticalId, x => x.tablesNames).ToList();

                    if (AffectedLogisticalItems.Count != 0)
                    {
                        List<TableAffected> ListOfResponse = new List<TableAffected>();

                        if (Logistical.tablePartName.PartName.ToLower() == Helpers.Constants.TablePartName.CivilSupport.ToString().ToLower())
                        {
                            // CivilWithLeg..
                            List<TLIlogisticalitem> CivilWithLegLibraryLogisticalItems = AffectedLogisticalItems
                                .Where(x => x.tablesNames.TableName.ToLower() == Helpers.Constants.TablesNames.TLIcivilWithLegLibrary.ToString().ToLower()).ToList();

                            if (CivilWithLegLibraryLogisticalItems.Count != 0)
                            {
                                List<TLIcivilWithLegLibrary> CivilWithLegLibraries = _unitOfWork.CivilWithLegLibraryRepository
                                    .GetWhere(x => CivilWithLegLibraryLogisticalItems.Select(x => x.RecordId).Contains(x.Id) && !x.Deleted).ToList();

                                ListOfResponse.Add(new TableAffected()
                                {
                                    TableName = "Civil Steel Support With Legs Library",
                                    isLibrary = true,
                                    RecordsAffected = CivilWithLegLibraries.Select(x => new RecordAffected
                                    {
                                        RecordName = x.Model,
                                        SiteCode = null
                                    }).ToList()
                                });
                            }

                            // CivilWithoutLeg..
                            List<TLIlogisticalitem> CivilWithoutLegLibraryLogisticalItems = AffectedLogisticalItems
                                .Where(x => x.tablesNames.TableName.ToLower() == Helpers.Constants.TablesNames.TLIcivilWithoutLegLibrary.ToString().ToLower()).ToList();

                            if (CivilWithoutLegLibraryLogisticalItems.Count != 0)
                            {
                                List<TLIcivilWithoutLegLibrary> CivilWithoutLegLibraries = _unitOfWork.CivilWithoutLegLibraryRepository
                                    .GetWhere(x => CivilWithoutLegLibraryLogisticalItems.Select(x => x.RecordId).Contains(x.Id) && !x.Deleted).ToList();

                                ListOfResponse.Add(new TableAffected()
                                {
                                    TableName = "Civil Steel Support Without Legs Library",
                                    isLibrary = true,
                                    RecordsAffected = CivilWithoutLegLibraries.Select(x => new RecordAffected
                                    {
                                        RecordName = x.Model,
                                        SiteCode = null
                                    }).ToList()
                                });
                            }

                            // CivilNonSteel..
                            List<TLIlogisticalitem> CivilNonSteelLibraryLogisticalItems = AffectedLogisticalItems
                                .Where(x => x.tablesNames.TableName.ToLower() == Helpers.Constants.TablesNames.TLIcivilNonSteelLibrary.ToString().ToLower()).ToList();

                            if (CivilNonSteelLibraryLogisticalItems.Count != 0)
                            {
                                List<TLIcivilNonSteelLibrary> CivilNonSteelLibraries = _unitOfWork.CivilNonSteelLibraryRepository
                                    .GetWhere(x => CivilNonSteelLibraryLogisticalItems.Select(x => x.RecordId).Contains(x.Id) && !x.Deleted).ToList();

                                ListOfResponse.Add(new TableAffected()
                                {
                                    TableName = "Civil Non Steel Library",
                                    isLibrary = true,
                                    RecordsAffected = CivilNonSteelLibraries.Select(x => new RecordAffected
                                    {
                                        RecordName = x.Model,
                                        SiteCode = null
                                    }).ToList()
                                });
                            }
                        }
                        else if (Logistical.tablePartName.PartName.ToLower() == Helpers.Constants.TablePartName.SideArm.ToString().ToLower())
                        {
                            List<TLIlogisticalitem> SideArmLibraryLogisticalItems = AffectedLogisticalItems
                                .Where(x => x.tablesNames.TableName.ToLower() == Helpers.Constants.TablesNames.TLIsideArmLibrary.ToString().ToLower()).ToList();

                            if (SideArmLibraryLogisticalItems.Count != 0)
                            {
                                List<TLIsideArmLibrary> SideArmLibraries = _unitOfWork.SideArmLibraryRepository
                                    .GetWhere(x => SideArmLibraryLogisticalItems.Select(x => x.RecordId).Contains(x.Id) && !x.Deleted).ToList();

                                ListOfResponse.Add(new TableAffected()
                                {
                                    TableName = "Side Arm Library",
                                    isLibrary = true,
                                    RecordsAffected = SideArmLibraries.Select(x => new RecordAffected
                                    {
                                        RecordName = x.Model,
                                        SiteCode = null
                                    }).ToList()
                                });
                            }
                        }
                        else if (Logistical.tablePartName.PartName.ToLower() == Helpers.Constants.TablePartName.MW.ToString().ToLower())
                        {
                            // MW_RFU..
                            List<TLIlogisticalitem> MW_RFULibraryLogisticalItems = AffectedLogisticalItems
                                .Where(x => x.tablesNames.TableName.ToLower() == Helpers.Constants.TablesNames.TLImwRFULibrary.ToString().ToLower()).ToList();

                            if (MW_RFULibraryLogisticalItems.Count != 0)
                            {
                                List<TLImwRFULibrary> MW_RFULibraries = _unitOfWork.MW_RFULibraryRepository
                                    .GetWhere(x => MW_RFULibraryLogisticalItems.Select(x => x.RecordId).Contains(x.Id) && !x.Deleted).ToList();

                                ListOfResponse.Add(new TableAffected()
                                {
                                    TableName = "MW_RFU Library",
                                    isLibrary = true,
                                    RecordsAffected = MW_RFULibraries.Select(x => new RecordAffected
                                    {
                                        RecordName = x.Model,
                                        SiteCode = null
                                    }).ToList()
                                });
                            }

                            // MW_Dish..
                            List<TLIlogisticalitem> MW_DishLibraryLogisticalItems = AffectedLogisticalItems
                                .Where(x => x.tablesNames.TableName.ToLower() == Helpers.Constants.TablesNames.TLImwDishLibrary.ToString().ToLower()).ToList();

                            if (MW_DishLibraryLogisticalItems.Count != 0)
                            {
                                List<TLImwDishLibrary> MW_DishLibraries = _unitOfWork.MW_DishLibraryRepository
                                    .GetWhere(x => MW_DishLibraryLogisticalItems.Select(x => x.RecordId).Contains(x.Id) && !x.Deleted).ToList();

                                ListOfResponse.Add(new TableAffected()
                                {
                                    TableName = "MW_Dish Library",
                                    isLibrary = true,
                                    RecordsAffected = MW_DishLibraries.Select(x => new RecordAffected
                                    {
                                        RecordName = x.Model,
                                        SiteCode = null
                                    }).ToList()
                                });
                            }

                            // MW_BU..
                            List<TLIlogisticalitem> MW_BULibraryLogisticalItems = AffectedLogisticalItems
                                .Where(x => x.tablesNames.TableName.ToLower() == Helpers.Constants.TablesNames.TLImwBULibrary.ToString().ToLower()).ToList();

                            if (MW_BULibraryLogisticalItems.Count != 0)
                            {
                                List<TLImwBULibrary> MW_BULibraries = _unitOfWork.MW_BULibraryRepository
                                    .GetWhere(x => MW_BULibraryLogisticalItems.Select(x => x.RecordId).Contains(x.Id) && !x.Deleted).ToList();

                                ListOfResponse.Add(new TableAffected()
                                {
                                    TableName = "MW_BU Library",
                                    isLibrary = true,
                                    RecordsAffected = MW_BULibraries.Select(x => new RecordAffected
                                    {
                                        RecordName = x.Model,
                                        SiteCode = null
                                    }).ToList()
                                });
                            }

                            // MW_ODU..
                            List<TLIlogisticalitem> MW_ODULibraryLogisticalItems = AffectedLogisticalItems
                                .Where(x => x.tablesNames.TableName.ToLower() == Helpers.Constants.TablesNames.TLImwODULibrary.ToString().ToLower()).ToList();

                            if (MW_ODULibraryLogisticalItems.Count != 0)
                            {
                                List<TLImwODULibrary> MW_ODULibraries = _unitOfWork.MW_ODULibraryRepository
                                    .GetWhere(x => MW_ODULibraryLogisticalItems.Select(x => x.RecordId).Contains(x.Id) && !x.Deleted).ToList();

                                ListOfResponse.Add(new TableAffected()
                                {
                                    TableName = "MW_ODU Library",
                                    isLibrary = true,
                                    RecordsAffected = MW_ODULibraries.Select(x => new RecordAffected
                                    {
                                        RecordName = x.Model,
                                        SiteCode = null
                                    }).ToList()
                                });
                            }

                            // MW_Other..
                            List<TLIlogisticalitem> MW_OtherLibraryLogisticalItems = AffectedLogisticalItems
                                .Where(x => x.tablesNames.TableName.ToLower() == Helpers.Constants.TablesNames.TLImwOtherLibrary.ToString().ToLower()).ToList();

                            if (MW_OtherLibraryLogisticalItems.Count != 0)
                            {
                                List<TLImwOtherLibrary> MW_OtherLibraries = _unitOfWork.MW_OtherLibraryRepository
                                    .GetWhere(x => MW_OtherLibraryLogisticalItems.Select(x => x.RecordId).Contains(x.Id) && !x.Deleted).ToList();

                                ListOfResponse.Add(new TableAffected()
                                {
                                    TableName = "MW_Other Library",
                                    isLibrary = true,
                                    RecordsAffected = MW_OtherLibraries.Select(x => new RecordAffected
                                    {
                                        RecordName = x.Model,
                                        SiteCode = null
                                    }).ToList()
                                });
                            }
                        }
                        else if (Logistical.tablePartName.PartName.ToLower() == Helpers.Constants.TablePartName.Radio.ToString().ToLower())
                        {
                            // RadioAntenna..
                            List<TLIlogisticalitem> RadioAntennaLibraryLogisticalItems = AffectedLogisticalItems
                                .Where(x => x.tablesNames.TableName.ToLower() == Helpers.Constants.TablesNames.TLIradioAntennaLibrary.ToString().ToLower()).ToList();

                            if (RadioAntennaLibraryLogisticalItems.Count != 0)
                            {
                                List<TLIradioAntennaLibrary> RadioAntennaLibraries = _unitOfWork.RadioAntennaLibraryRepository
                                    .GetWhere(x => RadioAntennaLibraryLogisticalItems.Select(x => x.RecordId).Contains(x.Id) && !x.Deleted).ToList();

                                ListOfResponse.Add(new TableAffected()
                                {
                                    TableName = "Radio Antenna Library",
                                    isLibrary = true,
                                    RecordsAffected = RadioAntennaLibraries.Select(x => new RecordAffected
                                    {
                                        RecordName = x.Model,
                                        SiteCode = null
                                    }).ToList()
                                });
                            }

                            // RadioRRU..
                            List<TLIlogisticalitem> RadioRRULibraryLogisticalItems = AffectedLogisticalItems
                                .Where(x => x.tablesNames.TableName.ToLower() == Helpers.Constants.TablesNames.TLIradioRRULibrary.ToString().ToLower()).ToList();

                            if (RadioRRULibraryLogisticalItems.Count != 0)
                            {
                                List<TLIradioRRULibrary> RadioRRULibraries = _unitOfWork.RadioRRULibraryRepository
                                    .GetWhere(x => RadioRRULibraryLogisticalItems.Select(x => x.RecordId).Contains(x.Id) && !x.Deleted).ToList();

                                ListOfResponse.Add(new TableAffected()
                                {
                                    TableName = "Radio RRU Library",
                                    isLibrary = true,
                                    RecordsAffected = RadioRRULibraries.Select(x => new RecordAffected
                                    {
                                        RecordName = x.Model,
                                        SiteCode = null
                                    }).ToList()
                                });
                            }

                            // RadioOther..
                            List<TLIlogisticalitem> RadioOtherLibraryLogisticalItems = AffectedLogisticalItems
                                .Where(x => x.tablesNames.TableName.ToLower() == Helpers.Constants.TablesNames.TLIradioOtherLibrary.ToString().ToLower()).ToList();

                            if (RadioOtherLibraryLogisticalItems.Count != 0)
                            {
                                List<TLIradioOtherLibrary> RadioOtherLibraries = _unitOfWork.RadioOtherLibraryRepository
                                    .GetWhere(x => RadioOtherLibraryLogisticalItems.Select(x => x.RecordId).Contains(x.Id) && !x.Deleted).ToList();

                                ListOfResponse.Add(new TableAffected()
                                {
                                    TableName = "Radio Other Library",
                                    isLibrary = true,
                                    RecordsAffected = RadioOtherLibraries.Select(x => new RecordAffected
                                    {
                                        RecordName = x.Model,
                                        SiteCode = null
                                    }).ToList()
                                });
                            }
                        }
                        else if (Logistical.tablePartName.PartName.ToLower() == Helpers.Constants.TablePartName.Power.ToString().ToLower())
                        {
                            // Power..
                            List<TLIlogisticalitem> PowerLibraryLogisticalItems = AffectedLogisticalItems
                                .Where(x => x.tablesNames.TableName.ToLower() == Helpers.Constants.TablesNames.TLIpowerLibrary.ToString().ToLower()).ToList();

                            if (PowerLibraryLogisticalItems.Count != 0)
                            {
                                List<TLIpowerLibrary> PowerLibraries = _unitOfWork.PowerLibraryRepository
                                    .GetWhere(x => PowerLibraryLogisticalItems.Select(x => x.RecordId).Contains(x.Id) && !x.Deleted).ToList();

                                ListOfResponse.Add(new TableAffected()
                                {
                                    TableName = "Power Load Library",
                                    isLibrary = true,
                                    RecordsAffected = PowerLibraries.Select(x => new RecordAffected
                                    {
                                        RecordName = x.Model,
                                        SiteCode = null
                                    }).ToList()
                                });
                            }

                        }
                        else if (Logistical.tablePartName.PartName.ToLower() == Helpers.Constants.TablePartName.LoadOther.ToString().ToLower())
                        {
                            // LoadOther..
                            List<TLIlogisticalitem> LoadOtherLibraryLogisticalItems = AffectedLogisticalItems
                                .Where(x => x.tablesNames.TableName.ToLower() == Helpers.Constants.TablesNames.TLIloadOtherLibrary.ToString().ToLower()).ToList();

                            if (LoadOtherLibraryLogisticalItems.Count != 0)
                            {
                                List<TLIloadOtherLibrary> LoadOtherLibraries = _unitOfWork.LoadOtherLibraryRepository
                                    .GetWhere(x => LoadOtherLibraryLogisticalItems.Select(x => x.RecordId).Contains(x.Id) && !x.Deleted).ToList();

                                ListOfResponse.Add(new TableAffected()
                                {
                                    TableName = "Load Other Library",
                                    isLibrary = true,
                                    RecordsAffected = LoadOtherLibraries.Select(x => new RecordAffected
                                    {
                                        RecordName = x.Model,
                                        SiteCode = null
                                    }).ToList()
                                });
                            }
                        }
                        else if (Logistical.tablePartName.PartName.ToLower() == Helpers.Constants.TablePartName.OtherInventory.ToString().ToLower())
                        {
                            // CabinetPower..
                            List<TLIlogisticalitem> CabinetPowerLibraryLogisticalItems = AffectedLogisticalItems
                                .Where(x => x.tablesNames.TableName.ToLower() == Helpers.Constants.TablesNames.TLIcabinetPowerLibrary.ToString().ToLower()).ToList();

                            if (CabinetPowerLibraryLogisticalItems.Count != 0)
                            {
                                List<TLIcabinetPowerLibrary> CabinetPowerLibraries = _unitOfWork.CabinetPowerLibraryRepository
                                    .GetWhere(x => CabinetPowerLibraryLogisticalItems.Select(x => x.RecordId).Contains(x.Id) && !x.Deleted).ToList();

                                ListOfResponse.Add(new TableAffected()
                                {
                                    TableName = "Cabinet Power Library",
                                    isLibrary = true,
                                    RecordsAffected = CabinetPowerLibraries.Select(x => new RecordAffected
                                    {
                                        RecordName = x.Model,
                                        SiteCode = null
                                    }).ToList()
                                });
                            }

                            // CabinetTelecom..
                            List<TLIlogisticalitem> CabinetTelecomLibraryLogisticalItems = AffectedLogisticalItems
                                .Where(x => x.tablesNames.TableName.ToLower() == Helpers.Constants.TablesNames.TLIcabinetTelecomLibrary.ToString().ToLower()).ToList();

                            if (CabinetTelecomLibraryLogisticalItems.Count != 0)
                            {
                                List<TLIcabinetTelecomLibrary> CabinetTelecomLibraries = _unitOfWork.CabinetTelecomLibraryRepository
                                    .GetWhere(x => CabinetTelecomLibraryLogisticalItems.Select(x => x.RecordId).Contains(x.Id) && !x.Deleted).ToList();

                                ListOfResponse.Add(new TableAffected()
                                {
                                    TableName = "Cabinet Telecom Library",
                                    isLibrary = true,
                                    RecordsAffected = CabinetTelecomLibraries.Select(x => new RecordAffected
                                    {
                                        RecordName = x.Model,
                                        SiteCode = null
                                    }).ToList()
                                });
                            }

                            // Solar..
                            List<TLIlogisticalitem> SolarLibraryLogisticalItems = AffectedLogisticalItems
                                .Where(x => x.tablesNames.TableName.ToLower() == Helpers.Constants.TablesNames.TLIsolarLibrary.ToString().ToLower()).ToList();

                            if (SolarLibraryLogisticalItems.Count != 0)
                            {
                                List<TLIsolarLibrary> SolarLibraries = _unitOfWork.SolarLibraryRepository
                                    .GetWhere(x => SolarLibraryLogisticalItems.Select(x => x.RecordId).Contains(x.Id) && !x.Deleted).ToList();

                                ListOfResponse.Add(new TableAffected()
                                {
                                    TableName = "Solar Library",
                                    isLibrary = true,
                                    RecordsAffected = SolarLibraries.Select(x => new RecordAffected
                                    {
                                        RecordName = x.Model,
                                        SiteCode = null
                                    }).ToList()
                                });
                            }

                            // Generator..
                            List<TLIlogisticalitem> GeneratorLibraryLogisticalItems = AffectedLogisticalItems
                                .Where(x => x.tablesNames.TableName.ToLower() == Helpers.Constants.TablesNames.TLIgeneratorLibrary.ToString().ToLower()).ToList();

                            if (GeneratorLibraryLogisticalItems.Count != 0)
                            {
                                List<TLIgeneratorLibrary> GeneratorLibraries = _unitOfWork.GeneratorLibraryRepository
                                    .GetWhere(x => GeneratorLibraryLogisticalItems.Select(x => x.RecordId).Contains(x.Id) && !x.Deleted).ToList();

                                ListOfResponse.Add(new TableAffected()
                                {
                                    TableName = "Generator Library",
                                    isLibrary = true,
                                    RecordsAffected = GeneratorLibraries.Select(x => new RecordAffected
                                    {
                                        RecordName = x.Model,
                                        SiteCode = null
                                    }).ToList()
                                });
                            }
                        }

                        if (ListOfResponse.Count != 0)
                            return new Response<List<TableAffected>>(true, ListOfResponse, null, null, (int)Helpers.Constants.ApiReturnCode.success);
                        else
                        {
                            Logistical.Active = !Logistical.Active;
                            _unitOfWork.SaveChanges();
                        }
                    }
                    else
                    {
                        Logistical.Active = !Logistical.Active;
                        _unitOfWork.SaveChanges();
                    }
                }
                else
                {
                    return new Response<List<TableAffected>>(true, null, null, $"The LogisticalId Is Not Found", (int)Helpers.Constants.ApiReturnCode.fail);
                }
                return new Response<List<TableAffected>>(true, null, null, null, (int)Helpers.Constants.ApiReturnCode.success);
            }
            catch (Exception err)
            {

                return new Response<List<TableAffected>>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
           
        }
        public Response<bool> EditLogistical(EditLogisticalViewModel EditLogisticalViewModel)
        {
            TLIlogistical Logistical = _unitOfWork.LogistcalRepository.GetByID(EditLogisticalViewModel.Id);
            Logistical.Name = EditLogisticalViewModel.Name;
            Logistical.Description = EditLogisticalViewModel.Description;

            TLIlogistical CheckLogistical = _unitOfWork.LogistcalRepository
                .GetWhereFirst(x => x.Name.ToLower() == EditLogisticalViewModel.Name.ToLower() && !x.Deleted &&
                    x.tablePartNameId == Logistical.tablePartNameId && x.logisticalTypeId == Logistical.logisticalTypeId &&
                    x.Id != Logistical.Id);
           
            if (CheckLogistical != null)
                return new Response<bool>(false, false, null, "This logistical is already exist", (int)Helpers.Constants.ApiReturnCode.fail);

            _unitOfWork.SaveChanges();
            return new Response<bool>(true, true, null, null, (int)Helpers.Constants.ApiReturnCode.success);
        }
        public Response<List<LogisticalViewModel>> GetLogisticalTypes()
        {
            List<LogisticalViewModel> LogisticalTypes = _mapper.Map<List<LogisticalViewModel>>(_unitOfWork.logisticalTypeRepository
                .GetWhere(x => !x.Disable && !x.Deleted).ToList());

            return new Response<List<LogisticalViewModel>>(true, LogisticalTypes, null, null, (int)Helpers.Constants.ApiReturnCode.success);
        }
    }
}
