using BusinessLogic;
using BusinessLogic.DataExchange;
using Hangfire;
using Hangfire.RecurringJobs;
using Hangfire.RecurringJobs.Common;
using Hangfire.Storage;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace WebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHttpClient();

            services.AddLogging(config =>
            {
                config.ClearProviders();
                config.AddConsole();
            });

            services.AddHangfire(config =>
            {
                config.UseInMemoryStorage();
            });

            Assembly.GetAssembly(typeof(IRecurringJob))
                .DefinedTypes
                .Where(x => x.IsAssignableTo(typeof(IRecurringJob)) && !x.IsInterface && !x.IsAbstract)
                .ToList()
                .ForEach(x =>
                {
                    services.AddScoped(typeof(IRecurringJob), x);
                });

            Assembly.GetAssembly(typeof(ITaskHandler))
               .DefinedTypes
               .Where(x => x.IsAssignableTo(typeof(ITaskHandler)) && !x.IsInterface && !x.IsAbstract)
               .ToList()
               .ForEach(x =>
               {
                   services.AddScoped(typeof(ITaskHandler), x);
               });

            services.AddScoped<ITaskHandlerDispatcher, TaskHandlerDispatcher>();
            services.AddScoped<DataExchangeJobHandler>();

            services.AddHangfireServer();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider provider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Hello World!");
                });
            });

            app.UseHangfireDashboard();
            ClearHangfireRecurringJobs();
            AddHangfireRecurringJobs(provider);
        }

        private void ClearHangfireRecurringJobs()
        {
            var connection = JobStorage.Current.GetConnection();
            foreach (var item in connection.GetRecurringJobs())
            {
                RecurringJob.RemoveIfExists(item.Id);
            }
        }

        private void AddHangfireRecurringJobs(IServiceProvider provider)
        {
            var jobTemplates = Configuration.GetSection("HangfireRecurringJobs").Get<List<RecurringJobTemplate>>();
            var recurringJobs = provider.GetRequiredService<IEnumerable<IRecurringJob>>();

            foreach (var template in jobTemplates)
            {
                var job = recurringJobs.Single(y => y.GetType().Name == template.JobTypeName);
                RecurringJob.AddOrUpdate(template.Name, () => job.Execute(template, CancellationToken.None), cronExpression: template.Cron, timeZone: null);
            }
        }
    }
}
