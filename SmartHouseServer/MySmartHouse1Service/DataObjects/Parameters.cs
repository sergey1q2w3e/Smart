using Microsoft.Azure.Mobile.Server;

namespace MySmartHouse1Service.DataObjects
{
    public class Parameters : EntityData
    {
        public string Name { get; set; }

        public int Value { get; set; }
    }
}