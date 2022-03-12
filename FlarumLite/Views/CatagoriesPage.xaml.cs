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
    public sealed partial class CatagoriesPage : Page
    {
        public Catagories CatagoriesData= new Catagories();
        public ObservableCollection<Catagory> Catagories = new ObservableCollection<Catagory>();
        public CatagoriesPage()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = NavigationCacheMode.Required;
            GetCatagories();
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {

            base.OnNavigatedTo(e);

            if (e.NavigationMode != NavigationMode.Back)//判断是不是按了返回键载入的
            {
                GetCatagories();
            }
            else
            {
                return;
            }
        }


        private async void GetCatagories()
        {
            LoadingProgressBar.Visibility = Visibility.Visible;
            var forum = ApplicationData.Current.LocalSettings.Values["forum"].ToString();
            CatagoriesData = await FlarumProxy.GetCatagories($"https://{forum}/api/tags");
            Catagories = CatagoriesData.data;
            CatagoriesListView.ItemsSource = Catagories;
            LoadingProgressBar.Visibility = Visibility.Collapsed;
        }

        private void CatagoriesListView_RefreshRequested(object sender, EventArgs e)
        {
            GetCatagories();
        }

        private void CatagoriesListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var clicked = e.ClickedItem as Catagory;
            var forum = ApplicationData.Current.LocalSettings.Values["forum"].ToString();
            NavigationService.Navigate<MainPage>($"https://{forum}/api/discussions?&filter[tag]={clicked.attributes.slug}");

        }

        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            //width.Width = (WidthFit.GetWidth(ActualWidth, 500, 250)) - 100;
            width.Width = ActualWidth - 200;
        }
    }
}
