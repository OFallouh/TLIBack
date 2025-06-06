﻿using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using System.Threading.Tasks;
using TLIS_Service.ServiceBase;
using TLIS_DAL;
using TLIS_DAL.Models;

namespace TLIS_API.BackGroundServices
{
    public class BackGroundSMISServices : BackgroundService
    {
        private readonly IServiceProvider _services;

        public BackGroundSMISServices(IServiceProvider services)
        {
            _services = services;
        }

        //protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        //  {
        //      while (!stoppingToken.IsCancellationRequested)
        //      {
        //          // حساب الوقت المتبقي حتى الساعة 12 منتصف الليل
        //          var currentTime = DateTime.UtcNow;
        //          var nextRunTime = DateTime.UtcNow.Date.AddDays(1); // الساعة 12 منتصف الليل ليوم غد

        //          // تأكد من أن الوقت التالي هو فعلاً في المستقبل
        //          if (currentTime > nextRunTime)
        //          {
        //              nextRunTime = nextRunTime.AddDays(1);
        //          }

        //          var timeToWait = nextRunTime - currentTime;

        //          // انتظر حتى الساعة 12 منتصف الليل
        //          await Task.Delay(timeToWait, stoppingToken);

        //          using (var scope = _services.CreateScope())
        //          {
        //              var unitOfWorkService = scope.ServiceProvider.GetRequiredService<IUnitOfWorkService>();
        //              var _context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        //              try
        //              {
        //                  Console.WriteLine("🔹 بدء تنفيذ جميع العمليات بالتسلسل...");

        //                  // تنفيذ العمليات واحدة تلو الأخرى مع تسجيل أي أخطاء في الجدول
        //                  await ExecuteWithLogging(() => unitOfWorkService.SiteService.GetSMIS_Site(), "GetSMIS_Site", _context);
        //                  await ExecuteWithLogging(() => unitOfWorkService.SiteService.ProcessFilesAsync1(), "ProcessFilesAsync1", _context);
        //                  await ExecuteWithLogging(() => unitOfWorkService.SiteService.ProcessFilesAsync2(), "ProcessFilesAsync2", _context);
        //                  await ExecuteWithLogging(() => unitOfWorkService.SiteService.ProcessFilesAsync3(), "ProcessFilesAsync3", _context);
        //                  await ExecuteWithLogging(() => unitOfWorkService.SiteService.ProcessFilesAsync4(), "ProcessFilesAsync4", _context);
        //                  await ExecuteWithLogging(() => unitOfWorkService.SiteService.ProcessFilesAsync5(), "ProcessFilesAsync5", _context);
        //                  await ExecuteWithLogging(() => unitOfWorkService.SiteService.ProcessFilesAsync6(), "ProcessFilesAsync6", _context);
        //                  await ExecuteWithLogging(() => unitOfWorkService.SiteService.ProcessFilesAsync7(), "ProcessFilesAsync7", _context);
        //                  await ExecuteWithLogging(() => unitOfWorkService.SiteService.ProcessFilesAsync8(), "ProcessFilesAsync8", _context);
        //                  await ExecuteWithLogging(() => unitOfWorkService.SiteService.ProcessFilesAsync9(), "ProcessFilesAsync9", _context);
        //                  await ExecuteWithLogging(() => unitOfWorkService.SiteService.ProcessFilesAsync10(), "ProcessFilesAsync10", _context);
        //                  await ExecuteWithLogging(() => unitOfWorkService.SiteService.ProcessFilesAsync11(), "ProcessFilesAsync11", _context);
        //                  await ExecuteWithLogging(() => unitOfWorkService.SiteService.ProcessFilesAsync12(), "ProcessFilesAsync12", _context);
        //                  await ExecuteWithLogging(() => unitOfWorkService.SiteService.ProcessFilesAsync13(), "ProcessFilesAsync13", _context);
        //                  await ExecuteWithLogging(() => unitOfWorkService.SiteService.ProcessFilesAsync14(), "ProcessFilesAsync14", _context);
        //                  await ExecuteWithLogging(() => unitOfWorkService.SiteService.ProcessFilesAsync15(), "ProcessFilesAsync15", _context);
        //                  await ExecuteWithLogging(() => unitOfWorkService.SiteService.ProcessFilesAsync16(), "ProcessFilesAsync16", _context);
        //                  await ExecuteWithLogging(() => unitOfWorkService.SiteService.ProcessFilesAsync17(), "ProcessFilesAsync17", _context);
        //                  await ExecuteWithLogging(() => unitOfWorkService.SiteService.ProcessFilesAsync18(), "ProcessFilesAsync18", _context);
        //                  await ExecuteWithLogging(() => unitOfWorkService.SiteService.ProcessFilesAsync19(), "ProcessFilesAsync19", _context);
        //                  await ExecuteWithLogging(() => unitOfWorkService.SiteService.ProcessFilesAsync20(), "ProcessFilesAsync20", _context);
        //                  await ExecuteWithLogging(() => unitOfWorkService.SiteService.ProcessFilesAsync21(), "ProcessFilesAsync21", _context);
        //                  await ExecuteWithLogging(() => unitOfWorkService.SiteService.ProcessFilesAsync22(), "ProcessFilesAsync22", _context);
        //                  await ExecuteWithLogging(() => unitOfWorkService.SiteService.ProcessFilesAsync23(), "ProcessFilesAsync23", _context);
        //                  await ExecuteWithLogging(() => unitOfWorkService.SiteService.ProcessFilesAsync24(), "ProcessFilesAsync24", _context);
        //                  await ExecuteWithLogging(() => unitOfWorkService.SiteService.ProcessFilesAsync25(), "ProcessFilesAsync25", _context);
        //                  await unitOfWorkService.SiteService.GetFilteredLogsBackGroundServices();
        //                  await unitOfWorkService.SiteService.GetHistoryFile();
        //                  await unitOfWorkService.UserService.GetSecurityLogsFile();

