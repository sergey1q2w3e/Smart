using HouseService.DataObjects;
using Microsoft.Owin;
using Microsoft.ServiceBus.Messaging;
using Owin;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

[assembly: OwinStartup(typeof(HouseService.Startup))]

namespace HouseService
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureMobileApp(app);
            //Thread receiveThread = new Thread(CreateReceiver);
            //receiveThread.Start();
        
        }
        static EventHubClient eventHubClient = null;
        static EventHubReceiver eventHubReceiver = null;

        private void SetHubs()
        {
            eventHubClient = EventHubClient.CreateFromConnectionString(connectionString, iotHubD2cEndpoint);

            int eventHubPartitionsCount = eventHubClient.GetRuntimeInformation().PartitionCount;
            string partition = EventHubPartitionKeyResolver.ResolveToPartition(device, eventHubPartitionsCount);
            eventHubReceiver = eventHubClient.GetConsumerGroup("$Default").CreateReceiver(partition, DateTime.UtcNow);
        }

        private async void GetData()
        {
            var events = await eventHubReceiver.ReceiveAsync(int.MaxValue);

            var eventData = await eventHubReceiver.ReceiveAsync(TimeSpan.FromSeconds(1));

            if (eventData != null)
            {

                var data = Encoding.UTF8.GetString(eventData.GetBytes());

                if (eventData.Properties.Count > 0)
                {
                    var value = eventData.Properties[""].ToString();
                }
            }
        }
        //private static void CreateReceiver()
        //{
        //    eventHubClient = EventHubClient.CreateFromConnectionString(connectionString, iotHubD2cEndpoint);

        //    var d2cPartitions = eventHubClient.GetRuntimeInformation().PartitionIds;

        //    //CancellationTokenSource cts = new CancellationTokenSource();
        //    var tasks = new List<Task>();
        //    foreach (string partition in d2cPartitions)
        //    {
        //        tasks.Add(ReceiveMessagesFromDeviceAsync(partition));
        //    }
        //    //Task.WaitAll(tasks.ToArray());
        //    //await ReceiveMessagesFromDeviceAsync("0");
        //}
        //private static async Task ReceiveMessagesFromDeviceAsync(string partition)
        //{
        //    var eventHubReceiver = eventHubClient.GetDefaultConsumerGroup().CreateReceiver(partition, DateTime.UtcNow);
        //    while (true)
        //    {
        //        EventData eventData = await eventHubReceiver.ReceiveAsync();
        //        if (eventData == null) continue;

        //        string data = Encoding.UTF8.GetString(eventData.GetBytes());
        //        Parameters p = new Parameters { Name = data, Value = 111 };
        //        //Console.WriteLine("Message received. Partition: {0} Data: '{1}'", partition, data);
        //        //var res = await controller.PostParameters(p);
        //        controller.SimplePostParameters(p);
        //        Thread.Sleep(0);
        //    }
        //}
    }
}