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
using System.Text.RegularExpressions;
using System.Windows.Forms.DataVisualization.Charting;

namespace 卫星轨道计算
{
    public partial class Form1 : Form
    {
        double APPROX_POSITION_X;//接收机位置
        double APPROX_POSITION_Y;
        double APPROX_POSITION_Z;
        double INTERVAL;//采样间隔
        double ANTENNA_DELTA_H;
        double ANTENNA_DELTA_E;
        double ANTENNA_DELTA_N;
        double c = 299792458;
        double L1 = 1575.42e6;
        double L2 = 1227.60e6;
        double B1 = 1561.098e6;
        double B3 = 1268.52e6;
         double alpha_0, alpha_1, alpha_2, alpha_3;//电离层延迟改正参数
        double beta_0, beta_1, beta_2, beta_3;//同上
        double A0, A1, U, W;
        double leap_seconds;
        double GM_GPS = 3.986005E14;
        double GM_BDS = 3.986004418E14;
        int GPS_num = 0;
        int BDS_num = 0;
        string result = null;//结果文件的输出
        int time = 900;//time 为时间间隔 此处以900秒为间隔
        int GPS_Sat_num;
        int BDS_Sat_num;
        ///GPS卫星
        //PRN/历元/...
        string[] PRN_GPS;
        //double[,] Data_GPS;
        double[] TOC_Year_GPS;
        double[] TOC_Month_GPS;
        double[] TOC_Day_GPS;
        double[] TOC_Hour_GPS;
        double[] TOC_Min_GPS;
        double[] TOC_Sec_GPS;
        double[] Clock_error_GPS;
        double[] Clock_drift_GPS;
        double[] Clock_offset_GPS;
        //广播轨道 1
        double[] IODE_GPS;
        double[] Crs_GPS;
        double[] delta_n_GPS;
        double[] M0_GPS;
        //广播轨道 2
        double[] Cuc_GPS;
        double[] e_GPS;
        double[] Cus_GPS;
        double[] sqrt_A_GPS;
        //广播轨道 3
        double[] TOE_GPS;
        double[] Cic_GPS;
        double[] Omega_GPS;
        double[] Cis_GPS;
        //广播轨道 4
        double[] i0_GPS;
        double[] Crc_GPS;
        double[] omega_GPS;
        double[] d_Omega_GPS;
        //广播轨道 5
        double[] d_i_GPS;
        double[] L2_code_GPS;
        double[] GPS_week_GPS;
        double[] L2_P_code_GPS;
        //广播轨道 6
        double[] Sat_accu_GPS;
        double[] Sat_health_GPS;
        double[] TGD_GPS;
        double[] IODC_age_GPS;
        //广播轨道 7
        double[] time_sent_GPS;
        double[] nihequjian_GPS;
        
///BDS卫星
        
        //PRN/历元/...
        string[] PRN_BDS;
        //double[,] Data_BDS;
        double[] TOC_Year_BDS;
        double[] TOC_Month_BDS;
        double[] TOC_Day_BDS;
        double[] TOC_Hour_BDS;
        double[] TOC_Min_BDS;
        double[] TOC_Sec_BDS;
        double[] Clock_error_BDS;
        double[] Clock_drift_BDS;
        double[] Clock_offset_BDS;
        //广播轨道 1
        double[] IODE_BDS;
        double[] Crs_BDS;
        double[] delta_n_BDS;
        double[] M0_BDS;
        //广播轨道 2
        double[] Cuc_BDS;
        double[] e_BDS;
        double[] Cus_BDS;
        double[] sqrt_A_BDS;
        //广播轨道 3
        double[] TOE_BDS;
        double[] Cic_BDS;
        double[] Omega_BDS;
        double[] Cis_BDS;
        //广播轨道 4
        double[] i0_BDS;
        double[] Crc_BDS;
        double[] omega_BDS;
        double[] d_Omega_BDS;
        //广播轨道 5
        double[] d_i_BDS;
        double[] L2_code_BDS;
        double[] GPS_week_BDS;
        double[] L2_P_code_BDS;
        //广播轨道 6
        double[] Sat_accu_BDS;
        double[] Sat_health_BDS;
        double[] TGD_BDS;
        double[] IODC_age_BDS;

       
        //广播轨道 7
        double[] time_sent_BDS;        
        double[] nihequjian_BDS;

        /// <summary>
        /// 读取观测值文件
        /// </summary>
        double[,] G02;
        double[,] G05;
        double[,] G13;
        double[,] G12;
        double[,] G15;
        double[,] G21;

        double[,] C01;
        double[,] C03;//GEO
        double[,] C06;
        double[,] C08;//IGSO
        double[,] C11;
        double[,] C12;//MEO

        DataTable Table;

        DataTable Table1;

        DataTable Table2;
        public Form1()
        {
            Table = new DataTable();
            Table1 = new DataTable();
            Table2 = new DataTable();
            Table.Columns.Add("Year", typeof(double));
            Table.Columns.Add("Month", typeof(double));
            Table.Columns.Add("Day", typeof(double));
            Table.Columns.Add("Hour", typeof(double));
            Table.Columns.Add("Minute", typeof(double));
            Table.Columns.Add("Second", typeof(double));
            Table.Columns.Add("PRN", typeof(string));
            Table.Columns.Add("X", typeof(double));
            Table.Columns.Add("Y", typeof(double));
            Table.Columns.Add("Z", typeof(double));
            Table.Columns.Add("Xsp3", typeof(double));
            Table.Columns.Add("Ysp3", typeof(double));
            Table.Columns.Add("Zsp3", typeof(double));
            Table.Columns.Add("dX", typeof(double));
            Table.Columns.Add("dY", typeof(double));
            Table.Columns.Add("dZ", typeof(double));

            Table1.Columns.Add("历元数", typeof(double));
            Table1.Columns.Add("PRN", typeof(string));
            Table1.Columns.Add("GF", typeof(double));
            Table1.Columns.Add("MW", typeof(double));
            Table1.Columns.Add("GF历元间求差", typeof(double));
            Table1.Columns.Add("MW历元间求差", typeof(double));

            Table2.Columns.Add("历元", typeof(double));
            //Table2.Columns.Add("Year", typeof(double));
            //Table2.Columns.Add("Month", typeof(double));
            //Table2.Columns.Add("Day", typeof(double));
            //Table2.Columns.Add("Hour", typeof(double));
            //Table2.Columns.Add("Minute", typeof(double));
            //Table2.Columns.Add("Second", typeof(double));
            Table2.Columns.Add("PRN", typeof(string));
            Table2.Columns.Add("X", typeof(double));
            Table2.Columns.Add("Y", typeof(double));
            Table2.Columns.Add("Z", typeof(double));
            Table2.Columns.Add("Clock", typeof(double));
            InitializeComponent();
        }

        
        private void 打开nToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                string line = null;
                alpha_0 = 0; alpha_1 = 0; alpha_2 = 0; alpha_3 = 0;
                beta_0 = 0; beta_1 = 0; beta_2 = 0; beta_3 = 0;
                A0 = 0;A1 = 0;U = 0;W = 0;
                leap_seconds = 0;
                GPS_num = 0;
                string[] line_GPS = new string[8];
                //GPS
                //PRN/历元/...
                PRN_GPS = new string[1000];
                TOC_Year_GPS = new double[1000];
                TOC_Month_GPS = new double[1000];
                TOC_Day_GPS = new double[1000];
                TOC_Hour_GPS = new double[1000];
                TOC_Min_GPS = new double[1000];
                TOC_Sec_GPS = new double[1000];
                Clock_error_GPS = new double[1000];
                Clock_drift_GPS = new double[1000];
                Clock_offset_GPS = new double[1000];
                //广播轨道 1
                IODE_GPS = new double[1000];
                Crs_GPS = new double[1000];
                delta_n_GPS = new double[1000];
                M0_GPS = new double[1000];
                //广播轨道 2
                Cuc_GPS = new double[1000];
                e_GPS = new double[1000];
                Cus_GPS = new double[1000];
                sqrt_A_GPS = new double[1000];
                //广播轨道 3
                TOE_GPS = new double[1000];
                Cic_GPS = new double[1000];
                Omega_GPS = new double[1000];
                Cis_GPS = new double[1000];
                //广播轨道 4
                i0_GPS = new double[1000];
                Crc_GPS = new double[1000];
                omega_GPS = new double[1000];
                d_Omega_GPS = new double[1000];
                //广播轨道 5
                d_i_GPS = new double[1000];
                L2_code_GPS = new double[1000];
                GPS_week_GPS = new double[1000];
                L2_P_code_GPS = new double[1000];
                //广播轨道 6
                Sat_accu_GPS = new double[1000];
                Sat_health_GPS = new double[1000];
                TGD_GPS = new double[1000];
                IODC_age_GPS = new double[1000];
                //广播轨道 7
                time_sent_GPS = new double[1000];
                nihequjian_GPS = new double[1000];               
                if (openFileDialog3.ShowDialog() == DialogResult.OK)
                {
                    StreamReader reader = new StreamReader(openFileDialog3.FileName);
                    openFileDialog3.Filter = "All(*.*)|*.*|nav(*.n)|*.n|txt(*.txt)|*.txt|tar(*.tar)|*.tar";
                    line = reader.ReadLine();
                    while (!line.Contains("END OF HEADER"))
                    {
                        line.Replace("D", "E");
                        if (line.Contains("ION ALPHA"))
                        {
                            line = line.Replace('D', 'E');
                            alpha_0 = Convert.ToDouble(line.Substring(3, 11));
                            alpha_1 = Convert.ToDouble(line.Substring(15, 11));
                            alpha_2 = Convert.ToDouble(line.Substring(27, 11));
                            alpha_3 = Convert.ToDouble(line.Substring(39, 11));
                        }
                        if (line.Contains("ION BETA"))
                        {
                            line = line.Replace('D', 'E');
                            beta_0 = Convert.ToDouble(line.Substring(3, 11));
                            beta_1 = Convert.ToDouble(line.Substring(15, 11));
                            beta_2 = Convert.ToDouble(line.Substring(27, 11));
                            beta_3 = Convert.ToDouble(line.Substring(39, 11));
                        }
                        if(line.Contains("DELTA-UTC"))
                        {
                            line = line.Replace('D', 'E');
                            A0 = Convert.ToDouble(line.Substring(4, 18));
                            A1 = Convert.ToDouble(line.Substring(23, 18));
                            U = Convert.ToDouble(line.Substring(44, 6));
                            W = Convert.ToDouble(line.Substring(55, 4));
                        }
                        if (line.Contains("LEAP SECONDS"))
                        {
                            leap_seconds = Convert.ToDouble(line.Substring(0, 60));
                        }

                        line = reader.ReadLine();
                    }
                    //richTextBox2.Text = Convert.ToString(A0) + '\n' + Convert.ToString(A1) + '\n' + Convert.ToString(U) + '\n' + Convert.ToString(W)+'\n';
                    while(!reader.EndOfStream)
                    {
                        for (int i = 0; i < 8; i++)
                        {
                            line = reader.ReadLine();
                            line = line.Replace('D', 'E');
                            line_GPS[i] = line;                            
                        }
                        PRN_GPS[GPS_num] = line_GPS[0].Substring(0, 2);
                        TOC_Year_GPS[GPS_num] = Convert.ToDouble(line_GPS[0].Substring(3, 2));//TOC 年
                        TOC_Month_GPS[GPS_num] = Convert.ToDouble(line_GPS[0].Substring(6, 2));//TOC 月
                        TOC_Day_GPS[GPS_num] = Convert.ToDouble(line_GPS[0].Substring(9, 2));// TOC 日
                        TOC_Hour_GPS[GPS_num] = Convert.ToDouble(line_GPS[0].Substring(12, 2));//TOC 时
                        TOC_Min_GPS[GPS_num] = Convert.ToDouble(line_GPS[0].Substring(15, 2));// TOC 分
                        TOC_Sec_GPS[GPS_num] = Convert.ToDouble(line_GPS[0].Substring(18, 4));//TOC 秒
                        Clock_error_GPS[GPS_num] = Convert.ToDouble(line_GPS[0].Substring(22, 19));// 钟差
                        Clock_drift_GPS[GPS_num] = Convert.ToDouble(line_GPS[0].Substring(41, 19));//钟漂
                        Clock_offset_GPS[GPS_num] = Convert.ToDouble(line_GPS[0].Substring(60, 19));//频偏
                        IODE_GPS[GPS_num] = Convert.ToDouble(line_GPS[1].Substring(3, 19));// 数据，星历发布时间
                        Crs_GPS[GPS_num] = Convert.ToDouble(line_GPS[1].Substring(22, 19));// r改正参数
                        delta_n_GPS[GPS_num] = Convert.ToDouble(line_GPS[1].Substring(41, 19));// δn
                        M0_GPS[GPS_num] = Convert.ToDouble(line_GPS[1].Substring(60, 19));// M0
                        Cuc_GPS[GPS_num] = Convert.ToDouble(line_GPS[2].Substring(3, 19));// u 改正数
                        e_GPS[GPS_num] = Convert.ToDouble(line_GPS[2].Substring(22, 19));// 轨道偏心率
                        Cus_GPS[GPS_num] = Convert.ToDouble(line_GPS[2].Substring(41, 19));// u 改正数
                        sqrt_A_GPS[GPS_num] = Convert.ToDouble(line_GPS[2].Substring(60, 19));// 根号A
                        TOE_GPS[GPS_num] = Convert.ToDouble(line_GPS[3].Substring(3, 19));//参考时刻
                        Cic_GPS[GPS_num] = Convert.ToDouble(line_GPS[3].Substring(22, 19));// i
                        Omega_GPS[GPS_num] = Convert.ToDouble(line_GPS[3].Substring(41, 19));// Omega
                        Cis_GPS[GPS_num] = Convert.ToDouble(line_GPS[3].Substring(60, 19));// i
                        i0_GPS[GPS_num] = Convert.ToDouble(line_GPS[4].Substring(3, 19));// i0
                        Crc_GPS[GPS_num] = Convert.ToDouble(line_GPS[4].Substring(22, 19));//r
                        omega_GPS[GPS_num] = Convert.ToDouble(line_GPS[4].Substring(41, 19));//omega
                        d_Omega_GPS[GPS_num] = Convert.ToDouble(line_GPS[4].Substring(60, 19));//Omega Dot
                        d_i_GPS[GPS_num] = Convert.ToDouble(line_GPS[5].Substring(3, 19));// d_i
                        L2_code_GPS[GPS_num] = Convert.ToDouble(line_GPS[5].Substring(22, 19));// L2上的码
                        GPS_week_GPS[GPS_num] = Convert.ToDouble(line_GPS[5].Substring(41, 19));//GPS周
                        L2_P_code_GPS[GPS_num] = Convert.ToDouble(line_GPS[5].Substring(60, 19));//L2上P码数据标记
                        Sat_accu_GPS[GPS_num] = Convert.ToDouble(line_GPS[6].Substring(3, 19));//卫星精度
                        Sat_health_GPS[GPS_num] = Convert.ToDouble(line_GPS[6].Substring(22, 19));//卫星健康
                        TGD_GPS[GPS_num] = Convert.ToDouble(line_GPS[6].Substring(41, 19));// 群延迟
                        IODC_age_GPS[GPS_num] = Convert.ToDouble(line_GPS[6].Substring(60, 19));//数据龄期
                        time_sent_GPS[GPS_num] = Convert.ToDouble(line_GPS[7].Substring(3, 19));//电文发送时刻
                        nihequjian_GPS[GPS_num] = Convert.ToDouble(line_GPS[6].Substring(22, 19));//拟合区间
                        GPS_num++;
                    }
                    reader.Close();
                    //for(int i=0;i<GPS_num;i++)
                    //{
                    //    richTextBox2.Text += Convert.ToString(IODE_GPS[i]) + '\n';
                    //}
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(Convert.ToString(ex));
            }
        }
        
        private void 打开pToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                string line = null;
                alpha_0 = 0; alpha_1 = 0; alpha_2 = 0; alpha_3 = 0;
                beta_0 = 0; beta_1 = 0; beta_2 = 0; beta_3 = 0;
                leap_seconds = 0;
                string[] line_GPS = new string[8];
                string[] line_BDS = new string[8];
            //GPS
                //PRN/历元/...
                PRN_GPS = new string[1000];
                TOC_Year_GPS = new double[1000];
                TOC_Month_GPS = new double[1000];
                TOC_Day_GPS = new double[1000];
                TOC_Hour_GPS = new double[1000];
                TOC_Min_GPS = new double[1000];
                TOC_Sec_GPS = new double[1000];
                Clock_error_GPS = new double[1000];
                Clock_drift_GPS = new double[1000];
                Clock_offset_GPS = new double[1000];
                //广播轨道 1
                IODE_GPS = new double[1000];
                Crs_GPS = new double[1000];
                delta_n_GPS = new double[1000];
                M0_GPS = new double[1000];
                //广播轨道 2
                Cuc_GPS = new double[1000];
                e_GPS = new double[1000];
                Cus_GPS = new double[1000];
                sqrt_A_GPS = new double[1000];
                //广播轨道 3
                TOE_GPS = new double[1000];
                Cic_GPS = new double[1000];
                Omega_GPS = new double[1000];
                Cis_GPS = new double[1000];
                //广播轨道 4
                i0_GPS = new double[1000];
                Crc_GPS = new double[1000];
                omega_GPS = new double[1000];
                d_Omega_GPS = new double[1000];
                //广播轨道 5
                d_i_GPS = new double[1000];
                L2_code_GPS = new double[1000];
                GPS_week_GPS = new double[1000];
                L2_P_code_GPS = new double[1000];
                //广播轨道 6
                Sat_accu_GPS = new double[1000];
                Sat_health_GPS = new double[1000];
                TGD_GPS = new double[1000];
                IODC_age_GPS = new double[1000];
                //广播轨道 7
                time_sent_GPS = new double[1000];
                nihequjian_GPS = new double[1000];
            ///BDS
                
