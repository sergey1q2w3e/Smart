using Microsoft.ServiceBus.Messaging;
using SmartHouseService.DataObjects;
using SmartHouseService.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Azure.Devices;

namespace SmartHouseService.IoT
{
    public static class IoTHubManager
    {
        private static string connectionString = "HostName=myiothubesp8266.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=3+lHjdnCdqguuSF+ghkLWYCY+5gu2aziirph/tYoHPY=";
        private static string iotHubD2cEndpoint = "messages/events";
        private static string deviceID = "esp8266";
        static ServiceClient serviceClient;


        public static void StartReceive()
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

        private async static Task ReceiveMessagesFromDeviceAsync(EventHubReceiver receiver)
        {
            while (true)
            {
                EventData eventData = await receiver.ReceiveAsync();
                if (eventData == null) continue;

                string data = Encoding.UTF8.GetString(eventData.GetBytes());
                Debug.WriteLine(String.Format("Message received: {0}", data));
                int h = int.Parse(data.Substring(0, 2));
                //Parameters p = new Parameters() { Name = "Humidity", Id = Guid.NewGuid().ToString(), Value = h };
                using (var contextService = new MobileServiceContext())
                {
                    Parameters paramChange =
                        contextService.Parameters.Where(p => p.Name == "Humidity").FirstOrDefault<Parameters>();
                    if (paramChange != null)
                    {
                        paramChange.Value = h++;
                        contextService.Entry(paramChange).State = EntityState.Modified;

                    }
                    else
                    {
                        contextService.Parameters.Add(new Parameters()
                        {
                            Name = "Humidity",
                            Id = Guid.NewGuid().ToString(),
                            Value = h
                        });
                    }

                    try
                    {
                        contextService.SaveChanges();
                    }
                    catch (DbUpdateConcurrencyException e)
                    {
                        Debug.WriteLine(String.Format(e.Message + "\n" + e.Entries.Single().ToString()));
                    }
                }
            }
        }

        public async static Task SendMessage(string message)
        {
            if(serviceClient == null) serviceClient = ServiceClient.CreateFromConnectionString(connectionString);
            var commandMessage = new Message(Encoding.UTF8.GetBytes(message));

            await serviceClient.SendAsync(deviceID, commandMessage);
            Debug.WriteLine(String.Format("Send message: {0}", message));
        }
    }
}