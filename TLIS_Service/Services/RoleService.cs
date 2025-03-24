using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TLIS_DAL.Helpers;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModels.RoleDTOs;
using TLIS_Repository.Base;
using TLIS_Service.IService;
using TLIS_Service.Helpers;
using System.Transactions;
using System.Linq;
using TLIS_DAL.Helper.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TLIS_DAL.ViewModels.GroupDTOs;
using TLIS_DAL.ViewModels.PermissionDTOs;
using Nancy;
using AutoMapper;
using TLIS_DAL.ViewModels.NewPermissionsDTOs.Permissions;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Engineering;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using Oracle.ManagedDataAccess.Client;
using Microsoft.Extensions.Configuration;
using System.DirectoryServices.ActiveDirectory;
using TLIS_DAL.ViewModels.UserDTOs;
using static TLIS_Service.Helpers.Constants;
using TLIS_DAL;

namespace TLIS_Service.Services
{
    public class RoleService : IRoleService
    {
        IUnitOfWork _unitOfWork;
        IServiceCollection _services;
        private IMapper _mapper;
        IConfiguration _configuration;
        private readonly ApplicationDbContext _dbContext;
        public RoleService(IUnitOfWork unitOfWork, IServiceCollection services, IConfiguration configuration, ApplicationDbContext context, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _services = services;
            _configuration = configuration;
            ServiceProvider serviceProvider = _services.BuildServiceProvider();
            _mapper = mapper;
            _dbContext = context;
        }
        //Function take 1 parameter
        //check the name of the role if already exists the return error message
        //else add role and permissions to the role
        public async Task<Response<RoleViewModel>> AddRole(AddRoleViewModel addRole, int UserId)
        {
            try
            {
                var connectionString = _configuration["ConnectionStrings:ActiveConnection"];
                using (var connection = new OracleConnection(connectionString))
                {
                    await connection.OpenAsync();

                    using (var transaction = connection.BeginTransaction())
                    {
                        // Check if role already exists
                        var checkRoleCommand = new OracleCommand("SELECT COUNT(*) FROM \"TLIrole\" WHERE LOWER(\"Name\") = :name AND \"Deleted\" = 0", connection);
                        checkRoleCommand.Parameters.Add(new OracleParameter("name", OracleDbType.Varchar2) { Value = addRole.Name.ToLower() });
                        checkRoleCommand.Transaction = transaction;  // Assign transaction to command

                        var roleExists = Convert.ToInt32(await checkRoleCommand.ExecuteScalarAsync());

                        if (roleExists > 0)
                        {
                            return new Response<RoleViewModel>(true, null, null, $"Role {addRole.Name} already exists in the database", (int)Constants.ApiReturnCode.fail);
                        }

                        // Insert new role
                        var insertRoleCommand = new OracleCommand("INSERT INTO \"TLIrole\" (\"Name\", \"Active\", \"Deleted\", \"Permissions\") VALUES (:name, :active, :deleted, :permissions) RETURNING \"Id\" INTO :id", connection);
                        insertRoleCommand.Parameters.Add(new OracleParameter("name", OracleDbType.Varchar2) { Value = addRole.Name });
                        insertRoleCommand.Parameters.Add(new OracleParameter("active", OracleDbType.Int32) { Value = 1 }); // Active = true
                        insertRoleCommand.Parameters.Add(new OracleParameter("deleted", OracleDbType.Int32) { Value = 0 }); // Deleted = false
                        insertRoleCommand.Parameters.Add(new OracleParameter("permissions", OracleDbType.Varchar2) { Value = addRole.Permissions });

                        var idParameter = new OracleParameter("id", OracleDbType.Int32) { Direction = System.Data.ParameterDirection.Output };
                        insertRoleCommand.Parameters.Add(idParameter);
                        insertRoleCommand.Transaction = transaction;  // Assign transaction to command

                        await insertRoleCommand.ExecuteNonQueryAsync();

                        // Optionally, handle the role permissions and other related data as per your logic.

                        // Commit transaction
                        transaction.Commit();

                        return new Response<RoleViewModel>(false, null, null, "Role has been successfully created", (int)Helpers.Constants.ApiReturnCode.success);
                    }
                }
            }
            catch (Exception err)
            {
                return new Response<RoleViewModel>(false, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
        }



        private async Task InsertPermissionsAsync(OracleConnection connection, int roleId, List<string> permissions)
        {
            var query = @"
            INSERT INTO ""TLIrole_Permissions"" (""RoleId"", ""PageUrl"", ""Active"", ""Delete"")
            VALUES (:pRoleId, :pPageUrl, :pActive, :pDelete)";

            using (var command = new OracleCommand(query, connection))
            {
                command.Parameters.Add(new OracleParameter("pRoleId", OracleDbType.Int32)).Value = roleId;
                var pageUrlParam = new OracleParameter("pPageUrl", OracleDbType.Varchar2);
                command.Parameters.Add(pageUrlParam);
                command.Parameters.Add(new OracleParameter("pActive", OracleDbType.Int32)).Value = 1; // true
                command.Parameters.Add(new OracleParameter("pDeleted", OracleDbType.Int32)).Value = 0; // false

                foreach (var permission in permissions)
                {
                    pageUrlParam.Value = permission;
                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        //Function check if there are groups take role 
        //Function return true or false
        public Response<bool> CheckRoleGroups(int RoleId)
        {
            try
            {
                var RoleGroups = _unitOfWork.GroupRoleRepository

                  .GetWhere(x => x.roleId == RoleId)
                  .ToList();
                if (RoleGroups.Count == 0)
                {
                    return new Response<bool>(true, false, null, null, (int)Helpers.Constants.ApiReturnCode.success);
                }
                return new Response<bool>(true, true, null, null, (int)Helpers.Constants.ApiReturnCode.success);
            }
            catch (Exception err)
            {

                return new Response<bool>(true, false, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
        }
        //Function check role name in database 
        //Function return true or false
        public bool CheckRoleNameInDatabaseAdd(string RoleName)
        {
            return _unitOfWork.RoleRepository.CheckRoleNameInDatabaseAdd(RoleName);
        }
        //Function check role name in database 
        //Function return true or false
        public bool CheckRoleNameInDatabaseUpdate(string RoleName, int RoleId)
        {
            return _unitOfWork.RoleRepository.CheckRoleNameInDatabaseUpdate(RoleName, RoleId);
        }
        //Function take 1 parameter
        //check if there are groups take that role
        //if not then delete the role
        //else delete all groupsrole for this role then delete role
        public Response<RoleViewModel> DeleteRole(int RoleId,int UserId)
        {
            try
            {
                using (TransactionScope transaction = new TransactionScope())
                {
                    var RoleGroups = _unitOfWork.GroupRoleRepository.GetWhere(x => x.roleId == RoleId).ToList();
                    if (RoleGroups.Count == 0)
                    {
                        var RoleName = _unitOfWork.RoleRepository.GetWhereFirst(x => x.Id == RoleId);
                        if (RoleName != null)
                        {
                            RoleName.Name = RoleName.Name + DateTime.Now;
                            _unitOfWork.RoleRepository.DeleteRole(RoleId);

                            var TabelNameRole = _unitOfWork.TablesNamesRepository.GetWhereFirst(x => x.TableName == "TLIrole").Id;
                            TLIhistory AddTablesHistory = new TLIhistory
                            {
                                HistoryTypeId = 3,
                                RecordId = RoleId.ToString(),
                                TablesNameId = TabelNameRole,
                                UserId = UserId
                            };

                            _dbContext.TLIhistory.Add(AddTablesHistory);
                            _dbContext.SaveChanges();
                        }
                        else
                        {
                            transaction.Complete();
                            return new Response<RoleViewModel>(true, null, null, "This Role Is Not Found", (int)Constants.ApiReturnCode.fail);
                        }
                        transaction.Complete();
                        return new Response<RoleViewModel>(true, null, null, null, (int)Constants.ApiReturnCode.success);
                    }
                    else
                    {
                        DeleteRoleGroups(RoleId);
                        transaction.Complete();
                        return new Response<RoleViewModel>(true, null, null, null, (int)Constants.ApiReturnCode.success);
                    }
                    
                }
            }
            catch (Exception err)
            {

                return new Response<RoleViewModel>(true, null, null, err.Message, (int)Constants.ApiReturnCode.fail);
            }
        }
        //delete groupsrole for specific role
        //delete role
        public Response<RoleViewModel> DeleteRoleGroups(int RoleId)
        {
            try
            {
                var RoleGroups = _unitOfWork.GroupRoleRepository.GetWhere(x => x.roleId == RoleId).ToList();
                foreach (var item in RoleGroups)
                {
                    item.Deleted = true;
                }
                // _unitOfWork.GroupRoleRepository.RemoveRangeItems(RoleGroups);
                _unitOfWork.SaveChanges();
                var Role = _unitOfWork.RoleRepository.GetByID(RoleId);
                Role.Name = Role.Name + DateTime.Now;
                Role.Deleted = true;
                // _unitOfWork.RoleRepository.RemoveItem(Role);
                _unitOfWork.SaveChanges();
                return new Response<RoleViewModel>();
            }
            catch (Exception err)
            {

                return new Response<RoleViewModel>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
        }
        //Function 1 parameter
        //Function check the name in database
        //if the name is already exists return error message 
        //get record by Id
        //update Entity
        //update permissions
        public async Task<Response<RoleViewModel>> EditRole(EditRoleViewModel editRole,int UserId)
        {
            var connectionString = _configuration["ConnectionStrings:ActiveConnection"];
            using (var connection = new OracleConnection(connectionString))
            {
                await connection.OpenAsync();

                using (var transaction = connection.BeginTransaction())
                {
                    var OldRole = _unitOfWork.RoleRepository.GetAllAsQueryable().AsNoTracking().FirstOrDefault(x => x.Id == editRole.Id);
                    if(OldRole==null)
                        return new Response<RoleViewModel>(true, null, null, "this role is not found", (int)Constants.ApiReturnCode.success);

                    TLIrole CheckRoleNameIfExist = _unitOfWork.RoleRepository
                   .GetWhereFirst(x => !x.Deleted && x.Name.ToLower() == editRole.Name.ToLower() && x.Id != editRole.Id);

                    if (CheckRoleNameIfExist != null)
                    {
                        return new Response<RoleViewModel>(true, null, null, $"Role {editRole.Name} is already exists", (int)Constants.ApiReturnCode.fail);
                    }

                    TLIrole RoleEntity = _unitOfWork.RoleRepository.GetByID(editRole.Id);
                    if (RoleEntity != null)
                    {
                        RoleEntity.Name = editRole.Name;
                        RoleEntity.Permissions = editRole.Permissions;

                        _unitOfWork.RoleRepository.Update(RoleEntity);
                        await _unitOfWork.SaveChangesAsync();


                        transaction.Commit();
                        return new Response<RoleViewModel>(true, null, null, null, (int)Constants.ApiReturnCode.success);
                    }
                    else
                    {
             
                        transaction.Rollback();
                        return new Response<RoleViewModel>(false, null, null, "This Role Is Not Found", (int)Helpers.Constants.ApiReturnCode.fail, 0);
                    }
                    
                    
                }
            }
        }
        //Function take 1 parameter
        //Function return all roles depened on filters
        public async Task<Response<IEnumerable<RoleViewModel>>> GetRoles(List<FilterObjectList> filters)
        {
            try
            {
                List<RoleViewModel> RolesModel = new List<RoleViewModel>();
                if (filters != null ? filters.Count() > 0 : false)
                    RolesModel = _mapper.Map<List<RoleViewModel>>(_unitOfWork.RoleRepository.GetWhere(x => x.Deleted != true && x.Name.ToLower()
                        .StartsWith(filters.FirstOrDefault().value.FirstOrDefault().ToString().ToLower())).OrderBy(x => x.Name).ToList());
                else
                    RolesModel = _mapper.Map<List<RoleViewModel>>(_unitOfWork.RoleRepository.GetWhere(x => x.Deleted != true).OrderBy(x => x.Name).ToList());

                return new Response<IEnumerable<RoleViewModel>>(true, RolesModel, null, null, (int)Helpers.Constants.ApiReturnCode.success);
            }
            catch (Exception err)
            {
                return new Response<IEnumerable<RoleViewModel>>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
        }


        public Response<List<string>> GetOldPermissionRoleById(int Id)
        {
            try
            {
                 List<string> newPermissionsViewModels = new List<string>();
                string newPermissionsViewModel = null;
               
                
                var Roles = _unitOfWork.RoleRepository.GetWhere(x => x.Id== Id && !x.Deleted && x.Active);
                if (Roles.Count() > 0)
                {
                    foreach (var item in Roles)
                    {
                        int RoleId = _unitOfWork.RoleRepository.GetWhereFirst(x => x.Name == item.Name).Id;
                        List<TLIrole_Permissions> Permissions = _unitOfWork.RolePermissionsRepository.GetWhere(x => x.RoleId == item.Id).ToList();
                        foreach (var itemPermissions in Permissions)
                        {
                            newPermissionsViewModel = itemPermissions.PageUrl;
                            newPermissionsViewModels.Add(newPermissionsViewModel);

                        }
                  
                    }


                    return new Response<List<string>>(true, newPermissionsViewModels, null, null, (int)Helpers.Constants.ApiReturnCode.success);
                }
                else
                {
                    return new Response<List<string>>(true, null, null, "This Role Is Not Exist", (int)Helpers.Constants.ApiReturnCode.fail); ;
                }
                

            }

            catch (Exception err)
            {
                return new Response<List<string>>(false, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }

        }
        public async Task<Response<IEnumerable<RoleViewModel>>> GetRolesFor_WF()
        {
            try
            {
                List<RoleViewModel> RolesModel = new List<RoleViewModel>();
                var res = _unitOfWork.RoleRepository.GetAllWithoutCount();
                RolesModel = _mapper.Map<List<RoleViewModel>>(res);
                return new Response<IEnumerable<RoleViewModel>>(true, RolesModel, null, null, (int)Helpers.Constants.ApiReturnCode.success);
            }
            catch (Exception err)
            {
                return new Response<IEnumerable<RoleViewModel>>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
        }
        public Response<List<RoleViewModel>> GetRoleByName(string RoleName)
        {
            try
            {
                if (String.IsNullOrEmpty(RoleName))
                {
                    List<RoleViewModel> RoleModel = _mapper.Map<List<RoleViewModel>>(_unitOfWork.RoleRepository.GetWhere(x => x.Active && !x.Deleted));
                    return new Response<List<RoleViewModel>>(true, RoleModel, null, null, (int)Helpers.Constants.ApiReturnCode.success, RoleModel.Count);
                }
                else
                {
                    List<RoleViewModel> rolemodel = _mapper.Map<List<RoleViewModel>>(_unitOfWork.RoleRepository.GetWhere(x =>
                       x.Name.ToLower().StartsWith(RoleName.ToLower()) && x.Active && !x.Deleted).ToList());
                    return new Response<List<RoleViewModel>>(true, rolemodel, null, null, (int)Helpers.Constants.ApiReturnCode.success, rolemodel.Count);
                }
            }
            catch (Exception err)
            {
                return new Response<List<RoleViewModel>>(false, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
        }
        public Response<List<RoleViewModel>> GetRoleByRoleName(string RoleName)
        {
            try 
            {
                List<RoleViewModel > Response = new List<RoleViewModel>();
                RoleViewModel roleViewModel = new RoleViewModel();
                List<string> newPermissionsViewModels = new List<string>();
                string newPermissionsViewModel = null;
                if (RoleName == null)
                {
                    Response = _mapper.Map<List<RoleViewModel>>(_unitOfWork.RoleRepository.GetWhere(x=>!x.Deleted && x.Active).ToList());
                    return new Response<List<RoleViewModel>>(true, Response, null, null, (int)Helpers.Constants.ApiReturnCode.success);
                }
                else
                {
                    var Roles = _unitOfWork.RoleRepository.GetWhere(x => x.Name.ToLower().Contains(RoleName.ToLower()) && !x.Deleted && x.Active);
                    if (Roles.Count() > 0)
                    {
                        foreach (var item in Roles)
                        {
                            int RoleId = _unitOfWork.RoleRepository.GetWhereFirst(x => x.Name == item.Name).Id;
                            List<TLIrole_Permissions> Permissions = _unitOfWork.RolePermissionsRepository.GetWhere(x => x.RoleId == item.Id).ToList();
                            foreach (var itemPermissions in Permissions)
                            {
                                newPermissionsViewModel = itemPermissions.PageUrl;
                                newPermissionsViewModels.Add(newPermissionsViewModel);

                            }
                            roleViewModel.Id = item.Id;
                            roleViewModel.Name = item.Name;
                            roleViewModel.Active = item.Active;
                            roleViewModel.Deleted = item.Deleted;
                            roleViewModel.Permissions = item.Permissions;
                            Response.Add(roleViewModel);
                        }


                        return new Response<List<RoleViewModel>>(true, Response, null, null, (int)Helpers.Constants.ApiReturnCode.success);
                    }
                    else
                    {
                        return new Response<List<RoleViewModel>>(true, null, null, "The Name Is Not Exist", (int)Helpers.Constants.ApiReturnCode.fail); ;
                    }
                }
               
            }
            
            catch (Exception err)
            {
                return new Response<List<RoleViewModel>>(false, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }

        }
        public Response<List<RoleViewModel>> GetRoleByRoleId(int RoleId)
        {
            try
            {
        
             
                var Roles = _unitOfWork.RoleRepository.GetWhere(x => x.Id==RoleId && !x.Deleted && x.Active);
                if (Roles.Count() > 0)
                {
                    var Response=_mapper.Map<List<RoleViewModel>>(Roles);
                    return new Response<List<RoleViewModel>>(true, Response, null, null, (int)Helpers.Constants.ApiReturnCode.success);
                }
                else
                {
                    return new Response<List<RoleViewModel>>(true, null, null, "The Name Is Not Exist", (int)Helpers.Constants.ApiReturnCode.fail); ;
                }
                

            }

            catch (Exception err)
            {
                return new Response<List<RoleViewModel>>(false, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }

        }
    }
}
