using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MySmartHouse1.Common
{
    public class ViewHouseEntity
    {
        public HouseEntity HouseEntity { get; set; }
        public ViewHouseEntity()
        {
            HouseEntity = new HouseEntity();
        }
    }
}
