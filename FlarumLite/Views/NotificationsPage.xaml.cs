using FlarentApp.Helpers;
using FlarumLite.core.Models;
using FlarumLite.Helpers;
using FlarumLite.Services;
using FlarumLite.Views.Controls;
using FlarumLite.Views.DetailPages;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Runtime.Serialization.Json;
using System.Text;
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
    public sealed partial class NotificationsPage : Page
    {
        public ObservableCollection<Notification> Notifications = new ObservableCollection<Notification>();
        public NotificationsPage()
        {
            NavigationCacheMode = NavigationCacheMode.Required;
            this.InitializeComponent();           
        }
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.NavigationMode != NavigationMode.Back)//判断是不是按了返回键载入的
            {
                var notifications = e.Parameter as Notifications;
                if(notifications != null && notifications.data != null)
                {                   
                    GetNotifications(e.Parameter as Notifications);
                }
                else
                {
                    var forum = Common.Settings.Forum;
                    var addingNotificationsData = await FlarumProxy.GetNotifications($"https://{forum}/api/notifications?&page[limit]=25");
                    if (addingNotificationsData.data == null)
                    {
                        new Toast("未登录").show();
                        return;
                    }
                    GetNotifications(addingNotificationsData);
                }
            }
            else
            {
                return;
            }
        }
        private void GetNotifications(Notifications addingNotificationsData)
        {
            Notifications.Clear();

            var addingNotifications = addingNotificationsData.data;
            var addingUsers = addingNotificationsData.included.Where(p => p.type == "users");
            var addingPosts = addingNotificationsData.included.Where(p => p.type == "posts");

            foreach (var notification in addingNotifications)
            {
                notification.attributes.user = addingUsers.FirstOrDefault(p => p.id == notification.relationships.fromUser.data.id);
                notification.attributes.post = addingPosts.FirstOrDefault(p => p.id == notification.relationships.subject.data.id);

                if(notification.attributes.content != null)
                {
                    if (notification.attributes.content.ToString().Contains("{"))
                    {
                        var serializer = new DataContractJsonSerializer(typeof(NotificationContent));
                        var ms = new MemoryStream(Encoding.UTF8.GetBytes(notification.attributes.content.ToString()));
                        notification.attributes.notificationContent = (NotificationContent)serializer.ReadObject(ms);
                    }
                }




                if(notification.attributes.user.attributes.avatarUrl == null)
                {
                    notification.attributes.user.attributes.avatarUrl = "https://flarum.csur.fun/2022-02-08/1644323380-214777-guest.png";
                }
                Notifications.Add(notification);
            }
            NotificationsListView.ItemsSource = Notifications;
        }

        private void UserHyperButton_Click(object sender, RoutedEventArgs e)
        {
            Included user;
            if(sender is HyperlinkButton)
            {
                var hypbtn = sender as HyperlinkButton;
                user = (hypbtn.DataContext as Notification).attributes.user;
            }
            else
            {
                var btn = sender as Button;
                user = (btn.DataContext as Notification).attributes.user;
            }
            NavigationService.Navigate<UserDetailPage>(user.id);
        }

        private async void NotificationsListView_RefreshRequested(object sender, EventArgs e)
        {
            Notifications.Clear();
            var forum = Common.Settings.Forum;
            var addingNotificationsData = await FlarumProxy.GetNotifications($"https://{forum}/api/notifications?&page[limit]=25");
            if (addingNotificationsData.data == null)
            {
                new Toast("未登录").show();
                return;
            }
            GetNotifications(addingNotificationsData);
        }

        private void NotificationsListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var clicked = e.ClickedItem as Notification;
            var navigate = new DiscussionNavigationInfo();
            if (clicked.attributes.post == null)
            {
                navigate = new DiscussionNavigationInfo { targetDiscussion = int.Parse(clicked.relationships.subject.data.id), targetPost = -1};
            }
            else
            {
                navigate = new DiscussionNavigationInfo { targetDiscussion = int.Parse(clicked.attributes.post.relationships.discussion.data.id), targetPost = (int)clicked.attributes.post.attributes.number };
            }

            NavigationService.Navigate<DiscussionDetailPage>(navigate);
        }
    }
}
