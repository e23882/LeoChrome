using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using MahAppBase.Command;
using MahAppBase.CustomerUserControl;
using YoutubeExplode;
using YoutubeExplode.Videos.Streams;
using MessageBox = System.Windows.MessageBox;

namespace MahAppBase.ViewModel
{
    public class LayoutDocument:ViewModelBase
    {
        #region Declarations
        private string _Url = string.Empty;
        private Visibility _ButtonDownLoadVisibility = Visibility.Collapsed;
        private Visibility _ButtonDownLoadListVisibility = Visibility.Collapsed;
        public Visibility _ShowDownloadTool;
        private UcDownLoadSetting uc = null;
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
            _ = Task.Run(async () => await DownloadAction());
        }

        public void ButtonDownLoadListClickAction()
        {
            if (IsDownloading)
                return;
            CurrentProgress = 0;
            MessageBox.Show("還沒做,一次只能下載一首");
            _ = Task.Run(async () => await DownloadAction());
        }
        
        private async Task DownloadAction()
        {
            try
            {
                IsDownloading = true;

                var youtube = new YoutubeClient();

                // 取得影片資訊
                var video = await youtube.Videos.GetAsync(Url);

                // 取得串流資訊
                var streamManifest = await youtube.Videos.Streams.GetManifestAsync(video.Id);

                // 選擇最高品質的混合串流（包含音訊和視訊）
                var streamInfo = streamManifest.GetMuxedStreams().GetWithHighestVideoQuality();

                if (streamInfo == null)
                {
                    // 如果沒有混合串流，選擇最高品質的純視訊串流
                    streamInfo = streamManifest.GetVideoOnlyStreams().GetWithHighestVideoQuality();
                }

                // 建立安全的檔案名稱
                var fileName = $"{video.Title}.{streamInfo.Container.Name}";
                fileName = string.Join("_", fileName.Split(Path.GetInvalidFileNameChars()));

                // 使用設定的下載路徑或 MyDocuments
                var downloadPath = string.IsNullOrWhiteSpace(DownloadPath)
                    ? Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
                    : DownloadPath;
                var filePath = Path.Combine(downloadPath, fileName);

                // 下載並追蹤進度
                var progress = new Progress<double>(p =>
                {
                    CurrentProgress = (long)(p * 100);
                    ProgressMax = 100;
                });

                await youtube.Videos.Streams.DownloadAsync(streamInfo, filePath, progress);

                MessageBox.Show($"下載完成：{fileName}");
            }
            catch (Exception ie)
            {
                MessageBox.Show($"Exception : {ie.Message}\r\n{ie.StackTrace}");
            }
            finally
            {
                IsDownloading = false;
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
