using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace 卫星轨道计算
{
    public partial class Form3 : Form
    {
        public Form3()
        {
            InitializeComponent();
        }

        private void timer1_Tick_1(object sender, EventArgs e)
        {
            toolStripLabel4.Text = "系统时间：" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }

        private void Form3_Load(object sender, EventArgs e)
        {
            timer1.Start();
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            Form1 f = new Form1();
            f.Show();
            this.Hide();
            
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            Form4 f = new Form4();
            f.Show();
            this.Hide();
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            Form2 f = new Form2();
            f.Show();
            this.Hide();
        }

        private void toolStripButton5_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void Form3_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            Spp f = new Spp();
            f.Show();
            //this.Hide();
        }
    }
}
