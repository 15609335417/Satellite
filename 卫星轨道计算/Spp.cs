using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Threading;
using System.Reflection;

namespace 卫星轨道计算
{
    public partial class Spp : Form
    {
        int Curcnt = 0;
        Thread thread1;
        Thread thread2;
        DataTable table1;//O文件

        /// <summary>
        /// O 文件所涉及全局变量
        /// </summary>
        string RINEX_VERSION=null;
        string MARKER_NAME=null;
        double INTERVAL = 0;
        string TIME_OF_FIRST_OBS = null;
        string TIME_OF_LAST_OBS = null;
        double[] APPROX_POSITION=new double[3];

        DataTable table2;//N文件

        /// <summary>
        /// N文件所涉及全局变量
        /// </summary>
        double[] GPSA = new double[4];
        double[] GPSB = new double[4];
        double[] GAL=new double[4];
        double[] GPUT=new double[4];
        double[] GAUT=new double[4];
        double[] GAGP=new double[4];
        int LEAP_SECONDS = 0;

        DataTable table3 = new DataTable();

        DataTable table4 = new DataTable();

        //进度条
        int MinValue = 0;
        int MaxValue = 100;

      public Spp()
        {
            table1 = new DataTable();
            table1.Columns.Add("时间", typeof(string));
            table1.Columns.Add("卫星数",typeof(int));
            table1.Columns.Add("PRN",typeof(string));
            //GPS C1C L1C D1C S1C C1W S1W C2W L2W D2W S2W
            //GLA C1C L1C D1C S1C C5Q L5Q D5Q S5Q
            //SBA C1C L1C D1C S1C
            //GLO C1C L1C D1C S1C C2P L2P D2P S2P
            //BDS C2I L2I D2I S2I C7I L7I D7I S7I
            //QSZ C1C L1C D1C S1C C2L L2L D2L S2L
            table1.Columns.Add("C1C/C2I", typeof(double));
            table1.Columns.Add("L1C/L2I", typeof(double));
            table1.Columns.Add("D1C/D2I", typeof(double));
            table1.Columns.Add("S1C/S2I", typeof(double));
            table1.Columns.Add("C2W/C7I", typeof(double));
            table1.Columns.Add("L2W/L7I", typeof(double));
            table1.Columns.Add("D2W/D7I", typeof(double));
            table1.Columns.Add("S2W/S7I", typeof(double));

            table2 = new DataTable();
            table2.Columns.Add("PRN", typeof(string));
            table2.Columns.Add("TOC", typeof(string));
            table2.Columns.Add("钟偏", typeof(double));
            table2.Columns.Add("钟漂", typeof(double));
            table2.Columns.Add("频漂",typeof (double));
            //广播轨道1
            table2.Columns.Add("IODE", typeof(double));
            table2.Columns.Add("Crs", typeof(double));
            table2.Columns.Add("·n", typeof(double));
            table2.Columns.Add("M0", typeof(double));
            //广播轨道2
            table2.Columns.Add("Cuc", typeof(double));
            table2.Columns.Add("e", typeof(double));
            table2.Columns.Add("Cus", typeof(double));
            table2.Columns.Add("Sqrt_A", typeof(double));
            //广播轨道3
            table2.Columns.Add("TOE",typeof(double));
            table2.Columns.Add("Cic", typeof(double));
            table2.Columns.Add("OMEGA", typeof(double));
            table2.Columns.Add("Cis", typeof(double));
            //广播轨道4
            table2.Columns.Add("i0", typeof(double));
            table2.Columns.Add("Crc", typeof(double));
            table2.Columns.Add("omega", typeof(double));
            table2.Columns.Add("d_omega", typeof(double));
            //广播轨道5
            table2.Columns.Add("d_i", typeof(double));
            table2.Columns.Add("L2_Code", typeof(double));
            table2.Columns.Add("GPS_Week", typeof(double));
            table2.Columns.Add("L2_P_Code", typeof(double));
            //广播轨道6
            table2.Columns.Add("Accu", typeof(double));
            table2.Columns.Add("Health", typeof(double));
            table2.Columns.Add("TGD", typeof(double));
            table2.Columns.Add("IODC_age", typeof(double));
            //广播轨道7
            table2.Columns.Add("time_sent",typeof (double));
            table2.Columns.Add("nihequjian", typeof(double));

            table3 = new DataTable();
            table3.Columns.Add("时间", typeof(string));
            table3.Columns.Add("卫星数", typeof(int));
            table3.Columns.Add("PRN", typeof(string));
            table3.Columns.Add("X", typeof(double));
            table3.Columns.Add("Y", typeof(double));
            table3.Columns.Add("Z", typeof(double));
            table3.Columns.Add("Clock", typeof(double));

            table4 = new DataTable();
            table4.Columns.Add("时间", typeof(string));
            table4.Columns.Add("X", typeof(double));
            table4.Columns.Add("Y", typeof(double));
            table4.Columns.Add("Z", typeof(double));
            table4.Columns.Add("Clock", typeof(double));
            table4.Columns.Add("后验权中误差", typeof(double));
            InitializeComponent();

            ///双缓冲，使下拉不闪烁
            /// 新添加这一句调用就行了，如果有ListViews也是这样添加，
            /// 但要注意方法里改为有关ListViews的声明即可
            dataGridView1.DoubleBufferedDataGirdView(true);
            dataGridView2.DoubleBufferedDataGirdView(true);
            dataGridView3.DoubleBufferedDataGirdView(true);
            dataGridView4.DoubleBufferedDataGirdView(true);

        }
        /// <summary>
        /// 打开O文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void 打开O文件ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                openFileDialog1.Filter = "(*.??O)|*.??O|(*.??o)|*.??o|(*.obs)|*.obs|(All Files)|*.*";
                if(openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    StreamReader reader=new StreamReader(openFileDialog1.FileName);
                    string line;
                    int NumRows;
                    int NumSats;
                    line = reader.ReadLine();
                    RINEX_VERSION = line.Substring(5, 4);
                    while (!line.Contains("END OF HEADER"))
                    {
                        line=reader.ReadLine();
                        if (line.Contains("MARKER NAME"))
                            MARKER_NAME = line.Substring(0, 4);
                        if(line.Contains("APPROX POSITION XYZ"))
                        {
                            APPROX_POSITION[0] = Convert.ToDouble(line.Substring(1, 13));
                            APPROX_POSITION[1] = Convert.ToDouble(line.Substring(15, 13));
                            APPROX_POSITION[2] = Convert.ToDouble(line.Substring(29, 13)); 
                        }
                        if (line.Contains("INTERVAL"))
                            INTERVAL = Convert.ToDouble(line.Substring(4, 6));
                        if (line.Contains("TIME OF FIRST OBS"))
                            TIME_OF_FIRST_OBS = line.Substring(0, 60);
                        if (line.Contains("TIME OF LAST OBS"))
                            TIME_OF_LAST_OBS = line.Substring(0, 60);
                    }
                    table1.Rows.Add();
                    NumRows = 0;
                    line= reader.ReadLine();
                    while (!reader.EndOfStream)
                    {
                        NumSats = 0;
                        table1.Rows[NumRows]["时间"] = line.Substring(2, 27);
                        line= reader.ReadLine();
                        while (!reader.EndOfStream && line.Substring(0, 1) != ">") 
                        {
                            if(line.Substring(0, 1) == "C"|| line.Substring(0, 1) == "G")
                            {
                                table1.Rows[NumRows]["PRN"] = line.Substring(0, 3);
                                if (line.Substring(0, 1) == "C")
                                {
                                    NumSats++;
                                    if (line.Substring(4, 13) != "             ")
                                        table1.Rows[NumRows]["C1C/C2I"] = Convert.ToDouble(line.Substring(4, 13));
                                    if (line.Substring(20, 15) != "               ")
                                        table1.Rows[NumRows]["L1C/L2I"] = Convert.ToDouble(line.Substring(20, 15));
                                    if (line.Substring(36, 13) != "             ")
                                        table1.Rows[NumRows]["D1C/D2I"] = Convert.ToDouble(line.Substring(36, 13));
                                    if (line.Substring(52, 13) != "             ")
                                        table1.Rows[NumRows]["S1C/S2I"] = Convert.ToDouble(line.Substring(52, 13));
                                    if (line.Length > 68)
                                    {
                                        if (line.Substring(68, 13) != "             ")
                                            table1.Rows[NumRows]["C2W/C7I"] = Convert.ToDouble(line.Substring(68, 13));
                                        if (line.Substring(84, 15) != "               ")
                                            table1.Rows[NumRows]["L2W/L7I"] = Convert.ToDouble(line.Substring(84, 15));
                                        if (line.Substring(100, 13) != "             ")
                                            table1.Rows[NumRows]["D2W/D7I"] = Convert.ToDouble(line.Substring(100, 13));
                                        if (line.Substring(116, 13) != "             ")
                                            table1.Rows[NumRows]["S2W/S7I"] = Convert.ToDouble(line.Substring(116, 13));
                                    }
                                }
                                if (line.Substring(0, 1) == "G")
                                {
                                    NumSats++;
                                    if (line.Substring(4, 13) != "             ")
                                        table1.Rows[NumRows]["C1C/C2I"] = Convert.ToDouble(line.Substring(4, 13));
                                    if (line.Substring(20, 15) != "               ")
                                        table1.Rows[NumRows]["L1C/L2I"] = Convert.ToDouble(line.Substring(20, 15));
                                    if (line.Substring(36, 13) != "             ")
                                        table1.Rows[NumRows]["D1C/D2I"] = Convert.ToDouble(line.Substring(36, 13));
                                    if (line.Substring(52, 13) != "             ")
                                        table1.Rows[NumRows]["S1C/S2I"] = Convert.ToDouble(line.Substring(52, 13));
                                    if (line.Length > 100)
                                    {
                                        if (line.Substring(100, 13) != "             ")
                                            table1.Rows[NumRows]["C2W/C7I"] = Convert.ToDouble(line.Substring(100, 13));
                                        if (line.Substring(116, 15) != "               ")
                                            table1.Rows[NumRows]["L2W/L7I"] = Convert.ToDouble(line.Substring(116, 15));
                                        if (line.Substring(132, 13) != "             ")
                                            table1.Rows[NumRows]["D2W/D7I"] = Convert.ToDouble(line.Substring(132, 13));
                                        if (line.Substring(148, 13) != "             ")
                                            table1.Rows[NumRows]["S2W/S7I"] = Convert.ToDouble(line.Substring(148, 13));
                                    }
                                }
                                table1.Rows.Add();
                                NumRows++;
                            }
                            line = reader.ReadLine();
                        }
                        table1.Rows[NumRows-NumSats]["卫星数"] = NumSats;
                    }
                }
                dataGridView1.DataSource = table1;
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// 打开N文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void 打开N文件ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                openFileDialog2.Filter = "(*.??N)|*.??N|(*.??n)|*.??n|(*.nav)|*.nav|(All Files)|*.*";
                if(openFileDialog2.ShowDialog() == DialogResult.OK)
                {
                    StreamReader reader = new StreamReader(openFileDialog2.FileName);
                    string line=reader.ReadLine();
                    int NumRows = 0;
                    while (!line.Contains("END OF HEADER"))
                    {
                        if (line.Contains("GPSA"))
                        {
                            GPSA[0] = Convert.ToDouble(line.Substring(6, 11));
                            GPSA[1] = Convert.ToDouble(line.Substring(18, 11));
                            GPSA[2]= Convert.ToDouble(line.Substring(30, 11));
                            GPSA[3]=Convert.ToDouble(line.Substring(42, 11));
                        }
                        if(line.Contains("GPSB"))
                        {
                            GPSB[0] = Convert.ToDouble(line.Substring(6, 11));
                            GPSB[1] = Convert.ToDouble(line.Substring(18, 11));
                            GPSB[2] = Convert.ToDouble(line.Substring(30, 11));
                            GPSB[3] = Convert.ToDouble(line.Substring(42, 11));
                        }
                        if(line.Contains("GAL"))
                        {
                            GAL[0] = Convert.ToDouble(line.Substring(6, 11));
                            GAL[1] = Convert.ToDouble(line.Substring(18, 11));
                            GAL[2] = Convert.ToDouble(line.Substring(30, 11));
                            GAL[3] = Convert.ToDouble(line.Substring(42, 11));
                        }
                        if (line.Contains("GPUT"))
                        {
                            GPUT[0] = Convert.ToDouble(line.Substring(6, 16));
                            GPUT[1] = Convert.ToDouble(line.Substring(22, 16));
                            GPUT[2] = Convert.ToDouble(line.Substring(39, 6));
                            GPUT[3] = Convert.ToDouble(line.Substring(46, 4));
                        }
                        if (line.Contains("GAUT"))
                        {
                            GAUT[0] = Convert.ToDouble(line.Substring(6, 16));
                            GAUT[1] = Convert.ToDouble(line.Substring(22, 16));
                            GAUT[2] = Convert.ToDouble(line.Substring(39, 6));
                            GAUT[3] = Convert.ToDouble(line.Substring(46, 4));
                        }
                        if (line.Contains("GAGP"))
                        {
                            GAGP[0] = Convert.ToDouble(line.Substring(6, 16));
                            GAGP[1] = Convert.ToDouble(line.Substring(22, 16));
                            GAGP[2] = Convert.ToDouble(line.Substring(39, 6));
                            GAGP[3] = Convert.ToDouble(line.Substring(46, 4));
                        }
                        if (line.Contains("LEAP SECONDS"))
                            LEAP_SECONDS = Convert.ToInt32(line.Substring(4, 2));
                        line = reader.ReadLine();
                    }
                    while(!reader.EndOfStream)
                    {
                        line= reader.ReadLine();
                        if(line.Substring(0,1)=="C"||line.Substring(0,1)=="G")
                        {
                            table2.Rows.Add();
                            table2.Rows[NumRows]["PRN"] = line.Substring(0, 3);
                            table2.Rows[NumRows]["TOC"] = line.Substring(4, 19);
                            table2.Rows[NumRows]["钟偏"] = Convert.ToDouble(line.Substring(23, 19));
                            table2.Rows[NumRows]["钟漂"] = Convert.ToDouble(line.Substring(42, 19));
                            table2.Rows[NumRows]["频漂"] = Convert.ToDouble(line.Substring(61, 19));
                            for (int i=0;i<7;i++)
                            {
                                line = reader.ReadLine();
                                if(i==0)
                                {
                                    table2.Rows[NumRows]["IODE"] = line.Substring(4, 19);
                                    table2.Rows[NumRows]["Crs"] = Convert.ToDouble(line.Substring(23, 19));
                                    table2.Rows[NumRows]["·n"] = Convert.ToDouble(line.Substring(42, 19));
                                    table2.Rows[NumRows]["M0"] = Convert.ToDouble(line.Substring(61, 19));
                                }
                                if(i==1)
                                {
                                    table2.Rows[NumRows]["Cuc"] = line.Substring(4, 19);
                                    table2.Rows[NumRows]["e"] = Convert.ToDouble(line.Substring(23, 19));
                                    table2.Rows[NumRows]["Cus"] = Convert.ToDouble(line.Substring(42, 19));
                                    table2.Rows[NumRows]["Sqrt_A"] = Convert.ToDouble(line.Substring(61, 19));
                                }
                                if (i == 2)
                                {
                                    table2.Rows[NumRows]["TOE"] = line.Substring(4, 19);
                                    table2.Rows[NumRows]["Cic"] = Convert.ToDouble(line.Substring(23, 19));
                                    table2.Rows[NumRows]["OMEGA"] = Convert.ToDouble(line.Substring(42, 19));
                                    table2.Rows[NumRows]["Cis"] = Convert.ToDouble(line.Substring(61, 19));
                                }
                                if (i == 3)
                                {
                                    table2.Rows[NumRows]["i0"] = line.Substring(4, 19);
                                    table2.Rows[NumRows]["Crc"] = Convert.ToDouble(line.Substring(23, 19));
                                    table2.Rows[NumRows]["omega"] = Convert.ToDouble(line.Substring(42, 19));
                                    table2.Rows[NumRows]["d_omega"] = Convert.ToDouble(line.Substring(61, 19));
                                }
                                if (i == 4)
                                {
                                    table2.Rows[NumRows]["d_i"] = line.Substring(4, 19);
                                    table2.Rows[NumRows]["L2_Code"] = Convert.ToDouble(line.Substring(23, 19));
                                    table2.Rows[NumRows]["GPS_week"] = Convert.ToDouble(line.Substring(42, 19));
                                    if(line.Length > 61)
                                        table2.Rows[NumRows]["L2_P_Code"] = Convert.ToDouble(line.Substring(61, 19));
                                }
                                if (i == 5)
                                {
                                    table2.Rows[NumRows]["Accu"] = line.Substring(4, 19);
                                    table2.Rows[NumRows]["Health"] = Convert.ToDouble(line.Substring(23, 19));
                                    table2.Rows[NumRows]["TGD"] = Convert.ToDouble(line.Substring(42, 19));
                                    table2.Rows[NumRows]["IODC_age"] = Convert.ToDouble(line.Substring(61, 19));
                                }
                                if (i == 6)
                                {
                                    table2.Rows[NumRows]["time_sent"] = line.Substring(4, 19);
                                    table2.Rows[NumRows]["nihequjian"] = Convert.ToDouble(line.Substring(23, 19));
                                }
                            }
                            NumRows++;
                        }
                    }
                }
                dataGridView2.DataSource = table2;
                dataGridView2.Sort(dataGridView2.Columns[0], ListSortDirection.Ascending);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// 计算卫星位置并进行伪距单点定位
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void 解算ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //多线程
            timer1.Enabled = true;
            thread1 = new Thread(new ThreadStart(Sat_postion));
            thread1.IsBackground = true;
            thread1.Start();
            //for (int i = 0; i <= 100; i++)
            //{
            //    Thread.Sleep(100);
            //    //启动进度条
            //    StartProgressBar(progressBar1, i, label1);

