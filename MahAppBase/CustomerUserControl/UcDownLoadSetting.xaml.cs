using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
using System.Windows.Shapes;

namespace MahAppBase.CustomerUserControl
{
    /// <summary>
    /// UcDownLoadSetting.xaml 的互動邏輯
    /// </summary>
    public partial class UcDownLoadSetting : Window
    {

        #region Declarations
        #endregion

        #region Property
        public string DownLoadPath { get; set; }
        #endregion

        #region Memberfunction
        public UcDownLoadSetting(string CurrentPath)
        {
            InitializeComponent();
            this.DownLoadPath = CurrentPath;
        }
        #endregion
    }
}
