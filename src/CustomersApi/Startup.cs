using System;
using CustomersApi.DataStore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Shared;

namespace CustomersApi
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddJaeger();

            // Enables OpenTracing instrumentation for ASP.NET Core, CoreFx, EF Core
            services.AddOpenTracing();
            
            services
                .AddDbContext<CustomerDbContext>(options =>
                {
                    var connectionStringBuilder = new SqliteConnectionStringBuilder
                    {
                        DataSource = ":memory:",
                        Mode = SqliteOpenMode.Memory,
                        Cache = SqliteCacheMode.Shared
                    };
                    var connection = new SqliteConnection(connectionStringBuilder.ConnectionString);

                    // Hack: EFCore resets the DB for every connection so we keep the connection open.
                    // This is obviously just demo code :)
                    connection.Open();
                    connection.EnableExtensions(true);

                    options.UseSqlite(connection);
                });
            
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            
            // Load some dummy data into the InMemory db.
            BootstrapDataStore(app.ApplicationServices);

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        private static void BootstrapDataStore(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            
            var dbContext = scope.ServiceProvider.GetRequiredService<CustomerDbContext>();
            dbContext.Seed();
        }
    }
}
