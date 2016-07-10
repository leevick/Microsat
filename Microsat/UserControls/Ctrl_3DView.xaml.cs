using Microsat.BackgroundTasks;
using Microsat.Shared;
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
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WFTools3D;

namespace Microsat.UserControls
{
    /// <summary>
    /// Ctrl_3DView.xaml 的交互逻辑
    /// </summary>
    public partial class Ctrl_3DView : UserControl
    {

        double imheight = 0;
        public Ctrl_3DView(double lines,System.Drawing.Bitmap[] bmpArray)
        {
            InitializeComponent();
            //WindowState = WindowState.Maximized;

            //ImageBrush brush = GetBrush("Ground", false);
            //brush.TileMode = TileMode.Tile;
            //brush.Viewport = new Rect(0, 0, 0.02, 0.02);
            // brush.Freeze();
            imheight = lines;
            //InitializeScene(brush);
            RenderBox(lines, bmpArray);
            InitializeCameras();
            //InitializeInfo();
        }

        public Ctrl_3DView()
        {
            InitializeComponent();


        }

       


        ImageBrush GetBrush(string name, bool doFreeze = true)
        {
            ImageBrush brush = Resources[name] as ImageBrush;
            if (doFreeze) brush.Freeze();
            return brush;
        }

        private void RenderBox(double lines, System.Drawing.Bitmap[] bmpArray)
        {
            ModelVisual3D mv3d = new ModelVisual3D();
            Model3DGroup m3dg = new Model3DGroup();
            Model3DCollection m3dc = new Model3DCollection();
            MeshGeometry3D mg3d_Top = new MeshGeometry3D();
            MeshGeometry3D mg3d_Bottom = new MeshGeometry3D();
            MeshGeometry3D mg3d_Left = new MeshGeometry3D();
            MeshGeometry3D mg3d_Right = new MeshGeometry3D();
            MeshGeometry3D mg3d_Up = new MeshGeometry3D();
            MeshGeometry3D mg3d_Down = new MeshGeometry3D();

            #region 3D立方体正视图
            mg3d_Top.Positions.Add(new Point3D(0, 0, 64.0));
            mg3d_Top.Positions.Add(new Point3D(204.8, 0, 64.0));
            mg3d_Top.Positions.Add(new Point3D(204.8, lines/10, 64.0));
            mg3d_Top.Positions.Add(new Point3D(0, lines/10, 64.0));
            mg3d_Top.TriangleIndices.Add(0);
            mg3d_Top.TriangleIndices.Add(1);
            mg3d_Top.TriangleIndices.Add(2);
            mg3d_Top.TriangleIndices.Add(2);
            mg3d_Top.TriangleIndices.Add(3);
            mg3d_Top.TriangleIndices.Add(0);
            mg3d_Top.TextureCoordinates.Add(new System.Windows.Point(0, 1));
            mg3d_Top.TextureCoordinates.Add(new System.Windows.Point(1, 1));
            mg3d_Top.TextureCoordinates.Add(new System.Windows.Point(1, 0));
            mg3d_Top.TextureCoordinates.Add(new System.Windows.Point(0, 0));
            #endregion

            #region 3D立方体左视图
            mg3d_Left.Positions.Add(new Point3D(0, 0, 0));
            mg3d_Left.Positions.Add(new Point3D(0, 0, 64.0));
            mg3d_Left.Positions.Add(new Point3D(0, lines/10, 64.0));
            mg3d_Left.Positions.Add(new Point3D(0, lines/10, 0));
            mg3d_Left.TriangleIndices.Add(0);
            mg3d_Left.TriangleIndices.Add(1);
            mg3d_Left.TriangleIndices.Add(2);
            mg3d_Left.TriangleIndices.Add(2);
            mg3d_Left.TriangleIndices.Add(3);
            mg3d_Left.TriangleIndices.Add(0);
            mg3d_Left.TextureCoordinates.Add(new System.Windows.Point(0, 0));
            mg3d_Left.TextureCoordinates.Add(new System.Windows.Point(0, 1));
            mg3d_Left.TextureCoordinates.Add(new System.Windows.Point(1, 1));
            mg3d_Left.TextureCoordinates.Add(new System.Windows.Point(1, 0));
            #endregion

            #region 3D立方体右视图
            mg3d_Right.Positions.Add(new Point3D(204.8, 0, 0));
            mg3d_Right.Positions.Add(new Point3D(204.8, lines/10, 0));
            mg3d_Right.Positions.Add(new Point3D(204.8, lines/10, 64.0));
            mg3d_Right.Positions.Add(new Point3D(204.8, 0, 64.0));
            mg3d_Right.TriangleIndices.Add(0);
            mg3d_Right.TriangleIndices.Add(1);
            mg3d_Right.TriangleIndices.Add(2);
            mg3d_Right.TriangleIndices.Add(2);
            mg3d_Right.TriangleIndices.Add(3);
            mg3d_Right.TriangleIndices.Add(0);
            mg3d_Right.TextureCoordinates.Add(new System.Windows.Point(0, 0));
            mg3d_Right.TextureCoordinates.Add(new System.Windows.Point(1, 0));
            mg3d_Right.TextureCoordinates.Add(new System.Windows.Point(1, 1));
            mg3d_Right.TextureCoordinates.Add(new System.Windows.Point(0, 1));
            #endregion

            #region 3D立方体背视图
            mg3d_Bottom.Positions.Add(new Point3D(0, 0, 0));
            mg3d_Bottom.Positions.Add(new Point3D(0, lines/10, 0));
            mg3d_Bottom.Positions.Add(new Point3D(204.8, lines/10, 0));
            mg3d_Bottom.Positions.Add(new Point3D(204.8, 0, 0));
            mg3d_Bottom.TriangleIndices.Add(0);
            mg3d_Bottom.TriangleIndices.Add(1);
            mg3d_Bottom.TriangleIndices.Add(2);
            mg3d_Bottom.TriangleIndices.Add(2);
            mg3d_Bottom.TriangleIndices.Add(3);
            mg3d_Bottom.TriangleIndices.Add(0);
            mg3d_Bottom.TextureCoordinates.Add(new System.Windows.Point(0, 1));
            mg3d_Bottom.TextureCoordinates.Add(new System.Windows.Point(0, 0));
            mg3d_Bottom.TextureCoordinates.Add(new System.Windows.Point(1, 0));
            mg3d_Bottom.TextureCoordinates.Add(new System.Windows.Point(1, 1));
            #endregion

            #region 3D立方体顶视图
            mg3d_Up.Positions.Add(new Point3D(0, lines/10, 0));
            mg3d_Up.Positions.Add(new Point3D(0, lines/10, 64.0));
            mg3d_Up.Positions.Add(new Point3D(204.8, lines/10, 64.0));
            mg3d_Up.Positions.Add(new Point3D(204.8, lines/10, 0));
            mg3d_Up.TriangleIndices.Add(0);
            mg3d_Up.TriangleIndices.Add(1);
            mg3d_Up.TriangleIndices.Add(2);
            mg3d_Up.TriangleIndices.Add(2);
            mg3d_Up.TriangleIndices.Add(3);
            mg3d_Up.TriangleIndices.Add(0);
            mg3d_Up.TextureCoordinates.Add(new System.Windows.Point(0, 0));
            mg3d_Up.TextureCoordinates.Add(new System.Windows.Point(0, 1));
            mg3d_Up.TextureCoordinates.Add(new System.Windows.Point(1, 1));
            mg3d_Up.TextureCoordinates.Add(new System.Windows.Point(1, 0));
            #endregion

            #region 3D立方体底视图
            mg3d_Down.Positions.Add(new Point3D(0, 0, 0));
            mg3d_Down.Positions.Add(new Point3D(204.8, 0, 0));
            mg3d_Down.Positions.Add(new Point3D(204.8, 0, 64.0));
            mg3d_Down.Positions.Add(new Point3D(0, 0, 64.0));
            mg3d_Down.TriangleIndices.Add(0);
            mg3d_Down.TriangleIndices.Add(1);
            mg3d_Down.TriangleIndices.Add(2);
            mg3d_Down.TriangleIndices.Add(2);
            mg3d_Down.TriangleIndices.Add(3);
            mg3d_Down.TriangleIndices.Add(0);
            mg3d_Down.TextureCoordinates.Add(new System.Windows.Point(0, 0));
            mg3d_Down.TextureCoordinates.Add(new System.Windows.Point(1, 0));
            mg3d_Down.TextureCoordinates.Add(new System.Windows.Point(1, 1));
            mg3d_Down.TextureCoordinates.Add(new System.Windows.Point(0, 1));
            #endregion


            BitmapImage[] bmpSource = new BitmapImage[6];

            /*
            for (int i = 0; i < 3; i++)
            {
                MemoryStream ms = new MemoryStream();
                bmpArray[i].Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
                bmpSource[i] = new BitmapImage();
                bmpSource[i].BeginInit();
                bmpSource[i].StreamSource = ms;
                bmpSource[i].EndInit();
            }
            */

            for (int i = 0; i <6; i++)
            {
                bmpArray[i].Save($"cube_{i}.bmp");
                bmpSource[i] = new BitmapImage(new Uri($"{Environment.CurrentDirectory}\\cube_{i}.bmp"));
            };

            DiffuseMaterial dm_Top = new DiffuseMaterial(new System.Windows.Media.ImageBrush(bmpSource[0]));
            DiffuseMaterial dm_Bottom = new DiffuseMaterial(new System.Windows.Media.ImageBrush(bmpSource[1]));
            DiffuseMaterial dm_Up = new DiffuseMaterial(new System.Windows.Media.ImageBrush(bmpSource[2]));
            DiffuseMaterial dm_Down = new DiffuseMaterial(new System.Windows.Media.ImageBrush(bmpSource[3]));
            DiffuseMaterial dm_Right = new DiffuseMaterial(new System.Windows.Media.ImageBrush(bmpSource[4]));
            DiffuseMaterial dm_Left = new DiffuseMaterial(new System.Windows.Media.ImageBrush(bmpSource[5]));
            GeometryModel3D gm3d_Top = new GeometryModel3D(mg3d_Top, dm_Top);
            GeometryModel3D gm3d_Left = new GeometryModel3D(mg3d_Left, dm_Left);
            GeometryModel3D gm3d_Right = new GeometryModel3D(mg3d_Right, dm_Right);
            GeometryModel3D gm3d_Bottom = new GeometryModel3D(mg3d_Bottom, dm_Bottom);
            GeometryModel3D gm3d_Up = new GeometryModel3D(mg3d_Up, dm_Up);
            GeometryModel3D gm3d_Down = new GeometryModel3D(mg3d_Down, dm_Down);

            m3dg.Children.Add(gm3d_Top);
            m3dg.Children.Add(gm3d_Left);
            m3dg.Children.Add(gm3d_Right);
            m3dg.Children.Add(gm3d_Bottom);
            m3dg.Children.Add(gm3d_Up);
            m3dg.Children.Add(gm3d_Down);
            mv3d.Content = m3dg;
            this.scene.Viewport.Children.Add(mv3d);
        }