            //}
            thread2 = new Thread(new ThreadStart(Approx_postion));
            thread2.IsBackground = true;
            thread2.Start();

        }

        private delegate void InvokeHandler();

        void Sat_postion()
        {
            Control.CheckForIllegalCrossThreadCalls = false;
            ///1.计算卫星位置
            //计算时间
            double[] cal = new double[6];
            cal[0] = Convert.ToDouble(TIME_OF_FIRST_OBS.Substring(2, 4));
            cal[1] = Convert.ToDouble(TIME_OF_FIRST_OBS.Substring(10, 2));
            cal[2] = Convert.ToDouble(TIME_OF_FIRST_OBS.Substring(16, 2));
            cal[3] = Convert.ToDouble(TIME_OF_FIRST_OBS.Substring(22, 2));
            cal[4] = Convert.ToDouble(TIME_OF_FIRST_OBS.Substring(28, 2));
            cal[5] = Convert.ToDouble(TIME_OF_FIRST_OBS.Substring(33, 10));
            cal2gps.Cal2gps(cal, out double TIME_OF_FIRST_OBS_week, out double TIME_OF_FIRST_OBS_second);
            cal = new double[6];
            cal[0] = Convert.ToDouble(TIME_OF_LAST_OBS.Substring(2, 4));
            cal[1] = Convert.ToDouble(TIME_OF_LAST_OBS.Substring(10, 2));
            cal[2] = Convert.ToDouble(TIME_OF_LAST_OBS.Substring(16, 2));
            cal[3] = Convert.ToDouble(TIME_OF_LAST_OBS.Substring(22, 2));
            cal[4] = Convert.ToDouble(TIME_OF_LAST_OBS.Substring(28, 2));
            cal[5] = Convert.ToDouble(TIME_OF_LAST_OBS.Substring(33, 10));
            cal2gps.Cal2gps(cal, out double TIME_OF_LAST_OBS_week, out double TIME_OF_LAST_OBS_second);
            int NumTime = Convert.ToInt32((TIME_OF_LAST_OBS_second - TIME_OF_FIRST_OBS_second) / INTERVAL) + 1;          
            double[] t = new double[NumTime];//所示任意时刻
            string[] timestrings = new string[NumTime];
            double[] week = new double[NumTime];
            cal = new double[6];
            int rows = 0;
            for (int i = 0; i < table1.Rows.Count - 1; i += Convert.ToInt32(table1.Rows[i]["卫星数"]))
            {
                cal[0] = Convert.ToDouble(table1.Rows[i]["时间"].ToString().Substring(0, 4));
                cal[1] = Convert.ToDouble(table1.Rows[i]["时间"].ToString().Substring(5, 2));
                cal[2] = Convert.ToDouble(table1.Rows[i]["时间"].ToString().Substring(8, 2));
                cal[3] = Convert.ToDouble(table1.Rows[i]["时间"].ToString().Substring(11, 2));
                cal[4] = Convert.ToDouble(table1.Rows[i]["时间"].ToString().Substring(14, 2));
                cal[5] = Convert.ToDouble(table1.Rows[i]["时间"].ToString().Substring(17, 10));
                cal2gps.Cal2gps(cal, out week[rows], out t[rows]);
                timestrings[rows] = table1.Rows[i]["时间"].ToString();
                rows++;
            }
            //计算每个历元最接近的星历
            //t[]为任意时间
            double MinTime;
            int[] MinTimeRows;
            int NumSat = 0;
            //为表格添加足够行
            for (int i = 0; i < NumTime; i++)
            {
                rows = 0;
                for (int j = 0; j < Convert.ToInt32(table1.Rows[NumSat]["卫星数"]); j++)
                {
                    table3.Rows.Add();
                    rows++;
                }
                NumSat += rows;
            }
            rows = 0;
            NumSat = 0;
            for (int i = 0; i < NumTime; i++)
            {
                rows = 0;//表格table2  行数用Numsat+rows检索
                //第i个时刻的卫星数在table2的最短时间间隔行数
                MinTimeRows = new int[Convert.ToInt32(table1.Rows[NumSat]["卫星数"])];
                for (int j = 0; j < Convert.ToInt32(table1.Rows[NumSat]["卫星数"]); j++)
                {
                    MinTime = 86400;//最小值赋值为最大
                    for (int l = 0; l < table2.Rows.Count; l++)
                    {
                        if (table2.Rows[l]["PRN"].ToString() == table1.Rows[j + NumSat]["PRN"].ToString())
                        {
                            if (Math.Abs(t[i] - Convert.ToDouble(table2.Rows[l]["TOE"])) < MinTime)
                            {
                                MinTime = Math.Abs(t[i] - Convert.ToDouble(table2.Rows[l]["TOE"]));
                                MinTimeRows[j] = l;
                            }
                            else
                                continue;
                        }
                    }
                    //第一个时刻一共这么多颗卫星，分别计算每颗卫星的这个xyz
                    //table3.Rows.Add();
                    table3.Rows[NumSat]["时间"] = timestrings[i];
                    table3.Rows[NumSat + rows]["PRN"] = table1.Rows[j + NumSat]["PRN"].ToString();
                    table3.Rows[NumSat]["卫星数"] = Convert.ToInt32(table1.Rows[NumSat]["卫星数"]);
                    table3.Rows[NumSat + rows]["X"] = MinTimeRows[j];
                    //01计算卫星运动的平均角速度
                    double GM = 0;
                    if (table1.Rows[j + NumSat]["PRN"].ToString().Substring(0, 1) == "G")
                        GM = 3.986005E14;
                    else if (table1.Rows[j + NumSat]["PRN"].ToString().Substring(0, 1) == "C")
                        GM = 3.986004418E14;
                    double sqrt_A = Convert.ToDouble(table2.Rows[MinTimeRows[j]]["Sqrt_A"]);
                    double n0 = Math.Sqrt(GM / (sqrt_A * sqrt_A * sqrt_A * sqrt_A * sqrt_A * sqrt_A));
                    //001计算观测时刻的平均角速度n
                    double n = n0 + Convert.ToDouble(table2.Rows[MinTimeRows[j]]["·n"]);
                    //02计算观测瞬间卫星的平近点角M
                    double dt = t[i] - Convert.ToDouble(table2.Rows[MinTimeRows[j]]["TOE"]);
                    if (dt > 302400)
                        dt -= 604800;
                    else if (dt < 302400)
                        dt += 604800;
                    double M = Convert.ToDouble(table2.Rows[MinTimeRows[j]]["M0"]) + n * dt;
                    //03计算偏近点角
                    double E0 = M;
                    int count = 0;
                    double E = 0;
                    while (count < 8)
                    {
                        E = M + Convert.ToDouble(table2.Rows[MinTimeRows[j]]["e"]) * Math.Sin(E0);
                        E0 = E;
                        count++;
                    }
                    double f = Math.Atan2(Math.Sqrt(1 - Convert.ToDouble(table2.Rows[MinTimeRows[j]]["e"]) * Convert.ToDouble(table2.Rows[MinTimeRows[j]]["e"])) * Math.Sin(E), ((Math.Cos(E) - Convert.ToDouble(table2.Rows[MinTimeRows[j]]["e"]))));
                    //05计算升交距角
                    double u_p = Convert.ToDouble(table2.Rows[MinTimeRows[j]]["omega"]) + f;
                    //06计算卫星向径
                    double r_p = sqrt_A * sqrt_A * (1 - Convert.ToDouble(table2.Rows[MinTimeRows[j]]["e"]) * Math.Cos(E));
                    //07计算摄动改正项
                    double delta_u = Convert.ToDouble(table2.Rows[MinTimeRows[j]]["Cuc"]) * Math.Cos(2 * u_p) + Convert.ToDouble(table2.Rows[MinTimeRows[j]]["Cus"]) * Math.Sin(2 * u_p);
                    double delta_r = Convert.ToDouble(table2.Rows[MinTimeRows[j]]["Crc"]) * Math.Cos(2 * u_p) + Convert.ToDouble(table2.Rows[MinTimeRows[j]]["Crs"]) * Math.Sin(2 * u_p);
                    double delta_i = Convert.ToDouble(table2.Rows[MinTimeRows[j]]["Cic"]) * Math.Cos(2 * u_p) + Convert.ToDouble(table2.Rows[MinTimeRows[j]]["Cis"]) * Math.Sin(2 * u_p);
                    //08进行摄动改正
                    double u = u_p + delta_u;
                    double r = r_p + delta_r;
                    double I = Convert.ToDouble(table2.Rows[MinTimeRows[j]]["i0"]) + delta_i + Convert.ToDouble(table2.Rows[MinTimeRows[j]]["d_i"]) * dt;
                    //09计算卫星在轨道平面上的位置
                    double x = r * Math.Cos(u);
                    double y = r * Math.Sin(u);
                    //10计算开交点的经度
                    double omiga_e = 7.292115E-5;
                    double L = 0;
                    if (table1.Rows[j + NumSat]["PRN"].ToString().Substring(0, 1) == "G")
                    {
                        L = Convert.ToDouble(table2.Rows[MinTimeRows[j]]["OMEGA"]) + Convert.ToDouble(table2.Rows[MinTimeRows[j]]["d_omega"]) * dt - omiga_e * t[i];
                        //11计算XYZ
                        table3.Rows[NumSat + rows]["X"] = x * Math.Cos(L) - y * Math.Cos(I) * Math.Sin(L);
                        table3.Rows[NumSat + rows]["Y"] = x * Math.Sin(L) + y * Math.Cos(I) * Math.Cos(L);
                        table3.Rows[NumSat + rows]["Z"] = y * Math.Sin(I);
                    }
                    else if (table1.Rows[j + NumSat]["PRN"].ToString().Substring(0, 1) == "C")
                    {
                        if (table1.Rows[j + NumSat]["PRN"].ToString() == "C06" || table1.Rows[j + NumSat]["PRN"].ToString() == "C07" || table1.Rows[j + NumSat]["PRN"].ToString() == "C08" || table1.Rows[j + NumSat]["PRN"].ToString() == "C09" || table1.Rows[j + NumSat]["PRN"].ToString() == "C10" || table1.Rows[j + NumSat]["PRN"].ToString() == "C13" || table1.Rows[j + NumSat]["PRN"].ToString() == "C16" || table1.Rows[j + NumSat]["PRN"].ToString() == "C31" || table1.Rows[j + NumSat]["PRN"].ToString() == "C38" || table1.Rows[j + NumSat]["PRN"].ToString() == "C39" || table1.Rows[j + NumSat]["PRN"].ToString() == "C10" || table1.Rows[j + NumSat]["PRN"].ToString() == "C56" || table1.Rows[j + NumSat]["PRN"].ToString() == "C57" || table1.Rows[j + NumSat]["PRN"].ToString() == "C58" || table1.Rows[j + NumSat]["PRN"].ToString() == "C11" || table1.Rows[j + NumSat]["PRN"].ToString() == "C12" || table1.Rows[j + NumSat]["PRN"].ToString() == "C14" || table1.Rows[j + NumSat]["PRN"].ToString() == "C19" || table1.Rows[j + NumSat]["PRN"].ToString() == "C10" || table1.Rows[j + NumSat]["PRN"].ToString() == "C20" || table1.Rows[j + NumSat]["PRN"].ToString() == "C21" || table1.Rows[j + NumSat]["PRN"].ToString() == "C22" || table1.Rows[j + NumSat]["PRN"].ToString() == "C23" || table1.Rows[j + NumSat]["PRN"].ToString() == "C24" || table1.Rows[j + NumSat]["PRN"].ToString() == "C25" || table1.Rows[j + NumSat]["PRN"].ToString() == "C26" || table1.Rows[j + NumSat]["PRN"].ToString() == "C27" || table1.Rows[j + NumSat]["PRN"].ToString() == "C28" || table1.Rows[j + NumSat]["PRN"].ToString() == "C29" || table1.Rows[j + NumSat]["PRN"].ToString() == "C30" || table1.Rows[j + NumSat]["PRN"].ToString() == "C32" || table1.Rows[j + NumSat]["PRN"].ToString() == "C33" || table1.Rows[j + NumSat]["PRN"].ToString() == "C34" || table1.Rows[j + NumSat]["PRN"].ToString() == "C35" || table1.Rows[j + NumSat]["PRN"].ToString() == "C36" || table1.Rows[j + NumSat]["PRN"].ToString() == "C37" || table1.Rows[j + NumSat]["PRN"].ToString() == "C41" || table1.Rows[j + NumSat]["PRN"].ToString() == "C42" || table1.Rows[j + NumSat]["PRN"].ToString() == "C43" || table1.Rows[j + NumSat]["PRN"].ToString() == "C44" || table1.Rows[j + NumSat]["PRN"].ToString() == "C45" || table1.Rows[j + NumSat]["PRN"].ToString() == "C46")
                        {
                            L = Convert.ToDouble(table2.Rows[MinTimeRows[j]]["OMEGA"]) + (Convert.ToDouble(table2.Rows[MinTimeRows[j]]["d_omega"]) - omiga_e) * dt - omiga_e * Convert.ToDouble(table2.Rows[MinTimeRows[j]]["TOE"]);
                            table3.Rows[NumSat + rows]["X"] = x * Math.Cos(L) - y * Math.Cos(I) * Math.Sin(L);
                            table3.Rows[NumSat + rows]["Y"] = x * Math.Sin(L) + y * Math.Cos(I) * Math.Cos(L);
                            table3.Rows[NumSat + rows]["Z"] = y * Math.Sin(I);
                        }
                        else if (table1.Rows[j + NumSat]["PRN"].ToString() == "C01" || table1.Rows[j + NumSat]["PRN"].ToString() == "C02" || table1.Rows[j + NumSat]["PRN"].ToString() == "C03" || table1.Rows[j + NumSat]["PRN"].ToString() == "C04" || table1.Rows[j + NumSat]["PRN"].ToString() == "C05" || table1.Rows[j + NumSat]["PRN"].ToString() == "C59" || table1.Rows[j + NumSat]["PRN"].ToString() == "C60" || table1.Rows[j + NumSat]["PRN"].ToString() == "C61")
                        {
                            L = Convert.ToDouble(table2.Rows[MinTimeRows[j]]["OMEGA"]) + Convert.ToDouble(table2.Rows[MinTimeRows[j]]["d_omega"]) * dt - omiga_e * Convert.ToDouble(table2.Rows[MinTimeRows[j]]["TOE"]);
                            table3.Rows[NumSat + rows]["X"] = (x * Math.Cos(L) - y * Math.Cos(I) * Math.Sin(L)) * Math.Cos(omiga_e * dt) + x * Math.Sin(L) + y * Math.Cos(I) * Math.Cos(L) * Math.Sin(omiga_e * dt) * Math.Cos(-5 * Math.PI / 180.0) + y * Math.Sin(I) * Math.Sin(omiga_e * dt) * Math.Sin(-5 * Math.PI / 180.0);
                            table3.Rows[NumSat + rows]["Y"] = -(x * Math.Cos(L) - y * Math.Cos(I) * Math.Sin(L)) * Math.Sin(omiga_e * dt) + x * Math.Sin(L) + y * Math.Cos(I) * Math.Cos(L) * Math.Cos(omiga_e * dt) * Math.Cos(-5 * Math.PI / 180.0) + y * Math.Sin(I) * Math.Cos(omiga_e * dt) * Math.Sin(-5 * Math.PI / 180.0);
                            table3.Rows[NumSat + rows]["Z"] = -(x * Math.Sin(L) + y * Math.Cos(I) * Math.Cos(L)) * Math.Sin(-5 * Math.PI / 180.0) + y * Math.Sin(I) * Math.Cos(-5 * Math.PI / 180.0);
                        }
                    }
                    //计算卫星钟差
                    double c = 299792458;
                    double F = -2 * Math.Sqrt(GM) / (c * c);
                    double delta_t = F * Convert.ToDouble(table2.Rows[MinTimeRows[j]]["e"]) * sqrt_A * Math.Sin(E);
                    double dtsv = Convert.ToDouble(table2.Rows[MinTimeRows[j]]["钟偏"]) +
                        Convert.ToDouble(table2.Rows[MinTimeRows[j]]["钟漂"]) * dt +
                        Convert.ToDouble(table2.Rows[MinTimeRows[j]]["频漂"]) * dt * dt +
                        delta_t - Convert.ToDouble(table2.Rows[MinTimeRows[j]]["TGD"]);
                    table3.Rows[NumSat + rows]["Clock"] = dtsv;
                    rows++;
                }
                NumSat += rows;
            } 
            this.Invoke(new InvokeHandler(delegate ()
                {
                    //dataGridView3.DataSource = null;
                    dataGridView3.DataSource = table3;
                }));


            Application.ExitThread();
        }

