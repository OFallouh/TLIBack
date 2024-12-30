using Microsoft.Extensions.Hosting;
using System.Threading;
using System;
using AutoMapper;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using System.Text.Json;
using System.Threading.Tasks;
using TLIS_Service.ServiceBase;
using Microsoft.Extensions.DependencyInjection;


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

                    await unitOfWorkService.SiteService.GetSMIS_Site(null, null, null, null, null);
                }

                // الانتظار لمدة 24 ساعة
                await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
            }
        }
    }

}
