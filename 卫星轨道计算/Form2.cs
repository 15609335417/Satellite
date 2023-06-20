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

namespace 卫星轨道计算
{
    public partial class Form2 : Form
    {
        DataTable Table;
        DataTable Table1;
        DataTable Table2;
        string HEADER;
        string BODY;
        string TIME_INF;
        string EMPTY;
        int INTEVAL;
        public Form2()
        {
            HEADER = null;
            BODY = null;
            INTEVAL = 0;
            Table = new DataTable();
            //Table.Columns.Add("历元", typeof(double));
            Table.Columns.Add("Year", typeof(double));
            Table.Columns.Add("Month", typeof(double));
            Table.Columns.Add("Day", typeof(double));
            Table.Columns.Add("Hour", typeof(double));
            Table.Columns.Add("Minute", typeof(double));
            Table.Columns.Add("Second", typeof(double));
            Table.Columns.Add("卫星数", typeof(int));
            Table.Columns.Add("PRN", typeof(string));
            Table.Columns.Add("X", typeof(double));
            Table.Columns.Add("Y", typeof(double));
            Table.Columns.Add("Z", typeof(double));
            Table.Columns.Add("Clock", typeof(double));
            Table1 = new DataTable();
            Table1.Columns.Add("历元", typeof(double));
            //Table.Columns.Add("Year", typeof(double));
            //Table.Columns.Add("Month", typeof(double));
            //Table.Columns.Add("Day", typeof(double));
            //Table.Columns.Add("Hour", typeof(double));
            //Table.Columns.Add("Minute", typeof(double));
            //Table.Columns.Add("Second", typeof(double));
            Table1.Columns.Add("卫星数", typeof(int));
            Table1.Columns.Add("PRN", typeof(string));
            Table1.Columns.Add("X", typeof(double));
            Table1.Columns.Add("Y", typeof(double));
            Table1.Columns.Add("Z", typeof(double));
            Table1.Columns.Add("Clock", typeof(double));
            Table1.Columns.Add("dx", typeof(double));
            Table1.Columns.Add("dy", typeof(double));
            Table1.Columns.Add("dz", typeof(double));
            Table1.Columns.Add("dc", typeof(double));


            Table2 = new DataTable();
            Table2.Columns.Add("历元", typeof(double));
            //Table.Columns.Add("Year", typeof(double));
            //Table.Columns.Add("Month", typeof(double));
            //Table.Columns.Add("Day", typeof(double));
            //Table.Columns.Add("Hour", typeof(double));
            //Table.Columns.Add("Minute", typeof(double));
            //Table.Columns.Add("Second", typeof(double));
            Table2.Columns.Add("卫星数", typeof(int));
            Table2.Columns.Add("PRN", typeof(string));
            Table2.Columns.Add("X", typeof(double));
            Table2.Columns.Add("Y", typeof(double));
            Table2.Columns.Add("Z", typeof(double));
            Table2.Columns.Add("Clock", typeof(double));
            Table2.Columns.Add("dX", typeof(double));
            Table2.Columns.Add("dY", typeof(double));
            Table2.Columns.Add("dZ", typeof(double));
            Table2.Columns.Add("dC", typeof(double));
            InitializeComponent();
        }