        internal async void Refresh()
        {
            Bitmap[] bmp = await DataProc.GetBmp3D();
            imheight = DataQuery.QueryResult.Rows.Count;
            //InitializeScene(brush);
            RenderBox(imheight, bmp);
            InitializeCameras();
            //InitializeInfo();

        }

        void InitializeScene(System.Windows.Media.Brush brush)
        {
            Square square = new Square(100);
            square.ScaleX = 60;
            square.ScaleY = 40;
            square.Transform.Freeze();
            square.DiffuseMaterial.Brush = brush;
            square.BackMaterial = square.Material;
            scene.Models.Add(square);

            System.Windows.Media.Brush brush1 = GetBrush("Facade");
            System.Windows.Media.Brush brush2 = GetBrush("Poster");

            for (int i = 0; i < 170; i++)
            {
                double x = 58 * (2 * randy.NextDouble() - 1);
                double y = 38 * (2 * randy.NextDouble() - 1);
                double z = 3 * randy.NextDouble() + 1;
                Object3D obj = RandomPrimitive(x, y, z, brush1, brush2);
                scene.Models.Add(obj);
            }

            Tube tube = new Tube(12) { Radius = 0.1, IsPathClosed = true };
            int n = 180;
            List<Point3D> path = new List<Point3D>(n);
            for (int i = 0; i < n; i++)
            {
                double t = MathUtils.PIx2 * i / n;
                path.Add(new Point3D(Math.Cos(t), Math.Sin(t), Math.Cos(6 * t) / 3));
            }
            tube.Path = path;
            tube.DiffuseMaterial.Brush = System.Windows.Media.Brushes.Green;
            tube.Position = new Point3D(-2, 2, 1);
            scene.Models.Add(tube);

            RunningMan man = new RunningMan(60, 40);
            scene.Models.Add(man);
            scene.TimerTicked += man.TimerTick;
            scene.TimerTicked += TimerTick;

            scene.ToggleHelperModels();
            FocusManager.SetFocusedElement(this, scene);
        }
        Random randy = new Random(0);

