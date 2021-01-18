using System.IO;
using System.IO.Compression;
using System.Text.Json.Serialization;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.PlatformAbstractions;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using Serilog;
using Serilog.Events;
using Training.Api.Controllers.Base;
using Training.Api.Middleware;
using Training.Api.Services;
using Training.Api.Services.Background.Quartz;
using Training.Configuration;
using Training.Dal.Context;
using IApplicationLifetime = Microsoft.Extensions.Hosting.IApplicationLifetime;

namespace Training.Api
{
    public class Startup
    {
        public Startup()
        {
            var loggerConfiguration = new LoggerConfiguration()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Information)
                .MinimumLevel.Override("System", LogEventLevel.Warning)
                .MinimumLevel.Override("Quartz", LogEventLevel.Information)
                .Enrich.FromLogContext()
                .MinimumLevel.Debug()
                .WriteTo.File("logs\\log_.log", rollingInterval: RollingInterval.Day);

            if (Settings.Current.BuildConfiguration == BuildConfiguration.Development)
            {
                loggerConfiguration = loggerConfiguration
                    .WriteTo.Console();
            }

            if (!string.IsNullOrWhiteSpace(Settings.Current.SeqUrl))
            {
                loggerConfiguration = loggerConfiguration.WriteTo.Seq(Settings.Current.SeqUrl);
            }

            Log.Logger = loggerConfiguration.CreateLogger();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<ServiceContextFilterAttribute>();

            // setup database context
            services.AddDbContextPool<DataContext>(SetupDatabaseContext);

            // add service for accessing the http context
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            // add automapper
            services.AddAutoMapper();

            // register services
            services.RegisterServices();

            // schedule jobs
            services.ScheduleJobs();

            // setup API controller
            services
                .AddMvcCore(options => options.EnableEndpointRouting = false)
                //.AddAuthorization()
                .AddApiExplorer()
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                    options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                    options.SerializerSettings.Converters.Add(new StringEnumConverter());
                });


            //AddAuthentication(services);

            // setup documentation
            services.AddSwaggerGen(o =>
            {
                o.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "API",
                    Version = "v1"
                });

                var filePath = Path.Combine(PlatformServices.Default.Application.ApplicationBasePath,
                    "Training.Api.xml");
                o.IncludeXmlComments(filePath);
            });

            services.AddSwaggerGenNewtonsoftSupport();


            // setup compression
            services.Configure<GzipCompressionProviderOptions>(options => options.Level = CompressionLevel.Optimal);
            services.AddResponseCompression();

            services.AddCors();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory, IApplicationLifetime appLifetime)
        {
            // logging
            loggerFactory.AddSerilog();
            appLifetime.ApplicationStopped.Register(Log.CloseAndFlush);

            // authentication
            //app.UseAuthentication();

            // in order to be able to log the username, the next step is added after the addition of the authentication middleware
            app.UseMiddleware<SerilogMiddleware>();

            // global exception handling
            app.UseMiddleware<ExceptionMiddleware>();

            // compression
            app.UseResponseCompression();

            // cors
            app.UseCors(o =>
            {
                o.AllowAnyHeader()
                    .WithExposedHeaders(
                        "Content-Disposition",
                        "Set-Cookie",
                        ".AspNetCore.Identity.Application",
                        ".AspNetCore.Session",
                        "X-ImportId")
                    //.AllowCredentials()
                    .AllowAnyMethod()
                    .AllowAnyOrigin()
                    .SetIsOriginAllowed(x => true)
                    .SetIsOriginAllowedToAllowWildcardSubdomains();
            });

            // url rewrite
            app.ApplyUrlRewrite();

            // webapi
            app.UseMvc();

            // enable static files
            app.UseStaticFiles();

            // documentation
            app.UseSwagger(o => o.RouteTemplate = "docs/{documentName}/swagger.json");
            app.UseSwaggerUI(o =>
            {
                o.RoutePrefix = "docs";
                o.SwaggerEndpoint("/docs/v1/swagger.json", "API V1");
            });

            Log.Logger.Information($"Current build configuration: {Settings.CurrentBuildConfiguration.ToString()}");
        }

        protected virtual void SetupDatabaseContext(DbContextOptionsBuilder options)
        {
            options
                .UseLazyLoadingProxies()
                .UseSqlServer(Settings.Current.ConnectionString);
        }

        protected virtual void AddAuthentication(IServiceCollection services)
        {
            //services.AddAuthentication("Bearer")
            //    .AddIdentityServerAuthentication(options =>
            //    {
            //        options.Authority = Settings.Current.AuthenticationAuthority;
            //        options.ApiName = Settings.Current.AuthenticationScope;
            //        options.RequireHttpsMetadata = false;
            //    });
        }

    }
}
