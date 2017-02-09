using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IRemote2Key
{
    public partial class Form2 : Form
    {
        public Form2(Form1 f)
        {
            parent = f;
            InitializeComponent();
        }

        string ircode;
        Form1 parent;

        public void SetText(string s)
        {
            ircode = s;
            label2.Text = s;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (ircode == "") {
                MessageBox.Show("赤外線コードの受信が出来ていません。もう一度試してください。もしかするとリモコンが非対応の可能性があります。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (textBox1.Text == "") {
                MessageBox.Show("キーが入力されていません。入力してからもう一度試してください。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                foreach (string s in parent.getIrcodes())
                {
                    if (s == ircode)
                    {
                        if (MessageBox.Show("既に登録されている赤外線コードです。上書きしますか？", "確認", MessageBoxButtons.YesNo,MessageBoxIcon.Information) == DialogResult.Yes)
                        {
                            System.IO.StreamReader sr = new System.IO.StreamReader(@"ir.config");
                            System.IO.StreamWriter sw = new System.IO.StreamWriter(@"ir.config.tmp");

                            while (sr.Peek() > -1)
                            {
                                string line = sr.ReadLine();
                                if (ircode == line.Split(',')[0])
                                {
                                    continue;
                                }
                                sw.WriteLine(line);
                            }
                            sr.Close();
                            sw.Close();

                            System.IO.File.Copy(@"ir.config.tmp", @"ir.config", true);
                            System.IO.File.Delete(@"ir.config.tmp");
                        }
                        else
                        {
                            return;
                        }
                    }
                }
                System.IO.StreamWriter writer = new System.IO.StreamWriter(@"ir.config", true, Encoding.GetEncoding("utf-8"));
                writer.WriteLine(ircode + "," + textBox1.Text);
                writer.Close();
                Close();
            }
        }

        private void ClosedForm(object sender, FormClosedEventArgs e)
        {
            parent.childFormClosed();
        }
    }
}
