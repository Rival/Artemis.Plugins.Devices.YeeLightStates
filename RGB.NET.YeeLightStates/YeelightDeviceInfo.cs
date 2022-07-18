using System.Collections.Generic;
using RGB.NET.Core;

namespace RGB.NET.YeeLightStates
{
    public class YeelightDeviceInfo : IRGBDeviceInfo
    {
        public RGBDeviceType DeviceType => RGBDeviceType.LedStripe;
        public string Manufacturer => "Yeelight";
        public string DeviceName => "Some Yeelight Device"; //TODO name/numerate
        public string Model => "Some Yeelight Model"; //TODO name/numerate
        public object LayoutMetadata { get; set; }

        private static readonly Dictionary<object, LedId> Mapping = new()
        {
            [0] = LedId.Custom1
        };
        
        internal Dictionary<object, LedId> KeyMapping => Mapping;
    }
}