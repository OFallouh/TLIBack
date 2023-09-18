using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Engineering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        public LogisticalService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
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
        public Response<bool> DeleteLogistical(int LogisticalId)
        {
            TLIlogistical Logistical = _unitOfWork.LogistcalRepository.GetByID(LogisticalId);
            Logistical.Deleted = !Logistical.Deleted;
            _unitOfWork.SaveChanges();

            return new Response<bool>(true, true, null, null, (int)Helpers.Constants.ApiReturnCode.success);
        }
        public Response<bool> DisableLogistical(int LogisticalId)
        {
            TLIlogistical Logistical = _unitOfWork.LogistcalRepository.GetByID(LogisticalId);
            Logistical.Active = !Logistical.Active;
            _unitOfWork.SaveChanges();

            return new Response<bool>(true, true, null, null, (int)Helpers.Constants.ApiReturnCode.success);
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
                return new Response<bool>(true, false, null, "This logistical is already exist", (int)Helpers.Constants.ApiReturnCode.fail);

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