        private void 打开sp3ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if(openFileDialog1.ShowDialog()==DialogResult.OK)
                {
                    StreamReader reader = new StreamReader(openFileDialog1.FileName);
                    Table.Rows.Clear();
                    string line;
                    int sat_num = 0;
                    int rows = 0;
                    int t = 0;
                    HEADER = null;
                    line = reader.ReadLine(); HEADER += line + '\n';
                    line = reader.ReadLine(); 
                    INTEVAL = Convert.ToInt32(line.Substring(26, 3));

                    while (line.Substring(0, 1) != "*")
                    {
                        HEADER += line + '\n';

                        line = reader.ReadLine();
                        //HEADER += line;
                    }
                    TIME_INF = line.Substring(0, 13);
                    Table.Rows.Add();
                    while (line != "EOF")
                    {
                        //Table.Rows[rows]["历元"] = t; t += 900;

                        Table.Rows[rows]["Year"] = Convert.ToDouble(line.Substring(3, 4));
                        Table.Rows[rows]["Month"] = Convert.ToDouble(line.Substring(8, 2));
                        Table.Rows[rows]["Day"] = Convert.ToDouble(line.Substring(11, 2));
                        Table.Rows[rows]["Hour"] = Convert.ToDouble(line.Substring(14, 2));
                        Table.Rows[rows]["Minute"] = Convert.ToDouble(line.Substring(17, 2));
                        Table.Rows[rows]["Second"] = Convert.ToDouble(line.Substring(20));
                        line = reader.ReadLine();
                        while (line.Substring(0, 1) != "*" && line != "EOF")
                        {
                            Table.Rows[rows]["PRN"] = line.Substring(0, 4);
                            Table.Rows[rows]["X"] = Convert.ToDouble(line.Substring(5, 13));
                            Table.Rows[rows]["Y"] = Convert.ToDouble(line.Substring(19, 13));
                            Table.Rows[rows]["Z"] = Convert.ToDouble(line.Substring(33, 13));
                            Table.Rows[rows]["Clock"] = Convert.ToDouble(line.Substring(47, 13));
                            line = reader.ReadLine();
                            rows++;sat_num++;
                            if (line != "EOF")
                            {
                                Table.Rows.Add();
                            }
                            else
                                continue;
                        }
                        Table.Rows[rows - sat_num]["卫星数"] = sat_num; sat_num = 0;
                    }
                    dataGridView1.DataSource = Table;
                    
                }

            }
            catch(Exception ex)
            {
                MessageBox.Show(Convert.ToString(ex));
            }
        }

        private void N阶拉格朗日插值(object sender, EventArgs e)
        {
            BODY = null;
            richTextBox1.Text = null;
            int cols = Convert.ToInt32(Table.Rows[0]["卫星数"]);
            int LiYuan = Table.Rows.Count / cols;
            string[] PRN = new string[cols];
            double[,] X = new double[LiYuan, cols];
            double[,] Y = new double[LiYuan, cols];
            double[,] Z = new double[LiYuan, cols];
            double[,] Clock = new double[LiYuan, cols];
            int[] T = new int[LiYuan];
            toolStripComboBox3.Items.Clear();
            toolStripComboBox4.Items.Clear();
            for (int j = 0; j < cols; j++)
            {
                PRN[j] = Convert.ToString(Table.Rows[j]["PRN"]);
                toolStripComboBox3.Items.Add(PRN[j]);
                toolStripComboBox4.Items.Add(PRN[j]);
            }
            for (int i = 0; i < LiYuan; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    X[i, j] = Convert.ToDouble(Table.Rows[j + cols * i]["X"]);
                    Y[i, j] = Convert.ToDouble(Table.Rows[j + cols * i]["Y"]);
                    Z[i, j] = Convert.ToDouble(Table.Rows[j + cols * i]["Z"]);
                    Clock[i, j] = Convert.ToDouble(Table.Rows[j + cols * i]["Clock"]);
                }
                //T[i] = 900 * i;
                Function.HMS2S(Convert.ToDouble(Table.Rows[cols * i]["Hour"]), Convert.ToDouble(Table.Rows[cols * i]["Minute"]), Convert.ToDouble(Table.Rows[cols * i]["Second"]), out  T[i]);
            }
            int n = Convert.ToInt16(toolStripComboBox1.Text);
            double[,] x = new double[LiYuan, cols];
            double[,] y = new double[LiYuan, cols];
            double[,] z = new double[LiYuan, cols];
            double[,] clock = new double[LiYuan, cols];
            double dx = 0;
            double dy = 0;
            double dz = 0;
            double dc = 0;
            double L = 1;
            int foreward = (n - 1) / 2;
            int backward = n - foreward;
            int k = 0;
            int[] t = new int[n];
            int dt;
            int[] time = new int[LiYuan];
            for (int l = 0; l < cols; l++)
            {
                dt = INTEVAL / 2;
                for (int m = 0; m < LiYuan; m++)
                {
                    time[m] = dt;
                    for (int i = 0; i < n; i++)
                    {
                        if (m - foreward < 0)
                        {
                            k = i;
                        }
                        else if (m + backward > 95)
                        {
                            k = LiYuan - n + i;
                        }
                        else
                        {
                            k = m + i - foreward;
                        }
                        t[i] = T[k];
                    }
                    for (int i = 0; i < n; i++)
                    {
                        for (int j = 0; j < n; j++)
                        {
                            if (i != j)
                            {
                                L = L * (dt - t[j]) / (t[i] - t[j]);
                            }
                        }
                        if (m - foreward < 0)
                        {
                            k = i;
                        }
                        else if (m + backward > 95)
                        {
                            k = LiYuan - n + i;
                        }
                        else
                        {
                            k = m + i - foreward;
                        }
                        dx += X[k, l] * L;
                        dy += Y[k, l] * L;
                        dz += Z[k, l] * L;
                        dc += Clock[k, l] * L;
                        L = 1;
                    }
                    x[m, l] = dx;
                    y[m, l] = dy;
                    z[m, l] = dz;
                    clock[m, l] = dc;
                    dx = 0;
                    dy = 0;
                    dz = 0;
                    dc = 0;
                    dt += INTEVAL;
                }
            }
            double[,] X_T = new double[LiYuan * 2, cols];
            double[,] Y_T = new double[LiYuan * 2, cols];
            double[,] Z_T = new double[LiYuan * 2, cols];
            double[,] Clock_T = new double[LiYuan * 2, cols];
            int[] Time = new int[LiYuan * 2];
            for (int i = 0; i < LiYuan; i++)
            {
                Time[i * 2] = T[i];
                Time[i * 2 + 1] = time[i];
                for (int j = 0; j < cols; j++)
                {
                    X_T[i * 2, j] = X[i, j];
                    X_T[i * 2 + 1, j] = x[i, j];
                    Y_T[i * 2, j] = Y[i, j];
                    Y_T[i * 2 + 1, j] = y[i, j];
                    Z_T[i * 2, j] = Z[i, j];
                    Z_T[i * 2 + 1, j] = z[i, j];
                    Clock_T[i * 2, j] = Clock[i, j];
                    Clock_T[i * 2 + 1, j] = clock[i, j];
                }
            }
            Table1.Rows.Clear();
            for (int i = 0; i < LiYuan * 2; i++)
            {
                //double hour, minute, second;
                //hour = Time[i] / 3600;
                //minute = Time[i] % 3600 / 60;
                //second = Time[i] % 60;
                //EMPTY = TIME_INF + " " + Convert.ToString(hour) + " " + Convert.ToString(minute) + " " + Convert.ToString(string.Format("{0:F8}", second));
                //if (EMPTY.Substring(15, 1) == " ")
                //{
                //    EMPTY = EMPTY.Insert(13, " ");
                //}
                //if (EMPTY.Substring(18, 1) == " ")
                //{
                //    EMPTY = EMPTY.Insert(16, " ");
                //}
                //if (EMPTY.Substring(21, 1) == " ")
                //{
                //    EMPTY = EMPTY.Insert(19, " ");
                //}
                //BODY += EMPTY + '\n';
                for (int j = 0; j < cols; j++)
                {
                    Table1.Rows.Add();
                    Table1.Rows[j + cols * i]["PRN"] = PRN[j];
                    Table1.Rows[j + cols * i]["X"] =Math.Round(X_T[i, j],6);
                    Table1.Rows[j + cols * i]["Y"] = Math.Round(Y_T[i, j], 6);
                    Table1.Rows[j + cols * i]["Z"] = Math.Round(Z_T[i, j], 6);
                    Table1.Rows[j + cols * i]["Clock"] = Math.Round(Clock_T[i, j], 6);
                    //EMPTY = Convert.ToString(PRN[j]) + " " + Convert.ToString(string.Format("{0:F6}", X_T[i, j])) + " " + Convert.ToString(string.Format("{0:F6}", Y_T[i, j])) + " " + Convert.ToString(string.Format("{0:F6}", Z_T[i, j])) + " " + Convert.ToString(string.Format("{0:F6}", Clock_T[i, j]));
                    //while (EMPTY.Substring(11, 1) != ".")
                    //{
                    //    EMPTY = EMPTY.Insert(4, " ");
                    //}
                    //while (EMPTY.Substring(25, 1) != ".")
                    //{
                    //    EMPTY = EMPTY.Insert(18, " ");
                    //}
                    //while (EMPTY.Substring(39, 1) != ".")
                    //{
                    //    EMPTY = EMPTY.Insert(32, " ");
                    //}
                    //while (EMPTY.Length < 60)
                    //{
                    //    EMPTY = EMPTY.Insert(46, " ");
                    //}
                    //BODY += EMPTY + '\n';
                }
                Table1.Rows[cols * i]["历元"] = Time[i];
                Table1.Rows[cols * i]["卫星数"] = cols;
            }
            dataGridView2.DataSource = Table1;
            BODY += "EOF" + '\n';
            richTextBox1.Text = HEADER + BODY;
        }

        private void N阶Neville插值(object sender, EventArgs e)
        {
            BODY = null;
            richTextBox2.Text = null;
            int cols = Convert.ToInt32(Table.Rows[0]["卫星数"]);
            int LiYuan = Table.Rows.Count / cols;
            string[] PRN = new string[cols];
            double[,] X = new double[LiYuan, cols];
            double[,] Y = new double[LiYuan, cols];
            double[,] Z = new double[LiYuan, cols];
            double[,] Clock = new double[LiYuan, cols];
            int[] T = new int[LiYuan];
            toolStripComboBox3.Items.Clear();
            toolStripComboBox4.Items.Clear();
            for (int j = 0; j < cols; j++)
            {
                PRN[j] = Convert.ToString(Table.Rows[j]["PRN"]);
                toolStripComboBox3.Items.Add(PRN[j]);
                toolStripComboBox4.Items.Add(PRN[j]);
            }
            for (int i = 0; i < LiYuan; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    X[i, j] = Convert.ToDouble(Table.Rows[j + cols * i]["X"]);
                    Y[i, j] = Convert.ToDouble(Table.Rows[j + cols * i]["Y"]);
                    Z[i, j] = Convert.ToDouble(Table.Rows[j + cols * i]["Z"]);
                    Clock[i, j] = Convert.ToDouble(Table.Rows[j + cols * i]["Clock"]);
                }
                //T[i] = 900 * i;
                Function.HMS2S(Convert.ToDouble(Table.Rows[cols * i]["Hour"]), Convert.ToDouble(Table.Rows[cols * i]["Minute"]), Convert.ToDouble(Table.Rows[cols * i]["Second"]), out T[i]);
            }
            int n = Convert.ToInt16(toolStripComboBox2.Text);
            double[,] x = new double[LiYuan, cols];
            double[,] y = new double[LiYuan, cols];
            double[,] z = new double[LiYuan, cols];
            double[,] clock = new double[LiYuan, cols];
            int foreward = (n - 1) / 2;
            int backward = n - foreward;
            int k = 0;
            int[] t = new int[n];
            double[] x_nev = new double[n];
            double[] y_nev = new double[n];
            double[] z_nev = new double[n];
            double[] clock_nev = new double[n];
            int dt;
            int[] time = new int[LiYuan];
            for (int l = 0; l < cols; l++)
            {
                dt = INTEVAL/2;
                for (int m = 0; m < LiYuan; m++)
                {
                    time[m] = dt;
                    for (int i = 0; i < n; i++)
                    {
                        if (m - foreward < 0)
                        {
                            k = i;
                        }
                        else if (m + backward > 95)
                        {
                            k = LiYuan - n + i;
                        }
                        else
                        {
                            k = m + i - foreward;
                        }
                        t[i] = T[k];
                        x_nev[i] = X[k,l];
                        y_nev[i] = Y[k, l];
                        z_nev[i] = Z[k, l];
                        clock_nev[i] = Clock[k, l];
                        
                    }
                    for(int j=1;j<n;j++)
                    {
                        for(int i=0;i<n-j;i++)
                        {
                            x_nev[i] = ((t[i + j]-dt) * x_nev[i] + (dt-t[i]) * x_nev[i + 1]) / (t[i + j] - t[i]);
                            y_nev[i] = ((t[i + j] - dt) * y_nev[i] + (dt - t[i]) * y_nev[i + 1]) / (t[i + j] - t[i]);
                            z_nev[i] = ((t[i + j] - dt) * z_nev[i] + (dt - t[i]) * z_nev[i + 1]) / (t[i + j] - t[i]);
                            clock_nev[i] = ((t[i + j] - dt) * clock_nev[i] + (dt - t[i]) * clock_nev[i + 1]) / (t[i + j] - t[i]);
                        }
                    }
                    x[m, l] = x_nev[0];
                    y[m, l] = y_nev[0];
                    z[m, l] = z_nev[0];
                    clock[m, l] = clock_nev[0];
                    dt += INTEVAL;
                }
            }
            double[,] X_T = new double[LiYuan * 2, cols];
            double[,] Y_T = new double[LiYuan * 2, cols];
            double[,] Z_T = new double[LiYuan * 2, cols];
            double[,] Clock_T = new double[LiYuan * 2, cols];
            int[] Time = new int[LiYuan * 2];
            for (int i = 0; i < LiYuan; i++)
            {
                Time[i * 2] = T[i];
                Time[i * 2 + 1] = time[i];
                for (int j = 0; j < cols; j++)
                {
                    X_T[i * 2, j] = X[i, j];
                    X_T[i * 2 + 1, j] = x[i, j];
                    Y_T[i * 2, j] = Y[i, j];
                    Y_T[i * 2 + 1, j] = y[i, j];
                    Z_T[i * 2, j] = Z[i, j];
                    Z_T[i * 2 + 1, j] = z[i, j];
                    Clock_T[i * 2, j] = Clock[i, j];
                    Clock_T[i * 2 + 1, j] = clock[i, j];
                }
            }
            Table2.Rows.Clear();
            for (int i = 0; i < LiYuan * 2; i++)
            {
                //double hour, minute, second;
                //hour = Time[i] / 3600;
                //minute = Time[i] % 3600 / 60;
                //second = Time[i] % 60;
                //EMPTY = TIME_INF + " " + Convert.ToString(hour) + " " + Convert.ToString(minute) + " " + Convert.ToString(string.Format("{0:F8}", second));
                //if (EMPTY.Substring(15, 1) == " ")
                //{
                //    EMPTY = EMPTY.Insert(13, " ");
                //}
                //if (EMPTY.Substring(18, 1) == " ")
                //{
                //    EMPTY = EMPTY.Insert(16, " ");
                //}
                //if (EMPTY.Substring(21, 1) == " ")
                //{
                //    EMPTY = EMPTY.Insert(19, " ");
                //}
                //BODY += EMPTY + '\n';
                for (int j = 0; j < cols; j++)
                {
                    Table2.Rows.Add();
                    Table2.Rows[j + cols * i]["PRN"] = PRN[j];
                    Table2.Rows[j + cols * i]["X"] = Math.Round(X_T[i, j], 6);
                    Table2.Rows[j + cols * i]["Y"] = Math.Round(Y_T[i, j], 6);
                    Table2.Rows[j + cols * i]["Z"] = Math.Round(Z_T[i, j], 6);
                    Table2.Rows[j + cols * i]["Clock"] = Math.Round(Clock_T[i, j], 6);
                    //EMPTY = Convert.ToString(PRN[j]) + " " + Convert.ToString(string.Format("{0:F6}", X_T[i, j])) + " " + Convert.ToString(string.Format("{0:F6}", Y_T[i, j])) + " " + Convert.ToString(string.Format("{0:F6}", Z_T[i, j])) + " " + Convert.ToString(string.Format("{0:F6}", Clock_T[i, j]));
                    //while (EMPTY.Substring(11, 1) != ".")
                    //{
                    //    EMPTY = EMPTY.Insert(4, " ");
                    //}
                    //while (EMPTY.Substring(25, 1) != ".")
                    //{
                    //    EMPTY = EMPTY.Insert(18, " ");
                    //}
                    //while (EMPTY.Substring(39, 1) != ".")
                    //{
                    //    EMPTY = EMPTY.Insert(32, " ");
                    //}
                    //while (EMPTY.Length < 60)
                    //{
                    //    EMPTY = EMPTY.Insert(46, " ");
                    //}
                    //BODY += EMPTY + '\n';
                }
                Table2.Rows[cols * i]["历元"] = Time[i];
                Table2.Rows[cols * i]["卫星数"] = cols;
            }
            dataGridView3.DataSource = Table2;
            BODY += "EOF" + '\n';
            richTextBox2.Text = HEADER + BODY;
        }
        
        private void Form2_FormClosing(object sender, FormClosingEventArgs e)
        {
            Form3 f = new Form3();
            f.Show();
        }

        private void 拉格朗日插值ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    StreamWriter writer = new StreamWriter(saveFileDialog1.FileName);
                    //string result = HEADER + BODY;
                    writer.Write(richTextBox1.Text);
                    MessageBox.Show("写入成功！");
                    writer.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void neville插值ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (saveFileDialog2.ShowDialog() == DialogResult.OK)
                {
                    StreamWriter writer = new StreamWriter(saveFileDialog2.FileName);
                    
                    writer.Write(richTextBox2.Text);
                    MessageBox.Show("写入成功！");
                    writer.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void 检验ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int LiYuan;
            
            string[] PRN;
            int cols;
            int[] t;
            double[] dx_l;
            double[] dy_l;
            double[] dz_l;
            double[] dc_l;
            if (Table1.Rows.Count!=0)
            {
                cols = Convert.ToInt32(Table1.Rows[0]["卫星数"]);
                LiYuan = Table.Rows.Count / cols;
                for (int i = Convert.ToInt32(Table1.Rows[0]["卫星数"]); i < Table1.Rows.Count; i++)
                {
                    //cols = Convert.ToInt32(Table1.Rows[i - 1]["卫星数"]);
                    Table1.Rows[i]["dx"] = Convert.ToDouble(Table1.Rows[i]["X"]) - Convert.ToDouble(Table1.Rows[i - cols]["X"]);
                    Table1.Rows[i]["dy"] = Convert.ToDouble(Table1.Rows[i]["Y"]) - Convert.ToDouble(Table1.Rows[i - cols]["Y"]);
                    Table1.Rows[i]["dz"] = Convert.ToDouble(Table1.Rows[i]["Z"]) - Convert.ToDouble(Table1.Rows[i - cols]["Z"]);
                    Table1.Rows[i]["dc"] = Convert.ToDouble(Table1.Rows[i]["Clock"]) - Convert.ToDouble(Table1.Rows[i - cols]["Clock"]);
                }
                dx_l = new double[LiYuan * 2 - 1];
                dy_l = new double[LiYuan * 2 - 1];
                dz_l = new double[LiYuan * 2 - 1];
                dc_l = new double[LiYuan * 2 - 1];
                t = new int[LiYuan * 2 - 1];
                PRN = new string[cols];
                for (int j = 0; j < cols; j++)
                {
                    PRN[j] = Convert.ToString(Table.Rows[j]["PRN"]);
                }
                for (int i = 0; i < LiYuan * 2 - 1; i++)
                {
                    t[i] = INTEVAL/2 * (i + 1);
                    for (int j = 0; j < cols; j++)
                    {
                        if (toolStripComboBox3.Text == PRN[j])
                        {
                            dx_l[i] = Convert.ToDouble(Table1.Rows[j + cols * (i + 1)]["dx"]);
                            dy_l[i] = Convert.ToDouble(Table1.Rows[j + cols * (i + 1)]["dy"]);
                            dz_l[i] = Convert.ToDouble(Table1.Rows[j + cols * (i + 1)]["dz"]);
                            dc_l[i] = Convert.ToDouble(Table1.Rows[j + cols * (i + 1)]["dc"]);
                        }
                    }
                }
                chart1.ChartAreas[0].AxisX.Title = "t(s)";
                chart1.ChartAreas[0].AxisY.Title = "(m)";
                chart1.ChartAreas[0].AxisX.MajorGrid.Enabled = false;
                chart1.ChartAreas[0].AxisY.MajorGrid.Enabled = false;
                chart1.ChartAreas[0].AxisY2.MajorGrid.Enabled = false;
                chart1.Series[0].Points.DataBindXY(t, dx_l); chart1.Series[0].ToolTip = "(#VALX,#VALY)";
                chart1.Series[1].Points.DataBindXY(t, dy_l); chart1.Series[1].ToolTip = "(#VALX,#VALY)";
                chart1.Series[2].Points.DataBindXY(t, dz_l); chart1.Series[2].ToolTip = "(#VALX,#VALY)";
                chart1.Series[3].Points.DataBindXY(t, dc_l); chart1.Series[3].ToolTip = "(#VALX,#VALY)";

            }
            
            if(Table2.Rows.Count!=0)
            {
                cols = Convert.ToInt32(Table2.Rows[0]["卫星数"]);
                LiYuan = Table.Rows.Count / cols;
                for (int i = 0; i < Table2.Rows.Count; i++)
                {
                    Table2.Rows[i]["dX"] = Convert.ToDouble(Table2.Rows[i]["X"]) - Convert.ToDouble(Table1.Rows[i]["X"]);
                    Table2.Rows[i]["dY"] = Convert.ToDouble(Table2.Rows[i]["Y"]) - Convert.ToDouble(Table1.Rows[i ]["Y"]);
                    Table2.Rows[i]["dZ"] = Convert.ToDouble(Table2.Rows[i]["Z"]) - Convert.ToDouble(Table1.Rows[i]["Z"]);
                    Table2.Rows[i]["dC"] = Convert.ToDouble(Table2.Rows[i]["Clock"]) - Convert.ToDouble(Table1.Rows[i]["Clock"]);
                }
                dx_l = new double[LiYuan * 2];
                dy_l = new double[LiYuan * 2];
                dz_l = new double[LiYuan * 2];
                dc_l = new double[LiYuan * 2];
                t = new int[LiYuan * 2];
                PRN = new string[cols];
                for (int j = 0; j < cols; j++)
                {
                    PRN[j] = Convert.ToString(Table.Rows[j]["PRN"]);
                }
                for (int i = 0; i < LiYuan * 2; i++)
                {
                    t[i] = INTEVAL/2 * i;
                    for (int j = 0; j < cols; j++)
                    {
                        if (toolStripComboBox3.Text == PRN[j])
                        {
                            dx_l[i] = Convert.ToDouble(Table2.Rows[j + cols * i]["dX"]);
                            dy_l[i] = Convert.ToDouble(Table2.Rows[j + cols * i]["dY"]);
                            dz_l[i] = Convert.ToDouble(Table2.Rows[j + cols * i]["dZ"]);
                            dc_l[i] = Convert.ToDouble(Table2.Rows[j + cols * i]["dC"]);
                        }
                    }
                }
                chart1.ChartAreas[1].AxisX.Title = "t(s)";
                chart1.ChartAreas[1].AxisY.Title = "(m)";
                chart1.ChartAreas[1].AxisX.MajorGrid.Enabled = false;
                chart1.ChartAreas[1].AxisY.MajorGrid.Enabled = false;
                chart1.ChartAreas[1].AxisY2.MajorGrid.Enabled = false;
                chart1.Series[4].Points.DataBindXY(t, dx_l); chart1.Series[4].ToolTip = "(#VALX,#VALY)";
                chart1.Series[5].Points.DataBindXY(t, dy_l); chart1.Series[5].ToolTip = "(#VALX,#VALY)";
                chart1.Series[6].Points.DataBindXY(t, dz_l); chart1.Series[6].ToolTip = "(#VALX,#VALY)";
                chart1.Series[7].Points.DataBindXY(t, dc_l); chart1.Series[7].ToolTip = "(#VALX,#VALY)";
            }
            
        }

        private void toolStripComboBox4_TextUpdate(object sender, EventArgs e)
        {
            string[] PRN;
            int cols = Convert.ToInt32(Table.Rows[0]["卫星数"]);
            int LiYuan = Table.Rows.Count / cols;
            double[] x = new double[LiYuan * 2];
            double[] y = new double[LiYuan * 2];
            double[] z = new double[LiYuan * 2];
            double[] b = new double[LiYuan * 2];
            double[] l = new double[LiYuan * 2];
            double[] h = new double[LiYuan * 2];
            PRN = new string[cols];
            for (int j = 0; j < cols; j++)
            {
                PRN[j] = Convert.ToString(Table.Rows[j]["PRN"]);
            }

            if (Table2.Rows.Count != 0)
            {
                for (int i = 0; i < LiYuan * 2; i++)
                {
                    for (int j = 0; j < cols; j++)
                    {
                        if (toolStripComboBox4.Text == PRN[j])
                        {
                            x[i] = Convert.ToDouble(Table2.Rows[j + cols * (i)]["X"]) * 1000;
                            y[i] = Convert.ToDouble(Table2.Rows[j + cols * (i)]["Y"]) * 1000;
                            z[i] = Convert.ToDouble(Table2.Rows[j + cols * (i)]["Z"]) * 1000;
                            Function.XYZ_BLH(x[i], y[i], z[i], out b[i], out l[i], out h[i]);
                        }
                    }
                }
                chart2.ChartAreas[1].AxisX.Maximum = 180;
                chart2.ChartAreas[1].AxisX.Minimum = -180;
                chart2.ChartAreas[1].AxisY.Maximum = 90;
                chart2.ChartAreas[1].AxisY.Minimum = -90;
                chart2.Series[1].Points.DataBindXY(l, b);
            }

            if(Table1.Rows.Count!=0)
            {
                for (int i = 0; i < LiYuan * 2; i++)
                {
                    for (int j = 0; j < cols; j++)
                    {
                        if (toolStripComboBox4.Text == PRN[j])
                        {
                            x[i] = Convert.ToDouble(Table1.Rows[j + cols * (i)]["X"]) * 1000;
                            y[i] = Convert.ToDouble(Table1.Rows[j + cols * (i)]["Y"]) * 1000;
                            z[i] = Convert.ToDouble(Table1.Rows[j + cols * (i)]["Z"]) * 1000;
                            Function.XYZ_BLH(x[i], y[i], z[i], out b[i], out l[i], out h[i]);
                        }
                    }
                }
                chart2.ChartAreas[0].AxisX.Maximum = 180;
                chart2.ChartAreas[0].AxisX.Minimum = -180;
                chart2.ChartAreas[0].AxisY.Maximum = 90;
                chart2.ChartAreas[0].AxisY.Minimum = -90;
                chart2.Series[0].Points.DataBindXY(l, b);
            }
                
            if(Table.Rows.Count!=0)
            {
                x = new double[LiYuan];
                y = new double[LiYuan];
                z = new double[LiYuan];
                b = new double[LiYuan];
                l = new double[LiYuan];
                h = new double[LiYuan];
                for (int i = 0; i < LiYuan ; i++)
                {
                    for (int j = 0; j < cols; j++)
                    {
                        if (toolStripComboBox4.Text == PRN[j])
                        {
                            x[i] = Convert.ToDouble(Table.Rows[j + cols * (i)]["X"]) * 1000;
                            y[i] = Convert.ToDouble(Table.Rows[j + cols * (i)]["Y"]) * 1000;
                            z[i] = Convert.ToDouble(Table.Rows[j + cols * (i)]["Z"]) * 1000;
                            Function.XYZ_BLH(x[i], y[i], z[i], out b[i], out l[i], out h[i]);
                        }
                    }
                }
                chart2.ChartAreas[2].AxisX.Maximum = 180;
                chart2.ChartAreas[2].AxisX.Minimum = -180;
                chart2.ChartAreas[2].AxisY.Maximum = 90;
                chart2.ChartAreas[2].AxisY.Minimum = -90;
                chart2.Series[2].Points.DataBindXY(l, b);
            }

        }

        private void toolStripComboBox1_Click(object sender, EventArgs e)
        {

        }
    }
}
