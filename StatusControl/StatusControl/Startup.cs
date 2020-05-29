using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using KubeMQ.SDK.csharp.Events;
using KubeMQ.SDK.csharp.Subscription;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Models;
using Newtonsoft.Json;
using StatusControl.Data;
using StatusControl.Services;

namespace StatusControl
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        private IWebHostEnvironment CurrentEnvironment { get; set; }
        private IStatusConsumptionService statusConsumptionService;
        public Startup(IWebHostEnvironment env, IConfiguration configuration)
        {
            CurrentEnvironment = env;
            Configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddJsonFile($"appsettings.{CurrentEnvironment.EnvironmentName}.json")
                .AddEnvironmentVariables()
                .Build();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<StatusControlDBSettings>(
                Configuration.GetSection(nameof(StatusControlDBSettings)));

            services.AddSingleton<IStatusControlDBSettings>(sp =>
                sp.GetRequiredService<IOptions<StatusControlDBSettings>>().Value);

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Status Control", Version = "v1" });
            });

            services.AddControllers()
                .AddNewtonsoftJson(options => options.UseMemberCasing()); ;
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Status Control v1");
            });

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            SetupEventStore();
        }

        private void SetupEventStore()
        {
            var ChannelName = Configuration["StatusControlKubeMQSettings:ChannelName"];
            var ClientID = Configuration["StatusControlKubeMQSettings:ClientID"];
            var KubeMQServerAddress = Configuration["StatusControlKubeMQSettings:KubeMQServerAddress"];

            statusConsumptionService = new StatusConsumptionService(new StatusControlDBSettings()
            {
                ConnectionString = Configuration["StatusControlDBSettings:ConnectionString"],
                DatabaseName = Configuration["StatusControlDBSettings:DatabaseName"],
                StatusConsumptionCollectionName = Configuration["StatusControlDBSettings:StatusConsumptionCollectionName"],
            });
            
            var subscriber = new Subscriber(KubeMQServerAddress);
            SubscribeRequest subscribeRequest = new SubscribeRequest()
            {
                Channel = ChannelName,
                ClientID = ClientID + new Random().Next().ToString(),
                EventsStoreType = EventsStoreType.StartAtSequence,
                EventsStoreTypeValue = 1,
                SubscribeType = SubscribeType.EventsStore
            };

            subscriber.SubscribeToEvents(subscribeRequest, HandleIncomingEvents, HandeIncomingErrors);
        }

        private void HandeIncomingErrors(Exception eventReceive)
        {
            Console.WriteLine(eventReceive.Message);
        }

        private void HandleIncomingEvents(EventReceive eventReceive)
        {
            if (eventReceive != null)
            {
                // Convert submission
                string strMsg = string.Empty;
                var obj = KubeMQ.SDK.csharp.Tools.Converter.FromByteArray(eventReceive.Body);
                Submission submission = (Submission)obj;

                // Save accountConsumption
                statusConsumptionService.PostStatusConsumption(new StatusConsumption()
                {
                    EventSequence = eventReceive.Sequence,
                    RessourceUsage = submission.RessourceUsage,
                    Type = submission.MeteringUnit.Type,
                    UnitOfMeassure = submission.UnitOfMeassure,
                    Location = submission.MeteringUnit.Location,
                    TimeStamp = DateTime.Now
                });
            }
        }
    }
}
