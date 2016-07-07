using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Microsat.Shared;
using System.Drawing;

namespace Microsat
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        public static Window_SpecCurv global_Win_ImageShow;
        public static Window_Map global_Win_Map;
        public static Window_3DCube global_Win_3DCube;

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            Variables.Screen_Locations = new Rectangle[4];
            #region 设定屏幕属性
            foreach (System.Windows.Forms.Screen s in System.Windows.Forms.Screen.AllScreens)
            {
                Variables.Screens.Add(new ScreenParams(s.WorkingArea,s.DeviceName,s.Primary));
                if (s.WorkingArea.Width == 3840) Variables.Screen_Locations[1] = s.WorkingArea;
            }
            #endregion
            global_Win_ImageShow = new Window_SpecCurv(Variables.Screen_Locations[1]);
            global_Win_Map = new Window_Map(Variables.Screen_Locations[1]);
            global_Win_3DCube = new Window_3DCube();
            //global_Win_Map.Show();
            //global_Win_ImageShow.Show();
        }
    }

}
