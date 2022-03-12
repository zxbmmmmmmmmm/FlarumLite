using FlarumLite.core.ViewModels;
using FlarumLite.Services;
using FlarumLite.Views.Controls;
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

namespace FlarumLite.Views.MyPages
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class EditForumPage : Page
    {
        public ObservableCollection<Forum> Forums = new ObservableCollection<Forum>();
        public EditForumPage()
        {
            this.InitializeComponent();
            Forums = GetForums();
            if(ApplicationData.Current.LocalSettings.Values["forum"] != null)
            {
                ForumAutoSuggestBox.Text = ApplicationData.Current.LocalSettings.Values["forum"].ToString();
            }
        }

        private ObservableCollection<Forum> GetForums()
        {
            var forums = new ObservableCollection<Forum>();

            forums.Add(new Forum { name = "wvbCommunity", website = "community.wvbtech.com",logo= "https://community.wvbtech.com/assets/avatars/okdBCGYRcqm84crb.png" });
            forums.Add(new Forum { name = "Flarum", website = "discuss.flarum.org", logo = "https://flarum.csur.fun/2022-02-19/1645260254-167356-logo.png" });
            forums.Add(new Forum { name = "Flarum中文站", website = "discuss.flarum.org.cn", logo = "https://flarum.csur.fun/2022-02-19/1645260254-167356-logo.png" });
            forums.Add(new Forum { name = "Flarum/demo", website = "demo.flarum.site", logo = "https://flarum.csur.fun/2022-02-19/1645260254-167356-logo.png" });
            forums.Add(new Forum { name = "Flarum/nightly", website = "nightly.flarum.site", logo = "https://flarum.csur.fun/2022-02-19/1645260254-167356-logo.png" });

            return forums;
        }

        private void RecommendForumsListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            EditForum((e.ClickedItem as Forum).website);

        }

        private void AutoSuggestBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            EditForum(args.QueryText);
        }
        public void EditForum(string website)
        {
            var localsettings = ApplicationData.Current.LocalSettings;
            localsettings.Values["forum"] = website;
            localsettings.Values["userId"] = null;
            localsettings.Values["token"] = "";
            new Toast("更改成功!请刷新页面(需要重新登录)").show();
            NavigationService.GoBack();
        }
    }
}
