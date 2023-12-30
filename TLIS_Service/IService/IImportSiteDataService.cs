using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using TLIS_DAL.Helpers;
using TLIS_DAL.ViewModels.ComplixFilter;
using TLIS_DAL.ViewModels.ImportSheetDtos;

namespace TLIS_Service.IService
{
    public interface IImportSiteDataService
    {
        Response<string> ImportSiteData(IFormFile File);
        Response<string> ImportFileData(IFormFile File);
        Response<string> ImportLibraryFileData(IFormFile File);
        //Response<string> ImportInstallationFileData(IFormFile File);
        Response<string> ImportLibraryFileData2(IFormFile File, string ConnectionString);
        Response<string> ImportInstallationFileData2(IFormFile File, string ConnectionString);
        Response<List<ImportSheetViewModel>> GetAllWarningData(ClxFilter f);
        Response<string> ExportErrorDataTable(string ErrorType, string FileDirectory);
        Response<string> ImportInstallationFileDataTower(IFormFile File, string ConnectionString);
    }
}
