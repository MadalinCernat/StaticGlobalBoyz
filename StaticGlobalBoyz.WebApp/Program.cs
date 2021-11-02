using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Https;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace StaticGlobalBoyz.WebApp
{
    public class Program
    {
        private static X509Certificate2 _certificate;
        public static void Main(string[] args)
        {
            //var config = new ConfigurationBuilder()
            //            .AddJsonFile("appsettings.json", optional: false)
            //            .Build();
            //string certificateFileName = config.GetValue<string>("Kestrel:Endpoints:Https:Certificate:Path");
            //string certificatePassword = config.GetValue<string>("Kestrel:Endpoints:Https:Certificate:Password");

            //var certificate = new X509Certificate2("sgb.pfx", certificatePassword);
            //_certificate = certificate;
            string certThumbprint = "A151588FEADA555A63640BE046005CCC346C8EF8";
            bool validOnly = false;

            using (X509Store certStore = new X509Store(StoreName.My, StoreLocation.CurrentUser))
            {
                certStore.Open(OpenFlags.ReadOnly);

                X509Certificate2Collection certCollection = certStore.Certificates.Find(
                                            X509FindType.FindByThumbprint,
                                            // Replace below with your certificate's thumbprint
                                            certThumbprint,
                                            validOnly);
                // Get the first cert with the thumbprint
                X509Certificate2 cert = certCollection.OfType<X509Certificate2>().FirstOrDefault();
                _certificate = cert;
                certStore.Close();
            }
            
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseIISIntegration();
                    //webBuilder.UseKestrel((hostingContext, options) =>
                    //{
                    //    if (hostingContext.HostingEnvironment.IsDevelopment())
                    //    {
                    //        options.ListenAnyIP(9000);
                    //        options.ListenAnyIP(9001, listenOptions =>
                    //        {
                    //            listenOptions.UseHttps(_certificate);
                    //        });
                    //    }
                    //});
                    webBuilder.UseContentRoot(Directory.GetCurrentDirectory());
                    webBuilder.UseStartup<Startup>();
                });

    }
}
