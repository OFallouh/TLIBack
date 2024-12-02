using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OfficeOpenXml;
using OfficeOpenXml.DataValidation;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using TLIS_DAL;
using TLIS_DAL.Helper;
using TLIS_DAL.Helpers;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModelBase;
using TLIS_DAL.ViewModels.AttachedFilesDTOs;
using TLIS_DAL.ViewModels.TablesHistoryDTOs;
using TLIS_Repository.Base;
using TLIS_Service.IService;
using Microsoft.Extensions.Configuration;
using System.Transactions;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Engineering;
using System.ComponentModel.Design;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using System.Xml.Linq;
using Org.BouncyCastle.Utilities.Zlib;
using Microsoft.Extensions.Primitives;
using TLIS_DAL.ViewModels.LogisticalitemDTOs;
using Org.BouncyCastle.Bcpg;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using TLIS_DAL.ViewModels.CivilWithLegLibraryDTOs;
using System.Reflection.Metadata;
using DocumentFormat.OpenXml.Spreadsheet;
using System.Numerics;
using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.InkML;


namespace TLIS_Service.Services
{

    public class FileManagmentService : IFileManagmentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _dbContext;
        IServiceCollection _services;
        ServiceProvider _serviceProvider;
        private IMapper _mapper;
        public FileManagmentService(IUnitOfWork unitOfWork, IServiceCollection services, ApplicationDbContext context, IConfiguration configuration, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _services = services;
            _serviceProvider = _services.BuildServiceProvider();
            _configuration = configuration;
            _mapper = mapper;
            _dbContext = context;
        }
        //Function take 3 parameters 
        //First TableName to specify the table i deal with
        //Second fileDirectory have the current directory i deal with
        //Third CategoryId specify the category if i deal with civil without leg
        public Response<string> GenerateExcelTemplacteByTableName(string TableName, string fileDirectory, int? CategoryId)
        {
            try
            {
                string fileName = TableName;

                // Check if fileDirectory exists; if not, create it
                if (!Directory.Exists(fileDirectory))
                {
                    Directory.CreateDirectory(fileDirectory);
                }

                // Create new file path
                var filePath = Path.Combine(fileDirectory, fileName + ".xlsx");

                // If file exists, delete it
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }

                FileInfo file = new FileInfo(filePath);

                // Get TableName record from TLItablesNames
                TLItablesNames TableNameEntity = _unitOfWork.TablesNamesRepository.GetWhereFirst(x => x.TableName == TableName);

                // Get activated attributes based on TableName
                List<string> TableNameAtts = _unitOfWork.AttributeActivatedRepository
                    .GetAttributeActivated(TableName, null, CategoryId)
                    .AsQueryable()
                    .Where(x => x.enable == true)
                    .Select(x => x.Key)
                    .ToList();

                // Remove unwanted attributes
                foreach (var e in TableNameAtts.ToList())
                {
                    if (e.ToLower().Contains("_name"))
                    {
                        TableNameAtts.Remove(e);
                    }

                    if (TableName == "TLIcivilWithLegLibrary" && e.ToLower() == "NumberOfLegs".ToLower())
                    {
                        TableNameAtts.Remove(e);
                    }
                    if ((TableName != "TLIcivilWithLegLibrary" || TableName != "TLIcivilWithoutLegLibrary" || TableName != "TLIcivilNonSteelLibrary"
                        || TableName != "TLIcabinetPowerLibrary" || TableName != "TLIcabinetTelecomLibrary") &&
                            e.ToLower() == "spacelibrary".ToLower())
                    {
                        TableNameAtts.Remove(e);
                    }
                }
                TableNameAtts.Remove("Id");

                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                using (ExcelPackage package = new ExcelPackage())
                {
                    var worksheet = package.Workbook.Worksheets.Add(TableName);

                    // Specify the starting column in Excel
                    int j = 1;

                    // Get related tables based on table name
                    var RelatedTables = GetTableNameRelatedTables(TableName);
                    List<string> RelatedTable = new List<string>();
                    string RelatedTab = "";
                    foreach (var item in RelatedTables)
                    {
                        RelatedTab = item.Key;
                        RelatedTable.Add(RelatedTab);
                    }
                    var Table = TableNameAtts.Union(RelatedTable).ToList();

                    // Loop through each property in the main table
                    foreach (var TableNameAtt in Table)
                    {
                        worksheet.Cells[1, j].Value = TableNameAtt;
                        var attributeType = _unitOfWork.AttributeActivatedRepository
                           .GetAttributeActivated(TableName, null, CategoryId)
                           .AsQueryable()
                           .FirstOrDefault(x => x.Key == TableNameAtt)?.DataType;

                        // إذا كان النوع bool، أضف قائمة منسدلة
                        if (attributeType?.ToLower() == "bool")
                        {
                            var validation = worksheet.DataValidations.AddListValidation(worksheet.Cells[2, j, 10000, j].Address);
                            validation.ShowErrorMessage = true;
                            validation.ErrorStyle = ExcelDataValidationWarningStyle.warning;
                            validation.ErrorTitle = "Invalid Selection";
                            validation.Error = "Please select either True or False.";
                            validation.Formula.Values.Add("True");
                            validation.Formula.Values.Add("False");
                        }

                        // Handle dropdown for foreign keys or specific fields
                        if (TableNameAtt.Contains("Id") || TableNameAtt.Contains("Suppliers") ||
                            TableNameAtt.Contains("Designers") || TableNameAtt.Contains("Manufacturers") ||
                            TableNameAtt.Contains("Vendors") || TableNameAtt.Contains("RFUType") ||
                            TableNameAtt.Contains("IntegratedWith") || TableNameAtt.Contains("Contractors") ||
                            TableNameAtt.Contains("Conultants"))
                        {
                            if (RelatedTables.Find(x => x.Key == TableNameAtt).Value != null ?
                                RelatedTables.Find(x => x.Key == TableNameAtt).Value.Count > 0 : false)
                            {
                                var records = RelatedTables.Find(x => x.Key == TableNameAtt);
                                if (records.Value != null)
                                {
                                    var validation = worksheet.DataValidations.AddListValidation(worksheet.Cells[2, j, 10000, j].Address);
                                    validation.ShowErrorMessage = true;
                                    validation.ErrorStyle = ExcelDataValidationWarningStyle.warning;
                                    validation.ErrorTitle = "An invalid value was entered";
                                    validation.Error = "Select a value from the list";
                                    foreach (var value in records.Value)
                                    {
                                        validation.Formula.Values.Add(value.Value);
                                    }
                                }
                            }
                        }
                        // Add dropdown for boolean fields
                        else if (TableNameAtt.ToLower().Contains("bool"))
                        {
                            var validation = worksheet.DataValidations.AddListValidation(worksheet.Cells[2, j, 10000, j].Address);
                            validation.ShowErrorMessage = true;
                            validation.ErrorStyle = ExcelDataValidationWarningStyle.warning;
                            validation.ErrorTitle = "Invalid Selection";
                            validation.Error = "Please select either True or False.";
                            validation.Formula.Values.Add("True");
                            validation.Formula.Values.Add("False");
                        }

                        j++;
                    }

                    // Get dynamic attributes
                    var TableNameDynamicAtts = _unitOfWork.DynamicAttRepository
                        .GetDynamicLibAtts(TableNameEntity.Id, CategoryId)
                        .AsQueryable()
                        .Where(x => !x.disable)
                        .ToList();

                    // Loop through dynamic attributes
                    foreach (var TableNameDynamicAtt in TableNameDynamicAtts)
                    {
                        worksheet.Cells[1, j].Value = TableNameDynamicAtt.Key;

                        if (TableNameDynamicAtt.DataType.ToLower() == "bool")
                        {
                            var validation = worksheet.DataValidations.AddListValidation(worksheet.Cells[2, j, 10000, j].Address);
                            validation.ShowErrorMessage = true;
                            validation.ErrorStyle = ExcelDataValidationWarningStyle.warning;
                            validation.ErrorTitle = "Invalid Selection";
                            validation.Error = "Please select either True or False.";
                            validation.Formula.Values.Add("True");
                            validation.Formula.Values.Add("False");
                        }
                        else if (TableNameDynamicAtt.DataType.ToLower() == "list")
                        {
                            var values = _unitOfWork.DynamicListValuesRepository.GetWhere(x => x.dynamicAttId == TableNameDynamicAtt.Id).ToList();
                            if (values.Count > 0)
                            {
                                var validation = worksheet.DataValidations.AddListValidation(worksheet.Cells[2, j, 10000, j].Address);
                                validation.ShowErrorMessage = true;
                                validation.ErrorStyle = ExcelDataValidationWarningStyle.warning;
                                validation.ErrorTitle = "Invalid Selection";
                                validation.Error = "Select a value from the list.";
                                foreach (var value in values)
                                {
                                    validation.Formula.Values.Add(value.Value);
                                }
                            }
                        }
                        else if (TableNameDynamicAtt.DataType.ToLower() == "datetime")
                        {
                            var validation = worksheet.DataValidations.AddDateTimeValidation(worksheet.Cells[2, j, 1000, j].Address);
                            validation.ShowErrorMessage = true;
                            validation.ErrorStyle = ExcelDataValidationWarningStyle.warning;
                            validation.ErrorTitle = "Invalid Date";
                            validation.Error = "The date you have entered is invalid. Please use mm/dd/yyyy Hh:Mm:ss AM format.";
                            validation.Formula.Value = new DateTime(2000, 03, 16, 12, 0, 0);
                            validation.Formula2.Value = new DateTime(2050, 01, 01, 12, 0, 0);
                            validation.AllowBlank = true;

                            worksheet.Cells[2, j].Value = TableNameDynamicAtt.Value;
                        }
                        else
                        {
                            worksheet.Cells[2, j].Value = TableNameDynamicAtt.Value;
                        }

                        j++;
                    }

                    // Set document properties
                    package.Workbook.Properties.Title = TableName;
                    package.Workbook.Properties.Author = "DIS";
                    worksheet.Calculate();
                    worksheet.Cells.AutoFitColumns(0);
                    package.SaveAs(file);
                }

                return new Response<string>(true, file.DirectoryName, null, null, (int)Helpers.Constants.ApiReturnCode.success);
            }
            catch (Exception err)
            {
                return new Response<string>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
        }




        //public Response<List<int>> ImportFile(string FilePath, string TableName)
        //{
        //    using (TransactionScope transaction = new TransactionScope())
        //    {
        //        try
        //        {
        //            FileInfo existingFile = new FileInfo(FilePath);
        //            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        //            var ActivatedAtts = _unitOfWork.AttributeActivatedRepository.GetAllAsQueryable().Where(x => x.Tabel == TableName).Select(x => x.Key).ToList();
        //            List<int> UnsavedRows = new List<int>();
        //            using (ExcelPackage package = new ExcelPackage(existingFile))
        //            {
        //                ExcelWorksheet sheet = package.Workbook.Worksheets[0];
        //                int Rows = sheet.Dimension.End.Row;
        //                int Columns = sheet.Dimension.End.Column;
        //                int ActColumns = 0;
        //                for (int i = 1; i <= Columns; i++)
        //                {ب
        //                    var ColName = sheet.Cells[1, i].Value.ToString();
        //                    if ((ActivatedAtts.Where(x => x == ColName).FirstOrDefault()) == null)
        //                    {
        //                        ActColumns = i;
        //                        break;
        //                    }
        //                }
        //                if (Helpers.Constants.CivilType.TLIcivilWithLegLibrary.ToString() == TableName)
        //                {
        //                    bool test = false;
        //                    for (int i = 2; i <= Rows; i++)
        //                    {
        //                        TLIcivilWithLegLibrary civilWithLeg = new TLIcivilWithLegLibrary();
        //                        test = false;
        //                        for (int j = 1; j < ActColumns; j++)
        //                        {
        //                            var ColName = sheet.Cells[1, j].Value.ToString();
        //                            var value = sheet.Cells[i, j].Value;
        //                            test = AddCivilWithLegLibrary(ref civilWithLeg, ColName, value, i);
        //                            if (test == true)
        //                            {
        //                                UnsavedRows.Add(i);
        //                                break;
        //                            }
        //                        }
        //                        if (test == false)
        //                        {
        //                            _unitOfWork.CivilWithLegLibraryRepository.Add(civilWithLeg);
        //                            _unitOfWork.SaveChanges();
        //                        }
        //                    }
        //                }
        //                else if (Helpers.Constants.CivilType.TLIcivilWithoutLegLibrary.ToString() == TableName)
        //                {
        //                    bool test = false;
        //                    for (int i = 2; i <= Rows; i++)
        //                    {
        //                        TLIcivilWithLegLibrary civilWithLeg = new TLIcivilWithLegLibrary();
        //                        test = false;
        //                        for (int j = 1; j < ActColumns; j++)
        //                        {
        //                            var ColName = sheet.Cells[1, j].Value.ToString();
        //                            var value = sheet.Cells[i, j].Value;
        //                            test = AddCivilWithLegLibrary(ref civilWithLeg, ColName, value, i);
        //                            if (test == true)
        //                            {
        //                                UnsavedRows.Add(i);
        //                                break;
        //                            }
        //                        }
        //                        if (test == false)
        //                        {
        //                            try
        //                            {
        //                                _unitOfWork.CivilWithLegLibraryRepository.Add(civilWithLeg);
        //                                _unitOfWork.SaveChanges();
        //                            }
        //                            catch (Exception err)
        //                            {
        //                                
        //                                UnsavedRows.Add(i);
        //                            }
        //                        }
        //                    }
        //                }
        //            }
        //            transaction.Complete();
        //            return new Response<List<int>>(true, UnsavedRows, null, null, (int)Helpers.Constants.ApiReturnCode.success);
        //        }
        //        catch (Exception err)
        //        {
        //            
        //            return new Response<List<int>>(true, null, null, null, (int)Helpers.Constants.ApiReturnCode.fail);
        //        }
        //    }


        //}
        //Function take 2 parameters 
        //First FilePath refer to document i deal with
        //Second TableName refer to table i deal with
        public Response<List<KeyValuePair<int, string>>> ImportFile(IFormFile file, string TableName, int? CategoryId, string ConnectionString,int UserId)
        {

            using (var connection = new OracleConnection(ConnectionString))
            {

                connection.Open();

                using (var tran = connection.BeginTransaction())
                {

                    List<KeyValuePair<int, string>> UnsavedRows = new List<KeyValuePair<int, string>>();

                    var FilePath = SaveFileAndGetFilePath(file);
                    try
                    {
                        //Get info about file
                        FileInfo existingFile = new FileInfo(FilePath);
                        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                        //Get activated attributes depened on table name
                        List<string> ActivatedAtts = _unitOfWork.AttributeActivatedRepository.GetWhereAndSelect(x =>
                            x.Tabel == TableName && x.enable, x => x.Key).ToList();
                        //remove Id from activated attributes

                        foreach (var x in ActivatedAtts.ToList())
                        {
                            ActivatedAtts.Remove("Id");
                            ActivatedAtts.Remove("Active");
                            ActivatedAtts.Remove("Deleted");
                            if (x.ToLower().Contains("_name"))
                            {
                                ActivatedAtts.Remove(x);
                            }
                        }


                        //UnsavedRows list key and value
                        //key refer to row number
                        //value refer to error type
                        //Get related tables depened on table name
                        List<KeyValuePair<string, List<DropDownListFilters>>> RelatedTables = GetTableNameRelatedTables(TableName);
                        //Get table name entity from TLItablesNames by TableName
                        TLItablesNames TableNameEntity = _unitOfWork.TablesNamesRepository.GetWhereFirst(x => x.TableName == TableName);
                        //int TableNameId = _unitOfWork.TablesNamesRepository.GetWhere(x => x.TableName == TableName).Select(x => x.Id).FirstOrDefault();
                        //get dynamic attributes depened on table name Id
                        List<string> DynamicAtts = _unitOfWork.DynamicAttRepository.GetWhereAndSelect(x =>
                            x.tablesNamesId == TableNameEntity.Id && x.CivilWithoutLegCategoryId == CategoryId && !x.disable, x => x.Key).ToList();
                        List<string> RelatedAtt = new List<string>();
                        string Related = "";
                        foreach (var item in RelatedTables)
                        {
                            Related = item.Key;
                            RelatedAtt.Add(Related);
                        }
                        var StaticAttribute = ActivatedAtts.Union(RelatedAtt);
                        using (ExcelPackage package = new ExcelPackage(existingFile))
                        {
                            //Get the first sheet in excel file
                            ExcelWorksheet sheet = package.Workbook.Worksheets[0];
                            //Get number of rows in excel file
                            int Rows = sheet.Dimension.End.Row;
                            //Get number of columns in excel file
                            int Columns = sheet.Dimension.End.Column;
                            int ActColumns = 0;
                            DataTable dt = new DataTable();
                            List<string> SheetColumn = new List<string>();
                            //Get Columns names from first rows from excel sheet and add it SheetColumn
                            for (int i = 1; i <= Columns; i++)
                            {
                                SheetColumn.Add(sheet.Cells[1, i].Value.ToString());

                            }
                            if(TableName== "TLIcivilWithLegLibrary")
                            {
                                ActivatedAtts.Remove("NumberOfLegs");
                            }
                            List<string> MissedAttributes = new List<string>();
                            //Get activated attributes that exist in database and not found in excel sheet
                            MissedAttributes = ActivatedAtts.Except(SheetColumn).ToList();
                            List<string> MissedDynamicAttributes = new List<string>();
                            List<TLIdynamicAtt> OnlyForTestMissedDynamicAttributes = new List<TLIdynamicAtt>();
                            //Get dynamic attributes that exist in database and not found in excel sheet
                            MissedDynamicAttributes = DynamicAtts.Except(SheetColumn).ToList();
                            string message = null;
                            //if there is difference between activated attributes and excel sheet
                            //build error message contains attributes that missed 
                            if (MissedAttributes.Count > 0)
                            {
                                message = "Those base Atts ";
                                for (int i = 0; i < MissedAttributes.Count; i++)
                                {
                                    message += $"{MissedAttributes[i]}/";
                                }
                                message += " are missing";
                            }

                            //if there is difference between dynamic attributes and excel sheet
                            //build error message contains attributes that missed 
                            if (MissedDynamicAttributes.Count > 0)
                            {
                                message += ", Those dynamic attributes ";
                                for (int i = 0; i < MissedDynamicAttributes.Count; i++)
                                {
                                    message += $"{MissedDynamicAttributes[i]}/";
                                }
                                message += " are missing";
                            }

                            //check if message not null then there is error message return it to user
                            if (!String.IsNullOrEmpty(message))
                            {
                                return new Response<List<KeyValuePair<int, string>>>(false, null, null, message, (int)Helpers.Constants.ApiReturnCode.fail);
                            }
                            //to check where activated attributes ended and where dynamic attributes start  
                            //Add columns of excel sheet to datatable
                            for (int i = 1; i <= Columns; i++)
                            {
                                var ColName = sheet.Cells[1, i].Value.ToString();
                                if (StaticAttribute.FirstOrDefault(x => x == ColName) == null && ActColumns == 0)
                                {
                                    ActColumns = i;
                                }
                                dt.Columns.Add(ColName);
                            }
                            //Check table i deal with
                            if (Helpers.Constants.CivilType.TLIcivilWithLegLibrary.ToString() == TableName)
                            {
                                bool test = false;
                                //loop begin from second row from excel sheet
                                for (int i = 2; i <= Rows; i++)
                                {
                                    //add new row to data row
                                    DataRow dr = dt.NewRow();
                                    test = false;
                                    //loop begin from first column in specific row
                                    for (int j = 1; j <= Columns; j++)
                                    {
                                        //take the column name 
                                        var ColName = sheet.Cells[1, j].Value.ToString();
                                        //if column name not Active or Deleted the add value to the row in datatable
                                        if (ColName != "Active" && ColName != "Deleted")
                                        {
                                            var value = sheet.Cells[i, j].Value;
                                            dr[ColName] = value;
                                        }
                                    }
                                    //Add row to the datatable
                                    dt.Rows.Add(dr);
                                }
                                List<int> RecordId = new List<int>();
                                var TableNameId = _unitOfWork.TablesNamesRepository.GetWhereFirst(x => x.TableName == TableNameEntity.TableName).Id;
                                SaveCivilWithLegLibraryUsingOracleBulkCopy(UserId,out RecordId, dt, ref UnsavedRows, ActColumns, Columns, sheet, RelatedTables, TableNameEntity, connection);
                                if (UnsavedRows != null && UnsavedRows.Count != 0)
                                {
                                    return new Response<List<KeyValuePair<int, string>>>(false, UnsavedRows, null, UnsavedRows[0].Value, (int)Helpers.Constants.ApiReturnCode.fail);

                                }
                                else
                                {
                                    SaveLogisticalItemUsingOracleBulkCopy(RecordId, dt, ref UnsavedRows, ActColumns, Columns, sheet, RelatedTables, TableNameId, connection);
                                }
                            }
                            else if (Helpers.Constants.CivilType.TLIcivilWithoutLegLibrary.ToString() == TableName)
                            {
                                bool test = false;
                                for (int i = 2; i <= Rows; i++)
                                {
                                    DataRow dr = dt.NewRow();
                                    test = false;
                                    for (int j = 1; j <= Columns; j++)
                                    {
                                        var ColName = sheet.Cells[1, j].Value.ToString();
                                        if (ColName != "Active" && ColName != "Deleted")
                                        {
                                            var value = sheet.Cells[i, j].Value;
                                            dr[ColName] = value;
                                        }
                                    }
                                    dt.Rows.Add(dr);
                                }
                                List<int> RecordId = new List<int>();
                                var TableNameId = _unitOfWork.TablesNamesRepository.GetWhereFirst(x => x.TableName == TableNameEntity.TableName).Id;
                                SaveCivilWithoutLegLibraryUsingOracleBulkCopy(UserId, out RecordId, dt, ref UnsavedRows, ActColumns, Columns, sheet, RelatedTables, TableNameEntity, connection);
                                if (UnsavedRows != null && UnsavedRows.Count != 0)
                                {
                                    return new Response<List<KeyValuePair<int, string>>>(false, UnsavedRows, null, UnsavedRows[0].Value, (int)Helpers.Constants.ApiReturnCode.fail);

                                }
                                else
                                {
                                    SaveLogisticalItemUsingOracleBulkCopy(RecordId, dt, ref UnsavedRows, ActColumns, Columns, sheet, RelatedTables, TableNameId, connection);
                                }
                            }
                            else if (Helpers.Constants.CivilType.TLIcivilNonSteelLibrary.ToString() == TableName)
                            {
                                bool test = false;
                                for (int i = 2; i <= Rows; i++)
                                {
                                    DataRow dr = dt.NewRow();
                                    test = false;
                                    for (int j = 1; j <= Columns; j++)
                                    {
                                        var ColName = sheet.Cells[1, j].Value.ToString();
                                        if (ColName != "Active" && ColName != "Deleted")
                                        {
                                            var value = sheet.Cells[i, j].Value;
                                            dr[ColName] = value;
                                        }
                                    }
                                    dt.Rows.Add(dr);
                                }
                                List<int> RecordId = new List<int>();
                                var TableNameId = _unitOfWork.TablesNamesRepository.GetWhereFirst(x => x.TableName == TableNameEntity.TableName).Id;
                                SaveCivilNonSteelLibraryUsingOracleBulkCopy(UserId,out RecordId, dt, ref UnsavedRows, ActColumns, Columns, sheet, RelatedTables, TableNameEntity, connection);
                                if (UnsavedRows != null && UnsavedRows.Count != 0)
                                {
                                    return new Response<List<KeyValuePair<int, string>>>(false, UnsavedRows, null, UnsavedRows[0].Value, (int)Helpers.Constants.ApiReturnCode.fail);

                                }
                                else
                                {
                                    SaveLogisticalItemUsingOracleBulkCopy(RecordId, dt, ref UnsavedRows, ActColumns, Columns, sheet, RelatedTables, TableNameId, connection);
                                }
                            }
                            else if (Helpers.Constants.LoadSubType.TLIloadOtherLibrary.ToString() == TableName)
                            {
                                bool test = false;
                                for (int i = 2; i <= Rows; i++)
                                {
                                    DataRow dr = dt.NewRow();
                                    test = false;
                                    for (int j = 1; j <= Columns; j++)
                                    {
                                        var ColName = sheet.Cells[1, j].Value.ToString();
                                        if (ColName != "Active" && ColName != "Deleted")
                                        {
                                            var value = sheet.Cells[i, j].Value;
                                            dr[ColName] = value;
                                        }
                                    }
                                    dt.Rows.Add(dr);
                                }
                                List<int> RecordId = new List<int>();
                                var TableNameId = _unitOfWork.TablesNamesRepository.GetWhereFirst(x => x.TableName == TableNameEntity.TableName).Id;
                                SaveloadOtherLibraryUsingOracleBulkCopy(UserId,out RecordId, dt, ref UnsavedRows, ActColumns, Columns, sheet, TableNameEntity, connection);
                                if (UnsavedRows != null && UnsavedRows.Count != 0)
                                {
                                    return new Response<List<KeyValuePair<int, string>>>(false, UnsavedRows, null, UnsavedRows[0].Value, (int)Helpers.Constants.ApiReturnCode.fail);

                                }
                                else
                                {
                                    SaveLogisticalItemUsingOracleBulkCopy(RecordId, dt, ref UnsavedRows, ActColumns, Columns, sheet, RelatedTables, TableNameId, connection);
                                }
                            }
                            else if (Helpers.Constants.LoadSubType.TLImwBULibrary.ToString() == TableName)
                            {
                                bool test = false;
                                for (int i = 2; i <= Rows; i++)
                                {
                                    DataRow dr = dt.NewRow();
                                    test = false;
                                    for (int j = 1; j <= Columns; j++)
                                    {
                                        var ColName = sheet.Cells[1, j].Value.ToString();
                                        if (ColName != "Active" && ColName != "Deleted")
                                        {
                                            var value = sheet.Cells[i, j].Value;
                                            dr[ColName] = value;
                                        }
                                    }
                                    dt.Rows.Add(dr);
                                }
                                List<int> RecordId = new List<int>();
                                var TableNameId = _unitOfWork.TablesNamesRepository.GetWhereFirst(x => x.TableName == TableNameEntity.TableName).Id;
                                SavemwBULibraryUsingOracleBulkCopy(UserId,out RecordId, dt, ref UnsavedRows, ActColumns, Columns, sheet, RelatedTables, TableNameEntity, connection);
                                if (UnsavedRows != null && UnsavedRows.Count != 0)
                                {
                                    return new Response<List<KeyValuePair<int, string>>>(false, UnsavedRows, null, UnsavedRows[0].Value, (int)Helpers.Constants.ApiReturnCode.fail);

                                }
                                else
                                {
                                    SaveLogisticalItemUsingOracleBulkCopy(RecordId, dt, ref UnsavedRows, ActColumns, Columns, sheet, RelatedTables, TableNameId, connection);
                                }
                            }
                            else if (Helpers.Constants.LoadSubType.TLImwDishLibrary.ToString() == TableName)
                            {
                                bool test = false;
                                for (int i = 2; i <= Rows; i++)
                                {
                                    DataRow dr = dt.NewRow();
                                    test = false;
                                    for (int j = 1; j <= Columns; j++)
                                    {
                                        var ColName = sheet.Cells[1, j].Value.ToString();
                                        if (ColName != "Active" && ColName != "Deleted")
                                        {
                                            var value = sheet.Cells[i, j].Value;
                                            dr[ColName] = value;
                                        }
                                    }
                                    dt.Rows.Add(dr);
                                }
                                List<int> RecordId = new List<int>();
                                var TableNameId = _unitOfWork.TablesNamesRepository.GetWhereFirst(x => x.TableName == TableNameEntity.TableName).Id;
                                SavemwDishLibraryUsingOracleBulkCopy(UserId, out RecordId, dt, ref UnsavedRows, ActColumns, Columns, sheet, RelatedTables, TableNameEntity, connection);
                                if (UnsavedRows != null && UnsavedRows.Count != 0)
                                {
                                    return new Response<List<KeyValuePair<int, string>>>(false, UnsavedRows, null, UnsavedRows[0].Value, (int)Helpers.Constants.ApiReturnCode.fail);

                                }
                                else
                                {
                                    SaveLogisticalItemUsingOracleBulkCopy(RecordId, dt, ref UnsavedRows, ActColumns, Columns, sheet, RelatedTables, TableNameId, connection);
                                }
                            }
                            else if (Helpers.Constants.LoadSubType.TLImwODULibrary.ToString() == TableName)
                            {
                                bool test = false;
                                for (int i = 2; i <= Rows; i++)
                                {
                                    DataRow dr = dt.NewRow();
                                    test = false;
                                    for (int j = 1; j <= Columns; j++)
                                    {
                                        var ColName = sheet.Cells[1, j].Value.ToString();
                                        if (ColName != "Active" && ColName != "Deleted")
                                        {
                                            var value = sheet.Cells[i, j].Value;
                                            dr[ColName] = value;
                                        }
                                    }
                                    dt.Rows.Add(dr);
                                }
                                List<int> RecordId = new List<int>();
                                var TableNameId = _unitOfWork.TablesNamesRepository.GetWhereFirst(x => x.TableName == TableNameEntity.TableName).Id;
                                SavemwODULibraryUsingOracleBulkCopy(UserId, out RecordId, dt, ref UnsavedRows, ActColumns, Columns, sheet, RelatedTables, TableNameEntity, connection);
                                if (UnsavedRows != null && UnsavedRows.Count != 0)
                                {
                                    return new Response<List<KeyValuePair<int, string>>>(false, UnsavedRows, null, UnsavedRows[0].Value, (int)Helpers.Constants.ApiReturnCode.fail);

                                }
                                else
                                {
                                    SaveLogisticalItemUsingOracleBulkCopy(RecordId, dt, ref UnsavedRows, ActColumns, Columns, sheet, RelatedTables, TableNameId, connection);
                                }
                            }
                            else if (Helpers.Constants.LoadSubType.TLImwOtherLibrary.ToString() == TableName)
                            {
                                bool test = false;
                                for (int i = 2; i <= Rows; i++)
                                {
                                    DataRow dr = dt.NewRow();
                                    test = false;
                                    for (int j = 1; j <= Columns; j++)
                                    {
                                        var ColName = sheet.Cells[1, j].Value.ToString();
                                        if (ColName != "Active" && ColName != "Deleted")
                                        {
                                            var value = sheet.Cells[i, j].Value;
                                            dr[ColName] = value;
                                        }
                                    }
                                    dt.Rows.Add(dr);
                                }
                                List<int> RecordId = new List<int>();
                                var TableNameId = _unitOfWork.TablesNamesRepository.GetWhereFirst(x => x.TableName == TableNameEntity.TableName).Id;
                                SavemwOtherLibraryUsingOracleBulkCopy(UserId, out RecordId, dt, ref UnsavedRows, ActColumns, Columns, sheet, TableNameEntity, connection);
                                if (UnsavedRows != null && UnsavedRows.Count != 0)
                                {
                                    return new Response<List<KeyValuePair<int, string>>>(false, UnsavedRows, null, UnsavedRows[0].Value, (int)Helpers.Constants.ApiReturnCode.fail);

                                }
                                else
                                {
                                    SaveLogisticalItemUsingOracleBulkCopy(RecordId, dt, ref UnsavedRows, ActColumns, Columns, sheet, RelatedTables, TableNameId, connection);
                                }
                            }
                            else if (Helpers.Constants.LoadSubType.TLImwRFULibrary.ToString() == TableName)
                            {
                                bool test = false;
                                for (int i = 2; i <= Rows; i++)
                                {
                                    DataRow dr = dt.NewRow();
                                    test = false;
                                    for (int j = 1; j <= Columns; j++)
                                    {
                                        var ColName = sheet.Cells[1, j].Value.ToString();
                                        if (ColName != "Active" && ColName != "Deleted")
                                        {
                                            var value = sheet.Cells[i, j].Value;
                                            dr[ColName] = value;
                                        }
                                    }
                                    dt.Rows.Add(dr);
                                }
                                List<int> RecordId = new List<int>();
                                var TableNameId = _unitOfWork.TablesNamesRepository.GetWhereFirst(x => x.TableName == TableNameEntity.TableName).Id;
                                SavemwRFULibraryUsingOracleBulkCopy(UserId, out RecordId, dt, ref UnsavedRows, ActColumns, Columns, sheet, RelatedTables, TableNameEntity, connection);
                                if (UnsavedRows != null && UnsavedRows.Count != 0)
                                {
                                    return new Response<List<KeyValuePair<int, string>>>(false, UnsavedRows, null, UnsavedRows[0].Value, (int)Helpers.Constants.ApiReturnCode.fail);

                                }
                                else
                                {
                                    SaveLogisticalItemUsingOracleBulkCopy(RecordId, dt, ref UnsavedRows, ActColumns, Columns, sheet, RelatedTables, TableNameId, connection);
                                }
                            }
                            else if (Helpers.Constants.LoadSubType.TLIpowerLibrary.ToString() == TableName)
                            {
                                bool test = false;
                                for (int i = 2; i <= Rows; i++)
                                {
                                    DataRow dr = dt.NewRow();
                                    test = false;
                                    for (int j = 1; j <= Columns; j++)
                                    {
                                        var ColName = sheet.Cells[1, j].Value.ToString();
                                        if (ColName != "Active" && ColName != "Deleted")
                                        {
                                            var value = sheet.Cells[i, j].Value;
                                            dr[ColName] = value;
                                        }
                                    }
                                    dt.Rows.Add(dr);
                                }
                                List<int> RecordId = new List<int>();
                                var TableNameId = _unitOfWork.TablesNamesRepository.GetWhereFirst(x => x.TableName == TableNameEntity.TableName).Id;
                                SavepowerLibraryUsingOracleBulkCopy(UserId, out RecordId, dt, ref UnsavedRows, ActColumns, Columns, sheet, RelatedTables, TableNameEntity, connection);
                                if (UnsavedRows != null && UnsavedRows.Count != 0)
                                {
                                    return new Response<List<KeyValuePair<int, string>>>(false, UnsavedRows, null, UnsavedRows[0].Value, (int)Helpers.Constants.ApiReturnCode.fail);

                                }
                                else
                                {
                                    SaveLogisticalItemUsingOracleBulkCopy(RecordId, dt, ref UnsavedRows, ActColumns, Columns, sheet, RelatedTables, TableNameId, connection);
                                }
                            }
                            else if (Helpers.Constants.LoadSubType.TLIradioAntennaLibrary.ToString() == TableName)
                            {
                                bool test = false;
                                for (int i = 2; i <= Rows; i++)
                                {
                                    DataRow dr = dt.NewRow();
                                    test = false;
                                    for (int j = 1; j <= Columns; j++)
                                    {
                                        var ColName = sheet.Cells[1, j].Value.ToString();
                                        if (ColName != "Active" && ColName != "Deleted")
                                        {
                                            var value = sheet.Cells[i, j].Value;
                                            dr[ColName] = value;
                                        }
                                    }
                                    dt.Rows.Add(dr);
                                }
                                List<int> RecordId = new List<int>();
                                var TableNameId = _unitOfWork.TablesNamesRepository.GetWhereFirst(x => x.TableName == TableNameEntity.TableName).Id;
                                SaveradioAntennaLibraryUsingOracleBulkCopy(UserId, out RecordId, dt, ref UnsavedRows, ActColumns, Columns, sheet, RelatedTables, TableNameEntity, connection);
                                if (UnsavedRows != null && UnsavedRows.Count != 0)
                                {
                                    return new Response<List<KeyValuePair<int, string>>>(false, UnsavedRows, null, UnsavedRows[0].Value, (int)Helpers.Constants.ApiReturnCode.fail);

                                }
                                else
                                {
                                    SaveLogisticalItemUsingOracleBulkCopy(RecordId, dt, ref UnsavedRows, ActColumns, Columns, sheet, RelatedTables, TableNameId, connection);
                                }
                            }
                            else if (Helpers.Constants.LoadSubType.TLIradioOtherLibrary.ToString() == TableName)
                            {
                                bool test = false;
                                for (int i = 2; i <= Rows; i++)
                                {
                                    DataRow dr = dt.NewRow();
                                    test = false;
                                    for (int j = 1; j <= Columns; j++)
                                    {
                                        var ColName = sheet.Cells[1, j].Value.ToString();
                                        if (ColName != "Active" && ColName != "Deleted")
                                        {
                                            var value = sheet.Cells[i, j].Value;
                                            dr[ColName] = value;
                                        }
                                    }
                                    dt.Rows.Add(dr);
                                }
                                List<int> RecordId = new List<int>();
                                var TableNameId = _unitOfWork.TablesNamesRepository.GetWhereFirst(x => x.TableName == TableNameEntity.TableName).Id;
                                SaveradioOtherLibraryUsingOracleBulkCopy(UserId, out RecordId, dt, ref UnsavedRows, ActColumns, Columns, sheet, TableNameEntity, connection);
                                if (UnsavedRows != null && UnsavedRows.Count != 0)
                                {
                                    return new Response<List<KeyValuePair<int, string>>>(false, UnsavedRows, null, UnsavedRows[0].Value, (int)Helpers.Constants.ApiReturnCode.fail);

                                }
                                else
                                {
                                    SaveLogisticalItemUsingOracleBulkCopy(RecordId, dt, ref UnsavedRows, ActColumns, Columns, sheet, RelatedTables, TableNameId, connection);
                                }
                            }
                            else if (Helpers.Constants.LoadSubType.TLIradioRRULibrary.ToString() == TableName)
                            {
                                bool test = false;
                                for (int i = 2; i <= Rows; i++)
                                {
                                    DataRow dr = dt.NewRow();
                                    test = false;
                                    for (int j = 1; j <= Columns; j++)
                                    {
                                        var ColName = sheet.Cells[1, j].Value.ToString();
                                        if (ColName != "Active" && ColName != "Deleted")
                                        {
                                            var value = sheet.Cells[i, j].Value;
                                            dr[ColName] = value;
                                        }
                                    }
                                    dt.Rows.Add(dr);
                                }
                                List<int> RecordId = new List<int>();
                                var TableNameId = _unitOfWork.TablesNamesRepository.GetWhereFirst(x => x.TableName == TableNameEntity.TableName).Id;
                                SaveradioRRULibraryUsingOracleBulkCopy(UserId, out RecordId, dt, ref UnsavedRows, ActColumns, Columns, sheet, RelatedTables, TableNameEntity, connection);
                                if (UnsavedRows != null && UnsavedRows.Count != 0)
                                {
                                    return new Response<List<KeyValuePair<int, string>>>(false, UnsavedRows, null, UnsavedRows[0].Value, (int)Helpers.Constants.ApiReturnCode.fail);

                                }
                                else
                                {
                                    SaveLogisticalItemUsingOracleBulkCopy(RecordId, dt, ref UnsavedRows, ActColumns, Columns, sheet, RelatedTables, TableNameId, connection);
                                }
                            }
                            else if (Helpers.Constants.OtherInventoryType.TLIcabinetPowerLibrary.ToString() == TableName)
                            {
                                bool test = false;
                                for (int i = 2; i <= Rows; i++)
                                {
                                    DataRow dr = dt.NewRow();
                                    test = false;
                                    for (int j = 1; j <= Columns; j++)
                                    {
                                        var ColName = sheet.Cells[1, j].Value.ToString();
                                        if (ColName != "Active" && ColName != "Deleted")
                                        {
                                            var value = sheet.Cells[i, j].Value;
                                            dr[ColName] = value;
                                        }
                                    }
                                    dt.Rows.Add(dr);
                                }
                                List<int> RecordId = new List<int>();
                                var TableNameId = _unitOfWork.TablesNamesRepository.GetWhereFirst(x => x.TableName == TableNameEntity.TableName).Id;
                                SavecabinetPowerLibraryUsingOracleBulkCopy(UserId, out RecordId, dt, ref UnsavedRows, ActColumns, Columns, sheet, RelatedTables, TableNameEntity, connection);
                                if (UnsavedRows != null && UnsavedRows.Count != 0)
                                {
                                    return new Response<List<KeyValuePair<int, string>>>(false, UnsavedRows, null, UnsavedRows[0].Value, (int)Helpers.Constants.ApiReturnCode.fail);

                                }
                                else
                                {
                                    SaveLogisticalItemUsingOracleBulkCopy(RecordId, dt, ref UnsavedRows, ActColumns, Columns, sheet, RelatedTables, TableNameId, connection);
                                }
                            }
                            else if (Helpers.Constants.OtherInventoryType.TLIcabinetTelecomLibrary.ToString() == TableName)
                            {
                                bool test = false;
                                for (int i = 2; i <= Rows; i++)
                                {
                                    DataRow dr = dt.NewRow();
                                    test = false;
                                    for (int j = 1; j <= Columns; j++)
                                    {
                                        var ColName = sheet.Cells[1, j].Value.ToString();
                                        if (ColName != "Active" && ColName != "Deleted")
                                        {
                                            var value = sheet.Cells[i, j].Value;
                                            dr[ColName] = value;
                                        }
                                    }
                                    dt.Rows.Add(dr);
                                }
                                List<int> RecordId = new List<int>();
                                var TableNameId = _unitOfWork.TablesNamesRepository.GetWhereFirst(x => x.TableName == TableNameEntity.TableName).Id;
                                SavecabinetTelecomLibraryUsingOracleBulkCopy(UserId, out RecordId, dt, ref UnsavedRows, ActColumns, Columns, sheet, RelatedTables, TableNameEntity, connection);
                                if (UnsavedRows != null && UnsavedRows.Count != 0)
                                {
                                    return new Response<List<KeyValuePair<int, string>>>(false, UnsavedRows, null, UnsavedRows[0].Value, (int)Helpers.Constants.ApiReturnCode.fail);

                                }
                                else
                                {
                                    SaveLogisticalItemUsingOracleBulkCopy(RecordId, dt, ref UnsavedRows, ActColumns, Columns, sheet, RelatedTables, TableNameId, connection);
                                }
                            }
                            else if (Helpers.Constants.OtherInventoryType.TLIgeneratorLibrary.ToString() == TableName)
                            {
                                bool test = false;
                                for (int i = 2; i <= Rows; i++)
                                {
                                    DataRow dr = dt.NewRow();
                                    test = false;
                                    for (int j = 1; j <= Columns; j++)
                                    {
                                        var ColName = sheet.Cells[1, j].Value.ToString();
                                        if (ColName != "Active" && ColName != "Deleted")
                                        {
                                            var value = sheet.Cells[i, j].Value;
                                            dr[ColName] = value;
                                        }
                                    }
                                    dt.Rows.Add(dr);
                                }
                                List<int> RecordId = new List<int>();
                                var TableNameId = _unitOfWork.TablesNamesRepository.GetWhereFirst(x => x.TableName == TableNameEntity.TableName).Id;
                                SavegeneratorLibraryUsingOracleBulkCopy(UserId, out RecordId, dt, ref UnsavedRows, ActColumns, Columns, sheet, RelatedTables, TableNameEntity, connection);
                                if (UnsavedRows != null && UnsavedRows.Count != 0)
                                {
                                    return new Response<List<KeyValuePair<int, string>>>(false, UnsavedRows, null, UnsavedRows[0].Value, (int)Helpers.Constants.ApiReturnCode.fail);

                                }
                                else
                                {
                                    SaveLogisticalItemUsingOracleBulkCopy(RecordId, dt, ref UnsavedRows, ActColumns, Columns, sheet, RelatedTables, TableNameId, connection);
                                }
                            }
                            else if (Helpers.Constants.OtherInventoryType.TLIsolarLibrary.ToString() == TableName)
                            {
                                bool test = false;
                                for (int i = 2; i <= Rows; i++)
                                {
                                    DataRow dr = dt.NewRow();
                                    test = false;
                                    for (int j = 1; j <= Columns; j++)
                                    {
                                        var ColName = sheet.Cells[1, j].Value.ToString();
                                        if (ColName != "Active" && ColName != "Deleted")
                                        {
                                            var value = sheet.Cells[i, j].Value;
                                            dr[ColName] = value;
                                        }
                                    }
                                    dt.Rows.Add(dr);
                                }
                                List<int> RecordId = new List<int>();
                                var TableNameId = _unitOfWork.TablesNamesRepository.GetWhereFirst(x => x.TableName == TableNameEntity.TableName).Id;
                                SavesolarLibraryUsingOracleBulkCopy(UserId, out RecordId, dt, ref UnsavedRows, ActColumns, Columns, sheet, RelatedTables, TableNameEntity, connection);
                                if (UnsavedRows != null && UnsavedRows.Count != 0)
                                {
                                    return new Response<List<KeyValuePair<int, string>>>(false, UnsavedRows, null, UnsavedRows[0].Value, (int)Helpers.Constants.ApiReturnCode.fail);

                                }
                                else
                                {
                                    SaveLogisticalItemUsingOracleBulkCopy(RecordId, dt, ref UnsavedRows, ActColumns, Columns, sheet, RelatedTables, TableNameId, connection);
                                }
                            }
                            else if (Helpers.Constants.TablesNames.TLIsideArmLibrary.ToString() == TableName)
                            {
                                bool test = false;
                                for (int i = 2; i <= Rows; i++)
                                {
                                    DataRow dr = dt.NewRow();
                                    test = false;
                                    for (int j = 1; j <= Columns; j++)
                                    {
                                        var ColName = sheet.Cells[1, j].Value.ToString();
                                        if (ColName != "Active" && ColName != "Deleted")
                                        {
                                            var value = sheet.Cells[i, j].Value;
                                            dr[ColName] = value;
                                        }
                                    }
                                    dt.Rows.Add(dr);
                                }
                                List<int> RecordId = new List<int>();
                                var TableNameId = _unitOfWork.TablesNamesRepository.GetWhereFirst(x => x.TableName == TableNameEntity.TableName).Id;
                                SavesideArmLibraryUsingOracleBulkCopy(UserId, out RecordId, dt, ref UnsavedRows, ActColumns, Columns, sheet, TableNameEntity, connection);
                                if (UnsavedRows != null && UnsavedRows.Count != 0)
                                {
                                    return new Response<List<KeyValuePair<int, string>>>(false, UnsavedRows, null, UnsavedRows[0].Value, (int)Helpers.Constants.ApiReturnCode.fail);

                                }
                                else
                                {
                                    SaveLogisticalItemUsingOracleBulkCopy(RecordId, dt, ref UnsavedRows, ActColumns, Columns, sheet, RelatedTables, TableNameId, connection);
                                }
                            }

                            File.Delete(FilePath);
                            if (UnsavedRows == null || UnsavedRows.Count == 0)
                            {
                                tran.Commit();
                            }
                        }

                        if (UnsavedRows != null && UnsavedRows.Count != 0)
                        {
                            return new Response<List<KeyValuePair<int, string>>>(false, UnsavedRows, null, UnsavedRows[0].Value, (int)Helpers.Constants.ApiReturnCode.fail);

                        }
                        return new Response<List<KeyValuePair<int, string>>>(true, null, null, null, (int)Helpers.Constants.ApiReturnCode.success);
                    }
                    catch (Exception err)
                    {
                        File.Delete(FilePath);
                        tran.Dispose();
                        return new Response<List<KeyValuePair<int, string>>>(false, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
                    }

                }


            }

        }
        private List<KeyValuePair<string, List<DropDownListFilters>>> GetTableNameRelatedTables(string TableName)
        {
            List<KeyValuePair<string, List<DropDownListFilters>>> RelatedTables = new List<KeyValuePair<string, List<DropDownListFilters>>>();
            if (Helpers.Constants.CivilType.TLIcivilWithLegLibrary.ToString() == TableName)
            {
                RelatedTables = _unitOfWork.CivilWithLegLibraryRepository.GetRelatedTables();
            }
            else if (Helpers.Constants.CivilType.TLIcivilWithoutLegLibrary.ToString() == TableName)
            {
                RelatedTables = _unitOfWork.CivilWithoutLegLibraryRepository.GetRelatedTables();
            }
            else if (Helpers.Constants.CivilType.TLIcivilNonSteelLibrary.ToString() == TableName)
            {
                RelatedTables = _unitOfWork.CivilNonSteelLibraryRepository.GetRelatedTables();
            }
            else if (Helpers.Constants.LoadSubType.TLImwBULibrary.ToString() == TableName)
            {
                RelatedTables = _unitOfWork.MW_BULibraryRepository.GetRelatedTables();
            }
            else if (Helpers.Constants.LoadSubType.TLImwDishLibrary.ToString() == TableName)
            {
                RelatedTables = _unitOfWork.MW_DishLibraryRepository.GetRelatedTables();
            }
            else if (Helpers.Constants.LoadSubType.TLImwODULibrary.ToString() == TableName)
            {
                RelatedTables = _unitOfWork.MW_ODULibraryRepository.GetRelatedTables();
            }
            else if (Helpers.Constants.LoadSubType.TLImwRFULibrary.ToString() == TableName)
            {
                RelatedTables = _unitOfWork.MW_RFULibraryRepository.GetRelatedTables();
            }
            else if (Helpers.Constants.OtherInventoryType.TLIcabinetPowerLibrary.ToString() == TableName)
            {
                RelatedTables = _unitOfWork.CabinetPowerLibraryRepository.GetRelatedTables();
            }
            else if (Helpers.Constants.OtherInventoryType.TLIcabinetTelecomLibrary.ToString() == TableName)
            {
                RelatedTables = _unitOfWork.CabinetTelecomLibraryRepository.GetRelatedTables();
            }
            else if (Helpers.Constants.OtherInventoryType.TLIgeneratorLibrary.ToString() == TableName)
            {
                RelatedTables = _unitOfWork.GeneratorLibraryRepository.GetRelatedTables();
            }
            else if (Helpers.Constants.OtherInventoryType.TLIsolarLibrary.ToString() == TableName)
            {
                RelatedTables = _unitOfWork.SolarLibraryRepository.GetRelatedTables();
            }
            else if (Helpers.Constants.LoadSubType.TLIpowerLibrary.ToString() == TableName)
            {
                RelatedTables = _unitOfWork.PowerLibraryRepository.GetRelatedTables();
            }
            else if (Helpers.Constants.LoadSubType.TLIsideArmLibrary.ToString() == TableName)
            {
                RelatedTables = _unitOfWork.SideArmLibraryRepository.GetRelatedTables();
            }
            else if (Helpers.Constants.LoadSubType.TLIloadOtherLibrary.ToString() == TableName)
            {
                RelatedTables = _unitOfWork.LoadOtherLibraryRepository.GetRelatedTables();
            }
            else if (Helpers.Constants.LoadSubType.TLIradioAntennaLibrary.ToString() == TableName)
            {
                RelatedTables = _unitOfWork.RadioAntennaLibraryRepository.GetRelatedTables();
            }
            else if (Helpers.Constants.LoadSubType.TLIradioRRULibrary.ToString() == TableName)
            {
                RelatedTables = _unitOfWork.RadioRRULibraryRepository.GetRelatedTables();
            }
            else if (Helpers.Constants.LoadSubType.TLIradioOtherLibrary.ToString() == TableName)
            {
                RelatedTables = _unitOfWork.RadioOtherLibraryRepository.GetRelatedTables();
            }
            else if (Helpers.Constants.LoadSubType.TLImwOtherLibrary.ToString() == TableName)
            {
                RelatedTables = _unitOfWork.MW_OtherLibraryRepository.GetRelatedTables();
            }
            return RelatedTables;
        }

        //private bool AddCivilWithLegLibrary(ref TLIcivilWithLegLibrary civilWithLeg, string ColumnName, object Value, int RowNumber)
        //{
        //    var CivilWithLegLibraryRelatedTables = GetTableNameRelatedTables(Helpers.Constants.CivilType.TLIcivilWithLegLibrary.ToString());
        //    bool test = false;
        //    if(ColumnName == "Model")
        //    {
        //        civilWithLeg.Model = (string)Value;
        //    }
        //    else if(ColumnName == "Note")
        //    {
        //        civilWithLeg.Note = (string)Value;
        //    }
        //    else if (ColumnName == "Prefix")
        //    {
        //        if(Value == null)
        //        {
        //            civilWithLeg.Prefix = null;
        //        }
        //        else
        //        {
        //            civilWithLeg.Prefix = Convert.ToSingle(Value);
        //        }
        //    }
        //    else if (ColumnName == "Height_Designed")
        //    {
        //         if (Value == null)
        //         {
        //            civilWithLeg.Prefix = null;
        //         }
        //         else
        //         {
        //            civilWithLeg.Height_Designed = Convert.ToSingle(Value);
        //         }
        //    }
        //    else if (ColumnName == "Max_load_M2")
        //    {
        //          if (Value == null)
        //          {
        //            civilWithLeg.Prefix = null;
        //          }
        //          else
        //          {
        //            civilWithLeg.Max_load_M2 = Convert.ToSingle(Value);
        //          }
        //    }
        //    else if (ColumnName == "SpaceLibrary")
        //    {
        //           if(Value == null)
        //           {
        //             test = true;
        //           }
        //        civilWithLeg.SpaceLibrary = Convert.ToSingle(Value);
        //    }
        //    else if (ColumnName == "supportTypeDesignedId")
        //    {
        //        if (Value == null)
        //        {
        //            test = true;
        //        }
        //        var supportTypeDesigned = CivilWithLegLibraryRelatedTables.Where(x => x.Key == ColumnName).Select(x => x.Value).FirstOrDefault().Where(x => x.Value == Value.ToString()).FirstOrDefault();
        //        if(supportTypeDesigned != null)
        //        {
        //            civilWithLeg.supportTypeDesignedId = supportTypeDesigned.Id;
        //        }
        //        else
        //        {
        //            test = true;
        //        }
        //    }
        //    else if (ColumnName == "sectionsLegTypeId")
        //    {
        //        if (Value == null)
        //        {
        //            test = true;
        //        }
        //        var sectionsLegType = CivilWithLegLibraryRelatedTables.Where(x => x.Key == ColumnName).Select(x => x.Value).FirstOrDefault().Where(x => x.Value == Value.ToString()).FirstOrDefault();
        //        if (sectionsLegType != null)
        //        {
        //            civilWithLeg.sectionsLegTypeId = sectionsLegType.Id;
        //        }
        //        else
        //        {
        //            test = true;
        //        }
        //    }
        //    else if (ColumnName == "structureTypeId")
        //    {
        //        if(Value == null)
        //        {
        //            civilWithLeg.structureTypeId = null;
        //        }
        //        else
        //        {
        //            var structureType = CivilWithLegLibraryRelatedTables.Where(x => x.Key == ColumnName).Select(x => x.Value).FirstOrDefault().Where(x => x.Value == Value.ToString()).FirstOrDefault();
        //            if (structureType != null)
        //            {
        //                civilWithLeg.structureTypeId = structureType.Id;
        //            }
        //            else
        //            {
        //                civilWithLeg.structureTypeId = null;
        //            }
        //        }            
        //    }
        //    else if (ColumnName == "civilSteelSupportCategoryId")
        //    {
        //        if (Value == null)
        //        {
        //            test = true;
        //        }
        //        var sectionsLegType = CivilWithLegLibraryRelatedTables.Where(x => x.Key == ColumnName).Select(x => x.Value).FirstOrDefault().Where(x => x.Value == Value.ToString()).FirstOrDefault();
        //        if (sectionsLegType != null)
        //        {
        //            civilWithLeg.civilSteelSupportCategoryId = Convert.ToInt16(sectionsLegType.Id);
        //        }
        //        else
        //        {
        //            test = true;
        //        }
        //    }
        //    return test;
        //}

        /* Done*/
        private void SaveLogisticalItemUsingOracleBulkCopy(List<int> RecordId, DataTable dt, ref List<KeyValuePair<int, string>> UnsavedRows, int ActColumns, int Columns, ExcelWorksheet sheet, List<KeyValuePair<string, List<DropDownListFilters>>> RelatedTables, int TableNameId, OracleConnection connection)
        {
            using (TransactionScope trans = new TransactionScope())
            {
                try
                {
                    var TabelName = _dbContext.TLItablesNames.FirstOrDefault(x => x.Id == TableNameId);
                    TLIlogisticalitem addLogisticalitem = new TLIlogisticalitem();
                    for (int j = 0; j < dt.Rows.Count; j++)
                    {
                        string Name = "";

                        if (dt.Columns.Contains("Suppliers"))
                        {
                            DropDownListFilters Suppliertest = RelatedTables.FirstOrDefault(x =>
                                                       x.Key == "Suppliers").Value.FirstOrDefault(x => x.Value == dt.Rows[j]["Suppliers"].ToString());
                            if (Suppliertest != null)
                            {
                                addLogisticalitem = new TLIlogisticalitem()
                                {
                                    IsLib = true,
                                    RecordId = RecordId[j],
                                    tablesNamesId = TableNameId,
                                    logisticalId = Suppliertest.Id

                                };
                                _unitOfWork.LogisticalitemRepository.Add(addLogisticalitem);
                            }
                            else
                            {

                                addLogisticalitem = new TLIlogisticalitem()
                                {
                                    IsLib = true,
                                    RecordId = RecordId[j],
                                    tablesNamesId = TableNameId,
                                    logisticalId = null

                                };
                                _unitOfWork.LogisticalitemRepository.Add(addLogisticalitem);

                            }

                        }

                        if (dt.Columns.Contains("Designers"))
                        {
                            DropDownListFilters Designertest = RelatedTables.FirstOrDefault(x =>
                                                       x.Key == "Designers").Value.FirstOrDefault(x => x.Value == dt.Rows[j]["Designers"].ToString());
                            if (Designertest != null)
                            {

                                addLogisticalitem = new TLIlogisticalitem()
                                {
                                    IsLib = true,
                                    RecordId = RecordId[j],
                                    tablesNamesId = TableNameId,
                                    logisticalId = Designertest.Id

                                };
                                _unitOfWork.LogisticalitemRepository.Add(addLogisticalitem);
                            }
                            else
                            {
                                if (TabelName.TableName == "TLIsolarLibrary")
                                {
                                    UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"Designers can not to be null in the row {j + 2}"));
                                    goto ERROR;
                                }
                                addLogisticalitem = new TLIlogisticalitem()
                                {
                                    IsLib = true,
                                    RecordId = RecordId[j],
                                    tablesNamesId = TableNameId,
                                    logisticalId = null

                                };
                                _unitOfWork.LogisticalitemRepository.Add(addLogisticalitem);
                            }

                        }

                        if (dt.Columns.Contains("Manufacturers"))
                        {
                            DropDownListFilters Manufacturertest = RelatedTables.FirstOrDefault(x =>
                                                       x.Key == "Manufacturers").Value.FirstOrDefault(x => x.Value == dt.Rows[j]["Manufacturers"].ToString());
                            if (Manufacturertest != null)
                            {

                                addLogisticalitem = new TLIlogisticalitem()
                                {
                                    IsLib = true,
                                    RecordId = RecordId[j],
                                    tablesNamesId = TableNameId,
                                    logisticalId = Manufacturertest.Id

                                };
                                _unitOfWork.LogisticalitemRepository.Add(addLogisticalitem);
                            }
                            else
                            {
                                addLogisticalitem = new TLIlogisticalitem()
                                {
                                    IsLib = true,
                                    RecordId = RecordId[j],
                                    tablesNamesId = TableNameId,
                                    logisticalId = null

                                };
                                _unitOfWork.LogisticalitemRepository.Add(addLogisticalitem);
                            }

                        }

                        if (dt.Columns.Contains("Vendors"))
                        {
                            DropDownListFilters Vendortest = RelatedTables.FirstOrDefault(x =>
                                                       x.Key == "Vendors").Value.FirstOrDefault(x => x.Value == dt.Rows[j]["Vendors"].ToString());
                            if (Vendortest != null)
                            {

                                addLogisticalitem = new TLIlogisticalitem()
                                {
                                    IsLib = true,
                                    RecordId = RecordId[j],
                                    tablesNamesId = TableNameId,
                                    logisticalId = Vendortest.Id

                                };
                                _unitOfWork.LogisticalitemRepository.Add(addLogisticalitem);
                            }
                            else
                            {

                                if (TabelName.TableName == "TLIcivilWithLegLibrary" || TabelName.TableName == "TLIcivilWithoutLegLibrary"
                                   || TabelName.TableName == "TLImwDishLibrary" || TabelName.TableName == "TLImwBULibrary" || TabelName.TableName == "TLImwODULibrary"
                                   || TabelName.TableName == "TLImwRFULibrary" || TabelName.TableName == "TLIradioAntennaLibrary" || TabelName.TableName == "TLIradioRRULibrary"
                                   || TabelName.TableName == "TLIpowerLibrary" || TabelName.TableName == "TLIcabinetPowerLibrary" || TabelName.TableName == "TLIcabinetTelecomLibrary"
                                   || TabelName.TableName == "TLIgeneratorLibrary")
                                {
                                    UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"Vendors can not to be null in the row {j + 2}"));
                                    goto ERROR;
                                }
                                else
                                {
                                    addLogisticalitem = new TLIlogisticalitem()
                                    {
                                        IsLib = true,
                                        RecordId = RecordId[j],
                                        tablesNamesId = TableNameId,
                                        logisticalId = null

                                    };
                                    _unitOfWork.LogisticalitemRepository.Add(addLogisticalitem);
                                }
                            }

                        }

                        if (dt.Columns.Contains("Contractors"))
                        {
                            DropDownListFilters Contractorstest = RelatedTables.FirstOrDefault(x =>
                                                       x.Key == "Contractors").Value.FirstOrDefault(x => x.Value == dt.Rows[j]["Contractors"].ToString());
                            if (Contractorstest != null)
                            {

                                addLogisticalitem = new TLIlogisticalitem()
                                {
                                    IsLib = true,
                                    RecordId = RecordId[j],
                                    tablesNamesId = TableNameId,
                                    logisticalId = Contractorstest.Id

                                };
                                _unitOfWork.LogisticalitemRepository.Add(addLogisticalitem);
                            }
                            else
                            {
                                addLogisticalitem = new TLIlogisticalitem()
                                {
                                    IsLib = true,
                                    RecordId = RecordId[j],
                                    tablesNamesId = TableNameId,
                                    logisticalId = null

                                };
                                _unitOfWork.LogisticalitemRepository.Add(addLogisticalitem);
                            }

                        }

                        if (dt.Columns.Contains("Conultants"))
                        {
                            DropDownListFilters Conultantstest = RelatedTables.FirstOrDefault(x =>
                                                       x.Key == "Conultants").Value.FirstOrDefault(x => x.Value == dt.Rows[j]["Conultants"].ToString());
                            if (Conultantstest != null)
                            {
                                addLogisticalitem = new TLIlogisticalitem()
                                {
                                    IsLib = true,
                                    RecordId = RecordId[j],
                                    tablesNamesId = TableNameId,
                                    logisticalId = Conultantstest.Id

                                };
                                _unitOfWork.LogisticalitemRepository.Add(addLogisticalitem);
                            }
                            else
                            {
                                addLogisticalitem = new TLIlogisticalitem()
                                {
                                    IsLib = true,
                                    RecordId = RecordId[j],
                                    tablesNamesId = TableNameId,
                                    logisticalId = null

                                };
                                _unitOfWork.LogisticalitemRepository.Add(addLogisticalitem);
                            }

                        }
                    ERROR:;

                    }
                    if (UnsavedRows != null && UnsavedRows.Count != 0)
                    {

                    ERROR:;
                    }
                    else
                    {
                        _unitOfWork.SaveChanges();
                    }


                    trans.Complete();

                }
                catch (Exception err)
                {

                    throw err;
                }
            }

        }
        private void SaveCivilWithLegLibraryUsingOracleBulkCopy(int UserId, out List<int> RecordId, DataTable dt, ref List<KeyValuePair<int, string>> UnsavedRows, int ActColumns, int Columns, ExcelWorksheet sheet, List<KeyValuePair<string, List<DropDownListFilters>>> RelatedTables, TLItablesNames TableNameEntity, OracleConnection connection)
        {

            try
            {
                RecordId = new List<int>();
                //Get list of models for each record 
                List<string> Models = _dbContext.MV_CIVIL_WITHLEG_LIBRARY_VIEW.Where(x => !x.Deleted).Where(x => !x.Deleted).Select(x => x.Model).ToList();
                //get last id for table i deal with in the database
                TLIcivilWithLegLibrary LastId = _serviceProvider.GetService<ApplicationDbContext>().
                    TLIcivilWithLegLibrary.OrderByDescending(a => a.Id).FirstOrDefault();
                //Get dynamic attributes by table name
                List<TLIdynamicAtt> CivilWithLegLibraryDynamicAtts = _unitOfWork.DynamicAttRepository.GetIncludeWhere(x =>
                    x.tablesNamesId == TableNameEntity.Id && !x.disable, x => x.DataType).ToList();
                //var CivilWithLegLibraryDynamicAtts = _unitOfWork.DynamicAttRepository.GetAllAsQueryable().Include(x => x.DataType).Where(x => x.tablesNamesId == TableNameEntity.Id && !x.disable).ToList();
                //open connection to database
                //Create list for each property have all values for all rows
                List<string> models = new List<string>();
                List<string> notes = new List<string>();
                List<string> WidthVariations = new List<string>();
                List<string> prefixes = new List<string>();
                List<float> HeightsDesigned = new List<float>();
                List<float> maxloadsm2 = new List<float>();
                List<float> SpaceLibraries = new List<float>();
                List<float> ManufacturedMaxLoad = new List<float>();
                List<int> supportTypeDesignedIds = new List<int>();
                List<int> sectionsLegTypeIds = new List<int>();
                List<int> structureTypeIds = new List<int>();
                List<int> NumberOfLegss = new List<int>();


                //Create list of key and value
                //key refer to dynamic attribute id
                //value refer to list of values for this dynamic attribute
                List<Tuple<int, string, List<dynamic>>> DynamicAtts = new List<Tuple<int, string, List<dynamic>>>();
                TLIdynamicAtt DA = new TLIdynamicAtt();
                string ColName = null;
                //If ActColumns then there are dynamic attributes
                if (ActColumns > 0)
                {
                    //loop on each dynamic attribute
                    for (int i = ActColumns; i <= Columns; i++)
                    {
                        //Get column name
                        ColName = sheet.Cells[1, i].Value.ToString();
                        //Get dynamic attribute entity by column name
                        DA = CivilWithLegLibraryDynamicAtts.FirstOrDefault(x => x.Key == ColName);
                        if (DA != null)
                            DynamicAtts.Add(new Tuple<int, string, List<dynamic>>(DA.Id, DA.DataType.Name.ToLower(), new List<dynamic>()));
                    }
                }
                bool test;
                for (int j = 0; j < dt.Rows.Count; j++)
                {
                    try
                    {
                        //Check if model is alredy exists in database then add it to UnsavedRows
                        //check each property in datatable if match the database ex:if model in database is required the value in sheet shouldn't be null or empty
                        //if there is wrong insert or datatype then add it UnsavedRows

                        float spacelibrary_test = 0;
                        if (dt.Columns.Contains("SpaceLibrary"))
                        {
                            if (!String.IsNullOrEmpty(dt.Rows[j]["SpaceLibrary"].ToString()))
                            {
                                float spacelibrary_test1;
                                test = float.TryParse(dt.Rows[j]["SpaceLibrary"].ToString(), out spacelibrary_test1);
                                if (test == true)
                                {
                                    spacelibrary_test = spacelibrary_test1;
                                }
                                else if (test == true && spacelibrary_test1 == 0)
                                {
                                    UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"SpaceLibrary must to be bigger of zero{j + 2}"));
                                    goto ERROR;
                                }
                                else
                                {
                                    UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"SpaceLibrary Wrong Input DataType in the row {j + 2}"));
                                    goto ERROR;
                                }
                            }
                            else
                            {
                                UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"SpaceLibrary can not to be null and must to be bigger of zero{j + 2}"));
                                goto ERROR;
                            }
                        }
                        string note_test = null;
                        if (dt.Columns.Contains("Note"))
                        {
                            if (!String.IsNullOrEmpty(dt.Rows[j]["Note"].ToString()))
                            {
                                note_test = Convert.ToString(dt.Rows[j]["Note"]);
                            }
                        }
                        string WidthVariation_test = null;
                        if (dt.Columns.Contains("WidthVariation"))
                        {
                            if (!String.IsNullOrEmpty(dt.Rows[j]["WidthVariation"].ToString()))
                            {
                                WidthVariation_test = Convert.ToString(dt.Rows[j]["WidthVariation"]);
                            }
                        }
                        string prefix_test = null;
                        if (dt.Columns.Contains("Prefix"))
                        {
                            if (!String.IsNullOrEmpty(dt.Rows[j]["Prefix"].ToString()))
                            {
                                prefix_test = Convert.ToString(dt.Rows[j]["Prefix"]);
                            }
                        }

                        float heightdesigned_test = 0;
                        if (dt.Columns.Contains("Height_Designed"))
                        {
                            if (!String.IsNullOrEmpty(dt.Rows[j]["Height_Designed"].ToString()))
                            {
                                float heightdesigned_test_1;
                                test = float.TryParse(dt.Rows[j]["Height_Designed"].ToString(), out heightdesigned_test_1);
                                if (test == true)
                                {
                                    heightdesigned_test = (float)Math.Round(heightdesigned_test_1, 1, MidpointRounding.ToEven);
                                }
                                else
                                {
                                    UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"Height_Designed Wrong Input DataType in the row {j + 2}"));
                                    goto ERROR;
                                }
                            }
                            else
                            {
                                UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"Height_Designed can not to be null must input number in the row  {j + 2}"));
                                goto ERROR;
                            }
                        }

                        float Manufactured_Max_Load_test = 0;
                        if (dt.Columns.Contains("Manufactured_Max_Load"))
                        {
                            if (!String.IsNullOrEmpty(dt.Rows[j]["Manufactured_Max_Load"].ToString()))
                            {
                                float Manufactured_Max_Load_test_1;
                                test = float.TryParse(dt.Rows[j]["Manufactured_Max_Load"].ToString(), out Manufactured_Max_Load_test_1);
                                if (test == true)
                                {
                                    Manufactured_Max_Load_test = Manufactured_Max_Load_test_1;
                                }
                                else
                                {
                                    UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"Manufactured_Max_Load Wrong Input DataType in the row {j + 2}"));
                                    goto ERROR;
                                }
                            }
                            else
                            {
                                UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"Manufactured_Max_Load can not to be null must input number in the row  {j + 2}"));
                                goto ERROR;
                            }
                        }

                        float maxloadm2_test = 0;
                        if (dt.Columns.Contains("Max_load_M2"))
                        {
                            if (!String.IsNullOrEmpty(dt.Rows[j]["Max_load_M2"].ToString()))
                            {
                                float maxloadm2_test_1;
                                test = float.TryParse(dt.Rows[j]["Max_load_M2"].ToString(), out maxloadm2_test_1);
                                if (test == true)
                                {
                                    maxloadm2_test = maxloadm2_test_1;
                                }
                                else
                                {
                                    UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"Max_load_M2 Wrong Input DataType in the row {j + 2}"));
                                    goto ERROR;
                                }
                            }
                            else
                            {
                                UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"Max_load_M2 can not to be null must input number in the row  {j + 2}"));
                                goto ERROR;
                            }
                        }

                      
                        int NumberOfLegs_test = 0;
                    
                           
                    
                        int supporttypedesignedId_test = 0;

                        if (dt.Columns.Contains("supportTypeDesignedId"))
                        {
                            DropDownListFilters supportTypeDesigned = RelatedTables.FirstOrDefault(x =>
                                                       x.Key == "supportTypeDesignedId").Value.FirstOrDefault(x => x.Value == dt.Rows[j]["supportTypeDesignedId"].ToString());
                            if (supportTypeDesigned != null)
                            {
                                supporttypedesignedId_test = Convert.ToInt32(supportTypeDesigned.Id);
                            }
                            else
                            {
                                UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"SupportTypeDesignedId  must to be null  in the row {j + 2}"));
                                goto ERROR;
                            }
                        }
                        int sectionslegtypeId_test = 0;

                        if (dt.Columns.Contains("sectionsLegTypeId"))
                        {
                            DropDownListFilters sectionsLegType = RelatedTables.FirstOrDefault(x =>
                           x.Key == "sectionsLegTypeId").Value.FirstOrDefault(x =>
                               x.Value == dt.Rows[j]["sectionsLegTypeId"].ToString());
                            if (sectionsLegType != null)
                            {
                                sectionslegtypeId_test = Convert.ToInt32(sectionsLegType.Id);
                            }
                            else
                            {
                                UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"SectionsLegTypeId must to be null in the row {j + 2}"));
                                goto ERROR;
                            }
                        }

                        int structuretypeId_test = 0;
                        string structuretypeName_test = null;
                        if (dt.Columns.Contains("structureTypeId"))
                        {
                            if (!String.IsNullOrEmpty(dt.Rows[j]["structureTypeId"].ToString()))
                            {
                                DropDownListFilters structureType = RelatedTables.FirstOrDefault(x =>
                                    x.Key == "structureTypeId").Value.FirstOrDefault(x =>
                                        x.Value == dt.Rows[j]["structureTypeId"].ToString());
                                if (structureType != null)
                                {
                                    structuretypeId_test = Convert.ToInt32(structureType.Id);
                                    structuretypeName_test = structureType.Value;

                                    if (structuretypeName_test.ToLower() == "Triangular".ToLower())
                                    {
                                        NumberOfLegs_test = 3;
                                    }
                                    if (structuretypeName_test.ToLower() == "Square".ToLower())
                                    {
                                        NumberOfLegs_test = 4;
                                    }
                                }
                                else
                                {
                                    UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"StructureTypeId can not to be null in the row {j + 2}"));
                                    goto ERROR;
                                }
                            }
                        }
                        string model_test = null;
                        if (dt.Columns.Contains("Model"))
                        {
                            if (!string.IsNullOrEmpty(dt.Rows[j]["Vendors"].ToString()))
                            {
                                DropDownListFilters Vendortest = RelatedTables.FirstOrDefault(x =>
                                                      x.Key == "Vendors").Value.FirstOrDefault(x => x.Value == dt.Rows[j]["Vendors"].ToString());
                                if (Vendortest != null)
                                {
                                    model_test = Vendortest.Value + ' ' + prefix_test + ' ' + structuretypeName_test + ' ' + heightdesigned_test + "HE"; ;
                                    if ((Models.Find(x => x == Convert.ToString(model_test))) != null)
                                    {
                                        UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"Model is already exist in the row {j + 2}"));
                                        goto ERROR;
                                    }
                                }
                            }
                            else
                            {
                                UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"Vendors can not to be null in the row {j + 2}"));
                                goto ERROR;
                            }

                        }
                        List<dynamic> DynamicAttList = new List<dynamic>();
                        //check if there are dynamic attributes
                        if (ActColumns > 0)
                        {
                            for (int i = ActColumns; i <= Columns; i++)
                            {
                                string StringDynamicAtt = string.Empty;
                                double? DoubleDynamicAtt = null;
                                DateTime? DateTimeDynamicAtt = null;
                                Boolean? BooleanDynamicAtt = null;
                                ColName = sheet.Cells[1, i].Value.ToString();
                                DA = CivilWithLegLibraryDynamicAtts.FirstOrDefault(x => x.Key == ColName);
                                //Check dynamic attribute if required or not
                                if (DA != null)
                                {
                                    double double_Test = 0;
                                    DateTime datetime_Test = DateTime.Now;
                                    Boolean boolean_Test = false;
                                    if (DA.Required == true)
                                    {
                                        //if somthing wrong then add row to UnsavedRows
                                        if (String.IsNullOrEmpty(dt.Rows[j][ColName].ToString()))
                                        {
                                            UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"Dynamic Attribute ({ColName}) is required"));
                                            goto ERROR;
                                        }
                                        else
                                        {
                                            if (DA.DataType.Name.ToLower() == "string")
                                            {

                                                StringDynamicAtt = Convert.ToString(dt.Rows[j][ColName]);
                                            }
                                            else if (DA.DataType.Name.ToLower() == "double" || DA.DataType.Name.ToLower() == "int")
                                            {
                                                test = double.TryParse(dt.Rows[j][ColName].ToString(), out double_Test);
                                                if (test == false)
                                                {
                                                    UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"{ColName} Dynamic Attribute Wrong Input DataType in the row {j + 2}"));
                                                    goto ERROR;
                                                }
                                                DoubleDynamicAtt = double_Test;
                                            }
                                            else if (DA.DataType.Name.ToLower() == "datetime")
                                            {
                                                test = DateTime.TryParse(dt.Rows[j][ColName].ToString(), out datetime_Test);
                                                if (test == false)
                                                {
                                                    UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"{ColName} Dynamic Attribute Wrong Input DataType in the row {j + 2}"));
                                                    goto ERROR;
                                                }
                                                DateTimeDynamicAtt = datetime_Test;
                                            }
                                            else if (DA.DataType.Name.ToLower() == "bool")
                                            {
                                                test = Boolean.TryParse(dt.Rows[j][ColName].ToString(), out boolean_Test);
                                                if (test == false)
                                                {
                                                    UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"{ColName} Dynamic Attribute Wrong Input DataType in the row {j + 2}"));
                                                    goto ERROR;
                                                }
                                                BooleanDynamicAtt = boolean_Test;
                                            }


                                        }
                                    }
                                    else
                                    {
                                        if (!String.IsNullOrEmpty(dt.Rows[j][ColName].ToString()))
                                        {
                                            if (DA.DataType.Name.ToLower() == "string")
                                            {
                                                StringDynamicAtt = Convert.ToString(dt.Rows[j][ColName]);

                                            }
                                            else if (DA.DataType.Name.ToLower() == "double" || DA.DataType.Name.ToLower() == "int")
                                            {
                                                test = double.TryParse(dt.Rows[j][ColName].ToString(), out double_Test);
                                                if (test == false)
                                                {
                                                    UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"{ColName} Dynamic Attribute Wrong Input DataType in the row {j + 2}"));
                                                    goto ERROR;
                                                }
                                                DoubleDynamicAtt = double_Test;
                                            }
                                            else if (DA.DataType.Name.ToLower() == "datetime")
                                            {
                                                test = DateTime.TryParse(dt.Rows[j][ColName].ToString(), out datetime_Test);
                                                if (test == false)
                                                {
                                                    UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"{ColName} Dynamic Attribute Wrong Input DataType in the row {j + 2}"));
                                                    goto ERROR;
                                                }
                                                DateTimeDynamicAtt = datetime_Test;

                                            }
                                            else if (DA.DataType.Name.ToLower() == "bool")
                                            {
                                                test = Boolean.TryParse(dt.Rows[j][ColName].ToString(), out boolean_Test);
                                                if (test == false)
                                                {
                                                    UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"{ColName} Dynamic Attribute Wrong Input DataType in the row {j + 2}"));
                                                    goto ERROR;
                                                }
                                                BooleanDynamicAtt = boolean_Test;
                                            }

                                        }
                                    }
                                    if (DA.DataType.Name.ToLower() == "string")
                                    {
                                        DynamicAttList.Add(StringDynamicAtt);
                                    }
                                    else if (DA.DataType.Name.ToLower() == "double" || DA.DataType.Name.ToLower() == "int")
                                    {
                                        DynamicAttList.Add(DoubleDynamicAtt);
                                    }
                                    else if (DA.DataType.Name.ToLower() == "datetime")
                                    {
                                        DynamicAttList.Add(DateTimeDynamicAtt);
                                    }
                                    else if (DA.DataType.Name.ToLower() == "bool")
                                    {
                                        DynamicAttList.Add(BooleanDynamicAtt);
                                    }


                                }
                            }
                        }
                        models.Add(model_test != null ? model_test : null);
                        notes.Add(note_test != null ? note_test : null);
                        WidthVariations.Add(WidthVariation_test != null ? WidthVariation_test : null);
                        prefixes.Add(prefix_test != null ? prefix_test : null);
                        HeightsDesigned.Add(heightdesigned_test != null ? heightdesigned_test : (float)0);
                        maxloadsm2.Add(maxloadm2_test != null ? maxloadm2_test : (float)0);
                        ManufacturedMaxLoad.Add(Manufactured_Max_Load_test != null ? Manufactured_Max_Load_test : (float)0);
                        SpaceLibraries.Add(spacelibrary_test != null ? spacelibrary_test : 0);
                        supportTypeDesignedIds.Add(supporttypedesignedId_test != null ? supporttypedesignedId_test : 0);
                        NumberOfLegss.Add(NumberOfLegs_test != null ? NumberOfLegs_test : 0);
                        sectionsLegTypeIds.Add(sectionslegtypeId_test != null ? sectionslegtypeId_test : 0);
                        structureTypeIds.Add(structuretypeId_test != null ? structuretypeId_test : 0);

                        for (int f = 0; f < DynamicAttList.Count; f++)
                        {
                            DynamicAtts[f].Item3.Add(DynamicAttList[f]);
                        }


                    }
                    catch (Exception err)
                    {

                        //UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, "Wrong Input DataType"));
                    }
                }
                if (models.Count > 0)
                {
                    //Intial parameters to add records to database
                    //add parameter and give the parameter name
                    OracleParameter model = new OracleParameter();
                    //specify parameter datatype
                    model.OracleDbType = OracleDbType.NVarchar2;
                    //Add parameter value
                    model.Value = models.ToArray();

                    OracleParameter note = new OracleParameter();
                    note.OracleDbType = OracleDbType.NVarchar2;
                    note.Value = notes.ToArray();

                    OracleParameter prefixe = new OracleParameter();
                    prefixe.OracleDbType = OracleDbType.NVarchar2;
                    prefixe.Value = prefixes.ToArray();

                    OracleParameter HeightDesigned = new OracleParameter();
                    HeightDesigned.OracleDbType = OracleDbType.BinaryFloat;
                    HeightDesigned.Value = HeightsDesigned.ToArray();

                    OracleParameter maxloadm2 = new OracleParameter();
                    maxloadm2.OracleDbType = OracleDbType.BinaryFloat;
                    maxloadm2.Value = maxloadsm2.ToArray();

                    OracleParameter ManufacturedMaxLoads = new OracleParameter();
                    ManufacturedMaxLoads.OracleDbType = OracleDbType.BinaryFloat;
                    ManufacturedMaxLoads.Value = ManufacturedMaxLoad.ToArray();

                    OracleParameter SpaceLibrary = new OracleParameter();
                    SpaceLibrary.OracleDbType = OracleDbType.BinaryFloat;
                    SpaceLibrary.Value = SpaceLibraries.ToArray();

                    OracleParameter supportTypeDesignedId = new OracleParameter();
                    supportTypeDesignedId.OracleDbType = OracleDbType.Int32;
                    supportTypeDesignedId.Value = supportTypeDesignedIds.ToArray();

                    OracleParameter sectionsLegTypeId = new OracleParameter();
                    sectionsLegTypeId.OracleDbType = OracleDbType.Int32;
                    sectionsLegTypeId.Value = sectionsLegTypeIds.ToArray();

                    OracleParameter structureTypeId = new OracleParameter();
                    structureTypeId.OracleDbType = OracleDbType.Int32;
                    structureTypeId.Value = structureTypeIds.ToArray();


                    OracleParameter NumberOfLegSS = new OracleParameter();
                    NumberOfLegSS.OracleDbType = OracleDbType.Int32;
                    NumberOfLegSS.Value = NumberOfLegss.ToArray();

                    OracleParameter WidthVariation = new OracleParameter();
                    WidthVariation.OracleDbType = OracleDbType.NVarchar2;
                    WidthVariation.Value = WidthVariations.ToArray();

                    // create command and set properties
                    OracleCommand cmd = connection.CreateCommand();
                    cmd.CommandText = "INSERT INTO \"TLIcivilWithLegLibrary\" (\"Model\", \"Note\", \"Prefix\", \"Height_Designed\", \"Max_load_M2\", \"SpaceLibrary\", \"supportTypeDesignedId\", \"sectionsLegTypeId\", \"structureTypeId\", \"civilSteelSupportCategoryId\",\"Manufactured_Max_Load\",\"NumberOfLegs\",\"WidthVariation\") VALUES ( :1, :2, :3, :4, :5, :6, :7, :8, :9, :10, :11 , :12 ,:13)";
                    cmd.ArrayBindCount = models.Count;
                    cmd.Parameters.Add(model != null ? model : null);
                    cmd.Parameters.Add(note != null ? note : null);
                    cmd.Parameters.Add(prefixe != null ? prefixe : null);
                    cmd.Parameters.Add(HeightDesigned != null ? HeightDesigned : (float)0);
                    cmd.Parameters.Add(maxloadm2 != null ? maxloadm2 : 0);
                    cmd.Parameters.Add(SpaceLibrary != null ? SpaceLibrary : 0);
                    cmd.Parameters.Add(supportTypeDesignedId != null ? supportTypeDesignedId : null);
                    cmd.Parameters.Add(sectionsLegTypeId != null ? sectionsLegTypeId : null);
                    cmd.Parameters.Add(structureTypeId != null ? structureTypeId : null);
                    cmd.Parameters.Add(new OracleParameter(":10", OracleDbType.Int32) { Value = Enumerable.Repeat(DBNull.Value, models.Count).ToArray() });
                    cmd.Parameters.Add(ManufacturedMaxLoads != null ? ManufacturedMaxLoads : 0);
                    cmd.Parameters.Add(NumberOfLegSS != null ? NumberOfLegSS : 0);
                    cmd.Parameters.Add(WidthVariation != null ? WidthVariation : null);

                    cmd.ExecuteNonQuery();

                    //connection.Close();
                    List<int> InsertedIds = new List<int>();
                    //Check if LastId is null then table is empty
                    //else get inserted records ids
                    if (LastId != null)
                    {
                        OracleCommand InventoryCmd = connection.CreateCommand();
                        InventoryCmd.CommandType = CommandType.Text;
                        InventoryCmd.CommandText = $"Select \"Id\" From \"TLIcivilWithLegLibrary\" where \"TLIcivilWithLegLibrary\".\"Id\" > {LastId.Id}";
                        OracleDataReader Reader = InventoryCmd.ExecuteReader();
                        if (Reader.HasRows)
                        {
                            while (Reader.Read())
                            {
                                InsertedIds.Add(Reader.GetInt32(0));
                            }
                        }
                    }
                    else
                    {
                        OracleCommand InventoryCmd = connection.CreateCommand();
                        InventoryCmd.CommandType = CommandType.Text;
                        InventoryCmd.CommandText = $"Select \"Id\" From \"TLIcivilWithLegLibrary\"";
                        OracleDataReader Reader = InventoryCmd.ExecuteReader();
                        if (Reader.HasRows)
                        {
                            while (Reader.Read())
                            {
                                InsertedIds.Add(Reader.GetInt32(0));
                            }
                        }

                    }
                    TLIhistory tLIhistory = new TLIhistory()
                    {
                        UserId = UserId,
                        TablesNameId = TableNameEntity.Id,
                        HistoryTypeId = 1,

                    };
                    _dbContext.TLIhistory.Add(tLIhistory);
                    _dbContext.SaveChanges();
                    var civilWithLegLibraryList = new List<TLIcivilWithLegLibrary>();

                    for (int i = 0; i < models.Count; i++)
                    {
                        civilWithLegLibraryList.Add(new TLIcivilWithLegLibrary
                        {
                            Id = InsertedIds[i],
                            Model = models[i],
                            Note = notes[i],
                            Prefix = prefixes[i],
                            WidthVariation = WidthVariations[i],
                            Height_Designed = HeightsDesigned[i],
                            Max_load_M2 = maxloadsm2[i],
                            SpaceLibrary = SpaceLibraries[i],
                            Manufactured_Max_Load = ManufacturedMaxLoad[i],
                            supportTypeDesignedId = supportTypeDesignedIds[i],
                            sectionsLegTypeId = sectionsLegTypeIds[i],
                            structureTypeId = structureTypeIds[i],
                            NumberOfLegs = NumberOfLegss[i],
                        });
                    }


                    foreach (int recordId in InsertedIds)
                    {
                        var RecordName = civilWithLegLibraryList.FirstOrDefault(x => x.Id == recordId);
                        if (RecordName != null)
                        {
                            var attributeNames = RecordName.GetType().GetProperties()
                                .Where(x =>
                                    (x.PropertyType.IsGenericType && x.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>) &&
                                        new Type[] { typeof(int), typeof(string), typeof(double), typeof(float), typeof(Single), typeof(bool), typeof(DateTime) }
                                        .Contains(x.PropertyType.GetGenericArguments()[0]))
                                    || new Type[] { typeof(int), typeof(string), typeof(double), typeof(bool), typeof(DateTime), typeof(float), typeof(Single) }
                                    .Contains(x.PropertyType)
                                )
                                .ToList();

                            foreach (var propertyInfo in attributeNames)
                            {

                                var propertyName = propertyInfo.Name;

                                var propertyValue = propertyInfo.GetValue(RecordName)?.ToString().Trim();

                                TLIhistoryDet tLIhistoryDet = new TLIhistoryDet()
                                {
                                    HistoryId = tLIhistory.Id,
                                    TablesNameId = TableNameEntity.Id,
                                    RecordId = recordId.ToString(),
                                    AttributeName = propertyName,
                                    NewValue = propertyValue
                                };


                                _dbContext.TLIhistoryDet.Add(tLIhistoryDet);
                            }
                        }
                    }


                    foreach (var DynamicAtt in DynamicAtts)
                    {
                        for (int k = 0; k < DynamicAtt.Item3.Count; k++)
                        {
                            if (!String.IsNullOrEmpty(DynamicAtt.Item3[k].ToString()))

                            {
                                var DynamicAttrName = _dbContext.TLIdynamicAtt.FirstOrDefault(x => x.Id == DynamicAtt.Item1);
                                var Message = _unitOfWork.CivilWithLegsRepository.CheckDynamicValidationAndDependenceDynamic(DynamicAtt.Item1, DynamicAtt.Item3[k], Convert.ToInt32(InsertedIds[k]), tLIhistory.Id, civilWithLegLibraryList[0]).Message;
                                if (Message != "Success")
                                {
                                    UnsavedRows.Add(new KeyValuePair<int, string>(k + 2, Message + ' ' + $"in the Colum Name{DynamicAttrName.Key} in the Row Number {k + 2} "));
                                    goto ERROR;
                                }

                            }

                        }


                        //List<string> StringValues = new List<string>();
                        //List<int> IntValues = new List<int>();
                        //List<double> DoubleValues = new List<double>();
                        //List<DateTime> DateTimeValues = new List<DateTime>();
                        //List<Int32> BooleanValues = new List<Int32>();
                        //List<int> DynamicAttIds = new List<int>();
                        //List<bool> disables = new List<bool>();
                        //List<int?> sideArmLibIds = new List<int?>();
                        //List<int?> sideArmLibraryIds = new List<int?>();
                        //List<int> InventoryIds = new List<int>();
                        //List<int> tablesNamesIds = new List<int>();
                        //if (DynamicAtt.Item2 == "string")
                        //{
                        //    for (int k = 0; k < DynamicAtt.Item3.Count; k++)
                        //    {
                        //        if (!String.IsNullOrEmpty(DynamicAtt.Item3[k].ToString()))
                        //        {
                        //            StringValues.Add(DynamicAtt.Item3[k].ToString());
                        //            DynamicAttIds.Add(DynamicAtt.Item1);
                        //            disables.Add(false);
                        //            sideArmLibIds.Add(null);
                        //            sideArmLibraryIds.Add(null);
                        //            InventoryIds.Add(Convert.ToInt32(InsertedIds[k]));
                        //            tablesNamesIds.Add(TableNameEntity.Id);
                        //        }
                        //    }
                        //}
                        //else if (DynamicAtt.Item2 == "double")
                        //{
                        //    for (int k = 0; k < DynamicAtt.Item3.Count; k++)
                        //    {
                        //        if (!String.IsNullOrEmpty(DynamicAtt.Item3[k].ToString()))
                        //        {
                        //            DoubleValues.Add(Convert.ToInt32(DynamicAtt.Item3[k]));
                        //            DynamicAttIds.Add(DynamicAtt.Item1);
                        //            disables.Add(false);
                        //            sideArmLibIds.Add(null);
                        //            sideArmLibraryIds.Add(null);
                        //            InventoryIds.Add(Convert.ToInt32(InsertedIds[k]));
                        //            tablesNamesIds.Add(TableNameEntity.Id);
                        //        }
                        //    }
                        //}


                        //else if (DynamicAtt.Item2 == "datetime")
                        //{
                        //    for (int k = 0; k < DynamicAtt.Item3.Count; k++)
                        //    {
                        //        if (!String.IsNullOrEmpty(DynamicAtt.Item3[k].ToString()))
                        //        {
                        //            DateTimeValues.Add(Convert.ToDateTime(DynamicAtt.Item3[k]));
                        //            DynamicAttIds.Add(DynamicAtt.Item1);
                        //            disables.Add(false);
                        //            sideArmLibIds.Add(null);
                        //            sideArmLibraryIds.Add(null);
                        //            InventoryIds.Add(Convert.ToInt32(InsertedIds[k]));
                        //            tablesNamesIds.Add(TableNameEntity.Id);
                        //        }
                        //    }
                        //}
                        //else if (DynamicAtt.Item2 == "boolean")
                        //{
                        //    for (int k = 0; k < DynamicAtt.Item3.Count; k++)
                        //    {
                        //        if (!String.IsNullOrEmpty(DynamicAtt.Item3[k].ToString()))
                        //        {
                        //            bool BoolValue = Convert.ToBoolean(DynamicAtt.Item3[k]);
                        //            if (BoolValue == true)
                        //            {
                        //                BooleanValues.Add(1);
                        //            }
                        //            else
                        //            {
                        //                BooleanValues.Add(0);
                        //            }
                        //            DynamicAttIds.Add(DynamicAtt.Item1);
                        //            disables.Add(false);
                        //            sideArmLibIds.Add(null);
                        //            sideArmLibraryIds.Add(null);
                        //            InventoryIds.Add(Convert.ToInt32(InsertedIds[k]));
                        //            tablesNamesIds.Add(TableNameEntity.Id);
                        //        }
                        //    }
                        //}
                        ////connection.Open();
                        //OracleParameter value = new OracleParameter();
                        //if (DynamicAtt.Item2 == "string")
                        //{
                        //    value.OracleDbType = OracleDbType.NVarchar2;
                        //    value.Value = StringValues.ToArray();
                        //}
                        //else if (DynamicAtt.Item2 == "double")
                        //{
                        //    value.OracleDbType = OracleDbType.BinaryDouble;
                        //    value.Value = DoubleValues.ToArray();
                        //}
                        //else if (DynamicAtt.Item2 == "datetime")
                        //{
                        //    value.OracleDbType = OracleDbType.Date;
                        //    value.Value = DateTimeValues.ToArray();
                        //}
                        //else if (DynamicAtt.Item2 == "boolean")
                        //{
                        //    value.OracleDbType = OracleDbType.Int32;
                        //    value.Value = BooleanValues.ToArray();
                        //}
                        //OracleParameter DynamicAttId = new OracleParameter();
                        //DynamicAttId.OracleDbType = OracleDbType.Int32;
                        //DynamicAttId.Value = DynamicAttIds.ToArray();

                        //OracleParameter disable = new OracleParameter();
                        //disable.OracleDbType = OracleDbType.Boolean;
                        //disable.Value = disables.ToArray();

                        //OracleParameter sideArmLibId = new OracleParameter();
                        //sideArmLibId.OracleDbType = OracleDbType.Int32;
                        //sideArmLibId.Value = sideArmLibIds.ToArray();

                        //OracleParameter sideArmLibraryId = new OracleParameter();
                        //sideArmLibraryId.OracleDbType = OracleDbType.Int32;
                        //sideArmLibraryId.Value = sideArmLibraryIds.ToArray();

                        //OracleParameter InventoryId = new OracleParameter();
                        //InventoryId.OracleDbType = OracleDbType.Int32;
                        //InventoryId.Value = InventoryIds.ToArray();

                        //OracleParameter tablesNamesId = new OracleParameter();
                        //tablesNamesId.OracleDbType = OracleDbType.Int32;
                        //tablesNamesId.Value = tablesNamesIds.ToArray();

                        //OracleCommand dalvcmd = connection.CreateCommand();
                        //if (DynamicAtt.Item2 == "string")
                        //{
                        //    dalvcmd.CommandText = "INSERT INTO \"TLIdynamicAttLibValue\" (\"DynamicAttId\", \"InventoryId\", \"tablesNamesId\", \"ValueString\") VALUES ( :1, :2, :3, :4)";
                        //    dalvcmd.ArrayBindCount = StringValues.Count;
                        //}
                        //else if (DynamicAtt.Item2 == "double")
                        //{
                        //    dalvcmd.CommandText = "INSERT INTO \"TLIdynamicAttLibValue\" (\"DynamicAttId\", \"InventoryId\", \"tablesNamesId\", \"ValueDouble\") VALUES ( :1, :2, :3, :4)";
                        //    dalvcmd.ArrayBindCount = DoubleValues.Count;
                        //}
                        //else if (DynamicAtt.Item2 == "datetime")
                        //{
                        //    dalvcmd.CommandText = "INSERT INTO \"TLIdynamicAttLibValue\" (\"DynamicAttId\", \"InventoryId\", \"tablesNamesId\", \"ValueDateTime\") VALUES ( :1, :2, :3, :4)";
                        //    dalvcmd.ArrayBindCount = DateTimeValues.Count;
                        //}
                        //else if (DynamicAtt.Item2 == "boolean")
                        //{
                        //    dalvcmd.CommandText = "INSERT INTO \"TLIdynamicAttLibValue\" (\"DynamicAttId\", \"InventoryId\", \"tablesNamesId\", \"ValueBoolean\") VALUES ( :1, :2, :3, :4)";
                        //    dalvcmd.ArrayBindCount = BooleanValues.Count;
                        //}
                        ////dalvcmd.Parameters.Clear();
                        //dalvcmd.Parameters.Add(DynamicAttId);
                        //dalvcmd.Parameters.Add(InventoryId);
                        //dalvcmd.Parameters.Add(tablesNamesId);
                        //dalvcmd.Parameters.Add(value);
                        //dalvcmd.ExecuteNonQuery();

                    }

                    RecordId.AddRange(InsertedIds);
                }

            ERROR:;

            }
            catch (Exception err)
            {
                throw err;

            }

        }

        /* Done*/

        private void SaveCivilWithoutLegLibraryUsingOracleBulkCopy(int UserId, out List<int> RecordId, DataTable dt, ref List<KeyValuePair<int, string>> UnsavedRows, int ActColumns, int Columns, ExcelWorksheet sheet, List<KeyValuePair<string, List<DropDownListFilters>>> RelatedTables, TLItablesNames TableNameEntity, OracleConnection connection)
        {
            try
            {
                RecordId = new List<int>();
                List<string> Models = _dbContext.MV_CIVIL_WITHOUTLEG_LIBRARY_VIEW.Where(x => !x.Deleted).Select(x => x.Model).ToList();

                TLIcivilWithoutLegLibrary LastId = _serviceProvider.GetService<ApplicationDbContext>().
                    TLIcivilWithoutLegLibrary.OrderByDescending(a => a.Id).FirstOrDefault();

                List<TLIdynamicAtt> CivilWithoutLegLibraryDynamicAtts = _unitOfWork.DynamicAttRepository.GetIncludeWhere(x =>
                    x.tablesNamesId == TableNameEntity.Id && !x.disable, x => x.DataType).ToList();

                List<string> models = new List<string>();
                List<string> notes = new List<string>();
                List<string> WidthVariations = new List<string>();
                List<float> HeightsDesigned = new List<float>();
                List<float> maxloads = new List<float>();
                List<float> max_man_loads = new List<float>();
                List<float> SpaceLibraries = new List<float>();
                List<int?> CivilSteelSupportCategoryIds = new List<int?>();
                List<int?> InstCivilwithoutLegsTypeIds = new List<int?>();
                List<int> CivilWithoutLegCategoryIds = new List<int>();
                List<float> HeightBases = new List<float>();
                List<string> Prefixes = new List<string>();
                List<int> structureTypeIds = new List<int>();
                List<Tuple<int, string, List<dynamic>>> DynamicAtts = new List<Tuple<int, string, List<dynamic>>>();
                TLIdynamicAtt DA = new TLIdynamicAtt();
                string ColName = null;
                //List<string> test = new List<string>();
                if (ActColumns > 0)
                {
                    for (int i = ActColumns; i <= Columns; i++)
                    {
                        ColName = sheet.Cells[1, i].Value.ToString();
                        DA = CivilWithoutLegLibraryDynamicAtts.FirstOrDefault(x => x.Key == ColName);
                        if (DA != null)
                            DynamicAtts.Add(new Tuple<int, string, List<dynamic>>(DA.Id, DA.DataType.Name.ToLower(), new List<dynamic>()));
                    }
                }
                bool test;
                for (int j = 0; j < dt.Rows.Count; j++)
                {
                    try
                    {

                        string note_test = null;
                        if (dt.Columns.Contains("Note"))
                        {
                            if (!String.IsNullOrEmpty(dt.Rows[j]["Note"].ToString()))
                            {
                                note_test = Convert.ToString(dt.Rows[j]["Note"]);
                            }
                        }
                        string WidthVariation_test = null;
                        if (dt.Columns.Contains("WidthVariation"))
                        {
                            if (!String.IsNullOrEmpty(dt.Rows[j]["WidthVariation"].ToString()))
                            {
                                WidthVariation_test = Convert.ToString(dt.Rows[j]["WidthVariation"]);
                            }
                        }

                        float heightdesigned_test = 0;
                        if (dt.Columns.Contains("Height_Designed"))
                        {
                            if (!String.IsNullOrEmpty(dt.Rows[j]["Height_Designed"].ToString()))
                            {
                                float heightdesigned_test_1;
                                test = float.TryParse(dt.Rows[j]["Height_Designed"].ToString(), out heightdesigned_test_1);
                                if (test == true)
                                {
                                    heightdesigned_test = (float)Math.Round(heightdesigned_test_1, 1, MidpointRounding.ToEven);
                                }
                                else
                                {
                                    UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"Height_Designed Wrong Input DataType in the row {j + 2}"));
                                    goto ERROR;
                                }
                            }
                            else
                            {
                                UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"Height_Designed can not to be null must input number in the row  {j + 2}"));
                                goto ERROR;
                            }
                        }
                        float maxloadm2_test = 0;
                        if (dt.Columns.Contains("Max_Load"))
                        {
                            test = float.TryParse(dt.Rows[j]["Max_Load"].ToString(), out maxloadm2_test);
                            if (test == false)
                            {
                                UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"Max_Load Wrong Input DataType in the row {j + 2}"));
                            }
                        }

                        float max_man_load_test = 0;
                        if (dt.Columns.Contains("Manufactured_Max_Load"))
                        {
                            test = float.TryParse(dt.Rows[j]["Manufactured_Max_Load"].ToString(), out max_man_load_test);
                            if (test == false)
                            {
                                UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"Manufactured_Max_Load Wrong Input DataType in the row {j + 2}"));
                            }
                        }

                        float spacelibrary_test = 0;
                        if (dt.Columns.Contains("SpaceLibrary"))
                        {
                            if (!String.IsNullOrEmpty(dt.Rows[j]["SpaceLibrary"].ToString()))
                            {
                                test = float.TryParse(dt.Rows[j]["SpaceLibrary"].ToString(), out spacelibrary_test);
                                if (test == false)
                                {
                                    UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"SpaceLibrary Wrong Input DataType in the row {j + 2}"));
                                    goto ERROR;
                                }
                                if (test == true && spacelibrary_test == 0)
                                {
                                    UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"SpaceLibrary must to be bigger of zero in thr row {j + 2}"));
                                    goto ERROR;
                                }
                            }
                            else
                            {
                                UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"SpaceLibrary can not to be null in the row must to be bigger of zero {j + 2}"));
                                goto ERROR;
                            }
                        }

                        int? CivilSteelSupportCategoryId_test = null;
                        if (dt.Columns.Contains("CivilSteelSupportCategoryId"))
                        {
                            if (!String.IsNullOrEmpty(dt.Rows[j]["CivilSteelSupportCategoryId"].ToString()))
                            {
                                DropDownListFilters CivilSteelSupportCategory = RelatedTables.FirstOrDefault(x =>
                                    x.Key == "CivilSteelSupportCategoryId").Value.FirstOrDefault(x =>
                                        x.Value == dt.Rows[j]["CivilSteelSupportCategoryId"].ToString());
                                if (CivilSteelSupportCategory != null)
                                {
                                    CivilSteelSupportCategoryId_test = Convert.ToInt32(CivilSteelSupportCategory.Id);
                                }
                            }
                        }
                        int? InstCivilwithoutLegsTypeId_test = null;

                        if (dt.Columns.Contains("InstCivilwithoutLegsTypeId"))
                        {
                            DropDownListFilters InstCivilwithoutLegsType = RelatedTables.FirstOrDefault(x =>
                                                        x.Key == "InstCivilwithoutLegsTypeId").Value.FirstOrDefault(x =>
                                                            x.Value == dt.Rows[j]["InstCivilwithoutLegsTypeId"].ToString());


                            if (InstCivilwithoutLegsType != null)
                            {
                                InstCivilwithoutLegsTypeId_test = Convert.ToInt32(InstCivilwithoutLegsType.Id);
                            }

                        }

                        int structureTypeId_test = 0;
                        string structuretypeName_test = null;
                        if (dt.Columns.Contains("structureTypeId"))
                        {
                            DropDownListFilters structureType = RelatedTables.FirstOrDefault(x =>
                            x.Key == "structureTypeId").Value.FirstOrDefault(x =>
                                x.Value == dt.Rows[j]["structureTypeId"].ToString());
                            if (structureType != null)
                            {
                                structureTypeId_test = Convert.ToInt32(structureType.Id);
                                structuretypeName_test = structureType.Value;
                            }
                            else
                            {
                                UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"structureTypeId can not to be null in the row {j + 2}"));
                                goto ERROR;
                            }
                        }

                        int CivilWithoutLegCategoryId_test = 0;
                        string CivilCategoryName_test = null;
                        if (dt.Columns.Contains("CivilWithoutLegCategoryId"))
                        {
                            if (!String.IsNullOrEmpty(dt.Rows[j]["CivilWithoutLegCategoryId"].ToString()))
                            {
                                DropDownListFilters CivilWithoutLegCategory = RelatedTables.FirstOrDefault(x =>
                                    x.Key == "CivilWithoutLegCategoryId").Value.FirstOrDefault(x =>
                                        x.Value == dt.Rows[j]["CivilWithoutLegCategoryId"].ToString());
                                if (CivilWithoutLegCategory != null)
                                    CivilWithoutLegCategoryId_test = Convert.ToInt32(CivilWithoutLegCategory.Id);
                                CivilCategoryName_test = CivilWithoutLegCategory.Value;
                            }
                            else
                            {
                                UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"CivilWithoutLegCategoryId can not to be null in the row{j + 2}"));
                                goto ERROR;
                            }
                        }

                        float HeightBase_test = 0;
                        if (dt.Columns.Contains("HeightBase"))
                        {
                            test = float.TryParse(dt.Rows[j]["HeightBase"].ToString(), out HeightBase_test);
                            if (test == false)
                            {
                                UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"HeightBase Wrong Input Value in the row {j + 2}"));
                            }
                        }

                        string Prefix_test = null;
                        if (dt.Columns.Contains("Prefix"))
                        {
                            if (!String.IsNullOrEmpty(dt.Rows[j]["Prefix"].ToString()))
                            {
                                Prefix_test = Convert.ToString(dt.Rows[j]["Prefix"]);
                            }
                        }

                        string model_test = null;
                        if (dt.Columns.Contains("Model"))
                        {
                            if (!string.IsNullOrEmpty(dt.Columns.Contains("Vendors").ToString()))
                            {
                                DropDownListFilters Vendortest = RelatedTables.FirstOrDefault(x =>
                                                      x.Key == "Vendors").Value.FirstOrDefault(x => x.Value == dt.Rows[j]["Vendors"].ToString());
                                if (Vendortest != null)
                                {
                                    model_test = CivilCategoryName_test + ' ' + Vendortest.Value + ' ' + Prefix_test + ' ' + structuretypeName_test + ' ' + heightdesigned_test + "HE"; ;
                                    if ((Models.Find(x => x == Convert.ToString(model_test))) != null)
                                    {
                                        UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"Model is already exist in the row {j + 2}"));
                                        goto ERROR;
                                    }
                                }
                            }
                            else
                            {
                                UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"Vendors can not to be null in the row {j + 2}"));
                                goto ERROR;
                            }

                        }
                        List<dynamic> DynamicAttList = new List<dynamic>();
                        if (ActColumns > 0)
                        {
                            for (int i = ActColumns; i <= Columns; i++)
                            {
                                string StringDynamicAtt = string.Empty;
                                double? DoubleDynamicAtt = null;
                                DateTime? DateTimeDynamicAtt = null;
                                Boolean? BooleanDynamicAtt = null;
                                ColName = sheet.Cells[1, i].Value.ToString();
                                DA = CivilWithoutLegLibraryDynamicAtts.FirstOrDefault(x => x.Key == ColName);
                                //Check dynamic attribute if required or not
                                if (DA != null)
                                {
                                    double double_Test = 0;
                                    DateTime datetime_Test = DateTime.Now;
                                    Boolean boolean_Test = false;
                                    if (DA.Required == true)
                                    {
                                        //if somthing wrong then add row to UnsavedRows
                                        if (String.IsNullOrEmpty(dt.Rows[j][ColName].ToString()))
                                        {
                                            UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"Dynamic Attribute ({ColName}) is required"));
                                            goto ERROR;
                                        }
                                        else
                                        {
                                            if (DA.DataType.Name.ToLower() == "string")
                                            {

                                                StringDynamicAtt = Convert.ToString(dt.Rows[j][ColName]);
                                            }
                                            else if (DA.DataType.Name.ToLower() == "double" || DA.DataType.Name.ToLower() == "int")
                                            {
                                                test = double.TryParse(dt.Rows[j][ColName].ToString(), out double_Test);
                                                if (test == false)
                                                {
                                                    UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"{ColName} Dynamic Attribute Wrong Input DataType in the row {j + 2}"));
                                                    goto ERROR;
                                                }
                                                DoubleDynamicAtt = double_Test;
                                            }
                                            else if (DA.DataType.Name.ToLower() == "datetime")
                                            {
                                                test = DateTime.TryParse(dt.Rows[j][ColName].ToString(), out datetime_Test);
                                                if (test == false)
                                                {
                                                    UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"{ColName} Dynamic Attribute Wrong Input DataType in the row {j + 2}"));
                                                    goto ERROR;
                                                }
                                                DateTimeDynamicAtt = datetime_Test;
                                            }
                                            else if (DA.DataType.Name.ToLower() == "bool")
                                            {
                                                test = Boolean.TryParse(dt.Rows[j][ColName].ToString(), out boolean_Test);
                                                if (test == false)
                                                {
                                                    UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"{ColName} Dynamic Attribute Wrong Input DataType in the row {j + 2}"));
                                                    goto ERROR;
                                                }
                                                BooleanDynamicAtt = boolean_Test;
                                            }


                                        }
                                    }
                                    else
                                    {
                                        if (!String.IsNullOrEmpty(dt.Rows[j][ColName].ToString()))
                                        {
                                            if (DA.DataType.Name.ToLower() == "string")
                                            {
                                                StringDynamicAtt = Convert.ToString(dt.Rows[j][ColName]);
                                            }
                                            else if (DA.DataType.Name.ToLower() == "double" || DA.DataType.Name.ToLower() == "int")
                                            {
                                                test = double.TryParse(dt.Rows[j][ColName].ToString(), out double_Test);
                                                if (test == false)
                                                {
                                                    UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"{ColName} Dynamic Attribute Wrong Input DataType in the row {j + 2}"));
                                                    goto ERROR;
                                                }
                                                DoubleDynamicAtt = double_Test;
                                            }
                                            else if (DA.DataType.Name.ToLower() == "datetime")
                                            {
                                                test = DateTime.TryParse(dt.Rows[j][ColName].ToString(), out datetime_Test);
                                                if (test == false)
                                                {
                                                    UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"{ColName} Dynamic Attribute Wrong Input DataType in the row {j + 2}"));
                                                    goto ERROR;
                                                }
                                                DateTimeDynamicAtt = datetime_Test;

                                            }
                                            else if (DA.DataType.Name.ToLower() == "bool")
                                            {
                                                test = Boolean.TryParse(dt.Rows[j][ColName].ToString(), out boolean_Test);
                                                if (test == false)
                                                {
                                                    UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"{ColName} Dynamic Attribute Wrong Input DataType in the row {j + 2}"));
                                                    goto ERROR;
                                                }
                                                BooleanDynamicAtt = boolean_Test;
                                            }

                                        }
                                    }
                                    if (DA.DataType.Name.ToLower() == "string")
                                    {
                                        DynamicAttList.Add(StringDynamicAtt);
                                    }
                                    else if (DA.DataType.Name.ToLower() == "double" || DA.DataType.Name.ToLower() == "int")
                                    {
                                        DynamicAttList.Add(DoubleDynamicAtt);
                                    }
                                    else if (DA.DataType.Name.ToLower() == "datetime")
                                    {
                                        DynamicAttList.Add(DateTimeDynamicAtt);
                                    }
                                    else if (DA.DataType.Name.ToLower() == "bool")
                                    {
                                        DynamicAttList.Add(BooleanDynamicAtt);
                                    }


                                }
                            }
                        }
                        models.Add(model_test != null ? model_test : null);
                        notes.Add(note_test != null ? model_test : null);
                        WidthVariations.Add(WidthVariation_test != null ? WidthVariation_test : null);
                        HeightsDesigned.Add(heightdesigned_test != 0 ? heightdesigned_test : 0);
                        maxloads.Add(maxloadm2_test != 0 ? maxloadm2_test : 0);
                        SpaceLibraries.Add(spacelibrary_test != 0 ? spacelibrary_test : 0);
                        HeightBases.Add(HeightBase_test != 0 ? HeightBase_test : 0);
                        Prefixes.Add(Prefix_test != null ? Prefix_test : null);
                        max_man_loads.Add(max_man_load_test != 0 ? max_man_load_test : 0);
                        structureTypeIds.Add(structureTypeId_test != 0 ? structureTypeId_test : 0);

                        CivilSteelSupportCategoryIds.Add(CivilSteelSupportCategoryId_test != 0 ? CivilSteelSupportCategoryId_test : null);
                        InstCivilwithoutLegsTypeIds.Add(InstCivilwithoutLegsTypeId_test != 0 ? InstCivilwithoutLegsTypeId_test : 0);
                        CivilWithoutLegCategoryIds.Add(CivilWithoutLegCategoryId_test != 0 ? CivilWithoutLegCategoryId_test : 0);
                        for (int f = 0; f < DynamicAttList.Count; f++)
                        {
                            //var h = DynamicAtts[f].Value;
                            DynamicAtts[f].Item3.Add(DynamicAttList[f]);
                            //h.Add(DynamicAttList[f]);
                        }


                    }
                    catch (Exception err)
                    {

                        //UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, "Wrong Input DataType"));
                    }
                }
                if (models.Count > 0)
                {
                    OracleParameter model = new OracleParameter();
                    model.OracleDbType = OracleDbType.NVarchar2;
                    model.Value = models.ToArray();

                    OracleParameter Prefix = new OracleParameter();
                    Prefix.OracleDbType = OracleDbType.NVarchar2;
                    Prefix.Value = Prefixes.ToArray();

                    OracleParameter note = new OracleParameter();
                    note.OracleDbType = OracleDbType.NVarchar2;
                    note.Value = notes.ToArray();

                    OracleParameter HeightDesigned = new OracleParameter();
                    HeightDesigned.OracleDbType = OracleDbType.BinaryFloat;
                    HeightDesigned.Value = HeightsDesigned.ToArray();

                    OracleParameter maxload = new OracleParameter();
                    maxload.OracleDbType = OracleDbType.BinaryFloat;
                    maxload.Value = maxloads.ToArray();

                    OracleParameter max_man_load = new OracleParameter();
                    max_man_load.OracleDbType = OracleDbType.BinaryFloat;
                    max_man_load.Value = max_man_loads.ToArray();

                    OracleParameter SpaceLibrary = new OracleParameter();
                    SpaceLibrary.OracleDbType = OracleDbType.BinaryFloat;
                    SpaceLibrary.Value = SpaceLibraries.ToArray();

                    OracleParameter HeightBase = new OracleParameter();
                    HeightBase.OracleDbType = OracleDbType.BinaryFloat;
                    HeightBase.Value = HeightBases.ToArray();

                    OracleParameter civilSteelSupportCategoryId = new OracleParameter();
                    civilSteelSupportCategoryId.OracleDbType = OracleDbType.Int32;
                    civilSteelSupportCategoryId.Value = CivilSteelSupportCategoryIds.ToArray();

                    OracleParameter InstCivilwithoutLegsTypeId = new OracleParameter();
                    InstCivilwithoutLegsTypeId.OracleDbType = OracleDbType.Int32;
                    InstCivilwithoutLegsTypeId.Value = InstCivilwithoutLegsTypeIds.ToArray();

                    OracleParameter structureTypeId = new OracleParameter();
                    structureTypeId.OracleDbType = OracleDbType.Int32;
                    structureTypeId.Value = structureTypeIds.ToArray();

                    OracleParameter civilWithoutLegCategoryId = new OracleParameter();
                    civilWithoutLegCategoryId.OracleDbType = OracleDbType.Int32;
                    civilWithoutLegCategoryId.Value = CivilWithoutLegCategoryIds.ToArray();

                    OracleParameter WidthVariation = new OracleParameter();
                    WidthVariation.OracleDbType = OracleDbType.NVarchar2;
                    WidthVariation.Value = WidthVariations.ToArray();

                    // create command and set properties
                    OracleCommand cmd = connection.CreateCommand();
                    cmd.CommandText = "INSERT INTO \"TLIcivilWithoutLegLibrary\" (\"Model\", \"Note\", \"Height_Designed\", \"Max_Load\", \"SpaceLibrary\", \"HeightBase\", \"Prefix\", \"structureTypeId\", \"CivilSteelSupportCategoryId\", \"InstCivilwithoutLegsTypeId\", \"CivilWithoutLegCategoryId\",\"Manufactured_Max_Load\",\"WidthVariation\") VALUES ( :1, :2, :3, :4, :5, :6, :7, :8, :9, :10, :11,:12 ,:13)";
                    cmd.ArrayBindCount = models.Count;
                    cmd.Parameters.Add(model);
                    cmd.Parameters.Add(note);
                    cmd.Parameters.Add(HeightDesigned);
                    cmd.Parameters.Add(maxload);
                    cmd.Parameters.Add(SpaceLibrary);
                    cmd.Parameters.Add(HeightBase);
                    cmd.Parameters.Add(Prefix);
                    cmd.Parameters.Add(structureTypeId);
                    cmd.Parameters.Add(civilSteelSupportCategoryId);
                    cmd.Parameters.Add(InstCivilwithoutLegsTypeId);
                    cmd.Parameters.Add(civilWithoutLegCategoryId);
                    cmd.Parameters.Add(max_man_load);
                    cmd.Parameters.Add(WidthVariation);

                    cmd.ExecuteNonQuery();
                    //connection.Close();
                    List<int> InsertedIds = new List<int>();
                    if (LastId != null)
                    {
                        OracleCommand InventoryCmd = connection.CreateCommand();
                        InventoryCmd.CommandType = CommandType.Text;
                        InventoryCmd.CommandText = $"Select \"Id\" From \"TLIcivilWithoutLegLibrary\" where \"TLIcivilWithoutLegLibrary\".\"Id\" > {LastId.Id}";
                        OracleDataReader Reader = InventoryCmd.ExecuteReader();
                        if (Reader.HasRows)
                        {
                            while (Reader.Read())
                            {
                                InsertedIds.Add(Reader.GetInt32(0));
                            }
                        }
                    }
                    else
                    {
                        OracleCommand InventoryCmd = connection.CreateCommand();
                        InventoryCmd.CommandType = CommandType.Text;
                        InventoryCmd.CommandText = $"Select \"Id\" From \"TLIcivilWithoutLegLibrary\"";
                        OracleDataReader Reader = InventoryCmd.ExecuteReader();
                        if (Reader.HasRows)
                        {
                            while (Reader.Read())
                            {
                                InsertedIds.Add(Reader.GetInt32(0));
                            }
                        }
                    }
                    TLIhistory tLIhistory = new TLIhistory()
                    {
                        UserId = UserId,
                        TablesNameId = TableNameEntity.Id,
                        HistoryTypeId = 1,

                    };
                    _dbContext.TLIhistory.Add(tLIhistory);
                    _dbContext.SaveChanges();
                    var civilWithoutLegLibraryist = new List<TLIcivilWithoutLegLibrary>();

                    for (int i = 0; i < models.Count; i++)
                    {
                        civilWithoutLegLibraryist.Add(new TLIcivilWithoutLegLibrary
                        {
                            Id = InsertedIds[i],
                            Model = models[i],
                            Note = notes[i],
                            Height_Designed = HeightsDesigned[i],
                            Max_Load = maxloads[i],
                            SpaceLibrary = SpaceLibraries[i],
                            HeightBase = HeightBases[i],
                            Prefix = Prefixes[i],
                            structureTypeId = structureTypeIds[i],
                            CivilSteelSupportCategoryId = CivilSteelSupportCategoryIds[i],
                            InstCivilwithoutLegsTypeId = InstCivilwithoutLegsTypeIds[i],
                            CivilWithoutLegCategoryId = CivilWithoutLegCategoryIds[i],
                            Manufactured_Max_Load = max_man_loads[i],
                            WidthVariation = WidthVariations[i],
                        });
                    }


                    foreach (int recordId in InsertedIds)
                    {
                        var RecordName = civilWithoutLegLibraryist.FirstOrDefault(x => x.Id == recordId);
                        if (RecordName != null)
                        {
                            var attributeNames = RecordName.GetType().GetProperties()
                                .Where(x =>
                                    (x.PropertyType.IsGenericType && x.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>) &&
                                        new Type[] { typeof(int), typeof(string), typeof(double), typeof(float), typeof(Single), typeof(bool), typeof(DateTime) }
                                        .Contains(x.PropertyType.GetGenericArguments()[0]))
                                    || new Type[] { typeof(int), typeof(string), typeof(double), typeof(bool), typeof(DateTime), typeof(float), typeof(Single) }
                                    .Contains(x.PropertyType)
                                )
                                .ToList();

                            foreach (var propertyInfo in attributeNames)
                            {

                                var propertyName = propertyInfo.Name;

                                var propertyValue = propertyInfo.GetValue(RecordName)?.ToString().Trim();

                                TLIhistoryDet tLIhistoryDet = new TLIhistoryDet()
                                {
                                    HistoryId = tLIhistory.Id,
                                    TablesNameId = TableNameEntity.Id,
                                    RecordId = recordId.ToString(),
                                    AttributeName = propertyName,
                                    NewValue = propertyValue
                                };


                                _dbContext.TLIhistoryDet.Add(tLIhistoryDet);
                            }
                        }
                    }
                    _dbContext.SaveChanges();

                    foreach (var DynamicAtt in DynamicAtts)
                    {
                        for (int k = 0; k < DynamicAtt.Item3.Count; k++)
                        {
                            if (!String.IsNullOrEmpty(DynamicAtt.Item3[k].ToString()))
                            {

                                var Message = _unitOfWork.CivilWithLegsRepository.CheckDynamicValidationAndDependenceDynamic(DynamicAtt.Item1, DynamicAtt.Item3[k], Convert.ToInt32(InsertedIds[k]), tLIhistory.Id, civilWithoutLegLibraryist[0]).Message;
                                if (Message != "Success")
                                {
                                    UnsavedRows.Add(new KeyValuePair<int, string>(k + 2, Message + ' ' + $"in the Colum Name{ColName} in the Row Number {k + 2} "));
                                    goto ERROR;
                                }

                            }

                        }
                        //List<string> StringValues = new List<string>();
                        //List<double> DoubleValues = new List<double>();
                        //List<DateTime> DateTimeValues = new List<DateTime>();
                        //List<Int32> BooleanValues = new List<Int32>();
                        //List<int> DynamicAttIds = new List<int>();
                        //List<bool> disables = new List<bool>();
                        //List<int?> sideArmLibIds = new List<int?>();
                        //List<int?> sideArmLibraryIds = new List<int?>();
                        //List<int> InventoryIds = new List<int>();
                        //List<int> tablesNamesIds = new List<int>();
                        //if (DynamicAtt.Item2 == "string")
                        //{
                        //    for (int k = 0; k < DynamicAtt.Item3.Count; k++)
                        //    {
                        //        if (!String.IsNullOrEmpty(DynamicAtt.Item3[k].ToString()))
                        //        {
                        //            StringValues.Add(DynamicAtt.Item3[k].ToString());
                        //            DynamicAttIds.Add(DynamicAtt.Item1);
                        //            disables.Add(false);
                        //            sideArmLibIds.Add(null);
                        //            sideArmLibraryIds.Add(null);
                        //            InventoryIds.Add(Convert.ToInt32(InsertedIds[k]));
                        //            tablesNamesIds.Add(TableNameEntity.Id);
                        //        }
                        //    }
                        //}
                        //if (DynamicAtt.Item2 == "double")
                        //{
                        //    for (int k = 0; k < DynamicAtt.Item3.Count; k++)
                        //    {
                        //        if (!String.IsNullOrEmpty(DynamicAtt.Item3[k].ToString()))
                        //        {
                        //            DoubleValues.Add(Convert.ToInt32(DynamicAtt.Item3[k]));
                        //            DynamicAttIds.Add(DynamicAtt.Item1);
                        //            disables.Add(false);
                        //            sideArmLibIds.Add(null);
                        //            sideArmLibraryIds.Add(null);
                        //            InventoryIds.Add(Convert.ToInt32(InsertedIds[k]));
                        //            tablesNamesIds.Add(TableNameEntity.Id);
                        //        }
                        //    }
                        //}
                        //if (DynamicAtt.Item2 == "datetime")
                        //{
                        //    for (int k = 0; k < DynamicAtt.Item3.Count; k++)
                        //    {
                        //        if (!String.IsNullOrEmpty(DynamicAtt.Item3[k].ToString()))
                        //        {
                        //            DateTimeValues.Add(Convert.ToDateTime(DynamicAtt.Item3[k]));
                        //            DynamicAttIds.Add(DynamicAtt.Item1);
                        //            disables.Add(false);
                        //            sideArmLibIds.Add(null);
                        //            sideArmLibraryIds.Add(null);
                        //            InventoryIds.Add(Convert.ToInt32(InsertedIds[k]));
                        //            tablesNamesIds.Add(TableNameEntity.Id);
                        //        }
                        //    }
                        //}
                        //if (DynamicAtt.Item2 == "boolean")
                        //{
                        //    for (int k = 0; k < DynamicAtt.Item3.Count; k++)
                        //    {
                        //        if (!String.IsNullOrEmpty(DynamicAtt.Item3[k].ToString()))
                        //        {
                        //            bool BoolValue = Convert.ToBoolean(DynamicAtt.Item3[k]);
                        //            if (BoolValue == true)
                        //            {
                        //                BooleanValues.Add(1);
                        //            }
                        //            else
                        //            {
                        //                BooleanValues.Add(0);
                        //            }
                        //            DynamicAttIds.Add(DynamicAtt.Item1);
                        //            disables.Add(false);
                        //            sideArmLibIds.Add(null);
                        //            sideArmLibraryIds.Add(null);
                        //            InventoryIds.Add(Convert.ToInt32(InsertedIds[k]));
                        //            tablesNamesIds.Add(TableNameEntity.Id);
                        //        }
                        //    }
                        //}
                        ////connection.Open();
                        //OracleParameter value = new OracleParameter();
                        //if (DynamicAtt.Item2 == "string")
                        //{
                        //    value.OracleDbType = OracleDbType.NVarchar2;
                        //    value.Value = StringValues.ToArray();
                        //}
                        //else if (DynamicAtt.Item2 == "double")
                        //{
                        //    value.OracleDbType = OracleDbType.BinaryDouble;
                        //    value.Value = DoubleValues.ToArray();
                        //}
                        //else if (DynamicAtt.Item2 == "datetime")
                        //{
                        //    value.OracleDbType = OracleDbType.Date;
                        //    value.Value = DateTimeValues.ToArray();
                        //}
                        //else if (DynamicAtt.Item2 == "boolean")
                        //{
                        //    value.OracleDbType = OracleDbType.Int32;
                        //    value.Value = BooleanValues.ToArray();
                        //}
                        //OracleParameter DynamicAttId = new OracleParameter();
                        //DynamicAttId.OracleDbType = OracleDbType.Int32;
                        //DynamicAttId.Value = DynamicAttIds.ToArray();

                        //OracleParameter disable = new OracleParameter();
                        //disable.OracleDbType = OracleDbType.Boolean;
                        //disable.Value = disables.ToArray();

                        //OracleParameter sideArmLibId = new OracleParameter();
                        //sideArmLibId.OracleDbType = OracleDbType.Int32;
                        //sideArmLibId.Value = sideArmLibIds.ToArray();

                        //OracleParameter sideArmLibraryId = new OracleParameter();
                        //sideArmLibraryId.OracleDbType = OracleDbType.Int32;
                        //sideArmLibraryId.Value = sideArmLibraryIds.ToArray();

                        //OracleParameter InventoryId = new OracleParameter();
                        //InventoryId.OracleDbType = OracleDbType.Int32;
                        //InventoryId.Value = InventoryIds.ToArray();

                        //OracleParameter tablesNamesId = new OracleParameter();
                        //tablesNamesId.OracleDbType = OracleDbType.Int32;
                        //tablesNamesId.Value = tablesNamesIds.ToArray();

                        //OracleCommand dalvcmd = connection.CreateCommand();
                        //if (DynamicAtt.Item2 == "string")
                        //{
                        //    dalvcmd.CommandText = "INSERT INTO \"TLIdynamicAttLibValue\" (\"DynamicAttId\", \"InventoryId\", \"tablesNamesId\", \"ValueString\") VALUES ( :1, :2, :3, :4)";
                        //    dalvcmd.ArrayBindCount = StringValues.Count;
                        //}
                        //else if (DynamicAtt.Item2 == "double")
                        //{
                        //    dalvcmd.CommandText = "INSERT INTO \"TLIdynamicAttLibValue\" (\"DynamicAttId\", \"InventoryId\", \"tablesNamesId\", \"ValueDouble\") VALUES ( :1, :2, :3, :4)";
                        //    dalvcmd.ArrayBindCount = DoubleValues.Count;
                        //}
                        //else if (DynamicAtt.Item2 == "datetime")
                        //{
                        //    dalvcmd.CommandText = "INSERT INTO \"TLIdynamicAttLibValue\" (\"DynamicAttId\", \"InventoryId\", \"tablesNamesId\", \"ValueDateTime\") VALUES ( :1, :2, :3, :4)";
                        //    dalvcmd.ArrayBindCount = DateTimeValues.Count;
                        //}
                        //else if (DynamicAtt.Item2 == "boolean")
                        //{
                        //    dalvcmd.CommandText = "INSERT INTO \"TLIdynamicAttLibValue\" (\"DynamicAttId\", \"InventoryId\", \"tablesNamesId\", \"ValueBoolean\") VALUES ( :1, :2, :3, :4)";
                        //    dalvcmd.ArrayBindCount = BooleanValues.Count;
                        //}
                        ////dalvcmd.Parameters.Clear();
                        //dalvcmd.Parameters.Add(DynamicAttId);
                        //dalvcmd.Parameters.Add(InventoryId);
                        //dalvcmd.Parameters.Add(tablesNamesId);
                        //dalvcmd.Parameters.Add(value);
                        //dalvcmd.ExecuteNonQuery();
                    }
                    RecordId.AddRange(InsertedIds);
                }
            ERROR:;
            }
            catch (Exception err)
            {

                throw err;
            }
        }

        /* Done*/
        private void SaveCivilNonSteelLibraryUsingOracleBulkCopy(int UserId, out List<int> RecordId, DataTable dt, ref List<KeyValuePair<int, string>> UnsavedRows, int ActColumns, int Columns, ExcelWorksheet sheet, List<KeyValuePair<string, List<DropDownListFilters>>> RelatedTables, TLItablesNames TableNameEntity, OracleConnection connection)
        {
            try
            {
                RecordId = new List<int>();
                List<string> Models = _dbContext.MV_CIVIL_NONSTEEL_LIBRARY_VIEW.Where(x => !x.Deleted).Select(x => x.Model).ToList();

                TLIcivilNonSteelLibrary LastId = _serviceProvider.GetService<ApplicationDbContext>().
                    TLIcivilNonSteelLibrary.OrderByDescending(a => a.Id).FirstOrDefault();

                List<TLIdynamicAtt> CivilNonSteelLibraryDynamicAtts = _unitOfWork.DynamicAttRepository.GetIncludeWhere(x =>
                    x.tablesNamesId == TableNameEntity.Id && !x.disable, x => x.DataType).ToList();

                List<string> models = new List<string>();
                List<string> Prefixs = new List<string>();
                List<string> notes = new List<string>();
                List<string> WidthVariations = new List<string>();
                List<float> Heights = new List<float>();
                List<float> SpaceLibraries = new List<float>();
                List<float> Manufactured_Max_Loads = new List<float>();
                List<Boolean> VerticalMeasuredList = new List<Boolean>();
                List<Int32> civilNonSteelTypeIds = new List<Int32>();
                List<float> NumberofBoltHolesList = new List<float>();
                List<Tuple<int, string, List<dynamic>>> DynamicAtts = new List<Tuple<int, string, List<dynamic>>>();
                TLIdynamicAtt DA = new TLIdynamicAtt();
                string ColName = null;
                //List<string> test = new List<string>();
                if (ActColumns > 0)
                {
                    for (int i = ActColumns; i <= Columns; i++)
                    {
                        ColName = sheet.Cells[1, i].Value.ToString();
                        DA = CivilNonSteelLibraryDynamicAtts.FirstOrDefault(x => x.Key == ColName);
                        if (DA != null)
                            DynamicAtts.Add(new Tuple<int, string, List<dynamic>>(DA.Id, DA.DataType.Name.ToLower(), new List<dynamic>()));
                    }
                }
                bool test;
                for (int j = 0; j < dt.Rows.Count; j++)
                {
                    try
                    {
                        string model_test = null;

                        if (dt.Columns.Contains("Model"))
                        {
                            if ((Models.Find(x => x == Convert.ToString(dt.Rows[j]["Model"]))) != null)
                            {
                                UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"Model is already exist in the row {j + 2}"));
                                goto ERROR;

                            }
                            if (!String.IsNullOrEmpty(dt.Rows[j]["Model"].ToString()))
                            {
                                model_test = Convert.ToString(dt.Rows[j]["Model"]);
                            }
                            else
                            {
                                UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"Model can not to be null in the row {j + 2}"));
                                goto ERROR;
                            }
                        }

                        string note_test = null;
                        if (dt.Columns.Contains("Note"))
                        {
                            if (!String.IsNullOrEmpty(dt.Rows[j]["Note"].ToString()))
                            {
                                note_test = Convert.ToString(dt.Rows[j]["Note"]);
                            }
                        }

                        string Prefix_test = null;
                        if (dt.Columns.Contains("Prefix"))
                        {
                            if (!String.IsNullOrEmpty(dt.Rows[j]["Prefix"].ToString()))
                            {
                                Prefix_test = Convert.ToString(dt.Rows[j]["Prefix"]);
                            }
                        }
                        string WidthVariation_test = null;
                        if (dt.Columns.Contains("WidthVariation"))
                        {
                            if (!String.IsNullOrEmpty(dt.Rows[j]["WidthVariation"].ToString()))
                            {
                                WidthVariation_test = Convert.ToString(dt.Rows[j]["WidthVariation"]);
                            }
                        }

                        float height_test = 0;
                        if (dt.Columns.Contains("Hight"))
                        {
                            test = float.TryParse(dt.Rows[j]["Hight"].ToString(), out height_test);
                            if (test == false)
                            {
                                UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"Hight Wrong Input DataType in the row {j + 2}"));
                                goto ERROR;
                            }
                        }

                        float Manufactured_Max_Load_test = 0;
                        if (dt.Columns.Contains("Manufactured_Max_Load"))
                        {
                            test = float.TryParse(dt.Rows[j]["Manufactured_Max_Load"].ToString(), out Manufactured_Max_Load_test);
                            if (test == false)
                            {
                                UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"Manufactured_Max_Load Wrong Input DataType in the row {j + 2}"));
                                goto ERROR;
                            }
                        }

                        float spacelibrary_test = 0;
                        if (dt.Columns.Contains("SpaceLibrary"))
                        {
                            test = float.TryParse(dt.Rows[j]["SpaceLibrary"].ToString(), out spacelibrary_test);
                            if (test == false)
                            {
                                UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"SpaceLibrary Wrong Input DataType in the row {j + 2}"));
                                goto ERROR;
                            }
                        }
                        Boolean verticalMeasured_test = false;
                        if (dt.Columns.Contains("VerticalMeasured"))
                        {
                            test = Boolean.TryParse(dt.Rows[j]["VerticalMeasured"].ToString(), out verticalMeasured_test);
                            if (test == false)
                            {
                                UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"VerticalMeasured Wrong Input DataType in the row {j + 2} must to be true or false"));
                                goto ERROR;
                            }

                        }
                        int civilNonSteelTypeId_test = 0;

                        if (dt.Columns.Contains("civilNonSteelTypeId"))
                        {
                            DropDownListFilters civilNonSteelType = RelatedTables.FirstOrDefault(x =>
                           x.Key == "civilNonSteelTypeId").Value.FirstOrDefault(x => x.Value == dt.Rows[j]["civilNonSteelTypeId"].ToString());
                            if (civilNonSteelType != null)
                            {
                                civilNonSteelTypeId_test = Convert.ToInt32(civilNonSteelType.Id);
                            }
                            else
                            {
                                UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"CivilNonSteelTypeId Wrong Input Value in the row {j + 2}"));
                                goto ERROR;
                            }
                        }

                        float NumberofBoltHoles_test = 0;
                        if (dt.Columns.Contains("NumberofBoltHoles"))
                        {
                            test = float.TryParse(dt.Rows[j]["NumberofBoltHoles"].ToString(), out NumberofBoltHoles_test);
                            if (test == false)
                            {
                                UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"NumberofBoltHoles Wrong Input DataType in the row {j + 2}"));
                                goto ERROR;
                            }
                        }

                        List<dynamic> DynamicAttList = new List<dynamic>();
                        if (ActColumns > 0)
                        {
                            for (int i = ActColumns; i <= Columns; i++)
                            {
                                string StringDynamicAtt = string.Empty;
                                double? DoubleDynamicAtt = null;
                                DateTime? DateTimeDynamicAtt = null;
                                Boolean? BooleanDynamicAtt = null;
                                ColName = sheet.Cells[1, i].Value.ToString();
                                DA = CivilNonSteelLibraryDynamicAtts.FirstOrDefault(x => x.Key == ColName);
                                //Check dynamic attribute if required or not
                                if (DA != null)
                                {
                                    double double_Test = 0;
                                    DateTime datetime_Test = DateTime.Now;
                                    Boolean boolean_Test = false;
                                    if (DA.Required == true)
                                    {
                                        //if somthing wrong then add row to UnsavedRows
                                        if (String.IsNullOrEmpty(dt.Rows[j][ColName].ToString()))
                                        {
                                            UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"Dynamic Attribute ({ColName}) is required"));
                                            goto ERROR;
                                        }
                                        else
                                        {
                                            if (DA.DataType.Name.ToLower() == "string")
                                            {

                                                StringDynamicAtt = Convert.ToString(dt.Rows[j][ColName]);
                                            }
                                            else if (DA.DataType.Name.ToLower() == "double" || DA.DataType.Name.ToLower() == "int")
                                            {
                                                test = double.TryParse(dt.Rows[j][ColName].ToString(), out double_Test);
                                                if (test == false)
                                                {
                                                    UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"{ColName} Dynamic Attribute Wrong Input DataType in the row {j + 2}"));
                                                    goto ERROR;
                                                }
                                                DoubleDynamicAtt = double_Test;
                                            }
                                            else if (DA.DataType.Name.ToLower() == "datetime")
                                            {
                                                test = DateTime.TryParse(dt.Rows[j][ColName].ToString(), out datetime_Test);
                                                if (test == false)
                                                {
                                                    UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"{ColName} Dynamic Attribute Wrong Input DataType in the row {j + 2}"));
                                                    goto ERROR;
                                                }
                                                DateTimeDynamicAtt = datetime_Test;
                                            }
                                            else if (DA.DataType.Name.ToLower() == "bool")
                                            {
                                                test = Boolean.TryParse(dt.Rows[j][ColName].ToString(), out boolean_Test);
                                                if (test == false)
                                                {
                                                    UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"{ColName} Dynamic Attribute Wrong Input DataType in the row {j + 2}"));
                                                    goto ERROR;
                                                }
                                                BooleanDynamicAtt = boolean_Test;
                                            }


                                        }
                                    }
                                    else
                                    {
                                        if (!String.IsNullOrEmpty(dt.Rows[j][ColName].ToString()))
                                        {
                                            if (DA.DataType.Name.ToLower() == "string")
                                            {
                                                StringDynamicAtt = Convert.ToString(dt.Rows[j][ColName]);
                                            }
                                            else if (DA.DataType.Name.ToLower() == "double" || DA.DataType.Name.ToLower() == "int")
                                            {
                                                test = double.TryParse(dt.Rows[j][ColName].ToString(), out double_Test);
                                                if (test == false)
                                                {
                                                    UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"{ColName} Dynamic Attribute Wrong Input DataType in the row {j + 2}"));
                                                    goto ERROR;
                                                }
                                                DoubleDynamicAtt = double_Test;
                                            }
                                            else if (DA.DataType.Name.ToLower() == "datetime")
                                            {
                                                test = DateTime.TryParse(dt.Rows[j][ColName].ToString(), out datetime_Test);
                                                if (test == false)
                                                {
                                                    UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"{ColName} Dynamic Attribute Wrong Input DataType in the row {j + 2}"));
                                                    goto ERROR;
                                                }
                                                DateTimeDynamicAtt = datetime_Test;

                                            }
                                            else if (DA.DataType.Name.ToLower() == "bool")
                                            {
                                                test = Boolean.TryParse(dt.Rows[j][ColName].ToString(), out boolean_Test);
                                                if (test == false)
                                                {
                                                    UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"{ColName} Dynamic Attribute Wrong Input DataType in the row {j + 2}"));
                                                    goto ERROR;
                                                }
                                                BooleanDynamicAtt = boolean_Test;
                                            }

                                        }
                                    }
                                    if (DA.DataType.Name.ToLower() == "string")
                                    {
                                        DynamicAttList.Add(StringDynamicAtt);
                                    }
                                    else if (DA.DataType.Name.ToLower() == "double" || DA.DataType.Name.ToLower() == "int")
                                    {
                                        DynamicAttList.Add(DoubleDynamicAtt);
                                    }
                                    else if (DA.DataType.Name.ToLower() == "datetime")
                                    {
                                        DynamicAttList.Add(DateTimeDynamicAtt);
                                    }
                                    else if (DA.DataType.Name.ToLower() == "bool")
                                    {
                                        DynamicAttList.Add(BooleanDynamicAtt);
                                    }


                                }
                            }
                        }
                        models.Add(model_test != null ? model_test : null);
                        Prefixs.Add(Prefix_test != null ? Prefix_test : null);
                        notes.Add(note_test != null ? model_test : null);
                        WidthVariations.Add(WidthVariation_test != null ? WidthVariation_test : null);
                        Heights.Add(height_test != 0 ? height_test : 0);
                        SpaceLibraries.Add(spacelibrary_test != 0 ? spacelibrary_test : 0);
                        VerticalMeasuredList.Add(verticalMeasured_test);
                        civilNonSteelTypeIds.Add(civilNonSteelTypeId_test != 0 ? civilNonSteelTypeId_test : 0);
                        NumberofBoltHolesList.Add(NumberofBoltHoles_test != 0 ? NumberofBoltHoles_test : 0);
                        Manufactured_Max_Loads.Add(Manufactured_Max_Load_test != 0 ? Manufactured_Max_Load_test : 0);
                        for (int f = 0; f < DynamicAttList.Count; f++)
                        {
                            DynamicAtts[f].Item3.Add(DynamicAttList[f]);
                        }


                    }
                    catch (Exception err)
                    {

                        //UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, "Wrong Input DataType"));
                    }
                }
                if (models.Count > 0)
                {
                    OracleParameter model = new OracleParameter();
                    model.OracleDbType = OracleDbType.NVarchar2;
                    model.Value = models.ToArray();

                    OracleParameter Note = new OracleParameter();
                    Note.OracleDbType = OracleDbType.NVarchar2;
                    Note.Value = notes.ToArray();

                    OracleParameter Prefix = new OracleParameter();
                    Prefix.OracleDbType = OracleDbType.NVarchar2;
                    Prefix.Value = Prefixs.ToArray();

                    OracleParameter Height = new OracleParameter();
                    Height.OracleDbType = OracleDbType.BinaryFloat;
                    Height.Value = Heights.ToArray();

                    OracleParameter SpaceLibrary = new OracleParameter();
                    SpaceLibrary.OracleDbType = OracleDbType.BinaryFloat;
                    SpaceLibrary.Value = SpaceLibraries.ToArray();

                    OracleParameter VerticalMeasured = new OracleParameter();
                    VerticalMeasured.OracleDbType = OracleDbType.Int32;
                    VerticalMeasured.Value = VerticalMeasuredList.Select(v => v ? 1 : 0).ToArray();


                    OracleParameter civilNonSteelTypeId = new OracleParameter();
                    civilNonSteelTypeId.OracleDbType = OracleDbType.Int32;
                    civilNonSteelTypeId.Value = civilNonSteelTypeIds.ToArray();

                    OracleParameter NumberofBoltHoles = new OracleParameter();
                    NumberofBoltHoles.OracleDbType = OracleDbType.BinaryFloat;
                    NumberofBoltHoles.Value = NumberofBoltHolesList.ToArray();

                    OracleParameter Manufactured_Max_LoadS = new OracleParameter();
                    Manufactured_Max_LoadS.OracleDbType = OracleDbType.BinaryFloat;
                    Manufactured_Max_LoadS.Value = Manufactured_Max_Loads.ToArray();

                    OracleParameter WidthVariationS = new OracleParameter();
                    WidthVariationS.OracleDbType = OracleDbType.NVarchar2;
                    WidthVariationS.Value = WidthVariations.ToArray();

                    // create command and set properties
                    OracleCommand cmd = connection.CreateCommand();
                    cmd.CommandText = "INSERT INTO \"TLIcivilNonSteelLibrary\" (\"Model\", \"Note\", \"Hight\", \"SpaceLibrary\", \"VerticalMeasured\", \"civilNonSteelTypeId\",  \"NumberofBoltHoles\",\"Manufactured_Max_Load\",\"WidthVariation\" ,\"Prefix\") VALUES ( :1, :2, :3, :4, :5, :6, :7 ,:8 ,:9 ,:10)";
                    cmd.ArrayBindCount = models.Count;
                    cmd.Parameters.Add(model);
                    cmd.Parameters.Add(Note);
                    cmd.Parameters.Add(Height);
                    cmd.Parameters.Add(SpaceLibrary);
                    cmd.Parameters.Add(VerticalMeasured);
                    cmd.Parameters.Add(civilNonSteelTypeId);
                    cmd.Parameters.Add(NumberofBoltHoles);
                    cmd.Parameters.Add(Manufactured_Max_LoadS);
                    cmd.Parameters.Add(WidthVariationS);
                    cmd.Parameters.Add(Prefix);
                    try
                    {
                        cmd.ExecuteNonQuery();
                        Console.WriteLine("Data inserted successfully.");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error inserting data: {ex.Message}");
                    }
                    //connection.Close();
                    List<int> InsertedIds = new List<int>();
                    if (LastId != null)
                    {
                        OracleCommand InventoryCmd = connection.CreateCommand();
                        InventoryCmd.CommandType = CommandType.Text;
                        InventoryCmd.CommandText = $"Select \"Id\" From \"TLIcivilNonSteelLibrary\" where \"TLIcivilNonSteelLibrary\".\"Id\" > {LastId.Id}";
                        OracleDataReader Reader = InventoryCmd.ExecuteReader();
                        if (Reader.HasRows)
                        {
                            while (Reader.Read())
                            {
                                InsertedIds.Add(Reader.GetInt32(0));
                            }
                        }
                    }
                    else
                    {
                        OracleCommand InventoryCmd = connection.CreateCommand();
                        InventoryCmd.CommandType = CommandType.Text;
                        InventoryCmd.CommandText = $"Select \"Id\" From \"TLIcivilNonSteelLibrary\"";
                        OracleDataReader Reader = InventoryCmd.ExecuteReader();
                        if (Reader.HasRows)
                        {
                            while (Reader.Read())
                            {
                                InsertedIds.Add(Reader.GetInt32(0));
                            }
                        }
                    }
                    TLIhistory tLIhistory = new TLIhistory()
                    {
                        UserId = UserId,
                        TablesNameId = TableNameEntity.Id,
                        HistoryTypeId = 1,

                    };
                    _dbContext.TLIhistory.Add(tLIhistory);
                    _dbContext.SaveChanges();
                    var civilNonSteelLibraryList = new List<TLIcivilNonSteelLibrary>();
                    if (InsertedIds.Count > 0)
                    {
                        for (int i = 0; i < models.Count; i++)
                        {
                            civilNonSteelLibraryList.Add(new TLIcivilNonSteelLibrary
                            {
                                Id = InsertedIds[i],
                                Model = models[i],
                                Note = notes[i],
                                Prefix = Prefixs[i],
                                Hight = Heights[i],
                                SpaceLibrary = SpaceLibraries[i],
                                VerticalMeasured = VerticalMeasuredList[i],
                                civilNonSteelTypeId = civilNonSteelTypeIds[i],
                                NumberofBoltHoles = NumberofBoltHolesList[i],
                                WidthVariation = WidthVariations[i],
                                Manufactured_Max_Load = Manufactured_Max_Loads[i],

                            });
                        }


                        foreach (int recordId in InsertedIds)
                        {
                            var RecordName = civilNonSteelLibraryList.FirstOrDefault(x => x.Id == recordId);
                            if (RecordName != null)
                            {
                                var attributeNames = RecordName.GetType().GetProperties()
                                    .Where(x =>
                                        (x.PropertyType.IsGenericType && x.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>) &&
                                            new Type[] { typeof(int), typeof(string), typeof(double), typeof(float), typeof(Single), typeof(bool), typeof(DateTime) }
                                            .Contains(x.PropertyType.GetGenericArguments()[0]))
                                        || new Type[] { typeof(int), typeof(string), typeof(double), typeof(bool), typeof(DateTime), typeof(float), typeof(Single) }
                                        .Contains(x.PropertyType)
                                    )
                                    .ToList();

                                foreach (var propertyInfo in attributeNames)
                                {

                                    var propertyName = propertyInfo.Name;

                                    var propertyValue = propertyInfo.GetValue(RecordName)?.ToString().Trim();

                                    TLIhistoryDet tLIhistoryDet = new TLIhistoryDet()
                                    {
                                        HistoryId = tLIhistory.Id,
                                        TablesNameId = TableNameEntity.Id,
                                        RecordId = recordId.ToString(),
                                        AttributeName = propertyName,
                                        NewValue = propertyValue
                                    };


                                    _dbContext.TLIhistoryDet.Add(tLIhistoryDet);
                                }
                            }
                        }
                        _dbContext.SaveChanges();
                    }
                    foreach (var DynamicAtt in DynamicAtts)
                    {
                        for (int k = 0; k < DynamicAtt.Item3.Count; k++)
                        {
                            if (!String.IsNullOrEmpty(DynamicAtt.Item3[k].ToString()))
                            {

                                var Message = _unitOfWork.CivilWithLegsRepository.CheckDynamicValidationAndDependenceDynamic(DynamicAtt.Item1, DynamicAtt.Item3[k], Convert.ToInt32(InsertedIds[k]), tLIhistory.Id, civilNonSteelLibraryList[0]).Message;
                                if (Message != "Success")
                                {
                                    UnsavedRows.Add(new KeyValuePair<int, string>(k + 2, Message + ' ' + $"in the Colum Name{ColName} in the Row Number {k + 2} "));
                                    goto ERROR;
                                }

                            }

                        }
                        //List<string> StringValues = new List<string>();
                        //List<double> DoubleValues = new List<double>();
                        //List<DateTime> DateTimeValues = new List<DateTime>();
                        //List<Int32> BooleanValues = new List<Int32>();
                        //List<int> DynamicAttIds = new List<int>();
                        //List<bool> disables = new List<bool>();
                        //List<int?> sideArmLibIds = new List<int?>();
                        //List<int?> sideArmLibraryIds = new List<int?>();
                        //List<int> InventoryIds = new List<int>();
                        //List<int> tablesNamesIds = new List<int>();
                        //if (DynamicAtt.Item2 == "string")
                        //{
                        //    for (int k = 0; k < DynamicAtt.Item3.Count; k++)
                        //    {
                        //        if (!String.IsNullOrEmpty(DynamicAtt.Item3[k].ToString()))
                        //        {
                        //            StringValues.Add(DynamicAtt.Item3[k].ToString());
                        //            DynamicAttIds.Add(DynamicAtt.Item1);
                        //            disables.Add(false);
                        //            sideArmLibIds.Add(null);
                        //            sideArmLibraryIds.Add(null);
                        //            InventoryIds.Add(Convert.ToInt32(InsertedIds[k]));
                        //            tablesNamesIds.Add(TableNameEntity.Id);
                        //        }
                        //    }
                        //}
                        //if (DynamicAtt.Item2 == "double")
                        //{
                        //    for (int k = 0; k < DynamicAtt.Item3.Count; k++)
                        //    {
                        //        if (!String.IsNullOrEmpty(DynamicAtt.Item3[k].ToString()))
                        //        {
                        //            DoubleValues.Add(Convert.ToInt32(DynamicAtt.Item3[k]));
                        //            DynamicAttIds.Add(DynamicAtt.Item1);
                        //            disables.Add(false);
                        //            sideArmLibIds.Add(null);
                        //            sideArmLibraryIds.Add(null);
                        //            InventoryIds.Add(Convert.ToInt32(InsertedIds[k]));
                        //            tablesNamesIds.Add(TableNameEntity.Id);
                        //        }
                        //    }
                        //}
                        //if (DynamicAtt.Item2 == "datetime")
                        //{
                        //    for (int k = 0; k < DynamicAtt.Item3.Count; k++)
                        //    {
                        //        if (!String.IsNullOrEmpty(DynamicAtt.Item3[k].ToString()))
                        //        {
                        //            DateTimeValues.Add(Convert.ToDateTime(DynamicAtt.Item3[k]));
                        //            DynamicAttIds.Add(DynamicAtt.Item1);
                        //            disables.Add(false);
                        //            sideArmLibIds.Add(null);
                        //            sideArmLibraryIds.Add(null);
                        //            InventoryIds.Add(Convert.ToInt32(InsertedIds[k]));
                        //            tablesNamesIds.Add(TableNameEntity.Id);
                        //        }
                        //    }
                        //}
                        //if (DynamicAtt.Item2 == "boolean")
                        //{
                        //    for (int k = 0; k < DynamicAtt.Item3.Count; k++)
                        //    {
                        //        if (!String.IsNullOrEmpty(DynamicAtt.Item3[k].ToString()))
                        //        {
                        //            bool BoolValue = Convert.ToBoolean(DynamicAtt.Item3[k]);
                        //            if (BoolValue == true)
                        //            {
                        //                BooleanValues.Add(1);
                        //            }
                        //            else
                        //            {
                        //                BooleanValues.Add(0);
                        //            }
                        //            DynamicAttIds.Add(DynamicAtt.Item1);
                        //            disables.Add(false);
                        //            sideArmLibIds.Add(null);
                        //            sideArmLibraryIds.Add(null);
                        //            InventoryIds.Add(Convert.ToInt32(InsertedIds[k]));
                        //            tablesNamesIds.Add(TableNameEntity.Id);
                        //        }
                        //    }
                        //}
                        ////connection.Open();
                        //OracleParameter value = new OracleParameter();
                        //if (DynamicAtt.Item2 == "string")
                        //{
                        //    value.OracleDbType = OracleDbType.NVarchar2;
                        //    value.Value = StringValues.ToArray();
                        //}
                        //else if (DynamicAtt.Item2 == "double")
                        //{
                        //    value.OracleDbType = OracleDbType.BinaryDouble;
                        //    value.Value = DoubleValues.ToArray();
                        //}
                        //else if (DynamicAtt.Item2 == "datetime")
                        //{
                        //    value.OracleDbType = OracleDbType.Date;
                        //    value.Value = DateTimeValues.ToArray();
                        //}
                        //else if (DynamicAtt.Item2 == "boolean")
                        //{
                        //    value.OracleDbType = OracleDbType.Int32;
                        //    value.Value = BooleanValues.ToArray();
                        //}
                        //OracleParameter DynamicAttId = new OracleParameter();
                        //DynamicAttId.OracleDbType = OracleDbType.Int32;
                        //DynamicAttId.Value = DynamicAttIds.ToArray();

                        //OracleParameter disable = new OracleParameter();
                        //disable.OracleDbType = OracleDbType.Boolean;
                        //disable.Value = disables.ToArray();

                        //OracleParameter sideArmLibId = new OracleParameter();
                        //sideArmLibId.OracleDbType = OracleDbType.Int32;
                        //sideArmLibId.Value = sideArmLibIds.ToArray();

                        //OracleParameter sideArmLibraryId = new OracleParameter();
                        //sideArmLibraryId.OracleDbType = OracleDbType.Int32;
                        //sideArmLibraryId.Value = sideArmLibraryIds.ToArray();

                        //OracleParameter InventoryId = new OracleParameter();
                        //InventoryId.OracleDbType = OracleDbType.Int32;
                        //InventoryId.Value = InventoryIds.ToArray();

                        //OracleParameter tablesNamesId = new OracleParameter();
                        //tablesNamesId.OracleDbType = OracleDbType.Int32;
                        //tablesNamesId.Value = tablesNamesIds.ToArray();

                        //OracleCommand dalvcmd = connection.CreateCommand();
                        //if (DynamicAtt.Item2 == "string")
                        //{
                        //    dalvcmd.CommandText = "INSERT INTO \"TLIdynamicAttLibValue\" (\"DynamicAttId\", \"InventoryId\", \"tablesNamesId\", \"ValueString\") VALUES ( :1, :2, :3, :4)";
                        //    dalvcmd.ArrayBindCount = StringValues.Count;
                        //}
                        //else if (DynamicAtt.Item2 == "double")
                        //{
                        //    dalvcmd.CommandText = "INSERT INTO \"TLIdynamicAttLibValue\" (\"DynamicAttId\", \"InventoryId\", \"tablesNamesId\", \"ValueDouble\") VALUES ( :1, :2, :3, :4)";
                        //    dalvcmd.ArrayBindCount = DoubleValues.Count;
                        //}
                        //else if (DynamicAtt.Item2 == "datetime")
                        //{
                        //    dalvcmd.CommandText = "INSERT INTO \"TLIdynamicAttLibValue\" (\"DynamicAttId\", \"InventoryId\", \"tablesNamesId\", \"ValueDateTime\") VALUES ( :1, :2, :3, :4)";
                        //    dalvcmd.ArrayBindCount = DateTimeValues.Count;
                        //}
                        //else if (DynamicAtt.Item2 == "boolean")
                        //{
                        //    dalvcmd.CommandText = "INSERT INTO \"TLIdynamicAttLibValue\" (\"DynamicAttId\", \"InventoryId\", \"tablesNamesId\", \"ValueBoolean\") VALUES ( :1, :2, :3, :4)";
                        //    dalvcmd.ArrayBindCount = BooleanValues.Count;
                        //}
                        ////dalvcmd.Parameters.Clear();
                        //dalvcmd.Parameters.Add(DynamicAttId);
                        //dalvcmd.Parameters.Add(InventoryId);
                        //dalvcmd.Parameters.Add(tablesNamesId);
                        //dalvcmd.Parameters.Add(value);
                        //dalvcmd.ExecuteNonQuery();
                    }
                    RecordId.AddRange(InsertedIds);
                }
            ERROR:;
            }
            catch (Exception err)
            {

                throw err;
            }
        }

        private void SaveloadOtherLibraryUsingOracleBulkCopy(int UserId, out List<int> RecordId, DataTable dt, ref List<KeyValuePair<int, string>> UnsavedRows, int ActColumns, int Columns, ExcelWorksheet sheet, TLItablesNames TableNameEntity, OracleConnection connection)
        {
            try
            {
                RecordId = new List<int>();
                List<string> Models = _dbContext.MV_LOAD_OTHER_LIBRARY_VIEW.Where(x => !x.Deleted).Select(x => x.Model).ToList();

                TLIloadOtherLibrary LastId = _serviceProvider.GetService<ApplicationDbContext>().
                    TLIloadOtherLibrary.OrderByDescending(a => a.Id).FirstOrDefault();

                List<TLIdynamicAtt> LoadOtherLibraryDynamicAtts = _unitOfWork.DynamicAttRepository.GetIncludeWhere(x =>
                    x.tablesNamesId == TableNameEntity.Id && !x.disable, x => x.DataType).ToList();

                List<string> models = new List<string>();
                List<string> notes = new List<string>();
                List<float> lengths = new List<float>();
                List<float> widths = new List<float>();
                List<float> heights = new List<float>();
                List<float> SpaceLibraries = new List<float>();
                List<Tuple<int, string, List<dynamic>>> DynamicAtts = new List<Tuple<int, string, List<dynamic>>>();
                TLIdynamicAtt DA = new TLIdynamicAtt();
                string ColName = null;
                //List<string> test = new List<string>();
                if (ActColumns > 0)
                {
                    for (int i = ActColumns; i <= Columns; i++)
                    {
                        ColName = sheet.Cells[1, i].Value.ToString();
                        DA = LoadOtherLibraryDynamicAtts.FirstOrDefault(x => x.Key == ColName);
                        if (DA != null)
                            DynamicAtts.Add(new Tuple<int, string, List<dynamic>>(DA.Id, DA.DataType.Name.ToLower(), new List<dynamic>()));
                    }
                }
                bool test;
                for (int j = 0; j < dt.Rows.Count; j++)
                {
                    try
                    {
                        string model_test = null;

                        if (dt.Columns.Contains("Model"))
                        {
                            if ((Models.Find(x => x == Convert.ToString(dt.Rows[j]["Model"]))) != null)
                            {
                                UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"Model is already exist in the row {j + 2}"));
                                goto ERROR;
                            }
                            if (!String.IsNullOrEmpty(dt.Rows[j]["Model"].ToString()))
                            {
                                model_test = Convert.ToString(dt.Rows[j]["Model"]);
                            }
                            else
                            {
                                UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"Model can not to be null In The Row {j + 2}"));
                                goto ERROR;
                            }
                        }

                        string note_test = null;
                        if (dt.Columns.Contains("Note"))
                        {
                            if (!String.IsNullOrEmpty(dt.Rows[j]["Note"].ToString()))
                            {
                                note_test = Convert.ToString(dt.Rows[j]["Note"]);
                            }
                        }

                        float length_test = 0;
                        if (dt.Columns.Contains("Length"))
                        {
                            if (!String.IsNullOrEmpty(dt.Rows[j]["Length"].ToString()))
                            {
                                float length_test_1;
                                test = float.TryParse(dt.Rows[j]["Length"].ToString(), out length_test_1);
                                if (test == true)
                                {
                                    length_test = length_test_1;
                                }
                                else
                                {
                                    UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"Length Wrong Input DataType In The Row {j + 2}"));
                                    goto ERROR;
                                }
                            }
                        }

                        float width_test = 0;
                        if (dt.Columns.Contains("Width"))
                        {
                            if (!String.IsNullOrEmpty(dt.Rows[j]["Width"].ToString()))
                            {
                                float width_test_1;
                                test = float.TryParse(dt.Rows[j]["Width"].ToString(), out width_test_1);
                                if (test == true)
                                {
                                    width_test = width_test_1;
                                }
                                else
                                {
                                    UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"Width Wrong Input DataType in the row {j + 2}"));
                                    goto ERROR;
                                }
                            }
                        }

                        float height_test = 0;
                        if (dt.Columns.Contains("Height"))
                        {
                            if (!String.IsNullOrEmpty(dt.Rows[j]["Height"].ToString()))
                            {
                                float height_test_1;
                                test = float.TryParse(dt.Rows[j]["Height"].ToString(), out height_test_1);
                                if (test == true)
                                {
                                    height_test = height_test_1;
                                }
                                else
                                {
                                    UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"Height Wrong Input DataType In The Row {j + 2}"));
                                    goto ERROR;
                                }
                            }
                        }

                        float spacelibrary_test = 0;
                      
                        if (length_test == 0)
                        {
                            UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"length must to be bigger of zero in the row {j + 2}"));
                            goto ERROR;
                        }
                        if (width_test == 0)
                        {
                            UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"width must to be bigger of zero in the row {j + 2}"));
                            goto ERROR;
                        }
                        else
                        {
                            spacelibrary_test = length_test * width_test;
                        }

                            
                        

                        List<dynamic> DynamicAttList = new List<dynamic>();
                        if (ActColumns > 0)
                        {
                            for (int i = ActColumns; i <= Columns; i++)
                            {
                                string StringDynamicAtt = string.Empty;
                                double? DoubleDynamicAtt = null;
                                DateTime? DateTimeDynamicAtt = null;
                                Boolean? BooleanDynamicAtt = null;
                                ColName = sheet.Cells[1, i].Value.ToString();
                                DA = LoadOtherLibraryDynamicAtts.FirstOrDefault(x => x.Key == ColName);
                                //Check dynamic attribute if required or not
                                if (DA != null)
                                {
                                    double double_Test = 0;
                                    DateTime datetime_Test = DateTime.Now;
                                    Boolean boolean_Test = false;
                                    if (DA.Required == true)
                                    {
                                        //if somthing wrong then add row to UnsavedRows
                                        if (String.IsNullOrEmpty(dt.Rows[j][ColName].ToString()))
                                        {
                                            UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"Dynamic Attribute ({ColName}) is required"));
                                            goto ERROR;
                                        }
                                        else
                                        {
                                            if (DA.DataType.Name.ToLower() == "string")
                                            {

                                                StringDynamicAtt = Convert.ToString(dt.Rows[j][ColName]);
                                            }
                                            else if (DA.DataType.Name.ToLower() == "double" || DA.DataType.Name.ToLower() == "int")
                                            {
                                                test = double.TryParse(dt.Rows[j][ColName].ToString(), out double_Test);
                                                if (test == false)
                                                {
                                                    UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"{ColName} Dynamic Attribute Wrong Input DataType in the row {j + 2}"));
                                                    goto ERROR;
                                                }
                                                DoubleDynamicAtt = double_Test;
                                            }
                                            else if (DA.DataType.Name.ToLower() == "datetime")
                                            {
                                                test = DateTime.TryParse(dt.Rows[j][ColName].ToString(), out datetime_Test);
                                                if (test == false)
                                                {
                                                    UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"{ColName} Dynamic Attribute Wrong Input DataType in the row {j + 2}"));
                                                    goto ERROR;
                                                }
                                                DateTimeDynamicAtt = datetime_Test;
                                            }
                                            else if (DA.DataType.Name.ToLower() == "bool")
                                            {
                                                test = Boolean.TryParse(dt.Rows[j][ColName].ToString(), out boolean_Test);
                                                if (test == false)
                                                {
                                                    UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"{ColName} Dynamic Attribute Wrong Input DataType in the row {j + 2}"));
                                                    goto ERROR;
                                                }
                                                BooleanDynamicAtt = boolean_Test;
                                            }


                                        }
                                    }
                                    else
                                    {
                                        if (!String.IsNullOrEmpty(dt.Rows[j][ColName].ToString()))
                                        {
                                            if (DA.DataType.Name.ToLower() == "string")
                                            {
                                                StringDynamicAtt = Convert.ToString(dt.Rows[j][ColName]);
                                            }
                                            else if (DA.DataType.Name.ToLower() == "double" || DA.DataType.Name.ToLower() == "int")
                                            {
                                                test = double.TryParse(dt.Rows[j][ColName].ToString(), out double_Test);
                                                if (test == false)
                                                {
                                                    UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"{ColName} Dynamic Attribute Wrong Input DataType in the row {j + 2}"));
                                                    goto ERROR;
                                                }
                                                DoubleDynamicAtt = double_Test;
                                            }
                                            else if (DA.DataType.Name.ToLower() == "datetime")
                                            {
                                                test = DateTime.TryParse(dt.Rows[j][ColName].ToString(), out datetime_Test);
                                                if (test == false)
                                                {
                                                    UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"{ColName} Dynamic Attribute Wrong Input DataType in the row {j + 2}"));
                                                    goto ERROR;
                                                }
                                                DateTimeDynamicAtt = datetime_Test;

                                            }
                                            else if (DA.DataType.Name.ToLower() == "bool")
                                            {
                                                test = Boolean.TryParse(dt.Rows[j][ColName].ToString(), out boolean_Test);
                                                if (test == false)
                                                {
                                                    UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"{ColName} Dynamic Attribute Wrong Input DataType in the row {j + 2}"));
                                                    goto ERROR;
                                                }
                                                BooleanDynamicAtt = boolean_Test;
                                            }

                                        }
                                    }
                                    if (DA.DataType.Name.ToLower() == "string")
                                    {
                                        DynamicAttList.Add(StringDynamicAtt);
                                    }
                                    else if (DA.DataType.Name.ToLower() == "double" || DA.DataType.Name.ToLower() == "int")
                                    {
                                        DynamicAttList.Add(DoubleDynamicAtt);
                                    }
                                    else if (DA.DataType.Name.ToLower() == "datetime")
                                    {
                                        DynamicAttList.Add(DateTimeDynamicAtt);
                                    }
                                    else if (DA.DataType.Name.ToLower() == "bool")
                                    {
                                        DynamicAttList.Add(BooleanDynamicAtt);
                                    }


                                }
                            }
                        }
                        models.Add(model_test != null ? model_test : null);
                        notes.Add(note_test != null ? note_test : null);
                        lengths.Add(length_test != 0 ? length_test : (float)0);
                        widths.Add(width_test != 0 ? width_test : (float)0);
                        heights.Add(height_test != 0 ? height_test : (float)0);
                        SpaceLibraries.Add(spacelibrary_test != 0 ? spacelibrary_test : 0);
                        for (int f = 0; f < DynamicAttList.Count; f++)
                        {
                            DynamicAtts[f].Item3.Add(DynamicAttList[f]);
                        }


                    }
                    catch (Exception err)
                    {

                        //UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, "Wrong Input DataType"));
                    }
                }
                if (models.Count > 0)
                {
                    OracleParameter model = new OracleParameter();
                    model.OracleDbType = OracleDbType.NVarchar2;
                    model.Value = models.ToArray();

                    OracleParameter note = new OracleParameter();
                    note.OracleDbType = OracleDbType.NVarchar2;
                    note.Value = notes.ToArray();

                    OracleParameter length = new OracleParameter();
                    length.OracleDbType = OracleDbType.BinaryFloat;
                    length.Value = lengths.ToArray();

                    OracleParameter width = new OracleParameter();
                    width.OracleDbType = OracleDbType.BinaryFloat;
                    width.Value = widths.ToArray();

                    OracleParameter height = new OracleParameter();
                    height.OracleDbType = OracleDbType.BinaryFloat;
                    height.Value = heights.ToArray();

                    OracleParameter SpaceLibrary = new OracleParameter();
                    SpaceLibrary.OracleDbType = OracleDbType.BinaryFloat;
                    SpaceLibrary.Value = SpaceLibraries.ToArray();

                    // create command and set properties
                    OracleCommand cmd = connection.CreateCommand();
                    cmd.CommandText = "INSERT INTO \"TLIloadOtherLibrary\" (\"Model\", \"Note\", \"Length\", \"Width\", \"Height\", \"SpaceLibrary\") VALUES ( :1, :2, :3, :4, :5, :6)";
                    cmd.ArrayBindCount = models.Count;
                    cmd.Parameters.Add(model);
                    cmd.Parameters.Add(note);
                    cmd.Parameters.Add(length);
                    cmd.Parameters.Add(width);
                    cmd.Parameters.Add(height);
                    cmd.Parameters.Add(SpaceLibrary);
                    cmd.ExecuteNonQuery();
                    List<int> InsertedIds = new List<int>();
                    if (LastId != null)
                    {
                        OracleCommand InventoryCmd = connection.CreateCommand();
                        InventoryCmd.CommandType = CommandType.Text;
                        InventoryCmd.CommandText = $"Select \"Id\" From \"TLIloadOtherLibrary\" where \"TLIloadOtherLibrary\".\"Id\" > {LastId.Id}";
                        OracleDataReader Reader = InventoryCmd.ExecuteReader();
                        if (Reader.HasRows)
                        {
                            while (Reader.Read())
                            {
                                InsertedIds.Add(Reader.GetInt32(0));
                            }
                        }
                    }
                    else
                    {
                        OracleCommand InventoryCmd = connection.CreateCommand();
                        InventoryCmd.CommandType = CommandType.Text;
                        InventoryCmd.CommandText = $"Select \"Id\" From \"TLIloadOtherLibrary\"";
                        OracleDataReader Reader = InventoryCmd.ExecuteReader();
                        if (Reader.HasRows)
                        {
                            while (Reader.Read())
                            {
                                InsertedIds.Add(Reader.GetInt32(0));
                            }
                        }
                    }
                    TLIhistory tLIhistory = new TLIhistory()
                    {
                        UserId = UserId,
                        TablesNameId = TableNameEntity.Id,
                        HistoryTypeId = 1,

                    };
                    _dbContext.TLIhistory.Add(tLIhistory);
                    _dbContext.SaveChanges();
                    var loadOtherLibraryList = new List<TLIloadOtherLibrary>();

                    for (int i = 0; i < models.Count; i++)
                    {
                        loadOtherLibraryList.Add(new TLIloadOtherLibrary
                        {
                            Id = InsertedIds[i],
                            Model = models[i],
                            Note = notes[i],
                            Length = lengths[i],
                            Height = heights[i],
                            Width = SpaceLibraries[i],
                            SpaceLibrary = SpaceLibraries[i],

                        });
                    }


                    foreach (int recordId in InsertedIds)
                    {
                        var RecordName = loadOtherLibraryList.FirstOrDefault(x => x.Id == recordId);
                        if (RecordName != null)
                        {
                            var attributeNames = RecordName.GetType().GetProperties()
                                .Where(x =>
                                    (x.PropertyType.IsGenericType && x.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>) &&
                                        new Type[] { typeof(int), typeof(string), typeof(double), typeof(float), typeof(Single), typeof(bool), typeof(DateTime) }
                                        .Contains(x.PropertyType.GetGenericArguments()[0]))
                                    || new Type[] { typeof(int), typeof(string), typeof(double), typeof(bool), typeof(DateTime), typeof(float), typeof(Single) }
                                    .Contains(x.PropertyType)
                                )
                                .ToList();

                            foreach (var propertyInfo in attributeNames)
                            {

                                var propertyName = propertyInfo.Name;

                                var propertyValue = propertyInfo.GetValue(RecordName)?.ToString().Trim();

                                TLIhistoryDet tLIhistoryDet = new TLIhistoryDet()
                                {
                                    HistoryId = tLIhistory.Id,
                                    TablesNameId = TableNameEntity.Id,
                                    RecordId = recordId.ToString(),
                                    AttributeName = propertyName,
                                    NewValue = propertyValue
                                };


                                _dbContext.TLIhistoryDet.Add(tLIhistoryDet);
                            }
                        }
                    }
                    _dbContext.SaveChanges();

                    foreach (var DynamicAtt in DynamicAtts)
                    {
                        for (int k = 0; k < DynamicAtt.Item3.Count; k++)
                        {
                            if (!String.IsNullOrEmpty(DynamicAtt.Item3[k].ToString()))
                            {

                                var Message = _unitOfWork.CivilWithLegsRepository.CheckDynamicValidationAndDependenceDynamic(DynamicAtt.Item1, DynamicAtt.Item3[k], Convert.ToInt32(InsertedIds[k]), tLIhistory.Id, loadOtherLibraryList[0]).Message;
                                if (Message != "Success")
                                {
                                    UnsavedRows.Add(new KeyValuePair<int, string>(k + 2, Message + ' ' + $"in the Colum Name{ColName} in the Row Number {k + 2} "));
                                    goto ERROR;
                                }

                            }

                        }
                        //List<string> StringValues = new List<string>();
                        //List<double> DoubleValues = new List<double>();
                        //List<DateTime> DateTimeValues = new List<DateTime>();
                        //List<Int32> BooleanValues = new List<Int32>();
                        //List<int> DynamicAttIds = new List<int>();
                        //List<bool> disables = new List<bool>();
                        //List<int?> sideArmLibIds = new List<int?>();
                        //List<int?> sideArmLibraryIds = new List<int?>();
                        //List<int> InventoryIds = new List<int>();
                        //List<int> tablesNamesIds = new List<int>();
                        //if (DynamicAtt.Item2 == "string")
                        //{
                        //    for (int k = 0; k < DynamicAtt.Item3.Count; k++)
                        //    {
                        //        if (!String.IsNullOrEmpty(DynamicAtt.Item3[k].ToString()))
                        //        {
                        //            StringValues.Add(DynamicAtt.Item3[k].ToString());
                        //            DynamicAttIds.Add(DynamicAtt.Item1);
                        //            disables.Add(false);
                        //            sideArmLibIds.Add(null);
                        //            sideArmLibraryIds.Add(null);
                        //            InventoryIds.Add(Convert.ToInt32(InsertedIds[k]));
                        //            tablesNamesIds.Add(TableNameEntity.Id);
                        //        }
                        //    }
                        //}
                        //if (DynamicAtt.Item2 == "double")
                        //{
                        //    for (int k = 0; k < DynamicAtt.Item3.Count; k++)
                        //    {
                        //        if (!String.IsNullOrEmpty(DynamicAtt.Item3[k].ToString()))
                        //        {
                        //            DoubleValues.Add(Convert.ToInt32(DynamicAtt.Item3[k]));
                        //            DynamicAttIds.Add(DynamicAtt.Item1);
                        //            disables.Add(false);
                        //            sideArmLibIds.Add(null);
                        //            sideArmLibraryIds.Add(null);
                        //            InventoryIds.Add(Convert.ToInt32(InsertedIds[k]));
                        //            tablesNamesIds.Add(TableNameEntity.Id);
                        //        }
                        //    }
                        //}
                        //if (DynamicAtt.Item2 == "datetime")
                        //{
                        //    for (int k = 0; k < DynamicAtt.Item3.Count; k++)
                        //    {
                        //        if (!String.IsNullOrEmpty(DynamicAtt.Item3[k].ToString()))
                        //        {
                        //            DateTimeValues.Add(Convert.ToDateTime(DynamicAtt.Item3[k]));
                        //            DynamicAttIds.Add(DynamicAtt.Item1);
                        //            disables.Add(false);
                        //            sideArmLibIds.Add(null);
                        //            sideArmLibraryIds.Add(null);
                        //            InventoryIds.Add(Convert.ToInt32(InsertedIds[k]));
                        //            tablesNamesIds.Add(TableNameEntity.Id);
                        //        }
                        //    }
                        //}
                        //if (DynamicAtt.Item2 == "boolean")
                        //{
                        //    for (int k = 0; k < DynamicAtt.Item3.Count; k++)
                        //    {
                        //        if (!String.IsNullOrEmpty(DynamicAtt.Item3[k].ToString()))
                        //        {
                        //            bool BoolValue = Convert.ToBoolean(DynamicAtt.Item3[k]);
                        //            if (BoolValue == true)
                        //            {
                        //                BooleanValues.Add(1);
                        //            }
                        //            else
                        //            {
                        //                BooleanValues.Add(0);
                        //            }
                        //            DynamicAttIds.Add(DynamicAtt.Item1);
                        //            disables.Add(false);
                        //            sideArmLibIds.Add(null);
                        //            sideArmLibraryIds.Add(null);
                        //            InventoryIds.Add(Convert.ToInt32(InsertedIds[k]));
                        //            tablesNamesIds.Add(TableNameEntity.Id);
                        //        }
                        //    }
                        //}
                        ////connection.Open();
                        //OracleParameter value = new OracleParameter();
                        //if (DynamicAtt.Item2 == "string")
                        //{
                        //    value.OracleDbType = OracleDbType.NVarchar2;
                        //    value.Value = StringValues.ToArray();
                        //}
                        //else if (DynamicAtt.Item2 == "double")
                        //{
                        //    value.OracleDbType = OracleDbType.BinaryDouble;
                        //    value.Value = DoubleValues.ToArray();
                        //}
                        //else if (DynamicAtt.Item2 == "datetime")
                        //{
                        //    value.OracleDbType = OracleDbType.Date;
                        //    value.Value = DateTimeValues.ToArray();
                        //}
                        //else if (DynamicAtt.Item2 == "boolean")
                        //{
                        //    value.OracleDbType = OracleDbType.Int32;
                        //    value.Value = BooleanValues.ToArray();
                        //}
                        //OracleParameter DynamicAttId = new OracleParameter();
                        //DynamicAttId.OracleDbType = OracleDbType.Int32;
                        //DynamicAttId.Value = DynamicAttIds.ToArray();

                        //OracleParameter disable = new OracleParameter();
                        //disable.OracleDbType = OracleDbType.Boolean;
                        //disable.Value = disables.ToArray();

                        //OracleParameter sideArmLibId = new OracleParameter();
                        //sideArmLibId.OracleDbType = OracleDbType.Int32;
                        //sideArmLibId.Value = sideArmLibIds.ToArray();

                        //OracleParameter sideArmLibraryId = new OracleParameter();
                        //sideArmLibraryId.OracleDbType = OracleDbType.Int32;
                        //sideArmLibraryId.Value = sideArmLibraryIds.ToArray();

                        //OracleParameter InventoryId = new OracleParameter();
                        //InventoryId.OracleDbType = OracleDbType.Int32;
                        //InventoryId.Value = InventoryIds.ToArray();

                        //OracleParameter tablesNamesId = new OracleParameter();
                        //tablesNamesId.OracleDbType = OracleDbType.Int32;
                        //tablesNamesId.Value = tablesNamesIds.ToArray();

                        //OracleCommand dalvcmd = connection.CreateCommand();
                        //if (DynamicAtt.Item2 == "string")
                        //{
                        //    dalvcmd.CommandText = "INSERT INTO \"TLIdynamicAttLibValue\" (\"DynamicAttId\", \"InventoryId\", \"tablesNamesId\", \"ValueString\") VALUES ( :1, :2, :3, :4)";
                        //    dalvcmd.ArrayBindCount = StringValues.Count;
                        //}
                        //else if (DynamicAtt.Item2 == "double")
                        //{
                        //    dalvcmd.CommandText = "INSERT INTO \"TLIdynamicAttLibValue\" (\"DynamicAttId\", \"InventoryId\", \"tablesNamesId\", \"ValueDouble\") VALUES ( :1, :2, :3, :4)";
                        //    dalvcmd.ArrayBindCount = DoubleValues.Count;
                        //}
                        //else if (DynamicAtt.Item2 == "datetime")
                        //{
                        //    dalvcmd.CommandText = "INSERT INTO \"TLIdynamicAttLibValue\" (\"DynamicAttId\", \"InventoryId\", \"tablesNamesId\", \"ValueDateTime\") VALUES ( :1, :2, :3, :4)";
                        //    dalvcmd.ArrayBindCount = DateTimeValues.Count;
                        //}
                        //else if (DynamicAtt.Item2 == "boolean")
                        //{
                        //    dalvcmd.CommandText = "INSERT INTO \"TLIdynamicAttLibValue\" (\"DynamicAttId\", \"InventoryId\", \"tablesNamesId\", \"ValueBoolean\") VALUES ( :1, :2, :3, :4)";
                        //    dalvcmd.ArrayBindCount = BooleanValues.Count;
                        //}
                        ////dalvcmd.Parameters.Clear();
                        //dalvcmd.Parameters.Add(DynamicAttId);
                        //dalvcmd.Parameters.Add(InventoryId);
                        //dalvcmd.Parameters.Add(tablesNamesId);
                        //dalvcmd.Parameters.Add(value);
                        //dalvcmd.ExecuteNonQuery();
                    }
                    RecordId.AddRange(InsertedIds);
                }
            ERROR:;
            }
            catch (Exception err)
            {

                throw err;
            }
        }

        private void SavemwBULibraryUsingOracleBulkCopy(int UserId, out List<int> RecordId, DataTable dt, ref List<KeyValuePair<int, string>> UnsavedRows, int ActColumns, int Columns, ExcelWorksheet sheet, List<KeyValuePair<string, List<DropDownListFilters>>> RelatedTables, TLItablesNames TableNameEntity, OracleConnection connection)
        {
            try
            {
                RecordId = new List<int>();
                List<string> Models = _dbContext.MV_MWBU_LIBRARY_VIEW.Where(x => !x.Deleted).Select(x => x.Model).ToList();

                TLImwBULibrary LastId = _serviceProvider.GetService<ApplicationDbContext>().
                    TLImwBULibrary.OrderByDescending(a => a.Id).FirstOrDefault();

                List<TLIdynamicAtt> MW_BULibraryDynamicAtts = _unitOfWork.DynamicAttRepository.GetIncludeWhere(x =>
                    x.tablesNamesId == TableNameEntity.Id && !x.disable, x => x.DataType).ToList();

                List<string> models = new List<string>();
                List<string> types = new List<string>();
                List<string> notes = new List<string>();
                List<float> lengths = new List<float>();
                List<float> widths = new List<float>();
                List<float> heights = new List<float>();
                List<string> frequencybands = new List<string>();
                List<float> channelbandwidths = new List<float>();
                List<float> SpaceLibraries = new List<float>();
                List<int> diversityTypeIds = new List<int>();
                List<string> FreqChannels = new List<string>();
                List<int> NumOfRFUs = new List<int>();
                List<string> BUSizes = new List<string>();
                List<float> Weights = new List<float>();

                List<Tuple<int, string, List<dynamic>>> DynamicAtts = new List<Tuple<int, string, List<dynamic>>>();
                TLIdynamicAtt DA = new TLIdynamicAtt();
                string ColName = null;
                //List<string> test = new List<string>();
                if (ActColumns > 0)
                {
                    for (int i = ActColumns; i <= Columns; i++)
                    {
                        ColName = sheet.Cells[1, i].Value.ToString();
                        DA = MW_BULibraryDynamicAtts.FirstOrDefault(x => x.Key == ColName);
                        if (DA != null)
                            DynamicAtts.Add(new Tuple<int, string, List<dynamic>>(DA.Id, DA.DataType.Name.ToLower(), new List<dynamic>()));
                    }
                }
                bool test;
                for (int j = 0; j < dt.Rows.Count; j++)
                {
                    try
                    {
                        string model_test = null;

                        if (dt.Columns.Contains("Model"))
                        {
                            if ((Models.Find(x => x == Convert.ToString(dt.Rows[j]["Model"]))) != null)
                            {
                                UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"Model is already exist in the row {j + 2}"));
                                goto ERROR;
                            }
                            if (!String.IsNullOrEmpty(dt.Rows[j]["Model"].ToString()))
                            {
                                model_test = Convert.ToString(dt.Rows[j]["Model"]);
                            }
                            else
                            {
                                UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"Model can not to be null In The Row {j + 2}"));
                                goto ERROR;
                            }
                        }

                        string type_test = null;
                        if (dt.Columns.Contains("Type"))
                        {
                            if (!String.IsNullOrEmpty(dt.Rows[j]["Type"].ToString()))
                            {
                                type_test = Convert.ToString(dt.Rows[j]["Type"]);
                            }
                        }

                        string note_test = null;
                        if (dt.Columns.Contains("Note"))
                        {
                            if (!String.IsNullOrEmpty(dt.Rows[j]["Note"].ToString()))
                            {
                                note_test = Convert.ToString(dt.Rows[j]["Note"]);
                            }
                        }

                        float length_test = 0;
                        if (dt.Columns.Contains("Length"))
                        {
                            test = float.TryParse(dt.Rows[j]["Length"].ToString(), out length_test);
                            if (test == false)
                            {
                                UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"Length Wrong Input DataType In The Row {j + 2}"));
                                goto ERROR;
                            }
                        }

                        float width_test = 0;
                        if (dt.Columns.Contains("Width"))
                        {
                            test = float.TryParse(dt.Rows[j]["Width"].ToString(), out width_test);
                            if (test == false)
                            {
                                UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"Width Wrong Input DataType In The Row {j + 2}"));
                                goto ERROR;
                            }
                        }

                        float Weight_test = 0;
                        if (dt.Columns.Contains("Weight"))
                        {
                            test = float.TryParse(dt.Rows[j]["Weight"].ToString(), out Weight_test);
                            if (test == false)
                            {
                                UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"Weight Wrong Input DataType In The Row {j + 2}"));
                                goto ERROR;
                            }
                        }

                        float height_test = 0;
                        if (dt.Columns.Contains("Height"))
                        {
                            test = float.TryParse(dt.Rows[j]["Height"].ToString(), out height_test);
                            if (test == false)
                            {
                                UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"Height Wrong Input DataType In The Row {j + 2}"));
                                goto ERROR;
                            }
                        }

                        string frequencyband_test = null;
                        if (dt.Columns.Contains("frequency_band"))
                        {
                            if (!String.IsNullOrEmpty(dt.Rows[j]["frequency_band"].ToString()))
                            {
                                frequencyband_test = Convert.ToString(dt.Rows[j]["frequency_band"]);
                            }
                        }

                        string FreqChannels_test = null;
                        if (dt.Columns.Contains("FreqChannel"))
                        {
                            if (!String.IsNullOrEmpty(dt.Rows[j]["FreqChannel"].ToString()))
                            {
                                FreqChannels_test = Convert.ToString(dt.Rows[j]["FreqChannel"]);
                            }
                        }

                        string BUSizes_test = null;
                        if (dt.Columns.Contains("BUSize"))
                        {
                            if (!String.IsNullOrEmpty(dt.Rows[j]["BUSize"].ToString()))
                            {
                                BUSizes_test = Convert.ToString(dt.Rows[j]["BUSize"]);
                            }
                        }

                        float channelbandwidth_test = 0;
                        if (dt.Columns.Contains("channel_bandwidth"))
                        {
                            test = float.TryParse(dt.Rows[j]["channel_bandwidth"].ToString(), out channelbandwidth_test);
                            if (test == false)
                            {
                                UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"channel_bandwidth Wrong Input DataType In The Row {j + 2}"));
                                goto ERROR;
                            }
                        }

                        int NumOfRFUs_test = 0;
                        if (dt.Columns.Contains("NumOfRFU"))
                        {
                            test = int.TryParse(dt.Rows[j]["NumOfRFU"].ToString(), out NumOfRFUs_test);
                            if (test == false)
                            {
                                UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"NumOfRFU Wrong Input Value In The Row {j + 2}"));
                                goto ERROR;
                            }
                        }

                        float spacelibrary_test = 0;
                      
                        if (length_test == 0)
                        {
                            UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"length must to be bigger of zero in the row {j + 2}"));
                            goto ERROR;
                        }
                        if (width_test == 0)
                        {
                            UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"width must to be bigger of zero in the row {j + 2}"));
                            goto ERROR;
                        }
                        else
                        {
                            spacelibrary_test = length_test * width_test;
                        }

                            
                        

                        int diversityTypeId_test = 0;
                        if (dt.Columns.Contains("diversityTypeId"))
                        {
                            if (!String.IsNullOrEmpty(dt.Rows[j]["diversityTypeId"].ToString()))
                            {
                                var diversityType = RelatedTables.Where(x => x.Key == "diversityTypeId").Select(x => x.Value).FirstOrDefault().Where(x => x.Value == (dt.Rows[j]["diversityTypeId"]).ToString()).FirstOrDefault();
                                diversityTypeId_test = Convert.ToInt32(diversityType.Id);
                            }
                            else
                            {
                                UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"diversityTypeId can not to be null In The Row {j + 2}"));
                                goto ERROR;
                            }
                        }

                        List<dynamic> DynamicAttList = new List<dynamic>();
                        if (ActColumns > 0)
                        {
                            for (int i = ActColumns; i <= Columns; i++)
                            {
                                string StringDynamicAtt = string.Empty;
                                double? DoubleDynamicAtt = null;
                                DateTime? DateTimeDynamicAtt = null;
                                Boolean? BooleanDynamicAtt = null;
                                ColName = sheet.Cells[1, i].Value.ToString();
                                DA = MW_BULibraryDynamicAtts.FirstOrDefault(x => x.Key == ColName);
                                //Check dynamic attribute if required or not
                                if (DA != null)
                                {
                                    double double_Test = 0;
                                    DateTime datetime_Test = DateTime.Now;
                                    Boolean boolean_Test = false;
                                    if (DA.Required == true)
                                    {
                                        //if somthing wrong then add row to UnsavedRows
                                        if (String.IsNullOrEmpty(dt.Rows[j][ColName].ToString()))
                                        {
                                            UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"Dynamic Attribute ({ColName}) is required"));
                                            goto ERROR;
                                        }
                                        else
                                        {
                                            if (DA.DataType.Name.ToLower() == "string")
                                            {

                                                StringDynamicAtt = Convert.ToString(dt.Rows[j][ColName]);
                                            }
                                            else if (DA.DataType.Name.ToLower() == "double" || DA.DataType.Name.ToLower() == "int")
                                            {
                                                test = double.TryParse(dt.Rows[j][ColName].ToString(), out double_Test);
                                                if (test == false)
                                                {
                                                    UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"{ColName} Dynamic Attribute Wrong Input DataType in the row {j + 2}"));
                                                    goto ERROR;
                                                }
                                                DoubleDynamicAtt = double_Test;
                                            }
                                            else if (DA.DataType.Name.ToLower() == "datetime")
                                            {
                                                test = DateTime.TryParse(dt.Rows[j][ColName].ToString(), out datetime_Test);
                                                if (test == false)
                                                {
                                                    UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"{ColName} Dynamic Attribute Wrong Input DataType in the row {j + 2}"));
                                                    goto ERROR;
                                                }
                                                DateTimeDynamicAtt = datetime_Test;
                                            }
                                            else if (DA.DataType.Name.ToLower() == "bool")
                                            {
                                                test = Boolean.TryParse(dt.Rows[j][ColName].ToString(), out boolean_Test);
                                                if (test == false)
                                                {
                                                    UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"{ColName} Dynamic Attribute Wrong Input DataType in the row {j + 2}"));
                                                    goto ERROR;
                                                }
                                                BooleanDynamicAtt = boolean_Test;
                                            }


                                        }
                                    }
                                    else
                                    {
                                        if (!String.IsNullOrEmpty(dt.Rows[j][ColName].ToString()))
                                        {
                                            if (DA.DataType.Name.ToLower() == "string")
                                            {
                                                StringDynamicAtt = Convert.ToString(dt.Rows[j][ColName]);
                                            }
                                            else if (DA.DataType.Name.ToLower() == "double" || DA.DataType.Name.ToLower() == "int")
                                            {
                                                test = double.TryParse(dt.Rows[j][ColName].ToString(), out double_Test);
                                                if (test == false)
                                                {
                                                    UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"{ColName} Dynamic Attribute Wrong Input DataType in the row {j + 2}"));
                                                    goto ERROR;
                                                }
                                                DoubleDynamicAtt = double_Test;
                                            }
                                            else if (DA.DataType.Name.ToLower() == "datetime")
                                            {
                                                test = DateTime.TryParse(dt.Rows[j][ColName].ToString(), out datetime_Test);
                                                if (test == false)
                                                {
                                                    UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"{ColName} Dynamic Attribute Wrong Input DataType in the row {j + 2}"));
                                                    goto ERROR;
                                                }
                                                DateTimeDynamicAtt = datetime_Test;

                                            }
                                            else if (DA.DataType.Name.ToLower() == "bool")
                                            {
                                                test = Boolean.TryParse(dt.Rows[j][ColName].ToString(), out boolean_Test);
                                                if (test == false)
                                                {
                                                    UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"{ColName} Dynamic Attribute Wrong Input DataType in the row {j + 2}"));
                                                    goto ERROR;
                                                }
                                                BooleanDynamicAtt = boolean_Test;
                                            }

                                        }
                                    }
                                    if (DA.DataType.Name.ToLower() == "string")
                                    {
                                        DynamicAttList.Add(StringDynamicAtt);
                                    }
                                    else if (DA.DataType.Name.ToLower() == "double" || DA.DataType.Name.ToLower() == "int")
                                    {
                                        DynamicAttList.Add(DoubleDynamicAtt);
                                    }
                                    else if (DA.DataType.Name.ToLower() == "datetime")
                                    {
                                        DynamicAttList.Add(DateTimeDynamicAtt);
                                    }
                                    else if (DA.DataType.Name.ToLower() == "bool")
                                    {
                                        DynamicAttList.Add(BooleanDynamicAtt);
                                    }


                                }
                            }
                        }
                        models.Add(model_test != null ? model_test : null);
                        types.Add(type_test != null ? type_test : null);
                        notes.Add(note_test != null ? note_test : null);
                        lengths.Add(length_test != null ? length_test : 0);
                        widths.Add(width_test != null ? width_test : 0);
                        Weights.Add(Weight_test != null ? Weight_test : 0);
                        heights.Add(height_test != null ? height_test : 0);
                        frequencybands.Add(frequencyband_test != null ? frequencyband_test : null);
                        FreqChannels.Add(FreqChannels_test != null ? FreqChannels_test : null);
                        BUSizes.Add(BUSizes_test != null ? BUSizes_test : null);
                        channelbandwidths.Add(channelbandwidth_test != null ? channelbandwidth_test : 0);
                        NumOfRFUs.Add(NumOfRFUs_test != null ? NumOfRFUs_test : 0);
                        SpaceLibraries.Add(spacelibrary_test != null ? spacelibrary_test : 0);
                        diversityTypeIds.Add(diversityTypeId_test != null ? diversityTypeId_test : 1);
                        for (int f = 0; f < DynamicAttList.Count; f++)
                        {
                            //var h = DynamicAtts[f].Value;
                            DynamicAtts[f].Item3.Add(DynamicAttList[f]);
                            //h.Add(DynamicAttList[f]);
                        }


                    }
                    catch (Exception err)
                    {

                        //UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, "Wrong Input DataType"));
                    }
                }
                if (models.Count > 0)
                {
                    OracleParameter model = new OracleParameter();
                    model.OracleDbType = OracleDbType.NVarchar2;
                    model.Value = models.ToArray();

                    OracleParameter type = new OracleParameter();
                    type.OracleDbType = OracleDbType.NVarchar2;
                    type.Value = types.ToArray();

                    OracleParameter note = new OracleParameter();
                    note.OracleDbType = OracleDbType.NVarchar2;
                    note.Value = notes.ToArray();

                    OracleParameter Length = new OracleParameter();
                    Length.OracleDbType = OracleDbType.BinaryFloat;
                    Length.Value = lengths.ToArray();

                    OracleParameter Width = new OracleParameter();
                    Width.OracleDbType = OracleDbType.BinaryFloat;
                    Width.Value = widths.ToArray();

                    OracleParameter Weight = new OracleParameter();
                    Weight.OracleDbType = OracleDbType.BinaryFloat;
                    Weight.Value = Weights.ToArray();

                    OracleParameter Height = new OracleParameter();
                    Height.OracleDbType = OracleDbType.BinaryFloat;
                    Height.Value = heights.ToArray();

                    OracleParameter frequency_band = new OracleParameter();
                    frequency_band.OracleDbType = OracleDbType.NVarchar2;
                    frequency_band.Value = frequencybands.ToArray();

                    OracleParameter FreqChannel = new OracleParameter();
                    FreqChannel.OracleDbType = OracleDbType.NVarchar2;
                    FreqChannel.Value = FreqChannels.ToArray();

                    OracleParameter BUSize = new OracleParameter();
                    BUSize.OracleDbType = OracleDbType.NVarchar2;
                    BUSize.Value = BUSizes.ToArray();

                    OracleParameter channel_bandwidth = new OracleParameter();
                    channel_bandwidth.OracleDbType = OracleDbType.BinaryFloat;
                    channel_bandwidth.Value = channelbandwidths.ToArray();

                    OracleParameter NumOfRFU = new OracleParameter();
                    NumOfRFU.OracleDbType = OracleDbType.Int64;
                    NumOfRFU.Value = NumOfRFUs.ToArray();

                    OracleParameter SpaceLibrary = new OracleParameter();
                    SpaceLibrary.OracleDbType = OracleDbType.BinaryFloat;
                    SpaceLibrary.Value = SpaceLibraries.ToArray();

                    OracleParameter diversityTypeId = new OracleParameter();
                    diversityTypeId.OracleDbType = OracleDbType.Int32;
                    diversityTypeId.Value = diversityTypeIds.ToArray();

                    // create command and set properties
                    OracleCommand cmd = connection.CreateCommand();
                    cmd.CommandText = "INSERT INTO \"TLImwBULibrary\" (\"Model\", \"Type\", \"Note\", \"L_W_H\", \"Length\", \"Width\", \"Weight\", \"Height\", \"frequency_band\", \"FreqChannel\", \"BUSize\" ,\"channel_bandwidth\", \"SpaceLibrary\", \"NumOfRFU\", \"diversityTypeId\") VALUES ( :1, :2, :3, :4, :5, :6, :7, :8, :9, :10, :11, :12, :13, :14, :15)";
                    cmd.ArrayBindCount = models.Count;
                    cmd.Parameters.Add(model != null ? model : "NA");
                    cmd.Parameters.Add(type != null ? type : "NA");
                    cmd.Parameters.Add(note != null ? note : "NA");
                    cmd.Parameters.Add(new OracleParameter(":4", OracleDbType.NVarchar2) { Value = DBNull.Value });
                    cmd.Parameters.Add(Length != null ? Length : 0);
                    cmd.Parameters.Add(Width != null ? Width : 0);
                    cmd.Parameters.Add(Weight != null ? Weight : 0);
                    cmd.Parameters.Add(Height != null ? Height : 0);
                    cmd.Parameters.Add(frequency_band != null ? frequency_band : 0);
                    cmd.Parameters.Add(FreqChannel != null ? FreqChannel : 0);
                    cmd.Parameters.Add(BUSize != null ? BUSize : 0);
                    cmd.Parameters.Add(channel_bandwidth != null ? channel_bandwidth : 0);
                    cmd.Parameters.Add(SpaceLibrary != null ? SpaceLibrary : 0);
                    cmd.Parameters.Add(NumOfRFU != null ? NumOfRFU : 0);
                    cmd.Parameters.Add(diversityTypeId != null ? diversityTypeId : -1);
                    cmd.ExecuteNonQuery();
                    //connection.Close();
                    List<int> InsertedIds = new List<int>();
                    if (LastId != null)
                    {
                        OracleCommand InventoryCmd = connection.CreateCommand();
                        InventoryCmd.CommandType = CommandType.Text;
                        InventoryCmd.CommandText = $"Select \"Id\" From \"TLImwBULibrary\" where \"TLImwBULibrary\".\"Id\" > {LastId.Id}";
                        OracleDataReader Reader = InventoryCmd.ExecuteReader();
                        if (Reader.HasRows)
                        {
                            while (Reader.Read())
                            {
                                InsertedIds.Add(Reader.GetInt32(0));
                            }
                        }
                    }
                    else
                    {
                        OracleCommand InventoryCmd = connection.CreateCommand();
                        InventoryCmd.CommandType = CommandType.Text;
                        InventoryCmd.CommandText = $"Select \"Id\" From \"TLImwBULibrary\"";
                        OracleDataReader Reader = InventoryCmd.ExecuteReader();
                        if (Reader.HasRows)
                        {
                            while (Reader.Read())
                            {
                                InsertedIds.Add(Reader.GetInt32(0));
                            }
                        }
                    }
                    TLIhistory tLIhistory = new TLIhistory()
                    {
                        UserId = UserId,
                        TablesNameId = TableNameEntity.Id,
                        HistoryTypeId = 1,

                    };
                    _dbContext.TLIhistory.Add(tLIhistory);
                    _dbContext.SaveChanges();
                    var mwBULibraryList = new List<TLImwBULibrary>();

                    for (int i = 0; i < models.Count; i++)
                    {
                        mwBULibraryList.Add(new TLImwBULibrary
                        {
                            Id = InsertedIds[i],
                            Model = models[i],
                            Note = notes[i],
                            Length = lengths[i],
                            Height = heights[i],
                            Width = widths[i],
                            SpaceLibrary = SpaceLibraries[i],
                            Type = types[i],
                            BUSize = BUSizes[i],
                            Weight = Weights[i],
                            diversityTypeId = diversityTypeIds[i],
                            FreqChannel = frequencybands[i],
                            channel_bandwidth = channelbandwidths[i],
                            frequency_band = frequencybands[i],
                            NumOfRFU = NumOfRFUs[i],

                        });
                    }


                    foreach (int recordId in InsertedIds)
                    {
                        var RecordName = mwBULibraryList.FirstOrDefault(x => x.Id == recordId);
                        if (RecordName != null)
                        {
                            var attributeNames = RecordName.GetType().GetProperties()
                                .Where(x =>
                                    (x.PropertyType.IsGenericType && x.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>) &&
                                        new Type[] { typeof(int), typeof(string), typeof(double), typeof(float), typeof(Single), typeof(bool), typeof(DateTime) }
                                        .Contains(x.PropertyType.GetGenericArguments()[0]))
                                    || new Type[] { typeof(int), typeof(string), typeof(double), typeof(bool), typeof(DateTime), typeof(float), typeof(Single) }
                                    .Contains(x.PropertyType)
                                )
                                .ToList();

                            foreach (var propertyInfo in attributeNames)
                            {

                                var propertyName = propertyInfo.Name;

                                var propertyValue = propertyInfo.GetValue(RecordName)?.ToString().Trim();

                                TLIhistoryDet tLIhistoryDet = new TLIhistoryDet()
                                {
                                    HistoryId = tLIhistory.Id,
                                    TablesNameId = TableNameEntity.Id,
                                    RecordId = recordId.ToString(),
                                    AttributeName = propertyName,
                                    NewValue = propertyValue
                                };


                                _dbContext.TLIhistoryDet.Add(tLIhistoryDet);
                            }
                        }
                    }
                    _dbContext.SaveChanges();

                    foreach (var DynamicAtt in DynamicAtts)
                    {
                        for (int k = 0; k < DynamicAtt.Item3.Count; k++)
                        {
                            if (!String.IsNullOrEmpty(DynamicAtt.Item3[k].ToString()))
                            {

                                var Message = _unitOfWork.CivilWithLegsRepository.CheckDynamicValidationAndDependenceDynamic(DynamicAtt.Item1, DynamicAtt.Item3[k], Convert.ToInt32(InsertedIds[k]), tLIhistory.Id, mwBULibraryList[0]).Message;
                                if (Message != "Success")
                                {
                                    UnsavedRows.Add(new KeyValuePair<int, string>(k + 2, Message + ' ' + $"in the Colum Name{ColName} in the Row Number {k + 2} "));
                                    goto ERROR;
                                }

                            }

                        }
                        //List<string> StringValues = new List<string>();
                        //List<double> DoubleValues = new List<double>();
                        //List<DateTime> DateTimeValues = new List<DateTime>();
                        //List<Int32> BooleanValues = new List<Int32>();
                        //List<int> DynamicAttIds = new List<int>();
                        //List<bool> disables = new List<bool>();
                        //List<int?> sideArmLibIds = new List<int?>();
                        //List<int?> sideArmLibraryIds = new List<int?>();
                        //List<int> InventoryIds = new List<int>();
                        //List<int> tablesNamesIds = new List<int>();
                        //if (DynamicAtt.Item2 == "string")
                        //{
                        //    for (int k = 0; k < DynamicAtt.Item3.Count; k++)
                        //    {
                        //        if (!String.IsNullOrEmpty(DynamicAtt.Item3[k].ToString()))
                        //        {
                        //            StringValues.Add(DynamicAtt.Item3[k].ToString());
                        //            DynamicAttIds.Add(DynamicAtt.Item1);
                        //            disables.Add(false);
                        //            sideArmLibIds.Add(null);
                        //            sideArmLibraryIds.Add(null);
                        //            InventoryIds.Add(Convert.ToInt32(InsertedIds[k]));
                        //            tablesNamesIds.Add(TableNameEntity.Id);
                        //        }
                        //    }
                        //}
                        //if (DynamicAtt.Item2 == "double")
                        //{
                        //    for (int k = 0; k < DynamicAtt.Item3.Count; k++)
                        //    {
                        //        if (!String.IsNullOrEmpty(DynamicAtt.Item3[k].ToString()))
                        //        {
                        //            DoubleValues.Add(Convert.ToInt32(DynamicAtt.Item3[k]));
                        //            DynamicAttIds.Add(DynamicAtt.Item1);
                        //            disables.Add(false);
                        //            sideArmLibIds.Add(null);
                        //            sideArmLibraryIds.Add(null);
                        //            InventoryIds.Add(Convert.ToInt32(InsertedIds[k]));
                        //            tablesNamesIds.Add(TableNameEntity.Id);
                        //        }
                        //    }
                        //}
                        //if (DynamicAtt.Item2 == "datetime")
                        //{
                        //    for (int k = 0; k < DynamicAtt.Item3.Count; k++)
                        //    {
                        //        if (!String.IsNullOrEmpty(DynamicAtt.Item3[k].ToString()))
                        //        {
                        //            DateTimeValues.Add(Convert.ToDateTime(DynamicAtt.Item3[k]));
                        //            DynamicAttIds.Add(DynamicAtt.Item1);
                        //            disables.Add(false);
                        //            sideArmLibIds.Add(null);
                        //            sideArmLibraryIds.Add(null);
                        //            InventoryIds.Add(Convert.ToInt32(InsertedIds[k]));
                        //            tablesNamesIds.Add(TableNameEntity.Id);
                        //        }
                        //    }
                        //}
                        //if (DynamicAtt.Item2 == "boolean")
                        //{
                        //    for (int k = 0; k < DynamicAtt.Item3.Count; k++)
                        //    {
                        //        if (!String.IsNullOrEmpty(DynamicAtt.Item3[k].ToString()))
                        //        {
                        //            bool BoolValue = Convert.ToBoolean(DynamicAtt.Item3[k]);
                        //            if (BoolValue == true)
                        //            {
                        //                BooleanValues.Add(1);
                        //            }
                        //            else
                        //            {
                        //                BooleanValues.Add(0);
                        //            }
                        //            DynamicAttIds.Add(DynamicAtt.Item1);
                        //            disables.Add(false);
                        //            sideArmLibIds.Add(null);
                        //            sideArmLibraryIds.Add(null);
                        //            InventoryIds.Add(Convert.ToInt32(InsertedIds[k]));
                        //            tablesNamesIds.Add(TableNameEntity.Id);
                        //        }
                        //    }
                        //}
                        ////connection.Open();
                        //OracleParameter value = new OracleParameter();
                        //if (DynamicAtt.Item2 == "string")
                        //{
                        //    value.OracleDbType = OracleDbType.NVarchar2;
                        //    value.Value = StringValues.ToArray();
                        //}
                        //else if (DynamicAtt.Item2 == "double")
                        //{
                        //    value.OracleDbType = OracleDbType.BinaryDouble;
                        //    value.Value = DoubleValues.ToArray();
                        //}
                        //else if (DynamicAtt.Item2 == "datetime")
                        //{
                        //    value.OracleDbType = OracleDbType.Date;
                        //    value.Value = DateTimeValues.ToArray();
                        //}
                        //else if (DynamicAtt.Item2 == "boolean")
                        //{
                        //    value.OracleDbType = OracleDbType.Int32;
                        //    value.Value = BooleanValues.ToArray();
                        //}
                        //OracleParameter DynamicAttId = new OracleParameter();
                        //DynamicAttId.OracleDbType = OracleDbType.Int32;
                        //DynamicAttId.Value = DynamicAttIds.ToArray();

                        //OracleParameter disable = new OracleParameter();
                        //disable.OracleDbType = OracleDbType.Boolean;
                        //disable.Value = disables.ToArray();

                        //OracleParameter sideArmLibId = new OracleParameter();
                        //sideArmLibId.OracleDbType = OracleDbType.Int32;
                        //sideArmLibId.Value = sideArmLibIds.ToArray();

                        //OracleParameter sideArmLibraryId = new OracleParameter();
                        //sideArmLibraryId.OracleDbType = OracleDbType.Int32;
                        //sideArmLibraryId.Value = sideArmLibraryIds.ToArray();

                        //OracleParameter InventoryId = new OracleParameter();
                        //InventoryId.OracleDbType = OracleDbType.Int32;
                        //InventoryId.Value = InventoryIds.ToArray();

                        //OracleParameter tablesNamesId = new OracleParameter();
                        //tablesNamesId.OracleDbType = OracleDbType.Int32;
                        //tablesNamesId.Value = tablesNamesIds.ToArray();

                        //OracleCommand dalvcmd = connection.CreateCommand();
                        //if (DynamicAtt.Item2 == "string")
                        //{
                        //    dalvcmd.CommandText = "INSERT INTO \"TLIdynamicAttLibValue\" (\"DynamicAttId\", \"InventoryId\", \"tablesNamesId\", \"ValueString\") VALUES ( :1, :2, :3, :4)";
                        //    dalvcmd.ArrayBindCount = StringValues.Count;
                        //}
                        //else if (DynamicAtt.Item2 == "double")
                        //{
                        //    dalvcmd.CommandText = "INSERT INTO \"TLIdynamicAttLibValue\" (\"DynamicAttId\", \"InventoryId\", \"tablesNamesId\", \"ValueDouble\") VALUES ( :1, :2, :3, :4)";
                        //    dalvcmd.ArrayBindCount = DoubleValues.Count;
                        //}
                        //else if (DynamicAtt.Item2 == "datetime")
                        //{
                        //    dalvcmd.CommandText = "INSERT INTO \"TLIdynamicAttLibValue\" (\"DynamicAttId\", \"InventoryId\", \"tablesNamesId\", \"ValueDateTime\") VALUES ( :1, :2, :3, :4)";
                        //    dalvcmd.ArrayBindCount = DateTimeValues.Count;
                        //}
                        //else if (DynamicAtt.Item2 == "boolean")
                        //{
                        //    dalvcmd.CommandText = "INSERT INTO \"TLIdynamicAttLibValue\" (\"DynamicAttId\", \"InventoryId\", \"tablesNamesId\", \"ValueBoolean\") VALUES ( :1, :2, :3, :4)";
                        //    dalvcmd.ArrayBindCount = BooleanValues.Count;
                        //}
                        ////dalvcmd.Parameters.Clear();
                        //dalvcmd.Parameters.Add(DynamicAttId);
                        //dalvcmd.Parameters.Add(InventoryId);
                        //dalvcmd.Parameters.Add(tablesNamesId);
                        //dalvcmd.Parameters.Add(value);
                        //dalvcmd.ExecuteNonQuery();
                    }
                    RecordId.AddRange(InsertedIds);
                }
            ERROR:;
            }
            catch (Exception err)
            {

                throw err;
            }
        }

        private void SavemwDishLibraryUsingOracleBulkCopy(int UserId, out List<int> RecordId, DataTable dt, ref List<KeyValuePair<int, string>> UnsavedRows, int ActColumns, int Columns, ExcelWorksheet sheet, List<KeyValuePair<string, List<DropDownListFilters>>> RelatedTables, TLItablesNames TableNameEntity, OracleConnection connection)
        {
            try
            {
                RecordId = new List<int>();
                List<string> Models = _dbContext.MV_MWDISH_LIBRARY_VIEW.Where(x => !x.Deleted).Select(x => x.Model).ToList();

                TLImwDishLibrary LastId = _serviceProvider.GetService<ApplicationDbContext>().
                    TLImwDishLibrary.OrderByDescending(a => a.Id).FirstOrDefault();

                List<TLIdynamicAtt> MW_BULibraryDynamicAtts = _unitOfWork.DynamicAttRepository.GetIncludeWhere(x =>
                    x.tablesNamesId == TableNameEntity.Id && !x.disable, x => x.DataType).ToList();

                List<string> models = new List<string>();
                List<string> descriptions = new List<string>();
                List<string> notes = new List<string>();
                List<float> weights = new List<float>();
                List<string> dimensionsList = new List<string>();
                List<float> lengths = new List<float>();
                List<float> widths = new List<float>();
                List<float> heights = new List<float>();
                List<float> diameters = new List<float>();
                List<string> frequencybands = new List<string>();
                List<float> SpaceLibraries = new List<float>();
                List<int> polarityTypeIds = new List<int>();
                List<int> asTypeIds = new List<int>();
                List<Tuple<int, string, List<dynamic>>> DynamicAtts = new List<Tuple<int, string, List<dynamic>>>();
                TLIdynamicAtt DA = new TLIdynamicAtt();
                string ColName = null;
                //List<string> test = new List<string>();
                if (ActColumns > 0)
                {
                    for (int i = ActColumns; i <= Columns; i++)
                    {
                        ColName = sheet.Cells[1, i].Value.ToString();
                        DA = MW_BULibraryDynamicAtts.FirstOrDefault(x => x.Key == ColName);
                        if (DA != null)
                            DynamicAtts.Add(new Tuple<int, string, List<dynamic>>(DA.Id, DA.DataType.Name.ToLower(), new List<dynamic>()));
                    }
                }
                bool test;
                for (int j = 0; j < dt.Rows.Count; j++)
                {
                    try
                    {
                        string model_test = null;

                        if (dt.Columns.Contains("Model"))
                        {
                            if ((Models.Find(x => x == Convert.ToString(dt.Rows[j]["Model"]))) != null)
                            {
                                UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"Model is already exist in the row {j + 2}"));
                                goto ERROR;
                            }
                            if (!String.IsNullOrEmpty(dt.Rows[j]["Model"].ToString()))
                            {
                                model_test = Convert.ToString(dt.Rows[j]["Model"]);
                            }
                            else
                            {
                                UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"Model can not to be null in the row {j + 2}"));
                                goto ERROR;
                            }

                        }

                        string description_test = null;
                        if (dt.Columns.Contains("Description"))
                        {
                            if (!String.IsNullOrEmpty(dt.Rows[j]["Description"].ToString()))
                            {
                                description_test = Convert.ToString(dt.Rows[j]["Description"]);
                            }
                        }

                        string note_test = null;
                        if (dt.Columns.Contains("Note"))
                        {
                            if (!String.IsNullOrEmpty(dt.Rows[j]["Note"].ToString()))
                            {
                                note_test = Convert.ToString(dt.Rows[j]["Note"]);
                            }
                        }

                        float weight_test = 0;
                        if (dt.Columns.Contains("Weight"))
                        {
                            test = float.TryParse(dt.Rows[j]["Weight"].ToString(), out weight_test);
                            if (test == false)
                            {
                                UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"Weight Wrong Input DataType In The Row {j + 2}"));
                                goto ERROR;
                            }
                        }

                        string dimensions_test = null;
                        if (dt.Columns.Contains("dimensions"))
                        {
                            if (!String.IsNullOrEmpty(dt.Rows[j]["dimensions"].ToString()))
                            {
                                dimensions_test = Convert.ToString(dt.Rows[j]["dimensions"]);
                            }
                        }

                        float length_test = 0;
                        if (dt.Columns.Contains("Length"))
                        {
                            test = float.TryParse(dt.Rows[j]["Length"].ToString(), out length_test);
                            if (test == false)
                            {
                                UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"Length Wrong Input DataType  In The Row {j + 2}"));
                                goto ERROR;
                            }
                        }

                        float width_test = 0;
                        if (dt.Columns.Contains("Width"))
                        {
                            test = float.TryParse(dt.Rows[j]["Width"].ToString(), out width_test);
                            if (test == false)
                            {
                                UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"Width Wrong Input DataType  In The Row {j + 2}"));
                                goto ERROR;
                            }
                        }

                        float height_test = 0;
                        if (dt.Columns.Contains("Height"))
                        {
                            test = float.TryParse(dt.Rows[j]["Height"].ToString(), out height_test);
                            if (test == false)
                            {
                                UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"Height Wrong Input DataType  In The Row {j + 2}"));
                                goto ERROR;
                            }
                        }

                        float diameter_test = 0;
                        if (dt.Columns.Contains("diameter"))
                        {
                            test = float.TryParse(dt.Rows[j]["diameter"].ToString(), out diameter_test);
                            if (test == false)
                            {
                                UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"diameter Wrong Input DataType  In The Row {j + 2}"));
                                goto ERROR;
                            }
                        }

                        string frequencyband_test = null;
                        if (dt.Columns.Contains("frequency_band"))
                        {
                            if (!String.IsNullOrEmpty(dt.Rows[j]["frequency_band"].ToString()))
                            {
                                frequencyband_test = Convert.ToString(dt.Rows[j]["frequency_band"]);
                            }
                        }

                        float spacelibrary_test = 0;
                       
                        if (diameter_test == 0)
                        {
                            UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"Diameter must to be bigger of zero in the Row {j + 2}"));
                            goto ERROR;
                        }
                        else
                        {
                            spacelibrary_test = Convert.ToSingle(3.14) * (float)Math.Pow(diameter_test / 2, 2); ;
                        }

                           
                        

                        int polarityTypeId_test = 0;
                        if (dt.Columns.Contains("polarityTypeId"))
                        {
                            if (!String.IsNullOrEmpty(dt.Rows[j]["polarityTypeId"].ToString()))
                            {
                                DropDownListFilters polarityType = RelatedTables.FirstOrDefault(x => x.Key == "polarityTypeId").Value.FirstOrDefault(x => x.Value == (dt.Rows[j]["polarityTypeId"]).ToString());
                                if (polarityType != null)
                                    polarityTypeId_test = Convert.ToInt32(polarityType.Id);
                            }
                            else
                            {
                                UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"polarityTypeId can not to be null In The Row {j + 2}"));
                                goto ERROR;
                            }
                        }

                        int asTypeId_test = 0;
                        if (dt.Columns.Contains("asTypeId"))
                        {
                            if (!String.IsNullOrEmpty(dt.Rows[j]["asTypeId"].ToString()))
                            {
                                DropDownListFilters asType = RelatedTables.FirstOrDefault(x => x.Key == "asTypeId").Value.FirstOrDefault(x => x.Value == (dt.Rows[j]["asTypeId"]).ToString());
                                if (asType != null)
                                    asTypeId_test = Convert.ToInt32(asType.Id);
                            }
                            else
                            {
                                UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"asTypeId can not to be null In The Row {j + 2}"));
                                goto ERROR;
                            }
                        }

                        List<dynamic> DynamicAttList = new List<dynamic>();
                        if (ActColumns > 0)
                        {
                            for (int i = ActColumns; i <= Columns; i++)
                            {
                                string StringDynamicAtt = string.Empty;
                                double? DoubleDynamicAtt = null;
                                DateTime? DateTimeDynamicAtt = null;
                                Boolean? BooleanDynamicAtt = null;
                                ColName = sheet.Cells[1, i].Value.ToString();
                                DA = MW_BULibraryDynamicAtts.FirstOrDefault(x => x.Key == ColName);
                                //Check dynamic attribute if required or not
                                if (DA != null)
                                {
                                    double double_Test = 0;
                                    DateTime datetime_Test = DateTime.Now;
                                    Boolean boolean_Test = false;
                                    if (DA.Required == true)
                                    {
                                        //if somthing wrong then add row to UnsavedRows
                                        if (String.IsNullOrEmpty(dt.Rows[j][ColName].ToString()))
                                        {
                                            UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"Dynamic Attribute ({ColName}) is required"));
                                            goto ERROR;
                                        }
                                        else
                                        {
                                            if (DA.DataType.Name.ToLower() == "string")
                                            {

                                                StringDynamicAtt = Convert.ToString(dt.Rows[j][ColName]);
                                            }
                                            else if (DA.DataType.Name.ToLower() == "double" || DA.DataType.Name.ToLower() == "int")
                                            {
                                                test = double.TryParse(dt.Rows[j][ColName].ToString(), out double_Test);
                                                if (test == false)
                                                {
                                                    UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"{ColName} Dynamic Attribute Wrong Input DataType in the row {j + 2}"));
                                                    goto ERROR;
                                                }
                                                DoubleDynamicAtt = double_Test;
                                            }
                                            else if (DA.DataType.Name.ToLower() == "datetime")
                                            {
                                                test = DateTime.TryParse(dt.Rows[j][ColName].ToString(), out datetime_Test);
                                                if (test == false)
                                                {
                                                    UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"{ColName} Dynamic Attribute Wrong Input DataType in the row {j + 2}"));
                                                    goto ERROR;
                                                }
                                                DateTimeDynamicAtt = datetime_Test;
                                            }
                                            else if (DA.DataType.Name.ToLower() == "bool")
                                            {
                                                test = Boolean.TryParse(dt.Rows[j][ColName].ToString(), out boolean_Test);
                                                if (test == false)
                                                {
                                                    UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"{ColName} Dynamic Attribute Wrong Input DataType in the row {j + 2}"));
                                                    goto ERROR;
                                                }
                                                BooleanDynamicAtt = boolean_Test;
                                            }


                                        }
                                    }
                                    else
                                    {
                                        if (!String.IsNullOrEmpty(dt.Rows[j][ColName].ToString()))
                                        {
                                            if (DA.DataType.Name.ToLower() == "string")
                                            {
                                                StringDynamicAtt = Convert.ToString(dt.Rows[j][ColName]);
                                            }
                                            else if (DA.DataType.Name.ToLower() == "double" || DA.DataType.Name.ToLower() == "int")
                                            {
                                                test = double.TryParse(dt.Rows[j][ColName].ToString(), out double_Test);
                                                if (test == false)
                                                {
                                                    UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"{ColName} Dynamic Attribute Wrong Input DataType in the row {j + 2}"));
                                                    goto ERROR;
                                                }
                                                DoubleDynamicAtt = double_Test;
                                            }
                                            else if (DA.DataType.Name.ToLower() == "datetime")
                                            {
                                                test = DateTime.TryParse(dt.Rows[j][ColName].ToString(), out datetime_Test);
                                                if (test == false)
                                                {
                                                    UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"{ColName} Dynamic Attribute Wrong Input DataType in the row {j + 2}"));
                                                    goto ERROR;
                                                }
                                                DateTimeDynamicAtt = datetime_Test;

                                            }
                                            else if (DA.DataType.Name.ToLower() == "bool")
                                            {
                                                test = Boolean.TryParse(dt.Rows[j][ColName].ToString(), out boolean_Test);
                                                if (test == false)
                                                {
                                                    UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"{ColName} Dynamic Attribute Wrong Input DataType in the row {j + 2}"));
                                                    goto ERROR;
                                                }
                                                BooleanDynamicAtt = boolean_Test;
                                            }

                                        }
                                    }
                                    if (DA.DataType.Name.ToLower() == "string")
                                    {
                                        DynamicAttList.Add(StringDynamicAtt);
                                    }
                                    else if (DA.DataType.Name.ToLower() == "double" || DA.DataType.Name.ToLower() == "int")
                                    {
                                        DynamicAttList.Add(DoubleDynamicAtt);
                                    }
                                    else if (DA.DataType.Name.ToLower() == "datetime")
                                    {
                                        DynamicAttList.Add(DateTimeDynamicAtt);
                                    }
                                    else if (DA.DataType.Name.ToLower() == "bool")
                                    {
                                        DynamicAttList.Add(BooleanDynamicAtt);
                                    }


                                }
                            }
                        }
                        models.Add(model_test != null ? model_test : null);
                        descriptions.Add(description_test != null ? description_test : null);
                        notes.Add(note_test != null ? note_test : null);
                        weights.Add(weight_test != 0 ? weight_test : 0);
                        dimensionsList.Add(dimensions_test != null ? dimensions_test : null);
                        lengths.Add(length_test != 0 ? length_test : 0);
                        widths.Add(width_test != 0 ? width_test : 0);
                        heights.Add(height_test != 0 ? height_test : 0);
                        diameters.Add(diameter_test != 0 ? diameter_test : 0);
                        frequencybands.Add(frequencyband_test != null ? frequencyband_test : null);
                        SpaceLibraries.Add(spacelibrary_test != 0 ? spacelibrary_test : 0);
                        polarityTypeIds.Add(polarityTypeId_test != null ? polarityTypeId_test : 0);
                        asTypeIds.Add(asTypeId_test != null ? asTypeId_test : 0);
                        for (int f = 0; f < DynamicAttList.Count; f++)
                        {
                            //var h = DynamicAtts[f].Value;
                            DynamicAtts[f].Item3.Add(DynamicAttList[f]);
                            //h.Add(DynamicAttList[f]);
                        }


                    }
                    catch (Exception err)
                    {

                        //UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, "Wrong Input DataType"));
                    }
                }
                if (models.Count > 0)
                {
                    OracleParameter model = new OracleParameter();
                    model.OracleDbType = OracleDbType.NVarchar2;
                    model.Value = models.ToArray();

                    OracleParameter Description = new OracleParameter();
                    Description.OracleDbType = OracleDbType.NVarchar2;
                    Description.Value = descriptions.ToArray();

                    OracleParameter note = new OracleParameter();
                    note.OracleDbType = OracleDbType.NVarchar2;
                    note.Value = notes.ToArray();

                    OracleParameter Weight = new OracleParameter();
                    Weight.OracleDbType = OracleDbType.BinaryFloat;
                    Weight.Value = weights.ToArray();

                    OracleParameter dimensions = new OracleParameter();
                    dimensions.OracleDbType = OracleDbType.NVarchar2;
                    dimensions.Value = dimensionsList.ToArray();

                    OracleParameter Length = new OracleParameter();
                    Length.OracleDbType = OracleDbType.BinaryFloat;
                    Length.Value = lengths.ToArray();

                    OracleParameter Width = new OracleParameter();
                    Width.OracleDbType = OracleDbType.BinaryFloat;
                    Width.Value = widths.ToArray();

                    OracleParameter Height = new OracleParameter();
                    Height.OracleDbType = OracleDbType.BinaryFloat;
                    Height.Value = heights.ToArray();

                    OracleParameter diameter = new OracleParameter();
                    diameter.OracleDbType = OracleDbType.BinaryFloat;
                    diameter.Value = diameters.ToArray();

                    OracleParameter frequency_band = new OracleParameter();
                    frequency_band.OracleDbType = OracleDbType.NVarchar2;
                    frequency_band.Value = frequencybands.ToArray();

                    OracleParameter SpaceLibrary = new OracleParameter();
                    SpaceLibrary.OracleDbType = OracleDbType.BinaryFloat;
                    SpaceLibrary.Value = SpaceLibraries.ToArray();

                    OracleParameter polarityTypeId = new OracleParameter();
                    polarityTypeId.OracleDbType = OracleDbType.Int32;
                    //polarityTypeId.IsNullable = true;
                    //polarityTypeId.SourceColumnNullMapping = true;
                    polarityTypeId.Value = polarityTypeIds.ToArray();

                    OracleParameter asTypeId = new OracleParameter();
                    asTypeId.OracleDbType = OracleDbType.Int32;
                    //asTypeId.IsNullable = true;
                    //asTypeId.SourceColumnNullMapping = true;
                    asTypeId.Value = asTypeIds.ToArray();

                    // create command and set properties
                    OracleCommand cmd = connection.CreateCommand();
                    cmd.CommandText = "INSERT INTO \"TLImwDishLibrary\" (\"Model\", \"Description\", \"Note\", \"Weight\", \"dimensions\", \"Length\", \"Width\", \"Height\", \"diameter\", \"frequency_band\", \"SpaceLibrary\", \"polarityTypeId\", \"asTypeId\") VALUES ( :1, :2, :3, :4, :5, :6, :7, :8, :9, :10, :11, :12, :13)";
                    cmd.ArrayBindCount = models.Count;
                    cmd.Parameters.Add(model);
                    cmd.Parameters.Add(Description);
                    cmd.Parameters.Add(note);
                    cmd.Parameters.Add(Weight);
                    cmd.Parameters.Add(dimensions);
                    cmd.Parameters.Add(Length);
                    cmd.Parameters.Add(Width);
                    cmd.Parameters.Add(Height);
                    cmd.Parameters.Add(diameter);
                    cmd.Parameters.Add(frequency_band);
                    cmd.Parameters.Add(SpaceLibrary);
                    cmd.Parameters.Add(polarityTypeId);
                    cmd.Parameters.Add(asTypeId);
                    cmd.ExecuteNonQuery();
                    //connection.Close();
                    List<int> InsertedIds = new List<int>();
                    if (LastId != null)
                    {
                        OracleCommand InventoryCmd = connection.CreateCommand();
                        InventoryCmd.CommandType = CommandType.Text;
                        InventoryCmd.CommandText = $"Select \"Id\" From \"TLImwDishLibrary\" where \"TLImwDishLibrary\".\"Id\" > {LastId.Id}";
                        OracleDataReader Reader = InventoryCmd.ExecuteReader();
                        if (Reader.HasRows)
                        {
                            while (Reader.Read())
                            {
                                InsertedIds.Add(Reader.GetInt32(0));
                            }
                        }
                    }
                    else
                    {
                        OracleCommand InventoryCmd = connection.CreateCommand();
                        InventoryCmd.CommandType = CommandType.Text;
                        InventoryCmd.CommandText = $"Select \"Id\" From \"TLImwDishLibrary\"";
                        OracleDataReader Reader = InventoryCmd.ExecuteReader();
                        if (Reader.HasRows)
                        {
                            while (Reader.Read())
                            {
                                InsertedIds.Add(Reader.GetInt32(0));
                            }
                        }
                    }
                    TLIhistory tLIhistory = new TLIhistory()
                    {
                        UserId = UserId,
                        TablesNameId = TableNameEntity.Id,
                        HistoryTypeId = 1,

                    };
                    _dbContext.TLIhistory.Add(tLIhistory);
                    _dbContext.SaveChanges();
                    var mwDishLibraryList = new List<TLImwDishLibrary>();

                    for (int i = 0; i < models.Count; i++)
                    {
                        mwDishLibraryList.Add(new TLImwDishLibrary
                        {
                            Id = InsertedIds[i],
                            Model = models[i],
                            Note = notes[i],
                            Description = descriptions[i],
                            dimensions = dimensionsList[i],
                            Weight = weights[i],
                            Length = lengths[i],
                            Width = widths[i],
                            Height = heights[i],
                            diameter = diameters[i],
                            frequency_band = frequencybands[i],
                            SpaceLibrary = SpaceLibraries[i],
                            polarityTypeId = polarityTypeIds[i],
                            asTypeId = asTypeIds[i],


                        });
                    }


                    foreach (int recordId in InsertedIds)
                    {
                        var RecordName = mwDishLibraryList.FirstOrDefault(x => x.Id == recordId);
                        if (RecordName != null)
                        {
                            var attributeNames = RecordName.GetType().GetProperties()
                                .Where(x =>
                                    (x.PropertyType.IsGenericType && x.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>) &&
                                        new Type[] { typeof(int), typeof(string), typeof(double), typeof(float), typeof(Single), typeof(bool), typeof(DateTime) }
                                        .Contains(x.PropertyType.GetGenericArguments()[0]))
                                    || new Type[] { typeof(int), typeof(string), typeof(double), typeof(bool), typeof(DateTime), typeof(float), typeof(Single) }
                                    .Contains(x.PropertyType)
                                )
                                .ToList();

                            foreach (var propertyInfo in attributeNames)
                            {

                                var propertyName = propertyInfo.Name;

                                var propertyValue = propertyInfo.GetValue(RecordName)?.ToString().Trim();

                                TLIhistoryDet tLIhistoryDet = new TLIhistoryDet()
                                {
                                    HistoryId = tLIhistory.Id,
                                    TablesNameId = TableNameEntity.Id,
                                    RecordId = recordId.ToString(),
                                    AttributeName = propertyName,
                                    NewValue = propertyValue
                                };


                                _dbContext.TLIhistoryDet.Add(tLIhistoryDet);
                            }
                        }
                    }
                    _dbContext.SaveChanges();

                    foreach (var DynamicAtt in DynamicAtts)
                    {
                        for (int k = 0; k < DynamicAtt.Item3.Count; k++)
                        {
                            if (!String.IsNullOrEmpty(DynamicAtt.Item3[k].ToString()))
                            {

                                var Message = _unitOfWork.CivilWithLegsRepository.CheckDynamicValidationAndDependenceDynamic(DynamicAtt.Item1, DynamicAtt.Item3[k], Convert.ToInt32(InsertedIds[k]), tLIhistory.Id, mwDishLibraryList[0]).Message;
                                if (Message != "Success")
                                {
                                    UnsavedRows.Add(new KeyValuePair<int, string>(k + 2, Message + ' ' + $"in the Colum Name{ColName} in the Row Number {k + 2} "));
                                    goto ERROR;
                                }

                            }

                        }
                        //List<string> StringValues = new List<string>();
                        //List<double> DoubleValues = new List<double>();
                        //List<DateTime> DateTimeValues = new List<DateTime>();
                        //List<Int32> BooleanValues = new List<Int32>();
                        //List<int> DynamicAttIds = new List<int>();
                        //List<bool> disables = new List<bool>();
                        //List<int?> sideArmLibIds = new List<int?>();
                        //List<int?> sideArmLibraryIds = new List<int?>();
                        //List<int> InventoryIds = new List<int>();
                        //List<int> tablesNamesIds = new List<int>();
                        //if (DynamicAtt.Item2 == "string")
                        //{
                        //    for (int k = 0; k < DynamicAtt.Item3.Count; k++)
                        //    {
                        //        if (!String.IsNullOrEmpty(DynamicAtt.Item3[k].ToString()))
                        //        {
                        //            StringValues.Add(DynamicAtt.Item3[k].ToString());
                        //            DynamicAttIds.Add(DynamicAtt.Item1);
                        //            disables.Add(false);
                        //            sideArmLibIds.Add(null);
                        //            sideArmLibraryIds.Add(null);
                        //            InventoryIds.Add(Convert.ToInt32(InsertedIds[k]));
                        //            tablesNamesIds.Add(TableNameEntity.Id);
                        //        }
                        //    }
                        //}
                        //if (DynamicAtt.Item2 == "double")
                        //{
                        //    for (int k = 0; k < DynamicAtt.Item3.Count; k++)
                        //    {
                        //        if (!String.IsNullOrEmpty(DynamicAtt.Item3[k].ToString()))
                        //        {
                        //            DoubleValues.Add(Convert.ToInt32(DynamicAtt.Item3[k]));
                        //            DynamicAttIds.Add(DynamicAtt.Item1);
                        //            disables.Add(false);
                        //            sideArmLibIds.Add(null);
                        //            sideArmLibraryIds.Add(null);
                        //            InventoryIds.Add(Convert.ToInt32(InsertedIds[k]));
                        //            tablesNamesIds.Add(TableNameEntity.Id);
                        //        }
                        //    }
                        //}
                        //if (DynamicAtt.Item2 == "datetime")
                        //{
                        //    for (int k = 0; k < DynamicAtt.Item3.Count; k++)
                        //    {
                        //        if (!String.IsNullOrEmpty(DynamicAtt.Item3[k].ToString()))
                        //        {
                        //            DateTimeValues.Add(Convert.ToDateTime(DynamicAtt.Item3[k]));
                        //            DynamicAttIds.Add(DynamicAtt.Item1);
                        //            disables.Add(false);
                        //            sideArmLibIds.Add(null);
                        //            sideArmLibraryIds.Add(null);
                        //            InventoryIds.Add(Convert.ToInt32(InsertedIds[k]));
                        //            tablesNamesIds.Add(TableNameEntity.Id);
                        //        }
                        //    }
                        //}
                        //if (DynamicAtt.Item2 == "boolean")
                        //{
                        //    for (int k = 0; k < DynamicAtt.Item3.Count; k++)
                        //    {
                        //        if (!String.IsNullOrEmpty(DynamicAtt.Item3[k].ToString()))
                        //        {
                        //            bool BoolValue = Convert.ToBoolean(DynamicAtt.Item3[k]);
                        //            if (BoolValue == true)
                        //            {
                        //                BooleanValues.Add(1);
                        //            }
                        //            else
                        //            {
                        //                BooleanValues.Add(0);
                        //            }
                        //            DynamicAttIds.Add(DynamicAtt.Item1);
                        //            disables.Add(false);
                        //            sideArmLibIds.Add(null);
                        //            sideArmLibraryIds.Add(null);
                        //            InventoryIds.Add(Convert.ToInt32(InsertedIds[k]));
                        //            tablesNamesIds.Add(TableNameEntity.Id);
                        //        }
                        //    }
                        //}
                        ////connection.Open();
                        //OracleParameter value = new OracleParameter();
                        //if (DynamicAtt.Item2 == "string")
                        //{
                        //    value.OracleDbType = OracleDbType.NVarchar2;
                        //    value.Value = StringValues.ToArray();
                        //}
                        //else if (DynamicAtt.Item2 == "double")
                        //{
                        //    value.OracleDbType = OracleDbType.BinaryDouble;
                        //    value.Value = DoubleValues.ToArray();
                        //}
                        //else if (DynamicAtt.Item2 == "datetime")
                        //{
                        //    value.OracleDbType = OracleDbType.Date;
                        //    value.Value = DateTimeValues.ToArray();
                        //}
                        //else if (DynamicAtt.Item2 == "boolean")
                        //{
                        //    value.OracleDbType = OracleDbType.Int32;
                        //    value.Value = BooleanValues.ToArray();
                        //}
                        //OracleParameter DynamicAttId = new OracleParameter();
                        //DynamicAttId.OracleDbType = OracleDbType.Int32;
                        //DynamicAttId.Value = DynamicAttIds.ToArray();

                        //OracleParameter disable = new OracleParameter();
                        //disable.OracleDbType = OracleDbType.Boolean;
                        //disable.Value = disables.ToArray();

                        //OracleParameter sideArmLibId = new OracleParameter();
                        //sideArmLibId.OracleDbType = OracleDbType.Int32;
                        //sideArmLibId.Value = sideArmLibIds.ToArray();

                        //OracleParameter sideArmLibraryId = new OracleParameter();
                        //sideArmLibraryId.OracleDbType = OracleDbType.Int32;
                        //sideArmLibraryId.Value = sideArmLibraryIds.ToArray();

                        //OracleParameter InventoryId = new OracleParameter();
                        //InventoryId.OracleDbType = OracleDbType.Int32;
                        //InventoryId.Value = InventoryIds.ToArray();

                        //OracleParameter tablesNamesId = new OracleParameter();
                        //tablesNamesId.OracleDbType = OracleDbType.Int32;
                        //tablesNamesId.Value = tablesNamesIds.ToArray();

                        //OracleCommand dalvcmd = connection.CreateCommand();
                        //if (DynamicAtt.Item2 == "string")
                        //{
                        //    dalvcmd.CommandText = "INSERT INTO \"TLIdynamicAttLibValue\" (\"DynamicAttId\", \"InventoryId\", \"tablesNamesId\", \"ValueString\") VALUES ( :1, :2, :3, :4)";
                        //    dalvcmd.ArrayBindCount = StringValues.Count;
                        //}
                        //else if (DynamicAtt.Item2 == "double")
                        //{
                        //    dalvcmd.CommandText = "INSERT INTO \"TLIdynamicAttLibValue\" (\"DynamicAttId\", \"InventoryId\", \"tablesNamesId\", \"ValueDouble\") VALUES ( :1, :2, :3, :4)";
                        //    dalvcmd.ArrayBindCount = DoubleValues.Count;
                        //}
                        //else if (DynamicAtt.Item2 == "datetime")
                        //{
                        //    dalvcmd.CommandText = "INSERT INTO \"TLIdynamicAttLibValue\" (\"DynamicAttId\", \"InventoryId\", \"tablesNamesId\", \"ValueDateTime\") VALUES ( :1, :2, :3, :4)";
                        //    dalvcmd.ArrayBindCount = DateTimeValues.Count;
                        //}
                        //else if (DynamicAtt.Item2 == "boolean")
                        //{
                        //    dalvcmd.CommandText = "INSERT INTO \"TLIdynamicAttLibValue\" (\"DynamicAttId\", \"InventoryId\", \"tablesNamesId\", \"ValueBoolean\") VALUES ( :1, :2, :3, :4)";
                        //    dalvcmd.ArrayBindCount = BooleanValues.Count;
                        //}
                        ////dalvcmd.Parameters.Clear();
                        //dalvcmd.Parameters.Add(DynamicAttId);
                        //dalvcmd.Parameters.Add(InventoryId);
                        //dalvcmd.Parameters.Add(tablesNamesId);
                        //dalvcmd.Parameters.Add(value);
                        //dalvcmd.ExecuteNonQuery();
                    }
                    RecordId.AddRange(InsertedIds);
                }
            ERROR:;
            }
            catch (Exception err)
            {

                throw err;
            }
        }

        private void SavemwODULibraryUsingOracleBulkCopy(int UserId, out List<int> RecordId, DataTable dt, ref List<KeyValuePair<int, string>> UnsavedRows, int ActColumns, int Columns, ExcelWorksheet sheet, List<KeyValuePair<string, List<DropDownListFilters>>> RelatedTables, TLItablesNames TableNameEntity, OracleConnection connection)
        {
            try
            {
                RecordId = new List<int>();
                List<string> Models = _unitOfWork.MW_ODULibraryRepository.GetSelect(x => x.Model).ToList();

                TLImwODULibrary LastId = _serviceProvider.GetService<ApplicationDbContext>().
                    TLImwODULibrary.OrderByDescending(a => a.Id).FirstOrDefault();

                List<TLIdynamicAtt> MW_BULibraryDynamicAtts = _unitOfWork.DynamicAttRepository.GetIncludeWhere(x =>
                    x.tablesNamesId == TableNameEntity.Id && !x.disable, x => x.DataType).ToList();

                List<string> models = new List<string>();
                List<string> notes = new List<string>();
                List<float> weights = new List<float>();
                List<float> depths = new List<float>();
                List<float> widths = new List<float>();
                List<float> Diameters = new List<float>();
                List<float> heights = new List<float>();
                List<string> frequencyranges = new List<string>();
                List<string> frequencybands = new List<string>();
                List<float> SpaceLibraries = new List<float>();
                List<int> parityIds = new List<int>();
                List<Tuple<int, string, List<dynamic>>> DynamicAtts = new List<Tuple<int, string, List<dynamic>>>();
                TLIdynamicAtt DA = new TLIdynamicAtt();
                string ColName = null;
                //List<string> test = new List<string>();
                if (ActColumns > 0)
                {
                    for (int i = ActColumns; i <= Columns; i++)
                    {
                        ColName = sheet.Cells[1, i].Value.ToString();
                        DA = MW_BULibraryDynamicAtts.FirstOrDefault(x => x.Key == ColName);
                        if (DA != null)
                            DynamicAtts.Add(new Tuple<int, string, List<dynamic>>(DA.Id, DA.DataType.Name.ToLower(), new List<dynamic>()));
                    }
                }
                bool test;
                for (int j = 0; j < dt.Rows.Count; j++)
                {
                    try
                    {
                        string model_test = null;

                        if (dt.Columns.Contains("Model"))
                        {
                            if ((Models.Find(x => x == Convert.ToString(dt.Rows[j]["Model"]))) != null)
                            {
                                UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"Model is already exist in the row {j + 2}"));
                                goto ERROR;
                            }
                            if (!String.IsNullOrEmpty(dt.Rows[j]["Model"].ToString()))
                            {
                                model_test = Convert.ToString(dt.Rows[j]["Model"]);
                            }
                            else
                            {
                                UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"Model can not to be null In The Row {j + 2}"));
                                goto ERROR;
                            }
                        }

                        string note_test = null;
                        if (dt.Columns.Contains("Note"))
                        {
                            if (!String.IsNullOrEmpty(dt.Rows[j]["Note"].ToString()))
                            {
                                note_test = Convert.ToString(dt.Rows[j]["Note"]);
                            }
                        }

                        float weight_test = 0;
                        if (dt.Columns.Contains("Weight"))
                        {
                            test = float.TryParse(dt.Rows[j]["Weight"].ToString(), out weight_test);
                            if (test == false)
                            {
                                UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"Weight Wrong Input DataType In The Row {j + 2}"));
                                goto ERROR;
                            }
                        }


                        float depth_test = 0;
                        if (dt.Columns.Contains("Depth"))
                        {
                            test = float.TryParse(dt.Rows[j]["Depth"].ToString(), out depth_test);
                            if (test == false)
                            {
                                UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"Depth Wrong Input DataType In The Row {j + 2}"));
                                goto ERROR;
                            }
                        }

                        float width_test = 0;
                        if (dt.Columns.Contains("Width"))
                        {
                            test = float.TryParse(dt.Rows[j]["Width"].ToString(), out width_test);
                            if (test == false)
                            {
                                UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"Width Wrong Input DataType In The Row {j + 2}"));
                                goto ERROR;
                            }
                        }

                        float height_test = 0;
                        if (dt.Columns.Contains("Height"))
                        {
                            test = float.TryParse(dt.Rows[j]["Height"].ToString(), out height_test);
                            if (test == false)
                            {
                                UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"Height Wrong Input DataType In The Row {j + 2}"));
                                goto ERROR;
                            }
                        }

                        float Diameter_test = 0;
                        if (dt.Columns.Contains("Diameter"))
                        {
                            test = float.TryParse(dt.Rows[j]["Diameter"].ToString(), out Diameter_test);
                            if (test == false)
                            {
                                UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"Diameter Wrong Input DataType In The Row {j + 2}"));
                                goto ERROR;
                            }
                        }

                        string frequencyrange_test = null;
                        if (dt.Columns.Contains("frequency_range"))
                        {
                            if (!String.IsNullOrEmpty(dt.Rows[j]["frequency_range"].ToString()))
                            {
                                frequencyrange_test = Convert.ToString(dt.Rows[j]["frequency_range"]);
                            }
                        }

                        string frequencyband_test = null;
                        if (dt.Columns.Contains("frequency_band"))
                        {
                            if (!String.IsNullOrEmpty(dt.Rows[j]["frequency_band"].ToString()))
                            {
                                frequencyband_test = Convert.ToString(dt.Rows[j]["frequency_band"]);
                            }
                        }

                        float spacelibrary_test = 0;
                        
                        if (height_test == 0)
                        {
                            UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"height must to be bigger of zero in the row {j + 2}"));
                            goto ERROR;
                        }
                        if (width_test == 0)
                        {
                            UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"width must to be bigger of zero in the row {j + 2}"));
                            goto ERROR;
                        }
                        else
                        {
                            spacelibrary_test = height_test * width_test;
                        }

                            
                        

                        int parityId_test = 0;
                        if (dt.Columns.Contains("parityId"))
                        {
                            if (!String.IsNullOrEmpty(dt.Rows[j]["parityId"].ToString()))
                            {
                                var parity = RelatedTables.Where(x => x.Key == "parityId").Select(x => x.Value).FirstOrDefault().Where(x => x.Value == (dt.Rows[j]["parityId"]).ToString()).FirstOrDefault();
                                parityId_test = Convert.ToInt32(parity.Id);
                            }
                            else
                            {
                                UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"parityId can not to be null In The Row {j + 2}"));
                                goto ERROR;
                            }
                        }

                        List<dynamic> DynamicAttList = new List<dynamic>();
                        if (ActColumns > 0)
                        {
                            for (int i = ActColumns; i <= Columns; i++)
                            {
                                string StringDynamicAtt = string.Empty;
                                double? DoubleDynamicAtt = null;
                                DateTime? DateTimeDynamicAtt = null;
                                Boolean? BooleanDynamicAtt = null;
                                ColName = sheet.Cells[1, i].Value.ToString();
                                DA = MW_BULibraryDynamicAtts.FirstOrDefault(x => x.Key == ColName);
                                //Check dynamic attribute if required or not
                                if (DA != null)
                                {
                                    double double_Test = 0;
                                    DateTime datetime_Test = DateTime.Now;
                                    Boolean boolean_Test = false;
                                    if (DA.Required == true)
                                    {
                                        //if somthing wrong then add row to UnsavedRows
                                        if (String.IsNullOrEmpty(dt.Rows[j][ColName].ToString()))
                                        {
                                            UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"Dynamic Attribute ({ColName}) is required"));
                                            goto ERROR;
                                        }
                                        else
                                        {
                                            if (DA.DataType.Name.ToLower() == "string")
                                            {

                                                StringDynamicAtt = Convert.ToString(dt.Rows[j][ColName]);
                                            }
                                            else if (DA.DataType.Name.ToLower() == "double" || DA.DataType.Name.ToLower() == "int")
                                            {
                                                test = double.TryParse(dt.Rows[j][ColName].ToString(), out double_Test);
                                                if (test == false)
                                                {
                                                    UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"{ColName} Dynamic Attribute Wrong Input DataType in the row {j + 2}"));
                                                    goto ERROR;
                                                }
                                                DoubleDynamicAtt = double_Test;
                                            }
                                            else if (DA.DataType.Name.ToLower() == "datetime")
                                            {
                                                test = DateTime.TryParse(dt.Rows[j][ColName].ToString(), out datetime_Test);
                                                if (test == false)
                                                {
                                                    UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"{ColName} Dynamic Attribute Wrong Input DataType in the row {j + 2}"));
                                                    goto ERROR;
                                                }
                                                DateTimeDynamicAtt = datetime_Test;
                                            }
                                            else if (DA.DataType.Name.ToLower() == "bool")
                                            {
                                                test = Boolean.TryParse(dt.Rows[j][ColName].ToString(), out boolean_Test);
                                                if (test == false)
                                                {
                                                    UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"{ColName} Dynamic Attribute Wrong Input DataType in the row {j + 2}"));
                                                    goto ERROR;
                                                }
                                                BooleanDynamicAtt = boolean_Test;
                                            }


                                        }
                                    }
                                    else
                                    {
                                        if (!String.IsNullOrEmpty(dt.Rows[j][ColName].ToString()))
                                        {
                                            if (DA.DataType.Name.ToLower() == "string")
                                            {
                                                StringDynamicAtt = Convert.ToString(dt.Rows[j][ColName]);
                                            }
                                            else if (DA.DataType.Name.ToLower() == "double" || DA.DataType.Name.ToLower() == "int")
                                            {
                                                test = double.TryParse(dt.Rows[j][ColName].ToString(), out double_Test);
                                                if (test == false)
                                                {
                                                    UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"{ColName} Dynamic Attribute Wrong Input DataType in the row {j + 2}"));
                                                    goto ERROR;
                                                }
                                                DoubleDynamicAtt = double_Test;
                                            }
                                            else if (DA.DataType.Name.ToLower() == "datetime")
                                            {
                                                test = DateTime.TryParse(dt.Rows[j][ColName].ToString(), out datetime_Test);
                                                if (test == false)
                                                {
                                                    UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"{ColName} Dynamic Attribute Wrong Input DataType in the row {j + 2}"));
                                                    goto ERROR;
                                                }
                                                DateTimeDynamicAtt = datetime_Test;

                                            }
                                            else if (DA.DataType.Name.ToLower() == "bool")
                                            {
                                                test = Boolean.TryParse(dt.Rows[j][ColName].ToString(), out boolean_Test);
                                                if (test == false)
                                                {
                                                    UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"{ColName} Dynamic Attribute Wrong Input DataType in the row {j + 2}"));
                                                    goto ERROR;
                                                }
                                                BooleanDynamicAtt = boolean_Test;
                                            }

                                        }
                                    }
                                    if (DA.DataType.Name.ToLower() == "string")
                                    {
                                        DynamicAttList.Add(StringDynamicAtt);
                                    }
                                    else if (DA.DataType.Name.ToLower() == "double" || DA.DataType.Name.ToLower() == "int")
                                    {
                                        DynamicAttList.Add(DoubleDynamicAtt);
                                    }
                                    else if (DA.DataType.Name.ToLower() == "datetime")
                                    {
                                        DynamicAttList.Add(DateTimeDynamicAtt);
                                    }
                                    else if (DA.DataType.Name.ToLower() == "bool")
                                    {
                                        DynamicAttList.Add(BooleanDynamicAtt);
                                    }


                                }
                            }
                        }
                        models.Add(model_test != null ? model_test : null);
                        notes.Add(note_test != null ? note_test : null);
                        weights.Add(weight_test != 0 ? weight_test : 0);
                        depths.Add(depth_test != 0 ? depth_test : 0);
                        widths.Add(width_test != 0 ? width_test : 0);
                        heights.Add(height_test != 0 ? height_test : 0);
                        frequencyranges.Add(frequencyrange_test != null ? frequencyrange_test : null);
                        frequencybands.Add(frequencyband_test != null ? frequencyband_test : null);
                        SpaceLibraries.Add(spacelibrary_test != 0 ? spacelibrary_test : 0);
                        parityIds.Add(parityId_test != 0 ? parityId_test : 0);
                        Diameters.Add(Diameter_test != 0 ? Diameter_test : 0);
                        for (int f = 0; f < DynamicAttList.Count; f++)
                        {
                            //var h = DynamicAtts[f].Value;
                            DynamicAtts[f].Item3.Add(DynamicAttList[f]);
                            //h.Add(DynamicAttList[f]);
                        }


                    }
                    catch (Exception err)
                    {

                        //UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, "Wrong Input DataType"));
                    }
                }
                if (models.Count > 0)
                {
                    OracleParameter model = new OracleParameter();
                    model.OracleDbType = OracleDbType.NVarchar2;
                    model.Value = models.ToArray();

                    OracleParameter note = new OracleParameter();
                    note.OracleDbType = OracleDbType.NVarchar2;
                    note.Value = notes.ToArray();

                    OracleParameter Weight = new OracleParameter();
                    Weight.OracleDbType = OracleDbType.BinaryFloat;
                    Weight.Value = weights.ToArray();

                    OracleParameter Depth = new OracleParameter();
                    Depth.OracleDbType = OracleDbType.BinaryFloat;
                    Depth.Value = depths.ToArray();

                    OracleParameter Width = new OracleParameter();
                    Width.OracleDbType = OracleDbType.BinaryFloat;
                    Width.Value = widths.ToArray();

                    OracleParameter Height = new OracleParameter();
                    Height.OracleDbType = OracleDbType.BinaryFloat;
                    Height.Value = heights.ToArray();

                    OracleParameter frequency_range = new OracleParameter();
                    frequency_range.OracleDbType = OracleDbType.NVarchar2;
                    frequency_range.Value = frequencyranges.ToArray();

                    OracleParameter frequency_band = new OracleParameter();
                    frequency_band.OracleDbType = OracleDbType.NVarchar2;
                    frequency_band.Value = frequencybands.ToArray();

                    OracleParameter SpaceLibrary = new OracleParameter();
                    SpaceLibrary.OracleDbType = OracleDbType.BinaryFloat;
                    SpaceLibrary.Value = SpaceLibraries.ToArray();

                    OracleParameter DiameterS = new OracleParameter();
                    DiameterS.OracleDbType = OracleDbType.BinaryFloat;
                    DiameterS.Value = Diameters.ToArray();

                    OracleParameter parityId = new OracleParameter();
                    parityId.OracleDbType = OracleDbType.Int32;
                    parityId.Value = parityIds.ToArray();

                    // create command and set properties
                    OracleCommand cmd = connection.CreateCommand();
                    cmd.CommandText = "INSERT INTO \"TLImwODULibrary\" (\"Model\", \"Note\", \"Weight\", \"H_W_D\", \"Depth\", \"Width\", \"Height\", \"frequency_range\", \"frequency_band\", \"SpaceLibrary\", \"parityId\",\"Diameter\") VALUES ( :1, :2, :3, :4, :5, :6, :7, :8, :9, :10, :11 ,:12)";
                    cmd.ArrayBindCount = models.Count;
                    cmd.Parameters.Add(model);
                    cmd.Parameters.Add(note);
                    cmd.Parameters.Add(Weight);
                    cmd.Parameters.Add(new OracleParameter(":4", OracleDbType.NVarchar2) { Value = DBNull.Value });
                    cmd.Parameters.Add(Depth);
                    cmd.Parameters.Add(Width);
                    cmd.Parameters.Add(Height);
                    cmd.Parameters.Add(frequency_range);
                    cmd.Parameters.Add(frequency_band);
                    cmd.Parameters.Add(SpaceLibrary);
                    cmd.Parameters.Add(parityId);
                    cmd.Parameters.Add(DiameterS);
                    cmd.ExecuteNonQuery();
                    //connection.Close();
                    List<int> InsertedIds = new List<int>();
                    if (LastId != null)
                    {
                        OracleCommand InventoryCmd = connection.CreateCommand();
                        InventoryCmd.CommandType = CommandType.Text;
                        InventoryCmd.CommandText = $"Select \"Id\" From \"TLImwODULibrary\" where \"TLImwODULibrary\".\"Id\" > {LastId.Id}";
                        OracleDataReader Reader = InventoryCmd.ExecuteReader();
                        if (Reader.HasRows)
                        {
                            while (Reader.Read())
                            {
                                InsertedIds.Add(Reader.GetInt32(0));
                            }
                        }
                    }
                    else
                    {
                        OracleCommand InventoryCmd = connection.CreateCommand();
                        InventoryCmd.CommandType = CommandType.Text;
                        InventoryCmd.CommandText = $"Select \"Id\" From \"TLImwODULibrary\"";
                        OracleDataReader Reader = InventoryCmd.ExecuteReader();
                        if (Reader.HasRows)
                        {
                            while (Reader.Read())
                            {
                                InsertedIds.Add(Reader.GetInt32(0));
                            }
                        }
                    }
                    TLIhistory tLIhistory = new TLIhistory()
                    {
                        UserId = UserId,
                        TablesNameId = TableNameEntity.Id,
                        HistoryTypeId = 1,

                    };
                    _dbContext.TLIhistory.Add(tLIhistory);
                    _dbContext.SaveChanges();
                    var mwODULibraryList = new List<TLImwODULibrary>();

                    for (int i = 0; i < models.Count; i++)
                    {
                        mwODULibraryList.Add(new TLImwODULibrary
                        {
                            Id = InsertedIds[i],
                            Model = models[i],
                            Note = notes[i],
                            Weight = weights[i],
                            Depth = depths[i],
                            Height = heights[i],
                            Width = widths[i],
                            frequency_band = frequencybands[i],
                            frequency_range = frequencyranges[i],
                            parityId = parityIds[i],
                            Diameter = Diameters[i],
                            SpaceLibrary = SpaceLibraries[i],

                        });
                    }


                    foreach (int recordId in InsertedIds)
                    {
                        var RecordName = mwODULibraryList.FirstOrDefault(x => x.Id == recordId);
                        if (RecordName != null)
                        {
                            var attributeNames = RecordName.GetType().GetProperties()
                                .Where(x =>
                                    (x.PropertyType.IsGenericType && x.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>) &&
                                        new Type[] { typeof(int), typeof(string), typeof(double), typeof(float), typeof(Single), typeof(bool), typeof(DateTime) }
                                        .Contains(x.PropertyType.GetGenericArguments()[0]))
                                    || new Type[] { typeof(int), typeof(string), typeof(double), typeof(bool), typeof(DateTime), typeof(float), typeof(Single) }
                                    .Contains(x.PropertyType)
                                )
                                .ToList();

                            foreach (var propertyInfo in attributeNames)
                            {

                                var propertyName = propertyInfo.Name;

                                var propertyValue = propertyInfo.GetValue(RecordName)?.ToString().Trim();

                                TLIhistoryDet tLIhistoryDet = new TLIhistoryDet()
                                {
                                    HistoryId = tLIhistory.Id,
                                    TablesNameId = TableNameEntity.Id,
                                    RecordId = recordId.ToString(),
                                    AttributeName = propertyName,
                                    NewValue = propertyValue
                                };


                                _dbContext.TLIhistoryDet.Add(tLIhistoryDet);
                            }
                        }
                    }
                    _dbContext.SaveChanges();

                    foreach (var DynamicAtt in DynamicAtts)
                    {
                        for (int k = 0; k < DynamicAtt.Item3.Count; k++)
                        {
                            if (!String.IsNullOrEmpty(DynamicAtt.Item3[k].ToString()))
                            {

                                var Message = _unitOfWork.CivilWithLegsRepository.CheckDynamicValidationAndDependenceDynamic(DynamicAtt.Item1, DynamicAtt.Item3[k], Convert.ToInt32(InsertedIds[k]), tLIhistory.Id, mwODULibraryList[0]).Message;
                                if (Message != "Success")
                                {
                                    UnsavedRows.Add(new KeyValuePair<int, string>(k + 2, Message + ' ' + $"in the Colum Name{ColName} in the Row Number {k + 2} "));
                                    goto ERROR;
                                }

                            }

                        }
                        //List<string> StringValues = new List<string>();
                        //List<double> DoubleValues = new List<double>();
                        //List<DateTime> DateTimeValues = new List<DateTime>();
                        //List<Int32> BooleanValues = new List<Int32>();
                        //List<int> DynamicAttIds = new List<int>();
                        //List<bool> disables = new List<bool>();
                        //List<int?> sideArmLibIds = new List<int?>();
                        //List<int?> sideArmLibraryIds = new List<int?>();
                        //List<int> InventoryIds = new List<int>();
                        //List<int> tablesNamesIds = new List<int>();
                        //if (DynamicAtt.Item2 == "string")
                        //{
                        //    for (int k = 0; k < DynamicAtt.Item3.Count; k++)
                        //    {
                        //        if (!String.IsNullOrEmpty(DynamicAtt.Item3[k].ToString()))
                        //        {
                        //            StringValues.Add(DynamicAtt.Item3[k].ToString());
                        //            DynamicAttIds.Add(DynamicAtt.Item1);
                        //            disables.Add(false);
                        //            sideArmLibIds.Add(null);
                        //            sideArmLibraryIds.Add(null);
                        //            InventoryIds.Add(Convert.ToInt32(InsertedIds[k]));
                        //            tablesNamesIds.Add(TableNameEntity.Id);
                        //        }
                        //    }
                        //}
                        //if (DynamicAtt.Item2 == "double" || DynamicAtt.Item2 == "int")
                        //{
                        //    for (int k = 0; k < DynamicAtt.Item3.Count; k++)
                        //    {
                        //        if (!String.IsNullOrEmpty(DynamicAtt.Item3[k].ToString()))
                        //        {
                        //            DoubleValues.Add(Convert.ToInt32(DynamicAtt.Item3[k]));
                        //            DynamicAttIds.Add(DynamicAtt.Item1);
                        //            disables.Add(false);
                        //            sideArmLibIds.Add(null);
                        //            sideArmLibraryIds.Add(null);
                        //            InventoryIds.Add(Convert.ToInt32(InsertedIds[k]));
                        //            tablesNamesIds.Add(TableNameEntity.Id);
                        //        }
                        //    }
                        //}
                        //if (DynamicAtt.Item2 == "datetime")
                        //{
                        //    for (int k = 0; k < DynamicAtt.Item3.Count; k++)
                        //    {
                        //        if (!String.IsNullOrEmpty(DynamicAtt.Item3[k].ToString()))
                        //        {
                        //            DateTimeValues.Add(Convert.ToDateTime(DynamicAtt.Item3[k]));
                        //            DynamicAttIds.Add(DynamicAtt.Item1);
                        //            disables.Add(false);
                        //            sideArmLibIds.Add(null);
                        //            sideArmLibraryIds.Add(null);
                        //            InventoryIds.Add(Convert.ToInt32(InsertedIds[k]));
                        //            tablesNamesIds.Add(TableNameEntity.Id);
                        //        }
                        //    }
                        //}
                        //if (DynamicAtt.Item2 == "boolean")
                        //{
                        //    for (int k = 0; k < DynamicAtt.Item3.Count; k++)
                        //    {
                        //        if (!String.IsNullOrEmpty(DynamicAtt.Item3[k].ToString()))
                        //        {
                        //            bool BoolValue = Convert.ToBoolean(DynamicAtt.Item3[k]);
                        //            if (BoolValue == true)
                        //            {
                        //                BooleanValues.Add(1);
                        //            }
                        //            else
                        //            {
                        //                BooleanValues.Add(0);
                        //            }
                        //            DynamicAttIds.Add(DynamicAtt.Item1);
                        //            disables.Add(false);
                        //            sideArmLibIds.Add(null);
                        //            sideArmLibraryIds.Add(null);
                        //            InventoryIds.Add(Convert.ToInt32(InsertedIds[k]));
                        //            tablesNamesIds.Add(TableNameEntity.Id);
                        //        }
                        //    }
                        //}
                        ////connection.Open();
                        //OracleParameter value = new OracleParameter();
                        //if (DynamicAtt.Item2 == "string")
                        //{
                        //    value.OracleDbType = OracleDbType.NVarchar2;
                        //    value.Value = StringValues.ToArray();
                        //}
                        //else if (DynamicAtt.Item2 == "double" || DynamicAtt.Item2 == "int")
                        //{
                        //    value.OracleDbType = OracleDbType.BinaryDouble;
                        //    value.Value = DoubleValues.ToArray();
                        //}
                        //else if (DynamicAtt.Item2 == "datetime")
                        //{
                        //    value.OracleDbType = OracleDbType.Date;
                        //    value.Value = DateTimeValues.ToArray();
                        //}
                        //else if (DynamicAtt.Item2 == "boolean")
                        //{
                        //    value.OracleDbType = OracleDbType.Int32;
                        //    value.Value = BooleanValues.ToArray();
                        //}
                        //OracleParameter DynamicAttId = new OracleParameter();
                        //DynamicAttId.OracleDbType = OracleDbType.Int32;
                        //DynamicAttId.Value = DynamicAttIds.ToArray();

                        //OracleParameter disable = new OracleParameter();
                        //disable.OracleDbType = OracleDbType.Boolean;
                        //disable.Value = disables.ToArray();

                        //OracleParameter sideArmLibId = new OracleParameter();
                        //sideArmLibId.OracleDbType = OracleDbType.Int32;
                        //sideArmLibId.Value = sideArmLibIds.ToArray();

                        //OracleParameter sideArmLibraryId = new OracleParameter();
                        //sideArmLibraryId.OracleDbType = OracleDbType.Int32;
                        //sideArmLibraryId.Value = sideArmLibraryIds.ToArray();

                        //OracleParameter InventoryId = new OracleParameter();
                        //InventoryId.OracleDbType = OracleDbType.Int32;
                        //InventoryId.Value = InventoryIds.ToArray();

                        //OracleParameter tablesNamesId = new OracleParameter();
                        //tablesNamesId.OracleDbType = OracleDbType.Int32;
                        //tablesNamesId.Value = tablesNamesIds.ToArray();

                        //OracleCommand dalvcmd = connection.CreateCommand();
                        //if (DynamicAtt.Item2 == "string")
                        //{
                        //    dalvcmd.CommandText = "INSERT INTO \"TLIdynamicAttLibValue\" (\"DynamicAttId\", \"InventoryId\", \"tablesNamesId\", \"ValueString\") VALUES ( :1, :2, :3, :4)";
                        //    dalvcmd.ArrayBindCount = StringValues.Count;
                        //}
                        //else if (DynamicAtt.Item2 == "double" || DynamicAtt.Item2 == "int")
                        //{
                        //    dalvcmd.CommandText = "INSERT INTO \"TLIdynamicAttLibValue\" (\"DynamicAttId\", \"InventoryId\", \"tablesNamesId\", \"ValueDouble\") VALUES ( :1, :2, :3, :4)";
                        //    dalvcmd.ArrayBindCount = DoubleValues.Count;
                        //}
                        //else if (DynamicAtt.Item2 == "datetime")
                        //{
                        //    dalvcmd.CommandText = "INSERT INTO \"TLIdynamicAttLibValue\" (\"DynamicAttId\", \"InventoryId\", \"tablesNamesId\", \"ValueDateTime\") VALUES ( :1, :2, :3, :4)";
                        //    dalvcmd.ArrayBindCount = DateTimeValues.Count;
                        //}
                        //else if (DynamicAtt.Item2 == "boolean")
                        //{
                        //    dalvcmd.CommandText = "INSERT INTO \"TLIdynamicAttLibValue\" (\"DynamicAttId\", \"InventoryId\", \"tablesNamesId\", \"ValueBoolean\") VALUES ( :1, :2, :3, :4)";
                        //    dalvcmd.ArrayBindCount = BooleanValues.Count;
                        //}
                        ////dalvcmd.Parameters.Clear();
                        //dalvcmd.Parameters.Add(DynamicAttId);
                        //dalvcmd.Parameters.Add(InventoryId);
                        //dalvcmd.Parameters.Add(tablesNamesId);
                        //dalvcmd.Parameters.Add(value);
                        //dalvcmd.ExecuteNonQuery();
                    }
                    RecordId.AddRange(InsertedIds);
                }
            ERROR:;
            }
            catch (Exception err)
            {

                throw err;
            }
        }

        private void SavemwOtherLibraryUsingOracleBulkCopy(int UserId, out List<int> RecordId, DataTable dt, ref List<KeyValuePair<int, string>> UnsavedRows, int ActColumns, int Columns, ExcelWorksheet sheet, TLItablesNames TableNameEntity, OracleConnection connection)
        {
            try
            {
                RecordId = new List<int>();
                List<string> Models = _unitOfWork.MW_OtherLibraryRepository.GetSelect(x => x.Model).ToList();

                TLImwOtherLibrary LastId = _serviceProvider.GetService<ApplicationDbContext>().
                    TLImwOtherLibrary.OrderByDescending(a => a.Id).FirstOrDefault();

                List<TLIdynamicAtt> MW_OtherLibraryDynamicAtts = _unitOfWork.DynamicAttRepository.GetIncludeWhere(x =>
                    x.tablesNamesId == TableNameEntity.Id && !x.disable, x => x.DataType).ToList();

                List<string> models = new List<string>();
                List<string> notes = new List<string>();
                List<float> lengths = new List<float>();
                List<float> widths = new List<float>();
                List<float> heights = new List<float>();
                List<string> frequencybands = new List<string>();
                List<float> SpaceLibraries = new List<float>();
                List<Tuple<int, string, List<dynamic>>> DynamicAtts = new List<Tuple<int, string, List<dynamic>>>();
                TLIdynamicAtt DA = new TLIdynamicAtt();
                string ColName = null;
                //List<string> test = new List<string>();
                if (ActColumns > 0)
                {
                    for (int i = ActColumns; i <= Columns; i++)
                    {
                        ColName = sheet.Cells[1, i].Value.ToString();
                        DA = MW_OtherLibraryDynamicAtts.FirstOrDefault(x => x.Key == ColName);
                        if (DA != null)
                            DynamicAtts.Add(new Tuple<int, string, List<dynamic>>(DA.Id, DA.DataType.Name.ToLower(), new List<dynamic>()));
                    }
                }
                bool test;
                for (int j = 0; j < dt.Rows.Count; j++)
                {
                    try
                    {
                        string model_test = null;

                        if (dt.Columns.Contains("Model"))
                        {
                            if ((Models.Find(x => x == Convert.ToString(dt.Rows[j]["Model"]))) != null)
                            {
                                UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"Model is already exist in the row {j + 2}"));
                                goto ERROR;
                            }
                            if (!String.IsNullOrEmpty(dt.Rows[j]["Model"].ToString()))
                            {
                                model_test = Convert.ToString(dt.Rows[j]["Model"]);
                            }
                            else
                            {
                                UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"Model can not to be null In The Row {j + 2}"));
                                goto ERROR;
                            }
                        }


                        string note_test = null;
                        if (dt.Columns.Contains("Note"))
                        {
                            if (!String.IsNullOrEmpty(dt.Rows[j]["Note"].ToString()))
                            {
                                note_test = Convert.ToString(dt.Rows[j]["Note"]);
                            }
                        }

                        float length_test = 0;
                        if (dt.Columns.Contains("Length"))
                        {
                            if (!String.IsNullOrEmpty(dt.Rows[j]["Length"].ToString()))
                            {
                                float length_test_1;
                                test = float.TryParse(dt.Rows[j]["Length"].ToString(), out length_test_1);
                                if (test == true)
                                {
                                    length_test = length_test_1;
                                }
                                else
                                {
                                    UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"Length Wrong Input DataType In The Row {j + 2}"));
                                    goto ERROR;
                                }
                            }
                        }

                        float width_test = 0;
                        if (dt.Columns.Contains("Width"))
                        {
                            if (!String.IsNullOrEmpty(dt.Rows[j]["Width"].ToString()))
                            {
                                float width_test_1;
                                test = float.TryParse(dt.Rows[j]["Width"].ToString(), out width_test_1);
                                if (test == true)
                                {
                                    width_test = width_test_1;
                                }
                                else
                                {
                                    UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"Width Wrong Input DataType In The Row {j + 2}"));
                                    goto ERROR;
                                }
                            }
                        }

                        float height_test = 0;
                        if (dt.Columns.Contains("Height"))
                        {
                            if (!String.IsNullOrEmpty(dt.Rows[j]["Height"].ToString()))
                            {
                                float height_test_1;
                                test = float.TryParse(dt.Rows[j]["Height"].ToString(), out height_test_1);
                                if (test == true)
                                {
                                    height_test = height_test_1;
                                }
                                else
                                {
                                    UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"Height Wrong Input DataType In The Row {j + 2}"));
                                    goto ERROR;
                                }
                            }
                        }

                        float spacelibrary_test = 0;
                   
                        if (length_test == 0)
                        {
                            UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"length must to be bigger of zero in the row {j + 2}"));
                            goto ERROR;
                        }
                        if (width_test == 0)
                        {
                            UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"Width must to be bigger of zero in the row {j + 2}"));
                            goto ERROR;
                        }
                        else
                        {
                            spacelibrary_test = length_test * width_test;
                        }

                            
                        

                        string frequencyband_test = null;
                        if (dt.Columns.Contains("frequency_band"))
                        {
                            if (!String.IsNullOrEmpty(dt.Rows[j]["frequency_band"].ToString()))
                            {
                                frequencyband_test = Convert.ToString(dt.Rows[j]["frequency_band"]);
                            }
                        }



                        List<dynamic> DynamicAttList = new List<dynamic>();
                        if (ActColumns > 0)
                        {
                            for (int i = ActColumns; i <= Columns; i++)
                            {
                                string StringDynamicAtt = string.Empty;
                                double? DoubleDynamicAtt = null;
                                DateTime? DateTimeDynamicAtt = null;
                                Boolean? BooleanDynamicAtt = null;
                                ColName = sheet.Cells[1, i].Value.ToString();
                                DA = MW_OtherLibraryDynamicAtts.FirstOrDefault(x => x.Key == ColName);
                                //Check dynamic attribute if required or not
                                if (DA != null)
                                {
                                    double double_Test = 0;
                                    DateTime datetime_Test = DateTime.Now;
                                    Boolean boolean_Test = false;
                                    if (DA.Required == true)
                                    {
                                        //if somthing wrong then add row to UnsavedRows
                                        if (String.IsNullOrEmpty(dt.Rows[j][ColName].ToString()))
                                        {
                                            UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"Dynamic Attribute ({ColName}) is required"));
                                            goto ERROR;
                                        }
                                        else
                                        {
                                            if (DA.DataType.Name.ToLower() == "string")
                                            {

                                                StringDynamicAtt = Convert.ToString(dt.Rows[j][ColName]);
                                            }
                                            else if (DA.DataType.Name.ToLower() == "double" || DA.DataType.Name.ToLower() == "int")
                                            {
                                                test = double.TryParse(dt.Rows[j][ColName].ToString(), out double_Test);
                                                if (test == false)
                                                {
                                                    UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"{ColName} Dynamic Attribute Wrong Input DataType in the row {j + 2}"));
                                                    goto ERROR;
                                                }
                                                DoubleDynamicAtt = double_Test;
                                            }
                                            else if (DA.DataType.Name.ToLower() == "datetime")
                                            {
                                                test = DateTime.TryParse(dt.Rows[j][ColName].ToString(), out datetime_Test);
                                                if (test == false)
                                                {
                                                    UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"{ColName} Dynamic Attribute Wrong Input DataType in the row {j + 2}"));
                                                    goto ERROR;
                                                }
                                                DateTimeDynamicAtt = datetime_Test;
                                            }
                                            else if (DA.DataType.Name.ToLower() == "bool")
                                            {
                                                test = Boolean.TryParse(dt.Rows[j][ColName].ToString(), out boolean_Test);
                                                if (test == false)
                                                {
                                                    UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"{ColName} Dynamic Attribute Wrong Input DataType in the row {j + 2}"));
                                                    goto ERROR;
                                                }
                                                BooleanDynamicAtt = boolean_Test;
                                            }


                                        }
                                    }
                                    else
                                    {
                                        if (!String.IsNullOrEmpty(dt.Rows[j][ColName].ToString()))
                                        {
                                            if (DA.DataType.Name.ToLower() == "string")
                                            {
                                                StringDynamicAtt = Convert.ToString(dt.Rows[j][ColName]);
                                            }
                                            else if (DA.DataType.Name.ToLower() == "double" || DA.DataType.Name.ToLower() == "int")
                                            {
                                                test = double.TryParse(dt.Rows[j][ColName].ToString(), out double_Test);
                                                if (test == false)
                                                {
                                                    UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"{ColName} Dynamic Attribute Wrong Input DataType in the row {j + 2}"));
                                                    goto ERROR;
                                                }
                                                DoubleDynamicAtt = double_Test;
                                            }
                                            else if (DA.DataType.Name.ToLower() == "datetime")
                                            {
                                                test = DateTime.TryParse(dt.Rows[j][ColName].ToString(), out datetime_Test);
                                                if (test == false)
                                                {
                                                    UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"{ColName} Dynamic Attribute Wrong Input DataType in the row {j + 2}"));
                                                    goto ERROR;
                                                }
                                                DateTimeDynamicAtt = datetime_Test;

                                            }
                                            else if (DA.DataType.Name.ToLower() == "bool")
                                            {
                                                test = Boolean.TryParse(dt.Rows[j][ColName].ToString(), out boolean_Test);
                                                if (test == false)
                                                {
                                                    UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"{ColName} Dynamic Attribute Wrong Input DataType in the row {j + 2}"));
                                                    goto ERROR;
                                                }
                                                BooleanDynamicAtt = boolean_Test;
                                            }

                                        }
                                    }
                                    if (DA.DataType.Name.ToLower() == "string")
                                    {
                                        DynamicAttList.Add(StringDynamicAtt);
                                    }
                                    else if (DA.DataType.Name.ToLower() == "double" || DA.DataType.Name.ToLower() == "int")
                                    {
                                        DynamicAttList.Add(DoubleDynamicAtt);
                                    }
                                    else if (DA.DataType.Name.ToLower() == "datetime")
                                    {
                                        DynamicAttList.Add(DateTimeDynamicAtt);
                                    }
                                    else if (DA.DataType.Name.ToLower() == "bool")
                                    {
                                        DynamicAttList.Add(BooleanDynamicAtt);
                                    }


                                }
                            }
                        }
                        models.Add(model_test != null ? model_test : null);
                        notes.Add(note_test != null ? note_test : null);
                        lengths.Add(length_test != 0 ? length_test : 0);
                        widths.Add(width_test != 0 ? width_test : 0);
                        heights.Add(height_test != 0 ? height_test : 0);
                        frequencybands.Add(frequencyband_test != null ? frequencyband_test : null);
                        SpaceLibraries.Add(spacelibrary_test != 0 ? spacelibrary_test : 0);
                        for (int f = 0; f < DynamicAttList.Count; f++)
                        {
                            DynamicAtts[f].Item3.Add(DynamicAttList[f]);
                        }


                    }
                    catch (Exception err)
                    {

                        //  UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, "Wrong Input DataType"));
                    }
                }
                if (models.Count > 0)
                {
                    OracleParameter model = new OracleParameter();
                    model.OracleDbType = OracleDbType.NVarchar2;
                    model.Value = models.ToArray();

                    OracleParameter note = new OracleParameter();
                    note.OracleDbType = OracleDbType.NVarchar2;
                    note.Value = notes.ToArray();

                    OracleParameter length = new OracleParameter();
                    length.OracleDbType = OracleDbType.BinaryFloat;
                    length.Value = lengths.ToArray();

                    OracleParameter width = new OracleParameter();
                    width.OracleDbType = OracleDbType.BinaryFloat;
                    width.Value = widths.ToArray();

                    OracleParameter height = new OracleParameter();
                    height.OracleDbType = OracleDbType.BinaryFloat;
                    height.Value = heights.ToArray();


                    OracleParameter frequency_band = new OracleParameter();
                    frequency_band.OracleDbType = OracleDbType.NVarchar2;
                    frequency_band.Value = frequencybands.ToArray();

                    OracleParameter SpaceLibrary = new OracleParameter();
                    SpaceLibrary.OracleDbType = OracleDbType.BinaryFloat;
                    SpaceLibrary.Value = SpaceLibraries.ToArray();

                    // create command and set properties
                    OracleCommand cmd = connection.CreateCommand();
                    cmd.CommandText = "INSERT INTO \"TLImwOtherLibrary\" (\"Model\", \"Note\", \"Length\", \"Width\", \"Height\", \"L_W_H\", \"frequency_band\", \"SpaceLibrary\") VALUES ( :1, :2, :3, :4, :5, :6, :7, :8)";
                    cmd.ArrayBindCount = models.Count;
                    cmd.Parameters.Add(model);
                    cmd.Parameters.Add(note);
                    cmd.Parameters.Add(length);
                    cmd.Parameters.Add(width);
                    cmd.Parameters.Add(height);
                    cmd.Parameters.Add(new OracleParameter(":6", OracleDbType.NVarchar2) { Value = DBNull.Value });
                    cmd.Parameters.Add(frequency_band);
                    cmd.Parameters.Add(SpaceLibrary);
                    cmd.ExecuteNonQuery();
                    List<int> InsertedIds = new List<int>();
                    if (LastId != null)
                    {
                        OracleCommand InventoryCmd = connection.CreateCommand();
                        InventoryCmd.CommandType = CommandType.Text;
                        InventoryCmd.CommandText = $"Select \"Id\" From \"TLImwOtherLibrary\" where \"TLImwOtherLibrary\".\"Id\" > {LastId.Id}";
                        OracleDataReader Reader = InventoryCmd.ExecuteReader();
                        if (Reader.HasRows)
                        {
                            while (Reader.Read())
                            {
                                InsertedIds.Add(Reader.GetInt32(0));
                            }
                        }
                    }
                    else
                    {
                        OracleCommand InventoryCmd = connection.CreateCommand();
                        InventoryCmd.CommandType = CommandType.Text;
                        InventoryCmd.CommandText = $"Select \"Id\" From \"TLImwOtherLibrary\"";
                        OracleDataReader Reader = InventoryCmd.ExecuteReader();
                        if (Reader.HasRows)
                        {
                            while (Reader.Read())
                            {
                                InsertedIds.Add(Reader.GetInt32(0));
                            }
                        }
                    }
                    TLIhistory tLIhistory = new TLIhistory()
                    {
                        UserId = UserId,
                        TablesNameId = TableNameEntity.Id,
                        HistoryTypeId = 1,

                    };
                    _dbContext.TLIhistory.Add(tLIhistory);
                    _dbContext.SaveChanges();
                    var mwOtherLibraryList = new List<TLImwOtherLibrary>();

                    for (int i = 0; i < models.Count; i++)
                    {
                        mwOtherLibraryList.Add(new TLImwOtherLibrary
                        {
                            Id = InsertedIds[i],
                            Model = models[i],
                            Note = notes[i],
                            Length = lengths[i],
                            Height = heights[i],
                            Width = widths[i],
                            frequency_band = frequencybands[i],
                            SpaceLibrary = SpaceLibraries[i],

                        });
                    }


                    foreach (int recordId in InsertedIds)
                    {
                        var RecordName = mwOtherLibraryList.FirstOrDefault(x => x.Id == recordId);
                        if (RecordName != null)
                        {
                            var attributeNames = RecordName.GetType().GetProperties()
                                .Where(x =>
                                    (x.PropertyType.IsGenericType && x.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>) &&
                                        new Type[] { typeof(int), typeof(string), typeof(double), typeof(float), typeof(Single), typeof(bool), typeof(DateTime) }
                                        .Contains(x.PropertyType.GetGenericArguments()[0]))
                                    || new Type[] { typeof(int), typeof(string), typeof(double), typeof(bool), typeof(DateTime), typeof(float), typeof(Single) }
                                    .Contains(x.PropertyType)
                                )
                                .ToList();

                            foreach (var propertyInfo in attributeNames)
                            {

                                var propertyName = propertyInfo.Name;

                                var propertyValue = propertyInfo.GetValue(RecordName)?.ToString().Trim();

                                TLIhistoryDet tLIhistoryDet = new TLIhistoryDet()
                                {
                                    HistoryId = tLIhistory.Id,
                                    TablesNameId = TableNameEntity.Id,
                                    RecordId = recordId.ToString(),
                                    AttributeName = propertyName,
                                    NewValue = propertyValue
                                };


                                _dbContext.TLIhistoryDet.Add(tLIhistoryDet);
                            }
                        }
                    }
                    _dbContext.SaveChanges();

                    foreach (var DynamicAtt in DynamicAtts)
                    {
                        for (int k = 0; k < DynamicAtt.Item3.Count; k++)
                        {
                            if (!String.IsNullOrEmpty(DynamicAtt.Item3[k].ToString()))
                            {

                                var Message = _unitOfWork.CivilWithLegsRepository.CheckDynamicValidationAndDependenceDynamic(DynamicAtt.Item1, DynamicAtt.Item3[k], Convert.ToInt32(InsertedIds[k]), tLIhistory.Id, mwOtherLibraryList[0]).Message;
                                if (Message != "Success")
                                {
                                    UnsavedRows.Add(new KeyValuePair<int, string>(k + 2, Message + ' ' + $"in the Colum Name{ColName} in the Row Number {k + 2} "));
                                    goto ERROR;
                                }

                            }

                        }
                        //List<string> StringValues = new List<string>();
                        //List<double> DoubleValues = new List<double>();
                        //List<DateTime> DateTimeValues = new List<DateTime>();
                        //List<Int32> BooleanValues = new List<Int32>();
                        //List<int> DynamicAttIds = new List<int>();
                        //List<bool> disables = new List<bool>();
                        //List<int?> sideArmLibIds = new List<int?>();
                        //List<int?> sideArmLibraryIds = new List<int?>();
                        //List<int> InventoryIds = new List<int>();
                        //List<int> tablesNamesIds = new List<int>();
                        //if (DynamicAtt.Item2 == "string")
                        //{
                        //    for (int k = 0; k < DynamicAtt.Item3.Count; k++)
                        //    {
                        //        if (!String.IsNullOrEmpty(DynamicAtt.Item3[k].ToString()))
                        //        {
                        //            StringValues.Add(DynamicAtt.Item3[k].ToString());
                        //            DynamicAttIds.Add(DynamicAtt.Item1);
                        //            disables.Add(false);
                        //            sideArmLibIds.Add(null);
                        //            sideArmLibraryIds.Add(null);
                        //            InventoryIds.Add(Convert.ToInt32(InsertedIds[k]));
                        //            tablesNamesIds.Add(TableNameEntity.Id);
                        //        }
                        //    }
                        //}
                        //if (DynamicAtt.Item2 == "double")
                        //{
                        //    for (int k = 0; k < DynamicAtt.Item3.Count; k++)
                        //    {
                        //        if (!String.IsNullOrEmpty(DynamicAtt.Item3[k].ToString()))
                        //        {
                        //            DoubleValues.Add(Convert.ToInt32(DynamicAtt.Item3[k]));
                        //            DynamicAttIds.Add(DynamicAtt.Item1);
                        //            disables.Add(false);
                        //            sideArmLibIds.Add(null);
                        //            sideArmLibraryIds.Add(null);
                        //            InventoryIds.Add(Convert.ToInt32(InsertedIds[k]));
                        //            tablesNamesIds.Add(TableNameEntity.Id);
                        //        }
                        //    }
                        //}
                        //if (DynamicAtt.Item2 == "datetime")
                        //{
                        //    for (int k = 0; k < DynamicAtt.Item3.Count; k++)
                        //    {
                        //        if (!String.IsNullOrEmpty(DynamicAtt.Item3[k].ToString()))
                        //        {
                        //            DateTimeValues.Add(Convert.ToDateTime(DynamicAtt.Item3[k]));
                        //            DynamicAttIds.Add(DynamicAtt.Item1);
                        //            disables.Add(false);
                        //            sideArmLibIds.Add(null);
                        //            sideArmLibraryIds.Add(null);
                        //            InventoryIds.Add(Convert.ToInt32(InsertedIds[k]));
                        //            tablesNamesIds.Add(TableNameEntity.Id);
                        //        }
                        //    }
                        //}
                        //if (DynamicAtt.Item2 == "boolean")
                        //{
                        //    for (int k = 0; k < DynamicAtt.Item3.Count; k++)
                        //    {
                        //        if (!String.IsNullOrEmpty(DynamicAtt.Item3[k].ToString()))
                        //        {
                        //            bool BoolValue = Convert.ToBoolean(DynamicAtt.Item3[k]);
                        //            if (BoolValue == true)
                        //            {
                        //                BooleanValues.Add(1);
                        //            }
                        //            else
                        //            {
                        //                BooleanValues.Add(0);
                        //            }
                        //            DynamicAttIds.Add(DynamicAtt.Item1);
                        //            disables.Add(false);
                        //            sideArmLibIds.Add(null);
                        //            sideArmLibraryIds.Add(null);
                        //            InventoryIds.Add(Convert.ToInt32(InsertedIds[k]));
                        //            tablesNamesIds.Add(TableNameEntity.Id);
                        //        }
                        //    }
                        //}
                        ////connection.Open();
                        //OracleParameter value = new OracleParameter();
                        //if (DynamicAtt.Item2 == "string")
                        //{
                        //    value.OracleDbType = OracleDbType.NVarchar2;
                        //    value.Value = StringValues.ToArray();
                        //}
                        //else if (DynamicAtt.Item2 == "double")
                        //{
                        //    value.OracleDbType = OracleDbType.BinaryDouble;
                        //    value.Value = DoubleValues.ToArray();
                        //}
                        //else if (DynamicAtt.Item2 == "datetime")
                        //{
                        //    value.OracleDbType = OracleDbType.Date;
                        //    value.Value = DateTimeValues.ToArray();
                        //}
                        //else if (DynamicAtt.Item2 == "boolean")
                        //{
                        //    value.OracleDbType = OracleDbType.Int32;
                        //    value.Value = BooleanValues.ToArray();
                        //}
                        //OracleParameter DynamicAttId = new OracleParameter();
                        //DynamicAttId.OracleDbType = OracleDbType.Int32;
                        //DynamicAttId.Value = DynamicAttIds.ToArray();

                        //OracleParameter disable = new OracleParameter();
                        //disable.OracleDbType = OracleDbType.Boolean;
                        //disable.Value = disables.ToArray();

                        //OracleParameter sideArmLibId = new OracleParameter();
                        //sideArmLibId.OracleDbType = OracleDbType.Int32;
                        //sideArmLibId.Value = sideArmLibIds.ToArray();

                        //OracleParameter sideArmLibraryId = new OracleParameter();
                        //sideArmLibraryId.OracleDbType = OracleDbType.Int32;
                        //sideArmLibraryId.Value = sideArmLibraryIds.ToArray();

                        //OracleParameter InventoryId = new OracleParameter();
                        //InventoryId.OracleDbType = OracleDbType.Int32;
                        //InventoryId.Value = InventoryIds.ToArray();

                        //OracleParameter tablesNamesId = new OracleParameter();
                        //tablesNamesId.OracleDbType = OracleDbType.Int32;
                        //tablesNamesId.Value = tablesNamesIds.ToArray();

                        //OracleCommand dalvcmd = connection.CreateCommand();
                        //if (DynamicAtt.Item2 == "string")
                        //{
                        //    dalvcmd.CommandText = "INSERT INTO \"TLIdynamicAttLibValue\" (\"DynamicAttId\", \"InventoryId\", \"tablesNamesId\", \"ValueString\") VALUES ( :1, :2, :3, :4)";
                        //    dalvcmd.ArrayBindCount = StringValues.Count;
                        //}
                        //else if (DynamicAtt.Item2 == "double")
                        //{
                        //    dalvcmd.CommandText = "INSERT INTO \"TLIdynamicAttLibValue\" (\"DynamicAttId\", \"InventoryId\", \"tablesNamesId\", \"ValueDouble\") VALUES ( :1, :2, :3, :4)";
                        //    dalvcmd.ArrayBindCount = DoubleValues.Count;
                        //}
                        //else if (DynamicAtt.Item2 == "datetime")
                        //{
                        //    dalvcmd.CommandText = "INSERT INTO \"TLIdynamicAttLibValue\" (\"DynamicAttId\", \"InventoryId\", \"tablesNamesId\", \"ValueDateTime\") VALUES ( :1, :2, :3, :4)";
                        //    dalvcmd.ArrayBindCount = DateTimeValues.Count;
                        //}
                        //else if (DynamicAtt.Item2 == "boolean")
                        //{
                        //    dalvcmd.CommandText = "INSERT INTO \"TLIdynamicAttLibValue\" (\"DynamicAttId\", \"InventoryId\", \"tablesNamesId\", \"ValueBoolean\") VALUES ( :1, :2, :3, :4)";
                        //    dalvcmd.ArrayBindCount = BooleanValues.Count;
                        //}
                        ////dalvcmd.Parameters.Clear();
                        //dalvcmd.Parameters.Add(DynamicAttId);
                        //dalvcmd.Parameters.Add(InventoryId);
                        //dalvcmd.Parameters.Add(tablesNamesId);
                        //dalvcmd.Parameters.Add(value);
                        //dalvcmd.ExecuteNonQuery();
                    }
                    RecordId.AddRange(InsertedIds);
                }
            ERROR:;
            }
            catch (Exception err)
            {

                throw err;
            }
        }

        private void SavemwRFULibraryUsingOracleBulkCopy(int UserId, out List<int> RecordId, DataTable dt, ref List<KeyValuePair<int, string>> UnsavedRows, int ActColumns, int Columns, ExcelWorksheet sheet, List<KeyValuePair<string, List<DropDownListFilters>>> RelatedTables, TLItablesNames TableNameEntity, OracleConnection connection)
        {
            try
            {
                RecordId = new List<int>();
                List<string> Models = _unitOfWork.MW_RFULibraryRepository.GetSelect(x => x.Model).ToList();

                TLImwRFULibrary LastId = _serviceProvider.GetService<ApplicationDbContext>().
                    TLImwRFULibrary.OrderByDescending(a => a.Id).FirstOrDefault();

                List<TLIdynamicAtt> MW_RFULibraryDynamicAtts = _unitOfWork.DynamicAttRepository.GetIncludeWhere(x =>
                    x.tablesNamesId == TableNameEntity.Id && !x.disable, x => x.DataType).ToList();
                List<RFUType> RFUTypess = new List<RFUType>();
                List<string> models = new List<string>();
                List<string> notes = new List<string>();
                List<float> weights = new List<float>();
                List<float> lengths = new List<float>();
                List<float> widths = new List<float>();
                List<float> heights = new List<float>();
                List<string> sizes = new List<string>();
                List<Boolean> tx_parities = new List<Boolean>();
                List<string> frequencybands = new List<string>();
                List<string> FrequencyRanges = new List<string>();
                List<int> RFUTypes = new List<int>();
                List<string> VenferBoardNames = new List<string>();
                List<float> SpaceLibraries = new List<float>();
                List<int> diversityTypeIds = new List<int>();
                List<int> TLIboardTypeIds = new List<int>();
                List<Tuple<int, string, List<dynamic>>> DynamicAtts = new List<Tuple<int, string, List<dynamic>>>();
                TLIdynamicAtt DA = new TLIdynamicAtt();
                string ColName = null;
                //List<string> test = new List<string>();
                if (ActColumns > 0)
                {
                    for (int i = ActColumns; i <= Columns; i++)
                    {
                        ColName = sheet.Cells[1, i].Value.ToString();
                        DA = MW_RFULibraryDynamicAtts.FirstOrDefault(x => x.Key == ColName);
                        if (DA != null)
                            DynamicAtts.Add(new Tuple<int, string, List<dynamic>>(DA.Id, DA.DataType.Name.ToLower(), new List<dynamic>()));
                    }
                }
                bool test;
                for (int j = 0; j < dt.Rows.Count; j++)
                {
                    try
                    {
                        string model_test = null;

                        if (dt.Columns.Contains("Model"))
                        {
                            if ((Models.Find(x => x == Convert.ToString(dt.Rows[j]["Model"]))) != null)
                            {
                                UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"Model is already exist in the row {j + 2}"));
                                goto ERROR;
                            }
                            if (!String.IsNullOrEmpty(dt.Rows[j]["Model"].ToString()))
                            {
                                model_test = Convert.ToString(dt.Rows[j]["Model"]);
                            }
                            else
                            {
                                UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"Model can not to be null In The Row {j + 2}"));
                                goto ERROR;
                            }
                        }

                        string note_test = null;
                        if (dt.Columns.Contains("Note"))
                        {
                            if (!String.IsNullOrEmpty(dt.Rows[j]["Note"].ToString()))
                            {
                                note_test = Convert.ToString(dt.Rows[j]["Note"]);
                            }
                        }

                        float weight_test = 0;
                        if (dt.Columns.Contains("Weight"))
                        {
                            test = float.TryParse(dt.Rows[j]["Weight"].ToString(), out weight_test);
                            if (test == false)
                            {
                                UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"Weight Wrong Input DataType In The Row {j + 2}"));
                                goto ERROR;
                            }
                        }


                        float length_test = 0;
                        if (dt.Columns.Contains("Length"))
                        {
                            test = float.TryParse(dt.Rows[j]["Length"].ToString(), out length_test);
                            if (test == false)
                            {
                                UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"Length Wrong Input DataType In The Row {j + 2}"));
                                goto ERROR;
                            }
                        }

                        float width_test = 0;
                        if (dt.Columns.Contains("Width"))
                        {
                            test = float.TryParse(dt.Rows[j]["Width"].ToString(), out width_test);
                            if (test == false)
                            {
                                UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"Width Wrong Input DataType In The Row {j + 2}"));
                                goto ERROR;
                            }
                        }

                        float height_test = 0;
                        if (dt.Columns.Contains("Height"))
                        {
                            test = float.TryParse(dt.Rows[j]["Height"].ToString(), out height_test);
                            if (test == false)
                            {
                                UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"Height Wrong Input DataType In The Row {j + 2}"));
                                goto ERROR;
                            }
                        }

                        string size_test = null;
                        if (dt.Columns.Contains("size"))
                        {
                            if (!String.IsNullOrEmpty(dt.Rows[j]["size"].ToString()))
                            {
                                size_test = Convert.ToString(dt.Rows[j]["size"]);
                            }
                        }

                        Boolean tx_parity_test = false;
                        if (dt.Columns.Contains("tx_parity"))
                        {
                            test = Boolean.TryParse(dt.Rows[j]["tx_parity"].ToString(), out tx_parity_test);
                            if (test == false)
                            {
                                UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"tx_parity Wrong Input DataType In The Row {j + 2} must to be true or false"));
                                goto ERROR;
                            }
                        }

                        string FrequencyRange_test = null;
                        if (dt.Columns.Contains("FrequencyRange"))
                        {
                            if (!String.IsNullOrEmpty(dt.Rows[j]["FrequencyRange"].ToString()))
                            {
                                FrequencyRange_test = Convert.ToString(dt.Rows[j]["FrequencyRange"]);
                            }
                        }



                        string VenferBoardName_test = null;
                        if (dt.Columns.Contains("VenferBoardName"))
                        {
                            if (!String.IsNullOrEmpty(dt.Rows[j]["VenferBoardName"].ToString()))
                            {
                                VenferBoardName_test = Convert.ToString(dt.Rows[j]["VenferBoardName"]);
                            }
                        }

                        string frequencyband_test = null;
                        if (dt.Columns.Contains("frequency_band"))
                        {
                            if (!String.IsNullOrEmpty(dt.Rows[j]["frequency_band"].ToString()))
                            {
                                frequencyband_test = Convert.ToString(dt.Rows[j]["frequency_band"]);
                            }
                        }

                        float spacelibrary_test = 0;
                      
                        if (length_test == 0)
                        {
                            UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"length must to be bigger of zero in the row {j + 2}"));
                            goto ERROR;
                        }
                        if (width_test == 0)
                        {
                            UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"width must to be bigger of zero in the roww {j + 2}"));
                            goto ERROR;
                        }
                        else
                        {
                            spacelibrary_test = length_test * width_test;
                        }

                            
                        
                        int RFUType_test = 0;

                        if (dt.Columns.Contains("RFUType"))
                        {
                            string rfuTypeValue = dt.Rows[j]["RFUType"].ToString();

                            if (!String.IsNullOrEmpty(rfuTypeValue))
                            {
                                var relatedRFUType = RelatedTables
                                    .Where(x => x.Key == "RFUType")
                                    .Select(x => x.Value)
                                    .FirstOrDefault()
                                    .Where(x => x.Value == rfuTypeValue)
                                    .FirstOrDefault();

                                if (relatedRFUType != null)
                                {
                                    RFUType_test = Convert.ToInt32(relatedRFUType.Id);

                                    var rfuTypeList = Enum.GetValues(typeof(RFUType))
                                        .Cast<RFUType>()
                                        .FirstOrDefault(e => (int)e == RFUType_test);

                                    if (rfuTypeList != null)
                                    {
                                        RFUTypess.Add(rfuTypeList);
                                    }
                                }
                                else
                                {
                                    UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"Invalid RFUType value '{rfuTypeValue}' in Row {j + 2}"));
                                    goto ERROR;
                                }
                            }
                            else
                            {
                                UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"RFUType cannot be null in Row {j + 2}"));
                                goto ERROR;
                            }
                        }

                        int diversityTypeId_test = 0;
                        if (dt.Columns.Contains("diversityTypeId"))
                        {
                            if (!String.IsNullOrEmpty(dt.Rows[j]["diversityTypeId"].ToString()))
                            {
                                var diversityType = RelatedTables.Where(x => x.Key == "diversityTypeId").Select(x => x.Value).FirstOrDefault().Where(x => x.Value == (dt.Rows[j]["diversityTypeId"]).ToString()).FirstOrDefault();
                                diversityTypeId_test = Convert.ToInt32(diversityType.Id);
                            }
                            else
                            {
                                UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"diversityTypeId can not to be null In The Row {j + 2}"));
                                goto ERROR;
                            }
                        }

                        int TLIboardTypeId_test = 0;
                        if (dt.Columns.Contains("boardTypeId"))
                        {
                            if (!String.IsNullOrEmpty(dt.Rows[j]["boardTypeId"].ToString()))
                            {
                                var TLIboardType = RelatedTables.Where(x => x.Key == "boardTypeId").Select(x => x.Value).FirstOrDefault().Where(x => x.Value == (dt.Rows[j]["boardTypeId"]).ToString()).FirstOrDefault();
                                TLIboardTypeId_test = Convert.ToInt32(TLIboardType.Id);
                            }
                            else
                            {
                                UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"boardTypeId can not to be null In The Row {j + 2}"));
                                goto ERROR;
                            }
                        }

                        List<dynamic> DynamicAttList = new List<dynamic>();
                        if (ActColumns > 0)
                        {
                            for (int i = ActColumns; i <= Columns; i++)
                            {
                                string StringDynamicAtt = string.Empty;
                                double? DoubleDynamicAtt = null;
                                DateTime? DateTimeDynamicAtt = null;
                                Boolean? BooleanDynamicAtt = null;
                                ColName = sheet.Cells[1, i].Value.ToString();
                                DA = MW_RFULibraryDynamicAtts.FirstOrDefault(x => x.Key == ColName);
                                //Check dynamic attribute if required or not
                                if (DA != null)
                                {
                                    double double_Test = 0;
                                    DateTime datetime_Test = DateTime.Now;
                                    Boolean boolean_Test = false;
                                    if (DA.Required == true)
                                    {
                                        //if somthing wrong then add row to UnsavedRows
                                        if (String.IsNullOrEmpty(dt.Rows[j][ColName].ToString()))
                                        {
                                            UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"Dynamic Attribute ({ColName}) is required"));
                                            goto ERROR;
                                        }
                                        else
                                        {
                                            if (DA.DataType.Name.ToLower() == "string")
                                            {

                                                StringDynamicAtt = Convert.ToString(dt.Rows[j][ColName]);
                                            }
                                            else if (DA.DataType.Name.ToLower() == "double" || DA.DataType.Name.ToLower() == "int")
                                            {
                                                test = double.TryParse(dt.Rows[j][ColName].ToString(), out double_Test);
                                                if (test == false)
                                                {
                                                    UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"{ColName} Dynamic Attribute Wrong Input DataType in the row {j + 2}"));
                                                    goto ERROR;
                                                }
                                                DoubleDynamicAtt = double_Test;
                                            }
                                            else if (DA.DataType.Name.ToLower() == "datetime")
                                            {
                                                test = DateTime.TryParse(dt.Rows[j][ColName].ToString(), out datetime_Test);
                                                if (test == false)
                                                {
                                                    UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"{ColName} Dynamic Attribute Wrong Input DataType in the row {j + 2}"));
                                                    goto ERROR;
                                                }
                                                DateTimeDynamicAtt = datetime_Test;
                                            }
                                            else if (DA.DataType.Name.ToLower() == "bool")
                                            {
                                                test = Boolean.TryParse(dt.Rows[j][ColName].ToString(), out boolean_Test);
                                                if (test == false)
                                                {
                                                    UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"{ColName} Dynamic Attribute Wrong Input DataType in the row {j + 2}"));
                                                    goto ERROR;
                                                }
                                                BooleanDynamicAtt = boolean_Test;
                                            }


                                        }
                                    }
                                    else
                                    {
                                        if (!String.IsNullOrEmpty(dt.Rows[j][ColName].ToString()))
                                        {
                                            if (DA.DataType.Name.ToLower() == "string")
                                            {
                                                StringDynamicAtt = Convert.ToString(dt.Rows[j][ColName]);
                                            }
                                            else if (DA.DataType.Name.ToLower() == "double" || DA.DataType.Name.ToLower() == "int")
                                            {
                                                test = double.TryParse(dt.Rows[j][ColName].ToString(), out double_Test);
                                                if (test == false)
                                                {
                                                    UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"{ColName} Dynamic Attribute Wrong Input DataType in the row {j + 2}"));
                                                    goto ERROR;
                                                }
                                                DoubleDynamicAtt = double_Test;
                                            }
                                            else if (DA.DataType.Name.ToLower() == "datetime")
                                            {
                                                test = DateTime.TryParse(dt.Rows[j][ColName].ToString(), out datetime_Test);
                                                if (test == false)
                                                {
                                                    UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"{ColName} Dynamic Attribute Wrong Input DataType in the row {j + 2}"));
                                                    goto ERROR;
                                                }
                                                DateTimeDynamicAtt = datetime_Test;

                                            }
                                            else if (DA.DataType.Name.ToLower() == "bool")
                                            {
                                                test = Boolean.TryParse(dt.Rows[j][ColName].ToString(), out boolean_Test);
                                                if (test == false)
                                                {
                                                    UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"{ColName} Dynamic Attribute Wrong Input DataType in the row {j + 2}"));
                                                    goto ERROR;
                                                }
                                                BooleanDynamicAtt = boolean_Test;
                                            }

                                        }
                                    }
                                    if (DA.DataType.Name.ToLower() == "string")
                                    {
                                        DynamicAttList.Add(StringDynamicAtt);
                                    }
                                    else if (DA.DataType.Name.ToLower() == "double" || DA.DataType.Name.ToLower() == "int")
                                    {
                                        DynamicAttList.Add(DoubleDynamicAtt);
                                    }
                                    else if (DA.DataType.Name.ToLower() == "datetime")
                                    {
                                        DynamicAttList.Add(DateTimeDynamicAtt);
                                    }
                                    else if (DA.DataType.Name.ToLower() == "bool")
                                    {
                                        DynamicAttList.Add(BooleanDynamicAtt);
                                    }


                                }
                            }
                        }
                        models.Add(model_test != null ? model_test : null);
                        notes.Add(note_test != null ? note_test : null);
                        weights.Add(weight_test != 0 ? weight_test : 0);
                        lengths.Add(length_test != 0 ? length_test : 0);
                        widths.Add(width_test != 0 ? width_test : 0);
                        heights.Add(height_test != 0 ? height_test : 0);
                        sizes.Add(size_test != null ? size_test : null);
                        tx_parities.Add(tx_parity_test);
                        frequencybands.Add(frequencyband_test != null ? frequencyband_test : null);
                        SpaceLibraries.Add(spacelibrary_test != 0 ? spacelibrary_test : 0);
                        FrequencyRanges.Add(frequencyband_test != null ? frequencyband_test : null);
                        RFUTypes.Add(RFUType_test != 0 ? RFUType_test : 0);
                        VenferBoardNames.Add(VenferBoardName_test != null ? VenferBoardName_test : null);
                        diversityTypeIds.Add(diversityTypeId_test != 0 ? diversityTypeId_test : 0);
                        TLIboardTypeIds.Add(TLIboardTypeId_test != 0 ? TLIboardTypeId_test : 0);
                        for (int f = 0; f < DynamicAttList.Count; f++)
                        {
                            //var h = DynamicAtts[f].Value;
                            DynamicAtts[f].Item3.Add(DynamicAttList[f]);
                            //h.Add(DynamicAttList[f]);
                        }


                    }
                    catch (Exception err)
                    {

                        // UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, "Wrong Input DataType"));
                    }
                }
                if (models.Count > 0)
                {
                    OracleParameter model = new OracleParameter();
                    model.OracleDbType = OracleDbType.NVarchar2;
                    model.Value = models.ToArray();

                    OracleParameter note = new OracleParameter();
                    note.OracleDbType = OracleDbType.NVarchar2;
                    note.Value = notes.ToArray();

                    OracleParameter Weight = new OracleParameter();
                    Weight.OracleDbType = OracleDbType.BinaryFloat;
                    Weight.Value = weights.ToArray();

                    OracleParameter Length = new OracleParameter();
                    Length.OracleDbType = OracleDbType.BinaryFloat;
                    Length.Value = lengths.ToArray();

                    OracleParameter Width = new OracleParameter();
                    Width.OracleDbType = OracleDbType.BinaryFloat;
                    Width.Value = widths.ToArray();

                    OracleParameter Height = new OracleParameter();
                    Height.OracleDbType = OracleDbType.BinaryFloat;
                    Height.Value = heights.ToArray();

                    OracleParameter size = new OracleParameter();
                    size.OracleDbType = OracleDbType.NVarchar2;
                    size.Value = sizes.ToArray();

                    OracleParameter tx_parity = new OracleParameter();
                    tx_parity.OracleDbType = OracleDbType.Int32;
                    tx_parity.Value = tx_parities.Select(v => v ? 1 : 0).ToArray();

                    OracleParameter frequency_band = new OracleParameter();
                    frequency_band.OracleDbType = OracleDbType.NVarchar2;
                    frequency_band.Value = frequencybands.ToArray();

                    OracleParameter SpaceLibrary = new OracleParameter();
                    SpaceLibrary.OracleDbType = OracleDbType.BinaryFloat;
                    SpaceLibrary.Value = SpaceLibraries.ToArray();

                    OracleParameter diversityTypeId = new OracleParameter();
                    diversityTypeId.OracleDbType = OracleDbType.Int32;
                    diversityTypeId.Value = diversityTypeIds.ToArray();

                    OracleParameter TLIboardTypeId = new OracleParameter();
                    TLIboardTypeId.OracleDbType = OracleDbType.Int32;
                    TLIboardTypeId.Value = TLIboardTypeIds.ToArray();

                    OracleParameter FrequencyRangeS = new OracleParameter();
                    FrequencyRangeS.OracleDbType = OracleDbType.NVarchar2;
                    FrequencyRangeS.Value = FrequencyRanges.ToArray();

                    OracleParameter RFUTypeS = new OracleParameter();
                    RFUTypeS.OracleDbType = OracleDbType.NVarchar2;
                    RFUTypeS.Value = RFUTypes.ToArray();

                    OracleParameter VenferBoardNameS = new OracleParameter();
                    VenferBoardNameS.OracleDbType = OracleDbType.NVarchar2;
                    VenferBoardNameS.Value = VenferBoardNames.ToArray();

                    // create command and set properties
                    OracleCommand cmd = connection.CreateCommand();
                    cmd.CommandText = "INSERT INTO \"TLImwRFULibrary\" (\"Model\", \"Note\", \"Weight\", \"L_W_H\", \"Length\", \"Width\", \"Height\", \"size\", \"tx_parity\", \"frequency_band\", \"SpaceLibrary\", \"diversityTypeId\", \"boardTypeId\",\"FrequencyRange\",\"RFUType\",\"VenferBoardName\") VALUES ( :1, :2, :3, :4, :5, :6, :7, :8, :9, :10, :11, :12, :13 ,:14 ,:15 ,:16)";
                    cmd.ArrayBindCount = models.Count;
                    cmd.Parameters.Add(model);
                    cmd.Parameters.Add(note);
                    cmd.Parameters.Add(Weight);
                    cmd.Parameters.Add(new OracleParameter(":4", OracleDbType.NVarchar2) { Value = DBNull.Value });
                    cmd.Parameters.Add(Length);
                    cmd.Parameters.Add(Width);
                    cmd.Parameters.Add(Height);
                    cmd.Parameters.Add(size);
                    cmd.Parameters.Add(tx_parity);
                    cmd.Parameters.Add(frequency_band);
                    cmd.Parameters.Add(SpaceLibrary);
                    cmd.Parameters.Add(diversityTypeId);
                    cmd.Parameters.Add(TLIboardTypeId);
                    cmd.Parameters.Add(FrequencyRangeS);
                    cmd.Parameters.Add(RFUTypeS);
                    cmd.Parameters.Add(VenferBoardNameS);
                    cmd.ExecuteNonQuery();
                    //connection.Close();
                    List<int> InsertedIds = new List<int>();
                    if (LastId != null)
                    {
                        OracleCommand InventoryCmd = connection.CreateCommand();
                        InventoryCmd.CommandType = CommandType.Text;
                        InventoryCmd.CommandText = $"Select \"Id\" From \"TLImwRFULibrary\" where \"TLImwRFULibrary\".\"Id\" > {LastId.Id}";
                        OracleDataReader Reader = InventoryCmd.ExecuteReader();
                        if (Reader.HasRows)
                        {
                            while (Reader.Read())
                            {
                                InsertedIds.Add(Reader.GetInt32(0));
                            }
                        }
                    }
                    else
                    {
                        OracleCommand InventoryCmd = connection.CreateCommand();
                        InventoryCmd.CommandType = CommandType.Text;
                        InventoryCmd.CommandText = $"Select \"Id\" From \"TLImwRFULibrary\"";
                        OracleDataReader Reader = InventoryCmd.ExecuteReader();
                        if (Reader.HasRows)
                        {
                            while (Reader.Read())
                            {
                                InsertedIds.Add(Reader.GetInt32(0));
                            }
                        }
                    }
                    TLIhistory tLIhistory = new TLIhistory()
                    {
                        UserId = UserId,
                        TablesNameId = TableNameEntity.Id,
                        HistoryTypeId = 1,

                    };
                    _dbContext.TLIhistory.Add(tLIhistory);
                    _dbContext.SaveChanges();
                    var mwRFULibraryList = new List<TLImwRFULibrary>();

                    for (int i = 0; i < models.Count; i++)
                    {
                        mwRFULibraryList.Add(new TLImwRFULibrary
                        {
                            Id = InsertedIds[i],
                            Model = models[i],
                            Note = notes[i],
                            Weight = weights[i],
                            Height = heights[i],
                            Length = lengths[i],
                            Width = widths[i],
                            tx_parity = tx_parities[i],
                            size = sizes[i],
                            frequency_band = frequencybands[i],
                            FrequencyRange = FrequencyRanges[i],
                            RFUType = RFUTypess[i],
                            VenferBoardName = VenferBoardNames[i],
                            SpaceLibrary = SpaceLibraries[i],
                            boardTypeId = TLIboardTypeIds[i],
                            diversityTypeId = diversityTypeIds[i],

                        });
                    }


                    foreach (int recordId in InsertedIds)
                    {
                        var RecordName = mwRFULibraryList.FirstOrDefault(x => x.Id == recordId);
                        if (RecordName != null)
                        {
                            var attributeNames = RecordName.GetType().GetProperties()
                                .Where(x =>
                                    (x.PropertyType.IsGenericType && x.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>) &&
                                        new Type[] { typeof(int), typeof(string), typeof(double), typeof(float), typeof(Single), typeof(bool), typeof(DateTime) }
                                        .Contains(x.PropertyType.GetGenericArguments()[0]))
                                    || new Type[] { typeof(int), typeof(string), typeof(double), typeof(bool), typeof(DateTime), typeof(float), typeof(Single) }
                                    .Contains(x.PropertyType)
                                )
                                .ToList();

                            foreach (var propertyInfo in attributeNames)
                            {

                                var propertyName = propertyInfo.Name;

                                var propertyValue = propertyInfo.GetValue(RecordName)?.ToString().Trim();

                                TLIhistoryDet tLIhistoryDet = new TLIhistoryDet()
                                {
                                    HistoryId = tLIhistory.Id,
                                    TablesNameId = TableNameEntity.Id,
                                    RecordId = recordId.ToString(),
                                    AttributeName = propertyName,
                                    NewValue = propertyValue
                                };


                                _dbContext.TLIhistoryDet.Add(tLIhistoryDet);
                            }
                        }
                    }
                    _dbContext.SaveChanges();

                    foreach (var DynamicAtt in DynamicAtts)
                    {
                        for (int k = 0; k < DynamicAtt.Item3.Count; k++)
                        {
                            if (!String.IsNullOrEmpty(DynamicAtt.Item3[k].ToString()))
                            {

                                var Message = _unitOfWork.CivilWithLegsRepository.CheckDynamicValidationAndDependenceDynamic(DynamicAtt.Item1, DynamicAtt.Item3[k], Convert.ToInt32(InsertedIds[k]), tLIhistory.Id, mwRFULibraryList[0]).Message;
                                if (Message != "Success")
                                {
                                    UnsavedRows.Add(new KeyValuePair<int, string>(k + 2, Message + ' ' + $"in the Colum Name{ColName} in the Row Number {k + 2} "));
                                    goto ERROR;
                                }

                            }

                        }
                        //List<string> StringValues = new List<string>();
                        //List<double> DoubleValues = new List<double>();
                        //List<DateTime> DateTimeValues = new List<DateTime>();
                        //List<Int32> BooleanValues = new List<Int32>();
                        //List<int> DynamicAttIds = new List<int>();
                        //List<bool> disables = new List<bool>();
                        //List<int?> sideArmLibIds = new List<int?>();
                        //List<int?> sideArmLibraryIds = new List<int?>();
                        //List<int> InventoryIds = new List<int>();
                        //List<int> tablesNamesIds = new List<int>();
                        //if (DynamicAtt.Item2 == "string")
                        //{
                        //    for (int k = 0; k < DynamicAtt.Item3.Count; k++)
                        //    {
                        //        if (!String.IsNullOrEmpty(DynamicAtt.Item3[k].ToString()))
                        //        {
                        //            StringValues.Add(DynamicAtt.Item3[k].ToString());
                        //            DynamicAttIds.Add(DynamicAtt.Item1);
                        //            disables.Add(false);
                        //            sideArmLibIds.Add(null);
                        //            sideArmLibraryIds.Add(null);
                        //            InventoryIds.Add(Convert.ToInt32(InsertedIds[k]));
                        //            tablesNamesIds.Add(TableNameEntity.Id);
                        //        }
                        //    }
                        //}
                        //if (DynamicAtt.Item2 == "double" || DynamicAtt.Item2 == "int")
                        //{
                        //    for (int k = 0; k < DynamicAtt.Item3.Count; k++)
                        //    {
                        //        if (!String.IsNullOrEmpty(DynamicAtt.Item3[k].ToString()))
                        //        {
                        //            DoubleValues.Add(Convert.ToInt32(DynamicAtt.Item3[k]));
                        //            DynamicAttIds.Add(DynamicAtt.Item1);
                        //            disables.Add(false);
                        //            sideArmLibIds.Add(null);
                        //            sideArmLibraryIds.Add(null);
                        //            InventoryIds.Add(Convert.ToInt32(InsertedIds[k]));
                        //            tablesNamesIds.Add(TableNameEntity.Id);
                        //        }
                        //    }
                        //}
                        //if (DynamicAtt.Item2 == "datetime")
                        //{
                        //    for (int k = 0; k < DynamicAtt.Item3.Count; k++)
                        //    {
                        //        if (!String.IsNullOrEmpty(DynamicAtt.Item3[k].ToString()))
                        //        {
                        //            DateTimeValues.Add(Convert.ToDateTime(DynamicAtt.Item3[k]));
                        //            DynamicAttIds.Add(DynamicAtt.Item1);
                        //            disables.Add(false);
                        //            sideArmLibIds.Add(null);
                        //            sideArmLibraryIds.Add(null);
                        //            InventoryIds.Add(Convert.ToInt32(InsertedIds[k]));
                        //            tablesNamesIds.Add(TableNameEntity.Id);
                        //        }
                        //    }
                        //}
                        //if (DynamicAtt.Item2 == "boolean")
                        //{
                        //    for (int k = 0; k < DynamicAtt.Item3.Count; k++)
                        //    {
                        //        if (!String.IsNullOrEmpty(DynamicAtt.Item3[k].ToString()))
                        //        {
                        //            bool BoolValue = Convert.ToBoolean(DynamicAtt.Item3[k]);
                        //            if (BoolValue == true)
                        //            {
                        //                BooleanValues.Add(1);
                        //            }
                        //            else
                        //            {
                        //                BooleanValues.Add(0);
                        //            }
                        //            DynamicAttIds.Add(DynamicAtt.Item1);
                        //            disables.Add(false);
                        //            sideArmLibIds.Add(null);
                        //            sideArmLibraryIds.Add(null);
                        //            InventoryIds.Add(Convert.ToInt32(InsertedIds[k]));
                        //            tablesNamesIds.Add(TableNameEntity.Id);
                        //        }
                        //    }
                        //}
                        ////connection.Open();
                        //OracleParameter value = new OracleParameter();
                        //if (DynamicAtt.Item2 == "string")
                        //{
                        //    value.OracleDbType = OracleDbType.NVarchar2;
                        //    value.Value = StringValues.ToArray();
                        //}
                        //else if (DynamicAtt.Item2 == "double" || DynamicAtt.Item2 == "int")
                        //{
                        //    value.OracleDbType = OracleDbType.BinaryDouble;
                        //    value.Value = DoubleValues.ToArray();
                        //}
                        //else if (DynamicAtt.Item2 == "datetime")
                        //{
                        //    value.OracleDbType = OracleDbType.Date;
                        //    value.Value = DateTimeValues.ToArray();
                        //}
                        //else if (DynamicAtt.Item2 == "boolean")
                        //{
                        //    value.OracleDbType = OracleDbType.Int32;
                        //    value.Value = BooleanValues.ToArray();
                        //}
                        //OracleParameter DynamicAttId = new OracleParameter();
                        //DynamicAttId.OracleDbType = OracleDbType.Int32;
                        //DynamicAttId.Value = DynamicAttIds.ToArray();

                        //OracleParameter disable = new OracleParameter();
                        //disable.OracleDbType = OracleDbType.Boolean;
                        //disable.Value = disables.ToArray();

                        //OracleParameter sideArmLibId = new OracleParameter();
                        //sideArmLibId.OracleDbType = OracleDbType.Int32;
                        //sideArmLibId.Value = sideArmLibIds.ToArray();

                        //OracleParameter sideArmLibraryId = new OracleParameter();
                        //sideArmLibraryId.OracleDbType = OracleDbType.Int32;
                        //sideArmLibraryId.Value = sideArmLibraryIds.ToArray();

                        //OracleParameter InventoryId = new OracleParameter();
                        //InventoryId.OracleDbType = OracleDbType.Int32;
                        //InventoryId.Value = InventoryIds.ToArray();

                        //OracleParameter tablesNamesId = new OracleParameter();
                        //tablesNamesId.OracleDbType = OracleDbType.Int32;
                        //tablesNamesId.Value = tablesNamesIds.ToArray();

                        //OracleCommand dalvcmd = connection.CreateCommand();
                        //if (DynamicAtt.Item2 == "string")
                        //{
                        //    dalvcmd.CommandText = "INSERT INTO \"TLIdynamicAttLibValue\" (\"DynamicAttId\", \"InventoryId\", \"tablesNamesId\", \"ValueString\") VALUES ( :1, :2, :3, :4)";
                        //    dalvcmd.ArrayBindCount = StringValues.Count;
                        //}
                        //else if (DynamicAtt.Item2 == "double" || DynamicAtt.Item2 == "int")
                        //{
                        //    dalvcmd.CommandText = "INSERT INTO \"TLIdynamicAttLibValue\" (\"DynamicAttId\", \"InventoryId\", \"tablesNamesId\", \"ValueDouble\") VALUES ( :1, :2, :3, :4)";
                        //    dalvcmd.ArrayBindCount = DoubleValues.Count;
                        //}
                        //else if (DynamicAtt.Item2 == "datetime")
                        //{
                        //    dalvcmd.CommandText = "INSERT INTO \"TLIdynamicAttLibValue\" (\"DynamicAttId\", \"InventoryId\", \"tablesNamesId\", \"ValueDateTime\") VALUES ( :1, :2, :3, :4)";
                        //    dalvcmd.ArrayBindCount = DateTimeValues.Count;
                        //}
                        //else if (DynamicAtt.Item2 == "boolean")
                        //{
                        //    dalvcmd.CommandText = "INSERT INTO \"TLIdynamicAttLibValue\" (\"DynamicAttId\", \"InventoryId\", \"tablesNamesId\", \"ValueBoolean\") VALUES ( :1, :2, :3, :4)";
                        //    dalvcmd.ArrayBindCount = BooleanValues.Count;
                        //}
                        ////dalvcmd.Parameters.Clear();
                        //dalvcmd.Parameters.Add(DynamicAttId);
                        //dalvcmd.Parameters.Add(InventoryId);
                        //dalvcmd.Parameters.Add(tablesNamesId);
                        //dalvcmd.Parameters.Add(value);
                        //dalvcmd.ExecuteNonQuery();
                    }
                    RecordId.AddRange(InsertedIds);
                }
            ERROR:;
            }
            catch (Exception err)
            {

                throw err;
            }
        }

        private void SavepowerLibraryUsingOracleBulkCopy(int UserId, out List<int> RecordId, DataTable dt, ref List<KeyValuePair<int, string>> UnsavedRows, int ActColumns, int Columns, ExcelWorksheet sheet, List<KeyValuePair<string, List<DropDownListFilters>>> RelatedTables, TLItablesNames TableNameEntity, OracleConnection connection)
        {
            try
            {
                RecordId = new List<int>();
                List<string> Models = _unitOfWork.PowerLibraryRepository.GetSelect(x => x.Model).ToList();

                TLIpowerLibrary LastId = _serviceProvider.GetService<ApplicationDbContext>().
                    TLIpowerLibrary.OrderByDescending(a => a.Id).FirstOrDefault();

                List<TLIdynamicAtt> PowerLibraryDynamicAtts = _unitOfWork.DynamicAttRepository.GetIncludeWhere(x =>
                    x.tablesNamesId == TableNameEntity.Id && !x.disable, x => x.DataType).ToList();

                List<string> models = new List<string>();
                List<string> notes = new List<string>();
                List<float> weights = new List<float>();
                List<float> Heights = new List<float>();
                List<float> widths = new List<float>();
                List<float> lengths = new List<float>();
                List<float> sizes = new List<float>();
                List<float> depths = new List<float>();
                List<float> SpaceLibraries = new List<float>();
                List<string> FrequencyRanges = new List<string>();
                List<string> ChannelBandWidths = new List<string>();
                List<string> BandWidths = new List<string>();
                List<string> Types = new List<string>();
                List<Tuple<int, string, List<dynamic>>> DynamicAtts = new List<Tuple<int, string, List<dynamic>>>();
                TLIdynamicAtt DA = new TLIdynamicAtt();
                string ColName = null;
                //List<string> test = new List<string>();
                if (ActColumns > 0)
                {
                    for (int i = ActColumns; i <= Columns; i++)
                    {
                        ColName = sheet.Cells[1, i].Value.ToString();
                        DA = PowerLibraryDynamicAtts.FirstOrDefault(x => x.Key == ColName);
                        if (DA != null)
                            DynamicAtts.Add(new Tuple<int, string, List<dynamic>>(DA.Id, DA.DataType.Name.ToLower(), new List<dynamic>()));
                    }
                }
                bool test;
                for (int j = 0; j < dt.Rows.Count; j++)
                {
                    try
                    {
                        string model_test = null;

                        if (dt.Columns.Contains("Model"))
                        {
                            if ((Models.Find(x => x == Convert.ToString(dt.Rows[j]["Model"]))) != null)
                            {
                                UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"Model is already exist in the Row {j + 2}"));
                                goto ERROR;
                            }
                            if (!String.IsNullOrEmpty(dt.Rows[j]["Model"].ToString()))
                            {
                                model_test = Convert.ToString(dt.Rows[j]["Model"]);
                            }
                            else
                            {
                                UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"Model can not to be null In The Row {j + 2}"));
                                goto ERROR;
                            }
                        }

                        string note_test = null;
                        if (dt.Columns.Contains("Note"))
                        {
                            if (!String.IsNullOrEmpty(dt.Rows[j]["Note"].ToString()))
                            {
                                note_test = Convert.ToString(dt.Rows[j]["Note"]);
                            }
                        }

                        string FrequencyRange_test = null;
                        if (dt.Columns.Contains("FrequencyRange"))
                        {
                            if (!String.IsNullOrEmpty(dt.Rows[j]["FrequencyRange"].ToString()))
                            {
                                FrequencyRange_test = Convert.ToString(dt.Rows[j]["FrequencyRange"]);
                            }
                        }

                        string Type_test = null;
                        if (dt.Columns.Contains("Type"))
                        {
                            if (!String.IsNullOrEmpty(dt.Rows[j]["Type"].ToString()))
                            {
                                Type_test = Convert.ToString(dt.Rows[j]["Type"]);
                            }
                        }

                        string ChannelBandWidths_test = null;
                        if (dt.Columns.Contains("L_W_H"))
                        {
                            if (!String.IsNullOrEmpty(dt.Rows[j]["L_W_H"].ToString()))
                            {
                                ChannelBandWidths_test = Convert.ToString(dt.Rows[j]["L_W_H"]);
                            }
                        }
                        string BandWidths_test = null;
                        if (dt.Columns.Contains("L_W_H"))
                        {
                            if (!String.IsNullOrEmpty(dt.Rows[j]["L_W_H"].ToString()))
                            {
                                BandWidths_test = Convert.ToString(dt.Rows[j]["L_W_H"]);
                            }
                        }
                        float weight_test = 0;
                        if (dt.Columns.Contains("Weight"))
                        {
                            test = float.TryParse(dt.Rows[j]["Weight"].ToString(), out weight_test);
                            if (test == false)
                            {
                                UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"Weight Wrong Input DataType In The Row {j + 2}"));
                                goto ERROR;
                            }
                        }

                        float Height_test = 0;
                        if (dt.Columns.Contains("Height"))
                        {
                            test = float.TryParse(dt.Rows[j]["Height"].ToString(), out Height_test);
                            if (test == false)
                            {
                                UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"Height Wrong Input DataType In The Row {j + 2}"));
                                goto ERROR;
                            }
                        }

                        float width_test = 0;
                        if (dt.Columns.Contains("Width"))
                        {
                            test = float.TryParse(dt.Rows[j]["Width"].ToString(), out width_test);
                            if (test == false)
                            {
                                UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"Width Wrong Input DataType In The Row {j + 2}"));
                                goto ERROR;
                            }
                        }

                        //add size
                        float size_test = 0;
                        if (dt.Columns.Contains("Size"))
                        {
                            test = float.TryParse(dt.Rows[j]["Size"].ToString(), out size_test);
                            if (test == false)
                            {
                                UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"Size Wrong Input DataType In The Row {j + 2}"));
                                goto ERROR;
                            }
                        }

                        float length_test = 0;
                        if (dt.Columns.Contains("Length"))
                        {
                            test = float.TryParse(dt.Rows[j]["Length"].ToString(), out length_test);
                            if (test == false)
                            {
                                UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"Length Wrong Input DataType In The Row {j + 2}"));
                                goto ERROR;
                            }
                        }

                        float depth_test = 0;
                        if (dt.Columns.Contains("Depth"))
                        {
                            test = float.TryParse(dt.Rows[j]["Depth"].ToString(), out depth_test);
                            if (test == false)
                            {
                                UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"Depth Wrong Input DataTyp In The Row {j + 2}"));
                                goto ERROR;
                            }
                        }

                        float spacelibrary_test = 0;
                       
                        if (length_test == 0)
                        {
                            UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"length must to be bigger of zero in the row {j + 2}"));
                            goto ERROR;
                        }
                        if (width_test == 0)
                        {
                            UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"width must to be bigger of zero in the row {j + 2}"));
                            goto ERROR;
                        }
                        else
                        {
                            spacelibrary_test = length_test * width_test;
                        }

                            
                        

                        List<dynamic> DynamicAttList = new List<dynamic>();
                        if (ActColumns > 0)
                        {
                            for (int i = ActColumns; i <= Columns; i++)
                            {
                                string StringDynamicAtt = string.Empty;
                                double? DoubleDynamicAtt = null;
                                DateTime? DateTimeDynamicAtt = null;
                                Boolean? BooleanDynamicAtt = null;
                                ColName = sheet.Cells[1, i].Value.ToString();
                                DA = PowerLibraryDynamicAtts.FirstOrDefault(x => x.Key == ColName);
                                //Check dynamic attribute if required or not
                                if (DA != null)
                                {
                                    double double_Test = 0;
                                    DateTime datetime_Test = DateTime.Now;
                                    Boolean boolean_Test = false;
                                    if (DA.Required == true)
                                    {
                                        //if somthing wrong then add row to UnsavedRows
                                        if (String.IsNullOrEmpty(dt.Rows[j][ColName].ToString()))
                                        {
                                            UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"Dynamic Attribute ({ColName}) is required"));
                                            goto ERROR;
                                        }
                                        else
                                        {
                                            if (DA.DataType.Name.ToLower() == "string")
                                            {

                                                StringDynamicAtt = Convert.ToString(dt.Rows[j][ColName]);
                                            }
                                            else if (DA.DataType.Name.ToLower() == "double" || DA.DataType.Name.ToLower() == "int")
                                            {
                                                test = double.TryParse(dt.Rows[j][ColName].ToString(), out double_Test);
                                                if (test == false)
                                                {
                                                    UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"{ColName} Dynamic Attribute Wrong Input DataType in the row {j + 2}"));
                                                    goto ERROR;
                                                }
                                                DoubleDynamicAtt = double_Test;
                                            }
                                            else if (DA.DataType.Name.ToLower() == "datetime")
                                            {
                                                test = DateTime.TryParse(dt.Rows[j][ColName].ToString(), out datetime_Test);
                                                if (test == false)
                                                {
                                                    UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"{ColName} Dynamic Attribute Wrong Input DataType in the row {j + 2}"));
                                                    goto ERROR;
                                                }
                                                DateTimeDynamicAtt = datetime_Test;
                                            }
                                            else if (DA.DataType.Name.ToLower() == "bool")
                                            {
                                                test = Boolean.TryParse(dt.Rows[j][ColName].ToString(), out boolean_Test);
                                                if (test == false)
                                                {
                                                    UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"{ColName} Dynamic Attribute Wrong Input DataType in the row {j + 2}"));
                                                    goto ERROR;
                                                }
                                                BooleanDynamicAtt = boolean_Test;
                                            }


                                        }
                                    }
                                    else
                                    {
                                        if (!String.IsNullOrEmpty(dt.Rows[j][ColName].ToString()))
                                        {
                                            if (DA.DataType.Name.ToLower() == "string")
                                            {
                                                StringDynamicAtt = Convert.ToString(dt.Rows[j][ColName]);
                                            }
                                            else if (DA.DataType.Name.ToLower() == "double" || DA.DataType.Name.ToLower() == "int")
                                            {
                                                test = double.TryParse(dt.Rows[j][ColName].ToString(), out double_Test);
                                                if (test == false)
                                                {
                                                    UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"{ColName} Dynamic Attribute Wrong Input DataType in the row {j + 2}"));
                                                    goto ERROR;
                                                }
                                                DoubleDynamicAtt = double_Test;
                                            }
                                            else if (DA.DataType.Name.ToLower() == "datetime")
                                            {
                                                test = DateTime.TryParse(dt.Rows[j][ColName].ToString(), out datetime_Test);
                                                if (test == false)
                                                {
                                                    UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"{ColName} Dynamic Attribute Wrong Input DataType in the row {j + 2}"));
                                                    goto ERROR;
                                                }
                                                DateTimeDynamicAtt = datetime_Test;

                                            }
                                            else if (DA.DataType.Name.ToLower() == "bool")
                                            {
                                                test = Boolean.TryParse(dt.Rows[j][ColName].ToString(), out boolean_Test);
                                                if (test == false)
                                                {
                                                    UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"{ColName} Dynamic Attribute Wrong Input DataType in the row {j + 2}"));
                                                    goto ERROR;
                                                }
                                                BooleanDynamicAtt = boolean_Test;
                                            }

                                        }
                                    }
                                    if (DA.DataType.Name.ToLower() == "string")
                                    {
                                        DynamicAttList.Add(StringDynamicAtt);
                                    }
                                    else if (DA.DataType.Name.ToLower() == "double" || DA.DataType.Name.ToLower() == "int")
                                    {
                                        DynamicAttList.Add(DoubleDynamicAtt);
                                    }
                                    else if (DA.DataType.Name.ToLower() == "datetime")
                                    {
                                        DynamicAttList.Add(DateTimeDynamicAtt);
                                    }
                                    else if (DA.DataType.Name.ToLower() == "bool")
                                    {
                                        DynamicAttList.Add(BooleanDynamicAtt);
                                    }


                                }
                            }
                        }
                        models.Add(model_test != null ? model_test : null);
                        notes.Add(note_test != null ? note_test : null);
                        weights.Add(weight_test != 0 ? weight_test : 0);
                        widths.Add(width_test != 0 ? width_test : 0);
                        lengths.Add(length_test != 0 ? length_test : 0);
                        sizes.Add(size_test != 0 ? size_test : 0);
                        depths.Add(depth_test != 0 ? depth_test : 0);
                        SpaceLibraries.Add(spacelibrary_test != 0 ? spacelibrary_test : 0);
                        FrequencyRanges.Add(FrequencyRange_test != null ? FrequencyRange_test : null);
                        Types.Add(Type_test != null ? Type_test : null);
                        BandWidths.Add(BandWidths_test != null ? BandWidths_test : null);
                        ChannelBandWidths.Add(ChannelBandWidths_test != null ? ChannelBandWidths_test : null);
                        Heights.Add(Height_test != 0 ? Height_test : 0);
                        for (int f = 0; f < DynamicAttList.Count; f++)
                        {
                            //var h = DynamicAtts[f].Value;
                            DynamicAtts[f].Item3.Add(DynamicAttList[f]);
                            //h.Add(DynamicAttList[f]);
                        }


                    }
                    catch (Exception err)
                    {

                        //UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, "Wrong Input DataType"));
                    }
                }
                if (models.Count > 0)
                {
                    OracleParameter model = new OracleParameter();
                    model.OracleDbType = OracleDbType.NVarchar2;
                    model.Value = models.ToArray();

                    OracleParameter note = new OracleParameter();
                    note.OracleDbType = OracleDbType.NVarchar2;
                    note.Value = notes.ToArray();

                    OracleParameter Weight = new OracleParameter();
                    Weight.OracleDbType = OracleDbType.BinaryFloat;
                    Weight.Value = weights.ToArray();

                    OracleParameter Width = new OracleParameter();
                    Width.OracleDbType = OracleDbType.BinaryFloat;
                    Width.Value = widths.ToArray();

                    OracleParameter Size = new OracleParameter();
                    Size.OracleDbType = OracleDbType.BinaryFloat;
                    Size.Value = sizes.ToArray();

                    OracleParameter Length = new OracleParameter();
                    Length.OracleDbType = OracleDbType.BinaryFloat;
                    Length.Value = lengths.ToArray();

                    OracleParameter Depth = new OracleParameter();
                    Depth.OracleDbType = OracleDbType.BinaryFloat;
                    Depth.Value = depths.ToArray();

                    OracleParameter SpaceLibrary = new OracleParameter();
                    SpaceLibrary.OracleDbType = OracleDbType.BinaryFloat;
                    SpaceLibrary.Value = SpaceLibraries.ToArray();

                    OracleParameter FrequencyRangeS = new OracleParameter();
                    FrequencyRangeS.OracleDbType = OracleDbType.NVarchar2;
                    FrequencyRangeS.Value = FrequencyRanges.ToArray();

                    OracleParameter TypeS = new OracleParameter();
                    TypeS.OracleDbType = OracleDbType.NVarchar2;
                    TypeS.Value = Types.ToArray();

                    OracleParameter BandWidthS = new OracleParameter();
                    BandWidthS.OracleDbType = OracleDbType.NVarchar2;
                    BandWidthS.Value = BandWidths.ToArray();

                    OracleParameter ChannelBandWidthS = new OracleParameter();
                    ChannelBandWidthS.OracleDbType = OracleDbType.NVarchar2;
                    ChannelBandWidthS.Value = ChannelBandWidths.ToArray();

                    OracleParameter HeightsS = new OracleParameter();
                    HeightsS.OracleDbType = OracleDbType.BinaryFloat;
                    HeightsS.Value = Heights.ToArray();

                    // create command and set properties
                    OracleCommand cmd = connection.CreateCommand();
                    cmd.CommandText = "INSERT INTO \"TLIpowerLibrary\" (\"Model\", \"Note\", \"Weight\", \"width\", \"Length\", \"Depth\", \"SpaceLibrary\",\"Size\",\"FrequencyRange\",\"Type\",\"L_W_H\",\"ChannelBandWidth\",\"BandWidth\",\"Height\") VALUES ( :1, :2, :3, :4, :5, :6, :7,:8 ,:9 ,:9 ,:10 ,:11 ,:12,:13)";
                    cmd.ArrayBindCount = models.Count;
                    cmd.Parameters.Add(model);
                    cmd.Parameters.Add(note);
                    cmd.Parameters.Add(Weight);
                    cmd.Parameters.Add(Width);
                    cmd.Parameters.Add(Length);
                    cmd.Parameters.Add(Depth);
                    cmd.Parameters.Add(SpaceLibrary);
                    cmd.Parameters.Add(Size);
                    cmd.Parameters.Add(FrequencyRangeS);
                    cmd.Parameters.Add(TypeS);
                    cmd.Parameters.Add(new OracleParameter(":10", OracleDbType.NVarchar2) { Value = DBNull.Value });
                    cmd.Parameters.Add(ChannelBandWidthS);
                    cmd.Parameters.Add(BandWidthS);
                    cmd.Parameters.Add(HeightsS);

                    cmd.ExecuteNonQuery();
                    //connection.Close();
                    List<int> InsertedIds = new List<int>();
                    if (LastId != null)
                    {
                        OracleCommand InventoryCmd = connection.CreateCommand();
                        InventoryCmd.CommandType = CommandType.Text;
                        InventoryCmd.CommandText = $"Select \"Id\" From \"TLIpowerLibrary\" where \"TLIpowerLibrary\".\"Id\" > {LastId.Id}";
                        OracleDataReader Reader = InventoryCmd.ExecuteReader();
                        if (Reader.HasRows)
                        {
                            while (Reader.Read())
                            {
                                InsertedIds.Add(Reader.GetInt32(0));
                            }
                        }
                    }
                    else
                    {
                        OracleCommand InventoryCmd = connection.CreateCommand();
                        InventoryCmd.CommandType = CommandType.Text;
                        InventoryCmd.CommandText = $"Select \"Id\" From \"TLIpowerLibrary\"";
                        OracleDataReader Reader = InventoryCmd.ExecuteReader();
                        if (Reader.HasRows)
                        {
                            while (Reader.Read())
                            {
                                InsertedIds.Add(Reader.GetInt32(0));
                            }
                        }
                    }
                    TLIhistory tLIhistory = new TLIhistory()
                    {
                        UserId = UserId,
                        TablesNameId = TableNameEntity.Id,
                        HistoryTypeId = 1,

                    };
                    _dbContext.TLIhistory.Add(tLIhistory);
                    _dbContext.SaveChanges();
                    var powerLibraryist = new List<TLIpowerLibrary>();

                    for (int i = 0; i < models.Count; i++)
                    {
                        powerLibraryist.Add(new TLIpowerLibrary
                        {
                            Id = InsertedIds[i],
                            Model = models[i],
                            Note = notes[i],
                            BandWidth = BandWidths[i],
                            FrequencyRange = FrequencyRanges[i],
                            Type = Types[i],
                            Size = sizes[i],
                            width = widths[i],
                            Weight = weights[i],
                            Length = lengths[i],
                            Height = Heights[i],
                            Depth = depths[i],
                            SpaceLibrary = SpaceLibraries[i],

                        });
                    }


                    foreach (int recordId in InsertedIds)
                    {
                        var RecordName = powerLibraryist.FirstOrDefault(x => x.Id == recordId);
                        if (RecordName != null)
                        {
                            var attributeNames = RecordName.GetType().GetProperties()
                                .Where(x =>
                                    (x.PropertyType.IsGenericType && x.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>) &&
                                        new Type[] { typeof(int), typeof(string), typeof(double), typeof(float), typeof(Single), typeof(bool), typeof(DateTime) }
                                        .Contains(x.PropertyType.GetGenericArguments()[0]))
                                    || new Type[] { typeof(int), typeof(string), typeof(double), typeof(bool), typeof(DateTime), typeof(float), typeof(Single) }
                                    .Contains(x.PropertyType)
                                )
                                .ToList();

                            foreach (var propertyInfo in attributeNames)
                            {

                                var propertyName = propertyInfo.Name;

                                var propertyValue = propertyInfo.GetValue(RecordName)?.ToString().Trim();

                                TLIhistoryDet tLIhistoryDet = new TLIhistoryDet()
                                {
                                    HistoryId = tLIhistory.Id,
                                    TablesNameId = TableNameEntity.Id,
                                    RecordId = recordId.ToString(),
                                    AttributeName = propertyName,
                                    NewValue = propertyValue
                                };


                                _dbContext.TLIhistoryDet.Add(tLIhistoryDet);
                            }
                        }
                    }
                    _dbContext.SaveChanges();

                    foreach (var DynamicAtt in DynamicAtts)
                    {
                        for (int k = 0; k < DynamicAtt.Item3.Count; k++)
                        {
                            if (!String.IsNullOrEmpty(DynamicAtt.Item3[k].ToString()))
                            {

                                var Message = _unitOfWork.CivilWithLegsRepository.CheckDynamicValidationAndDependenceDynamic(DynamicAtt.Item1, DynamicAtt.Item3[k], Convert.ToInt32(InsertedIds[k]), tLIhistory.Id, powerLibraryist[0]).Message;
                                if (Message != "Success")
                                {
                                    UnsavedRows.Add(new KeyValuePair<int, string>(k + 2, Message + ' ' + $"in the Colum Name{ColName} in the Row Number {k + 2} "));
                                    goto ERROR;
                                }

                            }

                        }
                        //List<string> StringValues = new List<string>();
                        //List<double> DoubleValues = new List<double>();
                        //List<DateTime> DateTimeValues = new List<DateTime>();
                        //List<Int32> BooleanValues = new List<Int32>();
                        //List<int> DynamicAttIds = new List<int>();
                        //List<bool> disables = new List<bool>();
                        //List<int?> sideArmLibIds = new List<int?>();
                        //List<int?> sideArmLibraryIds = new List<int?>();
                        //List<int> InventoryIds = new List<int>();
                        //List<int> tablesNamesIds = new List<int>();
                        //if (DynamicAtt.Item2 == "string")
                        //{
                        //    for (int k = 0; k < DynamicAtt.Item3.Count; k++)
                        //    {
                        //        if (!String.IsNullOrEmpty(DynamicAtt.Item3[k].ToString()))
                        //        {
                        //            StringValues.Add(DynamicAtt.Item3[k].ToString());
                        //            DynamicAttIds.Add(DynamicAtt.Item1);
                        //            disables.Add(false);
                        //            sideArmLibIds.Add(null);
                        //            sideArmLibraryIds.Add(null);
                        //            InventoryIds.Add(Convert.ToInt32(InsertedIds[k]));
                        //            tablesNamesIds.Add(TableNameEntity.Id);
                        //        }
                        //    }
                        //}
                        //if (DynamicAtt.Item2 == "double")
                        //{
                        //    for (int k = 0; k < DynamicAtt.Item3.Count; k++)
                        //    {
                        //        if (!String.IsNullOrEmpty(DynamicAtt.Item3[k].ToString()))
                        //        {
                        //            DoubleValues.Add(Convert.ToInt32(DynamicAtt.Item3[k]));
                        //            DynamicAttIds.Add(DynamicAtt.Item1);
                        //            disables.Add(false);
                        //            sideArmLibIds.Add(null);
                        //            sideArmLibraryIds.Add(null);
                        //            InventoryIds.Add(Convert.ToInt32(InsertedIds[k]));
                        //            tablesNamesIds.Add(TableNameEntity.Id);
                        //        }
                        //    }
                        //}
                        //if (DynamicAtt.Item2 == "datetime")
                        //{
                        //    for (int k = 0; k < DynamicAtt.Item3.Count; k++)
                        //    {
                        //        if (!String.IsNullOrEmpty(DynamicAtt.Item3[k].ToString()))
                        //        {
                        //            DateTimeValues.Add(Convert.ToDateTime(DynamicAtt.Item3[k]));
                        //            DynamicAttIds.Add(DynamicAtt.Item1);
                        //            disables.Add(false);
                        //            sideArmLibIds.Add(null);
                        //            sideArmLibraryIds.Add(null);
                        //            InventoryIds.Add(Convert.ToInt32(InsertedIds[k]));
                        //            tablesNamesIds.Add(TableNameEntity.Id);
                        //        }
                        //    }
                        //}
                        //if (DynamicAtt.Item2 == "boolean")
                        //{
                        //    for (int k = 0; k < DynamicAtt.Item3.Count; k++)
                        //    {
                        //        if (!String.IsNullOrEmpty(DynamicAtt.Item3[k].ToString()))
                        //        {
                        //            bool BoolValue = Convert.ToBoolean(DynamicAtt.Item3[k]);
                        //            if (BoolValue == true)
                        //            {
                        //                BooleanValues.Add(1);
                        //            }
                        //            else
                        //            {
                        //                BooleanValues.Add(0);
                        //            }
                        //            DynamicAttIds.Add(DynamicAtt.Item1);
                        //            disables.Add(false);
                        //            sideArmLibIds.Add(null);
                        //            sideArmLibraryIds.Add(null);
                        //            InventoryIds.Add(Convert.ToInt32(InsertedIds[k]));
                        //            tablesNamesIds.Add(TableNameEntity.Id);
                        //        }
                        //    }
                        //}
                        ////connection.Open();
                        //OracleParameter value = new OracleParameter();
                        //if (DynamicAtt.Item2 == "string")
                        //{
                        //    value.OracleDbType = OracleDbType.NVarchar2;
                        //    value.Value = StringValues.ToArray();
                        //}
                        //else if (DynamicAtt.Item2 == "double")
                        //{
                        //    value.OracleDbType = OracleDbType.BinaryDouble;
                        //    value.Value = DoubleValues.ToArray();
                        //}
                        //else if (DynamicAtt.Item2 == "datetime")
                        //{
                        //    value.OracleDbType = OracleDbType.Date;
                        //    value.Value = DateTimeValues.ToArray();
                        //}
                        //else if (DynamicAtt.Item2 == "boolean")
                        //{
                        //    value.OracleDbType = OracleDbType.Int32;
                        //    value.Value = BooleanValues.ToArray();
                        //}
                        //OracleParameter DynamicAttId = new OracleParameter();
                        //DynamicAttId.OracleDbType = OracleDbType.Int32;
                        //DynamicAttId.Value = DynamicAttIds.ToArray();

                        //OracleParameter disable = new OracleParameter();
                        //disable.OracleDbType = OracleDbType.Boolean;
                        //disable.Value = disables.ToArray();

                        //OracleParameter sideArmLibId = new OracleParameter();
                        //sideArmLibId.OracleDbType = OracleDbType.Int32;
                        //sideArmLibId.Value = sideArmLibIds.ToArray();

                        //OracleParameter sideArmLibraryId = new OracleParameter();
                        //sideArmLibraryId.OracleDbType = OracleDbType.Int32;
                        //sideArmLibraryId.Value = sideArmLibraryIds.ToArray();

                        //OracleParameter InventoryId = new OracleParameter();
                        //InventoryId.OracleDbType = OracleDbType.Int32;
                        //InventoryId.Value = InventoryIds.ToArray();

                        //OracleParameter tablesNamesId = new OracleParameter();
                        //tablesNamesId.OracleDbType = OracleDbType.Int32;
                        //tablesNamesId.Value = tablesNamesIds.ToArray();

                        //OracleCommand dalvcmd = connection.CreateCommand();
                        //if (DynamicAtt.Item2 == "string")
                        //{
                        //    dalvcmd.CommandText = "INSERT INTO \"TLIdynamicAttLibValue\" (\"DynamicAttId\", \"InventoryId\", \"tablesNamesId\", \"ValueString\") VALUES ( :1, :2, :3, :4)";
                        //    dalvcmd.ArrayBindCount = StringValues.Count;
                        //}
                        //else if (DynamicAtt.Item2 == "double")
                        //{
                        //    dalvcmd.CommandText = "INSERT INTO \"TLIdynamicAttLibValue\" (\"DynamicAttId\", \"InventoryId\", \"tablesNamesId\", \"ValueDouble\") VALUES ( :1, :2, :3, :4)";
                        //    dalvcmd.ArrayBindCount = DoubleValues.Count;
                        //}
                        //else if (DynamicAtt.Item2 == "datetime")
                        //{
                        //    dalvcmd.CommandText = "INSERT INTO \"TLIdynamicAttLibValue\" (\"DynamicAttId\", \"InventoryId\", \"tablesNamesId\", \"ValueDateTime\") VALUES ( :1, :2, :3, :4)";
                        //    dalvcmd.ArrayBindCount = DateTimeValues.Count;
                        //}
                        //else if (DynamicAtt.Item2 == "boolean")
                        //{
                        //    dalvcmd.CommandText = "INSERT INTO \"TLIdynamicAttLibValue\" (\"DynamicAttId\", \"InventoryId\", \"tablesNamesId\", \"ValueBoolean\") VALUES ( :1, :2, :3, :4)";
                        //    dalvcmd.ArrayBindCount = BooleanValues.Count;
                        //}
                        ////dalvcmd.Parameters.Clear();
                        //dalvcmd.Parameters.Add(DynamicAttId);
                        //dalvcmd.Parameters.Add(InventoryId);
                        //dalvcmd.Parameters.Add(tablesNamesId);
                        //dalvcmd.Parameters.Add(value);
                        //dalvcmd.ExecuteNonQuery();
                    }
                    RecordId.AddRange(InsertedIds);
                }
            ERROR:;
            }
            catch (Exception err)
            {

                throw err;
            }
        }

        private void SaveradioAntennaLibraryUsingOracleBulkCopy(int UserId, out List<int> RecordId, DataTable dt, ref List<KeyValuePair<int, string>> UnsavedRows, int ActColumns, int Columns, ExcelWorksheet sheet, List<KeyValuePair<string, List<DropDownListFilters>>> RelatedTables, TLItablesNames TableNameEntity, OracleConnection connection)
        {
            try
            {
                RecordId = new List<int>();
                List<string> Models = _unitOfWork.RadioAntennaLibraryRepository.GetSelect(x => x.Model).ToList();

                TLIradioAntennaLibrary LastId = _serviceProvider.GetService<ApplicationDbContext>().
                    TLIradioAntennaLibrary.OrderByDescending(a => a.Id).FirstOrDefault();

                List<TLIdynamicAtt> RadioAntennaLibraryDynamicAtts = _unitOfWork.DynamicAttRepository.GetIncludeWhere(x =>
                    x.tablesNamesId == TableNameEntity.Id && !x.disable, x => x.DataType).ToList();

                List<string> models = new List<string>();
                List<string> frequencybands = new List<string>();
                List<float> weights = new List<float>();
                List<float> widths = new List<float>();
                List<float> depths = new List<float>();
                List<float> lengths = new List<float>();
                List<string> notes = new List<string>();
                List<float> SpaceLibraries = new List<float>();
                List<Tuple<int, string, List<dynamic>>> DynamicAtts = new List<Tuple<int, string, List<dynamic>>>();
                TLIdynamicAtt DA = new TLIdynamicAtt();
                string ColName = null;
                //List<string> test = new List<string>();
                if (ActColumns > 0)
                {
                    for (int i = ActColumns; i <= Columns; i++)
                    {
                        ColName = sheet.Cells[1, i].Value.ToString();
                        DA = RadioAntennaLibraryDynamicAtts.FirstOrDefault(x => x.Key == ColName);
                        if (DA != null)
                            DynamicAtts.Add(new Tuple<int, string, List<dynamic>>(DA.Id, DA.DataType.Name.ToLower(), new List<dynamic>()));
                    }
                }
                bool test;
                for (int j = 0; j < dt.Rows.Count; j++)
                {
                    try
                    {
                        string model_test = null;

                        if (dt.Columns.Contains("Model"))
                        {
                            if ((Models.Find(x => x == Convert.ToString(dt.Rows[j]["Model"]))) != null)
                            {
                                UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"Model is already exist in the row {j + 2}"));
                                goto ERROR;
                            }
                            if (!String.IsNullOrEmpty(dt.Rows[j]["Model"].ToString()))
                            {
                                model_test = Convert.ToString(dt.Rows[j]["Model"]);
                            }
                            else
                            {
                                UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"Model can not to be null In The Row {j + 2}"));
                                goto ERROR;
                            }
                        }

                        string frequencyband_test = null;
                        if (dt.Columns.Contains("FrequencyBand"))
                        {
                            if (!String.IsNullOrEmpty(dt.Rows[j]["FrequencyBand"].ToString()))
                            {
                                frequencyband_test = Convert.ToString(dt.Rows[j]["FrequencyBand"]);
                            }
                        }

                        float weight_test_1 = 0;

                        if (dt.Columns.Contains("Weight"))
                        {
                            if (!String.IsNullOrEmpty(dt.Rows[j]["Weight"].ToString()))
                            {
                                test = float.TryParse(dt.Rows[j]["Weight"].ToString(), out weight_test_1);
                                if (test == false)
                                {
                                    UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"Weight Wrong Input DataType In The Row {j + 2}"));
                                    goto ERROR;
                                }
                            }
                        }

                        float width_test = 0;
                        if (dt.Columns.Contains("Width"))
                        {
                            test = float.TryParse(dt.Rows[j]["Width"].ToString(), out width_test);
                            if (test == false)
                            {
                                UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"Width Wrong Input DataType In The Row {j + 2}"));
                                goto ERROR;
                            }
                        }

                        float depth_test = 0;
                        if (dt.Columns.Contains("Depth"))
                        {
                            test = float.TryParse(dt.Rows[j]["Depth"].ToString(), out depth_test);
                            if (test == false)
                            {
                                UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"Depth Wrong Input DataType In The Row {j + 2}"));
                                goto ERROR;
                            }
                        }

                        float length_test = 0;
                        if (dt.Columns.Contains("Length"))
                        {
                            test = float.TryParse(dt.Rows[j]["Length"].ToString(), out length_test);
                            if (test == false)
                            {
                                UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"Length Wrong Input DataType In The Row {j + 2}"));
                                goto ERROR;
                            }
                        }

                        string note_test = null;
                        if (dt.Columns.Contains("Notes"))
                        {
                            if (!String.IsNullOrEmpty(dt.Rows[j]["Notes"].ToString()))
                            {
                                note_test = Convert.ToString(dt.Rows[j]["Notes"]);
                            }
                        }

                        float spacelibrary_test = 0;
                       
                        if (length_test == 0)
                        {
                            UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"length must to be bigger of zero in the row {j + 2}"));
                            goto ERROR;
                        }
                        if (width_test == 0)
                        {
                            UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"width must to be bigger of zero in the row {j + 2}"));
                            goto ERROR;
                        }
                        else
                        {
                            spacelibrary_test = length_test * width_test;
                        }

                            
                        

                        List<dynamic> DynamicAttList = new List<dynamic>();
                        if (ActColumns > 0)
                        {
                            for (int i = ActColumns; i <= Columns; i++)
                            {
                                string StringDynamicAtt = string.Empty;
                                double? DoubleDynamicAtt = null;
                                DateTime? DateTimeDynamicAtt = null;
                                Boolean? BooleanDynamicAtt = null;
                                ColName = sheet.Cells[1, i].Value.ToString();
                                DA = RadioAntennaLibraryDynamicAtts.FirstOrDefault(x => x.Key == ColName);
                                //Check dynamic attribute if required or not
                                if (DA != null)
                                {
                                    double double_Test = 0;
                                    DateTime datetime_Test = DateTime.Now;
                                    Boolean boolean_Test = false;
                                    if (DA.Required == true)
                                    {
                                        //if somthing wrong then add row to UnsavedRows
                                        if (String.IsNullOrEmpty(dt.Rows[j][ColName].ToString()))
                                        {
                                            UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"Dynamic Attribute ({ColName}) is required"));
                                            goto ERROR;
                                        }
                                        else
                                        {
                                            if (DA.DataType.Name.ToLower() == "string")
                                            {

                                                StringDynamicAtt = Convert.ToString(dt.Rows[j][ColName]);
                                            }
                                            else if (DA.DataType.Name.ToLower() == "double" || DA.DataType.Name.ToLower() == "int")
                                            {
                                                test = double.TryParse(dt.Rows[j][ColName].ToString(), out double_Test);
                                                if (test == false)
                                                {
                                                    UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"{ColName} Dynamic Attribute Wrong Input DataType in the row {j + 2}"));
                                                    goto ERROR;
                                                }
                                                DoubleDynamicAtt = double_Test;
                                            }
                                            else if (DA.DataType.Name.ToLower() == "datetime")
                                            {
                                                test = DateTime.TryParse(dt.Rows[j][ColName].ToString(), out datetime_Test);
                                                if (test == false)
                                                {
                                                    UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"{ColName} Dynamic Attribute Wrong Input DataType in the row {j + 2}"));
                                                    goto ERROR;
                                                }
                                                DateTimeDynamicAtt = datetime_Test;
                                            }
                                            else if (DA.DataType.Name.ToLower() == "bool")
                                            {
                                                test = Boolean.TryParse(dt.Rows[j][ColName].ToString(), out boolean_Test);
                                                if (test == false)
                                                {
                                                    UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"{ColName} Dynamic Attribute Wrong Input DataType in the row {j + 2}"));
                                                    goto ERROR;
                                                }
                                                BooleanDynamicAtt = boolean_Test;
                                            }


                                        }
                                    }
                                    else
                                    {
                                        if (!String.IsNullOrEmpty(dt.Rows[j][ColName].ToString()))
                                        {
                                            if (DA.DataType.Name.ToLower() == "string")
                                            {
                                                StringDynamicAtt = Convert.ToString(dt.Rows[j][ColName]);
                                            }
                                            else if (DA.DataType.Name.ToLower() == "double" || DA.DataType.Name.ToLower() == "int")
                                            {
                                                test = double.TryParse(dt.Rows[j][ColName].ToString(), out double_Test);
                                                if (test == false)
                                                {
                                                    UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"{ColName} Dynamic Attribute Wrong Input DataType in the row {j + 2}"));
                                                    goto ERROR;
                                                }
                                                DoubleDynamicAtt = double_Test;
                                            }
                                            else if (DA.DataType.Name.ToLower() == "datetime")
                                            {
                                                test = DateTime.TryParse(dt.Rows[j][ColName].ToString(), out datetime_Test);
                                                if (test == false)
                                                {
                                                    UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"{ColName} Dynamic Attribute Wrong Input DataType in the row {j + 2}"));
                                                    goto ERROR;
                                                }
                                                DateTimeDynamicAtt = datetime_Test;

                                            }
                                            else if (DA.DataType.Name.ToLower() == "bool")
                                            {
                                                test = Boolean.TryParse(dt.Rows[j][ColName].ToString(), out boolean_Test);
                                                if (test == false)
                                                {
                                                    UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"{ColName} Dynamic Attribute Wrong Input DataType in the row {j + 2}"));
                                                    goto ERROR;
                                                }
                                                BooleanDynamicAtt = boolean_Test;
                                            }

                                        }
                                    }
                                    if (DA.DataType.Name.ToLower() == "string")
                                    {
                                        DynamicAttList.Add(StringDynamicAtt);
                                    }
                                    else if (DA.DataType.Name.ToLower() == "double" || DA.DataType.Name.ToLower() == "int")
                                    {
                                        DynamicAttList.Add(DoubleDynamicAtt);
                                    }
                                    else if (DA.DataType.Name.ToLower() == "datetime")
                                    {
                                        DynamicAttList.Add(DateTimeDynamicAtt);
                                    }
                                    else if (DA.DataType.Name.ToLower() == "bool")
                                    {
                                        DynamicAttList.Add(BooleanDynamicAtt);
                                    }


                                }
                            }
                        }
                        models.Add(model_test != null ? model_test : null);
                        frequencybands.Add(frequencyband_test != null ? model_test : null);
                        weights.Add(weight_test_1 != 0 ? weight_test_1 : 0);
                        widths.Add(width_test != 0 ? width_test : 0);
                        depths.Add(depth_test != 0 ? depth_test : 0);
                        lengths.Add(length_test != 0 ? length_test : 0);
                        notes.Add(note_test != null ? note_test : null);
                        SpaceLibraries.Add(spacelibrary_test != 0 ? spacelibrary_test : 0);
                        for (int f = 0; f < DynamicAttList.Count; f++)
                        {
                            //var h = DynamicAtts[f].Value;
                            DynamicAtts[f].Item3.Add(DynamicAttList[f]);
                            //h.Add(DynamicAttList[f]);
                        }


                    }
                    catch (Exception err)
                    {

                        //UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, "Wrong Input DataType"));
                    }
                }
                if (models.Count > 0)
                {
                    OracleParameter model = new OracleParameter();
                    model.OracleDbType = OracleDbType.NVarchar2;
                    model.Value = models.ToArray();

                    OracleParameter FrequencyBand = new OracleParameter();
                    FrequencyBand.OracleDbType = OracleDbType.NVarchar2;
                    FrequencyBand.Value = frequencybands.ToArray();

                    OracleParameter Weight = new OracleParameter();
                    Weight.OracleDbType = OracleDbType.BinaryFloat;
                    Weight.Value = weights.ToArray();

                    OracleParameter Width = new OracleParameter();
                    Width.OracleDbType = OracleDbType.BinaryFloat;
                    Width.Value = widths.ToArray();

                    OracleParameter Depth = new OracleParameter();
                    Depth.OracleDbType = OracleDbType.BinaryFloat;
                    Depth.Value = depths.ToArray();

                    OracleParameter Length = new OracleParameter();
                    Length.OracleDbType = OracleDbType.BinaryFloat;
                    Length.Value = lengths.ToArray();

                    OracleParameter note = new OracleParameter();
                    note.OracleDbType = OracleDbType.NVarchar2;
                    note.Value = notes.ToArray();

                    OracleParameter SpaceLibrary = new OracleParameter();
                    SpaceLibrary.OracleDbType = OracleDbType.BinaryFloat;
                    SpaceLibrary.Value = SpaceLibraries.ToArray();

                    // create command and set properties
                    OracleCommand cmd = connection.CreateCommand();
                    cmd.CommandText = "INSERT INTO \"TLIradioAntennaLibrary\" (\"Model\", \"FrequencyBand\", \"Weight\", \"Width\", \"Depth\", \"Length\", \"Notes\", \"SpaceLibrary\") VALUES ( :1, :2, :3, :4, :5, :6, :7, :8)";
                    cmd.ArrayBindCount = models.Count;
                    cmd.Parameters.Add(model);
                    cmd.Parameters.Add(FrequencyBand);
                    cmd.Parameters.Add(Weight);
                    cmd.Parameters.Add(Width);
                    cmd.Parameters.Add(Depth);
                    cmd.Parameters.Add(Length);
                    cmd.Parameters.Add(note);
                    cmd.Parameters.Add(SpaceLibrary);
                    cmd.ExecuteNonQuery();
                    //connection.Close();
                    List<int> InsertedIds = new List<int>();
                    if (LastId != null)
                    {
                        OracleCommand InventoryCmd = connection.CreateCommand();
                        InventoryCmd.CommandType = CommandType.Text;
                        InventoryCmd.CommandText = $"Select \"Id\" From \"TLIradioAntennaLibrary\" where \"TLIradioAntennaLibrary\".\"Id\" > {LastId.Id}";
                        OracleDataReader Reader = InventoryCmd.ExecuteReader();
                        if (Reader.HasRows)
                        {
                            while (Reader.Read())
                            {
                                InsertedIds.Add(Reader.GetInt32(0));
                            }
                        }
                    }
                    else
                    {
                        OracleCommand InventoryCmd = connection.CreateCommand();
                        InventoryCmd.CommandType = CommandType.Text;
                        InventoryCmd.CommandText = $"Select \"Id\" From \"TLIradioAntennaLibrary\"";
                        OracleDataReader Reader = InventoryCmd.ExecuteReader();
                        if (Reader.HasRows)
                        {
                            while (Reader.Read())
                            {
                                InsertedIds.Add(Reader.GetInt32(0));
                            }
                        }
                    }
                    TLIhistory tLIhistory = new TLIhistory()
                    {
                        UserId = UserId,
                        TablesNameId = TableNameEntity.Id,
                        HistoryTypeId = 1,

                    };
                    _dbContext.TLIhistory.Add(tLIhistory);
                    _dbContext.SaveChanges();
                    var radioAntennaLibraryist = new List<TLIradioAntennaLibrary>();

                    for (int i = 0; i < models.Count; i++)
                    {
                        radioAntennaLibraryist.Add(new TLIradioAntennaLibrary
                        {
                            Id = InsertedIds[i],
                            Model = models[i],
                            FrequencyBand = notes[i],
                            Width = widths[i],
                            Weight = weights[i],
                            Length = lengths[i],
                            Depth = depths[i],
                            Notes = notes[i],
                            SpaceLibrary = SpaceLibraries[i],

                        });
                    }


                    foreach (int recordId in InsertedIds)
                    {
                        var RecordName = radioAntennaLibraryist.FirstOrDefault(x => x.Id == recordId);
                        if (RecordName != null)
                        {
                            var attributeNames = RecordName.GetType().GetProperties()
                                .Where(x =>
                                    (x.PropertyType.IsGenericType && x.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>) &&
                                        new Type[] { typeof(int), typeof(string), typeof(double), typeof(float), typeof(Single), typeof(bool), typeof(DateTime) }
                                        .Contains(x.PropertyType.GetGenericArguments()[0]))
                                    || new Type[] { typeof(int), typeof(string), typeof(double), typeof(bool), typeof(DateTime), typeof(float), typeof(Single) }
                                    .Contains(x.PropertyType)
                                )
                                .ToList();

                            foreach (var propertyInfo in attributeNames)
                            {

                                var propertyName = propertyInfo.Name;

                                var propertyValue = propertyInfo.GetValue(RecordName)?.ToString().Trim();

                                TLIhistoryDet tLIhistoryDet = new TLIhistoryDet()
                                {
                                    HistoryId = tLIhistory.Id,
                                    TablesNameId = TableNameEntity.Id,
                                    RecordId = recordId.ToString(),
                                    AttributeName = propertyName,
                                    NewValue = propertyValue
                                };


                                _dbContext.TLIhistoryDet.Add(tLIhistoryDet);
                            }
                        }
                    }
                    _dbContext.SaveChanges();

                    foreach (var DynamicAtt in DynamicAtts)
                    {
                        for (int k = 0; k < DynamicAtt.Item3.Count; k++)
                        {
                            if (!String.IsNullOrEmpty(DynamicAtt.Item3[k].ToString()))
                            {

                                var Message = _unitOfWork.CivilWithLegsRepository.CheckDynamicValidationAndDependenceDynamic(DynamicAtt.Item1, DynamicAtt.Item3[k], Convert.ToInt32(InsertedIds[k]), tLIhistory.Id, radioAntennaLibraryist[0]).Message;
                                if (Message != "Success")
                                {
                                    UnsavedRows.Add(new KeyValuePair<int, string>(k + 2, Message + ' ' + $"in the Colum Name{ColName} in the Row Number {k + 2} "));
                                    goto ERROR;
                                }

                            }

                        }
                        //List<string> StringValues = new List<string>();
                        //List<double> DoubleValues = new List<double>();
                        //List<DateTime> DateTimeValues = new List<DateTime>();
                        //List<Int32> BooleanValues = new List<Int32>();
                        //List<int> DynamicAttIds = new List<int>();
                        //List<bool> disables = new List<bool>();
                        //List<int?> sideArmLibIds = new List<int?>();
                        //List<int?> sideArmLibraryIds = new List<int?>();
                        //List<int> InventoryIds = new List<int>();
                        //List<int> tablesNamesIds = new List<int>();
                        //if (DynamicAtt.Item2 == "string")
                        //{
                        //    for (int k = 0; k < DynamicAtt.Item3.Count; k++)
                        //    {
                        //        if (!String.IsNullOrEmpty(DynamicAtt.Item3[k].ToString()))
                        //        {
                        //            StringValues.Add(DynamicAtt.Item3[k].ToString());
                        //            DynamicAttIds.Add(DynamicAtt.Item1);
                        //            disables.Add(false);
                        //            sideArmLibIds.Add(null);
                        //            sideArmLibraryIds.Add(null);
                        //            InventoryIds.Add(Convert.ToInt32(InsertedIds[k]));
                        //            tablesNamesIds.Add(TableNameEntity.Id);
                        //        }
                        //    }
                        //}
                        //if (DynamicAtt.Item2 == "double")
                        //{
                        //    for (int k = 0; k < DynamicAtt.Item3.Count; k++)
                        //    {
                        //        if (!String.IsNullOrEmpty(DynamicAtt.Item3[k].ToString()))
                        //        {
                        //            DoubleValues.Add(Convert.ToInt32(DynamicAtt.Item3[k]));
                        //            DynamicAttIds.Add(DynamicAtt.Item1);
                        //            disables.Add(false);
                        //            sideArmLibIds.Add(null);
                        //            sideArmLibraryIds.Add(null);
                        //            InventoryIds.Add(Convert.ToInt32(InsertedIds[k]));
                        //            tablesNamesIds.Add(TableNameEntity.Id);
                        //        }
                        //    }
                        //}
                        //if (DynamicAtt.Item2 == "datetime")
                        //{
                        //    for (int k = 0; k < DynamicAtt.Item3.Count; k++)
                        //    {
                        //        if (!String.IsNullOrEmpty(DynamicAtt.Item3[k].ToString()))
                        //        {
                        //            DateTimeValues.Add(Convert.ToDateTime(DynamicAtt.Item3[k]));
                        //            DynamicAttIds.Add(DynamicAtt.Item1);
                        //            disables.Add(false);
                        //            sideArmLibIds.Add(null);
                        //            sideArmLibraryIds.Add(null);
                        //            InventoryIds.Add(Convert.ToInt32(InsertedIds[k]));
                        //            tablesNamesIds.Add(TableNameEntity.Id);
                        //        }
                        //    }
                        //}
                        //if (DynamicAtt.Item2 == "boolean")
                        //{
                        //    for (int k = 0; k < DynamicAtt.Item3.Count; k++)
                        //    {
                        //        if (!String.IsNullOrEmpty(DynamicAtt.Item3[k].ToString()))
                        //        {
                        //            bool BoolValue = Convert.ToBoolean(DynamicAtt.Item3[k]);
                        //            if (BoolValue == true)
                        //            {
                        //                BooleanValues.Add(1);
                        //            }
                        //            else
                        //            {
                        //                BooleanValues.Add(0);
                        //            }
                        //            DynamicAttIds.Add(DynamicAtt.Item1);
                        //            disables.Add(false);
                        //            sideArmLibIds.Add(null);
                        //            sideArmLibraryIds.Add(null);
                        //            InventoryIds.Add(Convert.ToInt32(InsertedIds[k]));
                        //            tablesNamesIds.Add(TableNameEntity.Id);
                        //        }
                        //    }
                        //}
                        ////connection.Open();
                        //OracleParameter value = new OracleParameter();
                        //if (DynamicAtt.Item2 == "string")
                        //{
                        //    value.OracleDbType = OracleDbType.NVarchar2;
                        //    value.Value = StringValues.ToArray();
                        //}
                        //else if (DynamicAtt.Item2 == "double")
                        //{
                        //    value.OracleDbType = OracleDbType.BinaryDouble;
                        //    value.Value = DoubleValues.ToArray();
                        //}
                        //else if (DynamicAtt.Item2 == "datetime")
                        //{
                        //    value.OracleDbType = OracleDbType.Date;
                        //    value.Value = DateTimeValues.ToArray();
                        //}
                        //else if (DynamicAtt.Item2 == "boolean")
                        //{
                        //    value.OracleDbType = OracleDbType.Int32;
                        //    value.Value = BooleanValues.ToArray();
                        //}
                        //OracleParameter DynamicAttId = new OracleParameter();
                        //DynamicAttId.OracleDbType = OracleDbType.Int32;
                        //DynamicAttId.Value = DynamicAttIds.ToArray();

                        //OracleParameter disable = new OracleParameter();
                        //disable.OracleDbType = OracleDbType.Boolean;
                        //disable.Value = disables.ToArray();

                        //OracleParameter sideArmLibId = new OracleParameter();
                        //sideArmLibId.OracleDbType = OracleDbType.Int32;
                        //sideArmLibId.Value = sideArmLibIds.ToArray();

                        //OracleParameter sideArmLibraryId = new OracleParameter();
                        //sideArmLibraryId.OracleDbType = OracleDbType.Int32;
                        //sideArmLibraryId.Value = sideArmLibraryIds.ToArray();

                        //OracleParameter InventoryId = new OracleParameter();
                        //InventoryId.OracleDbType = OracleDbType.Int32;
                        //InventoryId.Value = InventoryIds.ToArray();

                        //OracleParameter tablesNamesId = new OracleParameter();
                        //tablesNamesId.OracleDbType = OracleDbType.Int32;
                        //tablesNamesId.Value = tablesNamesIds.ToArray();

                        //OracleCommand dalvcmd = connection.CreateCommand();
                        //if (DynamicAtt.Item2 == "string")
                        //{
                        //    dalvcmd.CommandText = "INSERT INTO \"TLIdynamicAttLibValue\" (\"DynamicAttId\", \"InventoryId\", \"tablesNamesId\", \"ValueString\") VALUES ( :1, :2, :3, :4)";
                        //    dalvcmd.ArrayBindCount = StringValues.Count;
                        //}
                        //else if (DynamicAtt.Item2 == "double")
                        //{
                        //    dalvcmd.CommandText = "INSERT INTO \"TLIdynamicAttLibValue\" (\"DynamicAttId\", \"InventoryId\", \"tablesNamesId\", \"ValueDouble\") VALUES ( :1, :2, :3, :4)";
                        //    dalvcmd.ArrayBindCount = DoubleValues.Count;
                        //}
                        //else if (DynamicAtt.Item2 == "datetime")
                        //{
                        //    dalvcmd.CommandText = "INSERT INTO \"TLIdynamicAttLibValue\" (\"DynamicAttId\", \"InventoryId\", \"tablesNamesId\", \"ValueDateTime\") VALUES ( :1, :2, :3, :4)";
                        //    dalvcmd.ArrayBindCount = DateTimeValues.Count;
                        //}
                        //else if (DynamicAtt.Item2 == "boolean")
                        //{
                        //    dalvcmd.CommandText = "INSERT INTO \"TLIdynamicAttLibValue\" (\"DynamicAttId\", \"InventoryId\", \"tablesNamesId\", \"ValueBoolean\") VALUES ( :1, :2, :3, :4)";
                        //    dalvcmd.ArrayBindCount = BooleanValues.Count;
                        //}
                        ////dalvcmd.Parameters.Clear();
                        //dalvcmd.Parameters.Add(DynamicAttId);
                        //dalvcmd.Parameters.Add(InventoryId);
                        //dalvcmd.Parameters.Add(tablesNamesId);
                        //dalvcmd.Parameters.Add(value);
                        //dalvcmd.ExecuteNonQuery();
                    }
                    RecordId.AddRange(InsertedIds);
                }
            ERROR:;
            }
            catch (Exception err)
            {

                throw err;
            }
        }

        private void SaveradioOtherLibraryUsingOracleBulkCopy(int UserId, out List<int> RecordId, DataTable dt, ref List<KeyValuePair<int, string>> UnsavedRows, int ActColumns, int Columns, ExcelWorksheet sheet, TLItablesNames TableNameEntity, OracleConnection connection)
        {
            try
            {
                RecordId = new List<int>();
                List<string> Models = _unitOfWork.RadioOtherLibraryRepository.GetSelect(x => x.Model).ToList();

                TLIradioOtherLibrary LastId = _serviceProvider.GetService<ApplicationDbContext>().
                    TLIradioOtherLibrary.OrderByDescending(a => a.Id).FirstOrDefault();

                List<TLIdynamicAtt> RadioOtherLibraryDynamicAtts = _unitOfWork.DynamicAttRepository.GetIncludeWhere(x =>
                    x.tablesNamesId == TableNameEntity.Id && !x.disable, x => x.DataType).ToList();

                List<string> models = new List<string>();
                List<float> weights = new List<float>();
                List<float> widths = new List<float>();
                List<float> lengths = new List<float>();
                List<float> heights = new List<float>();
                List<string> notes = new List<string>();
                List<float> SpaceLibraries = new List<float>();
                List<Tuple<int, string, List<dynamic>>> DynamicAtts = new List<Tuple<int, string, List<dynamic>>>();
                TLIdynamicAtt DA = new TLIdynamicAtt();
                string ColName = null;
                //List<string> test = new List<string>();
                if (ActColumns > 0)
                {
                    for (int i = ActColumns; i <= Columns; i++)
                    {
                        ColName = sheet.Cells[1, i].Value.ToString();
                        DA = RadioOtherLibraryDynamicAtts.FirstOrDefault(x => x.Key == ColName);
                        if (DA != null)
                            DynamicAtts.Add(new Tuple<int, string, List<dynamic>>(DA.Id, DA.DataType.Name.ToLower(), new List<dynamic>()));
                    }
                }
                bool test;
                for (int j = 0; j < dt.Rows.Count; j++)
                {
                    try
                    {
                        string model_test = null;

                        if (dt.Columns.Contains("Model"))
                        {
                            if ((Models.Find(x => x == Convert.ToString(dt.Rows[j]["Model"]))) != null)
                            {
                                UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"Model is already exist in the row {j + 2}"));
                                goto ERROR;
                            }
                            if (!String.IsNullOrEmpty(dt.Rows[j]["Model"].ToString()))
                            {
                                model_test = Convert.ToString(dt.Rows[j]["Model"]);
                            }
                        }

                        float weight_test = 0;
                        if (dt.Columns.Contains("Weight"))
                        {
                            if (!String.IsNullOrEmpty(dt.Rows[j]["Weight"].ToString()))
                            {
                                float weight_test_1;
                                test = float.TryParse(dt.Rows[j]["Weight"].ToString(), out weight_test_1);
                                if (test == true)
                                {
                                    weight_test = weight_test_1;
                                }
                                else
                                {
                                    UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"Weight Wrong Input DataType In The Row {j + 2}"));
                                    goto ERROR;
                                }
                            }
                        }

                        float width_test = 0;
                        if (dt.Columns.Contains("Width"))
                        {
                            test = float.TryParse(dt.Rows[j]["Width"].ToString(), out width_test);
                            if (test == false)
                            {
                                UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"Width Wrong Input DataType In The Row {j + 2}"));
                                goto ERROR;
                            }
                        }

                        float length_test = 0;
                        if (dt.Columns.Contains("Length"))
                        {
                            test = float.TryParse(dt.Rows[j]["Length"].ToString(), out length_test);
                            if (test == false)
                            {
                                UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"Length Wrong Input DataType In The Row {j + 2}"));
                                goto ERROR;
                            }
                        }

                        float height_test = 0;
                        if (dt.Columns.Contains("Height"))
                        {
                            test = float.TryParse(dt.Rows[j]["Height"].ToString(), out height_test);
                            if (test == false)
                            {
                                UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"Height Wrong Input DataType In The Row {j + 2}"));
                                goto ERROR;
                            }
                        }

                        string note_test = null;
                        if (dt.Columns.Contains("Notes"))
                        {
                            if (!String.IsNullOrEmpty(dt.Rows[j]["Notes"].ToString()))
                            {
                                note_test = Convert.ToString(dt.Rows[j]["Notes"]);
                            }
                        }

                        float spacelibrary_test = 0;
                       
                        if (length_test == 0)
                        {
                            UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"length must to be bigger of zero in the row {j + 2}"));
                            goto ERROR;
                        }
                        if (width_test == 0)
                        {
                            UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"width must to be bigger of zero in the row {j + 2}"));
                            goto ERROR;
                        }
                        else
                        {
                            spacelibrary_test = length_test * width_test;
                        }

                            
                        



                        //float spacelibrary_test = Convert.ToSingle(dt.Rows[j]["SpaceLibrary"]);
                        List<dynamic> DynamicAttList = new List<dynamic>();
                        if (ActColumns > 0)
                        {
                            for (int i = ActColumns; i <= Columns; i++)
                            {
                                string StringDynamicAtt = string.Empty;
                                double? DoubleDynamicAtt = null;
                                DateTime? DateTimeDynamicAtt = null;
                                Boolean? BooleanDynamicAtt = null;
                                ColName = sheet.Cells[1, i].Value.ToString();
                                DA = RadioOtherLibraryDynamicAtts.FirstOrDefault(x => x.Key == ColName);
                                //Check dynamic attribute if required or not
                                if (DA != null)
                                {
                                    double double_Test = 0;
                                    DateTime datetime_Test = DateTime.Now;
                                    Boolean boolean_Test = false;
                                    if (DA.Required == true)
                                    {
                                        //if somthing wrong then add row to UnsavedRows
                                        if (String.IsNullOrEmpty(dt.Rows[j][ColName].ToString()))
                                        {
                                            UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"Dynamic Attribute ({ColName}) is required"));
                                            goto ERROR;
                                        }
                                        else
                                        {
                                            if (DA.DataType.Name.ToLower() == "string")
                                            {

                                                StringDynamicAtt = Convert.ToString(dt.Rows[j][ColName]);
                                            }
                                            else if (DA.DataType.Name.ToLower() == "double" || DA.DataType.Name.ToLower() == "int")
                                            {
                                                test = double.TryParse(dt.Rows[j][ColName].ToString(), out double_Test);
                                                if (test == false)
                                                {
                                                    UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"{ColName} Dynamic Attribute Wrong Input DataType in the row {j + 2}"));
                                                    goto ERROR;
                                                }
                                                DoubleDynamicAtt = double_Test;
                                            }
                                            else if (DA.DataType.Name.ToLower() == "datetime")
                                            {
                                                test = DateTime.TryParse(dt.Rows[j][ColName].ToString(), out datetime_Test);
                                                if (test == false)
                                                {
                                                    UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"{ColName} Dynamic Attribute Wrong Input DataType in the row {j + 2}"));
                                                    goto ERROR;
                                                }
                                                DateTimeDynamicAtt = datetime_Test;
                                            }
                                            else if (DA.DataType.Name.ToLower() == "bool")
                                            {
                                                test = Boolean.TryParse(dt.Rows[j][ColName].ToString(), out boolean_Test);
                                                if (test == false)
                                                {
                                                    UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"{ColName} Dynamic Attribute Wrong Input DataType in the row {j + 2}"));
                                                    goto ERROR;
                                                }
                                                BooleanDynamicAtt = boolean_Test;
                                            }


                                        }
                                    }
                                    else
                                    {
                                        if (!String.IsNullOrEmpty(dt.Rows[j][ColName].ToString()))
                                        {
                                            if (DA.DataType.Name.ToLower() == "string")
                                            {
                                                StringDynamicAtt = Convert.ToString(dt.Rows[j][ColName]);
                                            }
                                            else if (DA.DataType.Name.ToLower() == "double" || DA.DataType.Name.ToLower() == "int")
                                            {
                                                test = double.TryParse(dt.Rows[j][ColName].ToString(), out double_Test);
                                                if (test == false)
                                                {
                                                    UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"{ColName} Dynamic Attribute Wrong Input DataType in the row {j + 2}"));
                                                    goto ERROR;
                                                }
                                                DoubleDynamicAtt = double_Test;
                                            }
                                            else if (DA.DataType.Name.ToLower() == "datetime")
                                            {
                                                test = DateTime.TryParse(dt.Rows[j][ColName].ToString(), out datetime_Test);
                                                if (test == false)
                                                {
                                                    UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"{ColName} Dynamic Attribute Wrong Input DataType in the row {j + 2}"));
                                                    goto ERROR;
                                                }
                                                DateTimeDynamicAtt = datetime_Test;

                                            }
                                            else if (DA.DataType.Name.ToLower() == "bool")
                                            {
                                                test = Boolean.TryParse(dt.Rows[j][ColName].ToString(), out boolean_Test);
                                                if (test == false)
                                                {
                                                    UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"{ColName} Dynamic Attribute Wrong Input DataType in the row {j + 2}"));
                                                    goto ERROR;
                                                }
                                                BooleanDynamicAtt = boolean_Test;
                                            }

                                        }
                                    }
                                    if (DA.DataType.Name.ToLower() == "string")
                                    {
                                        DynamicAttList.Add(StringDynamicAtt);
                                    }
                                    else if (DA.DataType.Name.ToLower() == "double" || DA.DataType.Name.ToLower() == "int")
                                    {
                                        DynamicAttList.Add(DoubleDynamicAtt);
                                    }
                                    else if (DA.DataType.Name.ToLower() == "datetime")
                                    {
                                        DynamicAttList.Add(DateTimeDynamicAtt);
                                    }
                                    else if (DA.DataType.Name.ToLower() == "bool")
                                    {
                                        DynamicAttList.Add(BooleanDynamicAtt);
                                    }


                                }
                            }
                        }
                        models.Add(model_test != null ? model_test : null);
                        weights.Add(weight_test != 0 ? weight_test : 0);
                        widths.Add(width_test != 0 ? width_test : 0);
                        lengths.Add(length_test != 0 ? length_test : 0);
                        heights.Add(height_test != 0 ? height_test : 0);
                        notes.Add(note_test != null ? note_test : null);
                        SpaceLibraries.Add(spacelibrary_test != 0 ? spacelibrary_test : 0);

                        for (int f = 0; f < DynamicAttList.Count; f++)
                        {
                            //var h = DynamicAtts[f].Value;
                            DynamicAtts[f].Item3.Add(DynamicAttList[f]);
                            //h.Add(DynamicAttList[f]);
                        }


                    }
                    catch (Exception err)
                    {

                        //UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, "Wrong Input DataType"));
                    }
                }
                if (models.Count > 0)
                {
                    OracleParameter model = new OracleParameter();
                    model.OracleDbType = OracleDbType.NVarchar2;
                    model.Value = models.ToArray();

                    OracleParameter Weight = new OracleParameter();
                    Weight.OracleDbType = OracleDbType.BinaryFloat;
                    Weight.Value = weights.ToArray();

                    OracleParameter Width = new OracleParameter();
                    Width.OracleDbType = OracleDbType.BinaryFloat;
                    Width.Value = widths.ToArray();

                    OracleParameter Length = new OracleParameter();
                    Length.OracleDbType = OracleDbType.BinaryFloat;
                    Length.Value = lengths.ToArray();

                    OracleParameter Height = new OracleParameter();
                    Height.OracleDbType = OracleDbType.BinaryFloat;
                    Height.Value = heights.ToArray();

                    OracleParameter note = new OracleParameter();
                    note.OracleDbType = OracleDbType.NVarchar2;
                    note.Value = notes.ToArray();

                    OracleParameter SpaceLibrary = new OracleParameter();
                    SpaceLibrary.OracleDbType = OracleDbType.BinaryFloat;
                    SpaceLibrary.Value = SpaceLibraries.ToArray();

                    // create command and set properties
                    OracleCommand cmd = connection.CreateCommand();
                    cmd.CommandText = "INSERT INTO \"TLIradioOtherLibrary\" (\"Model\", \"Weight\", \"Width\", \"Length\", \"Height\", \"Notes\", \"SpaceLibrary\") VALUES ( :1, :2, :3, :4, :5, :6, :7)";
                    cmd.ArrayBindCount = models.Count;
                    cmd.Parameters.Add(model);
                    cmd.Parameters.Add(Weight);
                    cmd.Parameters.Add(Width);
                    cmd.Parameters.Add(Length);
                    cmd.Parameters.Add(Height);
                    cmd.Parameters.Add(note);
                    cmd.Parameters.Add(SpaceLibrary);
                    cmd.ExecuteNonQuery();
                    //connection.Close();
                    List<int> InsertedIds = new List<int>();
                    if (LastId != null)
                    {
                        OracleCommand InventoryCmd = connection.CreateCommand();
                        InventoryCmd.CommandType = CommandType.Text;
                        InventoryCmd.CommandText = $"Select \"Id\" From \"TLIradioAntennaLibrary\" where \"TLIradioAntennaLibrary\".\"Id\" > {LastId.Id}";
                        OracleDataReader Reader = InventoryCmd.ExecuteReader();
                        if (Reader.HasRows)
                        {
                            while (Reader.Read())
                            {
                                InsertedIds.Add(Reader.GetInt32(0));
                            }
                        }
                    }
                    else
                    {
                        OracleCommand InventoryCmd = connection.CreateCommand();
                        InventoryCmd.CommandType = CommandType.Text;
                        InventoryCmd.CommandText = $"Select \"Id\" From \"TLIradioAntennaLibrary\"";
                        OracleDataReader Reader = InventoryCmd.ExecuteReader();
                        if (Reader.HasRows)
                        {
                            while (Reader.Read())
                            {
                                InsertedIds.Add(Reader.GetInt32(0));
                            }
                        }
                    }
                    TLIhistory tLIhistory = new TLIhistory()
                    {
                        UserId = UserId,
                        TablesNameId = TableNameEntity.Id,
                        HistoryTypeId = 1,

                    };
                    _dbContext.TLIhistory.Add(tLIhistory);
                    _dbContext.SaveChanges();
                    var radioOtherLibraryist = new List<TLIradioOtherLibrary>();

                    for (int i = 0; i < models.Count; i++)
                    {
                        radioOtherLibraryist.Add(new TLIradioOtherLibrary
                        {
                            Id = InsertedIds[i],
                            Model = models[i],
                            Notes = notes[i],
                            Width = widths[i],
                            Weight = weights[i],
                            Length = lengths[i],
                            Height = heights[i],
                            SpaceLibrary = SpaceLibraries[i],

                        });
                    }


                    foreach (int recordId in InsertedIds)
                    {
                        var RecordName = radioOtherLibraryist.FirstOrDefault(x => x.Id == recordId);
                        if (RecordName != null)
                        {
                            var attributeNames = RecordName.GetType().GetProperties()
                                .Where(x =>
                                    (x.PropertyType.IsGenericType && x.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>) &&
                                        new Type[] { typeof(int), typeof(string), typeof(double), typeof(float), typeof(Single), typeof(bool), typeof(DateTime) }
                                        .Contains(x.PropertyType.GetGenericArguments()[0]))
                                    || new Type[] { typeof(int), typeof(string), typeof(double), typeof(bool), typeof(DateTime), typeof(float), typeof(Single) }
                                    .Contains(x.PropertyType)
                                )
                                .ToList();

                            foreach (var propertyInfo in attributeNames)
                            {

                                var propertyName = propertyInfo.Name;

                                var propertyValue = propertyInfo.GetValue(RecordName)?.ToString().Trim();

                                TLIhistoryDet tLIhistoryDet = new TLIhistoryDet()
                                {
                                    HistoryId = tLIhistory.Id,
                                    TablesNameId = TableNameEntity.Id,
                                    RecordId = recordId.ToString(),
                                    AttributeName = propertyName,
                                    NewValue = propertyValue
                                };


                                _dbContext.TLIhistoryDet.Add(tLIhistoryDet);
                            }
                        }
                    }
                    _dbContext.SaveChanges();

                    foreach (var DynamicAtt in DynamicAtts)
                    {
                        for (int k = 0; k < DynamicAtt.Item3.Count; k++)
                        {
                            if (!String.IsNullOrEmpty(DynamicAtt.Item3[k].ToString()))
                            {

                                var Message = _unitOfWork.CivilWithLegsRepository.CheckDynamicValidationAndDependenceDynamic(DynamicAtt.Item1, DynamicAtt.Item3[k], Convert.ToInt32(InsertedIds[k]), tLIhistory.Id, radioOtherLibraryist[0]).Message;
                                if (Message != "Success")
                                {
                                    UnsavedRows.Add(new KeyValuePair<int, string>(k + 2, Message + ' ' + $"in the Colum Name{ColName} in the Row Number {k + 2} "));
                                    goto ERROR;
                                }

                            }

                        }
                        //List<string> StringValues = new List<string>();
                        //List<double> DoubleValues = new List<double>();
                        //List<DateTime> DateTimeValues = new List<DateTime>();
                        //List<Int32> BooleanValues = new List<Int32>();
                        //List<int> DynamicAttIds = new List<int>();
                        //List<bool> disables = new List<bool>();
                        //List<int?> sideArmLibIds = new List<int?>();
                        //List<int?> sideArmLibraryIds = new List<int?>();
                        //List<int> InventoryIds = new List<int>();
                        //List<int> tablesNamesIds = new List<int>();
                        //if (DynamicAtt.Item2 == "string")
                        //{
                        //    for (int k = 0; k < DynamicAtt.Item3.Count; k++)
                        //    {
                        //        if (!String.IsNullOrEmpty(DynamicAtt.Item3[k].ToString()))
                        //        {
                        //            StringValues.Add(DynamicAtt.Item3[k].ToString());
                        //            DynamicAttIds.Add(DynamicAtt.Item1);
                        //            disables.Add(false);
                        //            sideArmLibIds.Add(null);
                        //            sideArmLibraryIds.Add(null);
                        //            InventoryIds.Add(Convert.ToInt32(InsertedIds[k]));
                        //            tablesNamesIds.Add(TableNameEntity.Id);
                        //        }
                        //    }
                        //}
                        //if (DynamicAtt.Item2 == "double")
                        //{
                        //    for (int k = 0; k < DynamicAtt.Item3.Count; k++)
                        //    {
                        //        if (!String.IsNullOrEmpty(DynamicAtt.Item3[k].ToString()))
                        //        {
                        //            DoubleValues.Add(Convert.ToInt32(DynamicAtt.Item3[k]));
                        //            DynamicAttIds.Add(DynamicAtt.Item1);
                        //            disables.Add(false);
                        //            sideArmLibIds.Add(null);
                        //            sideArmLibraryIds.Add(null);
                        //            InventoryIds.Add(Convert.ToInt32(InsertedIds[k]));
                        //            tablesNamesIds.Add(TableNameEntity.Id);
                        //        }
                        //    }
                        //}
                        //if (DynamicAtt.Item2 == "datetime")
                        //{
                        //    for (int k = 0; k < DynamicAtt.Item3.Count; k++)
                        //    {
                        //        if (!String.IsNullOrEmpty(DynamicAtt.Item3[k].ToString()))
                        //        {
                        //            DateTimeValues.Add(Convert.ToDateTime(DynamicAtt.Item3[k]));
                        //            DynamicAttIds.Add(DynamicAtt.Item1);
                        //            disables.Add(false);
                        //            sideArmLibIds.Add(null);
                        //            sideArmLibraryIds.Add(null);
                        //            InventoryIds.Add(Convert.ToInt32(InsertedIds[k]));
                        //            tablesNamesIds.Add(TableNameEntity.Id);
                        //        }
                        //    }
                        //}
                        //if (DynamicAtt.Item2 == "boolean")
                        //{
                        //    for (int k = 0; k < DynamicAtt.Item3.Count; k++)
                        //    {
                        //        if (!String.IsNullOrEmpty(DynamicAtt.Item3[k].ToString()))
                        //        {
                        //            bool BoolValue = Convert.ToBoolean(DynamicAtt.Item3[k]);
                        //            if (BoolValue == true)
                        //            {
                        //                BooleanValues.Add(1);
                        //            }
                        //            else
                        //            {
                        //                BooleanValues.Add(0);
                        //            }
                        //            DynamicAttIds.Add(DynamicAtt.Item1);
                        //            disables.Add(false);
                        //            sideArmLibIds.Add(null);
                        //            sideArmLibraryIds.Add(null);
                        //            InventoryIds.Add(Convert.ToInt32(InsertedIds[k]));
                        //            tablesNamesIds.Add(TableNameEntity.Id);
                        //        }
                        //    }
                        //}
                        ////connection.Open();
                        //OracleParameter value = new OracleParameter();
                        //if (DynamicAtt.Item2 == "string")
                        //{
                        //    value.OracleDbType = OracleDbType.NVarchar2;
                        //    value.Value = StringValues.ToArray();
                        //}
                        //else if (DynamicAtt.Item2 == "double")
                        //{
                        //    value.OracleDbType = OracleDbType.BinaryDouble;
                        //    value.Value = DoubleValues.ToArray();
                        //}
                        //else if (DynamicAtt.Item2 == "datetime")
                        //{
                        //    value.OracleDbType = OracleDbType.Date;
                        //    value.Value = DateTimeValues.ToArray();
                        //}
                        //else if (DynamicAtt.Item2 == "boolean")
                        //{
                        //    value.OracleDbType = OracleDbType.Int32;
                        //    value.Value = BooleanValues.ToArray();
                        //}
                        //OracleParameter DynamicAttId = new OracleParameter();
                        //DynamicAttId.OracleDbType = OracleDbType.Int32;
                        //DynamicAttId.Value = DynamicAttIds.ToArray();

                        //OracleParameter disable = new OracleParameter();
                        //disable.OracleDbType = OracleDbType.Boolean;
                        //disable.Value = disables.ToArray();

                        //OracleParameter sideArmLibId = new OracleParameter();
                        //sideArmLibId.OracleDbType = OracleDbType.Int32;
                        //sideArmLibId.Value = sideArmLibIds.ToArray();

                        //OracleParameter sideArmLibraryId = new OracleParameter();
                        //sideArmLibraryId.OracleDbType = OracleDbType.Int32;
                        //sideArmLibraryId.Value = sideArmLibraryIds.ToArray();

                        //OracleParameter InventoryId = new OracleParameter();
                        //InventoryId.OracleDbType = OracleDbType.Int32;
                        //InventoryId.Value = InventoryIds.ToArray();

                        //OracleParameter tablesNamesId = new OracleParameter();
                        //tablesNamesId.OracleDbType = OracleDbType.Int32;
                        //tablesNamesId.Value = tablesNamesIds.ToArray();

                        //OracleCommand dalvcmd = connection.CreateCommand();
                        //if (DynamicAtt.Item2 == "string")
                        //{
                        //    dalvcmd.CommandText = "INSERT INTO \"TLIdynamicAttLibValue\" (\"DynamicAttId\", \"InventoryId\", \"tablesNamesId\", \"ValueString\") VALUES ( :1, :2, :3, :4)";
                        //    dalvcmd.ArrayBindCount = StringValues.Count;
                        //}
                        //else if (DynamicAtt.Item2 == "double")
                        //{
                        //    dalvcmd.CommandText = "INSERT INTO \"TLIdynamicAttLibValue\" (\"DynamicAttId\", \"InventoryId\", \"tablesNamesId\", \"ValueDouble\") VALUES ( :1, :2, :3, :4)";
                        //    dalvcmd.ArrayBindCount = DoubleValues.Count;
                        //}
                        //else if (DynamicAtt.Item2 == "datetime")
                        //{
                        //    dalvcmd.CommandText = "INSERT INTO \"TLIdynamicAttLibValue\" (\"DynamicAttId\", \"InventoryId\", \"tablesNamesId\", \"ValueDateTime\") VALUES ( :1, :2, :3, :4)";
                        //    dalvcmd.ArrayBindCount = DateTimeValues.Count;
                        //}
                        //else if (DynamicAtt.Item2 == "boolean")
                        //{
                        //    dalvcmd.CommandText = "INSERT INTO \"TLIdynamicAttLibValue\" (\"DynamicAttId\", \"InventoryId\", \"tablesNamesId\", \"ValueBoolean\") VALUES ( :1, :2, :3, :4)";
                        //    dalvcmd.ArrayBindCount = BooleanValues.Count;
                        //}
                        ////dalvcmd.Parameters.Clear();
                        //dalvcmd.Parameters.Add(DynamicAttId);
                        //dalvcmd.Parameters.Add(InventoryId);
                        //dalvcmd.Parameters.Add(tablesNamesId);
                        //dalvcmd.Parameters.Add(value);
                        //dalvcmd.ExecuteNonQuery();
                    }
                    RecordId.AddRange(InsertedIds);
                }
            ERROR:;
            }
            catch (Exception err)
            {

                throw err;
            }
        }

        private void SaveradioRRULibraryUsingOracleBulkCopy(int UserId, out List<int> RecordId, DataTable dt, ref List<KeyValuePair<int, string>> UnsavedRows, int ActColumns, int Columns, ExcelWorksheet sheet, List<KeyValuePair<string, List<DropDownListFilters>>> RelatedTables, TLItablesNames TableNameEntity, OracleConnection connection)
        {
            try
            {
                RecordId = new List<int>();
                List<string> Models = _unitOfWork.RadioRRULibraryRepository.GetSelect(x => x.Model).ToList();

                TLIradioRRULibrary LastId = _serviceProvider.GetService<ApplicationDbContext>().
                    TLIradioRRULibrary.OrderByDescending(a => a.Id).FirstOrDefault();

                List<TLIdynamicAtt> RadioRRULibraryDynamicAtts = _unitOfWork.DynamicAttRepository.GetIncludeWhere(x =>
                    x.tablesNamesId == TableNameEntity.Id && !x.disable, x => x.DataType).ToList();

                List<string> models = new List<string>();
                List<string> types = new List<string>();
                List<string> bands = new List<string>();
                List<float> channelbandwidths = new List<float>();
                List<float> weights = new List<float>();
                List<float> lengths = new List<float>();
                List<float> Depths = new List<float>();
                List<float> widths = new List<float>();
                List<float> heights = new List<float>();
                List<string> notes = new List<string>();
                List<float> SpaceLibraries = new List<float>();
                List<Tuple<int, string, List<dynamic>>> DynamicAtts = new List<Tuple<int, string, List<dynamic>>>();
                TLIdynamicAtt DA = new TLIdynamicAtt();
                string ColName = null;
                //List<string> test = new List<string>();
                if (ActColumns > 0)
                {
                    for (int i = ActColumns; i <= Columns; i++)
                    {
                        ColName = sheet.Cells[1, i].Value.ToString();
                        DA = RadioRRULibraryDynamicAtts.FirstOrDefault(x => x.Key == ColName);
                        if (DA != null)
                            DynamicAtts.Add(new Tuple<int, string, List<dynamic>>(DA.Id, DA.DataType.Name.ToLower(), new List<dynamic>()));
                    }
                }
                bool test;
                for (int j = 0; j < dt.Rows.Count; j++)
                {
                    try
                    {
                        string model_test = null;

                        if (dt.Columns.Contains("Model"))
                        {
                            if ((Models.Find(x => x == Convert.ToString(dt.Rows[j]["Model"]))) != null)
                            {
                                UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"Model is already exist in the row {j + 2}"));
                                goto ERROR;
                            }

                            if (!String.IsNullOrEmpty(dt.Rows[j]["Model"].ToString()))
                            {
                                model_test = Convert.ToString(dt.Rows[j]["Model"]);
                            }
                            else
                            {
                                UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"Model can not to be null In The Row {j + 2}"));
                                goto ERROR;
                            }
                        }

                        string type_test = null;
                        if (dt.Columns.Contains("Type"))
                        {
                            if (!String.IsNullOrEmpty(dt.Rows[j]["Type"].ToString()))
                            {
                                type_test = Convert.ToString(dt.Rows[j]["Type"]);
                            }
                        }

                        string band_test = null;
                        if (dt.Columns.Contains("Band"))
                        {
                            if (!String.IsNullOrEmpty(dt.Rows[j]["Band"].ToString()))
                            {
                                band_test = Convert.ToString(dt.Rows[j]["Band"]);
                            }
                        }

                        float channelbandwidth_test = 0;
                        if (dt.Columns.Contains("ChannelBandwidth"))
                        {
                            test = float.TryParse(dt.Rows[j]["ChannelBandwidth"].ToString(), out channelbandwidth_test);
                            if (test == false)
                            {
                                UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"ChannelBandwidth Wrong Input DataType In The Row {j + 2}"));
                                goto ERROR;
                            }
                        }

                        float Depth_test = 0;
                        if (dt.Columns.Contains("Depth"))
                        {
                            test = float.TryParse(dt.Rows[j]["Depth"].ToString(), out Depth_test);
                            if (test == false)
                            {
                                UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"Depth Wrong Input DataType In The Row {j + 2}"));
                                goto ERROR;
                            }
                        }

                        float weight_test = 0;
                        if (dt.Columns.Contains("Weight"))
                        {
                            test = float.TryParse(dt.Rows[j]["Weight"].ToString(), out weight_test);
                            if (test == false)
                            {
                                UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"Weight Wrong Input DataType In The Row {j + 2}"));
                                goto ERROR;
                            }
                        }

                        float length_test = 0;
                        if (dt.Columns.Contains("Length"))
                        {
                            test = float.TryParse(dt.Rows[j]["Length"].ToString(), out length_test);
                            if (test == false)
                            {
                                UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"Length Wrong Input DataType In The Row {j + 2}"));
                                goto ERROR;
                            }
                        }

                        float width_test = 0;
                        if (dt.Columns.Contains("Width"))
                        {
                            test = float.TryParse(dt.Rows[j]["Width"].ToString(), out width_test);
                            if (test == false)
                            {
                                UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"Width Wrong Input DataType In The Row {j + 2}"));
                                goto ERROR;
                            }
                        }

                        float height_test = 0;
                        if (dt.Columns.Contains("Height"))
                        {
                            test = float.TryParse(dt.Rows[j]["Height"].ToString(), out height_test);
                            if (test == false)
                            {
                                UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"Height Wrong Input DataType In The Row {j + 2}"));
                                goto ERROR;
                            }
                        }

                        string note_test = null;
                        if (dt.Columns.Contains("Notes"))
                        {
                            if (!String.IsNullOrEmpty(dt.Rows[j]["Notes"].ToString()))
                            {
                                note_test = Convert.ToString(dt.Rows[j]["Notes"]);
                            }
                        }

                        float spacelibrary_test = 0;
                      
                        if (length_test == 0)
                        {
                            UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"length must to be bigger of zero in the row{j + 2}"));
                            goto ERROR;
                        }
                        if (width_test == 0)
                        {
                            UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"width must to be bigger of zero in the row {j + 2}"));
                            goto ERROR;
                        }
                        else
                        {
                            spacelibrary_test = length_test * width_test;
                        }

                            
                        

                        List<dynamic> DynamicAttList = new List<dynamic>();
                        if (ActColumns > 0)
                        {
                            for (int i = ActColumns; i <= Columns; i++)
                            {
                                string StringDynamicAtt = string.Empty;
                                double? DoubleDynamicAtt = null;
                                DateTime? DateTimeDynamicAtt = null;
                                Boolean? BooleanDynamicAtt = null;
                                ColName = sheet.Cells[1, i].Value.ToString();
                                DA = RadioRRULibraryDynamicAtts.FirstOrDefault(x => x.Key == ColName);
                                //Check dynamic attribute if required or not
                                if (DA != null)
                                {
                                    double double_Test = 0;
                                    DateTime datetime_Test = DateTime.Now;
                                    Boolean boolean_Test = false;
                                    if (DA.Required == true)
                                    {
                                        //if somthing wrong then add row to UnsavedRows
                                        if (String.IsNullOrEmpty(dt.Rows[j][ColName].ToString()))
                                        {
                                            UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"Dynamic Attribute ({ColName}) is required"));
                                            goto ERROR;
                                        }
                                        else
                                        {
                                            if (DA.DataType.Name.ToLower() == "string")
                                            {

                                                StringDynamicAtt = Convert.ToString(dt.Rows[j][ColName]);
                                            }
                                            else if (DA.DataType.Name.ToLower() == "double" || DA.DataType.Name.ToLower() == "int")
                                            {
                                                test = double.TryParse(dt.Rows[j][ColName].ToString(), out double_Test);
                                                if (test == false)
                                                {
                                                    UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"{ColName} Dynamic Attribute Wrong Input DataType in the row {j + 2}"));
                                                    goto ERROR;
                                                }
                                                DoubleDynamicAtt = double_Test;
                                            }
                                            else if (DA.DataType.Name.ToLower() == "datetime")
                                            {
                                                test = DateTime.TryParse(dt.Rows[j][ColName].ToString(), out datetime_Test);
                                                if (test == false)
                                                {
                                                    UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"{ColName} Dynamic Attribute Wrong Input DataType in the row {j + 2}"));
                                                    goto ERROR;
                                                }
                                                DateTimeDynamicAtt = datetime_Test;
                                            }
                                            else if (DA.DataType.Name.ToLower() == "bool")
                                            {
                                                test = Boolean.TryParse(dt.Rows[j][ColName].ToString(), out boolean_Test);
                                                if (test == false)
                                                {
                                                    UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"{ColName} Dynamic Attribute Wrong Input DataType in the row {j + 2}"));
                                                    goto ERROR;
                                                }
                                                BooleanDynamicAtt = boolean_Test;
                                            }


                                        }
                                    }
                                    else
                                    {
                                        if (!String.IsNullOrEmpty(dt.Rows[j][ColName].ToString()))
                                        {
                                            if (DA.DataType.Name.ToLower() == "string")
                                            {
                                                StringDynamicAtt = Convert.ToString(dt.Rows[j][ColName]);
                                            }
                                            else if (DA.DataType.Name.ToLower() == "double" || DA.DataType.Name.ToLower() == "int")
                                            {
                                                test = double.TryParse(dt.Rows[j][ColName].ToString(), out double_Test);
                                                if (test == false)
                                                {
                                                    UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"{ColName} Dynamic Attribute Wrong Input DataType in the row {j + 2}"));
                                                    goto ERROR;
                                                }
                                                DoubleDynamicAtt = double_Test;
                                            }
                                            else if (DA.DataType.Name.ToLower() == "datetime")
                                            {
                                                test = DateTime.TryParse(dt.Rows[j][ColName].ToString(), out datetime_Test);
                                                if (test == false)
                                                {
                                                    UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"{ColName} Dynamic Attribute Wrong Input DataType in the row {j + 2}"));
                                                    goto ERROR;
                                                }
                                                DateTimeDynamicAtt = datetime_Test;

                                            }
                                            else if (DA.DataType.Name.ToLower() == "bool")
                                            {
                                                test = Boolean.TryParse(dt.Rows[j][ColName].ToString(), out boolean_Test);
                                                if (test == false)
                                                {
                                                    UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"{ColName} Dynamic Attribute Wrong Input DataType in the row {j + 2}"));
                                                    goto ERROR;
                                                }
                                                BooleanDynamicAtt = boolean_Test;
                                            }

                                        }
                                    }
                                    if (DA.DataType.Name.ToLower() == "string")
                                    {
                                        DynamicAttList.Add(StringDynamicAtt);
                                    }
                                    else if (DA.DataType.Name.ToLower() == "double" || DA.DataType.Name.ToLower() == "int")
                                    {
                                        DynamicAttList.Add(DoubleDynamicAtt);
                                    }
                                    else if (DA.DataType.Name.ToLower() == "datetime")
                                    {
                                        DynamicAttList.Add(DateTimeDynamicAtt);
                                    }
                                    else if (DA.DataType.Name.ToLower() == "bool")
                                    {
                                        DynamicAttList.Add(BooleanDynamicAtt);
                                    }


                                }
                            }
                        }
                        models.Add(model_test != null ? model_test : null);
                        types.Add(type_test != null ? type_test : null);
                        bands.Add(band_test != null ? band_test : null);
                        channelbandwidths.Add(channelbandwidth_test != 0 ? channelbandwidth_test : 0);
                        weights.Add(weight_test != 0 ? weight_test : 0);
                        lengths.Add(length_test != 0 ? length_test : 0);
                        widths.Add(width_test != 0 ? width_test : 0);
                        heights.Add(height_test != 0 ? height_test : 0);
                        heights.Add(Depth_test != 0 ? Depth_test : 0);
                        notes.Add(note_test != null ? note_test : null);
                        SpaceLibraries.Add(spacelibrary_test != 0 ? spacelibrary_test : 0);
                        Depths.Add(Depth_test != 0 ? Depth_test : 0);
                        for (int f = 0; f < DynamicAttList.Count; f++)
                        {
                            //var h = DynamicAtts[f].Value;
                            DynamicAtts[f].Item3.Add(DynamicAttList[f]);
                            //h.Add(DynamicAttList[f]);
                        }


                    }
                    catch (Exception err)
                    {

                        // UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, "Wrong Input DataType"));
                    }
                }
                if (models.Count > 0)
                {
                    OracleParameter model = new OracleParameter();
                    model.OracleDbType = OracleDbType.NVarchar2;
                    model.Value = models.ToArray();

                    OracleParameter Type = new OracleParameter();
                    Type.OracleDbType = OracleDbType.NVarchar2;
                    Type.Value = types.ToArray();

                    OracleParameter Band = new OracleParameter();
                    Band.OracleDbType = OracleDbType.NVarchar2;
                    Band.Value = bands.ToArray();

                    OracleParameter ChannelBandwidth = new OracleParameter();
                    ChannelBandwidth.OracleDbType = OracleDbType.BinaryFloat;
                    ChannelBandwidth.Value = channelbandwidths.ToArray();

                    OracleParameter Weight = new OracleParameter();
                    Weight.OracleDbType = OracleDbType.BinaryFloat;
                    Weight.Value = weights.ToArray();

                    OracleParameter Depth = new OracleParameter();
                    Depth.OracleDbType = OracleDbType.BinaryFloat;
                    Depth.Value = Depths.ToArray();

                    OracleParameter Length = new OracleParameter();
                    Length.OracleDbType = OracleDbType.BinaryFloat;
                    Length.Value = lengths.ToArray();

                    OracleParameter Width = new OracleParameter();
                    Width.OracleDbType = OracleDbType.BinaryFloat;
                    Width.Value = widths.ToArray();

                    OracleParameter Height = new OracleParameter();
                    Height.OracleDbType = OracleDbType.BinaryFloat;
                    Height.Value = heights.ToArray();

                    OracleParameter note = new OracleParameter();
                    note.OracleDbType = OracleDbType.NVarchar2;
                    note.Value = notes.ToArray();

                    OracleParameter SpaceLibrary = new OracleParameter();
                    SpaceLibrary.OracleDbType = OracleDbType.BinaryFloat;
                    SpaceLibrary.Value = SpaceLibraries.ToArray();

                    // create command and set properties
                    OracleCommand cmd = connection.CreateCommand();
                    cmd.CommandText = "INSERT INTO \"TLIradioRRULibrary\" (\"Model\", \"Type\", \"Band\", \"ChannelBandwidth\", \"Weight\", \"L_W_H_cm3\", \"Length\", \"Width\", \"Height\", \"Notes\", \"SpaceLibrary\", \"Depth\") VALUES ( :1, :2, :3, :4, :5, :6, :7, :8, :9, :10, :11 ,:12)";
                    cmd.ArrayBindCount = models.Count;
                    cmd.Parameters.Add(model);
                    cmd.Parameters.Add(Type);
                    cmd.Parameters.Add(Band);
                    cmd.Parameters.Add(ChannelBandwidth);
                    cmd.Parameters.Add(Weight);
                    cmd.Parameters.Add(new OracleParameter(":6", OracleDbType.NVarchar2) { Value = DBNull.Value });
                    cmd.Parameters.Add(Length);
                    cmd.Parameters.Add(Width);
                    cmd.Parameters.Add(Height);
                    cmd.Parameters.Add(note);
                    cmd.Parameters.Add(SpaceLibrary);
                    cmd.Parameters.Add(Depth);
                    cmd.ExecuteNonQuery();
                    //connection.Close();
                    List<int> InsertedIds = new List<int>();
                    if (LastId != null)
                    {
                        OracleCommand InventoryCmd = connection.CreateCommand();
                        InventoryCmd.CommandType = CommandType.Text;
                        InventoryCmd.CommandText = $"Select \"Id\" From \"TLIradioRRULibrary\" where \"TLIradioRRULibrary\".\"Id\" > {LastId.Id}";
                        OracleDataReader Reader = InventoryCmd.ExecuteReader();
                        if (Reader.HasRows)
                        {
                            while (Reader.Read())
                            {
                                InsertedIds.Add(Reader.GetInt32(0));
                            }
                        }
                    }
                    else
                    {
                        OracleCommand InventoryCmd = connection.CreateCommand();
                        InventoryCmd.CommandType = CommandType.Text;
                        InventoryCmd.CommandText = $"Select \"Id\" From \"TLIradioRRULibrary\"";
                        OracleDataReader Reader = InventoryCmd.ExecuteReader();
                        if (Reader.HasRows)
                        {
                            while (Reader.Read())
                            {
                                InsertedIds.Add(Reader.GetInt32(0));
                            }
                        }
                    }
                    TLIhistory tLIhistory = new TLIhistory()
                    {
                        UserId = UserId,
                        TablesNameId = TableNameEntity.Id,
                        HistoryTypeId = 1,

                    };
                    _dbContext.TLIhistory.Add(tLIhistory);
                    _dbContext.SaveChanges();
                    var radioRRULibraryist = new List<TLIradioRRULibrary>();

                    for (int i = 0; i < models.Count; i++)
                    {
                        radioRRULibraryist.Add(new TLIradioRRULibrary
                        {
                            Id = InsertedIds[i],
                            Model = models[i],
                            Notes = notes[i],
                            ChannelBandwidth = channelbandwidths[i],
                            Band = bands[i],
                            Type = types[i],
                            Width = widths[i],
                            Weight = weights[i],
                            Length = lengths[i],
                            Height = heights[i],
                            Depth = Depths[i],
                            SpaceLibrary = SpaceLibraries[i],

                        });
                    }


                    foreach (int recordId in InsertedIds)
                    {
                        var RecordName = radioRRULibraryist.FirstOrDefault(x => x.Id == recordId);
                        if (RecordName != null)
                        {
                            var attributeNames = RecordName.GetType().GetProperties()
                                .Where(x =>
                                    (x.PropertyType.IsGenericType && x.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>) &&
                                        new Type[] { typeof(int), typeof(string), typeof(double), typeof(float), typeof(Single), typeof(bool), typeof(DateTime) }
                                        .Contains(x.PropertyType.GetGenericArguments()[0]))
                                    || new Type[] { typeof(int), typeof(string), typeof(double), typeof(bool), typeof(DateTime), typeof(float), typeof(Single) }
                                    .Contains(x.PropertyType)
                                )
                                .ToList();

                            foreach (var propertyInfo in attributeNames)
                            {

                                var propertyName = propertyInfo.Name;

                                var propertyValue = propertyInfo.GetValue(RecordName)?.ToString().Trim();

                                TLIhistoryDet tLIhistoryDet = new TLIhistoryDet()
                                {
                                    HistoryId = tLIhistory.Id,
                                    TablesNameId = TableNameEntity.Id,
                                    RecordId = recordId.ToString(),
                                    AttributeName = propertyName,
                                    NewValue = propertyValue
                                };


                                _dbContext.TLIhistoryDet.Add(tLIhistoryDet);
                            }
                        }
                    }
                    _dbContext.SaveChanges();

                    foreach (var DynamicAtt in DynamicAtts)
                    {
                        for (int k = 0; k < DynamicAtt.Item3.Count; k++)
                        {
                            if (!String.IsNullOrEmpty(DynamicAtt.Item3[k].ToString()))
                            {

                                var Message = _unitOfWork.CivilWithLegsRepository.CheckDynamicValidationAndDependenceDynamic(DynamicAtt.Item1, DynamicAtt.Item3[k], Convert.ToInt32(InsertedIds[k]), tLIhistory.Id, radioRRULibraryist[0]).Message;
                                if (Message != "Success")
                                {
                                    UnsavedRows.Add(new KeyValuePair<int, string>(k + 2, Message + ' ' + $"in the Colum Name{ColName} in the Row Number {k + 2} "));
                                    goto ERROR;
                                }

                            }

                        }
                        //List<string> StringValues = new List<string>();
                        //List<double> DoubleValues = new List<double>();
                        //List<DateTime> DateTimeValues = new List<DateTime>();
                        //List<Int32> BooleanValues = new List<Int32>();
                        //List<int> DynamicAttIds = new List<int>();
                        //List<bool> disables = new List<bool>();
                        //List<int?> sideArmLibIds = new List<int?>();
                        //List<int?> sideArmLibraryIds = new List<int?>();
                        //List<int> InventoryIds = new List<int>();
                        //List<int> tablesNamesIds = new List<int>();
                        //if (DynamicAtt.Item2 == "string")
                        //{
                        //    for (int k = 0; k < DynamicAtt.Item3.Count; k++)
                        //    {
                        //        if (!String.IsNullOrEmpty(DynamicAtt.Item3[k].ToString()))
                        //        {
                        //            StringValues.Add(DynamicAtt.Item3[k].ToString());
                        //            DynamicAttIds.Add(DynamicAtt.Item1);
                        //            disables.Add(false);
                        //            sideArmLibIds.Add(null);
                        //            sideArmLibraryIds.Add(null);
                        //            InventoryIds.Add(Convert.ToInt32(InsertedIds[k]));
                        //            tablesNamesIds.Add(TableNameEntity.Id);
                        //        }
                        //    }
                        //}
                        //if (DynamicAtt.Item2 == "double")
                        //{
                        //    for (int k = 0; k < DynamicAtt.Item3.Count; k++)
                        //    {
                        //        if (!String.IsNullOrEmpty(DynamicAtt.Item3[k].ToString()))
                        //        {
                        //            DoubleValues.Add(Convert.ToInt32(DynamicAtt.Item3[k]));
                        //            DynamicAttIds.Add(DynamicAtt.Item1);
                        //            disables.Add(false);
                        //            sideArmLibIds.Add(null);
                        //            sideArmLibraryIds.Add(null);
                        //            InventoryIds.Add(Convert.ToInt32(InsertedIds[k]));
                        //            tablesNamesIds.Add(TableNameEntity.Id);
                        //        }
                        //    }
                        //}
                        //if (DynamicAtt.Item2 == "datetime")
                        //{
                        //    for (int k = 0; k < DynamicAtt.Item3.Count; k++)
                        //    {
                        //        if (!String.IsNullOrEmpty(DynamicAtt.Item3[k].ToString()))
                        //        {
                        //            DateTimeValues.Add(Convert.ToDateTime(DynamicAtt.Item3[k]));
                        //            DynamicAttIds.Add(DynamicAtt.Item1);
                        //            disables.Add(false);
                        //            sideArmLibIds.Add(null);
                        //            sideArmLibraryIds.Add(null);
                        //            InventoryIds.Add(Convert.ToInt32(InsertedIds[k]));
                        //            tablesNamesIds.Add(TableNameEntity.Id);
                        //        }
                        //    }
                        //}
                        //if (DynamicAtt.Item2 == "boolean")
                        //{
                        //    for (int k = 0; k < DynamicAtt.Item3.Count; k++)
                        //    {
                        //        if (!String.IsNullOrEmpty(DynamicAtt.Item3[k].ToString()))
                        //        {
                        //            bool BoolValue = Convert.ToBoolean(DynamicAtt.Item3[k]);
                        //            if (BoolValue == true)
                        //            {
                        //                BooleanValues.Add(1);
                        //            }
                        //            else
                        //            {
                        //                BooleanValues.Add(0);
                        //            }
                        //            DynamicAttIds.Add(DynamicAtt.Item1);
                        //            disables.Add(false);
                        //            sideArmLibIds.Add(null);
                        //            sideArmLibraryIds.Add(null);
                        //            InventoryIds.Add(Convert.ToInt32(InsertedIds[k]));
                        //            tablesNamesIds.Add(TableNameEntity.Id);
                        //        }
                        //    }
                        //}
                        ////connection.Open();
                        //OracleParameter value = new OracleParameter();
                        //if (DynamicAtt.Item2 == "string")
                        //{
                        //    value.OracleDbType = OracleDbType.NVarchar2;
                        //    value.Value = StringValues.ToArray();
                        //}
                        //else if (DynamicAtt.Item2 == "double")
                        //{
                        //    value.OracleDbType = OracleDbType.BinaryDouble;
                        //    value.Value = DoubleValues.ToArray();
                        //}
                        //else if (DynamicAtt.Item2 == "datetime")
                        //{
                        //    value.OracleDbType = OracleDbType.Date;
                        //    value.Value = DateTimeValues.ToArray();
                        //}
                        //else if (DynamicAtt.Item2 == "boolean")
                        //{
                        //    value.OracleDbType = OracleDbType.Int32;
                        //    value.Value = BooleanValues.ToArray();
                        //}
                        //OracleParameter DynamicAttId = new OracleParameter();
                        //DynamicAttId.OracleDbType = OracleDbType.Int32;
                        //DynamicAttId.Value = DynamicAttIds.ToArray();

                        //OracleParameter disable = new OracleParameter();
                        //disable.OracleDbType = OracleDbType.Boolean;
                        //disable.Value = disables.ToArray();

                        //OracleParameter sideArmLibId = new OracleParameter();
                        //sideArmLibId.OracleDbType = OracleDbType.Int32;
                        //sideArmLibId.Value = sideArmLibIds.ToArray();

                        //OracleParameter sideArmLibraryId = new OracleParameter();
                        //sideArmLibraryId.OracleDbType = OracleDbType.Int32;
                        //sideArmLibraryId.Value = sideArmLibraryIds.ToArray();

                        //OracleParameter InventoryId = new OracleParameter();
                        //InventoryId.OracleDbType = OracleDbType.Int32;
                        //InventoryId.Value = InventoryIds.ToArray();

                        //OracleParameter tablesNamesId = new OracleParameter();
                        //tablesNamesId.OracleDbType = OracleDbType.Int32;
                        //tablesNamesId.Value = tablesNamesIds.ToArray();

                        //OracleCommand dalvcmd = connection.CreateCommand();
                        //if (DynamicAtt.Item2 == "string")
                        //{
                        //    dalvcmd.CommandText = "INSERT INTO \"TLIdynamicAttLibValue\" (\"DynamicAttId\", \"InventoryId\", \"tablesNamesId\", \"ValueString\") VALUES ( :1, :2, :3, :4)";
                        //    dalvcmd.ArrayBindCount = StringValues.Count;
                        //}
                        //else if (DynamicAtt.Item2 == "double")
                        //{
                        //    dalvcmd.CommandText = "INSERT INTO \"TLIdynamicAttLibValue\" (\"DynamicAttId\", \"InventoryId\", \"tablesNamesId\", \"ValueDouble\") VALUES ( :1, :2, :3, :4)";
                        //    dalvcmd.ArrayBindCount = DoubleValues.Count;
                        //}
                        //else if (DynamicAtt.Item2 == "datetime")
                        //{
                        //    dalvcmd.CommandText = "INSERT INTO \"TLIdynamicAttLibValue\" (\"DynamicAttId\", \"InventoryId\", \"tablesNamesId\", \"ValueDateTime\") VALUES ( :1, :2, :3, :4)";
                        //    dalvcmd.ArrayBindCount = DateTimeValues.Count;
                        //}
                        //else if (DynamicAtt.Item2 == "boolean")
                        //{
                        //    dalvcmd.CommandText = "INSERT INTO \"TLIdynamicAttLibValue\" (\"DynamicAttId\", \"InventoryId\", \"tablesNamesId\", \"ValueBoolean\") VALUES ( :1, :2, :3, :4)";
                        //    dalvcmd.ArrayBindCount = BooleanValues.Count;
                        //}
                        ////dalvcmd.Parameters.Clear();
                        //dalvcmd.Parameters.Add(DynamicAttId);
                        //dalvcmd.Parameters.Add(InventoryId);
                        //dalvcmd.Parameters.Add(tablesNamesId);
                        //dalvcmd.Parameters.Add(value);
                        //dalvcmd.ExecuteNonQuery();
                    }
                    RecordId.AddRange(InsertedIds);
                }
            ERROR:;
            }
            catch (Exception err)
            {

                throw err;
            }
        }

        private void SavecabinetPowerLibraryUsingOracleBulkCopy(int UserId, out List<int> RecordId, DataTable dt, ref List<KeyValuePair<int, string>> UnsavedRows, int ActColumns, int Columns, ExcelWorksheet sheet, List<KeyValuePair<string, List<DropDownListFilters>>> RelatedTables, TLItablesNames TableNameEntity, OracleConnection connection)
        {
            try
            {
                RecordId = new List<int>();
                List<string> Models = _unitOfWork.CabinetPowerLibraryRepository.GetSelect(x => x.Model).ToList();

                TLIcabinetPowerLibrary LastId = _serviceProvider.GetService<ApplicationDbContext>().
                    TLIcabinetPowerLibrary.OrderByDescending(a => a.Id).FirstOrDefault();

                List<TLIdynamicAtt> CabinetPowerLibraryDynamicAtts = _unitOfWork.DynamicAttRepository.GetIncludeWhere(x =>
                    x.tablesNamesId == TableNameEntity.Id && !x.disable, x => x.DataType).ToList();

                List<string> models = new List<string>();
                List<float> weights = new List<float>();
                List<int> numberofbatteriesList = new List<int>();
                List<string> LayoutCodes = new List<string>();
                List<string> Dimension_W_D_H_List = new List<string>();
                List<float> batteryweights = new List<float>();
                List<string> batterytypes = new List<string>();
                List<string> BatteryDimension_W_D_H_List = new List<string>();
                List<float> spaceLibraries = new List<float>();
                List<int> CabinetPowerTypeIds = new List<int>();
                List<float> SpaceLibraries = new List<float>();
                List<int?> IntegratedWiths = new List<int?>();
                List<bool> PowerIntegrateds = new List<bool>();
                List<IntegratedWith> IntegratedWithss = new List<IntegratedWith>();
                List<Tuple<int, string, List<dynamic>>> DynamicAtts = new List<Tuple<int, string, List<dynamic>>>();
                TLIdynamicAtt DA = new TLIdynamicAtt();
                string ColName = null;
                //List<string> test = new List<string>();
                if (ActColumns > 0)
                {
                    for (int i = ActColumns; i <= Columns; i++)
                    {
                        ColName = sheet.Cells[1, i].Value.ToString();
                        DA = CabinetPowerLibraryDynamicAtts.FirstOrDefault(x => x.Key == ColName);
                        if (DA != null)
                            DynamicAtts.Add(new Tuple<int, string, List<dynamic>>(DA.Id, DA.DataType.Name.ToLower(), new List<dynamic>()));
                    }
                }
                bool test;
                for (int j = 0; j < dt.Rows.Count; j++)
                {
                    try
                    {
                        string model_test = null;

                        if (dt.Columns.Contains("Model"))
                        {
                            if ((Models.Find(x => x == Convert.ToString(dt.Rows[j]["Model"]))) != null)
                            {
                                UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"Model is already exist  in the row {j + 2}"));
                                goto ERROR;
                            }
                            if (!String.IsNullOrEmpty(dt.Rows[j]["Model"].ToString()))
                            {
                                model_test = Convert.ToString(dt.Rows[j]["Model"]);
                            }
                        }

                        float Weight_test = 0;
                        if (dt.Columns.Contains("Weight"))
                        {
                            if (!String.IsNullOrEmpty(dt.Rows[j]["Weight"].ToString()))
                            {
                                float Weight_test_1;
                                test = float.TryParse(dt.Rows[j]["Weight"].ToString(), out Weight_test_1);
                                if (test == true)
                                {
                                    Weight_test = Weight_test_1;
                                }
                                else
                                {
                                    UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"Weight Wrong Input DataType In The Row {j + 2}"));
                                    goto ERROR;
                                }
                            }
                        }

                        int NumberOfBatteries_test = 0;
                        if (dt.Columns.Contains("NumberOfBatteries"))
                        {
                            if (!String.IsNullOrEmpty(dt.Rows[j]["NumberOfBatteries"].ToString()))
                            {
                                int NumberOfBatteries_test_1;
                                test = int.TryParse(dt.Rows[j]["NumberOfBatteries"].ToString(), out NumberOfBatteries_test_1);
                                if (test == true)
                                {
                                    NumberOfBatteries_test = NumberOfBatteries_test_1;
                                }
                                else
                                {
                                    UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"NumberOfBatteries Wrong Input DataType In The Row {j + 2}"));
                                    goto ERROR;
                                }
                            }
                        }

                        string LayoutCode_test = null;
                        if (dt.Columns.Contains("LayoutCode"))
                        {
                            if (!String.IsNullOrEmpty(dt.Rows[j]["LayoutCode"].ToString()))
                            {
                                LayoutCode_test = Convert.ToString(dt.Rows[j]["LayoutCode"]);
                            }
                        }

                        string Dimension_W_D_H_test = null;
                        if (dt.Columns.Contains("Dimension_W_D_H"))
                        {
                            if (!String.IsNullOrEmpty(dt.Rows[j]["Dimension_W_D_H"].ToString()))
                            {
                                Dimension_W_D_H_test = Convert.ToString(dt.Rows[j]["Dimension_W_D_H"]);
                            }
                        }

                        float BatteryWeight_test = 0;
                        if (dt.Columns.Contains("BatteryWeight"))
                        {
                            if (!String.IsNullOrEmpty(dt.Rows[j]["BatteryWeight"].ToString()))
                            {
                                float BatteryWeight_test_1;
                                test = float.TryParse(dt.Rows[j]["BatteryWeight"].ToString(), out BatteryWeight_test_1);
                                if (test == false)
                                {
                                    UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"BatteryWeight Wrong Input DataType In The Row {j + 2}"));
                                    goto ERROR;
                                }
                                else
                                {
                                    BatteryWeight_test = BatteryWeight_test_1;

                                }
                            }
                        }

                        string BatteryType_test = null;
                        if (dt.Columns.Contains("BatteryType"))
                        {
                            if (!String.IsNullOrEmpty(dt.Rows[j]["BatteryType"].ToString()))
                            {
                                BatteryType_test = Convert.ToString(dt.Rows[j]["BatteryType"]);
                            }
                        }

                        string BatteryDimension_W_D_H_test = null;
                        if (dt.Columns.Contains("BatteryDimension_W_D_H"))
                        {
                            if (!String.IsNullOrEmpty(dt.Rows[j]["BatteryDimension_W_D_H"].ToString()))
                            {
                                BatteryDimension_W_D_H_test = Convert.ToString(dt.Rows[j]["BatteryDimension_W_D_H"]);
                            }
                        }

                        float spacelibrary_test = 0;
                        if (dt.Columns.Contains("SpaceLibrary"))
                        {
                            if (!String.IsNullOrEmpty(dt.Rows[j]["SpaceLibrary"].ToString()))
                            {
                                float spacelibrary_test1;
                                test = float.TryParse(dt.Rows[j]["SpaceLibrary"].ToString(), out spacelibrary_test1);
                                if (test == true)
                                {
                                    spacelibrary_test = spacelibrary_test1;
                                }
                                else if (test == true && spacelibrary_test1 == 0)
                                {
                                    UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"SpaceLibrary must to be bigger of zero{j + 2}"));
                                    goto ERROR;
                                }
                                else
                                {
                                    UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"SpaceLibrary Wrong Input DataType in the row {j + 2}"));
                                    goto ERROR;
                                }
                            }
                            else
                            {
                                UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"SpaceLibrary can not to be null and must to be bigger of zero{j + 2}"));
                                goto ERROR;
                            }
                        }

                        int? IntegratedWith_test = null;

                        if (dt.Columns.Contains("IntegratedWith"))
                        {
                            string IntegratedWithValue = dt.Rows[j]["IntegratedWith"].ToString();

                            if (!String.IsNullOrEmpty(IntegratedWithValue))
                            {
                                var relatedRFUType = RelatedTables
                                    .Where(x => x.Key == "IntegratedWith")
                                    .Select(x => x.Value)
                                    .FirstOrDefault()
                                    .Where(x => x.Value == IntegratedWithValue)
                                    .FirstOrDefault();

                                if (relatedRFUType != null)
                                {
                                    IntegratedWith_test = Convert.ToInt32(relatedRFUType.Id);

                                    var IntegratedWithList = Enum.GetValues(typeof(IntegratedWith))
                                        .Cast<IntegratedWith>()
                                        .FirstOrDefault(e => (int)e == IntegratedWith_test);

                                    if (IntegratedWithList != null)
                                    {
                                        IntegratedWithss.Add(IntegratedWithList);
                                    }
                                }
                                else
                                {
                                    UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"Invalid IntegratedWith value '{IntegratedWithValue}' in Row {j + 2}"));
                                    goto ERROR;
                                }
                            }

                        }
                        int CabinetPowerTypeId_test = 0;
                        if (dt.Columns.Contains("CabinetPowerTypeId"))
                        {
                            if (!String.IsNullOrEmpty(dt.Rows[j]["CabinetPowerTypeId"].ToString()))
                            {
                                var CabinetPowerType = RelatedTables.Where(x => x.Key == "CabinetPowerTypeId").Select(x => x.Value).FirstOrDefault().Where(x => x.Value == (dt.Rows[j]["CabinetPowerTypeId"]).ToString()).FirstOrDefault();
                                CabinetPowerTypeId_test = Convert.ToInt32(CabinetPowerType.Id);
                            }
                            else
                            {
                                UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"CabinetPowerTypeId can not to be null in the row {j + 2}"));
                                goto ERROR;
                            }
                        }

                        bool PowerIntegrated_test = false;

                        if (dt.Columns.Contains("PowerIntegrated"))
                        {
                            if (!String.IsNullOrEmpty(dt.Rows[j]["PowerIntegrated"].ToString()))
                            {
                                bool PowerIntegrated_1;
                                test = Boolean.TryParse(dt.Rows[j]["PowerIntegrated"].ToString(), out PowerIntegrated_1);

                                if (!test)
                                {
                                    UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"PowerIntegrated: Wrong Input DataType in Row {j + 2} must to be true or false"));
                                    goto ERROR;
                                }
                                else
                                {
                                    PowerIntegrated_test = PowerIntegrated_1;
                                }
                            }
                        }

                        List<dynamic> DynamicAttList = new List<dynamic>();
                        if (ActColumns > 0)
                        {
                            for (int i = ActColumns; i <= Columns; i++)
                            {
                                string StringDynamicAtt = string.Empty;
                                double? DoubleDynamicAtt = null;
                                DateTime? DateTimeDynamicAtt = null;
                                Boolean? BooleanDynamicAtt = null;
                                ColName = sheet.Cells[1, i].Value.ToString();
                                DA = CabinetPowerLibraryDynamicAtts.FirstOrDefault(x => x.Key == ColName);
                                //Check dynamic attribute if required or not
                                if (DA != null)
                                {
                                    double double_Test = 0;
                                    DateTime datetime_Test = DateTime.Now;
                                    Boolean boolean_Test = false;
                                    if (DA.Required == true)
                                    {
                                        //if somthing wrong then add row to UnsavedRows
                                        if (String.IsNullOrEmpty(dt.Rows[j][ColName].ToString()))
                                        {
                                            UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"Dynamic Attribute ({ColName}) is required"));
                                            goto ERROR;
                                        }
                                        else
                                        {
                                            if (DA.DataType.Name.ToLower() == "string")
                                            {

                                                StringDynamicAtt = Convert.ToString(dt.Rows[j][ColName]);
                                            }
                                            else if (DA.DataType.Name.ToLower() == "double" || DA.DataType.Name.ToLower() == "int")
                                            {
                                                test = double.TryParse(dt.Rows[j][ColName].ToString(), out double_Test);
                                                if (test == false)
                                                {
                                                    UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"{ColName} Dynamic Attribute Wrong Input DataType in the row {j + 2}"));
                                                    goto ERROR;
                                                }
                                                DoubleDynamicAtt = double_Test;
                                            }
                                            else if (DA.DataType.Name.ToLower() == "datetime")
                                            {
                                                test = DateTime.TryParse(dt.Rows[j][ColName].ToString(), out datetime_Test);
                                                if (test == false)
                                                {
                                                    UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"{ColName} Dynamic Attribute Wrong Input DataType in the row {j + 2}"));
                                                    goto ERROR;
                                                }
                                                DateTimeDynamicAtt = datetime_Test;
                                            }
                                            else if (DA.DataType.Name.ToLower() == "bool")
                                            {
                                                test = Boolean.TryParse(dt.Rows[j][ColName].ToString(), out boolean_Test);
                                                if (test == false)
                                                {
                                                    UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"{ColName} Dynamic Attribute Wrong Input DataType in the row {j + 2}"));
                                                    goto ERROR;
                                                }
                                                BooleanDynamicAtt = boolean_Test;
                                            }


                                        }
                                    }
                                    else
                                    {
                                        if (!String.IsNullOrEmpty(dt.Rows[j][ColName].ToString()))
                                        {
                                            if (DA.DataType.Name.ToLower() == "string")
                                            {
                                                StringDynamicAtt = Convert.ToString(dt.Rows[j][ColName]);
                                            }
                                            else if (DA.DataType.Name.ToLower() == "double" || DA.DataType.Name.ToLower() == "int")
                                            {
                                                test = double.TryParse(dt.Rows[j][ColName].ToString(), out double_Test);
                                                if (test == false)
                                                {
                                                    UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"{ColName} Dynamic Attribute Wrong Input DataType in the row {j + 2}"));
                                                    goto ERROR;
                                                }
                                                DoubleDynamicAtt = double_Test;
                                            }
                                            else if (DA.DataType.Name.ToLower() == "datetime")
                                            {
                                                test = DateTime.TryParse(dt.Rows[j][ColName].ToString(), out datetime_Test);
                                                if (test == false)
                                                {
                                                    UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"{ColName} Dynamic Attribute Wrong Input DataType in the row {j + 2}"));
                                                    goto ERROR;
                                                }
                                                DateTimeDynamicAtt = datetime_Test;

                                            }
                                            else if (DA.DataType.Name.ToLower() == "bool")
                                            {
                                                test = Boolean.TryParse(dt.Rows[j][ColName].ToString(), out boolean_Test);
                                                if (test == false)
                                                {
                                                    UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"{ColName} Dynamic Attribute Wrong Input DataType in the row {j + 2}"));
                                                    goto ERROR;
                                                }
                                                BooleanDynamicAtt = boolean_Test;
                                            }

                                        }
                                    }
                                    if (DA.DataType.Name.ToLower() == "string")
                                    {
                                        DynamicAttList.Add(StringDynamicAtt);
                                    }
                                    else if (DA.DataType.Name.ToLower() == "double" || DA.DataType.Name.ToLower() == "int")
                                    {
                                        DynamicAttList.Add(DoubleDynamicAtt);
                                    }
                                    else if (DA.DataType.Name.ToLower() == "datetime")
                                    {
                                        DynamicAttList.Add(DateTimeDynamicAtt);
                                    }
                                    else if (DA.DataType.Name.ToLower() == "bool")
                                    {
                                        DynamicAttList.Add(BooleanDynamicAtt);
                                    }


                                }
                            }
                        }
                        models.Add(model_test != null ? model_test : "NA");
                        weights.Add(Weight_test != 0 ? Weight_test : 0);
                        numberofbatteriesList.Add(NumberOfBatteries_test != 0 ? NumberOfBatteries_test : 0);
                        LayoutCodes.Add(LayoutCode_test != null ? LayoutCode_test : "NA");
                        Dimension_W_D_H_List.Add(Dimension_W_D_H_test != null ? Dimension_W_D_H_test : "NA");
                        batteryweights.Add(BatteryWeight_test != 0 ? BatteryWeight_test : 0);
                        batterytypes.Add(BatteryType_test != null ? BatteryType_test : "NA");
                        BatteryDimension_W_D_H_List.Add(BatteryDimension_W_D_H_test != null ? BatteryDimension_W_D_H_test : "NA");
                        IntegratedWiths.Add(IntegratedWith_test != null ? IntegratedWith_test : null);
                        CabinetPowerTypeIds.Add(CabinetPowerTypeId_test != 0 ? CabinetPowerTypeId_test : 0);
                        SpaceLibraries.Add(spacelibrary_test != 0 ? spacelibrary_test : 0);
                        PowerIntegrateds.Add(PowerIntegrated_test);
                        for (int f = 0; f < DynamicAttList.Count; f++)
                        {
                            //var h = DynamicAtts[f].Value;
                            DynamicAtts[f].Item3.Add(DynamicAttList[f]);
                            //h.Add(DynamicAttList[f]);
                        }


                    }
                    catch (Exception err)
                    {

                        //UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, "Wrong Input DataType"));
                    }
                }
                if (models.Count > 0)
                {
                    OracleParameter model = new OracleParameter();
                    model.OracleDbType = OracleDbType.NVarchar2;
                    model.Value = models.ToArray();

                    OracleParameter Weight = new OracleParameter();
                    Weight.OracleDbType = OracleDbType.BinaryFloat;
                    Weight.Value = weights.ToArray();

                    OracleParameter NumberOfBatteries = new OracleParameter();
                    NumberOfBatteries.OracleDbType = OracleDbType.Int32;
                    NumberOfBatteries.Value = numberofbatteriesList.ToArray();

                    OracleParameter LayoutCode = new OracleParameter();
                    LayoutCode.OracleDbType = OracleDbType.NVarchar2;
                    LayoutCode.Value = LayoutCodes.ToArray();

                    OracleParameter Dimension_W_D_H = new OracleParameter();
                    Dimension_W_D_H.OracleDbType = OracleDbType.NVarchar2;
                    Dimension_W_D_H.Value = Dimension_W_D_H_List.ToArray();

                    OracleParameter BatteryWeight = new OracleParameter();
                    BatteryWeight.OracleDbType = OracleDbType.BinaryFloat;
                    BatteryWeight.Value = batteryweights.ToArray();

                    OracleParameter BatteryType = new OracleParameter();
                    BatteryType.OracleDbType = OracleDbType.NVarchar2;
                    BatteryType.Value = batterytypes.ToArray();

                    OracleParameter BatteryDimension_W_D_H = new OracleParameter();
                    BatteryDimension_W_D_H.OracleDbType = OracleDbType.NVarchar2;
                    BatteryDimension_W_D_H.Value = BatteryDimension_W_D_H_List.ToArray();

                    OracleParameter SpaceLibrary = new OracleParameter();
                    SpaceLibrary.OracleDbType = OracleDbType.BinaryFloat;
                    SpaceLibrary.Value = SpaceLibraries.ToArray();

                    OracleParameter IntegratedWith = new OracleParameter();
                    IntegratedWith.OracleDbType = OracleDbType.Int32;
                    IntegratedWith.Value = IntegratedWiths.ToArray();

                    OracleParameter CabinetPowerTypeId = new OracleParameter();
                    CabinetPowerTypeId.OracleDbType = OracleDbType.Int32;
                    CabinetPowerTypeId.Value = CabinetPowerTypeIds.ToArray();

                    OracleParameter PowerIntegrated = new OracleParameter();
                    PowerIntegrated.OracleDbType = OracleDbType.Int32;
                    PowerIntegrated.Value = PowerIntegrateds.Select(v => v ? 1 : 0).ToArray();

                    // create command and set properties
                    OracleCommand cmd = connection.CreateCommand();
                    cmd.CommandText = "INSERT INTO \"TLIcabinetPowerLibrary\" (\"Model\", \"Weight\", \"NumberOfBatteries\", \"LayoutCode\", \"Dimension_W_D_H\", \"BatteryWeight\", \"BatteryType\", \"BatteryDimension_W_D_H\", \"Depth\", \"Width\", \"Height\", \"SpaceLibrary\", \"CabinetPowerTypeId\", \"IntegratedWith\", \"PowerIntegrated\") VALUES ( :1, :2, :3, :4, :5, :6, :7, :8, 0, 0, 0, :12, :13 ,:14 ,:15)";
                    cmd.ArrayBindCount = models.Count;
                    cmd.Parameters.Add(model);
                    cmd.Parameters.Add(Weight);
                    cmd.Parameters.Add(NumberOfBatteries);
                    cmd.Parameters.Add(LayoutCode);
                    cmd.Parameters.Add(Dimension_W_D_H);
                    cmd.Parameters.Add(BatteryWeight);
                    cmd.Parameters.Add(BatteryType);
                    cmd.Parameters.Add(BatteryDimension_W_D_H);
                    cmd.Parameters.Add(SpaceLibrary);
                    cmd.Parameters.Add(CabinetPowerTypeId);
                    cmd.Parameters.Add(IntegratedWith);
                    cmd.Parameters.Add(PowerIntegrated);
                    cmd.ExecuteNonQuery();
                    //connection.Close();
                    List<int> InsertedIds = new List<int>();
                    if (LastId != null)
                    {
                        OracleCommand InventoryCmd = connection.CreateCommand();
                        InventoryCmd.CommandType = CommandType.Text;
                        InventoryCmd.CommandText = $"Select \"Id\" From \"TLIcabinetPowerLibrary\" where \"TLIcabinetPowerLibrary\".\"Id\" > {LastId.Id}";
                        OracleDataReader Reader = InventoryCmd.ExecuteReader();
                        if (Reader.HasRows)
                        {
                            while (Reader.Read())
                            {
                                InsertedIds.Add(Reader.GetInt32(0));
                            }
                        }
                    }
                    else
                    {
                        OracleCommand InventoryCmd = connection.CreateCommand();
                        InventoryCmd.CommandType = CommandType.Text;
                        InventoryCmd.CommandText = $"Select \"Id\" From \"TLIcabinetPowerLibrary\"";
                        OracleDataReader Reader = InventoryCmd.ExecuteReader();
                        if (Reader.HasRows)
                        {
                            while (Reader.Read())
                            {
                                InsertedIds.Add(Reader.GetInt32(0));
                            }
                        }
                    }
                    TLIhistory tLIhistory = new TLIhistory()
                    {
                        UserId = UserId,
                        TablesNameId = TableNameEntity.Id,
                        HistoryTypeId = 1,

                    };
                    _dbContext.TLIhistory.Add(tLIhistory);
                    _dbContext.SaveChanges();
                    var cabinetPowerLibraryist = new List<TLIcabinetPowerLibrary>();

                    for (int i = 0; i < models.Count; i++)
                    {
                        cabinetPowerLibraryist.Add(new TLIcabinetPowerLibrary
                        {
                            Id = InsertedIds[i],
                            Model = models[i],
                            Weight = weights[i],
                            NumberOfBatteries = numberofbatteriesList[i],
                            LayoutCode = LayoutCodes[i],
                            Dimension_W_D_H = Dimension_W_D_H_List[i],
                            BatteryWeight = batteryweights[i],
                            BatteryType = batterytypes[i],
                            BatteryDimension_W_D_H = BatteryDimension_W_D_H_List[i],
                            CabinetPowerTypeId = CabinetPowerTypeIds[i],
                            PowerIntegrated = PowerIntegrateds[i],
                            IntegratedWith = IntegratedWithss[i],

                        });
                    }


                    foreach (int recordId in InsertedIds)
                    {
                        var RecordName = cabinetPowerLibraryist.FirstOrDefault(x => x.Id == recordId);
                        if (RecordName != null)
                        {
                            var attributeNames = RecordName.GetType().GetProperties()
                                .Where(x =>
                                    (x.PropertyType.IsGenericType && x.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>) &&
                                        new Type[] { typeof(int), typeof(string), typeof(double), typeof(float), typeof(Single), typeof(bool), typeof(DateTime) }
                                        .Contains(x.PropertyType.GetGenericArguments()[0]))
                                    || new Type[] { typeof(int), typeof(string), typeof(double), typeof(bool), typeof(DateTime), typeof(float), typeof(Single) }
                                    .Contains(x.PropertyType)
                                )
                                .ToList();

                            foreach (var propertyInfo in attributeNames)
                            {

                                var propertyName = propertyInfo.Name;

                                var propertyValue = propertyInfo.GetValue(RecordName)?.ToString().Trim();

                                TLIhistoryDet tLIhistoryDet = new TLIhistoryDet()
                                {
                                    HistoryId = tLIhistory.Id,
                                    TablesNameId = TableNameEntity.Id,
                                    RecordId = recordId.ToString(),
                                    AttributeName = propertyName,
                                    NewValue = propertyValue
                                };


                                _dbContext.TLIhistoryDet.Add(tLIhistoryDet);
                            }
                        }
                    }
                    _dbContext.SaveChanges();

                    foreach (var DynamicAtt in DynamicAtts)
                    {
                        for (int k = 0; k < DynamicAtt.Item3.Count; k++)
                        {
                            if (!String.IsNullOrEmpty(DynamicAtt.Item3[k].ToString()))
                            {

                                var Message = _unitOfWork.CivilWithLegsRepository.CheckDynamicValidationAndDependenceDynamic(DynamicAtt.Item1, DynamicAtt.Item3[k], Convert.ToInt32(InsertedIds[k]), tLIhistory.Id, cabinetPowerLibraryist[0]).Message;
                                if (Message != "Success")
                                {
                                    UnsavedRows.Add(new KeyValuePair<int, string>(k + 2, Message + ' ' + $"in the Colum Name{ColName} in the Row Number {k + 2} "));
                                    goto ERROR;
                                }

                            }

                        }
                        //List<string> StringValues = new List<string>();
                        //List<double> DoubleValues = new List<double>();
                        //List<DateTime> DateTimeValues = new List<DateTime>();
                        //List<Int32> BooleanValues = new List<Int32>();
                        //List<int> DynamicAttIds = new List<int>();
                        //List<bool> disables = new List<bool>();
                        //List<int?> sideArmLibIds = new List<int?>();
                        //List<int?> sideArmLibraryIds = new List<int?>();
                        //List<int> InventoryIds = new List<int>();
                        //List<int> tablesNamesIds = new List<int>();
                        //if (DynamicAtt.Item2 == "string")
                        //{
                        //    for (int k = 0; k < DynamicAtt.Item3.Count; k++)
                        //    {
                        //        if (!String.IsNullOrEmpty(DynamicAtt.Item3[k].ToString()))
                        //        {
                        //            StringValues.Add(DynamicAtt.Item3[k].ToString());
                        //            DynamicAttIds.Add(DynamicAtt.Item1);
                        //            disables.Add(false);
                        //            sideArmLibIds.Add(null);
                        //            sideArmLibraryIds.Add(null);
                        //            InventoryIds.Add(Convert.ToInt32(InsertedIds[k]));
                        //            tablesNamesIds.Add(TableNameEntity.Id);
                        //        }
                        //    }
                        //}
                        //if (DynamicAtt.Item2 == "double")
                        //{
                        //    for (int k = 0; k < DynamicAtt.Item3.Count; k++)
                        //    {
                        //        if (!String.IsNullOrEmpty(DynamicAtt.Item3[k].ToString()))
                        //        {
                        //            DoubleValues.Add(Convert.ToInt32(DynamicAtt.Item3[k]));
                        //            DynamicAttIds.Add(DynamicAtt.Item1);
                        //            disables.Add(false);
                        //            sideArmLibIds.Add(null);
                        //            sideArmLibraryIds.Add(null);
                        //            InventoryIds.Add(Convert.ToInt32(InsertedIds[k]));
                        //            tablesNamesIds.Add(TableNameEntity.Id);
                        //        }
                        //    }
                        //}
                        //if (DynamicAtt.Item2 == "datetime")
                        //{
                        //    for (int k = 0; k < DynamicAtt.Item3.Count; k++)
                        //    {
                        //        if (!String.IsNullOrEmpty(DynamicAtt.Item3[k].ToString()))
                        //        {
                        //            DateTimeValues.Add(Convert.ToDateTime(DynamicAtt.Item3[k]));
                        //            DynamicAttIds.Add(DynamicAtt.Item1);
                        //            disables.Add(false);
                        //            sideArmLibIds.Add(null);
                        //            sideArmLibraryIds.Add(null);
                        //            InventoryIds.Add(Convert.ToInt32(InsertedIds[k]));
                        //            tablesNamesIds.Add(TableNameEntity.Id);
                        //        }
                        //    }
                        //}
                        //if (DynamicAtt.Item2 == "boolean")
                        //{
                        //    for (int k = 0; k < DynamicAtt.Item3.Count; k++)
                        //    {
                        //        if (!String.IsNullOrEmpty(DynamicAtt.Item3[k].ToString()))
                        //        {
                        //            bool BoolValue = Convert.ToBoolean(DynamicAtt.Item3[k]);
                        //            if (BoolValue == true)
                        //            {
                        //                BooleanValues.Add(1);
                        //            }
                        //            else
                        //            {
                        //                BooleanValues.Add(0);
                        //            }
                        //            DynamicAttIds.Add(DynamicAtt.Item1);
                        //            disables.Add(false);
                        //            sideArmLibIds.Add(null);
                        //            sideArmLibraryIds.Add(null);
                        //            InventoryIds.Add(Convert.ToInt32(InsertedIds[k]));
                        //            tablesNamesIds.Add(TableNameEntity.Id);
                        //        }
                        //    }
                        //}
                        ////connection.Open();
                        //OracleParameter value = new OracleParameter();
                        //if (DynamicAtt.Item2 == "string")
                        //{
                        //    value.OracleDbType = OracleDbType.NVarchar2;
                        //    value.Value = StringValues.ToArray();
                        //}
                        //else if (DynamicAtt.Item2 == "double")
                        //{
                        //    value.OracleDbType = OracleDbType.BinaryDouble;
                        //    value.Value = DoubleValues.ToArray();
                        //}
                        //else if (DynamicAtt.Item2 == "datetime")
                        //{
                        //    value.OracleDbType = OracleDbType.Date;
                        //    value.Value = DateTimeValues.ToArray();
                        //}
                        //else if (DynamicAtt.Item2 == "boolean")
                        //{
                        //    value.OracleDbType = OracleDbType.Int32;
                        //    value.Value = BooleanValues.ToArray();
                        //}
                        //OracleParameter DynamicAttId = new OracleParameter();
                        //DynamicAttId.OracleDbType = OracleDbType.Int32;
                        //DynamicAttId.Value = DynamicAttIds.ToArray();

                        //OracleParameter disable = new OracleParameter();
                        //disable.OracleDbType = OracleDbType.Boolean;
                        //disable.Value = disables.ToArray();

                        //OracleParameter sideArmLibId = new OracleParameter();
                        //sideArmLibId.OracleDbType = OracleDbType.Int32;
                        //sideArmLibId.Value = sideArmLibIds.ToArray();

                        //OracleParameter sideArmLibraryId = new OracleParameter();
                        //sideArmLibraryId.OracleDbType = OracleDbType.Int32;
                        //sideArmLibraryId.Value = sideArmLibraryIds.ToArray();

                        //OracleParameter InventoryId = new OracleParameter();
                        //InventoryId.OracleDbType = OracleDbType.Int32;
                        //InventoryId.Value = InventoryIds.ToArray();

                        //OracleParameter tablesNamesId = new OracleParameter();
                        //tablesNamesId.OracleDbType = OracleDbType.Int32;
                        //tablesNamesId.Value = tablesNamesIds.ToArray();

                        //OracleCommand dalvcmd = connection.CreateCommand();
                        //if (DynamicAtt.Item2 == "string")
                        //{
                        //    dalvcmd.CommandText = "INSERT INTO \"TLIdynamicAttLibValue\" (\"DynamicAttId\", \"InventoryId\", \"tablesNamesId\", \"ValueString\") VALUES ( :1, :2, :3, :4)";
                        //    dalvcmd.ArrayBindCount = StringValues.Count;
                        //}
                        //else if (DynamicAtt.Item2 == "double")
                        //{
                        //    dalvcmd.CommandText = "INSERT INTO \"TLIdynamicAttLibValue\" (\"DynamicAttId\", \"InventoryId\", \"tablesNamesId\", \"ValueDouble\") VALUES ( :1, :2, :3, :4)";
                        //    dalvcmd.ArrayBindCount = DoubleValues.Count;
                        //}
                        //else if (DynamicAtt.Item2 == "datetime")
                        //{
                        //    dalvcmd.CommandText = "INSERT INTO \"TLIdynamicAttLibValue\" (\"DynamicAttId\", \"InventoryId\", \"tablesNamesId\", \"ValueDateTime\") VALUES ( :1, :2, :3, :4)";
                        //    dalvcmd.ArrayBindCount = DateTimeValues.Count;
                        //}
                        //else if (DynamicAtt.Item2 == "boolean")
                        //{
                        //    dalvcmd.CommandText = "INSERT INTO \"TLIdynamicAttLibValue\" (\"DynamicAttId\", \"InventoryId\", \"tablesNamesId\", \"ValueBoolean\") VALUES ( :1, :2, :3, :4)";
                        //    dalvcmd.ArrayBindCount = BooleanValues.Count;
                        //}
                        ////dalvcmd.Parameters.Clear();
                        //dalvcmd.Parameters.Add(DynamicAttId);
                        //dalvcmd.Parameters.Add(InventoryId);
                        //dalvcmd.Parameters.Add(tablesNamesId);
                        //dalvcmd.Parameters.Add(value);
                        //dalvcmd.ExecuteNonQuery();
                    }
                    RecordId.AddRange(InsertedIds);
                }
            ERROR:;
            }
            catch (Exception err)
            {

                throw err;
            }
        }

        private void SavecabinetTelecomLibraryUsingOracleBulkCopy(int UserId, out List<int> RecordId, DataTable dt, ref List<KeyValuePair<int, string>> UnsavedRows, int ActColumns, int Columns, ExcelWorksheet sheet, List<KeyValuePair<string, List<DropDownListFilters>>> RelatedTables, TLItablesNames TableNameEntity, OracleConnection connection)
        {
            try
            {
                RecordId = new List<int>();
                List<string> Models = _unitOfWork.CabinetTelecomLibraryRepository.GetSelect(x => x.Model).ToList();

                TLIcabinetTelecomLibrary LastId = _serviceProvider.GetService<ApplicationDbContext>().
                    TLIcabinetTelecomLibrary.OrderByDescending(a => a.Id).FirstOrDefault();

                List<TLIdynamicAtt> CabinetTelecomLibraryDynamicAtts = _unitOfWork.DynamicAttRepository.GetIncludeWhere(x =>
                    x.tablesNamesId == TableNameEntity.Id && !x.disable, x => x.DataType).ToList();

                List<string> models = new List<string>();
                List<float> maxweights = new List<float>();
                List<string> LayoutCodes = new List<string>();
                List<string> Dimension_W_D_H_List = new List<string>();
                List<float> widths = new List<float>();
                List<float> depths = new List<float>();
                List<float> heights = new List<float>();
                List<int> TelecomTypeIds = new List<int>();
                List<float> SpaceLibraries = new List<float>();
                List<Tuple<int, string, List<dynamic>>> DynamicAtts = new List<Tuple<int, string, List<dynamic>>>();
                TLIdynamicAtt DA = new TLIdynamicAtt();
                string ColName = null;
                //List<string> test = new List<string>();
                if (ActColumns > 0)
                {
                    for (int i = ActColumns; i <= Columns; i++)
                    {
                        ColName = sheet.Cells[1, i].Value.ToString();
                        DA = CabinetTelecomLibraryDynamicAtts.FirstOrDefault(x => x.Key == ColName);
                        if (DA != null)
                            DynamicAtts.Add(new Tuple<int, string, List<dynamic>>(DA.Id, DA.DataType.Name.ToLower(), new List<dynamic>()));
                    }
                }
                bool test;
                for (int j = 0; j < dt.Rows.Count; j++)
                {
                    try
                    {
                        string model_test = null;

                        if (dt.Columns.Contains("Model"))
                        {
                            if ((Models.Find(x => x == Convert.ToString(dt.Rows[j]["Model"]))) != null)
                            {
                                UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"Model is already exist in the row {j + 2}"));
                                goto ERROR;
                            }
                            if (!String.IsNullOrEmpty(dt.Rows[j]["Model"].ToString()))
                            {
                                model_test = Convert.ToString(dt.Rows[j]["Model"]);
                            }
                        }

                        float MaxWeight_test = 0;
                        if (dt.Columns.Contains("MaxWeight"))
                        {
                            if (!String.IsNullOrEmpty(dt.Rows[j]["MaxWeight"].ToString()))
                            {
                                float MaxWeight_test_1;
                                test = float.TryParse(dt.Rows[j]["MaxWeight"].ToString(), out MaxWeight_test_1);
                                if (test == true)
                                {
                                    MaxWeight_test = MaxWeight_test_1;
                                }
                                else
                                {
                                    UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"MaxWeight Wrong Input DataType In The Row {j + 2}"));
                                    goto ERROR;
                                }
                            }
                        }

                        string LayoutCode_test = null;
                        if (dt.Columns.Contains("LayoutCode"))
                        {
                            if (!String.IsNullOrEmpty(dt.Rows[j]["LayoutCode"].ToString()))
                            {
                                LayoutCode_test = Convert.ToString(dt.Rows[j]["LayoutCode"]);
                            }
                        }

                        string Dimension_W_D_H_test = null;
                        if (dt.Columns.Contains("Dimension_W_D_H"))
                        {
                            if (!String.IsNullOrEmpty(dt.Rows[j]["Dimension_W_D_H"].ToString()))
                            {
                                Dimension_W_D_H_test = Convert.ToString(dt.Rows[j]["Dimension_W_D_H"]);
                            }
                        }

                        float width_test = 0;
                        if (dt.Columns.Contains("Width"))
                        {

                            test = float.TryParse(dt.Rows[j]["Width"].ToString(), out width_test);
                            if (test == false)
                            {
                                UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"Width Wrong Input DataType In The Row {j + 2}"));
                                goto ERROR;
                            }
                        }
                        float depth_test = 0;
                        if (dt.Columns.Contains("Depth"))
                        {
                            test = float.TryParse(dt.Rows[j]["Depth"].ToString(), out depth_test);
                            if (test == false)
                            {
                                UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"Depth Wrong Input DataType In The Row {j + 2}"));
                                goto ERROR;
                            }
                        }

                        float height_test = 0;
                        if (dt.Columns.Contains("Height"))
                        {
                            test = float.TryParse(dt.Rows[j]["Height"].ToString(), out height_test);
                            if (test == false)
                            {
                                UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"Height Wrong Input DataType In The Row {j + 2}"));
                                goto ERROR;
                            }
                        }

                        int TelecomTypeId_test = 0;
                        if (dt.Columns.Contains("TelecomTypeId"))
                        {
                            if (!String.IsNullOrEmpty(dt.Rows[j]["TelecomTypeId"].ToString()))
                            {
                                var TelecomType = RelatedTables.Where(x => x.Key == "TelecomTypeId").Select(x => x.Value).FirstOrDefault().Where(x => x.Value == (dt.Rows[j]["TelecomTypeId"]).ToString()).FirstOrDefault();
                                TelecomTypeId_test = Convert.ToInt32(TelecomType.Id);
                            }
                            else
                            {
                                UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"TelecomTypeId must to be bigger of zero{j + 2}"));
                                goto ERROR;
                            }
                        }

                        float spacelibrary_test = 0;
                        if (dt.Columns.Contains("SpaceLibrary"))
                        {
                            if (!String.IsNullOrEmpty(dt.Rows[j]["SpaceLibrary"].ToString()))
                            {
                                float spacelibrary_test1;
                                test = float.TryParse(dt.Rows[j]["SpaceLibrary"].ToString(), out spacelibrary_test1);
                                if (test == true)
                                {
                                    spacelibrary_test = spacelibrary_test1;
                                }
                                else if (test == true && spacelibrary_test1 == 0)
                                {
                                    UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"SpaceLibrary must to be bigger of zero{j + 2}"));
                                    goto ERROR;
                                }
                                else
                                {
                                    UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"SpaceLibrary Wrong Input DataType in the row {j + 2}"));
                                    goto ERROR;
                                }
                            }
                            else
                            {
                                UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"SpaceLibrary can not to be null and must to be bigger of zero{j + 2}"));
                                goto ERROR;
                            }
                        }

                        List<dynamic> DynamicAttList = new List<dynamic>();
                        if (ActColumns > 0)
                        {
                            for (int i = ActColumns; i <= Columns; i++)
                            {
                                string StringDynamicAtt = string.Empty;
                                double? DoubleDynamicAtt = null;
                                DateTime? DateTimeDynamicAtt = null;
                                Boolean? BooleanDynamicAtt = null;
                                ColName = sheet.Cells[1, i].Value.ToString();
                                DA = CabinetTelecomLibraryDynamicAtts.FirstOrDefault(x => x.Key == ColName);
                                //Check dynamic attribute if required or not
                                if (DA != null)
                                {
                                    double double_Test = 0;
                                    DateTime datetime_Test = DateTime.Now;
                                    Boolean boolean_Test = false;
                                    if (DA.Required == true)
                                    {
                                        //if somthing wrong then add row to UnsavedRows
                                        if (String.IsNullOrEmpty(dt.Rows[j][ColName].ToString()))
                                        {
                                            UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"Dynamic Attribute ({ColName}) is required"));
                                            goto ERROR;
                                        }
                                        else
                                        {
                                            if (DA.DataType.Name.ToLower() == "string")
                                            {

                                                StringDynamicAtt = Convert.ToString(dt.Rows[j][ColName]);
                                            }
                                            else if (DA.DataType.Name.ToLower() == "double" || DA.DataType.Name.ToLower() == "int")
                                            {
                                                test = double.TryParse(dt.Rows[j][ColName].ToString(), out double_Test);
                                                if (test == false)
                                                {
                                                    UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"{ColName} Dynamic Attribute Wrong Input DataType in the row {j + 2}"));
                                                    goto ERROR;
                                                }
                                                DoubleDynamicAtt = double_Test;
                                            }
                                            else if (DA.DataType.Name.ToLower() == "datetime")
                                            {
                                                test = DateTime.TryParse(dt.Rows[j][ColName].ToString(), out datetime_Test);
                                                if (test == false)
                                                {
                                                    UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"{ColName} Dynamic Attribute Wrong Input DataType in the row {j + 2}"));
                                                    goto ERROR;
                                                }
                                                DateTimeDynamicAtt = datetime_Test;
                                            }
                                            else if (DA.DataType.Name.ToLower() == "bool")
                                            {
                                                test = Boolean.TryParse(dt.Rows[j][ColName].ToString(), out boolean_Test);
                                                if (test == false)
                                                {
                                                    UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"{ColName} Dynamic Attribute Wrong Input DataType in the row {j + 2}"));
                                                    goto ERROR;
                                                }
                                                BooleanDynamicAtt = boolean_Test;
                                            }


                                        }
                                    }
                                    else
                                    {
                                        if (!String.IsNullOrEmpty(dt.Rows[j][ColName].ToString()))
                                        {
                                            if (DA.DataType.Name.ToLower() == "string")
                                            {
                                                StringDynamicAtt = Convert.ToString(dt.Rows[j][ColName]);
                                            }
                                            else if (DA.DataType.Name.ToLower() == "double" || DA.DataType.Name.ToLower() == "int")
                                            {
                                                test = double.TryParse(dt.Rows[j][ColName].ToString(), out double_Test);
                                                if (test == false)
                                                {
                                                    UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"{ColName} Dynamic Attribute Wrong Input DataType in the row {j + 2}"));
                                                    goto ERROR;
                                                }
                                                DoubleDynamicAtt = double_Test;
                                            }
                                            else if (DA.DataType.Name.ToLower() == "datetime")
                                            {
                                                test = DateTime.TryParse(dt.Rows[j][ColName].ToString(), out datetime_Test);
                                                if (test == false)
                                                {
                                                    UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"{ColName} Dynamic Attribute Wrong Input DataType in the row {j + 2}"));
                                                    goto ERROR;
                                                }
                                                DateTimeDynamicAtt = datetime_Test;

                                            }
                                            else if (DA.DataType.Name.ToLower() == "bool")
                                            {
                                                test = Boolean.TryParse(dt.Rows[j][ColName].ToString(), out boolean_Test);
                                                if (test == false)
                                                {
                                                    UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"{ColName} Dynamic Attribute Wrong Input DataType in the row {j + 2}"));
                                                    goto ERROR;
                                                }
                                                BooleanDynamicAtt = boolean_Test;
                                            }

                                        }
                                    }
                                    if (DA.DataType.Name.ToLower() == "string")
                                    {
                                        DynamicAttList.Add(StringDynamicAtt);
                                    }
                                    else if (DA.DataType.Name.ToLower() == "double" || DA.DataType.Name.ToLower() == "int")
                                    {
                                        DynamicAttList.Add(DoubleDynamicAtt);
                                    }
                                    else if (DA.DataType.Name.ToLower() == "datetime")
                                    {
                                        DynamicAttList.Add(DateTimeDynamicAtt);
                                    }
                                    else if (DA.DataType.Name.ToLower() == "bool")
                                    {
                                        DynamicAttList.Add(BooleanDynamicAtt);
                                    }


                                }
                            }
                        }
                        models.Add(model_test != null ? model_test : null);
                        maxweights.Add(MaxWeight_test != 0 ? MaxWeight_test : 0);
                        LayoutCodes.Add(LayoutCode_test != null ? LayoutCode_test : null);
                        Dimension_W_D_H_List.Add(Dimension_W_D_H_test != null ? Dimension_W_D_H_test : null);
                        widths.Add(width_test != 0 ? width_test : 0);
                        depths.Add(depth_test != 0 ? depth_test : 0);
                        heights.Add(height_test != 0 ? height_test : 0);
                        TelecomTypeIds.Add(TelecomTypeId_test != 0 ? TelecomTypeId_test : 0);
                        SpaceLibraries.Add(spacelibrary_test != 0 ? spacelibrary_test : 0);
                        for (int f = 0; f < DynamicAttList.Count; f++)
                        {
                            //var h = DynamicAtts[f].Value;
                            DynamicAtts[f].Item3.Add(DynamicAttList[f]);
                            //h.Add(DynamicAttList[f]);
                        }


                    }
                    catch (Exception err)
                    {

                        // UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, "Wrong Input DataType"));
                    }
                }
                if (models.Count > 0)
                {
                    OracleParameter model = new OracleParameter();
                    model.OracleDbType = OracleDbType.NVarchar2;
                    model.Value = models.ToArray();

                    OracleParameter MaxWeight = new OracleParameter();
                    MaxWeight.OracleDbType = OracleDbType.BinaryFloat;
                    MaxWeight.Value = maxweights.ToArray();

                    OracleParameter LayoutCode = new OracleParameter();
                    LayoutCode.OracleDbType = OracleDbType.NVarchar2;
                    LayoutCode.Value = LayoutCodes.ToArray();

                    OracleParameter Dimension_W_D_H = new OracleParameter();
                    Dimension_W_D_H.OracleDbType = OracleDbType.NVarchar2;
                    Dimension_W_D_H.Value = Dimension_W_D_H_List.ToArray();

                    OracleParameter Width = new OracleParameter();
                    Width.OracleDbType = OracleDbType.BinaryFloat;
                    Width.Value = widths.ToArray();

                    OracleParameter Depth = new OracleParameter();
                    Depth.OracleDbType = OracleDbType.BinaryFloat;
                    Depth.Value = depths.ToArray();

                    OracleParameter Height = new OracleParameter();
                    Height.OracleDbType = OracleDbType.BinaryFloat;
                    Height.Value = heights.ToArray();

                    OracleParameter SpaceLibrary = new OracleParameter();
                    SpaceLibrary.OracleDbType = OracleDbType.BinaryFloat;
                    SpaceLibrary.Value = SpaceLibraries.ToArray();

                    OracleParameter TelecomTypeId = new OracleParameter();
                    TelecomTypeId.OracleDbType = OracleDbType.Int32;
                    TelecomTypeId.Value = TelecomTypeIds.ToArray();

                    // create command and set properties
                    OracleCommand cmd = connection.CreateCommand();
                    cmd.CommandText = "INSERT INTO \"TLIcabinetTelecomLibrary\" (\"Model\", \"MaxWeight\", \"LayoutCode\", \"Dimension_W_D_H\", \"Width\", \"Depth\", \"Height\", \"SpaceLibrary\", \"TelecomTypeId\") VALUES ( :1, :2, :3, :4, 0, 0, 0, :8, :9)";
                    cmd.ArrayBindCount = models.Count;
                    cmd.Parameters.Add(model);
                    cmd.Parameters.Add(MaxWeight);
                    cmd.Parameters.Add(LayoutCode);
                    cmd.Parameters.Add(Dimension_W_D_H);
                    cmd.Parameters.Add(SpaceLibrary);
                    cmd.Parameters.Add(TelecomTypeId);
                    cmd.ExecuteNonQuery();
                    //connection.Close();
                    List<int> InsertedIds = new List<int>();
                    if (LastId != null)
                    {
                        OracleCommand InventoryCmd = connection.CreateCommand();
                        InventoryCmd.CommandType = CommandType.Text;
                        InventoryCmd.CommandText = $"Select \"Id\" From \"TLIcabinetTelecomLibrary\" where \"TLIcabinetTelecomLibrary\".\"Id\" > {LastId.Id}";
                        OracleDataReader Reader = InventoryCmd.ExecuteReader();
                        if (Reader.HasRows)
                        {
                            while (Reader.Read())
                            {
                                InsertedIds.Add(Reader.GetInt32(0));
                            }
                        }
                    }
                    else
                    {
                        OracleCommand InventoryCmd = connection.CreateCommand();
                        InventoryCmd.CommandType = CommandType.Text;
                        InventoryCmd.CommandText = $"Select \"Id\" From \"TLIcabinetTelecomLibrary\"";
                        OracleDataReader Reader = InventoryCmd.ExecuteReader();
                        if (Reader.HasRows)
                        {
                            while (Reader.Read())
                            {
                                InsertedIds.Add(Reader.GetInt32(0));
                            }
                        }
                    }
                    TLIhistory tLIhistory = new TLIhistory()
                    {
                        UserId = UserId,
                        TablesNameId = TableNameEntity.Id,
                        HistoryTypeId = 1,

                    };
                    _dbContext.TLIhistory.Add(tLIhistory);
                    _dbContext.SaveChanges();
                    var cabinetTelecomLibraryist = new List<TLIcabinetTelecomLibrary>();

                    for (int i = 0; i < models.Count; i++)
                    {
                        cabinetTelecomLibraryist.Add(new TLIcabinetTelecomLibrary
                        {
                            Id = InsertedIds[i],
                            Model = models[i],
                            MaxWeight = maxweights[i],
                            LayoutCode = LayoutCodes[i],
                            Dimension_W_D_H = Dimension_W_D_H_List[i],
                            SpaceLibrary = SpaceLibraries[i],
                            TelecomTypeId = TelecomTypeIds[i],

                        });
                    }


                    foreach (int recordId in InsertedIds)
                    {
                        var RecordName = cabinetTelecomLibraryist.FirstOrDefault(x => x.Id == recordId);
                        if (RecordName != null)
                        {
                            var attributeNames = RecordName.GetType().GetProperties()
                                .Where(x =>
                                    (x.PropertyType.IsGenericType && x.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>) &&
                                        new Type[] { typeof(int), typeof(string), typeof(double), typeof(float), typeof(Single), typeof(bool), typeof(DateTime) }
                                        .Contains(x.PropertyType.GetGenericArguments()[0]))
                                    || new Type[] { typeof(int), typeof(string), typeof(double), typeof(bool), typeof(DateTime), typeof(float), typeof(Single) }
                                    .Contains(x.PropertyType)
                                )
                                .ToList();

                            foreach (var propertyInfo in attributeNames)
                            {

                                var propertyName = propertyInfo.Name;

                                var propertyValue = propertyInfo.GetValue(RecordName)?.ToString().Trim();

                                TLIhistoryDet tLIhistoryDet = new TLIhistoryDet()
                                {
                                    HistoryId = tLIhistory.Id,
                                    TablesNameId = TableNameEntity.Id,
                                    RecordId = recordId.ToString(),
                                    AttributeName = propertyName,
                                    NewValue = propertyValue
                                };


                                _dbContext.TLIhistoryDet.Add(tLIhistoryDet);
                            }
                        }
                    }
                    _dbContext.SaveChanges();

                    foreach (var DynamicAtt in DynamicAtts)
                    {
                        for (int k = 0; k < DynamicAtt.Item3.Count; k++)
                        {
                            if (!String.IsNullOrEmpty(DynamicAtt.Item3[k].ToString()))
                            {

                                var Message = _unitOfWork.CivilWithLegsRepository.CheckDynamicValidationAndDependenceDynamic(DynamicAtt.Item1, DynamicAtt.Item3[k], Convert.ToInt32(InsertedIds[k]), tLIhistory.Id, cabinetTelecomLibraryist[0]).Message;
                                if (Message != "Success")
                                {
                                    UnsavedRows.Add(new KeyValuePair<int, string>(k + 2, Message + ' ' + $"in the Colum Name{ColName} in the Row Number {k + 2} "));
                                    goto ERROR;
                                }

                            }

                        }
                        //List<string> StringValues = new List<string>();
                        //List<double> DoubleValues = new List<double>();
                        //List<DateTime> DateTimeValues = new List<DateTime>();
                        //List<Int32> BooleanValues = new List<Int32>();
                        //List<int> DynamicAttIds = new List<int>();
                        //List<bool> disables = new List<bool>();
                        //List<int?> sideArmLibIds = new List<int?>();
                        //List<int?> sideArmLibraryIds = new List<int?>();
                        //List<int> InventoryIds = new List<int>();
                        //List<int> tablesNamesIds = new List<int>();
                        //if (DynamicAtt.Item2 == "string")
                        //{
                        //    for (int k = 0; k < DynamicAtt.Item3.Count; k++)
                        //    {
                        //        if (!String.IsNullOrEmpty(DynamicAtt.Item3[k].ToString()))
                        //        {
                        //            StringValues.Add(DynamicAtt.Item3[k].ToString());
                        //            DynamicAttIds.Add(DynamicAtt.Item1);
                        //            disables.Add(false);
                        //            sideArmLibIds.Add(null);
                        //            sideArmLibraryIds.Add(null);
                        //            InventoryIds.Add(Convert.ToInt32(InsertedIds[k]));
                        //            tablesNamesIds.Add(TableNameEntity.Id);
                        //        }
                        //    }
                        //}
                        //if (DynamicAtt.Item2 == "double")
                        //{
                        //    for (int k = 0; k < DynamicAtt.Item3.Count; k++)
                        //    {
                        //        if (!String.IsNullOrEmpty(DynamicAtt.Item3[k].ToString()))
                        //        {
                        //            DoubleValues.Add(Convert.ToInt32(DynamicAtt.Item3[k]));
                        //            DynamicAttIds.Add(DynamicAtt.Item1);
                        //            disables.Add(false);
                        //            sideArmLibIds.Add(null);
                        //            sideArmLibraryIds.Add(null);
                        //            InventoryIds.Add(Convert.ToInt32(InsertedIds[k]));
                        //            tablesNamesIds.Add(TableNameEntity.Id);
                        //        }
                        //    }
                        //}
                        //if (DynamicAtt.Item2 == "datetime")
                        //{
                        //    for (int k = 0; k < DynamicAtt.Item3.Count; k++)
                        //    {
                        //        if (!String.IsNullOrEmpty(DynamicAtt.Item3[k].ToString()))
                        //        {
                        //            DateTimeValues.Add(Convert.ToDateTime(DynamicAtt.Item3[k]));
                        //            DynamicAttIds.Add(DynamicAtt.Item1);
                        //            disables.Add(false);
                        //            sideArmLibIds.Add(null);
                        //            sideArmLibraryIds.Add(null);
                        //            InventoryIds.Add(Convert.ToInt32(InsertedIds[k]));
                        //            tablesNamesIds.Add(TableNameEntity.Id);
                        //        }
                        //    }
                        //}
                        //if (DynamicAtt.Item2 == "boolean")
                        //{
                        //    for (int k = 0; k < DynamicAtt.Item3.Count; k++)
                        //    {
                        //        if (!String.IsNullOrEmpty(DynamicAtt.Item3[k].ToString()))
                        //        {
                        //            bool BoolValue = Convert.ToBoolean(DynamicAtt.Item3[k]);
                        //            if (BoolValue == true)
                        //            {
                        //                BooleanValues.Add(1);
                        //            }
                        //            else
                        //            {
                        //                BooleanValues.Add(0);
                        //            }
                        //            DynamicAttIds.Add(DynamicAtt.Item1);
                        //            disables.Add(false);
                        //            sideArmLibIds.Add(null);
                        //            sideArmLibraryIds.Add(null);
                        //            InventoryIds.Add(Convert.ToInt32(InsertedIds[k]));
                        //            tablesNamesIds.Add(TableNameEntity.Id);
                        //        }
                        //    }
                        //}
                        ////connection.Open();
                        //OracleParameter value = new OracleParameter();
                        //if (DynamicAtt.Item2 == "string")
                        //{
                        //    value.OracleDbType = OracleDbType.NVarchar2;
                        //    value.Value = StringValues.ToArray();
                        //}
                        //else if (DynamicAtt.Item2 == "double")
                        //{
                        //    value.OracleDbType = OracleDbType.BinaryDouble;
                        //    value.Value = DoubleValues.ToArray();
                        //}
                        //else if (DynamicAtt.Item2 == "datetime")
                        //{
                        //    value.OracleDbType = OracleDbType.Date;
                        //    value.Value = DateTimeValues.ToArray();
                        //}
                        //else if (DynamicAtt.Item2 == "boolean")
                        //{
                        //    value.OracleDbType = OracleDbType.Int32;
                        //    value.Value = BooleanValues.ToArray();
                        //}
                        //OracleParameter DynamicAttId = new OracleParameter();
                        //DynamicAttId.OracleDbType = OracleDbType.Int32;
                        //DynamicAttId.Value = DynamicAttIds.ToArray();

                        //OracleParameter disable = new OracleParameter();
                        //disable.OracleDbType = OracleDbType.Boolean;
                        //disable.Value = disables.ToArray();

                        //OracleParameter sideArmLibId = new OracleParameter();
                        //sideArmLibId.OracleDbType = OracleDbType.Int32;
                        //sideArmLibId.Value = sideArmLibIds.ToArray();

                        //OracleParameter sideArmLibraryId = new OracleParameter();
                        //sideArmLibraryId.OracleDbType = OracleDbType.Int32;
                        //sideArmLibraryId.Value = sideArmLibraryIds.ToArray();

                        //OracleParameter InventoryId = new OracleParameter();
                        //InventoryId.OracleDbType = OracleDbType.Int32;
                        //InventoryId.Value = InventoryIds.ToArray();

                        //OracleParameter tablesNamesId = new OracleParameter();
                        //tablesNamesId.OracleDbType = OracleDbType.Int32;
                        //tablesNamesId.Value = tablesNamesIds.ToArray();

                        //OracleCommand dalvcmd = connection.CreateCommand();
                        //if (DynamicAtt.Item2 == "string")
                        //{
                        //    dalvcmd.CommandText = "INSERT INTO \"TLIdynamicAttLibValue\" (\"DynamicAttId\", \"InventoryId\", \"tablesNamesId\", \"ValueString\") VALUES ( :1, :2, :3, :4)";
                        //    dalvcmd.ArrayBindCount = StringValues.Count;
                        //}
                        //else if (DynamicAtt.Item2 == "double")
                        //{
                        //    dalvcmd.CommandText = "INSERT INTO \"TLIdynamicAttLibValue\" (\"DynamicAttId\", \"InventoryId\", \"tablesNamesId\", \"ValueDouble\") VALUES ( :1, :2, :3, :4)";
                        //    dalvcmd.ArrayBindCount = DoubleValues.Count;
                        //}
                        //else if (DynamicAtt.Item2 == "datetime")
                        //{
                        //    dalvcmd.CommandText = "INSERT INTO \"TLIdynamicAttLibValue\" (\"DynamicAttId\", \"InventoryId\", \"tablesNamesId\", \"ValueDateTime\") VALUES ( :1, :2, :3, :4)";
                        //    dalvcmd.ArrayBindCount = DateTimeValues.Count;
                        //}
                        //else if (DynamicAtt.Item2 == "boolean")
                        //{
                        //    dalvcmd.CommandText = "INSERT INTO \"TLIdynamicAttLibValue\" (\"DynamicAttId\", \"InventoryId\", \"tablesNamesId\", \"ValueBoolean\") VALUES ( :1, :2, :3, :4)";
                        //    dalvcmd.ArrayBindCount = BooleanValues.Count;
                        //}
                        ////dalvcmd.Parameters.Clear();
                        //dalvcmd.Parameters.Add(DynamicAttId);
                        //dalvcmd.Parameters.Add(InventoryId);
                        //dalvcmd.Parameters.Add(tablesNamesId);
                        //dalvcmd.Parameters.Add(value);
                        //dalvcmd.ExecuteNonQuery();
                    }
                    RecordId.AddRange(InsertedIds);
                }
            ERROR:;
            }
            catch (Exception err)
            {

                throw err;
            }
        }

        private void SavegeneratorLibraryUsingOracleBulkCopy(int UserId, out List<int> RecordId, DataTable dt, ref List<KeyValuePair<int, string>> UnsavedRows, int ActColumns, int Columns, ExcelWorksheet sheet, List<KeyValuePair<string, List<DropDownListFilters>>> RelatedTables, TLItablesNames TableNameEntity, OracleConnection connection)
        {
            try
            {
                RecordId = new List<int>();
                List<string> Models = _unitOfWork.GeneratorLibraryRepository.GetSelect(x => x.Model).ToList();

                TLIgeneratorLibrary LastId = _serviceProvider.GetService<ApplicationDbContext>().
                    TLIgeneratorLibrary.OrderByDescending(a => a.Id).FirstOrDefault();

                List<TLIdynamicAtt> GeneratorLibraryDynamicAtts = _unitOfWork.DynamicAttRepository.GetIncludeWhere(x =>
                    x.tablesNamesId == TableNameEntity.Id && !x.disable, x => x.DataType).ToList();

                List<string> models = new List<string>();
                List<float> widths = new List<float>();
                List<float> weights = new List<float>();
                List<float> lengths = new List<float>();
                List<string> LayoutCodes = new List<string>();
                List<float> heights = new List<float>();
                List<float> SpaceLibraries = new List<float>();
                List<string> GeneratorCapaCitys = new List<string>();
                List<Tuple<int, string, List<dynamic>>> DynamicAtts = new List<Tuple<int, string, List<dynamic>>>();
                TLIdynamicAtt DA = new TLIdynamicAtt();
                string ColName = null;
                //List<string> test = new List<string>();
                if (ActColumns > 0)
                {
                    for (int i = ActColumns; i <= Columns; i++)
                    {
                        ColName = sheet.Cells[1, i].Value.ToString();
                        DA = GeneratorLibraryDynamicAtts.FirstOrDefault(x => x.Key == ColName);
                        if (DA != null)
                            DynamicAtts.Add(new Tuple<int, string, List<dynamic>>(DA.Id, DA.DataType.Name.ToLower(), new List<dynamic>()));
                    }
                }
                bool test;
                for (int j = 0; j < dt.Rows.Count; j++)
                {
                    try
                    {
                        string model_test = null;

                        if (dt.Columns.Contains("Model"))
                        {
                            if ((Models.Find(x => x == Convert.ToString(dt.Rows[j]["Model"]))) != null)
                            {
                                UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"Model is already exist in the row {j + 2}"));
                                goto ERROR;
                            }
                            if (!String.IsNullOrEmpty(dt.Rows[j]["Model"].ToString()))
                            {
                                model_test = Convert.ToString(dt.Rows[j]["Model"]);
                            }
                        }

                        float Width_test = 0;
                        if (dt.Columns.Contains("Width"))
                        {
                            test = float.TryParse(dt.Rows[j]["Width"].ToString(), out Width_test);
                            if (test == false)
                            {
                                UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"Width Wrong Input DataType In The Row {j + 2}"));
                                goto ERROR;
                            }
                        }

                        float Weight_test = 0;
                        if (dt.Columns.Contains("Weight"))
                        {
                            if (!String.IsNullOrEmpty(dt.Rows[j]["Weight"].ToString()))
                            {
                                float Weight_test_1;
                                test = float.TryParse(dt.Rows[j]["Weight"].ToString(), out Weight_test_1);
                                if (test == true)
                                {
                                    Weight_test = Weight_test_1;
                                }
                                else
                                {
                                    UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"Weight Wrong Input DataType In The Row {j + 2}"));
                                    goto ERROR;
                                }
                            }
                        }

                        float Length_test = 0;
                        if (dt.Columns.Contains("Length"))
                        {
                            test = float.TryParse(dt.Rows[j]["Length"].ToString(), out Length_test);
                            if (test == false)
                            {
                                UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"Length Wrong Input DataType In The Row {j + 2}"));
                                goto ERROR;
                            }
                        }

                        string LayoutCode_test = null;
                        if (dt.Columns.Contains("LayoutCode"))
                        {
                            if (!String.IsNullOrEmpty(dt.Rows[j]["LayoutCode"].ToString()))
                            {
                                LayoutCode_test = Convert.ToString(dt.Rows[j]["LayoutCode"]);
                            }
                        }

                        string GeneratorCapaCity_test = null;
                        if (dt.Columns.Contains("GeneratorCapaCity"))
                        {
                            if (!String.IsNullOrEmpty(dt.Rows[j]["GeneratorCapaCity"].ToString()))
                            {
                                GeneratorCapaCity_test = Convert.ToString(dt.Rows[j]["GeneratorCapaCity"]);
                            }
                        }

                        float Height_test = 0;
                        if (dt.Columns.Contains("Height"))
                        {
                            test = float.TryParse(dt.Rows[j]["Height"].ToString(), out Height_test);
                            if (test == false)
                            {
                                UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"Height Wrong Input DataType In The Row {j + 2}"));
                                goto ERROR;
                            }
                        }

                        float spacelibrary_test = 0;
                      
                        if (Length_test == 0)
                        {
                            UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"length must to be bigger of zero in the row {j + 2}"));
                            goto ERROR;
                        }
                        if (Width_test == 0)
                        {
                            UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"Width must to be bigger of zero in the row {j + 2}"));
                            goto ERROR;
                        }
                        else
                        {
                            spacelibrary_test = Length_test * Width_test;
                        }

                            
                       



                        List<dynamic> DynamicAttList = new List<dynamic>();
                        if (ActColumns > 0)
                        {
                            for (int i = ActColumns; i <= Columns; i++)
                            {
                                string StringDynamicAtt = string.Empty;
                                double? DoubleDynamicAtt = null;
                                DateTime? DateTimeDynamicAtt = null;
                                Boolean? BooleanDynamicAtt = null;
                                ColName = sheet.Cells[1, i].Value.ToString();
                                DA = GeneratorLibraryDynamicAtts.FirstOrDefault(x => x.Key == ColName);
                                //Check dynamic attribute if required or not
                                if (DA != null)
                                {
                                    double double_Test = 0;
                                    DateTime datetime_Test = DateTime.Now;
                                    Boolean boolean_Test = false;
                                    if (DA.Required == true)
                                    {
                                        //if somthing wrong then add row to UnsavedRows
                                        if (String.IsNullOrEmpty(dt.Rows[j][ColName].ToString()))
                                        {
                                            UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"Dynamic Attribute ({ColName}) is required"));
                                            goto ERROR;
                                        }
                                        else
                                        {
                                            if (DA.DataType.Name.ToLower() == "string")
                                            {

                                                StringDynamicAtt = Convert.ToString(dt.Rows[j][ColName]);
                                            }
                                            else if (DA.DataType.Name.ToLower() == "double" || DA.DataType.Name.ToLower() == "int")
                                            {
                                                test = double.TryParse(dt.Rows[j][ColName].ToString(), out double_Test);
                                                if (test == false)
                                                {
                                                    UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"{ColName} Dynamic Attribute Wrong Input DataType in the row {j + 2}"));
                                                    goto ERROR;
                                                }
                                                DoubleDynamicAtt = double_Test;
                                            }
                                            else if (DA.DataType.Name.ToLower() == "datetime")
                                            {
                                                test = DateTime.TryParse(dt.Rows[j][ColName].ToString(), out datetime_Test);
                                                if (test == false)
                                                {
                                                    UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"{ColName} Dynamic Attribute Wrong Input DataType in the row {j + 2}"));
                                                    goto ERROR;
                                                }
                                                DateTimeDynamicAtt = datetime_Test;
                                            }
                                            else if (DA.DataType.Name.ToLower() == "bool")
                                            {
                                                test = Boolean.TryParse(dt.Rows[j][ColName].ToString(), out boolean_Test);
                                                if (test == false)
                                                {
                                                    UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"{ColName} Dynamic Attribute Wrong Input DataType in the row {j + 2}"));
                                                    goto ERROR;
                                                }
                                                BooleanDynamicAtt = boolean_Test;
                                            }


                                        }
                                    }
                                    else
                                    {
                                        if (!String.IsNullOrEmpty(dt.Rows[j][ColName].ToString()))
                                        {
                                            if (DA.DataType.Name.ToLower() == "string")
                                            {
                                                StringDynamicAtt = Convert.ToString(dt.Rows[j][ColName]);
                                            }
                                            else if (DA.DataType.Name.ToLower() == "double" || DA.DataType.Name.ToLower() == "int")
                                            {
                                                test = double.TryParse(dt.Rows[j][ColName].ToString(), out double_Test);
                                                if (test == false)
                                                {
                                                    UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"{ColName} Dynamic Attribute Wrong Input DataType in the row {j + 2}"));
                                                    goto ERROR;
                                                }
                                                DoubleDynamicAtt = double_Test;
                                            }
                                            else if (DA.DataType.Name.ToLower() == "datetime")
                                            {
                                                test = DateTime.TryParse(dt.Rows[j][ColName].ToString(), out datetime_Test);
                                                if (test == false)
                                                {
                                                    UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"{ColName} Dynamic Attribute Wrong Input DataType in the row {j + 2}"));
                                                    goto ERROR;
                                                }
                                                DateTimeDynamicAtt = datetime_Test;

                                            }
                                            else if (DA.DataType.Name.ToLower() == "bool")
                                            {
                                                test = Boolean.TryParse(dt.Rows[j][ColName].ToString(), out boolean_Test);
                                                if (test == false)
                                                {
                                                    UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"{ColName} Dynamic Attribute Wrong Input DataType in the row {j + 2}"));
                                                    goto ERROR;
                                                }
                                                BooleanDynamicAtt = boolean_Test;
                                            }

                                        }
                                    }
                                    if (DA.DataType.Name.ToLower() == "string")
                                    {
                                        DynamicAttList.Add(StringDynamicAtt);
                                    }
                                    else if (DA.DataType.Name.ToLower() == "double" || DA.DataType.Name.ToLower() == "int")
                                    {
                                        DynamicAttList.Add(DoubleDynamicAtt);
                                    }
                                    else if (DA.DataType.Name.ToLower() == "datetime")
                                    {
                                        DynamicAttList.Add(DateTimeDynamicAtt);
                                    }
                                    else if (DA.DataType.Name.ToLower() == "bool")
                                    {
                                        DynamicAttList.Add(BooleanDynamicAtt);
                                    }


                                }
                            }
                        }
                        models.Add(model_test != null ? model_test : null);
                        widths.Add(Width_test != 0 ? Width_test : 0);
                        weights.Add(Weight_test != 0 ? Weight_test : 0);
                        lengths.Add(Length_test != 0 ? Length_test : 0);
                        LayoutCodes.Add(LayoutCode_test != null ? LayoutCode_test : null);
                        heights.Add(Height_test != 0 ? Height_test : 0);
                        SpaceLibraries.Add(spacelibrary_test != 0 ? spacelibrary_test : 0);
                        GeneratorCapaCitys.Add(GeneratorCapaCity_test != null ? GeneratorCapaCity_test : null);
                        for (int f = 0; f < DynamicAttList.Count; f++)
                        {
                            //var h = DynamicAtts[f].Value;
                            DynamicAtts[f].Item3.Add(DynamicAttList[f]);
                            //h.Add(DynamicAttList[f]);
                        }


                    }
                    catch (Exception err)
                    {

                        //UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, "Wrong Input DataType"));
                    }
                }
                if (models.Count > 0)
                {
                    OracleParameter model = new OracleParameter();
                    model.OracleDbType = OracleDbType.NVarchar2;
                    model.Value = models.ToArray();

                    OracleParameter Width = new OracleParameter();
                    Width.OracleDbType = OracleDbType.BinaryFloat;
                    Width.Value = widths.ToArray();

                    OracleParameter Weight = new OracleParameter();
                    Weight.OracleDbType = OracleDbType.BinaryFloat;
                    Weight.Value = weights.ToArray();

                    OracleParameter Length = new OracleParameter();
                    Length.OracleDbType = OracleDbType.BinaryFloat;
                    Length.Value = lengths.ToArray();

                    OracleParameter LayoutCode = new OracleParameter();
                    LayoutCode.OracleDbType = OracleDbType.NVarchar2;
                    LayoutCode.Value = LayoutCodes.ToArray();

                    OracleParameter Height = new OracleParameter();
                    Height.OracleDbType = OracleDbType.BinaryFloat;
                    Height.Value = heights.ToArray();

                    OracleParameter SpaceLibrary = new OracleParameter();
                    SpaceLibrary.OracleDbType = OracleDbType.BinaryFloat;
                    SpaceLibrary.Value = SpaceLibraries.ToArray();

                    OracleParameter GeneratorCapaCity = new OracleParameter();
                    GeneratorCapaCity.OracleDbType = OracleDbType.Int32;
                    GeneratorCapaCity.Value = GeneratorCapaCitys.ToArray();

                    // create command and set properties
                    OracleCommand cmd = connection.CreateCommand();
                    cmd.CommandText = "INSERT INTO \"TLIgeneratorLibrary\" (\"Model\", \"Width\", \"Weight\", \"Length\", \"LayoutCode\", \"Height\", \"SpaceLibrary\", \"CapacityId\", \"GeneratorCapaCity\") VALUES ( :1, :2, :3, :4, :5, :6, :7, :8 ,:9)";
                    cmd.ArrayBindCount = models.Count;
                    cmd.Parameters.Add(model);
                    cmd.Parameters.Add(Width);
                    cmd.Parameters.Add(Weight);
                    cmd.Parameters.Add(Length);
                    cmd.Parameters.Add(LayoutCode);
                    cmd.Parameters.Add(Height);
                    cmd.Parameters.Add(SpaceLibrary);
                    cmd.Parameters.Add(new OracleParameter(":8", OracleDbType.Int32) { Value = DBNull.Value });
                    cmd.Parameters.Add(GeneratorCapaCity);
                    cmd.ExecuteNonQuery();
                    //connection.Close();
                    List<int> InsertedIds = new List<int>();
                    if (LastId != null)
                    {
                        OracleCommand InventoryCmd = connection.CreateCommand();
                        InventoryCmd.CommandType = CommandType.Text;
                        InventoryCmd.CommandText = $"Select \"Id\" From \"TLIgeneratorLibrary\" where \"TLIgeneratorLibrary\".\"Id\" > {LastId.Id}";
                        OracleDataReader Reader = InventoryCmd.ExecuteReader();
                        if (Reader.HasRows)
                        {
                            while (Reader.Read())
                            {
                                InsertedIds.Add(Reader.GetInt32(0));
                            }
                        }
                    }
                    else
                    {
                        OracleCommand InventoryCmd = connection.CreateCommand();
                        InventoryCmd.CommandType = CommandType.Text;
                        InventoryCmd.CommandText = $"Select \"Id\" From \"TLIgeneratorLibrary\"";
                        OracleDataReader Reader = InventoryCmd.ExecuteReader();
                        if (Reader.HasRows)
                        {
                            while (Reader.Read())
                            {
                                InsertedIds.Add(Reader.GetInt32(0));
                            }
                        }
                    }
                    TLIhistory tLIhistory = new TLIhistory()
                    {
                        UserId = UserId,
                        TablesNameId = TableNameEntity.Id,
                        HistoryTypeId = 1,

                    };
                    _dbContext.TLIhistory.Add(tLIhistory);
                    _dbContext.SaveChanges();
                    var generatorLibraryist = new List<TLIgeneratorLibrary>();

                    for (int i = 0; i < models.Count; i++)
                    {
                        generatorLibraryist.Add(new TLIgeneratorLibrary
                        {
                            Id = InsertedIds[i],
                            Model = models[i],
                            Width = weights[i],
                            Weight = weights[i],
                            Length = lengths[i],
                            LayoutCode = LayoutCodes[i],
                            Height = heights[i],
                            SpaceLibrary = SpaceLibraries[i],
                            GeneratorCapaCity = GeneratorCapaCitys[i],


                        });
                    }


                    foreach (int recordId in InsertedIds)
                    {
                        var RecordName = generatorLibraryist.FirstOrDefault(x => x.Id == recordId);
                        if (RecordName != null)
                        {
                            var attributeNames = RecordName.GetType().GetProperties()
                                .Where(x =>
                                    (x.PropertyType.IsGenericType && x.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>) &&
                                        new Type[] { typeof(int), typeof(string), typeof(double), typeof(float), typeof(Single), typeof(bool), typeof(DateTime) }
                                        .Contains(x.PropertyType.GetGenericArguments()[0]))
                                    || new Type[] { typeof(int), typeof(string), typeof(double), typeof(bool), typeof(DateTime), typeof(float), typeof(Single) }
                                    .Contains(x.PropertyType)
                                )
                                .ToList();

                            foreach (var propertyInfo in attributeNames)
                            {

                                var propertyName = propertyInfo.Name;

                                var propertyValue = propertyInfo.GetValue(RecordName)?.ToString().Trim();

                                TLIhistoryDet tLIhistoryDet = new TLIhistoryDet()
                                {
                                    HistoryId = tLIhistory.Id,
                                    TablesNameId = TableNameEntity.Id,
                                    RecordId = recordId.ToString(),
                                    AttributeName = propertyName,
                                    NewValue = propertyValue
                                };


                                _dbContext.TLIhistoryDet.Add(tLIhistoryDet);
                            }
                        }
                    }
                    _dbContext.SaveChanges();

                    foreach (var DynamicAtt in DynamicAtts)
                    {
                        for (int k = 0; k < DynamicAtt.Item3.Count; k++)
                        {
                            if (!String.IsNullOrEmpty(DynamicAtt.Item3[k].ToString()))
                            {

                                var Message = _unitOfWork.CivilWithLegsRepository.CheckDynamicValidationAndDependenceDynamic(DynamicAtt.Item1, DynamicAtt.Item3[k], Convert.ToInt32(InsertedIds[k]), tLIhistory.Id, generatorLibraryist[0]).Message;
                                if (Message != "Success")
                                {
                                    UnsavedRows.Add(new KeyValuePair<int, string>(k + 2, Message + ' ' + $"in the Colum Name{ColName} in the Row Number {k + 2} "));
                                    goto ERROR;
                                }

                            }

                        }
                        //List<string> StringValues = new List<string>();
                        //List<double> DoubleValues = new List<double>();
                        //List<DateTime> DateTimeValues = new List<DateTime>();
                        //List<Int32> BooleanValues = new List<Int32>();
                        //List<int> DynamicAttIds = new List<int>();
                        //List<bool> disables = new List<bool>();
                        //List<int?> sideArmLibIds = new List<int?>();
                        //List<int?> sideArmLibraryIds = new List<int?>();
                        //List<int> InventoryIds = new List<int>();
                        //List<int> tablesNamesIds = new List<int>();
                        //if (DynamicAtt.Item2 == "string")
                        //{
                        //    for (int k = 0; k < DynamicAtt.Item3.Count; k++)
                        //    {
                        //        if (!String.IsNullOrEmpty(DynamicAtt.Item3[k].ToString()))
                        //        {
                        //            StringValues.Add(DynamicAtt.Item3[k].ToString());
                        //            DynamicAttIds.Add(DynamicAtt.Item1);
                        //            disables.Add(false);
                        //            sideArmLibIds.Add(null);
                        //            sideArmLibraryIds.Add(null);
                        //            InventoryIds.Add(Convert.ToInt32(InsertedIds[k]));
                        //            tablesNamesIds.Add(TableNameEntity.Id);
                        //        }
                        //    }
                        //}
                        //if (DynamicAtt.Item2 == "double")
                        //{
                        //    for (int k = 0; k < DynamicAtt.Item3.Count; k++)
                        //    {
                        //        if (!String.IsNullOrEmpty(DynamicAtt.Item3[k].ToString()))
                        //        {
                        //            DoubleValues.Add(Convert.ToInt32(DynamicAtt.Item3[k]));
                        //            DynamicAttIds.Add(DynamicAtt.Item1);
                        //            disables.Add(false);
                        //            sideArmLibIds.Add(null);
                        //            sideArmLibraryIds.Add(null);
                        //            InventoryIds.Add(Convert.ToInt32(InsertedIds[k]));
                        //            tablesNamesIds.Add(TableNameEntity.Id);
                        //        }
                        //    }
                        //}
                        //if (DynamicAtt.Item2 == "datetime")
                        //{
                        //    for (int k = 0; k < DynamicAtt.Item3.Count; k++)
                        //    {
                        //        if (!String.IsNullOrEmpty(DynamicAtt.Item3[k].ToString()))
                        //        {
                        //            DateTimeValues.Add(Convert.ToDateTime(DynamicAtt.Item3[k]));
                        //            DynamicAttIds.Add(DynamicAtt.Item1);
                        //            disables.Add(false);
                        //            sideArmLibIds.Add(null);
                        //            sideArmLibraryIds.Add(null);
                        //            InventoryIds.Add(Convert.ToInt32(InsertedIds[k]));
                        //            tablesNamesIds.Add(TableNameEntity.Id);
                        //        }
                        //    }
                        //}
                        //if (DynamicAtt.Item2 == "boolean")
                        //{
                        //    for (int k = 0; k < DynamicAtt.Item3.Count; k++)
                        //    {
                        //        if (!String.IsNullOrEmpty(DynamicAtt.Item3[k].ToString()))
                        //        {
                        //            bool BoolValue = Convert.ToBoolean(DynamicAtt.Item3[k]);
                        //            if (BoolValue == true)
                        //            {
                        //                BooleanValues.Add(1);
                        //            }
                        //            else
                        //            {
                        //                BooleanValues.Add(0);
                        //            }
                        //            DynamicAttIds.Add(DynamicAtt.Item1);
                        //            disables.Add(false);
                        //            sideArmLibIds.Add(null);
                        //            sideArmLibraryIds.Add(null);
                        //            InventoryIds.Add(Convert.ToInt32(InsertedIds[k]));
                        //            tablesNamesIds.Add(TableNameEntity.Id);
                        //        }
                        //    }
                        //}
                        ////connection.Open();
                        //OracleParameter value = new OracleParameter();
                        //if (DynamicAtt.Item2 == "string")
                        //{
                        //    value.OracleDbType = OracleDbType.NVarchar2;
                        //    value.Value = StringValues.ToArray();
                        //}
                        //else if (DynamicAtt.Item2 == "double")
                        //{
                        //    value.OracleDbType = OracleDbType.BinaryDouble;
                        //    value.Value = DoubleValues.ToArray();
                        //}
                        //else if (DynamicAtt.Item2 == "datetime")
                        //{
                        //    value.OracleDbType = OracleDbType.Date;
                        //    value.Value = DateTimeValues.ToArray();
                        //}
                        //else if (DynamicAtt.Item2 == "boolean")
                        //{
                        //    value.OracleDbType = OracleDbType.Int32;
                        //    value.Value = BooleanValues.ToArray();
                        //}
                        //OracleParameter DynamicAttId = new OracleParameter();
                        //DynamicAttId.OracleDbType = OracleDbType.Int32;
                        //DynamicAttId.Value = DynamicAttIds.ToArray();

                        //OracleParameter disable = new OracleParameter();
                        //disable.OracleDbType = OracleDbType.Boolean;
                        //disable.Value = disables.ToArray();

                        //OracleParameter sideArmLibId = new OracleParameter();
                        //sideArmLibId.OracleDbType = OracleDbType.Int32;
                        //sideArmLibId.Value = sideArmLibIds.ToArray();

                        //OracleParameter sideArmLibraryId = new OracleParameter();
                        //sideArmLibraryId.OracleDbType = OracleDbType.Int32;
                        //sideArmLibraryId.Value = sideArmLibraryIds.ToArray();

                        //OracleParameter InventoryId = new OracleParameter();
                        //InventoryId.OracleDbType = OracleDbType.Int32;
                        //InventoryId.Value = InventoryIds.ToArray();

                        //OracleParameter tablesNamesId = new OracleParameter();
                        //tablesNamesId.OracleDbType = OracleDbType.Int32;
                        //tablesNamesId.Value = tablesNamesIds.ToArray();

                        //OracleCommand dalvcmd = connection.CreateCommand();
                        //if (DynamicAtt.Item2 == "string")
                        //{
                        //    dalvcmd.CommandText = "INSERT INTO \"TLIdynamicAttLibValue\" (\"DynamicAttId\", \"InventoryId\", \"tablesNamesId\", \"ValueString\") VALUES ( :1, :2, :3, :4)";
                        //    dalvcmd.ArrayBindCount = StringValues.Count;
                        //}
                        //else if (DynamicAtt.Item2 == "double")
                        //{
                        //    dalvcmd.CommandText = "INSERT INTO \"TLIdynamicAttLibValue\" (\"DynamicAttId\", \"InventoryId\", \"tablesNamesId\", \"ValueDouble\") VALUES ( :1, :2, :3, :4)";
                        //    dalvcmd.ArrayBindCount = DoubleValues.Count;
                        //}
                        //else if (DynamicAtt.Item2 == "datetime")
                        //{
                        //    dalvcmd.CommandText = "INSERT INTO \"TLIdynamicAttLibValue\" (\"DynamicAttId\", \"InventoryId\", \"tablesNamesId\", \"ValueDateTime\") VALUES ( :1, :2, :3, :4)";
                        //    dalvcmd.ArrayBindCount = DateTimeValues.Count;
                        //}
                        //else if (DynamicAtt.Item2 == "boolean")
                        //{
                        //    dalvcmd.CommandText = "INSERT INTO \"TLIdynamicAttLibValue\" (\"DynamicAttId\", \"InventoryId\", \"tablesNamesId\", \"ValueBoolean\") VALUES ( :1, :2, :3, :4)";
                        //    dalvcmd.ArrayBindCount = BooleanValues.Count;
                        //}
                        ////dalvcmd.Parameters.Clear();
                        //dalvcmd.Parameters.Add(DynamicAttId);
                        //dalvcmd.Parameters.Add(InventoryId);
                        //dalvcmd.Parameters.Add(tablesNamesId);
                        //dalvcmd.Parameters.Add(value);
                        //dalvcmd.ExecuteNonQuery();
                    }
                    RecordId.AddRange(InsertedIds);
                }
            ERROR:;
            }
            catch (Exception err)
            {

                throw err;
            }
        }

        private void SavesolarLibraryUsingOracleBulkCopy(int UserId, out List<int> RecordId, DataTable dt, ref List<KeyValuePair<int, string>> UnsavedRows, int ActColumns, int Columns, ExcelWorksheet sheet, List<KeyValuePair<string, List<DropDownListFilters>>> RelatedTables, TLItablesNames TableNameEntity, OracleConnection connection)
        {
            try
            {
                RecordId = new List<int>();
                List<string> Models = _unitOfWork.SolarLibraryRepository.GetSelect(x => x.Model).ToList();

                TLIsolarLibrary LastId = _serviceProvider.GetService<ApplicationDbContext>().
                    TLIsolarLibrary.OrderByDescending(a => a.Id).FirstOrDefault();

                List<TLIdynamicAtt> SolarLibraryRepositoryDynamicAtts = _unitOfWork.DynamicAttRepository.GetIncludeWhere(x =>
                    x.tablesNamesId == TableNameEntity.Id && !x.disable, x => x.DataType).ToList();

                List<string> models = new List<string>();
                List<float> weights = new List<float>();
                List<float> Lengths = new List<float>();
                List<float> Widths = new List<float>();
                List<string> TotaPanelsDimensions_List = new List<string>();
                List<string> StructureDesigns = new List<string>();
                List<string> LayoutCodes = new List<string>();
                List<float> HeightFromFront_List = new List<float>();
                List<float> HeightFromBack_List = new List<float>();
                List<string> BasePlateDimensions = new List<string>();
                List<float> SpaceLibraries = new List<float>();
                List<Tuple<int, string, List<dynamic>>> DynamicAtts = new List<Tuple<int, string, List<dynamic>>>();
                TLIdynamicAtt DA = new TLIdynamicAtt();
                string ColName = null;
                //List<string> test = new List<string>();
                if (ActColumns > 0)
                {
                    for (int i = ActColumns; i <= Columns; i++)
                    {
                        ColName = sheet.Cells[1, i].Value.ToString();
                        DA = SolarLibraryRepositoryDynamicAtts.FirstOrDefault(x => x.Key == ColName);
                        if (DA != null)
                            DynamicAtts.Add(new Tuple<int, string, List<dynamic>>(DA.Id, DA.DataType.Name.ToLower(), new List<dynamic>()));
                    }
                }
                bool test;
                for (int j = 0; j < dt.Rows.Count; j++)
                {
                    try
                    {
                        string model_test = null;
                        if (dt.Columns.Contains("Model"))
                        {
                            if ((Models.Find(x => x == Convert.ToString(dt.Rows[j]["Model"]))) != null)
                            {
                                UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"Model is already exist in the row {j + 2}"));
                                goto ERROR;
                            }
                            if (!String.IsNullOrEmpty(dt.Rows[j]["Model"].ToString()))
                            {
                                model_test = Convert.ToString(dt.Rows[j]["Model"]);
                            }
                        }

                        float Weight_test = 0;
                        if (dt.Columns.Contains("Weight"))
                        {
                            if (!String.IsNullOrEmpty(dt.Rows[j]["Weight"].ToString()))
                            {
                                float Weight_test_1;
                                test = float.TryParse(dt.Rows[j]["Weight"].ToString(), out Weight_test_1);
                                if (test == true)
                                {
                                    Weight_test = Weight_test_1;
                                }
                                else
                                {
                                    UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"Weight Wrong Input DataType In The Row {j + 2}"));
                                    goto ERROR;
                                }
                            }
                        }
                        float Width_test = 0;
                        if (dt.Columns.Contains("Width"))
                        {
                            if (!String.IsNullOrEmpty(dt.Rows[j]["Width"].ToString()))
                            {
                                float Width_test_test_1;
                                test = float.TryParse(dt.Rows[j]["Width"].ToString(), out Width_test_test_1);
                                if (test == true)
                                {
                                    Width_test = Width_test_test_1;
                                }
                                else
                                {
                                    UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"Width Wrong Input DataType In The Row {j + 2}"));
                                    goto ERROR;
                                }
                            }
                        }
                        float Length_test = 0;
                        if (dt.Columns.Contains("Length"))
                        {
                            if (!String.IsNullOrEmpty(dt.Rows[j]["Length"].ToString()))
                            {
                                float Length_test_1;
                                test = float.TryParse(dt.Rows[j]["Length"].ToString(), out Length_test_1);
                                if (test == true)
                                {
                                    Length_test = Length_test_1;
                                }
                                else
                                {
                                    UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"Length Wrong Input DataType In The Row {j + 2}"));
                                    goto ERROR;
                                }
                            }
                        }

                        string TotaPanelsDimensions_test = null;
                        if (dt.Columns.Contains("TotaPanelsDimensions"))
                        {
                            if (!String.IsNullOrEmpty(dt.Rows[j]["TotaPanelsDimensions"].ToString()))
                            {
                                TotaPanelsDimensions_test = Convert.ToString(dt.Rows[j]["TotaPanelsDimensions"]);
                            }
                        }

                        string StructureDesign_test = null;
                        if (dt.Columns.Contains("StructureDesign"))
                        {
                            if (!String.IsNullOrEmpty(dt.Rows[j]["StructureDesign"].ToString()))
                            {
                                StructureDesign_test = Convert.ToString(dt.Rows[j]["StructureDesign"]);
                            }
                        }

                        string LayoutCode_test = null;
                        if (dt.Columns.Contains("LayoutCode"))
                        {
                            if (!String.IsNullOrEmpty(dt.Rows[j]["LayoutCode"].ToString()))
                            {
                                LayoutCode_test = Convert.ToString(dt.Rows[j]["LayoutCode"]);
                            }
                        }

                        float HeightFromFront_test = 0;
                        if (dt.Columns.Contains("HeightFromFront"))
                        {
                            if (!String.IsNullOrEmpty(dt.Rows[j]["HeightFromFront"].ToString()))
                            {
                                float HeightFromFront_test_1;
                                test = float.TryParse(dt.Rows[j]["HeightFromFront"].ToString(), out HeightFromFront_test_1);
                                if (test == true)
                                {
                                    HeightFromFront_test = HeightFromFront_test_1;
                                }
                                else
                                {
                                    UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"HeightFromFront Wrong Input DataType In The Row {j + 2}"));
                                    goto ERROR;
                                }
                            }

                        }

                        float HeightFromBack_test = 0;
                        if (dt.Columns.Contains("HeightFromBack"))
                        {
                            if (!String.IsNullOrEmpty(dt.Rows[j]["HeightFromBack"].ToString()))
                            {
                                float HeightFromBack_test_1;
                                test = float.TryParse(dt.Rows[j]["HeightFromBack"].ToString(), out HeightFromBack_test_1);
                                if (test == true)
                                {
                                    HeightFromBack_test = HeightFromBack_test_1;
                                }
                                else
                                {
                                    UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"HeightFromBack Wrong Input DataType In The Row {j + 2}"));
                                    goto ERROR;
                                }
                            }
                        }

                        string BasePlateDimension_test = null;
                        if (dt.Columns.Contains("BasePlateDimension"))
                        {
                            if (!String.IsNullOrEmpty(dt.Rows[j]["BasePlateDimension"].ToString()))
                            {
                                BasePlateDimension_test = Convert.ToString(dt.Rows[j]["BasePlateDimension"]);
                            }
                        }


                        float spacelibrary_test = 0;
                        
                        if (Length_test == 0)
                        {
                            UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"length must to be bigger of zero in the row {j + 2}"));
                            goto ERROR;
                        }
                        if (Width_test == 0)
                        {
                            UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"Width must to be bigger of zero in the row {j + 2}"));
                            goto ERROR;
                        }
                        else
                        {
                            spacelibrary_test = Length_test * Width_test;
                        }

                            
                        

                        List<dynamic> DynamicAttList = new List<dynamic>();
                        if (ActColumns > 0)
                        {
                            for (int i = ActColumns; i <= Columns; i++)
                            {
                                string StringDynamicAtt = string.Empty;
                                double? DoubleDynamicAtt = null;
                                DateTime? DateTimeDynamicAtt = null;
                                Boolean? BooleanDynamicAtt = null;
                                ColName = sheet.Cells[1, i].Value.ToString();
                                DA = SolarLibraryRepositoryDynamicAtts.FirstOrDefault(x => x.Key == ColName);
                                //Check dynamic attribute if required or not
                                if (DA != null)
                                {
                                    double double_Test = 0;
                                    DateTime datetime_Test = DateTime.Now;
                                    Boolean boolean_Test = false;
                                    if (DA.Required == true)
                                    {
                                        //if somthing wrong then add row to UnsavedRows
                                        if (String.IsNullOrEmpty(dt.Rows[j][ColName].ToString()))
                                        {
                                            UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"Dynamic Attribute ({ColName}) is required"));
                                            goto ERROR;
                                        }
                                        else
                                        {
                                            if (DA.DataType.Name.ToLower() == "string")
                                            {

                                                StringDynamicAtt = Convert.ToString(dt.Rows[j][ColName]);
                                            }
                                            else if (DA.DataType.Name.ToLower() == "double" || DA.DataType.Name.ToLower() == "int")
                                            {
                                                test = double.TryParse(dt.Rows[j][ColName].ToString(), out double_Test);
                                                if (test == false)
                                                {
                                                    UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"{ColName} Dynamic Attribute Wrong Input DataType in the row {j + 2}"));
                                                    goto ERROR;
                                                }
                                                DoubleDynamicAtt = double_Test;
                                            }
                                            else if (DA.DataType.Name.ToLower() == "datetime")
                                            {
                                                test = DateTime.TryParse(dt.Rows[j][ColName].ToString(), out datetime_Test);
                                                if (test == false)
                                                {
                                                    UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"{ColName} Dynamic Attribute Wrong Input DataType in the row {j + 2}"));
                                                    goto ERROR;
                                                }
                                                DateTimeDynamicAtt = datetime_Test;
                                            }
                                            else if (DA.DataType.Name.ToLower() == "bool")
                                            {
                                                test = Boolean.TryParse(dt.Rows[j][ColName].ToString(), out boolean_Test);
                                                if (test == false)
                                                {
                                                    UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"{ColName} Dynamic Attribute Wrong Input DataType in the row {j + 2}"));
                                                    goto ERROR;
                                                }
                                                BooleanDynamicAtt = boolean_Test;
                                            }


                                        }
                                    }
                                    else
                                    {
                                        if (!String.IsNullOrEmpty(dt.Rows[j][ColName].ToString()))
                                        {
                                            if (DA.DataType.Name.ToLower() == "string")
                                            {
                                                StringDynamicAtt = Convert.ToString(dt.Rows[j][ColName]);
                                            }
                                            else if (DA.DataType.Name.ToLower() == "double" || DA.DataType.Name.ToLower() == "int")
                                            {
                                                test = double.TryParse(dt.Rows[j][ColName].ToString(), out double_Test);
                                                if (test == false)
                                                {
                                                    UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"{ColName} Dynamic Attribute Wrong Input DataType in the row {j + 2}"));
                                                    goto ERROR;
                                                }
                                                DoubleDynamicAtt = double_Test;
                                            }
                                            else if (DA.DataType.Name.ToLower() == "datetime")
                                            {
                                                test = DateTime.TryParse(dt.Rows[j][ColName].ToString(), out datetime_Test);
                                                if (test == false)
                                                {
                                                    UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"{ColName} Dynamic Attribute Wrong Input DataType in the row {j + 2}"));
                                                    goto ERROR;
                                                }
                                                DateTimeDynamicAtt = datetime_Test;

                                            }
                                            else if (DA.DataType.Name.ToLower() == "bool")
                                            {
                                                test = Boolean.TryParse(dt.Rows[j][ColName].ToString(), out boolean_Test);
                                                if (test == false)
                                                {
                                                    UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"{ColName} Dynamic Attribute Wrong Input DataType in the row {j + 2}"));
                                                    goto ERROR;
                                                }
                                                BooleanDynamicAtt = boolean_Test;
                                            }

                                        }
                                    }
                                    if (DA.DataType.Name.ToLower() == "string")
                                    {
                                        DynamicAttList.Add(StringDynamicAtt);
                                    }
                                    else if (DA.DataType.Name.ToLower() == "double" || DA.DataType.Name.ToLower() == "int")
                                    {
                                        DynamicAttList.Add(DoubleDynamicAtt);
                                    }
                                    else if (DA.DataType.Name.ToLower() == "datetime")
                                    {
                                        DynamicAttList.Add(DateTimeDynamicAtt);
                                    }
                                    else if (DA.DataType.Name.ToLower() == "bool")
                                    {
                                        DynamicAttList.Add(BooleanDynamicAtt);
                                    }


                                }
                            }
                        }
                        models.Add(model_test != null ? model_test : "NA");
                        weights.Add(Weight_test != 0 ? Weight_test : 0);
                        TotaPanelsDimensions_List.Add(TotaPanelsDimensions_test != null ? TotaPanelsDimensions_test : "NA");
                        StructureDesigns.Add(StructureDesign_test != null ? StructureDesign_test : "NA");
                        LayoutCodes.Add(LayoutCode_test != null ? LayoutCode_test : "NA");
                        HeightFromFront_List.Add(HeightFromFront_test != 0 ? HeightFromFront_test : 0);
                        HeightFromBack_List.Add(HeightFromBack_test != 0 ? HeightFromBack_test : 0);
                        BasePlateDimensions.Add(BasePlateDimension_test != null ? BasePlateDimension_test : "NA");
                        SpaceLibraries.Add(spacelibrary_test != 0 ? spacelibrary_test : 0);
                        Lengths.Add(Length_test != 0 ? Length_test : 0);
                        Widths.Add(Width_test != 0 ? Width_test : 0);
                        for (int f = 0; f < DynamicAttList.Count; f++)
                        {
                            //var h = DynamicAtts[f].Value;
                            DynamicAtts[f].Item3.Add(DynamicAttList[f]);
                            //h.Add(DynamicAttList[f]);
                        }


                    }
                    catch (Exception err)
                    {

                        // UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, "Wrong Input DataType"));
                    }
                }
                if (models.Count > 0)
                {
                    OracleParameter model = new OracleParameter();
                    model.OracleDbType = OracleDbType.NVarchar2;
                    model.Value = models.ToArray();

                    OracleParameter Weight = new OracleParameter();
                    Weight.OracleDbType = OracleDbType.BinaryFloat;
                    Weight.Value = weights.ToArray();

                    OracleParameter TotaPanelsDimensions = new OracleParameter();
                    TotaPanelsDimensions.OracleDbType = OracleDbType.NVarchar2;
                    TotaPanelsDimensions.Value = TotaPanelsDimensions_List.ToArray();

                    OracleParameter StructureDesign = new OracleParameter();
                    StructureDesign.OracleDbType = OracleDbType.NVarchar2;
                    StructureDesign.Value = StructureDesigns.ToArray();

                    OracleParameter LayoutCode = new OracleParameter();
                    LayoutCode.OracleDbType = OracleDbType.NVarchar2;
                    LayoutCode.Value = LayoutCodes.ToArray();

                    OracleParameter HeightFromFront = new OracleParameter();
                    HeightFromFront.OracleDbType = OracleDbType.BinaryFloat;
                    HeightFromFront.Value = HeightFromFront_List.ToArray();

                    OracleParameter HeightFromBack = new OracleParameter();
                    HeightFromBack.OracleDbType = OracleDbType.BinaryFloat;
                    HeightFromBack.Value = HeightFromBack_List.ToArray();

                    OracleParameter BasePlateDimension = new OracleParameter();
                    BasePlateDimension.OracleDbType = OracleDbType.NVarchar2;
                    BasePlateDimension.Value = BasePlateDimensions.ToArray();

                    OracleParameter SpaceLibrary = new OracleParameter();
                    SpaceLibrary.OracleDbType = OracleDbType.BinaryFloat;
                    SpaceLibrary.Value = SpaceLibraries.ToArray();

                    OracleParameter Length = new OracleParameter();
                    Length.OracleDbType = OracleDbType.Int32;
                    Length.Value = Lengths.ToArray();

                    OracleParameter Width = new OracleParameter();
                    Width.OracleDbType = OracleDbType.Int32;
                    Width.Value = Widths.ToArray();

                    // create command and set properties
                    OracleCommand cmd = connection.CreateCommand();
                    cmd.CommandText = "INSERT INTO \"TLIsolarLibrary\" (\"Model\", \"Weight\", \"TotaPanelsDimensions\", \"StructureDesign\", \"LayoutCode\", \"HeightFromFront\", \"HeightFromBack\", \"BasePlateDimension\", \"SpaceLibrary\", \"CapacityId\", \"Length\", \"Width\") VALUES ( :1, :2, :3, :4, :5, :6, :7, :8, :9, :10 ,:11 ,:12)";
                    cmd.ArrayBindCount = models.Count;
                    cmd.Parameters.Add(model);
                    cmd.Parameters.Add(Weight);
                    cmd.Parameters.Add(TotaPanelsDimensions);
                    cmd.Parameters.Add(StructureDesign);
                    cmd.Parameters.Add(LayoutCode);
                    cmd.Parameters.Add(HeightFromFront);
                    cmd.Parameters.Add(HeightFromBack);
                    cmd.Parameters.Add(BasePlateDimension);
                    cmd.Parameters.Add(SpaceLibrary);
                    cmd.Parameters.Add(new OracleParameter(":10", OracleDbType.Int32) { Value = DBNull.Value });
                    cmd.Parameters.Add(Width);
                    cmd.Parameters.Add(Length);

                    cmd.ExecuteNonQuery();
                    //connection.Close();
                    List<int> InsertedIds = new List<int>();
                    if (LastId != null)
                    {
                        OracleCommand InventoryCmd = connection.CreateCommand();
                        InventoryCmd.CommandType = CommandType.Text;
                        InventoryCmd.CommandText = $"Select \"Id\" From \"TLIsolarLibrary\" where \"TLIsolarLibrary\".\"Id\" > {LastId.Id}";
                        OracleDataReader Reader = InventoryCmd.ExecuteReader();
                        if (Reader.HasRows)
                        {
                            while (Reader.Read())
                            {
                                InsertedIds.Add(Reader.GetInt32(0));
                            }
                        }
                    }
                    else
                    {
                        OracleCommand InventoryCmd = connection.CreateCommand();
                        InventoryCmd.CommandType = CommandType.Text;
                        InventoryCmd.CommandText = $"Select \"Id\" From \"TLIsolarLibrary\"";
                        OracleDataReader Reader = InventoryCmd.ExecuteReader();
                        if (Reader.HasRows)
                        {
                            while (Reader.Read())
                            {
                                InsertedIds.Add(Reader.GetInt32(0));
                            }
                        }
                    }
                    TLIhistory tLIhistory = new TLIhistory()
                    {
                        UserId = UserId,
                        TablesNameId = TableNameEntity.Id,
                        HistoryTypeId = 1,

                    };
                    _dbContext.TLIhistory.Add(tLIhistory);
                    _dbContext.SaveChanges();
                    var solarLibraryist = new List<TLIsolarLibrary>();

                    for (int i = 0; i < models.Count; i++)
                    {
                        solarLibraryist.Add(new TLIsolarLibrary
                        {
                            Id = InsertedIds[i],
                            Model = models[i],
                            Weight = weights[i],
                            Length = Lengths[i],
                            Width = Widths[i],
                            TotaPanelsDimensions = TotaPanelsDimensions_List[i],
                            StructureDesign = StructureDesigns[i],
                            LayoutCode = LayoutCodes[i],
                            HeightFromFront = HeightFromFront_List[i],
                            HeightFromBack = HeightFromBack_List[i],
                            BasePlateDimension = BasePlateDimensions[i],
                            SpaceLibrary = SpaceLibraries[i],

                        });
                    }


                    foreach (int recordId in InsertedIds)
                    {
                        var RecordName = solarLibraryist.FirstOrDefault(x => x.Id == recordId);
                        if (RecordName != null)
                        {
                            var attributeNames = RecordName.GetType().GetProperties()
                                .Where(x =>
                                    (x.PropertyType.IsGenericType && x.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>) &&
                                        new Type[] { typeof(int), typeof(string), typeof(double), typeof(float), typeof(Single), typeof(bool), typeof(DateTime) }
                                        .Contains(x.PropertyType.GetGenericArguments()[0]))
                                    || new Type[] { typeof(int), typeof(string), typeof(double), typeof(bool), typeof(DateTime), typeof(float), typeof(Single) }
                                    .Contains(x.PropertyType)
                                )
                                .ToList();

                            foreach (var propertyInfo in attributeNames)
                            {

                                var propertyName = propertyInfo.Name;

                                var propertyValue = propertyInfo.GetValue(RecordName)?.ToString().Trim();

                                TLIhistoryDet tLIhistoryDet = new TLIhistoryDet()
                                {
                                    HistoryId = tLIhistory.Id,
                                    TablesNameId = TableNameEntity.Id,
                                    RecordId = recordId.ToString(),
                                    AttributeName = propertyName,
                                    NewValue = propertyValue
                                };


                                _dbContext.TLIhistoryDet.Add(tLIhistoryDet);
                            }
                        }
                    }
                    _dbContext.SaveChanges();

                    foreach (var DynamicAtt in DynamicAtts)
                    {
                        for (int k = 0; k < DynamicAtt.Item3.Count; k++)
                        {
                            if (!String.IsNullOrEmpty(DynamicAtt.Item3[k].ToString()))
                            {

                                var Message = _unitOfWork.CivilWithLegsRepository.CheckDynamicValidationAndDependenceDynamic(DynamicAtt.Item1, DynamicAtt.Item3[k], Convert.ToInt32(InsertedIds[k]), tLIhistory.Id, solarLibraryist[0]).Message;
                                if (Message != "Success")
                                {
                                    UnsavedRows.Add(new KeyValuePair<int, string>(k + 2, Message + ' ' + $"in the Colum Name{ColName} in the Row Number {k + 2} "));
                                    goto ERROR;
                                }

                            }

                        }
                        //List<string> StringValues = new List<string>();
                        //List<double> DoubleValues = new List<double>();
                        //List<DateTime> DateTimeValues = new List<DateTime>();
                        //List<Int32> BooleanValues = new List<Int32>();
                        //List<int> DynamicAttIds = new List<int>();
                        //List<bool> disables = new List<bool>();
                        //List<int?> sideArmLibIds = new List<int?>();
                        //List<int?> sideArmLibraryIds = new List<int?>();
                        //List<int> InventoryIds = new List<int>();
                        //List<int> tablesNamesIds = new List<int>();
                        //if (DynamicAtt.Item2 == "string")
                        //{
                        //    for (int k = 0; k < DynamicAtt.Item3.Count; k++)
                        //    {
                        //        if (!String.IsNullOrEmpty(DynamicAtt.Item3[k].ToString()))
                        //        {
                        //            StringValues.Add(DynamicAtt.Item3[k].ToString());
                        //            DynamicAttIds.Add(DynamicAtt.Item1);
                        //            disables.Add(false);
                        //            sideArmLibIds.Add(null);
                        //            sideArmLibraryIds.Add(null);
                        //            InventoryIds.Add(Convert.ToInt32(InsertedIds[k]));
                        //            tablesNamesIds.Add(TableNameEntity.Id);
                        //        }
                        //    }
                        //}
                        //if (DynamicAtt.Item2 == "double")
                        //{
                        //    for (int k = 0; k < DynamicAtt.Item3.Count; k++)
                        //    {
                        //        if (!String.IsNullOrEmpty(DynamicAtt.Item3[k].ToString()))
                        //        {
                        //            DoubleValues.Add(Convert.ToInt32(DynamicAtt.Item3[k]));
                        //            DynamicAttIds.Add(DynamicAtt.Item1);
                        //            disables.Add(false);
                        //            sideArmLibIds.Add(null);
                        //            sideArmLibraryIds.Add(null);
                        //            InventoryIds.Add(Convert.ToInt32(InsertedIds[k]));
                        //            tablesNamesIds.Add(TableNameEntity.Id);
                        //        }
                        //    }
                        //}
                        //if (DynamicAtt.Item2 == "datetime")
                        //{
                        //    for (int k = 0; k < DynamicAtt.Item3.Count; k++)
                        //    {
                        //        if (!String.IsNullOrEmpty(DynamicAtt.Item3[k].ToString()))
                        //        {
                        //            DateTimeValues.Add(Convert.ToDateTime(DynamicAtt.Item3[k]));
                        //            DynamicAttIds.Add(DynamicAtt.Item1);
                        //            disables.Add(false);
                        //            sideArmLibIds.Add(null);
                        //            sideArmLibraryIds.Add(null);
                        //            InventoryIds.Add(Convert.ToInt32(InsertedIds[k]));
                        //            tablesNamesIds.Add(TableNameEntity.Id);
                        //        }
                        //    }
                        //}
                        //if (DynamicAtt.Item2 == "boolean")
                        //{
                        //    for (int k = 0; k < DynamicAtt.Item3.Count; k++)
                        //    {
                        //        if (!String.IsNullOrEmpty(DynamicAtt.Item3[k].ToString()))
                        //        {
                        //            bool BoolValue = Convert.ToBoolean(DynamicAtt.Item3[k]);
                        //            if (BoolValue == true)
                        //            {
                        //                BooleanValues.Add(1);
                        //            }
                        //            else
                        //            {
                        //                BooleanValues.Add(0);
                        //            }
                        //            DynamicAttIds.Add(DynamicAtt.Item1);
                        //            disables.Add(false);
                        //            sideArmLibIds.Add(null);
                        //            sideArmLibraryIds.Add(null);
                        //            InventoryIds.Add(Convert.ToInt32(InsertedIds[k]));
                        //            tablesNamesIds.Add(TableNameEntity.Id);
                        //        }
                        //    }
                        //}
                        ////connection.Open();
                        //OracleParameter value = new OracleParameter();
                        //if (DynamicAtt.Item2 == "string")
                        //{
                        //    value.OracleDbType = OracleDbType.NVarchar2;
                        //    value.Value = StringValues.ToArray();
                        //}
                        //else if (DynamicAtt.Item2 == "double")
                        //{
                        //    value.OracleDbType = OracleDbType.BinaryDouble;
                        //    value.Value = DoubleValues.ToArray();
                        //}
                        //else if (DynamicAtt.Item2 == "datetime")
                        //{
                        //    value.OracleDbType = OracleDbType.Date;
                        //    value.Value = DateTimeValues.ToArray();
                        //}
                        //else if (DynamicAtt.Item2 == "boolean")
                        //{
                        //    value.OracleDbType = OracleDbType.Int32;
                        //    value.Value = BooleanValues.ToArray();
                        //}
                        //OracleParameter DynamicAttId = new OracleParameter();
                        //DynamicAttId.OracleDbType = OracleDbType.Int32;
                        //DynamicAttId.Value = DynamicAttIds.ToArray();

                        //OracleParameter disable = new OracleParameter();
                        //disable.OracleDbType = OracleDbType.Boolean;
                        //disable.Value = disables.ToArray();

                        //OracleParameter sideArmLibId = new OracleParameter();
                        //sideArmLibId.OracleDbType = OracleDbType.Int32;
                        //sideArmLibId.Value = sideArmLibIds.ToArray();

                        //OracleParameter sideArmLibraryId = new OracleParameter();
                        //sideArmLibraryId.OracleDbType = OracleDbType.Int32;
                        //sideArmLibraryId.Value = sideArmLibraryIds.ToArray();

                        //OracleParameter InventoryId = new OracleParameter();
                        //InventoryId.OracleDbType = OracleDbType.Int32;
                        //InventoryId.Value = InventoryIds.ToArray();

                        //OracleParameter tablesNamesId = new OracleParameter();
                        //tablesNamesId.OracleDbType = OracleDbType.Int32;
                        //tablesNamesId.Value = tablesNamesIds.ToArray();

                        //OracleCommand dalvcmd = connection.CreateCommand();
                        //if (DynamicAtt.Item2 == "string")
                        //{
                        //    dalvcmd.CommandText = "INSERT INTO \"TLIdynamicAttLibValue\" (\"DynamicAttId\", \"InventoryId\", \"tablesNamesId\", \"ValueString\") VALUES ( :1, :2, :3, :4)";
                        //    dalvcmd.ArrayBindCount = StringValues.Count;
                        //}
                        //else if (DynamicAtt.Item2 == "double")
                        //{
                        //    dalvcmd.CommandText = "INSERT INTO \"TLIdynamicAttLibValue\" (\"DynamicAttId\", \"InventoryId\", \"tablesNamesId\", \"ValueDouble\") VALUES ( :1, :2, :3, :4)";
                        //    dalvcmd.ArrayBindCount = DoubleValues.Count;
                        //}
                        //else if (DynamicAtt.Item2 == "datetime")
                        //{
                        //    dalvcmd.CommandText = "INSERT INTO \"TLIdynamicAttLibValue\" (\"DynamicAttId\", \"InventoryId\", \"tablesNamesId\", \"ValueDateTime\") VALUES ( :1, :2, :3, :4)";
                        //    dalvcmd.ArrayBindCount = DateTimeValues.Count;
                        //}
                        //else if (DynamicAtt.Item2 == "boolean")
                        //{
                        //    dalvcmd.CommandText = "INSERT INTO \"TLIdynamicAttLibValue\" (\"DynamicAttId\", \"InventoryId\", \"tablesNamesId\", \"ValueBoolean\") VALUES ( :1, :2, :3, :4)";
                        //    dalvcmd.ArrayBindCount = BooleanValues.Count;
                        //}
                        ////dalvcmd.Parameters.Clear();
                        //dalvcmd.Parameters.Add(DynamicAttId);
                        //dalvcmd.Parameters.Add(InventoryId);
                        //dalvcmd.Parameters.Add(tablesNamesId);
                        //dalvcmd.Parameters.Add(value);
                        //dalvcmd.ExecuteNonQuery();
                    }
                    RecordId.AddRange(InsertedIds);
                }
            ERROR:;
            }
            catch (Exception err)
            {

                throw err;
            }
        }

        private void SavesideArmLibraryUsingOracleBulkCopy(int UserId, out List<int> RecordId, DataTable dt, ref List<KeyValuePair<int, string>> UnsavedRows, int ActColumns, int Columns, ExcelWorksheet sheet, TLItablesNames TableNameEntity, OracleConnection connection)
        {
            try
            {
                RecordId = new List<int>();
                List<string> Models = _unitOfWork.SideArmLibraryRepository.GetSelect(x => x.Model).ToList();

                TLIsideArmLibrary LastId = _serviceProvider.GetService<ApplicationDbContext>().
                    TLIsideArmLibrary.OrderByDescending(a => a.Id).FirstOrDefault();

                List<TLIdynamicAtt> SideArmLibraryDynamicAtts = _unitOfWork.DynamicAttRepository.GetIncludeWhere(x =>
                    x.tablesNamesId == TableNameEntity.Id && !x.disable, x => x.DataType).ToList();

                List<string> models = new List<string>();
                List<float> widths = new List<float>();
                List<float> weights = new List<float>();
                List<float> lengths = new List<float>();
                List<float> heights = new List<float>();
                List<float> spaceLibraries = new List<float>();
                List<string> notes = new List<string>();
                List<Tuple<int, string, List<dynamic>>> DynamicAtts = new List<Tuple<int, string, List<dynamic>>>();
                TLIdynamicAtt DA = new TLIdynamicAtt();
                string ColName = null;
                //List<string> test = new List<string>();
                if (ActColumns > 0)
                {
                    for (int i = ActColumns; i <= Columns; i++)
                    {
                        ColName = sheet.Cells[1, i].Value.ToString();
                        DA = SideArmLibraryDynamicAtts.FirstOrDefault(x => x.Key == ColName);
                        if (DA != null)
                            DynamicAtts.Add(new Tuple<int, string, List<dynamic>>(DA.Id, DA.DataType.Name.ToLower(), new List<dynamic>()));
                    }
                }
                bool test;
                for (int j = 0; j < dt.Rows.Count; j++)
                {
                    try
                    {
                        string model_test = null;
                        if (dt.Columns.Contains("Model"))
                        {
                            if ((Models.Find(x => x == Convert.ToString(dt.Rows[j]["Model"]))) != null)
                            {
                                UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"Model is already exist in the row {j + 2}"));
                                goto ERROR;
                            }
                            if (!String.IsNullOrEmpty(dt.Rows[j]["Model"].ToString()))
                            {
                                model_test = Convert.ToString(dt.Rows[j]["Model"]);
                            }
                        }

                        string note_test = null;
                        if (dt.Columns.Contains("Note"))
                        {
                            if (!String.IsNullOrEmpty(dt.Rows[j]["Note"].ToString()))
                            {
                                note_test = Convert.ToString(dt.Rows[j]["Note"]);
                            }
                        }

                        float Width_test = 0;
                        if (dt.Columns.Contains("Width"))
                        {
                            test = float.TryParse(dt.Rows[j]["Width"].ToString(), out Width_test);
                            if (test == false)
                            {
                                UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"Width Wrong Input DataType In The Row {j + 2}"));
                                goto ERROR;
                            }
                        }

                        float Weight_test = 0;
                        if (dt.Columns.Contains("Weight"))
                        {
                            test = float.TryParse(dt.Rows[j]["Weight"].ToString(), out Weight_test);
                            if (test == false)
                            {
                                UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"Weight Wrong Input DataType In The Row {j + 2}"));
                                goto ERROR;
                            }
                        }

                        float Length_test = 0;
                        if (dt.Columns.Contains("Length"))
                        {
                            test = float.TryParse(dt.Rows[j]["Length"].ToString(), out Length_test);
                            if (test == false)
                            {
                                UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"Length Wrong Input DataType In The Row {j + 2}"));
                                goto ERROR;
                            }
                        }

                        float Height_test = 0;
                        if (dt.Columns.Contains("Height"))
                        {
                            test = float.TryParse(dt.Rows[j]["Height"].ToString(), out Height_test);
                            if (test == false)
                            {
                                UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"Height Wrong Input DataType In The Row {j + 2}"));
                                goto ERROR;
                            }
                        }

                        float SpaceLibrary_test = 0;
                        if (dt.Columns.Contains("SpaceLibrary"))
                        {
                            test = float.TryParse(dt.Rows[j]["SpaceLibrary"].ToString(), out SpaceLibrary_test);
                            if (test == false)
                            {
                                UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"SpaceLibrary Wrong Input DataType in the row {j + 2}"));
                                goto ERROR;
                            }
                        }

                        List<dynamic> DynamicAttList = new List<dynamic>();
                        if (ActColumns > 0)
                        {
                            for (int i = ActColumns; i <= Columns; i++)
                            {
                                string StringDynamicAtt = string.Empty;
                                double? DoubleDynamicAtt = null;
                                DateTime? DateTimeDynamicAtt = null;
                                Boolean? BooleanDynamicAtt = null;
                                ColName = sheet.Cells[1, i].Value.ToString();
                                DA = SideArmLibraryDynamicAtts.FirstOrDefault(x => x.Key == ColName);
                                //Check dynamic attribute if required or not
                                if (DA != null)
                                {
                                    double double_Test = 0;
                                    DateTime datetime_Test = DateTime.Now;
                                    Boolean boolean_Test = false;
                                    if (DA.Required == true)
                                    {
                                        //if somthing wrong then add row to UnsavedRows
                                        if (String.IsNullOrEmpty(dt.Rows[j][ColName].ToString()))
                                        {
                                            UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"Dynamic Attribute ({ColName}) is required"));
                                            goto ERROR;
                                        }
                                        else
                                        {
                                            if (DA.DataType.Name.ToLower() == "string")
                                            {

                                                StringDynamicAtt = Convert.ToString(dt.Rows[j][ColName]);
                                            }
                                            else if (DA.DataType.Name.ToLower() == "double" || DA.DataType.Name.ToLower() == "int")
                                            {
                                                test = double.TryParse(dt.Rows[j][ColName].ToString(), out double_Test);
                                                if (test == false)
                                                {
                                                    UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"{ColName} Dynamic Attribute Wrong Input DataType in the row {j + 2}"));
                                                    goto ERROR;
                                                }
                                                DoubleDynamicAtt = double_Test;
                                            }
                                            else if (DA.DataType.Name.ToLower() == "datetime")
                                            {
                                                test = DateTime.TryParse(dt.Rows[j][ColName].ToString(), out datetime_Test);
                                                if (test == false)
                                                {
                                                    UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"{ColName} Dynamic Attribute Wrong Input DataType in the row {j + 2}"));
                                                    goto ERROR;
                                                }
                                                DateTimeDynamicAtt = datetime_Test;
                                            }
                                            else if (DA.DataType.Name.ToLower() == "bool")
                                            {
                                                test = Boolean.TryParse(dt.Rows[j][ColName].ToString(), out boolean_Test);
                                                if (test == false)
                                                {
                                                    UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"{ColName} Dynamic Attribute Wrong Input DataType in the row {j + 2}"));
                                                    goto ERROR;
                                                }
                                                BooleanDynamicAtt = boolean_Test;
                                            }


                                        }
                                    }
                                    else
                                    {
                                        if (!String.IsNullOrEmpty(dt.Rows[j][ColName].ToString()))
                                        {
                                            if (DA.DataType.Name.ToLower() == "string")
                                            {
                                                StringDynamicAtt = Convert.ToString(dt.Rows[j][ColName]);
                                            }
                                            else if (DA.DataType.Name.ToLower() == "double" || DA.DataType.Name.ToLower() == "int")
                                            {
                                                test = double.TryParse(dt.Rows[j][ColName].ToString(), out double_Test);
                                                if (test == false)
                                                {
                                                    UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"{ColName} Dynamic Attribute Wrong Input DataType in the row {j + 2}"));
                                                    goto ERROR;
                                                }
                                                DoubleDynamicAtt = double_Test;
                                            }
                                            else if (DA.DataType.Name.ToLower() == "datetime")
                                            {
                                                test = DateTime.TryParse(dt.Rows[j][ColName].ToString(), out datetime_Test);
                                                if (test == false)
                                                {
                                                    UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"{ColName} Dynamic Attribute Wrong Input DataType in the row {j + 2}"));
                                                    goto ERROR;
                                                }
                                                DateTimeDynamicAtt = datetime_Test;

                                            }
                                            else if (DA.DataType.Name.ToLower() == "bool")
                                            {
                                                test = Boolean.TryParse(dt.Rows[j][ColName].ToString(), out boolean_Test);
                                                if (test == false)
                                                {
                                                    UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, $"{ColName} Dynamic Attribute Wrong Input DataType in the row {j + 2}"));
                                                    goto ERROR;
                                                }
                                                BooleanDynamicAtt = boolean_Test;
                                            }

                                        }
                                    }
                                    if (DA.DataType.Name.ToLower() == "string")
                                    {
                                        DynamicAttList.Add(StringDynamicAtt);
                                    }
                                    else if (DA.DataType.Name.ToLower() == "double" || DA.DataType.Name.ToLower() == "int")
                                    {
                                        DynamicAttList.Add(DoubleDynamicAtt);
                                    }
                                    else if (DA.DataType.Name.ToLower() == "datetime")
                                    {
                                        DynamicAttList.Add(DateTimeDynamicAtt);
                                    }
                                    else if (DA.DataType.Name.ToLower() == "bool")
                                    {
                                        DynamicAttList.Add(BooleanDynamicAtt);
                                    }


                                }
                            }
                        }

                        models.Add(model_test != null ? model_test : null);
                        widths.Add(Width_test != 0 ? Width_test : 0);
                        weights.Add(Weight_test != 0 ? Weight_test : 0);
                        lengths.Add(Length_test != 0 ? Length_test : 0);
                        heights.Add(Height_test != 0 ? Height_test : 0);
                        spaceLibraries.Add(SpaceLibrary_test != 0 ? SpaceLibrary_test : 0);
                        notes.Add(note_test != null ? note_test : null);
                        for (int f = 0; f < DynamicAttList.Count; f++)
                        {
                            //var h = DynamicAtts[f].Value;
                            DynamicAtts[f].Item3.Add(DynamicAttList[f]);
                            //h.Add(DynamicAttList[f]);
                        }


                    }
                    catch (Exception err)
                    {

                        //UnsavedRows.Add(new KeyValuePair<int, string>(j + 2, "Wrong Input DataType"));
                    }
                }
                if (models.Count > 0)
                {
                    OracleParameter model = new OracleParameter();
                    model.OracleDbType = OracleDbType.NVarchar2;
                    model.Value = models.ToArray();

                    OracleParameter width = new OracleParameter();
                    width.OracleDbType = OracleDbType.BinaryFloat;
                    width.Value = widths.ToArray();

                    OracleParameter weight = new OracleParameter();
                    weight.OracleDbType = OracleDbType.BinaryFloat;
                    weight.Value = weights.ToArray();

                    OracleParameter length = new OracleParameter();
                    length.OracleDbType = OracleDbType.BinaryFloat;
                    length.Value = lengths.ToArray();

                    OracleParameter Height = new OracleParameter();
                    Height.OracleDbType = OracleDbType.BinaryFloat;
                    Height.Value = heights.ToArray();

                    OracleParameter SpaceLibrary = new OracleParameter();
                    SpaceLibrary.OracleDbType = OracleDbType.BinaryFloat;
                    SpaceLibrary.Value = spaceLibraries.ToArray();

                    OracleParameter note = new OracleParameter();
                    note.OracleDbType = OracleDbType.NVarchar2;
                    note.Value = notes.ToArray();

                    // create command and set properties
                    OracleCommand cmd = connection.CreateCommand();
                    cmd.CommandText = "INSERT INTO \"TLIsideArmLibrary\" (\"Model\", \"Width\", \"Weight\", \"Length\", \"Height\", \"SpaceLibrary\", \"Note\") VALUES ( :1, :2, :3, :4, :5, :6, :7)";
                    cmd.ArrayBindCount = models.Count;
                    cmd.Parameters.Add(model);
                    cmd.Parameters.Add(width);
                    cmd.Parameters.Add(weight);
                    cmd.Parameters.Add(length);
                    cmd.Parameters.Add(Height);
                    cmd.Parameters.Add(SpaceLibrary);
                    cmd.Parameters.Add(note);
                    cmd.ExecuteNonQuery();
                    //connection.Close();
                    List<int> InsertedIds = new List<int>();
                    if (LastId != null)
                    {
                        OracleCommand InventoryCmd = connection.CreateCommand();
                        InventoryCmd.CommandType = CommandType.Text;
                        InventoryCmd.CommandText = $"Select \"Id\" From \"TLIsideArmLibrary\" where \"TLIsideArmLibrary\".\"Id\" > {LastId.Id}";
                        OracleDataReader Reader = InventoryCmd.ExecuteReader();
                        if (Reader.HasRows)
                        {
                            while (Reader.Read())
                            {
                                InsertedIds.Add(Reader.GetInt32(0));
                            }
                        }
                    }
                    else
                    {
                        OracleCommand InventoryCmd = connection.CreateCommand();
                        InventoryCmd.CommandType = CommandType.Text;
                        InventoryCmd.CommandText = $"Select \"Id\" From \"TLIsideArmLibrary\"";
                        OracleDataReader Reader = InventoryCmd.ExecuteReader();
                        if (Reader.HasRows)
                        {
                            while (Reader.Read())
                            {
                                InsertedIds.Add(Reader.GetInt32(0));
                            }
                        }
                    }
                    TLIhistory tLIhistory = new TLIhistory()
                    {
                        UserId = UserId,
                        TablesNameId = TableNameEntity.Id,
                        HistoryTypeId = 1,

                    };
                    _dbContext.TLIhistory.Add(tLIhistory);
                    _dbContext.SaveChanges();
                    var sideArmLibraryist = new List<TLIsideArmLibrary>();

                    for (int i = 0; i < models.Count; i++)
                    {
                        sideArmLibraryist.Add(new TLIsideArmLibrary
                        {
                            Id = InsertedIds[i],
                            Model = models[i],
                            Width = widths[i],
                            Weight = weights[i],
                            Length = lengths[i],
                            Height = heights[i],
                            SpaceLibrary = spaceLibraries[i],


                        });
                    }


                    foreach (int recordId in InsertedIds)
                    {
                        var RecordName = sideArmLibraryist.FirstOrDefault(x => x.Id == recordId);
                        if (RecordName != null)
                        {
                            var attributeNames = RecordName.GetType().GetProperties()
                                .Where(x =>
                                    (x.PropertyType.IsGenericType && x.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>) &&
                                        new Type[] { typeof(int), typeof(string), typeof(double), typeof(float), typeof(Single), typeof(bool), typeof(DateTime) }
                                        .Contains(x.PropertyType.GetGenericArguments()[0]))
                                    || new Type[] { typeof(int), typeof(string), typeof(double), typeof(bool), typeof(DateTime), typeof(float), typeof(Single) }
                                    .Contains(x.PropertyType)
                                )
                                .ToList();

                            foreach (var propertyInfo in attributeNames)
                            {

                                var propertyName = propertyInfo.Name;

                                var propertyValue = propertyInfo.GetValue(RecordName)?.ToString().Trim();

                                TLIhistoryDet tLIhistoryDet = new TLIhistoryDet()
                                {
                                    HistoryId = tLIhistory.Id,
                                    TablesNameId = TableNameEntity.Id,
                                    RecordId = recordId.ToString(),
                                    AttributeName = propertyName,
                                    NewValue = propertyValue
                                };


                                _dbContext.TLIhistoryDet.Add(tLIhistoryDet);
                            }
                        }
                    }
                    _dbContext.SaveChanges();

                    foreach (var DynamicAtt in DynamicAtts)
                    {
                        for (int k = 0; k < DynamicAtt.Item3.Count; k++)
                        {
                            if (!String.IsNullOrEmpty(DynamicAtt.Item3[k].ToString()))
                            {

                                var Message = _unitOfWork.CivilWithLegsRepository.CheckDynamicValidationAndDependenceDynamic(DynamicAtt.Item1, DynamicAtt.Item3[k], Convert.ToInt32(InsertedIds[k]), tLIhistory.Id, sideArmLibraryist[0]).Message;
                                if (Message != "Success")
                                {
                                    UnsavedRows.Add(new KeyValuePair<int, string>(k + 2, Message + ' ' + $"in the Colum Name{ColName} in the Row Number {k + 2} "));
                                    goto ERROR;
                                }

                            }

                        }
                        //List<string> StringValues = new List<string>();
                        //List<double> DoubleValues = new List<double>();
                        //List<DateTime> DateTimeValues = new List<DateTime>();
                        //List<Int32> BooleanValues = new List<Int32>();
                        //List<int> DynamicAttIds = new List<int>();
                        //List<bool> disables = new List<bool>();
                        //List<int?> sideArmLibIds = new List<int?>();
                        //List<int?> sideArmLibraryIds = new List<int?>();
                        //List<int> InventoryIds = new List<int>();
                        //List<int> tablesNamesIds = new List<int>();
                        //if (DynamicAtt.Item2 == "string")
                        //{
                        //    for (int k = 0; k < DynamicAtt.Item3.Count; k++)
                        //    {
                        //        if (!String.IsNullOrEmpty(DynamicAtt.Item3[k].ToString()))
                        //        {
                        //            StringValues.Add(DynamicAtt.Item3[k].ToString());
                        //            DynamicAttIds.Add(DynamicAtt.Item1);
                        //            disables.Add(false);
                        //            sideArmLibIds.Add(null);
                        //            sideArmLibraryIds.Add(null);
                        //            InventoryIds.Add(Convert.ToInt32(InsertedIds[k]));
                        //            tablesNamesIds.Add(TableNameEntity.Id);
                        //        }
                        //    }
                        //}
                        //if (DynamicAtt.Item2 == "double")
                        //{
                        //    for (int k = 0; k < DynamicAtt.Item3.Count; k++)
                        //    {
                        //        if (!String.IsNullOrEmpty(DynamicAtt.Item3[k].ToString()))
                        //        {
                        //            DoubleValues.Add(Convert.ToInt32(DynamicAtt.Item3[k]));
                        //            DynamicAttIds.Add(DynamicAtt.Item1);
                        //            disables.Add(false);
                        //            sideArmLibIds.Add(null);
                        //            sideArmLibraryIds.Add(null);
                        //            InventoryIds.Add(Convert.ToInt32(InsertedIds[k]));
                        //            tablesNamesIds.Add(TableNameEntity.Id);
                        //        }
                        //    }
                        //}
                        //if (DynamicAtt.Item2 == "datetime")
                        //{
                        //    for (int k = 0; k < DynamicAtt.Item3.Count; k++)
                        //    {
                        //        if (!String.IsNullOrEmpty(DynamicAtt.Item3[k].ToString()))
                        //        {
                        //            DateTimeValues.Add(Convert.ToDateTime(DynamicAtt.Item3[k]));
                        //            DynamicAttIds.Add(DynamicAtt.Item1);
                        //            disables.Add(false);
                        //            sideArmLibIds.Add(null);
                        //            sideArmLibraryIds.Add(null);
                        //            InventoryIds.Add(Convert.ToInt32(InsertedIds[k]));
                        //            tablesNamesIds.Add(TableNameEntity.Id);
                        //        }
                        //    }
                        //}
                        //if (DynamicAtt.Item2 == "boolean")
                        //{
                        //    for (int k = 0; k < DynamicAtt.Item3.Count; k++)
                        //    {
                        //        if (!String.IsNullOrEmpty(DynamicAtt.Item3[k].ToString()))
                        //        {
                        //            bool BoolValue = Convert.ToBoolean(DynamicAtt.Item3[k]);
                        //            if (BoolValue == true)
                        //            {
                        //                BooleanValues.Add(1);
                        //            }
                        //            else
                        //            {
                        //                BooleanValues.Add(0);
                        //            }
                        //            DynamicAttIds.Add(DynamicAtt.Item1);
                        //            disables.Add(false);
                        //            sideArmLibIds.Add(null);
                        //            sideArmLibraryIds.Add(null);
                        //            InventoryIds.Add(Convert.ToInt32(InsertedIds[k]));
                        //            tablesNamesIds.Add(TableNameEntity.Id);
                        //        }
                        //    }
                        //}
                        ////connection.Open();
                        //OracleParameter value = new OracleParameter();
                        //if (DynamicAtt.Item2 == "string")
                        //{
                        //    value.OracleDbType = OracleDbType.NVarchar2;
                        //    value.Value = StringValues.ToArray();
                        //}
                        //else if (DynamicAtt.Item2 == "double")
                        //{
                        //    value.OracleDbType = OracleDbType.BinaryDouble;
                        //    value.Value = DoubleValues.ToArray();
                        //}
                        //else if (DynamicAtt.Item2 == "datetime")
                        //{
                        //    value.OracleDbType = OracleDbType.Date;
                        //    value.Value = DateTimeValues.ToArray();
                        //}
                        //else if (DynamicAtt.Item2 == "boolean")
                        //{
                        //    value.OracleDbType = OracleDbType.Int32;
                        //    value.Value = BooleanValues.ToArray();
                        //}
                        //OracleParameter DynamicAttId = new OracleParameter();
                        //DynamicAttId.OracleDbType = OracleDbType.Int32;
                        //DynamicAttId.Value = DynamicAttIds.ToArray();

                        //OracleParameter disable = new OracleParameter();
                        //disable.OracleDbType = OracleDbType.Boolean;
                        //disable.Value = disables.ToArray();

                        //OracleParameter sideArmLibId = new OracleParameter();
                        //sideArmLibId.OracleDbType = OracleDbType.Int32;
                        //sideArmLibId.Value = sideArmLibIds.ToArray();

                        //OracleParameter sideArmLibraryId = new OracleParameter();
                        //sideArmLibraryId.OracleDbType = OracleDbType.Int32;
                        //sideArmLibraryId.Value = sideArmLibraryIds.ToArray();

                        //OracleParameter InventoryId = new OracleParameter();
                        //InventoryId.OracleDbType = OracleDbType.Int32;
                        //InventoryId.Value = InventoryIds.ToArray();

                        //OracleParameter tablesNamesId = new OracleParameter();
                        //tablesNamesId.OracleDbType = OracleDbType.Int32;
                        //tablesNamesId.Value = tablesNamesIds.ToArray();

                        //OracleCommand dalvcmd = connection.CreateCommand();
                        //if (DynamicAtt.Item2 == "string")
                        //{
                        //    dalvcmd.CommandText = "INSERT INTO \"TLIdynamicAttLibValue\" (\"DynamicAttId\", \"InventoryId\", \"tablesNamesId\", \"ValueString\") VALUES ( :1, :2, :3, :4)";
                        //    dalvcmd.ArrayBindCount = StringValues.Count;
                        //}
                        //else if (DynamicAtt.Item2 == "double")
                        //{
                        //    dalvcmd.CommandText = "INSERT INTO \"TLIdynamicAttLibValue\" (\"DynamicAttId\", \"InventoryId\", \"tablesNamesId\", \"ValueDouble\") VALUES ( :1, :2, :3, :4)";
                        //    dalvcmd.ArrayBindCount = DoubleValues.Count;
                        //}
                        //else if (DynamicAtt.Item2 == "datetime")
                        //{
                        //    dalvcmd.CommandText = "INSERT INTO \"TLIdynamicAttLibValue\" (\"DynamicAttId\", \"InventoryId\", \"tablesNamesId\", \"ValueDateTime\") VALUES ( :1, :2, :3, :4)";
                        //    dalvcmd.ArrayBindCount = DateTimeValues.Count;
                        //}
                        //else if (DynamicAtt.Item2 == "boolean")
                        //{
                        //    dalvcmd.CommandText = "INSERT INTO \"TLIdynamicAttLibValue\" (\"DynamicAttId\", \"InventoryId\", \"tablesNamesId\", \"ValueBoolean\") VALUES ( :1, :2, :3, :4)";
                        //    dalvcmd.ArrayBindCount = BooleanValues.Count;
                        //}
                        ////dalvcmd.Parameters.Clear();
                        //dalvcmd.Parameters.Add(DynamicAttId);
                        //dalvcmd.Parameters.Add(InventoryId);
                        //dalvcmd.Parameters.Add(tablesNamesId);
                        //dalvcmd.Parameters.Add(value);
                        //dalvcmd.ExecuteNonQuery();
                    }
                    RecordId.AddRange(InsertedIds);
                }
            ERROR:;
            }
            catch (Exception err)
            {

                throw err;
            }
        }
        //Function to attach file take 6 parameters
        //First the file i want to attach
        //Second Model to add it to file name if i deal with library table
        //Third Name to add it to file name if i deal with Inst table
        //Fourth SiteCode to add it to file name if i deal with site
        //Fifth RecordId to add it to file name
        //Sixth TableName to specify the table i deal with
        public Response<string> AttachFile(int UserId,IFormFile file, string SiteCode, string? RecordId, string TableName, string connection, string AttachFolder, string asset,bool ExternalSys)
        {
            try
            {
                int? Record= null;
                string OldValueOfTableName = TableName;
                if (TableName.ToLower() == "TLIcabinetPower".ToLower() || TableName.ToLower() == "TLIcabinetTelecom".ToLower())
                    TableName = "TLIcabinet";
                 TLIattachedFiles LastId = _serviceProvider.GetService<ApplicationDbContext>().
                   TLIattachedFiles.OrderByDescending(a => a.Id).FirstOrDefault();
                // Set list of image types to check later if attach file is image or not
                List<string> ImgTypes = new List<string>() { "JPEG", "JPG", "PNG", "GIF", "TIFF", "PSD", "AI", "INDD", "RAW" };

                // Get table name entity by table name
                var TableNamesEntity = _unitOfWork.TablesNamesRepository.GetIncludeWhereFirst(x => x.TableName == TableName);
                var FileName = file.FileName;

                // Check if the file is already exist
                if (RecordId != null)
                {
                     Record = Convert.ToInt32(RecordId);
                }
                else
                {
                     Record = null;
                }
                var FileExists = _unitOfWork.AttachedFilesRepository.GetWhereFirst(x => x.Name == FileName && x.RecordId == Record && x.SiteCode == SiteCode);

                // If file exist then return error message 
                if (FileExists != null)
                {
                    return new Response<string>(false, null, null, "The file is already exist", (int)Helpers.Constants.ApiReturnCode.fail);
                }
                var FullFileName = FileName;
                var SplitFileName = FileName.Split('.');
                //Get file name
                FileName = SplitFileName[0];
                //Get file type
                var FileType = SplitFileName[1];
                //Check if file is img
                bool IsImg = ImgTypes.Contains(FileType.ToUpper());
                //Get current directory
                // string fileDirectory = Directory.GetCurrentDirectory();
                string DirectoryPath = null;
                string FilePath = null;

                //string fileDirectory = "\\\\192.168.1.50\\Share folder\\TLIATTACHED";
                string fileDirectory = AttachFolder;

                //Create file path depened on table name
                if (TableName.Contains("Library"))
                {
                    TLIsite site = new TLIsite();
                    site.SiteCode = null;
                    DirectoryPath = Path.Combine(fileDirectory, "AttachFiles", "Library", TableName);
                    FilePath = Path.Combine(DirectoryPath, $"{FileName}_{RecordId.ToString()}_{DateTime.Now.ToString("yyyy_MM_dd")}.{FileType}");
                }
                else if (TableName == "TLIsite")
                {
                    if (IsImg == true)
                    {
                        fileDirectory = asset;
                        DirectoryPath = fileDirectory;
                        FilePath = Path.Combine(fileDirectory, $"{FileName}.{FileType}");

                    }
                    else
                    {
                        DirectoryPath = Path.Combine(fileDirectory, "AttachFiles", "Site");
                        FilePath = Path.Combine(DirectoryPath, $"{FileName}.{FileType}");
                    }


                }


                else
                {
                    DirectoryPath = Path.Combine(fileDirectory, "AttachFiles", "Installation", OldValueOfTableName);
                    FilePath = Path.Combine(DirectoryPath, $"{FileName}_{RecordId.ToString()}_{DateTime.Now.ToString("yyyy_MM_dd")}.{FileType}");
                }
                //Check if DirectoryPath not exist then create it
                if (!Directory.Exists(DirectoryPath))
                {
                    Directory.CreateDirectory(DirectoryPath);
                }
                FilePath = Path.Combine(DirectoryPath, $"{FileName}.{FileType}");
                //Create file path
                using (var stream = new FileStream(FilePath, FileMode.Create))
                {
                    file.CopyTo(stream);
                }
                //Add attach file to TLIattachedFiles table in database
                //using ADO method
                double FileSizePerMega = (double)(file.Length / 1024) / 1042;

                var connectionString = new OracleConnection(connection);
                var IsImage = IsImg == false ? 0 : 1;

                OracleCommand cmd = connectionString.CreateCommand();
                var recordValue = Record.HasValue ? Record.Value.ToString() : "NULL"; // "NULL" سيتم استبدالها بالقيمة الفارغة الفعلية في SQL

                if (TableName.Contains("Library"))
                {
                    cmd.CommandText = "INSERT INTO \"TLIattachedFiles\" (\"Name\", \"Path\", \"RecordId\", \"tablesNamesId\", \"IsImg\", \"documenttypeId\", \"fileSize\", \"SiteCode\", \"Description\", \"Description2\", \"UnAttached\")" +
                       $" VALUES ('{FullFileName}', '{FilePath}', {(Record.HasValue ? Record.Value.ToString() : "NULL")}, {TableNamesEntity.Id}, {IsImage}, NULL, {FileSizePerMega}, '{SiteCode}', '', '', 0)";
                }
                else if (TableName.Contains("TLIsite"))
                {
                    if (IsImg)
                    {
                        FilePath = Path.Combine($"{asset}\\galleria", $"{FileName}.{FileType}");
                    }
                    cmd.CommandText = "INSERT INTO \"TLIattachedFiles\" (\"Name\", \"Path\", \"RecordId\", \"tablesNamesId\", \"IsImg\", \"documenttypeId\", \"fileSize\", \"SiteCode\", \"Description\", \"Description2\", \"UnAttached\")" +
                       $" VALUES ('{FullFileName}', '{FilePath}', {(Record.HasValue ? Record.Value.ToString() : "NULL")}, {TableNamesEntity.Id}, {IsImage}, NULL, {FileSizePerMega}, '{SiteCode}', '', '', 0)";
                }
                else
                {
                    cmd.CommandText = "INSERT INTO \"TLIattachedFiles\" (\"Name\", \"Path\", \"RecordId\", \"tablesNamesId\", \"IsImg\", \"documenttypeId\", \"fileSize\", \"SiteCode\", \"Description\", \"Description2\", \"UnAttached\")" +
                       $" VALUES ('{FullFileName}', '{FilePath}', {(Record.HasValue ? Record.Value.ToString() : "NULL")}, {TableNamesEntity.Id}, {IsImage}, NULL, {FileSizePerMega}, '{SiteCode}, '', '', 0)";
                }




                connectionString.Open();
                cmd.ExecuteNonQuery();
  
                //  AddHistoryForUnAttached(attachedFiles.Id, "Add", "TLIattachedFiles");
                var TabelNameId= _unitOfWork.TablesNamesRepository.GetWhereFirst(x => x.TableName == "TLIattachedFiles").Id;
                int newRecordId = 0;
                if (LastId != null)
                {
                    OracleCommand InventoryCmd = connectionString.CreateCommand();
                    InventoryCmd.CommandType = CommandType.Text;
                    InventoryCmd.CommandText = $"Select \"Id\" From \"TLIattachedFiles\" where \"TLIattachedFiles\".\"Id\" > {LastId.Id}";
                    OracleDataReader Reader = InventoryCmd.ExecuteReader();
                    if (Reader.HasRows)
                    {
                        while (Reader.Read())
                        {
                            newRecordId=Reader.GetInt32(0);
                        }
                    }
                }
                else
                {
                    OracleCommand InventoryCmd = connectionString.CreateCommand();
                    InventoryCmd.CommandType = CommandType.Text;
                    InventoryCmd.CommandText = $"Select \"Id\" From \"TLIattachedFiles\"";
                    OracleDataReader Reader = InventoryCmd.ExecuteReader();
                    if (Reader.HasRows)
                    {
                        while (Reader.Read())
                        {
                            newRecordId = Reader.GetInt32(0);
                        }
                    }
                }
                if (ExternalSys == false)
                {
                    TLIhistory tLIhistory = new TLIhistory()
                    {
                        RecordId = newRecordId.ToString(),
                        TablesNameId = TabelNameId,
                        HistoryTypeId = 1,
                        UserId = UserId,
                        SiteCode = SiteCode

                    };
                    _dbContext.TLIhistory.Add(tLIhistory);
                    _dbContext.SaveChanges();
                }
                if (ExternalSys == true)
                {
                    TLIhistory tLIhistory = new TLIhistory()
                    {
                        RecordId = newRecordId.ToString(),
                        TablesNameId = TabelNameId,
                        HistoryTypeId = 1,
                        ExternalSysId = UserId,
                        SiteCode = SiteCode

                    };
                    _dbContext.TLIhistory.Add(tLIhistory);
                    _dbContext.SaveChanges();
                }
                connectionString.Close();
                return new Response<string>(true, FilePath, null, null, (int)Helpers.Constants.ApiReturnCode.success);
            }
            catch (Exception err)
            {

                return new Response<string>(false, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
        }
        //Function take 3 parameters 
        //3 parameters helps me to get file from database
        public Response<string> DeleteFile(string FileName, int RecordId, string TableName, string SiteCode,int UserId,bool ExternaSys)
        {
            try
            {
                if (TableName.ToLower() == "TLIcabinetPower".ToLower() || TableName.ToLower() == "TLIcabinetTelecom".ToLower())
                    TableName = "TLIcabinet";

                if (RecordId != 0 || !string.IsNullOrEmpty(TableName))
                {
                    var TableNameEntity = _unitOfWork.TablesNamesRepository.GetWhereFirst(x => x.TableName == TableName);
                    var AttachFile = _unitOfWork.AttachedFilesRepository.GetWhereFirst(x => x.Name == FileName && x.RecordId == RecordId && x.tablesNamesId == TableNameEntity.Id);
                    //Find file and delete the file
                    if (File.Exists(AttachFile.Path))
                    {
                        File.Delete(AttachFile.Path);
                    }
                    //remove record from database
                    _unitOfWork.AttachedFilesRepository.RemoveItem(AttachFile);
                    if (ExternaSys == false)
                    {
                        TLIhistory tLIhistory = new TLIhistory
                        {
                            UserId = UserId,
                            HistoryTypeId = 2,
                            SiteCode = SiteCode,
                            RecordId=RecordId.ToString(),
                            TablesNameId= TableNameEntity.Id

                        };
                        _dbContext.TLIhistory.Add(tLIhistory);
                        _dbContext.SaveChanges();
                    }
                    if (ExternaSys == true)
                    {
                        TLIhistory tLIhistory = new TLIhistory
                        {
                            ExternalSysId = UserId,
                            HistoryTypeId = 2,
                            SiteCode = SiteCode,
                            RecordId = RecordId.ToString(),
                            TablesNameId = TableNameEntity.Id
                        };
                        _dbContext.TLIhistory.Add(tLIhistory);
                        _dbContext.SaveChanges();
                    }
                }

                else
                {
                    var AttachFile = _unitOfWork.AttachedFilesRepository.GetWhereFirst(x => x.Name == FileName && x.SiteCode == SiteCode);
                    //Find file and delete the file
                    if (File.Exists(AttachFile.Path))
                    {
                        File.Delete(AttachFile.Path);
                    }
                    //remove record from database
                    _unitOfWork.AttachedFilesRepository.RemoveItem(AttachFile);
                    if (ExternaSys == false)
                {
                    TLIhistory tLIhistory = new TLIhistory
                    {
                        UserId = UserId,
                        HistoryTypeId=2,
                        SiteCode=SiteCode,

                    };
                    _dbContext.TLIhistory.Add(tLIhistory);
                    _dbContext.SaveChanges();
                }
                if (ExternaSys == true)
                {
                    TLIhistory tLIhistory = new TLIhistory
                    {
                        ExternalSysId = UserId,
                        HistoryTypeId = 2,
                        SiteCode = SiteCode,

                    };
                    _dbContext.TLIhistory.Add(tLIhistory);
                    _dbContext.SaveChanges();
                }
                }
                _unitOfWork.SaveChanges();
                
                return new Response<string>(true, null, null, "File Deleted", (int)Helpers.Constants.ApiReturnCode.success);

            }
            catch (Exception err)
            {
                return new Response<string>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);

            }
        }
        //Get files specific record related to specific table
        public Response<IEnumerable<AttachedFilesViewModel>> GetFilesByRecordIdAndTableName(int RecordId, string TableName ,string SiteCode,int UserId,bool ExternalSys)
        {
            try
            {
                var TableNameEntity = _unitOfWork.TablesNamesRepository.GetIncludeWhereFirst(x => x.TableName == TableName);
                
                if (SiteCode != null)
                {

                    var AttachFiles = _unitOfWork.AttachedFilesRepository.GetWhere(x => x.SiteCode.ToLower() == SiteCode.ToLower() && x.tablesNamesId == TableNameEntity.Id && x.RecordId==RecordId).ToList();
                    var AttachFileViewModels = _mapper.Map<List<AttachedFilesViewModel>>(AttachFiles).ToList();
                    int Count = AttachFiles.Count();
                    if (ExternalSys == true)
                    {
                        var TabelNameId = _dbContext.TLItablesNames.FirstOrDefault(x => x.TableName == TableName).Id;
                        TLIhistory tLIhistory = new TLIhistory()
                        {
                            TablesNameId = TabelNameId,
                            ExternalSysId = UserId,
                            HistoryTypeId = 4,
                        };
                        _dbContext.TLIhistory.Add(tLIhistory);
                        _dbContext.SaveChanges();
                    }
                    return new Response<IEnumerable<AttachedFilesViewModel>>(true, AttachFileViewModels, null, null, (int)Helpers.Constants.ApiReturnCode.success, Count);
                }
                else
                {
                   var  AttachFiles = _unitOfWork.AttachedFilesRepository.GetWhere(x => x.RecordId == RecordId && x.tablesNamesId == TableNameEntity.Id).ToList();
                    var AttachFileViewModels = _mapper.Map<List<AttachedFilesViewModel>>(AttachFiles).ToList();
                    int Count = AttachFiles.Count();
                    if (ExternalSys == true)
                    {
                        var TabelNameId = _dbContext.TLItablesNames.FirstOrDefault(x => x.TableName == TableName).Id;
                        TLIhistory tLIhistory = new TLIhistory()
                        {
                            TablesNameId = TabelNameId,
                            ExternalSysId = UserId,
                            HistoryTypeId = 4,
                        };
                        _dbContext.TLIhistory.Add(tLIhistory);
                        _dbContext.SaveChanges();
                    }
                    return new Response<IEnumerable<AttachedFilesViewModel>>(true, AttachFileViewModels, null, null, (int)Helpers.Constants.ApiReturnCode.success, Count);
                }
            }
            catch (Exception err)
            {

                return new Response<IEnumerable<AttachedFilesViewModel>>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }

        }
        private string SaveFileAndGetFilePath(IFormFile file)
        {
            // var path = Directory.GetCurrentDirectory();
            var path = _configuration["StoreFiles"];

            path = Path.Combine(path, "TemporaryFile");
            if (Directory.Exists(path) == false)
            {
                Directory.CreateDirectory(path);
            }
            var FilePath = Path.Combine(path, file.FileName);
            if (File.Exists(FilePath) == true)
            {
                File.Delete(FilePath);
            }
            using (var stream = new FileStream(FilePath, FileMode.Create))
            {
                file.CopyTo(stream);
            }
            return FilePath;
        }
        public Response<List<AttachedFilesViewModel>> GetAttachecdFiles(int RecordId, string TableName)
        {
            try
            {
                var AttachedFiles = _unitOfWork.AttachedFilesRepository.GetIncludeWhere(x => x.RecordId == RecordId && x.tablesName.TableName == TableName && x.UnAttached == false, x => x.tablesName).ToList();
                return new Response<List<AttachedFilesViewModel>>(true, _mapper.Map<List<AttachedFilesViewModel>>(AttachedFiles), null, null, (int)Helpers.Constants.ApiReturnCode.success);
            }
            catch (Exception err)
            {

                return new Response<List<AttachedFilesViewModel>>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
        }
        public Response<List<AttachedFilesViewModel>> GetAttachecdFilesBySite(string SiteCode,int UseId,bool ExternalSys)
        {
            try
            {
                List<TLIattachedFiles> AttachedFiles = _unitOfWork.AttachedFilesRepository.GetWhereAndInclude(x => x.SiteCode == SiteCode, y => y.tablesName).ToList();


                int Count = AttachedFiles.Count();
                
               
                if (ExternalSys == true)
                {
                    var TabelNameId = _dbContext.TLItablesNames.FirstOrDefault(x => x.TableName == "TLIsite").Id;
                    TLIhistory tLIhistory = new TLIhistory()
                    {
                        TablesNameId = TabelNameId,
                        ExternalSysId = UseId,
                        HistoryTypeId = 4,
                        SiteCode= SiteCode,
                    };
                    _dbContext.TLIhistory.Add(tLIhistory);
                    _dbContext.SaveChanges();
                }
                

                return new Response<List<AttachedFilesViewModel>>(true, _mapper.Map<List<AttachedFilesViewModel>>(AttachedFiles), null, null, (int)Helpers.Constants.ApiReturnCode.success, Count);
            }
            catch (Exception err)
            {
                return new Response<List<AttachedFilesViewModel>>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
        }
        public Response<AttachedFilesViewModel> AttachedUnAttached(int Id)
        {
            try
            {
                var AttachedFile = _unitOfWork.AttachedFilesRepository.GetByID(Id);
                AttachedFile.UnAttached = !(AttachedFile.UnAttached);
                _unitOfWork.SaveChanges();
                // AddHistoryForUnAttached(AttachedFile.Id, "UnAttached", "TLIattachedFiles");
                return new Response<AttachedFilesViewModel>();

            }
            catch (Exception err)
            {

                return new Response<AttachedFilesViewModel>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
        }

        #region Add History
        public void AddHistoryForUnAttached(int RecordId, string historyType, string TableName)
        {
            AddTablesHistoryViewModel history = new AddTablesHistoryViewModel();
            history.RecordId = RecordId;
            history.TablesNameId = _unitOfWork.TablesNamesRepository.GetWhereFirst(x => x.TableName == TableName).Id;
            history.HistoryTypeId = _unitOfWork.HistoryTypeRepository.GetWhereSelectFirst(x => x.Name == historyType, x => new { x.Id }).Id;
            history.UserId = 83;
            history.Date = DateTime.Now;
            _unitOfWork.TablesHistoryRepository.AddTableHistory(history);

        }


        public Response<string> GetAttachedToDownload(string filename, int recordid, string tablename)
        {
            try
            {
                if (tablename.ToLower() == "TLIcabinetPower".ToLower() || tablename.ToLower() == "TLIcabinetTelecom".ToLower())
                    tablename = "TLIcabinet";

                var table = _unitOfWork.TablesNamesRepository.GetWhereFirst(x => x.TableName == tablename);

                if (recordid >= 0)
                {
                    var attached = _unitOfWork.AttachedFilesRepository.GetWhereFirst(x => x.RecordId == recordid && x.tablesNamesId == table.Id && x.Name == filename);
                    if (attached != null)
                    {
                        return new Response<string>(true, attached.Path, null, null, (int)Helpers.Constants.ApiReturnCode.fail);

                    }
                    return null;
                }
                else
                {
                    var attached = _unitOfWork.AttachedFilesRepository.GetWhereFirst(x => x.tablesNamesId == table.Id && x.Name == filename);


                    if (attached != null)
                    {
                        return new Response<string>(true, attached.Path, null, null, (int)Helpers.Constants.ApiReturnCode.fail);

                    }
                    return null;
                }
            }
            catch (Exception err)
            {

                return new Response<string>(false, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }




        }
        #endregion
    }
}
