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
    /// Ctrl_BusyMask.xaml 的交互逻辑
    /// </summary>
    public partial class Ctrl_BusyMask : UserControl
    {
        private bool _isBusy;
        public bool isBusy
        {
            get{return _isBusy;}
            set{ _isBusy = value;

                this.Mask.Visibility = value == true ? Visibility.Visible : Visibility.Hidden;
                this.Dialog.Visibility = value == true ? Visibility.Visible:Visibility.Hidden;
            }
        }
        public Ctrl_BusyMask()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            isBusy = false;
        }
    }
}
