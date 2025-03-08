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
                        var processFilesTask1 = unitOfWorkService.SiteService.ProcessFilesAsync1();
                        var processFilesTask2 = unitOfWorkService.SiteService.ProcessFilesAsync2();
                        var processFilesTask3 = unitOfWorkService.SiteService.ProcessFilesAsync3();
                        var processFilesTask4 = unitOfWorkService.SiteService.ProcessFilesAsync4();
                        var processFilesTask5 = unitOfWorkService.SiteService.ProcessFilesAsync5();
                        var processFilesTask6 = unitOfWorkService.SiteService.ProcessFilesAsync6();
                        var processFilesTask7 = unitOfWorkService.SiteService.ProcessFilesAsync7();
                        var processFilesTask8 = unitOfWorkService.SiteService.ProcessFilesAsync8();
                        var processFilesTask9 = unitOfWorkService.SiteService.ProcessFilesAsync9();
                        var logsTask = unitOfWorkService.SiteService.GetFilteredLogsBackGroundServices();
                        var historyTask = unitOfWorkService.SiteService.GetHistoryFile();
                        var securityLogsTask = unitOfWorkService.UserService.GetSecurityLogsFile();

                        // انتظار انتهاء جميع المهام معًا
                        await Task.WhenAll(getSmisTask, processFilesTask1, processFilesTask2, processFilesTask3, processFilesTask4, processFilesTask5, processFilesTask6, processFilesTask7, processFilesTask8, processFilesTask9, logsTask, historyTask, securityLogsTask);

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