using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using System.Threading.Tasks;
using TLIS_Service.ServiceBase;

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

                        // تشغيل جميع المهام بالتوازي
                        var getSmisTask = unitOfWorkService.SiteService.GetSMIS_Site();
                        var processFilesTask = unitOfWorkService.SiteService.ProcessFilesAsync();
                        var logsTask = unitOfWorkService.SiteService.GetFilteredLogsBackGroundServices();
                        var historyTask = unitOfWorkService.SiteService.GetHistoryFile();
                        var securityLogsTask = unitOfWorkService.UserService.GetSecurityLogsFile();

                        // انتظار انتهاء جميع المهام معًا
                        await Task.WhenAll(getSmisTask, processFilesTask, logsTask, historyTask, securityLogsTask);

                        Console.WriteLine("All background tasks completed successfully.");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error in Background Task: {ex.Message}");
                    }
                }

                await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
            }
        }

    }
}