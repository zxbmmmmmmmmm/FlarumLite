using FlarentApp.Helpers;
using FlarumApi;
using FlarumLite.core.Models;
using FlarumLite.Helpers;
using FlarumLite.Helpers.ValueConverters;
using FlarumLite.Services;
using FlarumLite.Views.DetailPages;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace FlarumLite.Views
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class UserDetailPage : Page
    {
        public DiscussionDetails UserDetails = new DiscussionDetails();

        public ObservableCollection<Datum> UserReplies = new ObservableCollection<Datum>();
        public Links Links = new Links();


        public string NavigatingItem;

        public UserDetailPage()
        {
            this.NavigationCacheMode = NavigationCacheMode.Required;
            this.InitializeComponent();
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            NavigatingItem = e.Parameter.ToString();
            if (e.NavigationMode == NavigationMode.Back)//判断是不是按了返回键载入的
            {
                return;
            }
            else
            {
                Links = new Links();
                UserReplies.Clear();
                LoadMoreButton.Visibility = Visibility.Visible;
                GetUserDetails(NavigatingItem);
            }

        }

        private async void GetUserDetails(string navigatingItem)
        {
            LoadingControl.IsLoading = true;
            string uid;
            var forum = Common.Settings.Forum;
            if (NavigatingItem.Contains("[username]"))//通过UserName传输(@)
            {
                var target = navigatingItem.Replace("[username]", "");
                var user = await FlarumApiProviders.GetUser($"https://{forum}/api/users/{target}?bySlug=true",Common.Settings.Token);//获取UID
                uid = user.Id.ToString();
            }
            else
            {
                uid = navigatingItem;
            }
            UserDetails = await FlarumProxy.GetDiscussionDetails($"https://{forum}/api/users/{uid}");
            LoadingControl.IsLoading = false;
            BindUserData();
            GetUserReplies();

        }

        private async void GetUserReplies()
        {
            var addingRepliesData = new MainPagePosts();
            var addingReplies = new ObservableCollection<Datum>();
            var addingDiscussions = new ObservableCollection<Included>();

            UserDetailsListView.IsEnabled = false;
            if(Links.next == null)
            {
                var forum = Common.Settings.Forum;
                addingRepliesData = await FlarumProxy.GetInfo($"https://{forum}/api/posts?filter[author]={UserDetails.data.attributes.username}&sort=-createdAt");
            }
            else
            {
                addingRepliesData = await FlarumProxy.GetInfo(Links.next);
            }
            UserDetailsListView.IsEnabled = true;

            addingReplies = addingRepliesData.data;
            foreach (var included in addingRepliesData.included)
            {
                if (included.type == "discussions")
                {
                    addingDiscussions.Add(included);
                }
            }

            foreach (Datum reply in addingReplies)
            {
                reply.attributes.user = UserDetails.data.attributes;
                reply.attributes.contentMD = CSStoMarkdown.HTMLtoMarkdown(reply.attributes.contentHtml);
                reply.attributes.discussion = addingDiscussions.FirstOrDefault(p => p.id == reply.relationships.discussion.data.id);
                UserReplies.Add(reply);
            }
            Links = addingRepliesData.links;
            if(Links.next == null)
            {
                LoadMoreButton.Visibility = Visibility.Collapsed;
            }
            UserDetailsListView.ItemsSource = UserReplies;
        }

        private void BindUserData()
        {
            if (UserDetails.data.attributes.displayName == null)
            {
                UserDetails.data.attributes.displayName = UserDetails.data.attributes.username;
            }
            DisplayNameTextBlock.Text = UserDetails.data.attributes.displayName;
            UserNameTextBlock.Text = UserDetails.data.attributes.username;
            if (UserDetails.data.attributes.bio != null)
            {
                BioTextBlock.Text = UserDetails.data.attributes.bio;
            }
            DiscussionCountTextBlock.Text = UserDetails.data.attributes.discussionCount.ToString();
            CommentCountTextBlock.Text = UserDetails.data.attributes.commentCount.ToString();

            if (UserDetails.data.attributes.avatarUrl == null)
            {

                UserDetails.data.attributes.avatarUrl = "https://flarum.csur.fun/2022-02-08/1644323380-214777-guest.png";
            }
            AvatarImageBrush.ImageSource = new BitmapImage(new Uri(UserDetails.data.attributes.avatarUrl));

            JoinTimeTextBlock.Text = DateConverter.StringDateTimeFriendFormat(UserDetails.data.attributes.joinTime);
            if (UserDetails.data.attributes.lastSeenAt != null)
            {
                LastSeenAtTextBlock.Text = DateConverter.StringDateTimeFriendFormat(UserDetails.data.attributes.lastSeenAt);
            }
        }

        private void MarkDownTextBlock_ImageClicked(object sender, Microsoft.Toolkit.Uwp.UI.Controls.LinkClickedEventArgs e)
        {

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
                    var navigate = new DiscussionNavigationInfo { targetDiscussion = discussionNumber, targetPost = postNumber };
                    NavigationService.Navigate<DiscussionDetailPage>(navigate);

                }
            }
            else
            {
                await Launcher.LaunchUriAsync(new Uri(e.Link));
            }
        }
        private void ViewSourceButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void DiscussionHyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as HyperlinkButton;
            var clicked = btn.DataContext as Datum;
            var navigate = new DiscussionNavigationInfo { targetDiscussion = int.Parse(clicked.attributes.discussion.id), targetPost = (int)clicked.attributes.number };
            NavigationService.Navigate<DiscussionDetailPage>(navigate);
        }

        private void LoadMoreButton_Click(object sender, RoutedEventArgs e)
        {
            GetUserReplies();
        }

        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            width.Width = ActualWidth - 110;
        }
    }
}
