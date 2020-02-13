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
            MainViewModel.MainGroupInstance = MainGroup;
            mwMain.DataContext = MainViewModel;
        }
        #endregion
    }
}