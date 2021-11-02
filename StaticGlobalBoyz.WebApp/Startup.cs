using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StaticGlobalBoyz.DataAccessLibrary;
using StaticGlobalBoyz.DataAccessLibrary.Data;
using StaticGlobalBoyz.WebApp.Data;
using StaticGlobalBoyz.WebApp.Models;
using StaticGlobalBoyz.WebApp.Services;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace StaticGlobalBoyz.WebApp
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            //services.AddCertificateForwarding(options =>
            //{
            //    options.CertificateHeader = "X-SSL-CERT";
            //    options.HeaderConverter = (headerValue) =>
            //    {
            //        X509Certificate2 clientCertificate = null;

            //        if (!string.IsNullOrWhiteSpace(headerValue))
            //        {
            //            byte[] bytes = StringToByteArray(headerValue);
            //            clientCertificate = new X509Certificate2(bytes);
            //        }

            //        return clientCertificate;
            //    };
            //});
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));

            services.AddDatabaseDeveloperPageExceptionFilter();

            services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();

            services.AddServerSideBlazor();
            services.AddControllersWithViews();

            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 8;
                options.Password.RequiredUniqueChars = 1;

                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 3;
                options.Lockout.AllowedForNewUsers = true;

                options.User.AllowedUserNameCharacters =
                "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+()";
                options.User.RequireUniqueEmail = true;
            });

            services.ConfigureApplicationCookie(options =>
            {
                options.Cookie.HttpOnly = true;
                options.ExpireTimeSpan = TimeSpan.FromDays(1);
                options.LoginPath = "/Identity/Account/Login";
                options.AccessDeniedPath = "/Administration/AccessDenied";
                options.SlidingExpiration = true;
                options.Cookie.SameSite = SameSiteMode.None;
                options.ReturnUrlParameter = CookieAuthenticationDefaults.ReturnUrlParameter;
            });
            services.Configure<DataProtectionTokenProviderOptions>(o => o.TokenLifespan = TimeSpan.FromHours(3));
            services.Configure<PasswordHasherOptions>(options =>
                    options.CompatibilityMode = PasswordHasherCompatibilityMode.IdentityV2);
            services.AddTransient<MongoDbDataAccess>();

            services.AddScoped(_ => new DbInfo("SgbDB", "MongoDbConnection"));
            services.AddHttpContextAccessor();

            services.AddAuthentication()
                .AddFacebook(options =>
                {
                    options.AppId = Configuration.GetValue<string>("Facebook:AppId");
                    options.AppSecret = Configuration.GetValue<string>("Facebook:AppSecret");
                    options.CallbackPath = new PathString("/signin-facebook");
                })
                .AddGoogle(options=>
                {
                    options.ClientId = Configuration.GetValue<string>("Google:ClientId");
                    options.ClientSecret = Configuration.GetValue<string>("Google:ClientSecret");
                    options.CallbackPath = new PathString("/signin-google");
                })
                .AddCookie();

            services.AddSession();

            services.AddTransient<IEmailSender, EmailSender>();

            var authMessageSenderOptions = new AuthMessageSenderOptions
            {
                SendGridUser = Configuration.GetValue<string>("SendGrid:SendGridUser"),
                SendGridKey = Configuration.GetValue<string>("SendGrid:SendGridKey")
            };
            services.AddSingleton(authMessageSenderOptions);
            var mailKitOptions = new MailKitOptions
            {
                UseDefaultCredentials = false,
                EnableSsl = true,
                Username = Configuration.GetValue<string>("MailKit:Username"),
                Password = Configuration.GetValue<string>("MailKit:Password"),
                Host = Configuration.GetValue<string>("MailKit:Host"),
                Port = Convert.ToInt32(Configuration.GetValue<string>("MailKit:Port"))
            };
            services.AddSingleton(mailKitOptions);
            services.AddTransient<MailKitSender>();
            services.Configure<StripeSettings>(Configuration.GetSection("Stripe"));
            services.AddTransient<JsonFileCountriesService>();
            services.AddTransient<DataService>();
            services.AddHttpClient();
            services.AddDataProtection();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider services)
        {
            StripeConfiguration.ApiKey = Configuration.GetValue<string>("Stripe:SecretKey");
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseSession();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Products}/{action=Index}/{title?}/{id?}/{format?}");
                endpoints.MapRazorPages();
                endpoints.MapBlazorHub();
            });
        }
        private static byte[] StringToByteArray(string hex)
        {
            int NumberChars = hex.Length;
            byte[] bytes = new byte[NumberChars / 2];

            for (int i = 0; i < NumberChars; i += 2)
            {
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            }

            return bytes;
        }
    }
}
