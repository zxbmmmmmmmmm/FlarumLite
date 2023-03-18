using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Storage;
using Windows.UI.Core;
using FlarumLite.core.ViewModels;
using System.IO;
using System.Runtime.Serialization.Json;

namespace FlarentApp.Helpers
{
    public static class Common
    {
        public static Settings Settings = new Settings();
    }

    public class Settings : INotifyPropertyChanged
    {
        /// <summary>
        /// 论坛网址
        /// 默认论坛请到Config.cs内更改
        /// </summary>
        public string Forum
        {
            get => GetSettings("Forum", "community.wvbtech.com");
            set
            {
                ApplicationData.Current.LocalSettings.Values["Forum"] = value;
                OnPropertyChanged();
            }
        }


        /// <summary>
        /// 当前已登录的用户ID
        /// 请勿随意更改
        /// </summary>
        public int UserId
        {
            get => GetSettings("UserId", 0);
            set
            {
                ApplicationData.Current.LocalSettings.Values["UserId"] = value;
                OnPropertyChanged();
            }
        }


        /// <summary>
        /// token，用于身份验证
        /// 请勿随意更改
        /// </summary>
        public string Token
        {
            get => GetSettings("Token", "");
            set
            {
                ApplicationData.Current.LocalSettings.Values["Token"] = value;
                OnPropertyChanged();
            }
        }
        public string LoginName
        {
            get => GetSettings("LoginName", "");
            set
            {
                ApplicationData.Current.LocalSettings.Values["LoginName"] = value;
                OnPropertyChanged();
            }
        }
        public string LoginPassword
        {
            get => GetSettings("LoginPassword", "");
            set
            {
                ApplicationData.Current.LocalSettings.Values["LoginPassword"] = value;
                OnPropertyChanged();
            }
        }



        public event PropertyChangedEventHandler PropertyChanged;

        public async void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (PropertyChanged != null)
                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                    () => { PropertyChanged(this, new PropertyChangedEventArgs(propertyName)); });
        }
        /// <summary>
        /// 获取设置
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="propertyName"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static T GetSettings<T>(string propertyName, T defaultValue)
        {
            try
            {
                if (ApplicationData.Current.LocalSettings.Values.ContainsKey(propertyName) &&
                    ApplicationData.Current.LocalSettings.Values[propertyName] != null &&
                    !string.IsNullOrEmpty(ApplicationData.Current.LocalSettings.Values[propertyName].ToString()))
                {
                    if (typeof(T).ToString() == "System.Boolean")
                        return (T)(object)bool.Parse(ApplicationData.Current.LocalSettings.Values[propertyName]
                            .ToString());

                    return (T)ApplicationData.Current.LocalSettings.Values[propertyName];
                }
                else
                {
                    ApplicationData.Current.LocalSettings.Values[propertyName] = defaultValue;
                }
                return defaultValue;
            }
            catch
            {
                return defaultValue;
            }
        }
        /// <summary>
        /// 将设置转换为Json后保存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="propertyName"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static T GetSettingsWithClass<T>(string propertyName, T defaultValue)//使用default value中的T
        {
            try
            {
                if (ApplicationData.Current.LocalSettings.Values.ContainsKey(propertyName) &&
                    ApplicationData.Current.LocalSettings.Values[propertyName] != null &&
                    !string.IsNullOrEmpty(ApplicationData.Current.LocalSettings.Values[propertyName].ToString()))
                {
                    if (typeof(T).ToString() == "System.Boolean")
                        return (T)(object)bool.Parse(ApplicationData.Current.LocalSettings.Values[propertyName]
                            .ToString());
                    var str = (string)ApplicationData.Current.LocalSettings.Values[propertyName];//获取字符串
                    return (T)JsonHelper.GetObjectByJson<T>(str);
                }
                else
                {
                    ApplicationData.Current.LocalSettings.Values[propertyName] = JsonHelper.GetJsonByObject(defaultValue);
                }
                return defaultValue;
            }
            catch
            {
                return defaultValue;
            }
        }
    }
    public class JsonHelper
    {
        public static string GetJsonByObject(Object obj)
        {
            //实例化DataContractJsonSerializer对象，需要待序列化的对象类型
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(obj.GetType());
            //实例化一个内存流，用于存放序列化后的数据
            MemoryStream stream = new MemoryStream();
            //使用WriteObject序列化对象
            serializer.WriteObject(stream, obj);
            //写入内存流中
            byte[] dataBytes = new byte[stream.Length];
            stream.Position = 0;
            stream.Read(dataBytes, 0, (int)stream.Length);
            //通过UTF8格式转换为字符串
            return Encoding.UTF8.GetString(dataBytes);
        }
        public static object GetObjectByJson<T>(string str)
        {
            var serializer = new DataContractJsonSerializer(typeof(T));
            var ms = new MemoryStream(Encoding.UTF8.GetBytes(str));
            var data = (T)serializer.ReadObject(ms);
            return data;
        }
    }
}
