using ColorCode.Compilation.Languages;
using FlarentApp.Helpers;
using FlarumLite.core.Models;
using FlarumLite.Helpers;
using FlarumLite.Helpers.ValueConverters;
using FlarumLite.Services;
using FlarumLite.Views.Controls;
using Html2Markdown;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
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

namespace FlarumLite.Views.DetailPages
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class DiscussionDetailPage : Page
    {
        public DiscussionDetails DiscussionDetails = new DiscussionDetails();
        public Datum DiscussionInfo = new Datum();

        public ObservableCollection<Included> Includeds = new ObservableCollection<Included>();
        public ObservableCollection<Included> Posts = new ObservableCollection<Included>();
        public ObservableCollection<Included> PosterPosts = new ObservableCollection<Included>();
        public ObservableCollection<Included> Users = new ObservableCollection<Included>();
        public string NavigatingDiscussion;
        public string NavigatingPost;
        public int PostNumberToLoad = 15;
        public double Scroll = 0;

        public DiscussionDetailPage()
        {
            this.NavigationCacheMode = NavigationCacheMode.Required;
            this.InitializeComponent(); 
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {

            base.OnNavigatedTo(e);

           

            var navigate = e.Parameter as DiscussionNavigationInfo;

            NavigatingDiscussion = navigate.targetDiscussion.ToString();
            NavigatingPost = navigate.targetPost.ToString();
            
            if (e.NavigationMode == NavigationMode.Back)//判断是不是按了返回键载入的
            {
                if(Scroll != 0)
                {
                    ScrollViewer scrviewer = FindVisualChildByName<ScrollViewer>(DiscussionDetailsListView, "ScrollViewer");
                    scrviewer.ScrollToVerticalOffset(Scroll);
                }
                return;
            }
            else
            {
                Includeds.Clear();
                Posts.Clear();
                PosterPosts.Clear();
                Users.Clear();
                PostNumberToLoad = 15;
                DiscussionDetails = new DiscussionDetails();
                DiscussionInfo = new Datum();
                GetDiscussionDetails(NavigatingDiscussion);
            }
        }

        private void ScrollIntoPost(string target)
        {
            if(target != "-1")
            {
                var items = DiscussionDetailsListView.Items;
                DiscussionDetailsListView.ScrollIntoView(items[int.Parse(target) - 1]);
            }

        }

        private async Task GetDiscussionDetails(string DiscussionId)  
        {
            LoadingControl.IsLoading = true;
            var addinPosts = new ObservableCollection<Included>();
            var addingIncludeds = new ObservableCollection<Included>();
            var addingUsers = new ObservableCollection<Included>();
            var addingDiscussionDetails = new DiscussionDetails();
            var forum = Common.Settings.Forum;
            int num = 15;
            if(Posts != null&&Posts.Count!=0)
            {
                num = (int)Posts[Posts.Count - 1].attributes.number +15;
            }


            addingDiscussionDetails = await FlarumProxy.GetDiscussionDetails($"https://{forum}/api/discussions/{DiscussionId}?bySlug=true&page[near]={num}&page[limit]=30");

            LoadingControl.IsLoading = false;
            

            addingIncludeds = addingDiscussionDetails.included;

            LoadingControl.IsLoading = false;
            //TitleTextBlcok.Text = DiscussionDetails.data[0].attributes.title;
            addingDiscussionDetails.data.tags = new ObservableCollection<Included>();

            foreach (var included in addingIncludeds)
            {
                switch (included.type)
                {
                    case "posts":
                        if (included.relationships == null)
                        {
                            included.relationships =  new Relationships { user = new core.Models.User { data = new Data { id = "0" } } };
                        }
                        else if(included.relationships.user == null)
                        {
                            included.relationships.user = new core.Models.User { data = new Data { id = "0" } };
                        }
                        if (included.attributes.number < num + 15 && included.attributes.number > num - 15)
                        {
                            if(included.relationships.discussion!= null)
                            {
                                addinPosts.Add(included);

                            }
                        }
                        break;
                    case "tags":
                        if(Posts == null || Posts.Count == 0)
                        {
                            addingDiscussionDetails.data.tags.Add(included);
                        }
                        break;
                    case "users":
                        addingUsers.Add(included);
                        break;
                }
            }
            if (Posts == null ||Posts.Count == 0)//第一次加载
            {
                if (addingDiscussionDetails.data.relationships.user == null)
                {
                    addingDiscussionDetails.data.relationships.user =  new core.Models.User { data = new Data { id = "0" } };
                }
                DiscussionInfo = addingDiscussionDetails.data;
                TitleTextBlock.Text = DiscussionInfo.attributes.title;
                WrapPanelContainer.ItemsSource = DiscussionInfo.tags;
            }

            addingUsers.Add(new Included { id = "0", attributes = new Attributes { displayName = "【已注销】", name = "已注销"  } });
            foreach (var post in addinPosts)
            {
                
                if (post.attributes.contentHtml == null)
                {
                    switch (post.attributes.contentType)
                    {
                        case "discussionStickied":
                            post.attributes.contentHtml = "*置顶了此贴*";
                            break;
                        case "discussionTagged":
                            post.attributes.contentHtml = "*更改了标签*";
                            break;
                        case "discussionLocked":
                            post.attributes.contentHtml = "*锁定了此贴*";
                            break;
                        case "discussionRenamed":
                            post.attributes.contentHtml = "*更改了标题*";
                            break;
                        case "discussionSplit":
                            post.attributes.contentHtml = "*拆分了回复*";
                            break;
                        default:
                            post.attributes.contentHtml = "";
                            break;
                    }


                }
                
                post.attributes.user = addingUsers.FirstOrDefault(p => p.id == post.relationships.user.data.id).attributes;
                if (post.attributes.user.avatarUrl == null)
                {
                    post.attributes.user.avatarUrl = "https://flarum.csur.fun/2022-02-08/1644323380-214777-guest.png";
                }
                var converter = new Html2Markdown.Converter();

                post.attributes.contentMD = CSStoMarkdown.HTMLtoMarkdown(post.attributes.contentHtml);

            }
            var ordered = addinPosts.OrderBy(p => p.attributes.number);
            foreach (var post in ordered)
            {
                Posts.Add(post);

                if (post.relationships.user.data.id == DiscussionInfo.relationships.user.data.id)
                {
                    PosterPosts.Add(post);
                }


            }
            if(ViewPosterAppBarToggleButton.IsChecked == true)
            {
                DiscussionDetailsListView.ItemsSource = PosterPosts;
            }
            else
            {
                DiscussionDetailsListView.ItemsSource = Posts;
            }
            if(Posts[Posts.Count - 1].attributes.number >= DiscussionInfo.attributes.commentCount)
            {
                LoadMoreButton.Visibility = Visibility.Collapsed;
            }
            else
            {
                LoadMoreButton.Visibility = Visibility.Visible;
            }
            if (PostNumberToLoad == 15)
            {
                ScrollIntoPost(NavigatingPost);
            }
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            ScrollViewer scrviewer = FindVisualChildByName<ScrollViewer>(DiscussionDetailsListView,"ScrollViewer");
            Scroll = scrviewer.VerticalOffset;
            base.OnNavigatingFrom(e);
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
        }

        private async void ViewSourceButton_Click(object sender, RoutedEventArgs e)
        {

            var item = sender as MenuFlyoutItem;
            var html = (item.DataContext as Included).attributes.contentHtml;
            var md = (item.DataContext as Included).attributes.contentMD;

            var dialog = new PostSourceDialog(md,html);
            await dialog.ShowAsync();
        }

        private void UserButton_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as HyperlinkButton;
            var clicked = btn.DataContext as Included;
            if(clicked.relationships.user.data.id != "0")
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
                        if(str == "d")
                        {
                            place = i;//"d"出现的位置，那么下一个就是帖子
                            break;
                        }
                    }
                    int discussionNumber = int.Parse(split[place + 1]);
                    int postNumber = 0;
                    if(place + 2 < split.Length)
                    {
                        postNumber = int.Parse(split[place + 2]);
                    }
                    if (discussionNumber.ToString() == NavigatingDiscussion)
                    {
                        var items = DiscussionDetailsListView.Items;
                        var selected = items.First(p => (p as Included).attributes.number == postNumber);
                        DiscussionDetailsListView.ScrollIntoView(selected);
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
            var images = new ObservableCollection<Uri>();
            images.Add(new Uri(e.Link));
            NavigationService.Navigate<ImageDetailPage>();
        }

        private void ViewPosterAppBarToggleButton_Click(object sender, RoutedEventArgs e)
        {
            var appBar = sender as AppBarToggleButton;
            if ((bool)appBar.IsChecked)
            {
                DiscussionDetailsListView.ItemsSource = PosterPosts;
            }
            else
            {
                DiscussionDetailsListView.ItemsSource = Posts;
            }
        }

        private async void RefreshAppBarButton_Click(object sender, RoutedEventArgs e)
        {
            NavigatingPost = "-1";
            DiscussionDetails = new DiscussionDetails();
            Includeds.Clear();
            Posts.Clear();
            Users.Clear();
            PostNumberToLoad = 15;
            PosterPosts.Clear();
            LoadMoreButton.Visibility = Visibility.Visible;
            await GetDiscussionDetails(NavigatingDiscussion);

        }

        private void ToTopButton_Click(object sender, RoutedEventArgs e)
        {
            DiscussionDetailsListView.ScrollIntoView(DiscussionDetailsListView.Items[0]);
        }

        private async void OpenInBrowserButton_Click(object sender, RoutedEventArgs e)
        {
            var forum = Common.Settings.Forum;
            await Launcher.LaunchUriAsync(new Uri($"https://{forum}/d/{NavigatingDiscussion}"));
        }

        private async void LoadMoreButton_Click(object sender, RoutedEventArgs e)
        {
            await GetDiscussionDetails(NavigatingDiscussion);
        }

        private async void OpenAPIButton_Click(object sender, RoutedEventArgs e)
        {
            var forum = Common.Settings.Forum;
            await Launcher.LaunchUriAsync(new Uri($"https://{forum}/api/discussions/{DiscussionInfo.id}?bySlug=true&page[near]={PostNumberToLoad}&page[limit]=20"));
        }



        private void ToBottomBButton_Click(object sender, RoutedEventArgs e)
        {
            if ((bool)ViewPosterAppBarToggleButton.IsChecked)
            {
                DiscussionDetailsListView.ScrollIntoView(DiscussionDetailsListView.Items[PosterPosts.Count() - 1]);
            }
            else
            {
                DiscussionDetailsListView.ScrollIntoView(DiscussionDetailsListView.Items[Posts.Count() - 1]);
            }
        }

        private void WrapPanelContainer_ItemClick(object sender, ItemClickEventArgs e)
        {
            var clicked = e.ClickedItem as Included;
            var forum = Common.Settings.Forum;
            NavigationService.Navigate<MainPage>($"https://{forum}/api/discussions?&filter[tag]={clicked.attributes.slug}");
        }

        private void ReplyButton_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            var data = btn.DataContext as Included;
            var id = data.id; 
            var user = data.attributes.user.displayName;
            string text = $"@\"{user}\"#p{id} ";
            string discussionName = DiscussionInfo.attributes.title;
            string[] navigate = { NavigatingDiscussion, discussionName,text};

            NavigationService.Navigate<ReplyPage>(navigate);
        }

        private void ReplyAppBarButton_Click(object sender, RoutedEventArgs e)
        {
            string discussionName = DiscussionInfo.attributes.title;
            string[] navigate = { NavigatingDiscussion, discussionName };
            NavigationService.Navigate<ReplyPage> (navigate);
        }

        private async void LoadAllButton_Click(object sender, RoutedEventArgs e)
        {
            
            while ((Posts[Posts.Count - 1].attributes.number >= DiscussionInfo.attributes.commentCount) != true)
            {
                await GetDiscussionDetails(NavigatingDiscussion);
                ToBottomBButton_Click(null, null);
            }


        }

        public static T FindVisualChildByName<T>(DependencyObject parent, string name) where T : DependencyObject
        {
            try
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
                {
                    var child = VisualTreeHelper.GetChild(parent, i);
                    string controlName = child.GetValue(Control.NameProperty) as string;
                    if ((string.IsNullOrEmpty(name) || controlName == name) && child is T)
                    {
                        return child as T;
                    }
                    else
                    {
                        T result = FindVisualChildByName<T>(child, name);
                        if (result != null)
                            return result;
                    }
                }
                return null;
            }
            catch
            {
                return null;
            }
        }

//在list
    }
    public class DiscussionNavigationInfo 
    {
        public int targetDiscussion { get; set; }
        public int targetPost { get; set; }
    }

}
