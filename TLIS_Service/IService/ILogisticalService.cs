﻿using System;
using System.Collections.Generic;
using System.Text;
using TLIS_DAL.Helper;
using TLIS_DAL.Helpers;
using TLIS_DAL.ViewModelBase;
using TLIS_DAL.ViewModels.LogisticalDTOs;

namespace TLIS_Service.IService
{
    public interface ILogisticalService
    {
        Response<MainLogisticalViewModel> GetById(int LogisticalId);
        Response<List<MainLogisticalViewModel>> GetLogisticalByTypeOrPart(string TablePartName, string LogisticalType, string Search, ParameterPagination parameterPagination);
        Response<bool> AddLogistical(AddNewLogistical NewLogistical);
        Response<List<TableAffected>> DeleteLogistical(int LogisticalId);
        Response<List<TableAffected>> DisableLogistical(int LogisticalId);
        Response<bool> EditLogistical(EditLogisticalViewModel EditLogisticalViewModel);
        Response<List<LogisticalViewModel>> GetLogisticalTypes();
    }
}
