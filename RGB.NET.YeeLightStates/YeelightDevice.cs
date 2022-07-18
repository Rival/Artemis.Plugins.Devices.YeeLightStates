using System.Collections.Generic;
using RGB.NET.Core;

namespace RGB.NET.YeeLightStates
{
    public class YeelightDevice : AbstractRGBDevice<YeelightDeviceInfo>
    {
        private readonly YeelightAPI.Device _yeeLightDevice;
        private readonly YeelightDeviceInfo _deviceInfo;
        
        internal YeelightDevice(YeelightAPI.Device yeeLightDevice, YeelightDeviceInfo deviceInfo, IUpdateQueue updateQueue)
            : base(deviceInfo, updateQueue)
        {
            _yeeLightDevice = yeeLightDevice;
            _deviceInfo = deviceInfo;
            InitializeLayout();
        }

        private void InitializeLayout()
        {
            int x = 0;
            foreach (var key in _deviceInfo.KeyMapping.Keys)
            {
                if (!_deviceInfo.KeyMapping.TryGetValue(key, out LedId ledId))
                {
                    continue;
                }

                AddLed(ledId, new Point(x, 0), new Size(19), key);
                x += 20;
            }
        }
    }
}