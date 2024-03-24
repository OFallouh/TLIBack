using AutoMapper;
using MailKit.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MimeKit;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Engineering;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.DirectoryServices.Protocols;
using System.Linq;
using System.Text;
using System.Transactions;
using TLIS_DAL;
using TLIS_DAL.Helpers;
using TLIS_DAL.Models;
using TLIS_DAL.ViewModelBase;
using TLIS_DAL.ViewModels.SiteDTOs;
using TLIS_DAL.ViewModels.wf;
using TLIS_Repository.Base;
using TLIS_Repository.IRepository;
using MailKit.Net.Smtp;
using static TLIS_Repository.Helpers.Constants;
using System.Net.Http;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using Newtonsoft.Json;


namespace TLIS_Repository.Repositories
{
    public class SiteRepository : RepositoryBase<TLIsite, SiteViewModel, string>, ISiteRepository
    {
        private ApplicationDbContext _context = null;
        IMapper _mapper;
        private readonly IConfiguration _configuration;
        IServiceProvider Services;
        public SiteRepository(ApplicationDbContext context, IServiceProvider service, IConfiguration configuration, IMapper mapper) : base(context, mapper)
        {
            _context = context;
            _mapper = mapper;
            _configuration = configuration;
            Services = service;
        }

        public List<KeyValuePair<string, List<DropDownListFilters>>> GetRelatedTables()
        {
            List<KeyValuePair<string, List<DropDownListFilters>>> RelatedTables = new List<KeyValuePair<string, List<DropDownListFilters>>>();
            //var siteStatus = _context.TLIsiteStatus.ToList();
            //List<DropDownListFilters> siteStatusLists = _mapper.Map<List<DropDownListFilters>>(siteStatus);
            //RelatedTables.Add(new KeyValuePair<string, List<DropDownListFilters>>("siteStatusId", siteStatusLists));
            List<TLIregion> Region = _context.TLIregion.ToList();
            List<DropDownListFilters> RegionLists = _mapper.Map<List<DropDownListFilters>>(Region);
            RelatedTables.Add(new KeyValuePair<string, List<DropDownListFilters>>("RegionCode", RegionLists));
            var Area = _context.TLIarea.ToList();
            List<DropDownListFilters> AreaLists = _mapper.Map<List<DropDownListFilters>>(Area);
            RelatedTables.Add(new KeyValuePair<string, List<DropDownListFilters>>("AreaCode", AreaLists));
            return RelatedTables;
        }

        public void UpdateReservedSpace(string SiteCode, float SpaceInstallation)
        {
            var Site = _context.TLIsite.Where(x => x.SiteCode == SiteCode).FirstOrDefault();
            Site.ReservedSpace += SpaceInstallation;
            _context.TLIsite.Update(Site);
            _context.SaveChanges();

        }
        //public Response<string> CheckSpace (string SiteCode , string TableName, int LibraryId)
        // {
        //     try
        //     {
        //         var civilsite = _context.TLIcivilSiteDate.Where(x => x.SiteCode == SiteCode && x.ReservedSpace == true).Include(x => x.allCivilInst).Select(x => x.allCivilInst).ToList();
        //         float Space = 0;
        //         foreach (var Item in civilsite)
        //         {
        //            if(Item.civilWithLegsId != null)
        //             {
        //                 var civilWithLegsSpace = _context.TLIcivilWithLegs.Where(x => x.Id == Item.civilWithLegsId).Select(x=>x.SpaceInstallation).FirstOrDefault();
        //                 Space = civilWithLegsSpace + Space;
        //             }
        //            if(Item.civilWithoutLegId != null)
        //             {
        //                 var civilWithoutLegSpace = _context.TLIcivilWithoutLeg.Where(x => x.Id == Item.civilWithoutLegId).Select(x => x.SpaceInstallation).FirstOrDefault();
        //                 Space = civilWithoutLegSpace + Space;
        //             }

        //         }
        //         var OtherInst = _context.TLIotherInSite.Where(x => x.SiteCode == SiteCode && x.ReservedSpace == true).Include(x => x.allOtherInventoryInst).Select(x => x.allOtherInventoryInst).ToList();

