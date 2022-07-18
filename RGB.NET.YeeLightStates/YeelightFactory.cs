using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using RGB.NET.Core;

namespace RGB.NET.YeeLightStates
{
    public class YeelightProvider : AbstractRGBDeviceProvider
    {
        private static YeelightProvider _instance;
        public static YeelightProvider Instance => _instance ??= new YeelightProvider();
        public static IPAddress IpAddress = IPAddress.Broadcast;

        private readonly List<YeelightDevice> _lights = new();
        private const int LightListenPort = 55443;

        protected override void InitializeSDK()
        {
            // nothing
        }

        protected override IEnumerable<IRGBDevice> LoadDevices()
        {
            //var lightIp = IPAddress.Parse("192.168.1.248");
            
            var light = new YeelightAPI.Device(IpAddress.ToString(), 1982);
            LightConnectAndEnableMusicMode(light);

            _lights.Add(
                new YeelightDevice(
                    light, 
                    new YeelightDeviceInfo(),
                    new YeelightUpdateQueue(
                        GetUpdateTrigger(),
                        light
                        )
                    )
                );
            return _lights;
        }

        private void LightConnectAndEnableMusicMode(YeelightAPI.Device light)
        {
            var localMusicModeListenPort = GetFreeTCPPort(); // This can be any free port

            //IPAddress localIP;
            using (var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0))
            {
                light.Connect();
                //var lightIP = light .GetLightIPAddressAndPort().ipAddress;
                //socket.Connect(lightIP, LightListenPort);
                //localIP = ((IPEndPoint)socket.LocalEndPoint).Address;
            }

            light.Connect();
            var connectionTries = 100;
            do
            {
                Thread.Sleep(500);
            } while (!light.IsConnected && --connectionTries > 0);
            try
            {
                //light.SetMusSicMode(localIP, (ushort)localMusicModeListenPort, true);
            }
            catch
            {
                // ignored
            }
        }

        private int GetFreeTCPPort()
        {
            int freePort;

            // When a TCPListener is created with 0 as port, the TCP/IP stack will asign it a free port
            TcpListener listener = new TcpListener(IPAddress.Loopback, 0); // Create a TcpListener on loopback with 0 as the port
            listener.Start();
            freePort = ((IPEndPoint)listener.LocalEndpoint).Port; // Gets the local port the TcpListener is listening on
            listener.Stop();
            return freePort;
        }
    }
}