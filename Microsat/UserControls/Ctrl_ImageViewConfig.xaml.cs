using System;
using System.Collections.Generic;
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

namespace Microsat.UserControls
{
    /// <summary>
    /// Ctrl_ImageViewConfig.xaml 的交互逻辑
    /// </summary>
    /// 
    

    public partial class Ctrl_ImageViewConfig : UserControl
    {
        private int _ScreenIndex;
        public int ScreenIndex
        {
            get { return _ScreenIndex; }
            set { _ScreenIndex = value;
                this.groupBox3.Header = $"窗 {_ScreenIndex+1} 设置";
            }
        }
        public Ctrl_ImageViewConfig()
        {
            InitializeComponent();
        }
    }
}
