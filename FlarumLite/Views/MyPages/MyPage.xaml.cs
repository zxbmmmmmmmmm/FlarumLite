using FlarumLite.core.Models;
using FlarumLite.Helpers;
using FlarumLite.Services;
using FlarumLite.Views.DetailPages;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace FlarumLite.Views.MyPages
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MyPage : Page
    {
        private string _versionDescription;

        public string VersionDescription
        {
            get { return _versionDescription; }

            set { Set(ref _versionDescription, value); }
        }
        private void Set<T>(ref T storage, T value, [CallerMemberName]string propertyName = null)
        {
            if (Equals(storage, value))
            {
                return;
            }

            storage = value;
            OnPropertyChanged(propertyName);
        }

        private string GetVersionDescription()
        {
            //var appName = "AppDisplayName".GetLocalized();
            var appName = Package.Current.DisplayName;
            var package = Package.Current;
            var packageId = package.Id;
            var version = packageId.Version;

            return $"{appName} - Beta {version.Major}.{version.Minor}.{version.Build}.{version.Revision}";
        }
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public DiscussionDetails MyInfo = new DiscussionDetails();

        public MyPage()
        {
            this.InitializeComponent();
            VersionDescription = GetVersionDescription();

            GetMyData();
        }

        private async void GetMyData()
        {
            ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
            if (localSettings.Values["userId"] != null)
            {
                var userId = localSettings.Values["userId"].ToString();
                var forum = ApplicationData.Current.LocalSettings.Values["forum"].ToString();

                MyInfo = await FlarumProxy.GetDiscussionDetails($"https://{forum}/api/users/{userId}");
                SetPageBinding();
            }
            else
            {
                DisplayNameTextBlock.Text = "未登录";
                UserNameTextBlock.Text = "登录以使用更多功能";
                var avatar = new BitmapImage(new Uri("https://bkimg.cdn.bcebos.com/pic/10dfa9ec8a13632762d0979f23c7b7ec08fa513d694c?x-bce-process=image/resize,m_lfit,w_440,limit_1/format,f_auto"));
                AvatarImageBrush.ImageSource = avatar;
                LoginButton.Content = "登录";
                LogoutButton.Visibility = Visibility.Collapsed;
            }
        }

        private void SetPageBinding()
        {
            UserNameTextBlock.Text = MyInfo.data.attributes.username;
            DisplayNameTextBlock.Text = MyInfo.data.attributes.displayName;
            if(MyInfo.data.attributes.avatarUrl == null )
            {
                MyInfo.data.attributes.avatarUrl = "ms-appx:///Assets/guest.png";
            }
            var avatar = new BitmapImage(new Uri(MyInfo.data.attributes.avatarUrl));

            AvatarImageBrush.ImageSource = avatar;
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate<LoginPage>();
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
            localSettings.Values["userId"] = null;
            localSettings.Values["token"] = "0";
            localSettings.Values["loginName"] = null;
            localSettings.Values["loginPassword"] = null;

            GetMyData();
        }

        private void AboutHyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            var navigate = new DiscussionNavigationInfo { targetDiscussion = 2720, targetPost = -1 };
            NavigationService.Navigate<DiscussionDetailPage>(navigate);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate<SettingsPage>();
        }
    }
}
