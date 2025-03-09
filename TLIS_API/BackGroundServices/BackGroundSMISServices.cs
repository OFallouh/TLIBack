using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using TLIS_Service.ServiceBase;
using System.Collections.Generic;

namespace TLIS_API.BackGroundServices
{
    public class BackGroundSMISServices : BackgroundService
    {
        private readonly IServiceProvider _services;
        private readonly ILogger<BackGroundSMISServices> _logger;

        // حقن ILogger في الـ constructor
        public BackGroundSMISServices(IServiceProvider services, ILogger<BackGroundSMISServices> logger)
        {
            _services = services;
            _logger = logger;
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

                        // إنشاء قائمة من المهام بحيث يتم تنفيذها بشكل منفصل
                        var tasks = new List<Task>
                {
                    Task.Run(async () => { try { await unitOfWorkService.SiteService.GetSMIS_Site(); } catch (Exception ex) { _logger.LogError(ex, "Error in GetSMIS_Site"); } }),
                    Task.Run(async () => { try { await unitOfWorkService.SiteService.ProcessFilesAsync1(); } catch (Exception ex) { _logger.LogError(ex, "Error in ProcessFilesAsync1"); } }),
                    Task.Run(async () => { try { await unitOfWorkService.SiteService.ProcessFilesAsync2(); } catch (Exception ex) { _logger.LogError(ex, "Error in ProcessFilesAsync2"); } }),
                    Task.Run(async () => { try { await unitOfWorkService.SiteService.ProcessFilesAsync3(); } catch (Exception ex) { _logger.LogError(ex, "Error in ProcessFilesAsync3"); } }),
                    Task.Run(async () => { try { await unitOfWorkService.SiteService.ProcessFilesAsync4(); } catch (Exception ex) { _logger.LogError(ex, "Error in ProcessFilesAsync4"); } }),
                    Task.Run(async () => { try { await unitOfWorkService.SiteService.ProcessFilesAsync5(); } catch (Exception ex) { _logger.LogError(ex, "Error in ProcessFilesAsync5"); } }),
                    Task.Run(async () => { try { await unitOfWorkService.SiteService.ProcessFilesAsync6(); } catch (Exception ex) { _logger.LogError(ex, "Error in ProcessFilesAsync6"); } }),
                    Task.Run(async () => { try { await unitOfWorkService.SiteService.ProcessFilesAsync7(); } catch (Exception ex) { _logger.LogError(ex, "Error in ProcessFilesAsync7"); } }),
                    Task.Run(async () => { try { await unitOfWorkService.SiteService.ProcessFilesAsync8(); } catch (Exception ex) { _logger.LogError(ex, "Error in ProcessFilesAsync8"); } }),
                    Task.Run(async () => { try { await unitOfWorkService.SiteService.ProcessFilesAsync9(); } catch (Exception ex) { _logger.LogError(ex, "Error in ProcessFilesAsync9"); } }),
                    Task.Run(async () => { try { await unitOfWorkService.SiteService.GetFilteredLogsBackGroundServices(); } catch (Exception ex) { _logger.LogError(ex, "Error in GetFilteredLogsBackGroundServices"); } }),
                    Task.Run(async () => { try { await unitOfWorkService.SiteService.GetHistoryFile(); } catch (Exception ex) { _logger.LogError(ex, "Error in GetHistoryFile"); } }),
                    Task.Run(async () => { try { await unitOfWorkService.UserService.GetSecurityLogsFile(); } catch (Exception ex) { _logger.LogError(ex, "Error in GetSecurityLogsFile"); } })
                };

                        // انتظار انتهاء جميع المهام معًا
                        await Task.WhenAll(tasks);

                        Console.WriteLine("All background tasks completed successfully.");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error in Background Task: {ex.Message}");
                        _logger.LogError(ex, "Unexpected error in background tasks");
                    }
                }

                await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
            }
        }

    }
}
