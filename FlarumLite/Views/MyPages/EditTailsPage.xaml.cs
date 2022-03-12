using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Security.ExchangeActiveSyncProvisioning;
using Windows.Storage;
using Windows.System;
using Windows.System.Profile;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace FlarumLite.Views.MyPages
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class EditTailsPage : Page
    {
        public EditTailsPage()
        {
            this.InitializeComponent();
            ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;

            if (localSettings.Values["tails"] != null)
            {
                EditZone.TextDocument.SetText(Windows.UI.Text.TextSetOptions.None, localSettings.Values["tails"].ToString());
            }
            else
            {
                localSettings.Values["tails"] = "***\n 本回复来自 *__Flarum Lite__* *{AppVersion}*";
                EditZone.TextDocument.SetText(Windows.UI.Text.TextSetOptions.None, localSettings.Values["tails"].ToString());
            }
        }

        private void EditZone_TextChanged(object sender, RoutedEventArgs e)
        {
            ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;

            string text;
            EditZone.Document.GetText(Windows.UI.Text.TextGetOptions.UseCrlf, out text);
            var preview = TailsHelper.CustonTails(text);
            preview = preview.Replace("\r\n\r\n", "\n\n \n\n");
            preview = preview.Replace("\r\n", "\n\n");

            Previewer.Text = string.IsNullOrWhiteSpace(preview) ? "*部分内容可能和发布后不同*" : preview;
            localSettings.Values["tails"] = text;
        }

       
        private void Previewer_LinkClicked(object sender, Microsoft.Toolkit.Uwp.UI.Controls.LinkClickedEventArgs e)
        {
            try
            {
                var link = new Uri(e.Link);
                var linkOpen = Task.Run(() => Launcher.LaunchUriAsync(link));
            }
            catch
            {
            }
        }
    }
    public class TailsHelper 
    {
        public static string CustonTails(string text)
        {
            var eas = new EasClientDeviceInformation();
            var package = Package.Current;
            var version = package.Id.Version;
            AnalyticsVersionInfo analyticsVersion = AnalyticsInfo.VersionInfo;
            ulong v = ulong.Parse(AnalyticsInfo.VersionInfo.DeviceFamilyVersion);
            ulong v1 = (v & 0xFFFF000000000000L) >> 48;
            ulong v2 = (v & 0x0000FFFF00000000L) >> 32;
            ulong v3 = (v & 0x00000000FFFF0000L) >> 16;
            ulong v4 = (v & 0x000000000000FFFFL);
            var systemVersion = $"{v1}.{v2}.{v3}.{v4}";

            text = text.Replace("{AppVersion}", $"{version.Major}.{version.Minor}.{version.Build}.{version.Revision}");
            text = text.Replace("{OperatingSystem}", $"{eas.OperatingSystem}");
            text = text.Replace("{SystemFirmwareVersion}", $"{eas.SystemFirmwareVersion}");
            text = text.Replace("{SystemManufacturer}", $"{eas.SystemManufacturer}");
            text = text.Replace("{SystemVersion}", $"{systemVersion}");
            text = text.Replace("{SystemBuild}", $"{v3}.{v4}");
            var sku = eas.SystemSku.Replace("_", "\\_");
            text = text.Replace("{SystemSku}", $"{sku}");
            text = text.Replace("{DeviceName}", $"{eas.FriendlyName}");
            return text;
        }

    }

}
