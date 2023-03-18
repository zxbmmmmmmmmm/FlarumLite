using FlarumLite.Views.Dialogs;
using FlarumLite.Views.MyPages;
using Microsoft.Toolkit.Uwp.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Storage;
using Windows.UI.Core;

namespace FlarumLite.Services
{
    public static class FirstRunDisplayService
    {
        private static bool shown = false;

        internal static async Task ShowIfAppropriateAsync()
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(
                CoreDispatcherPriority.Normal, async () =>
                {
                    if (SystemInformation.IsFirstRun && !shown)
                    {
                        ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
                        localSettings.Values["CloudflareUnderAttackMode"] = false;
                        localSettings.Values["tails"] = "***\n 本回复来自 *__Flarent Lite__* *{AppVersion}*";

                        localSettings.Values["forum"] = "community.wvbtech.com";
                        localSettings.Values["token"] = "";
                        shown = true;
                        var dialog = new FirstRunDialog();
                        await dialog.ShowAsync();
                        NavigationService.Navigate<EditForumPage>();
                    }
                });
        }
    }
}
