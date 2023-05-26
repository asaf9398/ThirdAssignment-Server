using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ThirdAssignment_Server
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

            // Configure Serilog
            Log.Logger = new LoggerConfiguration().Enrich.WithProperty("request-logger", "requestsLogger")
                .WriteTo.File("\\logs\\requests.log") // Path to the first log file
                .CreateLogger();

            // Add the first logger
            services.AddLogging(builder =>
            {
                builder.AddSerilog(dispose: true); // Dispose the logger when the application shuts down
            });

            // Configure Serilog for the second logger
            Log.Logger = new LoggerConfiguration().Enrich.WithProperty("todo-logger", "toDoLogger")
                .WriteTo.File("\\logs\\todos.log") // Path to the second log file
                .CreateLogger();

            // Add the second logger
            services.AddLogging(builder =>
            {
                builder.AddSerilog(dispose: true); // Dispose the logger when the application shuts down
            });



            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "ThirdAssignment_Server", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //if (env.IsDevelopment())
            //{
            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "ThirdAssignment_Server v1"));
            //}

       

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
