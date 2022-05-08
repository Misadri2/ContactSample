using Amazon.DynamoDBv2;
using Amazon.SQS;
using ContactSample.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

namespace ContactSample
{
    public class Startup
    {
        private readonly string _applicationName;

        [System.Obsolete]
        public Startup(IHostingEnvironment env)
        {
          var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"app.setting.{env.EnvironmentName}.json", optional: true) 
                .AddEnvironmentVariables();

            Configuration = builder.Build();
            _applicationName = Configuration["ApplicationName"];

        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            //Add AWS Services
            services.AddDefaultAWSOptions(Configuration.GetAWSOptions());
            services.AddAWSService<IAmazonDynamoDB>();
            services.AddAWSService<IAmazonSQS>();

            //Add Application Services
            services.AddTransient<ContactQueueService>();
            services.AddTransient<ContactService>();
            services.AddTransient<ContactDbService>();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddAWSProvider(this.Configuration.GetAWSLoggingConfigSection(),
                formatter: (logLevel, message, exception) => JsonConvert.SerializeObject(
                     new Dictionary<string, string>
                     {
                        {"DateTimeUTC",DateTimeOffset.UtcNow.ToString("s")},
                        {"logLevel", logLevel.ToString()},
                        {"message", message?.ToString()},
                        {"exception", Common.GetJsonFromException(exception)}
                     }));
            var logger = loggerFactory.CreateLogger<Program>();
            logger.LogInformation($"new instance of { _applicationName} started.");

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
