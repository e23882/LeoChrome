﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using MahAppBase.Command;
using MahAppBase.CustomerUserControl;
using VideoLibrary;
using MessageBox = System.Windows.MessageBox;

namespace MahAppBase.ViewModel
{
    public class LayoutDocument:ViewModelBase
    {
        #region Declarations
        private string _Url = string.Empty;
        private Visibility _ButtonDownLoadVisibility = Visibility.Collapsed;
        private Visibility _ButtonDownLoadListVisibility = Visibility.Collapsed;
        private UcDownLoadSetting uc = null;
        private Thread th;
        private bool _IsDownloading = false;
        private int _ProgressMax = 100;
        private string _ProgressPercent = "100 %";
        private string _DownloadPath = string.Empty;
        private long _CurrentProgress = 0;
        #endregion

        #region Property
        public string Url
        {
            get { return _Url; }
            set
            {
                _Url = value;
                if (_Url.Contains("youtube") && _Url.Contains("watch?v="))
                    ButtonDownLoadVisibility = Visibility.Visible;
                else
                    ButtonDownLoadVisibility = Visibility.Collapsed;
                if (_Url.Contains("youtube") && _Url.Contains("watch?v=")&&_Url.Contains("&list="))
                    ButtonDownLoadListVisibility = Visibility.Visible;
                else
                    ButtonDownLoadListVisibility = Visibility.Collapsed;
                OnPropertyChanged("Url");
            }
        }
        public Visibility ButtonDownLoadVisibility
        {
            get
            {
                return _ButtonDownLoadVisibility;
            }
            set
            {
                _ButtonDownLoadVisibility = value;
                OnPropertyChanged("ButtonDownLoadVisibility");
            }
        }
        public Visibility ButtonDownLoadListVisibility
        {
            get
            {
                return _ButtonDownLoadListVisibility;
            }
            set
            {
                _ButtonDownLoadListVisibility = value;
                OnPropertyChanged("ButtonDownLoadListVisibility");
            }
        }

        private Visibility _ShowDownloadTool;
        public Visibility ShowDownloadTool
        {
            get
            {
                return _ShowDownloadTool;
            }
            set
            {
                _ShowDownloadTool = value;
                OnPropertyChanged();
            }
        }  
        public NoParameterCommand ButtonDownLoadClick { get; set; }
        public NoParameterCommand ButtonDownLoadListClick { get; set; }
        public NoParameterCommand ButtonSettingOnClick { get; set; }
        public string DownloadPath
        {
            get
            {
                return _DownloadPath;
            }
            set
            {
                _DownloadPath = value;
                OnPropertyChanged("DownloadPath");
            }
        }
        public long CurrentProgress
        {
            get
            {
                return _CurrentProgress;
            }
            set
            {
                _CurrentProgress = value;
                OnPropertyChanged("CurrentProgress");
                OnPropertyChanged("ProgressPercent");
            }
        }
        public string ProgressPercent
        {
            get
            {
                return (CurrentProgress*100)/ProgressMax+" %";
            }
            set
            {
                _ProgressPercent = value;
                OnPropertyChanged("ProgressPercent");
            }
        }
        public int ProgressMax
        {
            get
            {
                return _ProgressMax;
            }
            set
            {
                _ProgressMax = value;
                OnPropertyChanged("ProgressMax");
                OnPropertyChanged("ProgressPercent");
            }
        }
        public bool IsDownloading
        {
            get
            {
                return _IsDownloading;
            }
            set
            {
                _IsDownloading = value;
                OnPropertyChanged("IsDownloading");
            }
        }
        
        #endregion

        #region Memberfunction
        public LayoutDocument()
        {
            ButtonDownLoadClick = new NoParameterCommand(ButtonDownLoadClickAction);
            ButtonDownLoadListClick = new NoParameterCommand(ButtonDownLoadListClickAction);
            ButtonSettingOnClick = new NoParameterCommand(ButtonSettingOnClickAction);
        }
        public void ButtonDownLoadClickAction()
        {
            if(IsDownloading)
                return;
            CurrentProgress = 0;
            th = new Thread(DownloadAction);
            th.Start();
        }
        public void ButtonDownLoadListClickAction()
        {
            if (IsDownloading)
                return;
            CurrentProgress = 0;
            MessageBox.Show("還沒做,一次只能下載一首");
            th = new Thread(DownloadAction);
            th.Start();
        }

        private YouTube youTube = null;
        private YouTubeVideo video = null;
        public void DownloadAction()
        {
            try
            {
                IsDownloading = true;
                youTube = YouTube.Default; // starting point for YouTube actions
                video = youTube.GetVideo(Url); // gets a Video object with info about the video
                using (var writer = new BinaryWriter(System.IO.File.Open(@"D:\"+video.FullName, FileMode.Create)))
                {
                    var bytesLeft = video.GetBytes().Length;
                    var array = video.GetBytes();
                    ProgressMax = array.Length;
                    var bytesWritten = 0;
                    while (bytesLeft > 0)
                    {
                        int chunk = Math.Min(64, bytesLeft);
                        writer.Write(array, bytesWritten, chunk);
                        bytesWritten += chunk;
                        bytesLeft -= chunk;
                        CurrentProgress = bytesWritten;
                    }
                }

            }
            catch (Exception ie)
            {
                MessageBox.Show($"Exception : {ie.Message}\r\n{ie.StackTrace}");
            }
            finally
            {
                IsDownloading = false;
                youTube = null;
                video = null;


            }

        }

        public void ButtonSettingOnClickAction()
        {
            uc = new UcDownLoadSetting(DownloadPath);
            uc.Title = "Im Lazy";
            uc.Show();
            uc.Closed += Uc_Closed;
        }

        private void Uc_Closed(object sender, EventArgs e)
        {
            DownloadPath = uc.DownLoadPath;
        }

        #endregion
    }
}
