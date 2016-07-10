using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
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
using Microsat.BackgroundTasks;

namespace Microsat.Windows
{
    /// <summary>
    /// Window_SpecImg.xaml 的交互逻辑
    /// </summary>
    public partial class Window_Split : Window
    {
        public UserControl[] u = new UserControl[4];
        private GridMode _DisplayMode;
        public GridMode DisplayMode
        {
            get
            {
                return _DisplayMode;
            }
            set {
                _DisplayMode = value;
                switch (value)
                {
                    case GridMode.One:
                        {
                            this.grid.ColumnDefinitions[0].Width = new GridLength(1, GridUnitType.Star);
                            this.grid.RowDefinitions[0].Height = new GridLength(1, GridUnitType.Star);
                            this.grid.ColumnDefinitions[1].Width = new GridLength(0, GridUnitType.Star);
                            this.grid.RowDefinitions[1].Height = new GridLength(0, GridUnitType.Star);
                        };break;
                    case GridMode.Two:
                        {
                            this.grid.ColumnDefinitions[0].Width = new GridLength(1, GridUnitType.Star);
                            this.grid.RowDefinitions[0].Height = new GridLength(1, GridUnitType.Star);
                            this.grid.ColumnDefinitions[1].Width = new GridLength(1, GridUnitType.Star);
                            this.grid.RowDefinitions[1].Height = new GridLength(0, GridUnitType.Star);
                        }; break;
                    case GridMode.Four:
                        {
                            this.grid.ColumnDefinitions[0].Width = new GridLength(1, GridUnitType.Star);
                            this.grid.RowDefinitions[0].Height = new GridLength(1, GridUnitType.Star);
                            this.grid.ColumnDefinitions[1].Width = new GridLength(1, GridUnitType.Star);
                            this.grid.RowDefinitions[1].Height = new GridLength(1, GridUnitType.Star);
                        }; break;
                    default:break;
                }
            }
        }
        public Window_Split()
        {
            InitializeComponent();
            DisplayMode = GridMode.Two;
        }
        public enum GridMode { One,Two,Four};
        private void Window_Closed(object sender, EventArgs e)
        {
            this.Hide();
        }
    }
}
