using ASPNET_Test.Data;
using Microsoft.EntityFrameworkCore;

namespace ASPNET_Test.Models
{
    public static class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new ApplicationDbContext(
                serviceProvider.GetRequiredService<
                    DbContextOptions<ApplicationDbContext>>()))
            {
                // Look for any movies.
                if (context.MyModel.Any())
                {
                    return;   // DB has been seeded
                }

                context.MyModel.AddRange(
                    new MyModel
                    {
                        name = "Bob",
                        surname = "Bobson"
                    },

                    new MyModel
                    {
                        name = "Alice",
                        surname = "Allison"
                    }
                );
                context.SaveChanges();
            }
        }
    }
}