        //                  Console.WriteLine("✅ جميع العمليات تم تنفيذها بنجاح.");
        //              }
        //              catch (Exception ex)
        //              {
        //                  Console.WriteLine($"❌ خطأ عام أثناء تنفيذ المهام: {ex.Message}");
        //                  await LogErrorToDb(ex, "GeneralBackgroundTaskError", _context);
        //              }
        //          }
        //      }
        //  }

        //  private async Task ExecuteWithLogging(Func<Task> operation, string methodName, ApplicationDbContext context)
        //  {
        //      try
        //      {
        //          Console.WriteLine($"▶️ بدء تنفيذ {methodName}...");
        //          await operation();
        //          Console.WriteLine($"✅ تم تنفيذ {methodName} بنجاح.");
        //      }
        //      catch (Exception ex)
        //      {
        //          Console.WriteLine($"❌ خطأ أثناء تنفيذ {methodName}: {ex.Message}");
        //          await LogErrorToDb(ex, methodName, context);
        //      }
        //  }

        //  private async Task LogErrorToDb(Exception ex, string methodName, ApplicationDbContext context)
        //  {
        //      try
        //      {
        //          var errorRecord = new TLIimportSheet
        //          {
        //              UniqueName = Guid.NewGuid().ToString(),
        //              SheetName = methodName,
        //              RefTable = "N/A",
        //              IsLib = false,
        //              ErrMsg = ex.Message,
        //              CreatedAt = DateTime.UtcNow,
        //              IsDeleted = false
        //          };

        //          context.TLIimportSheets.Add(errorRecord);
        //          await context.SaveChangesAsync();

