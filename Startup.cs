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
using AspNetCore.Identity.MongoDbCore;
using Expense.Authentication;
using MongoDB.Bson;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.VisualBasic;

namespace Expense
{
    public class Startup
    {
        private const string SecretKey = "tajnyklucztajnyklucztajnyklucztajnyklucz"; // todo: get this from somewhere secure
        private readonly SymmetricSecurityKey _signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(SecretKey));

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var uri = Configuration["MongoConnectionString"];
            if (uri == null)
            {
                uri = Environment.GetEnvironmentVariable("MongoConnectionString");
            }
            var database = Configuration["MongoDb"];

            services.AddSingleton(s =>
            {
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

            services.AddIdentity<AppUser, ApplicationRole>()
                .AddMongoDbStores<AppUser, ApplicationRole, ObjectId>
                (
                    uri, database
                ).AddDefaultTokenProviders();

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(configureOptions =>
            {
                configureOptions.ClaimsIssuer = "Szef tokenow jwt";
                configureOptions.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = "Szef tokenow jwt",

                    ValidateAudience = true,
                    ValidAudience = "Parobasy",

                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = _signingKey,

                    RequireExpirationTime = false,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };
                configureOptions.SaveToken = true;
            });

            services.AddSingleton<JwtFactory>();


            services.Configure<JwtIssuerOptions>(options =>
            {
                options.Issuer = "Szef tokenow jwt";
                options.Audience = "Parobasy";
                options.SigningCredentials = new SigningCredentials(_signingKey, SecurityAlgorithms.HmacSha256);
            });

            services.AddHttpClient<FacebookAuthService>();
            services.AddCors(options =>
            {
                options.AddDefaultPolicy(
                    builder =>
                    {
                        builder.WithOrigins("http://example.com",
                                            "http://www.contoso.com");
                    });
            });

            services.AddControllers();

            services.AddAuthorization(options =>
            {
                options.AddPolicy("ApiUser", policy => policy.RequireClaim("rol", "api_access"));
            });

            services.AddSingleton<IExpensesDbService, ExpensesMongoDbService>();

            services.AddSingleton<IAuthService, FacebookAuthService>();

            services.Configure<FBLoginConfig>(Configuration.GetSection("FBLoginConfig"));



            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Expense", Version = "v1" });
            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //if (env.IsDevelopment())
            //{
            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Expense v1"));
            //}

            app.UseCors(builder =>
            {
                builder
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader();
            });

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
