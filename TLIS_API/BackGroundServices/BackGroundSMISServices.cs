using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using TLIS_Service.ServiceBase;
using System.Collections.Generic;
using TLIS_DAL.Models;
using TLIS_DAL;
using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;

namespace TLIS_API.BackGroundServices
{
    public class BackGroundSMISServices : BackgroundService
    {
        private readonly IServiceProvider _services;


        // حقن ILogger في الـ constructor
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

                        // إنشاء قائمة من المهام بحيث يتم تنفيذها بشكل منفصل
                        var tasks = new List<Task>
                    {
                        Task.Run(async () => {
                            try {
                                await unitOfWorkService.SiteService.GetSMIS_Site();
                            }
                            catch (Exception ex)
                            {
                                await LogErrorToDb(ex, "GetSMIS_Site");
                            }
                        }),
                        Task.Run(async () => {
                            try {
                                await unitOfWorkService.SiteService.ProcessFilesAsync1();
                            }
                            catch (Exception ex)
                            {
                                await LogErrorToDb(ex, "ProcessFilesAsync1");
                            }
                        }),
                        Task.Run(async () => {
                            try {
                                await unitOfWorkService.SiteService.ProcessFilesAsync2();
                            }
                            catch (Exception ex)
                            {
                                await LogErrorToDb(ex, "ProcessFilesAsync2");
                            }
                        }),
                        Task.Run(async () => {
                            try {
                                await unitOfWorkService.SiteService.ProcessFilesAsync3();
                            }
                            catch (Exception ex)
                            {
                                await LogErrorToDb(ex, "ProcessFilesAsync3");
                            }
                        }),
                        // إضافة المزيد من المهام إذا لزم الأمر
                    };

                        // انتظار انتهاء جميع المهام معًا
                        await Task.WhenAll(tasks);

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
                ApplicationDbContext? _context =
                scope.ServiceProvider
                .GetRequiredService<ApplicationDbContext>();
                var errorRecord = new TLIimportSheet
                {
                    UniqueName = Guid.NewGuid().ToString(),  // أو أي قيمة فريدة أخرى
                    SheetName = methodName,
                    RefTable = "N/A", // يمكن تخصيصها حسب الحاجة
                    IsLib = false, // حسب الحاجة
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
