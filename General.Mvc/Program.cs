using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using General.Core.Data;
using General.Entities;
using General.Services;
using General.Services.LibrarySeat;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace General.Mvc
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = BuildWebHost(args);

            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var context = services.GetRequiredService<GeneralDbContext>();
                    context.Database.Migrate();
                    SysUserData.Initialize(services);
                    LibrarySeatData.Initialize(services);


                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred seeding the DB.");
                }
            }

            //var context1 = host.Services.CreateScope().ServiceProvider.GetRequiredService<GeneralDbContext>();
            //IRepository<LibrarySeat> seatRepository = new EfRepository<LibrarySeat>(context1);
            //IRepository<OrderDetail> orderRepository = new EfRepository<OrderDetail>(context1);
            //IRepository<SysUser> userRepository = new EfRepository<SysUser>(context1);
            //UpdateLibrarySeatService seatService = new UpdateLibrarySeatService(seatRepository, orderRepository,userRepository);
            //seatService.Run();

            host.Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseIISIntegration()
                .UseKestrel()
            .UseApplicationInsights()
            .UseContentRoot(Directory.GetCurrentDirectory())
                .UseStartup<Startup>()
                .Build();
    }
}
