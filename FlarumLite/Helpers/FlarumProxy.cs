using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Web.Http.Filters;
using FlarumLite.Views;
using System.Collections;
using FlarumLite.core.Models;
using FlarumLite.Views.MyPages;
using FlarumLite.Services;
using FlarumLite.Views.Controls;
using FlarumLite.Views.DetailPages;
using Windows.Storage.Streams;
using System.Diagnostics;
using FlarentApp.Helpers;
using FlarumApi.Models;
using Newtonsoft.Json;

namespace FlarumLite.Helpers
{
    class FlarumProxy
    {
        public static async Task<MainPagePosts> GetInfo (string link)
        {
            var response = new Windows.Web.Http.HttpResponseMessage();
            var client = new Windows.Web.Http.HttpClient();
            client.DefaultRequestHeaders.Add("user-agent", "Mozilla/5.0 (Windows NT 6.1; Win64; x64; rv:57.0) Gecko/20100101 Firefox/57.0");//模拟浏览器
            client.DefaultRequestHeaders.Add("Connection", "Keep-Alive");
            client.DefaultRequestHeaders.Add("Keep-Alive", "timeout=600");
            var token = Common.Settings.Token;
            client.DefaultRequestHeaders.Add("Authorization", "Token " + token);


            response = await client.GetAsync(new Uri(link));
            var result = await response.Content.ReadAsStringAsync();
            var serializer = new DataContractJsonSerializer(typeof(MainPagePosts));
            var ms = new MemoryStream(Encoding.UTF8.GetBytes(result));
            var data = (MainPagePosts)serializer.ReadObject(ms);
            return data;
        }

        public static async Task<DiscussionDetails> GetDiscussionDetails(string link)
        {
            var client = new Windows.Web.Http.HttpClient();
            client.DefaultRequestHeaders.Add("user-agent", "Mozilla/5.0 (Windows NT 6.1; Win64; x64; rv:57.0) Gecko/20100101 Firefox/57.0");//模拟浏览器
            client.DefaultRequestHeaders.Add("Connection", "Keep-Alive");
            client.DefaultRequestHeaders.Add("Keep-Alive", "timeout=600");

            var token = Common.Settings.Token;
            client.DefaultRequestHeaders.Add("Authorization", "Token " + token);

            var response = await client.GetAsync(new Uri(link));
            var result = await response.Content.ReadAsStringAsync();
            var serializer = new DataContractJsonSerializer(typeof(DiscussionDetails));
            var ms = new MemoryStream(Encoding.UTF8.GetBytes(result));
            var data = (DiscussionDetails)serializer.ReadObject(ms);
            return data;
        }
        public async static Task<LoginData> GetToken(string userName, string password)
        {
            var client = new HttpClient();

            client.DefaultRequestHeaders.Add("user-agent", "Mozilla/5.0 (Windows NT 6.1; Win64; x64; rv:57.0) Gecko/20100101 Firefox/57.0");//模拟浏览器
            client.DefaultRequestHeaders.Add("Connection", "Keep-Alive");
            client.DefaultRequestHeaders.Add("Keep-Alive", "timeout=600");

            var token = Common.Settings.Token;
            client.DefaultRequestHeaders.Add("Authorization", "Token " + token);


            var values = new Dictionary<string, string>
            {
                { "identification", userName },
                { "password", password }
            };

            var content = new FormUrlEncodedContent(values);
            var forum = Common.Settings.Forum;
            var response = await client.PostAsync($"https://{forum}/api/token", content);
            var code = response.StatusCode;

            var responseString = await response.Content.ReadAsStringAsync();

            var serializer = new DataContractJsonSerializer(typeof(LoginData));
            var ms = new MemoryStream(Encoding.UTF8.GetBytes(responseString));
            var data = (LoginData)serializer.ReadObject(ms);

            data.statusCode = code.ToString();
            return data;

        }

        public static async Task<Catagories> GetCatagories(string link)
        {
            var client = new Windows.Web.Http.HttpClient();
            client.DefaultRequestHeaders.Add("user-agent", "Mozilla/5.0 (Windows NT 6.1; Win64; x64; rv:57.0) Gecko/20100101 Firefox/57.0");//模拟浏览器
            client.DefaultRequestHeaders.Add("Connection", "Keep-Alive");
            client.DefaultRequestHeaders.Add("Keep-Alive", "timeout=600");

            var token = Common.Settings.Token;
            client.DefaultRequestHeaders.Add("Authorization", "Token " + token);

            var response = await client.GetAsync(new Uri(link));
            var result = await response.Content.ReadAsStringAsync();
            var serializer = new DataContractJsonSerializer(typeof(Catagories));
            var ms = new MemoryStream(Encoding.UTF8.GetBytes(result));
            var data = (Catagories)serializer.ReadObject(ms);
            return data;
        }
        public static async Task<Notifications> GetNotifications(string link)
        {
            var client = new Windows.Web.Http.HttpClient();
            client.DefaultRequestHeaders.Add("user-agent", "Mozilla/5.0 (Windows NT 6.1; Win64; x64; rv:57.0) Gecko/20100101 Firefox/57.0");//模拟浏览器
            client.DefaultRequestHeaders.Add("Connection", "Keep-Alive");
            client.DefaultRequestHeaders.Add("Keep-Alive", "timeout=600");

            var token = Common.Settings.Token;
            client.DefaultRequestHeaders.Add("Authorization", "Token " + token);

            var response = await client.GetAsync(new Uri(link));
            var result = await response.Content.ReadAsStringAsync();
            var serializer = new DataContractJsonSerializer(typeof(Notifications));
            var ms = new MemoryStream(Encoding.UTF8.GetBytes(result));
            var data = (Notifications)serializer.ReadObject(ms);
            return data;
        }

