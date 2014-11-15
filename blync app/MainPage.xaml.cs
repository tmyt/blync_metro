using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.HumanInterfaceDevice;
using Windows.Devices.Usb;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace blync_app
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private HidDevice device;

        public MainPage()
        {
            this.InitializeComponent(); ;
        }

        private async Task<HidDevice> Initialize()
        {
            ushort vendorId = 0x1130;
            ushort productId = 0x0001;
            ushort usagePage = 0x0001;
            ushort usageId = 0x0003;
            var selector = HidDevice.GetDeviceSelector(usagePage, usageId, vendorId, productId);
            var devices = await DeviceInformation.FindAllAsync(selector);
            return await HidDevice.FromIdAsync(devices.First().Id, FileAccessMode.ReadWrite);
        }

        async Task SendReport(byte code)
        {
            var report = device.CreateOutputReport();
            var data = new byte[] { 0, 85, 83, 66, 67, 0, 64, 2, code };
            report.Data = data.AsBuffer();
            await device.SendOutputReportAsync(report);
        }

        async void MainPage_OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            device = await Initialize();
        }

        async void Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs selectionChangedEventArgs)
        {
            var combobox = (ComboBox)sender;
            var item = combobox.SelectedItem;
            if (item == null) return;
            await SendReport(byte.Parse((string)(item as ComboBoxItem).Tag));
        }
    }
}
