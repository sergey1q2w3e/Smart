using Microsoft.ServiceBus.Messaging;
using SmartHouseService.DataObjects;
using SmartHouseService.Models;
using System;
using System.Configuration;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Devices;
using Microsoft.Azure.NotificationHubs;
using System.Globalization;

namespace SmartHouseService.IoT
{
    public static class IoTHubManager
    {
        //private static string connectionString = "HostName=myiothubesp8266.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=3+lHjdnCdqguuSF+ghkLWYCY+5gu2aziirph/tYoHPY=";
        //private static string iotHubD2cEndpoint = "messages/events";
        //private static string deviceID = "esp8266";
        //private static string notifHubConnectionStr = "Endpoint=sb://myhubnamespace.servicebus.windows.net/;SharedAccessKeyName=DefaultFullSharedAccessSignature;SharedAccessKey=voDbGB7f0c99MG1t+wvRZEMruKapTmrywNB7r05mMss=";
        //private static string notificationHubName = "myhub";
        private static ServiceClient serviceClient;

        public static void StartReceive()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["IoTHubConnectionString"].ConnectionString;
            string iotHubD2CEndpoint = ConfigurationManager.AppSettings["IoTHubEndPoint"];
            var eventHubClient = EventHubClient.CreateFromConnectionString(connectionString, iotHubD2CEndpoint);
            
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
            if(serviceClient == null) serviceClient = ServiceClient.CreateFromConnectionString(ConfigurationManager.ConnectionStrings["IoTHubConnectionString"].ConnectionString);
            var commandMessage = new Message(Encoding.UTF8.GetBytes(message));

            await serviceClient.SendAsync(ConfigurationManager.AppSettings["DeviceID"], commandMessage);
            Debug.WriteLine(String.Format("Send message: {0}", message));
        }

        public static void ParseAndSave(string incMsg)
        {
            Debug.WriteLine(String.Format("Parsing..  {0}", incMsg));
            string[] massParam = incMsg.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < massParam.Length; i++)
            {
                string[] parameter = massParam[i].Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                if (parameter.Length != 2)
                {
                    string strParameter = parameter.Aggregate("", (current, item) => current + item);
                    Debug.WriteLine(String.Format("Incorrect parameter received from IoT: {0}\nCan't parse this",strParameter));
                    continue;
                }
                try
                {
                    switch (parameter[0])
                    {
                        case "H":
                            double h = double.Parse(parameter[1], CultureInfo.InvariantCulture);
                            SaveParameter("Humidity", (int)h);
                            break;
                        case "T":
                            double t = double.Parse(parameter[1], CultureInfo.InvariantCulture);
                            SaveParameter("Temperature", (int)t);
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
                catch(Exception e)
                {
                    Debug.WriteLine(String.Format("{0}: {1}",e.Message,parameter[1]));
                }
            }
        }
        private static void SaveParameter(string name, int value)
        {
            using (var contextService = new MobileServiceContext())
            {
                Parameters paramChange = contextService.Parameters.FirstOrDefault(p => p.Name == name);
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
            string notifHubConnectionStr = ConfigurationManager.ConnectionStrings["NotifHubConnectionString"].ConnectionString;
            string notificationHubName = ConfigurationManager.AppSettings["NotificationHubName"];
            NotificationHubClient hub = NotificationHubClient
                .CreateClientFromConnectionString(notifHubConnectionStr, notificationHubName);

            var windowsToastPayload = @"<toast><visual><binding template=""ToastText01""><text id=""1"">"
                                    + "Внимание! Дверь осталась открытой" + @"</text></binding></visual></toast>";
            try
            {
                // Send the push notification.
                var result = await hub.SendWindowsNativeNotificationAsync(windowsToastPayload);
                Debug.WriteLine(result.State);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }
    }
}