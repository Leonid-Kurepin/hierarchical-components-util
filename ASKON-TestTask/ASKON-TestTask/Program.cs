using ASKON_TestTask.Forms;
using ASKON_TestTask.Persistence;
using ASKON_TestTask.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Windows.Forms;

namespace ASKON_TestTask
{
    static class Program
    {
        // Should be moved from code
        private const string _connectionString =
            @"Server=.\SQLEXPRESS;Database=ASKON-TestTaskDb;Trusted_Connection=True;";

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
                options.UseSqlServer(_connectionString);
            });

            services.AddScoped<MainForm>();

            services.AddScoped<FormsHelper>();

            services.AddScoped<IDetailService, DetailService>();
        }
    }
}
