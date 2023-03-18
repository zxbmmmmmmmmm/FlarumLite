using FlarentApp.Helpers;
using FlarumApi;
using FlarumApi.Models;
using FlarumLite.Services;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace FlarumLite.Views.DetailPages
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class DiscussionDetailPageNew : Page,INotifyPropertyChanged
    {
        public DiscussionDetailPageNew()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = NavigationCacheMode.Required;
        }

        public ScrollViewer PostScrollViewer = new ScrollViewer();
        //public Discussion Discussion= new Discussion();
        public List<int> PostIds = new List<int>();
        //public int CurrentPage;
        public int LastPosts;
        public int TotalPages;
        public int discussionId;
        // bool isLastPage = false;
        public Discussion Discussion
        {
            get => _discussion;
            set
            {
                if (_discussion != value)
                {
                    _discussion = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Discussion)));
                }
            }
        }
        private Discussion _discussion;
        public int CurrentPage
        {
            get => _currentPage;
            set
            {
                if (_currentPage != value)
                {
                    _currentPage = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CurrentPage)));
                }
            }
        }
        private int _currentPage;

        public bool IsLastPage
        {
            get => _isLastPage;
            set
            {
                if (_isLastPage != value)
                {
                    _isLastPage = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsLastPage)));
                }
            }
        }
        private bool _isLastPage;
        public bool IsFirstPage
        {
            get => _isFirstPage;
            set
            {
                if (_isFirstPage != value)
                {
                    _isFirstPage = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsFirstPage)));
                }
            }
        }
        private bool _isFirstPage;

        public event PropertyChangedEventHandler PropertyChanged;

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            var anim = ConnectedAnimationService.GetForCurrentView().GetAnimation("ForwardConnectedAnimation");

            if (anim != null)
            {
                anim.TryStart(this);
            }
            if (e.NavigationMode == NavigationMode.Back)
                return;

            if (e.Parameter != null)
            {
                discussionId = (int)e.Parameter;
            }
            await GetDiscussion();
            await TurnToPage(0);

        }

        /// <summary>
        /// 获取讨论信息
        /// </summary>
        public async Task GetDiscussion()
        {
            PostsListView.Visibility = Visibility.Collapsed;
            LoadingControl.IsLoading = true;
            ReplyButton.IsEnabled = false;
            Discussion = await FlarumApiProviders.GetDiscussion(discussionId, 0, Common.Settings.Forum, Common.Settings.Token);
            PostIds = new List<int>();
            foreach (var item in Discussion.Posts)//获取帖子列表
            {
                PostIds.Add((int)item.Id);
            }
            CurrentPage = 0;
            TotalPages = (int)Math.Ceiling((double)Discussion.Posts.Count / 30);
            LastPosts = PostIds.Count % 30;//将post的数量取余30，得到剩余的post数量（29）
            PostsListView.Visibility = Visibility.Visible;
            ReplyButton.IsEnabled = true;            
            LoadingControl.IsLoading = false;
        }
        private async Task GetPost(int min, int range)
        {
            var list = PostIds.GetRange(min, range);
            var data = await FlarumApiProviders.GetPostsWithId(list, Common.Settings.Forum, Common.Settings.Token);
            foreach(var post in data)
            {
                if (post.ContentHtml == null)
                {
                    switch (post.ContentType)
                    {
                        case "discussionStickied":
                            post.ContentHtml = "*置顶了此贴*";
                            break;
                        case "discussionTagged":
                            post.ContentHtml = "*更改了标签*";
                            break;
                        case "discussionLocked":
                            post.ContentHtml = "*锁定了此贴*";
                            break;
                        case "discussionRenamed":
                            post.ContentHtml = "*更改了标题*";
                            break;
                        case "discussionSplit":
                            post.ContentHtml = "*拆分了回复*";
                            break;
                        default:
                            post.ContentHtml = "*未知操作*";
                            break;
                    }


                }
            }
            PostsListView.ItemsSource = data;
            PageTextBlock.Text = $"{CurrentPage + 1}/{TotalPages}页";
            PagePostsTextBlock.Text = $"第{min + 1}-{min + range}条回复";


        }
        public static ScrollViewer GetScrollViewer(DependencyObject parent)
        {
            if (parent == null)
                return null;
            var count = VisualTreeHelper.GetChildrenCount(parent);

            for (int i = 0; i < count;)
            {
                var item = VisualTreeHelper.GetChild(parent, i);
                if (item is ScrollViewer viewer)
                {
                    return viewer;
                }
                else
                {
                    return GetScrollViewer(item);
                }
            }
            return null;
        }

        private async void NextPageButton_Click(object sender, RoutedEventArgs e)
        {
            //IsLastPage = CurrentPage + 2 >= TotalPages;
            //if (IsLastPage)  
            //PostSlider.Value = (CurrentPage + 1) * 30;          
            //else
            //PostSlider.Value = (CurrentPage + 1) * 30;

            await TurnToPage(CurrentPage + 1);
        }

        private async Task TurnToPage(int page)
        {
            LoadingControl.IsLoading = true;

            CurrentPage = page;

            IsFirstPage = CurrentPage == 0;

            IsLastPage = TotalPages <= CurrentPage + 1;
            var min = CurrentPage * 30;//最小值
            var range = 30;
            if (IsLastPage && LastPosts != 0)
            {
                range = LastPosts;
            }
            await GetPost(min, range);

            LoadingControl.IsLoading = false;


        }

        private async void PreviousPageButton_Click(object sender, RoutedEventArgs e)
        {
            await TurnToPage(CurrentPage - 1);
            if (PostScrollViewer == null) return;
            PostScrollViewer.ChangeView(PostScrollViewer.HorizontalOffset, PostScrollViewer.ExtentHeight, PostScrollViewer.ZoomFactor);//导航到底部
        }

        private void ReplyButton_Click(object sender, RoutedEventArgs e)
        {

            string[] navigate = { discussionId.ToString(), Discussion.Title };

            NavigationService.Navigate<ReplyPage>(navigate);
        }

        private async void DownloadItem_Click(object sender, RoutedEventArgs e)
        {
            var tuple = new Tuple<List<int>, Discussion>(PostIds, Discussion);
        }
        public async void TurnToLastPage()
        {
            await TurnToPage(TotalPages - 1);
            if (PostScrollViewer == null) return;
            PostScrollViewer.ChangeView(PostScrollViewer.HorizontalOffset, PostScrollViewer.ExtentHeight, PostScrollViewer.ZoomFactor);//导航到底部
        }

        private async void RefreshItem_Click(object sender, RoutedEventArgs e)
        {
            await GetDiscussion();
            await TurnToPage(0);
        }


        private void PageBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            var text = sender.Text;
            int page;
            if (int.TryParse(text, out page))
            {
                if (!(page <= TotalPages) || !(page > 0))
                    sender.Text = "";
            }
            else
                sender.Text = "";
        }

        private async void PageBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            await TurnToPage(int.Parse(sender.Text) - 1);
            sender.Text = "";
        }
    }
}

