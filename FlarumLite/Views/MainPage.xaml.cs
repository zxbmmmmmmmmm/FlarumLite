using FlarumLite.Helpers;
using FlarumLite.Helpers.ValueConverters;
using FlarumLite.Services;
using FlarumLite.Views.Dialogs;
using FlarumLite.Views.DetailPages;
using FlarumLite.core.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using FlarumLite.Views.Controls;
using Microsoft.Toolkit.Uwp.UI.Controls;
using Windows.Storage;

// https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x804 上介绍了“空白页”项模板

namespace FlarumLite.Views
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPagePosts MainPageData = new MainPagePosts();
        public ObservableCollection<Datum> Posts = new ObservableCollection<Datum>();
        public ObservableCollection<Included> Includeds = new ObservableCollection<Included>();
        public Links Links = new Links();
        public string TargetLink;
        public bool CanNavigate;

        public MainPage()
        {
            this.NavigationCacheMode = NavigationCacheMode.Required;
            this.InitializeComponent();
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            CanNavigate = true;

            if (e.NavigationMode != NavigationMode.Back)//判断是不是按了返回键载入的
            {
                ClearAll();
                if (e.Parameter.ToString().Contains("https://"))
                {
                    TargetLink = e.Parameter.ToString();
                }
                else
                {
                    var forum = ApplicationData.Current.LocalSettings.Values["forum"].ToString();
                    TargetLink = $"https://{forum}/api/discussions";
                }
                GetMainPagePostsData(TargetLink);
            }
            else
            {
                return;
            }
        }

        private void ClearAll()
        {
            Posts.Clear();
            //Includeds.Clear();
            MainPageData = new MainPagePosts();
            Links = new Links();
        }

        private async void GetMainPagePostsData(string link)
        {
            var addingData = new MainPagePosts();
            LoadMoreButton.Visibility = Visibility.Collapsed;
            LoadingProgressBar.Visibility = Visibility.Visible;
            addingData = await FlarumProxy.GetInfo(link);
            var addingPosts = addingData.data;

            var addingUsers = new ObservableCollection<Included>();
            var addingTags = new ObservableCollection<Included>();

            var addingIncludeds = addingData.included; 
            foreach (var included in addingIncludeds)
            {
                if(included.type == "users")
                {
                    addingUsers.Add(included);
                }
                else if (included.type == "tags")
                {
                    addingTags.Add(included);
                }
                Includeds.Add(included);
            }

            //Includeds = MainPageData.included;
            
            addingPosts = GetPostUsers(addingPosts,addingUsers);

            foreach (var post in addingPosts)
            {
                if (post.relationships.firstPost != null)
                {
                    post.attributes.firstPost = addingIncludeds.First(p => p.id == post.relationships.firstPost.data.id);
                }
                post.tags = new ObservableCollection<Included>();
                foreach(var tag in post.relationships.tags.data)
                {
                    post.tags.Add(addingTags.FirstOrDefault(p => p.id == tag.id));
                    
                }
                Posts.Add(post);
            }
            MainPageListView.ItemsSource = Posts;
            Links = addingData.links;
            LoadingProgressBar.Visibility = Visibility.Collapsed;     
            if(Links.next != null)
            {
                LoadMoreButton.Visibility = Visibility.Visible;

            }
        }

        private ObservableCollection<Datum> GetPostUsers(ObservableCollection<Datum> addingPosts,ObservableCollection<Included> users)
        {
            foreach (var user in users)
            {
                if (user.attributes.avatarUrl == null)
                {
                   user.attributes.avatarUrl = "https://flarum.csur.fun/2022-02-08/1644323380-214777-guest.png";
                }

            }

            foreach (var post in addingPosts)
            {

                if (post.relationships.user!= null)
                {
                    post.attributes.user = users.First(p => p.id == post.relationships.user.data.id).attributes;
                }
                else
                {
                    post.attributes.user = new Attributes { displayName = "【已注销】", name = "已注销" ,avatarUrl = "https://flarum.csur.fun/2022-02-08/1644323380-214777-guest.png" };
                }

                if (post.relationships.lastPostedUser != null)
                {
                    post.attributes.lastPostedUser = users.First(p => p.id == post.relationships.lastPostedUser.data.id).attributes;
                }
                else
                {
                    post.attributes.lastPostedUser = new Attributes { displayName = "【已注销】", name = "已注销" ,avatarUrl = "https://flarum.csur.fun/2022-02-08/1644323380-214777-guest.png" };
                }

            }
            return addingPosts;
        }

        private void MainPageListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            if(e.ClickedItem is Datum item)
            {
                var navigate = new DiscussionNavigationInfo { targetDiscussion = int.Parse(item.id), targetPost = -1 };
                var page = new DiscussionDetailPage();
                NavigationService.Navigate<DiscussionDetailPage>(navigate);
            }
        }

        private void LoadMoreButton_Click(object sender, RoutedEventArgs e)
        {
            GetMainPagePostsData(Links.next);
        }

        private void UserHyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as HyperlinkButton;
            var clicked = btn.DataContext as Datum;
            if (clicked.relationships.lastPostedUser != null)
            {
                NavigationService.Navigate<UserDetailPage>(clicked.relationships.lastPostedUser.data.id);
            }
        }

        private void PosterButton_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            var clicked = btn.DataContext as Datum;
            if (clicked.relationships.lastPostedUser != null)
            {
                NavigationService.Navigate<UserDetailPage>(clicked.relationships.user.data.id);
            }
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            ClearAll();
            GetMainPagePostsData(TargetLink);
        }

        private void MainPageListView_RefreshRequested(object sender, EventArgs e)
        {
            ClearAll();
            GetMainPagePostsData(TargetLink);
        }

        private async void SlidableListItem_RightCommandRequested(object sender, EventArgs e)
        {
            var item = sender as SlidableListItem;
            var clicked = item.DataContext as Datum;

            var dialog = new DiscussionPreviewDialog(clicked.id);
            dialog.BindData(clicked.attributes.firstPost);
            await dialog.ShowAsync();

        }

        private void WrapPanelContainer_ItemClick(object sender, ItemClickEventArgs e)
        {

            var clicked = e.ClickedItem as Included;
            var forum = ApplicationData.Current.LocalSettings.Values["forum"].ToString();
            NavigationService.Navigate<MainPage>($"https://{forum}/api/discussions?&filter[tag]={clicked.attributes.slug}");
            CanNavigate = false;


        }

        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            //width.Width = WidthFit.GetWidth(ActualWidth, 600, 300);
            width.Width = ActualWidth - 100;

        }
    }
}
