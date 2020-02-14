using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using System.Windows.Navigation;
using MahAppBase.Command;
using MahAppBase.CustomerUserControl;
using MahApps.Metro.Controls;
using Xceed.Wpf.AvalonDock.Layout;
using TextBox = System.Windows.Forms.TextBox;

namespace MahAppBase.ViewModel
{
    public class MainComponent:ViewModelBase
    {
        #region Declarations
        private WindowState _State = WindowState.Normal;
        private Status _CurrentStatus = new Status();
        private Visibility _Render = Visibility.Visible;
        private int _FlyOutWidth = 400;
        private int _MainWindowWidth = 1100;
        private Thickness _WebMargin = new Thickness(400,0, 0, 0);
        private string _ErrorMessage = string.Empty;
        private string _CurrentTab = string.Empty;
        private string _SettingPath = string.Empty;
        private bool _FlyOutSettingIsOpen = true;
        private bool _FlyOutDonateIsOpen = false;
        private bool _BetterPerformance = true;
        private bool _SaveSetting = false;
        private LayoutDocumentPane _MainGroupInstance;
        private List<LayoutDocument> SubViewModelList = new List<LayoutDocument>();
        #endregion

        #region Property
        /// <summary>
        /// 贊助按鈕點擊Command
        /// </summary>
        public NoParameterCommand ButtonDonateClick { get; set; }

        /// <summary>
        /// 主視窗關閉命令
        /// </summary>
        public NoParameterCommand MainWindowClosed { get; set; }

        /// <summary>
        /// 設定按鈕點擊命令
        /// </summary>
        public NoParameterCommand ButtonSettingClick { get; set; }

        /// <summary>
        /// 設定Flyout元件關閉命令
        /// </summary>
        public NoParameterCommand FlyOutSettingClose { get; set; }

        /// <summary>
        /// 贊助Flyout元件關閉命令
        /// </summary>
        public NoParameterCommand FlyOutDonateClose { get; set; }

        /// <summary>
        /// 設定下載影片位置按紐點擊命令
        /// </summary>
        public NoParameterCommand ButtonChoosePathClick { get; set; }

        /// <summary>
        /// 開新頁面按鈕點擊命令
        /// </summary>
        public CommonCommand ButtonNewTabClick { get; set; }

        /// <summary>
        /// 設定FlyOut元件是否開啟屬性
        /// </summary>
        public bool FlyOutSettingIsOpen
        {
            get
            {
                return _FlyOutSettingIsOpen;
            }
            set
            {
                if (value)
                {
                    _FlyOutSettingIsOpen = value;
                    FlyOutDonateIsOpen = false;
                }
                else
                    _FlyOutSettingIsOpen = value;
                OnPropertyChanged("FlyOutSettingIsOpen");
            }
        }

        /// <summary>
        /// 贊助FlyOut元件是否開啟屬性
        /// </summary>
        public bool FlyOutDonateIsOpen
        {
            get
            {
                return _FlyOutDonateIsOpen;
            }
            set
            {
                if (value)
                {
                    _FlyOutDonateIsOpen = value;
                    FlyOutSettingIsOpen = false;
                }
                else
                    _FlyOutDonateIsOpen = value;
                OnPropertyChanged("FlyOutDonateIsOpen");
            }
        }

        /// <summary>
        /// 錯誤訊息屬性
        /// </summary>
        public string ErrorMessage
        {
            get
            {
                return _ErrorMessage;
            }
            set
            {
                _ErrorMessage += value;
                OnPropertyChanged("ErrorMessage");
            }
        }

        /// <summary>
        /// 目前Tab名稱屬性
        /// </summary>
        public string CurrentTab
        {
            get { return _CurrentTab; }
            set { _CurrentTab = value; OnPropertyChanged("CurrentTab"); }
        }

        /// <summary>
        /// Flyout元件寬度屬性
        /// </summary>
        public int FlyOutWidth
        {
            get
            {
                return _FlyOutWidth;
            }
            set
            {
                _FlyOutWidth = value;
                OnPropertyChanged("FlyOutWidth");
                OnPropertyChanged("WebMargin");
            }
        }

        /// <summary>
        /// 是否Render TabItem內容屬性
        /// </summary>
        public Visibility Render
        {
            get { return _Render; }
            set
            {
                _Render = value;
                OnPropertyChanged("Render");
            }
        }

        /// <summary>
        /// 主視窗狀態屬性
        /// </summary>
        public WindowState State
        {
            get { return _State; }
            set
            {
                _State = value;
                if (BetterPerformance)
                {
                    switch (value)
                    {
                        case WindowState.Minimized:
                            Render = Visibility.Collapsed;
                            break;
                        case WindowState.Normal:
                            Render = Visibility.Visible;
                            break;
                        case WindowState.Maximized:
                            Render = Visibility.Visible;
                            break;
                        default:
                            Render = Visibility.Visible;
                            break;
                    }
                }
                
                OnPropertyChanged("State");
            }
        }

        /// <summary>
        /// 程式目前狀態屬性
        /// </summary>
        public Status CurrentStatus
        {
            get
            {
                return _CurrentStatus;
            }
            set
            {
                _CurrentStatus = value;
                OnPropertyChanged("CurrentStatus");
            }
        }

