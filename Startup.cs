using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using Expense.Services;
using MongoDB.Driver.Core.Events;

namespace Expense
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
            services.AddSingleton(s =>
            {
                var uri = Configuration["MongoConnectionString"];
                if(uri==null)
                {
                    uri = Environment.GetEnvironmentVariable("MongoConnectionString");
                }
                var database = Configuration["MongoDb"];
                var mongoSettings = MongoClientSettings.FromConnectionString(uri);
                
                //mongoSettings.ClusterConfigurator = cb =>
                //{
                //    cb.Subscribe<CommandStartedEvent>(e =>
                //    {
                //        Console.WriteLine($"{e.CommandName}, {e.Command}");
                //    });
                //};

                var client = new MongoClient(mongoSettings);
				return client.GetDatabase(database);
            });

            services.AddSingleton<IExpensesDbService, ExpensesMongoDbService>();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Expense", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Expense v1"));
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
