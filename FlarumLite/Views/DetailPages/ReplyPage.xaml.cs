using FlarumLite.core.Models;
using FlarumLite.Helpers;
using FlarumLite.Services;
using FlarumLite.Views.Controls;
using FlarumLite.Views.MyPages;
using Microsoft.Toolkit.Uwp.UI.Controls.TextToolbarButtons;
using Microsoft.Toolkit.Uwp.UI.Controls.TextToolbarFormats.MarkDown;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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
    public sealed partial class ReplyPage : Page
    {
        public string discussion;
        public string TailsStr;
        public ReplyPage()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = NavigationCacheMode.Required;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            //var navigate = e.Parameter as DiscussionNavigationInfo;
            ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
            TailsStr = TailsHelper.CustonTails(localSettings.Values["tails"].ToString());
            var str = TailsStr;
            str = str.Replace("\r\n\r\n", "\n\nㅤ\n\n");
            str = str.Replace("\r\n", "\n\n");
            TailsTextBlock.Text = str;
            if(Toolbar.CustomButtons.Count == 0)
            {
                AddButtons();

            }


            if (e.NavigationMode == NavigationMode.Back)//判断是不是按了返回键载入的
            {
                return;
            }
            else
            {
                var partner = e.Parameter as string[];

                DiscussionTextBlock.Text = partner[1];
                if (partner.Length == 3)
                {
                    string text = partner[2];
                    EditZone.Document.SetText(Windows.UI.Text.TextSetOptions.None, text);
                }

                discussion = partner[0];             
            }
        }

        private void EditZone_TextChanged(object sender, RoutedEventArgs e)
        {

            //string text = Toolbar.Formatter?.Text;
            string text;
            EditZone.Document.GetText(Windows.UI.Text.TextGetOptions.UseCrlf, out text);
            text = text.Replace("\r\n\r\n", "\n\n \n\n");
            text = text.Replace("\r\n", "\n\n");

            Previewer.Text = string.IsNullOrWhiteSpace(text) ? "*部分内容可能和发布后不同*" : text;




        }

        private void Previewer_LinkClicked(object sender, Microsoft.Toolkit.Uwp.UI.Controls.LinkClickedEventArgs e)
        {
            try
            {
                var link = new Uri(e.Link);
                var linkOpen = Task.Run(() => Launcher.LaunchUriAsync(link));
            }
            catch
            {
            }

        }

        private async void SendButton_Click(object sender, RoutedEventArgs e)
        {
            string text;
            EditZone.Document.GetText(Windows.UI.Text.TextGetOptions.UseLf, out text);
            if ((bool)TailsCheckBox.IsChecked)
            {
                text = $"{text}\n\n{TailsStr}";
            }
            SendButton.IsEnabled = false;
            try
            {
                var res = await FlarumProxy.Reply(discussion, text);
                if (res.IsSuccessStatusCode)
                {
                    new Toast("发送成功").show();
                    NavigationService.GoBack();
                }
                else
                {
                    new Toast(res.StatusCode.ToString()).show();
                }
            }
            catch
            {
                new Toast("发送失败").show();
            }
            SendButton.IsEnabled = true;
        }

        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            width.Width = ActualWidth - 80;
        }

        private void EditTailsButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate<EditTailsPage>();
        }
        public void AddButtons()
        {
            string demoText = "上传";


            var demoButton = new ToolbarButton
            {
                Icon = new SymbolIcon { Symbol = Symbol.Upload },
                ToolTip = demoText,

            };
            demoButton.Click += demoButton_Click;
            Toolbar.CustomButtons.Add(demoButton);

        }
        private async void demoButton_Click(object sender, RoutedEventArgs e)
        {
            FlarumProxy.UploadFile(discussion);
        }
    }
    public class Reply
    {
        public string type { get; set; }
        public Relationships relationships { get; set; }
        public ReplyAttributes attributes { get; set; }
    }
    public class ReplyData
    {
        public Reply data { get; set; }
    }
    public class ReplyAttributes
    {
        public string content { get; set; }
    }
}
