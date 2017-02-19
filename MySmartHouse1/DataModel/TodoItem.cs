using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace MySmartHouse1
{
    public class TodoItem
    {
        public string Id { get; set; }

        [JsonProperty(PropertyName = "text")]
        public string Text { get; set; }

        [JsonProperty(PropertyName = "Value")]
        public int Value { get; set; }

        [JsonProperty(PropertyName = "complete")]
        public bool Complete { get; set; }
    }
}
