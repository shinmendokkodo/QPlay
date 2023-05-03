using System;
using System.Net.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Play.Common.Extensions;
using Play.Inventory.Service.Clients;
using Play.Inventory.Service.Models.Entities;
using Polly;
using Polly.Timeout;

namespace Play.Inventory.Service
{
    public class Startup
    {
        private const string AllowedOriginSetting = "AllowedOrigin";
        private readonly ILogger logger;
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            using var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.SetMinimumLevel(LogLevel.Information);
                builder.AddConsole();
            });
            logger = loggerFactory.CreateLogger("Startup");
            logger.LogInformation("Hello World");
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMongo()
                .AddMongoRepository<InventoryItem>("inventoryitems")
                .AddMongoRepository<CatalogItem>("catalogitems");

            services.AddMassTransitWithRabbitMq();

            /** 
            * ! Below method contains code to register 'CatalogClient'
            * ! It is a HTTPClient, used for synchronous communication with Catalog Service.
            * ! It also contains code for timeout, exponential back-off and circuit breaker pattern.
            * ! Not being used now. Async communication through queues is implemented.
            */
            // AddCatalogClient(services);

            services.AddControllers();

            services.AddSwaggerGen(c => c.SwaggerDoc("v1", new OpenApiInfo { Title = "Play.Inventory.Service", Version = "v1" }));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Play.Inventory.Service v1"));
                app.UseCors(builder =>
                {
                    builder.WithOrigins(Configuration[AllowedOriginSetting])
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => endpoints.MapControllers());
        }

        private void AddCatalogClient(IServiceCollection services)
        {
            Random jitter = new();

            services.AddHttpClient<CatalogClient>(client => client.BaseAddress = new Uri("https://localhost:5001"))
            .AddTransientHttpErrorPolicy(builder => builder.Or<TimeoutRejectedException>().WaitAndRetryAsync
            (
                // * Exponential back-off
                5,
                retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)) + TimeSpan.FromMilliseconds(jitter.Next(0, 1000)),
                onRetry: (outcome, timeSpan, retryAttempt, context) => logger.LogWarning($"Delaying for {timeSpan.TotalSeconds} seconds, then making retry {retryAttempt}")
            ))
            .AddTransientHttpErrorPolicy(builder => builder.Or<TimeoutRejectedException>().CircuitBreakerAsync
            (
                // * Circuit breaker pattern
                3,
                TimeSpan.FromSeconds(15),
                onBreak: (outcome, timeSpan) => logger.LogWarning($"Opening the circuit for {timeSpan} seconds."),
                onReset: () => logger.LogWarning("Circuit is closed.")
            ))
            .AddPolicyHandler(Policy.TimeoutAsync<HttpResponseMessage>(TimeSpan.FromSeconds(1)));
        }
    }
}
