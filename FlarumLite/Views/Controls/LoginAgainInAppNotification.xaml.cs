using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

//https://go.microsoft.com/fwlink/?LinkId=234236 上介绍了“用户控件”项模板

namespace FlarumLite.Views.Controls
{
    public sealed partial class LoginAgainInAppNotification : UserControl
    {
        public LoginAgainInAppNotification()
        {
            this.InitializeComponent();
        }
        public void ShowLoginNotification()
        {
            int duration = 2000;
            InAppNotification.Show("Some text.", duration);

            // Show notification using a DataTemplate
            object inAppNotificationWithButtonsTemplate;
            bool isTemplatePresent = Resources.TryGetValue("InAppNotificationWithButtonsTemplate", out inAppNotificationWithButtonsTemplate);

            if (isTemplatePresent && inAppNotificationWithButtonsTemplate is DataTemplate)
            {
                InAppNotification.Show(inAppNotificationWithButtonsTemplate as DataTemplate);
            }

            // Dismiss notification
            InAppNotification.Dismiss();

        }

        private void YesButton_Click(object sender, RoutedEventArgs e)
        {
            InAppNotification.Dismiss();
        }

        private void NoButton_Click(object sender, RoutedEventArgs e)
        {
            InAppNotification.Dismiss();
        }
    }
}
