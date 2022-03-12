using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace FlarumLite.Helpers
{
    public class NavigateHelper 
    {
        private static readonly Dictionary<Type, Stack<PageStackContent>> DictPageContent
= new Dictionary<Type, Stack<PageStackContent>>();
        public static Frame MainContentFrame { get; set; }
        public static void OnNavigatedTo(Type pageType, NavigationMode mode, Action newPageCallBack = null,
Action<object> backPageCallBack = null)
        {
            if (mode == NavigationMode.New || mode == NavigationMode.Refresh)
            {
                newPageCallBack?.Invoke();
            }
            else if (mode == NavigationMode.Back)
            {
                object pageParameter = null;
                if (DictPageContent.ContainsKey(pageType) && DictPageContent[pageType].Count != 0)
                {
                    var temp = DictPageContent[pageType].Pop();
                    MainContentFrame.Content = temp.PageContent;
                    pageParameter = temp.PageParameter;
                }
                backPageCallBack?.Invoke(pageParameter);
            }
        }
    }

    public class PageStackContent
    {

        public PageStackContent()
        {
        }
        public PageStackContent(object pageContent, object pageParameter = null)
        {
            this.PageContent = pageContent;
            this.PageParameter = pageParameter;
        }

        public object PageContent { get; set; }
        public object PageParameter { get; set; }
    }
}
