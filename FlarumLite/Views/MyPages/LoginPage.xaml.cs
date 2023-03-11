using FlarentApp.Helpers;
using FlarumLite.Helpers;
using FlarumLite.Services;
using FlarumLite.Views.Controls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.System;
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
    public sealed partial class LoginPage : Page
    {
        public string UserNameEntered;
        public string PasswordEntered;
        public bool Succeed;

        public LoginPage()
        {
            this.InitializeComponent();
        }

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            var loginData = await FlarumProxy.GetToken(UserNameEntered, PasswordEntered);
            ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
            switch (loginData.statusCode)
            {
                case "Unauthorized"://用户名或密码错误
                    StatusTextBlock.Visibility = Visibility.Visible;
                    StatusTextBlock.Text = "用户名或密码错误,请重试";
                    break;
                case "OK"://登录成功
                    new Toast("登录成功!请刷新当前页面").show();
                    Common.Settings.LoginName = UserNameEntered;
                    Common.Settings.LoginPassword = PasswordEntered;
                    Common.Settings.UserId = loginData.userId;
                    Common.Settings.Token = loginData.token;

                    Succeed = true;
                    NavigationService.GoBack();
                    break;
            }
        }

        private void LoginPassWordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            StatusTextBlock.Visibility = Visibility.Collapsed;
            var passwordBox = sender as PasswordBox;
            PasswordEntered = passwordBox.Password;
        }

        private void UserNameAutoSuggestBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            StatusTextBlock.Visibility = Visibility.Collapsed;
            UserNameEntered = sender.Text;
        }

        private async void SignUpButton_Click(object sender, RoutedEventArgs e)
        {
            await Launcher.LaunchUriAsync(new Uri(""));
        }
    }
}
