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
using Xceed.Wpf.Toolkit;
using Xceed.Wpf.DataGrid;
using Microsat.BackgroundTasks;
using Microsat.Shared;
using System.Threading;
using System.Data;

namespace Microsat
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.Top = Variables.Screen_Locations[0].Y;
            this.Left = Variables.Screen_Locations[0].X;
        }



        #region 数据管
        private CancellationTokenSource cancelImport = new CancellationTokenSource();
        private async void b_Start_Import_Click(object sender, RoutedEventArgs e)
        {
            Button b = sender as Button;
            IProgress<double> IProgress_Prog = new Progress<double>((ProgressValue) => { prog_Import.Value = ProgressValue * this.prog_Import.Maximum; });
            IProgress<string> IProgress_List = new Progress<string>((ProgressString) => { this.tb_Console.Text=ProgressString+"\n"; });
            this.b_Abort_Import.IsEnabled = true;
            this.b_Start_Import.IsEnabled = false;
            this.b_Open_Import.IsEnabled = false;
            string result= await BackgroundTasks.DataProc.Import_4(IProgress_Prog,IProgress_List,cancelImport.Token);
            System.Windows.MessageBox.Show(result);
            this.b_Start_Import.IsEnabled = false;
            this.b_Abort_Import.IsEnabled = false;
            this.b_Open_Import.IsEnabled = true;
            this.prog_Import.Value = 0;
        }
        private void b_Open_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFile = new Microsoft.Win32.OpenFileDialog();
            openFile.Filter = "All Files(*.*)|*.*";
            if ((bool)openFile.ShowDialog())
            {
                Variables.str_FilePath = openFile.FileName;
                this.tb_Path.Text = openFile.FileName;
                this.b_Start_Import.IsEnabled = true;
            }
        }
        private void b_Abort_Import_Click(object sender, RoutedEventArgs e)
        {
            cancelImport.Cancel();
        }
        #endregion

        #region 数据分析显示界面

        #region 局部变量
        public DateTime start_time; //起始检索时刻
        public DateTime end_time;   //终止检索时刻
        public Coord Coord_TL = new Coord(0,0);      //左上角坐标
        public Coord Coord_DR = new Coord(0,0);      //右下角坐标
        public long start_FrmCnt;   //起始帧号
        public long end_FrmCnt;     //终止帧号
        DataTable dt_Result=new DataTable();        //结果
        #endregion

        #region 界面控制按钮
        private async void b_Query_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DataTable dt = await DataProc.QueryResult((bool)cb_byTime.IsChecked, (bool)this.cb_byCoord.IsChecked, (bool)this.cb_byFrmCnt.IsChecked, start_time, end_time, start_FrmCnt, end_FrmCnt, Coord_TL, Coord_DR);
                this.dataGrid_Result.ItemsSource = dt.DefaultView;
                this.dt_Result = dt;
            }
            catch (Exception E)
            {
                System.Windows.MessageBox.Show(E.Message);
            }

            System.Windows.MessageBox.Show("显示完成！");
        }
        private void button_Exit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private async void b_Find_Click(object sender, RoutedEventArgs e)
        {
            DataTable dt = await DataProc.QueryResult();
            this.dataGrid.ItemsSource = dt.DefaultView;
            System.Windows.MessageBox.Show("显示完成！");
        }
        private void button_Display_Click(object sender, RoutedEventArgs e)
        {

            DataTable dt = this.dt_Result;
            App.global_Win_Map.Show();
            App.global_Win_Map.DrawRectangle(new Point((double)dt_Result.Rows[0].ItemArray[3], (double)dt_Result.Rows[0].ItemArray[4]), new Point((double)dt_Result.Rows[dt_Result.Rows.Count - 1].ItemArray[3], (double)dt_Result.Rows[dt_Result.Rows.Count - 1].ItemArray[4]));
            //App.global_Win_ImageShow.Refresh(dt);
            //App.global_Win_ImageShow.Show();
            App.global_Win_3DCube.Show();
            App.global_Win_3DCube.Refresh(dt);
            
        }
        private void Win_Main_Closed(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }
        private void button_Clear_Result_Click(object sender, RoutedEventArgs e)
        {
            dt_Result.Clear();
            dataGrid_Result.ItemsSource = null;

        }
        #endregion

        #region 查询条件设定
        private void dtp_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            DateTimePicker dp = sender as DateTimePicker;
            switch (dp.Name)
            {
                case "dtp_Start": { start_time = (DateTime)dtp_Start.Value; } break;
                case "dtp_End": { end_time = (DateTime)dtp_End.Value; } break;
                default: break;
            }
        }

        private void tb_frm_TextChanged(object sender, TextChangedEventArgs e)
        {
            long.TryParse(this.tb_start_frm.Text, out start_FrmCnt);
            long.TryParse(this.tb_end_frm.Text, out end_FrmCnt);
        }

        private void DDMMSS_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            DDMMSS_SetValue();
        }

        private void DD_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            DD_SetValue();

        }

        private void DDMM_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            DDMM_SetValue();
        }

        private void DDMMSS_GetValue()
        {
            tb_TL_lat_DDMMSS_DD.Value = (int)Coord_TL.Lat;
            tb_TL_lat_DDMMSS_MM.Value = (int)((Coord_TL.Lat * 60) - (int)Coord_TL.Lat * 60);
            tb_TL_lat_DDMMSS_SS.Value = (int)(Coord_TL.Lat * 3600 - ((int)((Coord_TL.Lat * 60) - (int)Coord_TL.Lat * 60)) * 60 - (int)Coord_TL.Lat * 3600);
            tb_TL_lon_DDMMSS_DD.Value = (int)Coord_TL.Lon;
            tb_TL_lon_DDMMSS_MM.Value = (int)(Coord_TL.Lon * 60) - (int)Coord_TL.Lon * 60;
            tb_TL_lon_DDMMSS_SS.Value = (int)(Coord_TL.Lon * 3600 - ((int)((Coord_TL.Lon * 60) - (int)Coord_TL.Lon * 60)) * 60 - (int)Coord_TL.Lon * 3600);
            tb_DR_lat_DDMMSS_DD.Value = (int)Coord_DR.Lat;
            tb_DR_lat_DDMMSS_MM.Value = (int)(Coord_DR.Lat * 60) - (int)Coord_DR.Lat * 60;
            tb_DR_lat_DDMMSS_SS.Value = (int)(Coord_DR.Lat * 3600 - ((int)((Coord_DR.Lat * 60) - (int)Coord_DR.Lat * 60)) * 60 - (int)Coord_DR.Lat * 3600);
            tb_DR_lon_DDMMSS_DD.Value = (int)Coord_DR.Lon;
            tb_DR_lon_DDMMSS_MM.Value = (int)(Coord_DR.Lon * 60) - (int)Coord_DR.Lon * 60;
            tb_DR_lon_DDMMSS_SS.Value = (int)(Coord_DR.Lon * 3600 - ((int)((Coord_DR.Lon * 60) - (int)Coord_DR.Lon * 60)) * 60 - (int)Coord_DR.Lon * 3600);
        }

        private void DDMM_GetValue()
        {
            tb_TL_lat_DDMM_DD.Value = (int)Coord_TL.Lat;
            tb_TL_lat_DDMM_MM.Value = (decimal)(Coord_TL.Lat * 60) - (int)Coord_TL.Lat * 60;
            tb_TL_lon_DDMM_DD.Value = (int)Coord_TL.Lon;
            tb_TL_lon_DDMM_MM.Value = (decimal)(Coord_TL.Lon * 60) - (int)Coord_TL.Lon * 60;
            tb_DR_lat_DDMM_DD.Value = (int)Coord_DR.Lat;
            tb_DR_lat_DDMM_MM.Value = (decimal)(Coord_DR.Lat * 60) - (int)Coord_DR.Lat * 60;
            tb_DR_lon_DDMM_DD.Value = (int)Coord_DR.Lon;
            tb_DR_lon_DDMM_MM.Value = (decimal)(Coord_DR.Lon * 60) - (int)Coord_DR.Lon * 60;
        }

        private void DD_GetValue()
        {
            tb_TL_lat_DD_DD.Value = Coord_TL.Lat;

            tb_TL_lon_DD_DD.Value = Coord_TL.Lon;

            tb_DR_lat_DD_DD.Value = Coord_DR.Lat;

            tb_DR_lon_DD_DD.Value = Coord_DR.Lon;
        }
        private void DDMMSS_SetValue()
        {
            Coord_TL.Lat = (double)tb_TL_lat_DDMMSS_DD.Value + (double)tb_TL_lat_DDMMSS_MM.Value / 60 + (double)tb_TL_lat_DDMMSS_SS.Value / 3600;
            Coord_TL.Lon = (double)tb_TL_lon_DDMMSS_DD.Value + (double)tb_TL_lon_DDMMSS_MM.Value / 60 + (double)tb_TL_lon_DDMMSS_SS.Value / 3600;

            Coord_DR.Lat = (double)tb_DR_lat_DDMMSS_DD.Value + (double)tb_DR_lat_DDMMSS_MM.Value / 60 + (double)tb_DR_lat_DDMMSS_SS.Value / 3600;
            Coord_DR.Lon = (double)tb_DR_lon_DDMMSS_DD.Value + (double)tb_DR_lon_DDMMSS_MM.Value / 60 + (double)tb_DR_lon_DDMMSS_SS.Value / 3600;
        }

        private void DDMM_SetValue()
        {
            Coord_TL.Lat = (double)tb_TL_lat_DDMM_DD.Value + (double)tb_TL_lat_DDMM_MM.Value / 60;
            Coord_TL.Lon = (double)tb_TL_lon_DDMM_DD.Value + (double)tb_TL_lon_DDMM_MM.Value / 60;

            Coord_DR.Lat = (double)tb_DR_lat_DDMM_DD.Value + (double)tb_DR_lat_DDMM_MM.Value / 60;
            Coord_DR.Lon = (double)tb_DR_lon_DDMM_DD.Value + (double)tb_DR_lon_DDMM_MM.Value / 60;
        }

        private void DD_SetValue()
        {
            Coord_TL.Lat = (double)tb_TL_lat_DD_DD.Value;
            Coord_TL.Lon = (double)tb_TL_lon_DD_DD.Value;

            Coord_DR.Lat = (double)tb_DR_lat_DD_DD.Value;
            Coord_DR.Lon = (double)tb_DR_lon_DD_DD.Value;
        }

        private void tc_Coord_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (tc_Coord.SelectedIndex)
            {
                case 0:
                    {
                        DDMMSS_UnsetTrigger();
                        DDMM_UnsetTrigger();
                        DD_UnsetTrigger();
                        DDMMSS_GetValue();
                        DDMM_SetValue();
                        DD_SetValue();
                        DDMMSS_SetTrigger();
                    }
                    break;
                case 1:
                    {
                        DDMMSS_UnsetTrigger();
                        DDMM_UnsetTrigger();
                        DD_UnsetTrigger();
                        DD_GetValue();
                        DDMM_SetValue();
                        DDMMSS_SetValue();
                        DD_SetTrigger();
                    }
                    break;
                case 2:
                    {
                        DDMMSS_UnsetTrigger();
                        DDMM_UnsetTrigger();
                        DD_UnsetTrigger();
                        DDMM_GetValue();
                        DDMMSS_SetValue();
                        DD_SetValue();
                        DDMM_SetTrigger();

                    }
                    break;
                default: break;
            }
        }

        private void tc_Coord_Loaded(object sender, RoutedEventArgs e)
        {
            tc_Coord.SelectionChanged += new SelectionChangedEventHandler(tc_Coord_SelectionChanged);
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {

        }


        private void DDMMSS_SetTrigger()
        {
            this.tb_TL_lat_DDMMSS_DD.ValueChanged += DDMMSS_ValueChanged;
            this.tb_TL_lon_DDMMSS_MM.ValueChanged += DDMMSS_ValueChanged;
            this.tb_TL_lat_DDMMSS_SS.ValueChanged += DDMMSS_ValueChanged;
            this.tb_TL_lon_DDMMSS_DD.ValueChanged += DDMMSS_ValueChanged;
            this.tb_TL_lat_DDMMSS_MM.ValueChanged += DDMMSS_ValueChanged;
            this.tb_TL_lon_DDMMSS_SS.ValueChanged += DDMMSS_ValueChanged;
            this.tb_DR_lat_DDMMSS_DD.ValueChanged += DDMMSS_ValueChanged;
            this.tb_DR_lon_DDMMSS_MM.ValueChanged += DDMMSS_ValueChanged;
            this.tb_DR_lat_DDMMSS_SS.ValueChanged += DDMMSS_ValueChanged;
            this.tb_DR_lon_DDMMSS_DD.ValueChanged += DDMMSS_ValueChanged;
            this.tb_DR_lat_DDMMSS_MM.ValueChanged += DDMMSS_ValueChanged;
            this.tb_DR_lon_DDMMSS_SS.ValueChanged += DDMMSS_ValueChanged;
        }

        private void DDMMSS_UnsetTrigger()
        {
            this.tb_TL_lat_DDMMSS_DD.ValueChanged -= DDMMSS_ValueChanged;
            this.tb_TL_lon_DDMMSS_MM.ValueChanged -= DDMMSS_ValueChanged;
            this.tb_TL_lat_DDMMSS_SS.ValueChanged -= DDMMSS_ValueChanged;
            this.tb_TL_lon_DDMMSS_DD.ValueChanged -= DDMMSS_ValueChanged;
            this.tb_TL_lat_DDMMSS_MM.ValueChanged -= DDMMSS_ValueChanged;
            this.tb_TL_lon_DDMMSS_SS.ValueChanged -= DDMMSS_ValueChanged;
            this.tb_DR_lat_DDMMSS_DD.ValueChanged -= DDMMSS_ValueChanged;
            this.tb_DR_lon_DDMMSS_MM.ValueChanged -= DDMMSS_ValueChanged;
            this.tb_DR_lat_DDMMSS_SS.ValueChanged -= DDMMSS_ValueChanged;
            this.tb_DR_lon_DDMMSS_DD.ValueChanged -= DDMMSS_ValueChanged;
            this.tb_DR_lat_DDMMSS_MM.ValueChanged -= DDMMSS_ValueChanged;
            this.tb_DR_lon_DDMMSS_SS.ValueChanged -= DDMMSS_ValueChanged;
        }

        private void DD_SetTrigger()
        {
            this.tb_TL_lat_DD_DD.ValueChanged += DD_ValueChanged;
            this.tb_TL_lon_DD_DD.ValueChanged += DD_ValueChanged;
            this.tb_DR_lat_DD_DD.ValueChanged += DD_ValueChanged;
            this.tb_DR_lat_DD_DD.ValueChanged += DD_ValueChanged;


        }

        private void DD_UnsetTrigger()
        {
            this.tb_TL_lat_DD_DD.ValueChanged -= DD_ValueChanged;
            this.tb_TL_lon_DD_DD.ValueChanged -= DD_ValueChanged;
            this.tb_DR_lat_DD_DD.ValueChanged -= DD_ValueChanged;
            this.tb_DR_lat_DD_DD.ValueChanged -= DD_ValueChanged;

        }

        private void DDMM_SetTrigger()
        {
            this.tb_TL_lat_DDMM_DD.ValueChanged += DDMM_ValueChanged;
            this.tb_TL_lon_DDMM_DD.ValueChanged += DDMM_ValueChanged;

            this.tb_TL_lat_DDMM_MM.ValueChanged += DDMM_ValueChanged;
            this.tb_TL_lon_DDMM_MM.ValueChanged += DDMM_ValueChanged;

            this.tb_DR_lat_DDMM_DD.ValueChanged += DDMM_ValueChanged;
            this.tb_DR_lon_DDMM_DD.ValueChanged += DDMM_ValueChanged;

            this.tb_DR_lat_DDMM_MM.ValueChanged += DDMM_ValueChanged;
            this.tb_DR_lon_DDMM_MM.ValueChanged += DDMM_ValueChanged;

        }

        private void DDMM_UnsetTrigger()
        {
            this.tb_TL_lat_DDMM_DD.ValueChanged -= DDMM_ValueChanged;
            this.tb_TL_lon_DDMM_DD.ValueChanged -= DDMM_ValueChanged;

            this.tb_TL_lat_DDMM_MM.ValueChanged -= DDMM_ValueChanged;
            this.tb_TL_lon_DDMM_MM.ValueChanged -= DDMM_ValueChanged;

            this.tb_DR_lat_DDMM_DD.ValueChanged -= DDMM_ValueChanged;
            this.tb_DR_lon_DDMM_DD.ValueChanged -= DDMM_ValueChanged;

            this.tb_DR_lat_DDMM_MM.ValueChanged -= DDMM_ValueChanged;
            this.tb_DR_lon_DDMM_MM.ValueChanged -= DDMM_ValueChanged;
        }

        #endregion

        #endregion




    }
}
