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
using System.Web.Http;
using Microsoft.Azure.Devices;
using Microsoft.Azure.Mobile.Server;
using Microsoft.Azure.NotificationHubs;
using Microsoft.Azure.Mobile.Server.Config;

namespace SmartHouseService.IoT
{
    public static class IoTHubManager
    {
        private static string connectionString = "HostName=myiothubesp8266.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=3+lHjdnCdqguuSF+ghkLWYCY+5gu2aziirph/tYoHPY=";
        private static string iotHubD2cEndpoint = "messages/events";
        private static string deviceID = "esp8266";
        static ServiceClient serviceClient;

        public static HttpConfiguration CotrollerConfiguration;


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
                ParseAndSave(data);
                //int h = int.Parse(data.Substring(0, 2));
                //Parameters p = new Parameters() { Name = "Humidity", Id = Guid.NewGuid().ToString(), Value = h };
                
            }
        }

        public async static Task SendParameterMessage(Parameters paramMessage)
        {
            string message = "";
            switch (paramMessage.Name)
            {
                case "FanMode":
                    message += "M:" + paramMessage.Value;
                    break;
                case "FanPower":
                    message += "P:" + paramMessage.Value;
                    break;
                default:
                    Debug.WriteLine("Unknow parameter for send to esp8266");
                    break;
            }
            if(serviceClient == null) serviceClient = ServiceClient.CreateFromConnectionString(connectionString);
            var commandMessage = new Message(Encoding.UTF8.GetBytes(message));

            await serviceClient.SendAsync(deviceID, commandMessage);
            Debug.WriteLine(String.Format("Send message: {0}", message));
        }

        public static void ParseAndSave(string incMsg)
        {
            Debug.WriteLine(String.Format("Parsing..  {0}", incMsg));
            string[] massParam = incMsg.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < massParam.Length; i++)
            {
                string[] parameter = massParam[i].Split(':');
                switch (parameter[0])
                {
                    case "H":
                        int h = int.Parse(parameter[1].Substring(0, 2));
                        SaveParameter("Humidity", h);
                        break;
                    case "T":
                        int t = int.Parse(parameter[1].Substring(0, 2));
                        SaveParameter("Temperature", t);
                        break;
                    case "D":
                        int d = int.Parse(parameter[1]);
                        if (d < 0)
                        {
                            Alarm();
                        }
                        else
                        {
                            SaveParameter("Door", d);
                        }
                        break;
                }
            }
        }
        private static void SaveParameter(string name, int value)
        {
            using (var contextService = new MobileServiceContext())
            {
                Parameters paramChange = contextService.Parameters.Where(p => p.Name == name).FirstOrDefault<Parameters>();
                if (paramChange != null)
                {
                    paramChange.Value = value;
                    contextService.Entry(paramChange).State = EntityState.Modified;
                }
                else
                {
                    contextService.Parameters.Add(new Parameters()
                    {
                        Name = name,
                        Id = Guid.NewGuid().ToString(),
                        Value = value
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

        private static async void Alarm()
        {
            // Get the settings for the server project.
            HttpConfiguration config = CotrollerConfiguration;
            MobileAppSettingsDictionary settings =
                CotrollerConfiguration.GetMobileAppSettingsProvider().GetMobileAppSettings();

            // Get the Notification Hubs credentials for the Mobile App.
            string notificationHubName = settings.NotificationHubName;
            string notificationHubConnection = settings
                .Connections[MobileAppSettingsKeys.NotificationHubConnectionString].ConnectionString;

            // Create the notification hub client.
            NotificationHubClient hub = NotificationHubClient
                .CreateClientFromConnectionString(notificationHubConnection, notificationHubName);

            // Define a WNS payload
            var windowsToastPayload = @"<toast><visual><binding template=""ToastText01""><text id=""1"">"
                                    + "Внимание! Дверь осталась открытой" + @"</text></binding></visual></toast>";
            try
            {
                // Send the push notification.
                var result = await hub.SendWindowsNativeNotificationAsync(windowsToastPayload);

                // Write the success result to the logs.
                config.Services.GetTraceWriter().Info(result.State.ToString());
            }
            catch (System.Exception ex)
            {
                // Write the failure result to the logs.
                config.Services.GetTraceWriter()
                    .Error(ex.Message, null, "Push.SendAsync Error");
            }
        }
    }
}