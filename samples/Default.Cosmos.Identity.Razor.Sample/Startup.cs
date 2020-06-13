using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Mobsites.Cosmos.Identity;
using IdentityRole = Mobsites.Cosmos.Identity.IdentityRole;
using Microsoft.Azure.Cosmos;

namespace Default.Cosmos.Identity.Razor.Sample
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Register the default storage provider, passing in setup options if any.
            // The default behavior without any setup options is to use the Azure Cosmos DB Emulator with default names for database, container, and partition key path.
            services
                .AddCosmosStorageProvider(options =>
                {
                    //options.ConnectionString defaults to the default Azure Cosmos DB Emulator connection string, which is what is desired here for the sample.
                    options.CosmosClientOptions = new CosmosClientOptions
                    {
                        SerializerOptions = new CosmosSerializationOptions
                        {
                            IgnoreNullValues = false
                        }
                    };
                    options.DatabaseId = "DefaultCosmosIdentity";
                    options.ContainerProperties = new ContainerProperties
                    {
                        Id = "Data",
                        //PartitionKeyPath defaults to "/PartitionKey", which is what is desired for the default setup.
                    };
                });

            // Add Cosmos Identity using the default storage provider and default identity models, passing in Identity options if any.
            services
                .AddDefaultCosmosIdentity(options =>
                {
                    // User settings
                    options.User.RequireUniqueEmail = true;

                    // Password settings
                    options.Password.RequireDigit = true;
                    options.Password.RequiredLength = 8;
                    options.Password.RequireLowercase = true;
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequireUppercase = true;

                    // Lockout settings
                    options.Lockout.AllowedForNewUsers = true;
                    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
                    options.Lockout.MaxFailedAccessAttempts = 5;

                })
                // Add other IdentityBuilder methods.
                .AddDefaultUI()
                .AddDefaultTokenProviders();

            // Add Razor Pages
            services
                .AddRazorPages();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, RoleManager<IdentityRole> roleManager)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
            });

            // Add three roles.
            if (!roleManager.RoleExistsAsync("Admin").Result)
            {
                roleManager.CreateAsync(new IdentityRole
                {
                    Name = "Admin"
                }).Wait();
            }
            if (!roleManager.RoleExistsAsync("Employee").Result)
            {
                roleManager.CreateAsync(new IdentityRole
                {
                    Name = "Employee"
                }).Wait();
            }
            if (!roleManager.RoleExistsAsync("Customer").Result)
            {
                roleManager.CreateAsync(new IdentityRole
                {
                    Name = "Customer"
                }).Wait();
            }
        }
    }
}
