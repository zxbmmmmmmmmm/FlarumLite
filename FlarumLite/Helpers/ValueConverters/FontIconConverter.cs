using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace FlarumLite.Helpers.ValueConverters
{

    public class FontIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
            {
                return null;
            }
            else
            {
                string converted = string.Empty;
                switch (value.ToString())
                {
                    case "fas fa-code-branch"://开发日志
                        converted = "\uE9D5";
                        break;
                    case "fas fa-quote-right"://天南海北
                        converted = "\uE134";
                        break;
                    case "fas fa-bullhorn"://论坛站务
                        converted = "\uE789";
                        break;
                    case "fab fa-windows"://windows
                        converted = "\uECA5";
                        break;
                    case "fas fa-flask"://beta
                        converted = "\uF1AD";
                        break;
                    case "fab fa-microsoft"://office
                        converted = "\uF0E2";
                        break;
                    case "fas fa-desktop"://PC
                        converted = "\uE977";
                        break;
                    case "fab fa-apple"://Apple
                        converted = "\uE130";//未找到
                        break;
                    case "fas fa-unlink"://OS X
                        converted = "\uE167";
                        break;
                    case "fas fa-project-diagram"://开源生态
                        converted = "\uF003";
                        break;
                    case "fas fa-pen"://内容创作
                        converted = "\uEDFB";
                        break;
                    case "fas fa-globe"://网络社交
                        converted = "\uE12B";
                        break;
                    case "fas fa-comment-alt"://Flarum
                        converted = "\uE15F";
                        break;
                    case "fas fa-microchip"://复古电子
                        converted = "\uE964";
                        break;
                    case "fas fa-rss"://IT资讯
                        converted = "\uE95A";
                        break;
                    case "fas fa-comment"://意见反馈
                        converted = "\uED15";
                        break;
                    case "fas fa-box-open"://开箱专场
                        converted = "\uF133";
                        break;
                    case "fas fa-mobile"://移动设备
                        converted = "\uE1C9";
                        break;
                    case "fas fa-user-secret"://里世界
                        converted = "\uE727";
                        break;
                    case "fas fa-camera-retro"://玄学电子
                        converted = "\uE114";
                        break;
                    case "fas fa-server"://网络技术
                        converted = "\uE968";
                        break;
                    case "fas fa-gamepad"://游戏娱乐
                        converted = "\uE7FC";
                        break;
                    case "fas fa-paint-brush"://ACGN
                        converted = "\uE790";
                        break;
                    case "fas fa-vote-yea"://选举专区
                        converted = "\uE749";
                        break;
                    case "fas fa-book-open"://吧史专区
                        converted = "\uE736";
                        break;
                    case "fas fa-magic"://系统美化
                        converted = "\uE771";
                        break;
                    case "fas fa-code"://编程开发
                        converted = "\uE943";
                        break;
                    case "fas fa-tv"://放送文化
                        converted = "\uE7F4";
                        break;
                    case "fas fa-globe-asia"://旅游交通
                        converted = "\uE128";
                        break;
                    case "fas fa-boxes"://资源专区
                        converted = "\uED25";
                        break;
                    case "fas fa-terminal"://*nix
                        converted = "\uE62F";
                        break;
                    default:
                        converted = "\uE130";
                        break;

                }


                return converted;
            }
        }
        public object ConvertBack(object value, Type targetType, object parameter, string language) => DependencyProperty.UnsetValue;

    }
    public class IconVisiblityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
            {
                return null;
            }
            else
            {
                if((int)value == 1)
                {
                    return Visibility.Collapsed;
                }
                else
                {
                    return Visibility.Visible;
                }
            }
        }
        public object ConvertBack(object value, Type targetType, object parameter, string language) => DependencyProperty.UnsetValue;

    }

}