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
using Microsoft.AspNetCore.Antiforgery;

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

            services.AddSingleton<JwtFactory>();
            

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

                    RequireExpirationTime = true,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };
                configureOptions.SaveToken = true;
            });

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

            services.AddAuthorization(options =>
            {
                options.AddPolicy("ApiUser", policy => policy.RequireClaim("rol", "api_access"));
            });

            services.AddSingleton<IExpensesDbService, ExpensesMongoDbService>();

            services.AddSingleton<IAuthService, FacebookAuthService>();

            services.Configure<FBLoginConfig>(Configuration.GetSection("FBLoginConfig"));

            services.AddMvc(options =>
            {
                options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
            });

            services.AddAntiforgery(options =>
            {
                options.HeaderName = "X-XSRF-TOKEN";
                options.SuppressXFrameOptionsHeader = false;
            });

            services.AddControllersWithViews();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Expense", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IAntiforgery antiforgery)
        {
            //if (env.IsDevelopment())
            //{
            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Expense v1");
                c.Interceptors.RequestInterceptorFunction = "(req) => {"
                + "var getCookieValue = function(key) {"
                    + "var equalities = document.cookie.split('; ');"
                + " for (var i = 0; i < equalities.length; i++)"
                + "{"
           + "if (!equalities[i])"
           + "{"
               + "continue;"
           + "}"

           + "var splitted = equalities[i].split('=');"
           + "if (splitted.length !== 2)"
           + "{"
               + "continue;"
           + "}"

           + "if (decodeURIComponent(splitted[0]) === key)"
           + "{"
               + "return decodeURIComponent(splitted[1] || '');"
           + "}"
        + "}"
        + "return null;"
        + "};"
                + "req.headers['X-XSRF-TOKEN']=getCookieValue('XSRF-TOKEN'); return req; }";
               // "(req) => {req.headers['X-XSRF-TOKEN'] = browser.cookies.get('XSRF-TOKEN');return req;}";
            });
            //}

            app.UseRouting();

            app.UseCors(builder =>
            {
                builder
                .WithOrigins("http://localhost:4200")
                .AllowCredentials()
                .AllowAnyMethod()
                .AllowAnyHeader();
            });
            //Add jwt cookie as auth header
            app.Use(async (context, next) =>
            {
                context.Request.Headers.Add("Authorization", $"Bearer {context.Request.Cookies["jwt"] ?? "none"}");
                await next();
            });

            app.UseAuthentication();
            
            app.UseAuthorization();

            app.Use(async (context, next) =>
            {
                if (context.Request.Method.Equals("GET"))
                {
                    var tokens = antiforgery.GetAndStoreTokens(context);
                    context.Response.Cookies.Append("XSRF-TOKEN", tokens.RequestToken, new Microsoft.AspNetCore.Http.CookieOptions
                    {
                        HttpOnly = false
                    });
                }
                await next();
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