        public static async void UpdateToken()//更新Token
        {
            ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
            if (localSettings.Values["loginName"] != null)
            {
                var loginName = localSettings.Values["loginName"].ToString();
                var loginPassword = localSettings.Values["loginPassword"].ToString();
                var loginData = await FlarumProxy.GetToken(loginName, loginPassword);
                switch (loginData.statusCode)
                {
                    case "Unauthorized"://用户名或密码错误
                        
                        localSettings.Values["userId"] = null;
                        localSettings.Values["token"] = "0";
                        new Toast("获取Token失败，请重新登录").show();
                        NavigationService.Navigate<LoginPage>();//跳转到登录页面
                        //var notification = new LoginAgainInAppNotification();
                        //notification.ShowLoginNotification();
                        break;
                    case "OK"://登录成功
                        new Toast("登录成功!请刷新当前页面").show();
                        localSettings.Values["userId"] = loginData.userId;
                        localSettings.Values["token"] = loginData.token;
                        break;
                }
            }

        }
        public async static Task<HttpResponseMessage> Reply(string discussion,string text)
        {
            var client = new HttpClient();

            client.DefaultRequestHeaders.Add("user-agent", "Mozilla/5.0 (Windows NT 6.1; Win64; x64; rv:57.0) Gecko/20100101 Firefox/57.0");//模拟浏览器
            client.DefaultRequestHeaders.Add("Connection", "Keep-Alive");
            client.DefaultRequestHeaders.Add("Keep-Alive", "timeout=600");

            var token = Common.Settings.Token;
            client.DefaultRequestHeaders.Add("Authorization", "Token " + token);

            var contents = new Dictionary<string, object>
            {
                {"data",new Dictionary<string,object>
                {
                    {"type","posts" },
                    {"attributes",new Dictionary<string,string>
                    {
                        {"content",text }
                    }
                    },
                    {"relationships",new Dictionary<string,object>
                    {
                        {"discussion",new Dictionary<string,object>
                        {
                            { "data",new Dictionary<string,string>
                            {
                                {"type","discussions" },
                                {"id",discussion.ToString() }
                            }
                            }
                        }
                        }
                    }
                    }
                }
                }
            };
            var json = JsonConvert.SerializeObject(contents, Formatting.None);
            var forum = Common.Settings.Forum;
            var response = await client.PostAsync($"https://{forum}/api/posts", new StringContent(json, System.Text.Encoding.UTF8, "application/json"));
            var code = response.StatusCode;

            var responseString = await response.Content.ReadAsStringAsync();

            /*var serializer = new DataContractJsonSerializer(typeof(LoginData));
            var ms = new MemoryStream(Encoding.UTF8.GetBytes(responseString));
            var data = (LoginData)serializer.ReadObject(ms);*/

            return response;

        }
        public static async void UploadFile(string discussion)//上传文件
        {
            var picker = new Windows.Storage.Pickers.FileOpenPicker();
            picker.ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail;
            picker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.PicturesLibrary;
            picker.FileTypeFilter.Add(".jpg");
            picker.FileTypeFilter.Add(".jpeg");
            picker.FileTypeFilter.Add(".png");
            StorageFile file = await picker.PickSingleFileAsync();//选择文件
            if(file != null)
            {
                var client = new HttpClient();
                var token = Common.Settings.Token;
                client.DefaultRequestHeaders.Add("Authorization", "Token " + token);
                var forum = Common.Settings.Forum;
                client.DefaultRequestHeaders.Add("user-agent", "Mozilla/5.0 (Windows NT 6.1; Win64; x64; rv:57.0) Gecko/20100101 Firefox/57.0");
                client.DefaultRequestHeaders.Add("Connection", "Keep-Alive");
                client.DefaultRequestHeaders.Add("Keep-Alive", "timeout=600");
                client.DefaultRequestHeaders.Add("Referer", $"https://{forum}/d/{discussion}/21");



                var content = new MultipartFormDataContent();
                var streamData = await file.OpenReadAsync();
                var bytes = new byte[streamData.Size];
                using (var dataReader = new DataReader(streamData))
                {
                    await dataReader.LoadAsync((uint)streamData.Size);
                    dataReader.ReadBytes(bytes);
                }
                var streamContent = new ByteArrayContent(bytes);
                content.Add(streamContent,"files[]");

                var response = await client.PostAsync(new Uri($"https://{forum}/api/fof/upload"),content);//上传
                var responseString = await response.Content.ReadAsStringAsync();

            }

        }
        private static string getJsonByObject(Object obj)
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
    }
}
