using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MudBlazor;
using MudBlazor.Services;
using DevExample.Platform.Services;
using Microsoft.AspNetCore.HttpOverrides;
using AspNetCore.Identity.Mongo;
using AspNetCore.Identity.Mongo.Model;
using Blazor.Analytics;

namespace DevExample.Platform
{
    public static class TokenState
    {
        public static string AccessToken { get; set; }
        public static string RefreshToken { get; set; }
    }

    public class TokenProvider
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }

    public class InitialApplicationState
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }

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

            services.AddIdentityMongoDbProvider<MongoUser>(
            mongo =>
            {
                mongo.ConnectionString = ConfigManager.GetVariable("mongodb-connection-identity-string");
                // other options
            });

            services.AddMvc(options => options.EnableEndpointRouting = false).SetCompatibilityVersion(CompatibilityVersion.Version_3_0);
            services.AddRazorPages();
            services.AddServerSideBlazor();
            services.AddHttpClient();
            services.AddHttpContextAccessor();
            services.AddScoped<TokenProvider>();

            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders =
                    ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
            });
            services.AddMudServices(config =>
            {
                config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.TopRight;
                config.SnackbarConfiguration.NewestOnTop = true;
                config.SnackbarConfiguration.ShowCloseIcon = true;
                config.SnackbarConfiguration.VisibleStateDuration = 5000;
                config.SnackbarConfiguration.HideTransitionDuration = 500;
                config.SnackbarConfiguration.ShowTransitionDuration = 500;
                config.SnackbarConfiguration.SnackbarVariant = Variant.Outlined;
                config.SnackbarConfiguration.ClearAfterNavigation = false;
            });

            services.Configure<CookiePolicyOptions>(options =>
                {
                    options.MinimumSameSitePolicy = SameSiteMode.Unspecified;
                    options.OnAppendCookie = cookieContext =>
                        CheckSameSite(cookieContext.Context, cookieContext.CookieOptions);
                    options.OnDeleteCookie = cookieContext =>
                        CheckSameSite(cookieContext.Context, cookieContext.CookieOptions);
                });

            services.AddGoogleAnalytics(ConfigManager.GetVariable("GTAG_ID"));

            // SERVICES
            AccountService accountService = new AccountService();
            TicketService ticket = new TicketService();
            FAQService fAQService = new FAQService();
            PaymentService stripe = new PaymentService();

            var contact = new OpenApiContact()
            {
                Name = "DevExample",
                Email = "support@devexample.io",
                Url = new Uri("http://www.devexample.io")
            };

            var license = new OpenApiLicense()
            {
                Name = "Proprietary",
                Url = new Uri("http://www.devexample.io")
            };

            var info = new OpenApiInfo()
            {
                Version = "v1",
                Title = "DevExample Platform API",
                Description = "",
                TermsOfService = new Uri("http://www.devexample.io"),
                Contact = contact,
                License = license
            };

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", info);
                var securitySchema = new OpenApiSecurityScheme
                {
                    Description = "Using the Authorization header with the Bearer scheme.",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                };

                c.AddSecurityDefinition("Bearer", securitySchema);

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
          {
              { securitySchema, new[] { "Bearer" } }
          });
            });

            services.AddHttpContextAccessor();
        }

        private void CheckSameSite(HttpContext httpContext, CookieOptions options)
        {
            if (options.SameSite == SameSiteMode.None)
            {
                var userAgent = httpContext.Request.Headers["User-Agent"].ToString();
                if (DisallowsSameSiteNone(userAgent))
                {
                    options.SameSite = SameSiteMode.Strict;
                }
            }
        }


        private bool DisallowsSameSiteNone(string userAgent)
        {
            if (string.IsNullOrEmpty(userAgent))
            {
                return false;
            }

            // Cover all iOS based browsers here. This includes:
            // - Safari on iOS 12 for iPhone, iPod Touch, iPad
            // - WkWebview on iOS 12 for iPhone, iPod Touch, iPad
            // - Chrome on iOS 12 for iPhone, iPod Touch, iPad
            // All of which are broken by SameSite=None, because they use the iOS networking stack
            if (userAgent.Contains("CPU iPhone OS 12") || userAgent.Contains("iPad; CPU OS 12"))
            {
                return true;
            }

            // Cover Mac OS X based browsers that use the Mac OS networking stack. This includes:
            // - Safari on Mac OS X.
            // This does not include:
            // - Chrome on Mac OS X
            // Because they do not use the Mac OS networking stack.
            if (userAgent.Contains("Macintosh; Intel Mac OS X 10_14") &&
                userAgent.Contains("Version/") && userAgent.Contains("Safari"))
            {
                return true;
            }

            // Cover Chrome 50-69, because some versions are broken by SameSite=None, 
            // and none in this range require it.
            // Note: this covers some pre-Chromium Edge versions, 
            // but pre-Chromium Edge does not require SameSite=None.
            if (userAgent.Contains("Chrome/5") || userAgent.Contains("Chrome/6"))
            {
                return true;
            }

            return false;
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseCookiePolicy();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.

                app.UseHttpsRedirection();
                app.UseHsts();
            }

            app.UseStaticFiles();

            app.UseRouting();
            app.UseMvcWithDefaultRoute();



            app.UseAuthentication();
            app.UseAuthorization();

            app.UseForwardedHeaders();


            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedProto
            });

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json",
                "DevExample Platform API v1");
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });
        }
    }
}
