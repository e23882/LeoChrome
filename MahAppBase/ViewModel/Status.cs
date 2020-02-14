using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace MahAppBase.ViewModel
{
    /// <summary>
    /// 框架狀態，包含目前記憶體、CPU使用率
    /// </summary>
    public class Status : ViewModelBase
    {
        #region Declarations
        private float _Cpu;
        private float _Memory;
        private int _UpdateFrequence = 10;
        private bool _IsGetInfo = false;
        private bool _HideResource = false;
        private Task thUpdatStatus;
        private Visibility _ResourceVisibility = Visibility.Visible;
        private string _SoundSource = string.Empty;
        #endregion

        #region Property
        /// <summary>
        /// 程式CPU使用率
        /// </summary>
        public float Cpu
        {
            get { return _Cpu; }
            set { _Cpu = value; OnPropertyChanged("Cpu"); }
        }

        /// <summary>
        /// 程式使用記憶體
        /// </summary>
        public float Memory
        {
            get
            {
                return _Memory;
            }
            set
            {
                _Memory = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// 是否啟用監控程式資源屬性
        /// </summary>
        public bool IsGetInfo
        {
            get
            {
                return _IsGetInfo;
            }
            set
            {
                _IsGetInfo = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// 更新頻率屬性
        /// </summary>
        public int UpdateFrequence
        {
            get
            {
                return _UpdateFrequence;
            }
            set
            {
                _UpdateFrequence = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// StatusBar顯示屬性
        /// </summary>
        public Visibility ResourceVisibility
        {
            get
            {
                return _ResourceVisibility;
            }
            set
            {
                _ResourceVisibility = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// 是否隱藏StatusBar屬性
        /// </summary>
        public bool HideResource
        {
            get
            {
                return _HideResource;
            }
            set
            {
                if (_HideResource != value)
                {
                    _HideResource = value;
                    if (_HideResource)
                        ResourceVisibility = Visibility.Collapsed;
                    else
                        ResourceVisibility = Visibility.Visible;
                    OnPropertyChanged();
                }
            }
        }
        #endregion

        #region Memberfunction
        /// <summary>
        /// ViewModel建構子
        /// </summary>
        public Status()
        {
            //初始化、啟動獲取程式狀態Task
            this.thUpdatStatus = new Task(() => { CatchPcStatus(); });
            thUpdatStatus.Start();
        }

        /// <summary>
        /// 取得程式目前使用資源
        /// </summary>
        public void CatchPcStatus()
        {
            var name = Process.GetCurrentProcess().ProcessName;
            var cpuCounter = new PerformanceCounter("Process", "% Processor Time", name);
            var ramCounter = new PerformanceCounter("Process", "Working Set", name);
            
            while (true)
            {
                if (IsGetInfo)
                {
                    try
                    {
                        Cpu = cpuCounter.NextValue();
                        Memory = float.Parse((ramCounter.NextValue() / 1e+6).ToString());
                    }
                    catch (Exception)
                    {
                        Cpu = 0;
                        Memory = 0;
                    }
                    finally
                    {
                        Thread.Sleep(UpdateFrequence*1000);
                    }
                }
                else
                {
                    Thread.Sleep(1000);
                }
            }
        }
        #endregion
    }
}
