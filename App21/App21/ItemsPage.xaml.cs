using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Windows.Data.Xml.Dom;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Notifications;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// “项目页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234233 上提供

namespace App21
{
    /// <summary>
    /// 显示项预览集合的页。在“拆分布局应用程序”中，此页
    /// 用于显示及选择可用组之一。
    /// </summary>
    public sealed partial class ItemsPage : App21.Common.LayoutAwarePage
    {
        public ItemsPage()
        {
            this.InitializeComponent();
            TileUpdateManager.CreateTileUpdaterForApplication().EnableNotificationQueue(true);
            //设置磁贴队列为可用
        }

        /// <summary>
        /// 使用在导航过程中传递的内容填充页。在从以前的会话
        /// 重新创建页时，也会提供任何已保存状态。
        /// </summary>
        /// <param name="navigationParameter">最初请求此页时传递给
        /// <see cref="Frame.Navigate(Type, Object)"/> 的参数值。
        /// </param>
        /// <param name="pageState">此页在以前会话期间保留的状态
        /// 字典。首次访问页面时为 null。</param>
        protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
            FeedDataSource feedDataSource = (FeedDataSource)App.Current.Resources["feedDataSource"];

            if (feedDataSource != null)
            {
                this.DefaultViewModel["Items"] = feedDataSource.Feeds;

                ShowTile();
            }

            // TODO: 将可绑定项集合分配到 this.DefaultViewModel["Items"]
        }

        private void itemGridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (e.ClickedItem != null)
            {
                string title = ((FeedData)e.ClickedItem).Title;
                this.Frame.Navigate(typeof(SplitPage), title);
            }

        }

        private void itemListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (e.ClickedItem != null)
            {
                string title = ((FeedData)e.ClickedItem).Title;
                this.Frame.Navigate(typeof(SplitPage), title);
            }

        }

        private void buttun_set_Click(object sender, RoutedEventArgs e)
        {
            MessageDialog md = new MessageDialog("添加一个Rss地址");
             md.ShowAsync();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(SettingsPage));
        }

        //按下帮助按钮
        private async void buttun_set_Click_1(object sender, RoutedEventArgs e)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("核心功能：多线程爬虫，支持多关键词匹配，可后台运行");
            sb.AppendLine("          RSS阅读器，支持动态添加删除RSS源");
            sb.AppendLine("应用栏功能：");
            sb.AppendLine("1.设置：用于添加删除RSS阅读器的RSS源URI地址，设置爬虫关键词，爬虫网址等");
            sb.AppendLine("2.帮助：弹出帮助对话框，显示该应用的主要功能");
            sb.AppendLine("3.关于：显示开发人员的信息");
            await new MessageDialog(sb.ToString(), "帮助").ShowAsync();
        }

        //关于按钮
        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("版本号：1.1.0.0");
            sb.AppendLine("团队成员：单斌 何晨 李洪锐 武越 柴歆韵");
            sb.AppendLine("本项目目的是，基于win8系统的全新平台，利用网络爬虫技术，相关编程语言，以及对http协议和半结构化的数据机构化");
            sb.AppendLine("的研究的基础上，同时运用win8这个全新的平台，以及metro的分屏风格及磁贴的自动更新消息的优势对信息的检索与处理");
            sb.AppendLine("进行了更加人性化与智能化的改造。");
　　　      sb.AppendLine("本项目优势在于，针对当下搜索引擎搜索大量信息但命中度低的缺点，该软件可以自动根据用户的设定，及时通过桌面弹窗");
            sb.AppendLine("的方式对用户进行提醒，而用户只需轻松设置关键词，和自己需要留意的网站，该软件就能自动进行推送，同时提供语音识别，这不但方便了");
            sb.AppendLine("人们对信息的获取，节约了人们的时间，使信息的获取更能适应快节奏的生活，而且让也让信息服务人性化。而语音识别功能不仅是win8特性");
            sb.AppendLine("，也为因种种原因不进行键盘操作的用户提供了便利。");
            await new MessageDialog(sb.ToString(),"关于").ShowAsync();
        }

        private void ShowTile()
        {
            FeedDataSource feedDataSource = (FeedDataSource)App.Current.Resources["feedDataSource"];
            foreach (var blogpost in feedDataSource.Feeds)
            {
                foreach (var item in blogpost.Items)
                {

                    //string tileXmlString = "<tile>"
                    //    // 下面是一段XML代码
                    //                        + "<visual>"
                    //                        + "<binding template='TileWide310x150PeekImage01'>"
                    //                        + "<image id='1' src='ms-appx:///Assets/LogoMedium.png'/>"
                    //                        + "<text id='1'>" + blogpost.Title + "</text>"
                    //                        + "<text id='2'>" + item.Title + "</text>"
                    //                        + "</binding>"
                    //                        + "<binding template='TileSquare150x150Text01'>"
                    //                        + "<text id='1'>" + blogpost.Title + "</text>"
                    //                        + "<text id='2'>" + item.Title + "</text>"
                    //                        + "</binding>"
                    //                        + "</visual>"
                    //                        + "</tile>";
                    //Windows.Data.Xml.Dom.XmlDocument tileDOM = new Windows.Data.Xml.Dom.XmlDocument();
                    //tileDOM.LoadXml(tileXmlString);
                    //TileNotification tile = new TileNotification(tileDOM);
                    //TileUpdateManager.CreateTileUpdaterForApplication().Update(tile);
                    XmlDocument largeTileData = TileUpdateManager.GetTemplateContent(TileTemplateType.TileWide310x150Text01);

                    XmlNodeList largeTextData = largeTileData.GetElementsByTagName("text");
                    largeTextData[0].InnerText = blogpost.Title;
                    largeTextData[1].InnerText = item.Title;

                    //XmlNodeList imageData = largeTileData.GetElementsByTagName("image");
                    //((XmlElement)imageData[0]).SetAttribute("src", "ms-appx:///Assets/LogoMedium.jpg");

                    //Create the notification the same way.
                    TileNotification notification = new TileNotification(largeTileData);
                    notification.ExpirationTime = DateTimeOffset.UtcNow.AddSeconds(30);
                    //Push the update to the tile.
                    TileUpdateManager.CreateTileUpdaterForApplication().Update(notification);
                }
            }
        }
    }
}
