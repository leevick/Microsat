using Microsoft.Research.DynamicDataDisplay;
using Microsoft.Research.DynamicDataDisplay.DataSources;
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
using Microsat.Shared;
using System.Drawing;
using System.IO;
using Microsat.BackgroundTasks;
using System.Data;
using System.Windows.Media.Animation;
using System.Windows.Media.Media3D;
using Microsoft.Research.DynamicDataDisplay.PointMarkers;

namespace Microsat
{
    /// <summary>
    /// Window_SpecCurv.xaml 的交互逻辑
    /// </summary>
    public partial class Window_SpecCurv : Window
    {
        public int ImportId;
        public int FrmCnt_Start;
        public int FrmCnt_End;
        public int SpecSelected;
        public Coord Start;
        public Coord End;
        public Window_SpecCurv(System.Drawing.Rectangle rectangle)
        {
            InitializeComponent();
            this.Top = rectangle.Y;
            this.Left = rectangle.X;
            initChart();
            
        }

        #region 光谱曲线图
        public int yChart1st = 0;
        public LineAndMarker<MarkerPointsGraph> lm = new LineAndMarker<MarkerPointsGraph>();
        ObservableDataSource<System.Windows.Point> dtsChart1st = new ObservableDataSource<System.Windows.Point>();
        public void initChart()
        {

            //chart1st.AddLineGraph(dtsChart1st, Colors.DeepSkyBlue, 2, "Sin");
              lm = chart1st.AddLineGraph(dtsChart1st,
              new System.Windows.Media.Pen(System.Windows.Media.Brushes.Green, 2),
              new CirclePointMarker { Size = 3.0, Fill = System.Windows.Media.Brushes.Red },
              new PenDescription("Gray-level value"));
              
        }
    
        public async void Refresh(DataTable dt_Result)
        { 
            Bitmap[] bmp = await DataProc.GetBmp(dt_Result);
            ImportId =(int) (dt_Result.Rows[0].ItemArray[14]);
            FrmCnt_Start = int.MaxValue;
            FrmCnt_End = int.MinValue;
            
            foreach (DataRow dr in dt_Result.Rows)
            {
                int accountLevel = dr.Field<int>("FrameId");
                FrmCnt_Start = Math.Min(FrmCnt_Start, accountLevel);
                FrmCnt_End = Math.Max(FrmCnt_End, accountLevel);
            }
            MemoryStream ms = new MemoryStream();
            bmp[0].Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
            BitmapImage bmpSource = new BitmapImage();
            bmpSource.BeginInit();
            bmpSource.StreamSource = ms;
            bmpSource.EndInit();
            this.image.Source = bmpSource;
            this.Stack_Right.Children.Add(new Ctrl_ImageView(bmp[0]));
            foreach (object o in Stack_Right.Children)
            {
                if (o is Ctrl_ImageView)
                {
                    ((Ctrl_ImageView)(o)).grid_Image.Height = this.Stack_Right.ActualHeight / 2;
                }
            }


        }
        private async void image_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            System.Windows.Point p = e.GetPosition(this.image);
            p.X = p.X / this.image.Source.Width;
            p.Y = p.Y / this.image.Source.Height;
            this.image.MouseLeftButtonUp -= image_MouseLeftButtonUp;
            System.Windows.Point[] points = await BackgroundTasks.SpecProc.GetSpecCurv(ImportId,FrmCnt_Start,FrmCnt_End,80,p);
            dtsChart1st = new ObservableDataSource<System.Windows.Point>();
            foreach (System.Windows.Point point in points)
            {
                dtsChart1st.AppendAsync(base.Dispatcher, point);   
            }
            chart1st.Children.Remove(lm.LineGraph);
            chart1st.Children.Remove(lm.MarkerGraph);
            initChart();
            this.image.MouseLeftButtonUp += image_MouseLeftButtonUp;
        }
        #endregion
        private void Window_Closed(object sender, EventArgs e)
        {
            this.Hide();
        }
    }
}
