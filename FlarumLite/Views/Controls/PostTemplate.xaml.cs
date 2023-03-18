using FlarentApp.Helpers;
using FlarumLite.core.Models;
using FlarumLite.Services;
using FlarumLite.Views.DetailPages;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;
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
    public sealed partial class PostTemplate : UserControl,INotifyPropertyChanged
    {
        public PostTemplate()
        {
            this.InitializeComponent();
            this.DataContextChanged += (s, e) => Bindings.Update();
        }

        public Included Post
        {
            get { return (Included)GetValue(PostProperty); }
            set { SetValue(PostProperty, value); }
        }
        public static readonly DependencyProperty PostProperty =
           DependencyProperty.Register("Post", typeof(Included), typeof(PostTemplate), new PropertyMetadata(new Included()));

        public DiscussionDetailPage DetailPage
        {
            get => NavigationService.Frame.Content as DiscussionDetailPage;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private async void ViewSourceButton_Click(object sender, RoutedEventArgs e)
        {

            var item = sender as MenuFlyoutItem;
            var html = (item.DataContext as Included).attributes.contentHtml;
            var md = (item.DataContext as Included).attributes.contentMD;

            var dialog = new PostSourceDialog(md, html);
            await dialog.ShowAsync();
        }

        private void UserButton_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as HyperlinkButton;
            var clicked = btn.DataContext as Included;
            if (clicked.relationships.user.data.id != "0")
            {
                NavigationService.Navigate<UserDetailPage>(clicked.relationships.user.data.id);

            }
        }

        private async void MarkDownTextBlock_LinkClicked(object sender, Microsoft.Toolkit.Uwp.UI.Controls.LinkClickedEventArgs e)
        {
            var link = e.Link;
            var forum = Common.Settings.Forum;
            if (link.Contains(forum))//如果是特殊链接（如回复，@等）
            {
                if (link.Contains($"{forum}/u".ToLower()))
                {
                    var split = link.Split(new char[] { '/' }, 10);//拆分
                    int place = 0;
                    for (int i = 0; i < split.Length; i++)
                    {
                        var str = split[i];
                        if (str == "u")
                        {
                            place = i;//"d"出现的位置，那么下一个就是帖子
                            break;
                        }
                    }
                    string userName = split[place + 1];
                    NavigationService.Navigate<UserDetailPage>($"[username]{userName}");

                }
                if (link.Contains($"{forum}/d".ToLower()))
                {
                    var split = link.Split(new char[] { '/' }, 10);//拆分
                    int place = 0;
                    for (int i = 0; i < split.Length; i++)
                    {
                        var str = split[i];
                        if (str == "d")
                        {
                            place = i;//"d"出现的位置，那么下一个就是帖子
                            break;
                        }
                    }
                    int discussionNumber = int.Parse(split[place + 1]);
                    int postNumber = 0;
                    if (place + 2 < split.Length)
                    {
                        postNumber = int.Parse(split[place + 2]);
                    }
                    if (discussionNumber.ToString() == DetailPage.NavigatingDiscussion)
                    {
                        var items = DetailPage.DiscussionDetailsListView.Items;
                        var selected = items.First(p => (p as Included).attributes.number == postNumber);
                        DetailPage.DiscussionDetailsListView.ScrollIntoView(selected);
                    }
                    else
                    {
                        var navigate = new DiscussionNavigationInfo { targetDiscussion = discussionNumber, targetPost = postNumber };
                        NavigationService.Navigate<DiscussionDetailPage>(navigate);
                    }
                }
            }
            else
            {
                await Launcher.LaunchUriAsync(new Uri(e.Link));
            }
        }

        private void MarkDownTextBlock_ImageClicked(object sender, Microsoft.Toolkit.Uwp.UI.Controls.LinkClickedEventArgs e)
        {
            new ImageView().Show(e.Link);
        }
        private void ReplyButton_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            var data = btn.DataContext as Included;
            var id = data.id;
            var user = data.attributes.user.displayName;
            string text = $"@\"{user}\"#p{id} ";
            string discussionName = DetailPage.DiscussionInfo.attributes.title;
            string[] navigate = { DetailPage.NavigatingDiscussion, discussionName, text };

            NavigationService.Navigate<ReplyPage>(navigate);
        }
    }
}