        //         foreach (var Item in OtherInst)
        //         {
        //             if (Item.cabinetId != null)
        //             {
        //                 var civilWithLegsSpace = _context.TLIcabinet.Where(x => x.Id == Item.cabinetId).Select(x => x.SpaceInstallation).FirstOrDefault();
        //                 Space = civilWithLegsSpace + Space;
        //             }
        //             if (Item.solarId != null)
        //             {
        //                 var civilWithoutLegSpace = _context.TLIsolar.Where(x => x.Id == Item.solarId).Select(x => x.SpaceInstallation).FirstOrDefault();
        //                 Space = civilWithoutLegSpace + Space;
        //             }
        //             if (Item.generatorId != null)
        //             {
        //                 var civilWithoutLegSpace = _context.TLIgenerator.Where(x => x.Id == Item.generatorId).Select(x => x.SpaceInstallation).FirstOrDefault();
        //                 Space = civilWithoutLegSpace + Space;
        //             }
        //         }
        //           if (Helpers.Constants.CivilType.TLIcivilWithLegs.ToString() == TableName)
        //         {
        //             var civilwithlegsSpaceLibrary = _context.TLIcivilWithLegLibrary.FirstOrDefault(x => x.Id == LibraryId).SpaceLibrary;
        //             Space = Space + civilwithlegsSpaceLibrary;
        //         }
        //         if (Helpers.Constants.CivilType.TLIcivilWithoutLeg.ToString() == TableName)
        //         {
        //             var civilWithoutLegSpaceLibrary = _context.TLIcivilWithoutLegLibrary.FirstOrDefault(x => x.Id == LibraryId).SpaceLibrary;
        //             Space = Space + civilWithoutLegSpaceLibrary;
        //         }
        //         if (OtherInventoryType.TLIcabinet.ToString() == TableName)
        //         {
        //             var cabinetPowerLibrarySpaceLibrary = _context.TLIcabinetPowerLibrary.FirstOrDefault(x => x.Id == LibraryId).SpaceLibrary;
        //             Space = Space + cabinetPowerLibrarySpaceLibrary;

        //         }
        //         if (OtherInventoryType.TLIcabinet.ToString() == TableName)
        //         {
        //             var cabinetSpaceLibrary = _context.TLIcabinetTelecomLibrary.FirstOrDefault(x => x.Id == LibraryId).SpaceLibrary;
        //             Space = Space + cabinetSpaceLibrary;
        //         }
        //         if (OtherInventoryType.TLIgenerator.ToString() == TableName)
        //         {
        //             var generatorLibrarySpaceLibrary = _context.TLIgeneratorLibrary.FirstOrDefault(x => x.Id == LibraryId).SpaceLibrary;
        //             Space = Space + generatorLibrarySpaceLibrary;
        //         }
        //         if (OtherInventoryType.TLIsolar.ToString() == TableName)
        //         {
        //             var solarLibrarySpaceLibrary = _context.TLIsolarLibrary.FirstOrDefault(x => x.Id == LibraryId).SpaceLibrary;
        //             Space = Space + solarLibrarySpaceLibrary;
        //         }
        //         var siteSpace = _context.TLIsite.FirstOrDefault(x => x.SiteCode == SiteCode).RentedSpace;
        //         if (Space > siteSpace)
        //         {
        //             return new Response<string>(true, null, null, "Not available space", (int)Helpers.Constants.ApiReturnCode.fail);
        //         }                                              
        //         return new Response<string>();
        //     }

        //     catch (Exception err)
        //     {

