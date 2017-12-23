using Microsoft.Azure.Mobile.Server;
using System;

namespace SmartHouseService.DataObjects
{
    public class StatisticsHTs : EntityData
    {
        public int Humidity { get; set; }
        public DateTime ValueDateTime { get; set; }
        public int Temperature { get; set; }
    }
}