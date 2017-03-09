using Microsoft.Azure.Mobile.Server;

namespace SmartHouseService.DataObjects
{
    public class Parameters : EntityData
    {
        public string Name { get; set; }
        public int Value { get; set; }
    }
}