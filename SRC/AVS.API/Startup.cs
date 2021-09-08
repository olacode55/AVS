using AVS.Common;
using AVS.Common.Utiities;
using AVS.Core;
using AVS.Repository;
using AVS.Services;
using AVS.Services.Integrations;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AVS.API
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

            services.AddControllers();
            services.AddDbContext<AVS.Data.AppContext>(options =>
                          options.UseSqlServer(
                              Configuration.GetConnectionString("AVSConnectionString")));
            //Register dapper in scope    
            services.AddScoped<IDapper, Dapperr>();
            services.AddScoped<IAVSRepository, AVSRepository>();
            services.AddScoped<IClientRepository, ClientRepository>();
            services.AddScoped<IProviderRepository, ProviderRepository>();
            services.AddScoped<FidelityBank, FidelityBank>();
            services.AddScoped<GTBank, GTBank>();
            services.AddScoped<InputValidationCore, InputValidationCore>();
            services.AddScoped<AccountValidationCore, AccountValidationCore>();
            services.AddScoped<FactoryImplemetation, FactoryImplemetation>();

            services.AddSingleton<ILoggerManager, LoggerManager>();
            services.AddSingleton<AccountValidationResponses, AccountValidationResponses>();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Com.Xpresspayments.AVS.API", Version = "v1" });
            });
            services.AddAutoMapper(typeof(Startup));
            services.AddCors(options =>
            {
                options.AddDefaultPolicy(
                    builder =>
                    {
                        builder.WithOrigins("http://example.com",
                                            "http://www.contoso.com");
                    });
            });
            services.AddMemoryCache();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
     
            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Com.Xpresspayments.AVS.API v1"));

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();
            app.UseCors();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
