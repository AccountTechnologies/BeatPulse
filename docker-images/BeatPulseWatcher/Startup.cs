using System;
using System.Linq;
using BeatPulse;
using BeatPulse.BeatPulse;
using BeatPulse.UI;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BeatPulseWatcher
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
            var beatPulseSection = Configuration.GetSection("BeatPulse");  // enable Configuration Services
            services.Configure<BeatPulseConfiguration>(beatPulseSection);

            var beatPulseConfiguration = beatPulseSection.Get<BeatPulseConfiguration>();

            services.AddBeatPulseUI();
            services.AddCors();
            services.AddBeatPulse(setup =>
            {
                if (beatPulseConfiguration.SqlServers?.Entries != null)
                {
                    foreach (var entry in beatPulseConfiguration.SqlServers.Entries)
                    {
                        setup.AddSqlServer(connectionString: entry.Connection, name: entry.Name, healthQuery: entry.Query, defaultPath: entry.Path);
                    }
                }

                if (beatPulseConfiguration.UrlGroups?.Entries != null)
                {
                    foreach (var entry in beatPulseConfiguration.UrlGroups.Entries)
                    {
                        setup.AddUrlGroup(uris: entry.Entries.Select(c => new Uri(c)).ToArray(), name: entry.Name, defaultPath: entry.Path);
                    }
                }

                if (beatPulseConfiguration.BeatPulseEndPoints?.Entries != null)
                {
                    foreach (var entry in beatPulseConfiguration.BeatPulseEndPoints.Entries)
                    {
                        setup.AddBeatPulseGroup(uri: new Uri(entry.Uri), name: entry.Name, defaultPath: entry.Path);
                    }
                }
            });
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseBeatPulseUI();

            app.UseBeatPulse(setup => { }, builder =>
            {
                builder.UseCors(setup =>
                {
                    setup.AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowAnyOrigin()
                        .AllowCredentials();
                });
            });
        }

    }
}
