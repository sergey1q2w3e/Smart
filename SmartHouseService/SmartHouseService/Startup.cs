using Microsoft.Owin;
using Microsoft.ServiceBus.Messaging;
using Owin;
using SmartHouseService.DataObjects;
using System;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

[assembly: OwinStartup(typeof(SmartHouseService.Startup))]

namespace SmartHouseService
{
    public partial class Startup
    {
        string connectionString = "HostName=myiothubesp8266.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=3+lHjdnCdqguuSF+ghkLWYCY+5gu2aziirph/tYoHPY=";
        string iotHubD2cEndpoint = "messages/events";
        public void Configuration(IAppBuilder app)
        {
            ConfigureMobileApp(app);
            Start();
        }
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
                int h = int.Parse(data.Substring(0, 2));
                Parameters p = new Parameters() { Name = "Humidity", Id = Guid.NewGuid().ToString(), Value = h };
                
                //contextService.Set<Parameters>().Add(p);
                //contextService.SaveChanges();
            }
        }
    }
}