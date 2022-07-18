using System;
using Artemis.Core;
using Artemis.Core.Services;
using Artemis.UI.Shared;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Artemis.Plugins.Devices.YeeLightStates.ViewModels
{
    public class YeelightStatesConfigurationDialogViewModel : PluginConfigurationViewModel
    {
        private readonly IPluginManagementService _pluginManagementService;
        private readonly PluginSetting<string> _ipAddress;

        public String IpAddress
        {
            get => _ipAddress.Value;
            set => _ipAddress.Value = value;
        }

        public YeelightStatesConfigurationDialogViewModel(
            Plugin plugin, PluginSettings settings, IPluginManagementService pluginManagementService
            ) : base(plugin)
        {
            _pluginManagementService = pluginManagementService;
            _ipAddress = settings.GetSetting(YeelightDeviceProvider.DeviceIpAddress, "0.0.0.0");
        }

        protected override void OnClose()
        {
            _ipAddress.Save();

            // Fire & forget re-enabling the plugin
            Task.Run(() =>
            {
                var deviceProvider = Plugin.GetFeature<YeelightDeviceProvider>();
                if (deviceProvider is not { IsEnabled: true }) return;
                _pluginManagementService.DisablePluginFeature(deviceProvider, false);
                _pluginManagementService.EnablePluginFeature(deviceProvider, false);
            });
            
            base.OnClose();
        }
    }
}