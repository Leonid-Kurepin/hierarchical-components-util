using ComponentsUtil.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ComponentsUtil
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
