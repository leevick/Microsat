using Microsat.BackgroundTasks;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
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

namespace Microsat.UserControls
{
    /// <summary>
    /// Ctrl_ImageViewConfig.xaml 的交互逻辑
    /// </summary>
    /// 
    

    public partial class Ctrl_ImageViewConfig : UserControl
    {
        public enum RenderMode { byBand,byCoord,byRGB,None}

        public RenderMode ImgRenderMode;
        private int _ScreenIndex;
        public int ScreenIndex
        {
            get { return _ScreenIndex; }
            set { _ScreenIndex = value;
                this.groupBox3.Header = $"窗 {_ScreenIndex+1} 设置";
            }
        }
        private RadioButton[] rb = new RadioButton[3];
        public Ctrl_ImageViewConfig()
        {
            InitializeComponent();
            rb[0] = this.radioButton_byBand;
            rb[1] = this.radioButton_byCoord;
            rb[2] = this.radioButton_byRGB;

            rb[0].Checked += RadioButton_Checked;
            rb[1].Checked += RadioButton_Checked;
            rb[2].Checked += RadioButton_Checked;


        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < 3; i++)
            {
                ImgRenderMode = rb[i].IsChecked==true ?(RenderMode)i:RenderMode.None;
            }
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {

            this.ProgressBar_Status.IsIndeterminate = true;
            App.global_Win_SpecImg.u[this.ScreenIndex].Busy.isBusy = true;
            App.global_Win_SpecImg.u[this.ScreenIndex].Refresh((int)this.IntegerUpDown_Band.Value,this.ImgRenderMode,new int [3]{0 ,0,0});
            this.ProgressBar_Status.IsIndeterminate = false;
            App.global_Win_SpecImg.u[this.ScreenIndex].Busy.isBusy = false;
        }

        private void button_Copy_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
