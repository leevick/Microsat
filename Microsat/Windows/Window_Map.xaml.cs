using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
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
using Microsat.Shared;
namespace Microsat
{
    /// <summary>
    /// Window_Map.xaml 的交互逻辑
    /// </summary>
    public partial class Window_Map : Window
    {
        public static String pathMap = "file://127.0.0.1/D$/Amap/Amap.html?zoom=7&disp=true";
        public Window_Map()
        {
            InitializeComponent();
        }
        public Window_Map(System.Drawing.Rectangle rectangle)
        {
            InitializeComponent();
            this.Top = rectangle.Y;
            this.Left = rectangle.X;
        }
        public void DrawRectangle(System.Windows.Point Start, System.Windows.Point End)
        {
            Uri uri = new Uri(pathMap + "&lat=" + (0.5*Start.X+0.5*End.X).ToString()+ "&lon=" + (0.5*Start.Y+0.5*End.Y).ToString()+"&start_lat="+Start.X.ToString()+"&start_lon="+Start.Y.ToString()+"&end_lat="+End.X.ToString()+"&end_lon="+End.Y.ToString());
            webMap.Navigate(uri);
            Uri uri_2 = new Uri("file://127.0.0.1/C$/Users/Victor/Desktop/f3earth-master/examples/simple.html");
            webEarth.Navigate(uri_2);
        }
        private void windowMap_Loaded(object sender, RoutedEventArgs e)
        {
            Uri uri = new Uri(pathMap);
            webMap.Navigate(uri);
        }
        private void windowMap_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.Hide();
            e.Cancel = true;
        }
    }
}