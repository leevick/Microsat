﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using Microsat.Shared;
using System.IO;
using Microsat.DB;
using System.Data.SQLite;
using System.Data;
//using FreeImageAPI;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Windows.Threading;
using System.Drawing.Imaging;
using System.Windows;
using Microsat.SpecProc;
using System.Windows.Media.Imaging;
using FreeImageAPI;

namespace Microsat.BackgroundTasks
{
    public class DataProc
    {
        #region File Operations
        public static Task<string> Import_3(IProgress<double> Prog, IProgress<string> List, CancellationToken cancel)
        {
            return Task.Run(()=> 
            {

                try
                {


                    SQLiteConnection conn = new SQLiteConnection(Variables.dbConString);
                    conn.Open();
                    SQLiteCommand cmd = new SQLiteCommand("INSERT INTO Import_History (FileName)VALUES(\"" + Variables.str_FilePath + "\")", conn);
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = "SELECT ID from Import_History ORDER BY id DESC";
                    long import_id = (long)(cmd.ExecuteScalar());
                    FileStream fs_input = new FileStream(Variables.str_FilePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                    long total_rows = fs_input.Length / 288;

                List<UInt16> list_frame = new List<UInt16>();

                FileStream fs_temp = new FileStream(Variables.str_FilePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                byte[] full_file = new byte[fs_temp.Length];
                fs_temp.Read(full_file, 0, (int)fs_temp.Length);
                SQLiteDatabase sqlExcute = new SQLiteDatabase(Variables.dbPath);
                    sqlExcute.BeginInsert();
                    Parallel.For(0, total_rows, i =>
                       {

                           if (full_file[i * 288 + 4] == 0x08 && full_file[i * 288 + 5] == 0x01)
                           {
                               byte[] buf_row = new byte[10];
                               Array.Copy(full_file, i * 288, buf_row, 0, 10);
                               AuxDataRow row = new AuxDataRow(buf_row, import_id);
                               row.Insert(sqlExcute);
                               Prog.Report((double)i / total_rows);
                               list_frame.Add(row.FrameCount);
                               Parallel.For(1, 5, j =>
                               {
                                   FileStream fs_o = new FileStream($"{Variables.str_pathWork}\\{import_id}_{row.FrameCount}_{j}.jp2",FileMode.Create);
                                   fs_o.Close();
                               });
                           }

                           if (full_file[i * 288 + 4] == 0x0A)
                           {
                               byte[] buf_row = new byte[10];
                               Array.Copy(full_file, i * 288, buf_row, 0, 10);
                               ROW row = new ROW(buf_row, import_id);
                               row.InsertTemp(sqlExcute, i);
                           }
                       });
                sqlExcute.EndInsert();
                fs_temp.Close();


                FileStream fs_split = new FileStream(Variables.str_FilePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                byte[] file_full = new byte[fs_split.Length];
                    fs_split.Read(file_full, 0, (int)fs_split.Length);
                double counter = 0;

                    Parallel.For(0,list_frame.Count, j =>
                     {
                         Parallel.For(1,5,k=>
                         {

                             DataTable dt = sqlExcute.GetDataTable($"SELECT row_addr FROM temp WHERE import_id={import_id} AND frame_id={list_frame[j]} AND chanel={k} ORDER BY pkg_id ASC");

                             FileStream fs = new FileStream($"{Variables.str_pathWork}\\{import_id}_{list_frame[j]}_{k}.jp2", FileMode.Append, FileAccess.Write, FileShare.Write);
                             for(int i=0;i<dt.Rows.Count;i++)
                               {
                                   byte[] buf_split = new byte[288];
                                   long fm = (long)(dt.Rows[i].ItemArray[0]);
                                   Array.Copy(file_full, 288 * fm, buf_split, 0, 288);
                                   RealDataRow row = new RealDataRow(buf_split, import_id);
                                   row.Insert(fs);
                               };
                             fs.Close();

                         if (File.Exists($"{Variables.str_pathWork}\\{import_id}_{list_frame[j]}_{k}.jp2"))
                             {
                                 try
                                 {

                                     FIBITMAP fibmp = FreeImage.LoadEx($"{Variables.str_pathWork}\\{import_id}_{list_frame[j]}_{k}.jp2");
                                     if (fibmp != null)
                                     {
                                         byte[] buf_JP2 = new byte[512 * 160 * 2];
                                         Marshal.Copy(FreeImage.GetBits(fibmp), buf_JP2, 0, 512 * 160 * 2);
                                         FreeImage.Unload(fibmp);
                                         FileStream fs_out = new FileStream($"{Variables.str_pathWork}\\{import_id}_{list_frame[j]}_{k}.raw", FileMode.Create);
                                         fs_out.Write(buf_JP2, 0, 512 * 160 * 2);
                                         fs_out.Close();
                                     }


                                 }
                                 catch {
                                 }


                            }

                             /*

                             fs_split.Read(file_full, 0, (int)fs_split.Length);
                             for (int i = 0; i < total_rows; i++)
                             {
                                 if (file_full[288 * i + 4] == 0x0A && (((UInt16)file_full[288 * i + 6]) << 8 | ((UInt16)file_full[288 * i + 7])) == j)
                                 {
                                     byte[] buf_split = new byte[288];
                                     Array.Copy(file_full, 288 * i, buf_split, 0, 288);
                                         RealDataRow row = new RealDataRow(buf_split, import_id);
                                         row.Insert();
                                 }
                                 else continue;
                             }

                             */

                             /*
                             Parallel.For(1, 5, i => {
                                 ThreadPool.QueueUserWorkItem(o=> {

                                     FIBITMAP fibmp = FreeImage.LoadEx($"{Variables.str_pathWork}\\{import_id}_{j}_{i}.jp2");
                                     if (fibmp != null)
                                     {
                                         byte[] buf_JP2 = new byte[512 * 160 * 2];
                                         Marshal.Copy(FreeImage.GetBits(fibmp), buf_JP2, 0, 512 * 160 * 2);
                                         FreeImage.Unload(fibmp);
                                         FileStream fs_out = new FileStream($"{Variables.str_pathWork}\\{import_id}_{j}_{i}.raw", FileMode.Create);
                                         fs_out.Write(buf_JP2, 0, 512 * 160 * 2);
                                         fs_out.Close();
                                     }
                                 });
                             });
                             */

                         });
                    counter++; if (counter > list_frame.Count) counter = list_frame.Count; Prog.Report(counter/list_frame.Count);
                });
                fs_split.Close();


                    sqlExcute.ExecuteNonQuery("DELETE FROM temp WHERE 1=1");

                }
               catch (Exception e)
               {
                   return e.Message;
               }
                return "Decode Normally!";

            });
        }

       
#endregion

#region Bitmap Operations
        public static Task<Bitmap> GetBmp(int v, IProgress<double> progress_Prog)
        {
            return Task.Run(() =>
            {
                Bitmap bmpTop = new Bitmap(2048, DataQuery.QueryResult.Rows.Count, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                BitmapData bmpData = bmpTop.LockBits(new System.Drawing.Rectangle(0, 0, 2048, DataQuery.QueryResult.Rows.Count), System.Drawing.Imaging.ImageLockMode.WriteOnly, bmpTop.PixelFormat);
                byte[] buf_full = new byte[2048 * DataQuery.QueryResult.Rows.Count * 3];
                Parallel.For(0, DataQuery.QueryResult.Rows.Count, (i) => {
                    progress_Prog.Report((double)i/DataQuery.QueryResult.Rows.Count);
                    byte[] buf_rgb = new byte[2048 * 3];
                    Parallel.For(1, 5, k => {
                        FileStream fs = new FileStream($"{Variables.str_pathWork}\\{(long)(DataQuery.QueryResult.Rows[i].ItemArray[14])}_{(long)(DataQuery.QueryResult.Rows[i].ItemArray[0])}_{k}.raw", FileMode.Open, FileAccess.Read, FileShare.Read);
                        fs.Seek(v * 512 * 2, SeekOrigin.Begin);
                        byte[] temp = new byte[512 * 2];
                        fs.Read(temp, 0, 1024);
                        Parallel.For(0, 512, l =>
                        {
                            Spectra2RGB.HsvToRgb(300 * ((double)v / 160), 1, ((double)(readU16(temp, l * 2)) / 65536), out buf_rgb[512 * 3 * (k - 1) + 3 * l + 2], out buf_rgb[512 * 3 * (k - 1) + 3 * l + 1], out buf_rgb[512 * 3 * (k - 1) + 3 * l + 0]);
                            // buf_rgb[512 * 4 * (k - 1) + 4 * l + 3] = 255;

                        });
                        fs.Close();
                    });
                    Array.Copy(buf_rgb, 0, buf_full, 3 * 2048 * i, 3 * 2048);
                });
                Marshal.Copy(buf_full, 0, bmpData.Scan0, 2048 * DataQuery.QueryResult.Rows.Count * 3);
                bmpTop.UnlockBits(bmpData);
                return bmpTop;
            });
        }

        public static Task<Bitmap> GetBmp(int v)
        {
            return Task.Run(()=> 
            {
                Bitmap bmpTop = new Bitmap(2048, DataQuery.QueryResult.Rows.Count, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                BitmapData bmpData = bmpTop.LockBits(new System.Drawing.Rectangle(0, 0, 2048, DataQuery.QueryResult.Rows.Count), System.Drawing.Imaging.ImageLockMode.WriteOnly, bmpTop.PixelFormat);
                byte[] buf_full = new byte[2048 * DataQuery.QueryResult.Rows.Count * 3];
                Parallel.For(0, DataQuery.QueryResult.Rows.Count, (i) => {
                    byte[] buf_rgb = new byte[2048 * 3];
                    Parallel.For(1, 5, k => {
                        FileStream fs = new FileStream($"{Variables.str_pathWork}\\{(long)(DataQuery.QueryResult.Rows[i].ItemArray[14])}_{(long)(DataQuery.QueryResult.Rows[i].ItemArray[0])}_{k}.raw", FileMode.Open, FileAccess.Read, FileShare.Read);
                        fs.Seek(v * 512 * 2, SeekOrigin.Begin);
                        byte[] temp = new byte[512 * 2];
                        fs.Read(temp, 0, 1024);
                        Parallel.For(0, 512, l =>
                        {
                            Spectra2RGB.HsvToRgb(300 * ((double)v / 160), 1, ((double)(readU16(temp,l*2)) /65536), out buf_rgb[512 * 3 * (k - 1) + 3 * l+2], out buf_rgb[512 * 3 * (k - 1) + 3 * l + 1], out buf_rgb[512 * 3 * (k - 1) + 3 * l + 0]);
                           // buf_rgb[512 * 4 * (k - 1) + 4 * l + 3] = 255;
                            
                        });
                        fs.Close();
                    });
                    Array.Copy(buf_rgb, 0, buf_full, 3 * 2048 * i, 3 * 2048);
                });
                Marshal.Copy(buf_full, 0, bmpData.Scan0, 2048 * DataQuery.QueryResult.Rows.Count * 3);
                bmpTop.UnlockBits(bmpData);
                return bmpTop;
            });
        }
        public static Task<Bitmap[]> GetBmp3D()
        {
            return Task.Run(async () =>
            {

                Bitmap[] r = new Bitmap[6];

                r[0] = await GetBmp(159);
                r[1] = await GetBmp(0);
                Thread _tUp = new Thread(new ThreadStart(() => {
                    Bitmap bmpUp = new Bitmap(2048, 160, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                    BitmapData bmpData = bmpUp.LockBits(new System.Drawing.Rectangle(0, 0, 2048, 160), System.Drawing.Imaging.ImageLockMode.WriteOnly, bmpUp.PixelFormat);
                    byte[] buf_full = new byte[2048 * 160 * 3];
                    
                    Parallel.For(0, 4, k =>
                    {
                        byte[] buf_file = new byte[512 * 160 * 2];
                        FileStream fs = new FileStream($"{Variables.str_pathWork}\\{(long)(DataQuery.QueryResult.Rows[0].ItemArray[14])}_{(long)(DataQuery.QueryResult.Rows[0].ItemArray[0])}_{k+1}.raw", FileMode.Open, FileAccess.Read, FileShare.Read);
                        fs.Read(buf_file, 0, 512 * 160 * 2);
                        Parallel.For(0, 160, i => {
                            Parallel.For(0, 512, j =>
                            {
                                Spectra2RGB.HsvToRgb((double)i/160*300, (double)(readU16(buf_file,1024 * i + 2 * j)) / 65536,1,out buf_full[6144*i+1536*k+3*j+2], out buf_full[6144 * i + 1536 * k + 3 * j+1], out buf_full[6144 * i + 1536 * k + 3 * j]);
                            });
                        });
                    });
                    Marshal.Copy(buf_full, 0, bmpData.Scan0, 2048 *160 * 3);
                    bmpUp.UnlockBits(bmpData);
                    MemoryStream ms = new MemoryStream();
                    bmpUp.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
                    r[2] = bmpUp;
                }));
                Thread _tDown = new Thread(new ThreadStart(() => {
                    Bitmap bmpDown = new Bitmap(2048, 160, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                    BitmapData bmpData = bmpDown.LockBits(new System.Drawing.Rectangle(0, 0, 2048, 160), System.Drawing.Imaging.ImageLockMode.WriteOnly, bmpDown.PixelFormat);
                    byte[] buf_full = new byte[2048 * 160 * 3];

                    Parallel.For(0, 4, k =>
                    {
                        byte[] buf_file = new byte[512 * 160 * 2];
                        FileStream fs = new FileStream($"{Variables.str_pathWork}\\{(long)(DataQuery.QueryResult.Rows[DataQuery.QueryResult.Rows.Count-1].ItemArray[14])}_{(long)(DataQuery.QueryResult.Rows[DataQuery.QueryResult.Rows.Count - 1].ItemArray[0])}_{k + 1}.raw", FileMode.Open, FileAccess.Read, FileShare.Read);
                        fs.Read(buf_file, 0, 512 * 160 * 2);
                        Parallel.For(0, 160, i => {
                            Parallel.For(0, 512, j =>
                            {
                                Spectra2RGB.HsvToRgb((double)i / 160 * 300, (double)(readU16(buf_file, 1024 * i + 2 * j)) / 65536, 1, out buf_full[6144 * i + 1536 * k + 3 * j + 2], out buf_full[6144 * i + 1536 * k + 3 * j + 1], out buf_full[6144 * i + 1536 * k + 3 * j]);
                            });
                        });
                    });
                    Marshal.Copy(buf_full, 0, bmpData.Scan0, 2048 * 160 * 3);
                    bmpDown.UnlockBits(bmpData);
                    MemoryStream ms = new MemoryStream();
                    bmpDown.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
                    r[3] = bmpDown;


                }));
                Thread _tRight = new Thread(new ThreadStart(() => {
                    Bitmap bmpTop = new Bitmap(DataQuery.QueryResult.Rows.Count, 160);
                    BitmapData bmpData = bmpTop.LockBits(new System.Drawing.Rectangle(0, 0, DataQuery.QueryResult.Rows.Count, 160), System.Drawing.Imaging.ImageLockMode.WriteOnly, bmpTop.PixelFormat);
                    byte[] buf_full = new byte[160 * DataQuery.QueryResult.Rows.Count * 4];
                    Parallel.For(0, DataQuery.QueryResult.Rows.Count, (i) => {
                        FileStream fs = new FileStream($"{Variables.str_pathWork}\\{(long)(DataQuery.QueryResult.Rows[i].ItemArray[14])}_{(long)(DataQuery.QueryResult.Rows[DataQuery.QueryResult.Rows.Count-1-i].ItemArray[0])}_4.raw", FileMode.Open, FileAccess.Read, FileShare.Read);
                        byte[] buf_temp = new byte[512 * 160 * 2];
                        fs.Read(buf_temp, 0, 512 * 160 * 2);
                        Parallel.For(0, 160, j => {

                            Spectra2RGB.HsvToRgb((double)(j) / 160 * 300, (double)buf_temp[j*512*2+2*511] / 255, 1, out buf_full[(j) * DataQuery.QueryResult.Rows.Count * 4 + i * 4 +2], out buf_full[j * DataQuery.QueryResult.Rows.Count * 4 + i * 4 + 1], out buf_full[j * DataQuery.QueryResult.Rows.Count * 4 + i * 4 ]);
                            buf_full[j * DataQuery.QueryResult.Rows.Count * 4 + i * 4 + 3] = 255;

                        });
                    });
                    Marshal.Copy(buf_full, 0, bmpData.Scan0, 160 * DataQuery.QueryResult.Rows.Count * 4);
                    bmpTop.UnlockBits(bmpData);

                    r[4] = bmpTop;

                }));
                Thread _tLeft = new Thread(new ThreadStart(() => {
                    Bitmap bmpTop = new Bitmap(DataQuery.QueryResult.Rows.Count, 160);
                    BitmapData bmpData = bmpTop.LockBits(new System.Drawing.Rectangle(0, 0, DataQuery.QueryResult.Rows.Count, 160), System.Drawing.Imaging.ImageLockMode.WriteOnly, bmpTop.PixelFormat);
                    byte[] buf_full = new byte[160 * DataQuery.QueryResult.Rows.Count * 4];
                    Parallel.For(0, DataQuery.QueryResult.Rows.Count, (i) => {
                        FileStream fs = new FileStream($"{Variables.str_pathWork}\\{(long)(DataQuery.QueryResult.Rows[i].ItemArray[14])}_{(long)(DataQuery.QueryResult.Rows[i].ItemArray[0])}_1.raw", FileMode.Open, FileAccess.Read, FileShare.Read);
                        byte[] buf_temp = new byte[512 * 160 * 2];
                        fs.Read(buf_temp, 0, 512 * 160 * 2);
                        Parallel.For(0, 160, j => {

                            Spectra2RGB.HsvToRgb((double)j / 160 * 300, (double)buf_temp[j * 512 * 2 + 0] / 255, 1, out buf_full[j * DataQuery.QueryResult.Rows.Count * 4 + i * 4+2], out buf_full[j * DataQuery.QueryResult.Rows.Count * 4 + i * 4 + 1], out buf_full[j * DataQuery.QueryResult.Rows.Count * 4 + i * 4]);
                            buf_full[j * DataQuery.QueryResult.Rows.Count * 4 + i * 4 + 3] = 255;

                        });
                    });
                    Marshal.Copy(buf_full, 0, bmpData.Scan0, 160 * DataQuery.QueryResult.Rows.Count * 4);
                    bmpTop.UnlockBits(bmpData);

                    r[5] = bmpTop;


                }));


                _tUp.Start();
                _tUp.Join();
                _tDown.Start();
                _tDown.Join();
                _tRight.Start();
                _tRight.Join();
                _tLeft.Start();
                _tLeft.Join();
                return r;
                
            });
        }

#endregion

#region Spectrum Curves
        public static Task<double[,]> GetChart()
        {
            return Task.Run(()=> {
                double[,] result = new double[160, 2];



                return result;
            });
        }
#endregion

#region database operations
        public static Task<DataTable> QueryResult(bool isChecked1, bool isChecked2, bool isChecked3, DateTime start_time, DateTime end_time, long start_FrmCnt, long end_FrmCnt, Coord coord_TL, Coord coord_DR)
        {

            return Task.Run(() =>
            {
                SQLiteConnection conn = new SQLiteConnection(Variables.dbConString);
                conn.Open();
                SQLiteCommand cmmd = new SQLiteCommand("", conn);
                cmmd.CommandText = "SELECT ID from Import_History ORDER BY id DESC";
                long import_id = (long)(cmmd.ExecuteScalar());

                string command = $"SELECT * FROM AuxData WHERE Chanel=1 AND ImportId={import_id}";
                if ((bool)isChecked2)
                {
                    command += " AND Lat>" + (coord_DR.Lat.ToString()) + " AND Lat<" + coord_TL.Lat.ToString() + " AND Lon>" + (coord_TL.Lon.ToString()) + " AND Lon<" + coord_DR.Lon.ToString();
                }
                if ((bool)isChecked1)
                {

                    DateTime selectedStartDate = start_time;
                    DateTime selectedEndDate = end_time;
                    DateTime T0 = new DateTime(1970, 1, 1, 0, 0, 0);
                    TimeSpan ts_Start = selectedStartDate.Subtract(T0);
                    TimeSpan ts_End = selectedEndDate.Subtract(T0);

                    command += " AND GST>" + (ts_Start.TotalSeconds.ToString()) + " AND GST<" + ts_End.TotalSeconds.ToString();

                }
                if ((bool)isChecked3)
                {
                    command += " AND FrameId>" + start_FrmCnt.ToString() + " AND FrameId<" + end_FrmCnt.ToString();
                }

                //command = command.Substring(0, command.LastIndexOf("AND"));
                SQLiteDatabase db = new SQLiteDatabase(Variables.dbPath);
                return db.GetDataTable(command);


            });
        }
#endregion

#region 处理
        public static UInt32 readU32(byte[] buf_row, int addr)
        {
            byte[] conv = new byte[4] { buf_row[addr + 3], buf_row[addr + 2], buf_row[addr + 1], buf_row[addr] };
            return BitConverter.ToUInt32(conv, 0);
        }
        public static Int32 readI32(byte[] buf_row, int addr)
        {
            byte[] conv = new byte[4] { buf_row[addr + 3], buf_row[addr + 2], buf_row[addr + 1], buf_row[addr] };
            return BitConverter.ToInt32(conv, 0);
        }
        public static UInt16 readU16(byte[] buf_row, int addr)
        {
            byte[] conv = new byte[2] { buf_row[addr + 1], buf_row[addr] };
            return BitConverter.ToUInt16(conv, 0);
        }
        public static byte readU8(byte[] buf_row, int addr)
        {
            return buf_row[addr];
        }
        public static float readF32(byte[] buf_row, int addr)
        {
            byte[] conv = new byte[4] { buf_row[addr + 3], buf_row[addr + 2], buf_row[addr + 1], buf_row[addr] };
            int a = BitConverter.ToInt32(conv, 0);
            float result = (float)a / 1000;
            return result;
        }
        public static float readLength(byte[] buf_row, int addr)
        {
            float result = (float)readI32(buf_row, addr) / 1000;
            return result;
        }
        public static float readDegree(byte[] buf_row, int addr)
        {
            float result = (float)((float)readI32(buf_row, addr) / 1000 * 180 / Math.PI);
            return result;
        }

        internal static double[] GetWaveLen(int col)
        {
            double[] result = new double[160];
            SQLiteDatabase db = new SQLiteDatabase(Variables.dbPath);
            DataTable dt = db.GetDataTable("SELECT * FROM SpectrumMap WHERE SpaN="+(col+1).ToString());
            Array.Copy(dt.Rows[0].ItemArray, 1, result, 0, 160);
            return result;
        }
#endregion
    }

#region FRAME class

    public class ROW
    {
        public UInt16 FrameCount;
        public UInt16 PackCount;
        public byte Chanel;
        public long ImportId = 0;
        public byte[] buf_Row;
        public ROW(byte[] ROW, long id)
        {
            FrameCount = DataProc.readU16(ROW, 6);
            PackCount = DataProc.readU16(ROW, 8);
            Chanel = DataProc.readU8(ROW, 5);
            buf_Row = ROW;
            ImportId = id;

        }

        public void InsertTemp(SQLiteDatabase sqlExcute,long row_addr)
        {
            var sql = "insert into temp values(@import_id,@frame_id,@pkg_id,@chanel,@row_addr);";
            var cmdparams = new List<SQLiteParameter>()
                {
                    new SQLiteParameter("import_id", ImportId),
                    new SQLiteParameter("frame_id",FrameCount),
                    new SQLiteParameter("pkg_id",PackCount),
                    new SQLiteParameter("chanel",Chanel),
                    new SQLiteParameter("row_addr",row_addr),
                };
            try
            {
                sqlExcute.ExecuteNonQuery(sql, cmdparams);
            }
            catch (Exception e)
            {
                //Do any logging operation here if necessary
                throw e;
            }
        }
    }

    public class RealDataRow : ROW
    {
        public bool isHead = false;
        public bool isTail = false;
        public RealDataRow(byte[] ROW, long id) : base(ROW, id)
        {

            if ((UInt32)(ROW[32] << 24 | ROW[33] << 16 | ROW[34] << 8 | ROW[35]) == 0xFF4FFF51)
            {
                isHead = true;
            }


        }
        public void Insert(FileStream fs)
        {
            if (isHead) fs.Write(buf_Row, 32, 240);
            else fs.Write(buf_Row, 16, 256);
        }
    }

    public class AuxDataRow : ROW
    {
        protected double GST;
        protected double Lat;
        protected double Lon;
        protected double X;
        protected double Y;
        protected double Z;
        protected double Vx;
        protected double Vy;
        protected double Vz;
        protected double Ox;
        protected double Oy;
        protected double Oz;
        public AuxDataRow(byte[] ROW, long id) : base(ROW, id)
        {

            X = 7000 * Math.Cos(Math.PI * ((double)(FrameCount % 360) / 180));//readLength(32);
            Y = 7000 * Math.Sin(Math.PI * ((double)(FrameCount % 360) / 180));//readLength(36);
            Z = 0;//readLength(40);
            Vx = 7.546 * Math.Cos(Math.PI * ((double)(FrameCount % 360) / 180) + Math.PI / 2);//readLength(44);
            Vy = 7.546 * Math.Sin(Math.PI * ((double)(FrameCount % 360) / 180) + Math.PI / 2);//readLength();
            Vz = 0;//0;//
            GST = DateTime.Now.Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds;//OrbitCalc.CalGST(new POSE_TIME(DateTime.Now));
            double[] latlon = new double[2];
            latlon = OrbitCalc.CalEarthLonLat(new double[3] { X, Y, Z }, GST);
            Lat = latlon[1] * 180 / Math.PI;
            Lon = latlon[0] * 180 / Math.PI;
            Ox = 0;
            Oy = 0;
            Oz = 0;
        }

        public void Insert(SQLiteDatabase sqlExcute)
        {
            var sql = "insert into AuxData values(@FrameId,@SatelliteId,@GST,@Lat,@Lon,@X,@Y,@Z,@Vx,@Vy,@Vz,@Ox,@Oy,@Oz,@ImportId,@Chanel);";
            var cmdparams = new List<SQLiteParameter>()
                {
                    new SQLiteParameter("FrameId", FrameCount),
                    new SQLiteParameter("SatelliteId","MicroSat"),
                    new SQLiteParameter("GST",GST),
                    new SQLiteParameter("Lat",Lat),
                    new SQLiteParameter("Lon",Lon),
                    new SQLiteParameter("X",X),
                    new SQLiteParameter("Y",Y),
                    new SQLiteParameter("Z",Z),
                    new SQLiteParameter("Vx",Vx),
                    new SQLiteParameter("Vy",Vy),
                    new SQLiteParameter("Vz",Vz),
                    new SQLiteParameter("Ox",Ox),
                    new SQLiteParameter("Oy",Oy),
                    new SQLiteParameter("Oz",Oz),
                    new SQLiteParameter("ImportId",ImportId),
                    new SQLiteParameter("Chanel",Chanel)
                };

            try
            {
                
                sqlExcute.ExecuteNonQuery(sql, cmdparams);
            }
            catch (Exception e)
            {
                //Do any logging operation here if necessary
                throw e;
            }
        }
    }
    public class FRAME
    {
        public List<RealDataRow>[] ItemList;
        
        public ushort FrmCnt;

        public FRAME()
        {
            ItemList = new List<RealDataRow>[4];
            FrmCnt = 0;
        }

        public override bool Equals(object obj)
        {
            if (obj is FRAME)
            {
                FRAME f = obj as FRAME;
                if (f.FrmCnt == FrmCnt) return true;
                else return false;

            }

            return false;
        }

        public FRAME(ushort frameCount)
        {
            this.FrmCnt = frameCount;
            ItemList = new List<RealDataRow>[4];
           
        }

        public void InsertRow(RealDataRow row)
        {
            ItemList[row.Chanel-1].Add(row);
            FrmCnt = row.FrameCount;
            //ItemList.Sort(byFrmCnt);
        }

        private int byFrmCnt(RealDataRow T1,RealDataRow T2)
        {
            if (T1.PackCount > T2.PackCount) return 1;
            else return -1;
        }

        public void WriteFile()
        {
      
                for (int i = 0; i < 4; i++)
                {

                ThreadPool.QueueUserWorkItem(o => { 
                    FileStream fs = new FileStream(Variables.str_pathWork + "\\" + ItemList[0][0].ImportId.ToString() + "_" + this.FrmCnt.ToString() + "_" + (i + 1).ToString() + ".jp2", FileMode.Append, FileAccess.Write, FileShare.Write);

                    foreach (RealDataRow rdr in ItemList[i])
                    {
                        if (rdr.isHead) fs.Write(rdr.buf_Row, 32, 240);
                        else fs.Write(rdr.buf_Row, 16, 256);
                    }

                    fs.Close();

                });
                }
        
            
            }


            

        }

#endregion

#region Spectrum Operations
    public class SpecProc
    {

        public static Task<System.Windows.Point[]> GetSpecCurv(System.Windows.Point p)
        {
            return Task.Run(()=>
            {
                long importId = (long)DataQuery.QueryResult.Rows[0].ItemArray[14];
                long frmCnt_Start = (long)DataQuery.QueryResult.Rows[0].ItemArray[0];
                long frmCnt_End = (long)DataQuery.QueryResult.Rows[DataQuery.QueryResult.Rows.Count-1].ItemArray[0];
                System.Windows.Point[] result = new System.Windows.Point[149];
                FileStream fs = new FileStream($"{Variables.str_pathWork}\\{importId}_{(int)(frmCnt_Start + p.Y * (frmCnt_End - frmCnt_Start))}_{(int)(p.X*2048)/512+1}.raw", FileMode.Open, FileAccess.Read, FileShare.Read);
                byte[] buf = new byte[512 * 160 * 2];
                fs.Read(buf, 0, 512 * 160 * 2);
                int col = (int)(p.X*2048)%512;
                double[] spec_nm = DataProc.GetWaveLen(col);

                Parallel.For(5, 154, i =>
                {
                    result[i-5] = new System.Windows.Point(spec_nm[i], DataProc.readU16(buf, i * 1024 + 2 * col));

                });


                return result;

            });

        }
    }

#endregion

#region Orbit Calculation Class
    public class OrbitCalc
    {


        const int POSE_GST0TIME = 63417600;
        const float POSE_GST0 = 5.986782214F;
        const float POSE_WE = 7.29211514667e-5F;
        const float POSE_FZERO = 0.0000001F;
        public OrbitCalc()
        { }
        public static double[] CalEarthLonLat(double[] cuR, double fgst)
        {
            double[] clonlat = new double[2];
            double temp, sra, lon;
            temp = Math.Sqrt(cuR[0] * cuR[0] + cuR[1] * cuR[1]);             //|R|
            if (Math.Abs(temp) < POSE_FZERO)                             //R==0
            {
                clonlat[1] = Math.PI * 0.5 * cuR[2] / Math.Abs(cuR[2]);    //Recs[2]>0.0,则fLati = PI05
                sra = 0.0F;
            }
            else
            {
                clonlat[1] = Math.Atan(cuR[2] / temp);             //得到纬度[-PI05 PI05]
                sra = Math.Atan2(cuR[1], cuR[0]);                  //得到经度[-PI PI]
            }
            lon = sra - fgst;
            lon = lon - (int)(lon / Math.PI / 2) * Math.PI * 2;                       //POSE_MODF规整到给定范围

            if (lon > Math.PI)
                lon = lon - Math.PI * 2;
            else if (lon < -Math.PI)
                lon = lon + Math.PI * 2;
            else { }
            clonlat[0] = lon;                                       //经度
            return clonlat;
        }

        public static float CalGST(POSE_TIME cTime)
        {
            float lfgst;
            float LfDelt;
            POSE_TIME LgstT = new POSE_TIME();

            LgstT.S = POSE_GST0TIME;    //秒值为宏
            LgstT.M = 0;                //微秒为0

            LfDelt = CalDeltTime(cTime, LgstT);
            lfgst = POSE_GST0 + LfDelt * POSE_WE;
            return lfgst;
        }

        public static float CalDeltTime(POSE_TIME cTime, POSE_TIME cTime1)
        {
            float LfDelt, temp;

            temp = (cTime.M) * 0.001F;
            if (cTime1.S > cTime.S)
            {
                LfDelt = 1.0F * (cTime.S - cTime1.S) - temp;
                LfDelt = -LfDelt;
            }
            else
                LfDelt = 1.0F * (cTime.S - cTime1.S) + temp;
            return LfDelt;
        }

        public class POSE_TIME
        {
            public int S;
            public int M;
            private DateTime now;
            public POSE_TIME()
            {
                S = 0;
                M = 0;
            }
            public POSE_TIME(int a, int b)
            {
                S = a;
                M = b;
            }
            public POSE_TIME(DateTime now)
            {
                this.now = now;
                S = (int)(now.Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds);
                M = (int)((now.Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds - (int)(now.Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds)) * 1000000);
            }
        }
    }

#endregion

}
