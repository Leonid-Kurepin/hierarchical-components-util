using ASKON_TestTask.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ASKON_TestTask
{
    public static class DbConfiguringExtensions
    {
        public static void UpdateDatabase(this ServiceProvider serviceProvider)
        {
            var context = serviceProvider.GetRequiredService<TestTaskContext>();
            context.Database.Migrate();
        }
    }
}
