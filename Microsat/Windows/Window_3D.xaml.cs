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
using System.Windows.Shapes;
using Microsat.UserControls;

namespace Microsat.Windows
{
    /// <summary>
    /// Window_3D.xaml 的交互逻辑
    /// </summary>
    public partial class Window_3D : Window
    {
        public Window_3D()
        {
            InitializeComponent();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            this.Hide();
        }
    }
}
