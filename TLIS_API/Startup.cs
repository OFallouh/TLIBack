using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TLIS_DAL;
using TLIS_Repository.Base;
using TLIS_Service.ServiceBase;
using AutoMapper;
using TLIS_DAL.ViewModels;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using TLIS_Service.Helpers;
using TLIS_Repository.Repositories;
using NLog.Config;
using System.Threading.Tasks;
using TLIS_Service.IService;
using TLIS_Service.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Net;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Security.Principal;
using System.DirectoryServices.AccountManagement;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModels.UserDTOs;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Internal;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using NLog.LayoutRenderers;
using NLog;
using Microsoft.AspNetCore.Mvc.Filters;
using TLIS_API.Middleware;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using NLog.Web;
using TLIS_API.Middleware.ActionFilters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using OfficeOpenXml.ConditionalFormatting;
using TLIS_DAL.ViewModels.SiteDTOs;

namespace TLIS_API
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public IUnitOfWork _unitOfWork { get; set; }
        public IUnitOfWorkService _unitOfWorkService { get; set; }
        public ApplicationDbContext _DbContext { get; set; }
        public IMapper _Mapper { get; set; }
        public IHostEnvironment HostingEnvironment { get; private set; }
        public Startup(IConfiguration configuration, IHostEnvironment env)
        {
            this.HostingEnvironment = env;
            this.Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddSingleton(Configuration);

            //services.AddDbContext<ApplicationDbContext>(options =>
            //{
            //    options.UseOracle(Configuration["ConnectionStrings:ActiveConnection"], b => b.MigrationsAssembly("TLIS_API"));
            //    options.EnableSensitiveDataLogging();
            //});


            //add connection with oracle database
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseOracle(Configuration["ConnectionStrings:ActiveConnection"]);
            });

            var domainSettingsSection = Configuration.GetSection("Domain");

            services.Configure<string>(domainSettingsSection);

            /*------------------------------------------------------------------------------------*/
            /*                              Custom Logging Layout                                 */
            /*------------------------------------------------------------------------------------*/
            ConfigurationItemFactory.Default.LayoutRenderers
                    .RegisterDefinition("Custom-Layout", typeof(CustomlayoutRenderer));
            /*------------------------------------------------------------------------------------*/
            /*                              Used In Windows Authentication                        */
            /*------------------------------------------------------------------------------------*/
            var serviceProvider = services.BuildServiceProvider();

            services.AddHttpContextAccessor();

            //_unitOfWorkService = serviceProvider.GetRequiredService<IUnitOfWorkService>();
            _DbContext = serviceProvider.GetService<ApplicationDbContext>();

            _unitOfWork = new UnitOfWork(_DbContext, _Mapper);
            _unitOfWorkService = new UnitOfWorkService(_unitOfWork, Configuration);
            /*------------------------------------------------------------------------------------*/

            services.AddSingleton(services);

            services.AddSingleton<LogRepository>();
            services.AddSingleton<DbContextOptionsBuilder>();
            services.AddHttpContextAccessor();

            services.AddSwaggerGen();
            // Add cors
            services.AddCors();
            services.AddHttpContextAccessor();
            services.AddScoped<IUnitOfWorkService, UnitOfWorkService>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped<LogFilterAttribute>();
            services.AddScoped<ExternalSystemFilter>();
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = Configuration["JWT:Issuer"],
                        ValidAudience = Configuration["JWT:Issuer"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JWT:Key"]))
                    };
                });
            services.AddMvc();
            services.AddDistributedMemoryCache();
            services.AddSession(options =>
            {
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            //-----------------------------------------Add Auto Mapper


            //_Mapper = serviceProvider.GetService<Mapper>();
            // services.AddSingleton<Mapper>();

            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            //var config = new MapperConfiguration(cfg => {
            //    //cfg.AddProfile<AutoMapperProfile>();
            //    cfg.AddProfile(new AutoMapperProfile());
            //});
            //IMapper mapper = config.CreateMapper();
            //services.AddSingleton(mapper);
            //services.AddAutoMapper(typeof(Startup));                     
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseCors(x => x
           .AllowAnyOrigin()
           .AllowAnyMethod()
           .AllowAnyHeader());

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "TLIS Api Docs");
                c.DisplayRequestDuration();
            });
            app.UseSession();

            app.UseAuthentication();

            app.UseRouting();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });

            //app.UseMvc(routes =>
            //{
            //    routes.MapRoute(
            //        name: "default",
            //        template: "{controller}/{action=Index}/{id?}");
            //});

            #region Windows Authentication
            /*------------------------------------------------------------------------------------*/
            /*                              Windows Authentication                                */
            /*------------------------------------------------------------------------------------*/
            app.Use(async (Runcontext, next) =>
            {
                ApplicationDbContext _Context = Runcontext.RequestServices.GetService<ApplicationDbContext>();
                IMapper _Mapper = Runcontext.RequestServices.GetService<IMapper>();

                //
                // Sites..
                //

                SiteService._MySites = await _Context.TLIsite.Include(x => x.Area).Include(x => x.Region)
                    .Include(x => x.siteStatus).ToListAsync();

                //
                // General Lists..
                //

                UnitOfWork.AllAttributeViewManagment = await _Context.TLIattributeViewManagment
                    .Include(x => x.AttributeActivated)
                    .Include(x => x.DynamicAtt)
                    .Include(x => x.DynamicAtt.CivilWithoutLegCategory)
                    .Include(x => x.DynamicAtt.DataType)
                    .Include(x => x.DynamicAtt.tablesNames)
                    .Include(x => x.EditableManagmentView)
                    .Include(x => x.EditableManagmentView.TLItablesNames1)
                    .ToListAsync();

                UnitOfWork.AllAttributeActivated = await _Context.TLIattributeActivated.ToListAsync();
                UnitOfWork.AllAttributeActivatedCategory = await _Context.TLIattActivatedCategory
                    .Include(x => x.attributeActivated).Include(x => x.civilWithoutLegCategory).ToListAsync();

                UnitOfWork.AllDynamicAttribute = await _Context.TLIdynamicAtt
                    .Include(x => x.CivilWithoutLegCategory).Include(x => x.DataType)
                    .Include(x => x.tablesNames).ToListAsync();

                UnitOfWork.AllDynamicAttInstValue = await _Context.TLIdynamicAttInstValue
                    .Include(x => x.DynamicAtt)
                    .Include(x => x.sideArm)
                    .Include(x => x.tablesNames)
                    .Where(x => UnitOfWork.AllDynamicAttribute
                        .Select(y => y.Id).Contains(x.DynamicAttId)).ToListAsync();

                UnitOfWork.AllDynamicAttLibValue = await _Context.TLIdynamicAttLibValue
                    .Include(x => x.DynamicAtt)
                    .Include(x => x.tablesNames)
                    .Where(x => UnitOfWork.AllDynamicAttribute
                        .Select(y => y.Id).Contains(x.DynamicAttId)).ToListAsync();

                //
                // Civil With Leg Library..
                //

                CivilLibraryService._CivilWithLegLibraryAttributeActivated = UnitOfWork.AllAttributeViewManagment.Where(x =>
                    x.Enable && x.AttributeActivatedId != null &&
                    x.AttributeActivated.DataType.ToLower() != "datetime" &&
                    x.EditableManagmentView.View == Constants.EditableManamgmantViewNames.CivilWithLegsLibrary.ToString() &&
                    x.EditableManagmentView.TLItablesNames1.TableName == Helpers.Constants.TablesNames.TLIcivilWithLegLibrary.ToString())
                .Select(x => x.AttributeActivated).ToList();

                CivilLibraryService._CivilWithLegLibraryDynamicAttributes = UnitOfWork.AllDynamicAttribute.Where(x =>
                    x.LibraryAtt && !x.disable &&
                    x.tablesNames.TableName == Helpers.Constants.TablesNames.TLIcivilWithLegLibrary.ToString()).ToList();

                CivilLibraryService._CivilWithLegLibraryEntities = await _Context.TLIcivilWithLegLibrary
                    .Include(x => x.civilSteelSupportCategory)
                    .Include(x => x.sectionsLegType)
                    .Include(x => x.structureType)
                    .Include(x => x.supportTypeDesigned)
                    .Where(x => !x.Deleted).ToListAsync();

                CivilLibraryService._CivilWithLegLibraryAllAttributesViewManagement = UnitOfWork.AllAttributeViewManagment.Where(x =>
                   (x.Enable && x.EditableManagmentView.View == Constants.EditableManamgmantViewNames.CivilWithLegsLibrary.ToString() &&
                   (x.AttributeActivatedId != null ?
                        (x.AttributeActivated.Tabel == Helpers.Constants.TablesNames.TLIcivilWithLegLibrary.ToString() && x.AttributeActivated.enable) :
                        (x.DynamicAtt.LibraryAtt && !x.DynamicAtt.disable && x.DynamicAtt.tablesNames.TableName == Helpers.Constants.TablesNames.TLIcivilWithLegLibrary.ToString()))) ||
                    (x.AttributeActivated != null ?
                        ((x.AttributeActivated.Key.ToLower() == "id" || x.AttributeActivated.Key.ToLower() == "active") && x.AttributeActivated.Tabel == Helpers.Constants.TablesNames.TLIcivilWithLegLibrary.ToString()) : false)).ToList();

                //
                // Civil NON Steel Library..
                //

                CivilLibraryService._CivilNonSteelLibraryAttributeActivated = UnitOfWork.AllAttributeViewManagment.Where(x =>
                        x.Enable && x.AttributeActivatedId != null &&
                        x.AttributeActivated.DataType.ToLower() != "datetime" &&
                        x.EditableManagmentView.View == Constants.EditableManamgmantViewNames.CivilNonSteelLibrary.ToString() &&
                        x.EditableManagmentView.TLItablesNames1.TableName == Helpers.Constants.TablesNames.TLIcivilNonSteelLibrary.ToString())
                    .Select(x => x.AttributeActivated).ToList();

                CivilLibraryService._CivilNonSteelLibraryDynamicAttributes = UnitOfWork.AllDynamicAttribute.Where(x =>
                    x.LibraryAtt && !x.disable &&
                    x.tablesNames.TableName == Helpers.Constants.TablesNames.TLIcivilNonSteelLibrary.ToString()).ToList();

                CivilLibraryService._CivilNonSteelLibraryEntities = await _Context.TLIcivilNonSteelLibrary
                    .Include(x => x.civilNonSteelType).Where(x => !x.Deleted).ToListAsync();

                CivilLibraryService._CivilNonSteelLibraryAllAttributesViewManagement = UnitOfWork.AllAttributeViewManagment.Where(x =>
                   (x.Enable && x.EditableManagmentView.View == Constants.EditableManamgmantViewNames.CivilNonSteelLibrary.ToString() &&
                   (x.AttributeActivatedId != null ?
                        (x.AttributeActivated.Tabel == Helpers.Constants.TablesNames.TLIcivilNonSteelLibrary.ToString() && x.AttributeActivated.enable) :
                        (x.DynamicAtt.LibraryAtt && !x.DynamicAtt.disable && x.DynamicAtt.tablesNames.TableName == Helpers.Constants.TablesNames.TLIcivilNonSteelLibrary.ToString()))) ||
                    (x.AttributeActivated != null ?
                        ((x.AttributeActivated.Key.ToLower() == "id" || x.AttributeActivated.Key.ToLower() == "active") && x.AttributeActivated.Tabel == Helpers.Constants.TablesNames.TLIcivilNonSteelLibrary.ToString()) : false)).ToList();

                //
                // Civil Without Leg Library..
                //

                CivilLibraryService._CivilWithoutLegLibraryAttributeActivatedCategory = await _Context.TLIattActivatedCategory
                    .Include(x => x.attributeActivated).ToListAsync();

                CivilLibraryService._CivilWithoutLegLibraryAttributeViewManagement = UnitOfWork.AllAttributeViewManagment.Where(x =>
                    x.Enable && x.AttributeActivatedId != null &&
                    x.EditableManagmentView.TLItablesNames1.TableName == Helpers.Constants.TablesNames.TLIcivilWithoutLegLibrary.ToString()).ToList();

                CivilLibraryService._CivilWithoutLegLibraryDynamicAttributes = UnitOfWork.AllDynamicAttribute.Where(x =>
                    x.LibraryAtt && !x.disable &&
                    x.tablesNames.TableName == Helpers.Constants.TablesNames.TLIcivilWithoutLegLibrary.ToString()).ToList();

                CivilLibraryService._CivilWithoutLegLibraryEntities = await _Context.TLIcivilWithoutLegLibrary
                    .Where(x => !x.Deleted)
                    .Include(x => x.CivilSteelSupportCategory)
                    .Include(x => x.CivilWithoutLegCategory)
                    .Include(x => x.InstallationCivilwithoutLegsType)
                    .Include(x => x.structureType).ToListAsync();

                //
                // Cabinet Power Library..
                //

                OtherInventoryLibraryService._CabinetPowerLibraryAttributeActivatedCategory = UnitOfWork.AllAttributeViewManagment.Where(x =>
                        x.Enable && x.AttributeActivatedId != null &&
                        x.AttributeActivated.DataType.ToLower() != "datetime" &&
                        x.EditableManagmentView.View == Constants.EditableManamgmantViewNames.CabinetPowerLibrary.ToString() &&
                        x.EditableManagmentView.TLItablesNames1.TableName == Helpers.Constants.TablesNames.TLIcabinetPowerLibrary.ToString())
                    .Select(x => x.AttributeActivated).ToList();

                OtherInventoryLibraryService._CabinetPowerLibraryDynamicAttributes = UnitOfWork.AllDynamicAttribute.Where(x => 
                    x.LibraryAtt && !x.disable &&
                    x.tablesNames.TableName == Constants.TablesNames.TLIcabinetPowerLibrary.ToString()).ToList();

                OtherInventoryLibraryService._CabinetPowerLibraryEntities = await _Context.TLIcabinetPowerLibrary
                    .Where(x => !x.Deleted)
                    .Include(x => x.CabinetPowerType).ToListAsync();

                OtherInventoryLibraryService._CabinetPowerLibraryAllAttributesViewManagement = UnitOfWork.AllAttributeViewManagment.Where(x =>
                   (x.Enable && x.EditableManagmentView.View == Constants.EditableManamgmantViewNames.CabinetPowerLibrary.ToString() &&
                   (x.AttributeActivatedId != null ?
                        (x.AttributeActivated.Tabel == Helpers.Constants.TablesNames.TLIcabinetPowerLibrary.ToString() && x.AttributeActivated.enable) :
                        (x.DynamicAtt.LibraryAtt && !x.DynamicAtt.disable && x.DynamicAtt.tablesNames.TableName == Helpers.Constants.TablesNames.TLIcabinetPowerLibrary.ToString()))) ||
                    (x.AttributeActivated != null ?
                        ((x.AttributeActivated.Key.ToLower() == "id" || x.AttributeActivated.Key.ToLower() == "active") && 
                        x.AttributeActivated.Tabel == Helpers.Constants.TablesNames.TLIcabinetPowerLibrary.ToString()) : false)).ToList();

                //
                // Cabinet Telecom Library..
                //

                OtherInventoryLibraryService._CabinetTelecomLibraryAttributeActivatedCategory = UnitOfWork.AllAttributeViewManagment.Where(x =>
                        x.Enable && x.AttributeActivatedId != null &&
                        x.AttributeActivated.DataType.ToLower() != "datetime" &&
                        x.EditableManagmentView.View == Constants.EditableManamgmantViewNames.CabinetTelecomLibrary.ToString() &&
                        x.EditableManagmentView.TLItablesNames1.TableName == Helpers.Constants.TablesNames.TLIcabinetTelecomLibrary.ToString())
                    .Select(x => x.AttributeActivated).ToList();

                OtherInventoryLibraryService._CabinetTelecomLibraryDynamicAttributes = UnitOfWork.AllDynamicAttribute.Where(x =>
                    x.LibraryAtt && !x.disable &&
                    x.tablesNames.TableName == Constants.TablesNames.TLIcabinetTelecomLibrary.ToString()).ToList();

                OtherInventoryLibraryService._CabinetTelecomLibraryEntities = await _Context.TLIcabinetTelecomLibrary
                    .Where(x => !x.Deleted)
                    .Include(x => x.TelecomType).ToListAsync();

                OtherInventoryLibraryService._CabinetTelecomLibraryAllAttributesViewManagement = UnitOfWork.AllAttributeViewManagment.Where(x =>
                   (x.Enable && x.EditableManagmentView.View == Constants.EditableManamgmantViewNames.CabinetTelecomLibrary.ToString() &&
                   (x.AttributeActivatedId != null ?
                        (x.AttributeActivated.Tabel == Helpers.Constants.TablesNames.TLIcabinetTelecomLibrary.ToString() && x.AttributeActivated.enable) :
                        (x.DynamicAtt.LibraryAtt && !x.DynamicAtt.disable && x.DynamicAtt.tablesNames.TableName == Helpers.Constants.TablesNames.TLIcabinetTelecomLibrary.ToString()))) ||
                    (x.AttributeActivated != null ?
                        ((x.AttributeActivated.Key.ToLower() == "id" || x.AttributeActivated.Key.ToLower() == "active") && 
                        x.AttributeActivated.Tabel == Helpers.Constants.TablesNames.TLIcabinetTelecomLibrary.ToString()) : false)).ToList();

                //
                // Solar Library..
                //

                OtherInventoryLibraryService._SolarLibraryAttributeActivatedCategory = UnitOfWork.AllAttributeViewManagment.Where(x =>
                    x.Enable && x.AttributeActivatedId != null &&
                    x.AttributeActivated.DataType.ToLower() != "datetime" &&
                    x.EditableManagmentView.View == Constants.EditableManamgmantViewNames.SolarLibrary.ToString() &&
                    x.EditableManagmentView.TLItablesNames1.TableName == Helpers.Constants.TablesNames.TLIsolarLibrary.ToString())
                .Select(x => x.AttributeActivated).ToList();

                OtherInventoryLibraryService._SolarLibraryDynamicAttributes = UnitOfWork.AllDynamicAttribute.Where(x =>
                    x.LibraryAtt && !x.disable &&
                    x.tablesNames.TableName == Constants.TablesNames.TLIsolarLibrary.ToString()).ToList();

                OtherInventoryLibraryService._SolarLibraryEntities = await _Context.TLIsolarLibrary
                    .Where(x => !x.Deleted)
                    .Include(x => x.Capacity).ToListAsync();

                OtherInventoryLibraryService._SolarLibraryAllAttributesViewManagement = UnitOfWork.AllAttributeViewManagment.Where(x =>
                   (x.Enable && x.EditableManagmentView.View == Constants.EditableManamgmantViewNames.SolarLibrary.ToString() &&
                   (x.AttributeActivatedId != null ?
                        (x.AttributeActivated.Tabel == Helpers.Constants.TablesNames.TLIsolarLibrary.ToString() && x.AttributeActivated.enable) :
                        (x.DynamicAtt.LibraryAtt && !x.DynamicAtt.disable && x.DynamicAtt.tablesNames.TableName == Helpers.Constants.TablesNames.TLIsolarLibrary.ToString()))) ||
                    (x.AttributeActivated != null ?
                        ((x.AttributeActivated.Key.ToLower() == "id" || x.AttributeActivated.Key.ToLower() == "active") && x.AttributeActivated.Tabel == Helpers.Constants.TablesNames.TLIsolarLibrary.ToString()) : false)).ToList();

                //
                // Generator Library..
                //

                OtherInventoryLibraryService._GeneratorLibraryAttributeActivatedCategory = UnitOfWork.AllAttributeViewManagment.Where(x =>
                    x.Enable && x.AttributeActivatedId != null &&
                    x.AttributeActivated.DataType.ToLower() != "datetime" &&
                    x.EditableManagmentView.View == Constants.EditableManamgmantViewNames.GeneratorLibrary.ToString() &&
                    x.EditableManagmentView.TLItablesNames1.TableName == Helpers.Constants.TablesNames.TLIgeneratorLibrary.ToString())
                .Select(x => x.AttributeActivated).ToList();

                OtherInventoryLibraryService._GeneratorLibraryDynamicAttributes = UnitOfWork.AllDynamicAttribute.Where(x =>
                    x.LibraryAtt && !x.disable &&
                    x.tablesNames.TableName == Constants.TablesNames.TLIgeneratorLibrary.ToString()).ToList();

                OtherInventoryLibraryService._GeneratorLibraryEntities = await _Context.TLIgeneratorLibrary
                    .Where(x => !x.Deleted)
                    .Include(x => x.Capacity).ToListAsync();

                OtherInventoryLibraryService._GeneratorLibraryAllAttributesViewManagement = UnitOfWork.AllAttributeViewManagment.Where(x =>
                   (x.Enable && x.EditableManagmentView.View == Constants.EditableManamgmantViewNames.GeneratorLibrary.ToString() &&
                   (x.AttributeActivatedId != null ?
                        (x.AttributeActivated.Tabel == Helpers.Constants.TablesNames.TLIgeneratorLibrary.ToString() && x.AttributeActivated.enable) :
                        (x.DynamicAtt.LibraryAtt && !x.DynamicAtt.disable && x.DynamicAtt.tablesNames.TableName == Helpers.Constants.TablesNames.TLIgeneratorLibrary.ToString()))) ||
                    (x.AttributeActivated != null ?
                        ((x.AttributeActivated.Key.ToLower() == "id" || x.AttributeActivated.Key.ToLower() == "active") && x.AttributeActivated.Tabel == Helpers.Constants.TablesNames.TLIgeneratorLibrary.ToString()) : false)).ToList();

                await next();
            });
            //app.Use(async (Runcontext, next) =>
            //{
            //    ApplicationDbContext _context = Runcontext.RequestServices.GetService<ApplicationDbContext>();
            //    string requestbody = Runcontext.Request.Path.ToString();
            //    string[] test = requestbody.Split("/");
            //    IHeaderDictionary requestHeader = Runcontext.Request.Headers;
            //    // If The Request Is Sent With Api Then Check Authentication...
            //    if (test[1] == "api")
            //    {
            //        // This Condition Means -->> If The User is Not Calling The (TokenController)...
            //        // OR If The Action Is Either (AddInternalUser Or AddExternalUser) Then -->> No Need To Check User's Token...
            //        // In Other Meaning -->> If The User Is Calling Any Controller Except The (TokenController)...
            //        if (test[2] != "Token")
            //        {
            //            string token = requestHeader["Authorization"].ToString();

            //            if (!String.IsNullOrEmpty(token))
            //            {
            //                // The Token Is Sent With The Request's Headers With A Header's Name [Authorization]...
            //                // And It is Splited Into Two Halves -->> 1. "Bearer" Word + 2. The Token Value...
            //                string[] tokenArray = token.Split(" ");
            //                string tokenValue = tokenArray[1];
            //                JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
            //                JwtSecurityToken tokenInstance = handler.ReadJwtToken(tokenValue);
            //                List<Claim> tokenClaims = tokenInstance.Claims.ToList();
            //                string FirstClaim = tokenClaims[0].Value;
            //                //requestHeader["Cookie"] = FirstClaim;
            //                //var Cookie = requestHeader["Cookie"].ToString();
            //                string UserId = "UserId";
            //                Runcontext.Session.SetString(UserId, FirstClaim);
            //                TLIpermission Permission = _context.TLIpermission.FirstOrDefault(p => p.ControllerName.Equals(test[2]) && p.ActionName.Equals(test[3]));
            //                bool TestAccess = false;
            //                if (Permission != null)
            //                {
            //                    TestAccess = _context.TLIuserPermission.Any(u => u.userId.Equals(Int32.Parse(FirstClaim)) && u.permissionId.Equals(Permission.Id));
            //                    if (TestAccess.Equals(false))
            //                    {
            //                        List<int> UsersGroups = _unitOfWork.GroupUserRepository.GetWhere(x => x.userId.Equals(Int32.Parse(FirstClaim))).Select(x => x.groupId).ToList();
            //                        List<int> GroupRoles = _unitOfWork.GroupRoleRepository.GetWhere(x => UsersGroups.Contains(x.groupId)).Select(x => x.roleId).ToList();
            //                        List<int> RolePermissions = _unitOfWork.RolePermissionRepository.GetWhere(x => GroupRoles.Contains(x.roleId)).Select(x => x.permissionId).ToList();
            //                        if (RolePermissions.Contains(Permission.Id))
            //                            TestAccess = true;
            //                    }
            //                }
            //                if (TestAccess.Equals(true))
            //                {
            //                    await next.Invoke();
            //                }
            //                else
            //                {
            //                    Runcontext.Response.Clear();
            //                    Runcontext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            //                    await Runcontext.Response.WriteAsync("Unauthorized");
            //                }
            //            }
            //        }
            //        //else
            //        //{
            //        //    string UserName = Environment.UserName;
            //        //    string domain = Configuration["Domain"];
            //        //    using (PrincipalContext context = new PrincipalContext(ContextType.Domain, domain, null, ContextOptions.SimpleBind))
            //        //    {
            //        //        UserPrincipal principal = new UserPrincipal(context);

            //        //        if (context != null)
            //        //        {
            //        //            principal = UserPrincipal.FindByIdentity(context, IdentityType.SamAccountName, UserName);
            //        //            if (principal != null)
            //        //            {
            //        //                // Check The UserName If It is Exist In The DataBase..
            //        //                TLIuser CheckUserIfExist = _unitOfWork.UserRepository.GetWhereFirst(x => x.UserName == UserName);
            //        //                if (CheckUserIfExist != null)
            //        //                {
            //        //                    string secretKey = Configuration["JWT:Key"];
            //        //                    string TokenValue = _unitOfWorkService.TokenService.CreateToken(
            //        //                        new LoginViewModel
            //        //                        {
            //        //                            UserName = UserName
            //        //                        }, secretKey, null, null).Data;

            //        //JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
            //        //JwtSecurityToken tokenInstance = handler.ReadJwtToken(TokenValue);
            //        //List<Claim> tokenClaims = tokenInstance.Claims.ToList();
            //        //string FirstClaim = tokenClaims[0].Value;
            //        //string UserId = "UserId";
            //        //Runcontext.Session.SetString(UserId, FirstClaim);
            //        //TLIpermission Permission = _context.TLIpermission.FirstOrDefault(p => p.ControllerName.Equals(test[2]) && p.ActionName.Equals(test[3]));
            //        //bool TestAccess = false;
            //        //if (Permission != null)
            //        //{
            //        //    TestAccess = _context.TLIuserPermission.Any(u => u.userId.Equals(Int32.Parse(FirstClaim)) && u.permissionId.Equals(Permission.Id));
            //        //}
            //        //if (TestAccess.Equals(true))
            //        //{
            //        //await next.Invoke();

            //        //string expected = JsonConvert.SerializeObject(new { Token = TokenValue });
            //        //await Runcontext.Response.WriteAsync(expected);
            //        //}
            //        //else
            //        //{
            //        //    Runcontext.Response.Clear();
            //        //    Runcontext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;

            //        //    string expected = JsonConvert.SerializeObject(new { UserState = "Unauthorized", Token = TokenValue });
            //        //    await Runcontext.Response.WriteAsync(expected);
            //        //}
            //        //  }
            //        //    else
            //        //    {
            //        //        Runcontext.Response.Clear();
            //        //        Runcontext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;

            //        //        string expected = JsonConvert.SerializeObject(new { UserState = "Unauthenticated" });
            //        //        await Runcontext.Response.WriteAsync(expected);
            //        //    }
            //        //}
            //        //else
            //        //{
            //        //    Runcontext.Response.Clear();
            //        //    Runcontext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;

            //        //    string expected = JsonConvert.SerializeObject(new { UserState = "Unauthorized" });
            //        //    await Runcontext.Response.WriteAsync(expected);
            //        //}
            //        // }
            //        // }
            //        // }
            //        // }

            //        // Else If (TokenController) Has Been Called -->> Go Straight To (TokenController)
            //        // No Need To Check User's Token...
            //        //else
            //        //{
            //        //    await next.Invoke();
            //        //}
            //    }

            //    //else
            //    //{
            //    //    await next.Invoke();
            //    //}
            //    // }
            //    // else If The Request Is Sent When The Browser Is Opened (No Api) Then Check Windows Authentication...
            //    //else
            //    //{
            //    //    // var identity = WindowsIdentity.GetCurrent();
            //    //    var windowsIdentity = _httpContextAccessor.HttpContext.User.Identity as WindowsIdentity;
            //    //    // Get the username from the Windows identity
            //    //    var UserName = windowsIdentity?.Name;
            //    //    // If The User Is Exist And Doesn't Has Token In The DataBase...
            //    //    string domain = Configuration["Domain"];
            //    //    using (PrincipalContext context = new PrincipalContext(ContextType.Domain, domain, null, ContextOptions.SimpleBind))
            //    //    {
            //    //        UserPrincipal principal = new UserPrincipal(context);

            //    //        if (context != null)
            //    //        {
            //    //            principal = UserPrincipal.FindByIdentity(context, IdentityType.SamAccountName, UserName);
            //    //            if (principal != null)
            //    //            {
            //    //                // Check The UserName If It is Exist In The DataBase..
            //    //                TLIuser CheckUserIfExist = _unitOfWork.UserRepository.GetWhereFirst(x => x.UserName == UserName);
            //    //                if (CheckUserIfExist != null)
            //    //                {
            //    //                    string secretKey = Configuration["JWT:Key"];
            //    //                    string TokenValue = _unitOfWorkService.TokenService.CreateToken(
            //    //                        new LoginViewModel
            //    //                        {
            //    //                            UserName = UserName
            //    //                        }, secretKey, null, null).Data;
            //    //                    string expected = JsonConvert.SerializeObject(new { UserState = "authenticated", Token = TokenValue });

            //    //                    await Runcontext.Response.WriteAsync(expected);
            //    //                }

            //    //                // If The User Isn't Exist In The DataBase Then Return Response 
            //    //                else
            //    //                {
            //    //                    string expected = JsonConvert.SerializeObject(new { UserState = "Unauthenticated" });
            //    //                    await Runcontext.Response.WriteAsync(expected);
            //    //                }
            //    //            }

            //    // Else If The User Is Not Exist In The Domain..
            //    //else
            //    //{
            //    //    string expected = JsonConvert.SerializeObject(new { UserState = "External User" });
            //    //    await Runcontext.Response.WriteAsync(expected);
            //    //}
            //    // }
            //    // }
            //    //}
            //});
            #endregion

        }
    }
}
