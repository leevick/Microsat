using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Microsat
{
    /// <summary>
    /// Ctrl_ImageView.xaml 的交互逻辑
    /// </summary>
    public partial class Ctrl_ImageView : UserControl
    {
        /// <summary>
        /// 图片名称
        /// </summary>
        public string Title;

        public Ctrl_ImageView()
        {
            InitializeComponent();
        }
        public Ctrl_ImageView(string title)
        {
            InitializeComponent();
        }

        public Ctrl_ImageView(string title,Bitmap bmp)
        {
            InitializeComponent();
           // this.IMG1.Source = bmp;
        }

     
    }
}
