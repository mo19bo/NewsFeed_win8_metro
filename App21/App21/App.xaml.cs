using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using App21;

// “空白应用程序”模板在 http://go.microsoft.com/fwlink/?LinkId=234227 上有介绍

namespace App21
{
    /// <summary>
    /// 提供特定于应用程序的行为，以补充默认的应用程序类。
    /// </summary>
    sealed partial class App : Application
    {
        /// <summary>
        /// 初始化单一实例应用程序对象。这是执行的创作代码的第一行，
        /// 逻辑上等同于 main() 或 WinMain()。
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;
        }

        /// <summary>
        /// 在应用程序由最终用户正常启动时进行调用。
        /// 当启动应用程序以执行打开特定的文件或显示搜索结果等操作时
        /// 将使用其他入口点。
        /// </summary>
        /// <param name="args">有关启动请求和过程的详细信息。</param>
        protected async override void OnLaunched(LaunchActivatedEventArgs args)
        {
            
            Frame rootFrame = Window.Current.Content as Frame;
           
            // 不要在窗口已包含内容时重复应用程序初始化，
            // 只需确保窗口处于活动状态
            if (rootFrame == null)
            {
                // 创建要充当导航上下文的框架，并导航到第一页
                rootFrame = new Frame();
               App21.Common.SuspensionManager.RegisterFrame(rootFrame, "AppFrame");
                var connectionProfile = Windows.Networking.Connectivity.NetworkInformation.GetInternetConnectionProfile();
                if (connectionProfile != null)
                {
                    FeedDataSource feedDataSource = (FeedDataSource)App.Current.Resources["feedDataSource"];
                    if (feedDataSource != null)
                    {
                        if (feedDataSource.Feeds.Count == 0)
                        {
                            await feedDataSource.GetFeedsAsync();
                        }
                    }
                }
                else
                {
                    var messageDialog = new Windows.UI.Popups.MessageDialog("An internet connection is needed to download feeds. Please check your connection and restart the app.");
                    var result = messageDialog.ShowAsync();
                }
              
                if (args.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: 从之前挂起的应用程序加载状态
                    await App21.Common.SuspensionManager.RestoreAsync();
                }
                
                
                Window.Current.Content = rootFrame;
            }

            if (rootFrame.Content == null)
            {
                // 当未还原导航堆栈时，导航到第一页，
                // 并通过将所需信息作为导航参数传入来配置
                // 参数
                if (!rootFrame.Navigate(typeof(ItemsPage), args.Arguments))
                {
                    throw new Exception("Failed to create initial page");
                }
            }
            // 确保当前窗口处于活动状态
            Window.Current.Activate();
        }
     
        /// <summary>
        /// 在将要挂起应用程序执行时调用。在不知道应用程序
        /// 将被终止还是恢复的情况下保存应用程序状态，
        /// 并让内存内容保持不变。
        /// </summary>
        /// <param name="sender">挂起的请求的源。</param>
        /// <param name="e">有关挂起的请求的详细信息。</param>
        private async void OnSuspending(object sender, SuspendingEventArgs e)
        {
         
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: 保存应用程序状态并停止任何后台活动
            await App21.Common.SuspensionManager.SaveAsync();
            deferral.Complete();
        }
    }
}
