using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.OData.Builder;
using Microsoft.AspNet.OData.Extensions;
using Microsoft.AspNet.OData.Formatter;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using Microsoft.OData.Edm;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;

namespace DotNetLambda
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
            services.AddControllers().AddNewtonsoftJson(
                options =>
                {
                    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                    options.SerializerSettings.MaxDepth = 2;
                });
            services.AddOData();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Example REST API",
                    Version = "v1"
                });
                c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "bootstrap.xml"));
            });
            SetSwaggerFormatters(services);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment()) app.UseDeveloperExceptionPage();
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.EnableDependencyInjection();

                endpoints.Expand()
                    .Select()
                    .Count()
                    .Filter()
                    .MaxTop(null)
                    .OrderBy()
                    .MapODataRoute("odata", "odata", GetEdmModel());
            });
            app.UseSwagger();
            // Change the above line to this if client-side tooling doesn't support v3
            // app.UseSwagger(c => c.SerializeAsV2 = true);
            app.UseSwaggerUI(c =>
            {
                string endpointBase =
                    string.IsNullOrEmpty(Environment.GetEnvironmentVariable("AWS_LAMBDA_FUNCTION_NAME"))
                        ? string.Empty
                        : "/Prod";
                c.SwaggerEndpoint($"{endpointBase}/swagger/v1/swagger.json", "Example REST API v1");
                c.RoutePrefix = string.Empty;
            });
        }

        /// <summary>
        /// Creates the EDM model for ODATA using the convention-based builder.
        /// </summary>
        /// <returns>The EDM model.</returns>
        private static IEdmModel GetEdmModel()
        {
            ODataConventionModelBuilder builder = new ODataConventionModelBuilder();
            builder.EntitySet<WeatherForecast>(nameof(WeatherForecast));
            return builder.GetEdmModel();
        }

        /// <summary>
        /// Sets up the input and output formatters for the swagger documentation.
        /// </summary>
        /// <param name="services">The service collection to add to.</param>
        private static void SetSwaggerFormatters(IServiceCollection services)
        {
            services.AddMvcCore(options =>
            {
                foreach (ODataOutputFormatter outputFormatter in options.OutputFormatters
                    .OfType<ODataOutputFormatter>()
                    .Where(formatter => formatter.SupportedMediaTypes.Count == 0))
                {
                    outputFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/odata"));
                }

                foreach (ODataInputFormatter inputFormatter in options.InputFormatters
                    .OfType<ODataInputFormatter>()
                    .Where(formatter => formatter.SupportedMediaTypes.Count == 0))
                {
                    inputFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/odata"));
                }
            });
        }
    }
}
