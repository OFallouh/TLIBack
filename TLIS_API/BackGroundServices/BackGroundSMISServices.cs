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
                DateTime now = DateTime.Now;
                DateTime nextRun = now.Date.AddDays(1).AddHours(0); // الساعة 12:00 منتصف الليل

                TimeSpan waitTime = nextRun - now;

                Console.WriteLine($"Next execution scheduled at: {nextRun}");

                // الانتظار حتى منتصف الليل
                await Task.Delay(waitTime, stoppingToken);

                using (var scope = _services.CreateScope())
                {
                    var unitOfWorkService = scope.ServiceProvider.GetRequiredService<IUnitOfWorkService>();

                    try
                    {
                        await unitOfWorkService.SiteService.GetSMIS_Site();
                        await unitOfWorkService.SiteService.ProcessFilesAsync();
                        await unitOfWorkService.SiteService.GetFilteredLogsBackGroundServices();
                        await unitOfWorkService.SiteService.GetHistoryFile();
                        await unitOfWorkService.UserService.GetSecurityLogsFile();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error in Background Task: {ex.Message}");
                    }
                }
            }
        }
    }
}
