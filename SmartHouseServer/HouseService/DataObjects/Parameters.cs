using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Azure.Mobile.Server;

namespace HouseService.DataObjects
{
    public class Parameters : EntityData
    {
        public string Name { get; set; }
        public int Value { get; set; }

    }
}