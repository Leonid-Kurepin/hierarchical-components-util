using System;
using System.Windows.Forms;
using ComponentsUtil.Forms;
using ComponentsUtil.Persistence;
using ComponentsUtil.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ComponentsUtil
{
    static class Program
    {
        // Should be removed from code
        private const string _connectionString =
            @"Server=.\SQLEXPRESS;Database=HierarchicalComponentsDb;Trusted_Connection=True;";

        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var services = new ServiceCollection();

            ConfigureServices(services);

            using (ServiceProvider serviceProvider = services.BuildServiceProvider())
            {
                serviceProvider.UpdateDatabase();

                var mainForm = serviceProvider.GetRequiredService<MainForm>();
                Application.Run(mainForm);
            }
        }

        private static void ConfigureServices(ServiceCollection services)
        {
            services.AddDbContext<TestTaskContext>(options =>
            {
                options.UseSqlServer(_connectionString, 
                    x => x.UseHierarchyId());
            });

            services.AddScoped<MainForm>();

            services.AddScoped<FormsHelper>();

            services.AddScoped<IDetailService, DetailService>();
        }
    }
}
