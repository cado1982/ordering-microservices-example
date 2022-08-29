using Microsoft.EntityFrameworkCore;
using Ordering.Models;
using Ordering.SyncDataService.Grpc;

namespace Ordering.Data
{
    public static class PrepDb
    {
        public static void PrepPopulation(IApplicationBuilder app, bool isProd)
        {
            using (var serviceScope = app.ApplicationServices.CreateScope())
            {
                var grpcClient = serviceScope.ServiceProvider.GetService<IAccountsDataClient>()!;
                var repo = serviceScope.ServiceProvider.GetService<IOrdersRepository>()!;
                var dbContext = serviceScope.ServiceProvider.GetService<AppDbContext>()!;

                var accounts = grpcClient.GetAllAccounts();

                SaveAccounts(accounts, repo);

                SeedData(dbContext, isProd);
            }
        }

        private static void SeedData(AppDbContext context, bool isProd)
        {
            if (isProd)
            {
                Console.WriteLine("--> Attempting to apply migrations...");
                try
                {
                    context.Database.Migrate();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"--> Could not apply migrations: {ex.Message}");
                    throw;
                }
            }
        }

        private static void SaveAccounts(IEnumerable<Account> accounts, IOrdersRepository repo)
        {
            Console.WriteLine("Seeding new accounts...");

            foreach (var account in accounts)
            {
                if (!repo.ExternalAccountExists(account.ExternalId))
                {
                    repo.CreateAccount(account);
                }
            }

            repo.SaveChanges();
        }
    }
}