        //         return new Response<string>(true, null, null,err.Message , (int)Helpers.Constants.ApiReturnCode.fail); 
        //     }
        // }
        public Response<string> CheckSpace(string SiteCode, string TableName, int LibraryId, float SpaceInstallation, string Cabinet)
        {
            try
            {
                var Site = _context.TLIsite.Where(x => x.SiteCode == SiteCode).FirstOrDefault();
                if (SpaceInstallation != 0)
                {
                    var space = Site.ReservedSpace + SpaceInstallation;
                    if (space <= Site.RentedSpace)
                    {
                        Site.ReservedSpace = space;
                        _context.TLIsite.Update(Site);
                        _context.SaveChanges();
                    }
                    else
                    {
                        return new Response<string>(true, null, null, "Not available space", (int)Helpers.Constants.ApiReturnCode.fail);
                    }
                }
                else
                {
                    if (Helpers.Constants.CivilType.TLIcivilWithoutLeg.ToString() == TableName)
                    {
                        var civilWithoutLegSpaceLibrary = _context.TLIcivilWithoutLegLibrary.FirstOrDefault(x => x.Id == LibraryId).SpaceLibrary;
                        if (civilWithoutLegSpaceLibrary != 0)
                        {
                            var space = Site.ReservedSpace + civilWithoutLegSpaceLibrary;
                            if (space <= Site.RentedSpace)
                            {
                                Site.ReservedSpace = space;
                                _context.TLIsite.Update(Site);
                                _context.SaveChanges();

                            }
                            else
                            {
                                return new Response<string>(true, null, null, "Not available space", (int)Helpers.Constants.ApiReturnCode.fail);
                            }

                        }
                        else
                        {
                            return new Response<string>(true, null, null, "Add spacelibrary or spaceinstallation", (int)Helpers.Constants.ApiReturnCode.fail);
                        }

                    }
                    else if (Helpers.Constants.CivilType.TLIcivilWithLegs.ToString() == TableName)
                    {
                        var civilWithLegSpaceLibrary = _context.TLIcivilWithLegLibrary.FirstOrDefault(x => x.Id == LibraryId);
                        if (civilWithLegSpaceLibrary.SpaceLibrary != 0)
                        {
                            var space = Site.ReservedSpace + civilWithLegSpaceLibrary.SpaceLibrary;
                            if (space <= Site.RentedSpace)
                            {
                                Site.ReservedSpace = space;
                                _context.TLIsite.Update(Site);
                                _context.SaveChanges();

                            }
                            else
                            {
                                return new Response<string>(true, null, null, "Not available space", (int)Helpers.Constants.ApiReturnCode.fail);
                            }

                        }
                        else
                        {
                            return new Response<string>(true, null, null, "Add spacelibrary or spaceinstallation", (int)Helpers.Constants.ApiReturnCode.fail);
                        }
                    }
                    else if (Helpers.Constants.CivilType.TLIcivilNonSteel.ToString() == TableName)
                    {
                        var civilNonSteelLibrary = _context.TLIcivilNonSteelLibrary.FirstOrDefault(x => x.Id == LibraryId);
                        if (civilNonSteelLibrary.SpaceLibrary != 0)
                        {
                            var space = Site.ReservedSpace + civilNonSteelLibrary.SpaceLibrary;
                            if (space <= Site.RentedSpace)
                            {
                                Site.ReservedSpace = space;
                                _context.TLIsite.Update(Site);
                                _context.SaveChanges();

                            }
                            else
                            {
                                return new Response<string>(true, null, null, "Not available space", (int)Helpers.Constants.ApiReturnCode.fail);
                            }

                        }
                        else
                        {
                            return new Response<string>(true, null, null, "Add spacelibrary or spaceinstallation", (int)Helpers.Constants.ApiReturnCode.fail);
                        }
                    }
                    else if (OtherInventoryType.TLIcabinet.ToString() == TableName && Cabinet == "Power")
                    {
                        var cabinetPowerLibrarySpaceLibrary = _context.TLIcabinetPowerLibrary.FirstOrDefault(x => x.Id == LibraryId);
                        if (cabinetPowerLibrarySpaceLibrary.SpaceLibrary != 0)
                        {
                            var space = Site.ReservedSpace + cabinetPowerLibrarySpaceLibrary.SpaceLibrary;
                            if (space <= Site.RentedSpace)
                            {
                                Site.ReservedSpace = space;
                                _context.TLIsite.Update(Site);
                                _context.SaveChanges();

                            }

                            else
                            {
                                return new Response<string>(true, null, null, "Not available space", (int)Helpers.Constants.ApiReturnCode.fail);
                            }
                        }
                        else if (cabinetPowerLibrarySpaceLibrary.Depth != 0 && cabinetPowerLibrarySpaceLibrary.Width != 0)
                        {
                            var lengh = cabinetPowerLibrarySpaceLibrary.Depth;
                            var Width = cabinetPowerLibrarySpaceLibrary.Width;
                            var result = (lengh * Width) + Site.ReservedSpace;
                            if (result <= Site.RentedSpace)
                            {
                                Site.ReservedSpace = result;
                                _context.TLIsite.Update(Site);
                                _context.SaveChanges();

                            }
                            else
                            {
                                return new Response<string>(true, null, null, "Not available space", (int)Helpers.Constants.ApiReturnCode.fail);
                            }
                        }
                        else
                        {
                            return new Response<string>(true, null, null, "Add spacelibrary or spaceinstallation", (int)Helpers.Constants.ApiReturnCode.fail);
                        }
                    }
                    else if (OtherInventoryType.TLIcabinet.ToString() == TableName && Cabinet == "Telecom")
                    {
                        var cabinetSpaceLibrary = _context.TLIcabinetTelecomLibrary.FirstOrDefault(x => x.Id == LibraryId);
                        if (cabinetSpaceLibrary.SpaceLibrary != 0)
                        {
                            var space = Site.ReservedSpace + cabinetSpaceLibrary.SpaceLibrary;
                            if (space <= Site.RentedSpace)
                            {
                                Site.ReservedSpace = space;
                                _context.TLIsite.Update(Site);
                                _context.SaveChanges();

                            }
                            else
                            {
                                return new Response<string>(true, null, null, "Not available space", (int)Helpers.Constants.ApiReturnCode.fail);
                            }

                        }
                        else if (cabinetSpaceLibrary.Depth != 0 && cabinetSpaceLibrary.Width != 0)
                        {
                            var lengh = cabinetSpaceLibrary.Depth;
                            var Width = cabinetSpaceLibrary.Width;
                            var result = (lengh * Width) + Site.ReservedSpace;
                            if (result <= Site.RentedSpace)
                            {
                                Site.ReservedSpace = result;
                                _context.TLIsite.Update(Site);
                                _context.SaveChanges();

                            }
                            else
                            {
                                return new Response<string>(true, null, null, "Not available space", (int)Helpers.Constants.ApiReturnCode.fail);
                            }
                        }
                        else
                        {
                            return new Response<string>(true, null, null, "Add spacelibrary or spaceinstallation", (int)Helpers.Constants.ApiReturnCode.fail);
                        }
                    }
                    else if (OtherInventoryType.TLIgenerator.ToString() == TableName)
                    {
                        var generatorLibrarySpaceLibrary = _context.TLIgeneratorLibrary.FirstOrDefault(x => x.Id == LibraryId);
                        if (generatorLibrarySpaceLibrary.SpaceLibrary != 0)
                        {
                            var space = Site.ReservedSpace + generatorLibrarySpaceLibrary.SpaceLibrary;
                            if (space <= Site.RentedSpace)
                            {
                                Site.ReservedSpace = space;
                                _context.TLIsite.Update(Site);
                                _context.SaveChanges();

                            }
                            else
                            {
                                return new Response<string>(true, null, null, "Not available space", (int)Helpers.Constants.ApiReturnCode.fail);
                            }

                        }

                        else if (generatorLibrarySpaceLibrary.Length != 0 && generatorLibrarySpaceLibrary.Width != 0)
                        {
                            var lengh = generatorLibrarySpaceLibrary.Length;
                            var Width = generatorLibrarySpaceLibrary.Width;
                            var result = (lengh * Width) + Site.ReservedSpace;
                            if (result <= Site.RentedSpace)
                            {
                                Site.ReservedSpace = result;
                                _context.TLIsite.Update(Site);
                                _context.SaveChanges();

                            }
                            else
                            {
                                return new Response<string>(true, null, null, "Not available space", (int)Helpers.Constants.ApiReturnCode.fail);
                            }
                        }
                        else
                        {
                            return new Response<string>(true, null, null, "Add spacelibrary or spaceinstallation", (int)Helpers.Constants.ApiReturnCode.fail);
                        }
                    }
                    else if (OtherInventoryType.TLIsolar.ToString() == TableName)
                    {
                        var solarLibrarySpaceLibrary = _context.TLIsolarLibrary.FirstOrDefault(x => x.Id == LibraryId);
                        if (solarLibrarySpaceLibrary.SpaceLibrary != 0)
                        {
                            var space = Site.ReservedSpace + solarLibrarySpaceLibrary.SpaceLibrary;
                            if (space <= Site.RentedSpace)
                            {
                                Site.ReservedSpace = space;
                                _context.TLIsite.Update(Site);
                                _context.SaveChanges();

                            }
                            else
                            {
                                return new Response<string>(true, null, null, "Not available space", (int)Helpers.Constants.ApiReturnCode.fail);
                            }

                        }
                        else
                        {
                            return new Response<string>(true, null, null, "Add spacelibrary or spaceinstallation", (int)Helpers.Constants.ApiReturnCode.fail);
                        }
                    }
                }

                return new Response<string>();
            }
            catch (Exception err)
            {
                return new Response<string>(true, null, null, err.Message, (int)Helpers.Constants.ApiReturnCode.fail);
            }
        }
        public class SumbitsTaskByTLI
        {

