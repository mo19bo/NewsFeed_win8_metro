using App21.Classes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Windows.Data.Xml.Dom;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Notifications;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上有介绍

namespace App21
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class Results : Page
    {
        private Spider spider;
        private CancellationTokenSource tokenSource;
        private Queue<string> queue;
        private List<string> all;
        private List<UriInfo> bingo;
        private Task[] tasks;
        private const int taskNum = 10;

        private Object myLock = new object();
        private Object myLock2 = new object();

        private List<string> keyWords;
        private string baseUri;
        private int timeSpan;
        private string encoding = "GBK";
        private DispatcherTimer dt;
        private Stopwatch sw;
        public Results()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Enabled;
            //为保存搜索结果必须打开缓存

            queue = new Queue<string>();
            all = new List<string>();
            bingo = new List<UriInfo>();


            dt = new DispatcherTimer();
            dt.Interval = TimeSpan.FromMilliseconds(1000);

            sw = new Stopwatch();

        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            //if(e.SourcePageType.Equals(typeof(SettingsPage)))
            //    new MessageDialog("what").ShowAsync();
            //为什么上面的不行？？
            if(e.NavigationMode == NavigationMode.New)
            {
                spider = (Spider)e.Parameter;
                baseUri = spider.BaseUri;
                keyWords = spider.KeyWords;
                timeSpan = spider.TimeSpan;

                queue.Enqueue(baseUri);
            }
        }

        private void Start()
        {
            tokenSource = new CancellationTokenSource();
            tasks = new Task[taskNum];
            for (int i = 0; i < taskNum; i++)
            {
                tasks[i] = Task.Factory.StartNew(Find, tokenSource.Token);
            }
            dt.Tick += dt_Tick;
            dt.Start();
            sw.Start();
        }

        void dt_Tick(object sender, object e)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("总数:" + all.Count);
            sb.Append(" 找到:" + bingo.Count);
            sb.Append(" 剩余:" + queue.Count);
            sb.Append(" 经过:" + sw.ElapsedMilliseconds / 1000);
            tbk_status.Text = sb.ToString();

        }

        private void Stop()
        {
            tokenSource.Cancel();
            sw.Stop();
            dt.Stop();
            dt.Tick -= dt_Tick;
            tbk_status.Text = "正在关闭线程";
            for(int i=0;i<taskNum;i++)
            {
                if (tasks[i].Status == TaskStatus.Running)
                    tasks[i].Wait();
            }
            tbk_status.Text = "已停止";

            lv_results.ItemsSource = null;
            lv_results.ItemsSource = bingo;
            //为什么这样就行，难道ItemsSource再次设置为同一个Collection后lv_results不会更新吗？
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //if (spider.tasks == null)
            switch (((Button)sender).Name)
            {
                case "btn_start":
                    Start();
                    break;
                case "btn_stop":
                    Stop();
                    break;
            }
        }

        private void lv_results_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            if(lv_results.SelectedIndex != -1)
            {
                UriInfo uriInfo = (UriInfo)lv_results.SelectedItem;
                tbk_uri.Text = uriInfo.UriStr;
                wv_webPage.Navigate(new Uri(uriInfo.UriStr));
            }
        }

        private async void Find() //多线程同时使用该函数，则该函数不要包含全局变量
        {
            while (!tokenSource.IsCancellationRequested)
            {
                string targetUri = null;
                lock (myLock)
                {
                    if (queue.Count == 0)
                        continue;
                    targetUri = queue.Dequeue();
                    if (!targetUri.Equals(baseUri))
                        targetUri = baseUri + targetUri;
                }

                try
                {
                    //1.获取html文档
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(targetUri);
                    HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync();      
                    StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.GetEncoding(encoding));
                    string htmlStr = reader.ReadToEnd();

                    //2.链接搜索
                    Regex regUri = new Regex("(/\\w+)+\\.htm(l)?");
                    MatchCollection results = regUri.Matches(htmlStr);
                    for (int i = 0; i < results.Count; i++)
                    {
                        string temp = results[i].ToString();

                        if (!all.Contains(temp))
                        {
                            all.Add(temp);
                            queue.Enqueue(temp);
                        }
                    }


                    //针对软件学院官网，所有有内容的网页要都包含"webinfo"
                    if (baseUri.Equals("http://sw.scu.edu.cn") && !targetUri.Contains("webinfo"))
                        continue;
                    //3.关键词搜索
                    int j;
                    for (j = 0; j < keyWords.Count; j++)
                        if (!htmlStr.Contains(keyWords[j]))
                            break;
                    if (j == keyWords.Count)
                    {
                        Regex regTitle = new Regex("<title.*?>(.*?)</title>",RegexOptions.IgnoreCase);
                        Match matchTitle = regTitle.Match(htmlStr);
                        bingo.Add(new UriInfo(targetUri, response.Headers["Last-Modified"],matchTitle.Groups[1].Value));
                        ShowToast("找到网址", targetUri);
                    }
                }
                catch (WebException)
                {
                    //好像没什么用
                }
            }
        }

        private void ShowToast(string title,string content)
        {
            string toastStr = "<toast duration='long' launch='Page1'>"//通过给Toast节点添加一个duration属性来设置Toast通知的显示时间长(long)短(short)
                               + "<visual version='1'>"
                               + "<binding template='ToastText02'>"
                               + "<text id='1'>" + title + "</text>"
                               + "<text id='2'>" + content + "</text>"
                               + "</binding>"
                               + "</visual>"
                               + "<audio src='ms-winsoundevent:Notification.Mail' loop='false'/>"//可以在toast的节点下面设置声音节点,当声音设置循环的时候，duration必须设置为long，不播放声音的时候需要设置audio元素的属性silent=true
                               + "</toast>";
            XmlDocument toastXml = new XmlDocument();
            toastXml.LoadXml(toastStr);
            ToastNotification toastNotification = new ToastNotification(toastXml);
            //toastNotification.Activated += toastNotification_Activated;
            //在调试前必须先在程序清单中Package.appxmanifest文件中将支持Toast通知设置为'是'
            ToastNotificationManager.CreateToastNotifier().Show(toastNotification);
        }

        //void toastNotification_Activated(ToastNotification sender, object args)
        //{
            
        //}

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(ItemsPage));
        }
    }
}