        Object3D RandomPrimitive(double x, double y, double z, System.Windows.Media.Brush brush1, System.Windows.Media.Brush brush2)
        {
            Primitive3D obj = null;
            double angle1 = 180 * randy.NextDouble();
            double angle2 = 180 * (1 + randy.NextDouble());
            int index = count > 150 ? 2 : ++count % 10;
            switch (index)
            {
                case 0: //--- more cubes, please :-)
                case 1: obj = new Cube(); break;
                case 2: obj = new Balloon(z); break;
                case 3: obj = new Cone { IsClosed = true }; break;
                case 4: obj = new Cylinder { IsClosed = true }; break;
                case 5: obj = new Cylinder { StartDegrees = angle1, StopDegrees = angle2, IsClosed = true }; break;
                case 6: obj = new Disk(); break;
                case 7: obj = new Disk { StartDegrees = angle1, StopDegrees = angle2 }; break;
                case 8: obj = new Square(); break;
                case 9: obj = new Triangle(); break;
            }
            obj.ScaleZ = z;
            obj.Position = new Point3D(x, y, z);
            obj.DiffuseMaterial.Brush = GetRandomBrush();

            if (index > 5)
            {
                //--- flat objects need a BackMaterial and are rotated
                obj.BackMaterial = obj.Material;
                obj.Rotation1 = Math3D.RotationX(angle1);
                obj.Rotation2 = Math3D.RotationY(angle2);
            }
            else if (index < 2)//--- cubes
            {
                obj.DiffuseMaterial.Brush = brush1;
                obj.Position = new Point3D(x, y, z + 0.01);//--- avoid z fighting with the ground
            }
            else if (index == 2)//--- balloon
            {
                obj.ScaleZ = obj.ScaleX * 1.2;
            }
            else if (index == 4 || index == 5)//--- cylinder
            {
                obj.DiffuseMaterial.Brush = brush2;
                obj.Rotation1 = new Quaternion(Math3D.UnitZ, angle1);
            }
            return obj;
        }
        int count;

