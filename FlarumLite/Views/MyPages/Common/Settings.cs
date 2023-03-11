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
        /// ��̳��ַ
        /// Ĭ����̳�뵽Config.cs�ڸ���
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
        /// ��ǰ�ѵ�¼���û�ID
        /// �����������
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
        /// token�����������֤
        /// �����������
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
        /// ��ȡ����
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
        /// ������ת��ΪJson�󱣴�
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="propertyName"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static T GetSettingsWithClass<T>(string propertyName, T defaultValue)//ʹ��default value�е�T
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
                    var str = (string)ApplicationData.Current.LocalSettings.Values[propertyName];//��ȡ�ַ���
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
            //ʵ����DataContractJsonSerializer������Ҫ�����л��Ķ�������
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(obj.GetType());
            //ʵ����һ���ڴ��������ڴ�����л��������
            MemoryStream stream = new MemoryStream();
            //ʹ��WriteObject���л�����
            serializer.WriteObject(stream, obj);
            //д���ڴ�����
            byte[] dataBytes = new byte[stream.Length];
            stream.Position = 0;
            stream.Read(dataBytes, 0, (int)stream.Length);
            //ͨ��UTF8��ʽת��Ϊ�ַ���
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