            public bool result { get; set; }
            public object count { get; set; }
            public object errorMessage { get; set; }


        }
        public class EditTicketInfoBinding
        {
            public int? TaskId { get; set; }
            public string? SiteCode { get; set; }
            public string? RegionName { get; set; }
            public string? AreaName { get; set; }
            public string? CityName { get; set; }

        }
        private static readonly HttpClient _httpClient = new HttpClient();
        public async Task<SumbitsTaskByTLI> SubmitTaskByTLI(int? TaskId)
        {

            var ExternalApi = _configuration["ExternalApi"];
            using (var scope = Services.CreateScope())
            {
                IMapper _Mapper = scope.ServiceProvider.GetRequiredService<IMapper>();
                string apiUrl = $"{ExternalApi}/api/TicketManagement/SubmitTaskByTLI?taskId={TaskId}";

                int maxRetries = 1; // Number of retries
                int retryDelayMilliseconds = 180000; // 3 minutes in milliseconds

                for (int retryCount = 0; retryCount < maxRetries; retryCount++)
                {
                    try
                    {
                        HttpResponseMessage response = await _httpClient.PostAsync(apiUrl, null);

                        if (response.IsSuccessStatusCode)
                        {
                            string responseBody = await response.Content.ReadAsStringAsync();

                            if (responseBody != null)
                            {
                                var siteInfoObject = System.Text.Json.JsonSerializer.Deserialize<SumbitsTaskByTLI>(responseBody);
                                return siteInfoObject;
                            }
                        }
                        else
                        {
                            Console.WriteLine($"API request failed with status code: {response.StatusCode}");
                            throw new Exception($"API request failed with status code: {response.StatusCode}");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"An error occurred: {ex.Message}");
                        throw; // Rethrow the exception
                    }

                    await Task.Delay(retryDelayMilliseconds);
                }

                // If the loop completes without returning, it means all retries failed
                throw new Exception("All retries failed for SubmitTaskByTLI");
            }
        }
        public async Task<SumbitsTaskByTLI> EditTicketInfoByTLI(EditTicketInfoBinding editTicketInfoBinding)
        {
            var ExternalApi = _configuration["ExternalApi"];
            using (var scope = Services.CreateScope())
            {
                IMapper _Mapper = scope.ServiceProvider.GetRequiredService<IMapper>();
                string apiUrl = $"{ExternalApi}/api/TicketManagement/EditTicketInfo";

                int maxRetries = 1;
                int retryDelayMilliseconds = 180000; // 3 minutes in milliseconds

                for (int retryCount = 0; retryCount < maxRetries; retryCount++)
                {
                    try
                    {
                        var jsonContent = JsonConvert.SerializeObject(editTicketInfoBinding);
                        var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                        HttpResponseMessage response = await _httpClient.PostAsync(apiUrl, content);

                        if (response.IsSuccessStatusCode)
                        {
                            string responseBody = await response.Content.ReadAsStringAsync();

                            if (responseBody != null)
                            {
                                var siteInfoObject = System.Text.Json.JsonSerializer.Deserialize<SumbitsTaskByTLI>(responseBody);
                                return siteInfoObject;
                            }
                        }
                        else
                        {
                            Console.WriteLine($"API request failed with status code: {response.StatusCode}");
                            throw new Exception($"API request failed with status code: {response.StatusCode}");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"An error occurred: {ex.Message}");
                        throw; 
                    }

                    await Task.Delay(retryDelayMilliseconds);
                }

                throw new Exception("All retries failed for SubmitTaskByTLI");
            }
        }


    }
}
