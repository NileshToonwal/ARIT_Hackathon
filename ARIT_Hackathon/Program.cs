using ARIT_Hackathon.Extentions;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using NLog;
using System.Text;


using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Microsoft.AspNetCore.HttpOverrides;
using System.IO;
using NLog;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;


namespace ARIT_Hackathon
{
    public class Program
    {
        public IConfiguration Configuration { get; }
        public Program(IConfiguration configuration)
        {
            LogManager.LoadConfiguration(String.Concat(Directory.GetCurrentDirectory(), "/nlog.config"));
            Configuration = configuration;
        }
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.


            builder.Services.AddControllersWithViews();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();


            app.MapControllerRoute(
                name: "default",
                pattern: "{controller}/{action=Index}/{id?}");

            app.MapFallbackToFile("index.html");

            app.Run();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //services.ConfigureCors();

            //services.ConfigureIISIntegration();

            services.ConfigureLoggerService();
            LogManager.Configuration.Variables["LogsDirPath"] = "c:/";


            services.ConfigureSqlContext(Configuration);

            services.ConfigureRepositoryWrapper();


            //services.AddScoped<IViewRenderService, ViewRenderService>();

            ////Access UCC Batch Upload 
            //var appSettings = Configuration.GetSection("AppSettings");
            //services.Configure<FilePathExtended>(appSettings);

            //services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            //services.Configure<TokenManagement>(Configuration.GetSection("tokenManagement"));
            //var token = Configuration.GetSection("tokenManagement").Get<TokenManagement>();
            //var secret = Encoding.ASCII.GetBytes(token.Secret);

            //services.AddAuthentication(x =>
            //{
            //    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            //    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            //}).AddJwtBearer(x =>
            //{
            //    x.RequireHttpsMetadata = false;
            //    x.SaveToken = true;
            //    x.TokenValidationParameters = new TokenValidationParameters()
            //    {
            //        ValidateIssuerSigningKey = true,
            //        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(token.Secret)),
            //        ValidIssuer = token.Issuer,
            //        ValidAudience = token.Audience,
            //        ValidateIssuer = false,
            //        ValidateAudience = false
            //    };
            //});

           

            //services.AddScoped<IAuthenticateService, TokenAuthenticationService>();
            //// In production, the Angular files will be served from this directory
            //services.AddSpaStaticFiles(configuration =>
            //{
            //    configuration.RootPath = "ClientApp/dist";

            //});
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        //public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        //{
        //    if (env.IsDevelopment())
        //    {
        //        app.UseDeveloperExceptionPage();
        //    }
        //    else
        //    {
        //        app.UseExceptionHandler("/Error");
        //        // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        //        app.UseHsts();
        //    }

        //    app.UseCors("CorsPolicy");

        //    app.UseForwardedHeaders(new ForwardedHeadersOptions
        //    {
        //        ForwardedHeaders = ForwardedHeaders.All
        //    });

        //    app.Use(async (context, next) =>
        //    {
        //        await next();

        //        if (context.Response.StatusCode == 404
        //            && !Path.HasExtension(context.Request.Path.Value))
        //        {
        //            context.Request.Path = "/index.html";
        //            await next();
        //        }
        //    });

        //    app.UseHttpsRedirection();
        //    app.UseStaticFiles();
        //    app.UseSpaStaticFiles();
        //    app.UseAuthentication();
        //    app.UseMiddleware<GExceptionMiddleware>();
        //    app.UseMvc(routes =>
        //    {
        //        routes.MapRoute(
        //            name: "default",
        //            template: "{controller}/{action=Index}/{id?}");
        //    });

        //    app.UseSpa(spa =>
        //    {
        //        // To learn more about options for serving an Angular SPA from ASP.NET Core,
        //        // see https://go.microsoft.com/fwlink/?linkid=864501

        //        spa.Options.SourcePath = "ClientApp";

        //        if (env.IsDevelopment())
        //        {
        //            //spa.UseAngularCliServer(npmScript: "start");
        //            spa.UseProxyToSpaDevelopmentServer("http://localhost:4200");
        //        }
        //    });
        //}
    }
}
