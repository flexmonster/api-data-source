using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NetCoreServer.DataStorages;
using NetCoreServer.JsonConverters;
using System.Text.Json.Serialization;

namespace NetCoreServer
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        private readonly string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy(MyAllowSpecificOrigins,
                builder =>
                {
                    builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
                });
            });
            services.ConfigureFlexmonsterOptions(Configuration);
            services.AddSingleton<IDataStorage, DataStorage>();
            services.AddScoped<IPrepopulatingService, PrepopulatingCacheService>();
            services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new ValuesJsonConverter());
                options.JsonSerializerOptions.Converters.Add(new ColumnTypeJsonConverter());
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });
            services.AddMemoryCache((options) =>
            {
                options.SizeLimit = 100;
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IPrepopulatingService prepopulatingService)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseCors(MyAllowSpecificOrigins);

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            prepopulatingService.Prepopulate();
        }
    }
}