                //PRN/历元/...
                PRN_BDS = new string[1000];
                
                TOC_Year_BDS = new double[1000];
                TOC_Month_BDS = new double[1000];
                TOC_Day_BDS = new double[1000];
                TOC_Hour_BDS = new double[1000];
                TOC_Min_BDS = new double[1000];
                TOC_Sec_BDS = new double[1000];
                Clock_error_BDS = new double[1000];
                Clock_drift_BDS = new double[1000];
                Clock_offset_BDS = new double[1000];
                //广播轨道 1
                IODE_BDS = new double[1000];
                Crs_BDS = new double[1000];
                delta_n_BDS = new double[1000];
                M0_BDS = new double[1000];
                //广播轨道 2
                Cuc_BDS = new double[1000];
                e_BDS = new double[1000];
                Cus_BDS = new double[1000];
                sqrt_A_BDS = new double[1000];
                //广播轨道 3
                TOE_BDS = new double[1000];
                Cic_BDS = new double[1000];
                Omega_BDS = new double[1000];
                Cis_BDS = new double[1000];
                //广播轨道 4
                i0_BDS = new double[1000];
                Crc_BDS = new double[1000];
                omega_BDS = new double[1000];
                d_Omega_BDS = new double[1000];
                //广播轨道 5
                d_i_BDS = new double[1000];
                L2_code_BDS = new double[1000];
                GPS_week_BDS = new double[1000];
                L2_P_code_BDS = new double[1000];
                //广播轨道 6
                Sat_accu_BDS = new double[1000];
                Sat_health_BDS = new double[1000];
                TGD_BDS = new double[1000];
                IODC_age_BDS = new double[1000];
                //广播轨道 7
                time_sent_BDS = new double[1000];
                nihequjian_BDS = new double[1000];
                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    StreamReader reader = new StreamReader(openFileDialog1.FileName);
                    openFileDialog1.Filter = "All(*.*)|*.*|nav(*.p)|*.p|txt(*.txt)|*.txt|tar(*.tar)|*.tar";
                    line = reader.ReadLine();
                    while (!line.Contains("END OF HEADER"))
                    {
                        //line.Replace("D", "E");
                        if (line.Contains("ION ALPHA"))
                        {
                            line = line.Replace('D', 'E');
                            alpha_0 = Convert.ToDouble(line.Substring(3, 11));
                            alpha_1 = Convert.ToDouble(line.Substring(15, 11));
                            alpha_2 = Convert.ToDouble(line.Substring(27, 11));
                            alpha_3 = Convert.ToDouble(line.Substring(39, 11));
                        }
                        if (line.Contains("ION BETA"))
                        {
                            line = line.Replace('D', 'E');
                            beta_0 = Convert.ToDouble(line.Substring(3, 11));
                            beta_1 = Convert.ToDouble(line.Substring(15, 11));
                            beta_2 = Convert.ToDouble(line.Substring(27, 11));
                            beta_3 = Convert.ToDouble(line.Substring(39, 11));
                        }
                        if (line.Contains("LEAP SECONDS"))
                        {
                            leap_seconds = Convert.ToDouble(line.Substring(0, 60));
                        }
                        
                        line = reader.ReadLine();
                    }
                   
