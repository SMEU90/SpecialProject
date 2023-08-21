using GrpcServiceSpecialProjectTest.Models.Base;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using GrpcServiceSpecialProjectTest.Repositories.Interfaces;
using Microsoft.Extensions.Hosting;

namespace GrpcServiceSpecialProjectTest
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddGrpc();
            var configuration = new ConfigurationBuilder()
           .AddJsonFile("appsettings.json")
           .Build();
            services.AddDbContext<ContextDB>(option =>
            {
                option.UseNpgsql(configuration.GetConnectionString("Default"));
            });
            services.AddHttpClient();
            services.AddScoped<IProductCountryService, GrpcServiceSpecialProjectTest.Repositories.ProductCountryService>();
            services.AddScoped<IProductQuestionnaireService, GrpcServiceSpecialProjectTest.Repositories.ProductQuestionnaireService>();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                //Add Service
                endpoints.MapGrpcService<CountryService>();
                endpoints.MapGrpcService<QuestionnaireService>();

                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");
                });
            });
        }
    }
}
