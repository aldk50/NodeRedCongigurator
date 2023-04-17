using Microsoft.Web.WebView2.WinForms;
using Microsoft.Web.WebView2.Wpf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
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
using wv2 = Microsoft.Web.WebView2.Wpf.WebView2;
namespace NodeRedConfigurator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;



            IsFullSize = !Settings.Default.NoFullScreen;
            new CycleAction(() => { SystemTime = DateTime.Now; }, 1, TimeIntervalType.Seconds, "Get SystemTime").Start();
            try
            {
                System.Diagnostics.Process process = new System.Diagnostics.Process();
                System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
                startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Minimized;
                startInfo.FileName = "cmd.exe";
                startInfo.Arguments = "/C node-red";
                process.StartInfo = startInfo;
                process.Start();
                ConfBut.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#900001"));
                DashBut.Background = new SolidColorBrush(Colors.DimGray);
                AllBut.Background = new SolidColorBrush(Colors.DimGray);
                RedGrid.Children.Add(confwv);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            this.Closed += MainWindow_Closed;
            this.KeyDown += MainWindow_KeyDown;
            this.SizeChanged += MainWindow_SizeChanged;
        }

        private void MainWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (IsAllView)
            {
                if (DashWidth == 0) DashWidth = 0.5;
                if (DashWidth < 0.1) DashWidth = 0.1;
                if (DashWidth > 0.9) DashWidth = 0.9;
                ColumnDefinition col0 = new ColumnDefinition() { Width = new GridLength(10 * DashWidth, GridUnitType.Star) };
                ColumnDefinition col1 = new ColumnDefinition() { Width = new GridLength(10 - (10 * DashWidth), GridUnitType.Star) };
            }
        }

        static wv2 confwv = new wv2() { Source = new Uri("http://localhost:1880") };
        static wv2 dashwv = new wv2() { Source = new Uri("http://localhost:1880/ui") };
        static GridSplitter spliter = new GridSplitter()
        {
            Width = 6,
            Margin = new Thickness(0),
            VerticalAlignment = VerticalAlignment.Stretch,
            HorizontalAlignment = HorizontalAlignment.Right,
            Background = new SolidColorBrush(Colors.Black)
        };

        Binding bindingConf;
        Binding bindingDash;
        bool IsAllView;

        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F11) { IsFullSize = !IsFullSize; }
        }

        private void MainWindow_Closed(object? sender, EventArgs e)
        {
            foreach (var proc in Process.GetProcessesByName("node"))
            {
                proc.Kill();
            }
        }

        private void ConfigView_Click(object sender, RoutedEventArgs e)
        {
            IsAllView = false;
            ConfBut.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#900001"));
            DashBut.Background = new SolidColorBrush(Colors.DimGray);
            AllBut.Background = new SolidColorBrush(Colors.DimGray);
            RedGrid.Children.Clear();
            RedGrid.ColumnDefinitions.Clear();
            RedGrid.Children.Add(confwv);
        }

        private void DashboardView_Click(object sender, RoutedEventArgs e)
        {
            IsAllView = false;
            DashBut.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#900001"));
            ConfBut.Background = new SolidColorBrush(Colors.DimGray);
            AllBut.Background = new SolidColorBrush(Colors.DimGray);
            RedGrid.Children.Clear();
            RedGrid.ColumnDefinitions.Clear();
            dashwv.Margin = new Thickness(0, 0, 6, 0);
            dashwv.UpdateLayout();
            RedGrid.Children.Add(dashwv);
        }
        private void AllView_Click(object sender, RoutedEventArgs e)
        {
            if (DashWidth == 0) DashWidth = 0.5;
            if (DashWidth < 0.1) DashWidth = 0.1;
            if (DashWidth > 0.9) DashWidth = 0.9;
            IsAllView = true;
            AllBut.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#900001"));
            ConfBut.Background = new SolidColorBrush(Colors.DimGray);
            DashBut.Background = new SolidColorBrush(Colors.DimGray);
            RedGrid.Children.Clear();
            RedGrid.ColumnDefinitions.Clear();
            RedGrid.Children.Add(dashwv);
            RedGrid.Children.Add(confwv);
            RedGrid.Children.Add(spliter);
            ColumnDefinition col0 = new ColumnDefinition() { Width = new GridLength(10 * DashWidth, GridUnitType.Star) };
            ColumnDefinition col1 = new ColumnDefinition() { Width = new GridLength(10 - (10 * DashWidth), GridUnitType.Star) };
            RedGrid.ColumnDefinitions.Add(col0);
            RedGrid.ColumnDefinitions.Add(col1);
            dashwv.Margin = new Thickness(0, 0, 6, 0);
            Grid.SetColumn(dashwv, 0);
            Grid.SetColumn(spliter, 0);
            Grid.SetColumn(confwv, 1);
            confwv.SizeChanged += Confwv_SizeChanged; ;
            dashwv.SizeChanged += Dashwv_SizeChanged; ;
        }

        private void Confwv_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (IsAllView) ConfWidth = e.NewSize.Width / ActualWidth;
        }

        private void Dashwv_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (IsAllView) DashWidth = e.NewSize.Width / ActualWidth;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        private bool isFullSize;
        public bool IsFullSize
        {
            get { return isFullSize; }
            set
            {
                isFullSize = value;

                if (IsFullSize)
                {
                    this.ResizeMode = ResizeMode.NoResize;
                    this.WindowStyle = WindowStyle.None;
                    this.WindowState = WindowState.Maximized;
                    //BottomPanel.Visibility= Visibility.Visible;
                    DT_PanelBorder.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#900001"));
                }
                else
                {
                    //BottomPanel.Visibility = Visibility.Collapsed;
                    DT_PanelBorder.BorderBrush = new SolidColorBrush(Colors.Transparent);
                    App.Current.MainWindow.WindowState = WindowState.Normal;
                    App.Current.MainWindow.WindowStyle = WindowStyle.ThreeDBorderWindow;
                    App.Current.MainWindow.ResizeMode = ResizeMode.CanResize;
                }
                OnPropertyChanged(nameof(IsFullSize));
                Settings.Default.NoFullScreen = !isFullSize;
                Settings.Default.Save();
            }
        }

        private void FullScreen_Click(object sender, RoutedEventArgs e)
        {
            IsFullSize = !IsFullSize;
        }

        private DateTime systemTime = DateTime.Now;

        public DateTime SystemTime
        {
            get { return systemTime; }
            set { systemTime = value; OnPropertyChanged(nameof(SystemTime)); }
        }

        private double confWidth;

        public double ConfWidth
        {
            get { return confWidth; }
            set
            {
                confWidth = value; OnPropertyChanged(nameof(ConfWidth));
            }
        }
        private double dashWidth;

        public double DashWidth
        {
            get { return dashWidth; }
            set { dashWidth = value; OnPropertyChanged(nameof(DashWidth)); }
        }

    }
}
