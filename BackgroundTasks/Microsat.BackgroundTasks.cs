using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using Microsat.Shared;
using System.IO;
using Microsat.DB;
using System.Data.OleDb;
using System.Data;
using FreeImageAPI;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Windows.Threading;
using System.Drawing.Imaging;
using System.Windows;
using Microsat.SpecProc;
using System.Windows.Media.Imaging;

namespace Microsat.BackgroundTasks
{
    public class DataProc
    {
        public static Task<string> Import_4(IProgress<double> Prog, IProgress<string> List, CancellationToken cancel)
        {
            return Task.Run(() =>
            {
                try
                {
                    OleDbConnection conn = new OleDbConnection(Variables.dbConString);
                    conn.Open();
                    OleDbCommand cmd = new OleDbCommand("INSERT INTO Import_History (FileName)VALUES(\"" + Variables.str_FilePath + "\")", conn);
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = "SELECT ID from Import_History ORDER BY id DESC";
                    int import_id = (int)(cmd.ExecuteScalar());
                    Thread t1 = new Thread(new ThreadStart(()=> 
                                {
                                    double d_progress = 0;
                                    string cmdline = "";
                                    cmdline = "开始分包...";
                                    FileStream fs_chanel = new FileStream(Variables.str_FilePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                                    byte[] buf_row1 = new byte[288];
                                    while (fs_chanel.Position < fs_chanel.Length)
                                    {
                                        fs_chanel.Read(buf_row1, 0, 288);
                                        if ((buf_row1[4] == 0x0A)&&(buf_row1[5]==0x01) && buf_row1[7] % 2 == 0)
                                        {
                                            RealDataRow rdl = new RealDataRow(buf_row1,import_id);
                                            cmdline =$"分包中...\n帧编号：{rdl.FrameCount}";
                                            d_progress = (double)fs_chanel.Position / fs_chanel.Length;
                                            rdl.Insert();
                                        }

                                        if (fs_chanel.Position % (288 * 1024) == 0)
                                        {
                                            Prog.Report(d_progress);
                                            List.Report(cmdline);
                                        }
                                    }
                                }));

                                Thread t2 = new Thread(new ThreadStart(() =>
                                {
                                    FileStream fs_chanel = new FileStream(Variables.str_FilePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                                    byte[] buf_row1 = new byte[288];
                                    while (fs_chanel.Position < fs_chanel.Length)
                                    {
                                        fs_chanel.Read(buf_row1, 0, 288);
                                        if (buf_row1[4] == 0x0A && buf_row1[5] == 0x02 && buf_row1[7] % 2 == 0)
                                        {
                                            RealDataRow rdl = new RealDataRow(buf_row1,import_id);
                                            rdl.Insert();
                                        }
                                    }


                                }));

                                Thread t3 = new Thread(new ThreadStart(() =>
                                {

                                    FileStream fs_chanel = new FileStream(Variables.str_FilePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                                    byte[] buf_row1 = new byte[288];
                                    while (fs_chanel.Position < fs_chanel.Length)
                                    {
                                        fs_chanel.Read(buf_row1, 0, 288);
                                        if (buf_row1[4] == 0x0A && buf_row1[5] == 0x03 && buf_row1[7] % 2 == 0)
                                        {
                                            RealDataRow rdl = new RealDataRow(buf_row1,import_id);
                                            rdl.Insert();
                                        }
                                    }

                                }));

                                Thread t4 = new Thread(new ThreadStart(() =>
                                {
                                    FileStream fs_chanel = new FileStream(Variables.str_FilePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                                    byte[] buf_row1 = new byte[288];
                                    while (fs_chanel.Position < fs_chanel.Length)
                                    {
                                        fs_chanel.Read(buf_row1, 0, 288);
                                        if (buf_row1[4] == 0x0A && buf_row1[5] == 0x04 &&buf_row1[7]%2==0)
                                        {
                                            RealDataRow rdl = new RealDataRow(buf_row1,import_id);
                                            rdl.Insert();
                                        }
                                    }
                                }));




                    Thread t5 = new Thread(new ThreadStart(() =>
                    {
                        FileStream fs_chanel = new FileStream(Variables.str_FilePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                        byte[] buf_row1 = new byte[288];
                        while (fs_chanel.Position < fs_chanel.Length)
                        {
                            fs_chanel.Read(buf_row1, 0, 288);
                            if ((buf_row1[4] == 0x0A) && (buf_row1[5] == 0x01) && buf_row1[7] % 2 == 1)
                            {
                                RealDataRow rdl = new RealDataRow(buf_row1,import_id);
                                rdl.Insert();
                            }
                        }
                    }));

                    Thread t6 = new Thread(new ThreadStart(() =>
                    {
                        FileStream fs_chanel = new FileStream(Variables.str_FilePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                        byte[] buf_row1 = new byte[288];
                        while (fs_chanel.Position < fs_chanel.Length)
                        {
                            fs_chanel.Read(buf_row1, 0, 288);
                            if (buf_row1[4] == 0x0A && buf_row1[5] == 0x02 && buf_row1[7] % 2 == 1)
                            {
                                RealDataRow rdl = new RealDataRow(buf_row1,import_id);
                                rdl.Insert();
                            }
                        }


                    }));


                    Thread t7 = new Thread(new ThreadStart(() =>
                    {

                        FileStream fs_chanel = new FileStream(Variables.str_FilePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                        byte[] buf_row1 = new byte[288];
                        while (fs_chanel.Position < fs_chanel.Length)
                        {
                            fs_chanel.Read(buf_row1, 0, 288);
                            if (buf_row1[4] == 0x0A && buf_row1[5] == 0x03 && buf_row1[7] % 2 == 1)
                            {
                                RealDataRow rdl = new RealDataRow(buf_row1,import_id);
                                rdl.Insert();
                            }
                        }

                    }));

                    Thread t8 = new Thread(new ThreadStart(() =>
                    {
                        FileStream fs_chanel = new FileStream(Variables.str_FilePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                        byte[] buf_row1 = new byte[288];
                        while (fs_chanel.Position < fs_chanel.Length)
                        {
                            fs_chanel.Read(buf_row1, 0, 288);
                            if (buf_row1[4] == 0x0A && buf_row1[5] == 0x04 && buf_row1[7] % 2 == 1)
                            {
                                RealDataRow rdl = new RealDataRow(buf_row1,import_id);
                                rdl.Insert();
                            }
                        }
                    }));

                    t1.Start();
                    t2.Start();
                    t3.Start();
                    t4.Start();
                    t5.Start();
                    t6.Start();
                    t7.Start();
                    t8.Start();
                    t1.Join();
                    t2.Join();
                    t3.Join();
                    t4.Join();
                    t5.Join();
                    t6.Join();
                    t7.Join();
                    t8.Join();
                    Prog.Report(0);
                    List.Report("分包完成！准备开始解压！");
                    Thread.Sleep(30);
                    t1 = new Thread(new ThreadStart(() =>
                    {
                        double d_progress = 0;
                        string cmdline = "";
                        FileStream fs_chanel = new FileStream(Variables.str_FilePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                        byte[] buf_row1 = new byte[288];
                        while (fs_chanel.Position < fs_chanel.Length)
                        {
                            fs_chanel.Read(buf_row1, 0, 288);
                            if ((buf_row1[4] == 0x08) && (buf_row1[5] == 0x01) && buf_row1[7] % 2 == 0)
                            {
                                FIBITMAP fibmp = FreeImage.LoadEx($"{Variables.str_pathWork}\\{import_id}_{readU16(buf_row1,6)}_1.jp2");
                                cmdline = $"解压中..\n帧号：{readU16(buf_row1, 6)}\n";
                                AuxDataRow adr = new AuxDataRow(buf_row1, import_id);
                                if (!fibmp.IsNull)
                                {
                                    byte[] buf_JP2 = new byte[512 * 160 * 2];
                                    Marshal.Copy(FreeImage.GetBits(fibmp), buf_JP2, 0, 512 * 160 * 2);
                                    FreeImage.Unload(fibmp);
                                    FileStream fs_out = new FileStream($"{Variables.str_pathWork}\\{import_id}_{readU16(buf_row1, 6)}_1.raw", FileMode.Create);
                                    fs_out.Write(buf_JP2, 0, 512 * 160 * 2);
                                    fs_out.Close();
                                    cmdline += "解压成功！";
                                }
                                else
                                {
                                    cmdline += "解压失败";
                                }
                            }

                            if (fs_chanel.Position % (288 * 1024) == 0)
                            {
                                List.Report(cmdline);
                                Prog.Report((double)fs_chanel.Position/fs_chanel.Length);
                            }
                        }
                    }));

                    t2 = new Thread(new ThreadStart(() =>
                    {
                        FileStream fs_chanel = new FileStream(Variables.str_FilePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                        byte[] buf_row1 = new byte[288];
                        while (fs_chanel.Position < fs_chanel.Length)
                        {
                            fs_chanel.Read(buf_row1, 0, 288);
                            if ((buf_row1[4] == 0x08) && (buf_row1[5] == 0x02) && buf_row1[7] % 2 == 0)
                            {
                                AuxDataRow adr = new AuxDataRow(buf_row1, import_id);
                                FIBITMAP fibmp = FreeImage.LoadEx($"{Variables.str_pathWork}\\{import_id}_{readU16(buf_row1, 6)}_2.jp2");
                                if (!fibmp.IsNull)
                                {
                                    byte[] buf_JP2 = new byte[512 * 160 * 2];
                                    Marshal.Copy(FreeImage.GetBits(fibmp), buf_JP2, 0, 512 * 160 * 2);
                                    FreeImage.Unload(fibmp);
                                    FileStream fs_out = new FileStream($"{Variables.str_pathWork}\\{import_id}_{readU16(buf_row1, 6)}_2.raw", FileMode.Create);
                                    fs_out.Write(buf_JP2, 0, 512 * 160 * 2);
                                    fs_out.Close();
                                    
                                }
                            }
                        }
                    }));
                    t3 = new Thread(new ThreadStart(() =>
                    {

                        FileStream fs_chanel = new FileStream(Variables.str_FilePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                        byte[] buf_row1 = new byte[288];
                        while (fs_chanel.Position < fs_chanel.Length)
                        {
                            fs_chanel.Read(buf_row1, 0, 288);
                            if ((buf_row1[4] == 0x08) && (buf_row1[5] == 0x03) && buf_row1[7] % 2 == 0)
                            {
                                AuxDataRow adr = new AuxDataRow(buf_row1, import_id);
                                FIBITMAP fibmp = FreeImage.LoadEx($"{Variables.str_pathWork}\\{import_id}_{readU16(buf_row1, 6)}_3.jp2");
                                if (!fibmp.IsNull)
                                {
                                    byte[] buf_JP2 = new byte[512 * 160 * 2];
                                    Marshal.Copy(FreeImage.GetBits(fibmp), buf_JP2, 0, 512 * 160 * 2);
                                    FreeImage.Unload(fibmp);
                                    FileStream fs_out = new FileStream($"{Variables.str_pathWork}\\{import_id}_{readU16(buf_row1, 6)}_3.raw", FileMode.Create);
                                    fs_out.Write(buf_JP2, 0, 512 * 160 * 2);
                                    fs_out.Close();
                                }
                            }
                        }

                    }));

                    t4 = new Thread(new ThreadStart(() =>
                    {
                        FileStream fs_chanel = new FileStream(Variables.str_FilePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                        byte[] buf_row1 = new byte[288];
                        while (fs_chanel.Position < fs_chanel.Length)
                        {
                            fs_chanel.Read(buf_row1, 0, 288);
                            if ((buf_row1[4] == 0x08) && (buf_row1[5] == 0x04) && buf_row1[7] % 2 == 0)
                            {
                                AuxDataRow adr = new AuxDataRow(buf_row1, import_id);

                                FIBITMAP fibmp = FreeImage.LoadEx($"{Variables.str_pathWork}\\{import_id}_{readU16(buf_row1, 6)}_4.jp2");

                                if (!fibmp.IsNull)
                                {
                                    byte[] buf_JP2 = new byte[512 * 160 * 2];
                                    Marshal.Copy(FreeImage.GetBits(fibmp), buf_JP2, 0, 512 * 160 * 2);
                                    FreeImage.Unload(fibmp);
                                    FileStream fs_out = new FileStream($"{Variables.str_pathWork}\\{import_id}_{readU16(buf_row1, 6)}_4.raw", FileMode.Create);
                                    fs_out.Write(buf_JP2, 0, 512 * 160 * 2);
                                    fs_out.Close();
                                }
                            }
                        }
                    }));
                    t5 = new Thread(new ThreadStart(() =>
                    {
                        FileStream fs_chanel = new FileStream(Variables.str_FilePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                        byte[] buf_row1 = new byte[288];
                        while (fs_chanel.Position < fs_chanel.Length)
                        {
                            fs_chanel.Read(buf_row1, 0, 288);
                            if ((buf_row1[4] == 0x08) && (buf_row1[5] == 0x01) && buf_row1[7] % 2 == 1)
                            {
                                AuxDataRow adr = new AuxDataRow(buf_row1, import_id);
                                FIBITMAP fibmp = FreeImage.LoadEx($"{Variables.str_pathWork}\\{import_id}_{readU16(buf_row1, 6)}_1.jp2");
                                if (!fibmp.IsNull)
                                {
                                    byte[] buf_JP2 = new byte[512 * 160 * 2];
                                    Marshal.Copy(FreeImage.GetBits(fibmp), buf_JP2, 0, 512 * 160 * 2);
                                    FreeImage.Unload(fibmp);
                                    FileStream fs_out = new FileStream($"{Variables.str_pathWork}\\{import_id}_{readU16(buf_row1, 6)}_1.raw", FileMode.Create);
                                    fs_out.Write(buf_JP2, 0, 512 * 160 * 2);
                                    fs_out.Close();
                                }
                            }
                        }
                    }));

                    t6 = new Thread(new ThreadStart(() =>
                    {
                        FileStream fs_chanel = new FileStream(Variables.str_FilePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                        byte[] buf_row1 = new byte[288];
                        while (fs_chanel.Position < fs_chanel.Length)
                        {
                            fs_chanel.Read(buf_row1, 0, 288);
                            if ((buf_row1[4] == 0x08) && (buf_row1[5] == 0x02) && buf_row1[7] % 2 == 1)
                            {
                                AuxDataRow adr = new AuxDataRow(buf_row1, import_id);
                                FIBITMAP fibmp = FreeImage.LoadEx($"{Variables.str_pathWork}\\{import_id}_{readU16(buf_row1, 6)}_2.jp2");
                                if (!fibmp.IsNull)
                                {
                                    byte[] buf_JP2 = new byte[512 * 160 * 2];
                                    Marshal.Copy(FreeImage.GetBits(fibmp), buf_JP2, 0, 512 * 160 * 2);
                                    FreeImage.Unload(fibmp);
                                    FileStream fs_out = new FileStream($"{Variables.str_pathWork}\\{import_id}_{readU16(buf_row1, 6)}_2.raw", FileMode.Create);
                                    fs_out.Write(buf_JP2, 0, 512 * 160 * 2);
                                    fs_out.Close();
                                }
                            }
                        }
                    }));
                    t7 = new Thread(new ThreadStart(() =>
                    {
                        FileStream fs_chanel = new FileStream(Variables.str_FilePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                        byte[] buf_row1 = new byte[288];
                        while (fs_chanel.Position < fs_chanel.Length)
                        {
                            fs_chanel.Read(buf_row1, 0, 288);
                            if ((buf_row1[4] == 0x08) && (buf_row1[5] == 0x03) && buf_row1[7] % 2 == 1)
                            {
                                AuxDataRow adr = new AuxDataRow(buf_row1, import_id);
                                FIBITMAP fibmp = FreeImage.LoadEx($"{Variables.str_pathWork}\\{import_id}_{readU16(buf_row1, 6)}_3.jp2");
                                if (!fibmp.IsNull)
                                {
                                    byte[] buf_JP2 = new byte[512 * 160 * 2];
                                    Marshal.Copy(FreeImage.GetBits(fibmp), buf_JP2, 0, 512 * 160 * 2);
                                    FreeImage.Unload(fibmp);
                                    FileStream fs_out = new FileStream($"{Variables.str_pathWork}\\{import_id}_{readU16(buf_row1, 6)}_3.raw", FileMode.Create);
                                    fs_out.Write(buf_JP2, 0, 512 * 160 * 2);
                                    fs_out.Close();
                                }
                            }
                        }
                    }));

                    t8 = new Thread(new ThreadStart(() =>
                    {
                        FileStream fs_chanel = new FileStream(Variables.str_FilePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                        byte[] buf_row1 = new byte[288];
                        while (fs_chanel.Position < fs_chanel.Length)
                        {
                            fs_chanel.Read(buf_row1, 0, 288);
                            if ((buf_row1[4] == 0x08) && (buf_row1[5] == 0x04) && buf_row1[7] % 2 == 1)
                            {
                                AuxDataRow adr = new AuxDataRow(buf_row1, import_id);
                                FIBITMAP fibmp = FreeImage.LoadEx($"{Variables.str_pathWork}\\{import_id}_{readU16(buf_row1, 6)}_4.jp2");
                                if (!fibmp.IsNull)
                                {
                                    byte[] buf_JP2 = new byte[512 * 160 * 2];
                                    Marshal.Copy(FreeImage.GetBits(fibmp), buf_JP2, 0, 512 * 160 * 2);
                                    FreeImage.Unload(fibmp);
                                    FileStream fs_out = new FileStream($"{Variables.str_pathWork}\\{import_id}_{readU16(buf_row1, 6)}_4.raw", FileMode.Create);
                                    fs_out.Write(buf_JP2, 0, 512 * 160 * 2);
                                    fs_out.Close();
                                    
                                }
                            }
                        }
                    }));
                    t1.Start();
                    t2.Start();
                    t3.Start();
                    t4.Start();
                    t5.Start();
                    t6.Start();
                    t7.Start();
                    t8.Start();
                    t1.Join();
                    t2.Join();
                    t3.Join();
                    t4.Join();
                    t5.Join();
                    t6.Join();
                    t7.Join();
                    t8.Join();

                    FileStream fs_db = new FileStream(Variables.str_FilePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                    byte[] buf_row_db = new byte[288];
                    List.Report("开始写入数据库..");
                    while (fs_db.Position < fs_db.Length)
                    {
                        fs_db.Read(buf_row_db, 0, 288);
                        if ((buf_row_db[4] == 0x08&&buf_row_db[5]==0x01))
                        {
                            AuxDataRow adr = new AuxDataRow(buf_row_db, import_id);
                            bool flag = true;

                            Parallel.For(1, 4, i => 
                            {

                                if (File.Exists($"{Variables.str_pathWork}\\{import_id}_{adr.FrameCount}_{i}.raw")) flag = flag & true;
                                else flag = flag & false;

                            });

                            if (flag)
                            adr.Insert();
                        }

                        if (fs_db.Position % (288 * 1024) == 0) Prog.Report((double)fs_db.Position/fs_db.Length);
                    }


                }
                catch (Exception e)
                {
                    return e.Message;
                }
                return "解压完成";

            });

        }


        public static Task<string> Import(IProgress<double> Prog, IProgress<string> List, CancellationToken cancel)
        {
            return Task.Run(() =>
            {
                try
                {
                    FileStream fs_Import = new FileStream(Variables.str_FilePath, FileMode.Open);
                    double file_length = fs_Import.Length;
                    byte[] buf_row = new byte[288];
                    AuxData Aux = new AuxData(ref buf_row);
                    OleDbConnection conn = new OleDbConnection(Variables.dbConString);
                    conn.Open();
                    OleDbCommand cmd = new OleDbCommand("INSERT INTO Import_History (FileName)VALUES(\"" + Variables.str_FilePath + "\")", conn);
                    OledbDatabase db = new OledbDatabase(Variables.dbPath);
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = "SELECT ID from Import_History ORDER BY id DESC";
                    int import_id = (int)(cmd.ExecuteScalar());

                    while (fs_Import.Position < fs_Import.Length)
                    {
                        fs_Import.Read(buf_row, 0, 288);
                        if (buf_row[4] == 0x08 && buf_row[5] == 0x01)
                        {
                            Aux = new AuxData(ref buf_row);
                            if (!Aux.Insert(import_id)) throw (new Exception("数据库写入失败！"));
                            
                        }
                        else if (buf_row[4] == 0x0A)
                        {

                            RealDataRow rdl = new RealDataRow(buf_row,import_id);

                            ThreadPool.QueueUserWorkItem(o=> { rdl.Insert(); });
                            
                            
                            

                            //cmd.CommandText = $"insert into Temp values ({import_id},{AuxData.readU16(buf_row, 6)},{ AuxData.readU8(buf_row, 5)},{ (fs_Import.Position / 288 - 1)},{ AuxData.readU16(buf_row, 8)})";
                            //cmd.ExecuteNonQuery();
                        }
                        else continue;

                        if (fs_Import.Position % (288 * 1024) == 0)
                        {
                            Prog.Report((double)fs_Import.Position / file_length);
                            List.Report(Aux.ToString());
                        }
                           
                        cancel.ThrowIfCancellationRequested();
                    }

                    /*

                    DataTable dt = db.GetDataTable("SELECT distinct FrameId From AuxData WHERE ImportId=" + import_id.ToString());
                    


                    byte[] buf_Image = new byte[256];
              
                        foreach (DataRow dr in dt.Rows)
                        {
                         int progint = 0;
                         int FrmCnt = (int)(dr.ItemArray[0]);

                        cmd.CommandText = "SELECT COUNT(*) FROM (SELECT distinct Chanel FROM Temp WHERE FrmCnt=" + ((int)(dr.ItemArray[0])).ToString() + " AND ImportId="+import_id.ToString()+")";
                        if ((int)cmd.ExecuteScalar() == 4)
                        {
                            OledbDatabase db2 = new OledbDatabase(Variables.dbPath);
                            MemoryStream[] ms = new MemoryStream[4];

                            byte[] bufferBMP = new byte[2048 * 160 * 2];
                            byte[] bufferJP2 = new byte[512 * 160 * 2];
                            for (int i = 0; i < 4; i++)
                            {
                               
                                OleDbCommand cmd2 = new OleDbCommand("SELECT [Position] FROM [Temp] WHERE [FrmCnt]=" + ((int)(dr.ItemArray[0])).ToString() + " and [ImportId] =" + import_id.ToString() + " and [Chanel]=" + (i + 1).ToString() + " ORDER BY [PackCnt] ASC", conn);
                                OleDbDataReader reader2 = cmd2.ExecuteReader();
                                DataTable dt2 = new DataTable();
                                dt2.Load(reader2);

                                
                                ms[i] = new MemoryStream(dt2.Rows.Count*256);
                                foreach (DataRow dr2 in dt2.Rows)
                                {
                                    fs_Import.Position = (int)dr2.ItemArray[0] * 288 + 16;
                                    fs_Import.Read(buf_Image, 0, 256);
                                    ms[i].Write(buf_Image, 0, 256);
                                    
                                }

                                ms[i] = new MemoryStream(ms[i].GetBuffer(),16,ms[i].GetBuffer().Length-16);

                            }


 
                        
                        for(int i=0;i<4;i++)
                        {
                            FIBITMAP dib = FreeImage.LoadFromStream(ms[i]);
                            Marshal.Copy(FreeImage.GetBits(dib), bufferJP2, 0, 512 * 160 * 2);
                            for (int j = 0; j < 160; j++)
                            {
                                for (int k = 0; k < 512; k++)
                                {
                                    bufferBMP[2 * (j * 2048 + k + i  * 512)] = bufferJP2[2 * (j * 512 + k)];
                                    bufferBMP[2 * (j * 2048 + k + i * 512) + 1] = bufferJP2[2 * (j * 512 + k) + 1];
                                }
                            }

                        }

                        FileStream fs = new FileStream(Variables.str_pathWork + "\\" + (import_id).ToString()+"_"+FrmCnt.ToString() + ".raw", FileMode.Create);
                        fs.Write(bufferBMP, 0, 2048 * 160 * 2);
                        fs.Close();

                        Prog.Report((double)(progint++)/(double)dt.Rows.Count);

                        }

                        }
                    fs_Import.Close();

                    */


                }
                catch (Exception e)
                {
                    return e.Message;
                }
                
               
                return "解压完成！";
            });
        }


        /*
        public static Task<string> Import_2(IProgress<double> Prog, IProgress<string> List, CancellationToken cancel)
        {
            return Task.Run(()=> 
            {
                List<AuxDataRow> ListAuxDataRow;
                List<FRAME> ListFrame;
                List<int> ListFrmCnt;
                try
                {            
                    FileStream fs = new FileStream(Variables.str_FilePath,FileMode.Open);
                    byte[] buf_Row = new byte[288];
                    ListAuxDataRow = new List<AuxDataRow>();
                    ListFrmCnt = new List<int>();
                    ListFrame = new List<FRAME>();
                    while (fs.Position<fs.Length)
                    {
                        fs.Read(buf_Row, 0, 288);
                        if (buf_Row[4] == 0x08 && buf_Row[5] == 0x01)
                        {
                            ThreadPool.QueueUserWorkItem(o=> 
                            {
                            ListAuxDataRow.Add(new AuxDataRow(buf_Row));
                            },buf_Row);
                        }
                        else if(buf_Row[4]==0x0A)
                        {
                            RealDataRow rdl = new RealDataRow(buf_Row);

                            if (ListFrmCnt.Contains(rdl.FrameCount))
                            {
                                ListFrame[ListFrame.IndexOf(ListFrame.Find(delegate (FRAME f) { return (f.FrmCnt == rdl.FrameCount); }))].InsertRow(rdl);
                            }
                            else
                            {
                                ListFrmCnt.Add(rdl.FrameCount);
                                FRAME f = new FRAME(rdl.FrameCount);
                                f.InsertRow(rdl);
                                ListFrame.Add(f);
                            }
                        }
                        if(fs.Position%(288*1000)==0)
                        Prog.Report((double)fs.Position / (double)fs.Length);
                    }

                    fs.Close();
                    ThreadPool.QueueUserWorkItem((o) =>
                    {
                        int i = 0;
                        foreach (AuxDataRow r in ListAuxDataRow)
                        {
                            r.Insert();
                            Prog.Report((double)i / (double)ListAuxDataRow.Count);
                            i++;
                        }
                    });
                }
                catch (Exception Error)
                {
                    return Error.Message;
                }

                return "分包完成！";
            });


        }

    


        public static Task<string> Import_3(IProgress<double> Prog, IProgress<string> List, CancellationToken cancel)
        {
            return Task.Run(() =>
            {
                try
                {
                    FileStream fs = new FileStream(Variables.str_FilePath, FileMode.Open);
                    byte[] buf_Row = new byte[288];
                    while (fs.Position < fs.Length)
                    {
                        fs.Read(buf_Row, 0, 288);
                        if (buf_Row[4] == 0x08 && buf_Row[5] == 0x01)
                        {

                        }
                        else if (buf_Row[4] == 0x0A)
                        {
                            RealDataRow rdl = new RealDataRow(buf_Row);
                            rdl.Insert();

   
                        }
                        if (fs.Position % (288 * 1000) == 0)
                            Prog.Report((double)fs.Position / (double)fs.Length);
                    }

                    fs.Close();
                }
                catch (Exception Error)
                {
                    return Error.Message;
                }

                return "分包完成！";
            });


        }


    */

        public static Task<Bitmap[]> GetBmp(DataTable dt_Result)
        {

            return Task.Run(() =>
            {

                Bitmap[] r = new Bitmap[6];


                Thread _tTop = new Thread(new ThreadStart(() => {
                    Bitmap bmpTop = new Bitmap(2048, dt_Result.Rows.Count);
                    BitmapData bmpData = bmpTop.LockBits(new Rectangle(0, 0, 2048, dt_Result.Rows.Count), System.Drawing.Imaging.ImageLockMode.WriteOnly, bmpTop.PixelFormat);
                    byte[] buf_full = new byte[2048 * dt_Result.Rows.Count * 4];
                    Parallel.For(0, dt_Result.Rows.Count, (i) => {
                        byte[] buf_rgb = new byte[2048 * 4];
                        Parallel.For(1, 5, k => {
                            FileStream fs = new FileStream($"{Variables.str_pathWork}\\{(int)(dt_Result.Rows[i].ItemArray[14])}_{(int)(dt_Result.Rows[i].ItemArray[0])}_{k}.raw", FileMode.Open, FileAccess.Read, FileShare.Read);
                            fs.Seek(80 * 512 * 2, SeekOrigin.Begin);
                            byte[] temp = new byte[512 * 2];
                            fs.Read(temp, 0, 1024);

                            Parallel.For(0, 512, l =>
                            {
                                Spectra2RGB.HsvToRgb(360 * (0 / 160), 1, ((double)temp[l * 2] / 256), out buf_rgb[512 * 4 * (k - 1) + 4 * l], out buf_rgb[512 * 4 * (k - 1) + 4 * l + 1], out buf_rgb[512 * 4 * (k - 1) + 4 * l + 2]);
                                buf_rgb[512 * 4 * (k - 1) + 4 * l + 3] = 255;
                            });
                            fs.Close();
                        });
                        Array.Copy(buf_rgb, 0, buf_full, 4 * 2048 * i, 4 * 2048);
                    });
                    Marshal.Copy(buf_full, 0, bmpData.Scan0, 2048 * dt_Result.Rows.Count * 4);
                    bmpTop.UnlockBits(bmpData);

                    r[0] = bmpTop;
                }));
                Thread _tBottom = new Thread(new ThreadStart(() => {
                    Bitmap bmpBottom = new Bitmap(2048, dt_Result.Rows.Count);
                    BitmapData bmpData = bmpBottom.LockBits(new Rectangle(0, 0, 2048, dt_Result.Rows.Count), System.Drawing.Imaging.ImageLockMode.WriteOnly, bmpBottom.PixelFormat);
                    byte[] buf_full = new byte[2048 * dt_Result.Rows.Count * 4];
                    Parallel.For(0, dt_Result.Rows.Count, (i) => {
                        byte[] buf_rgb = new byte[2048 * 4];
                        Parallel.For(1, 5, k => {
                            FileStream fs = new FileStream($"{Variables.str_pathWork}\\{(int)(dt_Result.Rows[i].ItemArray[14])}_{(int)(dt_Result.Rows[i].ItemArray[0])}_{k}.raw", FileMode.Open, FileAccess.Read, FileShare.Read);
                            fs.Seek(80 * 512 * 2, SeekOrigin.Begin);
                            byte[] temp = new byte[512 * 2];
                            fs.Read(temp, 0, 1024);

                            Parallel.For(0, 512, l =>
                            {
                                Spectra2RGB.HsvToRgb(360 * (159 / 160), 1, ((double)temp[l * 2] / 256), out buf_rgb[512 * 4 * (k - 1) + 4 * l], out buf_rgb[512 * 4 * (k - 1) + 4 * l + 1], out buf_rgb[512 * 4 * (k - 1) + 4 * l + 2]);
                                buf_rgb[512 * 4 * (k - 1) + 4 * l + 3] = 255;
                            });
                            fs.Close();
                        });
                        Array.Copy(buf_rgb, 0, buf_full, 4 * 2048 * i, 4 * 2048);
                    });
                    Marshal.Copy(buf_full, 0, bmpData.Scan0, 2048 * dt_Result.Rows.Count * 4);
                    bmpBottom.UnlockBits(bmpData);

                    r[1] = bmpBottom;
                }));
                Thread _tUp = new Thread(new ThreadStart(() => {
                    Bitmap bmpUp = new Bitmap(2048, 160);
                    BitmapData bmpData = bmpUp.LockBits(new Rectangle(0, 0, 2048, 160), System.Drawing.Imaging.ImageLockMode.WriteOnly, bmpUp.PixelFormat);
                    byte[] buf_full = new byte[2048 * 160 * 4];
                    
                    Parallel.For(0, 4, k =>
                    {
                        byte[] buf_file = new byte[512 * 160 * 2];
                        FileStream fs = new FileStream($"{Variables.str_pathWork}\\{(int)(dt_Result.Rows[0].ItemArray[14])}_{(int)(dt_Result.Rows[0].ItemArray[0])}_{k+1}.raw", FileMode.Open, FileAccess.Read, FileShare.Read);
                        fs.Read(buf_file, 0, 512 * 160 * 2);
                        Parallel.For(0, 160, i => {
                            Parallel.For(0, 512, j =>
                            {
                                Spectra2RGB.HsvToRgb((double)i/160*360,1,(double)buf_file[1024*i+2*j]/255,out buf_full[8192*i+2048*k+4*j], out buf_full[8192 * i + 2048 * k + 4 * j+1], out buf_full[8192 * i + 2048 * k + 4 * j+2]);
                            });
                        });
                    });
                    Marshal.Copy(buf_full, 0, bmpData.Scan0, 2048 *160 * 4);
                    bmpUp.UnlockBits(bmpData);
                    MemoryStream ms = new MemoryStream();
                    bmpUp.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
                    r[2] = bmpUp;
                }));
                Thread _tDown = new Thread(new ThreadStart(() => {



                }));
                Thread _tRight = new Thread(new ThreadStart(() => {


                }));
                Thread _tLeft = new Thread(new ThreadStart(() => {


                }));

                _tTop.Start();
                _tBottom.Start();
                _tUp.Start();
                _tTop.Join();
                _tBottom.Join();
                _tUp.Join();
                return r;
                
            });
        }

        public static Task<double[,]> GetChart(DataTable dt_Result)
        {
            return Task.Run(()=> {
                double[,] result = new double[160, 2];



                return result;
            });
        }

        public static Task<DataTable> QueryResult(bool isChecked1, bool isChecked2, bool isChecked3, DateTime start_time, DateTime end_time, long start_FrmCnt, long end_FrmCnt, Coord coord_TL, Coord coord_DR)
        {

            return Task.Run(() =>
            {
                OleDbConnection conn = new OleDbConnection(Variables.dbConString);
                conn.Open();
                OleDbCommand cmmd = new OleDbCommand("", conn);
                cmmd.CommandText = "SELECT ID from Import_History ORDER BY id DESC";
                int import_id = (int)(cmmd.ExecuteScalar());

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
                OledbDatabase db = new OledbDatabase(Variables.dbPath);
                return db.GetDataTable(command);


            });
            }

        public static Task<DataTable> QueryResult()
        {
            return Task.Run(() => {

                OledbDatabase db = new OledbDatabase(Variables.dbConString);

                return db.GetDataTable("select * from AuxData");

            });
        }


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
            OledbDatabase db = new OledbDatabase(Variables.dbPath);
            DataTable dt = db.GetDataTable("SELECT * FROM SpectrumMap WHERE SpaN="+(col+1).ToString());
            Array.Copy(dt.Rows[0].ItemArray, 1, result, 0, 160);
            return result;
        }
        #endregion
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



    public class SpecProc
    {

        public static Task<System.Windows.Point[]> GetSpecCurv(int importId, int frmCnt_Start, int frmCnt_End, int specSelected, System.Windows.Point p)
        {
            return Task.Run(()=>
            {

                System.Windows.Point[] result = new System.Windows.Point[160];
                FileStream fs = new FileStream($"{Variables.str_pathWork}\\{importId}_{(int)(frmCnt_Start + p.Y * (frmCnt_End - frmCnt_Start))}_{(int)(p.X*2048)/512+1}.raw", FileMode.Open, FileAccess.Read, FileShare.Read);
                byte[] buf = new byte[512 * 160 * 2];
                fs.Read(buf, 0, 512 * 160 * 2);
                int col = (int)(p.X*2048)%512;
                double[] spec_nm = DataProc.GetWaveLen(col);

                Parallel.For(0, 160, i =>
                {
                    result[i] = new System.Windows.Point(spec_nm[i], DataProc.readU16(buf, i * 1024 + 2 * col));

                });


                return result;

            });

        }
    }






    public class ROW
    {
        public UInt16 FrameCount;
        public UInt16 PackCount;
        public byte Chanel;
        public int ImportId=0;
        public byte[] buf_Row;
        public ROW(byte[] ROW,int id)
        {
            FrameCount = DataProc.readU16(ROW, 6);
            PackCount = DataProc.readU16(ROW, 8);
            Chanel = DataProc.readU8(ROW, 5);
            buf_Row = ROW;
            ImportId = id;

        }
    }

    public class RealDataRow : ROW
    {
        public bool isHead = false;
        public bool isTail = false;
        public RealDataRow(byte[] ROW,int id) : base(ROW,id)
        {

            if ((UInt32)(ROW[32] << 24 | ROW[33] << 16 | ROW[34] << 8 | ROW[35]) == 0xFF4FFF51)
            {
                isHead = true;
            }
        

        }
        public void Insert()
        {

            FileStream fs = new FileStream(Variables.str_pathWork + "\\" + ImportId.ToString() + "_" + FrameCount.ToString() + "_" + Chanel.ToString() + ".jp2", FileMode.Append,FileAccess.Write,FileShare.Write);
            if(isHead)fs.Write(buf_Row, 32, 240);
            else fs.Write(buf_Row, 16, 256);
            fs.Close();

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
        public AuxDataRow(byte[] ROW,int id) : base(ROW,id)
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

        internal void Insert()
        {
            OledbDatabase sqlExcute = new OledbDatabase(Variables.dbPath);
            try
            {
                var sql = "insert into AuxData values(@FrameId,@SatelliteId,@GST,@Lat,@Lon,@X,@Y,@Z,@Vx,@Vy,@Vz,@Ox,@Oy,@Oz,@ImportId,@Chanel);";
                var cmdparams = new List<OleDbParameter>()
                {
                    new OleDbParameter("FrameId", FrameCount),
                    new OleDbParameter("SatelliteId","MicroSat"),
                    new OleDbParameter("GST",GST),
                    new OleDbParameter("Lat",Lat),
                    new OleDbParameter("Lon",Lon),
                    new OleDbParameter("X",X),
                    new OleDbParameter("Y",Y),
                    new OleDbParameter("Z",Z),
                    new OleDbParameter("Vx",Vx),
                    new OleDbParameter("Vy",Vy),
                    new OleDbParameter("Vz",Vz),
                    new OleDbParameter("Ox",Ox),
                    new OleDbParameter("Oy",Oy),
                    new OleDbParameter("Oz",Oz),
                    new OleDbParameter("ImportId",ImportId),
                    new OleDbParameter("Chanel",Chanel)
                };
                sqlExcute.ExecuteNonQuery(sql, cmdparams);
            }
            catch (Exception e)
            {
                //Do any logging operation here if necessary
                throw e;
            }
        }
    }



    public class AuxData
    {
        public UInt16 FrmCnt;
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
        public byte[] buf_row;

        public AuxData(ref byte[] buf_row)
        {
            this.buf_row = buf_row;
            FrmCnt = readU16(buf_row,6);
            X = 7000 * Math.Cos(Math.PI*((double)(FrmCnt%360)/180));//readLength(32);
            Y = 7000 * Math.Sin(Math.PI * ((double)(FrmCnt % 360) / 180));//readLength(36);
            Z =  0;//readLength(40);
            Vx = 7.546 * Math.Cos(Math.PI * ((double)(FrmCnt % 360) / 180) + Math.PI / 2);//readLength(44);
            Vy = 7.546 * Math.Sin(Math.PI * ((double)(FrmCnt % 360) / 180) + Math.PI / 2);//readLength();
            Vz = 0;//0;//
            GST = DateTime.Now.Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds;//OrbitCalc.CalGST(new POSE_TIME(DateTime.Now));
            double[] latlon = new double[2];
            latlon = OrbitCalc.CalEarthLonLat(new double[3] { X, Y, Z }, GST);
            Lat =  latlon[1] * 180 / Math.PI;
            Lon = latlon[0] * 180 / Math.PI;
            Ox = 0;
            Oy = 0;
            Oz = 0;
        }

        public override string ToString()
        {
            string s = null;

            s +="帧编号："+ FrmCnt.ToString()+"\n";
            s += "经度：" + Lon.ToString() + "\n";
            s += "纬度:" + Lat.ToString() + "\n"
;            return s;

        }

        public bool Insert(int ImportId)
        {
            OledbDatabase sqlExcute = new OledbDatabase(Variables.dbPath);
            try
            {

                var sql = "insert into AuxData values(@FrameId,@SatelliteId,@GST,@Lat,@Lon,@X,@Y,@Z,@Vx,@Vy,@Vz,@Ox,@Oy,@Oz,@ImportId);";
                var cmdparams = new List<OleDbParameter>()
                {
                    new OleDbParameter("FrameId", FrmCnt),
                    new OleDbParameter("SatelliteId","MicroSat"),
                    new OleDbParameter("GST",GST),
                    new OleDbParameter("Lat",Lat),
                    new OleDbParameter("Lon",Lon),
                    new OleDbParameter("X",X),
                    new OleDbParameter("Y",Y),
                    new OleDbParameter("Z",Z),
                    new OleDbParameter("Vx",Vx),
                    new OleDbParameter("Vy",Vy),
                    new OleDbParameter("Vz",Vz),
                    new OleDbParameter("Ox",Ox),
                    new OleDbParameter("Oy",Oy),
                    new OleDbParameter("Oz",Oz),
                    new OleDbParameter("ImportId",ImportId)
                };
                return sqlExcute.ExecuteNonQuery(sql, cmdparams);
            }
            catch (Exception e)
            {
                //Do any logging operation here if necessary
                return false;
                throw e;
                
            }
        }
        #region 数据解析函数
        public static UInt32 readU32(byte[]buf_row, int addr)
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
            float result = (float)readI32(buf_row,addr) / 1000;
            return result;
        }
        public static float readDegree(byte[] buf_row, int addr)
        {
            float result = (float)((float)readI32(buf_row,addr) / 1000 * 180/Math.PI);
            return result;
        }
        #endregion
    }
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
            M =(int) ((now.Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds - (int)(now.Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds)) * 1000000);
        }
    }
}