        private delegate void InvokeHandler2();

        void Approx_postion()
        {
            Control.CheckForIllegalCrossThreadCalls = false;
            Thread.Sleep(TimeSpan.FromSeconds(0.1));
            double[] cal = new double[6];
            cal[0] = Convert.ToDouble(TIME_OF_FIRST_OBS.Substring(2, 4));
            cal[1] = Convert.ToDouble(TIME_OF_FIRST_OBS.Substring(10, 2));
            cal[2] = Convert.ToDouble(TIME_OF_FIRST_OBS.Substring(16, 2));
            cal[3] = Convert.ToDouble(TIME_OF_FIRST_OBS.Substring(22, 2));
            cal[4] = Convert.ToDouble(TIME_OF_FIRST_OBS.Substring(28, 2));
            cal[5] = Convert.ToDouble(TIME_OF_FIRST_OBS.Substring(33, 10));
            cal2gps.Cal2gps(cal, out double TIME_OF_FIRST_OBS_week, out double TIME_OF_FIRST_OBS_second);
            cal = new double[6];
            cal[0] = Convert.ToDouble(TIME_OF_LAST_OBS.Substring(2, 4));
            cal[1] = Convert.ToDouble(TIME_OF_LAST_OBS.Substring(10, 2));
            cal[2] = Convert.ToDouble(TIME_OF_LAST_OBS.Substring(16, 2));
            cal[3] = Convert.ToDouble(TIME_OF_LAST_OBS.Substring(22, 2));
            cal[4] = Convert.ToDouble(TIME_OF_LAST_OBS.Substring(28, 2));
            cal[5] = Convert.ToDouble(TIME_OF_LAST_OBS.Substring(33, 10));
            cal2gps.Cal2gps(cal, out double TIME_OF_LAST_OBS_week, out double TIME_OF_LAST_OBS_second);
            int NumTime = Convert.ToInt32((TIME_OF_LAST_OBS_second - TIME_OF_FIRST_OBS_second) / INTERVAL) + 1;
            double[] t = new double[NumTime];//所示任意时刻
            string[] timestrings = new string[NumTime];
            double[] week = new double[NumTime];
            cal = new double[6];
            int rows = 0;
            for (int i = 0; i < table1.Rows.Count - 1; i += Convert.ToInt32(table1.Rows[i]["卫星数"]))
            {
                cal[0] = Convert.ToDouble(table1.Rows[i]["时间"].ToString().Substring(0, 4));
                cal[1] = Convert.ToDouble(table1.Rows[i]["时间"].ToString().Substring(5, 2));
                cal[2] = Convert.ToDouble(table1.Rows[i]["时间"].ToString().Substring(8, 2));
                cal[3] = Convert.ToDouble(table1.Rows[i]["时间"].ToString().Substring(11, 2));
                cal[4] = Convert.ToDouble(table1.Rows[i]["时间"].ToString().Substring(14, 2));
                cal[5] = Convert.ToDouble(table1.Rows[i]["时间"].ToString().Substring(17, 10));
                cal2gps.Cal2gps(cal, out week[rows], out t[rows]);
                timestrings[rows] = table1.Rows[i]["时间"].ToString();
                rows++;
            }
            for (int i = 0; i < NumTime; i++)
            {
                table4.Rows.Add();
            }
            for (int i = 0; i < NumTime; i++)
            {
                Matrix B;
                Matrix VL;
                Matrix PP;
                Matrix xyz;
                Matrix V;
                int Count = 0;
                int Rows = 0;
                int NumSat1 = 0;
                double sigma = 0;
                double[] APPROX_POSITION_1 = new double[4];
                double[,] vl = new double[Convert.ToInt32(table1.Rows[NumSat1]["卫星数"]), 1];
                double[,] b = new double[Convert.ToInt32(table1.Rows[NumSat1]["卫星数"]), 4];
                double[,] p = new double[Convert.ToInt32(table1.Rows[NumSat1]["卫星数"]), Convert.ToInt32(table1.Rows[NumSat1]["卫星数"])];
                //table4.Rows.Add();
                for (int j = 0; j < 3; j++)
                {
                    APPROX_POSITION_1[j] = APPROX_POSITION[j];
                }
                while (Count < 8)
                {
                    for (int j = 0; j < Convert.ToInt32(table1.Rows[NumSat1]["卫星数"]); j++)
                    {
                        double Xsat = Convert.ToDouble(table3.Rows[NumSat1 + Rows]["X"]);
                        double Ysat = Convert.ToDouble(table3.Rows[NumSat1 + Rows]["Y"]);
                        double Zsat = Convert.ToDouble(table3.Rows[NumSat1 + Rows]["Z"]);
                        double P0 = Math.Sqrt((Xsat - APPROX_POSITION_1[0]) * (Xsat - APPROX_POSITION_1[0]) + (Ysat - APPROX_POSITION_1[1]) * (Ysat - APPROX_POSITION_1[1]) + (Zsat - APPROX_POSITION_1[2]) * (Zsat - APPROX_POSITION_1[2]));
                        double P;
                        p[j, j] = 1;
                        if (table1.Rows[NumSat1 + j]["C1C/C2I"].ToString() != "")
                            P = Convert.ToDouble(table1.Rows[NumSat1 + j]["C1C/C2I"]);
                        else
                            P = 0;
                        vl[j, 0] = P - P0;
                        b[j, 0] = -(Xsat - APPROX_POSITION_1[0]) / P0;
                        b[j, 1] = -(Ysat - APPROX_POSITION_1[1]) / P0;
                        b[j, 2] = -(Zsat - APPROX_POSITION_1[2]) / P0;
                        b[j, 3] = -1;
                        Rows++;
                    }
                    B = new Matrix(b);
                    VL = new Matrix(vl);
                    PP = new Matrix(p);
                    xyz = (B.Transpose() * PP * B).Inverse() * B.Transpose() * PP * VL;
                    V = B * xyz - VL;
                    APPROX_POSITION_1[0] = xyz[0, 0];
                    APPROX_POSITION_1[1] = xyz[1, 0];
                    APPROX_POSITION_1[2] = xyz[2, 0];
                    APPROX_POSITION_1[3] = xyz[3, 0];
                    sigma = Math.Sqrt(Convert.ToDouble((V.Transpose() * PP * V)[0, 0]) / Convert.ToDouble(table1.Rows[NumSat1]["卫星数"]));
                    Count++;
                }
                table4.Rows[i]["时间"] = t[i].ToString();
                table4.Rows[i]["X"] = APPROX_POSITION_1[0];
                table4.Rows[i]["Y"] = APPROX_POSITION_1[1];
                table4.Rows[i]["Z"] = APPROX_POSITION_1[2];
                table4.Rows[i]["Clock"] = APPROX_POSITION_1[3];
                table4.Rows[i]["后验权中误差"] = APPROX_POSITION_1[0] - APPROX_POSITION[0];
                NumSat1 += Rows;

            }
             this.Invoke(new InvokeHandler(delegate ()
                {
                    //dataGridView4.DataSource = null;
                    dataGridView4.DataSource = table4;
                }));

            timer1.Enabled = false;
        }

        /// <summary>
        /// 计时器
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer1_Tick(object sender, EventArgs e)
        {
            
            if (timer1.Enabled==true)
            {
                Curcnt += 1;
                toolStripTextBox1.Text = Curcnt.ToString().Trim();
            }
        }

        ///<summary>
        ///进度条
        /// </summary>
        /// 1.启动进度条
        private void StartProgressBar(ProgressBar progressBar, int value, Label label)
        {
            if (progressBar == null || label == null) return;
            Application.DoEvents();
            progressBar.Value = value;
            int tmp = value * 100 / progressBar.Maximum;

            label.Text = tmp + "%";
            label.Refresh();

            progressBar.Refresh();
        }


   }

    /// <summary>
    /// 双缓冲
    /// </summary>
    public static class DoubleBufferDataGridView
    {
        /// <summary>
        /// 双缓冲，解决闪烁问题
        /// </summary>
        public static void DoubleBufferedDataGirdView(this DataGridView dgv, bool flag)
        {
            Type dgvType = dgv.GetType();
            PropertyInfo pi = dgvType.GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
            pi.SetValue(dgv, flag, null);
        }
    }
}
