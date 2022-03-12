using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace FlarumLite.Helpers.ValueConverters
{

    public class NotificationTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
            {
                return null;
            }
            else
            {
                string converted;
                switch (value.ToString())
                {
                    case "postReacted":
                        converted = "戳了一个";
                        break;
                    case "newPost":
                        converted = "回复了您关注的主题";
                        break;
                    case "postMentioned":
                        converted = "回复了您";
                        break;
                    case "vote":
                        converted = "赞同了您";
                        break;
                    case "postLiked":
                        converted = "赞了您";
                        break;
                    default:
                        converted = value.ToString();
                        break;
                }
                return converted;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
    public class NotificationIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
            {
                return null;
            }
            else
            {
                string converted;
                switch (value.ToString())
                {
                    case "postReacted":
                        converted = "\uED58";
                        break;
                    case "newPost":
                        converted = "\uE90A";
                        break;
                    case "postMentioned":
                        converted = "\uE97A";
                        break;
                    case "vote":
                        converted = "\uE19F";
                        break;
                    case "postLiked":
                        converted = "\uE19F";
                        break;
                    default:
                        converted = "\uEA8F";
                        break;
                }
                return converted;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
    public class EmojiConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
            {
                return null;
            }
            else
            {
                string converted;
                switch (value.ToString())
                {
                    case "thinkin":
                        converted = "🤔";
                        break;
                    case "heart":
                        converted = "❤️";
                        break;
                    case "tada":
                        converted = "🎉";
                        break;
                    case "rofl":
                        converted = "🤣";
                        break;
                    case "herb":
                        converted = "🌿";
                        break;
                    case "lemon":
                        converted = "🍋";
                        break;
                    default:
                        converted = value.ToString();
                        break;
                }
                return converted;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

}
