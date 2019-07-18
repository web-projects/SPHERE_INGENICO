using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IPA.Core.Data.Entity.Other
{
    /// <summary>
    /// Holds information on the device used to get credit card data for a transaction.
    /// </summary>
    public class DeviceModel
    {
        public Entity.Device Device { get; set; }
        public Entity.Model Model { get; set; }
        public Entity.Manufacturer Manufacturer { get; set; }
    }
}
