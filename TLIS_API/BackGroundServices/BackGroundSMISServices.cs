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
                        // تنفيذ دالة GetSMIS_Site كل 24 ساعة
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

                // الانتظار لمدة 24 ساعة قبل التشغيل مرة أخرى
                await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
            }
        }
    }
}