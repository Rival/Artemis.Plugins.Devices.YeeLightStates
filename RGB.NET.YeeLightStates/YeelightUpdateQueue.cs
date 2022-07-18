using System;
using RGB.NET.Core;

namespace RGB.NET.YeeLightStates
{
    public class YeelightUpdateQueue : UpdateQueue
    {
        private readonly YeelightAPI.Device _yeeLightDevice;

        public YeelightUpdateQueue(IDeviceUpdateTrigger updateTrigger, YeelightAPI.Device yeeLightDevice) : base(updateTrigger)
        {
            _yeeLightDevice = yeeLightDevice;
        }

        protected override void Update(in ReadOnlySpan<(object key, Color color)> dataSet)
        {
            foreach ((object key, Color color) item in dataSet)
            {
                UpdateColor(item.color);
                _yeeLightDevice.SetRGBColor(item.color.GetR(), item.color.GetG(), item.color.GetB()); //TODO move
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

            if (previousColor.Equals(targetColor))
                ProceedSameColor(targetColor);
            previousColor = targetColor;

            if (isWhiteTone(targetColor))
            {
                ProceedDifferentWhiteColor(targetColor);
            }
            whiteCounter = 64;
            ProceedColor(targetColor);
        }
        private void ProceedSameColor(Color targetColor)
        {
            if (isWhiteTone(targetColor))
            {
                ProceedWhiteColor(targetColor);
            }

            //if (ShouldSendKeepAlive())
            //{
            //    ProceedColor(targetColor);
            //}
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
            _yeeLightDevice.SetRGBColor(targetColor.GetR(), targetColor.GetG(), targetColor.GetB());
            _yeeLightDevice.SetBrightness(Math.Max(targetColor.GetR(), Math.Max(targetColor.GetG(), Math.Max(targetColor.GetB(), (short)1))) * 100 / 255);
        }
        
        private bool isWhiteTone(Color color)
        {
            return color.GetR() == color.GetG() && color.GetG() == color.GetB();
        }
    }
}