using Microsoft.AspNetCore.Http;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Text;
using TLIS_DAL.Helper;
using TLIS_DAL.Helpers;
using TLIS_DAL.ViewModels.AttachedFilesDTOs;

namespace TLIS_Service.IService
{
    public interface IFileManagmentService
    {
        Response<string> GenerateExcelTemplacteByTableName(string TableName, string fileDirectory, int? CategoryId);
        Response<List<KeyValuePair<int, string>>> ImportFile(IFormFile file, string TableName, int? CategoryId, string ConnectionString, int UserId);
        Response<string> AttachFile(int UserId, IFormFile file , string SiteCode, string RecordId, string TableName, string connection, string AttachFolder, string asset, bool ExternalSys);
        Response<string> DeleteFile(string FileName, int RecordId, string TableName, string SiteCode,int UserId,bool ExternalSys);
        Response<IEnumerable<AttachedFilesViewModel>> GetFilesByRecordIdAndTableName(int RecordId, string TableName, string SiteCode, int UserId, bool ExternalSys);
        Response<List<AttachedFilesViewModel>> GetAttachecdFiles(int RecordId, string TableName);
        Response<AttachedFilesViewModel> AttachedUnAttached(int Id);
        Response<string> GetAttachedToDownload(string filename, int recordid, string tablename);
        Response<List<AttachedFilesViewModel>> GetAttachecdFilesBySite(string SiteCode, int UseId, bool ExternalSys);
       
    }
}