        /// <summary>
        /// TabItem中內容Margin屬性
        /// </summary>
        public Thickness WebMargin
        {
            get
            {
                return _WebMargin;
            }
            set
            {
                _WebMargin = value;
                OnPropertyChanged("WebMargin");
            }
        }

        /// <summary>
        /// 是否在視窗縮小時取消Render Tabitem內容屬性
        /// </summary>
        public bool BetterPerformance
        {
            get
            {
                return _BetterPerformance;
            }
            set
            {
                _BetterPerformance = value;
                OnPropertyChanged("BetterPerformance");
            }
        }

        /// <summary>
        /// 是否儲存目前設定屬性(not working)
        /// </summary>
        public bool SaveSetting
        {
            get
            {
                return _SaveSetting;
            }
            set
            {
                _SaveSetting = value;
                OnPropertyChanged("SaveSetting");
            }
        }
        
        /// <summary>
        /// 儲存目前設定路徑屬性(not working)
        /// </summary>
        public string SettingPath
        {
            get
            {
                return _SettingPath;
            }
            set
            {
                _SettingPath = value;
                OnPropertyChanged("SettingPath");
            }

        }

        /// <summary>
        /// 主視窗寬度屬性
        /// </summary>
        public int MainWindowWidth
        {
            get
            {
                return _MainWindowWidth;
            }
            set
            {
                if(_MainWindowWidth != value)
                {
                    _MainWindowWidth = value;
                    if (_MainWindowWidth < 900)
                    {
                        Parallel.ForEach(SubViewModelList, item => 
                        {
                            item.ShowDownloadTool = Visibility.Collapsed;
                        });
                    }
                    else
                    {
                        Parallel.ForEach(SubViewModelList, item =>
                        {
                            item.ShowDownloadTool = Visibility.Visible;
                        });
                    }
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public LayoutDocumentPane MainGroupInstance
        {
            get
            {
                return _MainGroupInstance;
            }
            set
            {
                _MainGroupInstance = value;
                OnPropertyChanged();
            }
        }
        
       
        #endregion

        #region MemberFunction
        /// <summary>
        /// 建構子
        /// </summary>
        public MainComponent()
        {
            ButtonDonateClick = new NoParameterCommand(ButtonDonateClickAction);
            MainWindowClosed = new NoParameterCommand(MainWindowClosedAction);
            ButtonSettingClick = new NoParameterCommand(ButtonSettingClickAction);
            FlyOutSettingClose = new NoParameterCommand(FlyOutSettingCloseAction);
            FlyOutDonateClose = new NoParameterCommand(FlyOutDonateCloseAction);
            ButtonChoosePathClick = new NoParameterCommand(ButtonChoosePathClickAction);
            ButtonNewTabClick = new CommonCommand(ButtonNewTabClickAction);
        }
        
        private void ButtonNewTabClickAction(object obj)
        {
            //初始化網頁UserControl instance、委派事件回主視窗
            var dt = new UcPageContent();
            dt.Tag = DateTime.Now.ToShortTimeString();
            LayoutDocument SubWindowViewModel = new LayoutDocument();
            SubViewModelList.Add(SubWindowViewModel);
            dt.gdMain.DataContext = SubWindowViewModel;
            dt.KeyDownEventHandler += Dt_KeyDownEventHandler;
            dt.LoadCompleteEventHandler += Dt_LoadCompleteEventHandler;
            //產生AvalonDock頁面實例、委派事件回主視窗
            MainGroupInstance.Children.Add(dt);
            dt = null;
        }

        /// <summary>
        /// 接受動態產生元件委派事件，WebBorwser載入完成時取得網頁標題
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Dt_LoadCompleteEventHandler(object sender, NavigationEventArgs e)
        {
            var dt = (sender as System.Windows.Controls.WebBrowser);
            if (dt is null)
            {
                this.ErrorMessage = $"{DateTime.Now.ToShortTimeString()} : 取得標題失敗";
                return;
            }
            else
            {
                dynamic doc = dt.Document;
                var title = doc.Title;
            }
        }

        /// <summary>
        /// 接受動態產生元件委派事件，接收元件例外訊息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Dt_KeyDownEventHandler(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if ((sender as System.Windows.Controls.TextBox) is null)
                return;
            var actionUrl = (sender as TextBox).Tag;
            this.ErrorMessage = (string)actionUrl;
        }

        public void ButtonChoosePathClickAction()
        {
            using (var fbd = new FolderBrowserDialog())
            {
                DialogResult result = fbd.ShowDialog();
                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                    SettingPath = fbd.SelectedPath;
            }
        }

        public void FlyOutDonateCloseAction()
        {
            WebMargin = new Thickness(0, 0, 0, 0);
            FlyOutDonateIsOpen = false;
        }

        public void FlyOutSettingCloseAction()
        {
            WebMargin = new Thickness(0, 0, 0, 0);
            FlyOutSettingIsOpen = false;
        }

        public void ButtonDonateClickAction()
        {
            FlyOutDonateIsOpen = true;
            
            WebMargin = new Thickness(400, 0, 0, 0);
        }

        public void MainWindowClosedAction()
        {
            System.Windows.Application.Current.Shutdown();
        }

        public void ButtonSettingClickAction()
        {
            FlyOutSettingIsOpen = true;
            WebMargin = new Thickness(400, 0, 0, 0);
        }
        #endregion
    }
}