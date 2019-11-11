using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MahAppBase.CustomerUserControl;
using MahAppBase.ViewModel;
using MahApps.Metro.Controls;
using Xceed.Wpf.AvalonDock.Layout;

namespace MahAppBase
{
    /// <summary>
    /// MainWindow.xaml 的互動邏輯
    /// </summary>
    public partial class MainWindow : MetroWindow
    {

        #region Declarations
        private MainComponent MainViewModel = null;
        #endregion

        #region MemberFunction
        public MainWindow()
        {
            InitializeComponent();
            MainViewModel = new MainComponent();
            mwMain.DataContext = MainViewModel;
        }

        /// <summary>
        /// 我懶惰
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonAdd_OnClick(object sender, RoutedEventArgs e)
        {
            //初始化網頁UserControl instance、委派事件回主視窗
            var dt = new CustomerUserControl.UcPageContent();
            dt.Tag = DateTime.Now.ToShortTimeString();
            dt.KeyDownEventHandler += Dt_KeyDownEventHandler;
            dt.LoadCompleteEventHandler += Dt_LoadCompleteEventHandler;
            //產生AvalonDock頁面實例、委派事件回主視窗
            MainGroup.Children.Add(dt);
            dt = null;
        }

        /// <summary>
        /// 接受動態產生元件委派事件，WebBorwser載入完成時取得網頁標題
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Dt_LoadCompleteEventHandler(object sender, NavigationEventArgs e)
        {

            var dt = (sender as WebBrowser);
            if (dt is null)
            {
                MainViewModel.ErrorMessage = $"{DateTime.Now.ToShortTimeString()} : 取得標題失敗";
                return;
            }
            else
            {
                dynamic doc = dt.Document;
                var title = doc.Title;
                //MainViewModel.CurrentTab = title;
            }
        }

        /// <summary>
        /// 接受動態產生元件委派事件，接收元件例外訊息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Dt_KeyDownEventHandler(object sender, KeyEventArgs e)
        {
            if((sender as TextBox) is null)
                return;
            var actionUrl = (sender as TextBox).Tag;
            MainViewModel.ErrorMessage = (string)actionUrl;
        }
        #endregion
    }
}