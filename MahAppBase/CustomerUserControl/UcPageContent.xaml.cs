using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms.VisualStyles;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using CefSharp;
using CefSharp.Wpf;
using Xceed.Wpf.AvalonDock.Layout;

namespace MahAppBase.CustomerUserControl
{
    /// <summary>
    /// UcPageContent.xaml 的互動邏輯
    /// </summary>
    public partial class UcPageContent : LayoutDocument
    {
        #region Declarations
        private ViewModel.LayoutDocument MainViewModel;
        public event UserControlEventHandler KeyDownEventHandler;
        public delegate void UserControlEventHandler(object sender, KeyEventArgs e);
        public event WebBrowserLoadEventHandler LoadCompleteEventHandler;
        public delegate void WebBrowserLoadEventHandler(object sender, NavigationEventArgs e);
        #endregion

        #region Memberfunction
        public UcPageContent()
        {
            InitializeComponent();
            MainViewModel = new ViewModel.LayoutDocument();
            gdMain.DataContext = MainViewModel;
            
        }
        
        private void UIElement_OnKeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Enter)
                {
                    if (tbUrl.Text.Contains("http"))
                    {
                        MainViewModel.Url = tbUrl.Text;
                        cwUrl.Load(MainViewModel.Url);
                    }
                    else
                        cwUrl.Load("https://www.google.com/search?q=" + tbUrl.Text + "&sourceid=chrome=UTF-8");
                }
            }
            catch (Exception ie)
            {
                tbUrl.Tag = $"{DateTime.Now.ToShortTimeString()} \r\n執行網址發生例外,網址:{tbUrl.Text}, 例外:{ie.Message}\r\n{ie.StackTrace}";
                this.KeyDownEventHandler?.Invoke(sender, e);
            }
        }

        private void TbUrl_OnGotMouseCapture(object sender, MouseEventArgs e)
        {
            tbUrl.SelectAll();
        }

        private void UcPageContent_OnClosed(object sender, EventArgs e)
        {
            gdMain.DataContext = null;
            MainViewModel = null;
            cwUrl.Dispose();
            BindingOperations.ClearBinding(tbUrl, TextBlock.TextProperty);
            tbUrl.UndoLimit = 0;
        }
        private void CwUrl_OnTitleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((sender as ChromiumWebBrowser) is null)
                return;
            MainViewModel.Url = (sender as ChromiumWebBrowser).Address;
            layDoc.ToolTip = layDoc.Title = (sender as ChromiumWebBrowser).Title;

        }
        #endregion

        #region Property
        public object Tag { get; set; }

        public ViewModel.LayoutDocument PageContentViewModel
        {
            get
            {
                return MainViewModel;
            }
        }
        #endregion
    }
}
