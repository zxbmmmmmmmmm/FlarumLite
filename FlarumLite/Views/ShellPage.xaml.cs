using FlarentApp.Helpers;
using FlarumLite.core.Models;
using FlarumLite.Helpers;
using FlarumLite.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace FlarumLite.Views
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class ShellPage : Page
    {
        public MenuItem MainItem = MenuItem.GetMainItems()[0];
        public Notifications NotificationsData = new Notifications();
        public ShellPage()
        {
            this.NavigationCacheMode = NavigationCacheMode.Required;

            this.InitializeComponent();


            PageNameTextBlock.Text = MainItem.Name;

        }
        protected  async override void OnNavigatedTo(NavigationEventArgs e)
        {

            base.OnNavigatedTo(e);

            if (e.NavigationMode == NavigationMode.Back)//判断是不是按了返回键载入的
            {
                return;
            }
            else
            {

                hamburgerMenuControl.ItemsSource = MenuItem.GetMainItems();
                hamburgerMenuControl.OptionsItemsSource = MenuItem.GetOptionsItems();

                //var menuItem = MenuItem.GetMainItems()[0];
                hamburgerMenuControl.SelectedItem = MainItem;
                hamburgerMenuControl.SelectedIndex = 0;
                contentFrame.Navigate(MainItem.PageType, true);
                var forum = Common.Settings.Forum;
                NotificationsData = await FlarumProxy.GetNotifications($"https://{forum}/api/notifications?&page[limit]=25");

                if (NotificationsData.data == null)
                {
                    FlarumProxy.UpdateToken();
                }
                else
                {
                    var unReadedNotifications = NotificationsData.data.Where(p => p.attributes.isRead == false);
                    var unReadedNotificationsCount = unReadedNotifications.Count();
                    NotiCountTextBlock.Text = unReadedNotificationsCount.ToString();

                }
            }
        }
        private void OnMenuItemClick(object sender, ItemClickEventArgs e)
        {
            MainItem = e.ClickedItem as MenuItem;
            if(MainItem.Name == "我的关注")
            {
                var forum = Common.Settings.Forum;
                contentFrame.Navigate(MainItem.PageType, $"https://{forum}/api/discussions?&filter[subscription]=following&sort&page%5Boffset%5D=0");
                NotificationsCountButton.Visibility = Visibility.Visible;
            }
            else if(MainItem.Name == "主页")
            {
                contentFrame.Navigate(MainItem.PageType, true); 
                NotificationsCountButton.Visibility = Visibility.Visible;
            }
            else if(MainItem.Name == "通知")
            {
                contentFrame.Navigate(MainItem.PageType, NotificationsData);
                NotificationsCountButton.Visibility = Visibility.Collapsed;
            }
            else
            {
                contentFrame.Navigate(MainItem.PageType);
                NotificationsCountButton.Visibility = Visibility.Visible;
            }
            PageNameTextBlock.Text = MainItem.Name;

            if(Window.Current.Bounds.Width <= 1000)//若为宽布局则点击时不再收回菜单
            {
                hamburgerMenuControl.IsPaneOpen = false;
            }
        }

        private void NotificationsCountButton_Click(object sender, RoutedEventArgs e)
        {
            contentFrame.Navigate(typeof(NotificationsPage),NotificationsData);
            hamburgerMenuControl.SelectedIndex = -1;
            hamburgerMenuControl.SelectedOptionsIndex = 0;
            NotificationsCountButton.Visibility = Visibility.Collapsed;
            PageNameTextBlock.Text = "通知";
        }
    }

    public class MenuItem
    {
        public Symbol Icon { get; set; }
        public string Name { get; set; }
        public Type PageType { get; set; }

        public static List<MenuItem> GetMainItems()
        {
            var items = new List<MenuItem>();
            items.Add(new MenuItem() { Icon = Symbol.Home, Name = "主页", PageType = typeof(Views.MainPage) });
            items.Add(new MenuItem() { Icon = Symbol.Library, Name = "分类", PageType = typeof(Views.CatagoriesPage) });
            items.Add(new MenuItem() { Icon = (Symbol)59188, Name = "我的关注", PageType = typeof(Views.MainPage) });
            return items;
        }

        public static List<MenuItem> GetOptionsItems()
        {
            var items = new List<MenuItem>();
            items.Add(new MenuItem() { Icon = (Symbol)60047, Name = "通知", PageType = typeof(NotificationsPage)});
            items.Add(new MenuItem() { Icon = Symbol.Contact, Name = "我的", PageType = typeof(MyPages.MyPage) });
            return items;
        }
    }

}
