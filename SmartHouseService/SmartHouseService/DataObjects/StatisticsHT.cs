using Microsoft.Azure.Mobile.Server;
using System;

namespace SmartHouseService.DataObjects
{
    public class StatisticsHT : EntityData
    {
        public int Humidity { get; set; }
        public DateTime HumidityDateTime { get; set; }
        public int Temperature { get; set; }
        public DateTime TemperatureDateTime { get; set; }
    }
}