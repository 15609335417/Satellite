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
using System.Windows.Forms.DataVisualization.Charting;

namespace 卫星轨道计算
{
    public partial class Form4 : Form
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
        DataTable Table1;
        public Form4()
        {
            Table1 = new DataTable();
            Table1.Columns.Add("历元数", typeof(double));
            Table1.Columns.Add("PRN", typeof(string));
            Table1.Columns.Add("GF", typeof(double));
            Table1.Columns.Add("MW", typeof(double));
            Table1.Columns.Add("GF历元间求差", typeof(double));
            Table1.Columns.Add("MW历元间求差", typeof(double));
            InitializeComponent();
        }

        private void 打开OToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                string line;
                double second = 0;
                int t = 0;
                APPROX_POSITION_X = 0;
                APPROX_POSITION_Y = 0;
                APPROX_POSITION_Z = 0;
                ANTENNA_DELTA_H = 0;
                ANTENNA_DELTA_E = 0;
                ANTENNA_DELTA_N = 0;
                INTERVAL = 0;
                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    StreamReader reader = new StreamReader(openFileDialog1.FileName);
                    openFileDialog1.Filter = "All(*.*)|*.*|obs(*.O)|*.O|txt(*.txt)|*.txt|tar(*.tar)|*.tar";
                    line = reader.ReadLine();
                    if (line.Substring(60).Contains("RINEX VERSION / TYPE") && line.Substring(5, 1) == "3")
                    {

                        while (!line.Contains("END OF HEADER"))
                        {
                            if (line.Contains("APPROX POSITION XYZ"))
                            {
                                APPROX_POSITION_X = Convert.ToDouble(line.Substring(1, 13));
                                APPROX_POSITION_Y = Convert.ToDouble(line.Substring(15, 13));
                                APPROX_POSITION_Z = Convert.ToDouble(line.Substring(29, 13));
                            }
                            if (line.Contains("ANTENNA: DELTA H/E/N"))
                            {
                                ANTENNA_DELTA_H = Convert.ToDouble(line.Substring(1, 13));
                                ANTENNA_DELTA_E = Convert.ToDouble(line.Substring(15, 13));
                                ANTENNA_DELTA_N = Convert.ToDouble(line.Substring(29, 13));
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
                            richTextBox1.Text += Convert.ToString(G05[i, 0]) + ' ' + Convert.ToString(G05[i, 1]) + ' ' + Convert.ToString(G05[i, 2]) + ' ' + Convert.ToString(G05[i, 3]) + '\n';
                        }
                    }
                    else if (line.Substring(60).Contains("RINEX VERSION / TYPE") && line.Substring(5, 1) == "2")
                    {
                        richTextBox1.Text = null;
                        richTextBox1.Text = line;
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

        private void gF组合ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            double λ_L1 = c / L1;
            double λ_L2 = c / L2;
            double λ_B1 = c / B1;
            double λ_B3 = c / B3;

            if (Table1.Rows.Count == 0)
            {
                for (int t = 0; t < Convert.ToInt32(86400 / INTERVAL); t++)
                {
                    Table1.Rows.Add();
                    Table1.Rows[t * 11]["历元数"] = t;
                    Table1.Rows[t * 11]["PRN"] = "G02"; if (G02[t, 2] != 0 && G02[t, 3] != 0) { Table1.Rows[t * 11]["GF"] = λ_L1 * G02[t, 2] - λ_L2 * G02[t, 3]; }
                    Table1.Rows.Add();
                    Table1.Rows[t * 11 + 1]["PRN"] = "G05"; if (G05[t, 2] != 0 && G05[t, 3] != 0) { Table1.Rows[t * 11 + 1]["GF"] = λ_L1 * G05[t, 2] - λ_L2 * G05[t, 3]; }
                    Table1.Rows.Add();
                    Table1.Rows[t * 11 + 2]["PRN"] = "G13"; if (G13[t, 2] != 0 && G13[t, 3] != 0) { Table1.Rows[t * 11 + 2]["GF"] = λ_L1 * G13[t, 2] - λ_L2 * G13[t, 3]; }
                    Table1.Rows.Add();
                    Table1.Rows[t * 11 + 3]["PRN"] = "G15"; if (G15[t, 2] != 0 && G15[t, 3] != 0) { Table1.Rows[t * 11 + 3]["GF"] = λ_L1 * G15[t, 2] - λ_L2 * G15[t, 3]; }
                    Table1.Rows.Add();
                    Table1.Rows[t * 11 + 4]["PRN"] = "G21"; if (G21[t, 2] != 0 && G21[t, 3] != 0) { Table1.Rows[t * 11 + 4]["GF"] = λ_L1 * G21[t, 2] - λ_L2 * G21[t, 3]; }
                    Table1.Rows.Add();
                    Table1.Rows[t * 11 + 5]["PRN"] = "C01"; if (C01[t, 2] != 0 && C01[t, 3] != 0) { Table1.Rows[t * 11 + 5]["GF"] = λ_B1 * C01[t, 2] - λ_B3 * C01[t, 3]; }
                    Table1.Rows.Add();
                    Table1.Rows[t * 11 + 6]["PRN"] = "C03"; if (C03[t, 2] != 0 && C03[t, 3] != 0) { Table1.Rows[t * 11 + 6]["GF"] = λ_B1 * C03[t, 2] - λ_B3 * C03[t, 3]; }
                    Table1.Rows.Add();
                    Table1.Rows[t * 11 + 7]["PRN"] = "C06"; if (C06[t, 2] != 0 && C06[t, 3] != 0) { Table1.Rows[t * 11 + 7]["GF"] = λ_B1 * C06[t, 2] - λ_B3 * C06[t, 3]; }
                    Table1.Rows.Add();
                    Table1.Rows[t * 11 + 8]["PRN"] = "C08"; if (C08[t, 2] != 0 && C08[t, 3] != 0) { Table1.Rows[t * 11 + 8]["GF"] = λ_B1 * C08[t, 2] - λ_B3 * C08[t, 3]; }
                    Table1.Rows.Add();
                    Table1.Rows[t * 11 + 9]["PRN"] = "C11"; if (C11[t, 2] != 0 && C11[t, 3] != 0) { Table1.Rows[t * 11 + 9]["GF"] = λ_B1 * C11[t, 2] - λ_B3 * C11[t, 3]; }
                    Table1.Rows.Add();
                    Table1.Rows[t * 11 + 10]["PRN"] = "C12"; if (C12[t, 2] != 0 && C12[t, 3] != 0) { Table1.Rows[t * 11 + 10]["GF"] = λ_B1 * C12[t, 2] - λ_B3 * C12[t, 3]; }
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
            dataGridView1.DataSource = Table1;
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
                    Table1.Rows[t * 11]["PRN"] = "G02"; if (G02[t, 0] != 0 && G02[t, 1] != 0 && G02[t, 2] != 0 && G02[t, 3] != 0) { Table1.Rows[t * 11]["MW"] = m_L * G02[t, 0] + n_L * G02[t, 1] - G02[t, 2] + G02[t, 3]; }
                    Table1.Rows.Add();
                    Table1.Rows[t * 11 + 1]["PRN"] = "G05"; if (G05[t, 0] != 0 && G05[t, 1] != 0 && G05[t, 2] != 0 && G05[t, 3] != 0) { Table1.Rows[t * 11 + 1]["MW"] = m_L * G05[t, 0] + n_L * G05[t, 1] - G05[t, 2] + G05[t, 3]; }
                    Table1.Rows.Add();
                    Table1.Rows[t * 11 + 2]["PRN"] = "G13"; if (G13[t, 0] != 0 && G13[t, 1] != 0 && G13[t, 2] != 0 && G13[t, 3] != 0) { Table1.Rows[t * 11 + 2]["MW"] = m_L * G13[t, 0] + n_L * G13[t, 1] - G13[t, 2] + G13[t, 3]; }
                    Table1.Rows.Add();
                    Table1.Rows[t * 11 + 3]["PRN"] = "G15"; if (G15[t, 0] != 0 && G15[t, 1] != 0 && G15[t, 2] != 0 && G15[t, 3] != 0) { Table1.Rows[t * 11 + 3]["MW"] = m_L * G15[t, 0] + n_L * G15[t, 1] - G15[t, 2] + G15[t, 3]; }
                    Table1.Rows.Add();
                    Table1.Rows[t * 11 + 4]["PRN"] = "G21"; if (G21[t, 0] != 0 && G21[t, 1] != 0 && G21[t, 2] != 0 && G21[t, 3] != 0) { Table1.Rows[t * 11 + 4]["MW"] = m_L * G21[t, 0] + n_L * G21[t, 1] - G21[t, 2] + G21[t, 3]; }
                    Table1.Rows.Add();
                    Table1.Rows[t * 11 + 5]["PRN"] = "C01"; if (C01[t, 0] != 0 && C01[t, 1] != 0 && C01[t, 2] != 0 && C01[t, 3] != 0) { Table1.Rows[t * 11 + 5]["MW"] = m_B * C01[t, 0] + n_B * C01[t, 1] - C01[t, 2] + C01[t, 3]; }
                    Table1.Rows.Add();
                    Table1.Rows[t * 11 + 6]["PRN"] = "C03"; if (C03[t, 0] != 0 && C03[t, 1] != 0 && C03[t, 2] != 0 && C03[t, 3] != 0) { Table1.Rows[t * 11 + 6]["MW"] = m_B * C03[t, 0] + n_B * C03[t, 1] - C03[t, 2] + C03[t, 3]; }
                    Table1.Rows.Add();
                    Table1.Rows[t * 11 + 7]["PRN"] = "C06"; if (C06[t, 0] != 0 && C06[t, 1] != 0 && C06[t, 2] != 0 && C06[t, 3] != 0) { Table1.Rows[t * 11 + 7]["MW"] = m_B * C06[t, 0] + n_B * C06[t, 1] - C06[t, 2] + C06[t, 3]; }
                    Table1.Rows.Add();
                    Table1.Rows[t * 11 + 8]["PRN"] = "C08"; if (C08[t, 0] != 0 && C08[t, 1] != 0 && C08[t, 2] != 0 && C08[t, 3] != 0) { Table1.Rows[t * 11 + 8]["MW"] = m_B * C08[t, 0] + n_B * C08[t, 1] - C08[t, 2] + C08[t, 3]; }
                    Table1.Rows.Add();
                    Table1.Rows[t * 11 + 9]["PRN"] = "C11"; if (C11[t, 0] != 0 && C11[t, 1] != 0 && C11[t, 2] != 0 && C11[t, 3] != 0) { Table1.Rows[t * 11 + 9]["MW"] = m_B * C11[t, 0] + n_B * C11[t, 1] - C11[t, 2] + C11[t, 3]; }
                    Table1.Rows.Add();
                    Table1.Rows[t * 11 + 10]["PRN"] = "C12"; if (C12[t, 0] != 0 && C12[t, 1] != 0 && C12[t, 2] != 0 && C12[t, 3] != 0) { Table1.Rows[t * 11 + 10]["MW"] = m_B * C12[t, 0] + n_B * C12[t, 1] - C12[t, 2] + C12[t, 3]; }
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
            dataGridView1.DataSource = Table1;
        }

        private void gF历元间求差ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Table1.Rows.Count == 0)
            {
                gF组合ToolStripMenuItem_Click(sender, e);
                mW组合ToolStripMenuItem_Click(sender, e);
            }
            for (int t = 1; t < Convert.ToInt32(86400 / INTERVAL); t++)
            {
                if (Convert.ToString(Table1.Rows[t * 11]["GF"]) != "" && Convert.ToString(Table1.Rows[(t - 1) * 11]["MW"]) != "") { Table1.Rows[t * 11]["GF历元间求差"] = Convert.ToDouble(Table1.Rows[t * 11]["GF"]) - Convert.ToDouble(Table1.Rows[(t - 1) * 11]["GF"]); Table1.Rows[t * 11]["MW历元间求差"] = Convert.ToDouble(Table1.Rows[t * 11]["MW"]) - Convert.ToDouble(Table1.Rows[(t - 1) * 11]["MW"]); }
                if (Convert.ToString(Table1.Rows[t * 11 + 1]["GF"]) != "" && Convert.ToString(Table1.Rows[(t - 1) * 11 + 1]["MW"]) != "") { Table1.Rows[t * 11 + 1]["GF历元间求差"] = Convert.ToDouble(Table1.Rows[t * 11 + 1]["GF"]) - Convert.ToDouble(Table1.Rows[(t - 1) * 11 + 1]["GF"]); Table1.Rows[t * 11 + 1]["MW历元间求差"] = Convert.ToDouble(Table1.Rows[t * 11 + 1]["MW"]) - Convert.ToDouble(Table1.Rows[(t - 1) * 11 + 1]["MW"]); }
                if (Convert.ToString(Table1.Rows[t * 11 + 2]["GF"]) != "" && Convert.ToString(Table1.Rows[(t - 1) * 11 + 2]["MW"]) != "") { Table1.Rows[t * 11 + 2]["GF历元间求差"] = Convert.ToDouble(Table1.Rows[t * 11 + 2]["GF"]) - Convert.ToDouble(Table1.Rows[(t - 1) * 11 + 2]["GF"]); Table1.Rows[t * 11 + 2]["MW历元间求差"] = Convert.ToDouble(Table1.Rows[t * 11 + 2]["MW"]) - Convert.ToDouble(Table1.Rows[(t - 1) * 11 + 2]["MW"]); }
                if (Convert.ToString(Table1.Rows[t * 11 + 3]["GF"]) != "" && Convert.ToString(Table1.Rows[(t - 1) * 11 + 3]["MW"]) != "") { Table1.Rows[t * 11 + 3]["GF历元间求差"] = Convert.ToDouble(Table1.Rows[t * 11 + 3]["GF"]) - Convert.ToDouble(Table1.Rows[(t - 1) * 11 + 3]["GF"]); Table1.Rows[t * 11 + 3]["MW历元间求差"] = Convert.ToDouble(Table1.Rows[t * 11 + 3]["MW"]) - Convert.ToDouble(Table1.Rows[(t - 1) * 11 + 3]["MW"]); }
                if (Convert.ToString(Table1.Rows[t * 11 + 4]["GF"]) != "" && Convert.ToString(Table1.Rows[(t - 1) * 11 + 4]["MW"]) != "") { Table1.Rows[t * 11 + 4]["GF历元间求差"] = Convert.ToDouble(Table1.Rows[t * 11 + 4]["GF"]) - Convert.ToDouble(Table1.Rows[(t - 1) * 11 + 4]["GF"]); Table1.Rows[t * 11 + 4]["MW历元间求差"] = Convert.ToDouble(Table1.Rows[t * 11 + 4]["MW"]) - Convert.ToDouble(Table1.Rows[(t - 1) * 11 + 4]["MW"]); }
                if (Convert.ToString(Table1.Rows[t * 11 + 5]["GF"]) != "" && Convert.ToString(Table1.Rows[(t - 1) * 11 + 5]["MW"]) != "") { Table1.Rows[t * 11 + 5]["GF历元间求差"] = Convert.ToDouble(Table1.Rows[t * 11 + 5]["GF"]) - Convert.ToDouble(Table1.Rows[(t - 1) * 11 + 5]["GF"]); Table1.Rows[t * 11 + 5]["MW历元间求差"] = Convert.ToDouble(Table1.Rows[t * 11 + 5]["MW"]) - Convert.ToDouble(Table1.Rows[(t - 1) * 11 + 5]["MW"]); }
                if (Convert.ToString(Table1.Rows[t * 11 + 6]["GF"]) != "" && Convert.ToString(Table1.Rows[(t - 1) * 11 + 6]["MW"]) != "") { Table1.Rows[t * 11 + 6]["GF历元间求差"] = Convert.ToDouble(Table1.Rows[t * 11 + 6]["GF"]) - Convert.ToDouble(Table1.Rows[(t - 1) * 11 + 6]["GF"]); Table1.Rows[t * 11 + 6]["MW历元间求差"] = Convert.ToDouble(Table1.Rows[t * 11 + 6]["MW"]) - Convert.ToDouble(Table1.Rows[(t - 1) * 11 + 6]["MW"]); }
                if (Convert.ToString(Table1.Rows[t * 11 + 7]["GF"]) != "" && Convert.ToString(Table1.Rows[(t - 1) * 11 + 7]["MW"]) != "") { Table1.Rows[t * 11 + 7]["GF历元间求差"] = Convert.ToDouble(Table1.Rows[t * 11 + 7]["GF"]) - Convert.ToDouble(Table1.Rows[(t - 1) * 11 + 7]["GF"]); Table1.Rows[t * 11 + 7]["MW历元间求差"] = Convert.ToDouble(Table1.Rows[t * 11 + 7]["MW"]) - Convert.ToDouble(Table1.Rows[(t - 1) * 11 + 7]["MW"]); }
                if (Convert.ToString(Table1.Rows[t * 11 + 8]["GF"]) != "" && Convert.ToString(Table1.Rows[(t - 1) * 11 + 8]["MW"]) != "") { Table1.Rows[t * 11 + 8]["GF历元间求差"] = Convert.ToDouble(Table1.Rows[t * 11 + 8]["GF"]) - Convert.ToDouble(Table1.Rows[(t - 1) * 11 + 8]["GF"]); Table1.Rows[t * 11 + 8]["MW历元间求差"] = Convert.ToDouble(Table1.Rows[t * 11 + 8]["MW"]) - Convert.ToDouble(Table1.Rows[(t - 1) * 11 + 8]["MW"]); }
                if (Convert.ToString(Table1.Rows[t * 11 + 9]["GF"]) != "" && Convert.ToString(Table1.Rows[(t - 1) * 11 + 9]["MW"]) != "") { Table1.Rows[t * 11 + 9]["GF历元间求差"] = Convert.ToDouble(Table1.Rows[t * 11 + 9]["GF"]) - Convert.ToDouble(Table1.Rows[(t - 1) * 11 + 9]["GF"]); Table1.Rows[t * 11 + 9]["MW历元间求差"] = Convert.ToDouble(Table1.Rows[t * 11 + 9]["MW"]) - Convert.ToDouble(Table1.Rows[(t - 1) * 11 + 9]["MW"]); }
                if (Convert.ToString(Table1.Rows[t * 11 + 10]["GF"]) != "" && Convert.ToString(Table1.Rows[(t - 1) * 11 + 10]["MW"]) != "") { Table1.Rows[t * 11 + 10]["GF历元间求差"] = Convert.ToDouble(Table1.Rows[t * 11 + 10]["GF"]) - Convert.ToDouble(Table1.Rows[(t - 1) * 11 + 10]["GF"]); Table1.Rows[t * 11 + 10]["MW历元间求差"] = Convert.ToDouble(Table1.Rows[t * 11 + 10]["MW"]) - Convert.ToDouble(Table1.Rows[(t - 1) * 11 + 10]["MW"]); }
            }
        }

        private void 时间序列图ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Table1.Rows.Count == 0)
            {
                gF历元间求差ToolStripMenuItem_Click(sender, e);
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
            for (int i = 0; i < Convert.ToInt32(86400 / INTERVAL); i++)
            {
                t[i] = 30 * i;
                //GF
                if (Convert.ToString(Table1.Rows[i * 11]["GF"]) != "") { GF_G02[i] = Convert.ToDouble(Table1.Rows[i * 11]["GF"]); } else { GF_G02[i] = double.NaN; }
                if (Convert.ToString(Table1.Rows[i * 11 + 1]["GF"]) != "") { GF_G05[i] = Convert.ToDouble(Table1.Rows[i * 11 + 1]["GF"]); } else { GF_G05[i] = double.NaN; }
                if (Convert.ToString(Table1.Rows[i * 11 + 2]["GF"]) != "") { GF_G13[i] = Convert.ToDouble(Table1.Rows[i * 11 + 2]["GF"]); } else { GF_G13[i] = double.NaN; }
                if (Convert.ToString(Table1.Rows[i * 11 + 3]["GF"]) != "") { GF_G15[i] = Convert.ToDouble(Table1.Rows[i * 11 + 3]["GF"]); } else { GF_G15[i] = double.NaN; }
                if (Convert.ToString(Table1.Rows[i * 11 + 4]["GF"]) != "") { GF_G21[i] = Convert.ToDouble(Table1.Rows[i * 11 + 4]["GF"]); } else { GF_G21[i] = double.NaN; }
                if (Convert.ToString(Table1.Rows[i * 11 + 5]["GF"]) != "") { GF_C01[i] = Convert.ToDouble(Table1.Rows[i * 11 + 5]["GF"]); } else { GF_C01[i] = double.NaN; }
                if (Convert.ToString(Table1.Rows[i * 11 + 6]["GF"]) != "") { GF_C03[i] = Convert.ToDouble(Table1.Rows[i * 11 + 6]["GF"]); } else { GF_C03[i] = double.NaN; }
                if (Convert.ToString(Table1.Rows[i * 11 + 7]["GF"]) != "") { GF_C06[i] = Convert.ToDouble(Table1.Rows[i * 11 + 7]["GF"]); } else { GF_C06[i] = double.NaN; }
                if (Convert.ToString(Table1.Rows[i * 11 + 8]["GF"]) != "") { GF_C08[i] = Convert.ToDouble(Table1.Rows[i * 11 + 8]["GF"]); } else { GF_C08[i] = double.NaN; }
                if (Convert.ToString(Table1.Rows[i * 11 + 9]["GF"]) != "") { GF_C11[i] = Convert.ToDouble(Table1.Rows[i * 11 + 9]["GF"]); } else { GF_C11[i] = double.NaN; }
                if (Convert.ToString(Table1.Rows[i * 11 + 10]["GF"]) != "") { GF_C12[i] = Convert.ToDouble(Table1.Rows[i * 11 + 10]["GF"]); } else { GF_C12[i] = double.NaN; }
                //MW
                if (Convert.ToString(Table1.Rows[i * 11]["MW"]) != "") { MW_G02[i] = Convert.ToDouble(Table1.Rows[i * 11]["MW"]); } else { MW_G02[i] = double.NaN; }
                if (Convert.ToString(Table1.Rows[i * 11 + 1]["MW"]) != "") { MW_G05[i] = Convert.ToDouble(Table1.Rows[i * 11 + 1]["MW"]); } else { MW_G05[i] = double.NaN; }
                if (Convert.ToString(Table1.Rows[i * 11 + 2]["MW"]) != "") { MW_G13[i] = Convert.ToDouble(Table1.Rows[i * 11 + 2]["MW"]); } else { MW_G13[i] = double.NaN; }
                if (Convert.ToString(Table1.Rows[i * 11 + 3]["MW"]) != "") { MW_G15[i] = Convert.ToDouble(Table1.Rows[i * 11 + 3]["MW"]); } else { MW_G15[i] = double.NaN; }
                if (Convert.ToString(Table1.Rows[i * 11 + 4]["MW"]) != "") { MW_G21[i] = Convert.ToDouble(Table1.Rows[i * 11 + 4]["MW"]); } else { MW_G21[i] = double.NaN; }
                if (Convert.ToString(Table1.Rows[i * 11 + 5]["MW"]) != "") { MW_C01[i] = Convert.ToDouble(Table1.Rows[i * 11 + 5]["MW"]); } else { MW_C01[i] = double.NaN; }
                if (Convert.ToString(Table1.Rows[i * 11 + 6]["MW"]) != "") { MW_C03[i] = Convert.ToDouble(Table1.Rows[i * 11 + 6]["MW"]); } else { MW_C03[i] = double.NaN; }
                if (Convert.ToString(Table1.Rows[i * 11 + 7]["MW"]) != "") { MW_C06[i] = Convert.ToDouble(Table1.Rows[i * 11 + 7]["MW"]); } else { MW_C06[i] = double.NaN; }
                if (Convert.ToString(Table1.Rows[i * 11 + 8]["MW"]) != "") { MW_C08[i] = Convert.ToDouble(Table1.Rows[i * 11 + 8]["MW"]); } else { MW_C08[i] = double.NaN; }
                if (Convert.ToString(Table1.Rows[i * 11 + 9]["MW"]) != "") { MW_C11[i] = Convert.ToDouble(Table1.Rows[i * 11 + 9]["MW"]); } else { MW_C11[i] = double.NaN; }
                if (Convert.ToString(Table1.Rows[i * 11 + 10]["MW"]) != "") { MW_C12[i] = Convert.ToDouble(Table1.Rows[i * 11 + 10]["MW"]); } else { MW_C12[i] = double.NaN; }
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
                if (Convert.ToString(Table1.Rows[i * 11 + 1]["MW历元间求差"]) != "") { MW历元间求差_G05[i] = Convert.ToDouble(Table1.Rows[i * 11 + 1]["MW历元间求差"]); } else { MW历元间求差_G05[i] = double.NaN; }
                if (Convert.ToString(Table1.Rows[i * 11 + 2]["MW历元间求差"]) != "") { MW历元间求差_G13[i] = Convert.ToDouble(Table1.Rows[i * 11 + 2]["MW历元间求差"]); } else { MW历元间求差_G13[i] = double.NaN; }
                if (Convert.ToString(Table1.Rows[i * 11 + 3]["MW历元间求差"]) != "") { MW历元间求差_G15[i] = Convert.ToDouble(Table1.Rows[i * 11 + 3]["MW历元间求差"]); } else { MW历元间求差_G15[i] = double.NaN; }
                if (Convert.ToString(Table1.Rows[i * 11 + 4]["MW历元间求差"]) != "") { MW历元间求差_G21[i] = Convert.ToDouble(Table1.Rows[i * 11 + 4]["MW历元间求差"]); } else { MW历元间求差_G21[i] = double.NaN; }
                if (Convert.ToString(Table1.Rows[i * 11 + 5]["MW历元间求差"]) != "") { MW历元间求差_C01[i] = Convert.ToDouble(Table1.Rows[i * 11 + 5]["MW历元间求差"]); } else { MW历元间求差_C01[i] = double.NaN; }
                if (Convert.ToString(Table1.Rows[i * 11 + 6]["MW历元间求差"]) != "") { MW历元间求差_C03[i] = Convert.ToDouble(Table1.Rows[i * 11 + 6]["MW历元间求差"]); } else { MW历元间求差_C03[i] = double.NaN; }
                if (Convert.ToString(Table1.Rows[i * 11 + 7]["MW历元间求差"]) != "") { MW历元间求差_C06[i] = Convert.ToDouble(Table1.Rows[i * 11 + 7]["MW历元间求差"]); } else { MW历元间求差_C06[i] = double.NaN; }
                if (Convert.ToString(Table1.Rows[i * 11 + 8]["MW历元间求差"]) != "") { MW历元间求差_C08[i] = Convert.ToDouble(Table1.Rows[i * 11 + 8]["MW历元间求差"]); } else { MW历元间求差_C08[i] = double.NaN; }
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
        private void toolStripComboBox1_Click(object sender, EventArgs e)
        {
            if (toolStripComboBox1.Text == "G02 ON")
            {
                chart3.Series[0].Enabled = true;
                chart4.Series[0].Enabled = true;
                chart5.Series[0].Enabled = true;
                chart6.Series[0].Enabled = true;

            }
            if (toolStripComboBox1.Text == "G02 OFF")
            {
                chart3.Series[0].Enabled = false;
                chart4.Series[0].Enabled = false;
                chart5.Series[0].Enabled = false;
                chart6.Series[0].Enabled = false;
            }
            if (toolStripComboBox1.Text == "G05 ON")
            {
                chart3.Series[1].Enabled = true;
                chart4.Series[1].Enabled = true;
                chart5.Series[1].Enabled = true;
                chart6.Series[1].Enabled = true;

            }
            if (toolStripComboBox1.Text == "G05 OFF")
            {
                chart3.Series[1].Enabled = false;
                chart4.Series[1].Enabled = false;
                chart5.Series[1].Enabled = false;
                chart6.Series[1].Enabled = false;
            }
            if (toolStripComboBox1.Text == "G13 ON")
            {
                chart3.Series[2].Enabled = true;
                chart4.Series[2].Enabled = true;
                chart5.Series[2].Enabled = true;
                chart6.Series[2].Enabled = true;

            }
            if (toolStripComboBox1.Text == "G13 OFF")
            {
                chart3.Series[2].Enabled = false;
                chart4.Series[2].Enabled = false;
                chart5.Series[2].Enabled = false;
                chart6.Series[2].Enabled = false;
            }
            if (toolStripComboBox1.Text == "G15 ON")
            {
                chart3.Series[3].Enabled = true;
                chart4.Series[3].Enabled = true;
                chart5.Series[3].Enabled = true;
                chart6.Series[3].Enabled = true;

            }
            if (toolStripComboBox1.Text == "G15 OFF")
            {
                chart3.Series[3].Enabled = false;
                chart4.Series[3].Enabled = false;
                chart5.Series[3].Enabled = false;
                chart6.Series[3].Enabled = false;
            }
            if (toolStripComboBox1.Text == "G21 ON")
            {
                chart3.Series[4].Enabled = true;
                chart4.Series[4].Enabled = true;
                chart5.Series[4].Enabled = true;
                chart6.Series[4].Enabled = true;

            }
            if (toolStripComboBox1.Text == "G21 OFF")
            {
                chart3.Series[4].Enabled = false;
                chart4.Series[4].Enabled = false;
                chart5.Series[4].Enabled = false;
                chart6.Series[4].Enabled = false;
            }
            if (toolStripComboBox1.Text == "C01 ON")
            {
                chart3.Series[5].Enabled = true;
                chart4.Series[5].Enabled = true;
                chart5.Series[5].Enabled = true;
                chart6.Series[5].Enabled = true;

            }
            if (toolStripComboBox1.Text == "C01 OFF")
            {
                chart3.Series[5].Enabled = false;
                chart4.Series[5].Enabled = false;
                chart5.Series[5].Enabled = false;
                chart6.Series[5].Enabled = false;
            }
            if (toolStripComboBox1.Text == "C03 ON")
            {
                chart3.Series[6].Enabled = true;
                chart4.Series[6].Enabled = true;
                chart5.Series[6].Enabled = true;
                chart6.Series[6].Enabled = true;

            }
            if (toolStripComboBox1.Text == "C03 OFF")
            {
                chart3.Series[6].Enabled = false;
                chart4.Series[6].Enabled = false;
                chart5.Series[6].Enabled = false;
                chart6.Series[6].Enabled = false;
            }
            if (toolStripComboBox1.Text == "C06 ON")
            {
                chart3.Series[7].Enabled = true;
                chart4.Series[7].Enabled = true;
                chart5.Series[7].Enabled = true;
                chart6.Series[7].Enabled = true;

            }
            if (toolStripComboBox1.Text == "C06 OFF")
            {
                chart3.Series[7].Enabled = false;
                chart4.Series[7].Enabled = false;
                chart5.Series[7].Enabled = false;
                chart6.Series[7].Enabled = false;
            }
            if (toolStripComboBox1.Text == "C08 ON")
            {
                chart3.Series[8].Enabled = true;
                chart4.Series[8].Enabled = true;
                chart5.Series[8].Enabled = true;
                chart6.Series[8].Enabled = true;

            }
            if (toolStripComboBox1.Text == "C08 OFF")
            {
                chart3.Series[8].Enabled = false;
                chart4.Series[8].Enabled = false;
                chart5.Series[8].Enabled = false;
                chart6.Series[8].Enabled = false;
            }
            if (toolStripComboBox1.Text == "C11 ON")
            {
                chart3.Series[9].Enabled = true;
                chart4.Series[9].Enabled = true;
                chart5.Series[9].Enabled = true;
                chart6.Series[9].Enabled = true;

            }
            if (toolStripComboBox1.Text == "C11 OFF")
            {
                chart3.Series[9].Enabled = false;
                chart4.Series[9].Enabled = false;
                chart5.Series[9].Enabled = false;
                chart6.Series[9].Enabled = false;
            }
            if (toolStripComboBox1.Text == "C12 ON")
            {
                chart3.Series[10].Enabled = true;
                chart4.Series[10].Enabled = true;
                chart5.Series[10].Enabled = true;
                chart6.Series[10].Enabled = true;

            }
            if (toolStripComboBox1.Text == "C12 OFF")
            {
                chart3.Series[10].Enabled = false;
                chart4.Series[10].Enabled = false;
                chart5.Series[10].Enabled = false;
                chart6.Series[10].Enabled = false;
            }
            if (toolStripComboBox1.Text == "ALL ON")
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
            if (toolStripComboBox1.Text == "ALL OFF")
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

        private void 菜单ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form3 f = new Form3();
            f.Show();
        }

        private void 退出ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form3 f = new Form3();
            f.Show();
            this.Close();

        }

        private void Form4_FormClosing(object sender, FormClosingEventArgs e)
        {
            Form3 f = new Form3();
            f.Show();
        }
    }
}
