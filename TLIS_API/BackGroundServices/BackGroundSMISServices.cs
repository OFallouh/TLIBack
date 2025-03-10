using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using TLIS_Service.ServiceBase;
using TLIS_DAL.Models;
using TLIS_DAL;

namespace TLIS_API.BackGroundServices
{
    public class BackGroundSMISServices : BackgroundService
    {
        private readonly IServiceProvider _services;

        public BackGroundSMISServices(IServiceProvider services)
        {
            _services = services;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _services.CreateScope())
                {
                    var unitOfWorkService = scope.ServiceProvider.GetRequiredService<IUnitOfWorkService>();
                    try
                    {
                        Console.WriteLine("Starting all background tasks...");

                        await unitOfWorkService.SiteService.GetSMIS_Site();
                        await unitOfWorkService.SiteService.ProcessFilesAsync1();
                        await unitOfWorkService.SiteService.ProcessFilesAsync2();
                        await unitOfWorkService.SiteService.ProcessFilesAsync3();
                        await unitOfWorkService.SiteService.ProcessFilesAsync4();
                        await unitOfWorkService.SiteService.ProcessFilesAsync5();
                        await unitOfWorkService.SiteService.ProcessFilesAsync6();
                        await unitOfWorkService.SiteService.ProcessFilesAsync7();
                        await unitOfWorkService.SiteService.ProcessFilesAsync8();
                        await unitOfWorkService.SiteService.ProcessFilesAsync9();
                        await unitOfWorkService.SiteService.ProcessFilesAsync10();
                        await unitOfWorkService.SiteService.ProcessFilesAsync11();
                        await unitOfWorkService.SiteService.ProcessFilesAsync12();
                        await unitOfWorkService.SiteService.ProcessFilesAsync13();
                        await unitOfWorkService.SiteService.ProcessFilesAsync14();
                        await unitOfWorkService.SiteService.ProcessFilesAsync15();

                        Console.WriteLine("All background tasks completed successfully.");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error in Background Task: {ex.Message}");
                        await LogErrorToDb(ex, "GeneralBackgroundTaskError");
                    }
                }

                await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
            }
        }

        private async Task LogErrorToDb(Exception ex, string methodName)
        {
            using (var scope = _services.CreateScope())
            {
                ApplicationDbContext? _context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var errorRecord = new TLIimportSheet
                {
                    UniqueName = Guid.NewGuid().ToString(),
                    SheetName = methodName,
                    RefTable = "N/A",
                    IsLib = false,
                    ErrMsg = ex.Message,
                    CreatedAt = DateTime.UtcNow,
                    IsDeleted = false
                };

                _context.TLIimportSheets.Add(errorRecord);
                await _context.SaveChangesAsync();
            }
        }
    }
}