                    line = reader.ReadLine();
                    do
                    {
                        line = line.Replace('D', 'E');
                        for(int i=0;i<8;i++)
                        {
                            line_GPS[i] = line;
                            line = line.Replace('D', 'E');
                            //richTextBox1.Text += line + '\n';
                            line = reader.ReadLine();
                        }
                        PRN_GPS[GPS_num] = line_GPS[0].Substring(0, 3);
                        TOC_Year_GPS[GPS_num] = Convert.ToDouble(line_GPS[0].Substring(4, 4));//TOC 年
                        TOC_Month_GPS[GPS_num] = Convert.ToDouble(line_GPS[0].Substring(9, 2));//TOC 月
                        TOC_Day_GPS[GPS_num] = Convert.ToDouble(line_GPS[0].Substring(12, 2));// TOC 日
                        TOC_Hour_GPS[GPS_num] = Convert.ToDouble(line_GPS[0].Substring(15, 2));//TOC 时
                        TOC_Min_GPS[GPS_num] = Convert.ToDouble(line_GPS[0].Substring(18, 2));// TOC 分
                        TOC_Sec_GPS[GPS_num] = Convert.ToDouble(line_GPS[0].Substring(21, 2));//TOC 秒
                        Clock_error_GPS[GPS_num] = Convert.ToDouble(line_GPS[0].Substring(23, 19));// 钟差
                        Clock_drift_GPS[GPS_num] = Convert.ToDouble(line_GPS[0].Substring(42, 19));//钟漂
                        Clock_offset_GPS[GPS_num] = Convert.ToDouble(line_GPS[0].Substring(61, 19));//频偏
                        IODE_GPS[GPS_num] = Convert.ToDouble(line_GPS[1].Substring(4, 19));// 数据，星历发布时间
                        Crs_GPS[GPS_num] = Convert.ToDouble(line_GPS[1].Substring(23, 19));// r改正参数
                        delta_n_GPS[GPS_num] = Convert.ToDouble(line_GPS[1].Substring(42, 19));// δn
                        M0_GPS[GPS_num] = Convert.ToDouble(line_GPS[1].Substring(61, 19));// M0
                        Cuc_GPS[GPS_num] = Convert.ToDouble(line_GPS[2].Substring(4, 19));// u 改正数
                        e_GPS[GPS_num] = Convert.ToDouble(line_GPS[2].Substring(23, 19));// 轨道偏心率
                        Cus_GPS[GPS_num] = Convert.ToDouble(line_GPS[2].Substring(42, 19));// u 改正数
                        sqrt_A_GPS[GPS_num] = Convert.ToDouble(line_GPS[2].Substring(61, 19));// 根号A
                        TOE_GPS[GPS_num] = Convert.ToDouble(line_GPS[3].Substring(4, 19));//参考时刻
                        Cic_GPS[GPS_num] = Convert.ToDouble(line_GPS[3].Substring(23, 19));// i
                        Omega_GPS[GPS_num] = Convert.ToDouble(line_GPS[3].Substring(42, 19));// Omega
                        Cis_GPS[GPS_num] = Convert.ToDouble(line_GPS[3].Substring(61, 19));// i
                        i0_GPS[GPS_num] = Convert.ToDouble(line_GPS[4].Substring(4, 19));// i0
                        Crc_GPS[GPS_num] = Convert.ToDouble(line_GPS[4].Substring(23, 19));//r
                        omega_GPS[GPS_num] = Convert.ToDouble(line_GPS[4].Substring(42, 19));//omega
                        d_Omega_GPS[GPS_num] = Convert.ToDouble(line_GPS[4].Substring(61, 19));//Omega Dot
                        d_i_GPS[GPS_num] = Convert.ToDouble(line_GPS[5].Substring(4, 19));// d_i
                        L2_code_GPS[GPS_num] = Convert.ToDouble(line_GPS[5].Substring(23, 19));// L2上的码
                        GPS_week_GPS[GPS_num] = Convert.ToDouble(line_GPS[5].Substring(42, 19));//GPS周
                        L2_P_code_GPS[GPS_num]= Convert.ToDouble(line_GPS[5].Substring(61, 19));//L2上P码数据标记
                        Sat_accu_GPS[GPS_num] = Convert.ToDouble(line_GPS[6].Substring(4, 19));//卫星精度
                        Sat_health_GPS[GPS_num] = Convert.ToDouble(line_GPS[6].Substring(23, 19));//卫星健康
                        TGD_GPS[GPS_num] = Convert.ToDouble(line_GPS[6].Substring(42, 19));// 群延迟
                        IODC_age_GPS[GPS_num] = Convert.ToDouble(line_GPS[6].Substring(61, 19));//数据龄期
                        time_sent_GPS[GPS_num] = Convert.ToDouble(line_GPS[7].Substring(4, 19));//电文发送时刻
                        nihequjian_GPS[GPS_num] = Convert.ToDouble(line_GPS[6].Substring(23, 19));//拟合区间
                        GPS_num++;
                    } while (!line.Contains('S'));
                    do
                    {
                        //richTextBox1.Text += line + '\n';
                        line = reader.ReadLine();

                    } while (!line.Contains("C"));
                    do
                    {
                        //line = line.Replace('D', 'E');
                        for (int i = 0; i < 8; i++)
                        {
                            line_BDS[i] = line;
                            line = line.Replace('D', 'E');
                            //richTextBox1.Text += line + '\n';
                            line = reader.ReadLine();
                        }
                        PRN_BDS[BDS_num] = line_BDS[0].Substring(0, 3);
                        TOC_Year_BDS[BDS_num] = Convert.ToDouble(line_BDS[0].Substring(4, 4));//TOC 年
                        TOC_Month_BDS[BDS_num] = Convert.ToDouble(line_BDS[0].Substring(9, 2));//TOC 月
                        TOC_Day_BDS[BDS_num] = Convert.ToDouble(line_BDS[0].Substring(12, 2));// TOC 日
                        TOC_Hour_BDS[BDS_num] = Convert.ToDouble(line_BDS[0].Substring(15, 2));//TOC 时
                        TOC_Min_BDS[BDS_num] = Convert.ToDouble(line_BDS[0].Substring(18, 2));// TOC 分
                        TOC_Sec_BDS[BDS_num] = Convert.ToDouble(line_BDS[0].Substring(21, 2));//TOC 秒
                        Clock_error_BDS[BDS_num] = Convert.ToDouble(line_BDS[0].Substring(23, 19));// 钟差
                        Clock_drift_BDS[BDS_num] = Convert.ToDouble(line_BDS[0].Substring(42, 19));//钟漂
                        Clock_offset_BDS[BDS_num] = Convert.ToDouble(line_BDS[0].Substring(61, 19));//频偏
                        IODE_BDS[BDS_num] = Convert.ToDouble(line_BDS[1].Substring(4, 19));// 数据，星历发布时间
                        Crs_BDS[BDS_num] = Convert.ToDouble(line_BDS[1].Substring(23, 19));// r改正参数
                        delta_n_BDS[BDS_num] = Convert.ToDouble(line_BDS[1].Substring(42, 19));// δn
                        M0_BDS[BDS_num] = Convert.ToDouble(line_BDS[1].Substring(61, 19));// M0
                        Cuc_BDS[BDS_num] = Convert.ToDouble(line_BDS[2].Substring(4, 19));// u 改正数
                        e_BDS[BDS_num] = Convert.ToDouble(line_BDS[2].Substring(23, 19));// 轨道偏心率
                        Cus_BDS[BDS_num] = Convert.ToDouble(line_BDS[2].Substring(42, 19));// u 改正数
                        sqrt_A_BDS[BDS_num] = Convert.ToDouble(line_BDS[2].Substring(61, 19));// 根号A
                        TOE_BDS[BDS_num] = Convert.ToDouble(line_BDS[3].Substring(4, 19));//参考时刻
                        Cic_BDS[BDS_num] = Convert.ToDouble(line_BDS[3].Substring(23, 19));// i
                        Omega_BDS[BDS_num] = Convert.ToDouble(line_BDS[3].Substring(42, 19));// Omega
                        Cis_BDS[BDS_num] = Convert.ToDouble(line_BDS[3].Substring(61, 19));// i
                        i0_BDS[BDS_num] = Convert.ToDouble(line_BDS[4].Substring(4, 19));// i0
                        Crc_BDS[BDS_num] = Convert.ToDouble(line_BDS[4].Substring(23, 19));//r
                        omega_BDS[BDS_num] = Convert.ToDouble(line_BDS[4].Substring(42, 19));//omega
                        d_Omega_BDS[BDS_num] = Convert.ToDouble(line_BDS[4].Substring(61, 19));//Omega Dot
                        d_i_BDS[BDS_num] = Convert.ToDouble(line_BDS[5].Substring(4, 19));// d_i
                        L2_code_BDS[BDS_num] = Convert.ToDouble(line_BDS[5].Substring(23, 19));// L2上的码
                        GPS_week_BDS[BDS_num] = Convert.ToDouble(line_BDS[5].Substring(42, 19));//BDS周
                        L2_P_code_BDS[BDS_num] = Convert.ToDouble(line_BDS[5].Substring(61, 19));//L2上P码数据标记
                        Sat_accu_BDS[BDS_num] = Convert.ToDouble(line_BDS[6].Substring(4, 19));//卫星精度
                        Sat_health_BDS[BDS_num] = Convert.ToDouble(line_BDS[6].Substring(23, 19));//卫星健康
                        TGD_BDS[BDS_num] = Convert.ToDouble(line_BDS[6].Substring(42, 19));// 群延迟
                        IODC_age_BDS[BDS_num] = Convert.ToDouble(line_BDS[6].Substring(61, 19));//数据龄期
                        time_sent_BDS[BDS_num] = Convert.ToDouble(line_BDS[7].Substring(4, 19));//电文发送时刻
                        nihequjian_BDS[BDS_num] = Convert.ToDouble(line_BDS[6].Substring(23, 19));//拟合区间
                        BDS_num++;
                        
                    } while (!line.Contains('J'));
                    do
                    {
                        line = reader.ReadLine();
                    } while (!reader.EndOfStream);
                    reader.Close();
                   
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(Convert.ToString(ex));
            }
            
        }

        private void 打开OToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                string line;
                double second=0;
                int t = 0;
                APPROX_POSITION_X = 0;
                APPROX_POSITION_Y = 0;
                APPROX_POSITION_Z = 0;
                ANTENNA_DELTA_H = 0;
                ANTENNA_DELTA_E = 0;
                ANTENNA_DELTA_N = 0;
                INTERVAL = 0;
                if (openFileDialog4.ShowDialog() == DialogResult.OK)
                {
                    StreamReader reader = new StreamReader(openFileDialog4.FileName);
                    openFileDialog4.Filter = "All(*.*)|*.*|obs(*.O)|*.O|txt(*.txt)|*.txt|tar(*.tar)|*.tar";
                    line = reader.ReadLine();
                    if(line.Substring(60).Contains("RINEX VERSION / TYPE")&& line.Substring(5,1)=="3")
                    {
                        
                        while(!line.Contains("END OF HEADER"))
                        {
                            if(line.Contains("APPROX POSITION XYZ"))
                            {
                                APPROX_POSITION_X = Convert.ToDouble(line.Substring(1, 13));
                                APPROX_POSITION_Y = Convert.ToDouble(line.Substring(15, 13));
                                APPROX_POSITION_Z = Convert.ToDouble(line.Substring(29, 13));
                            }
                            if(line.Contains("ANTENNA: DELTA H/E/N"))
                            {
                                ANTENNA_DELTA_H= Convert.ToDouble(line.Substring(1, 13)); 
                                ANTENNA_DELTA_E= Convert.ToDouble(line.Substring(15, 13));
                                ANTENNA_DELTA_N= Convert.ToDouble(line.Substring(29, 13));
                            }
                            if (line.Contains("INTERVAL"))
                                INTERVAL = Convert.ToDouble(line.Substring(0, 60));
                            line = reader.ReadLine();
                        }
                        G02 = new double[Convert.ToInt32(86400 / INTERVAL), 4];//四列分别为C1C，C2W，L1C，L2W
                        G05 = new double[Convert.ToInt32(86400 / INTERVAL), 4];
                        G13 = new double[Convert.ToInt32(86400 / INTERVAL), 4];
                        G12 = new double[Convert.ToInt32(86400 / INTERVAL), 4];
                        G15 = new double[Convert.ToInt32(86400 / INTERVAL), 4];
                        G21 = new double[Convert.ToInt32(86400 / INTERVAL), 4];

                        C01 = new double[Convert.ToInt32(86400 / INTERVAL), 4];//四列分别为C2I，C6I，L2I，L6I
                        C03 = new double[Convert.ToInt32(86400 / INTERVAL), 4];
                        C06 = new double[Convert.ToInt32(86400 / INTERVAL), 4];
                        C08 = new double[Convert.ToInt32(86400 / INTERVAL), 4];
                        C11 = new double[Convert.ToInt32(86400 / INTERVAL), 4];
                        C12 = new double[Convert.ToInt32(86400 / INTERVAL), 4];
                        line = reader.ReadLine();
                        while (!reader.EndOfStream)
                        {
                            second = Convert.ToDouble(line.Substring(13, 2)) * 3600 + Convert.ToDouble(line.Substring(16, 2)) * 60 + Convert.ToDouble(line.Substring(19, 10));
                            t = Convert.ToInt32(second / INTERVAL);
                            line = reader.ReadLine();
                            while (line.Substring(0, 1) != ">")
                            {
                                if (line.Substring(0, 3) == "C01")
                                {
                                    if (line.Substring(5, 12) != "            ")
                                        C01[t, 0] = Convert.ToDouble(line.Substring(5, 12));
                                    if (line.Substring(21, 12) != "            ")
                                        C01[t, 1] = Convert.ToDouble(line.Substring(21, 12));
                                    if (line.Substring(100, 14) != "              ")
                                        C01[t, 2] = Convert.ToDouble(line.Substring(100, 14));
                                    if (line.Substring(116, 14) != "              ")
                                        C01[t, 3] = Convert.ToDouble(line.Substring(116, 14));
                                }
                                if (line.Substring(0, 3) == "C03")
                                {
                                    if (line.Substring(5, 12) != "            ")
                                        C03[t, 0] = Convert.ToDouble(line.Substring(5, 12));
                                    if (line.Substring(21, 12) != "            ")
                                        C03[t, 1] = Convert.ToDouble(line.Substring(21, 12));
                                    if (line.Substring(100, 14) != "              ")
                                        C03[t, 2] = Convert.ToDouble(line.Substring(100, 14));
                                    if (line.Substring(116, 14) != "              ")
                                        C03[t, 3] = Convert.ToDouble(line.Substring(116, 14));
                                }
                                if (line.Substring(0, 3) == "C06")
                                {
                                    if (line.Substring(5, 12) != "            ")
                                        C06[t, 0] = Convert.ToDouble(line.Substring(5, 12));
                                    if (line.Substring(21, 12) != "            ")
                                        C06[t, 1] = Convert.ToDouble(line.Substring(21, 12));
                                    if (line.Substring(100, 14) != "              ")
                                        C06[t, 2] = Convert.ToDouble(line.Substring(100, 14));
                                    if (line.Substring(116, 14) != "              ")
                                        C06[t, 3] = Convert.ToDouble(line.Substring(116, 14));
                                }
                                if (line.Substring(0, 3) == "C08")
                                {
                                    if (line.Substring(5, 12) != "            ")
                                        C08[t, 0] = Convert.ToDouble(line.Substring(5, 12));
                                    if (line.Substring(21, 12) != "            ")
                                        C08[t, 1] = Convert.ToDouble(line.Substring(21, 12));
                                    if (line.Substring(100, 14) != "              ")
                                        C08[t, 2] = Convert.ToDouble(line.Substring(100, 14));
                                    if (line.Substring(116, 14) != "              ")
                                        C08[t, 3] = Convert.ToDouble(line.Substring(116, 14));
                                }
                                if (line.Substring(0, 3) == "C11")
                                {
                                    if (line.Substring(5, 12) != "            ")
                                        C11[t, 0] = Convert.ToDouble(line.Substring(5, 12));
                                    if (line.Substring(21, 12) != "            ")
                                        C11[t, 1] = Convert.ToDouble(line.Substring(21, 12));
                                    if (line.Substring(100, 14) != "              ")
                                        C11[t, 2] = Convert.ToDouble(line.Substring(100, 14));
                                    if (line.Substring(116, 14) != "              ")
                                        C11[t, 3] = Convert.ToDouble(line.Substring(116, 14));
                                }
                                if (line.Substring(0, 3) == "C12")
                                {
                                    if (line.Substring(5, 12) != "            ")
                                        C12[t, 0] = Convert.ToDouble(line.Substring(5, 12));
                                    if (line.Substring(21, 12) != "            ")
                                        C12[t, 1] = Convert.ToDouble(line.Substring(21, 12));
                                    if (line.Substring(100, 14) != "              ")
                                        C12[t, 2] = Convert.ToDouble(line.Substring(100, 14));
                                    if (line.Substring(116, 14) != "              ")
                                        C12[t, 3] = Convert.ToDouble(line.Substring(116, 14));
                                }
                                if (line.Substring(0, 3) == "G02")
                                {
                                    if (line.Substring(5, 12) != "            ")
                                        G02[t, 0] = Convert.ToDouble(line.Substring(5, 12));
                                    if (line.Substring(21, 12) != "            ")
                                        G02[t, 1] = Convert.ToDouble(line.Substring(21, 12));
                                    if (line.Substring(132, 14) != "              ")
                                        G02[t, 2] = Convert.ToDouble(line.Substring(132, 14));
                                    if (line.Substring(148, 14) != "              ")
                                        G02[t, 3] = Convert.ToDouble(line.Substring(148, 14));
                                }
                                if (line.Substring(0, 3) == "G05")
                                {
                                    if (line.Substring(5, 12) != "            ")
                                        G05[t, 0] = Convert.ToDouble(line.Substring(5, 12));
                                    if (line.Substring(21, 12) != "            ")
                                        G05[t, 1] = Convert.ToDouble(line.Substring(21, 12));
                                    if (line.Substring(132, 14) != "              ")
                                        G05[t, 2] = Convert.ToDouble(line.Substring(132, 14));
                                    if (line.Substring(148, 14) != "              ")
                                        G05[t, 3] = Convert.ToDouble(line.Substring(148, 14));
                                }
                                if (line.Substring(0, 3) == "G12")
                                {
                                    if (line.Substring(5, 12) != "            ")
                                        G12[t, 0] = Convert.ToDouble(line.Substring(5, 12));
                                    if (line.Substring(21, 12) != "            ")
                                        G12[t, 1] = Convert.ToDouble(line.Substring(21, 12));
                                    if (line.Substring(132, 14) != "              ")
                                        G12[t, 2] = Convert.ToDouble(line.Substring(132, 14));
                                    if (line.Substring(148, 14) != "              ")
                                        G12[t, 3] = Convert.ToDouble(line.Substring(148, 14));
                                }
                                if (line.Substring(0, 3) == "G13")
                                {
                                    if (line.Substring(5, 12) != "            ")
                                        G13[t, 0] = Convert.ToDouble(line.Substring(5, 12));
                                    if (line.Substring(21, 12) != "            ")
                                        G13[t, 1] = Convert.ToDouble(line.Substring(21, 12));
                                    if (line.Substring(132, 14) != "              ")
                                        G13[t, 2] = Convert.ToDouble(line.Substring(132, 14));
                                    if (line.Substring(148, 14) != "              ")
                                        G13[t, 3] = Convert.ToDouble(line.Substring(148, 14));
                                }
                                if (line.Substring(0, 3) == "G15")
                                {
                                    if (line.Substring(5, 12) != "            ")
                                        G15[t, 0] = Convert.ToDouble(line.Substring(5, 12));
                                    if (line.Substring(21, 12) != "            ")
                                        G15[t, 1] = Convert.ToDouble(line.Substring(21, 12));
                                    if (line.Substring(132, 14) != "              ")
                                        G15[t, 2] = Convert.ToDouble(line.Substring(132, 14));
                                    if (line.Substring(148, 14) != "              ")
                                        G15[t, 3] = Convert.ToDouble(line.Substring(148, 14));
                                }
                                if (line.Substring(0, 3) == "G21")
                                {
                                    if (line.Substring(5, 12) != "            ")
                                        G21[t, 0] = Convert.ToDouble(line.Substring(5, 12));
                                    if (line.Substring(21, 12) != "            ")
                                        G21[t, 1] = Convert.ToDouble(line.Substring(21, 12));
                                    if (line.Substring(132, 14) != "              ")
                                        G21[t, 2] = Convert.ToDouble(line.Substring(132, 14));
                                    if (line.Substring(148, 14) != "              ")
                                        G21[t, 3] = Convert.ToDouble(line.Substring(148, 14));
                                }
                                line = reader.ReadLine();
                                if (line == null)
                                    break;
                            }
       
                        }

                        for (int i = 0; i < Convert.ToInt32(86400 / INTERVAL); i++)
                        {
                            richTextBox2.Text += Convert.ToString(G05[i, 0]) + ' ' + Convert.ToString(G05[i, 1]) + ' ' + Convert.ToString(G05[i, 2]) + ' ' + Convert.ToString(G05[i, 3]) + '\n';
                        }
                    }
                    else if(line.Substring(60).Contains("RINEX VERSION / TYPE") && line.Substring(5, 1) == "2")
                    {
                        richTextBox2.Text = null;
                        richTextBox2.Text = line;
                        while (!reader.EndOfStream)
                        {
                            line = reader.ReadLine();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(Convert.ToString(ex));
            }
            
        }

        private void opensp3ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
               
                if (openFileDialog2.ShowDialog() == DialogResult.OK)
                {
                    StreamReader reader = new StreamReader(openFileDialog2.FileName);
                    openFileDialog2.Filter = "All(*.*)|*.*|nav(*.sp3)|*.sp3|txt(*.txt)|*.txt|tar(*.tar)|*.tar";

                    if(Table.Rows.Count!=0)
                    {
                        string line;
                        int t = 0;
                        line = reader.ReadLine();

                        while (line.Substring(0, 1) != "*")
                        {
                            line = reader.ReadLine();
                        }

                        while (!reader.EndOfStream)
                        {
                            while (!line.Contains("EOF") && Convert.ToDouble(line.Substring(14, 2)) == Convert.ToDouble(Table.Rows[74 * t]["Hour"]) && Convert.ToDouble(line.Substring(17, 2)) == Convert.ToDouble(Table.Rows[74 * t]["Minute"]) && Convert.ToDouble(line.Substring(20)) == Convert.ToDouble(Table.Rows[74 * t]["Second"]))
                            {
                                line = reader.ReadLine();
                                do
                                {
                                    for (int j = 0; j < 74; j++)
                                    {
                                        if (!line.Contains("EOF") && line.Substring(1, 3) == Convert.ToString(Table.Rows[j + 74 * t]["PRN"]))
                                        {
                                            Table.Rows[j + 74 * t]["Xsp3"] = Convert.ToDouble(line.Substring(5, 13)) * 1000;
                                            Table.Rows[j + 74 * t]["Ysp3"] = Convert.ToDouble(line.Substring(19, 13)) * 1000;
                                            Table.Rows[j + 74 * t]["Zsp3"] = Convert.ToDouble(line.Substring(33, 13)) * 1000;
                                        }
                                        else
                                            continue;
                                    }


                                    line = reader.ReadLine();
                                } while (!line.Contains("EOF") && line.Substring(0, 1) != "*");
                                t++;

                            }
                            line = reader.ReadLine();
                        }

                        for (int i = 0; i < Table.Rows.Count; i++)
                        {
                            if (Convert.ToString(Table.Rows[i]["X"]) != "" && Convert.ToString(Table.Rows[i]["Xsp3"]) != "")
                            {
                                Table.Rows[i]["dx"] = Convert.ToDouble(Table.Rows[i]["X"]) - Convert.ToDouble(Table.Rows[i]["Xsp3"]);
                                Table.Rows[i]["dy"] = Convert.ToDouble(Table.Rows[i]["Y"]) - Convert.ToDouble(Table.Rows[i]["Ysp3"]);
                                Table.Rows[i]["dz"] = Convert.ToDouble(Table.Rows[i]["Z"]) - Convert.ToDouble(Table.Rows[i]["Zsp3"]);
                            }

                        }

                    } 
                    else
                    {
                        Table2.Rows.Clear();
                        string line;
                        int rows = 0;
                        int t = 0;
                        line = reader.ReadLine();
                        while (line.Substring(0, 1) != "*")
                        {
                            line = reader.ReadLine();
                        }
                        Table2.Rows.Add();
                        while(line!="EOF")
                        {
                            
                            Table2.Rows[rows]["历元"] = t;t += 900;
                            //Table2.Rows[rows]["Year"] = Convert.ToDouble(line.Substring(3, 4));
                            //Table2.Rows[rows]["Month"] = Convert.ToDouble(line.Substring(8, 2));
                            //Table2.Rows[rows]["Day"] = Convert.ToDouble(line.Substring(11, 2));
                            //Table2.Rows[rows]["Hour"] = Convert.ToDouble(line.Substring(14, 2));
                            //Table2.Rows[rows]["Minute"] = Convert.ToDouble(line.Substring(17, 2));
                            //Table2.Rows[rows]["Second"] = Convert.ToDouble(line.Substring(20));
                            line = reader.ReadLine();
                            while (line.Substring(0, 1) != "*" && line != "EOF")
                            {
                                Table2.Rows[rows]["PRN"] = line.Substring(0, 4);
                                Table2.Rows[rows]["X"] = Convert.ToDouble(line.Substring(5, 13));
                                Table2.Rows[rows]["Y"] = Convert.ToDouble(line.Substring(19, 13));
                                Table2.Rows[rows]["Z"] = Convert.ToDouble(line.Substring(33, 13));
                                Table2.Rows[rows]["Clock"] = Convert.ToDouble(line.Substring(47,13));


                                line = reader.ReadLine();
                                rows++;
                                if (line != "EOF")
                                {
                                    Table2.Rows.Add();
                                }
                                else
                                    continue;
                            } 
                        }
                        dataGridView3.DataSource = Table2;
                    }
                    
                }
               
            }
            catch (Exception ex)
            {
                MessageBox.Show(Convert.ToString(ex));
            }
        }

        private void 退出ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form3 f = new Form3();
            f.Show();
            this.Close();
        }

        private void 菜单ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form3 f = new Form3();
            f.Show();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Form3 f = new Form3();
            f.Show();
        }

        private void 卫星轨道ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (PRN_BDS == null)
            {

            }
            else
            {
                GPS_Sat_num = 0;
                BDS_Sat_num = 0;
                //先计算GPS的
                double n0;
                double n;
                double M;
                double E0;
                double E;
                double f;
                double u_p;//未改正
                double r_p;
                double delta_u;
                double delta_r;
                double delta_i;
                double u;
                double r;
                double I;
                double x;
                double y;
                double L;
                double[] X;
                double[] Y;
                double[] Z;
                double[] X_BDS;
                double[] Y_BDS;
                double[] Z_BDS;
                double X_BDS_GEO;
                double Y_BDS_GEO;
                double Z_BDS_GEO;
                double dE = 0;
                int count = 0;
                int[] row_num;
                int[] row_num_BDS;
                int row_num_count;
                int row_num_count_BDS;
                int d_row = 0;
                int d_d_row = 0;
                int d_row_BDS = 0;
                int d_d_row_BDS = 0;
                double d_t = 0;
                double omiga_e = 7.292115E-5;
                for (int t = 0; t < 86400; t += time)
                {
                    GPS_Sat_num = 0;
                    BDS_Sat_num = 0;
                    for (int i = 0; i < GPS_num; i++)
                    {
                        if (PRN_GPS[i] != PRN_GPS[i + 1])
                        {
                            GPS_Sat_num++;
                        }
                    }
                    for (int i = 0; i < BDS_num; i++)
                    {
                        if (PRN_BDS[i] != PRN_BDS[i + 1])
                        {
                            BDS_Sat_num++;
                        }
                    }
                    //GPS
                    row_num = new int[GPS_Sat_num];
                    row_num_BDS = new int[BDS_Sat_num];
                    row_num_count = 0;
                    for (int i = 0; i < GPS_num; i++)
                    {

                        if (PRN_GPS[i] == PRN_GPS[i + 1])
                        {
                            if (Math.Abs(t - TOE_GPS[d_row]) >= Math.Abs(t - TOE_GPS[i]))
                            {
                                row_num[row_num_count] = i;
                                d_row = i;
                            }
                            else
                                row_num[row_num_count] = d_row;
                            d_d_row++;
                        }
                        else
                        {
                            d_row = d_row + d_d_row;
                            d_d_row = 0;
                            row_num_count++;
                        }
                    }

                    //BDS
                    row_num_count_BDS = 0;

                    for (int i = 0; i < BDS_num; i++)
                    {
                        if (PRN_BDS[i] == PRN_BDS[i + 1])
                        {
                            if (Math.Abs(t - TOE_BDS[d_row_BDS]) >= Math.Abs(t - TOE_BDS[i]))
                            {
                                row_num_BDS[row_num_count_BDS] = i;
                                d_row_BDS = i;
                            }
                            else
                                row_num_BDS[row_num_count_BDS] = d_row_BDS;
                            d_d_row_BDS++;
                        }
                        else
                        {
                            d_row_BDS = d_row_BDS + d_d_row_BDS;
                            d_d_row_BDS = 0;
                            row_num_count_BDS++;
                        }
                    }
                    /////开始计算
                    X = new double[GPS_Sat_num];
                    Y = new double[GPS_Sat_num];
                    Z = new double[GPS_Sat_num];
                    n0 = 0;
                    n = 0;
                    M = 0;
                    E0 = 0;
                    E = 0;
                    f = 0;
                    u_p = 0;//未改正
                    r_p = 0;
                    delta_u = 0;
                    delta_r = 0;
                    delta_i = 0;
                    u = 0;
                    r = 0;
                    I = 0;
                    x = 0;
                    y = 0;
                    L = 0;
                    //GPS
                    for (int j = 0; j < GPS_Sat_num; j++)
                    {
                        //1、计算卫星运动的平均角速度
                        n0 = Math.Sqrt(GM_GPS / (sqrt_A_GPS[row_num[j]] * sqrt_A_GPS[row_num[j]] * sqrt_A_GPS[row_num[j]] * sqrt_A_GPS[row_num[j]] * sqrt_A_GPS[row_num[j]] * sqrt_A_GPS[row_num[j]]));
                        // 计算观测时刻的平均角速度n
                        n = n0 + delta_n_GPS[row_num[j]];
                        //2、计算观测瞬间卫星的平近点角M
                        d_t = t - TOE_GPS[row_num[j]];
                        if (d_t > 302400)
                            d_t -= 604800;
                        else if (d_t < -302400)
                            d_t += 604800;
                        M = M0_GPS[row_num[j]] + n * d_t;
                        //if (M < 0)
                        //    M += 2 * Math.PI;
                        //else if (M > 2 * Math.PI)
                        //    M -= 2 * Math.PI;
                        //3、计算偏近点角
                        E0 = M;
                        count = 0;
                        while (count < 8)
                        {
                            E = M + e_GPS[row_num[j]] * Math.Sin(E0);
                            dE = E - E0;
                            E0 = E;
                            count++;
                        }

                        f = Math.Atan2(Math.Sqrt(1 - e_GPS[row_num[j]] * e_GPS[row_num[j]]) * Math.Sin(E), ((Math.Cos(E) - e_GPS[row_num[j]])));

                        //5、计算升交距角
                        u_p = omega_GPS[row_num[j]] + f;
                        //6、计算卫星向径
                        r_p = sqrt_A_GPS[row_num[j]] * sqrt_A_GPS[row_num[j]] * (1 - e_GPS[row_num[j]] * Math.Cos(E));
                        //7、计算摄动改正项
                        delta_u = Cuc_GPS[row_num[j]] * Math.Cos(2 * u_p) + Cus_GPS[row_num[j]] * Math.Sin(2 * u_p);
                        delta_r = Crc_GPS[row_num[j]] * Math.Cos(2 * u_p) + Crs_GPS[row_num[j]] * Math.Sin(2 * u_p);
                        delta_i = Cic_GPS[row_num[j]] * Math.Cos(2 * u_p) + Cis_GPS[row_num[j]] * Math.Sin(2 * u_p);
                        //8、进行摄动改正
                        u = u_p + delta_u;
                        r = r_p + delta_r;
                        I = i0_GPS[row_num[j]] + delta_i + d_i_GPS[row_num[j]] * d_t;
                        //9、计算卫星在轨道平面上的位置
                        x = r * Math.Cos(u);
                        y = r * Math.Sin(u);
                        //10、计算开交点的经度
                        L = Omega_GPS[row_num[j]] + d_Omega_GPS[row_num[j]] * d_t - omiga_e * t;
                        //11.计算XYZ
                        X[j] = x * Math.Cos(L) - y * Math.Cos(I) * Math.Sin(L);
                        Y[j] = x * Math.Sin(L) + y * Math.Cos(I) * Math.Cos(L);
                        Z[j] = y * Math.Sin(I);
                        //row = Table.NewRow();
                        Table.Rows.Add();
                        Table.Rows[t / time * (GPS_Sat_num + BDS_Sat_num)]["Year"] = TOC_Year_GPS[row_num[j]];
                        Table.Rows[j + t / time * (GPS_Sat_num + BDS_Sat_num)]["PRN"] = PRN_GPS[row_num[j]];
                        Table.Rows[j + t / time * (GPS_Sat_num + BDS_Sat_num)]["X"] = X[j];
                        Table.Rows[j + t / time * (GPS_Sat_num + BDS_Sat_num)]["Y"] = Y[j];
                        Table.Rows[j + t / time * (GPS_Sat_num + BDS_Sat_num)]["Z"] = Z[j];

                    }
                    omiga_e = 7.2921150E-5;
                    X_BDS = new double[BDS_Sat_num];
                    Y_BDS = new double[BDS_Sat_num];
                    Z_BDS = new double[BDS_Sat_num];
                    X_BDS_GEO = 0;
                    Y_BDS_GEO = 0;
                    Z_BDS_GEO = 0;
                    n0 = 0;
                    n = 0;
                    M = 0;
                    E0 = 0;
                    E = 0;
                    f = 0;
                    u_p = 0;//未改正
                    r_p = 0;
                    delta_u = 0;
                    delta_r = 0;
                    delta_i = 0;
                    u = 0;
                    r = 0;
                    I = 0;
                    x = 0;
                    y = 0;
                    L = 0;
                    for (int j = 0; j < BDS_Sat_num; j++)
                    {
                        //0.计算A
                        double A = sqrt_A_BDS[row_num_BDS[j]] * sqrt_A_BDS[row_num_BDS[j]];
                        //1、计算卫星运动的平均角速度
                        n0 = Math.Sqrt(GM_BDS / (A * A * A));
                        // 计算观测时刻的平均角速度n
                        n = n0 + delta_n_BDS[row_num_BDS[j]];
                        //2、计算观测瞬间卫星的平近点角M
                        d_t = t - TOE_BDS[row_num_BDS[j]] - 14;
                        if (d_t > 302400)
                            d_t -= 604800;
                        else if (d_t < -302400)
                            d_t += 604800;
                        M = M0_BDS[row_num_BDS[j]] + n * d_t;
                        //3、计算偏近点角
                        E0 = M;
                        count = 0;
                        while (count < 8)
                        {
                            E = M + e_BDS[row_num_BDS[j]] * Math.Sin(E0);
                            dE = E - E0;
                            E0 = E;
                            count++;
                        }
                        //4.计算真近点角
                        //f = Math.Acos((Math.Cos(E) - e_BDS[row_num_BDS[j]]) / (1 - e_BDS[row_num_BDS[j]] * Math.Cos(E)));
                        f = Math.Atan2(Math.Sqrt(1 - e_BDS[row_num_BDS[j]] * e_BDS[row_num_BDS[j]]) * Math.Sin(E), Math.Cos(E) - e_BDS[row_num_BDS[j]]);
                        //5、计算纬度幅角参数
                        u_p = omega_BDS[row_num_BDS[j]] + f;
                        //6、计算卫星向径
                        r_p = A * (1 - e_BDS[row_num_BDS[j]] * Math.Cos(E));
                        //7、计算摄动改正项
                        delta_u = Cuc_BDS[row_num_BDS[j]] * Math.Cos(2 * u_p) + Cus_BDS[row_num_BDS[j]] * Math.Sin(2 * u_p);
                        delta_r = Crc_BDS[row_num_BDS[j]] * Math.Cos(2 * u_p) + Crs_BDS[row_num_BDS[j]] * Math.Sin(2 * u_p);
                        delta_i = Cic_BDS[row_num_BDS[j]] * Math.Cos(2 * u_p) + Cis_BDS[row_num_BDS[j]] * Math.Sin(2 * u_p);
                        //8、进行摄动改正
                        u = u_p + delta_u;
                        r = r_p + delta_r;
                        I = i0_BDS[row_num_BDS[j]] + delta_i + d_i_BDS[row_num_BDS[j]] * d_t;
                        //9、计算卫星在轨道平面上的位置
                        x = r * Math.Cos(u);
                        y = r * Math.Sin(u);
                        if (PRN_BDS[row_num_BDS[j]] == "C06" || PRN_BDS[row_num_BDS[j]] == "C07" || PRN_BDS[row_num_BDS[j]] == "C08" || PRN_BDS[row_num_BDS[j]] == "C09" || PRN_BDS[row_num_BDS[j]] == "C10" || PRN_BDS[row_num_BDS[j]] == "C13" || PRN_BDS[row_num_BDS[j]] == "C16" || PRN_BDS[row_num_BDS[j]] == "C31" || PRN_BDS[row_num_BDS[j]] == "C38" || PRN_BDS[row_num_BDS[j]] == "C39" || PRN_BDS[row_num_BDS[j]] == "C10" || PRN_BDS[row_num_BDS[j]] == "C56" || PRN_BDS[row_num_BDS[j]] == "C57" || PRN_BDS[row_num_BDS[j]] == "C58" || PRN_BDS[row_num_BDS[j]] == "C11" || PRN_BDS[row_num_BDS[j]] == "C12" || PRN_BDS[row_num_BDS[j]] == "C14" || PRN_BDS[row_num_BDS[j]] == "C19" || PRN_BDS[row_num_BDS[j]] == "C10" || PRN_BDS[row_num_BDS[j]] == "C20" || PRN_BDS[row_num_BDS[j]] == "C21" || PRN_BDS[row_num_BDS[j]] == "C22" || PRN_BDS[row_num_BDS[j]] == "C23" || PRN_BDS[row_num_BDS[j]] == "C24" || PRN_BDS[row_num_BDS[j]] == "C25" || PRN_BDS[row_num_BDS[j]] == "C26" || PRN_BDS[row_num_BDS[j]] == "C27" || PRN_BDS[row_num_BDS[j]] == "C28" || PRN_BDS[row_num_BDS[j]] == "C29" || PRN_BDS[row_num_BDS[j]] == "C30" || PRN_BDS[row_num_BDS[j]] == "C32" || PRN_BDS[row_num_BDS[j]] == "C33" || PRN_BDS[row_num_BDS[j]] == "C34" || PRN_BDS[row_num_BDS[j]] == "C35" || PRN_BDS[row_num_BDS[j]] == "C36" || PRN_BDS[row_num_BDS[j]] == "C37" || PRN_BDS[row_num_BDS[j]] == "C41" || PRN_BDS[row_num_BDS[j]] == "C42" || PRN_BDS[row_num_BDS[j]] == "C43" || PRN_BDS[row_num_BDS[j]] == "C44" || PRN_BDS[row_num_BDS[j]] == "C45" || PRN_BDS[row_num_BDS[j]] == "C46")
                        {
                            //10、计算开交点的经度
                            L = Omega_BDS[row_num_BDS[j]] + (d_Omega_BDS[row_num_BDS[j]] - omiga_e) * d_t - omiga_e * TOE_BDS[row_num_BDS[j]];
                            X_BDS[j] = x * Math.Cos(L) - y * Math.Cos(I) * Math.Sin(L);
                            Y_BDS[j] = x * Math.Sin(L) + y * Math.Cos(I) * Math.Cos(L);
                            Z_BDS[j] = y * Math.Sin(I);
                        }
                        else if (PRN_BDS[row_num_BDS[j]] == "C01" || PRN_BDS[row_num_BDS[j]] == "C02" || PRN_BDS[row_num_BDS[j]] == "C03" || PRN_BDS[row_num_BDS[j]] == "C04" || PRN_BDS[row_num_BDS[j]] == "C05" || PRN_BDS[row_num_BDS[j]] == "C59" || PRN_BDS[row_num_BDS[j]] == "C60" || PRN_BDS[row_num_BDS[j]] == "C61")
                        {
                            L = Omega_BDS[row_num_BDS[j]] + d_Omega_BDS[row_num_BDS[j]] * d_t - omiga_e * TOE_BDS[row_num_BDS[j]];
                            X_BDS_GEO = x * Math.Cos(L) - y * Math.Cos(I) * Math.Sin(L);
                            Y_BDS_GEO = x * Math.Sin(L) + y * Math.Cos(I) * Math.Cos(L);
                            Z_BDS_GEO = y * Math.Sin(I);
                            X_BDS[j] = X_BDS_GEO * Math.Cos(omiga_e * d_t) + Y_BDS_GEO * Math.Sin(omiga_e * d_t) * Math.Cos(-5 * Math.PI / 180.0) + Z_BDS_GEO * Math.Sin(omiga_e * d_t) * Math.Sin(-5 * Math.PI / 180.0);
                            Y_BDS[j] = -X_BDS_GEO * Math.Sin(omiga_e * d_t) + Y_BDS_GEO * Math.Cos(omiga_e * d_t) * Math.Cos(-5 * Math.PI / 180.0) + Z_BDS_GEO * Math.Cos(omiga_e * d_t) * Math.Sin(-5 * Math.PI / 180.0);
                            Z_BDS[j] = -Y_BDS_GEO * Math.Sin(-5 * Math.PI / 180.0) + Z_BDS_GEO * Math.Cos(-5 * Math.PI / 180.0);
                        }
                        Table.Rows.Add();
                        Table.Rows[j + GPS_Sat_num + t / time * (GPS_Sat_num + BDS_Sat_num)]["PRN"] = PRN_BDS[row_num_BDS[j]];
                        Table.Rows[j + GPS_Sat_num + t / time * (GPS_Sat_num + BDS_Sat_num)]["X"] = X_BDS[j];
                        Table.Rows[j + GPS_Sat_num + t / time * (GPS_Sat_num + BDS_Sat_num)]["Y"] = Y_BDS[j];
                        Table.Rows[j + GPS_Sat_num + t / time * (GPS_Sat_num + BDS_Sat_num)]["Z"] = Z_BDS[j];
                    }
                    Table.Rows[t / time * (GPS_Sat_num + BDS_Sat_num)]["Month"] = TOC_Month_GPS[0];
                    Table.Rows[t / time * (GPS_Sat_num + BDS_Sat_num)]["Day"] = TOC_Day_GPS[0];
                    Table.Rows[t / time * (GPS_Sat_num + BDS_Sat_num)]["Hour"] = t / 3600;
                    Table.Rows[t / time * (GPS_Sat_num + BDS_Sat_num)]["Minute"] = (t - t / 3600 * 3600) / 60;
                    Table.Rows[t / time * (GPS_Sat_num + BDS_Sat_num)]["Second"] = t - t / 3600 * 3600 - (t - t / 3600 * 3600) / 60 * 60;

                    result += '*' + "  " + Convert.ToString(Table.Rows[t / time * (GPS_Sat_num + BDS_Sat_num)]["Year"]) + "  " + Convert.ToString(Table.Rows[t / time * (GPS_Sat_num + BDS_Sat_num)]["Month"]) + "  " + Convert.ToString(Table.Rows[t / time * (GPS_Sat_num + BDS_Sat_num)]["Day"]) + "  " + Convert.ToString(Table.Rows[t / 900 * 74]["Hour"]) + "  " + Convert.ToString(Table.Rows[t / time * (GPS_Sat_num + BDS_Sat_num)]["Minute"]) + "  " + Convert.ToString(Table.Rows[t / time * (GPS_Sat_num + BDS_Sat_num)]["Second"]) + '\n';
                    for (int i = 0; i < (GPS_Sat_num + BDS_Sat_num); i++)
                    {
                        result += 'P' + Convert.ToString(Table.Rows[i]["PRN"]) + "  " + Convert.ToString(Table.Rows[i]["X"]) + "  " + Convert.ToString(Table.Rows[i]["Y"]) + "  " + Convert.ToString(Table.Rows[i]["Z"]) + '\n';
                    }
                    dataGridView1.DataSource = Table;
                    // richTextBox1.Text = result;   

                }

            }
        }
   
        private void 保存sp3ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if(saveFileDialog1.ShowDialog()==DialogResult.OK)
                {
                    StreamWriter writer = new StreamWriter(saveFileDialog1.FileName);
                    saveFileDialog1.Filter = "all(*.*)|*.*|sp3(*.sp3)|*.sp3|xlsm(*.xlsm)|*.xlsm";
                    writer.Write(result);
                    writer.Close();
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(Convert.ToString(ex));
            }
        }

        private void 轨迹计算ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
            double[] X = new double[86400 / time];
            double[] Y = new double[86400 / time];
            double[] Z = new double[86400 / time];
            double[] B = new double[86400 / time];
            double[] L = new double[86400 / time];
            double[] H = new double[86400 / time];
            double B0;
            double a = 6378137;
            double f= 1 / 298.257223563;
            double N ;
            double Ee = 1 - (1 - f) * (1 - f);//e^2  
            int[] row_num = new int[86400 / time];
            for(int t=0;t<86400;t+=time)
            {
                for(int i=0;i<32;i++)
                {
                    if (Convert.ToString(Table.Rows[i + t / time * 74]["PRN"]) == toolStripComboBox1.Text)
                    {
                        row_num[t / time] = i + t / time * 74;
                    }
                    
                }
                
            }
            for(int i=0;i<86400/time;i++)
            {
                int count = 0; 
                X[i] = Convert.ToDouble(Table.Rows[row_num[i]]["X"]);
                Y[i] = Convert.ToDouble(Table.Rows[row_num[i]]["Y"]);
                Z[i] = Convert.ToDouble(Table.Rows[row_num[i]]["Z"]);
                L[i] = Math.Atan(Y[i] / X[i]);
                B0 = Math.Atan(Z[i] / Math.Sqrt(X[i] * X[i] + Y[i] * Y[i]));
                while(count<8)
                {
                    N = a / Math.Sqrt(1 - Ee  * Math.Sin(B0)  * Math.Sin(B0));
                    B[i] = Math.Atan((Z[i]+N*Ee*Math.Sin(B0))/(Math.Sqrt(X[i] * X[i] + Y[i] * Y[i])));
                    B0 = B[i];
                    count++;
                }
                B[i] = B[i] * 180 / Math.PI;
                L[i] = L[i] * 180 / Math.PI;
            }
            //chartAreas属性
            chart1.ChartAreas[0].AxisX.Title = "Lon(°)";
            chart1.ChartAreas[0].AxisY.Title = "Lat(°)";
            chart1.ChartAreas[0].AxisX.Maximum = 180;
            chart1.ChartAreas[0].AxisX.Minimum = -180;
            chart1.ChartAreas[0].AxisY.Maximum = 90;
            chart1.ChartAreas[0].AxisY.Minimum = -90;            
            chart1.Series[0].Points.DataBindXY(L, B);

            double[] T = new double[86400 / time];
            double[] dx = new double[86400 / time];
            double[] dy = new double[86400 / time];
            double[] dz = new double[86400 / time];
            int j = 0;
            for (int i = 0; i < 86400 / time; i++)
            {
                T[i] = time * i;
            }
            for (int i = 0; i < Table.Rows.Count; i++)
            {
                if (Convert.ToString(Table.Rows[i]["PRN"]) == toolStripComboBox1.Text)
                {
                    if(Convert.ToString(Table.Rows[i]["dx"])!="")
                    {
                        dx[j] = Convert.ToDouble(Table.Rows[i]["dx"]);
                        dy[j] = Convert.ToDouble(Table.Rows[i]["dy"]);
                        dz[j] = Convert.ToDouble(Table.Rows[i]["dz"]);
                    }
                    else
                    {
                        dx[j] = double.NaN;
                        dy[j] = double.NaN;
                        dz[j] = double.NaN;
                    }
                    j++;
                }
               
            }
            //chartAreas属性
            chart1.ChartAreas[1].AxisX.Title = "t(s)";
            chart1.ChartAreas[1].AxisY.Title = "d(m)";

            //将X轴上格网取消
            chart1.ChartAreas[1].AxisX.MajorGrid.Enabled = false;
            //chart1.ChartAreas[1].AxisY.MajorGrid.Enabled = false;            
            chart1.Series[1].Points.DataBindXY(T, dx);
            chart1.Series[2].Points.DataBindXY(T, dy);
            chart1.Series[3].Points.DataBindXY(T, dz);


        }

        private void 轨迹计算ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            double[] X = new double[86400 / time];
            double[] Y = new double[86400 / time];
            double[] Z = new double[86400 / time];
            double[] B = new double[86400 / time];
            double[] L = new double[86400 / time];
            double[] H = new double[86400 / time];
            double B0;
            double a = 6378137;
            double f = 1 / 298.257222101;
            double Ee = 1 - (1 - f) * (1 - f);
            double N;

            int[] row_num = new int[86400 / time];
            for (int t = 0; t < 86400; t += time)
            {
                for (int i = 0; i < 42; i++)
                {
                    if (Convert.ToString(Table.Rows[i +32+ t / time * 74]["PRN"]) == toolStripComboBox2.Text)
                    {
                        row_num[t / time] = i +32+ t / time * 74;
                    }

                }

            }
            for (int i = 0; i < 86400 / time; i++)
            {
                int count = 0;
                X[i] = Convert.ToDouble(Table.Rows[row_num[i]]["X"]);
                Y[i] = Convert.ToDouble(Table.Rows[row_num[i]]["Y"]);
                Z[i] = Convert.ToDouble(Table.Rows[row_num[i]]["Z"]);
                L[i] = Math.Atan(Y[i] / X[i]);
                B0 = Math.Atan(Z[i]/ Math.Sqrt(X[i] * X[i] + Y[i] * Y[i]));
                while (count < 8)
                {
                    N = a / Math.Sqrt(1 - Ee * Math.Sin(B0) * Math.Sin(B0));
                    //H[i] = Z[i] / Math.Sin(B0) - N * (1 - Ee );
                    B[i] = Math.Atan((Z[i] +N*Ee*Math.Sin(B0))/ (Math.Sqrt(X[i] * X[i] + Y[i] * Y[i])));
                    B0 = B[i];
                    count++;
                }
                B[i] = B[i] * 180 / Math.PI;
                L[i] = L[i] * 180 / Math.PI;
                
            }
            //chartAreas属性
            chart2.ChartAreas[0].AxisX.Title = "Lon(°)";
            chart2.ChartAreas[0].AxisY.Title = "Lat(°)";
            chart2.ChartAreas[0].AxisX.Maximum = 180;
            chart2.ChartAreas[0].AxisX.Minimum = -180;
            chart2.ChartAreas[0].AxisY.Maximum = 90;
            chart2.ChartAreas[0].AxisY.Minimum = -90;
            chart2.Series[0].Points.DataBindXY(L,B);            
            double[] T = new double[86400 / time];
            double[] dx = new double[86400 / time];
            double[] dy = new double[86400 / time];
            double[] dz = new double[86400 / time];
            int j = 0;
            for (int i = 0; i < 86400 / time; i++)
            {
                T[i] = time * i;
            }
            for (int i = 0; i < Table.Rows.Count; i++)
            {
                if (Convert.ToString(Table.Rows[i]["PRN"]) == toolStripComboBox2.Text )
                {
                    if (Convert.ToString(Table.Rows[i]["dx"]) != "")
                    {
                        dx[j] = Convert.ToDouble(Table.Rows[i]["dx"]);
                        dy[j] = Convert.ToDouble(Table.Rows[i]["dy"]);
                        dz[j] = Convert.ToDouble(Table.Rows[i]["dz"]);

                    }
                    else
                    {
                        dx[j] = double.NaN;
                        dy[j] = double.NaN;
                        dz[j] = double.NaN;
                    }

                    j++;
                }




            }
            //chartAreas属性
            chart2.ChartAreas[1].AxisX.Title = "t(s)";
            chart2.ChartAreas[1].AxisY.Title = "d(m)";

            //将X轴上格网取消
            chart2.ChartAreas[1].AxisX.MajorGrid.Enabled = false;
            //chart2.ChartAreas[1].AxisY.MajorGrid.Enabled = false;

            chart2.Series[1].Points.DataBindXY(T, dx);
            chart2.Series[2].Points.DataBindXY(T, dy);
            chart2.Series[3].Points.DataBindXY(T, dz);

        }

        private void gF组合ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            double λ_L1 = c / L1;
            double λ_L2 = c / L2;
            double λ_B1 = c / B1;
            double λ_B3 = c / B3;

            if (Table1.Rows.Count==0)
            {
                for(int t=0;t<Convert.ToInt32(86400/INTERVAL);t++)
                {
                    Table1.Rows.Add();
                    Table1.Rows[t * 11]["历元数"] = t;
                    Table1.Rows[t * 11]["PRN"] = "G02";if (G02[t, 2] != 0 && G02[t, 3] != 0) { Table1.Rows[t * 11]["GF"] = λ_L1 * G02[t, 2] - λ_L2 * G02[t, 3]; }
                    Table1.Rows.Add();
                    Table1.Rows[t * 11+1]["PRN"] = "G05"; if (G05[t, 2] != 0 && G05[t, 3] != 0) { Table1.Rows[t * 11 + 1]["GF"] = λ_L1 * G05[t, 2] - λ_L2 * G05[t, 3]; }
                    Table1.Rows.Add();
                    Table1.Rows[t * 11+2]["PRN"] = "G13"; if (G13[t, 2] != 0 && G13[t, 3] != 0) { Table1.Rows[t * 11 + 2]["GF"] = λ_L1 * G13[t, 2] - λ_L2 * G13[t, 3]; }
                    Table1.Rows.Add();
                    Table1.Rows[t * 11+3]["PRN"] = "G15"; if (G15[t, 2] != 0 && G15[t, 3] != 0) { Table1.Rows[t * 11 + 3]["GF"] = λ_L1 * G15[t, 2] - λ_L2 * G15[t, 3]; }
                    Table1.Rows.Add();
                    Table1.Rows[t * 11+4]["PRN"] = "G21"; if (G21[t, 2] != 0 && G21[t, 3] != 0) { Table1.Rows[t * 11 + 4]["GF"] = λ_L1 * G21[t, 2] - λ_L2 * G21[t, 3]; }
                    Table1.Rows.Add();
                    Table1.Rows[t * 11+5]["PRN"] = "C01"; if (C01[t, 2] != 0 && C01[t, 3] != 0) { Table1.Rows[t * 11 + 5]["GF"] = λ_B1 * C01[t, 2] - λ_B3 * C01[t, 3]; }
                    Table1.Rows.Add();
                    Table1.Rows[t * 11+6]["PRN"] = "C03"; if (C03[t, 2] != 0 && C03[t, 3] != 0) { Table1.Rows[t * 11 + 6]["GF"] = λ_B1 * C03[t, 2] - λ_B3 * C03[t, 3]; }
                    Table1.Rows.Add();
                    Table1.Rows[t * 11+7]["PRN"] = "C06"; if (C06[t, 2] != 0 && C06[t, 3] != 0) { Table1.Rows[t * 11 + 7]["GF"] = λ_B1 * C06[t, 2] - λ_B3 * C06[t, 3]; }
                    Table1.Rows.Add();
                    Table1.Rows[t * 11+8]["PRN"] = "C08"; if (C08[t, 2] != 0 && C08[t, 3] != 0) { Table1.Rows[t * 11 + 8]["GF"] = λ_B1 * C08[t, 2] - λ_B3 * C08[t, 3]; }
                    Table1.Rows.Add();
                    Table1.Rows[t * 11+9]["PRN"] = "C11"; if (C11[t, 2] != 0 && C11[t, 3] != 0) { Table1.Rows[t * 11 + 9]["GF"] = λ_B1 * C11[t, 2] - λ_B3 * C11[t, 3]; }
                    Table1.Rows.Add();
                    Table1.Rows[t * 11+10]["PRN"] = "C12"; if (C12[t, 2] != 0 && C12[t, 3] != 0) { Table1.Rows[t * 11 + 10]["GF"] = λ_B1 * C12[t, 2] - λ_B3 * C12[t, 3]; }
                }
            }
            else
            {
                for (int t = 0; t < Convert.ToInt32(86400 / INTERVAL); t++)
                {
                    Table1.Rows[t * 11]["历元数"] = t;
                    Table1.Rows[t * 11]["PRN"] = "G02"; if (G02[t, 2] != 0 && G02[t, 3] != 0) { Table1.Rows[t * 11]["GF"] = λ_L1 * G02[t, 2] - λ_L2 * G02[t, 3]; }
                    Table1.Rows[t * 11 + 1]["PRN"] = "G05"; if (G05[t, 2] != 0 && G05[t, 3] != 0) { Table1.Rows[t * 11 + 1]["GF"] = λ_L1 * G05[t, 2] - λ_L2 * G05[t, 3]; }
                    Table1.Rows[t * 11 + 2]["PRN"] = "G13"; if (G13[t, 2] != 0 && G13[t, 3] != 0) { Table1.Rows[t * 11 + 2]["GF"] = λ_L1 * G13[t, 2] - λ_L2 * G13[t, 3]; }
                    Table1.Rows[t * 11 + 3]["PRN"] = "G15"; if (G15[t, 2] != 0 && G15[t, 3] != 0) { Table1.Rows[t * 11 + 3]["GF"] = λ_L1 * G15[t, 2] - λ_L2 * G15[t, 3]; }
                    Table1.Rows[t * 11 + 4]["PRN"] = "G21"; if (G21[t, 2] != 0 && G21[t, 3] != 0) { Table1.Rows[t * 11 + 4]["GF"] = λ_L1 * G21[t, 2] - λ_L2 * G21[t, 3]; }
                    Table1.Rows[t * 11 + 5]["PRN"] = "C01"; if (C01[t, 2] != 0 && C01[t, 3] != 0) { Table1.Rows[t * 11 + 5]["GF"] = λ_B1 * C01[t, 2] - λ_B3 * C01[t, 3]; }
                    Table1.Rows[t * 11 + 6]["PRN"] = "C03"; if (C03[t, 2] != 0 && C03[t, 3] != 0) { Table1.Rows[t * 11 + 6]["GF"] = λ_B1 * C03[t, 2] - λ_B3 * C03[t, 3]; }
                    Table1.Rows[t * 11 + 7]["PRN"] = "C06"; if (C06[t, 2] != 0 && C06[t, 3] != 0) { Table1.Rows[t * 11 + 7]["GF"] = λ_B1 * C06[t, 2] - λ_B3 * C06[t, 3]; }
                    Table1.Rows[t * 11 + 8]["PRN"] = "C08"; if (C08[t, 2] != 0 && C08[t, 3] != 0) { Table1.Rows[t * 11 + 8]["GF"] = λ_B1 * C08[t, 2] - λ_B3 * C08[t, 3]; }
                    Table1.Rows[t * 11 + 9]["PRN"] = "C11"; if (C11[t, 2] != 0 && C11[t, 3] != 0) { Table1.Rows[t * 11 + 9]["GF"] = λ_B1 * C11[t, 2] - λ_B3 * C11[t, 3]; }
                    Table1.Rows[t * 11 + 10]["PRN"] = "C12"; if (C12[t, 2] != 0 && C12[t, 3] != 0) { Table1.Rows[t * 11 + 10]["GF"] = λ_B1 * C12[t, 2] - λ_B3 * C12[t, 3]; }
                }
            }
            dataGridView2.DataSource = Table1;
        }

        private void mW组合ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            double λ_L1 = c / L1;
            double λ_L2 = c / L2;
            double λ_B1 = c / B1;
            double λ_B3 = c / B3;
            double m_L = (L1 - L2) / (L1 + L2) * (1 / λ_L1);
            double n_L = (L1 - L2) / (L1 + L2) * (1 / λ_L2);
            double m_B = (B1 - B3) / (B1 + B3) * (1 / λ_B1);
            double n_B = (B1 - B3) / (B1 + B3) * (1 / λ_B3);
            if (Table1.Rows.Count == 0)
            {
                for (int t = 0; t < Convert.ToInt32(86400 / INTERVAL); t++)
                {
                    Table1.Rows.Add();
                    Table1.Rows[t * 11]["历元数"] = t;
                    Table1.Rows[t * 11]["PRN"] = "G02"; if (G02[t, 0] != 0 && G02[t, 1] != 0&&G02[t, 2] != 0 && G02[t, 3] != 0) { Table1.Rows[t * 11]["MW"] = m_L * G02[t, 0] + n_L * G02[t, 1] - G02[t, 2] + G02[t, 3]; }
                    Table1.Rows.Add();
                    Table1.Rows[t * 11 + 1]["PRN"] = "G05"; if (G05[t, 0] != 0 && G05[t, 1] != 0&&G05[t, 2] != 0 && G05[t, 3] != 0) { Table1.Rows[t * 11 + 1]["MW"] = m_L * G05[t, 0] + n_L * G05[t, 1] - G05[t, 2] + G05[t, 3]; }
                    Table1.Rows.Add();
                    Table1.Rows[t * 11 + 2]["PRN"] = "G13"; if (G13[t, 0] != 0 && G13[t, 1] != 0&&G13[t, 2] != 0 && G13[t, 3] != 0) { Table1.Rows[t * 11 + 2]["MW"] = m_L * G13[t, 0] + n_L * G13[t, 1] - G13[t, 2] + G13[t, 3]; }
                    Table1.Rows.Add();
                    Table1.Rows[t * 11 + 3]["PRN"] = "G15"; if (G15[t, 0] != 0 && G15[t, 1] != 0 && G15[t, 2] != 0 && G15[t, 3] != 0) { Table1.Rows[t * 11 + 3]["MW"] = m_L * G15[t, 0] + n_L * G15[t, 1] - G15[t, 2] + G15[t, 3]; }
                    Table1.Rows.Add();
                    Table1.Rows[t * 11 + 4]["PRN"] = "G21"; if (G21[t, 0] != 0 && G21[t, 1] != 0&&G21[t, 2] != 0 && G21[t, 3] != 0) { Table1.Rows[t * 11 + 4]["MW"] = m_L * G21[t, 0] + n_L * G21[t, 1] - G21[t, 2] + G21[t, 3]; }
                    Table1.Rows.Add();
                    Table1.Rows[t * 11 + 5]["PRN"] = "C01"; if (C01[t, 0] != 0 && C01[t, 1] != 0&&C01[t, 2] != 0 && C01[t, 3] != 0) { Table1.Rows[t * 11 + 5]["MW"] = m_B * C01[t, 0] + n_B * C01[t, 1] - C01[t, 2] + C01[t, 3]; }
                    Table1.Rows.Add();
                    Table1.Rows[t * 11 + 6]["PRN"] = "C03"; if (C03[t, 0] != 0 && C03[t, 1] != 0&&C03[t, 2] != 0 && C03[t, 3] != 0) { Table1.Rows[t * 11 + 6]["MW"] = m_B * C03[t, 0] + n_B * C03[t, 1] - C03[t, 2] + C03[t, 3];  }
                    Table1.Rows.Add();
                    Table1.Rows[t * 11 + 7]["PRN"] = "C06"; if (C06[t, 0] != 0 && C06[t, 1] != 0&&C06[t, 2] != 0 && C06[t, 3] != 0) { Table1.Rows[t * 11 + 7]["MW"] = m_B * C06[t, 0] + n_B * C06[t, 1] - C06[t, 2] + C06[t, 3];  }
                    Table1.Rows.Add();
                    Table1.Rows[t * 11 + 8]["PRN"] = "C08"; if (C08[t, 0] != 0 && C08[t, 1] != 0&&C08[t, 2] != 0 && C08[t, 3] != 0) { Table1.Rows[t * 11 + 8]["MW"] = m_B * C08[t, 0] + n_B * C08[t, 1] - C08[t, 2] + C08[t, 3]; }
                    Table1.Rows.Add();
                    Table1.Rows[t * 11 + 9]["PRN"] = "C11"; if (C11[t, 0] != 0 && C11[t, 1] != 0&&C11[t, 2] != 0 && C11[t, 3] != 0) { Table1.Rows[t * 11 + 9]["MW"] = m_B * C11[t, 0] + n_B * C11[t, 1] - C11[t, 2] + C11[t, 3]; }
                    Table1.Rows.Add();
                    Table1.Rows[t * 11 + 10]["PRN"] = "C12"; if (C12[t, 0] != 0 && C12[t, 1] != 0&&C12[t, 2] != 0 && C12[t, 3] != 0) { Table1.Rows[t * 11 + 10]["MW"] = m_B * C12[t, 0] + n_B * C12[t, 1] - C12[t, 2] + C12[t, 3];  }
                }
            }
            else
            {
                for (int t = 0; t < Convert.ToInt32(86400 / INTERVAL); t++)
                {
                    Table1.Rows[t * 11]["PRN"] = "G02"; if (G02[t, 0] != 0 && G02[t, 1] != 0 && G02[t, 2] != 0 && G02[t, 3] != 0) { Table1.Rows[t * 11]["MW"] = m_L * G02[t, 0] + n_L * G02[t, 1] - G02[t, 2] + G02[t, 3]; }
                    Table1.Rows[t * 11 + 1]["PRN"] = "G05"; if (G05[t, 0] != 0 && G05[t, 1] != 0 && G05[t, 2] != 0 && G05[t, 3] != 0) { Table1.Rows[t * 11 + 1]["MW"] = m_L * G05[t, 0] + n_L * G05[t, 1] - G05[t, 2] + G05[t, 3]; }
                    Table1.Rows[t * 11 + 2]["PRN"] = "G13"; if (G13[t, 0] != 0 && G13[t, 1] != 0 && G13[t, 2] != 0 && G13[t, 3] != 0) { Table1.Rows[t * 11 + 2]["MW"] = m_L * G13[t, 0] + n_L * G13[t, 1] - G13[t, 2] + G13[t, 3]; }
                    Table1.Rows[t * 11 + 3]["PRN"] = "G15"; if (G15[t, 0] != 0 && G15[t, 1] != 0 && G15[t, 2] != 0 && G15[t, 3] != 0) { Table1.Rows[t * 11 + 3]["MW"] = m_L * G15[t, 0] + n_L * G15[t, 1] - G15[t, 2] + G15[t, 3]; }
                    Table1.Rows[t * 11 + 4]["PRN"] = "G21"; if (G21[t, 0] != 0 && G21[t, 1] != 0 && G21[t, 2] != 0 && G21[t, 3] != 0) { Table1.Rows[t * 11 + 4]["MW"] = m_L * G21[t, 0] + n_L * G21[t, 1] - G21[t, 2] + G21[t, 3]; }
                    Table1.Rows[t * 11 + 5]["PRN"] = "C01"; if (C01[t, 0] != 0 && C01[t, 1] != 0 && C01[t, 2] != 0 && C01[t, 3] != 0) { Table1.Rows[t * 11 + 5]["MW"] = m_B * C01[t, 0] + n_B * C01[t, 1] - C01[t, 2] + C01[t, 3]; }
                    Table1.Rows[t * 11 + 6]["PRN"] = "C03"; if (C03[t, 0] != 0 && C03[t, 1] != 0 && C03[t, 2] != 0 && C03[t, 3] != 0) { Table1.Rows[t * 11 + 6]["MW"] = m_B * C03[t, 0] + n_B * C03[t, 1] - C03[t, 2] + C03[t, 3]; }
                    Table1.Rows[t * 11 + 7]["PRN"] = "C06"; if (C06[t, 0] != 0 && C06[t, 1] != 0 && C06[t, 2] != 0 && C06[t, 3] != 0) { Table1.Rows[t * 11 + 7]["MW"] = m_B * C06[t, 0] + n_B * C06[t, 1] - C06[t, 2] + C06[t, 3]; }
                    Table1.Rows[t * 11 + 8]["PRN"] = "C08"; if (C08[t, 0] != 0 && C08[t, 1] != 0 && C08[t, 2] != 0 && C08[t, 3] != 0) { Table1.Rows[t * 11 + 8]["MW"] = m_B * C08[t, 0] + n_B * C08[t, 1] - C08[t, 2] + C08[t, 3]; }
                    Table1.Rows[t * 11 + 9]["PRN"] = "C11"; if (C11[t, 0] != 0 && C11[t, 1] != 0 && C11[t, 2] != 0 && C11[t, 3] != 0) { Table1.Rows[t * 11 + 9]["MW"] = m_B * C11[t, 0] + n_B * C11[t, 1] - C11[t, 2] + C11[t, 3]; }
                    Table1.Rows[t * 11 + 10]["PRN"] = "C12"; if (C12[t, 0] != 0 && C12[t, 1] != 0 && C12[t, 2] != 0 && C12[t, 3] != 0) { Table1.Rows[t * 11 + 10]["MW"] = m_B * C12[t, 0] + n_B * C12[t, 1] - C12[t, 2] + C12[t, 3]; }
                }
            }
            dataGridView2.DataSource = Table1;
        }

        private void 历元间求差ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(Table1.Rows.Count==0)
            {
                gF组合ToolStripMenuItem_Click(sender, e);
                mW组合ToolStripMenuItem_Click(sender, e);
            }
            for (int t = 1; t < Convert.ToInt32(86400 / INTERVAL); t++)
            {
                if (Convert.ToString(Table1.Rows[t * 11]["GF"])!=""&&Convert.ToString(Table1.Rows[(t-1)*11]["MW"])!="") { Table1.Rows[t * 11]["GF历元间求差"] = Convert.ToDouble( Table1.Rows[t * 11]["GF"])-Convert.ToDouble( Table1.Rows[(t-1)*11]["GF"]); Table1.Rows[t * 11]["MW历元间求差"] = Convert.ToDouble(Table1.Rows[t * 11]["MW"]) - Convert.ToDouble(Table1.Rows[(t-1)*11]["MW"]); }
                if (Convert.ToString(Table1.Rows[t * 11+1]["GF"]) != "" && Convert.ToString(Table1.Rows[(t-1)*11+1]["MW"]) != "") { Table1.Rows[t * 11 + 1]["GF历元间求差"] = Convert.ToDouble(Table1.Rows[t * 11+1]["GF"]) - Convert.ToDouble(Table1.Rows[(t-1)*11+1]["GF"]); Table1.Rows[t * 11 + 1]["MW历元间求差"] = Convert.ToDouble(Table1.Rows[t * 11 + 1]["MW"]) - Convert.ToDouble(Table1.Rows[(t-1)*11+1]["MW"]); }
                if (Convert.ToString(Table1.Rows[t * 11+2]["GF"]) != "" && Convert.ToString(Table1.Rows[(t-1)*11+2]["MW"]) != "") { Table1.Rows[t * 11 + 2]["GF历元间求差"] = Convert.ToDouble(Table1.Rows[t * 11+2]["GF"]) - Convert.ToDouble(Table1.Rows[(t-1)*11+2]["GF"]); Table1.Rows[t * 11 + 2]["MW历元间求差"] = Convert.ToDouble(Table1.Rows[t * 11 + 2]["MW"]) - Convert.ToDouble(Table1.Rows[(t-1)*11+2]["MW"]); }
                if (Convert.ToString(Table1.Rows[t * 11+3]["GF"]) != "" && Convert.ToString(Table1.Rows[(t-1)*11+3]["MW"]) != "") { Table1.Rows[t * 11 + 3]["GF历元间求差"] = Convert.ToDouble(Table1.Rows[t * 11+3]["GF"]) - Convert.ToDouble(Table1.Rows[(t-1)*11+3]["GF"]); Table1.Rows[t * 11 + 3]["MW历元间求差"] = Convert.ToDouble(Table1.Rows[t * 11 + 3]["MW"]) - Convert.ToDouble(Table1.Rows[(t-1)*11+3]["MW"]); }
                if (Convert.ToString(Table1.Rows[t * 11+4]["GF"]) != "" && Convert.ToString(Table1.Rows[(t-1)*11+4]["MW"]) != "") { Table1.Rows[t * 11 + 4]["GF历元间求差"] = Convert.ToDouble(Table1.Rows[t * 11+4]["GF"]) - Convert.ToDouble(Table1.Rows[(t-1)*11+4]["GF"]); Table1.Rows[t * 11 + 4]["MW历元间求差"] = Convert.ToDouble(Table1.Rows[t * 11 + 4]["MW"]) - Convert.ToDouble(Table1.Rows[(t-1)*11+4]["MW"]); }
                if (Convert.ToString(Table1.Rows[t * 11+5]["GF"]) != "" && Convert.ToString(Table1.Rows[(t-1)*11+5]["MW"]) != "") { Table1.Rows[t * 11 + 5]["GF历元间求差"] = Convert.ToDouble(Table1.Rows[t * 11+5]["GF"]) - Convert.ToDouble(Table1.Rows[(t-1)*11+5]["GF"]); Table1.Rows[t * 11 + 5]["MW历元间求差"] = Convert.ToDouble(Table1.Rows[t * 11 + 5]["MW"]) - Convert.ToDouble(Table1.Rows[(t-1)*11+5]["MW"]); }
                if (Convert.ToString(Table1.Rows[t * 11+6]["GF"]) != "" && Convert.ToString(Table1.Rows[(t-1)*11+6]["MW"]) != "") { Table1.Rows[t * 11 + 6]["GF历元间求差"] = Convert.ToDouble(Table1.Rows[t * 11+6]["GF"]) - Convert.ToDouble(Table1.Rows[(t-1)*11+6]["GF"]); Table1.Rows[t * 11 + 6]["MW历元间求差"] = Convert.ToDouble(Table1.Rows[t * 11 + 6]["MW"]) - Convert.ToDouble(Table1.Rows[(t-1)*11+6]["MW"]); }
                if (Convert.ToString(Table1.Rows[t * 11+7]["GF"]) != "" && Convert.ToString(Table1.Rows[(t-1)*11+7]["MW"]) != "") { Table1.Rows[t * 11 + 7]["GF历元间求差"] = Convert.ToDouble(Table1.Rows[t * 11+7]["GF"]) - Convert.ToDouble(Table1.Rows[(t-1)*11+7]["GF"]); Table1.Rows[t * 11 + 7]["MW历元间求差"] = Convert.ToDouble(Table1.Rows[t * 11 + 7]["MW"]) - Convert.ToDouble(Table1.Rows[(t-1)*11+7]["MW"]); }
                if (Convert.ToString(Table1.Rows[t * 11+8]["GF"]) != "" && Convert.ToString(Table1.Rows[(t-1)*11+8]["MW"]) != "") { Table1.Rows[t * 11 + 8]["GF历元间求差"] = Convert.ToDouble(Table1.Rows[t * 11+8]["GF"]) - Convert.ToDouble(Table1.Rows[(t-1)*11+8]["GF"]); Table1.Rows[t * 11 + 8]["MW历元间求差"] = Convert.ToDouble(Table1.Rows[t * 11 + 8]["MW"]) - Convert.ToDouble(Table1.Rows[(t-1)*11+8]["MW"]); }
                if (Convert.ToString(Table1.Rows[t * 11+9]["GF"]) != "" && Convert.ToString(Table1.Rows[(t-1)*11+9]["MW"]) != "") { Table1.Rows[t * 11 + 9]["GF历元间求差"] = Convert.ToDouble(Table1.Rows[t * 11+9]["GF"]) - Convert.ToDouble(Table1.Rows[(t-1)*11+9]["GF"]); Table1.Rows[t * 11 + 9]["MW历元间求差"] = Convert.ToDouble(Table1.Rows[t * 11 + 9]["MW"]) - Convert.ToDouble(Table1.Rows[(t-1)*11+9]["MW"]); }
                if (Convert.ToString(Table1.Rows[t * 11+10]["GF"]) != "" && Convert.ToString(Table1.Rows[(t-1)*11+10]["MW"]) != "") { Table1.Rows[t * 11 + 10]["GF历元间求差"] = Convert.ToDouble(Table1.Rows[t * 11+10]["GF"]) - Convert.ToDouble(Table1.Rows[(t-1)*11+10]["GF"]); Table1.Rows[t * 11 + 10]["MW历元间求差"] = Convert.ToDouble(Table1.Rows[t * 11 + 10]["MW"]) - Convert.ToDouble(Table1.Rows[(t-1)*11+10]["MW"]); }
            }
        }

        private void 绘图ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(Table1.Rows.Count==0)
            {
                历元间求差ToolStripMenuItem_Click(sender, e);
            }
            int[] t = new int[Convert.ToInt32(86400 / INTERVAL)];
            double[] GF_G02 = new double[Convert.ToInt32(86400 / INTERVAL)];
            double[] GF_G05 = new double[Convert.ToInt32(86400 / INTERVAL)];
            double[] GF_G13 = new double[Convert.ToInt32(86400 / INTERVAL)];
            double[] GF_G15 = new double[Convert.ToInt32(86400 / INTERVAL)];
            double[] GF_G21 = new double[Convert.ToInt32(86400 / INTERVAL)];
            double[] GF_C01 = new double[Convert.ToInt32(86400 / INTERVAL)];
            double[] GF_C03 = new double[Convert.ToInt32(86400 / INTERVAL)];
            double[] GF_C06 = new double[Convert.ToInt32(86400 / INTERVAL)];
            double[] GF_C08 = new double[Convert.ToInt32(86400 / INTERVAL)];
            double[] GF_C11 = new double[Convert.ToInt32(86400 / INTERVAL)];
            double[] GF_C12 = new double[Convert.ToInt32(86400 / INTERVAL)];
            double[] MW_G02 = new double[Convert.ToInt32(86400 / INTERVAL)];
            double[] MW_G05 = new double[Convert.ToInt32(86400 / INTERVAL)];
            double[] MW_G13 = new double[Convert.ToInt32(86400 / INTERVAL)];
            double[] MW_G15 = new double[Convert.ToInt32(86400 / INTERVAL)];
            double[] MW_G21 = new double[Convert.ToInt32(86400 / INTERVAL)];
            double[] MW_C01 = new double[Convert.ToInt32(86400 / INTERVAL)];
            double[] MW_C03 = new double[Convert.ToInt32(86400 / INTERVAL)];
            double[] MW_C06 = new double[Convert.ToInt32(86400 / INTERVAL)];
            double[] MW_C08 = new double[Convert.ToInt32(86400 / INTERVAL)];
            double[] MW_C11 = new double[Convert.ToInt32(86400 / INTERVAL)];
            double[] MW_C12 = new double[Convert.ToInt32(86400 / INTERVAL)];
            double[] GF历元间求差_G02 = new double[Convert.ToInt32(86400 / INTERVAL)];
            double[] GF历元间求差_G05 = new double[Convert.ToInt32(86400 / INTERVAL)];
            double[] GF历元间求差_G13 = new double[Convert.ToInt32(86400 / INTERVAL)];
            double[] GF历元间求差_G15 = new double[Convert.ToInt32(86400 / INTERVAL)];
            double[] GF历元间求差_G21 = new double[Convert.ToInt32(86400 / INTERVAL)];
            double[] GF历元间求差_C01 = new double[Convert.ToInt32(86400 / INTERVAL)];
            double[] GF历元间求差_C03 = new double[Convert.ToInt32(86400 / INTERVAL)];
            double[] GF历元间求差_C06 = new double[Convert.ToInt32(86400 / INTERVAL)];
            double[] GF历元间求差_C08 = new double[Convert.ToInt32(86400 / INTERVAL)];
            double[] GF历元间求差_C11 = new double[Convert.ToInt32(86400 / INTERVAL)];
            double[] GF历元间求差_C12 = new double[Convert.ToInt32(86400 / INTERVAL)];
            double[] MW历元间求差_G02 = new double[Convert.ToInt32(86400 / INTERVAL)];
            double[] MW历元间求差_G05 = new double[Convert.ToInt32(86400 / INTERVAL)];
            double[] MW历元间求差_G13 = new double[Convert.ToInt32(86400 / INTERVAL)];
            double[] MW历元间求差_G15 = new double[Convert.ToInt32(86400 / INTERVAL)];
            double[] MW历元间求差_G21 = new double[Convert.ToInt32(86400 / INTERVAL)];
            double[] MW历元间求差_C01 = new double[Convert.ToInt32(86400 / INTERVAL)];
            double[] MW历元间求差_C03 = new double[Convert.ToInt32(86400 / INTERVAL)];
            double[] MW历元间求差_C06 = new double[Convert.ToInt32(86400 / INTERVAL)];
            double[] MW历元间求差_C08 = new double[Convert.ToInt32(86400 / INTERVAL)];
            double[] MW历元间求差_C11 = new double[Convert.ToInt32(86400 / INTERVAL)];
            double[] MW历元间求差_C12 = new double[Convert.ToInt32(86400 / INTERVAL)];
            for (int i=0;i<Convert.ToInt32( 86400/INTERVAL);i++)
            {
                t[i] = 30 * i;
                //GF
                if(Convert.ToString(Table1.Rows[i*11]["GF"])!="") {  GF_G02[i] = Convert.ToDouble(Table1.Rows[i * 11]["GF"]); } else { GF_G02[i] = double.NaN; }
                if (Convert.ToString(Table1.Rows[i * 11+1]["GF"]) != "") { GF_G05[i] = Convert.ToDouble(Table1.Rows[i * 11+1]["GF"]); } else { GF_G05[i] = double.NaN; }
                if (Convert.ToString(Table1.Rows[i * 11+2]["GF"]) != "") { GF_G13[i] = Convert.ToDouble(Table1.Rows[i * 11+2]["GF"]); } else { GF_G13[i] = double.NaN; }
                if (Convert.ToString(Table1.Rows[i * 11+3]["GF"]) != "") { GF_G15[i] = Convert.ToDouble(Table1.Rows[i * 11+3]["GF"]); } else { GF_G15[i] = double.NaN; }
                if (Convert.ToString(Table1.Rows[i * 11+4]["GF"]) != "") { GF_G21[i] = Convert.ToDouble(Table1.Rows[i * 11+4]["GF"]); } else { GF_G21[i] = double.NaN; }
                if (Convert.ToString(Table1.Rows[i * 11+5]["GF"]) != "") { GF_C01[i] = Convert.ToDouble(Table1.Rows[i * 11+5]["GF"]); } else { GF_C01[i] = double.NaN; }
                if (Convert.ToString(Table1.Rows[i * 11+6]["GF"]) != "") { GF_C03[i] = Convert.ToDouble(Table1.Rows[i * 11+6]["GF"]); } else { GF_C03[i] = double.NaN; }
                if (Convert.ToString(Table1.Rows[i * 11+7]["GF"]) != "") { GF_C06[i] = Convert.ToDouble(Table1.Rows[i * 11+7]["GF"]); } else { GF_C06[i] = double.NaN; }
                if (Convert.ToString(Table1.Rows[i * 11+8]["GF"]) != "") { GF_C08[i] = Convert.ToDouble(Table1.Rows[i * 11+8]["GF"]); } else { GF_C08[i] = double.NaN; }
                if (Convert.ToString(Table1.Rows[i * 11+9]["GF"]) != "") { GF_C11[i] = Convert.ToDouble(Table1.Rows[i * 11+9]["GF"]); } else { GF_C11[i] = double.NaN; }
                if (Convert.ToString(Table1.Rows[i * 11+10]["GF"]) != "") { GF_C12[i] = Convert.ToDouble(Table1.Rows[i * 11+10]["GF"]); } else { GF_C12[i] = double.NaN; }
                //MW
                if (Convert.ToString(Table1.Rows[i * 11]["MW"]) != "") { MW_G02[i] = Convert.ToDouble(Table1.Rows[i * 11]["MW"]); } else { MW_G02[i] = double.NaN; }
                if (Convert.ToString(Table1.Rows[i * 11 + 1]["MW"]) != "") { MW_G05[i] = Convert.ToDouble(Table1.Rows[i * 11 + 1]["MW"]) ; } else { MW_G05[i] = double.NaN; }
                if (Convert.ToString(Table1.Rows[i * 11 + 2]["MW"]) != "") { MW_G13[i] = Convert.ToDouble(Table1.Rows[i * 11 + 2]["MW"]) ; } else { MW_G13[i] = double.NaN; }
                if (Convert.ToString(Table1.Rows[i * 11 + 3]["MW"]) != "") { MW_G15[i] = Convert.ToDouble(Table1.Rows[i * 11 + 3]["MW"]) ; } else { MW_G15[i] = double.NaN; }
                if (Convert.ToString(Table1.Rows[i * 11 + 4]["MW"]) != "") { MW_G21[i] = Convert.ToDouble(Table1.Rows[i * 11 + 4]["MW"]) ; } else { MW_G21[i] = double.NaN; }
                if (Convert.ToString(Table1.Rows[i * 11 + 5]["MW"]) != "") { MW_C01[i] = Convert.ToDouble(Table1.Rows[i * 11 + 5]["MW"]) ; } else { MW_C01[i] = double.NaN; }
                if (Convert.ToString(Table1.Rows[i * 11 + 6]["MW"]) != "") { MW_C03[i] = Convert.ToDouble(Table1.Rows[i * 11 + 6]["MW"]) ; } else { MW_C03[i] = double.NaN; }
                if (Convert.ToString(Table1.Rows[i * 11 + 7]["MW"]) != "") { MW_C06[i] = Convert.ToDouble(Table1.Rows[i * 11 + 7]["MW"]) ; } else { MW_C06[i] = double.NaN; }
                if (Convert.ToString(Table1.Rows[i * 11 + 8]["MW"]) != "") { MW_C08[i] = Convert.ToDouble(Table1.Rows[i * 11 + 8]["MW"]) ; } else { MW_C08[i] = double.NaN; }
                if (Convert.ToString(Table1.Rows[i * 11 + 9]["MW"]) != "") { MW_C11[i] = Convert.ToDouble(Table1.Rows[i * 11 + 9]["MW"]) ; } else { MW_C11[i] = double.NaN; }
                if (Convert.ToString(Table1.Rows[i * 11 + 10]["MW"]) != "") { MW_C12[i] = Convert.ToDouble(Table1.Rows[i * 11 + 10]["MW"]) ; } else { MW_C12[i] = double.NaN; }
                //GF历元间求差
                if (Convert.ToString(Table1.Rows[i * 11]["GF历元间求差"]) != "") { GF历元间求差_G02[i] = Convert.ToDouble(Table1.Rows[i * 11]["GF历元间求差"]); } else { GF历元间求差_G02[i] = double.NaN; }
                if (Convert.ToString(Table1.Rows[i * 11 + 1]["GF历元间求差"]) != "") { GF历元间求差_G05[i] = Convert.ToDouble(Table1.Rows[i * 11 + 1]["GF历元间求差"]); } else { GF历元间求差_G05[i] = double.NaN; }
                if (Convert.ToString(Table1.Rows[i * 11 + 2]["GF历元间求差"]) != "") { GF历元间求差_G13[i] = Convert.ToDouble(Table1.Rows[i * 11 + 2]["GF历元间求差"]); } else { GF历元间求差_G13[i] = double.NaN; }
                if (Convert.ToString(Table1.Rows[i * 11 + 3]["GF历元间求差"]) != "") { GF历元间求差_G15[i] = Convert.ToDouble(Table1.Rows[i * 11 + 3]["GF历元间求差"]); } else { GF历元间求差_G15[i] = double.NaN; }
                if (Convert.ToString(Table1.Rows[i * 11 + 4]["GF历元间求差"]) != "") { GF历元间求差_G21[i] = Convert.ToDouble(Table1.Rows[i * 11 + 4]["GF历元间求差"]); } else { GF历元间求差_G21[i] = double.NaN; }
                if (Convert.ToString(Table1.Rows[i * 11 + 5]["GF历元间求差"]) != "") { GF历元间求差_C01[i] = Convert.ToDouble(Table1.Rows[i * 11 + 5]["GF历元间求差"]); } else { GF历元间求差_C01[i] = double.NaN; }
                if (Convert.ToString(Table1.Rows[i * 11 + 6]["GF历元间求差"]) != "") { GF历元间求差_C03[i] = Convert.ToDouble(Table1.Rows[i * 11 + 6]["GF历元间求差"]); } else { GF历元间求差_C03[i] = double.NaN; }
                if (Convert.ToString(Table1.Rows[i * 11 + 7]["GF历元间求差"]) != "") { GF历元间求差_C06[i] = Convert.ToDouble(Table1.Rows[i * 11 + 7]["GF历元间求差"]); } else { GF历元间求差_C06[i] = double.NaN; }
                if (Convert.ToString(Table1.Rows[i * 11 + 8]["GF历元间求差"]) != "") { GF历元间求差_C08[i] = Convert.ToDouble(Table1.Rows[i * 11 + 8]["GF历元间求差"]); } else { GF历元间求差_C08[i] = double.NaN; }
                if (Convert.ToString(Table1.Rows[i * 11 + 9]["GF历元间求差"]) != "") { GF历元间求差_C11[i] = Convert.ToDouble(Table1.Rows[i * 11 + 9]["GF历元间求差"]); } else { GF历元间求差_C11[i] = double.NaN; }
                if (Convert.ToString(Table1.Rows[i * 11 + 10]["GF历元间求差"]) != "") { GF历元间求差_C12[i] = Convert.ToDouble(Table1.Rows[i * 11 + 10]["GF历元间求差"]); } else { GF历元间求差_C12[i] = double.NaN; }
                //MW历元间求差
                if (Convert.ToString(Table1.Rows[i * 11]["MW历元间求差"]) != "") { MW历元间求差_G02[i] = Convert.ToDouble(Table1.Rows[i * 11]["MW历元间求差"]); } else { MW历元间求差_G02[i] = double.NaN; }
                if (Convert.ToString(Table1.Rows[i * 11 + 1]["MW历元间求差"]) != "") { MW历元间求差_G05[i] = Convert.ToDouble(Table1.Rows[i * 11 + 1]["MW历元间求差"]) ; } else { MW历元间求差_G05[i] = double.NaN; }
                if (Convert.ToString(Table1.Rows[i * 11 + 2]["MW历元间求差"]) != "") { MW历元间求差_G13[i] = Convert.ToDouble(Table1.Rows[i * 11 + 2]["MW历元间求差"]) ; } else { MW历元间求差_G13[i] = double.NaN; }
                if (Convert.ToString(Table1.Rows[i * 11 + 3]["MW历元间求差"]) != "") { MW历元间求差_G15[i] = Convert.ToDouble(Table1.Rows[i * 11 + 3]["MW历元间求差"]) ; } else { MW历元间求差_G15[i] = double.NaN; }
                if (Convert.ToString(Table1.Rows[i * 11 + 4]["MW历元间求差"]) != "") { MW历元间求差_G21[i] = Convert.ToDouble(Table1.Rows[i * 11 + 4]["MW历元间求差"]); } else { MW历元间求差_G21[i] = double.NaN; }
                if (Convert.ToString(Table1.Rows[i * 11 + 5]["MW历元间求差"]) != "") { MW历元间求差_C01[i] = Convert.ToDouble(Table1.Rows[i * 11 + 5]["MW历元间求差"]) ; } else { MW历元间求差_C01[i] = double.NaN; }
                if (Convert.ToString(Table1.Rows[i * 11 + 6]["MW历元间求差"]) != "") { MW历元间求差_C03[i] = Convert.ToDouble(Table1.Rows[i * 11 + 6]["MW历元间求差"]) ; } else { MW历元间求差_C03[i] = double.NaN; }
                if (Convert.ToString(Table1.Rows[i * 11 + 7]["MW历元间求差"]) != "") { MW历元间求差_C06[i] = Convert.ToDouble(Table1.Rows[i * 11 + 7]["MW历元间求差"]) ; } else { MW历元间求差_C06[i] = double.NaN; }
                if (Convert.ToString(Table1.Rows[i * 11 + 8]["MW历元间求差"]) != "") { MW历元间求差_C08[i] = Convert.ToDouble(Table1.Rows[i * 11 + 8]["MW历元间求差"]) ; } else { MW历元间求差_C08[i] = double.NaN; }
                if (Convert.ToString(Table1.Rows[i * 11 + 9]["MW历元间求差"]) != "") { MW历元间求差_C11[i] = Convert.ToDouble(Table1.Rows[i * 11 + 9]["MW历元间求差"]) / 0.86; } else { MW历元间求差_C11[i] = double.NaN; }
                if (Convert.ToString(Table1.Rows[i * 11 + 10]["MW历元间求差"]) != "") { MW历元间求差_C12[i] = Convert.ToDouble(Table1.Rows[i * 11 + 10]["MW历元间求差"]) / 0.86; } else { MW历元间求差_C12[i] = double.NaN; }

            }
            chart3.ChartAreas[0].AxisX.Title = "t(s)";
            chart3.ChartAreas[0].AxisY.Title = "GF(m)";
            chart3.ChartAreas[0].AxisX.Maximum = 86400;
            chart3.ChartAreas[0].AxisX.Minimum = 0;
            
            //将X.Y轴上格网取消
            chart3.ChartAreas[0].AxisX.MajorGrid.Enabled = false;
            //chart3.ChartAreas[0].AxisY.MajorGrid.Enabled = false;
            chart3.Series[0].ChartType = SeriesChartType.Spline;
            chart3.Series[0].ToolTip = "(#VALX,#VALY)";
            chart3.Series[0].Points.DataBindXY(t, GF_G02);
            chart3.Series[1].ChartType = SeriesChartType.Spline;
            chart3.Series[1].ToolTip = "(#VALX,#VALY)";
            chart3.Series[1].Points.DataBindXY(t, GF_G05);
            chart3.Series[2].ChartType = SeriesChartType.Spline;
            chart3.Series[2].ToolTip = "(#VALX,#VALY)";
            chart3.Series[2].Points.DataBindXY(t, GF_G13);
            chart3.Series[3].ChartType = SeriesChartType.Spline;
            chart3.Series[3].ToolTip = "(#VALX,#VALY)";
            chart3.Series[3].Points.DataBindXY(t, GF_G15);
            chart3.Series[4].ChartType = SeriesChartType.Spline;
            chart3.Series[4].ToolTip = "(#VALX,#VALY)";
            chart3.Series[4].Points.DataBindXY(t, GF_G21);
            chart3.Series[5].ChartType = SeriesChartType.Spline;
            chart3.Series[5].ToolTip = "(#VALX,#VALY)";
            chart3.Series[5].Points.DataBindXY(t, GF_C01);
            chart3.Series[6].ChartType = SeriesChartType.Spline;
            chart3.Series[6].ToolTip = "(#VALX,#VALY)";
            chart3.Series[6].Points.DataBindXY(t, GF_C03);
            chart3.Series[7].ChartType = SeriesChartType.Spline;
            chart3.Series[7].ToolTip = "(#VALX,#VALY)";
            chart3.Series[7].Points.DataBindXY(t, GF_C06);
            chart3.Series[8].ChartType = SeriesChartType.Spline;
            chart3.Series[8].ToolTip = "(#VALX,#VALY)";
            chart3.Series[8].Points.DataBindXY(t, GF_C08);
            chart3.Series[9].ChartType = SeriesChartType.Spline;
            chart3.Series[9].ToolTip = "(#VALX,#VALY)";
            chart3.Series[9].Points.DataBindXY(t, GF_C11);
            chart3.Series[10].ChartType = SeriesChartType.Spline;
            chart3.Series[10].ToolTip = "(#VALX,#VALY)";           
            chart3.Series[10].Points.DataBindXY(t, GF_C12);

            //MW
            chart4.ChartAreas[0].AxisX.Title = "t(s)";
            chart4.ChartAreas[0].AxisY.Title = "MW(周)";
            chart4.ChartAreas[0].AxisX.Maximum = 86400;
            chart4.ChartAreas[0].AxisX.Minimum = 0;

            //将X轴上格网取消
            chart4.ChartAreas[0].AxisX.MajorGrid.Enabled = false;
           // chart4.ChartAreas[0].AxisY.MajorGrid.Enabled = false;
            chart4.Series[0].ChartType = SeriesChartType.Spline;
            chart4.Series[0].ToolTip = "(#VALX,#VALY)";
            chart4.Series[0].Points.DataBindXY(t, MW_G02);
            chart4.Series[1].ChartType = SeriesChartType.Spline;
            chart4.Series[1].ToolTip = "(#VALX,#VALY)";
            chart4.Series[1].Points.DataBindXY(t, MW_G05);
            chart4.Series[2].ChartType = SeriesChartType.Spline;
            chart4.Series[2].ToolTip = "(#VALX,#VALY)";
            chart4.Series[2].Points.DataBindXY(t, MW_G13);
            chart4.Series[3].ChartType = SeriesChartType.Spline;
            chart4.Series[3].ToolTip = "(#VALX,#VALY)";
            chart4.Series[3].Points.DataBindXY(t, MW_G15);
            chart4.Series[4].ChartType = SeriesChartType.Spline;
            chart4.Series[4].ToolTip = "(#VALX,#VALY)";
            chart4.Series[4].Points.DataBindXY(t, MW_G21);
            chart4.Series[5].ChartType = SeriesChartType.Spline;
            chart4.Series[5].ToolTip = "(#VALX,#VALY)";
            chart4.Series[5].Points.DataBindXY(t, MW_C01);
            chart4.Series[6].ChartType = SeriesChartType.Spline;
            chart4.Series[6].ToolTip = "(#VALX,#VALY)";
            chart4.Series[6].Points.DataBindXY(t, MW_C03);
            chart4.Series[7].ChartType = SeriesChartType.Spline;
            chart4.Series[7].ToolTip = "(#VALX,#VALY)";
            chart4.Series[7].Points.DataBindXY(t, MW_C06);
            chart4.Series[8].ChartType = SeriesChartType.Spline;
            chart4.Series[8].ToolTip = "(#VALX,#VALY)";
            chart4.Series[8].Points.DataBindXY(t, MW_C08);
            chart4.Series[9].ChartType = SeriesChartType.Spline;
            chart4.Series[9].ToolTip = "(#VALX,#VALY)";
            chart4.Series[9].Points.DataBindXY(t, MW_C11);
            chart4.Series[10].ChartType = SeriesChartType.Spline;
            chart4.Series[10].ToolTip = "(#VALX,#VALY)";
            chart4.Series[10].Points.DataBindXY(t, MW_C12);

            //GF历元间求差

            chart5.ChartAreas[0].AxisX.Title = "t(s)";
            chart5.ChartAreas[0].AxisY.Title = "GF历元间求差(m)";
            chart5.ChartAreas[0].AxisX.Maximum = 86400;
            chart5.ChartAreas[0].AxisX.Minimum = 0;

            //将X轴上格网取消
            chart5.ChartAreas[0].AxisX.MajorGrid.Enabled = false;
            //chart5.ChartAreas[0].AxisY.MajorGrid.Enabled = false;
            chart5.Series[0].ChartType = SeriesChartType.Spline;
            chart5.Series[0].ToolTip = "(#VALX,#VALY)";
            chart5.Series[0].Points.DataBindXY(t, GF历元间求差_G02);
            chart5.Series[1].ChartType = SeriesChartType.Spline;
            chart5.Series[1].ToolTip = "(#VALX,#VALY)";
            chart5.Series[1].Points.DataBindXY(t, GF历元间求差_G05);
            chart5.Series[2].ChartType = SeriesChartType.Spline;
            chart5.Series[2].ToolTip = "(#VALX,#VALY)";
            chart5.Series[2].Points.DataBindXY(t, GF历元间求差_G13);
            chart5.Series[3].ChartType = SeriesChartType.Spline;
            chart5.Series[3].ToolTip = "(#VALX,#VALY)";
            chart5.Series[3].Points.DataBindXY(t, GF历元间求差_G15);
            chart5.Series[4].ChartType = SeriesChartType.Spline;
            chart5.Series[4].ToolTip = "(#VALX,#VALY)";
            chart5.Series[4].Points.DataBindXY(t, GF历元间求差_G21);
            chart5.Series[5].ChartType = SeriesChartType.Spline;
            chart5.Series[5].ToolTip = "(#VALX,#VALY)";
            chart5.Series[5].Points.DataBindXY(t, GF历元间求差_C01);
            chart5.Series[6].ChartType = SeriesChartType.Spline;
            chart5.Series[6].ToolTip = "(#VALX,#VALY)";
            chart5.Series[6].Points.DataBindXY(t, GF历元间求差_C03);
            chart5.Series[7].ChartType = SeriesChartType.Spline;
            chart5.Series[7].ToolTip = "(#VALX,#VALY)";
            chart5.Series[7].Points.DataBindXY(t, GF历元间求差_C06);
            chart5.Series[8].ChartType = SeriesChartType.Spline;
            chart5.Series[8].ToolTip = "(#VALX,#VALY)";
            chart5.Series[8].Points.DataBindXY(t, GF历元间求差_C08);
            chart5.Series[9].ChartType = SeriesChartType.Spline;
            chart5.Series[9].ToolTip = "(#VALX,#VALY)";
            chart5.Series[9].Points.DataBindXY(t, GF历元间求差_C11);
            chart5.Series[10].ChartType = SeriesChartType.Spline;
            chart5.Series[10].ToolTip = "(#VALX,#VALY)";
            chart5.Series[10].Points.DataBindXY(t, GF历元间求差_C12);

            //MW历元间求差
            chart6.ChartAreas[0].AxisX.Title = "t(s)";
            chart6.ChartAreas[0].AxisY.Title = "MW历元间求差(周)";
            chart6.ChartAreas[0].AxisX.Maximum = 86400;
            chart6.ChartAreas[0].AxisX.Minimum = 0;

            //将X轴上格网取消
            chart6.ChartAreas[0].AxisX.MajorGrid.Enabled = false;
            //chart6.ChartAreas[0].AxisY.MajorGrid.Enabled = false;
            chart6.Series[0].ChartType = SeriesChartType.Spline;
            chart6.Series[0].ToolTip = "(#VALX,#VALY)";
            chart6.Series[0].Points.DataBindXY(t, MW历元间求差_G02);
            chart6.Series[1].ChartType = SeriesChartType.Spline;
            chart6.Series[1].ToolTip = "(#VALX,#VALY)";
            chart6.Series[1].Points.DataBindXY(t, MW历元间求差_G05);
            chart6.Series[2].ChartType = SeriesChartType.Spline;
            chart6.Series[2].ToolTip = "(#VALX,#VALY)";
            chart6.Series[2].Points.DataBindXY(t, MW历元间求差_G13);
            chart6.Series[3].ChartType = SeriesChartType.Spline;
            chart6.Series[3].ToolTip = "(#VALX,#VALY)";
            chart6.Series[3].Points.DataBindXY(t, MW历元间求差_G15);
            chart6.Series[4].ChartType = SeriesChartType.Spline;
            chart6.Series[4].ToolTip = "(#VALX,#VALY)";
            chart6.Series[4].Points.DataBindXY(t, MW历元间求差_G21);
            chart6.Series[5].ChartType = SeriesChartType.Spline;
            chart6.Series[5].ToolTip = "(#VALX,#VALY)";
            chart6.Series[5].Points.DataBindXY(t, MW历元间求差_C01);
            chart6.Series[6].ChartType = SeriesChartType.Spline;
            chart6.Series[6].ToolTip = "(#VALX,#VALY)";
            chart6.Series[6].Points.DataBindXY(t, MW历元间求差_C03);
            chart6.Series[7].ChartType = SeriesChartType.Spline;
            chart6.Series[7].ToolTip = "(#VALX,#VALY)";
            chart6.Series[7].Points.DataBindXY(t, MW历元间求差_C06);
            chart6.Series[8].ChartType = SeriesChartType.Spline;
            chart6.Series[8].ToolTip = "(#VALX,#VALY)";
            chart6.Series[8].Points.DataBindXY(t, MW历元间求差_C08);
            chart6.Series[9].ChartType = SeriesChartType.Spline;
            chart6.Series[9].ToolTip = "(#VALX,#VALY)";
            chart6.Series[9].Points.DataBindXY(t, MW历元间求差_C11);
            chart6.Series[10].ChartType = SeriesChartType.Spline;
            chart6.Series[10].ToolTip = "(#VALX,#VALY)";
            chart6.Series[10].Points.DataBindXY(t, MW历元间求差_C12);
            
        }

        private void toolStripComboBox3_Click(object sender, EventArgs e)
        {
            if(toolStripComboBox3.Text=="G02 ON")
            {
                chart3.Series[0].Enabled = true;
                chart4.Series[0].Enabled = true;
                chart5.Series[0].Enabled = true;
                chart6.Series[0].Enabled = true;
                
            }
            if (toolStripComboBox3.Text == "G02 OFF")
            {
                chart3.Series[0].Enabled = false;
                chart4.Series[0].Enabled = false;
                chart5.Series[0].Enabled = false;
                chart6.Series[0].Enabled = false;
            }
            if (toolStripComboBox3.Text == "G05 ON")
            {
                chart3.Series[1].Enabled = true;
                chart4.Series[1].Enabled = true;
                chart5.Series[1].Enabled = true;
                chart6.Series[1].Enabled = true;
                
            }
            if (toolStripComboBox3.Text == "G05 OFF")
            {
                chart3.Series[1].Enabled = false;
                chart4.Series[1].Enabled = false;
                chart5.Series[1].Enabled = false;
                chart6.Series[1].Enabled = false;
            }
            if (toolStripComboBox3.Text == "G13 ON")
            {
                chart3.Series[2].Enabled = true;
                chart4.Series[2].Enabled = true;
                chart5.Series[2].Enabled = true;
                chart6.Series[2].Enabled = true;
               
            }
            if (toolStripComboBox3.Text == "G13 OFF")
            {
                chart3.Series[2].Enabled = false;
                chart4.Series[2].Enabled = false;
                chart5.Series[2].Enabled = false;
                chart6.Series[2].Enabled = false;
            }
            if (toolStripComboBox3.Text == "G15 ON")
            {
                chart3.Series[3].Enabled = true;
                chart4.Series[3].Enabled = true;
                chart5.Series[3].Enabled = true;
                chart6.Series[3].Enabled = true;
                
            }
            if (toolStripComboBox3.Text == "G15 OFF")
            {
                chart3.Series[3].Enabled = false;
                chart4.Series[3].Enabled = false;
                chart5.Series[3].Enabled = false;
                chart6.Series[3].Enabled = false;
            }
            if (toolStripComboBox3.Text == "G21 ON")
            {
                chart3.Series[4].Enabled = true;
                chart4.Series[4].Enabled = true;
                chart5.Series[4].Enabled = true;
                chart6.Series[4].Enabled = true;
                
            }
            if (toolStripComboBox3.Text == "G21 OFF")
            {
                chart3.Series[4].Enabled = false;
                chart4.Series[4].Enabled = false;
                chart5.Series[4].Enabled = false;
                chart6.Series[4].Enabled = false;
            }
            if (toolStripComboBox3.Text == "C01 ON")
            {
                chart3.Series[5].Enabled = true;
                chart4.Series[5].Enabled = true;
                chart5.Series[5].Enabled = true;
                chart6.Series[5].Enabled = true;
                
            }
            if (toolStripComboBox3.Text == "C01 OFF")
            {
                chart3.Series[5].Enabled = false;
                chart4.Series[5].Enabled = false;
                chart5.Series[5].Enabled = false;
                chart6.Series[5].Enabled = false;
            }
            if (toolStripComboBox3.Text == "C03 ON")
            {
                chart3.Series[6].Enabled = true;
                chart4.Series[6].Enabled = true;
                chart5.Series[6].Enabled = true;
                chart6.Series[6].Enabled = true;
                
            }
            if (toolStripComboBox3.Text == "C03 OFF")
            {
                chart3.Series[6].Enabled = false;
                chart4.Series[6].Enabled = false;
                chart5.Series[6].Enabled = false;
                chart6.Series[6].Enabled = false;
            }
            if (toolStripComboBox3.Text == "C06 ON")
            {
                chart3.Series[7].Enabled = true;
                chart4.Series[7].Enabled = true;
                chart5.Series[7].Enabled = true;
                chart6.Series[7].Enabled = true;
                
            }
            if (toolStripComboBox3.Text == "C06 OFF")
            {
                chart3.Series[7].Enabled = false;
                chart4.Series[7].Enabled = false;
                chart5.Series[7].Enabled = false;
                chart6.Series[7].Enabled = false;
            }
            if (toolStripComboBox3.Text == "C08 ON")
            {
                chart3.Series[8].Enabled = true;
                chart4.Series[8].Enabled = true;
                chart5.Series[8].Enabled = true;
                chart6.Series[8].Enabled = true;
                
            }
            if (toolStripComboBox3.Text == "C08 OFF")
            {
                chart3.Series[8].Enabled = false;
                chart4.Series[8].Enabled = false;
                chart5.Series[8].Enabled = false;
                chart6.Series[8].Enabled = false;
            }
            if (toolStripComboBox3.Text == "C11 ON")
            {
                chart3.Series[9].Enabled = true;
                chart4.Series[9].Enabled = true;
                chart5.Series[9].Enabled = true;
                chart6.Series[9].Enabled = true;
                
            }
            if (toolStripComboBox3.Text == "C11 OFF")
            {
                chart3.Series[9].Enabled = false;
                chart4.Series[9].Enabled = false;
                chart5.Series[9].Enabled = false;
                chart6.Series[9].Enabled = false;
            }
            if (toolStripComboBox3.Text == "C12 ON")
            {
                chart3.Series[10].Enabled = true;
                chart4.Series[10].Enabled = true;
                chart5.Series[10].Enabled = true;
                chart6.Series[10].Enabled = true;
                
            }
            if (toolStripComboBox3.Text == "C12 OFF")
            {
                chart3.Series[10].Enabled = false;
                chart4.Series[10].Enabled = false;
                chart5.Series[10].Enabled = false;
                chart6.Series[10].Enabled = false;
            }
            if(toolStripComboBox3.Text=="ALL ON")
            {
                chart3.Series[0].Enabled = true;
                chart4.Series[0].Enabled = true;
                chart5.Series[0].Enabled = true;
                chart6.Series[0].Enabled = true;
                chart3.Series[1].Enabled = true;
                chart4.Series[1].Enabled = true;
                chart5.Series[1].Enabled = true;
                chart6.Series[1].Enabled = true;
                chart3.Series[2].Enabled = true;
                chart4.Series[2].Enabled = true;
                chart5.Series[2].Enabled = true;
                chart6.Series[2].Enabled = true;
                chart3.Series[3].Enabled = true;
                chart4.Series[3].Enabled = true;
                chart5.Series[3].Enabled = true;
                chart6.Series[3].Enabled = true;
                chart3.Series[4].Enabled = true;
                chart4.Series[4].Enabled = true;
                chart5.Series[4].Enabled = true;
                chart6.Series[4].Enabled = true;
                chart3.Series[5].Enabled = true;
                chart4.Series[5].Enabled = true;
                chart5.Series[5].Enabled = true;
                chart6.Series[5].Enabled = true;
                chart3.Series[6].Enabled = true;
                chart4.Series[6].Enabled = true;
                chart5.Series[6].Enabled = true;
                chart6.Series[6].Enabled = true;
                chart3.Series[7].Enabled = true;
                chart4.Series[7].Enabled = true;
                chart5.Series[7].Enabled = true;
                chart6.Series[7].Enabled = true;               
                chart3.Series[8].Enabled = true;
                chart4.Series[8].Enabled = true;
                chart5.Series[8].Enabled = true;
                chart6.Series[8].Enabled = true;
                chart3.Series[9].Enabled = true;
                chart4.Series[9].Enabled = true;
                chart5.Series[9].Enabled = true;
                chart6.Series[9].Enabled = true;
                chart3.Series[10].Enabled = true;
                chart4.Series[10].Enabled = true;
                chart5.Series[10].Enabled = true;
                chart6.Series[10].Enabled = true;
            }
            if(toolStripComboBox3.Text=="ALL OFF")
            {
                chart3.Series[0].Enabled = false;
                chart4.Series[0].Enabled = false;
                chart5.Series[0].Enabled = false;
                chart6.Series[0].Enabled = false;
                chart3.Series[1].Enabled = false;
                chart4.Series[1].Enabled = false;
                chart5.Series[1].Enabled = false;
                chart6.Series[1].Enabled = false;
                chart3.Series[2].Enabled = false;
                chart4.Series[2].Enabled = false;
                chart5.Series[2].Enabled = false;
                chart6.Series[2].Enabled = false;
                chart3.Series[3].Enabled = false;
                chart4.Series[3].Enabled = false;
                chart5.Series[3].Enabled = false;
                chart6.Series[3].Enabled = false;
                chart3.Series[4].Enabled = false;
                chart4.Series[4].Enabled = false;
                chart5.Series[4].Enabled = false;
                chart6.Series[4].Enabled = false;
                chart3.Series[5].Enabled = false;
                chart4.Series[5].Enabled = false;
                chart5.Series[5].Enabled = false;
                chart6.Series[5].Enabled = false;
                chart3.Series[6].Enabled = false;
                chart4.Series[6].Enabled = false;
                chart5.Series[6].Enabled = false;
                chart6.Series[6].Enabled = false;
                chart3.Series[7].Enabled = false;
                chart4.Series[7].Enabled = false;
                chart5.Series[7].Enabled = false;
                chart6.Series[7].Enabled = false;
                chart3.Series[8].Enabled = false;
                chart4.Series[8].Enabled = false;
                chart5.Series[8].Enabled = false;
                chart6.Series[8].Enabled = false;
                chart3.Series[9].Enabled = false;
                chart4.Series[9].Enabled = false;
                chart5.Series[9].Enabled = false;
                chart6.Series[9].Enabled = false;
                chart3.Series[10].Enabled = false;
                chart4.Series[10].Enabled = false;
                chart5.Series[10].Enabled = false;
                chart6.Series[10].Enabled = false;
            }

        }

    }

}
