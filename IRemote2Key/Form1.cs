using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Collections;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Ports;

namespace IRemote2Key
{
    public partial class Form1 : Form
    {
        private delegate void Delegate_sendKey(string s);
        private delegate void Delegate_SetText(string s);
        public Form1()
        {
            InitializeComponent();
        }

        ArrayList ircodes = new ArrayList();
        Dictionary<string, string> keys = new Dictionary<string, string>();
        bool isRegister = false;

        public ArrayList getIrcodes()
        {
            return ircodes;
        }

        public void configLoad()
        {
            keys = new Dictionary<string, string>();
            ircodes = new ArrayList();

            System.IO.StreamReader sr = new System.IO.StreamReader(@"ir.config",System.Text.Encoding.GetEncoding("utf-8"));
            
            while(sr.Peek() >= 0)
            {
                String s = sr.ReadLine();
                String[] array = s.Split(',');
                if (array.Length != 2) break;
                ircodes.Add(array[0]);
                keys.Add(array[0],array[1]);
            }

            sr.Close();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            configLoad();
            string[] PortList = SerialPort.GetPortNames();
            comboBox1.Items.Clear();

            foreach (string PortName in PortList)
                comboBox1.Items.Add(PortName);
            if (comboBox1.Items.Count > 0)
                comboBox1.SelectedIndex = 0;
        }
        
        private void button1_Click(object sender, EventArgs e)
        {
            if (serialPort1.IsOpen)
            {
                serialPort1.Close();
                button1.Text = "Connect";
            } else
            {
                serialPort1.PortName = comboBox1.SelectedItem.ToString();
                serialPort1.BaudRate = 9600;

                serialPort1.DataBits = 8;
                serialPort1.Parity = Parity.None;
                serialPort1.StopBits = StopBits.One;

                serialPort1.Handshake = Handshake.None;
                serialPort1.Encoding = Encoding.UTF8;

                try
                {
                    serialPort1.Open();
                    button1.Text = "Disconnect";
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }
        private void serialPort1_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            if (serialPort1.IsOpen)
            {
                string receivedData = serialPort1.ReadLine();
                if (!string.IsNullOrEmpty(receivedData))
                {
                    receivedData = receivedData.Replace("\r", "");
                    if (isRegister)
                        Invoke(new Delegate_SetText(SetText), new Object[] { receivedData });
                    else
                        foreach (string ircode in ircodes)
                        {
                            if (ircode == receivedData)
                                Invoke(new Delegate_sendKey(sendKey), new Object[] { keys[ircode] });
                        }
                        
                    Console.Write(receivedData);
                }
            }
        }

        private void SetText(string s)
        {
            f.SetText(s);
        }
        
        private void sendKey(string s)
        {
            SendKeys.Send(s);
        }

        public void childFormClosed()
        {
            isRegister = false;
            configLoad();
        }

        Form2 f;

        private void button2_Click(object sender, EventArgs e)
        {
            f = new Form2(this);
            f.Show(this);
            isRegister = true;
        }
    }
}