        //          Console.WriteLine($"📝 تم تسجيل الخطأ في الجدول: {methodName} - {ex.Message}");
        //      }
        //      catch (Exception logEx)
        //      {
        //          Console.WriteLine($"⚠️ فشل تسجيل الخطأ في الجدول: {logEx.Message}");
        //      }
        //  }




        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await ProcessSiteServices();
                    await Task.Delay(TimeSpan.FromHours(24), stoppingToken); // تأخير 24 ساعة فقط
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ خطأ عام أثناء تنفيذ المهام: {ex.Message}");
                    await Task.Delay(TimeSpan.FromHours(1), stoppingToken); // انتظر ساعة قبل إعادة المحاولة
                }
            }
        }

        private async Task ProcessSiteServices()
        {
            using var scope = _services.CreateScope();
            var unitOfWorkService = scope.ServiceProvider.GetRequiredService<IUnitOfWorkService>();
            var _context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            Console.WriteLine("🔹 بدء تنفيذ جميع العمليات بالتسلسل...");
            var startTime = DateTime.UtcNow;

            // تنفيذ العمليات واحدة تلو الأخرى مع تسجيل أي أخطاء في الجدول
            await ExecuteWithLogging(() => unitOfWorkService.SiteService.GetSMIS_Site(), "GetSMIS_Site", _context);
            await ExecuteWithLogging(() => unitOfWorkService.SiteService.ProcessFilesAsync1(), "ProcessFilesAsync1", _context);
            await ExecuteWithLogging(() => unitOfWorkService.SiteService.ProcessFilesAsync2(), "ProcessFilesAsync2", _context);
            await ExecuteWithLogging(() => unitOfWorkService.SiteService.ProcessFilesAsync3(), "ProcessFilesAsync3", _context);
            await ExecuteWithLogging(() => unitOfWorkService.SiteService.ProcessFilesAsync4(), "ProcessFilesAsync4", _context);
            await ExecuteWithLogging(() => unitOfWorkService.SiteService.ProcessFilesAsync5(), "ProcessFilesAsync5", _context);
            await ExecuteWithLogging(() => unitOfWorkService.SiteService.ProcessFilesAsync6(), "ProcessFilesAsync6", _context);
            await ExecuteWithLogging(() => unitOfWorkService.SiteService.ProcessFilesAsync7(), "ProcessFilesAsync7", _context);
            await ExecuteWithLogging(() => unitOfWorkService.SiteService.ProcessFilesAsync8(), "ProcessFilesAsync8", _context);
            await ExecuteWithLogging(() => unitOfWorkService.SiteService.ProcessFilesAsync9(), "ProcessFilesAsync9", _context);
            await ExecuteWithLogging(() => unitOfWorkService.SiteService.ProcessFilesAsync10(), "ProcessFilesAsync10", _context);
            await ExecuteWithLogging(() => unitOfWorkService.SiteService.ProcessFilesAsync11(), "ProcessFilesAsync11", _context);
            await ExecuteWithLogging(() => unitOfWorkService.SiteService.ProcessFilesAsync12(), "ProcessFilesAsync12", _context);
            await ExecuteWithLogging(() => unitOfWorkService.SiteService.ProcessFilesAsync13(), "ProcessFilesAsync13", _context);
            await ExecuteWithLogging(() => unitOfWorkService.SiteService.ProcessFilesAsync14(), "ProcessFilesAsync14", _context);
            await ExecuteWithLogging(() => unitOfWorkService.SiteService.ProcessFilesAsync15(), "ProcessFilesAsync15", _context);
            await ExecuteWithLogging(() => unitOfWorkService.SiteService.ProcessFilesAsync16(), "ProcessFilesAsync16", _context);
            await ExecuteWithLogging(() => unitOfWorkService.SiteService.ProcessFilesAsync17(), "ProcessFilesAsync17", _context);
            await ExecuteWithLogging(() => unitOfWorkService.SiteService.ProcessFilesAsync18(), "ProcessFilesAsync18", _context);
            await ExecuteWithLogging(() => unitOfWorkService.SiteService.ProcessFilesAsync19(), "ProcessFilesAsync19", _context);
            await ExecuteWithLogging(() => unitOfWorkService.SiteService.ProcessFilesAsync20(), "ProcessFilesAsync20", _context);
            await ExecuteWithLogging(() => unitOfWorkService.SiteService.ProcessFilesAsync21(), "ProcessFilesAsync21", _context);
            await ExecuteWithLogging(() => unitOfWorkService.SiteService.ProcessFilesAsync22(), "ProcessFilesAsync22", _context);
            await ExecuteWithLogging(() => unitOfWorkService.SiteService.ProcessFilesAsync23(), "ProcessFilesAsync23", _context);
            await ExecuteWithLogging(() => unitOfWorkService.SiteService.ProcessFilesAsync24(), "ProcessFilesAsync24", _context);
            await ExecuteWithLogging(() => unitOfWorkService.SiteService.ProcessFilesAsync25(), "ProcessFilesAsync25", _context);
            await unitOfWorkService.SiteService.GetFilteredLogsBackGroundServices();
            await unitOfWorkService.SiteService.GetHistoryFile();
            await unitOfWorkService.SiteService.GetSecurityLogsFile();

            var endTime = DateTime.UtcNow;
            Console.WriteLine($"✅ جميع العمليات تم تنفيذها بنجاح. الوقت المستغرق: {(endTime - startTime).TotalMinutes} دقيقة");
        }

        private async Task ExecuteWithLogging(Func<Task> action, string operationName, ApplicationDbContext context)
        {
            try
            {
                await action();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ خطأ أثناء تنفيذ {operationName}: {ex.Message}");
                await LogErrorToDb(ex, operationName, context);
            }
        }

        private async Task LogErrorToDb(Exception ex, string methodName, ApplicationDbContext context)
        {
            try
            {
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

                context.TLIimportSheets.Add(errorRecord);
                await context.SaveChangesAsync();

                Console.WriteLine($"📝 تم تسجيل الخطأ في الجدول: {methodName} - {ex.Message}");
            }
            catch (Exception logEx)
            {
                Console.WriteLine($"⚠️ فشل تسجيل الخطأ في الجدول: {logEx.Message}");
            }
        }

        
    }
}
