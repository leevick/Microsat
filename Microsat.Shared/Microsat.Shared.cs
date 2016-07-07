using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Drawing;
using System.IO;
using System.Configuration;



namespace Microsat.Shared
{
    public static class Variables
    {
        public static string str_FilePath;
        public static List<ScreenParams> Screens=new List<ScreenParams>();

        public struct OperaFile
        {
            public FileStream file;
            public String filePath;         //文件路径
            public String fileOnlyName;     //文件名+后缀
            public String fileAllName;      //文件路径+文件名+后缀
            public Byte[] fileBuf;          //文件缓存
            public long fileLength;         //文件总字节数
        }

        public struct ReportList
        {
            public UInt16[] minFrmID;
            public UInt16[] maxFrmID;
        }
        /*-------------------------------------------------窗体--------------------------------------------------------*/

        public static byte[] bufferImgDisp;




        /*-----------------------------------------------图像常量------------------------------------------------------*/
        public const UInt16 IMG_FULL_WID = 2048;
        public const UInt16 IMG_FULL_HEI = 160;

        /*-----------------------------------------------文件常量------------------------------------------------------*/
        public const UInt16 LEN_ORIG = 1024;                            //原始文件每包包长1024B
        public const UInt16 LEN_COMP = 280;                             //压缩文件每包包长280B
        public const UInt16 LEN_IMG = 256;                              //每帧压缩图像长度
        //public static string pathWork = ConfigurationManager.AppSettings["pathWork"]; //工作目录

        /*---------------------------------------------原始图像显示----------------------------------------------------*/
        public static String pathImgPathShow = "";
        public static Byte moveLeftHigh = 0;					        //高位向左移的位数
        public static Byte moveRightLow = 0;					        //低位向右移的位数

        /*-----------------------------------------------图像解压------------------------------------------------------*/
        public static OperaFile fileOrig = new OperaFile();             //原始文件，即从星务接收的第一手文件
        public static OperaFile fileComp = new OperaFile();             //压缩文件，即从原始文件解包后的压缩文件
        public static OperaFile[] fileCompSplit = new OperaFile[4];     //各通道压缩文件，即从压缩文件分包后的各通道压缩文件
        public static OperaFile fileDecomp = new OperaFile();           //解压后文件，即从压缩文件解压后的文件
        public static string pathDecomp = null;

        public static long curOrigPosiCnt = 0;                          //原始文件读到的位置
        public static long curCompPosiCnt = 0;                         //原始文件读到的位置

        public static long curDecodePosiCnt = 0;                        //当前解压位置
        public static long totalDecodeCnt = 0;                          //待解压文件总数
        public static Thread threadOrigResolve;                         //从原始文件解包成压缩文件的线程
        public static Thread threadCompSplit;                           //从压缩文件分包成帧文件的线程
        public static Thread threadDecode;                              //解压缩线程

        /*-------------------------------------------------数据库------------------------------------------------------*/
        public static DataSet dataReport = new DataSet("数据报表库");               //数据报表库
        public static DataTable dataErrSum = dataReport.Tables.Add("错误汇总");     //错误汇总表
        public static DataTable dataErrDetail = dataReport.Tables.Add("错误细则");  //错误细则表
        public static string dbPath = Environment.CurrentDirectory + "\\db.mdb";     //数据库文件地址
        public static string dbConString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + dbPath;//数据库连接字符串


        public static long cntErrHead = 0;  //帧头错误数
        public static long cntErrTail = 0;  //帧尾错误数
        public static long cntErrSum = 0;   //校验和错误数
        public static string str_pathWork = Environment.CurrentDirectory + "\\Work";
        /*-------------------------------------------------数据库------------------------------------------------------*/
        public static System.Drawing.Rectangle[] Screen_Locations;
        public static DataTable dt = new DataTable();

        public static string MapPath = @"D:\Amap\Amap.html";
    }

    public class ScreenParams
    {
        ScreenType Type;
        public double Width;
        public double Height;
        public double X;
        public double Y;
        public string Name;
        public bool Primary;

        public ScreenParams(Rectangle workingArea, string deviceName, bool primary)
        {
            this.Width = workingArea.Width;
            this.Height = workingArea.Height;
            this.X = workingArea.X;
            this.Y = workingArea.Y;
            this.Name = deviceName;
            this.Primary = primary;
            if (Height > Width) Type = ScreenType.Portrait;
            else Type = ScreenType.Landscape;

        }
    }
    public enum ScreenType {Landscape,Portrait };

    public static class Functions
    {

    }

    public static class Threads
    {

        public static Thread[] _thread= new Thread[8];
     
    }

    public class Coord
    {
        public double Lat;
        public double Lon;

        public Coord(double v1, double v2)
        {
            this.Lat = v1;
            this.Lon = v2;
        }
    }

}
