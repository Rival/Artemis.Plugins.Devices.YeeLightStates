using System;
using System.Timers;
using RGB.NET.Core;
using System.Diagnostics;

namespace RGB.NET.Devices.Bloody
{
    public class YeelightUpdateQueue : UpdateQueue
    {
        private readonly YeeLightAPI.YeeLightDevice _yeeLightDevice;


        private readonly Timer _refreshTimer; // To avoid strip power off by inactivity
        private readonly Timer _connectTimer;
        public YeelightUpdateQueue(IDeviceUpdateTrigger updateTrigger, YeeLightAPI.YeeLightDevice yeeLightDevice) : base(updateTrigger)
        {
            _yeeLightDevice = yeeLightDevice;

            _refreshTimer = new Timer(5000);
            _refreshTimer.Elapsed += _refreshTimer_Elapsed;
            _refreshTimer.AutoReset = true;

            _connectTimer = new Timer(1000);
            _connectTimer.Elapsed += _connectTimer_Elapsed;
            _connectTimer.Start();
        }

        private void _refreshTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            _refreshTimer.Stop();
            _refreshTimer.Start();
            if (_yeeLightDevice.IsConnected())
            {
                ProceedColor(previousColor);
            }
        }

        private void _connectTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (_yeeLightDevice.IsConnected() == false)
            {
                _yeeLightDevice.Connect();
            }
        }

        protected override void Update(in ReadOnlySpan<(object key, Color color)> dataSet)
        {
            _refreshTimer.Stop();
            _refreshTimer.Start();

            if (_yeeLightDevice.IsConnected() == false)
            {
                return;
            }

            foreach ((object key, Color color) item in dataSet)
            {
               UpdateColor(item.color);
               // _yeeLightDevice.SetColor(item.color.GetR(), item.color.GetG(), item.color.GetB()); //TODO move
            }
        }
        
        private void UpdateColor(Color color)
        {
            try
            {
                TryUpdate(color);
            }catch
            {
                Reset();
                TryUpdate(color);
            }
        }

        private Color previousColor = Color.Transparent;
        private int whiteCounter = 10;
        private void TryUpdate(Color targetColor)
        {
            //var sendDelay = Math.Max(5, Global.Configuration.VarRegistry.GetVariable<int>($"{DeviceName}_send_delay"));
            //if (updateDelayStopWatch.ElapsedMilliseconds <= sendDelay)
            //    return false;

            //if (previousColor.Equals(targetColor))
            //    ProceedSameColor(targetColor);
            previousColor = targetColor;

            //if (isWhiteTone(targetColor))
            //{
            //    ProceedDifferentWhiteColor(targetColor);
            //}
            whiteCounter = 64;
            ProceedColor(targetColor);
        }
        private void ProceedSameColor(Color targetColor)
        {
            //if (isWhiteTone(targetColor))
            //{
            //    ProceedWhiteColor(targetColor);
            //}

            //if (ShouldSendKeepAlive())
            {
                ProceedColor(targetColor);
            }
        }

        private void ProceedWhiteColor(Color targetColor)
        {
            if (whiteCounter == 0)
            {
                //if (ShouldSendKeepAlive())
                //{
                        _yeeLightDevice.SetColorTemperature(6500);
                        _yeeLightDevice.SetBrightness(targetColor.GetR() * 100 / 255);
                //}
            }
            if (whiteCounter == 1)
            {
                 _yeeLightDevice.SetColorTemperature(6500);
                 _yeeLightDevice.SetBrightness(targetColor.GetR() * 100 / 255);
            }
            whiteCounter--;
        }

        private void ProceedDifferentWhiteColor(Color targetColor)
        {
            Debug.WriteLine($"ProceedDifferentWhiteColor {targetColor}");
            if (whiteCounter > 0)
            {
                whiteCounter--;
                ProceedColor(targetColor);
            }
            _yeeLightDevice.SetColorTemperature(6500);
            _yeeLightDevice.SetBrightness(targetColor.GetR() * 100 / 255);
        }

        private void ProceedColor(Color targetColor)
        {
            //Debug.WriteLine($"Time:{DateTime.Now.ToString("mm:ss.fff")} ProceedColor {targetColor}");
            var brightness = Math.Max(targetColor.GetR(), Math.Max(targetColor.GetG(), Math.Max(targetColor.GetB(), (short)1))) * 100 / 255;
            //_yeeLightDevice.SetColorAndBrightness(targetColor.GetR(), targetColor.GetG(), targetColor.GetB(), brightness, effectType: YeeLightAPI.YeeLightConstants.Constants.EffectParamValues.SMOOTH);
            _yeeLightDevice.SetColor(targetColor.GetR(), targetColor.GetG(), targetColor.GetB());
            _yeeLightDevice.SetBrightness(brightness);
        }
        
        private bool isWhiteTone(Color color)
        {
            return color.GetR() == color.GetG() && color.GetG() == color.GetB();
        }

        public override void Dispose()
        {
            _connectTimer.Stop();
            _refreshTimer.Stop();
            base.Dispose();
        }
    }
}