using Microsat.BackgroundTasks;
using Microsat.Shared;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;

namespace Microsat
{
    /// <summary>
    /// Window_3DCube.xaml 的交互逻辑
    /// </summary>
    public partial class Window_3DCube : Window
    {
        public Window_3DCube()
        {
            InitializeComponent();
        }

        #region 局部变量区
        public bool bool_span = false;
        public System.Windows.Point mouseLastPosition;
        public double mouseDeltaFactor = 5;
        public int ImportId;
        public int FrmCnt_Start;
        public int FrmCnt_End;
        public int SpecSelected;
        public Coord Start;
        public Coord End;
        #endregion

        #region 旋转与缩放
        private void VerticalTransform(bool upDown, double angleDeltaFactor)
        {
            Vector3D postion = new Vector3D(camera.Position.X, camera.Position.Y, camera.Position.Z);
            Vector3D rotateAxis = Vector3D.CrossProduct(postion, camera.UpDirection);
            RotateTransform3D rt3d = new RotateTransform3D();
            AxisAngleRotation3D rotate = new AxisAngleRotation3D(rotateAxis, angleDeltaFactor * (upDown ? -1 : 1));
            rt3d.Rotation = rotate;
            Matrix3D matrix = rt3d.Value;
            Point3D newPostition = matrix.Transform(camera.Position);
            camera.Position = newPostition;
            camera.LookDirection = new Vector3D(-newPostition.X, -newPostition.Y, -newPostition.Z);
            //update the up direction
            Vector3D newUpDirection = Vector3D.CrossProduct(camera.LookDirection, rotateAxis);
            newUpDirection.Normalize();
            camera.UpDirection = newUpDirection;
        }
        private void HorizontalTransform(bool leftRight, double angleDeltaFactor)
        {
            Vector3D postion = new Vector3D(camera.Position.X, camera.Position.Y, camera.Position.Z);
            Vector3D rotateAxis = camera.UpDirection;
            RotateTransform3D rt3d = new RotateTransform3D();
            AxisAngleRotation3D rotate = new AxisAngleRotation3D(rotateAxis, angleDeltaFactor * (leftRight ? -1 : 1));
            rt3d.Rotation = rotate;
            Matrix3D matrix = rt3d.Value;
            Point3D newPostition = matrix.Transform(camera.Position);
            camera.Position = newPostition;
            camera.LookDirection = new Vector3D(-newPostition.X, -newPostition.Y, -newPostition.Z);
        }
        private void Viewport3D_MouseMove(object sender, MouseEventArgs e)
        {
            if (Mouse.LeftButton == MouseButtonState.Pressed)
            {
                System.Windows.Point newMousePosition = e.GetPosition(this);
                if (mouseLastPosition.X != newMousePosition.X)
                {
                    HorizontalTransform(mouseLastPosition.X < newMousePosition.X, mouseDeltaFactor);//水平变换
                }
                if (mouseLastPosition.Y != newMousePosition.Y)// change position in the horizontal direction
                {
                    VerticalTransform(mouseLastPosition.Y > newMousePosition.Y, mouseDeltaFactor);//垂直变换
                }
                mouseLastPosition = newMousePosition;
            }
        }
        private void Viewport3D_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            mouseLastPosition = e.GetPosition(this);
        }
        private void Viewport3D_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            vp3d.MouseLeftButtonDown -= Viewport3D_MouseLeftButtonDown;
            vp3d.MouseMove -= Viewport3D_MouseMove;
            vp3d.MouseWheel -= Viewport3D_MouseWheel;
            double scaleFactor = 3;
            //120 near ,   -120 far
            System.Diagnostics.Debug.WriteLine(e.Delta.ToString());
            Point3D currentPosition = camera.Position;
            Vector3D lookDirection = camera.LookDirection;//new Vector3D(camera.LookDirection.X, camera.LookDirection.Y, camera.LookDirection.Z);
            lookDirection.Normalize();
            lookDirection *= scaleFactor;
            if (e.Delta == 120)//getting near
            {
                if ((currentPosition.X + lookDirection.X) * currentPosition.X > 0)
                {
                    currentPosition += lookDirection;
                }
            }
            if (e.Delta == -120)//getting far
            {
                currentPosition -= lookDirection;
            }
            Point3DAnimation positionAnimation = new Point3DAnimation();
            positionAnimation.BeginTime = new TimeSpan(0, 0, 0);
            positionAnimation.Duration = TimeSpan.FromMilliseconds(100);
            positionAnimation.To = currentPosition;
            positionAnimation.From = camera.Position;
            //positionAnimation.Completed += new EventHandler(positionAnimation_Completed);
            camera.BeginAnimation(PerspectiveCamera.PositionProperty, positionAnimation, HandoffBehavior.Compose);
            vp3d.MouseLeftButtonDown += Viewport3D_MouseLeftButtonDown;
            vp3d.MouseMove += Viewport3D_MouseMove;
            vp3d.MouseWheel += Viewport3D_MouseWheel;
        }
        private void Viewport3D_KeyDown(object sender, KeyEventArgs e)
        { }
        #endregion
        #region 刷新图像
        private void RenderBox(double lines, Bitmap[] bmpArray)
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
                mg3d_Top.Positions.Add(new Point3D(0, 0, 640));
                mg3d_Top.Positions.Add(new Point3D(2048, 0, 640));
                mg3d_Top.Positions.Add(new Point3D(2048, lines, 640));
                mg3d_Top.Positions.Add(new Point3D(0, lines, 640));
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
                mg3d_Left.Positions.Add(new Point3D(0, 0, 640));
                mg3d_Left.Positions.Add(new Point3D(0, lines, 640));
                mg3d_Left.Positions.Add(new Point3D(0, lines, 0));
                mg3d_Left.TriangleIndices.Add(0);
                mg3d_Left.TriangleIndices.Add(1);
                mg3d_Left.TriangleIndices.Add(2);
                mg3d_Left.TriangleIndices.Add(2);
                mg3d_Left.TriangleIndices.Add(3);
                mg3d_Left.TriangleIndices.Add(0);
                mg3d_Left.TextureCoordinates.Add(new System.Windows.Point(0, 1));
                mg3d_Left.TextureCoordinates.Add(new System.Windows.Point(0, 0));
                mg3d_Left.TextureCoordinates.Add(new System.Windows.Point(1, 0));
                mg3d_Left.TextureCoordinates.Add(new System.Windows.Point(1, 1));
                #endregion

                #region 3D立方体右视图
                mg3d_Right.Positions.Add(new Point3D(2048, 0, 0));
                mg3d_Right.Positions.Add(new Point3D(2048, lines, 0));
                mg3d_Right.Positions.Add(new Point3D(2048, lines, 640));
                mg3d_Right.Positions.Add(new Point3D(2048, 0, 640));
                mg3d_Right.TriangleIndices.Add(0);
                mg3d_Right.TriangleIndices.Add(1);
                mg3d_Right.TriangleIndices.Add(2);
                mg3d_Right.TriangleIndices.Add(2);
                mg3d_Right.TriangleIndices.Add(3);
                mg3d_Right.TriangleIndices.Add(0);
                mg3d_Right.TextureCoordinates.Add(new System.Windows.Point(0, 1));
                mg3d_Right.TextureCoordinates.Add(new System.Windows.Point(1, 1));
                mg3d_Right.TextureCoordinates.Add(new System.Windows.Point(1, 0));
                mg3d_Right.TextureCoordinates.Add(new System.Windows.Point(0, 0));
                #endregion

                #region 3D立方体背视图
                mg3d_Bottom.Positions.Add(new Point3D(0, 0, 0));
                mg3d_Bottom.Positions.Add(new Point3D(0, lines, 0));
                mg3d_Bottom.Positions.Add(new Point3D(2048, lines, 0));
                mg3d_Bottom.Positions.Add(new Point3D(2048, 0, 0));
                mg3d_Bottom.TriangleIndices.Add(0);
                mg3d_Bottom.TriangleIndices.Add(1);
                mg3d_Bottom.TriangleIndices.Add(2);
                mg3d_Bottom.TriangleIndices.Add(2);
                mg3d_Bottom.TriangleIndices.Add(3);
                mg3d_Bottom.TriangleIndices.Add(0);
                mg3d_Bottom.TextureCoordinates.Add(new System.Windows.Point(1, 1));
                mg3d_Bottom.TextureCoordinates.Add(new System.Windows.Point(1, 0));
                mg3d_Bottom.TextureCoordinates.Add(new System.Windows.Point(0, 0));
                mg3d_Bottom.TextureCoordinates.Add(new System.Windows.Point(0, 1));
                #endregion

                #region 3D立方体顶视图
                mg3d_Up.Positions.Add(new Point3D(0, lines, 0));
                mg3d_Up.Positions.Add(new Point3D(0, lines, 640));
                mg3d_Up.Positions.Add(new Point3D(2048, lines, 640));
                mg3d_Up.Positions.Add(new Point3D(2048, lines, 0));
                mg3d_Up.TriangleIndices.Add(0);
                mg3d_Up.TriangleIndices.Add(1);
                mg3d_Up.TriangleIndices.Add(2);
                mg3d_Up.TriangleIndices.Add(2);
                mg3d_Up.TriangleIndices.Add(3);
                mg3d_Up.TriangleIndices.Add(0);
                mg3d_Up.TextureCoordinates.Add(new System.Windows.Point(1, 1));
                mg3d_Up.TextureCoordinates.Add(new System.Windows.Point(1, 0));
                mg3d_Up.TextureCoordinates.Add(new System.Windows.Point(0, 0));
                mg3d_Up.TextureCoordinates.Add(new System.Windows.Point(0, 1));
                #endregion

                #region 3D立方体底视图
                mg3d_Down.Positions.Add(new Point3D(0, 0, 0));
                mg3d_Down.Positions.Add(new Point3D(2048, 0, 0));
                mg3d_Down.Positions.Add(new Point3D(2048, 0, 640));
                mg3d_Down.Positions.Add(new Point3D(0, 0, 640));
                mg3d_Down.TriangleIndices.Add(0);
                mg3d_Down.TriangleIndices.Add(1);
                mg3d_Down.TriangleIndices.Add(2);
                mg3d_Down.TriangleIndices.Add(2);
                mg3d_Down.TriangleIndices.Add(3);
                mg3d_Down.TriangleIndices.Add(0);
                mg3d_Down.TextureCoordinates.Add(new System.Windows.Point(1, 1));
                mg3d_Down.TextureCoordinates.Add(new System.Windows.Point(0, 1));
                mg3d_Down.TextureCoordinates.Add(new System.Windows.Point(0, 0));
                mg3d_Down.TextureCoordinates.Add(new System.Windows.Point(1, 0));
                #endregion


            BitmapImage[] bmpSource = new BitmapImage[3];

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

            for(int i=0;i<3;i++)
            {
                bmpArray[i].Save($"cube_{i}.bmp");
                bmpSource[i] = new BitmapImage(new Uri($"{Environment.CurrentDirectory}\\cube_{i}.bmp"));
            };

            DiffuseMaterial dm_Top = new DiffuseMaterial(new System.Windows.Media.ImageBrush(bmpSource[0]));
            DiffuseMaterial dm_Bottom = new DiffuseMaterial(new System.Windows.Media.ImageBrush(bmpSource[1]));
            DiffuseMaterial dm_Up = new DiffuseMaterial(new System.Windows.Media.ImageBrush(bmpSource[2]));
            GeometryModel3D gm3d_Top = new GeometryModel3D(mg3d_Top, dm_Top);
            GeometryModel3D gm3d_Left = new GeometryModel3D(mg3d_Left, dm_Top);
            GeometryModel3D gm3d_Right = new GeometryModel3D(mg3d_Right, dm_Top);
            GeometryModel3D gm3d_Bottom = new GeometryModel3D(mg3d_Bottom, dm_Bottom);
            GeometryModel3D gm3d_Up = new GeometryModel3D(mg3d_Up, dm_Up);
            GeometryModel3D gm3d_Down = new GeometryModel3D(mg3d_Down, dm_Up);

            m3dg.Children.Add(gm3d_Top);
            m3dg.Children.Add(gm3d_Left);
            m3dg.Children.Add(gm3d_Right);
            m3dg.Children.Add(gm3d_Bottom);
            m3dg.Children.Add(gm3d_Up);
            m3dg.Children.Add(gm3d_Down);
            mv3d.Content = m3dg;
            vp3d.Children.Add(mv3d);
        }
        public async void Refresh(DataTable dt_Result)
        {
            Bitmap[] bmp = await DataProc.GetBmp(dt_Result);
            ImportId = (int)(dt_Result.Rows[0].ItemArray[14]);
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
            RenderBox(bmpSource.Height, bmp);
        }
        #endregion
    }
}
