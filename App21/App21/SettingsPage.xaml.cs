using App21.Classes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
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
    public sealed partial class SettingsPage : Page
    {
        private IList<string> list;
        string filename = "URL.txt";
        StorageFile file;
        StorageFolder root = ApplicationData.Current.LocalFolder;
        private static List<string> list_freq = new List<string>() { "10分钟","半小时","一小时" };
        public SettingsPage()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// 在此页将要在 Frame 中显示时进行调用。
        /// </summary>
        /// <param name="e">描述如何访问此页的事件数据。Parameter
        /// 属性通常用于配置页。</param>
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.NavigationMode == NavigationMode.New)
            {
                file = await root.GetFileAsync(filename);
                list = await FileIO.ReadLinesAsync(file);
                cbList.DataContext = list;
                cb_freq.ItemsSource = list_freq;
            }
        }

        private void GoBack(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(ItemsPage));
        }

        private async void Button_Click_1(object sender, RoutedEventArgs e)
        {
            MessageDialog ms_ponints = new MessageDialog(textbox_add.Text);
            await ms_ponints.ShowAsync();
        }

        private async void Button_Click_2(object sender, RoutedEventArgs e)
        {
            try
            {
                string temp = textbox_add.Text.Trim();
                if(temp == "")
                {
                    await new MessageDialog("RSS地址为空。重新输入！").ShowAsync();
                    return;
                }
                Regex regRss = new Regex("^http://\\w+(\\.\\w+){2,3}");
                if (regRss.IsMatch(temp))
                {
                    if(!list.Contains<string>(temp))
                    {
                        list.Add(temp);
                        await FileIO.WriteLinesAsync(file, list);
                        MessageDialog ms_ponints_2 = new MessageDialog("成功添加" + temp + "等待下次启动程序可见");
                        await ms_ponints_2.ShowAsync();
                    }
                    else
                    {
                        await new MessageDialog("RSS地址已存在").ShowAsync();
                    }
                }
                else
                {
                    await new MessageDialog("RSS地址不合法").ShowAsync();
                    return;
                }
            }
            catch (Exception e1)
            {

                MessageDialog ms_ponints = new MessageDialog(e1.ToString());
                ms_ponints.ShowAsync();
            }
        }

        private async void Button_Click_3(object sender, RoutedEventArgs e)
        {
            try
            {
                if (cbList.SelectedIndex != -1)
                {
                    list.RemoveAt(cbList.SelectedIndex);
                    await FileIO.WriteLinesAsync(file, list);
                    //使用await不行？？？必须异步？？？
                    if (list.Count > 0)
                    {
                        cbList.SelectedIndex = 0;
                    }

                    MessageDialog ms_ponints_1 = new MessageDialog("成功删除"+cbList.SelectedValue+"等待下次启动程序可见");
                    await ms_ponints_1.ShowAsync();
                }
            }
            catch (Exception e2)
            {
                MessageDialog ms_ponints = new MessageDialog(e2.ToString());
                ms_ponints.ShowAsync();
            }
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            if(cb_freq.SelectedIndex == -1)
            {
                await new MessageDialog("请选择检索频率").ShowAsync();
                return;
            }

            if(tbx_keyWord.Text.Trim() == "")
            {
                await new MessageDialog("关键词不能为空").ShowAsync();
                return;
            }

            if(tbx_uri.Text.Trim() == "")
            {
                await new MessageDialog("网址不能为空").ShowAsync();
                return;
            }
            else
            {
                Regex reg = new Regex("^http://\\w+(\\.\\w+){2,3}$");
                if(!reg.Match(tbx_uri.Text.Trim()).Success)
                {
                    await new MessageDialog("网址不合法（http://xxx.xxx.xxx(.xxx)）").ShowAsync();
                    return;
                }
            }

            string[] strs = Regex.Split(tbx_keyWord.Text.Trim()," +");
            int timeSpan = 0;
            switch (cb_freq.SelectedIndex)
            {
                case 0:
                    timeSpan = 600000;
                    break;
                case 1:
                    timeSpan = 1800000;
                    break;
                case 2:
                    timeSpan = 3600000;
                    break;
            }
            this.Frame.Navigate(typeof(Results),new Spider(tbx_uri.Text.Trim(),new List<string>(strs),timeSpan));
        }
    }
}
