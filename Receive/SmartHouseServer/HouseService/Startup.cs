using HouseService.DataObjects;
using Microsoft.Owin;
using Microsoft.ServiceBus.Messaging;
using Owin;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;
using System.Web.Http;

[assembly: OwinStartup(typeof(HouseService.Startup))]

namespace HouseService
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureMobileApp(app);
            Start();
        }
        string connectionString = "HostName=myiothubesp8266.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=3+lHjdnCdqguuSF+ghkLWYCY+5gu2aziirph/tYoHPY=";
        string iotHubD2cEndpoint = "messages/events";

        private void Start()
        {
            var eventHubClient = EventHubClient.CreateFromConnectionString(connectionString, iotHubD2cEndpoint);

            var d2cPartitions = eventHubClient.GetRuntimeInformation().PartitionIds;

            foreach (string partition in d2cPartitions)
            {
                var receiver = eventHubClient.GetDefaultConsumerGroup().
                    CreateReceiver(partition, DateTime.Now);
                ReceiveMessagesFromDeviceAsync(receiver);
            }
            
        }

        async static Task ReceiveMessagesFromDeviceAsync(EventHubReceiver receiver)
        {
            while (true)
            {
                EventData eventData = await receiver.ReceiveAsync();
                if (eventData == null) continue;

                string data = Encoding.UTF8.GetString(eventData.GetBytes());
                Debug.WriteLine(String.Format("Message received: {0}", data));
                int h = int.Parse(data.Substring(0,2));
                //var res = controller.PostParameters(new Parameters() { Value = h, Name = "Humidity", Id = Guid.NewGuid().ToString(), CreatedAt=DateTime.Now, Deleted = false, UpdatedAt = DateTime.Now, Version=new byte[] { } });
            }
        }
    }
}