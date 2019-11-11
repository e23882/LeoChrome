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
using GalaSoft.MvvmLight.Command;
using MahAppBase.Command;
using MahAppBase.CustomerUserControl;
using MahApps.Metro.Controls;

namespace MahAppBase.ViewModel
{
    public class MainComponent:ViewModelBase
    {
        #region Declarations
        private WindowState _State = WindowState.Normal;
        private ViewModel.Status _CurrentStatus = new Status();
        private Visibility _Render = Visibility.Visible;
        private int _FlyOutWidth = 400;
        private Thickness _WebMargin = new Thickness(400,0, 0, 0);
        private string _ErrorMessage = string.Empty;
        private string _CurrentTab = string.Empty;
        private bool _FlyOutSettingIsOpen = true;
        private bool _FlyOutDonateIsOpen = false;
        private bool _BetterPerformance = true;
        private bool _SaveSetting = false;
        #endregion

        #region Property
        public NoParameterCommand ButtonDonateClick { get; set; }
        public NoParameterCommand MainWindowClosed { get; set; }
        public NoParameterCommand ButtonSettingClick { get; set; }
        public NoParameterCommand FlyOutSettingClose { get; set; }
        public NoParameterCommand FlyOutDonateClose { get; set; }
        public NoParameterCommand ButtonChoosePathClick { get; set; }
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
        public string CurrentTab
        {
            get { return _CurrentTab; }
            set { _CurrentTab = value; OnPropertyChanged("CurrentTab"); }
        }
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
        public Visibility Render
        {
            get { return _Render; }
            set
            {
                _Render = value;
                OnPropertyChanged("Render");
            }
        }
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
        public ViewModel.Status CurrentStatus
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
        private string _SettingPath = string.Empty;
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

        #endregion

        #region DataModel

        public class DockData
        {
            public string DockText
            {
                get;
                set;
            }
        }
        

        #endregion

        #region MemberFunction
        public MainComponent()
        {
            ButtonDonateClick = new NoParameterCommand(ButtonDonateClickAction);
            MainWindowClosed = new NoParameterCommand(MainWindowClosedAction);
            ButtonSettingClick = new NoParameterCommand(ButtonSettingClickAction);
            FlyOutSettingClose = new NoParameterCommand(FlyOutSettingCloseAction);
            FlyOutDonateClose = new NoParameterCommand(FlyOutDonateCloseAction);
            ButtonChoosePathClick = new NoParameterCommand(ButtonChoosePathClickAction);
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