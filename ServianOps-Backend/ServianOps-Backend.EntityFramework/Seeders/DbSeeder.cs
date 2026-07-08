using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ServianOps_Backend.Core.Entities.Identity;
using ServianOps_Backend.Core.Entities.Saas;
using ServianOps_Backend.EntityFramework.Contexts;

namespace ServianOps_Backend.EntityFramework.Seeders
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(ApplicationDbContext context)
        {
            // Apply any pending migrations automatically
            if (context.Database.GetPendingMigrations().Any())
            {
                await context.Database.MigrateAsync();
            }

            // Seed Plans
            if (!context.Plans.Any())
            {
                context.Plans.AddRange(
                    new Plan
                    {
                        PlanName = "Basic Plan",
                        MaxUsers = 5,
                        MaxProjects = 10,
                        MaxStorageGB = 10,
                        Price = 1000,
                        BillingCycle = "Monthly",
                        IsTrialAvailable = true,
                        TrialDays = 14,
                        IsActive = true,
                        CreationTime = DateTime.UtcNow
                    },
                    new Plan
                    {
                        PlanName = "Premium Plan",
                        MaxUsers = 20,
                        MaxProjects = 50,
                        MaxStorageGB = 100,
                        Price = 5000,
                        BillingCycle = "Monthly",
                        IsTrialAvailable = true,
                        TrialDays = 14,
                        IsActive = true,
                        CreationTime = DateTime.UtcNow
                    },
                    new Plan
                    {
                        PlanName = "Enterprise Plan",
                        MaxUsers = 99999, // Unlimited
                        MaxProjects = 99999, // Unlimited
                        MaxStorageGB = 99999, // Unlimited
                        Price = 10000,
                        BillingCycle = "Monthly",
                        IsTrialAvailable = true,
                        TrialDays = 14,
                        IsActive = true,
                        CreationTime = DateTime.UtcNow
                    }
                );
                
                await context.SaveChangesAsync();
            }

            // Seed SuperAdmin User (Host User with TenantId = null)
            if (!context.Users.Any(u => u.Email == "admin@servianops.com"))
            {
                // Generate Password Hash and Salt for "123qwe"
                var passwordHasher = new ServianOps_Backend.Application.Services.PasswordHasher();
                var hash = passwordHasher.HashPassword("123qwe", out var salt);

                var superAdminUser = new User
                {
                    TenantId = null, // SuperAdmin has no tenant
                    FirstName = "Super",
                    LastName = "Admin",
                    Email = "admin@servianops.com",
                    Phone = "0000000000",
                    ProfileImage = "",
                    PasswordHash = hash,
                    PasswordSalt = salt,
                    IsEmailVerified = true,
                    IsActive = true,
                    CreationTime = DateTime.UtcNow
                };

                context.Users.Add(superAdminUser);
                await context.SaveChangesAsync();
            }
        }
    }
}