        class Balloon : Sphere
        {
            public Balloon(double z)
            {
                DeltaZ = z * 0.01;
            }
            public double DeltaZ;
        }

        void TimerTick(object sender, EventArgs e)
        {
            foreach (var item in scene.Models)
            {
                Balloon balloon = item as Balloon;
                if (balloon != null)
                {
                    Point3D pos = balloon.Position;
                    if ((pos.Z > 10 && balloon.DeltaZ > 0)
                        || (pos.Z < balloon.ScaleZ && balloon.DeltaZ < 0))
                        balloon.DeltaZ *= -1;
                    pos.Z += balloon.DeltaZ;
                    balloon.Position = pos;
                }
                else
                {
                    if (item is Cylinder || item is Tube)
                    {
                        Object3D obj = item as Object3D;
                        Quaternion q = obj.Rotation1;
                        obj.Rotation1 = new Quaternion(Math3D.UnitZ, q.Angle + 0.3);
                    }
                }
            }

            string msg = checker.GetResult();
            DateTime t1 = DateTime.Now;
            if ((t1 - t0).TotalSeconds > 1)
            {
                t0 = t1;
                //Title = string.Format("WFTools3D Demo ({0})", msg);
            }
        }
        DateTime t0;
        PerformanceChecker checker = new PerformanceChecker();

        System.Windows.Media.Brush GetRandomBrush()
        {
            byte[] b = new byte[4];
            randy.NextBytes(b);
            System.Windows.Media.Color c1 = System.Windows.Media.Color.FromRgb(b[0], b[1], b[2]);
            randy.NextBytes(b);
            System.Windows.Media.Color c2 = System.Windows.Media.Color.FromRgb(b[0], b[1], b[2]);
            System.Windows.Media.Brush brush = new LinearGradientBrush(c1, c2, 90);
            brush.Freeze();
            return brush;
        }

