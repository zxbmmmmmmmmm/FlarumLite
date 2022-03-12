using FlarumLite.core.Models;
using FlarumLite.Helpers;
using FlarumLite.Services;
using FlarumLite.Views.DetailPages;
using System;
using System.Collections.Generic;
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
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“内容对话框”项模板

namespace FlarumLite.Views.Dialogs
{
    public sealed partial class DiscussionPreviewDialog : ContentDialog
    {
        private DiscussionNavigationInfo NavigatePost = new DiscussionNavigationInfo();
        public DiscussionPreviewDialog(string Id)
        {
            this.InitializeComponent();
            NavigatePost.targetDiscussion = int.Parse(Id);
        }

        public void BindData(Included included)
        {
            MainMarkdownTextBlock.Text = CSStoMarkdown.HTMLtoMarkdown(included.attributes.contentHtml);
                
        }

        private void ViewDiscussionButton_Click(object sender, RoutedEventArgs e)
        {
            NavigatePost.targetPost = 1;
            NavigationService.Navigate<DiscussionDetailPage>(NavigatePost);
            Hide();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Hide();
        }
    }
}