        void InitializeCameras()
        {
            scene.ActivateCamera(2);
            scene.Camera.Position = new Point3D(-67, 19, 1);
            scene.Camera.LookDirection = new Vector3D(1, -0.24, 0);
            scene.Camera.UpDirection = Math3D.UnitZ;
            scene.Camera.FieldOfView = 60;
            scene.Camera.Speed = 15;

            scene.ActivateCamera(1);
            scene.Camera.Position = new Point3D(-8, 12, 2);
            scene.Camera.LookDirection = -Math3D.UnitY;
            scene.Camera.UpDirection = Math3D.UnitZ;
            scene.Camera.ChangeRoll(15);
            scene.Camera.Speed = 15;

            scene.ActivateCamera(0);
            scene.Camera.Position = new Point3D(2000, 2000, 2000);
            scene.Camera.FieldOfView = 60;
            //scene.Camera.LookAtOrigin();
            double scalar = 0.2;
            scene.Camera.Position = new Point3D(scalar*2048, scalar * imheight, scalar * 2048);
            scene.Camera.LookDirection = new Vector3D(-2048, -imheight , -2048);
            scene.Camera.UpDirection = Math3D.UnitY;
            scene.Camera.FieldOfView = 60;
            scene.Camera.Speed = 0;
        }

        /*void InitializeInfo()
        {
            AddInfo("1, 2, 3:", "Activate camera 1, 2 or 3");
            AddInfo("W, S:", "Increase/decrease speed");
            AddInfo("X:", "Set speed to 0");
            AddInfo("T:", "Turn backwards");
            AddInfo("Space:", "Turn to origin");
            AddInfo("Wheel:", "Increase/decrease field of view");
            AddInfo("H:", "Toggle airplanes and ADI");
            AddInfo();
            AddInfo("SPEED <> 0", "――――――――――――――");
            AddInfo("LMB,");
            AddInfo("Arrows:", "Change pitch and roll angles");
            AddInfo("Ctrl+LMB", "Change look direction");
            AddInfo("A, D:", "Fly standard turn left/right");
            AddInfo("F:", "Fly parallel to the ground");
            AddInfo();
            AddInfo("SPEED == 0", "――――――――――――――");
            AddInfo("LMB:", "Rotate scene about origin");
            AddInfo("Ctrl+LMB:", "Rotate scene about touchpoint");
            AddInfo("Arrows:", "Change look direction");
            AddInfo("PgUp, PgDn:", "Change roll angle");
            AddInfo("Ctrl+Arrows:", "Move camera left/right");
            AddInfo("", "and forward/backward");
            AddInfo("Ctrl+PgUp:", "Move camera up");
            AddInfo("Ctrl+PgDn:", "Move camera down");
            AddInfo("Shift:", "Increase all above motion steps");
        }*/
        /*
        void AddInfo(string s1 = null, string s2 = null)
        {
            int row = info.RowDefinitions.Count;
            info.RowDefinitions.Add(new RowDefinition());
            AddInfo(row, 0, s1);
            if (s2 != null)
                AddInfo(row, 1, s2);
        }
        */
        void AddInfo(int row, int col, string text)
        {
            TextBlock tb = new TextBlock { Text = text, Padding = new Thickness(4, 0, 4, 0) };
            Grid.SetRow(tb, row);
            Grid.SetColumn(tb, col);
            //info.Children.Add(tb);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            //if (e.Key == Key.Escape) ;
               // Close();
        }

        private void scene_MouseDown(object sender, MouseButtonEventArgs e)
        {
            int X = (int)(scene.touchPoint.X * 10);
            int Y = (int)(scene.touchPoint.Y * 10);
            int Z = (int)(scene.touchPoint.Z * 10 / 4);
            this.tb_3DCoord.Text = $"({X},{Y},{Z})";
            if (IsValid(ref X,ref Y,ref Z))
            {
                App.global_Win_SpecImg.Refresh(Z, 1);
                
            }
            
            
        }

        private bool IsValid(ref int x, ref int y, ref int z)
        {
            if (x < 0 || x > 2048) return false;
            if (y < 0 || y > imheight) return false;
            if (z < 0 || z > 160) return false;
            if (x==2048) x = 2047;
            if (y == imheight) y = (int)imheight-1;
            if (z == 160) z = 159;
            return true;
        }
    }
}
